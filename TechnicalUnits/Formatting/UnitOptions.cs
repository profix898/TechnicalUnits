using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TechnicalUnits.UnitDefinition;

namespace TechnicalUnits.Formatting;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnitOptions
{
    private int _cacheAlternateCount = -1;
    private DerivedUnit[]? _sortedUnitsCache;
    private Unit _unit;

    public UnitOptions()
    {
        _unit = SIUnits.Dimensionless;
        AlternateUnits = [];
    }

    public UnitOptions(Unit unit)
    {
        _unit = unit;
        AlternateUnits = [];
    }

    public UnitOptions(Unit unit, IEnumerable<DerivedUnit> alternateUnits)
    {
        _unit = unit;
        AlternateUnits = new List<DerivedUnit>(alternateUnits);
    }

    [Category("Units")]
    [Description("Collection of alternate units (must derive from base unit)")]
    public List<DerivedUnit> AlternateUnits { get; }

    [Category("Units")]
    [Description("(Base) Unit")]
    public Unit Unit
    {
        get => _unit;
        set
        {
            _unit = value ?? throw new ArgumentNullException(nameof(value));
            _sortedUnitsCache = null;
        }
    }

    /// <summary>Sort unit symbols starting with the longest, so that short units do not "eat" the longer ones.</summary>
    /// <remarks>Results are cached and reused until <see cref="Unit" /> changes or the <see cref="AlternateUnits" /> count changes.</remarks>
    internal DerivedUnit[] GetSortedUnits()
    {
        if (_sortedUnitsCache != null && _cacheAlternateCount == AlternateUnits.Count)
            return _sortedUnitsCache;

        var sortedUnits = new[] { new DerivedUnit(Unit.Name, Unit.Symbol, 1.0, Unit) }.Concat(AlternateUnits).ToArray();
        Array.Sort(sortedUnits, (a, b) => b.Symbol.Length.CompareTo(a.Symbol.Length));

        _sortedUnitsCache = sortedUnits;
        _cacheAlternateCount = AlternateUnits.Count;

        return sortedUnits;
    }

    public static implicit operator UnitOptions(Unit unit) => new UnitOptions(unit);
}
