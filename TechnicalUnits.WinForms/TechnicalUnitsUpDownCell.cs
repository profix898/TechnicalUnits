using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using TechnicalUnits.Formatting;

namespace TechnicalUnits.WinForms;

public class TechnicalUnitsUpDownCell : DataGridViewTextBoxCell, ITechnicalUnitsControl
{
    private const int ButtonMargin = 8; // Margin between the text and buttons
    private const int ButtonsWidth = 16; // Width of the up/down buttons
    private bool _clipValueToMinMax;
    private double _defaultValue;
    private bool _enableMath;

    private FormattingOptions _formattingOptions;
    private double _increment;
    private double _incrementMult;
    private bool _isReadOnly;
    private double _maximum;
    private double _minimum;
    private UnitOptions _unitOptions;

    public TechnicalUnitsUpDownCell()
    {
        FormattingOptions = _formattingOptions = new FormattingOptions();
        UnitOptions = _unitOptions = new UnitOptions();
        Minimum = Double.MinValue;
        Maximum = Double.MaxValue;
        ClipValueToMinMax = true;
        Increment = 1.0;
        IncrementMult = 10.0;
        Value = 1000;
    }

    #region Options

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the formatting options and culture info for parsing and displaying values.")]
    public FormattingOptions FormattingOptions
    {
        get => _formattingOptions;
        set
        {
            _formattingOptions = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.FormattingOptions = value;

            UpdateOnChange();
        }
    }

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the unit options for parsing and displaying values.")]
    public UnitOptions UnitOptions
    {
        get => _unitOptions;
        set
        {
            _unitOptions = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.UnitOptions = value;

            UpdateOnChange();
        }
    }

    [Category("MathEvaluation")]
    [Description("Indicates whether math expressions are enabled.")]
    [DefaultValue(false)]
    public bool EnableMath
    {
        get => _enableMath;
        set
        {
            _enableMath = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.EnableMath = value;

            UpdateOnChange();
        }
    }

    #endregion

    #region MinMax

