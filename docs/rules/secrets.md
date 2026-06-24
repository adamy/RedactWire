# Secret detection

Country-agnostic credential detection — API keys, tokens, private keys. Enabled via
`PiiDetectorBuilder.AddSecretDetection()` (the static `Redactor` has it on by default).
Secrets register as **invariant** rules and emit `PiiType.Secret` (severity **Critical**)
with the provider in `Subtype`, so redaction labels them `[OpenAiKey]`, `[Jwt]`, etc.

```csharp
var d = PiiDetectorBuilder.CreateDefault().AddSecretDetection().Build();
d.Detect("AKIAIOSFODNN7EXAMPLE").AllMatches;   // Secret / Subtype=AwsAccessKeyId
```

## v1 — high-precision, provider-prefixed (near-zero false positives)

| Subtype | Matches |
|---|---|
| `OpenAiKey` | `sk-…`, `sk-proj-…` |
| `AwsAccessKeyId` | `AKIA/ASIA/AROA/AIDA…` + 16 |
| `GitHubToken` | `ghp_/gho_/ghs_/ghr_/ghu_…`, `github_pat_…` |
| `StripeSecretKey` | `sk_live_…`, `rk_live_…` |
| `SlackToken` | `xoxb-/xoxp-/xoxa-/xoxr-/xoxs-…` |
| `GoogleApiKey` | `AIza…` |
| `SendGridKey` | `SG.…` |
| `NpmToken` | `npm_…` |
| `Jwt` | `eyJ….eyJ….…` |
| `PrivateKey` | `-----BEGIN … PRIVATE KEY-----` |

## Open (phase 2 — needs entropy/context)

AWS **secret** access key (40-char base64), Azure keys (32-hex), database **connection
strings**, and generic `API_KEY=…` assignments — these flood without an entropy score or a
context cue (`password=`, `KEY=`), so they're deferred to a context/entropy pass.

VERIFY token formats against provider docs before release; providers rotate prefixes.
