// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class InvariantRulesTests
{
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();

    [Fact]
    public void Email_positive()
    {
        var r = Detector.Detect("Contact me at john@x.co.nz");
        Assert.Contains(r.Invariant.Matches, m => m.Type == PiiType.Email && m.Value == "john@x.co.nz");
    }

    [Fact]
    public void CreditCard_luhn_pass()
    {
        var r = Detector.Detect("Card 4242 4242 4242 4242");
        Assert.Contains(r.AllMatches, m => m.Type == PiiType.CreditCard);
    }

    [Fact]
    public void CreditCard_luhn_fail_dropped()
    {
        var r = Detector.Detect("Card 4242 4242 4242 4241");
        Assert.DoesNotContain(r.AllMatches, m => m.Type == PiiType.CreditCard);
    }

    [Fact]
    public void IPv4_positive()
    {
        var r = Detector.Detect("host 192.168.1.1 up");
        Assert.Contains(r.AllMatches, m => m.Type == PiiType.IpAddress && m.Value == "192.168.1.1");
    }

    [Fact]
    public void IPv4_octet_over_255_no_match()
    {
        var r = Detector.Detect("not 999.1.1.1 valid");
        Assert.DoesNotContain(r.AllMatches, m => m.Type == PiiType.IpAddress);
    }
}
