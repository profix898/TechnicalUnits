using System;
using TechnicalUnits.Extensions;
using TechnicalUnits.UnitDefinition;

namespace DemoConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello TechnicalUnits!");

        Console.WriteLine(SIUnits.Meter.FormatSimple(1.23e3));
        Console.WriteLine(SIUnits.Meter.Format(1.23e3));
        Console.WriteLine(SIUnits.Meter.FormatSimple(1.23e-3));
        Console.WriteLine(SIUnits.Meter.Format(1.23e-3));

        Console.WriteLine(SIUnits.Meter.FormatSimple(1.2345e3));
        Console.WriteLine(SIUnits.Meter.Format(1.2345e3));
        Console.WriteLine(SIUnits.Meter.FormatSimple(1.2345e-3));
        Console.WriteLine(SIUnits.Meter.Format(1.2345e-3));

        Console.WriteLine(SIUnits.Meter.FormatSimple(1.2e3));
        Console.WriteLine(SIUnits.Meter.Format(1.2e3));
        Console.WriteLine(SIUnits.Meter.FormatSimple(1.2e-3));
        Console.WriteLine(SIUnits.Meter.Format(1.2e-3));

        Console.WriteLine($"1.2e3 -> {SIUnits.Meter.Parse(SIUnits.Meter.Format(1.2e3)):F3}");
        Console.WriteLine($"1.2e-3 -> {SIUnits.Meter.Parse(SIUnits.Meter.Format(1.2e-3)):F6}");

        Console.WriteLine($"1k23 -> {SIUnits.Meter.Parse("1k23"):F3}");
        Console.WriteLine($"1k23m -> {SIUnits.Meter.Parse("1k23m"):F3}");

        Console.WriteLine($"1m23 -> {SIUnits.Meter.Parse("1m23"):F6}");
        Console.WriteLine($"1m23m -> {SIUnits.Meter.Parse("1m23m"):F6}");

        Console.WriteLine($"10.0in -> {SIUnits.Meter.Parse("10.0in", [Units.Inch, Units.Mil]):F6}");
        Console.WriteLine($"10.0mil -> {SIUnits.Meter.Parse("10.0mil", [Units.Inch, Units.Mil]):F6}");
    }
}
