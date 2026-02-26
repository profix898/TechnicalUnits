using System;
using System.Globalization;
using static TechnicalUnits.Formatting.FormattingOptions;

namespace TechnicalUnits.Math.Expressions;

/// <summary>
/// Expression that represents a literal numeric value (leaf node in the expression tree).
/// </summary>
public sealed class NumberExpression : ExpressionBase
{
    /// <summary>
    /// Initializes a new <see cref="NumberExpression" /> with the specified value.
    /// </summary>
    /// <param name="value">The numeric value this expression represents.</param>
    public NumberExpression(double value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override int ArgumentCount => 0;

    /// <summary>Gets the numeric value.</summary>
    public double Value { get; }

    /// <inheritdoc />
    public override double Evaluate(double[] values) => Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    #region Internal

    /// <summary>Determines whether <paramref name="ch" /> is a digit or a decimal separator.</summary>
    public static bool IsNumber(char ch) => Char.IsDigit(ch) || Default.NumberFormat.NumberDecimalSeparator.IndexOf(ch) >= 0;

    /// <summary>Determines whether <paramref name="ch" /> is a positive sign character.</summary>
    public static bool IsPositiveSign(char ch) => Default.NumberFormat.PositiveSign.IndexOf(ch) >= 0;

    /// <summary>Determines whether <paramref name="ch" /> is a negative sign character.</summary>
    public static bool IsNegativeSign(char ch) => Default.NumberFormat.NegativeSign.IndexOf(ch) >= 0;

    #endregion
}
