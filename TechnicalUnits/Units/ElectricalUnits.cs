using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class ElectricalUnits
{
    public static readonly Unit ElectricField = new Unit("electricField", "V/m", Volt / Meter);
    public static readonly Unit CurrentDensity = new Unit("currentDensity", "A/m²", Ampere / Area);
}