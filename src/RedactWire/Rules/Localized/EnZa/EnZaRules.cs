// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnZa;

/// <summary>South Africa (en-ZA) rule pack. See <c>docs/rules/localized/en-ZA.md</c>.
/// VERIFY: formats + ID (Luhn + DOB).</summary>
internal static class EnZaRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID: 13 digits, Luhn + embedded birth date.
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b\d{13}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SouthAfricaId(v) ? (true, 0.92) : (false, 0)),

        // Mobile: (+27 | 0) [6-8] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?27|0)[6-8]\d{8}\b)",
            baseConfidence: 0.7),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
