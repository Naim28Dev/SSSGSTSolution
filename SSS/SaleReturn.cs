using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace SSS
{
    public partial class SaleReturn : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strDeletedSID = "",strSaleBillCode="", strAmendedQuery = "", strOldPartyName="";
        double dOldNetAmt = 0;
        public bool saleStatus = false,updateStatus=false,newStatus=false;       

        public SaleReturn()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public SaleReturn(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            newStatus = bStatus;          
        }

        public SaleReturn(string strCode,string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);            
        }

        public SaleReturn(string strCode, string strSNo, bool sStatus)
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
                        if ( btnAdd.Text == "&Add" && btnEdit.Text== "&Edit" && txtBillNo.Text != "")
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
                string strQuery = " Select SBillCode,GReturnCode,(Select ISNULL(MAX(BillNo),0) from SaleReturn Where BillCode=GReturnCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' "; 
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtSaleBillCode.Text =strSaleBillCode= Convert.ToString(dt.Rows[0]["SBillCode"]);
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["GReturnCode"]);
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleReturn Where BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleReturn Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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

                    string strQuery = " Select *,Convert(varchar,Date,103)BDate,Convert(varchar,SaleBillDate,103)SBDate,dbo.GetFullName(SalePartyID) SalesParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') SubParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SR.Date))) LockType,(Select TOP 1 SRD.PurchaseReturnStatus from SaleReturnDetails SRD Where SR.BillNo=SRD.BillNo and SR.BillCode=SR.BillCode and PurchaseReturnStatus=1)PReturn  from SaleReturn SR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                    + " Select SRD.*,(PurchasePartyID+' '+Name) PurchaseParty,Category from SaleReturnDetails SRD OUTER APPLY (Select Name,TINNumber as Category from SupplierMaster SM Where SRD.PurchasePartyID=(SM.AreaCode+SM.AccountNo)) SM Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID "
                                    + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    pnlDeletionConfirmation.Visible = false;
                    txtReason.Text = "";
                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            //pnlTax.Visible = true;                           
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
            grpQtr.Enabled = false;
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                txtBillCode.Text = Convert.ToString(row["BillCode"]);
                txtBillNo.Text = Convert.ToString(row["BillNo"]);
                txtDate.Text = Convert.ToString(row["BDate"]);
                strOldPartyName= txtSalesParty.Text = Convert.ToString(row["SalesParty"]);
                txtSubParty.Text = Convert.ToString(row["SubParty"]);
                txtSaleBillCode.Text = Convert.ToString(row["SaleBillCode"]);
                txtSaleBillNo.Text = Convert.ToString(row["SaleBillNo"]);
                txtSaleBillDate.Text = Convert.ToString(row["SBDate"]);
                txtSaleType.Text = Convert.ToString(row["SaleType"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtSignAmt.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);
                txtPacking.Text = Convert.ToString(row["PackingAmt"]);
                txtDiscountAmt.Text = Convert.ToString(row["NetDiscount"]);
                txtServiceAmt.Text = Convert.ToString(row["ServiceAmt"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                lblQty.Text = Convert.ToString(row["TotalQty"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                dOldNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                txtRoundOffSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOffAmt.Text = Convert.ToString(row["RoundOffAmt"]);

                if (txtRoundOffSign.Text == "")
                    txtRoundOffSign.Text = "+";
                if (txtRoundOffAmt.Text == "")
                    txtRoundOffAmt.Text = "0.00";


                if (Convert.ToString(row["EntryType"]) == "BYPURCHASE")
                    rdoByPurchaseSNo.Checked = true;
                else if (Convert.ToString(row["EntryType"]) == "MANUAL")
                    rdoManual.Checked = true;
                else
                    rdoAll.Checked = true;

                if (dt.Columns.Contains("IRNNO"))
                    txtIRNNo.Text = Convert.ToString(row["IRNNo"]);
                if (Convert.ToString(row["OtherValue"]) == "STOCK")
                    rdoStock.Checked = true;
                else if (Convert.ToString(row["OtherValue"]) == "NONE")
                    rdoNone.Checked = true;
                else
                    rdoPurchaseReturn.Checked = true;

                if (Convert.ToString(row["OtherText"]) == "PREVIOUS")
                    rdoOldYear.Checked = true;
                else
                    rdoCurrent.Checked = true;


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

                if (Convert.ToString(row["PReturn"]) != "")
                {
                    if (Convert.ToBoolean(row["PReturn"]) && !MainPage.strUserRole.Contains("SUPERADMIN"))
                        btnEdit.Enabled = btnDelete.Enabled = false;
                }
                if (Convert.ToBoolean(row["DiscountType"]))
                    rdoIncludeDisc.Checked = true;
                else
                    rdoExcludeDisc.Checked = true;

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
                    dgrdDetails.Rows[_index].Cells["purchaseSerialNo"].Value = row["PurchaseBillNo"];
                    dgrdDetails.Rows[_index].Cells["purchaseParty"].Value = row["PurchaseParty"];   
                    dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[_index].Cells["disStatus"].Value = row["DisStatus"];
                    dgrdDetails.Rows[_index].Cells["dis"].Value = row["Discount"];
                    dgrdDetails.Rows[_index].Cells["dhara"].Value = row["Dhara"];
                    dgrdDetails.Rows[_index].Cells["gQty"].Value = row["Qty"];
                    dgrdDetails.Rows[_index].Cells["gAmount"].Value = row["Amount"];
                    dgrdDetails.Rows[_index].Cells["gPacking"].Value = row["Packing"];
                    dgrdDetails.Rows[_index].Cells["gFreight"].Value = row["Freight"];
                    dgrdDetails.Rows[_index].Cells["gTax"].Value = row["TaxFree"];
                    dgrdDetails.Rows[_index].Cells["totalAmt"].Value = row["TotalAmt"];
                    dgrdDetails.Rows[_index].Cells["categoryName"].Value = row["Category"];

                    //if (Convert.ToString(row["DesignName"]) != "")
                        dgrdDetails.Rows[_index].Cells["designName"].Value = row["DesignName"];
                    //else
                    //    dgrdDetails.Rows[_index].Cells["designName"].Value = row["ItemName"];

                    _index++;
                }
                grpDiscountType.Enabled = false;


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
            txtIRNNo.ReadOnly = txtDate.ReadOnly = txtSaleBillDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly = txtTaxPer.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtIRNNo.ReadOnly = txtDate.ReadOnly = txtSaleBillDate.ReadOnly= txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly =txtTaxPer.ReadOnly= true;
        }

        private void ClearAllText()
        {
            txtIRNNo.Text= strOldPartyName = txtSalesParty.Text = txtSaleType.Text = txtSubParty.Text = txtSaleBillNo.Text = txtRemark.Text = strDeletedSID = lblMsg.Text = lblCreatedBy.Text = "";
            txtPacking.Text = txtDiscountAmt.Text = txtRoundOffAmt.Text = lblTaxableAmt.Text = txtTaxAmt.Text = txtServiceAmt.Text = txtOtherAmt.Text = "0.00";
            txtSaleBillCode.Text = strSaleBillCode;
            txtTaxPer.Text = "18.00";
            lblQty.Text = lblGrossAmt.Text = lblNetAmt.Text = "0.00";
            txtSignAmt.Text = "-";
            txtRoundOffSign.Text = "+";
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;
            grpDiscountType.Enabled = grpQtr.Enabled = true;
            rdoExcludeDisc.Checked = true;
            rdoCurrent.Checked =chkEmail.Checked= true;

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
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
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(GRBillNo)+1,1) from MaxSerialNo)BillNo  from [SaleReturn] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        int receiptNo = Convert.ToInt32(table.Rows[0]["SNo"]), maxReceiptNo = Convert.ToInt32(table.Rows[0]["BillNo"]);
                        if (receiptNo > maxReceiptNo)
                            txtBillNo.Text = Convert.ToString(receiptNo);
                        else
                            txtBillNo.Text = Convert.ToString(maxReceiptNo);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Set Bill No in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
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
                        string strData = objRead.ReadDataFromCard("SALESPARTY");
                        if (strData != "")
                        {
                            dgrdDetails.Rows.Clear();
                            txtSalesParty.Text = strData;
                            txtSubParty.Text = "SELF";
                            txtSaleBillNo.Text = "";                            
                        }
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
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
                                    dgrdDetails.Rows.Clear();
                                    txtSalesParty.Text = strData;
                                    txtSubParty.Text = "SELF";
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
            if (txtSaleBillDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid sale date  !!", "Sale Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSaleBillDate.Focus();
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
          
            if (MainPage._bTaxStatus)
            {
                if (txtSaleType.Text == "")
                {
                    MessageBox.Show("Sorry ! Sale Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSaleType.Focus();
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
         
            double dQty = 0,dAmt=0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(row.Cells["itemName"].Value), strDhara = Convert.ToString(row.Cells["dhara"].Value);
                string strDesign = Convert.ToString(row.Cells["designName"].Value);
                dQty = dba.ConvertObjectToDouble(row.Cells["gQty"].Value);
                dAmt=dba.ConvertObjectToDouble(row.Cells["totalAmt"].Value);
                if (strItem == "" && dQty == 0 && dAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    if (strDesign == "")
                    {
                        MessageBox.Show("Sorry ! Design name can't be blank", "Enter Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["designName"];
                        dgrdDetails.Focus();
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                        else
                            break;
                    }
                    //if (strDhara == "")
                    //{
                    //    MessageBox.Show("Sorry ! Dhara can't be blank", "Enter Dhara", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.CurrentCell = row.Cells["dhara"];
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}
                    //if (dQty == 0)
                    //{
                    //    MessageBox.Show("Sorry ! Quantity can't be blank", "Enter Qty party", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.CurrentCell = row.Cells["gQty"];
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}
                    if (dAmt == 0)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (txtIRNNo.Text != "")
            {
                double dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
                if (dOldNetAmt != dNetAmt)
                {
                    MessageBox.Show("E-Invoice has been generated, Please cancel EInvoice and remove IRN from this bill !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }            
            
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSaleType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
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

                            if (dba.ConvertObjectToDouble(dt.Rows[0]["BillDays"]) > 40 && !_bUpdateStatus)
                            {
                                DialogResult result = MessageBox.Show("Are you want to amend this bill for GSTR-1 if GSTR-1 has been filed ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    strAmendedQuery = " if not exists (Select [OBillCode] from [dbo].[AmendmentDetails] Where [OBillCode]='" + txtBillCode.Text + "' and [OBillNo]=" + txtBillNo.Text + " ) begin INSERT INTO [dbo].[AmendmentDetails]([BillType],[Date],[OBillCode],[OBillNo],[ODate],[ORBillCode],[ORBillNo],[ORDate],[Columnof1],[Columnof2],[Columnof3],[Columnof4],[Columnof5],[CreatedBy],[InsertStatus],[UpdateStatus]) Select 'SALERETURN' as BillType,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) as [Date],BillCode,BillNo,Date,SaleBillCode,SaleBillNo,NULL,(Select TOP 1 GSTNo from SupplierMaster SM Where (AreaCode+AccountNo)=SalePartyID) as GSTNo,'','','','','" + MainPage.strLoginName + "',1,0 from SaleReturn Where BillNo=" + txtBillNo.Text + " and BillCode='" + txtBillCode.Text + "'  end ";
                                }
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
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GRBillNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    {
                        int check = dba.CheckSaleReturnAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(BillNo)+1 from SaleReturn Where BillCode='" + txtBillCode.Text + "' "));
                            MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  Bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            chkStatus = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Bill No is already in used please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();
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

        private string GetEntryType()
        {
            if (rdoByPurchaseSNo.Checked)
                return "BYPURCHASE";
            else if (rdoManual.Checked)
                return "MANUAL";
            else
                return "ALL";
        }

        private string GetReturnType()
        {
            if (rdoPurchaseReturn.Checked)
                return "PURCHASERETURN";
            else if (rdoStock.Checked)
                return "STOCK";
            else
                return "NONE";
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
            btnAdd.Enabled = true ;
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT" ;
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strDesignName="", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "", strTaxAccountID="";
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
                if (rdoOldYear.Checked)
                    strQtrStatus = "PREVIOUS";

                double dAmt = 0, dQty = 0, dPacking = 0, dFreightAmt=0,dTaxFree=0, dTotalAmt = 0,dDis=0, _dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text), dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text),dNetAmt= dba.ConvertObjectToDouble(lblNetAmt.Text),_dOtherAmt= dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text),dPackingAmt= dba.ConvertObjectToDouble(txtPacking.Text),dServiceAmt= dba.ConvertObjectToDouble(txtServiceAmt.Text),dFinalAmt=0,_dOtherNetAmt=0;
                string strQuery = "",strReturnType= GetReturnType(),strStockQuery="", strItemName="",_strPurchasePartyName="";


                strQuery += " if not exists (Select BillCode from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[SaleReturn] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SaleBillCode],[SaleBillNo],[EntryType],[SaleType],[Remark],[OtherSign],[OtherAmt],[PackingAmt],[NetDiscount],[ServiceAmt],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleBillDate],[DiscountType],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[IRNNO]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSaleBillCode.Text + "','" + txtSaleBillNo.Text + "','" + GetEntryType() + "','" + txtSaleType.Text + "','" + txtRemark.Text + "','" + txtSignAmt.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," + dPackingAmt + "," +
                               +dba.ConvertObjectToDouble(txtDiscountAmt.Text) + "," + dServiceAmt + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dPTaxAmt + "," + dba.ConvertObjectToDouble(lblQty.Text) + "," + dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dNetAmt + ",'" + strQtrStatus + "','" + strReturnType + "','" + MainPage.strLoginName + "','',1,0,'" + strSDate + "','" + rdoIncludeDisc.Checked.ToString() + "','" + txtRoundOffSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text)+ ",'"+txtIRNNo.Text+"')  ";
                              
                
                double dRate = 0, dMRP=0,_dTotalQty = 0, _dNetPackingAmt=0;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    _dTotalQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTaxFree = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    _dNetPackingAmt = (dPacking + dFreightAmt);
                    dTotalAmt = (dAmt + dPacking + dFreightAmt + dTaxFree);
                    dDis = dba.ConvertObjectToDouble(rows.Cells["dis"].Value);

                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);
                    strPurchaseParty = Convert.ToString(rows.Cells["purchaseParty"].Value);
                    strDesignName = Convert.ToString(rows.Cells["designName"].Value).Trim();
                    strPurchasePartyID = "";
                    if (strPurchaseParty != "PERSONAL")
                    {
                        strFullName = strPurchaseParty.Split(' ');
                        if (strFullName.Length > 0)
                            strPurchasePartyID = strFullName[0].Trim();

                        _strPurchasePartyName = strPurchaseParty.Replace(strPurchasePartyID + " ", "");
                    }
                    else
                        _strPurchasePartyName = "";

                    strQuery += " INSERT INTO [dbo].[SaleReturnDetails] ([BillCode],[BillNo],[RemoteID],[PurchaseBillNo],[PurchasePartyID],[ItemName],[DesignName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus]) VALUES  "
                                    + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + rows.Cells["purchaseSerialNo"].Value + "','" + strPurchasePartyID + "','" + strItemName + "','" + strDesignName + "','" + rows.Cells["disStatus"].Value + "'," + dDis + ",'" + rows.Cells["dhara"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxFree + "," + dTotalAmt + ",0,'' ,1,0) ";

                    if (rdoStock.Checked)
                    {
                        if (Convert.ToString(rows.Cells["disStatus"].Value) == "-")
                            dDis = dDis * -1;                      
                        if (rdoIncludeDisc.Checked)
                        {
                            if (Convert.ToString(rows.Cells["categoryName"].Value) == "CASH PURCHASE")
                                dDis -= 5;
                            else
                                dDis -= 3;
                        }

                        if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate>Convert.ToDateTime("09/01/2019")))
                            dDis += 1;

                        if (dQty > 0)
                            dMRP = dAmt / dQty;
                        else
                            dMRP = dAmt;

                        dAmt = (dAmt * (100 + dDis)) / 100;

                        if (dQty > 0)
                            dRate = dAmt / dQty;
                        else
                            dRate = dAmt;

                        strStockQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                                       + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + strItemName + "','','','" + rows.Cells["purchaseSerialNo"].Value + "','" + _strPurchasePartyName + "',''," + dQty + "," + dRate + " ,'"+ dDis+"','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "') ";
                    }
                }

                _dOtherNetAmt = (_dOtherAmt + dPackingAmt + dServiceAmt);
                _dOtherNetAmt += ((_dOtherNetAmt + _dNetPackingAmt) * _dTaxPer) / 100;

                dFinalAmt = (dNetAmt - _dOtherNetAmt);


                strQuery += " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                         + " ('" + strDate + "','" + strSaleParty + "','SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dNetAmt + "','CR','" + dFinalAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "') ";

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";
                               
                if (dPTaxAmt > 0 && txtSaleType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName=TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSaleType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                                   + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value  + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";



                strQuery += strStockQuery+"  end";



                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    SendSMSToParty(strMobileNo);

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
                    strAmendedQuery = "";
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
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "", strTaxAccountID = "",strDeletedSIDQuery="";
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
                if (rdoOldYear.Checked)
                    strQtrStatus = "PREVIOUS";

                double dAmt = 0, dRate=0, dQty = 0,dMRP=0, dPacking = 0, dFreightAmt = 0, dTaxFree = 0, dTotalAmt = 0, dDis = 0,dTaxPer= dba.ConvertObjectToDouble(txtTaxPer.Text), dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text), dPackingAmt = dba.ConvertObjectToDouble(txtPacking.Text), dServiceAmt = dba.ConvertObjectToDouble(txtServiceAmt.Text), dFinalAmt = 0,_dOtherNetAmt=0,_dNetPackingAmt=0;

                string strQuery = "",strID="", strReturnType=GetReturnType(), strStockQuery="", _strPurchasePartyName="", strItemName="", strDesignName="";
           
                strQuery += strAmendedQuery;

                strQuery += " UPDATE  [dbo].[SaleReturn]  SET [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SaleBillCode]='" + txtSaleBillCode.Text + "',[SaleBillNo]='" + txtSaleBillNo.Text + "',[EntryType]='" + GetEntryType() + "',[SaleType]='" + txtSaleType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSignAmt.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[PackingAmt]=" + dPackingAmt + ",[OtherText]='" + strQtrStatus+ "',[OtherValue]='" + strReturnType+"', "
                         + "[RoundOffSign]='" + txtRoundOffSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ", [NetDiscount]=" + dba.ConvertObjectToDouble(txtDiscountAmt.Text) + ",[ServiceAmt]=" + dServiceAmt+ ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dNetAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SaleBillDate]='"+ strSDate+"',[DiscountType]='"+rdoIncludeDisc.Checked.ToString()+ "',[IRNNO]='"+txtIRNNo.Text+"' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " Delete from StockMaster Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";
                

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTaxFree = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    dTotalAmt = (dAmt + dPacking + dFreightAmt + dTaxFree);
                    dDis = dba.ConvertObjectToDouble(rows.Cells["dis"].Value);

                    _dNetPackingAmt += (dPacking + dFreightAmt);

                    strID = Convert.ToString(rows.Cells["sid"].Value);
                    strPurchaseParty = Convert.ToString(rows.Cells["purchaseParty"].Value);
                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);
                    strDesignName = Convert.ToString(rows.Cells["designName"].Value).Trim();

                    strPurchasePartyID = "";
                    if (strPurchaseParty != "PERSONAL")
                    {
                        strFullName = strPurchaseParty.Split(' ');
                        if (strFullName.Length > 0)
                            strPurchasePartyID = strFullName[0].Trim();
                        _strPurchasePartyName = strPurchaseParty.Replace(strPurchasePartyID + " ", "");
                    }

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SaleReturnDetails] ([BillCode],[BillNo],[RemoteID],[PurchaseBillNo],[PurchasePartyID],[ItemName],[DesignName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus]) VALUES  "
                                        + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + rows.Cells["purchaseSerialNo"].Value + "','" + strPurchasePartyID + "','" + strItemName + "','" + strDesignName + "','" + rows.Cells["disStatus"].Value + "'," + dDis + ",'" + rows.Cells["dhara"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxFree + "," + dTotalAmt + ",0,'' ,1,0) ";
                    }
                    else
                    {
                        strQuery += "Update [dbo].[SaleReturnDetails] SET [PurchaseBillNo]='" + rows.Cells["purchaseSerialNo"].Value + "',[PurchasePartyID]='" + strPurchasePartyID + "',[ItemName]='" + strItemName + "',[DesignName]='" + strDesignName + "',[DisStatus]='" + rows.Cells["disStatus"].Value + "',[Discount]=" + dDis + ",[Dhara]='" + rows.Cells["dhara"].Value + "',[Qty]=" + dQty + ",[Amount]=" + dAmt + ",[Packing]=" + dPacking + ",[Freight]=" + dFreightAmt + " ,[TaxFree]=" + dTaxFree + ",[TotalAmt]=" + dTotalAmt + ",[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + " ";
                    }

                    if (rdoStock.Checked)
                    {
                        if (Convert.ToString(rows.Cells["disStatus"].Value) == "-")
                            dDis = dDis * -1;
                        //if (rdoExcludeDisc.Checked)
                        //{
                        //    if (Convert.ToString(rows.Cells["categoryName"].Value) == "CASH PURCHASE")
                        //        dDis += 5;
                        //    else
                        //        dDis += 3;
                        //}
                        if (rdoIncludeDisc.Checked)
                        {
                            if (Convert.ToString(rows.Cells["categoryName"].Value) == "CASH PURCHASE")
                                dDis -= 5;
                            else
                                dDis -= 3;
                        }
                        if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                            dDis += 1;

                        if (dQty > 0)
                            dMRP = dAmt / dQty;
                        else
                            dMRP = dAmt;

                        dAmt = (dAmt * (100 + dDis)) / 100;

                        if (dQty > 0)
                            dRate = dAmt / dQty;
                        else
                            dRate = dAmt;


                        strStockQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                                       + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + strItemName + "','','','" + rows.Cells["purchaseSerialNo"].Value + "','" + _strPurchasePartyName + "',''," + dQty + "," + dRate + " ,'" + dDis + "','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "') ";
                    }
                }

                _dOtherNetAmt = (_dOtherAmt + dPackingAmt + dServiceAmt);
                _dOtherNetAmt += ((_dOtherNetAmt+_dNetPackingAmt) * dTaxPer) / 100;

                dFinalAmt = (dNetAmt - _dOtherNetAmt);

                strQuery += " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]='" + dNetAmt + "',[FinalAmount]='" + dFinalAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strSalePartyID + "' Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' ";
                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtSaleType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSaleType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end ";
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
                                   + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                object objValue = "True";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[SaleReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strDeletedSID + ") ";
                    strDeletedSIDQuery = " Delete from [dbo].[SaleReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";

                    objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                       + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";



                strQuery += strStockQuery+ " end ";      

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (Convert.ToString(objValue) != "" && strDeletedSIDQuery!="")
                    {
                        if (!Convert.ToBoolean(objValue))
                        {                           
                            DataBaseAccess.CreateDeleteQuery(strDeletedSIDQuery);
                        }
                    }

                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    SendSMSToParty(strMobileNo);

                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
              
                    btnEdit.Text = "&Edit";
                    updateStatus = true;
                    strDeletedSIDQuery = strDeletedSID = "";
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
                            int check = dba.CheckSaleReturnAvailability(txtBillCode.Text, txtBillNo.Text);
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
                string[] strReport = { "Error occurred in Check  Availability in Sale Return ", ex.Message };
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
                    else if (btnAdd.Text=="&Add" && btnEdit.Text == "&Edit")
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
                        btnEdit.Enabled = btnDelete.Enabled =  false;
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
                    if (cIndex == 7)
                        dba.KeyHandlerPoint(sender, e, 2);
                    else
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
                    if(value == null || value.ToString() == "")
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
                dDiscountAmt += ((dAmt * dDis) / 100) + ((dPacking * MainPage.dPackingDhara) / 100) + (((dFreightAmt) * MainPage.dFreightDhara) / 100);

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
            double dServiceAmt=0, dDiscount = 0,dPackingAmt=0, dOtherAmt = 0, dRoundOffAmt = 0, dTaxableAmt = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dTaxAmt = 0, dFinalAmt = 0;
            try
            {

             //   dDisPer = dba.ConvertObjectToDouble(txtDisPer.Text);       
       
                dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);
                dDiscount = dba.ConvertObjectToDouble(txtDiscountAmt.Text);
                dPackingAmt = dba.ConvertObjectToDouble(txtPacking.Text);

                dTOAmt = dOtherAmt + dPackingAmt;
                dFinalAmt = dGrossAmt + dDiscount + dTOAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt,ref dTaxableAmt);
                dServiceAmt = dba.ConvertObjectToDouble(txtServiceAmt.Text);

                dNetAmt = dGrossAmt + dDiscount + dOtherAmt + dPackingAmt + dTaxAmt + dServiceAmt;

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
            double dTaxAmt = 0,  dTaxPer = 0,dServiceAmt=0;
            string _strTaxType = "";
            try
            {
                if (MainPage._bTaxStatus && txtSaleType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);
                    if (MainPage.startFinDate >= Convert.ToDateTime("04/01/2021"))
                        dTaxPer = 18;

                    dgrdTax.Rows.Clear();
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

                            string strQuery = "",  strServiceQuery="",strItemName="";
                            DateTime sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);

                            double dDisStatus = 0, dPackingDhara = 3;
                            if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                                dPackingDhara = 2;
                            double dAmt = 0, dQty = 0, dPacking = 0,_dOtherAmt=0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                                dDisStatus = dba.ConvertObjectToDouble(rows.Cells["disStatus"].Value + "" + rows.Cells["dis"].Value);

                                dPacking += dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value) + dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);// + dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                                if (dAmt > 0)
                                {
                                    strItemName =Convert.ToString(rows.Cells["itemName"].Value );

                                    if (strQuery != "")
                                    {
                                        strQuery += " UNION ALL ";
                                        strServiceQuery += " UNION ALL ";
                                    }

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 + " + dDisStatus + "))/ 100.00),2)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + " * 100) / (100 + TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/ " + dQty + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + "* 100) / (100 + TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dAmt + ">0  ";
                                    strServiceQuery += " Select (SUM(CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+GM.TaxRate)) else " + dAmt + " end)  *(100 + " + dDisStatus + ")/ 100.00)  as Amount,'" + strItemName + "' as ItemName," + dQty + " Quantity from Items _IM Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((" + dAmt + "*100)/(100+TaxRate)) else " + dAmt + " end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='') as GM  Where _IM.ItemName='" + strItemName + "' ";
                                }
                            }

                            if (strQuery != "")
                            {
                                if (dPacking != 0)
                                    dPacking = (dPacking * (100 + dPackingDhara) / 100.00);

                                dPacking += dOtherAmt;
                                if (dPacking != 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,"+dTaxPer+" as TaxRate ";
                                }
                                if (strQuery != "")
                                {
                                    strQuery = "  Select SUM(TaxableAmt)TaxableAmt,SUM(Amt)Amt,SUM(ROUND(Amt,2))TaxAmt,TaxRate,0 as ServiceAmt from ("
                                                   + " Select *,0 ServiceTax from ( "
                                                   + " Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate,MAX(TaxRate) OVER(PARTITION BY ID)  MTaxRate from ( Select 1 as ID,HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                                   + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate,ID,HSNCode )_Goods "
                                                   + "  )_FinalSales Group by TaxRate ";


                                    DataTable dt = dba.GetDataTable(strQuery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        double dMaxRate = 0, dTTaxAmt = 0;
                                        //BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                        dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                        dTaxAmt = dTTaxAmt;
                                        if (dPacking == 0 || dTaxPer==0)
                                            dTaxPer = dMaxRate;
                                        dServiceAmt = dba.ConvertObjectToDouble(dt.Rows[0]["ServiceAmt"]);
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
                            txtServiceAmt.Text = dServiceAmt.ToString("N2", MainPage.indianCurancy);
                            //pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text = txtServiceAmt.Text = "0.00";
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
            txtServiceAmt.Text = dServiceAmt.ToString("N2", MainPage.indianCurancy);

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
        //                dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);
        //                if (dTaxRate > dMaxRate)
        //                    dMaxRate = dTaxRate;
        //                dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);

        //                dgrdTax.Rows[_index].Cells["taxName"].Value = strIGST;
        //                dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;

        //                if (strRegion == "LOCAL")
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
        //                    _index++;
        //                    dgrdTax.Rows[_index].Cells["taxName"].Value = strSGST;
        //                    dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
        //                }
        //                else
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = dTaxRate.ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = dTaxAmt.ToString("N2", MainPage.indianCurancy);
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
                        if (rdoManual.Checked)
                        {
                            if (e.ColumnIndex == 1)
                            {
                                SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                dgrdDetails.CurrentRow.Cells["dis"].Value = dgrdDetails.CurrentRow.Cells["disStatus"].Value = dgrdDetails.CurrentRow.Cells["dhara"].Value = "";
                            }
                            else if (e.ColumnIndex == 2)
                            {
                                SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strItem = objSearch.strSelectedData.Split('|');
                                    if (strItem.Length > 0)
                                    {                                       
                                        dgrdDetails.CurrentCell.Value = strItem[0];
                                        if (Convert.ToString(dgrdDetails.CurrentRow.Cells["designName"].Value) == "")
                                        {
                                            strItem = strItem[0].Split(':');
                                            dgrdDetails.CurrentRow.Cells["designName"].Value = strItem[0].Trim();
                                        }
                                    }
                                }
                                CalculateTotalAmount();
                            }
                        }
                        else
                        {
                            string strQuery = "";
                            if (txtSaleBillNo.Text != "" && txtSaleBillNo.Text != "0")
                                strQuery += " and GRSNo in (Select SE.GRSNo from SalesEntry SE Where (SE.BillCode+CAST(SE.BillNo as varchar))='" + txtSaleBillCode.Text + txtSaleBillNo.Text + "')  ";
                            if (txtSalesParty.Text != "")
                            {
                                string[] strFullName = txtSalesParty.Text.Split(' ');
                                if (strFullName.Length > 1)
                                    strQuery += " and SalePartyID ='" + strFullName[0].Trim() + "'  ";
                            }

                            if (rdoOldYear.Checked)
                            {
                                SearchDataOnOld objSearch = new SearchDataOnOld("PURCHASEBILLDETAIL", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space,true);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strItem = objSearch.strSelectedData.Split('|');
                                    if (strItem.Length > 0)
                                        SetSelectedDetails(strItem);
                                    CalculateTotalAmount();
                                }
                            }
                            else
                            {
                                SearchData objSearch = new SearchData("PURCHASEBILLDETAIL", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    string[] strItem = objSearch.strSelectedData.Split('|');
                                    if (strItem.Length > 0)
                                        SetSelectedDetails(strItem);
                                    CalculateTotalAmount();
                                }
                            }
                            e.Cancel = true;
                        }
                        if(e.ColumnIndex != 3)
                        e.Cancel = true;
                    }
                    else if (rdoManual.Checked && e.ColumnIndex == 6)
                    {
                        SearchData objSearch = new SearchData("DHARA", "SEARCH DHARA TYPE", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            GetDharaDetails(dgrdDetails.CurrentRow);                           
                                grpDiscountType.Enabled = false;
                        }
                        CalculateTotalAmount();
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 12)
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
            string strParty = Convert.ToString(row.Cells["purchaseParty"].Value), strDType = Convert.ToString(row.Cells["dhara"].Value), strDhara = "", strQuery = "";
            if (strDType == "NORMAL")
                strQuery = " Select NormalDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";
            else if (strDType == "SUPER")
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
            double _dPer = 3;
            if (strDhara != "")
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);

                if (strCategory.ToUpper() == "CASH PURCHASE")
                    _dPer = 5;
                if (rdoExcludeDisc.Checked)
                    _dPer = 0;

                if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                    _dPer -= 1;

                double dDhara = dba.ConvertObjectToDouble(strDhara);

                dDhara = (dDhara * -1) + _dPer;
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

                dgrdDetails.Rows[rowIndex].Cells["categoryName"].Value = strCategory.ToUpper();

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

        private void SetSelectedDetails(string[] strDetails)
        {
            try
            {

                dgrdDetails.CurrentRow.Cells["purchaseSerialNo"].Value = strDetails[2];
                dgrdDetails.CurrentRow.Cells["purchaseParty"].Value = strDetails[1];
                dgrdDetails.CurrentRow.Cells["itemName"].Value = strDetails[0];
                dgrdDetails.CurrentRow.Cells["designName"].Value = strDetails[7];
                dgrdDetails.CurrentRow.Cells["dhara"].Value = strDetails[5];
                dgrdDetails.CurrentRow.Cells["disStatus"].Value = strDetails[3];
                dgrdDetails.CurrentRow.Cells["dis"].Value = strDetails[4];
                dgrdDetails.CurrentRow.Cells["categoryName"].Value = strDetails[6].ToUpper();

                if (rdoIncludeDisc.Checked)
                {
                    dgrdDetails.CurrentRow.Cells["disStatus"].Value = strDetails[3];
                    dgrdDetails.CurrentRow.Cells["dis"].Value = strDetails[4];
                }
                else
                {
                    string strCategory = strDetails[6];
                    double dDis = dba.ConvertObjectToDouble(strDetails[3] + strDetails[4]);
                    DateTime sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);

                    if (strCategory.ToUpper() == "CASH PURCHASE")
                        dDis -= 5;
                    else
                        dDis -= 3;

                    if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                        dDis += 1;

                    if (dDis >= 0)
                    {
                        dgrdDetails.CurrentRow.Cells["disStatus"].Value = "+";
                        dgrdDetails.CurrentRow.Cells["dis"].Value = dDis;
                    }
                    else
                    {
                        dgrdDetails.CurrentRow.Cells["disStatus"].Value = "-";
                        dgrdDetails.CurrentRow.Cells["dis"].Value = Math.Abs(dDis);
                    }
                }
                grpDiscountType.Enabled = false;

            }
            catch
            {
            }
        }

        private void txtSaleBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && (!rdoManual.Checked || rdoOldYear.Checked))
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strQuery = "";
                        if (txtSalesParty.Text != "")
                        {
                            string[] strFullName = txtSalesParty.Text.Split(' ');
                            if (strFullName.Length > 1)
                                strQuery = " Where SalePartyID ='" + strFullName[0].Trim() + "'  ";
                        }
                        if (rdoOldYear.Checked)
                        {
                            SearchDataOnOld objSearch = new SearchDataOnOld("SALEBILLNOFORRETURN", strQuery, "SEARCH SALE BILL NO", e.KeyCode,true);
                            objSearch.ShowDialog();
                            string[] strBillNo = objSearch.strSelectedData.Split('|');
                            txtSaleBillCode.Text = strBillNo[0];
                            if (strBillNo.Length > 1)
                            {
                                txtSaleBillNo.Text = strBillNo[1];
                                txtSaleBillDate.Text = strBillNo[2];
                            }
                        }
                        else
                        {
                            SearchData objSearch = new SearchData("SALEBILLNOFORRETURN", strQuery, "SEARCH SALE BILL NO", e.KeyCode);
                            objSearch.ShowDialog();
                            string[] strBillNo = objSearch.strSelectedData.Split('|');
                            txtSaleBillNo.Text = strBillNo[0];
                            if (strBillNo.Length > 1)
                                txtSaleBillDate.Text = strBillNo[1];
                            if(btnAdd.Text == "&Save")
                            {
                                dgrdDetails.Rows.Clear();
                                dgrdDetails.Rows.Add();
                                rdoByPurchaseSNo.Checked = true;
                            }
                        }
                        
                        if (dgrdDetails.Rows.Count == 0)
                            dgrdDetails.Rows.Add();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SetDeletedID()
        {
            if (btnEdit.Text == "&Update")
            {
                string strID = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strID = Convert.ToString(row.Cells["sid"].Value);
                    if (strID != "")
                    {
                        if (strDeletedSID != "")
                            strDeletedSID += ",";
                        strDeletedSID += strID;
                    }
                }
            }
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoAll.Checked && btnAdd.Text == "&Save")
                {
                    GetSaleReturnBillDetails();
                }
            }
            catch
            {
            }
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (rdoManual.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = false;
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                    grpDiscountType.Enabled = true;
                }
                else if (!rdoOldYear.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = true;
                    txtSaleBillCode.Text = strSaleBillCode;
                    grpDiscountType.Enabled = true;
                }
            }
        }

        private void GetSaleReturnBillDetails()
        {
            string strQuery = "";
            if (txtBillNo.Text != "")
            {
                dgrdDetails.Rows.Clear();
                DataTable dt = null;
                DateTime sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);

                strQuery = " Select  (BillCode+' '+CAST(BillNo as varchar)) as BillNo,(PurchasePartyID+' '+SM.Name)PurchaseParty,GR.ItemName,GR.DesignName,PR.DiscountStatus,PR.Discount,PR.Dhara,GR.Quantity as Qty,GR.Amount,GR.PackingAmt,GR.FreightAmt,GR.TaxAmt,(GR.Amount + GR.PackingAmt+GR.FreightAmt+GR.TaxAmt)TotalAmt,SM.Category  from PurchaseRecord PR Left join GoodsReceiveDetails GR on PR.GRSNO=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))  OUTER APPLY (Select TOP 1 Name,Category from SupplierMaster Where (AreaCode+AccountNo)=PR.PurchasePartyID)SM  Where  GRSNo in (Select SE.GRSNo from SalesEntry SE Where (SE.BillCode+CAST(SE.BillNo as varchar))='" + txtSaleBillCode.Text + txtSaleBillNo.Text + "')   Order by PR.BillNo ";
                if (rdoOldYear.Checked)
                {
                    SearchDataOnOld objSearch = new SSS.SearchDataOnOld(true);
                    dt = objSearch.GetDataTable(strQuery);
                }
                else
                    dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["purchaseSerialNo"].Value = row["BillNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["purchaseParty"].Value = row["PurchaseParty"];
                        dgrdDetails.Rows[_rowIndex].Cells["itemName"].Value = row["ItemName"];
                        dgrdDetails.Rows[_rowIndex].Cells["designName"].Value = row["DesignName"];
                        dgrdDetails.Rows[_rowIndex].Cells["dhara"].Value = row["Dhara"];
                        dgrdDetails.Rows[_rowIndex].Cells["gQty"].Value = row["Qty"];
                        dgrdDetails.Rows[_rowIndex].Cells["gAmount"].Value = row["Amount"];
                        dgrdDetails.Rows[_rowIndex].Cells["gPacking"].Value = row["PackingAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["gFreight"].Value = row["FreightAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["gTax"].Value = row["TaxAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["totalAmt"].Value = row["TotalAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["categoryName"].Value = row["Category"];

                        if (rdoIncludeDisc.Checked)
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = row["DiscountStatus"];
                            dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = row["Discount"];
                        }
                        else
                        {
                            string strCategory = Convert.ToString(row["Category"]);

                            double dDis = dba.ConvertObjectToDouble(row["DiscountStatus"] + "" + row["Discount"]);
                            if (strCategory.ToUpper() == "CASH PURCHASE")
                                dDis -= 5;
                            else
                                dDis -= 3;

                            if (txtBillCode.Text.Contains("SRT") || (txtBillCode.Text.Contains("CCK") && sDate > Convert.ToDateTime("09/01/2019")))
                                dDis += 1;

                            if (dDis >= 0)
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = "+";
                                dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = dDis;
                            }
                            else
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["disStatus"].Value = "-";
                                dgrdDetails.Rows[_rowIndex].Cells["dis"].Value = Math.Abs(dDis);
                            }
                        }
                        _rowIndex++;
                        grpDiscountType.Enabled = false;

                    }
                }
                CalculateTotalAmount();
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
                string[] strReport = { "Exception occurred in Preview  in Sales Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (txtSaleType.Text != "")
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

        private void txtSaleBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtSaleBillCode.ReadOnly)
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSaleBillCode.Text = objSearch.strSelectedData;
                    }
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
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
                        SearchData objSearch = new SearchData("SALERETURNCODE", "SEARCH SALE RETURN CODE", e.KeyCode);
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

        private void txtSaleBillDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtSaleBillDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, false, false);
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnEdit.Text == "&Update")
                e.Handled = true;
            else
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALERETURN", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void rdoOldYear_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (rdoOldYear.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = false;
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                    grpDiscountType.Enabled = true;
                }
                else if (!rdoManual.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = true;
                    txtSaleBillCode.Text = strSaleBillCode;
                    grpDiscountType.Enabled = true;
                }
            }
        }

        private void rdoCurrent_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoExcludeDisc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoIncludeDisc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;

                DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    bool Created = false;
                    string strFileName = CreatePDFFile(true, ref Created);
                    if (Created)
                        MessageBox.Show("Thank you ! PDF generated on " + strFileName, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && dba.ValidateBackDateEntry(txtDate.Text))
                {
                    if (txtReason.Text != "" && ValidateOtherValidation(true))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strQuery = "";

                            strQuery += " Delete from [SaleReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                     + " Delete from [SaleReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('SALE RETURN','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " Delete from StockMaster Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from SaleReturn Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");

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
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtOtherAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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

                    if (IndexColmn < dgrdDetails.ColumnCount - 3)
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
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        grpDiscountType.Enabled = grpQtr.Enabled = true;
                        dgrdDetails.Rows.Add();
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
                if (rdoByPurchaseSNo.Checked && btnAdd.Text == "&Save")
                {
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();
                    grpDiscountType.Enabled = true;
                }
            }
            catch
            {
            }
        }

        private void txtTaxPer_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
        }

        private bool GSTPrintAndPreview(bool _pstatus, string strPath)
        {           

            DataTable _dtGST = null, _dtSalesAmt = null; 
            bool _bIGST = false;
            DataTable dt = dba.CreateDebitNoteDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt,"CREDIT NOTE", "CREDIT NOTE");
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
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    return true;
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        if (strValue != "" && strValue != "0")
                        //        {
                        //            int nCopy = Int32.Parse(strValue);
                        //            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                        //        objReport.myPreview.ReportSource = objOL_salebill;
                        //        objReport.ShowDialog();
                        //    }
                        //}
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
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    return true;
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        if (strValue != "" && strValue != "0")
                        //        {
                        //            int nCopy = Int32.Parse(strValue);
                        //            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                        //        objReport.myPreview.ReportSource = objOL_salebill;
                        //        objReport.ShowDialog();
                        //    }
                        //}
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
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    return true;
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        if (strValue != "" && strValue != "0")
                        //        {
                        //            int nCopy = Int32.Parse(strValue);
                        //            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                        //        objReport.myPreview.ReportSource = objOL_salebill;
                        //        objReport.ShowDialog();
                        //    }
                        //}
                    }
                    else
                    {
                        Reporting.DCNoteReport_IGST_Retail objOL_salebill = new Reporting.DCNoteReport_IGST_Retail();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus,objOL_salebill,strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    return true;
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        if (strValue != "" && strValue != "0")
                        //        {
                        //            int nCopy = Int32.Parse(strValue);
                        //            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                        //        objReport.myPreview.ReportSource = objOL_salebill;
                        //        objReport.ShowDialog();
                        //    }
                        //}
                    }
                }
            }
            return false;
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
                                var _success = dba.GenerateEInvoiceJSON_SaleBook(true,strBillNo, "CREDITNOTE","CRN");
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
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnEInvoice.Enabled = true;
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

        private void FinallyPrint(bool _pstatus, CrystalDecisions.CrystalReports.Engine.ReportClass objReport, string strPath)
        {
            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = (short)MainPage.iNCopySaleRtn;
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;

            if (strPath != "")
            {
                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
            }
            else
            {
                if (_pstatus)
                {
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport, false, MainPage.iNCopySaleRtn);
                    else
                    {
                        // string strValue = "0";
                        if (_pstatus)
                        {
                            //   strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                            //if (strValue != "" && strValue != "0")
                            //{
                            //  int nCopy = Int32.Parse(strValue);
                            objReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            //}
                        }
                    }
                }
                else
                {
                    Reporting.ShowReport report = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                    report.myPreview.ReportSource = objReport;
                    report.ShowDialog();
                }
            }
            objReport.Close();
            objReport.Dispose();
        }

        private string CreatePDFFile(bool _createPDF, ref bool Created)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
            {
                //string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
                //strFileName = strNewPath + "\\" + txtBillNo.Text + ".pdf";
                //if (File.Exists(strFileName))
                //    File.Delete(strFileName);
                //Directory.CreateDirectory(strNewPath);

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
                    string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SaleReturn\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    _strPath += "\\" + _strFileName;

                    strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                    if (File.Exists(strPath))
                        File.Delete(strPath);
                    Directory.CreateDirectory(_strPath);
                    Created = true;
                }

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strPath);
                    if (_bstatus)
                    {
                        Created = true;
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
                    bool Created = false;
                    string strPath = CreatePDFFile(false,ref Created), strEmailID = "", strWhatsAppNo = "";
                    if (strPath != "")
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
            string _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text;
            if (!strBranchCode.Contains("-"))
                strBranchCode = "18-19/" + strBranchCode;
            string strWhastappMessage = "", strMsgType = "",strMType="";
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
            if (btnEdit.Text == "&Update")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                strMessage = "M/S " + strName + ", credit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " UPDATED.";
                strMType = "creditnote_generation";
                strMsgType = "credit_note";
            }
            else
            {
                strMessage = "M/S " + strName + ", new credit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " CREATED.";
                strMType = "creditnote_update";
                strMsgType = "credit_note_update_pdf";
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
                //string strResult = WhatsappClass.SendWhatsAppMessage(strMobileNo, strMessage, strFilePath, "CREDITNOTE", "", "PDF");
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                strWhastappMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + lblNetAmt.Text + "\"}";
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
                        strMessage = "M/S : " + txtSalesParty.Text + " , we have created your credit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "M/S : " + txtSalesParty.Text + ", we have updated your credit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
                    }

                    if (btnAdd.Text == "&Save")
                        strSub = "Credit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " created.";
                    else
                        strSub = "Alert ! Credit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " updated.";
                }
                else
                {
                    strMessage = " Alert ! Credit note bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + "</b> is Deleted by : " + MainPage.strLoginName + "  and  the deleted credit note bill is attached with this mail. ";
                    strSub = "Alert ! Credit note bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " deleted by : " + MainPage.strLoginName;
                }

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "CREDIT NOTE", true);
                if (billStatus == 0 && bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
        }

        private void SendSMSToParty(string strMobileNo)
        {
            try
            {                
                if (chkSendSMS.Checked)
                {
                    if (strMobileNo == "")
                        strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtSalesParty.Text));

                    string strName = dba.GetSafePartyName(txtSalesParty.Text);
                    if (strMobileNo != "")
                    {
                        SendSMS objSMS = new SendSMS();                      
                        string strMessage = "";

                        if (btnAdd.Text == "&Save")
                            strMessage = "M/s " + strName + ", Credit note created with bill no : " + txtBillCode.Text + " " + txtBillNo.Text + ", DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblQty.Text;
                        else
                            strMessage = "M/s " + strName + ", Credit note no : " + txtBillCode.Text + " " + txtBillNo.Text + " updated with dated : " + txtDate.Text + ", AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblQty.Text;

                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
