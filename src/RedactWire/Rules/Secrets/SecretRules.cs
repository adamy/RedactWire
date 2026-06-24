// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire.Rules.Secrets;

/// <summary>Secret / credential detection — API keys, tokens, private keys. These are
/// country-agnostic, so they register as invariant rules via
/// <c>PiiDetectorBuilder.AddSecretDetection()</c>. v1 is the high-precision,
/// provider-prefixed set (near-zero false positives); entropy/context-based detection
/// (AWS secret keys, Azure keys, connection strings) is a later phase.
/// All emit <see cref="PiiType.Secret"/> with the provider in <c>Subtype</c>.</summary>
internal static class SecretRules
{
    private static RegexRule Secret(string subtype, string pattern, double conf = 0.97) =>
        new(subtype, PiiType.Secret, pattern, baseConfidence: conf, subtype: subtype);

    public static readonly IPiiRule[] Rules =
    {
        // OpenAI: sk-... and sk-proj-...
        Secret("OpenAiKey", @"(?<v>\bsk-(?:proj-)?[A-Za-z0-9_-]{20,}\b)"),

        // AWS access key id (not the secret key, which needs entropy/context).
        Secret("AwsAccessKeyId", @"(?<v>\b(?:AKIA|ASIA|AROA|AIDA|AGPA|ANPA|ANVA|AIPA)[0-9A-Z]{16}\b)", 0.98),

        // GitHub tokens: ghp_/gho_/ghs_/ghr_/ghu_ and fine-grained github_pat_.
        Secret("GitHubToken", @"(?<v>\bgh[posru]_[A-Za-z0-9]{36,}\b)", 0.98),
        Secret("GitHubToken", @"(?<v>\bgithub_pat_[A-Za-z0-9_]{82}\b)", 0.98),

        // Stripe secret / restricted keys.
        Secret("StripeSecretKey", @"(?<v>\b[sr]k_live_[A-Za-z0-9]{24,}\b)", 0.98),

        // Slack tokens.
        Secret("SlackToken", @"(?<v>\bxox[baprs]-[A-Za-z0-9-]{10,}\b)"),

        // Google API key.
        Secret("GoogleApiKey", @"(?<v>\bAIza[0-9A-Za-z_-]{35}\b)", 0.98),

        // SendGrid.
        Secret("SendGridKey", @"(?<v>\bSG\.[A-Za-z0-9_-]{22}\.[A-Za-z0-9_-]{43}\b)", 0.98),

        // npm access token.
        Secret("NpmToken", @"(?<v>\bnpm_[A-Za-z0-9]{36}\b)", 0.98),

        // JSON Web Token (header.payload.signature).
        Secret("Jwt", @"(?<v>\beyJ[A-Za-z0-9_-]+\.eyJ[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+)", 0.9),

        // PEM private key block.
        Secret("PrivateKey",
            @"(?<v>-----BEGIN (?:RSA |EC |DSA |OPENSSH |PGP )?PRIVATE KEY-----)", 0.99),
    };
}
