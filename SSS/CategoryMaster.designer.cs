﻿namespace SSS
{
    partial class CategoryMaster
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panColor = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtDepeciationPer = new System.Windows.Forms.TextBox();
            this.lblDepreciation = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.pangrid = new System.Windows.Forms.Panel();
            this.dgrdName = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.categoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depreciationPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorname = new System.Windows.Forms.ErrorProvider(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblNameHeader = new System.Windows.Forms.Label();
            this.panColor.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
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
            this.panColor.Controls.Add(this.panel2);
            this.panColor.Controls.Add(this.panel5);
            this.panColor.Controls.Add(this.pangrid);
            this.panColor.Location = new System.Drawing.Point(17, 94);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(960, 539);
            this.panColor.TabIndex = 93;
            this.panColor.TabStop = true;
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
            this.panel3.Location = new System.Drawing.Point(17, 460);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(530, 56);
            this.panel3.TabIndex = 102;
            this.panel3.TabStop = true;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(219, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(97, 35);
            this.btnSearch.TabIndex = 105;
            this.btnSearch.Text = "Sea&rch";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.tsbtnSearch_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(317, 8);
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
            this.btnEdit.Location = new System.Drawing.Point(114, 8);
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
            this.btnCancel.Location = new System.Drawing.Point(417, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 35);
            this.btnCancel.TabIndex = 107;
            this.btnCancel.Text = "Ca&ncel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.tsbtnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(18, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 35);
            this.btnAdd.TabIndex = 103;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtSearch);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(575, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(370, 48);
            this.panel2.TabIndex = 94;
            this.panel2.TabStop = true;
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
            // txtSearch
            // 
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSearch.Location = new System.Drawing.Point(148, 12);
            this.txtSearch.MaxLength = 40;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(203, 23);
            this.txtSearch.TabIndex = 95;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(11, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Search Category :";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.txtDepeciationPer);
            this.panel5.Controls.Add(this.lblDepreciation);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.txtGroupName);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.lblId);
            this.panel5.Controls.Add(this.txtCategoryName);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.lblMsg);
            this.panel5.Location = new System.Drawing.Point(37, 17);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(511, 169);
            this.panel5.TabIndex = 98;
            this.panel5.TabStop = true;
            // 
            // txtDepeciationPer
            // 
            this.txtDepeciationPer.BackColor = System.Drawing.Color.White;
            this.txtDepeciationPer.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDepeciationPer.Font = new System.Drawing.Font("Arial", 10F);
            this.txtDepeciationPer.Location = new System.Drawing.Point(189, 96);
            this.txtDepeciationPer.MaxLength = 2;
            this.txtDepeciationPer.Name = "txtDepeciationPer";
            this.txtDepeciationPer.ReadOnly = true;
            this.txtDepeciationPer.Size = new System.Drawing.Size(78, 23);
            this.txtDepeciationPer.TabIndex = 101;
            this.txtDepeciationPer.Visible = false;
            this.txtDepeciationPer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDecimalPoint_KeyPress);
            // 
            // lblDepreciation
            // 
            this.lblDepreciation.AutoSize = true;
            this.lblDepreciation.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDepreciation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDepreciation.Location = new System.Drawing.Point(69, 99);
            this.lblDepreciation.Name = "lblDepreciation";
            this.lblDepreciation.Size = new System.Drawing.Size(115, 16);
            this.lblDepreciation.TabIndex = 210;
            this.lblDepreciation.Text = "Depreciation(%) :";
            this.lblDepreciation.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(437, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 18);
            this.label7.TabIndex = 209;
            this.label7.Text = "*";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(437, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 18);
            this.label6.TabIndex = 208;
            this.label6.Text = "*";
            // 
            // txtGroupName
            // 
            this.txtGroupName.BackColor = System.Drawing.Color.White;
            this.txtGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtGroupName.Location = new System.Drawing.Point(189, 65);
            this.txtGroupName.MaxLength = 40;
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.ReadOnly = true;
            this.txtGroupName.Size = new System.Drawing.Size(244, 23);
            this.txtGroupName.TabIndex = 100;
            this.txtGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFormalName_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(88, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Group Name :";
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
            // txtCategoryName
            // 
            this.txtCategoryName.BackColor = System.Drawing.Color.White;
            this.txtCategoryName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCategoryName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCategoryName.Location = new System.Drawing.Point(189, 34);
            this.txtCategoryName.MaxLength = 40;
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.ReadOnly = true;
            this.txtCategoryName.Size = new System.Drawing.Size(244, 23);
            this.txtCategoryName.TabIndex = 99;
            this.txtCategoryName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtname_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(72, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Category Name :";
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.Maroon;
            this.lblMsg.Location = new System.Drawing.Point(190, 10);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 14);
            this.lblMsg.TabIndex = 2;
            this.lblMsg.Text = " ";
            // 
            // pangrid
            // 
            this.pangrid.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pangrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pangrid.Controls.Add(this.dgrdName);
            this.pangrid.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pangrid.Location = new System.Drawing.Point(575, 67);
            this.pangrid.Name = "pangrid";
            this.pangrid.Size = new System.Drawing.Size(371, 445);
            this.pangrid.TabIndex = 96;
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
            this.categoryName,
            this.groupName,
            this.depreciationPoint});
            this.dgrdName.EnableHeadersVisualStyles = false;
            this.dgrdName.Location = new System.Drawing.Point(19, 16);
            this.dgrdName.MultiSelect = false;
            this.dgrdName.Name = "dgrdName";
            this.dgrdName.ReadOnly = true;
            this.dgrdName.RowHeadersVisible = false;
            this.dgrdName.RowTemplate.Height = 25;
            this.dgrdName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdName.Size = new System.Drawing.Size(332, 403);
            this.dgrdName.TabIndex = 97;
            this.dgrdName.SelectionChanged += new System.EventHandler(this.dgrdName_SelectionChanged);
            this.dgrdName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgrdName_KeyPress);
            this.dgrdName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgrdName_MouseClick);
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // categoryName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.categoryName.DefaultCellStyle = dataGridViewCellStyle3;
            this.categoryName.HeaderText = "Category Name";
            this.categoryName.MinimumWidth = 15;
            this.categoryName.Name = "categoryName";
            this.categoryName.ReadOnly = true;
            this.categoryName.Width = 160;
            // 
            // groupName
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9F);
            this.groupName.DefaultCellStyle = dataGridViewCellStyle4;
            this.groupName.HeaderText = "Group Name";
            this.groupName.Name = "groupName";
            this.groupName.ReadOnly = true;
            this.groupName.Width = 150;
            // 
            // depreciationPoint
            // 
            this.depreciationPoint.HeaderText = "Point";
            this.depreciationPoint.Name = "depreciationPoint";
            this.depreciationPoint.ReadOnly = true;
            this.depreciationPoint.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.depreciationPoint.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.depreciationPoint.Visible = false;
            // 
            // errorname
            // 
            this.errorname.ContainerControl = this;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
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
            this.panHeader.Location = new System.Drawing.Point(17, 19);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(960, 52);
            this.panHeader.TabIndex = 98;
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.AutoSize = true;
            this.lblNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblNameHeader.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.lblNameHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNameHeader.Location = new System.Drawing.Point(347, 14);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(263, 21);
            this.lblNameHeader.TabIndex = 206;
            this.lblNameHeader.Text = "GROUP CATEGORY MASTER";
            // 
            // CategoryMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "CategoryMaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unit Master";
            this.Load += new System.EventHandler(this.UnitMaster_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UnitMaster_KeyDown);
            this.panColor.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pangrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorname)).EndInit();
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtCategoryName;
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
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.TextBox txtDepeciationPer;
        private System.Windows.Forms.Label lblDepreciation;
        private System.Windows.Forms.Label lblNameHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn depreciationPoint;
    }
}

