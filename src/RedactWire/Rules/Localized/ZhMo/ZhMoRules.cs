// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ZhMo;

/// <summary>Macau (zh-MO) rule pack. See <c>docs/rules/localized/zh-MO.md</c>.
/// The BIR check digit is not publicly standardised, so the ID is format-only.
/// VERIFY: formats.</summary>
internal static class ZhMoRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Resident ID (BIR): 1/5/7 + 6 digits + (check digit). Format only.
        new RegexRule("ResidentId", PiiType.NationalId,
            @"(?<v>\b[1257]\d{6}\(\d\))",
            baseConfidence: 0.55),

        // Mobile: 8 digits starting 6 (optional +853).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?853\s?)?6\d{7}\b)",
            baseConfidence: 0.6),
    };
}
