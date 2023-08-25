using System;
using TechnicalUnits.Units;

namespace TechnicalUnits.Math.Expressions;

public sealed class ConvertExpression : ExpressionBase
{
    public ConvertExpression(string expression)
    {
        Expression = expression;
    }

    public string Expression { get; }

    public override int ArgumentCount => 1;

    public override double Evaluate(double[] values)
    {
        ValidateArguments(values);

        throw new NotImplementedException();
    }

    #region Internal

    public static (DerivedUnit, Unit) ParseConvertExpression()
    {
        // Expression format: [{0}>{1}]
        throw new NotImplementedException();
    }
    
    public static bool IsConvertExpression(string expression)
    {
        if (String.IsNullOrEmpty(expression))
            return false;

        if (!expression.StartsWith('['))
            return false;

        if (!expression.EndsWith(']'))
            return false;

        if (!expression.Contains('>'))
            return false;

        return true;
    }

    #endregion

    public override string ToString()
    {
        return Expression;
    }
}
