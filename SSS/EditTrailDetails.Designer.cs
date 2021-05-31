namespace SSS
{
    partial class EditTrailDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTax = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.panDisp = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.netAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTax.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panDisp.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTax
            // 
            this.pnlTax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.pnlTax.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlTax.Controls.Add(this.panel4);
            this.pnlTax.Location = new System.Drawing.Point(15, 49);
            this.pnlTax.Name = "pnlTax";
            this.pnlTax.Size = new System.Drawing.Size(391, 440);
            this.pnlTax.TabIndex = 165;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.dgrdDetails);
            this.panel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.panel4.Location = new System.Drawing.Point(7, 15);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(371, 406);
            this.panel4.TabIndex = 165;
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdDetails.ColumnHeadersHeight = 28;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.updatedBy,
            this.netAmt,
            this.remark,
            this.editStatus});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.Color.Black;
            this.dgrdDetails.Location = new System.Drawing.Point(12, 10);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdDetails.RowTemplate.Height = 28;
            this.dgrdDetails.Size = new System.Drawing.Size(347, 383);
            this.dgrdDetails.TabIndex = 138;
            // 
            // panDisp
            // 
            this.panDisp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panDisp.AutoSize = true;
            this.panDisp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panDisp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panDisp.Controls.Add(this.lblName);
            this.panDisp.Location = new System.Drawing.Point(14, 7);
            this.panDisp.Name = "panDisp";
            this.panDisp.Size = new System.Drawing.Size(392, 35);
            this.panDisp.TabIndex = 219;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(115, 6);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(156, 18);
            this.lblName.TabIndex = 222;
            this.lblName.Text = "EDIT TRAIL DETAILS";
            // 
            // date
            // 
            dataGridViewCellStyle3.Format = "MMM dd yyyy hh:mm tt";
            this.date.DefaultCellStyle = dataGridViewCellStyle3;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 130;
            // 
            // updatedBy
            // 
            this.updatedBy.HeaderText = "Updated By";
            this.updatedBy.Name = "updatedBy";
            this.updatedBy.ReadOnly = true;
            // 
            // netAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.netAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.netAmt.HeaderText = "Net Amt";
            this.netAmt.Name = "netAmt";
            this.netAmt.ReadOnly = true;
            // 
            // remark
            // 
            this.remark.HeaderText = "Remarks";
            this.remark.Name = "remark";
            this.remark.ReadOnly = true;
            this.remark.Visible = false;
            this.remark.Width = 300;
            // 
            // editStatus
            // 
            this.editStatus.HeaderText = "Edit Status";
            this.editStatus.Name = "editStatus";
            this.editStatus.ReadOnly = true;
            this.editStatus.Visible = false;
            // 
            // EditTrailDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(411, 502);
            this.Controls.Add(this.panDisp);
            this.Controls.Add(this.pnlTax);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "EditTrailDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Edit Trail Details";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditTrailDetails_KeyDown);
            this.pnlTax.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panDisp.ResumeLayout(false);
            this.panDisp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTax;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panDisp;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn netAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn remark;
        private System.Windows.Forms.DataGridViewTextBoxColumn editStatus;
    }
}