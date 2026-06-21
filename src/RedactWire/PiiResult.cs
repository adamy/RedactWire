// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire;

// ════════════════════════════════════════════════════════════════════════════
//  RESULTS  — grouped by culture, so callers can see which rule-set hit what.
// ════════════════════════════════════════════════════════════════════════════

/// <summary>Top-level result. Holds the input plus a breakdown per culture that ran,
/// plus the culture-agnostic (invariant) matches that always run.</summary>
public sealed record PiiResult(
    string Input,
    IReadOnlyList<CulturePiiResult> Cultures,   // one entry per requested culture
    CulturePiiResult Invariant)                 // email/card/ip... always evaluated
{
    /// <summary>True if any rule (invariant or culture) matched.</summary>
    public bool HasPii =>
        Invariant.Matches.Count > 0 || Cultures.Any(c => c.Matches.Count > 0);

    /// <summary>Flat view across everything, if the caller just wants all hits.</summary>
    public IEnumerable<PiiMatch> AllMatches =>
        Invariant.Matches.Concat(Cultures.SelectMany(c => c.Matches));
}

/// <summary>Per-culture outcome. Tells you which rules ran and which matched —
/// e.g. en-US ran SSN + Phone, SSN matched, Phone did not.</summary>
public sealed record CulturePiiResult(
    string Culture,                          // "en-US", "invariant", ...
    IReadOnlyList<PiiMatch> Matches,
    IReadOnlyList<string> RulesEvaluated)     // names of every rule that ran for this culture
{
    /// <summary>True only if a rule pack was found for the requested culture.
    /// Distinguishes "checked, found nothing" from "this country isn't covered yet".</summary>
    public bool Supported { get; init; } = true;

    /// <summary>"Passed" = the culture is supported AND no PII was detected.
    /// An unsupported culture never "passes" — we cannot make that claim.</summary>
    public bool Passed => Supported && Matches.Count == 0;
}

/// <summary>A single detected PII span.</summary>
public sealed record PiiMatch(
    PiiType Type,
    string Value,
    int Start,
    int Length,
    double Confidence,
    string Rule,            // "en-US:SSN", "invariant:Email", ...
    string Culture,         // owning culture, or "invariant"
    PiiSeverity Severity);  // how sensitive — drives overlap resolution

public enum PiiType
{
    Phone, NationalId, DriverLicense, Passport, TaxId, PostalCode, BankAccount,
    Email, CreditCard, IpAddress, Iban,
    SocialSecurity,                 // SSN — distinct from TaxId
    Address,                        // active (composite regex, US)
    Person, Organization            // semantic — reserved for the optional NER ext
}
