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
    // Buckets are keyed by ISO-3166 region (country), so every culture of a country shares
    // one pack. Cultures without a region fall back to keying by the culture name.
    private readonly Dictionary<string, List<IPiiRule>> _culture = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<IPiiRule> _allCultureRules = new();
    private readonly List<CultureInfo> _defaults = new();
    private OverlapStrategy _overlap = OverlapStrategy.KeepHighestConfidence;

    /// <summary>Culture codes that ship with a built-in rule pack (e.g. "en-US").
    /// Pass any of these to <see cref="AddCulture"/>. Grows as packs are added — use this
    /// instead of hard-coding the list in UIs/CLIs.</summary>
    public static IReadOnlyList<string> AvailableCultures { get; } =
        DefaultRules.ByCulture.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToArray();

    /// <summary>Start with the built-in invariant rules (email, credit card, IP, IBAN).</summary>
    public static PiiDetectorBuilder CreateDefault()
    {
        var b = new PiiDetectorBuilder();
        foreach (var r in DefaultRules.Invariant) b._invariant.Add(r);
        return b;
    }

    /// <summary>Completely empty builder — consumer wires everything.</summary>
    public static PiiDetectorBuilder CreateEmpty() => new();

    /// <summary>Register the built-in rule pack for a culture's country and make it part of
    /// the default culture set used when Detect() is called without cultures. The pack is
    /// resolved by region, so any culture of a country (en-IN/hi-IN, en-CA/fr-CA, de-CH/fr-CH)
    /// loads the same pack.</summary>
    public PiiDetectorBuilder AddCulture(CultureInfo culture)
    {
        var key = KeyFor(culture);
        if (DefaultRules.ByRegion.TryGetValue(key, out var rules))
            GetBucket(key).AddRange(rules);
        if (!_defaults.Any(c => c.Name == culture.Name)) _defaults.Add(culture);
        return this;
    }

    /// <summary>Add a custom rule for a specific culture's country (extensibility hook).
    /// Bound by region, so it applies to every culture of that country.</summary>
    public PiiDetectorBuilder AddRule(CultureInfo culture, IPiiRule rule)
    {
        GetBucket(KeyFor(culture)).Add(rule);
        return this;
    }

    // Region (country) is the bucket key; cultures without a region key by their name.
    private static string KeyFor(CultureInfo culture) => Regions.Of(culture.Name) ?? culture.Name;

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

    /// <summary>Enable built-in secret/credential detection (API keys, tokens, private keys).
    /// These are country-agnostic, so they run as invariant rules. Matches are
    /// <see cref="PiiType.Secret"/> with the provider in <see cref="PiiMatch.Subtype"/>.</summary>
    public PiiDetectorBuilder AddSecretDetection()
    {
        foreach (var r in Rules.Secrets.SecretRules.Rules) _invariant.Add(r);
        return this;
    }

    /// <summary>Remove every already-registered rule (built-in or custom) matching
    /// <paramref name="predicate"/>, across invariant, all cultures, and all-culture rules.
    /// Call after the <c>Add*</c> that loaded the rules (e.g. after <see cref="AddCulture"/>).</summary>
    public PiiDetectorBuilder RemoveRules(Func<IPiiRule, bool> predicate)
    {
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));
        _invariant.RemoveAll(r => predicate(r));
        foreach (var bucket in _culture.Values) bucket.RemoveAll(r => predicate(r));
        _allCultureRules.RemoveAll(r => predicate(r));
        return this;
    }

    /// <summary>Remove built-in/custom rules by <paramref name="type"/> (and optional
    /// <paramref name="subtype"/>) — e.g. <c>RemoveRule(PiiType.PostalCode)</c> or
    /// <c>RemoveRule(PiiType.Secret, "OpenAiKey")</c>.</summary>
    public PiiDetectorBuilder RemoveRule(PiiType type, string? subtype = null) =>
        RemoveRules(r => r.Type == type
            && (subtype is null || string.Equals(r.Subtype, subtype, StringComparison.OrdinalIgnoreCase)));

    /// <summary>Replace an invariant rule with your own: removes any invariant rule sharing
    /// <paramref name="rule"/>.Name, then adds it. (e.g. swap the built-in Email rule.)</summary>
    public PiiDetectorBuilder ReplaceInvariantRule(IPiiRule rule)
    {
        _invariant.RemoveAll(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
        _invariant.Add(rule);
        return this;
    }

    /// <summary>Replace a rule for a culture's country with your own: removes any rule in
    /// that region sharing <paramref name="rule"/>.Name, then adds it.</summary>
    public PiiDetectorBuilder ReplaceRule(CultureInfo culture, IPiiRule rule)
    {
        var bucket = GetBucket(KeyFor(culture));
        bucket.RemoveAll(r => string.Equals(r.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
        bucket.Add(rule);
        return this;
    }

    public PiiDetectorBuilder UseOverlapStrategy(OverlapStrategy strategy)
    {
        _overlap = strategy;
        return this;
    }

    public PiiDetector Build()
    {
        // Build into fresh collections so Build() is idempotent (callable more than once
        // without accumulating rules/cultures on the builder).
        var defaults = _defaults.Count == 0
            ? new List<CultureInfo> { CultureInfo.CurrentCulture }
            : new List<CultureInfo>(_defaults);

        var buckets = _culture.ToDictionary(
            kv => kv.Key, kv => new List<IPiiRule>(kv.Value),
            StringComparer.OrdinalIgnoreCase);

        // Bind "all configured cultures" rules to every configured country's bucket.
        if (_allCultureRules.Count > 0)
            foreach (var ci in defaults)
            {
                var key = KeyFor(ci);
                if (!buckets.TryGetValue(key, out var list))
                    buckets[key] = list = new List<IPiiRule>();
                list.AddRange(_allCultureRules);
            }

        var frozen = buckets.ToDictionary(
            kv => kv.Key, kv => (IReadOnlyList<IPiiRule>)kv.Value,
            StringComparer.OrdinalIgnoreCase);
        return new PiiDetector(_invariant, frozen, defaults, _overlap);
    }

    private List<IPiiRule> GetBucket(string culture)
    {
        if (!_culture.TryGetValue(culture, out var list))
            _culture[culture] = list = new List<IPiiRule>();
        return list;
    }
}
