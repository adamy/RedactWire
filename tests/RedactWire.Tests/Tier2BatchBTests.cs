// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 2 batch B: EG, IR, TH, KR, IT.
public class Tier2BatchBTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Egypt ────────────────────────────────────────────────────────────────────
    [Fact] public void Eg_id_valid() => Assert.Contains(M("ar-EG", "29001012112345"), m => m.Type == PiiType.NationalId);
    [Fact] public void Eg_id_bad_month() => Assert.DoesNotContain(M("ar-EG", "29013012112345"), m => m.Type == PiiType.NationalId);
    [Fact] public void Eg_mobile() => Assert.Contains(M("ar-EG", "01012345678"), m => m.Type == PiiType.Phone);

    // ── Iran ─────────────────────────────────────────────────────────────────────
    [Fact] public void Ir_id_valid() => Assert.Contains(M("fa-IR", "1234567891"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ir_id_invalid() => Assert.DoesNotContain(M("fa-IR", "1234567890"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ir_mobile() => Assert.Contains(M("fa-IR", "09123456789"), m => m.Type == PiiType.Phone);

    // ── Thailand ─────────────────────────────────────────────────────────────────
    [Fact] public void Th_id_valid() => Assert.Contains(M("th-TH", "1234567890121"), m => m.Type == PiiType.NationalId);
    [Fact] public void Th_id_invalid() => Assert.DoesNotContain(M("th-TH", "1234567890120"), m => m.Type == PiiType.NationalId);
    [Fact] public void Th_mobile() => Assert.Contains(M("th-TH", "0812345678"), m => m.Type == PiiType.Phone);

    // ── South Korea ──────────────────────────────────────────────────────────────
    [Fact] public void Kr_rrn_valid() => Assert.Contains(M("ko-KR", "900101-1234568"), m => m.Type == PiiType.NationalId);
    [Fact] public void Kr_rrn_invalid() => Assert.DoesNotContain(M("ko-KR", "900101-1234560"), m => m.Type == PiiType.NationalId);
    [Fact] public void Kr_mobile() => Assert.Contains(M("ko-KR", "010-1234-5678"), m => m.Type == PiiType.Phone);

    // ── Italy (find the valid check char, then confirm a wrong one fails) ─────────
    [Fact]
    public void It_codice_fiscale_checkchar()
    {
        const string baseCf = "RSSMRA85T10A562";
        var valid = Enumerable.Range(0, 26).Select(i => baseCf + (char)('A' + i))
            .First(s => M("it-IT", s).Any(m => m.Type == PiiType.NationalId));

        Assert.Contains(M("it-IT", valid), m => m.Type == PiiType.NationalId);
        char wrong = valid[15] == 'A' ? 'B' : 'A';
        Assert.DoesNotContain(M("it-IT", baseCf + wrong), m => m.Type == PiiType.NationalId);
    }

    [Fact] public void It_mobile() => Assert.Contains(M("it-IT", "+39 320 1234567"), m => m.Type == PiiType.Phone);
}
