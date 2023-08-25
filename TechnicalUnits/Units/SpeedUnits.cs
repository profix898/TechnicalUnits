using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class SpeedUnits
{
    public static readonly DerivedUnit Knot = new DerivedUnit("knot", "kn", 0.514, Speed);
    public static readonly DerivedUnit Mach = new DerivedUnit("mach", "Ma", 340.3, Speed);
}
