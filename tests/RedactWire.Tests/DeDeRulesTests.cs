// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class DeDeRulesTests
{
    private static readonly CultureInfo DeDe = new("de-DE");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(DeDe).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, DeDe).Cultures.Single(c => c.Culture == "de-DE").Matches;

    [Fact]
    public void TaxId_valid_positive()
    {
        Assert.Contains(Matches("12345678903"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void TaxId_bad_checkdigit_no_match()
    {
        Assert.DoesNotContain(Matches("12345678900"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("01511234567"), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Plz_positive_low_confidence()
    {
        var m = Matches("10115").Single(x => x.Type == PiiType.PostalCode);
        Assert.True(m.Confidence < 0.5);
    }
}
