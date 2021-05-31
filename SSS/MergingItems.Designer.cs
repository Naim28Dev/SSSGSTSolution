namespace SSS
{
    partial class MergingItems
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.MergeParty = new System.Windows.Forms.TabControl();
            this.groupName = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFinalGroupName = new System.Windows.Forms.TextBox();
            this.txtSGroupName = new System.Windows.Forms.TextBox();
            this.txtFGroupName = new System.Windows.Forms.TextBox();
            this.btnSCancel = new System.Windows.Forms.Button();
            this.btnSMerge = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.itemName = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFinalItemName = new System.Windows.Forms.TextBox();
            this.txtSItemName = new System.Windows.Forms.TextBox();
            this.txtFItemName = new System.Windows.Forms.TextBox();
            this.btnPClose = new System.Windows.Forms.Button();
            this.btnPMerge = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.MergeParty.SuspendLayout();
            this.groupName.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.itemName.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(51, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(835, 46);
            this.panel1.TabIndex = 13;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.MergeParty);
            this.panel2.Location = new System.Drawing.Point(51, 128);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(835, 434);
            this.panel2.TabIndex = 101;
            // 
            // MergeParty
            // 
            this.MergeParty.Controls.Add(this.groupName);
            this.MergeParty.Controls.Add(this.itemName);
            this.MergeParty.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MergeParty.Location = new System.Drawing.Point(48, 26);
            this.MergeParty.Name = "MergeParty";
            this.MergeParty.Padding = new System.Drawing.Point(50, 10);
            this.MergeParty.SelectedIndex = 0;
            this.MergeParty.Size = new System.Drawing.Size(744, 364);
            this.MergeParty.TabIndex = 102;
            // 
            // groupName
            // 
            this.groupName.Controls.Add(this.groupBox1);
            this.groupName.Controls.Add(this.txtFinalGroupName);
            this.groupName.Controls.Add(this.txtSGroupName);
            this.groupName.Controls.Add(this.txtFGroupName);
            this.groupName.Controls.Add(this.btnSCancel);
            this.groupName.Controls.Add(this.btnSMerge);
            this.groupName.Controls.Add(this.label7);
            this.groupName.Controls.Add(this.label5);
            this.groupName.Controls.Add(this.label4);
            this.groupName.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupName.Location = new System.Drawing.Point(4, 39);
            this.groupName.Name = "groupName";
            this.groupName.Padding = new System.Windows.Forms.Padding(3);
            this.groupName.Size = new System.Drawing.Size(736, 321);
            this.groupName.TabIndex = 0;
            this.groupName.Text = "Group Name";
            this.groupName.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Location = new System.Drawing.Point(93, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(552, 53);
            this.groupBox1.TabIndex = 159;
            this.groupBox1.TabStop = false;
            // 
            // txtFinalGroupName
            // 
            this.txtFinalGroupName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFinalGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinalGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFinalGroupName.Location = new System.Drawing.Point(263, 191);
            this.txtFinalGroupName.Name = "txtFinalGroupName";
            this.txtFinalGroupName.ReadOnly = true;
            this.txtFinalGroupName.Size = new System.Drawing.Size(382, 23);
            this.txtFinalGroupName.TabIndex = 153;
            this.txtFinalGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFinalSalesParty_KeyDown);
            // 
            // txtSGroupName
            // 
            this.txtSGroupName.BackColor = System.Drawing.SystemColors.Window;
            this.txtSGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSGroupName.Location = new System.Drawing.Point(264, 151);
            this.txtSGroupName.Name = "txtSGroupName";
            this.txtSGroupName.ReadOnly = true;
            this.txtSGroupName.Size = new System.Drawing.Size(381, 23);
            this.txtSGroupName.TabIndex = 152;
            this.txtSGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSSalesParty_KeyDown);
            // 
            // txtFGroupName
            // 
            this.txtFGroupName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFGroupName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFGroupName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFGroupName.Location = new System.Drawing.Point(263, 108);
            this.txtFGroupName.Name = "txtFGroupName";
            this.txtFGroupName.ReadOnly = true;
            this.txtFGroupName.Size = new System.Drawing.Size(382, 23);
            this.txtFGroupName.TabIndex = 151;
            this.txtFGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFSalesParty_KeyDown);
            // 
            // btnSCancel
            // 
            this.btnSCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSCancel.ForeColor = System.Drawing.Color.White;
            this.btnSCancel.Location = new System.Drawing.Point(473, 250);
            this.btnSCancel.Name = "btnSCancel";
            this.btnSCancel.Size = new System.Drawing.Size(172, 40);
            this.btnSCancel.TabIndex = 155;
            this.btnSCancel.Text = "&Close";
            this.btnSCancel.UseVisualStyleBackColor = false;
            this.btnSCancel.Click += new System.EventHandler(this.btnSCancel_Click);
            // 
            // btnSMerge
            // 
            this.btnSMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSMerge.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSMerge.ForeColor = System.Drawing.Color.White;
            this.btnSMerge.Location = new System.Drawing.Point(217, 250);
            this.btnSMerge.Name = "btnSMerge";
            this.btnSMerge.Size = new System.Drawing.Size(252, 40);
            this.btnSMerge.TabIndex = 154;
            this.btnSMerge.Text = "&Merge Item Group Name";
            this.btnSMerge.UseVisualStyleBackColor = false;
            this.btnSMerge.Click += new System.EventHandler(this.btnSMerge_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(117, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(141, 16);
            this.label7.TabIndex = 158;
            this.label7.Text = "Final Group Name :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(98, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 16);
            this.label5.TabIndex = 157;
            this.label5.Text = "Second Group Name :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(118, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 16);
            this.label4.TabIndex = 156;
            this.label4.Text = "First Group Name :";
            // 
            // itemName
            // 
            this.itemName.Controls.Add(this.groupBox2);
            this.itemName.Controls.Add(this.txtFinalItemName);
            this.itemName.Controls.Add(this.txtSItemName);
            this.itemName.Controls.Add(this.txtFItemName);
            this.itemName.Controls.Add(this.btnPClose);
            this.itemName.Controls.Add(this.btnPMerge);
            this.itemName.Controls.Add(this.label3);
            this.itemName.Controls.Add(this.label6);
            this.itemName.Controls.Add(this.label8);
            this.itemName.Location = new System.Drawing.Point(4, 39);
            this.itemName.Name = "itemName";
            this.itemName.Padding = new System.Windows.Forms.Padding(3);
            this.itemName.Size = new System.Drawing.Size(736, 321);
            this.itemName.TabIndex = 1;
            this.itemName.Text = "Item Name";
            this.itemName.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(93, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 53);
            this.groupBox2.TabIndex = 159;
            this.groupBox2.TabStop = false;
            // 
            // txtFinalItemName
            // 
            this.txtFinalItemName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFinalItemName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinalItemName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFinalItemName.Location = new System.Drawing.Point(250, 190);
            this.txtFinalItemName.Name = "txtFinalItemName";
            this.txtFinalItemName.ReadOnly = true;
            this.txtFinalItemName.Size = new System.Drawing.Size(395, 23);
            this.txtFinalItemName.TabIndex = 153;
            this.txtFinalItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFinalPurchaseParty_KeyDown);
            // 
            // txtSItemName
            // 
            this.txtSItemName.BackColor = System.Drawing.SystemColors.Window;
            this.txtSItemName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSItemName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSItemName.Location = new System.Drawing.Point(250, 150);
            this.txtSItemName.Name = "txtSItemName";
            this.txtSItemName.ReadOnly = true;
            this.txtSItemName.Size = new System.Drawing.Size(395, 23);
            this.txtSItemName.TabIndex = 152;
            this.txtSItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSPurchaseParty_KeyDown);
            // 
            // txtFItemName
            // 
            this.txtFItemName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFItemName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFItemName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFItemName.Location = new System.Drawing.Point(250, 107);
            this.txtFItemName.Name = "txtFItemName";
            this.txtFItemName.ReadOnly = true;
            this.txtFItemName.Size = new System.Drawing.Size(395, 23);
            this.txtFItemName.TabIndex = 151;
            this.txtFItemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFPurchaseParty_KeyDown);
            // 
            // btnPClose
            // 
            this.btnPClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPClose.ForeColor = System.Drawing.Color.White;
            this.btnPClose.Location = new System.Drawing.Point(467, 250);
            this.btnPClose.Name = "btnPClose";
            this.btnPClose.Size = new System.Drawing.Size(177, 40);
            this.btnPClose.TabIndex = 155;
            this.btnPClose.Text = "&Close";
            this.btnPClose.UseVisualStyleBackColor = false;
            this.btnPClose.Click += new System.EventHandler(this.btnPClose_Click);
            // 
            // btnPMerge
            // 
            this.btnPMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnPMerge.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnPMerge.ForeColor = System.Drawing.Color.White;
            this.btnPMerge.Location = new System.Drawing.Point(217, 250);
            this.btnPMerge.Name = "btnPMerge";
            this.btnPMerge.Size = new System.Drawing.Size(249, 40);
            this.btnPMerge.TabIndex = 154;
            this.btnPMerge.Text = "&Merge Item Name";
            this.btnPMerge.UseVisualStyleBackColor = false;
            this.btnPMerge.Click += new System.EventHandler(this.btnPMerge_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(120, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 16);
            this.label3.TabIndex = 158;
            this.label3.Text = "Final Item Name :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(102, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 16);
            this.label6.TabIndex = 157;
            this.label6.Text = "Second Item Name :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(120, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 16);
            this.label8.TabIndex = 156;
            this.label8.Text = "First  Item Name :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(275, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "MERGE ITEMS AND GROUP NAME";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(186, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(181, 18);
            this.label20.TabIndex = 148;
            this.label20.Text = "Merge Item Group Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(210, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 18);
            this.label2.TabIndex = 149;
            this.label2.Text = "Merge Item Name";
            // 
            // MergingItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(950, 610);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MergingItems";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Merging Party";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MergingParty_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.MergeParty.ResumeLayout(false);
            this.groupName.ResumeLayout(false);
            this.groupName.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.itemName.ResumeLayout(false);
            this.itemName.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl MergeParty;
        private System.Windows.Forms.TabPage groupName;
        private System.Windows.Forms.TabPage itemName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFinalGroupName;
        private System.Windows.Forms.TextBox txtSGroupName;
        private System.Windows.Forms.TextBox txtFGroupName;
        private System.Windows.Forms.Button btnSCancel;
        private System.Windows.Forms.Button btnSMerge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtFinalItemName;
        private System.Windows.Forms.TextBox txtSItemName;
        private System.Windows.Forms.TextBox txtFItemName;
        private System.Windows.Forms.Button btnPClose;
        private System.Windows.Forms.Button btnPMerge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label2;
    }
}