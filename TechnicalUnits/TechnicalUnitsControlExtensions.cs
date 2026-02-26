using System;
using System.Collections.Generic;
using TechnicalUnits.Formatting;
using TechnicalUnits.Math;

namespace TechnicalUnits;

/// <summary>
/// Provides extension methods for <see cref="ITechnicalUnitsControl" /> for value/text conversion and validation.
/// </summary>
public static class TechnicalUnitsControlExtensions
{
    private static readonly Lazy<MathEvaluator> _mathEval = new Lazy<MathEvaluator>(() => new MathEvaluator());

    /// <summary>Converts a value to its formatted text representation.</summary>
    public static string ConvertValueToText(this ITechnicalUnitsControl control, object value)
    {
        if (value is not double doubleValue)
            return String.Empty;

        return Formatter.Format(doubleValue, control.UnitOptions, control.FormattingOptions);
    }

    /// <summary>Converts a value to its formatted text representation.</summary>
    public static string ConvertValueToText(this ITechnicalUnitsControl control, double value) => Formatter.Format(value, control.UnitOptions, control.FormattingOptions);

    /// <summary>Converts a text representation to its corresponding value.</summary>
    /// <remarks>On failure to convert text to value, the function does not throw exceptions,
    /// but returns a list of errors and potentially a null value. The caller is responsible
    /// for deciding on an appropriate action or UI response.</remarks>
    public static double? ConvertTextToValue(this ITechnicalUnitsControl control, object value, List<Exception>? errors = null)
    {
        if (value is not string text)
            return 0.0;

        return control.ConvertTextToValue(text, errors);
    }

    /// <summary>Converts a text representation to its corresponding value.</summary>
    /// <remarks>On failure to convert text to value, the function does not throw exceptions,
    /// but returns a list of errors and potentially a null value. The caller is responsible
    /// for deciding on an appropriate action or UI response.</remarks>
    public static double? ConvertTextToValue(this ITechnicalUnitsControl control, string? text, List<Exception>? errors = null)
    {
        if (String.IsNullOrEmpty(text))
            return null;

        try
        {
            double result;
            if (control.EnableMath)
                result = _mathEval.Value.Evaluate(text, control.UnitOptions, control.FormattingOptions, errors);
            else
                result = Parser.ParseString(text, control.UnitOptions, control.FormattingOptions, errors);

            return control.ValidateLimits(result, errors);
        }
        catch (Exception ex)
        {
            errors?.Add(ex);
            return null;
        }
    }

    /// <summary>Validates the specified value against the control's minimum and maximum limits.</summary>
    /// <returns>The validated value, possibly clamped to the minimum or maximum if out of bounds.
    /// If <see cref="ITechnicalUnitsControl.ClipValueToMinMax" /> is true, the value is clamped to the range.
    /// Otherwise, if the value is out of bounds, an <see cref="ArgumentOutOfRangeException" /> is added to <paramref name="errors" />,
    /// and the minimum or maximum value is returned as appropriate.</returns>
    public static double ValidateLimits(this ITechnicalUnitsControl control, double value, List<Exception>? errors = null)
    {
        if (control.ClipValueToMinMax)
            return System.Math.Clamp(value, control.Minimum, control.Maximum);

        if (value < control.Minimum)
        {
            errors?.Add(new ArgumentOutOfRangeException(nameof(value), $"Value {value:e} out of lower bounds [{control.Minimum:e}; {control.Maximum:e}]."));
            return control.Minimum;
        }

        if (value > control.Maximum)
        {
            errors?.Add(new ArgumentOutOfRangeException(nameof(value), $"Value {value:e} out of upper bounds [{control.Minimum:e}; {control.Maximum:e}]."));
            return control.Maximum;
        }

        return value;
    }
}
