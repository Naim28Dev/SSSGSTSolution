namespace SSS
{
    partial class SchemeDetailMaster
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle35 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle34 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle36 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle37 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle39 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle38 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle40 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panColor = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this._TourTab = new System.Windows.Forms.TabControl();
            this._supplierDetails = new System.Windows.Forms.TabPage();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.dgrdSupplier = new System.Windows.Forms.DataGridView();
            this.sSno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.supplierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disPer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amtValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.branchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._customerDetails = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgrdCustomer = new System.Windows.Forms.DataGridView();
            this.cSno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.txtSchemeName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblNameHeader = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.LinkLabel();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.panColor.SuspendLayout();
            this.panel5.SuspendLayout();
            this._TourTab.SuspendLayout();
            this._supplierDetails.SuspendLayout();
            this.grpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSupplier)).BeginInit();
            this._customerDetails.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdCustomer)).BeginInit();
            this.panel3.SuspendLayout();
            this.panHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panColor
            // 
            this.panColor.BackColor = System.Drawing.Color.White;
            this.panColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panColor.Controls.Add(this.panel5);
            this.panColor.Location = new System.Drawing.Point(19, 82);
            this.panColor.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(980, 557);
            this.panColor.TabIndex = 100;
            this.panColor.TabStop = true;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this._TourTab);
            this.panel5.Controls.Add(this.panel3);
            this.panel5.Controls.Add(this.lblId);
            this.panel5.Location = new System.Drawing.Point(25, 18);
            this.panel5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(933, 525);
            this.panel5.TabIndex = 101;
            this.panel5.TabStop = true;
            // 
            // _TourTab
            // 
            this._TourTab.Controls.Add(this._supplierDetails);
            this._TourTab.Controls.Add(this._customerDetails);
            this._TourTab.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._TourTab.Location = new System.Drawing.Point(22, 57);
            this._TourTab.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._TourTab.Name = "_TourTab";
            this._TourTab.Padding = new System.Drawing.Point(40, 7);
            this._TourTab.SelectedIndex = 0;
            this._TourTab.Size = new System.Drawing.Size(884, 450);
            this._TourTab.TabIndex = 209;
            // 
            // _supplierDetails
            // 
            this._supplierDetails.Controls.Add(this.grpDetails);
            this._supplierDetails.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._supplierDetails.Location = new System.Drawing.Point(4, 33);
            this._supplierDetails.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._supplierDetails.Name = "_supplierDetails";
            this._supplierDetails.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._supplierDetails.Size = new System.Drawing.Size(876, 413);
            this._supplierDetails.TabIndex = 0;
            this._supplierDetails.Text = "Supplier Details";
            this._supplierDetails.UseVisualStyleBackColor = true;
            // 
            // grpDetails
            // 
            this.grpDetails.Controls.Add(this.dgrdSupplier);
            this.grpDetails.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.grpDetails.Location = new System.Drawing.Point(15, 7);
            this.grpDetails.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpDetails.Size = new System.Drawing.Size(847, 389);
            this.grpDetails.TabIndex = 118;
            this.grpDetails.TabStop = false;
            // 
            // dgrdSupplier
            // 
            this.dgrdSupplier.AllowUserToAddRows = false;
            this.dgrdSupplier.AllowUserToDeleteRows = false;
            this.dgrdSupplier.AllowUserToResizeRows = false;
            dataGridViewCellStyle31.BackColor = System.Drawing.Color.White;
            this.dgrdSupplier.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle31;
            this.dgrdSupplier.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle32.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle32.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle32.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle32.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle32.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSupplier.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle32;
            this.dgrdSupplier.ColumnHeadersHeight = 30;
            this.dgrdSupplier.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSupplier.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sSno,
            this.supplierName,
            this.disPer,
            this.amtValue,
            this.startDate,
            this.endDate,
            this.branchName,
            this.sID});
            this.dgrdSupplier.Cursor = System.Windows.Forms.Cursors.Default;
            this.dgrdSupplier.EnableHeadersVisualStyles = false;
            this.dgrdSupplier.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSupplier.Location = new System.Drawing.Point(16, 24);
            this.dgrdSupplier.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dgrdSupplier.Name = "dgrdSupplier";
            this.dgrdSupplier.RowHeadersVisible = false;
            dataGridViewCellStyle35.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSupplier.RowsDefaultCellStyle = dataGridViewCellStyle35;
            this.dgrdSupplier.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSupplier.RowTemplate.Height = 28;
            this.dgrdSupplier.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdSupplier.Size = new System.Drawing.Size(813, 347);
            this.dgrdSupplier.TabIndex = 119;
            this.dgrdSupplier.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdSupplier_CellBeginEdit);
            this.dgrdSupplier.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgrdSupplier_CellValidating);
            this.dgrdSupplier.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdSupplier_EditingControlShowing);
            this.dgrdSupplier.Sorted += new System.EventHandler(this.dgrdSupplier_Sorted);
            this.dgrdSupplier.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdSupplier_KeyDown);
            // 
            // sSno
            // 
            this.sSno.HeaderText = "S.No.";
            this.sSno.Name = "sSno";
            this.sSno.Width = 55;
            // 
            // supplierName
            // 
            this.supplierName.HeaderText = "Supplier Name";
            this.supplierName.Name = "supplierName";
            this.supplierName.Width = 290;
            // 
            // disPer
            // 
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle33.Format = "N2";
            this.disPer.DefaultCellStyle = dataGridViewCellStyle33;
            this.disPer.HeaderText = "Dis %";
            this.disPer.Name = "disPer";
            this.disPer.Width = 80;
            // 
            // amtValue
            // 
            dataGridViewCellStyle34.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle34.Format = "N2";
            this.amtValue.DefaultCellStyle = dataGridViewCellStyle34;
            this.amtValue.HeaderText = "Value";
            this.amtValue.Name = "amtValue";
            this.amtValue.Width = 90;
            // 
            // startDate
            // 
            this.startDate.HeaderText = "Start Date";
            this.startDate.Name = "startDate";
            // 
            // endDate
            // 
            this.endDate.HeaderText = "End Date";
            this.endDate.Name = "endDate";
            // 
            // branchName
            // 
            this.branchName.HeaderText = "Branch";
            this.branchName.Name = "branchName";
            this.branchName.Width = 70;
            // 
            // sID
            // 
            this.sID.HeaderText = "ID";
            this.sID.Name = "sID";
            this.sID.Visible = false;
            // 
            // _customerDetails
            // 
            this._customerDetails.Controls.Add(this.groupBox1);
            this._customerDetails.Location = new System.Drawing.Point(4, 33);
            this._customerDetails.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._customerDetails.Name = "_customerDetails";
            this._customerDetails.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._customerDetails.Size = new System.Drawing.Size(876, 413);
            this._customerDetails.TabIndex = 1;
            this._customerDetails.Text = "Customer Details";
            this._customerDetails.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgrdCustomer);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(847, 391);
            this.groupBox1.TabIndex = 119;
            this.groupBox1.TabStop = false;
            // 
            // dgrdCustomer
            // 
            this.dgrdCustomer.AllowUserToAddRows = false;
            this.dgrdCustomer.AllowUserToDeleteRows = false;
            this.dgrdCustomer.AllowUserToResizeRows = false;
            dataGridViewCellStyle36.BackColor = System.Drawing.Color.White;
            this.dgrdCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle36;
            this.dgrdCustomer.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle37.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle37.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle37.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle37.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle37.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle37.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdCustomer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle37;
            this.dgrdCustomer.ColumnHeadersHeight = 30;
            this.dgrdCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSno,
            this.customerName,
            this.targetValue,
            this.cID});
            this.dgrdCustomer.Cursor = System.Windows.Forms.Cursors.Default;
            this.dgrdCustomer.EnableHeadersVisualStyles = false;
            this.dgrdCustomer.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdCustomer.Location = new System.Drawing.Point(16, 24);
            this.dgrdCustomer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dgrdCustomer.Name = "dgrdCustomer";
            this.dgrdCustomer.RowHeadersVisible = false;
            dataGridViewCellStyle39.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdCustomer.RowsDefaultCellStyle = dataGridViewCellStyle39;
            this.dgrdCustomer.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdCustomer.RowTemplate.Height = 28;
            this.dgrdCustomer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdCustomer.Size = new System.Drawing.Size(812, 352);
            this.dgrdCustomer.TabIndex = 119;
            this.dgrdCustomer.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdCustomer_CellBeginEdit);
            this.dgrdCustomer.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgrdCustomer_EditingControlShowing);
            this.dgrdCustomer.Sorted += new System.EventHandler(this.dgrdCustomer_Sorted);
            this.dgrdCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdCustomer_KeyDown);
            // 
            // cSno
            // 
            this.cSno.HeaderText = "S.No.";
            this.cSno.Name = "cSno";
            this.cSno.Width = 70;
            // 
            // customerName
            // 
            this.customerName.HeaderText = "Customer Name";
            this.customerName.Name = "customerName";
            this.customerName.Width = 450;
            // 
            // targetValue
            // 
            dataGridViewCellStyle38.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle38.Format = "N2";
            this.targetValue.DefaultCellStyle = dataGridViewCellStyle38;
            this.targetValue.HeaderText = "Target Value";
            this.targetValue.Name = "targetValue";
            this.targetValue.Width = 200;
            // 
            // cID
            // 
            this.cID.HeaderText = "ID";
            this.cID.Name = "cID";
            this.cID.Visible = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.txtBranchCode);
            this.panel3.Controls.Add(this.label13);
            this.panel3.Controls.Add(this.btnDownload);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnSubmit);
            this.panel3.Controls.Add(this.txtSchemeName);
            this.panel3.Controls.Add(this.label1);
            this.panel3.ForeColor = System.Drawing.Color.White;
            this.panel3.Location = new System.Drawing.Point(22, 13);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(882, 44);
            this.panel3.TabIndex = 102;
            this.panel3.TabStop = true;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDownload.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDownload.ForeColor = System.Drawing.Color.White;
            this.btnDownload.Location = new System.Drawing.Point(708, 2);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(92, 33);
            this.btnDownload.TabIndex = 117;
            this.btnDownload.Text = "Do&wnload";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(623, 2);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 33);
            this.btnDelete.TabIndex = 115;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(799, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 33);
            this.btnCancel.TabIndex = 116;
            this.btnCancel.Text = "C&lose";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.tsbtnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSubmit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(542, 2);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(82, 33);
            this.btnSubmit.TabIndex = 105;
            this.btnSubmit.Text = "&Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtSchemeName
            // 
            this.txtSchemeName.BackColor = System.Drawing.Color.White;
            this.txtSchemeName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSchemeName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSchemeName.Location = new System.Drawing.Point(127, 7);
            this.txtSchemeName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtSchemeName.MaxLength = 40;
            this.txtSchemeName.Name = "txtSchemeName";
            this.txtSchemeName.ReadOnly = true;
            this.txtSchemeName.Size = new System.Drawing.Size(247, 23);
            this.txtSchemeName.TabIndex = 103;
            this.txtSchemeName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSchemeName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scheme Name :";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(7, 110);
            this.lblId.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(0, 16);
            this.lblId.TabIndex = 6;
            this.lblId.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle40.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle40.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle40.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle40.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle40;
            this.dataGridViewTextBoxColumn1.HeaderText = "          WEAVE  NAME";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 15;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 180;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "id";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // panHeader
            // 
            this.panHeader.BackColor = System.Drawing.Color.White;
            this.panHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panHeader.Controls.Add(this.lblNameHeader);
            this.panHeader.Location = new System.Drawing.Point(20, 16);
            this.panHeader.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(979, 47);
            this.panHeader.TabIndex = 98;
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.AutoSize = true;
            this.lblNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblNameHeader.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.lblNameHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNameHeader.Location = new System.Drawing.Point(369, 11);
            this.lblNameHeader.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(237, 21);
            this.lblNameHeader.TabIndex = 211;
            this.lblNameHeader.Text = "SCHEME DETAIL MASTER";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCreatedBy.LinkColor = System.Drawing.Color.White;
            this.lblCreatedBy.Location = new System.Drawing.Point(23, 64);
            this.lblCreatedBy.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(975, 18);
            this.lblCreatedBy.TabIndex = 508;
            this.lblCreatedBy.TabStop = true;
            this.lblCreatedBy.Text = "_";
            this.lblCreatedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.BackColor = System.Drawing.Color.White;
            this.txtBranchCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBranchCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBranchCode.Location = new System.Drawing.Point(447, 7);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.ReadOnly = true;
            this.txtBranchCode.Size = new System.Drawing.Size(86, 23);
            this.txtBranchCode.TabIndex = 104;
            this.txtBranchCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBranchCode_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(380, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 16);
            this.label13.TabIndex = 144;
            this.label13.Text = "Branch :";
            // 
            // SchemeDetailMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1020, 658);
            this.Controls.Add(this.lblCreatedBy);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "SchemeDetailMaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fa";
            this.Load += new System.EventHandler(this.TourMaster_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UnitMaster_KeyDown);
            this.panColor.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this._TourTab.ResumeLayout(false);
            this._supplierDetails.ResumeLayout(false);
            this.grpDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSupplier)).EndInit();
            this._customerDetails.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdCustomer)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtSchemeName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.LinkLabel lblCreatedBy;
        private System.Windows.Forms.TabControl _TourTab;
        private System.Windows.Forms.TabPage _supplierDetails;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.DataGridView dgrdSupplier;
        private System.Windows.Forms.TabPage _customerDetails;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgrdCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn cSno;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.Label lblNameHeader;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn sSno;
        private System.Windows.Forms.DataGridViewTextBoxColumn supplierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn disPer;
        private System.Windows.Forms.DataGridViewTextBoxColumn amtValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn sID;
        protected internal System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label label13;
    }
}

