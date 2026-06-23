# Colombia — `es-CO` PII rules
> Common rules via invariant pack. Cédula has no checksum. **VERIFY**: formats.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Cedula | `NationalId` | Critical | `\d{8,10}` | none | 0.35 |
| Mobile | `Phone` | High | `(+57\|0)3…` | none | 0.6 |

Test: `12345678`→Cédula · `+573001234567`→Mobile.
Note: a bare 10-digit starting 3 is ambiguous (cédula vs mobile); cédula wins on severity.
