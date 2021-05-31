namespace SSS
{
    partial class ShowCategoryWiseDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.sNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.particulars = new System.Windows.Forms.DataGridViewLinkColumn();
            this.debitAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creditAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel2.SuspendLayout();
            this.grpData.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGroupName
            // 
            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblGroupName.Location = new System.Drawing.Point(5, 20);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(91, 16);
            this.lblGroupName.TabIndex = 115;
            this.lblGroupName.Text = "GroupName";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDate.Location = new System.Drawing.Point(352, 20);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(37, 16);
            this.lblDate.TabIndex = 114;
            this.lblDate.Text = "Date";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeColumns = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sNo,
            this.particulars,
            this.debitAmt,
            this.creditAmt});
            this.dgrdDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(22, 63);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.25F);
            this.dgrdDetails.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgrdDetails.RowTemplate.Height = 27;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(909, 483);
            this.dgrdDetails.TabIndex = 110;
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.AutoSize = true;
            this.lblTotalAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmt.Location = new System.Drawing.Point(669, 21);
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size(36, 16);
            this.lblTotalAmt.TabIndex = 113;
            this.lblTotalAmt.Text = "0.00";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.GhostWhite;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.grpData);
            this.panel2.Controls.Add(this.dgrdDetails);
            this.panel2.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(22, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(959, 569);
            this.panel2.TabIndex = 113;
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.lblGroupName);
            this.grpData.Controls.Add(this.lblDate);
            this.grpData.Controls.Add(this.btnClose);
            this.grpData.Controls.Add(this.lblTotalAmt);
            this.grpData.Controls.Add(this.label1);
            this.grpData.Location = new System.Drawing.Point(23, 1);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(908, 52);
            this.grpData.TabIndex = 114;
            this.grpData.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(824, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(77, 33);
            this.btnClose.TabIndex = 111;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(594, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 112;
            this.label1.Text = "Total Amt :";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.lblDisplay);
            this.panel4.Location = new System.Drawing.Point(22, 16);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(959, 40);
            this.panel4.TabIndex = 2145;
            // 
            // lblDisplay
            // 
            this.lblDisplay.AutoSize = true;
            this.lblDisplay.BackColor = System.Drawing.Color.Transparent;
            this.lblDisplay.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblDisplay.ForeColor = System.Drawing.Color.Black;
            this.lblDisplay.Location = new System.Drawing.Point(320, 9);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Size = new System.Drawing.Size(304, 19);
            this.lblDisplay.TabIndex = 117;
            this.lblDisplay.Text = "CATEGORY WISE BALANCE DETAILS";
            // 
            // sNo
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sNo.DefaultCellStyle = dataGridViewCellStyle9;
            this.sNo.HeaderText = "S.No.";
            this.sNo.Name = "sNo";
            this.sNo.ReadOnly = true;
            this.sNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.sNo.Width = 60;
            // 
            // particulars
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.particulars.DefaultCellStyle = dataGridViewCellStyle10;
            this.particulars.HeaderText = "Particulars";
            this.particulars.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.particulars.LinkColor = System.Drawing.Color.Black;
            this.particulars.Name = "particulars";
            this.particulars.ReadOnly = true;
            this.particulars.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.particulars.Width = 420;
            // 
            // debitAmt
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.Format = "N2";
            this.debitAmt.DefaultCellStyle = dataGridViewCellStyle11;
            this.debitAmt.HeaderText = "Debit Amount";
            this.debitAmt.Name = "debitAmt";
            this.debitAmt.ReadOnly = true;
            this.debitAmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.debitAmt.Width = 200;
            // 
            // creditAmt
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.Format = "N2";
            this.creditAmt.DefaultCellStyle = dataGridViewCellStyle12;
            this.creditAmt.HeaderText = "Credit Amount";
            this.creditAmt.Name = "creditAmt";
            this.creditAmt.ReadOnly = true;
            this.creditAmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.creditAmt.Width = 200;
            // 
            // ShowCategoryWiseDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ShowCategoryWiseDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Show Balance Sheet Details";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShowBalanceSheetDetails_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel2.ResumeLayout(false);
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Label lblTotalAmt;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblDisplay;
        private System.Windows.Forms.DataGridViewTextBoxColumn sNo;
        private System.Windows.Forms.DataGridViewLinkColumn particulars;
        private System.Windows.Forms.DataGridViewTextBoxColumn debitAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn creditAmt;
    }
}