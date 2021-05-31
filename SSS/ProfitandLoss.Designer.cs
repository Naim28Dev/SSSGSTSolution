namespace SSS
{
    partial class ProfitandLoss
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgrdPL = new System.Windows.Forms.DataGridView();
            this.leftParticulars = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.leftAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rightParticulars = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rightAmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.btnDetailView = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.picPleasewait = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPL)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(23, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(973, 40);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(404, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "PROFIT && LOSS";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dgrdPL);
            this.panel2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(24, 124);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(973, 511);
            this.panel2.TabIndex = 4;
            // 
            // dgrdPL
            // 
            this.dgrdPL.AllowUserToAddRows = false;
            this.dgrdPL.AllowUserToDeleteRows = false;
            this.dgrdPL.AllowUserToResizeColumns = false;
            this.dgrdPL.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdPL.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdPL.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdPL.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdPL.ColumnHeadersHeight = 30;
            this.dgrdPL.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdPL.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.leftParticulars,
            this.leftAmt,
            this.rightParticulars,
            this.rightAmt});
            this.dgrdPL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdPL.EnableHeadersVisualStyles = false;
            this.dgrdPL.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdPL.Location = new System.Drawing.Point(17, 15);
            this.dgrdPL.Name = "dgrdPL";
            this.dgrdPL.ReadOnly = true;
            this.dgrdPL.RowHeadersVisible = false;
            this.dgrdPL.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdPL.RowTemplate.Height = 27;
            this.dgrdPL.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrdPL.Size = new System.Drawing.Size(937, 472);
            this.dgrdPL.TabIndex = 1;
            this.dgrdPL.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdPL_CellBeginEdit);
            this.dgrdPL.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdPL_CellContentClick);
            this.dgrdPL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdPL_KeyDown);
            // 
            // leftParticulars
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 10.25F);
            this.leftParticulars.DefaultCellStyle = dataGridViewCellStyle3;
            this.leftParticulars.HeaderText = "Particulars";
            this.leftParticulars.Name = "leftParticulars";
            this.leftParticulars.ReadOnly = true;
            this.leftParticulars.Width = 285;
            // 
            // leftAmt
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = null;
            this.leftAmt.DefaultCellStyle = dataGridViewCellStyle4;
            this.leftAmt.HeaderText = "Amount";
            this.leftAmt.Name = "leftAmt";
            this.leftAmt.ReadOnly = true;
            this.leftAmt.Width = 170;
            // 
            // rightParticulars
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 10.25F);
            this.rightParticulars.DefaultCellStyle = dataGridViewCellStyle5;
            this.rightParticulars.HeaderText = "Particulars";
            this.rightParticulars.Name = "rightParticulars";
            this.rightParticulars.ReadOnly = true;
            this.rightParticulars.Width = 285;
            // 
            // rightAmt
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = null;
            this.rightAmt.DefaultCellStyle = dataGridViewCellStyle6;
            this.rightAmt.HeaderText = "Amount";
            this.rightAmt.Name = "rightAmt";
            this.rightAmt.ReadOnly = true;
            this.rightAmt.Width = 170;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel7.Controls.Add(this.txtFromDate);
            this.panel7.Controls.Add(this.txtToDate);
            this.panel7.Controls.Add(this.btnDetailView);
            this.panel7.Controls.Add(this.btnGo);
            this.panel7.Controls.Add(this.chkDate);
            this.panel7.Controls.Add(this.Label21);
            this.panel7.Controls.Add(this.btnCancel);
            this.panel7.Controls.Add(this.btnPrint);
            this.panel7.Controls.Add(this.btnPreview);
            this.panel7.Location = new System.Drawing.Point(23, 69);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(973, 43);
            this.panel7.TabIndex = 100;
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(95, 8);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(95, 23);
            this.txtFromDate.TabIndex = 102;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(225, 8);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(89, 23);
            this.txtToDate.TabIndex = 103;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // btnDetailView
            // 
            this.btnDetailView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDetailView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDetailView.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDetailView.ForeColor = System.Drawing.Color.White;
            this.btnDetailView.Location = new System.Drawing.Point(752, 3);
            this.btnDetailView.Name = "btnDetailView";
            this.btnDetailView.Size = new System.Drawing.Size(121, 33);
            this.btnDetailView.TabIndex = 107;
            this.btnDetailView.Text = "&Detail View";
            this.btnDetailView.UseVisualStyleBackColor = false;
            this.btnDetailView.Click += new System.EventHandler(this.btnDetailView_Click);
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(331, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(71, 33);
            this.btnGo.TabIndex = 104;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDate.Location = new System.Drawing.Point(30, 10);
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
            this.Label21.Location = new System.Drawing.Point(197, 12);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(20, 15);
            this.Label21.TabIndex = 105;
            this.Label21.Text = "To";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(872, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 33);
            this.btnCancel.TabIndex = 108;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(559, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(97, 33);
            this.btnPrint.TabIndex = 105;
            this.btnPrint.TabStop = false;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(655, 3);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(97, 33);
            this.btnPreview.TabIndex = 106;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // picPleasewait
            // 
            this.picPleasewait.Image = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.InitialImage = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.Location = new System.Drawing.Point(401, 254);
            this.picPleasewait.Name = "picPleasewait";
            this.picPleasewait.Size = new System.Drawing.Size(199, 150);
            this.picPleasewait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPleasewait.TabIndex = 143;
            this.picPleasewait.TabStop = false;
            this.picPleasewait.Visible = false;
            // 
            // ProfitandLoss
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1020, 658);
            this.Controls.Add(this.picPleasewait);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProfitandLoss";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProfitandLoss_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPL)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgrdPL;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox picPleasewait;
        private System.Windows.Forms.Button btnGo;
        protected internal System.Windows.Forms.CheckBox chkDate;
        public System.Windows.Forms.Label Label21;
        private System.Windows.Forms.Button btnDetailView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn leftParticulars;
        private System.Windows.Forms.DataGridViewTextBoxColumn leftAmt;
        private System.Windows.Forms.DataGridViewTextBoxColumn rightParticulars;
        private System.Windows.Forms.DataGridViewTextBoxColumn rightAmt;
        private System.Windows.Forms.MaskedTextBox txtToDate;
        private System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}