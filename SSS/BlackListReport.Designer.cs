namespace SSS
{
    partial class BlackListReport
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
            this.btnClose = new System.Windows.Forms.Button();
            this.forwardingTabs = new System.Windows.Forms.TabControl();
            this.blackListTab = new System.Windows.Forms.TabPage();
            this.dgrdBlackList = new System.Windows.Forms.DataGridView();
            this.sno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.partyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.blackReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transaction = new System.Windows.Forms.TabPage();
            this.dgrdTransactionLock = new System.Windows.Forms.DataGridView();
            this.tSno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tPartyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tUpdatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.forwardingTabs.SuspendLayout();
            this.blackListTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBlackList)).BeginInit();
            this.transaction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdTransactionLock)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(35, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(876, 43);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(279, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(295, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Black List / Transaction Lock Report";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.forwardingTabs);
            this.panel2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(35, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(876, 554);
            this.panel2.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(743, 516);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(109, 34);
            this.btnClose.TabIndex = 101;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // forwardingTabs
            // 
            this.forwardingTabs.Controls.Add(this.blackListTab);
            this.forwardingTabs.Controls.Add(this.transaction);
            this.forwardingTabs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.forwardingTabs.Location = new System.Drawing.Point(16, 10);
            this.forwardingTabs.Name = "forwardingTabs";
            this.forwardingTabs.Padding = new System.Drawing.Point(20, 10);
            this.forwardingTabs.SelectedIndex = 0;
            this.forwardingTabs.Size = new System.Drawing.Size(839, 503);
            this.forwardingTabs.TabIndex = 100;
            // 
            // blackListTab
            // 
            this.blackListTab.Controls.Add(this.dgrdBlackList);
            this.blackListTab.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.blackListTab.Location = new System.Drawing.Point(4, 39);
            this.blackListTab.Name = "blackListTab";
            this.blackListTab.Padding = new System.Windows.Forms.Padding(3);
            this.blackListTab.Size = new System.Drawing.Size(831, 460);
            this.blackListTab.TabIndex = 2;
            this.blackListTab.Text = "Black List Record";
            this.blackListTab.UseVisualStyleBackColor = true;
            // 
            // dgrdBlackList
            // 
            this.dgrdBlackList.AllowUserToAddRows = false;
            this.dgrdBlackList.AllowUserToDeleteRows = false;
            this.dgrdBlackList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdBlackList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdBlackList.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdBlackList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdBlackList.ColumnHeadersHeight = 32;
            this.dgrdBlackList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdBlackList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sno,
            this.partyName,
            this.groupName,
            this.blackReason,
            this.updatedBy});
            this.dgrdBlackList.EnableHeadersVisualStyles = false;
            this.dgrdBlackList.GridColor = System.Drawing.SystemColors.ControlText;
            this.dgrdBlackList.Location = new System.Drawing.Point(17, 19);
            this.dgrdBlackList.Name = "dgrdBlackList";
            this.dgrdBlackList.RowHeadersVisible = false;
            this.dgrdBlackList.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdBlackList.RowTemplate.Height = 25;
            this.dgrdBlackList.Size = new System.Drawing.Size(792, 423);
            this.dgrdBlackList.TabIndex = 106;
            this.dgrdBlackList.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdBlackList_CellBeginEdit);
            // 
            // sno
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.sno.DefaultCellStyle = dataGridViewCellStyle3;
            this.sno.HeaderText = "S.No.";
            this.sno.Name = "sno";
            this.sno.Width = 55;
            // 
            // partyName
            // 
            this.partyName.HeaderText = "Name";
            this.partyName.Name = "partyName";
            this.partyName.Width = 255;
            // 
            // groupName
            // 
            this.groupName.HeaderText = "Group Name";
            this.groupName.Name = "groupName";
            this.groupName.Width = 140;
            // 
            // blackReason
            // 
            this.blackReason.HeaderText = "Reason";
            this.blackReason.Name = "blackReason";
            this.blackReason.Width = 200;
            // 
            // updatedBy
            // 
            this.updatedBy.HeaderText = "Updated By";
            this.updatedBy.Name = "updatedBy";
            this.updatedBy.Width = 115;
            // 
            // transaction
            // 
            this.transaction.Controls.Add(this.dgrdTransactionLock);
            this.transaction.Location = new System.Drawing.Point(4, 39);
            this.transaction.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.transaction.Name = "transaction";
            this.transaction.Padding = new System.Windows.Forms.Padding(3);
            this.transaction.Size = new System.Drawing.Size(831, 460);
            this.transaction.TabIndex = 0;
            this.transaction.Text = "Transaction Lock Record";
            this.transaction.UseVisualStyleBackColor = true;
            // 
            // dgrdTransactionLock
            // 
            this.dgrdTransactionLock.AllowUserToAddRows = false;
            this.dgrdTransactionLock.AllowUserToDeleteRows = false;
            this.dgrdTransactionLock.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdTransactionLock.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdTransactionLock.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdTransactionLock.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdTransactionLock.ColumnHeadersHeight = 32;
            this.dgrdTransactionLock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdTransactionLock.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tSno,
            this.tPartyName,
            this.tGroupName,
            this.tUpdatedBy});
            this.dgrdTransactionLock.EnableHeadersVisualStyles = false;
            this.dgrdTransactionLock.GridColor = System.Drawing.SystemColors.ControlText;
            this.dgrdTransactionLock.Location = new System.Drawing.Point(25, 21);
            this.dgrdTransactionLock.Name = "dgrdTransactionLock";
            this.dgrdTransactionLock.RowHeadersVisible = false;
            this.dgrdTransactionLock.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdTransactionLock.RowTemplate.Height = 25;
            this.dgrdTransactionLock.Size = new System.Drawing.Size(779, 418);
            this.dgrdTransactionLock.TabIndex = 107;
            this.dgrdTransactionLock.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdBlackList_CellBeginEdit);
            // 
            // tSno
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.tSno.DefaultCellStyle = dataGridViewCellStyle6;
            this.tSno.HeaderText = "S. No.";
            this.tSno.Name = "tSno";
            this.tSno.Width = 60;
            // 
            // tPartyName
            // 
            this.tPartyName.HeaderText = "Name";
            this.tPartyName.Name = "tPartyName";
            this.tPartyName.Width = 350;
            // 
            // tGroupName
            // 
            this.tGroupName.HeaderText = "Group Name";
            this.tGroupName.Name = "tGroupName";
            this.tGroupName.Width = 180;
            // 
            // tUpdatedBy
            // 
            this.tUpdatedBy.HeaderText = "Last Updated By";
            this.tUpdatedBy.Name = "tUpdatedBy";
            this.tUpdatedBy.Width = 160;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 250;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Group Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Reasion";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 180;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Last Updated By";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Name";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 300;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Group Name";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 160;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Last Updated By";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 160;
            // 
            // BlackListReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(953, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BlackListReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BlackListReport";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BlackListReport_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.forwardingTabs.ResumeLayout(false);
            this.blackListTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBlackList)).EndInit();
            this.transaction.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdTransactionLock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl forwardingTabs;
        private System.Windows.Forms.TabPage blackListTab;
        private System.Windows.Forms.TabPage transaction;
        private System.Windows.Forms.DataGridView dgrdBlackList;
        private System.Windows.Forms.DataGridView dgrdTransactionLock;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn sno;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn blackReason;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn tSno;
        private System.Windows.Forms.DataGridViewTextBoxColumn tPartyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn tGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn tUpdatedBy;
    }
}