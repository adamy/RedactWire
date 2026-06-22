// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.DeDe;

/// <summary>Germany (de-DE) rule pack. See <c>docs/rules/localized/de-DE.md</c>.
/// IBAN is covered by the invariant pack. VERIFY: formats + tax-ID check digit.</summary>
internal static class DeDeRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Tax ID (Steuer-ID): 11 digits, ISO 7064 MOD 11,10 check digit.
        new RegexRule("TaxId", PiiType.TaxId,
            @"(?<v>\b\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.GermanyTaxId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: 015x/016x/017x (or +49), then 7-9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+49|0)1[567]\d{7,9}\b)",
            baseConfidence: 0.7),

        // Postal code (PLZ): 5 digits.
        new RegexRule("Plz", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
