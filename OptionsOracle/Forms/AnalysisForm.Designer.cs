namespace OptionsOracle.Forms
{
    partial class AnalysisForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisForm));
            this.label3 = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.parameterGroupBox = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.underlyingPanel = new System.Windows.Forms.Panel();
            this.undelyingButton_last = new System.Windows.Forms.Button();
            this.undelyingNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.undelyingNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.underlyingTextBox = new System.Windows.Forms.TextBox();
            this.volatilityPanel = new System.Windows.Forms.Panel();
            this.volatilityButton_optionImplied = new System.Windows.Forms.Button();
            this.volatilityButton_stockHistorical = new System.Windows.Forms.Button();
            this.volatilityButton_stockImplied = new System.Windows.Forms.Button();
            this.volatilityNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.volatilityNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.volatilityTextBox = new System.Windows.Forms.TextBox();
            this.endDatePanel = new System.Windows.Forms.Panel();
            this.endDateButton_endDate = new System.Windows.Forms.Button();
            this.endDateTextBox = new System.Windows.Forms.TextBox();
            this.endDateButton_today = new System.Windows.Forms.Button();
            this.endDateNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDateNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.resultsDataGridView = new System.Windows.Forms.DataGridView();
            this.analysisTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.analysisSet = new OptionsOracle.Data.AnalysisSet();
            this.analysisSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableGroupBox = new System.Windows.Forms.GroupBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.addRowButton = new System.Windows.Forms.Button();
            this.deleteRowButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.resoTextBox = new System.Windows.Forms.TextBox();
            this.stepLabel = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.toTextBox = new System.Windows.Forms.TextBox();
            this.toDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.toLabel = new System.Windows.Forms.Label();
            this.fromTextBox = new System.Windows.Forms.TextBox();
            this.fromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.fromLabel = new System.Windows.Forms.Label();
            this.scaleGroupBox = new System.Windows.Forms.GroupBox();
            this.fromNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.fromNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.resoNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.resoNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.toNumericUpDown_big = new System.Windows.Forms.NumericUpDown();
            this.toNumericUpDown_small = new System.Windows.Forms.NumericUpDown();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsIndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsPriceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsVolatilityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsEndDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsProfitColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsProfitPrcColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsDeltaColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsGammaColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsThetaColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultsVegaColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterGroupBox.SuspendLayout();
            this.panel3.SuspendLayout();
            this.underlyingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.undelyingNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.undelyingNumericUpDown_small)).BeginInit();
            this.volatilityPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volatilityNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volatilityNumericUpDown_small)).BeginInit();
            this.endDatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endDateNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateNumericUpDown_small)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisSetBindingSource)).BeginInit();
            this.tableGroupBox.SuspendLayout();
            this.scaleGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fromNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromNumericUpDown_small)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resoNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resoNumericUpDown_small)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toNumericUpDown_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toNumericUpDown_small)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Mode";
            // 
            // modeComboBox
            // 
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Profit/Loss Vs. Underlying",
            "Profit/Loss Vs. Date",
            "Profit/Loss Vs. Volatility"});
            this.modeComboBox.Location = new System.Drawing.Point(4, 18);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(170, 21);
            this.modeComboBox.TabIndex = 2;
            this.modeComboBox.Text = "Profit/Loss Vs. Underlying";
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // parameterGroupBox
            // 
            this.parameterGroupBox.Controls.Add(this.panel3);
            this.parameterGroupBox.Controls.Add(this.underlyingPanel);
            this.parameterGroupBox.Controls.Add(this.volatilityPanel);
            this.parameterGroupBox.Controls.Add(this.endDatePanel);
            this.parameterGroupBox.Location = new System.Drawing.Point(696, 12);
            this.parameterGroupBox.Name = "parameterGroupBox";
            this.parameterGroupBox.Size = new System.Drawing.Size(199, 261);
            this.parameterGroupBox.TabIndex = 7;
            this.parameterGroupBox.TabStop = false;
            this.parameterGroupBox.Text = "Parameters";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.modeComboBox);
            this.panel3.Location = new System.Drawing.Point(6, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(184, 45);
            this.panel3.TabIndex = 9;
            // 
            // underlyingPanel
            // 
            this.underlyingPanel.BackColor = System.Drawing.Color.Transparent;
            this.underlyingPanel.Controls.Add(this.undelyingButton_last);
            this.underlyingPanel.Controls.Add(this.undelyingNumericUpDown_big);
            this.underlyingPanel.Controls.Add(this.undelyingNumericUpDown_small);
            this.underlyingPanel.Controls.Add(this.label7);
            this.underlyingPanel.Controls.Add(this.underlyingTextBox);
            this.underlyingPanel.Location = new System.Drawing.Point(6, 155);
            this.underlyingPanel.Name = "underlyingPanel";
            this.underlyingPanel.Size = new System.Drawing.Size(184, 45);
            this.underlyingPanel.TabIndex = 10;
            // 
            // undelyingButton_last
            // 
            this.undelyingButton_last.Location = new System.Drawing.Point(121, 18);
            this.undelyingButton_last.Name = "undelyingButton_last";
            this.undelyingButton_last.Size = new System.Drawing.Size(20, 21);
            this.undelyingButton_last.TabIndex = 12;
            this.undelyingButton_last.Text = "L";
            this.toolTip.SetToolTip(this.undelyingButton_last, "Last Underlying Price");
            this.undelyingButton_last.UseVisualStyleBackColor = true;
            this.undelyingButton_last.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // undelyingNumericUpDown_big
            // 
            this.undelyingNumericUpDown_big.Location = new System.Drawing.Point(102, 19);
            this.undelyingNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.undelyingNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.undelyingNumericUpDown_big.Name = "undelyingNumericUpDown_big";
            this.undelyingNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.undelyingNumericUpDown_big.TabIndex = 11;
            this.undelyingNumericUpDown_big.Tag = "0";
            this.undelyingNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // undelyingNumericUpDown_small
            // 
            this.undelyingNumericUpDown_small.Location = new System.Drawing.Point(85, 19);
            this.undelyingNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.undelyingNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.undelyingNumericUpDown_small.Name = "undelyingNumericUpDown_small";
            this.undelyingNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.undelyingNumericUpDown_small.TabIndex = 10;
            this.undelyingNumericUpDown_small.Tag = "0";
            this.undelyingNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "At Underlying";
            // 
            // underlyingTextBox
            // 
            this.underlyingTextBox.Location = new System.Drawing.Point(4, 19);
            this.underlyingTextBox.Name = "underlyingTextBox";
            this.underlyingTextBox.Size = new System.Drawing.Size(80, 20);
            this.underlyingTextBox.TabIndex = 9;
            this.underlyingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.underlyingTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.underlyingTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            this.underlyingTextBox.Enter += new System.EventHandler(this.xxxText_Enter);
            // 
            // volatilityPanel
            // 
            this.volatilityPanel.BackColor = System.Drawing.Color.Transparent;
            this.volatilityPanel.Controls.Add(this.volatilityButton_optionImplied);
            this.volatilityPanel.Controls.Add(this.volatilityButton_stockHistorical);
            this.volatilityPanel.Controls.Add(this.volatilityButton_stockImplied);
            this.volatilityPanel.Controls.Add(this.volatilityNumericUpDown_big);
            this.volatilityPanel.Controls.Add(this.volatilityNumericUpDown_small);
            this.volatilityPanel.Controls.Add(this.label1);
            this.volatilityPanel.Controls.Add(this.volatilityTextBox);
            this.volatilityPanel.Location = new System.Drawing.Point(6, 104);
            this.volatilityPanel.Name = "volatilityPanel";
            this.volatilityPanel.Size = new System.Drawing.Size(184, 45);
            this.volatilityPanel.TabIndex = 6;
            // 
            // volatilityButton_optionImplied
            // 
            this.volatilityButton_optionImplied.Location = new System.Drawing.Point(161, 18);
            this.volatilityButton_optionImplied.Name = "volatilityButton_optionImplied";
            this.volatilityButton_optionImplied.Size = new System.Drawing.Size(20, 21);
            this.volatilityButton_optionImplied.TabIndex = 8;
            this.volatilityButton_optionImplied.Text = "X";
            this.toolTip.SetToolTip(this.volatilityButton_optionImplied, "Option Specific Implied Volatility");
            this.volatilityButton_optionImplied.UseVisualStyleBackColor = true;
            this.volatilityButton_optionImplied.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // volatilityButton_stockHistorical
            // 
            this.volatilityButton_stockHistorical.Location = new System.Drawing.Point(141, 18);
            this.volatilityButton_stockHistorical.Name = "volatilityButton_stockHistorical";
            this.volatilityButton_stockHistorical.Size = new System.Drawing.Size(20, 21);
            this.volatilityButton_stockHistorical.TabIndex = 7;
            this.volatilityButton_stockHistorical.Text = "H";
            this.toolTip.SetToolTip(this.volatilityButton_stockHistorical, "Stock Historical Volatility");
            this.volatilityButton_stockHistorical.UseVisualStyleBackColor = true;
            this.volatilityButton_stockHistorical.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // volatilityButton_stockImplied
            // 
            this.volatilityButton_stockImplied.Location = new System.Drawing.Point(121, 18);
            this.volatilityButton_stockImplied.Name = "volatilityButton_stockImplied";
            this.volatilityButton_stockImplied.Size = new System.Drawing.Size(20, 21);
            this.volatilityButton_stockImplied.TabIndex = 6;
            this.volatilityButton_stockImplied.Text = "I";
            this.toolTip.SetToolTip(this.volatilityButton_stockImplied, "Stock Average Implied Volatility");
            this.volatilityButton_stockImplied.UseVisualStyleBackColor = true;
            this.volatilityButton_stockImplied.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // volatilityNumericUpDown_big
            // 
            this.volatilityNumericUpDown_big.Location = new System.Drawing.Point(102, 19);
            this.volatilityNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.volatilityNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.volatilityNumericUpDown_big.Name = "volatilityNumericUpDown_big";
            this.volatilityNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.volatilityNumericUpDown_big.TabIndex = 5;
            this.volatilityNumericUpDown_big.Tag = "0";
            this.volatilityNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // volatilityNumericUpDown_small
            // 
            this.volatilityNumericUpDown_small.Location = new System.Drawing.Point(85, 19);
            this.volatilityNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.volatilityNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.volatilityNumericUpDown_small.Name = "volatilityNumericUpDown_small";
            this.volatilityNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.volatilityNumericUpDown_small.TabIndex = 4;
            this.volatilityNumericUpDown_small.Tag = "0";
            this.volatilityNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "At Volatility";
            // 
            // volatilityTextBox
            // 
            this.volatilityTextBox.Location = new System.Drawing.Point(4, 19);
            this.volatilityTextBox.Name = "volatilityTextBox";
            this.volatilityTextBox.Size = new System.Drawing.Size(80, 20);
            this.volatilityTextBox.TabIndex = 3;
            this.volatilityTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.volatilityTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.volatilityTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            this.volatilityTextBox.Enter += new System.EventHandler(this.xxxText_Enter);
            // 
            // endDatePanel
            // 
            this.endDatePanel.BackColor = System.Drawing.Color.Transparent;
            this.endDatePanel.Controls.Add(this.endDateButton_endDate);
            this.endDatePanel.Controls.Add(this.endDateTextBox);
            this.endDatePanel.Controls.Add(this.endDateButton_today);
            this.endDatePanel.Controls.Add(this.endDateNumericUpDown_big);
            this.endDatePanel.Controls.Add(this.endDateTimePicker);
            this.endDatePanel.Controls.Add(this.endDateNumericUpDown_small);
            this.endDatePanel.Controls.Add(this.label2);
            this.endDatePanel.Location = new System.Drawing.Point(6, 206);
            this.endDatePanel.Name = "endDatePanel";
            this.endDatePanel.Size = new System.Drawing.Size(184, 45);
            this.endDatePanel.TabIndex = 8;
            // 
            // endDateButton_endDate
            // 
            this.endDateButton_endDate.Location = new System.Drawing.Point(141, 18);
            this.endDateButton_endDate.Name = "endDateButton_endDate";
            this.endDateButton_endDate.Size = new System.Drawing.Size(20, 21);
            this.endDateButton_endDate.TabIndex = 17;
            this.endDateButton_endDate.Text = "E";
            this.toolTip.SetToolTip(this.endDateButton_endDate, "Date");
            this.endDateButton_endDate.UseVisualStyleBackColor = true;
            this.endDateButton_endDate.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // endDateTextBox
            // 
            this.endDateTextBox.BackColor = System.Drawing.Color.White;
            this.endDateTextBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateTextBox.Location = new System.Drawing.Point(4, 19);
            this.endDateTextBox.Name = "endDateTextBox";
            this.endDateTextBox.ReadOnly = true;
            this.endDateTextBox.Size = new System.Drawing.Size(80, 20);
            this.endDateTextBox.TabIndex = 13;
            this.endDateTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.endDateTextBox.Click += new System.EventHandler(this.xxxText_Click);
            this.endDateTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.endDateTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            // 
            // endDateButton_today
            // 
            this.endDateButton_today.Location = new System.Drawing.Point(121, 18);
            this.endDateButton_today.Name = "endDateButton_today";
            this.endDateButton_today.Size = new System.Drawing.Size(20, 21);
            this.endDateButton_today.TabIndex = 16;
            this.endDateButton_today.Text = "T";
            this.toolTip.SetToolTip(this.endDateButton_today, "Today");
            this.endDateButton_today.UseVisualStyleBackColor = true;
            this.endDateButton_today.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // endDateNumericUpDown_big
            // 
            this.endDateNumericUpDown_big.Location = new System.Drawing.Point(102, 19);
            this.endDateNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.endDateNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.endDateNumericUpDown_big.Name = "endDateNumericUpDown_big";
            this.endDateNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.endDateNumericUpDown_big.TabIndex = 15;
            this.endDateNumericUpDown_big.Tag = "0";
            this.endDateNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // endDateTimePicker
            // 
            this.endDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.endDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.endDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.endDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.endDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGreen;
            this.endDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.endDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateTimePicker.Location = new System.Drawing.Point(4, 19);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.Size = new System.Drawing.Size(79, 20);
            this.endDateTimePicker.TabIndex = 34;
            this.endDateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            // 
            // endDateNumericUpDown_small
            // 
            this.endDateNumericUpDown_small.Location = new System.Drawing.Point(85, 19);
            this.endDateNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.endDateNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.endDateNumericUpDown_small.Name = "endDateNumericUpDown_small";
            this.endDateNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.endDateNumericUpDown_small.TabIndex = 14;
            this.endDateNumericUpDown_small.Tag = "0";
            this.endDateNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "At Date";
            // 
            // resultsDataGridView
            // 
            this.resultsDataGridView.AllowUserToAddRows = false;
            this.resultsDataGridView.AllowUserToDeleteRows = false;
            this.resultsDataGridView.AllowUserToOrderColumns = true;
            this.resultsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsDataGridView.AutoGenerateColumns = false;
            this.resultsDataGridView.ColumnHeadersHeight = 22;
            this.resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.resultsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.resultsIndexColumn,
            this.resultsPriceColumn,
            this.resultsVolatilityColumn,
            this.resultsEndDateColumn,
            this.resultsProfitColumn,
            this.resultsProfitPrcColumn,
            this.resultsDeltaColumn,
            this.resultsGammaColumn,
            this.resultsThetaColumn,
            this.resultsVegaColumn});
            this.resultsDataGridView.DataSource = this.analysisTableBindingSource;
            this.resultsDataGridView.Location = new System.Drawing.Point(8, 19);
            this.resultsDataGridView.Name = "resultsDataGridView";
            this.resultsDataGridView.RowHeadersVisible = false;
            this.resultsDataGridView.RowHeadersWidth = 16;
            this.resultsDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultsDataGridView.RowTemplate.Height = 20;
            this.resultsDataGridView.Size = new System.Drawing.Size(660, 488);
            this.resultsDataGridView.TabIndex = 1;
            this.resultsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.resultsDataGridView_CellValueChanged);
            this.resultsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.resultsDataGridView_CellFormatting);
            this.resultsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.resultsDataGridView_DataError);
            // 
            // analysisTableBindingSource
            // 
            this.analysisTableBindingSource.DataMember = "AnalysisTable";
            this.analysisTableBindingSource.DataSource = this.analysisSet;
            // 
            // analysisSet
            // 
            this.analysisSet.DataSetName = "AnalysisSet";
            this.analysisSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // analysisSetBindingSource
            // 
            this.analysisSetBindingSource.DataSource = this.analysisSet;
            this.analysisSetBindingSource.Position = 0;
            // 
            // tableGroupBox
            // 
            this.tableGroupBox.Controls.Add(this.saveButton);
            this.tableGroupBox.Controls.Add(this.addRowButton);
            this.tableGroupBox.Controls.Add(this.deleteRowButton);
            this.tableGroupBox.Controls.Add(this.clearButton);
            this.tableGroupBox.Controls.Add(this.resultsDataGridView);
            this.tableGroupBox.Location = new System.Drawing.Point(12, 12);
            this.tableGroupBox.Name = "tableGroupBox";
            this.tableGroupBox.Size = new System.Drawing.Size(678, 570);
            this.tableGroupBox.TabIndex = 42;
            this.tableGroupBox.TabStop = false;
            this.tableGroupBox.Text = "Results Table";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(578, 514);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(90, 24);
            this.saveButton.TabIndex = 55;
            this.saveButton.TabStop = false;
            this.saveButton.Text = "Export to Excel";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // addRowButton
            // 
            this.addRowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addRowButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addRowButton.Location = new System.Drawing.Point(102, 514);
            this.addRowButton.Name = "addRowButton";
            this.addRowButton.Size = new System.Drawing.Size(90, 24);
            this.addRowButton.TabIndex = 26;
            this.addRowButton.Text = "Add Row";
            this.addRowButton.UseVisualStyleBackColor = true;
            this.addRowButton.Click += new System.EventHandler(this.addRowButton_Click);
            // 
            // deleteRowButton
            // 
            this.deleteRowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteRowButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteRowButton.Location = new System.Drawing.Point(6, 514);
            this.deleteRowButton.Name = "deleteRowButton";
            this.deleteRowButton.Size = new System.Drawing.Size(90, 24);
            this.deleteRowButton.TabIndex = 25;
            this.deleteRowButton.Text = "Delete Row";
            this.deleteRowButton.UseVisualStyleBackColor = true;
            this.deleteRowButton.Click += new System.EventHandler(this.deleteRowButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearButton.Location = new System.Drawing.Point(6, 540);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(90, 24);
            this.clearButton.TabIndex = 24;
            this.clearButton.Text = "Delete All";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // resoTextBox
            // 
            this.resoTextBox.Location = new System.Drawing.Point(10, 75);
            this.resoTextBox.Name = "resoTextBox";
            this.resoTextBox.Size = new System.Drawing.Size(80, 20);
            this.resoTextBox.TabIndex = 8;
            this.resoTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.resoTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.resoTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            this.resoTextBox.Enter += new System.EventHandler(this.xxxText_Enter);
            // 
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Location = new System.Drawing.Point(8, 59);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(29, 13);
            this.stepLabel.TabIndex = 1;
            this.stepLabel.Tag = "Step";
            this.stepLabel.Text = "Step";
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(10, 153);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(177, 24);
            this.updateButton.TabIndex = 44;
            this.updateButton.TabStop = false;
            this.updateButton.Text = "Auto Fill Results Table";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // toTextBox
            // 
            this.toTextBox.Location = new System.Drawing.Point(10, 117);
            this.toTextBox.Name = "toTextBox";
            this.toTextBox.Size = new System.Drawing.Size(80, 20);
            this.toTextBox.TabIndex = 8;
            this.toTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toTextBox.Click += new System.EventHandler(this.xxxText_Click);
            this.toTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.toTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            this.toTextBox.Enter += new System.EventHandler(this.xxxText_Enter);
            // 
            // toDateTimePicker
            // 
            this.toDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.toDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.toDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.toDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.toDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGreen;
            this.toDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.toDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDateTimePicker.Location = new System.Drawing.Point(10, 117);
            this.toDateTimePicker.Name = "toDateTimePicker";
            this.toDateTimePicker.Size = new System.Drawing.Size(80, 20);
            this.toDateTimePicker.TabIndex = 34;
            this.toDateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(8, 101);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(20, 13);
            this.toLabel.TabIndex = 1;
            this.toLabel.Tag = "To";
            this.toLabel.Text = "To";
            // 
            // fromTextBox
            // 
            this.fromTextBox.Location = new System.Drawing.Point(10, 34);
            this.fromTextBox.Name = "fromTextBox";
            this.fromTextBox.Size = new System.Drawing.Size(80, 20);
            this.fromTextBox.TabIndex = 8;
            this.fromTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.fromTextBox.Click += new System.EventHandler(this.xxxText_Click);
            this.fromTextBox.Leave += new System.EventHandler(this.xxxText_Leave);
            this.fromTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxText_KeyUp);
            this.fromTextBox.Enter += new System.EventHandler(this.xxxText_Enter);
            // 
            // fromDateTimePicker
            // 
            this.fromDateTimePicker.CalendarForeColor = System.Drawing.Color.Cornsilk;
            this.fromDateTimePicker.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.fromDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.DarkSeaGreen;
            this.fromDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.fromDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.LightGreen;
            this.fromDateTimePicker.CustomFormat = "d-MMM-yyyy";
            this.fromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDateTimePicker.Location = new System.Drawing.Point(10, 34);
            this.fromDateTimePicker.Name = "fromDateTimePicker";
            this.fromDateTimePicker.Size = new System.Drawing.Size(80, 20);
            this.fromDateTimePicker.TabIndex = 34;
            this.fromDateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(8, 19);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(30, 13);
            this.fromLabel.TabIndex = 1;
            this.fromLabel.Tag = "From";
            this.fromLabel.Text = "From";
            // 
            // scaleGroupBox
            // 
            this.scaleGroupBox.Controls.Add(this.fromNumericUpDown_big);
            this.scaleGroupBox.Controls.Add(this.button1);
            this.scaleGroupBox.Controls.Add(this.fromNumericUpDown_small);
            this.scaleGroupBox.Controls.Add(this.button2);
            this.scaleGroupBox.Controls.Add(this.updateButton);
            this.scaleGroupBox.Controls.Add(this.button3);
            this.scaleGroupBox.Controls.Add(this.resoNumericUpDown_big);
            this.scaleGroupBox.Controls.Add(this.fromTextBox);
            this.scaleGroupBox.Controls.Add(this.toTextBox);
            this.scaleGroupBox.Controls.Add(this.stepLabel);
            this.scaleGroupBox.Controls.Add(this.resoTextBox);
            this.scaleGroupBox.Controls.Add(this.resoNumericUpDown_small);
            this.scaleGroupBox.Controls.Add(this.fromDateTimePicker);
            this.scaleGroupBox.Controls.Add(this.toNumericUpDown_big);
            this.scaleGroupBox.Controls.Add(this.toLabel);
            this.scaleGroupBox.Controls.Add(this.toNumericUpDown_small);
            this.scaleGroupBox.Controls.Add(this.toDateTimePicker);
            this.scaleGroupBox.Controls.Add(this.fromLabel);
            this.scaleGroupBox.Location = new System.Drawing.Point(696, 279);
            this.scaleGroupBox.Name = "scaleGroupBox";
            this.scaleGroupBox.Size = new System.Drawing.Size(199, 190);
            this.scaleGroupBox.TabIndex = 47;
            this.scaleGroupBox.TabStop = false;
            this.scaleGroupBox.Text = "Scale";
            // 
            // fromNumericUpDown_big
            // 
            this.fromNumericUpDown_big.Location = new System.Drawing.Point(91, 34);
            this.fromNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.fromNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.fromNumericUpDown_big.Name = "fromNumericUpDown_big";
            this.fromNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.fromNumericUpDown_big.TabIndex = 54;
            this.fromNumericUpDown_big.Tag = "0";
            this.fromNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 24);
            this.button1.TabIndex = 48;
            this.button1.TabStop = false;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // fromNumericUpDown_small
            // 
            this.fromNumericUpDown_small.Location = new System.Drawing.Point(108, 34);
            this.fromNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.fromNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.fromNumericUpDown_small.Name = "fromNumericUpDown_small";
            this.fromNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.fromNumericUpDown_small.TabIndex = 53;
            this.fromNumericUpDown_small.Tag = "0";
            this.fromNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(500, 443);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 24);
            this.button2.TabIndex = 47;
            this.button2.TabStop = false;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(500, 413);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 24);
            this.button3.TabIndex = 45;
            this.button3.TabStop = false;
            this.button3.Text = "Save...";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // resoNumericUpDown_big
            // 
            this.resoNumericUpDown_big.Location = new System.Drawing.Point(91, 75);
            this.resoNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.resoNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.resoNumericUpDown_big.Name = "resoNumericUpDown_big";
            this.resoNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.resoNumericUpDown_big.TabIndex = 52;
            this.resoNumericUpDown_big.Tag = "0";
            this.resoNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // resoNumericUpDown_small
            // 
            this.resoNumericUpDown_small.Location = new System.Drawing.Point(108, 75);
            this.resoNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.resoNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.resoNumericUpDown_small.Name = "resoNumericUpDown_small";
            this.resoNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.resoNumericUpDown_small.TabIndex = 51;
            this.resoNumericUpDown_small.Tag = "0";
            this.resoNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // toNumericUpDown_big
            // 
            this.toNumericUpDown_big.Location = new System.Drawing.Point(91, 117);
            this.toNumericUpDown_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.toNumericUpDown_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.toNumericUpDown_big.Name = "toNumericUpDown_big";
            this.toNumericUpDown_big.Size = new System.Drawing.Size(18, 20);
            this.toNumericUpDown_big.TabIndex = 50;
            this.toNumericUpDown_big.Tag = "0";
            this.toNumericUpDown_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // toNumericUpDown_small
            // 
            this.toNumericUpDown_small.Location = new System.Drawing.Point(108, 117);
            this.toNumericUpDown_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.toNumericUpDown_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.toNumericUpDown_small.Name = "toNumericUpDown_small";
            this.toNumericUpDown_small.Size = new System.Drawing.Size(18, 20);
            this.toNumericUpDown_small.TabIndex = 49;
            this.toNumericUpDown_small.Tag = "0";
            this.toNumericUpDown_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Price";
            this.dataGridViewTextBoxColumn1.HeaderText = "Price";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 65;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Volatility";
            this.dataGridViewTextBoxColumn2.HeaderText = "Volatility %";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 65;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "EndDate";
            this.dataGridViewTextBoxColumn3.HeaderText = "End Date";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Profit";
            this.dataGridViewTextBoxColumn4.HeaderText = "Profit";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 77;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "ProfitPrc";
            this.dataGridViewTextBoxColumn5.HeaderText = "Profit %";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 60;
            // 
            // resultsIndexColumn
            // 
            this.resultsIndexColumn.DataPropertyName = "Index";
            this.resultsIndexColumn.HeaderText = "Key";
            this.resultsIndexColumn.Name = "resultsIndexColumn";
            this.resultsIndexColumn.Visible = false;
            // 
            // resultsPriceColumn
            // 
            this.resultsPriceColumn.DataPropertyName = "Price";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N2";
            this.resultsPriceColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.resultsPriceColumn.HeaderText = "Underlying";
            this.resultsPriceColumn.Name = "resultsPriceColumn";
            this.resultsPriceColumn.Width = 75;
            // 
            // resultsVolatilityColumn
            // 
            this.resultsVolatilityColumn.DataPropertyName = "Volatility";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "P2";
            dataGridViewCellStyle2.NullValue = "Option IV";
            this.resultsVolatilityColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.resultsVolatilityColumn.HeaderText = "Volatility %";
            this.resultsVolatilityColumn.Name = "resultsVolatilityColumn";
            this.resultsVolatilityColumn.Width = 70;
            // 
            // resultsEndDateColumn
            // 
            this.resultsEndDateColumn.DataPropertyName = "EndDate";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "dd-MMM-yy";
            this.resultsEndDateColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.resultsEndDateColumn.HeaderText = "Date";
            this.resultsEndDateColumn.Name = "resultsEndDateColumn";
            this.resultsEndDateColumn.Width = 70;
            // 
            // resultsProfitColumn
            // 
            this.resultsProfitColumn.DataPropertyName = "Profit";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.resultsProfitColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.resultsProfitColumn.HeaderText = "Profit";
            this.resultsProfitColumn.Name = "resultsProfitColumn";
            this.resultsProfitColumn.ReadOnly = true;
            this.resultsProfitColumn.Width = 70;
            // 
            // resultsProfitPrcColumn
            // 
            this.resultsProfitPrcColumn.DataPropertyName = "ProfitPrc";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "P2";
            this.resultsProfitPrcColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.resultsProfitPrcColumn.HeaderText = "Profit %";
            this.resultsProfitPrcColumn.Name = "resultsProfitPrcColumn";
            this.resultsProfitPrcColumn.ReadOnly = true;
            this.resultsProfitPrcColumn.Width = 70;
            // 
            // resultsDeltaColumn
            // 
            this.resultsDeltaColumn.DataPropertyName = "Delta";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            this.resultsDeltaColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.resultsDeltaColumn.HeaderText = "Delta";
            this.resultsDeltaColumn.Name = "resultsDeltaColumn";
            this.resultsDeltaColumn.Width = 70;
            // 
            // resultsGammaColumn
            // 
            this.resultsGammaColumn.DataPropertyName = "Gamma";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            this.resultsGammaColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.resultsGammaColumn.HeaderText = "Gamma";
            this.resultsGammaColumn.Name = "resultsGammaColumn";
            this.resultsGammaColumn.Width = 70;
            // 
            // resultsThetaColumn
            // 
            this.resultsThetaColumn.DataPropertyName = "Theta";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            this.resultsThetaColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.resultsThetaColumn.HeaderText = "Theta [Day]";
            this.resultsThetaColumn.Name = "resultsThetaColumn";
            this.resultsThetaColumn.Width = 70;
            // 
            // resultsVegaColumn
            // 
            this.resultsVegaColumn.DataPropertyName = "Vega";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N2";
            this.resultsVegaColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.resultsVegaColumn.HeaderText = "Vega [% Vol]";
            this.resultsVegaColumn.Name = "resultsVegaColumn";
            this.resultsVegaColumn.Width = 71;
            // 
            // AnalysisForm
            // 
            this.ClientSize = new System.Drawing.Size(907, 586);
            this.Controls.Add(this.scaleGroupBox);
            this.Controls.Add(this.tableGroupBox);
            this.Controls.Add(this.parameterGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AnalysisForm";
            this.Tag = "Strategy Analysis";
            this.Text = "Strategy Analysis";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AnalysisForm_FormClosed);
            this.parameterGroupBox.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.underlyingPanel.ResumeLayout(false);
            this.underlyingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.undelyingNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.undelyingNumericUpDown_small)).EndInit();
            this.volatilityPanel.ResumeLayout(false);
            this.volatilityPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volatilityNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volatilityNumericUpDown_small)).EndInit();
            this.endDatePanel.ResumeLayout(false);
            this.endDatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endDateNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateNumericUpDown_small)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisSetBindingSource)).EndInit();
            this.tableGroupBox.ResumeLayout(false);
            this.scaleGroupBox.ResumeLayout(false);
            this.scaleGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fromNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromNumericUpDown_small)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resoNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resoNumericUpDown_small)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toNumericUpDown_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toNumericUpDown_small)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.GroupBox parameterGroupBox;
        private System.Windows.Forms.Panel volatilityPanel;
        private System.Windows.Forms.Button volatilityButton_optionImplied;
        private System.Windows.Forms.Button volatilityButton_stockHistorical;
        private System.Windows.Forms.Button volatilityButton_stockImplied;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox underlyingTextBox;
        private System.Windows.Forms.Panel endDatePanel;
        private System.Windows.Forms.Button endDateButton_endDate;
        private System.Windows.Forms.Button endDateButton_today;
        private System.Windows.Forms.NumericUpDown endDateNumericUpDown_big;
        private System.Windows.Forms.DateTimePicker endDateTimePicker;
        private System.Windows.Forms.NumericUpDown endDateNumericUpDown_small;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.DataGridView resultsDataGridView;
        private OptionsOracle.Data.AnalysisSet analysisSet;
        private System.Windows.Forms.BindingSource analysisTableBindingSource;
        private System.Windows.Forms.BindingSource analysisSetBindingSource;
        private System.Windows.Forms.GroupBox tableGroupBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.DateTimePicker toDateTimePicker;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.DateTimePicker fromDateTimePicker;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.Panel underlyingPanel;
        private System.Windows.Forms.Button undelyingButton_last;
        private System.Windows.Forms.NumericUpDown undelyingNumericUpDown_big;
        private System.Windows.Forms.NumericUpDown undelyingNumericUpDown_small;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox endDateTextBox;
        private System.Windows.Forms.NumericUpDown volatilityNumericUpDown_big;
        private System.Windows.Forms.NumericUpDown volatilityNumericUpDown_small;
        private System.Windows.Forms.TextBox volatilityTextBox;
        private System.Windows.Forms.TextBox resoTextBox;
        private System.Windows.Forms.TextBox toTextBox;
        private System.Windows.Forms.TextBox fromTextBox;
        private System.Windows.Forms.GroupBox scaleGroupBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.NumericUpDown fromNumericUpDown_big;
        private System.Windows.Forms.NumericUpDown fromNumericUpDown_small;
        private System.Windows.Forms.NumericUpDown resoNumericUpDown_big;
        private System.Windows.Forms.NumericUpDown resoNumericUpDown_small;
        private System.Windows.Forms.NumericUpDown toNumericUpDown_big;
        private System.Windows.Forms.NumericUpDown toNumericUpDown_small;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addRowButton;
        private System.Windows.Forms.Button deleteRowButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsIndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsPriceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsVolatilityColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsEndDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsProfitColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsProfitPrcColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsDeltaColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsGammaColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsThetaColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultsVegaColumn;
    }
}