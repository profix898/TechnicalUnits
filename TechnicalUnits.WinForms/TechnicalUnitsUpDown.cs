using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TechnicalUnits.Formatting;
using TechnicalUnits.Internal;
using TechnicalUnits.WinForms.Properties;
using static System.Math;

namespace TechnicalUnits.WinForms;

/// <summary>
/// Represents a specialized numeric up-down control that supports engineering notation
/// and unit formatting. This control allows users to input and manipulate numeric values
/// with advanced formatting and validation options.
/// </summary>
/// <remarks>
/// The <see cref="TechnicalUnitsUpDown"/> control extends the functionality of the standard
/// <see cref="UpDownBase"/> control by providing support for engineering notation and unit
/// formatting. It also integrates with <see cref="IDataGridViewEditingControl"/> to enable
/// editing within a <see cref="DataGridView"/>.
/// 
/// This control is designed to be used in scenarios where precise numeric input and display
/// are required, such as engineering, scientific, or technical applications. It provides
/// features like customizable formatting options, unit handling, and value constraints.
/// </remarks>
public sealed class TechnicalUnitsUpDown : UpDownBase, ITechnicalUnitsControl, ITechnicalUnitsControlImpl, IDataGridViewEditingControl, ISupportInitialize
{
    private readonly ToolTip _toolTipInfo = new ToolTip();
    private readonly Icon _mathModeIcon = Resources.MathModeIcon;

    private readonly TextBox _upDownTextbox;
    private readonly Graphics _textBoxGraphics;

    private FormattingOptions _formattingOptions = new FormattingOptions();
    private UnitOptions _unitOptions = new UnitOptions();

    private double _value;

    private readonly TechnicalUnitsControlMixin _mixin;

    public TechnicalUnitsUpDown()
        : this(null)
    {
    }

    public TechnicalUnitsUpDown(IContainer? container)
    {
        _mixin = new TechnicalUnitsControlMixin(this);

        Minimum = Double.MinValue;
        Maximum = Double.MaxValue;
        ClipValueToMinMax = true;
        Increment = 1.0;
        IncrementMult = 10.0;
        Value = 1000;

        _upDownTextbox = Controls[1] as TextBox;
        _textBoxGraphics = _upDownTextbox.CreateGraphics();

        container?.Add(this);
    }

    #region Options

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the formatting options and culture info for parsing and displaying values.")]
    public FormattingOptions FormattingOptions
    {
        get { return _formattingOptions; }
        set
        {
            _formattingOptions = value;
            UpdateText();
        }
    }

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the unit options for parsing and displaying values.")]
    public UnitOptions UnitOptions
    {
        get { return _unitOptions; }
        set
        {
            _unitOptions = value;
            UpdateText();
        }
    }

    [Category("MathEvaluation")]
    [Description("Indicates whether math expressions are enabled.")]
    [DefaultValue(false)]
    public bool EnableMath { get; set; }

    #endregion

    #region MinMax

