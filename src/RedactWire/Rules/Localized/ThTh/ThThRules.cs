// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ThTh;

/// <summary>Thailand (th-TH) rule pack. See <c>docs/rules/localized/th-TH.md</c>.
/// VERIFY: formats + national-ID checksum.</summary>
internal static class ThThRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID: 13 digits, mod-11 check digit.
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b\d{13}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.ThailandId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+66 | 0) [689] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?66|0)[689]\d{8}\b)",
            baseConfidence: 0.7),
    };
}
