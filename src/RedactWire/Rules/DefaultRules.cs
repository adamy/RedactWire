// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules.Localized.ArEg;
using RedactWire.Rules.Localized.ArSa;
using RedactWire.Rules.Localized.BnBd;
using RedactWire.Rules.Localized.CsCz;
using RedactWire.Rules.Localized.DaDk;
using RedactWire.Rules.Localized.DeCh;
using RedactWire.Rules.Localized.DeDe;
using RedactWire.Rules.Localized.EnAu;
using RedactWire.Rules.Localized.EnCa;
using RedactWire.Rules.Localized.EnGb;
using RedactWire.Rules.Localized.EnIe;
using RedactWire.Rules.Localized.EnIn;
using RedactWire.Rules.Localized.EnNg;
using RedactWire.Rules.Localized.EnNz;
using RedactWire.Rules.Localized.EnPh;
using RedactWire.Rules.Localized.EnPk;
using RedactWire.Rules.Localized.EnUs;
using RedactWire.Rules.Localized.EnZa;
using RedactWire.Rules.Localized.EsAr;
using RedactWire.Rules.Localized.EsCl;
using RedactWire.Rules.Localized.EsCo;
using RedactWire.Rules.Localized.EsEs;
using RedactWire.Rules.Localized.EsMx;
using RedactWire.Rules.Localized.EtEe;
using RedactWire.Rules.Localized.FaIr;
using RedactWire.Rules.Localized.HeIl;
using RedactWire.Rules.Localized.FiFi;
using RedactWire.Rules.Localized.IsIs;
using RedactWire.Rules.Localized.FrFr;
using RedactWire.Rules.Localized.IdId;
using RedactWire.Rules.Localized.ItIt;
using RedactWire.Rules.Localized.JaJp;
using RedactWire.Rules.Localized.KoKr;
using RedactWire.Rules.Localized.NbNo;
using RedactWire.Rules.Localized.NlBe;
using RedactWire.Rules.Localized.NlNl;
using RedactWire.Rules.Localized.PlPl;
using RedactWire.Rules.Localized.PtBr;
using RedactWire.Rules.Localized.PtPt;
using RedactWire.Rules.Localized.RuRu;
using RedactWire.Rules.Localized.SvSe;
using RedactWire.Rules.Localized.ThTh;
using RedactWire.Rules.Localized.TrTr;
using RedactWire.Rules.Localized.ViVn;
using RedactWire.Rules.Localized.ZhCn;
using RedactWire.Rules.Localized.ZhHk;
using RedactWire.Rules.Localized.ZhMo;
using RedactWire.Rules.Localized.ZhTw;

namespace RedactWire.Rules;

/// <summary>Built-in rule packs. Invariant (country-agnostic) rules live here;
/// per-country packs live under <c>Localized/</c>. Kept deliberately small and regex-first.</summary>
internal static class DefaultRules
{
    // ── Culture-agnostic: always run ─────────────────────────────────────────
    public static readonly IPiiRule[] Invariant =
    {
        new RegexRule("Email", PiiType.Email,
            @"(?<v>[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,})",
            baseConfidence: 0.95),

        // 13–19 digit groups, optionally space/dash separated, gated by Luhn.
        new RegexRule("CreditCard", PiiType.CreditCard,
            @"(?<v>\b(?:\d[ -]?){13,19}\b)",
            baseConfidence: 0.5,
            validate: v => Checksums.Luhn(Checksums.Digits(v)) ? (true, 0.97) : (false, 0)),

        new RegexRule("IPv4", PiiType.IpAddress,
            @"(?<v>\b(?:(?:25[0-5]|2[0-4]\d|1?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|1?\d?\d)\b)",
            baseConfidence: 0.9),

        // IBAN: 2 letters, 2 check digits, up to 30 alphanumerics; mod-97 validated.
        new RegexRule("Iban", PiiType.Iban,
            @"(?<v>\b[A-Z]{2}\d{2}[A-Z0-9]{10,30}\b)",
            baseConfidence: 0.6,
            validate: v => Checksums.IbanValid(v) ? (true, 0.97) : (false, 0)),
    };

    // ── Per-culture registry ─────────────────────────────────────────────────
    public static readonly IReadOnlyDictionary<string, IPiiRule[]> ByCulture =
        new Dictionary<string, IPiiRule[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["en-US"] = EnUsRules.Rules,
            ["zh-CN"] = ZhCnRules.Rules,
            ["en-IN"] = EnInRules.Rules,
            ["id-ID"] = IdIdRules.Rules,
            ["pt-BR"] = PtBrRules.Rules,
            ["ru-RU"] = RuRuRules.Rules,
            ["ja-JP"] = JaJpRules.Rules,
            ["en-NG"] = EnNgRules.Rules,
            ["es-MX"] = EsMxRules.Rules,
            ["de-DE"] = DeDeRules.Rules,
            ["en-PK"] = EnPkRules.Rules,
            ["en-PH"] = EnPhRules.Rules,
            ["vi-VN"] = ViVnRules.Rules,
            ["en-GB"] = EnGbRules.Rules,
            ["tr-TR"] = TrTrRules.Rules,
            ["fr-FR"] = FrFrRules.Rules,
            ["ar-EG"] = ArEgRules.Rules,
            ["fa-IR"] = FaIrRules.Rules,
            ["th-TH"] = ThThRules.Rules,
            ["ko-KR"] = KoKrRules.Rules,
            ["it-IT"] = ItItRules.Rules,
            ["es-ES"] = EsEsRules.Rules,
            ["bn-BD"] = BnBdRules.Rules,
            ["en-CA"] = EnCaRules.Rules,
            ["en-AU"] = EnAuRules.Rules,
            ["en-NZ"] = EnNzRules.Rules,
            ["nl-NL"] = NlNlRules.Rules,
            ["pl-PL"] = PlPlRules.Rules,
            ["sv-SE"] = SvSeRules.Rules,
            ["nb-NO"] = NbNoRules.Rules,
            ["da-DK"] = DaDkRules.Rules,
            ["fi-FI"] = FiFiRules.Rules,
            ["en-ZA"] = EnZaRules.Rules,
            ["ar-SA"] = ArSaRules.Rules,
            ["es-AR"] = EsArRules.Rules,
            ["he-IL"] = HeIlRules.Rules,
            ["es-CL"] = EsClRules.Rules,
            ["es-CO"] = EsCoRules.Rules,
            ["zh-HK"] = ZhHkRules.Rules,
            ["zh-TW"] = ZhTwRules.Rules,
            ["zh-MO"] = ZhMoRules.Rules,
            ["pt-PT"] = PtPtRules.Rules,
            ["de-CH"] = DeChRules.Rules,
            ["nl-BE"] = NlBeRules.Rules,
            ["cs-CZ"] = CsCzRules.Rules,
            ["en-IE"] = EnIeRules.Rules,
            ["et-EE"] = EtEeRules.Rules,
            ["is-IS"] = IsIsRules.Rules,
        };
}
