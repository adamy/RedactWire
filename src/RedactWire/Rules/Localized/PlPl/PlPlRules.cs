// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.PlPl;

/// <summary>Poland (pl-PL) rule pack. See <c>docs/rules/localized/pl-PL.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + PESEL checksum.</summary>
internal static class PlPlRules
{
    public static readonly IPiiRule[] Rules =
    {
        // PESEL: 11 digits, weighted check digit + embedded birth date.
        new RegexRule("Pesel", PiiType.NationalId,
            @"(?<v>\b\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.PolandPesel(v) ? (true, 0.95) : (false, 0)),

        // Mobile: optional +48, 9 digits in 3-3-3 groups.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?48[\s-]?)?\d{3}[\s-]?\d{3}[\s-]?\d{3}\b)",
            baseConfidence: 0.5),

        // Postal code: 00-000.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{2}-\d{3}\b)",
            baseConfidence: 0.4),
    };
}
