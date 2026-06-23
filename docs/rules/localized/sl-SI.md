# Slovenia — `sl-SI`
> IBAN via invariant. **VERIFY**: EMŠO check.

| Rule | Type | Checksum | Conf |
|---|---|---|---|
| Emso | NationalId | **weighted mod-11** | 0.95 |
| Mobile | Phone | none | 0.6 |
| Postcode | PostalCode | none | 0.2 |

Test: `0101950500019`→ID · `0101950500010`→none.
