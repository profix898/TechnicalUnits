//using System;
//using System.ComponentModel;
//using System.Drawing;
//using TechnicalUnits.Parser;

//namespace TechnicalUnits.EngineeringUpDown
//{
//    public class EngineeringUpDownCell : DataGridViewTextBoxCell
//    {
//        private const int ButtonsWidth = 16; // Width of the up/down buttons
//        private const int ButtonMargin = 8; // Margin between the text and buttons

//        private readonly EngineeringNotation engineeringNotation = new EngineeringNotation();

//        public EngineeringUpDownCell()
//        {
//            Increment = 1.0;
//            IncrementMult = 10.0;
//            EnableLimits = true;
//            Minimum = Double.MinValue;
//            Maximum = Double.MaxValue;
//            Value = 0.0;
//        }

//        #region Properties

//        private double increment;

//        [Category("EngineeringNotation")]
//        [Description("Increment")]
//        [DefaultValue(1.0)]
//        public double Increment
//        {
//            get { return increment; }
//            set
//            {
//                increment = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.Increment = value;

//                UpdateOnChange();
//            }
//        }

//        private double incrementMult;

//        [Category("EngineeringNotation")]
//        [Description("IncrementMult")]
//        [DefaultValue(10.0)]
//        public double IncrementMult
//        {
//            get { return incrementMult; }
//            set
//            {
//                incrementMult = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.IncrementMult = value;

//                UpdateOnChange();
//            }
//        }

//        private bool enableLimits;

//        [Category("EngineeringNotation")]
//        [Description("Validates value within Minimum/Maximum bounds.")]
//        [DefaultValue(true)]
//        public bool EnableLimits
//        {
//            get { return enableLimits; }
//            set
//            {
//                enableLimits = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.EnableLimits = value;

//                UpdateOnChange();
//            }
//        }

//         FormattingOptions options;

//        [Browsable(true)]
//        [Category("EngineeringNotation")]
//        [Description("Parser options")]
//        public FormattingOptions Options
//        {
//            get { return engineeringNotation.Options; }
//            set
//            {
//                engineeringNotation.Options = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.Options = value;

//                UpdateOnChange();
//            }
//        }

//        private double minimum;

//        [Category("EngineeringNotation")]
//        [Description("Minimum value (set 'EnableLimits' option to enforce).")]
//        [DefaultValue(Double.MinValue)]
//        public double Minimum
//        {
//            get { return minimum; }
//            set
//            {
//                minimum = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.Minimum = value;

//                UpdateOnChange();
//            }
//        }

//        private double maximum;

//        [Category("EngineeringNotation")]
//        [Description("Maximum value (set 'EnableLimits' option to enforce).")]
//        [DefaultValue(Double.MaxValue)]
//        public double Maximum
//        {
//            get { return maximum; }
//            set
//            {
//                maximum = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.Maximum = value;

//                UpdateOnChange();
//            }
//        }

//        private int exp3;

//        [Category("EngineeringNotation")]
//        [Description("Exponent to base 1000")]
//        [DefaultValue(1.0)]
//        public int Exp3
//        {
//            get { return exp3; }
//            set
//            {
//                exp3 = value;

//                if (OwnsEditingEngineeringUpDown(RowIndex))
//                    EditingEngineeringUpDown.Exp3 = value;

//                UpdateOnChange();
//            }
//        }

//        #endregion

//        #region Overrides of DataGridViewTextBoxCell

//        public override object Clone()
//        {
//            var dataGridViewCell = base.Clone() as EngineeringUpDownCell;
//            if (dataGridViewCell != null)
//            {
//                dataGridViewCell.Increment = Increment;
//                dataGridViewCell.IncrementMult = IncrementMult;
//                dataGridViewCell.EnableLimits = EnableLimits;
//                dataGridViewCell.Options = new FormattingOptions(Options);
//                dataGridViewCell.Maximum = Maximum;
//                dataGridViewCell.Minimum = Minimum;
//            }

//            return dataGridViewCell;
//        }

//        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
//        {
//            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

//            var engineeringUpDown = EditingEngineeringUpDown;
//            if (engineeringUpDown != null)
//            {
//                engineeringUpDown.BorderStyle = BorderStyle.None;
//                engineeringUpDown.Increment = Increment;
//                engineeringUpDown.IncrementMult = IncrementMult;
//                engineeringUpDown.EnableLimits = EnableLimits;
//                engineeringUpDown.Options = new FormattingOptions(Options);
//                engineeringUpDown.Maximum = Maximum;
//                engineeringUpDown.Minimum = Minimum;
//                engineeringUpDown.EditingControlFormattedValue = initialFormattedValue as string ?? String.Empty;
//            }
//        }

//        [EditorBrowsable(EditorBrowsableState.Advanced)]
//        public override void DetachEditingControl()
//        {
//            if (DataGridView != null)
//                (EditingEngineeringUpDown?.Controls[1] as TextBox)?.ClearUndo();

//            base.DetachEditingControl();
//        }

//        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
//        {
//            if (DataGridView == null)
//                return new Size(-1, -1);

//            var preferredSize = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
//            if (constraintSize.Width == 0)
//                preferredSize.Width += ButtonsWidth + ButtonMargin;

//            return preferredSize;
//        }

//        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
//        {
//            var errorIconBounds = base.GetErrorIconBounds(graphics, cellStyle, rowIndex);
//            if (DataGridView.RightToLeft == RightToLeft.Yes)
//                errorIconBounds.X = errorIconBounds.Left + ButtonsWidth;
//            else
//                errorIconBounds.X = errorIconBounds.Left - ButtonsWidth;

//            return errorIconBounds;
//        }

//        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter,
//                                                   TypeConverter valueTypeConverter)
//        {
//            return engineeringNotation.StringToNumber((string) formattedValue);
//        }

//        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter,
//                                                    TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
//        {
//            if (value == null)
//                return String.Empty;

//            return engineeringNotation.NumberToString((double) value);
//        }

//        public override bool KeyEntersEditMode(KeyEventArgs e)
//        {
//            var isCtrl = e.Shift || e.Alt || e.Control;
//            var isSign = Keys.Subtract == e.KeyCode || Keys.Add == e.KeyCode;
//            var isDigit = Char.IsDigit((char) e.KeyCode) || e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9;

//            return (!isCtrl && (isSign || isDigit));
//        }

//        public override Type EditType => typeof(EngineeringUpDown);

//        public override Type FormattedValueType => typeof(string);

//        public override Type ValueType => typeof(double);

//        #endregion

//        #region Private

//        private EngineeringUpDown EditingEngineeringUpDown => DataGridView.EditingControl as EngineeringUpDown;

//        private bool OwnsEditingEngineeringUpDown(int rowIndex)
//        {
//            if (rowIndex == -1 || DataGridView == null)
//                return false;

//            return (EditingEngineeringUpDown != null && EditingEngineeringUpDown.EditingControlRowIndex == rowIndex);
//        }

//        private void UpdateOnChange()
//        {
//            if (DataGridView != null && !DataGridView.IsDisposed && !DataGridView.Disposing)
//            {
//                if (RowIndex == -1)
//                    DataGridView.InvalidateColumn(ColumnIndex);
//                else
//                    DataGridView.UpdateCellValue(ColumnIndex, RowIndex);
//            }
//        }

//        #endregion
//    }
//}