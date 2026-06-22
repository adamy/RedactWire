# Australia — `en-AU` PII rules

> Common rules run via the invariant pack. **VERIFY**: formats + TFN checksum.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Tfn | `TaxId` | Critical | `\d{3} \d{3} \d{3}` | **weighted mod-11** | 0.9 (pass) | tax file number |
| Mobile | `Phone` | High | `(+61\|0)4…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 | weak alone |

Test: `123456782`→TFN · `123456780`→none · `0412345678`→Mobile.
Open: Medicare and ABN (both have checksums) not yet added. See [`../severity.md`](../severity.md).
