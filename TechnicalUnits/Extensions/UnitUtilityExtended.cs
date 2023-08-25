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
            formattingOptions.DecimalSeparator = cultureInfo?.NumberFormat.NumberDecimalSeparator ?? formattingOptions.DecimalSeparator;
            formattingOptions.GroupSeparator = cultureInfo?.NumberFormat.NumberGroupSeparator ?? formattingOptions.GroupSeparator;
        }

        return Formatter.Format(value, unit, formattingOptions);
    }

    public static double Parse(this Unit unit, string strValue, CultureInfo? cultureInfo = null)
    {
        var formattingOptions = FormattingOptions.Default;
        if (cultureInfo != null)
        {
            formattingOptions.DecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.GroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        return Parser.ParseString(strValue, unit, formattingOptions);
    }

    public static double Parse(this Unit unit, string strValue, IEnumerable<DerivedUnit> alternateUnits, CultureInfo? cultureInfo = null)
    {
        var formattingOptions = FormattingOptions.Default;
        if (cultureInfo != null)
        {
            formattingOptions.DecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            formattingOptions.GroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
        }

        var unitOptions = new UnitOptions(unit, alternateUnits);

        return Parser.ParseString(strValue, unitOptions, formattingOptions);
    }
}
