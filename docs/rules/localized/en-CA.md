# Canada — `en-CA` PII rules

> Common rules run via the invariant pack. **VERIFY**: formats + SIN (Luhn).

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Sin | `NationalId` | Critical | `\d{3}-\d{3}-\d{3}` | **Luhn** | 0.9 (pass) | |
| Phone | `Phone` | High | NANP | none | 0.6 | |
| Postcode | `PostalCode` | Medium | `A1A 1A1` | none | 0.5 | |

Test: `046454286`→SIN · `046454287`→none · `K1A 0B1`→Postcode.
See [`../severity.md`](../severity.md).
