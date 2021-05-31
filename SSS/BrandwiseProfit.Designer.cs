namespace SSS
{
    partial class BrandwiseProfit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgrdProfitDetails = new System.Windows.Forms.DataGridView();
            this.sno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.brandName = new System.Windows.Forms.DataGridViewLinkColumn();
            this.saleAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBranch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdoBranch = new System.Windows.Forms.RadioButton();
            this.rdoItemName = new System.Windows.Forms.RadioButton();
            this.rdoBrandWise = new System.Windows.Forms.RadioButton();
            this.txtBrandName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.pnlProfitDetails = new System.Windows.Forms.Panel();
            this.pnlLossDetails = new System.Windows.Forms.Panel();
            this.dgrdLossDetails = new System.Windows.Forms.DataGridView();
            this.lSNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lBrandName = new System.Windows.Forms.DataGridViewLinkColumn();
            this.lSaleAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdProfitDetails)).BeginInit();
            this.panel4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlProfitDetails.SuspendLayout();
            this.pnlLossDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdLossDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // dgrdProfitDetails
            // 
            this.dgrdProfitDetails.AllowUserToAddRows = false;
            this.dgrdProfitDetails.AllowUserToDeleteRows = false;
            this.dgrdProfitDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdProfitDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgrdProfitDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdProfitDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgrdProfitDetails.ColumnHeadersHeight = 30;
            this.dgrdProfitDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdProfitDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sno,
            this.brandName,
            this.saleAmt});
            this.dgrdProfitDetails.EnableHeadersVisualStyles = false;
            this.dgrdProfitDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdProfitDetails.Location = new System.Drawing.Point(37, 19);
            this.dgrdProfitDetails.Name = "dgrdProfitDetails";
            this.dgrdProfitDetails.ReadOnly = true;
            this.dgrdProfitDetails.RowHeadersVisible = false;
            this.dgrdProfitDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdProfitDetails.RowTemplate.Height = 25;
            this.dgrdProfitDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdProfitDetails.Size = new System.Drawing.Size(463, 405);
            this.dgrdProfitDetails.TabIndex = 137;
            this.dgrdProfitDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdProfitDetails_CellContentClick);
            // 
            // sno
            // 
            this.sno.HeaderText = "S.No.";
            this.sno.Name = "sno";
            this.sno.ReadOnly = true;
            this.sno.Width = 60;
            // 
            // brandName
            // 
            this.brandName.HeaderText = "Brand Name";
            this.brandName.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.brandName.LinkColor = System.Drawing.Color.Black;
            this.brandName.Name = "brandName";
            this.brandName.ReadOnly = true;
            this.brandName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.brandName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.brandName.Width = 270;
            // 
            // saleAmt
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N2";
            this.saleAmt.DefaultCellStyle = dataGridViewCellStyle9;
            this.saleAmt.HeaderText = "Sale Amt";
            this.saleAmt.Name = "saleAmt";
            this.saleAmt.ReadOnly = true;
            this.saleAmt.Width = 130;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.txtToDate);
            this.panel4.Controls.Add(this.txtFromDate);
            this.panel4.Controls.Add(this.txtBranch);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.txtItemName);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.groupBox3);
            this.panel4.Controls.Add(this.txtBrandName);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.btnGo);
            this.panel4.Controls.Add(this.chkDate);
            this.panel4.Controls.Add(this.Label21);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Location = new System.Drawing.Point(53, 71);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(551, 79);
            this.panel4.TabIndex = 100;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(350, 43);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(75, 23);
            this.txtToDate.TabIndex = 110;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(238, 43);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(85, 23);
            this.txtFromDate.TabIndex = 109;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtBranch
            // 
            this.txtBranch.BackColor = System.Drawing.Color.White;
            this.txtBranch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBranch.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBranch.Location = new System.Drawing.Point(73, 43);
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.ReadOnly = true;
            this.txtBranch.Size = new System.Drawing.Size(86, 23);
            this.txtBranch.TabIndex = 107;
            this.txtBranch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBranch_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 16);
            this.label2.TabIndex = 156;
            this.label2.Text = "Branch :";
            // 
            // txtItemName
            // 
            this.txtItemName.BackColor = System.Drawing.Color.White;
            this.txtItemName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtItemName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtItemName.Location = new System.Drawing.Point(399, 10);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(140, 23);
            this.txtItemName.TabIndex = 106;
            this.txtItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(358, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 154;
            this.label1.Text = "Item :";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.rdoBranch);
            this.groupBox3.Controls.Add(this.rdoItemName);
            this.groupBox3.Controls.Add(this.rdoBrandWise);
            this.groupBox3.Location = new System.Drawing.Point(2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 34);
            this.groupBox3.TabIndex = 101;
            this.groupBox3.TabStop = false;
            // 
            // rdoBranch
            // 
            this.rdoBranch.AutoSize = true;
            this.rdoBranch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBranch.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoBranch.ForeColor = System.Drawing.Color.Black;
            this.rdoBranch.Location = new System.Drawing.Point(117, 11);
            this.rdoBranch.Name = "rdoBranch";
            this.rdoBranch.Size = new System.Drawing.Size(66, 19);
            this.rdoBranch.TabIndex = 104;
            this.rdoBranch.Text = "Branch";
            this.rdoBranch.UseVisualStyleBackColor = true;
            this.rdoBranch.CheckedChanged += new System.EventHandler(this.rdoBranch_CheckedChanged);
            // 
            // rdoItemName
            // 
            this.rdoItemName.AutoSize = true;
            this.rdoItemName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoItemName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoItemName.ForeColor = System.Drawing.Color.Black;
            this.rdoItemName.Location = new System.Drawing.Point(66, 11);
            this.rdoItemName.Name = "rdoItemName";
            this.rdoItemName.Size = new System.Drawing.Size(50, 19);
            this.rdoItemName.TabIndex = 103;
            this.rdoItemName.Text = "Item";
            this.rdoItemName.UseVisualStyleBackColor = true;
            this.rdoItemName.CheckedChanged += new System.EventHandler(this.rdoItemName_CheckedChanged);
            // 
            // rdoBrandWise
            // 
            this.rdoBrandWise.AutoSize = true;
            this.rdoBrandWise.Checked = true;
            this.rdoBrandWise.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBrandWise.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoBrandWise.Location = new System.Drawing.Point(7, 11);
            this.rdoBrandWise.Name = "rdoBrandWise";
            this.rdoBrandWise.Size = new System.Drawing.Size(59, 19);
            this.rdoBrandWise.TabIndex = 102;
            this.rdoBrandWise.TabStop = true;
            this.rdoBrandWise.Text = "Brand";
            this.rdoBrandWise.UseVisualStyleBackColor = true;
            this.rdoBrandWise.CheckedChanged += new System.EventHandler(this.rdoBrandWise_CheckedChanged);
            // 
            // txtBrandName
            // 
            this.txtBrandName.BackColor = System.Drawing.Color.White;
            this.txtBrandName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBrandName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBrandName.Location = new System.Drawing.Point(237, 10);
            this.txtBrandName.Name = "txtBrandName";
            this.txtBrandName.ReadOnly = true;
            this.txtBrandName.Size = new System.Drawing.Size(121, 23);
            this.txtBrandName.TabIndex = 105;
            this.txtBrandName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBrandName_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(186, 12);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 16);
            this.label14.TabIndex = 151;
            this.label14.Text = "Brand :";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(431, 39);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(46, 31);
            this.btnGo.TabIndex = 111;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(173, 45);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 108;
            this.chkDate.Text = "&Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(324, 47);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 145;
            this.Label21.Text = "To";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(482, 39);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 31);
            this.btnClose.TabIndex = 112;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblHeader);
            this.panel1.Location = new System.Drawing.Point(54, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(550, 40);
            this.panel1.TabIndex = 144;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHeader.Location = new System.Drawing.Point(79, 11);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(364, 19);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "BRAND WISE HIGH PROFIT SUMMARY";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlProfitDetails
            // 
            this.pnlProfitDetails.BackColor = System.Drawing.Color.White;
            this.pnlProfitDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlProfitDetails.Controls.Add(this.dgrdProfitDetails);
            this.pnlProfitDetails.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlProfitDetails.Location = new System.Drawing.Point(53, 162);
            this.pnlProfitDetails.Name = "pnlProfitDetails";
            this.pnlProfitDetails.Size = new System.Drawing.Size(551, 458);
            this.pnlProfitDetails.TabIndex = 145;
            this.pnlProfitDetails.Tag = "0";
            // 
            // pnlLossDetails
            // 
            this.pnlLossDetails.BackColor = System.Drawing.Color.White;
            this.pnlLossDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlLossDetails.Controls.Add(this.dgrdLossDetails);
            this.pnlLossDetails.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlLossDetails.Location = new System.Drawing.Point(53, 162);
            this.pnlLossDetails.Name = "pnlLossDetails";
            this.pnlLossDetails.Size = new System.Drawing.Size(551, 458);
            this.pnlLossDetails.TabIndex = 148;
            this.pnlLossDetails.Tag = "0";
            // 
            // dgrdLossDetails
            // 
            this.dgrdLossDetails.AllowUserToAddRows = false;
            this.dgrdLossDetails.AllowUserToDeleteRows = false;
            this.dgrdLossDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdLossDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgrdLossDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdLossDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgrdLossDetails.ColumnHeadersHeight = 30;
            this.dgrdLossDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdLossDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lSNo,
            this.lBrandName,
            this.lSaleAmt});
            this.dgrdLossDetails.EnableHeadersVisualStyles = false;
            this.dgrdLossDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdLossDetails.Location = new System.Drawing.Point(37, 19);
            this.dgrdLossDetails.Name = "dgrdLossDetails";
            this.dgrdLossDetails.ReadOnly = true;
            this.dgrdLossDetails.RowHeadersVisible = false;
            this.dgrdLossDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdLossDetails.RowTemplate.Height = 25;
            this.dgrdLossDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdLossDetails.Size = new System.Drawing.Size(463, 405);
            this.dgrdLossDetails.TabIndex = 139;
            // 
            // lSNo
            // 
            this.lSNo.HeaderText = "S.No.";
            this.lSNo.Name = "lSNo";
            this.lSNo.ReadOnly = true;
            this.lSNo.Width = 60;
            // 
            // lBrandName
            // 
            this.lBrandName.HeaderText = "Brand Name";
            this.lBrandName.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lBrandName.LinkColor = System.Drawing.Color.Black;
            this.lBrandName.Name = "lBrandName";
            this.lBrandName.ReadOnly = true;
            this.lBrandName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.lBrandName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.lBrandName.Width = 270;
            // 
            // lSaleAmt
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.Format = "N2";
            this.lSaleAmt.DefaultCellStyle = dataGridViewCellStyle12;
            this.lSaleAmt.HeaderText = "Sale Amt";
            this.lSaleAmt.Name = "lSaleAmt";
            this.lSaleAmt.ReadOnly = true;
            this.lSaleAmt.Width = 130;
            // 
            // BrandwiseProfit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(659, 658);
            this.Controls.Add(this.pnlLossDetails);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlProfitDetails);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BrandwiseProfit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Brand wise Profit";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrandwiseProfit_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdProfitDetails)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnlProfitDetails.ResumeLayout(false);
            this.pnlLossDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdLossDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrdProfitDetails;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlProfitDetails;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        protected internal System.Windows.Forms.TextBox txtBrandName;
        private System.Windows.Forms.Label label14;
        protected internal System.Windows.Forms.TextBox txtBranch;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdoBranch;
        private System.Windows.Forms.RadioButton rdoItemName;
        private System.Windows.Forms.RadioButton rdoBrandWise;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.Panel pnlLossDetails;
        private System.Windows.Forms.DataGridView dgrdLossDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn sno;
        private System.Windows.Forms.DataGridViewLinkColumn brandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn saleAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn lSNo;
        private System.Windows.Forms.DataGridViewLinkColumn lBrandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn lSaleAmt;
    }
}