# Finland — `fi-FI` PII rules

> Common rules run via the invariant pack (Finnish IBANs covered there). **VERIFY**: HETU control char.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Hetu | `NationalId` | Critical | `DDMMYY±NNNC` | **mod-31 control char** | 0.95 (pass) | century sign |
| Mobile | `Phone` | High | `(+358\|0)4…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

Test: `010190-123M`→HETU · `010190-123A`→none. See [`../severity.md`](../severity.md).
