using TechnicalUnits.WinForms;

namespace DemoWinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            technicalUnits1 = new TechnicalUnitsUpDown();
            SuspendLayout();
            // 
            // technicalUnits1
            // 
            technicalUnits1.Location = new System.Drawing.Point(12, 12);
            technicalUnits1.Name = "technicalUnits1";
            technicalUnits1.Size = new System.Drawing.Size(250, 27);
            technicalUnits1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(582, 303);
            Controls.Add(technicalUnits1);
            Name = "Form1";
            Text = "Technical Units Demo";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TechnicalUnitsUpDown technicalUnits1;
    }
}
