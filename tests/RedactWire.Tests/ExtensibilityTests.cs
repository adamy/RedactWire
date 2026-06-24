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
        public string? Subtype => null;

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

    // A consumer PII type that doesn't fit the enum: use PiiType.Custom + a Subtype name.
    private sealed class NhiRule : IPiiRule
    {
        public string Name => "NhiNumber";
        public PiiType Type => PiiType.Custom;
        public string? Subtype => "NhiNumber";

        public IEnumerable<RuleHit> Find(string text)
        {
            int i = text.IndexOf("ABC1234", StringComparison.Ordinal);
            if (i >= 0)
                yield return new RuleHit("ABC1234", i, 7, 0.9,
                    Severity: PiiSeverity.Critical, Subtype: "NhiNumber");
        }
    }

    [Fact]
    public void Custom_type_carries_subtype_and_severity()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(Ci("en-NZ"), new NhiRule())
            .Build();

        var m = Assert.Single(d.Detect("patient ABC1234", Ci("en-NZ")).Cultures.Single().Matches);
        Assert.Equal(PiiType.Custom, m.Type);
        Assert.Equal("NhiNumber", m.Subtype);
        Assert.Equal(PiiSeverity.Critical, m.Severity);
    }

    [Fact]
    public void Custom_type_redaction_label_uses_subtype()
    {
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddRule(Ci("en-NZ"), new NhiRule())
            .Build();

        var redacted = d.Detect("patient ABC1234", Ci("en-NZ"))
            .Redact(new RedactionOptions { Mode = RedactionMode.Label });
        Assert.Equal("patient [NhiNumber]", redacted);
    }

    [Fact]
    public void Build_is_idempotent()
    {
        var builder = PiiDetectorBuilder.CreateEmpty()
            .AddCulture(Ci("en-US"))
            .AddRule(new TokenRule(PiiType.Email));

        builder.Build();                 // first build
        var d = builder.Build();         // second build must not double-bind the rule

        var matches = d.Detect("TOKEN", Ci("en-US")).Cultures.Single().Matches;
        Assert.Single(matches);
    }

    [Fact]
    public void Redaction_merges_partial_overlaps_no_leak()
    {
        // Invariant rule matches [0,5); a culture rule matches [2,8). Their union is the
        // whole string; redaction must mask all of it, not leave the [0,2) prefix exposed.
        var d = PiiDetectorBuilder.CreateEmpty()
            .AddInvariantRule(new RegexRule("Inv", PiiType.Email, "(?<v>X{5})", baseConfidence: 0.9))
            .AddRule(Ci("en-US"),
                new RegexRule("Cul", PiiType.Phone, "(?<v>X{3}Y{3})", baseConfidence: 0.9))
            .Build();

        var redacted = d.Detect("XXXXXYYY", Ci("en-US")).Redact();
        Assert.Equal("********", redacted);   // 8 chars, fully masked
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
