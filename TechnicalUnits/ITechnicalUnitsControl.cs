using System;
using TechnicalUnits.Formatting;

namespace TechnicalUnits;

/// <summary>
/// Interface for a technical units control that provides functionality for handling technical values with units, formatting, and validation.
/// </summary>
public interface ITechnicalUnitsControl
{
    #region Options

    /// <summary>
    /// Specifies the formatting options and culture info for parsing and displaying values.
    /// </summary>
    FormattingOptions FormattingOptions { get; set; }

    /// <summary>
    /// Specifies the unit options for parsing and displaying values.
    /// </summary>
    UnitOptions UnitOptions { get; set; }

    /// <summary>
    /// Indicates whether math expressions are enabled.
    /// </summary>
    bool EnableMath { get; set; }

    #endregion

    #region MinMax

    /// <summary>
    /// Specifies the minimum allowed value (set 'EnableLimits' option to enforce).
    /// </summary>
    double Minimum { get; set; }

    /// <summary>
    /// Specifies the maximum allowed value (set 'EnableLimits' option to enforce).
    /// </summary>
    double Maximum { get; set; }

    /// <summary>
    /// Indicates whether to clip the value to the min/max bounds.
    /// </summary>
    bool ClipValueToMinMax { get; set; }

    #endregion

    #region UpDown

    /// <summary>
    /// Specifies the increment value for up/down operations. Determines the amount by which the value is increased/decreased when the up/down button is pressed.
    /// </summary>
    double Increment { get; set; }

    /// <summary>
    /// Specifies the multiplicative increment for value on up/down. Determines the amount by which the value is multiplied/divided when the up/down button is pressed (while the Shift key is pressed).
    /// </summary>
    double IncrementMult { get; set; }

    #endregion

    #region ValueText

    /// <summary>
    /// Indicates whether the control is read-only. When set to true, the value cannot be edited by the user.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Specifies the (current) numeric value of the control.
    /// </summary>
    double Value { get; set; }

    /// <summary>
    /// Gets the formatted text representation of the current value.
    /// </summary>
    string Text { get; }

    #endregion
}
