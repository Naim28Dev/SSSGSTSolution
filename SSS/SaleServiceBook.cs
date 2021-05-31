using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class SaleServiceBook : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strOldPartyName = "", strDeletedSID = "";
        double dOldNetAmt = 0;
        public SaleServiceBook()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public SaleServiceBook(string strCode, string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);
        }

        private void SaleServiceBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlDeletionConfirmation.Visible)
                        pnlDeletionConfirmation.Visible = false;
                    else if (pnlTax.Visible)
                        pnlTax.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused && !txtOtherAmt.Focused)
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
                            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                            {
                                BindRecordWithControl(txtBillNo.Text);
                            }
                        }
                        else if (e.Control && e.Shift && e.KeyCode == Keys.D)
                        {
                            if (btnAdd.Enabled && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                            {
                                btnAdd.Text = "&Save";                               
                                SetSerialNo();
                                EnableAllControls();                              
                                txtDate.Focus();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = " Select SaleServiceCode,(Select ISNULL(MAX(BillNo),0) from SaleServiceBook Where BillCode=SaleServiceCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["SaleServiceCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                if (strSerialNo != "")
                {
                    DisableAllControls();
                    txtReason.Clear();
                    pnlDeletionConfirmation.Visible = false;

                    string strQuery = "  Select *,Convert(varchar,Date,103)BDate,dbo.GetFullName(SalePartyID) SalesParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') SubParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,Date))) LockType  from SaleServiceBook SR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                             + " Select * from SaleServiceDetails Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by ID "
                                             + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALESERVICE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            pnlTax.Visible = true;
                            BindDataWithControlUsingDataTable(_dt);
                            BindSaleServiceDetails(ds.Tables[1]);
                            BindGSTDetailsWithControl(ds.Tables[2]);
                        }
                    }
                    EditOption();
                }
            }
            catch
            {
            }
        }

        private void BindDataWithControlUsingDataTable(DataTable dt)
        {
            DisableAllControls();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                txtBillCode.Text = Convert.ToString(row["BillCode"]);
                txtBillNo.Text = Convert.ToString(row["BillNo"]);
                txtDate.Text = Convert.ToString(row["BDate"]);
                strOldPartyName = txtSalesParty.Text = Convert.ToString(row["SalesParty"]);
                txtSubParty.Text = Convert.ToString(row["SubParty"]);              
                txtSaleType.Text = Convert.ToString(row["SaleType"]);
                txtRemarks.Text = Convert.ToString(row["Remark"]);
                txtSign.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);
                txtOtherText.Text = Convert.ToString(row["OtherText"]);             
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);
                if (dt.Columns.Contains("IRNNO"))
                    txtIRNo.Text = Convert.ToString(row["IRNNO"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                txtRoundOffSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOffAmt.Text = Convert.ToString(row["RoundOffAmt"]);

                if (txtRoundOffSign.Text == "")
                    txtRoundOffSign.Text = "+";
                if (txtRoundOffAmt.Text == "")
                    txtRoundOffAmt.Text = "0.00";

                dOldNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                              
                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
               
                
                txtBillNo.ReadOnly = false;
            }
        }

        private void BindSaleServiceDetails(DataTable _dtDetails)
        {
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dgrdDetails.Rows[_index].Cells["ID"].Value = row["ID"];
                    dgrdDetails.Rows[_index].Cells["sno"].Value = (_index+1);
                    dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[_index].Cells["againstSaleBIll"].Value = row["SAC"];
                    dgrdDetails.Rows[_index].Cells["Amount"].Value = row["Amount"];                 

                    _index++;
                }
            }
        }

        private void BindGSTDetailsWithControl(DataTable dt)
        {
            int rowIndex = 0;
            dgrdTax.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdTax.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdTax.Rows[rowIndex].Cells["taxName"].Value = row["AccountName"];
                    dgrdTax.Rows[rowIndex].Cells["taxRate"].Value = row["TaxRate"];
                    dgrdTax.Rows[rowIndex].Cells["taxAmt"].Value = row["TaxAmount"];
                    dgrdTax.Rows[rowIndex].Cells["taxType"].Value = row["TaxType"];

                    rowIndex++;
                }
            }
        }


        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView)
                {
                    if (!MainPage.mymainObject.bSaleAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    if (!MainPage.mymainObject.bSaleView)
                        txtBillNo.Focus();
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

        private void EnableAllControls()
        {
            txtIRNo.ReadOnly = txtDate.ReadOnly =  txtRemarks.ReadOnly = txtSign.ReadOnly = txtOtherAmt.ReadOnly = txtOtherText.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtIRNo.ReadOnly= txtDate.ReadOnly = txtRemarks.ReadOnly = txtSign.ReadOnly = txtOtherAmt.ReadOnly = txtOtherText.ReadOnly = true;
        }

        private void ClearAllText()
        {
            strDeletedSID = strOldPartyName = txtSalesParty.Text = txtSaleType.Text = txtSubParty.Text = txtRemarks.Text = txtOtherText.Text = lblMsg.Text = lblCreatedBy.Text = "";
            txtIRNo.Text= txtTaxPer.Text = txtTaxAmt.Text = txtOtherAmt.Text = txtRoundOffAmt.Text = lblTaxableAmt.Text =  "0.00";
            lblGrossAmt.Text = lblNetAmt.Text = "0.00";
            txtSign.Text = "-";
            txtRoundOffSign.Text = "+";
            dOldNetAmt = 0;
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["sno"].Value = 1;
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;

            if (DateTime.Today > MainPage.startFinDate && DateTime.Today <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo from [SaleServiceBook] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SNo"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Set Bill No in Sale Service", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }          
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }          
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors can't be blank !!", "Sundry Debtors required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (txtSubParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sub party can't be blank !!", "Sub party required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }

            if (txtSaleType.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Sorry ! Sale Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSaleType.Focus();
                return false;
            }

            double dAmt = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(row.Cells["itemName"].Value);
                dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                if (strItem == "" && dAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter order no", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }                   
                    if (dAmt == 0)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Amount party", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["gAmount"];
                        dgrdDetails.Focus();
                        return false;
                    }

                }
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add();
                MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                dgrdDetails.Focus();
                return false;
            }
            return ValidateOtherValidation(false);
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSaleType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtSalesParty.Text || dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || _bUpdateStatus)
                    {
                        if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                        {
                            bool iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);

                            if (!iStatus && MainPage.strOnlineDataBaseName != "")
                            {
                                bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtBillCode.Text + " " + txtBillNo.Text);
                                if (!netStatus)
                                {
                                    MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                            else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
                            {
                                MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                           
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
                if (!_bUpdateStatus)
                {
                    string strRegion = Convert.ToString(dt.Rows[0]["Region"]), strCStateName = Convert.ToString(dt.Rows[0]["CStateName"]).ToUpper(), strSStateName = Convert.ToString(dt.Rows[0]["SStateName"]).ToUpper();
                    if (strRegion != "")
                    {
                        if (strRegion == "LOCAL" && strSStateName != strCStateName)
                        {
                            MessageBox.Show("You are entering a central transaction for a party belonging to same state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //if (result == DialogResult.Yes)
                            //    return true;
                            //else
                            return false;
                        }
                        else if (strRegion == "INTERSTATE" && strSStateName == strCStateName)
                        {
                            MessageBox.Show("You are entering a local  transaction for a party belonging to other  state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //if (result == DialogResult.Yes)
                            //    return true;
                            //else
                            return false;
                        }
                    }
                }
                //if (Convert.ToString(dt.Rows[0]["IncludeStatus"]) == "DENY")
                //{
                //    MessageBox.Show("Sorry Sale type and purchase type doesn't match in tax inclusion!\nPlease enter correct purchase type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool CheckBillNoAndSuggest()
        {
            bool chkStatus = true;
            try
            {
                if (txtBillNo.Text != "")
                {
                    int check = dba.CheckSaleServiceAvailability(txtBillCode.Text, txtBillNo.Text);
                    if (check > 0)
                    {
                        string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(BillNo)+1 from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' "));
                        MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  Bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bill No can't be blank  ..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
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
                    }
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();

                    txtDate.Focus();
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else if (ValidateControls() && CheckBillNoAndSuggest())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");              

                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "",  strTaxAccountID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }

                double dAmt = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "";

                strQuery += " if not exists (Select BillCode from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[SaleServiceBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SaleType],[TransportName],[StationName],[Remark],[OtherText],[OtherSign],[OtherAmt],[TaxPer],[TaxAmt],[GrossAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[IRNNO]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','"+txtSaleType.Text+"','','','" + txtRemarks.Text + "','" + txtOtherText.Text + "','" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + "," +
                               +  dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "','',1,0" + ",'" + txtRoundOffSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",'"+txtIRNo.Text+"')  "
                               + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strSaleParty + "','SALE SERVICE','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNetAmt.Text + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "') ";
                
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);

                    strQuery += " INSERT INTO [dbo].[SaleServiceDetails] ([BillCode],[BillNo],[ItemName],[SAC],[Amount],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                             + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["itemName"].Value + "','"+rows.Cells["againstSaleBIll"].Value+"'," + dAmt + ",0,'" + MainPage.strLoginName + "','',1,0) ";
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtSaleType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSaleType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                //GST Details

                foreach (DataGridViewRow rows in dgrdTax.Rows)
                {
                    strTaxAccountID = "";
                    strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strTaxAccountID = strFullName[0].Trim();
                    }

                    strQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                             + " ('SALESERVICE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                          + "('SALESERVICE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                strQuery += "  end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    //SendSMSToParty(strMobileNo);
                    if (chkEmail.Checked)
                        NotificationClass.SetNotification("SALESERVICE", strSalePartyID, dAmt, txtBillCode.Text + " " + txtBillNo.Text);

                    AskForPrint(strPath);

                    btnAdd.Text = "&Add";
                    ClearAllText();
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    bool Created = false;
                    string strPath = SetSignatureInBill(false, false, true, ref Created), strEmailID = "", strWhatsAppNo = "";
                    if (Created)
                    {
                        strFilePath = strPath;
                        string[] strParty = txtSalesParty.Text.Split(' ');
                        if (strParty.Length > 1)
                        {
                            string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "'   ";
                            DataTable _dt = dba.GetDataTable(strQuery);
                            if (_dt.Rows.Count > 0)
                            {
                                strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                                strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                                strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                                if (strEmailID != "")
                                {
                                    CreateEmailBody(strEmailID, strPath, 0);
                                }
                                else if (_bStatus)
                                    MessageBox.Show("Sorry ! Please enter mail id in party master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                if (strWhatsAppNo != "")
                                {
                                    SendWhatsappMessage(strWhatsAppNo, strPath);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void SendWhatsappMessage(string strMobileNo, string strPath)
        {
            if (MainPage.strCompanyName.Contains("SARAOGI SUPER SALE") && MainPage.strSoftwareType == "AGENT")
            {
                string strMsgType = "", _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text, strWhastappMessage = "";
                string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
                string strMType = "";
                if (btnEdit.Text == "&Update")
                {
                    dba.DeleteSaleBillFile(strPath, strBranchCode);
                    strMsgType = "sale_service_update";
                    strMType = "invoice_update";
                }
                else
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                        dba.DeleteSaleBillFile(strPath, strBranchCode);

                    strMsgType = "sale_service";
                    strMType = "invoice_generation";
                }


                bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
                if (!_bStatus)
                {
                    DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                    if (_updateResult == DialogResult.Retry)
                        _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
                }


                if (_bStatus)
                {

                    strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + lblNetAmt.Text + "\",";
                    if (strMobileNo != "")
                    {
                        string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                        if (strResult != "")
                            MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    strWhastappMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + lblNetAmt.Text + "\"}";
                    WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strWhastappMessage, "", "");
                }
            }
        }

        private void CreateEmailBody(string strEmail, string strpath, int billStatus)
        {
            try
            {              

                string strMessage = "", strSub = "";
                if (billStatus == 0)
                {
                    if (btnAdd.Text == "&Save" || (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit"))
                    {
                        strMessage = "M/S : " + txtSalesParty.Text + " , we have created your sale service bill <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "M/S : " + txtSalesParty.Text + ", we have updated your sale service bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
                    }

                    if (btnAdd.Text == "&Save")
                        strSub = "Sale Service bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " created.";
                    else
                        strSub = "Alert ! Sale Service bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " updated.";
                }
                else
                {
                    strMessage = " Alert ! Sale Service bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + "</b> is Deleted by : " + MainPage.strLoginName + "  and  the deleted Sale Service bill is attached with this mail. ";
                    strSub = "Alert ! Sale Service bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " deleted by : " + MainPage.strLoginName;
                }

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "SALE BILL",true);
                if (billStatus == 0 && bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        BindLastRecord();
                        btnAdd.Text = "&Add";
                    }
                    if (btnEdit.Enabled)
                    {
                        btnEdit.Text = "&Update";
                        EnableAllControls();
                        txtBillNo.ReadOnly = true;
                        strDeletedSID = "";
                        txtDate.Focus();
                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;                   
                    if (ValidateControls())
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
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
                string strDate = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strTaxAccountID = "", strDeletedSIDQuery = "", strQuery = "", strID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                double dAmt = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);

                strQuery += " UPDATE [dbo].[SaleServiceBook] SET [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SaleType]='" + txtSaleType.Text + "',[Remark]='" + txtRemarks.Text + "',[OtherText]='" + txtOtherText.Text + "',[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[RoundOffSign]='" + txtRoundOffSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[IRNNO]='"+txtIRNo.Text+"' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " UPDATE [dbo].[BalanceAmount] Set [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' Where [AccountStatus]='SALE SERVICE' and [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from [dbo].[GSTDetails] Where [BillType]='SALESERVICE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    strID = Convert.ToString(rows.Cells["ID"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SaleServiceDetails] ([BillCode],[BillNo],[ItemName],[SAC],[Amount],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["itemName"].Value + "','" + rows.Cells["againstSaleBIll"].Value + "'," + dAmt + ",0,'" + MainPage.strLoginName + "','',1,0) ";
                    }
                    else
                    {
                        strQuery += " UPDATE [dbo].[SaleServiceDetails] SET [ItemName]='" + rows.Cells["itemName"].Value + "',[SAC]='" + rows.Cells["againstSaleBIll"].Value + "',[Amount]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + "  ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtSaleType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSaleType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                //GST Details

                foreach (DataGridViewRow rows in dgrdTax.Rows)
                {
                    strTaxAccountID = "";
                    strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strTaxAccountID = strFullName[0].Trim();
                    }

                    strQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                             + " ('SALESERVICE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }
                object objValue = "";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[SaleServiceDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and ID in (" + strDeletedSID + ") ";
                    strDeletedSIDQuery = " Delete from [dbo].[SaleServiceDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";

                    objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALESERVICE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";



                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (Convert.ToString(objValue) != "" && strDeletedSIDQuery != "")
                    {
                        if (!Convert.ToBoolean(objValue))
                        {
                            DataBaseAccess.CreateDeleteQuery(strDeletedSIDQuery);
                        }
                    }
                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    //SendSMSToParty(strMobileNo);

                   // AskForPrint(strPath);

                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);                   
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    BindRecordWithControl(txtBillNo.Text);
                   
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Updating Record in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
            {
                pnlDeletionConfirmation.Visible = true;
                txtReason.Clear();
                txtReason.Focus();
            }
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && dba.ValidateBackDateEntry(txtDate.Text))
                {
                    if (txtReason.Text != "" && ValidateOtherValidation(true))
                    {
                        btnFinalDelete.Enabled = false;
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                           
                            string strQuery = "";

                            strQuery += " Delete from [SaleServiceBook] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                     + " Delete from [SaleServiceDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('SALE SERVICE','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='SALESERVICE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('SALESERVICE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from SaleServiceBook Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                txtReason.Clear();
                                pnlDeletionConfirmation.Visible = false;
                                MessageBox.Show("Thank You ! Record deleted successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindNextRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReason.Focus();
                    }
                }
            }
            catch
            {
            }
            btnFinalDelete.Enabled = true;


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBillNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
                else
                {
                    txtBillNo.Focus();
                }
            }
            catch
            {
            }
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    {
                        if (e.KeyCode == Keys.F1)
                        {
                            ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                            string strData = objRead.ReadDataFromCard("SALESPARTY");
                            if (strData != "")
                            {
                                txtSalesParty.Text = strData;
                                txtSubParty.Text = "SELF";
                                if (btnAdd.Text == "&Save")
                                {
                                    dgrdDetails.Rows.Clear();
                                    dgrdDetails.Rows.Add();
                                    dgrdDetails.Rows[0].Cells["sno"].Value = 1;
                                }
                            }
                        }
                        else
                        {
                            char objChar = Convert.ToChar(e.KeyCode);
                            int value = e.KeyValue;
                            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                            {
                                if (btnEdit.Text == "&Update" && GetAgainstBillNo() != "")
                                {
                                    MessageBox.Show("Sorry ! Please remove below against bill .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                                    objSearch.ShowDialog();
                                    string strData = objSearch.strSelectedData;
                                    if (strData != "")
                                    {                                       

                                        bool _blackListed = false;
                                        if (dba.CheckTransactionLockWithBlackList(strData, ref _blackListed))
                                        {
                                            MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            txtSalesParty.Text = "";
                                            txtSubParty.Text = "";
                                        }
                                        else if (_blackListed)
                                        {
                                            MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            txtSalesParty.Text = "";
                                            txtSubParty.Text = "";
                                        }
                                        else
                                        {
                                            txtSalesParty.Text = strData;
                                            txtSubParty.Text = "SELF";
                                            if (btnAdd.Text == "&Save")
                                            {
                                                dgrdDetails.Rows.Clear();
                                                dgrdDetails.Rows.Add();
                                                dgrdDetails.Rows[0].Cells["sno"].Value = 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private string GetAgainstBillNo()
        {
            string strSerialNo = "";
            try
            {
                if (btnEdit.Text == "&Update")
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSerialNo = Convert.ToString(row.Cells["againstSaleBIll"].Value);
                        if (strSerialNo != "")
                            break;
                    }
                }
            }
            catch { }
            return strSerialNo;
        }

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtSalesParty.Text != "")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtSubParty.Text = objSearch.strSelectedData;
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSaleType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESTYPE", "SEARCH SALES TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSaleType.Text = objSearch.strSelectedData;
                        CalculateNetAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //    {
            //        char objChar = Convert.ToChar(e.KeyCode);
            //        int value = e.KeyValue;
            //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            //        {
            //            SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
            //            objSearch.ShowDialog();
            //            txtTransport.Text = objSearch.strSelectedData;
            //        }
            //    }
            //    e.Handled = true;
            //}
            //catch
            //{
            //}
        }

        private void txtStation_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //    {
            //        char objChar = Convert.ToChar(e.KeyCode);
            //        int value = e.KeyValue;
            //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            //        {
            //            SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION NAME", e.KeyCode);
            //            objSearch.ShowDialog();
            //            txtStation.Text = objSearch.strSelectedData;
            //        }
            //    }
            //    e.Handled = true;
            //}
            //catch
            //{
            //}
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtOtherText_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtOtherAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                e.Handled = true;
            else
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex < 2)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            CalculateNetAmount();
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3)
                    {
                        //if (txtSalesParty.Text != "")
                        //{
                        //    SearchData objSearch = new SearchData("SALEBILLNOFORSERVICE",txtSalesParty.Text, "SEARCH SALE BILL NO", Keys.Space);
                        //    objSearch.ShowDialog();

                        //    dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        //    if (objSearch.strSelectedData != "")
                        //        e.Cancel = true;
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Sorry ! Please enter Sundry Debtors name !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    e.Cancel = true;
                        //}
                    }
                }
                else
                    e.Cancel = true;
            }
            catch { }
        }

        private void CalculateTotalAmt()
        {
            double dAmt = 0;
            int _rowIndex = 1;

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                rows.Cells["sno"].Value = _rowIndex + ".";            
                 dAmt += dba.ConvertObjectToDouble(rows.Cells["amount"].Value);             
                _rowIndex++;
            } 
           
            lblGrossAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateNetAmount();
        }

        private void CalculateNetAmount()
        {
            double dOtherAmt = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dRoundOffAmt = 0, dTaxableAmt= 0 , dTaxAmt = 0, dFinalAmt = 0;
            try
            {
                dOtherAmt = dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);

                dTaxAmt = GetTaxAmount(dFinalAmt, dOtherAmt,ref dTaxableAmt);
                dNetAmt = dGrossAmt + dOtherAmt + dTaxAmt;
                lblNetAmt.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));// Math.Round(dNetAmt, 0);
                dRoundOffAmt = (dNNetAmt - dNetAmt);

                if (dRoundOffAmt >= 0)
                {
                    txtRoundOffSign.Text = "+";
                    txtRoundOffAmt.Text = dRoundOffAmt.ToString("0.00");
                }
                else
                {
                    txtRoundOffSign.Text = "-";
                    txtRoundOffAmt.Text = Math.Abs(dRoundOffAmt).ToString("0.00");
                }

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

            }
            catch// (Exception ex)
            {
                //  string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                // dba.CreateErrorReports(strReport);
            }
        }

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0, dServiceAmt = 0;
            string _strTaxType = "";
            try
            {
                if (MainPage._bTaxStatus && txtSaleType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtSaleType.Text, "SALES");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        string strTaxationType = Convert.ToString(row["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                                _strTaxType = "INCLUDED";

                            dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);

                            string strQuery = "", strServiceQuery = "", strItemName = "";
                            double dDisStatus = 0;

                            double dAmt = 0, dQty = 1;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value); 
                                if (dAmt > 0)
                                {
                                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);

                                    if (strQuery != "")
                                    {
                                        strQuery += " UNION ALL ";
                                        strServiceQuery += " UNION ALL ";
                                    }

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 + " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + " * 100) / (100 + TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/ " + dQty + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + "* 100) / (100 + TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dAmt + ">0  ";
                                    strServiceQuery += " Select (SUM(CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+GM.TaxRate)) else " + dAmt + " end)  *(100 + " + dDisStatus + ")/ 100.00)  as Amount,'" + strItemName + "' as ItemName," + dQty + " Quantity from Items _IM Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='') as GM  Where _IM.ItemName='" + strItemName + "' ";
                                }
                            }

                            if (strQuery != "")
                            {                                
                                if (dOtherAmt != 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount,0 as TaxRate ";
                                }
                                if (strQuery != "")
                                {
                                    strQuery = " Select TaxableAmt,(CASE WHEN TaxRate=MTaxRate then (Amt+ServiceTax) else Amt end) Amt,TaxRate,MTaxRate,0 as ServiceAmt from ( "
                                                   + " Select *,0 ServiceTax from ( "
                                                   + " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate,MAX(TaxRate) OVER(PARTITION BY ID)  MTaxRate from ( Select 1 as ID,HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                                   + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate,ID )_Goods "                                               
                                                   + "  )_FinalSales ";

                                    DataTable dt = dba.GetDataTable(strQuery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        double dMaxRate = 0, dTTaxAmt = 0;
                                        // BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                        dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                        dTaxAmt = dTTaxAmt;// dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]);
                                        dTaxPer = dMaxRate;// dba.ConvertObjectToDouble(dt.Rows[0]["TaxRate"]);
                                      //  dServiceAmt = dba.ConvertObjectToDouble(dt.Rows[0]["ServiceAmt"]);
                                        //if (rdoExcludeDisc.Checked)
                                        //    dServiceAmt = 0;
                                       // pnlTax.Visible = true;
                                    }
                                }
                            }
                        }
                        else if (strTaxationType == "VOUCHERWISE")
                        {
                            double _dTaxPer = dba.ConvertObjectToDouble(row["TaxRate"]);
                            if (_dTaxPer > 0)
                            {
                                dTaxAmt = (dFinalAmt * _dTaxPer) / 100;
                            }
                            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                            txtTaxPer.Text = _dTaxPer.ToString("0.00");                          
                          //  pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text =  "0.00";
                    }
                }

                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bSaleAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bSaleEdit)
                    btnEdit.Enabled = false;

            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Sale Return Book", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEdit.Enabled = btnAdd.Enabled = false;
            }

            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);           

            if (_strTaxType == "INCLUDED")
                dTaxAmt = 0;
            return dTaxAmt;
        }

        //private void BindTaxDetails(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt,ref double dTaxableAmt)
        //{
        //    try
        //    {
        //        dgrdTax.Rows.Clear();
        //        if (_dt.Rows.Count > 0)
        //        {
        //            dgrdTax.Rows.Add(_dt.Rows.Count);
        //            int _index = 0;
        //            string strRegion = Convert.ToString(_row["Region"]), strIGST = Convert.ToString(_row["IGSTName"]), strSGST = Convert.ToString(_row["SGSTName"]); ;
        //            if (strRegion == "LOCAL")
        //                dgrdTax.Rows.Add(_dt.Rows.Count);
        //            double dTaxRate = 0, dTaxAmt = 0;

        //            foreach (DataRow row in _dt.Rows)
        //            {
        //                dTaxRate = dba.ConvertObjectToDouble(row["TaxRate"]);
        //                dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);                     
        //                dTTaxAmt += Convert.ToDouble(dTaxAmt.ToString("0.00"));

        //                if (dTaxRate > dMaxRate)
        //                    dMaxRate = dTaxRate;
        //                dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
        //                dgrdTax.Rows[_index].Cells["taxName"].Value = strIGST;
        //                dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;

        //                if (strRegion == "LOCAL")
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N4", MainPage.indianCurancy);
        //                    _index++;
        //                    dgrdTax.Rows[_index].Cells["taxName"].Value = strSGST;
        //                    dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N4", MainPage.indianCurancy);
        //                }
        //                else
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = dTaxRate.ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = dTaxAmt.ToString("N4", MainPage.indianCurancy);
        //                }

        //                _index++;
        //            }
        //        }
        //    }
        //    catch { }
        //}

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 2)
                        CalculateNetAmount();
                    else if (e.ColumnIndex == 4)
                        CalculateTotalAmt();
                }                    
            }
            catch
            { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentCell.ColumnIndex == 3)
                {
                    TextBox txt = (TextBox)e.Control;
                    txt.CharacterCasing = CharacterCasing.Upper;
                    txt.KeyPress += new KeyPressEventHandler(txtDescription_KeyPress);
                }
                else if (dgrdDetails.CurrentCell.ColumnIndex == 4)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch { }
        }

        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 3)
            {
                dba.ValidateSpace(sender, e);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 4)
            {
                dba.KeyHandlerPoint(sender, e, 2);
            }
        }

        private void txtOtherAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtSign.Text == "")
                        txtSign.Text = "+";
                    if (txtOtherAmt.Text == "")
                        txtOtherAmt.Text = "0.00";
                    CalculateNetAmount();
                }
            }
            catch { }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALESERVICE", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    GSTPrintAndPreview(false, "", false);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Service Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }


        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC)
        {
            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSaleServiceBookDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt);
            if (dt.Rows.Count > 0)
            {
                if (MainPage.strSoftwareType != "AGENT")
                    _bDSC = false;
                if (!_bIGST)
                {
                    if (_bDSC && strPath != "" && MainPage.strCompanyName.Contains("SARAOGI"))
                    {

                        Reporting.SaleServiceReport_CGST_DSC objOL_salebill = new Reporting.SaleServiceReport_CGST_DSC();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                            return true;
                        }
                    }
                    else
                    {
                        Reporting.SaleServiceReport_CGST objOL_salebill = new Reporting.SaleServiceReport_CGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                }
                else
                {
                    if (_bDSC && strPath != "" && MainPage.strCompanyName.Contains("SARAOGI SUPER"))
                    {
                        Reporting.SaleServiceReport_IGST_DSC objOL_salebill = new Reporting.SaleServiceReport_IGST_DSC();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                            return true;
                        }
                    }
                    else
                    {
                        Reporting.SaleServiceReport_IGST objOL_salebill = new Reporting.SaleServiceReport_IGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                }
            }
            return false;
        }

        private void FinallyPrint(bool _pstatus, CrystalDecisions.CrystalReports.Engine.ReportClass Report, string strPath)
        {
            Report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            if (strPath != "")
            {
                Report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
            }
            else
            {
                if (_pstatus)
                {
                    System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                    defS.Collate = false;
                    defS.FromPage = 0;
                    defS.ToPage = 0;
                    defS.Copies = (short)MainPage.iNCopySServ;

                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(Report, false, MainPage.iNCopySServ);
                    else
                    {
                        //string strValue = "0";
                        //strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT ! ", "Number of Copies", "2", 400, 300);
                        //if (strValue != "" && strValue != "0")
                        //{
                        //    int nCopy = Int32.Parse(strValue);
                        Report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        //}
                    }
                }
                else
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES SERVICE BOOK REPORT PREVIEW");
                    objReport.myPreview.ReportSource = Report;
                    objReport.myPreview.ShowExportButton = false;
                    objReport.myPreview.ShowPrintButton = false;
                    objReport.ShowDialog();
                }
            }
            Report.Close();
            Report.Dispose();
        }

        private void AskForPrint(string strPath)
        {
            try
            {

                DialogResult _result = MessageBox.Show("Are you want to print Sale Service Bill ?", "Print Sale Service Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_result == DialogResult.Yes)
                {
                    bool Created = false;
                    if (strPath != "")
                        System.Diagnostics.Process.Start(strPath);
                    else
                        SetSignatureInBill(true, false, true, ref Created);
                }
            }
            catch
            {
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    bool Created = false;
                    SetSignatureInBill(true, false, true, ref Created);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }



        private string SetSignatureInBill(bool _bPStatus, bool _createPDF, bool _dscVerified, ref bool Created)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
            {
                if (!_bPStatus)
                {
                    string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Service";
                    strFileName = strNewPath + "\\" + txtBillNo.Text + ".pdf";
                    if (File.Exists(strFileName))
                        File.Delete(strFileName);
                    Directory.CreateDirectory(strNewPath);

                    if (_createPDF)
                    {
                        SaveFileDialog _browser = new SaveFileDialog();
                        _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                        _browser.FileName = txtBillNo.Text + ".pdf";
                        if (_browser.ShowDialog() == DialogResult.OK)
                        {
                            if (_browser.FileName != "")
                                strPath = _browser.FileName;
                        }
                    }
                    else
                    {
                        string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SalesService\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        _strPath += "\\" + _strFileName;

                        strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                        if (File.Exists(strPath))
                            File.Delete(strPath);
                        Directory.CreateDirectory(_strPath);
                        Created = true;
                    }
                }
                if (strPath != "")
                {
                    if (!MainPage.strCompanyName.Contains("SARAOGI"))
                        strFileName = strPath;

                    bool _bstatus = GSTPrintAndPreview(false, strFileName, _dscVerified);
                    if (_bstatus)
                    {
                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";
                            PDFSigner _objSigner = new PDFSigner();
                            bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                            if (!_bFileStatus)
                                strPath = "";
                            if (_bPStatus && _bFileStatus)
                            {
                                System.Diagnostics.Process.Start(strPath);
                                Created = true;
                            }
                        }
                    }
                }
                else
                {
                    GSTPrintAndPreview(true, "", false);
                }
            }
            catch (Exception ex)
            {
                strPath = "";
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return strPath;
        }


        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESERVICECODE", "SEARCH SALE SERVICE CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtBillCode.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SaleServiceBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            }
            catch { }
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        bool __bCreated = false;
                        string strPath = SetSignatureInBill(false, true,true, ref __bCreated);
                        if (strPath != "")
                            MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private void btnEInvoice_Click(object sender, EventArgs e)
        {
            btnEInvoice.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want E-Invoice JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            object obj = DataBaseAccess.ExecuteMyScalar("Select GSTNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtSalesParty.Text + "' and GSTNo!=''");
                            if (Convert.ToString(obj) != "")
                            {
                                string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";
                                var _success = dba.GenerateEInvoiceJSON_SaleBook(false,strBillNo, "SERVICE");
                                if (_success)
                                {
                                    DialogResult _result = MessageBox.Show("Are you want to open e-invoice site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                        System.Diagnostics.Process.Start("https://einvoice1.gst.gov.in/Invoice/BulkUpload");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! E-Invoice is allowed only for B2B customer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch { }
            btnEInvoice.Enabled = true;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdDetails.CurrentCell.RowIndex;
                    IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                    if (Index < dgrdDetails.RowCount - 1)
                        CurrentRow = Index - 1;
                    else
                        CurrentRow = Index;

                    if (IndexColmn < dgrdDetails.ColumnCount - 1)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdDetails.RowCount - 1)
                    {
                        if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value) != "" && Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value) != "")
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[CurrentRow + 1].Cells["sno"].Value = (CurrentRow + 2) + ".";                           
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow + 1].Cells["itemName"];
                        }
                        else
                        {
                            if (btnAdd.Text == "&Save")
                                btnAdd.Focus();
                            else
                                btnEdit.Focus();
                        }
                    }                   
                }
                else if (e.KeyCode == Keys.F1)
                {
                    Index = dgrdDetails.CurrentCell.RowIndex;
                    if (btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(Index);
                        CalculateTotalAmt();
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["ID"].Value);
                        if (strID != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (strDeletedSID != "")
                                    strDeletedSID += ",";
                                strDeletedSID += strID;

                                dgrdDetails.Rows.RemoveAt(Index);
                                CalculateTotalAmt();
                            }
                        }
                        else
                        {
                            dgrdDetails.Rows.RemoveAt(Index);
                            CalculateTotalAmt();
                        }
                    }
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["sno"].Value = dgrdDetails.Rows.Count + ".";
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                    }
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    int _rowCount = dgrdDetails.Rows.Count;
                    dgrdDetails.Rows.Add(1);

                    dgrdDetails.Rows[_rowCount].Cells["sno"].Value = (_rowCount + 1); 
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[_rowCount].Cells["itemName"];
                }
            }
            catch
            {
            }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnEdit.Text == "&Update")
                e.Handled = true;
            else
                dba.KeyHandlerPoint(sender, e, 0);
        }

        

    }
}
