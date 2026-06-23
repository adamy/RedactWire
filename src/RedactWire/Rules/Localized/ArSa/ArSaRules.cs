// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ArSa;

/// <summary>Saudi Arabia (ar-SA) rule pack. See <c>docs/rules/localized/ar-SA.md</c>.
/// VERIFY: formats + national-ID Luhn.</summary>
internal static class ArSaRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID / Iqama: 10 digits, leading 1 (citizen) or 2 (resident), Luhn.
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b[12]\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Luhn(Checksums.Digits(v)) ? (true, 0.9) : (false, 0)),

        // Mobile: (+966 | 0) 5 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?966|0)5\d{8}\b)",
            baseConfidence: 0.7),
    };
}
