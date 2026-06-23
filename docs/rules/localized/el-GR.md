# Greece — `el-GR`
> IBAN via invariant. **VERIFY**: AFM/AMKA.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| Amka | Custom (AMKA) | **Luhn + DOB** | 0.92 |
| Afm | TaxId | **weighted mod-11 mod-10** | 0.9 |
| Mobile | Phone | none | 0.7 |

Test: `123456783`→AFM · `123456780`→none · `01019012341`→AMKA · `6912345678`→Mobile.
