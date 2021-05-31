namespace SSS
{
    partial class Month_Lock
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgrdMonth = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.changePanel = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.monthName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMonth)).BeginInit();
            this.panel3.SuspendLayout();
            this.changePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgrdMonth
            // 
            this.dgrdMonth.AllowUserToAddRows = false;
            this.dgrdMonth.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgrdMonth.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrdMonth.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdMonth.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdMonth.ColumnHeadersHeight = 30;
            this.dgrdMonth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdMonth.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkStatus,
            this.monthName,
            this.status});
            this.dgrdMonth.EnableHeadersVisualStyles = false;
            this.dgrdMonth.GridColor = System.Drawing.SystemColors.ControlText;
            this.dgrdMonth.Location = new System.Drawing.Point(22, 24);
            this.dgrdMonth.Name = "dgrdMonth";
            this.dgrdMonth.RowHeadersVisible = false;
            this.dgrdMonth.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 10F);
            this.dgrdMonth.RowTemplate.Height = 26;
            this.dgrdMonth.Size = new System.Drawing.Size(451, 357);
            this.dgrdMonth.TabIndex = 106;
            this.dgrdMonth.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdMonth_CellBeginEdit);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.changePanel);
            this.panel3.Controls.Add(this.dgrdMonth);
            this.panel3.Location = new System.Drawing.Point(129, 25);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(507, 459);
            this.panel3.TabIndex = 0;
            // 
            // changePanel
            // 
            this.changePanel.BackColor = System.Drawing.Color.White;
            this.changePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.changePanel.Controls.Add(this.btnClose);
            this.changePanel.Controls.Add(this.btnResetPassword);
            this.changePanel.Location = new System.Drawing.Point(22, 392);
            this.changePanel.Name = "changePanel";
            this.changePanel.Size = new System.Drawing.Size(452, 44);
            this.changePanel.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(300, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 37);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnResetPassword.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.Location = new System.Drawing.Point(56, 2);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(215, 37);
            this.btnResetPassword.TabIndex = 19;
            this.btnResetPassword.Text = "Lock/Unlock Months";
            this.btnResetPassword.UseVisualStyleBackColor = false;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(103, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(783, 42);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(100, 99);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 515);
            this.panel2.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(323, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "MONTH LOCK";
            // 
            // chkStatus
            // 
            this.chkStatus.HeaderText = "";
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Width = 35;
            // 
            // monthName
            // 
            this.monthName.HeaderText = "Month Name";
            this.monthName.Name = "monthName";
            this.monthName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.monthName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.monthName.Width = 250;
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            this.status.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.status.Width = 150;
            // 
            // Month_Lock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Month_Lock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Month Lock";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Month_Lock_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMonth)).EndInit();
            this.panel3.ResumeLayout(false);
            this.changePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgrdMonth;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel changePanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn monthName;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
    }
}