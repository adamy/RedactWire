// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.LtLt;

/// <summary>Lithuania (lt-LT) rule pack. Asmens kodas shares the two-pass mod-11 algorithm.
/// See <c>docs/rules/localized/lt-LT.md</c>. IBAN via invariant. VERIFY.</summary>
internal static class LtLtRules
{
    public static readonly IPiiRule[] Rules =
    {
        new RegexRule("AsmensKodas", PiiType.NationalId,
            @"(?<v>\b[1-6]\d{10}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.EstoniaIk(v) ? (true, 0.95) : (false, 0)),

        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?370\s?)?6\d{7}\b)",
            baseConfidence: 0.6),

        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\bLT-?\d{5}\b)",
            baseConfidence: 0.3),
    };
}
