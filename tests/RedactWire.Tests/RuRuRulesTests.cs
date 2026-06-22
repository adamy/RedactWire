// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class RuRuRulesTests
{
    private static readonly CultureInfo RuRu = new("ru-RU");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(RuRu).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, RuRu).Cultures.Single(c => c.Culture == "ru-RU").Matches;

    [Theory]
    [InlineData("7707083893")]     // 10-digit entity INN
    [InlineData("500100732259")]   // 12-digit individual INN
    public void Inn_valid_positive(string text)
    {
        Assert.Contains(Matches(text), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Inn_invalid_no_match()
    {
        Assert.DoesNotContain(Matches("7707083890"), m => m.Type == PiiType.TaxId);
    }

    [Theory]
    [InlineData("112-233-445 95")]
    [InlineData("11223344595")]
    public void Snils_valid_positive(string text)
    {
        var m = Matches(text).Single(x => x.Type == PiiType.Custom);
        Assert.Equal("SNILS", m.Subtype);
        Assert.Equal(PiiSeverity.Critical, m.Severity);
    }

    [Fact]
    public void Snils_invalid_no_match()
    {
        Assert.DoesNotContain(Matches("11223344594"), m => m.Type == PiiType.Custom);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("+79161234567"), m => m.Type == PiiType.Phone);
    }
}
