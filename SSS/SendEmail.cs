using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace SSS
{
    public partial class SendEmail : Form
    {
        DataBaseAccess dba;
        string[] strGRSNoFiles;
        string  strGRSNoFilePath="";
        
        public SendEmail()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindCollectionwithText();
        }
        public SendEmail(string strSendTo, string strSubject, string strFilePath)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindCollectionwithText();
            txtSendTo.Text = strSendTo;
            lblSendTO.Text = GetEmailID(strSendTo);
            txtSubject.Text = strSubject;
            txtAttachFileI.Text = strFilePath;
        }

        public SendEmail(string strSendTo, string strSubject, string strFilePath,string[] strFiles)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindCollectionwithText();
            txtSendTo.Text = strSendTo;
            lblSendTO.Text = GetEmailID(strSendTo);
            strGRSNoFiles = strFiles;
            strGRSNoFilePath = strFilePath;
            txtSubject.Text = strSubject;
            SetFileinAttatchTextbox(strFiles);
        }

        private void SetFileinAttatchTextbox(string[] strFile)
        {
            for (int i = 0; i < strFile.Length; i++)
            {
                if (i == 0)
                {
                    txtAttachFileI.Text = strFile[i];
                }
                else
                {
                    txtAttachFileI.Text += ", "+strFile[i];
                }
            }
            txtAttachFileI.ReadOnly = true;
            btnBrowseI.Enabled = false;
        }

        private void BindCollectionwithText()
        {
            AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
            DataTable partyTable = dba.GetPartyRecord();
            try
            {
                if (txtSendTo.Text == "")
                {
                    foreach (DataRow dr in partyTable.Rows)
                    {
                        namesCollection.Add(dr["Name"].ToString());
                    }
                }

                txtSendTo.AutoCompleteCustomSource = namesCollection;
                txtAddCC.AutoCompleteCustomSource = namesCollection;


            }
            catch
            {
            }
        }

        private string GetEmailID(string strSendTo)
        {
            string emailID = dba.GetPartyEmailID(strSendTo);
            return emailID;
        }

        private void SendMail()
        {
            try
            {
                if (lblSendTO.Text != "" && lblSendTO.Text.Contains("@"))
                {
                    MailMessage message = new MailMessage(MainPage.strSenderEmailID, lblSendTO.Text);
                    if (lblAddCC.Text != " ")
                    {
                        MailAddress mailCC = new MailAddress(lblAddCC.Text);
                        message.CC.Add(mailCC);
                    }
                    message.IsBodyHtml = true;
                    message.Subject = txtSubject.Text;
                    message.Body = txtMessage.Text;
                    if (txtAttachFileI.Text != "")
                    {
                        if (strGRSNoFilePath != "")
                        {
                            foreach (string strFile in strGRSNoFiles)
                            {
                                Attachment attach = new Attachment(strGRSNoFilePath+strFile);
                                message.Attachments.Add(attach);
                            }
                        }
                        else
                        {
                            Attachment attach = new Attachment(txtAttachFileI.Text);
                            message.Attachments.Add(attach);
                        }

                    }
                    if (txtAttachFileII.Text != "")
                    {
                        Attachment attach = new Attachment(txtAttachFileII.Text);
                        message.Attachments.Add(attach);
                    }
                    if (txtAttachFileIII.Text != "")
                    {
                        Attachment attach = new Attachment(txtAttachFileIII.Text);
                        message.Attachments.Add(attach);
                    }
                    DataBaseAccess.SendEmail(message);
                    MessageBox.Show("Mail Send Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnSubmit.Enabled = true;
                    ClearAllText();
                }
                else
                {
                    MessageBox.Show("Please Provide Valid Email Id  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnSubmit.Enabled = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtSendTo.Text != "")
            {
                DialogResult dr = MessageBox.Show("Are you sure want to Send Mail ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    btnSubmit.Enabled = false;
                    SendMail();
                }
            }
        }

        private void btnBrowseI_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            txtAttachFileI.Text = dialog.FileName;
        }

        private void btnBrowseII_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            txtAttachFileII.Text = dialog.FileName;
        }

        private void btnBrowseIII_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            txtAttachFileIII.Text = dialog.FileName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SendEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        private void txtSendTo_Leave(object sender, EventArgs e)
        {
            if (txtSendTo.Text != "")
            {
                lblSendTO.Text = GetEmailID(txtSendTo.Text);
            }
        }

        private void txtAddCC_Leave(object sender, EventArgs e)
        {
            if (txtAddCC.Text != "")
            {
                lblAddCC.Text = GetEmailID(txtAddCC.Text);
            }
            else
            {
                lblAddCC.Text = " ";
            }
        }

        private void ClearAllText()
        {
            txtAddCC.Clear();
            txtAttachFileI.Clear();
            txtAttachFileII.Clear();
            txtAttachFileIII.Clear();
            txtMessage.Clear();
            txtSendTo.Clear();
            txtSubject.Clear();
            lblAddCC.Text = " ";
            lblSendTO.Text = " ";
            btnBrowseI.Enabled = true;
            txtAttachFileI.ReadOnly = false;
        }

        private void SendEmail_Load(object sender, EventArgs e)
        {
            try
            {
                if (!MainPage.mymainObject.bSMSReport)
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch { }
        }
    }
}
