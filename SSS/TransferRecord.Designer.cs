namespace SSS
{
    partial class TransferRecord
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
            this.forwardingDataTabs = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkPendingOrder = new System.Windows.Forms.CheckBox();
            this.chkItemMaster = new System.Windows.Forms.CheckBox();
            this.chkStock = new System.Windows.Forms.CheckBox();
            this.chkParty = new System.Windows.Forms.CheckBox();
            this.chkAccount = new System.Windows.Forms.CheckBox();
            this.chkBalance = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.forwardingTabs.SuspendLayout();
            this.forwardingDataTabs.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(33, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(869, 46);
            this.panel1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(373, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "DATA TRANSFER";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblUpdatedTime);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Location = new System.Drawing.Point(33, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(869, 522);
            this.panel2.TabIndex = 8;
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
            this.panel4.Location = new System.Drawing.Point(36, 29);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(791, 456);
            this.panel4.TabIndex = 0;
            // 
            // forwardingTabs
            // 
            this.forwardingTabs.Controls.Add(this.forwardingDataTabs);
            this.forwardingTabs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.forwardingTabs.Location = new System.Drawing.Point(30, 16);
            this.forwardingTabs.Name = "forwardingTabs";
            this.forwardingTabs.Padding = new System.Drawing.Point(20, 7);
            this.forwardingTabs.SelectedIndex = 0;
            this.forwardingTabs.Size = new System.Drawing.Size(727, 413);
            this.forwardingTabs.TabIndex = 99;
            // 
            // forwardingDataTabs
            // 
            this.forwardingDataTabs.Controls.Add(this.label3);
            this.forwardingDataTabs.Controls.Add(this.panel3);
            this.forwardingDataTabs.Controls.Add(this.btnCancel);
            this.forwardingDataTabs.Controls.Add(this.btnSubmit);
            this.forwardingDataTabs.Location = new System.Drawing.Point(4, 33);
            this.forwardingDataTabs.Name = "forwardingDataTabs";
            this.forwardingDataTabs.Padding = new System.Windows.Forms.Padding(3);
            this.forwardingDataTabs.Size = new System.Drawing.Size(719, 376);
            this.forwardingDataTabs.TabIndex = 1;
            this.forwardingDataTabs.Text = "Forward Data to Next Finanical Year";
            this.forwardingDataTabs.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(226, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 16);
            this.label3.TabIndex = 148;
            this.label3.Text = "Forward to Next Finanical Year";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Tan;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.chkPendingOrder);
            this.panel3.Controls.Add(this.chkItemMaster);
            this.panel3.Controls.Add(this.chkStock);
            this.panel3.Controls.Add(this.chkParty);
            this.panel3.Controls.Add(this.chkAccount);
            this.panel3.Controls.Add(this.chkBalance);
            this.panel3.Location = new System.Drawing.Point(122, 65);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(455, 239);
            this.panel3.TabIndex = 99;
            // 
            // chkPendingOrder
            // 
            this.chkPendingOrder.AutoSize = true;
            this.chkPendingOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPendingOrder.Enabled = false;
            this.chkPendingOrder.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkPendingOrder.Location = new System.Drawing.Point(122, 198);
            this.chkPendingOrder.Name = "chkPendingOrder";
            this.chkPendingOrder.Size = new System.Drawing.Size(198, 22);
            this.chkPendingOrder.TabIndex = 105;
            this.chkPendingOrder.Text = " Forward Pending Order";
            this.chkPendingOrder.UseVisualStyleBackColor = true;
            // 
            // chkItemMaster
            // 
            this.chkItemMaster.AutoSize = true;
            this.chkItemMaster.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkItemMaster.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkItemMaster.Location = new System.Drawing.Point(124, 92);
            this.chkItemMaster.Name = "chkItemMaster";
            this.chkItemMaster.Size = new System.Drawing.Size(176, 22);
            this.chkItemMaster.TabIndex = 102;
            this.chkItemMaster.Text = " Forward Item Master";
            this.chkItemMaster.UseVisualStyleBackColor = true;
            // 
            // chkStock
            // 
            this.chkStock.AutoSize = true;
            this.chkStock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkStock.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkStock.Location = new System.Drawing.Point(123, 161);
            this.chkStock.Name = "chkStock";
            this.chkStock.Size = new System.Drawing.Size(191, 22);
            this.chkStock.TabIndex = 104;
            this.chkStock.Text = " Forward Closing Stock";
            this.chkStock.UseVisualStyleBackColor = true;
            // 
            // chkParty
            // 
            this.chkParty.AutoSize = true;
            this.chkParty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkParty.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkParty.Location = new System.Drawing.Point(124, 57);
            this.chkParty.Name = "chkParty";
            this.chkParty.Size = new System.Drawing.Size(202, 22);
            this.chkParty.TabIndex = 101;
            this.chkParty.Text = " Forward Account Master";
            this.chkParty.UseVisualStyleBackColor = true;
            // 
            // chkAccount
            // 
            this.chkAccount.AutoSize = true;
            this.chkAccount.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAccount.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkAccount.Location = new System.Drawing.Point(124, 126);
            this.chkAccount.Name = "chkAccount";
            this.chkAccount.Size = new System.Drawing.Size(187, 22);
            this.chkAccount.TabIndex = 103;
            this.chkAccount.Text = " Forward Other Master";
            this.chkAccount.UseVisualStyleBackColor = true;
            // 
            // chkBalance
            // 
            this.chkBalance.AutoSize = true;
            this.chkBalance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBalance.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.chkBalance.Location = new System.Drawing.Point(124, 19);
            this.chkBalance.Name = "chkBalance";
            this.chkBalance.Size = new System.Drawing.Size(196, 22);
            this.chkBalance.TabIndex = 100;
            this.chkBalance.Text = " Forward Balance Sheet";
            this.chkBalance.UseVisualStyleBackColor = true;
            this.chkBalance.CheckedChanged += new System.EventHandler(this.chkBalance_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(361, 318);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 41);
            this.btnCancel.TabIndex = 107;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSubmit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(208, 318);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(154, 41);
            this.btnSubmit.TabIndex = 106;
            this.btnSubmit.Text = "&Forward";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
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
            // TransferRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(946, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "TransferRecord";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Record";
            this.Load += new System.EventHandler(this.TransferRecord_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransferRecord_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.forwardingTabs.ResumeLayout(false);
            this.forwardingDataTabs.ResumeLayout(false);
            this.forwardingDataTabs.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lblUpdatedTime;
        private System.Windows.Forms.TabControl forwardingTabs;
        private System.Windows.Forms.TabPage forwardingDataTabs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chkStock;
        private System.Windows.Forms.CheckBox chkParty;
        private System.Windows.Forms.CheckBox chkAccount;
        private System.Windows.Forms.CheckBox chkBalance;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.CheckBox chkItemMaster;
        private System.Windows.Forms.CheckBox chkPendingOrder;
    }
}