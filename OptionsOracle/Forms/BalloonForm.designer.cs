namespace OptionsOracle
{
    partial class BalloonForm
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
            this.closeButton = new System.Windows.Forms.Button();
            this.htmlPanel = new System.Windows.Forms.Panel();
            this.selectionListBox = new System.Windows.Forms.ListBox();
            this.form2Button = new System.Windows.Forms.Button();
            this.form1Button = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.htmlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(287, 11);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(23, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "X";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Visible = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // htmlPanel
            // 
            this.htmlPanel.Controls.Add(this.selectionListBox);
            this.htmlPanel.Controls.Add(this.form2Button);
            this.htmlPanel.Controls.Add(this.form1Button);
            this.htmlPanel.Controls.Add(this.closeButton);
            this.htmlPanel.Controls.Add(this.webBrowser);
            this.htmlPanel.Location = new System.Drawing.Point(25, 25);
            this.htmlPanel.Name = "htmlPanel";
            this.htmlPanel.Size = new System.Drawing.Size(320, 300);
            this.htmlPanel.TabIndex = 1;
            this.htmlPanel.Tag = "";
            // 
            // selectionListBox
            // 
            this.selectionListBox.FormattingEnabled = true;
            this.selectionListBox.Location = new System.Drawing.Point(10, 111);
            this.selectionListBox.Name = "selectionListBox";
            this.selectionListBox.Size = new System.Drawing.Size(300, 134);
            this.selectionListBox.TabIndex = 4;
            // 
            // form2Button
            // 
            this.form2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.form2Button.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.form2Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.form2Button.ForeColor = System.Drawing.Color.Black;
            this.form2Button.Location = new System.Drawing.Point(223, 262);
            this.form2Button.Name = "form2Button";
            this.form2Button.Size = new System.Drawing.Size(87, 30);
            this.form2Button.TabIndex = 1;
            this.form2Button.Text = "Start";
            this.form2Button.UseVisualStyleBackColor = false;
            this.form2Button.Click += new System.EventHandler(this.form2Button_Click);
            // 
            // form1Button
            // 
            this.form1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.form1Button.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.form1Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.form1Button.ForeColor = System.Drawing.Color.Black;
            this.form1Button.Location = new System.Drawing.Point(130, 262);
            this.form1Button.Name = "form1Button";
            this.form1Button.Size = new System.Drawing.Size(87, 30);
            this.form1Button.TabIndex = 2;
            this.form1Button.Text = "Cancel";
            this.form1Button.UseVisualStyleBackColor = false;
            this.form1Button.Click += new System.EventHandler(this.form1Button_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(10, 11);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(300, 245);
            this.webBrowser.TabIndex = 1;
            this.webBrowser.TabStop = false;
            // 
            // BalloonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(370, 350);
            this.Controls.Add(this.htmlPanel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BalloonForm";
            this.Opacity = 0.9;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "BalloonForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BalloonForm_Load);
            this.htmlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel htmlPanel;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button form2Button;
        private System.Windows.Forms.Button form1Button;
        private System.Windows.Forms.ListBox selectionListBox;
    }
}