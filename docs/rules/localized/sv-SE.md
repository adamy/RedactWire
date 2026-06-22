# Sweden — `sv-SE` PII rules

> Common rules run via the invariant pack (Swedish IBANs covered there). **VERIFY**: personnummer.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Personnummer | `NationalId` | Critical | `YYMMDD-NNNC` (10/12) | **Luhn + DOB** | 0.92 (pass) | |
| Mobile | `Phone` | High | `(+46\|0)7…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `NNN NN` | none | 0.2 | weak alone |

Test: `8112189876`→PNR · `8112189870`→none. See [`../severity.md`](../severity.md).
