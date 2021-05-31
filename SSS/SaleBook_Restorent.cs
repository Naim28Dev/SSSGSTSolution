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
    public partial class SaleBook_Restorent : Form
    {
        DataBaseAccess dba;
        double dOldNetAmt = 0;
        string strLastSerialNo = "", strOldPartyName = "", strSaleBillType = "", strHoldBilLCode = "", strHoldBillNo = "";
        Boolean bsalesman, bBarcode, bBrandname, bitemname, bqty, bamount, bcustomername, bmobile, bcity, bemail, bsms, blocation, bConfSmsSave;
        int isno = 0, isalesman = 0, ibarcode = 0, ibrandname = 0, istylename = 0, iitemname = 0, iqty = 0, iUOM = 0, iMRP = 0, iDis = 0, irate = 0, iamount = 0, iSMI = 0, istockqty = 0;

        SearchCategory_Custom objSearch;
        SearchData _objData;
        public SaleBook_Restorent()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
           // GetStartupData(true);
            GetCellSetup();

            btnAdd.Text = "&Save(F5)";
            btnEdit.Text = "&Edit(F6)";
            EnableAllControl();
            txtBillNo.ReadOnly = false;
            ClearText();
            SetSerialNo();
            SetFocusinGrid();
            dgrdDetails.Focus();
            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
            btnSavePrint.Enabled = true;
            btnAdd.TabStop = btnSavePrint.TabStop = true;
            btnHold.Enabled = true;
        }


        public SaleBook_Restorent(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtBillCode.Text = strSerialCode;
            GetStartupData(false);
            BindRecordWithControl(strSerialNo);
            GetCellSetup();
        }

        public void GetCellSetup()
        {
            try
            {
                string StrQuery = "select ID,columnName,IndexNo from RetailSaleBook_FormControl order by IndexNo   select ID,ColumnName,Mendatorystatus from RetailSaleBook_FormControl where MendatoryFields='Mendatory'";
                DataSet ds = DataBaseAccess.GetDataSetRecord(StrQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    DataTable dtMend = ds.Tables[1];
                    if (dt.Rows.Count > 0)
                    {
                        isno = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "S.No"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        isalesman = (from DataRow dr in dt.Rows
                                     where (string)dr["ColumnName"] == "SalesMan"
                                     select (int)dr["IndexNo"]).FirstOrDefault();

                        ibarcode = (from DataRow dr in dt.Rows
                                    where (string)dr["ColumnName"] == "BarCode"
                                    select (int)dr["IndexNo"]).FirstOrDefault();

                        ibrandname = (from DataRow dr in dt.Rows
                                      where (string)dr["ColumnName"] == "Brand Name"
                                      select (int)dr["IndexNo"]).FirstOrDefault();

                        istylename = (from DataRow dr in dt.Rows
                                      where (string)dr["ColumnName"] == "Style Name"
                                      select (int)dr["IndexNo"]).FirstOrDefault();

                        iitemname = (from DataRow dr in dt.Rows
                                     where (string)dr["ColumnName"] == "ItemName"
                                     select (int)dr["IndexNo"]).FirstOrDefault();

                        iqty = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "Qty"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        iUOM = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "UOM"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        iMRP = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "MRP"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        iDis = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "Dis(%)"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        irate = (from DataRow dr in dt.Rows
                                 where (string)dr["ColumnName"] == "Rate"
                                 select (int)dr["IndexNo"]).FirstOrDefault();

                        iamount = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "Amount"
                                   select (int)dr["IndexNo"]).FirstOrDefault();

                        int T1 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "CustomerName"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T1 == 0)
                        {
                            txtCustomerName.TabStop = false;
                        }
                        else
                        {
                            txtCustomerName.TabIndex = T1;
                        }

                        int T2 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "MobileNo."
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T2 == 0)
                        {
                            txtMobileNo.TabStop = false;
                        }
                        else
                        {
                            txtMobileNo.TabIndex = T2;
                        }

                        int T3 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "City"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T3 == 0)
                        {
                            txtRemark.TabStop = false;
                        }
                        else
                        {
                            txtRemark.TabIndex = T3;
                        }

                        int T4 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "CardDetail"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T4 == 0)
                        {
                            dgrdCardDetail.TabStop = false;
                            grpBoxCard.TabStop = false;
                        }
                        else
                        {
                            dgrdCardDetail.TabStop = true;
                            grpBoxCard.TabStop = true;
                            dgrdCardDetail.TabIndex = T4;
                            grpBoxCard.TabIndex = T4;
                        }

                        int T5 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "Location"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T5 == 0)
                        {
                            txtLocation.TabStop = false;
                        }
                        else
                        {
                            txtLocation.TabIndex = T5;
                        }

                        int T6 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "CardAmt"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T6 == 0)
                        {
                            txtCardAmt.TabStop = false;
                        }
                        else
                        {
                            txtCardAmt.TabIndex = T6;
                        }

                        int T7 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "CashAmt"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T7 == 0)
                        {
                            txtCashAmt.TabStop = false;
                        }
                        else
                        {
                            txtCashAmt.TabIndex = T7;
                        }

                        int ChqAmtTab = (from DataRow dr in dt.Rows
                                         where (string)dr["ColumnName"] == "ChequeAmt"
                                         select (int)dr["IndexNo"]).FirstOrDefault();
                        if (ChqAmtTab == 0)
                        {
                            txtChequeAmt.TabStop = false;
                        }
                        else
                        {
                            txtChequeAmt.TabIndex = ChqAmtTab;
                        }

                        int T8 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "SType"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T8 == 0)
                        {
                            txtSalesType.TabStop = false;
                        }
                        else
                        {
                            txtSalesType.TabIndex = T8;
                        }
                        int T9 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "OtherAmt"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T9 == 0)
                        {
                            txtSign.TabStop = false;
                            txtOtherAmount.TabStop = false;
                        }
                        else
                        {
                            txtSign.TabIndex = T9;
                            txtOtherAmount.TabIndex = T9;
                        }

                        int T10 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "TenderAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T10 == 0)
                        {
                            txtTenderAmt.TabStop = false;
                        }
                        else
                        {
                            txtTenderAmt.TabIndex = T10;
                        }

                        int T11 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "RefundAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T11 == 0)
                        {
                            txtRefundAmt.TabStop = false;
                        }
                        else
                        {
                            txtRefundAmt.TabIndex = T11;
                        }
                    }

                    if (dtMend.Rows.Count > 0)
                    {
                        bsalesman = (from DataRow dr in dtMend.Rows
                                     where (string)dr["ColumnName"] == "SalesMan"
                                     select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bBarcode = (from DataRow dr in dtMend.Rows
                                    where (string)dr["ColumnName"] == "BarCode"
                                    select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bBrandname = (from DataRow dr in dtMend.Rows
                                      where (string)dr["ColumnName"] == "BrandName"
                                      select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bitemname = (from DataRow dr in dtMend.Rows
                                     where (string)dr["ColumnName"] == "ItemName"
                                     select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bqty = (from DataRow dr in dtMend.Rows
                                where (string)dr["ColumnName"] == "Qty"
                                select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bamount = (from DataRow dr in dtMend.Rows
                                   where (string)dr["ColumnName"] == "Amount"
                                   select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bcustomername = (from DataRow dr in dtMend.Rows
                                         where (string)dr["ColumnName"] == "CustomerName"
                                         select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bmobile = (from DataRow dr in dtMend.Rows
                                   where (string)dr["ColumnName"] == "MobileNo."
                                   select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bcity = (from DataRow dr in dtMend.Rows
                                 where (string)dr["ColumnName"] == "City"
                                 select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bemail = (from DataRow dr in dtMend.Rows
                                  where (string)dr["ColumnName"] == "Email"
                                  select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        bsms = (from DataRow dr in dtMend.Rows
                                where (string)dr["ColumnName"] == "SMS"
                                select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();

                        blocation = (from DataRow dr in dtMend.Rows
                                     where (string)dr["ColumnName"] == "Location"
                                     select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();
                        bConfSmsSave = (from DataRow dr in dtMend.Rows
                                        where (string)dr["ColumnName"] == "Confirmation Message on Save"
                                        select (Boolean)dr["MendatoryStatus"]).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select SBillCode,(Select ISNULL(MAX(BillNo),0) from SalesBook Where BillCode=SBillCode)SerialNo,(Select Top 1 Layout from PrintLayoutMaster) as Layout,(Select TOP 1 TaxName from SaleTypeMaster Where SaleType='SALES' and TaxIncluded=1 and Region='LOCAL')TaxName from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (txtBillCode.Text == "")
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["SBillCode"]);
                    strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    txtSalesType.Text = Convert.ToString(dt.Rows[0]["TaxName"]);
                }
                MainPage.strPrintLayout = Convert.ToString(dt.Rows[0]["Layout"]);

                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Sale Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        //private void SetCategory()
        //{
        //    try
        //    {
        //        if (MainPage.StrCategory1 != "")
        //        {
        //            dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
        //            dgrdDetails.Columns["variant1"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant1"].Visible = false;

        //        if (MainPage.StrCategory2 != "")
        //        {
        //            dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
        //            dgrdDetails.Columns["variant2"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant2"].Visible = false;

        //        if (MainPage.StrCategory3 != "")
        //        {
        //            dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
        //            dgrdDetails.Columns["variant3"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant3"].Visible = false;

        //        if (MainPage.StrCategory4 != "")
        //        {
        //            dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
        //            dgrdDetails.Columns["variant4"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant4"].Visible = false;

        //        if (MainPage.StrCategory5 != "")
        //        {
        //            dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
        //            dgrdDetails.Columns["variant5"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant5"].Visible = false;
        //    }
        //    catch
        //    {
        //    }
        //}

        private Control SetFocusOnNextPanel()
        {
            Control focused = FindFocusedControl(this);
            if (focused is TextBox)
            {
                var vControl = ((TextBox)focused).Parent;
                if (vControl is Panel)
                {
                    if (vControl.Name == "pnlTop")
                    {
                        focused = pnlMiddle;
                        pnlMiddle.Focus();
                    }
                    else if (vControl.Name == "pnlMiddle")
                    {
                        focused = pnlLast;
                        pnlLast.Focus();
                    }
                    else if (vControl.Name == "pnlLast")
                    {
                        focused = pnlButton;
                        pnlButton.Focus();
                    }
                    this.GetNextControl(focused, true).Focus();
                }
            }
            else if (focused is DataGridView)
            {
                var vControl = ((DataGridView)focused).Parent;
                if (vControl is Panel)
                {
                    if (vControl.Name == "pnlTop")
                    {
                        focused = pnlMiddle;
                        pnlMiddle.Focus();
                    }
                    else if (vControl.Name == "pnlMiddle")
                    {
                        focused = pnlLast;
                        pnlLast.Focus();
                    }
                    else if (vControl.Name == "pnlLast")
                    {
                        focused = pnlButton;
                        pnlButton.Focus();
                    }
                    this.GetNextControl(focused, true).Focus();
                }
                else if (vControl is GroupBox)
                {
                    if (vControl.Name == "pnlTop")
                    {
                        focused = pnlMiddle;
                        pnlMiddle.Focus();
                        this.GetNextControl(focused, true).Focus();
                    }
                    else if (vControl.Name == "pnlMiddle")
                    {
                        focused = pnlLast;
                        pnlLast.Focus();
                        this.GetNextControl(focused, true).Focus();
                    }
                    else if (vControl.Name == "grpBoxCard")
                    {
                        SelectNextControl(dgrdCardDetail, true, true, true, true);
                    }
                    else if (vControl.Name == "pnlLast")
                    {
                        focused = pnlButton;
                        pnlButton.Focus();
                        this.GetNextControl(focused, true).Focus();
                    }
                }
            }
            return focused;

        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return false;
            }
            return base.ProcessDialogKey(keyData);
        }


        private void SaleBook_Restorent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlHold.Visible)
                    pnlHold.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                Control clt = SetFocusOnNextPanel();
                //this.GetNextControl(clt, true).Focus();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused && !dgrdCardDetail.Focused)
                SelectNextControl(ActiveControl, true, true, true, true);// this.GetNextControl(ActiveControl, true).Focus();
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.T)
            {
                txtTenderAmt.Focus();
            }
            //else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.A)
            //{
            //    btnAltSlip.PerformClick();
            //}
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.B)
            {
                btnSavePrint.PerformClick();
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Z)
            {
                txtCardAmt.Focus();
            }
            else if (e.KeyCode == Keys.F2 && btnAdd.Text == "&Add(F2)")
            {
                btnAdd.PerformClick();
            }
            else if (e.KeyCode == Keys.F6)
            {
                btnEdit.PerformClick();
            }
            else if (e.KeyCode == Keys.F8)
            {
                btnDelete.PerformClick();
            }
            else if (e.KeyCode == Keys.F5 && btnAdd.Text == "&Save(F5)")
            {
                btnAdd.PerformClick();
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
            //else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            //{
            //    btnCreatePDF.PerformClick();
            //}
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F)
            {
                btnSaleSetup.PerformClick();
            }
            else
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && MainPage.mymainObject.bSaleView)
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

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "' ");//[SaleBillType]='RETAIL' and 
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearText();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  ");//[SaleBillType]='RETAIL' and 
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearText();
        }

        private void BindNextRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  and BillNo>" + txtBillNo.Text + " ");//[SaleBillType]='RETAIL' and
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " "); //[SaleBillType]='RETAIL' and 
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
                strOldPartyName = "";
                string strQuery = " Select *,ISNULL((SalePartyID+' '+SName),SalePartyID) SParty,CONVERT(varchar,Date,103)BDate,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SB.Date))) LockType from SalesBook SB OUTER APPLY (Select Top 1 SM.Name as SName,NormalDhara from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.SalePartyID)SM1    Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "    Select SBS.*,ISNULL(dbo.GetFullName(SBS.SalesMan),'DIRECT')SalesManName from SalesBookSecondary SBS Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " order by SID  "
                                + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                + " Select * from dbo.[CardDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                + " select (RefundableAmt+AdjustedAmt)Amount from AdvanceAdjustment where BillCode+' '+cast(BillNo as varchar)=(select AdvanceSlipNo from SalesBook where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + ")"
                                + " Select (BillCode+' '+CAST(BillNo as varchar))SaleBillNo,CONVERT(varchar,Date,103)Date,ISNULL(dbo.GetFullName(SalePartyID),SalePartyID) SalePartyID from SalesBook WHere SaleBillType='RETAIL_HOLD' Order by BillNo desc ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                DisableAllControl();
                txtReason.Text = "";
                pnlDeletionConfirmation.Visible = false;
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = "";
                btnSavePrint.Enabled = btnSavePrint.TabStop = false;
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
                            txtCustomerName.Text = strOldPartyName = Convert.ToString(row["SParty"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtLocation.Text = Convert.ToString(row["MaterialLocation"]);
                            txtSalesType.Text = Convert.ToString(row["SalesType"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                            txtDiscPer.Text = Convert.ToString(row["DisPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DisAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);
                            txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                            txtImportData.Text = Convert.ToString(row["Description_3"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                            txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                            txtAdvanceAmt.Text = ConvertObjectToDouble(row["AdvanceAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtReturnAmt.Text = ConvertObjectToDouble(row["ReturnAmt"]).ToString("N2", MainPage.indianCurancy);

                            double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0;
                            dCardAmt = ConvertObjectToDouble(row["CardAmt"]);
                            dCashAmt = ConvertObjectToDouble(row["CashAmt"]);
                            dCreditAmt = ConvertObjectToDouble(row["CreditAmt"]);

                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            txtOfrDisAmt.Text = ConvertObjectToDouble(row["OfferDisAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtOfrDisPer.Text = ConvertObjectToDouble(row["OfferDisPer"]).ToString("N2", MainPage.indianCurancy);

                            txtCardAmt.Text = dCardAmt.ToString("N2", MainPage.indianCurancy);
                            txtCashAmt.Text = dCashAmt.ToString("N2", MainPage.indianCurancy);
                            txtNetAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);

                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);
                            txtTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            txtGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtFinalAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                            double dTenderAmt = ConvertObjectToDouble(row["TenderAmt"]);
                            txtTenderAmt.Text = dTenderAmt.ToString("N2", MainPage.indianCurancy);
                            if (dTenderAmt > 0)
                                txtRefundAmt.Text = ConvertObjectToDouble(row["RefundAmt"]).ToString("N2", MainPage.indianCurancy);
                            else
                                txtRefundAmt.Text = "0.00";
                            if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                                btnEdit.Enabled = btnDelete.Enabled = false;
                            else
                            {
                                if (!MainPage.mymainObject.bSaleEdit)
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
                    BindSalesBookDetails(ds.Tables[1]);
                    BindGSTDetailsWithControl(ds.Tables[2]);
                    BindCardDetailsWithControl(ds.Tables[3]);
                    //BindAdvanceSlipAmt(ds.Tables[4]);
                    BindHoldDetailsWithControl(ds.Tables[5]);
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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
                    dgrdHold.Rows[rowIndex].Cells["tableNo"].Value = row["SalePartyID"];
                    rowIndex++;
                }
                btnHoldList.BackColor = Color.DarkGreen;
            }
        }

        private void BindSalesBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["OrderNo"].Value = row["Other1"];
                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["salesMan"].Value = row["SalesManName"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];

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


        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtCashAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                    {
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3)
                    {
                        _objData = new SearchData("WAITERNAME", "SEARCH WAITER NAME", Keys.Space);
                        _objData.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        _objData = new SearchData("TABLENO", "SEARCH TABLE NO", Keys.Space);
                        _objData.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                        if (_objData.strSelectedData != "")
                            dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells[5];
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 8)
                    {
                        string strBrandName = Convert.ToString(dgrdDetails.CurrentRow.Cells["brandName"].Value);
                        if (strBrandName != "")
                        {
                            objSearch = new SearchCategory_Custom("", "SALEITEMS_RESTO", strBrandName, "", "", "", "", "", "", Keys.Space, false, false, "ItemName");
                            objSearch.ShowDialog();

                            GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);

                            ArrangeSerialNo();
                            CalculateAllAmount();
                        }
                        else
                        {
                            MessageBox.Show("Please select a table number first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells[4];
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 7 || e.ColumnIndex == 9 || e.ColumnIndex == 10)
                    {
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            e.Cancel = true;
                    }
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
            }
        }

        public DateTime ConvertDateInExactFormat(string strDate)
        {
            if (strDate.Length == 10)
            {
                DateTime date = DateTime.ParseExact(strDate, "dd/MM/yyyy", MainPage.indianCurancy);
                return date;
            }
            else
                return new DateTime(1900, 01, 01);
        }

        private void GetAllDesignSizeColorWithBarCode(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {
                if (objCategory != null)
                {
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        List<string> strData = objCategory.strSelectedRows;
                        {
                            dgrdDetails.Rows.Clear();
                            int index = 1;
                            foreach (string strList in strData)
                            {
                                string[] strAllItem = strList.Split('|');
                                if (strAllItem.Length > 0)
                                {
                                    dgrdDetails.Rows.Add(1);
                                    DataGridViewRow nr = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1];
                                    //if (!checkDuplicateEntry(strAllItem))
                                    {
                                        nr.Cells["OrderNo"].Value = strAllItem[0];
                                        nr.Cells["salesMan"].Value = strAllItem[1];
                                        nr.Cells["brandName"].Value = strAllItem[2];
                                        nr.Cells["itemName"].Value = strAllItem[3];
                                        nr.Cells["styleName"].Value = strAllItem[4];
                                        nr.Cells["qty"].Value = strAllItem[5];
                                        nr.Cells["rate"].Value = strAllItem[6];
                                        nr.Cells["unitName"].Value = strAllItem[7];
                                        nr.Cells["amount"].Value = strAllItem[8];
                                    }
                                }
                                //if (strData.Count == index)
                                //{
                                //    dgrdDetails.Rows.Add(1);
                                //    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                //    if (dgrdDetails.Rows.Count > 1)
                                //        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                //    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["brandName"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["brandName"].Value;
                                //    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                                //    dgrdDetails.Focus();
                                //}
                                index++;
                            }
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                            dgrdDetails.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool checkDuplicateEntry(string[] strAllItem)
        {
            bool isFound = false;
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (dgrdDetails.CurrentRow != row)
                    {
                        if (Convert.ToString(row.Cells["OrderNo"].Value) == strAllItem[0] && Convert.ToString(row.Cells["brandName"].Value) == strAllItem[2] && Convert.ToString(row.Cells["ItemName"].Value) == strAllItem[3])
                        {
                            MessageBox.Show("Sorry! Duplicate item entry with same order number on same table is not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                    }
                }
            }
            return isFound;
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

        private void ArrangeCardSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdCardDetail.Rows)
            {
                row.Cells["cSNo"].Value = serialNo;
                serialNo++;
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

        //private void GetSaleRate(DataGridViewRow row)
        //{
        //    try
        //    {

        //        string strBrandName = Convert.ToString(row.Cells["brandName"].Value), strItemName = Convert.ToString(row.Cells["itemName"].Value), strVariant1 = Convert.ToString(row.Cells["variant1"].Value), strVariant2 = Convert.ToString(row.Cells["variant2"].Value);

        //        DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);

        //        string strQuery = "";
        //        DataTable dt = dba.GetDataTable(strQuery);
        //        if (dt.Rows.Count > 0)
        //        {
        //            DataRow _row = dt.Rows[0];

        //            row.Cells["mrp"].Value = dba.ConvertObjectToDouble(_row["SaleMRP"]);
        //            row.Cells["rate"].Value = _row["SaleRate"];
        //            row.Cells["styleName"].Value = _row["DesignName"];
        //            row.Cells["unitName"].Value = _row["UnitName"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

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

                        if (IndexColmn < dgrdDetails.ColumnCount - 1)
                        {
                            IndexColmn += 1;
                            if (CurrentRow >= 0)
                            {
                                if (dgrdDetails.Columns[IndexColmn].Visible)
                                {
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                                }
                            }

                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value) != "" && IndexColmn == 11)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                if (dgrdDetails.Rows.Count > 1)
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                dgrdDetails.Focus();
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "" && (dAmt > 0))
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                if (dgrdDetails.Rows.Count > 1)
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtCustomerName.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1) //&& btnAdd.Text == "&Save(F5)"
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        if (strID == "")
                        {
                            //foreach (DataGridViewRow dr in dgrdDetails.Rows)
                            //{
                            dgrdDetails.Rows.Clear();

                            //dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            //if (dgrdDetails.Rows.Count == 0)
                            //{
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                            dgrdDetails.Enabled = true;
                            //}
                            //else
                            //{
                            //    ArrangeSerialNo();
                            //}
                            //}
                            CalculateAllAmount();
                        }
                        else
                        {
                            //DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            //if (result == DialogResult.Yes)
                            //{
                            //    DeleteOneRow(strID, CurrentRow);
                            //}
                        }
                    }
                    //else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update(F6)")
                    //{
                    //    string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                    //    if (strID == "")
                    //    {
                    //        dgrdDetails.Rows.Clear();
                    //        //dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                    //        //if (dgrdDetails.Rows.Count == 0)
                    //        //{
                    //        //    dgrdDetails.Rows.Add(1);
                    //        //    dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    //        //    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
                    //        //    dgrdDetails.Enabled = true;
                    //        //}
                    //        //else
                    //        //{
                    //        //    ArrangeSerialNo();
                    //        //}
                    //        CalculateAllAmount();
                    //    }
                    //    else
                    //    {
                    //        //DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //        //if (result == DialogResult.Yes)
                    //        //{
                    //        //    DeleteOneRow(strID, CurrentRow);
                    //        //    if (dgrdDetails.Rows.Count == 0)
                    //        //    {
                    //        //        dgrdDetails.Rows.Add(1);
                    //        //        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    //        //        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
                    //        //        dgrdDetails.Enabled = true;
                    //        //    }
                    //        //}
                    //    }
                    //}
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 3)
                            dgrdDetails.CurrentCell.Value = "";
                    }
                }
                else
                {
                    //  dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                }
            }
            catch (Exception ex)
            { }
        }

        private void DeleteOneRow(string strID, int rowindex)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    if (strHoldBilLCode != "" && strHoldBillNo != "")
                    {
                        txtBillCode.Text = strHoldBilLCode;
                        txtBillNo.Text = strHoldBillNo;
                    }

                    string strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and [SID]=" + strID + " ";

                    DataGridViewRow row = dgrdDetails.Rows[rowindex];
                    strQuery += " UPDATE Res_SalesStock set Status = 'PENDING' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";

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
                            strQuery = " Delete from SalesBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " ";
                            strQuery += " UPDATE Res_SalesStock set Status = 'PENDING' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";

                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
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

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    if (e.ColumnIndex == 7)
                    {
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    }
                    else if (e.ColumnIndex == 9)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                }
            }
            catch
            {
            }
        }

        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            //if (dQty > dSQty)
            //{
            //    lblMsg.Text = "Total stock qty : " + dSQty.ToString() + ", You can't sale more than that.";
            //    lblMsg.ForeColor = Color.Red;
            //    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
            //    {
            //        rows.Cells["qty"].Value = dSQty;
            //        dQty = dSQty;
            //    }
            //}
            //else
            //    lblMsg.Text = "";

            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void CalculateAmountWithMRP(DataGridViewRow rows)
        {
            double dRate = 0;
            if (rows != null)
            {
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        //private void CalculateDisWithAmountMRP(DataGridViewRow rows)
        //{

        //    double dDisPer = 0, dRate = 0;
        //    if (rows != null)
        //    {
        //        dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);
        //        rows.Cells["disPer"].Value = dDisPer * -1;
        //        double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
        //        dAmt = dQty * dRate;

        //        rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

        //        CalculateAllAmount();
        //    }
        //}

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
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, _dMRP = 0, dAmt = 0, dRate = 0, dQty = 0, dOfferDisPer = 0, dOfferDisAmt = 0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);

                    dOfferDisPer = dba.ConvertObjectToDouble(row.Cells["offerDisPer"].Value);
                    dOfferDisAmt = dba.ConvertObjectToDouble(row.Cells["discAmt"].Value);

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                }
                txtSplDisAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
            }
        }

        private void CalculateAllAmount()
        {
            try
            {
                CalculateSpecialDiscount();

                double dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dNetAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dRoundOff = 0, dPaybleAmt = 0;
                double dDisPer = 0, dCardAmt = 0, dCashAmt = 0, dChequeAmt = 0, dCreditAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0;

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dChequeAmt = dba.ConvertObjectToDouble(txtChequeAmt.Text);

                if (btnEdit.Text == "&Update(F6)")
                {
                    dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                }
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);

                dDisPer = ConvertObjectToDouble(txtDiscPer.Text);
                dDiscAmt = ConvertObjectToDouble(txtDiscAmt.Text);

                dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                dTOAmt = dOtherAmt;
                dFinalAmt = dBasicAmt - dDiscAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, dDiscAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTOAmt;

                dPaybleAmt = dNetAmt - dAdvanceAmt - dReturnAmt - dCardAmt - dCashAmt - dCreditAmt - dChequeAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));
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

                dPaybleAmt = Math.Round(dPaybleAmt, 0);
                txtTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                txtFinalAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtNetAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);
                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

                //if (!chkTenderAmt.Checked)
                //{
                //    txtTenderAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);
                //    txtRefundAmt.Text = "0.00";
                //}
                //else
                //{
                double dTenderAmt = 0, dRefundAmt = 0;
                dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                dRefundAmt = dTenderAmt - dPaybleAmt;
                txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                //}
            }
            catch
            {
            }
        }

        private void CalculateAllAmountFinal()
        {
            try
            {
                CalculateSpecialDiscount();

                double dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dNetAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dRoundOff = 0, dPaybleAmt = 0;
                double dDisPer = 0, dCardAmt = 0, dCashAmt = 0, dChequeAmt = 0, dCreditAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0, dTenderAmt = 0;

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);

                dChequeAmt = dba.ConvertObjectToDouble(txtChequeAmt.Text);

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                if (btnAdd.Text == "&Save")
                {
                    if (dTenderAmt > dNetAmt)
                        txtCashAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        txtCashAmt.Text = dTenderAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    if (dTenderAmt > dNetAmt)
                    {
                        dCashAmt += dNetAmt;
                        txtCashAmt.Text = dCashAmt.ToString("N2", MainPage.indianCurancy);
                    }
                    else
                    {
                        dCashAmt += dTenderAmt;
                        txtCashAmt.Text = dCashAmt.ToString("N2", MainPage.indianCurancy);
                    }
                }

                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);

                dDisPer = ConvertObjectToDouble(txtDiscPer.Text);
                dDiscAmt = ((dBasicAmt - dReturnAmt) * dDisPer) / 100;

                dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                dTOAmt = dOtherAmt;
                dFinalAmt = dBasicAmt - dDiscAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, dDiscAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTOAmt;

                dPaybleAmt = dNetAmt - dAdvanceAmt - dReturnAmt - dCardAmt - dCashAmt - dCreditAmt - dChequeAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));
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

                dPaybleAmt = Math.Round(dPaybleAmt, 0);
                txtTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                txtFinalAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtNetAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);
                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
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

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0 ";
                                }
                            }
                            strTaxRate = "0";

                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount," + strTaxRate + " as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(TaxableAmt)TaxableAmt,SUM(ROUND(Amt,4)) as Amt,SUM(ROUND(Amt,2)) as TaxAmt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) OtherChargesAmt from (Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' and Qty>0 then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by HSNCode,TaxRate)_Sales  Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    if (dt.Rows.Count > 0)
                                        dOtherChargesAmt = dba.ConvertObjectToDouble(dt.Rows[0]["OtherChargesAmt"]);
                                    dTaxAmt = dTTaxAmt;
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
                if (MainPage.strCompanyName.Contains("SARAOGI"))
                {
                    txtSign.Text = "+";
                    txtOtherAmount.Text = dServiceAmount.ToString("N2", MainPage.indianCurancy);
                }
            }

            if (_strTaxType == "INCLUDED")
                dTaxAmt = dOtherChargesAmt;
            return dTaxAmt;
        }

        private void CalculateCardAmount()
        {
            try
            {
                double dAmt = 0;
                foreach (DataGridViewRow row in dgrdCardDetail.Rows)
                    dAmt += ConvertObjectToDouble(row.Cells["cAmt"].Value);

                txtCardAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                CalculateAllAmount();
            }
            catch { }
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

        private void dgrdCardDetail_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellFocusColor(sender, e);
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

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    string strCName = txtCustomerName.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {

                            txtCustomerName.Text = objSearch.strSelectedData;
                            string strMobileNo = "", strStation = "";
                            bool _bStatus = dba.CheckTransactionLockWithMobileNoStation(txtCustomerName.Text, ref strMobileNo, ref strStation);
                            if (_bStatus)
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtCustomerName.Text = txtMobileNo.Text = "";
                            }
                            else
                            {
                                if (strMobileNo != "" || strStation != "")
                                {
                                    txtMobileNo.Text = strMobileNo;
                                }
                            }

                            if (txtCustomerName.Text != strCName)
                            {
                                txtAdvanceAmt.Text = txtReturnAmt.Text = "0.00";
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

        private void dgrdPendingAdv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = true;
            }
        }

        private void dgrdReturnSlip_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = true;
            }
        }

        private void ClearText()
        {
            lblCreatedBy.Text = txtCustomerName.Text = strHoldBilLCode = strHoldBillNo = txtMobileNo.Text = txtRemark.Text = txtRemark.Text = txtLocation.Text = txtChqSrNo.Text = "";

            txtCardAmt.Text = lblTaxableAmt.Text = txtCashAmt.Text = txtOtherAmount.Text = txtTotalQty.Text = txtAdvanceAmt.Text = txtReturnAmt.Text = txtSpclDisPer.Text = txtSplDisAmt.Text = txtTaxPer.Text = txtTaxAmt.Text = txtGrossAmt.Text = txtDiscPer.Text = txtDiscAmt.Text = txtRoundOff.Text = txtFinalAmt.Text = txtNetAmt.Text = txtTenderAmt.Text = txtRefundAmt.Text = txtChequeAmt.Text = txtOfrDisAmt.Text = txtOfrDisPer.Text = "0.00";
            txtSign.Text = txtROSign.Text = "+";
            pnlHold.Visible = false;
            dgrdCardDetail.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdCardDetail.Rows.Add();
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;

            if (DateTime.Today > MainPage.startFinDate && DateTime.Today <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void EnableAllControl()
        {
            txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtSpclDisPer.ReadOnly = txtDiscPer.ReadOnly = false;
            txtReturnAmt.ReadOnly = txtAdvanceAmt.ReadOnly = txtTenderAmt.ReadOnly = txtChequeAmt.ReadOnly = txtCardAmt.ReadOnly = txtCashAmt.ReadOnly = false;

            dgrdDetails.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtSpclDisPer.ReadOnly = txtDiscPer.ReadOnly = true;
            txtReturnAmt.ReadOnly = txtAdvanceAmt.ReadOnly = txtTenderAmt.ReadOnly = txtChequeAmt.ReadOnly = txtCardAmt.ReadOnly = true;
            txtCashAmt.ReadOnly = true;
            dgrdDetails.ReadOnly = true;
            lblMsg.Text = lblCreatedBy.Text = "";
        }

        private void SetSerialNo()
        {
            DataTable table = DataBaseAccess.GetDataTableRecord("Declare @BillCode nvarchar(250); Select @BillCode=SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select  @BillCode as SBillCode, (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode=@BillCode )SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='SALES' and TaxIncluded=1) TaxName  from SalesRecord Where BillCode=@BillCode )Sales ");
            if (table.Rows.Count > 0)
            {
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
            if (txtDate.Text == "")
            {
                MessageBox.Show("Sorry ! Date can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtLocation.Text == "" && blocation)
            {
                MessageBox.Show("Sorry ! Please enter location ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return false;
            }
            if (txtMobileNo.Text == "" && bmobile)
            {
                MessageBox.Show("Sorry ! Please enter register Mobile No. ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMobileNo.Focus();
                return false;
            }

            if (txtCustomerName.Text == "" && bcustomername)
            {
                MessageBox.Show("Sorry ! Please enter register customer name for cedit sale ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            double dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text), dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text), dCreditSale = dba.ConvertObjectToDouble(txtRefundAmt.Text);

            if (dCardAmt > 0 || dCashAmt > 0)
            {
                string strQuery = "Select (Select Top 1 (AreaCode+AccountNo)AccountNo from SupplierMaster Where Category='CARD SALE')CardSale,(Select Top 1 (AreaCode+AccountNo)AccountNo from SupplierMaster Where Category='CASH SALE')CashSale ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (dCardAmt > 0 && Convert.ToString(dt.Rows[0]["CardSale"]) == "")
                    {
                        MessageBox.Show("Sorry ! Please create account with 'CARD SALE' as category in account master! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (dCashAmt > 0 && Convert.ToString(dt.Rows[0]["CashSale"]) == "")
                    {
                        MessageBox.Show("Sorry ! Please create account with 'CASH SALE' as category in account master! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;

            string strSalesMan = "", strItem = "";

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                strItem = Convert.ToString(rows.Cells["itemName"].Value);
                strSalesMan = Convert.ToString(rows.Cells["salesMan"].Value);

                double dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);
                if (strItem == "" && dAmount == 0)
                    dgrdDetails.Rows.Remove(rows);
                else
                {
                    if (bsalesman && strSalesMan == "")
                    {
                        MessageBox.Show("Sorry ! Sales man name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["salesMan"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    if (strItem == "" && bitemname)
                    {
                        MessageBox.Show("Sorry ! Item Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    else if (dAmount == 0 && bamount)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["qty"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }
            CalculateAllAmountFinal();

            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                MessageBox.Show("Sorry ! Please add atleast one entry in table ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
            {
                string strCardType = Convert.ToString(rows.Cells["cCardType"].Value);
                double dAmount = ConvertObjectToDouble(rows.Cells["cAmt"].Value);
                if (strCardType == "" && dAmount == 0)
                    dgrdCardDetail.Rows.Remove(rows);
                else
                {
                    if (strCardType == "")
                    {
                        MessageBox.Show("Sorry ! Card Type can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdCardDetail.CurrentCell = rows.Cells["cCardType"];
                        dgrdCardDetail.Focus();
                        return false;
                    }
                    else if (dAmount == 0)
                    {
                        MessageBox.Show("Sorry ! Card Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdCardDetail.CurrentCell = rows.Cells["cAmt"];
                        dgrdCardDetail.Focus();
                        return false;
                    }
                }
            }
            return true;
        }



        //private bool ValidateStock()
        //{
        //    if (!MainPage._bTaxStatus && txtImportData.Text != "")
        //        return true;
        //    else
        //    {
        //        DataTable _dt = GenerateDistinctItemName();
        //        bool _bStatus = dba.CheckQtyAvalability(_dt, txtBillCode.Text, txtBillNo.Text, dgrdDetails, lblMsg);
        //        if (!_bStatus && MainPage.strUserRole.Contains("SUPERADMIN"))
        //            _bStatus = true;
        //        return _bStatus;
        //    }
        //}

        private DataTable GenerateDistinctItemName()
        {
            DataTable _dt = new DataTable();
            try
            {
                _dt.Columns.Add("ItemName", typeof(String));
                _dt.Columns.Add("BrandName", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("ItemName='" + row.Cells["itemName"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]), dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                        _rows[0]["Qty"] = dOQty + dQty;
                    }
                    else
                    {
                        DataRow _row = _dt.NewRow();
                        _row["ItemName"] = row.Cells["itemName"].Value;
                        _row["BrandName"] = row.Cells["brandName"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
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
                        MessageBox.Show("This Bill No is already in use please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void SetFocusinGrid()
        {
            if (isno == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["srNo"];
            else if (isalesman == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
            else if (ibrandname == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
            else if (istylename == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["styleName"];
            else
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)")
                {
                    if (btnEdit.Text == "&Update(F6)")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Save(F5)";
                    btnEdit.Text = "&Edit(F6)";
                    EnableAllControl();
                    txtBillNo.ReadOnly = false;
                    ClearText();
                    SetSerialNo();
                    SetFocusinGrid();
                    dgrdDetails.Focus();
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
                    btnSavePrint.Enabled = true;
                    btnAdd.TabStop = btnSavePrint.TabStop = true;
                    btnHold.Enabled = true;
                }
                else if (ValidateControls() && CheckBillNoAndSuggest() && ValidateOtherValidation(false))
                {
                    double dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                    string strQuery = "SELECT CUSTOMERNAME FROM(Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as CUSTOMERNAME,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','CASH A/C'))Sales WHERE CUSTOMERNAME = '" + txtCustomerName.Text + "'";
                    DataTable dt = dba.GetDataTable(strQuery);
                    bool isRegistered = (dt.Rows.Count > 0);

                    if ((dNetAmt > 0 && !isRegistered))
                    {
                        DialogResult result = MessageBox.Show("No Tender amount found! do you want save this bill as hold ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            HoldRecord();
                        }
                    }
                    else
                    {
                        if (bConfSmsSave == true)
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                SaveRecord();
                            }
                        }
                        else
                            SaveRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void txtTaxAmt_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = true;
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private int SaveRecordReturnInt()
        {
            int _count = 0;
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");
                bool _registeredParty = false;
                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dFinalAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0, dFOtherAmt = 0;
                string strSaleParty = "", strSalePartyID = "", strSubPartyID = "", strPetiAgent = "DIRECT";
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                if (strCustomer != "")
                {
                    string[] _strFullName = txtCustomerName.Text.Split(' ');
                    if (_strFullName.Length > 1)
                    {
                        strSalePartyID = _strFullName[0].Trim();
                        strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                        _registeredParty = true;
                    }
                }
                else
                    strSalePartyID = strSaleParty = txtCustomerName.Text;

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);
                dFOtherAmt = _dOtherAmt;
                if (txtSign.Text == "-")
                    dFOtherAmt = (dFOtherAmt) * -1;

                dFinalAmt = dGrossAmt + _dOtherAmt - dDisc;

                string strQuery = " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + "  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ") begin "
                                + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[MobileNo],[AdvanceSlipNo],[AdvanceAmt],[ReturnSlipNo],[ReturnAmt],[CardAmt],[CashAmt],[CreditAmt],[SaleBillType],[MaterialLocation],[TenderAmt],[RefundAmt],[ChequeAmt],[ChequeSerialNo],[OfferApplied],[GrossProfit],[OfferDisPer],[OfferDisAmt],[TaxableAmt]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','','','','','0',''," + strLRDate + ",'','','" + txtRemark.Text + "','-',''," + strPDate + ",'','', " + dba.ConvertObjectToDouble(txtDiscPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",0,'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",0,0,'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ","
                                + " " + dba.ConvertObjectToDouble(txtTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + "," + dAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strPetiAgent + "','','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",'" + txtMobileNo.Text + "',''," + dAdvanceAmt + ",''," + dReturnAmt + "," + dCardAmt + "," + dCashAmt + ", " + dCreditAmt + ",'RETAIL','" + txtLocation.Text + "','" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + "','" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + "','" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + "','" + txtChqSrNo.Text + "','" + chkOfferApply.Checked + "','" + chkGrossProfit.Checked + "'," + dba.ConvertObjectToDouble(txtOfrDisPer.Text) + "," + dba.ConvertObjectToDouble(txtOfrDisAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ") ";

                if (_registeredParty && strSalePartyID != "")
                {
                    strQuery += " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','DR','" + dAmt + "','0','FALSE','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  ";
                }

                if (dCardAmt > 0)
                {
                    if (_registeredParty && strSalePartyID != "")
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE' "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "',@CardName,'CARD RECEIVE','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'" + strSalePartyID + "')  "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "','" + strSaleParty + "','CARD RECEIVE','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dFinalAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CardName)  ";

                    }
                    else
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE' "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CardName,'SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'CARD SALE')  ";
                    }
                }

                if (dCashAmt > 0)
                {
                    if (_registeredParty && strSalePartyID != "")
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "',@CashName,'CASH RECEIVE','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "','" + strSaleParty + "','CASH RECEIVE','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dFinalAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName)  ";

                    }
                    else
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + strDate + "',@CashName,'SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'CASH SALE')  ";
                    }
                }

                double dQty = 0, dRate = 0;
                string strSalesMan = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    if (strSalesMan != "" && strSalesMan != "DIRECT")
                    {
                        string[] _strFullName = strSalesMan.Split(' ');
                        if (_strFullName.Length > 0)
                        {
                            strSalesMan = _strFullName[0].Trim();
                        }
                    }

                    strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[Other1],[SalesMan],[ItemName],[Qty],[MRP],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BrandName],[DesignName]) VALUES "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + row.Cells["OrderNo"].Value + "','" + strSalesMan + "','" + row.Cells["itemName"].Value + "'," + dQty + "," + dRate + "," + dRate + ","
                                  + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + ", " + ConvertObjectToDouble(row.Cells["amount"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "')";
                    strQuery += " UPDATE Res_SalesStock set Status = 'BILLED' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                if (dTaxAmt > 0)
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
                    strQuery += " UPDATE Res set Status = 'PENDING' From Res_SalesStock Res LEFT JOIN SalesBookSecondary SBS ON OrderNo = SBS.Other1 AND TableNo = SBS.BrandName AND Item = ItemName Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo;
                    strQuery += " Delete from SalesBook Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " Delete from SalesBookSecondary Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo;
                }

                strQuery += " end ";

                if (strQuery != "")
                {
                    _count = dba.ExecuteMyQuery(strQuery);
                }
            }
            catch (Exception ex) { }
            return _count;
        }

        private void SaveRecord()
        {
            try
            {
                int count = SaveRecordReturnInt();
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add(F2)";
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                    MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveAndPrintRecord()
        {
            try
            {
                int count = SaveRecordReturnInt();
                if (count > 0)
                {

                    btnAdd.Text = "&Add(F2)";
                    BindRecordWithControl(txtBillNo.Text);

                    if (dgrdDetails.Rows.Count > 0)
                    {
                        btnPrint.Enabled = false;

                        if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                        {
                            if (MainPage.strPrintLayout == "F")
                            {
                                GSTPrint(true, "", false, true);
                            }
                            else
                            {
                                GSTPrintOther(true, "", false, true);
                            }
                        }

                        btnPrint.Enabled = true;
                    }
                }
                else
                    MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    EnableAllControl();
                    txtBillNo.ReadOnly = true;
                    btnAdd.TabStop = btnSavePrint.TabStop = false;
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
                    dgrdDetails.Focus();
                }
                else if (ValidateControls() && ValidateOtherValidation(false))
                {
                    double dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text), dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                    string strQuery = "SELECT CUSTOMERNAME FROM(Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as CUSTOMERNAME,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','CASH A/C'))Sales WHERE CUSTOMERNAME = '" + txtCustomerName.Text + "'";
                    DataTable dt = dba.GetDataTable(strQuery);
                    bool isRegistered = (dt.Rows.Count > 0);

                    if (dNetAmt > 0 && dTenderAmt <= 0 && !isRegistered)
                    {
                        MessageBox.Show("Sorry ! Please enter registered customer or tender amount.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (bConfSmsSave == true)
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
                        else
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private int UpdateRecord(string strSubQuery)
        {
            int result = 0;
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL", strBillCode = "", strBillNo = "", strAdvSlipStatus = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                bool _registeredParty = false;
                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dFOtherAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dFinalAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0;
                string strSaleParty = "", strSalePartyID = "", strPetiAgent = "DIRECT";
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                if (strCustomer != "")
                {
                    string[] _strFullName = txtCustomerName.Text.Split(' ');
                    if (_strFullName.Length > 1)
                    {
                        strSalePartyID = _strFullName[0].Trim();
                        strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                        _registeredParty = true;
                    }
                }
                else
                    strSalePartyID = strSaleParty = txtCustomerName.Text;

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);

                if (dCreditAmt != 0)
                {
                    double dTenderAmt = 0, dRefundAmt = 0;
                    dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                    dRefundAmt = dTenderAmt - dCreditAmt;
                    txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                }

                dFOtherAmt = _dOtherAmt;
                if (txtSign.Text == "-")
                    dFOtherAmt = (dFOtherAmt) * -1;

                dFinalAmt = dGrossAmt + dFOtherAmt - dDisc;

                string strQuery = "  if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin "
                                + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='',[TransportName]='',[WaybillNo]='',[WayBillDate]='',[NoOfCase]='0',[LRNumber]='',[LRDate]=" + strLRDate + ",[LRTime]='',[PvtMarka]='',[Remark]='" + txtRemark.Text + "',[Description]='-',[PackerName]='',[PackingDate]=" + strPDate + ",[CartonType]='',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DisAmt]=" + dDisc + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ","
                                + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=0,[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=0,[GreenTax]=0,[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(txtTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strPetiAgent + "',[Description_2]='' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[MobileNo]='" + txtMobileNo.Text + "',[AdvanceAmt]=" + dAdvanceAmt + ",[ReturnAmt]=" + dReturnAmt + ",[CardAmt]=" + dCardAmt + ",[CashAmt]=" + dCashAmt + ", [CreditAmt]=" + dCreditAmt + ", [MaterialLocation]='" + txtLocation.Text + "', [TenderAmt]=" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + ", [RefundAmt]=" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + " , [ChequeAmt]=" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + ",[ChequeSerialno]='" + txtChqSrNo.Text + "',[OfferApplied]='" + chkOfferApply.Checked + "',[GrossProfit]='" + chkGrossProfit.Checked + "',[OfferDisPer]=" + txtOfrDisPer.Text + ",[OfferDisAmt]=" + txtOfrDisAmt.Text + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from [dbo].[CardDetails]Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";
                //+ " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                if (_registeredParty && strSalePartyID != "")
                {
                    strQuery += " if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') NOT IN ('CARD SALE','CASH SALE')) begin "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','DR','" + dAmt + "','0','FALSE','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  end else begin "
                             + " Update [dbo].[BalanceAmount] Set [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dAmt + ",[AccountID]='" + strSalePartyID + "'  Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') NOT IN ('CARD SALE','CASH SALE') end "
                             + " Delete from BalanceAmount Where [AccountStatus] in ('SALES A/C') AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') IN ('CARD SALE','CASH SALE') ";
                }
                else
                    strQuery += " Delete from BalanceAmount Where [AccountStatus] in ('SALES A/C','CARD RECEIVE','CASH RECEIVE') AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') NOT IN ('CARD SALE','CASH SALE') ";

                if (dCardAmt > 0)
                {
                    if (_registeredParty && strSalePartyID != "")
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CARD RECEIVE' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "') begin "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "',@CardName,'CARD RECEIVE','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'" + strSalePartyID + "')  "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "','" + strSaleParty + "','CARD RECEIVE','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CardName) end else begin "
                                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CardName,[Amount]=" + dCardAmt + ",[FinalAmount]='" + dCardAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CardName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='DEBIT' "
                                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCardAmt + ",[FinalAmount]='" + dCardAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='CREDIT' end ";
                    }
                    else
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE'  if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CARD SALE' ) begin "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + strDate + "',@CardName,'SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'CARD SALE')  end else begin "
                                    + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CardName,[Amount]=" + dCardAmt + ",[FinalAmount]='" + dCardAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CardName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C'  and [AccountStatusID]='CARD SALE'  end";
                    }
                }
                else
                    strQuery += " Delete from BalanceAmount Where (([AccountStatus] in ('SALES A/C') and [AccountStatusID]='CARD SALE') OR [AccountStatus] in ('CARD RECEIVE')) AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  ";

                if (dCashAmt > 0)
                {
                    if (_registeredParty && strSalePartyID != "")
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CASH RECEIVE' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "') begin "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "',@CashName,'CASH RECEIVE','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                   + " ('" + strDate + "','" + strSaleParty + "','CASH RECEIVE','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dFinalAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName) end else begin "
                                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='DEBIT' "
                                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='CREDIT' end ";
                    }
                    else
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CASH SALE' ) begin "
                                + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                + " ('" + strDate + "',@CashName,'SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'CASH SALE') end else begin "
                                + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='" + dCashAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C'  and [AccountStatusID]='CASH SALE'  end";
                    }
                }
                else
                    strQuery += " Delete from BalanceAmount Where (([AccountStatus] in ('SALES A/C') and [AccountStatusID]='CASH SALE') OR [AccountStatus] in ('CASH RECEIVE')) AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  ";

                string strID = "", strSalesMan = "";
                double dQty = 0, dRate = 0, _dAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                    if (strSalesMan != "" && strSalesMan != "DIRECT")
                    {
                        string[] _strFullName = strSalesMan.Split(' ');
                        if (_strFullName.Length > 0)
                        {
                            strSalesMan = _strFullName[0].Trim();
                        }
                    }
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);

                    _dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                    strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[Other1],[SalesMan],[ItemName],[Qty],[MRP],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BrandName],[DesignName]) VALUES "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + row.Cells["OrderNo"].Value + "','" + strSalesMan + "','" + row.Cells["itemName"].Value + "'," + dQty + "," + dRate + "," + dRate + ","
                                 + " " + _dAmt + "," + _dAmt + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "')";
                    }
                    else
                        strQuery += " Update [dbo].[SalesBookSecondary] SET [Other1]='" + row.Cells["OrderNo"].Value + "',[SalesMan]='" + strSalesMan + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dRate + ",[Rate]=" + dRate
                                + ",[Amount]=" + _dAmt + ",[BasicAmt]=" + _dAmt + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + "  ";

                    strQuery += " UPDATE Res_SalesStock set Status = 'BILLED' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";
                }
                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

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

                strQuery += " end";

                result = dba.ExecuteMyQuery(strQuery);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Text = "&Add(F2)";
                btnEdit.Text = "&Edit(F6)";
                BindLastRecord();
            }
            catch { }
        }

        private void SaleBook_Restorent_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void SetPermission()
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
                this.BeginInvoke(new MethodInvoker(Close));
            }
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

        private void txtMobileNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    string strMob = txtMobileNo.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("CUSTOMERMOBILE", "SEARCH CUSTOMER MOBILE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtMobileNo.Text = objSearch.strSelectedData;
                            string strCustomerName = "", strStation = "";
                            bool _bStatus = dba.CheckTransactionLockWithNameStationfromMobileNo(txtMobileNo.Text, ref strCustomerName, ref strStation);
                            if (_bStatus)
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtCustomerName.Text = txtMobileNo.Text = "";
                            }
                            else
                            {
                                if (strCustomerName != "" || strStation != "")
                                {
                                    txtCustomerName.Text = strCustomerName;
                                }
                            }

                            if (txtMobileNo.Text != strMob)
                            {
                                txtAdvanceAmt.Text = txtReturnAmt.Text = "0.00";
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

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string strInvoiceCode = txtBillCode.Text, strInvoiceNo = txtBillNo.Text;
        //        if (btnAdd.Text == "&Add(F2)" || btnEdit.Text == "&Update(F6)")
        //        {
        //            AlterationSlip _obj = new AlterationSlip(true, strInvoiceCode, strInvoiceNo);
        //            _obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        //            //_obj.
        //            _obj.ShowInTaskbar = true;
        //            _obj.Show();
        //        }

        //    }
        //    catch (Exception ex)
        //    { }
        //}

        private void btnCustomerAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SupplierMaster objSupplier = new SupplierMaster(1, "SUNDRY DEBTORS", "CUSTOMER");
                objSupplier.txtName.Text = txtCustomerName.Text;
                objSupplier.txtMobile.Text = txtMobileNo.Text;
                objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSupplier.ShowInTaskbar = true;
                objSupplier.ShowDialog();
                if (objSupplier.strAccountName != "")
                    txtCustomerName.Text = objSupplier.strAccountName;

            }
            catch { }
        }



        private void txtChequeAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    TextBox txtNew = sender as TextBox;
                    if (txtNew.Text == "")
                        txtNew.Text = "0.00";
                    else
                    {
                        double dChqAmt = ConvertObjectToDouble(txtChequeAmt.Text);
                        if (dChqAmt > 0)
                        {
                            string strName = "";
                            string str = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                            if (str != "")
                                strName = txtCustomerName.Text;

                            ChequeDetails objChequeDetail = new ChequeDetails(dChqAmt, strName);
                            objChequeDetail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objChequeDetail.ShowInTaskbar = true;
                            objChequeDetail.ShowDialog();
                            double dSavedAmt = objChequeDetail._chqAmount;
                            txtChequeAmt.Text = dSavedAmt.ToString("N2", MainPage.indianCurancy);
                            txtChqSrNo.Text = Convert.ToString(objChequeDetail.StrChqSrNo);
                        }
                        else
                        {
                            txtChqSrNo.Text = "";
                        }
                    }
                    CalculateAllAmount();
                }
            }
            catch { }

            dba.ChangeLeaveColor(sender, e);
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                if (ValidateControls() && CheckBillNoAndSuggest() && ValidateOtherValidation(false))
                {
                    double dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text), dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                    string strQuery = "SELECT CUSTOMERNAME FROM(Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as CUSTOMERNAME,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','CASH A/C')  UNION ALL Select Distinct SalePartyID as CUSTOMERNAME,0,SalePartyID as Name from SalesBook Where SalePartyID!='' and SalePartyID Not like ('%[0-9]%' ) )Sales WHERE CUSTOMERNAME = '" + txtCustomerName.Text + "'";
                    DataTable dt = dba.GetDataTable(strQuery);
                    bool isRegistered = (dt.Rows.Count > 0);

                    if (dNetAmt > 0 && dTenderAmt <= 0 && !isRegistered)
                    {
                        DialogResult result = MessageBox.Show("No Tender amount found! do you want save this bill as hold ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _status = HoldRecord();

                            if (_status && dgrdDetails.Rows.Count > 0)
                            {
                                btnPrint.Enabled = false;

                                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                                {
                                    if (MainPage.strPrintLayout == "F")
                                    {
                                        GSTPrint(true, "", false, true);
                                    }
                                    else
                                    {
                                        GSTPrintOther(true, "", false, true);
                                    }
                                }

                                btnPrint.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (bConfSmsSave == true)
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                SaveAndPrintRecord();
                            }
                        }
                        else
                        {
                            SaveAndPrintRecord();
                        }
                    }
                }
            }
        }

        private void txtChequeAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }

        private void txtChequeAmt_KeyPress(object sender, KeyPressEventArgs e)
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

        private int HoldRecordReturnInt()
        {
            int _Count = 0;
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL", strBillCode = "", strBillNo = "", strAdvSlipStatus = "", strQuery = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");
                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dFinalAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0, dFOtherAmt = 0;
                string strSaleParty = "", strSalePartyID = "", strSubPartyID = "", strPetiAgent = "DIRECT", strTickStatus = "False";
                string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                if (strCustomer != "")
                {
                    string[] _strFullName = txtCustomerName.Text.Split(' ');
                    if (_strFullName.Length > 1)
                    {
                        strSalePartyID = _strFullName[0].Trim();
                        strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                    }
                }
                else
                    strSalePartyID = strSaleParty = txtCustomerName.Text;

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);
                dFOtherAmt = _dOtherAmt;
                if (txtSign.Text == "-")
                    dFOtherAmt = (dFOtherAmt) * -1;

                dFinalAmt = dGrossAmt + _dOtherAmt - dDisc;

                if (dFinalAmt == dCashAmt)
                    strTickStatus = "True";

                if (strHoldBilLCode != "" && strHoldBillNo != "")
                {
                    strQuery = "  if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " ) begin "
                                 + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SalesType]='" + txtSalesType.Text + "',[TransportName]='',[WaybillNo]='',[WayBillDate]='',[NoOfCase]='0',[LRNumber]='',[LRDate]=" + strLRDate + ",[LRTime]='',[PvtMarka]='',[Remark]='" + txtRemark.Text + "',[Description]='-',[PackerName]='',[PackingDate]=" + strPDate + ",[CartonType]='',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DisAmt]=" + dDisc + ","
                                 + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=0,[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=0,[GreenTax]=0,[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(txtTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strPetiAgent + "',[Description_2]='' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[MobileNo]='" + txtMobileNo.Text + "',[AdvanceAmt]=" + dAdvanceAmt + ",[ReturnAmt]=" + dReturnAmt + ",[CardAmt]=" + dCardAmt + ",[CashAmt]=" + dCashAmt + ", [CreditAmt]=" + dCreditAmt + ", [MaterialLocation]='" + txtLocation.Text + "', [TenderAmt]=" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + ", [RefundAmt]=" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + " , [ChequeAmt]=" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + ",[ChequeSerialno]='" + txtChqSrNo.Text + "' Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " "
                                 + " Delete from [dbo].[CardDetails]Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " ";
                    //   + " Delete from StockMaster Where BillType='SALES' and BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " ";



                    string strID = "", strSalesMan = "";
                    double dQty = 0, dRate = 0, _dAmt = 0;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                        if (strSalesMan != "" && strSalesMan != "DIRECT")
                        {
                            string[] _strFullName = strSalesMan.Split(' ');
                            if (_strFullName.Length > 0)
                            {
                                strSalesMan = _strFullName[0].Trim();
                            }
                        }
                        dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                        dRate = ConvertObjectToDouble(row.Cells["rate"].Value);

                        _dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strID == "")
                        {
                            strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SalesMan],[ItemName],[Qty],[MRP],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                     + " ('" + strHoldBilLCode + "'," + strHoldBillNo + ",0,'" + row.Cells["OrderNo"].Value + "','" + strSalesMan + "','" + row.Cells["itemName"].Value + "'," + dQty + "," + dRate + ","
                                     + " " + _dAmt + "," + _dAmt + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["smInc"].Value + "','')";
                        }
                        else
                            strQuery += " Update [dbo].[SalesBookSecondary] SET [Other1]='" + row.Cells["OrderNo"].Value + "', [SalesMan]='" + strSalesMan + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dRate + ",[Rate]=" + dRate + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[BasicAmt]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[Other1]='" + row.Cells["smInc"].Value + "' Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " and SID=" + strID + "  ";

                        strQuery += " UPDATE Res_SalesStock set Status = 'BILLED' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";
                    }

                    foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails] ([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                       + " ('" + strHoldBilLCode + "'," + strHoldBillNo + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strHoldBilLCode + "'," + strHoldBillNo + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                    strQuery += " end";

                }
                else
                {

                    strBillCode = txtBillCode.Text + "H";

                    strQuery = "Declare @SerialNo bigint; Select @SerialNo=(ISNULL(MAX(_BillNo),0)+1) from (Select MAX(BIllNo)_BillNo from SalesBook Where BillCode='" + strBillCode + "' UNION ALL Select MAX(BIllNo)_BillNo from SalesRecord Where BillCode='" + strBillCode + "')_SALES  "
                                   + " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo) begin "
                                   + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[MobileNo],[AdvanceSlipNo],[AdvanceAmt],[ReturnSlipNo],[ReturnAmt],[CardAmt],[CashAmt],[CreditAmt],[SaleBillType],[MaterialLocation],[TenderAmt],[RefundAmt],[ChequeAmt],[ChequeSerialNo]) VALUES  "
                                   + " ('" + strBillCode + "',@SerialNo,'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','','','','','0',''," + strLRDate + ",'','','" + txtRemark.Text + "','-',''," + strPDate + ",'','', " + dba.ConvertObjectToDouble(txtDiscPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",0,'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",0,0,'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ","
                                   + " " + dba.ConvertObjectToDouble(txtTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + "," + dAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strPetiAgent + "','','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",'" + txtMobileNo.Text + "',''," + dAdvanceAmt + ",''," + dReturnAmt + "," + dCardAmt + "," + dCashAmt + ", " + dCreditAmt + ",'RETAIL_HOLD','" + txtLocation.Text + "','" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + "','" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + "','" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + "','" + txtChqSrNo.Text + "') ";


                    double dQty = 0, dRate = 0;
                    string strSalesMan = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                        dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                        strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                        if (strSalesMan != "" && strSalesMan != "DIRECT")
                        {
                            string[] _strFullName = strSalesMan.Split(' ');
                            if (_strFullName.Length > 0)
                            {
                                strSalesMan = _strFullName[0].Trim();
                            }
                        }

                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[Other1],[SalesMan],[ItemName],[Qty],[MRP],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BrandName],[DesignName]) VALUES "
                                      + " ('" + strBillCode + "',@SerialNo,0,'" + row.Cells["OrderNo"].Value + "','" + strSalesMan + "','" + row.Cells["itemName"].Value + "'," + dQty + "," + dRate + "," + dRate + ","
                                      + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + ", " + ConvertObjectToDouble(row.Cells["amount"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "')";

                        strQuery += " UPDATE Res_SalesStock set Status = 'BILLED' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";
                    }

                    foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails] ([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                       + " ('" + strBillCode + "',@SerialNo,'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strBillCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                    strQuery += " end ";
                }

                if (strQuery != "")
                {
                    _Count = dba.ExecuteMyQuery(strQuery);
                }
            }
            catch (Exception ex)
            {
            }
            return _Count;
        }

        private void btnHClose_Click(object sender, EventArgs e)
        {
            pnlHold.Visible = false;
        }

        private void txtDiscAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dAdvAmt = ConvertObjectToDouble(txtAdvanceAmt.Text), dReturnAmt = 0, dQty = 0, dBasicAmt = 0, dDisPer = 0, dDiscAmt = 0;
                TextBox txtNew = sender as TextBox;

                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);

                dDiscAmt = ConvertObjectToDouble(txtDiscAmt.Text);
                dDisPer = (dDiscAmt * 100) / (dBasicAmt - dReturnAmt);
                txtDiscPer.Text = dDisPer.ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }

            dba.ChangeLeaveColor(sender, e);
        }

        private void txtDiscAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }

        private void txtDiscAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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
                        if (objSearch != null)
                            objSearch.Close();
                    }
                }
                else
                {
                    if (objSearch != null)
                    {
                        objSearch.txtSearch.Text = e.KeyChar.ToString().Trim();
                        objSearch.txtSearch.SelectionStart = 1;
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

        private bool HoldRecord()
        {
            int count = 0;
            try
            {
                count = HoldRecordReturnInt();
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return count > 0;
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

        private void dgrdHold_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double dFinalAmt = ConvertObjectToDouble(txtFinalAmt.Text);

                if (e.RowIndex >= 0 && e.ColumnIndex == 1)
                {
                    if (dFinalAmt > 0 && btnAdd.Text == "&Save(F5)")
                        HoldRecordReturnInt();


                    ClearText();
                    string strBillNo = Convert.ToString(dgrdHold.CurrentCell.Value);
                    strHoldBilLCode = ""; strHoldBillNo = ""; strSaleBillType = "";
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

                            EnableAllControl();
                            btnAdd.TabStop = true;
                            txtBillNo.ReadOnly = true;
                            btnHold.Enabled = btnSavePrint.Enabled = true;
                            SetSerialNo();
                            txtCustomerName.Focus();
                            strSaleBillType = "HOLD";

                            if (dgrdCardDetail.Rows.Count == 0)
                            {
                                dgrdCardDetail.Rows.Add(1);
                                dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                            }
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
                                        + " UPDATE Res set Status = 'PENDING' From Res_SalesStock Res LEFT JOIN SalesBookSecondary SBS ON OrderNo = SBS.Other1 AND TableNo = SBS.BrandName AND Item = ItemName where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        + " delete from SalesBookSecondary where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        //  + " delete from StockMaster where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        + " delete from CardDetails where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'";
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

        private void txtSalesType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
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

        private void btnSaleSetup_Click(object sender, EventArgs e)
        {
            try
            {
                SaleBookRetail_FormControl _obj = new SaleBookRetail_FormControl();
                _obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                _obj.ShowInTaskbar = true;
                _obj.ShowDialog();
                GetCellSetup();
            }
            catch (Exception ex)
            { }
        }

        private void txtCardAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtCardAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    TextBox txtNew = sender as TextBox;
                    if (txtNew.Text == "")
                        txtNew.Text = "0.00";
                    double dcardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                    if (dcardAmt > 0)
                    {
                        dgrdCardDetail.ReadOnly = false;
                        dgrdCardDetail.Enabled = true;
                        dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells["bank"];
                        dgrdCardDetail.Rows[0].Cells["cAmt"].Value = Convert.ToString(txtCardAmt.Text);
                        CalculateAllAmount();
                        dgrdCardDetail.Focus();
                        return;
                    }
                    else
                    {
                        dgrdCardDetail.ReadOnly = true;
                        dgrdCardDetail.Rows.Clear();
                        dgrdCardDetail.Rows.Add();
                        dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                    }

                }
            }
            catch (Exception ex)
            { }

            dba.ChangeLeaveColor(sender, e);
        }

        private void chkCashAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                //txtCashAmt.Enabled = chkCashAmt.Checked ? true : false;
                //if (!chkCashAmt.Checked)
                //{
                //    txtCashAmt.Text = "0.00";
                //    txtCashAmt.ReadOnly = true;
                //    CalculateAllAmount();
                //}

            }
        }

        //private void chkCreditSale_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
        //    {
        //        txtCreditSale.Enabled = chkCreditSale.Checked ? true : false;

        //        if (!chkCreditSale.Checked)
        //        {
        //            txtCreditSale.Text = "0.00";
        //            txtCreditSale.ReadOnly = true;
        //        }else
        //            txtCreditSale.ReadOnly = false;

        //    }
        //}

        private void txtCashAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }

        private void txtCashAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dAdvAmt = ConvertObjectToDouble(txtAdvanceAmt.Text), dReturnAmt = 0, dQty = 0, dBasicAmt = 0, dDisPer = 0, dDiscAmt = 0;
                TextBox txtNew = sender as TextBox;

                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);

                dDisPer = ConvertObjectToDouble(txtDiscPer.Text);
                dDiscAmt = ((dBasicAmt - dReturnAmt) * dDisPer) / 100;
                txtDiscAmt.Text = dDiscAmt.ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALES", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                CalculateAllAmount();

            dba.ChangeLeaveColor(sender, e);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)" && MainPage.strPrintLayout == "F")
                {
                    GSTPrintAndPreview(false, "", false, true);
                }
                else if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    GSTPreview(false, "", false, true);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }


        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            string strValue = "0";
            if (_pstatus)
            {
                strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
                if (strValue == "" || strValue == "0")
                {
                    return false;
                }
            }
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = (short)(int)Convert.ToDouble(strValue);
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSalesBookRetailDataTable_Retail(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (!MainPage._bTaxStatus)
                {

                    Reporting.SaleBookRetailReport objOL_salebill = new Reporting.SaleBookRetailReport();
                    objOL_salebill.SetDataSource(dt);
                    objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                    // objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                    if (strPath != "")
                    {
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
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
                                objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
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
                    if (!_bIGST)
                    {
                        //DSC

                        //if (MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO"))
                        //{
                        //    Reporting.SaleBookRetailReport_CSGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_DSC();
                        //    objOL_salebill.SetDataSource(dt);
                        //    objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        //    objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        //    objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //        return true;
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            //objReport.myPreview.ShowExportButton = false;
                        //            //objReport.myPreview.ShowPrintButton = false;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //}
                        //else
                        {
                            Reporting.RetailSaleBookReportFull objOL_salebill = new Reporting.RetailSaleBookReportFull();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            if (strPath != "")
                            {
                                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
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
                                        objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
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
                        // DSC 

                        // if (MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO"))
                        //{
                        //    Reporting.SaleBookRetailReport_IGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_IGST_DSC();
                        //    objOL_salebill.SetDataSource(dt);
                        //    objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        //    objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        //    objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //        return true;
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            //objReport.myPreview.ShowExportButton = false;
                        //            // objReport.myPreview.ShowPrintButton = false;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //}
                        //else
                        {
                            Reporting.RetailSaleBookReportFull_IGST objOL_salebill = new Reporting.RetailSaleBookReportFull_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);

                            if (strPath != "")
                            {
                                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
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
                                        objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
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

        private bool GSTPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            string strValue = "0";
            if (_pstatus)
            {
                strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
                if (strValue == "" || strValue == "0")
                {
                    return false;
                }
            }

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.SalesBookRestoDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (MainPage.strPrintLayout != "")
                {
                    if (MainPage.strPrintLayout == "T3")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportT3 objOL_salebill = new Reporting.RetailSaleBookReportT3();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT3_IGST objOL_salebill = new Reporting.RetailSaleBookReportT3_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

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
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportGatePassT2_IGST objOL_salebill = new Reporting.RetailSaleBookReportGatePassT2_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "H")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportHalf objOL_salebill = new Reporting.RetailSaleBookReportHalf();

                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportHalf_IGST objOL_salebill = new Reporting.RetailSaleBookReportHalf_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "Q")
                    {

                        Reporting.RetailSaleBookReportQuarter objOL_salebill = new Reporting.RetailSaleBookReportQuarter();
                        //DataSet ds = new DataSet();
                        //ds.Tables.Add(dt);
                        //ds.Tables.Add(_dtGST);
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                        objShow.myPreview.ReportSource = objOL_salebill;
                        objShow.ShowDialog();

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
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT1_IGST objOL_salebill = new Reporting.RetailSaleBookReportT1_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                            objShow.myPreview.ReportSource = objOL_salebill;
                            objShow.ShowDialog();

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Warning ! Please select print design.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
            //    {
            //        btnCreatePDF.Enabled = false;
            //        DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (result == DialogResult.Yes)
            //        {
            //            string strPath = SetSignatureInBill(false, true, true);
            //            if (strPath != "")
            //                MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Sorry ! Please Save the record...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //catch
            //{
            //}
            //btnCreatePDF.Enabled = true;
        }

        private string SetSignatureInBill(bool _bPStatus, bool _createPDF, bool _dscVerified)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
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

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strFileName, true, _dscVerified);
                    if (_bstatus)
                    {
                        //if (!_dscVerified && MainPage.strCompanyName.Contains("SARAOGI"))
                        //{
                        //    string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";

                        //    PDFSigner _objSigner = new PDFSigner();
                        //    bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                        //    if (!_bFileStatus)
                        //        strPath = "";
                        //    if (_bPStatus && _bFileStatus)
                        //        System.Diagnostics.Process.Start(strPath);
                        //}
                        //else
                        {
                            File.Copy(strFileName, strPath);
                            if (_bPStatus)
                                System.Diagnostics.Process.Start(strPath);
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

        private void txtBillNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearText();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;

                    if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                    {
                        if (MainPage.strPrintLayout == "F")
                        {
                            GSTPrint(true, "", false, true);
                        }
                        else
                        {
                            GSTPrintOther(true, "", false, true);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnPrint.Enabled = true;
                }
            }
            catch
            {
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = true;
            if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
            {
                GSTPreview(false, "", false, true);
            }
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

                                string strQuery = "  Delete from SalesBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('SALES A/C','DUTIES & TAXES','CARD RECEIVE','CASH RECEIVE')  "
                                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                                //  + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " Delete from CardDetails Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtFinalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                                foreach (DataGridViewRow row in dgrdDetails.Rows)
                                {
                                    strQuery += "UPDATE Res_SalesStock set Status = 'PENDING' where OrderNo = '" + row.Cells["OrderNo"].Value + "' AND TableNo = '" + row.Cells["brandName"].Value + "' AND Item = '" + row.Cells["itemName"].Value + "' ";
                                }

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

        private void SaleBook_Restorent_FormClosing(object sender, FormClosingEventArgs e)
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

        }

        private void txtTenderAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "")
                    txtNew.Text = "0.00";

                double dTenderAmt = 0, dRefundAmt = 0, dNetAmt = 0;
                dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                dRefundAmt = dTenderAmt - dNetAmt;
                txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);

                //if (dTenderAmt > dNetAmt)
                //    txtCashAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                //else
                //    txtCashAmt.Text = dTenderAmt.ToString("N2", MainPage.indianCurancy);
            }

            dba.ChangeLeaveColor(sender, e);
        }

        private void txtTenderAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtTenderAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            dba.ChangeFocusColor(sender, e);
        }


        private void txtTransection_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("MATERIALCENTER", txtLocation.Text, "SELECT LOCATION", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtLocation.Text = objSearch.strSelectedData;
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnFormControl_Click(object sender, EventArgs e)
        {
            try
            {
                SaleBookRetail_FormControl _obj = new SaleBookRetail_FormControl();
                _obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                _obj.ShowInTaskbar = true;
                _obj.ShowDialog();
                GetCellSetup();
            }
            catch (Exception ex)
            { }
        }

        private bool GSTPrint(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            string strValue = "0";
            if (_pstatus)
            {
                strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
                if (strValue == "" || strValue == "0")
                {
                    return false;
                }
            }
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = (short)(int)Convert.ToDouble(strValue);
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSalesBookRetailDataTable_Retail(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (!MainPage._bTaxStatus)
                {

                    Reporting.SaleBookRetailReport objOL_salebill = new Reporting.SaleBookRetailReport();
                    objOL_salebill.SetDataSource(dt);
                    objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                    // objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);

                    if (strPath != "")
                    {
                        objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
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
                                objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
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
                    if (!_bIGST)
                    {
                        //DSC

                        {
                            Reporting.RetailSaleBookReportFull objOL_salebill = new Reporting.RetailSaleBookReportFull();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            // objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else
                    {
                        // DSC 

                        {
                            Reporting.RetailSaleBookReportFull_IGST objOL_salebill = new Reporting.RetailSaleBookReportFull_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                }

            }

            return false;
        }

        private bool GSTPrintOther(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            string strValue = "0";
            if (_pstatus)
            {
                strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
                if (strValue == "" || strValue == "0")
                {
                    return false;
                }
            }

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.SalesBookRestoDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                defS.Copies = (short)(int)Convert.ToDouble(strValue);
                defS.Collate = false;
                defS.FromPage = 0;
                defS.ToPage = 0;
                if (MainPage.strPrintLayout != "")
                {
                    if (MainPage.strPrintLayout == "T3")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportT3 objOL_salebill = new Reporting.RetailSaleBookReportT3();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT3_IGST objOL_salebill = new Reporting.RetailSaleBookReportT3_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            var dr = objOL_salebill.PrintOptions.PaperSize;
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

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
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportGatePassT2_IGST objOL_salebill = new Reporting.RetailSaleBookReportGatePassT2_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "H")
                    {
                        if (!_bIGST)
                        {
                            Reporting.RetailSaleBookReportHalf objOL_salebill = new Reporting.RetailSaleBookReportHalf();

                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportHalf_IGST objOL_salebill = new Reporting.RetailSaleBookReportHalf_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                    }
                    else if (MainPage.strPrintLayout == "Q")
                    {

                        Reporting.RetailSaleBookReportQuarter objOL_salebill = new Reporting.RetailSaleBookReportQuarter();
                        //DataSet ds = new DataSet();
                        //ds.Tables.Add(dt);
                        //ds.Tables.Add(_dtGST);
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

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
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT1_IGST objOL_salebill = new Reporting.RetailSaleBookReportT1_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

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

            return false;
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='" + txtSalesType.Text + "' and TaxName='SALES') Region,ISNULL((Select TOP 1 InsertStatus from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where  (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtCustomerName.Text + "' OR NAME LIKE('%CASH%') ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtCustomerName.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (btnEdit.Text == "&Update(F6)" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtCustomerName.Text || dOldNetAmt != Convert.ToDouble(txtFinalAmt.Text) || _bUpdateStatus)
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
                }
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public Control FindFocusedControl(Control control)
        {
            var container = control as ContainerControl;
            return (null != container
                ? FindFocusedControl(container.ActiveControl)
                : control);
        }
    }
}
