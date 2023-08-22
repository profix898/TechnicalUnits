using System;
using System.Globalization;
using System.Text;
using TechnicalUnits.Internal;
using static System.Math;
using static TechnicalUnits.Internal.MathUtility;

namespace TechnicalUnits.Formatting;

public static class Formatter
{
    #region Format

    public static string Format(double value, UnitOptions unitOptions, FormattingOptions? formattingOptions = null)
    {
        formattingOptions ??= FormattingOptions.Default;

        int sign;

        if (value < 0)
        {
            value *= -1;
            sign = -1;
        }
        else
            sign = 1;

        // Round to significant digits
        value = Round(value, formattingOptions.SignificantDigits);

        // Case: SIStyleFloat (simple number formatting)
        if (formattingOptions.SIStyle == SIStyles.SIStyleFloat)
        {
            var formatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = formattingOptions.DecimalSeparator,
                NumberGroupSeparator = formattingOptions.GroupSeparator
            };

            var resultString = new StringBuilder((sign * value).ToString($"g{formattingOptions.SignificantDigits + 1:D}", formatInfo));
            resultString.Append(formattingOptions.UnitSeparator);
            resultString.Append(unitOptions.Unit);

            return resultString.ToString();
        }

        // Determine the exponent multiples of 3 that matches the value
        var exp3 = GetExp3Value(value);

        // Remove the exponent from the number
        value /= Pow(1000.0, exp3);

        // There should be a maximum of 3 places before the double
        if (Abs(value) > 1000.0)
            throw new Exception("Internal Error: More than three orders of magnitude before the decimal separator.");

        var preDec = Truncate(value);
        var postDec = value - Truncate(value);
        postDec = Round(postDec * Pow(10, formattingOptions.FractionalPrecision));

        if (postDec >= Pow(10, formattingOptions.FractionalPrecision))
        {
            // Fix late roundoff error due to float precision
            var postDecFix = Floor(postDec) / Pow(10, formattingOptions.FractionalPrecision);
            preDec += postDecFix;
            postDec -= Floor(postDecFix) * Pow(10, formattingOptions.FractionalPrecision);
        }

        var preDecStr = preDec.ToString(CultureInfo.InvariantCulture);

        var isPostDecEmpty = postDec == 0;

        string postDecStr;
        if (!isPostDecEmpty)
        {
            var postDecStrBuilder = new StringBuilder(postDec.ToString(CultureInfo.InvariantCulture));
            while (postDecStrBuilder.Length < formattingOptions.FractionalPrecision)
                postDecStrBuilder.Insert(0, "0");

            postDecStr = postDecStrBuilder.ToString().TrimEnd('0');
        }
        else
            postDecStr = String.Empty;

        var resultStrBuilder = new StringBuilder(postDecStr, Max(formattingOptions.WhitePostpad, formattingOptions.ZeroPostpad));
        var padLength = formattingOptions.ZeroPostpad - resultStrBuilder.Length;
        if (padLength > 0)
        {
            resultStrBuilder.Append(new string('0', padLength));
            isPostDecEmpty = false;
        }

        padLength = formattingOptions.WhitePostpad - resultStrBuilder.Length;
        if (padLength > 0)
            resultStrBuilder.Append(new string(' ', padLength));

        postDecStr = resultStrBuilder.ToString();

        resultStrBuilder = new StringBuilder(preDecStr, Max(formattingOptions.WhitePrepad, formattingOptions.ZeroPrepad));
        padLength = formattingOptions.ZeroPrepad - resultStrBuilder.Length;
        if (padLength > 0)
            resultStrBuilder.Insert(0, new string('0', padLength));

        padLength = formattingOptions.WhitePrepad - resultStrBuilder.Length;
        if (padLength > 0)
            resultStrBuilder.Insert(0, new string(' ', padLength));

        if (sign < 0)
            resultStrBuilder.Insert(0, '-');
        else if (formattingOptions.ForceSign)
            resultStrBuilder.Insert(0, '+');

        string decSepStr;
        var unitPlaced = false;
        if (formattingOptions.PrefixOrUnitAsDecimalSeparator)
        {
            try
            {
                decSepStr = SIPrefixes.GetSIPrefix(exp3 * 3, unitOptions, formattingOptions, out unitPlaced);
            }
            catch (ArgumentOutOfRangeException)
            {
                decSepStr = $"(e{3 * exp3})";
            }
        }
        else
            decSepStr = formattingOptions.DecimalSeparator;

        if (!isPostDecEmpty || formattingOptions.ForceDecimalSeparator || formattingOptions.PrefixOrUnitAsDecimalSeparator)
        {
            resultStrBuilder.Append(decSepStr);
            resultStrBuilder.Append(postDecStr);
        }

        var siBeforeUnit = false;

        if (!formattingOptions.PrefixOrUnitAsDecimalSeparator)
        {
            resultStrBuilder.Append(formattingOptions.SISeparator);
            try
            {
                resultStrBuilder.Append(SIPrefixes.GetSIPrefix(exp3 * 3, unitOptions, formattingOptions, out _));
                siBeforeUnit = exp3 != 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                resultStrBuilder.Append($"e{3 * exp3}");
                siBeforeUnit = true;
            }
        }

        if (!unitPlaced)
        {
            resultStrBuilder.Append(formattingOptions.UnitSeparator);
            if (!formattingOptions.PrefixOrUnitAsDecimalSeparator && formattingOptions.AdaptCompositeUnitCase && unitOptions.Unit.Symbol.Length > 1)
            {
                resultStrBuilder.Append(siBeforeUnit ? unitOptions.Unit.Symbol.ToLower()[0] : unitOptions.Unit.Symbol.ToUpper()[0]);
                resultStrBuilder.Append(unitOptions.Unit.Symbol.Substring(1, unitOptions.Unit.Symbol.Length - 1));
            }
            else
                resultStrBuilder.Append(unitOptions.Unit);
        }

        return resultStrBuilder.ToString();
    }

    #endregion
}