using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TechnicalUnits.Units;

namespace TechnicalUnits.Formatting;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnitOptions
{
    private Unit unit = null!;

    public UnitOptions()
    {
        Unit = SIUnits.Dimensionless;
    }

    public UnitOptions(Unit unit)
    {
        Unit = unit;
    }

    public UnitOptions(Unit unit, IEnumerable<DerivedUnit> alternateUnits)
    {
        Unit = unit;
        AlternateUnits = new List<DerivedUnit>(alternateUnits);
    }

    [Category("Units")]
    [Description("Unit for the given value")]
    public Unit Unit
    {
        get { return unit; }
        set { unit = value ?? throw new ArgumentNullException(nameof(value)); }
    }

    [Category("Units")]
    [Description("Collection of alternate units for the given value (must derive from base unit)")]
    public List<DerivedUnit> AlternateUnits { get; } = new List<DerivedUnit>();

    /// <summary>Sort unit symbols starting with the longest, so that short units do not "eat" the longer ones.</summary>
    internal DerivedUnit[] GetSortedUnits()
    {
        var sortedUnits = new[] { new DerivedUnit(Unit.Name, Unit.Symbol, 1.0, Unit) }.Concat(AlternateUnits).ToArray();
        Array.Sort(sortedUnits, (a, b) => b.Symbol.Length.CompareTo(a.Symbol.Length));

        return sortedUnits;
    }

    public static implicit operator UnitOptions(Unit unit) => new UnitOptions(unit);
}
