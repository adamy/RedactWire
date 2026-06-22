// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnCa;

/// <summary>Canada (en-CA) rule pack. See <c>docs/rules/localized/en-CA.md</c>.
/// VERIFY: formats + SIN (Luhn).</summary>
internal static class EnCaRules
{
    public static readonly IPiiRule[] Rules =
    {
        // SIN: 9 digits, Luhn.
        new RegexRule("Sin", PiiType.NationalId,
            @"(?<v>\b\d{3}-?\d{3}-?\d{3}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Luhn(Checksums.Digits(v)) ? (true, 0.9) : (false, 0)),

        // Phone (NANP): optional +1, area & exchange [2-9].
        new RegexRule("Phone", PiiType.Phone,
            @"(?<v>(?:\+?1[ .-]?)?\(?[2-9]\d{2}\)?[ .-]?[2-9]\d{2}[ .-]?\d{4})\b",
            baseConfidence: 0.6),

        // Postal code: A1A 1A1.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b[A-Za-z]\d[A-Za-z]\s?\d[A-Za-z]\d\b)",
            baseConfidence: 0.5),
    };
}
