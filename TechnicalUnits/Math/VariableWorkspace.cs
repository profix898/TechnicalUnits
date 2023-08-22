using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TechnicalUnits.Math;

public class VariableWorkspace : Dictionary<string, double>
{
    #region Variables

    public const string AnswerVariable = "ans";

    #endregion

    private readonly MathEvaluator evaluator;

    internal VariableWorkspace(MathEvaluator evaluator)
        : base(StringComparer.OrdinalIgnoreCase)
    {
        this.evaluator = evaluator;

        base.Add(AnswerVariable, 0.0);
        base.Add("pi", System.Math.PI);
        base.Add("e", System.Math.E);
    }

    public new void Add(string name, double value)
    {
        if (String.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (evaluator.IsFunction(name))
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Variable name conflicts with function '{0}'.", name), nameof(name));
        if (name.Any(ch => !Char.IsLetter(ch)))
            throw new ArgumentException("Variable name must contain letters only.", nameof(name));

        base.Add(name, value);
    }
}
