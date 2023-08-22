using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class LengthUnits
{
    public static readonly Unit Millimeter = new DerivedUnit("centimeter", "cm", 0.01, Meter);
    public static readonly Unit Inch = new DerivedUnit("inch", "in", 0.0254, Meter);
    public static readonly Unit Feet = new DerivedUnit("feet", "ft", 0.3048, Meter);
    public static readonly Unit Yard = new DerivedUnit("yard", "yd", 0.9144, Meter);
    public static readonly Unit Mile = new DerivedUnit("mile", "mile", 1609.344, Meter);
    public static readonly Unit Mil = new DerivedUnit("mil", "mil", 25.4e-6, Meter);
}
