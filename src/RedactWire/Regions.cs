// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;

namespace RedactWire;

/// <summary>Maps a culture to its ISO-3166 country/region. PII rule packs are keyed by
/// region, not language, so every culture of a country (en-IN, hi-IN, ta-IN; en-CA, fr-CA;
/// de-CH, fr-CH, it-CH) resolves to the same pack.</summary>
internal static class Regions
{
    /// <summary>Two-letter region for a culture name, or null for neutral/invariant cultures
    /// (and anything without a country, which then matches only the invariant rules).</summary>
    public static string? Of(string? cultureName)
    {
        if (string.IsNullOrEmpty(cultureName)) return null;
        try
        {
            // RegionInfo accepts "en-IN", "zh-Hans-CN", and bare "IN"; throws for neutral
            // cultures like "en" — which is exactly what we want to treat as "no region".
            return new RegionInfo(cultureName).TwoLetterISORegionName;
        }
        catch
        {
            return null;
        }
    }
}
