// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch G (final): GR, HU, LV.
public class Tier3BatchGTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Greece ───────────────────────────────────────────────────────────────────
    [Fact] public void Gr_afm_valid() => Assert.Contains(M("el-GR", "123456783"), m => m.Type == PiiType.TaxId);
    [Fact] public void Gr_afm_invalid() => Assert.DoesNotContain(M("el-GR", "123456780"), m => m.Type == PiiType.TaxId);
    [Fact] public void Gr_amka_valid() => Assert.Contains(M("el-GR", "01019012341"), m => m.Type == PiiType.Custom && m.Subtype == "AMKA");
    [Fact] public void Gr_mobile() => Assert.Contains(M("el-GR", "6912345678"), m => m.Type == PiiType.Phone);

    // ── Hungary ──────────────────────────────────────────────────────────────────
    [Fact] public void Hu_id_valid() => Assert.Contains(M("hu-HU", "12345678919"), m => m.Type == PiiType.NationalId);
    [Fact] public void Hu_id_invalid() => Assert.DoesNotContain(M("hu-HU", "12345678910"), m => m.Type == PiiType.NationalId);
    [Fact] public void Hu_mobile() => Assert.Contains(M("hu-HU", "+36201234567"), m => m.Type == PiiType.Phone);

    // ── Latvia ───────────────────────────────────────────────────────────────────
    [Fact] public void Lv_code() => Assert.Contains(M("lv-LV", "010190-12345"), m => m.Type == PiiType.NationalId);
    [Fact] public void Lv_mobile() => Assert.Contains(M("lv-LV", "21234567"), m => m.Type == PiiType.Phone);
    [Fact] public void Lv_postcode() => Assert.Contains(M("lv-LV", "LV-1010"), m => m.Type == PiiType.PostalCode);
}
