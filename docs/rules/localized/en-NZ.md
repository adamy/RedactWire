# New Zealand — `en-NZ` PII rules

> Common rules run via the invariant pack. Driver licence is a de-facto ID here, so it
> is included (unlike US/UK). **VERIFY**: formats + IRD algorithm + NHI form.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Ird | `TaxId` | Critical | `\d{2,3}-\d{3}-\d{3}` | **IR weighted algorithm** | 0.9 (pass) | 8–9 digits |
| DriverLicence | `DriverLicense` | Critical | `[A-Z]{2}\d{6}` | none | 0.6 | **must-have**, widely used as ID |
| Nhi | `Custom` (NHI) | Critical | `LLLNNNC` or `LLLNNLX` (I/O excluded) | none yet | 0.55 | health; legacy + 2019 forms |
| Mobile | `Phone` | High | `(+64\|0)2…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 | weak alone |

Test: `49091850`→IRD · `49091851`→none · `AB123456`→DriverLicence · `ABC1234`→NHI.
Open: NHI check digit/char (mod-11 legacy, mod-23 new form) not validated — format only.
See [`../severity.md`](../severity.md).
