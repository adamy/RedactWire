// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ZhTw;

/// <summary>Taiwan (zh-TW) rule pack. See <c>docs/rules/localized/zh-TW.md</c>.
/// VERIFY: formats + national-ID check digit.</summary>
internal static class ZhTwRules
{
    public static readonly IPiiRule[] Rules =
    {
        // National ID: 1 letter + 9 digits, weighted mod-10 check.
        new RegexRule("NationalId", PiiType.NationalId,
            @"(?<v>\b[A-Za-z]\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.TaiwanId(v) ? (true, 0.95) : (false, 0)),

        // Mobile: 09 + 8 digits (optional +886).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?886\s?)?09\d{8}\b)",
            baseConfidence: 0.7),
    };
}
