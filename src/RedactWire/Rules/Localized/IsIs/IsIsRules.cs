// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.IsIs;

/// <summary>Iceland (is-IS) rule pack. See <c>docs/rules/localized/is-IS.md</c>.
/// VERIFY: formats + kennitala check.</summary>
internal static class IsIsRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Kennitala: 10 digits (DDMMYY-SSCN), weighted mod-11 check + embedded birth date.
        new RegexRule("Kennitala", PiiType.NationalId,
            @"(?<v>\b\d{6}-?\d{4}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.IcelandKennitala(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+354)? [6-8] + 6 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?354\s?)?[6-8]\d{6}\b)",
            baseConfidence: 0.6),
    };
}
