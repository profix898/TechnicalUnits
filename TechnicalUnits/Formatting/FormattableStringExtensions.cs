using System;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Formatting;

/// <summary>
/// Extension methods for formatting strings with SI notation.
/// </summary>
public static class FormattableStringExtensions
{
    /// <summary>
    /// Formats an interpolated string using SI notation for numeric values with :SI format specifier.
    /// </summary>
    /// <param name="formattable">The interpolated string to format.</param>
    /// <returns>The formatted string with SI prefixes applied.</returns>
    /// <example>
    /// <code>
    /// double value = 25e-9;
    /// string result = SI($"Thickness = {value:SI}m");  // "Thickness = 25 nm"
    /// </code>
    /// </example>
    public static string SI(FormattableString formattable)
        => formattable.ToString(TechnicalUnitsFormatProvider.Default);

    /// <summary>
    /// Formats an interpolated string using SI notation with a specified unit symbol.
    /// </summary>
    /// <param name="formattable">The interpolated string to format.</param>
    /// <param name="unitSymbol">The unit symbol to append (e.g., "m", "V", "Hz").</param>
    /// <returns>The formatted string with SI prefixes and unit applied.</returns>
    /// <example>
    /// <code>
    /// double thickness = 25e-9;
    /// string result = SI($"Thickness = {thickness:SI}", "m");  // "Thickness = 25 nm"
    /// </code>
    /// </example>
    public static string SI(FormattableString formattable, string unitSymbol)
    {
        var provider = new TechnicalUnitsFormatProvider
        {
            UnitOptions = new UnitOptions(new Unit(unitSymbol, unitSymbol, Dimension.Dimensionless))
        };

        return formattable.ToString(provider);
    }

    /// <summary>
    /// Formats an interpolated string using SI notation with a specified unit.
    /// </summary>
    /// <param name="formattable">The interpolated string to format.</param>
    /// <param name="unit">The unit to use for formatting.</param>
    /// <returns>The formatted string with SI prefixes and unit applied.</returns>
    /// <example>
    /// <code>
    /// double thickness = 25e-9;
    /// string result = SI($"Thickness = {thickness:SI}", SIUnits.Meter);  // "Thickness = 25 nm"
    /// </code>
    /// </example>
    public static string SI(FormattableString formattable, Unit unit)
    {
        var provider = new TechnicalUnitsFormatProvider
        {
            UnitOptions = new UnitOptions(unit)
        };

        return formattable.ToString(provider);
    }

    /// <summary>
    /// Formats an interpolated string using SI notation with a custom format provider.
    /// </summary>
    /// <param name="formattable">The interpolated string to format.</param>
    /// <param name="provider">The custom format provider to use.</param>
    /// <returns>The formatted string with SI prefixes applied.</returns>
    public static string SI(FormattableString formattable, TechnicalUnitsFormatProvider provider)
        => formattable.ToString(provider);
}
