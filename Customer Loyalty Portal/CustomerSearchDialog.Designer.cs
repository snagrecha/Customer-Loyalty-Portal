namespace Customer_Loyalty_Portal
{
    partial class CustomerSearchDialog
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.customersGrid = new System.Windows.Forms.DataGridView();
            this.selectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.customersGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(43)))), ((int)(((byte)(73)))));
            this.titleLabel.Location = new System.Drawing.Point(16, 14);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(134, 21);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Select Customer";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.infoLabel.Location = new System.Drawing.Point(17, 39);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(325, 17);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "Multiple customers found. Select a customer to proceed:";
            // 
            // customersGrid
            // 
            this.customersGrid.AllowUserToAddRows = false;
            this.customersGrid.AllowUserToDeleteRows = false;
            this.customersGrid.AllowUserToResizeRows = false;
            this.customersGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customersGrid.BackgroundColor = System.Drawing.Color.White;
            this.customersGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.customersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.customersGrid.Location = new System.Drawing.Point(20, 68);
            this.customersGrid.MultiSelect = false;
            this.customersGrid.Name = "customersGrid";
            this.customersGrid.ReadOnly = true;
            this.customersGrid.RowHeadersVisible = false;
            this.customersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.customersGrid.Size = new System.Drawing.Size(520, 240);
            this.customersGrid.TabIndex = 2;
            this.customersGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.customersGrid_CellDoubleClick);
            this.customersGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.customersGrid_KeyDown);
            // 
            // selectButton
            // 
            this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.selectButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectButton.FlatAppearance.BorderSize = 0;
            this.selectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectButton.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectButton.ForeColor = System.Drawing.Color.White;
            this.selectButton.Location = new System.Drawing.Point(344, 324);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(95, 32);
            this.selectButton.TabIndex = 3;
            this.selectButton.Text = "Select";
            this.selectButton.UseVisualStyleBackColor = false;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(445, 324);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(95, 32);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // CustomerSearchDialog
            // 
            this.AcceptButton = this.selectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(560, 370);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.customersGrid);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerSearchDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Results - Select Customer";
            ((System.ComponentModel.ISupportInitialize)(this.customersGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.DataGridView customersGrid;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
