namespace OptionsOracle.Forms
{
    partial class OptionPainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPainForm));
            this.optionPainGraph = new ZedGraph.ZedGraphControl();
            this.optionPainToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripExpDateLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripExpDateButton = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDefaultScaleButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripInvertColorsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.optionPainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionPainGraph
            // 
            this.optionPainGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionPainGraph.AutoSize = true;
            this.optionPainGraph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(0)))));
            this.optionPainGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.optionPainGraph.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionPainGraph.ForeColor = System.Drawing.Color.Cornsilk;
            this.optionPainGraph.Location = new System.Drawing.Point(2, 1);
            this.optionPainGraph.Name = "optionPainGraph";
            this.optionPainGraph.ScrollGrace = 0;
            this.optionPainGraph.ScrollMaxX = 0;
            this.optionPainGraph.ScrollMaxY = 0;
            this.optionPainGraph.ScrollMaxY2 = 0;
            this.optionPainGraph.ScrollMinX = 0;
            this.optionPainGraph.ScrollMinY = 0;
            this.optionPainGraph.ScrollMinY2 = 0;
            this.optionPainGraph.Size = new System.Drawing.Size(640, 381);
            this.optionPainGraph.TabIndex = 1;
            this.optionPainGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.xxxGraph_MouseMoveEvent);
            this.optionPainGraph.MouseEnter += new System.EventHandler(this.xxxGraph_MouseEnter);
            this.optionPainGraph.KeyUp += new System.Windows.Forms.KeyEventHandler(this.xxxGraph_KeyUp);
            this.optionPainGraph.MouseLeave += new System.EventHandler(this.xxxGraph_MouseLeave);
            // 
            // optionPainToolStrip
            // 
            this.optionPainToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.optionPainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.optionPainToolStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionPainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripExpDateLabel,
            this.toolStripExpDateButton,
            this.toolStripSeparator2,
            this.toolStripDefaultScaleButton,
            this.toolStripSeparator3,
            this.toolStripInvertColorsButton,
            this.toolStripSeparator4,
            this.toolStripStatus,
            this.toolStripLabel});
            this.optionPainToolStrip.Location = new System.Drawing.Point(2, 385);
            this.optionPainToolStrip.Name = "optionPainToolStrip";
            this.optionPainToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.optionPainToolStrip.Size = new System.Drawing.Size(400, 27);
            this.optionPainToolStrip.TabIndex = 2;
            this.optionPainToolStrip.TabStop = true;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripExpDateLabel
            // 
            this.toolStripExpDateLabel.Name = "toolStripExpDateLabel";
            this.toolStripExpDateLabel.Size = new System.Drawing.Size(82, 24);
            this.toolStripExpDateLabel.Text = "Expiration Date:";
            // 
            // toolStripExpDateButton
            // 
            this.toolStripExpDateButton.Name = "toolStripExpDateButton";
            this.toolStripExpDateButton.Size = new System.Drawing.Size(110, 27);
            this.toolStripExpDateButton.Text = "By Expiration";
            this.toolStripExpDateButton.ToolTipText = "Select Expiration Date";
            this.toolStripExpDateButton.SelectedIndexChanged += new System.EventHandler(this.toolStripExpDateButton_SelectedIndexChanged);
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
            this.toolStripDefaultScaleButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripDefaultScaleButton.Name = "toolStripDefaultScaleButton";
            this.toolStripDefaultScaleButton.Size = new System.Drawing.Size(70, 24);
            this.toolStripDefaultScaleButton.Text = "Auto Scale";
            this.toolStripDefaultScaleButton.Click += new System.EventHandler(this.xxxGraph_ScaleToDefault);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripInvertColorsButton
            // 
            this.toolStripInvertColorsButton.AutoSize = false;
            this.toolStripInvertColorsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripInvertColorsButton.ImageTransparentColor = System.Drawing.SystemColors.Control;
            this.toolStripInvertColorsButton.Name = "toolStripInvertColorsButton";
            this.toolStripInvertColorsButton.Size = new System.Drawing.Size(70, 24);
            this.toolStripInvertColorsButton.Text = "Invert Colors";
            this.toolStripInvertColorsButton.Click += new System.EventHandler(this.xxxInvertColorsToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(0, 24);
            // 
            // toolStripLabel
            // 
            this.toolStripLabel.Name = "toolStripLabel";
            this.toolStripLabel.Size = new System.Drawing.Size(0, 24);
            // 
            // OptionPainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 412);
            this.Controls.Add(this.optionPainToolStrip);
            this.Controls.Add(this.optionPainGraph);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "OptionPainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Total Underlying Option Pain (Max Pain)";
            this.Text = "Total Underlying Option Pain (Max Pain)";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OptionPainForm_KeyUp);
            this.optionPainToolStrip.ResumeLayout(false);
            this.optionPainToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl optionPainGraph;
        private System.Windows.Forms.ToolStrip optionPainToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripDefaultScaleButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripInvertColorsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripStatus;
        private System.Windows.Forms.ToolStripLabel toolStripLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripExpDateButton;
        private System.Windows.Forms.ToolStripLabel toolStripExpDateLabel;
    }
}