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
    public partial class SendUnicodeSMS : Form
    {
        public SendUnicodeSMS()
        {
            InitializeComponent();
        }

        private void SendUnicodeSMS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnSMS_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtSMS.Text != "" && txtMobileNo.Text.Length > 9)
                {
                    //if (rdoEnglish.Checked || (rdoHindi.Checked && txtSMS.Text.Length < 100))
                    //{
                        DialogResult result = MessageBox.Show("Are you sure want to Send SMS ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SendSMS sendMessage = new SendSMS();
                            string strSMS = ConvertUnicodeSMS();
                            string strResult = "";
                            if (rdoHindi.Checked)
                                strResult = sendMessage.SendSingleSMSWithUnicode(strSMS, txtMobileNo.Text);
                            else
                                strResult = sendMessage.SendSingleSMS(strSMS, txtMobileNo.Text);
                            if (strResult.Contains("success"))
                            {
                                MessageBox.Show("Message Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtSMS.Clear();
                                txtMobileNo.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please Try Again  ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    //}
                    //else
                    //    MessageBox.Show("Sorry ! Message length must be smaller than 100 in hindi.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

        private string ConvertUnicodeSMS()
        {
            if (rdoHindi.Checked)
            {
                return txtSMS.Text.Replace(" ", "+");
            }
            else
                return txtSMS.Text;
          
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSMS_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lblCharCount.Text = "Char Count : " + txtSMS.Text.Length.ToString();
                if (txtSMS.Text.Length % 160 != 0)
                {
                    lblSMSCount.Text = "SMS Count : " + ((txtSMS.Text.Length / 160) + 1).ToString();
                }
                else
                {
                    lblSMSCount.Text = "SMS Count : " + (txtSMS.Text.Length / 160).ToString();
                }
            }
            catch
            {
                lblSMSCount.Text = "1";
            }
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

    }
}
