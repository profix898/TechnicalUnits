using System;
using System.Collections.Generic;

namespace TechnicalUnits;

/// <summary>
/// Implementation interface for a technical units control, extending <see cref="ITechnicalUnitsControl"/>.
/// </summary>
public interface ITechnicalUnitsControlImpl : ITechnicalUnitsControl
{
    /// <summary>
    /// Sets the value of the control, updates the control's text, and displays any validation errors.
    /// </summary>
    /// <param name="value">The value to set on the control.</param>
    /// <param name="errors">A list of exceptions representing validation or conversion errors.</param>
    void SetValue(double value, List<Exception>? errors = null);

    /// <summary>
    /// Updates the control's text representation internally from the value (without re-parsing).
    /// </summary>
    void UpdateText();
}
