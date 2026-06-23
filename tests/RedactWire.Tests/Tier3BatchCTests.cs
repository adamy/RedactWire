// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch C: SA, AR, IL, CL, CO.
public class Tier3BatchCTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Saudi Arabia ─────────────────────────────────────────────────────────────
    [Fact] public void Sa_id_valid() => Assert.Contains(M("ar-SA", "1000000008"), m => m.Type == PiiType.NationalId);
    [Fact] public void Sa_id_invalid() => Assert.DoesNotContain(M("ar-SA", "1000000009"), m => m.Type == PiiType.NationalId);
    [Fact] public void Sa_mobile() => Assert.Contains(M("ar-SA", "0501234567"), m => m.Type == PiiType.Phone);

    // ── Argentina ────────────────────────────────────────────────────────────────
    [Fact] public void Ar_cuit_valid() => Assert.Contains(M("es-AR", "20123456786"), m => m.Type == PiiType.TaxId);
    [Fact] public void Ar_cuit_invalid() => Assert.DoesNotContain(M("es-AR", "20123456780"), m => m.Type == PiiType.TaxId);
    [Fact] public void Ar_dni() => Assert.Contains(M("es-AR", "12.345.678"), m => m.Type == PiiType.NationalId);

    // ── Israel ───────────────────────────────────────────────────────────────────
    [Fact] public void Il_id_valid() => Assert.Contains(M("he-IL", "123456782"), m => m.Type == PiiType.NationalId);
    [Fact] public void Il_id_invalid() => Assert.DoesNotContain(M("he-IL", "123456780"), m => m.Type == PiiType.NationalId);
    [Fact] public void Il_mobile() => Assert.Contains(M("he-IL", "0501234567"), m => m.Type == PiiType.Phone);

    // ── Chile ────────────────────────────────────────────────────────────────────
    [Fact] public void Cl_rut_valid() => Assert.Contains(M("es-CL", "12345678-5"), m => m.Type == PiiType.NationalId);
    [Fact] public void Cl_rut_invalid() => Assert.DoesNotContain(M("es-CL", "12345678-6"), m => m.Type == PiiType.NationalId);
    [Fact] public void Cl_mobile() => Assert.Contains(M("es-CL", "0912345678"), m => m.Type == PiiType.Phone);

    // ── Colombia ─────────────────────────────────────────────────────────────────
    [Fact] public void Co_cedula() => Assert.Contains(M("es-CO", "12345678"), m => m.Type == PiiType.NationalId);
    [Fact] public void Co_mobile() => Assert.Contains(M("es-CO", "+573001234567"), m => m.Type == PiiType.Phone);
}
