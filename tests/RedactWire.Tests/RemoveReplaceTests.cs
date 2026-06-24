// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class RemoveReplaceTests
{
    private static readonly CultureInfo EnUs = new("en-US");

    private static IEnumerable<PiiMatch> Detect(PiiDetector d, string text) =>
        d.Detect(text, EnUs).AllMatches;

    [Fact]
    public void RemoveRule_by_type_drops_the_builtin()
    {
        var d = PiiDetectorBuilder.CreateDefault().AddCulture(EnUs)
            .RemoveRule(PiiType.PostalCode)          // drop ZIP
            .Build();
        Assert.DoesNotContain(Detect(d, "90210"), m => m.Type == PiiType.PostalCode);
    }

    [Fact]
    public void RemoveRule_by_type_and_subtype_drops_one_secret_only()
    {
        var d = PiiDetectorBuilder.CreateDefault().AddSecretDetection()
            .RemoveRule(PiiType.Secret, "OpenAiKey")
            .Build();

        Assert.DoesNotContain(Detect(d, "sk-abcdefghijklmnopqrstuvwxyz0123456789"),
            m => m.Type == PiiType.Secret);
        Assert.Contains(Detect(d, "AKIAIOSFODNN7EXAMPLE"),
            m => m.Type == PiiType.Secret && m.Subtype == "AwsAccessKeyId");   // others stay
    }

    [Fact]
    public void ReplaceInvariantRule_overrides_the_builtin()
    {
        var d = PiiDetectorBuilder.CreateDefault()
            .ReplaceInvariantRule(new RegexRule("Email", PiiType.Email, "(?<v>NOPE)"))
            .Build();
        Assert.DoesNotContain(Detect(d, "a@b.com"), m => m.Type == PiiType.Email);
        Assert.Contains(Detect(d, "NOPE"), m => m.Type == PiiType.Email);
    }

    [Fact]
    public void ReplaceRule_overrides_a_culture_rule_by_name()
    {
        var d = PiiDetectorBuilder.CreateDefault().AddCulture(EnUs)
            .ReplaceRule(EnUs, new RegexRule("Phone", PiiType.Phone, "(?<v>ZZZ)"))
            .Build();
        Assert.DoesNotContain(Detect(d, "(415) 555-0132"), m => m.Type == PiiType.Phone);
        Assert.Contains(Detect(d, "ZZZ"), m => m.Type == PiiType.Phone);
    }
}
