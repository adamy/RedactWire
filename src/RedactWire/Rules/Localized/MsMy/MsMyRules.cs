// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.MsMy;

/// <summary>Malaysia (ms-MY) rule pack. See <c>docs/rules/localized/ms-MY.md</c>.
/// MyKad has no check digit, so the birth date + place-of-birth code are sanity-checked.
/// VERIFY.</summary>
internal static class MsMyRules
{
    public static readonly IPiiRule[] Rules =
    {
        // MyKad (NRIC): 12 digits, YYMMDD-PB-#### .
        new RegexRule("MyKad", PiiType.NationalId,
            @"(?<v>\b\d{6}-?\d{2}-?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.MalaysiaMyKad(v) ? (true, 0.85) : (false, 0)),

        // Mobile: (+60 | 0) 1 + 8-9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?60|0)1\d{8,9}\b)",
            baseConfidence: 0.6),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
