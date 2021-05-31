namespace SSS
{
    partial class PrintMultiLedger
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtNickName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtStateName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCityName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoCredit = new System.Windows.Forms.RadioButton();
            this.rdoDebit = new System.Windows.Forms.RadioButton();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.Label20 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.all = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.unTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.partyName = new System.Windows.Forms.DataGridViewLinkColumn();
            this.group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.debitAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creditAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpFooter = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblBalAmount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrintMultiLdeger = new System.Windows.Forms.Button();
            this.lblDebit = new System.Windows.Forms.Label();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.chkTick = new System.Windows.Forms.CheckBox();
            this.chkUntick = new System.Windows.Forms.CheckBox();
            this.chkCheckAll = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.grpFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1026, 40);
            this.panel1.TabIndex = 900;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(401, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "MULTIPLE PARTY LEDGER";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.txtNickName);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtStateName);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.txtBranchCode);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.txtCityName);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtGroupName);
            this.panel2.Controls.Add(this.grpStatus);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.txtAmount);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.Label20);
            this.panel2.Location = new System.Drawing.Point(12, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1026, 64);
            this.panel2.TabIndex = 100;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(461, 5);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(81, 23);
            this.txtToDate.TabIndex = 104;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(344, 5);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(84, 23);
            this.txtFromDate.TabIndex = 103;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtNickName
            // 
            this.txtNickName.BackColor = System.Drawing.Color.White;
            this.txtNickName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtNickName.Location = new System.Drawing.Point(630, 34);
            this.txtNickName.Name = "txtNickName";
            this.txtNickName.ReadOnly = true;
            this.txtNickName.Size = new System.Drawing.Size(314, 23);
            this.txtNickName.TabIndex = 113;
            this.txtNickName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNickName_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(544, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 16);
            this.label7.TabIndex = 150;
            this.label7.Text = "Nick Name :";
            // 
            // txtStateName
            // 
            this.txtStateName.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtStateName.BackColor = System.Drawing.SystemColors.Window;
            this.txtStateName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtStateName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtStateName.Location = new System.Drawing.Point(344, 34);
            this.txtStateName.Name = "txtStateName";
            this.txtStateName.ReadOnly = true;
            this.txtStateName.Size = new System.Drawing.Size(199, 23);
            this.txtStateName.TabIndex = 112;
            this.txtStateName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStateName_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(295, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 16);
            this.label5.TabIndex = 148;
            this.label5.Text = "State :";
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.BackColor = System.Drawing.Color.White;
            this.txtBranchCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBranchCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBranchCode.Location = new System.Drawing.Point(615, 5);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.ReadOnly = true;
            this.txtBranchCode.Size = new System.Drawing.Size(87, 23);
            this.txtBranchCode.TabIndex = 105;
            this.txtBranchCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBranchCode_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(553, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 16);
            this.label13.TabIndex = 146;
            this.label13.Text = "Branch :";
            // 
            // txtCityName
            // 
            this.txtCityName.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtCityName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCityName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCityName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCityName.Location = new System.Drawing.Point(55, 33);
            this.txtCityName.Name = "txtCityName";
            this.txtCityName.ReadOnly = true;
            this.txtCityName.Size = new System.Drawing.Size(221, 23);
            this.txtCityName.TabIndex = 111;
            this.txtCityName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCityName_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(14, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 120;
            this.label3.Text = "City :";
            // 
            // txtGroupName
            // 
            this.txtGroupName.AutoCompleteCustomSource.AddRange(new string[] {
            "Ram",
            "Red",
            "Ratan",
            "Rohan"});
            this.txtGroupName.BackColor = System.Drawing.SystemColors.Window;
            this.txtGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtGroupName.Location = new System.Drawing.Point(55, 5);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.ReadOnly = true;
            this.txtGroupName.Size = new System.Drawing.Size(221, 23);
            this.txtGroupName.TabIndex = 101;
            this.txtGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGroupName_KeyDown);
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoAll);
            this.grpStatus.Controls.Add(this.rdoCredit);
            this.grpStatus.Controls.Add(this.rdoDebit);
            this.grpStatus.Location = new System.Drawing.Point(884, -5);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(133, 34);
            this.grpStatus.TabIndex = 107;
            this.grpStatus.TabStop = false;
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
            this.rdoAll.TabIndex = 108;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // rdoCredit
            // 
            this.rdoCredit.AutoSize = true;
            this.rdoCredit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoCredit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoCredit.Location = new System.Drawing.Point(90, 11);
            this.rdoCredit.Name = "rdoCredit";
            this.rdoCredit.Size = new System.Drawing.Size(38, 18);
            this.rdoCredit.TabIndex = 110;
            this.rdoCredit.Text = "Cr";
            this.rdoCredit.UseVisualStyleBackColor = true;
            // 
            // rdoDebit
            // 
            this.rdoDebit.AutoSize = true;
            this.rdoDebit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoDebit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoDebit.Location = new System.Drawing.Point(49, 11);
            this.rdoDebit.Name = "rdoDebit";
            this.rdoDebit.Size = new System.Drawing.Size(37, 18);
            this.rdoDebit.TabIndex = 109;
            this.rdoDebit.Text = "Dr";
            this.rdoDebit.UseVisualStyleBackColor = true;
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkDate.Location = new System.Drawing.Point(282, 7);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "Da&te :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // txtAmount
            // 
            this.txtAmount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.txtAmount.Location = new System.Drawing.Point(783, 5);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(94, 23);
            this.txtAmount.TabIndex = 106;
            this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAmount_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(710, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 75;
            this.label2.Text = "Balance >";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(-1, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 16);
            this.label8.TabIndex = 72;
            this.label8.Text = "Group :";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(950, 29);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(68, 31);
            this.btnGo.TabIndex = 114;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.Label20.Location = new System.Drawing.Point(434, 9);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(23, 16);
            this.Label20.TabIndex = 24;
            this.Label20.Text = "To";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Controls.Add(this.grpFooter);
            this.panel3.Location = new System.Drawing.Point(12, 128);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1025, 518);
            this.panel3.TabIndex = 901;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
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
            this.check,
            this.all,
            this.tick,
            this.unTick,
            this.partyName,
            this.group,
            this.debitAmt,
            this.creditAmt});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(13, 6);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 28;
            this.dgrdDetails.Size = new System.Drawing.Size(1000, 460);
            this.dgrdDetails.TabIndex = 117;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            // 
            // check
            // 
            this.check.HeaderText = "";
            this.check.Name = "check";
            this.check.Width = 35;
            // 
            // all
            // 
            this.all.HeaderText = "";
            this.all.Name = "all";
            this.all.Width = 50;
            // 
            // tick
            // 
            this.tick.HeaderText = "";
            this.tick.Name = "tick";
            this.tick.Width = 65;
            // 
            // unTick
            // 
            this.unTick.HeaderText = "";
            this.unTick.Name = "unTick";
            this.unTick.Width = 80;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.partyName.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.partyName.Name = "partyName";
            this.partyName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.partyName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.partyName.Width = 350;
            // 
            // group
            // 
            this.group.HeaderText = "Group Name";
            this.group.Name = "group";
            this.group.Width = 135;
            // 
            // debitAmt
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.debitAmt.DefaultCellStyle = dataGridViewCellStyle3;
            this.debitAmt.HeaderText = "Debit Amt";
            this.debitAmt.Name = "debitAmt";
            this.debitAmt.Width = 130;
            // 
            // creditAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.creditAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.creditAmt.HeaderText = "Credit Amt";
            this.creditAmt.Name = "creditAmt";
            this.creditAmt.Width = 130;
            // 
            // grpFooter
            // 
            this.grpFooter.Controls.Add(this.btnClose);
            this.grpFooter.Controls.Add(this.btnPrint);
            this.grpFooter.Controls.Add(this.label6);
            this.grpFooter.Controls.Add(this.label12);
            this.grpFooter.Controls.Add(this.lblBalAmount);
            this.grpFooter.Controls.Add(this.label4);
            this.grpFooter.Controls.Add(this.lblCredit);
            this.grpFooter.Controls.Add(this.btnPreview);
            this.grpFooter.Controls.Add(this.btnPrintMultiLdeger);
            this.grpFooter.Controls.Add(this.lblDebit);
            this.grpFooter.Location = new System.Drawing.Point(13, 462);
            this.grpFooter.Name = "grpFooter";
            this.grpFooter.Size = new System.Drawing.Size(1000, 49);
            this.grpFooter.TabIndex = 118;
            this.grpFooter.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(916, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(79, 33);
            this.btnClose.TabIndex = 116;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(746, 11);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(79, 33);
            this.btnPrint.TabIndex = 114;
            this.btnPrint.Text = "Pri&nt";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(239, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 18);
            this.label6.TabIndex = 87;
            this.label6.Text = "Total Amt :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(10, 29);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 15);
            this.label12.TabIndex = 90;
            this.label12.Text = "Credit Amt :";
            // 
            // lblBalAmount
            // 
            this.lblBalAmount.AutoSize = true;
            this.lblBalAmount.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblBalAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblBalAmount.Location = new System.Drawing.Point(319, 19);
            this.lblBalAmount.Name = "lblBalAmount";
            this.lblBalAmount.Size = new System.Drawing.Size(36, 18);
            this.lblBalAmount.TabIndex = 88;
            this.lblBalAmount.Text = "0.00";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(15, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 89;
            this.label4.Text = "Debit Amt :";
            // 
            // lblCredit
            // 
            this.lblCredit.AutoSize = true;
            this.lblCredit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblCredit.Location = new System.Drawing.Point(87, 29);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(31, 15);
            this.lblCredit.TabIndex = 92;
            this.lblCredit.Text = "0.00";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(825, 11);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(92, 33);
            this.btnPreview.TabIndex = 115;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrintMultiLdeger
            // 
            this.btnPrintMultiLdeger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrintMultiLdeger.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrintMultiLdeger.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrintMultiLdeger.ForeColor = System.Drawing.Color.White;
            this.btnPrintMultiLdeger.Location = new System.Drawing.Point(565, 11);
            this.btnPrintMultiLdeger.Name = "btnPrintMultiLdeger";
            this.btnPrintMultiLdeger.Size = new System.Drawing.Size(181, 33);
            this.btnPrintMultiLdeger.TabIndex = 113;
            this.btnPrintMultiLdeger.Text = "&Print Multi Ledger";
            this.btnPrintMultiLdeger.UseVisualStyleBackColor = false;
            this.btnPrintMultiLdeger.Click += new System.EventHandler(this.btnPrintMultiLdeger_Click);
            // 
            // lblDebit
            // 
            this.lblDebit.AutoSize = true;
            this.lblDebit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblDebit.Location = new System.Drawing.Point(87, 12);
            this.lblDebit.Name = "lblDebit";
            this.lblDebit.Size = new System.Drawing.Size(31, 15);
            this.lblDebit.TabIndex = 91;
            this.lblDebit.Text = "0.00";
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.Transparent;
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.ForeColor = System.Drawing.Color.White;
            this.chkAll.Location = new System.Drawing.Point(68, 144);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(44, 20);
            this.chkAll.TabIndex = 111;
            this.chkAll.Text = "All";
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // chkTick
            // 
            this.chkTick.AutoSize = true;
            this.chkTick.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkTick.ForeColor = System.Drawing.Color.White;
            this.chkTick.Location = new System.Drawing.Point(118, 144);
            this.chkTick.Name = "chkTick";
            this.chkTick.Size = new System.Drawing.Size(53, 20);
            this.chkTick.TabIndex = 112;
            this.chkTick.Text = "&Tick";
            this.chkTick.UseVisualStyleBackColor = true;
            this.chkTick.CheckedChanged += new System.EventHandler(this.chkTick_CheckedChanged);
            // 
            // chkUntick
            // 
            this.chkUntick.AutoSize = true;
            this.chkUntick.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkUntick.ForeColor = System.Drawing.Color.White;
            this.chkUntick.Location = new System.Drawing.Point(184, 143);
            this.chkUntick.Name = "chkUntick";
            this.chkUntick.Size = new System.Drawing.Size(66, 20);
            this.chkUntick.TabIndex = 113;
            this.chkUntick.Text = "Untic&k";
            this.chkUntick.UseVisualStyleBackColor = true;
            this.chkUntick.CheckedChanged += new System.EventHandler(this.chkUntick_CheckedChanged);
            // 
            // chkCheckAll
            // 
            this.chkCheckAll.AutoSize = true;
            this.chkCheckAll.BackColor = System.Drawing.Color.Transparent;
            this.chkCheckAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCheckAll.Location = new System.Drawing.Point(38, 147);
            this.chkCheckAll.Name = "chkCheckAll";
            this.chkCheckAll.Size = new System.Drawing.Size(15, 14);
            this.chkCheckAll.TabIndex = 114;
            this.chkCheckAll.UseVisualStyleBackColor = false;
            this.chkCheckAll.CheckedChanged += new System.EventHandler(this.chkCheckAll_CheckedChanged);
            // 
            // PrintMultiLedger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.chkCheckAll);
            this.Controls.Add(this.chkUntick);
            this.Controls.Add(this.chkTick);
            this.Controls.Add(this.chkAll);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PrintMultiLedger";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ba";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintMultiLedger_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.grpFooter.ResumeLayout(false);
            this.grpFooter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnGo;
        public System.Windows.Forms.Label Label20;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rdoCredit;
        private System.Windows.Forms.RadioButton rdoDebit;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Button btnPrintMultiLdeger;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBalAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblDebit;
        private System.Windows.Forms.Label lblCredit;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        protected internal System.Windows.Forms.CheckBox chkDate;
        protected internal System.Windows.Forms.TextBox txtCityName;
        private System.Windows.Forms.Label label3;
        protected internal System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.GroupBox grpFooter;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.CheckBox chkTick;
        private System.Windows.Forms.CheckBox chkUntick;
        private System.Windows.Forms.CheckBox chkCheckAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label13;
        protected internal System.Windows.Forms.TextBox txtStateName;
        private System.Windows.Forms.Label label5;
        protected internal System.Windows.Forms.TextBox txtNickName;
        private System.Windows.Forms.Label label7;
        protected internal System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.DataGridViewCheckBoxColumn check;
        private System.Windows.Forms.DataGridViewCheckBoxColumn all;
        private System.Windows.Forms.DataGridViewCheckBoxColumn tick;
        private System.Windows.Forms.DataGridViewCheckBoxColumn unTick;
        private System.Windows.Forms.DataGridViewLinkColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn group;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn creditAmt;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}