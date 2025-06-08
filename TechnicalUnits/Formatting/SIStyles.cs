namespace TechnicalUnits.Formatting;

/// <summary>
/// Specifies the style to use when formatting.
/// </summary>
public enum SIStyles
{
    /// <summary>
    /// Use SI symbols (e.g., "m", "kg", "s") and SI prefixes (e.g., "µ", "k", "M").
    /// </summary>
    SIStyleSI,

    /// <summary>
    /// Use SI symbols (e"m", "kg", "s") and English names for SI prefixes (e.g., "micro", "kilo", "mega").
    /// </summary>
    SIStyleSINamesEN,

    /// <summary>
    /// Use SI symbols (e.g., "m", "kg", "s") and safe (ASCII) SI prefixes (e.g., "u", "k", "M").
    /// </summary>
    SIStyleSIAz,

    /// <summary>
    /// Use floating-point representation for values, without unit symbols.
    /// </summary>
    SIStyleFloat
}
