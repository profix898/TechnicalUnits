using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class MassUnits
{
    public static readonly DerivedUnit Milligram = new DerivedUnit("milligram", "mg", 1e-6, Kilogram);
    public static readonly DerivedUnit Gram = new DerivedUnit("gram", "g", 0.001, Kilogram);
    public static readonly DerivedUnit Ounce = new DerivedUnit("ounce", "oz", 0.028349523125, Kilogram);
    public static readonly DerivedUnit Pound = new DerivedUnit("pound", "lb", 0.45359237, Kilogram);
    public static readonly DerivedUnit MetricTon = new DerivedUnit("metricTon", "t", 1000.0, Kilogram);
    public static readonly DerivedUnit ShortTon = new DerivedUnit("shortTon", "tn", 907.18474, Kilogram);
    public static readonly DerivedUnit LongTon = new DerivedUnit("longTon", "LT", 1016.0469, Kilogram);
}

