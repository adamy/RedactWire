// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.IdId;

/// <summary>Indonesia (id-ID) rule pack. See <c>docs/rules/localized/id-ID.md</c>.
/// VERIFY: formats + NIK structure against authoritative sources.</summary>
internal static class IdIdRules
{
    public static readonly IPiiRule[] Rules =
    {
        // NIK (KTP): 16 digits, embedded birth date (no check digit → date sanity only).
        new RegexRule("Nik", PiiType.NationalId,
            @"(?<v>\b\d{16}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.IndonesiaNik(v) ? (true, 0.85) : (false, 0)),

        // NPWP (tax): 15 digits. Format only for now.
        new RegexRule("Npwp", PiiType.TaxId,
            @"(?<v>\b\d{15}\b)",
            baseConfidence: 0.3),

        // Mobile: 08 + 8..11 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b08\d{8,11}\b)",
            baseConfidence: 0.8),

        // Passport: a letter + 7 digits. Loose.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{7}\b)",
            baseConfidence: 0.4),

        // Postal code: 5 digits. Weak alone.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{5}\b)",
            baseConfidence: 0.2),
    };
}
