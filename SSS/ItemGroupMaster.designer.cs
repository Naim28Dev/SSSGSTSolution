namespace SSS
{
    partial class ItemGroupMaster
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panColor = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDownloadMaster = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panSearch = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.txtTaxCategory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.stackPanel1 = new System.Windows.Forms.GroupBox();
            this.rdoSAC = new System.Windows.Forms.RadioButton();
            this.rdoHSN = new System.Windows.Forms.RadioButton();
            this.txtHSNCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.pangrid = new System.Windows.Forms.Panel();
            this.dgrdName = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.categoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parentGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hsnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taxCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.other = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saleCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorname = new System.Windows.Forms.ErrorProvider(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblGoodsPurchaseBook = new System.Windows.Forms.Label();
            this.lnkCheckHSN = new System.Windows.Forms.LinkLabel();
            this.panColor.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panSearch.SuspendLayout();
            this.panel5.SuspendLayout();
            this.stackPanel1.SuspendLayout();
            this.pangrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorname)).BeginInit();
            this.panHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panColor
            // 
            this.panColor.BackColor = System.Drawing.Color.White;
            this.panColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panColor.Controls.Add(this.panel3);
            this.panColor.Controls.Add(this.panSearch);
            this.panColor.Controls.Add(this.panel5);
            this.panColor.Controls.Add(this.pangrid);
            this.panColor.Location = new System.Drawing.Point(26, 88);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(946, 546);
            this.panColor.TabIndex = 96;
            this.panColor.TabStop = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnDownloadMaster);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Controls.Add(this.btnEdit);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnAdd);
            this.panel3.Location = new System.Drawing.Point(16, 443);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(483, 89);
            this.panel3.TabIndex = 109;
            this.panel3.TabStop = true;
            // 
            // btnDownloadMaster
            // 
            this.btnDownloadMaster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDownloadMaster.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDownloadMaster.ForeColor = System.Drawing.Color.White;
            this.btnDownloadMaster.Location = new System.Drawing.Point(17, 45);
            this.btnDownloadMaster.Name = "btnDownloadMaster";
            this.btnDownloadMaster.Size = new System.Drawing.Size(288, 33);
            this.btnDownloadMaster.TabIndex = 116;
            this.btnDownloadMaster.Text = "Download Merged Master";
            this.btnDownloadMaster.UseVisualStyleBackColor = false;
            this.btnDownloadMaster.Click += new System.EventHandler(this.btnDownloadMaster_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(240, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 33);
            this.btnSearch.TabIndex = 114;
            this.btnSearch.Text = "Sea&rch";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.tsbtnSearch_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(350, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(114, 33);
            this.btnDelete.TabIndex = 115;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEdit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(124, 10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(116, 33);
            this.btnEdit.TabIndex = 113;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(305, 45);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(159, 33);
            this.btnCancel.TabIndex = 117;
            this.btnCancel.Text = "C&lose";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.tsbtnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(17, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(107, 33);
            this.btnAdd.TabIndex = 112;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panSearch
            // 
            this.panSearch.BackColor = System.Drawing.Color.White;
            this.panSearch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panSearch.Controls.Add(this.txtSearch);
            this.panSearch.Controls.Add(this.label3);
            this.panSearch.Location = new System.Drawing.Point(522, 13);
            this.panSearch.Name = "panSearch";
            this.panSearch.Size = new System.Drawing.Size(398, 44);
            this.panSearch.TabIndex = 97;
            this.panSearch.TabStop = true;
            // 
            // txtSearch
            // 
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSearch.Location = new System.Drawing.Point(125, 10);
            this.txtSearch.MaxLength = 40;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(257, 23);
            this.txtSearch.TabIndex = 98;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(9, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Search Group :";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.lnkCheckHSN);
            this.panel5.Controls.Add(this.lblWarning);
            this.panel5.Controls.Add(this.txtTaxCategory);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.stackPanel1);
            this.panel5.Controls.Add(this.txtHSNCode);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.lblId);
            this.panel5.Controls.Add(this.txtGroupName);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.lblMsg);
            this.panel5.Location = new System.Drawing.Point(16, 14);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(485, 391);
            this.panel5.TabIndex = 101;
            this.panel5.TabStop = true;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblWarning.ForeColor = System.Drawing.Color.Maroon;
            this.lblWarning.Location = new System.Drawing.Point(179, 135);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(10, 14);
            this.lblWarning.TabIndex = 217;
            this.lblWarning.Text = " ";
            // 
            // txtTaxCategory
            // 
            this.txtTaxCategory.BackColor = System.Drawing.Color.White;
            this.txtTaxCategory.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxCategory.Font = new System.Drawing.Font("Arial", 10F);
            this.txtTaxCategory.Location = new System.Drawing.Point(137, 99);
            this.txtTaxCategory.MaxLength = 40;
            this.txtTaxCategory.Name = "txtTaxCategory";
            this.txtTaxCategory.ReadOnly = true;
            this.txtTaxCategory.Size = new System.Drawing.Size(244, 23);
            this.txtTaxCategory.TabIndex = 108;
            this.txtTaxCategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTaxCategory_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(35, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 216;
            this.label4.Text = "Tax Category :";
            // 
            // stackPanel1
            // 
            this.stackPanel1.Controls.Add(this.rdoSAC);
            this.stackPanel1.Controls.Add(this.rdoHSN);
            this.stackPanel1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stackPanel1.Location = new System.Drawing.Point(10, 53);
            this.stackPanel1.Name = "stackPanel1";
            this.stackPanel1.Size = new System.Drawing.Size(112, 33);
            this.stackPanel1.TabIndex = 104;
            this.stackPanel1.TabStop = false;
            // 
            // rdoSAC
            // 
            this.rdoSAC.AutoSize = true;
            this.rdoSAC.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSAC.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoSAC.Location = new System.Drawing.Point(56, 11);
            this.rdoSAC.Name = "rdoSAC";
            this.rdoSAC.Size = new System.Drawing.Size(48, 18);
            this.rdoSAC.TabIndex = 106;
            this.rdoSAC.Text = "SAC";
            this.rdoSAC.UseVisualStyleBackColor = true;
            // 
            // rdoHSN
            // 
            this.rdoHSN.AutoSize = true;
            this.rdoHSN.Checked = true;
            this.rdoHSN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoHSN.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoHSN.Location = new System.Drawing.Point(9, 11);
            this.rdoHSN.Name = "rdoHSN";
            this.rdoHSN.Size = new System.Drawing.Size(46, 18);
            this.rdoHSN.TabIndex = 105;
            this.rdoHSN.TabStop = true;
            this.rdoHSN.Text = "HSN";
            this.rdoHSN.UseVisualStyleBackColor = true;
            // 
            // txtHSNCode
            // 
            this.txtHSNCode.BackColor = System.Drawing.Color.White;
            this.txtHSNCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtHSNCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtHSNCode.Location = new System.Drawing.Point(137, 61);
            this.txtHSNCode.MaxLength = 8;
            this.txtHSNCode.Name = "txtHSNCode";
            this.txtHSNCode.ReadOnly = true;
            this.txtHSNCode.Size = new System.Drawing.Size(244, 23);
            this.txtHSNCode.TabIndex = 107;
            this.txtHSNCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHSNCode_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(123, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 16);
            this.label10.TabIndex = 215;
            this.label10.Text = ":";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(384, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 18);
            this.label6.TabIndex = 208;
            this.label6.Text = "*";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(5, 89);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(0, 16);
            this.lblId.TabIndex = 6;
            this.lblId.Visible = false;
            // 
            // txtGroupName
            // 
            this.txtGroupName.BackColor = System.Drawing.Color.White;
            this.txtGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtGroupName.Location = new System.Drawing.Point(137, 26);
            this.txtGroupName.MaxLength = 40;
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.ReadOnly = true;
            this.txtGroupName.Size = new System.Drawing.Size(244, 23);
            this.txtGroupName.TabIndex = 102;
            this.txtGroupName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGroupName_KeyPress);
            this.txtGroupName.Leave += new System.EventHandler(this.txtGroupName_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(40, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Group Name :";
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.Maroon;
            this.lblMsg.Location = new System.Drawing.Point(140, 6);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 14);
            this.lblMsg.TabIndex = 2;
            this.lblMsg.Text = " ";
            // 
            // pangrid
            // 
            this.pangrid.BackColor = System.Drawing.Color.White;
            this.pangrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pangrid.Controls.Add(this.dgrdName);
            this.pangrid.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.pangrid.Location = new System.Drawing.Point(522, 65);
            this.pangrid.Name = "pangrid";
            this.pangrid.Size = new System.Drawing.Size(399, 466);
            this.pangrid.TabIndex = 99;
            // 
            // dgrdName
            // 
            this.dgrdName.AllowUserToAddRows = false;
            this.dgrdName.AllowUserToDeleteRows = false;
            this.dgrdName.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.dgrdName.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdName.BackgroundColor = System.Drawing.Color.White;
            this.dgrdName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgrdName.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdName.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdName.ColumnHeadersHeight = 30;
            this.dgrdName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.groupName,
            this.categoryName,
            this.parentGroup,
            this.hsnCode,
            this.taxCategory,
            this.other,
            this.saleCount});
            this.dgrdName.EnableHeadersVisualStyles = false;
            this.dgrdName.GridColor = System.Drawing.Color.Gray;
            this.dgrdName.Location = new System.Drawing.Point(15, 16);
            this.dgrdName.MultiSelect = false;
            this.dgrdName.Name = "dgrdName";
            this.dgrdName.ReadOnly = true;
            this.dgrdName.RowHeadersVisible = false;
            this.dgrdName.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdName.RowTemplate.Height = 25;
            this.dgrdName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdName.Size = new System.Drawing.Size(365, 428);
            this.dgrdName.TabIndex = 100;
            this.dgrdName.SelectionChanged += new System.EventHandler(this.dgrdName_SelectionChanged);
            this.dgrdName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgrdName_KeyPress);
            this.dgrdName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgrdName_KeyUp);
            this.dgrdName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgrdName_MouseClick);
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // groupName
            // 
            this.groupName.HeaderText = "Group Name";
            this.groupName.Name = "groupName";
            this.groupName.ReadOnly = true;
            this.groupName.Width = 180;
            // 
            // categoryName
            // 
            this.categoryName.HeaderText = "Category Name";
            this.categoryName.Name = "categoryName";
            this.categoryName.ReadOnly = true;
            this.categoryName.Visible = false;
            this.categoryName.Width = 120;
            // 
            // parentGroup
            // 
            this.parentGroup.HeaderText = "Group Name";
            this.parentGroup.Name = "parentGroup";
            this.parentGroup.ReadOnly = true;
            this.parentGroup.Visible = false;
            this.parentGroup.Width = 120;
            // 
            // hsnCode
            // 
            this.hsnCode.HeaderText = "HSN Code";
            this.hsnCode.Name = "hsnCode";
            this.hsnCode.ReadOnly = true;
            this.hsnCode.Width = 70;
            // 
            // taxCategory
            // 
            this.taxCategory.HeaderText = "Tax Category";
            this.taxCategory.Name = "taxCategory";
            this.taxCategory.ReadOnly = true;
            this.taxCategory.Width = 90;
            // 
            // other
            // 
            this.other.HeaderText = "Other";
            this.other.Name = "other";
            this.other.ReadOnly = true;
            this.other.Visible = false;
            // 
            // saleCount
            // 
            this.saleCount.HeaderText = "Count";
            this.saleCount.Name = "saleCount";
            this.saleCount.ReadOnly = true;
            this.saleCount.Visible = false;
            // 
            // errorname
            // 
            this.errorname.ContainerControl = this;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle6;
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
            this.panHeader.Controls.Add(this.lblGoodsPurchaseBook);
            this.panHeader.Location = new System.Drawing.Point(26, 22);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(945, 48);
            this.panHeader.TabIndex = 101;
            // 
            // lblGoodsPurchaseBook
            // 
            this.lblGoodsPurchaseBook.AutoSize = true;
            this.lblGoodsPurchaseBook.BackColor = System.Drawing.Color.Transparent;
            this.lblGoodsPurchaseBook.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblGoodsPurchaseBook.ForeColor = System.Drawing.Color.Black;
            this.lblGoodsPurchaseBook.Location = new System.Drawing.Point(359, 13);
            this.lblGoodsPurchaseBook.Name = "lblGoodsPurchaseBook";
            this.lblGoodsPurchaseBook.Size = new System.Drawing.Size(222, 19);
            this.lblGoodsPurchaseBook.TabIndex = 7;
            this.lblGoodsPurchaseBook.Text = "HSN/SAC GROUP MASTER";
            // 
            // lnkCheckHSN
            // 
            this.lnkCheckHSN.AutoSize = true;
            this.lnkCheckHSN.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lnkCheckHSN.LinkColor = System.Drawing.Color.DodgerBlue;
            this.lnkCheckHSN.Location = new System.Drawing.Point(381, 65);
            this.lnkCheckHSN.Name = "lnkCheckHSN";
            this.lnkCheckHSN.Size = new System.Drawing.Size(98, 14);
            this.lnkCheckHSN.TabIndex = 218;
            this.lnkCheckHSN.TabStop = true;
            this.lnkCheckHSN.Text = "Check HSN Code";
            this.lnkCheckHSN.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCheckHSN_LinkClicked);
            // 
            // ItemGroupMaster
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ItemGroupMaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Group Master";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemGroupMaster_FormClosing);
            this.Load += new System.EventHandler(this.GroupMaster_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.panColor.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panSearch.ResumeLayout(false);
            this.panSearch.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.stackPanel1.ResumeLayout(false);
            this.stackPanel1.PerformLayout();
            this.pangrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorname)).EndInit();
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.Panel panSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pangrid;
        private System.Windows.Forms.DataGridView dgrdName;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.ErrorProvider errorname;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtHSNCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox stackPanel1;
        private System.Windows.Forms.RadioButton rdoSAC;
        private System.Windows.Forms.RadioButton rdoHSN;
        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.TextBox txtTaxCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Button btnDownloadMaster;
        private System.Windows.Forms.Label lblGoodsPurchaseBook;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn parentGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn hsnCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn taxCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn other;
        private System.Windows.Forms.DataGridViewTextBoxColumn saleCount;
        private System.Windows.Forms.LinkLabel lnkCheckHSN;
    }
}

