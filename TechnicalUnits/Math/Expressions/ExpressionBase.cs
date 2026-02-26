using System;

namespace TechnicalUnits.Math.Expressions;

/// <summary>
/// Abstract base class for all expression nodes in the evaluation tree.
/// </summary>
public abstract class ExpressionBase
{
    /// <summary>Gets the number of arguments this expression consumes from the evaluation stack.</summary>
    public abstract int ArgumentCount { get; }

    /// <summary>
    /// Evaluates the expression with the given argument values.
    /// </summary>
    /// <param name="values">
    /// An array whose length must equal <see cref="ArgumentCount" />.
    /// Values are supplied in left-to-right order.
    /// </param>
    /// <returns>The result of the evaluation.</returns>
    public abstract double Evaluate(double[] values);

    /// <summary>
    /// Validates that <paramref name="values" /> is non-null and has the expected <see cref="ArgumentCount" />.
    /// </summary>
    /// <param name="values">The argument array to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="values" /> length does not match <see cref="ArgumentCount" />.</exception>
    protected void ValidateArguments(double[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length != ArgumentCount)
            throw new ArgumentException("Invalid number of arguments.", nameof(values));
    }
}
