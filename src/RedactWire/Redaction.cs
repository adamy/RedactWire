// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Text;

namespace RedactWire;

public enum RedactionMode
{
    /// <summary>Replace each character of the match with <see cref="RedactionOptions.MaskChar"/>
    /// (length-preserving). "123-45-6789" → "***********".</summary>
    Mask,
    /// <summary>Remove the match entirely.</summary>
    Remove,
    /// <summary>Replace with a type label, e.g. "[SocialSecurity]".</summary>
    Label,
}

public sealed class RedactionOptions
{
    public RedactionMode Mode { get; set; } = RedactionMode.Mask;
    public char MaskChar { get; set; } = '*';
    /// <summary>Custom replacement; receives the match, returns the replacement text.
    /// Overrides <see cref="Mode"/> when set.</summary>
    public Func<PiiMatch, string>? Custom { get; set; }

    public static readonly RedactionOptions Default = new();
}

public static class Redaction
{
    /// <summary>Apply redactions to the original input of a result.
    /// <para>Overlapping matches (which can occur because invariant and culture rules are
    /// resolved independently) are merged into one span so no PII is left partially
    /// exposed. Each merged span is replaced once, using its highest-severity contributor
    /// for the label/custom replacement. Spans are applied right-to-left so indices stay
    /// valid.</para></summary>
    public static string Redact(this PiiResult result, RedactionOptions? options = null)
    {
        options ??= RedactionOptions.Default;

        var matches = result.AllMatches
            .Where(m => m.Length > 0)
            .OrderBy(m => m.Start)
            .ThenByDescending(m => m.Length)
            .ToList();
        if (matches.Count == 0) return result.Input;

        // Merge overlapping/contained spans into a union; keep the strongest match as
        // the representative for labelling.
        var merged = new List<(int Start, int End, PiiMatch Rep)>();
        foreach (var m in matches)
        {
            int start = m.Start, end = m.Start + m.Length;
            if (merged.Count > 0 && start < merged[merged.Count - 1].End)
            {
                var last = merged[merged.Count - 1];
                merged[merged.Count - 1] = (last.Start, Math.Max(last.End, end), Stronger(last.Rep, m));
            }
            else
            {
                merged.Add((start, end, m));
            }
        }

        var sb = new StringBuilder(result.Input);
        for (int i = merged.Count - 1; i >= 0; i--)   // right-to-left
        {
            var span = merged[i];
            int len = span.End - span.Start;
            sb.Remove(span.Start, len);
            sb.Insert(span.Start, Replacement(span.Rep, len, options));
        }
        return sb.ToString();
    }

    // The match that should represent a merged span: highest severity, then longest,
    // then highest confidence.
    private static PiiMatch Stronger(PiiMatch a, PiiMatch b)
    {
        if (b.Severity != a.Severity) return b.Severity > a.Severity ? b : a;
        if (b.Length != a.Length) return b.Length > a.Length ? b : a;
        return b.Confidence > a.Confidence ? b : a;
    }

    private static string Replacement(PiiMatch m, int length, RedactionOptions o)
    {
        if (o.Custom is not null) return o.Custom(m);
        return o.Mode switch
        {
            RedactionMode.Remove => string.Empty,
            RedactionMode.Label => $"[{m.Subtype ?? m.Type.ToString()}]",
            _ => new string(o.MaskChar, length),
        };
    }
}
