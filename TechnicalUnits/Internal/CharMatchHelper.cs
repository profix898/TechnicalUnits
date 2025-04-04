using System;
using System.Linq;
using TechnicalUnits.Formatting;
using TechnicalUnits.Math.Expressions;
using static System.Char;

namespace TechnicalUnits.Internal;

internal static class CharMatchHelper
{
    #region IsChecks

    public static bool IsSign(char ch)
    {
        return ch is '+' or '-';
    }

    public static bool IsNegativeSign(char ch)
    {
        return ch is '-'; // Note: Might seem trivial but can be extended to support unicode etc.
    }

    public static bool IsNumeric(char ch)
    {
        return ch is >= '0' and <= '9' || IsDigit(ch);
    }

    public static bool IsDecSep(string str, FormattingOptions formattingOptions)
    {
        return str == "." || str == "," || str == formattingOptions.DecimalSeparator;
    }

    public static bool IsBlank(char ch)
    {
        return ch is ' ' or '\t' or '\r' or '\n';
    }

    public static bool IsInvalidChar(char ch)
    {
        return ch is '(' or ')';
    }

    public static bool IsOperator(char ch)
    {
        return OperatorExpression.OperatorSymbols.Contains(ch);
    }

    public static bool IsValidUnitChar(char ch, UnitOptions unitOptions)
    {
        if (unitOptions.Unit.Symbol.Contains(new string(ch, 1)))
            return true;

        if (unitOptions.AlternateUnits.Count > 0)
        {
            foreach (var altUnit in unitOptions.AlternateUnits)
            {
                if (altUnit.Symbol.Contains(new string(ch, 1)))
                    return true;
            }
        }

        return false;
    }

    public static int IsUnit(string str, UnitOptions unitOptions, ref double unitConvFactor)
    {
        var unitSymbolLength = 0;
        if (StrEndsWithPattern(str, unitOptions.Unit.Symbol))
        {
            unitConvFactor = 1.0;
            unitSymbolLength = unitOptions.Unit.Symbol.Length;

            return unitSymbolLength;
        }

        if (unitOptions.AlternateUnits.Count <= 0)
            return unitSymbolLength;

        foreach (var altUnit in unitOptions.AlternateUnits.Where(altUnit => StrEndsWithPattern(str, altUnit.Symbol)))
        {
            unitSymbolLength = altUnit.Symbol.Length;
            if (altUnit.BaseUnit == unitOptions.Unit)
                unitConvFactor = altUnit.BaseConversionFactor;
            else
                throw new AmbiguousUnitException("No conversion factor to base unit available.", altUnit);

            return unitSymbolLength;
        }

        return unitSymbolLength;
    }

    #endregion

    #region StringMatch

    public static bool StrEndsWithPattern(string str, string pattern)
    {
        if (pattern.Length == 0)
            return false;

        if (str.Length <= pattern.Length)
            return str == pattern;

        return str.Substring(str.Length - pattern.Length, pattern.Length) == pattern;
    }

    public static string SubstringTolerant(string str, int start, int length)
    {
        if (start < 0)
            start = 0;

        if (start + length > str.Length)
            length = str.Length - start;

        if (length > 0)
            return str.Substring(start, length);

        return String.Empty;
    }

    #endregion
}
