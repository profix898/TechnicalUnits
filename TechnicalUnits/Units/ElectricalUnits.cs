using TechnicalUnits.Units;
using static TechnicalUnits.Units.SIUnits;

public static class ElectricalUnits
{
    public static readonly Unit ElectricField = new Unit("electricField", "V/m", Volt / Meter);
    public static readonly Unit CurrentDensity = new Unit("currentDensity", "A/m²", Ampere / Area);
}