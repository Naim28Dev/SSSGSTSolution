namespace SSS
{
    partial class TemplateSetting
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblID = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.panColor = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtHeaderRow = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSheetNo = new System.Windows.Forms.TextBox();
            this.txtTemplateName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.grpExcel = new System.Windows.Forms.GroupBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lstColumn = new System.Windows.Forms.ListBox();
            this.lstExcel = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgrdName = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lblNameHeader = new System.Windows.Forms.Label();
            this.srNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.systemColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reqStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.chkExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panHeader.SuspendLayout();
            this.panColor.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpExcel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panHeader
            // 
            this.panHeader.BackColor = System.Drawing.Color.White;
            this.panHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panHeader.Controls.Add(this.lblNameHeader);
            this.panHeader.Controls.Add(this.lblID);
            this.panHeader.Controls.Add(this.lblMsg);
            this.panHeader.Location = new System.Drawing.Point(17, 16);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(1020, 44);
            this.panHeader.TabIndex = 101;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblID.ForeColor = System.Drawing.Color.Maroon;
            this.lblID.Location = new System.Drawing.Point(962, 23);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(10, 14);
            this.lblID.TabIndex = 209;
            this.lblID.Text = " ";
            this.lblID.Visible = false;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.Maroon;
            this.lblMsg.Location = new System.Drawing.Point(9, 23);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 14);
            this.lblMsg.TabIndex = 208;
            this.lblMsg.Text = " ";
            // 
            // panColor
            // 
            this.panColor.BackColor = System.Drawing.Color.White;
            this.panColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panColor.Controls.Add(this.groupBox1);
            this.panColor.Controls.Add(this.grpExcel);
            this.panColor.Controls.Add(this.groupBox3);
            this.panColor.Controls.Add(this.groupBox2);
            this.panColor.Location = new System.Drawing.Point(17, 73);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(1019, 566);
            this.panColor.TabIndex = 100;
            this.panColor.TabStop = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.txtHeaderRow);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtSheetNo);
            this.groupBox1.Controls.Add(this.txtTemplateName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtFilePath);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(7, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1002, 47);
            this.groupBox1.TabIndex = 101;
            this.groupBox1.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnOK.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(938, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 30);
            this.btnOK.TabIndex = 107;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtHeaderRow
            // 
            this.txtHeaderRow.BackColor = System.Drawing.Color.White;
            this.txtHeaderRow.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtHeaderRow.Font = new System.Drawing.Font("Arial", 10F);
            this.txtHeaderRow.Location = new System.Drawing.Point(902, 15);
            this.txtHeaderRow.MaxLength = 40;
            this.txtHeaderRow.Name = "txtHeaderRow";
            this.txtHeaderRow.ReadOnly = true;
            this.txtHeaderRow.Size = new System.Drawing.Size(34, 23);
            this.txtHeaderRow.TabIndex = 106;
            this.txtHeaderRow.Text = "1";
            this.txtHeaderRow.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtHeaderRow.Enter += new System.EventHandler(this.txtHeaderRow_Enter);
            this.txtHeaderRow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSheetNo_KeyPress);
            this.txtHeaderRow.Leave += new System.EventHandler(this.txtHeaderRow_Leave);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(807, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(94, 16);
            this.label13.TabIndex = 130;
            this.label13.Text = "Header Row :";
            // 
            // txtSheetNo
            // 
            this.txtSheetNo.BackColor = System.Drawing.Color.White;
            this.txtSheetNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSheetNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSheetNo.Location = new System.Drawing.Point(770, 16);
            this.txtSheetNo.MaxLength = 40;
            this.txtSheetNo.Name = "txtSheetNo";
            this.txtSheetNo.ReadOnly = true;
            this.txtSheetNo.Size = new System.Drawing.Size(33, 23);
            this.txtSheetNo.TabIndex = 105;
            this.txtSheetNo.Text = "1";
            this.txtSheetNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSheetNo.Enter += new System.EventHandler(this.txtSheetNo_Enter);
            this.txtSheetNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSheetNo_KeyPress);
            this.txtSheetNo.Leave += new System.EventHandler(this.txtSheetNo_Leave);
            // 
            // txtTemplateName
            // 
            this.txtTemplateName.BackColor = System.Drawing.Color.White;
            this.txtTemplateName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTemplateName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtTemplateName.Location = new System.Drawing.Point(79, 16);
            this.txtTemplateName.MaxLength = 40;
            this.txtTemplateName.Name = "txtTemplateName";
            this.txtTemplateName.ReadOnly = true;
            this.txtTemplateName.Size = new System.Drawing.Size(146, 23);
            this.txtTemplateName.TabIndex = 102;
            this.txtTemplateName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTemplateName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(5, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 103;
            this.label1.Text = "Template :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(695, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 16);
            this.label12.TabIndex = 128;
            this.label12.Text = "Sheet No :";
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnBrowse.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(621, 13);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(70, 27);
            this.btnBrowse.TabIndex = 104;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFilePath.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFilePath.Location = new System.Drawing.Point(281, 15);
            this.txtFilePath.MaxLength = 40;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(339, 23);
            this.txtFilePath.TabIndex = 103;
            this.txtFilePath.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(226, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 16);
            this.label11.TabIndex = 124;
            this.label11.Text = "Excel  :";
            // 
            // grpExcel
            // 
            this.grpExcel.Controls.Add(this.btnLeft);
            this.grpExcel.Controls.Add(this.btnRight);
            this.grpExcel.Controls.Add(this.label15);
            this.grpExcel.Controls.Add(this.label14);
            this.grpExcel.Controls.Add(this.lstColumn);
            this.grpExcel.Controls.Add(this.lstExcel);
            this.grpExcel.Location = new System.Drawing.Point(10, 57);
            this.grpExcel.Name = "grpExcel";
            this.grpExcel.Size = new System.Drawing.Size(373, 494);
            this.grpExcel.TabIndex = 108;
            this.grpExcel.TabStop = false;
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLeft.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold);
            this.btnLeft.ForeColor = System.Drawing.Color.White;
            this.btnLeft.Location = new System.Drawing.Point(291, 248);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(70, 34);
            this.btnLeft.TabIndex = 112;
            this.btnLeft.Text = "<<";
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRight.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold);
            this.btnRight.ForeColor = System.Drawing.Color.White;
            this.btnRight.Location = new System.Drawing.Point(290, 214);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(70, 34);
            this.btnRight.TabIndex = 111;
            this.btnRight.Text = ">>";
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label15.Location = new System.Drawing.Point(79, 245);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(148, 16);
            this.label15.TabIndex = 130;
            this.label15.Text = "System Column Name";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label14.Location = new System.Drawing.Point(86, 11);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(137, 16);
            this.label14.TabIndex = 129;
            this.label14.Text = "Excel Column Name";
            // 
            // lstColumn
            // 
            this.lstColumn.Font = new System.Drawing.Font("Arial", 9.75F);
            this.lstColumn.FormattingEnabled = true;
            this.lstColumn.ItemHeight = 16;
            this.lstColumn.Location = new System.Drawing.Point(18, 265);
            this.lstColumn.Name = "lstColumn";
            this.lstColumn.Size = new System.Drawing.Size(269, 212);
            this.lstColumn.TabIndex = 110;
            // 
            // lstExcel
            // 
            this.lstExcel.Font = new System.Drawing.Font("Arial", 9.75F);
            this.lstExcel.FormattingEnabled = true;
            this.lstExcel.ItemHeight = 16;
            this.lstExcel.Location = new System.Drawing.Point(18, 28);
            this.lstExcel.Name = "lstExcel";
            this.lstExcel.Size = new System.Drawing.Size(269, 212);
            this.lstExcel.TabIndex = 109;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgrdName);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(391, 57);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(619, 448);
            this.groupBox3.TabIndex = 113;
            this.groupBox3.TabStop = false;
            // 
            // dgrdName
            // 
            this.dgrdName.AllowUserToAddRows = false;
            this.dgrdName.AllowUserToDeleteRows = false;
            this.dgrdName.AllowUserToResizeRows = false;
            this.dgrdName.BackgroundColor = System.Drawing.Color.White;
            this.dgrdName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgrdName.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdName.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdName.ColumnHeadersHeight = 28;
            this.dgrdName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.srNo,
            this.excelColumn,
            this.columnType,
            this.systemColumn,
            this.reqStatus,
            this.chkExists});
            this.dgrdName.EnableHeadersVisualStyles = false;
            this.dgrdName.Location = new System.Drawing.Point(14, 21);
            this.dgrdName.MultiSelect = false;
            this.dgrdName.Name = "dgrdName";
            this.dgrdName.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdName.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrdName.RowTemplate.Height = 25;
            this.dgrdName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdName.Size = new System.Drawing.Size(592, 415);
            this.dgrdName.TabIndex = 114;
            this.dgrdName.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdName_CellBeginEdit);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Location = new System.Drawing.Point(435, 506);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(571, 52);
            this.groupBox2.TabIndex = 115;
            this.groupBox2.TabStop = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(343, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(103, 33);
            this.btnSearch.TabIndex = 119;
            this.btnSearch.Text = "Sea&rch";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(449, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 33);
            this.btnCancel.TabIndex = 120;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(248, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 33);
            this.btnDelete.TabIndex = 118;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(51, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 33);
            this.btnAdd.TabIndex = 116;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEdit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(151, 12);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(97, 33);
            this.btnEdit.TabIndex = 117;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.AutoSize = true;
            this.lblNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblNameHeader.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.lblNameHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNameHeader.Location = new System.Drawing.Point(424, 11);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(168, 19);
            this.lblNameHeader.TabIndex = 211;
            this.lblNameHeader.Text = "TEMPLATE MASTER";
            // 
            // srNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.srNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.srNo.HeaderText = "S.No.";
            this.srNo.Name = "srNo";
            this.srNo.Width = 45;
            // 
            // excelColumn
            // 
            this.excelColumn.HeaderText = "Excel Column";
            this.excelColumn.Name = "excelColumn";
            this.excelColumn.Width = 180;
            // 
            // columnType
            // 
            this.columnType.HeaderText = "Column";
            this.columnType.Name = "columnType";
            this.columnType.Width = 60;
            // 
            // systemColumn
            // 
            this.systemColumn.HeaderText = "System Column";
            this.systemColumn.Name = "systemColumn";
            this.systemColumn.Width = 190;
            // 
            // reqStatus
            // 
            this.reqStatus.HeaderText = "Req.";
            this.reqStatus.Name = "reqStatus";
            this.reqStatus.Width = 35;
            // 
            // chkExists
            // 
            this.chkExists.HeaderText = "M.Check";
            this.chkExists.Name = "chkExists";
            this.chkExists.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.chkExists.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.chkExists.Width = 55;
            // 
            // TemplateSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TemplateSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales Template";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SalesTemplate_KeyDown);
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.panColor.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpExcel.ResumeLayout(false);
            this.grpExcel.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.TextBox txtTemplateName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtHeaderRow;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtSheetNo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpExcel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListBox lstColumn;
        private System.Windows.Forms.ListBox lstExcel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.DataGridView dgrdName;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label lblNameHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn srNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn excelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn systemColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn reqStatus;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkExists;
    }
}