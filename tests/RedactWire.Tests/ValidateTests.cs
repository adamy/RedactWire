// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class ValidateTests
{
    private static readonly CultureInfo EnUs = new("en-US");
    private static readonly CultureInfo ZhCn = new("zh-CN");
    private static readonly CultureInfo RuRu = new("ru-RU");
    private static readonly CultureInfo FrFr = new("fr-FR");   // no pack

    private static readonly PiiDetector Detector = PiiDetectorBuilder.CreateDefault()
        .AddCulture(EnUs).AddCulture(ZhCn).AddCulture(RuRu).Build();

    [Fact]
    public void Valid_when_value_passes_rule_and_checksum()
    {
        Assert.Equal(ValidationResult.Valid, Detector.Validate("123-45-6789", EnUs, PiiType.SocialSecurity));
        Assert.Equal(ValidationResult.Valid, Detector.Validate("110101199001010015", ZhCn, PiiType.NationalId));
    }

    [Fact]
    public void Invalid_when_rule_exists_but_value_fails()
    {
        Assert.Equal(ValidationResult.Invalid, Detector.Validate("123-45-0000", EnUs, PiiType.SocialSecurity));
        Assert.Equal(ValidationResult.Invalid, Detector.Validate("110101199001010014", ZhCn, PiiType.NationalId));
    }

    [Fact]
    public void Unsupported_when_no_rule_for_type_in_culture()
    {
        Assert.Equal(ValidationResult.Unsupported, Detector.Validate("12345", FrFr, PiiType.NationalId));
    }

    [Fact]
    public void Invariant_types_validate_under_any_culture()
    {
        Assert.Equal(ValidationResult.Valid, Detector.Validate("john@x.co.nz", FrFr, PiiType.Email));
    }

    [Fact]
    public void Default_overload_uses_configured_cultures()
    {
        // No culture argument → uses the detector's configured set (incl. zh-CN).
        Assert.Equal(ValidationResult.Valid, Detector.Validate("110101199001010015", PiiType.NationalId));
    }

    [Fact]
    public void Must_be_full_match_not_substring()
    {
        Assert.Equal(ValidationResult.Invalid,
            Detector.Validate("my ssn is 123-45-6789", EnUs, PiiType.SocialSecurity));
    }

    [Fact]
    public void Surrounding_whitespace_is_trimmed()
    {
        Assert.Equal(ValidationResult.Valid, Detector.Validate("  123-45-6789  ", EnUs, PiiType.SocialSecurity));
    }

    [Fact]
    public void Wrong_type_is_invalid_not_valid()
    {
        Assert.Equal(ValidationResult.Invalid, Detector.Validate("123-45-6789", EnUs, PiiType.Email));
    }

    [Fact]
    public void Custom_type_requires_matching_subtype()
    {
        Assert.Equal(ValidationResult.Valid,
            Detector.Validate("112-233-445 95", RuRu, PiiType.Custom, "SNILS"));
        Assert.Equal(ValidationResult.Invalid,
            Detector.Validate("112-233-445 95", RuRu, PiiType.Custom, "OTHER"));
    }
}
