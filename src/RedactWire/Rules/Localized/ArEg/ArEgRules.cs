// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ArEg;

/// <summary>Egypt (ar-EG) rule pack. See <c>docs/rules/localized/ar-EG.md</c>.
/// VERIFY: formats + national-ID structure.</summary>
internal static class ArEgRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID: 14 digits, century + DOB + governorate embedded (date/gov sanity).
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b[23]\d{13}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.EgyptId(v) ? (true, 0.85) : (false, 0)),

        // Mobile: (+20 | 0) 1[0125] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?20|0)1[0125]\d{8}\b)",
            baseConfidence: 0.7),

        // Passport: a letter + 8 digits (loose).
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{8}\b)",
            baseConfidence: 0.4),
    };
}
