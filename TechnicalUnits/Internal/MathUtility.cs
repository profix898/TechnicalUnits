using System;
using static System.Math;

namespace TechnicalUnits.Internal;

internal static class MathUtility
{
    public static double GetDecimals(double x)
    {
        return x - (int) x;
    }

    /// <summary>GetExp3Value: Get the exponent to base 1000 of the argument as integer rounded towards neg.infinity.</summary>
    /// <returns>Returns 0 for 0 as input. Returns 1 for 1000, 2000, ... as input.</returns>
    public static int GetExp3Value(double num)
    {
        if (Abs(num) > Double.Epsilon)
            return (int) Floor(Log(Abs(num), 1000));

        return 0;
    }

    public static bool NearlyEqual(double a, double b, double epsilon)
    {
        var absA = Abs(a);
        var absB = Abs(b);
        var diff = Abs(a - b);

        if (Abs(a - b) < Double.Epsilon)
            return true;

        if (a == 0 || b == 0 || diff < Double.MinValue)
            return diff < epsilon * Double.MinValue;

        // Use relative error
        return diff / (absA + absB) < epsilon;
    }
}
