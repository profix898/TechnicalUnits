using System;

namespace TechnicalUnits.Units;

public class Unit
{
    public Unit(string name, Dimension dimension)
        : this(name, dimension.ToString(), dimension)
    {
        if (String.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (dimension == null)
            throw new ArgumentNullException(nameof(dimension));
    }

    public Unit(string name, string symbol, Dimension dimensions)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));
        if (dimensions == null)
            throw new ArgumentNullException(nameof(dimensions));

        Name = name;
        Symbol = symbol;
        Dimensions = dimensions;
    }

    public string Name { get; }

    public string Symbol { get; }

    public Dimension Dimensions { get; }

    #region Operators

    public static Dimension operator *(Unit a, Unit b)
    {
        return a.Dimensions * b.Dimensions;
    }

    public static Dimension operator *(Unit a, Dimension b)
    {
        return a.Dimensions * b;
    }

    public static Dimension operator *(Dimension a, Unit b)
    {
        return a * b.Dimensions;
    }

    public static Dimension operator /(Unit a, Unit b)
    {
        return a.Dimensions / b.Dimensions;
    }

    public static Dimension operator /(Unit a, Dimension b)
    {
        return a.Dimensions / b;
    }

    public static Dimension operator /(Dimension a, Unit b)
    {
        return a / b.Dimensions;
    }

    public static Dimension operator ^(Unit a, int exp)
    {
        return a.Dimensions ^ exp;
    }

    public static implicit operator Dimension(Unit unit) => unit.Dimensions;

    public static implicit operator string(Unit unit) => unit.ToString();

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
        return Symbol;
    }

    #endregion
}