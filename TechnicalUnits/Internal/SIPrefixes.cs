using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalUnits.Formatting;

namespace TechnicalUnits.Internal;

internal static class SIPrefixes
{
    #region Prefixes

    public static readonly int[] Exponents =
    {
        -24,
        -21,
        -18,
        -15,
        -12,
        -9,
        -6,
        -3,
        -2,
        -1,
        1,
        2,
        3,
        6,
        9,
        12,
        15,
        18,
        21,
        24
    };

    public static readonly string[] PrefixesSI =
    {
        "y",
        "z",
        "a",
        "f",
        "p",
        "n",
        "µ",
        "m",
        "c",
        "d",
        "",
        "h",
        "k",
        "M",
        "G",
        "T",
        "P",
        "E",
        "Z",
        "Y"
    };

    public static readonly string[] PrefixesSIAz =
    {
        "y",
        "z",
        "a",
        "f",
        "p",
        "n",
        "u",
        "m",
        "c",
        "d",
        "",
        "h",
        "k",
        "M",
        "G",
        "T",
        "P",
        "E",
        "Z",
        "Y"
    };

    public static readonly string[] PrefixesSINames =
    {
        "yocto",
        "zepto",
        "atto",
        "femto",
        "pico",
        "nano",
        "micro",
        "milli",
        "centi",
        "deci",
        "deca",
        "hecto",
        "kilo",
        "mega",
        "giga",
        "tera",
        "peta",
        "exa",
        "zetta",
        "yotta"
    };

    #endregion

    #region Checks

    public static bool IsSIPrefix(string str, out int exp)
    {
        exp = 0;

        if (String.IsNullOrEmpty(str))
            return false;

        if (PrefixesSI.Contains(str, out var index))
            exp = Exponents[index];
        else if (PrefixesSINames.Contains(str, out index))
            exp = Exponents[index];
        else if (PrefixesSIAz.Contains(str, out index))
            exp = Exponents[index];

        return (exp != 0);
    }

    public static bool IsValidSIChar(char ch) // Checks whether c is contained in _any_ SI prefix
    {
        var str = ch.ToString();

        if (String.IsNullOrEmpty(str))
            return false;

        if (PrefixesSI.Contains(str))
            return true;
        if (PrefixesSINames.Contains(str))
            return true;
        if (PrefixesSIAz.Contains(str))
            return true;

        return false;
    }

    public static string GetSIPrefix(int exp, UnitOptions unitOptions, FormattingOptions formattingOptions, out bool unitPlaced)
    {
        unitPlaced = false;

        if (exp == 0)
        {
            if (!formattingOptions.PrefixOrUnitAsDecimalSeparator)
                return String.Empty;

            if (unitOptions.Unit.Symbol.Length > 0
                && unitOptions.Unit.Symbol != "1") // Ignore 'dimensionless' unit
            {
                unitPlaced = true;

                return unitOptions.Unit.Symbol;
            }

            return formattingOptions.NumberFormat.NumberDecimalSeparator;

        }

        if (!Exponents.Contains(exp, out var index))
            throw new ArgumentOutOfRangeException(nameof(exp), "No SI Prefix for the value of exp.");

        return formattingOptions.SIStyle switch
        {
            SIStyles.SIStyleSI => PrefixesSI[index],
            SIStyles.SIStyleSINamesEN => PrefixesSINames[index],
            SIStyles.SIStyleSIAz => PrefixesSIAz[index],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static bool IsExpPrefix(string str)
    {
        return str is "e" or "E";
    }

    public static bool ContainsSIPrefix(string str, out string prefix)
    {
        var containsPrefix = false;
        prefix = "";

        if (IsSIPrefix(str, out _))
            return true;

        for (var k = 0; k < str.Length; k++)
        {
            if (IsExpPrefix(str.Substring(k, 1)) || IsSIPrefix(str.Substring(k, 1), out _))
            {
                containsPrefix = true;
                prefix = str.Substring(k, 1);
                break;
            }
        }

        // TODO: Evaluate more sophisticated checks for subparts of str

        return containsPrefix;
    }

    #endregion

    #region Private

    private static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, out int index, IEqualityComparer<TSource>? comparer = null)
    {
        index = 0;

        if (comparer == null)
        {
            foreach (var element in source)
            {
                if (EqualityComparer<TSource>.Default.Equals(element, value))
                    return true;

                index++;
            }
        }
        else
        {
            foreach (var element in source)
            {
                if (comparer.Equals(element, value))
                    return true;

                index++;
            }
        }

        index = -1;

        return false;
    }

    #endregion
}
