namespace OptionsOracle.Forms
{
    partial class OptionsFilterForm
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
            this.expirationListBox = new System.Windows.Forms.ListBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.strikeListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.typeListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.selectionGroupBox = new System.Windows.Forms.GroupBox();
            this.strikeAllButton = new System.Windows.Forms.Button();
            this.strikeNoneButton = new System.Windows.Forms.Button();
            this.expirationAllButton = new System.Windows.Forms.Button();
            this.expirationNoneButton = new System.Windows.Forms.Button();
            this.typeAllButton = new System.Windows.Forms.Button();
            this.typeNoneButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.strategyOptionsCheckBox = new System.Windows.Forms.CheckBox();
            this.selectionGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // expirationListBox
            // 
            this.expirationListBox.FormattingEnabled = true;
            this.expirationListBox.Location = new System.Drawing.Point(102, 32);
            this.expirationListBox.Name = "expirationListBox";
            this.expirationListBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.expirationListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.expirationListBox.Size = new System.Drawing.Size(90, 173);
            this.expirationListBox.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(134, 303);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(80, 24);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(220, 303);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(80, 24);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Expiration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Strike";
            // 
            // strikeListBox
            // 
            this.strikeListBox.FormattingEnabled = true;
            this.strikeListBox.Location = new System.Drawing.Point(198, 32);
            this.strikeListBox.Name = "strikeListBox";
            this.strikeListBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.strikeListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.strikeListBox.Size = new System.Drawing.Size(90, 173);
            this.strikeListBox.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Type";
            // 
            // typeListBox
            // 
            this.typeListBox.FormattingEnabled = true;
            this.typeListBox.Items.AddRange(new object[] {
            "Call",
            "Put"});
            this.typeListBox.Location = new System.Drawing.Point(6, 32);
            this.typeListBox.Name = "typeListBox";
            this.typeListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.typeListBox.Size = new System.Drawing.Size(90, 173);
            this.typeListBox.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 236);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(285, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "* Hold the CTRL key and click items to select and unselect";
            // 
            // selectionGroupBox
            // 
            this.selectionGroupBox.Controls.Add(this.strikeAllButton);
            this.selectionGroupBox.Controls.Add(this.strikeNoneButton);
            this.selectionGroupBox.Controls.Add(this.expirationAllButton);
            this.selectionGroupBox.Controls.Add(this.expirationNoneButton);
            this.selectionGroupBox.Controls.Add(this.typeAllButton);
            this.selectionGroupBox.Controls.Add(this.typeNoneButton);
            this.selectionGroupBox.Controls.Add(this.typeListBox);
            this.selectionGroupBox.Controls.Add(this.label4);
            this.selectionGroupBox.Controls.Add(this.expirationListBox);
            this.selectionGroupBox.Controls.Add(this.label3);
            this.selectionGroupBox.Controls.Add(this.label1);
            this.selectionGroupBox.Controls.Add(this.strikeListBox);
            this.selectionGroupBox.Controls.Add(this.label2);
            this.selectionGroupBox.Location = new System.Drawing.Point(5, 45);
            this.selectionGroupBox.Name = "selectionGroupBox";
            this.selectionGroupBox.Size = new System.Drawing.Size(295, 252);
            this.selectionGroupBox.TabIndex = 21;
            this.selectionGroupBox.TabStop = false;
            // 
            // strikeAllButton
            // 
            this.strikeAllButton.Location = new System.Drawing.Point(246, 206);
            this.strikeAllButton.Name = "strikeAllButton";
            this.strikeAllButton.Size = new System.Drawing.Size(43, 24);
            this.strikeAllButton.TabIndex = 26;
            this.strikeAllButton.Text = "All";
            this.strikeAllButton.UseVisualStyleBackColor = true;
            this.strikeAllButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // strikeNoneButton
            // 
            this.strikeNoneButton.Location = new System.Drawing.Point(197, 206);
            this.strikeNoneButton.Name = "strikeNoneButton";
            this.strikeNoneButton.Size = new System.Drawing.Size(43, 24);
            this.strikeNoneButton.TabIndex = 25;
            this.strikeNoneButton.Text = "None";
            this.strikeNoneButton.UseVisualStyleBackColor = true;
            this.strikeNoneButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // expirationAllButton
            // 
            this.expirationAllButton.Location = new System.Drawing.Point(150, 206);
            this.expirationAllButton.Name = "expirationAllButton";
            this.expirationAllButton.Size = new System.Drawing.Size(43, 24);
            this.expirationAllButton.TabIndex = 24;
            this.expirationAllButton.Text = "All";
            this.expirationAllButton.UseVisualStyleBackColor = true;
            this.expirationAllButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // expirationNoneButton
            // 
            this.expirationNoneButton.Location = new System.Drawing.Point(101, 206);
            this.expirationNoneButton.Name = "expirationNoneButton";
            this.expirationNoneButton.Size = new System.Drawing.Size(43, 24);
            this.expirationNoneButton.TabIndex = 23;
            this.expirationNoneButton.Text = "None";
            this.expirationNoneButton.UseVisualStyleBackColor = true;
            this.expirationNoneButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // typeAllButton
            // 
            this.typeAllButton.Location = new System.Drawing.Point(54, 206);
            this.typeAllButton.Name = "typeAllButton";
            this.typeAllButton.Size = new System.Drawing.Size(43, 24);
            this.typeAllButton.TabIndex = 22;
            this.typeAllButton.Text = "All";
            this.typeAllButton.UseVisualStyleBackColor = true;
            this.typeAllButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // typeNoneButton
            // 
            this.typeNoneButton.Location = new System.Drawing.Point(5, 206);
            this.typeNoneButton.Name = "typeNoneButton";
            this.typeNoneButton.Size = new System.Drawing.Size(43, 24);
            this.typeNoneButton.TabIndex = 21;
            this.typeNoneButton.Text = "None";
            this.typeNoneButton.UseVisualStyleBackColor = true;
            this.typeNoneButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.strategyOptionsCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(5, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 37);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // strategyOptionsCheckBox
            // 
            this.strategyOptionsCheckBox.AutoSize = true;
            this.strategyOptionsCheckBox.Location = new System.Drawing.Point(6, 14);
            this.strategyOptionsCheckBox.Name = "strategyOptionsCheckBox";
            this.strategyOptionsCheckBox.Size = new System.Drawing.Size(211, 17);
            this.strategyOptionsCheckBox.TabIndex = 0;
            this.strategyOptionsCheckBox.Text = "Show Only Options from Strategy Table";
            this.strategyOptionsCheckBox.UseVisualStyleBackColor = true;
            this.strategyOptionsCheckBox.CheckedChanged += new System.EventHandler(this.strategyOptionsCheckBox_CheckedChanged);
            // 
            // OptionsFilterForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(306, 339);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.selectionGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "OptionsFilterForm";
            this.Text = "Options Filter";
            this.selectionGroupBox.ResumeLayout(false);
            this.selectionGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox expirationListBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox strikeListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox typeListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox selectionGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox strategyOptionsCheckBox;
        private System.Windows.Forms.Button strikeAllButton;
        private System.Windows.Forms.Button strikeNoneButton;
        private System.Windows.Forms.Button expirationAllButton;
        private System.Windows.Forms.Button expirationNoneButton;
        private System.Windows.Forms.Button typeAllButton;
        private System.Windows.Forms.Button typeNoneButton;

    }
}