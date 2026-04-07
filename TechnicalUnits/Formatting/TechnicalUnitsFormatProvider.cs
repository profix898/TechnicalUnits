using System;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Formatting;

/// <summary>
/// A custom format provider that enables SI notation formatting in interpolated strings.
/// </summary>
/// <remarks>
/// <para>
/// Supports the following format specifiers:
/// <list type="bullet">
///   <item><c>SI</c> or <c>SI0</c>-<c>SI9</c> – SI notation with optional precision (e.g., "25n", "1.5k")</item>
/// </list>
/// </para>
/// <para>
/// Usage requires wrapping the interpolated string with the <see cref="FormattableStringExtensions.SI"/> 
/// extension method:
/// <code>
/// double thickness = 25e-9;
/// string result = SI($"Thickness = {thickness:SI}", "m");  // "Thickness = 25 nm"
/// string result2 = SI($"Value = {thickness:SI3}", "m");    // "Thickness = 25.0 nm"
/// </code>
/// </para>
/// </remarks>
public sealed class TechnicalUnitsFormatProvider : IFormatProvider, ICustomFormatter
{
    /// <summary>
    /// Gets the default shared instance of the format provider.
    /// </summary>
    public static TechnicalUnitsFormatProvider Default { get; } = new TechnicalUnitsFormatProvider();

    /// <summary>
    /// Gets or sets the unit options used for formatting. Default is dimensionless.
    /// </summary>
    public UnitOptions UnitOptions { get; set; } = new UnitOptions(SIUnits.Dimensionless);

    /// <summary>
    /// Gets or sets the formatting options. Default uses standard SI formatting.
    /// </summary>
    public FormattingOptions FormattingOptions { get; set; } = FormattingOptions.Default;

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
        => formatType == typeof(ICustomFormatter) ? this : null;

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (String.IsNullOrEmpty(format))
            return FormatDefault(arg, formatProvider);

        var upperFormat = format.ToUpperInvariant();

        // Check for SI format specifier
        if (upperFormat.StartsWith("SI"))
        {
            if (arg is not double
                && arg is not float
                && arg is not decimal
                && arg is not int
                && arg is not long
                && arg is not short
                && arg is not byte
                && arg is not uint
                && arg is not ulong
                && arg is not ushort
                && arg is not sbyte)
            {
                return FormatDefault(arg, formatProvider);
            }

            var value = Convert.ToDouble(arg);
            var options = GetFormattingOptionsWithPrecision(format, upperFormat);

            return Formatter.Format(value, UnitOptions, options);
        }

        // Fall back to default formatting for unrecognized formats
        return FormatDefault(arg, formatProvider);
    }

    private FormattingOptions GetFormattingOptionsWithPrecision(string format, string upperFormat)
    {
        // Extract precision from format (e.g., "SI3" -> 3)
        const int prefixLength = 2;

        if (format.Length > prefixLength && Int32.TryParse(format.Substring(prefixLength), out var precision))
        {
            var options = FormattingOptions.Clone();
            options.FractionalPrecision = precision;

            return options;
        }

        return FormattingOptions;
    }

    private static string FormatDefault(object? arg, IFormatProvider? formatProvider)
    {
        if (arg is IFormattable formattable)
            return formattable.ToString(null, formatProvider);

        return arg?.ToString() ?? String.Empty;
    }
}