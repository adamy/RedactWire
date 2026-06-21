// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using System.Text.Json;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class JsonScanTests
{
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();

    [Fact]
    public void Reports_matches_by_jsonpath()
    {
        const string json = """
            {"user":{"email":"a@b.com","contacts":[{"phone":"(415) 555-0132"}]},"ssn":"123-45-6789"}
            """;
        var hits = Detector.DetectJson(json);

        Assert.Contains(hits, h => h.Path == "$.user.email" && h.Match.Type == PiiType.Email);
        Assert.Contains(hits, h => h.Path == "$.user.contacts[0].phone" && h.Match.Type == PiiType.Phone);
        Assert.Contains(hits, h => h.Path == "$.ssn" && h.Match.Type == PiiType.SocialSecurity);
    }

    [Fact]
    public void Ignores_non_string_values()
    {
        // 123456789 as a number is not scanned; only the string field is.
        var hits = Detector.DetectJson("""{"n":123456789,"s":"a@b.com"}""");
        Assert.Single(hits);
        Assert.Equal("$.s", hits[0].Path);
    }

    [Fact]
    public void Nested_arrays_path()
    {
        var hits = Detector.DetectJson("""{"a":[["x@y.com"]]}""");
        var h = Assert.Single(hits);
        Assert.Equal("$.a[0][0]", h.Path);
        Assert.Equal(PiiType.Email, h.Match.Type);
    }

    [Fact]
    public void Multiple_matches_in_one_value_share_path()
    {
        var hits = Detector.DetectJson("""{"s":"a@b.com and c@d.com"}""");
        Assert.Equal(2, hits.Count);
        Assert.All(hits, h => Assert.Equal("$.s", h.Path));
    }

    [Fact]
    public void No_pii_returns_empty()
    {
        Assert.Empty(Detector.DetectJson("""{"x":"hello world"}"""));
    }

    [Fact]
    public void Credit_card_is_validated_and_critical()
    {
        var h = Assert.Single(Detector.DetectJson("""{"card":"4242 4242 4242 4242"}"""));
        Assert.Equal("$.card", h.Path);
        Assert.Equal(PiiType.CreditCard, h.Match.Type);
        Assert.Equal(PiiSeverity.Critical, h.Match.Severity);
    }

    [Fact]
    public void Invalid_json_throws()
    {
        Assert.ThrowsAny<JsonException>(() => Detector.DetectJson("{ not json"));
    }
}
