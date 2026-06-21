// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace RedactWire;

/// <summary>Scans an object graph for PII via reflection. Walks public properties and
/// fields recursively, scanning string values and reporting a property-path location
/// (<c>User.Contacts[0].Email</c>).
/// <para>Only string members are scanned. Scalars (numbers, dates, enums, Guid, …) are
/// skipped, and recursion does not descend into framework (<c>System.*</c>/<c>Microsoft.*</c>)
/// types except dictionaries and enumerables. Cycles are detected; depth is capped.</para></summary>
public static class ObjectPiiScanner
{
    private const int MaxDepth = 32;

    private static readonly ConcurrentDictionary<Type, MemberInfo[]> MemberCache = new();

    /// <summary>Detect PII in an arbitrary object graph.</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectObject(
        this PiiDetector detector, object? graph, params CultureInfo[] cultures)
    {
        if (detector is null) throw new ArgumentNullException(nameof(detector));

        var results = new List<StructuredPiiMatch>();
        var visited = new HashSet<object>(ReferenceComparer.Instance);
        Walk(detector, graph, "", 0, cultures, visited, results);
        return results;
    }

    private static void Walk(PiiDetector detector, object? obj, string path, int depth,
        CultureInfo[] cultures, HashSet<object> visited, List<StructuredPiiMatch> sink)
    {
        if (obj is null || depth > MaxDepth) return;

        if (obj is string s)
        {
            if (!string.IsNullOrEmpty(s))
                foreach (var m in detector.Detect(s, cultures).AllMatches)
                    sink.Add(new StructuredPiiMatch(path.Length == 0 ? "$" : path, m));
            return;
        }

        var type = obj.GetType();
        if (IsScalar(type)) return;

        // Reference types only: guard against cycles / shared references.
        if (!type.IsValueType && !visited.Add(obj)) return;

        if (obj is IDictionary dict)
        {
            foreach (DictionaryEntry e in dict)
                Walk(detector, e.Value, $"{path}[{e.Key}]", depth + 1, cultures, visited, sink);
            return;
        }

        if (obj is IEnumerable seq)   // string already handled above
        {
            int i = 0;
            foreach (var item in seq)
                Walk(detector, item, $"{path}[{i++}]", depth + 1, cultures, visited, sink);
            return;
        }

        // Don't reflect into framework types (CultureInfo, Type, etc.).
        if (IsFrameworkType(type)) return;

        foreach (var member in MembersOf(type))
        {
            object? value;
            try
            {
                value = member switch
                {
                    PropertyInfo p => p.GetValue(obj),
                    FieldInfo f => f.GetValue(obj),
                    _ => null,
                };
            }
            catch
            {
                continue;   // getter threw — skip this member
            }

            var childPath = path.Length == 0 ? member.Name : $"{path}.{member.Name}";
            Walk(detector, value, childPath, depth + 1, cultures, visited, sink);
        }
    }

    private static MemberInfo[] MembersOf(Type type) =>
        MemberCache.GetOrAdd(type, static t =>
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var props = t.GetProperties(flags)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .Cast<MemberInfo>();
            var fields = t.GetFields(flags).Cast<MemberInfo>();
            return props.Concat(fields).ToArray();
        });

    private static bool IsScalar(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t.IsPrimitive || t.IsEnum) return true;
        return t == typeof(decimal) || t == typeof(DateTime) || t == typeof(DateTimeOffset)
            || t == typeof(TimeSpan) || t == typeof(Guid) || t == typeof(Uri);
    }

    private static bool IsFrameworkType(Type type)
    {
        var ns = type.Namespace;
        return ns is not null && (ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal)
            || ns == "Microsoft" || ns.StartsWith("Microsoft.", StringComparison.Ordinal));
    }

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Instance = new();
        bool IEqualityComparer<object>.Equals(object? x, object? y) => ReferenceEquals(x, y);
        int IEqualityComparer<object>.GetHashCode(object obj) =>
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}
