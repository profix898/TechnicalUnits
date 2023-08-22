using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class PhysicalUnits
{
    public static readonly Unit DynamicViscosity = new Unit("dynamicViscosity", "Pa s", Pascal * Second);
    public static readonly Unit MomentOfForce = new Unit("momentOfForce", "N m", Newton * Meter);
    public static readonly Unit SurfaceTension = new Unit("surfaceTension", "N/m", Newton / Meter);
    public static readonly Unit GyromagneticRatio = new Unit("gyromagneticRatio", "rad/(s*T)", Radian / (Second * Tesla));
    public static readonly Unit EnergyDensity = new Unit("energyDensity", "J/m³", Joule / Volume);
}