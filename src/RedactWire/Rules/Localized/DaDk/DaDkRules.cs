// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.DaDk;

/// <summary>Denmark (da-DK) rule pack. See <c>docs/rules/localized/da-DK.md</c>.
/// IBAN via the invariant pack. CPR mod-11 has exceptions, so only the date is checked.
/// VERIFY: formats.</summary>
internal static class DaDkRules
{
    public static readonly IPiiRule[] Rules =
    {
        // CPR: 10 digits (DDMMYY-SSSS), birth-date sanity only.
        new RegexRule("Cpr", PiiType.NationalId,
            @"(?<v>\b\d{6}-?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.DenmarkCpr(v) ? (true, 0.75) : (false, 0)),

        // Mobile: (+45)? 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?45\s?)?[2-9]\d{7}\b)",
            baseConfidence: 0.55),

        // Postal code: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
