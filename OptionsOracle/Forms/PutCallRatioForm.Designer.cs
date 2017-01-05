namespace OptionsOracle.Forms
{
    partial class PutCallRatioForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PutCallRatioForm));
            this.putCallRatioGraph = new ZedGraph.ZedGraphControl();
            this.putCallRatioToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripModeButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tscbExpiryList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDefaultScaleButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripInvertColorsButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatus1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.putCallRatioToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // putCallRatioGraph
            // 
            this.putCallRatioGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.putCallRatioGraph.AutoSize = true;
            this.putCallRatioGraph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.putCallRatioGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.putCallRatioGraph.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.putCallRatioGraph.ForeColor = System.Drawing.Color.Cornsilk;
            this.putCallRatioGraph.Location = new System.Drawing.Point(3, 1);
            this.putCallRatioGraph.Margin = new System.Windows.Forms.Padding(5);
            this.putCallRatioGraph.Name = "putCallRatioGraph";
            this.putCallRatioGraph.ScrollGrace = 0D;
            this.putCallRatioGraph.ScrollMaxX = 0D;
            this.putCallRatioGraph.ScrollMaxY = 0D;
            this.putCallRatioGraph.ScrollMaxY2 = 0D;
            this.putCallRatioGraph.ScrollMinX = 0D;
            this.putCallRatioGraph.ScrollMinY = 0D;
            this.putCallRatioGraph.ScrollMinY2 = 0D;
            this.putCallRatioGraph.Size = new System.Drawing.Size(853, 469);
            this.putCallRatioGraph.TabIndex = 1;
            this.putCallRatioGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxGraph_KeyUp);
            this.putCallRatioGraph.MouseEnter += new System.EventHandler(this.xxxGraph_MouseEnter);
            this.putCallRatioGraph.MouseLeave += new System.EventHandler(this.xxxGraph_MouseLeave);
            // 
            // putCallRatioToolStrip
            // 
            this.putCallRatioToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.putCallRatioToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.putCallRatioToolStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.putCallRatioToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.putCallRatioToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripModeButton1,
            this.toolStripSeparator4,
            this.tscbExpiryList,
            this.toolStripDefaultScaleButton1,
            this.toolStripSeparator5,
            this.toolStripInvertColorsButton1,
            this.toolStripSeparator6,
            this.toolStripStatus1,
            this.toolStripLabel1});
            this.putCallRatioToolStrip.Location = new System.Drawing.Point(3, 479);
            this.putCallRatioToolStrip.Name = "putCallRatioToolStrip";
            this.putCallRatioToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.putCallRatioToolStrip.Size = new System.Drawing.Size(431, 28);
            this.putCallRatioToolStrip.TabIndex = 2;
            this.putCallRatioToolStrip.TabStop = true;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripModeButton1
            // 
            this.toolStripModeButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripModeButton1.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripModeButton1.Name = "toolStripModeButton1";
            this.toolStripModeButton1.Size = new System.Drawing.Size(94, 25);
            this.toolStripModeButton1.Text = "By Expiration";
            this.toolStripModeButton1.ToolTipText = "Select Graph Mode";
            this.toolStripModeButton1.Click += new System.EventHandler(this.toolStripModeButton1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // tscbExpiryList
            // 
            this.tscbExpiryList.Name = "tscbExpiryList";
            this.tscbExpiryList.Size = new System.Drawing.Size(121, 28);
            this.tscbExpiryList.SelectedIndexChanged += new System.EventHandler(this.tscbExpiryList_SelectedIndexChanged);
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
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripInvertColorsButton1
            // 
            this.toolStripInvertColorsButton1.AutoSize = false;
            this.toolStripInvertColorsButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripInvertColorsButton1.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripInvertColorsButton1.Name = "toolStripInvertColorsButton1";
            this.toolStripInvertColorsButton1.Size = new System.Drawing.Size(70, 24);
            this.toolStripInvertColorsButton1.Text = "Invert Colors";
            this.toolStripInvertColorsButton1.Click += new System.EventHandler(this.xxxInvertColorsToolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripStatus1
            // 
            this.toolStripStatus1.Name = "toolStripStatus1";
            this.toolStripStatus1.Size = new System.Drawing.Size(0, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 25);
            // 
            // PutCallRatioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 507);
            this.Controls.Add(this.putCallRatioToolStrip);
            this.Controls.Add(this.putCallRatioGraph);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PutCallRatioForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Put/Call Ratio";
            this.Text = "Put/Call Ratio";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PutCallRatioForm_KeyUp);
            this.putCallRatioToolStrip.ResumeLayout(false);
            this.putCallRatioToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl putCallRatioGraph;
        private System.Windows.Forms.ToolStrip putCallRatioToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripInvertColorsButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripStatus1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripModeButton1;
        private System.Windows.Forms.ToolStripComboBox tscbExpiryList;
    }
}