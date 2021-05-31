namespace SSS
{
    partial class BiltyDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnEInvoice = new System.Windows.Forms.Button();
            this.btnImportExcel = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnGenerateJSON = new System.Windows.Forms.Button();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBillNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtToSerialNo = new System.Windows.Forms.TextBox();
            this.txtFromSerialNo = new System.Windows.Forms.TextBox();
            this.chkSerial = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSalesParty = new System.Windows.Forms.TextBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoWithoutLR = new System.Windows.Forms.RadioButton();
            this.rdoWithLR = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoWithoutWayBillNo = new System.Windows.Forms.RadioButton();
            this.rdoWithWayBill = new System.Windows.Forms.RadioButton();
            this.rdoWayBillAll = new System.Windows.Forms.RadioButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblBill = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgrdBilty = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoWithoutIRN = new System.Windows.Forms.RadioButton();
            this.rdoWithIRN = new System.Windows.Forms.RadioButton();
            this.rdoIAll = new System.Windows.Forms.RadioButton();
            this.chk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transportName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lrNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lrDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.waybillNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.waybillDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.supplierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IRNNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.attachedBill = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.purchaseSNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.goodsType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oldLRNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBilty)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(7, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1083, 36);
            this.panel1.TabIndex = 101;
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkDate.Location = new System.Drawing.Point(793, 16);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 105;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(944, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 214;
            this.label2.Text = "To";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(971, 37);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(84, 34);
            this.btnGo.TabIndex = 120;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(3, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 16);
            this.label4.TabIndex = 215;
            this.label4.Text = "Party :";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnEInvoice);
            this.panel3.Controls.Add(this.btnImportExcel);
            this.panel3.Controls.Add(this.btnBrowse);
            this.panel3.Controls.Add(this.txtFilePath);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnPreview);
            this.panel3.Controls.Add(this.btnGenerateJSON);
            this.panel3.Controls.Add(this.chkAll);
            this.panel3.Controls.Add(this.grpSearch);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.lblBill);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.dgrdBilty);
            this.panel3.Location = new System.Drawing.Point(8, 51);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1082, 598);
            this.panel3.TabIndex = 100;
            this.panel3.TabStop = true;
            // 
            // btnEInvoice
            // 
            this.btnEInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEInvoice.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnEInvoice.ForeColor = System.Drawing.Color.White;
            this.btnEInvoice.Location = new System.Drawing.Point(706, 562);
            this.btnEInvoice.Name = "btnEInvoice";
            this.btnEInvoice.Size = new System.Drawing.Size(77, 29);
            this.btnEInvoice.TabIndex = 127;
            this.btnEInvoice.Text = "E-Invoice";
            this.btnEInvoice.UseVisualStyleBackColor = false;
            this.btnEInvoice.Click += new System.EventHandler(this.btnEInvoice_Click);
            // 
            // btnImportExcel
            // 
            this.btnImportExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnImportExcel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnImportExcel.ForeColor = System.Drawing.Color.White;
            this.btnImportExcel.Location = new System.Drawing.Point(602, 562);
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.Size = new System.Drawing.Size(77, 29);
            this.btnImportExcel.TabIndex = 126;
            this.btnImportExcel.Text = "&Import";
            this.btnImportExcel.UseVisualStyleBackColor = false;
            this.btnImportExcel.Click += new System.EventHandler(this.btnImportExcel_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnBrowse.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(541, 562);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(62, 29);
            this.btnBrowse.TabIndex = 125;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.SystemColors.Window;
            this.txtFilePath.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFilePath.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFilePath.Location = new System.Drawing.Point(215, 566);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(325, 23);
            this.txtFilePath.TabIndex = 219;
            this.txtFilePath.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(164, 568);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 16);
            this.label8.TabIndex = 221;
            this.label8.Text = "Excel :";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(885, 562);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(61, 29);
            this.btnPrint.TabIndex = 129;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(945, 562);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(66, 29);
            this.btnPreview.TabIndex = 130;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnGenerateJSON
            // 
            this.btnGenerateJSON.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGenerateJSON.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnGenerateJSON.ForeColor = System.Drawing.Color.White;
            this.btnGenerateJSON.Location = new System.Drawing.Point(782, 562);
            this.btnGenerateJSON.Name = "btnGenerateJSON";
            this.btnGenerateJSON.Size = new System.Drawing.Size(104, 29);
            this.btnGenerateJSON.TabIndex = 128;
            this.btnGenerateJSON.Text = "Ge&nerate JSON";
            this.btnGenerateJSON.UseVisualStyleBackColor = false;
            this.btnGenerateJSON.Click += new System.EventHandler(this.btnGenerateJSON_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Location = new System.Drawing.Point(13, 87);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 127;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.txtToDate);
            this.grpSearch.Controls.Add(this.txtFromDate);
            this.grpSearch.Controls.Add(this.txtBillNo);
            this.grpSearch.Controls.Add(this.label6);
            this.grpSearch.Controls.Add(this.txtBillCode);
            this.grpSearch.Controls.Add(this.label10);
            this.grpSearch.Controls.Add(this.txtToSerialNo);
            this.grpSearch.Controls.Add(this.txtFromSerialNo);
            this.grpSearch.Controls.Add(this.chkSerial);
            this.grpSearch.Controls.Add(this.label7);
            this.grpSearch.Controls.Add(this.txtSalesParty);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.label4);
            this.grpSearch.Controls.Add(this.btnGo);
            this.grpSearch.Controls.Add(this.chkDate);
            this.grpSearch.Controls.Add(this.grpStatus);
            this.grpSearch.Controls.Add(this.groupBox1);
            this.grpSearch.Controls.Add(this.groupBox2);
            this.grpSearch.Location = new System.Drawing.Point(6, -2);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(1064, 75);
            this.grpSearch.TabIndex = 101;
            this.grpSearch.TabStop = false;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(969, 13);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(85, 23);
            this.txtToDate.TabIndex = 107;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(858, 13);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(85, 23);
            this.txtFromDate.TabIndex = 106;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtBillNo
            // 
            this.txtBillNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtBillNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillNo.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtBillNo.Location = new System.Drawing.Point(408, 15);
            this.txtBillNo.Name = "txtBillNo";
            this.txtBillNo.Size = new System.Drawing.Size(235, 22);
            this.txtBillNo.TabIndex = 103;
            this.txtBillNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBillNo_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(350, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 16);
            this.label6.TabIndex = 229;
            this.label6.Text = "Bill No :";
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(719, 14);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(71, 23);
            this.txtBillCode.TabIndex = 104;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(646, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 16);
            this.label10.TabIndex = 227;
            this.label10.Text = "Bill Code :";
            // 
            // txtToSerialNo
            // 
            this.txtToSerialNo.BackColor = System.Drawing.Color.White;
            this.txtToSerialNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToSerialNo.Location = new System.Drawing.Point(888, 43);
            this.txtToSerialNo.MaxLength = 6;
            this.txtToSerialNo.Name = "txtToSerialNo";
            this.txtToSerialNo.ReadOnly = true;
            this.txtToSerialNo.Size = new System.Drawing.Size(74, 23);
            this.txtToSerialNo.TabIndex = 119;
            this.txtToSerialNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // txtFromSerialNo
            // 
            this.txtFromSerialNo.BackColor = System.Drawing.Color.White;
            this.txtFromSerialNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromSerialNo.Location = new System.Drawing.Point(782, 43);
            this.txtFromSerialNo.MaxLength = 10;
            this.txtFromSerialNo.Name = "txtFromSerialNo";
            this.txtFromSerialNo.ReadOnly = true;
            this.txtFromSerialNo.Size = new System.Drawing.Size(74, 23);
            this.txtFromSerialNo.TabIndex = 118;
            this.txtFromSerialNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            this.txtFromSerialNo.Leave += new System.EventHandler(this.txtFromSerialNo_Leave);
            // 
            // chkSerial
            // 
            this.chkSerial.AutoSize = true;
            this.chkSerial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSerial.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkSerial.Location = new System.Drawing.Point(705, 45);
            this.chkSerial.Name = "chkSerial";
            this.chkSerial.Size = new System.Drawing.Size(77, 20);
            this.chkSerial.TabIndex = 117;
            this.chkSerial.Text = "Bill No :";
            this.chkSerial.UseVisualStyleBackColor = true;
            this.chkSerial.CheckedChanged += new System.EventHandler(this.chkSerial_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(859, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 16);
            this.label7.TabIndex = 225;
            this.label7.Text = "To";
            // 
            // txtSalesParty
            // 
            this.txtSalesParty.BackColor = System.Drawing.SystemColors.Window;
            this.txtSalesParty.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSalesParty.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtSalesParty.Location = new System.Drawing.Point(51, 14);
            this.txtSalesParty.Name = "txtSalesParty";
            this.txtSalesParty.ReadOnly = true;
            this.txtSalesParty.Size = new System.Drawing.Size(294, 22);
            this.txtSalesParty.TabIndex = 102;
            this.txtSalesParty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSalesParty_KeyDown);
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoWithoutLR);
            this.grpStatus.Controls.Add(this.rdoWithLR);
            this.grpStatus.Controls.Add(this.rdoAll);
            this.grpStatus.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.grpStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grpStatus.Location = new System.Drawing.Point(5, 33);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(209, 37);
            this.grpStatus.TabIndex = 108;
            this.grpStatus.TabStop = false;
            // 
            // rdoWithoutLR
            // 
            this.rdoWithoutLR.AutoSize = true;
            this.rdoWithoutLR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithoutLR.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithoutLR.Location = new System.Drawing.Point(119, 13);
            this.rdoWithoutLR.Name = "rdoWithoutLR";
            this.rdoWithoutLR.Size = new System.Drawing.Size(84, 18);
            this.rdoWithoutLR.TabIndex = 111;
            this.rdoWithoutLR.Text = "Without LR";
            this.rdoWithoutLR.UseVisualStyleBackColor = true;
            // 
            // rdoWithLR
            // 
            this.rdoWithLR.AutoSize = true;
            this.rdoWithLR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithLR.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithLR.Location = new System.Drawing.Point(47, 13);
            this.rdoWithLR.Name = "rdoWithLR";
            this.rdoWithLR.Size = new System.Drawing.Size(66, 18);
            this.rdoWithLR.TabIndex = 110;
            this.rdoWithLR.Text = "With LR";
            this.rdoWithLR.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(7, 13);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(39, 18);
            this.rdoAll.TabIndex = 109;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoWithoutWayBillNo);
            this.groupBox1.Controls.Add(this.rdoWithWayBill);
            this.groupBox1.Controls.Add(this.rdoWayBillAll);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Location = new System.Drawing.Point(216, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 37);
            this.groupBox1.TabIndex = 112;
            this.groupBox1.TabStop = false;
            // 
            // rdoWithoutWayBillNo
            // 
            this.rdoWithoutWayBillNo.AutoSize = true;
            this.rdoWithoutWayBillNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithoutWayBillNo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithoutWayBillNo.Location = new System.Drawing.Point(143, 13);
            this.rdoWithoutWayBillNo.Name = "rdoWithoutWayBillNo";
            this.rdoWithoutWayBillNo.Size = new System.Drawing.Size(111, 18);
            this.rdoWithoutWayBillNo.TabIndex = 115;
            this.rdoWithoutWayBillNo.Text = "Without Way Bill";
            this.rdoWithoutWayBillNo.UseVisualStyleBackColor = true;
            // 
            // rdoWithWayBill
            // 
            this.rdoWithWayBill.AutoSize = true;
            this.rdoWithWayBill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithWayBill.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithWayBill.Location = new System.Drawing.Point(47, 13);
            this.rdoWithWayBill.Name = "rdoWithWayBill";
            this.rdoWithWayBill.Size = new System.Drawing.Size(93, 18);
            this.rdoWithWayBill.TabIndex = 114;
            this.rdoWithWayBill.Text = "With Way Bill";
            this.rdoWithWayBill.UseVisualStyleBackColor = true;
            // 
            // rdoWayBillAll
            // 
            this.rdoWayBillAll.AutoSize = true;
            this.rdoWayBillAll.Checked = true;
            this.rdoWayBillAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWayBillAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWayBillAll.Location = new System.Drawing.Point(6, 13);
            this.rdoWayBillAll.Name = "rdoWayBillAll";
            this.rdoWayBillAll.Size = new System.Drawing.Size(39, 18);
            this.rdoWayBillAll.TabIndex = 113;
            this.rdoWayBillAll.TabStop = true;
            this.rdoWayBillAll.Text = "All";
            this.rdoWayBillAll.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1010, 562);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(62, 29);
            this.btnClose.TabIndex = 131;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblBill
            // 
            this.lblBill.AutoSize = true;
            this.lblBill.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblBill.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblBill.Location = new System.Drawing.Point(92, 569);
            this.lblBill.Name = "lblBill";
            this.lblBill.Size = new System.Drawing.Size(16, 16);
            this.lblBill.TabIndex = 121;
            this.lblBill.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(1, 569);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 16);
            this.label3.TabIndex = 120;
            this.label3.Text = "Total Bill No:";
            // 
            // dgrdBilty
            // 
            this.dgrdBilty.AllowUserToAddRows = false;
            this.dgrdBilty.AllowUserToDeleteRows = false;
            this.dgrdBilty.AllowUserToResizeRows = false;
            this.dgrdBilty.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdBilty.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdBilty.ColumnHeadersHeight = 30;
            this.dgrdBilty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdBilty.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chk,
            this.date,
            this.billNo,
            this.partyName,
            this.transportName,
            this.stationName,
            this.lrNumber,
            this.lrDate,
            this.waybillNo,
            this.waybillDate,
            this.description,
            this.supplierName,
            this.IRNNO,
            this.attachedBill,
            this.purchaseSNo,
            this.goodsType,
            this.oldLRNumber});
            this.dgrdBilty.EnableHeadersVisualStyles = false;
            this.dgrdBilty.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdBilty.Location = new System.Drawing.Point(5, 77);
            this.dgrdBilty.Name = "dgrdBilty";
            this.dgrdBilty.RowHeadersVisible = false;
            this.dgrdBilty.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdBilty.RowTemplate.Height = 27;
            this.dgrdBilty.Size = new System.Drawing.Size(1065, 480);
            this.dgrdBilty.TabIndex = 120;
            this.dgrdBilty.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdBilty_CellBeginEdit);
            this.dgrdBilty.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdBilty_CellClick);
            this.dgrdBilty.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdBilty_CellEndEdit);
            this.dgrdBilty.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgrdBilty_CellValidating);
            this.dgrdBilty.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdBilty_EditingControlShowing);
            this.dgrdBilty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdBilty_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoWithoutIRN);
            this.groupBox2.Controls.Add(this.rdoWithIRN);
            this.groupBox2.Controls.Add(this.rdoIAll);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox2.Location = new System.Drawing.Point(480, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 37);
            this.groupBox2.TabIndex = 113;
            this.groupBox2.TabStop = false;
            // 
            // rdoWithoutIRN
            // 
            this.rdoWithoutIRN.AutoSize = true;
            this.rdoWithoutIRN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithoutIRN.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithoutIRN.Location = new System.Drawing.Point(121, 13);
            this.rdoWithoutIRN.Name = "rdoWithoutIRN";
            this.rdoWithoutIRN.Size = new System.Drawing.Size(87, 18);
            this.rdoWithoutIRN.TabIndex = 116;
            this.rdoWithoutIRN.Text = "Without IRN";
            this.rdoWithoutIRN.UseVisualStyleBackColor = true;
            // 
            // rdoWithIRN
            // 
            this.rdoWithIRN.AutoSize = true;
            this.rdoWithIRN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithIRN.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoWithIRN.Location = new System.Drawing.Point(51, 13);
            this.rdoWithIRN.Name = "rdoWithIRN";
            this.rdoWithIRN.Size = new System.Drawing.Size(69, 18);
            this.rdoWithIRN.TabIndex = 115;
            this.rdoWithIRN.Text = "With IRN";
            this.rdoWithIRN.UseVisualStyleBackColor = true;
            // 
            // rdoIAll
            // 
            this.rdoIAll.AutoSize = true;
            this.rdoIAll.Checked = true;
            this.rdoIAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoIAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoIAll.Location = new System.Drawing.Point(9, 13);
            this.rdoIAll.Name = "rdoIAll";
            this.rdoIAll.Size = new System.Drawing.Size(39, 18);
            this.rdoIAll.TabIndex = 114;
            this.rdoIAll.TabStop = true;
            this.rdoIAll.Text = "All";
            this.rdoIAll.UseVisualStyleBackColor = true;
            // 
            // chk
            // 
            this.chk.HeaderText = "";
            this.chk.Name = "chk";
            this.chk.Width = 25;
            // 
            // date
            // 
            dataGridViewCellStyle6.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle6;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Width = 90;
            // 
            // billNo
            // 
            this.billNo.HeaderText = "Bill No";
            this.billNo.LinkColor = System.Drawing.Color.Black;
            this.billNo.Name = "billNo";
            this.billNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.billNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.billNo.Width = 120;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.Name = "partyName";
            this.partyName.Width = 250;
            // 
            // transportName
            // 
            this.transportName.HeaderText = "Transport Name";
            this.transportName.Name = "transportName";
            this.transportName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.transportName.Width = 150;
            // 
            // stationName
            // 
            this.stationName.HeaderText = "Station";
            this.stationName.Name = "stationName";
            this.stationName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.stationName.Visible = false;
            // 
            // lrNumber
            // 
            this.lrNumber.HeaderText = "LR No";
            this.lrNumber.Name = "lrNumber";
            this.lrNumber.Width = 120;
            // 
            // lrDate
            // 
            this.lrDate.HeaderText = "LR Date";
            this.lrDate.Name = "lrDate";
            this.lrDate.Width = 90;
            // 
            // waybillNo
            // 
            this.waybillNo.HeaderText = "Way Bill No";
            this.waybillNo.Name = "waybillNo";
            this.waybillNo.Width = 120;
            // 
            // waybillDate
            // 
            this.waybillDate.HeaderText = "Way Bill Date";
            this.waybillDate.Name = "waybillDate";
            this.waybillDate.Width = 120;
            // 
            // description
            // 
            this.description.HeaderText = "Description";
            this.description.Name = "description";
            // 
            // supplierName
            // 
            this.supplierName.HeaderText = "Supplier Name";
            this.supplierName.Name = "supplierName";
            this.supplierName.Width = 150;
            // 
            // IRNNO
            // 
            this.IRNNO.HeaderText = "IRNo";
            this.IRNNO.Name = "IRNNO";
            this.IRNNO.Width = 150;
            // 
            // attachedBill
            // 
            this.attachedBill.HeaderText = "Attached Bill";
            this.attachedBill.Name = "attachedBill";
            this.attachedBill.Visible = false;
            // 
            // purchaseSNo
            // 
            this.purchaseSNo.HeaderText = "PSNo";
            this.purchaseSNo.Name = "purchaseSNo";
            this.purchaseSNo.Visible = false;
            // 
            // goodsType
            // 
            this.goodsType.HeaderText = "Goods Type";
            this.goodsType.Name = "goodsType";
            this.goodsType.Visible = false;
            // 
            // oldLRNumber
            // 
            this.oldLRNumber.HeaderText = "Old LRNo";
            this.oldLRNumber.Name = "oldLRNumber";
            this.oldLRNumber.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.5F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(385, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "BILTY/E-WAYBILL/E-INVOICE DETAILS";
            // 
            // BiltyDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1100, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "BiltyDetails";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bilty Details";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BiltyDetails_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBilty)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblBill;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgrdBilty;
        private System.Windows.Forms.TextBox txtSalesParty;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RadioButton rdoWithoutLR;
        private System.Windows.Forms.RadioButton rdoWithLR;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoWithoutWayBillNo;
        private System.Windows.Forms.RadioButton rdoWithWayBill;
        private System.Windows.Forms.RadioButton rdoWayBillAll;
        protected internal System.Windows.Forms.TextBox txtToSerialNo;
        protected internal System.Windows.Forms.TextBox txtFromSerialNo;
        private System.Windows.Forms.CheckBox chkSerial;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Button btnGenerateJSON;
        private System.Windows.Forms.TextBox txtBillNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnImportExcel;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.Button btnEInvoice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdoWithoutIRN;
        private System.Windows.Forms.RadioButton rdoWithIRN;
        private System.Windows.Forms.RadioButton rdoIAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chk;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewLinkColumn billNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn transportName;
        private System.Windows.Forms.DataGridViewTextBoxColumn stationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn lrNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn lrDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn waybillNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn waybillDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.DataGridViewTextBoxColumn supplierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IRNNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn attachedBill;
        private System.Windows.Forms.DataGridViewTextBoxColumn purchaseSNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn goodsType;
        private System.Windows.Forms.DataGridViewTextBoxColumn oldLRNumber;
        private System.Windows.Forms.Label label1;
    }
}