    [Category("EngineeringNotation")]
    [Description("Specifies the minimum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MinValue)]
    public double Minimum
    {
        get => _minimum;
        set
        {
            _minimum = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.Minimum = value;

            UpdateOnChange();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the maximum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MaxValue)]
    public double Maximum
    {
        get => _maximum;
        set
        {
            _maximum = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.Maximum = value;

            UpdateOnChange();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Indicates whether to clip the value to the min/max bounds.")]
    [DefaultValue(true)]
    public bool ClipValueToMinMax
    {
        get => _clipValueToMinMax;
        set
        {
            _clipValueToMinMax = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.ClipValueToMinMax = value;

            UpdateOnChange();
        }
    }

    #endregion

    #region UpDown

    [Category("EngineeringNotation")]
    [Description("Specifies the increment value for up/down operations. Determines the amount by which the value is increased/decreased when the up/down button is pressed.")]
    [DefaultValue(1.0)]
    public double Increment
    {
        get => _increment;
        set
        {
            _increment = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.Increment = value;

            UpdateOnChange();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the multiplicative increment for value on up/down. Determines the amount by which the value is multiplied/divided when the up/down button is pressed (while the Shift key is pressed).")]
    [DefaultValue(10.0)]
    public double IncrementMult
    {
        get => _incrementMult;
        set
        {
            _incrementMult = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.IncrementMult = value;

            UpdateOnChange();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Indicates whether the control is read-only. When set to true, the value cannot be edited by the user.")]
    [DefaultValue(1.0)]
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            _isReadOnly = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.IsReadOnly = value;

            UpdateOnChange();
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the (current) numeric value of the control.")]
    [DefaultValue(1.0)]
    public double Value
    {
        get => _defaultValue;
        set
        {
            _defaultValue = value;

            if (TryGetEditingTechnicalUnitsUpDown(RowIndex, out var editingEngineeringUpDown))
                editingEngineeringUpDown.Value = value;

            UpdateOnChange();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// Raised when the
    /// <see cref="Value" />
    /// changes.
    public event EventHandler? ValueChanged;

    [ReadOnly(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Text => this.ConvertValueToText(_defaultValue);

    #endregion

    #region Overrides of DataGridViewTextBoxCell

    public override object Clone()
    {
        var technicalUnitsCell = base.Clone() as TechnicalUnitsUpDownCell;
        if (technicalUnitsCell != null)
        {
            technicalUnitsCell.FormattingOptions = FormattingOptions;
            technicalUnitsCell.UnitOptions = UnitOptions;
            technicalUnitsCell.EnableMath = EnableMath;
            technicalUnitsCell.Minimum = Minimum;
            technicalUnitsCell.Maximum = Maximum;
            technicalUnitsCell.ClipValueToMinMax = ClipValueToMinMax;
            technicalUnitsCell.Increment = Increment;
            technicalUnitsCell.IncrementMult = IncrementMult;
            technicalUnitsCell.IsReadOnly = IsReadOnly;
        }

        return technicalUnitsCell;
    }

    public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
        base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

        if (!TryGetEditingTechnicalUnitsUpDown(rowIndex, out var technicalUnitsUpDown))
            return;

        technicalUnitsUpDown.BorderStyle = BorderStyle.None;
        technicalUnitsUpDown.FormattingOptions = FormattingOptions;
        technicalUnitsUpDown.UnitOptions = UnitOptions;
        technicalUnitsUpDown.EnableMath = EnableMath;
        technicalUnitsUpDown.Minimum = Minimum;
        technicalUnitsUpDown.Maximum = Maximum;
        technicalUnitsUpDown.ClipValueToMinMax = ClipValueToMinMax;
        technicalUnitsUpDown.Increment = Increment;
        technicalUnitsUpDown.IncrementMult = IncrementMult;
        technicalUnitsUpDown.IsReadOnly = IsReadOnly;
        technicalUnitsUpDown.EditingControlFormattedValue = initialFormattedValue as string ?? String.Empty;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override void DetachEditingControl()
    {
        if (DataGridView != null && TryGetEditingTechnicalUnitsUpDown(RowIndex, out var technicalUnitsUpDown))
            (technicalUnitsUpDown.Controls[1] as TextBox)?.ClearUndo();

        base.DetachEditingControl();
    }

    protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
    {
        if (DataGridView == null)
            return new Size(-1, -1);

        var preferredSize = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
        if (constraintSize.Width == 0)
            preferredSize.Width += ButtonsWidth + ButtonMargin;

        return preferredSize;
    }

    protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
    {
        var errorIconBounds = base.GetErrorIconBounds(graphics, cellStyle, rowIndex);
        if (DataGridView.RightToLeft == RightToLeft.Yes)
            errorIconBounds.X = errorIconBounds.Left + ButtonsWidth;
        else
            errorIconBounds.X = errorIconBounds.Left - ButtonsWidth;

        return errorIconBounds;
    }

    public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        => this.ConvertTextToValue(formattedValue);

    protected override object GetFormattedValue(object? value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter,
                                                TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        => this.ConvertValueToText(value);

    public override bool KeyEntersEditMode(KeyEventArgs e)
    {
        var isCtrl = e.Shift || e.Alt || e.Control;
        var isSign = System.Windows.Forms.Keys.Subtract == e.KeyCode || System.Windows.Forms.Keys.Add == e.KeyCode;
        var isDigit = Char.IsDigit((char) e.KeyCode) || (e.KeyCode >= System.Windows.Forms.Keys.NumPad0 && e.KeyCode <= System.Windows.Forms.Keys.NumPad9);

        return !isCtrl && (isSign || isDigit);
    }

    public override Type EditType => typeof(TechnicalUnitsUpDown);

    public override Type FormattedValueType => typeof(string);

    public override Type ValueType => typeof(double);

    #endregion

    #region Private

    private bool TryGetEditingTechnicalUnitsUpDown(int rowIndex, [NotNullWhen(true)] out TechnicalUnitsUpDown? technicalUnitsUpDown)
    {
        if (DataGridView != null && rowIndex >= 0 && rowIndex < DataGridView.Rows.Count && DataGridView.EditingControl is TechnicalUnitsUpDown editingControl
            && editingControl.EditingControlRowIndex == rowIndex)
        {
            technicalUnitsUpDown = editingControl;
            return true;
        }

        technicalUnitsUpDown = null;
        return false;
    }

    private void UpdateOnChange()
    {
        if (DataGridView != null && !DataGridView.IsDisposed && !DataGridView.Disposing)
        {
            if (RowIndex == -1)
                DataGridView.InvalidateColumn(ColumnIndex);
            else
                DataGridView.UpdateCellValue(ColumnIndex, RowIndex);
        }
    }

    #endregion
}
