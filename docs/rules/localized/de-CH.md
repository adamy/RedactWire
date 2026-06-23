# Switzerland — `de-CH` PII rules
> Common rules via invariant pack (IBAN there). **VERIFY**: AHV EAN-13 check.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Ahv | `NationalId` | Critical | `756.NNNN.NNNN.NN` | **EAN-13 mod-10** | 0.95 |
| Mobile | `Phone` | High | `(+41\|0)7[5-9]…` | none | 0.7 |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 |

Test: `7561234567897`→AHV · `7561234567890`→none · `0791234567`→Mobile.
