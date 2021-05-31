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
    public partial class SaleBook_Retail_Custom : Form
    {
        DataBaseAccess dba;
        SendSMS objSMS;
        string strLastSerialNo = "", strOldPartyName = "", strOldLRNumber = "", strSaleBillType = "", strHoldBilLCode = "", strHoldBillNo = "", strAllDeletedID = "", _strAttachBillWithComma = "";
        bool qtyAdjustStatus = false;
        double dOldNetAmt = 0, dSalesPartyDiscount = 0, dOldCashAmt = 0, dOldSaleAmt = 0;
        DataTable NextYearDBNames = new DataTable();

        SearchCategory_Custom _objSearch;
        SearchData _objData;
        public SaleBook_Retail_Custom()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            GetStartupData(true);
            NextYearDBNames.Columns.Add("CurrentDB", typeof(string));
            NextYearDBNames.Columns.Add("NextDB", typeof(string));
            NextYearDBNames.Rows.Clear();
        }

        public SaleBook_Retail_Custom(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            txtBillCode.Text = strSerialCode;
            GetStartupData(false);
            BindRecordWithControl(strSerialNo);
            NextYearDBNames.Columns.Add("CurrentDB", typeof(string));
            NextYearDBNames.Columns.Add("NextDB", typeof(string));
            NextYearDBNames.Rows.Clear();
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select SBillCode,(Select ISNULL(MAX(BillNo),0) from SalesBook Where BillCode=SBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (txtBillCode.Text == "")
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["SBillCode"]);
                    strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                }

                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'   ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  and BillNo>" + txtBillNo.Text + " ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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
                chkPick.Checked = false;
                ClearAllText();
                strOldPartyName = "";
                string strQuery = " Select *,(SalePartyID+' '+SName) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') HParty,CONVERT(varchar,Date,103)BDate,CONVERT(varchar,ISNULL(PackingDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)PDate,CONVERT(varchar,ISNULL(LrDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)LDate,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SB.Date))) LockType,ISNULL(PAgent,'DIRECT') PAgent,NormalDhara from SalesBook SB OUTER APPLY (Select Top 1 SM.Name as SName,NormalDhara from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.SalePartyID)SM1  OUTER APPLY (Select Top 1 (Description_1+' '+Name)PAgent from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.Description_1)SM  Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  Select SBS.*,dbo.GetFullName(SBS.SalesMan) SalesManName,(Qty+(Select SUM(Qty)StockQty from (Select SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select -SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASERETURN','SALES','STOCKOUT') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select 1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and _IM.DisStatus=0  and ISNULL(_IS.Description,'')=ISNULL(SBS.BarCode,'') and _IM.ItemName=SBS.ItemName and _IS.Variant1=SBS.Variant1 and _IS.Variant2=SBS.Variant2)Stock))StockQty,(Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=SBS.ItemName)HSNCode from SalesBookSecondary SBS Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " order by SID "
                                + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                + " Select (BillCode+' '+CAST(BillNo as varchar))SaleBillNo,CONVERT(varchar,Date,103)Date,ISNULL(dbo.GetFullName(SalePartyID),'') SalePartyID from SalesBook WHere SaleBillType='HOLD' Order by BillNo desc "
                                + " Select * from dbo.[CardDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                DataSet ds = dba.GetDataSet(strQuery);
                DisableAllControls();
                txtReason.Text = strOldLRNumber = "";
                pnlDeletionConfirmation.Visible = false;
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = txtIRNo.Text = "";
                btnAdd.TabStop = true;
                btnHold.Enabled = false;

                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            DataRow row = dt.Rows[0];
                            txtBillNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["BDate"]);
                            txtSalesParty.Text = strOldPartyName = Convert.ToString(row["SParty"]);
                            txtSubParty.Text = Convert.ToString(row["HParty"]);
                            txtSalesType.Text = Convert.ToString(row["SalesType"]);
                            txtWayBIllDate.Text = Convert.ToString(row["WayBillDate"]);
                            txtWayBillNo.Text = Convert.ToString(row["WaybillNo"]);
                            txtNoofCases.Text = Convert.ToString(row["NoOfCase"]);
                            strOldLRNumber = txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            txtLRDate.Text = Convert.ToString(row["LDate"]);
                            txtTimeOfSupply.Text = Convert.ToString(row["LRTime"]);
                            txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                            txtPackerName.Text = Convert.ToString(row["PackerName"]);
                            txtPackingDate.Text = Convert.ToString(row["PDate"]);
                            txtCartonType.Text = Convert.ToString(row["CartonType"]);
                            txtSalesType.Text = Convert.ToString(row["SalesType"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtTransport.Text = Convert.ToString(row["TransportName"]);
                            txtBStation.Text = Convert.ToString(row["Station"]);
                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                            txtOtherPer.Text = Convert.ToString(row["DisPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DisAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);
                            txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                            txtPostage.Text = Convert.ToString(row["PostageAmt"]);
                            txtGreenTax.Text = Convert.ToString(row["GreenTax"]);
                            txtOtherPerSign.Text = Convert.ToString(row["Description"]);
                            txtPetiType.Text = Convert.ToString(row["Description_2"]);
                            txtImportData.Text = Convert.ToString(row["Description_3"]);
                            txtMarketer.Text = Convert.ToString(row["PAgent"]);
                            txtAttachBill.Text = Convert.ToString(row["AttachedBill"]);
                            if (txtMarketer.Text == "")
                                txtMarketer.Text = Convert.ToString(row["Description_1"]);

                            if (dt.Columns.Contains("IntStatus") && Convert.ToString(row["IntStatus"]) != "")
                                chkStatus.Checked = Convert.ToBoolean(row["IntStatus"]);
                            if (dt.Columns.Contains("IRNNO"))
                                txtIRNo.Text = Convert.ToString(row["IRNNO"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                txtTaxableAmt.Text = dba.ConvertObjectToDouble(Convert.ToString(row["TaxableAmt"])).ToString("N2", MainPage.indianCurancy);
                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            txtCardAmt.Text = ConvertObjectToDouble(row["CardAmt"]).ToString("N2", MainPage.indianCurancy);
                            dSalesPartyDiscount = dba.ConvertObjectToDouble(row["NormalDhara"]);
                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);
                            dOldSaleAmt = ConvertObjectToDouble(row["PartSaleAmt"]);
                            lblTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            dOldCashAmt = ConvertObjectToDouble(row["CashAmt"]);
                            txtCashAmt.Text = dOldCashAmt.ToString("N2", MainPage.indianCurancy);
                            txtSaleAmt.Text = dOldSaleAmt.ToString("N2", MainPage.indianCurancy);
                            txtNetAmt.Text = ConvertObjectToDouble(row["CreditAmt"]).ToString("N2", MainPage.indianCurancy);

                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);
                            txtGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtFinalAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                            strSaleBillType = Convert.ToString(row["SaleBillType"]);
                            if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                                btnEdit.Enabled = btnDelete.Enabled = false;
                            else
                            {
                                if (!MainPage.mymainObject.bSaleEdit)
                                    btnEdit.Enabled = btnDelete.Enabled = false;
                                else
                                    btnEdit.Enabled = btnDelete.Enabled = true;

                                if (strSaleBillType == "HOLD")
                                    btnEdit.Enabled = false;
                            }

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                            GetLedgerBalance();

                            if (txtLRDate.Text.Length == 10)
                            {
                                DateTime lDate = dba.ConvertDateInExactFormat(txtLRDate.Text);
                                if (lDate < Convert.ToDateTime("01/01/2010"))
                                    txtLRDate.Text = lDate.ToString("dd/MM/yyyy");
                            }
                            else
                                txtLRDate.Text = MainPage.strCurrentDate;

                        }
                    }

                    BindSalesBookDetails(ds.Tables[1]);
                    BindGSTDetailsWithControl(ds.Tables[2]);
                    BindHoldDetailsWithControl(ds.Tables[3]);
                    BindCardDetailsWithControl(ds.Tables[4]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnEdit.Enabled = false;
            }
        }
        private void BindCardDetailsWithControl(DataTable dt)
        {
            int rowIndex = 0;
            dgrdCardDetail.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdCardDetail.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdCardDetail.Rows[rowIndex].Cells["cSNo"].Value = (rowIndex + 1) + ".";
                    dgrdCardDetail.Rows[rowIndex].Cells["bank"].Value = row["BankName"];
                    dgrdCardDetail.Rows[rowIndex].Cells["cCardType"].Value = row["CardType"];
                    dgrdCardDetail.Rows[rowIndex].Cells["cCardNo"].Value = row["CardNo"];
                    dgrdCardDetail.Rows[rowIndex].Cells["cExpiryDate"].Value = row["ExpiryDate"];
                    dgrdCardDetail.Rows[rowIndex].Cells["cAmt"].Value = row["CardAmount"];
                    rowIndex++;
                }
            }
        }

        private void BindSalesBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            double dTBox = 0, dBox = 0;
            if (dt.Rows.Count > 0)
            {
                txtSalesMan.Text = Convert.ToString(dt.Rows[0]["SalesManName"]);
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
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
                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                    dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = row["StockQty"];
                    dgrdDetails.Rows[rowIndex].Cells["hsnCode"].Value = row["HSNCode"];

                    dTBox += dBox = ConvertObjectToDouble(row["Other2"]);
                    dgrdDetails.Rows[rowIndex].Cells["description"].Value = row["Other1"];
                    dgrdDetails.Rows[rowIndex].Cells["boxRoll"].Value = dBox;
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];

                    rowIndex++;
                }
            }

            lblTotalBox.Text = dTBox.ToString("N2", MainPage.indianCurancy);
        }

        private void BindHoldDetailsWithControl(DataTable dt)
        {
            int rowIndex = 0;
            dgrdHold.Rows.Clear();
            btnHoldList.BackColor = Color.FromArgb(185, 30, 12);
            if (dt.Rows.Count > 0)
            {
                dgrdHold.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdHold.Rows[rowIndex].Cells["hSNo"].Value = (rowIndex + 1);
                    dgrdHold.Rows[rowIndex].Cells["hBillNo"].Value = row["SaleBillNo"];
                    dgrdHold.Rows[rowIndex].Cells["hDate"].Value = row["Date"];
                    dgrdHold.Rows[rowIndex].Cells["hPartyName"].Value = row["SalePartyID"];
                    rowIndex++;
                }
                btnHoldList.BackColor = Color.DarkGreen;
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
            //else
            //    pnlTax.Visible = false;
        }

        private bool CheckSONumberInDetails()
        {
            if (dgrdDetails.Rows.Count > 0)
            {

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["soNumber"].Value) != "")
                    {
                        MessageBox.Show("Sorry ! You can't update party name because few sales order added", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (MainPage.strUserRole.Contains("ADMIN"))
                            return true;
                        else
                            return false;
                    }
                }
            }
            return true;
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (CheckSONumberInDetails())
                        {
                            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                txtSalesParty.Text = objSearch.strSelectedData;
                                string strData = objSearch.strSelectedData;
                                if (strData != "")
                                {
                                    txtSalesParty.Text = strData;
                                    txtSubParty.Text = "SELF";
                                    GetSalesPartyRecord();
                                }
                                GetLedgerBalance();
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

        private void GetLedgerBalance()
        {
            try
            {
                if (txtSalesParty.Text != "")
                {
                    double dAmt = dba.GetPartyAmountFromQuery(txtSalesParty.Text);
                    if (dAmt > 0)
                        lblLedgerBal.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else if (dAmt < 0)
                        lblLedgerBal.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                    else
                        lblLedgerBal.Text = dAmt.ToString("0.00");
                }
                else
                    lblLedgerBal.Text = "0.00";
            }
            catch { }
        }

        private DataSet GetPendingRecordDataSet()
        {
            DataSet ds = null;
            string strSaleParty = "", strSubParty = "";
            string[] strFullName = txtSalesParty.Text.Split(' ');
            if (strFullName.Length > 1)
                strSaleParty = strFullName[0].Trim();
            strFullName = txtSubParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strSubParty = strFullName[0].Trim();

            if (strSaleParty != "" && strSubParty != "")
            {
                string strQuery = "";

                if (strSubParty == "SELF")
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";
                else
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSubParty + "' ";

                strQuery += " Select TransactionLock,GroupII,BlackList,Reference,(CASE When DueDays!='' then DueDays else (Select TOP 1 GraceDays from CompanySetting) end) DueDays,(CASE When Postage!='' then Postage else (Select TOP 1 Postage from CompanySetting) end) Postage,Category,NormalDhara,SNDhara as SUPERDhara from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";

                ds = DataBaseAccess.GetDataSetRecord(strQuery);
            }
            return ds;
        }

        private void GetSalesPartyRecord()
        {
            txtTransport.Text = txtPvtMarka.Text = txtBStation.Text = "";
            bool tStatus = true;
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                DataSet ds = GetPendingRecordDataSet();
                if (ds != null)
                {
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[1];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            txtPostage.Text = Convert.ToString(dt.Rows[0]["Postage"]);
                            dSalesPartyDiscount = dba.ConvertObjectToDouble(dt.Rows[0]["NormalDhara"]);

                            if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                tStatus = false;
                            }
                            else if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                            {
                                MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Text = "";
                                txtSubParty.Text = "";
                                tStatus = false;
                            }
                            else
                            {
                                //if (Convert.ToString(dt.Rows[0]["Category"]) == "CASH PARTY")
                                //    pnlCash.Visible = true;
                                //else
                                //    pnlCash.Visible = false;
                            }
                        }
                        if (tStatus)
                        {
                            dt.Clear();
                            dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                if (row != null)
                                {
                                    txtTransport.Text = Convert.ToString(row["Transport"]);
                                    txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                                    txtBStation.Text = Convert.ToString(row["BookingStation"]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SaleBook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlHold.Visible)
                    pnlHold.Visible = false;
                else if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else if (pnlCard.Visible)
                    pnlCard.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused && !dgrdCardDetail.Focused)
                SelectNextControl(ActiveControl, true, true, true, true);
            else
            {
                if (e.KeyCode == Keys.F2 || e.KeyCode == Keys.F5)
                {
                    btnAdd.PerformClick();
                }
                else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Z)
                {
                    txtCardAmt.Focus();
                }
                else if (e.KeyCode == Keys.F6)
                {
                    btnEdit.PerformClick();
                }
                else if (e.KeyCode == Keys.F8)
                {
                    btnDelete.PerformClick();
                }
                else if (e.KeyCode == Keys.F7)
                {
                    btnSearch.PerformClick();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    btnPreview.PerformClick();
                }
                else if (e.KeyCode == Keys.F4)
                {
                    btnPrint.PerformClick();
                }
                else if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && MainPage.mymainObject.bSaleView)
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

        private void EnableAllControls()
        {
            txtIRNo.ReadOnly = txtSpclDisPer.ReadOnly = txtCardAmt.ReadOnly = txtDiscAmt.ReadOnly = txtOtherPerSign.ReadOnly = txtPackingDate.ReadOnly = txtPvtMarka.ReadOnly = txtWayBillNo.ReadOnly = txtWayBIllDate.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtNoofCases.ReadOnly = txtTimeOfSupply.ReadOnly = txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtOtherPer.ReadOnly = txtPostage.ReadOnly = txtGreenTax.ReadOnly = txtTaxPer.ReadOnly = txtCashAmt.ReadOnly = false;
            dgrdDetails.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtIRNo.ReadOnly = txtSpclDisPer.ReadOnly = txtCardAmt.ReadOnly = txtDiscAmt.ReadOnly = txtOtherPerSign.ReadOnly = txtPackingDate.ReadOnly = txtPvtMarka.ReadOnly = txtWayBillNo.ReadOnly = txtWayBIllDate.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtNoofCases.ReadOnly = txtTimeOfSupply.ReadOnly = txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtOtherPer.ReadOnly = txtTaxPer.ReadOnly = txtPostage.ReadOnly = txtGreenTax.ReadOnly = txtTaxPer.ReadOnly = txtCashAmt.ReadOnly = true;
            dgrdDetails.ReadOnly = true;
            btnAdd.TabStop = true;
            lblMsg.Text = lblCreatedBy.Text = "";
        }

        private void ClearAllText()
        {
            txtIRNo.Text = txtAttachBill.Text = txtBStation.Text = strHoldBilLCode = strHoldBillNo = strSaleBillType = txtImportData.Text = txtPetiType.Text = txtMarketer.Text = txtSalesMan.Text = txtPackerName.Text = txtCartonType.Text = lblCreatedBy.Text = txtPvtMarka.Text = txtWayBillNo.Text = txtWayBIllDate.Text = txtLRNumber.Text = txtNoofCases.Text = txtTimeOfSupply.Text = txtSalesParty.Text = txtSalesType.Text = txtSubParty.Text = txtSalesType.Text = txtRemark.Text = txtTransport.Text = "";
            txtCardAmt.Text = txtTaxableAmt.Text = txtOtherPer.Text = txtGreenTax.Text = txtSpclDisPer.Text = txtSplDisAmt.Text = txtRoundOff.Text = txtOtherAmount.Text = txtPackingAmt.Text = txtDiscAmt.Text = txtTaxAmt.Text = lblTotalQty.Text = lblTotalBox.Text = txtGrossAmt.Text = txtFinalAmt.Text = txtNetAmt.Text = txtCashAmt.Text = txtSaleAmt.Text = "0.00";
            lblLedgerBal.Text = "0.00";
            if (MainPage._bTaxStatus)
                txtTaxPer.Text = "18.00";
            else
                txtTaxPer.Text = "0.00";

            txtSign.Text = txtROSign.Text = txtOtherPerSign.Text = "+";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdCardDetail.Rows.Add();
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            chkStatus.Checked = qtyAdjustStatus = chkPick.Checked = pnlTax.Visible = pnlDeletionConfirmation.Visible = false;
            strOldLRNumber = strAllDeletedID = "";
            dOldCashAmt = dOldNetAmt = dOldSaleAmt = 0;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtLRDate.Text = txtPackingDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtLRDate.Text = txtPackingDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

        }

        private void SetSerialNo()
        {
            //"Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='INTERSTATE' and SaleType='SALES' and TaxIncluded=0) TaxName  from SalesRecord Where BillCode='" + txtBillCode.Text + "')Sales "
            string strQuery = "Declare @BillCode nvarchar(250); Select @BillCode=SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select  @BillCode as SBillCode, (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from ( Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode=@BillCode )SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='INTERSTATE' and SaleType='SALES' and TaxIncluded=0) TaxName  from SalesRecord SR Where SR.BillCode=@BillCode )Sales ";
            DataTable table = dba.GetDataTable(strQuery);
            if (table.Rows.Count > 0)
            {
                //double billNo = dba.ConvertObjectToDouble(table.Rows[0][0]), maxBillNo = dba.ConvertObjectToDouble(table.Rows[0][1]),dSerialNo=Convert(;
                //if (billNo > maxBillNo)
                //    txtBillNo.Text = Convert.ToString(billNo);
                //else
                txtBillCode.Text = Convert.ToString(table.Rows[0]["SBillCode"]);
                txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                if (MainPage._bTaxStatus)
                    txtSalesType.Text = Convert.ToString(table.Rows[0]["TaxName"]);
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
                MessageBox.Show("Sorry ! SUNDRY DEBTORS Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }

            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;
            if (MainPage._bTaxStatus)
            {
                if (!dba.GetBillNextPrevRecord("SALES", txtBillCode.Text, txtBillNo.Text, txtDate))
                    return false;
            }

            CalculateAllAmount();
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
                return false;
            }

            double dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
            if (btnAdd.Text == "&Save(F5)" || (dOldNetAmt != dNetAmt || strOldPartyName != txtSalesParty.Text))
            {
                bool __bStatus = ValidateAmountLimit(dNetAmt);
                if (!__bStatus)
                    return __bStatus;
            }
            return ValidateStock();
        }

        private bool ValidateAmountLimit(double dNetAmt)
        {
            object objLimit = DataBaseAccess.ExecuteMyScalar("Select AmountLimit from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dba.ConvertObjectToDouble(objLimit) > 0)
            {
                string strQuery = "";
                if (btnEdit.Text == "&Update(F6)")
                    strQuery = " +(Select ISNULL(SUM(CAST(NetAmt as Money)),0) Amt from SalesBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) ";

                double dAmt = dba.CheckAmountLimitValidation(txtSalesParty.Text, strQuery);
                if (dAmt != -1)
                {
                    dAmt -= dNetAmt;
                    if (dAmt < 0 && MainPage.mymainObject.bCreditLimitmanagement)
                    {
                        MessageBox.Show("Sorry ! Amount limit has been exceeded, Please extend amount limit of : " + Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
                else if (MainPage.strUserRole.Contains("ADMIN"))
                    return true;
                else
                {
                    MessageBox.Show("Unable to check balance amount from internet, please connect internet !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }


        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)") && (Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 15 || e.ColumnIndex == 19)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        string strItem = Convert.ToString(dgrdDetails.CurrentRow.Cells["itemName"].Value);
                        if (strItem == "")
                        {
                            _objData = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                            _objData.ShowDialog();
                            dgrdDetails.CurrentCell.Value = dgrdDetails.CurrentRow.Cells["oldbrandName"].Value = _objData.strSelectedData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11)
                    {
                        string strBrandName = Convert.ToString(dgrdDetails.CurrentRow.Cells["oldbrandName"].Value), strFrom = e.ColumnIndex == 3 ? "BarCode" : "ItemName";
                        //if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                        //{
                        _objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_RETAIL", strBrandName, "", "", "", "", "", "", Keys.Space, false, false, strFrom);
                        _objSearch.ShowDialog();
                        if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                            GetAllDesignSizeColorWithBarCode_Unique(_objSearch, dgrdDetails.CurrentRow.Index);
                        else
                            GetAllDesignSizeColorWithBarCode(_objSearch, dgrdDetails.CurrentRow.Index);

                        //}
                        //else
                        //{
                        //    _objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_SALEMERGE", strBrandName, "", "", "", "", "", "", Keys.Space, false, false, strFrom);
                        //    _objSearch.ShowDialog();
                        //    GetAllDesignSizeColorWithBarCode(_objSearch, dgrdDetails.CurrentRow.Index);
                        //}
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 17 || e.ColumnIndex == 18)
                    {
                        if (dSalesPartyDiscount != 0)
                        {
                            MessageBox.Show("Sorry ! Discount Managed by account master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                                e.Cancel = true;
                        }
                    }
                    else if (e.ColumnIndex == 16)
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

        private void GetAllSONumberDesignSizeColor(SearchCategory objCategory, int rowIndex)
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
                                string[] strAllItem = strItem.Split('|');
                                if (strAllItem.Length > 1)
                                {
                                    if (firstRow)
                                        dgrdDetails.Rows.Add();
                                    else
                                        firstRow = true;
                                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                    if (strAllItem.Length > 1)
                                        dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = strAllItem[1];

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) == "")
                                    {
                                        if (strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2];
                                        if (strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3];
                                        if (strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4];
                                    }
                                    if (strAllItem.Length > 1)
                                    {
                                        GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                    }

                                    rowIndex++;
                                    if (strAllItem[0].Contains(objCategory.txtSearch.Text.Trim()))
                                        break;

                                }
                            }
                            rowIndex--;
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                if (strAllItem.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = strAllItem[1];

                                if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) == "")
                                {
                                    if (strAllItem.Length > 2)
                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2];
                                    if (strAllItem.Length > 3)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3];
                                    if (strAllItem.Length > 4)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4];
                                }

                                if (strAllItem.Length > 1)
                                {
                                    GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                }
                            }
                        }
                        ArrangeSerialNo();
                    }

                    if (dgrdDetails.Rows.Count > 0)
                    {
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells["description"];
                        dgrdDetails.Focus();
                    }
                }

                CalculateAllAmount();
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
                                string[] strAllItem = strItem.Split('|');
                                if (strItem != "ADD NEW DESIGNNAME NAME")
                                {
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];

                                        if (strAllItem.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                        if (strAllItem.Length > 6)
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[6];
                                        if (strAllItem.Length > 7)
                                            dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[7];
                                        if (strAllItem.Length > 8)
                                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[8];
                                        if (strAllItem.Length > 9)
                                            dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[9];



                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "" && (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                            GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                        else
                                        {
                                            double dQty = 0, dRate = 0, dAmt = 0;
                                            if (strAllItem.Length > 6)
                                                dQty = ConvertObjectToDouble(strAllItem[6]);
                                            if (strAllItem.Length > 7)
                                                dRate = ConvertObjectToDouble(strAllItem[9]);
                                            dAmt = dQty * dRate;
                                            dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                                        }
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
                                if (strAllItem.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                if (strAllItem.Length > 2)
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                if (strAllItem.Length > 3)
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                if (strAllItem.Length > 4)
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                if (strAllItem.Length > 5)
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                if (strAllItem.Length > 6)
                                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[6];
                                if (strAllItem.Length > 7)
                                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[7];
                                if (strAllItem.Length > 8)
                                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[8];
                                if (strAllItem.Length > 9)
                                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[9];

                                if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                                    GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                else
                                {
                                    double dQty = 0, dRate = 0, dAmt = 0;
                                    if (strAllItem.Length > 6)
                                        dQty = ConvertObjectToDouble(strAllItem[6]);
                                    if (strAllItem.Length > 7)
                                        dRate = ConvertObjectToDouble(strAllItem[9]);
                                    dAmt = dQty * dRate;
                                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }

                                SetUnitName(strAllItem[0], rowIndex);
                            }
                        }

                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (dgrdDetails.Rows.Count > 0)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells["description"];
                            dgrdDetails.Focus();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void GetAllDesignSizeColorWithBarCode_Unique(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {
                if (objCategory != null)
                {
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData != "ADD NEW DESIGNNAMEWITHBARCODE NAME")
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                string strOldBarCode = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["barCode"].Value), strBarcode = "";

                                strBarcode = strAllItem[0].Trim();
                                if (CheckBarCodeDuplicate(strBarcode, rowIndex))
                                {
                                    string[] str = strBarcode.Split('.');
                                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = str[0];
                                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2].Trim();

                                    if (strAllItem.Length > 5)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3].Trim();
                                    if (strAllItem.Length > 6)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4].Trim();
                                    if (strAllItem.Length > 7)
                                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[5].Trim();
                                    if (strAllItem.Length > 8)
                                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[6].Trim();
                                    if (strAllItem.Length > 9)
                                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[7].Trim();

                                    dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = strAllItem[strAllItem.Length - 1];

                                    if (str.Length > 1)
                                        dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strBarcode;

                                    GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                    if (rowIndex == dgrdDetails.Rows.Count - 1)
                                    {
                                        dgrdDetails.Rows.Add(1);
                                        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
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
            }
            catch
            {
            }
        }

        private bool CheckBarCodeDuplicate(string strBarCode, int _index)
        {
            int _rowIndex = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (_rowIndex != _index)
                {
                    if (Convert.ToString(row.Cells["barcode_s"].Value) == strBarCode)
                    {
                        if (MainPage.strBarCodingType != "UNIQUE_BARCODE")
                        {
                            row.Cells["qty"].Value = ConvertObjectToDouble(row.Cells["qty"].Value) + 1;
                            CalculateAllAmount();
                        }
                        return false;
                    }
                }
                _rowIndex++;
            }
            return true;
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
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0].Trim();
                                dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                                //dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[2];
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2].Trim();

                                if (strAllItem.Length > 5)
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3].Trim();
                                if (strAllItem.Length > 6)
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4].Trim();
                                if (strAllItem.Length > 7)
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[5].Trim();
                                if (strAllItem.Length > 8)
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[6].Trim();
                                if (strAllItem.Length > 9)
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[7].Trim();

                                dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = strAllItem[strAllItem.Length - 1];
                                GetSaleRate(dgrdDetails.Rows[rowIndex]);
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (dgrdDetails.Rows.Count > 0)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells["description"];
                            dgrdDetails.Focus();
                        }
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
                DataTable table = dba.GetDataTable("Select ISNULL(QtyRatio,1) QtyRatio,UnitName as PurchaseUnit,StockUnitName UnitName from Items Where ItemName='" + strDesignName + "' ");
                if (table.Rows.Count > 0)
                {
                    // dgrdDetails.Rows[rowIndex].Cells["qtyRatio"].Value = table.Rows[0]["QtyRatio"];
                    //dgrdDetails.Rows[rowIndex].Cells["purchaseUnit"].Value = table.Rows[0]["PurchaseUnit"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void GetSaleRate(DataGridViewRow row)
        {
            try
            {
                string strBarCode = Convert.ToString(row.Cells["barCode"].Value), strBrandName = Convert.ToString(row.Cells["brandName"].Value), strItemName = Convert.ToString(row.Cells["itemName"].Value), strVariant1 = Convert.ToString(row.Cells["variant1"].Value), strVariant2 = Convert.ToString(row.Cells["variant2"].Value);

                string strQuery = "";
                strQuery = " Select BrandName,DesignName,SaleMRP,SaleDis as DisPer, SaleRate,UnitName,(Select QtyRatio from Items  WHere ItemName='" + strItemName + "')QRatio from(Select Top 1 BrandName,DesignName,SaleMRP,SaleDis, SaleRate,UnitName,0 ID from PurchaseBookSecondary PBS Where PBS.BarCode='" + strBarCode + "' and PBS.BrandName='" + strBrandName + "' and PBS.ItemName='" + strItemName + "' and PBS.Variant1='" + strVariant1 + "' and PBS.Variant2='" + strVariant2 + "' Order by BillNo desc UNION ALL  "
                          + " Select Top 1 BrandName,BuyerDesignName as DesignName,SaleRate as SaleMRP,0 as SaleDis,SaleRate,UnitName,1 ID from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _IS.Description='" + strBarCode + "' and _Im.BrandName='" + strBrandName + "' and _Im.ItemName='" + strItemName + "' and _IS.Variant1='" + strVariant1 + "' and _IS.Variant2='" + strVariant2 + "' and ActiveStatus=1 UNION ALL Select Top 1 PBS.BrandName,DesignName,SaleRate as SaleMRP,0 as SaleDis, SaleRate,UnitName,2 ID from ItemStock PBS inner join Items _Im on PBS.ItemName=_im.ItemName Where PBS.BarCode='" + strBarCode + "' and PBS.BrandName='" + strBrandName + "' and PBS.ItemName='" + strItemName + "' and PBS.Variant1='" + strVariant1 + "' and PBS.Variant2='" + strVariant2 + "')Sale Order by ID ";

                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];
                    double dMRP, dRate = 0, dQty = 0;
                    row.Cells["mrp"].Value = dMRP = dba.ConvertObjectToDouble(_row["SaleMRP"]);
                    if (dSalesPartyDiscount == 0)
                    {
                        row.Cells["disPer"].Value = "-" + _row["DisPer"];
                        row.Cells["rate"].Value = dRate = ConvertObjectToDouble(_row["SaleRate"]);
                    }
                    else
                    {

                        row.Cells["disPer"].Value = dSalesPartyDiscount * -1;
                        row.Cells["rate"].Value = dRate = (dMRP * dSalesPartyDiscount) / 100.00;
                    }

                    // if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                    row.Cells["qty"].Value = dQty = ConvertObjectToDouble(_row["QRatio"]);

                    row.Cells["styleName"].Value = _row["DesignName"];
                    row.Cells["unitName"].Value = _row["UnitName"];
                    if (dQty != 0 && dRate != 0)
                        row.Cells["amount"].Value = (dQty * dRate).ToString("N2", MainPage.indianCurancy);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (e.ColumnIndex == 13)
                    {
                        CalculateTotalBox();
                        if (Convert.ToString(dgrdDetails.CurrentRow.Cells["qty"].Value) == "")
                        {
                            dgrdDetails.CurrentRow.Cells["qty"].Value = dgrdDetails.CurrentRow.Cells["boxRoll"].Value;
                            CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                        }
                    }
                    else if (e.ColumnIndex == 14)
                    {
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    }
                    else if (e.ColumnIndex == 16 || e.ColumnIndex == 17)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 18)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 19)
                        CalculateRateWithQtyAmount(dgrdDetails.Rows[e.RowIndex]);
                }
            }
            catch
            {
            }
        }

        private void CalculateTotalBox()
        {
            double dTBox = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
                dTBox += ConvertObjectToDouble(row.Cells["boxRoll"].Value);
            lblTotalBox.Text = dTBox.ToString("N2", MainPage.indianCurancy);
        }

        private void CalculateRateWithQtyAmount(DataGridViewRow rows)
        {
            double dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            if (dAmount != 0 && dQty != 0)
                dRate = dAmount / dQty;
            rows.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
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
                        if (IndexColmn < dgrdDetails.ColumnCount - 6)
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

                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["barCode"].Value) != "" && IndexColmn == 11)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
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
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
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
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
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
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);//,strSONumber = Convert.ToString(dgrdDetails.CurrentRow.Cells["soNumber"].Value);
                        if (strID != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (strAllDeletedID != "")
                                    strAllDeletedID += ",";
                                strAllDeletedID += strID;
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
                        if (colIndex == 3 || colIndex == 4)
                            dgrdDetails.CurrentCell.Value = "";

                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                    {
                        DataGridViewRow row = dgrdDetails.CurrentRow;
                        dgrdDetails.Rows.Add();
                        DataGridViewRow _row = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1];

                        _row.Cells["srNo"].Value = dgrdDetails.Rows.Count;
                        _row.Cells["barCode"].Value = row.Cells["barCode"].Value;
                        _row.Cells["itemName"].Value = row.Cells["itemName"].Value;
                        _row.Cells["variant1"].Value = row.Cells["variant1"].Value;
                        _row.Cells["variant2"].Value = row.Cells["variant2"].Value;
                        _row.Cells["variant3"].Value = row.Cells["variant3"].Value;
                        _row.Cells["variant4"].Value = row.Cells["variant4"].Value;
                        _row.Cells["variant5"].Value = row.Cells["variant5"].Value;
                        _row.Cells["qty"].Value = row.Cells["qty"].Value;
                        _row.Cells["disPer"].Value = row.Cells["disPer"].Value;
                        _row.Cells["mrp"].Value = row.Cells["mrp"].Value;
                        _row.Cells["rate"].Value = row.Cells["rate"].Value;
                        _row.Cells["amount"].Value = row.Cells["amount"].Value;
                        _row.Cells["unitName"].Value = row.Cells["unitName"].Value;
                        _row.Cells["stockQty"].Value = row.Cells["stockQty"].Value;

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
                if (columnIndex == 15)
                {
                    Char pressedKey = e.KeyChar;
                    if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                        e.Handled = false;
                    else
                        dba.KeyHandlerPoint(sender, e, 2);
                }
                else if (columnIndex == 12)
                {
                    dba.ValidateSpace(sender, e);
                }
                else if (columnIndex > 11)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
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
                    string strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and [SID]=" + strID + " "
                                    + " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.[SID], SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text + " and SBS.[SID]=" + strID + " ";
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
                            strQuery = " Delete from SalesBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " "
                                    + " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.[SID], SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text + " and SBS.[SID]=" + strID + " ";
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
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
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dSQty = ConvertObjectToDouble(dgrdDetails.CurrentRow.Cells["stockQty"].Value), dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            if (dQty > dSQty)
            {
                lblMsg.Text = "Total stock qty : " + dSQty.ToString() + ", You can't sale more than that.";
                lblMsg.ForeColor = Color.Red;
                if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                {
                    rows.Cells["qty"].Value = dSQty;
                    dQty = dSQty;
                }
            }
            else
                lblMsg.Text = "";

            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
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
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value); //, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                //rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

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
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                //rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        //private void CalculateAmountWithDiscOtherChargese(DataGridViewRow rows)
        //{
        //    double dAmt = ConvertObjectToDouble(rows.Cells["amount"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
        //    rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
        //    CalculateAllAmount();
        //}

        private void CalculateAllAmount()
        {
            try
            {
                CalculateSpecialDiscount();

                double dTaxableAmt = 0, dFinalAmt = 0, dQty = 0, dCashAmt = 0, dCardAMt = 0, dCreditAmt = 0, dTOAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dPostage = 0, dGreenTaxAmt = 0, dRoundOff = 0, dSaleAmt = 0;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dSaleAmt = dba.ConvertObjectToDouble(txtSaleAmt.Text);

                dCardAMt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dPackingAmt = ConvertObjectToDouble(txtPackingAmt.Text);
                dPostage = ConvertObjectToDouble(txtPostage.Text);
                dGreenTaxAmt = ConvertObjectToDouble(txtGreenTax.Text);

                dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                double dDisPer = ConvertObjectToDouble(txtOtherPerSign.Text + txtOtherPer.Text);

                dDiscAmt = (dBasicAmt * dDisPer) / 100;
                dTOAmt = dOtherAmt + dPackingAmt + dPostage + dGreenTaxAmt + dDiscAmt + dCardAMt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, 0, ref dTaxableAmt);

                dFinalAmt = dBasicAmt + dTOAmt;
                dNetAmt = dFinalAmt + dTaxAmt;
                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));// Math.(dNetAmt,0);

                dCreditAmt = dNNetAmt - dCashAmt - dSaleAmt;

                dRoundOff = dNNetAmt - dNetAmt;

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
                if (dTaxableAmt > 0)
                    txtTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    txtTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

                dCreditAmt = Convert.ToDouble(dCreditAmt.ToString("0"));// Math.Round(dCreditAmt, 0);
                lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                txtFinalAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtNetAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
            }
            catch
            {
            }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "" && Convert.ToString(objValue) != "0")
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
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtPackingAmt.Text == "")
                    txtPackingAmt.Text = "0.00";
                CalculateAllAmount();
            }
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

        private void txtOtherAmount_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtOtherAmount.Text == "")
                    txtOtherAmount.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtOtherPer.Text == "")
                    txtOtherPer.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
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
            dba.ChangeLeaveColor(sender, e);
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
            if (btnAdd.Text == "&Add(F2)")
            {
                if (btnEdit.Text == "&Update(F6)")
                {
                    DialogResult _result = MessageBox.Show("Are you sure you want to add?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result != DialogResult.Yes)
                        return;
                }
                btnAdd.Text = "&Save(F5)";
                btnEdit.Text = "&Edit(F6)";
                EnableAllControls();
                btnAdd.TabStop = true;
                txtBillNo.ReadOnly = false;
                chkEmail.Checked = chkSendSMS.Checked = false;
                btnHold.Enabled = true;
                ClearAllText();
                SetSerialNo();
                txtSalesParty.Focus();
            }
            else if (ValidateControls() && CheckBillNoAndSuggest() && ValidateOtherValidation(false))
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
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(SaleBillNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    {
                        int check = dba.CheckSaleBillAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from (Select ISNULL(Max(BillNo)+1,1)BillNo from SalesRecord where BillCode='" + txtBillCode.Text + "' UNION ALL Select ISNULL(Max(BillNo)+1,1)BillNo from SalesBook where BillCode='" + txtBillCode.Text + "')_Sales "));
                            MessageBox.Show("Sorry ! Bill No is already taken !! Your Bill Number  : " + strBillNo, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Text = strBillNo;
                            chkStatus = true;
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

        private int HoldRecordReturnInt()
        {
            int _count = 0;
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL", strBillCode = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dCashAmt = 0, dCreditAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dFinalAmt = 0, dTaxableAmt = ConvertObjectToDouble(txtTaxableAmt.Text), dSaleAmt = ConvertObjectToDouble(txtSaleAmt.Text);
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strSalesMan = "", strMarketer = "DIRECT", strQuery = "";
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

                if (txtMarketer.Text != "" && txtMarketer.Text != "DIRECT")
                {
                    string[] _strFullName = txtMarketer.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strMarketer = _strFullName[0].Trim();
                    }
                }
                if (txtSalesMan.Text != "" && txtSalesMan.Text != "DIRECT")
                {
                    string[] _strFullName = txtSalesMan.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strSalesMan = _strFullName[0].Trim();
                    }
                }

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);

                dFinalAmt = dGrossAmt + _dOtherAmt;

                if (strHoldBilLCode != "" && strHoldBillNo != "")
                {
                    strQuery = " if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " ) begin "
                               + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='" + txtBStation.Text + "',[TransportName]='" + txtTransport.Text + "',[WaybillNo]='" + txtWayBillNo.Text + "',[WayBillDate]='" + txtWayBIllDate.Text + "',[NoOfCase]='" + txtNoofCases.Text + "',[LRNumber]='" + txtLRNumber.Text + "',[LRDate]=" + strLRDate + ",[LRTime]='" + txtTimeOfSupply.Text + "',[PvtMarka]='" + txtPvtMarka.Text + "',[Remark]='" + txtRemark.Text + "',[Description]='" + txtOtherPerSign.Text + "',[PackerName]='" + txtPackerName.Text + "',[PackingDate]=" + strPDate + ",[CartonType]='" + txtCartonType.Text + "',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtOtherPer.Text) + ",[DisAmt]=" + dDisc + ","
                               + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=" + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=" + dba.ConvertObjectToDouble(txtPostage.Text) + ",[GreenTax]=" + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strMarketer + "',[Description_2]='" + txtPetiType.Text + "' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[CashAmt]=" + dCashAmt + ", [CreditAmt]=" + dCreditAmt + ",[TaxableAmt]=" + dTaxableAmt + ",[PartSaleAmt]=" + dSaleAmt + " Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " "
                               + " Delete from StockMaster Where BillType='SALES' and BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " ";


                    string strID = "", strSONumber = "";
                    double dQty = 0, dRate = 0, dMRP = 0, dAmount = 0, dDisRate = 0;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                        dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                        dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                        dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                        dAmount = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                        dDisRate = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strID == "")
                        {
                            strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SalesMan]) VALUES "
                                     + " ('" + strHoldBilLCode + "'," + strHoldBillNo + ",0,'" + strSONumber + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisRate + "," + dRate + ","
                                     + " " + dAmount + ",0,0, " + dAmount + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["description"].Value + "','" + row.Cells["boxRoll"].Value + "','" + strSalesMan + "') ";
                        }
                        else
                            strQuery += " Update [dbo].[SalesBookSecondary] SET [SONumber]='" + strSONumber + "',[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + dDisRate + ",[Rate]=" + dRate + ",[Amount]=" + dAmount + ",[BasicAmt]=" + dAmount + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[Other1]='" + row.Cells["description"].Value + "',[Other2]='" + row.Cells["boxRoll"].Value + "',[SalesMan]='" + strSalesMan + "',[UpdateStatus]=1 Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " and SID=" + strID + "  ";


                        if ((MainPage._bTaxStatus || txtImportData.Text == "") && txtSalesType.Text != "NO STOCK")
                        {
                            strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                               + " ('SALES','" + strHoldBilLCode + "'," + strHoldBillNo + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                        }
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strHoldBilLCode + "'," + strHoldBillNo + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                    strQuery += " end ";
                }
                else
                {
                    strBillCode = txtBillCode.Text + "H";

                    strQuery = "Declare @SerialNo bigint; Select @SerialNo=(ISNULL(MAX(_BillNo),0)+1) from (Select MAX(BIllNo)_BillNo from SalesBook Where BillCode='" + strBillCode + "' UNION ALL Select MAX(BIllNo)_BillNo from SalesRecord Where BillCode='" + strBillCode + "')_SALES  "
                                   + " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo) begin "
                                   + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[CashAmt],[CreditAmt],[SaleBillType],[TaxableAmt],[IntStatus],[PartSaleAmt]) VALUES  "
                                   + " ('" + strBillCode + "',@SerialNo,'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','" + txtBStation.Text + "','" + txtTransport.Text + "','" + txtWayBillNo.Text + "','" + txtWayBIllDate.Text + "','" + txtNoofCases.Text + "','" + txtLRNumber.Text + "'," + strLRDate + ",'" + txtTimeOfSupply.Text + "','" + txtPvtMarka.Text + "','" + txtRemark.Text + "','" + txtOtherPerSign.Text + "','" + txtPackerName.Text + "'," + strPDate + ",'" + txtCartonType.Text + "','', "
                                   + " " + dba.ConvertObjectToDouble(txtOtherPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + "," + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + "," + dba.ConvertObjectToDouble(txtPostage.Text) + "," + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + "," + dba.ConvertObjectToDouble(lblTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + "," + dAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strMarketer + "','" + txtPetiType.Text + "','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + "," + dCashAmt + ", " + dCreditAmt + ",'HOLD'," + dTaxableAmt + ",0," + dSaleAmt + ")  ";


                    double dQty = 0, dRate = 0, dMRP = 0, dAmount = 0, dDisPer = 0;
                    string strSONumber = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                        dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                        dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                        dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                        dAmount = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                        dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SalesMan]) VALUES "
                                    + " ('" + strBillCode + "',@SerialNo,0,'" + strSONumber + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                    + " " + dAmount + ",0,0, " + dAmount + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["description"].Value + "','" + row.Cells["boxRoll"].Value + "','" + strSalesMan + "') ";

                        if ((MainPage._bTaxStatus || txtImportData.Text == "") && txtSalesType.Text != "NO STOCK")
                        {
                            strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                 + " ('SALES','" + strBillCode + "',@SerialNo, '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                        }
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strBillCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                    strQuery += " end ";
                }

                if (strQuery != "")
                {
                    _count = dba.ExecuteMyQuery(strQuery);
                }
            }
            catch
            {
            }
            return _count;
        }

        private void HoldRecord()
        {
            try
            {
                int count = HoldRecordReturnInt();
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record saved as hold successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add(F2)";
                    if (strHoldBilLCode != "")
                        txtBillCode.Text = strHoldBilLCode;
                    else
                        txtBillCode.Text = txtBillCode.Text + "H";

                    BindLastRecord();
                }
                else
                    MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch
            {
            }
        }


        private void SaveRecord()
        {
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dCashAmt = 0, dCardAmt = 0, dCreditAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dSaleAmt, dBalanceAmt;//,dFinalAmt=0;
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strSalesMan = "", strMarketer = "DIRECT", strTickStatus = "False";
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

                if (txtMarketer.Text != "" && txtMarketer.Text != "DIRECT")
                {
                    string[] _strFullName = txtMarketer.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strMarketer = _strFullName[0].Trim();
                    }
                }
                if (txtSalesMan.Text != "" && txtSalesMan.Text != "DIRECT")
                {
                    string[] _strFullName = txtSalesMan.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strSalesMan = _strFullName[0].Trim();
                    }
                }

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dSaleAmt = dba.ConvertObjectToDouble(txtSaleAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);

                dBalanceAmt = dAmt - dSaleAmt;
                if (txtImportData.Text != "" && !txtRemark.Text.Contains("UB"))
                    txtRemark.Text = ("UB " + txtRemark.Text).Trim();

                //dFinalAmt = dGrossAmt + _dOtherAmt;

                if (dBalanceAmt == dCashAmt)
                    strTickStatus = "True";

                string strQuery = " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + "  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ") begin "
                                + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[CashAmt],[CardAmt],[CreditAmt],[SaleBillType],[TaxableAmt],[IntStatus],[PartSaleAmt],[IRNNO]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','" + txtBStation.Text + "','" + txtTransport.Text + "','" + txtWayBillNo.Text + "','" + txtWayBIllDate.Text + "','" + txtNoofCases.Text + "','" + txtLRNumber.Text + "'," + strLRDate + ",'" + txtTimeOfSupply.Text + "','" + txtPvtMarka.Text + "','" + txtRemark.Text + "','" + txtOtherPerSign.Text + "','" + txtPackerName.Text + "'," + strPDate + ",'" + txtCartonType.Text + "','', "
                                + " " + dba.ConvertObjectToDouble(txtOtherPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + "," + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + "," + dba.ConvertObjectToDouble(txtPostage.Text) + "," + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + "," + dba.ConvertObjectToDouble(lblTotalQty.Text) + "," + dGrossAmt + "," + dAmt + "," + dAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strMarketer + "','" + txtPetiType.Text + "','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + "," + dCashAmt + "," + dCardAmt + ", " + dCreditAmt + ",'TRADING'" + "," + dba.ConvertObjectToDouble(txtTaxableAmt.Text) + ",'" + chkStatus.Checked.ToString() + "'," + dSaleAmt + ",'" + txtIRNo.Text + "')  "
                                + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dBalanceAmt + "','DR','" + dBalanceAmt + "','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  ";

                if (dCashAmt > 0)
                {
                    strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CashName,'CASH RECEIVED','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "','" + strSaleParty + "','CASH RECEIVED','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName)  ";

                }

                double dQty = 0, dRate = 0, dMRP = 0, dAmount = 0, dDisPer = 0;
                string strSONumber = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dAmount = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                    strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SalesMan],[SaleIncentive],[BarCode_S]) VALUES "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + strSONumber + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                + " " + dAmount + ",0,0, " + dAmount + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["description"].Value + "','" + row.Cells["boxRoll"].Value + "','" + strSalesMan + "',0,'" + row.Cells["barcode_s"].Value + "') ";

                    if ((MainPage._bTaxStatus || txtImportData.Text == "") && txtSalesType.Text != "NO STOCK")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                    }

                    if (strSONumber != "")
                        strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + dQty + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dQty + "), UpdateStatus=1 where (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode)='" + strSONumber + "'  ";
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                if (dTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                                   + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                {
                    strQuery += " INSERT INTO [dbo].[CardDetails]([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                   + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                if (strSaleBillType == "HOLD" && strHoldBilLCode != "" && strHoldBillNo != "")
                {
                    strQuery += "Delete from SalesBook Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " Delete from SalesBookSecondary Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " Delete from StockMaster Where BillType='SALES' and BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " ";
                }
                strQuery += " end ";

                if (strQuery != "")
                {
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        string strMobileNo = "", strPath = "";
                        SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                        SendSMSToParty(strMobileNo);

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
                    else
                        BindRecordWithControl(txtBillNo.Text);

                    btnEdit.Text = "&Update(F6)";
                    EnableAllControls();
                    dgrdDetails.ReadOnly = qtyAdjustStatus;
                    txtBillNo.ReadOnly = true;
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                    if (dgrdCardDetail.Rows.Count == 0)
                    {
                        dgrdCardDetail.Rows.Add(1);
                        dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                    }
                    btnAdd.TabStop = false;
                    txtDate.Focus();
                }
                else if (ValidateControls() && ValidateOtherValidation(false))
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
                string strDate = "", strLRDate = "NULL", strPDate = "NULL", strDeletedSIDQuery = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dSaleAmt = ConvertObjectToDouble(txtSaleAmt.Text), dBalanceAmt;//, dFinalAmt = 0; 
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strTaxAccountID = "", strSalesMan = "", strMarketer = "DIRECT", strTickStatus = "False", strTickQuery = "";
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

                if (txtMarketer.Text != "" && txtMarketer.Text != "DIRECT")
                {
                    string[] _strFullName = txtMarketer.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strMarketer = _strFullName[0].Trim();
                    }
                }
                if (txtSalesMan.Text != "" && txtSalesMan.Text != "DIRECT")
                {
                    string[] _strFullName = txtSalesMan.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strSalesMan = _strFullName[0].Trim();
                    }
                }
                //  dFinalAmt = dGrossAmt + _dOtherAmt;
                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);

                dBalanceAmt = dAmt - dSaleAmt;

                if (dOldCashAmt == (dOldNetAmt - dOldSaleAmt) && dBalanceAmt != dCashAmt)
                {
                    strTickQuery = ",[Tick]='False' ";
                    strTickStatus = "False";
                }

                if (dBalanceAmt == dCashAmt)
                {
                    strTickQuery = ",[Tick]='True' ";
                    strTickStatus = "True";
                }
                if (txtImportData.Text != "" && !txtRemark.Text.Contains("UB"))
                    txtRemark.Text = ("UB " + txtRemark.Text).Trim();

                string strQuery = " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text
                                + " if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin "
                                + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='" + txtBStation.Text + "',[TransportName]='" + txtTransport.Text + "',[WaybillNo]='" + txtWayBillNo.Text + "',[WayBillDate]='" + txtWayBIllDate.Text + "',[NoOfCase]='" + txtNoofCases.Text + "',[LRNumber]='" + txtLRNumber.Text + "',[LRDate]=" + strLRDate + ",[LRTime]='" + txtTimeOfSupply.Text + "',[PvtMarka]='" + txtPvtMarka.Text + "',[Remark]='" + txtRemark.Text + "',[Description]='" + txtOtherPerSign.Text + "',[PackerName]='" + txtPackerName.Text + "',[PackingDate]=" + strPDate + ",[CartonType]='" + txtCartonType.Text + "',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtOtherPer.Text) + ",[DisAmt]=" + dDisc + ","
                                + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=" + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=" + dba.ConvertObjectToDouble(txtPostage.Text) + ",[GreenTax]=" + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(txtTaxableAmt.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dAmt + ",[NetAmt]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strMarketer + "',[Description_2]='" + txtPetiType.Text + "' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[CashAmt]=" + dCashAmt + ",[CardAmt]=" + dCardAmt + ", [CreditAmt]=" + dCreditAmt + ", [IntStatus]='" + chkStatus.Checked.ToString() + "',[PartSaleAmt]=" + dSaleAmt + ",AttachedBill = '" + txtAttachBill.Text + "',[IRNNO]='" + txtIRNo.Text + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dBalanceAmt + ",[FinalAmount]='" + dBalanceAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' " + strTickQuery + " Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C'  "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from [dbo].[CardDetails]Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";


                if (dCashAmt > 0)
                {
                    strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus] in ('CASH RECEIVE','CASH RECEIVED') AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "') begin "
                                     + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CashName,'CASH RECEIVED','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "','" + strSaleParty + "','CASH RECEIVED','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','" + strTickStatus + "','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName) end else begin "
                                    + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('CASH RECEIVE','CASH RECEIVED') and Status='DEBIT' "
                                    + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' " + strTickQuery + " Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('CASH RECEIVE','CASH RECEIVED') and Status='CREDIT' end "
                                    + " Delete from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ";
                }
                else
                    strQuery += " Delete from BalanceAmount Where [AccountStatus] in ('CASH RECEIVE','CASH RECEIVED') AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                             + " Delete from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ";

                string strID = "", strSONumber = "";
                double dQty = 0, dRate = 0, dMRP = 0, dAmount = 0, dDisRate = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dAmount = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    dDisRate = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                    strID = Convert.ToString(row.Cells["id"].Value).Trim();
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[SalesMan],[SaleIncentive],[BarCode_S]) VALUES "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + strSONumber + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisRate + "," + dRate + ","
                                 + " " + dAmount + ",0,0, " + dAmount + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["description"].Value + "','" + row.Cells["boxRoll"].Value + "','" + strSalesMan + "',0,'" + row.Cells["barcode_s"].Value + "') ";
                    }
                    else
                        strQuery += " Update [dbo].[SalesBookSecondary] SET [SONumber]='" + strSONumber + "',[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + dDisRate + ",[Rate]=" + dRate + ",[Amount]=" + dAmount + ",[BasicAmt]=" + dAmount + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[Other1]='" + row.Cells["description"].Value + "',[Other2]='" + row.Cells["boxRoll"].Value + "',[SalesMan]='" + strSalesMan + "',[UpdateStatus]=1,[BarCode_S]='" + row.Cells["barcode_s"].Value + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + "  ";

                    if (strSONumber != "")
                        strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + dQty + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dQty + "), UpdateStatus=1 where (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode)='" + strSONumber + "'  ";


                    if ((MainPage._bTaxStatus || txtImportData.Text == "") && txtSalesType.Text != "NO STOCK")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                           + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                if (dTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName=Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                             + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }
                foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                {
                    strQuery += " INSERT INTO [dbo].[CardDetails] ([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                   + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                if (strAllDeletedID != "")
                {
                    strQuery += " Delete from [dbo].[SalesBookSecondary] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strAllDeletedID + ") ";
                    if (MainPage.strOnlineDataBaseName != "")
                    {
                        strDeletedSIDQuery = " Delete from [dbo].[SalesBookSecondary] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strAllDeletedID + ") ";
                    }
                }

                strQuery += " end";


                result = dba.ExecuteMyQuery(strQuery);
                if (result > 0 && strDeletedSIDQuery != "")
                {
                    DataBaseAccess.CreateDeleteQuery(strDeletedSIDQuery);
                    strAllDeletedID = "";
                }
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

        private void txtSalesType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
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
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
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
            dba.ChangeLeaveColor(sender, e);
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
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                dba.ValidateSpace(sender, e);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    GSTPrintAndPreview(false, "", false, true);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Book Custom", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    SetSignatureInBill(true, false, true);
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

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView)
            {
                if (!MainPage.mymainObject.bSaleAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bSaleEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bSaleView)
                    txtBillNo.Enabled = false;
                if (MainPage._bTaxStatus)
                    btnGenerateSeperateBill.Enabled = false;

                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(Close));
                return false;
            }
        }


        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, double dInsuranceAmt, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0, dServiceAmount = 0, dOtherChargesAmt = 0;//,dInsuranceAmt=0
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtSalesType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);

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

                            //dInsuranceAmt = dba.ConvertObjectToDouble(txtDiscAmt.Text);

                            string strQuery = "", strSubQuery = "", strGRSNo = "", strTaxRate = "", strSSQuery = "";
                            double dDisStatus = 0;

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;

                            double dRate = 0, dQty = 0, dAmt = 0, dBasicAmt = 0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dBasicAmt = dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                                //dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);

                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dBasicAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0 ";
                                    if (!txtBillCode.Text.Contains("PTN"))
                                    {
                                        if (MainPage.strSoftwareType == "AGENT")
                                            strQuery += " UNION ALL Select '' as ID, '' as HSNCode,0 as Quantity, (((((Amount*(100+" + dDisStatus + ")/100.00)*TaxRate/100.00)*3)/100.00)) Amount,0 TaxRate from (Select (CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end) Amount,TaxRate from( Select " + dQty + " as Quantity,ROUND((((" + dAmt + ")*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + " >0 )_Sales)_Sales ";
                                    }

                                    if (strSSQuery != "")
                                        strSSQuery += " Union ALL ";
                                    strSSQuery += " Select (((((Amount*(100+" + dDisStatus + ")/100.00)*TaxRate/100.00)*3)/100.00)) Amount from (Select (CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end) Amount,TaxRate from( Select " + dQty + " as Quantity,ROUND((((" + dAmt + ")*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + " >0 )_Sales)_Sales ";
                                }
                            }

                            //if (dInsuranceAmt != 0 && txtOtherPerSign.Text != "-")
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
                                strQuery = " Select SUM(TaxableAmt)TaxableAmt,SUM(ROUND(Amt,4)) as Amt,SUM(ROUND(Amt,2)) as TaxAmt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) OtherChargesAmt from (Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from (  "
                                         + " Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' and Qty>0 then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by HSNCode,TaxRate)_Sales  Group by TaxRate ";

                                strQuery += strSubQuery;
                                if (!txtBillCode.Text.Contains("PTN") && strSSQuery != "")
                                {
                                    if (MainPage.strSoftwareType == "AGENT")
                                        strSSQuery = " Select SUM(Amount)Amt from ( " + strSSQuery + ") _Sales ";
                                    else
                                        strSSQuery = "Select 0  as Amt";
                                }
                                else
                                    strSSQuery = "Select 0  as Amt";

                                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery + strSSQuery);
                                if (ds.Tables.Count > 0)
                                {
                                    DataTable dt = ds.Tables[0];
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    //BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    if (dt.Rows.Count > 0)
                                        dOtherChargesAmt = dba.ConvertObjectToDouble(dt.Rows[0]["OtherChargesAmt"]);
                                    if (ds.Tables[1].Rows.Count > 0)
                                        dServiceAmount = DataBaseAccess.ConvertObjectToDoubleStatic(ds.Tables[1].Rows[0][0]);

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
            if (!txtBillCode.Text.Contains("PTN"))
            {
                if (MainPage.strSoftwareType == "AGENT")
                {
                    txtSign.Text = "+";
                    txtOtherAmount.Text = dServiceAmount.ToString("N2", MainPage.indianCurancy);
                }
            }

            if (_strTaxType == "INCLUDED")
                dTaxAmt = dOtherChargesAmt;
            return dTaxAmt;
        }


        //private void BindTaxDetails(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt, ref double dTaxableAmt)
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

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSalesType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (btnEdit.Text == "&Update(F6)" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtSalesParty.Text || dOldNetAmt != Convert.ToDouble(txtFinalAmt.Text) || _bUpdateStatus)
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
                if (!_bUpdateStatus && MainPage._bTaxStatus)
                {
                    string strRegion = Convert.ToString(dt.Rows[0]["Region"]), strCStateName = Convert.ToString(dt.Rows[0]["CStateName"]).ToUpper(), strSStateName = Convert.ToString(dt.Rows[0]["SStateName"]).ToUpper();
                    if (strRegion != "" && MainPage._bTaxStatus)
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
                if (strSaleBillType != "HOLD")
                {
                    MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }
            return true;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (ValidateOtherValidation(true))
                    {
                        if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && txtBillNo.Text != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {

                                string strQuery = " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text
                                                + " Delete from SalesBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('SALES A/C','DUTIES & TAXES','CASH RECEIVE','CASH RECEIVED')  "
                                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                                + " Delete from CardDetails Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtFinalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

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

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)") && txtSalesParty.Text != "")
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

        private void txtBStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBStation.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPackerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PACKERNAME", "SEARCH PACKER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPackerName.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCartonType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CARTONTYPE", "SEARCH CARTON TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCartonType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtCartonSize_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
        //        {
        //            char objChar = Convert.ToChar(e.KeyCode);
        //            int value = e.KeyValue;
        //            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //            {
        //                SearchData objSearch = new SearchData("CARTONSIZE", "SEARCH CARTON SIZE", e.KeyCode);
        //                objSearch.ShowDialog();
        //                string strSize = objSearch.strSelectedData;
        //                if (strSize != "")
        //                {
        //                    string[] strAllData = strSize.Split('|');
        //                    if (strAllData.Length > 1)
        //                    {
        //                        txtCartonSize.Text = strAllData[0];
        //                        //txtPackingType.Text = strAllData[1];
        //                        txtPackingAmt.Text = strAllData[2];
        //                    }
        //                    else
        //                    {
        //                        txtCartonSize.Text = "";
        //                        txtPackingAmt.Text = MainPage.dPackingAmount.ToString("0");
        //                    }
        //                }
        //                else
        //                {
        //                    txtCartonSize.Text = "";
        //                    txtPackingAmt.Text = MainPage.dPackingAmount.ToString("0");
        //                }
        //                CalculateAllAmount();
        //            }
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}

        private void SetPackingTaxAmt()
        {
            try
            {
                double dPackingAmt = 0;

                double dCase = dba.ConvertObjectToDouble(txtNoofCases.Text), _dPackingAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text);
                dPackingAmt = (dCase * _dPackingAmt);

                txtPackingAmt.Text = dPackingAmt.ToString("0.00");
            }
            catch { }
        }

        private void txtPostage_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtPostage.Text == "")
                    txtPostage.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtGreenTax_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtGreenTax.Text == "")
                    txtGreenTax.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtLRDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtPackingDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
        }

        private void txtTransport_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenTransportMaster(txtTransport.Text);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save(F5)")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALES", txtBillCode.Text, txtBillNo.Text);
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
                    SetSignatureInBill(true, false, true);
            }
            catch
            {
            }
        }

        private string SetSignatureInBill(bool _bPStatus, bool _createPDF, bool _dscVerified)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
            {
                if (!_bPStatus)
                {
                    string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
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

                        if (File.Exists(strPath))
                            File.Delete(strPath);
                    }
                    else
                    {
                        string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SalesBill\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        _strPath += "\\" + _strFileName;

                        strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                        if (File.Exists(strPath))
                            File.Delete(strPath);
                        Directory.CreateDirectory(_strPath);
                    }
                }

                bool _bstatus = GSTPrintAndPreview(_bPStatus, strFileName, true, _dscVerified);
                if (!_bPStatus)
                {
                    if (_bstatus)
                    {
                        if (!_dscVerified && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";

                            PDFSigner _objSigner = new PDFSigner();
                            bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                            if (!_bFileStatus)
                                strPath = "";
                            if (_bPStatus && _bFileStatus)
                                System.Diagnostics.Process.Start(strPath);
                        }
                        else
                        {
                            File.Copy(strFileName, strPath);
                            //if (_bPStatus)
                            //    System.Diagnostics.Process.Start(strPath);
                        }

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

        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            string strValue = "0";
            if (_pstatus)
            {
                strValue = "1";// Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
                //if (strValue == "" || strValue == "0")
                //{
                //    return false;
                //}
            }

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSalesBookRetailDataTable_Custom(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (!MainPage._bTaxStatus)
                {
                    if (MainPage.strClientName == "LOTUS")
                    {
                        Reporting.SaleBookRetailReport_A5 objOL_salebill = new Reporting.SaleBookRetailReport_A5();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        // objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        // objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "" && !_pstatus)
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                            return true;

                        }
                        else
                        {
                            if (_pstatus)
                            {
                                if (strValue != "" && strValue != "0")
                                {
                                    int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else
                    {
                        Reporting.SaleBookRetailReport_Custom objOL_salebill = new Reporting.SaleBookRetailReport_Custom();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        // objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "" && !_pstatus)
                        {
                            objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                            return true;

                        }
                        else
                        {
                            if (_pstatus)
                            {
                                if (strValue != "" && strValue != "0")
                                {
                                    int nCopy = Int32.Parse(strValue);
                                    objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                objReport.myPreview.ReportSource = objOL_salebill;
                                objReport.ShowDialog();
                            }
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                }
                else
                {
                    if (!_bIGST)
                    {
                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            Reporting.SaleBookRetailReport_CSGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_DSC();
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
                            else
                            {
                                if (_pstatus)
                                {
                                    if (strValue != "" && strValue != "0")
                                    {
                                        int nCopy = Int32.Parse(strValue);
                                        objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //objReport.myPreview.ShowExportButton = false;
                                    //objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                                objOL_salebill.Close();
                                objOL_salebill.Dispose();
                            }
                        }
                        else
                        {
                            Reporting.SaleBookRetailReport_CSGST_Custom objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_Custom();
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
                            else
                            {
                                if (_pstatus)
                                {
                                    if (strValue != "" && strValue != "0")
                                    {
                                        int nCopy = Int32.Parse(strValue);
                                        objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    // objReport.myPreview.ShowExportButton = false;
                                    //objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                                objOL_salebill.Close();
                                objOL_salebill.Dispose();
                            }
                        }
                    }
                    else
                    {
                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            Reporting.SaleBookRetailReport_IGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_IGST_DSC();
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
                            else
                            {
                                if (_pstatus)
                                {
                                    if (strValue != "" && strValue != "0")
                                    {
                                        int nCopy = Int32.Parse(strValue);
                                        objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //objReport.myPreview.ShowExportButton = false;
                                    // objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                                objOL_salebill.Close();
                                objOL_salebill.Dispose();
                            }
                        }
                        else
                        {
                            Reporting.SaleBookRetailReport_IGST_Custom objOL_salebill = new Reporting.SaleBookRetailReport_IGST_Custom();
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
                            else
                            {
                                if (_pstatus)
                                {
                                    if (strValue != "" && strValue != "0")
                                    {
                                        int nCopy = Int32.Parse(strValue);
                                        objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //  objReport.myPreview.ShowExportButton = false;
                                    //  objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                                objOL_salebill.Close();
                                objOL_salebill.Dispose();
                            }
                        }
                    }
                }
            }

            return false;
        }


        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified, string strCompanyCode)
        {
            string strValue = "0";
            //if (_pstatus)
            //{
            //    strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
            //    if (strValue == "" || strValue == "0")
            //    {
            //        return false;
            //    }
            //}

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSalesBookRetailDataTable_Remote(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus, strCompanyCode);
            if (dt.Rows.Count > 0)
            {
                if (!_bIGST)
                {
                    Reporting.SaleBookRetailReport_CSGST objOL_salebill = new Reporting.SaleBookRetailReport_CSGST();
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
                    else
                    {
                        if (_pstatus)
                        {
                            if (strValue != "" && strValue != "0")
                            {
                                int nCopy = Int32.Parse(strValue);
                                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                            }
                        }
                        else
                        {
                            Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                            objReport.myPreview.ReportSource = objOL_salebill;
                            objReport.ShowDialog();
                        }
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                }
                else
                {
                    Reporting.SaleBookRetailReport_IGST objOL_salebill = new Reporting.SaleBookRetailReport_IGST();
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
                    else
                    {
                        if (_pstatus)
                        {
                            if (strValue != "" && strValue != "0")
                            {
                                int nCopy = Int32.Parse(strValue);
                                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                            }
                        }
                        else
                        {
                            Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                            objReport.myPreview.ReportSource = objOL_salebill;
                            objReport.ShowDialog();
                        }
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                }
            }
            return false;
        }

        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    string strPath = SetSignatureInBill(false, false, true), strEmailID = "", strWhatsAppNo = "";
                    if (strPath != "")
                    {
                        strFilePath = strPath;
                        string[] strParty = txtSalesParty.Text.Split(' ');
                        if (strParty.Length > 1)
                        {
                            string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "' and GroupName='SUNDRY DEBTORS'  ";
                            DataTable _dt = dba.GetDataTable(strQuery);
                            if (_dt.Rows.Count > 0)
                            {
                                strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                                strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                                strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                                if (strEmailID != "")
                                {
                                    SendMail(strEmailID, strPath, 0);
                                }
                                else if (_bStatus)
                                    MessageBox.Show("Sorry ! Please enter mail id in party master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                if (strWhatsAppNo != "")
                                {
                                    if ((MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strCompanyName.Contains("SUPER")))
                                        SendWhatsappMessage(strWhatsAppNo, strPath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SendWhatsappMessage(string strMobileNo, string strPath)
        {
            string strMsgType = "", _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strBranchCode = txtBillCode.Text, strWhastappMessage = "";
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
            string strMType = "";
            if (btnEdit.Text == "&Update(F6)")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                strMsgType = "sale_bill_update";
                strMType = "invoice_update";
            }
            else
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                    dba.DeleteSaleBillFile(strPath, strBranchCode);

                strMsgType = "sale_bill";
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

                strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + txtFinalAmt.Text + "\",";
                if (strMobileNo != "")
                {
                    string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                    if (strResult != "")
                        MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                strWhastappMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + txtFinalAmt.Text + "\"}";
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strWhastappMessage, "", "");
            }
        }

        private void SendEmailBiltyToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (strFilePath != "")
                {

                    string strEmailID = "", strWhatsAppNo = "";

                    string[] strParty = txtSalesParty.Text.Split(' ');
                    if (strParty.Length > 1)
                    {
                        string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "' and GroupName='SUNDRY DEBTORS'  ";
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                            strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                            strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                            if (strEmailID != "")
                                SendMail(strEmailID, strFilePath, 0);

                            if (strWhatsAppNo != "")
                            {
                                if ((MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strCompanyName.Contains("SUPER")) && MainPage.strSoftwareType == "AGENT")
                                    SendBiltyWhatsappMessage(strWhatsAppNo, strFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SendBiltyWhatsappMessage(string strMobileNo, string strPath)
        {
            if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
            {
                string _strFileName = "Bilty_" + txtBillCode.Text.Replace("18 -19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strBranchCode = txtBillCode.Text;
                string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
                string strWhastappMessage = "", strMsgType = "", strTextMsg = "", strMType = "";

                if (btnEdit.Text == "&Update(F6)")
                {
                    dba.DeleteSaleBillFile(strPath, strBranchCode);
                    strMsgType = "bilty_update";
                    strMType = "bilty_copy";
                    strTextMsg = "{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + strFilePath + "\"}";
                    strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + txtLRNumber.Text + "\",\"variable4\": \"" + txtLRDate.Text + "\",";
                }
                else
                {
                    strMsgType = "sale_bill_update";
                    strMType = "invoice_generation";
                    strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + txtFinalAmt.Text + "\",";
                    strTextMsg = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + txtFinalAmt.Text + "\"}";
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
                    string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                    if (strResult != "")
                        MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strTextMsg, "", "");
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

                    string strBalance = ".", strName = dba.GetSafePartyName(txtSalesParty.Text);
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
                        if (txtTransport.Text != "")
                            strSubMsg = " thru : " + txtTransport.Text;
                        if (txtLRNumber.Text != "")
                            strSubMsg += ", LR No. " + txtLRNumber.Text + " (" + txtLRDate.Text + ")";
                        if (txtRemark.Text != "")
                            strSubMsg += ", Note : " + txtRemark.Text;

                        if (btnAdd.Text == "&Save(F5)")
                            strMessage = "M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + txtFinalAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalQty.Text + " " + strSubMsg + strBalance;
                        else
                            strMessage = "Alert : M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + txtFinalAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalQty.Text + " " + strSubMsg + strBalance;


                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
            catch
            {
            }
        }

        private void SendMail(string strEmail, string strpath, int billStatus)
        {
            try
            {
                //MailAddress mailFrom = new MailAddress(MainPage.strSenderEmailID);
                //MailAddress mailTo = new MailAddress(strEmail);
                // MailMessage message = new MailMessage(mailFrom, mailTo);

                string strMessage = "", strSub = "";
                if (billStatus == 0)
                {
                    if (btnAdd.Text == "&Save(F5)" || (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)"))
                    {
                        strMessage = "A/c : " + txtSalesParty.Text + " , we have generated your sale bill <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "Update ! A/c : " + txtSalesParty.Text + ", we have update your sale bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
                    }

                    if (btnAdd.Text == "&Save(F5)")
                        strSub = "Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " created.";
                    else
                        strSub = "Alert ! Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " updated.";
                }
                else
                {
                    strMessage = " Alert ! Sale bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + "</b> is Deleted by : " + MainPage.strLoginName + "  and  the deleted Sale bill is attached with this mail. ";
                    strSub = "Alert ! Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " deleted by : " + MainPage.strLoginName;
                }

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "SALE BILL", true);
                if (billStatus == 0 && bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                dba.ChangeLeaveColor(sender, e);
            }
            catch { }
        }

        private void txtNoofCases_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                    SetPackingTaxAmt();
                dba.ChangeLeaveColor(sender, e);
            }
            catch { }
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
                        SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
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

        private bool ValidateStock()
        {
            if (!MainPage._bTaxStatus && txtImportData.Text != "")
                return true;
            else
            {
                DataTable _dt = GenerateDistinctItemName();
                bool _bStatus = dba.CheckQtyAvalability(_dt, txtBillCode.Text, txtBillNo.Text, dgrdDetails, lblMsg);
                if (!_bStatus && MainPage.strUserRole.Contains("SUPERADMIN"))
                    _bStatus = true;
                return _bStatus;
            }
        }

        private DataTable GenerateDistinctItemName()
        {
            DataTable _dt = new DataTable();
            try
            {
                _dt.Columns.Add("BarCode", typeof(String));
                _dt.Columns.Add("ItemName", typeof(String));
                _dt.Columns.Add("Variant1", typeof(String));
                _dt.Columns.Add("Variant2", typeof(String));
                _dt.Columns.Add("Variant3", typeof(String));
                _dt.Columns.Add("Variant4", typeof(String));
                _dt.Columns.Add("Variant5", typeof(String));
                _dt.Columns.Add("MRP", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("BarCode='" + row.Cells["barCode"].Value + "' and ItemName='" + row.Cells["itemName"].Value + "' and Variant1='" + row.Cells["variant1"].Value + "' and Variant2='" + row.Cells["variant2"].Value + "' and ISNULL(Variant3,'')='" + row.Cells["variant3"].Value + "' and ISNULL(Variant4,'')='" + row.Cells["variant4"].Value + "' and ISNULL(Variant5,'')='" + row.Cells["variant5"].Value + "'  and ISNULL(MRP,0)='" + row.Cells["mrp"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]), dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                        _rows[0]["Qty"] = dOQty + dQty;
                    }
                    else
                    {
                        DataRow _row = _dt.NewRow();
                        _row["BarCode"] = row.Cells["barCode"].Value;
                        _row["ItemName"] = row.Cells["itemName"].Value;
                        _row["Variant1"] = row.Cells["variant1"].Value;
                        _row["Variant2"] = row.Cells["variant2"].Value;
                        _row["Variant3"] = row.Cells["variant3"].Value;
                        _row["Variant4"] = row.Cells["variant4"].Value;
                        _row["Variant5"] = row.Cells["variant5"].Value;
                        _row["MRP"] = row.Cells["mrp"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
        }


        private void txtMarketer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MARKETER", "SEARCH MARKETER", e.KeyCode);
                        objSearch.ShowDialog();
                        txtMarketer.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        //private void btnShowBilty_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        btnShowBilty.Enabled = false;

        //        if (txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save(F5)")
        //        {
        //            DataBaseAccess.ShowBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnShowBilty.Enabled = true;
        //}

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

        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, dMRP = 0, _dMRP = 0, dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    _dMRP = dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (dSpclPer != 0 && dMRP != 0)
                    {
                        _dMRP = dMRP * (100.00 - dSpclPer) / 100.00;
                        dSpclAmt += ((dMRP * dSpclPer) / 100.00) * dQty;
                    }
                    else
                        _dMRP = dMRP;
                    dDisPer = Math.Abs(dDisPer);

                    if ((dDisPer != 0 || dSpclPer != 0) && _dMRP != 0)
                        dRate = _dMRP * (100.00 - (dDisPer)) / 100.00;
                    if (dRate == 0)
                        dRate = _dMRP;

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    // dDisc = ConvertObjectToDouble(row.Cells["disc"].Value);
                    // dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                    //  row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
                }

                txtSplDisAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
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
                        SearchDataOther objSearch = new SearchDataOther("_BillNo", "", "SEARCH SALE BILL NO", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportData.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
                        else
                            txtImportData.Text = "";
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
            if (txtImportData.Text != "" && (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
            {
                BindRecordWithControlWithImport();
            }
        }

        private void BindRecordWithControlWithImport()
        {
            try
            {
                strOldPartyName = "";
                string strQuery = " Select *,CONVERT(varchar,Date,103)BDate,CONVERT(varchar,ISNULL(PackingDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)PDate,CONVERT(varchar,ISNULL(LrDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)LDate,ISNULL(PAgent,'DIRECT') PAgent,(CASE WHEN SalesType Like('%INCLUDE%') then (GrossAmt-TaxAmt) else GrossAmt end)GAmt from SalesBook SB  OUTER APPLY (Select Top 1 (Description_1+' '+Name)PAgent from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.Description_1)SM Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "' "
                                + " Select SBS.*,dbo.GetFullName(SBS.SalesMan)SalesManName,_IM.BarCode,HSNCode from SalesBookSecondary SBS OUTER APPLY ( Select Top 1 _IS.Description as BarCode,IGM.HSNCode from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode  and _IM.BillNo=_IS.BillNo inner join ItemGroupMaster IGM on _IM.GroupName=IGM.GroupName Where _IM.ItemName=SBS.ItemName and _IS.Variant1=SBS.Variant1 and _IS.Variant2=SBS.Variant2 and [ActiveStatus]=1) _IM Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'  order by SID ";
                //DataSet ds = SearchDataOther.GetDataSet(strQuery);
                DataSet ds = dba.GetDatFromAllFirm_OtherCompany_DS(strQuery);
                txtReason.Text = strOldLRNumber = "";
                pnlDeletionConfirmation.Visible = false;
                lblCreatedBy.Text = "";
                DataTable dt = null;
                if (ds.Tables.Count > 1)
                {
                    dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            DataRow row = dt.Rows[0];

                            txtNoofCases.Text = Convert.ToString(row["NoOfCase"]);
                            strOldLRNumber = txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            txtLRDate.Text = Convert.ToString(row["LDate"]);
                            txtTimeOfSupply.Text = Convert.ToString(row["LRTime"]);
                            txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                            txtPackerName.Text = Convert.ToString(row["PackerName"]);
                            txtPackingDate.Text = Convert.ToString(row["PDate"]);
                            txtCartonType.Text = Convert.ToString(row["CartonType"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtTransport.Text = Convert.ToString(row["TransportName"]);
                            txtBStation.Text = Convert.ToString(row["Station"]);
                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtPetiType.Text = Convert.ToString(row["Description_2"]);
                            txtMarketer.Text = Convert.ToString(row["PAgent"]);
                            txtOtherAmount.Text = Convert.ToString(row["GAmt"]);
                            txtSign.Text = "-";
                            //double dSpclDisPer = dba.ConvertObjectToDouble(row["SpecialDscPer"]);
                            //if (dSpclDisPer != 0)
                            //    dSpclDisPer = (100 - dSpclDisPer);
                            //txtSpclDisPer.Text = dSpclDisPer.ToString("N2", MainPage.indianCurancy);
                        }
                    }
                }

                dt = ds.Tables[1];
                dgrdDetails.Rows.Clear();
                int rowIndex = 0;
                if (dt.Rows.Count > 0)
                {
                    string strHSNCode = "", strBarCode = "";
                    txtSalesMan.Text = Convert.ToString(dt.Rows[0]["SalesManName"]);
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
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
                        dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strBarCode;
                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strHSNCode;
                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = "";// row["Variant1"];
                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = ""; //row["Variant2"];
                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = "";//row["Variant3"];
                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = "";// row["Variant4"];
                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = "";// row["Variant5"];
                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                        dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                        dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                        //dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                        //dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                        //dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                        if (strHSNCode == "")
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;

                        rowIndex++;
                    }
                }

                CalculateAllAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private string GetHSNCode(object _objHSNCode)
        {
            string strQuery = "";

            strQuery = "Select Top 1 ItemName from Items _Im  inner join ItemGroupMaster IGM on _IM.GroupName=IGM.GroupName WHere ItemName Like('%" + _objHSNCode + "') OR HSNCode Like('" + _objHSNCode + "')";
            object obj = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(obj);
        }

        private void txtMarketer_DoubleClick(object sender, EventArgs e)
        {
            if (txtMarketer.Text != "" && txtMarketer.Text != "DIRECT")
                DataBaseAccess.OpenPartyMaster(txtMarketer.Text);
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;
                DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string strPath = SetSignatureInBill(false, true, true);
                    if (strPath != "")
                        MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }



        private void btnGenerateSeperateBill_Click(object sender, EventArgs e)
        {
            btnGenerateSeperateBill.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && dgrdDetails.Rows.Count > 0)
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

            btnGenerateSeperateBill.Enabled = true;
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
        private string GetNextYearDBName(string DBName)
        {
            try
            {
                foreach (DataRow dr in NextYearDBNames.Rows)
                {
                    if (Convert.ToString(dr["CurrentDB"]) == DBName)
                    {
                        DBName = Convert.ToString(dr["NextDB"]);
                        return DBName;
                    }
                }
                string strQry = "SELECT  DbName = 'A' + Right(Next_Y_Path, CHARINDEX('\\',REVERSE(Next_Y_Path))-1) FROM " + DBName + ".dbo.Company where Next_Y_Path != ''";
                DataTable dt = dba.GetDataTable(strQry);
                if (dt.Rows.Count > 0)
                {
                    string NextDB = Convert.ToString(dt.Rows[0]["DbName"]);
                    if (NextDB.Length > 1 && NextDB != "A0")
                    {
                        DataRow nr = NextYearDBNames.NewRow();
                        nr["CurrentDB"] = DBName;
                        nr["NextDB"] = NextDB;
                        NextYearDBNames.Rows.Add(nr);
                        DBName = NextDB;
                    }
                }
            }
            catch (Exception ex) { }
            return DBName;
        }
        private DataTable CreateSecondaryDataTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("id", typeof(String));
            _dt.Columns.Add("srNo", typeof(String));
            _dt.Columns.Add("soNumber", typeof(String));
            _dt.Columns.Add("barCode", typeof(String));
            _dt.Columns.Add("brandName", typeof(String));
            _dt.Columns.Add("styleName", typeof(String));
            _dt.Columns.Add("itemName", typeof(String));
            _dt.Columns.Add("variant1", typeof(String));
            _dt.Columns.Add("variant2", typeof(String));
            _dt.Columns.Add("variant3", typeof(String));
            _dt.Columns.Add("variant4", typeof(String));
            _dt.Columns.Add("variant5", typeof(String));
            _dt.Columns.Add("description", typeof(String));
            _dt.Columns.Add("boxRoll", typeof(Double));
            _dt.Columns.Add("qty", typeof(String));
            _dt.Columns.Add("unitName", typeof(String));
            _dt.Columns.Add("mrp", typeof(String));
            _dt.Columns.Add("disPer", typeof(String));
            _dt.Columns.Add("rate", typeof(String));
            _dt.Columns.Add("amount", typeof(String));
            _dt.Columns.Add("HSNCode", typeof(double));
            _dt.Columns.Add("barcode_s", typeof(String));
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
                        {
                            strCompanyCode = GetNextYearDBName(str[0]);
                        }

                        if (strCompanyCode != "" && strCompanyCode != MainPage.strDataBaseFile)
                        {
                            DataRow _row = _dt.NewRow();
                            for (int _index = 0; _index < dgrdDetails.ColumnCount - 2; _index++)
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
                string[] strReport = { "TAX CALCULATION : Sale Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string CreatePDFFile()
        {
            string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill", strFileName = strPath + "\\" + txtBillNo.Text + ".pdf";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);

            strFileName = SetSignatureInBill(false, true, true);

            return strFileName;
        }

        private void txtCashAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (txtCashAmt.Text == "")
                    txtCashAmt.Text = "0.00";
                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void SaleBook_Retail_Custom_FormClosing(object sender, FormClosingEventArgs e)
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

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 6)
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

        private void btnHClose_Click(object sender, EventArgs e)
        {
            pnlHold.Visible = false;
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

        private void btnWayBillNo_Click(object sender, EventArgs e)
        {
            btnWayBillNo.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "" && !txtTransport.Text.Contains("BY HAND"))
                {
                    if (txtTransport.Text != "")
                    {
                        if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want generate JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";

                                var _success = dba.GenerateEWayBillJSON(strBillNo, "TRADING");
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
                        txtTransport.Focus();
                    }
                }
            }
            catch { }
            btnWayBillNo.Enabled = true;
        }

        private void btnPrintDispatchSlip_Click(object sender, EventArgs e)
        {
            btnPrintDispatchSlip.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                    {
                        Reporting.cryDispatchSlip objReport = new Reporting.cryDispatchSlip();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("DISPATCH SLIP PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Dispatch Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrintDispatchSlip.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("SerialCode", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Remarks", typeof(String));
                myDataTable.Columns.Add("Particulars", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("NoofPages", typeof(String));
                myDataTable.Columns.Add("Vat", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));
                myDataTable.Columns.Add("TotalBundels", typeof(String));
                myDataTable.Columns.Add("GrandTotal", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));

                //foreach (DataGridViewRow dr in dgrdDetails.Rows)
                //{
                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = MainPage.strCompanyName;
                string strSRCode = Convert.ToString(txtBillCode.Text) + " " + Convert.ToString(txtBillNo.Text);
                row["SerialCode"] = strSRCode;
                row["Date"] = txtDate.Text;
                row["TotalQty"] = lblTotalQty.Text;
                row["Remarks"] = txtRemark.Text;
                row["PartyName"] = txtSalesParty.Text;
                row["HeaderImage"] = MainPage._headerImage;
                row["Particulars"] = strSRCode + " - " + txtDate.Text;
                row["Amount"] = txtFinalAmt.Text;
                row["NoofPages"] = "";
                row["Vat"] = "";
                row["TotalBundels"] = "";
                row["GrandTotal"] = txtFinalAmt.Text;

                row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                row["Headername"] = "DISPATCH SLIP";

                myDataTable.Rows.Add(row);
                // }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }

        private void btnEInvoice_Click(object sender, EventArgs e)
        {
            btnEInvoice.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && dgrdDetails.Rows.Count > 0)
                    {
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want E-Invoice JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                object obj = DataBaseAccess.ExecuteMyScalar("Select GSTNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtSalesParty.Text + "' and GSTNo!=''");
                                if (Convert.ToString(obj) != "")
                                {
                                    string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";
                                    var _success = dba.GenerateEInvoiceJSON_SaleBook(true, strBillNo, "TRADING");
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
            }
            catch { }
            btnEInvoice.Enabled = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTaxAmt_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
        }

        private void txtSalesMan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESMANNAME", "SEARCH SALES MAN NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSalesMan.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSalesMan_DoubleClick(object sender, EventArgs e)
        {
            if (txtSalesMan.Text != "" && txtSalesMan.Text != "DIRECT")
                DataBaseAccess.OpenPartyMaster(txtSalesMan.Text);
        }

        private void txtCardAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtCardAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }

        private void txtCardAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                double dcardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    TextBox txtNew = sender as TextBox;
                    if (txtNew.Text == "")
                        txtNew.Text = "0.00";
                    if (dcardAmt > 0)
                    {
                        dgrdCardDetail.ReadOnly = false;
                        dgrdCardDetail.Enabled = true;
                        dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells["bank"];
                        dgrdCardDetail.Rows[0].Cells["cAmt"].Value = Convert.ToString(txtCardAmt.Text);
                        CalculateAllAmount();

                    }
                    else
                    {
                        dgrdCardDetail.ReadOnly = true;
                        dgrdCardDetail.Rows.Clear();
                        dgrdCardDetail.Rows.Add();
                        dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                        CalculateAllAmount();
                    }
                }
                if (dcardAmt > 0)
                {
                    pnlCard.Visible = true;
                    dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells["bank"];
                }
            }
            catch (Exception ex)
            { }

            dba.ChangeLeaveColor(sender, e);
        }

        private void btnCardClose_Click(object sender, EventArgs e)
        {
            pnlCard.Visible = false;
        }

        private void dgrdCardDetail_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                    e.Cancel = true;
                else if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (e.ColumnIndex == 3)
                    {
                        SearchData objSearch = new SearchData("CARDTYPE", "Search Card Type", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSearchData != "")
                        {
                            dgrdCardDetail.CurrentCell.Value = objSearch.strSelectedData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdCardDetail.CurrentCell.Value = objSearch.strSelectedData;

                        e.Cancel = true;
                    }

                }
            }
            catch
            {
                e.Cancel = true;
            }
        }
        private void CalculateCardAmount()
        {
            try
            {
                double dAmt = 0;
                foreach (DataGridViewRow row in dgrdCardDetail.Rows)
                    dAmt += ConvertObjectToDouble(row.Cells["cAmt"].Value);
                //chkCardAmt.Checked = dAmt > 0 ? true : false;

                txtCardAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                CalculateAllAmount();
            }
            catch { }
        }
        private void dgrdCardDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 6)
                    CalculateCardAmount();

            }
            catch { }
        }

        private void dgrdCardDetail_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellFocusColor(sender, e);
        }

        private void dgrdCardDetail_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellLeaveColor(sender, e);
        }

        private void dgrdCardDetail_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                {
                    int Date = 0;
                    Date = Convert.ToInt32(dgrdCardDetail.CurrentRow.Cells["cExpiryDate"].Value);

                    if (Convert.ToBoolean(Date))
                    {
                        string strDate = Convert.ToString(dgrdCardDetail.CurrentCell.EditedFormattedValue);
                        if (strDate != "")
                        {
                            strDate = strDate.Replace("/", "");
                            if (strDate.Length == 4)
                            {
                                TextBox txtDate = new TextBox();
                                //txtDate.Text = strDate;
                                //dba.GetStringFromDateForCompany(txtDate);
                                Double dMonth = Convert.ToDouble(strDate.Substring(0, 2)), dYear = Convert.ToDouble(strDate.Substring(2, 2));
                                if (dMonth < 1 || dMonth > 12)
                                {
                                    MessageBox.Show("Month is not valid : " + dMonth, "Invalid Month ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdCardDetail.CurrentCell = dgrdCardDetail.CurrentRow.Cells["cExpiryDate"];
                                    e.Cancel = true;
                                    dgrdCardDetail.Focus();
                                    return;
                                }
                                if (dYear < 20)
                                {
                                    MessageBox.Show("Year is not valid : " + dYear, "Invalid Year ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdCardDetail.CurrentCell = dgrdCardDetail.CurrentRow.Cells["cExpiryDate"];
                                    dgrdCardDetail.Focus();
                                    return;
                                }
                                string strMon = Convert.ToString(dMonth);
                                if (strMon.Length < 2)
                                    txtDate.Text = "0" + Convert.ToString(dMonth) + "/" + Convert.ToString(dYear);
                                else
                                    txtDate.Text = Convert.ToString(dMonth) + "/" + Convert.ToString(dYear);
                                try
                                {
                                    if (!txtDate.Text.Contains("/"))
                                    {
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        if (e.RowIndex != dgrdCardDetail.Rows.Count - 1)
                                        {
                                            dgrdCardDetail.EndEdit();
                                        }
                                    }
                                    dgrdCardDetail.CurrentCell.Value = txtDate.Text;
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                MessageBox.Show("Date format is not valid ! Please Specify in MMyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                e.Cancel = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void dgrdCardDetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdCardDetail.CurrentCell.ColumnIndex;
                if (columnIndex == 4 || columnIndex == 5 || columnIndex == 6)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_Card_KeyPress);
                }
            }
            catch
            { }
        }
        private void txtBox_Card_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdCardDetail.CurrentCell.ColumnIndex;
                if (columnIndex == 3 || columnIndex == 4)
                {
                    dba.ValidateSpace(sender, e);
                }
                else if (columnIndex == 5)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
        }

        private void dgrdCardDetail_KeyDown(object sender, KeyEventArgs e)
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
                        Index = dgrdCardDetail.CurrentCell.RowIndex;
                        IndexColmn = dgrdCardDetail.CurrentCell.ColumnIndex;
                        if (Index < dgrdCardDetail.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdCardDetail.ColumnCount - 1)
                        {
                            IndexColmn += 1;
                            if (!dgrdCardDetail.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdCardDetail.RowCount - 1)
                        {
                            string strCardType = Convert.ToString(dgrdCardDetail.Rows[CurrentRow].Cells["cCardType"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdCardDetail.Rows[CurrentRow].Cells["cAmt"].Value);

                            if (strCardType != "" && dAmt > 0)
                            {
                                dgrdCardDetail.Rows.Add(1);
                                dgrdCardDetail.Rows[dgrdCardDetail.RowCount - 1].Cells["cSNo"].Value = dgrdCardDetail.Rows.Count;
                                dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[dgrdCardDetail.RowCount - 1].Cells["bank"];
                                dgrdCardDetail.Focus();
                            }
                            else
                            {
                                SelectNextControl(dgrdCardDetail, true, true, true, true);
                            }

                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save(F5)")
                    {
                        dgrdCardDetail.Rows.RemoveAt(dgrdCardDetail.CurrentRow.Index);
                        if (dgrdCardDetail.Rows.Count == 0)
                        {
                            dgrdCardDetail.Rows.Add(1);
                            dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                            dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells["salesMan"];
                            dgrdCardDetail.Enabled = true;
                        }
                        else
                        {
                            ArrangeCardSerialNo();
                        }
                        CalculateCardAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update(F6)")
                    {
                        //string strID = Convert.ToString(dgrdCardDetail.CurrentRow.Cells["cID"].Value);
                        //if (strID == "")
                        //{
                        dgrdCardDetail.Rows.RemoveAt(dgrdCardDetail.CurrentRow.Index);
                        if (dgrdCardDetail.Rows.Count == 0)
                        {
                            dgrdCardDetail.Rows.Add(1);
                            dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                            dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells[2];
                            dgrdCardDetail.Enabled = true;
                        }
                        else
                        {
                            ArrangeCardSerialNo();
                        }
                        CalculateCardAmount();
                        //}
                        //else
                        //{
                        //    DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //    if (result == DialogResult.Yes)
                        //    {
                        //        //DeleteOneRow(strID);
                        //    }
                        //}
                    }

                }
            }
            catch (Exception ex)
            { }
        }
        private void ArrangeCardSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdCardDetail.Rows)
            {//cSNo
                row.Cells["cSNo"].Value = serialNo;
                serialNo++;
            }
        }
        private void btnPackingSlip_Click(object sender, EventArgs e)
        {
            btnPackingSlip.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable_PackingSlip();
                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                    {
                        Reporting.cryPackingSlip objReport = new Reporting.cryPackingSlip();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PACKING SLIP PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Packing Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPackingSlip.Enabled = true;

        }

        private DataTable CreateDataTable_PackingSlip()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));
                myDataTable.Columns.Add("SerialCode", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("SubPartyName", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Remarks", typeof(String));
                myDataTable.Columns.Add("City", typeof(String));
                myDataTable.Columns.Add("PmTransport", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("Item", typeof(String));
                myDataTable.Columns.Add("Boxes", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));
                myDataTable.Columns.Add("Cases", typeof(String));
                myDataTable.Columns.Add("BilltyNo", typeof(String));
                myDataTable.Columns.Add("GrandTotal", typeof(String));
                myDataTable.Columns.Add("BillDetail", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();

                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    row["Headername"] = "Packing Slip";
                    row["TotalQty"] = dba.ConvertObjectToDouble(lblTotalQty.Text).ToString("N0", MainPage.indianCurancy)
                        + " " + Convert.ToString(dr.Cells["unitName"].Value);
                    row["PartyName"] = txtSalesParty.Text;
                    row["SubPartyName"] = txtSubParty.Text;
                    string strSRCode = Convert.ToString(txtBillCode.Text) + " " + Convert.ToString(txtBillNo.Text);
                    row["SerialCode"] = strSRCode;
                    row["City"] = txtBStation.Text;
                    row["Remarks"] = txtRemark.Text;

                    row["Date"] = txtDate.Text;
                    row["PmTransport"] = txtPvtMarka.Text + " / " + txtTransport.Text;
                    row["BillDetail"] = "";
                    row["Cases"] = txtNoofCases.Text;
                    row["BilltyNo"] = txtLRNumber.Text;
                    row["GrandTotal"] = txtFinalAmt.Text;

                    row["Boxes"] = Convert.ToString(dr.Cells["boxRoll"].Value);
                    row["Item"] = Convert.ToString(dr.Cells["itemName"].Value);
                    row["Qty"] = dba.ConvertObjectToDouble(dr.Cells["qty"].Value).ToString("N0", MainPage.indianCurancy)
                                   + " " + Convert.ToString(dr.Cells["unitName"].Value);
                    myDataTable.Rows.Add(row);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return myDataTable;
        }

        private void btnPrintWayBill_Click(object sender, EventArgs e)
        {
            btnPrintWayBill.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                    {
                        if (txtWayBillNo.Text != "" && txtWayBIllDate.Text != "")
                        {
                            if (txtWayBIllDate.Text.Length == 19)
                            {
                                DataTable _dt = dba.CreateTradingWayBillDataTable(txtBillCode.Text, txtBillNo.Text);
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
                                txtWayBIllDate.Focus();
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
            catch { }
            btnPrintWayBill.Enabled = true;
        }

        private void btnHoldList_Click(object sender, EventArgs e)
        {
            pnlHold.Visible = !pnlHold.Visible;
            if (dgrdHold.Rows.Count > 0)
            {
                dgrdHold.CurrentCell = dgrdHold.Rows[0].Cells["hBillNo"];
                dgrdHold.Focus();
            }
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)")
                {
                    DialogResult _result = MessageBox.Show("Are you sure you want to hold this bill ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result == DialogResult.Yes)
                    {
                        HoldRecord();
                    }
                }
            }
            catch { }
        }

        private void dgrdHold_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double dFinalAmt = ConvertObjectToDouble(txtFinalAmt.Text);
                if (e.RowIndex >= 0 && e.ColumnIndex == 1)
                {
                    if (dFinalAmt > 0 && btnAdd.Text == "&Save(F5)")
                        HoldRecordReturnInt();

                    ClearAllText();
                    string strBillNo = Convert.ToString(dgrdHold.CurrentCell.Value);
                    strHoldBilLCode = ""; strHoldBillNo = "";
                    if (strBillNo != "")
                    {
                        string[] str = strBillNo.Split(' ');
                        if (str.Length > 1)
                        {
                            pnlHold.Visible = false;
                            txtBillCode.Text = str[0];
                            BindRecordWithControl(str[1]);
                            strHoldBilLCode = str[0];
                            strHoldBillNo = str[1];
                            //if (btnAdd.Text == "&Add(F2)")
                            //{
                            btnAdd.Text = "&Save(F5)";
                            btnEdit.Text = "&Edit(F6)";
                            EnableAllControls();
                            btnAdd.TabStop = true;
                            txtBillNo.ReadOnly = true;
                            chkEmail.Checked = chkSendSMS.Checked = false;
                            btnHold.Enabled = true;
                            SetSerialNo();
                            txtSalesParty.Focus();
                            //}                           
                        }
                    }
                }
                else if (e.RowIndex >= 0 && e.ColumnIndex == 4)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Delete this Hold Bill ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strNo = "", strBCode = "", strBNo = "";
                        strNo = Convert.ToString(dgrdHold.CurrentRow.Cells["hBillNo"].Value);
                        string[] str = strNo.Split(' ');
                        if (str.Length > 1)
                        {
                            strBCode = str[0].Trim();
                            strBNo = str[1].Trim();
                        }
                        string strQuery = " Delete from SalesBook where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        + " delete from SalesBookSecondary where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        + " delete from StockMaster where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'";

                        if (strQuery != "")
                        {
                            int Count = dba.ExecuteMyQuery(strQuery);
                            if (Count > 0)
                            {
                                MessageBox.Show("Thank you ! Record Deleted successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindLastRecord();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        }

        private void txtDiscAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dOtherPerAmt = 0, dOtherPer = 0, dGrossAmt = 0;
                dOtherPerAmt = ConvertObjectToDouble(txtDiscAmt.Text);
                if (dOtherPerAmt > 0)
                {
                    dGrossAmt = ConvertObjectToDouble(txtGrossAmt.Text);
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


        private void txtPetiType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PETITYPE", "SEARCH PETI TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPetiType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SetGridViewBackGroundColor()
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.Tomato)
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                }
            }
            catch { }
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

            if (_dt.Rows.Count > 0)
            {
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string strQuery = "", strCompanyCode = "", strHSNCode = "", strHSNQuery = "", strBrandName = "", strDesignName = "", strItemName = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "", strBarCode = "", strBarCode_S = "";
                double dAmt = 0, dGrossAmt = 0, dNetAmt = 0, dQty = 0, dTQty = 0, dRate = 0, _dDisPer, dMRP = 0, dSpclDisPer = 0, dSpclDisAmt = 0, dCardAmt = 0, dCashAmt = 0;
                dSpclDisPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);
                DataTable _dtCompany = _dt.DefaultView.ToTable(true, "CompanyCode");
                foreach (DataRow row in _dtCompany.Rows)
                {
                    strCompanyCode = Convert.ToString(row["CompanyCode"]);

                    DataRow[] _rows = _dt.Select("CompanyCode='" + strCompanyCode + "'");
                    int _index = 1;
                    dSpclDisAmt = dGrossAmt = dTQty = dNetAmt = 0;
                    strQuery = "";

                    foreach (DataRow _dr in _rows)
                    {
                        strBrandName = strDesignName = strItemName = strVariant1 = strVariant2 = strVariant3 = strVariant4 = strVariant5 = "";

                        dGrossAmt += dAmt = dba.ConvertObjectToDouble(_dr["amount"]);
                        dTQty += dQty = dba.ConvertObjectToDouble(_dr["qty"]);
                        dRate = ConvertObjectToDouble(_dr["rate"]);
                        _dDisPer = ConvertObjectToDouble(_dr["disPer"]);
                        dMRP = dba.ConvertObjectToDouble(_dr["mrp"]);

                        if (dSpclDisPer != 0 && dMRP != 0)
                            dSpclDisAmt += ((dMRP * dSpclDisPer) / 100.00) * dQty;

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
                            strBarCode_S = Convert.ToString(_dr["barcode_s"]);
                            strBrandName = Convert.ToString(_dr["brandName"]);
                            strDesignName = Convert.ToString(_dr["styleName"]);
                            strVariant1 = Convert.ToString(_dr["variant1"]);
                            strVariant2 = Convert.ToString(_dr["variant2"]);
                            strVariant3 = Convert.ToString(_dr["variant3"]);
                            strVariant4 = Convert.ToString(_dr["variant4"]);
                            strVariant5 = Convert.ToString(_dr["variant5"]);
                        }

                        strQuery += strHSNQuery + " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2],[BarCode_S]) VALUES "
                                 + " (@BillCode,@BillNo,0,'" + _dr["soNumber"] + "'," + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ","
                                 + " " + dAmt + ",0,0, " + dAmt + ",'" + _dr["unitName"] + "','" + MainPage.strLoginName + "','',1,0,'" + strBarCode + "','" + strBrandName + "','" + strDesignName
                                 + "','" + _dr["description"] + "','" + _dr["boxRoll"] + "','" + strBarCode_S + "')";

                        //if (MainPage._bTaxStatus || txtImportData.Text == "")
                        //{
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                             + " ('SALES',@BillCode,@BillNo, " + strItemName + ",'" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + strBarCode + "','" + strBrandName + "','" + strDesignName + "','','') ";
                        //}

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
                    dCardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                    if (strQuery != "")
                    {
                        string strBillNo = txtBillCode.Text + " " + txtBillNo.Text;
                        dAllNetAmt += Convert.ToDouble((dGrossAmt).ToString("0"));

                        result += _count = dba.SaveRecord_SaleBook(txtSalesParty.Text, strDate, strQuery, dGrossAmt, dMaxPer, dTTaxAmt, dSpclDisPer, dSpclDisAmt, dTQty, dNetAmt, txtLRNumber.Text, txtLRDate.Text, _dtTax, strCompanyCode, strBillNo, _bInclude, dCashAmt, dCardAmt, ref _strAttachBill, txtRemark.Text);
                        if (_strAttachBill != "")
                            _strAttachBillWithComma += _strAttachBill + ",";

                        if (_count > 0)
                        {
                            DialogResult _result = MessageBox.Show("Are you want to print Sale Bill ?", "Print Sale Service Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (_result == DialogResult.Yes)
                                GSTPrintAndPreview(true, "", false, false, strCompanyCode);
                        }
                    }
                }
            }
            else
                result = 1;

            if (_strAttachBillWithComma.Length > 0)
                txtAttachBill.Text = _strAttachBillWithComma.Substring(0, _strAttachBillWithComma.Length - 1);

            if (MainPage.bHSNWisePurchase)
                txtSaleAmt.Text = dAllNetAmt.ToString("N2", MainPage.indianCurancy);
            else
                txtSaleAmt.Text = "0.00";

            return result;
        }
    }
}
