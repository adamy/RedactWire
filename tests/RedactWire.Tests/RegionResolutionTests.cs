// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// PII packs are keyed by country (region), so every language of a country shares one pack.
public class RegionResolutionTests
{
    private static PiiDetector With(string culture) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build();

    private static IEnumerable<PiiMatch> Detect(PiiDetector d, string text, string culture) =>
        d.Detect(text, new CultureInfo(culture)).Cultures.Single().Matches;

    [Fact]
    public void India_pack_resolves_for_any_indian_language()
    {
        var d = With("en-IN");                                   // registered via English
        Assert.Contains(Detect(d, "234123412346", "hi-IN"), m => m.Type == PiiType.NationalId);
        Assert.Contains(Detect(d, "234123412346", "ta-IN"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Adding_a_non_default_language_still_loads_the_country_pack()
    {
        // Register India via Hindi; detect with Tamil — both map to region IN.
        var d = PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("hi-IN")).Build();
        Assert.Contains(Detect(d, "234123412346", "ta-IN"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Canada_pack_resolves_for_french()
    {
        var d = With("en-CA");
        Assert.Contains(Detect(d, "046454286", "fr-CA"), m => m.Type == PiiType.NationalId);   // SIN
    }

    [Fact]
    public void Switzerland_pack_resolves_for_french_and_italian()
    {
        var d = With("de-CH");
        Assert.Contains(Detect(d, "7561234567897", "fr-CH"), m => m.Type == PiiType.NationalId); // AHV
        Assert.Contains(Detect(d, "7561234567897", "it-CH"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Validate_is_region_aware()
    {
        var d = With("en-IN");
        Assert.Equal(ValidationResult.Valid,
            d.Validate("234123412346", new CultureInfo("hi-IN"), PiiType.NationalId));
    }

    [Fact]
    public void Unsupported_country_still_reports_unsupported()
    {
        var d = With("en-IN");
        var r = d.Detect("x", new CultureInfo("fr-FR"));   // France not registered here
        Assert.False(r.Cultures.Single().Supported);
    }

    // The flip side: a shared LANGUAGE must NOT share rules. zh-CN, zh-TW, zh-HK, zh-MO and
    // zh-SG are all Chinese but are different countries with different ID systems.
    [Fact]
    public void Chinese_regions_use_distinct_packs()
    {
        var d = PiiDetectorBuilder.CreateDefault()
            .AddCulture(new CultureInfo("zh-CN"))
            .AddCulture(new CultureInfo("zh-TW"))
            .AddCulture(new CultureInfo("zh-HK"))
            .Build();

        const string chinaId = "110101199001010015";   // 18-digit Resident ID
        const string taiwanId = "A123456789";           // letter + 9 digits
        const string hkid = "A123456(3)";

        // Each ID is recognised only under its own region.
        Assert.Contains(Detect(d, chinaId, "zh-CN"), m => m.Type == PiiType.NationalId);
        Assert.DoesNotContain(Detect(d, chinaId, "zh-TW"), m => m.Type == PiiType.NationalId);

        Assert.Contains(Detect(d, taiwanId, "zh-TW"), m => m.Type == PiiType.NationalId);
        Assert.DoesNotContain(Detect(d, taiwanId, "zh-CN"), m => m.Type == PiiType.NationalId);

        Assert.Contains(Detect(d, hkid, "zh-HK"), m => m.Type == PiiType.NationalId);
        Assert.DoesNotContain(Detect(d, hkid, "zh-CN"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Singapore_pack_resolves_for_all_four_official_languages()
    {
        var d = With("en-SG");                       // registered via English
        foreach (var lang in new[] { "en-SG", "zh-SG", "ms-SG", "ta-SG" })
            Assert.Contains(Detect(d, "S1234567D", lang), m => m.Type == PiiType.NationalId);  // NRIC
    }

    [Fact]
    public void Chinese_singapore_uses_singapore_rules_not_china()
    {
        var d = PiiDetectorBuilder.CreateDefault()
            .AddCulture(new CultureInfo("zh-CN"))
            .AddCulture(new CultureInfo("en-SG"))
            .Build();

        // zh-SG detects the Singapore NRIC, not China's Resident ID, despite both being zh.
        Assert.Contains(Detect(d, "S1234567D", "zh-SG"), m => m.Type == PiiType.NationalId);
        Assert.DoesNotContain(Detect(d, "110101199001010015", "zh-SG"), m => m.Type == PiiType.NationalId);
        // ...and China still recognises its own ID under zh-CN.
        Assert.Contains(Detect(d, "110101199001010015", "zh-CN"), m => m.Type == PiiType.NationalId);
    }
}
