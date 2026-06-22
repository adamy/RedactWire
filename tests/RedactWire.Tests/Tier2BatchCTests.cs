// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 2 batch C: ES, BD.
public class Tier2BatchCTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Spain ────────────────────────────────────────────────────────────────────
    [Fact] public void Es_dni_valid() => Assert.Contains(M("es-ES", "12345678Z"), m => m.Type == PiiType.NationalId);
    [Fact] public void Es_dni_invalid() => Assert.DoesNotContain(M("es-ES", "12345678A"), m => m.Type == PiiType.NationalId);
    [Fact] public void Es_nie_valid() => Assert.Contains(M("es-ES", "X1234567L"), m => m.Type == PiiType.NationalId);
    [Fact] public void Es_mobile() => Assert.Contains(M("es-ES", "612345678"), m => m.Type == PiiType.Phone);

    // ── Bangladesh ───────────────────────────────────────────────────────────────
    [Theory]
    [InlineData("1234567890")]
    [InlineData("1234567890123")]
    [InlineData("12345678901234567")]
    public void Bd_nid(string text) => Assert.Contains(M("bn-BD", text), m => m.Type == PiiType.NationalId);

    [Fact] public void Bd_mobile() => Assert.Contains(M("bn-BD", "01712345678"), m => m.Type == PiiType.Phone);
}
