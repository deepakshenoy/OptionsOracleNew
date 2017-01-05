namespace OptionsOracle
{
    partial class AdvanceDiagForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvanceDiagForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.deleteConfigButton = new System.Windows.Forms.Button();
            this.checkIPButton = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.enableServerDebugButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(429, 268);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // deleteConfigButton
            // 
            this.deleteConfigButton.Location = new System.Drawing.Point(12, 12);
            this.deleteConfigButton.Name = "deleteConfigButton";
            this.deleteConfigButton.Size = new System.Drawing.Size(160, 24);
            this.deleteConfigButton.TabIndex = 1;
            this.deleteConfigButton.Text = "Reset Configuration";
            this.deleteConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.deleteConfigButton.UseVisualStyleBackColor = true;
            this.deleteConfigButton.Click += new System.EventHandler(this.deleteConfigButton_Click);
            // 
            // checkIPButton
            // 
            this.checkIPButton.Location = new System.Drawing.Point(178, 12);
            this.checkIPButton.Name = "checkIPButton";
            this.checkIPButton.Size = new System.Drawing.Size(160, 23);
            this.checkIPButton.TabIndex = 2;
            this.checkIPButton.Text = "Check Internet Connection";
            this.checkIPButton.UseVisualStyleBackColor = true;
            this.checkIPButton.Click += new System.EventHandler(this.checkIPButton_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(12, 176);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(492, 86);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TabStop = false;
            this.webBrowser.Url = new System.Uri(global::OptionsOracle.Properties.Resources.AppDefaultLanguage, System.UriKind.Relative);
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // statusTextBox
            // 
            this.statusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusTextBox.Location = new System.Drawing.Point(12, 42);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(492, 128);
            this.statusTextBox.TabIndex = 0;
            this.statusTextBox.TabStop = false;
            // 
            // enableServerDebugButton
            // 
            this.enableServerDebugButton.Location = new System.Drawing.Point(344, 12);
            this.enableServerDebugButton.Name = "enableServerDebugButton";
            this.enableServerDebugButton.Size = new System.Drawing.Size(160, 23);
            this.enableServerDebugButton.TabIndex = 4;
            this.enableServerDebugButton.Text = "Enable Server Debug Mode";
            this.enableServerDebugButton.UseVisualStyleBackColor = true;
            this.enableServerDebugButton.Click += new System.EventHandler(this.enableServerDebugButton_Click);
            // 
            // AdvanceDiagForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(516, 299);
            this.Controls.Add(this.deleteConfigButton);
            this.Controls.Add(this.enableServerDebugButton);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.checkIPButton);
            this.Controls.Add(this.cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AdvanceDiagForm";
            this.Text = "Advance Diagnostic...";
            this.Load += new System.EventHandler(this.AdvanceDiagForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteConfigButton;
        private System.Windows.Forms.Button checkIPButton;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Button enableServerDebugButton;
    }
}