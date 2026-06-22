# Japan — `ja-JP` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats + My Number check digit against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| MyNumber | `NationalId` | Critical | `\d{12}` | **mod-11 check digit** | 0.95 (pass) | sensitive |
| Mobile | `Phone` | High | `0[789]0-NNNN-NNNN` | none | 0.8 | |
| Passport | `Passport` | Critical | `[A-Z]{2}\d{7}` | none | 0.4 | loose |
| Postcode | `PostalCode` | Medium | `000-0000` | none | 0.3 | |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| MyNumber | `123456789018` | NationalId, conf ~0.95 |
| MyNumber | `123456789010` | **no** match (check fail) |
| Mobile | `090-1234-5678` | Phone |
| Postcode | `100-0001` | PostalCode |

## Notes / gotchas

- My Number gated by its mod-11 check digit.
