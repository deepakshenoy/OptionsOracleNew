namespace OptionsOracle.Forms
{
    partial class StockGroupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.earningAddButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.earningDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.earningRadioButton = new System.Windows.Forms.RadioButton();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // earningAddButton
            // 
            this.earningAddButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.earningAddButton.Enabled = false;
            this.earningAddButton.Location = new System.Drawing.Point(204, 76);
            this.earningAddButton.Name = "earningAddButton";
            this.earningAddButton.Size = new System.Drawing.Size(90, 24);
            this.earningAddButton.TabIndex = 1;
            this.earningAddButton.Text = "Add";
            this.earningAddButton.UseVisualStyleBackColor = true;
            this.earningAddButton.Click += new System.EventHandler(this.earningAddButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(108, 76);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 24);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // earningDateTimePicker
            // 
            this.earningDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.earningDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.earningDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.earningDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.earningDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGray;
            this.earningDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.earningDateTimePicker.Enabled = false;
            this.earningDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.earningDateTimePicker.Location = new System.Drawing.Point(197, 35);
            this.earningDateTimePicker.Name = "earningDateTimePicker";
            this.earningDateTimePicker.Size = new System.Drawing.Size(97, 20);
            this.earningDateTimePicker.TabIndex = 5;
            // 
            // earningRadioButton
            // 
            this.earningRadioButton.AutoSize = true;
            this.earningRadioButton.Enabled = false;
            this.earningRadioButton.Location = new System.Drawing.Point(12, 36);
            this.earningRadioButton.Name = "earningRadioButton";
            this.earningRadioButton.Size = new System.Drawing.Size(179, 17);
            this.earningRadioButton.TabIndex = 6;
            this.earningRadioButton.Text = "Add Stocks with Earning Date at";
            this.earningRadioButton.UseVisualStyleBackColor = true;
            this.earningRadioButton.Click += new System.EventHandler(this.xxxRadioButton_CheckedChanged);
            // 
            // allRadioButton
            // 
            this.allRadioButton.AutoSize = true;
            this.allRadioButton.Enabled = false;
            this.allRadioButton.Location = new System.Drawing.Point(12, 13);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(103, 17);
            this.allRadioButton.TabIndex = 7;
            this.allRadioButton.Text = "Add All Symbols.";
            this.allRadioButton.UseVisualStyleBackColor = true;
            this.allRadioButton.CheckedChanged += new System.EventHandler(this.xxxRadioButton_CheckedChanged);
            // 
            // StockGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 109);
            this.Controls.Add(this.allRadioButton);
            this.Controls.Add(this.earningRadioButton);
            this.Controls.Add(this.earningDateTimePicker);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.earningAddButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StockGroupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Stock Group";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button earningAddButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DateTimePicker earningDateTimePicker;
        private System.Windows.Forms.RadioButton earningRadioButton;
        private System.Windows.Forms.RadioButton allRadioButton;
    }
}