// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.SlSi;

/// <summary>Slovenia (sl-SI) rule pack. See <c>docs/rules/localized/sl-SI.md</c>.
/// IBAN via the invariant pack. VERIFY: EMŠO check.</summary>
internal static class SlSiRules
{
    public static readonly IPiiRule[] Rules =
    {
        new RegexRule("Emso", PiiType.NationalId,
            @"(?<v>\b\d{13}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SloveniaEmso(v) ? (true, 0.95) : (false, 0)),

        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?386\s?|0)[3-7]\d{7}\b)",
            baseConfidence: 0.6),

        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
