using System;
using System.Globalization;
using System.Linq;
using static System.Math;

namespace TechnicalUnits.Math.Expressions;

public sealed class FunctionExpression : ExpressionBase
{
    public static readonly string[] oneArgFunctions = { "abs", "acos", "asin", "atan", "ceiling", "cos", "cosh", "exp", "floor", "log", "log10", "sin", "sinh", "sqrt", "tan", "tanh" };
    
    public static readonly string[] twoArgFunction = { "max", "min", "pow" };

    public FunctionExpression(string function, bool validate = true)
    {
        function = function.ToLowerInvariant();

        if (validate && !IsFunction(function))
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid function name '{0}'.", function), nameof(function));

        Function = function;
    }

    public string Function { get; }

    public override int ArgumentCount => IsOneArgFunction(Function) ? 1 : 2;

    public override double Evaluate(double[] values)
    {
        ValidateArguments(values);

        if (IsOneArgFunction(Function))
        {
            return Function switch
            {
                "abs" => Abs(values[0]),
                "acos" => Acos(values[0]),
                "asin" => Asin(values[0]),
                "atan" => Atan(values[0]),
                "ceiling" => Ceiling(values[0]),
                "cos" => Cos(values[0]),
                "cosh" => Cosh(values[0]),
                "exp" => Exp(values[0]),
                "floor" => Floor(values[0]),
                "log" => Log(values[0]),
                "log10" => Log10(values[0]),
                "sin" => Sin(values[0]),
                "sinh" => Sinh(values[0]),
                "sqrt" => Sqrt(values[0]),
                "tan" => Tan(values[0]),
                "tanh" => Tanh(values[0]),
                _ => throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid function name '{0}'.", Function), nameof(Function))
            };
        }

        if (IsTwoArgFunction(Function))
        {
            return Function switch
            {
                "max" => Max(values[0], values[1]),
                "min" => Min(values[0], values[1]),
                "pow" => Pow(values[0], values[1]),
                _ => throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid function name '{0}'.", Function), nameof(Function))
            };
        }

        throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid function name '{0}'.", Function), nameof(Function));
    }

    #region Internal

    public static string[] GetFunctionNames()
    {
        return oneArgFunctions.Concat(twoArgFunction).ToArray();
    }

    public static bool IsFunction(string function)
    {
        return IsOneArgFunction(function) || IsTwoArgFunction(function);
    }

    public static bool IsOneArgFunction(string function)
    {
        return oneArgFunctions.Contains(function, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsTwoArgFunction(string function)
    {
        return twoArgFunction.Contains(function, StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    public override string ToString()
    {
        return Function;
    }
}
