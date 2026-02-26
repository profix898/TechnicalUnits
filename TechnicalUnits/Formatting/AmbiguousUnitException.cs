using System;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Formatting;

/// <summary>
/// The exception that is raised when a unit symbol is ambiguous with an SI prefix.
/// </summary>
public class AmbiguousUnitException : Exception
{
    /// <summary>
    /// Initializes a new instance for an ambiguous unit.
    /// </summary>
    public AmbiguousUnitException(string message, Unit unit)
        : base(message)
    {
        Unit = unit;
    }

    /// <summary>
    /// Initializes a new instance for a unit that is ambiguous with a specific SI prefix.
    /// </summary>
    public AmbiguousUnitException(string message, Unit unit, string ambiguousPrefix)
        : base(message)
    {
        Unit = unit;
        AmbiguousPrefix = ambiguousPrefix;
    }

    /// <summary>Gets the SI prefix string that the unit symbol was confused with, or <c>null</c>.</summary>
    public string? AmbiguousPrefix { get; }

    /// <summary>Gets the unit that caused the ambiguity.</summary>
    public Unit Unit { get; }
}
