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

    /// <summary>Validate that <paramref name="value"/> is, in its entirety, a valid PII
    /// item of <paramref name="type"/> — running the same rules (and checksums) used for
    /// detection against the detector's configured cultures. Surrounding whitespace is
    /// ignored; the value must match a rule end-to-end (not just contain one).
    /// <para>For <see cref="PiiType.Custom"/>, pass <paramref name="subtype"/> (e.g.
    /// "SNILS") to require a specific custom type.</para></summary>
    public ValidationResult Validate(string value, PiiType type, string? subtype = null) =>
        ValidateCore(value, type, subtype, _defaultCultures);

    /// <summary>As <see cref="Validate(string, PiiType, string?)"/>, but validating against
    /// one explicit <paramref name="culture"/> instead of the detector's configured set.</summary>
    public ValidationResult Validate(string value, CultureInfo culture, PiiType type, string? subtype = null) =>
        ValidateCore(value, type, subtype, new[] { culture });

    private ValidationResult ValidateCore(
        string value, PiiType type, string? subtype, IReadOnlyList<CultureInfo> cultures)
    {
        var v = (value ?? string.Empty).Trim();
        bool anyRule = false;

        foreach (var rule in Candidates(type, cultures))
        {
            anyRule = true;
            if (v.Length == 0) continue;
            foreach (var h in rule.Find(v))
                if (h.Start == 0 && h.Length == v.Length
                    && (subtype is null
                        || string.Equals(h.Subtype, subtype, StringComparison.OrdinalIgnoreCase)))
                    return ValidationResult.Valid;
        }

        // A rule of this type exists → the value is simply Invalid; otherwise Unsupported.
        return anyRule ? ValidationResult.Invalid : ValidationResult.Unsupported;
    }

    // Rules that could produce a match of the requested type: invariant + the culture packs.
    private IEnumerable<IPiiRule> Candidates(PiiType type, IReadOnlyList<CultureInfo> cultures)
    {
        foreach (var r in _invariantRules) if (r.Type == type) yield return r;
        foreach (var ci in cultures)
            foreach (var r in ResolveRulesFor(ci)) if (r.Type == type) yield return r;
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
