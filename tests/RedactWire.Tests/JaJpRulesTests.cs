// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class JaJpRulesTests
{
    private static readonly CultureInfo JaJp = new("ja-JP");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(JaJp).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, JaJp).Cultures.Single(c => c.Culture == "ja-JP").Matches;

    [Fact]
    public void MyNumber_valid_positive()
    {
        Assert.Contains(Matches("123456789018"),
            m => m.Type == PiiType.NationalId && m.Value == "123456789018");
    }

    [Fact]
    public void MyNumber_bad_checkdigit_no_match()
    {
        Assert.DoesNotContain(Matches("123456789010"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("090-1234-5678"), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Postcode_positive()
    {
        Assert.Contains(Matches("100-0001"), m => m.Type == PiiType.PostalCode);
    }
}
