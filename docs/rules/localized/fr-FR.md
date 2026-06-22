# France — `fr-FR` PII rules

> Common rules run via the invariant pack (French IBANs covered there). **VERIFY**: formats + NIR key.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Nir | `NationalId` | Critical | `[12]\d{14}` | **mod-97 key** | 0.95 (pass) | sécurité sociale, sensitive |
| Mobile | `Phone` | High | `(+33\|0)[67]…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | `\d{5}` | none | 0.2 | weak alone |

Test: `180067504812307`→NIR · `180067504812308`→none · `0612345678`→Mobile.
Open: Corsica (2A/2B) NIR not handled (numeric only). See [`../severity.md`](../severity.md).
