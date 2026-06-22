// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.BnBd;

/// <summary>Bangladesh (bn-BD) rule pack. See <c>docs/rules/localized/bn-BD.md</c>.
/// NID has no public checksum. VERIFY: formats.</summary>
internal static class BnBdRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NID: 10, 13, or 17 digits. No checksum → format only.
        new RegexRule("Nid", PiiType.NationalId,
            @"(?<v>\b(?:\d{17}|\d{13}|\d{10})\b)",
            baseConfidence: 0.4),

        // Mobile: (+880 | 0) 1[3-9] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?880|0)1[3-9]\d{8}\b)",
            baseConfidence: 0.7),
    };
}
