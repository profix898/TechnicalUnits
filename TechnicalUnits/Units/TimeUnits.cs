using static TechnicalUnits.Units.SIUnits;

namespace TechnicalUnits.Units;

public static class TimeUnits
{
    public static readonly DerivedUnit Minute = new DerivedUnit("minute", "min", 60.0, Second);
    public static readonly DerivedUnit Hour = new DerivedUnit("hour", "hr", 3600.0, Second);
    public static readonly DerivedUnit Day = new DerivedUnit("day", "d", 86400.0, Second);
    public static readonly DerivedUnit Week = new DerivedUnit("week", "wk", 604800.0, Second);
}

