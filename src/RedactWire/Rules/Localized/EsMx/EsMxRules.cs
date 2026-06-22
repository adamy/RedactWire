// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EsMx;

/// <summary>Mexico (es-MX) rule pack. See <c>docs/rules/localized/es-MX.md</c>.
/// VERIFY: formats + CURP check digit against authoritative sources.</summary>
internal static class EsMxRules
{
    public static readonly IPiiRule[] Rules =
    {
        // CURP: 18 chars, embedded DOB/sex/state + mod-10 check digit.
        new RegexRule("Curp", PiiType.NationalId,
            @"(?<v>\b[A-Z]{4}\d{6}[HM][A-Z]{5}[A-Z0-9]\d\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.MexicoCurp(v) ? (true, 0.97) : (false, 0)),

        // RFC (tax): 13 (person) / 12 (company) chars. Format only.
        new RegexRule("Rfc", PiiType.TaxId,
            @"(?<v>\b[A-Z]{3,4}\d{6}[A-Z0-9]{3}\b)",
            baseConfidence: 0.6),

        // Mobile: optional +52 (and a leading 1); 10 digits, 2-3 digit area code.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?52[\s-]?1?[\s-]?)?\d{2,3}[\s-]?\d{3,4}[\s-]?\d{4})",
            baseConfidence: 0.55),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
