// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnPh;

/// <summary>Philippines (en-PH) rule pack. See <c>docs/rules/localized/en-PH.md</c>.
/// No public checksums. VERIFY: formats.</summary>
internal static class EnPhRules
{
    public static readonly IPiiRule[] Rules =
    {
        // PhilSys PSN: 12 digits, usually 0000-0000-0000. No checksum → format only.
        new RegexRule("PhilSys", PiiType.NationalId,
            @"(?<v>\b\d{4}-?\d{4}-?\d{4}\b)",
            baseConfidence: 0.45),

        // TIN (tax): 000-000-000 with optional 3-5 digit branch code.
        new RegexRule("Tin", PiiType.TaxId,
            @"(?<v>\b\d{3}-\d{3}-\d{3}(?:-\d{3,5})?\b)",
            baseConfidence: 0.45),

        // Mobile: (+63 | 0) 9 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?63|0)9\d{9}\b)",
            baseConfidence: 0.7),

        // Passport: a letter + 7 digits (loose).
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{7}\b)",
            baseConfidence: 0.4),
    };
}
