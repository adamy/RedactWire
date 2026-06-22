# Russia — `ru-RU` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats + INN/SNILS checksums against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Inn | `TaxId` | Critical | `\d{10}` or `\d{12}` | **mod-11** | 0.95 (pass) | 10 = entity, 12 = individual |
| Snils | `Custom` (SNILS) | Critical | `000-000-000 00` | **control number** | 0.95 (pass) | insurance number |
| Mobile | `Phone` | High | `(+7\|8)9XXXXXXXXX` | none | 0.7 | |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Inn (entity) | `7707083893` | TaxId |
| Inn (individual) | `500100732259` | TaxId |
| Inn | `7707083890` | **no** match (check fail) |
| Snils | `112-233-445 95` | Custom (SNILS) |
| Snils | `11223344594` | **no** match (control fail) |
| Mobile | `+79161234567` | Phone |

## Notes / gotchas

- SNILS uses `PiiType.Custom` + `Subtype="SNILS"` (no dedicated enum value); redaction
  labels it `[SNILS]`.
- **Open:** internal passport (`\d{4}\s?\d{6}`) is FP-prone; deferred.
