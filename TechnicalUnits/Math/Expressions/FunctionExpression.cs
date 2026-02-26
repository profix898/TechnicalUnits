using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static System.Math;

namespace TechnicalUnits.Math.Expressions;

/// <summary>
/// Expression that invokes a built-in mathematical function (sin, cos, sqrt, max, pow, …).
/// </summary>
public sealed class FunctionExpression : ExpressionBase
{
    private static readonly string[] OneArgFunctionNames =
    [
        "abs", "acos", "asin", "atan", "ceiling", "cos", "cosh", "exp", "floor", "log",
        "log10", "sin", "sinh", "sqrt", "tan", "tanh"
    ];

    private static readonly HashSet<string> OneArgFunctions = new HashSet<string>(OneArgFunctionNames, StringComparer.OrdinalIgnoreCase);

    private static readonly string[] TwoArgFunctionNames = ["max", "min", "pow"];

    private static readonly HashSet<string> TwoArgFunctions = new HashSet<string>(TwoArgFunctionNames, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new <see cref="FunctionExpression" /> for the specified function name.
    /// </summary>
    /// <param name="function">The function name (case-insensitive, e.g. <c>"sin"</c>, <c>"max"</c>).</param>
    /// <param name="validate">When <c>true</c>, throws if <paramref name="function" /> is not a recognised built-in.</param>
    /// <exception cref="ArgumentException"><paramref name="validate" /> is <c>true</c> and the function name is invalid.</exception>
    public FunctionExpression(string function, bool validate = true)
    {
        function = function.ToLowerInvariant();

        if (validate && !IsFunction(function))
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid function name '{0}'.", function), nameof(function));

        Function = function;
    }

    /// <inheritdoc />
    public override int ArgumentCount => IsOneArgFunction(Function) ? 1 : 2;

    /// <summary>Gets the lower-case function name.</summary>
    public string Function { get; }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override string ToString() => Function;

    #region Internal

    /// <summary>Returns the names of all built-in functions.</summary>
    public static string[] GetFunctionNames() => OneArgFunctionNames.Concat(TwoArgFunctionNames).ToArray();

    /// <summary>Determines whether <paramref name="function" /> is a recognised built-in function.</summary>
    public static bool IsFunction(string function) => IsOneArgFunction(function) || IsTwoArgFunction(function);

    /// <summary>Determines whether <paramref name="function" /> is a one-argument function.</summary>
    public static bool IsOneArgFunction(string function) => OneArgFunctions.Contains(function);

    /// <summary>Determines whether <paramref name="function" /> is a two-argument function.</summary>
    public static bool IsTwoArgFunction(string function) => TwoArgFunctions.Contains(function);

    #endregion
}
