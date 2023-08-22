using System;

namespace TechnicalUnits.Formatting;

public class UnknownCharacterException : Exception
{
    public UnknownCharacterException(string message, char ch)
        : base(message)
    {
        Char = ch;
    }

    public char Char { get; private set; }
}
