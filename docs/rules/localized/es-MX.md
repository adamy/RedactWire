# Mexico — `es-MX` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats + CURP check digit against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Curp | `NationalId` | Critical | 18 chars `AAAA######HAAAAAXD` | **mod-10 check digit** | 0.97 (pass) | DOB/sex/state embedded |
| Rfc | `TaxId` | Critical | 12–13 chars | none (format) | 0.6 | homoclave not validated |
| Mobile | `Phone` | High | `(+52) NNN NNN NNNN` | none | 0.6 | |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Curp | `GOMA800101HDFXYZ08` | NationalId, conf ~0.97 |
| Curp | `GOMA800101HDFXYZ09` | **no** match (check fail) |
| Rfc | `GODE561231GR8` | TaxId |
| Mobile | `+52 55 1234 5678` | Phone |

## Notes / gotchas

- CURP letters with Ñ are not matched (regex uses `[A-Z]`); rare edge, noted.
- RFC homoclave check digit not yet validated (format only).
