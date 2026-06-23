// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.HeIl;

/// <summary>Israel (he-IL) rule pack. See <c>docs/rules/localized/he-IL.md</c>.
/// VERIFY: formats + Teudat Zehut (Luhn over 9 digits).</summary>
internal static class HeIlRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Teudat Zehut: 9 digits (leading zeros allowed), Luhn check.
        new RegexRule("TeudatZehut", PiiType.NationalId,
            @"(?<v>\b\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Luhn(Checksums.Digits(v)) ? (true, 0.9) : (false, 0)),

        // Mobile: (+972 | 0) 5 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?972|0)5\d{8}\b)",
            baseConfidence: 0.7),
    };
}
