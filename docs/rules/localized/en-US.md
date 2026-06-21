# United States — `en-US` PII rules

> Per-country pack. Runs only when `en-US` is a requested/default culture.
> Common rules (Email, CreditCard, IPv4, IBAN) run separately via the invariant pack.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| SSN | `SocialSecurity` | Critical | `###-##-####` (also spaces); area ≠ 000/666/900-999, group ≠ 00, serial ≠ 0000 | none exists | 0.85 | SSN has no public check digit; pattern constraints are the only defense |
| Phone | `Phone` | High | NANP: optional `+1`, area & exchange `[2-9]xx`, `[2-9]xx xxxx` | none | 0.7 | **US has NO mobile/landline format split** — single rule |
| ZIP | `PostalCode` | Medium | `#####` or `#####-####` | none | 0.25 | weak alone; easily a false positive |
| Passport | `Passport` | Critical | 9 digits, or 1 letter + 8 digits | none | 0.4 | loose |
| Address | `Address` | High | composite (street line + city/state/ZIP), see below | none | ~0.4–0.7 | heuristic; extensible via `IPiiRule` |
| ~~DriverLicense~~ | — | — | — | — | — | **Skipped v1** — 50 states, no common format. Add per-state later. |

Severity = primary overlap-resolution key (higher wins, even at lower confidence).
See [`../severity.md`](../severity.md).

### Address (composite)

Not a single `RegexRule` — implements `IPiiRule` directly, merges parts into one span.

- **Street line:** house number + name + suffix — `123 Main St`, `4567 N Oak Avenue`.
  Suffix set: St/Street, Ave/Avenue, Blvd/Boulevard, Rd/Road, Ln/Lane, Dr/Drive,
  Ct/Court, Way, Pl/Place, Ter/Terrace, Cir/Circle (extend later).
- **City, State ZIP:** `Springfield, IL 62704` — city words, **2-letter state validated
  against the real 50-state set** (+ DC/territories — open), ZIP `#####(-####)?`.
- Confidence: full street + city/state/ZIP scores higher than a bare street fragment.

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| SSN | `My SSN is 123-45-6789` | SocialSecurity, conf ~0.85 |
| SSN | `000-12-3456` | **no** match (area 000) |
| SSN | `666-12-3456` | **no** match (area 666) |
| SSN | `123-00-6789` | **no** match (group 00) |
| Phone | `Call (415) 555-0132` | Phone |
| Phone | `+1 415 555 0132` | Phone |
| Phone | `123 456 7890` | **no** match (area starts 1) |
| ZIP | `Reno, NV 89501` | PostalCode (+ Address) |
| Passport | `Passport 123456789` | Passport, low conf |
| Address | `123 Main St, Springfield, IL 62704` | Address (full), higher conf |
| Address | `4567 N Oak Avenue` | Address (street only), lower conf |
| Address | `123 Main St, Springfield, ZZ 62704` | street part only (ZZ not a state) |

## Notes / gotchas

- No checksums anywhere in the US set (SSN/phone/ZIP/passport have none) → confidence
  calibration is the ONLY false-positive defense. SSN pattern strong; ZIP weak.
- ZIP and Address city/state/ZIP overlap — overlap strategy resolves (prefer Address span).
- **Open:** validate state codes against full 50-state list; decide DC/territories.
- **Open:** ITIN / EIN if needed later.
