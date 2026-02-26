using static System.Math;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// Defines the seven SI base units, the SI derived units with special names, and
/// common secondary (compound) units such as area, volume, speed, and acceleration.
/// </summary>
public static class SIUnits
{
    // SI Base Units
    public static readonly Unit Second = new Unit("time", Dimension.Second);
    public static readonly Unit Meter = new Unit("length", Dimension.Meter);
    public static readonly Unit Kilogram = new Unit("mass", Dimension.Kilogram);
    public static readonly Unit Ampere = new Unit("current", Dimension.Ampere);
    public static readonly Unit Kelvin = new Unit("temperature", Dimension.Kelvin);
    public static readonly Unit Mol = new Unit("amountOfSubstance", Dimension.Mol);
    public static readonly Unit Candela = new Unit("luminousIntensity", Dimension.Candela);

    public static readonly Unit Dimensionless = new Unit("", "", Dimension.Dimensionless);

    // SI Derived Units
    public static readonly Unit Radian = new Unit("planeAngleRad", "rad", Dimension.Angle);
    public static readonly DerivedUnit Degree = new DerivedUnit("planeAngleDeg", "deg", PI / 180.0, Radian);
    public static readonly Unit Steradian = new Unit("solidAngle", "sr", Dimension.Angle);
    public static readonly Unit Hertz = new Unit("frequency", "Hz", Dimensionless / Second);
    public static readonly Unit Newton = new Unit("force", "N", Kilogram * Meter / (Second * Second));
    public static readonly Unit Pascal = new Unit("pressure", "Pa", Newton / (Meter * Meter));
    public static readonly Unit Joule = new Unit("energy", "J", Newton * Meter);
    public static readonly Unit Watt = new Unit("power", "W", Joule / Second);
    public static readonly Unit Coulomb = new Unit("charge", "C", Second * Ampere);
    public static readonly Unit Volt = new Unit("voltage", "V", Watt / Ampere);
    public static readonly Unit Farad = new Unit("capacitance", "F", Coulomb / Volt);
    public static readonly Unit Ohm = new Unit("resistance", "Ω", Volt / Ampere);
    public static readonly Unit Siemens = new Unit("conductance", "S", Dimensionless / Ohm);
    public static readonly Unit Weber = new Unit("magneticFlux", "Wb", Volt * Second);
    public static readonly Unit Tesla = new Unit("magneticFluxDensity", "T", Weber / (Meter * Meter));
    public static readonly Unit Henry = new Unit("inductance", "H", Weber / Ampere);
    public static readonly Unit Lumen = new Unit("luminousFlux", "lm", Candela * Steradian);
    public static readonly Unit Lux = new Unit("illuminance", "lx", Lumen / (Meter * Meter));
    public static readonly Unit Becquerel = new Unit("radioactivity", "Bq", Dimensionless / Second);
    public static readonly Unit Gray = new Unit("absorbedDose", "Gy", Joule / Kilogram);
    public static readonly Unit Sievert = new Unit("equivalentDose", "Sv", Joule / Kilogram);
    public static readonly Unit CatalyticActivity = new Unit("catalyticActivity", "kat", Mol / Second);

    // (Common) Secondary Units
    public static readonly Unit Area = new Unit("area", Meter * Meter);
    public static readonly Unit Volume = new Unit("volume", Meter * Meter * Meter);
    public static readonly Unit Speed = new Unit("speed", Meter / Second);
    public static readonly Unit Acceleration = new Unit("acceleration", Meter / (Second * Second));
    public static readonly Unit VolumetricFlow = new Unit("volumetricFlow", Meter * Meter * Meter / Second);
    public static readonly Unit AngularVelocity = new Unit("angularVelocity", Radian / Second);
}
