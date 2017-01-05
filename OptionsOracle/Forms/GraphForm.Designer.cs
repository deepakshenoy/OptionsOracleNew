namespace OptionsOracle.Forms
{
    partial class GraphForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            this.zedGraph = new ZedGraph.ZedGraphControl();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripShowDetailsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripShowStdDevButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDefaultScaleButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.invertColorsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatusXY = new System.Windows.Forms.ToolStripLabel();
            this.numericUpDown2_big = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2_small = new System.Windows.Forms.NumericUpDown();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2_right = new System.Windows.Forms.Button();
            this.button2_left = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.button1_left = new System.Windows.Forms.Button();
            this.button1_middle = new System.Windows.Forms.Button();
            this.typeButton = new System.Windows.Forms.Button();
            this.button1_right = new System.Windows.Forms.Button();
            this.numericUpDown1_big = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1_small = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stockPriceTextBox = new System.Windows.Forms.TextBox();
            this.volatilityTextBox = new System.Windows.Forms.TextBox();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.modeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_small)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_big)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_small)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedGraph
            // 
            this.zedGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraph.AutoSize = true;
            this.zedGraph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.zedGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraph.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zedGraph.ForeColor = System.Drawing.Color.Cornsilk;
            this.zedGraph.LinkModifierKeys = System.Windows.Forms.Keys.None;
            this.zedGraph.Location = new System.Drawing.Point(1, 33);
            this.zedGraph.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.zedGraph.Name = "zedGraph";
            this.zedGraph.ScrollGrace = 0D;
            this.zedGraph.ScrollMaxX = 0D;
            this.zedGraph.ScrollMaxY = 0D;
            this.zedGraph.ScrollMaxY2 = 0D;
            this.zedGraph.ScrollMinX = 0D;
            this.zedGraph.ScrollMinY = 0D;
            this.zedGraph.ScrollMinY2 = 0D;
            this.zedGraph.Size = new System.Drawing.Size(1040, 514);
            this.zedGraph.TabIndex = 1;
            this.zedGraph.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.zedGraph_ContextMenuBuilder);
            this.zedGraph.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedGraph_ZoomEvent);
            this.zedGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraph_MouseMoveEvent);
            this.zedGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.zedGraph_KeyUp);
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripShowDetailsButton,
            this.toolStripSeparator1,
            this.toolStripShowStdDevButton,
            this.toolStripSeparator2,
            this.toolStripDefaultScaleButton,
            this.toolStripSeparator3,
            this.invertColorsToolStripButton,
            this.toolStripSeparator4,
            this.toolStripStatusXY});
            this.toolStrip.Location = new System.Drawing.Point(0, 555);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(336, 27);
            this.toolStrip.TabIndex = 14;
            // 
            // toolStripShowDetailsButton
            // 
            this.toolStripShowDetailsButton.AutoSize = false;
            this.toolStripShowDetailsButton.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripShowDetailsButton.CheckOnClick = true;
            this.toolStripShowDetailsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripShowDetailsButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripShowDetailsButton.Name = "toolStripShowDetailsButton";
            this.toolStripShowDetailsButton.Size = new System.Drawing.Size(75, 24);
            this.toolStripShowDetailsButton.Text = "Show Positions";
            this.toolStripShowDetailsButton.ToolTipText = "Show/Hide Position Details";
            this.toolStripShowDetailsButton.CheckStateChanged += new System.EventHandler(this.toolStripShowDetailsButton_CheckStateChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripShowStdDevButton
            // 
            this.toolStripShowStdDevButton.AutoSize = false;
            this.toolStripShowStdDevButton.CheckOnClick = true;
            this.toolStripShowStdDevButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripShowStdDevButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripShowStdDevButton.Name = "toolStripShowStdDevButton";
            this.toolStripShowStdDevButton.Size = new System.Drawing.Size(75, 24);
            this.toolStripShowStdDevButton.Text = "Show StdDev";
            this.toolStripShowStdDevButton.ToolTipText = "Show Underlying StdDev";
            this.toolStripShowStdDevButton.CheckStateChanged += new System.EventHandler(this.toolStripShowStdDevButton_CheckStateChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDefaultScaleButton
            // 
            this.toolStripDefaultScaleButton.AutoSize = false;
            this.toolStripDefaultScaleButton.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDefaultScaleButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDefaultScaleButton.Image")));
            this.toolStripDefaultScaleButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton.Name = "toolStripDefaultScaleButton";
            this.toolStripDefaultScaleButton.Size = new System.Drawing.Size(75, 24);
            this.toolStripDefaultScaleButton.Text = "Auto Scale";
            this.toolStripDefaultScaleButton.Click += new System.EventHandler(this.toolStripDefaultScaleButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // invertColorsToolStripButton
            // 
            this.invertColorsToolStripButton.AutoSize = false;
            this.invertColorsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.invertColorsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("invertColorsToolStripButton.Image")));
            this.invertColorsToolStripButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.invertColorsToolStripButton.Name = "invertColorsToolStripButton";
            this.invertColorsToolStripButton.Size = new System.Drawing.Size(75, 24);
            this.invertColorsToolStripButton.Text = "Invert Colors";
            this.invertColorsToolStripButton.Click += new System.EventHandler(this.invertColorsToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripStatusXY
            // 
            this.toolStripStatusXY.Name = "toolStripStatusXY";
            this.toolStripStatusXY.Size = new System.Drawing.Size(0, 24);
            // 
            // numericUpDown2_big
            // 
            this.numericUpDown2_big.Location = new System.Drawing.Point(216, 0);
            this.numericUpDown2_big.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDown2_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown2_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown2_big.Name = "numericUpDown2_big";
            this.numericUpDown2_big.Size = new System.Drawing.Size(24, 22);
            this.numericUpDown2_big.TabIndex = 10;
            this.numericUpDown2_big.TabStop = false;
            this.numericUpDown2_big.Tag = "0";
            this.numericUpDown2_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // numericUpDown2_small
            // 
            this.numericUpDown2_small.Location = new System.Drawing.Point(193, 0);
            this.numericUpDown2_small.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDown2_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown2_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown2_small.Name = "numericUpDown2_small";
            this.numericUpDown2_small.Size = new System.Drawing.Size(24, 22);
            this.numericUpDown2_small.TabIndex = 9;
            this.numericUpDown2_small.TabStop = false;
            this.numericUpDown2_small.Tag = "0";
            this.numericUpDown2_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.Cornsilk;
            this.textBox2.Location = new System.Drawing.Point(85, 0);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(104, 23);
            this.textBox2.TabIndex = 8;
            this.textBox2.TabStop = false;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.Click += new System.EventHandler(this.textBoxX_Click);
            this.textBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxX_KeyUp);
            this.textBox2.Leave += new System.EventHandler(this.textBoxX_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 4);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "End Date";
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
            this.endDateTimePicker.Location = new System.Drawing.Point(85, 0);
            this.endDateTimePicker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.Size = new System.Drawing.Size(104, 22);
            this.endDateTimePicker.TabIndex = 34;
            this.endDateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.button2_right);
            this.panel2.Controls.Add(this.button2_left);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.numericUpDown2_big);
            this.panel2.Controls.Add(this.endDateTimePicker);
            this.panel2.Controls.Add(this.numericUpDown2_small);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(747, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(295, 26);
            this.panel2.TabIndex = 1;
            // 
            // button2_right
            // 
            this.button2_right.Location = new System.Drawing.Point(268, 0);
            this.button2_right.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2_right.Name = "button2_right";
            this.button2_right.Size = new System.Drawing.Size(27, 26);
            this.button2_right.TabIndex = 12;
            this.button2_right.TabStop = false;
            this.button2_right.Text = "E";
            this.toolTip.SetToolTip(this.button2_right, "End-Date");
            this.button2_right.UseVisualStyleBackColor = true;
            this.button2_right.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // button2_left
            // 
            this.button2_left.Location = new System.Drawing.Point(241, 0);
            this.button2_left.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2_left.Name = "button2_left";
            this.button2_left.Size = new System.Drawing.Size(27, 26);
            this.button2_left.TabIndex = 11;
            this.button2_left.TabStop = false;
            this.button2_left.Text = "T";
            this.toolTip.SetToolTip(this.button2_left, "Today");
            this.button2_left.UseVisualStyleBackColor = true;
            this.button2_left.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // button1_left
            // 
            this.button1_left.Location = new System.Drawing.Point(241, 0);
            this.button1_left.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1_left.Name = "button1_left";
            this.button1_left.Size = new System.Drawing.Size(27, 26);
            this.button1_left.TabIndex = 6;
            this.button1_left.TabStop = false;
            this.button1_left.Text = "I";
            this.toolTip.SetToolTip(this.button1_left, "Stock Average Implied Volatility");
            this.button1_left.UseVisualStyleBackColor = true;
            this.button1_left.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // button1_middle
            // 
            this.button1_middle.Location = new System.Drawing.Point(268, 0);
            this.button1_middle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1_middle.Name = "button1_middle";
            this.button1_middle.Size = new System.Drawing.Size(27, 26);
            this.button1_middle.TabIndex = 7;
            this.button1_middle.TabStop = false;
            this.button1_middle.Text = "H";
            this.toolTip.SetToolTip(this.button1_middle, "Stock Historical Volatility");
            this.button1_middle.UseVisualStyleBackColor = true;
            this.button1_middle.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // typeButton
            // 
            this.typeButton.Location = new System.Drawing.Point(284, 2);
            this.typeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.typeButton.Name = "typeButton";
            this.typeButton.Size = new System.Drawing.Size(27, 26);
            this.typeButton.TabIndex = 2;
            this.typeButton.TabStop = false;
            this.typeButton.Text = "%";
            this.toolTip.SetToolTip(this.typeButton, "Percentage / Value");
            this.typeButton.UseVisualStyleBackColor = true;
            this.typeButton.Click += new System.EventHandler(this.typeButton_Click);
            // 
            // button1_right
            // 
            this.button1_right.Location = new System.Drawing.Point(295, 0);
            this.button1_right.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1_right.Name = "button1_right";
            this.button1_right.Size = new System.Drawing.Size(27, 26);
            this.button1_right.TabIndex = 8;
            this.button1_right.TabStop = false;
            this.button1_right.Text = "X";
            this.toolTip.SetToolTip(this.button1_right, "Option Specific Implied Volatility");
            this.button1_right.UseVisualStyleBackColor = true;
            this.button1_right.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // numericUpDown1_big
            // 
            this.numericUpDown1_big.Location = new System.Drawing.Point(216, 0);
            this.numericUpDown1_big.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDown1_big.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1_big.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown1_big.Name = "numericUpDown1_big";
            this.numericUpDown1_big.Size = new System.Drawing.Size(24, 22);
            this.numericUpDown1_big.TabIndex = 5;
            this.numericUpDown1_big.TabStop = false;
            this.numericUpDown1_big.Tag = "0";
            this.numericUpDown1_big.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // numericUpDown1_small
            // 
            this.numericUpDown1_small.Location = new System.Drawing.Point(193, 0);
            this.numericUpDown1_small.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDown1_small.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1_small.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown1_small.Name = "numericUpDown1_small";
            this.numericUpDown1_small.Size = new System.Drawing.Size(24, 22);
            this.numericUpDown1_small.TabIndex = 4;
            this.numericUpDown1_small.TabStop = false;
            this.numericUpDown1_small.Tag = "0";
            this.numericUpDown1_small.ValueChanged += new System.EventHandler(this.numericUpDownX_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.button1_right);
            this.panel1.Controls.Add(this.button1_middle);
            this.panel1.Controls.Add(this.button1_left);
            this.panel1.Controls.Add(this.numericUpDown1_big);
            this.panel1.Controls.Add(this.numericUpDown1_small);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.stockPriceTextBox);
            this.panel1.Controls.Add(this.volatilityTextBox);
            this.panel1.Location = new System.Drawing.Point(417, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(321, 26);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Volatility";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Cornsilk;
            this.textBox1.Location = new System.Drawing.Point(85, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(104, 23);
            this.textBox1.TabIndex = 3;
            this.textBox1.TabStop = false;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.Click += new System.EventHandler(this.textBoxX_Click);
            this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxX_KeyUp);
            this.textBox1.Leave += new System.EventHandler(this.textBoxX_Leave);
            // 
            // stockPriceTextBox
            // 
            this.stockPriceTextBox.Location = new System.Drawing.Point(143, 0);
            this.stockPriceTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.stockPriceTextBox.Name = "stockPriceTextBox";
            this.stockPriceTextBox.Size = new System.Drawing.Size(44, 22);
            this.stockPriceTextBox.TabIndex = 4;
            this.stockPriceTextBox.Visible = false;
            this.stockPriceTextBox.TextChanged += new System.EventHandler(this.stockPriceTextBox_TextChanged);
            // 
            // volatilityTextBox
            // 
            this.volatilityTextBox.Location = new System.Drawing.Point(85, 0);
            this.volatilityTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.volatilityTextBox.Name = "volatilityTextBox";
            this.volatilityTextBox.Size = new System.Drawing.Size(51, 22);
            this.volatilityTextBox.TabIndex = 5;
            this.volatilityTextBox.Visible = false;
            this.volatilityTextBox.TextChanged += new System.EventHandler(this.volatilityTextBox_TextChanged);
            // 
            // modeComboBox
            // 
            this.modeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.modeComboBox.ForeColor = System.Drawing.Color.Cornsilk;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Profit/Loss Vs. Underlying",
            "Profit/Loss Vs. End-Date",
            "Profit/Loss Vs. Volatility"});
            this.modeComboBox.Location = new System.Drawing.Point(49, 2);
            this.modeComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(225, 24);
            this.modeComboBox.TabIndex = 1;
            this.modeComboBox.Text = "Profit/Loss Vs. Underlying";
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // modeTextBox
            // 
            this.modeTextBox.BackColor = System.Drawing.Color.Black;
            this.modeTextBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modeTextBox.ForeColor = System.Drawing.Color.Cornsilk;
            this.modeTextBox.Location = new System.Drawing.Point(49, 4);
            this.modeTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.modeTextBox.Name = "modeTextBox";
            this.modeTextBox.ReadOnly = true;
            this.modeTextBox.ShortcutsEnabled = false;
            this.modeTextBox.Size = new System.Drawing.Size(200, 23);
            this.modeTextBox.TabIndex = 1;
            this.modeTextBox.TabStop = false;
            this.modeTextBox.Text = "Profit/Loss Vs. Underlying";
            this.modeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Mode";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(318, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1043, 581);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.typeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.modeTextBox);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.zedGraph);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Strategy Graph";
            this.Text = "Strategy Graph";
            this.Activated += new System.EventHandler(this.GraphForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GraphForm_FormClosed);
            this.Shown += new System.EventHandler(this.GraphForm_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphForm_KeyUp);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_small)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_big)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_small)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraph;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripStatusXY;
        private System.Windows.Forms.ToolStripButton toolStripShowDetailsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.NumericUpDown numericUpDown2_big;
        private System.Windows.Forms.NumericUpDown numericUpDown2_small;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker endDateTimePicker;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button2_left;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1_left;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1_big;
        private System.Windows.Forms.NumericUpDown numericUpDown1_small;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1_middle;
        private System.Windows.Forms.TextBox volatilityTextBox;
        private System.Windows.Forms.TextBox stockPriceTextBox;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.TextBox modeTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton invertColorsToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Button typeButton;
        private System.Windows.Forms.ToolStripButton toolStripShowStdDevButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Button button1_right;
        private System.Windows.Forms.Button button2_right;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timer1;
    }
}