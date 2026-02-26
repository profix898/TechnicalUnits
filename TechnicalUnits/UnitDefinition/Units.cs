using static System.Math;
using static TechnicalUnits.UnitDefinition.SIUnits;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// Defines commonly used derived and non-SI units organised by physical category
/// (length, mass, temperature, etc.).
/// </summary>
public static class Units
{
    // ──────────────────────────────────────────────
    //  Chemical Units
    // ──────────────────────────────────────────────

    public static readonly Unit MassDensity = new Unit("massDensity", "kg/m³", Kilogram / Volume);
    public static readonly Unit MassConcentration = new Unit("massConc", "kg/m³ (= g/L)", Kilogram / Volume);
    public static readonly Unit MolarConcentration = new Unit("molarConc", "mol/m³", Mol / Volume);

    // ──────────────────────────────────────────────
    //  Electrical Units
    // ──────────────────────────────────────────────

    public static readonly Unit ElectricField = new Unit("electricField", "V/m", Volt / Meter);
    public static readonly Unit CurrentDensity = new Unit("currentDensity", "A/m²", Ampere / Area);

    // ──────────────────────────────────────────────
    //  Length Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Millimeter = new DerivedUnit("millimeter", "mm", 0.001, Meter);
    public static readonly DerivedUnit Centimeter = new DerivedUnit("centimeter", "cm", 0.01, Meter);
    public static readonly DerivedUnit Kilometer = new DerivedUnit("kilometer", "km", 1000.0, Meter);
    public static readonly DerivedUnit Micrometer = new DerivedUnit("micrometer", "µm", 1e-6, Meter);
    public static readonly DerivedUnit Nanometer = new DerivedUnit("nanometer", "nm", 1e-9, Meter);
    public static readonly DerivedUnit Inch = new DerivedUnit("inch", "in", 0.0254, Meter);
    public static readonly DerivedUnit Feet = new DerivedUnit("feet", "ft", 0.3048, Meter);
    public static readonly DerivedUnit Yard = new DerivedUnit("yard", "yd", 0.9144, Meter);
    public static readonly DerivedUnit Mile = new DerivedUnit("mile", "mile", 1609.344, Meter);
    public static readonly DerivedUnit NauticalMile = new DerivedUnit("nauticalMile", "nmi", 1852.0, Meter);
    public static readonly DerivedUnit Mil = new DerivedUnit("mil", "mil", 25.4e-6, Meter);

    // ──────────────────────────────────────────────
    //  Magnetical Units
    // ──────────────────────────────────────────────

    public static readonly Unit MagneticField = new Unit("magneticField", "A/m", Ampere / Meter);
    public static readonly Unit MagneticMoment = new Unit("magneticMoment", "Am²", Ampere * Meter * Meter);
    public static readonly Unit MagneticGradient = new Unit("magneticGradient", "T/m", Tesla / Meter);

    // ──────────────────────────────────────────────
    //  Mass Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Milligram = new DerivedUnit("milligram", "mg", 1e-6, Kilogram);
    public static readonly DerivedUnit Gram = new DerivedUnit("gram", "g", 0.001, Kilogram);
    public static readonly DerivedUnit Ounce = new DerivedUnit("ounce", "oz", 0.028349523125, Kilogram);
    public static readonly DerivedUnit Pound = new DerivedUnit("pound", "lb", 0.45359237, Kilogram);
    public static readonly DerivedUnit MetricTon = new DerivedUnit("metricTon", "t", 1000.0, Kilogram);
    public static readonly DerivedUnit ShortTon = new DerivedUnit("shortTon", "tn", 907.18474, Kilogram);
    public static readonly DerivedUnit LongTon = new DerivedUnit("longTon", "LT", 1016.0469, Kilogram);

    // ──────────────────────────────────────────────
    //  Physical Units
    // ──────────────────────────────────────────────

