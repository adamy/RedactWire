// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.TrTr;

/// <summary>Turkey (tr-TR) rule pack. See <c>docs/rules/localized/tr-TR.md</c>.
/// VERIFY: formats + TC Kimlik checksum.</summary>
internal static class TrTrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // TC Kimlik No: 11 digits (non-zero leading), two check digits.
        new RegexRule("TcKimlik", PiiType.NationalId,
            @"(?<v>\b[1-9]\d{10}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.TurkeyId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+90 | 0) 5 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?90|0)5\d{9}\b)",
            baseConfidence: 0.7),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
