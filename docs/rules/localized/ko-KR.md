# South Korea — `ko-KR` PII rules

> Common rules run via the invariant pack. RRN is highly sensitive. **VERIFY**: RRN checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Rrn | `NationalId` | Critical | `\d{6}-\d{7}` | **mod-11 + DOB** | 0.97 (pass) | 주민등록번호, sensitive |
| Mobile | `Phone` | High | `01[016789]-XXXX-XXXX` | none | 0.7 | |

Test: `900101-1234568`→RRN · `900101-1234560`→none · `010-1234-5678`→Mobile.
See [`../severity.md`](../severity.md).
