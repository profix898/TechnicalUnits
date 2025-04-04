using System;
using System.ComponentModel;
using System.Windows.Forms;
using TechnicalUnits.Formatting;

namespace TechnicalUnits.WinForms;

public class EngineeringUpDownColumn : DataGridViewColumn
{
    public EngineeringUpDownColumn()
        : base(new EngineeringUpDownCell()) { }

    #region Properties

    [Category("EngineeringNotation")]
    [Description("Increment")]
    [DefaultValue(1.0)]
    public double Increment
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.Increment;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.Increment = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.Increment = value;
            }
        }
    }

    [Category("EngineeringNotation")]
    [Description("IncrementMult")]
    [DefaultValue(10.0)]
    public double IncrementMult
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.IncrementMult;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.IncrementMult = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.IncrementMult = value;
            }
        }
    }

    [Category("EngineeringNotation")]
    [Description("Validates value within Minimum/Maximum bounds.")]
    [DefaultValue(true)]
    public bool EnableLimits
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.EnableLimits;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.EnableLimits = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.EnableLimits = value;
            }
        }
    }

    [Browsable(true)]
    [Category("EngineeringNotation")]
    [Description("Parser options")]
    public FormattingOptions Options
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.Options;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.Options = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.Options = value;
            }
        }
    }

    [Category("EngineeringNotation")]
    [Description("Minimum value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MinValue)]
    public double Minimum
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.Minimum;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.Minimum = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.Minimum = value;
            }
        }
    }

    [Category("EngineeringNotation")]
    [Description("Maximum value (set 'EnableLimits' option to enforce).")]
    [DefaultValue(Double.MaxValue)]
    public double Maximum
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.Maximum;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.Maximum = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.Maximum = value;
            }
        }
    }

    [Category("EngineeringNotation")]
    [Description("Exponent to base 1000")]
    [DefaultValue(1.0)]
    public int Exp3
    {
        get
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            return EngineeringUpDownCellTemplate.Exp3;
        }
        set
        {
            if (EngineeringUpDownCellTemplate == null)
                throw new InvalidOperationException("Operation cannot be completed because this DataGridViewColumn does not have a CellTemplate.");

            EngineeringUpDownCellTemplate.Exp3 = value;

            if (DataGridView == null)
                return;

            var dataGridViewRows = DataGridView.Rows;
            var rowCount = dataGridViewRows.Count;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                if (dataGridViewRow.Cells[Index] is EngineeringUpDownCell dataGridViewCell)
                    dataGridViewCell.Exp3 = value;
            }
        }
    }

    #endregion

    #region Overrides of DataGridViewColumn

    public override DataGridViewCell CellTemplate
    {
        get { return base.CellTemplate; }
        set
        {
            if (value is not EngineeringUpDownCell)
                throw new InvalidCastException("Value provided for CellTemplate must be of type EngineeringUpDownCell.");

            base.CellTemplate = value;
        }
    }

    #endregion

    private EngineeringUpDownCell EngineeringUpDownCellTemplate => (EngineeringUpDownCell)CellTemplate;
}