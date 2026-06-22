// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class EsMxRulesTests
{
    private static readonly CultureInfo EsMx = new("es-MX");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(EsMx).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, EsMx).Cultures.Single(c => c.Culture == "es-MX").Matches;

    [Fact]
    public void Curp_valid_positive()
    {
        Assert.Contains(Matches("GOMA800101HDFXYZ08"),
            m => m.Type == PiiType.NationalId && m.Value == "GOMA800101HDFXYZ08");
    }

    [Fact]
    public void Curp_bad_checkdigit_no_match()
    {
        Assert.DoesNotContain(Matches("GOMA800101HDFXYZ09"), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Rfc_positive()
    {
        Assert.Contains(Matches("GODE561231GR8"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("+52 55 1234 5678"), m => m.Type == PiiType.Phone);
    }
}
