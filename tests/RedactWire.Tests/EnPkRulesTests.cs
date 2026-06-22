// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class EnPkRulesTests
{
    private static readonly CultureInfo EnPk = new("en-PK");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(EnPk).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, EnPk).Cultures.Single(c => c.Culture == "en-PK").Matches;

    [Theory]
    [InlineData("42101-1234567-8")]
    [InlineData("4210112345678")]
    public void Cnic_positive(string text)
    {
        Assert.Contains(Matches(text), m => m.Type == PiiType.NationalId);
    }

    [Theory]
    [InlineData("03001234567")]
    [InlineData("+923001234567")]
    public void Mobile_positive(string text)
    {
        Assert.Contains(Matches(text), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Passport_positive()
    {
        Assert.Contains(Matches("AB1234567"), m => m.Type == PiiType.Passport);
    }
}
