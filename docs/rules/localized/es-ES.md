# Spain — `es-ES` PII rules

> Common rules run via the invariant pack (Spanish IBANs covered there). **VERIFY**: DNI/NIE letter.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Dni | `NationalId` | Critical | `\d{8}[A-Z]` | **mod-23** | 0.95 (pass) | |
| Nie | `NationalId` | Critical | `[XYZ]\d{7}[A-Z]` | **mod-23** | 0.95 (pass) | foreigners |
| Mobile | `Phone` | High | `(+34) [67]…` | none | 0.6 | |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

Test: `12345678Z`→DNI · `12345678A`→none · `X1234567L`→NIE · `612345678`→Mobile.
See [`../severity.md`](../severity.md).
