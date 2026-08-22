namespace Customer_Loyalty_Portal
{
    partial class BillDetailDialog
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.mobileLabel = new System.Windows.Forms.Label();
            this.customerLabel = new System.Windows.Forms.Label();
            this.dateLabel = new System.Windows.Forms.Label();
            this.billTitleLabel = new System.Windows.Forms.Label();
            this.itemsGrid = new System.Windows.Forms.DataGridView();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.netAmountLabel = new System.Windows.Forms.Label();
            this.paymentModeLabel = new System.Windows.Forms.Label();
            this.totalDiscLabel = new System.Windows.Forms.Label();
            this.totalMrpLabel = new System.Windows.Forms.Label();
            this.totalQtyLabel = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGrid)).BeginInit();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel.Controls.Add(this.mobileLabel);
            this.headerPanel.Controls.Add(this.customerLabel);
            this.headerPanel.Controls.Add(this.dateLabel);
            this.headerPanel.Controls.Add(this.billTitleLabel);
            this.headerPanel.Location = new System.Drawing.Point(18, 15);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(868, 76);
            this.headerPanel.TabIndex = 0;
            // 
            // mobileLabel
            // 
            this.mobileLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mobileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.mobileLabel.Location = new System.Drawing.Point(540, 42);
            this.mobileLabel.Name = "mobileLabel";
            this.mobileLabel.Size = new System.Drawing.Size(315, 22);
            this.mobileLabel.TabIndex = 3;
            this.mobileLabel.Text = "Mobile: -";
            this.mobileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // customerLabel
            // 
            this.customerLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.customerLabel.Location = new System.Drawing.Point(540, 12);
            this.customerLabel.Name = "customerLabel";
            this.customerLabel.Size = new System.Drawing.Size(315, 24);
            this.customerLabel.TabIndex = 2;
            this.customerLabel.Text = "Customer: -";
            this.customerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.dateLabel.Location = new System.Drawing.Point(14, 44);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(120, 17);
            this.dateLabel.TabIndex = 1;
            this.dateLabel.Text = "Date: --/--/---- --:--";
            // 
            // billTitleLabel
            // 
            this.billTitleLabel.AutoSize = true;
            this.billTitleLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(43)))), ((int)(((byte)(73)))));
            this.billTitleLabel.Location = new System.Drawing.Point(12, 10);
            this.billTitleLabel.Name = "billTitleLabel";
            this.billTitleLabel.Size = new System.Drawing.Size(185, 28);
            this.billTitleLabel.TabIndex = 0;
            this.billTitleLabel.Text = "Bill Details #00000";
            // 
            // itemsGrid
            // 
            this.itemsGrid.AllowUserToAddRows = false;
            this.itemsGrid.AllowUserToDeleteRows = false;
            this.itemsGrid.AllowUserToResizeRows = false;
            this.itemsGrid.BackgroundColor = System.Drawing.Color.White;
            this.itemsGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.itemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.itemsGrid.Location = new System.Drawing.Point(18, 102);
            this.itemsGrid.Name = "itemsGrid";
            this.itemsGrid.ReadOnly = true;
            this.itemsGrid.RowHeadersVisible = false;
            this.itemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.itemsGrid.Size = new System.Drawing.Size(868, 335);
            this.itemsGrid.TabIndex = 1;
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.White;
            this.footerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.footerPanel.Controls.Add(this.closeButton);
            this.footerPanel.Controls.Add(this.netAmountLabel);
            this.footerPanel.Controls.Add(this.paymentModeLabel);
            this.footerPanel.Controls.Add(this.totalDiscLabel);
            this.footerPanel.Controls.Add(this.totalMrpLabel);
            this.footerPanel.Controls.Add(this.totalQtyLabel);
            this.footerPanel.Location = new System.Drawing.Point(18, 448);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(868, 72);
            this.footerPanel.TabIndex = 2;
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(760, 18);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(95, 35);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // netAmountLabel
            // 
            this.netAmountLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.netAmountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.netAmountLabel.Location = new System.Drawing.Point(475, 12);
            this.netAmountLabel.Name = "netAmountLabel";
            this.netAmountLabel.Size = new System.Drawing.Size(270, 26);
            this.netAmountLabel.TabIndex = 4;
            this.netAmountLabel.Text = "Net Amount: ₹0";
            this.netAmountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // paymentModeLabel
            // 
            this.paymentModeLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paymentModeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(135)))), ((int)(((byte)(84)))));
            this.paymentModeLabel.Location = new System.Drawing.Point(475, 40);
            this.paymentModeLabel.Name = "paymentModeLabel";
            this.paymentModeLabel.Size = new System.Drawing.Size(270, 20);
            this.paymentModeLabel.TabIndex = 3;
            this.paymentModeLabel.Text = "Cash: ₹0 | Card: ₹0";
            this.paymentModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // totalDiscLabel
            // 
            this.totalDiscLabel.AutoSize = true;
            this.totalDiscLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalDiscLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.totalDiscLabel.Location = new System.Drawing.Point(235, 38);
            this.totalDiscLabel.Name = "totalDiscLabel";
            this.totalDiscLabel.Size = new System.Drawing.Size(109, 19);
            this.totalDiscLabel.TabIndex = 2;
            this.totalDiscLabel.Text = "Total Disc: ₹0.00";
            // 
            // totalMrpLabel
            // 
            this.totalMrpLabel.AutoSize = true;
            this.totalMrpLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalMrpLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.totalMrpLabel.Location = new System.Drawing.Point(235, 14);
            this.totalMrpLabel.Name = "totalMrpLabel";
            this.totalMrpLabel.Size = new System.Drawing.Size(110, 19);
            this.totalMrpLabel.TabIndex = 1;
            this.totalMrpLabel.Text = "Total MRP: ₹0.00";
            // 
            // totalQtyLabel
            // 
            this.totalQtyLabel.AutoSize = true;
            this.totalQtyLabel.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalQtyLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.totalQtyLabel.Location = new System.Drawing.Point(14, 25);
            this.totalQtyLabel.Name = "totalQtyLabel";
            this.totalQtyLabel.Size = new System.Drawing.Size(114, 19);
            this.totalQtyLabel.TabIndex = 0;
            this.totalQtyLabel.Text = "Total Items / Qty: 0";
            // 
            // BillDetailDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(249)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(904, 536);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.itemsGrid);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BillDetailDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bill Details";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BillDetailDialog_KeyDown);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGrid)).EndInit();
            this.footerPanel.ResumeLayout(false);
            this.footerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label mobileLabel;
        private System.Windows.Forms.Label customerLabel;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.Label billTitleLabel;
        private System.Windows.Forms.DataGridView itemsGrid;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Label totalQtyLabel;
        private System.Windows.Forms.Label totalMrpLabel;
        private System.Windows.Forms.Label totalDiscLabel;
        private System.Windows.Forms.Label paymentModeLabel;
        private System.Windows.Forms.Label netAmountLabel;
        private System.Windows.Forms.Button closeButton;
    }
}
