# RedactWire — project conventions

## Comments & docs
- **All code comments must be written in English, always.** No exceptions, regardless
  of the language used in chat. This applies to inline comments, XML doc comments, and
  TODOs in source files. (Prose in `docs/*.md` may quote other languages where useful,
  but source-code comments are English-only.)

## License
- Project license is **Apache 2.0** (root `LICENSE`).
- Every `.cs` file starts with an SPDX header (two lines, then a blank line):
  ```
  // SPDX-License-Identifier: Apache-2.0
  // Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ
  ```
  New source files must include it.

## Naming
- Solution / projects / namespace = **RedactWire** (the `_research/` prototype uses the
  old name "PiiGuard" — translate, don't copy).
- The static facade type is `Redactor` (a type named `RedactWire` would clash with the
  namespace). Use `Redactor.Detect/HasPii/Redact` for zero-config calls.

## Architecture
- Core library (`src/RedactWire`) targets **netstandard2.0**; its only dependency is
  `Microsoft.Extensions.DependencyInjection.Abstractions` (contracts-only) so the DI
  bootstrap ships in core. **No `System.Text.Json`** — keep it that way.
- Structured scanning is a separate opt-in package, **`src/RedactWire.Structured`**
  (depends on core + `System.Text.Json`): `DetectJson` (System.Text.Json), `DetectXml`
  (System.Xml, **XXE-safe**: DtdProcessing.Prohibit + null resolver), `DetectObject`
  (reflection; cycle detection, depth cap, collections, skips framework types). All are
  extension methods on `PiiDetector` in namespace `RedactWire`; string values only. The
  static `Redactor` has NO structured forwarders — call `Redactor.Default.DetectJson(...)`.
- Three ways to use it:
  1. Static `Redactor` facade — zero bootstrap.
  2. `PiiDetectorBuilder` — manual configuration.
  3. DI: `services.AddRedactWire(b => b.AddCulture(...))` registers a singleton
     `PiiDetector` (extension in `ServiceCollectionExtensions.cs`, namespace
     `Microsoft.Extensions.DependencyInjection`).
- `samples/RedactWire.Sample.Web` — ASP.NET Core Razor Pages PII tester (uses the DI path).
- Per-country rule packs are grouped by country: files under
  `Rules/Localized/<CountryCode>/` with namespace `RedactWire.Rules.Localized.<CountryCode>`
  (e.g. `EnUs/` → `RedactWire.Rules.Localized.EnUs`). Use the culture code with the hyphen
  removed and PascalCased (`en-US` → `EnUs`). Register the pack in `DefaultRules.ByCulture`
  under a representative culture (e.g. `en-IN`).
- **Packs resolve by ISO-3166 region (country), not language** (`Regions.Of`, indexed in
  `DefaultRules.ByRegion`). So every culture of a country shares one pack — `en-IN/hi-IN/ta-IN`,
  `en-CA/fr-CA`, `de-CH/fr-CH/it-CH` all map to the same rules. No per-language registration
  or `AddIndia()`-style helpers needed. `AvailableCultures` still lists representative codes.
- Secret/credential detection (API keys, tokens, JWT, PEM private keys) lives in
  `Rules/Secrets/SecretRules.cs` — country-agnostic, so it registers as invariant rules via
  `PiiDetectorBuilder.AddSecretDetection()` (on by default in the static `Redactor`). Emits
  `PiiType.Secret` with the provider in `Subtype`. v1 is provider-prefixed high-precision;
  entropy/context secrets (AWS secret key, connection strings) are a later phase. Doc:
  `docs/rules/secrets.md`.
- Rule docs live in `docs/rules/` (`common.md` + `localized/<culture>.md`); each rule has
  a Severity column. Severity model: `docs/rules/severity.md`.
- Detection rules implement `IPiiRule`; most are `RegexRule`. A rule reports raw
  `RuleHit`s (value/start/length/confidence + optional severity); the **engine** stamps
  culture, rule id (`"culture:Name"`), and the severity default — rules never do that
  plumbing. Checksums gate matches (a failed checksum drops the candidate, never emits a
  low-confidence match).
- Builder rule registration: `AddRule(culture, rule)` (one culture), `AddRule(rule)`
  (all configured cultures, bound at `Build`), `AddInvariantRule(rule)` (always-on).
- Builder rule removal/override: `RemoveRule(type[, subtype])` / `RemoveRules(predicate)`
  drop built-in or custom rules; `ReplaceInvariantRule(rule)` / `ReplaceRule(culture, rule)`
  swap a rule by `Name`. `IPiiRule` exposes `Subtype` so removal can match type+subtype.
  Call these after the `Add*` that loaded the rules.
- `PiiType` is a fixed enum (enums aren't extensible). For consumer types that don't fit,
  use `PiiType.Custom` + `RuleHit.Subtype` (the name); it flows to `PiiMatch.Subtype` and
  redaction labels (`[Subtype ?? Type]`).
- Overlap resolution is severity-first, then the chosen `OverlapStrategy`.
- `PiiDetector.Validate(value, [culture,] type, subtype?)` reuses the rules/checksums to
  full-string-validate a value, returning `ValidationResult` (Valid/Invalid/Unsupported —
  Unsupported = no rule of that type for the culture(s)). No-culture overload uses the
  detector's configured cultures.

## Testing
- Every rule needs a positive, negative, and boundary test (`tests/RedactWire.Tests`).
