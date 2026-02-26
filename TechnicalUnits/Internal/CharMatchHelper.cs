using System;
using TechnicalUnits.Formatting;
using TechnicalUnits.Math.Expressions;
using static System.Char;

namespace TechnicalUnits.Internal;

/// <summary>
/// Helper methods for character and string classification used by the <see cref="Parser" />.
/// </summary>
internal static class CharMatchHelper
{
    #region IsChecks

    /// <summary>Determines whether <paramref name="ch" /> is a sign character (<c>+</c> or <c>-</c>).</summary>
    public static bool IsSign(char ch) => ch is '+' or '-';

    /// <summary>Determines whether <paramref name="ch" /> is a negative sign (<c>-</c>).</summary>
    public static bool IsNegativeSign(char ch) => ch is '-';

    /// <summary>Determines whether <paramref name="ch" /> is a numeric digit.</summary>
    public static bool IsNumeric(char ch) => ch is >= '0' and <= '9' || IsDigit(ch);

    /// <summary>Determines whether <paramref name="str" /> is a decimal separator for the given formatting options.</summary>
    public static bool IsDecSep(string str, FormattingOptions formattingOptions) => str == "." || str == "," || str == formattingOptions.NumberFormat.NumberDecimalSeparator;

    /// <summary>Determines whether <paramref name="str" /> is a decimal separator (span overload, avoids string allocation).</summary>
    public static bool IsDecSep(ReadOnlySpan<char> str, FormattingOptions formattingOptions)
    {
        if (str.Length == 1 && str[0] is '.' or ',')
            return true;

        return str.SequenceEqual(formattingOptions.NumberFormat.NumberDecimalSeparator.AsSpan());
    }

    /// <summary>Determines whether <paramref name="ch" /> is a whitespace character.</summary>
    public static bool IsBlank(char ch) => ch is ' ' or '\t' or '\r' or '\n';

    /// <summary>Determines whether <paramref name="ch" /> is an invalid (grouping) character for parser context.</summary>
    public static bool IsInvalidChar(char ch) => ch is '(' or ')';

    /// <summary>Determines whether <paramref name="ch" /> is a math operator character.</summary>
    public static bool IsOperator(char ch) => OperatorExpression.IsOperator(ch);

    /// <summary>
    /// Determines whether <paramref name="ch" /> appears in the symbol of the primary or any alternate unit.
    /// </summary>
    public static bool IsValidUnitChar(char ch, UnitOptions unitOptions)
    {
        if (unitOptions.Unit.Symbol.Contains(ch))
            return true;

        if (unitOptions.AlternateUnits.Count > 0)
        {
            foreach (var altUnit in unitOptions.AlternateUnits)
            {
                if (altUnit.Symbol.Contains(ch))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether <paramref name="str" /> ends with a known unit symbol and, if so,
    /// sets <paramref name="unitConvFactor" /> for alternate-unit conversion.
    /// </summary>
    /// <returns>The length of the matched unit symbol, or 0 if no match.</returns>
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

        return unitSymbolLength;
    }

    #endregion

    #region StringMatch

    /// <summary>
    /// Returns <c>true</c> if <paramref name="str" /> ends with <paramref name="pattern" /> (ordinal comparison).
    /// Returns <c>false</c> for empty patterns.
    /// </summary>
    public static bool StrEndsWithPattern(string str, string pattern) => pattern.Length > 0 && str.EndsWith(pattern, StringComparison.Ordinal);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="str" /> ends with <paramref name="pattern" /> (span overload).
    /// Returns <c>false</c> for empty patterns.
    /// </summary>
    public static bool StrEndsWithPattern(ReadOnlySpan<char> str, ReadOnlySpan<char> pattern) => pattern.Length > 0 && str.EndsWith(pattern);

    /// <summary>
    /// Returns a substring that clamps <paramref name="start" /> and <paramref name="length" />
    /// to the valid range instead of throwing.
    /// </summary>
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
