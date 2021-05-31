namespace SSS
{
    partial class ViewPaymentRequest
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
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.txtPartyName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.chkValue = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.netAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.netStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requestStatus = new System.Windows.Forms.DataGridViewLinkColumn();
            this.branchCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paidDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deleteButton = new System.Windows.Forms.DataGridViewLinkColumn();
            this.bankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.branchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ifscCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filepath = new System.Windows.Forms.DataGridViewLinkColumn();
            this.createdBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.purchaseAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cashAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.beniID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.finalPartyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGo = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToPaidDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromPaidDate = new System.Windows.Forms.MaskedTextBox();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkPaidDate = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnPriority = new System.Windows.Forms.Button();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBranch = new System.Windows.Forms.Button();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStatus = new System.Windows.Forms.Button();
            this.btnParty = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.btnStatusChanged = new System.Windows.Forms.Button();
            this.txtStatusChanged = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.lblNetStatus = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblNetAmt = new System.Windows.Forms.Label();
            this.btnSendRequest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(433, 7);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(109, 20);
            this.chkDate.TabIndex = 103;
            this.chkDate.Text = "Added Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(634, 9);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 111;
            this.Label21.Text = "To";
            // 
            // txtPartyName
            // 
            this.txtPartyName.BackColor = System.Drawing.Color.White;
            this.txtPartyName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPartyName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPartyName.Location = new System.Drawing.Point(93, 6);
            this.txtPartyName.Name = "txtPartyName";
            this.txtPartyName.ReadOnly = true;
            this.txtPartyName.Size = new System.Drawing.Size(314, 23);
            this.txtPartyName.TabIndex = 101;
            this.txtPartyName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPartyName_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(2, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 16);
            this.label8.TabIndex = 72;
            this.label8.Text = "Party Name :";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkValue,
            this.Date,
            this.partyName,
            this.netAmt,
            this.netStatus,
            this.requestStatus,
            this.branchCode,
            this.priority,
            this.remark,
            this.paidDate,
            this.deleteButton,
            this.bankName,
            this.branchName,
            this.accountNo,
            this.accountName,
            this.ifscCode,
            this.filepath,
            this.createdBy,
            this.purchaseAmt,
            this.cashAmt,
            this.beniID,
            this.finalPartyName,
            this.id});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(12, 12);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(1003, 448);
            this.dgrdDetails.TabIndex = 118;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellEndEdit);
            this.dgrdDetails.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgrdDetails_CellValidating);
            this.dgrdDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdDetails_EditingControlShowing);
            this.dgrdDetails.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgrdDetails_Scroll);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // chkValue
            // 
            this.chkValue.HeaderText = "";
            this.chkValue.Name = "chkValue";
            this.chkValue.Width = 30;
            // 
            // Date
            // 
            dataGridViewCellStyle2.Format = "dd/MM/yyyy";
            this.Date.DefaultCellStyle = dataGridViewCellStyle2;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 90;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.Name = "partyName";
            this.partyName.ReadOnly = true;
            this.partyName.Width = 180;
            // 
            // netAmt
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            this.netAmt.DefaultCellStyle = dataGridViewCellStyle3;
            this.netAmt.HeaderText = "Net Amt";
            this.netAmt.Name = "netAmt";
            this.netAmt.Width = 130;
            // 
            // netStatus
            // 
            this.netStatus.HeaderText = "";
            this.netStatus.Name = "netStatus";
            this.netStatus.ReadOnly = true;
            this.netStatus.Width = 30;
            // 
            // requestStatus
            // 
            this.requestStatus.HeaderText = "Req Status";
            this.requestStatus.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.requestStatus.LinkColor = System.Drawing.Color.Black;
            this.requestStatus.Name = "requestStatus";
            this.requestStatus.ReadOnly = true;
            this.requestStatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.requestStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.requestStatus.Width = 110;
            // 
            // branchCode
            // 
            this.branchCode.HeaderText = "B. Code";
            this.branchCode.Name = "branchCode";
            this.branchCode.ReadOnly = true;
            this.branchCode.Width = 70;
            // 
            // priority
            // 
            this.priority.HeaderText = "Priority";
            this.priority.Name = "priority";
            this.priority.Width = 80;
            // 
            // remark
            // 
            this.remark.HeaderText = "Remark";
            this.remark.Name = "remark";
            this.remark.Width = 120;
            // 
            // paidDate
            // 
            dataGridViewCellStyle4.Format = "dd/MM/yyyy";
            this.paidDate.DefaultCellStyle = dataGridViewCellStyle4;
            this.paidDate.HeaderText = "Paid Date";
            this.paidDate.Name = "paidDate";
            this.paidDate.Width = 90;
            // 
            // deleteButton
            // 
            this.deleteButton.HeaderText = "Action";
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.ReadOnly = true;
            this.deleteButton.Width = 90;
            // 
            // bankName
            // 
            this.bankName.HeaderText = "Bank Name";
            this.bankName.Name = "bankName";
            this.bankName.ReadOnly = true;
            // 
            // branchName
            // 
            this.branchName.HeaderText = "Branch Name";
            this.branchName.Name = "branchName";
            this.branchName.ReadOnly = true;
            // 
            // accountNo
            // 
            this.accountNo.HeaderText = "Account No";
            this.accountNo.Name = "accountNo";
            this.accountNo.ReadOnly = true;
            this.accountNo.Width = 150;
            // 
            // accountName
            // 
            this.accountName.HeaderText = "Account Name";
            this.accountName.Name = "accountName";
            this.accountName.ReadOnly = true;
            this.accountName.Width = 110;
            // 
            // ifscCode
            // 
            this.ifscCode.HeaderText = "IFSC Code";
            this.ifscCode.Name = "ifscCode";
            this.ifscCode.ReadOnly = true;
            this.ifscCode.Width = 110;
            // 
            // filepath
            // 
            this.filepath.HeaderText = "PDF File";
            this.filepath.LinkColor = System.Drawing.Color.Black;
            this.filepath.Name = "filepath";
            this.filepath.ReadOnly = true;
            this.filepath.Width = 150;
            // 
            // createdBy
            // 
            this.createdBy.HeaderText = "Created By";
            this.createdBy.Name = "createdBy";
            this.createdBy.ReadOnly = true;
            this.createdBy.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.createdBy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // purchaseAmt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.purchaseAmt.DefaultCellStyle = dataGridViewCellStyle5;
            this.purchaseAmt.HeaderText = "Purchase Amt";
            this.purchaseAmt.Name = "purchaseAmt";
            this.purchaseAmt.ReadOnly = true;
            this.purchaseAmt.Width = 120;
            // 
            // cashAmt
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.cashAmt.DefaultCellStyle = dataGridViewCellStyle6;
            this.cashAmt.HeaderText = "Cash Amt";
            this.cashAmt.Name = "cashAmt";
            this.cashAmt.ReadOnly = true;
            this.cashAmt.Width = 120;
            // 
            // beniID
            // 
            this.beniID.HeaderText = "Beni ID";
            this.beniID.Name = "beniID";
            this.beniID.ReadOnly = true;
            // 
            // finalPartyName
            // 
            this.finalPartyName.HeaderText = "Final P.Name";
            this.finalPartyName.Name = "finalPartyName";
            this.finalPartyName.ReadOnly = true;
            this.finalPartyName.Visible = false;
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(968, 29);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(54, 32);
            this.btnGo.TabIndex = 116;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.chkAll);
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.panel3.Location = new System.Drawing.Point(11, 128);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1028, 472);
            this.panel3.TabIndex = 117;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Location = new System.Drawing.Point(23, 22);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 143;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(961, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 31);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToPaidDate);
            this.panel2.Controls.Add(this.txtFromPaidDate);
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.txtRemark);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.chkPaidDate);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.btnPriority);
            this.panel2.Controls.Add(this.txtPriority);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.btnBranch);
            this.panel2.Controls.Add(this.txtBranchCode);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnStatus);
            this.panel2.Controls.Add(this.btnParty);
            this.panel2.Controls.Add(this.txtStatus);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.Label21);
            this.panel2.Controls.Add(this.txtPartyName);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Location = new System.Drawing.Point(13, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1028, 65);
            this.panel2.TabIndex = 100;
            // 
            // txtToPaidDate
            // 
            this.txtToPaidDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToPaidDate.Location = new System.Drawing.Point(658, 34);
            this.txtToPaidDate.Mask = "00/00/0000";
            this.txtToPaidDate.Name = "txtToPaidDate";
            this.txtToPaidDate.Size = new System.Drawing.Size(90, 23);
            this.txtToPaidDate.TabIndex = 114;
            this.txtToPaidDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToPaidDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromPaidDate
            // 
            this.txtFromPaidDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromPaidDate.Location = new System.Drawing.Point(541, 34);
            this.txtFromPaidDate.Mask = "00/00/0000";
            this.txtFromPaidDate.Name = "txtFromPaidDate";
            this.txtFromPaidDate.Size = new System.Drawing.Size(91, 23);
            this.txtFromPaidDate.TabIndex = 113;
            this.txtFromPaidDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromPaidDate.Leave += new System.EventHandler(this.txtFromPaidDate_Leave);
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(658, 5);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(90, 23);
            this.txtToDate.TabIndex = 105;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(541, 6);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(90, 23);
            this.txtFromDate.TabIndex = 104;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtRemark
            // 
            this.txtRemark.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRemark.Font = new System.Drawing.Font("Arial", 10F);
            this.txtRemark.Location = new System.Drawing.Point(841, 33);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(126, 23);
            this.txtRemark.TabIndex = 115;
            this.txtRemark.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRemark_KeyPress_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(771, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 16);
            this.label7.TabIndex = 126;
            this.label7.Text = "Remark :";
            // 
            // chkPaidDate
            // 
            this.chkPaidDate.AutoSize = true;
            this.chkPaidDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPaidDate.Location = new System.Drawing.Point(444, 34);
            this.chkPaidDate.Name = "chkPaidDate";
            this.chkPaidDate.Size = new System.Drawing.Size(97, 20);
            this.chkPaidDate.TabIndex = 112;
            this.chkPaidDate.Text = "Paid Date :";
            this.chkPaidDate.UseVisualStyleBackColor = true;
            this.chkPaidDate.CheckedChanged += new System.EventHandler(this.chkPaidDate_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(635, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 15);
            this.label6.TabIndex = 125;
            this.label6.Text = "To";
            // 
            // btnPriority
            // 
            this.btnPriority.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnPriority.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPriority.Location = new System.Drawing.Point(189, 34);
            this.btnPriority.Name = "btnPriority";
            this.btnPriority.Size = new System.Drawing.Size(24, 25);
            this.btnPriority.TabIndex = 109;
            this.btnPriority.TabStop = false;
            this.btnPriority.UseVisualStyleBackColor = true;
            this.btnPriority.Click += new System.EventHandler(this.btnPriority_Click);
            // 
            // txtPriority
            // 
            this.txtPriority.BackColor = System.Drawing.Color.White;
            this.txtPriority.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPriority.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPriority.Location = new System.Drawing.Point(93, 35);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.ReadOnly = true;
            this.txtPriority.Size = new System.Drawing.Size(96, 23);
            this.txtPriority.TabIndex = 108;
            this.txtPriority.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPriority_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 16);
            this.label5.TabIndex = 121;
            this.label5.Text = "Req Priority :";
            // 
            // btnBranch
            // 
            this.btnBranch.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnBranch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBranch.Location = new System.Drawing.Point(407, 32);
            this.btnBranch.Name = "btnBranch";
            this.btnBranch.Size = new System.Drawing.Size(24, 25);
            this.btnBranch.TabIndex = 111;
            this.btnBranch.TabStop = false;
            this.btnBranch.UseVisualStyleBackColor = true;
            this.btnBranch.Click += new System.EventHandler(this.btnBranch_Click);
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.BackColor = System.Drawing.Color.White;
            this.txtBranchCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBranchCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBranchCode.Location = new System.Drawing.Point(314, 33);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.ReadOnly = true;
            this.txtBranchCode.Size = new System.Drawing.Size(93, 23);
            this.txtBranchCode.TabIndex = 110;
            this.txtBranchCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBranchCode_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(213, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 16);
            this.label3.TabIndex = 118;
            this.label3.Text = "Branch Code :";
            // 
            // btnStatus
            // 
            this.btnStatus.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStatus.Location = new System.Drawing.Point(998, 5);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(23, 24);
            this.btnStatus.TabIndex = 107;
            this.btnStatus.TabStop = false;
            this.btnStatus.UseVisualStyleBackColor = true;
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);
            // 
            // btnParty
            // 
            this.btnParty.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnParty.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnParty.Location = new System.Drawing.Point(407, 5);
            this.btnParty.Name = "btnParty";
            this.btnParty.Size = new System.Drawing.Size(24, 25);
            this.btnParty.TabIndex = 102;
            this.btnParty.TabStop = false;
            this.btnParty.UseVisualStyleBackColor = true;
            this.btnParty.Click += new System.EventHandler(this.btnParty_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.Color.White;
            this.txtStatus.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtStatus.Font = new System.Drawing.Font("Arial", 10F);
            this.txtStatus.Location = new System.Drawing.Point(841, 6);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(156, 23);
            this.txtStatus.TabIndex = 106;
            this.txtStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStatus_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(750, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 115;
            this.label2.Text = "Req. Status :";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1029, 40);
            this.panel1.TabIndex = 119;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(391, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = "PAYMENT REQUEST DETAILS";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.btnUpload);
            this.panel5.Controls.Add(this.btnChangeStatus);
            this.panel5.Controls.Add(this.btnStatusChanged);
            this.panel5.Controls.Add(this.txtStatusChanged);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.btnDownload);
            this.panel5.Controls.Add(this.btnPrint);
            this.panel5.Controls.Add(this.btnPreview);
            this.panel5.Controls.Add(this.lblNetStatus);
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.lblNetAmt);
            this.panel5.Controls.Add(this.btnCancel);
            this.panel5.Controls.Add(this.btnSendRequest);
            this.panel5.Location = new System.Drawing.Point(9, 609);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1030, 39);
            this.panel5.TabIndex = 119;
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnUpload.Enabled = false;
            this.btnUpload.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnUpload.ForeColor = System.Drawing.Color.White;
            this.btnUpload.Location = new System.Drawing.Point(624, 3);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(90, 31);
            this.btnUpload.TabIndex = 124;
            this.btnUpload.Text = "&Upload";
            this.btnUpload.UseVisualStyleBackColor = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnChangeStatus
            // 
            this.btnChangeStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnChangeStatus.Enabled = false;
            this.btnChangeStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnChangeStatus.ForeColor = System.Drawing.Color.White;
            this.btnChangeStatus.Location = new System.Drawing.Point(426, 2);
            this.btnChangeStatus.Name = "btnChangeStatus";
            this.btnChangeStatus.Size = new System.Drawing.Size(111, 31);
            this.btnChangeStatus.TabIndex = 122;
            this.btnChangeStatus.Text = "&Change Status";
            this.btnChangeStatus.UseVisualStyleBackColor = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.btnChangeStatus_Click);
            // 
            // btnStatusChanged
            // 
            this.btnStatusChanged.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnStatusChanged.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStatusChanged.Location = new System.Drawing.Point(403, 6);
            this.btnStatusChanged.Name = "btnStatusChanged";
            this.btnStatusChanged.Size = new System.Drawing.Size(23, 24);
            this.btnStatusChanged.TabIndex = 121;
            this.btnStatusChanged.TabStop = false;
            this.btnStatusChanged.UseVisualStyleBackColor = true;
            this.btnStatusChanged.Click += new System.EventHandler(this.btnStatusChanged_Click);
            // 
            // txtStatusChanged
            // 
            this.txtStatusChanged.BackColor = System.Drawing.Color.White;
            this.txtStatusChanged.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtStatusChanged.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtStatusChanged.Location = new System.Drawing.Point(277, 7);
            this.txtStatusChanged.Name = "txtStatusChanged";
            this.txtStatusChanged.ReadOnly = true;
            this.txtStatusChanged.Size = new System.Drawing.Size(126, 22);
            this.txtStatusChanged.TabIndex = 120;
            this.txtStatusChanged.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStatusChanged_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(223, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 16);
            this.label4.TabIndex = 128;
            this.label4.Text = "Status :";
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDownload.Enabled = false;
            this.btnDownload.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDownload.ForeColor = System.Drawing.Color.White;
            this.btnDownload.Location = new System.Drawing.Point(536, 2);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(90, 31);
            this.btnDownload.TabIndex = 123;
            this.btnDownload.Text = "&Download";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(820, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(63, 31);
            this.btnPrint.TabIndex = 126;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(883, 2);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(78, 31);
            this.btnPreview.TabIndex = 127;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // lblNetStatus
            // 
            this.lblNetStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNetStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblNetStatus.Location = new System.Drawing.Point(199, 11);
            this.lblNetStatus.Name = "lblNetStatus";
            this.lblNetStatus.Size = new System.Drawing.Size(23, 16);
            this.lblNetStatus.TabIndex = 121;
            this.lblNetStatus.Text = "Dr";
            this.lblNetStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(1, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 16);
            this.label9.TabIndex = 120;
            this.label9.Text = "Payable Amt :";
            // 
            // lblNetAmt
            // 
            this.lblNetAmt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNetAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblNetAmt.Location = new System.Drawing.Point(97, 11);
            this.lblNetAmt.Name = "lblNetAmt";
            this.lblNetAmt.Size = new System.Drawing.Size(100, 16);
            this.lblNetAmt.TabIndex = 119;
            this.lblNetAmt.Text = "0.00";
            this.lblNetAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSendRequest
            // 
            this.btnSendRequest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSendRequest.Enabled = false;
            this.btnSendRequest.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSendRequest.ForeColor = System.Drawing.Color.White;
            this.btnSendRequest.Location = new System.Drawing.Point(713, 2);
            this.btnSendRequest.Name = "btnSendRequest";
            this.btnSendRequest.Size = new System.Drawing.Size(108, 31);
            this.btnSendRequest.TabIndex = 125;
            this.btnSendRequest.Text = "&Send Request";
            this.btnSendRequest.UseVisualStyleBackColor = false;
            this.btnSendRequest.Click += new System.EventHandler(this.btnSendRequest_Click);
            // 
            // ViewPaymentRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ViewPaymentRequest";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Payment Request";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewPaymentRequest_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.TextBox txtPartyName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnSendRequest;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblNetAmt;
        private System.Windows.Forms.Label lblNetStatus;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnParty;
        private System.Windows.Forms.Button btnStatus;
        private System.Windows.Forms.Button btnBranch;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnChangeStatus;
        private System.Windows.Forms.Button btnStatusChanged;
        private System.Windows.Forms.TextBox txtStatusChanged;
        private System.Windows.Forms.Label label4;
        protected internal System.Windows.Forms.CheckBox chkPaidDate;
        public System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnPriority;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn netAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn netStatus;
        private System.Windows.Forms.DataGridViewLinkColumn requestStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn priority;
        private System.Windows.Forms.DataGridViewTextBoxColumn remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn paidDate;
        private System.Windows.Forms.DataGridViewLinkColumn deleteButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn bankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ifscCode;
        private System.Windows.Forms.DataGridViewLinkColumn filepath;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn purchaseAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn cashAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn beniID;
        private System.Windows.Forms.DataGridViewTextBoxColumn finalPartyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        public System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label label1;
        protected internal System.Windows.Forms.MaskedTextBox txtToPaidDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromPaidDate;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}