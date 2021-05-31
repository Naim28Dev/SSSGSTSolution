using System;
using System.Data;
using System.Windows.Forms;

namespace SSS
{
    public partial class AdminPanel : Form
    {
        DataBaseAccess dba;        
        SendSMS mySMS;

        public AdminPanel()
        {
            InitializeComponent();
            dba = new DataBaseAccess();            
            mySMS = new SendSMS();
            if (MainPage.strUserRole == "SUPERADMIN")
                btnSynchronize.Enabled = true;
            GetGroupRecord();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            changePanel.Enabled = true;
        }

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            CreateUser cu = new CreateUser();
            cu.MdiParent = MainPage.mymainObject;
            cu.Show();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CreateUser cu = new CreateUser("Update");
            cu.MdiParent = MainPage.mymainObject;
            cu.Show();
        }

        private void GetGroupRecord()
        {
            try
            {
                dgrdParty.Rows.Clear();
                double dCount = 0, dTCount = 0;
                DataTable dt = dba.GetNumberofPartiesByGroup();
                if (dt.Rows.Count > 0)
                {
                    dgrdParty.Rows.Add(dt.Rows.Count);
                    int rowCount = 0;
                    foreach(DataRow row in dt.Rows)
                    {
                        dTCount += dCount = dba.ConvertObjectToDouble(row["SCount"]);
                        dgrdParty.Rows[rowCount].Cells["sno"].Value = (rowCount + 1) + ".";
                        dgrdParty.Rows[rowCount].Cells["groupName"].Value = row["GroupName"];
                        dgrdParty.Rows[rowCount].Cells["count"].Value = dCount.ToString("N0",MainPage.indianCurancy);
                        rowCount++;
                    }

                }

                lblNo.Text = dTCount.ToString("N0", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string strOldPass = dba.GetAdminPassword(MainPage.strLoginName);
                if (strOldPass == txtOldPassword.Text && txtNewPassword.Text != "" && txtConfirmPass.Text != "")
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Change Password", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strQuery = "Update Admin set Password='" + txtNewPassword.Text + "' Where UserName='"+MainPage.strLoginName+"' ";

                        int result = dba.ExecuteMyQuery(strQuery);
                        if (result > 0)
                        {
                            MessageBox.Show("Password has been Changed ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            changePanel.Enabled = false;
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
            catch
            {
            }
        }

        private void txtConfirmPass_Leave(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Password and Confirm Password is not Match", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            changePanel.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdminPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); 
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {
            //lblBalance.Text = mySMS.GetSMSBalance();
            if (!MainPage.strUserRole.Contains("ADMIN"))
                btnMonthLock.Enabled =btnLock.Enabled= false;

        }

        private void dgrdParty_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to lock this quarter ? ", "Lock This Quarter", MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    string strQuery = " Update  UserAccount set JournalEntry='False',JournalEdit='False',CashEntry='False',CashEdit='False',OrderEntry='False',OrderEdit='False' ,GoodsEntry='False',"
                                  + "GoodsEdit='False',SaleEntry='False' ,SaleEdit='False' ,PurchaseEntry='False',PurchaseEdit='False',ForwardingEntry='False',ForwardingEdit='False',CourierEntry='False',CourierEdit='False' ,NewParty='False',NewPartyEdit='False',"
                                  + "NewSubParty='False',SubPartyEdit='False' ,NewAccountmaster='False' ,AccountMasterEdit='False' ,Merging='False',BackDateEntry='False',Reminder='False'  Update CompanySetting Set StandardLogin='NO' ";
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Quarter locked successfully ! ", "Locked", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please try after some time ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch
            {
            }
        }

        private void btnMonthLock_Click(object sender, EventArgs e)
        {
            Month_Lock objMonth_Lock = new Month_Lock();
            objMonth_Lock.MdiParent = MainPage.mymainObject;
            objMonth_Lock.Show();
        }

        private void btnSynchronize_Click(object sender, EventArgs e)
        {
            try
            {
                btnSynchronize.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to Synchronize data with internet ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result==DialogResult.Yes)
                {
                    //double count = DataBaseAccess.GetAllInsertedRecordAtOnce();
                    //if (count > 0)
                    //{
                    //    MessageBox.Show("Thank you !! Record synchronized successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //}
                    //else
                    //    MessageBox.Show("Sorry !! Unable to synchronize", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex) { MessageBox.Show("Sorry !! "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnSynchronize.Enabled = true;
        }
    }
}
