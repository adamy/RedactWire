// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class PtBrRulesTests
{
    private static readonly CultureInfo PtBr = new("pt-BR");
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(PtBr).Build();

    private static IEnumerable<PiiMatch> Matches(string text) =>
        Detector.Detect(text, PtBr).Cultures.Single(c => c.Culture == "pt-BR").Matches;

    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    public void Cpf_valid_positive(string text)
    {
        Assert.Contains(Matches(text), m => m.Type == PiiType.NationalId);
    }

    [Theory]
    [InlineData("111.444.777-00")]   // wrong check digits
    [InlineData("000.000.000-00")]   // all-equal
    public void Cpf_invalid_no_match(string text)
    {
        Assert.DoesNotContain(Matches(text), m => m.Type == PiiType.NationalId);
    }

    [Fact]
    public void Cnpj_valid_positive()
    {
        Assert.Contains(Matches("11.222.333/0001-81"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Cnpj_invalid_no_match()
    {
        Assert.DoesNotContain(Matches("11.222.333/0001-80"), m => m.Type == PiiType.TaxId);
    }

    [Fact]
    public void Mobile_positive()
    {
        Assert.Contains(Matches("+55 11 98765-4321"), m => m.Type == PiiType.Phone);
    }

    [Fact]
    public void Cep_positive()
    {
        Assert.Contains(Matches("01310-200"), m => m.Type == PiiType.PostalCode);
    }
}
