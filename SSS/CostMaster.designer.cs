namespace SSS
{
    partial class CostMaster
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
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtSearchCostName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.lboxCost = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtCostType = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(112, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(775, 46);
            this.panel1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(309, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cost Master";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Location = new System.Drawing.Point(112, 165);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(775, 389);
            this.panel2.TabIndex = 8;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label19.Location = new System.Drawing.Point(575, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(113, 18);
            this.label19.TabIndex = 132;
            this.label19.Text = "Required Field";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label20.Location = new System.Drawing.Point(551, 13);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(17, 22);
            this.label20.TabIndex = 131;
            this.label20.Text = "*";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.txtSearchCostName);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.btnDelete);
            this.panel4.Controls.Add(this.lblMsg);
            this.panel4.Controls.Add(this.lboxCost);
            this.panel4.Controls.Add(this.btnCancel);
            this.panel4.Controls.Add(this.txtCostType);
            this.panel4.Controls.Add(this.btnSubmit);
            this.panel4.Controls.Add(this.label30);
            this.panel4.Controls.Add(this.label32);
            this.panel4.Location = new System.Drawing.Point(34, 45);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(704, 300);
            this.panel4.TabIndex = 0;
            // 
            // txtSearchCostName
            // 
            this.txtSearchCostName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearchCostName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearchCostName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchCostName.Location = new System.Drawing.Point(467, 10);
            this.txtSearchCostName.Name = "txtSearchCostName";
            this.txtSearchCostName.Size = new System.Drawing.Size(219, 22);
            this.txtSearchCostName.TabIndex = 26;
            this.txtSearchCostName.Visible = false;
            this.txtSearchCostName.TextChanged += new System.EventHandler(this.txtSearchStation_TextChanged);
            this.txtSearchCostName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchStation_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label2.Location = new System.Drawing.Point(404, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 22);
            this.label2.TabIndex = 132;
            this.label2.Text = "*";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(293, 156);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(103, 42);
            this.btnDelete.TabIndex = 87;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMsg.Location = new System.Drawing.Point(144, 61);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(8, 11);
            this.lblMsg.TabIndex = 86;
            this.lblMsg.Text = " ";
            // 
            // lboxCost
            // 
            this.lboxCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lboxCost.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxCost.FormattingEnabled = true;
            this.lboxCost.HorizontalScrollbar = true;
            this.lboxCost.ItemHeight = 16;
            this.lboxCost.Location = new System.Drawing.Point(467, 36);
            this.lboxCost.Name = "lboxCost";
            this.lboxCost.Size = new System.Drawing.Size(219, 258);
            this.lboxCost.Sorted = true;
            this.lboxCost.TabIndex = 27;
            this.lboxCost.SelectedIndexChanged += new System.EventHandler(this.lboxStation_SelectedIndexChanged);
            this.lboxCost.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lboxStation_KeyPress);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(179, 156);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 42);
            this.btnCancel.TabIndex = 39;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtCostType
            // 
            this.txtCostType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCostType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCostType.Location = new System.Drawing.Point(168, 95);
            this.txtCostType.Name = "txtCostType";
            this.txtCostType.Size = new System.Drawing.Size(233, 22);
            this.txtCostType.TabIndex = 28;
            this.txtCostType.TextChanged += new System.EventHandler(this.txtStation_TextChanged);
            this.txtCostType.Leave += new System.EventHandler(this.txtStation_Leave);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSubmit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(67, 156);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(103, 42);
            this.btnSubmit.TabIndex = 38;
            this.btnSubmit.Text = "&Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(10, 98);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(122, 16);
            this.label30.TabIndex = 11;
            this.label30.Text = "Cost Centre Name";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(153, 99);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(10, 15);
            this.label32.TabIndex = 12;
            this.label32.Text = ":";
            // 
            // CostMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "CostMaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.StationMaster_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StationMaster_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.ListBox lboxCost;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtCostType;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtSearchCostName;
        private System.Windows.Forms.Label label2;
    }
}