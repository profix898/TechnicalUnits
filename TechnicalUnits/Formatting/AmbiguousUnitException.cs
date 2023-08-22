using System;
using TechnicalUnits.Units;

namespace TechnicalUnits.Formatting;

public class AmbiguousUnitException : Exception
{
    public AmbiguousUnitException(string message, Unit unit)
        : base(message)
    {
        Unit = unit;
    }

    public AmbiguousUnitException(string message, Unit unit, string ambiguousPrefix)
        : base(message)
    {
        Unit = unit;
        AmbiguousPrefix = ambiguousPrefix;
    }

    public Unit Unit { get; }

    public string? AmbiguousPrefix { get; }
}
