namespace SSS
{
    partial class SupplierMapingRegister
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LabelHeader = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtMarketer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPToSNo = new System.Windows.Forms.TextBox();
            this.txtPFromSNo = new System.Windows.Forms.TextBox();
            this.chkPSNo = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSupplier = new System.Windows.Forms.TextBox();
            this.lblPartyHeader = new System.Windows.Forms.Label();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSerialCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dgrdDetails = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.LabelHeader);
            this.panel1.Font = new System.Drawing.Font("Arial", 8.25F);
            this.panel1.Location = new System.Drawing.Point(16, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(878, 29);
            this.panel1.TabIndex = 0;
            // 
            // LabelHeader
            // 
            this.LabelHeader.AutoSize = true;
            this.LabelHeader.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.LabelHeader.ForeColor = System.Drawing.Color.Black;
            this.LabelHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelHeader.Location = new System.Drawing.Point(271, 3);
            this.LabelHeader.Name = "LabelHeader";
            this.LabelHeader.Size = new System.Drawing.Size(290, 19);
            this.LabelHeader.TabIndex = 7;
            this.LabelHeader.Text = "Marketer Supplier Mapping Register";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtMarketer);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtPToSNo);
            this.panel2.Controls.Add(this.txtPFromSNo);
            this.panel2.Controls.Add(this.chkPSNo);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.txtSupplier);
            this.panel2.Controls.Add(this.lblPartyHeader);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtSerialCode);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Location = new System.Drawing.Point(16, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(878, 63);
            this.panel2.TabIndex = 1;
            this.panel2.TabStop = true;
            // 
            // txtMarketer
            // 
            this.txtMarketer.BackColor = System.Drawing.Color.White;
            this.txtMarketer.Font = new System.Drawing.Font("Arial", 9F);
            this.txtMarketer.Location = new System.Drawing.Point(71, 31);
            this.txtMarketer.Name = "txtMarketer";
            this.txtMarketer.ReadOnly = true;
            this.txtMarketer.Size = new System.Drawing.Size(314, 21);
            this.txtMarketer.TabIndex = 5;
            this.txtMarketer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMarketer_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(5, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 176;
            this.label1.Text = "Marketer :";
            // 
            // txtPToSNo
            // 
            this.txtPToSNo.BackColor = System.Drawing.Color.White;
            this.txtPToSNo.Font = new System.Drawing.Font("Arial", 9F);
            this.txtPToSNo.Location = new System.Drawing.Point(590, 30);
            this.txtPToSNo.MaxLength = 10;
            this.txtPToSNo.Name = "txtPToSNo";
            this.txtPToSNo.ReadOnly = true;
            this.txtPToSNo.Size = new System.Drawing.Size(82, 21);
            this.txtPToSNo.TabIndex = 8;
            this.txtPToSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPFromSNo_KeyPress);
            // 
            // txtPFromSNo
            // 
            this.txtPFromSNo.BackColor = System.Drawing.Color.White;
            this.txtPFromSNo.Font = new System.Drawing.Font("Arial", 9F);
            this.txtPFromSNo.Location = new System.Drawing.Point(480, 30);
            this.txtPFromSNo.MaxLength = 10;
            this.txtPFromSNo.Name = "txtPFromSNo";
            this.txtPFromSNo.ReadOnly = true;
            this.txtPFromSNo.Size = new System.Drawing.Size(84, 21);
            this.txtPFromSNo.TabIndex = 7;
            this.txtPFromSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPFromSNo_KeyPress);
            // 
            // chkPSNo
            // 
            this.chkPSNo.AutoSize = true;
            this.chkPSNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPSNo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.chkPSNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkPSNo.Location = new System.Drawing.Point(394, 30);
            this.chkPSNo.Name = "chkPSNo";
            this.chkPSNo.Size = new System.Drawing.Size(83, 19);
            this.chkPSNo.TabIndex = 6;
            this.chkPSNo.Text = "Serial No :";
            this.chkPSNo.UseVisualStyleBackColor = true;
            this.chkPSNo.CheckedChanged += new System.EventHandler(this.chkPSNo_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(566, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 15);
            this.label4.TabIndex = 174;
            this.label4.Text = "To";
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 9F);
            this.txtToDate.Location = new System.Drawing.Point(781, 5);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(83, 21);
            this.txtToDate.TabIndex = 4;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 9F);
            this.txtFromDate.Location = new System.Drawing.Point(656, 5);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(87, 21);
            this.txtFromDate.TabIndex = 3;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(781, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(85, 31);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "&Go";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSupplier
            // 
            this.txtSupplier.BackColor = System.Drawing.Color.White;
            this.txtSupplier.Font = new System.Drawing.Font("Arial", 9F);
            this.txtSupplier.Location = new System.Drawing.Point(71, 7);
            this.txtSupplier.Name = "txtSupplier";
            this.txtSupplier.ReadOnly = true;
            this.txtSupplier.Size = new System.Drawing.Size(314, 21);
            this.txtSupplier.TabIndex = 0;
            this.txtSupplier.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSupplier_KeyDown);
            // 
            // lblPartyHeader
            // 
            this.lblPartyHeader.AutoSize = true;
            this.lblPartyHeader.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblPartyHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPartyHeader.Location = new System.Drawing.Point(5, 10);
            this.lblPartyHeader.Name = "lblPartyHeader";
            this.lblPartyHeader.Size = new System.Drawing.Size(60, 15);
            this.lblPartyHeader.TabIndex = 152;
            this.lblPartyHeader.Text = "Supplier :";
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.chkDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkDate.Location = new System.Drawing.Point(599, 7);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(58, 19);
            this.chkDate.TabIndex = 2;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(751, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 15);
            this.label2.TabIndex = 106;
            this.label2.Text = "To";
            // 
            // txtSerialCode
            // 
            this.txtSerialCode.BackColor = System.Drawing.Color.White;
            this.txtSerialCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerialCode.Font = new System.Drawing.Font("Arial", 9F);
            this.txtSerialCode.Location = new System.Drawing.Point(480, 5);
            this.txtSerialCode.Name = "txtSerialCode";
            this.txtSerialCode.ReadOnly = true;
            this.txtSerialCode.Size = new System.Drawing.Size(102, 21);
            this.txtSerialCode.TabIndex = 1;
            this.txtSerialCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialCode_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(397, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 15);
            this.label10.TabIndex = 164;
            this.label10.Text = "Serial Code :";
            // 
            // dgrdDetails
            // 
            this.dgrdDetails.AllowUserToAddRows = false;
            this.dgrdDetails.AllowUserToDeleteRows = false;
            this.dgrdDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.MistyRose;
            this.dgrdDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dgrdDetails.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgrdDetails.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgrdDetails.ColumnHeadersHeight = 30;
            this.dgrdDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdDetails.EnableHeadersVisualStyles = false;
            this.dgrdDetails.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdDetails.Location = new System.Drawing.Point(8, 9);
            this.dgrdDetails.Name = "dgrdDetails";
            this.dgrdDetails.ReadOnly = true;
            this.dgrdDetails.RowHeadersVisible = false;
            this.dgrdDetails.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdDetails.RowTemplate.Height = 25;
            this.dgrdDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdDetails.Size = new System.Drawing.Size(858, 380);
            this.dgrdDetails.TabIndex = 0;
            this.dgrdDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellClick);
            this.dgrdDetails.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdDetails_CellMouseEnter);
            this.dgrdDetails.Sorted += new System.EventHandler(this.dgrdDetails_Sorted);
            this.dgrdDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdDetails_KeyDown);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dgrdDetails);
            this.panel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.Location = new System.Drawing.Point(16, 123);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(876, 432);
            this.panel3.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(734, 514);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(72, 29);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "&Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(816, 514);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 29);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // SupplierMapingRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(913, 572);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "SupplierMapingRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supplier Marketer Mapping Register";
            this.Load += new System.EventHandler(this.SupplierMapingRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SupplierMapingRegister_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdDetails)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LabelHeader;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.TextBox txtSupplier;
        private System.Windows.Forms.Label lblPartyHeader;
        private System.Windows.Forms.TextBox txtSerialCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgrdDetails;
        private System.Windows.Forms.Panel panel3;
        protected internal System.Windows.Forms.CheckBox chkDate;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
        protected internal System.Windows.Forms.TextBox txtPToSNo;
        protected internal System.Windows.Forms.TextBox txtPFromSNo;
        private System.Windows.Forms.CheckBox chkPSNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
        protected internal System.Windows.Forms.TextBox txtMarketer;
        private System.Windows.Forms.Label label1;
    }
}