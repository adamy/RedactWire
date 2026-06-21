// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Text.RegularExpressions;

namespace RedactWire.Rules.Localized.EnUs;

/// <summary>US street address — a composite rule (not a single regex).
/// Detects a "street line" and a "city, ST ZIP" line, validates the state code,
/// and merges them into one high-confidence <see cref="PiiType.Address"/> span when
/// adjacent. Either part alone is emitted at lower confidence.
/// Implements <see cref="IPiiRule"/> directly so consumers can swap in a richer
/// address recognizer (or a future NER one) via the same builder hook.</summary>
public sealed class AddressRule : IPiiRule
{
    public string Name => "Address";
    public PiiType Type => PiiType.Address;

    private const RegexOptions Opts =
        RegexOptions.Compiled | RegexOptions.CultureInvariant;

    // House number + up to 4 Title-case/numbered name tokens + a (case-insensitive) suffix.
    // Name tokens must start upper/digit so lowercase prose ("...lives at 123 Main St")
    // can't be swallowed into a bogus multi-word street.
    private static readonly Regex Street = new(
        @"\b\d{1,6}\s+(?:[A-Z0-9][A-Za-z0-9.'#-]*\s+){0,4}" +
        @"(?i:St|Street|Ave|Avenue|Blvd|Boulevard|Rd|Road|Ln|Lane|Dr|Drive|" +
        @"Ct|Court|Way|Pl|Place|Ter|Terrace|Cir|Circle)\b\.?",
        Opts);

    // city, ST ZIP  (state validated against UsStates)
    private static readonly Regex CityStateZip = new(
        @"\b[A-Za-z][A-Za-z .'-]*,\s*(?<state>[A-Za-z]{2})\s+\d{5}(?:-\d{4})?\b",
        Opts);

    private const double ConfFull = 0.7;
    private const double ConfStreetOnly = 0.45;
    private const double ConfCityOnly = 0.5;

    public IEnumerable<RuleHit> Find(string text)
    {
        var streets = Street.Matches(text).Cast<Match>().ToList();
        var cities = CityStateZip.Matches(text).Cast<Match>()
            .Where(m => UsStates.Codes.Contains(m.Groups["state"].Value))
            .ToList();

        var usedCity = new bool[cities.Count];

        foreach (var s in streets)
        {
            int sEnd = s.Index + s.Length;
            // a city/state/zip immediately after the street (allow ", " gap) → one address
            int ci = -1;
            for (int i = 0; i < cities.Count; i++)
            {
                if (usedCity[i]) continue;
                int gap = cities[i].Index - sEnd;
                if (gap >= 0 && gap <= 3) { ci = i; break; }
            }
            if (ci >= 0)
            {
                usedCity[ci] = true;
                var c = cities[ci];
                int start = s.Index;
                int len = c.Index + c.Length - start;
                yield return new RuleHit(text.Substring(start, len), start, len, ConfFull);
            }
            else
            {
                yield return new RuleHit(s.Value, s.Index, s.Length, ConfStreetOnly);
            }
        }

        for (int i = 0; i < cities.Count; i++)
        {
            if (usedCity[i]) continue;
            var c = cities[i];
            yield return new RuleHit(c.Value, c.Index, c.Length, ConfCityOnly);
        }
    }
}
