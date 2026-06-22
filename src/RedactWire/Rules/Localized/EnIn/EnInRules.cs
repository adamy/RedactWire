// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.EnIn;

/// <summary>India (en-IN) rule pack. See <c>docs/rules/localized/en-IN.md</c>.
/// VERIFY: formats and the Aadhaar Verhoeff checksum against authoritative sources.</summary>
internal static class EnInRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Aadhaar: 12 digits, must not start 0/1, Verhoeff check digit.
        new RegexRule("Aadhaar", PiiType.NationalId,
            @"(?<v>\b[2-9]\d{11}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Verhoeff(v) ? (true, 0.97) : (false, 0)),

        // PAN (tax): 5 letters, 4 digits, 1 letter. No public checksum → format only.
        new RegexRule("Pan", PiiType.TaxId,
            @"(?<v>\b[A-Z]{5}\d{4}[A-Z]\b)",
            baseConfidence: 0.7),

        // Mobile: 10 digits starting 6-9 (optionally +91 prefixed).
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b[6-9]\d{9}\b)",
            baseConfidence: 0.8),

        // Passport: a letter + 7 digits. Loose.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Z]\d{7}\b)",
            baseConfidence: 0.4),

        // PIN postal code: 6 digits. Weak on its own.
        new RegexRule("Pin", PiiType.PostalCode,
            @"(?<v>\b\d{6}\b)",
            baseConfidence: 0.2),
    };
}
