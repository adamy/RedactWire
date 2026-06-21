// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire.Rules.Localized.EnUs;

/// <summary>United States (en-US) rule pack. See <c>docs/rules/localized/en-US.md</c>.
/// No checksums exist for these formats, so confidence calibration is the only
/// false-positive defense. DriverLicense is intentionally skipped in v1 (50 states,
/// no common format).</summary>
internal static class EnUsRules
{
    public static readonly IPiiRule[] Rules =
    {
        // SSN: ###-##-#### (also spaces). Reject invalid areas/groups/serials.
        // area ≠ 000/666/900-999, group ≠ 00, serial ≠ 0000.
        new RegexRule("SSN", PiiType.SocialSecurity,
            @"(?<v>\b(?!000|666|9\d{2})\d{3}[ -](?!00)\d{2}[ -](?!0000)\d{4}\b)",
            baseConfidence: 0.85),

        // NANP phone: optional +1, area & exchange start [2-9].
        new RegexRule("Phone", PiiType.Phone,
            @"(?<v>(?:\+?1[ .-]?)?\(?[2-9]\d{2}\)?[ .-]?[2-9]\d{2}[ .-]?\d{4})\b",
            baseConfidence: 0.7),

        // ZIP: 5 digits, optional +4. Weak alone.
        new RegexRule("ZIP", PiiType.PostalCode,
            @"(?<v>\b\d{5}(?:-\d{4})?\b)",
            baseConfidence: 0.25),

        // US passport: 9 digits, or 1 letter + 8 digits. Loose.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[A-Za-z]?\d{8}\b|\b\d{9}\b)",
            baseConfidence: 0.4),

        // Composite street address (see AddressRule).
        new AddressRule(),
    };
}
