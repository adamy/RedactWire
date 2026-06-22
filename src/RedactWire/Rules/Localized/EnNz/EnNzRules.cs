// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnNz;

/// <summary>New Zealand (en-NZ) rule pack. See <c>docs/rules/localized/en-NZ.md</c>.
/// Driver licence is a de-facto ID here, so it is included. VERIFY: formats + IRD.</summary>
internal static class EnNzRules
{
    public static readonly IPiiRule[] Rules =
    {
        // IRD (tax): 8 or 9 digits, IR's weighted algorithm.
        new RegexRule("Ird", PiiType.TaxId,
            @"(?<v>\b\d{2,3}-?\d{3}-?\d{3}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.NzIrd(v) ? (true, 0.9) : (false, 0)),

        // Driver licence: 2 letters + 6 digits. Widely used as an ID (must-have).
        new RegexRule("DriverLicence", PiiType.DriverLicense,
            @"(?<v>\b[A-Za-z]{2}\d{6}\b)",
            baseConfidence: 0.6),

        // NHI (health): 3 letters + 4 digits (legacy form). Custom type with a name.
        new RegexRule("Nhi", PiiType.Custom,
            @"(?<v>\b[A-HJ-NP-Za-hj-np-z]{3}\d{4}\b)",
            baseConfidence: 0.55,
            severity: PiiSeverity.Critical,
            subtype: "NHI"),

        // Mobile: (+64 | 0) 2 + 7-9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b(?:\+?64|0)2\d{7,9}\b)",
            baseConfidence: 0.7),

        // Postcode: 4 digits.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{4}\b)",
            baseConfidence: 0.2),
    };
}
