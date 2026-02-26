using System.Collections.Generic;
using System.Globalization;
using TechnicalUnits.Formatting;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Extensions;

public static partial class UnitUtility
{
    /// <summary>
    /// Formats a numeric value with the specified unit using SI notation.
    /// </summary>
    /// <param name="unit">The unit to use for formatting.</param>
    /// <param name="value">The numeric value to format.</param>
    /// <param name="precision">The number of decimal places (default: 3).</param>
    /// <param name="cultureInfo">Optional culture info for number formatting.</param>
    /// <param name="formattingOptions">Optional formatting options.</param>
    /// <returns>A formatted string representation of the value with unit.</returns>
    public static string Format(this Unit unit, double value, int precision = 3, CultureInfo? cultureInfo = null, FormattingOptions? formattingOptions = null)
    {
        formattingOptions = (formattingOptions ?? FormattingOptions.Default).Clone();
        formattingOptions.FractionalPrecision = precision;
        if (cultureInfo != null)
        {
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        return Formatter.Format(value, unit, formattingOptions);
    }

    /// <summary>
    /// Parses a string representation of a value with the specified unit.
    /// </summary>
    /// <param name="unit">The expected unit for parsing.</param>
    /// <param name="strValue">The string to parse.</param>
    /// <param name="cultureInfo">Optional culture info for number parsing.</param>
    /// <param name="formattingOptions">Optional formatting options.</param>
    /// <returns>The parsed numeric value in the base unit.</returns>
    public static double Parse(this Unit unit, string strValue, CultureInfo? cultureInfo = null, FormattingOptions? formattingOptions = null)
    {
        formattingOptions = (formattingOptions ?? FormattingOptions.Default).Clone();
        if (cultureInfo != null)
        {
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        return Parser.ParseString(strValue, unit, formattingOptions);
    }

    /// <summary>
    /// Parses a string representation of a value with the specified unit and alternate units.
    /// </summary>
    /// <param name="unit">The primary unit for parsing.</param>
    /// <param name="strValue">The string to parse.</param>
    /// <param name="alternateUnits">A collection of alternate units that may appear in the string.</param>
    /// <param name="cultureInfo">Optional culture info for number parsing.</param>
    /// <param name="formattingOptions">Optional formatting options.</param>
    /// <returns>The parsed numeric value in the base unit.</returns>
    public static double Parse(this Unit unit, string strValue, IEnumerable<DerivedUnit> alternateUnits, CultureInfo? cultureInfo = null,
                               FormattingOptions? formattingOptions = null)
    {
        formattingOptions = (formattingOptions ?? FormattingOptions.Default).Clone();
        if (cultureInfo != null)
        {
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        var unitOptions = new UnitOptions(unit, alternateUnits);

        return Parser.ParseString(strValue, unitOptions, formattingOptions);
    }
}
