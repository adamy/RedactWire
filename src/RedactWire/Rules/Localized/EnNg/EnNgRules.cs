// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnNg;

/// <summary>Nigeria (en-NG) rule pack. See <c>docs/rules/localized/en-NG.md</c>.
/// NIN/BVN have no public checksum, so confidence is the only defence and NIN is
/// constrained to a non-zero leading digit to avoid colliding with mobile numbers.
/// VERIFY: formats (including the NIN leading-digit assumption).</summary>
internal static class EnNgRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NIN: 11 random digits, no checksum. NIN and a local mobile number are both 11
        // digits and are genuinely ambiguous without context. Disambiguation heuristic:
        // a 0-leading 11-digit string is treated as a phone (mobiles start 0), so NIN is
        // matched only with a non-zero leading digit. Trade-off: misses the rare NIN that
        // truly starts with 0, but never mislabels a phone as a NIN. (BVN shares the shape.)
        new RegexRule("Nin", PiiType.NationalId,
            @"(?<v>\b[1-9]\d{10}\b)",
            baseConfidence: 0.4),

        // Mobile: 0[789] + 9 digits (or +234).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?234|0)[789]\d{9}\b)",
            baseConfidence: 0.7),

        // Passport: a letter + 8 digits.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{8}\b)",
            baseConfidence: 0.4),
    };
}
