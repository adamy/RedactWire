# Brazil — `pt-BR` PII rules

> Per-country pack. Common rules (Email, CreditCard, IPv4, IBAN) run via the invariant pack.
> **VERIFY**: formats + CPF/CNPJ checksums against authoritative sources.

Status: **draft**

## Rules

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Cpf | `NationalId` | Critical | `000.000.000-00` | **mod-11 ×2** | 0.97 (pass) | personal ID; all-equal rejected |
| Cnpj | `TaxId` | Critical | `00.000.000/0000-00` | **mod-11 ×2** | 0.97 (pass) | business |
| Mobile | `Phone` | High | `(+55) (NN) 9NNNN-NNNN` | none | 0.75 | |
| Cep | `PostalCode` | Medium | `00000-000` | none | 0.3 | |

See [`../severity.md`](../severity.md).

## Test vectors

| Rule | Input | Expect |
|---|---|---|
| Cpf | `111.444.777-35` | NationalId, conf ~0.97 |
| Cpf | `111.444.777-00` | **no** match (check fail) |
| Cnpj | `11.222.333/0001-81` | TaxId |
| Mobile | `+55 11 98765-4321` | Phone |
| Cep | `01310-200` | PostalCode |

## Notes / gotchas

- CPF/CNPJ gated by mod-11; all-identical-digit strings rejected even though they pass the math.
