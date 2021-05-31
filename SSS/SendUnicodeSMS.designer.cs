namespace SSS
{
    partial class SendUnicodeSMS
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblSMSCount = new System.Windows.Forms.Label();
            this.lblCharCount = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoHindi = new System.Windows.Forms.RadioButton();
            this.rdoEnglish = new System.Windows.Forms.RadioButton();
            this.txtMobileNo = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSMS = new System.Windows.Forms.Button();
            this.txtSMS = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(950, 45);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(396, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "SENDING SMS";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblSMSCount);
            this.panel2.Controls.Add(this.lblCharCount);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.txtMobileNo);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSMS);
            this.panel2.Controls.Add(this.txtSMS);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(25, 90);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(950, 538);
            this.panel2.TabIndex = 101;
            // 
            // lblSMSCount
            // 
            this.lblSMSCount.AutoSize = true;
            this.lblSMSCount.Location = new System.Drawing.Point(542, 475);
            this.lblSMSCount.Name = "lblSMSCount";
            this.lblSMSCount.Size = new System.Drawing.Size(16, 16);
            this.lblSMSCount.TabIndex = 125;
            this.lblSMSCount.Text = "  ";
            // 
            // lblCharCount
            // 
            this.lblCharCount.AutoSize = true;
            this.lblCharCount.Location = new System.Drawing.Point(391, 476);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new System.Drawing.Size(16, 16);
            this.lblCharCount.TabIndex = 124;
            this.lblCharCount.Text = "  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoHindi);
            this.groupBox1.Controls.Add(this.rdoEnglish);
            this.groupBox1.Location = new System.Drawing.Point(126, 455);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 53);
            this.groupBox1.TabIndex = 104;
            this.groupBox1.TabStop = false;
            // 
            // rdoHindi
            // 
            this.rdoHindi.AutoSize = true;
            this.rdoHindi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoHindi.Location = new System.Drawing.Point(119, 21);
            this.rdoHindi.Name = "rdoHindi";
            this.rdoHindi.Size = new System.Drawing.Size(59, 20);
            this.rdoHindi.TabIndex = 106;
            this.rdoHindi.Text = "Hindi";
            this.rdoHindi.UseVisualStyleBackColor = true;
            // 
            // rdoEnglish
            // 
            this.rdoEnglish.AutoSize = true;
            this.rdoEnglish.Checked = true;
            this.rdoEnglish.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoEnglish.Location = new System.Drawing.Point(23, 20);
            this.rdoEnglish.Name = "rdoEnglish";
            this.rdoEnglish.Size = new System.Drawing.Size(72, 20);
            this.rdoEnglish.TabIndex = 105;
            this.rdoEnglish.TabStop = true;
            this.rdoEnglish.Text = "English";
            this.rdoEnglish.UseVisualStyleBackColor = true;
            // 
            // txtMobileNo
            // 
            this.txtMobileNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMobileNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMobileNo.Location = new System.Drawing.Point(126, 26);
            this.txtMobileNo.Name = "txtMobileNo";
            this.txtMobileNo.Size = new System.Drawing.Size(789, 163);
            this.txtMobileNo.TabIndex = 102;
            this.txtMobileNo.Text = "";
            this.txtMobileNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMobileNo_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(23, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 16);
            this.label9.TabIndex = 123;
            this.label9.Text = "Mobile No  :";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(822, 469);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(103, 39);
            this.btnClose.TabIndex = 108;
            this.btnClose.Text = "Cl&ose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSMS
            // 
            this.btnSMS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSMS.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSMS.ForeColor = System.Drawing.Color.White;
            this.btnSMS.Location = new System.Drawing.Point(680, 469);
            this.btnSMS.Name = "btnSMS";
            this.btnSMS.Size = new System.Drawing.Size(128, 39);
            this.btnSMS.TabIndex = 107;
            this.btnSMS.Text = "&Send SMS";
            this.btnSMS.UseVisualStyleBackColor = false;
            this.btnSMS.Click += new System.EventHandler(this.btnSMS_Click);
            // 
            // txtSMS
            // 
            this.txtSMS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSMS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSMS.Location = new System.Drawing.Point(126, 213);
            this.txtSMS.Name = "txtSMS";
            this.txtSMS.Size = new System.Drawing.Size(789, 224);
            this.txtSMS.TabIndex = 103;
            this.txtSMS.Text = "";
            this.txtSMS.TextChanged += new System.EventHandler(this.txtSMS_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(32, 220);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 118;
            this.label6.Text = "Message  :";
            // 
            // SendUnicodeSMS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SendUnicodeSMS";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Send Unicode SMS";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SendUnicodeSMS_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox txtMobileNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSMS;
        private System.Windows.Forms.RichTextBox txtSMS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoHindi;
        private System.Windows.Forms.RadioButton rdoEnglish;
        private System.Windows.Forms.Label lblSMSCount;
        private System.Windows.Forms.Label lblCharCount;
    }
}