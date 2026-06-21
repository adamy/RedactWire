// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
using RedactWire;
using Xunit;

namespace RedactWire.Tests;

public class ObjectScanTests
{
    private static readonly PiiDetector Detector =
        PiiDetectorBuilder.CreateDefault().AddCulture(new CultureInfo("en-US")).Build();

    private sealed class Person
    {
        public string? Email { get; set; }
        public string? Ssn { get; set; }
        public List<Contact> Contacts { get; set; } = new();
        public int Age { get; set; }           // scalar — ignored
        public Person? Manager { get; set; }   // for cycle test
    }

    private sealed class Contact
    {
        public string? Phone { get; set; }
    }

    [Fact]
    public void Reports_matches_by_property_path()
    {
        var p = new Person
        {
            Email = "a@b.com",
            Ssn = "123-45-6789",
            Age = 1234567,                 // not a string → not scanned
            Contacts = { new Contact { Phone = "(415) 555-0132" } },
        };
        var hits = Detector.DetectObject(p);

        Assert.Contains(hits, h => h.Path == "Email" && h.Match.Type == PiiType.Email);
        Assert.Contains(hits, h => h.Path == "Ssn" && h.Match.Type == PiiType.SocialSecurity);
        Assert.Contains(hits, h => h.Path == "Contacts[0].Phone" && h.Match.Type == PiiType.Phone);
        Assert.DoesNotContain(hits, h => h.Match.Value == "1234567");
    }

    [Fact]
    public void Handles_cycles()
    {
        var a = new Person { Email = "a@b.com" };
        a.Manager = a;                          // self-reference
        var hits = Detector.DetectObject(a);    // must terminate
        Assert.Contains(hits, h => h.Path == "Email");
    }

    private sealed class Pair
    {
        public Person? A { get; set; }
        public Person? B { get; set; }
    }

    [Fact]
    public void Shared_non_cyclic_reference_is_scanned_at_each_path()
    {
        var shared = new Person { Email = "a@b.com" };
        var pair = new Pair { A = shared, B = shared };   // same object, two paths
        var hits = Detector.DetectObject(pair);
        Assert.Contains(hits, h => h.Path == "A.Email");
        Assert.Contains(hits, h => h.Path == "B.Email");  // not pruned by a global visited set
    }

    [Fact]
    public void Null_members_are_skipped()
    {
        var p = new Person { Email = null, Ssn = "123-45-6789" };
        var hits = Detector.DetectObject(p);
        Assert.DoesNotContain(hits, h => h.Path == "Email");
        Assert.Contains(hits, h => h.Path == "Ssn");
    }

    private sealed class Bag
    {
        public Dictionary<string, string?> Map { get; set; } = new();
    }

    [Fact]
    public void Scans_dictionary_values_by_key()
    {
        var bag = new Bag { Map = { ["home"] = "a@b.com" } };
        var h = Assert.Single(Detector.DetectObject(bag));
        Assert.Equal("Map[\"home\"]", h.Path);
        Assert.Equal(PiiType.Email, h.Match.Type);
    }

    private sealed class Inbox
    {
        public string[] Emails { get; set; } = System.Array.Empty<string>();
    }

    [Fact]
    public void Scans_arrays_by_index()
    {
        var inbox = new Inbox { Emails = new[] { "a@b.com", "c@d.com" } };
        var hits = Detector.DetectObject(inbox);
        Assert.Contains(hits, h => h.Path == "Emails[0]");
        Assert.Contains(hits, h => h.Path == "Emails[1]");
    }

    private enum Mood { Happy, Sad }

    private sealed class Scalars
    {
        public System.DateTime When { get; set; }
        public System.Guid Id { get; set; }
        public Mood Mood { get; set; }
        public string? Note { get; set; }
    }

    [Fact]
    public void Skips_scalar_types()
    {
        var s = new Scalars
        {
            When = System.DateTime.Now,
            Id = System.Guid.NewGuid(),
            Mood = Mood.Happy,
            Note = "a@b.com",
        };
        var h = Assert.Single(Detector.DetectObject(s));   // only the string member
        Assert.Equal("Note", h.Path);
    }
}
