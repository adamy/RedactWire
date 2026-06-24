// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedactWire;

namespace RedactWire.Sample.Web.Pages;

public class IndexModel : PageModel
{
    private readonly PiiDetector _detector;   // injected singleton (see Program.cs)

    public IndexModel(PiiDetector detector) => _detector = detector;

    [BindProperty]
    public string? Input { get; set; }

    [BindProperty]
    public string Culture { get; set; } = "en-US";

    [BindProperty]
    public bool ShowRedacted { get; set; }

    public PiiResult? Result { get; private set; }
    public string? Redacted { get; private set; }

    // Built-in culture packs to choose from (grows automatically as packs are added).
    public static IReadOnlyList<string> Cultures => PiiDetectorBuilder.AvailableCultures;

    public void OnGet()
    {
        Input ??= "Email john@x.co.nz, SSN 123-45-6789, call (415) 555-0132,\n"
                + "card 4242 4242 4242 4242, 123 Main St, Springfield, IL 62704 \n"
                // Sample JWT (jwt.io demo token — not a real credential).
                + "token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9."
                + "eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ."
                + "SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
    }

    public void OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input)) return;

        var ci = new CultureInfo(Culture);
        Result = _detector.Detect(Input, ci);

        if (ShowRedacted)
            Redacted = Result.Redact(new RedactionOptions { Mode = RedactionMode.Label });
    }
}
