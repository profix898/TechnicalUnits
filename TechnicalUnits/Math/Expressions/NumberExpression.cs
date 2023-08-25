using System;
using System.Globalization;
using static TechnicalUnits.Formatting.FormattingOptions;

namespace TechnicalUnits.Math.Expressions;

public sealed class NumberExpression : ExpressionBase
{
    public NumberExpression(double value)
    {
        Value = value;
    }

    public double Value { get; }

    public override int ArgumentCount => 0;

    public override double Evaluate(double[] values)
    {
        return Value;
    }

    #region Internal

    public static bool IsNumber(char ch)
    {
        return Char.IsDigit(ch) || Default.DecimalSeparator.IndexOf(ch) >= 0;
    }

    public static bool IsPositiveSign(char ch)
    {
        return Default.PositiveSign.IndexOf(ch) >= 0;
    }

    public static bool IsNegativeSign(char ch)
    {
        return Default.NegativeSign.IndexOf(ch) >= 0;
    }

    #endregion

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
