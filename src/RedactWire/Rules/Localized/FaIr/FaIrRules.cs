// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.FaIr;

/// <summary>Iran (fa-IR) rule pack. See <c>docs/rules/localized/fa-IR.md</c>.
/// VERIFY: formats + national-ID checksum.</summary>
internal static class FaIrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID (کد ملی): 10 digits, mod-11 check digit.
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b\d{10}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.IranId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+98 | 0) 9 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?98|0)9\d{9}\b)",
            baseConfidence: 0.7),
    };
}
