# Philippines — `en-PH` PII rules

> Common rules run via the invariant pack. No public checksums. **VERIFY**: formats.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| PhilSys | `NationalId` | Critical | `0000-0000-0000` | none | 0.45 | 12 digits |
| Tin | `TaxId` | Critical | `000-000-000(-000)` | none | 0.45 | optional branch code |
| Mobile | `Phone` | High | `(+63\|0)9…` | none | 0.7 | |
| Passport | `Passport` | Critical | `[A-Z]\d{7}` | none | 0.4 | loose |

Test: `1234-5678-9012`→PhilSys · `123-456-789`→TIN · `09171234567`→Mobile.
Open: PhilSys/TIN have no checksum (format only). See [`../severity.md`](../severity.md).
