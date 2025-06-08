using System;
using static System.Math;

namespace TechnicalUnits.Units;

public sealed class Dimension : IEquatable<Dimension>
{
    // SI base units (https://en.wikipedia.org/wiki/SI_base_unit)

    private readonly short _t; // Time
    private readonly short _l; // Length
    private readonly short _m; // Mass
    private readonly short _i; // Current
    private readonly short _th; // Temperature
    private readonly short _n; // Amount of substance
    private readonly short _j; // Luminous intensity
    private readonly short _a; // Angle

    public Dimension(int t = 0, int l = 0, int m = 0, int i = 0, int th = 0, int n = 0, int j = 0, int a = 0)
        : this((short) t, (short) l, (short) m, (short) i, (short) th, (short) n, (short) j, (short) a)
    {
    }

    public Dimension(short t = 0, short l = 0, short m = 0, short i = 0, short th = 0, short n = 0, short j = 0, short a = 0)
    {
        _t = t;
        _l = l;
        _m = m;
        _i = i;
        _th = th;
        _n = n;
        _j = j;
        _a = a;
    }

    public static Dimension operator *(Dimension a, Dimension b)
    {
        return new Dimension(a._t + b._t, a._l + b._l, a._m + b._m, a._i + b._i, a._th + b._th, a._n + b._n, a._j + b._j, a._a + b._a);
    }

    public static Dimension operator /(Dimension a, Dimension b)
    {
        return new Dimension(a._t - b._t, a._l - b._l, a._m - b._m, a._i - b._i, a._th - b._th, a._n - b._n, a._j - b._j, a._a - b._a);
    }

    public static Dimension operator ^(Dimension a, int exp)
    {
        if (exp < 0)
        {
            exp = Abs(exp);

            return new Dimension(a._t / exp, a._l / exp, a._m / exp, a._i / exp, a._th / exp, a._n / exp, a._j / exp, a._a / exp);
        }

        return new Dimension(a._t * exp, a._l * exp, a._m * exp, a._i * exp, a._th * exp, a._n * exp, a._j * exp, a._a * exp);
    }

    #region Equality members

    public bool Equals(Dimension other)
    {
        return _t == other._t
               && _l == other._l
               && _m == other._m
               && _i == other._i
               && _th == other._th
               && _n == other._n
               && _j == other._j
               && _a == other._a;
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
        return HashCode.Combine(_t, _l, _m, _i, _th, _n, _j, _a);
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
        if (_t > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("s", _t);
        if (_l > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("m", _l);
        if (_m > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("kg", _m);
        if (_i > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("A", _i);
        if (_th > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("K", _th);
        if (_n > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("mol", _n);
        if (_j > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("cd", _j);
        if (_a > 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("°", _a);

        if (_t < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("s", _t);
        if (_l < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("m", _l);
        if (_m < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("kg", _m);
        if (_i < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("A", _i);
        if (_th < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("K", _th);
        if (_n < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("mol", _n);
        if (_j < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("cd", _j);
        if (_a < 0)
            text += (text.Length > 0 ? " " : "") + PowerOf("°", _a);

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
