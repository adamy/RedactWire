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
    /// <summary>Apply redactions to the original input of a result. Matches are applied
    /// right-to-left so earlier indices stay valid; overlaps are skipped.</summary>
    public static string Redact(this PiiResult result, RedactionOptions? options = null)
    {
        options ??= RedactionOptions.Default;
        var ordered = result.AllMatches
            .OrderByDescending(m => m.Start)
            .ThenByDescending(m => m.Length)
            .ToList();

        var sb = new StringBuilder(result.Input);
        int lastStart = result.Input.Length; // track left edge of the last region we edited
        foreach (var m in ordered)
        {
            int end = m.Start + m.Length;
            if (end > lastStart) continue;     // overlaps an already-redacted region → skip
            sb.Remove(m.Start, m.Length);
            sb.Insert(m.Start, Replacement(m, options));
            lastStart = m.Start;
        }
        return sb.ToString();
    }

    private static string Replacement(PiiMatch m, RedactionOptions o)
    {
        if (o.Custom is not null) return o.Custom(m);
        return o.Mode switch
        {
            RedactionMode.Remove => string.Empty,
            RedactionMode.Label => $"[{m.Type}]",
            _ => new string(o.MaskChar, m.Length),
        };
    }
}
