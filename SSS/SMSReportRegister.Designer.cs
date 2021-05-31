namespace SSS
{
    partial class SMSReportRegister
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtToDate = new System.Windows.Forms.MaskedTextBox();
            this.txtFromDate = new System.Windows.Forms.MaskedTextBox();
            this.txtMobileNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPartyName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.rdoFailed = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoSent = new System.Windows.Forms.RadioButton();
            this.chkDate = new System.Windows.Forms.CheckBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picPleasewait = new System.Windows.Forms.PictureBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnResend = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblFailedSMS = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblSentSMS = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAllSMS = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgrdSMS = new System.Windows.Forms.DataGridView();
            this.chkStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.senderID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mobileNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smsStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sendedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smsResendedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smsID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSMS)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.txtToDate);
            this.panel2.Controls.Add(this.txtFromDate);
            this.panel2.Controls.Add(this.txtMobileNo);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtPartyName);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.grpStatus);
            this.panel2.Controls.Add(this.chkDate);
            this.panel2.Controls.Add(this.txtMessage);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnGo);
            this.panel2.Location = new System.Drawing.Point(13, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(971, 72);
            this.panel2.TabIndex = 100;
            // 
            // txtToDate
            // 
            this.txtToDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtToDate.Location = new System.Drawing.Point(441, 7);
            this.txtToDate.Mask = "00/00/0000";
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Size = new System.Drawing.Size(75, 23);
            this.txtToDate.TabIndex = 107;
            this.txtToDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtToDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtFromDate
            // 
            this.txtFromDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFromDate.Location = new System.Drawing.Point(332, 7);
            this.txtFromDate.Mask = "00/00/0000";
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Size = new System.Drawing.Size(78, 23);
            this.txtFromDate.TabIndex = 106;
            this.txtFromDate.Enter += new System.EventHandler(this.txtFromDate_Enter);
            this.txtFromDate.Leave += new System.EventHandler(this.txtFromDate_Leave);
            // 
            // txtMobileNo
            // 
            this.txtMobileNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMobileNo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMobileNo.Location = new System.Drawing.Point(640, 40);
            this.txtMobileNo.MaxLength = 10;
            this.txtMobileNo.Name = "txtMobileNo";
            this.txtMobileNo.Size = new System.Drawing.Size(217, 22);
            this.txtMobileNo.TabIndex = 110;
            this.txtMobileNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFromDate_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(549, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 16);
            this.label7.TabIndex = 168;
            this.label7.Text = "Mobile No :";
            // 
            // txtPartyName
            // 
            this.txtPartyName.BackColor = System.Drawing.Color.White;
            this.txtPartyName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPartyName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPartyName.Location = new System.Drawing.Point(639, 7);
            this.txtPartyName.Name = "txtPartyName";
            this.txtPartyName.ReadOnly = true;
            this.txtPartyName.Size = new System.Drawing.Size(313, 22);
            this.txtPartyName.TabIndex = 108;
            this.txtPartyName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPartyName_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(14, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 16);
            this.label5.TabIndex = 167;
            this.label5.Text = "Message :";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoFailed);
            this.grpStatus.Controls.Add(this.rdoAll);
            this.grpStatus.Controls.Add(this.rdoSent);
            this.grpStatus.Location = new System.Drawing.Point(11, -4);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(227, 40);
            this.grpStatus.TabIndex = 101;
            this.grpStatus.TabStop = false;
            // 
            // rdoFailed
            // 
            this.rdoFailed.AutoSize = true;
            this.rdoFailed.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoFailed.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rdoFailed.Location = new System.Drawing.Point(154, 14);
            this.rdoFailed.Name = "rdoFailed";
            this.rdoFailed.Size = new System.Drawing.Size(66, 20);
            this.rdoFailed.TabIndex = 104;
            this.rdoFailed.TabStop = true;
            this.rdoFailed.Text = "Failed";
            this.rdoFailed.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoAll.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rdoAll.Location = new System.Drawing.Point(7, 14);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(43, 20);
            this.rdoAll.TabIndex = 102;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // rdoSent
            // 
            this.rdoSent.AutoSize = true;
            this.rdoSent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSent.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rdoSent.Location = new System.Drawing.Point(57, 13);
            this.rdoSent.Name = "rdoSent";
            this.rdoSent.Size = new System.Drawing.Size(88, 20);
            this.rdoSent.TabIndex = 103;
            this.rdoSent.TabStop = true;
            this.rdoSent.Text = "Sent SMS";
            this.rdoSent.UseVisualStyleBackColor = true;
            // 
            // chkDate
            // 
            this.chkDate.AutoSize = true;
            this.chkDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chkDate.Location = new System.Drawing.Point(252, 9);
            this.chkDate.Name = "chkDate";
            this.chkDate.Size = new System.Drawing.Size(67, 20);
            this.chkDate.TabIndex = 105;
            this.chkDate.Text = "Date :";
            this.chkDate.UseVisualStyleBackColor = true;
            this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
            // 
            // txtMessage
            // 
            this.txtMessage.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMessage.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(100, 40);
            this.txtMessage.MaxLength = 500;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(430, 22);
            this.txtMessage.TabIndex = 109;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(535, 10);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(90, 16);
            this.label26.TabIndex = 164;
            this.label26.Text = "Party Name :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(413, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "To";
            // 
            // btnGo
            // 
            this.btnGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnGo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnGo.ForeColor = System.Drawing.Color.White;
            this.btnGo.Location = new System.Drawing.Point(880, 33);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(70, 33);
            this.btnGo.TabIndex = 111;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = false;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(13, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(972, 42);
            this.panel1.TabIndex = 119;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(384, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Show SMS Report";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.btnSendSMS);
            this.panel3.Controls.Add(this.btnDelete);
            this.panel3.Controls.Add(this.picPleasewait);
            this.panel3.Controls.Add(this.chkAll);
            this.panel3.Controls.Add(this.btnResend);
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.lblFailedSMS);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.lblSentSMS);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.lblAllSMS);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.dgrdSMS);
            this.panel3.Location = new System.Drawing.Point(13, 132);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(969, 516);
            this.panel3.TabIndex = 112;
            this.panel3.TabStop = true;
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSendSMS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendSMS.ForeColor = System.Drawing.Color.White;
            this.btnSendSMS.Location = new System.Drawing.Point(206, 477);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(141, 33);
            this.btnSendSMS.TabIndex = 115;
            this.btnSendSMS.Text = "Send New SMS";
            this.btnSendSMS.UseVisualStyleBackColor = false;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(111, 477);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 33);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picPleasewait
            // 
            this.picPleasewait.Image = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.InitialImage = global::SSS.Properties.Resources.PleaseWait;
            this.picPleasewait.Location = new System.Drawing.Point(382, 95);
            this.picPleasewait.Name = "picPleasewait";
            this.picPleasewait.Size = new System.Drawing.Size(185, 132);
            this.picPleasewait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picPleasewait.TabIndex = 207;
            this.picPleasewait.TabStop = false;
            this.picPleasewait.Visible = false;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.BackColor = System.Drawing.Color.Transparent;
            this.chkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAll.Location = new System.Drawing.Point(19, 22);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 115;
            this.chkAll.UseVisualStyleBackColor = false;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // btnResend
            // 
            this.btnResend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnResend.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnResend.ForeColor = System.Drawing.Color.White;
            this.btnResend.Location = new System.Drawing.Point(7, 477);
            this.btnResend.Name = "btnResend";
            this.btnResend.Size = new System.Drawing.Size(104, 33);
            this.btnResend.TabIndex = 113;
            this.btnResend.Text = "Rese&nd";
            this.btnResend.UseVisualStyleBackColor = false;
            this.btnResend.Click += new System.EventHandler(this.btnResend_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(347, 477);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 33);
            this.btnClose.TabIndex = 116;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // lblFailedSMS
            // 
            this.lblFailedSMS.AutoSize = true;
            this.lblFailedSMS.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblFailedSMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblFailedSMS.Location = new System.Drawing.Point(892, 484);
            this.lblFailedSMS.Name = "lblFailedSMS";
            this.lblFailedSMS.Size = new System.Drawing.Size(16, 16);
            this.lblFailedSMS.TabIndex = 125;
            this.lblFailedSMS.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(796, 484);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 16);
            this.label9.TabIndex = 124;
            this.label9.Text = "Failed SMS  :";
            // 
            // lblSentSMS
            // 
            this.lblSentSMS.AutoSize = true;
            this.lblSentSMS.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblSentSMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblSentSMS.Location = new System.Drawing.Point(717, 484);
            this.lblSentSMS.Name = "lblSentSMS";
            this.lblSentSMS.Size = new System.Drawing.Size(16, 16);
            this.lblSentSMS.TabIndex = 123;
            this.lblSentSMS.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(630, 484);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 16);
            this.label6.TabIndex = 122;
            this.label6.Text = "Sent SMS  :";
            // 
            // lblAllSMS
            // 
            this.lblAllSMS.AutoSize = true;
            this.lblAllSMS.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblAllSMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblAllSMS.Location = new System.Drawing.Point(529, 484);
            this.lblAllSMS.Name = "lblAllSMS";
            this.lblAllSMS.Size = new System.Drawing.Size(16, 16);
            this.lblAllSMS.TabIndex = 121;
            this.lblAllSMS.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(441, 484);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 16);
            this.label3.TabIndex = 120;
            this.label3.Text = "Total SMS  :";
            // 
            // dgrdSMS
            // 
            this.dgrdSMS.AllowUserToAddRows = false;
            this.dgrdSMS.AllowUserToDeleteRows = false;
            this.dgrdSMS.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.dgrdSMS.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdSMS.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSMS.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdSMS.ColumnHeadersHeight = 30;
            this.dgrdSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSMS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkStatus,
            this.date,
            this.senderID,
            this.mobileNo,
            this.message,
            this.smsStatus,
            this.sendedBy,
            this.smsResendedBy,
            this.smsID});
            this.dgrdSMS.EnableHeadersVisualStyles = false;
            this.dgrdSMS.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSMS.Location = new System.Drawing.Point(10, 13);
            this.dgrdSMS.Name = "dgrdSMS";
            this.dgrdSMS.RowHeadersVisible = false;
            this.dgrdSMS.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSMS.RowTemplate.Height = 25;
            this.dgrdSMS.Size = new System.Drawing.Size(943, 458);
            this.dgrdSMS.TabIndex = 119;
            this.dgrdSMS.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgrdSMS_CellBeginEdit);
            this.dgrdSMS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrdSMS_KeyDown);
            // 
            // chkStatus
            // 
            this.chkStatus.HeaderText = "";
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Width = 30;
            // 
            // date
            // 
            dataGridViewCellStyle3.Format = "dd/MM/yyyy";
            this.date.DefaultCellStyle = dataGridViewCellStyle3;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.Width = 90;
            // 
            // senderID
            // 
            this.senderID.HeaderText = "Type";
            this.senderID.Name = "senderID";
            this.senderID.Width = 70;
            // 
            // mobileNo
            // 
            this.mobileNo.HeaderText = "Mobile No";
            this.mobileNo.Name = "mobileNo";
            this.mobileNo.Width = 110;
            // 
            // message
            // 
            this.message.HeaderText = "Message";
            this.message.Name = "message";
            this.message.Width = 530;
            // 
            // smsStatus
            // 
            this.smsStatus.HeaderText = "Status";
            this.smsStatus.Name = "smsStatus";
            this.smsStatus.Width = 90;
            // 
            // sendedBy
            // 
            this.sendedBy.HeaderText = "Sent By";
            this.sendedBy.Name = "sendedBy";
            // 
            // smsResendedBy
            // 
            this.smsResendedBy.HeaderText = "Resend By";
            this.smsResendedBy.Name = "smsResendedBy";
            // 
            // smsID
            // 
            this.smsID.HeaderText = "ID";
            this.smsID.Name = "smsID";
            this.smsID.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle4.Format = "dd/MM/yyyy";
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn1.HeaderText = "Date";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Sender ID";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 80;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Mobile No";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 160;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Message";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 310;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Status";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 90;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Reason";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "ID";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Visible = false;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "ID";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Visible = false;
            // 
            // SMSReportRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SMSReportRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMSReportRegister";
            this.Load += new System.EventHandler(this.SMSReportRegister_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SMSReportRegister_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPleasewait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSMS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgrdSMS;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lblAllSMS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFailedSMS;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblSentSMS;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox picPleasewait;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnResend;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtPartyName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RadioButton rdoFailed;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.RadioButton rdoSent;
        private System.Windows.Forms.TextBox txtMobileNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSendSMS;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn senderID;
        private System.Windows.Forms.DataGridViewTextBoxColumn mobileNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn message;
        private System.Windows.Forms.DataGridViewTextBoxColumn smsStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn sendedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn smsResendedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn smsID;
        protected internal System.Windows.Forms.MaskedTextBox txtToDate;
        protected internal System.Windows.Forms.MaskedTextBox txtFromDate;
    }
}