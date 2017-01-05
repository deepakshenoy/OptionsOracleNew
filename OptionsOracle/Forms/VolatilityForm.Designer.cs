namespace OptionsOracle.Forms
{
    partial class VolatilityForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolatilityForm));
            this.volatilityGraph = new ZedGraph.ZedGraphControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.volatilitytoolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDefaultScaleButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.invertColorsToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFilterButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatus1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.historyGraph = new ZedGraph.ZedGraphControl();
            this.historyToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDefaultScaleButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.invertColorsToolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatus2 = new System.Windows.Forms.ToolStripLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.volatilitytoolStrip.SuspendLayout();
            this.historyToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // volatilityGraph
            // 
            this.volatilityGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.volatilityGraph.AutoSize = true;
            this.volatilityGraph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.volatilityGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.volatilityGraph.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volatilityGraph.ForeColor = System.Drawing.Color.Cornsilk;
            this.volatilityGraph.Location = new System.Drawing.Point(3, 3);
            this.volatilityGraph.Name = "volatilityGraph";
            this.volatilityGraph.ScrollGrace = 0;
            this.volatilityGraph.ScrollMaxX = 0;
            this.volatilityGraph.ScrollMaxY = 0;
            this.volatilityGraph.ScrollMaxY2 = 0;
            this.volatilityGraph.ScrollMinX = 0;
            this.volatilityGraph.ScrollMinY = 0;
            this.volatilityGraph.ScrollMinY2 = 0;
            this.volatilityGraph.Size = new System.Drawing.Size(874, 300);
            this.volatilityGraph.TabIndex = 1;
            this.volatilityGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.xxxGraph_MouseMoveEvent);
            this.volatilityGraph.MouseEnter += new System.EventHandler(this.xxxGraph_MouseEnter);
            this.volatilityGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxGraph_KeyUp);
            this.volatilityGraph.MouseLeave += new System.EventHandler(this.xxxGraph_MouseLeave);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(4, 1);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.volatilitytoolStrip);
            this.splitContainer1.Panel1.Controls.Add(this.volatilityGraph);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.historyGraph);
            this.splitContainer1.Panel2.Controls.Add(this.historyToolStrip);
            this.splitContainer1.Size = new System.Drawing.Size(880, 666);
            this.splitContainer1.SplitterDistance = 333;
            this.splitContainer1.TabIndex = 3;
            // 
            // volatilitytoolStrip
            // 
            this.volatilitytoolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volatilitytoolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.volatilitytoolStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volatilitytoolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator4,
            this.toolStripDefaultScaleButton1,
            this.toolStripSeparator5,
            this.invertColorsToolStripButton1,
            this.toolStripSeparator6,
            this.toolStripFilterButton1,
            this.toolStripSeparator7,
            this.toolStripStatus1,
            this.toolStripLabel1});
            this.volatilitytoolStrip.Location = new System.Drawing.Point(3, 306);
            this.volatilitytoolStrip.Name = "volatilitytoolStrip";
            this.volatilitytoolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.volatilitytoolStrip.Size = new System.Drawing.Size(256, 27);
            this.volatilitytoolStrip.TabIndex = 2;
            this.volatilitytoolStrip.TabStop = true;
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDefaultScaleButton1
            // 
            this.toolStripDefaultScaleButton1.AutoSize = false;
            this.toolStripDefaultScaleButton1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDefaultScaleButton1.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton1.Name = "toolStripDefaultScaleButton1";
            this.toolStripDefaultScaleButton1.Size = new System.Drawing.Size(70, 24);
            this.toolStripDefaultScaleButton1.Text = "Auto Scale";
            this.toolStripDefaultScaleButton1.Click += new System.EventHandler(this.xxxGraph_ScaleToDefault);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // invertColorsToolStripButton1
            // 
            this.invertColorsToolStripButton1.AutoSize = false;
            this.invertColorsToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.invertColorsToolStripButton1.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.invertColorsToolStripButton1.Name = "invertColorsToolStripButton1";
            this.invertColorsToolStripButton1.Size = new System.Drawing.Size(70, 24);
            this.invertColorsToolStripButton1.Text = "Invert Colors";
            this.invertColorsToolStripButton1.Click += new System.EventHandler(this.xxxInvertColorsToolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripFilterButton1
            // 
            this.toolStripFilterButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripFilterButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFilterButton1.Image")));
            this.toolStripFilterButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFilterButton1.Name = "toolStripFilterButton1";
            this.toolStripFilterButton1.Size = new System.Drawing.Size(81, 24);
            this.toolStripFilterButton1.Text = "Options Filter...";
            this.toolStripFilterButton1.ToolTipText = "Displayed Options Filter...";
            this.toolStripFilterButton1.Click += new System.EventHandler(this.toolStripFilterButton1_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripStatus1
            // 
            this.toolStripStatus1.Name = "toolStripStatus1";
            this.toolStripStatus1.Size = new System.Drawing.Size(0, 24);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 24);
            // 
            // historyGraph
            // 
            this.historyGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.historyGraph.AutoSize = true;
            this.historyGraph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.historyGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.historyGraph.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historyGraph.ForeColor = System.Drawing.Color.Cornsilk;
            this.historyGraph.Location = new System.Drawing.Point(3, 1);
            this.historyGraph.Name = "historyGraph";
            this.historyGraph.ScrollGrace = 0;
            this.historyGraph.ScrollMaxX = 0;
            this.historyGraph.ScrollMaxY = 0;
            this.historyGraph.ScrollMaxY2 = 0;
            this.historyGraph.ScrollMinX = 0;
            this.historyGraph.ScrollMinY = 0;
            this.historyGraph.ScrollMinY2 = 0;
            this.historyGraph.Size = new System.Drawing.Size(874, 300);
            this.historyGraph.TabIndex = 3;
            this.historyGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.xxxGraph_MouseMoveEvent);
            this.historyGraph.MouseEnter += new System.EventHandler(this.xxxGraph_MouseEnter);
            this.historyGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxGraph_KeyUp);
            this.historyGraph.MouseLeave += new System.EventHandler(this.xxxGraph_MouseLeave);
            // 
            // historyToolStrip
            // 
            this.historyToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.historyToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.historyToolStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historyToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripDefaultScaleButton2,
            this.toolStripSeparator2,
            this.invertColorsToolStripButton2,
            this.toolStripSeparator3,
            this.toolStripStatus2});
            this.historyToolStrip.Location = new System.Drawing.Point(3, 302);
            this.historyToolStrip.Name = "historyToolStrip";
            this.historyToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.historyToolStrip.Size = new System.Drawing.Size(169, 27);
            this.historyToolStrip.TabIndex = 4;
            this.historyToolStrip.TabStop = true;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDefaultScaleButton2
            // 
            this.toolStripDefaultScaleButton2.AutoSize = false;
            this.toolStripDefaultScaleButton2.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDefaultScaleButton2.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton2.Name = "toolStripDefaultScaleButton2";
            this.toolStripDefaultScaleButton2.Size = new System.Drawing.Size(70, 24);
            this.toolStripDefaultScaleButton2.Text = "Auto Scale";
            this.toolStripDefaultScaleButton2.Click += new System.EventHandler(this.xxxGraph_ScaleToDefault);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // invertColorsToolStripButton2
            // 
            this.invertColorsToolStripButton2.AutoSize = false;
            this.invertColorsToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.invertColorsToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("invertColorsToolStripButton2.Image")));
            this.invertColorsToolStripButton2.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.invertColorsToolStripButton2.Name = "invertColorsToolStripButton2";
            this.invertColorsToolStripButton2.Size = new System.Drawing.Size(70, 24);
            this.invertColorsToolStripButton2.Text = "Invert Colors";
            this.invertColorsToolStripButton2.Click += new System.EventHandler(this.xxxInvertColorsToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripStatus2
            // 
            this.toolStripStatus2.Name = "toolStripStatus2";
            this.toolStripStatus2.Size = new System.Drawing.Size(0, 24);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // VolatilityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 672);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "VolatilityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Historical Prices & Volatility Cone";
            this.Text = "Historical Prices & Volatility Cone";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VolatilityForm_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.volatilitytoolStrip.ResumeLayout(false);
            this.volatilitytoolStrip.PerformLayout();
            this.historyToolStrip.ResumeLayout(false);
            this.historyToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl volatilityGraph;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ZedGraph.ZedGraphControl historyGraph;
        private System.Windows.Forms.ToolStrip historyToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton invertColorsToolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripStatus2;
        private System.Windows.Forms.ToolStrip volatilitytoolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton invertColorsToolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripStatus1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolStripFilterButton1;
    }
}