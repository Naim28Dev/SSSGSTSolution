using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace SSS
{
    public partial class SendWhatsappPage : Form
    {
        bool _bClosingStatus = false;
        DataBaseAccess dba;
        public SendWhatsappPage()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }

        public SendWhatsappPage(string strMobileNo, string strMessage)
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            _bClosingStatus = true;
            txtWhatappNo.Text = strMobileNo;
            txtSMS.Text = strMessage;
            //if (!MainPage.strUserRole.Contains("ADMIN"))
            //    txtSMS.ReadOnly = true;
        }

        private void SendUnicodeSMS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !txtSMS.Focused)
                SendKeys.Send("{TAB}");
        }

        private void btnSMS_Click(object sender, EventArgs e)
        {
            try
            {
                btnSMS.Enabled = false;
                if ((txtSMS.Text != "" || txtFileName.Text!="") && txtWhatappNo.Text.Length > 9)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to send whatsapp message ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = SendWhatsappMessages();
                        if (count>0)
                        {
                            MessageBox.Show("Message sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            txtSMS.Clear();
                            txtWhatappNo.Clear();
                            if (_bClosingStatus)
                                this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Please try again  ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please fill the message box & mobile no and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnSMS.Enabled = true;
        }
      
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == (Char)44)
                {
                    e.Handled = false;
                }
                else
                {
                    if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            catch
            {
            }
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                    GetMobileNo();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetMobileNo()
        {
            txtWhatappNo.Clear();
            if(txtPartyName.Text!="")
            {
                DataBaseAccess dba = new SSS.DataBaseAccess();
                txtWhatappNo.Text = Convert.ToString(dba.GetPartyWhatsappNo(txtPartyName.Text));
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.ShowDialog();
                txtFileName.Text = objDialog.FileName;
            }
            catch
            {
            }
        }

        private int SendWhatsappMessages()
        {
            int _count = 0;
            try
            {
                string strResult = "", strFileType = "TEXT", strFileName = txtFileName.Text, _strFileName = "", strHttp_File = "" ;
                if (strFileName != "")
                {
                    if (rdoPDF.Checked)
                        strFileType = "PDF";
                    else if (rdoVideo.Checked)
                        strFileType = "VIDEO";
                    else if (rdoImage.Checked)
                        strFileType = "IMAGE";


                    FileInfo objFile = new FileInfo(strFileName);
                    string[] str = objFile.Name.Split('.');
                    if (str.Length > 0)
                    {
                        string strOrginalFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        _strFileName = str[0] + "_" + strOrginalFileName + "." + str[1];

                        strHttp_File = MainPage.strHttpPath + "/images/" + _strFileName;
                        bool _bStatus = dba.UploadImageForWhatsApp(strFileName, _strFileName);
                        if (!_bStatus)
                            _strFileName = strHttp_File = "";
                    }
                }

                string[] strAllMobileNo = txtWhatappNo.Text.Split(',');
                foreach (string strMobile in strAllMobileNo)
                {
                    strResult = WhatsappClass.SendWhatsAppMessage(strMobile, txtSMS.Text, strHttp_File, "MANUAL", "", strFileType);
                    if (strResult != "")
                        _count++;
                }
            }
            catch { }
            return _count;
        }

    }
}
