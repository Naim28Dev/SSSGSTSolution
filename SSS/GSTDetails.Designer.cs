namespace SSS
{
    partial class GSTDetails
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtMonth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.btnMonth = new System.Windows.Forms.Button();
            this.btnStateName = new System.Windows.Forms.Button();
            this.txtStateName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotalTaxAmt = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblNetAmt = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vchType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serialNo = new System.Windows.Forms.DataGridViewLinkColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.netAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.igstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cgstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sgstAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.5F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(427, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "GST DETAILS";
            // 
            // txtMonth
            // 
            this.txtMonth.BackColor = System.Drawing.SystemColors.Window;
            this.txtMonth.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMonth.Font = new System.Drawing.Font("Arial", 10F);
            this.txtMonth.Location = new System.Drawing.Point(531, 16);
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.ReadOnly = true;
            this.txtMonth.Size = new System.Drawing.Size(105, 23);
            this.txtMonth.TabIndex = 105;
            this.txtMonth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMonth_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(468, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 16);
            this.label5.TabIndex = 221;
            this.label5.Text = "Month :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(356, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 214;
            this.label2.Text = "To";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(895, 10);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(53, 34);
            this.btnGo.TabIndex = 109;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Location = new System.Drawing.Point(208, 16);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(64, 20);
            this.chkDate.TabIndex = 102;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(977, 37);
            this.panel1.TabIndex = 105;
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.txtToDate);
            this.grpSearch.Controls.Add(this.txtFromDate);
            this.grpSearch.Controls.Add(this.lblDetails);
            this.grpSearch.Controls.Add(this.btnMonth);
            this.grpSearch.Controls.Add(this.btnStateName);
            this.grpSearch.Controls.Add(this.txtStateName);
            this.grpSearch.Controls.Add(this.label6);
            this.grpSearch.Controls.Add(this.txtMonth);
            this.grpSearch.Controls.Add(this.label5);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.btnGo);
            this.grpSearch.Controls.Add(this.chkDate);
            this.grpSearch.Location = new System.Drawing.Point(11, 2);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(953, 48);
            this.grpSearch.TabIndex = 101;
            this.grpSearch.TabStop = false;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(385, 15);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(75, 23);
            this.txtToDate.TabIndex = 104;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(276, 15);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(78, 23);
            this.txtFromDate.TabIndex = 103;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetails.Location = new System.Drawing.Point(5, 18);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(39, 15);
            this.lblDetails.TabIndex = 222;
            this.lblDetails.Text = "Detail";
            // 
            // btnMonth
            // 
            this.btnMonth.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnMonth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMonth.Location = new System.Drawing.Point(636, 15);
            this.btnMonth.Name = "btnMonth";
            this.btnMonth.Size = new System.Drawing.Size(24, 25);
            this.btnMonth.TabIndex = 106;
            this.btnMonth.TabStop = false;
            this.btnMonth.UseVisualStyleBackColor = true;
            this.btnMonth.Click += new System.EventHandler(this.btnMonth_Click);
            // 
            // btnStateName
            // 
            this.btnStateName.BackgroundImage = global::SSS.Properties.Resources.downArrow;
            this.btnStateName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStateName.Location = new System.Drawing.Point(869, 14);
            this.btnStateName.Name = "btnStateName";
            this.btnStateName.Size = new System.Drawing.Size(24, 25);
            this.btnStateName.TabIndex = 108;
            this.btnStateName.TabStop = false;
            this.btnStateName.UseVisualStyleBackColor = true;
            this.btnStateName.Click += new System.EventHandler(this.btnStateName_Click);
            // 
            // txtStateName
            // 
            this.txtStateName.BackColor = System.Drawing.SystemColors.Window;
            this.txtStateName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtStateName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtStateName.Location = new System.Drawing.Point(711, 15);
            this.txtStateName.Name = "txtStateName";
            this.txtStateName.ReadOnly = true;
            this.txtStateName.Size = new System.Drawing.Size(157, 23);
            this.txtStateName.TabIndex = 107;
            this.txtStateName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStateName_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(659, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 16);
            this.label6.TabIndex = 227;
            this.label6.Text = "State :";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(863, 544);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 35);
            this.btnClose.TabIndex = 109;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.lblTotalTaxAmt);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.lblNetAmt);
            this.panel3.Controls.Add(this.grpSearch);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Location = new System.Drawing.Point(13, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(976, 585);
            this.panel3.TabIndex = 100;
            this.panel3.TabStop = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(281, 553);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 16);
            this.label4.TabIndex = 225;
            this.label4.Text = "Total Tax Amt :";
            // 
            // lblTotalTaxAmt
            // 
            this.lblTotalTaxAmt.AutoSize = true;
            this.lblTotalTaxAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalTaxAmt.Location = new System.Drawing.Point(389, 553);
            this.lblTotalTaxAmt.Name = "lblTotalTaxAmt";
            this.lblTotalTaxAmt.Size = new System.Drawing.Size(36, 16);
            this.lblTotalTaxAmt.TabIndex = 224;
            this.lblTotalTaxAmt.Text = "0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(13, 553);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 223;
            this.label3.Text = "Net Amount :";
            // 
            // lblNetAmt
            // 
            this.lblNetAmt.AutoSize = true;
            this.lblNetAmt.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblNetAmt.Location = new System.Drawing.Point(112, 553);
            this.lblNetAmt.Name = "lblNetAmt";
            this.lblNetAmt.Size = new System.Drawing.Size(36, 16);
            this.lblNetAmt.TabIndex = 222;
            this.lblNetAmt.Text = "0.00";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.vchType,
            this.serialNo,
            this.partyName,
            this.netAmt,
            this.igstAmt,
            this.cgstAmt,
            this.sgstAmt});
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(11, 58);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 30;
            this.dgrdDetails.Size = new System.Drawing.Size(950, 483);
            this.dgrdDetails.TabIndex = 108;
            this.dgrdDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellContentClick);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // date
            // 
            dataGridViewCellStyle2.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle2;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            // 
            // vchType
            // 
            this.vchType.HeaderText = "Vch Type";
            this.vchType.Name = "vchType";
            this.vchType.ReadOnly = true;
            this.vchType.Width = 110;
            // 
            // serialNo
            // 
            this.serialNo.HeaderText = "Serial No";
            this.serialNo.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.serialNo.Name = "serialNo";
            this.serialNo.ReadOnly = true;
            this.serialNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.serialNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Party Name";
            this.partyName.Name = "partyName";
            this.partyName.ReadOnly = true;
            this.partyName.Width = 220;
            // 
            // netAmt
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.netAmt.DefaultCellStyle = dataGridViewCellStyle3;
            this.netAmt.HeaderText = "Net Amt";
            this.netAmt.Name = "netAmt";
            this.netAmt.ReadOnly = true;
            this.netAmt.Width = 130;
            // 
            // igstAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.igstAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.igstAmt.HeaderText = "IGST Amt";
            this.igstAmt.Name = "igstAmt";
            this.igstAmt.ReadOnly = true;
            this.igstAmt.Visible = false;
            this.igstAmt.Width = 120;
            // 
            // cgstAmt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.cgstAmt.DefaultCellStyle = dataGridViewCellStyle5;
            this.cgstAmt.HeaderText = "CGST Amt";
            this.cgstAmt.Name = "cgstAmt";
            this.cgstAmt.ReadOnly = true;
            this.cgstAmt.Visible = false;
            this.cgstAmt.Width = 120;
            // 
            // sgstAmt
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.sgstAmt.DefaultCellStyle = dataGridViewCellStyle6;
            this.sgstAmt.HeaderText = "SGST Amt";
            this.sgstAmt.Name = "sgstAmt";
            this.sgstAmt.ReadOnly = true;
            this.sgstAmt.Visible = false;
            this.sgstAmt.Width = 120;
            // 
            // GSTDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GSTDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GST Details";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GSTDetails_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Label lblDetails;
        protected internal System.Windows.Forms.TextBox txtMonth;
        protected internal System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotalTaxAmt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblNetAmt;
        private System.Windows.Forms.Button btnStateName;
        protected internal System.Windows.Forms.TextBox txtStateName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnMonth;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn vchType;
        private System.Windows.Forms.DataGridViewLinkColumn serialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn netAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn igstAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn cgstAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn sgstAmt;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}