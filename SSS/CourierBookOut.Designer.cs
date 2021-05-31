namespace SSS
{
    partial class CourierBookOut
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
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblCreatedBy = new System.Windows.Forms.LinkLabel();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.txtDate = new System.Windows.Forms.MaskedTextBox();
            this.txtBillCode = new System.Windows.Forms.TextBox();
            this.txtBillNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkSendSMS = new System.Windows.Forms.CheckBox();
            this.txtDocType = new System.Windows.Forms.TextBox();
            this.txtCourierName = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.txtSerialCode = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtSNo = new System.Windows.Forms.TextBox();
            this.txtStation = new System.Windows.Forms.TextBox();
            this.txtPartyName = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.txtCourierNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.grpDetails.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.BackColor = System.Drawing.Color.White;
            this.lblMsg.Font = new System.Drawing.Font("Arial", 7.75F, System.Drawing.FontStyle.Bold);
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMsg.Location = new System.Drawing.Point(130, 19);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 14);
            this.lblMsg.TabIndex = 212;
            this.lblMsg.Text = " ";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(481, 352);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(105, 34);
            this.btnSearch.TabIndex = 118;
            this.btnSearch.Text = "Searc&h";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label18.Location = new System.Drawing.Point(754, 55);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(16, 19);
            this.label18.TabIndex = 211;
            this.label18.Text = "*";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblCreatedBy);
            this.panel2.Controls.Add(this.grpDetails);
            this.panel2.Location = new System.Drawing.Point(25, 98);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(901, 509);
            this.panel2.TabIndex = 100;
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCreatedBy.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lblCreatedBy.LinkColor = System.Drawing.Color.Black;
            this.lblCreatedBy.Location = new System.Drawing.Point(40, 17);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(825, 15);
            this.lblCreatedBy.TabIndex = 202;
            this.lblCreatedBy.TabStop = true;
            this.lblCreatedBy.Text = "_";
            this.lblCreatedBy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCreatedBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCreatedBy_LinkClicked);
            // 
            // grpDetails
            // 
            this.grpDetails.Controls.Add(this.txtDate);
            this.grpDetails.Controls.Add(this.txtBillCode);
            this.grpDetails.Controls.Add(this.txtBillNo);
            this.grpDetails.Controls.Add(this.label2);
            this.grpDetails.Controls.Add(this.lblMsg);
            this.grpDetails.Controls.Add(this.label18);
            this.grpDetails.Controls.Add(this.btnSearch);
            this.grpDetails.Controls.Add(this.btnEdit);
            this.grpDetails.Controls.Add(this.btnDelete);
            this.grpDetails.Controls.Add(this.btnClose);
            this.grpDetails.Controls.Add(this.btnAdd);
            this.grpDetails.Controls.Add(this.chkSendSMS);
            this.grpDetails.Controls.Add(this.txtDocType);
            this.grpDetails.Controls.Add(this.txtCourierName);
            this.grpDetails.Controls.Add(this.txtCode);
            this.grpDetails.Controls.Add(this.txtSerialCode);
            this.grpDetails.Controls.Add(this.label25);
            this.grpDetails.Controls.Add(this.txtSNo);
            this.grpDetails.Controls.Add(this.txtStation);
            this.grpDetails.Controls.Add(this.txtPartyName);
            this.grpDetails.Controls.Add(this.label23);
            this.grpDetails.Controls.Add(this.label22);
            this.grpDetails.Controls.Add(this.label12);
            this.grpDetails.Controls.Add(this.label11);
            this.grpDetails.Controls.Add(this.label13);
            this.grpDetails.Controls.Add(this.label15);
            this.grpDetails.Controls.Add(this.label9);
            this.grpDetails.Controls.Add(this.label6);
            this.grpDetails.Controls.Add(this.txtRemark);
            this.grpDetails.Controls.Add(this.txtCourierNo);
            this.grpDetails.Controls.Add(this.label5);
            this.grpDetails.Controls.Add(this.label3);
            this.grpDetails.Location = new System.Drawing.Point(38, 30);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(827, 446);
            this.grpDetails.TabIndex = 101;
            this.grpDetails.TabStop = false;
            // 
            // txtDate
            // 
            this.txtDate.Font = new System.Drawing.Font("Arial", 10F);
            this.txtDate.Location = new System.Drawing.Point(128, 101);
            this.txtDate.Mask = "00/00/0000";
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(207, 23);
            this.txtDate.TabIndex = 106;
            this.txtDate.Enter += new System.EventHandler(this.txtDate_Enter);
            this.txtDate.Leave += new System.EventHandler(this.txtDate_Leave);
            // 
            // txtBillCode
            // 
            this.txtBillCode.BackColor = System.Drawing.Color.White;
            this.txtBillCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBillCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillCode.Location = new System.Drawing.Point(519, 151);
            this.txtBillCode.Name = "txtBillCode";
            this.txtBillCode.ReadOnly = true;
            this.txtBillCode.Size = new System.Drawing.Size(92, 23);
            this.txtBillCode.TabIndex = 109;
            this.txtBillCode.TabStop = false;
            this.txtBillCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillCode_KeyDown);
            // 
            // txtBillNo
            // 
            this.txtBillNo.BackColor = System.Drawing.Color.White;
            this.txtBillNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtBillNo.Location = new System.Drawing.Point(612, 151);
            this.txtBillNo.MaxLength = 6;
            this.txtBillNo.Name = "txtBillNo";
            this.txtBillNo.ReadOnly = true;
            this.txtBillNo.Size = new System.Drawing.Size(137, 23);
            this.txtBillNo.TabIndex = 110;
            this.txtBillNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBillNo_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(422, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 16);
            this.label2.TabIndex = 215;
            this.label2.Text = "Sale Bill No :";
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEdit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(250, 352);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(113, 34);
            this.btnEdit.TabIndex = 116;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(369, 352);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(105, 34);
            this.btnDelete.TabIndex = 117;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(592, 352);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 34);
            this.btnClose.TabIndex = 119;
            this.btnClose.Text = "C&lose";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(131, 352);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(113, 34);
            this.btnAdd.TabIndex = 115;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkSendSMS
            // 
            this.chkSendSMS.AutoSize = true;
            this.chkSendSMS.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSendSMS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSendSMS.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Bold);
            this.chkSendSMS.Location = new System.Drawing.Point(654, 261);
            this.chkSendSMS.Name = "chkSendSMS";
            this.chkSendSMS.Size = new System.Drawing.Size(98, 20);
            this.chkSendSMS.TabIndex = 114;
            this.chkSendSMS.Text = "Send S&MS";
            this.chkSendSMS.UseVisualStyleBackColor = true;
            // 
            // txtDocType
            // 
            this.txtDocType.BackColor = System.Drawing.Color.White;
            this.txtDocType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDocType.Font = new System.Drawing.Font("Arial", 10F);
            this.txtDocType.Location = new System.Drawing.Point(128, 151);
            this.txtDocType.Name = "txtDocType";
            this.txtDocType.ReadOnly = true;
            this.txtDocType.Size = new System.Drawing.Size(208, 23);
            this.txtDocType.TabIndex = 108;
            this.txtDocType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDocType_KeyDown);
            // 
            // txtCourierName
            // 
            this.txtCourierName.BackColor = System.Drawing.Color.White;
            this.txtCourierName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCourierName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCourierName.Location = new System.Drawing.Point(519, 101);
            this.txtCourierName.Name = "txtCourierName";
            this.txtCourierName.ReadOnly = true;
            this.txtCourierName.Size = new System.Drawing.Size(232, 23);
            this.txtCourierName.TabIndex = 107;
            this.txtCourierName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourierName_KeyDown);
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.White;
            this.txtCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCode.Location = new System.Drawing.Point(128, 53);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(91, 23);
            this.txtCode.TabIndex = 102;
            this.txtCode.TabStop = false;
            this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCode_KeyDown);
            // 
            // txtSerialCode
            // 
            this.txtSerialCode.BackColor = System.Drawing.Color.White;
            this.txtSerialCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSerialCode.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSerialCode.Location = new System.Drawing.Point(287, 53);
            this.txtSerialCode.MaxLength = 2;
            this.txtSerialCode.Name = "txtSerialCode";
            this.txtSerialCode.Size = new System.Drawing.Size(48, 23);
            this.txtSerialCode.TabIndex = 104;
            this.txtSerialCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSerialCode_KeyPress);
            this.txtSerialCode.Leave += new System.EventHandler(this.txtSerialCode_Leave);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label25.Location = new System.Drawing.Point(336, 55);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(16, 19);
            this.label25.TabIndex = 201;
            this.label25.Text = "*";
            // 
            // txtSNo
            // 
            this.txtSNo.BackColor = System.Drawing.Color.White;
            this.txtSNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtSNo.Location = new System.Drawing.Point(220, 53);
            this.txtSNo.MaxLength = 6;
            this.txtSNo.Name = "txtSNo";
            this.txtSNo.Size = new System.Drawing.Size(66, 23);
            this.txtSNo.TabIndex = 103;
            this.txtSNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSNo_KeyPress);
            this.txtSNo.Leave += new System.EventHandler(this.txtSNo_Leave);
            // 
            // txtStation
            // 
            this.txtStation.BackColor = System.Drawing.Color.White;
            this.txtStation.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtStation.Font = new System.Drawing.Font("Arial", 10F);
            this.txtStation.Location = new System.Drawing.Point(128, 207);
            this.txtStation.Name = "txtStation";
            this.txtStation.ReadOnly = true;
            this.txtStation.Size = new System.Drawing.Size(208, 23);
            this.txtStation.TabIndex = 111;
            this.txtStation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStation_KeyDown);
            // 
            // txtPartyName
            // 
            this.txtPartyName.BackColor = System.Drawing.Color.White;
            this.txtPartyName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPartyName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtPartyName.Location = new System.Drawing.Point(518, 207);
            this.txtPartyName.Name = "txtPartyName";
            this.txtPartyName.ReadOnly = true;
            this.txtPartyName.Size = new System.Drawing.Size(232, 23);
            this.txtPartyName.TabIndex = 112;
            this.txtPartyName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPartyName_KeyDown);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label23.Location = new System.Drawing.Point(340, 154);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(16, 19);
            this.label23.TabIndex = 200;
            this.label23.Text = "*";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 12.25F, System.Drawing.FontStyle.Bold);
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label22.Location = new System.Drawing.Point(340, 211);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(16, 19);
            this.label22.TabIndex = 199;
            this.label22.Text = "*";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(28, 154);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(83, 16);
            this.label12.TabIndex = 198;
            this.label12.Text = "Doc. Type :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(36, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 16);
            this.label11.TabIndex = 197;
            this.label11.Text = "Serial No :";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(402, 104);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(112, 16);
            this.label13.TabIndex = 191;
            this.label13.Text = "Courier Name :";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(46, 262);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 16);
            this.label15.TabIndex = 192;
            this.label15.Text = "Remark :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(70, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 16);
            this.label9.TabIndex = 196;
            this.label9.Text = "Date :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(423, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 16);
            this.label6.TabIndex = 195;
            this.label6.Text = "Courier No :";
            // 
            // txtRemark
            // 
            this.txtRemark.BackColor = System.Drawing.Color.White;
            this.txtRemark.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRemark.Font = new System.Drawing.Font("Arial", 10F);
            this.txtRemark.Location = new System.Drawing.Point(128, 259);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(520, 23);
            this.txtRemark.TabIndex = 113;
            this.txtRemark.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSerialCode_KeyPress);
            // 
            // txtCourierNo
            // 
            this.txtCourierNo.BackColor = System.Drawing.Color.White;
            this.txtCourierNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCourierNo.Font = new System.Drawing.Font("Arial", 10F);
            this.txtCourierNo.Location = new System.Drawing.Point(518, 53);
            this.txtCourierNo.Name = "txtCourierNo";
            this.txtCourierNo.Size = new System.Drawing.Size(232, 23);
            this.txtCourierNo.TabIndex = 105;
            this.txtCourierNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSerialCode_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(50, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 16);
            this.label5.TabIndex = 194;
            this.label5.Text = "Station :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(422, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 16);
            this.label3.TabIndex = 193;
            this.label3.Text = "Party Name :";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(901, 46);
            this.panel1.TabIndex = 101;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(345, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "COURIER BOOK (OUT)";
            // 
            // CourierBookOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(956, 642);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CourierBookOut";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CourierBookOut";
            this.Load += new System.EventHandler(this.CourierBookOut_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CourierBookOut_KeyDown);
            this.panel2.ResumeLayout(false);
            this.grpDetails.ResumeLayout(false);
            this.grpDetails.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.CheckBox chkSendSMS;
        private System.Windows.Forms.TextBox txtDocType;
        private System.Windows.Forms.TextBox txtCourierName;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.TextBox txtSerialCode;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtSNo;
        private System.Windows.Forms.TextBox txtStation;
        private System.Windows.Forms.TextBox txtPartyName;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.TextBox txtCourierNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtBillCode;
        private System.Windows.Forms.TextBox txtBillNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lblCreatedBy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtDate;
    }
}