using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class SpeedUnits
{
    public static readonly Unit Knot = new DerivedUnit("knot", "kn", 0.514, Speed);
    public static readonly Unit Mach = new DerivedUnit("mach", "Ma", 340.3, Speed);
}
