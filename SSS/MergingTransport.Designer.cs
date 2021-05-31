namespace SSS
{
    partial class MergingTransport
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
            this.MergeTransport = new System.Windows.Forms.TabControl();
            this.transport = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtFinalTransport = new System.Windows.Forms.TextBox();
            this.txtSTransport = new System.Windows.Forms.TextBox();
            this.txtFTransport = new System.Windows.Forms.TextBox();
            this.btnTClose = new System.Windows.Forms.Button();
            this.btnTMerge = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.station = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFinalStation = new System.Windows.Forms.TextBox();
            this.txtSStation = new System.Windows.Forms.TextBox();
            this.txtFStation = new System.Windows.Forms.TextBox();
            this.btnSClose = new System.Windows.Forms.Button();
            this.btnSMerge = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.MergeTransport.SuspendLayout();
            this.transport.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.station.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(57, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(835, 46);
            this.panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(224, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(330, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "MERGE TRANSPORT && STATION NAMES";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.MergeTransport);
            this.panel2.Location = new System.Drawing.Point(57, 135);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(835, 422);
            this.panel2.TabIndex = 14;
            // 
            // MergeTransport
            // 
            this.MergeTransport.Controls.Add(this.transport);
            this.MergeTransport.Controls.Add(this.station);
            this.MergeTransport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.MergeTransport.Location = new System.Drawing.Point(48, 26);
            this.MergeTransport.Name = "MergeTransport";
            this.MergeTransport.Padding = new System.Drawing.Point(50, 10);
            this.MergeTransport.SelectedIndex = 0;
            this.MergeTransport.Size = new System.Drawing.Size(744, 354);
            this.MergeTransport.TabIndex = 0;
            // 
            // transport
            // 
            this.transport.Controls.Add(this.groupBox1);
            this.transport.Controls.Add(this.txtFinalTransport);
            this.transport.Controls.Add(this.txtSTransport);
            this.transport.Controls.Add(this.txtFTransport);
            this.transport.Controls.Add(this.btnTClose);
            this.transport.Controls.Add(this.btnTMerge);
            this.transport.Controls.Add(this.label9);
            this.transport.Controls.Add(this.label10);
            this.transport.Controls.Add(this.label11);
            this.transport.Location = new System.Drawing.Point(4, 39);
            this.transport.Name = "transport";
            this.transport.Size = new System.Drawing.Size(736, 311);
            this.transport.TabIndex = 0;
            this.transport.Text = "Transport";
            this.transport.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(93, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(552, 53);
            this.groupBox1.TabIndex = 168;
            this.groupBox1.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(178, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(171, 18);
            this.label8.TabIndex = 149;
            this.label8.Text = "Merge Transport Name";
            // 
            // txtFinalTransport
            // 
            this.txtFinalTransport.BackColor = System.Drawing.SystemColors.Window;
            this.txtFinalTransport.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinalTransport.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFinalTransport.Location = new System.Drawing.Point(251, 190);
            this.txtFinalTransport.Name = "txtFinalTransport";
            this.txtFinalTransport.ReadOnly = true;
            this.txtFinalTransport.Size = new System.Drawing.Size(394, 23);
            this.txtFinalTransport.TabIndex = 162;
            this.txtFinalTransport.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFinalTransport_KeyDown);
            // 
            // txtSTransport
            // 
            this.txtSTransport.BackColor = System.Drawing.SystemColors.Window;
            this.txtSTransport.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSTransport.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSTransport.Location = new System.Drawing.Point(251, 150);
            this.txtSTransport.Name = "txtSTransport";
            this.txtSTransport.ReadOnly = true;
            this.txtSTransport.Size = new System.Drawing.Size(394, 23);
            this.txtSTransport.TabIndex = 161;
            this.txtSTransport.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSTransport_KeyDown);
            // 
            // txtFTransport
            // 
            this.txtFTransport.BackColor = System.Drawing.SystemColors.Window;
            this.txtFTransport.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFTransport.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFTransport.Location = new System.Drawing.Point(251, 107);
            this.txtFTransport.Name = "txtFTransport";
            this.txtFTransport.ReadOnly = true;
            this.txtFTransport.Size = new System.Drawing.Size(394, 23);
            this.txtFTransport.TabIndex = 160;
            this.txtFTransport.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFTransport_KeyDown);
            // 
            // btnTClose
            // 
            this.btnTClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnTClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnTClose.ForeColor = System.Drawing.Color.White;
            this.btnTClose.Location = new System.Drawing.Point(389, 249);
            this.btnTClose.Name = "btnTClose";
            this.btnTClose.Size = new System.Drawing.Size(117, 40);
            this.btnTClose.TabIndex = 164;
            this.btnTClose.Text = "&Close";
            this.btnTClose.UseVisualStyleBackColor = false;
            this.btnTClose.Click += new System.EventHandler(this.btnTClose_Click);
            // 
            // btnTMerge
            // 
            this.btnTMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnTMerge.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnTMerge.ForeColor = System.Drawing.Color.White;
            this.btnTMerge.Location = new System.Drawing.Point(217, 249);
            this.btnTMerge.Name = "btnTMerge";
            this.btnTMerge.Size = new System.Drawing.Size(167, 40);
            this.btnTMerge.TabIndex = 163;
            this.btnTMerge.Text = "&Merge Transport";
            this.btnTMerge.UseVisualStyleBackColor = false;
            this.btnTMerge.Click += new System.EventHandler(this.btnTMerge_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(111, 193);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 16);
            this.label9.TabIndex = 167;
            this.label9.Text = "Final Transport :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(89, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 16);
            this.label10.TabIndex = 166;
            this.label10.Text = "Second Transport :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(113, 110);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 16);
            this.label11.TabIndex = 165;
            this.label11.Text = "First Transport :";
            // 
            // station
            // 
            this.station.Controls.Add(this.groupBox2);
            this.station.Controls.Add(this.txtFinalStation);
            this.station.Controls.Add(this.txtSStation);
            this.station.Controls.Add(this.txtFStation);
            this.station.Controls.Add(this.btnSClose);
            this.station.Controls.Add(this.btnSMerge);
            this.station.Controls.Add(this.label7);
            this.station.Controls.Add(this.label5);
            this.station.Controls.Add(this.label4);
            this.station.Location = new System.Drawing.Point(4, 39);
            this.station.Name = "station";
            this.station.Size = new System.Drawing.Size(736, 311);
            this.station.TabIndex = 1;
            this.station.Text = "Station";
            this.station.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Location = new System.Drawing.Point(93, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 53);
            this.groupBox2.TabIndex = 168;
            this.groupBox2.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(188, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(152, 18);
            this.label20.TabIndex = 148;
            this.label20.Text = "Merge Station Name";
            // 
            // txtFinalStation
            // 
            this.txtFinalStation.BackColor = System.Drawing.SystemColors.Window;
            this.txtFinalStation.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinalStation.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFinalStation.Location = new System.Drawing.Point(217, 190);
            this.txtFinalStation.Name = "txtFinalStation";
            this.txtFinalStation.ReadOnly = true;
            this.txtFinalStation.Size = new System.Drawing.Size(428, 23);
            this.txtFinalStation.TabIndex = 162;
            this.txtFinalStation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFinalStation_KeyDown);
            // 
            // txtSStation
            // 
            this.txtSStation.BackColor = System.Drawing.SystemColors.Window;
            this.txtSStation.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSStation.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSStation.Location = new System.Drawing.Point(217, 150);
            this.txtSStation.Name = "txtSStation";
            this.txtSStation.ReadOnly = true;
            this.txtSStation.Size = new System.Drawing.Size(428, 23);
            this.txtSStation.TabIndex = 161;
            this.txtSStation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSStation_KeyDown);
            // 
            // txtFStation
            // 
            this.txtFStation.BackColor = System.Drawing.SystemColors.Window;
            this.txtFStation.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFStation.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFStation.Location = new System.Drawing.Point(217, 107);
            this.txtFStation.Name = "txtFStation";
            this.txtFStation.ReadOnly = true;
            this.txtFStation.Size = new System.Drawing.Size(428, 23);
            this.txtFStation.TabIndex = 160;
            this.txtFStation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFStation_KeyDown);
            // 
            // btnSClose
            // 
            this.btnSClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSClose.ForeColor = System.Drawing.Color.White;
            this.btnSClose.Location = new System.Drawing.Point(389, 249);
            this.btnSClose.Name = "btnSClose";
            this.btnSClose.Size = new System.Drawing.Size(117, 40);
            this.btnSClose.TabIndex = 164;
            this.btnSClose.Text = "&Close";
            this.btnSClose.UseVisualStyleBackColor = false;
            this.btnSClose.Click += new System.EventHandler(this.btnSClose_Click);
            // 
            // btnSMerge
            // 
            this.btnSMerge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSMerge.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSMerge.ForeColor = System.Drawing.Color.White;
            this.btnSMerge.Location = new System.Drawing.Point(217, 249);
            this.btnSMerge.Name = "btnSMerge";
            this.btnSMerge.Size = new System.Drawing.Size(167, 40);
            this.btnSMerge.TabIndex = 163;
            this.btnSMerge.Text = "&Merge Station";
            this.btnSMerge.UseVisualStyleBackColor = false;
            this.btnSMerge.Click += new System.EventHandler(this.btnSMerge_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(98, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 16);
            this.label7.TabIndex = 167;
            this.label7.Text = "Final Station :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(78, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 16);
            this.label5.TabIndex = 166;
            this.label5.Text = "Second Station :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(100, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 16);
            this.label4.TabIndex = 165;
            this.label4.Text = "First Station :";
            // 
            // MergingTransport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(950, 610);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MergingTransport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Merging Transport";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MergingTransport_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.MergeTransport.ResumeLayout(false);
            this.transport.ResumeLayout(false);
            this.transport.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.station.ResumeLayout(false);
            this.station.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl MergeTransport;
        private System.Windows.Forms.TabPage transport;
        private System.Windows.Forms.TabPage station;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFinalTransport;
        private System.Windows.Forms.TextBox txtSTransport;
        private System.Windows.Forms.TextBox txtFTransport;
        private System.Windows.Forms.Button btnTClose;
        private System.Windows.Forms.Button btnTMerge;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtFinalStation;
        private System.Windows.Forms.TextBox txtSStation;
        private System.Windows.Forms.TextBox txtFStation;
        private System.Windows.Forms.Button btnSClose;
        private System.Windows.Forms.Button btnSMerge;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
    }
}