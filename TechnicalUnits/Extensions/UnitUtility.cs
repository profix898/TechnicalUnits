using System;
using System.Globalization;
using TechnicalUnits.Units;
using static System.Math;
using static TechnicalUnits.Internal.MathHelper;

namespace TechnicalUnits.Extensions;

public static class UnitUtility
{
    private static readonly string[] _siPrefixes = { "y", "z", "a", "f", "p", "n", "µ", "m", "", "k", "M", "G", "T", "P", "E", "Z", "Y" };

    /// <summary>Formats value in SI unit format (e.g. '10.0 mT')</summary>
    /// <param name="unit">SI unit to use for formatting.</param>
    /// <param name="value">Double-precision value.</param>
    /// <param name="precision">(Optional) Decimal precision (number of decimals).</param>
    /// <param name="shortenTrailingZeros">(Optional) Trailing zero decimals are shortened (to a single digit).</param>
    public static string FormatSimple(this Unit unit, double value, int precision = 3, bool shortenTrailingZeros = true, CultureInfo? cultureInfo = null)
    {
        return FormatSimple(unit.ToString(), value, precision, shortenTrailingZeros, cultureInfo);
    }

    public static string FormatSimple(string unit, double value, int precision = 3, bool shortenTrailingZeros = true, CultureInfo? cultureInfo = null)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));
        if (precision < 0)
            throw new ArgumentOutOfRangeException(nameof(precision));

        cultureInfo ??= CultureInfo.InvariantCulture;

        // Zero value
        if (value == 0.0)
            return String.Format(cultureInfo, "{0} {1}", value, unit);

        var valueAbs = Abs(value); // Prefixes based on absolute value (for negative values)
        var prefixRange = Log10(valueAbs) / 3.0;
        if (valueAbs < 1.0 && prefixRange < (int) prefixRange)
            prefixRange -= 1; // Log10(value / 1e3)
        var prefixGroup = (int) prefixRange;
        var divisor = Pow(10.0, prefixGroup * 3);

        prefixGroup += 8; // Offset in 'prefixes' list
        if (prefixGroup < 0 || prefixGroup >= _siPrefixes.Length)
            return String.Format(cultureInfo, "{0} {1}", value, unit);

        if (shortenTrailingZeros)
            precision = GetPrecision(valueAbs, divisor, precision);

        return String.Format(cultureInfo, "{0} {1}{2}", (value / divisor).ToString($"F{precision}", cultureInfo), _siPrefixes[prefixGroup], unit);
    }

    #region Private

    private static int GetPrecision(double value, double divisor, int precision)
    {
        var div = Round(10.0 * value / divisor, 3);
        for (var i = 1; i <= precision; i++)
        {
            if (GetDecimals(div) < 0.1)
                return i;

            div *= 10.0; // Left shift by one digit
        }

        return precision;
    }

    #endregion
}
