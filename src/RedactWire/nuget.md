# RedactWire

Lightweight, culture-aware **PII detection, validation and redaction** for .NET.
Regex-first and **checksum-validated** — a random 16-digit number isn't reported as a
credit card, and a national ID with a bad check digit is dropped, not guessed.

- Targets `netstandard2.0` (works on .NET Framework 4.6.1+, .NET Core/5–8+, Mono, Unity).
- Built-in packs for **50+ countries** (national IDs, tax numbers, phones, passports,
  postcodes), most gated by the real check-digit algorithm.
- **Secret detection** — API keys & tokens (OpenAI, AWS, GitHub, Stripe, Slack, Google,
  SendGrid, npm), JWT, PEM private keys — via `AddSecretDetection()`. Great for AI/LLM logs.
- Severity-driven overlap resolution; redaction (mask / remove / label / custom).
- Structured scanning of **JSON / XML / objects** is an opt-in add-on —
  [`RedactWire.Structured`](https://www.nuget.org/packages/RedactWire.Structured/) — so this
  core package carries no `System.Text.Json` dependency.

## Quick start

```csharp
using RedactWire;

// Zero-config static facade
PiiResult r   = Redactor.Detect("My SSN is 123-45-6789");
bool      hit = Redactor.HasPii("Call (415) 555-0132");
string    safe = Redactor.Redact("Card 4242 4242 4242 4242");

// Validate a single value
Redactor.Validate("123-45-6789", PiiType.SocialSecurity);   // Valid / Invalid / Unsupported

// Pick cultures via the builder
var detector = PiiDetectorBuilder.CreateDefault()
    .AddCulture(new CultureInfo("en-GB"))
    .Build();
```

```csharp
// ASP.NET Core / DI
builder.Services.AddRedactWire(b => b.AddCulture(new CultureInfo("de-DE")));
```

Packs resolve by **country**: a country's languages share one pack (`en-IN`/`hi-IN`,
`en-CA`/`fr-CA`), while the same language across countries stays distinct
(`zh-CN`/`zh-TW`/`zh-HK`/`zh-SG`).

## Links

- **Docs, full README, coverage list, and source:**
  https://github.com/adamy/RedactWire
- License: **Apache-2.0**

> Patterns/checksums are knowledge-based and being verified against authoritative
> sources — treat unverified packs as best-effort until confirmed.
