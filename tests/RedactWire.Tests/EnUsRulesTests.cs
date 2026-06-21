// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class EnUsRulesTests
{
    private static readonly CultureInfo EnUs = new("en-US");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(EnUs).Build();

    private static IEnumerable<PiiMatch> UsMatches(string text) =>
        Detector.Detect(text, EnUs).Cultures.Single(c => c.Culture == "en-US").Matches;

    // ── SSN ──────────────────────────────────────────────────────────────────
    [Fact]
    public void Ssn_positive()
    {
        Assert.Contains(UsMatches("My SSN is 123-45-6789"),
            m => m.Type == PiiType.SocialSecurity && m.Value == "123-45-6789");
    }

    [Theory]
    [InlineData("000-12-3456")]   // area 000
    [InlineData("666-12-3456")]   // area 666
    [InlineData("900-12-3456")]   // area 9xx
    [InlineData("123-00-6789")]   // group 00
    [InlineData("123-45-0000")]   // serial 0000
    public void Ssn_invalid_no_match(string text)
    {
        Assert.DoesNotContain(UsMatches(text), m => m.Type == PiiType.SocialSecurity);
    }

    // ── Phone (NANP) ───────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Call (415) 555-0132")]
    [InlineData("+1 415 555 0132")]
    [InlineData("415-555-0132")]
    public void Phone_positive(string text)
    {
        Assert.Contains(UsMatches(text), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Phone_area_starts_with_1_no_match()
    {
        Assert.DoesNotContain(UsMatches("123 456 7890"), m => m.Type == PiiType.Phone);
    }

    // ── Address ──────────────────────────────────────────────────────────────
    [Fact]
    public void Address_full_higher_confidence()
    {
        var m = Assert.Single(UsMatches("123 Main St, Springfield, IL 62704")
            .Where(x => x.Type == PiiType.Address));
        Assert.Equal("123 Main St, Springfield, IL 62704", m.Value);
        Assert.True(m.Confidence >= 0.7);
    }

    [Fact]
    public void Address_street_only_lower_confidence()
    {
        var m = Assert.Single(UsMatches("4567 N Oak Avenue")
            .Where(x => x.Type == PiiType.Address));
        Assert.Equal("4567 N Oak Avenue", m.Value);
        Assert.True(m.Confidence < 0.7);
    }

    [Fact]
    public void Address_invalid_state_drops_city_part()
    {
        // ZZ is not a real state → no full/city address; only the street fragment.
        var addrs = UsMatches("123 Main St, Springfield, ZZ 62704")
            .Where(x => x.Type == PiiType.Address).ToList();
        Assert.Single(addrs);
        Assert.Equal("123 Main St", addrs[0].Value);
    }
}
