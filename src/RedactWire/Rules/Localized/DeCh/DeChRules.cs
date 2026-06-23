// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.DeCh;

/// <summary>Switzerland (de-CH) rule pack. See <c>docs/rules/localized/de-CH.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + AHV EAN-13 check.</summary>
internal static class DeChRules
{
    public static readonly IPiiRule[] Rules =
    {
        // AHV/AVS social security: 756 + 9 digits, EAN-13 check digit.
        new RegexRule("Ahv", PiiType.NationalId,
            @"(?<v>\b756\.?\d{4}\.?\d{4}\.?\d{2}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Ean13(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+41 | 0) 7[5-9] + 7 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?41|0)7[5-9]\d{7}\b)",
            baseConfidence: 0.7),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
