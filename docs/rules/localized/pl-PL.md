# Poland — `pl-PL` PII rules

> Common rules run via the invariant pack (Polish IBANs covered there). **VERIFY**: PESEL checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Pesel | `NationalId` | Critical | `\d{11}` | **check digit + DOB** | 0.95 (pass) | month encodes century |
| Mobile | `Phone` | High | `(+48) NNN NNN NNN` | none | 0.5 | |
| Postcode | `PostalCode` | Medium | `00-000` | none | 0.4 | |

Test: `90010123459`→PESEL · `90010123450`→none · `00-001`→Postcode.
Open: NIP (tax) not yet added. See [`../severity.md`](../severity.md).
