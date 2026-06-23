// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch F: SK, SI, LT, LU, AT.
public class Tier3BatchFTests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Slovakia (shares Czech rodné číslo) ──────────────────────────────────────
    [Fact] public void Sk_rc_valid() => Assert.Contains(M("sk-SK", "9001011239"), m => m.Type == PiiType.NationalId);
    [Fact] public void Sk_rc_invalid() => Assert.DoesNotContain(M("sk-SK", "9001011238"), m => m.Type == PiiType.NationalId);

    // ── Slovenia (EMŠO) ──────────────────────────────────────────────────────────
    [Fact] public void Si_emso_valid() => Assert.Contains(M("sl-SI", "0101950500019"), m => m.Type == PiiType.NationalId);
    [Fact] public void Si_emso_invalid() => Assert.DoesNotContain(M("sl-SI", "0101950500010"), m => m.Type == PiiType.NationalId);

    // ── Lithuania (shares two-pass mod-11) ───────────────────────────────────────
    [Fact] public void Lt_id_valid() => Assert.Contains(M("lt-LT", "38501171232"), m => m.Type == PiiType.NationalId);
    [Fact] public void Lt_id_invalid() => Assert.DoesNotContain(M("lt-LT", "38501171230"), m => m.Type == PiiType.NationalId);

    // ── Luxembourg (format only) ─────────────────────────────────────────────────
    [Fact] public void Lu_matricule() => Assert.Contains(M("lb-LU", "1234567890123"), m => m.Type == PiiType.NationalId);

    // ── Austria (no national-ID number; mobile/postcode) ─────────────────────────
    [Fact] public void At_mobile() => Assert.Contains(M("de-AT", "0664123456"), m => m.Type == PiiType.Phone);
}
