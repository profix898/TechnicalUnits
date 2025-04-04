using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TechnicalUnits.Formatting;
using TechnicalUnits.Math;
using TechnicalUnits.WinForms.Properties;
using static System.Math;
using static TechnicalUnits.Internal.MathUtility;

namespace TechnicalUnits.WinForms;

public sealed class EngineeringUpDown : UpDownBase, IDataGridViewEditingControl, ISupportInitialize
{
    private const double ApproxEpsilon = 0.01;

    private readonly ToolTip toolTipInfo = new ToolTip();

    private readonly FormattingOptions formattingOptions = new FormattingOptions();
    private readonly UnitOptions unitOptions = new UnitOptions();

    private readonly TextBox upDownTextbox;
    private readonly Graphics textBoxGraphics;
    private readonly Icon mathIcon = Resources.MathModeIcon;

    private bool tooltipOverride;
    private string lastToolTipStr;
    private int previousTooltipDuration;
    private MathEvaluator? mathEval;
    private double value;

    public EngineeringUpDown()
        : this(null)
    {
        Icon icon1 = new Icon(Resources.MathModeIcon, 40, 40);
    }

    public EngineeringUpDown(IContainer container)
    {
        Increment = 1.0;
        IncrementMult = 10.0;
        EnableLimits = true;
        Minimum = Double.MinValue;
        Maximum = Double.MaxValue;
        Value = 1000;
        WarningToolTipDuration = 7000;
        upDownTextbox = Controls[1] as TextBox;
        textBoxGraphics = upDownTextbox.CreateGraphics();

        container?.Add(this);
    }

    #region Properties

    [Category("MathEvaluation")]
    [Description("EnableMath")]
    [DefaultValue(false)]
    public bool EnableMath { get; set; }

    [Category("EngineeringNotation")]
    [Description("Increment")]
    [DefaultValue(1.0)]
    public double Increment { get; set; }

    [Category("EngineeringNotation")]
    [Description("IncrementMult")]
    [DefaultValue(10.0)]
    public double IncrementMult { get; set; }

    [Category("EngineeringNotation")]
    [Description("Validates value within Minimum/Maximum bounds.")]
    [DefaultValue(true)]
    public bool EnableLimits { get; set; }

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Parser and Display options")]
    public FormattingOptions Options
    {
        get { return unitOptions.Options; }
        set
        {
            unitOptions.Options = value;

            UpdateEditText();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Minimum value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MinValue)]
    public double Minimum { get; set; }

