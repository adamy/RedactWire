// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class DetectorBehaviorTests
{
    [Fact]
    public void Static_facade_works_without_bootstrap()
    {
        var r = Redactor.Detect("My SSN is 123-45-6789");
        Assert.True(r.HasPii);
        Assert.Contains(r.AllMatches, m => m.Type == PiiType.SocialSecurity);
    }

    [Fact]
    public void AvailableCultures_lists_builtin_packs()
    {
        Assert.Contains("en-US", PiiDetectorBuilder.AvailableCultures);
    }

    [Fact]
    public void Unsupported_culture_does_not_pass()
    {
        var detector = PiiDetectorBuilder.CreateDefault().Build();
        var r = detector.Detect("hello", new CultureInfo("fr-FR")); // no fr pack
        var fr = r.Cultures.Single();
        Assert.False(fr.Supported);
        Assert.False(fr.Passed);   // unsupported never "passes"
    }

    [Fact]
    public void Supported_culture_with_no_pii_passes()
    {
        var detector = PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();
        var r = detector.Detect("nothing sensitive here", new CultureInfo("en-US"));
        var us = r.Cultures.Single();
        Assert.True(us.Supported);
        Assert.True(us.Passed);
    }

    [Fact]
    public void Ssn_is_critical_severity()
    {
        var m = Redactor.Detect("123-45-6789").AllMatches.Single(x => x.Type == PiiType.SocialSecurity);
        Assert.Equal(PiiSeverity.Critical, m.Severity);
    }

    [Fact]
    public void Higher_severity_wins_overlap_even_at_lower_confidence()
    {
        // Two rules match the exact same span: a High-severity one at conf 0.5 and a
        // Medium-severity one at conf 0.9. Severity-first resolution keeps the High one.
        var detector = PiiDetectorBuilder.CreateEmpty()
            .AddInvariantRule(new RegexRule("Hi", PiiType.Email, "(?<v>TOKEN)", baseConfidence: 0.5))
            .AddInvariantRule(new RegexRule("Lo", PiiType.IpAddress, "(?<v>TOKEN)", baseConfidence: 0.9))
            .Build();
        var m = Assert.Single(detector.Detect("TOKEN").AllMatches);
        Assert.Equal(PiiType.Email, m.Type);        // High beat Medium
        Assert.Equal(PiiSeverity.High, m.Severity);
    }

    [Fact]
    public void Redact_label_mode()
    {
        var r = Redactor.Detect("My SSN is 123-45-6789");
        Assert.Equal("My SSN is [SocialSecurity]", r.Redact(new RedactionOptions { Mode = RedactionMode.Label }));
    }

    [Fact]
    public void Redact_mask_is_length_preserving()
    {
        var r = Redactor.Detect("SSN 123-45-6789");
        Assert.Equal("SSN ***********", r.Redact()); // 11 chars masked
    }
}
