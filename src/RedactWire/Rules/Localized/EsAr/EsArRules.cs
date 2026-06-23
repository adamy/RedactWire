// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EsAr;

/// <summary>Argentina (es-AR) rule pack. See <c>docs/rules/localized/es-AR.md</c>.
/// VERIFY: formats + CUIT checksum.</summary>
internal static class EsArRules
{
    public static readonly IPiiRule[] Rules =
    {
        // CUIT/CUIL (tax): 11 digits (XX-XXXXXXXX-X), mod-11 check digit.
        new RegexRule("Cuit", PiiType.TaxId,
            @"(?<v>\b\d{2}-?\d{8}-?\d\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.ArgentinaCuit(v) ? (true, 0.95) : (false, 0)),

        // DNI: 7-8 digits (often dotted). No checksum → format only.
        new RegexRule("Dni", PiiType.NationalId,
            @"(?<v>\b\d{1,3}\.?\d{3}\.?\d{3}\b)",
            baseConfidence: 0.35),

        // Mobile: (+54) optional 9, area + number.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?54\s?9?\s?)?\d{2,4}[\s-]?\d{6,8})",
            baseConfidence: 0.5),
    };
}
