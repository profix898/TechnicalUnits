using System;
using System.Collections.Generic;
using TechnicalUnits.Internal;

namespace TechnicalUnits;

/// <summary>
/// Provides shared functionality for technical units controls, including value conversion,
/// validation, and keyboard/button handling for increment/decrement operations.
/// </summary>
public class TechnicalUnitsControlMixin
{
    /// <summary>
    /// The epsilon value used for approximate floating-point comparisons.
    /// </summary>
    public const double ApproxEpsilon = 0.01;

    private readonly ITechnicalUnitsControlImpl _control;

    public TechnicalUnitsControlMixin(ITechnicalUnitsControlImpl control)
    {
        _control = control;
    }

    #region TextValueConversion
    
    /// <summary>Converts a value to its formatted text representation.</summary>
    public string ConvertValueToText(double value)
    {
        return _control.ConvertValueToText(value);
    }

    /// <summary>Converts a text representation to its corresponding value.</summary>
    /// <remarks>On failure to convert text to value, the function does not throw exceptions, 
    /// but returns a list of errors and potentially a null value. The caller is responsible 
    /// for deciding on an appropriate action or UI response.</remarks>
    public double? ConvertTextToValue(string? text, List<Exception>? errors = null)
    {
        return _control.ConvertTextToValue(text, errors);
    }
    
    /// <summary>Validates the specified value against the control's minimum and maximum limits.</summary>
    /// <returns>The validated value, possibly clamped to the minimum or maximum if out of bounds.
    /// If <see cref="ITechnicalUnitsControl.ClipValueToMinMax"/> is true, the value is clamped to the range.
    /// Otherwise, if the value is out of bounds, an <see cref="ArgumentOutOfRangeException"/> is added to <paramref name="errors"/>,
    /// and the minimum or maximum value is returned as appropriate.</returns>
    public double ValidateLimits(double value, List<Exception>? errors = null)
    {
        return _control.ValidateLimits(value, errors);
    }

    /// <summary>Attempts to convert the current control text to a value and set it on the control.</summary>
    /// <remarks>Parses the text in the control, converts it to a value using the current formatting and
    /// unit options, and sets the value on the control. Any parsing or validation errors are
    /// collected and passed to the control. If conversion fails, the current value is retained.</remarks>
    public void TryUpdateValue()
    {
        try
        {
            var errors = new List<Exception>();
            var value = ConvertTextToValue(_control.Text, errors) ?? _control.Value;
            _control.SetValue(value, errors);
        }
        catch (Exception)
        {
            _control.SetValue(_control.Value, []);
        }
    }

    #endregion

    #region UpDownButton

