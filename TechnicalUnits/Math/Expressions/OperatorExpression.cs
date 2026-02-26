using System;

namespace TechnicalUnits.Math.Expressions;

/// <summary>
/// Expression that applies a binary arithmetic operator (+, -, *, /, ^) to two operands.
/// </summary>
public sealed class OperatorExpression : ExpressionBase
{
    #region MathOperators enum

    /// <summary>Supported binary math operators.</summary>
    public enum MathOperators
    {
        /// <summary>Addition (+).</summary>
        Add,

        /// <summary>Subtraction (-).</summary>
        Subtract,

        /// <summary>Multiplication (*).</summary>
        Multiply,

        /// <summary>Division (/).</summary>
        Divide,

        /// <summary>Exponentiation (^).</summary>
        Power
    }

    #endregion

    /// <summary>The set of recognised operator characters.</summary>
    public static readonly char[] OperatorSymbols = ['+', '-', '*', '/', '^'];

    /// <summary>
    /// Initializes a new <see cref="OperatorExpression" /> from an operator string.
    /// </summary>
    /// <param name="operation">A single-character operator string (+, -, *, /, ^).</param>
    /// <exception cref="ArgumentNullException"><paramref name="operation" /> is <c>null</c> or empty.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation" /> is not a recognised operator.</exception>
    public OperatorExpression(string operation)
    {
        if (String.IsNullOrEmpty(operation))
            throw new ArgumentNullException(nameof(operation));

        MathOperator = operation switch
        {
            "+" => MathOperators.Add,
            "-" => MathOperators.Subtract,
            "*" => MathOperators.Multiply,
            "/" => MathOperators.Divide,
            "^" => MathOperators.Power,
            _ => throw new ArgumentException("Invalid operator: " + operation, nameof(operation))
        };
    }

    /// <inheritdoc />
    public override int ArgumentCount => 2;

    /// <summary>Gets the parsed operator kind.</summary>
    public MathOperators MathOperator { get; }

    /// <inheritdoc />
    public override double Evaluate(double[] values)
    {
        ValidateArguments(values);

        return MathOperator switch
        {
            MathOperators.Add => values[0] + values[1],
            MathOperators.Subtract => values[0] - values[1],
            MathOperators.Multiply => values[0] * values[1],
            MathOperators.Divide => values[0] / values[1],
            MathOperators.Power => System.Math.Pow(values[0], values[1]),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <inheritdoc />
    public override string ToString() => MathOperator.ToString();

    #region Internal

    /// <summary>Determines whether <paramref name="str" /> is a single-character operator.</summary>
    public static bool IsOperator(string str) => str.Length == 1 && IsOperator(str[0]);

    /// <summary>Determines whether <paramref name="ch" /> is a recognised operator character.</summary>
    public static bool IsOperator(char ch) => Array.IndexOf(OperatorSymbols, ch) >= 0;

    #endregion
}
