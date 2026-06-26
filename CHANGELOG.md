# Changelog

All notable changes to RedactWire. This project is pre-1.0 — breaking changes can land in
minor versions.

## 0.4.0

### ⚠️ Breaking

- **Structured scanning (JSON / XML / object) moved out of core** into a new opt-in package
  **`RedactWire.Structured`**. The core `RedactWire` package no longer depends on
  `System.Text.Json` — string-only consumers carry one less dependency.
- **`Redactor.DetectJson` / `DetectXml` / `DetectObject` removed.** Reference the
  `RedactWire.Structured` package and call them on a detector instead:
  ```csharp
  // before
  Redactor.DetectJson(json);
  // after  (after `dotnet add package RedactWire.Structured`)
  Redactor.Default.DetectJson(json);          // shared static detector
  detector.DetectJson(json, culture);          // or your own builder/detector
  ```
  `DetectJson/DetectXml/DetectObject` are extension methods on `PiiDetector` (namespace
  `RedactWire`), so they light up automatically once the add-on is referenced. Method
  signatures and detection behavior are unchanged — this is a packaging move only.

### Added

- **Secret detection** — `AddSecretDetection()` enables provider-prefixed API keys/tokens
  (OpenAI, AWS access-key-id, GitHub, Stripe, Slack, Google, SendGrid, npm), JWT and PEM
  private keys, as `PiiType.Secret` (provider in `Subtype`). On by default in `Redactor`.
- **Remove / replace built-in rules** — `RemoveRule(type[, subtype])`, `RemoveRules(predicate)`,
  `ReplaceInvariantRule(rule)`, `ReplaceRule(culture, rule)`. `IPiiRule` now exposes `Subtype`.
- **Region-based culture resolution** — a country's languages share one pack
  (`en-IN`/`hi-IN`, `en-CA`/`fr-CA`, Singapore's four languages); same language across
  countries stays distinct (`zh-CN`/`zh-TW`/`zh-HK`/`zh-SG`).
- More country packs (now 50+), incl. Malaysia (MyKad) and Australia Medicare.
