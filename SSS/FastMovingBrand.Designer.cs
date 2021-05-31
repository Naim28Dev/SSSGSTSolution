namespace SSS
{
    partial class FastMovingBrand
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.dgrdFastMoving = new System.Windows.Forms.DataGridView();
            this.fSNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fBrandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fSaleAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFastMoving = new System.Windows.Forms.Panel();
            this.pnlSlowMoving = new System.Windows.Forms.Panel();
            this.dgrdSlowMoving = new System.Windows.Forms.DataGridView();
            this.sSno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sBrandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sSaleRatio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBrandName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdFastMoving)).BeginInit();
            this.pnlFastMoving.SuspendLayout();
            this.pnlSlowMoving.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSlowMoving)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblHeader);
            this.panel1.Location = new System.Drawing.Point(49, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(490, 40);
            this.panel1.TabIndex = 145;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHeader.Location = new System.Drawing.Point(93, 11);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(306, 19);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "BRAND WISE FAST MOVING DETAILS";
            // 
            // dgrdFastMoving
            // 
            this.dgrdFastMoving.AllowUserToAddRows = false;
            this.dgrdFastMoving.AllowUserToDeleteRows = false;
            this.dgrdFastMoving.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdFastMoving.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dgrdFastMoving.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdFastMoving.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dgrdFastMoving.ColumnHeadersHeight = 30;
            this.dgrdFastMoving.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdFastMoving.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fSNo,
            this.fBrandName,
            this.fSaleAmt});
            this.dgrdFastMoving.EnableHeadersVisualStyles = false;
            this.dgrdFastMoving.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdFastMoving.Location = new System.Drawing.Point(23, 12);
            this.dgrdFastMoving.Name = "dgrdFastMoving";
            this.dgrdFastMoving.ReadOnly = true;
            this.dgrdFastMoving.RowHeadersVisible = false;
            this.dgrdFastMoving.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdFastMoving.RowTemplate.Height = 25;
            this.dgrdFastMoving.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdFastMoving.Size = new System.Drawing.Size(438, 422);
            this.dgrdFastMoving.TabIndex = 137;
            // 
            // fSNo
            // 
            this.fSNo.HeaderText = "S.No.";
            this.fSNo.Name = "fSNo";
            this.fSNo.ReadOnly = true;
            this.fSNo.Width = 60;
            // 
            // fBrandName
            // 
            this.fBrandName.HeaderText = "Brand Name";
            this.fBrandName.Name = "fBrandName";
            this.fBrandName.ReadOnly = true;
            this.fBrandName.Width = 225;
            // 
            // fSaleAmt
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle15.Format = "N2";
            this.fSaleAmt.DefaultCellStyle = dataGridViewCellStyle15;
            this.fSaleAmt.HeaderText = "Sale Ratio";
            this.fSaleAmt.Name = "fSaleAmt";
            this.fSaleAmt.ReadOnly = true;
            this.fSaleAmt.Width = 150;
            // 
            // pnlFastMoving
            // 
            this.pnlFastMoving.BackColor = System.Drawing.Color.White;
            this.pnlFastMoving.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlFastMoving.Controls.Add(this.dgrdFastMoving);
            this.pnlFastMoving.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFastMoving.Location = new System.Drawing.Point(49, 177);
            this.pnlFastMoving.Name = "pnlFastMoving";
            this.pnlFastMoving.Size = new System.Drawing.Size(490, 447);
            this.pnlFastMoving.TabIndex = 149;
            this.pnlFastMoving.Tag = "0";
            // 
            // pnlSlowMoving
            // 
            this.pnlSlowMoving.BackColor = System.Drawing.Color.White;
            this.pnlSlowMoving.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlSlowMoving.Controls.Add(this.dgrdSlowMoving);
            this.pnlSlowMoving.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSlowMoving.Location = new System.Drawing.Point(49, 177);
            this.pnlSlowMoving.Name = "pnlSlowMoving";
            this.pnlSlowMoving.Size = new System.Drawing.Size(490, 447);
            this.pnlSlowMoving.TabIndex = 151;
            this.pnlSlowMoving.Tag = "0";
            // 
            // dgrdSlowMoving
            // 
            this.dgrdSlowMoving.AllowUserToAddRows = false;
            this.dgrdSlowMoving.AllowUserToDeleteRows = false;
            this.dgrdSlowMoving.AllowUserToResizeRows = false;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdSlowMoving.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle16;
            this.dgrdSlowMoving.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSlowMoving.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dgrdSlowMoving.ColumnHeadersHeight = 30;
            this.dgrdSlowMoving.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSlowMoving.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sSno,
            this.sBrandName,
            this.sSaleRatio});
            this.dgrdSlowMoving.EnableHeadersVisualStyles = false;
            this.dgrdSlowMoving.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSlowMoving.Location = new System.Drawing.Point(22, 12);
            this.dgrdSlowMoving.Name = "dgrdSlowMoving";
            this.dgrdSlowMoving.ReadOnly = true;
            this.dgrdSlowMoving.RowHeadersVisible = false;
            this.dgrdSlowMoving.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSlowMoving.RowTemplate.Height = 25;
            this.dgrdSlowMoving.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdSlowMoving.Size = new System.Drawing.Size(438, 422);
            this.dgrdSlowMoving.TabIndex = 140;
            // 
            // sSno
            // 
            this.sSno.HeaderText = "S.No.";
            this.sSno.Name = "sSno";
            this.sSno.ReadOnly = true;
            this.sSno.Width = 60;
            // 
            // sBrandName
            // 
            this.sBrandName.HeaderText = "Brand Name";
            this.sBrandName.Name = "sBrandName";
            this.sBrandName.ReadOnly = true;
            this.sBrandName.Width = 225;
            // 
            // sSaleRatio
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle18.Format = "N2";
            this.sSaleRatio.DefaultCellStyle = dataGridViewCellStyle18;
            this.sSaleRatio.HeaderText = "Sale Ratio";
            this.sSaleRatio.Name = "sSaleRatio";
            this.sSaleRatio.ReadOnly = true;
            this.sSaleRatio.Width = 150;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.txtToDate);
            this.panel4.Controls.Add(this.txtFromDate);
            this.panel4.Controls.Add(this.txtBrandName);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.btnGo);
            this.panel4.Controls.Add(this.chkDate);
            this.panel4.Controls.Add(this.Label21);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Location = new System.Drawing.Point(49, 77);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(490, 90);
            this.panel4.TabIndex = 152;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(205, 46);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(95, 23);
            this.txtToDate.TabIndex = 104;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(81, 46);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(95, 23);
            this.txtFromDate.TabIndex = 103;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtBrandName
            // 
            this.txtBrandName.BackColor = System.Drawing.Color.White;
            this.txtBrandName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBrandName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBrandName.Location = new System.Drawing.Point(110, 14);
            this.txtBrandName.Name = "txtBrandName";
            this.txtBrandName.ReadOnly = true;
            this.txtBrandName.Size = new System.Drawing.Size(360, 23);
            this.txtBrandName.TabIndex = 101;
            this.txtBrandName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBrandName_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(11, 17);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(95, 16);
            this.label14.TabIndex = 151;
            this.label14.Text = "Brand Name :";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(312, 42);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(71, 31);
            this.btnGo.TabIndex = 105;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(15, 48);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "&Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(181, 52);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 145;
            this.Label21.Text = "To";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(384, 42);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 31);
            this.btnClose.TabIndex = 106;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FastMovingBrand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(590, 658);
            this.Controls.Add(this.pnlSlowMoving);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.pnlFastMoving);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FastMovingBrand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fast Moving Brand";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FastMovingBrand_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdFastMoving)).EndInit();
            this.pnlFastMoving.ResumeLayout(false);
            this.pnlSlowMoving.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSlowMoving)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgrdFastMoving;
        private System.Windows.Forms.Panel pnlFastMoving;
        private System.Windows.Forms.Panel pnlSlowMoving;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.DataGridView dgrdSlowMoving;
        private System.Windows.Forms.Panel panel4;
        protected internal System.Windows.Forms.TextBox txtBrandName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn fSNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn fBrandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn fSaleAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn sSno;
        private System.Windows.Forms.DataGridViewTextBoxColumn sBrandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn sSaleRatio;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.MaskedTextBox txtToDate;
    }
}