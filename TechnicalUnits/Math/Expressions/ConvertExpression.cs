using System;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Math.Expressions;

/// <summary>
/// Expression that converts a value from one unit to another.
/// Uses the bracket notation <c>[sourceUnit&gt;targetUnit]</c>.
/// </summary>
/// <remarks>
/// Units are resolved via <see cref="UnitsLocator.FindBySymbol" /> (case-sensitive)
/// first, then <see cref="UnitsLocator.FindByName" /> (case-insensitive) as fallback.
/// Source and target must share the same <see cref="Unit.Dimensions" />.
/// </remarks>
public sealed class ConvertExpression : ExpressionBase
{
    private readonly Func<double, double> _convertFunc;

    /// <summary>
    /// Initializes a new <see cref="ConvertExpression" /> from a bracket expression string.
    /// </summary>
    /// <param name="expression">The conversion expression (e.g. <c>"[°C&gt;K]"</c>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="expression" /> is <c>null</c> or empty.</exception>
    /// <exception cref="ArgumentException">The expression cannot be parsed or the units are incompatible.</exception>
    public ConvertExpression(string expression)
    {
        if (String.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        Expression = expression;

        var (source, target) = ParseConvertExpression(expression);
        _convertFunc = BuildConvertFunc(source, target);
    }

    /// <inheritdoc />
    public override int ArgumentCount => 1;

    /// <summary>Gets the raw conversion expression string.</summary>
    public string Expression { get; }

    /// <inheritdoc />
    public override double Evaluate(double[] values)
    {
        ValidateArguments(values);

        return _convertFunc(values[0]);
    }

    /// <inheritdoc />
    public override string ToString() => Expression;

    #region Internal

    /// <summary>
    /// Parses a conversion expression string into source and target <see cref="Unit" /> instances.
    /// </summary>
    /// <param name="expression">The full bracket expression (e.g. <c>"[mm&gt;in]"</c>).</param>
    /// <returns>A tuple of (source <see cref="Unit" />, target <see cref="Unit" />).</returns>
    /// <exception cref="ArgumentException">
    /// The expression format is invalid, a unit cannot be found, or the units have incompatible dimensions.
    /// </exception>
    public static (Unit Source, Unit Target) ParseConvertExpression(string expression)
    {
        if (String.IsNullOrEmpty(expression) || expression.Length < 4)
            throw new ArgumentException($"Invalid conversion expression '{expression}'.", nameof(expression));

        // Strip brackets: [source>target] -> source, target
        var inner = expression.Substring(1, expression.Length - 2);
        var arrowIdx = inner.IndexOf('>');
        if (arrowIdx < 0)
            throw new ArgumentException($"Invalid conversion expression '{expression}': missing '>' separator.", nameof(expression));

        var sourceSymbol = inner.Substring(0, arrowIdx).Trim();
        var targetSymbol = inner.Substring(arrowIdx + 1).Trim();

        if (sourceSymbol.Length == 0)
            throw new ArgumentException($"Invalid conversion expression '{expression}': empty source unit.", nameof(expression));
        if (targetSymbol.Length == 0)
            throw new ArgumentException($"Invalid conversion expression '{expression}': empty target unit.", nameof(expression));

        var sourceUnit = UnitsLocator.FindBySymbol(sourceSymbol) ?? UnitsLocator.FindByName(sourceSymbol);
        var targetUnit = UnitsLocator.FindBySymbol(targetSymbol) ?? UnitsLocator.FindByName(targetSymbol);

        if (sourceUnit == null)
            throw new ArgumentException($"Unknown source unit '{sourceSymbol}' in conversion expression '{expression}'.", nameof(expression));
        if (targetUnit == null)
            throw new ArgumentException($"Unknown target unit '{targetSymbol}' in conversion expression '{expression}'.", nameof(expression));

        if (sourceUnit.Dimensions != targetUnit.Dimensions)
        {
            throw new ArgumentException($"Incompatible dimensions: cannot convert from '{sourceSymbol}' ({sourceUnit.Dimensions}) to '{targetSymbol}' ({targetUnit.Dimensions}).",
                                        nameof(expression));
        }

        return (sourceUnit, targetUnit);
    }

    /// <summary>
    /// Determines whether <paramref name="expression" /> has valid conversion expression syntax
    /// (<c>[…&gt;…]</c>).
    /// </summary>
    /// <param name="expression">The string to test.</param>
    /// <returns><c>true</c> if the string matches the bracket-arrow-bracket pattern.</returns>
    public static bool IsConvertExpression(string expression)
    {
        if (String.IsNullOrEmpty(expression))
            return false;

        if (!expression.StartsWith('['))
            return false;

        if (!expression.EndsWith(']'))
            return false;

        if (!expression.Contains('>'))
            return false;

        return true;
    }

    private static Func<double, double> BuildConvertFunc(Unit source, Unit target)
    {
        // Identity conversion
        if (ReferenceEquals(source, target))
            return static v => v;

        var sourceDerived = source as DerivedUnit;
        var targetDerived = target as DerivedUnit;

        // Both are base SI units with same dimensions → identity
        if (sourceDerived == null && targetDerived == null)
            return static v => v;

        // Source is derived, target is its base unit
        if (sourceDerived != null && targetDerived == null)
            return sourceDerived.ToBase;

        // Source is base unit, target is derived
        if (sourceDerived == null && targetDerived != null)
            return targetDerived.FromBase;

        // Both derived → convert through shared base: source → base → target
        return v => targetDerived!.FromBase(sourceDerived!.ToBase(v));
    }

    #endregion
}
