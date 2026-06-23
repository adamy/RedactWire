# Chile — `es-CL` PII rules
> Common rules via invariant pack. **VERIFY**: RUT check digit.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Rut | `NationalId` | Critical | `12.345.678-K` | **mod-11 (0-9/K)** | 0.95 |
| Mobile | `Phone` | High | `(+56\|0)9…` | none | 0.7 |

Test: `12345678-5`→RUT · `12345678-6`→none · `0912345678`→Mobile.
