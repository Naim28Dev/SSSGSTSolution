using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class SendSMSPage : Form
    {
        bool _bClosingStatus = false;
        public SendSMSPage(string strMobileNo, string strMessage)
        {
            InitializeComponent();
            _bClosingStatus = true;
            txtMobileNo.Text = strMobileNo;
            txtSMS.Text = strMessage;
            if (!MainPage.strUserRole.Contains("ADMIN"))
                txtSMS.ReadOnly = true;
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

                if (txtSMS.Text != "" && txtMobileNo.Text.Length > 9)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Send SMS ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SendSMS sendMessage = new SendSMS();
                        string strSMS = txtSMS.Text;
                        string strResult = "";

                        strResult = sendMessage.SendSingleSMS(strSMS, txtMobileNo.Text);
                        if (strResult.Contains("success"))
                        {
                            MessageBox.Show("Message Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            txtSMS.Clear();
                            txtMobileNo.Clear();
                            if (_bClosingStatus)
                                this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Please Try Again  ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtMobileNo.Clear();
            if(txtPartyName.Text!="")
            {
                DataBaseAccess dba = new SSS.DataBaseAccess();
                txtMobileNo.Text = Convert.ToString(dba.GetPartyMobileNo(txtPartyName.Text));
            }
        }
    }
}
