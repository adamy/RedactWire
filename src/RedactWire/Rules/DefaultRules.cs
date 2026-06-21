// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules.Localized.EnUs;

namespace RedactWire.Rules;

/// <summary>Built-in rule packs. Invariant (country-agnostic) rules live here;
/// per-country packs live under <c>Localized/</c>. Kept deliberately small and regex-first.</summary>
internal static class DefaultRules
{
    // ── Culture-agnostic: always run ─────────────────────────────────────────
    public static readonly IPiiRule[] Invariant =
    {
        new RegexRule("Email", PiiType.Email,
            @"(?<v>[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,})",
            baseConfidence: 0.95),

        // 13–19 digit groups, optionally space/dash separated, gated by Luhn.
        new RegexRule("CreditCard", PiiType.CreditCard,
            @"(?<v>\b(?:\d[ -]?){13,19}\b)",
            baseConfidence: 0.5,
            validate: v => Checksums.Luhn(Checksums.Digits(v)) ? (true, 0.97) : (false, 0)),

        new RegexRule("IPv4", PiiType.IpAddress,
            @"(?<v>\b(?:(?:25[0-5]|2[0-4]\d|1?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|1?\d?\d)\b)",
            baseConfidence: 0.9),

        // IBAN: 2 letters, 2 check digits, up to 30 alphanumerics; mod-97 validated.
        new RegexRule("Iban", PiiType.Iban,
            @"(?<v>\b[A-Z]{2}\d{2}[A-Z0-9]{10,30}\b)",
            baseConfidence: 0.6,
            validate: v => Checksums.IbanValid(v) ? (true, 0.97) : (false, 0)),
    };

    // ── Per-culture registry ─────────────────────────────────────────────────
    public static readonly IReadOnlyDictionary<string, IPiiRule[]> ByCulture =
        new Dictionary<string, IPiiRule[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["en-US"] = EnUsRules.Rules,
        };
}
