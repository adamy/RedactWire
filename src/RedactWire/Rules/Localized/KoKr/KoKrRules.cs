// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.KoKr;

/// <summary>South Korea (ko-KR) rule pack. See <c>docs/rules/localized/ko-KR.md</c>.
/// RRN is highly sensitive. VERIFY: formats + RRN checksum.</summary>
internal static class KoKrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // RRN (주민등록번호): 13 digits (often 000000-0000000), mod-11 check + DOB sanity.
        new RegexRule("Rrn", PiiType.NationalId,
            @"(?<v>\b\d{6}-?\d{7}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.KoreaRrn(v) ? (true, 0.97) : (false, 0)),

        // Mobile: 01[016789]-XXXX-XXXX.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>\b01[016789]-?\d{3,4}-?\d{4}\b)",
            baseConfidence: 0.7),
    };
}
