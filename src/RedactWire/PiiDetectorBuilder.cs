// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire.Rules;

namespace RedactWire;

// ════════════════════════════════════════════════════════════════════════════
//  BOOTSTRAP  — builder that wires default rules + cultures, and lets the
//  consumer add their own rules without touching the library.
//  (For zero-config usage with no builder, see the static `Redactor` facade.)
// ════════════════════════════════════════════════════════════════════════════

public sealed class PiiDetectorBuilder
{
    private readonly List<IPiiRule> _invariant = new();
    private readonly Dictionary<string, List<IPiiRule>> _culture = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<IPiiRule> _allCultureRules = new();
    private readonly List<CultureInfo> _defaults = new();
    private OverlapStrategy _overlap = OverlapStrategy.KeepHighestConfidence;

    /// <summary>Start with the built-in invariant rules (email, credit card, IP, IBAN).</summary>
    public static PiiDetectorBuilder CreateDefault()
    {
        var b = new PiiDetectorBuilder();
        foreach (var r in DefaultRules.Invariant) b._invariant.Add(r);
        return b;
    }

    /// <summary>Completely empty builder — consumer wires everything.</summary>
    public static PiiDetectorBuilder CreateEmpty() => new();

    /// <summary>Register the built-in rule pack for a culture (currently en-US) and
    /// make it part of the default culture set used when Detect() is called without cultures.</summary>
    public PiiDetectorBuilder AddCulture(CultureInfo culture)
    {
        if (DefaultRules.ByCulture.TryGetValue(culture.Name, out var rules))
        {
            var bucket = GetBucket(culture.Name);
            bucket.AddRange(rules);
        }
        if (!_defaults.Any(c => c.Name == culture.Name)) _defaults.Add(culture);
        return this;
    }

    /// <summary>Add a custom rule for a specific culture (extensibility hook).</summary>
    public PiiDetectorBuilder AddRule(CultureInfo culture, IPiiRule rule)
    {
        GetBucket(culture.Name).Add(rule);
        return this;
    }

    /// <summary>Add a custom rule to every configured culture (those added via
    /// <see cref="AddCulture"/>). Bound at <see cref="Build"/>, so call order vs
    /// <see cref="AddCulture"/> does not matter. For an always-on rule independent of
    /// culture, use <see cref="AddInvariantRule"/> instead.</summary>
    public PiiDetectorBuilder AddRule(IPiiRule rule)
    {
        _allCultureRules.Add(rule);
        return this;
    }

    /// <summary>Add a custom culture-agnostic rule (always runs).</summary>
    public PiiDetectorBuilder AddInvariantRule(IPiiRule rule)
    {
        _invariant.Add(rule);
        return this;
    }

    public PiiDetectorBuilder UseOverlapStrategy(OverlapStrategy strategy)
    {
        _overlap = strategy;
        return this;
    }

    public PiiDetector Build()
    {
        if (_defaults.Count == 0) _defaults.Add(CultureInfo.CurrentCulture);

        // Bind "all configured cultures" rules to every configured culture's bucket.
        if (_allCultureRules.Count > 0)
            foreach (var ci in _defaults)
                GetBucket(ci.Name).AddRange(_allCultureRules);

        var frozen = _culture.ToDictionary(
            kv => kv.Key, kv => (IReadOnlyList<IPiiRule>)kv.Value,
            StringComparer.OrdinalIgnoreCase);
        return new PiiDetector(_invariant, frozen, _defaults, _overlap);
    }

    private List<IPiiRule> GetBucket(string culture)
    {
        if (!_culture.TryGetValue(culture, out var list))
            _culture[culture] = list = new List<IPiiRule>();
        return list;
    }
}
