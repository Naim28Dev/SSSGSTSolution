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
    public partial class ChangePassword : Form
    {
        DataBaseAccess dba;
        public ChangePassword()
        {
            InitializeComponent();
            dba = new DataBaseAccess();

            lblWelcome.Text = "WELCOME TO :  " + MainPage.strLoginName;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (txtConfirmPass.Text == txtNewPassword.Text)
            {
                string strLoginName = MainPage.strLoginName;

                string strOldPass;
                if (MainPage.strLoginName == "ADMIN" || MainPage.strLoginName == "SUPERADMIN")
                {
                    strOldPass = dba.GetAdminPassword(strLoginName);
                }
                else
                {
                    strOldPass = dba.GetUserPassword(strLoginName);
                }

                if (strOldPass == txtOldPassword.Text)
                {
                    if (txtNewPassword.Text != "" && txtConfirmPass.Text != "")
                    {
                        DialogResult dr = MessageBox.Show("Are you sure want to Change Password", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            string strQuery = "";
                            if (MainPage.strLoginName == "ADMIN" || MainPage.strLoginName == "SUPERADMIN")
                            {
                                strQuery = "Update Admin set Password='" + txtNewPassword.Text + "' Where UserName='"+strLoginName+"' ";
                            }
                            else
                            {
                                strQuery = "Update UserAccount set Password='" + txtNewPassword.Text + "' where LoginName='" + strLoginName + "'";
                            }

                            strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                     + " ('PASSWORDCHANGE','" + MainPage.strLoginName + "',0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',0,0,'UPDATION') ";


                            int result = dba.ExecuteMyQuery(strQuery);
                            if (result > 0)
                            {
                                MessageBox.Show("Password has been Changed ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtOldPassword.Clear();
                                txtConfirmPass.Clear();
                                txtNewPassword.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Sorry Unable to Change Password ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Old Password doesn't Matched  !  Please Try Again  !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Sorry ! Password and Confirm Password is not matched !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangePassword_KeyDown(object sender, KeyEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtConfirmPass_Leave(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Password and Confirm Password is not Match", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
              //  txtConfirmPass.Focus();
            }
        }

    }
}
