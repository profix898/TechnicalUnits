using System;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// Represents a physical unit defined by a name, symbol, and <see cref="Dimension" />.
/// Supports dimensional arithmetic via operator overloads so that compound dimensions
/// can be expressed naturally (e.g. <c>Kilogram * Meter / (Second * Second)</c>).
/// </summary>
public class Unit
{
    /// <summary>
    /// Initializes a new <see cref="Unit" /> whose symbol is derived from the dimension's string representation.
    /// </summary>
    /// <param name="name">A unique identifier for the unit (e.g. "length").</param>
    /// <param name="dimension">The physical dimension of the unit.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> or <paramref name="dimension" /> is <c>null</c>.</exception>
    public Unit(string name, Dimension dimension)
        : this(name, dimension.ToString(), dimension)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Unit" /> with an explicit symbol.
    /// </summary>
    /// <param name="name">A unique identifier for the unit (e.g. "force").</param>
    /// <param name="symbol">The display symbol (e.g. "N").</param>
    /// <param name="dimensions">The physical dimension of the unit.</param>
    /// <exception cref="ArgumentNullException">Any parameter is <c>null</c>.</exception>
    public Unit(string name, string symbol, Dimension dimensions)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));

        Name = name;
        Symbol = symbol;
        Dimensions = dimensions;
    }

    /// <summary>Gets the <see cref="Dimension" /> that describes this unit's physical dimension.</summary>
    public Dimension Dimensions { get; }

    /// <summary>Gets the unique name of this unit.</summary>
    public string Name { get; }

    /// <summary>Gets the display symbol (e.g. "m", "kg", "N").</summary>
    public string Symbol { get; }

    #region Overrides of Object

    public override string ToString() => Symbol;

    #endregion

    #region Operators

    /// <summary>Multiplies two units, returning the combined <see cref="Dimension" />.</summary>
    public static Dimension operator *(Unit a, Unit b) => a.Dimensions * b.Dimensions;

    /// <summary>Multiplies a unit by a dimension.</summary>
    public static Dimension operator *(Unit a, Dimension b) => a.Dimensions * b;

    /// <summary>Multiplies a dimension by a unit.</summary>
    public static Dimension operator *(Dimension a, Unit b) => a * b.Dimensions;

    /// <summary>Divides two units, returning the resulting <see cref="Dimension" />.</summary>
    public static Dimension operator /(Unit a, Unit b) => a.Dimensions / b.Dimensions;

    /// <summary>Divides a unit by a dimension.</summary>
    public static Dimension operator /(Unit a, Dimension b) => a.Dimensions / b;

    /// <summary>Divides a dimension by a unit.</summary>
    public static Dimension operator /(Dimension a, Unit b) => a / b.Dimensions;

    /// <summary>Raises a unit's dimension to the given integer power.</summary>
    public static Dimension operator ^(Unit a, int exp) => a.Dimensions ^ exp;

    /// <summary>Implicitly converts a <see cref="Unit" /> to its <see cref="Dimension" />.</summary>
    public static implicit operator Dimension(Unit unit) => unit.Dimensions;

    /// <summary>Implicitly converts a <see cref="Unit" /> to its string symbol.</summary>
    public static implicit operator string(Unit unit) => unit.ToString();

    #endregion
}
