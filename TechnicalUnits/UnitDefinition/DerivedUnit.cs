using System;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// A unit derived from a <see cref="BaseUnit" /> by a linear conversion factor or
/// by arbitrary from-base / to-base conversion functions (e.g. temperature offsets).
/// </summary>
public class DerivedUnit : Unit
{
    private readonly Func<double, double>? _fromBaseUnitFunc;
    private readonly Func<double, double>? _toBaseUnitFunc;

    /// <summary>
    /// Initializes a <see cref="DerivedUnit" /> that converts to/from its base unit by a
    /// simple multiplicative factor (<c>baseValue = derivedValue / factor</c>).
    /// </summary>
    /// <param name="name">A unique identifier for the unit.</param>
    /// <param name="symbol">The display symbol.</param>
    /// <param name="baseConvFactor">The factor such that <c>derivedValue = baseValue * factor</c>.</param>
    /// <param name="baseUnit">The base <see cref="Unit" /> this unit derives from.</param>
    /// <exception cref="ArgumentNullException">Any reference parameter is <c>null</c>.</exception>
    public DerivedUnit(string name, string symbol, double baseConvFactor, Unit baseUnit)
        : base(name, symbol, baseUnit.Dimensions)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));
        if (baseUnit == null)
            throw new ArgumentNullException(nameof(baseUnit));

        BaseConversionFactor = baseConvFactor;
        BaseUnit = baseUnit;
    }

    /// <summary>
    /// Initializes a <see cref="DerivedUnit" /> that uses arbitrary functions for conversion
    /// (required for non-linear conversions such as Celsius ↔ Kelvin).
    /// </summary>
    /// <param name="name">A unique identifier for the unit.</param>
    /// <param name="symbol">The display symbol.</param>
    /// <param name="fromBaseUnitFunc">Converts a value <em>from</em> the base unit to this unit.</param>
    /// <param name="toBaseUnitFunc">Converts a value <em>from</em> this unit <em>to</em> the base unit.</param>
    /// <param name="baseUnit">The base <see cref="Unit" /> this unit derives from.</param>
    /// <exception cref="ArgumentNullException">Any parameter is <c>null</c>.</exception>
    public DerivedUnit(string name, string symbol, Func<double, double> fromBaseUnitFunc, Func<double, double> toBaseUnitFunc, Unit baseUnit)
        : base(name, symbol, baseUnit.Dimensions)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));
        if (fromBaseUnitFunc == null)
            throw new ArgumentNullException(nameof(fromBaseUnitFunc));
        if (toBaseUnitFunc == null)
            throw new ArgumentNullException(nameof(toBaseUnitFunc));
        if (baseUnit == null)
            throw new ArgumentNullException(nameof(baseUnit));

        _fromBaseUnitFunc = fromBaseUnitFunc;
        _toBaseUnitFunc = toBaseUnitFunc;
        BaseUnit = baseUnit;
    }

    /// <summary>Gets the linear conversion factor (1.0 when custom functions are used).</summary>
    public double BaseConversionFactor { get; } = 1.0;

    /// <summary>Gets the base <see cref="Unit" /> from which this unit is derived.</summary>
    public Unit BaseUnit { get; }

    #region ConversionFunction

    /// <summary>
    /// Converts a value expressed in the <see cref="BaseUnit" /> to this derived unit.
    /// </summary>
    public double FromBase(double value)
    {
        if (_fromBaseUnitFunc != null)
            return _fromBaseUnitFunc(value);

        return value * BaseConversionFactor;
    }

    /// <summary>
    /// Converts a value expressed in this derived unit to the <see cref="BaseUnit" />.
    /// </summary>
    public double ToBase(double value)
    {
        if (_toBaseUnitFunc != null)
            return _toBaseUnitFunc(value);

        return value / BaseConversionFactor;
    }

    #endregion
}