    [Category("EngineeringNotation")]
    [Description("Maximum value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MaxValue)]
    public double Maximum { get; set; }

    [Category("EngineeringNotation")]
    [Description("Value")]
    [DefaultValue(1.0)]
    public double Value
    {
        get
        {
            if (UserEdit)
                ValidateEditText();

            return value;
        }

        set
        {
            if (Abs(value - this.value) < Double.Epsilon)
                return;

            if (!initializing && EnableLimits)
            {
                if (value < Minimum)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Value out of lower bounds [{Minimum};{Maximum}].");
                if (value > Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Value out of upper bounds [{Minimum};{Maximum}].");
            }

            this.value = value;
            Exp3 = GetExp3Value(value);
            Text = DoubeToEngString(value);
        }
    }

    public string DoubeToEngString(double val)
    { // conversion method with current options and no side effects
        string str = "";
        str = unitOptions.NumberToString(val);
        return str;
    }

    public event EventHandler ValueChanged;

    /// <summary>
    ///     GetExp3Value: Get the exponent to base 1000 of the argument as integer rounded towards neg.infinity.
    ///     Returns 0 for 0 as input.
    ///     Returns 1 for 1000 or 2000 as input
    /// </summary>
    [Category("EngineeringNotation")]
    [Description("Exponent to base 1000")]
    [DefaultValue(1.0)]
    public int Exp3
    {
        get { return GetExp3Value(value); }

        set
        {
            var exp3 = GetExp3Value(this.value);
            if (value != exp3)
                this.value *= Math.Pow(10, 3 * (value - exp3));
        }
    }

    [Category("EngineeringUpDown")]
    [Description("Duration of error ToolTips in milliseconds, 0 to disable them.")]
    [DefaultValue(7000)]
    public int WarningToolTipDuration { get; set; } // Duration of the tooltip in ms

    #endregion

    #region Overrides

    [ReadOnly(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get { return base.Text; }
        set { base.Text = value; }
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                toolTipInfo.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    #region KeysButtonsWheel

    public override void UpButton()
    {
        var warnings = new List<Exception>();

        if (ReadOnly)
        {
            return;
        }

        if (UserEdit)
            ParseEditText(warnings);

        if (value < 0)
        {
            // Complex case, where we have to handle the possibility that the decrement step will result in values very close to zero, which are not desirable for the modifiers
            if (ModifierKeys == (Keys.Shift | Keys.Alt))
            {
                if (value != 0)
                {
                    var exp1 = Floor(Log10(Abs(value))); // Get the exponent of 10 for the value
                    var dynamicIncrement = Pow(10, exp1); // Calculate the value of the highest position

                    // Next value would be zero or very close to it
                    if (NearlyEqual(dynamicIncrement, -1 * value, ApproxEpsilon))
                        dynamicIncrement = Pow(10, Floor(Log10(Abs(value / 10))));
                    value += dynamicIncrement;
                }
                else
                {
                    value += Increment;
                }
            }
            else if (ModifierKeys == Keys.Shift)
            {
                value *= IncrementMult;
            }
            else if (ModifierKeys == Keys.Alt)
            {
                double exp3 = GetExp3Value(value);
                var dynamicIncrement = Pow(10, 3 * exp3);
                // Decrement at the position before the decimal separator (ignoring the SI prefix following, i.e. we use the relative decimal separator, not the absolute including the SI prefix)

                var valueBeforeRelativeDecSep = Floor(value / Pow(1000, exp3));

                if (NearlyEqual(dynamicIncrement, -1 * value, ApproxEpsilon) // Next value would be zero or very close to it
                    || NearlyEqual(valueBeforeRelativeDecSep, -1, ApproxEpsilon) // The position before the decimal separator is 1
                   )
                    value += dynamicIncrement / 10;
                else
                    value += dynamicIncrement;
            }
            else
            {
                value += Increment;
            }
        }
        else
        {
            // value > 0: Simple direction for incrementing
            if (UserEdit)
                ParseEditText(warnings);
            if (ModifierKeys == (Keys.Shift | Keys.Alt))
                if (value != 0)
                {
                    var exp1 = (int)Floor(Log10(Abs(value))); // Get exponent of 10 for value
                    value += Pow(10, exp1); // Increase value by the highest position value
                }
                else
                {
                    value += Increment; // Zero is an exception, since there is no position with any value. Use the increment for this case instead.
                }
            else if (ModifierKeys == Keys.Shift)
                value *= IncrementMult; // Use the multiplicative increment
            else if (ModifierKeys == Keys.Alt)
                value += Pow(10, 3 * GetExp3Value(value));
            // Increment the position before the decimal separator (ignoring the SI prefix following, i.e. we use the relative decimal separator, not the absolute including the SI prefix)
            else
                value += Increment; // Normal incrementing
        }

        value = ValidateLimits(value, warnings);
        Exp3 = GetExp3Value(value);
        Text = Formatter.Format(value, unitOptions, formattingOptions);

        ShowTooltip(warnings);
    }

    public override void DownButton()
    {
        var warnings = new List<Exception>();

        if (ReadOnly)
        {
            return;
        }

        if (UserEdit)
            ParseEditText(warnings);

        if (value > 0)
        {
            // Complex case, where we have to handle the possibility that the decrement step will result in values very close to zero, which are not desirable for the modifiers
            if (ModifierKeys == (Keys.Shift | Keys.Alt))
            {
                if (value != 0)
                {
                    var exp1 = Floor(Log10(Abs(value))); // Get the exponent of 10 for the value
                    var dynamicDecrement = Pow(10, exp1); // Calculate the value of the highest position

                    if (NearlyEqual(dynamicDecrement, value, ApproxEpsilon))
                        // Next value would be zero or very close to it
                        dynamicDecrement = Pow(10, Floor(Log10(Abs(value / 10))));
                    value -= dynamicDecrement;
                }
                else
                {
                    value -= Increment;
                }
            }
            else if (ModifierKeys == Keys.Shift)
            {
                value /= IncrementMult;
            }
            else if (ModifierKeys == Keys.Alt)
            {
                double exp3 = GetExp3Value(value);
                var dynamicDecrement = Pow(10, 3 * exp3);
                // Decrement at the position before the decimal separator (ignoring the SI prefix following, i.e. we use the relative decimal separator, not the absolute including the SI prefix)

                var valueBeforeRelativeDecSep = Floor(value / Pow(1000, exp3));

                if (NearlyEqual(dynamicDecrement, value, ApproxEpsilon)
                    // Next value would be zero or very close to it
                    || NearlyEqual(valueBeforeRelativeDecSep, 1, ApproxEpsilon)
                    // The position before the decimal separator is 1
                   )
                    value -= dynamicDecrement / 10;
                else
                    value -= dynamicDecrement;
            }
            else
            {
                value -= Increment;
            }
        }
        else
        {
            // value < 0: Simple direction for decrementing
            if (ModifierKeys == (Keys.Shift | Keys.Alt))
                if (value != 0)
                {
                    var exp = (int)Floor(Log10(Abs(value))); // Get exponent of 10 for value
                    value -= Pow(10, exp); // Increase value by the highest position value
                }
                else
                {
                    value -= Increment; // Zero is an exception, since there is no position with any value. Use the increment for this case instead.
                }
            else if (ModifierKeys == Keys.Shift)
                value /= IncrementMult; // Use the multiplicative increment
            else if (ModifierKeys == Keys.Alt)
                value -= Pow(10, 3 * GetExp3Value(value));
            // Increment the position before the decimal separator (ignoring the SI prefix following, i.e. we use the relative decimal separator, not the absolute including the SI prefix)
            else
                value -= Increment; // Normal incrementing
        }

        value = ValidateLimits(value, warnings);
        Exp3 = GetExp3Value(value);
        Text = Formatter.Format(value, unitOptions, formattingOptions);

        ShowTooltip(warnings);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (EnableMath)
        {
            textBoxGraphics.DrawIcon(Resources.MathModeIcon, upDownTextbox.Width - mathIcon.Width - 2, (upDownTextbox.Height - mathIcon.Height) / 2 - 1);
            e.Graphics.DrawRectangle(Pens.CornflowerBlue, new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        if (UserEdit)
            UpdateEditText();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyValue == (char)Keys.Enter || e.KeyValue == (char)Keys.Return)
        {
            // Intercept return key
            OnLeave(e); // Mark as leaving
            e.Handled = true;
        }
        if (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt || e.Modifiers == (Keys.Shift | Keys.Alt))
        {
            if (e.KeyValue == (char)Keys.Up && InterceptArrowKeys)
            {
                e.Handled = true;
                UpButton();

                return;
            }

            if (e.KeyValue == (char)Keys.Down && InterceptArrowKeys)
            {
                e.Handled = true;
                DownButton();

                return;
            }
        }

        if (e.Modifiers == Keys.Control)
        {
            if (e.KeyValue == (char)Keys.M && InterceptArrowKeys)
            {
                // Ctrl+M : Toggle math mode
                EnableMath = !EnableMath;
                ShowTooltip($"Math mode is now {(EnableMath ? "enabled" : "disabled")}.");
                Invalidate();
                Update();
            }

            if (e.KeyValue == (char)Keys.T && InterceptArrowKeys)
            {
                // Ctrl+T : Toggle tooltip
                tooltipOverride = !tooltipOverride;
                if (tooltipOverride)
                {
                    previousTooltipDuration = WarningToolTipDuration;
                    WarningToolTipDuration = 15000;
                    ShowLastTooltip();
                }
                else
                {
                    WarningToolTipDuration = previousTooltipDuration;
                }
            }
        }

        base.OnKeyDown(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            for (var i = 0; i < e.Delta / SystemInformation.MouseWheelScrollDelta; i++)
                UpButton();
        }
        else if (e.Delta < 0)
        {
            for (var i = 0; i < Abs(e.Delta) / SystemInformation.MouseWheelScrollDelta; i++)
                DownButton();
        }
    }

    protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
    {
        base.OnTextBoxKeyPress(source, e);

        var numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
        var decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
        var groupSeparator = numberFormatInfo.NumberGroupSeparator;
        var negativeSign = numberFormatInfo.NegativeSign;

        var keyInput = e.KeyChar.ToString(CultureInfo.InvariantCulture);

        if (Char.IsDigit(e.KeyChar))
        {
            // Digits are OK
            NotifyDataGridViewValueChange();
        }
        else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) || keyInput.Equals(negativeSign))
        {
            // Decimal separator is OK 
            NotifyDataGridViewValueChange();
        }
        else if (e.KeyChar == '\b')
        {
            // Backspace key is OK 
            NotifyDataGridViewValueChange();
        }
        else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
        {
            // Let the edit control handle control and alt key combinations 
        }
        else if (e.KeyChar == '\r')
        {
            e.Handled = true;
        }
    }

    #endregion

    protected override void UpdateEditText()
    {
        // If we're initializing, we don't want to update the edit text yet, 
        // just in case the value is invalid
        if (initializing)
            return;

        var warnings = new List<Exception>();

        // If the current value is user-edited, then parse this value before reformatting
        if (UserEdit)
            ParseEditText(warnings);

        // VSWhidbey 173332: Verify that the user is not starting the string with a "-" 
        // before attempting to set the Value property since a "-" is a valid character with 
        // which to start a string representing a negative number.
        if (!String.IsNullOrEmpty(Text) && !(Text.Length == 1 && Text == "-"))
        {
            ChangingText = true;

            Text = Formatter.Format(value, unitOptions, formattingOptions);

            ShowTooltip(warnings);

            // Notify DataGridView
            NotifyDataGridViewValueChange();

            // Raise ValueChanged event
            ValueChanged?.Invoke(this, EventArgs.Empty); // ck DEBUG 
        }
    }

    protected override void ValidateEditText()
    {
        // See if the edit text parses to a valid decimal
        UpdateEditText();
    }

    #endregion

    #region IDataGridViewEditingControl Members

    public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
    {
        Font = dataGridViewCellStyle.Font;
        ForeColor = dataGridViewCellStyle.ForeColor;
        if (dataGridViewCellStyle.BackColor.A < 255)
        {
            // NumericUpDown control does not support transparent back colors
            var opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
            BackColor = opaqueBackColor;
            EditingControlDataGridView.EditingPanel.BackColor = opaqueBackColor;
        }
        else
            BackColor = dataGridViewCellStyle.BackColor;
        TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
    }

    public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
    {
        switch (keyData & Keys.KeyCode)
        {
            case Keys.Shift:
            case Keys.Alt:
                return true;
            case Keys.Right:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (RightToLeft == RightToLeft.No && !(textBox.SelectionLength == 0 && textBox.SelectionStart == textBox.Text.Length)
                        || RightToLeft == RightToLeft.Yes && !(textBox.SelectionLength == 0 && textBox.SelectionStart == 0))
                        return true;
                }
                break;
            }
            case Keys.Left:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (RightToLeft == RightToLeft.No && !(textBox.SelectionLength == 0 && textBox.SelectionStart == 0)
                        || RightToLeft == RightToLeft.Yes && !(textBox.SelectionLength == 0 && textBox.SelectionStart == textBox.Text.Length))
                        return true;
                }
                break;
            }
            case Keys.Down:
                if (Value > Minimum)
                    return true;
                break;
            case Keys.Up:
                if (Value < Maximum)
                    return true;
                break;
            case Keys.Home:
            case Keys.End:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (textBox.SelectionLength != textBox.Text.Length)
                        return true;
                }
                break;
            }
            case Keys.Delete:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (textBox.SelectionLength > 0 ||
                        textBox.SelectionStart < textBox.Text.Length)
                        return true;
                }
                break;
            }
        }

        return !dataGridViewWantsInputKey;
    }

    public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
    {
        var userEdit = UserEdit;
        try
        {
            UserEdit = ((context & DataGridViewDataErrorContexts.Display) == 0);

            UpdateEditText();

            return EditingControlFormattedValue;
        }
        finally
        {
            UserEdit = userEdit;
        }
    }

    public void PrepareEditingControlForEdit(bool selectAll)
    {
        if (Controls[1] is TextBox textBox)
        {
            if (selectAll)
                textBox.SelectAll();
            else
                textBox.SelectionStart = textBox.Text.Length;
        }
    }

    public DataGridView EditingControlDataGridView { get; set; }

    public object EditingControlFormattedValue
    {
        get { return Text; }
        set { Text = value as string ?? String.Empty; }
    }

    public int EditingControlRowIndex { get; set; }

    public bool EditingControlValueChanged { get; set; }

    public Cursor EditingPanelCursor => Cursor;

    public bool RepositionEditingControlOnValueChange => false;

    #endregion

    #region ISupportInitialize Member

    private bool initializing;

    public void BeginInit()
    {
        initializing = true;
    }

    public void EndInit()
    {
        initializing = false;

        UpdateEditText();
    }

    #endregion

    public void RefreshTextFromValue()
    {
        UpdateEditText();
    }

    public void RefreshValueFromText()
    {
        UserEdit = true;
        UpdateEditText();
    }

    #region Private

    private void ShowTooltip(List<Exception> warnings)
    {

        if (warnings.Any())
        {
            string tooltipstr = EngineeringNotation.CondenseWarnings(warnings);
            ShowTooltip(tooltipstr);
        }
    }

    private void ShowTooltip(string text)
    {
        var point = new Point(Width - 20, Height - 2);

        if (WarningToolTipDuration > 0 && !String.IsNullOrEmpty(text))
        {
            toolTipInfo?.Show(text, this, point, WarningToolTipDuration);
        }

        lastToolTipStr = text;
    }

    private void ShowLastTooltip()
    {
        ShowTooltip(lastToolTipStr);
    }

    private void ParseEditText(List<Exception> warnings)
    {
        try
        {
            value = EngStringToDouble(Text, warnings);
        }
        catch (Exception ex)
        {
            warnings.Add(ex);
        }
        finally
        {
            UserEdit = false;
        }
    }

    // Public conversion method with (almost) no side-effects (changes answer from math evaluator)
    public double EngStringToDouble(string str, List<Exception> warnings)
    {
        double value;
        if (EnableMath)
        {
            if (mathEval == null) // Late initalizer to save resources
                mathEval = new MathEvaluator();

            value = mathEval.Evaluate(str, warnings);
        }
        else
        {
            value = Parser.ParseString(str, unitOptions, formattingOptions, warnings);
        }

        return ValidateLimits(value, warnings);
    }

    private double ValidateLimits(double value, List<Exception> warnings)
    {
        if (initializing || !EnableLimits)
            return value;

        if (value < Minimum)
        {
            warnings.Add(new ArgumentOutOfRangeException(nameof(value), $"Value {value:e} out of lower bounds [{Minimum:e}; {Maximum:e}]."));
            return Minimum;
        }

        if (value > Maximum)
        {
            warnings.Add(new ArgumentOutOfRangeException(nameof(value), $"Value {value:e} out of upper bounds [{Minimum:e}; {Maximum:e}]."));
            return Maximum;
        }

        return value;
    }

    private void NotifyDataGridViewValueChange()
    {
        if (EditingControlDataGridView == null)
            return;

        EditingControlValueChanged = true;
        EditingControlDataGridView.NotifyCurrentCellDirty(true);
    }

    private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align)
    {
        if ((align & (DataGridViewContentAlignment.TopRight) | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight) != 0)
            return HorizontalAlignment.Right;

        if ((align & (DataGridViewContentAlignment.TopCenter) | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter) != 0)
            return HorizontalAlignment.Center;

        return HorizontalAlignment.Left;
    }

    #endregion
}