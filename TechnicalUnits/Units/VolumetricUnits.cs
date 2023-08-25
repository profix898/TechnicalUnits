using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class VolumetricUnits
{
    public static readonly DerivedUnit Liter = new DerivedUnit("liter", "L", 1e-3, Volume);
    public static readonly DerivedUnit Pint = new DerivedUnit("pint", "pt", 0.000473176, Volume);
    public static readonly DerivedUnit Quart = new DerivedUnit("quart", "qt", 0.000946353, Volume);
    public static readonly DerivedUnit Gallon = new DerivedUnit("gallon", "gal", 0.00378541, Volume);
}
