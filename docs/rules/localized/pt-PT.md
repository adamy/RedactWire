# Portugal — `pt-PT` PII rules
> Common rules via invariant pack (IBAN there). **VERIFY**: NIF check digit.

| Rule | Type | Severity | Pattern | Checksum | Conf |
|---|---|---|---|---|---|
| Nif | `TaxId` | Critical | `\d{9}` | **weighted mod-11** | 0.92 |
| Mobile | `Phone` | High | `9\d{8}` (+351) | none | 0.7 |
| Postcode | `PostalCode` | Medium | `0000-000` | none | 0.4 |

Test: `123456789`→NIF · `123456780`→none · `1234-567`→Postcode.
