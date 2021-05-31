namespace SSS
{
    partial class AmendedBillRegister
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoPurchaseReturn = new System.Windows.Forms.RadioButton();
            this.rdoSaleReturn = new System.Windows.Forms.RadioButton();
            this.rdoPurchase = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoSales = new System.Windows.Forms.RadioButton();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picPleasewait = new System.Windows.Forms.PictureBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.chkStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oBillCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oBillDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oRBillCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oRBillDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.txtBillCode);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.grpStatus);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Location = new System.Drawing.Point(15, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(970, 43);
            this.panel2.TabIndex = 1;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(629, 8);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(89, 23);
            this.txtToDate.TabIndex = 3;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(515, 7);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(89, 23);
            this.txtFromDate.TabIndex = 2;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(806, 8);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(90, 23);
            this.txtBillCode.TabIndex = 4;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(728, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 16);
            this.label10.TabIndex = 168;
            this.label10.Text = "Bill Code :";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoPurchaseReturn);
            this.grpStatus.Controls.Add(this.rdoSaleReturn);
            this.grpStatus.Controls.Add(this.rdoPurchase);
            this.grpStatus.Controls.Add(this.rdoAll);
            this.grpStatus.Controls.Add(this.rdoSales);
            this.grpStatus.Location = new System.Drawing.Point(11, -4);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(424, 40);
            this.grpStatus.TabIndex = 0;
            this.grpStatus.TabStop = false;
            // 
            // rdoPurchaseReturn
            // 
            this.rdoPurchaseReturn.AutoSize = true;
            this.rdoPurchaseReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoPurchaseReturn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoPurchaseReturn.Location = new System.Drawing.Point(297, 14);
            this.rdoPurchaseReturn.Name = "rdoPurchaseReturn";
            this.rdoPurchaseReturn.Size = new System.Drawing.Size(121, 19);
            this.rdoPurchaseReturn.TabIndex = 4;
            this.rdoPurchaseReturn.TabStop = true;
            this.rdoPurchaseReturn.Text = "Purchase Return";
            this.rdoPurchaseReturn.UseVisualStyleBackColor = true;
            // 
            // rdoSaleReturn
            // 
            this.rdoSaleReturn.AutoSize = true;
            this.rdoSaleReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSaleReturn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoSaleReturn.Location = new System.Drawing.Point(195, 14);
            this.rdoSaleReturn.Name = "rdoSaleReturn";
            this.rdoSaleReturn.Size = new System.Drawing.Size(98, 19);
            this.rdoSaleReturn.TabIndex = 3;
            this.rdoSaleReturn.TabStop = true;
            this.rdoSaleReturn.Text = "Sales Return";
            this.rdoSaleReturn.UseVisualStyleBackColor = true;
            // 
            // rdoPurchase
            // 
            this.rdoPurchase.AutoSize = true;
            this.rdoPurchase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoPurchase.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoPurchase.Location = new System.Drawing.Point(110, 14);
            this.rdoPurchase.Name = "rdoPurchase";
            this.rdoPurchase.Size = new System.Drawing.Size(80, 19);
            this.rdoPurchase.TabIndex = 2;
            this.rdoPurchase.TabStop = true;
            this.rdoPurchase.Text = "Purchase";
            this.rdoPurchase.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(7, 14);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(39, 19);
            this.rdoAll.TabIndex = 0;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // rdoSales
            // 
            this.rdoSales.AutoSize = true;
            this.rdoSales.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSales.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rdoSales.Location = new System.Drawing.Point(48, 14);
            this.rdoSales.Name = "rdoSales";
            this.rdoSales.Size = new System.Drawing.Size(57, 19);
            this.rdoSales.TabIndex = 1;
            this.rdoSales.TabStop = true;
            this.rdoSales.Text = "Sales";
            this.rdoSales.UseVisualStyleBackColor = true;
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chkDate.Location = new System.Drawing.Point(449, 9);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(67, 20);
            this.chkDate.TabIndex = 1;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(606, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "To";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(900, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(62, 33);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(15, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(970, 38);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(373, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "AMENDED BILL REGISTER";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnExport);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Controls.Add(this.picPleasewait);
            this.panel3.Controls.Add(this.chkAll);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Location = new System.Drawing.Point(15, 112);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(970, 531);
            this.panel3.TabIndex = 2;
            this.panel3.TabStop = true;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(121, 492);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(84, 33);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(8, 492);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 33);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picPleasewait
            // 
            this.picPleasewait.Image = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.InitialImage = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.Location = new System.Drawing.Point(402, 162);
            this.picPleasewait.Name = "picPleasewait";
            this.picPleasewait.Size = new System.Drawing.Size(185, 132);
            this.picPleasewait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPleasewait.TabIndex = 207;
            this.picPleasewait.TabStop = false;
            this.picPleasewait.Visible = false;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Location = new System.Drawing.Point(22, 22);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 115;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(224, 492);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 33);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkStatus,
            this.date,
            this.billType,
            this.oBillCode,
            this.oBillDate,
            this.oRBillCode,
            this.oRBillDate,
            this.description1,
            this.description2,
            this.description3,
            this.createdBy});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.Color.DimGray;
            this.dgrdDetails.Location = new System.Drawing.Point(12, 12);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.Size = new System.Drawing.Size(943, 477);
            this.dgrdDetails.TabIndex = 0;
            this.dgrdDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdSMS_CellBeginEdit);
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdSMS_KeyDown);
            // 
            // chkStatus
            // 
            this.chkStatus.HeaderText = "";
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Width = 30;
            // 
            // date
            // 
            dataGridViewCellStyle15.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle15;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Width = 90;
            // 
            // billType
            // 
            this.billType.HeaderText = "Bill Type";
            this.billType.Name = "billType";
            // 
            // oBillCode
            // 
            this.oBillCode.HeaderText = "O.Bill No";
            this.oBillCode.Name = "oBillCode";
            // 
            // oBillDate
            // 
            dataGridViewCellStyle16.Format = "dd/MM/yyyy";
            this.oBillDate.DefaultCellStyle = dataGridViewCellStyle16;
            this.oBillDate.HeaderText = "O.Bill Date";
            this.oBillDate.Name = "oBillDate";
            // 
            // oRBillCode
            // 
            this.oRBillCode.HeaderText = "O.R.Bill No";
            this.oRBillCode.Name = "oRBillCode";
            // 
            // oRBillDate
            // 
            dataGridViewCellStyle17.Format = "dd/MM/yyyy";
            this.oRBillDate.DefaultCellStyle = dataGridViewCellStyle17;
            this.oRBillDate.HeaderText = "O.R.Bill Date";
            this.oRBillDate.Name = "oRBillDate";
            // 
            // description1
            // 
            this.description1.HeaderText = "Description_1";
            this.description1.Name = "description1";
            // 
            // description2
            // 
            this.description2.HeaderText = "Description_2";
            this.description2.Name = "description2";
            // 
            // description3
            // 
            this.description3.HeaderText = "Description_3";
            this.description3.Name = "description3";
            // 
            // createdBy
            // 
            this.createdBy.HeaderText = "Created By";
            this.createdBy.Name = "createdBy";
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle18.Format = "dd/MM/yyyy";
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle18;
            this.dataGridViewTextBoxColumn1.HeaderText = "Date";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Sender ID";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 80;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Mobile No";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 160;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Message";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 310;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Status";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 90;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Reason";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "ID";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Visible = false;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "ID";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Visible = false;
            // 
            // AmendedBillRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AmendedBillRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMSReportRegister";
            this.Load += new System.EventHandler(this.AmendedBillRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SMSReportRegister_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.PictureBox picPleasewait;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RadioButton rdoPurchase;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.RadioButton rdoSales;
        private System.Windows.Forms.RadioButton rdoPurchaseReturn;
        private System.Windows.Forms.RadioButton rdoSaleReturn;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn billType;
        private System.Windows.Forms.DataGridViewTextBoxColumn oBillCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn oBillDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn oRBillCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn oRBillDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn description1;
        private System.Windows.Forms.DataGridViewTextBoxColumn description2;
        private System.Windows.Forms.DataGridViewTextBoxColumn description3;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdBy;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}