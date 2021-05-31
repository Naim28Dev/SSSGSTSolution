using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class PurchaseReturn : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strDeletedSID = "", strPurchaseBillCode = "", strOldPartyName = "", strOLDYearDB = "";
        double dOldNetAmt = 0, dTotalAmount = 0;
        public bool saleStatus = false, updateStatus = false, newStatus = false;

        public PurchaseReturn()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public PurchaseReturn(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            newStatus = bStatus;
        }

        public PurchaseReturn(string strCode, string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);
        }

        public PurchaseReturn(string strCode, string strSNo, bool sStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            saleStatus = sStatus;
            BindRecordWithControl(strSNo);
            EnableAllControls();
        }

        private void GoodsReciept_KeyDown(object sender, KeyEventArgs e)
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
                }
            }
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = " Select PBillCode,PurchaseReturnCode,(Select ISNULL(MAX(BillNo),0) from PurchaseReturn Where BillCode=PurchaseReturnCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtPBillCode.Text = strPurchaseBillCode = Convert.ToString(dt.Rows[0]["PBillCode"]);
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["PurchaseReturnCode"]);
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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

                    string strQuery = "  Select *,Convert(varchar,Date,103)BDate,Convert(varchar,PurchaseBillDate,103)PDate,dbo.GetFullName(PurchasePartyID) PurchaseParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,PR.Date))) LockType from PurchaseReturn PR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                    + " Select *,dbo.GetFullName(SalePartyID) SalesParty from PurchaseReturnDetails Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID "
                                    + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            pnlTax.Visible = true;
                            BindDataWithControlUsingDataTable(_dt);
                            BindSaleReturnDetails(_dt, ds.Tables[1]);
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
                txtPBillCode.Text = Convert.ToString(row["PurchaseBillCode"]);
                txtPBillNo.Text = Convert.ToString(row["PurchaseBillNo"]);
                txtPDate.Text = Convert.ToString(row["PDate"]);
                strOldPartyName = txtPurchaseParty.Text = Convert.ToString(row["PurchaseParty"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtPurchaseType.Text = Convert.ToString(row["PurchaseType"]);
                txtPurchaseInvoiceNo.Text = Convert.ToString(row["ReverseCharge"]);
                txtSignAmt.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);
                txtDiscountAmt.Text = Convert.ToString(row["NetDiscount"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                txtTaxFree.Text = Convert.ToString(row["TaxFree"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                txtRoundOffSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOffAmt.Text = Convert.ToString(row["RoundOffAmt"]);

                if (txtRoundOffSign.Text == "")
                    txtRoundOffSign.Text = "+";
                if (txtRoundOffAmt.Text == "")
                    txtRoundOffAmt.Text = "0.00";

                lblQty.Text = Convert.ToString(row["TotalQty"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dba.ConvertObjectToDouble(row["NetAmt"]).ToString("N2", MainPage.indianCurancy);
                // txtRemark.Text = Convert.ToString(row["ReverseCharge"]);


                if (Convert.ToString(row["EntryType"]) == "BYSALERETURN")
                    rdoBySaleReturnSNo.Checked = true;
                else if (Convert.ToString(row["EntryType"]) == "MANUAL")
                    rdoManual.Checked = true;
                else if (Convert.ToString(row["EntryType"]) == "BYPURCHASE")
                    rdoPurchase.Checked = true;
                else
                    rdoAll.Checked = true;

                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;


                if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                txtBillNo.ReadOnly = false;
            }
        }

        private void BindSaleReturnDetails(DataTable _dtMain, DataTable _dtDetails)
        {
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dgrdDetails.Rows[_index].Cells["sid"].Value = row["SID"];
                    dgrdDetails.Rows[_index].Cells["saleReturnNo"].Value = row["SRBillNo"];
                    dgrdDetails.Rows[_index].Cells["salesParty"].Value = row["SalesParty"];
                    dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[_index].Cells["designName"].Value = row["DesignName"];
                    dgrdDetails.Rows[_index].Cells["disStatus"].Value = row["DisStatus"];
                    dgrdDetails.Rows[_index].Cells["dis"].Value = row["Discount"];
                    dgrdDetails.Rows[_index].Cells["dhara"].Value = row["Dhara"];
                    dgrdDetails.Rows[_index].Cells["gQty"].Value = row["Qty"];
                    dgrdDetails.Rows[_index].Cells["gAmount"].Value = row["Amount"];
                    dgrdDetails.Rows[_index].Cells["gPacking"].Value = row["Packing"];
                    dgrdDetails.Rows[_index].Cells["gFreight"].Value = row["Freight"];
                    dgrdDetails.Rows[_index].Cells["gTax"].Value = row["TaxFree"];
                    dgrdDetails.Rows[_index].Cells["totalAmt"].Value = row["TotalAmt"];

                    _index++;
                }
            }
            else
            {
                dgrdDetails.Rows.Add();
                DataRow row = _dtMain.Rows[0];

                dgrdDetails.Rows[0].Cells["itemName"].Value = row["Item"];
                dgrdDetails.Rows[0].Cells["gQty"].Value = row["Quantity"];
                dgrdDetails.Rows[0].Cells["gAmount"].Value = row["Amount"];
                dgrdDetails.Rows[0].Cells["gPacking"].Value = row["Packing"];
                dgrdDetails.Rows[0].Cells["gFreight"].Value = row["Freight"];
                dgrdDetails.Rows[0].Cells["gTax"].Value = row["Tax"];

            }
        }

        private void EnableAllControls()
        {
            txtTaxFree.ReadOnly = txtPDate.ReadOnly = txtDate.ReadOnly = txtPurchaseInvoiceNo.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtRemark.ReadOnly = txtTaxPer.ReadOnly = false;
            grpQtr.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtTaxFree.ReadOnly = txtPDate.ReadOnly = txtDate.ReadOnly = txtPurchaseInvoiceNo.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtRemark.ReadOnly = txtTaxPer.ReadOnly = true;
            grpQtr.Enabled = false;
        }

        private void ClearAllText()
        {
            strOldPartyName = txtPurchaseParty.Text = txtPurchaseType.Text = txtRoundOffAmt.Text = txtPurchaseInvoiceNo.Text = strDeletedSID = lblMsg.Text = lblCreatedBy.Text = txtRemark.Text = txtRemark.Text = txtPBillNo.Text = "";
            txtDiscountAmt.Text = txtTaxAmt.Text = txtOtherAmt.Text = lblTaxableAmt.Text = "0.00";
            lblQty.Text = lblGrossAmt.Text = lblNetAmt.Text = txtTaxFree.Text = "0.00";
            txtTaxPer.Text = "18.00";
            txtRoundOffSign.Text = "+";
            txtRemark.Text = "";
            txtPBillCode.Text = strPurchaseBillCode;
            txtSignAmt.Text = "-";
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;
            chkEmail.Checked = true;
            rdoCurrent.Checked = true;

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = txtPDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtPDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo  from [PurchaseReturn] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SNo"]);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtReceiptNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                        {
                            dgrdDetails.Rows.Clear();
                            txtPurchaseParty.Text = strData;
                            rdoBySaleReturnSNo.Checked = true;
                            if (dgrdDetails.Rows.Count == 0)
                                dgrdDetails.Rows.Add();
                        }
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                bool _blackListed = false;
                                if (dba.CheckTransactionLockWithBlackList(strData, ref _blackListed))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = "";
                                }
                                else if (_blackListed)
                                {
                                    MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = "";
                                }
                                else
                                {
                                    dgrdDetails.Rows.Clear();
                                    txtPurchaseParty.Text = strData;
                                    rdoBySaleReturnSNo.Checked = true;
                                    txtPBillNo.Clear();
                                    if (dgrdDetails.Rows.Count == 0)
                                        dgrdDetails.Rows.Add();
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
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! SUNDRY CREDITOR can't be blank !!", "SUNDRY CREDITOR required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }

            if (MainPage._bTaxStatus)
            {
                if (txtPurchaseType.Text == "")
                {
                    MessageBox.Show("Sorry ! Purchase Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseType.Focus();
                    return false;
                }
                if (dba.ConvertObjectToDouble(txtTaxAmt.Text) == 0)
                {
                    MessageBox.Show("Sorry ! Tax Amt can't be zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTaxPer.Focus();
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }

            double dQty = 0, dAmt = 0;
            string strPartyName = "", strItem = "", strDhara = "";

            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strPartyName = Convert.ToString(row.Cells["salesParty"].Value);
                strItem = Convert.ToString(row.Cells["itemName"].Value);
                strDhara = Convert.ToString(row.Cells["dhara"].Value);
                dQty = dba.ConvertObjectToDouble(row.Cells["gQty"].Value);
                dAmt = dba.ConvertObjectToDouble(row.Cells["totalAmt"].Value);

                if (strItem == "" && dQty == 0 && dAmt == 0)
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
                    if (strPartyName != "")
                    {
                        if (strDhara == "")
                        {
                            MessageBox.Show("Sorry ! Dhara can't be blank", "Enter order no", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell = row.Cells["dhara"];
                            dgrdDetails.Focus();
                            return false;
                        }
                        if (dQty == 0)
                        {
                            MessageBox.Show("Sorry ! Quantity can't be blank", "Enter SUNDRY CREDITOR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell = row.Cells["gQty"];
                            dgrdDetails.Focus();
                            return false;
                        }
                    }
                    if (dAmt == 0)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter SUNDRY CREDITOR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtPurchaseType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtPurchaseParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtPurchaseParty.Text || dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || _bUpdateStatus)
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
                    //object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GRBillNo),1) from MaxSerialNo");
                    //int maxBillNo = Convert.ToInt32(objMax);
                    //if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    //{
                    int check = dba.CheckPurchaseReturnAvailability(txtBillCode.Text, txtBillNo.Text);
                    if (check > 0)
                    {
                        string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(BillNo)+1 from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' "));
                        MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  Bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        chkStatus = false;
                    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("This Bill No is already in used please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    txtBillNo.Focus();
                    //    chkStatus = false;
                    //}
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

        private string GetEntryType()
        {
            if (rdoBySaleReturnSNo.Checked)
                return "BYSALERETURN";
            if (rdoPurchase.Checked)
                return "BYPURCHASE";
            else if (rdoManual.Checked)
                return "MANUAL";
            else
                return "ALL";
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
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
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
                string strDate = "", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtPDate.Text.Length == 10 && txtPBillNo.Text != "")
                {
                    DateTime pDate = dba.ConvertDateInExactFormat(txtPDate.Text);
                    strPDate = "'" + pDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strNetQuery = "", strSaleParty = "", strPurchaseParty = "", strSalePartyID = "", strPurchasePartyID = "", strTaxAccountID = "";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }

                double dAmt = 0, dQty = 0, dPacking = 0, dFreightAmt = 0, dTaxFree = 0, dTotalAmt = 0, dDis = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "";

                strQuery += " if not exists (Select BillCode from [PurchaseReturn] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[PurchaseReturn] ([BillCode],[BillNo],[Date],[PurchasePartyID],[EntryType],[PurchaseType],[Remark],[OtherSign],[OtherAmt],[NetDiscount],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[ReverseCharge],[InsertStatus],[UpdateStatus],[PurchaseBillCode],[PurchaseBillNo],[PurchaseBillDate],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[TaxFree]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strPurchasePartyID + "','" + GetEntryType() + "','" + txtPurchaseType.Text + "','" + txtRemark.Text + "','" + txtSignAmt.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," +
                               +dba.ConvertObjectToDouble(txtDiscountAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dPTaxAmt + "," + dba.ConvertObjectToDouble(lblQty.Text) + "," + dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'','','" + MainPage.strLoginName + "','','" + txtPurchaseInvoiceNo.Text + "',1,0,'" + txtPBillCode.Text + "','" + txtPBillNo.Text + "'," + strPDate + ",'" + txtRoundOffSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxFree.Text) + ")  "
                               + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strPurchaseParty + "','PURCHASE RETURN','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNetAmt.Text + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "') ";

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTaxFree = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    dTotalAmt = (dAmt + dPacking + dFreightAmt + dTaxFree);
                    dDis = dba.ConvertObjectToDouble(rows.Cells["dis"].Value);

                    strSaleParty = Convert.ToString(rows.Cells["salesParty"].Value);
                    strFullName = strSaleParty.Split(' ');
                    if (strFullName.Length > 0)
                        strSalePartyID = strFullName[0].Trim();
                    else
                        strSalePartyID = "";

                    strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([BillCode],[BillNo],[RemoteID],[SRBillNo],[SalePartyID],[ItemName],[DesignName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[InsertStatus],[UpdateStatus]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + rows.Cells["saleReturnNo"].Value + "','" + strSalePartyID + "','" + rows.Cells["itemName"].Value + "','" + rows.Cells["designName"].Value + "','" + rows.Cells["disStatus"].Value + "'," + dDis + ",'" + rows.Cells["dhara"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxFree + "," + dTotalAmt + ",1,0) ";
                    if (rdoPurchase.Checked)
                        strQuery += strNetQuery = " Update GoodsReceive Set SaleBill='CLEAR' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar))='" + rows.Cells["saleReturnNo"].Value + "'  ";
                    else
                    {
                        if (Convert.ToString(rows.Cells["saleReturnNo"].Value) != "")
                            strQuery += strNetQuery = " Update SaleReturnDetails Set PurchaseReturnStatus=1,PurchaseReturnNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' Where (BillCode+' '+CAST(BillNo as varchar))='" + rows.Cells["saleReturnNo"].Value + "' and ItemName='" + rows.Cells["itemName"].Value + "' and DesignName='" + rows.Cells["designName"].Value + "' and PurchasePartyID='" + strPurchasePartyID + "' and PurchaseBillNo='" + txtPBillCode.Text + " " + txtPBillNo.Text + "' ";
                    }
                }



                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
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
                                   + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                strQuery += "  end";



                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (strNetQuery != "")
                        DataBaseAccess.CreateDeleteQuery(strNetQuery);

                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                string strDate = "", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtPDate.Text.Length == 10)
                {
                    DateTime pDate = dba.ConvertDateInExactFormat(txtPDate.Text);
                    strPDate = "'" + pDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strSaleParty = "", strPurchaseParty = "", strSalePartyID = "", strPurchasePartyID = "", strTaxAccountID = "", strDeletedSIDQuery = "";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }


                double dAmt = 0, dQty = 0, dPacking = 0, dFreightAmt = 0, dTaxFree = 0, dTotalAmt = 0, dDis = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "", strID = "";

                strQuery += "UPDATE  [dbo].[PurchaseReturn]  SET [Date]='" + strDate + "',[PurchasePartyID]='" + strPurchasePartyID + "',[EntryType]='" + GetEntryType() + "',[PurchaseType]='" + txtPurchaseType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSignAmt.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[ReverseCharge]='" + txtPurchaseInvoiceNo.Text + "',[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ", "
                         + "[RoundOffSign]='" + txtRoundOffSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + ", [NetDiscount]=" + dba.ConvertObjectToDouble(txtDiscountAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[PurchaseBillCode]='" + txtPBillCode.Text + "',[PurchaseBillNo]='" + txtPBillNo.Text + "',[PurchaseBillDate]=" + strPDate + ",[TaxFree]=" + dba.ConvertObjectToDouble(txtTaxFree.Text) + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strPurchaseParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strPurchasePartyID + "' Where [AccountStatus]='PURCHASE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                         + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";

                if (strDeletedSID != "" && rdoPurchase.Checked)
                {
                    strQuery += " Update GoodsReceive Set SaleBill='PENDING' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (Select SRBillNo from PurchaseReturnDetails Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID in (" + strDeletedSID + "))  ";
                    strDeletedSIDQuery += " Update GoodsReceive Set SaleBill='PENDING' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (Select SRBillNo from PurchaseReturnDetails Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ")) ";
                }

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTaxFree = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    dTotalAmt = (dAmt + dPacking + dFreightAmt + dTaxFree);
                    dDis = dba.ConvertObjectToDouble(rows.Cells["dis"].Value);

                    strID = Convert.ToString(rows.Cells["sid"].Value);
                    strSaleParty = Convert.ToString(rows.Cells["salesParty"].Value);
                    strFullName = strSaleParty.Split(' ');
                    if (strFullName.Length > 0)
                        strSalePartyID = strFullName[0].Trim();
                    else
                        strSalePartyID = "";

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([BillCode],[BillNo],[RemoteID],[SRBillNo],[SalePartyID],[ItemName],[DesignName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[InsertStatus],[UpdateStatus]) VALUES  "
                                        + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + rows.Cells["saleReturnNo"].Value + "','" + strSalePartyID + "','" + rows.Cells["itemName"].Value + "','" + rows.Cells["designName"].Value + "','" + rows.Cells["disStatus"].Value + "'," + dDis + ",'" + rows.Cells["dhara"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxFree + "," + dTotalAmt + ",1,0) ";
                    }
                    else
                    {
                        strQuery += "Update [dbo].[PurchaseReturnDetails] SET [SRBillNo]='" + rows.Cells["saleReturnNo"].Value + "',[SalePartyID]='" + strSalePartyID + "',[ItemName]='" + rows.Cells["itemName"].Value + "',[DesignName]='" + rows.Cells["designName"].Value + "',[DisStatus]='" + rows.Cells["disStatus"].Value + "',[Discount]=" + dDis + ",[Dhara]='" + rows.Cells["dhara"].Value + "',[Qty]=" + dQty + ",[Amount]=" + dAmt + ",[Packing]=" + dPacking + ",[Freight]=" + dFreightAmt + " ,[TaxFree]=" + dTaxFree + ",[TotalAmt]=" + dTotalAmt + ",[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + " ";
                    }

                    if (rdoPurchase.Checked)
                        strQuery += " Update GoodsReceive Set SaleBill='CLEAR' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar))='" + rows.Cells["saleReturnNo"].Value + "'  ";
                    else
                    {
                        if (Convert.ToString(rows.Cells["saleReturnNo"].Value) != "")
                            strQuery += " Update SaleReturnDetails Set PurchaseReturnStatus=1,PurchaseReturnNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' Where (BillCode+' '+CAST(BillNo as varchar))='" + rows.Cells["saleReturnNo"].Value + "' and ItemName='" + rows.Cells["itemName"].Value + "' and DesignName='" + rows.Cells["designName"].Value + "' and PurchasePartyID='" + strPurchasePartyID + "'  and PurchaseBillNo='" + txtPBillCode.Text + " " + txtPBillNo.Text + "' ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
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
                             + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                object objValue = "True";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strDeletedSID + ") ";
                    if (!rdoPurchase.Checked)
                        strQuery += " Update SRD SET PurchaseReturnStatus=0,PurchaseReturnNumber='' from PurchaseReturn PR inner join PurchaseReturnDetails PRD on PR.BillCode=PRD.BillCode and PR.BillNo=PRD.BillNo inner join SaleReturnDetails SRD on (SRD.BillCode+' '+CAST(SRD.BillNo as varchar))=PRD.SRBillNo and SRD.PurchasePartyID=PR.PurchasePartyID and PRD.ItemName=SRD.ItemName and PRD.Qty=SRD.Qty and PRD.Amount=SRD.Amount Where PRD.BillCode='" + txtBillCode.Text + "' and PRD.BillNo=" + txtBillNo.Text + " and PRD.SID in (" + strDeletedSID + ") ";


                    strDeletedSIDQuery += " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";
                    if (!rdoPurchase.Checked)
                        strDeletedSIDQuery += " Update SRD SET PurchaseReturnStatus=0,PurchaseReturnNumber='' from PurchaseReturn PR inner join PurchaseReturnDetails PRD on PR.BillCode=PRD.BillCode and PR.BillNo=PRD.BillNo inner join SaleReturnDetails SRD on (SRD.BillCode+' '+CAST(SRD.BillNo as varchar))=PRD.SRBillNo and SRD.PurchasePartyID=PR.PurchasePartyID and PRD.ItemName=SRD.ItemName and PRD.Qty=SRD.Qty and PRD.Amount=SRD.Amount Where PRD.BillCode='" + txtBillCode.Text + "' and PRD.BillNo=" + txtBillNo.Text + " and PRD.RemoteID in (" + strDeletedSID + ") ";

                    objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";




                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (!Convert.ToBoolean(objValue))
                    {
                        strQuery = strQuery.Replace("Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;", "");
                        DataBaseAccess.CreateDeleteQuery(strQuery);
                    }
                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);

                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    btnEdit.Text = "&Edit";
                    updateStatus = true;
                    if (saleStatus)
                        this.Close();
                    else
                    {
                        ClearAllText();
                        BindRecordWithControl(txtBillNo.Text);
                    }
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
            {
                txtReason.Clear();
                pnlDeletionConfirmation.Visible = true;
                txtReason.Focus();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
        }

        private void CheckAvailability()
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (txtBillNo.Text != "")
                    {
                        //object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),0) from MaxSerialNo");
                        //int maxBillNo = Convert.ToInt32(objMax);
                        //if (maxBillNo < Convert.ToInt32(txtBillNo.Text))
                        //{
                        int check = dba.CheckPurchaseReturnAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check < 1)
                        {
                            lblMsg.Text = txtBillNo.Text + "  Bill No is Available ........";
                            lblMsg.ForeColor = Color.White;
                            lblMsg.Visible = true;

                        }
                        else
                        {
                            lblMsg.Text = txtBillNo.Text + " Bill No is already exist ! ";
                            lblMsg.ForeColor = Color.White;
                            lblMsg.Visible = true;
                            txtBillNo.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Bill No is already in used please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();

                    }
                    //}
                    //else
                    //{
                    //    lblMsg.Text = "Please Choose Bill Number .......";
                    //    lblMsg.ForeColor = Color.White;
                    //    lblMsg.Visible = true;
                    //    txtBillNo.Focus();

                    //}
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability in Purchase Return ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtBillNo.Focus();
            }
        }
        private void txtReceiptNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        CheckAvailability();
                    }
                    else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
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

        private void GoodsReciept_Load(object sender, EventArgs e)
        {
            try
            {
                EditOption();
                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtBillNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView)
                {
                    if (!MainPage.mymainObject.bDrCrNoteAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    if (!MainPage.mymainObject.bDrCrNoteView)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPurchaseType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPurchaseType.Text = objSearch.strSelectedData;
                        CalculateNetAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSignAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdItem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int cIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (cIndex == 7 || cIndex == 8 || cIndex == 9 || cIndex == 10 || cIndex == 11)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
            else if (cIndex == 3)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.CharacterCasing = CharacterCasing.Upper;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox1_KeyPress);
            }
        }
        private void txtBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int cIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (cIndex == 7 || cIndex == 8 || cIndex == 9 || cIndex == 10 || cIndex == 11)
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                {
                    //if (cIndex == 6)
                    //    dba.KeyHandlerPoint(sender, e, 0);
                    //else
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
        }

        private void dgrdItem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int cIndex = e.ColumnIndex;
                if (cIndex == 7 || cIndex == 8 || cIndex == 9 || cIndex == 10 || cIndex == 11)
                {
                    CalculateTotalAmount();
                }
                if (cIndex == 3)
                {
                    var value = dgrdDetails.CurrentCell.Value;
                    if (value == null || value.ToString() == "")
                        dgrdDetails.CurrentCell.Value = dgrdDetails.CurrentRow.Cells["itemName"].Value;
                }
            }
            catch
            {
            }
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtSignAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSignAmt.Text == "")
                    txtSignAmt.Text = "-";
                if (txtOtherAmt.Text == "")
                    txtOtherAmt.Text = "0.00";
                CalculateNetAmount();
            }
        }

        private void txtTaxPer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape)
            {
                if (!pnlTax.Visible)
                    pnlTax.Visible = true;
                else
                    pnlTax.Visible = false;
            }
        }

        private void txtOtherAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                if (btnAdd.Text == "&Save")
                    btnAdd.Focus();
                else if (btnEdit.Text == "&Update")
                    btnEdit.Focus();
            }
        }

        private void GoodscumPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void CalculateTotalAmount()
        {
            double dQty = 0, dAmt = 0, dPacking = 0, dFreightAmt = 0, dTaxAmt = 0, dTQty = 0, dTAmt = 0, dTPackingAmt = 0, dTFreightAmt = 0, dTTaxAmt = 0, dTotalAmt = 0, dTTotalAmt = 0, dDis = 0, dDiscountAmt = 0;

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                dTQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                dTPackingAmt += dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                dTFreightAmt += dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                dDis = dba.ConvertObjectToDouble(rows.Cells["disStatus"].Value + "" + rows.Cells["dis"].Value);
                dDiscountAmt += ((dAmt * dDis) / 100);
                //if (!rdoPurchase.Checked)
                //    dDiscountAmt += ((dPacking * MainPage.dPackingDhara) / 100) + (((dFreightAmt) * MainPage.dFreightDhara) / 100);

                dTTotalAmt += dTotalAmt = dAmt + dPacking + dFreightAmt + dTaxAmt;
                rows.Cells["totalAmt"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy);
            }

            txtDiscountAmt.Text = dDiscountAmt.ToString("N2", MainPage.indianCurancy);
            lblQty.Text = dTQty.ToString("N2", MainPage.indianCurancy);
            lblGrossAmt.Text = dTTotalAmt.ToString("N2", MainPage.indianCurancy);

            CalculateNetAmount();
        }

        private void CalculateNetAmount()
        {

            double dTaxFree, dServiceAmt = 0, dDiscount = 0, dPackingAmt = 0, dOtherAmt = 0, dRoundOffAmt = 0, dTaxableAmt = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dTaxAmt = 0, dFinalAmt = 0;
            try
            {

                //   dDisPer = dba.ConvertObjectToDouble(txtDisPer.Text);       

                dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);
                dDiscount = dba.ConvertObjectToDouble(txtDiscountAmt.Text);
                dTaxFree = dba.ConvertObjectToDouble(txtTaxFree.Text);

                dTOAmt = dOtherAmt + dPackingAmt;
                dFinalAmt = dGrossAmt + dDiscount + dTOAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                dNetAmt = dGrossAmt + dDiscount + dOtherAmt + dPackingAmt + dTaxAmt + dServiceAmt + dTaxFree;


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

                lblNetAmt.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);
                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0, dServiceAmt = 0;
            string _strTaxType = "";
            try
            {
                if (MainPage._bTaxStatus && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dgrdTax.Rows.Clear();
                    DataTable _dt = dba.GetSaleTypeDetails(txtPurchaseType.Text, "PURCHASE");
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

                            double dAmt = 0, dQty = 0, dPacking = 0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                                dDisStatus = dba.ConvertObjectToDouble(rows.Cells["disStatus"].Value + "" + rows.Cells["dis"].Value);

                                dPacking += dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value) + dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);// + dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                                if (dAmt > 0)
                                {
                                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);

                                    if (strQuery != "")
                                    {
                                        strQuery += " UNION ALL ";
                                        strServiceQuery += " UNION ALL ";
                                    }

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 + " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + " * 100) / (100 + TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/ " + dQty + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + "* 100) / (100 + TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dAmt + ">0  ";
                                    //  strServiceQuery += " Select (SUM(CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+GM.TaxRate)) else " + dAmt + " end)  *(100 + " + dDisStatus + ")/ 100.00)  as Amount,'" + strItemName + "' as ItemName," + dQty + " Quantity from Items _IM Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end))*(100+" + dDisStatus + "))/100.00)/" + dQty + ")>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end))*(100+" + dDisStatus + "))/100.00)/" + dQty + ")<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='') as GM  Where _IM.ItemName='" + strItemName + "' ";
                                }
                            }

                            if (strQuery != "")
                            {
                                dPacking += dOtherAmt;
                                if (dPacking != 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount," + dTaxPer + " as TaxRate ";
                                }
                                if (strQuery != "")
                                {
                                    strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                                   + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate";
                                    //+ " OUTER APPLY (Select (SUM(Amount)+(CASE WHen '" + _strTaxType + "'='INCLUDED' then ((" + dPacking + "*_Goods.MTaxRate)/100.00) else 0 end)) ServiceAmt   from ( "
                                    //+ " Select (((((Amount)*TaxRate/100.00)*TaxDhara)/100.00)*(CASE WHen '" + _strTaxType + "'='INCLUDED' then ((100+_Goods.MTaxRate)/100.00) else 1 end)) Amount from ( " + strServiceQuery
                                    //+ " )_Sales OUTER APPLY (Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((_Sales.Amount*100)/(100+TaxRate)) else _Sales.Amount end))))/Quantity)>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((_Sales.Amount*100)/(100+TaxRate)) else _Sales.Amount end))))/Quantity)<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _Sales.ItemName=_IM.ItemName) as GM  ) _Sales) SCharge)_FinalSales ";


                                    DataTable dt = dba.GetDataTable(strQuery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        double dMaxRate = 0, dTTaxAmt = 0;
                                        //  BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                        dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);

                                        dTaxAmt = dTTaxAmt;
                                        if (dPacking == 0)
                                            dTaxPer = dMaxRate;

                                        pnlTax.Visible = true;
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

                            pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text = "0.00";
                    }
                }

                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bDrCrNoteAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bDrCrNoteEdit)
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

        //private void BindTaxDetails(DataTable _dt, DataRow _row,ref double dMaxRate,ref double dTTaxAmt,ref double dTaxableAmt)
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                    {
                        if (rdoManual.Checked && e.ColumnIndex != 0)
                        {
                            if (e.ColumnIndex == 1)
                            {
                                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;

                            }
                            else if (e.ColumnIndex == 2)
                            {
                                SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strItem = objSearch.strSelectedData.Split('|');
                                    if (strItem.Length > 0)
                                        dgrdDetails.CurrentCell.Value = strItem[0];

                                    if (Convert.ToString(dgrdDetails.CurrentRow.Cells["designName"].Value) == "")
                                    {
                                        strItem = strItem[0].Split(':');
                                        dgrdDetails.CurrentRow.Cells["designName"].Value = strItem[0].Trim();
                                    }
                                }
                                CalculateTotalAmount();
                            }
                        }
                        else
                        {
                            string strQuery = "";
                            if (rdoPurchase.Checked)
                            {
                                if (txtPBillCode.Text != "" && txtPBillNo.Text != "" && txtPDate.Text.Length == 10)
                                {
                                    strQuery += " and GR.ReceiptNo=" + txtPBillNo.Text + "  ";

                                    if (txtPurchaseParty.Text != "")
                                    {
                                        string[] strFullName = txtPurchaseParty.Text.Split(' ');
                                        if (strFullName.Length > 1)
                                            strQuery += " and GR.PurchasePartyID ='" + strFullName[0].Trim() + "'  ";
                                    }

                                    SearchData objSearch = new SearchData("PURCHASEDETAILSFORPRETURN", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space);
                                    objSearch.ShowDialog();
                                    if (objSearch.strSelectedData != "")
                                    {
                                        string[] strItem = objSearch.strSelectedData.Split('|');
                                        if (strItem.Length > 0)
                                            SetSelectedDetails(strItem, true);
                                        CalculateTotalAmount();
                                    }
                                }
                            }
                            else if (rdoBySaleReturnSNo.Checked || rdoManual.Checked)
                            {
                                if (!rdoManual.Checked)
                                {
                                    if (txtPBillCode.Text != "" && txtPBillNo.Text != "")
                                        strQuery += " and SRD.PurchaseBillNo='" + txtPBillCode.Text + " " + txtPBillNo.Text + "'  ";
                                }
                                if (txtPurchaseParty.Text != "")
                                {
                                    string[] strFullName = txtPurchaseParty.Text.Split(' ');
                                    if (strFullName.Length > 1)
                                        strQuery += " and SRD.PurchasePartyID ='" + strFullName[0].Trim() + "'  ";
                                }

                                SearchData objSearch = new SearchData("SALESRETURNBILLDETAILS", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strItem = objSearch.strSelectedData.Split('|');
                                    if (strItem.Length > 0)
                                        SetSelectedDetails(strItem, false);

                                    CalculateTotalAmount();
                                }
                            }
                            e.Cancel = true;
                        }
                        if (e.ColumnIndex != 3)
                            e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10)
                        e.Cancel = false;
                    else if ((rdoManual.Checked || MainPage.strUserRole.Contains("ADMIN")) && e.ColumnIndex == 6)
                    {
                        SearchData objSearch = new SearchData("DHARA", "SEARCH DHARA TYPE", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            GetDharaDetails(dgrdDetails.CurrentRow);
                        }
                        CalculateTotalAmount();
                        e.Cancel = true;
                    }
                    else if (rdoManual.Checked && (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11))
                        e.Cancel = false;
                    else if (MainPage.strUserRole.Contains("ADMIN") && e.ColumnIndex == 8)
                        e.Cancel = false;
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void GetDharaDetails(DataGridViewRow row)
        {
            string strParty = txtPurchaseParty.Text, strDType = Convert.ToString(row.Cells["dhara"].Value), strDhara = "", strQuery = "";
            if (strDType == "NORMAL")
                strQuery = " Select NormalDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";
            else if (strDType == "SNDHARA")
                strQuery = " Select SNDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";
            else
                strQuery = " Select CFormApply as PremiumDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";

            DataTable _dt = dba.GetDataTable(strQuery);
            if (_dt.Rows.Count > 0)
            {

                strDhara = Convert.ToString(_dt.Rows[0][0]);
                if (strDhara != "")
                {
                    SetDiscountDetails(strDhara, strDType, row.Index, Convert.ToString(_dt.Rows[0][1]));
                }
                else
                {
                    MessageBox.Show("Please enter Super Net Dhara in party master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please enter Dhara in party master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = false;
            }
        }

        private void SetDiscountDetails(string strDhara, string strDType, int rowIndex, string strCategory)
        {
            //double _dPer = 3;
            if (strDhara != "")
            {
                //if (strCategory.ToUpper() == "CASH PURCHASE")
                //    _dPer = 5;
                double dDhara = dba.ConvertObjectToDouble(strDhara);

                // dDhara = _dPer - dDhara;
                dDhara = dDhara * -1.00;

                if (dDhara >= 0)
                {
                    dgrdDetails.Rows[rowIndex].Cells["disStatus"].Value = "+";
                    dgrdDetails.Rows[rowIndex].Cells["dis"].Value = dDhara;
                }
                else
                {
                    dgrdDetails.Rows[rowIndex].Cells["disStatus"].Value = "-";
                    dgrdDetails.Rows[rowIndex].Cells["dis"].Value = Math.Abs(dDhara);
                }
                CalculateTotalAmount();

                if (!MainPage.mymainObject.bDrCrNoteEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                    btnEdit.Enabled = btnDelete.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please Enter Normal Dhara in Party Master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnEdit.Enabled = btnDelete.Enabled = false;
            }
        }

        private void SetSelectedDetails(string[] strDetails, bool _status)
        {
            try
            {
                dgrdDetails.CurrentRow.Cells["saleReturnNo"].Value = strDetails[2];
                dgrdDetails.CurrentRow.Cells["salesParty"].Value = strDetails[1];
                dgrdDetails.CurrentRow.Cells["itemName"].Value = strDetails[0];
                dgrdDetails.CurrentRow.Cells["designName"].Value = strDetails[12];
                dgrdDetails.CurrentRow.Cells["dhara"].Value = strDetails[5];
                dgrdDetails.CurrentRow.Cells["gQty"].Value = strDetails[6];
                dgrdDetails.CurrentRow.Cells["gAmount"].Value = strDetails[7];
                dgrdDetails.CurrentRow.Cells["gPacking"].Value = strDetails[8];
                dgrdDetails.CurrentRow.Cells["gFreight"].Value = strDetails[9];
                dgrdDetails.CurrentRow.Cells["gTax"].Value = strDetails[10];
                dgrdDetails.CurrentRow.Cells["totalAmt"].Value = strDetails[11];

                if (_status)
                {
                    dgrdDetails.CurrentRow.Cells["disStatus"].Value = strDetails[3];
                    dgrdDetails.CurrentRow.Cells["dis"].Value = strDetails[4];
                }
                else
                {
                    bool _bStatus = true;
                    if (strDetails[14] == "0")
                        _bStatus = false;
                    if (!_bStatus)
                    {
                        dgrdDetails.CurrentRow.Cells["disStatus"].Value = strDetails[3];
                        dgrdDetails.CurrentRow.Cells["dis"].Value = strDetails[4];
                    }
                    else
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtPDate.Text);

                        double dDhara = dba.ConvertObjectToDouble(strDetails[3] + "" + strDetails[4]), _dPer = 0;
                        if (strDetails[13] == "CASH PURCHASE")
                            _dPer = 5;
                        else
                            _dPer = 3;

                        if (strDetails[13] == "CLOTH PURCHASE" || txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                            _dPer -= 1;

                        dDhara = _dPer - dDhara;
                        dDhara = dDhara * -1;
                        if (dDhara >= 0)
                        {
                            dgrdDetails.CurrentRow.Cells["disStatus"].Value = "+";
                            dgrdDetails.CurrentRow.Cells["dis"].Value = dDhara;
                        }
                        else
                        {
                            dgrdDetails.CurrentRow.Cells["disStatus"].Value = "-";
                            dgrdDetails.CurrentRow.Cells["dis"].Value = Math.Abs(dDhara);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoAll.Checked && btnAdd.Text == "&Save")
                {
                    if (txtPBillNo.Text != "" && txtPDate.Text.Length == 10)
                        GetPurchaseBillDetails(rdoCurrent.Checked);
                    else
                        MessageBox.Show("Sorry ! Please enter purchase bill no and purchase bill date !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoManual.Checked)
            {
                txtPBillCode.ReadOnly = txtPBillNo.ReadOnly = false;
                if (btnAdd.Text == "&Save")
                {
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                }
            }
            else
            {
                txtPBillNo.ReadOnly = true;// txtPBillCode.ReadOnly =
                                           // txtPBillCode.Text = strPurchaseBillCode;
            }
        }

        private void GetPurchaseBillDetails(bool _bstatus)
        {
            string strQuery = "", strPurchasePartyID = "", strDBName = "", strDBNameNOdbo = "";
            if (txtBillNo.Text != "" && txtPBillNo.Text != "")
            {
                dgrdDetails.Rows.Clear();
                if (txtPurchaseParty.Text != "")
                {
                    string[] strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strPurchasePartyID = strFullName[0].Trim();
                }
                //if (strOLDYearDB != "")
                //{
                //    strDBName = strOLDYearDB + ".dbo.";
                //    strDBNameNOdbo = strOLDYearDB + ".";
                //}

                if (rdoPurchase.Checked)
                    strQuery = "  Select (SR.ReceiptCode+' '+CAST(SR.ReceiptNo as varchar)) as BillNo," + strDBNameNOdbo + "dbo.GetFullName(SR.SalePartyID)SalesParty,SRD.ItemName,SRD.DesignName,'-' as DisStatus,SR.DisPer as Discount,SR.Dhara,SRD.Quantity Qty,SRD.Amount,SRD.PackingAmt as Packing,SRD.FreightAmt Freight,SRD.TaxAmt TaxFree,(SRD.Amount + SRD.PackingAmt+SRD.FreightAmt+SRD.TaxAmt)TotalAmt,(Select TOP 1 UPPER(Category) from " + strDBName + "SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))=SR.PurchasePartyID)Category,0 as DiscountType   from " + strDBName + "GoodsReceive SR inner join " + strDBName + "GoodsReceiveDetails SRD on SR.ReceiptCode=SRD.ReceiptCode and SR.ReceiptNo=SRD.ReceiptNo  Where SR.ReceiptCode='" + txtPBillCode.Text + "' and SR.ReceiptNo=" + txtPBillNo.Text + " and SR.PurchasePartyID='" + strPurchasePartyID + "'  Order by SRD.ID   ";
                else
                    strQuery = " Select (SR.BillCode+' '+CAST(SR.BillNo as varchar)) as BillNo," + strDBNameNOdbo + "dbo.GetFullName(SR.SalePartyID)SalesParty,SRD.ItemName,SRD.DesignName,SRD.DisStatus,SRD.Discount,SRD.Dhara,SRD.Qty,SRD.Amount,SRD.Packing,SRD.Freight,SRD.TaxFree,(SRD.Amount + SRD.Packing+SRD.Freight+SRD.TaxFree)TotalAmt,(Select TOP 1 UPPER(Category) from " + strDBName + "SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strPurchasePartyID + "')Category,DiscountType   from " + strDBName + "SaleReturn SR inner join " + strDBName + "SaleReturnDetails SRD on SR.BillCode=SRD.BillCOde and SR.BillNo=SRD.BillNo  Where SRD.PurchaseBillNo='" + txtPBillCode.Text + " " + txtPBillNo.Text + "' and SRD.PurchasePartyID='" + strPurchasePartyID + "' and PurchaseReturnStatus=0  Order by SR.BillNo  ";
                DataTable dt = null;
                if (_bstatus)
                {
                    dt = dba.GetDataTable(strQuery);
                }
                else
                {
                    SearchDataOnOld obj = new SearchDataOnOld();
                    obj._bPreviousDBStatus = true;
                    dt = obj.GetDataTable(strQuery);
                }
             
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    double _dPer = 3;
                    DateTime sDate = dba.ConvertDateInExactFormat(txtPDate.Text);
                    string strCategory = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        strCategory = Convert.ToString(row["Category"]);
                        if (strCategory == "CASH PURCHASE")
                            _dPer = 5;
                        else
                            _dPer = 3;

                        if (strCategory == "CLOTH PURCHASE" || txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                            _dPer -= 1;

                        dgrdDetails.Rows[_rowIndex].Cells["saleReturnNo"].Value = row["BillNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["salesParty"].Value = row["SalesParty"];
                        dgrdDetails.Rows[_rowIndex].Cells["itemName"].Value = row["ItemName"];
                        dgrdDetails.Rows[_rowIndex].Cells["designName"].Value = row["DesignName"];
                        //dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = row["DisStatus"];
                        //dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = row["Discount"];
                        dgrdDetails.Rows[_rowIndex].Cells["dhara"].Value = row["Dhara"];
                        dgrdDetails.Rows[_rowIndex].Cells["gQty"].Value = row["Qty"];
                        dgrdDetails.Rows[_rowIndex].Cells["gAmount"].Value = row["Amount"];
                        dgrdDetails.Rows[_rowIndex].Cells["gPacking"].Value = row["Packing"];
                        dgrdDetails.Rows[_rowIndex].Cells["gFreight"].Value = row["Freight"];
                        dgrdDetails.Rows[_rowIndex].Cells["gTax"].Value = row["TaxFree"];
                        dgrdDetails.Rows[_rowIndex].Cells["totalAmt"].Value = row["TotalAmt"];

                        if (!Convert.ToBoolean(row["DiscountType"]))
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = row["DisStatus"];
                            dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = row["Discount"];
                        }
                        else
                        {
                            double dDhara = dba.ConvertObjectToDouble(row["DisStatus"] + "" + row["Discount"]);
                            dDhara = _dPer - dDhara;
                            dDhara = dDhara * -1;
                            if (dDhara >= 0)
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = "+";
                                dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = dDhara;
                            }
                            else
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = "-";
                                dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = Math.Abs(dDhara);
                            }
                        }

                        _rowIndex++;
                    }
                }
                CalculateTotalAmount();
            }
        }

        private void txtOtherAmt_Enter_1(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0.00")
                        txt.Clear();
                }
            }
        }

        private void txtPBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && !rdoManual.Checked)
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strQuery = "";
                        if (txtPurchaseParty.Text != "")
                        {
                            string[] strFullName = txtPurchaseParty.Text.Split(' ');
                            if (strFullName.Length > 1)
                                strQuery = " Where PurchasePartyID ='" + strFullName[0].Trim() + "'  ";
                            if (rdoPurchase.Checked)
                            {
                                if (rdoOldYear.Checked)
                                {
                                    SearchDataOnOld objSearch = new SearchDataOnOld("PURCHASEBILLNOFORMPURCHASE", strQuery, "SEARCH PURCHASE BILL NO", e.KeyCode, true);
                                    objSearch.ShowDialog();
                                    string[] strBillNo = objSearch.strSelectedData.Split('|');                                    
                                    if (strBillNo.Length > 1)
                                    {
                                        txtPBillCode.Text = strBillNo[0];
                                        txtPBillNo.Text = strBillNo[1];
                                        txtPDate.Text = strBillNo[2];

                                        GetPurchaseBillDetails(false);
                                    }
                                }
                                else
                                {
                                    SearchData objSearch = new SearchData("PURCHASEBILLNOFORMPURCHASE", strQuery, "SEARCH PURCHASE BILL NO", e.KeyCode);
                                    objSearch.ShowDialog();
                                    if (objSearch.strSelectedData != "")
                                    {
                                        string[] strData = objSearch.strSelectedData.Split('|');                                       
                                        if (strData.Length > 1)
                                        {
                                            txtPBillCode.Text = strData[0];
                                            txtPBillNo.Text = strData[1];
                                            txtPDate.Text = strData[2];
                                            
                                            GetPurchaseBillDetails(true);
                                        }
                                    }                                   
                                }
                            }
                            else
                            {
                                SearchData objSearch = new SearchData("PURCHASEBILLNOFORRETURN", strQuery, "SEARCH SALE BILL NO", e.KeyCode);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strData = objSearch.strSelectedData.Split('|');                                  
                                    if (strData.Length > 1)
                                    {
                                        txtPBillNo.Text = strData[0];
                                        txtPDate.Text = strData[1];
                                    }
                                }
                                rdoBySaleReturnSNo.Checked = true;
                                if (dgrdDetails.Rows.Count == 0)
                                    dgrdDetails.Rows.Add();
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    GSTPrintAndPreview(false, "");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Purchase Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (txtPurchaseType.Text != "")
                    {
                        GSTPrintAndPreview(true, "");
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void txtOtherAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void rdoPurchase_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoPurchase.Checked && btnAdd.Text == "&Save")
                {
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                }
            }
            catch
            {
            }
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
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

                            strQuery += " Update SRD SET SRD.PurchaseReturnStatus=0,SRD.PurchaseReturnNumber='' from PurchaseReturn PR inner join PurchaseReturnDetails PRD on PR.BillCode=PRD.BillCode and PR.BillNo=PRD.BillNo inner join SaleReturnDetails SRD on (SRD.BillCode+' '+CAST(SRD.BillNo as varchar))=PRD.SRBillNo and SRD.PurchasePartyID=PR.PurchasePartyID and PRD.ItemName=SRD.ItemName and PRD.Qty=SRD.Qty and PRD.Amount=SRD.Amount Where PRD.BillCode='" + txtBillCode.Text + "' and PRD.BillNo=" + txtBillNo.Text + " "
                                     + " Delete from [PurchaseReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                     + " Delete from [PurchaseReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('PURCHASE RETURN','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            if (rdoPurchase.Checked)
                                strQuery += " Update GoodsReceive Set SaleBill='PENDING' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (Select SRBillNo from PurchaseReturnDetails Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ")  ";

                            strQuery += " Update SaleReturnDetails Set PurchaseReturnStatus=0 Where PurchaseReturnStatus=1 and (BillCode+' '+CAST(BillNo as nvarchar)) not in (Select SRBillNo from  PurchaseReturnDetails) ";

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from PurchaseReturn Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
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
                        SearchData objSearch = new SearchData("PURCHASERETURNCODE", "SEARCH PURCHASE RETURN CODE", e.KeyCode);
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

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("PURCHASERETURN", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtPBillCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtOtherAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";
                    CalculateNetAmount();
                }
            }
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

                    if (IndexColmn < dgrdDetails.ColumnCount - 2)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdDetails.RowCount - 1)
                    {
                        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                        {
                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value) != "" && Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["gAmount"].Value) != "")
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow + 1].Cells["purchaseSerialNo"];
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
                }
                else if (e.KeyCode == Keys.F1)
                {
                    Index = dgrdDetails.CurrentCell.RowIndex;
                    if (btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(Index);
                        CalculateTotalAmount();
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["sid"].Value);
                        if (strID != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (strDeletedSID != "")
                                    strDeletedSID += ",";
                                strDeletedSID += strID;
                                dgrdDetails.Rows.RemoveAt(Index);
                                CalculateTotalAmount();
                            }
                        }
                        else
                        {
                            dgrdDetails.Rows.RemoveAt(Index);
                            CalculateTotalAmount();
                        }
                    }

                }
            }
            catch
            {
            }
        }

        private void rdoByPurchaseSNo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoBySaleReturnSNo.Checked && btnAdd.Text == "&Save")
                {
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                }
            }
            catch
            {
            }
        }

        private void txtReverseCharge_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("REVERSECHARGES", "SEARCH REVERSE CHARGES", e.KeyCode);
                        objSearch.ShowDialog();
                        txtRemark.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "18.00";
                    double dTaxPer = dba.ConvertObjectToDouble(txt.Text);
                    if (dTaxPer != 3 && dTaxPer != 5 && dTaxPer != 12 && dTaxPer != 18 && dTaxPer != 28)
                        txt.Text = "18.00";
                    CalculateNetAmount();
                }
            }
        }

        private void txtTaxFree_Leave(object sender, EventArgs e)
        {

            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";
                    CalculateNetAmount();
                }
            }
        }
        private void GetOldYearDBName()
        {
            strOLDYearDB = "";
            if (rdoOldYear.Checked)
            {
                try
                {
                    string strQry = "SELECT DbName = 'A' + Right(Prev_Y_Path, CHARINDEX('\\',REVERSE(Prev_Y_Path))-1) FROM Company where Prev_Y_Path != ''";
                    DataTable dt = dba.GetDataTable(strQry);
                    if (dt.Rows.Count > 0)
                    {
                        string DbName = Convert.ToString(dt.Rows[0]["DbName"]);
                        if (DbName.Length > 1 && DbName != "A0")
                        {
                            strOLDYearDB = DbName;
                        }
                        else
                            MessageBox.Show("Sorry ! Previouse year Database is not set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        MessageBox.Show("Sorry ! Previouse year Database is not set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex) { }
            }
        }
        private void rdoOldYear_CheckedChanged(object sender, EventArgs e)
        {
            GetOldYearDBName();
        }

        private void txtTaxFree_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private bool GSTPrintAndPreview(bool _pstatus, string strPath)
        {
            DataTable _dtGST = null, _dtSalesAmt = null; ;
            bool _bIGST = false;
            DataTable dt = dba.CreateDebitNoteDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "DEBIT NOTE", "DEBIT NOTE");
            if (dt.Rows.Count > 0)
            {
                if (!_bIGST)
                {
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        Reporting.DCNoteReport_CGST objOL_salebill = new Reporting.DCNoteReport_CGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();

                    }
                    else
                    {
                        Reporting.DCNoteReport_CGST_Retail objOL_salebill = new Reporting.DCNoteReport_CGST_Retail();
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
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        Reporting.DCNoteReport_IGST objOL_salebill = new Reporting.DCNoteReport_IGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();

                    }
                    else
                    {
                        Reporting.DCNoteReport_IGST_Retail objOL_salebill = new Reporting.DCNoteReport_IGST_Retail();
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
        private void FinallyPrint(bool _pstatus, CrystalDecisions.CrystalReports.Engine.ReportClass objReport, string strPath)
        {
            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            if (strPath != "")
            {
                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                objReport.Close();
                objReport.Dispose();
            }
            else
            {
                if (_pstatus)
                {
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport, false, MainPage.iNCopyPurRtn);
                    else
                    {
                        //  string strValue = "0";
                        if (_pstatus)
                        {
                            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                            defS.Collate = false;
                            defS.FromPage = 0;
                            defS.ToPage = 0;
                            defS.Copies = (short)MainPage.iNCopyPurRtn;

                            //strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                            //if (strValue != "" && strValue != "0")
                            //{
                            //    int nCopy = Int32.Parse(strValue);
                            objReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            //}
                        }

                    }
                }
                else
                {
                    Reporting.ShowReport report = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                    report.myPreview.ReportSource = objReport;
                    report.ShowDialog();
                }
                objReport.Close();
                objReport.Dispose();
            }
        }

        private string CreatePDFFile(bool _createPDF)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
            {

                if (_createPDF)
                {
                    SaveFileDialog _browser = new SaveFileDialog();
                    _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                    _browser.FileName = txtBillNo.Text + ".pdf";
                    _browser.ShowDialog();

                    if (_browser.FileName != "")
                        strPath = _browser.FileName;
                }
                else
                {
                    string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SaleReturn\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    _strPath += "\\" + _strFileName;

                    strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                    if (File.Exists(strPath))
                        File.Delete(strPath);
                    Directory.CreateDirectory(_strPath);
                }

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strPath);
                    if (_bstatus)
                    {
                        return strPath;
                    }
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


        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    string strPath = CreatePDFFile(false), strEmailID = "", strWhatsAppNo = "";
                    if (strPath != "")
                    {
                        strFilePath = strPath;
                        string[] strParty = txtPurchaseParty.Text.Split(' ');
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
            string _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strBranchCode = txtBillCode.Text;
            if (!strBranchCode.Contains("-"))
                strBranchCode = "18-19/" + strBranchCode;
            string strWhastappMessage = "", strMsgType = "", strMType = "";
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtPurchaseParty.Text);
            if (btnEdit.Text == "&Update")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                //strMessage = "M/S " + strName + ", debit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " UPDATED.";
                strMsgType = "debit_note";
                strMType = "debitnote_generation";
            }
            else
            {
                // strMessage = "M/S " + strName + ", new debit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " CREATED.";
                strMsgType = "debit_note_update_pdf";
                strMType = "debitnote_update";
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

                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                strWhastappMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + lblNetAmt.Text + "\"}";
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strWhastappMessage, "", "");
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
                        strMessage = "M/S : " + txtPurchaseParty.Text + " , we have created your debit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "M/S : " + txtPurchaseParty.Text + ", we have updated your debit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
                    }

                    if (btnAdd.Text == "&Save")
                        strSub = "Debit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " created.";
                    else
                        strSub = "Alert ! Debit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " updated.";
                }
                else
                {
                    strMessage = " Alert ! Debit note bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + "</b> is Deleted by : " + MainPage.strLoginName + "  and  the deleted debit note bill is attached with this mail. ";
                    strSub = "Alert ! Debit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " deleted by : " + MainPage.strLoginName;
                }

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "DEBIT NOTE", true);
                if (billStatus == 0 && bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
        }

    }
}
