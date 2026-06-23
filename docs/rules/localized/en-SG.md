# Singapore — `en-SG`

> Singapore has four official languages — `en-SG`, `zh-SG`, `ms-SG`, `ta-SG` — which all
> resolve to this pack by region. Common rules (Email, CreditCard, IPv4, IBAN) run via the
> invariant pack. **VERIFY**: formats + NRIC check letter.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf |
|---|---|---|---|---|---|
| Nric | `NationalId` | Critical | `[STFGM]\d{7}[A-Z]` | **weighted mod-11 + table** | 0.95 |
| Mobile | `Phone` | High | `[89]\d{7}` (+65) | none | 0.6 |
| Postcode | `PostalCode` | Medium | `\d{6}` | none | 0.2 |

Test: `S1234567D`→NRIC · `S1234567A`→none · `91234567`→Mobile.
See [`../severity.md`](../severity.md).
