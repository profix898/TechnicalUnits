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
        return ch == '+' || ch == '-';
    }

    public static bool IsNegativeSign(char ch)
    {
        // Note: Might seem trivial but can be extended to support unicode etc.
        return ch == '-';
    }

    public static bool IsNumeric(char ch)
    {
        return ch >= '0' && ch <= '9' || IsDigit(ch);
    }

    public static bool IsDecSep(string str, FormattingOptions formattingOptions)
    {
        return str == "." || str == "," || str == formattingOptions.DecimalSeparator;
    }

    public static bool IsBlank(char ch)
    {
        return ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n';
    }

    public static bool IsInvalidChar(char ch)
    {
        if (ch == '(' || ch == ')')
            return true;

        return false;
    }

    public static bool IsOperator(char ch)
    {
        return OperatorMathExpression.OperatorSymbols.Contains(ch);
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

    public static int IsUnit(string str, UnitOptions unitOptions, out double unitConvFactor)
    {
        unitConvFactor = 1.0;

        var unitSymbolLength = 0;
        if (StrEndsWithPattern(str, unitOptions.Unit.Symbol))
        {
            unitSymbolLength = unitOptions.Unit.Symbol.Length;

            return unitSymbolLength;
        }

        if (unitOptions.AlternateUnits.Count > 0)
        {
            foreach (var altUnit in unitOptions.AlternateUnits)
            {
                if (!StrEndsWithPattern(str, altUnit.Symbol))
                    continue;

                unitSymbolLength = altUnit.Symbol.Length;
                if (altUnit.BaseUnit == unitOptions.Unit)
                    unitConvFactor = altUnit.BaseConversionFactor;
                else
                    throw new AmbiguousUnitException("No conversion factor to base unit available.", altUnit);

                return unitSymbolLength;
            }
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
        {
            start = 0;
            length += start;
        }

        if (start + length > str.Length)
            length = str.Length - start;

        if (length > 0)
            return str.Substring(start, length);

        return String.Empty;

    }

    #endregion
}
