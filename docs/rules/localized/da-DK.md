# Denmark — `da-DK` PII rules

> Common rules run via the invariant pack (Danish IBANs covered there). **VERIFY**: CPR structure.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Cpr | `NationalId` | Critical | `DDMMYY-SSSS` | date sanity (mod-11 has exceptions) | 0.75 | |
| Mobile | `Phone` | High | `(+45)?\d{8}` | none | 0.55 | |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 | weak alone |

Test: `010101-1234`→CPR · `320101-1234`→none (bad day). See [`../severity.md`](../severity.md).
