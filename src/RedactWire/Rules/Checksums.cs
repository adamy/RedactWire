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

    // Verhoeff tables (dihedral group D5). Used by India Aadhaar, among others.
    private static readonly int[][] VD =
    {
        new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        new[] { 1, 2, 3, 4, 0, 6, 7, 8, 9, 5 },
        new[] { 2, 3, 4, 0, 1, 7, 8, 9, 5, 6 },
        new[] { 3, 4, 0, 1, 2, 8, 9, 5, 6, 7 },
        new[] { 4, 0, 1, 2, 3, 9, 5, 6, 7, 8 },
        new[] { 5, 9, 8, 7, 6, 0, 4, 3, 2, 1 },
        new[] { 6, 5, 9, 8, 7, 1, 0, 4, 3, 2 },
        new[] { 7, 6, 5, 9, 8, 2, 1, 0, 4, 3 },
        new[] { 8, 7, 6, 5, 9, 3, 2, 1, 0, 4 },
        new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 },
    };
    private static readonly int[][] VP =
    {
        new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        new[] { 1, 5, 7, 6, 2, 8, 3, 0, 9, 4 },
        new[] { 5, 8, 0, 3, 7, 9, 6, 1, 4, 2 },
        new[] { 8, 9, 1, 6, 0, 4, 3, 5, 2, 7 },
        new[] { 9, 4, 5, 3, 1, 2, 6, 8, 7, 0 },
        new[] { 4, 2, 8, 6, 5, 7, 3, 9, 0, 1 },
        new[] { 2, 7, 9, 3, 8, 0, 6, 4, 1, 5 },
        new[] { 7, 0, 4, 6, 9, 1, 3, 2, 5, 8 },
    };

    /// <summary>Verhoeff checksum: valid when the running product over all digits
    /// (rightmost first, including the trailing check digit) collapses to 0.
    /// VERIFY: used by India Aadhaar (12 digits).</summary>
    public static bool Verhoeff(string digits)
    {
        int c = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            char ch = digits[digits.Length - 1 - i];
            if (ch < '0' || ch > '9') return false;
            c = VD[c][VP[i % 8][ch - '0']];
        }
        return c == 0;
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

    /// <summary>Brazil CPF: 11 digits, two mod-11 check digits. All-equal digit strings
    /// (e.g. 00000000000) are rejected. Accepts formatted input. VERIFY: CPF algorithm.</summary>
    public static bool Cpf(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11 || d.All(c => c == d[0])) return false;

        int Check(int len)
        {
            int sum = 0;
            for (int i = 0; i < len; i++) sum += (d[i] - '0') * (len + 1 - i);
            int r = sum % 11;
            return r < 2 ? 0 : 11 - r;
        }
        return Check(9) == d[9] - '0' && Check(10) == d[10] - '0';
    }

    /// <summary>Brazil CNPJ (business): 14 digits, two mod-11 check digits with the
    /// standard weight sequences. VERIFY: CNPJ algorithm.</summary>
    public static bool Cnpj(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 14 || d.All(c => c == d[0])) return false;

        int Check(int len, int[] w)
        {
            int sum = 0;
            for (int i = 0; i < len; i++) sum += (d[i] - '0') * w[i];
            int r = sum % 11;
            return r < 2 ? 0 : 11 - r;
        }
        int[] w1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] w2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        return Check(12, w1) == d[12] - '0' && Check(13, w2) == d[13] - '0';
    }

    /// <summary>Russia INN: 10 digits (legal entity) or 12 digits (individual), with
    /// mod-11 check digit(s). VERIFY: INN algorithm.</summary>
    public static bool RussiaInn(string raw)
    {
        var d = Digits(raw);
        int Check(int[] coef)
        {
            int sum = 0;
            for (int i = 0; i < coef.Length; i++) sum += (d[i] - '0') * coef[i];
            return sum % 11 % 10;
        }
        if (d.Length == 10)
            return Check(new[] { 2, 4, 10, 3, 5, 9, 4, 6, 8 }) == d[9] - '0';
        if (d.Length == 12)
            return Check(new[] { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }) == d[10] - '0'
                && Check(new[] { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }) == d[11] - '0';
        return false;
    }

    /// <summary>Russia SNILS: 11 digits, weighted control number over the first 9.
    /// VERIFY: SNILS algorithm.</summary>
    public static bool RussiaSnils(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (d[i] - '0') * (9 - i);
        int control = sum % 101;
        if (control == 100) control = 0;
        int given = (d[9] - '0') * 10 + (d[10] - '0');
        return control == given;
    }

    /// <summary>Indonesia NIK (KTP): 16 digits with an embedded birth date at positions
    /// 6..11 = DDMMYY (DD is +40 for females). No check digit, so we sanity-check the date.
    /// VERIFY: NIK structure.</summary>
    public static bool IndonesiaNik(string id)
    {
        if (id.Length != 16) return false;
        for (int i = 0; i < 16; i++) if (id[i] < '0' || id[i] > '9') return false;

        int dd = (id[6] - '0') * 10 + (id[7] - '0');
        int mm = (id[8] - '0') * 10 + (id[9] - '0');
        if (dd > 40) dd -= 40;                 // female
        return dd >= 1 && dd <= 31 && mm >= 1 && mm <= 12;
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
