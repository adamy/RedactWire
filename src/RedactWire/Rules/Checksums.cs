// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;
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

    /// <summary>China Resident Identity Card (GB 11643-1999): 18 chars, weighted mod-11
    /// check digit (last char may be 'X') plus an embedded birth date sanity check.
    /// VERIFY: algorithm per GB 11643-1999.</summary>
    public static bool ChinaResidentId(string id)
    {
        if (id.Length != 18) return false;

        int[] w = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        char[] map = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            if (id[i] < '0' || id[i] > '9') return false;
            sum += (id[i] - '0') * w[i];
        }
        if (char.ToUpperInvariant(id[17]) != map[sum % 11]) return false;

        // Embedded birth date: positions 6..13 = yyyyMMdd, must be a real past date.
        var ds = id.Substring(6, 8);
        return DateTime.TryParseExact(ds, "yyyyMMdd",
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob)
               && dob <= DateTime.Today && dob.Year >= 1900;
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
