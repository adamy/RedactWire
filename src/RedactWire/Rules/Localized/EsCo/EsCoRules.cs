// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EsCo;

/// <summary>Colombia (es-CO) rule pack. See <c>docs/rules/localized/es-CO.md</c>.
/// Cédula has no public checksum. VERIFY: formats.</summary>
internal static class EsCoRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Cédula de ciudadanía: 8-10 digits. No checksum → format only.
        new RegexRule("Cedula", PiiType.NationalId,
            @"(?<v>\b\d{8,10}\b)",
            baseConfidence: 0.35),

        // Mobile: (+57 | 0) 3 + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?57|0)?3\d{9}\b)",
            baseConfidence: 0.6),
    };
}
