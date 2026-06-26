// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using System.Xml;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class XmlScanTests
{
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();

    [Fact]
    public void Reports_attributes_and_text_by_xpath()
    {
        const string xml = "<root><user email=\"a@b.com\"><ssn>123-45-6789</ssn></user></root>";
        var hits = Detector.DetectXml(xml);

        Assert.Contains(hits, h => h.Path == "/root/user/@email" && h.Match.Type == PiiType.Email);
        Assert.Contains(hits, h => h.Path == "/root/user/ssn/text()" && h.Match.Type == PiiType.SocialSecurity);
    }

    [Fact]
    public void Indexes_repeated_siblings()
    {
        const string xml = "<root><item>a@b.com</item><item>c@d.com</item></root>";
        var hits = Detector.DetectXml(xml);
        Assert.Contains(hits, h => h.Path == "/root/item[1]/text()");
        Assert.Contains(hits, h => h.Path == "/root/item[2]/text()");
    }

    [Fact]
    public void Uses_local_name_ignoring_namespace()
    {
        const string xml = "<root xmlns:n=\"urn:x\"><n:user n:email=\"a@b.com\"/></root>";
        var h = Assert.Single(Detector.DetectXml(xml));
        Assert.Equal("/root/user/@email", h.Path);
        Assert.Equal(PiiType.Email, h.Match.Type);
    }

    [Fact]
    public void Reads_cdata()
    {
        const string xml = "<root><x><![CDATA[a@b.com]]></x></root>";
        var h = Assert.Single(Detector.DetectXml(xml));
        Assert.Equal("/root/x/text()", h.Path);
    }

    [Fact]
    public void Multiple_matches_in_text_share_path()
    {
        var hits = Detector.DetectXml("<root>a@b.com c@d.com</root>");
        Assert.Equal(2, hits.Count);
        Assert.All(hits, h => Assert.Equal("/root/text()", h.Path));
    }

    [Fact]
    public void Blocks_xxe()
    {
        // DOCTYPE/DTD must be rejected (DtdProcessing.Prohibit) → no entity expansion.
        const string evil = """
            <?xml version="1.0"?>
            <!DOCTYPE foo [ <!ENTITY xxe SYSTEM "file:///etc/passwd"> ]>
            <root>&xxe;</root>
            """;
        Assert.ThrowsAny<XmlException>(() => Detector.DetectXml(evil));
    }

    [Fact]
    public void Invalid_xml_throws()
    {
        Assert.ThrowsAny<XmlException>(() => Detector.DetectXml("<root><a></root>"));
    }
}
