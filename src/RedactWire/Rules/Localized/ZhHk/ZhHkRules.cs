// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ZhHk;

/// <summary>Hong Kong (zh-HK) rule pack. See <c>docs/rules/localized/zh-HK.md</c>.
/// VERIFY: formats + HKID check digit.</summary>
internal static class ZhHkRules
{
    public static readonly IPiiRule[] Rules =
    {
        // HKID: 1-2 letters + 6 digits + check (digit or A), often in parentheses.
        new RegexRule("Hkid", PiiType.NationalId,
            @"(?<v>\b[A-Za-z]{1,2}\d{6}\(?[0-9Aa]\)?)",
            baseConfidence: 0.4,
            validate: v => Checksums.HongKongId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: 8 digits starting 5/6/9 (optional +852).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?852\s?)?[569]\d{7}\b)",
            baseConfidence: 0.6),
    };
}
