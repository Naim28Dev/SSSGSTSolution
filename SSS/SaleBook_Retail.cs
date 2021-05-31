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
    public partial class SaleBook_Retail : Form
    {
        DataBaseAccess dba;
        double dSalesPartyDiscount = 0, dOldNetAmt = 0, dAdvanceSlipAmt = 0;
        string strOldAdvSlipNo = "", strLastSerialNo = "", strOldPartyName = "", strSaleBillType = "", strHoldBilLCode = "", strHoldBillNo = "";
        Boolean bsalesman, bBarcode, bBrandname, bitemname, bqty, bamount, bcustomername, bmobile, bcity, bemail, bsms, blocation, bConfSmsSave;
        int isno = 0, isalesman = 0, ibarcode = 0, ibrandname = 0, istylename = 0, iitemname = 0, iqty = 0, iUOM = 0, iMRP = 0, iDis = 0, irate = 0, iamount = 0, iSMI = 0, istockqty = 0;

        DataTable OfferDT = new DataTable();
        int OfferOnIndex = -1;
        double OfferAmt = 0, FixOfferAmt = 0, MainIsAddOn = 0;
        DataTable dtFreeItems = new DataTable();

        SearchCategory_Custom objSearch;
        SearchData _objData;
        public SaleBook_Retail()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            dtFreeItems.Columns.Add("Index", typeof(int));
            dtFreeItems.Columns.Add("Items", typeof(string));
            dtFreeItems.Columns.Add("FreeQty", typeof(int));
            dtFreeItems.Columns.Add("FreePer", typeof(double));
            dtFreeItems.Columns.Add("MinPayAmt", typeof(double));
            SetCategory();
            GetStartupData(true);
            GetCellSetup();
        }


        public SaleBook_Retail(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            dtFreeItems.Columns.Add("Index", typeof(int));
            dtFreeItems.Columns.Add("Items", typeof(string));
            dtFreeItems.Columns.Add("FreeQty", typeof(int));
            dtFreeItems.Columns.Add("FreePer", typeof(double));
            dtFreeItems.Columns.Add("MinPayAmt", typeof(double));
            SetCategory();
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

                        iSMI = (from DataRow dr in dt.Rows
                                where (string)dr["ColumnName"] == "SMI(%)"
                                select (int)dr["IndexNo"]).FirstOrDefault();

                        istockqty = (from DataRow dr in dt.Rows
                                     where (string)dr["ColumnName"] == "StockQty"
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
                            txtCity.TabStop = false;
                        }
                        else
                        {
                            txtCity.TabIndex = T3;
                        }

                        int T4 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "Remark"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T4 == 0)
                        {
                            txtRemark.TabStop = false;
                        }
                        else
                        {
                            txtRemark.TabIndex = T4;
                        }

                        int T5 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "AdvanceSlip"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T5 == 0)
                        {
                            txtAdvanceSlip.TabStop = false;
                        }
                        else
                        {
                            txtAdvanceSlip.TabIndex = T5;
                        }

                        int T6 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "ReturnSlip"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T6 == 0)
                        {
                            txtReturnSlip.TabStop = false;
                        }
                        else
                        {
                            txtReturnSlip.TabIndex = T6;
                        }

                        int T7 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "SType"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T7 == 0)
                        {
                            txtSalesType.TabStop = false;
                        }
                        else
                        {
                            txtSalesType.TabIndex = T7;
                        }

                        int T8 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "CardDetail"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T8 == 0)
                        {
                            dgrdCardDetail.TabStop = false;
                            grpBoxCard.TabStop = false;
                        }
                        else
                        {
                            dgrdCardDetail.TabStop = true;
                            grpBoxCard.TabStop = true;
                            dgrdCardDetail.TabIndex = T8;
                            grpBoxCard.TabIndex = T8;
                        }

                        int T9 = (from DataRow dr in dt.Rows
                                  where (string)dr["ColumnName"] == "Email"
                                  select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T9 == 0)
                        {
                            chkEmail.TabStop = false;
                        }
                        else
                        {
                            chkEmail.TabIndex = T9;
                        }

                        int T10 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "SMS"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T10 == 0)
                        {
                            chkSendSMS.TabStop = false;
                        }
                        else
                        {
                            chkSendSMS.TabIndex = T10;
                        }

                        int T11 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "Location"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T11 == 0)
                        {
                            txtLocation.TabStop = false;
                        }
                        else
                        {
                            txtLocation.TabIndex = T11;
                        }

                        int T12 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "CardAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T12 == 0)
                        {
                            txtCardAmt.TabStop = false;
                        }
                        else
                        {
                            txtCardAmt.TabIndex = T12;
                        }

                        int T13 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "CashAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T13 == 0)
                        {
                            txtCashAmt.TabStop = false;
                        }
                        else
                        {
                            txtCashAmt.TabIndex = T13;
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

                        int T14 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "OtherAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T14 == 0)
                        {
                            txtSign.TabStop = false;
                            txtOtherAmount.TabStop = false;
                        }
                        else
                        {
                            txtSign.TabIndex = T14;
                            txtOtherAmount.TabIndex = T14;
                        }

                        int T15 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "TenderAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T15 == 0)
                        {
                            txtTenderAmt.TabStop = false;
                        }
                        else
                        {
                            txtTenderAmt.TabIndex = T15;
                        }

                        int T16 = (from DataRow dr in dt.Rows
                                   where (string)dr["ColumnName"] == "RefundAmt"
                                   select (int)dr["IndexNo"]).FirstOrDefault();
                        if (T16 == 0)
                        {
                            txtRefundAmt.TabStop = false;
                        }
                        else
                        {
                            txtRefundAmt.TabIndex = T16;
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


        private void SaleBook_Retail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlPendingAdvance.Visible)
                    pnlPendingAdvance.Visible = false;
                else if (pnlReturn.Visible)
                    pnlReturn.Visible = false;
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
            //else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.T)
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
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.C)
            {
                btnTodaysSale.PerformClick();
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
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                btnCreatePDF.PerformClick();
            }
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
                string strQuery = " Select *,ISNULL((SalePartyID+' '+SName),SalePartyID) SParty,CONVERT(varchar,Date,103)BDate,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SB.Date))) LockType from SalesBook SB OUTER APPLY (Select Top 1 SM.Name as SName,NormalDhara from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.SalePartyID)SM1    Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "    Select SBS.*,(StockQty+Qty)StockQty,ISNULL(dbo.GetFullName(SBS.SalesMan),'DIRECT')SalesManName from SalesBookSecondary SBS OUTER APPLY (Select SUM(Qty)StockQty from (Select SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select -SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASERETURN','SALES','STOCKOUT') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select 1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and _IM.DisStatus=0  and ISNULL(_IS.Description,'')=ISNULL(SBS.BarCode,'') and _IM.ItemName=SBS.ItemName and _IS.Variant1=SBS.Variant1 and _IS.Variant2=SBS.Variant2)Stock)Stock Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " order by SID  "
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
                            txtSalesType.Text = Convert.ToString(row["SalesType"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtCity.Text = Convert.ToString(row["Station"]);
                            txtLocation.Text = Convert.ToString(row["MaterialLocation"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                            txtDiscPer.Text = dba.ConvertObjToFormtdString(row["DisPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DisAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);
                            txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                            txtImportData.Text = Convert.ToString(row["Description_3"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                            txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                            txtAdvanceSlip.Text = strOldAdvSlipNo = Convert.ToString(row["AdvanceSlipNo"]);
                            txtReturnSlip.Text = Convert.ToString(row["ReturnSlipNo"]);
                            txtChqSrNo.Text = Convert.ToString(row["ChequeSerialNo"]);
                            txtAdvanceAmt.Text = ConvertObjectToDouble(row["AdvanceAmt"]).ToString("N2", MainPage.indianCurancy);
                            dAdvanceSlipAmt = ConvertObjectToDouble(row["AdvanceAmt"]);
                            txtReturnAmt.Text = ConvertObjectToDouble(row["ReturnAmt"]).ToString("N2", MainPage.indianCurancy);

                            double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dChqAmt;
                            dCardAmt = ConvertObjectToDouble(row["CardAmt"]);
                            dCashAmt = ConvertObjectToDouble(row["CashAmt"]);
                            dCreditAmt = ConvertObjectToDouble(row["CreditAmt"]);
                            dChqAmt = ConvertObjectToDouble(row["ChequeAmt"]);

                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            //chkCardAmt.Checked = (dCardAmt > 0) ? true : false;

                            txtOfrDisAmt.Text = ConvertObjectToDouble(row["OfferDisAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtOfrDisPer.Text = ConvertObjectToDouble(row["OfferDisPer"]).ToString("N2", MainPage.indianCurancy);

                            txtCardAmt.Text = dCardAmt.ToString("N2", MainPage.indianCurancy);
                            txtCashAmt.Text = dCashAmt.ToString("N2", MainPage.indianCurancy);
                            txtChequeAmt.Text = dChqAmt.ToString("N2", MainPage.indianCurancy);
                            txtNetAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);

                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);
                            txtTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            txtGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            txtFinalAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                            double dTenderAmt = ConvertObjectToDouble(row["TenderAmt"]);
                            txtTenderAmt.Text = dTenderAmt.ToString("N2", MainPage.indianCurancy);
                            //if (dTenderAmt > 0)
                            txtRefundAmt.Text = ConvertObjectToDouble(row["RefundAmt"]).ToString("N2", MainPage.indianCurancy);
                            //else
                            //    txtRefundAmt.Text = "0.00";
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
                    BindAdvanceSlipAmt(ds.Tables[4]);
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
                    dgrdHold.Rows[rowIndex].Cells["hPartyName"].Value = row["SalePartyID"];
                    rowIndex++;
                }
                btnHoldList.BackColor = Color.DarkGreen;
            }

        }

        private void BindAdvanceSlipAmt(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (txtAdvanceSlip.Text != "")
                    dAdvanceSlipAmt = ConvertObjectToDouble(row["Amount"]);
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
                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
                    dgrdDetails.Rows[rowIndex].Cells["salesMan"].Value = row["SalesManName"];
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

                    dgrdDetails.Rows[rowIndex].Cells["offerName"].Value = row["OfferName"];
                    dgrdDetails.Rows[rowIndex].Cells["offerDisPer"].Value = row["OfferDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["discAmt"].Value = row["OfferDisAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["OfferedBarcode"].Value = row["RewardItem"];
                    dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = row["RewardCoupon"];

                    dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["BrandName"];
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["smInc"].Value = row["SaleIncentive"];
                    dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = row["StockQty"];
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];

                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["OfferedBarcode"].Value).Contains("OFFERITEM"))
                    {
                        dgrdDetails.Rows[rowIndex].ReadOnly = true;
                        dgrdDetails.Rows[rowIndex].Cells["salesMan"].ReadOnly = false;
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Green;
                    }
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
                //  pnlTax.Visible = true;
            }
            //else
            //pnlTax.Visible = false;
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
                dba.GetDateInExactFormat(sender, true, false, true);
                //SetDueDays();
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
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 13 || e.ColumnIndex == 16 || e.ColumnIndex == 18 || e.ColumnIndex == 19 || e.ColumnIndex == 21 || e.ColumnIndex == 22 || e.ColumnIndex == 23)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        _objData = new SearchData("SALESMANNAME", "SEARCH SALES MAN NAME", Keys.Space);
                        _objData.ShowDialog();
                        dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        if (Convert.ToString(dgrdDetails.CurrentRow.Cells["itemName"].Value) == "")
                        {
                            _objData = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                            _objData.ShowDialog();
                            dgrdDetails.CurrentCell.Value = dgrdDetails.CurrentRow.Cells["oldBrandname"].Value = _objData.strSelectedData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11)
                    {
                        string strBrandName = Convert.ToString(dgrdDetails.CurrentRow.Cells["oldbrandName"].Value), strFrom = e.ColumnIndex == 3 ? "BarCode" : "ItemName";
                        objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_RETAIL", strBrandName, "", "", "", "", "", "", Keys.Space, false, false, strFrom);
                        objSearch.ShowDialog();
                        //SearchCategory_Custom objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_SALEMERGE", strBrandName, "", "", "", "", "", Keys.Space, false, false);
                        //objSearch.ShowDialog();

                        RemoveOrLessFreeRow(e.RowIndex);
                        UnSetOfferRow(e.RowIndex);
                        if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                            GetAllDesignSizeColorWithBarCode_Unique(objSearch, dgrdDetails.CurrentRow.Index);
                        else
                            GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);
                        if (chkOfferApply.Checked)
                        {
                            if (!CheckIfOfferedItem(e.RowIndex))
                            {
                                GetOfferDetail(e.RowIndex);
                            }
                        }
                        ArrangeSerialNo();
                        calcGrossAmount(); //CalculateAllAmount();
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 15 || e.ColumnIndex == 17 || e.ColumnIndex == 14)
                    {
                        if (!MainPage.strUserRole.Contains("ADMIN"))
                            e.Cancel = true;
                        if (dgrdDetails.CurrentRow.Cells["isAddOn"].Value != null && Convert.ToInt32(dgrdDetails.CurrentRow.Cells["isAddOn"].Value) > 0)
                            e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 20)
                    {
                        if (chkOfferApply.Checked)
                            ShowOnlyValidOffers(e.RowIndex);
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

        private bool CheckIfOfferedItem(int RowIndex)
        {
            try
            {
                int tblIndex = 0;
                int FreeQty = 0;
                double FreePer = 0, MinPayAmt = 0;

                bool isOfferedItem = false;
                string entredBarcode = Convert.ToString(dgrdDetails.Rows[RowIndex].Cells["barcode_s"].Value);
                if (entredBarcode != "" && dtFreeItems.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtFreeItems.Rows)
                    {
                        string[] ItemsList = dr["Items"].ToString().Split(',');
                        if (ItemsList.Length > 0)
                        {
                            FreeQty = Convert.ToInt32(dr["FreeQty"]);
                            FreePer = dba.ConvertObjectToDouble(dr["FreePer"]);
                            MinPayAmt = dba.ConvertObjectToDouble(dr["MinPayAmt"]);
                            foreach (string Item in ItemsList)
                            {
                                if (entredBarcode == Item)
                                {
                                    isOfferedItem = true;
                                    break;
                                }
                            }
                            if (isOfferedItem)
                                break;
                        }
                        tblIndex++;
                    }

                    if (isOfferedItem)
                    {
                        dtFreeItems.Rows.RemoveAt(tblIndex);
                        if (MinPayAmt > 0)
                        {
                            dgrdDetails.Rows[RowIndex].Cells["mrp"].Value = MinPayAmt;
                        }
                        else
                        {
                            dgrdDetails.Rows[RowIndex].Cells["offerDisPer"].Value = FreePer;
                            if (MainIsAddOn == 0)
                                dgrdDetails.Rows[RowIndex].Cells["disPer"].Value = "";
                        }

                        dgrdDetails.Rows[RowIndex].Cells["isAddOn"].Value = MainIsAddOn;
                        dgrdDetails.Rows[RowIndex].Cells["qty"].Value = FreeQty;
                        dgrdDetails.Rows[RowIndex].Cells["OfferedBarcode"].Value = "OFFERITEM<1";
                        if (OfferOnIndex >= 0)
                        {
                            dgrdDetails.Rows[OfferOnIndex].Cells["rewardItem"].Value = dgrdDetails.Rows[RowIndex].Cells["itemName"].Value;
                            dgrdDetails.Rows[OfferOnIndex].Cells["OfferedBarcode"].Value = entredBarcode + "<" + FreeQty.ToString();
                        }

                        dgrdDetails.CurrentCell = dgrdDetails[3, RowIndex];
                        dgrdDetails.Rows[RowIndex].ReadOnly = true;
                        dgrdDetails.Rows[RowIndex].DefaultCellStyle.ForeColor = Color.Green;
                        CalculateSpecialDiscount();

                        return true;
                    }
                }
            }
            catch (Exception ex) { }
            return false;
        }


        private void GetOfferDetail(int rowIndex)
        {
            try
            {
                string strBrand = "", strBarCode = "", strItem = "";

                strBarCode = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value);
                strBrand = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["brandName"].Value);
                strItem = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value);

                if (strBarCode != "")
                {
                    OfferDT.Rows.Clear();

                    string qry = "exec[GetSaleOffer] '" + strBarCode + "', '" + strItem + "','" + strBrand + "'";
                    DataTable dt = dba.GetDataTable(qry);

                    OfferDT = dt;
                    ShowOnlyValidOffers(rowIndex);
                }
            }
            catch (Exception ex) { }
        }
        private void GatCoupanOfferDetails(int RowIndex, string strCoupanCode)
        {
            string Qry = "SELECT top 1 OM.OFFER_CODE OfferCode,OM.OFFER_NO OfferNo, OM.OFFER_NAME OfferName,OM.OFFER_DESC OfferDesc,OM.REWARD_COUPON RewardCoupan , OM.ITEM_ARRIVAL_DATE_FROM ArvFDate, OM.ITEM_ARRIVAL_DATE_TO ArvToDate, OM.OFFER_VALID_FROM OffValidFrom, OM.OFFER_VALID_TILL OffValidTo  , OM.FREE_QTY FreeQty, OM.QUANTITY PurQty  , OM.DISC_TYPE DiscType, OM.Discount DiscAmt  , OM.AMOUNT_FROM MinPurAmt, OM.AMOUNT_TO MaxPurAmt  , OM.IsAddOn, OM.OfferAmount FixOfferAmt, OM.FREE_PER FreePer, OM.FREE_MIN_AMT MinPayAmt, (STUFF((SELECT ',' + FiletrValue FROM OfferDetails OFD WHERE FilterType = 'GET' AND OFD.OfferCode = OD.OfferCode AND OFD.OfferNo = OD.OfferNo AND OFD.FilterName = 'BARCODE' FOR XML PATH('')),1,1,'')) FreeBarCodes FROM OfferDetails OD LEFT JOIN OFFER_MASTER OM on OD.OfferCode = OM.OFFER_CODE and OD.OfferNo = OM.OFFER_NO WHERE ISNULL(CoupanUsedCount,0) < ISNULL(CoupanValidCount,0) AND OM.IsCoupan = 1 AND OD.FilterType = 'GET' AND OM.OFFER_NAME = '" + strCoupanCode + "'";
            DataTable DT = dba.GetDataTable(Qry);

            if (DT.Rows.Count > 0)
            {
                GetOfferValus(RowIndex, DT);
            }
        }

        private void ShowAllOffers(int rowIndex, bool ShowAll = false)
        {
            DataTable SelectedOffer = new DataTable();
            if (OfferDT.Rows.Count == 1 && !ShowAll)
            {
                RemoveOrLessFreeRow(rowIndex);
                UnSetOfferRow(rowIndex);
                UnApplyOffer(rowIndex);

                GetOfferValus(rowIndex, OfferDT);
            }

            if (OfferDT.Rows.Count > 1 || ShowAll)
            {
                DataView DV = new DataView(OfferDT);
                DataTable DT = DV.ToTable(true, "OfferName");

                _objData = new SearchData(DT, "OFFERAVAILABLE", "SELECT AVAILABLE OFFER", Keys.Space);
                _objData.ShowDialog();

                string strOffername = _objData.strSelectedData;
                string strPreOffername = Convert.ToString(dgrdDetails[20, rowIndex].Value);

                if ((strPreOffername != "" && strPreOffername != strOffername) || strOffername == "")
                {
                    RemoveOrLessFreeRow(rowIndex);
                    UnSetOfferRow(rowIndex);
                    UnApplyOffer(rowIndex);
                }

                if (strOffername != "")
                {
                    dgrdDetails[20, rowIndex].Value = strOffername;

                    DataRow[] SelectedRows = OfferDT.Select("OfferName = '" + strOffername + "'");
                    GetOfferValus(rowIndex, SelectedRows.CopyToDataTable());
                }
                CalculateSpecialDiscount();
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

        private void ShowOnlyValidOffers(int rowIndex)
        {
            MainIsAddOn = 0;

            var ItemAmt = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["amount"].Value);
            var ItemQty = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["qty"].Value);
            var ItemRate = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["rate"].Value);
            var ItemMRP = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["mrp"].Value);
            var TotalAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text);
            DateTime dtBillDate = dba.ConvertDateInExactFormat(txtDate.Text);

            string OffName = "", DiscType = "", RewardCoupan = "", FreeBarCodes = "", OfferDesc = "";
            double PurQty = 0, FreeQty = 0, IsAddOn = 0, MinPurAmt = 0, MaxPurAmt = 0, FixOfferAmt = 0, FreePer = 0, DiscAmt = 0, MinPayAmt = 0;

            DataTable ValidOffers = OfferDT.Clone();

            foreach (DataRow dr in OfferDT.Rows)
            {
                DateTime Fromdt = ConvertDateInExactFormat(Convert.ToString(dr["OffValidFrom"]));
                DateTime Todt = ConvertDateInExactFormat(Convert.ToString(dr["OffValidTo"]));

                OffName = Convert.ToString(dr["OfferName"]);
                OfferDesc = Convert.ToString(dr["OfferDesc"]);

                MinPurAmt = dba.ConvertObjectToDouble(dr["MinPurAmt"]);
                MaxPurAmt = dba.ConvertObjectToDouble(dr["MaxPurAmt"]);
                PurQty = dba.ConvertObjectToDouble(dr["PurQty"]);

                IsAddOn = dba.ConvertObjectToDouble(dr["IsAddOn"]);
                DiscType = Convert.ToString(dr["DiscType"]);
                DiscAmt = dba.ConvertObjectToDouble(dr["DiscAmt"]);
                FixOfferAmt = dba.ConvertObjectToDouble(dr["FixOfferAmt"]);

                RewardCoupan = Convert.ToString(dr["RewardCoupan"]);
                FreeBarCodes = Convert.ToString(dr["FreeBarCodes"]);
                FreeQty = dba.ConvertObjectToDouble(dr["FreeQty"]);
                FreePer = dba.ConvertObjectToDouble(dr["FreePer"]);
                MinPayAmt = dba.ConvertObjectToDouble(dr["MinPayAmt"]);

                if (
                      (MinPurAmt > 0 && (MinPurAmt >= TotalAmt)) || (MaxPurAmt > 0 && (MaxPurAmt <= TotalAmt))
                      || (DiscType != "" && DiscAmt > 0)
                      || (PurQty > 0 && (PurQty == ItemQty) && FreeQty > 0 && FreeBarCodes != "")
                      || (FixOfferAmt > 0)
                      || (RewardCoupan != "")
                      || (FreeBarCodes != "" && (MinPayAmt + FreePer) > 0)
                  )
                {
                    if ((Fromdt.Year < 1947 || dtBillDate >= Fromdt) && (Todt.Year < 1947 || dtBillDate < Todt))
                    {
                        if (PurQty > 0 && (PurQty == ItemQty))
                        {
                            ValidOffers.ImportRow(dr);
                        }
                        else if (PurQty == 0)
                            ValidOffers.ImportRow(dr);
                    }
                }
            }
            if (ValidOffers.Rows.Count > 0)
            {
                DataView DV = new DataView(ValidOffers);
                DataTable DT = DV.ToTable(true, "OfferName");

                _objData = new SearchData(DT, "OFFERAVAILABLE", "SELECT AVAILABLE OFFER", Keys.Space);
                _objData.ShowDialog();

                string strOffername = _objData.strSelectedData;
                string strPreOffername = Convert.ToString(dgrdDetails[20, rowIndex].Value);

                if ((strPreOffername != "" && strPreOffername != strOffername) || strOffername == "")
                {
                    RemoveOrLessFreeRow(rowIndex);
                    UnSetOfferRow(rowIndex);
                    UnApplyOffer(rowIndex);
                }

                if (strOffername != "")
                {
                    dgrdDetails[20, rowIndex].Value = strOffername;

                    DataRow[] Selected = OfferDT.Select("OfferName = '" + strOffername + "'");
                    DataTable _dt = Selected.CopyToDataTable();
                    OffName = Convert.ToString(_dt.Rows[0]["OfferName"]);
                    OfferDesc = Convert.ToString(_dt.Rows[0]["OfferDesc"]);

                    MinPurAmt = dba.ConvertObjectToDouble(_dt.Rows[0]["MinPurAmt"]);
                    MaxPurAmt = dba.ConvertObjectToDouble(_dt.Rows[0]["MaxPurAmt"]);
                    PurQty = dba.ConvertObjectToDouble(_dt.Rows[0]["PurQty"]);

                    IsAddOn = dba.ConvertObjectToDouble(_dt.Rows[0]["IsAddOn"]);
                    DiscType = Convert.ToString(_dt.Rows[0]["DiscType"]);
                    DiscAmt = dba.ConvertObjectToDouble(_dt.Rows[0]["DiscAmt"]);
                    FixOfferAmt = dba.ConvertObjectToDouble(_dt.Rows[0]["FixOfferAmt"]);

                    RewardCoupan = Convert.ToString(_dt.Rows[0]["RewardCoupan"]);
                    FreeBarCodes = Convert.ToString(_dt.Rows[0]["FreeBarCodes"]);
                    FreeQty = dba.ConvertObjectToDouble(_dt.Rows[0]["FreeQty"]);
                    FreePer = dba.ConvertObjectToDouble(_dt.Rows[0]["FreePer"]);
                    MinPayAmt = dba.ConvertObjectToDouble(_dt.Rows[0]["MinPayAmt"]);

                    DataRow[] drs = dtFreeItems.Select("Index = " + rowIndex);
                    if (drs.Length > 0)
                        dtFreeItems.Rows.Remove(drs[0]);

                    if (FreeBarCodes != "")
                    {
                        dtFreeItems.Rows.Add();
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["Index"] = rowIndex;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["Items"] = FreeBarCodes;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["FreeQty"] = FreeQty;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["FreePer"] = FreePer;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["MinPayAmt"] = MinPayAmt;
                    }

                    OfferOnIndex = rowIndex;
                    dgrdDetails.Rows[rowIndex].Cells["isAddOn"].Value = MainIsAddOn = IsAddOn;
                    this.FixOfferAmt = FixOfferAmt;

                    ApplyOffer(rowIndex, OffName, OfferDesc, RewardCoupan, DiscType, DiscAmt);
                }
                CalculateSpecialDiscount();
            }
        }

        private void GetOfferValus(int rowIndex, DataTable SelectedOffer)
        {
            try
            {
                MainIsAddOn = 0;

                var ItemAmt = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["amount"].Value);
                var ItemQty = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["qty"].Value);
                var ItemRate = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["rate"].Value);
                var ItemMRP = dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["mrp"].Value);
                var TotalAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text);

                string OffName = "", DiscType = "", RewardCoupan = "", FreeBarCodes = "", OfferDesc = "";
                double PurQty = 0, FreeQty = 0, IsAddOn = 0, MinPurAmt = 0, MaxPurAmt = 0, FixOfferAmt = 0, FreePer = 0, DiscAmt = 0, MinPayAmt = 0;

                DateTime dtBillDate = dba.ConvertDateInExactFormat(txtDate.Text);
                DateTime Fromdt = ConvertDateInExactFormat(Convert.ToString(SelectedOffer.Rows[0]["OffValidFrom"]));
                DateTime Todt = ConvertDateInExactFormat(Convert.ToString(SelectedOffer.Rows[0]["OffValidTo"]));

                OffName = Convert.ToString(SelectedOffer.Rows[0]["OfferName"]);
                OfferDesc = Convert.ToString(SelectedOffer.Rows[0]["OfferDesc"]);

                MinPurAmt = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["MinPurAmt"]);
                MaxPurAmt = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["MaxPurAmt"]);
                PurQty = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["PurQty"]);

                IsAddOn = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["IsAddOn"]);
                DiscType = Convert.ToString(SelectedOffer.Rows[0]["DiscType"]);
                DiscAmt = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["DiscAmt"]);
                FixOfferAmt = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["FixOfferAmt"]);

                RewardCoupan = Convert.ToString(SelectedOffer.Rows[0]["RewardCoupan"]);
                FreeBarCodes = Convert.ToString(SelectedOffer.Rows[0]["FreeBarCodes"]);
                FreeQty = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["FreeQty"]);
                FreePer = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["FreePer"]);
                MinPayAmt = dba.ConvertObjectToDouble(SelectedOffer.Rows[0]["MinPayAmt"]);
                int isValid = 0;

                if (
                        (MinPurAmt > 0 && (MinPurAmt >= TotalAmt)) || (MaxPurAmt > 0 && (MaxPurAmt <= TotalAmt))
                        || (DiscType != "" && DiscAmt > 0)
                        || (PurQty > 0 && (PurQty == ItemQty) && FreeQty > 0 && FreeBarCodes != "")
                        || (FixOfferAmt > 0)
                        || (RewardCoupan != "")
                        || (FreeBarCodes != "" && (MinPayAmt + FreePer) > 0)
                    )
                {
                    if ((Fromdt.Year < 1947 || dtBillDate >= Fromdt) && (Todt.Year < 1947 || dtBillDate < Todt))
                    {
                        if (PurQty > 0 && (PurQty == ItemQty))
                            isValid = 1;
                        else if (PurQty == 0)
                            isValid = 1;
                    }
                }

                if (isValid == 1)
                {
                    DataRow[] drs = dtFreeItems.Select("Index = " + rowIndex);
                    if (drs.Length > 0)
                        dtFreeItems.Rows.Remove(drs[0]);

                    if (FreeBarCodes != "")
                    {
                        dtFreeItems.Rows.Add();
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["Index"] = rowIndex;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["Items"] = FreeBarCodes;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["FreeQty"] = FreeQty;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["FreePer"] = FreePer;
                        dtFreeItems.Rows[dtFreeItems.Rows.Count - 1]["MinPayAmt"] = MinPayAmt;
                    }

                    OfferOnIndex = rowIndex;
                    dgrdDetails.Rows[rowIndex].Cells["isAddOn"].Value = MainIsAddOn = IsAddOn;
                    this.FixOfferAmt = FixOfferAmt;

                    ApplyOffer(rowIndex, OffName, OfferDesc, RewardCoupan, DiscType, DiscAmt);
                }
                else
                {
                    RemoveOrLessFreeRow(rowIndex);
                    UnSetOfferRow(rowIndex);
                    UnApplyOffer(rowIndex);
                }
            }
            catch (Exception ex) { }
        }

        private void ApplyOffer(int rowIndex, string OffName, string OfferDesc, string RCoupan, string DiscType, double DiscAmt)
        {
            try
            {
                if (rowIndex >= 0)
                {
                    string[] FreeItemsList = new string[0];
                    if (dtFreeItems.Rows.Count == 1)
                    {
                        DataRow[] drs = dtFreeItems.Select("Index = " + rowIndex);
                        if (drs.Length > 0)
                            FreeItemsList = Convert.ToString((drs[0]["Items"])).Split(',');
                    }

                    if (RCoupan != "")
                    {
                        dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = RCoupan;
                        GatCoupanOfferDetails(rowIndex, RCoupan);
                    }
                    else if (this.FixOfferAmt > 0)
                    {
                        dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = FixOfferAmt;
                    }
                    else if (FreeItemsList.Length > 0)
                    {
                        if (FreeItemsList.Length == 1 && FreeItemsList[0] != "")
                        {
                            AddOfferRow(rowIndex, Convert.ToString(FreeItemsList[0]));
                        }
                    }
                    else if (DiscType == "PER")
                    {
                        dgrdDetails.Rows[rowIndex].Cells["offerDisPer"].Value = DiscAmt;
                        if (MainIsAddOn == 0)
                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = "";
                    }
                    else if (DiscType == "AMT")
                    {
                        this.OfferAmt += DiscAmt;
                    }
                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value) == "")
                        dgrdDetails.Rows[rowIndex].Cells["offerName"].Value = OffName;
                }
            }
            catch (Exception ex) { }
        }

        private void AddOfferRow(int rowIndex, string OfferBarcode)
        {
            int LastIndex = 0;
            bool AddedQty = false;
            string ItemName = "";

            string[] FreeItemsList = new string[0];
            int FreeQty = 0;
            double FreePer = 0, MinPayAmt = 0;

            if (dtFreeItems.Rows.Count == 1)
            {
                DataRow[] drs = dtFreeItems.Select("Index = " + rowIndex);
                if (drs.Length > 0)
                    FreeItemsList = Convert.ToString((drs[0]["Items"])).Split(',');
                FreeQty = Convert.ToInt32(drs[0]["FreeQty"]);
                FreePer = dba.ConvertObjectToDouble(drs[0]["FreePer"]);
                MinPayAmt = dba.ConvertObjectToDouble(drs[0]["MinPayAmt"]);
            }

            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (rowIndex != row.Index)
                {
                    if (Convert.ToString(row.Cells["barcode_s"].Value) == OfferBarcode && Convert.ToString(row.Cells["OfferedBarcode"].Value).Contains("OFFERITEM"))
                    {
                        ItemName = Convert.ToString(row.Cells["itemName"].Value);

                        row.Cells["qty"].Value = Convert.ToInt32(row.Cells["qty"].Value) + FreeQty;
                        dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = ItemName;
                        dgrdDetails.Rows[rowIndex].Cells["OfferedBarcode"].Value = OfferBarcode + "<" + FreeQty.ToString();
                        AddedQty = true;
                    }
                }
            }
            if (!AddedQty)
            {
                dgrdDetails.Rows.Add();
                LastIndex = dgrdDetails.Rows.Count - 1;
                string[] str = OfferBarcode.Split('.');

                dgrdDetails.Rows[LastIndex].Cells["salesMan"].Value = dgrdDetails.Rows[rowIndex].Cells["salesMan"].Value;
                dgrdDetails.Rows[LastIndex].Cells["barCode"].Value = str[0];
                dgrdDetails.Rows[LastIndex].Cells["barCode_s"].Value = OfferBarcode;
                dgrdDetails.Rows[LastIndex].Cells["qty"].Value = FreeQty;
                dgrdDetails.Rows[LastIndex].ReadOnly = true;
                dgrdDetails.Rows[LastIndex].DefaultCellStyle.ForeColor = Color.Green;
                dgrdDetails.Rows[LastIndex].Cells["OfferedBarcode"].Value = "OFFERITEM<" + FreeQty;
                dgrdDetails.Rows[LastIndex].Cells["isAddOn"].Value = MainIsAddOn;

                GetBarcodeDetails(OfferBarcode, LastIndex, ref ItemName);
                if (MinPayAmt > 0)
                    dgrdDetails.Rows[LastIndex].Cells["mrp"].Value = MinPayAmt;
                else
                    dgrdDetails.Rows[LastIndex].Cells["offerDisPer"].Value = FreePer;

                dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = ItemName;
                dgrdDetails.Rows[rowIndex].Cells["OfferedBarcode"].Value = OfferBarcode + "<" + FreeQty.ToString();
                dgrdDetails.CurrentCell = dgrdDetails[3, LastIndex];
            }

            dgrdDetails.Rows[LastIndex].Cells[2].ReadOnly = false;
            DataRow[] dr = dtFreeItems.Select("Index = " + rowIndex);
            if (dr.Length > 0)
                dtFreeItems.Rows.Remove(dr[0]);
        }
        private void UnApplyOffer(int OfferIndex)
        {
            dgrdDetails.Rows[OfferIndex].Cells["offerName"].Value = "";
            dgrdDetails.Rows[OfferIndex].Cells["rewardCoupon"].Value = "";
            dgrdDetails.Rows[OfferIndex].Cells["rewardItem"].Value = "";
            dgrdDetails.Rows[OfferIndex].Cells["offerDisPer"].Value = "";
            dgrdDetails.Rows[OfferIndex].Cells["OfferedBarcode"].Value = "";
            dgrdDetails.Rows[OfferIndex].Cells["isAddOn"].Value = null;
            dgrdDetails.Rows[OfferIndex].Cells["disPer"].Value = "";

            DataRow[] dr = dtFreeItems.Select("Index = " + OfferIndex);
            if (dr.Length > 0)
                dtFreeItems.Rows.Remove(dr[0]);
            dba.GetSaleRate_Retail(dgrdDetails.Rows[OfferIndex], txtDate.Text);
            // GetSaleRate(dgrdDetails.Rows[OfferIndex]);
        }
        private void RemoveOrLessFreeRow(int OfferIndex)
        {
            DataRow[] drs = dtFreeItems.Select("Index = " + OfferIndex);
            if (drs.Length > 0)
                dtFreeItems.Rows.Remove(drs[0]);

            string Barcode = Convert.ToString(dgrdDetails.Rows[OfferIndex].Cells["OfferedBarcode"].Value);
            string Qty = Barcode.Substring(Barcode.IndexOf('<') + 1, (Barcode.Length - Barcode.IndexOf('<') - 1));
            if (Barcode != "" && !Barcode.Contains("OFFERITEM"))
            {
                int LessQty = Convert.ToInt32(Qty);
                Barcode = Barcode.Substring(0, Barcode.IndexOf('<'));
                if (Barcode != "")
                {
                    foreach (DataGridViewRow dr in dgrdDetails.Rows)
                    {
                        if (Convert.ToString(dr.Cells["barCode_s"].Value) == Barcode)
                        {
                            if (Convert.ToInt32(dr.Cells["qty"].Value) != LessQty)
                            {
                                dr.Cells["qty"].Value = Convert.ToInt32(dr.Cells["qty"].Value) - LessQty;
                            }
                            else
                            {
                                dgrdDetails.Rows.RemoveAt(dr.Index);
                                dgrdDetails.Rows[OfferIndex].Cells["offerName"].Value = "";
                                dgrdDetails.Rows[OfferIndex].Cells["rewardCoupon"].Value = "";
                                dgrdDetails.Rows[OfferIndex].Cells["rewardItem"].Value = "";
                                dgrdDetails.Rows[OfferIndex].Cells["offerDisPer"].Value = "";
                                dgrdDetails.Rows[OfferIndex].Cells["OfferedBarcode"].Value = "";
                                dgrdDetails.Rows[OfferIndex].Cells["isAddOn"].Value = null;
                            }
                        }
                    }
                }
            }
        }
        private void UnSetOfferRow(int FreeIndex)
        {
            string Barcode = Convert.ToString(dgrdDetails.Rows[FreeIndex].Cells["barCode_s"].Value);
            string OfferBarcode = Convert.ToString(dgrdDetails.Rows[FreeIndex].Cells["OfferedBarcode"].Value);
            if (Barcode != "" && OfferBarcode.Contains("OFFERITEM"))
            {
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToString(dr.Cells["OfferedBarcode"].Value).Contains(Barcode))
                    {
                        dgrdDetails.Rows[dr.Index].Cells["offerName"].Value = "";
                        dgrdDetails.Rows[dr.Index].Cells["rewardCoupon"].Value = "";
                        dgrdDetails.Rows[dr.Index].Cells["rewardItem"].Value = "";
                        dgrdDetails.Rows[dr.Index].Cells["offerDisPer"].Value = "";
                        dgrdDetails.Rows[dr.Index].Cells["OfferedBarcode"].Value = "";
                        dgrdDetails.Rows[dr.Index].Cells["isAddOn"].Value = null;
                    }
                }
            }
            else if (Barcode != "")
            {
                dgrdDetails.Rows[FreeIndex].Cells["offerName"].Value = "";
                dgrdDetails.Rows[FreeIndex].Cells["rewardCoupon"].Value = "";
                dgrdDetails.Rows[FreeIndex].Cells["rewardItem"].Value = "";
                dgrdDetails.Rows[FreeIndex].Cells["offerDisPer"].Value = "";
                dgrdDetails.Rows[FreeIndex].Cells["OfferedBarcode"].Value = "";
                dgrdDetails.Rows[FreeIndex].Cells["isAddOn"].Value = null;
            }
        }

        private void GetBarcodeDetails(string Barcode, int rowIndex, ref string ItemName)
        {
            string strQuery = " Select _Stock.*,BCD.BarCode,'' as SaleRate from( Select Distinct BarCode as _BarCode, BrandName, ItemName + '|' + Variant1 + '|' + Variant2 as AllRec , SUM(Qty)Qty, ItemName , Variant1,Variant2 from( Select BarCode, BrandName, DesignName, ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)Qty from( Select ISNULL(BarCode, '')BarCode, ISNULL(BrandName, '')BrandName, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 , SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE', 'OPENING', 'SALERETURN', 'STOCKIN') Group by BarCode, ISNULL(BrandName, ''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 UNION ALL Select Distinct ISNULL(_IS.Description, '')BarCode, _IM.BrandName, BuyerDesignName as DesignName, ItemName, ISNULL(Variant1, '')Variant1, ISNULL(Variant2, '')Variant2 , ISNULL(Variant3, '')Variant3, ISNULL(Variant4, '')Variant4, ISNULL(Variant5, '')Variant5, 1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode = _IS.BillCode and _IM.BillNo = _IS.BillNo Where _IM.Other = 'WITHOUT STOCK' and DisStatus = 0 UNION ALL Select ISNULL(BarCode, '')BarCode, ISNULL(BrandName, '')BrandName, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 , -SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN', 'SALES', 'STOCKOUT') Group by BarCode, ISNULL(BrandName, ''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2, Variant3, Variant4, Variant5  )Stock  Group by BarCode, BrandName, DesignName, ItemName, Variant1, Variant2, Variant3, Variant4, Variant5  )Stock   Group by BarCode, BrandName, ItemName, ItemName, Variant1, Variant2 having(SUM(Qty) > 0)   )_Stock    left join BarCodeDetails BCD on _Stock._BarCode = BCD.ParentBarCode Where BCD.BarCode is not NULL AND BCD.Barcode = '" + Barcode + "'  Order by ItemName  ";
            DataTable DT = dba.GetDataTable(strQuery);
            if (DT.Rows.Count > 0)
            {
                string strBarcode = Convert.ToString(DT.Rows[0]["BarCode"]);

                string[] str = strBarcode.Split('.');
                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = str[0];
                dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = Convert.ToString(DT.Rows[0]["BrandName"]);
                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = Convert.ToString(DT.Rows[0]["ItemName"]);
                ItemName = Convert.ToString(DT.Rows[0]["ItemName"]);

                if (DT.Columns.Contains("Variant1"))
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = Convert.ToString(DT.Rows[0]["Variant1"]);
                if (DT.Columns.Contains("Variant2"))
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = Convert.ToString(DT.Rows[0]["Variant2"]);
                if (DT.Columns.Contains("Variant3"))
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = Convert.ToString(DT.Rows[0]["Variant3"]);
                if (DT.Columns.Contains("Variant4"))
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = Convert.ToString(DT.Rows[0]["Variant4"]);
                if (DT.Columns.Contains("Variant5"))
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = Convert.ToString(DT.Rows[0]["Variant5"]);

                dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = Convert.ToString(DT.Rows[0]["Qty"]);

                if (str.Length > 1)
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strBarcode;


                dba.GetSaleRate_Retail(dgrdDetails.Rows[rowIndex], txtDate.Text);

                dgrdDetails.Rows[rowIndex].Cells["smInc"].Value = "";

                //ArrangeSerialNo();
                //CalculateAllAmount();
            }
        }

        private void IfOfferValidOnRow(DataGridViewRow row)
        {
            try
            {
                DateTime dtBillDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string billDate = dtBillDate.ToString("yyyy-MM-dd");
                //foreach (DataGridViewRow dgr in dgrdDetails.Rows)
                //{
                bool notValidOffer = false;
                string OfferName = Convert.ToString(row.Cells["OfferName"].Value);
                foreach (DataRow dr in OfferDT.Rows)
                {
                    DateTime Fromdt = ConvertDateInExactFormat(Convert.ToString(dr["OffValidFrom"]));
                    DateTime Todt = ConvertDateInExactFormat(Convert.ToString(dr["OffValidTo"]));
                    if (OfferName == Convert.ToString(dr["OfferName"])
                        && ((Fromdt.Year < 1947 || dtBillDate >= Fromdt) && (Todt.Year < 1947 || dtBillDate < Todt)))
                    {
                        notValidOffer = true;
                    }
                }
                if (notValidOffer)
                {
                    if (Convert.ToString(row.Cells["rewardItem"].Value) != "")
                        RemoveOrLessFreeRow(row.Index);

                    if (Convert.ToString(row.Cells["offerDisPer"].Value) != ""
                        || Convert.ToString(row.Cells["OfferedBarcode"].Value) != ""
                        || Convert.ToString(row.Cells["OfferName"].Value) != "")
                        dba.GetSaleRate_Retail(row, txtDate.Text);

                    row.Cells["offerName"].Value = "";
                    row.Cells["rewardCoupon"].Value = "";
                    row.Cells["rewardItem"].Value = "";
                    row.Cells["offerDisPer"].Value = "";
                    row.Cells["OfferedBarcode"].Value = "";
                    row.Cells["isAddOn"].Value = null;
                }
            }
            catch { }
        }

        private void ApplyOfferOLD(string OfferName, int rowIndex)
        {
            try
            {
                string strOfferNameOfr = "", strDiscTypeOfr = "", strRewardCouponOfr = "", strRewardItemOfr = "", strDepartmentOfr = "", strBrandOfr = "", strItemOfr = "", strCustomerOfr = "", strOfrStatus = "";
                DateTime dtItemDate = DateTime.Now, dtItemArrivalFrom = DateTime.Now, dtItemArrivalTo = DateTime.Now, dtOfferFrom = DateTime.Now, dtOfferTill = DateTime.Now;
                double dDiscount = 0, dAmtFrom = 0, dAmtTo = 0, dGrossAmt = 0;
                int QtyOfr = 0, FreeQtyOfr = 0, qty = 0, TotalQty = 0;

                dgrdDetails.Rows[rowIndex].Cells["offerDisPer"].Value = "0";
                //txtDiscPer.Text = txtDiscAmt.Text = "0.00";
                dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = "0.00";
                dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = "0.00";
                dgrdDetails.Rows[rowIndex].Cells["discAmt"].Value = "0.00";

                qty = Convert.ToInt32(dgrdDetails.Rows[rowIndex].Cells["qty"].Value);
                TotalQty = Convert.ToInt32(Convert.ToDouble(txtTotalQty.Text));
                dGrossAmt = ConvertObjectToDouble(txtGrossAmt.Text);

                if (OfferName != null)
                {
                    string strData = Convert.ToString(OfferName);
                    DataTable dtOffer = dba.GetDataTable("Select *,(case when GETDATE()> OFFER_VALID_TILL then 'InActive' else 'Active' end) STATUS from Offer_Master where Offer_Name='" + strData.Trim() + "'");
                    if (dtOffer.Rows.Count > 0)
                    {
                        DataRow dr = dtOffer.Rows[0];
                        strOfferNameOfr = Convert.ToString(dr["Offer_Name"]);
                        dtItemArrivalFrom = Convert.ToDateTime(dr["Item_Arrival_Date_From"]);
                        dtItemArrivalTo = Convert.ToDateTime(dr["Item_Arrival_Date_To"]);
                        dtOfferFrom = Convert.ToDateTime(dr["Offer_Valid_From"]);
                        dtOfferTill = Convert.ToDateTime(dr["Offer_Valid_From"]);
                        strDiscTypeOfr = Convert.ToString(dr["Disc_Type"]);
                        dDiscount = ConvertObjectToDouble(dr["Discount"]);
                        dAmtFrom = ConvertObjectToDouble(dr["Amount_From"]);
                        dAmtTo = ConvertObjectToDouble(dr["Amount_to"]);
                        QtyOfr = Convert.ToInt32(dr["Quantity"]);
                        FreeQtyOfr = Convert.ToInt32(dr["Free_Qty"]);
                        strRewardItemOfr = Convert.ToString(dr["Reward_Item"]);
                        strRewardCouponOfr = Convert.ToString(dr["Reward_Coupon"]);
                        strOfrStatus = Convert.ToString(dr["STATUS"]);

                        if (strOfrStatus == "Active")
                        {
                            dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = strRewardItemOfr;
                            dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = strRewardCouponOfr;

                            txtOfrDisAmt.Text = txtOfrDisPer.Text = "0.00";


                            if (strDiscTypeOfr == "PER" && QtyOfr > 0 && chkOfferApply.Checked)
                                dgrdDetails.Rows[rowIndex].Cells["offerDisPer"].Value = dDiscount;
                            else if (strDiscTypeOfr == "AMT" && QtyOfr > 0 && chkOfferApply.Checked)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["discAmt"].Value = dDiscount;
                            }
                            else if (strDiscTypeOfr == "AMT" && (dGrossAmt > dAmtFrom || dGrossAmt == dAmtFrom) && chkOfferApply.Checked)
                            {
                                txtOfrDisAmt.Text = dDiscount.ToString("N2", MainPage.indianCurancy);
                            }
                            else if (strDiscTypeOfr == "PER" && (dGrossAmt > dAmtFrom || dGrossAmt == dAmtFrom) && chkOfferApply.Checked)
                            {
                                txtOfrDisPer.Text = dDiscount.ToString("N2", MainPage.indianCurancy);

                            }
                            else
                            {
                                MessageBox.Show("Offer Not Valid...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells["offerName"];
                                dgrdDetails.CurrentRow.Cells["offerName"].Value = "";
                            }
                        }

                    }
                    else
                    {
                        dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["offerDisPer"].Value = "0";
                        dgrdDetails.Rows[rowIndex].Cells["discAmt"].Value = "";
                        txtOfrDisAmt.Text = txtOfrDisPer.Text = "0.00";
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private bool CheckBarCodeDuplicate(string strBarCode, int _index)
        {
            int _rowIndex = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (_rowIndex != _index)
                {
                    if (Convert.ToString(row.Cells["barcode_s"].Value) == strBarCode && !Convert.ToString(row.Cells["OfferedBarcode"].Value).Contains("OFFERITEM"))
                    {
                        if (MainPage._bBarCodeStatus && MainPage.strBarCodingType != "UNIQUE_BARCODE")
                        {
                            UnSetOfferRow(row.Index);
                            RemoveOrLessFreeRow(row.Index);

                            row.Cells["qty"].Value = ConvertObjectToDouble(row.Cells["qty"].Value) + 1;
                            calcGrossAmount(); //CalculateAllAmount();
                        }
                        return false;
                    }
                }
                _rowIndex++;
            }
            return true;
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
                                //if (strOldBarCode == strAllItem[0].Trim() && strOldBarCode != "")
                                //{
                                //    if (!MainPage.bUniqueBarCode)
                                //    {
                                //        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) + 1;
                                //        ArrangeSerialNo();
                                //        CalculateAllAmount();
                                //    }
                                //}
                                //else
                                {
                                    //if (strOldBarCode != "")
                                    //{
                                    //    dgrdDetails.Rows.Add(1);
                                    //    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                    //    if (dgrdDetails.Rows.Count > 1)
                                    //        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                    //    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                    //    rowIndex++;
                                    //}

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


                                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        if (str.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strBarcode;

                                        dgrdDetails.Rows[rowIndex].Cells["offerName"].Value = "";
                                        dgrdDetails.Rows[rowIndex].Cells["discAmt"].Value = "";
                                        dgrdDetails.Rows[rowIndex].Cells["rewardItem"].Value = "";
                                        dgrdDetails.Rows[rowIndex].Cells["rewardCoupon"].Value = "";
                                        //txtDiscAmt.Text = "0.00";
                                        //txtDiscPer.Text = "0.00";
                                        dba.GetSaleRate_Retail(dgrdDetails.Rows[rowIndex], txtDate.Text);

                                        //ArrangeSerialNo();
                                        //CalculateAllAmount();
                                        if (rowIndex == dgrdDetails.Rows.Count - 1)
                                        {
                                            if (dba.ConvertObjectToDouble(dgrdDetails.Rows[rowIndex].Cells["mrp"].Value) == 0)
                                                dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells["mrp"];
                                            else
                                            {
                                                dgrdDetails.Rows.Add(1);
                                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                                if (dgrdDetails.Rows.Count > 1)
                                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["salesMan"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["salesMan"].Value;
                                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                            }
                                            dgrdDetails.Focus();
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch
            {
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

                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = strAllItem[strAllItem.Length - 1];
                                dba.GetSaleRate_Retail(dgrdDetails.Rows[rowIndex], txtDate.Text);
                            }
                        }
                        //  ArrangeSerialNo();
                        // CalculateAllAmount();

                        if (rowIndex == dgrdDetails.Rows.Count - 1)
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
            }
            catch
            {
            }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {//
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
            }
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

                        if (IndexColmn < dgrdDetails.ColumnCount - 15)
                        {

                            IndexColmn += 1;
                            if (!dgrdDetails.Columns[IndexColmn].Visible)
                                IndexColmn++;

                            if (IndexColmn == 2 && isalesman == 0)
                                IndexColmn++;
                            if (IndexColmn == 3 && ibarcode == 0)
                                IndexColmn++;
                            if (IndexColmn == 4 && ibrandname == 0)
                                IndexColmn++;
                            if (IndexColmn == 5 && istylename == 0)
                                IndexColmn++;
                            if (IndexColmn == 6 && iitemname == 0)
                                IndexColmn++;
                            if (IndexColmn == 7)
                                IndexColmn++;
                            if (IndexColmn == 8)
                                IndexColmn++;
                            if (IndexColmn == 9)
                                IndexColmn++;
                            if (IndexColmn == 10)
                                IndexColmn++;
                            if (IndexColmn == 11)
                                IndexColmn++;
                            if (IndexColmn == 12 && iqty == 0)
                                IndexColmn++;
                            if (IndexColmn == 13 && iUOM == 0)
                                IndexColmn++;
                            if (IndexColmn == 14 && iMRP == 0)
                                IndexColmn++;
                            if (IndexColmn == 15 && iDis == 0)
                                IndexColmn++;
                            if (IndexColmn == 16)
                                IndexColmn++;
                            if (IndexColmn == 17 && irate == 0)
                                IndexColmn++;
                            if (IndexColmn == 18 && iamount == 0)
                                IndexColmn++;
                            if (IndexColmn == 19 && iSMI == 0)
                                IndexColmn++;
                            if (IndexColmn == 20)
                                IndexColmn++;
                            if (IndexColmn == 21)
                                IndexColmn++;
                            if (IndexColmn == 22)
                                IndexColmn++;
                            if (IndexColmn == 23)
                                IndexColmn++;
                            if (IndexColmn == 24 && istockqty == 0)
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
                                if (dgrdDetails.Columns[IndexColmn].Visible)
                                {
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                                }
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

                            if (IndexColmn == 28)
                            {
                                string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                                double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);
                                bool strIsOffer = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["OfferedBarcode"].Value).Contains("OFFERITEM");

                                if (strItemName != "" && (dAmt > 0 || strIsOffer))
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
                                    txtCustomerName.Focus();
                                }
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);
                            bool strIsOffer = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["OfferedBarcode"].Value).Contains("OFFERITEM");

                            if (strItemName != "" && (dAmt > 0 || strIsOffer))
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
                                txtCustomerName.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save(F5)")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        if (strID == "")
                        {
                            RemoveOrLessFreeRow(dgrdDetails.CurrentCell.RowIndex);
                            UnSetOfferRow(dgrdDetails.CurrentCell.RowIndex);
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
                                dgrdDetails.Enabled = true;
                            }
                            else
                            {
                                ArrangeSerialNo();
                            }
                            calcGrossAmount();  //CalculateAllAmount();
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
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update(F6)")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        string freeBarcode = Convert.ToString(dgrdDetails.CurrentRow.Cells["OfferedBarcode"].Value);
                        if (strID == "")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
                                dgrdDetails.Enabled = true;
                            }
                            else
                            {
                                ArrangeSerialNo();
                            }
                            calcGrossAmount();// CalculateAllAmount();
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (freeBarcode != "")
                                {
                                    string Qty = freeBarcode.Substring(freeBarcode.IndexOf('<') + 1, (freeBarcode.Length - freeBarcode.IndexOf('<') - 1));
                                    string Barcode = freeBarcode.Substring(0, freeBarcode.IndexOf('<'));
                                    if (Barcode != "")
                                    {
                                        foreach (DataGridViewRow dr in dgrdDetails.Rows)
                                        {
                                            if (Convert.ToString(dr.Cells["barCode_s"].Value) == Barcode)
                                            {
                                                DeleteOneRow(Convert.ToString(dr.Cells["id"].Value));
                                                dgrdDetails.Rows.RemoveAt(dr.Index);
                                                break;
                                            }
                                        }
                                    }
                                }
                                DeleteOneRow(strID);
                            }
                        }
                        UnSetOfferRow(dgrdDetails.CurrentCell.RowIndex);
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
            catch (Exception ex)
            { }
        }

        private void DeleteOneRow(string strID)
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
                    string chkQry = " SELECT SRD.billCode + ' ' +Cast(SRD.BillNo as varchar(20)) BillCodeNo FROM SaleReturnDetails SRD LEFT JOIN SalesBookSecondary SBS ON SRD.BarCode_S = SBS.BarCode_S AND SRD.BarCode = SBS.BarCode AND SRD.ItemName = SBS.ItemName  AND SRD.BrandName = SBS.BrandName AND SRD.Variant1 = SBS.Variant1 Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text + " and SBS.[SID]=" + strID;
                    DataTable dtc = dba.GetDataTable(chkQry);
                    if (dtc.Rows.Count == 0)
                    {
                        string strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and [SID]=" + strID + " ";

                        int _index = dgrdDetails.CurrentRow.Index;
                        dgrdDetails.Rows.RemoveAt(_index);
                        calcGrossAmount(); //CalculateAllAmount();
                                           // if (ValidateControls())
                        {
                            int result = UpdateRecord(strQuery);
                            if (result < 1)
                                BindRecordWithControl(txtBillNo.Text);
                            else
                            {
                                strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " ";
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
                    else
                    {
                        MessageBox.Show("Sorry ! this item is present in sale return bill no " + dtc.Rows[0]["BillCodeNo"] + " ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    if (e.ColumnIndex == 12)
                    {
                        RemoveOrLessFreeRow(e.RowIndex);
                        UnSetOfferRow(e.RowIndex);

                        ShowOnlyValidOffers(e.RowIndex);
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    }
                    else if (e.ColumnIndex == 15 || e.ColumnIndex == 14)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 17)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    //else if (e.ColumnIndex == 15)
                    //    CalculateAmountWithDiscOtherChargese(dgrdDetails.Rows[e.RowIndex]);
                }
            }
            catch
            {
            }
        }

        private void calcGrossAmount(bool _bStatus = true)
        {
            try
            {
                double dAmt = 0, dQty = 0, dTQty = 0, dRate = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dTQty += dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    row.Cells["amount"].Value = dRate * dQty;
                    dAmt += (dRate * dQty);
                }
                txtGrossAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                txtTotalQty.Text = dTQty.ToString("N2", MainPage.indianCurancy);

                calcOfferDisAmt();
                calcDisAmt();
                calcFinalAmt(_bStatus);
            }
            catch { }
        }
        private void calcOfferDisAmt()
        {
            txtOfrDisAmt.Text = (ConvertObjectToDouble(txtGrossAmt.Text) * ConvertObjectToDouble(txtOfrDisPer.Text) / 100).ToString("N2", MainPage.indianCurancy);
        }
        private void calcDisAmt()
        {
            txtDiscAmt.Text = (ConvertObjectToDouble(txtGrossAmt.Text) * ConvertObjectToDouble(txtDiscPer.Text) / 100).ToString("N2", MainPage.indianCurancy);
        }
        private void calcDisPer()
        {
            txtDiscPer.Text = "0.00";
            if (ConvertObjectToDouble(txtGrossAmt.Text) > 0)
            {
                txtDiscPer.Text = dba.ConvertObjToFormtdString(ConvertObjectToDouble(txtDiscAmt.Text) * 100 / ConvertObjectToDouble(txtGrossAmt.Text));
            }
        }
        //private void calcFinalAmt()
        //{
        //    try
        //    {
        //        double dGrossAmt = 0, dRoundOff = 0, dFinalAmt = 0, dTaxAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dTOtherAmt = 0, dNetAmt = 0;

        //        dOtherAmt = ConvertObjectToDouble(txtSign.Text + txtOtherAmount.Text);
        //        dGrossAmt = ConvertObjectToDouble(txtGrossAmt.Text);
        //        dTOtherAmt = dOtherAmt - ConvertObjectToDouble(txtDiscAmt.Text) - ConvertObjectToDouble(txtOfrDisAmt.Text);
        //        dFinalAmt = (dGrossAmt + dTOtherAmt);

        //        double dAmt = Convert.ToDouble(dFinalAmt.ToString("0"));
        //        dRoundOff = dFinalAmt - dAmt;

        //        if (dRoundOff >= 0)
        //            txtROSign.Text = "-";
        //        else
        //            txtROSign.Text = "+";

        //        txtRoundOff.Text = Math.Abs(dRoundOff).ToString("0.00");
        //        txtFinalAmt.Text = (dAmt).ToString("N2", MainPage.indianCurancy);

        //        //  SECTION GET TAXABLE 
        //        dTaxAmt = GetTaxAmount(dFinalAmt, dTOtherAmt, ref dTaxableAmt);

        //        dNetAmt = dFinalAmt + dTaxAmt + dTOtherAmt;

        //        if (dTaxableAmt > 0)
        //            lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
        //        else
        //            lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
        //        //  END TAXABALE GET SECTION

        //        calcNetAmt();
        //    }
        //    catch { }
        //}

        private void calcFinalAmt(bool _bStatus = true)
        {
            try
            {
                double dCardAMt, dChqAmt, dGrossAmt = 0, dRoundOff = 0, dFinalAmt = 0, dTaxAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dTOtherAmt = 0, dNetAmt = 0, dAdvAmt = 0, dReturnAmt = 0;

                dOtherAmt = ConvertObjectToDouble(txtSign.Text + txtOtherAmount.Text);
                dGrossAmt = ConvertObjectToDouble(txtGrossAmt.Text);
                dTOtherAmt = dOtherAmt - ConvertObjectToDouble(txtDiscAmt.Text) - ConvertObjectToDouble(txtOfrDisAmt.Text);
                dFinalAmt = (dGrossAmt + dTOtherAmt);

                //  SECTION GET TAXABLE 
                dTaxAmt = GetTaxAmount(dFinalAmt, dTOtherAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt;// + dTOtherAmt;

                double dAmt = Convert.ToDouble(dNetAmt.ToString("0"));
                dRoundOff = dNetAmt - dAmt;

                if (dRoundOff >= 0)
                    txtROSign.Text = "-";
                else
                    txtROSign.Text = "+";

                txtRoundOff.Text = Math.Abs(dRoundOff).ToString("0.00");
                txtFinalAmt.Text = (dAmt).ToString("N2", MainPage.indianCurancy);
                if (_bStatus)
                {
                    dAdvAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                    dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);
                    dCardAMt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                    dChqAmt = dba.ConvertObjectToDouble(txtChequeAmt.Text);
                    dAmt = (dAmt - (dAdvAmt + dReturnAmt + dChqAmt + dCardAMt));
                    if (dAmt > 0)
                        txtTenderAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        txtTenderAmt.Text = "0.00";
                }
                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                //  END TAXABALE GET SECTION
                calcNetAmt();
            }
            catch { }
        }

        private void calcNetAmt()
        {
            try
            {
                double dNetAmt = 0;
                dNetAmt = ConvertObjectToDouble(txtFinalAmt.Text) - ConvertObjectToDouble(txtCardAmt.Text) - ConvertObjectToDouble(txtChequeAmt.Text)
                            - ConvertObjectToDouble(txtAdvanceAmt.Text) - ConvertObjectToDouble(txtReturnAmt.Text);
                //if (dNetAmt > 0)
                txtNetAmt.Text = dNetAmt.ToString();

                calcRefundAmt();
            }
            catch { }
        }
        private void calcRefundAmt()
        {
            try
            {
                double dTenderAmt = 0, dNetAmt = 0, dRefundAmt = 0;
                dTenderAmt = ConvertObjectToDouble(txtTenderAmt.Text);
                dNetAmt = ConvertObjectToDouble(txtNetAmt.Text);
                dRefundAmt = (dTenderAmt - dNetAmt);
                if (dRefundAmt >= 0)
                {
                    txtNetAmt.Text = "0.00";
                    txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                    if (dNetAmt > 0)
                        txtCashAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        txtCashAmt.Text = "0.00";
                }
                else
                {
                    txtNetAmt.Text = Math.Abs(dRefundAmt).ToString("N2", MainPage.indianCurancy);
                    txtRefundAmt.Text = "0.00";
                    txtCashAmt.Text = Math.Abs(dTenderAmt).ToString("N2", MainPage.indianCurancy);
                }
            }
            catch { }
        }
        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dSQty = ConvertObjectToDouble(dgrdDetails.CurrentRow.Cells["stockQty"].Value), dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);//, dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
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
            calcGrossAmount();
            //CalculateAllAmount();
        }

        private void CalculateAmountWithMRP(DataGridViewRow rows)
        {
            double dDisPer = 0, dMRP = 0, dRate = 0, dDisAmt = 0, dOfrDisPer = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);
                dDisAmt = ConvertObjectToDouble(rows.Cells["discAmt"].Value);
                dOfrDisPer = ConvertObjectToDouble(rows.Cells["offerDisPer"].Value);
                dOfrDisPer = Math.Abs(dOfrDisPer);
                dDisPer = Math.Abs(dDisPer);
                if (dOfrDisPer != 0)
                {
                    dDisAmt = dMRP * dOfrDisPer / 100;
                }
                if (dOfrDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dOfrDisPer) / 100;
                else if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;
                if (dDisAmt != 0 && dDisAmt > 0)
                    dRate = dRate - dDisAmt;
                dRate = Math.Round(dRate, 2);

                rows.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["discAmt"].Value = dDisAmt.ToString("N2", MainPage.indianCurancy);

                calcGrossAmount();  //CalculateAllAmount();
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

                calcGrossAmount();  //CalculateAllAmount();
            }
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

        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, dMRP = 0, _dMRP = 0, dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0, dOfferDisPer = 0, dOfferDisAmt = 0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    _dMRP = dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);

                    dOfferDisPer = dba.ConvertObjectToDouble(row.Cells["offerDisPer"].Value);
                    dOfferDisAmt = dba.ConvertObjectToDouble(row.Cells["discAmt"].Value);

                    if (dSpclPer != 0 && dMRP != 0)
                    {
                        dSpclAmt += (((dMRP * dSpclPer) / 100.00) * dQty);
                        _dMRP = dMRP * (100.00 - dSpclPer) / 100.00;
                    }
                    else
                        _dMRP = dMRP;
                    dDisPer = Math.Abs(dDisPer);

                    int isAddOn = 0;
                    if (Convert.ToString(row.Cells["isAddOn"].Value) != "")
                    {
                        isAddOn = Convert.ToInt32(row.Cells["isAddOn"].Value);
                        if (_dMRP > 0)
                        {
                            if (isAddOn == 1)
                            {
                                dDisPer = dDisPer + dOfferDisPer;
                            }
                            else
                            {
                                dDisPer = dOfferDisPer;
                            }
                        }
                    }

                    //if ((dOfferDisPer != 0 || dSpclPer != 0) && _dMRP != 0)
                    //    dRate = _dMRP * (100.00 - (dOfferDisPer)) / 100.00;
                    //if (dOfferDisAmt != 0 && _dMRP != 0)
                    //{
                    //    dRate = _dMRP - dOfferDisAmt;
                    //}


                    //if ((dDisPer != 0 || dSpclPer != 0) && _dMRP != 0)
                    dRate = _dMRP * (100.00 - (dDisPer)) / 100.00;

                    //if (dRate == 0)
                    //    dRate = _dMRP;

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                }
                txtSplDisAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
                //txtDiscPer.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
                //txtDiscAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
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
                double dDisPer = 0, dCardAmt = 0, dCashAmt = 0, dChequeAmt = 0, dCreditAmt = 0, dAdvanceAmt = 0, dReturnAmt = 0, dOfferDisPer = 0, dOfferDisAmt = 0;

                dOfferDisPer = dba.ConvertObjectToDouble(txtOfrDisPer.Text);
                dOfferDisAmt = dba.ConvertObjectToDouble(txtOfrDisAmt.Text);

                dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text);
                dChequeAmt = dba.ConvertObjectToDouble(txtChequeAmt.Text);

                if (btnEdit.Text == "&Update(F6)")
                {
                    dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text);
                }
                //if (chkCreditSale.Checked)
                //    dCreditAmt = dba.ConvertObjectToDouble(txtCreditSale.Text);
                dAdvanceAmt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
                dReturnAmt = dba.ConvertObjectToDouble(txtReturnAmt.Text);


                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);
                }
                txtGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);

                dDisPer = ConvertObjectToDouble(txtDiscPer.Text);
                //dDiscAmt = ((dBasicAmt - dReturnAmt) * dDisPer) / 100;
                dDiscAmt = ConvertObjectToDouble(txtDiscAmt.Text);


                dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                dTOAmt = dOtherAmt - dDiscAmt - dOfferDisAmt;
                dFinalAmt = dBasicAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTOAmt;

                dPaybleAmt = dNetAmt - dAdvanceAmt - dReturnAmt - dCardAmt - dCashAmt - dCreditAmt - dChequeAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));// Math.Round(dNetAmt, 0);
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

                //if (btnEdit.Text != "&Update")
                //{
                // if (!chkTenderAmt.Checked)
                {
                    if (dPaybleAmt >= 0)
                    {
                        txtTenderAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);
                        txtRefundAmt.Text = "0.00";
                    }
                    else
                    {
                        txtRefundAmt.Text = Math.Abs(dPaybleAmt).ToString("N2", MainPage.indianCurancy);
                        txtTenderAmt.Text = "0.00";
                    }
                }
                //else
                //{
                //    double dTenderAmt = 0, dRefundAmt = 0;
                //    dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                //    dRefundAmt = dTenderAmt - dPaybleAmt;
                //    txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                //}
                //txtCashAmt.Text = dPaybleAmt.ToString("N2", MainPage.indianCurancy);
                //}

                //txtOfferAmt.Text = OfferAmt.ToString("N2", MainPage.indianCurancy);


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

                double dOfferDisAmt, dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dNetAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dRoundOff = 0, dPaybleAmt = 0;
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
                //if (chkCreditSale.Checked)
                //    dCreditAmt = dba.ConvertObjectToDouble(txtCreditSale.Text);
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
                dOfferDisAmt = dba.ConvertObjectToDouble(txtOfrDisAmt.Text);

                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                dTOAmt = dOtherAmt - dDiscAmt - dOfferDisAmt;
                dFinalAmt = dBasicAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                dNetAmt = dFinalAmt + dTaxAmt + dTOAmt;

                dPaybleAmt = dNetAmt - dAdvanceAmt - dReturnAmt - dCardAmt - dCashAmt - dCreditAmt - dChequeAmt;

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);
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


        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, ref double dTaxableAmt)
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

                            //dInsuranceAmt = dba.ConvertObjectToDouble(txtDiscAmt.Text);

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
                                strQuery = " Select SUM(TaxableAmt)TaxableAmt,SUM(ROUND(Amt,4)) as Amt,SUM(ROUND(Amt,2)) as TaxAmt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) OtherChargesAmt from (Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by HSNCode,TaxRate)_Sales  Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    //  BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    //if (dt.Rows.Count > 0)
                                    //    dOtherChargesAmt = dba.ConvertObjectToDouble(dt.Rows[0]["OtherChargesAmt"]);
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
                            //pnlTax.Visible = true;
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


        private void CalculateCardAmount()
        {
            try
            {
                double dAmt = 0;
                foreach (DataGridViewRow row in dgrdCardDetail.Rows)
                    dAmt += ConvertObjectToDouble(row.Cells["cAmt"].Value);
                //chkCardAmt.Checked = dAmt > 0 ? true : false;

                txtCardAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                calcNetAmt();// CalculateAllAmount();
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
                    dba.ClearTextBoxOnKeyDown(sender, e);
                    string strCName = txtCustomerName.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (value == 8)
                        txtMobileNo.Text = "";
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
                                txtCustomerName.Text = txtMobileNo.Text = txtCity.Text = "";
                            }
                            else
                            {
                                if (strMobileNo != "" || strStation != "")
                                {
                                    txtMobileNo.Text = strMobileNo;
                                    txtCity.Text = strStation;
                                }
                            }

                            if (txtCustomerName.Text != strCName)
                            {
                                txtAdvanceAmt.Text = txtReturnAmt.Text = "0.00";
                                txtAdvanceSlip.Text = txtReturnSlip.Text = "";
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

        private void txtSaleType_KeyDown(object sender, KeyEventArgs e)
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
                        calcGrossAmount();// CalculateAllAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void BindAdvanceSlip()
        {
            try
            {
                dgrdPendingAdv.Rows.Clear();
                string strCustomerID = "";
                string[] str = txtCustomerName.Text.Split(' ');
                if (str.Length > 0)
                {
                    strCustomerID = str[0];
                    string strQuery = "select Distinct (BillCode+' '+CAST(BillNo as varchar))BillNo,(RefundableAmt)Amount,Date from AdvanceAdjustment where CustomerName ='" + txtCustomerName.Text + "' and RefundableAmt>0 AND AdjustedNumber = '0' and Status=0 Order By Date desc";
                    //string strQuery = "Select Distinct (VoucherCode+' '+CAST(VoucherNo as varchar))VoucherNo,Amount,Date from BalanceAMount Where GSTNature='ADVANCE' and Tick='FALSE' and AccountID='"+ strCustomerID + "' Order By Date desc ";
                    DataTable _dt = dba.GetDataTable(strQuery);
                    if (_dt.Rows.Count > 0)
                    {
                        dgrdPendingAdv.Rows.Add(_dt.Rows.Count);
                        int _index = 0;
                        foreach (DataRow row in _dt.Rows)
                        {
                            dgrdPendingAdv.Rows[_index].Cells["chkPAdv"].Value = false;
                            dgrdPendingAdv.Rows[_index].Cells["advBillNo"].Value = row["BillNo"];
                            dgrdPendingAdv.Rows[_index].Cells["advAmt"].Value = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                            _index++;
                        }
                    }
                }
                else
                    MessageBox.Show("Sorry ! Please enter customer name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch { }
        }



        private void BindReturnSlip()
        {
            try
            {
                dgrdReturnSlip.Rows.Clear();
                if (txtCustomerName.Text != "")
                {
                    string strCustomerID = "";
                    string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtCustomerName.Text, "[^0-9.]", "");
                    if (strCustomer != "")
                    {
                        string[] _strFullName = txtCustomerName.Text.Split(' ');
                        if (_strFullName.Length > 1)
                            strCustomerID = _strFullName[0].Trim();
                    }
                    else
                        strCustomerID = txtCustomerName.Text;


                    string strQuery = "";// Select Distinct Description as VoucherNo,Amount,Date from BalanceAmount Where AccountStatus='SALE RETURN' and AccountID='" + strCustomerID + "' Order By Date desc ";
                    strQuery = "Select (BillCode+' '+CAST(BillNo as varchar)) as VoucherNo,(NetAmt-(CashAmt+ISNULL(_SR.RAmt,0))) as Amount from SaleReturn SR OUTER APPLY (Select SUM(ReturnAmt) RAmt from SalesBook SB Where SB.ReturnSlipNo=(SR.BillCode+' '+ CAST(SR.BillNo as varchar)))_SR Where SalePartyID='" + strCustomerID + "'  and (NetAmt-(CashAmt+ISNULL(_SR.RAmt,0)))>0 Order by Date desc ";
                    DataTable _dt = dba.GetDataTable(strQuery);
                    if (_dt.Rows.Count > 0)
                    {
                        dgrdReturnSlip.Rows.Add(_dt.Rows.Count);
                        int _index = 0;
                        foreach (DataRow row in _dt.Rows)
                        {
                            dgrdReturnSlip.Rows[_index].Cells["chkReturn"].Value = false;
                            dgrdReturnSlip.Rows[_index].Cells["returnVoucherNo"].Value = row["VoucherNo"];
                            dgrdReturnSlip.Rows[_index].Cells["returnAmt"].Value = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                            _index++;
                        }
                    }
                }
                else
                    MessageBox.Show("Sorry ! Please enter customer name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch { }
        }


        private void btnBillCancel_Click(object sender, EventArgs e)
        {
            pnlPendingAdvance.Visible = false;
        }


        private void AddSelectedAdvanceSlip()
        {
            try
            {
                txtAdvanceAmt.Text = "0.00";
                txtAdvanceSlip.Text = "";
                foreach (DataGridViewRow row in dgrdPendingAdv.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkPAdv"].Value))
                    {
                        txtAdvanceSlip.Text = Convert.ToString(row.Cells["advBillNo"].Value);
                        txtAdvanceAmt.Text = Convert.ToString(row.Cells["advAmt"].Value);
                        dAdvanceSlipAmt = ConvertObjectToDouble(row.Cells["advAmt"].Value);
                        break;
                    }
                }
                calcFinalAmt(); // CalculateAllAmount();
                pnlPendingAdvance.Visible = false;
            }
            catch
            {
            }
        }

        private void AddSelectedReturnSlip()
        {
            try
            {
                txtReturnAmt.Text = "0.00";
                txtReturnSlip.Text = "";
                foreach (DataGridViewRow row in dgrdReturnSlip.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkReturn"].Value))
                    {
                        txtReturnSlip.Text = Convert.ToString(row.Cells["returnVoucherNo"].Value);
                        txtReturnAmt.Text = Convert.ToString(row.Cells["returnAmt"].Value);
                        break;
                    }
                }
                calcFinalAmt();// CalculateAllAmount();
                pnlReturn.Visible = false;
                txtReturnAmt.Focus();
            }
            catch
            {
            }
        }

        private void txtAdvanceSlip_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
                {
                    if (!pnlPendingAdvance.Visible)
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        dba.ClearTextBoxOnKeyDown(sender, e);
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                        {
                            if (txtCustomerName.Text != "")
                            {
                                pnlPendingAdvance.Visible = true;
                                BindAdvanceSlip();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please enter customer name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            if (txtAdvanceSlip.Text == "")
                                txtAdvanceSlip.Clear();
                            e.Handled = true;
                        }
                        calcFinalAmt();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnAttachBill_Click(object sender, EventArgs e)
        {
            AddSelectedAdvanceSlip();
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            btnAdvance.Enabled = false;
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
                {
                    if (!pnlPendingAdvance.Visible)
                    {
                        if (txtCustomerName.Text != "")
                        {
                            pnlPendingAdvance.Visible = true;
                            BindAdvanceSlip();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Please enter customer name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch
            {
            }
            btnAdvance.Enabled = true;
        }

        private void btnReturnCancel_Click(object sender, EventArgs e)
        {
            pnlReturn.Visible = false;
        }

        private void btnReturnAdjustment_Click(object sender, EventArgs e)
        {
            AddSelectedReturnSlip();
        }

        private void txtReturnSlip_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
                {
                    if (!pnlReturn.Visible)
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        dba.ClearTextBoxOnKeyDown(sender, e);
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                        {
                            pnlReturn.Visible = true;
                            BindReturnSlip();
                        }
                        else
                        {
                            if (txtReturnSlip.Text == "")
                                txtReturnSlip.Clear();
                            e.Handled = true;
                        }
                        calcFinalAmt();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnReturnSlip_Click(object sender, EventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)"))
                {
                    if (!pnlReturn.Visible)
                    {
                        pnlReturn.Visible = true;
                        BindReturnSlip();
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
            txtCustomerName.Text = strHoldBilLCode = strHoldBillNo = txtMobileNo.Text = txtCity.Text = txtRemark.Text = txtReturnSlip.Text = txtAdvanceSlip.Text = txtRemark.Text = txtLocation.Text = txtChqSrNo.Text = "";
            chkEmail.Checked = chkSendSMS.Checked = false;

            txtCardAmt.Text = lblTaxableAmt.Text = txtCashAmt.Text = txtOtherAmount.Text = txtTotalQty.Text = txtAdvanceAmt.Text = txtReturnAmt.Text = txtSpclDisPer.Text = txtSplDisAmt.Text = txtTaxPer.Text = txtTaxAmt.Text = txtGrossAmt.Text = txtDiscPer.Text = txtDiscAmt.Text = txtRoundOff.Text = txtFinalAmt.Text = txtNetAmt.Text = txtTenderAmt.Text = txtRefundAmt.Text = txtChequeAmt.Text = txtOfrDisAmt.Text = txtOfrDisPer.Text = "0.00";
            txtSign.Text = txtROSign.Text = "-";
            pnlHold.Visible = false;
            dgrdCardDetail.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdPendingAdv.Rows.Clear();
            dgrdReturnSlip.Rows.Clear();
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
            txtDate.ReadOnly = txtCity.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtSpclDisPer.ReadOnly = txtDiscPer.ReadOnly = false;
            txtAdvanceAmt.ReadOnly = txtTenderAmt.ReadOnly = txtChequeAmt.ReadOnly = txtCardAmt.ReadOnly = txtDiscAmt.ReadOnly = false;
            //txtReturnAmt.ReadOnly =
            /// = txtCashAmt.ReadOnly
            //if (chkCashAmt.Checked)
            //{
            //    txtCashAmt.Enabled = true;
            //    txtCashAmt.ReadOnly = false;
            //}
            //if (chkCreditSale.Checked)
            //{
            //    txtCreditSale.Enabled = true;
            //    txtCreditSale.ReadOnly = false;
            //}
            dgrdDetails.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtDate.ReadOnly = txtCity.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtSpclDisPer.ReadOnly = txtDiscPer.ReadOnly = true;
            txtAdvanceAmt.ReadOnly = txtTenderAmt.ReadOnly = txtChequeAmt.ReadOnly = txtCardAmt.ReadOnly = txtDiscAmt.ReadOnly = true;
            //txtReturnAmt.ReadOnly =
            // txtCashAmt.Enabled = false;
            // txtCashAmt.ReadOnly = true;
            dgrdDetails.ReadOnly = true;
            lblMsg.Text = lblCreatedBy.Text = "";
        }

        private void SetSerialNo()
        {
            DataTable table = DataBaseAccess.GetDataTableRecord("Declare @BillCode nvarchar(250); Select @BillCode=SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select  @BillCode as SBillCode, (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode=@BillCode )SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='SALES' and TaxIncluded=1) TaxName  from SalesRecord Where BillCode=@BillCode )Sales ");
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
            if (txtLocation.Text == "" && blocation == true)
            {
                MessageBox.Show("Sorry ! Please enter location ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return false;
            }
            if (txtMobileNo.Text == "" && bmobile == true)
            {
                MessageBox.Show("Sorry ! Please enter register Mobile No. ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMobileNo.Focus();
                return false;
            }

            if (txtCity.Text == "" && bcity == true)
            {
                MessageBox.Show("Sorry ! Please enter City Name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCity.Focus();
                return false;
            }

            if (chkEmail.Checked == false && bemail == true)
            {
                MessageBox.Show("Sorry ! Please Check Email Box ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkEmail.Focus();
                return false;
            }
            if (chkSendSMS.Checked == false && bsms == true)
            {
                MessageBox.Show("Sorry ! Please Check SMS Box ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkSendSMS.Focus();
                return false;
            }
            if (txtCustomerName.Text == "" && bcustomername == true)
            {
                MessageBox.Show("Sorry ! Please enter register customer name for cedit sale ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            double dCardAmt = dba.ConvertObjectToDouble(txtCardAmt.Text), dCashAmt = dba.ConvertObjectToDouble(txtCashAmt.Text), dCreditSale = dba.ConvertObjectToDouble(txtNetAmt.Text);
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
            double dChequeAmt = dba.ConvertObjectToDouble(txtChequeAmt.Text);
            if (dChequeAmt > 0 && txtChqSrNo.Text == "")
            {
                MessageBox.Show("Sorry ! Please enter a cheque Serial No.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChequeAmt.Focus();
                return false;
            }

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
            bool _bStatus = dba.ValidateBackDateEntry_Only(txtDate.Text);
            if (!_bStatus)
                return false;

            string strSalesMan = "", strItem = "", strBarcode = "", strUnitname = "";
            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                strItem = Convert.ToString(rows.Cells["itemName"].Value);
                strSalesMan = Convert.ToString(rows.Cells["salesMan"].Value);
                strBarcode = Convert.ToString(rows.Cells["barCode"].Value);
                strUnitname = Convert.ToString(rows.Cells["unitName"].Value);

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
                    else if (strBarcode == "" && bBarcode)
                    {
                        MessageBox.Show("Sorry ! Barcode can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["barCode"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    else if (strUnitname == "")
                    {
                        MessageBox.Show("Sorry ! Unit name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["unitName"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
                IfOfferValidOnRow(rows);
            }
            calcGrossAmount(false);//CalculateAllAmountFinal();

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
            return ValidateStock();
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

        //private bool CheckQtyAvalability(DataTable dt)
        //{
        //    string strQuery = "", strSubQuery = "", strNCQuery = "";
        //    try
        //    {
        //        //SetGridViewBackGroundColor();
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            strSubQuery = "";                

        //            if (strQuery != "")
        //            {
        //                strQuery += " UNION ALL ";
        //                strNCQuery += " UNION ALL ";
        //            }

        //            strQuery += " Select BarCode,ItemName,Variant1,Variant2,SUM(PQty+SQty) Qty from ( "
        //                 + " Select BarCode,ItemName, Variant1, Variant2, SUM(Qty)PQty, 0 SQty from StockMaster Where BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " and BillType in ('OPENING', 'PURCHASE', 'SALERETURN') Group by BarCode,ItemName,Variant1,Variant2 UNION ALL "
        //                 + " Select BarCode,ItemName,Variant1,Variant2,0 PQty,-SUM(Qty) SQty from StockMaster Where BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " and BillType in ('SALES','PURCHASERETURN') Group by BarCode,ItemName,Variant1,Variant2 UNION ALL "
        //                 + " Select BarCode,ItemName,Variant1,Variant2,SUM(Qty) PQty,0 SQty from StockMaster Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " Group by BarCode,ItemName,Variant1,Variant2 "
        //                 + " UNION ALL Select '" + row["BarCode"].Value + "' as BarCode,'" + row["ItemName"].Value + "' as ItemName,'" + row["Variant1"].Value + "' as Variant1,'" + row["Variant2"].Value + "' as Variant2,0 as PQty, -" + row["Qty"].Value + " Qty )Stock Group by BarCode,ItemName, Variant1, Variant2 ";

        //            strNCQuery += " Select BarCode,ItemName,Variant1,Variant2,SUM(PQty+SQty) Qty from ( "
        //                     + " Select BarCode,ItemName, Variant1, Variant2, SUM(Qty)PQty, 0 SQty from StockMaster Where BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " and BillType in ('OPENING', 'PURCHASE', 'SALERETURN') Group by BarCode,ItemName,Variant1,Variant2 UNION ALL "
        //                     + " Select BarCode,ItemName,Variant1,Variant2,0 PQty,-SUM(Qty) SQty from StockMaster Where BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " and BillType in ('SALES','PURCHASERETURN') Group by BarCode,ItemName,Variant1,Variant2 UNION ALL "
        //                     + " Select BarCode,ItemName,Variant1,Variant2,SUM(Qty) PQty,0 SQty from StockMaster Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and Variant2='" + row["Variant2"].Value + "' " + strSubQuery + " Group by BarCode,ItemName,Variant1,Variant2 "
        //                     + " )Stock Group by BarCode,ItemName, Variant1, Variant2 ";

        //        }

        //        if (strQuery != "")
        //        {
        //            DataTable _dTable = null, _dtNC = null;  
        //                _dTable = dba.GetDataTable(strQuery);

        //                // if (MainPage._bTaxStatus)
        //                _dtNC = SearchDataOther.GetDataTable_NC(strNCQuery);
        //                //else
        //                //    _dtNC = SearchDataOther.GetDataTable_TC(strNCQuery);

        //                if (_dtNC != null && _dtNC.Rows.Count > 0)
        //                    _dTable.Merge(_dtNC, true);

        //            DataTable _dtStock = Generate_Stock_Table(_dTable);
        //            bool _bStatus = SetOutOfStockColor(_dtStock);
        //            if (!_bStatus)
        //            {
        //                lblMsg.Text = "Red color item is out of stock ! Unable to generate sale bill !!";
        //                lblMsg.ForeColor = Color.Red;
        //                return false;
        //            }
        //            else
        //            {
        //                lblMsg.Text = "";
        //                lblMsg.ForeColor = Color.DarkGreen;
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        //    return false;
        //}

        //private DataTable Generate_Stock_Table(DataTable dt)
        //{
        //    DataTable dTable = dt.DefaultView.ToTable(true, "BarCode","ItemName", "Variant1", "Variant2");
        //    dTable.Columns.Add("Quantity", typeof(String));
        //    object objValue = "";
        //    foreach (DataRow row in dTable.Rows)
        //    {
        //        objValue = dt.Compute("Sum(Qty)", "BarCode='" + row["BarCode"].Value + "' and ItemName='" + row["ItemName"].Value + "' and Variant1='" + row["Variant1"].Value + "' and [Variant2]='" + row["Variant2"].Value + "' ");
        //        row["Quantity"] = objValue;
        //    }
        //    return dTable;
        //}

        //private bool SetOutOfStockColor(DataTable dt)
        //{
        //    string strItemName = "", strVariant1 = "", strVariant2 = "",strBarCode="";
        //    double dQty = 0;
        //    bool _bStatus = true;
        //    try
        //    {

        //        DataRow[] rows = null;
        //        foreach (DataGridViewRow _row in dgrdDetails.Rows)
        //        {
        //            strItemName = Convert.ToString(_row.Cells["itemName"].Value);
        //            strVariant1 = Convert.ToString(_row.Cells["variant1"].Value);
        //            strVariant2 = Convert.ToString(_row.Cells["variant2"].Value);
        //            strBarCode= Convert.ToString(_row.Cells["barCode"].Value);
        //            rows = dt.Select("BarCode='"+ strBarCode+"' and ItemName = '" + strItemName + "' and Variant1 = '" + strVariant1 + "' and[Variant2] = '" + strVariant2 + "' ");

        //            if (rows.Length > 0)
        //            {
        //                dQty = dba.ConvertObjectToDouble(rows[0]["Quantity"]);
        //                if (dQty < 0)
        //                {
        //                    _row.DefaultCellStyle.BackColor = Color.Tomato;
        //                    _bStatus = false;
        //                }
        //                else
        //                    _row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
        //            }
        //            else
        //                _row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        _bStatus = false;
        //    }
        //    return _bStatus;
        //}


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

        private void SetFocusinGrid()
        {
            if (isno == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["srNo"];
            else if (isalesman == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["salesMan"];
            else if (ibarcode == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
            else if (ibrandname == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["brandName"];
            else if (istylename == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["styleName"];
            else if (iitemname == 1)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
            else
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
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
                    lblCreatedBy.Text = "";
                    EnableAllControl();
                    txtBillNo.ReadOnly = false;
                    ClearText();
                    chkEmail.Checked = chkSendSMS.Checked = true;
                    SetSerialNo();
                    SetFocusinGrid();
                    dgrdDetails.Focus();
                    btnSavePrint.Enabled = true;
                    btnAdd.TabStop = btnSavePrint.TabStop = true;
                    btnHold.Enabled = true;
                }
                else if (ValidateControls() && CheckBillNoAndSuggest() && ValidateOtherValidation(false))
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
                    {
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
                string strDate = "", strLRDate = "NULL", strPDate = "NULL", strBillCode = "", strBillNo = "", strAdvSlipStatus = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");
                bool _registeredParty = false;
                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dFinalAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dAdvanceAmt = 0, dReturnAmt = 0, dFOtherAmt = 0;
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
                //dFOtherAmt = _dOtherAmt;
                //if (txtSign.Text == "-")
                //    dFOtherAmt = (dFOtherAmt) * -1;

                //dFinalAmt = dGrossAmt + _dOtherAmt - dDisc;
                string strQuery = getAdvanceQuery("SAVE");
                strQuery += " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + "  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ") begin "
                                + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[MobileNo],[AdvanceSlipNo],[AdvanceAmt],[ReturnSlipNo],[ReturnAmt],[CardAmt],[CashAmt],[CreditAmt],[SaleBillType],[MaterialLocation],[TenderAmt],[RefundAmt],[ChequeAmt],[ChequeSerialNo],[OfferApplied],[GrossProfit],[OfferDisPer],[OfferDisAmt],[TaxableAmt]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','" + txtCity.Text + "','','','','0',''," + strLRDate + ",'','','" + txtRemark.Text + "','-',''," + strPDate + ",'','', " + dba.ConvertObjectToDouble(txtDiscPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",0,'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",0,0,'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ","
                                + " " + dba.ConvertObjectToDouble(txtTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + "," + dFinalAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strPetiAgent + "','','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",'" + txtMobileNo.Text + "','" + txtAdvanceSlip.Text + "'," + dAdvanceAmt + ",'" + txtReturnSlip.Text + "'," + dReturnAmt + "," + dCardAmt + "," + dCashAmt + ", " + dCreditAmt + ",'RETAIL','" + txtLocation.Text + "','" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + "','" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + "','" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + "','" + txtChqSrNo.Text + "','" + chkOfferApply.Checked + "','" + chkGrossProfit.Checked + "'," + dba.ConvertObjectToDouble(txtOfrDisPer.Text) + "," + dba.ConvertObjectToDouble(txtOfrDisAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ") ";

                if (_registeredParty && strSalePartyID != "")
                {
                    strQuery += " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dFinalAmt + "','DR','" + dFinalAmt + "','0','FALSE','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  ";
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

                //if (dCreditAmt > 0)
                //{
                //    dCreditAmt += dAdvanceAmt + dReturnAmt;

                //    strQuery += "  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                //                    + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCreditAmt + "','DR','" + dCreditAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "','CREDIT SALE')  ";
                //}

                double dQty = 0, dRate = 0, dMRP = 0, dDisPer = 0;
                string strSalesMan = "", strCoupanCode = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                    strCoupanCode = Convert.ToString(row.Cells["rewardCoupon"].Value);
                    strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                    if (strSalesMan != "" && strSalesMan != "DIRECT")
                    {
                        string[] _strFullName = strSalesMan.Split(' ');
                        if (_strFullName.Length > 0)
                        {
                            strSalesMan = _strFullName[0].Trim();
                        }
                    }
                    dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                    strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SalesMan],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[SaleIncentive],[BarCode_S],[OfferName],[OfferDisPer],[OfferDisAmt],[RewardItem],[RewardCoupon]) VALUES "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + strSalesMan + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                  + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + ", " + ConvertObjectToDouble(row.Cells["amount"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["smInc"].Value + "','" + row.Cells["barcode_s"].Value + "','" + row.Cells["offerName"].Value + "'," + ConvertObjectToDouble(row.Cells["offerDisPer"].Value) + "," + ConvertObjectToDouble(row.Cells["discAmt"].Value) + ",'" + Convert.ToString(row.Cells["OfferedBarcode"].Value) + "','" + row.Cells["rewardCoupon"].Value + "')";

                    if (MainPage._bTaxStatus || txtImportData.Text == "")
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
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                if (strCoupanCode != "")
                {
                    strQuery += " update Offer_Master Set CoupanUsedCount= ISNULL(CoupanUsedCount,0) + 1 WHERE REWARD_COUPON = '" + strCoupanCode + "' ";
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
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                if (strSaleBillType == "HOLD" && strHoldBilLCode != "" && strHoldBillNo != "")
                {
                    strQuery += "Delete from SalesBook Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " Delete from SalesBookSecondary Where BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " Delete from StockMaster Where BillType='SALES' and BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " ";
                }

                strQuery += " end ";

                if (strQuery != "")
                {
                    _count = dba.ExecuteMyQuery(strQuery);
                }
            }
            catch { }
            return _count;
        }

        private string getAdvanceQuery(string Mode)
        {
            string strQry = "", strAdvBillCode = "", strAdvBillNo = "";
            string[] strAdvSlipNo = txtAdvanceSlip.Text.Split(' ');
            double advamt = dba.ConvertObjectToDouble(txtAdvanceAmt.Text);
            if (txtAdvanceSlip.Text != "")
            {
                strAdvBillCode = strAdvSlipNo[0].Trim();
                strAdvBillNo = strAdvSlipNo[1].Trim();
            }
            if (Mode == "SAVE")
            {
                if (strAdvBillCode != "")
                {
                    strQry = " Update AdvanceAdjustment Set AdjustedAmt = AdjustedAmt + " + advamt + " where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo
                            + " Update AdvanceAdjustment Set RefundableAmt = TotalAmt - ReturnedAmt - AdjustedAmt where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo
                            + " IF(('" + txtBillCode.Text + " " + txtBillNo.Text + "') NOT IN (SELECT * FROM SplitString((SELECT AdjustedinSaleBillNo FROM AdvanceAdjustment WHERE BillCode = '" + strAdvBillCode + "' and BillNo=" + strAdvBillNo + "),','))) BEGIN Update AdvanceAdjustment Set AdjustedinSaleBillNo = AdjustedinSaleBillNo + '," + txtBillCode.Text + " " + txtBillNo.Text + "' where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo + " END";
                }
            }
            else if (Mode == "UPDATE")
            {
                if (strAdvBillCode != "")
                {
                    strQry = " Declare @oldAdvAmt Numeric(18,2)=0,@oldAdvSlipNo varchar(50)='' SELECT @oldAdvAmt = AdvanceAmt,@oldAdvSlipNo = AdvanceSlipNo FROM SalesBook WHERE BillCode = '" + txtBillCode.Text + "' AND BillNo = " + txtBillNo.Text
                        + " if(@oldAdvSlipNo != '') BEGIN Update AdvanceAdjustment Set AdjustedAmt = AdjustedAmt - @oldAdvAmt where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo"
                        + " Update AdvanceAdjustment Set RefundableAmt = TotalAmt - ReturnedAmt  - AdjustedAmt, AdjustedinSaleBillNo = REPLACE(AdjustedinSaleBillNo,'," + txtBillCode.Text + " " + txtBillNo.Text + "','') where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo END"

                        + " Update AdvanceAdjustment Set AdjustedAmt = AdjustedAmt + " + advamt + " where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo
                        + " Update AdvanceAdjustment Set RefundableAmt = TotalAmt - ReturnedAmt  - AdjustedAmt where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo
                        + " IF(('" + txtBillCode.Text + " " + txtBillNo.Text + "') NOT IN (SELECT * FROM SplitString((SELECT AdjustedinSaleBillNo FROM AdvanceAdjustment WHERE BillCode = '" + strAdvBillCode + "' and BillNo=" + strAdvBillNo + "),','))) BEGIN Update AdvanceAdjustment Set AdjustedinSaleBillNo = AdjustedinSaleBillNo + '," + txtBillCode.Text + " " + txtBillNo.Text + "' where BillCode='" + strAdvBillCode + "' and BillNo=" + strAdvBillNo + " END";
                }
                else
                {
                    strQry = " Declare @oldAdvAmt Numeric(18,2)=0,@oldAdvSlipNo varchar(50)='' SELECT @oldAdvAmt = AdvanceAmt,@oldAdvSlipNo = AdvanceSlipNo FROM SalesBook WHERE BillCode = '" + txtBillCode.Text + "' AND BillNo = " + txtBillNo.Text
                       + " if(@oldAdvSlipNo != '') BEGIN Update AdvanceAdjustment Set AdjustedAmt = AdjustedAmt - @oldAdvAmt where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo"
                       + " Update AdvanceAdjustment Set RefundableAmt = TotalAmt - ReturnedAmt  - AdjustedAmt, AdjustedinSaleBillNo = REPLACE(AdjustedinSaleBillNo,'," + txtBillCode.Text + " " + txtBillNo.Text + "','') where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo END";
                }
            }
            else
            {
                strQry = " Declare @oldAdvAmt Numeric(18,2)=0,@oldAdvSlipNo varchar(50)='' SELECT @oldAdvAmt = AdvanceAmt,@oldAdvSlipNo = AdvanceSlipNo FROM SalesBook WHERE BillCode = '" + txtBillCode.Text + "' AND BillNo = " + txtBillNo.Text
                   + " if(@oldAdvSlipNo != '') BEGIN Update AdvanceAdjustment Set AdjustedAmt = AdjustedAmt - @oldAdvAmt where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo"
                   + " Update AdvanceAdjustment Set RefundableAmt = TotalAmt - AdjustedAmt, AdjustedinSaleBillNo = REPLACE(AdjustedinSaleBillNo,'," + txtBillCode.Text + " " + txtBillNo.Text + "','') where BillCode + ' ' + Cast(BillNo as Varchar(20)) = @oldAdvSlipNo END";
            }
            return strQry;
        }
        private void SaveRecord()
        {
            try
            {
                int count = SaveRecordReturnInt();
                if (count > 0)
                {
                    string strMobileNo = "", strPath = "";
                    //  SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    //SendSMSToParty(strMobileNo);

                    MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add(F2)";
                    btnAdd.PerformClick();
                    //AskForPrint();
                    //BindRecordWithControl(txtBillNo.Text);
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
                    // BindRecordWithControl(txtBillNo.Text);

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
                        btnAdd.PerformClick();
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
                    if (bConfSmsSave == true)
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            int count = UpdateRecord("");
                            if (count > 0)
                            {
                                string strMobileNo = "", strPath = "";
                                //if (strBiltyPath != "")
                                //    SendEmailBiltyToSalesParty(false, ref strMobileNo, ref strBiltyPath);

                                //SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                                //SendSMSToParty(strMobileNo);

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
                            string strMobileNo = "", strPath = "";

                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit(F6)";
                            BindRecordWithControl(txtBillNo.Text);
                        }
                        else
                            MessageBox.Show("Sorry ! Record not updated, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dFOtherAmt = 0, dFinalAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dAdvanceAmt = 0, dReturnAmt = 0;
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

                //if (dCreditAmt != 0)
                //{
                //    double dTenderAmt = 0, dRefundAmt = 0;
                //    dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                //    dRefundAmt = dTenderAmt - dCreditAmt;
                //    txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                //}

                //dFOtherAmt = _dOtherAmt;
                //if (txtSign.Text == "-")
                //    dFOtherAmt = (dFOtherAmt) * -1;

                //dFinalAmt = dGrossAmt + dFOtherAmt - dDisc;
                string strQuery = getAdvanceQuery("UPDATE");
                strQuery += "  if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin "
                                + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='" + txtCity.Text + "',[TransportName]='',[WaybillNo]='',[WayBillDate]='',[NoOfCase]='0',[LRNumber]='',[LRDate]=" + strLRDate + ",[LRTime]='',[PvtMarka]='',[Remark]='" + txtRemark.Text + "',[Description]='-',[PackerName]='',[PackingDate]=" + strPDate + ",[CartonType]='',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DisAmt]=" + dDisc + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ","
                                + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=0,[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=0,[GreenTax]=0,[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(txtTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dFinalAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strPetiAgent + "',[Description_2]='' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[MobileNo]='" + txtMobileNo.Text + "',[AdvanceSlipNo]='" + txtAdvanceSlip.Text + "',[AdvanceAmt]=" + dAdvanceAmt + ",[ReturnSlipNo]='" + txtReturnSlip.Text + "',[ReturnAmt]=" + dReturnAmt + ",[CardAmt]=" + dCardAmt + ",[CashAmt]=" + dCashAmt + ", [CreditAmt]=" + dCreditAmt + ", [MaterialLocation]='" + txtLocation.Text + "', [TenderAmt]=" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + ", [RefundAmt]=" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + " , [ChequeAmt]=" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + ",[ChequeSerialno]='" + txtChqSrNo.Text + "',[OfferApplied]='" + chkOfferApply.Checked + "',[GrossProfit]='" + chkGrossProfit.Checked + "',[OfferDisPer]=" + txtOfrDisPer.Text + ",[OfferDisAmt]=" + txtOfrDisAmt.Text + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from [dbo].[CardDetails]Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                if (_registeredParty && strSalePartyID != "")
                {
                    strQuery += " if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') NOT IN ('CARD SALE','CASH SALE')) begin "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dFinalAmt + "','DR','" + dFinalAmt + "','0','FALSE','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  end else begin "
                             + " Update [dbo].[BalanceAmount] Set [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dFinalAmt + ",[AccountID]='" + strSalePartyID + "'  Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and ISNULL([AccountStatusID],'') NOT IN ('CARD SALE','CASH SALE') end "
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

                //if (dCreditAmt > 0)
                //{
                //    dCreditAmt += dAdvanceAmt + dReturnAmt;

                //    strQuery += "  if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ) begin "
                //                   + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                //                   + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dCreditAmt + "','DR','" + dCreditAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "','CREDIT SALE')  end else begin "
                //                   + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSalePartyID + "',[Amount]=" + dCreditAmt + ",[FinalAmount]='" + dCreditAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C'  and [AccountStatusID]='CREDIT SALE' end ";
                //}
                //else
                //    strQuery += " Delete from BalanceAmount Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatusID]='CREDIT SALE' ";

                string strID = "", strSalesMan = "";
                double dQty = 0, dRate = 0, _dAmt = 0, dMRP = 0, dDisPer = 0;
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
                    dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                    _dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                    strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SalesMan],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[SaleIncentive],[BarCode_S],[OfferName],[OfferDisPer],[OfferDisAmt],[RewardItem],[RewardCoupon]) VALUES "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + strSalesMan + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                 + " " + _dAmt + "," + _dAmt + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["smInc"].Value + "','" + row.Cells["barcode_s"].Value + "','" + row.Cells["offerName"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["offerDisPer"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["discAmt"].Value) + ",'" + Convert.ToString(row.Cells["OfferedBarcode"].Value) + "','" + row.Cells["rewardCoupon"].Value + "')";
                    }
                    else
                        strQuery += " Update [dbo].[SalesBookSecondary] SET [SalesMan]='" + strSalesMan + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ","
                                 + " [SDisPer]=" + dDisPer + ",[Rate]=" + dRate + ",[Amount]=" + _dAmt + ",[BasicAmt]=" + _dAmt + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[SaleIncentive]='" + row.Cells["smInc"].Value + "',[BarCode_S]='" + row.Cells["barcode_s"].Value + "',"
                                 + " [OfferName]='" + row.Cells["offerName"].Value + "',[OfferDisPer]=" + dba.ConvertObjectToDouble(row.Cells["offerDisPer"].Value) + ",[OfferDisAmt]=" + dba.ConvertObjectToDouble(row.Cells["discAmt"].Value) + ",[RewardItem]='" + Convert.ToString(row.Cells["OfferedBarcode"].Value) + "',[RewardCoupon]='" + row.Cells["rewardCoupon"].Value + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + "  ";

                    if (MainPage._bTaxStatus || txtImportData.Text == "")
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
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                strQuery += " end";

                // end Purchase Entry

                //object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");


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

                SaleBook_Retail objSaleBill_Retail = new SaleBook_Retail();
                objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSaleBill_Retail.ShowInTaskbar = true;

                objSaleBill_Retail.BindLastRecord();
                objSaleBill_Retail.Show();
                objSaleBill_Retail.txtBillNo.TabStop = true;
                dba.SelectInTextBox(objSaleBill_Retail.txtBillNo, 0, 0);
            }
            catch { }
        }

        private void SaleBook_Retail_Load(object sender, EventArgs e)
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

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    dba.ClearTextBoxOnKeyDown(sender, e);
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH CITY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCity.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }

        }

        private void txtMobileNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
                {
                    dba.ClearTextBoxOnKeyDown(sender, e);
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
                                txtCustomerName.Text = txtMobileNo.Text = txtCity.Text = "";
                            }
                            else
                            {
                                if (strCustomerName != "" || strStation != "")
                                {
                                    txtCustomerName.Text = strCustomerName;
                                    txtCity.Text = strStation;
                                }
                            }

                            if (txtMobileNo.Text != strMob)
                            {
                                txtAdvanceAmt.Text = txtReturnAmt.Text = "0.00";
                                txtAdvanceSlip.Text = txtReturnSlip.Text = "";
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strInvoiceCode = txtBillCode.Text, strInvoiceNo = txtBillNo.Text;
                if (btnAdd.Text == "&Add(F2)" || btnEdit.Text == "&Update(F6)")
                {
                    AlterationSlip _obj = new AlterationSlip(true, strInvoiceCode, strInvoiceNo);
                    _obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    //_obj.
                    _obj.ShowInTaskbar = true;
                    _obj.Show();
                }

            }
            catch (Exception ex)
            { }
        }

        private void btnCustomerAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SupplierMaster objSupplier = new SupplierMaster(1, "SUNDRY DEBTORS", "CUSTOMER");
                objSupplier.txtName.Text = txtCustomerName.Text;
                objSupplier.txtMobile.Text = txtMobileNo.Text;
                objSupplier.txtStation.Text = txtCity.Text;
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
                    calcFinalAmt();//CalculateAllAmount();
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

        private void txtAdvanceSlip_Leave(object sender, EventArgs e)
        {
            if (txtAdvanceSlip.Text == "")
            {
                txtAdvanceAmt.Text = "0.00";
                calcFinalAmt();
            }
            dba.ChangeLeaveColor(sender, e);
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
                double dCardAmt = 0, dCashAmt = 0, dCreditAmt = 0, dFinalAmt = ConvertObjectToDouble(txtFinalAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(txtGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dAdvanceAmt = 0, dReturnAmt = 0, dFOtherAmt = 0;
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


                if (txtAdvanceSlip.Text != "")
                {
                    string[] strAdvSlipNo = txtAdvanceSlip.Text.Split(' ');
                    if (strAdvSlipNo.Length > 1)
                    {
                        strBillCode = strAdvSlipNo[0].Trim();
                        strBillNo = strAdvSlipNo[1].Trim();
                    }
                }

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
                                 + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='" + txtCity.Text + "',[TransportName]='',[WaybillNo]='',[WayBillDate]='',[NoOfCase]='0',[LRNumber]='',[LRDate]=" + strLRDate + ",[LRTime]='',[PvtMarka]='',[Remark]='" + txtRemark.Text + "',[Description]='-',[PackerName]='',[PackingDate]=" + strPDate + ",[CartonType]='',[CartonSize]='',[DisPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DisAmt]=" + dDisc + ","
                                 + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=0,[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=0,[GreenTax]=0,[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(txtTotalQty.Text) + ",[GrossAmt]=" + dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dFinalAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='" + strPetiAgent + "',[Description_2]='' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[MobileNo]='" + txtMobileNo.Text + "',[AdvanceSlipNo]='" + txtAdvanceSlip.Text + "',[AdvanceAmt]=" + dAdvanceAmt + ",[ReturnSlipNo]='" + txtReturnSlip.Text + "',[ReturnAmt]=" + dReturnAmt + ",[CardAmt]=" + dCardAmt + ",[CashAmt]=" + dCashAmt + ", [CreditAmt]=" + dCreditAmt + ", [MaterialLocation]='" + txtLocation.Text + "', [TenderAmt]=" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + ", [RefundAmt]=" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + " , [ChequeAmt]=" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + ",[ChequeSerialno]='" + txtChqSrNo.Text + "' Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " "
                                 + " Delete from [dbo].[CardDetails]Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " "
                                 + " Delete from StockMaster Where BillType='SALES' and BillCode='" + strHoldBilLCode + "' and BillNo=" + strHoldBillNo + " ";



                    string strID = "", strSalesMan = "";
                    double dQty = 0, dRate = 0, _dAmt = 0, dMRP = 0, dDisPer = 0;
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
                        dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                        dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                        _dAmt = ConvertObjectToDouble(row.Cells["amount"].Value);
                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strID == "")
                        {
                            strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                     + " ('" + strHoldBilLCode + "'," + strHoldBillNo + ",0,'" + strSalesMan + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                     + " " + _dAmt + "," + _dAmt + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["smInc"].Value + "','')";
                        }
                        else
                            strQuery += " Update [dbo].[SalesBookSecondary] SET [SONumber]='" + strSalesMan + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + dDisPer + ",[Rate]=" + dRate + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[BasicAmt]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BarCode]='" + row.Cells["barCode"].Value + "',[BrandName]='" + row.Cells["brandName"].Value + "',[DesignName]='" + row.Cells["styleName"].Value + "',[Other1]='" + row.Cells["smInc"].Value + "' Where [BillCode]='" + strHoldBilLCode + "' and [BillNo]=" + strHoldBillNo + " and SID=" + strID + "  ";

                        if (MainPage._bTaxStatus || txtImportData.Text == "")
                        {
                            strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                     + " ('RETAIL_HOLD','" + strHoldBilLCode + "'," + strHoldBillNo + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                        }
                    }

                    foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails] ([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                       + " ('" + strHoldBilLCode + "'," + strHoldBillNo + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strHoldBilLCode + "'," + strHoldBillNo + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                    strQuery += " end";

                }

                else
                {

                    strBillCode = txtBillCode.Text + "H";

                    strQuery = "Declare @SerialNo bigint; Select @SerialNo=(ISNULL(MAX(_BillNo),0)+1) from (Select MAX(BIllNo)_BillNo from SalesBook Where BillCode='" + strBillCode + "' UNION ALL Select MAX(BIllNo)_BillNo from SalesRecord Where BillCode='" + strBillCode + "')_SALES  "
                                   + " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + strBillCode + "' and [BillNo]=@SerialNo) begin "
                                   + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[MobileNo],[AdvanceSlipNo],[AdvanceAmt],[ReturnSlipNo],[ReturnAmt],[CardAmt],[CashAmt],[CreditAmt],[SaleBillType],[MaterialLocation],[TenderAmt],[RefundAmt],[ChequeAmt],[ChequeSerialNo]) VALUES  "
                                   + " ('" + strBillCode + "',@SerialNo,'" + strDate + "','" + strSalePartyID + "','" + strSubPartyID + "','" + txtSalesType.Text + "','" + txtCity.Text + "','','','','0',''," + strLRDate + ",'','','" + txtRemark.Text + "','-',''," + strPDate + ",'','', " + dba.ConvertObjectToDouble(txtDiscPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",0,'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",0,0,'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ","
                                   + " " + dba.ConvertObjectToDouble(txtTotalQty.Text) + "," + dGrossAmt + "," + dFinalAmt + "," + dFinalAmt + ",'" + MainPage.strLoginName + "','',1,0,'','','','" + strPetiAgent + "','','" + txtImportData.Text + "'," + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",'" + txtMobileNo.Text + "','" + txtAdvanceSlip.Text + "'," + dAdvanceAmt + ",'" + txtReturnSlip.Text + "'," + dReturnAmt + "," + dCardAmt + "," + dCashAmt + ", " + dCreditAmt + ",'RETAIL_HOLD','" + txtLocation.Text + "','" + dba.ConvertObjectToDouble(txtTenderAmt.Text) + "','" + dba.ConvertObjectToDouble(txtRefundAmt.Text) + "','" + dba.ConvertObjectToDouble(txtChequeAmt.Text) + "','" + txtChqSrNo.Text + "') ";


                    double dQty = 0, dRate = 0, dMRP = 0, dDisPer = 0;
                    string strSalesMan = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                        dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                        dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                        strSalesMan = Convert.ToString(row.Cells["salesMan"].Value);
                        if (strSalesMan != "" && strSalesMan != "DIRECT")
                        {
                            string[] _strFullName = strSalesMan.Split(' ');
                            if (_strFullName.Length > 0)
                            {
                                strSalesMan = _strFullName[0].Trim();
                            }
                        }
                        dDisPer = Math.Abs(dba.ConvertObjectToDouble(row.Cells["disPer"].Value)) * -1;

                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                      + " ('" + strBillCode + "',@SerialNo,0,'" + strSalesMan + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                      + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + ", " + ConvertObjectToDouble(row.Cells["amount"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0,'" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','" + row.Cells["smInc"].Value + "','')";

                        if (MainPage._bTaxStatus || txtImportData.Text == "")
                        {
                            strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName],[Other1],[Other2]) VALUES "
                                 + " ('RETAIL_HOLD','" + strBillCode + "',@SerialNo,'" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "','" + row.Cells["barCode"].Value + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "','','') ";
                        }
                    }

                    foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails] ([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                       + " ('" + strBillCode + "',@SerialNo,'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";// end ";
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('SALES','" + strBillCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dFinalAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                    strQuery += " end ";
                }

                if (strQuery != "")
                {
                    _Count = dba.ExecuteMyQuery(strQuery);
                }
            }
            catch
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
                calcDisPer();
                calcFinalAmt();
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

        private void txtOfferName_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("OFFERAVAILABLE", "SELECT OFFER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtOfferName.Text = objSearch.strSelectedData;

                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void RemoveAllOffers()
        {
            try
            {
                foreach (DataGridViewRow dgr in dgrdDetails.Rows)
                {
                    //DataRow[] dr = dtFreeItems.Select("Index = " + dgr.Index);
                    //if (dr.Length > 0)
                    //    dtFreeItems.Rows.Remove(dr[0]);

                    if (Convert.ToString(dgrdDetails.Rows[dgr.Index].Cells["rewardItem"].Value) != "")
                        RemoveOrLessFreeRow(dgr.Index);

                    if (Convert.ToString(dgr.Cells["offerDisPer"].Value) != ""
                        || Convert.ToString(dgr.Cells["OfferedBarcode"].Value) != ""
                        || Convert.ToString(dgr.Cells["OfferName"].Value) != "")
                        dba.GetSaleRate_Retail(dgrdDetails.Rows[dgr.Index], txtDate.Text);

                    dgrdDetails.Rows[dgr.Index].Cells["offerName"].Value = "";
                    dgrdDetails.Rows[dgr.Index].Cells["rewardCoupon"].Value = "";
                    dgrdDetails.Rows[dgr.Index].Cells["rewardItem"].Value = "";
                    dgrdDetails.Rows[dgr.Index].Cells["offerDisPer"].Value = "";
                    dgrdDetails.Rows[dgr.Index].Cells["OfferedBarcode"].Value = "";
                    dgrdDetails.Rows[dgr.Index].Cells["isAddOn"].Value = null;
                }
                //OfferDT.Rows.Clear();
                calcGrossAmount();// CalculateAllAmount();
            }
            catch { }
        }

        private void chkOfferApply_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkOfferApply.Checked)
            {
                if (dgrdDetails.Rows.Count > 0)
                    RemoveAllOffers();
                txtOfferName.Enabled = false;
            }
            else
            {
                txtOfferName.Enabled = true;
            }
        }

        private void txtBillCode_Enter(object sender, EventArgs e)
        {
            dba.ChangeFocusColor(sender, e);
        }

        private void txtBillCode_Leave(object sender, EventArgs e)
        {
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtOfrDisPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                calcOfferDisAmt();
                calcFinalAmt();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtAdvanceAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
            {
                double dAdvAmt = ConvertObjectToDouble(txtAdvanceAmt.Text);
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "")
                    txtNew.Text = "0.00";
                else
                {
                    if (dAdvAmt > dAdvanceSlipAmt)
                    {
                        MessageBox.Show("Amount must be less than or equal to Advance Slip Amt (" + dAdvanceSlipAmt + ")");
                        txtAdvanceAmt.Focus();
                    }
                }

                //    CalculateAllAmount();
                calcDisAmt();
                calcFinalAmt();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void txtReturnSlip_Leave(object sender, EventArgs e)
        {
            if (txtReturnSlip.Text == "")
            {
                txtReturnAmt.Text = "0.00";
                calcFinalAmt();
            }
            dba.ChangeLeaveColor(sender, e);
        }

        private void btnTodaysSale_Click(object sender, EventArgs e)
        {
            try
            {
                btnTodaysSale.Enabled = false;
                string strDate = txtDate.Text.Length == 10 ? txtDate.Text : MainPage.strCurrentDate;
                CustomSaleRegister objCustomSaleReg = new CustomSaleRegister(txtCustomerName.Text, strDate, strDate);
                objCustomSaleReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objCustomSaleReg.ShowInTaskbar = true;
                objCustomSaleReg.Show();
            }
            catch { }
            btnTodaysSale.Enabled = true;
        }

        private void dgrdDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellFocusColor(sender, e);
        }

        private void dgrdDetails_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dba.ChangeCellLeaveColor(sender, e);
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
                            chkEmail.Checked = chkSendSMS.Checked = false;
                            btnHold.Enabled = true;
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
                                        + " delete from SalesBookSecondary where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
                                        + " delete from StockMaster where BillCode='" + strBCode + "' and BillNo='" + strBNo + "'"
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
                        //CalculateAllAmount();
                        calcFinalAmt();
                        dgrdCardDetail.Focus();
                    }
                    else
                    {
                        dgrdCardDetail.ReadOnly = true;
                        dgrdCardDetail.Rows.Clear();
                        dgrdCardDetail.Rows.Add();
                        dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                        calcFinalAmt();
                    }
                }
            }
            catch (Exception ex)
            { }

            dba.ChangeLeaveColor(sender, e);
        }

        //private void chkCashAmt_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (btnAdd.Text == "&Save(F5)" || btnEdit.Text == "&Update(F6)")
        //    {
        //        //txtCashAmt.Enabled = chkCashAmt.Checked ? true : false;
        //        //if (!chkCashAmt.Checked)
        //        //{
        //        //    txtCashAmt.Text = "0.00";
        //        //    txtCashAmt.ReadOnly = true;
        //        //    CalculateAllAmount();
        //        //}

        //    }
        //}

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
                calcDisAmt();
                calcFinalAmt();
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
                calcFinalAmt(); //CalculateAllAmount();

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
            //string strValue = "0";
            //if (_pstatus)
            //{
            //    strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
            //    if (strValue == "" || strValue == "0")
            //    {
            //        return false;
            //    }
            //}
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = (short)Convert.ToInt32(MainPage.strNoofCopy);
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;

            DataTable _dtGST = null, _dtSalesAmt = null, dtRtnDetails = null;
            bool _bIGST = false;
            DataTable dt = dba.SalesBookRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, ref dtRtnDetails, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (MainPage.strPrintLayout != "")
                {
                    if (MainPage.strPrintLayout == "T5")
                    {
                        Reporting.RetailSaleBookReportT5_72 objOL_salebill = new Reporting.RetailSaleBookReportT5_72();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        if (dtRtnDetails != null && dtRtnDetails.Rows.Count > 0)
                            objOL_salebill.Subreports[0].SetDataSource(dtRtnDetails);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                        objShow.myPreview.ReportSource = objOL_salebill;
                        objShow.ShowDialog();

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else if (MainPage.strPrintLayout == "T4")
                    {
                        Reporting.RetailSaleBookReportT4_80 objOL_salebill = new Reporting.RetailSaleBookReportT4_80();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("SALE BOOK PREVIEW");
                        objShow.myPreview.ReportSource = objOL_salebill;
                        objShow.ShowDialog();

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
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
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
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
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
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
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
            try
            {
                if (btnAdd.Text == "&Add(F2)" && btnEdit.Text == "&Edit(F6)")
                {
                    btnCreatePDF.Enabled = false;
                    DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strPath = "";
                        bool created = SetSignatureInBill(ref strPath, false, true, true);
                        if (created)
                            MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please Save the record...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private bool SetSignatureInBill(ref string strPath, bool _bPStatus, bool _createPDF, bool _dscVerified)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "";
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
                    if (_browser.ShowDialog() == DialogResult.OK)
                    {
                        if (_browser.FileName != "")
                            strPath = _browser.FileName;
                        if (File.Exists(strPath))
                            File.Delete(strPath);
                    }
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
                            return true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                strPath = "";
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return false;
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

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnPrint.Enabled = true;
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
                                string strQuery = getAdvanceQuery("DELETE");
                                strQuery += " Declare @BillCodeNo varchar(50)	SELECT @BillCodeNo = (BillCode +' '+ Cast(BillNo as Varchar(20))) FROM SaleReturn Where SaleBillCode = '" + txtBillCode.Text + "' AND SaleBillNo = " + txtBillNo.Text + " IF(ISNULL(@BillCodeNo,'') = '') BEGIN "
                                        + " Delete from SalesBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                        + " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                        + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('SALES A/C','DUTIES & TAXES','CARD RECEIVE','CASH RECEIVE')  "
                                        + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                        + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                        + " Delete from CardDetails Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                        + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                        + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtFinalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') SELECT '0' BillCodeNo END ELSE BEGIN SELECT ISNULL(@BillCodeNo,'') BillCodeNo END";

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    string rtnCode = Convert.ToString(dt.Rows[0]["BillCodeNo"]);
                                    if (rtnCode == "0")
                                    {
                                        DataBaseAccess.CreateDeleteQuery(strQuery);
                                        MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                        txtReason.Text = "";
                                        pnlDeletionConfirmation.Visible = false;
                                        BindNextRecord();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sorry ! This Sale bill used in Return Bill (" + rtnCode + ") !  ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        txtReason.Text = "";
                                        pnlDeletionConfirmation.Visible = false;
                                    }
                                }
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Record not deleted due to " + ex.Message + ", Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnFinalDelete.Enabled = true;
        }

        private void SaleBook_Retail_FormClosing(object sender, FormClosingEventArgs e)
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
                calcNetAmt();
                //TextBox txtNew = sender as TextBox;
                //if (txtNew.Text == "")
                //    txtNew.Text = "0.00";

                //double dTenderAmt = 0, dRefundAmt = 0, dNetAmt = 0;
                //dNetAmt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                //dTenderAmt = dba.ConvertObjectToDouble(txtTenderAmt.Text);
                //dRefundAmt = dTenderAmt - dNetAmt;
                //if (dRefundAmt > 0)
                //    txtRefundAmt.Text = dRefundAmt.ToString("N2", MainPage.indianCurancy);
                //else
                //    txtRefundAmt.Text = "0.00";

                ////if (dTenderAmt > dNetAmt)
                ////    txtCashAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                ////else
                ////    txtCashAmt.Text = dTenderAmt.ToString("N2", MainPage.indianCurancy);
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
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
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
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
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
            string strValue = "0", strPrintValues = "1";
            if (_pstatus)
            {
                strPrintValues = MainPage.strNoofCopy;
                if (strPrintValues == "")
                {
                    strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", strPrintValues, 400, 300);
                    if (strValue == "" || strValue == "0")
                    {
                        return false;
                    }
                }
                else
                    strValue = strPrintValues;
            }

            DataTable _dtGST = null, _dtSalesAmt = null, dtRtnDetails = null;
            bool _bIGST = false;
            DataTable dt = dba.SalesBookRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, ref dtRtnDetails, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                defS.Copies = (short)(int)Convert.ToDouble(strValue);
                defS.Collate = false;
                defS.FromPage = 0;
                defS.ToPage = 0;
                if (MainPage.strPrintLayout != "")
                {
                    if (MainPage.strPrintLayout == "T5")
                    {
                        Reporting.RetailSaleBookReportT5_72 objOL_salebill = new Reporting.RetailSaleBookReportT5_72();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        if (dtRtnDetails != null && dtRtnDetails.Rows.Count > 0)
                            objOL_salebill.Subreports[0].SetDataSource(dtRtnDetails);
                        objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                    }
                    else if (MainPage.strPrintLayout == "T4")
                    {
                        Reporting.RetailSaleBookReportT4_80 objOL_salebill = new Reporting.RetailSaleBookReportT4_80();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                        objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

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
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);

                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportT3_IGST objOL_salebill = new Reporting.RetailSaleBookReportT3_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
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
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
                            objOL_salebill.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            objOL_salebill.Close();
                            objOL_salebill.Dispose();
                        }
                        else
                        {
                            Reporting.RetailSaleBookReportGatePassT2_IGST objOL_salebill = new Reporting.RetailSaleBookReportGatePassT2_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                            objOL_salebill.Subreports[1].SetDataSource(_dtGST);
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
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSalesType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where  (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtCustomerName.Text + "' OR NAME LIKE('%CASH%') ");
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
                    //if (strRegion != "")
                    //{
                    //    if (strRegion == "LOCAL" && strSStateName != strCStateName  && strSStateName!="")
                    //    {
                    //        MessageBox.Show("You are entering a central transaction for a party belonging to same state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        //if (result == DialogResult.Yes)
                    //        //    return true;
                    //        //else
                    //        return false;
                    //    }
                    //    else if (strRegion == "INTERSTATE" && strSStateName == strCStateName && strSStateName!="")
                    //    {
                    //        MessageBox.Show("You are entering a local  transaction for a party belonging to other  state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        //if (result == DialogResult.Yes)
                    //        //    return true;
                    //        //else
                    //        return false;
                    //    }
                    //}
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
