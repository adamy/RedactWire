// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.NlNl;

/// <summary>Netherlands (nl-NL) rule pack. See <c>docs/rules/localized/nl-NL.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + BSN 11-proef.</summary>
internal static class NlNlRules
{
    public static readonly IPiiRule[] Rules =
    {
        // BSN: 9 digits, 11-proef.
        new RegexRule("Bsn", PiiType.NationalId,
            @"(?<v>\b\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.NetherlandsBsn(v) ? (true, 0.9) : (false, 0)),

        // Mobile: (+31 | 0) 6 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?31|0)6\d{8}\b)",
            baseConfidence: 0.7),

        // Postal code: 1234 AB.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\s?[A-Za-z]{2}\b)",
            baseConfidence: 0.5),
    };
}
