namespace OptionsOracle.Forms
{
    partial class LookupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LookupForm));
            this.label2 = new System.Windows.Forms.Label();
            this.lookupDataGridView = new System.Windows.Forms.DataGridView();
            this.lookupSymbolColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lookupCompanyNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.symbolTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lookupSet = new OptionsOracle.Data.LookupSet();
            this.cancelButton = new System.Windows.Forms.Button();
            this.selectButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lookupDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbolTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookupSet)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(243, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Please select symbol from list or cancel operation -";
            // 
            // lookupDataGridView
            // 
            this.lookupDataGridView.AllowUserToAddRows = false;
            this.lookupDataGridView.AllowUserToDeleteRows = false;
            this.lookupDataGridView.AutoGenerateColumns = false;
            this.lookupDataGridView.ColumnHeadersHeight = 22;
            this.lookupDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.lookupDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lookupSymbolColumn,
            this.lookupCompanyNameColumn});
            this.lookupDataGridView.DataSource = this.symbolTableBindingSource;
            this.lookupDataGridView.Location = new System.Drawing.Point(15, 25);
            this.lookupDataGridView.Name = "lookupDataGridView";
            this.lookupDataGridView.RowHeadersWidth = 16;
            this.lookupDataGridView.RowTemplate.Height = 20;
            this.lookupDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lookupDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lookupDataGridView.Size = new System.Drawing.Size(290, 158);
            this.lookupDataGridView.TabIndex = 1;
            this.lookupDataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lookupDataGridView_MouseDoubleClick);
            this.lookupDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lookupDataGridView_KeyDown);
            // 
            // lookupSymbolColumn
            // 
            this.lookupSymbolColumn.DataPropertyName = "Symbol";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(0)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.MediumTurquoise;
            this.lookupSymbolColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.lookupSymbolColumn.HeaderText = "Symbol";
            this.lookupSymbolColumn.Name = "lookupSymbolColumn";
            this.lookupSymbolColumn.ReadOnly = true;
            this.lookupSymbolColumn.Width = 60;
            // 
            // lookupCompanyNameColumn
            // 
            this.lookupCompanyNameColumn.DataPropertyName = "CompanyName";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(0)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.MediumTurquoise;
            this.lookupCompanyNameColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.lookupCompanyNameColumn.HeaderText = "Company Name";
            this.lookupCompanyNameColumn.Name = "lookupCompanyNameColumn";
            this.lookupCompanyNameColumn.ReadOnly = true;
            this.lookupCompanyNameColumn.Width = 212;
            // 
            // symbolTableBindingSource
            // 
            this.symbolTableBindingSource.DataMember = "SymbolTable";
            this.symbolTableBindingSource.DataSource = this.lookupSet;
            // 
            // lookupSet
            // 
            this.lookupSet.DataSetName = "LookupSet";
            this.lookupSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(139, 189);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(80, 24);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(225, 189);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(80, 24);
            this.selectButton.TabIndex = 2;
            this.selectButton.Text = "Select >>";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // LookupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 226);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.lookupDataGridView);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LookupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Symbol Select";
            ((System.ComponentModel.ISupportInitialize)(this.lookupDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.symbolTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookupSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView lookupDataGridView;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.BindingSource symbolTableBindingSource;
        private OptionsOracle.Data.LookupSet lookupSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn lookupSymbolColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lookupCompanyNameColumn;

    }
}