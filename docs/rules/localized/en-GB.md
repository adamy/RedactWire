# United Kingdom — `en-GB` PII rules

> Common rules run via the invariant pack (UK IBANs covered there). **VERIFY**: formats + NHS check digit.

Status: **draft**

| Rule | Type | Severity | Pattern (gist) | Checksum | Conf | Notes |
|---|---|---|---|---|---|---|
| Nino | `NationalId` | Critical | `AA######A` | none (prefix-filtered) | 0.6 | National Insurance |
| Nhs | `Custom` (NHS) | Critical | `\d{3} \d{3} \d{4}` | **mod-11** | 0.95 (pass) | health, sensitive |
| Mobile | `Phone` | High | `(+44\|0)7…` | none | 0.7 | |
| Postcode | `PostalCode` | Medium | UK alphanumeric | none | 0.5 | distinctive |

Test: `AB123456C`→NINO · `401 023 2137`→NHS · `401 023 2130`→none · `SW1A 1AA`→Postcode · `07700900123`→Mobile.
See [`../severity.md`](../severity.md).
