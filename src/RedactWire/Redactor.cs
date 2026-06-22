// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;

namespace RedactWire;

/// <summary>Zero-config static facade. Use this when you don't want to build a detector
/// or wire any .NET host/DI pipeline:
/// <code>
/// var r = Redactor.Detect("My SSN is 123-45-6789");
/// bool hit = Redactor.HasPii(text);
/// string clean = Redactor.Redact(text);
/// </code>
/// Backed by a lazily-initialized, thread-safe default detector (invariant rules + en-US).
/// For culture/rule customization, use <see cref="PiiDetectorBuilder"/> instead.
/// <para>Named <c>Redactor</c> rather than <c>RedactWire</c> to avoid clashing with the
/// namespace of the same name.</para></summary>
public static class Redactor
{
    private static readonly Lazy<PiiDetector> _default = new(() =>
        PiiDetectorBuilder.CreateDefault()
            .AddCulture(new CultureInfo("en-US"))
            .Build());

    /// <summary>The shared default detector (invariant + en-US).</summary>
    public static PiiDetector Default => _default.Value;

    public static PiiResult Detect(string text, params CultureInfo[] cultures) =>
        Default.Detect(text, cultures);

    public static bool HasPii(string text, params CultureInfo[] cultures) =>
        Default.HasPii(text, cultures);

    public static string Redact(string text, RedactionOptions? options = null,
        params CultureInfo[] cultures) =>
        Default.Detect(text, cultures).Redact(options);

    /// <summary>Validate that a string is, in full, a valid PII item of the given type
    /// (using the default detector's cultures: invariant + en-US plus any others built in).</summary>
    public static ValidationResult Validate(string value, PiiType type, string? subtype = null) =>
        Default.Validate(value, type, subtype);

    /// <summary>Validate against one explicit culture.</summary>
    public static ValidationResult Validate(string value, CultureInfo culture, PiiType type, string? subtype = null) =>
        Default.Validate(value, culture, type, subtype);

    /// <summary>Scan a JSON string; matches are located by JSONPath.</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectJson(string json, params CultureInfo[] cultures) =>
        Default.DetectJson(json, cultures);

    /// <summary>Scan an XML string; matches are located by XPath. Parsed safely (no XXE).</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectXml(string xml, params CultureInfo[] cultures) =>
        Default.DetectXml(xml, cultures);

    /// <summary>Scan an object graph; matches are located by property path.</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectObject(object? graph, params CultureInfo[] cultures) =>
        Default.DetectObject(graph, cultures);
}
