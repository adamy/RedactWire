# Germany — `de-DE` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack
> (German IBANs are covered there). **VERIFY**: formats + tax-ID check digit.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| TaxId | `TaxId` | Critical | `\d{11}` | **ISO 7064 MOD 11,10** | 0.95 (pass) | Steuer-ID |
| Mobile | `Phone` | High | `(+49\|0)1[567]…` | none | 0.7 | |
| Plz | `PostalCode` | Medium | `\d{5}` | none | 0.2 | |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| TaxId | `12345678903` | TaxId, conf ~0.95 |
| TaxId | `12345678900` | **no** match (check fail) |
| Mobile | `01511234567` | Phone |
| Plz | `10115` | PostalCode |

## Notes / gotchas

- Steuer-ID gated by ISO 7064 MOD 11,10 — a coincidental 11-digit phone usually fails it.
- **Open:** the IdNr "exactly one repeated digit" structural rule is not enforced.
