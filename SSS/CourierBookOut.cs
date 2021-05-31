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
    public partial class CourierBookOut : Form
    {
        DataBaseAccess dba;
        string strLastID = "";
        bool bNewStatus = false;
        public CourierBookOut()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData(true);
        }

        public CourierBookOut(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData(false);
            bNewStatus = bStatus;
        }

        public CourierBookOut(string strCode, string strID)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtCode.Text = strCode;
            if(strCode=="")
            GetStartupData(false);
            BindRecordWithControl(strID);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select CourierCode,SBillCode,(Select MAX(ID) from CourierRegister CR Where CR.SCode=CS.CourierCode)ID from CompanySetting CS Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtCode.Text = Convert.ToString(dt.Rows[0]["CourierCode"]);
                    txtBillCode.Text = Convert.ToString(dt.Rows[0]["SBillCode"]);
                    strLastID = Convert.ToString(dt.Rows[0]["ID"]);
                }
                if (strLastID != "" && strLastID != "0" && bStatus)
                    BindRecordWithControl(strLastID);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Get StartupData in Courier Book Out", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CourierBookOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        BindNextRecord();
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        BindPreviousRecord();
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        BindFirstRecord();
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        BindLastRecord();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.E)
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && strLastID != "")
                        {
                            BindRecordWithControl(strLastID);
                        }
                    }
                }
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ID),'') from CourierRegister Where SCode='" + txtCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ID),'') from CourierRegister Where SCode='" + txtCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            if (strLastID != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ID),'') from CourierRegister Where SCode='" + txtCode.Text + "' and ID>" + strLastID + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
                else
                    BindLastRecord();
            }
            else
                ClearAllText();
        }

        private void BindPreviousRecord()
        {
            if (strLastID != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ID),'') from CourierRegister Where SCode='" + txtCode.Text + "' and ID<" + strLastID + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void ClearAllText()
        {
            txtSNo.Text = txtSerialCode.Text =  txtCourierNo.Text = txtPartyName.Text = txtRemark.Text = txtStation.Text = lblMsg.Text = lblCreatedBy.Text =txtBillNo.Text= "";
            
            chkSendSMS.Checked = false;
            // txtCourierName.Text =txtDocType.Text = "BILL";
            //if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
            //    txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            //else
            //    txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void BindRecordWithControl(string strID)
        {
            if (strID != "")
            {
                DisableAllControls();
                DataTable dt = dba.GetDataTable("Select *,CONVERT(varchar,Date,103) CDate,dbo.GetFullName(SalePartyID) PartyName from CourierRegister Where ID=" + strID + " and SCode='" + txtCode.Text + "'");
                BindControlfromDatatTable(dt);
            }
        }

        private void BindRecordWithControlWithSNo(string strSNo)
        {
            if (strSNo != "")
            {
                DisableAllControls();
                DataTable dt = dba.GetDataTable("Select *,CONVERT(varchar,Date,103) CDate,dbo.GetFullName(SalePartyID) PartyName from CourierRegister Where SNo=" + strSNo + " and SCode='" + txtCode.Text + "' and SerialCode='"+txtSerialCode.Text+"' ");
                BindControlfromDatatTable(dt);
            }
        }

        private void BindControlfromDatatTable(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtCode.Text = Convert.ToString(row["SCode"]);
                txtSNo.Text = Convert.ToString(row["SNo"]);
                txtSerialCode.Text = Convert.ToString(row["SerialCode"]);
                txtCourierNo.Text = Convert.ToString(row["CourierNo"]);
                txtCourierName.Text = Convert.ToString(row["CourierName"]);
                txtDate.Text = Convert.ToString(row["CDate"]);
                txtDocType.Text = Convert.ToString(row["DocType"]);
                txtPartyName.Text = Convert.ToString(row["PartyName"]);
                txtStation.Text = Convert.ToString(row["Station"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtBillCode.Text = Convert.ToString(row["SaleBillCode"]);
                txtBillNo.Text = Convert.ToString(row["SaleBillNo"]);
                if (txtBillNo.Text == "0")
                    txtBillNo.Text = "";
                strLastID = Convert.ToString(row["ID"]);
                string strCreatedBy = Convert.ToString(row["UserName"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;
                lblMsg.Text = "";
            }
            else
                ClearAllText();
        }

        private void EnableAllControls()
        {
            txtSNo.ReadOnly = txtSerialCode.ReadOnly = txtCourierNo.ReadOnly = txtRemark.ReadOnly = txtDate.ReadOnly = false;
        }

        private void DisableAllControls()
        {
           txtCourierNo.ReadOnly = txtRemark.ReadOnly = txtDate.ReadOnly = true;
            txtSNo.ReadOnly = txtSerialCode.ReadOnly = false;
        }

        private void SetSerialNo()
        {
            try
            {
                string strQuery = "select ISNULL(Max(SNo)+1,1) as SNo from CourierRegister Where SCode='" + txtCode.Text + "' ";
                object objSerialNo = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (objSerialNo != null)
                {
                    txtSNo.Text = Convert.ToString(objSerialNo);
                }
            }
            catch { }
        }

        private bool ValidateControls()
        {
            if (txtCode.Text == "")
            {
                MessageBox.Show(" Please Enter courier code ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }
            if (txtSNo.Text == "")
            {
                MessageBox.Show(" Please Enter Serial No ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSNo.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Date is not valid, Please enter valid date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            //if (txtCourierNo.Text == "")
            //{
            //    MessageBox.Show(" Please enter Courier no ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtCourierNo.Focus();
            //    return false;
            //}
            if (txtDocType.Text == "")
            {
                MessageBox.Show(" Please enter Doc Type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDocType.Focus();
                return false;
            }
            if (txtStation.Text == "")
            {
                MessageBox.Show(" Please enter Station Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStation.Focus();
                return false;
            }
            return CheckAvailability();
        }

        private bool CheckAvailability()
        {
            bool bStatus = CheckCourierSerialNoAvailability();
            if (bStatus)
            {
                lblMsg.Text = txtSNo.Text + " " + txtSerialCode.Text + "  Serial No is Available ........";
                lblMsg.ForeColor = Color.Green;
                return true;
            }
            else
            {
                lblMsg.Text = txtSNo.Text + " " + txtSerialCode.Text + "  Serial No is Already Exist ! ";
                lblMsg.ForeColor = Color.Red;
                txtSerialCode.Focus();
                return false;
            }
        }

        public bool CheckCourierSerialNoAvailability()
        {
            string strQuery = "Select ISNULL(Count(*),0) from CourierRegister where SCode='" + txtCode.Text + "' and SNo=" + txtSNo.Text + " and SerialCode='" + txtSerialCode.Text + "' ", strSubQuery = "";
            if (btnEdit.Text == "&Update")
                strSubQuery += " and ID!=" + strLastID + " ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery + strSubQuery);
            if (dba.ConvertObjectToDouble(objValue) > 0)
                return false;
            else
                return CheckPreviousPartyName();
        }

        private bool CheckPreviousPartyName()
        {
            try
            {
                string strQuery = "Select dbo.GetFullName(SalePartyID) SalesParty from CourierRegister where SCode='" + txtCode.Text + "' and SNo=" + txtSNo.Text + " ", strSubQuery = "";
                if (btnEdit.Text == "&Update")
                    strSubQuery += " and ID!=" + strLastID + " ";
                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery + strSubQuery);
                string strParty = Convert.ToString(objValue);
                if (strParty != "")
                {
                    if (strParty != txtPartyName.Text)
                    {
                        MessageBox.Show("Party Name of this Courier doesn't Match , Party Name :  " + strParty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                return CheckCourierNoExistence();
            }
            catch
            {
                return false;
            }
        }

        private bool CheckCourierNoExistence()
        {
            if (txtCourierNo.Text != "" && txtCourierNo.Text != "BY HAND" && txtCourierNo.Text != "BYHAND")
            {
                object objCourierNo = DataBaseAccess.ExecuteMyScalar(" Select SNo from CourierRegister Where SCode='" + txtCode.Text + "' and CourierNo='" + txtCourierNo.Text + "' and SNo!=" + txtSNo.Text + " ");
                string strBillNo = Convert.ToString(objCourierNo);
                if (strBillNo != "")
                {
                    MessageBox.Show(" This courier no. is already exist in Serial No : " + strBillNo, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //txtCourierNo.Focus();
                    //return false;
                }
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnEdit.Text = "&Edit";
                    }
                    
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();
                    txtCourierNo.Focus();
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Adding in Courier Out Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strQuery = "", strDate = "", strParty = "", strPartyID = "" ;
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtPartyName.Text != "")
                {
                    string[] strFullName = txtPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strPartyID = strFullName[0].Trim();
                }

                strParty = txtPartyName.Text.Replace(strPartyID + " ", "");

                strQuery = " if not exists (Select ID from CourierRegister where SCode='" + txtCode.Text + "' and SNo=" + txtSNo.Text + " and SerialCode='" + txtSerialCode.Text + "') begin "
                              + " INSERT INTO [dbo].[CourierRegister] ([SCode],[SNo],[SerialCode],[CourierNo],[CourierName],[Date],[DocType],[SalesParty],[Station],[Remark],[UserName],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleBillCode],[SaleBillNo],[SalePartyID]) VALUES "
                              + " ('" + txtCode.Text + "'," + txtSNo.Text + ",'" + txtSerialCode.Text + "','" + txtCourierNo.Text + "','" + txtCourierName.Text + "','" + strDate + "','" + txtDocType.Text + "','" + strParty + "','" + txtStation.Text + "','" + txtRemark.Text + "','" + MainPage.strLoginName + "','',1,0,'" + txtBillCode.Text + "','" + txtBillNo.Text + "','" + strPartyID + "') "
                              + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                              + "('COURIEROUT','" + txtCode.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    SendSMSToParty();
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    btnAdd.PerformClick();
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Courier out Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Enabled = false;
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnAdd.Text = "&Add";
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    txtCourierNo.Focus();
                }
                else if (ValidateControls() && strLastID != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        UpdateRecord();
                    }
                }
            }
            catch
            {
            }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord()
        {
            try
            {
                string strQuery = "", strDate = "", strParty = "", strPartyID = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtPartyName.Text != "")
                {
                    string[] strFullName = txtPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strPartyID = strFullName[0].Trim();
                }
                strParty = txtPartyName.Text.Replace(strPartyID + " ", "");

                string[] strNo = GetCourierNoAndCode();

                strQuery = " UPDATE [dbo].[CourierRegister] SET [SerialCode]='" + txtSerialCode.Text + "',[CourierNo]='" + txtCourierNo.Text + "',[CourierName]='" + txtCourierName.Text + "',[Date]='" + strDate + "',[DocType]='" + txtDocType.Text + "',[SaleBillCode]='" + txtBillCode.Text + "',[SaleBillNo]='" + txtBillNo.Text + "',"
                              + " [SalesParty]='" + strParty + "',[Station]='" + txtStation.Text + "',[Remark]='" + txtRemark.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SalePartyID]='" + strPartyID + "' Where [SCode]='" + txtCode.Text + "' and [SNo]='" + strNo[0] + "' and [SerialCode] ='" + strNo[1] + "' "
                              + "  INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                              + "('COURIEROUT','" + txtCode.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (!Convert.ToBoolean(strNo[2]))
                    {
                        DataBaseAccess.CreateDeleteQuery(strQuery);
                    }
                    SendSMSToParty();
                    MessageBox.Show("Thank You ! Record updated successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    lblMsg.Text = "";
                    DisableAllControls();
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Courier out Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string[] GetCourierNoAndCode()
        {
            string[] strNo = { "", "", "" };
            try
            {
                DataTable dt = DataBaseAccess.GetDataTableRecord("Select SNo,SerialCode,InsertStatus from CourierRegister Where Id =" + strLastID + " and SCode='" + txtCode.Text + "'  ");
                if (dt.Rows.Count > 0)
                {
                    strNo[0] = Convert.ToString(dt.Rows[0]["SNo"]);
                    strNo[1] = Convert.ToString(dt.Rows[0]["SerialCode"]);
                    strNo[2] = Convert.ToString(dt.Rows[0]["InsertStatus"]);
                }
            }
            catch
            {               
            }
            return strNo;
        }

        private void SendSMSToParty()
        {
            if (chkSendSMS.Checked && txtPartyName.Text != "")
            {
                string strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtPartyName.Text));
                if (strMobileNo != "")
                {
                    string strMessage = GetMessage();
                    if (strMessage != "")
                    {
                        SendSMS objSMS = new SendSMS();
                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
        }

        private string GetMessage()
        {
            string strMessage = "", strSubMessage = "";
            if (txtCourierNo.Text != "")
            {               
                if (txtBillNo.Text != "")
                    strSubMessage = " BillNo : " + txtBillCode.Text + " " + txtBillNo.Text;
                if (txtCourierNo.Text != "")
                    strSubMessage += " with Courier no : " + txtCourierNo.Text;
                if (txtCourierName.Text != "")
                    strSubMessage += " (" + txtCourierName.Text + ")";

                strMessage = "M/s " + txtPartyName.Text + " ! We have dispatched a courier  " + strSubMessage + ", " + txtRemark.Text + " on Date : " + txtDate.Text + " ";
            }
            return strMessage;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes && txtCode.Text != "" && txtSNo.Text != "")
                    {
                        string strQuery = " Delete from CourierRegister where SCode='" + txtCode.Text + "' and SNo='" + txtSNo.Text + "' and SerialCode='" + txtSerialCode.Text + "'  ";
                        object objValue = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from CourierRegister Where  SCode='" + txtCode.Text + "' and SNo='" + txtSNo.Text + "' and SerialCode='" + txtSerialCode.Text + "'");
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            if (objValue != null)
                            {
                                if (!Convert.ToBoolean(objValue))
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                }
                            }
                            MessageBox.Show("Thank You ! Record deleted successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            BindNextRecord();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.KeyHandlerPoint(sender, e, 0);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtSerialCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
        }

        private void txtCourierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("COURIERNAME", "SEARCH COURIER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCourierName.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtDocType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("DOCUMENTTYPE", "SEARCH DOC TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtDocType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtBillNo.Text == "")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("ALLPARTY");
                        if (strData != "")
                            txtPartyName.Text = strData;
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            txtPartyName.Text = objSearch.strSelectedData;
                        }
                    }
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtStation.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtBillCode.Text != "")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strQuery = " Where SaleBillCode='"+txtBillCode.Text+"' and ID!=0) ";
                        if (btnEdit.Text == "&Update")
                            strQuery = " Where SaleBillCode='" + txtBillCode.Text + "' and ID!= " + strLastID + ") ";
                        strQuery += " and BillCode='" + txtBillCode.Text + "' ";

                        SearchData objSearch = new SearchData("SALEBILLNOWTCOURIER",strQuery, "SEARCH SALE BILL NO", e.KeyCode);
                        objSearch.ShowDialog();
                        GetPartyName(objSearch.strSelectedData);
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetPartyName(string strFullName)
        {
            if (strFullName != "")
            {
                string[] strName = strFullName.Split('@');
                if (strName.Length > 1)
                {
                    txtBillNo.Text = strName[0];
                    txtPartyName.Text = strName[1];
                    txtStation.Text = strName[2];
                }
            }
            else
                txtBillNo.Text = txtPartyName.Text = txtStation.Text = "";
        }

        private void CourierBookOut_Load(object sender, EventArgs e)
        {
            try
            {
                if (bNewStatus)
                {
                    btnAdd.PerformClick();
                    txtSerialCode.Focus();
                }
                EditOption();
            }
            catch
            {
            }
        }


        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bCourierAdd || MainPage.mymainObject.bCourierEdit || MainPage.mymainObject.bCourierView)
                {
                    if (!MainPage.mymainObject.bCourierAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bCourierEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    if (!MainPage.mymainObject.bCourierView)
                        txtCourierNo.Focus();
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.Close();
                }

            }
            catch
            {
            }
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBillCode.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtCode.Text != "" && txtSNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("COURIEROUT", txtCode.Text, txtSNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("COURIERCODE", "SEARCH COURIER CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtCode.Text = objSearch.strSelectedData;

                        BindLastRecord();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtSNo.Text != "")
                {
                   if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControlWithSNo(txtSNo.Text);
                    }
                }
                else
                {
                    txtSNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtSerialCode_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtSNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControlWithSNo(txtSNo.Text);
                    }
                }                
            }
            catch
            {
            }
        }
    }
}
