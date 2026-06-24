// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// AU Medicare and Malaysia MyKad (filled-in coverage gaps).
public class CoverageGapsTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    [Fact]
    public void Au_medicare_valid() =>
        Assert.Contains(M("en-AU", "2123456701"), m => m.Type == PiiType.Custom && m.Subtype == "Medicare");

    [Fact]
    public void Au_medicare_invalid() =>
        Assert.DoesNotContain(M("en-AU", "2123456711"), m => m.Type == PiiType.Custom);

    [Fact]
    public void My_mykad_valid() =>
        Assert.Contains(M("ms-MY", "900101011234"), m => m.Type == PiiType.NationalId);

    [Fact]
    public void My_mykad_bad_date() =>
        Assert.DoesNotContain(M("ms-MY", "901301011234"), m => m.Type == PiiType.NationalId);

    [Fact]
    public void My_resolves_for_other_malaysian_languages() =>
        Assert.Contains(M("zh-MY", "900101011234"), m => m.Type == PiiType.NationalId);   // region MY
}