    /// <summary>
    /// Handles the up button press, incrementing the value based on current key modifiers.
    /// </summary>
    /// <remarks>
    /// The increment behavior varies based on modifier keys:
    /// <list type="bullet">
    /// <item><description>No modifier: Increments by <see cref="ITechnicalUnitsControl.Increment"/>.</description></item>
    /// <item><description>Shift: Multiplies by <see cref="ITechnicalUnitsControl.IncrementMult"/>.</description></item>
    /// <item><description>Alt: Increments at the position before the relative decimal separator.</description></item>
    /// <item><description>Shift+Alt: Increments at the highest significant position.</description></item>
    /// </list>
    /// </remarks>
    public void OnUpButton()
    {
        var errors = new List<Exception>();
        var value = _control.Value; // Store the current value

        if (_control.IsReadOnly)
            return;

        // Convert text to value, but do not update the control yet
        // This is necessary to ensure that the value is valid before we apply the increment
        if (!String.IsNullOrEmpty(_control.Text) && _control.Text != ConvertValueToText(value))
            value = ConvertTextToValue(_control.Text, errors) ?? value;

        if (value < 0)
        {
            // Complex case, where we have to handle the possibility that the decrement step will
            // result in values very close to zero, which are not desirable for the modifiers
            if (keyModifiers.HasFlag(KeyModifiers.Shift) && keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                if (value != 0)
                {
                    var exp = System.Math.Floor(System.Math.Log10(System.Math.Abs(value))); // Get the exponent of 10 for the value
                    var dynamicIncrement = System.Math.Pow(10, exp); // Calculate the value of the highest position

                    if (MathHelper.NearlyEqual(dynamicIncrement, -1 * value, ApproxEpsilon)) // Next value would be zero or very close to it
                        dynamicIncrement = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(value / 10))));
                    value += dynamicIncrement;
                }
                else
                    value += _control.Increment;
            }
            else if (keyModifiers.HasFlag(KeyModifiers.Shift))
                value *= _control.IncrementMult;
            else if (keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                double exp3 = MathHelper.GetExp3Value(value);
                var dynamicIncrement = System.Math.Pow(10, 3 * exp3);

                // Decrement at the position before the decimal separator (ignoring the SI prefix following,
                // i.e. we use the relative decimal separator, not the absolute including the SI prefix)
                var valueBeforeRelativeDecSep = System.Math.Floor(value / System.Math.Pow(1000, exp3));
                if (MathHelper.NearlyEqual(dynamicIncrement, -1 * value, ApproxEpsilon) // Next value would be zero or very close to it
                    || MathHelper.NearlyEqual(valueBeforeRelativeDecSep, -1, ApproxEpsilon)) // The position before the decimal separator is 1
                    value += dynamicIncrement / 10;
                else
                    value += dynamicIncrement;
            }
            else
                value += _control.Increment;
        }
        else
        {
            // value > 0: Simple direction for incrementing
            if (keyModifiers.HasFlag(KeyModifiers.Shift) && keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                if (value != 0)
                {
                    var exp = (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(value))); // Get exponent of 10 for value
                    value += System.Math.Pow(10, exp); // Increase value by the highest position value
                }
                else
                    value += _control.Increment; // Zero is an exception, since there is no position with any value.
                // Use the increment for this case instead.
            }
            else if (keyModifiers.HasFlag(KeyModifiers.Shift))
                value *= _control.IncrementMult; // Use the multiplicative increment
            else if (keyModifiers.HasFlag(KeyModifiers.Alt))
                value += System.Math.Pow(10, 3 * MathHelper.GetExp3Value(value));
            // _control.Increment the position before the decimal separator (ignoring the SI prefix following,
            // i.e. we use the relative decimal separator, not the absolute including the SI prefix)
            else
                value += _control.Increment; // Normal incrementing
        }

        value = ValidateLimits(value, errors); // Set the value (and update the control text)
            _control.SetValue(value, errors);
        }

        /// <summary>
        /// Handles the down button press, decrementing the value based on current key modifiers.
        /// </summary>
        /// <remarks>
        /// The decrement behavior varies based on modifier keys:
        /// <list type="bullet">
        /// <item><description>No modifier: Decrements by <see cref="ITechnicalUnitsControl.Increment"/>.</description></item>
        /// <item><description>Shift: Divides by <see cref="ITechnicalUnitsControl.IncrementMult"/>.</description></item>
        /// <item><description>Alt: Decrements at the position before the relative decimal separator.</description></item>
        /// <item><description>Shift+Alt: Decrements at the highest significant position.</description></item>
        /// </list>
        /// </remarks>
        public void OnDownButton()
    {
        var errors = new List<Exception>();
        var value = _control.Value; // Store the current value

        if (_control.IsReadOnly)
            return;

        // Convert text to value, but do not update the control yet
        // This is necessary to ensure that the value is valid before we apply the increment
        if (!String.IsNullOrEmpty(_control.Text) && _control.Text != ConvertValueToText(value))
            value = ConvertTextToValue(_control.Text, errors) ?? value;

        if (value > 0)
        {
            // Complex case, where we have to handle the possibility that the decrement step will
            // result in values very close to zero, which are not desirable for the modifiers
            if (keyModifiers.HasFlag(KeyModifiers.Shift) && keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                if (value != 0)
                {
                    var exp = System.Math.Floor(System.Math.Log10(System.Math.Abs(value))); // Get the exponent of 10 for the value
                    var dynamicDecrement = System.Math.Pow(10, exp); // Calculate the value of the highest position
                    if (MathHelper.NearlyEqual(dynamicDecrement, value, ApproxEpsilon)) // Next value would be zero or very close to it
                        dynamicDecrement = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(value / 10))));
                    value -= dynamicDecrement;
                }
                else
                {
                    // Zero is an exception, since there is no position with any value.
                    // Use the increment for this case instead.
                    value -= _control.Increment;
                }
            }
            else if (keyModifiers.HasFlag(KeyModifiers.Shift))
                value /= _control.IncrementMult; // Use the multiplicative increment
            else if (keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                // Decrement at the position before the decimal separator (ignoring the SI prefix following,
                // i.e. we use the relative decimal separator, not the absolute including the SI prefix)
                double exp3 = MathHelper.GetExp3Value(value);
                var dynamicDecrement = System.Math.Pow(10, 3 * exp3);
                var valueBeforeRelativeDecSep = System.Math.Floor(value / System.Math.Pow(1000, exp3));
                if (MathHelper.NearlyEqual(dynamicDecrement, value, ApproxEpsilon)
                    || MathHelper.NearlyEqual(valueBeforeRelativeDecSep, 1, ApproxEpsilon)) // Next value would be zero or very close to it
                    value -= dynamicDecrement / 10;
                else
                    value -= dynamicDecrement;
            }
            else
                value -= _control.Increment; // Normal decrementing
        }
        else
        {
            // value < 0: Simple direction for decrementing
            if (keyModifiers.HasFlag(KeyModifiers.Shift) && keyModifiers.HasFlag(KeyModifiers.Alt))
            {
                if (value != 0)
                {
                    var exp = (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(value))); // Get exponent of 10 for value
                    value -= System.Math.Pow(10, exp); // Increase value by the highest position value
                }
                else
                {
                    // Zero is an exception, since there is no position with any value.
                    // Use the increment for this case instead.
                    value -= _control.Increment;
                }
            }
            else if (keyModifiers.HasFlag(KeyModifiers.Shift))
                value /= _control.IncrementMult; // Use the multiplicative increment
            else if (keyModifiers.HasFlag(KeyModifiers.Alt))
                // Increment the position before the decimal separator (ignoring the SI prefix following,
                // i.e. we use the relative decimal separator, not the absolute including the SI prefix)
                value -= System.Math.Pow(10, 3 * MathHelper.GetExp3Value(value));
            else
                value -= _control.Increment; // Normal incrementing
        }

        value = ValidateLimits(value, errors); // Set the value (and update the control text)
        _control.SetValue(value, errors);
    }

    #endregion

    #region Keyboard

    private KeyModifiers keyModifiers;

    /// <summary>
    /// Handles key down events for the control.
    /// </summary>
    /// <param name="key">The key that was pressed, or <c>null</c> if only modifiers changed.</param>
    /// <param name="keyModifiers">The current state of modifier keys.</param>
    /// <returns><c>true</c> if the key event was handled; otherwise, <c>false</c>.</returns>
    public bool OnKeyDown(Keys? key, KeyModifiers keyModifiers)
    {
        this.keyModifiers |= keyModifiers;

        if (key == null)
            return false;

        if (key == Keys.Enter)
        {
            TryUpdateValue();
            return true;
        }

        if (this.keyModifiers.HasFlag(KeyModifiers.Shift)
            || this.keyModifiers.HasFlag(KeyModifiers.Alt)
            || this.keyModifiers.HasFlag(KeyModifiers.Shift | KeyModifiers.Alt))
        {
            if (key == Keys.Up)
            {
                OnUpButton();
                return true;
            }

            if (key == Keys.Down)
            {
                OnDownButton();
                return true;
            }
        }

        // Ctrl+M: Toggle math mode
        if (this.keyModifiers.HasFlag(KeyModifiers.Ctrl) && key == Keys.M)
        {
            _control.EnableMath = !_control.EnableMath;
            TryUpdateValue();
            return true;
        }

        return false;
        }

        /// <summary>
        /// Handles key up events for the control.
        /// </summary>
        /// <param name="keyModifiers">The modifier keys that were released.</param>
        /// <returns><c>true</c> if the key event was handled; otherwise, <c>false</c>.</returns>
        public bool OnKeyUp(KeyModifiers keyModifiers)
        {
            this.keyModifiers &= ~keyModifiers;

        return false;
    }

    #endregion
}
