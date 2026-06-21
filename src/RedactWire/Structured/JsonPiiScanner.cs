// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace RedactWire;

/// <summary>Scans JSON for PII. Walks every string value and reports each match with a
/// JSONPath-like location (<c>$.user.contacts[0].email</c>).
/// <para>Only string values are scanned. Numbers/booleans are ignored — a value that was
/// truly PII (e.g. an SSN) loses its formatting once stored as a JSON number, so regex
/// detection there is unreliable by design.</para></summary>
public static class JsonPiiScanner
{
    /// <summary>Detect PII in a JSON string. Throws <see cref="JsonException"/> on invalid JSON.</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectJson(
        this PiiDetector detector, string json, params CultureInfo[] cultures)
    {
        if (detector is null) throw new ArgumentNullException(nameof(detector));
        if (json is null) throw new ArgumentNullException(nameof(json));

        var results = new List<StructuredPiiMatch>();
        using var doc = JsonDocument.Parse(json);
        Walk(detector, doc.RootElement, "$", cultures, results);
        return results;
    }

    private static void Walk(PiiDetector detector, JsonElement el, string path,
        CultureInfo[] cultures, List<StructuredPiiMatch> sink)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in el.EnumerateObject())
                    Walk(detector, prop.Value, $"{path}.{prop.Name}", cultures, sink);
                break;

            case JsonValueKind.Array:
                int i = 0;
                foreach (var item in el.EnumerateArray())
                    Walk(detector, item, $"{path}[{i++}]", cultures, sink);
                break;

            case JsonValueKind.String:
                var s = el.GetString();
                if (!string.IsNullOrEmpty(s))
                    foreach (var m in detector.Detect(s!, cultures).AllMatches)
                        sink.Add(new StructuredPiiMatch(path, m));
                break;

            // Numbers / booleans / null are not scanned (see class summary).
        }
    }
}
