// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.CsCz;

/// <summary>Czechia (cs-CZ) rule pack. See <c>docs/rules/localized/cs-CZ.md</c>.
/// VERIFY: formats + rodné číslo.</summary>
internal static class CsCzRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Rodné číslo: 10 digits, divisible by 11 + embedded birth date.
        new RegexRule("RodneCislo", PiiType.NationalId,
            @"(?<v>\b\d{6}/?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.CzechRc(v) ? (true, 0.92) : (false, 0)),

        // Mobile: (+420)? [67] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?420\s?)?[67]\d{8}\b)",
            baseConfidence: 0.6),

        // Postal code: 3+2 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{3}\s?\d{2}\b)",
            baseConfidence: 0.2),
    };
}
