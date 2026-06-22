// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.FrFr;

/// <summary>France (fr-FR) rule pack. See <c>docs/rules/localized/fr-FR.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + NIR key.</summary>
internal static class FrFrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NIR (sécurité sociale): 15 digits starting 1/2, mod-97 key. Corsica (2A/2B) excluded.
        new RegexRule("Nir", PiiType.NationalId,
            @"(?<v>\b[12]\d{14}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.FranceNir(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+33 | 0) [67] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?33\s?|0)[67](?:\s?\d{2}){4}\b)",
            baseConfidence: 0.7),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
