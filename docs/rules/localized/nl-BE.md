# Belgium — `nl-BE`
> Invariant pack covers IBAN/email/card. **VERIFY**.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| NationalNumber | NationalId | **mod-97** | 0.95 |
| Mobile | Phone | none | 0.7 |
| Postcode | PostalCode | none | 0.2 |

Test: `85011733277`→ID · `85011733278`→none · `0470123456`→Mobile.
