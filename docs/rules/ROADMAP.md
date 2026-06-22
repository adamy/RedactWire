# Culture pack roadmap — main PII items per country

Goal: culture packs covering OECD + major emerging + large-population countries, enough
to reach **~80% of the world's internet users**. All packs ship in the core package.

> **Status: research — VERIFY before implementing.** The formats and checksum algorithms
> below are compiled from general knowledge. Each one MUST be confirmed against an
> authoritative source (government spec / standards body) before it becomes a rule, and
> every checksum needs synthetic valid/invalid/boundary test vectors (never real
> people's numbers). Accuracy is the product's entire value.

Legend: **★ = must-have** (the de-facto identifier(s) people actually use in that
country) · ✅ = has a validating checksum · ⚠ = highly sensitive · IBAN/Email/Card/IPv4
are already covered globally by the invariant pack.

---

## Coverage / sequencing

- **Tier 1** — top ~10 internet populations (~55% cumulative).
- **Tier 2** — next set, reaches **~80%**.
- **Tier 3** — remaining OECD + emerging (completeness beyond 80%).

Build order batches by **shared checksum family** (do all Luhn IDs together, all mod-11
together, all DOB-embedded IDs together) to amortise the shared-infra work.

---

## Tier 1

### China — `zh-CN`
- ★✅⚠ Resident Identity Card (身份证) — 18 chars, GB11643 mod-11 + embedded DOB.
- ★ Mobile — `1[3-9]\d{9}`.
- Landline — `0\d{2,3}-?\d{7,8}`.
- Passport — `[EGDSP]\d{8}` (format).
- Postal code — 6 digits (low conf).
- Unified Social Credit Code (org) — 18 char, mod-31 checksum.

### India — `en-IN` / `hi-IN`
- ★✅⚠ Aadhaar — 12 digits, Verhoeff.
- ★ PAN (tax) — `[A-Z]{5}\d{4}[A-Z]` (format; 5th char encodes holder type).
- ★ Mobile — `[6-9]\d{9}`.
- Voter ID (EPIC) — `[A-Z]{3}\d{7}`.
- Passport — `[A-Z]\d{7}`.
- GSTIN (business) — 15 char with checksum.
- Postal PIN — 6 digits.

### United States — `en-US` ✅ done
SSN, NANP phone, ZIP, passport, address. (DriverLicence still skipped — no common format.)

### Indonesia — `id-ID`
- ★ NIK (KTP) — 16 digits, region + DOB embedded (no check digit → validate DOB/region).
- NPWP (tax) — 15 digits.
- ★ Mobile — `08\d{8,11}` / `+62`.
- Passport — `[A-Z]\d{7}`.
- Postal — 5 digits.

### Brazil — `pt-BR`
- ★✅ CPF — 11 digits, 2 check digits.
- ★✅ CNPJ (business) — 14 digits, 2 check digits.
- ★ Mobile — `+55 (xx) 9xxxx-xxxx`.
- ✅ PIS/PASEP/NIS — 11 digits, checksum.
- CEP postal — `\d{5}-?\d{3}`.

### Russia — `ru-RU`
- ★✅ INN (tax) — 10 (legal) / 12 (individual) digits, checksum.
- ★✅ SNILS (pension) — 11 digits, checksum.
- Internal passport — `\d{4}\s?\d{6}`.
- ★ Mobile — `+7 9\d{9}`.

### Japan — `ja-JP`
- ★✅⚠ My Number (個人番号) — 12 digits, check digit.
- ★ Mobile — `0[789]0-?\d{4}-?\d{4}`.
- Passport — `[A-Z]{2}\d{7}`.
- Postal — `\d{3}-?\d{4}`.

### Nigeria — `en-NG`
- ★ NIN — 11 digits.
- ★ BVN (bank verification) — 11 digits.
- ★ Mobile — `0[789]\d{9}` / `+234`.
- Passport — `[A-Z]\d{8}`.

### Mexico — `es-MX`
- ★✅ CURP — 18 chars, DOB + sex + state + check digit.
- ★✅ RFC (tax) — 13 (person) / 12 (company) chars, homoclave check.
- ★ Mobile — `+52`.
- Postal — 5 digits.

### Germany — `de-DE`
- ★✅ Steuer-ID (tax) — 11 digits, check digit.
- ✅ Personalausweis (ID card) — 9 + check digits.
- Sozialversicherungsnummer — 12 chars.
- ★ Mobile — `01[5-7]\d` / `+49`; landline `+49`.
- PLZ postal — 5 digits.
- (IBAN via invariant.)

---

## Tier 2 (reaches ~80%)

### Pakistan — `ur-PK` / `en-PK`
- ★ CNIC — 13 digits `\d{5}-\d{7}-\d`. ★ Mobile — `03\d{9}` / `+92`. Passport.

### Philippines — `en-PH` / `fil-PH`
- ★ PhilSys PSN / national ID — 12 digits. TIN — 9–12 digits. SSS, PhilHealth.
  ★ Mobile — `09\d{9}` / `+63`.

### Vietnam — `vi-VN`
- ★ Citizen ID (CCCD) — 12 digits, region + DOB embedded. Old CMND — 9/12 digits.
  Tax code — 10/13 digits. ★ Mobile — `0[3-9]\d{8}` / `+84`.

### United Kingdom — `en-GB`
- ★ NINO — `[A-Z]{2}\d{6}[A-D]` (with prefix exclusions).
- ★✅⚠ NHS number — 10 digits, mod-11.
- ★ Postcode — UK alphanumeric format (distinctive, common).
- ★ Mobile — `07\d{9}` / `+44`.
- UTR (tax) — 10 digits. Passport — 9 digits.
- Driving licence — 16 chars, encodes surname/DOB (complex).

### Turkey — `tr-TR`
- ★✅ TC Kimlik No — 11 digits, checksum. VKN (tax) — 10 digits. ★ Mobile — `05\d{9}` / `+90`.

### France — `fr-FR`
- ★✅⚠ NIR / INSEE (sécurité sociale) — 15 digits, mod-97 + sex/DOB.
- SPI (tax) — 13 digits. ★ Mobile — `0[67]\d{8}` / `+33`. Postal — 5 digits. (IBAN invariant.)

### Egypt — `ar-EG`
- ★ National ID — 14 digits, century + DOB + governorate + check digit.
  ★ Mobile — `01[0-25]\d{8}` / `+20`. Passport — `[A-Z]\d{8}`.

### Iran — `fa-IR`
- ★✅ National ID (کد ملی) — 10 digits, checksum. ★ Mobile — `09\d{9}` / `+98`.

### Thailand — `th-TH`
- ★✅ National ID — 13 digits, checksum. ★ Mobile — `0[689]\d{8}` / `+66`.

### South Korea — `ko-KR`
- ★✅⚠ RRN (주민등록번호) — 13 digits, DOB + sex + check digit (highly sensitive).
- Foreigner registration no — same shape. ✅ Business reg no — 10 digits, checksum.
- ★ Mobile — `010-?\d{4}-?\d{4}`.

### Italy — `it-IT`
- ★✅ Codice Fiscale — 16 chars, name + DOB + check char.
- ✅ Partita IVA (VAT) — 11 digits, checksum. ★ Mobile — `3\d{8,9}` / `+39`. CAP — 5 digits.

### Spain — `es-ES`
- ★✅ DNI — 8 digits + letter (mod-23).
- ★✅ NIE — `[XYZ]\d{7}[A-Z]` (mod-23). CIF (company). ★ Mobile — `[67]\d{8}` / `+34`. Postal — 5.

### Bangladesh — `bn-BD`
- ★ NID — 10 / 13 / 17 digits. ★ Mobile — `01[3-9]\d{8}` / `+880`.

---

## Tier 3 (beyond 80% — completeness)

- **Canada `en-CA`/`fr-CA`** — ★✅ SIN (Luhn), postal `A1A 1A1`, NANP phone, passport.
- **Australia `en-AU`** — ★✅ TFN (checksum), ✅ Medicare (checksum), ABN 11-digit, `04` mobile.
- **New Zealand `en-NZ`** *(prototype exists)* — ★✅ IRD, **★ Driver Licence `AA\d{6}`** *(must — widely used as ID)*, NHI (health), phone, postcode.
- **Netherlands `nl-NL`** — ★✅ BSN (11-proef). **Poland `pl-PL`** — ★✅ PESEL (checksum + DOB), NIP.
- **Nordics (SE/NO/DK/FI)** — ★✅ personal numbers (DOB + Luhn/mod-11).
- **South Africa `en-ZA`** — ★✅ ID (Luhn + DOB). **Saudi Arabia `ar-SA`** — ★ National ID / Iqama 10-digit.
- **Argentina `es-AR`** — ★ DNI 7–8 digits, ✅ CUIT 11-digit. **Israel `he-IL`** — ★✅ Teudat Zehut (checksum).
- Remaining OECD: AT, BE, CH, CZ, GR, HU, IE, PT, CL, CO, EE, LT, LV, LU, SK, SI, IS.

---

## Shared infrastructure to build first (reused across packs)

Expand `Rules/Checksums.cs` — build + test each once, packs reuse:
- **Verhoeff** (IN Aadhaar).
- **Generic mod-11** (UK NHS, NL BSN, RU SNILS, IR, many national IDs).
- **mod-23** (ES DNI/NIE).
- **Generic mod-97** (FR NIR) — generalise the existing IBAN mod-97.
- **Luhn** — already present (CA SIN, ZA ID, AU variants).
- **DOB-embedded helper** — validate `yyMMdd`/`yyyyMMdd` inside an ID (CN, KR, ZA, MX, PESEL, Nordics, NIK, EG).
- **Bespoke**: IT Codice Fiscale, JP My Number, BR CPF/CNPJ, MX CURP/RFC, TR TC Kimlik.

## Per-pack checklist (v1 scope)

Phone + Mobile (split only where formats differ) · National ID(s) marked ★ · Tax ID if
distinct · Passport (format) · Postal (low conf) · **Driver licence only where it's a
de-facto ID** (e.g. NZ). Strong checksums wherever they exist. Each rule: positive +
negative + boundary tests with synthetic data; a `docs/rules/localized/<culture>.md` from
the template.
