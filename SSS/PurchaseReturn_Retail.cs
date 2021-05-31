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
    public partial class PurchaseReturn_Retail : Form
    {
        DataBaseAccess dba;
        SendSMS objSMS;
        string strLastSerialNo = "", strDeletedSID = "", strPurchaseBillCode = "", strOldPartyName="", _strAttachBillWithComma="";
        bool qtyAdjustStatus = false;
        public bool saleStatus = false, updateStatus = false, newStatus = false;
        double dOldNetAmt = 0, dOldPurReturnAmt = 0;
        SearchCategory_Custom _objSearch;
        SearchData _objData;

        public PurchaseReturn_Retail()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            GetStartupData(true);
        }

        public PurchaseReturn_Retail(string strSerialCode, string strSerialNo)
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
                    if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                        BindRecordWithControl(strLastSerialNo);
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
                    string strQuery = "  Select *,Convert(varchar(20),PR.WayBillDate,103)+' '+Convert(varchar(20),PR.WayBillDate,108)WayBDate,Convert(varchar,PR.Date,103)BDate,Convert(varchar,PurchaseBillDate,103)PDate,(PurchasePartyID+' '+SM.Name) PurchaseParty,SM.GSTNo,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,PR.Date))) LockType from PurchaseReturn PR LEFT JOIN SupplierMaster SM ON PR.PurchasePartyID = SM.AreaCode+SM.AccountNo Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                    + " Select PRD.*, HSNCode from PurchaseReturnDetails PRD OUTER APPLY ( Select Top 1 _IS.Description as BarCode,IGM.HSNCode from Items _IM inner join ItemSecondary _IS on _IM.BillCode = _IS.BillCode  and _IM.BillNo = _IS.BillNo inner join ItemGroupMaster IGM on _IM.GroupName = IGM.GroupName Where _IM.ItemName = PRD.ItemName and _IS.Variant1 = PRD.Variant1 and _IS.Variant2 = PRD.Variant2 and[ActiveStatus] = 1) _IM Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID "
                                    + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;


                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            //pnlTax.Visible = true;
                            BindDataWithControlUsingDataTable(_dt);
                            BindPurchaseReturnDetails(ds.Tables[1]);
                            BindGSTDetailsWithControl(ds.Tables[2]);
                        }
                    }
                    EditOption();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                strOldPartyName = txtSupplierName.Text = Convert.ToString(row["PurchaseParty"]);            
                txtPBillCode.Text = Convert.ToString(row["PurchaseBillCode"]);
                txtPurchaseBillNo.Text = Convert.ToString(row["PurchaseBillNo"]);
                txtPurchaseBillDate.Text = Convert.ToString(row["PDate"]);
                txtPurchaseInvoice.Text = Convert.ToString(row["ReverseCharge"]);
                txtPurchaseType.Text = Convert.ToString(row["PurchaseType"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtSignAmt.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);             
                txtInsuranceAmt.Text = Convert.ToString(row["NetDiscount"]);
                txtTransportName.Text = Convert.ToString(row["Transport"]);
                txtWayBillNo.Text = Convert.ToString(row["WaybillNo"]);
                txtWayBillDate.Text = Convert.ToString(row["WayBDate"]);

                txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                txtAttachBill.Text = Convert.ToString(row["AttachedBill"]);

                dOldPurReturnAmt = dba.ConvertObjectToDouble(row["PartPurReturnAmt"]);
                txtPurReturnAmt.Text = dOldPurReturnAmt.ToString("N2", MainPage.indianCurancy);

                if (MainPage._bTaxStatus)
                    txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                else
                    txtGSTNo.Text = Convert.ToString(row["Other2"]);

                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";


                // txtInsurancePer.Text = Convert.ToString(row["OtherValue"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                txtTaxFree.Text = Convert.ToString(row["TaxFree"]);
                lblQty.Text = Convert.ToString(row["TotalQty"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                dOldNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                
                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                double dOtherAmtPer = dba.ConvertObjectToDouble(Convert.ToString(row["OtherValue"]));
                if (dOtherAmtPer <= 0)
                {
                    txtOtherPerSign.Text = "-";
                    txtOtherPer.Text = Math.Abs(dOtherAmtPer).ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    txtOtherPerSign.Text = "+";
                    txtOtherPer.Text = dOtherAmtPer.ToString("N2", MainPage.indianCurancy);
                }

                if (Convert.ToString(row["EntryType"]) == "MANUAL")
                    rdoManual.Checked = true;
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
        private string GetHSNCode(object _objHSNCode)
        {
            string strQuery = "";

            strQuery = "Select Top 1 ItemName from Items _Im  inner join ItemGroupMaster IGM on _IM.GroupName=IGM.GroupName WHere ItemName Like('%" + _objHSNCode + "') OR HSNCode Like('" + _objHSNCode + "')";
            object obj = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(obj);
        }
        private void BindPurchaseReturnDetails(DataTable _dtDetails)
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
                        strHSNCode = GetHSNCode(Convert.ToString(row["HSNCode"]));
                        strBarCode = MainPage.strDataBaseFile;
                    }
                    else
                    {
                        strHSNCode = Convert.ToString(row["ItemName"]);
                        strBarCode = Convert.ToString(row["BarCode"]);
                    }
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["saleReturnNo"].Value = row["SRBillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["barcode"].Value = row["BarCode"];
                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strHSNCode; //row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = ConvertObjectToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = ConvertObjectToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dgrdDetails.Rows[rowIndex].Cells["amount"].Value = ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    //dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                    //dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["Packing"];
                    //dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
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
               // pnlTax.Visible = true;
            }
            else
                pnlTax.Visible = false;
        }

        private void BindRecordWithControl_Import()
        {
            try
            {
                if (txtImportData.Text != "")
                {

                    string strQuery = "  Select * from PurchaseReturnDetails Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'   order by SID ";

                    DataTable _dtDetails = SearchDataOther.GetDataTable_NC(strQuery);
                    if (_dtDetails.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Clear();
                        dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                        int rowIndex = 0;
                        foreach (DataRow row in _dtDetails.Rows)
                        {
                            dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                            dgrdDetails.Rows[rowIndex].Cells["saleReturnNo"].Value = row["SRBillNo"];
                            dgrdDetails.Rows[rowIndex].Cells["barcode"].Value = row["BarCode"];
                            dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
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
                            dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dgrdDetails.Rows[rowIndex].Cells["amount"].Value = ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                            //dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                            //dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["Packing"];
                            //dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                            rowIndex++;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || (btnEdit.Text == "&Update" && MainPage.strUserRole.Contains("ADMIN")))
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                        {
                            dgrdDetails.Rows.Clear();
                            txtSupplierName.Text = strData;
                            rdoPurchase.Checked = true;
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add();
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            }
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
                                    txtSupplierName.Text = "";
                                    if (MainPage._bTaxStatus)
                                        txtGSTNo.Text = "";
                                }
                                else if (_blackListed)
                                {
                                    MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSupplierName.Text = "";
                                    if (MainPage._bTaxStatus)
                                        txtGSTNo.Text = "";
                                }
                                else
                                {
                                    if (btnAdd.Text == "&Save")
                                    {
                                        txtGSTNo.Text = "";
                                        dgrdDetails.Rows.Clear();
                                        txtSupplierName.Text = strData;
                                        rdoPurchase.Checked = true;
                                        txtPurchaseBillNo.Clear();
                                        if (dgrdDetails.Rows.Count == 0)
                                        {
                                            dgrdDetails.Rows.Add();
                                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch(Exception ex)
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
             txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly =  txtOtherPer.ReadOnly=txtPurchaseInvoice.ReadOnly= txtOtherPerSign.ReadOnly =txtTaxFree.ReadOnly= txtTaxPer.ReadOnly = false;// txtTaxPer.ReadOnly=
            dgrdDetails.ReadOnly =  false;
            if (MainPage.strUserRole.Contains("ADMIN"))
                txtTaxPer.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly =  txtTaxPer.ReadOnly = txtOtherPer.ReadOnly = txtPurchaseInvoice.ReadOnly = txtOtherPerSign.ReadOnly= txtTaxFree.ReadOnly =txtTaxPer.ReadOnly= true;
            dgrdDetails.ReadOnly =true;          
            lblMsg.Text =lblCreatedBy.Text= "";
        }

        private void ClearAllText()
        {
            txtGSTNo.Text = txtAttachBill.Text = txtSupplierName.Text = txtPurchaseType.Text = lblTaxableAmt.Text = txtPurchaseType.Text = txtRemark.Text = txtPurchaseBillNo.Text = txtPurchaseInvoice.Text = "";
            txtRoundOff.Text = txtOtherAmt.Text = txtInsuranceAmt.Text = txtTaxAmt.Text = lblQty.Text = lblGrossAmt.Text = lblNetAmt.Text = txtOtherPer.Text = txtPurReturnAmt.Text = "0.00";
            txtSignAmt.Text = txtROSign.Text = "+";
            txtOtherPerSign.Text = "-";
            txtTaxPer.Text = "18.00";
            rdoAll.Checked = true;
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = false;
            lblCreatedBy.Text = "";
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtPurchaseBillDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtPurchaseBillDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

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
            if (txtSupplierName.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Creditor Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return false;
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
            return ValidateOtherValidation(false);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex <2 || e.ColumnIndex == 13 || e.ColumnIndex == 17)
                        e.Cancel = true;   
                    else if(e.ColumnIndex==3)
                    {
                        if (rdoManual.Checked)
                        {
                            _objData = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                            _objData.ShowDialog();
                            dgrdDetails.CurrentRow.Cells["oldbrandName"].Value = dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        //if (rdoManual.Checked)
                       // {
                        string strBrandName = Convert.ToString(dgrdDetails.CurrentRow.Cells["oldbrandName"].Value);
                        _objSearch = new SearchCategory_Custom("", "ITEMNAME_PURCHASERETURN_RETAIL", strBrandName, "", "", "", "", "", "", Keys.Space, false, false, "BarCode");
                        _objSearch.ShowDialog();
                        GetAllDesignSizeColorWithBarCode(_objSearch, dgrdDetails.CurrentRow.Index);
                        GetBillFromPurchase(dgrdDetails.CurrentRow);
                        AddNewRow(dgrdDetails.CurrentRow.Index);
                        // }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 5 || e.ColumnIndex ==6 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10)
                    {
                        string strQuery = "";
                        if (rdoManual.Checked)
                        {
                            if (e.ColumnIndex == 6)
                            {
                                SearchCategory objSearch = new SearchCategory("1", MainPage.StrCategory1, Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                e.Cancel = true;
                            }
                            else if (e.ColumnIndex == 7)
                            {
                                SearchCategory objSearch = new SearchCategory("2", MainPage.StrCategory2, Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                e.Cancel = true;
                            }
                            else if (e.ColumnIndex == 8)
                            {
                                SearchCategory objSearch = new SearchCategory("3", MainPage.StrCategory3, Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                e.Cancel = true;
                            }
                            else if (e.ColumnIndex == 9)
                            {
                                SearchCategory objSearch = new SearchCategory("4", MainPage.StrCategory4, Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                e.Cancel = true;
                            }
                            else if (e.ColumnIndex == 10)
                            {
                                SearchCategory objSearch = new SearchCategory("5", MainPage.StrCategory5, Keys.Space);
                                objSearch.ShowDialog();
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                e.Cancel = true;
                            }
                            else
                            {
                                SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", "", "", "", "", "", Keys.Space, true, "");
                                objSearch.ShowDialog();
                                GetAllDesignSizeColor(objSearch, dgrdDetails.CurrentRow.Index);
                            }
                        }
                        else if(txtPurchaseBillNo.Text != "")
                        {
                            strQuery += " and BillCode='" + txtPBillCode.Text + "' and BillNo=" + txtPurchaseBillNo.Text + "  ";

                            SearchData objSearch = new SearchData("PURCHASEBILLDETAILFORRETURN_RETAIL", strQuery, "SEARCH PURCHASE BILL DETAIL", Keys.Space);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                if (rdoAll.Checked)
                                    GetDetailsFromPurchaseBillNo(objSearch.strSelectedData, e.RowIndex);

                                GetBillFromPurchase(dgrdDetails.CurrentRow);
                                CalculateAllAmount();
                                AddNewRow(dgrdDetails.CurrentRow.Index);
                            }
                        }
                        e.Cancel = true;
                    }
                    else if(!rdoManual.Checked)
                    {
                        if ((e.ColumnIndex == 14 || e.ColumnIndex == 15 || e.ColumnIndex == 16) && !MainPage.strUserRole.Contains("ADMIN"))
                            e.Cancel = true;
                    }
                }               
                else
                    e.Cancel = true;
            }
            catch
            {
                e.Cancel = true;
            }
        }


        private void GetBillFromPurchase(DataGridViewRow row)
        {
            try
            {
                string strQuery = "", strBarcode = Convert.ToString(row.Cells["barcode"].Value);
                if (!rdoManual.Checked && strBarcode != "") // Check How Many Purchase bill having this Item -- For Only First Item
                {
                    if (row.Index == 0)
                        strQuery += " Select Distinct ((CAST(PB.BillNo as varchar))+'|'+ISNULL(CONVERT(varchar,PB.Date,103),'')) as PURCHASEBILLNOFORMPURCHASE_RETAIL,PB.BillNo,PB.BillCode, dbo.GetFullName(PB.PurchasePartyID)SupplierName,PB.PurchaseType,SM.GSTNo,PB.InvoiceNo,PB.Description Description_3 from PurchaseBook PB JOIN PurchaseBookSecondary PBS ON PBS.BillCode = PB.BillCode AND PBS.BillNo = PB.BillNo LEFT JOIN SupplierMaster SM ON AreaCode+AccountNo = PB.PurchasePartyID WHERE  BarCode = '" + strBarcode + "'";
                    else if (row.Index > 0 && txtBillNo.Text != "")
                        strQuery += " Select Distinct ((CAST(PB.BillNo as varchar))+'|'+ISNULL(CONVERT(varchar,PB.Date,103),'')) as PURCHASEBILLNOFORMPURCHASE_RETAIL,PB.BillNo,PB.BillCode, dbo.GetFullName(PB.PurchasePartyID)SupplierName,PB.PurchaseType,SM.GSTNo,PB.InvoiceNo,PB.Description Description_3 from PurchaseBook PB JOIN PurchaseBookSecondary PBS ON PBS.BillCode = PB.BillCode AND PBS.BillNo = PB.BillNo LEFT JOIN SupplierMaster SM ON AreaCode+AccountNo = PB.PurchasePartyID WHERE PB.BillCode = '" + txtPBillCode.Text + "' AND PB.BillNo = " + txtPurchaseBillNo.Text + " AND PBS.Barcode = '" + strBarcode + "'";

                    DataTable dt = dba.GetDataTable(strQuery);
                    if (row.Index == 0) // Check How Many Purchase bill having this Item -- For Only First Item
                    {
                        if (dt.Rows.Count < 1)
                        {
                            MessageBox.Show("Sorry ! This Item ( " + row.Cells["ItemName"].Value + " ) is not from Any Purchase Bill.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dgrdDetails.Rows.Remove(row);
                            txtPurchaseBillNo.Text = txtPurchaseBillDate.Text = "";
                        }
                        else if (dt.Rows.Count == 1)
                        {
                            string[] strBillNo = Convert.ToString(dt.Rows[0]["PURCHASEBILLNOFORMPURCHASE_RETAIL"]).Split('|');
                            txtPurchaseBillNo.Text = strBillNo[0];
                            txtPBillCode.Text = Convert.ToString(dt.Rows[0]["BillCode"]);

                            txtImportData.Text = Convert.ToString(dt.Rows[0]["Description_3"]);
                            txtPurchaseInvoice.Text = Convert.ToString(dt.Rows[0]["InvoiceNo"]);
                            txtSupplierName.Text = Convert.ToString(dt.Rows[0]["SupplierName"]);
                            txtPurchaseType.Text = Convert.ToString(dt.Rows[0]["PurchaseType"]);
                            txtGSTNo.Text = Convert.ToString(dt.Rows[0]["GSTNo"]);

                            if (strBillNo.Length > 1)
                                txtPurchaseBillDate.Text = strBillNo[1];
                        }
                        else
                        {
                            SearchData objSearch = new SearchData("", "", "SEARCH PURCHASE BILL NO", Keys.Space);
                            objSearch.table = dt;

                            if (objSearch.table != null)
                            {
                                foreach (DataRow ro in objSearch.table.Rows)
                                {
                                    objSearch.lbSearchBox.Items.Add(ro[0]);
                                }
                            }
                            objSearch.strSearchData = "PURCHASEBILLNOFORMPURCHASE_RETAIL";
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                string[] strBillNo = objSearch.strSelectedData.Split('|');
                                txtPurchaseBillNo.Text = strBillNo[0];
                                if (strBillNo.Length > 1)
                                    txtPurchaseBillDate.Text = strBillNo[1];
                            }
                            else
                            {
                                string[] strBillNo = Convert.ToString(objSearch.lbSearchBox.Items[0]).Split('|');
                                txtPurchaseBillNo.Text = strBillNo[0];
                                txtPurchaseBillDate.Text = strBillNo[1];
                            }
                        }
                    }
                    else 
                    {
                        // Check if this Item is belogning to the Purchase bill Selected by First Item For Only Next All Items
                        if (!(dt.Rows.Count > 0))
                        {
                            int index = row.Index;
                            MessageBox.Show("Sorry ! This Item ( " + row.Cells["ItemName"].Value + " ) is not from Selecetd Purchase Bill - " + txtPBillCode.Text + " " + txtPurchaseBillNo.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dgrdDetails.Rows.Remove(row);

                            string strItemName = Convert.ToString(dgrdDetails.Rows[index-1].Cells["itemName"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[index - 1].Cells["amount"].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "CHECKING ITEM IN PURCHASE BILL : Purchase Return Retail", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // btnEdit.Enabled = btnAdd.Enabled = false;
            }
        }

        private void GetAllDesignSizeColorWithBarCode(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {              
                if (objCategory != null)
                {
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData != "")                        
                        {
                            if (strData != "ADD NEW DESIGNNAMEWITHBARCODE_SALEMERGE NAME")
                            {
                                string[] strAllItem = strData.Split('|');
                                if (strAllItem.Length > 0)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0].Trim();
                                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[2];
                                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[3].Trim();
                                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;

                                    if (strAllItem.Length > 6)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[4].Trim();
                                    if (strAllItem.Length > 7)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[5].Trim();
                                    if (strAllItem.Length > 8)
                                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[6].Trim();
                                    if (strAllItem.Length > 9)
                                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[7].Trim();
                                    if (strAllItem.Length > 10)
                                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[8].Trim();

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value) == "")
                                        dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = strAllItem[strAllItem.Length - 1];

                                    GetPurchaseRate_BarCode(dgrdDetails.Rows[rowIndex]);
                                    SetUnitName(strAllItem[3], rowIndex);
                                }
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();                     

                        if (dgrdDetails.Rows.Count > 0)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells["barcode"];
                            dgrdDetails.Focus();
                        }
                    }
                }
            }
            catch
            {
            }
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
                                if (strItem != "ADD NEW DESIGNNAME NAME")
                                {
                                    string[] strAllItem = strItem.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
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
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
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
                        ArrangeSerialNo();
                        CalculateAllAmount();
                    }
                }
            }
            catch
            {
            }
        }

        private void AddNewRow(int rowIndex)
        {
            if (btnAdd.Text == "&Save")
            {
                if (rowIndex < 0)
                {
                    dgrdDetails.Rows.Add(1);
                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barcode"];
                    dgrdDetails.Focus();
                }
                else if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "")
                {
                    dgrdDetails.Rows.Add(1);
                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barcode"];
                    dgrdDetails.Focus();
                }
            }
        }

        private void GetPurchaseRate_BarCode(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, dRate = 0;
                if (row != null)
                {
                    object objDisPer = 0, objRate=0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate_Return(ref objDisPer, row.Cells["barcode"].Value, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                        dRate = ConvertObjectToDouble(objRate);
                    }
                }
                if (dRate == 0)
                    dRate = dMRP;
                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                dAmt = dQty * dRate;

                row.Cells["netAmt"].Value = row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                
            }
            catch
            {
            }
        }

        private void GetPurchaseRate(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, dRate = 0;
                if (row != null)
                {
                    object objDisPer = 0, objSaleRate = 0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                    }
                }
                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;

                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                dAmt = dQty * dRate;

                row.Cells["netAmt"].Value = row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);              
            }
            catch
            {
            }
        }


        private void SetUnitName(string strDesignName, int rowIndex)
        {
            if (strDesignName != "")
            {
                DataTable table = dba.GetDataTable("Select ISNULL(QtyRatio,1) QtyRatio,UnitName as PurchaseUnit,StockUnitName UnitName from Items Where ItemName='" + strDesignName + "' ");
                if (table.Rows.Count > 0)
                {                   
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void GetDetailsFromPurchaseBillNo(string strData,int rowIndex)
        {
            try
            {
                string[] strValue = strData.Split('|');
                if (strValue.Length > 4)
                {
                    string strQuery = " Select * from PurchaseBookSecondary Where BillCode='"+txtPBillCode.Text+"' and BillNo="+txtPurchaseBillNo.Text+" and BarCode='"+ strValue[0] + "' and ItemName='" + strValue[1]+ "' and Variant1='" + strValue[2] + "' and Variant2='" + strValue[3] + "' and Qty=" + ConvertObjectToDouble(strValue[4]) + " and Rate=" + ConvertObjectToDouble(strValue[5]) + " ";
                    if (strQuery != "")
                    {
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            DataRow row = _dt.Rows[0];
                            dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                            dgrdDetails.Rows[rowIndex].Cells["id"].Value = "";// row["ID"];
                            dgrdDetails.Rows[rowIndex].Cells["saleReturnNo"].Value = "";
                            dgrdDetails.Rows[rowIndex].Cells["barcode"].Value = row["BarCode"];
                            dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
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
                            dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                            //dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Discount"];
                            //dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                           // dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
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
                    if (e.ColumnIndex == 12)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 14)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 15)
                        CalculateRateWithQtyAmount(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 16) //|| e.ColumnIndex == 14
                        CalculateDisWithMRPRate(dgrdDetails.Rows[e.RowIndex]);
                   
                }
            }
            catch
            {
            }
        }

        private void CalculateRateWithQtyAmount(DataGridViewRow rows)
        {
            double dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dAmount = 0, dDisPer = 0, dMRP = 0;//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            if (dAmount != 0 && dQty != 0)
                dRate = dAmount / dQty;
            dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
            dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value); 

            if (dDisPer != 0 && dMRP != 0)
                dRate = dMRP * (100.00 - dDisPer) / 100.00;
            if (dRate == 0)
                dRate = dMRP;

            dAmount = dRate * dQty;

            rows.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
            rows.Cells["netAmt"].Value = rows.Cells["amount"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
          //  rows.Cells["netAmt"].Value = dRate.ToString("N2", MainPage.indianCurancy);
           // rows.Cells["netAmt"].Value = (dAmount - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

            CalculateAllAmount();
        }

        private void CalculateDisWithMRPRate(DataGridViewRow rows)
        {
            double dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dAmount = 0, dDisPer = 0, dMRP = 0;//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
         
            dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
            dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);

            if (dRate != 0 && dMRP != 0)
                dDisPer =  (100.00 - (dRate * 100 / dMRP)) ;         

            dAmount = dRate * dQty;
            
            rows.Cells["disPer"].Value = Math.Round(dDisPer,4);
            rows.Cells["netAmt"].Value = rows.Cells["amount"].Value = dAmount.ToString("N2", MainPage.indianCurancy);            
            // rows.Cells["netAmt"].Value = (dAmount - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

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
                        if (IndexColmn < dgrdDetails.ColumnCount - 7)
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
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
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
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[3];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtRemark.Focus();
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
                        if (dgrdDetails.Rows.Count <= 0)
                            AddNewRow(-1);
                    }
                }
            }
            catch(Exception ex) { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 11 || columnIndex == 12 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16) // || columnIndex == 14
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (columnIndex == 11)
                dba.ValidateSpace(sender, e);
            else if (columnIndex == 12 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16) // || columnIndex == 14
            {
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
            double dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            double dAmt = dQty * dRate;
            rows.Cells["netAmt"].Value = rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
           // rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void CalculateAmountWithMRP(DataGridViewRow rows)
        {
            double dRate = 0;
            if (rows != null)
            {
                double dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                double dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);
                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;

                rows.Cells["rate"].Value = dRate;
                double dAmt, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }       

        private void CalculateAllAmount()
        {
            try
            {             
                double dFinalAmt=0,dQty = 0, dDisPer=0, dPurReturnAmt = 0, dTOAmt =0, dBasicAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dPostage = 0,dGreenTaxAmt=0,dRoundOff=0,dTaxFree;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value) ;
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }

                lblGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);             
                dOtherAmt = ConvertObjectToDouble(txtOtherAmt.Text);
                dDisPer = ConvertObjectToDouble(txtOtherPer.Text);
                dTaxFree = ConvertObjectToDouble(txtTaxFree.Text);
                dPurReturnAmt = dba.ConvertObjectToDouble(txtPurReturnAmt.Text);
                if (txtSignAmt.Text == "-")
                    dOtherAmt *= -1;

                double dGrossAmt = 0;

                dDiscAmt = (dBasicAmt * dDisPer) / 100;   
                dTOAmt = dOtherAmt + dPackingAmt+dPostage+dGreenTaxAmt;

                if (txtOtherPerSign.Text == "-")
                    dDiscAmt *= -1;

                dTOAmt = dOtherAmt + dPackingAmt;
                dGrossAmt = dBasicAmt + dTOAmt;               

                dFinalAmt = dGrossAmt + dDiscAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt,ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTaxFree - dPurReturnAmt;

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
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(BillNo)+1 from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' "));
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
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtPurchaseBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strPurchaseParty = "",  strPurchasePartyID = "", strTaxAccountID = "";
                string[] strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }

                double dRate = 0, dQty = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dAmt = 0, dPurReturnAmt=0;
                string strQuery = "", strSRBillNo = "", strGSTNo="";
                dPurReturnAmt = dba.ConvertObjectToDouble(txtPurReturnAmt.Text);

                if (!MainPage._bTaxStatus)
                    strGSTNo = txtGSTNo.Text;

                strQuery += " if not exists (Select BillCode from [dbo].[PurchaseReturn] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                        + " INSERT INTO [dbo].[PurchaseReturn] ([BillCode],[BillNo],[Date],[PurchasePartyID],[EntryType],[PurchaseType],[Remark],[OtherSign],[OtherAmt],[NetDiscount],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[ReverseCharge],[InsertStatus],[UpdateStatus],[PurchaseBillCode],[PurchaseBillNo],[PurchaseBillDate],[Description1],[Description2],[Description3],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[TaxFree],[Other2],[PartPurReturnAmt],[Transport],[WaybillNo],[WayBillDate]) VALUES "
                        + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strPurchasePartyID + "','" + GetEntryType() + "','" + txtPurchaseType.Text + "','" + txtRemark.Text + "','" + txtSignAmt.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," +
                        + dba.ConvertObjectToDouble(txtInsuranceAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dPTaxAmt + "," + dba.ConvertObjectToDouble(lblQty.Text) + "," + dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + strQtrStatus + "','" + txtOtherPerSign.Text + txtOtherPer.Text + "','" + MainPage.strLoginName + "','','"+txtPurchaseInvoice.Text+"',1,0,'" + txtPBillCode.Text + "','" + txtPurchaseBillNo.Text + "','" + strSDate + "','','','"+txtImportData.Text + "','"  + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxFree.Text) + ",'"+ strGSTNo + "',"+ dPurReturnAmt + ",'" + txtTransportName.Text + "','" + txtWayBillNo.Text + "','" + txtWayBillDate.Text + "')  "
                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                        + " ('" + strDate + "','" + strPurchaseParty + "','PURCHASE RETURN','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNetAmt.Text + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "') ";

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSRBillNo = Convert.ToString(row.Cells["saleReturnNo"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);

                    strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([RemoteID],[BillCode],[BillNo],[SRBillNo],[SalePartyID],[BarCode],[BrandName],[DesignName],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[Packing],[TotalAmt],[UnitName],[InsertStatus],[UpdateStatus],[ItemStatus],[DisStatus],[Discount]) VALUES "
                                  + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strSRBillNo + "','','" + row.Cells["barcode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + "," + dRate + ","
                                  + " " + dAmt + ",0,0, " + dAmt + ",'" + row.Cells["unitName"].Value + "',1,0,'FRESH',0,0)";

                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[BrandName],[DesignName],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                             + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + row.Cells["barcode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "') ";
                    }

                    if (strSRBillNo != "")
                        strQuery += " Update SaleReturnDetails Set PurchaseReturnStatus=1,PurchaseReturnNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' Where (BillCode+' '+CAST(BillNo as varchar))='" + strSRBillNo + "' and ItemName='" + row.Cells["itemName"].Value + "' and PurchaseBillNo='" + txtPBillCode.Text + " " + txtPurchaseBillNo.Text + "' ";

                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "" && MainPage._bTaxStatus)
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
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
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
                            SendSMSToParty();
                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
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

        private int UpdateRecord(string strSQuery)
        {
            int _count = 0;
            try
            {
                string strDate = "", strSDate = "", strQtrStatus = "CURRENT", strGSTNo="";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), sDate = dba.ConvertDateInExactFormat(txtPurchaseBillDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strSDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strPurchaseParty = "", strPurchasePartyID = "", strTaxAccountID = "", strDeletedSIDQuery = "";
                string[] strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }

                double dRate = 0, dAmt = 0, dQty = 0, dDis = 0, dPurReturnAmt = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "", strID = "", strSRBillNo = "";
                dPurReturnAmt = dba.ConvertObjectToDouble(txtPurReturnAmt.Text);
                //     strQuery += strAmendedQuery;
                if (!MainPage._bTaxStatus)
                    strGSTNo = txtGSTNo.Text;

                strQuery += "UPDATE  [dbo].[PurchaseReturn]  SET  [Transport]='"+txtTransportName.Text+"',[WaybillNo]='"+txtWayBillNo.Text+"',[WayBillDate]='"+txtWayBillDate.Text+"',[AttachedBill]='" + txtAttachBill.Text+"', [Date]='" + strDate + "',[PurchasePartyID]='" + strPurchasePartyID + "',[EntryType]='" + GetEntryType() + "',[PurchaseType]='" + txtPurchaseType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSignAmt.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[ReverseCharge]='" + txtPurchaseInvoice.Text + "',[OtherText]='" + strQtrStatus + "',[OtherValue]='" + txtOtherPerSign.Text + txtOtherPer.Text + "',[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[TaxFree]=" + dba.ConvertObjectToDouble(txtTaxFree.Text) +",[Other2]='" + strGSTNo + "', [PartPurReturnAmt]=" + dPurReturnAmt+", "
                     + " [RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text)+ ", [NetDiscount]=" + dba.ConvertObjectToDouble(txtInsuranceAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[PurchaseBillCode]='" + txtPBillCode.Text + "',[PurchaseBillNo]='" + txtPurchaseBillNo.Text + "',[PurchaseBillDate]='" + strSDate + "',[Description3]='" + txtImportData.Text + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                     + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strPurchaseParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strPurchasePartyID + "' Where [AccountStatus]='PURCHASE RETURN' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                     + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                     + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                     + " Delete from StockMaster Where BillType='PURCHASERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strID = Convert.ToString(row.Cells["id"].Value);
                    strSRBillNo = Convert.ToString(row.Cells["saleReturnNo"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([RemoteID],[BillCode],[BillNo],[SRBillNo],[SalePartyID],[BarCode],[BrandName],[DesignName],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[Packing],[TotalAmt],[UnitName],[InsertStatus],[UpdateStatus],[ItemStatus],[DisStatus],[Discount]) VALUES "
                                  + " (0,'" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strSRBillNo + "','','" + row.Cells["barcode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + "," + dRate + ","
                                  + " " + dAmt + ",0,0, " + dAmt + ",'" + row.Cells["unitName"].Value + "',1,0,'FRESH',0,0)";
                    }
                    else
                    {
                        strQuery += " UPDATE [dbo].[PurchaseReturnDetails] Set [SRBillNo]='" + strSRBillNo + "',[BarCode]='" + row.Cells["barcode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",[SDisPer]=" + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + ",[Rate]=" + dRate + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[Disc]=" + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + ",[Packing]=" + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ",[TotalAmt]= " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdateStatus]=1 Where [SID]=" + strID + " and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";
                    }

                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[BrandName],[DesignName],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                           + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + row.Cells["barcode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "') ";
                    }
                }


                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "" && MainPage._bTaxStatus)
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
                }

                object objValue = "";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strDeletedSID + ") ";
                    if (MainPage.strOnlineDataBaseName != "")
                    {
                        strDeletedSIDQuery = " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";
                        objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                    }
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                       + "('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0 && MainPage.strOnlineDataBaseName != "")
                {
                    if (Convert.ToString(objValue) != "" && strDeletedSIDQuery != "")
                    {
                        if (!Convert.ToBoolean(objValue))
                        {
                            DataBaseAccess.CreateDeleteQuery(strDeletedSIDQuery);
                        }
                    }
                }
                //    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                //    btnEdit.Text = "&Edit";
                //    updateStatus = true;
                //    strDeletedSIDQuery = strDeletedSID = "";
                //    if (saleStatus)
                //        this.Close();
                //    else
                //    {
                //        ClearAllText();
                //        BindRecordWithControl(txtBillNo.Text);
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
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
                        SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPurchaseType.Text = objSearch.strSelectedData;
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
                    bool Created = false;
                    SetSignatureInBill(true, false,ref Created);
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

        
        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0, dDisStatus = 0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if ((MainPage._bTaxStatus || MainPage._bCustomPurchase) && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dDisStatus = dba.ConvertObjectToDouble(txtOtherPer.Text);
                    if (txtOtherPerSign.Text == "-")
                        dDisStatus = dDisStatus * -1;
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

                            string strQuery = "", strSubQuery = "", strGRSNo = "";
                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;

                            double dRate = 0, dQty = 0, dAmt = 0, dBasicAmt = 0;//, dOAmt = 0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                                dAmt = dRate * dQty;
                                dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["netAmt"].Value);

                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 + " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
                                }
                            }


                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount," + dTaxPer + " as TaxRate ";
                            }
                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    dTaxAmt = dTTaxAmt;
                                    if (dOtherAmt == 0)
                                        dTaxPer = dMaxRate;
                                    //pnlTax.Visible = true;
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
                            //pnlTax.Visible = true;
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

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {

            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtPurchaseType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSupplierName.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSupplierName.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtSupplierName.Text || dOldNetAmt != Convert.ToDouble(lblNetAmt.Text))
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
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
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
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strQuery = "";

                            strQuery += " Update SRD SET SRD.PurchaseReturnStatus=0,SRD.PurchaseReturnNumber='' from PurchaseReturn PR inner join PurchaseReturnDetails PRD on PR.BillCode=PRD.BillCode and PR.BillNo=PRD.BillNo inner join SaleReturnDetails SRD on (SRD.BillCode+' '+CAST(SRD.BillNo as varchar))=PRD.SRBillNo and SRD.PurchasePartyID=PR.PurchasePartyID and PRD.ItemName=SRD.ItemName and PRD.Qty=SRD.Qty and PRD.Amount=SRD.Amount Where PRD.BillCode='" + txtBillCode.Text + "' and PRD.BillNo=" + txtBillNo.Text + " "
                                     + " Delete from [PurchaseReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                     + " Delete from [PurchaseReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('PURCHASE RETURN','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASERETURN' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " Delete from [dbo].[StockMaster] Where BillType='PURCHASERETURN' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('PURCHASERETURN','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            //if (rdoPurchase.Checked)
                            //    strQuery += " Update GoodsReceive Set SaleBill='PENDING' Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (Select SRBillNo from PurchaseReturnDetails Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ")  ";

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

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
        
        
        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSupplierName.Text);
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

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void AskForPrint()
        {
            try
            {
                bool Created = false;
                DialogResult _result = MessageBox.Show("Are you want to print Sale Bill ?", "Print Sale Service Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_result == DialogResult.Yes)
                    SetSignatureInBill(true, false,ref Created);
            }
            catch
            {
            }
        }

        private string SetSignatureInBill(bool _bPStatus, bool _createPDF,ref bool Created)
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
                    if (_browser.ShowDialog() == DialogResult.OK)
                    {
                        if (_browser.FileName != "")
                            strPath = _browser.FileName;
                        Created = true;
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

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strFileName);
                    if (_bstatus  && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
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
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;
            defS.Copies = (short)MainPage.iNCopyPurRtn;

            DataTable _dtGST = null, _dtSalesAmt = null; ;
            bool _bIGST = false;
            DataTable dt = dba.CreateDebitNoteRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "DEBIT NOTE");
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
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        }
                        else
                        {
                            if (_pstatus)
                            {
                                // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                                //if (strValue != "" && strValue != "0")
                                //{
                                   // int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(defS,defS.DefaultPageSettings, false);
                                //}
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                        }

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else
                    {
                        Reporting.DCNoteReport_CGST_Retail objOL_salebill = new Reporting.DCNoteReport_CGST_Retail();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        }
                        else
                        {
                            if (_pstatus)
                            {
                                // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                                //if (strValue != "" && strValue != "0")
                                //{
                                //    int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                                //}
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                        }
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
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        }
                        else
                        {
                            if (_pstatus)
                            {
                                // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                                //if (strValue != "" && strValue != "0")
                                //{
                                //    int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                                // }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                        }
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else
                    {
                        Reporting.DCNoteReport_IGST_Retail objOL_salebill = new Reporting.DCNoteReport_IGST_Retail();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        }
                        else
                        {
                            if (_pstatus)
                            {
                                // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                                //if (strValue != "" && strValue != "0")
                                //{
                                //    int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                                // }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                        }
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                }
            }
            return true;
        }
        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtSupplierName.Text)), strBalance = ".", strName = dba.GetSafePartyName(txtSupplierName.Text);
                    if (strMobileNo != "")
                    {
                        if (MainPage.strSendBalanceInSMS == "YES")
                        {
                            double dAmt = dba.GetPartyAmountFromQuery(txtSupplierName.Text);
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

        private void txtPurchaseBillNo_KeyDown(object sender, KeyEventArgs e)
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
                        if (txtSupplierName.Text != "")
                        {
                            string[] strFullName = txtSupplierName.Text.Split(' ');
                            if (strFullName.Length > 1)
                                strQuery = " Where PurchasePartyID ='" + strFullName[0].Trim() + "'  ";
                            //if (rdoPurchase.Checked)
                            //{
                            SearchData objSearch = new SearchData("PURCHASEBILLNOFORMPURCHASE_RETAIL", strQuery, "SEARCH PURCHASE BILL NO", e.KeyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                string[] strData = objSearch.strSelectedData.Split('|');
                                txtPurchaseBillNo.Text = strData[0];
                                if (strData.Length > 1)
                                {
                                    txtPurchaseBillDate.Text = strData[1];
                                }
                                BindAllPurchaseRecord();
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

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoManual.Checked)
                {
                    txtPBillCode.ReadOnly = txtPurchaseBillNo.ReadOnly =txtPurchaseBillDate.ReadOnly= false;
                    txtPurchaseBillNo.Text = txtPurchaseBillDate.Text = txtSupplierName.Text = txtPurchaseInvoice.Text = txtGSTNo.Text = txtPurchaseType.Text = "";
                    if (btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.Clear();
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                }
                else
                {
                    txtPBillCode.ReadOnly = txtPurchaseBillNo.ReadOnly = txtPurchaseBillDate.ReadOnly = true;
                    txtPBillCode.Text = strPurchaseBillCode;
                }
            }
            catch { }
        }

        private void rdoPurchase_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoPurchase.Checked)
            {
                GetPurchaseBillDetails();
            }
        }

        private void txtPurchaseBillDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, (txtPurchaseBillNo.Text != ""), false, false);

        }

        private void txtPurchaseBillDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e,0);
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoAll.Checked)
                {
                    BindAllPurchaseRecord();
                }
            }
            catch { }
        }

        private void BindAllPurchaseRecord()
        {
            if (btnAdd.Text == "&Save")
                GetPurchaseBillDetails();
            else if (btnEdit.Text == "&Update")
            {
                if (dgrdDetails.Rows.Count > 1)
                {
                    DialogResult _result = MessageBox.Show("Are you want remove below mentioned details ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result == DialogResult.Yes)
                        GetPurchaseBillDetails();
                }
                else
                    GetPurchaseBillDetails();
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
            if (txtImportData.Text != "" && btnAdd.Text == "&Save")
            {
                BindRecordWithControl_Import();
            }
        }

        private void GetPurchaseBillDetails()
        {
            string strQuery = "", strPurchasePartyID = "";
            if (txtPurchaseBillNo.Text != "")
            {
                dgrdDetails.Rows.Clear();
                if (txtSupplierName.Text != "")
                {
                    string[] strFullName = txtSupplierName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strPurchasePartyID = strFullName[0].Trim();
                }

                strQuery = " Select dbo.GetFullName(PB.PurchasePartyID)SupplierName,PB.PurchaseType,SM.GSTNo,PB.InvoiceNo,PB.Description Description_3,PBS.* from PurchaseBookSecondary PBS LEFT JOIN  PurchaseBook PB ON PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo LEFT JOIN SupplierMaster SM ON AreaCode+AccountNo = PB.PurchasePartyID Where PBS.BillCode='" + txtPBillCode.Text + "' and PBS.BillNo=" + dba.ConvertObjectToDouble(txtPurchaseBillNo.Text) + " Order by PBS.ID ";

                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    txtImportData.Text = Convert.ToString(dt.Rows[0]["Description_3"]);
                    txtPurchaseInvoice.Text = Convert.ToString(dt.Rows[0]["InvoiceNo"]);

                    txtSupplierName.Text = Convert.ToString(dt.Rows[0]["SupplierName"]);
                    txtPurchaseType.Text = Convert.ToString(dt.Rows[0]["PurchaseType"]);
                    txtGSTNo.Text = Convert.ToString(dt.Rows[0]["GSTNo"]);

                    int rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {                       
                        dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                        dgrdDetails.Rows[rowIndex].Cells["id"].Value = "";// row["ID"];
                        dgrdDetails.Rows[rowIndex].Cells["saleReturnNo"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["barcode"].Value = row["BarCode"];
                        dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                        dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
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
                        dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                        //dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Discount"];
                        //dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                       // dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                        rowIndex++;
                    }
                }

                CalculateAllAmount();
            }
        }

        private void txtInsurancePer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtInsurancePer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtOtherPer.Text == "")
                    txtOtherPer.Text = "0.00";
                CalculateAllAmount();
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

        private void txtPurchaseBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateAllSpace(sender, e);
        }

        private void txtRemark_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex > 11)
                    {
                        if (_objData != null)
                            _objData.Close();
                        if (_objSearch != null)
                            _objSearch.Close();
                    }
                }
                else
                {
                    if (_objSearch != null)
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
                        int count = SavePurchaseRecord();
                        if (count > 0)
                        {
                            if (MainPage.bHSNWisePurchase)
                                CalculateAllAmount();
                            count = UpdateRecord("");
                          
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
        private int SavePurchaseRecord()
        {
            _strAttachBillWithComma = "";
            int _count = 0, result = 0;
            string _strAttachBill = "";
            DataTable _dt = CreateSecondaryDataTable();
            bool _bInclude = true;
            if (txtPurchaseType.Text.Contains("EXCLUDE"))
                _bInclude = false;

            double dAllNetAmt = 0;
            string strPurchasePartyID = "", strPurchaseParty = "";
            string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtSupplierName.Text, "[^0-9.]", "");
            bool _bRegistered = false;
            if (strCustomer != "")
            {
                string[] _strFullName = txtSupplierName.Text.Split(' ');
                if (_strFullName.Length > 1)
                {
                    strPurchasePartyID = _strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                    _bRegistered = true;
                }
            }
            else
                strPurchasePartyID = strPurchaseParty = txtSupplierName.Text;

            if (_dt.Rows.Count > 0)
            {
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string strQuery = "", strCompanyCode = "", strHSNCode = "", strHSNQuery = "", strBrandName = "", strDesignName = "", strItemName = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "", strBarCode = "", strBarCode_S = "";
                double dAmt = 0, dGrossAmt = 0, dNetAmt = 0, dQty = 0, dTQty = 0, dRate = 0, _dDisPer = 0, dMRP = 0, dCashAmt = 0, dTaxFree = 0, dNetDiscount = 0;
                double dOtherAmt = 0;
                dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmt.Text);
                dTaxFree = dba.ConvertObjectToDouble(txtTaxFree.Text);
                dNetDiscount = dba.ConvertObjectToDouble(txtInsuranceAmt.Text);
                dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);

                DataTable _dtCompany = _dt.DefaultView.ToTable(true, "CompanyCode");
                foreach (DataRow row in _dtCompany.Rows)
                {
                    strCompanyCode = Convert.ToString(row["CompanyCode"]);

                    DataRow[] _rows = _dt.Select("CompanyCode='" + strCompanyCode + "'");
                    int _index = 1;
                    _dDisPer = dGrossAmt = dTQty = 0;
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
                            strBrandName = Convert.ToString(_dr["brandName"]);
                            strDesignName = Convert.ToString(_dr["styleName"]);
                            strVariant1 = Convert.ToString(_dr["variant1"]);
                            strVariant2 = Convert.ToString(_dr["variant2"]);
                            strVariant3 = Convert.ToString(_dr["variant3"]);
                            strVariant4 = Convert.ToString(_dr["variant4"]);
                            strVariant5 = Convert.ToString(_dr["variant5"]);
                        }

                        strQuery += strHSNQuery + " INSERT INTO [dbo].[PurchaseReturnDetails] ([BillCode],[BillNo],[RemoteID],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP]"
                                                +",[SDisPer],[Rate],[Amount],[Disc],[UnitName],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[DisStatus],[Discount],[Dhara],[Packing],[Freight],[TaxFree],[TotalAmt],[ItemStatus]) VALUES "
                                                + " (@BillCode,@BillNo,0," + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dMRP 
                                                + "," + _dDisPer + "," + dRate + ", " + dAmt + ",0,'" + _dr["unitName"] + "',1,0,'" + strBarCode + "','" + strBrandName + "','" + strDesignName + "','','','+',0,0,0,0,0,"+dAmt+",'FRESH')";
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('PURCHASERETURN',@BillCode,@BillNo, " + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + strBarCode + "','" + strBrandName + "','" + strDesignName + "','','') ";

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

                    if (strQuery != "")
                    {
                        string strBillNo = txtBillCode.Text + " " + txtBillNo.Text;
                        dAllNetAmt += Convert.ToDouble((dGrossAmt).ToString("0"));
                        result += _count = dba.SaveRecord_PurchaseReturnBook(strPurchasePartyID, strDate, txtPurchaseType.Text, txtPBillCode.Text, txtPurchaseBillNo.Text, txtPurchaseBillDate.Text, strQuery, dGrossAmt, dMaxPer, dTTaxAmt, GetEntryType(), dTQty, dNetAmt, _dtTax, strCompanyCode, strBillNo, _bInclude, dCashAmt, txtSignAmt.Text, dOtherAmt, txtOtherPerSign.Text+txtOtherPer.Text, txtGSTNo.Text,dTaxFree, dNetDiscount,txtRemark.Text, ref _strAttachBill);
                        if (_strAttachBill != "")
                            _strAttachBillWithComma += _strAttachBill + ",";

                        if (_count > 0)
                        {
                            DialogResult _result = MessageBox.Show("Are you want to print Purchase Return Bill ?", "Print Purchase Return Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                txtPurReturnAmt.Text = dAllNetAmt.ToString("N2", MainPage.indianCurancy);
            else
                txtPurReturnAmt.Text = "0.00";

            return result;
        }

        private DataTable CreateSecondaryDataTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("id", typeof(String));
            _dt.Columns.Add("srNo", typeof(String));
            _dt.Columns.Add("saleRNo", typeof(String));
            _dt.Columns.Add("brandName", typeof(String));
            _dt.Columns.Add("barCode", typeof(String));
            _dt.Columns.Add("itemName", typeof(String));
            _dt.Columns.Add("variant1", typeof(String));
            _dt.Columns.Add("variant2", typeof(String));
            _dt.Columns.Add("variant3", typeof(String));
            _dt.Columns.Add("variant4", typeof(String));
            _dt.Columns.Add("variant5", typeof(String));
            _dt.Columns.Add("styleName", typeof(String));
            _dt.Columns.Add("qty", typeof(String));
            _dt.Columns.Add("unitName", typeof(String));
            _dt.Columns.Add("mrp", typeof(String));
            _dt.Columns.Add("disPer", typeof(String));
            _dt.Columns.Add("rate", typeof(String));
            _dt.Columns.Add("amount", typeof(String));
            _dt.Columns.Add("disc", typeof(String));
            _dt.Columns.Add("oCharges", typeof(String));
            _dt.Columns.Add("netAmt", typeof(String));
            _dt.Columns.Add("oldBrand", typeof(String));
            _dt.Columns.Add("stockQty", typeof(String));
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

        private void txtGSTNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (MainPage._bTaxStatus)
                        {
                            string strHeaderName = "SEARCH SUNDRY CREDITOR GST NO";
                            string strSearch = "PURCHASEPARTYWITHGSTNO";
                            if (MainPage.strUserRole.Contains("ADMIN"))
                            {
                                strSearch = "PURCHASEPERSONALPARTY";
                                strHeaderName = "SEARCH SUNDRY CREDITOR";
                            }

                            SearchData objSearch = new SearchData(strSearch, strHeaderName, e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData, strGSTNo = "", strStateName = "";
                            if (strData != "")
                            {
                                bool _blackListed = false;
                                if (dba.CheckTransactionLockWithBlackListGSTNo(strData, ref _blackListed, ref strGSTNo, ref strStateName))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSupplierName.Text = "";
                                    txtGSTNo.Text = "";
                                }
                                else if (_blackListed)
                                {
                                    MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSupplierName.Text = "";
                                    txtGSTNo.Text = "";
                                }
                                else
                                {
                                    txtSupplierName.Text = strData;
                                    txtGSTNo.Text = strGSTNo;
                                  //  txtStateName.Text = strStateName;
                                    //GetPartyDhara();
                                }
                            }
                        }
                        else
                        {
                            string strPParty = "";
                            if (txtSupplierName.Text != "")
                            {
                                string[] str = txtSupplierName.Text.Split(' ');
                                if (str.Length > 1)
                                    strPParty = str[0];
                            }
                            string strCompanyCode = GetCompanyCode();

                            SearchDataOther objSearch = new SearchDataOther("SUPPLIERGSTNO", strPParty, "SEARCH SUPPLIER GST NO", e.KeyCode, false, strCompanyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                string[] str = objSearch.strSelectedData.Split('|');
                                if (str.Length > 0)
                                {
                                    txtGSTNo.Text = str[0];
                                    //txtStateName.Text = str[1];
                                }
                            }
                            else
                                txtGSTNo.Text = "";
                        }
                    }
                }
                e.Handled = true;
            }
            catch (Exception Ex)
            {
            }
        }

        private void btnWayBillNo_Click(object sender, EventArgs e)
        {
            btnWayBillNo.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "" && !txtTransportName.Text.Contains("BY HAND"))
                {
                    if (txtTransportName.Text != "")
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want generate JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";

                                var _success = dba.GenerateEWayBillJSON(strBillNo, "PURCHASERETURN");
                                if (_success)
                                {
                                    DialogResult _result = MessageBox.Show("Are you want to open eway bill site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                        System.Diagnostics.Process.Start("https://ewaybillgst.gov.in/BillGeneration/BulkUploadEwayBill.aspx");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Transport Name can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTransportName.Focus();
                    }
                }
            }
            catch { }
            btnWayBillNo.Enabled = true;
        }

        private void btnPrintWayBill_Click(object sender, EventArgs e)
        {
            btnPrintWayBill.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        if (txtWayBillNo.Text != "" && txtWayBillDate.Text != "")
                        {
                            if (txtWayBillDate.Text.Length == 19)
                            {
                                DataTable _dt = dba.CreatePurchaseReturnWayBillDataTable(txtBillCode.Text, txtBillNo.Text);
                                if (_dt.Rows.Count > 0)
                                {
                                    Reporting.WayBillReport objReport = new Reporting.WayBillReport();
                                    objReport.SetDataSource(_dt);

                                    if (MainPage._PrintWithDialog)
                                        dba.PrintWithDialog(objReport);
                                    else
                                    {
                                        Reporting.ShowReport objShow = new Reporting.ShowReport("WAY BILL PREVIEW");
                                        objShow.myPreview.ReportSource = objReport;
                                        objShow.myPreview.ShowPrintButton = true;
                                        objShow.myPreview.ShowExportButton = true;
                                        objShow.ShowDialog();
                                    }

                                    objReport.Close();
                                    objReport.Dispose();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please enter valid way bill date (dd/MM/yyyy hh:mm tt).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtWayBillDate.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Way bill no. and Way bill date can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtWayBillNo.Focus();
                        }
                    }
                }
            }
            catch (Exception ex) { }
            btnPrintWayBill.Enabled = true;
        }

        private void txtWayBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransportName.Text = objSearch.strSelectedData;
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

        private void txtWayBillDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtTransportName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransportName.Text = objSearch.strSelectedData;
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

        private void PurchaseReturn_Retail_FormClosing(object sender, FormClosingEventArgs e)
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

        private string GetCompanyCode()
        {
            string strBarCode = "",strCompanyCode="";
            string strCCode = "";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strBarCode = Convert.ToString(row.Cells["barCode"].Value);
                string[] str = strBarCode.Split('-');
                if (str.Length > 0)
                {
                    strCompanyCode = str[0];
                    if (strCompanyCode != "" && strCompanyCode != MainPage.strDataBaseFile)
                    {
                        strCCode = strCompanyCode;
                    }
                }
            }
            return strCCode;
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
                        if (txtPurchaseType.Text.Contains("EXCLUDE"))
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
                string[] strReport = { "TAX CALCULATION : Purchase Return Book", ex.Message };
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
                    this.BeginInvoke(new MethodInvoker(Close));
                }

            }
            catch
            {
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
                        string[] strParty = txtSupplierName.Text.Split(' ');
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
            if (MainPage.strHttpPath != "")
            {
                string _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text;
                if (!strBranchCode.Contains("-"))
                    strBranchCode = "18-19/" + strBranchCode;
                string strWhastappMessage = "", strMsgType = "", strMType = "";
                string strFilePath = MainPage.strHttpPath + "/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSupplierName.Text);
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
                        strMessage = "M/S : " + txtSupplierName.Text + " , we have created your debit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "M/S : " + txtSupplierName.Text + ", we have updated your debit note with bill no. : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
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
