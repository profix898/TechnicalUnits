using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class MagneticalUnits
{
    public static readonly Unit MagneticField = new Unit("magneticField", "A/m", Ampere / Meter);
    public static readonly Unit MagneticMoment = new Unit("magneticMoment", "Am²", Ampere * Meter * Meter);
    public static readonly Unit MagneticGradient = new Unit("magneticGradient", "T/m", Tesla / Meter);
}