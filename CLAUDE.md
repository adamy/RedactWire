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
- Core library (`src/RedactWire`) targets **netstandard2.0** with **zero dependencies**.
- Library must be usable with no DI/host bootstrap (static `Redactor`) AND via
  `PiiDetectorBuilder` for configuration.
- Per-country rule packs are grouped by country: files under
  `Rules/Localized/<CountryCode>/` with namespace `RedactWire.Rules.Localized.<CountryCode>`
  (e.g. `EnUs/` → `RedactWire.Rules.Localized.EnUs`). Use the culture code with the hyphen
  removed and PascalCased (`en-US` → `EnUs`). Register the pack in `DefaultRules.ByCulture`.
- Rule docs live in `docs/rules/` (`common.md` + `localized/<culture>.md`); each rule has
  a Severity column. Severity model: `docs/rules/severity.md`.
- Detection rules implement `IPiiRule`; most are `RegexRule`. Checksums gate matches
  (a failed checksum drops the candidate, never emits a low-confidence match).
- Overlap resolution is severity-first, then the chosen `OverlapStrategy`.

## Testing
- Every rule needs a positive, negative, and boundary test (`tests/RedactWire.Tests`).
