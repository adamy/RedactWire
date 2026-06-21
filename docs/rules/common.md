# Common — `invariant` PII rules

> Culture-agnostic. **Always run**, regardless of requested culture(s).
> These are formats that don't vary by country (or carry their own country code).

Status: **draft** (ported from prototype `_research/DefaultRules.cs`)

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Email | `Email` | High | `local@domain.tld`, tld ≥ 2 alpha | none | 0.95 | strong, low false-positive |
| CreditCard | `CreditCard` | Critical | 13–19 digits, space/dash separated | **Luhn** | 0.97 (pass) | fail → dropped, not low-conf |
| IPv4 | `IpAddress` | Medium | dotted quad, each octet 0–255 | none | 0.9 | IPv6 = open item |
| IBAN | `Iban` | Critical | 2 alpha + 2 check + 10–30 alnum | **mod-97** | 0.97 (pass) | fail → dropped |

Severity drives overlap resolution (higher wins, even at lower confidence). See
[`severity.md`](severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Email | `Contact me at john@x.co.nz` | Email, conf ~0.95 |
| CreditCard | `Card 4242 4242 4242 4242` | CreditCard (Luhn pass), conf ~0.97 |
| CreditCard | `Card 4242 4242 4242 4241` | **no** match (Luhn fail) |
| IPv4 | `192.168.1.1` | IpAddress |
| IPv4 | `999.1.1.1` | **no** match (octet > 255) |
| IBAN | valid IBAN | Iban (mod-97 pass), conf ~0.97 |
| IBAN | IBAN w/ tampered check digit | **no** match |

## Notes / gotchas

- Luhn/IBAN failures DROP the candidate (treated as "not PII"), never emitted as low-confidence.
- Regex compiled with `RegexOptions.Compiled | RegexOptions.CultureInvariant`.
- **Open:** IPv6 rule. **Open:** card-number detection inside long digit runs (boundary).
