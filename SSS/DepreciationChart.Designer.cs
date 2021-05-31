namespace SSS
{
    partial class DepreciationChart
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
            this.txtVCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panDisp = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.detailPanel = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.lblDepAmt = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.voucherNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.categoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depreciaationPer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panDisp.SuspendLayout();
            this.detailPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // txtVCode
            // 
            this.txtVCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtVCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtVCode.Location = new System.Drawing.Point(876, 4);
            this.txtVCode.Name = "txtVCode";
            this.txtVCode.ReadOnly = true;
            this.txtVCode.Size = new System.Drawing.Size(89, 23);
            this.txtVCode.TabIndex = 106;
            this.txtVCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVCode_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(813, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 16);
            this.label4.TabIndex = 117;
            this.label4.Text = "V.Code :";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(967, 0);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(47, 31);
            this.btnGo.TabIndex = 107;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(357, 6);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "Da&te :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // txtAccountName
            // 
            this.txtAccountName.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtAccountName.BackColor = System.Drawing.SystemColors.Window;
            this.txtAccountName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAccountName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtAccountName.Location = new System.Drawing.Point(79, 4);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.ReadOnly = true;
            this.txtAccountName.Size = new System.Drawing.Size(274, 23);
            this.txtAccountName.TabIndex = 101;
            this.txtAccountName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAccountName_KeyDown);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(505, 9);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 24;
            this.Label21.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "A/c Name :";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.txtToDate);
            this.panel1.Controls.Add(this.txtFromDate);
            this.panel1.Controls.Add(this.txtCategory);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtVCode);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.btnGo);
            this.panel1.Controls.Add(this.chkDate);
            this.panel1.Controls.Add(this.txtAccountName);
            this.panel1.Controls.Add(this.Label21);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(13, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 35);
            this.panel1.TabIndex = 100;
            this.panel1.TabStop = true;
            // 
            // txtToDate
            // 
            this.txtToDate.BackColor = System.Drawing.SystemColors.Control;
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(532, 4);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(85, 23);
            this.txtToDate.TabIndex = 120;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.BackColor = System.Drawing.SystemColors.Control;
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(418, 4);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(84, 23);
            this.txtFromDate.TabIndex = 119;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtCategory
            // 
            this.txtCategory.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtCategory.BackColor = System.Drawing.SystemColors.Window;
            this.txtCategory.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCategory.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCategory.Location = new System.Drawing.Point(691, 4);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.ReadOnly = true;
            this.txtCategory.Size = new System.Drawing.Size(121, 23);
            this.txtCategory.TabIndex = 105;
            this.txtCategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategory_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(618, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 16);
            this.label3.TabIndex = 118;
            this.label3.Text = "Category :";
            // 
            // panDisp
            // 
            this.panDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panDisp.AutoSize = true;
            this.panDisp.BackColor = System.Drawing.Color.White;
            this.panDisp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panDisp.Controls.Add(this.label1);
            this.panDisp.Location = new System.Drawing.Point(13, 13);
            this.panDisp.Name = "panDisp";
            this.panDisp.Size = new System.Drawing.Size(1020, 39);
            this.panDisp.TabIndex = 1000;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(412, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "DEPRECIATION CHART";
            // 
            // detailPanel
            // 
            this.detailPanel.BackColor = System.Drawing.Color.White;
            this.detailPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.detailPanel.Controls.Add(this.btnExport);
            this.detailPanel.Controls.Add(this.btnPrint);
            this.detailPanel.Controls.Add(this.btnPreview);
            this.detailPanel.Controls.Add(this.label10);
            this.detailPanel.Controls.Add(this.lblDepAmt);
            this.detailPanel.Controls.Add(this.label8);
            this.detailPanel.Controls.Add(this.lblTotalAmt);
            this.detailPanel.Controls.Add(this.btnClose);
            this.detailPanel.Controls.Add(this.dgrdDetails);
            this.detailPanel.Location = new System.Drawing.Point(13, 105);
            this.detailPanel.Name = "detailPanel";
            this.detailPanel.Size = new System.Drawing.Size(1020, 536);
            this.detailPanel.TabIndex = 205;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(848, 501);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(79, 31);
            this.btnExport.TabIndex = 172;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(679, 501);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(79, 31);
            this.btnPrint.TabIndex = 127;
            this.btnPrint.Text = "Pri&nt";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(757, 501);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(92, 31);
            this.btnPreview.TabIndex = 128;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(262, 508);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 16);
            this.label10.TabIndex = 171;
            this.label10.Text = "Dep. Amt :";
            // 
            // lblDepAmt
            // 
            this.lblDepAmt.AutoSize = true;
            this.lblDepAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblDepAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblDepAmt.Location = new System.Drawing.Point(337, 508);
            this.lblDepAmt.Name = "lblDepAmt";
            this.lblDepAmt.Size = new System.Drawing.Size(16, 16);
            this.lblDepAmt.TabIndex = 170;
            this.lblDepAmt.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(5, 508);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 16);
            this.label8.TabIndex = 162;
            this.label8.Text = "Total Amount :";
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.AutoSize = true;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblTotalAmt.Location = new System.Drawing.Point(104, 509);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size(16, 16);
            this.lblTotalAmt.TabIndex = 165;
            this.lblTotalAmt.Text = "0";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(926, 501);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 31);
            this.btnClose.TabIndex = 129;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.voucherNo,
            this.categoryName,
            this.accountName,
            this.description,
            this.amount,
            this.depreciaationPer,
            this.depAmt});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(12, 11);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.Size = new System.Drawing.Size(994, 487);
            this.dgrdDetails.TabIndex = 121;
            this.dgrdDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellClick);
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Width = 85;
            // 
            // voucherNo
            // 
            this.voucherNo.HeaderText = "Voucher No";
            this.voucherNo.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.voucherNo.LinkColor = System.Drawing.Color.Black;
            this.voucherNo.Name = "voucherNo";
            this.voucherNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.voucherNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // categoryName
            // 
            this.categoryName.HeaderText = "Category Name";
            this.categoryName.Name = "categoryName";
            this.categoryName.Width = 150;
            // 
            // accountName
            // 
            this.accountName.HeaderText = "Account Name";
            this.accountName.Name = "accountName";
            this.accountName.Width = 200;
            // 
            // description
            // 
            this.description.HeaderText = "Description";
            this.description.Name = "description";
            this.description.Width = 150;
            // 
            // amount
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.amount.DefaultCellStyle = dataGridViewCellStyle2;
            this.amount.HeaderText = "Amount";
            this.amount.Name = "amount";
            this.amount.Width = 110;
            // 
            // depreciaationPer
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.depreciaationPer.DefaultCellStyle = dataGridViewCellStyle3;
            this.depreciaationPer.HeaderText = "Dep.Per";
            this.depreciaationPer.Name = "depreciaationPer";
            this.depreciaationPer.Width = 70;
            // 
            // depAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.depAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.depAmt.HeaderText = "Dep. Amt";
            this.depAmt.Name = "depAmt";
            // 
            // DepreciationChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panDisp);
            this.Controls.Add(this.detailPanel);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DepreciationChart";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DepriciationChart";
            this.Load += new System.EventHandler(this.DepreciationChart_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DepreciationChart_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panDisp.ResumeLayout(false);
            this.panDisp.PerformLayout();
            this.detailPanel.ResumeLayout(false);
            this.detailPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtVCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        protected internal System.Windows.Forms.TextBox txtAccountName;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panDisp;
        private System.Windows.Forms.Panel detailPanel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblDepAmt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTotalAmt;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Label label1;
        protected internal System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewLinkColumn voucherNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.DataGridViewTextBoxColumn amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn depreciaationPer;
        private System.Windows.Forms.DataGridViewTextBoxColumn depAmt;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}