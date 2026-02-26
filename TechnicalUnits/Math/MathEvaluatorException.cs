using System;

namespace TechnicalUnits.Math;

/// <summary>
/// The exception that is thrown when a mathematical expression cannot be parsed or evaluated.
/// </summary>
public class MathEvaluatorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MathEvaluatorException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MathEvaluatorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MathEvaluatorException" /> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MathEvaluatorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
