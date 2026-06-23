// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EsCl;

/// <summary>Chile (es-CL) rule pack. See <c>docs/rules/localized/es-CL.md</c>.
/// VERIFY: formats + RUT check digit.</summary>
internal static class EsClRules
{
    public static readonly IPiiRule[] Rules =
    {
        // RUT/RUN: 7-8 digit body + check char (0-9 or K), mod-11.
        new RegexRule("Rut", PiiType.NationalId,
            @"(?<v>\b\d{1,2}\.?\d{3}\.?\d{3}-?[\dkK]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.ChileRut(v) ? (true, 0.95) : (false, 0)),

        // Mobile: (+56 | 0) 9 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?56|0)9\d{8}\b)",
            baseConfidence: 0.7),
    };
}
