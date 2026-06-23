// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

// Tier 3 batch E: BE, CZ, IE, EE, IS.
public class Tier3BatchETests
{
    private static IEnumerable<PiiMatch> M(string culture, string text) =>
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo(culture)).Build()
            .Detect(text, new CultureInfo(culture)).Cultures.Single(c => c.Culture == culture).Matches;

    // ── Belgium ──────────────────────────────────────────────────────────────────
    [Fact] public void Be_nn_valid() => Assert.Contains(M("nl-BE", "85011733277"), m => m.Type == PiiType.NationalId);
    [Fact] public void Be_nn_invalid() => Assert.DoesNotContain(M("nl-BE", "85011733278"), m => m.Type == PiiType.NationalId);
    [Fact] public void Be_mobile() => Assert.Contains(M("nl-BE", "0470123456"), m => m.Type == PiiType.Phone);

    // ── Czechia ──────────────────────────────────────────────────────────────────
    [Fact] public void Cz_rc_valid() => Assert.Contains(M("cs-CZ", "9001011239"), m => m.Type == PiiType.NationalId);
    [Fact] public void Cz_rc_invalid() => Assert.DoesNotContain(M("cs-CZ", "9001011238"), m => m.Type == PiiType.NationalId);

    // ── Ireland ──────────────────────────────────────────────────────────────────
    [Fact] public void Ie_pps_valid() => Assert.Contains(M("en-IE", "1234567T"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ie_pps_invalid() => Assert.DoesNotContain(M("en-IE", "1234567A"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ie_mobile() => Assert.Contains(M("en-IE", "0831234567"), m => m.Type == PiiType.Phone);

    // ── Estonia ──────────────────────────────────────────────────────────────────
    [Fact] public void Ee_ik_valid() => Assert.Contains(M("et-EE", "38501171232"), m => m.Type == PiiType.NationalId);
    [Fact] public void Ee_ik_invalid() => Assert.DoesNotContain(M("et-EE", "38501171230"), m => m.Type == PiiType.NationalId);

    // ── Iceland ──────────────────────────────────────────────────────────────────
    [Fact] public void Is_kennitala_valid() => Assert.Contains(M("is-IS", "0101901269"), m => m.Type == PiiType.NationalId);
    [Fact] public void Is_kennitala_invalid() => Assert.DoesNotContain(M("is-IS", "0101901259"), m => m.Type == PiiType.NationalId);
}
