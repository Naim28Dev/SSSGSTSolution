namespace SSS
{
    partial class CashBook
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panDisp = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblLedgerBal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCurrentAmount = new System.Windows.Forms.Label();
            this.lblCashBalance = new System.Windows.Forms.Label();
            this.chkSendSMS = new System.Windows.Forms.CheckBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnGenerateTCS = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblTotalAmt = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panAccountHead = new System.Windows.Forms.Panel();
            this.txtDate = new System.Windows.Forms.MaskedTextBox();
            this.txtGSTNature = new System.Windows.Forms.TextBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.txtVoucherCode = new System.Windows.Forms.TextBox();
            this.txtCashAccount = new System.Windows.Forms.TextBox();
            this.lblCashAccount = new System.Windows.Forms.Label();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.grpPaymentype = new System.Windows.Forms.GroupBox();
            this.rdoReceipt = new System.Windows.Forms.RadioButton();
            this.rdoPayment = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.SrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.particular = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDeletionConfirmation = new System.Windows.Forms.Panel();
            this.btnDeletionClose = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnFinalDelete = new System.Windows.Forms.Button();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.LinkLabel();
            this.panDisp.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panAccountHead.SuspendLayout();
            this.grpPaymentype.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.pnlDeletionConfirmation.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // panDisp
            // 
            this.panDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panDisp.AutoSize = true;
            this.panDisp.BackColor = System.Drawing.Color.White;
            this.panDisp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panDisp.Controls.Add(this.lblName);
            this.panDisp.Controls.Add(this.label11);
            this.panDisp.Location = new System.Drawing.Point(17, 15);
            this.panDisp.Name = "panDisp";
            this.panDisp.Size = new System.Drawing.Size(1016, 44);
            this.panDisp.TabIndex = 218;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.Black;
            this.lblName.Location = new System.Drawing.Point(415, 11);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(183, 19);
            this.lblName.TabIndex = 221;
            this.lblName.Text = "CASH BOOK DETAILS";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(824, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(184, 15);
            this.label11.TabIndex = 126;
            this.label11.Text = "Press F1 to Delete Current Row";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.lblLedgerBal);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.lblCurrentAmount);
            this.panel3.Controls.Add(this.lblCashBalance);
            this.panel3.Controls.Add(this.chkSendSMS);
            this.panel3.Controls.Add(this.lblMsg);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.lblTotalAmt);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.panAccountHead);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.panel3.Location = new System.Drawing.Point(17, 80);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1016, 559);
            this.panel3.TabIndex = 99;
            // 
            // lblLedgerBal
            // 
            this.lblLedgerBal.AutoSize = true;
            this.lblLedgerBal.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.lblLedgerBal.Location = new System.Drawing.Point(257, 463);
            this.lblLedgerBal.Name = "lblLedgerBal";
            this.lblLedgerBal.Size = new System.Drawing.Size(36, 18);
            this.lblLedgerBal.TabIndex = 131;
            this.lblLedgerBal.Text = "0.00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(139, 464);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 16);
            this.label2.TabIndex = 130;
            this.label2.Text = "Ledger Balance :";
            // 
            // lblCurrentAmount
            // 
            this.lblCurrentAmount.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCurrentAmount.Location = new System.Drawing.Point(20, 75);
            this.lblCurrentAmount.Name = "lblCurrentAmount";
            this.lblCurrentAmount.Size = new System.Drawing.Size(952, 23);
            this.lblCurrentAmount.TabIndex = 129;
            this.lblCurrentAmount.Text = "  ";
            this.lblCurrentAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCashBalance
            // 
            this.lblCashBalance.AutoSize = true;
            this.lblCashBalance.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.lblCashBalance.Location = new System.Drawing.Point(539, 464);
            this.lblCashBalance.Name = "lblCashBalance";
            this.lblCashBalance.Size = new System.Drawing.Size(36, 18);
            this.lblCashBalance.TabIndex = 128;
            this.lblCashBalance.Text = "0.00";
            // 
            // chkSendSMS
            // 
            this.chkSendSMS.AutoSize = true;
            this.chkSendSMS.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSendSMS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSendSMS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkSendSMS.ForeColor = System.Drawing.Color.Black;
            this.chkSendSMS.Location = new System.Drawing.Point(23, 463);
            this.chkSendSMS.Name = "chkSendSMS";
            this.chkSendSMS.Size = new System.Drawing.Size(93, 20);
            this.chkSendSMS.TabIndex = 111;
            this.chkSendSMS.Text = "Send S&MS";
            this.chkSendSMS.UseVisualStyleBackColor = true;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblMsg.Location = new System.Drawing.Point(134, 3);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(13, 15);
            this.lblMsg.TabIndex = 126;
            this.lblMsg.Text = "  ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(430, 464);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 16);
            this.label15.TabIndex = 127;
            this.label15.Text = "Cash Balance :";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.btnGenerateTCS);
            this.panel4.Controls.Add(this.btnSearch);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.btnPreview);
            this.panel4.Controls.Add(this.btnPrint);
            this.panel4.Controls.Add(this.btnDelete);
            this.panel4.Controls.Add(this.btnEdit);
            this.panel4.Controls.Add(this.btnAdd);
            this.panel4.Location = new System.Drawing.Point(15, 489);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(981, 54);
            this.panel4.TabIndex = 112;
            // 
            // btnGenerateTCS
            // 
            this.btnGenerateTCS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGenerateTCS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerateTCS.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerateTCS.ForeColor = System.Drawing.Color.White;
            this.btnGenerateTCS.Location = new System.Drawing.Point(477, 6);
            this.btnGenerateTCS.Name = "btnGenerateTCS";
            this.btnGenerateTCS.Size = new System.Drawing.Size(137, 37);
            this.btnGenerateTCS.TabIndex = 117;
            this.btnGenerateTCS.Text = "&Generate TCS";
            this.btnGenerateTCS.UseVisualStyleBackColor = false;
            this.btnGenerateTCS.Click += new System.EventHandler(this.btnGenerateTCS_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(365, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(106, 37);
            this.btnSearch.TabIndex = 116;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(850, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(106, 37);
            this.btnClose.TabIndex = 120;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(735, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(110, 37);
            this.btnPreview.TabIndex = 119;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(621, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(106, 37);
            this.btnPrint.TabIndex = 118;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(251, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(106, 37);
            this.btnDelete.TabIndex = 115;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(137, 6);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(106, 37);
            this.btnEdit.TabIndex = 114;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(25, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 37);
            this.btnAdd.TabIndex = 113;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.BackColor = System.Drawing.Color.White;
            this.lblTotalAmt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblTotalAmt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmt.Location = new System.Drawing.Point(815, 464);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.ReadOnly = true;
            this.lblTotalAmt.Size = new System.Drawing.Size(138, 16);
            this.lblTotalAmt.TabIndex = 108;
            this.lblTotalAmt.TabStop = false;
            this.lblTotalAmt.Text = "0.00";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(709, 464);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 16);
            this.label12.TabIndex = 117;
            this.label12.Text = "Total Amount :";
            // 
            // panAccountHead
            // 
            this.panAccountHead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panAccountHead.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panAccountHead.Controls.Add(this.txtDate);
            this.panAccountHead.Controls.Add(this.txtGSTNature);
            this.panAccountHead.Controls.Add(this.lblAccount);
            this.panAccountHead.Controls.Add(this.txtVoucherCode);
            this.panAccountHead.Controls.Add(this.txtCashAccount);
            this.panAccountHead.Controls.Add(this.lblCashAccount);
            this.panAccountHead.Controls.Add(this.txtVoucherNo);
            this.panAccountHead.Controls.Add(this.Label10);
            this.panAccountHead.Controls.Add(this.grpPaymentype);
            this.panAccountHead.Location = new System.Drawing.Point(15, 22);
            this.panAccountHead.Name = "panAccountHead";
            this.panAccountHead.Size = new System.Drawing.Size(981, 52);
            this.panAccountHead.TabIndex = 100;
            // 
            // txtDate
            // 
            this.txtDate.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtDate.Location = new System.Drawing.Point(226, 12);
            this.txtDate.Mask = "00/00/0000";
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(79, 22);
            this.txtDate.TabIndex = 103;
            this.txtDate.Enter += new System.EventHandler(this.txtDate_Enter);
            this.txtDate.Leave += new System.EventHandler(this.txtDate_Leave);
            // 
            // txtGSTNature
            // 
            this.txtGSTNature.BackColor = System.Drawing.Color.White;
            this.txtGSTNature.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGSTNature.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGSTNature.Location = new System.Drawing.Point(778, 11);
            this.txtGSTNature.MaxLength = 40;
            this.txtGSTNature.Name = "txtGSTNature";
            this.txtGSTNature.ReadOnly = true;
            this.txtGSTNature.Size = new System.Drawing.Size(188, 22);
            this.txtGSTNature.TabIndex = 108;
            this.txtGSTNature.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGSTNature_KeyDown);
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccount.ForeColor = System.Drawing.Color.White;
            this.lblAccount.Location = new System.Drawing.Point(687, 14);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(91, 16);
            this.lblAccount.TabIndex = 124;
            this.lblAccount.Text = "Gst Nature :";
            this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVoucherCode
            // 
            this.txtVoucherCode.BackColor = System.Drawing.Color.White;
            this.txtVoucherCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVoucherCode.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherCode.Location = new System.Drawing.Point(96, 12);
            this.txtVoucherCode.Name = "txtVoucherCode";
            this.txtVoucherCode.ReadOnly = true;
            this.txtVoucherCode.Size = new System.Drawing.Size(70, 22);
            this.txtVoucherCode.TabIndex = 101;
            this.txtVoucherCode.TabStop = false;
            this.txtVoucherCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVoucherCode_KeyDown);
            // 
            // txtCashAccount
            // 
            this.txtCashAccount.BackColor = System.Drawing.Color.White;
            this.txtCashAccount.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCashAccount.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCashAccount.Location = new System.Drawing.Point(387, 12);
            this.txtCashAccount.Name = "txtCashAccount";
            this.txtCashAccount.ReadOnly = true;
            this.txtCashAccount.Size = new System.Drawing.Size(168, 22);
            this.txtCashAccount.TabIndex = 104;
            this.txtCashAccount.DoubleClick += new System.EventHandler(this.txtCashAccount_DoubleClick);
            this.txtCashAccount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCash_KeyDown);
            // 
            // lblCashAccount
            // 
            this.lblCashAccount.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCashAccount.ForeColor = System.Drawing.Color.White;
            this.lblCashAccount.Location = new System.Drawing.Point(313, 15);
            this.lblCashAccount.Name = "lblCashAccount";
            this.lblCashAccount.Size = new System.Drawing.Size(73, 16);
            this.lblCashAccount.TabIndex = 121;
            this.lblCashAccount.Text = "Cash A/c :";
            this.lblCashAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.BackColor = System.Drawing.Color.White;
            this.txtVoucherNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVoucherNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherNo.Location = new System.Drawing.Point(167, 12);
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(58, 22);
            this.txtVoucherNo.TabIndex = 102;
            this.txtVoucherNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVoucherNo_KeyPress);
            this.txtVoucherNo.Leave += new System.EventHandler(this.txtVoucherNo_Leave);
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.ForeColor = System.Drawing.Color.White;
            this.Label10.Location = new System.Drawing.Point(3, 14);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(89, 16);
            this.Label10.TabIndex = 116;
            this.Label10.Text = "Voucher No :";
            // 
            // grpPaymentype
            // 
            this.grpPaymentype.Controls.Add(this.rdoReceipt);
            this.grpPaymentype.Controls.Add(this.rdoPayment);
            this.grpPaymentype.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.grpPaymentype.Location = new System.Drawing.Point(564, 1);
            this.grpPaymentype.Name = "grpPaymentype";
            this.grpPaymentype.Size = new System.Drawing.Size(120, 36);
            this.grpPaymentype.TabIndex = 105;
            this.grpPaymentype.TabStop = false;
            // 
            // rdoReceipt
            // 
            this.rdoReceipt.AutoSize = true;
            this.rdoReceipt.Checked = true;
            this.rdoReceipt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoReceipt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoReceipt.ForeColor = System.Drawing.Color.White;
            this.rdoReceipt.Location = new System.Drawing.Point(4, 11);
            this.rdoReceipt.Name = "rdoReceipt";
            this.rdoReceipt.Size = new System.Drawing.Size(54, 19);
            this.rdoReceipt.TabIndex = 106;
            this.rdoReceipt.TabStop = true;
            this.rdoReceipt.Text = "&Debit";
            this.rdoReceipt.UseVisualStyleBackColor = true;
            this.rdoReceipt.CheckedChanged += new System.EventHandler(this.rdoReceipt_CheckedChanged);
            // 
            // rdoPayment
            // 
            this.rdoPayment.AutoSize = true;
            this.rdoPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoPayment.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoPayment.ForeColor = System.Drawing.Color.White;
            this.rdoPayment.Location = new System.Drawing.Point(59, 11);
            this.rdoPayment.Name = "rdoPayment";
            this.rdoPayment.Size = new System.Drawing.Size(59, 19);
            this.rdoPayment.TabIndex = 107;
            this.rdoPayment.TabStop = true;
            this.rdoPayment.Text = "&Credit";
            this.rdoPayment.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dgrdDetails);
            this.panel2.Location = new System.Drawing.Point(15, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(981, 357);
            this.panel2.TabIndex = 109;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 32;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SrNo,
            this.accountName,
            this.particular,
            this.amount,
            this.gridID,
            this.groupName});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.Color.Maroon;
            this.dgrdDetails.ImeMode = System.Windows.Forms.ImeMode.On;
            this.dgrdDetails.Location = new System.Drawing.Point(22, 20);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F);
            this.dgrdDetails.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.Size = new System.Drawing.Size(933, 315);
            this.dgrdDetails.TabIndex = 110;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellEndEdit);
            this.dgrdDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdDetails_EditingControlShowing);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            this.dgrdDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgrdDetails_KeyPress);
            // 
            // SrNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            this.SrNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.SrNo.HeaderText = "S.No.";
            this.SrNo.Name = "SrNo";
            this.SrNo.ReadOnly = true;
            this.SrNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SrNo.Width = 50;
            // 
            // accountName
            // 
            this.accountName.HeaderText = "CREDIT ACCOUNT NAME";
            this.accountName.Name = "accountName";
            this.accountName.Width = 300;
            // 
            // particular
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.particular.DefaultCellStyle = dataGridViewCellStyle3;
            this.particular.HeaderText = "PARTICULARS";
            this.particular.Name = "particular";
            this.particular.Width = 425;
            // 
            // amount
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = null;
            this.amount.DefaultCellStyle = dataGridViewCellStyle4;
            this.amount.HeaderText = "AMOUNT";
            this.amount.Name = "amount";
            this.amount.Width = 130;
            // 
            // gridID
            // 
            this.gridID.HeaderText = "ID";
            this.gridID.Name = "gridID";
            this.gridID.Visible = false;
            // 
            // groupName
            // 
            this.groupName.HeaderText = "GroupName";
            this.groupName.Name = "groupName";
            this.groupName.Visible = false;
            // 
            // pnlDeletionConfirmation
            // 
            this.pnlDeletionConfirmation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlDeletionConfirmation.Controls.Add(this.btnDeletionClose);
            this.pnlDeletionConfirmation.Controls.Add(this.panel8);
            this.pnlDeletionConfirmation.Location = new System.Drawing.Point(213, 284);
            this.pnlDeletionConfirmation.Name = "pnlDeletionConfirmation";
            this.pnlDeletionConfirmation.Size = new System.Drawing.Size(575, 87);
            this.pnlDeletionConfirmation.TabIndex = 501;
            this.pnlDeletionConfirmation.Visible = false;
            // 
            // btnDeletionClose
            // 
            this.btnDeletionClose.BackgroundImage = global::SSS.Properties.Resources.close;
            this.btnDeletionClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDeletionClose.Location = new System.Drawing.Point(550, -1);
            this.btnDeletionClose.Name = "btnDeletionClose";
            this.btnDeletionClose.Size = new System.Drawing.Size(21, 19);
            this.btnDeletionClose.TabIndex = 504;
            this.btnDeletionClose.Tag = "Close";
            this.btnDeletionClose.UseVisualStyleBackColor = true;
            this.btnDeletionClose.Click += new System.EventHandler(this.btnDeletionClose_Click);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel8.Controls.Add(this.btnFinalDelete);
            this.panel8.Controls.Add(this.txtReason);
            this.panel8.Controls.Add(this.label34);
            this.panel8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.panel8.Location = new System.Drawing.Point(13, 16);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(537, 51);
            this.panel8.TabIndex = 501;
            // 
            // btnFinalDelete
            // 
            this.btnFinalDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnFinalDelete.ForeColor = System.Drawing.Color.White;
            this.btnFinalDelete.Location = new System.Drawing.Point(451, 8);
            this.btnFinalDelete.Name = "btnFinalDelete";
            this.btnFinalDelete.Size = new System.Drawing.Size(75, 32);
            this.btnFinalDelete.TabIndex = 503;
            this.btnFinalDelete.Text = "Con&firm";
            this.btnFinalDelete.UseVisualStyleBackColor = false;
            this.btnFinalDelete.Click += new System.EventHandler(this.btnFinalDelete_Click);
            // 
            // txtReason
            // 
            this.txtReason.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtReason.Font = new System.Drawing.Font("Arial", 10F);
            this.txtReason.Location = new System.Drawing.Point(131, 12);
            this.txtReason.MaxLength = 100;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(317, 23);
            this.txtReason.TabIndex = 502;
            this.txtReason.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReason_KeyPress);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label34.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label34.Location = new System.Drawing.Point(2, 15);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(126, 15);
            this.label34.TabIndex = 181;
            this.label34.Text = "Reason For Deletion :";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCreatedBy.LinkColor = System.Drawing.Color.White;
            this.lblCreatedBy.Location = new System.Drawing.Point(482, 60);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(549, 16);
            this.lblCreatedBy.TabIndex = 505;
            this.lblCreatedBy.TabStop = true;
            this.lblCreatedBy.Text = "CreatedBy";
            this.lblCreatedBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCreatedBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCreatedBy_LinkClicked);
            // 
            // CashBook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 655);
            this.Controls.Add(this.lblCreatedBy);
            this.Controls.Add(this.pnlDeletionConfirmation);
            this.Controls.Add(this.panDisp);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CashBook";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cash Book";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CashBook_FormClosing);
            this.Load += new System.EventHandler(this.CashBook_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CashBook_KeyDown);
            this.panDisp.ResumeLayout(false);
            this.panDisp.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panAccountHead.ResumeLayout(false);
            this.panAccountHead.PerformLayout();
            this.grpPaymentype.ResumeLayout(false);
            this.grpPaymentype.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.pnlDeletionConfirmation.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panDisp;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblCashBalance;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox lblTotalAmt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panAccountHead;
        private System.Windows.Forms.TextBox txtCashAccount;
        private System.Windows.Forms.Label lblCashAccount;
        private System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.CheckBox chkSendSMS;
        private System.Windows.Forms.Label lblCurrentAmount;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtVoucherCode;
        private System.Windows.Forms.GroupBox grpPaymentype;
        protected internal System.Windows.Forms.RadioButton rdoPayment;
        protected internal System.Windows.Forms.RadioButton rdoReceipt;
        private System.Windows.Forms.Panel pnlDeletionConfirmation;
        private System.Windows.Forms.Button btnDeletionClose;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnFinalDelete;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.LinkLabel lblCreatedBy;
        private System.Windows.Forms.TextBox txtGSTNature;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SrNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn particular;
        private System.Windows.Forms.DataGridViewTextBoxColumn amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridID;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupName;
        private System.Windows.Forms.Button btnGenerateTCS;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblLedgerBal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox txtDate;
    }
}