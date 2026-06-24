# RedactWire

[![NuGet](https://img.shields.io/nuget/v/RedactWire.svg)](https://www.nuget.org/packages/RedactWire/)
[![CI](https://github.com/adamy/RedactWire/actions/workflows/ci.yml/badge.svg)](https://github.com/adamy/RedactWire/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE)

A lightweight, culture-aware **PII detection, validation and redaction** library for .NET.

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
- **Built-in packs for 50+ countries** (see [Coverage](#coverage)) — national IDs, tax
  numbers, phones, passports, postcodes — most gated by the real check digit/algorithm.
- **Region-based resolution:** a country's languages share one pack (`en-IN`/`hi-IN`/`ta-IN`,
  `en-CA`/`fr-CA`, `de-CH`/`fr-CH`/`it-CH`, Singapore's four official languages), while the
  same language across different countries stays distinct (`zh-CN`/`zh-TW`/`zh-HK`/`zh-SG`).
- **Validation:** `Validate(value, [culture,] type)` → `Valid` / `Invalid` / `Unsupported`.
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

## Coverage

Invariant (every culture): **Email**, **Credit card**, **IPv4**, **IBAN**.

Built-in country packs (representative culture shown; all languages of a country resolve to
the same pack):

| Region | Packs |
|---|---|
| Americas | `en-US` `en-CA` `es-MX` `pt-BR` `es-AR` `es-CL` `es-CO` |
| Europe | `en-GB` `fr-FR` `de-DE` `it-IT` `es-ES` `pt-PT` `nl-NL` `nl-BE` `de-CH` `de-AT` `pl-PL` `cs-CZ` `sk-SK` `hu-HU` `el-GR` `en-IE` `sv-SE` `nb-NO` `da-DK` `fi-FI` `is-IS` `et-EE` `lt-LT` `lv-LV` `sl-SI` `lb-LU` `ru-RU` `tr-TR` |
| Asia-Pacific | `zh-CN` `zh-HK` `zh-TW` `zh-MO` `ja-JP` `ko-KR` `en-IN` `id-ID` `vi-VN` `th-TH` `en-PH` `en-PK` `bn-BD` `en-SG` `ms-MY` `en-AU` `en-NZ` |
| Middle East / Africa | `ar-SA` `ar-EG` `fa-IR` `he-IL` `en-NG` `en-ZA` |

`PiiDetectorBuilder.AvailableCultures` returns this list at runtime. Each pack is documented
under [`docs/rules/localized/`](docs/rules/localized/), with verification status in
[`docs/rules/VERIFICATION.md`](docs/rules/VERIFICATION.md).

**Identifiers (searchable, incl. local names):**
SSN · SIN · NINO · NHS · TFN · Medicare · IRD · NRIC · MyKad · Aadhaar · PAN · CPF · CNPJ ·
CURP · RFC · Steuer-ID · NIR · BSN · DNI/NIE · Codice Fiscale · NIF · PESEL · Personnummer ·
Fødselsnummer · HETU · CPR · RRN · My Number · HKID ·
身份证 · 居民身份证 · 身分證 · 香港身份證 · マイナンバー · 個人番号 · 주민등록번호 ·
ИНН · СНИЛС · บัตรประชาชน · ΑΦΜ · ΑΜΚΑ · आधार · CCCD.

> Patterns and checksums are being verified against authoritative sources (see the
> verification log). Treat unverified packs as best-effort until confirmed.

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

## Validation

Check whether a string is, in full, a valid PII item of a given type — same rules and
checksums as detection. Returns `Valid` / `Invalid` / `Unsupported` (the last kept
distinct so "no rule for this type/culture" isn't mistaken for "invalid").

```csharp
Redactor.Validate("123-45-6789", PiiType.SocialSecurity);            // Valid
Redactor.Validate("123-45-0000", PiiType.SocialSecurity);            // Invalid
Redactor.Validate("110101199001010015",
    new CultureInfo("zh-CN"), PiiType.NationalId);                   // Valid (GB11643)
Redactor.Validate("x", new CultureInfo("fr-FR"), PiiType.NationalId); // Unsupported

// PiiType.Custom needs a subtype:
detector.Validate("112-233-445 95", new CultureInfo("ru-RU"), PiiType.Custom, "SNILS");
```

The no-culture overload validates against the detector's configured cultures; pass a
`CultureInfo` to target one. Must be a full-string match (surrounding whitespace ignored).

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

### Custom PII types

`PiiType` is an enum (enums can't be extended). For a type that isn't in the list, use
`PiiType.Custom` and give it a real name via `RuleHit.Subtype` — the name flows into the
match and into redaction labels:

```csharp
yield return new RuleHit(value, start, length, 0.9,
    Severity: PiiSeverity.Critical, Subtype: "NhiNumber");
// match.Type == PiiType.Custom, match.Subtype == "NhiNumber"
// Redact(Label)  ->  "[NhiNumber]"
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
