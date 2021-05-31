namespace SSS
{
    partial class SendInternet
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
            this.lblUpdatedTime = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.forwardingTabs = new System.Windows.Forms.TabControl();
            this.sendTab = new System.Windows.Forms.TabPage();
            this.btnSendCancel = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.forwardingTabs.SuspendLayout();
            this.sendTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(58, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(778, 46);
            this.panel1.TabIndex = 200;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(258, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "DATA TRANSFER TO INTERNET";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblUpdatedTime);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Location = new System.Drawing.Point(56, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(780, 502);
            this.panel2.TabIndex = 100;
            this.panel2.TabStop = true;
            // 
            // lblUpdatedTime
            // 
            this.lblUpdatedTime.AutoSize = true;
            this.lblUpdatedTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblUpdatedTime.Location = new System.Drawing.Point(9, 453);
            this.lblUpdatedTime.Name = "lblUpdatedTime";
            this.lblUpdatedTime.Size = new System.Drawing.Size(20, 16);
            this.lblUpdatedTime.TabIndex = 148;
            this.lblUpdatedTime.Text = "   ";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.forwardingTabs);
            this.panel4.Controls.Add(this.lblMsg);
            this.panel4.Controls.Add(this.label25);
            this.panel4.Location = new System.Drawing.Point(52, 43);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(677, 416);
            this.panel4.TabIndex = 101;
            this.panel4.TabStop = true;
            // 
            // forwardingTabs
            // 
            this.forwardingTabs.Controls.Add(this.sendTab);
            this.forwardingTabs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.forwardingTabs.Location = new System.Drawing.Point(37, 27);
            this.forwardingTabs.Name = "forwardingTabs";
            this.forwardingTabs.Padding = new System.Drawing.Point(25, 10);
            this.forwardingTabs.SelectedIndex = 0;
            this.forwardingTabs.Size = new System.Drawing.Size(598, 346);
            this.forwardingTabs.TabIndex = 102;
            this.forwardingTabs.TabStop = false;
            // 
            // sendTab
            // 
            this.sendTab.Controls.Add(this.btnSendCancel);
            this.sendTab.Controls.Add(this.btnSend);
            this.sendTab.Location = new System.Drawing.Point(4, 39);
            this.sendTab.Name = "sendTab";
            this.sendTab.Padding = new System.Windows.Forms.Padding(3);
            this.sendTab.Size = new System.Drawing.Size(590, 303);
            this.sendTab.TabIndex = 2;
            this.sendTab.Text = "Send Data to Cloud";
            this.sendTab.UseVisualStyleBackColor = true;
            // 
            // btnSendCancel
            // 
            this.btnSendCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSendCancel.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnSendCancel.ForeColor = System.Drawing.Color.White;
            this.btnSendCancel.Location = new System.Drawing.Point(181, 174);
            this.btnSendCancel.Name = "btnSendCancel";
            this.btnSendCancel.Size = new System.Drawing.Size(236, 47);
            this.btnSendCancel.TabIndex = 105;
            this.btnSendCancel.TabStop = false;
            this.btnSendCancel.Text = "&Close";
            this.btnSendCancel.UseVisualStyleBackColor = false;
            this.btnSendCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSend.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(181, 97);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(236, 50);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "&Send Data to Cloud";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMsg.Location = new System.Drawing.Point(216, 5);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(8, 11);
            this.lblMsg.TabIndex = 98;
            this.lblMsg.Text = " ";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label25.Location = new System.Drawing.Point(191, 11);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(8, 11);
            this.label25.TabIndex = 86;
            this.label25.Text = " ";
            // 
            // SendInternet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(898, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SendInternet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Record";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransferRecord_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.forwardingTabs.ResumeLayout(false);
            this.sendTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TabControl forwardingTabs;
        private System.Windows.Forms.Label lblUpdatedTime;
        private System.Windows.Forms.TabPage sendTab;
        private System.Windows.Forms.Button btnSendCancel;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label1;
    }
}