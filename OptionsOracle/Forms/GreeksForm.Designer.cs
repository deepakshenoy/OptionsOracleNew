namespace OptionsOracle.Forms
{
    partial class GreeksForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GreeksForm));
            this.expirationDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.stockPriceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.strikePriceTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.optionPriceTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.interestTextBox = new System.Windows.Forms.TextBox();
            this.calcOptionPriceButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.impliedVolatilityTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.deltaTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.thetaTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.vegaTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.gammaTextBox = new System.Windows.Forms.TextBox();
            this.calcImpliedVolatilityButton = new System.Windows.Forms.Button();
            this.optionsTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.optionsSet = new OptionsOracle.Data.OptionsSet();
            this.optionsSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.atDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.daysTextBox = new System.Windows.Forms.TextBox();
            this.optionSelectionGroupBox2 = new System.Windows.Forms.GroupBox();
            this.otmCheckBox = new System.Windows.Forms.CheckBox();
            this.atmCheckBox = new System.Windows.Forms.CheckBox();
            this.itmCheckBox = new System.Windows.Forms.CheckBox();
            this.optionSelectionGroupBox3 = new System.Windows.Forms.GroupBox();
            this.expRadioButton7 = new System.Windows.Forms.RadioButton();
            this.expRadioButton6 = new System.Windows.Forms.RadioButton();
            this.expRadioButton5 = new System.Windows.Forms.RadioButton();
            this.expRadioButton4 = new System.Windows.Forms.RadioButton();
            this.expRadioButton3 = new System.Windows.Forms.RadioButton();
            this.expRadioButton2 = new System.Windows.Forms.RadioButton();
            this.expRadioButton1 = new System.Windows.Forms.RadioButton();
            this.expRadioButtonAll = new System.Windows.Forms.RadioButton();
            this.optionSelectionGroupBox1 = new System.Windows.Forms.GroupBox();
            this.putsCheckBox = new System.Windows.Forms.CheckBox();
            this.callsCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsDataGridView = new System.Windows.Forms.DataGridView();
            this.optionsTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsStrikeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsExpirationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsSymbolColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsLastColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsChangeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsTimeValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsBidColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsAskColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsVolumeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsOpenIntColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsStockColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.optionsUpdateTimeStampColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.forwardTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.calcGroupBox = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.binominalStepsTextBox = new System.Windows.Forms.TextBox();
            this.binominalRadioButton = new System.Windows.Forms.RadioButton();
            this.blackScholesRadioButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.rhoTextBox = new System.Windows.Forms.TextBox();
            this.calcStockPriceButton = new System.Windows.Forms.Button();
            this.dividendRateTextBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.optionsTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsSetBindingSource)).BeginInit();
            this.optionSelectionGroupBox2.SuspendLayout();
            this.optionSelectionGroupBox3.SuspendLayout();
            this.optionSelectionGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsDataGridView)).BeginInit();
            this.calcGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // expirationDateTimePicker
            // 
            this.expirationDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.expirationDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.expirationDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.expirationDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.expirationDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGray;
            this.expirationDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.expirationDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.expirationDateTimePicker.Location = new System.Drawing.Point(101, 46);
            this.expirationDateTimePicker.Name = "expirationDateTimePicker";
            this.expirationDateTimePicker.Size = new System.Drawing.Size(94, 20);
            this.expirationDateTimePicker.TabIndex = 16;
            this.expirationDateTimePicker.ValueChanged += new System.EventHandler(this.xxxDateTimePicker_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Expiration Date";
            // 
            // stockPriceTextBox
            // 
            this.stockPriceTextBox.Location = new System.Drawing.Point(316, 72);
            this.stockPriceTextBox.Name = "stockPriceTextBox";
            this.stockPriceTextBox.Size = new System.Drawing.Size(94, 20);
            this.stockPriceTextBox.TabIndex = 22;
            this.stockPriceTextBox.TextChanged += new System.EventHandler(this.xxxTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(221, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Stock Price";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(221, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Strike Price";
            // 
            // strikePriceTextBox
            // 
            this.strikePriceTextBox.Location = new System.Drawing.Point(316, 46);
            this.strikePriceTextBox.Name = "strikePriceTextBox";
            this.strikePriceTextBox.Size = new System.Drawing.Size(94, 20);
            this.strikePriceTextBox.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(458, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Option Price";
            // 
            // optionPriceTextBox
            // 
            this.optionPriceTextBox.Location = new System.Drawing.Point(552, 20);
            this.optionPriceTextBox.Name = "optionPriceTextBox";
            this.optionPriceTextBox.Size = new System.Drawing.Size(94, 20);
            this.optionPriceTextBox.TabIndex = 27;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Interest [%]";
            // 
            // interestTextBox
            // 
            this.interestTextBox.Location = new System.Drawing.Point(316, 98);
            this.interestTextBox.Name = "interestTextBox";
            this.interestTextBox.Size = new System.Drawing.Size(94, 20);
            this.interestTextBox.TabIndex = 24;
            this.interestTextBox.TextChanged += new System.EventHandler(this.xxxTextBox_TextChanged);
            // 
            // calcOptionPriceButton
            // 
            this.calcOptionPriceButton.Location = new System.Drawing.Point(653, 20);
            this.calcOptionPriceButton.Name = "calcOptionPriceButton";
            this.calcOptionPriceButton.Size = new System.Drawing.Size(20, 20);
            this.calcOptionPriceButton.TabIndex = 28;
            this.calcOptionPriceButton.Text = "C";
            this.calcOptionPriceButton.UseVisualStyleBackColor = true;
            this.calcOptionPriceButton.Click += new System.EventHandler(this.calcButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(221, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Volatility [%]";
            // 
            // impliedVolatilityTextBox
            // 
            this.impliedVolatilityTextBox.Location = new System.Drawing.Point(316, 124);
            this.impliedVolatilityTextBox.Name = "impliedVolatilityTextBox";
            this.impliedVolatilityTextBox.Size = new System.Drawing.Size(94, 20);
            this.impliedVolatilityTextBox.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(458, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Delta";
            // 
            // deltaTextBox
            // 
            this.deltaTextBox.Location = new System.Drawing.Point(553, 46);
            this.deltaTextBox.Name = "deltaTextBox";
            this.deltaTextBox.ReadOnly = true;
            this.deltaTextBox.Size = new System.Drawing.Size(94, 20);
            this.deltaTextBox.TabIndex = 0;
            this.deltaTextBox.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(458, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Theta [day]";
            // 
            // thetaTextBox
            // 
            this.thetaTextBox.Location = new System.Drawing.Point(553, 98);
            this.thetaTextBox.Name = "thetaTextBox";
            this.thetaTextBox.ReadOnly = true;
            this.thetaTextBox.Size = new System.Drawing.Size(94, 20);
            this.thetaTextBox.TabIndex = 0;
            this.thetaTextBox.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(458, 128);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Vega [% volatility]";
            // 
            // vegaTextBox
            // 
            this.vegaTextBox.Location = new System.Drawing.Point(553, 124);
            this.vegaTextBox.Name = "vegaTextBox";
            this.vegaTextBox.ReadOnly = true;
            this.vegaTextBox.Size = new System.Drawing.Size(94, 20);
            this.vegaTextBox.TabIndex = 0;
            this.vegaTextBox.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(458, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Gamma";
            // 
            // gammaTextBox
            // 
            this.gammaTextBox.Location = new System.Drawing.Point(553, 72);
            this.gammaTextBox.Name = "gammaTextBox";
            this.gammaTextBox.ReadOnly = true;
            this.gammaTextBox.Size = new System.Drawing.Size(94, 20);
            this.gammaTextBox.TabIndex = 0;
            this.gammaTextBox.TabStop = false;
            // 
            // calcImpliedVolatilityButton
            // 
            this.calcImpliedVolatilityButton.Location = new System.Drawing.Point(416, 123);
            this.calcImpliedVolatilityButton.Name = "calcImpliedVolatilityButton";
            this.calcImpliedVolatilityButton.Size = new System.Drawing.Size(20, 20);
            this.calcImpliedVolatilityButton.TabIndex = 26;
            this.calcImpliedVolatilityButton.Text = "C";
            this.calcImpliedVolatilityButton.UseVisualStyleBackColor = true;
            this.calcImpliedVolatilityButton.Click += new System.EventHandler(this.calcButton_Click);
            // 
            // optionsTableBindingSource
            // 
            this.optionsTableBindingSource.DataMember = "OptionsTable";
            this.optionsTableBindingSource.DataSource = this.optionsSet;
            // 
            // optionsSet
            // 
            this.optionsSet.DataSetName = "OptionsSet";
            this.optionsSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // optionsSetBindingSource
            // 
            this.optionsSetBindingSource.DataSource = this.optionsSet;
            this.optionsSetBindingSource.Position = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Valuation Date";
            // 
            // atDateTimePicker
            // 
            this.atDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.atDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.atDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.atDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.atDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGray;
            this.atDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.atDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.atDateTimePicker.Location = new System.Drawing.Point(101, 20);
            this.atDateTimePicker.Name = "atDateTimePicker";
            this.atDateTimePicker.Size = new System.Drawing.Size(94, 20);
            this.atDateTimePicker.TabIndex = 15;
            this.atDateTimePicker.ValueChanged += new System.EventHandler(this.xxxDateTimePicker_ValueChanged);
            // 
            // typeComboBox
            // 
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Items.AddRange(new object[] {
            "Call",
            "Put"});
            this.typeComboBox.Location = new System.Drawing.Point(316, 19);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(94, 21);
            this.typeComboBox.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(221, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Option Type";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 76);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Total Days";
            // 
            // daysTextBox
            // 
            this.daysTextBox.Location = new System.Drawing.Point(101, 72);
            this.daysTextBox.Name = "daysTextBox";
            this.daysTextBox.ReadOnly = true;
            this.daysTextBox.Size = new System.Drawing.Size(94, 20);
            this.daysTextBox.TabIndex = 0;
            this.daysTextBox.TabStop = false;
            this.daysTextBox.TextChanged += new System.EventHandler(this.xxxTextBox_TextChanged);
            // 
            // optionSelectionGroupBox2
            // 
            this.optionSelectionGroupBox2.Controls.Add(this.otmCheckBox);
            this.optionSelectionGroupBox2.Controls.Add(this.atmCheckBox);
            this.optionSelectionGroupBox2.Controls.Add(this.itmCheckBox);
            this.optionSelectionGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optionSelectionGroupBox2.Location = new System.Drawing.Point(119, 2);
            this.optionSelectionGroupBox2.Name = "optionSelectionGroupBox2";
            this.optionSelectionGroupBox2.Size = new System.Drawing.Size(157, 34);
            this.optionSelectionGroupBox2.TabIndex = 0;
            this.optionSelectionGroupBox2.TabStop = false;
            // 
            // otmCheckBox
            // 
            this.otmCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.otmCheckBox.Checked = true;
            this.otmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.otmCheckBox.Location = new System.Drawing.Point(103, 9);
            this.otmCheckBox.Name = "otmCheckBox";
            this.otmCheckBox.Size = new System.Drawing.Size(50, 22);
            this.otmCheckBox.TabIndex = 6;
            this.otmCheckBox.Tag = "(TheMoney LIKE \'OTM\')";
            this.otmCheckBox.Text = "OTM";
            this.otmCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.otmCheckBox.UseVisualStyleBackColor = true;
            this.otmCheckBox.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // atmCheckBox
            // 
            this.atmCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.atmCheckBox.Checked = true;
            this.atmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.atmCheckBox.Location = new System.Drawing.Point(53, 9);
            this.atmCheckBox.Name = "atmCheckBox";
            this.atmCheckBox.Size = new System.Drawing.Size(50, 22);
            this.atmCheckBox.TabIndex = 5;
            this.atmCheckBox.Tag = "(TheMoney LIKE \'ATM\')";
            this.atmCheckBox.Text = "ATM";
            this.atmCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.atmCheckBox.UseVisualStyleBackColor = true;
            this.atmCheckBox.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // itmCheckBox
            // 
            this.itmCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.itmCheckBox.Checked = true;
            this.itmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.itmCheckBox.Location = new System.Drawing.Point(3, 9);
            this.itmCheckBox.Name = "itmCheckBox";
            this.itmCheckBox.Size = new System.Drawing.Size(50, 22);
            this.itmCheckBox.TabIndex = 4;
            this.itmCheckBox.Tag = "(TheMoney LIKE \'ITM\')";
            this.itmCheckBox.Text = "ITM";
            this.itmCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.itmCheckBox.UseVisualStyleBackColor = true;
            this.itmCheckBox.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // optionSelectionGroupBox3
            // 
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton7);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton6);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton5);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton4);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton3);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton2);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButton1);
            this.optionSelectionGroupBox3.Controls.Add(this.expRadioButtonAll);
            this.optionSelectionGroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optionSelectionGroupBox3.Location = new System.Drawing.Point(280, 2);
            this.optionSelectionGroupBox3.Name = "optionSelectionGroupBox3";
            this.optionSelectionGroupBox3.Size = new System.Drawing.Size(409, 34);
            this.optionSelectionGroupBox3.TabIndex = 0;
            this.optionSelectionGroupBox3.TabStop = false;
            // 
            // expRadioButton7
            // 
            this.expRadioButton7.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton7.Location = new System.Drawing.Point(355, 9);
            this.expRadioButton7.Name = "expRadioButton7";
            this.expRadioButton7.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton7.TabIndex = 14;
            this.expRadioButton7.TabStop = true;
            this.expRadioButton7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton7.UseVisualStyleBackColor = true;
            this.expRadioButton7.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton6
            // 
            this.expRadioButton6.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton6.Location = new System.Drawing.Point(304, 9);
            this.expRadioButton6.Name = "expRadioButton6";
            this.expRadioButton6.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton6.TabIndex = 13;
            this.expRadioButton6.TabStop = true;
            this.expRadioButton6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton6.UseVisualStyleBackColor = true;
            this.expRadioButton6.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton5
            // 
            this.expRadioButton5.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton5.Location = new System.Drawing.Point(253, 9);
            this.expRadioButton5.Name = "expRadioButton5";
            this.expRadioButton5.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton5.TabIndex = 12;
            this.expRadioButton5.TabStop = true;
            this.expRadioButton5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton5.UseVisualStyleBackColor = true;
            this.expRadioButton5.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton4
            // 
            this.expRadioButton4.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton4.Location = new System.Drawing.Point(203, 9);
            this.expRadioButton4.Name = "expRadioButton4";
            this.expRadioButton4.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton4.TabIndex = 11;
            this.expRadioButton4.TabStop = true;
            this.expRadioButton4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton4.UseVisualStyleBackColor = true;
            this.expRadioButton4.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton3
            // 
            this.expRadioButton3.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton3.Location = new System.Drawing.Point(153, 9);
            this.expRadioButton3.Name = "expRadioButton3";
            this.expRadioButton3.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton3.TabIndex = 10;
            this.expRadioButton3.TabStop = true;
            this.expRadioButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton3.UseVisualStyleBackColor = true;
            this.expRadioButton3.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton2
            // 
            this.expRadioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton2.Location = new System.Drawing.Point(103, 9);
            this.expRadioButton2.Name = "expRadioButton2";
            this.expRadioButton2.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton2.TabIndex = 9;
            this.expRadioButton2.TabStop = true;
            this.expRadioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton2.UseVisualStyleBackColor = true;
            this.expRadioButton2.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButton1
            // 
            this.expRadioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButton1.Location = new System.Drawing.Point(53, 9);
            this.expRadioButton1.Name = "expRadioButton1";
            this.expRadioButton1.Size = new System.Drawing.Size(50, 22);
            this.expRadioButton1.TabIndex = 8;
            this.expRadioButton1.TabStop = true;
            this.expRadioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButton1.UseVisualStyleBackColor = true;
            this.expRadioButton1.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // expRadioButtonAll
            // 
            this.expRadioButtonAll.Appearance = System.Windows.Forms.Appearance.Button;
            this.expRadioButtonAll.Checked = true;
            this.expRadioButtonAll.Location = new System.Drawing.Point(2, 9);
            this.expRadioButtonAll.Name = "expRadioButtonAll";
            this.expRadioButtonAll.Size = new System.Drawing.Size(50, 22);
            this.expRadioButtonAll.TabIndex = 7;
            this.expRadioButtonAll.TabStop = true;
            this.expRadioButtonAll.Text = "All";
            this.expRadioButtonAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.expRadioButtonAll.UseVisualStyleBackColor = true;
            this.expRadioButtonAll.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // optionSelectionGroupBox1
            // 
            this.optionSelectionGroupBox1.Controls.Add(this.putsCheckBox);
            this.optionSelectionGroupBox1.Controls.Add(this.callsCheckBox);
            this.optionSelectionGroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optionSelectionGroupBox1.Location = new System.Drawing.Point(6, 2);
            this.optionSelectionGroupBox1.Name = "optionSelectionGroupBox1";
            this.optionSelectionGroupBox1.Size = new System.Drawing.Size(109, 34);
            this.optionSelectionGroupBox1.TabIndex = 0;
            this.optionSelectionGroupBox1.TabStop = false;
            // 
            // putsCheckBox
            // 
            this.putsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.putsCheckBox.Checked = true;
            this.putsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.putsCheckBox.Location = new System.Drawing.Point(55, 9);
            this.putsCheckBox.Name = "putsCheckBox";
            this.putsCheckBox.Size = new System.Drawing.Size(50, 22);
            this.putsCheckBox.TabIndex = 3;
            this.putsCheckBox.Tag = "(Type = \'Put\')";
            this.putsCheckBox.Text = "Puts";
            this.putsCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.putsCheckBox.UseVisualStyleBackColor = true;
            this.putsCheckBox.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // callsCheckBox
            // 
            this.callsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.callsCheckBox.Checked = true;
            this.callsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.callsCheckBox.Location = new System.Drawing.Point(5, 9);
            this.callsCheckBox.Name = "callsCheckBox";
            this.callsCheckBox.Size = new System.Drawing.Size(50, 22);
            this.callsCheckBox.TabIndex = 2;
            this.callsCheckBox.Tag = "(Type = \'Call\')";
            this.callsCheckBox.Text = "Calls";
            this.callsCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.callsCheckBox.UseVisualStyleBackColor = true;
            this.callsCheckBox.CheckedChanged += new System.EventHandler(this.filtersCheckBox_CheckedChanged);
            // 
            // optionsDataGridView
            // 
            this.optionsDataGridView.AllowUserToAddRows = false;
            this.optionsDataGridView.AllowUserToDeleteRows = false;
            this.optionsDataGridView.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.optionsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.optionsDataGridView.ColumnHeadersHeight = 22;
            this.optionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.optionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.optionsTypeColumn,
            this.optionsStrikeColumn,
            this.optionsExpirationColumn,
            this.optionsSymbolColumn,
            this.optionsLastColumn,
            this.optionsChangeColumn,
            this.optionsTimeValueColumn,
            this.optionsBidColumn,
            this.optionsAskColumn,
            this.optionsVolumeColumn,
            this.optionsOpenIntColumn,
            this.optionsStockColumn,
            this.optionsUpdateTimeStampColumn});
            this.optionsDataGridView.DataSource = this.optionsTableBindingSource;
            this.optionsDataGridView.Location = new System.Drawing.Point(6, 42);
            this.optionsDataGridView.Name = "optionsDataGridView";
            this.optionsDataGridView.ReadOnly = true;
            this.optionsDataGridView.RowHeadersVisible = false;
            this.optionsDataGridView.RowHeadersWidth = 16;
            this.optionsDataGridView.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.optionsDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionsDataGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Cornsilk;
            this.optionsDataGridView.RowTemplate.Height = 20;
            this.optionsDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.optionsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.optionsDataGridView.Size = new System.Drawing.Size(684, 144);
            this.optionsDataGridView.TabIndex = 1;
            this.optionsDataGridView.SelectionChanged += new System.EventHandler(this.optionsDataGridView_SelectionChanged);
            // 
            // optionsTypeColumn
            // 
            this.optionsTypeColumn.DataPropertyName = "Type";
            this.optionsTypeColumn.HeaderText = "Type";
            this.optionsTypeColumn.Name = "optionsTypeColumn";
            this.optionsTypeColumn.ReadOnly = true;
            this.optionsTypeColumn.Width = 45;
            // 
            // optionsStrikeColumn
            // 
            this.optionsStrikeColumn.DataPropertyName = "Strike";
            dataGridViewCellStyle2.Format = "N2";
            this.optionsStrikeColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.optionsStrikeColumn.HeaderText = "Strike";
            this.optionsStrikeColumn.Name = "optionsStrikeColumn";
            this.optionsStrikeColumn.ReadOnly = true;
            this.optionsStrikeColumn.Width = 60;
            // 
            // optionsExpirationColumn
            // 
            this.optionsExpirationColumn.DataPropertyName = "Expiration";
            dataGridViewCellStyle3.Format = "dd-MMM-yyyy";
            this.optionsExpirationColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.optionsExpirationColumn.HeaderText = "Expiration";
            this.optionsExpirationColumn.Name = "optionsExpirationColumn";
            this.optionsExpirationColumn.ReadOnly = true;
            this.optionsExpirationColumn.Width = 75;
            // 
            // optionsSymbolColumn
            // 
            this.optionsSymbolColumn.DataPropertyName = "Symbol";
            this.optionsSymbolColumn.HeaderText = "Symbol";
            this.optionsSymbolColumn.Name = "optionsSymbolColumn";
            this.optionsSymbolColumn.ReadOnly = true;
            this.optionsSymbolColumn.Width = 60;
            // 
            // optionsLastColumn
            // 
            this.optionsLastColumn.DataPropertyName = "Last";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = "N/A";
            this.optionsLastColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.optionsLastColumn.HeaderText = "Last";
            this.optionsLastColumn.Name = "optionsLastColumn";
            this.optionsLastColumn.ReadOnly = true;
            this.optionsLastColumn.Width = 60;
            // 
            // optionsChangeColumn
            // 
            this.optionsChangeColumn.DataPropertyName = "Change";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = "N/A";
            this.optionsChangeColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.optionsChangeColumn.HeaderText = "Change";
            this.optionsChangeColumn.Name = "optionsChangeColumn";
            this.optionsChangeColumn.ReadOnly = true;
            this.optionsChangeColumn.Width = 60;
            // 
            // optionsTimeValueColumn
            // 
            this.optionsTimeValueColumn.DataPropertyName = "TimeValue";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = "N/A";
            this.optionsTimeValueColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.optionsTimeValueColumn.HeaderText = "TimeValue";
            this.optionsTimeValueColumn.Name = "optionsTimeValueColumn";
            this.optionsTimeValueColumn.ReadOnly = true;
            this.optionsTimeValueColumn.Width = 61;
            // 
            // optionsBidColumn
            // 
            this.optionsBidColumn.DataPropertyName = "Bid";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            dataGridViewCellStyle7.NullValue = "N/A";
            this.optionsBidColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.optionsBidColumn.HeaderText = "Bid";
            this.optionsBidColumn.Name = "optionsBidColumn";
            this.optionsBidColumn.ReadOnly = true;
            this.optionsBidColumn.Width = 60;
            // 
            // optionsAskColumn
            // 
            this.optionsAskColumn.DataPropertyName = "Ask";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            dataGridViewCellStyle8.NullValue = "N/A";
            this.optionsAskColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.optionsAskColumn.HeaderText = "Ask";
            this.optionsAskColumn.Name = "optionsAskColumn";
            this.optionsAskColumn.ReadOnly = true;
            this.optionsAskColumn.Width = 60;
            // 
            // optionsVolumeColumn
            // 
            this.optionsVolumeColumn.DataPropertyName = "Volume";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N0";
            this.optionsVolumeColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.optionsVolumeColumn.HeaderText = "Volume";
            this.optionsVolumeColumn.Name = "optionsVolumeColumn";
            this.optionsVolumeColumn.ReadOnly = true;
            this.optionsVolumeColumn.Width = 60;
            // 
            // optionsOpenIntColumn
            // 
            this.optionsOpenIntColumn.DataPropertyName = "OpenInt";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "N0";
            this.optionsOpenIntColumn.DefaultCellStyle = dataGridViewCellStyle10;
            this.optionsOpenIntColumn.HeaderText = "Open Int";
            this.optionsOpenIntColumn.Name = "optionsOpenIntColumn";
            this.optionsOpenIntColumn.ReadOnly = true;
            this.optionsOpenIntColumn.Width = 60;
            // 
            // optionsStockColumn
            // 
            this.optionsStockColumn.DataPropertyName = "Stock";
            this.optionsStockColumn.HeaderText = "Stock";
            this.optionsStockColumn.Name = "optionsStockColumn";
            this.optionsStockColumn.ReadOnly = true;
            this.optionsStockColumn.Visible = false;
            this.optionsStockColumn.Width = 60;
            // 
            // optionsUpdateTimeStampColumn
            // 
            this.optionsUpdateTimeStampColumn.DataPropertyName = "UpdateTimeStamp";
            dataGridViewCellStyle11.Format = "g";
            dataGridViewCellStyle11.NullValue = null;
            this.optionsUpdateTimeStampColumn.DefaultCellStyle = dataGridViewCellStyle11;
            this.optionsUpdateTimeStampColumn.HeaderText = "UpdateTimeStamp";
            this.optionsUpdateTimeStampColumn.Name = "optionsUpdateTimeStampColumn";
            this.optionsUpdateTimeStampColumn.ReadOnly = true;
            this.optionsUpdateTimeStampColumn.Visible = false;
            this.optionsUpdateTimeStampColumn.Width = 60;
            // 
            // forwardTextBox
            // 
            this.forwardTextBox.Location = new System.Drawing.Point(553, 177);
            this.forwardTextBox.Name = "forwardTextBox";
            this.forwardTextBox.ReadOnly = true;
            this.forwardTextBox.Size = new System.Drawing.Size(94, 20);
            this.forwardTextBox.TabIndex = 0;
            this.forwardTextBox.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(458, 180);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Forward Price";
            // 
            // calcGroupBox
            // 
            this.calcGroupBox.Controls.Add(this.dividendRateTextBox);
            this.calcGroupBox.Controls.Add(this.label17);
            this.calcGroupBox.Controls.Add(this.label16);
            this.calcGroupBox.Controls.Add(this.binominalStepsTextBox);
            this.calcGroupBox.Controls.Add(this.binominalRadioButton);
            this.calcGroupBox.Controls.Add(this.blackScholesRadioButton);
            this.calcGroupBox.Controls.Add(this.label15);
            this.calcGroupBox.Controls.Add(this.rhoTextBox);
            this.calcGroupBox.Controls.Add(this.calcStockPriceButton);
            this.calcGroupBox.Controls.Add(this.label11);
            this.calcGroupBox.Controls.Add(this.label14);
            this.calcGroupBox.Controls.Add(this.expirationDateTimePicker);
            this.calcGroupBox.Controls.Add(this.forwardTextBox);
            this.calcGroupBox.Controls.Add(this.label1);
            this.calcGroupBox.Controls.Add(this.stockPriceTextBox);
            this.calcGroupBox.Controls.Add(this.label2);
            this.calcGroupBox.Controls.Add(this.strikePriceTextBox);
            this.calcGroupBox.Controls.Add(this.label3);
            this.calcGroupBox.Controls.Add(this.label13);
            this.calcGroupBox.Controls.Add(this.optionPriceTextBox);
            this.calcGroupBox.Controls.Add(this.daysTextBox);
            this.calcGroupBox.Controls.Add(this.label4);
            this.calcGroupBox.Controls.Add(this.label12);
            this.calcGroupBox.Controls.Add(this.interestTextBox);
            this.calcGroupBox.Controls.Add(this.typeComboBox);
            this.calcGroupBox.Controls.Add(this.label5);
            this.calcGroupBox.Controls.Add(this.calcOptionPriceButton);
            this.calcGroupBox.Controls.Add(this.atDateTimePicker);
            this.calcGroupBox.Controls.Add(this.impliedVolatilityTextBox);
            this.calcGroupBox.Controls.Add(this.calcImpliedVolatilityButton);
            this.calcGroupBox.Controls.Add(this.label6);
            this.calcGroupBox.Controls.Add(this.label10);
            this.calcGroupBox.Controls.Add(this.deltaTextBox);
            this.calcGroupBox.Controls.Add(this.gammaTextBox);
            this.calcGroupBox.Controls.Add(this.label7);
            this.calcGroupBox.Controls.Add(this.label9);
            this.calcGroupBox.Controls.Add(this.thetaTextBox);
            this.calcGroupBox.Controls.Add(this.vegaTextBox);
            this.calcGroupBox.Controls.Add(this.label8);
            this.calcGroupBox.Location = new System.Drawing.Point(6, 192);
            this.calcGroupBox.Name = "calcGroupBox";
            this.calcGroupBox.Size = new System.Drawing.Size(684, 206);
            this.calcGroupBox.TabIndex = 0;
            this.calcGroupBox.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 107);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(74, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Pricing Model:";
            // 
            // binominalStepsTextBox
            // 
            this.binominalStepsTextBox.AcceptsReturn = true;
            this.binominalStepsTextBox.Enabled = false;
            this.binominalStepsTextBox.Location = new System.Drawing.Point(85, 149);
            this.binominalStepsTextBox.Name = "binominalStepsTextBox";
            this.binominalStepsTextBox.Size = new System.Drawing.Size(33, 20);
            this.binominalStepsTextBox.TabIndex = 19;
            this.binominalStepsTextBox.Text = "50";
            this.binominalStepsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // binominalRadioButton
            // 
            this.binominalRadioButton.AutoSize = true;
            this.binominalRadioButton.Location = new System.Drawing.Point(9, 150);
            this.binominalRadioButton.Name = "binominalRadioButton";
            this.binominalRadioButton.Size = new System.Drawing.Size(148, 17);
            this.binominalRadioButton.TabIndex = 18;
            this.binominalRadioButton.TabStop = true;
            this.binominalRadioButton.Text = "Binominal  (             Steps)";
            this.binominalRadioButton.UseVisualStyleBackColor = true;
            this.binominalRadioButton.CheckedChanged += new System.EventHandler(this.xxxRadioButton_CheckedChanged);
            // 
            // blackScholesRadioButton
            // 
            this.blackScholesRadioButton.AutoSize = true;
            this.blackScholesRadioButton.Checked = true;
            this.blackScholesRadioButton.Location = new System.Drawing.Point(9, 127);
            this.blackScholesRadioButton.Name = "blackScholesRadioButton";
            this.blackScholesRadioButton.Size = new System.Drawing.Size(93, 17);
            this.blackScholesRadioButton.TabIndex = 17;
            this.blackScholesRadioButton.TabStop = true;
            this.blackScholesRadioButton.Text = "Black-Scholes";
            this.blackScholesRadioButton.UseVisualStyleBackColor = true;
            this.blackScholesRadioButton.CheckedChanged += new System.EventHandler(this.xxxRadioButton_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(458, 155);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Rho [% interest]";
            // 
            // rhoTextBox
            // 
            this.rhoTextBox.Location = new System.Drawing.Point(553, 151);
            this.rhoTextBox.Name = "rhoTextBox";
            this.rhoTextBox.ReadOnly = true;
            this.rhoTextBox.Size = new System.Drawing.Size(94, 20);
            this.rhoTextBox.TabIndex = 0;
            this.rhoTextBox.TabStop = false;
            // 
            // calcStockPriceButton
            // 
            this.calcStockPriceButton.Location = new System.Drawing.Point(416, 72);
            this.calcStockPriceButton.Name = "calcStockPriceButton";
            this.calcStockPriceButton.Size = new System.Drawing.Size(20, 20);
            this.calcStockPriceButton.TabIndex = 23;
            this.calcStockPriceButton.Text = "C";
            this.calcStockPriceButton.UseVisualStyleBackColor = true;
            this.calcStockPriceButton.Click += new System.EventHandler(this.calcButton_Click);
            // 
            // dividendRateTextBox
            // 
            this.dividendRateTextBox.Location = new System.Drawing.Point(316, 150);
            this.dividendRateTextBox.Name = "dividendRateTextBox";
            this.dividendRateTextBox.Size = new System.Drawing.Size(94, 20);
            this.dividendRateTextBox.TabIndex = 30;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(221, 154);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(92, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "Dividend Rate [%]";
            // 
            // GreeksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 410);
            this.Controls.Add(this.calcGroupBox);
            this.Controls.Add(this.optionSelectionGroupBox2);
            this.Controls.Add(this.optionSelectionGroupBox3);
            this.Controls.Add(this.optionSelectionGroupBox1);
            this.Controls.Add(this.optionsDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GreeksForm";
            this.Text = "Greeks Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.optionsTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsSetBindingSource)).EndInit();
            this.optionSelectionGroupBox2.ResumeLayout(false);
            this.optionSelectionGroupBox3.ResumeLayout(false);
            this.optionSelectionGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.optionsDataGridView)).EndInit();
            this.calcGroupBox.ResumeLayout(false);
            this.calcGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker expirationDateTimePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox stockPriceTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox strikePriceTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox optionPriceTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox interestTextBox;
        private System.Windows.Forms.Button calcOptionPriceButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox impliedVolatilityTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox deltaTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox thetaTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox vegaTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox gammaTextBox;
        private System.Windows.Forms.Button calcImpliedVolatilityButton;
        private System.Windows.Forms.BindingSource optionsTableBindingSource;
        private OptionsOracle.Data.OptionsSet optionsSet;
        private System.Windows.Forms.BindingSource optionsSetBindingSource;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker atDateTimePicker;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox daysTextBox;
        private System.Windows.Forms.GroupBox optionSelectionGroupBox2;
        private System.Windows.Forms.CheckBox otmCheckBox;
        private System.Windows.Forms.CheckBox atmCheckBox;
        private System.Windows.Forms.CheckBox itmCheckBox;
        private System.Windows.Forms.GroupBox optionSelectionGroupBox3;
        private System.Windows.Forms.RadioButton expRadioButton7;
        private System.Windows.Forms.RadioButton expRadioButton6;
        private System.Windows.Forms.RadioButton expRadioButton5;
        private System.Windows.Forms.RadioButton expRadioButton4;
        private System.Windows.Forms.RadioButton expRadioButton3;
        private System.Windows.Forms.RadioButton expRadioButton2;
        private System.Windows.Forms.RadioButton expRadioButton1;
        private System.Windows.Forms.RadioButton expRadioButtonAll;
        private System.Windows.Forms.GroupBox optionSelectionGroupBox1;
        private System.Windows.Forms.CheckBox putsCheckBox;
        private System.Windows.Forms.CheckBox callsCheckBox;
        private System.Windows.Forms.DataGridView optionsDataGridView;
        private System.Windows.Forms.TextBox forwardTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox calcGroupBox;
        private System.Windows.Forms.Button calcStockPriceButton;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox rhoTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsStrikeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsExpirationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsSymbolColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsLastColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsChangeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsTimeValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsBidColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsAskColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsVolumeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsOpenIntColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsStockColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn optionsUpdateTimeStampColumn;
        private System.Windows.Forms.TextBox binominalStepsTextBox;
        private System.Windows.Forms.RadioButton binominalRadioButton;
        private System.Windows.Forms.RadioButton blackScholesRadioButton;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox dividendRateTextBox;
        private System.Windows.Forms.Label label17;
    }
}