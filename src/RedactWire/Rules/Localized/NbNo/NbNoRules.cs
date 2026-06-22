// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.NbNo;

/// <summary>Norway (nb-NO) rule pack. See <c>docs/rules/localized/nb-NO.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + fødselsnummer.</summary>
internal static class NbNoRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Fødselsnummer: 11 digits, two control digits.
        new RegexRule("Fodselsnummer", PiiType.NationalId,
            @"(?<v>\b\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.NorwayFnr(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+47)? 4 or 9 + 7 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?47)?[49]\d{7}\b)",
            baseConfidence: 0.6),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
