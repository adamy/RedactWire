// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch D: HK, TW, MO, PT, CH.
public class Tier3BatchDTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Hong Kong ────────────────────────────────────────────────────────────────
    [Fact] public void Hk_hkid_valid() => Assert.Contains(M("zh-HK", "A123456(3)"), m => m.Type == PiiType.NationalId);
    [Fact] public void Hk_hkid_invalid() => Assert.DoesNotContain(M("zh-HK", "A123456(4)"), m => m.Type == PiiType.NationalId);
    [Fact] public void Hk_mobile() => Assert.Contains(M("zh-HK", "51234567"), m => m.Type == PiiType.Phone);

    // ── Taiwan ───────────────────────────────────────────────────────────────────
    [Fact] public void Tw_id_valid() => Assert.Contains(M("zh-TW", "A123456789"), m => m.Type == PiiType.NationalId);
    [Fact] public void Tw_id_invalid() => Assert.DoesNotContain(M("zh-TW", "A123456788"), m => m.Type == PiiType.NationalId);
    [Fact] public void Tw_mobile() => Assert.Contains(M("zh-TW", "0912345678"), m => m.Type == PiiType.Phone);

    // ── Macau ────────────────────────────────────────────────────────────────────
    [Fact] public void Mo_id() => Assert.Contains(M("zh-MO", "1234567(8)"), m => m.Type == PiiType.NationalId);
    [Fact] public void Mo_mobile() => Assert.Contains(M("zh-MO", "61234567"), m => m.Type == PiiType.Phone);

    // ── Portugal ─────────────────────────────────────────────────────────────────
    [Fact] public void Pt_nif_valid() => Assert.Contains(M("pt-PT", "123456789"), m => m.Type == PiiType.TaxId);
    [Fact] public void Pt_nif_invalid() => Assert.DoesNotContain(M("pt-PT", "123456780"), m => m.Type == PiiType.TaxId);
    [Fact] public void Pt_postcode() => Assert.Contains(M("pt-PT", "1234-567"), m => m.Type == PiiType.PostalCode);

    // ── Switzerland ──────────────────────────────────────────────────────────────
    [Fact] public void Ch_ahv_valid() => Assert.Contains(M("de-CH", "7561234567897"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ch_ahv_invalid() => Assert.DoesNotContain(M("de-CH", "7561234567890"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ch_mobile() => Assert.Contains(M("de-CH", "0791234567"), m => m.Type == PiiType.Phone);
}
