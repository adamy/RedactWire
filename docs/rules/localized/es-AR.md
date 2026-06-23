# Argentina — `es-AR` PII rules
> Common rules via invariant pack. **VERIFY**: CUIT checksum.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Cuit | `TaxId` | Critical | `XX-XXXXXXXX-X` | **mod-11** | 0.95 |
| Dni | `NationalId` | Critical | `12.345.678` | none | 0.35 |
| Mobile | `Phone` | High | `(+54)…` | none | 0.5 |

Test: `20123456786`→CUIT · `20123456780`→none · `12.345.678`→DNI.
