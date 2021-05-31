namespace SSS
{
    partial class LedgerMonthlySummary
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCreditAmt = new System.Windows.Forms.Label();
            this.lblDebitAmt = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.dgrdItemSummery = new System.Windows.Forms.DataGridView();
            this.month = new System.Windows.Forms.DataGridViewLinkColumn();
            this.debit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.credit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.monthId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdItemSummery)).BeginInit();
            this.panel7.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblCreditAmt);
            this.panel1.Controls.Add(this.lblDebitAmt);
            this.panel1.Controls.Add(this.lblAccount);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.dgrdItemSummery);
            this.panel1.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.panel1.Location = new System.Drawing.Point(25, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(950, 486);
            this.panel1.TabIndex = 2133;
            // 
            // lblCreditAmt
            // 
            this.lblCreditAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblCreditAmt.ForeColor = System.Drawing.Color.Green;
            this.lblCreditAmt.Location = new System.Drawing.Point(546, 457);
            this.lblCreditAmt.Name = "lblCreditAmt";
            this.lblCreditAmt.Size = new System.Drawing.Size(150, 20);
            this.lblCreditAmt.TabIndex = 21;
            this.lblCreditAmt.Tag = "";
            this.lblCreditAmt.Text = "0.00";
            this.lblCreditAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDebitAmt
            // 
            this.lblDebitAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblDebitAmt.ForeColor = System.Drawing.Color.Green;
            this.lblDebitAmt.Location = new System.Drawing.Point(371, 456);
            this.lblDebitAmt.Name = "lblDebitAmt";
            this.lblDebitAmt.Size = new System.Drawing.Size(150, 20);
            this.lblDebitAmt.TabIndex = 20;
            this.lblDebitAmt.Tag = "";
            this.lblDebitAmt.Text = "0.00";
            this.lblDebitAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAccount
            // 
            this.lblAccount.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblAccount.Location = new System.Drawing.Point(35, 25);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(323, 22);
            this.lblAccount.TabIndex = 19;
            this.lblAccount.Text = "  ";
            this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label6);
            this.panel5.Location = new System.Drawing.Point(705, 16);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(210, 35);
            this.panel5.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(32, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "Total Balance";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(364, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(344, 35);
            this.panel3.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(62, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Transaction";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgrdItemSummery
            // 
            this.dgrdItemSummery.AllowUserToAddRows = false;
            this.dgrdItemSummery.AllowUserToDeleteRows = false;
            this.dgrdItemSummery.AllowUserToOrderColumns = true;
            this.dgrdItemSummery.AllowUserToResizeColumns = false;
            this.dgrdItemSummery.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdItemSummery.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdItemSummery.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdItemSummery.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdItemSummery.ColumnHeadersHeight = 30;
            this.dgrdItemSummery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdItemSummery.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.month,
            this.debit,
            this.credit,
            this.totalAmt,
            this.monthId});
            this.dgrdItemSummery.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdItemSummery.EnableHeadersVisualStyles = false;
            this.dgrdItemSummery.GridColor = System.Drawing.SystemColors.ControlText;
            this.dgrdItemSummery.Location = new System.Drawing.Point(32, 52);
            this.dgrdItemSummery.MultiSelect = false;
            this.dgrdItemSummery.Name = "dgrdItemSummery";
            this.dgrdItemSummery.ReadOnly = true;
            this.dgrdItemSummery.RowHeadersVisible = false;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 10.25F);
            this.dgrdItemSummery.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdItemSummery.RowTemplate.Height = 27;
            this.dgrdItemSummery.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrdItemSummery.Size = new System.Drawing.Size(883, 398);
            this.dgrdItemSummery.TabIndex = 0;
            this.dgrdItemSummery.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdItemSummery_CellClick);
            this.dgrdItemSummery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdItemSummery_KeyDown);
            // 
            // month
            // 
            this.month.HeaderText = "Month";
            this.month.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.month.LinkColor = System.Drawing.Color.Black;
            this.month.Name = "month";
            this.month.ReadOnly = true;
            this.month.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.month.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.month.Width = 330;
            // 
            // debit
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.debit.DefaultCellStyle = dataGridViewCellStyle3;
            this.debit.HeaderText = "Debit";
            this.debit.Name = "debit";
            this.debit.ReadOnly = true;
            this.debit.Width = 170;
            // 
            // credit
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = null;
            this.credit.DefaultCellStyle = dataGridViewCellStyle4;
            this.credit.HeaderText = "Credit";
            this.credit.Name = "credit";
            this.credit.ReadOnly = true;
            this.credit.Width = 170;
            // 
            // totalAmt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = null;
            this.totalAmt.DefaultCellStyle = dataGridViewCellStyle5;
            this.totalAmt.HeaderText = "Total Amount";
            this.totalAmt.Name = "totalAmt";
            this.totalAmt.ReadOnly = true;
            this.totalAmt.Width = 210;
            // 
            // monthId
            // 
            this.monthId.HeaderText = "ID";
            this.monthId.Name = "monthId";
            this.monthId.ReadOnly = true;
            this.monthId.Visible = false;
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.AutoSize = true;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmt.ForeColor = System.Drawing.Color.Green;
            this.lblTotalAmt.Location = new System.Drawing.Point(154, 19);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size(36, 16);
            this.lblTotalAmt.TabIndex = 5;
            this.lblTotalAmt.Tag = "";
            this.lblTotalAmt.Text = "0.00";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(30, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "Total Amount :";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel7.Controls.Add(this.btnClose);
            this.panel7.Controls.Add(this.btnPrint);
            this.panel7.Controls.Add(this.btnPreview);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Controls.Add(this.lblTotalAmt);
            this.panel7.Location = new System.Drawing.Point(25, 582);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(950, 52);
            this.panel7.TabIndex = 2135;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(827, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 34);
            this.btnClose.TabIndex = 84;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(598, 8);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(96, 34);
            this.btnPrint.TabIndex = 39;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(716, 8);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(96, 34);
            this.btnPreview.TabIndex = 40;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Month";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 330;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Debit";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 170;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Credit";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 170;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Closing Amount";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 210;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Location = new System.Drawing.Point(25, 11);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(950, 40);
            this.panel4.TabIndex = 2137;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(325, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(279, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ledger Account Monthly Summary";
            // 
            // LedgerMonthlySummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LedgerMonthlySummary";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ledger Monthly Summary";
            this.Load += new System.EventHandler(this.LedgerMonthlySummery_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LedgerMonthlySummery_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdItemSummery)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalAmt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgrdItemSummery;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewLinkColumn month;
        private System.Windows.Forms.DataGridViewTextBoxColumn debit;
        private System.Windows.Forms.DataGridViewTextBoxColumn credit;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn monthId;
        private System.Windows.Forms.Label lblCreditAmt;
        private System.Windows.Forms.Label lblDebitAmt;
    }
}