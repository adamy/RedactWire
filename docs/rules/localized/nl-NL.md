# Netherlands — `nl-NL` PII rules

> Common rules run via the invariant pack (Dutch IBANs covered there). **VERIFY**: BSN 11-proef.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Bsn | `NationalId` | Critical | `\d{9}` | **11-proef** | 0.9 (pass) | |
| Mobile | `Phone` | High | `(+31\|0)6…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `1234 AB` | none | 0.5 | |

Test: `111222333`→BSN · `111222334`→none · `1234 AB`→Postcode.
See [`../severity.md`](../severity.md).