    [Category("EngineeringNotation")]
    [Description("Specifies the minimum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MinValue)]
    public double Minimum { get; set; }

    [Category("EngineeringNotation")]
    [Description("Specifies the maximum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MaxValue)]
    public double Maximum { get; set; }

    [Category("EngineeringNotation")]
    [Description("Indicates whether to clip the value to the min/max bounds.")]
    [DefaultValue(true)]
    public bool ClipValueToMinMax { get; set; }

    #endregion

    #region UpDown

    [Category("EngineeringNotation")]
    [Description("Specifies the increment value for up/down operations. Determines the amount by which the value is increased/decreased when the up/down button is pressed.")]
    [DefaultValue(1.0)]
    public double Increment { get; set; }

    [Category("EngineeringNotation")]
    [Description("Specifies the multiplicative increment for value on up/down. Determines the amount by which the value is multiplied/divided when the up/down button is pressed (while the Shift key is pressed).")]
    [DefaultValue(10.0)]
    public double IncrementMult { get; set; }

    [Category("EngineeringNotation")]
    [Description("Indicates whether the control is read-only. When set to true, the value cannot be edited by the user.")]
    [DefaultValue(1.0)]
    public bool IsReadOnly { get; set; }

    [Category("EngineeringNotation")]
    [Description("Specifies the (current) numeric value of the control.")]
    [DefaultValue(1.0)]
    public double Value
    {
        get
        {
            if (UserEdit)
                _mixin.TryUpdateValue(); // Try to convert pending user edit to numeric value

            return _value;
        }

        set
        {
            _value = _mixin.ValidateLimits(value);
            UpdateText();

            if (!_initializing)
                ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// Raised when the <see cref="Value"/> changes.
    public event EventHandler? ValueChanged;

    [ReadOnly(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get { return base.Text; }
        set { base.Text = value; }
    }

    #endregion

    #region ITechnicalUnitsControlImpl Members

    /// <inheritdoc />
    public void SetValue(double value, List<Exception>? errors = null)
    {
        if (_initializing)
            return;

        // Set the value and update the text representation
        Value = value;

        // Show any validation errors in tooltip
        if (errors != null && errors.Count > 0)
        {
            var errorString = errors.FormatExceptionsToString();
            var point = new Point(Width - 20, Height - 2);
            _toolTipInfo.Show(errorString, this, point, 2000);
        }
    }

    /// <inheritdoc />
    public void UpdateText()
    {
        ChangingText = true;
        base.Text = _mixin.ConvertValueToText(_value);
        ChangingText = false;
        UserEdit = false;
    }

    #endregion

    #region Overrides

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (EnableMath)
        {
            _textBoxGraphics.DrawIcon(_mathModeIcon, _upDownTextbox.Width - _mathModeIcon.Width - 2, (_upDownTextbox.Height - _mathModeIcon.Height) / 2);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.SteelBlue, ButtonBorderStyle.Solid);
        }
    }

    protected override void UpdateEditText()
    {
        if (_initializing)
            return;

        // Try to convert pending user edit to numeric value
        _mixin.TryUpdateValue();
        UserEdit = false;

        // Notify DataGridView
        NotifyDataGridViewValueChange();

        // Raise ValueChanged event
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        if (UserEdit)
            UpdateEditText(); // Try to convert pending user edit to numeric value
    }

    #region UpDownButton

    public override void UpButton()
    {
        _mixin.OnUpButton();
    }

    public override void DownButton()
    {
        _mixin.OnDownButton();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        // Emulates the up/down button behavior on mouse wheel scroll
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

    #endregion

    #region Keyboard

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_mixin.OnKeyDown(MapKeys(e.KeyCode), MapKeyModifiers(e.Modifiers)))
            e.Handled = true;

        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (_mixin.OnKeyUp(MapKeyModifiers(e.Modifiers)))
            e.Handled = true;

        base.OnKeyDown(e);
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
        else if ((ModifierKeys & (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)) != 0)
        {
            // Let the edit control handle control and alt key combinations 
        }
        else if (e.KeyChar == '\r')
        {
            e.Handled = true;
        }
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
                _toolTipInfo.Dispose();
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    #endregion

    #region ISupportInitialize Member

    private bool _initializing;

    public void BeginInit()
    {
        _initializing = true;
    }

    public void EndInit()
    {
        _initializing = false;

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

    public bool EditingControlWantsInputKey(System.Windows.Forms.Keys keyData, bool dataGridViewWantsInputKey)
    {
        switch (keyData & System.Windows.Forms.Keys.KeyCode)
        {
            case System.Windows.Forms.Keys.Shift:
            case System.Windows.Forms.Keys.Alt:
                return true;
            case System.Windows.Forms.Keys.Right:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (RightToLeft == RightToLeft.No && !(textBox.SelectionLength == 0 && textBox.SelectionStart == textBox.Text.Length)
                        || RightToLeft == RightToLeft.Yes && !(textBox.SelectionLength == 0 && textBox.SelectionStart == 0))
                        return true;
                }
                break;
            }
            case System.Windows.Forms.Keys.Left:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (RightToLeft == RightToLeft.No && !(textBox.SelectionLength == 0 && textBox.SelectionStart == 0)
                        || RightToLeft == RightToLeft.Yes && !(textBox.SelectionLength == 0 && textBox.SelectionStart == textBox.Text.Length))
                        return true;
                }
                break;
            }
            case System.Windows.Forms.Keys.Down:
                if (Value > Minimum)
                    return true;
                break;
            case System.Windows.Forms.Keys.Up:
                if (Value < Maximum)
                    return true;
                break;
            case System.Windows.Forms.Keys.Home:
            case System.Windows.Forms.Keys.End:
            {
                if (Controls[1] is TextBox textBox)
                {
                    if (textBox.SelectionLength != textBox.Text.Length)
                        return true;
                }
                break;
            }
            case System.Windows.Forms.Keys.Delete:
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

    public DataGridView? EditingControlDataGridView { get; set; }

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

    #region Private
    
    private void NotifyDataGridViewValueChange()
    {
        if (EditingControlDataGridView == null)
            return;

        EditingControlValueChanged = true;
        EditingControlDataGridView.NotifyCurrentCellDirty(true);
    }

    private static Keys? MapKeys(System.Windows.Forms.Keys keyCode)
    {
        return keyCode switch
        {
            System.Windows.Forms.Keys.Enter => Keys.Enter,
            System.Windows.Forms.Keys.Up => Keys.Up,
            System.Windows.Forms.Keys.Down => Keys.Down,
            System.Windows.Forms.Keys.M => Keys.M,
            _ => null
        };
    }

    private static KeyModifiers MapKeyModifiers(System.Windows.Forms.Keys keyModifiers)
    {
        KeyModifiers mappedModifiers = KeyModifiers.None;

        if ((keyModifiers & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control)
            mappedModifiers |= KeyModifiers.Ctrl;
        if ((keyModifiers & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
            mappedModifiers |= KeyModifiers.Shift;
        if ((keyModifiers & System.Windows.Forms.Keys.Alt) == System.Windows.Forms.Keys.Alt)
            mappedModifiers |= KeyModifiers.Alt;

        return mappedModifiers;
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