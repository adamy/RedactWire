// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.SvSe;

/// <summary>Sweden (sv-SE) rule pack. See <c>docs/rules/localized/sv-SE.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + personnummer.</summary>
internal static class SvSeRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Personnummer: 10 or 12 digits, Luhn + birth-date sanity.
        new RegexRule("Personnummer", PiiType.NationalId,
            @"(?<v>\b(?:\d{8}-?\d{4}|\d{6}-?\d{4})\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SwedenPersonnummer(v) ? (true, 0.92) : (false, 0)),

        // Mobile: (+46 | 0) 7 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?46|0)7\d{8}\b)",
            baseConfidence: 0.7),

        // Postal code: 3+2 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{3}\s?\d{2}\b)",
            baseConfidence: 0.2),
    };
}
