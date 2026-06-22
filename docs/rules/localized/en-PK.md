# Pakistan — `en-PK` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> CNIC has no public checksum. **VERIFY**: formats.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Cnic | `NationalId` | Critical | `00000-0000000-0` | none | 0.5 | 13 digits, distinctive dashed form |
| Mobile | `Phone` | High | `(+92\|0)3 + 9 digits` | none | 0.7 | |
| Passport | `Passport` | Critical | `[A-Z]{2}\d{7}` | none | 0.4 | loose |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Cnic | `42101-1234567-8` | NationalId |
| Mobile | `03001234567` | Phone |
| Mobile | `+923001234567` | Phone |
| Passport | `AB1234567` | Passport |

## Notes / gotchas

- No CNIC checksum → confidence is the only defence; the dashed 5-7-1 form keeps it specific.
