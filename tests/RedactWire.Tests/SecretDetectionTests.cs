// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class SecretDetectionTests
{
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddSecretDetection().Build();

    private static IEnumerable<PiiMatch> Secrets(string text) =>
        Detector.Detect(text).AllMatches.Where(m => m.Type == PiiType.Secret);

    [Theory]
    [InlineData("sk-abcdefghijklmnopqrstuvwxyz0123456789", "OpenAiKey")]
    [InlineData("AKIAIOSFODNN7EXAMPLE", "AwsAccessKeyId")]
    [InlineData("ghp_1234567890abcdefghijklmnopqrstuvwxyz", "GitHubToken")]
    [InlineData("sk_live_1234567890abcdefghijklmnop", "StripeSecretKey")]
    [InlineData("AIzaSyA1234567890abcdefghijklmnopqrstuv", "GoogleApiKey")]
    [InlineData("xoxb-1234567890-abcdefghij", "SlackToken")]
    public void Detects_provider_keys(string text, string subtype)
    {
        Assert.Contains(Secrets(text),
            m => m.Subtype == subtype && m.Severity == PiiSeverity.Critical);
    }

    [Fact]
    public void Detects_jwt()
    {
        const string jwt = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjMifQ.s5Q-abcDEF_ghiJKLmnoPQR";
        Assert.Contains(Secrets($"token: {jwt}"), m => m.Subtype == "Jwt");
    }

    [Fact]
    public void Detects_private_key_block()
    {
        Assert.Contains(Secrets("-----BEGIN RSA PRIVATE KEY-----"), m => m.Subtype == "PrivateKey");
    }

    [Fact]
    public void Off_by_default_without_AddSecretDetection()
    {
        var plain = PiiDetectorBuilder.CreateDefault().Build();
        Assert.DoesNotContain(plain.Detect("sk-abcdefghijklmnopqrstuvwxyz0123456789").AllMatches,
            m => m.Type == PiiType.Secret);
    }

    [Fact]
    public void Static_Redactor_has_secrets_enabled()
    {
        Assert.Contains(Redactor.Detect("AKIAIOSFODNN7EXAMPLE").AllMatches, m => m.Type == PiiType.Secret);
    }

    [Fact]
    public void Redacts_secret_with_its_provider_label()
    {
        var r = Redactor.Detect("key=AKIAIOSFODNN7EXAMPLE")
            .Redact(new RedactionOptions { Mode = RedactionMode.Label });
        Assert.Equal("key=[AwsAccessKeyId]", r);
    }

    [Fact]
    public void Plain_text_is_not_flagged()
    {
        Assert.Empty(Secrets("the quick brown fox jumps over the lazy dog 12345"));
    }
}
