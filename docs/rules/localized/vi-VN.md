# Vietnam — `vi-VN` PII rules

> Common rules run via the invariant pack. CCCD has no check digit. **VERIFY**: formats.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Cccd | `NationalId` | Critical | `\d{12}` | none | 0.45 | region + DOB embedded |
| Mobile | `Phone` | High | `(+84\|0)[3-9]…` | none | 0.7 | |
| Passport | `Passport` | Critical | `[A-Z]\d{7}` | none | 0.4 | loose |

Test: `012345678901`→CCCD · `0987654321`→Mobile.
Open: old CMND (9-digit) too generic, deferred. See [`../severity.md`](../severity.md).
