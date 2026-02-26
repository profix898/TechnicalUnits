using System;

namespace TechnicalUnits.Formatting;

/// <summary>
/// The exception that is raised as a warning when the parser encounters unexpected syntax.
/// </summary>
public class UnexpectedSyntaxException : Exception
{
    /// <summary>
    /// Initializes a new instance with the parser state, message, and the unexpected token.
    /// </summary>
    /// <param name="message">A human-readable description of the problem.</param>
    /// <param name="part">The parser state in which the problem occurred.</param>
    /// <param name="unexpectedString">The string fragment that was unexpected.</param>
    public UnexpectedSyntaxException(string message, ParserPartEnum part, string unexpectedString)
        : base(message)
    {
        Part = part;
        UnexpectedString = unexpectedString;
    }

    /// <summary>Gets the parser state at the time of the error.</summary>
    public ParserPartEnum Part { get; }

    /// <summary>Gets the unexpected string fragment.</summary>
    public string UnexpectedString { get; }
}
