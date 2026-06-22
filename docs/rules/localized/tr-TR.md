# Turkey — `tr-TR` PII rules

> Common rules run via the invariant pack. **VERIFY**: formats + TC Kimlik checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| TcKimlik | `NationalId` | Critical | `[1-9]\d{10}` | **check digits** | 0.95 (pass) | non-zero leading |
| Mobile | `Phone` | High | `(+90\|0)5…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

Test: `10000000078`→TcKimlik · `10000000079`→none · `05001234567`→Mobile.
See [`../severity.md`](../severity.md).
