using System;
using System.Text;

namespace TechnicalUnits.UnitDefinition;

/// <summary>
/// Represents the dimensional exponents of a physical quantity in terms of the
/// seven SI base dimensions (time, length, mass, current, temperature, amount of substance,
/// luminous intensity) plus an additional angle dimension.
/// </summary>
/// <remarks>
/// Dimensions follow the rules of dimensional analysis: multiplication adds exponents,
/// division subtracts exponents, and exponentiation scales them.
/// </remarks>
public readonly struct Dimension : IEquatable<Dimension>
{
    private readonly short _t; // Time
    private readonly short _l; // Length
    private readonly short _m; // Mass
    private readonly short _i; // Current
    private readonly short _th; // Temperature
    private readonly short _n; // Amount of substance
    private readonly short _j; // Luminous intensity
    private readonly short _a; // Angle

    /// <summary>
    /// Initializes a new <see cref="Dimension" /> with the specified exponents (as <see cref="int" />).
    /// </summary>
    /// <param name="t">Time (s) exponent.</param>
    /// <param name="l">Length (m) exponent.</param>
    /// <param name="m">Mass (kg) exponent.</param>
    /// <param name="i">Electric current (A) exponent.</param>
    /// <param name="th">Temperature (K) exponent.</param>
    /// <param name="n">Amount of substance (mol) exponent.</param>
    /// <param name="j">Luminous intensity (cd) exponent.</param>
    /// <param name="a">Angle exponent.</param>
    public Dimension(int t = 0, int l = 0, int m = 0, int i = 0, int th = 0, int n = 0, int j = 0, int a = 0)
        : this((short) t, (short) l, (short) m, (short) i, (short) th, (short) n, (short) j, (short) a)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Dimension" /> with the specified exponents (as <see cref="short" />).
    /// </summary>
    /// <param name="t">Time (s) exponent.</param>
    /// <param name="l">Length (m) exponent.</param>
    /// <param name="m">Mass (kg) exponent.</param>
    /// <param name="i">Electric current (A) exponent.</param>
    /// <param name="th">Temperature (K) exponent.</param>
    /// <param name="n">Amount of substance (mol) exponent.</param>
    /// <param name="j">Luminous intensity (cd) exponent.</param>
    /// <param name="a">Angle exponent.</param>
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

    /// <summary>
    /// Multiplies two dimensions by adding their exponents (dimensional analysis).
    /// </summary>
    public static Dimension operator *(Dimension a, Dimension b)
        => new Dimension(a._t + b._t, a._l + b._l, a._m + b._m, a._i + b._i, a._th + b._th, a._n + b._n, a._j + b._j, a._a + b._a);

    /// <summary>
    /// Divides two dimensions by subtracting their exponents (dimensional analysis).
    /// </summary>
    public static Dimension operator /(Dimension a, Dimension b)
        => new Dimension(a._t - b._t, a._l - b._l, a._m - b._m, a._i - b._i, a._th - b._th, a._n - b._n, a._j - b._j, a._a - b._a);

    /// <summary>
    /// Raises a dimension to an integer power by scaling all exponents.
    /// </summary>
    /// <param name="a">The dimension to exponentiate.</param>
    /// <param name="exp">The integer exponent (may be negative).</param>
    public static Dimension operator ^(Dimension a, int exp) => new Dimension(a._t * exp, a._l * exp, a._m * exp, a._i * exp, a._th * exp, a._n * exp, a._j * exp, a._a * exp);

    #region Equality members

    /// <inheritdoc />
    public bool Equals(Dimension other)
    {
        return _t == other._t && _l == other._l && _m == other._m && _i == other._i && _th == other._th && _n == other._n && _j == other._j && _a == other._a;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Dimension other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(_t, _l, _m, _i, _th, _n, _j, _a);

    /// <summary>Determines whether two <see cref="Dimension" /> instances are equal.</summary>
    public static bool operator ==(Dimension left, Dimension right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="Dimension" /> instances are not equal.</summary>
    public static bool operator !=(Dimension left, Dimension right) => !left.Equals(right);

    #endregion

    #region ToString

    /// <summary>Implicitly converts a <see cref="Dimension" /> to its string representation.</summary>
    public static implicit operator string(Dimension dim) => dim.ToString();

    /// <summary>
    /// Returns a human-readable string showing the dimensional formula using SI base-unit
    /// symbols with Unicode superscript exponents (e.g. <c>kg m s⁻²</c>).
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();

        AppendIfNonZero(sb, "s", _t, true);
        AppendIfNonZero(sb, "m", _l, true);
        AppendIfNonZero(sb, "kg", _m, true);
        AppendIfNonZero(sb, "A", _i, true);
        AppendIfNonZero(sb, "K", _th, true);
        AppendIfNonZero(sb, "mol", _n, true);
        AppendIfNonZero(sb, "cd", _j, true);
        AppendIfNonZero(sb, "°", _a, true);

        AppendIfNonZero(sb, "s", _t, false);
        AppendIfNonZero(sb, "m", _l, false);
        AppendIfNonZero(sb, "kg", _m, false);
        AppendIfNonZero(sb, "A", _i, false);
        AppendIfNonZero(sb, "K", _th, false);
        AppendIfNonZero(sb, "mol", _n, false);
        AppendIfNonZero(sb, "cd", _j, false);
        AppendIfNonZero(sb, "°", _a, false);

        return sb.ToString();
    }

    private static void AppendIfNonZero(StringBuilder sb, string symbol, int power, bool positive)
    {
        if (positive ? power <= 0 : power >= 0)
            return;

        if (sb.Length > 0)
            sb.Append(' ');

        sb.Append(PowerOf(symbol, power));
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

    /// <summary>Dimensionless quantity (all exponents zero).</summary>
    public static readonly Dimension Dimensionless = new Dimension(0);

    /// <summary>Time dimension [T].</summary>
    public static readonly Dimension Second = new Dimension(1);

    /// <summary>Length dimension [L].</summary>
    public static readonly Dimension Meter = new Dimension(0, 1);

    /// <summary>Mass dimension [M].</summary>
    public static readonly Dimension Kilogram = new Dimension(0, 0, 1);

    /// <summary>Electric current dimension [I].</summary>
    public static readonly Dimension Ampere = new Dimension(0, 0, 0, 1);

    /// <summary>Thermodynamic temperature dimension [Θ].</summary>
    public static readonly Dimension Kelvin = new Dimension(0, 0, 0, 0, 1);

    /// <summary>Amount of substance dimension [N].</summary>
    public static readonly Dimension Mol = new Dimension(0, 0, 0, 0, 0, 1);

    /// <summary>Luminous intensity dimension [J].</summary>
    public static readonly Dimension Candela = new Dimension(0, 0, 0, 0, 0, 0, 1);

    /// <summary>Plane angle dimension.</summary>
    public static readonly Dimension Angle = new Dimension(0, 0, 0, 0, 0, 0, 0, 1);

    #endregion
}
