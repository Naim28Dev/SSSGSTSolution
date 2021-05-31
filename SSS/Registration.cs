using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace SSS
{
    public partial class Registration : Form
    {
        RegistrationClass objReg;
        public bool _activationStatus = false;
        const string strConnection = "";
        public Registration()
        {
            try
            {
                InitializeComponent();
                objReg = new RegistrationClass();
                txtMachineID.Text = RegistrationClass.GetMachineID();
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Registration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {                
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            objReg.KeyHandlerPoint(sender, e, 0);
        }

        private void txtEmailID_KeyPress(object sender, KeyPressEventArgs e)
        {
            objReg.ValidateSpace(sender,e);
        }

        private string PlanType
        {
            get
            {
                if (rdoSilver.Checked)
                    return "SILVER";
                else if (rdoGold.Checked)
                    return "GOLD";
                else
                    return "DIAMOND";

            }
        }

        private void btnConfirmation_Click(object sender, EventArgs e)
        {
            try
            {
                btnConfirmation.Enabled = false;
                if(ValidateControl())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to register with us?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(result==DialogResult.Yes)
                    {
                        string strPlanType="", strKey = objReg.CheckMachineID(txtEmailID.Text, txtMachineID.Text, ref strPlanType);
                        int _count = 0;
                        if (strKey.Length > 10)
                        {
                            MessageBox.Show("Sorry ! This machine is already registered with this email ID : " + txtEmailID.Text + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (strKey == "KEY")
                        {
                            //if (strPlanType != "")
                            {
                                if (strPlanType != PlanType && strPlanType!="")
                                {
                                    MessageBox.Show("Sorry ! This email has been registered with plan type : " + strPlanType + ", So please select " + PlanType + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    _count = objReg.SaveUserInfo(txtEmailID.Text, txtContactPerson.Text, txtMobileNo.Text, txtMachineID.Text, txtDate.Text, PlanType, txtSoftwareType.Text, "LIVE");
                            }
                            //else
                            //    MessageBox.Show("Sorry ! Plan type : " + strPlanType ,"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        
                        if(_count>0 || strKey == "")
                        {
                            //Genrate ActivationKey and Send it to Email 
                            string strActivationKey = GenerateActivationKey();
                            bool _bStatus = SendEmail(strActivationKey);
                            if(_bStatus)
                            {
                                this.Hide();
                                ActivateSoftware obj = new ActivateSoftware(strActivationKey,txtMachineID.Text,txtEmailID.Text);
                                obj.ShowDialog();
                                if (obj._activationSTatus)
                                {
                                    _activationStatus = obj._activationSTatus;
                                    this.Close();
                                }
                                else
                                    MainPage.mymainObject.Close();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnConfirmation.Enabled = true;
        }

        private string GetMessages(string strActivationKey)
        {
            string strMessage = "";

            strMessage += " <table border='1'> "
                       + " <tr><td>Email ID</td><td>" + txtEmailID.Text + "</td></tr> "
                       + " <tr><td>Contact Person</td><td>" + txtContactPerson.Text + "</td></tr> "
                       + " <tr><td>Mobile No</td><td>" + txtMobileNo.Text + "</td></tr> "
                       + " <tr><td>Machine ID</td><td>" + txtMachineID.Text + "</td></tr> "
                       + " <tr><td>Software Type</td><td>" + txtSoftwareType.Text + "</td></tr> "
                       + " <tr><td>Plan Type</td><td>" + PlanType + "</td></tr> "
                       + " <tr><td>Activation Key</td><td>" + strActivationKey + "</td></tr> "
                       + " <tr><td>Date</td><td>" + txtDate.Text + "</td></tr> "
                       + " </table> ";

            return strMessage;
        }

        private string GetTextMessages(string strActivationKey)
        {
            string strMessage = "";

            strMessage += "Email ID : " + txtEmailID.Text + "\n"
                       + "Contact Person : " + txtContactPerson.Text + "\n"
                       + "Mobile No : " + txtMobileNo.Text + "\n"
                       + "Machine ID : " + txtMachineID.Text + "\n"
                       + "Software Type : " + txtSoftwareType.Text + "\n"
                       + "Plan Type : " + PlanType + "\n"
                       + "Activation Key : " + strActivationKey + "\n"
                       + "Date : " + txtDate.Text + " ";
                     

            return strMessage;
        }

        private bool SendEmail(string strKey)
        {
            string strMessage = GetMessages(strKey);

            bool _bstatus = SendMail.SendEmailToCompany("info@ssssybertech.com", "Activation Key",strMessage, "", "", "ACTIVATION", true);
            if (!_bstatus)
            {
                DialogResult _updateResult = MessageBox.Show("Sorry ! Unable to send email, Please try again !! ", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (_updateResult == DialogResult.Retry)
                {
                    _bstatus = SendMail.SendEmailToCompany("chandra@ssssybertech.com", "Activation Key", strMessage, "", "", "ACTIVATION", true);
                }
            }
            if (!_bstatus)
            {
                SendSMS _sms = new SendSMS();
                string strResult = _sms.SendSingleSMSWithoutSaveINDB(GetTextMessages(strKey), "8802872474,9873316638");
                if (strResult.Contains("success"))
                    return true;
            }
            return _bstatus;
        }

        private string GenerateActivationKey()
        {
            int _firstNumber = 0, _secondNumber = 0, _thirdNumber = 0, _fourthNumber = 0;
            string strKey = "";
            Random obj = new Random();
            _firstNumber = obj.Next(1000, 9999);
            _secondNumber = obj.Next(2000, 9999);
            _thirdNumber = obj.Next(3000, 9999);
            _fourthNumber = obj.Next(4000, 9999);
            strKey = _firstNumber + "-" + _secondNumber + "-" + _thirdNumber + "-" + _fourthNumber;
            return strKey;
        }

        


        private bool ValidateControl()
        {
            if(txtEmailID.Text=="")
            {
                MessageBox.Show("Sorry ! Email id can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailID.Focus();
                return false;
            }
            if (!txtEmailID.Text.Contains("@"))
            {
                MessageBox.Show("Sorry ! Email id not valid !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailID.Focus();
                return false;
            }
            if (txtContactPerson.Text == "")
            {
                MessageBox.Show("Sorry ! Contact person name can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactPerson.Focus();
                return false;
            }
            if (txtMachineID.Text == "")
            {
                MessageBox.Show("Sorry ! Machine ID can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMachineID.Focus();
                return false;
            }
            if (txtSoftwareType.Text == "")
            {
                MessageBox.Show("Sorry ! Software type can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoftwareType.Focus();
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSoftwareType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SOFTWARETYPE", "SEARCH SOFTWARE TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtSoftwareType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }
    }
}
