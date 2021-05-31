namespace SSS
{
    partial class BalanceSheet_New
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnDetailView = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.liability = new System.Windows.Forms.DataGridViewLinkColumn();
            this.debitAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assests = new System.Windows.Forms.DataGridViewLinkColumn();
            this.creditAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDetailView
            // 
            this.btnDetailView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDetailView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDetailView.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDetailView.ForeColor = System.Drawing.Color.White;
            this.btnDetailView.Location = new System.Drawing.Point(749, 2);
            this.btnDetailView.Name = "btnDetailView";
            this.btnDetailView.Size = new System.Drawing.Size(135, 34);
            this.btnDetailView.TabIndex = 107;
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
            this.btnClose.Location = new System.Drawing.Point(885, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(77, 34);
            this.btnClose.TabIndex = 108;
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
            this.btnPreview.Location = new System.Drawing.Point(548, 2);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(107, 34);
            this.btnPreview.TabIndex = 105;
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
            this.btnPrint.Location = new System.Drawing.Point(656, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(93, 34);
            this.btnPrint.TabIndex = 106;
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
            this.btnGo.Location = new System.Drawing.Point(323, 2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(71, 34);
            this.btnGo.TabIndex = 104;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(22, 9);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 101;
            this.chkDate.Text = "&Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.txtFromDate);
            this.panel3.Controls.Add(this.txtToDate);
            this.panel3.Controls.Add(this.btnDetailView);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.btnPreview);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnGo);
            this.panel3.Controls.Add(this.chkDate);
            this.panel3.Controls.Add(this.Label21);
            this.panel3.Location = new System.Drawing.Point(13, 61);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(974, 41);
            this.panel3.TabIndex = 100;
            this.panel3.TabStop = true;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(212, 6);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(95, 23);
            this.txtToDate.TabIndex = 103;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtToDate_Leave);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(188, 13);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 24;
            this.Label21.Text = "To";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.GhostWhite;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dgrdDetails);
            this.panel2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(14, 110);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(973, 531);
            this.panel2.TabIndex = 116;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeColumns = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdDetails.ColumnHeadersHeight = 32;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.liability,
            this.debitAmt,
            this.assests,
            this.creditAmt});
            this.dgrdDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(17, 15);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F);
            this.dgrdDetails.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(937, 498);
            this.dgrdDetails.TabIndex = 110;
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // liability
            // 
            this.liability.HeaderText = "Liabilities";
            this.liability.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.liability.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.liability.Name = "liability";
            this.liability.ReadOnly = true;
            this.liability.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.liability.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.liability.Width = 295;
            // 
            // debitAmt
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            this.debitAmt.DefaultCellStyle = dataGridViewCellStyle7;
            this.debitAmt.HeaderText = "Amount";
            this.debitAmt.Name = "debitAmt";
            this.debitAmt.ReadOnly = true;
            this.debitAmt.Width = 160;
            // 
            // assests
            // 
            this.assests.HeaderText = "Assets";
            this.assests.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.assests.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.assests.Name = "assests";
            this.assests.ReadOnly = true;
            this.assests.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.assests.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.assests.Width = 295;
            // 
            // creditAmt
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            this.creditAmt.DefaultCellStyle = dataGridViewCellStyle8;
            this.creditAmt.HeaderText = "Amount";
            this.creditAmt.Name = "creditAmt";
            this.creditAmt.ReadOnly = true;
            this.creditAmt.Width = 160;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Location = new System.Drawing.Point(14, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(973, 40);
            this.panel4.TabIndex = 2144;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(410, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "BALANCE SHEET";
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(87, 6);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(95, 23);
            this.txtFromDate.TabIndex = 109;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtToDate_Leave);
            // 
            // BalanceSheet_New
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BalanceSheet_New";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Balance Sheet";
            this.Load += new System.EventHandler(this.BalanceSheet_New_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BalanceSheet_KeyDown);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDetailView;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridViewLinkColumn liability;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitAmt;
        private System.Windows.Forms.DataGridViewLinkColumn assests;
        private System.Windows.Forms.DataGridViewTextBoxColumn creditAmt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}