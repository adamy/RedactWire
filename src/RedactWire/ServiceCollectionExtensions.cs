// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System;
using RedactWire;

// Placed in the Microsoft.Extensions.DependencyInjection namespace (the idiomatic spot
// for DI registration helpers) so AddRedactWire is discoverable without an extra using.
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Pipeline bootstrap for apps using Microsoft DI (ASP.NET Core, generic host).
/// Registers a single shared <see cref="PiiDetector"/>.
/// <para>The library does not require this — it works with no bootstrap via the static
/// <see cref="Redactor"/> facade. Use this when you'd rather inject <see cref="PiiDetector"/>.</para></summary>
public static class RedactWireServiceCollectionExtensions
{
    /// <summary>Register a singleton <see cref="PiiDetector"/>. The builder starts from
    /// <see cref="PiiDetectorBuilder.CreateDefault"/> (invariant rules); use
    /// <paramref name="configure"/> to add cultures, rules, or an overlap strategy.</summary>
    /// <example>
    /// <code>
    /// services.AddRedactWire(b => b.AddCulture(new CultureInfo("en-US")));
    /// </code>
    /// </example>
    public static IServiceCollection AddRedactWire(
        this IServiceCollection services,
        Action<PiiDetectorBuilder>? configure = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var builder = PiiDetectorBuilder.CreateDefault();
        configure?.Invoke(builder);
        var detector = builder.Build();   // immutable + thread-safe → safe as a singleton

        services.AddSingleton(detector);
        return services;
    }
}
