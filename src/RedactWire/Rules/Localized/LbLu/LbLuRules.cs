// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.LbLu;

/// <summary>Luxembourg (lb-LU) rule pack. The 13-digit matricule (Luhn + Verhoeff) is
/// format-only for now. See <c>docs/rules/localized/lb-LU.md</c>. IBAN via invariant.
/// VERIFY.</summary>
internal static class LbLuRules
{
    public static readonly IPiiRule[] Rules =
    {
        new RegexRule("Matricule", PiiType.NationalId,
            @"(?<v>\b\d{13}\b)",
            baseConfidence: 0.35),

        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?352\s?)?6[269]\d{6}\b)",
            baseConfidence: 0.6),
    };
}
