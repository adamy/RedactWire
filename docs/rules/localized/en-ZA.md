# South Africa — `en-ZA` PII rules

> Common rules run via the invariant pack. **VERIFY**: ID (Luhn + DOB).

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| NationalId | `NationalId` | Critical | `\d{13}` | **Luhn + DOB** | 0.92 (pass) | |
| Mobile | `Phone` | High | `(+27\|0)[6-8]…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 | weak alone |

Test: `9001015000085`→ID · `9001015000080`→none · `0821234567`→Mobile.
Note: a 13-digit Luhn-valid SA ID also matches the invariant CreditCard rule (both reported).
See [`../severity.md`](../severity.md).
