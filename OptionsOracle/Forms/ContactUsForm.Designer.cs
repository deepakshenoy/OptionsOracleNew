namespace OptionsOracle.Forms
{
    partial class ContactUsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactUsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.iWouldLikeToComboBox = new System.Windows.Forms.ComboBox();
            this.attachDatabaseCheckBox = new System.Windows.Forms.CheckBox();
            this.attachConfigurationCheckBox = new System.Windows.Forms.CheckBox();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.subjectTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.yourEmailTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.yourNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.loadButton = new System.Windows.Forms.Button();
            this.loadText = new System.Windows.Forms.TextBox();
            this.advanceButton = new System.Windows.Forms.Button();
            this.attachDynamicServerCheckBox = new System.Windows.Forms.CheckBox();
            this.cryptoButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "I would like to...";
            // 
            // iWouldLikeToComboBox
            // 
            this.iWouldLikeToComboBox.FormattingEnabled = true;
            this.iWouldLikeToComboBox.Items.AddRange(new object[] {
            "Report a problem / issue",
            "Ask a question",
            "Suggest a future feature / change",
            "Other"});
            this.iWouldLikeToComboBox.Location = new System.Drawing.Point(12, 25);
            this.iWouldLikeToComboBox.Name = "iWouldLikeToComboBox";
            this.iWouldLikeToComboBox.Size = new System.Drawing.Size(388, 21);
            this.iWouldLikeToComboBox.TabIndex = 1;
            this.iWouldLikeToComboBox.SelectedIndexChanged += new System.EventHandler(this.iWouldLikeToComboBox_SelectedIndexChanged);
            // 
            // attachDatabaseCheckBox
            // 
            this.attachDatabaseCheckBox.AutoSize = true;
            this.attachDatabaseCheckBox.Checked = true;
            this.attachDatabaseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.attachDatabaseCheckBox.Location = new System.Drawing.Point(12, 333);
            this.attachDatabaseCheckBox.Name = "attachDatabaseCheckBox";
            this.attachDatabaseCheckBox.Size = new System.Drawing.Size(323, 17);
            this.attachDatabaseCheckBox.TabIndex = 6;
            this.attachDatabaseCheckBox.Text = "Attach Database (Quote, Options-Chain, Positions and Results)";
            this.attachDatabaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // attachConfigurationCheckBox
            // 
            this.attachConfigurationCheckBox.AutoSize = true;
            this.attachConfigurationCheckBox.Checked = true;
            this.attachConfigurationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.attachConfigurationCheckBox.Location = new System.Drawing.Point(12, 356);
            this.attachConfigurationCheckBox.Name = "attachConfigurationCheckBox";
            this.attachConfigurationCheckBox.Size = new System.Drawing.Size(367, 17);
            this.attachConfigurationCheckBox.TabIndex = 7;
            this.attachConfigurationCheckBox.Text = "Attach Configuration (Commission, Margin, Interest and Server Selection)";
            this.attachConfigurationCheckBox.UseVisualStyleBackColor = true;
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.Location = new System.Drawing.Point(12, 175);
            this.commentsTextBox.Multiline = true;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.Size = new System.Drawing.Size(387, 142);
            this.commentsTextBox.TabIndex = 5;
            this.commentsTextBox.TextChanged += new System.EventHandler(this.commentsTextBox_TextChanged);
            // 
            // subjectTextBox
            // 
            this.subjectTextBox.Location = new System.Drawing.Point(12, 75);
            this.subjectTextBox.Name = "subjectTextBox";
            this.subjectTextBox.Size = new System.Drawing.Size(387, 20);
            this.subjectTextBox.TabIndex = 2;
            this.subjectTextBox.TextChanged += new System.EventHandler(this.subjectTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Subject";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Your E-mail";
            // 
            // yourEmailTextBox
            // 
            this.yourEmailTextBox.Location = new System.Drawing.Point(167, 125);
            this.yourEmailTextBox.Name = "yourEmailTextBox";
            this.yourEmailTextBox.Size = new System.Drawing.Size(232, 20);
            this.yourEmailTextBox.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Comments";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(233, 404);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(80, 24);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.Enabled = false;
            this.sendButton.Location = new System.Drawing.Point(319, 404);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(80, 24);
            this.sendButton.TabIndex = 8;
            this.sendButton.Text = "Send!";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // yourNameTextBox
            // 
            this.yourNameTextBox.Location = new System.Drawing.Point(12, 125);
            this.yourNameTextBox.Name = "yourNameTextBox";
            this.yourNameTextBox.Size = new System.Drawing.Size(149, 20);
            this.yourNameTextBox.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Your Name";
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(341, 328);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(58, 24);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TabStop = false;
            this.webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser.Visible = false;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(164, 404);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(24, 24);
            this.loadButton.TabIndex = 0;
            this.loadButton.TabStop = false;
            this.loadButton.Text = "L";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Visible = false;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // loadText
            // 
            this.loadText.Location = new System.Drawing.Point(194, 404);
            this.loadText.MaxLength = 1024;
            this.loadText.Multiline = true;
            this.loadText.Name = "loadText";
            this.loadText.Size = new System.Drawing.Size(30, 24);
            this.loadText.TabIndex = 0;
            this.loadText.TabStop = false;
            this.loadText.Visible = false;
            // 
            // advanceButton
            // 
            this.advanceButton.Location = new System.Drawing.Point(12, 404);
            this.advanceButton.Name = "advanceButton";
            this.advanceButton.Size = new System.Drawing.Size(80, 24);
            this.advanceButton.TabIndex = 10;
            this.advanceButton.Text = "Advance ...";
            this.advanceButton.UseVisualStyleBackColor = true;
            this.advanceButton.Click += new System.EventHandler(this.advanceButton_Click);
            // 
            // attachDynamicServerCheckBox
            // 
            this.attachDynamicServerCheckBox.AutoSize = true;
            this.attachDynamicServerCheckBox.Enabled = false;
            this.attachDynamicServerCheckBox.Location = new System.Drawing.Point(12, 379);
            this.attachDynamicServerCheckBox.Name = "attachDynamicServerCheckBox";
            this.attachDynamicServerCheckBox.Size = new System.Drawing.Size(196, 17);
            this.attachDynamicServerCheckBox.TabIndex = 11;
            this.attachDynamicServerCheckBox.Text = "Attach Dynamic-Server Debug Data";
            this.attachDynamicServerCheckBox.UseVisualStyleBackColor = true;
            // 
            // cryptoButton
            // 
            this.cryptoButton.Location = new System.Drawing.Point(137, 404);
            this.cryptoButton.Name = "cryptoButton";
            this.cryptoButton.Size = new System.Drawing.Size(24, 24);
            this.cryptoButton.TabIndex = 12;
            this.cryptoButton.TabStop = false;
            this.cryptoButton.Text = "C";
            this.cryptoButton.UseVisualStyleBackColor = true;
            this.cryptoButton.Visible = false;
            this.cryptoButton.Click += new System.EventHandler(this.cryptoButton_Click);
            // 
            // ContactUsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 440);
            this.Controls.Add(this.cryptoButton);
            this.Controls.Add(this.attachDynamicServerCheckBox);
            this.Controls.Add(this.advanceButton);
            this.Controls.Add(this.loadText);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.yourNameTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.yourEmailTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.subjectTextBox);
            this.Controls.Add(this.commentsTextBox);
            this.Controls.Add(this.attachConfigurationCheckBox);
            this.Controls.Add(this.attachDatabaseCheckBox);
            this.Controls.Add(this.iWouldLikeToComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ContactUsForm";
            this.Text = "Contact Us";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox iWouldLikeToComboBox;
        private System.Windows.Forms.CheckBox attachDatabaseCheckBox;
        private System.Windows.Forms.CheckBox attachConfigurationCheckBox;
        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.TextBox subjectTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox yourEmailTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox yourNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.TextBox loadText;
        private System.Windows.Forms.Button advanceButton;
        private System.Windows.Forms.CheckBox attachDynamicServerCheckBox;
        private System.Windows.Forms.Button cryptoButton;
    }
}