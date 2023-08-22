using System;
using System.Globalization;
using TechnicalUnits.Internal;
using TechnicalUnits.Units;
using static System.Math;

namespace TechnicalUnits.Extensions;

public static class UnitUtility
{
    /// <summary>Formats value in SI unit format (e.g. '10.0 mT')</summary>
    /// <param name="unit">SI unit to use for formatting.</param>
    /// <param name="value">Double-precision value.</param>
    /// <param name="precision">(Optional) Decimal precision (number of decimals).</param>
    /// <param name="shortenTrailingZeros">(Optional) Trailing zero decimals are shortened (to a single digit).</param>
    public static string Format(this Unit unit, double value, int precision = 3, bool shortenTrailingZeros = true, CultureInfo? cultureInfo = null)
    {
        return Format(unit.ToString(), value, precision, shortenTrailingZeros, cultureInfo);
    }

    public static string Format(string unit, double value, int precision = 3, bool shortenTrailingZeros = true, CultureInfo? cultureInfo = null)
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
        if (valueAbs < 1.0 && prefixRange < (int)prefixRange)
            prefixRange -= 1; // Log10(value / 1e3)
        var prefixGroup = (int)prefixRange;
        var divisor = Pow(10.0, prefixGroup * 3);

        prefixGroup += 10; // Offset in 'prefixes' list
        if (prefixGroup < 0 || prefixGroup >= SIPrefixes.PrefixesSI.Length)
            return String.Format(cultureInfo, "{0} {1}", value, unit);

        // If all decimals within requested precision are zero, shorten to single digit/decimal precision
        if (shortenTrailingZeros && IEEERemainder(valueAbs, divisor) * Pow(10, precision) <= 1.0)
            precision = 1;

        return String.Format(cultureInfo, "{0} {1}{2}", (value / divisor).ToString($"F{precision}", cultureInfo), SIPrefixes.PrefixesSI[prefixGroup], unit);
    }
}
