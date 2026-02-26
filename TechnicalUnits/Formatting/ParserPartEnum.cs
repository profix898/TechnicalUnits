namespace TechnicalUnits.Formatting;

/// <summary>
/// Represents the current state of the parser's state machine during value/unit parsing.
/// </summary>
public enum ParserPartEnum
{
    /// <summary>Initial state — looking for the start of a number.</summary>
    PreDecPart = 0,

    /// <summary>Reading the sign character(s) before the integer part.</summary>
    PreDecSign,

    /// <summary>Reading the integer (pre-decimal) digits.</summary>
    PreDecNum,

    /// <summary>Looking for a decimal separator, SI prefix, or unit after the integer part.</summary>
    DecSepPart,

    /// <summary>After the decimal separator — looking for fractional digits or SI prefix.</summary>
    PostDecPart,

    /// <summary>Reading the fractional (post-decimal) digits.</summary>
    PostDecNum,

    /// <summary>After fractional digits — looking for SI prefix, exponent, or unit.</summary>
    PostPostDecPart,

    /// <summary>Start of an exponent section (e.g. after 'e' or 'E').</summary>
    ExpPart,

    /// <summary>Reading the exponent sign.</summary>
    ExpSign,

    /// <summary>Reading the exponent digits.</summary>
    ExpNum,

    /// <summary>After the exponent — looking for trailing SI prefix or unit.</summary>
    SuffixPart,

    /// <summary>Reading the unit symbol.</summary>
    UnitPart,

    /// <summary>End of input reached.</summary>
    EndOfStrPart
}
