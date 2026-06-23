// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.DeAt;

/// <summary>Austria (de-AT) rule pack. Austria has no single public national-ID number
/// (sector-specific PINs), so this pack covers mobile + postcode. IBAN via invariant.
/// See <c>docs/rules/localized/de-AT.md</c>. VERIFY.</summary>
internal static class DeAtRules
{
    public static readonly IPiiRule[] Rules =
    {
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?43|0)6[5-9]\d{6,11}\b)",
            baseConfidence: 0.6),

        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
