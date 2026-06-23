// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.NlBe;

/// <summary>Belgium (nl-BE) rule pack. See <c>docs/rules/localized/nl-BE.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + national-number check.</summary>
internal static class NlBeRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National register number: 11 digits (YY.MM.DD-SSS.CC), mod-97 check.
        new RegexRule("NationalNumber", PiiType.NationalId,
            @"(?<v>\b\d{2}\.?\d{2}\.?\d{2}-?\d{3}\.?\d{2}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.BelgiumNn(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+32 | 0) 4 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?32|0)4\d{8}\b)",
            baseConfidence: 0.7),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
