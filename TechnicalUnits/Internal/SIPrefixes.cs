using System;
using System.Collections.Generic;
using TechnicalUnits.Formatting;

namespace TechnicalUnits.Internal;

/// <summary>
/// Provides SI prefix symbols, English names, exponent values, and lookup methods
/// used by the <see cref="Formatter" /> and <see cref="Parser" />.
/// </summary>
internal static class SIPrefixes
{
    #region Prefixes

    /// <summary>SI prefix exponents, ordered from yocto (−24) to yotta (+24), including centi/deci/deca/hecto.</summary>
    public static readonly int[] Exponents =
    [
        -24, -21, -18, -15, -12, -9, -6, -3, -2, -1,
        1, 2, 3, 6, 9, 12, 15, 18, 21, 24
    ];

    /// <summary>SI prefix symbols (Unicode), index-aligned with <see cref="Exponents" />.</summary>
    public static readonly string[] PrefixesSI =
    [
        "y", "z", "a", "f", "p", "n", "µ", "m", "c", "d",
        "", "h", "k", "M", "G", "T", "P", "E", "Z", "Y"
    ];

    /// <summary>SI prefix symbols (ASCII-safe, e.g. "u" for micro), index-aligned with <see cref="Exponents" />.</summary>
    public static readonly string[] PrefixesSIAz =
    [
        "y", "z", "a", "f", "p", "n", "u", "m", "c", "d",
        "", "h", "k", "M", "G", "T", "P", "E", "Z", "Y"
    ];

    /// <summary>SI prefix English names, index-aligned with <see cref="Exponents" />.</summary>
    public static readonly string[] PrefixesSINames =
    [
        "yocto", "zepto", "atto", "femto", "pico", "nano", "micro", "milli", "centi", "deci",
        "deca", "hecto", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta", "yotta"
    ];

    private static readonly HashSet<char> ValidSIChars = BuildValidSIChars();
    private static readonly Dictionary<char, int> SingleCharPrefixMap = BuildSingleCharPrefixMap();

    private static HashSet<char> BuildValidSIChars()
    {
        var chars = new HashSet<char>();
        foreach (var prefix in PrefixesSI)
        {
            foreach (var ch in prefix)
                chars.Add(ch);
        }
        foreach (var prefix in PrefixesSINames)
        {
            foreach (var ch in prefix)
                chars.Add(ch);
        }
        foreach (var prefix in PrefixesSIAz)
        {
            foreach (var ch in prefix)
                chars.Add(ch);
        }
        return chars;
    }

    private static Dictionary<char, int> BuildSingleCharPrefixMap()
    {
        var map = new Dictionary<char, int>();
        for (var i = 0; i < PrefixesSI.Length; i++)
        {
            if (PrefixesSI[i].Length == 1 && Exponents[i] != 0)
                map.TryAdd(PrefixesSI[i][0], Exponents[i]);
        }
        for (var i = 0; i < PrefixesSIAz.Length; i++)
        {
            if (PrefixesSIAz[i].Length == 1 && Exponents[i] != 0)
                map.TryAdd(PrefixesSIAz[i][0], Exponents[i]);
        }
        return map;
    }

    #endregion

    #region Checks

    /// <summary>
    /// Determines whether <paramref name="ch" /> is a recognised single-character SI prefix
    /// and returns the corresponding exponent via <paramref name="exp" />.
    /// </summary>
    /// <param name="ch">The character to test.</param>
    /// <param name="exp">When the method returns <c>true</c>, the base-10 exponent; otherwise 0.</param>
    /// <returns><c>true</c> if a matching non-zero exponent prefix was found.</returns>
    public static bool IsSIPrefix(char ch, out int exp) => SingleCharPrefixMap.TryGetValue(ch, out exp);

