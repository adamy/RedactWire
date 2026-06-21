// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

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

/// <summary>Default severity per <see cref="PiiType"/>. A rule may override its own
/// severity at construction; this is the fallback when it doesn't.
/// See <c>docs/rules/severity.md</c>.</summary>
public static class PiiSeverities
{
    private static readonly IReadOnlyDictionary<PiiType, PiiSeverity> Map =
        new Dictionary<PiiType, PiiSeverity>
        {
            // Critical — direct identity / financial keys
            [PiiType.SocialSecurity] = PiiSeverity.Critical,
            [PiiType.NationalId]     = PiiSeverity.Critical,
            [PiiType.CreditCard]     = PiiSeverity.Critical,
            [PiiType.BankAccount]    = PiiSeverity.Critical,
            [PiiType.Iban]           = PiiSeverity.Critical,
            [PiiType.Passport]       = PiiSeverity.Critical,
            [PiiType.DriverLicense]  = PiiSeverity.Critical,
            [PiiType.TaxId]          = PiiSeverity.Critical,

            // High — strong contact / locating identifiers
            [PiiType.Email]   = PiiSeverity.High,
            [PiiType.Phone]   = PiiSeverity.High,
            [PiiType.Address] = PiiSeverity.High,
            [PiiType.Person]  = PiiSeverity.High,

            // Medium — quasi-identifiers
            [PiiType.IpAddress]  = PiiSeverity.Medium,
            [PiiType.PostalCode] = PiiSeverity.Medium,

            // Low
            [PiiType.Organization] = PiiSeverity.Low,
        };

    /// <summary>Default severity for a type (Medium if unmapped).</summary>
    public static PiiSeverity For(PiiType type) =>
        Map.TryGetValue(type, out var s) ? s : PiiSeverity.Medium;
}
