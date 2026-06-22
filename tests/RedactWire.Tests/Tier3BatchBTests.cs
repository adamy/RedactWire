// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch B: SE, NO, DK, FI, ZA.
public class Tier3BatchBTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Sweden ───────────────────────────────────────────────────────────────────
    [Fact] public void Se_pnr_valid() => Assert.Contains(M("sv-SE", "8112189876"), m => m.Type == PiiType.NationalId);
    [Fact] public void Se_pnr_invalid() => Assert.DoesNotContain(M("sv-SE", "8112189870"), m => m.Type == PiiType.NationalId);

    // ── Norway ───────────────────────────────────────────────────────────────────
    [Fact] public void No_fnr_valid() => Assert.Contains(M("nb-NO", "01010112377"), m => m.Type == PiiType.NationalId);
    [Fact] public void No_fnr_invalid() => Assert.DoesNotContain(M("nb-NO", "01010112370"), m => m.Type == PiiType.NationalId);

    // ── Denmark ──────────────────────────────────────────────────────────────────
    [Fact] public void Dk_cpr_valid() => Assert.Contains(M("da-DK", "010101-1234"), m => m.Type == PiiType.NationalId);
    [Fact] public void Dk_cpr_bad_date() => Assert.DoesNotContain(M("da-DK", "320101-1234"), m => m.Type == PiiType.NationalId);

    // ── Finland ──────────────────────────────────────────────────────────────────
    [Fact] public void Fi_hetu_valid() => Assert.Contains(M("fi-FI", "010190-123M"), m => m.Type == PiiType.NationalId);
    [Fact] public void Fi_hetu_invalid() => Assert.DoesNotContain(M("fi-FI", "010190-123A"), m => m.Type == PiiType.NationalId);

    // ── South Africa ─────────────────────────────────────────────────────────────
    [Fact] public void Za_id_valid() => Assert.Contains(M("en-ZA", "9001015000085"), m => m.Type == PiiType.NationalId);
    [Fact] public void Za_id_invalid() => Assert.DoesNotContain(M("en-ZA", "9001015000080"), m => m.Type == PiiType.NationalId);
    [Fact] public void Za_mobile() => Assert.Contains(M("en-ZA", "0821234567"), m => m.Type == PiiType.Phone);
}
