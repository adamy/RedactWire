// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ViVn;

/// <summary>Vietnam (vi-VN) rule pack. See <c>docs/rules/localized/vi-VN.md</c>.
/// CCCD has structure but no check digit. VERIFY: formats.</summary>
internal static class ViVnRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Citizen ID (CCCD): 12 digits, region + DOB embedded. No check digit → format only.
        new RegexRule("Cccd", PiiType.NationalId,
            @"(?<v>\b\d{12}\b)",
            baseConfidence: 0.45),

        // Mobile: (+84 | 0) [3-9] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?84|0)[3-9]\d{8}\b)",
            baseConfidence: 0.7),

        // Passport: a letter + 7 digits (loose).
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{7}\b)",
            baseConfidence: 0.4),
    };
}
