// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedactWire;   // PiiDetector + DetectXml extension method (RedactWire.Structured)

namespace RedactWire.Sample.Web.Pages;

public class StructuredXmlModel : PageModel
{
    private readonly PiiDetector _detector;   // injected singleton (see Program.cs)

    public StructuredXmlModel(PiiDetector detector) => _detector = detector;

    [BindProperty]
    public string? Input { get; set; }

    // Scope to one country — the DI detector has every built-in pack, so scanning against
    // all of them would surface a generic 4-digit "postcode" hit per country (noise).
    [BindProperty]
    public string Culture { get; set; } = "en-US";

    public static IReadOnlyList<string> Cultures => PiiDetectorBuilder.AvailableCultures;

    public IReadOnlyList<StructuredPiiMatch>? Results { get; private set; }
    public string? Error { get; private set; }

    private const string Sample =
        "<account email=\"john@x.co.nz\">\n  <ssn>123-45-6789</ssn>\n" +
        "  <phone>(415) 555-0132</phone>\n  <card>4242 4242 4242 4242</card>\n</account>";

    public void OnGet() => Input ??= Sample;

    public void OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input)) return;
        try { Results = _detector.DetectXml(Input, new CultureInfo(Culture)); }
        catch (XmlException ex) { Error = "Invalid XML: " + ex.Message; }
    }
}
