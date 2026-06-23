# Czechia — `cs-CZ`
> Invariant pack covers IBAN/email/card. **VERIFY**.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| RodneCislo | NationalId | **÷11 + DOB** | 0.92 |
| Mobile | Phone | none | 0.6 |
| Postcode | PostalCode | none | 0.2 |

Test: `9001011239`→ID · `9001011238`→none.
