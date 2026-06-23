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

    /// <summary>Luhn algorithm (length-agnostic): callers anchor the length via their
    /// regex. Used by credit cards (13–19 digits) and Canada SIN (9 digits).</summary>
    public static bool Luhn(string digits)
    {
        if (digits.Length == 0) return false;
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

    /// <summary>Japan My Number (個人番号): 12 digits, weighted mod-11 check digit (last).
    /// VERIFY: My Number check-digit algorithm.</summary>
    public static bool JapanMyNumber(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 12) return false;
        int sum = 0;
        for (int n = 1; n <= 11; n++)
        {
            int p = d[11 - n] - '0';
            int q = n <= 6 ? n + 1 : n - 5;
            sum += p * q;
        }
        int rem = sum % 11;
        int check = rem <= 1 ? 0 : 11 - rem;
        return check == d[11] - '0';
    }

    /// <summary>Mexico CURP: 18 chars, weighted mod-10 check digit (last).
    /// VERIFY: CURP check-digit algorithm.</summary>
    public static bool MexicoCurp(string id)
    {
        const string dict = "0123456789ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        if (id.Length != 18) return false;
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = dict.IndexOf(char.ToUpperInvariant(id[i]));
            if (v < 0) return false;
            sum += v * (18 - i);
        }
        if (id[17] < '0' || id[17] > '9') return false;
        int check = (10 - sum % 10) % 10;
        return check == id[17] - '0';
    }

    /// <summary>Germany tax ID (Steuerliche Identifikationsnummer): 11 digits, ISO 7064
    /// MOD 11,10 check digit (last). VERIFY: IdNr check algorithm.</summary>
    public static bool GermanyTaxId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int product = 10;
        for (int i = 0; i < 10; i++)
        {
            int sum = (d[i] - '0' + product) % 10;
            if (sum == 0) sum = 10;
            product = sum * 2 % 11;
        }
        int check = (11 - product) % 10;
        return check == d[10] - '0';
    }

    /// <summary>Turkey TC Kimlik No: 11 digits. d10 and d11 are check digits.
    /// VERIFY: TC Kimlik algorithm.</summary>
    public static bool TurkeyId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11 || d[0] == '0') return false;
        int[] n = new int[11];
        for (int i = 0; i < 11; i++) n[i] = d[i] - '0';
        int odd = n[0] + n[2] + n[4] + n[6] + n[8];
        int even = n[1] + n[3] + n[5] + n[7];
        if ((odd * 7 - even) % 10 != n[9]) return false;
        int sum = 0;
        for (int i = 0; i < 10; i++) sum += n[i];
        return sum % 10 == n[10];
    }

    /// <summary>France NIR (numéro de sécurité sociale): 15 digits, key = 97 - (N mod 97)
    /// where N is the first 13 digits. Corsica (2A/2B) letters are not handled here.
    /// VERIFY: NIR key algorithm.</summary>
    public static bool FranceNir(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 15) return false;
        long n = 0;
        for (int i = 0; i < 13; i++) n = n * 10 + (d[i] - '0');
        int key = (int)(97 - n % 97);
        int given = (d[13] - '0') * 10 + (d[14] - '0');
        return key == given;
    }

    /// <summary>UK NHS number: 10 digits, weighted mod-11 check digit (last).
    /// A computed check of 10 is invalid. VERIFY: NHS check-digit algorithm.</summary>
    public static bool NhsNumber(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 10) return false;
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (d[i] - '0') * (10 - i);
        int check = 11 - sum % 11;
        if (check == 11) check = 0;
        if (check == 10) return false;
        return check == d[9] - '0';
    }

    /// <summary>Iran national ID (کد ملی): 10 digits, weighted mod-11 check digit.
    /// VERIFY: algorithm.</summary>
    public static bool IranId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 10 || d.All(c => c == d[0])) return false;
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (d[i] - '0') * (10 - i);
        int r = sum % 11;
        int check = r < 2 ? r : 11 - r;
        return check == d[9] - '0';
    }

    /// <summary>Thailand national ID: 13 digits, weighted mod-11 check digit.
    /// VERIFY: algorithm.</summary>
    public static bool ThailandId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 13) return false;
        int sum = 0;
        for (int i = 0; i < 12; i++) sum += (d[i] - '0') * (13 - i);
        int check = (11 - sum % 11) % 10;
        return check == d[12] - '0';
    }

    /// <summary>South Korea RRN (주민등록번호): 13 digits, weighted mod-11 check digit plus
    /// an embedded birth date (positions 0..5 = YYMMDD). VERIFY: algorithm.</summary>
    public static bool KoreaRrn(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 13) return false;
        int[] w = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5 };
        int sum = 0;
        for (int i = 0; i < 12; i++) sum += (d[i] - '0') * w[i];
        int check = (11 - sum % 11) % 10;
        if (check != d[12] - '0') return false;
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        int dd = (d[4] - '0') * 10 + (d[5] - '0');
        return mm >= 1 && mm <= 12 && dd >= 1 && dd <= 31;
    }

    /// <summary>Italy Codice Fiscale: 16 chars, check character from odd/even position maps.
    /// VERIFY: algorithm.</summary>
    public static bool ItalyCodiceFiscale(string id)
    {
        if (id.Length != 16) return false;
        int[] odd =
        {
            1, 0, 5, 7, 9, 13, 15, 17, 19, 21,                          // 0-9
            1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3,      // A-P
            6, 8, 12, 14, 16, 10, 22, 25, 24, 23,                        // Q-Z
        };
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            char c = char.ToUpperInvariant(id[i]);
            int idx = c >= '0' && c <= '9' ? c - '0'
                    : c >= 'A' && c <= 'Z' ? 10 + c - 'A'
                    : -1;
            if (idx < 0) return false;
            sum += i % 2 == 0 ? odd[idx] : idx < 10 ? idx : idx - 10;
        }
        return char.ToUpperInvariant(id[15]) == (char)('A' + sum % 26);
    }

    /// <summary>Egypt national ID: 14 digits — century (2/3) + YYMMDD + governorate +
    /// serial + (unused) check digit. No reliable public check digit, so we sanity-check
    /// the embedded date and governorate. VERIFY: structure.</summary>
    public static bool EgyptId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 14) return false;
        if (d[0] != '2' && d[0] != '3') return false;
        int mm = (d[3] - '0') * 10 + (d[4] - '0');
        int dd = (d[5] - '0') * 10 + (d[6] - '0');
        int gov = (d[7] - '0') * 10 + (d[8] - '0');
        if (mm < 1 || mm > 12 || dd < 1 || dd > 31) return false;
        return (gov >= 1 && gov <= 35) || gov == 88;   // 88 = born abroad
    }

    /// <summary>Spain DNI / NIE: 8 digits (DNI) or X/Y/Z + 7 digits (NIE), plus a control
    /// letter via mod-23. VERIFY: algorithm.</summary>
    public static bool SpainDniNie(string raw)
    {
        const string letters = "TRWAGMYFPDXBNJZSQVHLCKE";
        var s = raw.Replace("-", "").Replace(" ", "").ToUpperInvariant();
        if (s.Length != 9) return false;
        char last = s[8];
        if (last < 'A' || last > 'Z') return false;

        string digits;
        char f = s[0];
        if (f == 'X') digits = "0" + s.Substring(1, 7);
        else if (f == 'Y') digits = "1" + s.Substring(1, 7);
        else if (f == 'Z') digits = "2" + s.Substring(1, 7);
        else if (f >= '0' && f <= '9') digits = s.Substring(0, 8);
        else return false;

        return long.TryParse(digits, out var n) && letters[(int)(n % 23)] == last;
    }

    /// <summary>Australia TFN: 9 digits, weighted sum divisible by 11. VERIFY: algorithm.</summary>
    public static bool AustraliaTfn(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 9) return false;
        int[] w = { 1, 4, 3, 7, 5, 8, 6, 9, 10 };
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (d[i] - '0') * w[i];
        return sum % 11 == 0;
    }

    /// <summary>Netherlands BSN: 9 digits, "11-proef" (weights 9..2 then -1, divisible by 11).
    /// VERIFY: algorithm.</summary>
    public static bool NetherlandsBsn(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 9) return false;
        int[] w = { 9, 8, 7, 6, 5, 4, 3, 2, -1 };
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (d[i] - '0') * w[i];
        return sum % 11 == 0;
    }

    /// <summary>Poland PESEL: 11 digits, weighted mod-10 check digit + an embedded birth
    /// date (the month encodes the century). VERIFY: algorithm.</summary>
    public static bool PolandPesel(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int[] w = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for (int i = 0; i < 10; i++) sum += (d[i] - '0') * w[i];
        if ((10 - sum % 10) % 10 != d[10] - '0') return false;

        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        int dd = (d[4] - '0') * 10 + (d[5] - '0');
        bool monthOk = (mm >= 1 && mm <= 12) || (mm >= 21 && mm <= 32)
                    || (mm >= 41 && mm <= 52) || (mm >= 61 && mm <= 72) || (mm >= 81 && mm <= 92);
        return monthOk && dd >= 1 && dd <= 31;
    }

    /// <summary>New Zealand IRD number: 8 or 9 digits, IR's weighted algorithm with a
    /// secondary weighting fallback. VERIFY: IRD algorithm.</summary>
    public static bool NzIrd(string raw)
    {
        var s = Digits(raw);
        if (s.Length == 8) s = "0" + s;
        if (s.Length != 9) return false;
        if (!long.TryParse(s, out var n) || n < 10_000_000 || n > 150_000_000) return false;

        int[] w1 = { 3, 2, 7, 6, 5, 4, 3, 2 };
        int[] w2 = { 7, 4, 3, 2, 5, 2, 7, 6 };
        int Calc(int[] w)
        {
            int sum = 0;
            for (int i = 0; i < 8; i++) sum += (s[i] - '0') * w[i];
            int r = sum % 11;
            return r == 0 ? 0 : 11 - r;
        }
        int check = s[8] - '0';
        int c = Calc(w1);
        if (c == 10) { c = Calc(w2); if (c == 10) return false; }
        return c == check;
    }

    /// <summary>Sweden personnummer: 10 or 12 digits, Luhn over the 10-digit form plus a
    /// birth-date sanity check. VERIFY: algorithm.</summary>
    public static bool SwedenPersonnummer(string raw)
    {
        var d = Digits(raw);
        if (d.Length == 12) d = d.Substring(2);
        if (d.Length != 10) return false;
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        int dd = (d[4] - '0') * 10 + (d[5] - '0');
        if (mm < 1 || mm > 12 || dd < 1 || dd > 31) return false;
        return Luhn(d);
    }

    /// <summary>Norway fødselsnummer: 11 digits, two weighted mod-11 control digits.
    /// VERIFY: algorithm.</summary>
    public static bool NorwayFnr(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int Ctrl(int[] w, int len)
        {
            int sum = 0;
            for (int i = 0; i < len; i++) sum += (d[i] - '0') * w[i];
            int k = 11 - sum % 11;
            return k == 11 ? 0 : k;   // 10 stays 10 → caller rejects
        }
        int k1 = Ctrl(new[] { 3, 7, 6, 1, 8, 9, 4, 5, 2 }, 9);
        if (k1 == 10 || k1 != d[9] - '0') return false;
        int k2 = Ctrl(new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 }, 10);
        return k2 != 10 && k2 == d[10] - '0';
    }

    /// <summary>Denmark CPR: 10 digits (DDMMYY-SSSS). The mod-11 check has documented
    /// exceptions, so only the embedded birth date is sanity-checked. VERIFY: structure.</summary>
    public static bool DenmarkCpr(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 10) return false;
        int dd = (d[0] - '0') * 10 + (d[1] - '0');
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        return dd >= 1 && dd <= 31 && mm >= 1 && mm <= 12;
    }

    /// <summary>Finland HETU: DDMMYY + century sign + 3-digit individual + control char
    /// (number mod 31 indexed into a fixed alphabet). VERIFY: algorithm.</summary>
    public static bool FinlandHetu(string raw)
    {
        var s = raw.Trim().ToUpperInvariant();
        if (s.Length != 11) return false;
        const string alphabet = "0123456789ABCDEFHJKLMNPRSTUVWXY";
        for (int i = 0; i < 6; i++) if (s[i] < '0' || s[i] > '9') return false;
        for (int i = 7; i < 10; i++) if (s[i] < '0' || s[i] > '9') return false;

        int dd = (s[0] - '0') * 10 + (s[1] - '0');
        int mm = (s[2] - '0') * 10 + (s[3] - '0');
        if (dd < 1 || dd > 31 || mm < 1 || mm > 12) return false;

        long number = long.Parse(s.Substring(0, 6) + s.Substring(7, 3));
        return alphabet[(int)(number % 31)] == s[10];
    }

    /// <summary>South Africa ID: 13 digits, Luhn plus an embedded birth date (YYMMDD).
    /// VERIFY: algorithm.</summary>
    public static bool SouthAfricaId(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 13) return false;
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        int dd = (d[4] - '0') * 10 + (d[5] - '0');
        if (mm < 1 || mm > 12 || dd < 1 || dd > 31) return false;
        return Luhn(d);
    }

    /// <summary>Argentina CUIT/CUIL: 11 digits, weighted mod-11 check digit. VERIFY.</summary>
    public static bool ArgentinaCuit(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int[] w = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        for (int i = 0; i < 10; i++) sum += (d[i] - '0') * w[i];
        int check = 11 - sum % 11;
        if (check == 11) check = 0;
        if (check == 10) return false;
        return check == d[10] - '0';
    }

    /// <summary>Chile RUT/RUN: body + check char (0-9 or K) via mod-11 with weights
    /// cycling 2..7 from the right. VERIFY.</summary>
    public static bool ChileRut(string raw)
    {
        var s = raw.Replace(".", "").Replace("-", "").ToUpperInvariant();
        if (s.Length < 2) return false;
        char checkChar = s[s.Length - 1];
        string body = s.Substring(0, s.Length - 1);
        foreach (char c in body) if (c < '0' || c > '9') return false;

        int sum = 0, w = 2;
        for (int i = body.Length - 1; i >= 0; i--)
        {
            sum += (body[i] - '0') * w;
            w = w == 7 ? 2 : w + 1;
        }
        int m = 11 - sum % 11;
        char expected = m == 11 ? '0' : m == 10 ? 'K' : (char)('0' + m);
        return expected == checkChar;
    }

    /// <summary>Hong Kong HKID: 1–2 letters + 6 digits + check (digit or A), weighted mod-11.
    /// A single leading letter is padded with a space (value 36). VERIFY.</summary>
    public static bool HongKongId(string raw)
    {
        var s = raw.ToUpperInvariant().Replace("(", "").Replace(")", "").Replace(" ", "");
        if (s.Length != 8 && s.Length != 9) return false;
        char checkChar = s[s.Length - 1];
        string prefix = s.Substring(0, s.Length - 1);   // 7 or 8 chars
        if (prefix.Length == 7) prefix = " " + prefix;   // pad single-letter form

        int Val(char c) => c == ' ' ? 36 : c >= 'A' && c <= 'Z' ? c - 'A' + 10
                          : c >= '0' && c <= '9' ? c - '0' : -1;
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int v = Val(prefix[i]);
            if (v < 0) return false;
            sum += v * (9 - i);
        }
        int check = (11 - sum % 11) % 11;
        char expected = check == 10 ? 'A' : (char)('0' + check);
        return expected == checkChar;
    }

    private static readonly Dictionary<char, int> TwLetters = new()
    {
        ['A'] = 10, ['B'] = 11, ['C'] = 12, ['D'] = 13, ['E'] = 14, ['F'] = 15, ['G'] = 16,
        ['H'] = 17, ['I'] = 34, ['J'] = 18, ['K'] = 19, ['L'] = 20, ['M'] = 21, ['N'] = 22,
        ['O'] = 35, ['P'] = 23, ['Q'] = 24, ['R'] = 25, ['S'] = 26, ['T'] = 27, ['U'] = 28,
        ['V'] = 29, ['W'] = 32, ['X'] = 30, ['Y'] = 31, ['Z'] = 33,
    };

    /// <summary>Taiwan national ID: 1 letter + 9 digits (last is check), weighted mod-10.
    /// VERIFY.</summary>
    public static bool TaiwanId(string raw)
    {
        var s = raw.ToUpperInvariant();
        if (s.Length != 10 || !TwLetters.TryGetValue(s[0], out int n)) return false;
        int sum = n / 10 + n % 10 * 9;
        int[] w = { 8, 7, 6, 5, 4, 3, 2, 1, 1 };
        for (int i = 0; i < 9; i++)
        {
            if (s[1 + i] < '0' || s[1 + i] > '9') return false;
            sum += (s[1 + i] - '0') * w[i];
        }
        return sum % 10 == 0;
    }

    /// <summary>Portugal NIF: 9 digits, weighted mod-11 check digit. VERIFY.</summary>
    public static bool PortugalNif(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 9) return false;
        int sum = 0;
        for (int i = 0; i < 8; i++) sum += (d[i] - '0') * (9 - i);
        int r = sum % 11;
        int check = r < 2 ? 0 : 11 - r;
        return check == d[8] - '0';
    }

    /// <summary>EAN-13 / GTIN-13 mod-10 check (used by Switzerland AHV "756…"). VERIFY.</summary>
    public static bool Ean13(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 13) return false;
        int sum = 0;
        for (int i = 0; i < 12; i++) sum += (d[i] - '0') * (i % 2 == 0 ? 1 : 3);
        int check = (10 - sum % 10) % 10;
        return check == d[12] - '0';
    }

    /// <summary>Belgium national register number: 11 digits; last 2 = 97 − (first 9 mod 97),
    /// with a "2" prepended for births from 2000. VERIFY.</summary>
    public static bool BelgiumNn(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        long n9 = long.Parse(d.Substring(0, 9));
        int given = int.Parse(d.Substring(9, 2));
        int c1 = 97 - (int)(n9 % 97);
        int c2 = 97 - (int)((2_000_000_000L + n9) % 97);
        return given == c1 || given == c2;
    }

    /// <summary>Czech/Slovak rodné číslo: 10 digits, whole number divisible by 11, with an
    /// embedded birth date (month +50 female, +20/+70 special cases). VERIFY.</summary>
    public static bool CzechRc(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 10) return false;
        long n = long.Parse(d);
        if (n % 11 != 0) return false;
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        if (mm > 70) mm -= 70; else if (mm > 50) mm -= 50; else if (mm > 20) mm -= 20;
        int dd = (d[4] - '0') * 10 + (d[5] - '0');
        return mm >= 1 && mm <= 12 && dd >= 1 && dd <= 31;
    }

    /// <summary>Ireland PPS number: 7 digits + check letter (+ optional 2nd letter), mod-23.
    /// VERIFY.</summary>
    public static bool IrelandPps(string raw)
    {
        var s = raw.ToUpperInvariant();
        if (s.Length < 8 || s.Length > 9) return false;
        for (int i = 0; i < 7; i++) if (s[i] < '0' || s[i] > '9') return false;
        char checkLetter = s[7];
        int sum = 0;
        for (int i = 0; i < 7; i++) sum += (s[i] - '0') * (8 - i);
        if (s.Length == 9)
        {
            char w = s[8];
            if (w < 'A' || w > 'W') return false;
            sum += (w == 'W' ? 0 : w - 'A' + 1) * 9;
        }
        int rem = sum % 23;
        char expected = rem == 0 ? 'W' : (char)('A' + rem - 1);
        return expected == checkLetter;
    }

    /// <summary>Estonia isikukood: 11 digits, two-pass weighted mod-11 check. VERIFY.</summary>
    public static bool EstoniaIk(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 11) return false;
        int Weighted(int[] w)
        {
            int sum = 0;
            for (int i = 0; i < 10; i++) sum += (d[i] - '0') * w[i];
            return sum % 11;
        }
        int r = Weighted(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 });
        int check = r < 10 ? r : Weighted(new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 });
        if (check == 10) check = 0;
        return check == d[10] - '0';
    }

    /// <summary>Iceland kennitala: 10 digits, weighted mod-11 check at position 9 plus an
    /// embedded birth date (DDMMYY). VERIFY.</summary>
    public static bool IcelandKennitala(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 10) return false;
        int[] w = { 3, 2, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        for (int i = 0; i < 8; i++) sum += (d[i] - '0') * w[i];
        int check = 11 - sum % 11;
        if (check == 11) check = 0;
        if (check == 10 || check != d[8] - '0') return false;
        int dd = (d[0] - '0') * 10 + (d[1] - '0');
        int mm = (d[2] - '0') * 10 + (d[3] - '0');
        return dd >= 1 && dd <= 31 && mm >= 1 && mm <= 12;
    }

    /// <summary>Slovenia/ex-Yugoslav EMŠO (JMBG): 13 digits, weighted mod-11 check digit.
    /// VERIFY.</summary>
    public static bool SloveniaEmso(string raw)
    {
        var d = Digits(raw);
        if (d.Length != 13) return false;
        int[] w = { 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        for (int i = 0; i < 12; i++) sum += (d[i] - '0') * w[i];
        int m = sum % 11;
        int check = m == 0 ? 0 : 11 - m;
        return check < 10 && check == d[12] - '0';
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
