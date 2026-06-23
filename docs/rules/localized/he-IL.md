# Israel — `he-IL` PII rules
> Common rules via invariant pack. **VERIFY**: Teudat Zehut (Luhn over 9 digits).

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| TeudatZehut | `NationalId` | Critical | `\d{9}` | **Luhn** | 0.9 |
| Mobile | `Phone` | High | `(+972\|0)5…` | none | 0.7 |

Test: `123456782`→ID · `123456780`→none · `0501234567`→Mobile.
