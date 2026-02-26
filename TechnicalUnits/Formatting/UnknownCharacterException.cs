using System;

namespace TechnicalUnits.Formatting;

/// <summary>
/// The exception that is raised as a warning when the parser encounters an unrecognised character.
/// </summary>
public class UnknownCharacterException : Exception
{
    /// <summary>
    /// Initializes a new instance with the specified message and the offending character.
    /// </summary>
    /// <param name="message">A human-readable description of the warning.</param>
    /// <param name="ch">The unrecognised character.</param>
    public UnknownCharacterException(string message, char ch)
        : base(message)
    {
        Char = ch;
    }

    /// <summary>Gets the character that could not be identified.</summary>
    public char Char { get; }
}
