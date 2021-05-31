namespace SSS
{
    partial class Profit_Margin
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rdoBrandWise = new System.Windows.Forms.RadioButton();
            this.txtBrandWise = new System.Windows.Forms.TextBox();
            this.rdoItemWise = new System.Windows.Forms.RadioButton();
            this.rdoPurchaseBill = new System.Windows.Forms.RadioButton();
            this.rdoFix = new System.Windows.Forms.RadioButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPurchaseBillwise = new System.Windows.Forms.TextBox();
            this.txtFixedProfit = new System.Windows.Forms.TextBox();
            this.txtItemWise = new System.Windows.Forms.TextBox();
            this.panColor = new System.Windows.Forms.Panel();
            this.panHeader = new System.Windows.Forms.Panel();
            this.lblNameHeader = new System.Windows.Forms.Label();
            this.rdoDesignMaster = new System.Windows.Forms.RadioButton();
            this.panel5.SuspendLayout();
            this.panColor.SuspendLayout();
            this.panHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn1.HeaderText = "          WEAVE  NAME";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 15;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 180;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "id";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.LightGray;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.rdoDesignMaster);
            this.panel5.Controls.Add(this.rdoBrandWise);
            this.panel5.Controls.Add(this.txtBrandWise);
            this.panel5.Controls.Add(this.rdoItemWise);
            this.panel5.Controls.Add(this.rdoPurchaseBill);
            this.panel5.Controls.Add(this.rdoFix);
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnSubmit);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.txtPurchaseBillwise);
            this.panel5.Controls.Add(this.txtFixedProfit);
            this.panel5.Controls.Add(this.txtItemWise);
            this.panel5.Location = new System.Drawing.Point(39, 39);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(878, 453);
            this.panel5.TabIndex = 100;
            this.panel5.TabStop = true;
            // 
            // rdoBrandWise
            // 
            this.rdoBrandWise.AutoSize = true;
            this.rdoBrandWise.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.rdoBrandWise.Location = new System.Drawing.Point(305, 142);
            this.rdoBrandWise.Name = "rdoBrandWise";
            this.rdoBrandWise.Size = new System.Drawing.Size(112, 20);
            this.rdoBrandWise.TabIndex = 103;
            this.rdoBrandWise.TabStop = true;
            this.rdoBrandWise.Text = "&Brand wise :";
            this.rdoBrandWise.UseVisualStyleBackColor = true;
            this.rdoBrandWise.CheckedChanged += new System.EventHandler(this.rdoBrandWise_CheckedChanged);
            // 
            // txtBrandWise
            // 
            this.txtBrandWise.Enabled = false;
            this.txtBrandWise.Font = new System.Drawing.Font("Arial", 11F);
            this.txtBrandWise.Location = new System.Drawing.Point(420, 141);
            this.txtBrandWise.Name = "txtBrandWise";
            this.txtBrandWise.Size = new System.Drawing.Size(135, 24);
            this.txtBrandWise.TabIndex = 104;
            this.txtBrandWise.Text = "0";
            this.txtBrandWise.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBrandWise.Enter += new System.EventHandler(this.txtFixedProfit_Enter);
            this.txtBrandWise.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFixedProfit_KeyPress);
            this.txtBrandWise.Leave += new System.EventHandler(this.txtFixedProfit_Leave);
            // 
            // rdoItemWise
            // 
            this.rdoItemWise.AutoSize = true;
            this.rdoItemWise.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.rdoItemWise.Location = new System.Drawing.Point(291, 182);
            this.rdoItemWise.Name = "rdoItemWise";
            this.rdoItemWise.Size = new System.Drawing.Size(126, 20);
            this.rdoItemWise.TabIndex = 105;
            this.rdoItemWise.TabStop = true;
            this.rdoItemWise.Text = "&Item Bill wise :";
            this.rdoItemWise.UseVisualStyleBackColor = true;
            this.rdoItemWise.CheckedChanged += new System.EventHandler(this.rdoItemWise_CheckedChanged);
            // 
            // rdoPurchaseBill
            // 
            this.rdoPurchaseBill.AutoSize = true;
            this.rdoPurchaseBill.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.rdoPurchaseBill.Location = new System.Drawing.Point(255, 226);
            this.rdoPurchaseBill.Name = "rdoPurchaseBill";
            this.rdoPurchaseBill.Size = new System.Drawing.Size(162, 20);
            this.rdoPurchaseBill.TabIndex = 107;
            this.rdoPurchaseBill.TabStop = true;
            this.rdoPurchaseBill.Text = "&Purchase Bill wise :";
            this.rdoPurchaseBill.UseVisualStyleBackColor = true;
            this.rdoPurchaseBill.CheckedChanged += new System.EventHandler(this.rdoPurchaseBill_CheckedChanged);
            // 
            // rdoFix
            // 
            this.rdoFix.AutoSize = true;
            this.rdoFix.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.rdoFix.Location = new System.Drawing.Point(302, 103);
            this.rdoFix.Name = "rdoFix";
            this.rdoFix.Size = new System.Drawing.Size(115, 20);
            this.rdoFix.TabIndex = 101;
            this.rdoFix.TabStop = true;
            this.rdoFix.Text = "&Fixed Profit :";
            this.rdoFix.UseVisualStyleBackColor = true;
            this.rdoFix.CheckedChanged += new System.EventHandler(this.rdoFix_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(462, 333);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 36);
            this.btnClose.TabIndex = 111;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSubmit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(345, 333);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(117, 36);
            this.btnSubmit.TabIndex = 110;
            this.btnSubmit.Text = "&Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Arial", 11.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(325, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(211, 19);
            this.label7.TabIndex = 10072;
            this.label7.Text = "Please select Profit margin";
            // 
            // txtPurchaseBillwise
            // 
            this.txtPurchaseBillwise.Enabled = false;
            this.txtPurchaseBillwise.Font = new System.Drawing.Font("Arial", 11F);
            this.txtPurchaseBillwise.Location = new System.Drawing.Point(420, 224);
            this.txtPurchaseBillwise.Name = "txtPurchaseBillwise";
            this.txtPurchaseBillwise.Size = new System.Drawing.Size(135, 24);
            this.txtPurchaseBillwise.TabIndex = 108;
            this.txtPurchaseBillwise.Text = "0";
            this.txtPurchaseBillwise.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPurchaseBillwise.Enter += new System.EventHandler(this.txtFixedProfit_Enter);
            this.txtPurchaseBillwise.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFixedProfit_KeyPress);
            this.txtPurchaseBillwise.Leave += new System.EventHandler(this.txtFixedProfit_Leave);
            // 
            // txtFixedProfit
            // 
            this.txtFixedProfit.Enabled = false;
            this.txtFixedProfit.Font = new System.Drawing.Font("Arial", 11F);
            this.txtFixedProfit.Location = new System.Drawing.Point(420, 100);
            this.txtFixedProfit.Name = "txtFixedProfit";
            this.txtFixedProfit.Size = new System.Drawing.Size(135, 24);
            this.txtFixedProfit.TabIndex = 102;
            this.txtFixedProfit.Text = "0";
            this.txtFixedProfit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtFixedProfit.Enter += new System.EventHandler(this.txtFixedProfit_Enter);
            this.txtFixedProfit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFixedProfit_KeyPress);
            this.txtFixedProfit.Leave += new System.EventHandler(this.txtFixedProfit_Leave);
            // 
            // txtItemWise
            // 
            this.txtItemWise.Enabled = false;
            this.txtItemWise.Font = new System.Drawing.Font("Arial", 11F);
            this.txtItemWise.Location = new System.Drawing.Point(420, 180);
            this.txtItemWise.Name = "txtItemWise";
            this.txtItemWise.Size = new System.Drawing.Size(135, 24);
            this.txtItemWise.TabIndex = 106;
            this.txtItemWise.Text = "0";
            this.txtItemWise.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtItemWise.Enter += new System.EventHandler(this.txtFixedProfit_Enter);
            this.txtItemWise.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFixedProfit_KeyPress);
            this.txtItemWise.Leave += new System.EventHandler(this.txtFixedProfit_Leave);
            // 
            // panColor
            // 
            this.panColor.BackColor = System.Drawing.Color.White;
            this.panColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panColor.Controls.Add(this.panel5);
            this.panColor.Location = new System.Drawing.Point(20, 97);
            this.panColor.Name = "panColor";
            this.panColor.Size = new System.Drawing.Size(960, 527);
            this.panColor.TabIndex = 99;
            this.panColor.TabStop = true;
            // 
            // panHeader
            // 
            this.panHeader.BackColor = System.Drawing.Color.White;
            this.panHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panHeader.Controls.Add(this.lblNameHeader);
            this.panHeader.Location = new System.Drawing.Point(20, 22);
            this.panHeader.Name = "panHeader";
            this.panHeader.Size = new System.Drawing.Size(960, 52);
            this.panHeader.TabIndex = 100;
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.AutoSize = true;
            this.lblNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblNameHeader.Font = new System.Drawing.Font("Arial", 13.25F, System.Drawing.FontStyle.Bold);
            this.lblNameHeader.ForeColor = System.Drawing.Color.Black;
            this.lblNameHeader.Location = new System.Drawing.Point(384, 14);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(188, 21);
            this.lblNameHeader.TabIndex = 207;
            this.lblNameHeader.Text = "Profit Margin Setting";
            // 
            // rdoDesignMaster
            // 
            this.rdoDesignMaster.AutoSize = true;
            this.rdoDesignMaster.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.rdoDesignMaster.Location = new System.Drawing.Point(255, 266);
            this.rdoDesignMaster.Name = "rdoDesignMaster";
            this.rdoDesignMaster.Size = new System.Drawing.Size(177, 20);
            this.rdoDesignMaster.TabIndex = 109;
            this.rdoDesignMaster.TabStop = true;
            this.rdoDesignMaster.Text = "&As per Design Master";
            this.rdoDesignMaster.UseVisualStyleBackColor = true;
            // 
            // Profit_Margin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.panHeader);
            this.Controls.Add(this.panColor);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Profit_Margin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Profit Margin";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Profit_Margin_KeyDown);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panColor.ResumeLayout(false);
            this.panHeader.ResumeLayout(false);
            this.panHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panColor;
        private System.Windows.Forms.Panel panHeader;
        private System.Windows.Forms.Label lblNameHeader;
        private System.Windows.Forms.RadioButton rdoItemWise;
        private System.Windows.Forms.RadioButton rdoPurchaseBill;
        private System.Windows.Forms.RadioButton rdoFix;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPurchaseBillwise;
        private System.Windows.Forms.TextBox txtFixedProfit;
        private System.Windows.Forms.TextBox txtItemWise;
        private System.Windows.Forms.RadioButton rdoBrandWise;
        private System.Windows.Forms.TextBox txtBrandWise;
        private System.Windows.Forms.RadioButton rdoDesignMaster;
    }
}