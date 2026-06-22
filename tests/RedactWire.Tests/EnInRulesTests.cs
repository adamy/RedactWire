// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class EnInRulesTests
{
    private static readonly CultureInfo EnIn = new("en-IN");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(EnIn).Build();

    private static IEnumerable<PiiMatch> InMatches(string text) =>
        Detector.Detect(text, EnIn).Cultures.Single(c => c.Culture == "en-IN").Matches;

    // ── Aadhaar (Verhoeff) ─────────────────────────────────────────────────────
    [Fact]
    public void Aadhaar_valid_positive()
    {
        // 234123412346 is Verhoeff-valid (synthetic).
        Assert.Contains(InMatches("234123412346"),
            m => m.Type == PiiType.NationalId && m.Value == "234123412346");
    }

    [Fact]
    public void Aadhaar_bad_checkdigit_no_match()
    {
        Assert.DoesNotContain(InMatches("234123412345"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Aadhaar_starting_with_one_no_match()
    {
        // Aadhaar cannot start with 0 or 1.
        Assert.DoesNotContain(InMatches("123412341234"), m => m.Type == PiiType.NationalId);
    }

    // ── PAN ────────────────────────────────────────────────────────────────────
    [Fact]
    public void Pan_positive()
    {
        Assert.Contains(InMatches("ABCDE1234F"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Pan_wrong_shape_no_match()
    {
        Assert.DoesNotContain(InMatches("ABCD1234F"), m => m.Type == PiiType.TaxId);
    }

    // ── Mobile ─────────────────────────────────────────────────────────────────
    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(InMatches("9876543210"), m => m.Type == PiiType.Phone);
    }

    [Theory]
    [InlineData("5876543210")]   // starts 5
    [InlineData("987654321")]    // 9 digits
    public void Mobile_invalid_no_match(string text)
    {
        Assert.DoesNotContain(InMatches(text), m => m.Type == PiiType.Phone);
    }

    // ── Passport / PIN ─────────────────────────────────────────────────────────
    [Fact]
    public void Passport_positive()
    {
        Assert.Contains(InMatches("A1234567"), m => m.Type == PiiType.Passport);
    }

    [Fact]
    public void Pin_positive_low_confidence()
    {
        var m = InMatches("560001").Single(x => x.Type == PiiType.PostalCode);
        Assert.True(m.Confidence < 0.5);
    }
}
