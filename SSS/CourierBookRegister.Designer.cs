namespace SSS
{
    partial class CourierBookRegister
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoOut = new System.Windows.Forms.RadioButton();
            this.rdoIN = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.btnAdvanceSearch = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPartyName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSendEmail = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.txtSToSNo = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.txtSFromSNo = new System.Windows.Forms.TextBox();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdoWithoutCNo = new System.Windows.Forms.RadioButton();
            this.rdoWithCNo = new System.Windows.Forms.RadioButton();
            this.rdoCNoAll = new System.Windows.Forms.RadioButton();
            this.txtDocType = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.chkSSNo = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCToSNo = new System.Windows.Forms.TextBox();
            this.txtCFromSNo = new System.Windows.Forms.TextBox();
            this.chkSNo = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCourierName = new System.Windows.Forms.TextBox();
            this.txtCourierNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serialNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.saleBillNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.courierNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.courierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.docType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.station = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.btnAdvanceSearch);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtPartyName);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(5, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(989, 45);
            this.panel2.TabIndex = 100;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(523, 8);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(75, 23);
            this.txtToDate.TabIndex = 104;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(412, 8);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(81, 23);
            this.txtFromDate.TabIndex = 103;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoOut);
            this.groupBox2.Controls.Add(this.rdoIN);
            this.groupBox2.Controls.Add(this.rdoAll);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox2.Location = new System.Drawing.Point(615, -1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(167, 38);
            this.groupBox2.TabIndex = 105;
            this.groupBox2.TabStop = false;
            // 
            // rdoOut
            // 
            this.rdoOut.AutoSize = true;
            this.rdoOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoOut.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoOut.Location = new System.Drawing.Point(109, 13);
            this.rdoOut.Name = "rdoOut";
            this.rdoOut.Size = new System.Drawing.Size(49, 19);
            this.rdoOut.TabIndex = 108;
            this.rdoOut.Text = "OUT";
            this.rdoOut.UseVisualStyleBackColor = true;
            // 
            // rdoIN
            // 
            this.rdoIN.AutoSize = true;
            this.rdoIN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoIN.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoIN.Location = new System.Drawing.Point(63, 13);
            this.rdoIN.Name = "rdoIN";
            this.rdoIN.Size = new System.Drawing.Size(36, 19);
            this.rdoIN.TabIndex = 107;
            this.rdoIN.Text = "IN";
            this.rdoIN.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(11, 13);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(47, 19);
            this.rdoAll.TabIndex = 106;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "ALL";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // btnAdvanceSearch
            // 
            this.btnAdvanceSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdvanceSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdvanceSearch.ForeColor = System.Drawing.Color.White;
            this.btnAdvanceSearch.Location = new System.Drawing.Point(842, 5);
            this.btnAdvanceSearch.Name = "btnAdvanceSearch";
            this.btnAdvanceSearch.Size = new System.Drawing.Size(143, 33);
            this.btnAdvanceSearch.TabIndex = 110;
            this.btnAdvanceSearch.Text = "&Advance Search";
            this.btnAdvanceSearch.UseVisualStyleBackColor = false;
            this.btnAdvanceSearch.Click += new System.EventHandler(this.btnAdvanceSearch_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.Location = new System.Drawing.Point(346, 11);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(788, 5);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(53, 33);
            this.btnGo.TabIndex = 109;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(496, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 125;
            this.label2.Text = "To";
            // 
            // txtPartyName
            // 
            this.txtPartyName.BackColor = System.Drawing.Color.White;
            this.txtPartyName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPartyName.Location = new System.Drawing.Point(109, 9);
            this.txtPartyName.Name = "txtPartyName";
            this.txtPartyName.ReadOnly = true;
            this.txtPartyName.Size = new System.Drawing.Size(232, 23);
            this.txtPartyName.TabIndex = 101;
            this.txtPartyName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPartyName_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(1, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 16);
            this.label6.TabIndex = 126;
            this.label6.Text = "Party Name :";
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(802, 13);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(172, 23);
            this.txtBillCode.TabIndex = 114;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(715, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 16);
            this.label10.TabIndex = 134;
            this.label10.Text = "Bill Code :";
            // 
            // btnSendEmail
            // 
            this.btnSendEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSendEmail.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSendEmail.ForeColor = System.Drawing.Color.White;
            this.btnSendEmail.Location = new System.Drawing.Point(464, 14);
            this.btnSendEmail.Name = "btnSendEmail";
            this.btnSendEmail.Size = new System.Drawing.Size(120, 35);
            this.btnSendEmail.TabIndex = 125;
            this.btnSendEmail.Text = "Send Email";
            this.btnSendEmail.UseVisualStyleBackColor = false;
            this.btnSendEmail.Click += new System.EventHandler(this.btnSendEmail_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(18, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 16);
            this.label9.TabIndex = 157;
            this.label9.Text = "Total Courier :";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCount.Location = new System.Drawing.Point(140, 24);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(16, 16);
            this.lblCount.TabIndex = 158;
            this.lblCount.Text = "0";
            // 
            // txtSToSNo
            // 
            this.txtSToSNo.BackColor = System.Drawing.Color.White;
            this.txtSToSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSToSNo.Location = new System.Drawing.Point(249, 44);
            this.txtSToSNo.MaxLength = 6;
            this.txtSToSNo.Name = "txtSToSNo";
            this.txtSToSNo.ReadOnly = true;
            this.txtSToSNo.Size = new System.Drawing.Size(80, 23);
            this.txtSToSNo.TabIndex = 117;
            this.txtSToSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(585, 14);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(95, 35);
            this.btnExport.TabIndex = 126;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(870, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 35);
            this.btnClose.TabIndex = 129;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(769, 14);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(99, 35);
            this.btnPreview.TabIndex = 128;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(681, 14);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(87, 35);
            this.btnPrint.TabIndex = 127;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtSFromSNo
            // 
            this.txtSFromSNo.BackColor = System.Drawing.Color.White;
            this.txtSFromSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSFromSNo.Location = new System.Drawing.Point(141, 44);
            this.txtSFromSNo.MaxLength = 6;
            this.txtSFromSNo.Name = "txtSFromSNo";
            this.txtSFromSNo.ReadOnly = true;
            this.txtSFromSNo.Size = new System.Drawing.Size(80, 23);
            this.txtSFromSNo.TabIndex = 116;
            this.txtSFromSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.White;
            this.panelSearch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSearch.Controls.Add(this.groupBox3);
            this.panelSearch.Controls.Add(this.txtBillCode);
            this.panelSearch.Controls.Add(this.label10);
            this.panelSearch.Controls.Add(this.txtDocType);
            this.panelSearch.Controls.Add(this.label26);
            this.panelSearch.Controls.Add(this.txtSToSNo);
            this.panelSearch.Controls.Add(this.txtSFromSNo);
            this.panelSearch.Controls.Add(this.chkSSNo);
            this.panelSearch.Controls.Add(this.label3);
            this.panelSearch.Controls.Add(this.txtCToSNo);
            this.panelSearch.Controls.Add(this.txtCFromSNo);
            this.panelSearch.Controls.Add(this.chkSNo);
            this.panelSearch.Controls.Add(this.label4);
            this.panelSearch.Controls.Add(this.txtCourierName);
            this.panelSearch.Controls.Add(this.txtCourierNo);
            this.panelSearch.Controls.Add(this.label5);
            this.panelSearch.Controls.Add(this.label11);
            this.panelSearch.Controls.Add(this.btnCancel);
            this.panelSearch.Controls.Add(this.btnSearch);
            this.panelSearch.Location = new System.Drawing.Point(6, 109);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(988, 128);
            this.panelSearch.TabIndex = 111;
            this.panelSearch.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdoWithoutCNo);
            this.groupBox3.Controls.Add(this.rdoWithCNo);
            this.groupBox3.Controls.Add(this.rdoCNoAll);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox3.Location = new System.Drawing.Point(24, 73);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(304, 38);
            this.groupBox3.TabIndex = 122;
            this.groupBox3.TabStop = false;
            // 
            // rdoWithoutCNo
            // 
            this.rdoWithoutCNo.AutoSize = true;
            this.rdoWithoutCNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithoutCNo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoWithoutCNo.Location = new System.Drawing.Point(166, 13);
            this.rdoWithoutCNo.Name = "rdoWithoutCNo";
            this.rdoWithoutCNo.Size = new System.Drawing.Size(132, 19);
            this.rdoWithoutCNo.TabIndex = 125;
            this.rdoWithoutCNo.Text = "Without Courier No";
            this.rdoWithoutCNo.UseVisualStyleBackColor = true;
            // 
            // rdoWithCNo
            // 
            this.rdoWithCNo.AutoSize = true;
            this.rdoWithCNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoWithCNo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoWithCNo.Location = new System.Drawing.Point(51, 13);
            this.rdoWithCNo.Name = "rdoWithCNo";
            this.rdoWithCNo.Size = new System.Drawing.Size(114, 19);
            this.rdoWithCNo.TabIndex = 124;
            this.rdoWithCNo.Text = "With Courier No";
            this.rdoWithCNo.UseVisualStyleBackColor = true;
            // 
            // rdoCNoAll
            // 
            this.rdoCNoAll.AutoSize = true;
            this.rdoCNoAll.Checked = true;
            this.rdoCNoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoCNoAll.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoCNoAll.Location = new System.Drawing.Point(8, 13);
            this.rdoCNoAll.Name = "rdoCNoAll";
            this.rdoCNoAll.Size = new System.Drawing.Size(39, 19);
            this.rdoCNoAll.TabIndex = 123;
            this.rdoCNoAll.TabStop = true;
            this.rdoCNoAll.Text = "All";
            this.rdoCNoAll.UseVisualStyleBackColor = true;
            // 
            // txtDocType
            // 
            this.txtDocType.BackColor = System.Drawing.Color.White;
            this.txtDocType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDocType.Font = new System.Drawing.Font("Arial", 10F);
            this.txtDocType.Location = new System.Drawing.Point(802, 46);
            this.txtDocType.Name = "txtDocType";
            this.txtDocType.ReadOnly = true;
            this.txtDocType.Size = new System.Drawing.Size(172, 23);
            this.txtDocType.TabIndex = 121;
            this.txtDocType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDocType_KeyDown);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(712, 49);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(79, 16);
            this.label26.TabIndex = 170;
            this.label26.Text = "Doc Type :";
            // 
            // chkSSNo
            // 
            this.chkSSNo.AutoSize = true;
            this.chkSSNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSSNo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chkSSNo.Location = new System.Drawing.Point(25, 47);
            this.chkSSNo.Name = "chkSSNo";
            this.chkSSNo.Size = new System.Drawing.Size(102, 20);
            this.chkSSNo.TabIndex = 115;
            this.chkSSNo.Text = "Sale B.No :";
            this.chkSSNo.UseVisualStyleBackColor = true;
            this.chkSSNo.CheckedChanged += new System.EventHandler(this.chkSSNo_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(224, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 16);
            this.label3.TabIndex = 167;
            this.label3.Text = "To";
            // 
            // txtCToSNo
            // 
            this.txtCToSNo.BackColor = System.Drawing.Color.White;
            this.txtCToSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCToSNo.Location = new System.Drawing.Point(602, 46);
            this.txtCToSNo.MaxLength = 6;
            this.txtCToSNo.Name = "txtCToSNo";
            this.txtCToSNo.ReadOnly = true;
            this.txtCToSNo.Size = new System.Drawing.Size(90, 23);
            this.txtCToSNo.TabIndex = 120;
            this.txtCToSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // txtCFromSNo
            // 
            this.txtCFromSNo.BackColor = System.Drawing.Color.White;
            this.txtCFromSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCFromSNo.Location = new System.Drawing.Point(479, 46);
            this.txtCFromSNo.MaxLength = 6;
            this.txtCFromSNo.Name = "txtCFromSNo";
            this.txtCFromSNo.ReadOnly = true;
            this.txtCFromSNo.Size = new System.Drawing.Size(87, 23);
            this.txtCFromSNo.TabIndex = 119;
            this.txtCFromSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // chkSNo
            // 
            this.chkSNo.AutoSize = true;
            this.chkSNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSNo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chkSNo.Location = new System.Drawing.Point(369, 48);
            this.chkSNo.Name = "chkSNo";
            this.chkSNo.Size = new System.Drawing.Size(98, 20);
            this.chkSNo.TabIndex = 118;
            this.chkSNo.Text = "Serial No :";
            this.chkSNo.UseVisualStyleBackColor = true;
            this.chkSNo.CheckedChanged += new System.EventHandler(this.chkSNo_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(572, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 16);
            this.label4.TabIndex = 164;
            this.label4.Text = "To";
            // 
            // txtCourierName
            // 
            this.txtCourierName.BackColor = System.Drawing.Color.White;
            this.txtCourierName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCourierName.Location = new System.Drawing.Point(141, 13);
            this.txtCourierName.Name = "txtCourierName";
            this.txtCourierName.ReadOnly = true;
            this.txtCourierName.Size = new System.Drawing.Size(188, 23);
            this.txtCourierName.TabIndex = 112;
            this.txtCourierName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourierName_KeyDown);
            // 
            // txtCourierNo
            // 
            this.txtCourierNo.BackColor = System.Drawing.Color.White;
            this.txtCourierNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCourierNo.Location = new System.Drawing.Point(479, 13);
            this.txtCourierNo.Name = "txtCourierNo";
            this.txtCourierNo.Size = new System.Drawing.Size(213, 23);
            this.txtCourierNo.TabIndex = 113;
            this.txtCourierNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCourierNo_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(9, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 142;
            this.label5.Text = "Courier Name :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(371, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 16);
            this.label11.TabIndex = 140;
            this.label11.Text = "Courier No :";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(892, 78);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 34);
            this.btnCancel.TabIndex = 127;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(797, 78);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(92, 34);
            this.btnSearch.TabIndex = 126;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSendEmail);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblCount);
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.btnPrint);
            this.groupBox1.Location = new System.Drawing.Point(8, 478);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(971, 55);
            this.groupBox1.TabIndex = 124;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Location = new System.Drawing.Point(5, 110);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(990, 540);
            this.panel3.TabIndex = 151;
            this.panel3.TabStop = true;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(238)))), ((int)(((byte)(239)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.White;
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
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.cStatus,
            this.date,
            this.serialNo,
            this.saleBillNo,
            this.partyName,
            this.courierNo,
            this.courierName,
            this.docType,
            this.station,
            this.remark,
            this.createdBy});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(4, 4);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.Size = new System.Drawing.Size(978, 477);
            this.dgrdDetails.TabIndex = 121;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellEndEdit);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(8, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(985, 42);
            this.panel1.TabIndex = 150;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(341, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "COURIER REGISTER (IN/OUT)";
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            this.id.Width = 10;
            // 
            // cStatus
            // 
            this.cStatus.HeaderText = "";
            this.cStatus.Name = "cStatus";
            this.cStatus.ReadOnly = true;
            this.cStatus.Width = 40;
            // 
            // date
            // 
            dataGridViewCellStyle3.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle3;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 85;
            // 
            // serialNo
            // 
            this.serialNo.HeaderText = "Serial No";
            this.serialNo.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.serialNo.LinkColor = System.Drawing.Color.Black;
            this.serialNo.Name = "serialNo";
            this.serialNo.ReadOnly = true;
            this.serialNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.serialNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.serialNo.Width = 120;
            // 
            // saleBillNo
            // 
            this.saleBillNo.HeaderText = "Sale Bill No";
            this.saleBillNo.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.saleBillNo.LinkColor = System.Drawing.Color.Black;
            this.saleBillNo.Name = "saleBillNo";
            this.saleBillNo.ReadOnly = true;
            this.saleBillNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.saleBillNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.saleBillNo.Width = 125;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.Name = "partyName";
            this.partyName.ReadOnly = true;
            this.partyName.Width = 200;
            // 
            // courierNo
            // 
            this.courierNo.HeaderText = "Courier No";
            this.courierNo.Name = "courierNo";
            this.courierNo.ReadOnly = true;
            this.courierNo.Width = 120;
            // 
            // courierName
            // 
            this.courierName.HeaderText = "Courier Name";
            this.courierName.Name = "courierName";
            this.courierName.ReadOnly = true;
            this.courierName.Width = 120;
            // 
            // docType
            // 
            this.docType.HeaderText = "Doc.Type";
            this.docType.Name = "docType";
            this.docType.ReadOnly = true;
            this.docType.Width = 80;
            // 
            // station
            // 
            this.station.HeaderText = "Station";
            this.station.Name = "station";
            this.station.ReadOnly = true;
            // 
            // remark
            // 
            this.remark.HeaderText = "Remark";
            this.remark.Name = "remark";
            this.remark.ReadOnly = true;
            // 
            // createdBy
            // 
            this.createdBy.HeaderText = "Created By";
            this.createdBy.Name = "createdBy";
            this.createdBy.ReadOnly = true;
            // 
            // CourierBookRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CourierBookRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Courier Book Register";
            this.Load += new System.EventHandler(this.CourierBookRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CourierBookRegister_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnAdvanceSearch;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.TextBox txtPartyName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSendEmail;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCount;
        protected internal System.Windows.Forms.TextBox txtSToSNo;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        protected internal System.Windows.Forms.TextBox txtSFromSNo;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.CheckBox chkSSNo;
        private System.Windows.Forms.Label label3;
        protected internal System.Windows.Forms.TextBox txtCToSNo;
        protected internal System.Windows.Forms.TextBox txtCFromSNo;
        private System.Windows.Forms.CheckBox chkSNo;
        private System.Windows.Forms.Label label4;
        protected internal System.Windows.Forms.TextBox txtCourierName;
        protected internal System.Windows.Forms.TextBox txtCourierNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdoOut;
        private System.Windows.Forms.RadioButton rdoIN;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.TextBox txtDocType;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdoWithoutCNo;
        private System.Windows.Forms.RadioButton rdoWithCNo;
        private System.Windows.Forms.RadioButton rdoCNoAll;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewLinkColumn serialNo;
        private System.Windows.Forms.DataGridViewLinkColumn saleBillNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn courierNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn courierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn docType;
        private System.Windows.Forms.DataGridViewTextBoxColumn station;
        private System.Windows.Forms.DataGridViewTextBoxColumn remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdBy;
    }
}