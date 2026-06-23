// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.SkSk;

/// <summary>Slovakia (sk-SK) rule pack. Rodné číslo shares the Czech algorithm.
/// See <c>docs/rules/localized/sk-SK.md</c>. VERIFY.</summary>
internal static class SkSkRules
{
    public static readonly IPiiRule[] Rules =
    {
        new RegexRule("RodneCislo", PiiType.NationalId,
            @"(?<v>\b\d{6}/?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.CzechRc(v) ? (true, 0.92) : (false, 0)),

        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?421\s?)?9\d{8}\b)",
            baseConfidence: 0.6),

        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{3}\s?\d{2}\b)",
            baseConfidence: 0.2),
    };
}
