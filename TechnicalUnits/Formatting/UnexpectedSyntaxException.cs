using System;

namespace TechnicalUnits.Formatting;

public class UnexpectedSyntaxException : Exception
{
    public UnexpectedSyntaxException(string message, ParserPartEnum part, string unexpectedString)
        : base(message)
    {
        Part = part;
        UnexpectedString = unexpectedString;
    }

    public ParserPartEnum Part { get; }

    public string UnexpectedString { get; }
}
