# Thailand — `th-TH` PII rules

> Common rules run via the invariant pack. **VERIFY**: national-ID checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `\d{13}` | **mod-11** | 0.95 (pass) | |
| Mobile | `Phone` | High | `(+66\|0)[689]…` | none | 0.7 | |

Test: `1234567890121`→ID · `1234567890120`→none · `0812345678`→Mobile.
See [`../severity.md`](../severity.md).
