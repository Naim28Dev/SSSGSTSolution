namespace SSS
{
    partial class BankGuaranteeRegister
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.chkTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.billno = new System.Windows.Forms.DataGridViewLinkColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bgno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.validupToDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtToValidUpto = new System.Windows.Forms.MaskedTextBox();
            this.txtFromValidUpto = new System.Windows.Forms.MaskedTextBox();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBGNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkValidUptoDate = new System.Windows.Forms.CheckBox();
            this.btnBankName = new System.Windows.Forms.Button();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnCustomerName = new System.Windows.Forms.Button();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblBankAccount = new System.Windows.Forms.Label();
            this.btnGO = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lnkColor = new System.Windows.Forms.LinkLabel();
            this.Label30 = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.pnlColor2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.pnlColor.SuspendLayout();
            this.pnlColor2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkAll.ForeColor = System.Drawing.Color.Black;
            this.chkAll.Location = new System.Drawing.Point(19, 19);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 150;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckStateChanged += new System.EventHandler(this.chkAll_CheckStateChanged);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(923, 465);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 33);
            this.btnClose.TabIndex = 114;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.btnExport);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.lblTotalAmt);
            this.panel2.Controls.Add(this.chkAll);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.dgrdDetails);
            this.panel2.Location = new System.Drawing.Point(15, 137);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1017, 505);
            this.panel2.TabIndex = 131;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(842, 465);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(82, 33);
            this.btnExport.TabIndex = 163;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 473);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 16);
            this.label9.TabIndex = 151;
            this.label9.Text = "Total Amt :";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.AutoSize = true;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAmt.Location = new System.Drawing.Point(90, 474);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size(33, 16);
            this.lblTotalAmt.TabIndex = 152;
            this.lblTotalAmt.Text = "0.00";
            this.lblTotalAmt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgrdDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkTick,
            this.billno,
            this.date,
            this.customername,
            this.bgno,
            this.amount,
            this.bankName,
            this.validupToDate,
            this.createdBy,
            this.updatedBy,
            this.id});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.MenuHighlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdDetails.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.Color.Gray;
            this.dgrdDetails.Location = new System.Drawing.Point(11, 10);
            this.dgrdDetails.Name = "dgrdDetails";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdDetails.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 9.75F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            this.dgrdDetails.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(993, 450);
            this.dgrdDetails.TabIndex = 110;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdDetails_CellBeginEdit);
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            // 
            // chkTick
            // 
            this.chkTick.HeaderText = "";
            this.chkTick.Name = "chkTick";
            this.chkTick.Width = 25;
            // 
            // billno
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.billno.DefaultCellStyle = dataGridViewCellStyle3;
            this.billno.HeaderText = "Bill No";
            this.billno.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.billno.Name = "billno";
            this.billno.ReadOnly = true;
            this.billno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.billno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.billno.Width = 110;
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 85;
            // 
            // customername
            // 
            this.customername.HeaderText = "Customer Name";
            this.customername.Name = "customername";
            this.customername.Width = 150;
            // 
            // bgno
            // 
            this.bgno.HeaderText = "BG No";
            this.bgno.Name = "bgno";
            this.bgno.Width = 80;
            // 
            // amount
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.amount.DefaultCellStyle = dataGridViewCellStyle4;
            this.amount.HeaderText = "Amount";
            this.amount.Name = "amount";
            // 
            // bankName
            // 
            this.bankName.HeaderText = "Bank Name";
            this.bankName.Name = "bankName";
            this.bankName.Width = 150;
            // 
            // validupToDate
            // 
            this.validupToDate.HeaderText = "Valid Upto";
            this.validupToDate.Name = "validupToDate";
            this.validupToDate.Width = 90;
            // 
            // createdBy
            // 
            this.createdBy.HeaderText = "Created By";
            this.createdBy.Name = "createdBy";
            this.createdBy.ReadOnly = true;
            // 
            // updatedBy
            // 
            this.updatedBy.HeaderText = "Updated By";
            this.updatedBy.Name = "updatedBy";
            this.updatedBy.ReadOnly = true;
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.txtToValidUpto);
            this.panel1.Controls.Add(this.txtFromValidUpto);
            this.panel1.Controls.Add(this.txtToDate);
            this.panel1.Controls.Add(this.txtFromDate);
            this.panel1.Controls.Add(this.txtBGNo);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.chkValidUptoDate);
            this.panel1.Controls.Add(this.btnBankName);
            this.panel1.Controls.Add(this.txtBankName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtBillCode);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.btnCustomerName);
            this.panel1.Controls.Add(this.txtCustomerName);
            this.panel1.Controls.Add(this.lblBankAccount);
            this.panel1.Controls.Add(this.btnGO);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.chkDate);
            this.panel1.Location = new System.Drawing.Point(15, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1018, 75);
            this.panel1.TabIndex = 130;
            // 
            // txtToValidUpto
            // 
            this.txtToValidUpto.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtToValidUpto.Location = new System.Drawing.Point(615, 33);
            this.txtToValidUpto.Mask = "00/00/0000";
            this.txtToValidUpto.Name = "txtToValidUpto";
            this.txtToValidUpto.Size = new System.Drawing.Size(76, 22);
            this.txtToValidUpto.TabIndex = 156;
            this.txtToValidUpto.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToValidUpto.Leave += new System.EventHandler(this.txtFromValidUpto_Leave);
            // 
            // txtFromValidUpto
            // 
            this.txtFromValidUpto.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtFromValidUpto.Location = new System.Drawing.Point(507, 32);
            this.txtFromValidUpto.Mask = "00/00/0000";
            this.txtFromValidUpto.Name = "txtFromValidUpto";
            this.txtFromValidUpto.Size = new System.Drawing.Size(81, 22);
            this.txtFromValidUpto.TabIndex = 155;
            this.txtFromValidUpto.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromValidUpto.Leave += new System.EventHandler(this.txtFromValidUpto_Leave);
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtToDate.Location = new System.Drawing.Point(615, 6);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(76, 22);
            this.txtToDate.TabIndex = 103;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtFromDate.Location = new System.Drawing.Point(507, 6);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(81, 22);
            this.txtFromDate.TabIndex = 102;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtBGNo
            // 
            this.txtBGNo.BackColor = System.Drawing.Color.White;
            this.txtBGNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBGNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBGNo.Location = new System.Drawing.Point(792, 34);
            this.txtBGNo.Name = "txtBGNo";
            this.txtBGNo.Size = new System.Drawing.Size(134, 23);
            this.txtBGNo.TabIndex = 160;
            this.txtBGNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBGNo_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(697, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 16);
            this.label5.TabIndex = 159;
            this.label5.Text = "Bank G. No  :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(590, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 157;
            this.label2.Text = "To";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkValidUptoDate
            // 
            this.chkValidUptoDate.AutoSize = true;
            this.chkValidUptoDate.ForeColor = System.Drawing.Color.Black;
            this.chkValidUptoDate.Location = new System.Drawing.Point(404, 35);
            this.chkValidUptoDate.Name = "chkValidUptoDate";
            this.chkValidUptoDate.Size = new System.Drawing.Size(100, 20);
            this.chkValidUptoDate.TabIndex = 154;
            this.chkValidUptoDate.Text = "Valid Date :";
            this.chkValidUptoDate.UseVisualStyleBackColor = true;
            this.chkValidUptoDate.CheckedChanged += new System.EventHandler(this.chkDepositeDate_CheckedChanged);
            // 
            // btnBankName
            // 
            this.btnBankName.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnBankName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBankName.Location = new System.Drawing.Point(372, 33);
            this.btnBankName.Name = "btnBankName";
            this.btnBankName.Size = new System.Drawing.Size(24, 25);
            this.btnBankName.TabIndex = 151;
            this.btnBankName.TabStop = false;
            this.btnBankName.UseVisualStyleBackColor = true;
            this.btnBankName.Click += new System.EventHandler(this.btnBankName_Click);
            // 
            // txtBankName
            // 
            this.txtBankName.BackColor = System.Drawing.Color.White;
            this.txtBankName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBankName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBankName.Location = new System.Drawing.Point(129, 34);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.ReadOnly = true;
            this.txtBankName.Size = new System.Drawing.Size(242, 23);
            this.txtBankName.TabIndex = 152;
            this.txtBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 16);
            this.label1.TabIndex = 153;
            this.label1.Text = "Bank Name         :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(792, 5);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(134, 23);
            this.txtBillCode.TabIndex = 150;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            this.txtBillCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBillCode_KeyPress);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(697, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(90, 16);
            this.label15.TabIndex = 149;
            this.label15.Text = "Bill Code     :";
            // 
            // btnCustomerName
            // 
            this.btnCustomerName.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnCustomerName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCustomerName.Location = new System.Drawing.Point(372, 4);
            this.btnCustomerName.Name = "btnCustomerName";
            this.btnCustomerName.Size = new System.Drawing.Size(24, 25);
            this.btnCustomerName.TabIndex = 146;
            this.btnCustomerName.TabStop = false;
            this.btnCustomerName.UseVisualStyleBackColor = true;
            this.btnCustomerName.Click += new System.EventHandler(this.btnCustomerName_Click);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.Color.White;
            this.txtCustomerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCustomerName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCustomerName.Location = new System.Drawing.Point(129, 5);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(242, 23);
            this.txtCustomerName.TabIndex = 147;
            this.txtCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerName_KeyDown);
            // 
            // lblBankAccount
            // 
            this.lblBankAccount.AutoSize = true;
            this.lblBankAccount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblBankAccount.ForeColor = System.Drawing.Color.Black;
            this.lblBankAccount.Location = new System.Drawing.Point(2, 9);
            this.lblBankAccount.Name = "lblBankAccount";
            this.lblBankAccount.Size = new System.Drawing.Size(127, 16);
            this.lblBankAccount.TabIndex = 148;
            this.lblBankAccount.Text = "Customer Name :";
            this.lblBankAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGO
            // 
            this.btnGO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGO.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGO.ForeColor = System.Drawing.Color.White;
            this.btnGO.Location = new System.Drawing.Point(932, 31);
            this.btnGO.Name = "btnGO";
            this.btnGO.Size = new System.Drawing.Size(72, 30);
            this.btnGO.TabIndex = 110;
            this.btnGO.Text = "&Go";
            this.btnGO.UseVisualStyleBackColor = false;
            this.btnGO.Click += new System.EventHandler(this.btnGO_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(590, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 16);
            this.label3.TabIndex = 145;
            this.label3.Text = "To";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.ForeColor = System.Drawing.Color.Black;
            this.chkDate.Location = new System.Drawing.Point(405, 7);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(100, 20);
            this.chkDate.TabIndex = 101;
            this.chkDate.Text = "Date          :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.AutoSize = true;
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.lnkColor);
            this.panel4.Controls.Add(this.Label30);
            this.panel4.Location = new System.Drawing.Point(15, 13);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1017, 37);
            this.panel4.TabIndex = 132;
            // 
            // lnkColor
            // 
            this.lnkColor.AutoSize = true;
            this.lnkColor.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lnkColor.ForeColor = System.Drawing.Color.Black;
            this.lnkColor.LinkColor = System.Drawing.Color.Black;
            this.lnkColor.Location = new System.Drawing.Point(927, 8);
            this.lnkColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkColor.Name = "lnkColor";
            this.lnkColor.Size = new System.Drawing.Size(78, 16);
            this.lnkColor.TabIndex = 100004;
            this.lnkColor.TabStop = true;
            this.lnkColor.Text = "Color Hint";
            this.lnkColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkColor_LinkClicked);
            // 
            // Label30
            // 
            this.Label30.AutoSize = true;
            this.Label30.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.Label30.ForeColor = System.Drawing.Color.Black;
            this.Label30.Location = new System.Drawing.Point(391, 6);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(253, 19);
            this.Label30.TabIndex = 10058;
            this.Label30.Text = "BANK GUARANTEE REGISTER";
            // 
            // pnlColor
            // 
            this.pnlColor.BackColor = System.Drawing.Color.DimGray;
            this.pnlColor.Controls.Add(this.pnlColor2);
            this.pnlColor.Location = new System.Drawing.Point(754, 42);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(279, 130);
            this.pnlColor.TabIndex = 165;
            this.pnlColor.Visible = false;
            // 
            // pnlColor2
            // 
            this.pnlColor2.BackColor = System.Drawing.Color.White;
            this.pnlColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColor2.Controls.Add(this.label4);
            this.pnlColor2.Controls.Add(this.panel8);
            this.pnlColor2.Controls.Add(this.panel5);
            this.pnlColor2.Controls.Add(this.label14);
            this.pnlColor2.Controls.Add(this.label11);
            this.pnlColor2.Location = new System.Drawing.Point(19, 17);
            this.pnlColor2.Margin = new System.Windows.Forms.Padding(4);
            this.pnlColor2.Name = "pnlColor2";
            this.pnlColor2.Size = new System.Drawing.Size(240, 93);
            this.pnlColor2.TabIndex = 100025;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(30, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 16);
            this.label4.TabIndex = 136;
            this.label4.Text = "Validity of Bank Guarantee";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Tomato;
            this.panel8.Location = new System.Drawing.Point(11, 60);
            this.panel8.Margin = new System.Windows.Forms.Padding(4);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(94, 20);
            this.panel8.TabIndex = 135;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Gold;
            this.panel5.Location = new System.Drawing.Point(11, 33);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(94, 20);
            this.panel5.TabIndex = 132;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(106, 33);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(117, 16);
            this.label14.TabIndex = 130;
            this.label14.Text = ": Withing 30 Days";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(106, 61);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 16);
            this.label11.TabIndex = 127;
            this.label11.Text = ": Expired";
            // 
            // BankGuaranteeRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BankGuaranteeRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cheque Detail Register";
            this.Load += new System.EventHandler(this.BankGuaranteeRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BankGuaranteeRegister_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.pnlColor.ResumeLayout(false);
            this.pnlColor2.ResumeLayout(false);
            this.pnlColor2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGO;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblBankAccount;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkValidUptoDate;
        private System.Windows.Forms.Button btnBankName;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Label30;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalAmt;
        private System.Windows.Forms.TextBox txtBGNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromValidUpto;
        private System.Windows.Forms.MaskedTextBox txtToValidUpto;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkTick;
        private System.Windows.Forms.DataGridViewLinkColumn billno;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn customername;
        private System.Windows.Forms.DataGridViewTextBoxColumn bgno;
        private System.Windows.Forms.DataGridViewTextBoxColumn amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn bankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn validupToDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.LinkLabel lnkColor;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.Panel pnlColor2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
    }
}