    public static readonly Unit DynamicViscosity = new Unit("dynamicViscosity", "Pa s", Pascal * Second);
    public static readonly Unit MomentOfForce = new Unit("momentOfForce", "N m", Newton * Meter);
    public static readonly Unit SurfaceTension = new Unit("surfaceTension", "N/m", Newton / Meter);
    public static readonly Unit GyromagneticRatio = new Unit("gyromagneticRatio", "rad/(s*T)", Radian / (Second * Tesla));
    public static readonly Unit EnergyDensity = new Unit("energyDensity", "J/m³", Joule / Volume);

    // ──────────────────────────────────────────────
    //  Force Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit KiloNewton = new DerivedUnit("kilonewton", "kN", 1000.0, Newton);
    public static readonly DerivedUnit Dyne = new DerivedUnit("dyne", "dyn", 1e-5, Newton);

    // ──────────────────────────────────────────────
    //  Pressure Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit KiloPascal = new DerivedUnit("kilopascal", "kPa", 1000.0, Pascal);
    public static readonly DerivedUnit Bar = new DerivedUnit("bar", "bar", 1e5, Pascal);
    public static readonly DerivedUnit Millibar = new DerivedUnit("millibar", "mbar", 100.0, Pascal);
    public static readonly DerivedUnit Atmosphere = new DerivedUnit("atmosphere", "atm", 101325.0, Pascal);
    public static readonly DerivedUnit Torr = new DerivedUnit("torr", "Torr", 133.322, Pascal);
    public static readonly DerivedUnit PSI = new DerivedUnit("psi", "psi", 6894.757, Pascal);

    // ──────────────────────────────────────────────
    //  Energy Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Calorie = new DerivedUnit("calorie", "cal", 4.184, Joule);
    public static readonly DerivedUnit Kilocalorie = new DerivedUnit("kilocalorie", "kcal", 4184.0, Joule);
    public static readonly DerivedUnit KilowattHour = new DerivedUnit("kilowattHour", "kWh", 3.6e6, Joule);
    public static readonly DerivedUnit Electronvolt = new DerivedUnit("electronvolt", "eV", 1.602176634e-19, Joule);
    public static readonly DerivedUnit BTU = new DerivedUnit("btu", "BTU", 1055.06, Joule);

    // ──────────────────────────────────────────────
    //  Speed Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Knot = new DerivedUnit("knot", "kn", 1852.0 / 3600.0, Speed);
    public static readonly DerivedUnit Mach = new DerivedUnit("mach", "Ma", 340.3, Speed);

    // ──────────────────────────────────────────────
    //  Temperature Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Celsius = new DerivedUnit("celsius", "°C", d => d - 273.15, d => d + 273.15, Kelvin);
    public static readonly DerivedUnit Fahrenheit = new DerivedUnit("fahrenheit", "°F", d => ((d - 273.15) * 9.0 / 5.0) + 32, d => ((d - 32) * 5.0 / 9.0) + 273.15, Kelvin);

    // ──────────────────────────────────────────────
    //  Time Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Minute = new DerivedUnit("minute", "min", 60.0, Second);
    public static readonly DerivedUnit Hour = new DerivedUnit("hour", "hr", 3600.0, Second);
    public static readonly DerivedUnit Day = new DerivedUnit("day", "d", 86400.0, Second);
    public static readonly DerivedUnit Week = new DerivedUnit("week", "wk", 604800.0, Second);

    // ──────────────────────────────────────────────
    //  Volumetric Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit Liter = new DerivedUnit("liter", "L", 1e-3, Volume);
    public static readonly DerivedUnit Pint = new DerivedUnit("pint", "pt", 0.000473176, Volume);
    public static readonly DerivedUnit Quart = new DerivedUnit("quart", "qt", 0.000946353, Volume);
    public static readonly DerivedUnit Gallon = new DerivedUnit("gallon", "gal", 0.00378541, Volume);

    // ──────────────────────────────────────────────
    //  Flow Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit LiterPerMinute = new DerivedUnit("literPerMinute", "L/min", 1e-3 / 60.0, VolumetricFlow);

    // ──────────────────────────────────────────────
    //  Angular Velocity Units
    // ──────────────────────────────────────────────

    public static readonly DerivedUnit RevolutionsPerMinute = new DerivedUnit("rpm", "rpm", 2.0 * PI / 60.0, AngularVelocity);
}
