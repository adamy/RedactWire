// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Reflection;

namespace RedactWire;

/// <summary>How sensitive a PII type is. Drives overlap resolution: a higher-severity
/// match wins over a lower-severity one on the same span, even at lower confidence.
/// Higher enum value = more severe.</summary>
public enum PiiSeverity
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3,
}

/// <summary>Declares the default <see cref="PiiSeverity"/> for a <see cref="PiiType"/>
/// member. Kept on the enum so the severity lives next to the value it describes.</summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class DefaultSeverityAttribute : Attribute
{
    public DefaultSeverityAttribute(PiiSeverity severity) => Severity = severity;
    public PiiSeverity Severity { get; }
}

/// <summary>Resolves the default severity for a <see cref="PiiType"/> from its
/// <see cref="DefaultSeverityAttribute"/>. A rule may override per match
/// (<see cref="RuleHit.Severity"/>); this is the fallback when it doesn't.
/// See <c>docs/rules/severity.md</c>.</summary>
public static class PiiSeverities
{
    // Read the attributes once (reflection) and cache, so For() stays O(1).
    private static readonly IReadOnlyDictionary<PiiType, PiiSeverity> Map = BuildMap();

    /// <summary>Default severity for a type (Medium if none is declared).</summary>
    public static PiiSeverity For(PiiType type) =>
        Map.TryGetValue(type, out var s) ? s : PiiSeverity.Medium;

    private static Dictionary<PiiType, PiiSeverity> BuildMap()
    {
        var map = new Dictionary<PiiType, PiiSeverity>();
        foreach (var field in typeof(PiiType).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = (PiiType)field.GetValue(null)!;
            var attr = field.GetCustomAttribute<DefaultSeverityAttribute>();
            map[value] = attr?.Severity ?? PiiSeverity.Medium;
        }
        return map;
    }
}
