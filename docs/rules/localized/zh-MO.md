# Macau — `zh-MO` PII rules
> Common rules via invariant pack. BIR check digit not standardised → format only. **VERIFY**.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| ResidentId | `NationalId` | Critical | `[1257]\d{6}(\d)` | none | 0.55 |
| Mobile | `Phone` | High | `6\d{7}` (+853) | none | 0.6 |

Test: `1234567(8)`→ID · `61234567`→Mobile.
