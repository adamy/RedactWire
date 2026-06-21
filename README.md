# RedactWire

A lightweight, culture-aware **PII detection and redaction** library for .NET.

RedactWire is regex-first and checksum-validated: it doesn't just match patterns, it
*verifies* them (Luhn for cards, mod-97 for IBAN, area/group rules for US SSNs), so a
random 16-digit number isn't reported as a credit card. Rules are grouped per culture,
results tell you which checks ran and which passed, and every match carries a severity
that drives overlap resolution.

- **Target:** `netstandard2.0` — works on .NET Framework 4.6.1+, .NET Core, .NET 5–8+,
  Mono, Unity, Xamarin.
- **Dependencies:** one, contracts-only (`Microsoft.Extensions.DependencyInjection.Abstractions`),
  so the DI bootstrap ships in the box. No other runtime dependencies.
- **License:** Apache 2.0.

## Features

- Invariant (country-agnostic) rules: **Email**, **Credit card** (Luhn), **IPv4**,
  **IBAN** (mod-97).
- Per-country packs (currently **en-US**): **SSN**, **NANP phone**, **ZIP**,
  **Passport**, and a composite **street address** rule with 50-state validation.
- **Checksum gating:** a failed checksum drops the candidate — it is not reported as
  low-confidence noise.
- **Severity model** (`Critical > High > Medium > Low`): the primary key for overlap
  resolution — a higher-severity match wins over an overlapping lower-severity one,
  even at lower confidence.
- **Structured scanning:** detect PII inside **JSON**, **XML**, and **object graphs**,
  with each hit located by JSONPath / XPath / property path. Standard libraries only
  (`System.Text.Json`, `System.Xml`, reflection); XML is parsed safely (no XXE).
- **Redaction:** mask (length-preserving), remove, type-label, or a custom replacement.
- **Honest "no PII":** a requested culture with no rule pack is flagged
  `Supported = false` instead of silently "passing".
- **Extensible:** add your own rules per culture without forking (implement `IPiiRule`,
  or just use `RegexRule`).

## Install

```bash
dotnet add package RedactWire
```

## Quick start

Three ways to use it — pick what fits your app.

### 1. Static facade (zero bootstrap)

```csharp
using RedactWire;

PiiResult r = Redactor.Detect("My SSN is 123-45-6789");
bool hasPii  = Redactor.HasPii("Call (415) 555-0132");
string clean = Redactor.Redact("Card 4242 4242 4242 4242");
```

### 2. Builder (manual configuration)

```csharp
using System.Globalization;
using RedactWire;

var detector = PiiDetectorBuilder.CreateDefault()      // invariant rules
    .AddCulture(new CultureInfo("en-US"))              // en-US pack
    .UseOverlapStrategy(OverlapStrategy.KeepHighestConfidence)
    .Build();

PiiResult result = detector.Detect("Email john@x.co.nz, SSN 123-45-6789");
foreach (var m in result.AllMatches)
    Console.WriteLine($"{m.Severity} {m.Type} '{m.Value}' conf={m.Confidence:0.00}");
```

### 3. Dependency injection (ASP.NET Core / generic host)

```csharp
// Program.cs
builder.Services.AddRedactWire(b => b.AddCulture(new CultureInfo("en-US")));

// then inject PiiDetector anywhere
public class MyService(PiiDetector detector) { /* detector.Detect(...) */ }
```

## Results

`Detect` returns a `PiiResult`:

- `Invariant` — `CulturePiiResult` of country-agnostic matches (always evaluated).
- `Cultures` — one `CulturePiiResult` per requested culture, each with:
  - `Matches`, `RulesEvaluated`
  - `Supported` — was a rule pack found for this culture?
  - `Passed` — `Supported && Matches.Count == 0`
- `HasPii`, `AllMatches` (flat view).

Each `PiiMatch` carries `Type, Value, Start, Length, Confidence, Rule, Culture, Severity`.

## Redaction

