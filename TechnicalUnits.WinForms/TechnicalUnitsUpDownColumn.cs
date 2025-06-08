using System;
using System.ComponentModel;
using System.Windows.Forms;
using TechnicalUnits.Formatting;

namespace TechnicalUnits.WinForms;

public class TechnicalUnitsUpDownColumn : DataGridViewColumn, ITechnicalUnitsControl
{
    public TechnicalUnitsUpDownColumn()
        : base(new TechnicalUnitsUpDownCell())
    {
    }

    #region Options

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the formatting options and culture info for parsing and displaying values.")]
    public FormattingOptions FormattingOptions
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.FormattingOptions;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.FormattingOptions = value;
            SetTechnicalUnitsCellOption(cell => cell.FormattingOptions = value);
        }
    }

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Specifies the unit options for parsing and displaying values.")]
    public UnitOptions UnitOptions
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.UnitOptions;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.UnitOptions = value;
            SetTechnicalUnitsCellOption(cell => cell.UnitOptions = value);
        }
    }

    [Category("MathEvaluation")]
    [Description("Indicates whether math expressions are enabled.")]
    [DefaultValue(false)]
    public bool EnableMath
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.EnableMath;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.EnableMath = value;
            SetTechnicalUnitsCellOption(cell => cell.EnableMath = value);
        }
    }

    #endregion

    #region MinMax

    [Category("EngineeringNotation")]
    [Description("Specifies the minimum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MinValue)]
    public double Minimum
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.Minimum;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.Minimum = value;
            SetTechnicalUnitsCellOption(cell => cell.Minimum = value);
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the minimum allowed value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MaxValue)]
    public double Maximum
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.Maximum;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.Maximum = value;
            SetTechnicalUnitsCellOption(cell => cell.Maximum = value);
        }
    }

    [Category("EngineeringNotation")]
    [Description("Indicates whether to clip the value to the min/max bounds.")]
    [DefaultValue(true)]
    public bool ClipValueToMinMax
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.ClipValueToMinMax;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.ClipValueToMinMax = value;
            SetTechnicalUnitsCellOption(cell => cell.ClipValueToMinMax = value);
        }
    }

    #endregion

    #region UpDown

    [Category("EngineeringNotation")]
    [Description("Specifies the increment value for up/down operations. Determines the amount by which the value is increased/decreased when the up/down button is pressed.")]
    [DefaultValue(1.0)]
    public double Increment
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.Increment;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.Increment = value;
            SetTechnicalUnitsCellOption(cell => cell.Increment = value);
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the multiplicative increment for value on up/down. Determines the amount by which the value is multiplied/divided when the up/down button is pressed (while the Shift key is pressed).")]
    [DefaultValue(10.0)]
    public double IncrementMult
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.IncrementMult;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.IncrementMult = value;
            SetTechnicalUnitsCellOption(cell => cell.IncrementMult = value);
        }
    }

    [Category("EngineeringNotation")]
    [Description("Indicates whether the control is read-only. When set to true, the value cannot be edited by the user.")]
    [DefaultValue(1.0)]
    public bool IsReadOnly
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.IsReadOnly;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.IsReadOnly = value;
            SetTechnicalUnitsCellOption(cell => cell.IsReadOnly = value);
        }
    }

    [Category("EngineeringNotation")]
    [Description("Specifies the (current) numeric value of the control.")]
    [DefaultValue(1.0)]
    public double Value
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.Value;
        }
        set
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            TechnicalUnitsUpDownCellTemplate.Value = value;
            SetTechnicalUnitsCellOption(cell => cell.Value = value);
        }
    }

    [ReadOnly(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Text
    {
        get
        {
            if (TechnicalUnitsUpDownCellTemplate == null)
                throw new InvalidOperationException("DataGridViewColumn does not have a CellTemplate (of type TechnicalUnitsUpDownCell).");

            return TechnicalUnitsUpDownCellTemplate.Text;
        }
    }

    #endregion

    #region Overrides of DataGridViewColumn

    public override DataGridViewCell CellTemplate
    {
        get { return base.CellTemplate; }
        set
        {
            if (value is not TechnicalUnitsUpDownCell)
                throw new InvalidCastException("Value provided for CellTemplate must be of type TechnicalUnitsUpDownCell.");

            base.CellTemplate = value;
        }
    }

    #endregion

    #region Private

    private TechnicalUnitsUpDownCell TechnicalUnitsUpDownCellTemplate => (TechnicalUnitsUpDownCell) CellTemplate;

    private void SetTechnicalUnitsCellOption(Action<TechnicalUnitsUpDownCell> action)
    {
        if (DataGridView == null)
            return;

        var dataGridViewRows = DataGridView.Rows;
        var rowCount = dataGridViewRows.Count;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
            if (dataGridViewRow.Cells[Index] is TechnicalUnitsUpDownCell dataGridViewCell)
                action(dataGridViewCell);
        }
    }

    #endregion
}