namespace OptionsOracle.Forms
{
    partial class VolatilitySmileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolatilitySmileForm));
            this.volatilityGraph = new ZedGraph.ZedGraphControl();
            this.volatilitytoolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDefaultScaleButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.invertColorsToolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatus1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripFilterButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.volatilitytoolStrip.SuspendLayout();
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
            this.volatilityGraph.Location = new System.Drawing.Point(2, 1);
            this.volatilityGraph.Name = "volatilityGraph";
            this.volatilityGraph.ScrollGrace = 0;
            this.volatilityGraph.ScrollMaxX = 0;
            this.volatilityGraph.ScrollMaxY = 0;
            this.volatilityGraph.ScrollMaxY2 = 0;
            this.volatilityGraph.ScrollMinX = 0;
            this.volatilityGraph.ScrollMinY = 0;
            this.volatilityGraph.ScrollMinY2 = 0;
            this.volatilityGraph.Size = new System.Drawing.Size(640, 381);
            this.volatilityGraph.TabIndex = 1;
            this.volatilityGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.xxxGraph_MouseMoveEvent);
            this.volatilityGraph.MouseEnter += new System.EventHandler(this.xxxGraph_MouseEnter);
            this.volatilityGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxGraph_KeyUp);
            this.volatilityGraph.MouseLeave += new System.EventHandler(this.xxxGraph_MouseLeave);
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
            this.toolStripSeparator1,
            this.toolStripStatus1,
            this.toolStripLabel1});
            this.volatilitytoolStrip.Location = new System.Drawing.Point(2, 385);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // VolatilitySmileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 412);
            this.Controls.Add(this.volatilitytoolStrip);
            this.Controls.Add(this.volatilityGraph);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "VolatilitySmileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Volatility Smile";
            this.Text = "Volatility Smile";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VolatilitySmileForm_KeyUp);
            this.volatilitytoolStrip.ResumeLayout(false);
            this.volatilitytoolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl volatilityGraph;
        private System.Windows.Forms.ToolStrip volatilitytoolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton invertColorsToolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripStatus1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripFilterButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}