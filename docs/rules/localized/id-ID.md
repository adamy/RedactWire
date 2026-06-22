# Indonesia — `id-ID` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats + NIK structure against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Nik | `NationalId` | Critical | `\d{16}` | DOB sanity (no check digit) | 0.85 (pass) | DDMMYY embedded, +40 day = female |
| Npwp | `TaxId` | Critical | `\d{15}` | none (format) | 0.3 | check digit not yet validated |
| Mobile | `Phone` | High | `08\d{8,11}` | none | 0.8 | |
| Passport | `Passport` | Critical | `[A-Z]\d{7}` | none | 0.4 | loose |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Nik | `3201010101900001` | NationalId, conf ~0.85 |
| Nik | `3201010113900001` | **no** match (month 13) |
| Mobile | `081234567890` | Phone |
| Npwp | `012345678901234` | TaxId |
| Postcode | `12345` | PostalCode (low conf) |

## Notes / gotchas

- NIK has no check digit; gated only by birth-date sanity → moderate confidence.
- **Open:** NPWP check digit; region-code validation for NIK.
