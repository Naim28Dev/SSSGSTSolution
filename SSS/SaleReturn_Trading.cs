using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class SaleReturn_Trading : Form
    {
        DataBaseAccess dba;
        SendSMS objSMS;
        string strLastSerialNo = "", strDeletedSID = "", strSaleBillCode="", strOldPartyName="";
        bool qtyAdjustStatus = false;
        public bool saleStatus = false, updateStatus = false, newStatus = false;
        double dOldNetAmt = 0;

        public SaleReturn_Trading()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            GetStartupData(true);
        }

        public SaleReturn_Trading(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();         
            GetStartupData(false);

            if (strSerialCode != "")
                txtBillCode.Text = strSerialCode;
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
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
                            txtSaleBillCode.Text = strSaleBillCode = Convert.ToString(dt.Rows[0]["SBillCode"]);
                            txtBillCode.Text = Convert.ToString(dt.Rows[0]["GReturnCode"]);
                            strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                        }

                        if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                            BindRecordWithControl(strLastSerialNo);

                    }
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Sale Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant2"].Visible = false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["variant3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["variant4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["variant5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant5"].Visible = false;
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

                    string strQuery = "  Select *,Convert(varchar,Date,103)BDate,Convert(varchar,SaleBillDate,103)SBDate,dbo.GetFullName(SalePartyID) SalesParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') SubParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SR.Date))) LockType,(Select TOP 1 SRD.PurchaseReturnStatus from SaleReturnDetails SRD Where SR.BillNo=SRD.BillNo and SR.BillCode=SR.BillCode and PurchaseReturnStatus=1)PReturn  from SaleReturn SR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                             + " Select *,dbo.GetFullName(PurchasePartyID) PurchaseParty from SaleReturnDetails Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID "
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
                            BindSaleReturnDetails(ds.Tables[1]);
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
                strOldPartyName = txtSalesParty.Text = Convert.ToString(row["SalesParty"]);
                txtSubParty.Text = Convert.ToString(row["SubParty"]);
                txtSaleBillCode.Text = Convert.ToString(row["SaleBillCode"]);
                txtSaleBillNo.Text = Convert.ToString(row["SaleBillNo"]);
                txtSaleBillDate.Text = Convert.ToString(row["SBDate"]);
                txtSalesType.Text = Convert.ToString(row["SaleType"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtSignAmt.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);
                txtPacking.Text = Convert.ToString(row["PackingAmt"]);
                txtInsuranceAmt.Text = Convert.ToString(row["NetDiscount"]);
                txtInsurancePer.Text = Convert.ToString(row["OtherValue"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                lblQty.Text = Convert.ToString(row["TotalQty"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                dOldNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);

                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";

                if (dt.Columns.Contains("IRNNO"))
                    txtIRNNo.Text = Convert.ToString(row["IRNNo"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                //if (Convert.ToString(row["EntryType"]) == "BYPURCHASE")
                //    rdoByPurchaseSNo.Checked = true;
                if (Convert.ToString(row["EntryType"]) == "MANUAL")
                    rdoManual.Checked = true;
                else
                    rdoAll.Checked = true;

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
                    if (Convert.ToBoolean(row["PReturn"]))
                        btnEdit.Enabled = btnDelete.Enabled = false;
                }
               
                txtBillNo.ReadOnly = false;
            }
        }

        private void BindSaleReturnDetails(DataTable _dtDetails)
        {
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Clear();
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = row["PurchaseBillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = ConvertObjectToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = ConvertObjectToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                    dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["Packing"];
                    dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                    rowIndex++;
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
                    dgrdTax.Rows[rowIndex].Cells["taxType"].Value = row["taxType"];

                    rowIndex++;
                }
               // pnlTax.Visible = true;
            }
           // else
               // pnlTax.Visible = false;
        }

        private void BindRecordWithControl_Import()
        {
            try
            {
                if (txtImportData.Text != "")
                {

                    string strQuery = " Select * from SaleReturnDetails Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'  order by SID ";

                    DataTable _dtDetails = SearchDataOther.GetDataTable_NC(strQuery);
                    if (_dtDetails.Rows.Count > 0)
                    {
                        if (_dtDetails.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                            int rowIndex = 0;
                            foreach (DataRow row in _dtDetails.Rows)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                                dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = row["PurchaseBillNo"];
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                                dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                                dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                                dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                                dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                                dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                                dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                                dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = ConvertObjectToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["rate"].Value = ConvertObjectToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["amount"].Value = ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                                dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["Packing"];
                                dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                                rowIndex++;
                            }
                        }
                    }

                }
                CalculateAllAmount();
            }
            catch
            {
            }
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (txtSaleBillNo.Text=="")
                        {
                            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                txtSalesParty.Text = objSearch.strSelectedData;
                                string strData = objSearch.strSelectedData;
                                if (strData != "")
                                {
                                    bool _blackListed = false;
                                    if (dba.CheckTransactionLockWithBlackList(txtSalesParty.Text, ref _blackListed))
                                    {
                                        MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        txtSalesParty.Text = "";
                                    }
                                    else if (_blackListed)
                                    {
                                        MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        txtSalesParty.Text = "";
                                    }
                                    else
                                    {
                                        txtSalesParty.Text = strData;
                                        txtSubParty.Text = "SELF";
                                    }
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }        
       
        private void SaleBook_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bSaleView)
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
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
            }
        }  

        private void EnableAllControls()
        {
            txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly = txtInsurancePer.ReadOnly=txtTaxPer.ReadOnly= false;// txtTaxPer.ReadOnly=
            dgrdDetails.ReadOnly =  false;
            grpQtr.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly = txtTaxPer.ReadOnly = txtInsurancePer.ReadOnly = true;
            dgrdDetails.ReadOnly =true;          
            lblMsg.Text =lblCreatedBy.Text= "";
        }

        private void ClearAllText()
        {
            txtIRNNo.Text = txtSalesParty.Text = txtSalesType.Text = txtSubParty.Text = txtSalesType.Text = txtRemark.Text = txtSaleBillNo.Text = "";
            txtRoundOff.Text = txtOtherAmt.Text = txtPacking.Text = lblTaxableAmt.Text = txtInsuranceAmt.Text = txtTaxAmt.Text =lblQty.Text = lblGrossAmt.Text = lblNetAmt.Text = txtInsurancePer.Text = "0.00";
            txtSignAmt.Text = txtROSign.Text = "+";
            txtTaxPer.Text = "18.00";
            rdoAll.Checked = rdoCurrent.Checked = true;
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = false;
            lblCreatedBy.Text = "";
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtSaleBillDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtSaleBillDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

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

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill No can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }          
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }

            if (MainPage._bTaxStatus)
            {
                if (txtSalesType.Text == "")
                {
                    MessageBox.Show("Sorry ! Sale Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesType.Focus();
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

            CalculateAllAmount();

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(rows.Cells["itemName"].Value);
                double dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);
                if (strItem == "" && dAmount ==0)
                    dgrdDetails.Rows.Remove(rows);
                else
                {                  
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    //else if (dAmount == 0)
                    //{
                    //    MessageBox.Show("Sorry ! Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.CurrentCell = rows.Cells["qty"];
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}                   
                }
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                MessageBox.Show("Sorry ! Please add atleast one entry in table ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return ValidateOtherValidation(false);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1 || e.ColumnIndex == 10 || e.ColumnIndex == 17)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                    {
                        string strQuery = "";
                        if (txtSaleBillNo.Text != "" && txtSaleBillNo.Text != "0")
                            strQuery += " and BillCode='" + txtSaleBillCode.Text + "' and BillNo=" + txtSaleBillNo.Text + "  ";

                        if (!rdoManual.Checked)
                        {
                            SearchData objSearch = new SearchData("SALEBILLDETAILFORRETURN", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                GetDetailsFromSaleBillNo(objSearch.strSelectedData, e.RowIndex);
                                CalculateAllAmount();
                            }
                        }                       
                        else
                        {
                            string strType = "DESIGNNAMEWITHBARCODE_SALERETURN_MANUAL";
                            SearchCategory_Custom objSearch = new SearchCategory_Custom("","", strType, "", "", "", "", "", "", Keys.Space, false, false);
                            objSearch.ShowDialog();
                            GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);
                        }
                    

                        e.Cancel = true;
                    }
                    else if ((e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12 || e.ColumnIndex == 13 || e.ColumnIndex == 14) && !MainPage.strUserRole.Contains("ADMIN"))
                        e.Cancel = true;
                }               
                else
                    e.Cancel = true;
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void GetAllDesignSizeColorWithBarCode(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {
                if (objCategory != null)
                {
                    int columnIndex = 0;
                    double dRate = 0;
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;

                        if (strData != "ADD NEW DESIGNNAMEWITHBARCODE NAME")
                        {
                            string[] strAllItem = strData.Split('|');
                            string strBarcode = "",strItemName="", strVariant1="",strVariant2="";
                            if (strAllItem.Length > 0)
                            {
                                //strBarcode = strAllItem[0].Trim();
                                //string[] str = strBarcode.Split('.');

                               // dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = str[0];
                               // dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                             //   dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[2];
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value =strItemName= strAllItem[3];

                                if (strAllItem.Length > 6)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strVariant1 = strAllItem[4];
                                    columnIndex++;
                                }
                                if (strAllItem.Length > 7)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strVariant2 = strAllItem[5];
                                    columnIndex++;
                                }
                                if (strAllItem.Length > 8)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[6];
                                    columnIndex++;
                                }
                                if (strAllItem.Length > 9)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[7];
                                    columnIndex++;
                                }
                                if (strAllItem.Length > 10)
                                {
                                    columnIndex++;
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[8];
                                }

                                dRate = dba.ConvertObjectToDouble(strAllItem[4 + columnIndex]);

                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[5 + columnIndex];
                                dgrdDetails.Rows[rowIndex].Cells["rate"].Value = dRate;
                                //if (str.Length > 1)
                                //    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strBarcode;
                                GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                SetUnitName(strItemName,strVariant1,strVariant2, rowIndex);
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();
                    }
                }
            }
            catch
            {
            }
        }

        private void GetSaleRate(DataGridViewRow row)
        {
            try
            {

                double dDisPer = 0, dMRP = 0,  dRate = 0, dQty = 1, _dQty = 0, dQtyRatio = 1;             

                if (row != null)
                {
                    object objDisPer = 0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                        object objValue = 0;
                        if (MainPage.strSoftwareType == "AGENT")
                            objValue = dba.GetSaleRate(row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref _dQty, ref objDisPer, _date);
                        else
                        {
                            objValue = dba.GetSaleRate_Other(row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref _dQty, ref objDisPer, _date, ref dQtyRatio);                          
                        }

                        dDisPer = ConvertObjectToDouble(objDisPer) * -1;
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                    }
                }

                if (_dQty <= 0)
                    row.DefaultCellStyle.BackColor = Color.Tomato;             

                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100.00 + dDisPer) / 100.00;
                if (dRate == 0)
                    dRate = dMRP;

                dQtyRatio = (dQty * dQtyRatio);

                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                row.Cells["qty"].Value = dQtyRatio;

                double dAmt = 0, dDisc = ConvertObjectToDouble(row.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQtyRatio * dRate;

                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SetUnitName(string strItemName,string strVariant1, string strVariant2, int rowIndex)
        {
            if (strItemName != "")
            {
                DataTable table = dba.GetDataTable("Select UnitName from Items _IM  Where ItemName='"+ strItemName+"' ");
                if (table.Rows.Count > 0)
                {
                    //dgrdDetails.Rows[rowIndex].Cells["rate"].Value = dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = table.Rows[0]["SaleRate"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void GetDetailsFromSaleBillNo(string strData,int rowIndex)
        {
            try
            {
                string[] strValue = strData.Split('|');
                if (strValue.Length > 4)
                {
                    string strQuery = " Select * from SalesBookSecondary Where ItemName='"+strValue[0]+ "' and Variant1='" + strValue[1] + "' and Variant2='" + strValue[2] + "' and Qty=" + ConvertObjectToDouble(strValue[3]) + " and Rate=" + ConvertObjectToDouble(strValue[4]) + " ";
                    if (strQuery != "")
                    {
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            DataRow row = _dt.Rows[0];
                            dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                       //     dgrdDetails.Rows[rowIndex].Cells["id"].Value = "";//row["SID"]
                            dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = row["SONumber"];
                            dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                            dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                            dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                            dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                        }
                    }
                }
            }
            catch { }
        }   
     
        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 9)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 11 || e.ColumnIndex==12)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    //else if (e.ColumnIndex == 12)
                    //    CalculateRateWithQtyAmount(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 13 || e.ColumnIndex == 14)
                        CalculateAmountWithDiscOtherChargese(dgrdDetails.Rows[e.RowIndex]);
                   
                }
            }
            catch
            {
            }
        }
        
        private void CalculateRateWithQtyAmount(DataGridViewRow rows)
        {
            double dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            if (dAmount != 0 && dQty != 0)
                dRate = dAmount / dQty ;
            rows.Cells["rate"].Value = dRate.ToString("N2",MainPage.indianCurancy);
            rows.Cells["netAmt"].Value = (dAmount - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int CurrentRow = 0;
                    int IndexColmn = 0;
                    int Index;
                    if (e.KeyCode == Keys.Enter)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                        if (Index < dgrdDetails.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdDetails.ColumnCount - 2)
                        {
                            IndexColmn += 1;
                            if (!dgrdDetails.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            // double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "") // && dAmt > 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[2];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtPacking.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        if (btnAdd.Text == "&Save")
                        {
                            dgrdDetails.Rows.RemoveAt(Index);
                            CalculateAllAmount();
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                            if (strID != "")
                            {
                                DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    if (strDeletedSID != "")
                                        strDeletedSID += ",";
                                    strDeletedSID += strID;
                                    dgrdDetails.Rows.RemoveAt(Index);
                                    CalculateAllAmount();
                                }
                            }
                            else
                            {
                                dgrdDetails.Rows.RemoveAt(Index);
                                CalculateAllAmount();
                            }
                        }
                        if (dgrdDetails.Rows.Count == 0)
                            grpQtr.Enabled = true;
                    }          
                   
                }
            }
            catch { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 9 || columnIndex == 11 || columnIndex == 12 || columnIndex == 13 || columnIndex == 14 || columnIndex == 15)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (columnIndex == 9 || columnIndex == 11 || columnIndex == 12 || columnIndex == 13 || columnIndex == 14)
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    dba.KeyHandlerPoint(sender, e, 2);
            }
            else if (columnIndex == 15)
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    dba.KeyHandlerPoint(sender, e, 2);
            }

        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
            }
        }

        //private void DeleteOneRow(string strID)
        //{
        //    try
        //    {
        //        if (dgrdDetails.Rows.Count > 1)
        //        {
        //            string strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + " ";

        //            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
        //            CalculateAllAmount();
        //            int result = UpdateRecord(strQuery);
        //            if (result < 1)
        //                BindRecordWithControl(txtBillNo.Text);
        //            else
        //            {
        //                strQuery = " Delete from SalesBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " ";
        //                DataBaseAccess.CreateDeleteQuery(strQuery);
        //                if (dgrdDetails.Rows.Count == 0)
        //                {
        //                    dgrdDetails.Rows.Add(1);
        //                    dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
        //                    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
        //                    dgrdDetails.Enabled = true;
        //                }
        //                else
        //                    ArrangeSerialNo();
        //            }
        //        }

        //    }
        //    catch
        //    {
        //    }
        //}
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }
        private void CalculateAmountWithMRP(DataGridViewRow rows)
        {            

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);
                dDisPer = Math.Abs(dDisPer);
                if (dDisPer > 0)
                    dDisPer = dDisPer * -1;

                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 + dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;

                rows.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["rate"].Value = dRate;
                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        private void CalculateAmountWithDiscOtherChargese(DataGridViewRow rows)
        {
            double dAmt = ConvertObjectToDouble(rows.Cells["amount"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void CalculateAllAmount()
        {
            try
            {             
                double dFinalAmt=0,dQty = 0, dDisPer=0, dTOAmt =0, dBasicAmt = 0, dTaxableAmt=0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dPostage = 0,dGreenTaxAmt=0,dRoundOff=0;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value) ;
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                }

                lblGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dPackingAmt = ConvertObjectToDouble(txtPacking.Text);
                dOtherAmt = ConvertObjectToDouble(txtOtherAmt.Text);
                dDisPer = ConvertObjectToDouble(txtInsurancePer.Text);
                //dPostage = ConvertObjectToDouble(txtPostage.Text);
                //dGreenTaxAmt = ConvertObjectToDouble(txtGreenTax.Text);

                if (txtSignAmt.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                double dGrossAmt = 0;

                dDiscAmt = (dBasicAmt * dDisPer) / 100;   
                dTOAmt = dOtherAmt + dPackingAmt+dPostage+dGreenTaxAmt;               

                dDiscAmt = (dGrossAmt * dDisPer) / 100;
                dTOAmt += dDiscAmt;

                dTOAmt = dOtherAmt + dPackingAmt;
                dGrossAmt = dBasicAmt + dTOAmt;
                dDiscAmt = (dGrossAmt * dDisPer) / 100;
                dTOAmt += dDiscAmt;

                dFinalAmt = dGrossAmt + dDiscAmt;

                txtInsuranceAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt,ref dTaxableAmt);                          

                dNetAmt = dFinalAmt + dTaxAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);
                dRoundOff = dNNetAmt - dNetAmt;

                if(dRoundOff>=0)
                {
                    txtROSign.Text = "+";
                    txtRoundOff.Text = dRoundOff.ToString("0.00");
                }
                else
                {
                    txtROSign.Text = "-";
                    txtRoundOff.Text = Math.Abs(dRoundOff).ToString("0.00");
                }

                lblQty.Text = dQty.ToString("N2", MainPage.indianCurancy);             
                lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtInsuranceAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "")
            {
                try
                {
                    dValue = Convert.ToDouble(objValue);
                }
                catch
                {
                }
            }
            return dValue;
        }

        private void txtPackingAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPacking.Text == "")
                    txtPacking.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSignAmt.Text == "")
                    txtSignAmt.Text = "+";
                CalculateAllAmount();
            }
        }

        private void txtOtherAmount_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtOtherAmt.Text == "")
                    txtOtherAmt.Text = "0.00";
                CalculateAllAmount();
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
                    CalculateAllAmount();
                }
            }
        }

        private void txtPackingAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add")
            {
                if (btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                }
                btnAdd.Text = "&Save";
                btnEdit.Text = "&Edit";
                EnableAllControls();
                txtBillNo.ReadOnly = false;
                chkEmail.Checked = chkSendSMS.Checked = true;
                ClearAllText();
                SetSerialNo();
                txtDate.Focus();
            }
            else if (ValidateControls() && CheckBillNoAndSuggest())
            {
                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
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
            if (rdoManual.Checked)
                return "MANUAL";
            else
                return "ALL";
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strSaleParty = "", strSubParty = "",  strSalePartyID = "", strSubPartyID = "", strTaxAccountID = "";
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

                double dRate = 0, dQty = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "", strSONumber="";

                strQuery += " if not exists (Select BillCode from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[SaleReturn] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SaleBillCode],[SaleBillNo],[EntryType],[SaleType],[Remark],[OtherSign],[OtherAmt],[PackingAmt],[NetDiscount],[ServiceAmt],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleBillDate],[DiscountType],[Description1],[Description2],[Description3],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[IRNNO]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSaleBillCode.Text + "','" + txtSaleBillNo.Text + "','" + GetEntryType() + "','" + txtSalesType.Text + "','" + txtRemark.Text + "','" + txtSignAmt.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," + dba.ConvertObjectToDouble(txtPacking.Text) + "," +
                               + dba.ConvertObjectToDouble(txtInsuranceAmt.Text) + ",0," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dPTaxAmt + "," + dba.ConvertObjectToDouble(lblQty.Text) + "," + dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + strQtrStatus + "','" + txtInsurancePer.Text + "','" + MainPage.strLoginName + "','',1,0,'" + strSDate + "',0,'','','"+txtImportData.Text+ "','" + txtROSign.Text + "'," + ConvertObjectToDouble(txtRoundOff.Text) + "," + ConvertObjectToDouble(lblTaxableAmt.Text) + ",'"+txtIRNNo.Text+"')  "
                               + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strSaleParty + "','SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNetAmt.Text + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "') ";

                double dDisPer = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    if (dDisPer > 0)
                        dDisPer = dDisPer * -1;

                    strQuery += " INSERT INTO [dbo].[SaleReturnDetails] ([RemoteID],[BillCode],[BillNo],[PurchaseBillNo],[PurchasePartyID],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[Packing],[TotalAmt],[UnitName],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus],[ItemStatus],[DisStatus],[Discount]) VALUES  "
                                  + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strSONumber + "','','','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dDisPer + "," + dRate + ","
                                  + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "',0,'' ,1,0,'FRESH',0,0)";
                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                             + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ," + (dDisPer - 3) + ",'" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "') ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
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
                                   + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";
                
                strQuery += "  Update SM Set SM.BarCode=_IM.BarCode from StockMaster SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo=" + txtBillNo.Text
                         + " Update SM Set SM.BarCode=_IM.BarCode from SaleReturnDetails SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo=" + txtBillNo.Text + " end";


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
                    string strPath = CreatePDFFile(false, ref Created), strEmailID = "", strWhatsAppNo = "";
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
            string _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text;
            if (!strBranchCode.Contains("-"))
                strBranchCode = "18-19/" + strBranchCode;
            string strWhastappMessage = "", strMsgType = "", strMType = "";
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

                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    dgrdDetails.ReadOnly = qtyAdjustStatus;
                    txtBillNo.ReadOnly = true;
                    txtDate.Focus();
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateRecord("");
                        if (count > 0)
                        {

                            string strMobileNo = "", strPath = "";
                            SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                            SendSMSToParty(strMobileNo);

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
                            MessageBox.Show("Sorry ! Record not updated, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
        }

        private int UpdateRecord(string strSQuery)
        {
            int _count = 0;
            try
            {
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "", strTaxAccountID = "", strDeletedSIDQuery = "";
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

                double dRate=0, dAmt = 0, dQty = 0,  dDisper = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "", strID = "", strSONumber="";

           //     strQuery += strAmendedQuery;

                strQuery += "UPDATE  [dbo].[SaleReturn]  SET [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SaleBillCode]='" + txtSaleBillCode.Text + "',[SaleBillNo]='" + txtSaleBillNo.Text + "',[EntryType]='" + GetEntryType() + "',[SaleType]='" + txtSalesType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSignAmt.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[PackingAmt]=" + dba.ConvertObjectToDouble(txtPacking.Text) + ",[OtherText]='" + strQtrStatus + "', "
                               + " [NetDiscount]=" + dba.ConvertObjectToDouble(txtInsuranceAmt.Text) + ",[ServiceAmt]=0,[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",[OtherValue]='"+txtInsurancePer.Text+"',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SaleBillDate]='" + strSDate + "',[Description3]='"+txtImportData.Text+ "',[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + ConvertObjectToDouble(txtRoundOff.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[IRNNO]='"+txtIRNNo.Text+"' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                               + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strSalePartyID + "' Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                               + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                               + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                               + " Delete from StockMaster Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strID = Convert.ToString(row.Cells["id"].Value);
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dDisper = ConvertObjectToDouble(row.Cells["disPer"].Value);
                    if (dDisper > 0)
                        dDisper = dDisper * -1;

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SaleReturnDetails] ([RemoteID],[BillCode],[BillNo],[PurchaseBillNo],[PurchasePartyID],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[Packing],[TotalAmt],[UnitName],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus],[ItemStatus],[DisStatus],[Discount]) VALUES  "
                                    + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strSONumber + "','','','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dDisper + "," + dRate + ","
                                    + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "',0,'' ,1,0,'FRESH',0,0)";
                    }
                    else
                    {
                        strQuery += " UPDATE [dbo].[SaleReturnDetails] Set [PurchaseBillNo]='" + strSONumber + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",[SDisPer]=" + dDisper + ",[Rate]=" + dRate + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[Disc]=" + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + ",[Packing]=" + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ",[TotalAmt]= " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdateStatus]=1 Where [SID]=" + strID + " and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";
                    }
                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                             + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "') ";
                    }
                }


                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
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
                                   + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                object objValue = "";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[SaleReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strDeletedSID + ") ";
                    strDeletedSIDQuery = " Delete from [dbo].[SaleReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";

                    objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                       + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery += " Update SM Set SM.BarCode=_IM.BarCode from StockMaster SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo=" + txtBillNo.Text+ "  and _IM.BarCode!='' "
                         + " Update SM Set SM.BarCode=_IM.BarCode from SaleReturnDetails SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo=" + txtBillNo.Text + " and _IM.BarCode!='' ";


                _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    if (Convert.ToString(objValue) != "" && strDeletedSIDQuery != "")
                    {
                        if (!Convert.ToBoolean(objValue))
                        {
                            DataBaseAccess.CreateDeleteQuery(strDeletedSIDQuery);
                        }
                    }
                    strDeletedSIDQuery = strDeletedSID = "";
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
            return _count;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }        

        private void txtSalesType_KeyDown(object sender, KeyEventArgs e)
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
                        txtSalesType.Text = objSearch.strSelectedData;
                        CalculateAllAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPackingAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
        }
      

    
        private void txtRoundOff_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtROSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                CalculateAllAmount();
            }
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearAllText();
            }
            //else if (txtSerialNo.Text != "")
            //    CheckSerialNoAvailability();
        }
        
        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
        }

        private void CreateDataTableColumn(ref DataTable dt)
        {
            dt.Columns.Add("HeaderName", typeof(String));
            dt.Columns.Add("CompanyName", typeof(String));
            dt.Columns.Add("CompanyAddress", typeof(String));
            dt.Columns.Add("CompanyEmail", typeof(String));
            dt.Columns.Add("BillNo", typeof(String));
            dt.Columns.Add("Date", typeof(String));
            dt.Columns.Add("PartyName", typeof(String));
            dt.Columns.Add("PartyAddress", typeof(String));
            dt.Columns.Add("PartyEmail", typeof(String));
            dt.Columns.Add("LedgerName", typeof(String));
            dt.Columns.Add("AgentName", typeof(String));
            dt.Columns.Add("TransportName", typeof(String));
            dt.Columns.Add("PONumber", typeof(String));
            dt.Columns.Add("PODate", typeof(String));
            dt.Columns.Add("Remark", typeof(String));
            dt.Columns.Add("SNo", typeof(String));
            dt.Columns.Add("ItemName", typeof(String));
            dt.Columns.Add("Qty", typeof(String));
            dt.Columns.Add("DQty", typeof(String));
            dt.Columns.Add("Rate", typeof(String));
            dt.Columns.Add("Unit", typeof(String));
            dt.Columns.Add("Amount", typeof(String));
            dt.Columns.Add("Disc", typeof(String));
            dt.Columns.Add("OtherCharges", typeof(String));
            dt.Columns.Add("BasicAmt", typeof(String));
            dt.Columns.Add("OtherText", typeof(String));
            dt.Columns.Add("NetAmt", typeof(String));
            dt.Columns.Add("TotalQty", typeof(String));
            dt.Columns.Add("AmountInWord", typeof(String));
            dt.Columns.Add("UserName", typeof(String));
         
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            try
            {
                CreateDataTableColumn(ref table);

                string strQuery = " Select CM.CmpAddress as CompanyEmail,CM.CmpCity as EmailPwd, CD.FullName,(Address+'-'+CAST(CD.PinCode as varchar))Address, ('Phone : '+CD.stdCode1+'-'+CD.PhoneNo1 +', Email : '+CD.EmailId) PhoneNo,(SM.PostalAddress+', '+SM.City)PartyAddress,(SM.State+'-'+SM.PinCode) PartyStatePIN, "
                                       + " ((CASE SM.PhoneNo When '' then '' else ' Phone No. : '+SM.PhoneNo end)+(CASE SM.EmailID When '' then '' else ' Email Id : '+SM.EMailID end)) PartyPhoneNo,SM.TINNumber as BuyerTIN,SM.CSTNumber as BuyerCST,CD.TinNo as CompanyTIN from CompanyDetails  CD,SupplierMaster SM,CompanyMaster CM Where SM.Name='" + txtSalesParty.Text + "'"
                                       + " DECLARE @PO VARCHAR(MAX),@PODate varchar(MAX) SET @PO='' ; SET @PODate='' SELECT  @PO = @PO + (Case When CharIndex('/',PONumber)>1 then SUBSTRING(Replace(PONumber,'PO/',''),0,CharIndex('/',Replace(PONumber,'PO/',''))) else PONumber end) +',',@PODate=@PODate+ Replace(Convert(Varchar(6), PODate, 106), ' ', '-')+',' FROM PurchaseOrder Where PONumber in (Select PONumber from PurchaseBookSecondary Where PONumber!='' and SerialNo=" + txtBillNo.Text + ") "
                                       + " Select PBS.ItemName,PBS.Category1,PBS.Category2,PBS.Category3,PBS.Category4,PBS.Category5,DM.SupplierDesignName,DM.ManfDesignName,Sum(PBS.Qty) Qty,SUM(ISNULL(PBS.AlterQty,0)) AlterQty,SUM(ISNULL(PBS.DQty,0)) DQty,PBS.Rate,(Sum(PBS.Qty+ISNULL(PBS.DQty,0))*Rate) Amount,SUM(PBS.Discount) Disc,SUM(PBS.OCharges) OtherCharges,SUM(PBS.BasicAmt)BasicAmt, PBS.UnitName,Case When LEN(@PO)>0 then (LEFT(@PO,LEN(@PO)-1)) else '' end AS PONumber, "
                                       + " Case When LEN(@PODate)>0 then (LEFT(@PODate,LEN(@PODate)-1))else '' end  AS PODate from PurchaseBookSecondary PBS left Join DesignMaster DM on PBS.ItemName=DM.DesignName Where PBS.SerialNo=" + txtBillNo.Text + " Group By PBS.ItemName,PBS.Category1,PBS.Category2,PBS.Category3,PBS.Category4,PBS.Category5,PBS.Rate,PBS.UnitName,DM.SupplierDesignName,DM.ManfDesignName Order by ItemName,PBS.Category2,PBS.Category1";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0], dtItem = ds.Tables[1];
                    DataRow dRow = dt.Rows[0];
                    string strCompanyName = "", strComapnyAddress = "", strCompanyEmail = "", strPartyName = "", strPartyAddress = "", strPartySP = "", strPartyEmail = "", strAgentName = "", strItemName = "";
                    //if (MainPage.pCompanyName)
                    strCompanyName = Convert.ToString(dRow["FullName"]);
                    //if (MainPage.pCompanyAddress)
                    //{
                    strComapnyAddress = Convert.ToString(dRow["Address"]);
                    strCompanyEmail = Convert.ToString(dRow["PhoneNo"]);
                    //}
                    //if (MainPage.pBuyerName)
                  //  strPartyName = txtPartyName.Text;
                    //if (MainPage.pBuyerAddress)
                    //{
                    strPartyAddress = Convert.ToString(dRow["PartyAddress"]);
                    strPartySP = Convert.ToString(dRow["PartyStatePIN"]);
                    strPartyEmail = Convert.ToString(dRow["PartyPhoneNo"]);
                    //}
                    //if (MainPage.pAgentName)
                  //  strAgentName = txtAgentName.Text;

                    int rowIndex = 1;
                    foreach (DataRow rows in dtItem.Rows)
                    {
                        DataRow row = table.NewRow();

                        row["HeaderName"] = "Purchase Bill";
                        row["CompanyName"] = strCompanyName;
                        row["CompanyAddress"] = strComapnyAddress;
                        row["CompanyEmail"] = strCompanyEmail;
                        row["PartyName"] = strPartyName;
                        row["PartyAddress"] = strPartyAddress + "  " + strPartySP;
                        row["PartyEmail"] = strPartyEmail;
                        row["LedgerName"] = txtSalesType.Text;
                        row["BillNo"] = txtBillNo.Text;
                        row["Date"] = txtDate.Text;
                      //  row["AgentName"] = txtAgentName.Text;
                      //  row["TransportName"] = txtTransport.Text;
                        row["PONumber"] = rows["PONumber"];
                        row["PODate"] = rows["PODate"];
                        if (txtRemark.Text != "")
                            row["Remark"] = "Remark : " + txtRemark.Text;
                   
                            strItemName = Convert.ToString(rows["ItemName"]);

                        //if (Convert.ToString(rows["Category2"]) != "")
                        //    strItemName += " / " + rows["Category2"];

                        //if (Convert.ToString(rows["Category1"]) != "")
                        //    strItemName += " / " + rows["Category1"];
                        row["SNo"] = rowIndex + ".";
                        row["ItemName"] = strItemName;
                        if (Convert.ToString(rows["AlterQty"]) == "0")
                            row["Qty"] = rows["Qty"];
                        else
                            row["Qty"] = rows["Qty"] + "/" + rows["AlterQty"];
                        row["DQty"] = rows["DQty"];
                        row["Unit"] = rows["UnitName"];
                        row["Rate"] = rows["Rate"];
                        row["Amount"] = Convert.ToDouble(rows["Amount"]).ToString("N2", MainPage.indianCurancy);
                        row["Disc"] = rows["Disc"];
                        row["OtherCharges"] = rows["OtherCharges"];
                        row["BasicAmt"] = Convert.ToDouble(rows["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                        row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");
                        table.Rows.Add(row);
                        rowIndex++;
                    }

                    AddOtherAmount(ref table);

                    if (table.Rows.Count > 0)
                    {
                        DataRow row = table.Rows[table.Rows.Count - 1];
                        row["TotalQty"] = lblQty.Text;
                        row["NetAmt"] = lblNetAmt.Text;
                        double dAmt = Convert.ToDouble(lblNetAmt.Text);
                        ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();
                        row["AmountInWord"] = objCurrency.changeCurrencyToWords(dAmt);
                    }
                }
            }
            catch
            {
            }
            return table;
        }

        private void AddOtherAmount(ref DataTable table)
        {
            double dPackingAmt = Convert.ToDouble(txtPacking.Text), dOtherAmt = Convert.ToDouble(txtOtherAmt.Text), dTaxAmt = Convert.ToDouble(txtTaxAmt.Text), dDiscount = Convert.ToDouble(txtInsuranceAmt.Text), dRoundOff = Convert.ToDouble(txtRoundOff.Text);
            DataRow row = table.NewRow();
            AsignPreviousData(ref row, table);
            row["BasicAmt"] = "-------------------";
            table.Rows.Add(row);
            row = table.NewRow();
            AsignPreviousData(ref row, table);
            row["BasicAmt"] = lblGrossAmt.Text;
            table.Rows.Add(row);

            if (dPackingAmt > 0)
            {
                row = table.NewRow();
                AsignPreviousData(ref row, table);
                row["Amount"] = "Packing";
                row["Disc"] = "";
                row["OtherCharges"] = "";
                row["BasicAmt"] = "(+) " + dPackingAmt.ToString("N2", MainPage.indianCurancy);
                table.Rows.Add(row);
            }
            if (dOtherAmt > 0)
            {
                row = table.NewRow();
                AsignPreviousData(ref row, table);
                row["Amount"] = "Other Amount";
                row["Disc"] = "";
                row["OtherCharges"] = "";
                row["BasicAmt"] = "(" + txtSignAmt.Text + ") " + dOtherAmt.ToString("N2", MainPage.indianCurancy);
                table.Rows.Add(row);
            }
            if (dTaxAmt > 0)
            {
                row = table.NewRow();
                AsignPreviousData(ref row, table);
                if (txtSalesType.Text != "")
                    row["Amount"] = txtSalesType.Text;
                else
                    row["Amount"] = "Tax";
                row["Disc"] = txtTaxPer.Text;
                row["OtherCharges"] = "%";
                row["BasicAmt"] = "(+) " + dTaxAmt.ToString("N2", MainPage.indianCurancy);
                table.Rows.Add(row);
            }
            //if (dAgentComm > 0)
            //{
            //    row = table.NewRow();
            //    AsignPreviousData(ref row, table);
            //    row["OtherAmount"] = "Agent Commission";
            //    row["Rate"] = txtAgentCommPer.Text;
            //    row["UOM"] = "%";
            //    row["Amount"] = "(+) " + dAgentComm.ToString("N2", MainPage.indianCurancy);
            //    table.Rows.Add(row);
            //}
            if (dDiscount > 0)
            {
                row = table.NewRow();
                AsignPreviousData(ref row, table);
                row["Amount"] = "Discount";
                row["Disc"] = "";
                row["OtherCharges"] = "%";
                row["BasicAmt"] = "(-) " + dDiscount.ToString("N2", MainPage.indianCurancy);
                table.Rows.Add(row);
            }
            if (dRoundOff > 0)
            {
                row = table.NewRow();
                AsignPreviousData(ref row, table);
                row["Amount"] = "Round Off";
                row["BasicAmt"] = "(" + txtROSign.Text + ") " + dRoundOff.ToString("N2", MainPage.indianCurancy);
                table.Rows.Add(row);
            }
        }

        private void AsignPreviousData(ref DataRow row, DataTable dt)
        {

            DataRow dr = dt.Rows[dt.Rows.Count - 1];
            row["HeaderName"] = dr["HeaderName"];
            row["CompanyName"] = dr["CompanyName"];
            row["CompanyAddress"] = dr["CompanyAddress"];
            row["CompanyEmail"] = dr["CompanyEmail"];
            row["PartyName"] = dr["PartyName"];
            row["PartyAddress"] = dr["PartyAddress"];
            row["PartyEmail"] = dr["PartyEmail"];
            row["LedgerName"] = dr["LedgerName"];
            row["BillNo"] = dr["BillNo"];
            row["Date"] = dr["Date"];
            row["AgentName"] = dr["AgentName"];
            row["TransportName"] = dr["TransportName"];
            row["PONumber"] = dr["PONumber"];
            row["PODate"] = dr["PODate"];
            row["Remark"] = dr["Remark"];
            row["UserName"] = dr["UserName"];         
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
                string[] strReport = { "Exception occurred in Preview  in Trading Sales Return", ex.Message };
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
                    GSTPrintAndPreview(true, "");
                   // SetSignatureInBill(true, false);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }      
        
        private void PurchaseBook_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void SetPermission()
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
                        txtBillNo.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch { }
        }


        //private double GetDataTaxable()
        //{
        //    double dTaxRate = 0, dTaxAmt = 0,dTOtherAmt = 0;
        //    string _strTaxType = "";
        //    if (txtTaxLedger.Text != "")
        //    {
        //        _strTaxType = DataBaseAccess.GetTaxType(txtTaxLedger.Text);
        //        if (_strTaxType != "NONE")
        //        {
        //            DataTable _dt = new DataTable();
        //            _dt.Columns.Add("ItemName", typeof(String));
        //            _dt.Columns.Add("Qty", typeof(String));
        //            _dt.Columns.Add("Rate", typeof(String));
        //            string strItem = "";
        //            double dQty = 0, dRate = 0, dPQty = 0;
        //            foreach (DataGridViewRow row in dgrdDetails.Rows)
        //            {
        //                strItem = Convert.ToString(row.Cells["itemName"].Value);
        //                dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
        //                dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
        //                dTOtherAmt += ConvertObjectToDouble(row.Cells["otherCharges"].Value);

        //                DataRow[] _rows = _dt.Select("ItemName='" + strItem + "' and Rate=" + dRate);
        //                if (_rows.Length > 0)
        //                {
        //                    dPQty = ConvertObjectToDouble(_rows[0]["Qty"]);
        //                    dPQty += dQty;
        //                    _rows[0]["Qty"] = dPQty;
        //                }
        //                else
        //                {
        //                    DataRow _row = _dt.NewRow();
        //                    _row["ItemName"] = strItem;
        //                    _row["qty"] = dQty;
        //                    _row["Rate"] = dRate;
        //                    _dt.Rows.Add(_row);
        //                }
        //            }

        //            foreach (DataRow row in _dt.Rows)
        //            {
        //                strItem = Convert.ToString(row["ItemName"]);
        //                dRate = ConvertObjectToDouble(row["Rate"]);
        //                dQty = ConvertObjectToDouble(row["Qty"]);

        //                dTaxRate = ConvertObjectToDouble(DataBaseAccess.GetGSTTaxRate(strItem, dRate));
        //                if (_strTaxType == "INCLUDED")
        //                    dRate = (dRate * 100) / (100 + dTaxRate);

        //                dTaxRate = ((dRate * dTaxRate) / 100);

        //                dTaxAmt += dQty * dTaxRate;
        //            }

        //            double dPackingAmt = 0, dOtherAmt = 0, dDiscount = 0;
        //            dPackingAmt = ConvertObjectToDouble(txtPackingAmt.Text);
        //            dDiscount = ConvertObjectToDouble(txtDiscAmt.Text);
        //            dOtherAmt = ConvertObjectToDouble(txtSign.Text + txtOtherAmount.Text);
        //            dTOtherAmt += dPackingAmt + dOtherAmt - dDiscount;

        //            double dSPer = DataBaseAccess.GetGSTShippingTaxRate(dTOtherAmt);
        //            dTaxAmt += (dTOtherAmt * dSPer) / 100;
        //            txtTaxPer.Text = "0.00";
        //        }
        //    }

        //    txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
        //    if (_strTaxType == "INCLUDED")
        //        dTaxAmt = 0;

        //    return dTaxAmt;
        //}

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer=0, dServiceAmt =0,dInsuranceAmt=0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtSalesType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);
                    if (MainPage.startFinDate >= Convert.ToDateTime("04/01/2021"))
                        dTaxPer = 18;

                    DataTable _dt = dba.GetSaleTypeDetails(txtSalesType.Text, "SALES");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        string strTaxationType = Convert.ToString(row["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                                _strTaxType = "INCLUDED";
                                                       
                            dInsuranceAmt = dba.ConvertObjectToDouble(txtInsuranceAmt.Text);

                            string strQuery = "", strSubQuery = "", strGRSNo = "",strTaxRate="";
                            double dDisStatus = 0;

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text; 

                            double dRate = 0,dQty = 0, dAmt = 0,dBasicAmt=0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                                dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                                                               
                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dBasicAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
                                }
                            }                           
                          
                            //if (dInsuranceAmt != 0)
                            //    strTaxRate = "18";
                            //else
                            //    strTaxRate = "0";

                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount,"+ dTaxPer+" as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) ServiceAmt from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' and Qty>0 then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    //BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    dServiceAmt = dba.ConvertObjectToDouble(dt.Rows[0]["ServiceAmt"]);
                                    dTaxAmt = dTTaxAmt;
                                    if (dOtherAmt == 0)
                                        dTaxPer = dMaxRate;
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
                            txtTaxAmt.Text = txtTaxPer.Text = "0.00";
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
                string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEdit.Enabled = btnAdd.Enabled = false;
            }

            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);

            if (_strTaxType == "INCLUDED")
                dTaxAmt = dServiceAmt;
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


        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (btnAdd.Text != "&Save" && dba.ValidateBackDateEntry(txtDate.Text))
                {
                    if (txtReason.Text != "" && ValidateOtherValidation(true))
                    {

                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strQuery = " Delete from [SaleReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                                + " Delete from [SaleReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " Delete from BalanceAmount Where AccountStatus in ('SALE RETURN','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                                + " Delete from [dbo].[StockMaster] Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                                int count = dba.ExecuteMyQuery(strQuery);
                                if (count > 0)
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);

                                    MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    txtReason.Text = "";
                                    pnlDeletionConfirmation.Visible = false;
                                    BindNextRecord();
                                }
                                else
                                    MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSalesType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
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


        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
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
        
        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
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

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void AskForPrint()
        {
            try
            {

                DialogResult _result = MessageBox.Show("Are you want to print Sale Bill ?", "Print Sale Service Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_result == DialogResult.Yes)
                    SetSignatureInBill(true, false);
            }
            catch
            {
            }
        }

        private string SetSignatureInBill(bool _bPStatus, bool _createPDF)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
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
                    _browser.ShowDialog();

                    if (_browser.FileName != "")
                        strPath = _browser.FileName;
                }
                else
                {
                    string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SalesService\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    _strPath += "\\" + _strFileName;

                    strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                    if (File.Exists(strPath))
                        File.Delete(strPath);
                    Directory.CreateDirectory(_strPath);
                }

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strFileName);
                    if (_bstatus && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";
                        PDFSigner _objSigner = new PDFSigner();
                        bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                        if (!_bFileStatus)
                            strPath = "";
                        if (_bPStatus && _bFileStatus)
                            System.Diagnostics.Process.Start(strPath);
                    }
                }
                //}
                //else
                //{
                //    GSTPrintAndPreview(true, "", false);
                //}
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

        private bool GSTPrintAndPreview(bool _pstatus, string strPath)
        {
            DataTable _dtGST = null, _dtSalesAmt = null; ;
            bool _bIGST = false;
            DataTable dt = dba.CreateDebitNoteRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "CREDIT NOTE");
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
                        if (MainPage._bTaxStatus)
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                       
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
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
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
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
                        if (MainPage._bTaxStatus)
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //if (strPath != "")
                        //{
                        //    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //}
                        //else
                        //{
                        //    if (_pstatus)
                        //    {
                        //        // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
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
            return true;
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
                    // string strValue = "0";
                    if (_pstatus)
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;
                        defS.Copies = (short)MainPage.iNCopySaleRtn;

                        // strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                        // if (strValue != "" && strValue != "0")
                        //{
                        //  int nCopy = Int32.Parse(strValue);
                        Report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        //  }
                    }
                }
                else
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES RETURN REPORT PREVIEW");
                    objReport.myPreview.ReportSource = Report;
                    objReport.ShowDialog();
                }
            }
            Report.Close();
            Report.Dispose();
        }
        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtSalesParty.Text)), strBalance = ".", strName = dba.GetSafePartyName(txtSalesParty.Text);
                    if (strMobileNo != "")
                    {
                        if (MainPage.strSendBalanceInSMS == "YES")
                        {
                            double dAmt = dba.GetPartyAmountFromQuery(txtSalesParty.Text);
                            if (dAmt > 0)
                                strBalance = " BAL : " + dAmt.ToString("0") + " Dr";
                            else if (dAmt < 0)
                                strBalance = " BAL : " + Math.Abs(dAmt).ToString("0") + " Cr";
                            else
                                strBalance = " BAL : 0";
                        }

                        string strMessage = "", strSubMsg = "";                       
                        if (txtRemark.Text != "")
                            strSubMsg += ", Note : " + txtRemark.Text;

                        if (btnAdd.Text == "&Save")
                            strMessage = "M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblQty.Text +" " + strSubMsg + strBalance;
                        else
                            strMessage = "Alert : M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblQty.Text + " " + strSubMsg + strBalance;

                     
                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
            catch
            {
            }
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
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

                            if (rdoOldYear.Checked)
                            {
                                SearchDataOnOld objSearch = new SearchDataOnOld("SALEBILLNOFORRETURNRETAIL", strQuery, "SEARCH SALE BILL NO", e.KeyCode, true);
                                objSearch.ShowDialog();
                                string[] strBillNo = objSearch.strSelectedData.Split('|');
                                txtSaleBillNo.Text = strBillNo[0];
                                if (strBillNo.Length > 1)
                                {
                                    txtSaleBillDate.Text = strBillNo[1];
                                    if (rdoAll.Checked)
                                        GetSaleReturnBillDetails();
                                }
                            }
                            else
                            {
                                SearchData objSearch = new SearchData("SALEBILLNOFORRETURNRETAIL", strQuery, "SEARCH SALE BILL NO", e.KeyCode);
                                objSearch.ShowDialog();
                                string[] strBillNo = objSearch.strSelectedData.Split('|');
                                txtSaleBillNo.Text = strBillNo[0];
                                if (strBillNo.Length > 1)
                                {
                                    txtSaleBillDate.Text = strBillNo[1];
                                    if(rdoAll.Checked)
                                        GetSaleReturnBillDetails();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Warning ! Please select Sundry Debtors name after that you can select bill no. !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSaleBillNo.Focus();
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

        private void txtInsurancePer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtInsurancePer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtInsurancePer.Text == "")
                    txtInsurancePer.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void rdoOldYear_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOldYear.Checked)
            {
                txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = false;
                dgrdDetails.Rows.Clear();
                dgrdDetails.Rows.Add();             
            }
            else if (!rdoManual.Checked)
            {
                txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = true;
                txtSaleBillCode.Text = strSaleBillCode;                
            }
        }

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtImportData.Enabled = chkPick.Checked;
                txtImportData.Clear();
            }
            else
            {
                txtImportData.Enabled = false;
                txtImportData.Clear();
            }
        }

        private void txtImportData_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchDataOther objSearch = new SearchDataOther("SALERETURNBILLNO", "", "SEARCH SALE RETURN BILL NO", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportData.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
                    }
                    else
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetDataFromLocal()
        {
            if (txtImportData.Text != "" && btnAdd.Text == "&Save")
            {
                BindRecordWithControl_Import();
            }

        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;
                DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    bool Printed = false;
                    string strPath = CreatePDFFile(true, ref Printed);
                    if (Printed)
                        MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }


        private void txtSaleBillDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
        }

        private void txtTaxAmt_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
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
                                var _success = dba.GenerateEInvoiceJSON_SaleBook(true,strBillNo, "CREDITNOTE", "CRN");
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
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnEInvoice.Enabled = true;
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoManual.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = false;
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add();                   
                }
                else if (!rdoOldYear.Checked)
                {
                    txtSaleBillCode.ReadOnly = txtSaleBillNo.ReadOnly = true;
                    txtSaleBillCode.Text = strSaleBillCode;                  
                }
            }
            catch { }
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

        private void GetSaleReturnBillDetails()
        {
            string strQuery = "";
            if (txtSaleBillNo.Text != "")
            {
                dgrdDetails.Rows.Clear();
                DataTable dt = null;
                strQuery = " Select *,(Select Description_3 from SalesBook SB Where SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo)Description_3 from SalesBookSecondary  SBS Where BillCode='" + txtSaleBillCode.Text+"' and BillNo="+txtSaleBillNo.Text+ " Order by SID asc ";
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
                    int rowIndex = 0;
                    txtImportData.Text = Convert.ToString(dt.Rows[0]["Description_3"]);

                    foreach (DataRow row in dt.Rows)
                    {

                        dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                        dgrdDetails.Rows[rowIndex].Cells["id"].Value ="";
                        dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = row["SONumber"];
                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                        dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                        dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                        dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                        dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                        rowIndex++; 
                    }

                    CalculateAllAmount();
                }                
            }
        }


        private DataTable GenerateDistinctItemName()
        {
            DataTable _dt = new DataTable();
            try
            {
               
                _dt.Columns.Add("ItemName", typeof(String));
                _dt.Columns.Add("Variant1", typeof(String));
                _dt.Columns.Add("Variant2", typeof(String));
                _dt.Columns.Add("Variant3", typeof(String));
                _dt.Columns.Add("Variant4", typeof(String));
                _dt.Columns.Add("Variant5", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("ItemName='" + row.Cells["itemName"].Value + "' and Variant1='" + row.Cells["variant1"].Value + "' and Variant2='" + row.Cells["variant2"].Value + "' and ISNULL(Variant3,'')='" + row.Cells["variant3"].Value + "' and ISNULL(Variant4,'')='" + row.Cells["variant4"].Value + "' and ISNULL(Variant5,'')='" + row.Cells["variant5"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]),dQty=dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                        _rows[0]["Qty"] = dOQty + dQty;
                    }
                    else
                    {
                        DataRow _row = _dt.NewRow();
                        _row["ItemName"] = row.Cells["itemName"].Value;
                        _row["Variant1"] = row.Cells["variant1"].Value;
                        _row["Variant2"] = row.Cells["variant2"].Value;
                        _row["Variant3"] = row.Cells["variant3"].Value;
                        _row["Variant4"] = row.Cells["variant4"].Value;
                        _row["Variant5"] = row.Cells["variant5"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
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
    }
}
