using System;

namespace TechnicalUnits.Math;

public class MathEvaluatorException : Exception
{
    public MathEvaluatorException(string message)
        : base(message)
    {
    }

    public MathEvaluatorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
