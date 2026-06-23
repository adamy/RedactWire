// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.PtPt;

/// <summary>Portugal (pt-PT) rule pack. See <c>docs/rules/localized/pt-PT.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + NIF check digit.</summary>
internal static class PtPtRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NIF (tax/ID): 9 digits, weighted mod-11 check digit.
        new RegexRule("Nif", PiiType.TaxId,
            @"(?<v>\b\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.PortugalNif(v) ? (true, 0.92) : (false, 0)),

        // Mobile: 9 + 8 digits (optional +351).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?351\s?)?9\d{8}\b)",
            baseConfidence: 0.7),

        // Postal code: 0000-000.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}-\d{3}\b)",
            baseConfidence: 0.4),
    };
}
