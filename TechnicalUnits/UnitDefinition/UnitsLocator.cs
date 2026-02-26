using System;
using System.Collections.Generic;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// Central registry of all known <see cref="Unit" /> instances (SI base, SI derived,
/// and common non-SI units). Consumers can use <see cref="AllUnits" /> to look up or
/// enumerate the full catalogue, or <see cref="FindBySymbol" /> / <see cref="FindByName" />
/// for O(1) lookup.
/// </summary>
public static class UnitsLocator
{
    private static readonly List<Unit> _unitsList = [];
    private static readonly Dictionary<string, Unit> _byName = new Dictionary<string, Unit>(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Unit> _bySymbol = new Dictionary<string, Unit>(StringComparer.Ordinal);

    /// <summary>Gets a read-only list of every registered unit.</summary>
    public static readonly IReadOnlyList<Unit> AllUnits = _unitsList;

    static UnitsLocator()
    {
        Register([
            // SI Base Units
            SIUnits.Second,
            SIUnits.Meter,
            SIUnits.Kilogram,
            SIUnits.Ampere,
            SIUnits.Kelvin,
            SIUnits.Mol,
            SIUnits.Candela,

            // SI Derived Units
            SIUnits.Radian,
            SIUnits.Degree,
            SIUnits.Steradian,
            SIUnits.Hertz,
            SIUnits.Newton,
            SIUnits.Pascal,
            SIUnits.Joule,
            SIUnits.Watt,
            SIUnits.Coulomb,
            SIUnits.Volt,
            SIUnits.Farad,
            SIUnits.Ohm,
            SIUnits.Siemens,
            SIUnits.Weber,
            SIUnits.Tesla,
            SIUnits.Henry,
            SIUnits.Lumen,
            SIUnits.Lux,
            SIUnits.Becquerel,
            SIUnits.Gray,
            SIUnits.Sievert,
            SIUnits.CatalyticActivity,

            // (Common) Secondary Units
            SIUnits.Area,
            SIUnits.Volume,
            SIUnits.Speed,
            SIUnits.Acceleration,
            SIUnits.VolumetricFlow,
            SIUnits.AngularVelocity,

            // ChemicalUnits
            Units.MassDensity,
            Units.MassConcentration,
            Units.MolarConcentration,

            // ElectricalUnits
            Units.ElectricField,
            Units.CurrentDensity,

            // LengthUnits
            Units.Millimeter,
            Units.Centimeter,
            Units.Kilometer,
            Units.Micrometer,
            Units.Nanometer,
            Units.Inch,
            Units.Feet,
            Units.Yard,
            Units.Mile,
            Units.NauticalMile,
            Units.Mil,

            // MagneticalUnits
            Units.MagneticField,
            Units.MagneticMoment,
            Units.MagneticGradient,

            // MassUnits
            Units.Milligram,
            Units.Gram,
            Units.Ounce,
            Units.Pound,
            Units.MetricTon,
            Units.ShortTon,
            Units.LongTon,

            // PhysicalUnits
            Units.DynamicViscosity,
            Units.MomentOfForce,
            Units.SurfaceTension,
            Units.GyromagneticRatio,
            Units.EnergyDensity,

            // ForceUnits
            Units.KiloNewton,
            Units.Dyne,

            // PressureUnits
            Units.KiloPascal,
            Units.Bar,
            Units.Millibar,
            Units.Atmosphere,
            Units.Torr,
            Units.PSI,

            // EnergyUnits
            Units.Calorie,
            Units.Kilocalorie,
            Units.KilowattHour,
            Units.Electronvolt,
            Units.BTU,

            // SpeedUnits
            Units.Knot,
            Units.Mach,

            // TemperatureUnits
            Units.Celsius,
            Units.Fahrenheit,

            // TimeUnits
            Units.Minute,
            Units.Hour,
            Units.Day,
            Units.Week,

            // VolumetricUnits
            Units.Liter,
            Units.Pint,
            Units.Quart,
            Units.Gallon,

            // FlowUnits
            Units.LiterPerMinute,

            // AngularVelocityUnits
            Units.RevolutionsPerMinute
        ]);
    }

    /// <summary>
    /// Finds a unit by its display symbol (case-sensitive), or <c>null</c> if not found.
    /// </summary>
    public static Unit? FindBySymbol(string symbol)
    {
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));

        _bySymbol.TryGetValue(symbol, out var unit);

        return unit;
    }

    /// <summary>
    /// Finds a unit by its name (case-insensitive), or <c>null</c> if not found.
    /// </summary>
    public static Unit? FindByName(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        _byName.TryGetValue(name, out var unit);

        return unit;
    }

    #region Private

    private static void Register(Unit[] units)
    {
        foreach (var unit in units)
        {
            _unitsList.Add(unit);

            if (!String.IsNullOrEmpty(unit.Symbol))
                _bySymbol[unit.Symbol] = unit;

            if (!String.IsNullOrEmpty(unit.Name))
                _byName[unit.Name] = unit;
        }
    }

    #endregion
}
