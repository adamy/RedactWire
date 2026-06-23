# Taiwan — `zh-TW` PII rules
> Common rules via invariant pack. **VERIFY**: national-ID check digit.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `[A-Z]\d{9}` | **weighted mod-10** | 0.95 |
| Mobile | `Phone` | High | `09\d{8}` (+886) | none | 0.7 |

Test: `A123456789`→ID · `A123456788`→none · `0912345678`→Mobile.
