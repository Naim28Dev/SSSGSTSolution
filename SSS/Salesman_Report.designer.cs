namespace SSS
{
    partial class Salesman_Report
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
            this.txtSalesMan = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPFromSNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkPSNo = new System.Windows.Forms.CheckBox();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.txtPToSNo = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.grpBoxChk = new System.Windows.Forms.GroupBox();
            this.chkBillDate = new System.Windows.Forms.CheckBox();
            this.chkBillNo = new System.Windows.Forms.CheckBox();
            this.chkSalesMan = new System.Windows.Forms.CheckBox();
            this.chkBillCode = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoSaleReturn = new System.Windows.Forms.RadioButton();
            this.rdoSale = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTotalQty = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotalSalesInc = new System.Windows.Forms.Label();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.grpBoxChk.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSalesMan
            // 
            this.txtSalesMan.BackColor = System.Drawing.Color.White;
            this.txtSalesMan.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSalesMan.Location = new System.Drawing.Point(92, 3);
            this.txtSalesMan.Name = "txtSalesMan";
            this.txtSalesMan.ReadOnly = true;
            this.txtSalesMan.Size = new System.Drawing.Size(307, 23);
            this.txtSalesMan.TabIndex = 101;
            this.txtSalesMan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSalesMan_KeyDown);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label17.Location = new System.Drawing.Point(6, 6);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 16);
            this.label17.TabIndex = 163;
            this.label17.Text = "Sales Man :";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.chkAll);
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtSalesMan);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.txtBillCode);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.txtPFromSNo);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.chkPSNo);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.txtPToSNo);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.grpBoxChk);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(12, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(976, 66);
            this.panel2.TabIndex = 100;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(601, 2);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(84, 23);
            this.txtToDate.TabIndex = 104;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(477, 2);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(89, 23);
            this.txtFromDate.TabIndex = 103;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(8, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 16);
            this.label7.TabIndex = 167;
            this.label7.Text = "Column :";
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(773, 29);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(118, 23);
            this.txtBillCode.TabIndex = 117;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(698, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 16);
            this.label10.TabIndex = 124;
            this.label10.Text = "Bill Code :";
            // 
            // txtPFromSNo
            // 
            this.txtPFromSNo.BackColor = System.Drawing.Color.White;
            this.txtPFromSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPFromSNo.Location = new System.Drawing.Point(772, 3);
            this.txtPFromSNo.MaxLength = 10;
            this.txtPFromSNo.Name = "txtPFromSNo";
            this.txtPFromSNo.ReadOnly = true;
            this.txtPFromSNo.Size = new System.Drawing.Size(73, 23);
            this.txtPFromSNo.TabIndex = 106;
            this.txtPFromSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPFromSNo_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(851, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 16);
            this.label5.TabIndex = 136;
            this.label5.Text = "To";
            // 
            // chkPSNo
            // 
            this.chkPSNo.AutoSize = true;
            this.chkPSNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPSNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkPSNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkPSNo.Location = new System.Drawing.Point(694, 5);
            this.chkPSNo.Name = "chkPSNo";
            this.chkPSNo.Size = new System.Drawing.Size(81, 20);
            this.chkPSNo.TabIndex = 105;
            this.chkPSNo.Text = " Bill No :";
            this.chkPSNo.UseVisualStyleBackColor = true;
            this.chkPSNo.CheckedChanged += new System.EventHandler(this.chkPSNo_CheckedChanged);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkDate.Location = new System.Drawing.Point(407, 5);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // txtPToSNo
            // 
            this.txtPToSNo.BackColor = System.Drawing.Color.White;
            this.txtPToSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPToSNo.Location = new System.Drawing.Point(877, 3);
            this.txtPToSNo.MaxLength = 10;
            this.txtPToSNo.Name = "txtPToSNo";
            this.txtPToSNo.ReadOnly = true;
            this.txtPToSNo.Size = new System.Drawing.Size(87, 23);
            this.txtPToSNo.TabIndex = 107;
            this.txtPToSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPFromSNo_KeyPress);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(901, 25);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(64, 29);
            this.btnGo.TabIndex = 118;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(573, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "To";
            // 
            // grpBoxChk
            // 
            this.grpBoxChk.BackColor = System.Drawing.Color.White;
            this.grpBoxChk.Controls.Add(this.chkBillDate);
            this.grpBoxChk.Controls.Add(this.chkBillNo);
            this.grpBoxChk.Controls.Add(this.chkSalesMan);
            this.grpBoxChk.Controls.Add(this.chkBillCode);
            this.grpBoxChk.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.grpBoxChk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grpBoxChk.Location = new System.Drawing.Point(115, 22);
            this.grpBoxChk.Name = "grpBoxChk";
            this.grpBoxChk.Size = new System.Drawing.Size(334, 32);
            this.grpBoxChk.TabIndex = 108;
            this.grpBoxChk.TabStop = false;
            // 
            // chkBillDate
            // 
            this.chkBillDate.AutoSize = true;
            this.chkBillDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBillDate.Location = new System.Drawing.Point(254, 9);
            this.chkBillDate.Name = "chkBillDate";
            this.chkBillDate.Size = new System.Drawing.Size(72, 19);
            this.chkBillDate.TabIndex = 112;
            this.chkBillDate.Text = "Bill Date";
            this.chkBillDate.UseVisualStyleBackColor = true;
            // 
            // chkBillNo
            // 
            this.chkBillNo.AutoSize = true;
            this.chkBillNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBillNo.Location = new System.Drawing.Point(186, 9);
            this.chkBillNo.Name = "chkBillNo";
            this.chkBillNo.Size = new System.Drawing.Size(61, 19);
            this.chkBillNo.TabIndex = 111;
            this.chkBillNo.Text = "Bill &No";
            this.chkBillNo.UseVisualStyleBackColor = true;
            // 
            // chkSalesMan
            // 
            this.chkSalesMan.AutoSize = true;
            this.chkSalesMan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSalesMan.Location = new System.Drawing.Point(10, 9);
            this.chkSalesMan.Name = "chkSalesMan";
            this.chkSalesMan.Size = new System.Drawing.Size(85, 19);
            this.chkSalesMan.TabIndex = 109;
            this.chkSalesMan.Text = "&Sales Man";
            this.chkSalesMan.UseVisualStyleBackColor = true;
            // 
            // chkBillCode
            // 
            this.chkBillCode.AutoSize = true;
            this.chkBillCode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBillCode.Location = new System.Drawing.Point(106, 9);
            this.chkBillCode.Name = "chkBillCode";
            this.chkBillCode.Size = new System.Drawing.Size(75, 19);
            this.chkBillCode.TabIndex = 110;
            this.chkBillCode.Text = "&Bill Code";
            this.chkBillCode.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoSaleReturn);
            this.groupBox1.Controls.Add(this.rdoSale);
            this.groupBox1.Controls.Add(this.rdoAll);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Location = new System.Drawing.Point(459, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 33);
            this.groupBox1.TabIndex = 113;
            this.groupBox1.TabStop = false;
            // 
            // rdoSaleReturn
            // 
            this.rdoSaleReturn.AutoSize = true;
            this.rdoSaleReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSaleReturn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoSaleReturn.Location = new System.Drawing.Point(133, 11);
            this.rdoSaleReturn.Name = "rdoSaleReturn";
            this.rdoSaleReturn.Size = new System.Drawing.Size(88, 18);
            this.rdoSaleReturn.TabIndex = 116;
            this.rdoSaleReturn.Text = "Sale Return";
            this.rdoSaleReturn.UseVisualStyleBackColor = true;
            // 
            // rdoSale
            // 
            this.rdoSale.AutoSize = true;
            this.rdoSale.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSale.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoSale.Location = new System.Drawing.Point(62, 11);
            this.rdoSale.Name = "rdoSale";
            this.rdoSale.Size = new System.Drawing.Size(55, 18);
            this.rdoSale.TabIndex = 115;
            this.rdoSale.Text = "Sales";
            this.rdoSale.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(8, 11);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(39, 18);
            this.rdoAll.TabIndex = 114;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(211, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 16);
            this.label8.TabIndex = 154;
            this.label8.Text = "Total Amt :";
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.AutoSize = true;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblTotalAmt.Location = new System.Drawing.Point(292, 12);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size(16, 16);
            this.lblTotalAmt.TabIndex = 157;
            this.lblTotalAmt.Text = "0";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(893, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 33);
            this.btnClose.TabIndex = 144;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.Location = new System.Drawing.Point(12, 130);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(976, 459);
            this.panel3.TabIndex = 152;
            this.panel3.Tag = "0";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(-2, 15);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(956, 437);
            this.dgrdDetails.TabIndex = 137;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(976, 39);
            this.panel1.TabIndex = 151;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(397, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 19);
            this.label1.TabIndex = 9;
            this.label1.Text = "SALES MAN REPORT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "Total  Qty :";
            // 
            // lblTotalQty
            // 
            this.lblTotalQty.AutoSize = true;
            this.lblTotalQty.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblTotalQty.Location = new System.Drawing.Point(91, 12);
            this.lblTotalQty.Name = "lblTotalQty";
            this.lblTotalQty.Size = new System.Drawing.Size(16, 16);
            this.lblTotalQty.TabIndex = 18;
            this.lblTotalQty.Text = "0";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.btnPrint);
            this.panel4.Controls.Add(this.btnPreview);
            this.panel4.Controls.Add(this.btnExport);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.lblTotalSalesInc);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.lblTotalAmt);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.lblTotalQty);
            this.panel4.Location = new System.Drawing.Point(11, 599);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(977, 40);
            this.panel4.TabIndex = 153;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(740, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(63, 33);
            this.btnPrint.TabIndex = 162;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(648, 2);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(84, 33);
            this.btnPreview.TabIndex = 161;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(812, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(76, 33);
            this.btnExport.TabIndex = 160;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(405, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 16);
            this.label4.TabIndex = 158;
            this.label4.Text = "Total Sale Inc. :";
            // 
            // lblTotalSalesInc
            // 
            this.lblTotalSalesInc.AutoSize = true;
            this.lblTotalSalesInc.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalSalesInc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblTotalSalesInc.Location = new System.Drawing.Point(514, 12);
            this.lblTotalSalesInc.Name = "lblTotalSalesInc";
            this.lblTotalSalesInc.Size = new System.Drawing.Size(36, 16);
            this.lblTotalSalesInc.TabIndex = 159;
            this.lblTotalSalesInc.Text = "0.00";
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold);
            this.chkAll.Location = new System.Drawing.Point(75, 31);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(40, 19);
            this.chkAll.TabIndex = 113;
            this.chkAll.Text = "All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // Salesman_Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Salesman_Report";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agent Report";
            this.Load += new System.EventHandler(this.Salesman_Report_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Agent_Report_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpBoxChk.ResumeLayout(false);
            this.grpBoxChk.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        protected internal System.Windows.Forms.TextBox txtSalesMan;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.TextBox txtPToSNo;
        protected internal System.Windows.Forms.TextBox txtPFromSNo;
        private System.Windows.Forms.CheckBox chkPSNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTotalAmt;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTotalQty;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpBoxChk;
        private System.Windows.Forms.CheckBox chkSalesMan;
        private System.Windows.Forms.CheckBox chkBillCode;
        private System.Windows.Forms.CheckBox chkBillNo;
        private System.Windows.Forms.CheckBox chkBillDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotalSalesInc;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoSaleReturn;
        private System.Windows.Forms.RadioButton rdoSale;
        private System.Windows.Forms.RadioButton rdoAll;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.CheckBox chkAll;
    }
}