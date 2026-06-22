// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.JaJp;

/// <summary>Japan (ja-JP) rule pack. See <c>docs/rules/localized/ja-JP.md</c>.
/// VERIFY: formats + My Number check digit against authoritative sources.</summary>
internal static class JaJpRules
{
    public static readonly IPiiRule[] Rules =
    {
        // My Number (個人番号): 12 digits, mod-11 check digit.
        new RegexRule("MyNumber", PiiType.NationalId,
            @"(?<v>\b\d{12}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.JapanMyNumber(v) ? (true, 0.95) : (false, 0)),

        // Mobile: 070 / 080 / 090 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b0[789]0-?\d{4}-?\d{4}\b)",
            baseConfidence: 0.8),

        // Passport: 2 letters + 7 digits.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]{2}\d{7}\b)",
            baseConfidence: 0.4),

        // Postal code: 000-0000.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{3}-?\d{4}\b)",
            baseConfidence: 0.3),
    };
}
