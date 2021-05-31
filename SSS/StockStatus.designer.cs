namespace SSS
{
    partial class StockStatus
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chkShowMRPAlso = new System.Windows.Forms.CheckBox();
            this.chkShowZeroStockItems = new System.Windows.Forms.CheckBox();
            this.chkShowParentGroup = new System.Windows.Forms.CheckBox();
            this.chkShowValueOfItem = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtItemShownBy = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.txtMaterialCenter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.LinkLabel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblNameHeader = new System.Windows.Forms.Label();
            this.pangrid = new System.Windows.Forms.Panel();
            this.dgrdName = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaterialCenter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemShownBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReportDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowZeroStockItems = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowParentGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowValueOfItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowMRPAlso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InsertStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorname = new System.Windows.Forms.ErrorProvider(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panColor = new System.Windows.Forms.Panel();
            this.txtReportDate = new System.Windows.Forms.MaskedTextBox();
            this.panel5.SuspendLayout();
            this.panHeader.SuspendLayout();
            this.pangrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorname)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 6;
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(11, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Search Material :";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.txtReportDate);
            this.panel5.Controls.Add(this.txtRemark);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.chkShowMRPAlso);
            this.panel5.Controls.Add(this.chkShowZeroStockItems);
            this.panel5.Controls.Add(this.chkShowParentGroup);
            this.panel5.Controls.Add(this.chkShowValueOfItem);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.txtItemShownBy);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.lblId);
            this.panel5.Controls.Add(this.txtMaterialCenter);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.lblMsg);
            this.panel5.Location = new System.Drawing.Point(17, 13);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(526, 368);
            this.panel5.TabIndex = 98;
            this.panel5.TabStop = true;
            // 
            // txtRemark
            // 
            this.txtRemark.BackColor = System.Drawing.Color.White;
            this.txtRemark.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRemark.Font = new System.Drawing.Font("Arial", 10F);
            this.txtRemark.Location = new System.Drawing.Point(198, 286);
            this.txtRemark.MaxLength = 40;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ReadOnly = true;
            this.txtRemark.Size = new System.Drawing.Size(244, 23);
            this.txtRemark.TabIndex = 205;
            this.txtRemark.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxes_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(108, 289);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 16);
            this.label12.TabIndex = 223;
            this.label12.Text = "Remark :";
            // 
            // chkShowMRPAlso
            // 
            this.chkShowMRPAlso.AutoSize = true;
            this.chkShowMRPAlso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowMRPAlso.Location = new System.Drawing.Point(198, 247);
            this.chkShowMRPAlso.Name = "chkShowMRPAlso";
            this.chkShowMRPAlso.Size = new System.Drawing.Size(47, 17);
            this.chkShowMRPAlso.TabIndex = 204;
            this.chkShowMRPAlso.Text = "Yes";
            this.chkShowMRPAlso.UseVisualStyleBackColor = true;
            // 
            // chkShowZeroStockItems
            // 
            this.chkShowZeroStockItems.AutoSize = true;
            this.chkShowZeroStockItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowZeroStockItems.Location = new System.Drawing.Point(199, 150);
            this.chkShowZeroStockItems.Name = "chkShowZeroStockItems";
            this.chkShowZeroStockItems.Size = new System.Drawing.Size(47, 17);
            this.chkShowZeroStockItems.TabIndex = 201;
            this.chkShowZeroStockItems.Text = "Yes";
            this.chkShowZeroStockItems.UseVisualStyleBackColor = true;
            // 
            // chkShowParentGroup
            // 
            this.chkShowParentGroup.AutoSize = true;
            this.chkShowParentGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowParentGroup.Location = new System.Drawing.Point(198, 180);
            this.chkShowParentGroup.Name = "chkShowParentGroup";
            this.chkShowParentGroup.Size = new System.Drawing.Size(47, 17);
            this.chkShowParentGroup.TabIndex = 202;
            this.chkShowParentGroup.Text = "Yes";
            this.chkShowParentGroup.UseVisualStyleBackColor = true;
            // 
            // chkShowValueOfItem
            // 
            this.chkShowValueOfItem.AutoSize = true;
            this.chkShowValueOfItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowValueOfItem.Location = new System.Drawing.Point(198, 214);
            this.chkShowValueOfItem.Name = "chkShowValueOfItem";
            this.chkShowValueOfItem.Size = new System.Drawing.Size(47, 17);
            this.chkShowValueOfItem.TabIndex = 203;
            this.chkShowValueOfItem.Text = "Yes";
            this.chkShowValueOfItem.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(58, 246);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 16);
            this.label11.TabIndex = 218;
            this.label11.Text = "Show MRP Also :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(28, 213);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(146, 16);
            this.label10.TabIndex = 216;
            this.label10.Text = "Show Value of Items :";
            // 
            // txtItemShownBy
            // 
            this.txtItemShownBy.BackColor = System.Drawing.Color.White;
            this.txtItemShownBy.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtItemShownBy.Font = new System.Drawing.Font("Arial", 10F);
            this.txtItemShownBy.Location = new System.Drawing.Point(198, 105);
            this.txtItemShownBy.MaxLength = 40;
            this.txtItemShownBy.Name = "txtItemShownBy";
            this.txtItemShownBy.ReadOnly = true;
            this.txtItemShownBy.Size = new System.Drawing.Size(244, 23);
            this.txtItemShownBy.TabIndex = 200;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(26, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 16);
            this.label8.TabIndex = 214;
            this.label8.Text = "Item to be Shown By :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(33, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 16);
            this.label4.TabIndex = 212;
            this.label4.Text = "Show Parent Group :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(13, 150);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(161, 16);
            this.label9.TabIndex = 210;
            this.label9.Text = "Show Zero Stock Items :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(449, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 18);
            this.label6.TabIndex = 208;
            this.label6.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(83, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Report Date :";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(5, 89);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(0, 13);
            this.lblId.TabIndex = 6;
            this.lblId.Visible = false;
            // 
            // txtMaterialCenter
            // 
            this.txtMaterialCenter.BackColor = System.Drawing.Color.White;
            this.txtMaterialCenter.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaterialCenter.Font = new System.Drawing.Font("Arial", 10F);
            this.txtMaterialCenter.Location = new System.Drawing.Point(198, 39);
            this.txtMaterialCenter.MaxLength = 40;
            this.txtMaterialCenter.Name = "txtMaterialCenter";
            this.txtMaterialCenter.ReadOnly = true;
            this.txtMaterialCenter.Size = new System.Drawing.Size(244, 23);
            this.txtMaterialCenter.TabIndex = 97;
            this.txtMaterialCenter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMaterialCenter_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(60, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Material Center :";
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.Maroon;
            this.lblMsg.Location = new System.Drawing.Point(201, 11);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 14);
            this.lblMsg.TabIndex = 2;
            this.lblMsg.Text = " ";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCreatedBy.LinkColor = System.Drawing.Color.White;
            this.lblCreatedBy.Location = new System.Drawing.Point(16, 71);
            this.lblCreatedBy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(908, 18);
            this.lblCreatedBy.TabIndex = 512;
            this.lblCreatedBy.TabStop = true;
            this.lblCreatedBy.Text = "_";
            this.lblCreatedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCreatedBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCreatedBy_LinkClicked);
            // 
            // txtSearch
            // 
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSearch.Location = new System.Drawing.Point(140, 12);
            this.txtSearch.MaxLength = 40;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(218, 23);
            this.txtSearch.TabIndex = 109;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // panHeader
            // 
            this.panHeader.BackColor = System.Drawing.Color.White;
            this.panHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panHeader.Controls.Add(this.lblNameHeader);
            this.panHeader.Location = new System.Drawing.Point(17, 16);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(943, 52);
            this.panHeader.TabIndex = 511;
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.AutoSize = true;
            this.lblNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblNameHeader.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.lblNameHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNameHeader.Location = new System.Drawing.Point(412, 14);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(148, 21);
            this.lblNameHeader.TabIndex = 205;
            this.lblNameHeader.Text = "STOCK STATUS";
            this.lblNameHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pangrid
            // 
            this.pangrid.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pangrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pangrid.Controls.Add(this.dgrdName);
            this.pangrid.Location = new System.Drawing.Point(554, 67);
            this.pangrid.Name = "pangrid";
            this.pangrid.Size = new System.Drawing.Size(368, 314);
            this.pangrid.TabIndex = 110;
            // 
            // dgrdName
            // 
            this.dgrdName.AllowUserToAddRows = false;
            this.dgrdName.AllowUserToDeleteRows = false;
            this.dgrdName.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.dgrdName.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdName.BackgroundColor = System.Drawing.Color.White;
            this.dgrdName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgrdName.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdName.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdName.ColumnHeadersHeight = 30;
            this.dgrdName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.MaterialCenter,
            this.ItemShownBy,
            this.Remark,
            this.ReportDate,
            this.ShowZeroStockItems,
            this.ShowParentGroup,
            this.ShowValueOfItem,
            this.ShowMRPAlso,
            this.CreatedBy,
            this.UpdatedBy,
            this.InsertStatus,
            this.UpdateStatus});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrdName.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdName.EnableHeadersVisualStyles = false;
            this.dgrdName.Location = new System.Drawing.Point(3, 3);
            this.dgrdName.MultiSelect = false;
            this.dgrdName.Name = "dgrdName";
            this.dgrdName.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdName.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdName.RowHeadersVisible = false;
            this.dgrdName.RowTemplate.Height = 25;
            this.dgrdName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdName.Size = new System.Drawing.Size(358, 304);
            this.dgrdName.TabIndex = 111;
            this.dgrdName.SelectionChanged += new System.EventHandler(this.dgrdName_SelectionChanged);
            this.dgrdName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgrdName_KeyPress);
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // MaterialCenter
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.MaterialCenter.DefaultCellStyle = dataGridViewCellStyle3;
            this.MaterialCenter.HeaderText = "MATERIAL CENTER";
            this.MaterialCenter.MinimumWidth = 15;
            this.MaterialCenter.Name = "MaterialCenter";
            this.MaterialCenter.ReadOnly = true;
            this.MaterialCenter.Width = 140;
            // 
            // ItemShownBy
            // 
            this.ItemShownBy.HeaderText = "ITEM SHOWN BY";
            this.ItemShownBy.Name = "ItemShownBy";
            this.ItemShownBy.ReadOnly = true;
            this.ItemShownBy.Width = 120;
            // 
            // Remark
            // 
            this.Remark.HeaderText = "REMARK";
            this.Remark.Name = "Remark";
            this.Remark.ReadOnly = true;
            // 
            // ReportDate
            // 
            this.ReportDate.HeaderText = "REPORT DATE";
            this.ReportDate.Name = "ReportDate";
            this.ReportDate.ReadOnly = true;
            this.ReportDate.Visible = false;
            this.ReportDate.Width = 120;
            // 
            // ShowZeroStockItems
            // 
            this.ShowZeroStockItems.HeaderText = "ShowZeroStockItems";
            this.ShowZeroStockItems.Name = "ShowZeroStockItems";
            this.ShowZeroStockItems.ReadOnly = true;
            this.ShowZeroStockItems.Visible = false;
            // 
            // ShowParentGroup
            // 
            this.ShowParentGroup.HeaderText = "ShowParentGroup";
            this.ShowParentGroup.Name = "ShowParentGroup";
            this.ShowParentGroup.ReadOnly = true;
            this.ShowParentGroup.Visible = false;
            // 
            // ShowValueOfItem
            // 
            this.ShowValueOfItem.HeaderText = "ShowValueOfItem";
            this.ShowValueOfItem.Name = "ShowValueOfItem";
            this.ShowValueOfItem.ReadOnly = true;
            this.ShowValueOfItem.Visible = false;
            // 
            // ShowMRPAlso
            // 
            this.ShowMRPAlso.HeaderText = "ShowMRPAlso";
            this.ShowMRPAlso.Name = "ShowMRPAlso";
            this.ShowMRPAlso.ReadOnly = true;
            this.ShowMRPAlso.Visible = false;
            // 
            // CreatedBy
            // 
            this.CreatedBy.HeaderText = "CreatedBy";
            this.CreatedBy.Name = "CreatedBy";
            this.CreatedBy.ReadOnly = true;
            this.CreatedBy.Visible = false;
            // 
            // UpdatedBy
            // 
            this.UpdatedBy.HeaderText = "UpdatedBy";
            this.UpdatedBy.Name = "UpdatedBy";
            this.UpdatedBy.ReadOnly = true;
            this.UpdatedBy.Visible = false;
            // 
            // InsertStatus
            // 
            this.InsertStatus.HeaderText = "InsertStatus";
            this.InsertStatus.Name = "InsertStatus";
            this.InsertStatus.ReadOnly = true;
            this.InsertStatus.Visible = false;
            // 
            // UpdateStatus
            // 
            this.UpdateStatus.HeaderText = "UpdateStatus";
            this.UpdateStatus.Name = "UpdateStatus";
            this.UpdateStatus.ReadOnly = true;
            this.UpdateStatus.Visible = false;
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
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtSearch);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(554, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(368, 48);
            this.panel2.TabIndex = 108;
            this.panel2.TabStop = true;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(219, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(97, 35);
            this.btnSearch.TabIndex = 105;
            this.btnSearch.Text = "Sea&rch";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(317, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(97, 35);
            this.btnDelete.TabIndex = 106;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEdit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(114, 6);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(104, 35);
            this.btnEdit.TabIndex = 104;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(417, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 35);
            this.btnCancel.TabIndex = 107;
            this.btnCancel.Text = "Ca&ncel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(18, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 35);
            this.btnAdd.TabIndex = 103;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Controls.Add(this.btnEdit);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnAdd);
            this.panel3.Location = new System.Drawing.Point(13, 387);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(909, 51);
            this.panel3.TabIndex = 102;
            this.panel3.TabStop = true;
            // 
            // panColor
            // 
            this.panColor.BackColor = System.Drawing.Color.White;
            this.panColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panColor.Controls.Add(this.panel3);
            this.panColor.Controls.Add(this.panel2);
            this.panColor.Controls.Add(this.panel5);
            this.panColor.Controls.Add(this.pangrid);
            this.panColor.Location = new System.Drawing.Point(19, 92);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(940, 450);
            this.panColor.TabIndex = 510;
            this.panColor.TabStop = true;
            // 
            // txtReportDate
            // 
            this.txtReportDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtReportDate.Location = new System.Drawing.Point(198, 72);
            this.txtReportDate.Mask = "00/00/0000";
            this.txtReportDate.Name = "txtReportDate";
            this.txtReportDate.Size = new System.Drawing.Size(244, 23);
            this.txtReportDate.TabIndex = 98;
            this.txtReportDate.Leave += new System.EventHandler(this.txtReportDate_Leave);
            // 
            // StockStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(978, 548);
            this.Controls.Add(this.lblCreatedBy);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "StockStatus";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StockStatus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StockStatus_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StockStatus_KeyDown);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.pangrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorname)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panColor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.CheckBox chkShowValueOfItem;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtItemShownBy;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtMaterialCenter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.LinkLabel lblCreatedBy;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.Label lblNameHeader;
        private System.Windows.Forms.Panel pangrid;
        private System.Windows.Forms.DataGridView dgrdName;
        private System.Windows.Forms.ErrorProvider errorname;
        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.CheckBox chkShowZeroStockItems;
        private System.Windows.Forms.CheckBox chkShowParentGroup;
        private System.Windows.Forms.CheckBox chkShowMRPAlso;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaterialCenter;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemShownBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowZeroStockItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowParentGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowValueOfItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShowMRPAlso;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn InsertStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdateStatus;
        private System.Windows.Forms.MaskedTextBox txtReportDate;
    }
}