# Saudi Arabia — `ar-SA` PII rules
> Common rules via invariant pack. **VERIFY**: national-ID Luhn.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `[12]\d{9}` | **Luhn** | 0.9 |
| Mobile | `Phone` | High | `(+966\|0)5…` | none | 0.7 |

Test: `1000000008`→ID · `1000000009`→none · `0501234567`→Mobile.
