# Hong Kong — `zh-HK` PII rules
> Common rules via invariant pack. **VERIFY**: HKID check digit.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Hkid | `NationalId` | Critical | `A123456(3)` | **weighted mod-11** | 0.95 |
| Mobile | `Phone` | High | `[569]\d{7}` (+852) | none | 0.6 |

Test: `A123456(3)`→HKID · `A123456(4)`→none · `51234567`→Mobile.
