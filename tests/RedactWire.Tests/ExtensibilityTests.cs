// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class ExtensibilityTests
{
    // A minimal custom rule: reports raw RuleHits, no culture/rule-id/severity plumbing.
    private sealed class TokenRule : IPiiRule
    {
        private readonly PiiSeverity? _severity;
        public TokenRule(PiiType type, PiiSeverity? severity = null)
        {
            Type = type;
            _severity = severity;
        }

        public string Name => "Token";
        public PiiType Type { get; }

        public IEnumerable<RuleHit> Find(string text)
        {
            int i = text.IndexOf("TOKEN", StringComparison.Ordinal);
            if (i >= 0) yield return new RuleHit("TOKEN", i, 5, 0.9, _severity);
        }
    }

    private static CultureInfo Ci(string n) => new(n);

    [Fact]
    public void Custom_rule_per_culture_is_stamped_by_engine()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(Ci("en-US"), new TokenRule(PiiType.Email))
            .Build();

        var m = Assert.Single(d.Detect("see TOKEN here", Ci("en-US")).Cultures.Single().Matches);
        Assert.Equal("en-US:Token", m.Rule);   // engine formatted the rule id
        Assert.Equal("en-US", m.Culture);       // engine stamped the culture
        Assert.Equal(PiiType.Email, m.Type);
    }

    [Fact]
    public void Custom_rule_severity_defaults_from_type()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(Ci("en-US"), new TokenRule(PiiType.NationalId))  // no severity given
            .Build();

        var m = Assert.Single(d.Detect("TOKEN", Ci("en-US")).Cultures.Single().Matches);
        Assert.Equal(PiiSeverity.Critical, m.Severity);   // NationalId default
    }

    [Fact]
    public void Custom_rule_can_override_severity()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(Ci("en-US"), new TokenRule(PiiType.Email, PiiSeverity.Low))
            .Build();

        var m = Assert.Single(d.Detect("TOKEN", Ci("en-US")).Cultures.Single().Matches);
        Assert.Equal(PiiSeverity.Low, m.Severity);   // overrides Email's High default
    }

    [Fact]
    public void AddRule_binds_to_all_configured_cultures()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddCulture(Ci("en-US"))
            .AddCulture(Ci("fr-FR"))
            .AddRule(new TokenRule(PiiType.Email))   // all configured cultures
            .Build();

        var r = d.Detect("TOKEN");   // defaults = en-US + fr-FR
        Assert.All(r.Cultures, c => Assert.Contains(c.Matches, m => m.Rule.EndsWith(":Token")));
    }

    [Fact]
    public void AddRule_all_cultures_is_order_independent()
    {
        // AddRule called before AddCulture still binds (deferred to Build).
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(new TokenRule(PiiType.Email))
            .AddCulture(Ci("en-US"))
            .Build();

        var m = Assert.Single(d.Detect("TOKEN", Ci("en-US")).Cultures.Single().Matches);
        Assert.Equal("en-US:Token", m.Rule);
    }
}
