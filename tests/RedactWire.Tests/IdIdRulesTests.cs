// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class IdIdRulesTests
{
    private static readonly CultureInfo IdId = new("id-ID");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(IdId).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, IdId).Cultures.Single(c => c.Culture == "id-ID").Matches;

    [Fact]
    public void Nik_valid_positive()
    {
        Assert.Contains(Matches("3201010101900001"),
            m => m.Type == PiiType.NationalId && m.Value == "3201010101900001");
    }

    [Fact]
    public void Nik_female_day_plus_40_positive()
    {
        // Day 41 = female (01 + 40).
        Assert.Contains(Matches("3201014101900001"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Nik_bad_month_no_match()
    {
        Assert.DoesNotContain(Matches("3201010113900001"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("081234567890"), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Npwp_positive()
    {
        Assert.Contains(Matches("012345678901234"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Postcode_positive_low_confidence()
    {
        var m = Matches("12345").Single(x => x.Type == PiiType.PostalCode);
        Assert.True(m.Confidence < 0.5);
    }
}
