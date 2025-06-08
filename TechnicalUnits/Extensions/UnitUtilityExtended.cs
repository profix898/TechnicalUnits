using System.Collections.Generic;
using System.Globalization;
using TechnicalUnits.Formatting;
using TechnicalUnits.Units;

namespace TechnicalUnits.Extensions;

public static class UnitUtilityExtended
{
    public static string Format(this Unit unit, double value, int precision = 3, CultureInfo? cultureInfo = null)
    {
        var formattingOptions = FormattingOptions.Default;
        if (precision != formattingOptions.FractionalPrecision || cultureInfo != null)
        {
            formattingOptions.FractionalPrecision = precision;
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo?.NumberFormat.NumberDecimalSeparator ?? formattingOptions.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo?.NumberFormat.NumberGroupSeparator ?? formattingOptions.NumberFormat.NumberGroupSeparator;
        }

        return Formatter.Format(value, unit, formattingOptions);
    }

    public static double Parse(this Unit unit, string strValue, CultureInfo? cultureInfo = null)
    {
        var formattingOptions = FormattingOptions.Default;
        if (cultureInfo != null)
        {
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        return Parser.ParseString(strValue, unit, formattingOptions);
    }

    public static double Parse(this Unit unit, string strValue, IEnumerable<DerivedUnit> alternateUnits, CultureInfo? cultureInfo = null)
    {
        var formattingOptions = FormattingOptions.Default;
        if (cultureInfo != null)
        {
            formattingOptions.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        var unitOptions = new UnitOptions(unit, alternateUnits);

        return Parser.ParseString(strValue, unitOptions, formattingOptions);
    }
}
