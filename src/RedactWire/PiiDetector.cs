// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;

namespace RedactWire;

// ════════════════════════════════════════════════════════════════════════════
//  ENGINE  — resolve cultures, run rules, resolve overlaps.
// ════════════════════════════════════════════════════════════════════════════

public enum OverlapStrategy { KeepHighestConfidence, KeepLongest, KeepAll }

public sealed class PiiDetector
{
    private readonly IReadOnlyList<IPiiRule> _invariantRules;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<IPiiRule>> _cultureRules;
    private readonly IReadOnlyList<CultureInfo> _defaultCultures;
    private readonly OverlapStrategy _overlap;

    internal PiiDetector(
        IReadOnlyList<IPiiRule> invariantRules,
        IReadOnlyDictionary<string, IReadOnlyList<IPiiRule>> cultureRules,
        IReadOnlyList<CultureInfo> defaultCultures,
        OverlapStrategy overlap)
    {
        _invariantRules = invariantRules;
        _cultureRules = cultureRules;
        _defaultCultures = defaultCultures;
        _overlap = overlap;
    }

    public bool HasPii(string text, params CultureInfo[] cultures) => Detect(text, cultures).HasPii;

    public PiiResult Detect(string text, params CultureInfo[] cultures)
    {
        if (text is null) throw new ArgumentNullException(nameof(text));
        var requested = cultures.Length > 0 ? cultures : _defaultCultures;

        // 1) invariant rules always run
        var invMatches = Resolve(RunRules(_invariantRules, text, "invariant"));
        var invariant = new CulturePiiResult("invariant", invMatches,
            _invariantRules.Select(r => r.Name).ToList());

        // 2) per requested culture, with fallback specific -> neutral (en-US -> en)
        var perCulture = new List<CulturePiiResult>();
        foreach (var ci in requested)
        {
            var rules = ResolveRulesFor(ci);
            var name = ci.Name;
            var supported = rules.Count > 0;
            var matches = Resolve(RunRules(rules, text, name));
            perCulture.Add(new CulturePiiResult(name, matches, rules.Select(r => r.Name).ToList())
            {
                Supported = supported,
            });
        }

        return new PiiResult(text, perCulture, invariant);
    }

    // Run rules and stamp each raw hit with culture, rule id, and a severity fallback.
    private static IEnumerable<PiiMatch> RunRules(
        IReadOnlyList<IPiiRule> rules, string text, string culture)
    {
        foreach (var rule in rules)
            foreach (var h in rule.Find(text))
                yield return new PiiMatch(rule.Type, h.Value, h.Start, h.Length, h.Confidence,
                    $"{culture}:{rule.Name}", culture, h.Severity ?? PiiSeverities.For(rule.Type),
                    h.Subtype);
    }

    private IReadOnlyList<IPiiRule> ResolveRulesFor(CultureInfo ci)
    {
        if (_cultureRules.TryGetValue(ci.Name, out var exact)) return exact;       // en-US
        var neutral = ci.TwoLetterISOLanguageName;                                  // en
        if (_cultureRules.TryGetValue(neutral, out var fb)) return fb;
        return Array.Empty<IPiiRule>();
    }

    private IReadOnlyList<PiiMatch> Resolve(IEnumerable<PiiMatch> raw)
    {
        var all = raw.ToList();
        if (_overlap == OverlapStrategy.KeepAll || all.Count < 2) return all;

        // Severity is always the primary key: a higher-severity match wins overlaps
        // even at lower confidence. The chosen strategy only orders matches of
        // equal severity.
        var ordered = _overlap == OverlapStrategy.KeepLongest
            ? all.OrderByDescending(m => m.Severity)
                 .ThenByDescending(m => m.Length).ThenByDescending(m => m.Confidence)
            : all.OrderByDescending(m => m.Severity)
                 .ThenByDescending(m => m.Confidence).ThenByDescending(m => m.Length);

        var kept = new List<PiiMatch>();
        foreach (var m in ordered)
            if (!kept.Any(k => Overlaps(k, m)))
                kept.Add(m);
        return kept.OrderBy(m => m.Start).ToList();
    }

    private static bool Overlaps(PiiMatch a, PiiMatch b) =>
        a.Start < b.Start + b.Length && b.Start < a.Start + a.Length;
}
