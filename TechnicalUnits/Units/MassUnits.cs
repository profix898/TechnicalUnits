using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class MassUnits
{
    public static readonly Unit Milligram = new DerivedUnit("milligram", "mg", 1e-6, Kilogram);
    public static readonly Unit Gram = new DerivedUnit("gram", "g", 0.001, Kilogram);
    public static readonly Unit Ounce = new DerivedUnit("ounce", "oz", 0.028349523125, Kilogram);
    public static readonly Unit Pound = new DerivedUnit("pound", "lb", 0.45359237, Kilogram);
    public static readonly Unit MetricTon = new DerivedUnit("metricTon", "t", 1000.0, Kilogram);
    public static readonly Unit ShortTon = new DerivedUnit("shortTon", "tn", 907.18474, Kilogram);
    public static readonly Unit LongTon = new DerivedUnit("longTon", "LT", 1016.0469, Kilogram);
}

