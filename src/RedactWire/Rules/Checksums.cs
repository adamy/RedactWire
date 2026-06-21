// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Linq;
using System.Text;

namespace RedactWire.Rules;

/// <summary>Checksum validators. These are what separate a real detector from a naive
/// pattern matcher — an 18-digit number isn't automatically an ID, a 16-digit number
/// isn't automatically a card. A failed checksum DROPS the candidate.</summary>
internal static class Checksums
{
    /// <summary>Keep only ASCII digits.</summary>
    public static string Digits(string s) => new string(s.Where(char.IsDigit).ToArray());

    /// <summary>Luhn algorithm for credit-card validation (13–19 digits).</summary>
    public static bool Luhn(string digits)
    {
        if (digits.Length < 13 || digits.Length > 19) return false;
        int sum = 0; bool dbl = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if (dbl) { d *= 2; if (d > 9) d -= 9; }
            sum += d; dbl = !dbl;
        }
        return sum % 10 == 0;
    }

    /// <summary>IBAN mod-97: move first 4 chars to the end, map A→10…Z→35, mod 97 == 1.</summary>
    public static bool IbanValid(string iban)
    {
        iban = iban.Replace(" ", "").ToUpperInvariant();
        if (iban.Length < 15 || iban.Length > 34) return false;
        var rearranged = iban.Substring(4) + iban.Substring(0, 4);
        var sb = new StringBuilder();
        foreach (char c in rearranged)
        {
            if (c >= '0' && c <= '9') sb.Append(c);
            else if (c >= 'A' && c <= 'Z') sb.Append((c - 'A' + 10).ToString());
            else return false;
        }
        int rem = 0;
        foreach (char c in sb.ToString())
            rem = (rem * 10 + (c - '0')) % 97;
        return rem == 1;
    }
}
