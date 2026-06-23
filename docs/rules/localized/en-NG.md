# Nigeria — `en-NG` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats, incl. the NIN leading-digit assumption.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Nin | `NationalId` | Critical | `[1-9]\d{10}` (11 digits) | none | 0.4 | no public checksum; non-zero lead avoids phone collision. BVN shares shape. |
| Mobile | `Phone` | High | `(+234\|0)[789]\d{9}` | none | 0.7 | |
| Passport | `Passport` | Critical | `[A-Z]\d{8}` | none | 0.4 | loose |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Nin | `12345678901` | NationalId |
| Mobile | `08031234567` | Phone (not NationalId) |
| Mobile | `+2348031234567` | Phone |
| Passport | `A12345678` | Passport |

## Notes / gotchas

- NIN is 11 random digits with no checksum, and a local mobile number is also 11 digits —
  genuinely ambiguous without context. **Disambiguation heuristic:** a 0-leading 11-digit
  string is treated as a phone (mobiles start 0), so NIN matches only with a non-zero lead.
  Trade-off: misses the rare NIN that truly starts with 0, but never mislabels a phone.
- BVN shares the 11-digit shape. **Open:** dedicated BVN rule.
