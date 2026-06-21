// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RedactWire;

/// <summary>Scans XML for PII. Walks element text and attribute values, reporting each
/// match with an XPath-like location.
/// <para><b>Security:</b> XML is parsed with DTD processing prohibited and no external
/// resolver, so hostile documents cannot trigger XXE (entity expansion / file / SSRF).</para></summary>
public static class XmlPiiScanner
{
    /// <summary>Detect PII in an XML string. Returns a match per (location, PII) pair.</summary>
    public static IReadOnlyList<StructuredPiiMatch> DetectXml(
        this PiiDetector detector, string xml, params CultureInfo[] cultures)
    {
        if (detector is null) throw new ArgumentNullException(nameof(detector));
        if (xml is null) throw new ArgumentNullException(nameof(xml));

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,   // block XXE
            XmlResolver = null,                        // no external entity resolution
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };

        XDocument doc;
        using (var sr = new StringReader(xml))
        using (var reader = XmlReader.Create(sr, settings))
            doc = XDocument.Load(reader);

        var results = new List<StructuredPiiMatch>();
        if (doc.Root is null) return results;

        foreach (var el in doc.Descendants())
        {
            var elPath = XPathOf(el);

            // attribute values
            foreach (var attr in el.Attributes())
            {
                if (attr.IsNamespaceDeclaration) continue;
                Scan(detector, attr.Value, $"{elPath}/@{attr.Name.LocalName}", cultures, results);
            }

            // direct text nodes only (avoid double-counting descendant text)
            foreach (var node in el.Nodes())
                if (node is XText t)
                    Scan(detector, t.Value, $"{elPath}/text()", cultures, results);
        }

        return results;
    }

    private static void Scan(PiiDetector detector, string value, string path,
        CultureInfo[] cultures, List<StructuredPiiMatch> sink)
    {
        if (string.IsNullOrEmpty(value)) return;
        foreach (var m in detector.Detect(value, cultures).AllMatches)
            sink.Add(new StructuredPiiMatch(path, m));
    }

    /// <summary>Build an XPath like <c>/root/items/item[2]/name</c>, adding a positional
    /// predicate only when an element has same-name siblings.</summary>
    private static string XPathOf(XElement el)
    {
        var sb = new StringBuilder();
        for (XElement? cur = el; cur is not null; cur = cur.Parent)
        {
            string name = cur.Name.LocalName;
            string step;
            if (cur.Parent is null)
            {
                step = "/" + name;
            }
            else
            {
                var sameName = cur.Parent.Elements(cur.Name).ToList();
                if (sameName.Count > 1)
                {
                    int idx = sameName.IndexOf(cur) + 1;   // XPath is 1-based
                    step = $"/{name}[{idx}]";
                }
                else
                {
                    step = "/" + name;
                }
            }
            sb.Insert(0, step);
        }
        return sb.ToString();
    }
}
