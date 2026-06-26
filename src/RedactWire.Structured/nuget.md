# RedactWire.Structured

Structured-data scanning add-on for [**RedactWire**](https://www.nuget.org/packages/RedactWire/) —
detect PII inside **JSON**, **XML** and **object graphs**, each hit located by JSONPath,
XPath, or property path.

> ⚠️ **This is an add-on, not a standalone package.** It extends the main
> [**RedactWire**](https://www.nuget.org/packages/RedactWire/) package — you build the
> detector with RedactWire's API (`PiiDetectorBuilder` / `Redactor`) and this package adds
> the `DetectJson` / `DetectXml` / `DetectObject` methods on top. RedactWire is pulled in
> automatically as a dependency, but everything except the three scan methods lives there,
> so read its docs first.

Split out of the core package so plain-string detection stays free of a
`System.Text.Json` dependency; install this only when you need structured scanning.

```bash
dotnet add package RedactWire.Structured   # brings in RedactWire automatically
```

```csharp
using RedactWire;                       // PiiDetector, Redactor
using Microsoft.Extensions.DependencyInjection; // (if using DI)

var detector = PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();

foreach (var h in detector.DetectJson("""{"user":{"email":"a@b.com"},"ssn":"123-45-6789"}"""))
    Console.WriteLine($"{h.Path}: {h.Match.Type}");   // $.user.email: Email  /  $.ssn: SocialSecurity

detector.DetectXml("<u email=\"a@b.com\"/>");   // -> /u/@email   (XXE-safe)
detector.DetectObject(myPoco);                  // -> User.Contacts[0].Phone

// or via the static facade's shared detector:
Redactor.Default.DetectJson(json);
```

`DetectJson` / `DetectXml` / `DetectObject` are extension methods on `PiiDetector`, so they
light up automatically once this package is referenced. License: **Apache-2.0**.
Docs: https://github.com/adamy/RedactWire
