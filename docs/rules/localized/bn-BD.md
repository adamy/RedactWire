# Bangladesh — `bn-BD` PII rules

> Common rules run via the invariant pack. NID has no public checksum. **VERIFY**: formats.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Nid | `NationalId` | Critical | `\d{10}` / `\d{13}` / `\d{17}` | none | 0.4 | smart-card vs old forms |
| Mobile | `Phone` | High | `(+880\|0)1[3-9]…` | none | 0.7 | |

Test: `1234567890` / `1234567890123` / `12345678901234567`→NID · `01712345678`→Mobile.
See [`../severity.md`](../severity.md).
