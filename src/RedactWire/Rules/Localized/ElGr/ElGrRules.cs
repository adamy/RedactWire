// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ElGr;

/// <summary>Greece (el-GR) rule pack. See <c>docs/rules/localized/el-GR.md</c>.
/// IBAN via the invariant pack. VERIFY: AFM/AMKA checks.</summary>
internal static class ElGrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // AMKA (social security): 11 digits, Luhn + birth date. Custom type with a name.
        new RegexRule("Amka", PiiType.Custom,
            @"(?<v>\b\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.GreeceAmka(v) ? (true, 0.92) : (false, 0),
            severity: PiiSeverity.Critical,
            subtype: "AMKA"),

        // AFM (tax): 9 digits, weighted mod-11 mod-10.
        new RegexRule("Afm", PiiType.TaxId,
            @"(?<v>\b\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.GreeceAfm(v) ? (true, 0.9) : (false, 0)),

        // Mobile: (+30)? 69 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?30\s?)?69\d{8}\b)",
            baseConfidence: 0.7),
    };
}
