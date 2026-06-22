# Norway — `nb-NO` PII rules

> Common rules run via the invariant pack (Norwegian IBANs covered there). **VERIFY**: fødselsnummer.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Fodselsnummer | `NationalId` | Critical | `\d{11}` | **two control digits** | 0.95 (pass) | |
| Mobile | `Phone` | High | `(+47)?[49]…` | none | 0.6 | |
| Postcode | `PostalCode` | Medium | `\d{4}` | none | 0.2 | weak alone |

Test: `01010112377`→FNR · `01010112370`→none. See [`../severity.md`](../severity.md).
