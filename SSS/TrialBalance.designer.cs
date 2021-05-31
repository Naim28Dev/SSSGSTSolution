namespace SSS
{
    partial class TrialBalance
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgrdTrial = new System.Windows.Forms.DataGridView();
            this.sNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewLinkColumn();
            this.category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openingAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.debitAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creditAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.closingAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkPartyName = new System.Windows.Forms.CheckBox();
            this.chkCategory = new System.Windows.Forms.CheckBox();
            this.chkGroup = new System.Windows.Forms.CheckBox();
            this.btnDetailView = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdTrial)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgrdTrial
            // 
            this.dgrdTrial.AllowUserToAddRows = false;
            this.dgrdTrial.AllowUserToDeleteRows = false;
            this.dgrdTrial.AllowUserToResizeColumns = false;
            this.dgrdTrial.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(249)))), ((int)(((byte)(245)))));
            this.dgrdTrial.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdTrial.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdTrial.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdTrial.ColumnHeadersHeight = 30;
            this.dgrdTrial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdTrial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sNo,
            this.name,
            this.category,
            this.partyName,
            this.openingAmt,
            this.debitAmt,
            this.creditAmt,
            this.closingAmt});
            this.dgrdTrial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdTrial.EnableHeadersVisualStyles = false;
            this.dgrdTrial.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdTrial.Location = new System.Drawing.Point(10, 10);
            this.dgrdTrial.Name = "dgrdTrial";
            this.dgrdTrial.ReadOnly = true;
            this.dgrdTrial.RowHeadersVisible = false;
            this.dgrdTrial.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.25F);
            this.dgrdTrial.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdTrial.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgrdTrial.RowTemplate.Height = 27;
            this.dgrdTrial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdTrial.Size = new System.Drawing.Size(1000, 497);
            this.dgrdTrial.TabIndex = 110;
            this.dgrdTrial.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdTrial_CellContentClick);
            this.dgrdTrial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdTrial_KeyDown);
            // 
            // sNo
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.sNo.HeaderText = "S.No";
            this.sNo.Name = "sNo";
            this.sNo.ReadOnly = true;
            this.sNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sNo.Width = 40;
            // 
            // name
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.name.DefaultCellStyle = dataGridViewCellStyle4;
            this.name.HeaderText = "Group Name";
            this.name.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.name.LinkColor = System.Drawing.Color.Black;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.name.Visible = false;
            this.name.Width = 200;
            // 
            // category
            // 
            this.category.HeaderText = "Category";
            this.category.Name = "category";
            this.category.ReadOnly = true;
            this.category.Visible = false;
            this.category.Width = 140;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.Name = "partyName";
            this.partyName.ReadOnly = true;
            this.partyName.Visible = false;
            this.partyName.Width = 220;
            // 
            // openingAmt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            this.openingAmt.DefaultCellStyle = dataGridViewCellStyle5;
            this.openingAmt.HeaderText = "Opening Amt";
            this.openingAmt.Name = "openingAmt";
            this.openingAmt.ReadOnly = true;
            this.openingAmt.Width = 130;
            // 
            // debitAmt
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            this.debitAmt.DefaultCellStyle = dataGridViewCellStyle6;
            this.debitAmt.HeaderText = "Debit Amount";
            this.debitAmt.Name = "debitAmt";
            this.debitAmt.ReadOnly = true;
            this.debitAmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.debitAmt.Width = 130;
            // 
            // creditAmt
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            this.creditAmt.DefaultCellStyle = dataGridViewCellStyle7;
            this.creditAmt.HeaderText = "Credit Amount";
            this.creditAmt.Name = "creditAmt";
            this.creditAmt.ReadOnly = true;
            this.creditAmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.creditAmt.Width = 130;
            // 
            // closingAmt
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            this.closingAmt.DefaultCellStyle = dataGridViewCellStyle8;
            this.closingAmt.HeaderText = "Closing Amt";
            this.closingAmt.Name = "closingAmt";
            this.closingAmt.ReadOnly = true;
            this.closingAmt.Width = 150;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.GhostWhite;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dgrdTrial);
            this.panel2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(17, 115);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1021, 524);
            this.panel2.TabIndex = 113;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.txtToDate);
            this.panel3.Controls.Add(this.txtFromDate);
            this.panel3.Controls.Add(this.btnExport);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.btnDetailView);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.btnPreview);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnGo);
            this.panel3.Controls.Add(this.chkDate);
            this.panel3.Controls.Add(this.Label21);
            this.panel3.Location = new System.Drawing.Point(16, 67);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1022, 41);
            this.panel3.TabIndex = 100;
            this.panel3.TabStop = true;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(185, 7);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(87, 23);
            this.txtToDate.TabIndex = 103;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(78, 7);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(83, 23);
            this.txtFromDate.TabIndex = 102;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(765, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 33);
            this.btnExport.TabIndex = 113;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkPartyName);
            this.groupBox1.Controls.Add(this.chkCategory);
            this.groupBox1.Controls.Add(this.chkGroup);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Location = new System.Drawing.Point(278, -2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 36);
            this.groupBox1.TabIndex = 104;
            this.groupBox1.TabStop = false;
            // 
            // chkPartyName
            // 
            this.chkPartyName.AutoSize = true;
            this.chkPartyName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPartyName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkPartyName.Location = new System.Drawing.Point(186, 13);
            this.chkPartyName.Name = "chkPartyName";
            this.chkPartyName.Size = new System.Drawing.Size(92, 19);
            this.chkPartyName.TabIndex = 107;
            this.chkPartyName.Text = "Party Name";
            this.chkPartyName.UseVisualStyleBackColor = true;
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkCategory.Location = new System.Drawing.Point(106, 13);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(77, 19);
            this.chkCategory.TabIndex = 106;
            this.chkCategory.Text = "Category";
            this.chkCategory.UseVisualStyleBackColor = true;
            // 
            // chkGroup
            // 
            this.chkGroup.AutoSize = true;
            this.chkGroup.Checked = true;
            this.chkGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkGroup.Location = new System.Drawing.Point(7, 13);
            this.chkGroup.Name = "chkGroup";
            this.chkGroup.Size = new System.Drawing.Size(96, 19);
            this.chkGroup.TabIndex = 105;
            this.chkGroup.Text = "Group Name";
            this.chkGroup.UseVisualStyleBackColor = true;
            // 
            // btnDetailView
            // 
            this.btnDetailView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDetailView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDetailView.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDetailView.ForeColor = System.Drawing.Color.White;
            this.btnDetailView.Location = new System.Drawing.Point(834, 2);
            this.btnDetailView.Name = "btnDetailView";
            this.btnDetailView.Size = new System.Drawing.Size(113, 33);
            this.btnDetailView.TabIndex = 111;
            this.btnDetailView.Text = "&Detail View";
            this.btnDetailView.UseVisualStyleBackColor = false;
            this.btnDetailView.Click += new System.EventHandler(this.btnDetailView_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(946, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(70, 33);
            this.btnClose.TabIndex = 112;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(624, 2);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 33);
            this.btnPreview.TabIndex = 109;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(698, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(68, 33);
            this.btnPrint.TabIndex = 110;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(567, 2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(58, 33);
            this.btnGo.TabIndex = 108;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(8, 9);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 101;
            this.chkDate.Text = "&Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(163, 12);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 24;
            this.Label21.Text = "To";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(18, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1019, 41);
            this.panel1.TabIndex = 111;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(420, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "TRIAL BALANCE";
            // 
            // TrialBalance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TrialBalance";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TrialBalance";
            this.Load += new System.EventHandler(this.TrialBalance_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrialBalance_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdTrial)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrdTrial;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnDetailView;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkPartyName;
        private System.Windows.Forms.CheckBox chkCategory;
        private System.Windows.Forms.CheckBox chkGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNo;
        private System.Windows.Forms.DataGridViewLinkColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn category;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn openingAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn creditAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn closingAmt;
        private System.Windows.Forms.Button btnExport;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}