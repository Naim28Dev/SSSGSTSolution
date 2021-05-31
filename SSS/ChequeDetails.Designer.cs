namespace SSS
{
    partial class ChequeDetails
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
            this.btnName = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.SrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.branchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chequeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.particular = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDeletionConfirmation = new System.Windows.Forms.Panel();
            this.btnDeletionClose = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnFinalDelete = new System.Windows.Forms.Button();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.txtVoucherCode = new System.Windows.Forms.TextBox();
            this.lblCreatedBy = new System.Windows.Forms.LinkLabel();
            this.chkSendSMS = new System.Windows.Forms.CheckBox();
            this.panDisp = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblCurrentAmount = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
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
            this.txtDepositeDate = new System.Windows.Forms.MaskedTextBox();
            this.txtDate = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpPaymentype = new System.Windows.Forms.GroupBox();
            this.rdoPDCCheque = new System.Windows.Forms.RadioButton();
            this.rdoSecurityChq = new System.Windows.Forms.RadioButton();
            this.txtBankAccount = new System.Windows.Forms.TextBox();
            this.lblBankAccount = new System.Windows.Forms.Label();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoDebit = new System.Windows.Forms.RadioButton();
            this.rdoCredit = new System.Windows.Forms.RadioButton();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.pnlDeletionConfirmation.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panDisp.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panAccountHead.SuspendLayout();
            this.grpPaymentype.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnName
            // 
            this.btnName.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnName.Location = new System.Drawing.Point(899, 8);
            this.btnName.Name = "btnName";
            this.btnName.Size = new System.Drawing.Size(24, 23);
            this.btnName.TabIndex = 106;
            this.btnName.TabStop = false;
            this.btnName.UseVisualStyleBackColor = true;
            this.btnName.Click += new System.EventHandler(this.btnName_Click);
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
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dgrdDetails);
            this.panel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(20, 129);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1023, 338);
            this.panel2.TabIndex = 111;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
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
            this.bankName,
            this.branchName,
            this.firmName,
            this.chequeNo,
            this.particular,
            this.amount,
            this.gridID});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.Color.Maroon;
            this.dgrdDetails.ImeMode = System.Windows.Forms.ImeMode.On;
            this.dgrdDetails.Location = new System.Drawing.Point(17, 15);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F);
            this.dgrdDetails.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.Size = new System.Drawing.Size(985, 301);
            this.dgrdDetails.TabIndex = 112;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellEndEdit);
            this.dgrdDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdDetails_EditingControlShowing);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
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
            this.SrNo.HeaderText = "S.No";
            this.SrNo.Name = "SrNo";
            this.SrNo.ReadOnly = true;
            this.SrNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SrNo.Width = 40;
            // 
            // accountName
            // 
            this.accountName.HeaderText = "CREDIT ACCOUNT NAME";
            this.accountName.Name = "accountName";
            this.accountName.Width = 200;
            // 
            // bankName
            // 
            this.bankName.HeaderText = "BANK NAME";
            this.bankName.Name = "bankName";
            this.bankName.Width = 150;
            // 
            // branchName
            // 
            this.branchName.HeaderText = "BRANCH NAME";
            this.branchName.Name = "branchName";
            this.branchName.Width = 110;
            // 
            // firmName
            // 
            this.firmName.HeaderText = "FIRM NAME";
            this.firmName.Name = "firmName";
            this.firmName.Width = 130;
            // 
            // chequeNo
            // 
            this.chequeNo.HeaderText = "CHEQUE NO";
            this.chequeNo.Name = "chequeNo";
            // 
            // particular
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.particular.DefaultCellStyle = dataGridViewCellStyle3;
            this.particular.HeaderText = "PARTICULARS";
            this.particular.Name = "particular";
            this.particular.Width = 150;
            // 
            // amount
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.NullValue = null;
            this.amount.DefaultCellStyle = dataGridViewCellStyle4;
            this.amount.HeaderText = "AMOUNT";
            this.amount.Name = "amount";
            // 
            // gridID
            // 
            this.gridID.HeaderText = "ID";
            this.gridID.Name = "gridID";
            this.gridID.Visible = false;
            // 
            // pnlDeletionConfirmation
            // 
            this.pnlDeletionConfirmation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlDeletionConfirmation.Controls.Add(this.btnDeletionClose);
            this.pnlDeletionConfirmation.Controls.Add(this.panel8);
            this.pnlDeletionConfirmation.Location = new System.Drawing.Point(245, 286);
            this.pnlDeletionConfirmation.Name = "pnlDeletionConfirmation";
            this.pnlDeletionConfirmation.Size = new System.Drawing.Size(575, 87);
            this.pnlDeletionConfirmation.TabIndex = 508;
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
            // 
            // txtVoucherCode
            // 
            this.txtVoucherCode.BackColor = System.Drawing.Color.White;
            this.txtVoucherCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVoucherCode.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherCode.Location = new System.Drawing.Point(131, 8);
            this.txtVoucherCode.Name = "txtVoucherCode";
            this.txtVoucherCode.ReadOnly = true;
            this.txtVoucherCode.Size = new System.Drawing.Size(84, 22);
            this.txtVoucherCode.TabIndex = 102;
            this.txtVoucherCode.TabStop = false;
            this.txtVoucherCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVoucherCode_KeyDown);
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCreatedBy.LinkColor = System.Drawing.Color.White;
            this.lblCreatedBy.Location = new System.Drawing.Point(542, 63);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(546, 15);
            this.lblCreatedBy.TabIndex = 509;
            this.lblCreatedBy.TabStop = true;
            this.lblCreatedBy.Text = "CreatedBy";
            this.lblCreatedBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCreatedBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCreatedBy_LinkClicked);
            // 
            // chkSendSMS
            // 
            this.chkSendSMS.AutoSize = true;
            this.chkSendSMS.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSendSMS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSendSMS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkSendSMS.ForeColor = System.Drawing.Color.White;
            this.chkSendSMS.Location = new System.Drawing.Point(840, 40);
            this.chkSendSMS.Name = "chkSendSMS";
            this.chkSendSMS.Size = new System.Drawing.Size(93, 20);
            this.chkSendSMS.TabIndex = 110;
            this.chkSendSMS.Text = "Send S&MS";
            this.chkSendSMS.UseVisualStyleBackColor = true;
            // 
            // panDisp
            // 
            this.panDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panDisp.AutoSize = true;
            this.panDisp.BackColor = System.Drawing.Color.White;
            this.panDisp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panDisp.Controls.Add(this.lblName);
            this.panDisp.Location = new System.Drawing.Point(23, 17);
            this.panDisp.Name = "panDisp";
            this.panDisp.Size = new System.Drawing.Size(1065, 44);
            this.panDisp.TabIndex = 507;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.Black;
            this.lblName.Location = new System.Drawing.Point(426, 11);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(208, 19);
            this.lblName.TabIndex = 224;
            this.lblName.Text = "CHEQUE BOOK DETAILS";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.lblCurrentAmount);
            this.panel3.Controls.Add(this.lblMsg);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.lblTotalAmt);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.panAccountHead);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.panel3.Location = new System.Drawing.Point(22, 82);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1066, 559);
            this.panel3.TabIndex = 100;
            // 
            // lblCurrentAmount
            // 
            this.lblCurrentAmount.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCurrentAmount.Location = new System.Drawing.Point(20, 103);
            this.lblCurrentAmount.Name = "lblCurrentAmount";
            this.lblCurrentAmount.Size = new System.Drawing.Size(959, 23);
            this.lblCurrentAmount.TabIndex = 129;
            this.lblCurrentAmount.Text = "  ";
            this.lblCurrentAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.btnSearch);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.btnPreview);
            this.panel4.Controls.Add(this.btnPrint);
            this.panel4.Controls.Add(this.btnDelete);
            this.panel4.Controls.Add(this.btnEdit);
            this.panel4.Controls.Add(this.btnAdd);
            this.panel4.Location = new System.Drawing.Point(20, 493);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1023, 52);
            this.panel4.TabIndex = 113;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(444, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(104, 37);
            this.btnSearch.TabIndex = 117;
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
            this.btnClose.Location = new System.Drawing.Point(880, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(104, 37);
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
            this.btnPreview.Location = new System.Drawing.Point(720, 5);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(120, 37);
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
            this.btnPrint.Location = new System.Drawing.Point(580, 5);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(104, 37);
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
            this.btnDelete.Location = new System.Drawing.Point(307, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(104, 37);
            this.btnDelete.TabIndex = 116;
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
            this.btnEdit.Location = new System.Drawing.Point(171, 5);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(104, 37);
            this.btnEdit.TabIndex = 115;
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
            this.btnAdd.Location = new System.Drawing.Point(40, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(104, 37);
            this.btnAdd.TabIndex = 114;
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
            this.lblTotalAmt.Location = new System.Drawing.Point(885, 471);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.ReadOnly = true;
            this.lblTotalAmt.Size = new System.Drawing.Size(138, 16);
            this.lblTotalAmt.TabIndex = 108;
            this.lblTotalAmt.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(780, 472);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 16);
            this.label12.TabIndex = 117;
            this.label12.Text = "Total Amount :";
            // 
            // panAccountHead
            // 
            this.panAccountHead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panAccountHead.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panAccountHead.Controls.Add(this.txtDepositeDate);
            this.panAccountHead.Controls.Add(this.txtDate);
            this.panAccountHead.Controls.Add(this.label2);
            this.panAccountHead.Controls.Add(this.chkSendSMS);
            this.panAccountHead.Controls.Add(this.label1);
            this.panAccountHead.Controls.Add(this.grpPaymentype);
            this.panAccountHead.Controls.Add(this.btnName);
            this.panAccountHead.Controls.Add(this.txtVoucherCode);
            this.panAccountHead.Controls.Add(this.txtBankAccount);
            this.panAccountHead.Controls.Add(this.lblBankAccount);
            this.panAccountHead.Controls.Add(this.txtVoucherNo);
            this.panAccountHead.Controls.Add(this.Label10);
            this.panAccountHead.Controls.Add(this.grpStatus);
            this.panAccountHead.Location = new System.Drawing.Point(20, 22);
            this.panAccountHead.Name = "panAccountHead";
            this.panAccountHead.Size = new System.Drawing.Size(1023, 80);
            this.panAccountHead.TabIndex = 101;
            // 
            // txtDepositeDate
            // 
            this.txtDepositeDate.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtDepositeDate.Location = new System.Drawing.Point(603, 35);
            this.txtDepositeDate.Mask = "00/00/0000";
            this.txtDepositeDate.Name = "txtDepositeDate";
            this.txtDepositeDate.Size = new System.Drawing.Size(98, 22);
            this.txtDepositeDate.TabIndex = 107;
            this.txtDepositeDate.Enter += new System.EventHandler(this.txtDate_Enter);
            this.txtDepositeDate.Leave += new System.EventHandler(this.txtDepositeDate_Leave);
            // 
            // txtDate
            // 
            this.txtDate.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtDate.Location = new System.Drawing.Point(274, 8);
            this.txtDate.Mask = "00/00/0000";
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(95, 22);
            this.txtDate.TabIndex = 104;
            this.txtDate.Enter += new System.EventHandler(this.txtDate_Enter);
            this.txtDate.Leave += new System.EventHandler(this.txtDate_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(31, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 16);
            this.label2.TabIndex = 126;
            this.label2.Text = "Chque Type :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(482, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 16);
            this.label1.TabIndex = 125;
            this.label1.Text = "Deposite Date :";
            // 
            // grpPaymentype
            // 
            this.grpPaymentype.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.grpPaymentype.Controls.Add(this.rdoPDCCheque);
            this.grpPaymentype.Controls.Add(this.rdoSecurityChq);
            this.grpPaymentype.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.grpPaymentype.Location = new System.Drawing.Point(131, 29);
            this.grpPaymentype.Name = "grpPaymentype";
            this.grpPaymentype.Size = new System.Drawing.Size(232, 37);
            this.grpPaymentype.TabIndex = 106;
            this.grpPaymentype.TabStop = false;
            // 
            // rdoPDCCheque
            // 
            this.rdoPDCCheque.AutoSize = true;
            this.rdoPDCCheque.Checked = true;
            this.rdoPDCCheque.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoPDCCheque.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rdoPDCCheque.ForeColor = System.Drawing.Color.White;
            this.rdoPDCCheque.Location = new System.Drawing.Point(9, 12);
            this.rdoPDCCheque.Name = "rdoPDCCheque";
            this.rdoPDCCheque.Size = new System.Drawing.Size(85, 20);
            this.rdoPDCCheque.TabIndex = 107;
            this.rdoPDCCheque.TabStop = true;
            this.rdoPDCCheque.Text = "PDC C&HQ";
            this.rdoPDCCheque.UseVisualStyleBackColor = true;
            // 
            // rdoSecurityChq
            // 
            this.rdoSecurityChq.AutoSize = true;
            this.rdoSecurityChq.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSecurityChq.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rdoSecurityChq.ForeColor = System.Drawing.Color.White;
            this.rdoSecurityChq.Location = new System.Drawing.Point(106, 12);
            this.rdoSecurityChq.Name = "rdoSecurityChq";
            this.rdoSecurityChq.Size = new System.Drawing.Size(110, 20);
            this.rdoSecurityChq.TabIndex = 108;
            this.rdoSecurityChq.TabStop = true;
            this.rdoSecurityChq.Text = "Security CH&Q";
            this.rdoSecurityChq.UseVisualStyleBackColor = true;
            this.rdoSecurityChq.CheckedChanged += new System.EventHandler(this.rdoSecurityChq_CheckedChanged);
            // 
            // txtBankAccount
            // 
            this.txtBankAccount.BackColor = System.Drawing.Color.White;
            this.txtBankAccount.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBankAccount.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBankAccount.Location = new System.Drawing.Point(603, 8);
            this.txtBankAccount.Name = "txtBankAccount";
            this.txtBankAccount.ReadOnly = true;
            this.txtBankAccount.Size = new System.Drawing.Size(297, 23);
            this.txtBankAccount.TabIndex = 105;
            this.txtBankAccount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankAccount_KeyDown);
            // 
            // lblBankAccount
            // 
            this.lblBankAccount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblBankAccount.ForeColor = System.Drawing.Color.White;
            this.lblBankAccount.Location = new System.Drawing.Point(471, 12);
            this.lblBankAccount.Name = "lblBankAccount";
            this.lblBankAccount.Size = new System.Drawing.Size(125, 16);
            this.lblBankAccount.TabIndex = 121;
            this.lblBankAccount.Text = "Debit Back A/c :";
            this.lblBankAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.BackColor = System.Drawing.Color.White;
            this.txtVoucherNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVoucherNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherNo.Location = new System.Drawing.Point(215, 8);
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(59, 22);
            this.txtVoucherNo.TabIndex = 103;
            this.txtVoucherNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVoucherNo_KeyPress);
            this.txtVoucherNo.Leave += new System.EventHandler(this.txtVoucherNo_Leave);
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.Label10.ForeColor = System.Drawing.Color.White;
            this.Label10.Location = new System.Drawing.Point(32, 10);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(97, 16);
            this.Label10.TabIndex = 116;
            this.Label10.Text = "Voucher No :";
            // 
            // grpStatus
            // 
            this.grpStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.grpStatus.Controls.Add(this.rdoDebit);
            this.grpStatus.Controls.Add(this.rdoCredit);
            this.grpStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.grpStatus.Location = new System.Drawing.Point(705, 28);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(131, 34);
            this.grpStatus.TabIndex = 127;
            this.grpStatus.TabStop = false;
            // 
            // rdoDebit
            // 
            this.rdoDebit.AutoSize = true;
            this.rdoDebit.Checked = true;
            this.rdoDebit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoDebit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoDebit.ForeColor = System.Drawing.Color.White;
            this.rdoDebit.Location = new System.Drawing.Point(8, 10);
            this.rdoDebit.Name = "rdoDebit";
            this.rdoDebit.Size = new System.Drawing.Size(54, 19);
            this.rdoDebit.TabIndex = 108;
            this.rdoDebit.TabStop = true;
            this.rdoDebit.Text = "&Debit";
            this.rdoDebit.UseVisualStyleBackColor = true;
            this.rdoDebit.CheckedChanged += new System.EventHandler(this.rdoDebit_CheckedChanged);
            // 
            // rdoCredit
            // 
            this.rdoCredit.AutoSize = true;
            this.rdoCredit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoCredit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoCredit.ForeColor = System.Drawing.Color.White;
            this.rdoCredit.Location = new System.Drawing.Point(66, 10);
            this.rdoCredit.Name = "rdoCredit";
            this.rdoCredit.Size = new System.Drawing.Size(59, 19);
            this.rdoCredit.TabIndex = 109;
            this.rdoCredit.TabStop = true;
            this.rdoCredit.Text = "&Credit";
            this.rdoCredit.UseVisualStyleBackColor = true;
            // 
            // ChequeDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1107, 658);
            this.Controls.Add(this.pnlDeletionConfirmation);
            this.Controls.Add(this.lblCreatedBy);
            this.Controls.Add(this.panDisp);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ChequeDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChequeDetails";
            this.Load += new System.EventHandler(this.ChequeDetails_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChequeDetails_KeyDown);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.pnlDeletionConfirmation.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panDisp.ResumeLayout(false);
            this.panDisp.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panAccountHead.ResumeLayout(false);
            this.panAccountHead.PerformLayout();
            this.grpPaymentype.ResumeLayout(false);
            this.grpPaymentype.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnName;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlDeletionConfirmation;
        private System.Windows.Forms.Button btnDeletionClose;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnFinalDelete;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.TextBox txtVoucherCode;
        private System.Windows.Forms.LinkLabel lblCreatedBy;
        private System.Windows.Forms.CheckBox chkSendSMS;
        private System.Windows.Forms.Panel panDisp;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblCurrentAmount;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.TextBox lblTotalAmt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panAccountHead;
        private System.Windows.Forms.TextBox txtBankAccount;
        private System.Windows.Forms.Label lblBankAccount;
        private System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpPaymentype;
        private System.Windows.Forms.RadioButton rdoPDCCheque;
        private System.Windows.Forms.RadioButton rdoSecurityChq;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RadioButton rdoDebit;
        private System.Windows.Forms.RadioButton rdoCredit;
        protected internal System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.MaskedTextBox txtDate;
        private System.Windows.Forms.MaskedTextBox txtDepositeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SrNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn bankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chequeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn particular;
        private System.Windows.Forms.DataGridViewTextBoxColumn amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridID;
        private System.Windows.Forms.Label lblName;
    }
}