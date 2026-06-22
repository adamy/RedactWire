// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnPk;

/// <summary>Pakistan (en-PK) rule pack. See <c>docs/rules/localized/en-PK.md</c>.
/// CNIC has no public checksum. VERIFY: formats.</summary>
internal static class EnPkRules
{
    public static readonly IPiiRule[] Rules =
    {
        // CNIC: 13 digits, usually 00000-0000000-0. No checksum → format only.
        new RegexRule("Cnic", PiiType.NationalId,
            @"(?<v>\b\d{5}-?\d{7}-?\d\b)",
            baseConfidence: 0.5),

        // Mobile: (+92 | 0) 3 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?92|0)3\d{9}\b)",
            baseConfidence: 0.7),

        // Passport: 2 letters + 7 digits (loose).
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]{2}\d{7}\b)",
            baseConfidence: 0.4),
    };
}
