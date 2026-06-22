# Italy — `it-IT` PII rules

> Common rules run via the invariant pack (Italian IBANs covered there). **VERIFY**: Codice Fiscale check char.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| CodiceFiscale | `NationalId` | Critical | `AAAAAA00A00A000A` | **check character** | 0.97 (pass) | name + DOB encoded |
| Mobile | `Phone` | High | `(+39) 3NN NNNNNN(N)` | none | 0.6 | |
| Cap | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

Test: a Codice Fiscale with the correct trailing check char validates; a wrong one is dropped.
`+39 320 1234567`→Mobile.
See [`../severity.md`](../severity.md).
