# India — `en-IN` PII rules

> Per-country pack. Runs only when `en-IN` is a requested/default culture.
> Common rules (Email, CreditCard, IPv4, IBAN) run separately via the invariant pack.
> **VERIFY**: formats + Aadhaar Verhoeff checksum against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Aadhaar | `NationalId` | Critical | `[2-9]\d{11}` (12 digits) | **Verhoeff** | 0.97 (pass) | must not start 0/1; fail → dropped. Sensitive. |
| Pan | `TaxId` | Critical | `[A-Z]{5}\d{4}[A-Z]` | none (format) | 0.7 | no public checksum |
| Mobile | `Phone` | High | `[6-9]\d{9}` | none | 0.8 | optional +91 |
| Passport | `Passport` | Critical | `[A-Z]\d{7}` | none | 0.4 | loose |
| Pin | `PostalCode` | Medium | `\d{6}` | none | 0.2 | weak alone |

Severity = primary overlap-resolution key. See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Aadhaar | `234123412346` | NationalId, conf ~0.97 |
| Aadhaar | `234123412345` | **no** match (Verhoeff fail) |
| Pan | `ABCDE1234F` | TaxId |
| Mobile | `9876543210` | Phone |
| Mobile | `5876543210` | **no** match (starts 5) |
| Passport | `A1234567` | Passport |
| Pin | `560001` | PostalCode (low conf) |

## Notes / gotchas

- Aadhaar gated by Verhoeff: a wrong check digit drops the candidate.
- **Open:** Voter ID (EPIC `[A-Z]{3}\d{7}`), GSTIN (business, has checksum), `hi-IN`
  registration (same numeric formats; add as an alias when needed).
