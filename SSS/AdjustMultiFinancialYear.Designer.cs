namespace SSS
{
    partial class AdjustMultiFinancialYear
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnPartyName = new System.Windows.Forms.Button();
            this.btnSelectCompany = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtParty = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblBalanceAmt = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDebit = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.Label();
            this.btnAdjust = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.dgrdLedger = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.debit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.credit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelCompany = new System.Windows.Forms.Panel();
            this.dgrdCompany = new System.Windows.Forms.DataGridView();
            this.companyCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.companyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sTextDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eTextDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdLedger)).BeginInit();
            this.panelCompany.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdCompany)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(11, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1077, 41);
            this.panel1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(380, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "MULTI-FINANICAL YEAR ADJUSTMENT";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.btnPartyName);
            this.panel2.Controls.Add(this.btnSelectCompany);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.txtParty);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(10, 58);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1078, 38);
            this.panel2.TabIndex = 101;
            this.panel2.TabStop = true;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(710, 5);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(87, 23);
            this.txtToDate.TabIndex = 106;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(593, 5);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(87, 23);
            this.txtFromDate.TabIndex = 105;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.chkDate.Location = new System.Drawing.Point(531, 6);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(58, 19);
            this.chkDate.TabIndex = 104;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(686, 8);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 16);
            this.label18.TabIndex = 2177;
            this.label18.Text = "To";
            // 
            // btnPartyName
            // 
            this.btnPartyName.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnPartyName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPartyName.Location = new System.Drawing.Point(494, 4);
            this.btnPartyName.Name = "btnPartyName";
            this.btnPartyName.Size = new System.Drawing.Size(24, 25);
            this.btnPartyName.TabIndex = 103;
            this.btnPartyName.TabStop = false;
            this.btnPartyName.UseVisualStyleBackColor = true;
            this.btnPartyName.Click += new System.EventHandler(this.btnPartyName_Click);
            // 
            // btnSelectCompany
            // 
            this.btnSelectCompany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSelectCompany.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectCompany.Enabled = false;
            this.btnSelectCompany.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSelectCompany.ForeColor = System.Drawing.Color.White;
            this.btnSelectCompany.Location = new System.Drawing.Point(825, 1);
            this.btnSelectCompany.Name = "btnSelectCompany";
            this.btnSelectCompany.Size = new System.Drawing.Size(176, 34);
            this.btnSelectCompany.TabIndex = 107;
            this.btnSelectCompany.Text = "Select &Financial Year";
            this.btnSelectCompany.UseVisualStyleBackColor = false;
            this.btnSelectCompany.Click += new System.EventHandler(this.btnSelectCompany_Click);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(1002, 1);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(65, 33);
            this.btnGo.TabIndex = 108;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtParty
            // 
            this.txtParty.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtParty.BackColor = System.Drawing.SystemColors.Window;
            this.txtParty.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtParty.Font = new System.Drawing.Font("Arial", 10F);
            this.txtParty.Location = new System.Drawing.Point(99, 5);
            this.txtParty.Name = "txtParty";
            this.txtParty.ReadOnly = true;
            this.txtParty.Size = new System.Drawing.Size(395, 23);
            this.txtParty.TabIndex = 102;
            this.txtParty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtParty_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Party Name :";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.lblBalanceAmt);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.lblDebit);
            this.panel3.Controls.Add(this.lblCredit);
            this.panel3.Controls.Add(this.btnAdjust);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.chkAll);
            this.panel3.Controls.Add(this.dgrdLedger);
            this.panel3.Location = new System.Drawing.Point(10, 109);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1078, 529);
            this.panel3.TabIndex = 145;
            // 
            // lblBalanceAmt
            // 
            this.lblBalanceAmt.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalanceAmt.Location = new System.Drawing.Point(683, 498);
            this.lblBalanceAmt.Name = "lblBalanceAmt";
            this.lblBalanceAmt.Size = new System.Drawing.Size(140, 18);
            this.lblBalanceAmt.TabIndex = 132;
            this.lblBalanceAmt.Text = "0.00";
            this.lblBalanceAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(329, 498);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 18);
            this.label3.TabIndex = 129;
            this.label3.Text = "Total Amt :";
            // 
            // lblDebit
            // 
            this.lblDebit.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDebit.Location = new System.Drawing.Point(412, 498);
            this.lblDebit.Name = "lblDebit";
            this.lblDebit.Size = new System.Drawing.Size(115, 18);
            this.lblDebit.TabIndex = 130;
            this.lblDebit.Text = "0.00";
            this.lblDebit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCredit
            // 
            this.lblCredit.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredit.Location = new System.Drawing.Point(544, 498);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(115, 18);
            this.lblCredit.TabIndex = 131;
            this.lblCredit.Text = "0.00";
            this.lblCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAdjust
            // 
            this.btnAdjust.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdjust.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdjust.ForeColor = System.Drawing.Color.White;
            this.btnAdjust.Location = new System.Drawing.Point(840, 489);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(130, 33);
            this.btnAdjust.TabIndex = 127;
            this.btnAdjust.Text = "&Adjust Bill";
            this.btnAdjust.UseVisualStyleBackColor = false;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(970, 489);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 33);
            this.btnCancel.TabIndex = 128;
            this.btnCancel.Text = "C&lose";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Location = new System.Drawing.Point(1018, 22);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 126;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // dgrdLedger
            // 
            this.dgrdLedger.AllowUserToAddRows = false;
            this.dgrdLedger.AllowUserToResizeColumns = false;
            this.dgrdLedger.AllowUserToResizeRows = false;
            this.dgrdLedger.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdLedger.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdLedger.ColumnHeadersHeight = 30;
            this.dgrdLedger.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.sno,
            this.date,
            this.account,
            this.desc,
            this.debit,
            this.credit,
            this.balance,
            this.tick,
            this.cCode});
            this.dgrdLedger.EnableHeadersVisualStyles = false;
            this.dgrdLedger.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdLedger.Location = new System.Drawing.Point(12, 12);
            this.dgrdLedger.Name = "dgrdLedger";
            this.dgrdLedger.RowHeadersVisible = false;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9.75F);
            this.dgrdLedger.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgrdLedger.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F);
            this.dgrdLedger.RowTemplate.Height = 27;
            this.dgrdLedger.Size = new System.Drawing.Size(1052, 470);
            this.dgrdLedger.TabIndex = 12;
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // sno
            // 
            this.sno.HeaderText = "S.No.";
            this.sno.Name = "sno";
            this.sno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sno.Width = 50;
            // 
            // date
            // 
            dataGridViewCellStyle2.Format = "dd/MM/yyyy";
            dataGridViewCellStyle2.NullValue = null;
            this.date.DefaultCellStyle = dataGridViewCellStyle2;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 95;
            // 
            // account
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.account.DefaultCellStyle = dataGridViewCellStyle3;
            this.account.HeaderText = "Account";
            this.account.Name = "account";
            this.account.ReadOnly = true;
            this.account.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.account.Width = 190;
            // 
            // desc
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.desc.DefaultCellStyle = dataGridViewCellStyle4;
            this.desc.HeaderText = "Description";
            this.desc.Name = "desc";
            this.desc.ReadOnly = true;
            this.desc.Width = 270;
            // 
            // debit
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = null;
            this.debit.DefaultCellStyle = dataGridViewCellStyle5;
            this.debit.HeaderText = "Debit";
            this.debit.Name = "debit";
            this.debit.ReadOnly = true;
            this.debit.Width = 125;
            // 
            // credit
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = null;
            this.credit.DefaultCellStyle = dataGridViewCellStyle6;
            this.credit.HeaderText = "Credit";
            this.credit.Name = "credit";
            this.credit.ReadOnly = true;
            this.credit.Width = 125;
            // 
            // balance
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.Format = "N2";
            dataGridViewCellStyle7.NullValue = null;
            this.balance.DefaultCellStyle = dataGridViewCellStyle7;
            this.balance.HeaderText = "Balance";
            this.balance.Name = "balance";
            this.balance.ReadOnly = true;
            this.balance.Width = 140;
            // 
            // tick
            // 
            this.tick.HeaderText = "";
            this.tick.Name = "tick";
            this.tick.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.tick.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.tick.Width = 30;
            // 
            // cCode
            // 
            this.cCode.HeaderText = "CCode";
            this.cCode.Name = "cCode";
            this.cCode.Width = 80;
            // 
            // panelCompany
            // 
            this.panelCompany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelCompany.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCompany.Controls.Add(this.dgrdCompany);
            this.panelCompany.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.panelCompany.Location = new System.Drawing.Point(648, 95);
            this.panelCompany.Name = "panelCompany";
            this.panelCompany.Size = new System.Drawing.Size(451, 260);
            this.panelCompany.TabIndex = 146;
            this.panelCompany.Visible = false;
            // 
            // dgrdCompany
            // 
            this.dgrdCompany.AllowUserToAddRows = false;
            this.dgrdCompany.AllowUserToResizeRows = false;
            this.dgrdCompany.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdCompany.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgrdCompany.ColumnHeadersHeight = 30;
            this.dgrdCompany.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.companyCheck,
            this.code,
            this.companyName,
            this.startDate,
            this.endDate,
            this.sTextDate,
            this.eTextDate});
            this.dgrdCompany.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdCompany.EnableHeadersVisualStyles = false;
            this.dgrdCompany.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdCompany.Location = new System.Drawing.Point(21, 16);
            this.dgrdCompany.Name = "dgrdCompany";
            this.dgrdCompany.RowHeadersVisible = false;
            this.dgrdCompany.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdCompany.RowTemplate.Height = 27;
            this.dgrdCompany.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdCompany.Size = new System.Drawing.Size(412, 223);
            this.dgrdCompany.TabIndex = 106;
            this.dgrdCompany.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdCompany_CellBeginEdit);
            this.dgrdCompany.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdCompany_CellContentClick);
            // 
            // companyCheck
            // 
            this.companyCheck.HeaderText = "";
            this.companyCheck.Name = "companyCheck";
            this.companyCheck.Width = 35;
            // 
            // code
            // 
            this.code.HeaderText = "code";
            this.code.Name = "code";
            this.code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.code.Visible = false;
            // 
            // companyName
            // 
            this.companyName.HeaderText = "Financial Year";
            this.companyName.Name = "companyName";
            this.companyName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.companyName.Width = 350;
            // 
            // startDate
            // 
            dataGridViewCellStyle10.Format = "d";
            dataGridViewCellStyle10.NullValue = null;
            this.startDate.DefaultCellStyle = dataGridViewCellStyle10;
            this.startDate.HeaderText = "Date";
            this.startDate.Name = "startDate";
            this.startDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.startDate.Visible = false;
            // 
            // endDate
            // 
            dataGridViewCellStyle11.Format = "d";
            dataGridViewCellStyle11.NullValue = null;
            this.endDate.DefaultCellStyle = dataGridViewCellStyle11;
            this.endDate.HeaderText = "Date";
            this.endDate.Name = "endDate";
            this.endDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.endDate.Visible = false;
            // 
            // sTextDate
            // 
            this.sTextDate.HeaderText = "SDate";
            this.sTextDate.Name = "sTextDate";
            this.sTextDate.Visible = false;
            // 
            // eTextDate
            // 
            this.eTextDate.HeaderText = "EDate";
            this.eTextDate.Name = "eTextDate";
            this.eTextDate.Visible = false;
            // 
            // AdjustMultiFinancialYear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.Controls.Add(this.panelCompany);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AdjustMultiFinancialYear";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adjust Multi-Financial Year";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AdjustMultiFinancialYear_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdLedger)).EndInit();
            this.panelCompany.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdCompany)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnPartyName;
        private System.Windows.Forms.Button btnSelectCompany;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.TextBox txtParty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.DataGridView dgrdLedger;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDebit;
        private System.Windows.Forms.Label lblCredit;
        private System.Windows.Forms.Panel panelCompany;
        private System.Windows.Forms.DataGridView dgrdCompany;
        private System.Windows.Forms.DataGridViewCheckBoxColumn companyCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn code;
        private System.Windows.Forms.DataGridViewTextBoxColumn companyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn sTextDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn eTextDate;
        private System.Windows.Forms.Label lblBalanceAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn sno;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn account;
        private System.Windows.Forms.DataGridViewTextBoxColumn desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn debit;
        private System.Windows.Forms.DataGridViewTextBoxColumn credit;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance;
        private System.Windows.Forms.DataGridViewCheckBoxColumn tick;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCode;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Label label18;
    }
}