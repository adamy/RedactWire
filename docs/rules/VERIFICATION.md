# Verification log

Tracks authoritative-source verification of each pack's formats and checksums. Until a
row is **Verified**, treat it as knowledge-based (the per-pack docs carry a VERIFY banner).

Status: `Verified` (checked against a cited source) · `Self-audited` (logic re-derived from
memory + synthetic test vector passes, no external source yet) · `Assumption` (a rule we
introduced without a known spec — must confirm).

> Note on method: web verification was attempted (rate-limited; small-model page extraction
> is unreliable and many Wikipedia pages omit the algorithm). Italy and Finland were
> confirmed by an exact table match. The remaining rows still need a careful pass against
> official country specifications.

Test vectors: every checksum has a synthetic valid sample in the test suite (round-trips
through the implemented algorithm), so a row that is only `Self-audited` still proves
internal consistency — it does **not** prove the algorithm matches the real-world spec.

## Checksums

| Item | Algorithm | Status | Source / note |
|---|---|---|---|
| Luhn (cards, CA SIN) | Luhn mod-10 | Self-audited | universal, textbook |
| IBAN | ISO 13616 mod-97 | Self-audited | universal |
| China Resident ID | GB 11643 mod-11 + DOB | **Verified** | Wikipedia "Resident Identity Card" — weights + check-map array match exactly |
| India Aadhaar | Verhoeff | **Verified** | Wikipedia "Verhoeff algorithm" — structure matches (i from right, p[i mod 8], collapse to 0) |
| Brazil CPF / CNPJ | mod-11 ×2 | Self-audited | |
| Russia INN / SNILS | mod-11 / control | Self-audited | |
| Japan My Number | weighted mod-11 | Self-audited | |
| Mexico CURP | weighted mod-10 (+Ñ) | Self-audited | |
| Germany Steuer-ID | ISO 7064 MOD 11,10 | Self-audited | |
| **Italy Codice Fiscale** | odd/even maps, sum mod-26 | **Verified** | Wikipedia "Italian fiscal code" — tables match exactly |
| **Finland HETU** | number mod-31 → char | **Verified** | Wikipedia "National identification number" — mod-31 over DDMMYYZZZ, same alphabet |
| Turkey TC Kimlik | two check digits | Self-audited | |
| France NIR | key = 97 − N mod 97 | **Verified** | Wikipedia "INSEE code" — key = 97 − (first 13 mod 97). Corsica (2A/2B) not handled |
| UK NHS | weighted mod-11 | Self-audited | |
| Spain DNI / NIE | mod-23 letter | Self-audited | letter table TRWAGMYFPDXBNJZSQVHLCKE is canonical; source page 404'd, re-fetch |
| Iran national ID | weighted mod-11 | Self-audited | |
| Thailand national ID | weighted mod-11 | Self-audited | |
| Korea RRN | weighted mod-11 + DOB | **Verified** | Wikipedia "Resident registration number" — weights 234567892345, [11−sum%11]%10 |
| Australia TFN | weighted /11 | Self-audited | |
| Netherlands BSN | 11-proef | Self-audited | |
| Poland PESEL | weighted mod-10 + DOB | **Verified** | Wikipedia "PESEL" — weights 1379137913, 10−sum%10, century month-offsets match exactly |
| NZ IRD | IR weighted + secondary | Self-audited | from prior prototype |
| Sweden personnummer | Luhn + DOB | Self-audited | |
| Norway fødselsnummer | two control digits | Self-audited | |
| Finland HETU | number mod-31 → char | Self-audited | |
| South Africa ID | Luhn + DOB | Self-audited | SA is Luhn (well documented); a Wikipedia extraction cross-contaminated it with China's weights — re-confirm |

## Date / structure only (no real check digit)

| Item | Basis | Status | Note |
|---|---|---|---|
| Indonesia NIK | DDMMYY (+40 female) | Self-audited | region codes not validated |
| Vietnam CCCD | format only | Self-audited | no check digit |
| Egypt national ID | century + DDMMYY + governorate | Self-audited | governorate set 01–35,88 needs confirm; "no check digit" claim to verify |
| Denmark CPR | DDMMYY | Self-audited | mod-11 has documented exceptions |

## Assumptions to confirm (introduced by us, no cited spec)

| Item | Assumption | Risk |
|---|---|---|
| **Nigeria NIN** | non-zero leading digit (to avoid mobile collision) | **High** — invented to disambiguate; confirm real NIN range |
| NZ NHI | legacy `AAA####` form only; new alphanumeric form + check digit not done | Medium |
| Egypt governorate | valid set = 01–35 plus 88 | Medium |
| Many mobile patterns | per-country prefixes/lengths | Medium — number plans change |
| Passport rules | loose `[A-Z]?\d+` shapes | Low (confidence already low) |

## Phone / postal / passport

Format-only by nature; lower confidence. Verify number plans (mobile prefixes) and postal
formats per country when doing the full pass.
