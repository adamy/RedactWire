// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 2 batch A: PH, VN, GB, TR, FR.
public class Tier2BatchATests
{
    private static PiiDetector Det(string culture) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build();

    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        Det(culture).Detect(text, new CultureInfo(culture))
            .Cultures.Single(c => c.Culture == culture).Matches;

    // ── Philippines ────────────────────────────────────────────────────────────
    [Fact] public void Ph_philsys() => Assert.Contains(M("en-PH", "1234-5678-9012"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ph_tin() => Assert.Contains(M("en-PH", "123-456-789"), m => m.Type == PiiType.TaxId);
    [Fact] public void Ph_mobile() => Assert.Contains(M("en-PH", "09171234567"), m => m.Type == PiiType.Phone);

    // ── Vietnam ────────────────────────────────────────────────────────────────
    [Fact] public void Vn_cccd() => Assert.Contains(M("vi-VN", "012345678901"), m => m.Type == PiiType.NationalId);
    [Fact] public void Vn_mobile() => Assert.Contains(M("vi-VN", "0987654321"), m => m.Type == PiiType.Phone);

    // ── United Kingdom ───────────────────────────────────────────────────────────
    [Fact] public void Gb_nino() => Assert.Contains(M("en-GB", "AB123456C"), m => m.Type == PiiType.NationalId);
    [Fact] public void Gb_nhs_valid() => Assert.Contains(M("en-GB", "401 023 2137"), m => m.Type == PiiType.Custom && m.Subtype == "NHS");
    [Fact] public void Gb_nhs_invalid() => Assert.DoesNotContain(M("en-GB", "401 023 2130"), m => m.Type == PiiType.Custom);
    [Fact] public void Gb_postcode() => Assert.Contains(M("en-GB", "SW1A 1AA"), m => m.Type == PiiType.PostalCode);
    [Fact] public void Gb_mobile() => Assert.Contains(M("en-GB", "07700900123"), m => m.Type == PiiType.Phone);

    // ── Turkey ───────────────────────────────────────────────────────────────────
    [Fact] public void Tr_kimlik_valid() => Assert.Contains(M("tr-TR", "10000000078"), m => m.Type == PiiType.NationalId);
    [Fact] public void Tr_kimlik_invalid() => Assert.DoesNotContain(M("tr-TR", "10000000079"), m => m.Type == PiiType.NationalId);
    [Fact] public void Tr_mobile() => Assert.Contains(M("tr-TR", "05001234567"), m => m.Type == PiiType.Phone);

    // ── France ───────────────────────────────────────────────────────────────────
    [Fact] public void Fr_nir_valid() => Assert.Contains(M("fr-FR", "180067504812307"), m => m.Type == PiiType.NationalId);
    [Fact] public void Fr_nir_invalid() => Assert.DoesNotContain(M("fr-FR", "180067504812308"), m => m.Type == PiiType.NationalId);
    [Fact] public void Fr_mobile() => Assert.Contains(M("fr-FR", "0612345678"), m => m.Type == PiiType.Phone);
}
