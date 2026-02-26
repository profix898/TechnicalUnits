using System;
using static System.Math;

namespace TechnicalUnits.Internal;

/// <summary>
/// Low-level numeric helper methods used by the formatter, parser, and simple formatting utilities.
/// </summary>
internal static class MathHelper
{
    /// <summary>Returns the fractional part of <paramref name="x" /> (i.e. <c>x − (int)x</c>).</summary>
    /// <remarks>Truncation is towards zero; only valid for non-negative values in the current usage.</remarks>
    public static double GetDecimals(double x) => x - (int) x;

    /// <summary>
    /// Returns the exponent to base 1000 of <paramref name="num" />, rounded towards negative infinity.
    /// </summary>
    /// <returns>0 for <paramref name="num" /> == 0; 1 for 1000 … 999 999; −1 for 0.001 … 0.999; etc.</returns>
    public static int GetExp3Value(double num)
    {
        if (Abs(num) > Double.Epsilon)
            return (int) Floor(Log(Abs(num), 1000));

        return 0;
    }

    /// <summary>
    /// Determines whether <paramref name="a" /> and <paramref name="b" /> are approximately equal
    /// within the given relative <paramref name="epsilon" />.
    /// </summary>
    public static bool NearlyEqual(double a, double b, double epsilon)
    {
        var diff = Abs(a - b);

        if (diff < Double.Epsilon)
            return true;

        if (a == 0 || b == 0 || diff < Double.MinValue)
            return diff < epsilon * Double.MinValue;

        // Use relative error
        return diff / (Abs(a) + Abs(b)) < epsilon;
    }
}
