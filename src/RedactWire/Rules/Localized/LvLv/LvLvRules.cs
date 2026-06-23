// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.LvLv;

/// <summary>Latvia (lv-LV) rule pack. New personal codes (starting 32) have no check
/// digit, so the personal code is format-only. See <c>docs/rules/localized/lv-LV.md</c>.
/// IBAN via the invariant pack. VERIFY.</summary>
internal static class LvLvRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Personal code: DDMMYY-XXXXX (11 digits). Format only.
        new RegexRule("PersonalCode", PiiType.NationalId,
            @"(?<v>\b\d{6}-?\d{5}\b)",
            baseConfidence: 0.4),

        // Mobile: (+371)? 2 + 7 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?371\s?)?2\d{7}\b)",
            baseConfidence: 0.6),

        // Postal code: LV-1234.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\bLV-?\d{4}\b)",
            baseConfidence: 0.3),
    };
}
