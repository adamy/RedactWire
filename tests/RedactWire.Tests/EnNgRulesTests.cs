// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class EnNgRulesTests
{
    private static readonly CultureInfo EnNg = new("en-NG");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(EnNg).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, EnNg).Cultures.Single(c => c.Culture == "en-NG").Matches;

    [Fact]
    public void Nin_positive()
    {
        Assert.Contains(Matches("12345678901"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Mobile_is_not_misread_as_nin()
    {
        var hits = Matches("08031234567").ToList();
        Assert.Contains(hits, m => m.Type == PiiType.Phone);
        Assert.DoesNotContain(hits, m => m.Type == PiiType.NationalId);   // 0-leading
    }

    [Fact]
    public void Mobile_international_positive()
    {
        Assert.Contains(Matches("+2348031234567"), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Passport_positive()
    {
        Assert.Contains(Matches("A12345678"), m => m.Type == PiiType.Passport);
    }
}
