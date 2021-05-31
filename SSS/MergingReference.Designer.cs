namespace SSS
{
    partial class MergingReference
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
            this.MergeParty = new System.Windows.Forms.TabControl();
            this.groupName = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFinalReferenceName = new System.Windows.Forms.TextBox();
            this.txtSReferenceName = new System.Windows.Forms.TextBox();
            this.txtFReferenceName = new System.Windows.Forms.TextBox();
            this.btnSCancel = new System.Windows.Forms.Button();
            this.btnSMerge = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.MergeParty.SuspendLayout();
            this.groupName.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(81, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(835, 46);
            this.panel1.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(305, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "MERGE REFRENCE BOOK";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.MergeParty);
            this.panel2.Location = new System.Drawing.Point(81, 128);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(835, 434);
            this.panel2.TabIndex = 100;
            this.panel2.TabStop = true;
            // 
            // MergeParty
            // 
            this.MergeParty.Controls.Add(this.groupName);
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
            this.groupName.Controls.Add(this.txtFinalReferenceName);
            this.groupName.Controls.Add(this.txtSReferenceName);
            this.groupName.Controls.Add(this.txtFReferenceName);
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
            this.groupName.Text = "Reference Name";
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
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(188, 21);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(177, 18);
            this.label20.TabIndex = 149;
            this.label20.Text = "Merge Reference Name";
            // 
            // txtFinalReferenceName
            // 
            this.txtFinalReferenceName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFinalReferenceName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinalReferenceName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFinalReferenceName.Location = new System.Drawing.Point(263, 191);
            this.txtFinalReferenceName.Name = "txtFinalReferenceName";
            this.txtFinalReferenceName.ReadOnly = true;
            this.txtFinalReferenceName.Size = new System.Drawing.Size(382, 23);
            this.txtFinalReferenceName.TabIndex = 104;
            this.txtFinalReferenceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFinalSalesParty_KeyDown);
            // 
            // txtSReferenceName
            // 
            this.txtSReferenceName.BackColor = System.Drawing.SystemColors.Window;
            this.txtSReferenceName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSReferenceName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSReferenceName.Location = new System.Drawing.Point(264, 151);
            this.txtSReferenceName.Name = "txtSReferenceName";
            this.txtSReferenceName.ReadOnly = true;
            this.txtSReferenceName.Size = new System.Drawing.Size(381, 23);
            this.txtSReferenceName.TabIndex = 103;
            this.txtSReferenceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSSalesParty_KeyDown);
            // 
            // txtFReferenceName
            // 
            this.txtFReferenceName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFReferenceName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFReferenceName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFReferenceName.Location = new System.Drawing.Point(263, 108);
            this.txtFReferenceName.Name = "txtFReferenceName";
            this.txtFReferenceName.ReadOnly = true;
            this.txtFReferenceName.Size = new System.Drawing.Size(382, 23);
            this.txtFReferenceName.TabIndex = 102;
            this.txtFReferenceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFSalesParty_KeyDown);
            // 
            // btnSCancel
            // 
            this.btnSCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSCancel.ForeColor = System.Drawing.Color.White;
            this.btnSCancel.Location = new System.Drawing.Point(495, 250);
            this.btnSCancel.Name = "btnSCancel";
            this.btnSCancel.Size = new System.Drawing.Size(149, 40);
            this.btnSCancel.TabIndex = 106;
            this.btnSCancel.Text = "&Close";
            this.btnSCancel.UseVisualStyleBackColor = false;
            this.btnSCancel.Click += new System.EventHandler(this.btnSCancel_Click);
            // 
            // btnSMerge
            // 
            this.btnSMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSMerge.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSMerge.ForeColor = System.Drawing.Color.White;
            this.btnSMerge.Location = new System.Drawing.Point(265, 250);
            this.btnSMerge.Name = "btnSMerge";
            this.btnSMerge.Size = new System.Drawing.Size(229, 40);
            this.btnSMerge.TabIndex = 105;
            this.btnSMerge.Text = "&Merge Reference Name";
            this.btnSMerge.UseVisualStyleBackColor = false;
            this.btnSMerge.Click += new System.EventHandler(this.btnSMerge_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(89, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(171, 16);
            this.label7.TabIndex = 158;
            this.label7.Text = "Final Reference Name :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(73, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 16);
            this.label5.TabIndex = 157;
            this.label5.Text = "Second Reference Name :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(94, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 16);
            this.label4.TabIndex = 156;
            this.label4.Text = "First Reference Name :";
            // 
            // MergingReference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(998, 650);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MergingReference";
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl MergeParty;
        private System.Windows.Forms.TabPage groupName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFinalReferenceName;
        private System.Windows.Forms.TextBox txtSReferenceName;
        private System.Windows.Forms.TextBox txtFReferenceName;
        private System.Windows.Forms.Button btnSCancel;
        private System.Windows.Forms.Button btnSMerge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label20;
    }
}