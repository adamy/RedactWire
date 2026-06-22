// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class ZhCnRulesTests
{
    private static readonly CultureInfo ZhCn = new("zh-CN");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(ZhCn).Build();

    private static IEnumerable<PiiMatch> CnMatches(string text) =>
        Detector.Detect(text, ZhCn).Cultures.Single(c => c.Culture == "zh-CN").Matches;

    // ── Resident ID ──────────────────────────────────────────────────────────
    [Fact]
    public void ResidentId_valid_positive()
    {
        Assert.Contains(CnMatches("身份证 110101199001010015"),
            m => m.Type == PiiType.NationalId && m.Value == "110101199001010015");
    }

    [Fact]
    public void ResidentId_bad_checkdigit_no_match()
    {
        Assert.DoesNotContain(CnMatches("110101199001010014"),
            m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void ResidentId_seventeen_digits_no_match()
    {
        // 17 digits — fails the 18-char pattern (good demo of why gating matters).
        Assert.DoesNotContain(CnMatches("22021982092811810"),
            m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void ResidentId_impossible_birthdate_no_match()
    {
        // Valid-looking length but month 13 → birth-date sanity fails.
        Assert.DoesNotContain(CnMatches("11010119901301001X"),
            m => m.Type == PiiType.NationalId);
    }

    // ── Mobile ───────────────────────────────────────────────────────────────
    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(CnMatches("手机 13693993330"), m => m.Type == PiiType.Phone);
    }

    [Theory]
    [InlineData("12345678901")]   // 2nd digit not 3-9
    [InlineData("1369399333")]    // 10 digits
    public void Mobile_invalid_no_match(string text)
    {
        Assert.DoesNotContain(CnMatches(text), m => m.Type == PiiType.Phone);
    }

    // ── Passport / Postcode ──────────────────────────────────────────────────
    [Fact]
    public void Passport_positive()
    {
        Assert.Contains(CnMatches("E12345678"), m => m.Type == PiiType.Passport);
    }

    [Fact]
    public void Postcode_positive_low_confidence()
    {
        var m = CnMatches("100080").Single(x => x.Type == PiiType.PostalCode);
        Assert.True(m.Confidence < 0.5);
    }
}
