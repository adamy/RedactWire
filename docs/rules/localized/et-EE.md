# Estonia — `et-EE`
> Invariant pack covers IBAN/email/card. **VERIFY**.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| Isikukood | NationalId | **two-pass mod-11 + DOB** | 0.95 |
| Mobile | Phone | none | 0.6 |

Test: `38501171232`→ID · `38501171230`→none.
