using System.Collections.Generic;

namespace TechnicalUnits.Units;

public static class UnitsLocator
{
    public static readonly List<Unit> Units = new List<Unit>();

    static UnitsLocator()
    {
        Units.AddRange(new[]
        {
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

            // ChemicalUnits
            ChemicalUnits.MassDensity,
            ChemicalUnits.MassConcentration,
            ChemicalUnits.MolarConcentration,

            // ElectricalUnits
            ElectricalUnits.ElectricField,
            ElectricalUnits.CurrentDensity,

            // LengthUnits
            LengthUnits.Millimeter,
            LengthUnits.Inch,
            LengthUnits.Feet,
            LengthUnits.Yard,
            LengthUnits.Mile,
            LengthUnits.Mil,

            // MagneticalUnits
            MagneticalUnits.MagneticField,
            MagneticalUnits.MagneticMoment,
            MagneticalUnits.MagneticGradient,

            // MassUnits
            MassUnits.Milligram,
            MassUnits.Gram,
            MassUnits.Ounce,
            MassUnits.Pound,
            MassUnits.MetricTon,
            MassUnits.ShortTon,
            MassUnits.LongTon,

            // PhysicalUnits
            PhysicalUnits.DynamicViscosity,
            PhysicalUnits.MomentOfForce,
            PhysicalUnits.SurfaceTension,
            PhysicalUnits.GyromagneticRatio,
            PhysicalUnits.EnergyDensity,

            // SpeedUnits
            SpeedUnits.Knot,
            SpeedUnits.Mach,

            // TemperatureUnits
            TemperatureUnits.Celcius,
            TemperatureUnits.Fahrenheit,

            // TimeUnits
            TimeUnits.Minute,
            TimeUnits.Hour,
            TimeUnits.Day,
            TimeUnits.Week,

            // VolumetricUnits
            VolumetricUnits.Liter,
            VolumetricUnits.Pint,
            VolumetricUnits.Quart,
            VolumetricUnits.Gallon,
        });
    }
}
