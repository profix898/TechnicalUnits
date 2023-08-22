using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class ChemicalUnits
{
    public static readonly Unit MassDensity = new Unit("massDensity", "kg/m³", Kilogram / Volume);
    public static readonly Unit MassConcentration = new Unit("massConc", "kg/m³ (= g/L)", Kilogram / Volume);
    public static readonly Unit MolarConcentration = new Unit("molarConc", "mol/m³", Mol / Volume);
}