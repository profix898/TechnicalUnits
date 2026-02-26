using System;
using System.Globalization;
using System.Text;
using TechnicalUnits.Internal;
using static System.Math;
using static TechnicalUnits.Internal.MathHelper;

namespace TechnicalUnits.Formatting;

/// <summary>
/// Formats numeric values with SI prefixes, unit symbols, and configurable padding/separators.
/// </summary>
/// <remarks>
/// The formatter outputs values using multiples-of-three exponents (k, M, G, …, m, µ, n, …).
/// Non-standard SI prefixes such as centi or hecto are <b>not</b> emitted.
/// The <see cref="Parser" /> can, however, <em>accept</em> any SI prefix on input.
/// </remarks>
public static class Formatter
{
    #region Format

    /// <summary>
    /// Formats <paramref name="value" /> with the specified unit and formatting options.
    /// </summary>
    /// <param name="value">The numeric value to format.</param>
    /// <param name="unitOptions">Unit and alternate-unit configuration.</param>
    /// <param name="formattingOptions">Display/formatting options (default: <see cref="FormattingOptions.Default" />).</param>
    /// <returns>A formatted string containing the value, SI prefix, and unit symbol.</returns>
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
                NumberDecimalSeparator = formattingOptions.NumberFormat.NumberDecimalSeparator, NumberGroupSeparator = formattingOptions.NumberFormat.NumberGroupSeparator
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
            throw new InvalidOperationException("Internal Error: More than three orders of magnitude before the decimal separator.");

        var preDec = Truncate(value);
        var postDec = value - Truncate(value);
        var fractionalScale = Pow(10, formattingOptions.FractionalPrecision);
        postDec = Round(postDec * fractionalScale);

        if (postDec >= fractionalScale)
        {
            // Fix late roundoff error due to float precision
            var postDecFix = Floor(postDec) / fractionalScale;
            preDec += postDecFix;
            postDec -= Floor(postDecFix) * fractionalScale;
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

        var resultStrBuilder = new StringBuilder(postDecStr, postDecStr.Length + Max(formattingOptions.WhitePostpad, formattingOptions.ZeroPostpad));
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
            decSepStr = formattingOptions.NumberFormat.NumberDecimalSeparator;

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
                resultStrBuilder.Append(siBeforeUnit ? Char.ToLowerInvariant(unitOptions.Unit.Symbol[0]) : Char.ToUpperInvariant(unitOptions.Unit.Symbol[0]));
                resultStrBuilder.Append(unitOptions.Unit.Symbol.Substring(1));
            }
            else
                resultStrBuilder.Append(unitOptions.Unit);
        }

        return resultStrBuilder.ToString();
    }

    #endregion
}
