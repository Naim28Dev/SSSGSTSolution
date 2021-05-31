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
    public partial class SendingEmailPage : Form
    {
        DataBaseAccess dba;
        bool eStatus = false;
        public bool _emailStatus=false;
        string strPartyName="",strAgentName="",strID="",strEmailType="NEW EMAIL";
        public SendingEmailPage()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindPartyNameAndMobileNo();           
        }

        //public SendingEmailPage(bool bStatus, string strPName, string strAName)
        //{
        //    InitializeComponent();
        //    dba = new DataBaseAccess();
        //    strPartyName = strPName;
        //    if (strAName != "SELF")
        //        strAgentName = strAName;
        //    BindPartyNameAndMobileNo();
        //    eStatus = bStatus;
        //}

        public SendingEmailPage(bool bStatus, string strPName, string strAName, string strSubject, string strMsgBody, string strFileName,string strEID,string strEType)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strPartyName = strPName;
            if (strAName != "SELF")
                strAgentName = strAName;
            BindPartyNameAndMobileNo();
            txtSubject.Text = strSubject;
            txtBody.Text = strMsgBody;
            txtFileName.Text = strFileName;
            strID = strEID;
            strEmailType = strEType;
            eStatus = bStatus;
        }

        public SendingEmailPage(string strEmailID,string strSubject, string strMsgBody, string strPath, string strEType)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            eStatus = true;
            txtEmailID.Text = strEmailID;
            txtSubject.Text = strSubject;
            txtBody.Text = strMsgBody;
            txtFileName.Text = strPath;

            strEmailType = strEType;
            btnBrowse.Enabled = false;

            //BindPartyNameAndMobileNo();
        }

        private void BindPartyNameAndMobileNo()
        {
            try
            {

                DataTable dt = dba.GetDataTable("Select dbo.GetFullName((AreaCode+CAST(AccountNo as varchar))) Name,EmailID from SupplierMaster Where EmailID!='' Order by Name");
                if (dt.Rows.Count > 0)
                {
                    dgrdParty.Rows.Add(dt.Rows.Count);
                    int rowIndex = 0;
                    string strName = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        strName = Convert.ToString(row["Name"]);
                        if ((strName == strPartyName && strPartyName!="") || (strName == strAgentName && strAgentName!=""))
                            dgrdParty.Rows[rowIndex].Cells["chkParty"].Value = true;
                        else
                            dgrdParty.Rows[rowIndex].Cells["chkParty"].Value = false;
                        dgrdParty.Rows[rowIndex].Cells["partyName"].Value = strName;
                        dgrdParty.Rows[rowIndex].Cells["emailID"].Value = row["EmailID"];
                        rowIndex++;
                    }
                }

                if (strPartyName != "" || strAgentName != "")
                    txtEmailID.Text = GetSelectedEmailID();               
            }
            catch
            {
            }
        }

        private void SendingSMSPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !txtEmailID.Focused && !txtBody.Focused)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetSelectedEmailID()
        {
            string strEmailID = "";
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    bool chkStatus = Convert.ToBoolean(row.Cells["chkParty"].Value);
                    if (chkStatus)
                    {
                        if (strEmailID == "")
                            strEmailID = Convert.ToString(row.Cells["emailID"].Value);
                        else
                            strEmailID += "," + row.Cells["emailID"].Value;
                    }
                }
            }
            catch
            {
            }
            return strEmailID;
        }

        private void SendingEmailPage_Load(object sender, EventArgs e)
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

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);

                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                    txtPartyName.Focus();
                    GetPartyEmailID();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;
                txtPartyName.Focus();
                GetPartyEmailID();
            }
            catch
            {
            }
        }

        private void GetPartyEmailID()
        {
            try
            {
                if (txtPartyName.Text != "")
                {
                    string strQuery = "Select EmailID from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtPartyName.Text + "' ";
                    object _objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                    if (Convert.ToString(_objValue) != "")
                    {
                        if (txtEmailID.Text != "")
                            txtEmailID.Text += ",";
                        txtEmailID.Text += Convert.ToString(_objValue);
                    }
                }
            }
            catch { }            
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                    row.Cells["chkParty"].Value = chkAll.Checked;
            }
            catch
            {
            }
        }

        private void btnCAdd_Click(object sender, EventArgs e)
        {
            txtEmailID.Text = GetSelectedEmailID();
            txtSubject.Focus();
            
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                btnEmail.Enabled = false;
                if (txtBody.Text != "" && txtEmailID.Text != "" && txtSubject.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Send Email ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = 0, failedCount = 0;
                        string[] strEmails = txtEmailID.Text.Split(',');
                        bool emailStatus = false;
                        foreach (string strEmail in strEmails)
                        {
                            emailStatus = DataBaseAccess.SendEmail(strEmail, txtSubject.Text, txtBody.Text, txtFileName.Text, strID, strEmailType, false);
                            if (emailStatus)
                                count++;
                            else
                                failedCount++;
                        }

                        if (count > 0)
                        {
                            MessageBox.Show(count + " email sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            txtFileName.Text = txtSubject.Text = txtBody.Text = txtEmailID.Text = "";
                            _emailStatus = true;
                            if (eStatus)
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
                    MessageBox.Show("Sorry ! Please fill the message box and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnEmail.Enabled = true;
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

    }
}
