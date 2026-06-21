// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Text.RegularExpressions;

namespace RedactWire;

// ════════════════════════════════════════════════════════════════════════════
//  RULE ABSTRACTION  — a rule is just "find matches in text".
//  Most rules are RegexRule; checksum rules add a validation predicate.
//  Composite rules (e.g. Address) implement IPiiRule directly.
// ════════════════════════════════════════════════════════════════════════════

/// <summary>A detection rule. The single extension point: consumers and the future
/// NER package both implement this, so the engine never changes.</summary>
public interface IPiiRule
{
    /// <summary>Unique within a pack, e.g. "SSN".</summary>
    string Name { get; }
    PiiType Type { get; }
    /// <summary>Find every match in <paramref name="text"/>; <paramref name="culture"/>
    /// is the owning culture name (or "invariant"), used to tag matches.</summary>
    IEnumerable<PiiMatch> Find(string text, string culture);
}

/// <summary>Regex rule with an optional validator (for checksum/semantic gating).
/// Validator returns (isValid, confidence) so a Luhn/checksum pass can boost confidence
/// and a fail can drop the candidate entirely.</summary>
public sealed class RegexRule : IPiiRule
{
    private readonly Regex _re;
    private readonly Func<string, (bool ok, double confidence)>? _validate;
    private readonly double _baseConfidence;
    private readonly PiiSeverity _severity;

    public string Name { get; }
    public PiiType Type { get; }

    public RegexRule(string name, PiiType type, string pattern,
        double baseConfidence = 0.6,
        Func<string, (bool, double)>? validate = null,
        PiiSeverity? severity = null,
        RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant)
    {
        Name = name;
        Type = type;
        _re = new Regex(pattern, options);
        _baseConfidence = baseConfidence;
        _validate = validate;
        _severity = severity ?? PiiSeverities.For(type);
    }

    public IEnumerable<PiiMatch> Find(string text, string culture)
    {
        foreach (Match m in _re.Matches(text))
        {
            // Use a capture group named "v" if present, else the whole match.
            var g = m.Groups["v"].Success ? m.Groups["v"] : (Group)m;
            var value = g.Value;
            double confidence = _baseConfidence;

            if (_validate is not null)
            {
                var (ok, conf) = _validate(value);
                if (!ok) continue;          // failed checksum → not PII, skip
                confidence = conf;
            }

            yield return new PiiMatch(Type, value, g.Index, g.Length, confidence,
                $"{culture}:{Name}", culture, _severity);
        }
    }
}
