using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace SSS
{
    public partial class PurchaseBook_Retail : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strOldPartyName = "", _strPDFFilePath = "", _strBillType = "", _strSType = "";
        double dOldNetAmt = 0,dRowCount=0;
        bool qtyAdjustStatus = false, _bVariant1 = false, _bVariant2 = false, _bRoundTo5 = false, _bRoundToU5 = false,_bMUAfterDisc=false,_bMUAfterTax=false;
        SearchData _objData;
        SearchCategory _objSearch;
        public PurchaseBook_Retail()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            GetStartupData(true);
        }

        public PurchaseBook_Retail(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();            
            SetCategory();
            GetStartupData(false);
            txtBillCode.Text = strSerialCode;
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select PBillCode,(Select ISNULL(MAX(BillNo),0) from PurchaseBook Where BillCode=PBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select * from [dbo].[Purchase_Setup] Where CompanyID='"+MainPage.strDataBaseFile+"' ";
               DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["PBillCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                    dt = ds.Tables[1];
                    if(dt.Rows.Count>0)
                    {
                        DataRow row = dt.Rows[0];
                        chkVariant1.Checked = _bVariant1= Convert.ToBoolean(row["Data1"]);
                        chkVariant2.Checked = _bVariant2 = Convert.ToBoolean(row["Data2"]);
                        chkRoundTo5.Checked = _bRoundTo5= Convert.ToBoolean(row["Data3"]);
                        chkRoundToU5.Checked = _bRoundToU5= Convert.ToBoolean(row["Data4"]);
                        chkMUAfterDisc.Checked =_bMUAfterDisc= Convert.ToBoolean(row["Data5"]);
                        chkMuAfterTax.Checked = _bMUAfterTax= Convert.ToBoolean(row["Data6"]);
                    }
                }
                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SetCategory()
        {
            try
            {
                chkVariant1.Text = "No Col 1";
                chkVariant2.Text = "No Col 2";
                chkVariant1.Enabled = chkVariant2.Enabled = false;
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                    chkVariant1.Text = "No "+ MainPage.StrCategory1+" col.";
                    chkVariant1.Enabled = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
                    chkVariant2.Text = "No " + MainPage.StrCategory2 + " col.";
                    chkVariant2.Enabled = true;
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'   ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'  ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'  and BillNo>" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                {
                    BindRecordWithControl(strSerialNo);
                }
                else
                {
                    BindLastRecord();
                }
            }
            catch
            {
            }
        }

        private void BindPreviousRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
                else
                    BindFirstRecord();
            }
            catch
            {
            }
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                string strQuery = " Select *,(Convert(varchar,Date,103)) SDate,(Convert(varchar,InvoiceDate,103)) IDate,(Convert(varchar,LRDate,103)) LDate,(Convert(varchar,ISNULL(DueDate,''),103)) DDate,(PurchasePartyID+' '+SM.Name) PartyName,GSTNo,State,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,PB.Date))) LockType from PurchaseBook PB OUTER APPLY (Select Name, GSTNo,State from SupplierMaster SM Where (AreaCode+AccountNo)=PB.PurchasePartyID)SM Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  Select PBS.*,HSNCode,IM.UnitName from PurchaseBookSecondary PBS inner join Items IM on IM.ItemName=PBS.ItemName inner join ItemGroupMaster IGM on IM.GroupName=IGM.GroupName  Where PBS.BillCode='" + txtBillCode.Text + "' and PBS.BillNo=" + strSerialNo + " order by ID "
                                + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                chkPick.Checked = false;
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                ClearAllText();
                DisableAllControls();
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = "";
                txtReason.Text = "";             
               pnlDeletionConfirmation.Visible = false;
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtBillNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["SDate"]);
                            txtInvoiceNo.Text = Convert.ToString(row["InvoiceNo"]);
                            txtInvoiceDate.Text = Convert.ToString(row["IDate"]);
                            txtPurchaseParty.Text = strOldPartyName = Convert.ToString(row["PartyName"]);
                            txtTaxLedger.Text = Convert.ToString(row["PurchaseType"]);
                            txtImportData.Text = Convert.ToString(row["Description"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtGodown.Text = Convert.ToString(row["GodownName"]);
                            txtTransport.Text = Convert.ToString(row["TransportName"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);
                            txtOtherPer.Text = Convert.ToString(row["DiscPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DiscAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);
                            txtROSign.Text = Convert.ToString(row["ROSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOff"]);
                            txtOtherPerSign.Text = Convert.ToString(row["Other"]);

                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);
                            txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                            txtStateName.Text = Convert.ToString(row["State"]);

                            if (dt.Columns.Contains("TaxFree"))
                            {
                                string strTFree = Convert.ToString(row["TaxFree"]);
                                if (strTFree.Contains("-"))
                                    txtTaxFreeSign.Text = "-";
                                txtTaxFreeAmt.Text = strTFree.Replace("-", "").Replace("+", "");

                                txtLRDate.Text = Convert.ToString(row["LDate"]);
                                txtLRNo.Text = Convert.ToString(row["LRNumber"]);
                                txtStockStatus.Text = Convert.ToString(row["StockStatus"]);
                                txtmanufacturer.Text = Convert.ToString(row["Agent"]);
                                txtDueDate.Text = Convert.ToString(row["DDate"]);
                                txtNoOfPacks.Text = Convert.ToString(row["NoOfPacks"]);
                                txtWeight.Text = Convert.ToString(row["PackWeight"]);
                                txtMode.Text = Convert.ToString(row["TransportMode"]);
                                txtReceivedBy.Text = Convert.ToString(row["ReceivedBy"]);
                                txtCountedBy.Text = Convert.ToString(row["CountedBy"]);
                                txtBarcodedBy.Text = Convert.ToString(row["BarCodedBy"]);

                            }

                            chkTCSAmt.Checked = false;
                            txtTCSPer.Text = txtTCSAmt.Text = "0.00";
                            if (dt.Columns.Contains("TCSPer"))
                            {
                                double dTCSPer = dba.ConvertObjectToDouble(row["TCSPer"]), dTCSAmt = dba.ConvertObjectToDouble(row["TCSAmt"]);
                                if (dTCSAmt > 0)
                                {
                                    txtTCSPer.Text = dTCSPer.ToString("0.000");
                                    txtTCSAmt.Text = dTCSAmt.ToString("N2", MainPage.indianCurancy);
                                    chkTCSAmt.Checked = true;
                                }
                                else
                                {
                                    DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                                    if (_date >= Convert.ToDateTime("10/01/2020"))
                                        txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");
                                }
                            }

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);

                            lblTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            lblGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                            if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                                btnEdit.Enabled = btnDelete.Enabled = false;
                            else
                            {
                                if (!MainPage.mymainObject.bPurchaseEdit)
                                    btnEdit.Enabled = btnDelete.Enabled = false;
                                else
                                    btnEdit.Enabled = btnDelete.Enabled = true;
                            }

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;                        
                        }
                    }

                    BindPurchaseBookDetails(ds.Tables[1]);
                    BindGSTDetailsWithControl(ds.Tables[2]);


                }
            }
            catch
            {
            }
        }

        private void BindPurchaseBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            qtyAdjustStatus = false;
            dRowCount = dt.Rows.Count;
            string strPMargin = "";
            if (dRowCount > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
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
                    dgrdDetails.Rows[rowIndex].Cells["saleMargin"].Value = ConvertObjectToDouble(row["SaleMargin"]);
                    dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = ConvertObjectToDouble(row["SaleMRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["wsDisc"].Value = row["WSDis"];
                    dgrdDetails.Rows[rowIndex].Cells["wsMRP"].Value = row["WSMRP"];
                    dgrdDetails.Rows[rowIndex].Cells["saleDis"].Value = row["SaleDis"];
                    dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];

                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["hsnCode"].Value = row["HSNCode"];
                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];

                    dgrdDetails.Rows[rowIndex].Cells["marginType"].Value = row["marginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cmarginType"].Value = row["CompanyMarginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cMargin"].Value = row["CompanyMargin"];
                    dgrdDetails.Rows[rowIndex].Cells["cMrp"].Value = row["CompanyMRP"];
                    dgrdDetails.Rows[rowIndex].Cells["gstAmt"].Value = row["TaxAmt"];

                    if (strPMargin == "")
                        strPMargin = Convert.ToString(row["SaleMargin"]);
                    rowIndex++;
                }
            }
            if (MainPage._bFixedMargin || MainPage._bPurchaseBillWiseMargin)
                txtProfitMargin.Text = strPMargin;            
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
            else
                pnlTax.Visible = false;
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {

                        string strPName = txtPurchaseParty.Text, strGSTNo = "",strStateName="";
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtPurchaseParty.Text = objSearch.strSelectedData;
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                bool _blackListed = false;
                                if (dba.CheckTransactionLockWithBlackListGSTNo(strData, ref _blackListed,ref strGSTNo, ref strStateName))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = "";
                                    if (MainPage._bTaxStatus)
                                        txtGSTNo.Text = txtStateName.Text = "";
                                }
                                else if (_blackListed)
                                {
                                    MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = "";
                                    if (MainPage._bTaxStatus)
                                        txtGSTNo.Text = txtStateName.Text = "";
                                }
                                else
                                {
                                    txtPurchaseParty.Text = strData;
                                    if (MainPage._bTaxStatus)
                                    {
                                        txtGSTNo.Text = strGSTNo;
                                        txtStateName.Text = strStateName;
                                    }
                                }
                            }
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
                if (panalColumnSetting.Visible)
                    panalColumnSetting.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else if (e.KeyCode == Keys.D && dgrdDetails.Focused)
            {
                SetValueToAllRow();
            }
            else
            {
                if (e.KeyCode == Keys.F2 && btnAdd.Text == "&Add(F2)")                
                    btnAdd.PerformClick();               
                else if (e.KeyCode == Keys.F6)                
                    btnEdit.PerformClick();               
                else if (e.KeyCode == Keys.F5 && btnAdd.Text == "&Save(F5)")                
                    btnAdd.PerformClick();               
                else if (e.KeyCode == Keys.F8)                
                    btnDelete.PerformClick();
                else if (e.KeyCode == Keys.F7)                
                    btnSearch.PerformClick();               
                else if (e.KeyCode == Keys.F3)                
                    btnPreview.PerformClick();               
                else if (e.KeyCode == Keys.F4)                
                    btnPrint.PerformClick();
                else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F)                
                    btnSetup.PerformClick();               
                else if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && MainPage.mymainObject.bPurchaseView)
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


        //private void txtAgentName_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
        //        {
        //            char objChar = Convert.ToChar(e.KeyCode);
        //            int value = e.KeyValue;
        //            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //            {
        //                SearchData objSearch = new SearchData("AGENTNAME", "SEARCH AGENT NAME", e.KeyCode);
        //                objSearch.ShowDialog();
        //                txtAgentName.Text = objSearch.strSelectedData;
        //                // CalculateAgentComm();
        //            }
        //            else
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void EnableAllControls()
        {
           txtSplDisAmt.ReadOnly= txtNoOfPacks.ReadOnly=txtWeight.ReadOnly=txtDueDate.ReadOnly= txtProfitMargin.ReadOnly = txtDiscAmt.ReadOnly = txtTaxFreeSign.ReadOnly = txtSpclDisPer.ReadOnly = txtOtherPerSign.ReadOnly = txtDate.ReadOnly = txtInvoiceDate.ReadOnly = txtInvoiceNo.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmt.ReadOnly = txtPackingAmt.ReadOnly = txtOtherPer.ReadOnly = txtTaxFreeAmt.ReadOnly= false;
            dgrdDetails.ReadOnly = false;
            chkTCSAmt.Enabled = true;


        }

        private void DisableAllControls()
        {
            txtSplDisAmt.ReadOnly = txtNoOfPacks.ReadOnly = txtWeight.ReadOnly = txtDueDate.ReadOnly = txtProfitMargin.ReadOnly = txtDiscAmt.ReadOnly = txtTaxFreeSign.ReadOnly = txtSpclDisPer.ReadOnly = txtOtherPerSign.ReadOnly = txtDate.ReadOnly = txtInvoiceDate.ReadOnly = txtInvoiceNo.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmt.ReadOnly = txtPackingAmt.ReadOnly = txtOtherPer.ReadOnly = txtTaxPer.ReadOnly = txtTaxFreeAmt.ReadOnly = true;
            dgrdDetails.ReadOnly = true;
            chkTCSAmt.Enabled = false;
            lblMsg.Text = "";
        }

        private void ClearAllText()
        {
            txtGSTNo.Text=txtStateName.Text= txtMode.Text=txtReceivedBy.Text=txtCountedBy.Text=txtBarcodedBy.Text= txtNoOfPacks.Text = txtWeight.Text = lblCreatedBy.Text= txtImportData.Text = txtTransport.Text = strOldPartyName = txtPurchaseParty.Text = txtTaxLedger.Text = txtTaxLedger.Text = txtRemark.Text = txtInvoiceNo.Text = txtGodown.Text = txtLRNo.Text = txtmanufacturer.Text = "";
            txtTCSAmt.Text = txtTaxFreeAmt.Text = txtSpclDisPer.Text = txtSplDisAmt.Text = txtRoundOff.Text = txtOtherAmt.Text = txtPackingAmt.Text = lblTaxableAmt.Text = txtOtherPer.Text = txtDiscAmt.Text = txtTaxAmt.Text = txtTaxPer.Text = lblTotalQty.Text = lblGrossAmt.Text = lblNetAmt.Text = txtTaxFreeAmt.Text = "0.00";
            txtSign.Text = txtROSign.Text = txtTaxFreeSign.Text = "+";
            txtStockStatus.Text = "STOCK IN";
            txtOtherPerSign.Text = "-";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = chkPick.Checked = chkTCSAmt.Checked = false;
            dOldNetAmt = dRowCount = 0;
            txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");

            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtInvoiceDate.Text = txtLRDate.Text = txtDueDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtInvoiceDate.Text = txtLRDate.Text = txtDueDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

            if (MainPage._bPurchaseBillWiseMargin)
                txtProfitMargin.Text = MainPage.dPurchaseBillMargin.ToString();
            else if (MainPage._bFixedMargin)
                txtProfitMargin.Text = MainPage.dFixedMargin.ToString();
            else
                txtProfitMargin.Text = "0.00";
        }

        private void ClearAllTextForPDF()
        {
            _strSType = "";
            txtImportData.Text = txtTransport.Text = strOldPartyName = txtPurchaseParty.Text = txtInvoiceNo.Text = txtGodown.Text = txtStockStatus.Text = txtLRNo.Text = txtmanufacturer.Text = "";
            txtSpclDisPer.Text = txtSplDisAmt.Text = txtRoundOff.Text = txtOtherAmt.Text = txtPackingAmt.Text = lblTaxableAmt.Text = txtOtherPer.Text = txtDiscAmt.Text = txtTaxAmt.Text = txtTaxPer.Text = lblTotalQty.Text = lblGrossAmt.Text = lblNetAmt.Text =txtTaxFreeAmt.Text= "0.00";
            txtSign.Text = txtROSign.Text = "+";
            txtOtherPerSign.Text = "-";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = chkPick.Checked = false;
            dOldNetAmt = dRowCount= 0;
            _strBillType = "";          
        }


        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(PurchaseBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from PurchaseBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='PURCHASE' and TaxIncluded=0) TaxName  from PurchaseRecord Where BillCode='" + txtBillCode.Text + "')Purchase ");
                    if (table.Rows.Count > 0)
                    {                      
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                        if (MainPage._bTaxStatus && Convert.ToString(table.Rows[0]["TaxName"]) != "")
                            txtTaxLedger.Text = Convert.ToString(table.Rows[0]["TaxName"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in set sale bill No in Purchase book", ex.Message };
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
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! Party Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }
            if(chkTCSAmt.Checked)
            {
                double dTCSAmt = ConvertObjectToDouble(txtTCSAmt.Text);
                if(dTCSAmt>0)
                {
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select (AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES'");
                    if(Convert.ToString(objValue)=="")
                    {
                        MessageBox.Show("Sorry ! Please create TCS Account under category : 'TCS RECEIVABLES' with Group Name : SHORT-TERM LOANS AND ADVANCES'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(rows.Cells["itemName"].Value);
                double dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);
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
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                dgrdDetails.Focus();
                return false;
            }
            return ValidateOtherValidation(false);
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            string strOrderQuery = "";
            strOrderQuery = "0";

            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(" + strOrderQuery + ") OQty, (Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select TOP 1 UPPER(Tick) from BalanceAmount Where AccountStatus='PURCHASE A/C' and Description Like(CS.PBillCode+' " + txtBillNo.Text + "')),'FALSE') TickStatus,CS.PBillCode from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtPurchaseParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (btnEdit.Text == "&Update(F6)" || _bUpdateStatus)
                {
                    if (dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || strOldPartyName != txtPurchaseParty.Text || _bUpdateStatus)
                    {
                        if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                        {
                            bool iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);
                            string strPBillCode = Convert.ToString(dt.Rows[0]["PBillCode"]);

                            if (!iStatus && MainPage.strOnlineDataBaseName != "")
                            {
                                bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(strPBillCode + " " + txtBillNo.Text);
                                if (!netStatus)
                                {
                                    // MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    {
                        int check = dba.CheckGoodsReceiptAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from (Select ISNULL(Max(BillNo)+1,1)BillNo from PurchaseBook where BillCode='" + txtBillCode.Text + "' UNION ALL Select ISNULL(Max(BillNo)+1,1)BillNo from PurchaseRecord where BillCode='" + txtBillCode.Text + "')_Sales"));
                            MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Text = strBillNo;
                            chkStatus = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Bill No is already in used please Choose Different bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bill No can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }


        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)") && (Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (e.ColumnIndex <2 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 18 || e.ColumnIndex == 19)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        string strValue = Convert.ToString(dgrdDetails.CurrentCell.FormattedValue);
                         _objData = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                        _objData.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                        if (MainPage._bBrandWiseMargin)
                            BindBrandMargin(_objData.strSelectedData);
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3)
                    {
                        string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";

                        _objSearch = new SearchCategory("", "DESIGNNAME", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, false, "");
                        _objSearch.ShowDialog();
                        GetAllDesignSizeColor(_objSearch, dgrdDetails.CurrentRow.Index);
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        _objSearch = new SearchCategory("1", MainPage.StrCategory1, Keys.Space);
                        _objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 5)
                    {
                        _objSearch = new SearchCategory("2", MainPage.StrCategory2, Keys.Space);
                        _objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 6)
                    {
                        _objSearch = new SearchCategory("3", MainPage.StrCategory3, Keys.Space);
                        _objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 7)
                    {
                        _objSearch = new SearchCategory("4", MainPage.StrCategory4, Keys.Space);
                        _objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 8)
                    {
                        _objSearch = new SearchCategory("5", MainPage.StrCategory5, Keys.Space);
                        _objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                        e.Cancel = true;
                    }                  
                    else if (e.ColumnIndex == 20 || e.ColumnIndex == 23)
                    {
                        _objData = new SearchData("MARGINTYPE", "SELECT MARGIN TYPE", Keys.Space);
                        _objData.ShowDialog();
                        if (_objData.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                            CalculateSaleMarginWithMargins(dgrdDetails.CurrentRow);
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 21)
                    {
                        if (!MainPage._bItemWiseMargin && !MainPage._bDesignMasterMargin)
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

        private void BindBrandMargin(string strBrandName)
        {
            try
            {
                double dValue = 0;
                if (strBrandName != "")
                {
                    string strQuery = "Select Margin from BrandMaster Where BrandName='" + strBrandName + "' ";
                    object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                    dValue = ConvertObjectToDouble(objValue);
                    if (dValue == 0)
                        dValue = MainPage.dBrandwiseMargin;
                }
                dgrdDetails.CurrentRow.Cells["saleMargin"].Value = dValue.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }


        private void GetAllDesignSizeColor(SearchCategory objCategory, int rowIndex)
        {
            try
            {
                bool firstRow = false;
                if (objCategory != null)
                {
                    if (objCategory.lbSearchBox.Items.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData == "")
                        {
                            foreach (string strItem in objCategory.lbSearchBox.Items)
                            {
                                if (strItem != "ADD NEW DESIGNNAME NAME" && strItem!= "ADD NEW ITEM NAME")
                                {
                                    string[] strAllItem = strItem.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                        //if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                                        //    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[0];

                                        if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                            GetPurchaseRate(dgrdDetails.Rows[rowIndex]);

                                        SetUnitName(strAllItem[0], rowIndex);

                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowIndex > 0)
                                rowIndex--;
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                if (strAllItem[0] != "ADD NEW DESIGNNAME NAME" && strAllItem[0] != "ADD NEW ITEM NAME")
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                    //if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                                    //    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[0];

                                    if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                    if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                    if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                    if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                    if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                        GetPurchaseRate(dgrdDetails.Rows[rowIndex]);
                                    SetUnitName(strAllItem[0], rowIndex);
                                }
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();

                        //if (btnAdd.Text == "&Save(F5)")
                        //{
                        //    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "")
                        //    {
                        //        dgrdDetails.Rows.Add(1);
                        //        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                        //        dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                        //        dgrdDetails.Focus();
                        //    }
                        //}
                    }
                }
            }
            catch
            {
            }
        }

        private void SetUnitName(string strDesignName, int rowIndex)
        {
            if (strDesignName != "")
            {
                DataTable table = dba.GetDataTable("Select BrandName,IGM.HSNCode,StockUnitName UnitName,BuyerDesignName from Items IM inner join ItemGroupMaster IGM on IM.GroupName=IGM.GroupName Where ItemName='" + strDesignName + "' ");
                if (table.Rows.Count > 0)
                {
                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                        dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = table.Rows[0]["BuyerDesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["hsnCode"].Value = table.Rows[0]["HSNCode"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void GetPurchaseRate(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, _dMRP = 0, dSaleRate=0, dRate = 0, dSpclDis = dba.ConvertObjectToDouble(txtSpclDisPer.Text);
                if (row != null)
                {
                    object objDisPer = 0, objSaleRate = 0; 
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value,ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        dSaleRate = ConvertObjectToDouble(objSaleRate);
                        row.Cells["mrp"].Value =row.Cells["wsMRP"].Value= dMRP;
                        if (dDisPer != 0)
                            dDisPer = dDisPer * -1;
                    }
                }
                if (dSpclDis != 0 && dMRP != 0)
                    _dMRP = dMRP * (100.00 - dSpclDis) / 100.00;
                else
                    _dMRP = dMRP;

                dDisPer = Math.Abs(dDisPer);
                if (dDisPer != 0 && _dMRP != 0)
                    dRate = _dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = _dMRP;

                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                dAmt = dQty * dRate;
                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                if (MainPage._bDesignMasterMargin)
                {
                    row.Cells["saleMRP"].Value = row.Cells["saleRate"].Value = dSaleRate;
                    CalculateSaleMarginWithSaleMRP_PRate(row, dMRP, dSaleRate);
                }

            }
            catch
            {
            }
        }

        private void GetPurchaseRate_Import(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, _dMRP = 0, dRate = 0, dSpclDis = dba.ConvertObjectToDouble(txtSpclDisPer.Text), dOldRate = 0, dOldMRP = 0;
                if (row != null)
                {
                    object objDisPer = 0,objSaleRate=0;
                    dOldRate = dba.ConvertObjectToDouble(row.Cells["rate"].Value);
                    dOldMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                        if (dDisPer != 0)
                            dDisPer = dDisPer * -1;
                    }
                }
                if (dSpclDis != 0 && dMRP != 0)
                    _dMRP = dMRP * (100.00 - dSpclDis) / 100.00;
                else
                    _dMRP = dMRP;



                dDisPer = Math.Abs(dDisPer);
                if (dDisPer != 0 && _dMRP != 0)
                    dRate = _dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = _dMRP;

                if (dOldMRP != dMRP && dOldMRP > 0)
                    dMRP = dOldMRP;

                if (dRate != dOldRate)
                {
                    dDisPer = (100.00 - ((dOldRate * 100.00) / dMRP));
                    dRate = dOldRate;
                }
                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value);//, dDisc = ConvertObjectToDouble(row.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                dAmt = dQty * dRate;

                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                //row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }


        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, dMRP = 0, _dMRP = 0, dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0, dDisc = 0, dOCharges = 0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    _dMRP = dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (dSpclPer != 0 && dMRP != 0)
                    {
                        dSpclAmt += (dMRP * dSpclPer) / 100.00;
                       _dMRP = dMRP * (100.00 - dSpclPer) / 100.00;
                    }
                    else
                        _dMRP = dMRP;

                    if ((dDisPer != 0) && _dMRP != 0)
                    {
                        dDisPer = Math.Abs(dDisPer);

                        dRate = _dMRP * (100.00 - (dDisPer)) / 100.00;
                        dRate = Math.Round(dRate, 2);
                    }
                    if (dRate == 0)
                        dRate = _dMRP;

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);                
                }

                txtSplDisAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void CalculateSelectAllValue()
        {
            try
            {
                double  dMRP = 0,  dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0, dWMRP = 0, dWDis = 0;

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dWMRP = dba.ConvertObjectToDouble(row.Cells["wsMRP"].Value);
                    dWDis = dba.ConvertObjectToDouble(row.Cells["wsDisc"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);

                    dMRP = Math.Round((dWMRP * dWDis) / 100.00, 4);
                    dRate = Math.Round((dMRP * dDisPer) / 100.00, 4);
                    dAmt = dQty * dRate;

                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (e.ColumnIndex == 13 || e.ColumnIndex == 14)
                        CalculateRateWithWS(dgrdDetails.Rows[e.RowIndex]);
                    if (e.ColumnIndex == 12 || e.ColumnIndex == 16)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex], e.ColumnIndex);
                    else if (e.ColumnIndex == 15)
                        CalculateWDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if(e.ColumnIndex == 17)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex==21 || e.ColumnIndex == 24)
                        CalculateSaleMarginWithMargins(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 22)
                        CalculateSaleMarginWithSaleMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 25)
                        CalculateSaleMarginWithCompanyMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 27)
                        CalculateSaleDisWithSaleRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 26)
                        CalculateSaleDisWithSaleDisc(dgrdDetails.Rows[e.RowIndex]);       
                               
                }
            }
            catch
            {
            }
        }

        private void CalculateSaleDisWithSaleRate(DataGridViewRow row)
        {
            try
            {
                double dSaleRate = 0, dSaleMRP = 0, dSaleDis = 0;
                dSaleRate = ConvertObjectToDouble(row.Cells["saleRate"].Value);
                dSaleMRP = ConvertObjectToDouble(row.Cells["saleMRP"].Value);

                if (dSaleMRP != 0 && dSaleRate != 0)
                    dSaleDis = ((dSaleMRP - dSaleRate) / dSaleMRP) * 100.00;

                row.Cells["saleDis"].Value = dSaleDis;

               // CalculateAllAmount();
            }
            catch { }
        }

        private void CalculateSaleDisWithSaleDisc(DataGridViewRow row)
        {
            try
            {
                double dSaleRate = 0, dSaleMRP = 0, dSaleDis = 0;
                dSaleDis = ConvertObjectToDouble(row.Cells["saleDis"].Value);
                dSaleMRP = ConvertObjectToDouble(row.Cells["saleMRP"].Value);

                if (dSaleMRP != 0)
                    dSaleRate = (dSaleMRP - ((dSaleDis * dSaleMRP) / 100.00)) ;

                row.Cells["saleRate"].Value = dSaleRate;

                // CalculateAllAmount();
            }
            catch { }
        }

        private void CalculateSaleMarginWithSaleMRP(DataGridViewRow rows)
        {
            try
            {
                string strMarginType = "", strCMarginType = "";
                double dSaleMRP = 0, dMRP = 0, dSaleMargin = 0, dComMargin = 0, dCompanyMRP = 0, dSaleDis=0, dSaleRate=0;
                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);
                dSaleMRP = ConvertObjectToDouble(rows.Cells["saleMRP"].Value);

                if (_bMUAfterTax)
                    dMRP += ConvertObjectToDouble(rows.Cells["gstAmt"].Value);

                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";


                if (strMarginType == "MARKUP")
                    dSaleMargin = Math.Round(((dSaleMRP * 100.00 / dMRP) - 100.00), 4);
                else
                    dSaleMargin = Math.Round((100.00 - dMRP * 100 / dSaleMRP), 4);

                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                if (strCMarginType == "MARKUP")
                    dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                else
                    dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

            }
            catch { }
        }

        private void CalculateSaleMarginWithCompanyMRP(DataGridViewRow rows)
        {
            try
            {
                string strMarginType = "", strCMarginType = "";
                double dSaleMRP = 0, dMRP = 0, dSaleMargin = 0, dComMargin = 0, dCompanyMRP = 0, dSaleDis = 0, dSaleRate = 0;

                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                dCompanyMRP = ConvertObjectToDouble(rows.Cells["cMrp"].Value);
                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);
                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";

                if (strCMarginType == "MARKUP")
                    dSaleMRP = Math.Round(((dCompanyMRP * 100.00) / (100.00 + dComMargin)), 2);
                else
                    dSaleMRP = Math.Round(((dCompanyMRP * (100.00 - dComMargin)) / 100.00), 2);

                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);

                if (_bMUAfterTax)
                    dMRP += ConvertObjectToDouble(rows.Cells["gstAmt"].Value);


                if (strMarginType == "MARKUP")
                    dSaleMargin = Math.Round(((dSaleMRP * 100.00 / dMRP) - 100.00), 4);
                else
                    dSaleMargin = Math.Round((100.00 - dMRP * 100 / dSaleMRP), 4);


                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                rows.Cells["saleMargin"].Value = dSaleMargin;
                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

            }
            catch { }
        }

        private void CalculateSaleMarginWithMargins(DataGridViewRow rows)
        {
            try
            {
                double dMRP = 0, dSaleMargin = 0, dSaleDis = 0, dItemTaxAmt = 0, dSaleRate = 0, dComMargin = 0, dCompanyMRP = 0, dSaleMRP = 0, dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dAgentCommAmt = 0, dRoundOff = 0;
                string strMarginType = "", strCMarginType = "";
                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);

                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";
               
                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);
               
                dItemTaxAmt = ConvertObjectToDouble(rows.Cells["gstAmt"].Value);

                if (_bMUAfterTax)
                    dMRP += dItemTaxAmt;

                if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin || MainPage._bDesignMasterMargin)
                {
                    dSaleMargin = ConvertObjectToDouble(rows.Cells["saleMargin"].Value);
                    if (dSaleMargin == 0)
                    {
                        if (MainPage._bItemWiseMargin)
                            dSaleMargin = MainPage.dItemwiseMargin;
                        if (MainPage._bBrandWiseMargin)
                            dSaleMargin = MainPage.dBrandwiseMargin;
                    }
                }

                if (strMarginType == "MARKUP")
                    dSaleMRP = Math.Round((dMRP * (100.00 + dSaleMargin) / 100.00), 2);
                else
                    dSaleMRP = Math.Round((dMRP / (100.00 - dSaleMargin) * 100.00), 2);

                if (_bRoundTo5)
                    dSaleMRP = dba.RoundOffNearest(dSaleMRP, 5);

                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                if (strCMarginType == "MARKUP")
                    dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                else
                    dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100.00 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

            }
            catch
            {
            }
        }

        private void CalculateSaleMarginWithSaleMRP_PRate(DataGridViewRow row, double dMRP, double dSaleMRP)
        {
            try
            {
                double dSaleMargin = 0;

                if (dSaleMRP != 0 && dMRP != 0)
                    dSaleMargin = ((dSaleMRP * 100.00) / dMRP) - 100.00;
                row.Cells["saleMargin"].Value = dSaleMargin;
            }
            catch { }
        }


        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
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
                                if (IndexColmn == 5 && _bVariant1)
                                    IndexColmn++;
                                if (IndexColmn == 6 && _bVariant2)
                                    IndexColmn++;

                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (IndexColmn == 10)
                                    IndexColmn+=2;
                                if (IndexColmn == 11)
                                    IndexColmn++;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
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

                                DataGridViewRow row = dgrdDetails.Rows[dgrdDetails.RowCount - 2];
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["brandName"].Value = row.Cells["brandName"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["saleMargin"].Value = row.Cells["saleMargin"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["disPer"].Value = row.Cells["disPer"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["wsDisc"].Value = row.Cells["wsDisc"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["marginType"].Value = row.Cells["marginType"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["cmarginType"].Value = row.Cells["cmarginType"].Value;
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["cMargin"].Value = row.Cells["cMargin"].Value;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[2];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtPackingAmt.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save(F5)")
                    {
                        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update(F6)")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        if (strID == "")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                                dgrdDetails.Enabled = true;
                            }
                            else
                            {
                                ArrangeSerialNo();
                            }
                            CalculateAllAmount();
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                DeleteOneRow(strID);
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                    {
                        DataGridViewRow row = dgrdDetails.CurrentRow;
                        dgrdDetails.Rows.Add();
                        DataGridViewRow _row = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1];

                        _row.Cells["srNo"].Value = dgrdDetails.Rows.Count; ;
                        _row.Cells["id"].Value = "";
                        _row.Cells["itemName"].Value = row.Cells["ItemName"].Value;
                        _row.Cells["variant1"].Value = row.Cells["Variant1"].Value;
                        _row.Cells["variant2"].Value = row.Cells["Variant2"].Value;
                        _row.Cells["variant3"].Value = row.Cells["Variant3"].Value;
                        _row.Cells["variant4"].Value = row.Cells["Variant4"].Value;
                        _row.Cells["variant5"].Value = row.Cells["Variant5"].Value;
                        _row.Cells["qty"].Value = row.Cells["Qty"].Value;
                        _row.Cells["disPer"].Value = row.Cells["disPer"].Value;
                        _row.Cells["mrp"].Value = row.Cells["MRP"].Value;
                        _row.Cells["rate"].Value = row.Cells["Rate"].Value;
                        _row.Cells["amount"].Value = row.Cells["Amount"].Value;
                        _row.Cells["saleMargin"].Value = row.Cells["SaleMargin"].Value;
                        _row.Cells["saleMRP"].Value = row.Cells["SaleMRP"].Value;
                        _row.Cells["wsDisc"].Value = row.Cells["wsDisc"].Value;
                        _row.Cells["wsMRP"].Value = row.Cells["WSMRP"].Value;                      
                        _row.Cells["unitName"].Value = row.Cells["UnitName"].Value;
                        _row.Cells["brandName"].Value = row.Cells["BrandName"].Value;
                        _row.Cells["styleName"].Value = row.Cells["styleName"].Value;
                        _row.Cells["hsnCode"].Value = row.Cells["hsnCode"].Value;                      
                        _row.Cells["saleDis"].Value = row.Cells["SaleDis"].Value;
                        _row.Cells["saleRate"].Value = row.Cells["SaleRate"].Value;
                        _row.Cells["cMargin"].Value = row.Cells["cMargin"].Value;
                        _row.Cells["cMrp"].Value = row.Cells["cMrp"].Value;
                        _row.Cells["marginType"].Value = row.Cells["marginType"].Value;
                        _row.Cells["cmarginType"].Value = row.Cells["cmarginType"].Value;
                        _row.Cells["gstAmt"].Value = row.Cells["gstAmt"].Value;

                        dgrdDetails.CurrentCell = _row.Cells["brandName"];

                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if ((columnIndex > 11 || columnIndex == 9) && columnIndex != 20 && columnIndex != 23)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
        }

        private void SetValueToAllRow()
        {
            int _columnIndex = dgrdDetails.CurrentCell.ColumnIndex, _rowIndex = dgrdDetails.CurrentCell.RowIndex;
            if (_columnIndex > 11 && _rowIndex >= 0 && _columnIndex!=18)
            {
                string strValue = Convert.ToString(dgrdDetails.CurrentCell.EditedFormattedValue);
                for (int _index = 0; _index < dgrdDetails.Rows.Count; _index++)
                {
                    dgrdDetails.Rows[_index].Cells[_columnIndex].Value = strValue;
                }
                if (_columnIndex == 14 || _columnIndex == 16)
                    CalculateSelectAllValue();
                else if (_columnIndex == 17)
                    CalculateAllDis();
                else if (_columnIndex == 15)
                    CalculateAllWSDis();
                CalculateAllAmount();
            }
        }

        private void CalculateAllDis()
        {
            try
            {
                double dMRP = 0, dDisPer = 0, dRate = 0;

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dRate = dba.ConvertObjectToDouble(row.Cells["rate"].Value);
                    if (dRate != 0 && dMRP != 0)
                    {
                        dDisPer = Math.Round((dRate * 100.00) / dMRP, 4);
                        row.Cells["disPer"].Value = dDisPer;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void CalculateAllWSDis()
        {
            try
            {
                double dWSMRP = 0, dWsDisPer = 0, dMRP = 0, dDisPer=0, dRate=0;

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dWSMRP = dba.ConvertObjectToDouble(row.Cells["wsMRP"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    if (dWSMRP != 0 && dMRP != 0)
                    {
                        dWsDisPer = Math.Round((dMRP * 100.00) / dWSMRP, 4);
                        row.Cells["wsDisc"].Value = dWsDisPer;
                    }
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);                   

                    dMRP = Math.Round((dWSMRP * dWsDisPer) / 100.00, 2);
                    dRate = Math.Round((dMRP * dDisPer) / 100.00, 2);                

                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                }
            }
            catch (Exception ex)
            {             
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;              
                
                {
                    if ( columnIndex == 9)
                    {
                        dba.ValidateSpace(sender, e);
                    }
                    if (columnIndex > 11)
                    {
                        if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                        {
                            e.Handled = true;
                            SetValueToAllRow();
                        }
                        else
                        {
                            if (columnIndex == 21 || columnIndex == 24)
                                dba.KeyHandlerPoint(sender, e, 4);
                            else if (columnIndex > 11 && columnIndex != 20 && columnIndex != 23)
                            {
                                dba.KeyHandlerPoint(sender, e, 2);
                            }
                        }
                    }
                }             
            }
            catch { }
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

        private void DeleteOneRow(string strID)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    string strQuery = " Delete from PurchaseBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + " ";
                    dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                    CalculateAllAmount();
                    int result = UpdateRecord(strQuery);
                    if (result < 1)
                        BindRecordWithControl(txtBillNo.Text);
                    else
                    {
                        strQuery = " Delete from PurchaseBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + " ";
                        DataBaseAccess.CreateDeleteQuery(strQuery);

                        dgrdDetails.ReadOnly = false;
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                            dgrdDetails.Enabled = true;
                        }
                        else
                            ArrangeSerialNo();
                    }
                }

            }
            catch
            {
            }
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void CalculateRateWithWS(DataGridViewRow rows)
        {
            double dMRP = 0, dRate = 0, dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dWSMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value), dWSDisc = ConvertObjectToDouble(rows.Cells["wsDisc"].Value), dDis = ConvertObjectToDouble(rows.Cells["disPer"].Value);
            dMRP = (dWSMRP * (100.00 - dWSDisc) / 100.00);

            if (dDis != 0 && dMRP != 0)
            {
                dRate = dMRP * (100.00 - dDis) / 100.00;
                dRate = Math.Round(dRate, 2);
            }
            if (dRate == 0)
                dRate = dMRP;

            dAmt = dQty * dRate;

            rows.Cells["mrp"].Value = dMRP;
            rows.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            GetRowTaxAmt(rows);
            CalculateAllAmount();
        }

        private void CalculateWDisWithAmountMRP(DataGridViewRow rows)
        {
            double dDisPer = 0, dMRP = 0, dRate = 0, dWSMRP = 0, dWSDis = 0;
            if (rows != null)
            {
                dWSMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                if (dWSMRP != 0 && dMRP != 0)
                    dWSDis = ((dWSMRP - dMRP) / dWSMRP) * 100.00;

                dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);
                dRate = dMRP - ((dDisPer * dMRP) / 100.00);// ConvertObjectToDouble(rows.Cells["rate"].Value);

                rows.Cells["rate"].Value = dRate;
                rows.Cells["wsDisc"].Value = dWSDis;

                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;
                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                GetRowTaxAmt(rows);
                CalculateAllAmount();
            }
        }

        private void CalculateDisWithAmountMRP(DataGridViewRow rows)
        {
            double dDisPer = 0, dMRP = 0, dRate = 0, dWSMRP = 0, dWSDis = 0;
            if (rows != null)
            {
                dWSMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                if (dWSMRP != 0 && dMRP != 0)
                    dWSDis = ((dWSMRP - dMRP) / dWSMRP) * 100.00;

                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);

                if (dRate != 0 && dMRP != 0)
                    dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                rows.Cells["disPer"].Value = dDisPer;
                rows.Cells["wsDisc"].Value = dWSDis;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;
                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                GetRowTaxAmt(rows);
                CalculateAllAmount();
            }
        }

        private void CalculateDisWithAmountMRP_Current(DataGridViewRow rows)
        {

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);


                if (dRate != 0 && dMRP != 0)
                    dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                rows.Cells["disPer"].Value = dDisPer;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                GetRowTaxAmt(rows);
            }
        }

        private void CalculateAmountWithQtyRate(DataGridViewRow rows, int _colIndex)
        {
            double dAmt = 0, dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value), dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
            dDisPer = Math.Abs(dDisPer);
            if (dDisPer != 0 && dMRP != 0)
            {
                dRate = dMRP * (100.00 - dDisPer) / 100.00;
                dRate = Math.Round(dRate, 2);
            }
            if (dRate == 0)
                dRate = dMRP;

            dAmt = dQty * dRate;
            rows.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            if (_colIndex != 12)
                GetRowTaxAmt(rows);
            CalculateAllAmount();
        }


        private void CalculateAllAmount()
        {
            try
            {
                CalculateSpecialDiscount();

                double dMRP = 0, dSaleMargin = 0, dSaleDis = 0, dSaleRate = 0, dComMargin = 0, dTaxableAmt = 0, dCompanyMRP = 0, dSaleMRP = 0, dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dAgentCommAmt = 0, dRoundOff = 0;

                if (MainPage._bPurchaseBillWiseMargin || MainPage._bFixedMargin)
                    dSaleMargin = ConvertObjectToDouble(txtProfitMargin.Text);
                string strMarginType = "", strCMarginType = "";
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                    strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                    if (strMarginType == "")
                        rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                    if (strCMarginType == "")
                        rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";

                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    if (_bMUAfterDisc)
                        dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                    else
                        dMRP = ConvertObjectToDouble(rows.Cells["wsMRP"].Value);

                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);


                    if (_bMUAfterTax)                    
                        dMRP += ConvertObjectToDouble(rows.Cells["gstAmt"].Value); 

                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin || MainPage._bDesignMasterMargin)
                    {
                        dSaleMargin = ConvertObjectToDouble(rows.Cells["saleMargin"].Value);
                        if (dSaleMargin == 0)
                        {
                            if (MainPage._bItemWiseMargin)
                                dSaleMargin = MainPage.dItemwiseMargin;
                            if (MainPage._bBrandWiseMargin)
                                dSaleMargin = MainPage.dBrandwiseMargin;
                        }
                    }

                    if (strMarginType == "MARKUP")
                        dSaleMRP = Math.Round((dMRP * (100.00 + dSaleMargin) / 100.00), 2);
                    else
                        dSaleMRP = Math.Round((dMRP / (100.00 - dSaleMargin) * 100.00), 2);

                    if (_bRoundTo5)
                        dSaleMRP = dba.RoundOffNearest(dSaleMRP, 5);

                    dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                    if (strCMarginType == "MARKUP")
                        dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                    else
                        dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                    rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                    rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                    dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                    dSaleRate = (dSaleMRP * (100 - dSaleDis)) / 100.00;
                    rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);
                }

                lblGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dPackingAmt = ConvertObjectToDouble(txtPackingAmt.Text);
                dOtherAmt = ConvertObjectToDouble(txtOtherAmt.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                double dDisPer = ConvertObjectToDouble(txtOtherPerSign.Text + txtOtherPer.Text), dGrossAmt = 0, dTaxFree = ConvertObjectToDouble(txtTaxFreeSign.Text + txtTaxFreeAmt.Text), dTcsPer = 0, dTCSAmt = 0;

                dTOAmt = dOtherAmt + dPackingAmt;
                dGrossAmt = dBasicAmt ;
                dDiscAmt = (dGrossAmt * dDisPer) / 100;

                dGrossAmt +=  dTOAmt;             
                dTOAmt += dDiscAmt;

                dFinalAmt = dBasicAmt + dTOAmt;
                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                dNetAmt = dBasicAmt + dOtherAmt + dPackingAmt + dTaxAmt + dDiscAmt + dTaxFree;
                dNetAmt = Math.Round(dNetAmt, 2);
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = (dNetAmt * dTcsPer) / 100.00;
                    dNetAmt += dTCSAmt;
                }

                double dNNetAmt =  Convert.ToDouble(dNetAmt.ToString("0"));// Math.Round(dNetAmt, 0);
                dRoundOff = (dNNetAmt - dNetAmt);

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

                txtTCSAmt.Text = dTCSAmt.ToString("N2", MainPage.indianCurancy);
                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
                lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);

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
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    TextBox txtNew = sender as TextBox;
                    if (txtNew.Text == "")
                        txtNew.Text = "0.00";
                    CalculateAllAmount();
                }
            }
            catch { }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtSign.Text == "")
                    txtSign.Text = "+";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }
    

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {           
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (txtTaxPer.Text == "")
                        txtTaxPer.Text = "0.00";                 
                    CalculateAllAmount();
                }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtPackingAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add(F2)")
            {//Add(F2)Save(F5)
                if (btnEdit.Text == "&Update(F6)")
                {
                    DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                }
                btnAdd.Text = "&Save(F5)";
                btnEdit.Text = "&Edit(F6)";
                EnableAllControls();
                txtBillNo.ReadOnly = false;
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
                string strInvoiceDate="NULL",strLRDate="NULL",strDueDate="NULL", strMNID="";
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (txtInvoiceDate.Text.Length == 10)
                {
                    DateTime iDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text);
                    strInvoiceDate = "'" + iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtLRDate.Text.Length == 10 && txtLRNo.Text!="")
                {
                    DateTime lDate = dba.ConvertDateInExactFormat(txtLRDate.Text);
                    strLRDate = "'" + lDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtDueDate.Text.Length == 10)
                {
                    DateTime dDate = dba.ConvertDateInExactFormat(txtDueDate.Text);
                    strDueDate = "'" + dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
                }

                double dAmt = Convert.ToDouble(lblNetAmt.Text),dTaxAmt=dba.ConvertObjectToDouble(txtTaxAmt.Text),dSpclDisPer=dba.ConvertObjectToDouble(txtSpclDisPer.Text),dTaxFree=dba.ConvertObjectToDouble(txtTaxFreeSign.Text+txtTaxFreeAmt.Text), dTcsPer = 0, dTCSAmt = 0;
                string strPurchaseParty = "", strPurchasePartyID = "";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }
                if (txtmanufacturer.Text != "")
                {
                    strFullName = txtmanufacturer.Text.Split(' ');
                    strMNID = strFullName[0];
                }
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                }

                string strQuery = " if not exists(Select ReceiptCode from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " UNION ALL Select BillCode from PurchaseBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ") begin "
                                       + " INSERT INTO [dbo].[PurchaseBook] ([BillCode],[BillNo],[Date],[InvoiceNo],[InvoiceDate],[PurchasePartyID],[PurchaseParty],[PurchaseType],[TransportName],[Remark],[Description],[Other],[PackingAmt],[OtherSign],[OtherAmt],[DiscPer],[DiscAmt],[TaxPer],[TaxAmt],[TotalQty],[GrossAmt],[NetAmt],[ROSign],[RoundOff],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SpecialDscPer],[SpecialDscAmt],[TaxFree],[GodownName],[LRNumber],[LRDate],[StockStatus],[DueDate],[NoOfPacks],[PackWeight],[TransportMode],[ChallanNo],[SupplierRefNo],[Agent],[ReceivedBy],[CountedBy],[BarCodedBy],[Other1],[Other2],[Other3],[TCSPer],[TCSAmt],[TaxableAmt]) VALUES "
                                       + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + txtInvoiceNo.Text + "'," + strInvoiceDate + ",'" + strPurchasePartyID + "','" + strPurchaseParty + "','"+txtTaxLedger.Text+"','"+ txtTransport.Text+"','" + txtRemark.Text + "','"+txtImportData.Text+"','" + txtOtherPerSign.Text + "'," + dba.ConvertObjectToDouble(txtPackingAmt.Text)+",'"+ txtSign.Text+"',"+dba.ConvertObjectToDouble(txtOtherAmt.Text)+","+dba.ConvertObjectToDouble(txtOtherPer.Text)+","+dba.ConvertObjectToDouble(txtDiscAmt.Text)+","+dba.ConvertObjectToDouble(txtTaxPer.Text)+","+ dTaxAmt + ","
                                       + dba.ConvertObjectToDouble(lblTotalQty.Text)+","+dba.ConvertObjectToDouble(lblGrossAmt.Text)+","+dAmt+",'"+txtROSign.Text+"',"+txtRoundOff.Text+",'"+MainPage.strLoginName+ "','',1,0," + dSpclDisPer + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ","+ dTaxFree+",'" + txtGodown.Text + "','" + txtLRNo.Text + "'," + strLRDate + ",'" + txtStockStatus.Text + "',"+strDueDate+",'"+txtNoOfPacks.Text+"','"+txtWeight.Text+"','"+txtMode.Text+ "','','','" + strMNID + "','"+txtReceivedBy.Text+"','"+txtCountedBy.Text+"','"+txtBarcodedBy.Text+ "','','',''," + dTcsPer + "," + dTCSAmt + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ")  "
                                       + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  " 
                                       + " ('" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "')  ";

                double dQty = 0, dRate = 0, _dDisPer = 0, dMRP =0,_dAmt=0,dSaleMargin=0,dSaleMRP=0, dWSDis = 0, dWSMRP = 0, dSaleDis = 0, dSaleRate = 0,dCompanyMargin=0,dCompanyMRP=0,dGSTAmt=0;
                string strSDis = "",strHSNCode="",strBarCode="", strCompanyCode="";
                if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                    strPurchaseParty = strPurchasePartyID = "";
                if (MainPage._bPurchaseBillWiseMargin || MainPage._bFixedMargin)
                    dSaleMargin = ConvertObjectToDouble(txtProfitMargin.Text);

                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    _dDisPer = ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    _dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin)
                        dSaleMargin = dba.ConvertObjectToDouble(row.Cells["saleMargin"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                    strHSNCode = Convert.ToString(row.Cells["hsnCode"].Value);
                    dWSDis = dba.ConvertObjectToDouble(row.Cells["wsDisc"].Value);
                    dWSMRP = dba.ConvertObjectToDouble(row.Cells["wsMRP"].Value);
                    dSaleDis = dba.ConvertObjectToDouble(row.Cells["saleDis"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                    dCompanyMargin = dba.ConvertObjectToDouble(row.Cells["cMargin"].Value);
                    dCompanyMRP = dba.ConvertObjectToDouble(row.Cells["cMrp"].Value);
                    dGSTAmt = dba.ConvertObjectToDouble(row.Cells["gstAmt"].Value);
                    strBarCode = Convert.ToString(row.Cells["barCode"].Value);

                    if (strBarCode == "")
                    {
                        if (MainPage._bBarCodeStatus)
                            strBarCode = dba.GetBarCode(txtBillNo.Text, _index,"");
                        else
                            strBarCode = "";

                        if (strCompanyCode == "")
                            strCompanyCode = MainPage.strDataBaseFile;

                        //if (strCompanyCode != "" && strBarCode != "")
                        //    strBarCode = strCompanyCode + "-" + strBarCode;

                        if (MainPage._bCustomPurchase && strBarCode == "")
                            strBarCode = strCompanyCode;
                    }

                    strSDis = "";

                    strQuery += " INSERT INTO [dbo].[PurchaseBookSecondary] ([BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Discount],[OCharges],[BasicAmt],[UnitName],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PONumber],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SaleMargin],[SaleMRP],[WSDis],[WSMRP],[SaleDis],[SaleRate],[TaxAmt],[MarginType],[CompanyMarginType],[CompanyMargin],[CompanyMRP]) VALUES  "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ","
                                  + " " + _dAmt + ",0,0, " + _dAmt + ",'" + row.Cells["unitName"].Value + "',0,'" + MainPage.strLoginName + "','',1,0,'','" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','',''," + dSaleMargin + "," + dSaleMRP + "," + dWSDis + "," + dWSMRP + "," + dSaleDis + "," + dSaleRate + ","+ dGSTAmt+",'"+row.Cells["marginType"].Value+ "','" + row.Cells["cmarginType"].Value + "',"+dCompanyMargin+ "," + dCompanyMRP + ")";
                   
                        if (txtStockStatus.Text == "STOCK IN")
                        {
                            strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                 + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + strPurchasePartyID + "','" + strPurchaseParty + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'" + strSDis + "','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                        }
                    if (strHSNCode != "")
                    {
                        strQuery += " if not exists(Select ItemName from[dbo].[ItemMapping]  Where ItemName = '" + row.Cells["itemName"].Value + "' and DesignName = '" + row.Cells["styleName"].Value + "' and UpdatedBy = '" + strHSNCode + "' ) begin "
                                          + " INSERT INTO [dbo].[ItemMapping] ([ItemName],[DesignName],[Date],[CreatedBy],[UpdatedBy]) Values ('" + row.Cells["itemName"].Value + "','" + row.Cells["styleName"].Value + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','" + strHSNCode + "') end";
                    }

                    _index++;
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@TCSAccount nvarchar(250); ";
                           
                if (dTaxAmt > 0 && txtTaxLedger.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" +txtBillCode.Text+" "+ txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                //GST Details
                string strTaxAccountID = "";
                foreach (DataGridViewRow rows in dgrdTax.Rows)
                {
                    strTaxAccountID = "";
                    strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strTaxAccountID = strFullName[0].Trim();
                    }

                    strQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                   + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                if (dTCSAmt > 0)
                {
                    strQuery += " Select @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }


                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";
                
                strQuery += " end ";

                if (strQuery != "")
                {
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add(F2)";
                        AskForPrint();

                        BindRecordWithControl(txtBillNo.Text);                       
                    }
                    else
                        MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (btnEdit.Text == "&Edit(F6)")
                {
                    if (btnAdd.Text == "&Save(F5)")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        btnAdd.Text = "&Add(F2)";
                        BindLastRecord();
                    }
                    btnEdit.Text = "&Update(F6)";
                    EnableAllControls();
                    dgrdDetails.ReadOnly = qtyAdjustStatus;
                    txtBillNo.ReadOnly = true;
                    if(dgrdDetails.Rows.Count==0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                    if (txtStockStatus.Text == "")
                        txtStockStatus.Text = "STOCK IN";
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
                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit(F6)";
                            BindRecordWithControl(txtBillNo.Text);                            
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

        private int UpdateRecord(string strSubQuery)
        {
            int result = 0;
            try
            {
                string strInvoiceDate = "NULL", strTaxAccountID = "", strLRDate="NULL",strDueDate="NULL",strMNID="";
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (txtInvoiceDate.Text.Length == 10)
                {
                    DateTime iDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text);
                    strInvoiceDate = "'" + iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtLRDate.Text.Length == 10 && txtLRNo.Text != "")
                {
                    DateTime lDate = dba.ConvertDateInExactFormat(txtLRDate.Text);
                    strLRDate = "'" + lDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtDueDate.Text.Length == 10)
                {
                    DateTime dDate = dba.ConvertDateInExactFormat(txtDueDate.Text);
                    strDueDate = "'" + dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                double dAmt = Convert.ToDouble(lblNetAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dSpclDisPer = ConvertObjectToDouble(txtSpclDisPer.Text),dTaxFree=ConvertObjectToDouble(txtTaxFreeSign.Text + txtTaxFreeAmt.Text), dTcsPer = 0, dTCSAmt = 0;
                string strPurchaseParty = "", strPurchasePartyID = "";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }
                if (txtmanufacturer.Text!="")
                {
                    strFullName = txtmanufacturer.Text.Split(' ');
                    strMNID = strFullName[0];
                }
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                }

                string strQuery = " if exists (Select [BillCode] from [PurchaseBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin UPDATE [dbo].[PurchaseBook] Set [Date]='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[InvoiceNo]='" + txtInvoiceNo.Text + "',[InvoiceDate]=" + strInvoiceDate + ",[PurchasePartyID]='" + strPurchasePartyID + "',[PurchaseParty]='" + strPurchaseParty + "',[PurchaseType]='" + txtTaxLedger.Text + "',[TransportName]='" + txtTransport.Text + "',"
                                + " [Remark]='" + txtRemark.Text + "',[Description]='" + txtImportData.Text + "',[Other]='" + txtOtherPerSign.Text + "',[PackingAmt]=" + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[DiscPer]=" + dba.ConvertObjectToDouble(txtOtherPer.Text) + ",[DiscAmt]=" + dba.ConvertObjectToDouble(txtDiscAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dTaxAmt + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ","
                                + " [TotalQty]=" + dba.ConvertObjectToDouble(lblTotalQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dAmt + ",[ROSign]='" + txtROSign.Text + "',[RoundOff]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[TaxFree]="+dTaxFree+ " ,[GodownName]='" + txtGodown.Text + "',"
                                + " [LRNumber]='" + txtLRNo.Text + "',[LRDate]=" + strLRDate + ",[StockStatus]='" + txtStockStatus.Text + "',[DueDate]=" + strDueDate + ",[NoOfPacks]='" + txtNoOfPacks.Text + "',[PackWeight]='" + txtWeight.Text + "',[TransportMode]='" + txtMode.Text + "',[ChallanNo]='',[SupplierRefNo]='',[Agent]='" + strMNID + "',[ReceivedBy]='" + txtReceivedBy.Text + "',[CountedBy]='" + txtCountedBy.Text + "',[BarCodedBy]='" + txtBarcodedBy.Text + "',[TCSPer]=" + dTcsPer + ",[TCSAmt]=" + dTCSAmt + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[PartyName]='" + strPurchaseParty + "',[Amount]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strPurchasePartyID + "' Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='PURCHASE A/C'  "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from StockMaster Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                string strID = "", strSDis = "", strBarCode = "", strCompanyCode="";
                double dQty = 0, dCompanyMargin=0, dCompanyMRP=0, dRate = 0, _dDisPer = 0, dMRP = 0, _dAmt = 0, dSaleMargin = 0, dSaleMRP = 0, dWSDis = 0, dWSMRP = 0, dSaleDis = 0, dSaleRate = 0,dGstAmt=0;


                strPurchaseParty = strPurchasePartyID = "";
                int _index = (int)dRowCount;
                if (MainPage._bPurchaseBillWiseMargin || MainPage._bFixedMargin)
                    dSaleMargin = ConvertObjectToDouble(txtProfitMargin.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                   
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    _dDisPer = ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    _dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin)
                        dSaleMargin = dba.ConvertObjectToDouble(row.Cells["saleMargin"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                    dWSDis = dba.ConvertObjectToDouble(row.Cells["wsDisc"].Value);
                    dWSMRP = dba.ConvertObjectToDouble(row.Cells["wsMRP"].Value);
                    dSaleDis = dba.ConvertObjectToDouble(row.Cells["saleDis"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                    dCompanyMargin = dba.ConvertObjectToDouble(row.Cells["cMargin"].Value);
                 dCompanyMRP = dba.ConvertObjectToDouble(row.Cells["cMrp"].Value);
            dGstAmt = dba.ConvertObjectToDouble(row.Cells["gstAmt"].Value);
                    strBarCode = Convert.ToString(row.Cells["barCode"].Value);

                    if (strBarCode == "")
                    {
                        if (MainPage._bBarCodeStatus)
                            strBarCode = dba.GetBarCode(txtBillNo.Text, _index,"");
                        else
                            strBarCode = "";

                        if (strCompanyCode == "")
                            strCompanyCode = MainPage.strDataBaseFile;

                        //if (strCompanyCode != "" && strBarCode != "")
                        //    strBarCode = strCompanyCode + "-" + strBarCode;

                        if (MainPage._bCustomPurchase && strBarCode == "")
                            strBarCode = strCompanyCode;
                    }

                    strSDis = "";

                    strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[PurchaseBookSecondary] ([BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Discount],[OCharges],[BasicAmt],[UnitName],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PONumber],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SaleMargin],[SaleMRP],[WSDis],[WSMRP],[SaleDis],[SaleRate],[TaxAmt],[MarginType],[CompanyMarginType],[CompanyMargin],[CompanyMRP]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ","
                                + " " + _dAmt + ",0,0, " + _dAmt + ",'" + row.Cells["unitName"].Value + "',0,'" + MainPage.strLoginName + "','',1,0,'','" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','',''," + dSaleMargin + "," + dSaleMRP + "," + dWSDis + "," + dWSMRP + "," + dSaleDis + "," + dSaleRate + "," + dGstAmt + ",'" + row.Cells["marginType"].Value + "','" + row.Cells["cmarginType"].Value + "'," + dCompanyMargin + "," + dCompanyMRP + ") ";
                    }
                    else
                    {
                        strQuery += " Update [dbo].[PurchaseBookSecondary] SET [ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + _dDisPer + ","
                                 + " [Rate]=" + dRate + ",[Amount]=" + _dAmt + ",[BasicAmt]=" + _dAmt + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[PONumber]='',[BarCode]='" + strBarCode + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[SaleMargin]=" + dSaleMargin + ",[SaleMRP]=" + dSaleMRP + ",[WSDis]=" + dWSDis + ",[WSMRP]=" + dWSMRP + ",[SaleDis]=" + dSaleDis + ",[SaleRate]=" + dSaleRate + ",[TaxAmt]=" + dGstAmt + ",[MarginType]='" + row.Cells["marginType"].Value + "',[CompanyMarginType]='" + row.Cells["cmarginType"].Value + "',[CompanyMargin]=" + dCompanyMargin + ",[CompanyMRP]=" + dCompanyMRP + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + "  ";
                    }

                    if (txtStockStatus.Text == "STOCK IN")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + strPurchasePartyID + "','" + strPurchaseParty + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'" + strSDis + "','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                    }
                    _index++;
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@TCSAccount nvarchar(250); ";

                if (dTaxAmt > 0 && txtTaxLedger.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end  end ";
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
                                   + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                if (dTCSAmt > 0)
                {
                    strQuery += " Select @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                        + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                strQuery += "  end";

                // end Purchase Entry

                //object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");


                result = dba.ExecuteMyQuery(strQuery);
            }
            catch
            {
            }
            return result;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add(F2)";
            btnEdit.Text = "&Edit(F6)";
            BindLastRecord();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtInvoiceDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtPurchaseLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTaxLedger.Text = objSearch.strSelectedData;
                        GetRowTaxAmt();
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
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00" || txtNew.Text == "0.0000")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }
    
        private void txtRoundOff_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender,e);
        }

        private void txtROSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearAllText();
            }
            dba.ChangeLeaveColor(sender, e);
            //else if (txtSerialNo.Text != "")
            //    CheckSerialNoAvailability();
        }
        
        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
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
            DataTable myDataTable = new DataTable();
            try
            {
                ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("SerialNo", typeof(String));
                myDataTable.Columns.Add("PParty", typeof(String));
                myDataTable.Columns.Add("SParty", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("NetAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("SupplierHead", typeof(String));
                myDataTable.Columns.Add("SubParty", typeof(String));
                myDataTable.Columns.Add("Freight", typeof(String));
                myDataTable.Columns.Add("Tax", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("Packing", typeof(String));
                myDataTable.Columns.Add("Remark", typeof(String));
                myDataTable.Columns.Add("InvoiceNo", typeof(String));
                myDataTable.Columns.Add("InvoiceDate", typeof(String));
                myDataTable.Columns.Add("BarValue", typeof(String));

                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                DataRow row = myDataTable.NewRow();

                double dOtherAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text) + dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmt.Text);

                if (MainPage.strSoftwareType == "AGENT")
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                else
                    row["CompanyName"] = "";
                if (!btnPrint.Enabled)
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text + "/D";
                else
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text;

                row["InvoiceNo"] = txtInvoiceNo.Text;
                row["InvoiceDate"] = txtInvoiceDate.Text;
                row["BarValue"] = txtBillCode.Text + txtBillNo.Text;

                row["SupplierHead"] = "SUPPLIER";
                row["PParty"] = txtPurchaseParty.Text;

                row["SParty"] = "----";
                row["SubParty"] = "----";
                row["Date"] = txtDate.Text;
                row["Qty"] = lblTotalQty.Text + " Pcs";
                row["Amount"] = lblGrossAmt.Text;
                row["Tax"] = txtTaxAmt.Text;
                if (txtOtherPerSign.Text == "-")
                    row["Freight"] = txtDiscAmt.Text;
                if (dOtherAmt >= 0)
                    row["Packing"] = "(+)" + dOtherAmt.ToString("N2", MainPage.indianCurancy);
                else
                    row["Packing"] = "(-)" + Math.Abs(dOtherAmt).ToString("N2", MainPage.indianCurancy);

                row["NetAmount"] = lblNetAmt.Text;
                row["Remark"] = txtRemark.Text;

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;
              
                row["UserName"] = MainPage.strLoginName + " ,  Date & Time : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void AskForPrint()
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    DialogResult result = MessageBox.Show("ARE YOU WANT TO PRINT PURCHASE SLIP ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            DataTable dt = CreateDataTable();
                            if (dt.Rows.Count > 0)
                            {
                                Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                                objReport.SetDataSource(dt);
                                if (MainPage._PrintWithDialog)
                                    dba.PrintWithDialog(objReport);
                                else
                                    objReport.PrintToPrinter(1, false, 0, 1);

                                objReport.Close();
                                objReport.Dispose();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
        

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("Purchase Receiving Slip");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.myPreview.ShowPrintButton = false;
                    objShow.myPreview.ShowExportButton = false;
                    objShow.ShowDialog();

                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {

                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;
                        defS.Copies = (short)MainPage.iNCopyPurchase;

                        Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport, false, MainPage.iNCopyPurchase);
                        else
                            objReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        btnPrint.Enabled = true;
                        objReport.Close();
                        objReport.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Printing in Goods Receive ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }

        private void PurchaseBook_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void SetPermission()
        {
            if (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView)
            {
                if (!MainPage.mymainObject.bPurchaseAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseView)
                    txtBillNo.Enabled = false;

                txtProfitMargin.Enabled = lblProfitMargin.Enabled = MainPage._bPurchaseBillWiseMargin;
                btnBarCodePrint.Enabled = MainPage.mymainObject.bBarcodePrint;

                //if (MainPage._bPurchaseBillWiseMargin)
                //    txtProfitMargin.Text = MainPage.dPurchaseBillMargin.ToString();
                //else if (MainPage._bFixedMargin)
                //    txtProfitMargin.Text = MainPage.dFixedMargin.ToString();

            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private double GetRowTaxAmt(DataGridViewRow rows)
        {
            double dTax = 0;
            try
            {
                if (MainPage._bTaxStatus)
                {
                    if (txtTaxLedger.Text != "")
                    {
                        double dRate = 0;
                        dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);                       
                      
                        if (dRate > 0)
                        {  
                            string strQuery = " if exists (Select TaxName from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "' and TaxIncluded=0) begin "
                                            + " Select(((" + dRate + ")*GM.TaxRate)/100.00)TaxAmt from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((" + dRate + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((" + dRate + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dRate + ">0 end ";

                            object objTax = DataBaseAccess.ExecuteMyScalar(strQuery);
                            dTax = ConvertObjectToDouble(objTax);
                        }
                        rows.Cells["gstAmt"].Value = dTax;
                    }
                }
            }
            catch { }
            return dTax;
        }

        private double GetRowTaxAmt()
        {
            double dTax = 0;
            try
            {

                if (txtTaxLedger.Text != "" && MainPage._bTaxStatus)
                {
                    double dRate = 0;
                    foreach (DataGridViewRow rows in dgrdDetails.Rows)
                    {
                        dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                        if (dRate > 0)
                        {

                            string strQuery = " if exists (Select TaxName from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "' and TaxIncluded=0) begin "
                                            + " Select (((" + dRate + ")*GM.TaxRate)/100.00)TaxAmt from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((" + dRate + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((" + dRate + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dRate + ">0 end ";

                            object objTax = DataBaseAccess.ExecuteMyScalar(strQuery);
                            dTax = ConvertObjectToDouble(objTax);
                        }
                        rows.Cells["gstAmt"].Value = dTax;
                    }
                }
                else
                {
                    foreach (DataGridViewRow rows in dgrdDetails.Rows)
                    {
                        rows.Cells["gstAmt"].Value = 0;
                    }
                }
            }
            catch { }
            return dTax;
        }

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtTaxLedger.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtTaxLedger.Text, "PURCHASE");
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

                            string strQuery = "", strSubQuery = "", strGRSNo = "";
                            double dDisStatus = 0;

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;
                          

                            double dRate = 0, dQty = 0, dAmt = 0, dBasicAmt = 0, dOAmt = 0; 
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                                dAmt = dRate * dQty;
                                dAmt=Math.Round(dAmt, 2);

                                dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                                dOAmt += (dBasicAmt - dAmt);                               
                                                               
                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
                                }
                            }

                            if (dOAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOAmt + " Amount,12 as TaxRate ";
                            }

                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount,0 as TaxRate ";
                            }
                            if (strQuery != "")
                            {

                                strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100.00/(100.00+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";


                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    //BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    dTaxAmt = dTTaxAmt;
                                    dTaxPer = dMaxRate;
                                   // pnlTax.Visible = true;
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
                           // pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text = "0.00";
                    }
                }
                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bPurchaseAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseEdit)
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


        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && txtBillNo.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " Delete from PurchaseBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                            + " Delete from PurchaseBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                            + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('PURCHASE A/C','DUTIES & TAXES')  "
                                            + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                            + " Delete from StockMaster Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

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
            catch
            {
            }
            btnFinalDelete.Enabled = true;
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtPartyName_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }     

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save(F5)")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("PURCHASE", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtOtherPerSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void txtOtherPerSign_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (txtOtherPerSign.Text == "")
                        txtOtherPerSign.Text = "+";
                    CalculateAllAmount();
                }
            }
            catch { }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtDiscPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 3);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
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

        private void txtOrderNo_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            //    {
            //        char objChar = Convert.ToChar(e.KeyCode);
            //        int value = e.KeyValue;
            //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            //        {
            //            SearchData objSearch = new SearchData("RETAILORDERNO", "SEARCH RETAIL ORDER NO", e.KeyCode);
            //            objSearch.ShowDialog();
            //            txtOrderNo.Text = objSearch.strSelectedData;
            //        }
            //    }
            //    e.Handled = true;
            //}
            //catch
            //{
            //}
        }

        private void txtSpclDisPer_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSpclDisPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtSpclDisPer.Text == "")
                    txtSpclDisPer.Text = "0";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
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
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchDataOther objSearch = new SearchDataOther("PURCHASEBILLNO", "", "SEARCH PURCHASE BILL NO", e.KeyCode, false);
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
            if (txtImportData.Text != "" && btnAdd.Text == "&Save(F5)")
            {
                BindRecordWithControlWithImport();
            }
        }

        private void BindRecordWithControlWithImport()
        {
            try
            {
                string strQuery = " Select PBS.*,GAmt from PurchaseBookSecondary PBS Outer APPLY (Select (CASE WHEN PurchaseType Like('%INCLUDE%') then (GrossAmt-TaxAmt) else GrossAmt end)GAmt,ISNULL(SpecialDscPer,0)SpecialDscPer from PurchaseBook PB Where PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo) PB  Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'  order by ID ";

                DataSet ds = SearchDataOther.GetDataSet(strQuery);
                DataTable dt = null;
                dgrdDetails.Rows.Clear();

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow _row = dt.Rows[0]; 
                            txtOtherAmt.Text = Convert.ToString(_row["GAmt"]);
                            txtSign.Text = "-";

                            int rowIndex = 0;

                            dgrdDetails.Rows.Add(dt.Rows.Count);
                            foreach (DataRow row in dt.Rows)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
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
                                dgrdDetails.Rows[rowIndex].Cells["saleMargin"].Value = ConvertObjectToDouble(row["SaleMargin"]);
                                dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = ConvertObjectToDouble(row["SaleMRP"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["wsDisc"].Value = row["WSDis"];
                                dgrdDetails.Rows[rowIndex].Cells["wsMRP"].Value = row["WSMRP"];
                                dgrdDetails.Rows[rowIndex].Cells["saleDis"].Value = row["SaleDis"];
                                dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];
                                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                                dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                                dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                                dgrdDetails.Rows[rowIndex].Cells["hsnCode"].Value = row["HSNCode"];
                                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
                                dgrdDetails.Rows[rowIndex].Cells["marginType"].Value = row["marginType"];
                                dgrdDetails.Rows[rowIndex].Cells["cmarginType"].Value = row["CompanyMarginType"];
                                dgrdDetails.Rows[rowIndex].Cells["cMargin"].Value = row["CompanyMargin"];
                                dgrdDetails.Rows[rowIndex].Cells["cMrp"].Value = row["CompanyMRP"];
                                dgrdDetails.Rows[rowIndex].Cells["gstAmt"].Value = row["TaxAmt"];

                                rowIndex++;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private bool ValidateFromPreviousBill(bool _bStatus)
        {
            string strPurchasePartyID = "";
            string[] strFullName = txtPurchaseParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strPurchasePartyID = strFullName[0].Trim();
            
            string strQuery = "Select BillNo from PurchaseBook  Where BillNo!=" + txtBillNo.Text + " and PurchasePartyID='" + strPurchasePartyID + "' and LTRIM(RTRIM(InvoiceNo)) Like('" + txtInvoiceNo.Text.Trim() + "') ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (Convert.ToString(objValue) != "")
            {
                MessageBox.Show("Sorry ! This detail might be saved in Bill no : " + objValue + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (_bStatus)
                    txtInvoiceNo.Focus();
                return false;
            }
            else
                return true;
        }

        private void chkImportPDF_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkImportPDF.Enabled = false;
                if (btnAdd.Text == "&Save(F5)")
                {
                    if (chkImportPDF.Checked)
                    {
                        string strFilePath = "";
                        OpenFileDialog _browser = new OpenFileDialog();
                        _browser.Filter = "PURCHASE PDF Files (*.pdf)|*.pdf;";
                        _browser.ShowDialog();
                        if (_browser.FileName != "")
                        {
                            strFilePath = _browser.FileName;
                            txtPDFFileName.Text = _browser.SafeFileName;
                            ExtractDataFromPDF(strFilePath);
                            _strPDFFilePath = strFilePath;
                        }
                    }
                    else
                    {
                        _strPDFFilePath = txtPDFFileName.Text = "";
                    }
                }
            }
            catch
            {
            }
            chkImportPDF.Enabled = true;
        }

        private void CopyPDFFileInGSTFolder()
        {
            try
            {
                if (_strPDFFilePath != "")
                {
                    string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Imported_PDF";
                    System.IO.Directory.CreateDirectory(strPath);

                    strPath += "\\" + txtBillNo.Text + ".pdf ";

                    if (System.IO.File.Exists(_strPDFFilePath))
                        System.IO.File.Copy(_strPDFFilePath, strPath, true);
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void DeleteImortedPDFFileInGSTFolder()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Imported_PDF";
                System.IO.Directory.CreateDirectory(strPath);
                strPath += "\\" + txtBillNo.Text + ".pdf ";
                if (System.IO.File.Exists(_strPDFFilePath))
                    System.IO.File.Delete(strPath);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnOpenFile.Enabled = false;

                if (txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save(F5)")
                {
                    DataBaseAccess.ShowPDFFiles(txtBillCode.Text, txtBillNo.Text);
                }
            }
            catch
            {
            }
            btnOpenFile.Enabled = true;
        }

        private void ExtractDataFromPDF(string strPath)
        {
            try
            {
                ClearAllTextForPDF();
                SetSerialNo();

                PdfReader reader = new PdfReader(strPath);
                int PageNum = reader.NumberOfPages;
                string text = "";
                int _itemIndex = 0;
                dgrdDetails.Rows.Clear();
                bool _bEndStatus = false, _bLongLable = false, bByteData = false;
                for (int i = 1; i <= PageNum; i++)
                {
                    try
                    {
                        text = PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());
                    }
                    catch { text = ReadPDFData.GetTextFromPDF(strPath, i); bByteData = true; }

                    if (i == 1)
                    {
                        if (text.Trim() == "")
                        {
                            MessageBox.Show("Sorry ! Please select valid pdf file !! ", "PDF file not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                        SetBasicDetails(ref _itemIndex, text);
                    }
                    else
                    {
                        if ((txtPurchaseParty.Text.Contains("FULLTOSS") || txtPurchaseParty.Text.Contains("JAI AMBEY")) && i == 2)
                            _itemIndex -= 8;
                        else if ((txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP")) && i == 2)
                            _itemIndex -= 22;
                        else if ((txtPurchaseParty.Text.Contains("MISHU ENTERPRISES")) && i == 2)
                            _itemIndex = 19;
                        else if (txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS"))
                            _itemIndex = 10;
                    }

                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP") || txtPurchaseParty.Text.Contains("BONNYS NX"))
                        _bEndStatus = SetItemDetailsByCustomize_Branches(_itemIndex, text, ref _bLongLable);
                    else if (txtPurchaseParty.Text.Contains("SARAOGI SUPER SALES") || txtGSTNo.Text.Contains("AAYCS8982Q"))
                        _bEndStatus = SetItemDetailsByCustomize_Saraogi(_itemIndex, text, ref _bLongLable);
                    else if (txtPurchaseParty.Text.Contains("LUCKY JACKET") || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS") || txtPurchaseParty.Text.Contains("KC GARMENTS") || txtPurchaseParty.Text.Contains("JANAK GARMENTEX") || txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") || txtPurchaseParty.Text.Contains("NIKUNJ TRADING") || txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") || txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS") || txtPurchaseParty.Text.Contains("DONARGOLD GARMENTS") || txtPurchaseParty.Text.Contains("W STAN GARMENTS") || txtPurchaseParty.Text.Contains("GEX GARMENTS") || txtPurchaseParty.Text.Contains("MOTI FASHIONS") || txtPurchaseParty.Text.Contains("TANEJA FASHION") || txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") || txtPurchaseParty.Text.Contains("VIPIN COLLECTION") || txtPurchaseParty.Text.Contains("JOLLY FASHIONS") || txtPurchaseParty.Text.Contains("CHANCELLOR INDUSTRIES") || txtPurchaseParty.Text.Contains("MAA PADMAVATI APPARELS") || txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("CLASSIN APPARELS") || txtPurchaseParty.Text.Contains("MAUZ FASHIONS") || txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("AASHI COLLECTION") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") || txtPurchaseParty.Text.Contains("S.R CREATION") || txtPurchaseParty.Text.Contains("SHUBHI GARMENTS") || txtPurchaseParty.Text.Contains("WORLD CHOICE") || txtPurchaseParty.Text.Contains("MIKEY FASHION") || txtPurchaseParty.Text.Contains("WORLD SAHAB") || txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) //|| txtPurchaseParty.Text.Contains("N.D. FASHION")|| txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES")
                        _bEndStatus = SetItemDetailsByCustomize_Delhi(_itemIndex, text, ref _bLongLable);
                    else if (_strBillType == "BUSY" || txtPurchaseParty.Text.Contains("SHRI KRISHNA GARMENTS"))
                        _bEndStatus = SetItemDetailsLineByBusy(_itemIndex, text);
                    else if (bByteData)
                        _bEndStatus = SetItemDetailsLineByLine(_itemIndex, text);
                    else
                        _bEndStatus = SetItemDetails(_itemIndex, text, ref _bLongLable);

                    if (_bEndStatus)
                        break;
                }
                //txtSalesParty.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            CalculateAllAmount();

            if (txtInvoiceNo.Text != "")
                ValidateFromPreviousBill(false);
        }


        private bool SetItemDetails(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :"))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        string __strFreight = _data[_data.Length - 1];
                        if (__strFreight.Length == 4 && (__strFreight.Contains("62") || __strFreight.Contains("63")))
                        {
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmt.Text = _data[0];
                        }
                        else
                            txtOtherAmt.Text = _data[_data.Length - 1];
                    }
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtPackingAmt.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("CHARGES"))
                {
                    strText = _lines[_index + 1].Trim();
                    if (strText != "")
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(strText).ToString("0.00");
                }
                else if (!strText.Contains("Less") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST") && !strText.ToUpper().Contains("DISCOUNT") && !strText.ToUpper().Contains("JURISDICTION") && !strText.ToUpper().Contains("COMPUTER"))
                {
                    strItem = strQty = strRate = "";
                    strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] _data = strText.Split(' ');
                    if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        if (_data.Length < 8)
                        {
                            if (_data.Length > 1)
                            {
                                strHSNCode = _data[0];
                                for (int i = 1; i < _data.Length - 4; i++)
                                {
                                    if (_data[i].Contains(".00"))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " " + _data[i];
                                        else
                                            strItem = _data[i].Trim();
                                    }
                                }

                                strText = _lines[_index + 1].Trim();
                                _data = strText.Split(' ');
                                if (_data.Length > 1)
                                {
                                    strQty = _data[0];
                                    strRate = _data[_data.Length - 1];
                                }
                                _index++;
                            }
                        }
                        else
                        {
                            strHSNCode = _data[0];
                            for (int i = 1; i < _data.Length - 4; i++)
                            {
                                if (_data[i].Contains(".00"))
                                    break;
                                else
                                {
                                    if (strItem != "")
                                        strItem += " " + _data[i];
                                    else
                                        strItem = _data[i].Trim();
                                }
                            }

                            strQty = _data[_data.Length - 6];
                            strRate = _data[_data.Length - 1];
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("TAANI INDUSTRIES PVT LTD") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                strDescription = _lines[_index + 1].Trim();

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - 2];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_data.Length - 3];
                                    strRate = _data[_data.Length - 4];
                                }
                                else
                                    strRate = _data[_data.Length - 3];
                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "" && !strDescription.Contains(",") && !strDescription.Contains(".00") && !strDescription.ToLower().Contains("continue") && strDescription.Length > 2)
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                strDescription = _lines[_index + 1].Trim();

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - 2];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_data.Length - 3];
                                    strRate = _data[_data.Length - 4];
                                }
                                else
                                    strRate = _data[_data.Length - 3];
                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "" && !strDescription.Contains(",") && !strDescription.Contains(".00") && !strDescription.ToLower().Contains("continue") && strDescription.Length > 2)
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("SNS GARMENTS"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                int _qtyIndex = 0;
                                strDescription = _lines[_index + 1].Trim();
                                if (strDescription.ToUpper().Contains("CONTINUED"))
                                    strDescription = "";

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - _qtyIndex - 2];
                                if (strQty == "%")
                                    _qtyIndex = 3;
                                //strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                //if (strQty == "" || strQty == "." || strQty == "..")
                                //    _qtyIndex++;

                                strQty = _data[_data.Length - _qtyIndex - 2];
                                strRate = _data[_data.Length - _qtyIndex - 3];

                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "")
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (_data.Length > 7 || ((txtPurchaseParty.Text.Contains("SOMANI") || txtPurchaseParty.Text.Contains("BATRA EXCLUSIVE") || txtPurchaseParty.Text.Contains("G.D. CREATION") || txtPurchaseParty.Text.Contains(" P.R. ENTERPRISES")) && _data.Length > 6))
                    {
                        int _length = _data.Length;
                        _bLongLable = true;
                        if (txtPurchaseParty.Text.Contains("SPARKY"))
                        {
                            strHSNCode = _data[_length - 7];
                            strQty = _data[_length - 6];
                            strRate = _data[_length - 4];

                            for (int i = 0; i < _length - 7; i++)
                            {
                                if (strItem != "")
                                    strItem += " " + _data[i];
                                else if (i == 0)
                                    strItem = _data[i].Replace(_lineIndex.ToString(), "").Trim();
                                else
                                    strItem = _data[i].Trim();
                            }
                        }
                        else if (txtPurchaseParty.Text.Contains("JAI AMBEY"))
                        {
                            _data = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Split(' ');
                            if (_data.Length > 9)
                            {
                                _length = _data.Length;
                                strHSNCode = _data[_length - 8];
                                strQty = _data[_length - 3];
                                strRate = _data[_length - 7];

                                for (int i = 0; i < _length - 8; i++)
                                {
                                    if (strItem != "")
                                        strItem += " " + _data[i];
                                    else if (i == 0)
                                    {
                                        strItem = _data[i].Replace(_lineIndex.ToString(), "").Trim();
                                    }
                                    else
                                        strItem = _data[i].Trim();
                                }
                            }
                        }
                        else
                        {
                            strHSNCode = _data[_length - 1];
                            int qtyIndex = 0;
                            string strDescription = "";
                            if (_data[_length - 2] == "%")
                            {
                                strQty = _data[_length - 5];
                                strRate = _data[_length - 6];
                                qtyIndex = 1;
                            }
                            else
                            {
                                strQty = _data[_length - 3];
                                strRate = _data[_length - 4];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_length - 4];
                                    strRate = _data[_length - 5];
                                    qtyIndex = 1;
                                }
                                strRate = System.Text.RegularExpressions.Regex.Replace(strRate, "[^0-9.]", "");
                                if (strRate == "" || strRate == "." || strRate == "..")
                                {
                                    strQty = _data[_length - 5];
                                    strRate = _data[_length - 6];
                                    qtyIndex = 2;
                                }
                            }

                            strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                            int _hsnIndex = 0;
                            if (strHSNCode == "")
                            {
                                string[] _hsnData = _lines[_index + 1].Trim().Split(' ');
                                strHSNCode = _hsnData[0];
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if (strHSNCode.Length != 4)
                                    strHSNCode = _hsnData[_hsnData.Length - 1];
                                strQty = _data[_length - 2];
                                strRate = _data[_length - 3];
                                _hsnIndex--;
                            }

                            int colIndex = 6;
                            if (strText.Contains("%"))
                                colIndex = 8;
                            if (Regex.Matches(strText, "%").Count > 1)
                                colIndex = 9;
                            string __strItem = "";
                            for (int i = 0; i < (_length - colIndex - _hsnIndex - qtyIndex); i++)
                            {
                                __strItem = _data[i];
                                if (__strItem != "" && !__strItem.Contains(".00") && !__strItem.Contains(".10") && !__strItem.Contains(".20") && !__strItem.Contains(".30") && !__strItem.Contains(".40") && !__strItem.Contains(".50") && !__strItem.Contains(".60") && !__strItem.Contains(".70") && !__strItem.Contains(".80") && !__strItem.Contains(".90"))
                                {
                                    if (strItem != "")
                                        strItem += " " + __strItem;
                                    else if (__strItem.Length > 2)
                                    {
                                        if (__strItem.Substring(0, 2).Trim() == _lineIndex.ToString() || __strItem.Substring(0, 2).Trim() == _lineIndex.ToString() + ".")
                                            __strItem = __strItem.Substring(2, __strItem.Length - 2);
                                        strItem = __strItem.Trim();
                                    }
                                }
                            }
                            if (strItem == "" && strHSNCode != "" && strRate != "")
                                strItem = _data[0];
                            if (txtPurchaseParty.Text.Contains("ENGLISH CHANNEL") && !_lines[_index + 1].ToUpper().Contains("GST"))
                                strDescription = _lines[_index + 1].Trim();
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                        }
                    }
                    else if (_data.Length > 2 && !_bLongLable)
                    {
                        if (!strText.ToUpper().Contains("BATCH :"))
                        {
                            strAmount = _data[_data.Length - 1];
                            strItem = strText.Replace(strAmount, "").Trim();
                            _data = strText.Split(' ');
                            if (_data[0] == _lineIndex.ToString() || _data[0] == _lineIndex + ".")
                                strItem = strItem.Substring(_data[0].Length);

                            if (strItem != "")
                            {
                                _index++;
                                strText = _lines[_index];
                                _data = strText.Split(' ');
                                if ((_data.Length == 6 && Regex.Matches(strText, "%").Count > 0) || Regex.Matches(strText, "%").Count > 1)
                                {
                                    strRate = _data[3];
                                    strQty = _data[4];
                                }
                                else
                                    strRate = _data[_data.Length - 1];
                                if (_lines[_index + 1].Length == 4)
                                    strHSNCode = _lines[_index + 1];
                                if (strQty == "")
                                {
                                    _index++;
                                    strText = _lines[_index];
                                    _data = strText.Split(' ');
                                    if (_data.Length > 2)
                                    {
                                        strQty = _data[0];
                                        strHSNCode = _data[_data.Length - 1];
                                    }
                                    else if (_data.Length > 0)
                                    {
                                        strQty = _data[0];
                                        _index++;
                                        strText = _lines[_index];
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                        {
                                            strHSNCode = _data[0];
                                            if (strHSNCode.Length < 4)
                                                strHSNCode = _data[_data.Length - 1];
                                            strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                            if (strHSNCode == "")
                                            {
                                                string[] _hsnData = _lines[_index + 1].Trim().Split(' ');
                                                strHSNCode = _hsnData[0];
                                                _index++;
                                            }
                                            else
                                            {
                                                string strDescription = _lines[_index + 1].Trim().ToUpper();
                                                string[] __data = strDescription.Split(' ');
                                                if (__data[0] != (_lineIndex + 1).ToString() && __data[0] != (_lineIndex + 1) + "." && !strDescription.Contains(".00") && !strDescription.Contains("CONTINUED") && !strDescription.Contains(","))
                                                {
                                                    if (strDescription != "" && strItem != "")
                                                        strItem += " " + strDescription;
                                                    _index++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (_data.Length > 1 && (_lines[_index - 1].Trim() == _lineIndex.ToString() || _data[0] == _lineIndex.ToString()))
                    {
                        strAmount = _data[_data.Length - 1];
                        strItem = strText.Replace(strAmount, "").Trim();
                        _data = strText.Split(' ');
                        if (_data[1] != "%")
                        {
                            if (_data[0] == _lineIndex.ToString() || _data[0] == _lineIndex + ".")
                                strItem = strItem.Substring(_data[0].Length);

                            _index++;
                            strText = _lines[_index];
                            _data = strText.Split(' ');

                            if ((_data.Length == 6 && Regex.Matches(strText, "%").Count > 0) || Regex.Matches(strText, "%").Count > 1)
                            {
                                strRate = _data[3];
                                strQty = _data[4];
                            }
                            else
                                strRate = _data[_data.Length - 1];
                            if (_lines[_index + 1].Length == 4)
                                strHSNCode = _lines[_index + 1];
                            if (strHSNCode == "")
                            {
                                _data = _lines[_index + 1].Trim().Split(' ');
                                strHSNCode = _data[_data.Length - 1];
                                _index++;
                            }
                            if (strQty == "")
                            {
                                _index++;
                                strText = _lines[_index];
                                _data = strText.Split(' ');
                                if (_data.Length > 2)
                                {
                                    strQty = _data[0];
                                    strHSNCode = _data[_data.Length - 1];
                                }
                                else if (_data.Length > 0)
                                {
                                    strQty = _data[0];
                                    _index++;
                                    strText = _lines[_index];
                                    _data = strText.Split(' ');
                                    if (_data.Length > 0)
                                        strHSNCode = _data[0];
                                }
                            }
                        }
                        else
                            strAmount = strItem = "";
                    }

                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();

                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["srNo"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["qty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["mrp"].Value = strRate;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["styleName"].Value = strItem;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["variant1"].Value = "";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["variant2"].Value = "";
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        if (strItem != "")
                        {
                            dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                            GetPurchaseRate_Import(dgrdDetails.Rows[_lineIndex - 1]);
                            SetUnitName(strItem, _lineIndex - 1);
                        }
                        else
                        {
                            dgrdDetails.Rows[_lineIndex - 1].DefaultCellStyle.BackColor = Color.Tomato;
                            CalculateDisWithAmountMRP_Current(dgrdDetails.Rows[_lineIndex - 1]);
                        }

                        _lineIndex++;
                    }
                }
            }
            return false;
        }

        private bool SetItemDetailsLineByLine(int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex - 1, _lineIndex = 1, _lineGap = 0, qtyIndex = 0, hsnCodeIndex = 0, rateIndex = 0, itemIndex = 0, _startIndex = 0;
            string strText = "", strItem = "", strHSNCode = "", strQty = "", strRate = "", strSNo = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            if (_index < 0)
                _index = 0;
            if (_index > 0 && _lines[_index - 1].Trim().ToUpper().Contains("NO. & KIND"))
                _lineGap++;

            for (; _index < _lines.Length; _index++)
            {
                string strLine = _lines[_index];
                if (strLine.Contains("Description"))
                {
                    itemIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("Quantity"))
                {
                    qtyIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("Rate"))
                {
                    rateIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("HSN"))
                {
                    hsnCodeIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Replace("\r", "") == "No.")
                {
                    _lineGap++;
                    _index++;
                    break;
                }
                if (_startIndex > 0)
                    _lineGap++;
            }
            // _index = _itemIndex + _lineGap;
            //_lineGap--;
            for (; _index < _lines.Length - 2;)
            {
                strText = _lines[_index];
                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND")) && !strText.ToUpper().Contains(" TOTAL : "))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING"))
                {
                    strText = _lines[_index + 1];
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 0)
                        txtOtherAmt.Text = _data[0];
                    break;
                }
                else if (strText.ToUpper().Contains("ADD : PACKING CHARGE"))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        string strPacking = _data[_data.Length - 1];
                        txtPackingAmt.Text = strPacking;
                        strPacking = System.Text.RegularExpressions.Regex.Replace(strPacking.Replace("/", ""), @"[\d-]", string.Empty); ;
                        if (strPacking.Length > 2)
                        {
                            _data = _lines[_index + 1].Split(' ');
                            if (_data.Length > 0)
                                txtPackingAmt.Text = _data[_data.Length - 1].Trim();
                        }
                    }
                    break;
                }
                else if (!strText.Contains("Less") && !strText.ToUpper().Contains("GST"))
                {
                    strItem = strQty = strRate = "";
                    strItem = strText.Replace("\r", "");
                    strSNo = _lines[_index - 1].Replace("\r", "");

                    if (strItem != "" && strSNo == _lineIndex.ToString())
                    {
                        strItem = _lines[_index + itemIndex].Replace("\r", "");
                        strHSNCode = _lines[_index + hsnCodeIndex].Trim().Replace("\r", "");
                        if (strHSNCode.Length == 8 && strHSNCode.Contains("000"))
                            strHSNCode = strHSNCode.Substring(0, 4);
                        string _strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, @"[\d-]", string.Empty);


                        int __index = 0;
                        if (_strHSNCode != "" || strHSNCode.Length != 4)
                        {
                            __index--;
                            if (_lineIndex == 1)
                                _lineGap--;
                            strHSNCode = _lines[_index + hsnCodeIndex + __index].Replace("\r", "");
                        }

                        strRate = _lines[_index + rateIndex + __index].Replace("\r", "");
                        string _strRate = System.Text.RegularExpressions.Regex.Replace(strRate, "[^0-9]", "");
                        if (_strRate == "")
                        {
                            __index++;
                            strRate = _lines[_index + rateIndex + __index].Replace("\r", "");
                        }
                        strQty = _lines[_index + qtyIndex + __index];
                        // strHSNCode = _lines[_index + hsnCodeIndex+ __index].Replace("\r", "");
                        string[] _strQty = strQty.Split(' ');
                        strQty = _strQty[0];

                        strItem = strItem.Replace("'", "").Trim();

                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        _index += _lineGap - 1;
                    }
                    else
                        _index++;
                }
                else
                    _index++;
            }
            return false;
        }

        private bool SetItemDetailsLineByBusy(int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex - 1, _lineIndex = 1, _rateIndex = 0, _qtyIndex = 0;
            string strText = "", strItem = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;

            for (; _index < _lines.Length - 4;)
            {
                strText = _lines[_index].Trim();
                if (strText.Contains("MRP      RATE"))
                    _rateIndex++;
                if (strText.Contains("Art No."))
                    _qtyIndex++;

                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND")) && !strText.ToUpper().Contains(" TOTAL : ") && !strText.ToUpper().Contains("TOTALS C/O") && !strText.ToUpper().Contains("LIST PRICE DISCOUNT"))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 3)
                    {
                        txtOtherAmt.Text = _data[_data.Length - 1];
                    }
                    else
                    {
                        strText = _lines[_index + 1];
                        _data = strText.Split(' ');
                        if (_data.Length > 0 && _data.Length < 10)
                            txtOtherAmt.Text = _data[0];
                    }
                    _index++;
                }
                else if (strText.ToUpper().Contains("ADD : PACKING CHARGE"))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;

                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtPackingAmt.Text = _data[_data.Length - 1];
                    _index++;
                }
                else if (strText.ToUpper().Contains("ADD  :") && !strText.ToUpper().Contains("CGST") && !strText.ToUpper().Contains("SGST") && !strText.ToUpper().Contains("IGST") && !strText.ToUpper().Contains("R/O"))
                {
                    string[] _data = strText.Replace("  ", " ").Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmt.Text = _data[_data.Length - 1];
                    _index++;
                }
                else if (!strText.Contains("Less") && !strText.ToUpper().Contains("CGST") && !strText.ToUpper().Contains("SGST") && !strText.ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    if (_lines[_index - 1].Trim() == _lineIndex + ".")
                    {
                        strItem = strText;// _lines[_index + 1].Trim();
                        strText = _lines[_index + 1].Replace("|", ":").Trim();
                        strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                        string[] __str = strText.Split('|');
                        if (__str.Length > 2)
                        {
                            strHSNCode = __str[0].Trim();
                            strQty = __str[1].Trim();
                            strRate = __str[2].Trim();

                            string[] __strQty = strQty.Split(' ');
                            strQty = __strQty[0];

                            if (__strQty.Length == 1)
                                strRate = __str[3].Trim();

                            __str = strRate.Split(' ');
                            strRate = __str[0];
                        }

                        _index += 1;

                        if (strItem != "")
                        {
                            strItem = strItem.Replace("'", "").Trim();
                            dgrdDetails.Rows.Add();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                            dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                            if (txtPurchaseParty.Text.Contains("SONKHIYA"))
                            {
                                dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = "BABA SUIT : " + strItem;
                                if (strHSNCode.Contains("6203"))
                                    strItem = "BABA SUIT : 6203";
                                else if (strHSNCode.Contains("6103"))
                                    strItem = "BABA SUIT:6103";
                                else
                                    CheckItemNameExistence(ref strItem, ref strHSNCode);
                            }
                            else
                                CheckItemNameExistence(ref strItem, ref strHSNCode);

                            dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                            _lineIndex++;
                            _index++;
                        }
                        else
                            _index++;
                    }
                    else if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + ".")
                    {
                        strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                        if (strText.Length > 5)
                        {
                            if (strText.Substring(strText.Length - 2, 2) == "|%")
                                strText = strText.Substring(0, strText.Length - 2);
                        }
                        string[] __str = strText.Split('|');
                        if (__str.Length > 2 || txtPurchaseParty.Text.Contains("DOLLCY GARMENTS"))
                        {
                            strItem = strQty = strRate = "";
                            int index = 0;
                            strItem = __str[0].Replace(_lineIndex + ".", "").Replace(". ", " ").Trim();
                            if (txtPurchaseParty.Text.Contains("DOLLCY GARMENTS") && __str.Length < 4)
                            {
                                strHSNCode = __str[1].Trim();
                                _rateIndex = 0;
                                if (__str.Length > 2)
                                {
                                    string[] _strQty = __str[2].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strQty = _strQty[0];
                                }
                                else
                                {
                                    strText = _lines[_index + 1].Trim();
                                    strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    __str = strText.Split('|');
                                    if (__str.Length > 0)
                                        strQty = __str[__str.Length - 1];
                                    _rateIndex = 1;
                                }

                                strText = _lines[_index + _rateIndex + 1].Trim();
                                strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                __str = strText.Split('|');

                                if (__str.Length > 0)
                                    strRate = __str[__str.Length - 1];
                            }
                            else if (__str.Length == 3)
                            {
                                string[] _strHSNCode = strItem.Split(' ');
                                if (_strHSNCode[_strHSNCode.Length - 1].Trim().Length == 4)
                                {
                                    strItem = strItem.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                    strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                }
                                string[] _strQty = __str[1].Trim().Split(' ');
                                if (_strQty.Length > 0)
                                    strQty = _strQty[0];
                                string[] _strRate = __str[2].Trim().Split(' ');
                                if (_strRate.Length > 0)
                                    strRate = _strRate[0];
                            }
                            else
                            {
                                strHSNCode = __str[1].Trim();
                                if (strHSNCode.Length == 8 && (strHSNCode.Contains("000") || strHSNCode.Contains("990")))
                                    strHSNCode = strHSNCode.Substring(0, 4);
                                string __strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if ((__strHSNCode.Length != 4 || strHSNCode.Length != 4) && strHSNCode != "63" && strHSNCode != "62")
                                {
                                    string[] _strHSNCode = strHSNCode.Split(' ');
                                    __strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                    string strNewHSNCode = __str[2].Trim(), ___strHSNCode = System.Text.RegularExpressions.Regex.Replace(__strHSNCode, "[^0-9]", "");
                                    strNewHSNCode = strNewHSNCode.Replace(".00", "");

                                    if (___strHSNCode.Length == 4 && __strHSNCode.Length == 4 && strNewHSNCode.Length != 4)
                                    {
                                        strItem += " " + strHSNCode.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                        strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strItem += " " + strHSNCode;
                                        strHSNCode = __str[2].Trim();
                                        _strHSNCode = strHSNCode.Split(' ');
                                        if (_strHSNCode.Length > 1)
                                        {
                                            strItem += " " + strHSNCode.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                            strHSNCode = _strHSNCode[_strHSNCode.Length - 1];
                                        }
                                        if (strHSNCode.Length != 4)
                                        {
                                            strItem += " " + strHSNCode;
                                            strHSNCode = __str[3].Trim();
                                            index++;
                                        }
                                        index++;
                                    }
                                }
                                string strDescription = "";
                                int _qtyRateIndex = 0;
                                if (_qtyIndex > 0)
                                    strDescription = __str[index + _qtyIndex + 1].Trim();

                                string[] _strQty = __str[index + _qtyIndex + 2].Trim().Split(' ');
                                if (_strQty.Length > 0)
                                    strQty = _strQty[0];
                                if (_strQty.Length == 1)
                                    _qtyRateIndex = 1;
                                string[] _strRate = __str[index + _qtyIndex + _rateIndex + _qtyRateIndex + 3].Trim().Split(' ');
                                if (_strRate.Length > 0)
                                    strRate = _strRate[0];
                                if (strRate == "0.00" && __str.Length > 6)
                                    strRate = __str[5].Trim();

                                if (_lines.Length > _index + 5)
                                {
                                    string _strDescription = _lines[_index + 5].Trim();
                                    string[] _strDesc = _strDescription.Split(' ');
                                    if (_strDesc.Length == 1 && strItem != "" && !_strDesc[0].Contains(".") && !_strDesc[0].Contains(",") && _strDesc[0].Length > 2)
                                        strItem += " " + _strDesc[0];

                                }
                                if (_qtyIndex > 0 && strDescription.Length > 2 && strItem != "")
                                    strItem += " " + strDescription;

                            }
                            if (strItem != "")
                            {
                                strItem = strItem.Replace("'", "").Trim();
                                dgrdDetails.Rows.Add();
                                dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                                dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                                dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                                dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                                if (txtPurchaseParty.Text.Contains("SONKHIYA"))
                                {
                                    dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = "BABA SUIT : " + strItem;
                                    if (strHSNCode.Contains("6203"))
                                        strItem = "BABA SUIT : 6203";
                                    else if (strHSNCode.Contains("6103"))
                                        strItem = "BABA SUIT:6103";
                                    else
                                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                                }
                                else
                                    CheckItemNameExistence(ref strItem, ref strHSNCode);

                                dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                                _lineIndex++;
                                _index++;
                            }
                            else
                                _index++;
                        }
                        else
                            _index++;
                    }
                    else
                        _index++;
                }
                else
                    _index++;
            }
            return false;
        }

        private bool SetItemDetailsByCustomize_Branches(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if ((strText.ToUpper().Contains("ADD:  FREIGHT/CARTAGE")) && txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS"))
                {
                    string[] _data = strText.Trim().Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmt.Text = _data[0];

                    break;
                }
                else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    return true;
                else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS"))
                {
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtOtherAmt.Text = _data[_data.Length - 1];
                    }
                }
                else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmt.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        strText = _lines[_index + 2].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtPackingAmt.Text = _data[0];
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmt.Text = _data[0];
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                            txtPackingAmt.Text = _data[_data.Length - 1];
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    strItem = strQty = strRate = strHSNCode = "";
                    if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + "." || (str[0] == _lineIndex.ToString() && txtPurchaseParty.Text.Contains("KC GARMENTS")))
                    {
                        if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                        {
                            if ((str[0] == _lineIndex.ToString() || str[0] == _lineIndex + ".") && str.Length > 2)
                            {
                                strText = strText.Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                string[] _data = strText.Split(' ');
                                str = _data;
                                if (_data.Length > 0)
                                {
                                    strQty = _data[_data.Length - 1].Trim();
                                    strHSNCode = _data[_data.Length - 3].Trim();
                                }
                                string __strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if (__strHSNCode.Length != 4 || strHSNCode.Length != 4)
                                {
                                    strItem = strText.Replace(_lineIndex + ".", "").Trim();
                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length == 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();

                                        strText = _lines[_index + 3].Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 1)
                                            strRate = _data[0];
                                    }
                                    else
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strText = _lines[_index + 2].Trim();
                                        _data = strText.Split(' ');
                                        strQty = _data[0].Trim();

                                        strText = _lines[_index + 4].Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 1)
                                            strRate = _data[0];
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i < str.Length - 3; i++)
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += str[i];
                                    }
                                    strText = _lines[_index + 2].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 1)
                                        strRate = _data[0];

                                }
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("BONNYS NX"))
                    {
                        strText = strText.Replace("├┼┼┼", "").Trim().Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 5)
                        {
                            string sno = "";
                            strRate = _data[_data.Length - 2].Trim();
                            strQty = _data[_data.Length - 3].Trim();
                            strHSNCode = _data[_data.Length - 5].Trim();
                            if (strHSNCode.Length == 4)
                            {
                                sno = strHSNCode.Substring(0, 1);
                                strHSNCode = "6" + strHSNCode.Substring(1);
                            }
                            else if (strHSNCode.Length == 5)
                            {
                                sno = strHSNCode.Substring(0, 2);
                                strHSNCode = "6" + strHSNCode.Substring(2);
                            }

                            if (sno == _lineIndex.ToString())
                            {
                                for (int i = 0; i < _data.Length - 5; i++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += _data[i];
                                }
                            }
                        }
                    }

                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        strItem = strQty = strRate = "";
                    }
                }
            }
            return false;
        }

        private bool SetItemDetailsByCustomize_Delhi(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            if ((txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") || txtPurchaseParty.Text.Contains("MISHU ENTERPRISES")) && _lines.Length < 60)
                _itemIndex = 17;

            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if ((strText.ToUpper().Contains("ADD:  FREIGHT/CARTAGE")) && txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS"))
                {
                    string[] _data = strText.Trim().Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmt.Text = _data[0];

                    break;
                }
                else if ((strText.ToUpper().Contains("OTHER CHARGES :")) && txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION"))
                {
                    strText = _lines[_index - 1].Trim();
                    double _dFreight = dba.ConvertObjectToDouble(strText);
                    if (_dFreight > 0)
                        txtOtherAmt.Text = _dFreight.ToString("0.00");

                    break;
                }
                else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("JANAK GARMENTEX") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") && !txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") && !txtPurchaseParty.Text.Contains("MOTI FASHIONS") && !txtPurchaseParty.Text.Contains("TANEJA FASHION") && !txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && !txtPurchaseParty.Text.Contains("VIPIN COLLECTION") && !txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && !txtPurchaseParty.Text.Contains("HARDIK TEXTILE") && !txtPurchaseParty.Text.Contains("SONY CREATION") && !txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && !txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && !txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION") && !txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && !txtPurchaseParty.Text.Contains("ARPIT FASHION"))
                    return true;
                else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                {
                    if (txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                    {
                        string[] _data = strText.Trim().Split(' ');
                        if (_data.Length > 2)
                            txtOtherAmt.Text = _data[0];
                        if (txtOtherAmt.Text == "(+)")
                            txtOtherAmt.Text = "";
                    }
                    else if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                    {
                        string[] _data = _lines[_index + 1].Trim().Split(' ');
                        if (_data.Length > 0)
                        {
                            txtOtherAmt.Text = _data[0].Trim();
                            if (txtOtherAmt.Text == "")
                            {
                                _data = _lines[_index].Trim().Split(' ');
                                if (_data.Length > 0)
                                    txtOtherAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1].Trim()).ToString("0.00");
                            }
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtOtherAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                    }
                }
                else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmt.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("PC. DISCOUNT") && txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                {
                    //string[] _data = strText.Split(' ');
                    //if (_data.Length > 1)
                    // txtPcsAmt.Text = _data[0];
                }
                else if (strText.ToUpper().Contains("DISCOUNT PER. PC.") && txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                {
                    //string _strText = strText.Replace("Amount", "").Replace("Chargeable", "").Replace("(", "").Replace(")", "").Replace("In Words", "").Replace(":", "").Trim();
                    //string[] _data = _strText.Split(' ');
                    //if (_data.Length > 1)
                    //  txtPcsAmt.Text = _data[0];
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    if (txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                    {
                        string[] _data = strText.Trim().Split(' ');
                        if (_data.Length > 2)
                            txtPackingAmt.Text = _data[0];
                        if (txtPackingAmt.Text == "(+)")
                            txtPackingAmt.Text = "";
                    }
                    else if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        strText = _lines[_index + 2].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtPackingAmt.Text = _data[0];
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmt.Text = _data[0];
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                            txtPackingAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE TIME :"))
                {
                    if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        //if (txtOtherAmount.Text != "0.00")
                        //{
                        strText = _lines[_index + 1].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtOtherAmt.Text = (dba.ConvertObjectToDouble(_data[0]) + dba.ConvertObjectToDouble(txtOtherAmt.Text)).ToString("0.00");
                        }
                        //  }
                        return true;
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    strItem = strQty = strRate = "";
                    if ((txtPurchaseParty.Text.Contains("JANAK GARMENTEX")))
                    {
                        if (str.Length > 10)
                        {
                            if (str[10] == _lineIndex.ToString())
                            {
                                strQty = str[9].Trim();
                                strRate = str[1].Trim();
                                strHSNCode = str[0].Trim();
                                for (int __index = 11; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            else
                            {
                                string[] _data = _lines[_index + 1].Trim().Split(' ');
                                if (_data.Length > 10)
                                {
                                    if (_data[10] == _lineIndex.ToString())
                                    {
                                        strQty = str[9].Trim();
                                        strRate = str[1].Trim();
                                        strHSNCode = str[0].Trim();
                                        for (int __index = 11; __index < str.Length; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                        }
                        else if (str.Length > 2)
                        {
                            if (str[0] == _lineIndex.ToString())
                            {
                                for (int __index = 1; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }

                                str = _lines[_index - 1].Trim().Split(' ');
                                if (str.Length > 2)
                                {
                                    strQty = str[9].Trim();
                                    strRate = str[1].Trim();
                                    strHSNCode = str[0].Trim();
                                }

                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            strRate = str[13].Trim();
                            strQty = str[14].Trim();
                            strHSNCode = str[15].Trim();
                            for (int __index = 16; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("VIPIN COLLECTION")) && str.Length > 10)
                    {
                        if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strRate = str[8].Trim();
                            strQty = str[9].Trim();
                            strHSNCode = str[10].Trim();
                            for (int __index = 11; __index < str.Length - 1; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("SHUBHI GARMENTS")) && str.Length > 10)
                    {
                        string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');
                        if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strRate = str[7].Trim();
                            strQty = str[9].Trim();
                            strHSNCode = str[10].Trim();
                            for (int __index = 11; __index < str.Length - 1; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("NIKUNJ TRADING")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 6; __index < str.Length - 4; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 6; __index < str.Length - 4; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("SONY CREATION") || txtPurchaseParty.Text.Contains("HARDIK TEXTILE")) && str.Length > 7)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 9; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 17; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 11; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem == "")
                                strItem = str[str.Length - 1].Trim();
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("WORLD SAHAB")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 13; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem == "")
                                strItem = str[str.Length - 1].Trim();
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("DONARGOLD GARMENTS") || txtPurchaseParty.Text.Contains("W STAN GARMENTS") || txtPurchaseParty.Text.Contains("GEX GARMENTS"))
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string strDescription = "";

                            strQty = str[str.Length - 1].Trim();
                            if (str.Length > 2)
                                strDescription = str[1];
                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0].Trim();
                            str = _lines[_index + 2].Trim().Split(' ');
                            strHSNCode = str[0].Trim();
                            for (int __index = 1; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                            _index += 2;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MOTI FASHIONS") && str.Length > 6)
                    {
                        if (_lines[_index - 1].Trim() == _lineIndex.ToString())
                        {
                            strHSNCode = str[str.Length - 2].Trim();
                            strQty = str[str.Length - 4].Trim();
                            strRate = str[str.Length - 5].Trim();

                            for (int __index = 0; __index < str.Length - 6; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            _index += 1;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && str.Length > 6)
                    {
                        if (_lines[_index + 1].Trim() == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 1].Trim();
                            strRate = str[str.Length - 9].Trim();
                            strHSNCode = str[str.Length - 10].Trim();

                            for (int __index = 0; __index < str.Length - 10; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                        else if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 2].Trim();
                            strRate = str[str.Length - 10].Trim();
                            strHSNCode = str[str.Length - 11].Trim();

                            for (int __index = 0; __index < str.Length - 11; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && str.Length > 6)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            int itemIndex = 2;
                            strRate = str[str.Length - 2].Trim();
                            strHSNCode = str[2].Trim();
                            string _strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                            if (_strHSNCode.Length == 4 && strHSNCode.Length == 4)
                            {
                                strQty = str[3].Trim();
                                strRate = str[4].Trim();
                            }
                            else
                            {
                                itemIndex = 3;
                                strHSNCode = str[3].Trim();
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                if (_strHSNCode.Length == 4 && strHSNCode.Length == 4)
                                {
                                    strQty = str[4].Trim();
                                    strRate = str[5].Trim();
                                }
                                else
                                {
                                    itemIndex = 4;
                                    strHSNCode = str[4].Trim();
                                    strQty = str[5].Trim();
                                    strRate = str[6].Trim();
                                }
                            }

                            for (int __index = 1; __index < itemIndex; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("TANEJA FASHION") && str.Length > 6)
                    {
                        strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        int ___index = strText.IndexOf(" " + _lineIndex + " ");
                        if (___index > 0)
                        {
                            str = strText.Trim().Split(' ');

                            string strDescription = "";

                            strQty = str[5].Trim();
                            strHSNCode = str[6].Trim();
                            strRate = str[str.Length - 1].Trim();

                            for (int __index = 7; __index < str.Length - 5; __index++)
                            {
                                if (str[__index] == _lineIndex.ToString())
                                    break;
                                else
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("JOLLY FASHIONS") && str.Length > 1)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strHSNCode = _lines[_index + 1].Trim();
                            if (strHSNCode.Length == 4 || strHSNCode == "62" || strHSNCode == "63")
                            {
                                string[] _strText = _lines[_index + 2].Trim().Split(' ');
                                if (_strText.Length > 1)
                                {
                                    strQty = _strText[0];
                                    strRate = _strText[1];
                                }
                                for (int __index = 1; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                                if (strItem != "")
                                    _index += 4;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("WORLD CHOICE") && str.Length > 1)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            for (int __index = 1; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            int _qtyIndex = 0;

                            string __strText = _lines[_index + 1].Trim();
                            str = __strText.Split(' ');
                            if (str.Length == 1)
                            {
                                strQty = str[0];
                                _qtyIndex++;

                                __strText = _lines[_index + _qtyIndex + 1].Trim();
                                str = __strText.Split(' ');
                                strRate = str[0];
                            }
                            else if (str.Length == 2)
                            {
                                strQty = str[0];
                                strRate = str[1];
                            }

                            __strText = _lines[_index + _qtyIndex + 4].Trim();
                            str = __strText.Split(' ');
                            if (str.Length > 2)
                                strHSNCode = str[str.Length - 2];
                            if (strItem != "")
                                _index += 5;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("CHANCELLOR INDUSTRIES") || txtPurchaseParty.Text.Contains("MAA PADMAVATI APPARELS"))
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 8];
                            strQty = str[str.Length - 10];
                            strHSNCode = str[str.Length - 11];

                            for (int __index = 1; __index < str.Length - 11; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 12];
                            strQty = str[str.Length - 13];
                            strHSNCode = str[str.Length - 14];

                            for (int __index = 1; __index < str.Length - 14; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && str.Length > 10)
                    {
                        if (str[str.Length - 8] == _lineIndex.ToString())
                        {
                            string strDesc = "";
                            if (strText.Contains("STD"))
                            {
                                strText = strText.Replace(" STD ", " ").Replace("  ", " ").Trim();
                                strDesc = " STD";
                            }
                            str = strText.Split(' ');

                            strRate = str[str.Length - 9];
                            strQty = str[str.Length - 10];
                            strHSNCode = str[str.Length - 2];

                            for (int __index = 1; __index < str.Length - 10; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }

                            if (strItem != "" && strDesc != "")
                                strItem += strDesc;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("AASHI COLLECTION") && str.Length > 4)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 1];
                            strHSNCode = str[str.Length - 2];
                            for (int __index = 1; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0];
                            _index += 2;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("S.R CREATION") && str.Length > 4)
                    {
                        if (str[str.Length - 5] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 2].Trim();
                            strHSNCode = str[str.Length - 3].Trim();
                            strItem = str[str.Length - 4].Trim();

                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0].Trim();
                        }
                        else if (str.Length > 5)
                        {
                            if (str[str.Length - 6] == _lineIndex.ToString())
                            {
                                strQty = str[str.Length - 3].Trim();
                                strHSNCode = str[str.Length - 4].Trim();
                                strItem = (str[str.Length - 6] + " " + str[str.Length - 5]).Trim();

                                str = _lines[_index + 1].Trim().Split(' ');
                                strRate = str[0].Trim();
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") && str.Length > 6)
                    {
                        if (str[5] == _lineIndex.ToString())
                        {
                            strQty = str[0];
                            strRate = str[10];
                            strHSNCode = str[11];

                            for (int __index = 12; __index <= str.Length - 1; __index++)
                            {
                                if (__index != 14)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            //if (str[12] != "")
                            //    strItem += " " + str[12];
                            if (str[8] != "")
                                strItem += " " + str[8];
                            if (str[9] != "")
                                strItem += " " + str[9];
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && str.Length > 6)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 2];
                            strQty = str[str.Length - 4];
                            strHSNCode = str[str.Length - 5];

                            for (int __index = 1; __index < str.Length - 5; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MIKEY FASHION") && str.Length > 4)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 1];
                            strQty = str[str.Length - 2];
                            strHSNCode = str[str.Length - 3];

                            for (int __index = 1; __index < str.Length - 3; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + "." || (str[0] == _lineIndex.ToString() && txtPurchaseParty.Text.Contains("KC GARMENTS")))
                    {

                        if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                        {
                            if (str[0] == _lineIndex.ToString() && str.Length > 2)
                            {
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 0)
                                    strQty = _data[_data.Length - 1].Trim();

                                strItem = strText.Replace(_lineIndex + " ", "").Replace(strQty, "");
                                _data = _lines[_index + 1].Trim().Split(' ');
                                if (_data.Length > 2)
                                    strRate = _data[_data.Length - 1].Trim();
                                else
                                {
                                    _data = _lines[_index + 2].Trim().Split(' ');
                                    strRate = _data[0].Trim();
                                }
                                if ((_index + 3) < _lines.Length)
                                {
                                    _data = _lines[_index - 1].Split(' ');
                                    strHSNCode = _data[_data.Length - 1].Trim();
                                }
                            }
                            else
                            {
                                strText = _lines[_index - 1].Trim();
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 0)
                                    strHSNCode = _data[_data.Length - 1].Trim();
                                strItem = _lines[_index + 1].Trim();
                                _data = _lines[_index + 2].Split(' ');
                                if (_data.Length > 3)
                                {
                                    strQty = _data[0].Trim();
                                    strRate = _data[3].Trim();
                                }
                                else
                                {
                                    strQty = _data[0];
                                    _data = _lines[_index + 3].Split(' ');
                                    if (_data.Length > 0)
                                        strRate = _data[0];
                                }
                            }
                            _index += 3;
                        }
                        else if (txtPurchaseParty.Text.Contains("LUCKY JACKET") || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                        {
                            if (str.Length < 2)
                            {
                                strItem = _lines[_index + 1].Trim();
                                strText = _lines[_index + 2].Trim();
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 3)
                                {
                                    strHSNCode = _data[0].Trim();
                                    strQty = _data[_data.Length - 1].Trim();
                                }
                                strText = _lines[_index + 5].Trim();
                                _data = strText.Split(' ');
                                if (_data.Length > 3)
                                    strRate = _data[_data.Length - 1].Trim();
                                else
                                {
                                    strText = _lines[_index + 4].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 2)
                                        strRate = _data[_data.Length - 1].Trim();
                                }

                                _index += 4;
                            }
                            else
                            {
                                strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                string[] __str = strText.Split('|');

                                if (__str.Length > 2)
                                {
                                    strItem = strQty = strRate = "";
                                    int index = 0;
                                    double dRate = 0;
                                    strItem = __str[0].Replace(_lineIndex + ".", "").Trim();
                                    strHSNCode = __str[1].Trim();
                                    if (strHSNCode.Length != 4)
                                    {
                                        strItem += " " + strHSNCode;
                                        strHSNCode = __str[2].Trim();
                                        index++;
                                    }
                                    string[] _strQty = __str[index + 2].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strQty = _strQty[0];
                                    if (txtPurchaseParty.Text.Contains("LUCKY JACKET"))
                                    {
                                        if (__str.Length > 3)
                                        {
                                            _strQty = __str[3].Trim().Split(' ');
                                            strRate = _strQty[0].Trim();
                                            dRate = dba.ConvertObjectToDouble(strRate);
                                        }
                                        strText = _lines[_index + 1].Trim();
                                    }
                                    else
                                        strText = _lines[_index + 2].Trim();

                                    strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    string[] _data = strText.Split('|');
                                    if (_data.Length == 1 && dRate == 0)
                                    {
                                        string[] __strRate = _data[0].Trim().Split(' ');
                                        if (__strRate.Length > 0)
                                            strRate = __strRate[0].Trim();
                                    }
                                    else if (_data.Length > 1 && dRate == 0)
                                    {
                                        string[] __strRate = _data[1].Trim().Split(' ');
                                        if (__strRate.Length > 0)
                                            strRate = __strRate[0].Trim();
                                        if (strRate == "%" && _data.Length > 2)
                                        {
                                            __strRate = _data[2].Trim().Split(' ');
                                            if (__strRate.Length > 0)
                                                strRate = __strRate[0].Trim();
                                        }

                                    }
                                    if (strRate == "0.00")
                                    {
                                        strText = _lines[_index + 3].Trim();
                                        if (strText == "%")
                                            strText = _lines[_index + 4].Trim();
                                        _data = strText.Trim().Split(' ');
                                        strRate = _data[0];
                                        if (strRate == "%")
                                            strRate = _data[_data.Length - 1].Trim();

                                        _index += 4;
                                    }
                                }
                                else if (txtPurchaseParty.Text.Contains("LUCKY JACKET"))
                                {
                                    strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    string[] _data = strText.Split('|');

                                    strItem = _data[0].Replace(_lineIndex + ".", "").Trim();
                                    strHSNCode = _data[1].Trim();
                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                        strQty = _data[_data.Length - 1].Trim();
                                    strText = _lines[_index + 2].Trim();
                                    if (Regex.Matches(strText, "%").Count > 1)
                                    {
                                        _data = strText.Replace("  ", " ").Trim().Split(' ');
                                        if (_data.Length > 4)
                                            strRate = _data[_data.Length - 3].Trim();
                                        else
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strText = _lines[_index + 2].Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    _index += 3;
                                }
                                else if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                                {
                                    string[] _data = null;
                                    if (__str.Length == 2)
                                    {
                                        string _strData = __str[0];
                                        _data = _strData.Split(' ');
                                        if (_data.Length > 1)
                                            strHSNCode = _data[_data.Length - 1];
                                        strItem = __str[0].Replace(_lineIndex + ".", "").Replace(strHSNCode, "").Trim();
                                        strQty = __str[__str.Length - 1].Trim();
                                    }
                                    else
                                        strItem = strText.Replace(_lineIndex + ".", "").Trim();

                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();
                                    }
                                    if (__str.Length == 2)
                                        strText = _lines[_index + 2].Trim();
                                    else
                                        strText = _lines[_index + 3].Trim();

                                    if (Regex.Matches(strText, "%").Count > 1)
                                    {
                                        _data = strText.Replace("  ", " ").Trim().Split(' ');
                                        if (_data.Length > 4)
                                            strRate = _data[_data.Length - 3].Trim();
                                        else
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strText = _lines[_index + 4].Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                            strRate = _data[0].Trim();
                                        if ((strRate == "0.00" || strRate == "0" || strRate == "%") && _data.Length > 2)
                                            strRate = _data[_data.Length - 1].Trim();
                                        if (dba.ConvertObjectToDouble(strRate) < 7)
                                        {
                                            strText = _lines[_index + 3].Trim();
                                            _data = strText.Replace("  ", " ").Trim().Split(' ');
                                            strRate = _data[_data.Length - 1].Trim();
                                        }
                                    }
                                    _index += 4;
                                }
                            }
                        }
                    }


                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        strItem = strQty = strRate = "";
                    }
                }
            }
            return false;
        }

        private int GetIndexOfValue(string[] str, string strValue, bool _reverse)
        {
            if (_reverse)
            {
                for (int index = str.Length - 1; index >= 0; index--)
                {
                    if (str[index].Contains(strValue))
                        return index;
                }
            }
            else
            {
                for (int index = 0; index <= str.Length - 1; index++)
                {
                    if (str[index].Contains(strValue))
                        return index;
                }
            }
            return 0;
        }

        private bool SetItemDetailsByCustomize_Saraogi(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');

            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "", strSize = "", strColor = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "", strMRP = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();

                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("JANAK GARMENTEX") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") && !txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") && !txtPurchaseParty.Text.Contains("MOTI FASHIONS") && !txtPurchaseParty.Text.Contains("TANEJA FASHION") && !txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && !txtPurchaseParty.Text.Contains("VIPIN COLLECTION") && !txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && !txtPurchaseParty.Text.Contains("HARDIK TEXTILE") && !txtPurchaseParty.Text.Contains("SONY CREATION") && !txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && !txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && !txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION") && !txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && !txtPurchaseParty.Text.Contains("ARPIT FASHION"))
                    return true;
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                }
                else if (strText.ToUpper().Contains("POSTAGE AMT") || strText.ToUpper().Contains("OTHER CHARGES") || strText.ToUpper().Contains("INSURANCE CHARGE") || strText.ToUpper().Contains("INSURANCE CHARGE") || strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("OTHER AMT") || strText.ToUpper().Contains("TRANSPORT CHARGE") || strText.ToUpper().Contains("DISPATCHED CHARGE"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        double dOther = dba.ConvertObjectToDouble(txtOtherAmt.Text) + dba.ConvertObjectToDouble(_data[_data.Length - 1]);
                        txtOtherAmt.Text = dOther.ToString("0.00");
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    strItem = strSize = strColor = strQty = strRate = "";
                    string[] str = strText.Split(' ');

                    if (str[str.Length - 1] == _lineIndex + "." && _strSType == "")
                    {
                        //Retails

                        string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');

                        strHSNCode = str[0];
                        if (strHSNCode.Length == 8)
                            strHSNCode = strHSNCode.Substring(0, 4);

                        int rIndex = 3;
                        if (Regex.Matches(strText, "%").Count > 2)
                            rIndex = 4;
                        strRate = str[rIndex].Trim();

                        for (int __index = rIndex + 1; __index <= str.Length - 4; __index++)
                        {
                            if (strItem != "")
                                strItem += " ";
                            strItem += str[__index];
                        }
                        strSize = str[str.Length - 3];
                        strColor = str[str.Length - 2];

                        strText = _lines[_index - 1].Trim();
                        _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');
                        if (str.Length > 3)
                        {
                            strMRP = str[2];
                            strQty = str[0];
                        }
                    }
                    else if (Regex.Matches(strText, "%").Count > 1)
                    {
                        if ((txtPurchaseParty.Text.Contains("SARAOGI") || txtGSTNo.Text.Contains("AAYCS8982Q")) && str.Length > 4)
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');

                            strHSNCode = str[0].Trim();

                            int perIndex = GetIndexOfValue(str, "%", true);
                            strRate = str[perIndex + 1].Trim();
                            if (strText.Contains("P.No:"))
                            {
                                int pIndex = GetIndexOfValue(str, "P.No", false);
                                for (int __index = perIndex + 2; __index < pIndex; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            else if (_lines[_index + 1].Trim().Contains("P.No:"))
                            {
                                int pIndex = str.Length;
                                for (int __index = perIndex + 2; __index < pIndex; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            else if (_lines[_index + 2].Trim().Contains("P.No:"))
                            {
                                int pIndex = str.Length;
                                for (int __index = perIndex + 2; __index < pIndex; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }

                            strText = _lines[_index - 1].Trim();
                            _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            if (str.Length > 5)
                            {
                                strQty = str[str.Length - 4].Trim();
                                strMRP = str[str.Length - 2].Trim();
                            }
                            else if (str.Length == 4)
                            {
                                strQty = str[0].Trim();
                                strMRP = str[2].Trim();
                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                if (Regex.Matches(_str, "%").Count < 1)
                                {
                                    strText = _lines[_index - 3].Trim();
                                    string _strPrv = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                    _strPrv += " " + _str;
                                    str = _strPrv.Trim().Split(' ');
                                }
                                else
                                    str = _str.Trim().Split(' ');
                                if (strItem == "")
                                {
                                    for (int __index = 2; __index < str.Length - 1; __index++)
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += str[__index];
                                    }
                                }
                            }
                            else
                            {
                                strMRP = str[0].Trim();
                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                string __strQty = System.Text.RegularExpressions.Regex.Replace(_str, "[^0-9]", "");
                                if (__strQty.Length > 0)
                                    strQty = __strQty;
                                else
                                {
                                    strText = _lines[_index - 3].Trim();
                                    _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                    __strQty = System.Text.RegularExpressions.Regex.Replace(_str, "[^0-9]", "");
                                    if (__strQty.Length > 0)
                                        strQty = _str.Trim();
                                    else
                                    {
                                        strText = _lines[_index - 4].Trim();
                                        _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                        __strQty = System.Text.RegularExpressions.Regex.Replace(_str, "[^0-9]", "");
                                        if (__strQty.Length > 0)
                                            strQty = _str.Trim();
                                    }
                                }
                            }
                            if (strItem == "")
                            {
                                for (int __index = 2; __index < str.Length - 4; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                        }
                    }
                    else if (str[str.Length - 1] == _lineIndex + ".")
                    {
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strRate = str[str.Length - 2].Trim();

                            strText = _lines[_index - 1].Trim();
                            if (!strText.Contains("%"))
                            {
                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                strHSNCode = str[0];

                                strText = _lines[_index - 3].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                if (str.Length > 4)
                                {
                                    strQty = str[str.Length - 4].Trim();
                                    strMRP = str[str.Length - 2].Trim();
                                    if (strItem == "")
                                    {
                                        for (int __index = 2; __index < str.Length - 4; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                strHSNCode = str[0];

                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                if (str.Length > 4)
                                {
                                    strQty = str[str.Length - 4].Trim();
                                    strMRP = str[str.Length - 2].Trim();
                                    if (strItem == "")
                                    {
                                        for (int __index = 2; __index < str.Length - 4; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (strItem != "")
                {
                    strItem = strItem.Replace("'", "").Trim();
                    dgrdDetails.Rows.Add();
                    dgrdDetails.Rows[_lineIndex - 1].Cells["srNo"].Value = _lineIndex + ".";
                    dgrdDetails.Rows[_lineIndex - 1].Cells["qty"].Value = strQty;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["wsMRP"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["mrp"].Value = strMRP;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["styleName"].Value = strItem;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["variant1"].Value = strSize;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["variant2"].Value = strColor;
                    CheckItemNameExistence(ref strItem, ref strHSNCode);
                    if (strItem != "")
                    {
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        GetPurchaseRate_Import(dgrdDetails.Rows[_lineIndex - 1]);
                        SetUnitName(strItem, _lineIndex - 1);
                    }
                    else
                    {
                        dgrdDetails.Rows[_lineIndex - 1].DefaultCellStyle.BackColor = Color.Tomato;
                        CalculateDisWithAmountMRP_Current(dgrdDetails.Rows[_lineIndex - 1]);
                    }
                    _lineIndex++;
                    strItem = strSize = strColor = strQty = strRate = "";
                }
            }
            return false;
        }

        private void SetBasicDetails(ref int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = 0;
            if ((_lines[0].Contains("GSTIN  :") || _lines[0].ToUpper().Contains("ORIGINAL COPY")) && (_lines[0].Contains("               ") || _lines[1].Contains("               ")) && (_lines[0].ToUpper().Contains("TAX INVOICE") || _lines[1].ToUpper().Contains("TAX INVOICE") || _lines[2].ToUpper().Contains("TAX INVOICE")))
                _strBillType = "BUSY";
            if ((_lines[0].Contains("GSTIN") || _lines[1].ToUpper().Contains("ORIGINAL COPY")) && (_lines[1].Contains("               ") || _lines[2].Contains("               ")) && _lines[2].ToUpper().Contains("TAX INVOICE"))
                _strBillType = "BUSY";
            if (_lines[3].Contains("N.A.R. TRADING"))
                _strBillType = "BUSY";

            bool _bBonny = false;
            foreach (string strText in _lines)
            {
                if ((strText.Contains("GSTIN  :") || strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN. :") || strText.Contains("GSTIN: ") || strText.Contains("GSTIN :")) && !strText.Contains("AAYCS8982Q"))
                {
                    string _strText = strText.Replace("  ", "");
                    string[] strGST = _strText.Trim().Split(' ');
                    if (strGST.Length > 1)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        if (strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN  :"))
                            strGSTNO = strGST[1];
                        else if ((strText.Contains("GSTIN. :") || strText.Contains("GSTIN :")) && strGSTNO.Length != 15 && strGST.Length > 2)
                            strGSTNO = strGST[2];
                        if (strGSTNO.Length == 18)
                            strGSTNO = strGSTNO.Substring(0, 15);

                        if (strGSTNO.Length == 15)
                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                        else
                        {
                            strGST = strText.Replace("  ", " ").Replace("  ", " ").Trim().Split(' ');
                            if (strGST.Length > 0)
                            {
                                strGSTNO = strGST[1];
                                if (strGSTNO == ":" && strGST.Length > 2)
                                    strGSTNO = strGST[2];

                                if (strGSTNO.Length == 15)
                                    SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                            }
                        }
                        if (_index > 1)
                        {
                            if (_lines[_index - 1].ToUpper().Contains("TAX INVOICE") && _lines[0].ToUpper().Contains("INVOICE NO."))
                            {
                                txtInvoiceNo.Text = _lines[_index - 3].ToUpper().Trim();
                                txtInvoiceDate.Text = _lines[_index - 2].ToUpper().Trim();
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN :"))
                {
                    if (_index > 1)
                    {
                        if ((_lines[_index - 1].ToUpper().Contains("TAX INVOICE") || _lines[_index - 1].ToUpper().Contains("COPY")) && _lines[0].ToUpper().Contains("INVOICE NO."))
                        {
                            string _strText = strText.Replace("  ", "");
                            string[] strGST = _strText.Trim().Split(' ');
                            if (strGST.Length > 1)
                            {
                                string strGSTNO = strGST[strGST.Length - 1];
                                if (strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN  :"))
                                    strGSTNO = strGST[1];
                                else if ((strText.Contains("GSTIN. :") || strText.Contains("GSTIN :")) && strGSTNO.Length != 15 && strGST.Length > 2)
                                    strGSTNO = strGST[2];
                                if (strGSTNO.Length == 18)
                                    strGSTNO = strGSTNO.Substring(0, 15);

                                if (strGSTNO.Length == 15)
                                    SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                                else
                                {
                                    strGST = strText.Replace("  ", " ").Replace("  ", " ").Trim().Split(' ');
                                    if (strGST.Length > 0)
                                    {
                                        strGSTNO = strGST[1];
                                        if (strGSTNO == ":" && strGST.Length > 2)
                                            strGSTNO = strGST[2];

                                        if (strGSTNO.Length == 15)
                                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                                    }
                                }
                                txtInvoiceNo.Text = _lines[_index - 3].ToUpper().Trim();
                                string strDate = _lines[_index - 2].ToUpper().Trim();
                                strGST = strDate.Split(' ');
                                txtInvoiceDate.Text = strGST[0];
                                if (strGST.Length > 2)
                                {
                                    txtLRDate.Text = strGST[strGST.Length - 1];
                                    txtLRNo.Text = strDate.Replace(txtInvoiceDate.Text, "").Replace(txtLRDate.Text, "").Trim();
                                }
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GST NO. :"))
                {
                    if (_index > 1)
                    {
                        string _strText = _lines[_index - 1];
                        string[] strGST = _strText.Trim().Split(' ');
                        if (strGST.Length > 0)
                        {
                            string strGSTNO = strGST[0];
                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                        }
                        if (txtGSTNo.Text == "")
                        {
                            _strText = _lines[_index];
                            strGST = _strText.Trim().Split(' ');
                            SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN.") && txtPurchaseParty.Text == "" && !strText.Contains("AAYCS8982Q"))
                {
                    string[] strGST = strText.Trim().Split('.');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN") && _strBillType == "BUSY" && txtPurchaseParty.Text == "")
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[0];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN") && !strText.Contains("AAYCS8982Q") && txtPurchaseParty.Text == "")
                {
                    string[] strGST = strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        string _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GST NO.") && !strText.Contains("AAYCS8982Q") && txtPurchaseParty.Text == "")
                {
                    string[] strGST = strText.Trim().Split('-');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        string _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("VEHICLE NO.") && txtPurchaseParty.Text == "")
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[0];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                }
                else if (strText.ToUpper().Contains("CUSTOMER NO.") && txtInvoiceDate.Text.Length != 10)
                {
                    string _strText = _lines[_index + 3];
                    string[] strDate = _strText.Trim().Split(' ');
                    if (strDate.Length > 0)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate[0]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO/ DATE"))
                {
                    if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        txtInvoiceNo.Text = _lines[_index + 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, _lines[_index + 2]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.    :"))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtInvoiceNo.Text = strInvoiceNo[4].Trim();
                            string strInv = Regex.Replace(txtInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtInvoiceNo.Text += strInvoiceNo[5].Trim();
                        }
                        if (txtInvoiceNo.Text == "")
                        {
                            _strText = strText.Replace("                     ", " ");
                            strInvoiceNo = _strText.Trim().Split(' ');
                            if (strInvoiceNo.Length > 1)
                                txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.     :"))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtInvoiceNo.Text = strInvoiceNo[0].Trim();
                            string strInv = Regex.Replace(txtInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtInvoiceNo.Text += strInvoiceNo[5].Trim();
                            if (_strText.ToUpper().Contains("DATED"))
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, strInvoiceNo[strInvoiceNo.Length - 2]);
                                txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO. :"))
                {
                    string _strText = strText.Replace("  ", " ");
                    string[] strInvoiceNo = _strText.Trim().Split(' ');

                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                    if (txtInvoiceNo.Text.Trim() == ":")
                        txtInvoiceNo.Text = "";
                    if (txtInvoiceNo.Text == "" && (txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION")))
                    {
                        strInvoiceNo = _lines[_index + 1].Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                            txtInvoiceNo.Text = strInvoiceNo[0].Trim();
                    }
                    else if (txtInvoiceNo.Text.Contains("CREDIT") && strInvoiceNo.Length > 3)
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 2].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE NO."))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        if (_lines[_index + 1].ToUpper().Contains(": CUSTOMER NAME"))
                        {
                            txtInvoiceNo.Text = _lines[_index - 2];
                            if (!txtInvoiceNo.Text.Contains("KC"))
                                txtInvoiceNo.Text = _lines[_index - 3];
                        }
                        else
                        {
                            txtInvoiceNo.Text = _lines[_index + 1].Replace("Credit", "").Trim();
                            if (txtInvoiceNo.Text == ":")
                                txtInvoiceNo.Text = "";
                            if ((txtInvoiceNo.Text.Contains("2020") || txtInvoiceNo.Text.Contains("2021")) && txtInvoiceNo.Text.Trim().Length < 11)
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, txtInvoiceNo.Text.Trim());
                                txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                                txtInvoiceNo.Text = "";
                            }
                            else
                            {
                                string _strText = strText.Replace("  ", " ");
                                string[] strGST = _strText.Split(' ');
                                if ((txtPurchaseParty.Text.Contains("LADLEE WESTERN") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) && strGST.Length > 3)
                                    txtInvoiceNo.Text = strGST[3];
                                else if (txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS"))
                                {
                                    strGST = _lines[_index + 1].Trim().Split(' ');
                                    txtInvoiceNo.Text = strGST[1];
                                }
                                else if (txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("WORLD SAHAB")) // || txtPurchaseParty.Text.Contains("N.D. FASHION") //|| txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES")
                                {
                                    txtInvoiceNo.Text = strGST[0];
                                }
                                if (_strText.ToUpper().Contains("DATED"))
                                {
                                    txtInvoiceNo.Text = "";
                                    if (strGST.Length > 1 && txtInvoiceNo.Text == "")
                                        txtInvoiceNo.Text = strGST[2];

                                    DateTime _iDate = DateTime.Now;
                                    ConvertDateTime(ref _iDate, strGST[strGST.Length - 1].Trim());
                                    txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    if (strGST.Length > 1 && txtInvoiceNo.Text == "")
                                        txtInvoiceNo.Text = strGST[strGST.Length - 1];
                                    if (txtInvoiceNo.Text.Contains("BILL"))
                                        txtInvoiceNo.Text = _lines[_index + 2];
                                    if (txtInvoiceNo.Text.Contains("LR DATE"))
                                        txtInvoiceNo.Text = "";
                                }
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO  :") || strText.ToUpper().Contains("INVOICE NO :"))
                {
                    string[] strGST = strText.Split(' ');
                    if (strGST.Length > 1 && txtInvoiceDate.Text.Length != 10)
                    {
                        txtInvoiceNo.Text = strGST[strGST.Length - 1];
                        DateTime _iDate = DateTime.Now;
                        strGST = _lines[_index + 1].Split(' ');
                        string strDate = strGST[0];
                        if (strDate.Length < 9)
                            strDate = strGST[strGST.Length - 1];
                        ConvertDateTime(ref _iDate, strDate);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        if (txtInvoiceNo.Text == ":")
                            txtInvoiceNo.Text = "";
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO:             DT."))
                {
                    string _strText = strText.Replace("  ", " ").Replace("  ", " ").Trim();
                    string[] strInvoiceDate = _strText.Trim().Split(' ');
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                    _strText = _lines[_index + 1].Trim();
                    string[] strInvoiceNo = _strText.Trim().Split(' ');
                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    _bBonny = true;
                }
                else if (strText.ToUpper().Contains("INVOICE NO"))
                {
                    if (strText.Contains(": "))
                    {
                        string[] strInvoiceNo = strText.Replace(": ", "").Trim().Split(' ');
                        txtInvoiceNo.Text = strInvoiceNo[0];
                    }
                    else if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index + 1].Replace(": ", "");
                }
                else if (strText.ToUpper().Contains("SUPPLIER'S REF.") && txtPurchaseParty.Text.Contains("SAM TRADERS"))
                {
                    if (txtInvoiceNo.Text.Contains("DELIVERY NOTE") || txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index + 1];
                }
                else if (strText.ToUpper().Contains("SARAOGI SUPER SALES PVT. LTD") && txtGSTNo.Text.Contains("AADCJ2544A") || txtPurchaseParty.Text.Contains("PAYT") || txtPurchaseParty.Text.Contains("PAY-T"))
                {
                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index - 1].Trim();
                }
                else if (strText.ToUpper().Contains("DATE OF INVOICE :"))
                {
                    string[] strInvoiceDate = strText.Replace("                    ", " ").Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[4].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 3)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[3].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains(": INVOICE DATE"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[0].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NUMBER   :"))
                {
                    string[] strInvoiceNo = strText.Trim().Split(' ');
                    if (strInvoiceNo.Length > 1)
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE DATE         :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE "))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        if (strInvoiceDate[2].Length > 8)
                        {
                            DateTime _iDate = DateTime.Now;
                            ConvertDateTime(ref _iDate, strInvoiceDate[2].Trim());
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Trim() == "INVOICE" && _index > 0)
                {
                    if (txtInvoiceNo.Text == "ISSUE DATE :")
                        txtInvoiceNo.Text = "";

                    if (txtInvoiceNo.Text == "" && txtInvoiceDate.Text.Length != 10)
                    {
                        string[] strGST = _lines[_index + 1].Split(' ');
                        txtInvoiceNo.Text = strGST[0];

                        strGST = _lines[_index + 2].Split(' ');
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strGST[0]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE"))
                {
                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');

                        if (txtInvoiceNo.Text == "")
                            txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    }
                }
                else if (strText.ToUpper().Contains("DATED") && !strText.ToUpper().Contains("LR DATED"))
                {
                    if (_lines[_index + 1] != "" && !_lines[_index + 1].Contains("Delivery Note"))
                    {
                        if (txtInvoiceDate.Text.Length != 10)
                        {
                            DateTime _iDate = DateTime.Now;
                            string[] strInvoiceDate = strText.Trim().Split(' ');
                            if (strInvoiceDate[strInvoiceDate.Length - 1].Length > 6)
                                ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                            else
                                ConvertDateTime(ref _iDate, _lines[_index + 1]);
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("DATE.   :"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        DateTime _iDate = DateTime.Now;
                        string[] strInvoiceDate = _strText.Trim().Split(' ');
                        if (strInvoiceDate.Length > 2)
                            ConvertDateTime(ref _iDate, strInvoiceDate[2]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATE :"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        DateTime _iDate = DateTime.Now;
                        string[] strInvoiceDate = _strText.Trim().Split(' ');
                        if (strInvoiceDate.Length > 2)
                            ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATE"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index + 1].Replace(":", "").Trim();
                        if (strDate.Length > 7 && (strDate.Contains("2019") || strDate.Contains("2020") || strDate.Contains("2021")))
                        {
                            DateTime _iDate = DateTime.Now;
                            ConvertDateTime(ref _iDate, strDate);
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("ORIGINAL FOR RECIPIENT"))
                {
                    string[] strGST = strText.Trim().Split(' ');
                    if (txtGSTNo.Text == "" && strGST.Length > 1)
                    {
                        string _strGST = strGST[0];
                        if (_strGST.Length == 15)
                            SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.Replace(" ", "").Replace(" ", "").ToUpper().Trim().Contains("BILLINGADDRESSSHIPPINGADDRESSTRANSPORT"))
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Trim().Split(' ');
                    SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                }
                else if (strText.ToUpper().Contains("PLACE OF SUPPLY"))
                {
                    if (txtPurchaseParty.Text.Contains("JAI AMBEY"))
                    {
                        txtInvoiceNo.Text = _lines[_index + 1];
                        _itemIndex = _index + 20;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("JOLLY FASHIONS"))
                {
                    txtInvoiceNo.Text = _lines[_index - 1];
                    _itemIndex = _index + 20;
                    break;
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("J.D. FASHION WEAR"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index - 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if ((strText.ToUpper().Contains("ME/") || strText.ToUpper().Contains("HLLP")) && (strText.ToUpper().Contains("19-20") || strText.ToUpper().Contains("20-21")))
                {
                    txtInvoiceNo.Text = strText.Trim();
                }
                else if (strText.Contains("MRP"))
                {
                    if (txtPurchaseParty.Text.Contains("SPARKY"))
                    {
                        _itemIndex = _index + 2;
                        break;
                    }
                    else if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        _itemIndex = _index + 6;
                        break;
                    }
                    else if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        _itemIndex = _index;
                        break;
                    }
                    else if (strText.ToUpper().Contains("DESCRIPTION"))
                    {
                        _itemIndex = _index + 1;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("DESCRIPTION OF GOODS"))
                {
                    if (txtInvoiceNo.Text != "" || txtPurchaseParty.Text != "")
                    {
                        _itemIndex = _index + 1;
                        if (txtPurchaseParty.Text.Contains("AGARWAL COLLECTION"))
                            _itemIndex--;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("DESCRIPTION"))
                {
                    _itemIndex = _index + 1;
                    if (txtPurchaseParty.Text.Contains("S.R CREATION"))
                        _itemIndex--;
                    break;
                }
                else if (strText.ToUpper().Contains("H.O. ADDRESS :"))
                {
                    _itemIndex = _index;
                    break;
                }
                else if (strText.ToUpper().Contains("GRS NO."))
                {
                    _strSType = "AGENT";
                }
                _index++;
            }

            if (_bBonny && txtPurchaseParty.Text == "")
            {
                txtGSTNo.Text = "24AATFB2023M1ZE";
                txtPurchaseParty.Text = "AH5112 BONNYS NX : AHD";
                if (txtPurchaseParty.Text.Contains("BONNYS NX"))
                    _itemIndex = 20;
            }
        }

        private void SetSupplierDetailsWithGSTNo(string strGSTNO)
        {
            string strPartyName = "", strStateName = "";
            if (strGSTNO != "")
            {
                bool _blackListed = false;
                if (dba.CheckTransactionLockWithBlackListStateNameFromGSTNo(strGSTNO, ref _blackListed, ref strPartyName, ref strStateName))
                {
                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (MainPage._bTaxStatus)
                        txtPurchaseParty.Text = "";
                    txtStateName.Text = txtGSTNo.Text = "";
                }
                else if (_blackListed)
                {
                    MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (MainPage._bTaxStatus)
                        txtPurchaseParty.Text = "";
                    txtStateName.Text = txtGSTNo.Text = "";
                }
                else
                {
                    if (MainPage._bTaxStatus)
                        txtPurchaseParty.Text = strPartyName;
                    txtGSTNo.Text = strGSTNO;
                    txtStateName.Text = strStateName;
                    if (!MainPage._bTaxStatus)
                    {
                        string str = dba.GetSupplierNickNameWithGSTNoWithOtherFirm(txtGSTNo.Text);
                        if (str != "")
                            txtPurchaseParty.Text = str;
                    }
                    //GetPartyDhara();
                }
            }
        }


        private bool ConvertDateTime(ref DateTime _date, string strDate)
        {
            try
            {
                double dDate = dba.ConvertObjectToDouble(strDate);
                if (dDate > 0)
                    _date = DateTime.FromOADate(dDate);
                else
                {
                    try
                    {
                        char split = '/';
                        if (strDate.Contains("-"))
                            split = '-';
                        string[] strNDate = strDate.Split(' ');
                        string[] strAllDate = strNDate[0].Split(split);
                        string strMonth = strAllDate[0], strFormat = "dd/MM/yyyy";
                        if (strMonth.Length == 1)
                            strFormat = "d/M/yyyy";

                        if (dba.ConvertObjectToInt(strMonth) == MainPage.currentDate.Month)
                        {
                            strFormat = "MM/dd/yyyy";
                            if (strMonth.Length == 1)
                                strFormat = "M/d/yyyy";
                        }
                        if (strAllDate.Length > 2)
                        {
                            if (strAllDate[2].Length == 2)
                                strFormat = strFormat.Replace("yyyy", "yy");
                        }

                        if (strDate.Contains("-"))
                            strFormat = strFormat.Replace("/", "-");

                        if (strDate.Length > 10)
                        {
                            string strTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                            if (strDate.Contains("AM") || strDate.Contains("PM"))
                                strFormat += " " + strTimeFormat;// " hh:mm:ss tt";//
                            else
                            {
                                string[] strTime = strDate.Split(':');
                                if (strTime.Length > 2)
                                    strFormat += " HH:mm:ss";
                                else
                                    strFormat += " HH:mm";
                            }
                        }

                        _date = dba.ConvertDateInExactFormat(strDate, strFormat);
                    }
                    catch
                    {
                        _date = Convert.ToDateTime(strDate);
                    }
                }
                return true;
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransport.Text = objSearch.strSelectedData;
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

        private void txtTaxAmt_DoubleClick(object sender, EventArgs e)
        {
            if (!pnlTax.Visible)
                pnlTax.Visible = true;
            else
                pnlTax.Visible = false;
        }

        private void txtProfitMargin_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (MainPage._bPurchaseBillWiseMargin)
                {
                    CalculateAllAmount();
                }
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtTaxFreeAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtTaxFreeAmt.Text == "")
                    txtTaxFreeAmt.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtTaxFreeAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }

        private void PurchaseBook_Retail_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            }
            catch { }
        }

        private void txtLRNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtLRDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtLRDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if(txtLRNo.Text!="")
                    dba.GetDateInExactFormat(sender, true, true, true);
                else
                    dba.GetDateInExactFormat(sender, false, false, true);
                if (txtLRDate.Text.Length != 10)
                    txtLRDate.Text = MainPage.strCurrentDate;
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtGodown_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH GODOWN NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtGodown.Text = objSearch.strSelectedData;
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

        private void txtStockStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASESTATUS", "SEARCH PURCHASE STATUS", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData!="")
                        txtStockStatus.Text = objSearch.strSelectedData;
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

        private void txtDueDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtMode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTMODE", "SEARCH TRANSPORT MODE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtMode.Text = objSearch.strSelectedData;
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

        private void txtReceivedBy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("RECEIVEDBY", "SEARCH RECEIVED BY", e.KeyCode);
                        objSearch.ShowDialog();
                        txtReceivedBy.Text = objSearch.strSelectedData;
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

        private void txtCountedBy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("RECEIVEDBY", "SEARCH COUNTED BY", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCountedBy.Text = objSearch.strSelectedData;
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

        private void txtBarcodedBy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("RECEIVEDBY", "SEARCH BARCODED BY", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBarcodedBy.Text = objSearch.strSelectedData;
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

        private void btnSetup_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = true;
        }

        private void chkTCSAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                CalculateAllAmount();
            }
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if(dgrdDetails.CurrentCell.ColumnIndex>11)
                    {
                        if (_objData != null)
                            _objData.Close();
                        if (_objSearch != null)
                            _objSearch.Close();
                    }
                }
                else
                {
                    if(_objSearch!=null)
                    {
                        _objSearch.txtSearch.Text = e.KeyChar.ToString().Trim();
                        _objSearch.txtSearch.SelectionStart = 1;
                    }
                    if (_objData != null)
                    {
                        _objData.txtSearch.Text = e.KeyChar.ToString().Trim();
                        _objData.txtSearch.SelectionStart = 1;
                    }
                }
            }
            catch { }

        }

        private void txtBillCode_Enter(object sender, EventArgs e)
        {
            dba.ChangeFocusColor(sender, e);
        }

        private void txtBillCode_Leave(object sender, EventArgs e)
        {
            dba.ChangeLeaveColor(sender, e);
        }

        private void dgrdDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellFocusColor(sender, e);
        }

        private void dgrdDetails_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellLeaveColor(sender, e);
        }

        private void txtSplDisAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dSpclPerAmt = 0, dSpclPer = 0, dGrossAmt = 0;
                dSpclPerAmt = ConvertObjectToDouble(txtSplDisAmt.Text);
                if (dSpclPerAmt > 0)
                {
                    dGrossAmt = ConvertObjectToDouble(lblGrossAmt.Text);
                    if (dGrossAmt > 0)
                    {
                        dSpclPer = (dSpclPerAmt * 100.00) / dGrossAmt;
                    }
                }
                txtSpclDisPer.Text = Math.Round(dSpclPer, 4).ToString();
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);

        }

        private void txtSplDisAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.KeyHandlerPoint(sender, e, 4);
            }
        }

        private void txtmanufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("AGENTNAME", "SEARCH AGENT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtmanufacturer.Text = objSearch.strSelectedData;
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

        private void txtDiscAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dOtherPerAmt = 0, dOtherPer = 0, dGrossAmt = 0;
                dOtherPerAmt = ConvertObjectToDouble(txtDiscAmt.Text);
                if (dOtherPerAmt > 0)
                {
                    dGrossAmt = ConvertObjectToDouble(lblGrossAmt.Text);
                    if (dGrossAmt > 0)
                    {
                        dOtherPer = (dOtherPerAmt * 100.00) / dGrossAmt;
                    }
                }
                txtOtherPer.Text = Math.Round(dOtherPer, 4).ToString();
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtTaxFreeSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtTaxFreeSign.Text == "")
                    txtTaxFreeSign.Text = "+";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 3)
                {
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        string strDesignName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strDesignName != "")
                        {
                            DesignMaster objDesign = new DesignMaster(strDesignName);
                            objDesign.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objDesign.ShowInTaskbar = true;
                            objDesign.ShowDialog();
                        }
                    }
                }
            }
            catch { }
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string strData1=chkVariant1.Checked.ToString(), strData2 = chkVariant2.Checked.ToString(), strData3= chkRoundTo5.Checked.ToString(), strData4 = chkRoundToU5.Checked.ToString(), strData5 = chkMUAfterDisc.Checked.ToString(), strData6 = chkMuAfterTax.Checked.ToString(), strData7="0", strData8 = "0", strData9 = "0", strData10= "0",strOther1="", strOther2 = "", strOther3 = "";
                int _count = dba.SavePurchaseSetup(strData1, strData2, strData3, strData4, strData5, strData6, strData7, strData8, strData9, strData10, strOther1, strOther2, strOther3);
                if(_count>0)
                {
                    _bVariant1 = chkVariant1.Checked;
                    _bVariant2 = chkVariant2.Checked;
                    _bRoundTo5 = chkRoundTo5.Checked;
                    _bRoundToU5 = chkRoundToU5.Checked;
                    _bMUAfterDisc = chkMUAfterDisc.Checked;
                    _bMUAfterTax = chkMuAfterTax.Checked;
                    panalColumnSetting.Visible = false;
                }
            }
            catch { }

        }


        private void btnBarCodePrint_Click(object sender, EventArgs e)
        {
            btnBarCodePrint.Enabled = false;
            try
            {
                
                BarCode_Printing objBarCode = new BarCode_Printing(txtPurchaseParty.Text, txtmanufacturer.Text, txtBillCode.Text,txtBillNo.Text,txtDate.Text,dgrdDetails);
                objBarCode.MdiParent = MainPage.mymainObject;
                objBarCode.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bar code in Purchase Book Retail", ex.Message };
                dba.CreateErrorReports(strReport);
            }

            btnBarCodePrint.Enabled = true;
        }

        private bool CheckItemNameExistence(ref string strItemName, ref string strHSNCode)
        {
          
            try
            {
                if (strItemName != "")
                {
                    strItemName = CheckItemName(strItemName, strHSNCode);
                    if (strItemName != "")
                    {
                        if (strHSNCode == "")
                            strHSNCode = GetHSNCodeFromItem(strItemName);
                    }
                    else if (strHSNCode == "")
                    {
                        strHSNCode = GetHSNCodeFromItem(strItemName);
                    }
                }
            }
            catch { }
            return true;
        }

        private void txtGSTNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strSearch = "PURCHASEPARTYWITHGSTNO";
                        if (MainPage.strUserRole.Contains("ADMIN"))
                            strSearch = "PURCHASEPERSONALPARTY";

                        SearchData objSearch = new SearchData(strSearch, "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData, strGSTNo = "", strStateName = "";
                        if (strData != "")
                        {
                            bool _blackListed = false;
                            if (dba.CheckTransactionLockWithBlackListGSTNo(strData, ref _blackListed, ref strGSTNo, ref strStateName))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPurchaseParty.Text = "";
                                txtGSTNo.Text = "";
                            }
                            else if (_blackListed)
                            {
                                MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPurchaseParty.Text = "";
                                txtGSTNo.Text = "";
                            }
                            else
                            {
                                txtPurchaseParty.Text = strData;
                                txtGSTNo.Text = strGSTNo;
                                txtStateName.Text = strStateName;
                                //GetPartyDhara();
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

        private string GetHSNCodeFromItem(string strItem)
        {
            string strQuery = "";
            strQuery = " Select _IGM.HSNCode from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where _IM.ItemName='" + strItem + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        private string CheckItemName(string strItemName, string strHSNCode)
        {
            try
            {
                if (strHSNCode.Length > 4)
                    strHSNCode = strHSNCode.Substring(0, 4);
                string strMainItemName = strItemName, strItemQuery = "", strFirstItemQuery = "", strSecondItemQuery = "", strThirdQuery = "", strFirstItemName = "", strSecondItemName = "", strThirdItemName = "", strSubQuery = "";
                string[] strItem = strItemName.Split(' ');
                strMainItemName = strMainItemName.Replace(" ", "").Replace(".", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("/", "").Replace("@", "");
                strMainItemName = System.Text.RegularExpressions.Regex.Replace(strMainItemName, @"[\d-]", string.Empty);
                if (strItem.Length > 1)
                {
                    strFirstItemName = strItem[0];
                    //strFirstItemName= System.Text.RegularExpressions.Regex.Replace(strFirstItemName, @"[\d-]", string.Empty);
                    if (strFirstItemName.Length > 2)
                    {
                        for (int i = 0; i < strFirstItemName.Length; i++)
                        {
                            if (i == 0)
                                strFirstItemQuery += " and DesignName Like('" + strFirstItemName[i] + "%') ";
                            else
                                strFirstItemQuery += " and DesignName Like('%" + strFirstItemName[i] + "%') ";
                        }
                    }
                    strSecondItemName = strItem[1];
                    strSecondItemName = System.Text.RegularExpressions.Regex.Replace(strSecondItemName, @"[\d-]", string.Empty);
                    if (strSecondItemName.Length > 2)
                    {
                        for (int i = 0; i < strSecondItemName.Length; i++)
                        {
                            if (i == 0)
                                strSecondItemQuery += " and DesignName Like('" + strSecondItemName[i] + "%') ";
                            else
                                strSecondItemQuery += " and DesignName Like('%" + strSecondItemName[i] + "%') ";
                        }
                    }
                    if (strItem.Length > 2)
                    {
                        strThirdItemName = strItem[2];
                        strThirdItemName = System.Text.RegularExpressions.Regex.Replace(strThirdItemName, @"[\d-]", string.Empty);
                        if (strThirdItemName.Length > 2)
                        {
                            for (int i = 0; i < strThirdItemName.Length - 1; i++)
                            {
                                if (i == 0)
                                    strThirdQuery += " and DesignName Like('" + strThirdItemName[i] + "%') ";
                                else
                                    strThirdQuery += " and DesignName Like('%" + strThirdItemName[i] + "%') ";
                            }
                        }
                    }
                }
                if (strMainItemName.Length > 1)
                {
                    for (int i = 0; i < strMainItemName.Length; i++)
                    {
                        if (i == 0)
                            strItemQuery += " and DesignName Like('" + strMainItemName[i] + "%') ";
                        else
                            strItemQuery += " and DesignName Like('%" + strMainItemName[i] + "%') ";
                    }
                }

                if (strFirstItemQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,2 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strFirstItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,3 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strFirstItemQuery;
                }
                if (strSecondItemQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,4 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strSecondItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,5 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strSecondItemQuery;
                }
                if (strThirdQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,6 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strThirdQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,7 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strThirdQuery;
                }

                if (strItemQuery != "")
                {
                    string strQuery = " Select Top 1 * from ( "
                                    + " Select Distinct ItemName,-1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName ='" + strItemName + "' UNION ALL "
                                    + " Select Distinct ItemName,0 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName Like('" + strItemName + "%') UNION ALL "
                                    + " Select Distinct ItemName,1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName Like('%" + strItemName + "%') UNION ALL "
                                    + " Select Distinct ItemName,0 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                    + " Select Distinct ItemName,1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strItemQuery + strSubQuery
                                    + " )_Sale Order By SerialNo ";

                    object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                    return Convert.ToString(objValue);
                }
                else
                    return "";
            }
            catch { }
            return "";
        }
    }
}
