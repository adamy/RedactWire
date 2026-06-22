// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnGb;

/// <summary>United Kingdom (en-GB) rule pack. See <c>docs/rules/localized/en-GB.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + NHS check digit.</summary>
internal static class EnGbRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National Insurance number: 2 letters + 6 digits + a suffix letter A-D.
        new RegexRule("Nino", PiiType.NationalId,
            @"(?<v>\b[A-CEGHJ-PR-TW-Z][A-CEGHJ-NPR-TW-Z]\d{6}[A-D]\b)",
            baseConfidence: 0.6),

        // NHS number: 10 digits, mod-11 check. Custom type with a name.
        new RegexRule("Nhs", PiiType.Custom,
            @"(?<v>\b\d{3}\s?\d{3}\s?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.NhsNumber(v) ? (true, 0.95) : (false, 0),
            severity: PiiSeverity.Critical,
            subtype: "NHS"),

        // Mobile: (+44 | 0) 7 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?44\s?|0)7\d{3}\s?\d{6}\b)",
            baseConfidence: 0.7),

        // Postcode: distinctive UK alphanumeric form.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}\b)",
            baseConfidence: 0.5),
    };
}