```csharp
var r = Redactor.Detect("SSN 123-45-6789");
r.Redact();                                                  // "SSN ***********"
r.Redact(new RedactionOptions { Mode = RedactionMode.Label }); // "SSN [SocialSecurity]"
r.Redact(new RedactionOptions { Custom = m => $"<{m.Type}>" });
```

## Structured scanning (JSON / XML / objects)

Scan structured data and get each match with its location. Available on `PiiDetector`
(and the static `Redactor`).

```csharp
foreach (var h in Redactor.DetectJson("""{"user":{"email":"a@b.com"},"ssn":"123-45-6789"}"""))
    Console.WriteLine($"{h.Path}: {h.Match.Type}");
// $.user.email: Email
// $.ssn: SocialSecurity

Redactor.DetectXml("<u email=\"a@b.com\"/>");   // -> /u/@email
Redactor.DetectObject(myPoco);                  // -> User.Contacts[0].Phone
```

Notes:
- Only **string** values are scanned (a number that lost its formatting isn't reliable PII).
- XML is parsed with DTD processing prohibited and no external resolver — **XXE-safe**.
- Object scanning walks public properties/fields, handles collections/dictionaries,
  detects cycles, caps depth, and does not recurse into framework types.
- `System.Text.Json` is built into modern .NET; on `netstandard2.0`/.NET Framework it
  comes as a NuGet dependency (the library multi-targets so net8+ stays clean).

## Extending

Add a custom rule without touching the library. Most needs are covered by `RegexRule`:

```csharp
var detector = PiiDetectorBuilder.CreateDefault()
    .AddCulture(new CultureInfo("en-US"))
    .AddRule(new CultureInfo("en-US"),                 // bind to one culture
        new RegexRule("EmployeeId", PiiType.NationalId,
            @"(?<v>\bEMP\d{6}\b)",
            baseConfidence: 0.8,
            severity: PiiSeverity.Critical))           // optional; defaults from the type
    .AddInvariantRule(myGlobalRule)                    // always-on, culture-agnostic
    .Build();
```

For anything beyond a single regex, implement `IPiiRule`. A rule only reports raw
`RuleHit`s — the engine stamps the culture, the rule id, and the severity default, so
there's no plumbing to get wrong:

```csharp
public sealed class MyRule : IPiiRule
{
    public string Name => "MyRule";
    public PiiType Type => PiiType.NationalId;

    public IEnumerable<RuleHit> Find(string text)
    {
        // ... locate a match ...
        yield return new RuleHit(value, start, length, confidence: 0.9);
        // (pass a PiiSeverity to override the type default)
    }
}

var detector = PiiDetectorBuilder.CreateDefault()
    .AddCulture(new CultureInfo("en-US"))
    .AddCulture(new CultureInfo("en-GB"))
    .AddRule(new MyRule())   // bind to every configured culture at once
    .Build();
```

## Scope & roadmap

- **Names and addresses by NER** are out of the core (regex floods false positives). The
  `Address` rule is a regex heuristic; full name detection is reserved for an optional
  `RedactWire.Ner` package (local ONNX, e.g. GLiNER) — kept separate to avoid forcing a
  large model + its license on every consumer.
- More country packs to come (UK, DE/FR, AU/JP/SG/IN, …).

## Projects

| Path | What |
|---|---|
| `src/RedactWire` | the library (+ DI bootstrap) |
| `src/RedactWire.Cli` | command-line scanner / redactor |
| `samples/RedactWire.Sample.Web` | ASP.NET Core Razor Pages PII tester |
| `tests/RedactWire.Tests` | xUnit tests |
| `docs/rules/` | per-rule documentation (`common.md`, `severity.md`, `localized/`) |

## CLI

```bash
echo "My SSN is 123-45-6789" | dotnet run --project src/RedactWire.Cli
dotnet run --project src/RedactWire.Cli -- --redact --text "Card 4242 4242 4242 4242"
```

Exit code is `1` when PII is found, `0` when clean — handy in CI.

## License

Apache License 2.0. See [LICENSE](LICENSE).
