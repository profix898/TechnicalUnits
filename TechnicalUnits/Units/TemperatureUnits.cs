using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class TemperatureUnits
{
    public static readonly Unit Celcius = new DerivedUnit("celcius", "°C", d => d - 273.15, d => d + 273.15, Kelvin);
    public static readonly Unit Fahrenheit = new DerivedUnit("fahrenheit", "°F", d => 5.0 / 9.0 * (d - 273.15 + 32), d => 5.0 / 9.0 * (d - 32) + 273.15, Kelvin);
}
