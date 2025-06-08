using System;

namespace TechnicalUnits.Units;

public class DerivedUnit : Unit
{
    private readonly Func<double, double>? _fromBaseUnitFunc;
    private readonly Func<double, double>? _toBaseUnitFunc;

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

    public Unit BaseUnit { get; }

    public double BaseConversionFactor { get; } = 1.0;

    #region ConversionFunction

    public double FromBase(double value)
    {
        if (_fromBaseUnitFunc != null)
            return _fromBaseUnitFunc(value);

        return value * BaseConversionFactor;
    }

    public double ToBase(double value)
    {
        if (_toBaseUnitFunc != null)
            return _toBaseUnitFunc(value);

        return value / BaseConversionFactor;
    }

    #endregion
}
