// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.FiFi;

/// <summary>Finland (fi-FI) rule pack. See <c>docs/rules/localized/fi-FI.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + HETU control char.</summary>
internal static class FiFiRules
{
    public static readonly IPiiRule[] Rules =
    {
        // HETU: DDMMYY + century sign + 3-digit individual + control char.
        new RegexRule("Hetu", PiiType.NationalId,
            @"(?<v>\b\d{6}[-+ABCDEFYXWVU]\d{3}[0-9A-Y]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.FinlandHetu(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+358 | 0) 4 + 6-8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?358|0)4\d{6,8}\b)",
            baseConfidence: 0.7),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
