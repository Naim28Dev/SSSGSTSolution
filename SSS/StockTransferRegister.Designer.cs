namespace SSS
{
    partial class StockTransferRegister
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtToSNo = new System.Windows.Forms.TextBox();
            this.txtFromSNo = new System.Windows.Forms.TextBox();
            this.chkSNo = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.txtSourceBillNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaterialCenter = new System.Windows.Forms.TextBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoStockOut = new System.Windows.Forms.RadioButton();
            this.rdoStockIn = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.fromMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stockType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.variant1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.variant2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceBillNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdby = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedby = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblAmt = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtToSNo
            // 
            this.txtToSNo.BackColor = System.Drawing.Color.White;
            this.txtToSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToSNo.Location = new System.Drawing.Point(547, 28);
            this.txtToSNo.MaxLength = 10;
            this.txtToSNo.Name = "txtToSNo";
            this.txtToSNo.ReadOnly = true;
            this.txtToSNo.Size = new System.Drawing.Size(88, 23);
            this.txtToSNo.TabIndex = 7;
            this.txtToSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // txtFromSNo
            // 
            this.txtFromSNo.BackColor = System.Drawing.Color.White;
            this.txtFromSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromSNo.Location = new System.Drawing.Point(435, 28);
            this.txtFromSNo.MaxLength = 10;
            this.txtFromSNo.Name = "txtFromSNo";
            this.txtFromSNo.ReadOnly = true;
            this.txtFromSNo.Size = new System.Drawing.Size(87, 23);
            this.txtFromSNo.TabIndex = 6;
            this.txtFromSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // chkSNo
            // 
            this.chkSNo.AutoSize = true;
            this.chkSNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.chkSNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkSNo.Location = new System.Drawing.Point(361, 29);
            this.chkSNo.Name = "chkSNo";
            this.chkSNo.Size = new System.Drawing.Size(77, 20);
            this.chkSNo.TabIndex = 5;
            this.chkSNo.Text = "Bill No :";
            this.chkSNo.UseVisualStyleBackColor = true;
            this.chkSNo.CheckedChanged += new System.EventHandler(this.chkSNo_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(524, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 16);
            this.label5.TabIndex = 136;
            this.label5.Text = "To";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtRemark);
            this.panel2.Controls.Add(this.txtSourceBillNo);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.txtItemName);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtToSNo);
            this.panel2.Controls.Add(this.txtFromSNo);
            this.panel2.Controls.Add(this.txtBillCode);
            this.panel2.Controls.Add(this.chkSNo);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtMaterialCenter);
            this.panel2.Controls.Add(this.grpStatus);
            this.panel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(7, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1036, 57);
            this.panel2.TabIndex = 0;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(547, 3);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(88, 23);
            this.txtToDate.TabIndex = 4;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(435, 3);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(88, 23);
            this.txtFromDate.TabIndex = 3;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(833, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 16);
            this.label7.TabIndex = 5;
            this.label7.Text = "Source Bill No :";
            // 
            // txtRemark
            // 
            this.txtRemark.BackColor = System.Drawing.Color.White;
            this.txtRemark.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRemark.Font = new System.Drawing.Font("Arial", 10F);
            this.txtRemark.Location = new System.Drawing.Point(709, 28);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(121, 23);
            this.txtRemark.TabIndex = 9;
            this.txtRemark.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSourceBillNo_KeyPress);
            // 
            // txtSourceBillNo
            // 
            this.txtSourceBillNo.BackColor = System.Drawing.Color.White;
            this.txtSourceBillNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSourceBillNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSourceBillNo.Location = new System.Drawing.Point(941, 2);
            this.txtSourceBillNo.Name = "txtSourceBillNo";
            this.txtSourceBillNo.Size = new System.Drawing.Size(86, 23);
            this.txtSourceBillNo.TabIndex = 10;
            this.txtSourceBillNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSourceBillNo_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(643, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 16);
            this.label6.TabIndex = 168;
            this.label6.Text = "Remark :";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label19.Location = new System.Drawing.Point(33, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(85, 16);
            this.label19.TabIndex = 167;
            this.label19.Text = "Stock Type :";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtItemName
            // 
            this.txtItemName.BackColor = System.Drawing.Color.White;
            this.txtItemName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtItemName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtItemName.Location = new System.Drawing.Point(873, 28);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(105, 23);
            this.txtItemName.TabIndex = 11;
            this.txtItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemName_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(832, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 16);
            this.label3.TabIndex = 138;
            this.label3.Text = "Item :";
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(709, 3);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(121, 23);
            this.txtBillCode.TabIndex = 8;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(4, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Material Center :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(635, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 16);
            this.label10.TabIndex = 124;
            this.label10.Text = "Bill Code :";
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkDate.Location = new System.Drawing.Point(361, 5);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(76, 20);
            this.chkDate.TabIndex = 2;
            this.chkDate.Text = "Date    :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(980, 25);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(50, 29);
            this.btnGo.TabIndex = 12;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(524, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "To";
            // 
            // txtMaterialCenter
            // 
            this.txtMaterialCenter.BackColor = System.Drawing.Color.White;
            this.txtMaterialCenter.Font = new System.Drawing.Font("Arial", 10F);
            this.txtMaterialCenter.Location = new System.Drawing.Point(122, 2);
            this.txtMaterialCenter.Name = "txtMaterialCenter";
            this.txtMaterialCenter.ReadOnly = true;
            this.txtMaterialCenter.Size = new System.Drawing.Size(231, 23);
            this.txtMaterialCenter.TabIndex = 0;
            this.txtMaterialCenter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMaterialCenter_KeyDown);
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoStockOut);
            this.grpStatus.Controls.Add(this.rdoStockIn);
            this.grpStatus.Controls.Add(this.rdoAll);
            this.grpStatus.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.grpStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.grpStatus.Location = new System.Drawing.Point(122, 21);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(231, 31);
            this.grpStatus.TabIndex = 1;
            this.grpStatus.TabStop = false;
            // 
            // rdoStockOut
            // 
            this.rdoStockOut.AutoSize = true;
            this.rdoStockOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoStockOut.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoStockOut.Location = new System.Drawing.Point(137, 10);
            this.rdoStockOut.Name = "rdoStockOut";
            this.rdoStockOut.Size = new System.Drawing.Size(82, 18);
            this.rdoStockOut.TabIndex = 2;
            this.rdoStockOut.Text = "Stock-OUT";
            this.rdoStockOut.UseVisualStyleBackColor = true;
            // 
            // rdoStockIn
            // 
            this.rdoStockIn.AutoSize = true;
            this.rdoStockIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoStockIn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoStockIn.Location = new System.Drawing.Point(59, 10);
            this.rdoStockIn.Name = "rdoStockIn";
            this.rdoStockIn.Size = new System.Drawing.Size(70, 18);
            this.rdoStockIn.TabIndex = 1;
            this.rdoStockIn.Text = "Stock-IN";
            this.rdoStockIn.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(9, 10);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(39, 18);
            this.rdoAll.TabIndex = 0;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(7, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1036, 34);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(418, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "STOCK TRANSFER REGISTER";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToOrderColumns = true;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle19;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.billNo,
            this.fromMC,
            this.toMC,
            this.stockType,
            this.itemName,
            this.variant1,
            this.variant2,
            this.qty,
            this.rate,
            this.amount,
            this.remark,
            this.sourceBillNo,
            this.createdby,
            this.updatedby,
            this.id});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(8, 6);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(1018, 478);
            this.dgrdDetails.TabIndex = 137;
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            // 
            // date
            // 
            dataGridViewCellStyle21.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle21;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.date.Width = 90;
            // 
            // billNo
            // 
            this.billNo.HeaderText = "Bill No";
            this.billNo.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.billNo.Name = "billNo";
            this.billNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.billNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.billNo.Width = 120;
            // 
            // fromMC
            // 
            this.fromMC.HeaderText = "From M.Center";
            this.fromMC.Name = "fromMC";
            this.fromMC.Width = 130;
            // 
            // toMC
            // 
            this.toMC.HeaderText = "To M.Center";
            this.toMC.Name = "toMC";
            this.toMC.Width = 130;
            // 
            // stockType
            // 
            this.stockType.HeaderText = "S.Type";
            this.stockType.Name = "stockType";
            this.stockType.Width = 60;
            // 
            // itemName
            // 
            this.itemName.HeaderText = "Item Name";
            this.itemName.Name = "itemName";
            this.itemName.Width = 120;
            // 
            // variant1
            // 
            this.variant1.HeaderText = "Variant1";
            this.variant1.Name = "variant1";
            this.variant1.Width = 80;
            // 
            // variant2
            // 
            this.variant2.HeaderText = "Variant2";
            this.variant2.Name = "variant2";
            this.variant2.Width = 80;
            // 
            // qty
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.qty.DefaultCellStyle = dataGridViewCellStyle22;
            this.qty.HeaderText = "Qty";
            this.qty.Name = "qty";
            this.qty.Width = 80;
            // 
            // rate
            // 
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.rate.DefaultCellStyle = dataGridViewCellStyle23;
            this.rate.HeaderText = "Rate";
            this.rate.Name = "rate";
            this.rate.Width = 90;
            // 
            // amount
            // 
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.amount.DefaultCellStyle = dataGridViewCellStyle24;
            this.amount.HeaderText = "Amount";
            this.amount.Name = "amount";
            this.amount.Width = 110;
            // 
            // remark
            // 
            this.remark.HeaderText = "Remark";
            this.remark.Name = "remark";
            this.remark.Width = 120;
            // 
            // sourceBillNo
            // 
            this.sourceBillNo.HeaderText = "Source BillNo";
            this.sourceBillNo.Name = "sourceBillNo";
            this.sourceBillNo.Width = 110;
            // 
            // createdby
            // 
            this.createdby.HeaderText = "Created By";
            this.createdby.Name = "createdby";
            this.createdby.Width = 110;
            // 
            // updatedby
            // 
            this.updatedby.HeaderText = "Updated By";
            this.updatedby.Name = "updatedby";
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.Location = new System.Drawing.Point(7, 113);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1036, 494);
            this.panel3.TabIndex = 2;
            this.panel3.Tag = "0";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.btnExport);
            this.panel4.Controls.Add(this.btnPreview);
            this.panel4.Controls.Add(this.btnPrint);
            this.panel4.Controls.Add(this.btnExpand);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.lblQty);
            this.panel4.Controls.Add(this.lblAmt);
            this.panel4.Location = new System.Drawing.Point(7, 613);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1037, 36);
            this.panel4.TabIndex = 3;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(702, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(82, 30);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(863, 2);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(90, 30);
            this.btnPreview.TabIndex = 3;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(784, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(79, 30);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExpand.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExpand.ForeColor = System.Drawing.Color.White;
            this.btnExpand.Location = new System.Drawing.Point(618, 2);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(82, 30);
            this.btnExpand.TabIndex = 0;
            this.btnExpand.Text = "Expand";
            this.btnExpand.UseVisualStyleBackColor = false;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(953, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(74, 30);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(200, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "Total Amt :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(6, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 16);
            this.label12.TabIndex = 12;
            this.label12.Text = "Total Qty :";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblQty.Location = new System.Drawing.Point(81, 10);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(16, 16);
            this.lblQty.TabIndex = 18;
            this.lblQty.Text = "0";
            // 
            // lblAmt
            // 
            this.lblAmt.AutoSize = true;
            this.lblAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblAmt.Location = new System.Drawing.Point(278, 10);
            this.lblAmt.Name = "lblAmt";
            this.lblAmt.Size = new System.Drawing.Size(16, 16);
            this.lblAmt.TabIndex = 16;
            this.lblAmt.Text = "0";
            // 
            // StockTransferRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StockTransferRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Transfer Register";
            this.Load += new System.EventHandler(this.StockTransferRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StockTransferRegister_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        protected internal System.Windows.Forms.TextBox txtToSNo;
        protected internal System.Windows.Forms.TextBox txtFromSNo;
        private System.Windows.Forms.CheckBox chkSNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkDate;
        protected internal System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.TextBox txtMaterialCenter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RadioButton rdoStockOut;
        private System.Windows.Forms.RadioButton rdoStockIn;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.TextBox txtSourceBillNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblAmt;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewLinkColumn billNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn fromMC;
        private System.Windows.Forms.DataGridViewTextBoxColumn toMC;
        private System.Windows.Forms.DataGridViewTextBoxColumn stockType;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn variant1;
        private System.Windows.Forms.DataGridViewTextBoxColumn variant2;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty;
        private System.Windows.Forms.DataGridViewTextBoxColumn rate;
        private System.Windows.Forms.DataGridViewTextBoxColumn amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn sourceBillNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdby;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedby;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.Button btnExport;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}