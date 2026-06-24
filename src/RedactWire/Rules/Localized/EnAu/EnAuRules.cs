// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnAu;

/// <summary>Australia (en-AU) rule pack. See <c>docs/rules/localized/en-AU.md</c>.
/// VERIFY: formats + TFN checksum.</summary>
internal static class EnAuRules
{
    public static readonly IPiiRule[] Rules =
    {
        // TFN (tax file number): 9 digits, weighted checksum.
        new RegexRule("Tfn", PiiType.TaxId,
            @"(?<v>\b\d{3}\s?\d{3}\s?\d{3}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.AustraliaTfn(v) ? (true, 0.9) : (false, 0)),

        // Medicare card: 10 digits, mod-10 check. Custom type with a name.
        new RegexRule("Medicare", PiiType.Custom,
            @"(?<v>\b[2-6]\d{9}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.AustraliaMedicare(v) ? (true, 0.92) : (false, 0),
            severity: PiiSeverity.Critical,
            subtype: "Medicare"),

        // Mobile: (+61 | 0) 4 + 8 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?61|0)4\d{8}\b)",
            baseConfidence: 0.7),

        // Postcode: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
