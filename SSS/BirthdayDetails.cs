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
    public partial class BirthdayDetails : Form
    {
        DataBaseAccess dba;
        bool _bSearch = false;
        public BirthdayDetails()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }

        public BirthdayDetails(bool _bStatus)
        {
            InitializeComponent();
            _bSearch = _bStatus;
            dba = new SSS.DataBaseAccess();
        }

        private void BirthdayDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {               
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void txtDOBFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDOBFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, false);
        }

        private void chkDOBDate_CheckedChanged(object sender, EventArgs e)
        {
            txtDOBFromDate.ReadOnly = txtDOBToDate.ReadOnly = !chkDOBDate.Checked;
            txtDOBFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtDOBToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkDOA_CheckedChanged(object sender, EventArgs e)
        {
            txtDOAFromDate.ReadOnly = txtDOAToDate.ReadOnly = !chkDOA.Checked;
            txtDOAFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtDOAToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", txtGroupName.Text, "SEARCH ACCOUNT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;

            }
            catch
            {
            }
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupName.Text = objSearch.strSelectedData;                   
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", txtGroupName.Text, "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                else
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
                SearchData objSearch = new SearchData("ALLPARTY", txtGroupName.Text, "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;

            }
            catch
            {
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtCategory.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDOBDate.Checked && (txtDOBFromDate.Text.Length != 10 || txtDOBToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill DOB Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkDOA.Checked && (txtDOAFromDate.Text.Length != 10 || txtDOAToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill DOA Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetDataFromDB();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strSubQuery = "";
            string[] strFullName;
            if (txtPartyName.Text != "")
            {
                strFullName = txtPartyName.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSubQuery += " and (SM.AreaCode+SM.AccountNo) = '" + strFullName[0].Trim() + "'  ";
            }

            if (txtGroupName.Text != "")
                strSubQuery += " and SM.GroupName='" + txtGroupName.Text + "' ";
            if (txtCategory.Text != "")
                strSubQuery += " and SM.Category='" + txtCategory.Text + "' ";
            if (txtBranchCode.Text != "")
                strSubQuery += " and SM.AreaCode='" + txtBranchCode.Text + "' ";

            if (chkDOBDate.Checked && txtDOBFromDate.Text.Length == 10 && txtDOBToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtDOBFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtDOBToDate.Text);
                strSubQuery += " and (CDOB>='" + sDate.Month + "/" + sDate.Day + "/2020' and CDOB<='" + eDate.Month + "/" + eDate.Day + "/2020' )";
            }

            if (chkDOA.Checked && txtDOAFromDate.Text.Length == 10 && txtDOAToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtDOAFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtDOAToDate.Text);
                strSubQuery += " and (CDOA>='" + sDate.Month + "/" + sDate.Day + "/2020' and CDOA<='" + eDate.Month + "/" + eDate.Day + "/2020' )";
            }

            return strSubQuery;
        }

        private void GetDataFromDB()
        {
            string strQuery = "", strSubQuery = CreateQuery();
            strQuery = " Select (SM.AreaCode+SM.AccountNo+' '+Name) PartyName,SM.Other as SSSName,ContactPerson,GroupName,Category,MobileNo,WhatsappNo,EmailID,CONVERT(varchar,(CASE WHEN ISNULL(DOB,'')<>'' then DOB else NULL end),103) as DOB,CONVERT(varchar,(CASE WHEN ISNULL(DOA,'')<>'' then DOA else NULL end),103) as DOA,SpouseName,CDOB,CDOA,CDate,(CASE WHEN DATEDIFF(dd,CDate,CDOB)<0 then DATEDIFF(dd,CDate,CDOB)+365 else DATEDIFF(dd,CDate,CDOB) end)CDays  from SupplierMaster SM CROSS APPLY (Select WaybillUserName as WhatsappNo,SpouseName,(CASE WHEN DOB='1900-01-01 00:00:00.000' then NULL else DOB end) DOB,(CASE WHEN DOA='1900-01-01 00:00:00.000' then NULL else DOA end) DOA,(CASE WHEN DOB<>'1900-01-01 00:00:00.000' then CONVERT(Date,''+CAST(DATEPART(MM,DOB) as varchar)+'/'+CAST(DATEPART(dd,DOB) AS varchar)+'/2020',0) else NULL end) CDOB,(CASE WHEN DOA<>'1900-01-01 00:00:00.000' then CONVERT(Date,''+CAST(DATEPART(MM,DOA) as varchar)+'/'+CAST(DATEPART(dd,DOA) AS varchar)+'/2020',0) else NULL end) CDOA,CONVERT(Date,''+CAST(DATEPART(MM,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) as varchar)+'/'+CAST(DATEPART(dd,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) AS varchar)+'/2020',0) as CDate from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where TransactionLock=0 and  (ISNULL(SOD.DOB,'')!='' OR ISNULL(SOD.DOA,'')!='') and DOB is NOT NULL " + strSubQuery + " Order by (CASE WHEN DATEDIFF(dd,CDate,CDOB)<0 then DATEDIFF(dd,CDate,CDOB)+365 else DATEDIFF(dd,CDate,CDOB) end) ";

            if (rdoUpcoming.Checked)
                strQuery += " asc ";
            else if (rdoBelated.Checked)
                strQuery += " desc ";
            DataTable dt = dba.GetDataTable(strQuery);

            BindRecordWithControl(dt);
        }

        private void BindRecordWithControl(DataTable dt)
        {
            try
            {
                dgrdDetails.Rows.Clear();
                int _index = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_index].Cells["chkTick"].Value = false;
                        dgrdDetails.Rows[_index].Cells["sno"].Value = ((_index+1) + ".");
                        dgrdDetails.Rows[_index].Cells["accountName"].Value = row["PartyName"];
                        dgrdDetails.Rows[_index].Cells["groupName"].Value = row["GroupName"];
                        dgrdDetails.Rows[_index].Cells["dob"].Value = Convert.ToString(row["DOB"]);
                        dgrdDetails.Rows[_index].Cells["doa"].Value = Convert.ToString(row["DOA"]);
                        dgrdDetails.Rows[_index].Cells["spouseName"].Value = row["SpouseName"];
                        dgrdDetails.Rows[_index].Cells["MobileNo"].Value = row["mobileNo"];
                        dgrdDetails.Rows[_index].Cells["whatsappNo"].Value = row["whatsappNo"];
                        dgrdDetails.Rows[_index].Cells["emailID"].Value = row["EmailID"];
                        dgrdDetails.Rows[_index].Cells["ContactPerson"].Value = row["ContactPerson"];
                        dgrdDetails.Rows[_index].Cells["cdob"].Value = row["CDOB"];
                        dgrdDetails.Rows[_index].Cells["cdoa"].Value = row["CDOA"];
                        dgrdDetails.Rows[_index].Cells["cDate"].Value = row["CDate"];
                        dgrdDetails.Rows[_index].Cells["sssName"].Value = row["SSSName"];

                        if (Convert.ToString(row["CDays"]) == "0")
                            dgrdDetails.Rows[_index].DefaultCellStyle.BackColor = Color.LightGreen;
                        
                        _index++;
                    }
                }
            }
            catch { throw; }
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != 0)
                    e.Cancel = true;
            }
            catch { }
        }

        private void SendBestWishes()
        {
            try
            {
                string strName = "", strGroupName = "",strSSSName="", strContactPerson = "", strMobileNo = "", strDOB = "", strDOA = "",strDate="", strWhatsappNo = "", strEmail = "",  strReason = "", strQuery = "", strStatus = "";
                bool _bSendStatus = false;
                int count = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkTick"].Value))
                    {
                        strName = Convert.ToString(row.Cells["accountName"].Value);
                        strGroupName = Convert.ToString(row.Cells["groupName"].Value);
                        strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value);
                        strWhatsappNo = Convert.ToString(row.Cells["whatsappNo"].Value);
                        strEmail = Convert.ToString(row.Cells["emailID"].Value);
                       // strWishType = Convert.ToString(row.Cells["WishType"].Value);
                        strDOB = Convert.ToString(row.Cells["cdob"].Value);
                        strDOA = Convert.ToString(row.Cells["cdoa"].Value);
                        strDate = Convert.ToString(row.Cells["cDate"].Value);
                        strSSSName= Convert.ToString(row.Cells["sssName"].Value);

                        strContactPerson = Convert.ToString(row.Cells["ContactPerson"].Value).Trim();
                        if (strContactPerson == "")
                            strContactPerson = strName;

                        strReason = "";
                        if (strDOB == strDate)
                        {
                            _bSendStatus = dba.SendBirthdayMessage(strContactPerson, strGroupName, strMobileNo, strWhatsappNo, strEmail, "BIRTHDAY", ref strReason);
                        }
                        if (strDOA == strDate)
                        {
                            _bSendStatus = dba.SendAnniversaryMessage(strContactPerson, strGroupName, strMobileNo, strWhatsappNo, strEmail, "ANNIVERSARY", ref strReason);
                        }

                        string[] strFullName = strName.Split(' ');

                        if (_bSendStatus)
                            strStatus = "SENT";
                        else
                            strStatus = "FAILED";

                        strQuery = " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason]) VALUES "
                              + "('WISHES','" + strFullName[0] + "',0,DATEPART(MM,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),0,'" + MainPage.strLoginName + "',1,0,'" + strStatus + "','" + strReason + "') ";

                        dba.ExecuteMyQuery(strQuery);
                        count++;
                    }
                }

                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Wishes sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    MessageBox.Show("Sorry please select atleast one account for sending wishes." , "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }            
        }

        private void btnSendWhatsappMessage_Click(object sender, EventArgs e)
        {
            btnSendWhatsappMessage.Enabled = false;
               DialogResult result = MessageBox.Show("Are you sure you want to send the best wishesh to selected account ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                SendBestWishes();
            btnSendWhatsappMessage.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BirthdayDetails_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (_bSearch)
                btnGo.PerformClick();
        }
    }
}

