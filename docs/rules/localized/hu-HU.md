# Hungary — `hu-HU`
> IBAN via invariant. **VERIFY**: personal-ID check.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| PersonalId | NationalId | **weighted mod-11** | 0.92 |
| Mobile | Phone | none | 0.7 |
| Postcode | PostalCode | none | 0.2 |

Test: `12345678919`→ID · `12345678910`→none · `+36201234567`→Mobile.
