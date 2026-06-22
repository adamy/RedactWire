# Iran — `fa-IR` PII rules

> Common rules run via the invariant pack. **VERIFY**: national-ID checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `\d{10}` | **mod-11** | 0.95 (pass) | کد ملی |
| Mobile | `Phone` | High | `(+98\|0)9…` | none | 0.7 | |

Test: `1234567891`→ID · `1234567890`→none · `09123456789`→Mobile.
See [`../severity.md`](../severity.md).
