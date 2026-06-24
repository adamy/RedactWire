# Malaysia — `ms-MY`
> All Malaysian languages (ms/en/zh/ta-MY) resolve here by region. **VERIFY**.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| MyKad | NationalId | Critical | `YYMMDD-PB-####` | DOB + PB-code sanity (no check digit) | 0.85 |
| Mobile | Phone | High | `(+60\|0)1…` | none | 0.6 |
| Postcode | PostalCode | Medium | `\d{5}` | none | 0.2 |

Test: `900101011234`→MyKad · `901301011234`→none (month 13).
