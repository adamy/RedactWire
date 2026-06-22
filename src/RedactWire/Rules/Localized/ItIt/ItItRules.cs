// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ItIt;

/// <summary>Italy (it-IT) rule pack. See <c>docs/rules/localized/it-IT.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + Codice Fiscale check char.</summary>
internal static class ItItRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Codice Fiscale: 16 chars, name + DOB encoded, check character.
        new RegexRule("CodiceFiscale", PiiType.NationalId,
            @"(?<v>\b[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.ItalyCodiceFiscale(v) ? (true, 0.97) : (false, 0)),

        // Mobile: optional +39, 3 + 8-9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?39\s?)?3\d{2}\s?\d{6,7})",
            baseConfidence: 0.6),

        // Postal code (CAP): 5 digits.
        new RegexRule("Cap", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
