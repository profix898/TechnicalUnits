using System;
using static System.Math;

namespace TechnicalUnits.Units;

public sealed class Dimension : IEquatable<Dimension>
{
    // SI base units (https://en.wikipedia.org/wiki/SI_base_unit)

    private readonly short t; // Time
    private readonly short l; // Length
    private readonly short m; // Mass
    private readonly short i; // Current
    private readonly short th; // Temperature
    private readonly short n; // Amount of substance
    private readonly short j; // Luminous intensity
    private readonly short a; // Angle

    public Dimension(int t = 0, int l = 0, int m = 0, int i = 0, int th = 0, int n = 0, int j = 0, int a = 0)
        : this((short) t, (short) l, (short) m, (short) i, (short) th, (short) n, (short) j, (short) a)
    {
    }

    public Dimension(short t = 0, short l = 0, short m = 0, short i = 0, short th = 0, short n = 0, short j = 0, short a = 0)
    {
        this.t = t;
        this.l = l;
        this.m = m;
        this.i = i;
        this.th = th;
        this.n = n;
        this.j = j;
        this.a = a;
    }

    public static Dimension operator *(Dimension a, Dimension b)
    {
        return new Dimension(a.t + b.t, a.l + b.l, a.m + b.m, a.i + b.i, a.th + b.th, a.n + b.n, a.j + b.j, a.a + b.a);
    }

    public static Dimension operator /(Dimension a, Dimension b)
    {
        return new Dimension(a.t - b.t, a.l - b.l, a.m - b.m, a.i - b.i, a.th - b.th, a.n - b.n, a.j - b.j, a.a - b.a);
    }

    public static Dimension operator ^(Dimension a, int exp)
    {
        if (exp < 0)
        {
            exp = Abs(exp);

            return new Dimension(a.t / exp, a.l / exp, a.m / exp, a.i / exp, a.th / exp, a.n / exp, a.j / exp, a.a / exp);
        }

        return new Dimension(a.t * exp, a.l * exp, a.m * exp, a.i * exp, a.th * exp, a.n * exp, a.j * exp, a.a * exp);
    }

    #region Equality members

    public bool Equals(Dimension other)
    {
        return t == other.t
               && l == other.l
               && m == other.m
               && i == other.i
               && th == other.th
               && n == other.n
               && j == other.j
               && a == other.a;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != typeof(Dimension))
            return false;

        return Equals((Dimension)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(t, l, m, i, th, n, j, a);
    }

    public static bool operator ==(Dimension left, Dimension right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Dimension left, Dimension right)
    {
        return !Equals(left, right);
    }

    #endregion

    #region ToString

    public static implicit operator string(Dimension dim) => dim.ToString();

    public override string ToString()
    {
        var text = "";
        if (t > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("s", t);
        if (l > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("m", l);
        if (m > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("kg", m);
        if (i > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("A", i);
        if (th > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("K", th);
        if (n > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("mol", n);
        if (j > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("cd", j);
        if (a > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("°", a);

        if (t < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("s", t);
        if (l < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("m", l);
        if (m < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("kg", m);
        if (i < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("A", i);
        if (th < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("K", th);
        if (n < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("mol", n);
        if (j < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("cd", j);
        if (a < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("°", a);

        return text;
    }

    private static string PowerOf(string symbol, int power)
    {
        return power switch
        {
            -9 => $"{symbol}⁻⁹",
            -8 => $"{symbol}⁻⁸",
            -7 => $"{symbol}⁻⁷",
            -6 => $"{symbol}⁻⁶",
            -5 => $"{symbol}⁻⁵",
            -4 => $"{symbol}⁻⁴",
            -3 => $"{symbol}⁻³",
            -2 => $"{symbol}⁻²",
            -1 => $"{symbol}⁻¹",
            0 => String.Empty,
            1 => symbol,
            2 => $"{symbol}²",
            3 => $"{symbol}³",
            4 => $"{symbol}⁴",
            5 => $"{symbol}⁵",
            6 => $"{symbol}⁶",
            7 => $"{symbol}⁷",
            8 => $"{symbol}⁸",
            9 => $"{symbol}⁹",
            _ => $"{symbol}^({power})"
        };
    }

    #endregion

    #region BaseUnitDimensions

    public static readonly Dimension Dimensionless = new Dimension(0);
    public static readonly Dimension Second = new Dimension(1);
    public static readonly Dimension Meter = new Dimension(0, 1);
    public static readonly Dimension Kilogram = new Dimension(0, 0, 1);
    public static readonly Dimension Ampere = new Dimension(0, 0, 0, 1);
    public static readonly Dimension Kelvin = new Dimension(0, 0, 0, 0, 1);
    public static readonly Dimension Mol = new Dimension(0, 0, 0, 0, 0, 1);
    public static readonly Dimension Candela = new Dimension(0, 0, 0, 0, 0, 0, 1);
    public static readonly Dimension Angle = new Dimension(0, 0, 0, 0, 0, 0, 0, 1);

    #endregion
}
