# Egypt — `ar-EG` PII rules

> Common rules run via the invariant pack. **VERIFY**: national-ID structure.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `[23]\d{13}` | date + governorate sanity | 0.85 | no reliable check digit |
| Mobile | `Phone` | High | `(+20\|0)1[0125]…` | none | 0.7 | |
| Passport | `Passport` | Critical | `[A-Z]\d{8}` | none | 0.4 | loose |

Test: `29001012112345`→ID · `29013012112345`→none (month 13) · `01012345678`→Mobile.
See [`../severity.md`](../severity.md).
