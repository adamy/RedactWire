// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Text.RegularExpressions;

namespace RedactWire;

// ════════════════════════════════════════════════════════════════════════════
//  RULE ABSTRACTION  — a rule just finds raw hits in text.
//  The engine stamps each hit with the owning culture, the rule id, and a
//  severity fallback, so authors never touch that plumbing.
//  Most rules are RegexRule; composite rules (e.g. Address) implement IPiiRule.
// ════════════════════════════════════════════════════════════════════════════

/// <summary>A raw detection produced by a rule: just what it found and where.
/// The engine turns this into a <see cref="PiiMatch"/> (adding culture + rule id, and
/// filling <see cref="PiiMatch.Severity"/> from the type default when
/// <see cref="Severity"/> is null).</summary>
/// <param name="Value">The matched substring.</param>
/// <param name="Start">Start index in the scanned text.</param>
/// <param name="Length">Length of the match.</param>
/// <param name="Confidence">Confidence in [0,1].</param>
/// <param name="Severity">Optional override; null = use the type's default severity.</param>
/// <param name="Subtype">Optional human name for the match, mainly for
/// <see cref="PiiType.Custom"/> (e.g. "NhiNumber"). Surfaces in redaction labels.</param>
public readonly record struct RuleHit(
    string Value, int Start, int Length, double Confidence,
    PiiSeverity? Severity = null, string? Subtype = null);

/// <summary>A detection rule. The single extension point: consumers and the future
/// NER package implement this. A rule reports raw <see cref="RuleHit"/>s; it does not
/// deal with culture tagging, rule-id formatting, or severity defaults.</summary>
public interface IPiiRule
{
    /// <summary>Unique within a pack, e.g. "SSN".</summary>
    string Name { get; }
    PiiType Type { get; }
    /// <summary>Find every match in <paramref name="text"/>.</summary>
    IEnumerable<RuleHit> Find(string text);
}

/// <summary>Regex rule with an optional validator (for checksum/semantic gating).
/// Validator returns (isValid, confidence) so a Luhn/checksum pass can boost confidence
/// and a fail can drop the candidate entirely.</summary>
public sealed class RegexRule : IPiiRule
{
    private readonly Regex _re;
    private readonly Func<string, (bool ok, double confidence)>? _validate;
    private readonly double _baseConfidence;
    private readonly PiiSeverity? _severity;
    private readonly string? _subtype;

    public string Name { get; }
    public PiiType Type { get; }

    public RegexRule(string name, PiiType type, string pattern,
        double baseConfidence = 0.6,
        Func<string, (bool, double)>? validate = null,
        PiiSeverity? severity = null,
        string? subtype = null,
        RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant)
    {
        Name = name;
        Type = type;
        _re = new Regex(pattern, options);
        _baseConfidence = baseConfidence;
        _validate = validate;
        _severity = severity;   // null → engine fills from the type default
        _subtype = subtype;
    }

    public IEnumerable<RuleHit> Find(string text)
    {
        foreach (Match m in _re.Matches(text))
        {
            // Use a capture group named "v" if present, else the whole match.
            var g = m.Groups["v"].Success ? m.Groups["v"] : (Group)m;
            double confidence = _baseConfidence;

            if (_validate is not null)
            {
                var (ok, conf) = _validate(g.Value);
                if (!ok) continue;          // failed checksum → not PII, skip
                confidence = conf;
            }

            yield return new RuleHit(g.Value, g.Index, g.Length, confidence, _severity, _subtype);
        }
    }
}
