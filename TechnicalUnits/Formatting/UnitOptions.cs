using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TechnicalUnits.Units;

namespace TechnicalUnits.Formatting;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnitOptions
{
    private Unit _unit;

    public UnitOptions()
    {
        _unit = SIUnits.Dimensionless;
        AlternateUnits = new List<DerivedUnit>();
    }

    public UnitOptions(Unit unit)
    {
        _unit = unit;
        AlternateUnits = new List<DerivedUnit>();
    }

    public UnitOptions(Unit unit, IEnumerable<DerivedUnit> alternateUnits)
    {
        _unit = unit;
        AlternateUnits = new List<DerivedUnit>(alternateUnits);
    }

    [Category("Units")]
    [Description("(Base) Unit")]
    public Unit Unit
    {
        get { return _unit; }
        set { _unit = value ?? throw new ArgumentNullException(nameof(value)); }
    }

    [Category("Units")]
    [Description("Collection of alternate units (must derive from base unit)")]
    public List<DerivedUnit> AlternateUnits { get; }

    /// <summary>Sort unit symbols starting with the longest, so that short units do not "eat" the longer ones.</summary>
    internal DerivedUnit[] GetSortedUnits()
    {
        var sortedUnits = new[] { new DerivedUnit(Unit.Name, Unit.Symbol, 1.0, Unit) }.Concat(AlternateUnits).ToArray();
        Array.Sort(sortedUnits, (a, b) => b.Symbol.Length.CompareTo(a.Symbol.Length));

        return sortedUnits;
    }

    public static implicit operator UnitOptions(Unit unit) => new UnitOptions(unit);
}
