// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnSg;

/// <summary>Singapore (en-SG) rule pack. Singapore has four official languages
/// (en/zh/ms/ta-SG); they all resolve to this pack by region. See
/// <c>docs/rules/localized/en-SG.md</c>. VERIFY: formats + NRIC check letter.</summary>
internal static class EnSgRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NRIC/FIN: prefix (S/T/F/G/M) + 7 digits + check letter.
        new RegexRule("Nric", PiiType.NationalId,
            @"(?<v>\b[STFGMstfgm]\d{7}[A-Za-z]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SingaporeNric(v) ? (true, 0.95) : (false, 0)),

        // Mobile: 8 or 9 + 7 digits (optional +65).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?65\s?)?[89]\d{7}\b)",
            baseConfidence: 0.6),

        // Postal code: 6 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{6}\b)",
            baseConfidence: 0.2),
    };
}
