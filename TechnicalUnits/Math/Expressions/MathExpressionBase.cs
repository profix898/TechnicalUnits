using System;

namespace TechnicalUnits.Math.Expressions;

public abstract class MathExpressionBase
{
    public abstract int ArgumentCount { get; }

    public abstract double Evaluate(double[] values);

    protected void ValidateArguments(double[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length != ArgumentCount)
            throw new ArgumentException("Invalid number of arguments.", nameof(values));
    }
}
