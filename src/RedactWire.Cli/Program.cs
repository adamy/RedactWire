// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;

// RedactWire CLI — scan text for PII, optionally redact.
//
//   echo "My SSN is 123-45-6789" | redactwire
//   redactwire --text "Call (415) 555-0132"
//   redactwire --redact --text "..."          # print redacted text
//   redactwire --culture en-US --text "..."
//
// Reads --text if given, else stdin. Exit code 1 if PII found (handy in CI), 0 if clean.

static (string? text, bool redact, List<CultureInfo> cultures, bool help) Parse(string[] a)
{
    string? text = null;
    bool redact = false, help = false;
    var cultures = new List<CultureInfo>();
    for (int i = 0; i < a.Length; i++)
    {
        switch (a[i])
        {
            case "--text" or "-t" when i + 1 < a.Length: text = a[++i]; break;
            case "--culture" or "-c" when i + 1 < a.Length: cultures.Add(new CultureInfo(a[++i])); break;
            case "--redact" or "-r": redact = true; break;
            case "--help" or "-h": help = true; break;
        }
    }
    return (text, redact, cultures, help);
}

var (textArg, redact, cultures, help) = Parse(args);

if (help)
{
    Console.WriteLine("""
        RedactWire — PII detector

        Usage:
          redactwire [--text <s>] [--culture <code>]... [--redact]
          <stdin> | redactwire

        Options:
          -t, --text <s>        text to scan (else read stdin)
          -c, --culture <code>  culture pack, repeatable (default: en-US)
          -r, --redact          print redacted text instead of a match report
          -h, --help            this help

        Exit: 1 if PII found, 0 if clean.
        """);
    return 0;
}

string input = textArg ?? Console.In.ReadToEnd();
if (string.IsNullOrEmpty(input))
{
    Console.Error.WriteLine("No input. Pass --text or pipe via stdin. See --help.");
    return 2;
}

var builder = PiiDetectorBuilder.CreateDefault();
if (cultures.Count == 0) builder.AddCulture(new CultureInfo("en-US"));
else foreach (var c in cultures) builder.AddCulture(c);
var result = builder.Build().Detect(input);

if (redact)
{
    Console.WriteLine(result.Redact(new RedactionOptions { Mode = RedactionMode.Label }));
}
else if (!result.HasPii)
{
    Console.WriteLine("No PII detected.");
}
else
{
    Console.WriteLine($"{result.AllMatches.Count()} match(es):");
    foreach (var m in result.AllMatches.OrderByDescending(m => m.Severity).ThenBy(m => m.Start))
        Console.WriteLine($"  {m.Severity,-8} [{m.Type,-14}] {m.Value,-28} conf={m.Confidence:0.00}  ({m.Rule})");
}

// Surface unsupported cultures so "no PII" isn't mistaken for "country not covered".
foreach (var c in result.Cultures.Where(c => !c.Supported))
    Console.Error.WriteLine($"note: culture '{c.Culture}' has no rule pack — not evaluated.");

return result.HasPii ? 1 : 0;
