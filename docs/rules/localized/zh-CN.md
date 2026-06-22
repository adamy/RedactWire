# China — `zh-CN` PII rules

> Per-country pack. Runs only when `zh-CN` is a requested/default culture.
> Common rules (Email, CreditCard, IPv4, IBAN) run separately via the invariant pack.
> **VERIFY**: formats + GB11643 checksum against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| ResidentId | `NationalId` | Critical | 18 chars, `\d{17}[\dX]` | **GB11643 mod-11 + DOB** | 0.97 (pass) | last char may be X; fail → dropped. Sensitive. |
| Mobile | `Phone` | High | `1[3-9]\d{9}` | none | 0.85 | mainland mobile |
| Passport | `Passport` | Critical | `[EGDSP]\d{8}` | none | 0.4 | ordinary passport, loose |
| Postcode | `PostalCode` | Medium | `\d{6}` | none | 0.2 | weak alone |

Severity = primary overlap-resolution key (higher wins, even at lower confidence).
See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| ResidentId | `身份证 110101199001010015` | NationalId, conf ~0.97 |
| ResidentId | `110101199001010014` | **no** match (check digit fail) |
| ResidentId | `22021982092811810` | **no** match (17 digits) |
| Mobile | `手机 13693993330` | Phone |
| Mobile | `12345678901` | **no** match (2nd digit not 3-9) |
| Passport | `E12345678` | Passport |
| Postcode | `100080` | PostalCode (low conf) |

## Notes / gotchas

- ResidentId checksum gates the match: a wrong check digit or an impossible birth date
  drops the candidate (never a low-confidence match).
- **Open:** landline (区号 + number) is FP-prone; deferred. UnionPay cards already covered
  by the invariant Luhn rule. Unified Social Credit Code (org) not yet added.
