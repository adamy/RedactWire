// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EtEe;

/// <summary>Estonia (et-EE) rule pack. See <c>docs/rules/localized/et-EE.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + isikukood check.</summary>
internal static class EtEeRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Isikukood: 11 digits, two-pass weighted mod-11 check + embedded birth date.
        new RegexRule("Isikukood", PiiType.NationalId,
            @"(?<v>\b[1-8]\d{10}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.EstoniaIk(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+372)? 5 + 7-8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?372\s?)?5\d{6,7}\b)",
            baseConfidence: 0.6),
    };
}
