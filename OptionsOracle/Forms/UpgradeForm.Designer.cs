namespace OptionsOracle.Forms
{
    partial class UpgradeForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.upgradeButton = new System.Windows.Forms.Button();
            this.skipButton = new System.Windows.Forms.Button();
            this.upgradeListDataGridView = new System.Windows.Forms.DataGridView();
            this.moduleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.latestVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updateInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.upgradeListDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "The following upgrades are available for OptionsOracle:";
            // 
            // upgradeButton
            // 
            this.upgradeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.upgradeButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.upgradeButton.Location = new System.Drawing.Point(212, 165);
            this.upgradeButton.Name = "upgradeButton";
            this.upgradeButton.Size = new System.Drawing.Size(85, 25);
            this.upgradeButton.TabIndex = 1;
            this.upgradeButton.Text = "Upgrade";
            this.upgradeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.upgradeButton.UseVisualStyleBackColor = true;
            // 
            // skipButton
            // 
            this.skipButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skipButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.skipButton.Location = new System.Drawing.Point(303, 165);
            this.skipButton.Name = "skipButton";
            this.skipButton.Size = new System.Drawing.Size(85, 25);
            this.skipButton.TabIndex = 2;
            this.skipButton.Text = "Skip";
            this.skipButton.UseVisualStyleBackColor = true;
            // 
            // upgradeListDataGridView
            // 
            this.upgradeListDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.upgradeListDataGridView.AutoGenerateColumns = false;
            this.upgradeListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.upgradeListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.moduleDataGridViewTextBoxColumn,
            this.currentVersionDataGridViewTextBoxColumn,
            this.latestVersionDataGridViewTextBoxColumn});
            this.upgradeListDataGridView.DataSource = this.updateInfoBindingSource;
            this.upgradeListDataGridView.Location = new System.Drawing.Point(8, 28);
            this.upgradeListDataGridView.Name = "upgradeListDataGridView";
            this.upgradeListDataGridView.ReadOnly = true;
            this.upgradeListDataGridView.RowHeadersVisible = false;
            this.upgradeListDataGridView.Size = new System.Drawing.Size(380, 131);
            this.upgradeListDataGridView.TabIndex = 4;
            // 
            // moduleDataGridViewTextBoxColumn
            // 
            this.moduleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.moduleDataGridViewTextBoxColumn.DataPropertyName = "Module";
            this.moduleDataGridViewTextBoxColumn.HeaderText = "Module";
            this.moduleDataGridViewTextBoxColumn.Name = "moduleDataGridViewTextBoxColumn";
            this.moduleDataGridViewTextBoxColumn.ReadOnly = true;
            this.moduleDataGridViewTextBoxColumn.ToolTipText = "Module Name";
            // 
            // currentVersionDataGridViewTextBoxColumn
            // 
            this.currentVersionDataGridViewTextBoxColumn.DataPropertyName = "CurrentVersion";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.currentVersionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.currentVersionDataGridViewTextBoxColumn.HeaderText = "Current";
            this.currentVersionDataGridViewTextBoxColumn.Name = "currentVersionDataGridViewTextBoxColumn";
            this.currentVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.currentVersionDataGridViewTextBoxColumn.ToolTipText = "Current Version";
            this.currentVersionDataGridViewTextBoxColumn.Width = 60;
            // 
            // latestVersionDataGridViewTextBoxColumn
            // 
            this.latestVersionDataGridViewTextBoxColumn.DataPropertyName = "LatestVersion";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.latestVersionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.latestVersionDataGridViewTextBoxColumn.HeaderText = "Latest";
            this.latestVersionDataGridViewTextBoxColumn.Name = "latestVersionDataGridViewTextBoxColumn";
            this.latestVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.latestVersionDataGridViewTextBoxColumn.ToolTipText = "Latest Version";
            this.latestVersionDataGridViewTextBoxColumn.Width = 60;
            // 
            // updateInfoBindingSource
            // 
            this.updateInfoBindingSource.DataSource = typeof(OptionsOracle.Update.UpdateInfo);
            // 
            // UpgradeForm
            // 
            this.AcceptButton = this.upgradeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.skipButton;
            this.ClientSize = new System.Drawing.Size(390, 193);
            this.ControlBox = false;
            this.Controls.Add(this.upgradeListDataGridView);
            this.Controls.Add(this.skipButton);
            this.Controls.Add(this.upgradeButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UpgradeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upgrade is Available";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.upgradeListDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateInfoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button upgradeButton;
        private System.Windows.Forms.Button skipButton;
        private System.Windows.Forms.DataGridView upgradeListDataGridView;
        private System.Windows.Forms.BindingSource updateInfoBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currentVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn latestVersionDataGridViewTextBoxColumn;
    }
}