using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class ActivateSoftware : Form
    {
        RegistrationClass obj;
        public bool _activationSTatus = false;
        string strActivationKey = "",strMachineID="",strEmailID="";
        public ActivateSoftware(string strKey,string strMID,string strEmail)
        {
            InitializeComponent();
            strActivationKey = strKey;
            strMachineID = strMID;
            strEmailID = strEmail;

            obj = new SSS.RegistrationClass();
        }

        private void ActivateSoftware_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _activationSTatus = false;
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtActivationKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            obj.ValidateSpace(sender, e);
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtActivationKey.Text != "" && txtActivationKey.Text.Length > 11)
                {
                    if (strActivationKey == txtActivationKey.Text)
                    {
                        int _count = obj.UpdateActivationKey(txtActivationKey.Text, strEmailID, strMachineID);
                        if (_count > 0)
                        {
                            _activationSTatus = true;
                            MessageBox.Show("Thank you ! Your registration activated succesfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            this.Close();
                        }
                        else { MessageBox.Show("Sorry ! Unable to activate right now, Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }                    
                        else { MessageBox.Show("Sorry ! Activation key is not valid, Please valid activation key.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter activation key.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtActivationKey.Focus();
                }
            }
            catch { }
        }
    }
}
