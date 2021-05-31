namespace SSS
{
    partial class GST_VoucherRegister
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblInvoiceAmt = new System.Windows.Forms.Label();
            this.lblTotalTax = new System.Windows.Forms.Label();
            this.lblSGSTAmt = new System.Windows.Forms.Label();
            this.lblCGSTAmt = new System.Windows.Forms.Label();
            this.lblIGSTAmt = new System.Windows.Forms.Label();
            this.lblTaxableAmt = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblVchCount = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoVoucherWise = new System.Windows.Forms.RadioButton();
            this.rdoPartyWise = new System.Windows.Forms.RadioButton();
            this.lblVoucherOF = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Particulars = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gstNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.voucherCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taxableValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.igstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cgstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sgstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalTaxAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.invoiceAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(13, 544);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(89, 36);
            this.btnClose.TabIndex = 108;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.Particulars,
            this.gstNo,
            this.voucherCount,
            this.taxableValue,
            this.igstAmt,
            this.cgstAmt,
            this.sgstAmt,
            this.totalTaxAmt,
            this.invoiceAmt,
            this.billType});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(13, 30);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 30;
            this.dgrdDetails.Size = new System.Drawing.Size(1040, 512);
            this.dgrdDetails.TabIndex = 107;
            this.dgrdDetails.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellDoubleClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1068, 37);
            this.panel1.TabIndex = 107;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.5F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(442, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "VOUCHER REGISTER";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.lblInvoiceAmt);
            this.panel3.Controls.Add(this.lblTotalTax);
            this.panel3.Controls.Add(this.lblSGSTAmt);
            this.panel3.Controls.Add(this.lblCGSTAmt);
            this.panel3.Controls.Add(this.lblIGSTAmt);
            this.panel3.Controls.Add(this.lblTaxableAmt);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.lblVchCount);
            this.panel3.Controls.Add(this.grpStatus);
            this.panel3.Controls.Add(this.lblVoucherOF);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Location = new System.Drawing.Point(14, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1067, 585);
            this.panel3.TabIndex = 106;
            this.panel3.TabStop = true;
            // 
            // lblInvoiceAmt
            // 
            this.lblInvoiceAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceAmt.Location = new System.Drawing.Point(942, 555);
            this.lblInvoiceAmt.Name = "lblInvoiceAmt";
            this.lblInvoiceAmt.Size = new System.Drawing.Size(111, 16);
            this.lblInvoiceAmt.TabIndex = 243;
            this.lblInvoiceAmt.Text = "0.00";
            this.lblInvoiceAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalTax
            // 
            this.lblTotalTax.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalTax.Location = new System.Drawing.Point(852, 555);
            this.lblTotalTax.Name = "lblTotalTax";
            this.lblTotalTax.Size = new System.Drawing.Size(94, 16);
            this.lblTotalTax.TabIndex = 242;
            this.lblTotalTax.Text = "0.00";
            this.lblTotalTax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSGSTAmt
            // 
            this.lblSGSTAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblSGSTAmt.Location = new System.Drawing.Point(767, 555);
            this.lblSGSTAmt.Name = "lblSGSTAmt";
            this.lblSGSTAmt.Size = new System.Drawing.Size(83, 16);
            this.lblSGSTAmt.TabIndex = 241;
            this.lblSGSTAmt.Text = "0.00";
            this.lblSGSTAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCGSTAmt
            // 
            this.lblCGSTAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblCGSTAmt.Location = new System.Drawing.Point(686, 555);
            this.lblCGSTAmt.Name = "lblCGSTAmt";
            this.lblCGSTAmt.Size = new System.Drawing.Size(82, 16);
            this.lblCGSTAmt.TabIndex = 240;
            this.lblCGSTAmt.Text = "0.00";
            this.lblCGSTAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIGSTAmt
            // 
            this.lblIGSTAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblIGSTAmt.Location = new System.Drawing.Point(577, 555);
            this.lblIGSTAmt.Name = "lblIGSTAmt";
            this.lblIGSTAmt.Size = new System.Drawing.Size(91, 16);
            this.lblIGSTAmt.TabIndex = 239;
            this.lblIGSTAmt.Text = "0.00";
            this.lblIGSTAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTaxableAmt
            // 
            this.lblTaxableAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTaxableAmt.Location = new System.Drawing.Point(478, 555);
            this.lblTaxableAmt.Name = "lblTaxableAmt";
            this.lblTaxableAmt.Size = new System.Drawing.Size(103, 16);
            this.lblTaxableAmt.TabIndex = 238;
            this.lblTaxableAmt.Text = "0.00";
            this.lblTaxableAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(330, 555);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 237;
            this.label3.Text = "Total :";
            // 
            // lblVchCount
            // 
            this.lblVchCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblVchCount.Location = new System.Drawing.Point(388, 555);
            this.lblVchCount.Name = "lblVchCount";
            this.lblVchCount.Size = new System.Drawing.Size(89, 16);
            this.lblVchCount.TabIndex = 236;
            this.lblVchCount.Text = "0.00";
            this.lblVchCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoVoucherWise);
            this.grpStatus.Controls.Add(this.rdoPartyWise);
            this.grpStatus.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.grpStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grpStatus.Location = new System.Drawing.Point(846, -5);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(206, 33);
            this.grpStatus.TabIndex = 224;
            this.grpStatus.TabStop = false;
            // 
            // rdoVoucherWise
            // 
            this.rdoVoucherWise.AutoSize = true;
            this.rdoVoucherWise.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoVoucherWise.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoVoucherWise.Location = new System.Drawing.Point(100, 11);
            this.rdoVoucherWise.Name = "rdoVoucherWise";
            this.rdoVoucherWise.Size = new System.Drawing.Size(101, 18);
            this.rdoVoucherWise.TabIndex = 127;
            this.rdoVoucherWise.Text = "&Voucher wise";
            this.rdoVoucherWise.UseVisualStyleBackColor = true;
            this.rdoVoucherWise.CheckedChanged += new System.EventHandler(this.rdoVoucherWise_CheckedChanged);
            // 
            // rdoPartyWise
            // 
            this.rdoPartyWise.AutoSize = true;
            this.rdoPartyWise.Checked = true;
            this.rdoPartyWise.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoPartyWise.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoPartyWise.Location = new System.Drawing.Point(13, 11);
            this.rdoPartyWise.Name = "rdoPartyWise";
            this.rdoPartyWise.Size = new System.Drawing.Size(83, 18);
            this.rdoPartyWise.TabIndex = 126;
            this.rdoPartyWise.TabStop = true;
            this.rdoPartyWise.Text = "&Party wise";
            this.rdoPartyWise.UseVisualStyleBackColor = true;
            this.rdoPartyWise.CheckedChanged += new System.EventHandler(this.rdoPartyWise_CheckedChanged);
            // 
            // lblVoucherOF
            // 
            this.lblVoucherOF.AutoSize = true;
            this.lblVoucherOF.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblVoucherOF.Location = new System.Drawing.Point(108, 8);
            this.lblVoucherOF.Name = "lblVoucherOF";
            this.lblVoucherOF.Size = new System.Drawing.Size(36, 16);
            this.lblVoucherOF.TabIndex = 223;
            this.lblVoucherOF.Text = "B2B";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(16, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 222;
            this.label5.Text = "Voucher of : ";
            // 
            // date
            // 
            dataGridViewCellStyle2.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle2;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 90;
            // 
            // Particulars
            // 
            this.Particulars.HeaderText = "Particulars";
            this.Particulars.Name = "Particulars";
            this.Particulars.ReadOnly = true;
            this.Particulars.Width = 185;
            // 
            // gstNo
            // 
            this.gstNo.HeaderText = "GSTIN";
            this.gstNo.Name = "gstNo";
            this.gstNo.ReadOnly = true;
            this.gstNo.Width = 115;
            // 
            // voucherCount
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.voucherCount.DefaultCellStyle = dataGridViewCellStyle3;
            this.voucherCount.HeaderText = "Vch Count";
            this.voucherCount.Name = "voucherCount";
            this.voucherCount.ReadOnly = true;
            this.voucherCount.Width = 80;
            // 
            // taxableValue
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.taxableValue.DefaultCellStyle = dataGridViewCellStyle4;
            this.taxableValue.HeaderText = "Taxable Value";
            this.taxableValue.Name = "taxableValue";
            this.taxableValue.ReadOnly = true;
            this.taxableValue.Width = 105;
            // 
            // igstAmt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            this.igstAmt.DefaultCellStyle = dataGridViewCellStyle5;
            this.igstAmt.HeaderText = "IGST Amt";
            this.igstAmt.Name = "igstAmt";
            this.igstAmt.ReadOnly = true;
            this.igstAmt.Width = 85;
            // 
            // cgstAmt
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            this.cgstAmt.DefaultCellStyle = dataGridViewCellStyle6;
            this.cgstAmt.HeaderText = "CGST Amt";
            this.cgstAmt.Name = "cgstAmt";
            this.cgstAmt.ReadOnly = true;
            this.cgstAmt.Width = 85;
            // 
            // sgstAmt
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            this.sgstAmt.DefaultCellStyle = dataGridViewCellStyle7;
            this.sgstAmt.HeaderText = "SGST Amt";
            this.sgstAmt.Name = "sgstAmt";
            this.sgstAmt.ReadOnly = true;
            this.sgstAmt.Width = 85;
            // 
            // totalTaxAmt
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            this.totalTaxAmt.DefaultCellStyle = dataGridViewCellStyle8;
            this.totalTaxAmt.HeaderText = "Total Tax";
            this.totalTaxAmt.Name = "totalTaxAmt";
            this.totalTaxAmt.ReadOnly = true;
            this.totalTaxAmt.Width = 90;
            // 
            // invoiceAmt
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N2";
            this.invoiceAmt.DefaultCellStyle = dataGridViewCellStyle9;
            this.invoiceAmt.HeaderText = "Invoice Amt";
            this.invoiceAmt.Name = "invoiceAmt";
            this.invoiceAmt.ReadOnly = true;
            // 
            // billType
            // 
            this.billType.HeaderText = "Bill Type";
            this.billType.Name = "billType";
            this.billType.ReadOnly = true;
            this.billType.Visible = false;
            // 
            // GST_VoucherRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1100, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GST_VoucherRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GST_VoucherRegister";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GST_VoucherRegister_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblVoucherOF;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblInvoiceAmt;
        private System.Windows.Forms.Label lblTotalTax;
        private System.Windows.Forms.Label lblSGSTAmt;
        private System.Windows.Forms.Label lblCGSTAmt;
        private System.Windows.Forms.Label lblIGSTAmt;
        private System.Windows.Forms.Label lblTaxableAmt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblVchCount;
        private System.Windows.Forms.Label label1;
        protected internal System.Windows.Forms.RadioButton rdoPartyWise;
        protected internal System.Windows.Forms.RadioButton rdoVoucherWise;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Particulars;
        private System.Windows.Forms.DataGridViewTextBoxColumn gstNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn voucherCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn taxableValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn igstAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn cgstAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn sgstAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalTaxAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn invoiceAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn billType;
    }
}