    /// <summary>
    /// Determines whether <paramref name="str" /> is a recognised SI prefix (symbol or name)
    /// and returns the corresponding exponent via <paramref name="exp" />.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <param name="exp">When the method returns <c>true</c>, the base-10 exponent; otherwise 0.</param>
    /// <returns><c>true</c> if a matching non-zero exponent prefix was found.</returns>
    public static bool IsSIPrefix(string str, out int exp)
    {
        exp = 0;

        if (String.IsNullOrEmpty(str))
            return false;

        var index = Array.IndexOf(PrefixesSI, str);
        if (index < 0)
            index = Array.IndexOf(PrefixesSINames, str);
        if (index < 0)
            index = Array.IndexOf(PrefixesSIAz, str);

        if (index >= 0)
            exp = Exponents[index];

        return exp != 0;
    }

    /// <summary>
    /// Determines whether <paramref name="ch" /> appears in any known SI prefix
    /// symbol or name (used to decide whether to continue lookahead scanning).
    /// </summary>
    public static bool IsValidSIChar(char ch) => ValidSIChars.Contains(ch);

    /// <summary>
    /// Returns the SI prefix string for the given exponent, respecting the active
    /// <see cref="FormattingOptions.SIStyle" />.
    /// </summary>
    /// <param name="exp">The base-10 exponent (must exist in <see cref="Exponents" />).</param>
    /// <param name="unitOptions">Unit options (used when <see cref="FormattingOptions.PrefixOrUnitAsDecimalSeparator" /> is <c>true</c>).</param>
    /// <param name="formattingOptions">Formatting options controlling the prefix style.</param>
    /// <param name="unitPlaced"><c>true</c> if the unit symbol was placed as decimal separator.</param>
    /// <returns>The SI prefix string.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="exp" /> has no matching SI prefix.</exception>
    public static string GetSIPrefix(int exp, UnitOptions unitOptions, FormattingOptions formattingOptions, out bool unitPlaced)
    {
        unitPlaced = false;

        if (exp == 0)
        {
            if (!formattingOptions.PrefixOrUnitAsDecimalSeparator)
                return String.Empty;

            if (unitOptions.Unit.Symbol.Length > 0 && unitOptions.Unit.Symbol != "1") // Ignore 'dimensionless' unit
            {
                unitPlaced = true;

                return unitOptions.Unit.Symbol;
            }

            return formattingOptions.NumberFormat.NumberDecimalSeparator;
        }

        var index = Array.IndexOf(Exponents, exp);
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(exp), "No SI Prefix for the value of exp.");

        return formattingOptions.SIStyle switch
        {
            SIStyles.SIStyleSI => PrefixesSI[index],
            SIStyles.SIStyleSINamesEN => PrefixesSINames[index],
            SIStyles.SIStyleSIAz => PrefixesSIAz[index],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>Determines whether <paramref name="str" /> is an exponent prefix (<c>"e"</c> or <c>"E"</c>).</summary>
    public static bool IsExpPrefix(string str) => str is "e" or "E";

    /// <summary>Determines whether <paramref name="ch" /> is an exponent prefix character (<c>'e'</c> or <c>'E'</c>).</summary>
    public static bool IsExpPrefix(char ch) => ch is 'e' or 'E';

    /// <summary>
    /// Checks whether any single character in <paramref name="str" /> could be
    /// mistaken for an SI prefix symbol.
    /// </summary>
    /// <param name="str">The string to scan (typically a unit symbol).</param>
    /// <param name="prefix">When the method returns <c>true</c>, the ambiguous prefix character.</param>
    /// <returns><c>true</c> if an SI-prefix-like character was found.</returns>
    public static bool ContainsSIPrefix(string str, out string prefix)
    {
        prefix = "";

        if (IsSIPrefix(str, out _))
        {
            prefix = str;
            return true;
        }

        for (var k = 0; k < str.Length; k++)
        {
            var ch = str[k];
            if (IsExpPrefix(ch) || IsSIPrefix(ch, out _))
            {
                prefix = ch.ToString();
                return true;
            }
        }

        return false;
    }

    #endregion
}
