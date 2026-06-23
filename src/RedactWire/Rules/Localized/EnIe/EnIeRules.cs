// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnIe;

/// <summary>Ireland (en-IE) rule pack. See <c>docs/rules/localized/en-IE.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + PPS check letter.</summary>
internal static class EnIeRules
{
    public static readonly IPiiRule[] Rules =
    {
        // PPS number: 7 digits + check letter (+ optional 2nd letter), mod-23.
        new RegexRule("Pps", PiiType.NationalId,
            @"(?<v>\b\d{7}[A-Wa-w][A-Wa-w]?\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.IrelandPps(v) ? (true, 0.92) : (false, 0)),

        // Mobile: (+353 | 0) 8[3-9] + 7 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?353\s?|0)8[3-9]\d{7}\b)",
            baseConfidence: 0.7),
    };
}
