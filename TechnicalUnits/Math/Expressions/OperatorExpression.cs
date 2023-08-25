using System;
using System.Linq;

namespace TechnicalUnits.Math.Expressions;

public sealed class OperatorExpression : ExpressionBase
{
    #region MathOperators enum

    public enum MathOperators
    {
        Add,
        Subtract,
        Multiple,
        Divide,
        Power
    }

    #endregion

    public static readonly char[] OperatorSymbols = { '+', '-', '*', '/', '^' };

    public OperatorExpression(string operation)
    {
        if (String.IsNullOrEmpty(operation))
            throw new ArgumentNullException(nameof(operation));

        MathOperator = operation switch
        {
            "+" => MathOperators.Add,
            "-" => MathOperators.Subtract,
            "*" => MathOperators.Multiple,
            "/" => MathOperators.Divide,
            "^" => MathOperators.Power,
            _ => throw new ArgumentException("Invalid operator: " + operation, nameof(operation))
        };
    }

    public MathOperators MathOperator { get; }

    public override int ArgumentCount => 2;

    public override double Evaluate(double[] values)
    {
        ValidateArguments(values);

        return MathOperator switch
        {
            MathOperators.Add => values[0] + values[1],
            MathOperators.Subtract => values[0] - values[1],
            MathOperators.Multiple => values[0] * values[1],
            MathOperators.Divide => values[0] / values[1],
            MathOperators.Power => System.Math.Pow(values[0], values[1]),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #region Internal

    public static bool IsOperator(string str)
    {
        return str.Length == 1 && IsOperator(str[0]);
    }

    public static bool IsOperator(char ch)
    {
        return OperatorSymbols.Contains(ch);
    }

    #endregion

    public override string ToString()
    {
        return MathOperator.ToString();
    }
}
