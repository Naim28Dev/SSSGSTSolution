namespace SSS
{
    partial class EditLogReport
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtNetAmt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.txtBillType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.sno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billtype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.billNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.netAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.txtNetAmt);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.Label21);
            this.panel2.Controls.Add(this.txtBillType);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Location = new System.Drawing.Point(12, 63);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1024, 37);
            this.panel2.TabIndex = 114;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(798, 5);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(87, 23);
            this.txtToDate.TabIndex = 114;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(677, 5);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(92, 23);
            this.txtFromDate.TabIndex = 113;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtNetAmt
            // 
            this.txtNetAmt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNetAmt.Font = new System.Drawing.Font("Arial", 10F);
            this.txtNetAmt.Location = new System.Drawing.Point(445, 6);
            this.txtNetAmt.Name = "txtNetAmt";
            this.txtNetAmt.Size = new System.Drawing.Size(90, 23);
            this.txtNetAmt.TabIndex = 121;
            this.txtNetAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNetAmt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReason_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(375, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 16);
            this.label2.TabIndex = 120;
            this.label2.Text = "Net Amt :";
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(566, 8);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(108, 20);
            this.chkDate.TabIndex = 112;
            this.chkDate.Text = "Edited Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(777, 10);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 111;
            this.Label21.Text = "To";
            // 
            // txtBillType
            // 
            this.txtBillType.BackColor = System.Drawing.Color.White;
            this.txtBillType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillType.Font = new System.Drawing.Font("Arial", 9.75F);
            this.txtBillType.Location = new System.Drawing.Point(83, 6);
            this.txtBillType.Name = "txtBillType";
            this.txtBillType.ReadOnly = true;
            this.txtBillType.Size = new System.Drawing.Size(261, 22);
            this.txtBillType.TabIndex = 101;
            this.txtBillType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillType_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 16);
            this.label8.TabIndex = 72;
            this.label8.Text = "Bill Type :";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(926, 1);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(77, 32);
            this.btnGo.TabIndex = 110;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(11, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1025, 40);
            this.panel1.TabIndex = 116;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(430, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = "EDIT LOG REPORT";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.panel3.Location = new System.Drawing.Point(10, 114);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1024, 528);
            this.panel3.TabIndex = 115;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(898, 489);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 34);
            this.btnCancel.TabIndex = 118;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sno,
            this.Date,
            this.billtype,
            this.billCode,
            this.billNo,
            this.netAmt,
            this.editStatus,
            this.userName});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(16, 14);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.Size = new System.Drawing.Size(989, 472);
            this.dgrdDetails.TabIndex = 112;
            this.dgrdDetails.Sorted += new System.EventHandler(this.dgrdDetails_Sorted);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // sno
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.sno.DefaultCellStyle = dataGridViewCellStyle2;
            this.sno.HeaderText = "S.No.";
            this.sno.Name = "sno";
            this.sno.ReadOnly = true;
            this.sno.Width = 55;
            // 
            // Date
            // 
            dataGridViewCellStyle3.Format = "dd/MM/yyyy";
            this.Date.DefaultCellStyle = dataGridViewCellStyle3;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // billtype
            // 
            this.billtype.HeaderText = "Bill Type";
            this.billtype.Name = "billtype";
            this.billtype.ReadOnly = true;
            this.billtype.Width = 110;
            // 
            // billCode
            // 
            this.billCode.HeaderText = "Bill Code";
            this.billCode.Name = "billCode";
            this.billCode.ReadOnly = true;
            this.billCode.Width = 130;
            // 
            // billNo
            // 
            this.billNo.HeaderText = "Bill No";
            this.billNo.Name = "billNo";
            this.billNo.ReadOnly = true;
            // 
            // netAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.netAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.netAmt.HeaderText = "Net Amt";
            this.netAmt.Name = "netAmt";
            this.netAmt.ReadOnly = true;
            this.netAmt.Width = 130;
            // 
            // editStatus
            // 
            this.editStatus.HeaderText = "Edit Status";
            this.editStatus.Name = "editStatus";
            this.editStatus.ReadOnly = true;
            this.editStatus.Width = 160;
            // 
            // userName
            // 
            this.userName.HeaderText = "User Name";
            this.userName.Name = "userName";
            this.userName.ReadOnly = true;
            this.userName.Width = 180;
            // 
            // EditLogReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "EditLogReport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EDIT LOG REPORT";
            this.Load += new System.EventHandler(this.EditLogReport_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RemovalReason_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Button btnCancel;
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.TextBox txtNetAmt;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.TextBox txtBillType;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn sno;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn billtype;
        private System.Windows.Forms.DataGridViewTextBoxColumn billCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn billNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn netAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn editStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn userName;
    }
}