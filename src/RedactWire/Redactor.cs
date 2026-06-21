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
}
