// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.HuHu;

/// <summary>Hungary (hu-HU) rule pack. See <c>docs/rules/localized/hu-HU.md</c>.
/// VERIFY: personal-ID check.</summary>
internal static class HuHuRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Personal identification number: 11 digits, weighted mod-11 check.
        new RegexRule("PersonalId", PiiType.NationalId,
            @"(?<v>\b\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.HungaryId(v) ? (true, 0.92) : (false, 0)),

        // Mobile: (+36 | 06) 20/30/70 + 7 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?36|06)(?:20|30|31|50|70)\d{7}\b)",
            baseConfidence: 0.7),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
