// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.ZhCn;

/// <summary>China (zh-CN) rule pack. See <c>docs/rules/localized/zh-CN.md</c>.
/// VERIFY: formats and the GB11643 checksum against authoritative sources.</summary>
internal static class ZhCnRules
{
    public static readonly IPiiRule[] Rules =
    {
        // Resident Identity Card: 18 chars, last may be X; GB11643 check digit + birth date.
        new RegexRule("ResidentId", PiiType.NationalId,
            @"(?<v>\b\d{17}[\dXx]\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.ChinaResidentId(v.ToUpperInvariant()) ? (true, 0.97) : (false, 0)),

        // Mainland mobile: 1 + [3-9] + 9 digits.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b1[3-9]\d{9}\b)",
            baseConfidence: 0.85),

        // Ordinary passport: a type letter + 8 digits (e.g. E12345678). Loose.
        new RegexRule("Passport", PiiType.Passport,
            @"(?<v>\b[EGDSP]\d{8}\b)",
            baseConfidence: 0.4),

        // Postal code: 6 digits. Weak on its own.
        new RegexRule("Postcode", PiiType.PostalCode,
            @"(?<v>\b\d{6}\b)",
            baseConfidence: 0.2),
    };
}
