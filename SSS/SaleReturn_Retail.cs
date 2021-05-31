using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class SaleReturn_Retail : Form
    {
        DataBaseAccess dba;
        SendSMS objSMS;
        string strLastSerialNo = "", strDeletedSID = "", strSaleBillCode = "", strOldPartyName = "", _strAttachBillWithComma = "", OldYearDBName = "";
        bool qtyAdjustStatus = false;
        public bool saleStatus = false, updateStatus = false, newStatus = false;
        double dOldNetAmt = 0, dOldCashAmt = 0, dOldSaleReturnAmt = 0;
        public SaleReturn_Retail()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            GetStartupData(true);
        }

        public SaleReturn_Retail(string strSerialCode, string strSerialNo)
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
                string strQuery = " Select SBillCode,GReturnCode,(Select ISNULL(MAX(BillNo),0) from SaleReturn Where BillCode=GReturnCode)SerialNo,(Select Top 1 Layout from PrintLayoutMaster) as Layout from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
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
                    MainPage.strPrintLayout = Convert.ToString(dt.Rows[0]["Layout"]);

                    if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                        BindRecordWithControl(strLastSerialNo);

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleReturn Where [ReturnType]='RETAIL' and BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleReturn Where [ReturnType]='RETAIL' and BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SaleReturn Where [ReturnType]='RETAIL' and BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SaleReturn Where [ReturnType]='RETAIL' and BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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

                    string strQuery = " Select *,Convert(varchar,Date,103)BDate,Convert(varchar,SaleBillDate,103)SBDate,ISNULL(dbo.GetFullName(SalePartyID),SalePartyID) as SalesParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SR.Date))) LockType from SaleReturn SR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                    + " Select SRD.*, HSNCode,ISNULL((SRD.SalesMan+' '+SM.Name),'DIRECT')SalesManName from SaleReturnDetails SRD left join SupplierMaster SM on Sm.AreaCode+SM.AccountNo=SRD.SalesMan left join(Select _IM.ItemName,IGM.HSNCode from Items _IM inner join ItemGroupMaster IGM on _IM.GroupName = IGM.GroupName)_IM on _IM.ItemName = SRD.ItemName Where BillCode ='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID " + " Select *,ISNULL((GSTAccount+' '+SM.Name),'') AccountName from dbo.[GSTDetails] GSTD left join SupplierMaster SM on Sm.AreaCode+SM.AccountNo=GSTD.GSTAccount  Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    pnlDeletionConfirmation.Visible = false;
                    txtReason.Text = "";
                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            pnlTax.Visible = true;
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
                strOldPartyName = txtCustomerName.Text = Convert.ToString(row["SalesParty"]);
                txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                txtSaleBillCode.Text = Convert.ToString(row["SaleBillCode"]);
                txtSaleBillNo.Text = Convert.ToString(row["SaleBillNo"]);
                txtSaleBillDate.Text = Convert.ToString(row["SBDate"]);
                txtSalesType.Text = Convert.ToString(row["SaleType"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                txtTotalQty.Text = Convert.ToString(row["TotalQty"]);
                txtSign.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                dOldCashAmt = dba.ConvertObjectToDouble(row["CashAmt"]);
                txtDisAmt.Text = Convert.ToString(row["NetDiscount"]);
                txtDiscPer.Text = Convert.ToString(row["OtherValue"]);
                txtCashAmt.Text = dOldCashAmt.ToString("N2", MainPage.indianCurancy);
                dOldSaleReturnAmt = dba.ConvertObjectToDouble(row["PartSaleReturnAmt"]);
                txtSaleReturnAmt.Text = dOldSaleReturnAmt.ToString("N2", MainPage.indianCurancy);
                txtGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                dOldNetAmt = dba.ConvertObjectToDouble(row["CreditAmt"]);
                txtNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                txtAttachBill.Text = Convert.ToString(row["AttachedBill"]);

                txtCashAmt.Text = Convert.ToString(row["CashAmt"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";


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

                txtBillNo.ReadOnly = false;
            }
        }
        private string GetHSNCode(object _objHSNCode)
        {
            string strQuery = "";

            strQuery = "Select Top 1 ItemName from Items _Im  inner join ItemGroupMaster IGM on _IM.GroupName=IGM.GroupName WHere ItemName Like('%" + _objHSNCode + "') OR HSNCode Like('" + _objHSNCode + "')";
            object obj = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(obj);
        }

        private void BindSaleReturnDetails(DataTable _dtDetails)
        {
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Clear();
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int rowIndex = 0;
                string strHSNCode = "", strBarCode = "";
                foreach (DataRow row in _dtDetails.Rows)
                {
                    if (MainPage._bTaxStatus)
                    {
                        //strHSNCode = GetHSNCode(Convert.ToString(row["HSNCode"]));
                        strBarCode = MainPage.strDataBaseFile;
                    }
                    else
                    {
                        strHSNCode = Convert.ToString(row["ItemName"]);
                        strBarCode = Convert.ToString(row["BarCode"]);
                    }
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["salesMan"].Value = row["SalesManName"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"]; // strHSNCode;
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = Math.Abs(ConvertObjectToDouble(row["SDisPer"]));
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = dba.ConvertObjectToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = dba.ConvertObjectToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                    dgrdDetails.Rows[rowIndex].Cells["hsnCode"].Value = row["HSNCode"];

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
                pnlTax.Visible = true;
            }
            else
                pnlTax.Visible = false;
        }

        private void SaleReturn_Retail_KeyDown(object sender, KeyEventArgs e)
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

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    string strCName = txtCustomerName.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strSalesParty = "SALESPARTY";
                        if (MainPage.strSoftwareType.Contains("RETAIL"))
                            strSalesParty = "CUSTOMERNAME";

                        SearchData objSearch = new SearchData(strSalesParty, "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtCustomerName.Text = objSearch.strSelectedData;
                            string strMobileNo = "", strStation = "";
                            bool _bStatus = dba.CheckTransactionLockWithMobileNoStation(txtCustomerName.Text, ref strMobileNo, ref strStation);
                            if (_bStatus)
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please select different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtCustomerName.Text = "";
                            }
                            else
                            {
                                if (strMobileNo != "" || strStation != "")
                                {
                                    txtMobileNo.Text = strMobileNo;
                                }
                            }
                        }
                    }
                    else if (value != 8)
                    {
                        e.Handled = true;
                    }
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

        private void txtBillNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearAllText();
            }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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
                        if (txtCustomerName.Text != "")
                        {
                            string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                            if (strCustomer != "")
                            {
                                string[] strFullName = txtCustomerName.Text.Split(' ');
                                if (strFullName.Length > 1)
                                    strQuery = " Where SalePartyID ='" + strFullName[0].Trim() + "'  ";
                            }
                            else
                                strQuery = " Where SalePartyID ='" + txtCustomerName.Text.Trim() + "'  ";
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
                                    if (rdoAll.Checked)
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

        private void GetSaleReturnBillDetails()
        {
            string strQuery = "";
            if (txtSaleBillNo.Text != "")
            {
                dgrdDetails.Rows.Clear();
                DataTable dt = null;
                strQuery = " Select *,ISNULL(dbo.GetFullName(SBS.SalesMan),'DIRECT')SaleManName,(Select Description_3 from SalesBook SB Where SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo)Description_3 from SalesBookSecondary  SBS Where BillCode='" + txtSaleBillCode.Text + "' and BillNo=" + txtSaleBillNo.Text + " Order by SID asc ";
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

                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                        dgrdDetails.Rows[rowIndex].Cells["id"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
                        dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                        dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                        dgrdDetails.Rows[rowIndex].Cells["salesMan"].Value = row["SaleManName"];
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
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                        rowIndex++;
                    }
                    CalculateAllAmount();
                }
            }
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 13 || e.ColumnIndex == 17 || e.ColumnIndex == 18)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        SearchData objSearch = new SearchData("SALESMANNAME", "SEARCH SALES MAN", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11)
                    {
                        if (!rdoManual.Checked)
                        {
                            string strType = "DESIGNNAMEWITHBARCODE_SALERETURN";
                            if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                                strType = "DESIGNNAMEWITHBARCODE_SALERETURN_RETAIL";
                            SearchCategory_Custom objSearch = new SearchCategory_Custom(OldYearDBName, "", strType, "", "", "", "", "", "", Keys.Space, false, false);
                            objSearch.ShowDialog();
                            GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);
                        }
                        else
                        {
                            string strType = "DESIGNNAMEWITHBARCODE_SALERETURN_MANUAL";
                            SearchCategory_Custom objSearch = new SearchCategory_Custom(OldYearDBName, "", strType, "", "", "", "", "", "", Keys.Space, false, false);
                            objSearch.ShowDialog();
                            GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 15 || e.ColumnIndex == 16 || e.ColumnIndex == 14)
                    {
                        if (!MainPage.strUserRole.Contains("ADMIN"))
                            e.Cancel = true;
                    }

                }
                else
                    e.Cancel = true;
            }
            catch
            {
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
                _dt.Columns.Add("BarCode", typeof(String));
                _dt.Columns.Add("BrandName", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("ItemName='" + row.Cells["itemName"].Value + "' and Variant1='" + row.Cells["variant1"].Value + "' and Variant2='" + row.Cells["variant2"].Value + "' and ISNULL(Variant3,'')='" + row.Cells["variant3"].Value + "' and ISNULL(Variant4,'')='" + row.Cells["variant4"].Value + "' and ISNULL(Variant5,'')='" + row.Cells["variant5"].Value + "' and BarCode='" + row.Cells["BarCode"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]), dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
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
                        _row["BarCode"] = row.Cells["barCode"].Value;
                        _row["BrandName"] = row.Cells["brandName"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
        }

        private bool ValidateStock()
        {
            DataTable _dt = GenerateDistinctItemName();
            bool _bStatus = dba.CheckSaleQtyFroSaleReturn(_dt, txtBillCode.Text, txtBillNo.Text, dgrdDetails, lblMsg);
            if (!_bStatus && MainPage.strUserRole.Contains("SUPERADMIN"))
                _bStatus = true;
            return _bStatus;
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
                            string strBarcode = "";
                            if (strAllItem.Length > 0)
                            {
                                strBarcode = strAllItem[0].Trim();
                                string[] str = strBarcode.Split('.');

                                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = str[0];
                                dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                                dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[2];
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[3];

                                if (MainPage.StrCategory1 != "" && strAllItem.Length > 7)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[4];
                                    columnIndex++;
                                }
                                if (MainPage.StrCategory2 != "" && strAllItem.Length > 8)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[5];
                                    columnIndex++;
                                }
                                if (MainPage.StrCategory3 != "" && strAllItem.Length > 9)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[6];
                                    columnIndex++;
                                }
                                if (MainPage.StrCategory4 != "" && strAllItem.Length > 10)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[7];
                                    columnIndex++;
                                }
                                if (MainPage.StrCategory5 != "" && strAllItem.Length > 11)
                                {
                                    columnIndex++;
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[8];
                                }

                                dRate = dba.ConvertObjectToDouble(strAllItem[strAllItem.Length - 2]);

                                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = strAllItem[strAllItem.Length - 3];

                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[strAllItem.Length - 1];
                                dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = dgrdDetails.Rows[rowIndex].Cells["rate"].Value = dRate;
                                if (str.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strBarcode;

                                SetUnitNameMRP(dgrdDetails.Rows[rowIndex], dRate);

                                if (Convert.ToString(dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["ItemName"].Value) != "")
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                    if (dgrdDetails.Rows.Count > 1)
                                        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                    dgrdDetails.Focus();
                                }

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

        private void SetUnitNameMRP(DataGridViewRow row, double dRate)
        {
            try
            {
                string strBrandName = Convert.ToString(row.Cells["brandName"].Value), strStyleName = Convert.ToString(row.Cells["styleName"].Value), strItem = Convert.ToString(row.Cells["itemName"].Value), strVariant1 = Convert.ToString(row.Cells["variant1"].Value), strVariant2 = Convert.ToString(row.Cells["variant2"].Value);
                string strBarcode = Convert.ToString(row.Cells["barcode"].Value),strBarcode_S = Convert.ToString(row.Cells["barcode_s"].Value);
                string strQuery = "Select MRP,SDisPer,UnitName,isnull(dbo.GetFullName(SalesMan),'DIRECT') SalesManName from SalesBookSecondary Where BrandName='" + strBrandName + "' and DesignName='" + strStyleName + "' and ItemName='" + strItem + "' and Variant1='" + strVariant1 + "' and Variant2='" + strVariant2 + "' and Rate=" + dRate;


                if (rdoAll.Checked) // Check How Many sale bill having this Item -- For Only First Item
                {
                    if (row.Index == 0)
                        strQuery += " Select (CAST(SBS.BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURNRETAIL,SBS.BillCode,SBS.BillNo, isnull(dbo.GetFullName(SBS.SalesMan),'DIRECT') SalesManName from SalesBookSecondary  SBS LEFT JOIN SalesBook SB on SBS.BillCode = SB.BillCode And SBS.BillNo = SB.BillNo Where SBS.Barcode = '" + strBarcode + "' AND SBS.Barcode_S = '" + strBarcode_S + "'";
                    else if (row.Index > 0 && txtBillNo.Text != "")
                        strQuery += " Select (CAST(SBS.BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURNRETAIL,SBS.BillCode,SBS.BillNo, isnull(dbo.GetFullName(SBS.SalesMan),'DIRECT') SalesManName from SalesBookSecondary  SBS LEFT JOIN SalesBook SB on SBS.BillCode = SB.BillCode And SBS.BillNo = SB.BillNo WHERE SB.BillCode = '" + txtSaleBillCode.Text + "' AND SB.BillNo = " + txtSaleBillNo.Text + " AND SBS.Barcode = '" + strBarcode + "' AND SBS.Barcode_S = '" + strBarcode_S + "'";
                }


                DataSet ds = dba.GetDataSet(strQuery);
                DataTable _dt = ds.Tables[0];
                if (_dt.Rows.Count > 0)
                {
                    row.Cells["mrp"].Value = _dt.Rows[0]["MRP"];
                    row.Cells["disPer"].Value = _dt.Rows[0]["SDisPer"];
                    row.Cells["unitName"].Value = _dt.Rows[0]["UnitName"];
                    row.Cells["salesMan"].Value = _dt.Rows[0]["SalesManName"];
                }

                // Check How Many sale bill having this Item -- For Only First Item
                DataTable dt2 = new DataTable();
                if (row.Index == 0 && ds.Tables.Count > 1)
                {
                    dt2 = ds.Tables[1];
                    if (dt2.Rows.Count < 1)
                    {
                        MessageBox.Show("Sorry ! This Item ( " + row.Cells["ItemName"].Value + " ) is not from Any Sale Bill.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgrdDetails.Rows.Remove(row);
                        txtSaleBillNo.Text = "";

                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[3];
                        dgrdDetails.Enabled = true;
                    }
                    else if(dt2.Rows.Count == 1)
                    {
                        row.Cells["salesMan"].Value = dt2.Rows[0]["SalesManName"];
                        string[] strBillNo = Convert.ToString(dt2.Rows[0]["SALEBILLNOFORRETURNRETAIL"]).Split('|');
                        txtSaleBillNo.Text = strBillNo[0];
                        txtSaleBillCode.Text = Convert.ToString(dt2.Rows[0]["BillCode"]);
                    }
                    else
                    {
                        SearchData objSearch = new SearchData("", "", "SEARCH SALE BILL NO", Keys.Space);
                        objSearch.table = dt2;

                        if (objSearch.table != null)
                        {
                            foreach (DataRow ro in objSearch.table.Rows)
                            {
                                objSearch.lbSearchBox.Items.Add(ro[0]);
                            }
                        }
                        objSearch.strSearchData = "SALEBILLNOFORRETURNRETAIL";
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            string[] strBillNo = objSearch.strSelectedData.Split('|');
                            txtSaleBillNo.Text = strBillNo[0];
                            if (strBillNo.Length > 1)
                                txtSaleBillDate.Text = strBillNo[1];
                        }
                        else
                        {
                            string[] strBillNo = Convert.ToString(objSearch.lbSearchBox.Items[0]).Split('|');
                            txtSaleBillNo.Text = strBillNo[0];
                            txtSaleBillDate.Text = strBillNo[1];
                        }
                    }
                }
                else if (row.Index > 0 && ds.Tables.Count > 1)
                {
                    // Check if this Item is belogning to the Sale bill Selected by First Item For Only Next All Items
                    dt2 = ds.Tables[1];
                    if (dt2.Rows.Count > 0)
                    {
                        row.Cells["salesMan"].Value = dt2.Rows[0]["SalesManName"];
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! This Item ( " + row.Cells["ItemName"].Value + " ) is not from Selecetd Sale Bill - " + txtBillCode.Text + " " + txtSaleBillNo.Text , "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgrdDetails.Rows.Remove(row);
                    }
                }
            }
            catch(Exception ex)
            {
                string[] strReport = { "CHECKING ITEM IN SALE BILL : Sale Return Retail", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // btnEdit.Enabled = btnAdd.Enabled = false;
            }
        }

        private void CalculateSpecialDiscount()
        {
            try
            {
                double dMRP = 0, dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0;

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);

                    dDisPer = Math.Abs(dDisPer);

                    if ((dDisPer != 0) && dMRP != 0)
                        dRate = dMRP * (100.00 - (dDisPer)) / 100.00;
                    if (dRate == 0)
                        dRate = dMRP;

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                }
            }
            catch
            {
            }
        }

        private void CalculateAllAmount()
        {
            try
            {
                CalculateSpecialDiscount();

                double dFinalAmt = 0, dQty = 0, dDisPer = 0, dTOAmt = 0, dSaleReturnAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dTaxableAmt = 0, dNetAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dRoundOff = 0, dPaybleAmt = 0;
                double dCashAmt = 0;

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                }

                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dSaleReturnAmt = dba.ConvertObjectToDouble(txtSaleReturnAmt.Text);
                dDisPer = ConvertObjectToDouble(txtDiscPer.Text);
                dDiscAmt = (dBasicAmt * dDisPer) / 100;

                dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                dTOAmt = dOtherAmt - dDiscAmt;
                dFinalAmt = dBasicAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTOAmt - dSaleReturnAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));// Math.Round(dNetAmt, 0);
                dRoundOff = dNNetAmt - dNetAmt;

                dPaybleAmt = dNNetAmt - dCashAmt;

                if (dRoundOff >= 0)
                {
                    txtROSign.Text = "+";
                    txtRoundOff.Text = dRoundOff.ToString("0.00");
                }
                else
                {
                    txtROSign.Text = "-";
                    txtRoundOff.Text = Math.Abs(dRoundOff).ToString("0.00");
                }

                dPaybleAmt = Math.Round(dPaybleAmt, 0);
                txtDisAmt.Text = dDiscAmt.ToString("N2", MainPage.indianCurancy);
                txtTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                txtNetAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0, dServiceAmt = 0, dInsuranceAmt = 0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtSalesType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtSalesType.Text, "SALES");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);

                        string strTaxationType = Convert.ToString(row["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                                _strTaxType = "INCLUDED";


                            string strQuery = "", strSubQuery = "", strGRSNo = "", strTaxRate = "";
                            double dDisStatus = 0;

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;

                            double dRate = 0, dQty = 0, dAmt = 0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);

                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
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
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount," + dTaxPer + " as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = "  Select SUM(TaxableAmt)TaxableAmt,SUM(ROUND(Amt,4)) as Amt,SUM(ROUND(Amt,2)) as TaxAmt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) ServiceAmt from (Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by HSNCode,TaxRate)_Sales  Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    // BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    dServiceAmt = dba.ConvertObjectToDouble(dt.Rows[0]["ServiceAmt"]);
                                    dTaxAmt = dTTaxAmt;
                                    if (dOtherAmt == 0)
                                        dTaxPer = dMaxRate;
                                    pnlTax.Visible = true;
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


        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {//
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
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

        private void EnableAllControls()
        {
            txtMobileNo.ReadOnly = txtDate.ReadOnly = txtSaleBillDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtCashAmt.ReadOnly = txtDiscPer.ReadOnly = txtTaxPer.ReadOnly = false;

            dgrdDetails.ReadOnly = false;
            grpQtr.Enabled = true;
            if (MainPage.strUserRole.Contains("ADMIN"))
                txtTaxPer.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtMobileNo.ReadOnly = txtDate.ReadOnly = txtSaleBillDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtCashAmt.ReadOnly = txtDiscPer.ReadOnly = txtTaxPer.ReadOnly = true;
            dgrdDetails.ReadOnly = true;
            lblMsg.Text = lblCreatedBy.Text = "";
        }

        private void ClearAllText()
        {
            txtMobileNo.Text = txtAttachBill.Text = txtCustomerName.Text = txtSalesType.Text = txtSalesType.Text = txtRemark.Text = txtSaleBillNo.Text = "";
            txtCashAmt.Text = txtDiscPer.Text = txtDisAmt.Text = txtRoundOff.Text = lblTaxableAmt.Text = txtTaxAmt.Text = txtOtherAmount.Text = txtTotalQty.Text = txtNetAmt.Text = txtGrossAmt.Text = txtSaleReturnAmt.Text = "0.00";
            txtSign.Text = txtROSign.Text = "+";
            txtTaxPer.Text = "18.00";

            rdoAll.Checked = rdoCurrent.Checked = true;
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = false;
            lblCreatedBy.Text = strDeletedSID = "";
            pnlTax.Visible = false;
            dOldCashAmt = dOldNetAmt = 0;
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
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(GRBillNo)+1,1) from MaxSerialNo)BillNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='SALES' and TaxIncluded=1) TaxName  from [SaleReturn] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        int receiptNo = Convert.ToInt32(table.Rows[0]["SNo"]), maxReceiptNo = Convert.ToInt32(table.Rows[0]["BillNo"]);
                        if (receiptNo > maxReceiptNo)
                            txtBillNo.Text = Convert.ToString(receiptNo);
                        else
                            txtBillNo.Text = Convert.ToString(maxReceiptNo);

                        if (MainPage._bTaxStatus)
                            txtSalesType.Text = Convert.ToString(table.Rows[0]["TaxName"]);
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
            if (MainPage._bTaxStatus)
            {
                if (!dba.GetBillNextPrevRecord("SALE RETURN", txtBillCode.Text, txtBillNo.Text, txtDate))
                    return false;
            }

            if (txtSalesType.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Sorry ! Sales type can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesType.Focus();
                return false;
            }
            double dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text), dCreditSale = dba.ConvertObjectToDouble(txtNetAmt.Text);
            if (dCreditSale > 0)
            {
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                if (strCustomer == "")
                {
                    MessageBox.Show("Sorry ! Please enter register customer name for cedit sale ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return false;
                }
            }
            if (dCashAmt > 0)
            {
                string strQuery = "Select (Select Top 1 (AreaCode+AccountNo)AccountNo from SupplierMaster Where Category='CARD SALE')CardSale,(Select Top 1 (AreaCode+AccountNo)AccountNo from SupplierMaster Where Category='CASH SALE')CashSale ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    //if (dCardAmt > 0 && Convert.ToString(dt.Rows[0]["CardSale"]) == "")
                    //{
                    //    MessageBox.Show("Sorry ! Please create card sale account in account master! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return false;
                    //}
                    if (dCashAmt > 0 && Convert.ToString(dt.Rows[0]["CashSale"]) == "")
                    {
                        MessageBox.Show("Sorry ! Please create account with 'CASH SALE' as category in account master! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            //CalculateAllAmount();

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(rows.Cells["itemName"].Value);
                double dAmount = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                if (strItem == "" && dAmount == 0)
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
                    else if (dAmount == 0)
                    {
                        MessageBox.Show("Sorry ! Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["qty"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                MessageBox.Show("Sorry ! Please add atleast one entry in table ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;// ValidateStock();
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            //DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            //DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSalesType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            //if (dt.Rows.Count > 0)
            //{
            //    if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
            //    {
            //        MessageBox.Show("Transaction has been locked on this Account : " + txtCustomerName.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }

            //    if (btnEdit.Text == "&Update" || _bUpdateStatus)
            //    {
            //        if (strOldPartyName != txtCustomerName.Text || dOldNetAmt != Convert.ToDouble(txtNetAmt.Text) || _bUpdateStatus)
            //        {
            //            if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
            //            {
            //                bool iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);

            //                if (!iStatus && MainPage.strOnlineDataBaseName != "")
            //                {
            //                    bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtBillCode.Text + " " + txtBillNo.Text);
            //                    if (!netStatus)
            //                    {
            //                        MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                        return false;
            //                    }
            //                }
            //                else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
            //                {
            //                    MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                    return false;
            //                }

            //            }
            //            else
            //            {
            //                MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                return false;
            //            }
            //        }
            //    }
            //    if (!_bUpdateStatus)
            //    {
            //        string strRegion = Convert.ToString(dt.Rows[0]["Region"]), strCStateName = Convert.ToString(dt.Rows[0]["CStateName"]).ToUpper(), strSStateName = Convert.ToString(dt.Rows[0]["SStateName"]).ToUpper();
            //        if (strRegion != "")
            //        {
            //            if (strRegion == "LOCAL" && strSStateName != strCStateName)
            //            {
            //                MessageBox.Show("You are entering a central transaction for a party belonging to same state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                //if (result == DialogResult.Yes)
            //                //    return true;
            //                //else
            //                return false;
            //            }
            //            else if (strRegion == "INTERSTATE" && strSStateName == strCStateName)
            //            {
            //                MessageBox.Show("You are entering a local  transaction for a party belonging to other  state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                //if (result == DialogResult.Yes)
            //                //    return true;
            //                //else
            //                return false;
            //            }
            //        }
            //    }
            //    //if (Convert.ToString(dt.Rows[0]["IncludeStatus"]) == "DENY")
            //    //{
            //    //    MessageBox.Show("Sorry Sale type and purchase type doesn't match in tax inclusion!\nPlease enter correct purchase type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    //    return false;
            //    //}
            //}
            //else
            //{
            //    MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            return true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

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
                                string strQuery = "Declare @BillCodeNo varchar(50) SELECT @BillCodeNo = (BillCode +' '+ Cast(BillNo as Varchar(20))) FROM SalesBook where ReturnSlipNo  = '" + txtBillCode.Text + " " + txtBillNo.Text +"' IF(ISNULL(@BillCodeNo,'') = '') BEGIN "
                                                + " Delete from [SaleReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                                + " Delete from [SaleReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " Delete from BalanceAmount Where AccountStatus in ('SALE RETURN','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                                + " Delete from [dbo].[StockMaster] Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') "
                                                + " SELECT '0' BillCodeNo END ELSE BEGIN SELECT ISNULL(@BillCodeNo,'') BillCodeNo END";

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    string Code = Convert.ToString(dt.Rows[0]["BillCodeNo"]);
                                    if (Code == "0")
                                    {
                                        DataBaseAccess.CreateDeleteQuery(strQuery);
                                        MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                        txtReason.Text = "";
                                        pnlDeletionConfirmation.Visible = false;
                                        BindNextRecord();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sorry ! This Return bill used in Sale Bill (" + Code + ") !  ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        txtReason.Text = "";
                                        pnlDeletionConfirmation.Visible = false;
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Record not deleted due to " + ex.Message + ", Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnFinalDelete.Enabled = true;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtCustomerName_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtCustomerName.Text);
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

        private void SaveRecord()
        {
            try
            {
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT", strImportData = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                double dSaleReturnAmt = 0, dCashAmt = 0, dCreditAmt = 0, dFinalAmt = 0, dFOtherAmt = 0, dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text);
                string strSaleParty = "", strSalePartyID = "", strSubPartyID = "", strTickStatus = "False";
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                bool _bRegistered = false;
                if (strCustomer != "")
                {
                    string[] _strFullName = txtCustomerName.Text.Split(' ');
                    if (_strFullName.Length > 1)
                    {
                        strSalePartyID = _strFullName[0].Trim();
                        strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                        _bRegistered = true;
                    }
                }
                else
                    strSalePartyID = strSaleParty = txtCustomerName.Text;

                if (rdoOldYear.Checked)
                    strQtrStatus = "PREVIOUS";

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dSaleReturnAmt = dba.ConvertObjectToDouble(txtSaleReturnAmt.Text);

                dFOtherAmt = _dOtherAmt;
                if (txtSign.Text == "-")
                    dFOtherAmt = (dFOtherAmt) * -1;

                dFinalAmt = dCreditAmt + dCashAmt - dSaleReturnAmt;

                if (dFinalAmt == dCashAmt)
                    strTickStatus = "True";

                string strQuery = " if not exists (Select BillCode from SaleReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[SaleReturn] ([BillCode],[BillNo],[Date],[SalePartyID],[MobileNo],[SubPartyID],[SaleBillCode],[SaleBillNo],[EntryType],[SaleType],[Remark],[OtherSign],[OtherAmt],[PackingAmt],[NetDiscount],[ServiceAmt],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleBillDate],[DiscountType],[RoundOffSign],[RoundOffAmt],[Description1],[Description2],[Description3],[CashAmt],[CreditAmt],[ReturnType],[TaxableAmt],[PartSaleReturnAmt]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + txtMobileNo.Text + "','" + strSubPartyID + "','" + txtSaleBillCode.Text + "','" + txtSaleBillNo.Text + "','" + GetEntryType() + "','" + txtSalesType.Text + "','" + txtRemark.Text + "','" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",0,"
                               + " " + dba.ConvertObjectToDouble(txtDisAmt.Text) + ",0," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dTaxAmt + "," + dba.ConvertObjectToDouble(txtTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + ",'" + strQtrStatus + "','" + txtDiscPer.Text + "','" + MainPage.strLoginName + "','',1,0,'" + strSDate + "',0,'" + txtROSign.Text + "'," + ConvertObjectToDouble(txtRoundOff.Text) + ",'','','" + strImportData + "'," + dCashAmt + "," + dCreditAmt + ",'RETAIL'" + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + "," + dSaleReturnAmt + ")  ";
                if (_bRegistered)
                {
                    strQuery += " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                            + " ('" + strDate + "','" + strSaleParty + "','SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dFinalAmt + "','CR','0','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "') ";

                    if (dCashAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + strDate + "',@CashName,'CASH PAID','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + strDate + "','" + strSaleParty + "','CASH PAID','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName)  ";
                    }
                }
                else if (dCashAmt > 0)
                {
                    strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CashName,'SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'CASH PAID')  ";
                }

                //if (dCreditAmt > 0)
                //{                  
                //    strQuery += "  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                //                    + " ('" + strDate + "','" + strSaleParty + "','SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCreditAmt + "','DR','" + dCreditAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "','CREDIT SALE')  ";
                //}

                double dQty = 0, dRate = 0, dMRP = 0, dAmt = 0, dSDis = 0;
                string strSalesMan = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {

                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                    dSDis = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dSDis = Math.Abs(dSDis) * -1;
                    strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                    if (strSalesMan != "")
                    {
                        string[] str = strSalesMan.Split(' ');
                        strSalesMan = str[0];
                    }

                    strQuery += " INSERT INTO [dbo].[SaleReturnDetails]([RemoteID],[BillCode],[BillNo],[PurchaseBillNo],[PurchasePartyID],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus],[UnitName],[ItemStatus],[Disc],[BarCode],[BrandName],[DesignName],[SalesMan],[Other2],[DisStatus],[Discount],[Dhara],[BarCode_S]) VALUES "
                                  + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dSDis + "," + dRate + ","
                                  + " " + dAmt + ",0,0,0," + dAmt + ",0,'',1,0,'" + row.Cells["unitName"].Value + "','FRESH',0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + strSalesMan + "','','+',0,'','" + row.Cells["barcode_s"].Value + "')";

                    if (MainPage._bTaxStatus || strImportData == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                    }

                    strQuery += " if not exists(select ParentBarCode from BarCodedetails where ParentBarCode = '" + row.Cells["barcode"].Value + "' and BarCode = '" + row.Cells["barcode_s"].Value + "') begin "
                       + " INSERT INTO [dbo].[BarcodeDetails] ([BillCode],[BillNo],[ParentBarCode],[BarCode],[NetQty],[SetQty],[LastPrintNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[InStock]) values('" + txtBillCode.Text+"',"+txtBillNo.Text+", '" + row.Cells["barcode"].Value + "', '" + row.Cells["barcode_s"].Value + "', " + dQty + ", 1, 1, '" + MainPage.strLoginName + "', '',1,0,1) end ";
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                if (dTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                //GST Details
                string strTaxAccountID = "";
                string[] strFullName;
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

                //foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                //{
                //    strQuery += " INSERT INTO [dbo].[CardDetails]([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus])VALUES "
                //                   + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0) ";// end ";
                //}

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                strQuery += " end ";

                if (strQuery != "")
                {
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {

                        MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        //AskForPrint();
                        BindRecordWithControl(txtBillNo.Text);
                    }
                    else
                        MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            //string strMobileNo = "", strPath = "";
                            //SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                            //SendSMSToParty(strMobileNo);

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
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT", strImportData = "";// txtImportData.Text;
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtSaleBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
                bool _bRegistered = false;
                string strSaleParty = "", strSalePartyID = "", strSubPartyID = "", strTaxAccountID = "", strDeletedSIDQuery = "";
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                string[] _strFullName;
                if (strCustomer != "")
                {
                    _strFullName = txtCustomerName.Text.Split(' ');
                    if (_strFullName.Length > 1)
                    {
                        strSalePartyID = _strFullName[0].Trim();
                        strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                        _bRegistered = true;
                    }
                }
                else
                    strSalePartyID = strSaleParty = txtCustomerName.Text;

                if (rdoOldYear.Checked)
                    strQtrStatus = "PREVIOUS";

                double dCashAmt = 0, dCreditAmt = 0, dFinalAmt = 0, dFOtherAmt = 0, dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dSaleReturnAmt = ConvertObjectToDouble(txtSaleReturnAmt.Text);

                double dRate = 0, dAmt = 0, dQty = 0, dMRP = 0, dSDis = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "", strID = "", strSalesMan = "", strTickStatus = "False", strTickQuery = "";

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dFOtherAmt = _dOtherAmt;
                if (txtSign.Text == "-")
                    dFOtherAmt = (dFOtherAmt) * -1;
                dFinalAmt = dCreditAmt + dCashAmt - dSaleReturnAmt;

                if (dOldCashAmt == dOldNetAmt && dFinalAmt != dCashAmt)
                {
                    strTickQuery = ",[Tick]='False' ";
                    strTickStatus = "False";
                }

                if (dFinalAmt == dCashAmt)
                {
                    strTickQuery = ",[Tick]='True' ";
                    strTickStatus = "True";
                }

                strQuery += "UPDATE  [dbo].[SaleReturn]  SET [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[MobileNo]='" + txtMobileNo.Text + "',[SubPartyID]='" + strSubPartyID + "',[SaleBillCode]='" + txtSaleBillCode.Text + "',[SaleBillNo]='" + txtSaleBillNo.Text + "',[EntryType]='" + GetEntryType() + "',[AttachedBill]='" + txtAttachBill.Text + "',[SaleType]='" + txtSalesType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PackingAmt]=0,[OtherText]='" + strQtrStatus + "',[PartSaleReturnAmt]=" + dSaleReturnAmt + ", "
                               + " [NetDiscount]=" + dba.ConvertObjectToDouble(txtDisAmt.Text) + ",[ServiceAmt]=0,[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(txtTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[NetAmt]=" + dFinalAmt + ",[OtherValue]='" + txtDiscPer.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SaleBillDate]='" + strSDate + "',[Description3]='" + strImportData + "',[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + ConvertObjectToDouble(txtRoundOff.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[CashAmt]=" + dCashAmt + ",[CreditAmt]=" + dCreditAmt + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                               // + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strSalePartyID + "' Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                               + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                               + " Delete from [dbo].[GSTDetails] Where [BillType]='SALERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                               + " Delete from StockMaster Where BillType='SALERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                if (_bRegistered)
                {
                    strQuery += " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]='" + dFinalAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strSalePartyID + "' " + strTickQuery + " Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' ";
                    if (dCashAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CASH RECEIVE' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "') begin "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                  + " ('" + strDate + "',@CashName,'CASH PAID','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                  + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                  + " ('" + strDate + "','" + strSaleParty + "','CASH PAID','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName) end else begin "
                                  + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CASH PAID'  and Status='CREDIT' "
                                  + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' " + strTickQuery + " Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CASH PAID'  and Status='DEBIT' "
                                  + " Delete from BalanceAmount Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CASH PAID' end ";

                    }
                }
                else if (dCashAmt > 0)
                {
                    strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CASH PAID' ) begin "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CashName,'SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'CASH PAID') end else begin "
                                    + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALE RETURN'  and [AccountStatusID]='CASH PAID'  end";
                }
                else
                    strQuery += " Delete from BalanceAmount Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CASH PAID' ";

                //if (dCreditAmt > 0)
                //{
                //    strQuery += "  if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ) begin "
                //                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                //                   + " ('" + strDate + "','" + strSaleParty + "','SALE RETURN','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCreditAmt + "','DR','" + dCreditAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "','CREDIT SALE')  end else begin "
                //                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSalePartyID + "',[Amount]=" + dCreditAmt + ",[FinalAmount]='" + dCreditAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALE RETURN'  and [AccountStatusID]='CREDIT SALE' end ";
                //}
                //else
                //    strQuery += " Delete from BalanceAmount Where [AccountStatus]='SALE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ";


                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strID = Convert.ToString(row.Cells["id"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                    dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dSDis = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                    if (strSalesMan != "")
                    {
                        string[] str = strSalesMan.Split(' ');
                        strSalesMan = str[0];
                    }
                    dSDis = Math.Abs(dSDis) * -1;

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SaleReturnDetails]([RemoteID],[BillCode],[BillNo],[PurchaseBillNo],[PurchasePartyID],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[PurchaseReturnStatus],[PurchaseReturnNumber],[InsertStatus],[UpdateStatus],[UnitName],[ItemStatus],[Disc],[BarCode],[BrandName],[DesignName],[SalesMan],[Other2],[DisStatus],[Discount],[Dhara],[BarCode_S]) VALUES "
                                 + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dSDis + "," + dRate + ","
                                 + " " + dAmt + ",0,0,0," + dAmt + ",0,'',1,0,'" + row.Cells["unitName"].Value + "','FRESH',0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + strSalesMan + "','','+',0,'','" + row.Cells["barcode_s"].Value + "')";
                    }
                    else
                    {
                        strQuery += " UPDATE [dbo].[SaleReturnDetails] Set [ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + dSDis + ",[Rate]=" + dRate + ",[Amount]=" + dAmt + ",[TotalAmt]= " + dAmt + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdateStatus]=1,[SalesMan]='" + strSalesMan + "',[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[BarCode_S]='" + row.Cells["barcode_s"].Value + "' Where [SID]=" + strID + " and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";
                    }


                    if (MainPage._bTaxStatus || strImportData == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                    }

                    strQuery += " if not exists(select ParentBarCode from BarCodedetails where ParentBarCode = '" + row.Cells["barcode"].Value + "' and BarCode = '" + row.Cells["barcode_s"].Value + "') begin "
                      + " Insert into BarCodeDetails values('FORWARDED', '0', '" + row.Cells["barcode"].Value + "', '" + row.Cells["barcode_s"].Value + "', " + dQty + ", 1, 1, '" + MainPage.strLoginName + "', '', '1', '0', '1') end ";
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
                    _strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strTaxAccountID = _strFullName[0].Trim();
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
                       + "('SALERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";



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

        private string CreatePDFFile(bool _createPDF,ref bool Printed)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strPath = "";
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
                    if(_browser.ShowDialog() == DialogResult.OK)
                    {
                        if (_browser.FileName != "")
                            strPath = _browser.FileName;
                        Printed = true;
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
                    Printed = true;
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
                Printed = false;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return strPath;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
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
                    string strPath = CreatePDFFile(true,ref Printed);
                    if (Printed)
                        MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
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
                string[] strReport = { "Exception occurred in Preview  in Retail Sales Return ", ex.Message };
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
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
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

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 12)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 15 || e.ColumnIndex == 14)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 16)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    //else if (e.ColumnIndex == 15)
                    //    CalculateAmountWithDiscOtherChargese(dgrdDetails.Rows[e.RowIndex]);
                }
            }
            catch
            {
            }
        }


        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            // rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
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
                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;
                dRate = Math.Round(dRate, 2);

                rows.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        private void CalculateDisWithAmountMRP(DataGridViewRow rows)
        {

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);


                if (dRate != 0 && dMRP != 0)
                    dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                rows.Cells["disPer"].Value = dDisPer * -1;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex > 11)
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
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex > 11)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
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
                        if (IndexColmn < dgrdDetails.ColumnCount - 3)
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

                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["barCode"].Value) != "" && IndexColmn == 10)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                if (dgrdDetails.Rows.Count > 1)
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                dgrdDetails.Focus();
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "" && dAmt > 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                if (dgrdDetails.Rows.Count > 1)
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtOtherAmount.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            txtSaleBillNo.Text = "";
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);//,strSONumber = Convert.ToString(dgrdDetails.CurrentRow.Cells["soNumber"].Value);
                        if (strID != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (strDeletedSID != "")
                                    strDeletedSID += ",";
                                strDeletedSID += strID;
                                DeleteCurrentRow();
                            }
                        }
                        else
                        {
                            DeleteCurrentRow();
                        }
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 3)// && colIndex != 10 && colIndex != 13 && colIndex != 15 && colIndex != 18)
                            dgrdDetails.CurrentCell.Value = "";
                        //if (colIndex == 9 || colIndex == 14)
                        //{
                        //    CalculateAmountWithQtyRate(dgrdDetails.CurrentRow);
                        //    CalculateAllAmount();
                        //}
                    }
                }
            }
            catch { }
        }

        private void DeleteCurrentRow()
        {
            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
            if (dgrdDetails.Rows.Count == 0)
            {
                txtSaleBillNo.Text = "";
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
                dgrdDetails.Enabled = true;
            }
            else
            {
                ArrangeSerialNo();
            }
            CalculateAllAmount();
        }

        private void DeleteOneRow(string strID)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    string strQuery = " Delete from SaleReturnDetails Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and [SID]=" + strID + " ";
                    int _index = dgrdDetails.CurrentRow.Index;
                    dgrdDetails.Rows.RemoveAt(_index);
                    CalculateAllAmount();
                    // if (ValidateControls())
                    {
                        int result = UpdateRecord(strQuery);
                        if (result < 1)
                            BindRecordWithControl(txtBillNo.Text);
                        else
                        {
                            strQuery = " Delete from SaleReturnDetails Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " ";
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                                dgrdDetails.Enabled = true;
                            }
                            else
                                ArrangeSerialNo();
                        }

                        dgrdDetails.ReadOnly = false;
                    }
                }

            }
            catch
            {
            }
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSign.Text == "")
                    txtSign.Text = "+";
                CalculateAllAmount();
            }
        }

        private void txtOtherAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtOtherAmount_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtOtherAmount.Text == "")
                    txtOtherAmount.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
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
            try
            {
                txtSaleBillCode.Text = txtSaleBillNo.Text = "";
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCashAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtCashAmt.Text == "")
                    txtCashAmt.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtOtherAmount_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtOtherAmount.Text == "0.00")
                    txtOtherAmount.Text = "";
            }
        }

        private void txtSaleBillDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, false);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtCashAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtCashAmt.Text == "0.00")
                    txtCashAmt.Text = "";
            }
        }

        private void btnGenSepBill_Click(object sender, EventArgs e)
        {
            btnGenSepBill.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && dgrdDetails.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to generate seperate invoice?", "Confimation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = SaveSaleRecord();
                        if (count > 0)
                        {
                            if (MainPage.bHSNWisePurchase)
                                CalculateAllAmount();
                            count = UpdateRecord("");
                            //string strQry = " Update SalesBook Set AttachedBill = '" + txtAttachBill.Text + "' WHERE BillCode = '" + txtBillCode.Text + "' And BillNo = '" + txtBillNo.Text + "'";
                            //if (dba.ExecuteMyQuery(strQry) > 0)
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! Invoice generated successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                            MessageBox.Show("Sorry ! Unable to generate invoice, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtAttachBill.Focus();
                        txtAttachBill.SelectionStart = 0;
                        txtAttachBill.SelectionLength = txtAttachBill.TextLength;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            btnGenSepBill.Enabled = true;
        }
        private int SaveSaleRecord()
        {
            _strAttachBillWithComma = "";
            int _count = 0, result = 0;
            string _strAttachBill = "";
            DataTable _dt = CreateSecondaryDataTable();
            bool _bInclude = true;
            if (txtSalesType.Text.Contains("EXCLUDE"))
                _bInclude = false;

            double dAllNetAmt = 0;
            string strSalePartyID = "", strSaleParty = "";
            string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
            bool _bRegistered = false;
            if (strCustomer != "")
            {
                string[] _strFullName = txtCustomerName.Text.Split(' ');
                if (_strFullName.Length > 1)
                {
                    strSalePartyID = _strFullName[0].Trim();
                    strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                    _bRegistered = true;
                }
            }
            else
                strSalePartyID = strSaleParty = txtCustomerName.Text;

            if (_dt.Rows.Count > 0)
            {
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string strQuery = "", strSalesMan = "", strCompanyCode = "", strHSNCode = "", strHSNQuery = "", strBrandName = "", strDesignName = "", strItemName = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "", strBarCode = "", strBarCode_S = "";
                double dAmt = 0, dGrossAmt = 0, dNetAmt = 0, dQty = 0, dTQty = 0, dRate = 0, _dDisPer = 0, dMRP = 0, dCashAmt = 0;
                double dOtherAmt = 0, dCreditAmt = 0;
                dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);

                DataTable _dtCompany = _dt.DefaultView.ToTable(true, "CompanyCode");
                foreach (DataRow row in _dtCompany.Rows)
                {
                    strCompanyCode = Convert.ToString(row["CompanyCode"]);

                    DataRow[] _rows = _dt.Select("CompanyCode='" + strCompanyCode + "'");
                    int _index = 1;
                    _dDisPer = dGrossAmt = dTQty = dNetAmt = 0;
                    strQuery = "";

                    foreach (DataRow _dr in _rows)
                    {
                        strBrandName = strDesignName = strItemName = strVariant1 = strVariant2 = strVariant3 = strVariant4 = strVariant5 = "";

                        dGrossAmt += dAmt = dba.ConvertObjectToDouble(_dr["amount"]);
                        dTQty += dQty = dba.ConvertObjectToDouble(_dr["qty"]);
                        dRate = ConvertObjectToDouble(_dr["rate"]);
                        dMRP = dba.ConvertObjectToDouble(_dr["mrp"]);
                        _dDisPer = ConvertObjectToDouble(_dr["disPer"]);
                        strHSNCode = Convert.ToString(_dr["HSNCode"]);
                        strSalesMan = Convert.ToString(_dr["salesMan"]);
                        if (strSalesMan != "")
                        {
                            string[] str = strSalesMan.Split(' ');
                            strSalesMan = str[0];
                        }
                        if (MainPage.bHSNWisePurchase)
                        {
                            strHSNQuery = " Select Top 1 @ItemName=ItemName from Items WHere ItemName Like('%" + strHSNCode + "') ";
                            strItemName = "@ItemName";
                            strBarCode = strCompanyCode;
                            strBarCode_S = "";
                        }
                        else
                        {
                            strItemName = "'" + _dr["itemName"] + "'";
                            strBarCode = Convert.ToString(_dr["barCode"]);
                            strBarCode_S = Convert.ToString(_dr["barcode_s"]);
                            strBrandName = Convert.ToString(_dr["brandName"]);
                            strDesignName = Convert.ToString(_dr["styleName"]);
                            strVariant1 = Convert.ToString(_dr["variant1"]);
                            strVariant2 = Convert.ToString(_dr["variant2"]);
                            strVariant3 = Convert.ToString(_dr["variant3"]);
                            strVariant4 = Convert.ToString(_dr["variant4"]);
                            strVariant5 = Convert.ToString(_dr["variant5"]);
                        }

                        strQuery += strHSNQuery + " INSERT INTO [dbo].[SaleReturnDetails] ([BillCode],[BillNo],[RemoteID],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[UnitName],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[BarCode_S],[PurchaseBillNo],[PurchasePartyID],[DisStatus],[Discount],[Dhara],[Packing],[Freight],[TaxFree],[TotalAmt],[PurchaseReturnStatus],[PurchaseReturnNumber],[ItemStatus],[SalesMan]) VALUES "
                                                + " (@BillCode,@BillNo,0," + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ", " + dAmt + ",0,'" + _dr["unitName"] + "',1,0,'" + strBarCode + "','" + strBrandName + "','" + strDesignName + "','','','" + strBarCode_S + "',0,'','+',0,'',0,0,0," + dAmt + ",0,'','FRESH','" + strSalesMan + "')";
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('SALERETURN',@BillCode,@BillNo, " + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + strBarCode + "','" + strBrandName + "','" + strDesignName + "','','') ";

                        _index++;
                    }

                    DataTable _dtTax = new DataTable();
                    double dTaxPer = 0, dMaxPer = 0, dTaxAmt = 0, dTTaxAmt = 0;

                    if (_rows.Length > 0)
                    {
                        _dtTax = _rows.CopyToDataTable().DefaultView.ToTable(true, "taxPer");
                        if (_dtTax.Rows.Count > 0)
                        {
                            _dtTax.Columns.Add("TaxAmt", typeof(Double));
                            foreach (DataRow __row in _dtTax.Rows)
                            {
                                dTaxPer = dba.ConvertObjectToDouble(__row["taxPer"]);
                                if (dTaxPer > dMaxPer)
                                    dMaxPer = dTaxPer;
                                object obj = _dt.Compute("SUM(TaxAmount)", "taxPer=" + dTaxPer + " and CompanyCode='" + strCompanyCode + "'");
                                dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(obj);
                                __row["TaxAmt"] = dTaxAmt;
                            }
                        }
                    }

                    dTTaxAmt = Math.Round(dTTaxAmt, 2);
                    if (!_bInclude)
                        dNetAmt += dTTaxAmt;
                    dNetAmt += dGrossAmt;

                    dCashAmt = ConvertObjectToDouble(txtCashAmt.Text);
                    if (strQuery != "")
                    {
                        string strBillNo = txtBillCode.Text + " " + txtBillNo.Text;
                        dAllNetAmt += Convert.ToDouble((dGrossAmt).ToString("0"));

                        result += _count = dba.SaveRecord_SaleReturnBook(strSalePartyID,txtMobileNo.Text, strDate, txtSalesType.Text, txtSaleBillCode.Text, txtSaleBillNo.Text, txtSaleBillDate.Text, strQuery, dGrossAmt, dMaxPer, dTTaxAmt, GetEntryType(), dTQty, dNetAmt, _dtTax, strCompanyCode, strBillNo, _bInclude, txtRemark.Text, ref _strAttachBill);
                        if (_strAttachBill != "")
                            _strAttachBillWithComma += _strAttachBill + ",";

                        if (_count > 0)
                        {
                            DialogResult _result = MessageBox.Show("Are you want to print Sale Return Bill ?", "Print Sale Return Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (_result == DialogResult.Yes)
                                GSTPrintAndPreview(true, "");
                        }
                    }
                }
            }
            else
                result = 1;

            if (_strAttachBillWithComma.Length > 0)
                txtAttachBill.Text = _strAttachBillWithComma.Substring(0, _strAttachBillWithComma.Length - 1);

            if (MainPage.bHSNWisePurchase)
                txtSaleReturnAmt.Text = dAllNetAmt.ToString("N2", MainPage.indianCurancy);
            else
                txtSaleReturnAmt.Text = "0.00";

            return result;
        }

        private DataTable CreateSecondaryDataTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("id", typeof(String));
            _dt.Columns.Add("srNo", typeof(String));
            _dt.Columns.Add("salesMan", typeof(String));
            _dt.Columns.Add("barCode", typeof(String));
            _dt.Columns.Add("brandName", typeof(String));
            _dt.Columns.Add("styleName", typeof(String));
            _dt.Columns.Add("itemName", typeof(String));
            _dt.Columns.Add("variant1", typeof(String));
            _dt.Columns.Add("variant2", typeof(String));
            _dt.Columns.Add("variant3", typeof(String));
            _dt.Columns.Add("variant4", typeof(String));
            _dt.Columns.Add("variant5", typeof(String));
            // _dt.Columns.Add("description", typeof(String));
            // _dt.Columns.Add("boxRoll", typeof(Double));
            _dt.Columns.Add("qty", typeof(String));
            _dt.Columns.Add("unitName", typeof(String));
            _dt.Columns.Add("mrp", typeof(String));
            _dt.Columns.Add("disPer", typeof(String));
            _dt.Columns.Add("rate", typeof(String));
            _dt.Columns.Add("amount", typeof(String));
            _dt.Columns.Add("barcode_s", typeof(String));
            _dt.Columns.Add("HSNCode", typeof(double));
            _dt.Columns.Add("CompanyCode", typeof(String));
            _dt.Columns.Add("taxper", typeof(double));
            _dt.Columns.Add("taxAmount", typeof(double));

            string strBarCode = "", strCompanyCode = "", strHSNCode = "", strDBName = "";
            DataTable dtTable = new DataTable();
            dtTable.Columns.Add("HSNCode", typeof(String));
            dtTable.Columns.Add("CCode", typeof(String));

            if (MainPage.bHSNWisePurchase)
                strDBName = BAL.GetLocalDBName();
            if (!MainPage.bHSNWisePurchase || strDBName != "")
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strBarCode = Convert.ToString(row.Cells["barCode"].Value);
                    string[] str = strBarCode.Split('-');
                    if (str.Length > 0 || strDBName != "")
                    {
                        if (strDBName != "")
                            strCompanyCode = strDBName;
                        else
                            strCompanyCode = str[0];

                        if (strCompanyCode != "" && strCompanyCode != MainPage.strDataBaseFile)
                        {
                            DataRow _row = _dt.NewRow();
                            for (int _index = 0; _index < dgrdDetails.ColumnCount - 1; _index++)
                            {
                                _row[_index] = row.Cells[_index].Value;
                            }
                            _row["CompanyCode"] = strCompanyCode;

                            if (MainPage.bHSNWisePurchase)
                            {
                                strHSNCode = Convert.ToString(row.Cells["hsnCode"].Value);
                                if (strHSNCode == "")
                                {
                                    MessageBox.Show("Sorry ! HSN code can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _dt.Rows.Clear();
                                    break;
                                }
                                else
                                {
                                    DataRow[] _rows = dtTable.Select("HSNCode='" + strHSNCode + "' and CCode='" + strCompanyCode + "' ");
                                    if (_rows.Length == 0)
                                    {
                                        DataRow __row = dtTable.NewRow();
                                        __row["HSNCode"] = strHSNCode;
                                        __row["CCode"] = strCompanyCode;
                                        dtTable.Rows.Add(__row);
                                    }
                                }
                            }

                            CalculateTaxAmount(_row);

                            _dt.Rows.Add(_row);
                        }
                    }
                }
                if (MainPage.bHSNWisePurchase)
                {
                    bool _bStatus = ValidateHSNCode(dtTable);
                    if (!_bStatus)
                    {
                        _dt.Rows.Clear();
                    }
                }
            }
            return _dt;
        }

        private void SaleReturn_Retail_Load(object sender, EventArgs e)
        {
            SetPermission();
        }
        private void SetPermission()
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
                        txtBillNo.Enabled = false;
                    if (MainPage._bTaxStatus)
                        btnGenSepBill.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.BeginInvoke(new MethodInvoker(Close));
                }
            }
            catch { }
        }

        private void txtDiscPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtDiscPer.Text == "")
                    txtDiscPer.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtDiscPer_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
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
        private void GetOldYearDBName()
        {
            OldYearDBName = "";
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
                            OldYearDBName = DbName;
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

        private void SaleReturn_Retail_FormClosing(object sender, FormClosingEventArgs e)
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

        private void rdoOldYear_CheckedChanged(object sender, EventArgs e)
        {
            GetOldYearDBName();
        }

        private bool ValidateHSNCode(DataTable _dt)
        {

            string strQuery = "", strHSNCode = "", strCCode = "";// "Select COUNT(*) from Items WHere ItemName!='' and ("+ strHSNCode+") ";
            foreach (DataRow row in _dt.Rows)
            {
                strCCode = Convert.ToString(row["CCode"]);
                strHSNCode = Convert.ToString(row["HSNCode"]);
                if (strQuery != "")
                    strQuery += " UNION ALL ";
                strQuery += "Select '" + strHSNCode + "' as HSNCode,ItemName from Items WHere ItemName Like('%" + strHSNCode + "') ";
            }

            DataTable dt = SearchDataOther.GetDataTable(strQuery, strCCode);
            if (_dt.Rows.Count != dt.Rows.Count)
            {
                foreach (DataRow row in _dt.Rows)
                {
                    strHSNCode = Convert.ToString(row["HSNCode"]);
                    DataRow[] _rows = dt.Select("HSNCode='" + strHSNCode + "' ");
                    if (_rows.Length == 0)
                    {
                        MessageBox.Show("Sorry ! " + strHSNCode + " not is in firm code " + strCCode + ". Please create item in that firm.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }
        private void CalculateTaxAmount(DataRow rows)
        {
            double dTaxAmt = 0, dTaxPer = 0;
            string _strTaxType = "", strItemName = "";
            try
            {
                string strCompanyCode = Convert.ToString(rows["CompanyCode"]);
                dgrdTax.Rows.Clear();
                if (strCompanyCode != "")
                {
                    if (dgrdDetails.Rows.Count > 0)
                    {
                        _strTaxType = "INCLUDED";
                        if (txtSalesType.Text.Contains("EXCLUDE"))
                            _strTaxType = "EXCLUDED";

                        string strQuery = "", strSubQuery = "";
                        double dDisStatus = 0;

                        double dRate = 0, dQty = 0, dAmt = 0, dBasicAmt = 0, dOAmt = 0;

                        dRate = dba.ConvertObjectToDouble(rows["rate"]);
                        dQty = dba.ConvertObjectToDouble(rows["qty"]);
                        dAmt = dRate * dQty;
                        dAmt = Math.Round(dAmt, 2);
                        strItemName = Convert.ToString(rows["itemName"]);
                        if (MainPage.bHSNWisePurchase)
                        {
                            string strHSNCode = Convert.ToString(rows["HSNCode"]);
                            // strItemName = "'" + strItemName + "'";
                            strItemName = " Select ItemName from Items _Im inner join ItemGroupMaster IGM on _Im.GroupName=IGM.GroupName WHere HSNCode Like('" + strHSNCode + "') OR ItemName Like('%" + strHSNCode + "') ";
                        }
                        else
                            strItemName = " Select ItemName from Items WHere ItemName Like('%" + strItemName + "') ";

                        dBasicAmt = dba.ConvertObjectToDouble(rows["amount"]);
                        dOAmt += (dBasicAmt - dAmt);

                        if (dRate > 0)
                        {
                            if (strQuery != "")
                                strQuery += " UNION ALL ";
                            strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName in (" + strItemName + ") and " + dAmt + ">0  ";
                        }


                        if (strQuery != "")
                        {
                            strQuery = " Select SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                     + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                            strQuery += strSubQuery;

                            DataTable dt = SearchDataOther.GetDataTable(strQuery, strCompanyCode);
                            if (dt.Rows.Count > 0)
                            {
                                object _objValue = dt.Compute("SUM(Amt)", "");
                                object _objPer = dt.Compute("MAX(TaxRate)", "");
                                dTaxAmt = dba.ConvertObjectToDouble(_objValue);
                                dTaxPer = dba.ConvertObjectToDouble(_objPer);

                                rows["taxPer"] = dTaxPer;
                                rows["taxAmount"] = dTaxAmt;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Sale Return Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool GSTPrintAndPreview(bool _pstatus, string strPath)
        {
            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt;
            if (MainPage.strPrintLayout == "F")
                dt = dba.CreateDebitNoteRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "CREDIT NOTE");
            else
                dt = dba.CreateDebitNoteRetailDataTable_Other(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "CREDIT NOTE");

            if (dt.Rows.Count > 0)
            {
                System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                defS.Copies = (short)MainPage.iNCopySaleRtn;
                defS.Collate = false;
                defS.FromPage = 0;
                defS.ToPage = 0;
                if (MainPage.strPrintLayout != "")
                {
                    if (MainPage.strPrintLayout == "F")
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
                                if (MainPage._bTaxStatus)
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
                                if (MainPage._bTaxStatus)
                                    objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                                objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                                FinallyPrint(_pstatus, objOL_salebill, strPath);
                                objOL_salebill.Close();
                                objOL_salebill.Dispose();
                            }
                        }
                    }
                    else if (MainPage.strPrintLayout == "T5")
                    {
                        Reporting.RetailSaleReturnReportT5_72 objOL_salebill = new Reporting.RetailSaleReturnReportT5_72();
                        objOL_salebill.SetDataSource(dt);
                        // objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else if (MainPage.strPrintLayout == "T4")
                    {
                        Reporting.RetailSaleBookReportT4_80 objOL_salebill = new Reporting.RetailSaleBookReportT4_80();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else if (MainPage.strPrintLayout == "T3")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportT3 objOL_salebill = new Reporting.RetailSaleBookReportT3();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT3_IGST objOL_salebill = new Reporting.RetailSaleBookReportT3_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "T2")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportGatePassT2 objOL_salebill = new Reporting.RetailSaleBookReportGatePassT2();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportGatePassT2_IGST objOL_salebill = new Reporting.RetailSaleBookReportGatePassT2_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "H")
                    {
                        if (!MainPage._bTaxStatus)
                        {
                            Reporting.SaleRetailReturnReport_H objOL_salebill = new Reporting.SaleRetailReturnReport_H();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportHalf objOL_salebill = new Reporting.RetailSaleBookReportHalf();

                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportHalf_IGST objOL_salebill = new Reporting.RetailSaleBookReportHalf_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "Q")
                    {

                        Reporting.RetailSaleBookReportQuarter objOL_salebill = new Reporting.RetailSaleBookReportQuarter();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        FinallyPrint(_pstatus, objOL_salebill, strPath);

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();

                    }
                    else
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportT1 objOL_salebill = new Reporting.RetailSaleBookReportT1();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT1_IGST objOL_salebill = new Reporting.RetailSaleBookReportT1_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            FinallyPrint(_pstatus, objOL_salebill, strPath);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Warning ! Please select design.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return true;
        }

        private void FinallyPrint(bool _pstatus, CrystalDecisions.CrystalReports.Engine.ReportClass Report, string strPath)
        {
            if (strPath != "")
            {
                //Report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                Report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
            }
            else
            {
                if (_pstatus)
                {
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(Report);
                    else
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;
                        defS.Copies = (short)MainPage.iNCopySaleRtn;
                        //string strValue = "0";
                        if (_pstatus)
                        {
                            //strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                            //if (strValue != "" && strValue != "0")
                            //{
                            // defS.Copies = (short)Convert.ToInt64(strValue);
                            Report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            //  }
                        }
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


        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    bool Prited = false;
                    string strPath = CreatePDFFile(false,ref Prited), strEmailID = "", strWhatsAppNo = "";
                    if (Prited)
                    {
                        strFilePath = strPath;
                        string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                        if (strCustomer != "")
                        {
                            string[] _strFullName = txtCustomerName.Text.Split(' ');
                            if (_strFullName.Length > 1)
                            {
                                string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + _strFullName[0] + "'   ";
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
            }
            catch { }
        }

        private void SendWhatsappMessage(string strMobileNo, string strPath)
        {
            string _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text;
            if (!strBranchCode.Contains("-"))
                strBranchCode = "18-19/" + strBranchCode;
            string strWhastappMessage = "", strMsgType = "";
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = txtCustomerName.Text;
            if (btnEdit.Text == "&Update")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                strMessage = "M/S " + strName + ", credit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " UPDATED.";
                //strMType = "creditnote_generation";
                strMsgType = "credit_note";
            }
            else
            {
                strMessage = "M/S " + strName + ", new credit note bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " CREATED.";
                //strMType = "creditnote_update";
                strMsgType = "credit_note_update_pdf";
            }

            bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);

            if (!_bStatus)
            {
                DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (_updateResult == DialogResult.Retry)
                    _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            }

            double dNetAmt = ConvertObjectToDouble(txtCashAmt.Text) + ConvertObjectToDouble(txtNetAmt.Text);
            if (_bStatus)
            {
                strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + dNetAmt + "\",";
                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                //string strResult = WhatsappClass.SendWhatsAppMessage(strMobileNo, strMessage, strFilePath, "CREDITNOTE", "", "PDF");
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                strWhastappMessage = "{\"default\": \"" + strName + "\"},{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + dNetAmt + "\"}";
            }
        }

        private void SendSMSToParty(string strMobileNo)
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    if (strMobileNo == "")
                        strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtCustomerName.Text));

                    string strName = txtCustomerName.Text;
                    if (strMobileNo != "")
                    {
                        double dNetAmt = ConvertObjectToDouble(txtCashAmt.Text) + ConvertObjectToDouble(txtNetAmt.Text);
                        SendSMS objSMS = new SendSMS();
                        string strMessage = "";

                        if (btnAdd.Text == "&Save")
                            strMessage = "M/s " + strName + ", Credit note created with bill no : " + txtBillCode.Text + " " + txtBillNo.Text + ", DT : " + txtDate.Text + " AMT : " + dNetAmt + " Pcs. : " + txtTotalQty.Text;
                        else
                            strMessage = "M/s " + strName + ", Credit note no : " + txtBillCode.Text + " " + txtBillNo.Text + " updated with dated : " + txtDate.Text + ", AMT : " + dNetAmt + " Pcs. : " + txtTotalQty.Text;

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
                        strMessage = "M/S : " + txtCustomerName.Text + " , we have created your credit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "M/S : " + txtCustomerName.Text + ", we have updated your credit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
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

    }
}
