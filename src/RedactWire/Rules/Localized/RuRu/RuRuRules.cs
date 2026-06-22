// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.RuRu;

/// <summary>Russia (ru-RU) rule pack. See <c>docs/rules/localized/ru-RU.md</c>.
/// VERIFY: formats + INN/SNILS checksums against authoritative sources.</summary>
internal static class RuRuRules
{
    public static readonly IPiiRule[] Rules =
    {
        // INN (tax): 10 digits (entity) or 12 digits (individual), mod-11 check.
        new RegexRule("Inn", PiiType.TaxId,
            @"(?<v>\b\d{12}\b|\b\d{10}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.RussiaInn(v) ? (true, 0.95) : (false, 0)),

        // SNILS (insurance): 11 digits, control number. Custom type with a name.
        new RegexRule("Snils", PiiType.Custom,
            @"(?<v>\b\d{3}-?\d{3}-?\d{3}\s?\d{2}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.RussiaSnils(v) ? (true, 0.95) : (false, 0),
            severity: PiiSeverity.Critical,
            subtype: "SNILS"),

        // Mobile: +7 / 8, then 9XXXXXXXXX.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?7|8)9\d{9})",
            baseConfidence: 0.7),
    };
}
