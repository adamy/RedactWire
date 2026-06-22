// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EsEs;

/// <summary>Spain (es-ES) rule pack. See <c>docs/rules/localized/es-ES.md</c>.
/// IBAN via the invariant pack. VERIFY: formats + DNI/NIE control letter.</summary>
internal static class EsEsRules
{
    public static readonly IPiiRule[] Rules =
    {
        // DNI: 8 digits + control letter (mod-23).
        new RegexRule("Dni", PiiType.NationalId,
            @"(?<v>\b\d{8}[A-Za-z]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SpainDniNie(v) ? (true, 0.95) : (false, 0)),

        // NIE (foreigners): X/Y/Z + 7 digits + control letter (mod-23).
        new RegexRule("Nie", PiiType.NationalId,
            @"(?<v>\b[XYZxyz]\d{7}[A-Za-z]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.SpainDniNie(v) ? (true, 0.95) : (false, 0)),

        // Mobile: optional +34, [67] + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?34\s?)?[67]\d{8})",
            baseConfidence: 0.6),

        // Postal code: 5 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
