// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch A: CA, AU, NZ, NL, PL.
public class Tier3BatchATests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Canada ───────────────────────────────────────────────────────────────────
    [Fact] public void Ca_sin_valid() => Assert.Contains(M("en-CA", "046454286"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ca_sin_invalid() => Assert.DoesNotContain(M("en-CA", "046454287"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ca_postcode() => Assert.Contains(M("en-CA", "K1A 0B1"), m => m.Type == PiiType.PostalCode);

    // ── Australia ────────────────────────────────────────────────────────────────
    [Fact] public void Au_tfn_valid() => Assert.Contains(M("en-AU", "123456782"), m => m.Type == PiiType.TaxId);
    [Fact] public void Au_tfn_invalid() => Assert.DoesNotContain(M("en-AU", "123456780"), m => m.Type == PiiType.TaxId);
    [Fact] public void Au_mobile() => Assert.Contains(M("en-AU", "0412345678"), m => m.Type == PiiType.Phone);

    // ── New Zealand (driver licence is a must-have here) ─────────────────────────
    [Fact] public void Nz_ird_valid() => Assert.Contains(M("en-NZ", "49091850"), m => m.Type == PiiType.TaxId);
    [Fact] public void Nz_ird_invalid() => Assert.DoesNotContain(M("en-NZ", "49091851"), m => m.Type == PiiType.TaxId);
    [Fact] public void Nz_driver_licence() => Assert.Contains(M("en-NZ", "AB123456"), m => m.Type == PiiType.DriverLicense);
    [Fact] public void Nz_nhi_legacy() => Assert.Contains(M("en-NZ", "ABC1234"), m => m.Type == PiiType.Custom && m.Subtype == "NHI");
    [Fact] public void Nz_nhi_2019_form() => Assert.Contains(M("en-NZ", "ABC12DV"), m => m.Type == PiiType.Custom && m.Subtype == "NHI");

    // ── Netherlands ──────────────────────────────────────────────────────────────
    [Fact] public void Nl_bsn_valid() => Assert.Contains(M("nl-NL", "111222333"), m => m.Type == PiiType.NationalId);
    [Fact] public void Nl_bsn_invalid() => Assert.DoesNotContain(M("nl-NL", "111222334"), m => m.Type == PiiType.NationalId);
    [Fact] public void Nl_postcode() => Assert.Contains(M("nl-NL", "1234 AB"), m => m.Type == PiiType.PostalCode);

    // ── Poland ───────────────────────────────────────────────────────────────────
    [Fact] public void Pl_pesel_valid() => Assert.Contains(M("pl-PL", "90010123459"), m => m.Type == PiiType.NationalId);
    [Fact] public void Pl_pesel_invalid() => Assert.DoesNotContain(M("pl-PL", "90010123450"), m => m.Type == PiiType.NationalId);
    [Fact] public void Pl_postcode() => Assert.Contains(M("pl-PL", "00-001"), m => m.Type == PiiType.PostalCode);
}
