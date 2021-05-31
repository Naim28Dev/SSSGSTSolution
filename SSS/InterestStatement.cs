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
    public partial class InterestStatement : Form
    {
        DataBaseAccess dba;
        string[] strColor = { "LightSteelBlue", "PeachPuff", "Thistle", "Lavender", "LightSalmon", "LightCoral", "ButtonShadow", "BurlyWood", "Gainsboro", "Beige" };
        int index = 0;
        ChangeCurrencyToWord currency;
        string[] strAllParty, strPartyStatus;
        protected internal string strCategoryName = "", strDiscountName = "", _STRChqDate = "", _STRGrade = "", _STRCategory = "", _STRAmtLimit = "", _STRMobileNo = "", _STRBlackList = "", _STRTransasactionLock = "";//,_STRLastPaymentDate="",_STRLastPaymentAmt="";
        double dNetSaleAmt = 0, dNetPurchaseAmt = 0;
        HideRecords objHide;
        bool oldBillStatus = false, _bLockdownStatus = false;
        protected internal DataTable dtDiscountDetails = null;//, _dtIntDiscDetails = null;
        
        SearchData objSearch = null;
        public static InterestStatement objInterest;

        public InterestStatement()
        {
            InitializeComponent();
            GetInitialData();
        }

        public InterestStatement(bool mStatus)
        {
            InitializeComponent();
            GetInitialData();
            if (mStatus)
            {
                btnSelectCompany.Enabled = true;
                GetMultiQuarterName();
            }
        }

        public InterestStatement(string strPartyName, bool mStatus)
        {
            InitializeComponent();
            GetInitialData();
            if (mStatus)
            {
                btnSelectCompany.Enabled = true;
                GetMultiQuarterName();
            }
            if (strPartyName != "")
            {
                txtParty.Text = strPartyName;
                GetPartyDueDaysAndCDDays();
                SearchRecord();
            }
        }

        private void GetInitialData()
        {
            dba = new DataBaseAccess();
            objHide = new HideRecords();
            objInterest = this;
            string strQuery = "Select DaysInYear,GraceDays,CashDiscDays,DrInterest,CrInterest,CashDiscRate,Rebate from CompanySetting "
                                  + " Select CategoryID,CategoryName,GraceDays,CDDays,DiscountPer,DiscountName,DiscountStatus from DiscountDetails Order by DiscountStatus,CategoryName,CDDays asc";
            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtDaysInYr.Text = Convert.ToString(row["DaysInYear"]);
                    txtGraceDays.Text = Convert.ToString(row["GraceDays"]);
                    txtCDDays.Text = Convert.ToString(row["CashDiscDays"]);
                    txtRateDr.Text = Convert.ToString(row["DrInterest"]);
                    txtRateCr.Text = Convert.ToString(row["CrInterest"]);
                    txtCD.Text = Convert.ToString(row["CashDiscRate"]);
                    txtWSR.Text = Convert.ToString(row["Rebate"]);
                }
                dtDiscountDetails = ds.Tables[1];
            }
            txtParty.Focus();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            txtLastDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");

            //btnPreview.Enabled = btnPrint.Enabled = MainPage.strProductType.Contains("NET") || MainPage.strUserRole.Contains("ADMIN") ? true : false;
        }

        private void txtAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e, dgrdRelatedParty);

                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                   // if (objSearch == null)
                   // {
                        objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS NAME", e.KeyCode);
                   // }
                   // else
                   // {
                   //     string strNPad = "";
                   //     objSearch.txtSearch.Text = "";
                   //     if (Keys.Space != e.KeyCode && e.KeyCode != Keys.F2)
                   //     {
                   //         strNPad = e.KeyCode.ToString();
                   //         if (strNPad.Contains("NumPad"))
                   //             strNPad = strNPad.Replace("NumPad", "");
                   //         objSearch.txtSearch.Text = strNPad;
                   //         objSearch.txtSearch.SelectionStart = 1;
                   //     }
                   //     if (objSearch.lbSearchBox.Items.Count > 0)
                   //         objSearch.lbSearchBox.SelectedIndex = 0;
                   // }
                    objSearch.txtSearch.Focus();
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtParty.Text = objSearch.strSelectedData;
                        GetPartyDueDaysAndCDDays();
                    }
                    ClearRecord();
                    GetRelatedpartyDetails();
                }
                else
                {
                    if (txtParty.Text == "")
                        ClearRecord();
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }


        private void GetPartyDueDaysAndCDDays()
        {
            try
            {
                string strQuery = "Select (Case When (DueDays!='' AND DueDays!='0') then DueDays else (Select TOP 1 GraceDays from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') end)GraceDays,(Select TOP 1 CashDiscDays from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "')CDDays,MobileNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtParty.Text + "'";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtGraceDays.Text = Convert.ToString(dt.Rows[0]["GraceDays"]);
                    txtCDDays.Text = Convert.ToString(dt.Rows[0]["CDDays"]);

                    //if (Convert.ToString(dt.Rows[0]["MobileNo"]) != "")
                    //    lblMobileNo.Text = "Mobile No : " + dt.Rows[0]["MobileNo"];
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            chkTax.Checked = true;
        }


        private void ClearRecord()
        {
            dgrdInterest.Rows.Clear();
            lblGrossAmt.Text = lblCrAmt.Text = lblDrAmt.Text = lblBalance.Text = lblFinalBal.Text = lblIntCr.Text = lblIntDr.Text = lblWSR.Text = lblCDiscount.Text = "0.00";
            lblAvgDays.Text = "0";
            strCategoryName = "";
            lblHeader.Text = "GENERAL INTEREST";
            objHide = null;

        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
            {
                txtFromDate.Enabled = txtToDate.Enabled = true;
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            else
                txtFromDate.Enabled = txtToDate.Enabled = false;
        }

        private void InterestStatement_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlRelatedParty.Visible)
                        pnlRelatedParty.Visible = false;
                    else if (pnlColor.Visible)
                        pnlColor.Visible = false;
                    else if (panelCompany.Visible)
                        panelCompany.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch
            {
            }
        }

        private void InterestStatement_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (!MainPage.mymainObject.bLedgerReport)
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
                else
                {
                    if (!btnSelectCompany.Enabled)
                    {
                        MainPage.multiQSDate = MainPage.startFinDate;
                        MainPage.multiQEDate = MainPage.endFinDate;
                    }
                    else if (!MainPage.mymainObject.bMultiCompany)
                    {
                        MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    }
                    else
                    {
                        txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                        txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
                    }

                    btnSendEmail.Enabled = MainPage.mymainObject.bSMSReport;
                }
            }
            catch
            { }
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            if (btnSelectCompany.Enabled)
                dba.GetDateInExactFormat(sender, chkDate.Checked, false, false, true);
            else
                dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtLastDate_Leave(object sender, EventArgs e)
        {
            //if (btnSelectCompany.Enabled)
            //    dba.GetStringFromDateForMultiQuarterReporting(txtLastDate);
            //else
            dba.GetDateInExactFormat(sender, true, false, true);
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = chkAll.Checked = false;
            SearchRecord();
            btnGo.Enabled = true;
        }

        private void SearchRecord()
        {
            try
            {
                if (txtParty.Text == "")
                {
                    MessageBox.Show(" Sorry ! Party name can't be blank ! ", "Party name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParty.Focus();
                }
                else if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    lblTaxAmt.Text = "0.00 Dr";
                    txtCD.Text = "0";
                    //chkTax.Checked = true;
                    lblDiscountName.Text = "Disc. Type";
                    objHide = new HideRecords();
                    if (btnSelectCompany.Enabled)
                        GetMultiQuarterDetails();
                    else
                        GetCurrentQuarterDetails();
                    panelCompany.Visible = false;

                    lblHeader.Text = "GENERAL INTEREST";
                    if (strCategoryName != "")
                        lblHeader.Text += " (" + strCategoryName + ")";
                    if (strDiscountName != "")
                        lblDiscountName.Text = strDiscountName;
                }
            }
            catch
            {
            }
        }


        private void SetNewAndOldScheme(DataTable table)
        {         
            if (table != null)
            {
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        txtCDDays.Text = "15";
                        txtCD.Text = "1";

                        if (strCategoryName == "")
                            strCategoryName = Convert.ToString(table.Rows[table.Rows.Count-1]["CategoryName"]);

                        DataRow[] rows = table.Select("Date<'01/01/2019' and AccountStatus='SALES A/C' ");
                        if (rows.Length > 0 && rdoNew.Checked)
                        {
                            DialogResult result = MessageBox.Show("There are old sale bills in this statement, Are you want to calculation with new scheme ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (DialogResult.Yes == result)
                            {
                                 GetPartyDueDaysAndCDDays();
                                //if (strCategoryName == "WHOLESALER")
                                //    txtGraceDays.Text = "60";
                                //else
                                //    txtGraceDays.Text = "45";
                                oldBillStatus = false;
                            }
                            else
                            {
                                oldBillStatus = true;
                                if (strCategoryName == "WHOLESALER")
                                    txtGraceDays.Text = "45";
                                else
                                    txtGraceDays.Text = "30";
                            }
                        }
                        else
                        {
                            rows = table.Select("Date>='01/01/2019' and AccountStatus='SALES A/C' ");
                            if (rows.Length > 0 && rdoOld.Checked)
                            {
                                DialogResult result = MessageBox.Show("There are new sale bills in this statement, Are you want to calculation with old scheme ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (DialogResult.Yes == result)
                                {
                                    oldBillStatus = true;
                                    if (strCategoryName == "WHOLESALER")
                                        txtGraceDays.Text = "45";
                                    else
                                        txtGraceDays.Text = "30";
                                }
                                else
                                {
                                    GetPartyDueDaysAndCDDays();
                                    //if (strCategoryName == "WHOLESALER")
                                    //    txtGraceDays.Text = "60";
                                    //else
                                    //    txtGraceDays.Text = "45";
                                    oldBillStatus = false;
                                }
                            }
                            else if (rdoNew.Checked)
                            {
                                GetPartyDueDaysAndCDDays();
                                //if (strCategoryName == "WHOLESALER")
                                //    txtGraceDays.Text = "60";
                                //else
                                //    txtGraceDays.Text = "45";
                                oldBillStatus = false;
                            }
                            else
                            {
                                oldBillStatus = true;
                                if (strCategoryName == "WHOLESALER")
                                    txtGraceDays.Text = "45";
                                else
                                    txtGraceDays.Text = "30";
                            }
                        }
                    }
                }
                catch
                {
                }
                SetRecordWithDataTable(table);
            }
        }

        public void GetCurrentQuarterDetails()
        {
            ClearRecord();
            string strQuery = "", strPartyID = "", strSubQuery = CreateQuery(ref strPartyID,false);

            strQuery += " Select Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,FinalAmount,AdjustedNumber,MultiCompanyNo,UserName,(Select Category from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))=PartyName) CategoryName,0 as Onaccount,GDays from ( "
                     + " Select 0 as ID,AccountID as PartyName,Date,AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,BA.FinalAmount,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,0 as GDays from BalanceAmount BA left join AdjustedIds AID on BA.BalanceID=AID.BalanceID and AID.MultiCompanyNo=0  Where AccountStatus='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " Union All  "
                     + " Select 1 as ID,AccountID as PartyName, (CASE WHEN (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) then ChqDate else Date end) as Date,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' OR AccountStatus='TCS CREDIT NOTE' OR AccountStatus='TCS DEBIT NOTE' OR AccountStatus='DUTIES & TAXES' then AccountStatus else AccountStatus end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, "
                     + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,BA.FinalAmount,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,(CASE WHEN AccountStatus='SALES A/C' then ISNULL((Select Top 1 GDM.BuyerDays as GDays from SalesRecord SR CROSS APPLY (Select GRSNO from SalesEntry SE Where SR.BillCode=SE.BillCode and SR.BillNo=SE.BillNo)SE Cross Apply(Select OrderNo,SalePartyID from GoodsReceive GR  Where SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)))GR Cross Apply (Select OfferName,SalePartyID as OBSalePartyID from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo)OB Cross Apply (Select BuyerDays from GraceDaysMaster GDM Where GDM.OfferName=OB.OfferName) GDM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BA.Description and GR.SalePartyID=BA.AccountID and OB.OBSalePartyID=BA.AccountID and SR.SalePartyID=BA.AccountID),0) else 0 end) as GDays from BalanceAmount BA left join AdjustedIds AID on BA.BalanceID=AID.BalanceID  and AID.MultiCompanyNo=0  Where AccountStatus!='OPENING' and CAST(Amount as Money)>0 and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1  " + strSubQuery + " ) Balance Order By ID,Date"
                     + " Select CONVERT(varchar,Date,103)BDate,(CASE When AccountStatus='JOURNAL A/C' then AccountStatus else (AccountID+' '+Name) end +(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and SM.TINNumber='COST CENTRE') SM Where AccountID!=''  " + strSubQuery.Replace(" AccountID=", " CostCentreAccountID=") + " Order By Date  "
                     //+ " Select CONVERT(varchar,Date,103)_Date,VoucherNo,NetAmt,Status,BillType,Date from (Select Date,(BillCode+' '+CAST(BIllNo as varchar))VoucherNo,NetAmt,'Dr' as Status,'SALESERVICE' as BillType from SaleServiceBook SSB Cross APPLY (Select Top 1 ItemName from SaleServiceDetails SSD Where SSB.BillCode=SSD.BillCode and SSB.BillNo=SSD.BillNo) SSD Where ItemName Like('%INT%') and SalePartyID='" + strPartyID + "' UNION ALL "
                     //+ " Select Date,(VoucherCode+' '+CAST(VoucherNo as varchar))VoucherNo, Amount,'Cr' as Status,'JOURNAL' as BillType from BalanceAmount Where GSTNature = 'DISCOUNT'  and Description Like('%DIS%')  and AccountID = '" + strPartyID + "' )Balance Order by Date desc "
                     + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('INTEREST','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName.Replace("'", "") + "/" + Environment.UserName.Replace("'", "")).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                SetNewAndOldScheme(dt);
                SetCostCenterDetails(ds.Tables[1]);
               // SetIntDiscountValue(ds.Tables[2]);
            }
        }

        //private void SetIntDiscountValue(DataTable dt)
        //{
        //    try
        //    {
        //        if (dt.Rows.Count > 0)
        //        {
        //            lnkShowIntDetails.Visible = true;
        //            DataView _dv = dt.DefaultView;
        //            _dv.Sort = "Date Desc";
        //            _dtIntDiscDetails = _dv.ToTable();
        //        }
        //    }
        //    catch { }
        //}

        private string CreateQuery(ref string strPartyID, bool _bStatus)
        {
            string strQuery = "";
            try
            {
                //if (txtParty.Text != "")
                //{
                //    string[] strFullName = txtParty.Text.Split(' ');
                //    if (strFullName.Length > 0)
                //    {
                //        strPartyID = strFullName[0].Trim();
                //        strQuery += " and AccountID='" + strPartyID + "' ";
                //    }
                //}

                //if (txtAccountID.Text != "" && !MainPage._bTaxStatus)
                //{
                //    string[] strFullName = txtAccountID.Text.Split(' ');
                //    if (strFullName.Length > 0)
                //    {
                //        strQuery += " and AccountID='" + strFullName[0].Trim() + "' ";
                //    }
                //}
                //else 
                if (txtParty.Text != "")
                {
                    string[] strFullName = txtParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPartyID = strFullName[0].Trim();
                        if (!MainPage._bTaxStatus && _bStatus)
                        {
                            strQuery += " and (AccountID in (Select (AreaCode+AccountNo) from SupplierMaster Where GroupName!='SUB PARTY' and Other Like('" + strPartyID + " %') UNION ALL Select '" + strPartyID + "' as _AccountID)) ";
                        }
                        else
                            strQuery += " and AccountID='" + strPartyID + "' ";
                    }
                }


                if (chkDate.Checked)
                {
                    DateTime endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                    strQuery += " and Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
                }

                string strStatus = GetStatus();
                if (strStatus != "")
                    strQuery += " and Tick='" + strStatus + "' ";

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Interest Statement Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private string GetStatus()
        {
            string strStatus = "";
            if (rdoTick.Checked)
                strStatus = "True";
            else if (rdoUnTick.Checked)
                strStatus = "False";

            return strStatus;
        }

        private void SetRecordWithDataTable(DataTable dt)
        {
            DataTable _datatable = CreateDataTable();
            string strAdjustedNo = "", strAdjuster = "";
            _bLockdownStatus = false;
            if (dt != null)
            {

                double dDebitAmt = 0, dCreditAmt = 0, dAmt = 0, dTotalAmt = 0, dIntAmt = 0, dDebitIntAmt = 0, dCreditIntAmt = 0, dDRate = 0, dCRate = 0, dDueDays = 0, dFinalAmt = 0, dNetWSRAmt = 0, dWSRAmt = 0;
                dNetSaleAmt = dNetPurchaseAmt = 0;
                int dExtraDueDays = 0, rowLength = 0, colorIndex = 0, daysInYear = 360;
                DateTime lDate = dba.ConvertDateInExactFormat(txtLastDate.Text).AddDays(1),_date= dba.ConvertDateInExactFormat("23/03/2020");
                daysInYear = Convert.ToInt32(txtDaysInYr.Text);
                dDRate = Convert.ToDouble(txtRateDr.Text);
                dCRate = Convert.ToDouble(txtRateCr.Text);
                dDueDays = Convert.ToDouble(txtGraceDays.Text);
                dExtraDueDays= dba.ConvertObjectToInt(txtExtraDays.Text);

                if (chkDate.Checked)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    DataRow[] rows = dt.Select("Date<'" + sDate + "' ");
                    bool tickStatus = false;

                    rowLength = rows.Length;
                    if (rowLength > 0)
                    {
                        foreach (DataRow row in rows)
                        {
                            if (Convert.ToString(row["DebitAmt"]) != "")
                                dDebitAmt += Convert.ToDouble(row["DebitAmt"]);
                            else if (Convert.ToString(row["CreditAmt"]) != "")
                                dCreditAmt += Convert.ToDouble(row["CreditAmt"]);
                            if (!tickStatus)
                            {
                                tickStatus = Convert.ToBoolean(row["Tick"]);
                                strAdjustedNo = Convert.ToString(row["AdjustedNumber"]);
                                strAdjuster = Convert.ToString(row["UserName"]);
                            }
                        }
                        dTotalAmt = dDebitAmt - dCreditAmt;

                        dDebitAmt = dCreditAmt = 0;
                        if (dTotalAmt > 0)
                            dDebitAmt = dTotalAmt;
                        else
                            dCreditAmt = Math.Abs(dTotalAmt);

                        if (dTotalAmt != 0)
                        {
                            DataRow dRow = _datatable.NewRow();
                            dRow["Date"] = sDate.ToString("dd/MM/yyyy");
                            dRow["AccountStatus"] = "OPENING";
                            dRow["Tick"] = tickStatus;
                            dRow["AdjustedNo"] = strAdjustedNo;
                            dRow["AdjusterName"] = strAdjuster;
                            dRow["Onaccount"] = 0;
                            dRow["Final"] = "0.00";
                            if (tickStatus)
                                dRow["ColorIndex"] = 0;

                            TimeSpan span = lDate.Subtract(sDate);
                            int days = span.Days;
                            if (_date > sDate)
                            {
                                days -= dExtraDueDays;
                                _bLockdownStatus = true;
                            }

                            dRow["IDays"] = days;
                            if (dTotalAmt > 0)
                            {
                                dRow["DebitAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                                dRow["BalanceAmt"] = dRow["DebitAmt"] + " Dr";
                                dIntAmt = (((days * dTotalAmt) * dDRate) / (daysInYear * 100));
                                if (dIntAmt > 0)
                                {
                                    dRow["IntDr"] = dIntAmt.ToString("N2", MainPage.indianCurancy);
                                    dDebitIntAmt = dIntAmt;
                                }
                                else
                                {
                                    dCreditIntAmt = Math.Abs(dIntAmt);
                                    dRow["IntCr"] = dCreditIntAmt.ToString("N2", MainPage.indianCurancy);
                                }
                                dDebitAmt = dTotalAmt;

                            }
                            else if (dTotalAmt < 0)
                            {
                                dCreditAmt = Math.Abs(dTotalAmt);
                                dRow["CreditAmt"] = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                                dRow["BalanceAmt"] = dRow["CreditAmt"] + " Cr";
                                dIntAmt = (((days * dTotalAmt) * dCRate) / (daysInYear * 100));
                                if (dIntAmt > 0)
                                {
                                    dRow["IntDr"] = dIntAmt.ToString("N2", MainPage.indianCurancy);
                                    dDebitIntAmt = dIntAmt;
                                }
                                else
                                {
                                    dCreditIntAmt = Math.Abs(dIntAmt);
                                    dRow["IntCr"] = dCreditIntAmt.ToString("N2", MainPage.indianCurancy);
                                }
                            }
                            _datatable.Rows.Add(dRow);
                        }
                    }
                }
                DateTime entryDate = DateTime.Now,eDate=DateTime.Now;
                string strAccountStatus = "";
                double _dGDays = 0;
                for (; rowLength < dt.Rows.Count; rowLength++)
                {
                    DataRow row = dt.Rows[rowLength];
                    DataRow dRow = _datatable.NewRow();

                    dRow["Date"] = row["BDate"];
                    dRow["AccountStatus"] = strAccountStatus = Convert.ToString(row["AccountStatus"]);
                    dRow["Description"] = row["Description"];
                    dRow["CreatedBy"] = row["CreatedBy"];
                    dRow["UpdatedBy"] = row["UpdatedBy"];
                    dRow["Onaccount"] = row["GDays"];
                    dRow["Final"] = "0.00";

                    if (strCategoryName == "")
                        strCategoryName = Convert.ToString(row["CategoryName"]);

                    entryDate = eDate= dba.ConvertDateInExactFormat(Convert.ToString(row["BDate"]));
                    if (strAccountStatus == "SALES A/C" || strAccountStatus == "PURCHASE A/C")
                    {
                        dFinalAmt = dba.ConvertObjectToDouble(row["FinalAmount"]);
                        _dGDays = dba.ConvertObjectToDouble(row["GDays"]);
                        dRow["Final"] = dFinalAmt.ToString("N2", MainPage.indianCurancy);
                        if (dDueDays != 0 || _dGDays != 0)
                            entryDate = entryDate.AddDays(dDueDays + _dGDays);
                    }
                    else if (strAccountStatus == "SALE RETURN")
                    {
                        dFinalAmt = dba.ConvertObjectToDouble(row["FinalAmount"]);
                        dRow["Final"] = dFinalAmt.ToString("N2", MainPage.indianCurancy);
                    }

                    TimeSpan span = lDate.Subtract(entryDate);
                    int days = span.Days;
                    if (_date > eDate)
                    {
                        _bLockdownStatus = true;
                        days -= dExtraDueDays;
                    }

                    dRow["IDays"] = days;

                    if (Convert.ToBoolean(row["Tick"]))
                    {
                        dRow["Tick"] = true;
                        dRow["AdjusterName"] = row["UserName"];
                        strAdjustedNo = Convert.ToString(row["AdjustedNumber"]);
                        if (strAdjustedNo == "" || strAdjustedNo == "0")
                            strAdjustedNo = Convert.ToString(row["MultiCompanyNo"]);
                        dRow["AdjustedNo"] = strAdjustedNo;
                        if (strAdjustedNo != "")
                        {
                            DataRow[] dCRow = _datatable.Select("AdjustedNo='" + strAdjustedNo + "' ");
                            if (dCRow.Length > 0)
                                dRow["ColorIndex"] = dCRow[0]["ColorIndex"];
                            else
                            {
                                dRow["ColorIndex"] = colorIndex;
                                if (colorIndex == 9)
                                    colorIndex = -1;
                                colorIndex++;
                            }
                        }
                        else
                            dRow["ColorIndex"] = colorIndex;
                    }

                    if (Convert.ToString(row["DebitAmt"]) != "")
                    {
                        dDebitAmt += dAmt = Convert.ToDouble(row["DebitAmt"]);
                        dTotalAmt += dAmt;
                        dRow["DebitAmt"] = dAmt.ToString("N2", MainPage.indianCurancy);
                        if (strAccountStatus == "SALES A/C")
                            dNetSaleAmt += dAmt;
                        else if (strAccountStatus == "PURCHASE A/C")
                            dNetPurchaseAmt += dAmt;
                        if (dTotalAmt > 0)
                            dRow["BalanceAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dTotalAmt < 0)
                            dRow["BalanceAmt"] = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            dRow["BalanceAmt"] = "0.00";
                        if (dAmt != 0)
                        {
                            dIntAmt = (((days * dAmt) * dDRate) / (daysInYear * 100));
                            if (dIntAmt >= 0)
                            {
                                dRow["IntDr"] = dIntAmt.ToString("N2", MainPage.indianCurancy);
                                dDebitIntAmt += dIntAmt;
                            }
                            else
                            {
                                dRow["IntCr"] = Math.Abs(dIntAmt).ToString("N2", MainPage.indianCurancy);
                                dCreditIntAmt += Math.Abs(dIntAmt);
                            }
                        }
                    }
                    else if (Convert.ToString(row["CreditAmt"]) != "")
                    {
                        dCreditAmt += dAmt = Convert.ToDouble(row["CreditAmt"]);
                        dRow["CreditAmt"] = dAmt.ToString("N2", MainPage.indianCurancy);
                        if (strAccountStatus == "SALES A/C")
                            dNetSaleAmt += dAmt;
                        else if (strAccountStatus == "PURCHASE A/C")
                            dNetPurchaseAmt += dAmt;
                        dTotalAmt -= dAmt;

                        if (dTotalAmt > 0)
                            dRow["BalanceAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dTotalAmt < 0)
                            dRow["BalanceAmt"] = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            dRow["BalanceAmt"] = "0.00";


                        if (dAmt != 0)
                        {
                            dIntAmt = (((days * dAmt) * dDRate) / (daysInYear * 100)) * -1;
                            if (dIntAmt >= 0)
                            {
                                dRow["IntDr"] = dIntAmt.ToString("N2", MainPage.indianCurancy);
                                dDebitIntAmt += dIntAmt;
                            }
                            else
                            {
                                dRow["IntCr"] = Math.Abs(dIntAmt).ToString("N2", MainPage.indianCurancy);
                                dCreditIntAmt += Math.Abs(dIntAmt);
                            }
                        }
                    }
                    _datatable.Rows.Add(dRow);
                }

                BindDataWithGrid(_datatable);

                lblDrAmt.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCrAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                lblIntCr.Text = dCreditIntAmt.ToString("N2", MainPage.indianCurancy);
                lblIntDr.Text = dDebitIntAmt.ToString("N2", MainPage.indianCurancy);

                if (dTotalAmt > 0)
                    lblBalance.Text = lblGrossAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalance.Text = lblGrossAmt.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalance.Text = lblGrossAmt.Text = "0";
                dIntAmt = dDebitIntAmt - dCreditIntAmt;
                if (dIntAmt > 0)
                    lblInterest.Text = dIntAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dIntAmt < 0)
                    lblInterest.Text = Math.Abs(dIntAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblInterest.Text = "0";

                if (dNetSaleAmt == 0 && dNetPurchaseAmt > 0)
                {
                    dNetSaleAmt = dNetPurchaseAmt;
                    dIntAmt = dIntAmt * -1;
                }

                CalculateAvgDays(dNetSaleAmt, dDebitAmt, dIntAmt);
            }

            picLockdown.Visible = _bLockdownStatus;
        }

        private bool CalculateAvgDays(double dSaleAmt, double dDebitAmt, double dIntAmt)
        {
            bool _cdStatus = false;
            try
            {
                double dAmt = 0, avgDays = 0, dCDDays = 0, dGraceDays = 0, dDrRate = 0;
                dDrRate = dba.ConvertObjectToDouble(txtRateDr.Text);
                dAmt = (dSaleAmt * dDrRate) / 36000;
                //dCDDays = Convert.ToDouble(txtCDDays.Text);
                dGraceDays = Convert.ToDouble(txtGraceDays.Text);

                // if (dIntAmt>=0)
                avgDays = Math.Round((dIntAmt / dAmt), 2);
                //else
                //    avgDays = (dIntAmt / dAmt)*-1;

                if (dSaleAmt < 1)
                    lblAvgDays.Text = "0";
                else
                    lblAvgDays.Text = avgDays.ToString("N2", MainPage.indianCurancy);

                // Calculate Cash Discount
                // bool bCDStatus = false;
                if (dSaleAmt > 0)
                    _cdStatus = CheckCDStatus(dGraceDays, avgDays);

                //if ((dCDDays - dGraceDays) >= avgDays && dSaleAmt > 0)                
                //    CalculateWSRAndCD(true);               

                CalculateWSRAndCD(_cdStatus);

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Calculation of Average Day in General Interest ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return _cdStatus;
        }

        private bool CheckCDStatus(double dGraceDays, double avgDays)
        {
            bool bStatus = false;
            strDiscountName = "";
            if (dtDiscountDetails != null)
            {
                txtCD.Text = "1";
                string strCategoryID = "2", strDiscountStatus = "0";
                if (strCategoryName == "WHOLESALER")
                    strCategoryID = "1";

                if (oldBillStatus)
                    strDiscountStatus = "1";

                DataRow[] rows = dtDiscountDetails.Select("CategoryID=" + strCategoryID + " and DiscountStatus=" + strDiscountStatus);
                if (rows.Length > 0)
                {
                    DataTable dtCash = rows.CopyToDataTable();
                    DataView dv = dtCash.DefaultView;
                    dv.Sort = "CDDays asc";
                    dtCash = dv.ToTable();
                    double dCDDays = 0;

                    foreach (DataRow row in dtCash.Rows)
                    {
                        dCDDays = dba.ConvertObjectToDouble(row["CDDays"]);
                        if ((dCDDays - dGraceDays) >= avgDays)
                        {
                            txtCD.Text = Convert.ToString(row["DiscountPer"]);
                            strDiscountName = Convert.ToString(row["DiscountName"]);
                            bStatus = true;
                            break;
                        }
                    }
                }
                else
                    txtCD.Text = "0";
            }
            return bStatus;
        }

        //private bool CheckCDStatus(double dGraceDays, double avgDays)
        //{
        //    bool bStatus = false;
        //    strDiscountName = "";
        //    if (dtDiscountDetails != null)
        //    {
        //        string strCategoryID = "2";
        //        if (strCategoryName == "WHOLESALER")
        //            strCategoryID = "1";
        //        DataRow[] rows = dtDiscountDetails.Select("CategoryID=" + strCategoryID);
        //        if (rows.Length > 0)
        //        {
        //            DataTable dtCash = rows.CopyToDataTable();
        //            DataView dv = dtCash.DefaultView;
        //            dv.Sort = "CDDays asc";
        //            dtCash = dv.ToTable();
        //            double dCDDays = 0;
        //            foreach (DataRow row in dtCash.Rows)
        //            {
        //                dCDDays = dba.ConvertObjectToDouble(row["CDDays"]);
        //                if ((dCDDays-dGraceDays) >= avgDays)
        //                {
        //                    txtCD.Text = Convert.ToString(row["DiscountPer"]);
        //                    strDiscountName = Convert.ToString(row["DiscountName"]);
        //                    bStatus = true;
        //                    break;
        //                }
        //            }
        //        }
        //        //if (strCategoryName == "WHOLESALER")
        //        //{
        //        //    if ((15 - dGraceDays) >= avgDays)
        //        //    {
        //        //        txtCD.Text = "2";
        //        //        bStatus = true;
        //        //    }
        //        //    else if ((30 - dGraceDays) >= avgDays)
        //        //    {
        //        //        txtCD.Text = "1.5";
        //        //        bStatus = true;
        //        //    }
        //        //    else if ((45 - dGraceDays) >= avgDays)
        //        //    {
        //        //        txtCD.Text = "1";
        //        //        bStatus = true;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    if ((15 - dGraceDays) >= avgDays)
        //        //    {
        //        //        txtCD.Text = "2";
        //        //        bStatus = true;
        //        //    }
        //        //    else if ((30 - dGraceDays) >= avgDays)
        //        //    {
        //        //        txtCD.Text = "1";
        //        //        bStatus = true;
        //        //    }
        //        //}
        //    }
        //    return bStatus;
        //}

        private void CalculateWSRAndCD(bool cdStatus)
        {
            try
            {
                double dFinalAmt = 0, dWSRRate = 0, dCDRate = 0, dNetCDAmt = 0, dNetWSR = 0, dAmt = 0;

                if (cdStatus)
                {
                    dgrdInterest.Columns["cd"].Visible = true;
                    dgrdInterest.Columns["iDays"].Visible = dgrdInterest.Columns["intDr"].Visible = dgrdInterest.Columns["intcr"].Visible = false;
                }
                else
                {
                    dgrdInterest.Columns["cd"].Visible = false;
                    dgrdInterest.Columns["iDays"].Visible = dgrdInterest.Columns["intDr"].Visible = dgrdInterest.Columns["intcr"].Visible = true;
                }


                double dDebitAmt = 0, dCreditAmt = 0, dIntDAmt = 0, dIntCAmt = 0, dTaxAmt = 0, _dNetIntAmt = 0;
                dDebitAmt = dba.ConvertObjectToDouble(lblDrAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(lblCrAmt.Text);
                dIntDAmt = dba.ConvertObjectToDouble(lblIntDr.Text);
                dIntCAmt = dba.ConvertObjectToDouble(lblIntCr.Text);
                if (strCategoryName == "WHOLESALER" || cdStatus)
                {
                    lblFinalBal.Text = "00";
                    dWSRRate = dba.ConvertObjectToDouble(txtWSR.Text);
                    dCDRate = dba.ConvertObjectToDouble(txtCD.Text);

                    string strAccount = "";
                    foreach (DataGridViewRow row in dgrdInterest.Rows)
                    {
                        strAccount = Convert.ToString(row.Cells["particulars"].Value);
                        if (strAccount == "SALES A/C" || strAccount == "PURCHASE A/C")
                        {
                            dFinalAmt = dba.ConvertObjectToDouble(row.Cells["final"].Value);
                            if (dFinalAmt != 0)
                            {
                                if (strCategoryName == "WHOLESALER")
                                {
                                    dAmt = (dFinalAmt * dWSRRate) / 100;
                                    dNetWSR += dAmt;
                                    row.Cells["wsd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }
                                if (cdStatus)
                                {
                                    dAmt = Math.Round(((dFinalAmt * dCDRate) / 100), 2);

                                    dNetCDAmt += dAmt;
                                    row.Cells["cd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }
                            }
                        }
                        else if (strAccount == "SALE RETURN")
                        {
                            if (cdStatus)
                            {
                                dFinalAmt = dba.ConvertObjectToDouble(row.Cells["final"].Value);
                                if (dFinalAmt == 0)
                                    dFinalAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);
                                dAmt = Math.Round(((dFinalAmt * dCDRate) / 100), 2) * -1;
                                dNetCDAmt += dAmt;
                                row.Cells["cd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            }
                        }
                    }
                }

                if (strCategoryName == "WHOLESALER" && dWSRRate != 0)
                    dgrdInterest.Columns["wsd"].Visible = true;
                else
                    dgrdInterest.Columns["wsd"].Visible = false;

                if (cdStatus)
                {
                    dCreditAmt += dNetWSR + dNetCDAmt;
                    lblIntCr.Text = lblIntDr.Text = lblInterest.Text = "0.00";

                }
                else
                {
                    _dNetIntAmt = dIntDAmt - dIntCAmt;
                    if (_dNetIntAmt > 0)
                    {
                        if (chkTax.Checked)
                            dTaxAmt = Math.Round((_dNetIntAmt * 18) / 100, 2);
                        else
                            dTaxAmt = (_dNetIntAmt - Math.Round((_dNetIntAmt / 118) * 100, 2));

                        lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy) + " Dr";

                        if (!chkTax.Checked)
                            dTaxAmt = 0;
                    }

                    dNetCDAmt = 0;
                    dDebitAmt += dIntDAmt + dTaxAmt;
                    dCreditAmt += dNetWSR + dIntCAmt;
                }

                dAmt = dDebitAmt - dCreditAmt;

                lblWSR.Text = dNetWSR.ToString("N2", MainPage.indianCurancy);
                if (dNetCDAmt >= 0)
                    lblCDiscount.Text = dNetCDAmt.ToString("N2", MainPage.indianCurancy) + " Cr";
                else if (dNetCDAmt < 0)
                    lblCDiscount.Text = Math.Abs(dNetCDAmt).ToString("N2", MainPage.indianCurancy) + " Dr";

                if (dAmt >= 0)
                    lblFinalBal.Text = dAmt.ToString("N0", MainPage.indianCurancy) + " Dr";
                else if (dAmt < 0)
                    lblFinalBal.Text = Math.Abs(dAmt).ToString("N0", MainPage.indianCurancy) + " Cr";
            }
            catch
            {
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable _datatable = new DataTable();
            _datatable.Columns.Add("BalanceID", typeof(String));
            _datatable.Columns.Add("Date", typeof(String));
            _datatable.Columns.Add("AccountStatus", typeof(String));
            _datatable.Columns.Add("Description", typeof(String));
            _datatable.Columns.Add("DebitAmt", typeof(String));
            _datatable.Columns.Add("CreditAmt", typeof(String));
            _datatable.Columns.Add("IDays", typeof(String));
            _datatable.Columns.Add("IntDr", typeof(String));
            _datatable.Columns.Add("IntCr", typeof(String));
            _datatable.Columns.Add("Final", typeof(String));
            _datatable.Columns.Add("BalanceAmt", typeof(String));
            _datatable.Columns.Add("Tick", typeof(Boolean));
            _datatable.Columns.Add("AdjusterName", typeof(String));
            _datatable.Columns.Add("AdjustedNo", typeof(String));
            _datatable.Columns.Add("CreatedBy", typeof(String));
            _datatable.Columns.Add("UpdatedBy", typeof(String));
            _datatable.Columns.Add("ColorIndex", typeof(String));
            _datatable.Columns.Add("Onaccount", typeof(String));

            return _datatable;
        }

        private void BindDataWithGrid(DataTable table)
        {
            int rowIndex = 0, colorIndex = 0;
            string strCIndex = "";
            if (table.Rows.Count > 0)
                dgrdInterest.Rows.Add(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                dgrdInterest.Rows[rowIndex].Cells["chk"].Value = false;
                dgrdInterest.Rows[rowIndex].Cells["bDate"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["Date"]));
                dgrdInterest.Rows[rowIndex].Cells["particulars"].Value = row["AccountStatus"];
                dgrdInterest.Rows[rowIndex].Cells["desc"].Value = row["Description"];
                dgrdInterest.Rows[rowIndex].Cells["amountDr"].Value = row["DebitAmt"];
                dgrdInterest.Rows[rowIndex].Cells["amountCr"].Value = row["CreditAmt"];
                dgrdInterest.Rows[rowIndex].Cells["bal"].Value = row["BalanceAmt"];
                dgrdInterest.Rows[rowIndex].Cells["iDays"].Value = row["IDays"];
                dgrdInterest.Rows[rowIndex].Cells["intDr"].Value = row["IntDr"];
                dgrdInterest.Rows[rowIndex].Cells["intcr"].Value = row["IntCr"];
                dgrdInterest.Rows[rowIndex].Cells["final"].Value = row["Final"];
                dgrdInterest.Rows[rowIndex].Cells["tick"].Value = row["Tick"];
                dgrdInterest.Rows[rowIndex].Cells["adjustedNo"].Value = row["AdjustedNo"];
                dgrdInterest.Rows[rowIndex].Cells["adjuster"].Value = row["AdjusterName"];
                dgrdInterest.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                dgrdInterest.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                dgrdInterest.Rows[rowIndex].Cells["wsd"].Value = "0";
                dgrdInterest.Rows[rowIndex].Cells["cd"].Value = "0";
                dgrdInterest.Rows[rowIndex].Cells["onaccountStatus"].Value = row["Onaccount"];
                strCIndex = Convert.ToString(row["ColorIndex"]);
                if (strCIndex != "")
                {
                    colorIndex = Convert.ToInt32(strCIndex);
                    dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromName(strColor[colorIndex]);
                }

                if (Convert.ToString(row["Onaccount"]) == "1")
                {
                    dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                rowIndex++;
            }
        }

        #region Hide Entry ...

        public void CalculateTotalAmount()
        {
            try
            {
                double dDAmt = 0, dCAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dIntDAmt = 0, dIntCAmt = 0, dTotalAmt = 0, dIntAmt = 0;
                dNetSaleAmt = dNetPurchaseAmt = 0;
                lblTaxAmt.Text = "0.00 Dr";
                DateTime _eDate = DateTime.Now, _date = dba.ConvertDateInExactFormat("23/03/2020");

                foreach (DataGridViewRow row in dgrdInterest.Rows)
                {
                    if (!_bLockdownStatus)
                    {
                        _eDate = Convert.ToDateTime(row.Cells["bDate"].Value);
                        if (_date > _eDate)
                            _bLockdownStatus = true;
                    }

                    if (row.DefaultCellStyle.BackColor.Name != "Gold")
                    {
                        dDAmt = dba.ConvertObjectToDouble(row.Cells["amountDr"].Value);
                        if (dDAmt != 0)
                        {
                            dDebitAmt += dDAmt;
                            dTotalAmt += dDAmt;
                            if (Convert.ToString(row.Cells["particulars"].Value).ToUpper() == "SALES A/C")
                                dNetSaleAmt += dDAmt;
                        }
                        else
                        {
                            dCAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);
                            dCreditAmt += dCAmt;
                            dTotalAmt -= dCAmt;
                            //if (Convert.ToString(row.Cells["particulars"].Value).ToUpper() == "PURCHASE A/C")
                            //    dNetPurchaseAmt += dCAmt;
                        }

                        dIntDAmt += dIntAmt = dba.ConvertObjectToDouble(row.Cells["intDr"].Value);
                        if (dIntAmt == 0)
                            dIntCAmt += dba.ConvertObjectToDouble(row.Cells["intCr"].Value);
                        row.Cells["wsd"].Value = 0;
                        row.Cells["cd"].Value = 0;
                        if (dTotalAmt > 0)
                            row.Cells["bal"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dTotalAmt < 0)
                            row.Cells["bal"].Value = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            row.Cells["bal"].Value = "0.00";
                    }
                }

                lblDrAmt.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCrAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                lblIntCr.Text = dIntCAmt.ToString("N2", MainPage.indianCurancy);
                lblIntDr.Text = dIntDAmt.ToString("N2", MainPage.indianCurancy);

                if (dTotalAmt > 0)
                    lblBalance.Text = lblGrossAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalance.Text = lblGrossAmt.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalance.Text = lblGrossAmt.Text = "0";
                dIntAmt = dIntDAmt - dIntCAmt;
                if (dIntAmt > 0)
                    lblInterest.Text = dIntAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dIntAmt < 0)
                    lblInterest.Text = Math.Abs(dIntAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblInterest.Text = "0";

                CalculateAvgDays(dNetSaleAmt, dDebitAmt, dIntAmt);

                if (strDiscountName != "")
                    lblDiscountName.Text = strDiscountName;
                else
                    lblDiscountName.Text = "Disc. Type";

                picLockdown.Visible = _bLockdownStatus;
            }
            catch { }
        }

        #endregion

        private void dgrdInterest_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdInterest_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 0)
                    {
                        if (dgrdInterest.CurrentRow.DefaultCellStyle.BackColor.Name != "Gold")
                        {
                            if (Convert.ToBoolean(dgrdInterest.CurrentCell.EditedFormattedValue))
                                dgrdInterest.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                            else
                                dgrdInterest.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                    else if (e.ColumnIndex == 2)
                        ShowDetails();
                }
            }
            catch
            {
            }
        }

        private void ShowDetails()
        {
            DateTime ledgerDate = Convert.ToDateTime(dgrdInterest.CurrentRow.Cells["bDate"].Value);// dba.ConvertDateInExactFormat(Convert.ToString(dgrdLedger.CurrentRow.Cells["date"].Value));
            if (ledgerDate >= MainPage.startFinDate && ledgerDate < MainPage.endFinDate)
            {
                string strAccount = Convert.ToString(dgrdInterest.CurrentRow.Cells["particulars"].Value).ToUpper();
                if (strAccount == "PURCHASE A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        if (dba.GetPurchaseRecordType(strNumber[0], strNumber[1]))
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                PurchaseBook objPurchase = new PurchaseBook(strNumber[0], strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                            else
                            {
                                string strCode = strNumber[0].Replace("PB", "GB");
                                GoodscumPurchase objPurchase = new GoodscumPurchase(strCode, strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                        }
                        else
                        {
                            PurchaseBook_Trading objPurchase = new PurchaseBook_Trading(strNumber[0], strNumber[1]);
                            objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objPurchase.ShowInTaskbar = true;
                            objPurchase.Show();
                        }
                    }
                }
                else if (strAccount == "SALES A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        string str = dba.GetSalesRecordType(strNumber[0], strNumber[1]);
                        if (str == "")
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                dba.ShowSaleBookPrint(strNumber[0], strNumber[1], false, false);
                            }
                            else
                            {
                                SaleBook objSale = new SaleBook(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }
                        else
                        {
                            if (str == "RETAIL")
                            {
                                SaleBook_Retail objSale = new SaleBook_Retail(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                            else
                            {
                                SaleBook_Trading objSale = new SaleBook_Trading(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }

                    }
                }
                else if (strAccount == "SALE RETURN")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        if (strNumber[0].Contains("PTN"))
                        {
                            SaleReturn_Trading objSale = new SaleReturn_Trading(strNumber[0], strNumber[1]);
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();
                        }
                        else
                        {
                            SaleReturn objSale = new SaleReturn(strNumber[0], strNumber[1]);
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();
                        }
                    }
                }
                else if (strAccount == "PURCHASE RETURN")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        if (strNumber[0].Contains("PTN"))
                        {
                            PurchaseReturn_Trading objSale = new PurchaseReturn_Trading(strNumber[0], strNumber[1]);
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();
                        }
                        else
                        {
                            PurchaseReturn objSale = new PurchaseReturn(strNumber[0], strNumber[1]);
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();
                        }
                    }
                }
                else if (strAccount == "SALE SERVICE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        SaleServiceBook objSale = new SaleServiceBook(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "CREDIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        CreditNote_Supplier objSale = new CreditNote_Supplier(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "DEBIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        DebitNote_Customer objDebitNote = new DebitNote_Customer(strNumber[0], strNumber[1]);
                        objDebitNote.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objDebitNote.ShowInTaskbar = true;
                        objDebitNote.Show();
                    }
                }
                else if (strAccount == "TCS DEBIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        TCSDetails objTCSDetails = new TCSDetails("DEBITNOTE", strNumber[0], strNumber[1]);
                        objTCSDetails.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTCSDetails.ShowInTaskbar = true;
                        objTCSDetails.Show();
                    }
                }
                else if (strAccount == "TCS CREDIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        TCSDetails objTCSDetails = new TCSDetails("CREDITNOTE", strNumber[0], strNumber[1]);
                        objTCSDetails.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTCSDetails.ShowInTaskbar = true;
                        objTCSDetails.Show();
                    }
                }
                else
                {
                    string[] strName = strAccount.Split('|');
                    if (strName.Length > 1)
                    {
                        string[] strVoucher = strName[1].Trim().Split(' ');
                        if (strVoucher.Length > 0)
                        {
                            if (strName[0].Trim() == "JOURNAL A/C")
                            {
                                JournalEntry_New objJournalEntry = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                                objJournalEntry.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                objJournalEntry.ShowInTaskbar = true;
                                objJournalEntry.Show();
                            }
                            else
                            {
                                object objCode = DataBaseAccess.ExecuteMyScalar("Select CashVCode from CompanySetting Where CashVCode='" + strVoucher[0] + "'");
                                if (Convert.ToString(objCode) != "")
                                {
                                    CashBook objCashBook = new CashBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                                    objCashBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                    objCashBook.ShowInTaskbar = true;
                                    objCashBook.Show();
                                }
                                else
                                {
                                    BankBook objBankBook = new BankBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                                    objBankBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                    objBankBook.ShowInTaskbar = true;
                                    objBankBook.Show();
                                }
                            }
                        }
                    }
                    else
                    {
                        //string strJournal = Convert.ToString(dgrdInterest.CurrentRow.Cells["journalID"].Value);
                        //string[] strVoucher = strJournal.Split(' ');
                        //if (strVoucher.Length > 0)
                        //{
                        //    JournalEntry_New objJournal = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                        //    objJournal.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        //    objJournal.ShowInTaskbar = true;
                        //    objJournal.Show();
                        //}
                    }
                }
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                //this.TopMost = false;
                //  objHide.TopMost = true;
                objHide.Visible = true;
                objHide.Show();

                //if (strDiscountName != "")
                //    lblDiscountName.Text = strDiscountName;
                //else
                //    lblDiscountName.Text = "Disc. Type";
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Click event of Show Button in General Interest ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            try
            {
                btnHide.Enabled = false;
                if (objHide == null)
                    objHide = new HideRecords();
                HideData();
                CalculateTotalAmount();
                //if (strDiscountName != "")
                //    lblDiscountName.Text = strDiscountName;
                //else
                //    lblDiscountName.Text = "Disc. Type";
            }
            catch
            {
            }
            btnHide.Enabled = true;
        }

        private void HideData()
        {
            try
            {
                int rowIndex = objHide.dgrdInterest.Rows.Count;
                for (int rIndex = 0; rIndex < dgrdInterest.Rows.Count; rIndex++)
                {
                    DataGridViewRow row = dgrdInterest.Rows[rIndex];
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        objHide.dgrdInterest.Rows.Add();
                        for (int colIndex = 0; colIndex < dgrdInterest.ColumnCount; colIndex++)
                        {
                            objHide.dgrdInterest.Rows[rowIndex].Cells[colIndex].Value = row.Cells[colIndex].Value;
                        }
                        if (Convert.ToString(row.Cells["onaccountStatus"].Value) == "COST")
                            objHide.dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;

                        rowIndex++;
                        dgrdInterest.Rows.RemoveAt(rIndex);
                        rIndex--;
                    }
                }
                objHide.chkTax.Checked = true;
                objHide.CalculateTotalAmount();
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            picPleasewait.Visible = true;
            try
            {
                if (txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    string strPath = "";
                    PrintPreviewExport(0, ref strPath);
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
            picPleasewait.Visible = false;
        }

        private void PrintPreviewExport(int _printStatus, ref string strPath)
        {
            try
            {
                bool wStatus = dgrdInterest.Columns["wsd"].Visible, cStatus = dgrdInterest.Columns["cd"].Visible;
                DataTable _dtAdvance = new DataTable();
                DataTable dt = CreatePrintDataTable(wStatus, cStatus, ref _dtAdvance);
                if (dt.Rows.Count > 0)
                {
                    if (!wStatus && !cStatus)
                    {
                        if (chkTax.Checked && dba.ConvertObjectToDouble(lblTaxAmt.Text.Replace(" Dr", "").Replace(" Cr", "")) > 0)
                        {
                            Reporting.InterestReport_WithAddress objReport = new SSS.Reporting.InterestReport_WithAddress();
                            objReport.SetDataSource(dt);
                            objReport.Subreports[0].SetDataSource(_dtAdvance);
                            FinallyPrint(_printStatus, objReport, strPath);
                            objReport.Close();
                            objReport.Dispose();
                            //if (_printStatus == 0)
                            //{
                            //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                            //    objShow.myPreview.ReportSource = objReport;
                            //    objShow.ShowDialog();
                            //}
                            //else if (_printStatus == 1)
                            //{
                            //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            //    objReport.PrintToPrinter(1, false, 0, 0);
                            //}
                            //else
                            //{
                            //    if (strPath != "")
                            //        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            //    else
                            //    {
                            //        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                            //        objViewer.ReportSource = objReport;
                            //        objViewer.ExportReport();
                            //    }
                            //}
                            //objReport.Close();
                            //objReport.Dispose();
                        }
                        else
                        {
                            Reporting.InterestReport_WithoutTax objReport = new SSS.Reporting.InterestReport_WithoutTax();
                            objReport.SetDataSource(dt);
                            objReport.Subreports[0].SetDataSource(_dtAdvance);
                            FinallyPrint(_printStatus, objReport, strPath);
                            objReport.Close();
                            objReport.Dispose();
                            //if (_printStatus == 0)
                            //{
                            //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                            //    objShow.myPreview.ReportSource = objReport;
                            //    objShow.ShowDialog();
                            //}
                            //else if (_printStatus == 1)
                            //{
                            //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            //    objReport.PrintToPrinter(1, false, 0, 0);
                            //}
                            //else
                            //{
                            //    if (strPath != "")
                            //        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            //    else
                            //    {
                            //        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                            //        objViewer.ReportSource = objReport;
                            //        objViewer.ExportReport();
                            //    }
                            //}
                            //objReport.Close();
                            //objReport.Dispose();
                        }

                    }
                    else if (wStatus && !cStatus)
                    {
                        Reporting.WSRInterestReport objReport = new SSS.Reporting.WSRInterestReport();
                        objReport.SetDataSource(dt);
                        FinallyPrint(_printStatus, objReport, strPath);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    objReport.PrintToPrinter(1, false, 0, 0);
                        //}
                        //else
                        //{
                        //    if (strPath != "")
                        //        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    else
                        //    {
                        //        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //        objViewer.ReportSource = objReport;
                        //        objViewer.ExportReport();
                        //    }
                        //}
                        //objReport.Close();
                        //objReport.Dispose();
                    }
                    else if (!wStatus && cStatus)
                    {
                        Reporting.CDInterestReport objReport = new SSS.Reporting.CDInterestReport();
                        objReport.SetDataSource(dt);
                        objReport.Subreports[0].SetDataSource(_dtAdvance);
                        FinallyPrint(_printStatus, objReport, strPath);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    objReport.PrintToPrinter(1, false, 0, 0);
                        //}
                        //else
                        //{
                        //    if (strPath != "")
                        //        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    else
                        //    {
                        //        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //        objViewer.ReportSource = objReport;
                        //        objViewer.ExportReport();
                        //    }
                        //}
                        // objReport.Close();
                        // objReport.Dispose();
                    }
                    else if (wStatus && cStatus)
                    {
                        Reporting.WSRCDReport objReport = new SSS.Reporting.WSRCDReport();
                        objReport.SetDataSource(dt);
                        FinallyPrint(_printStatus, objReport, strPath);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    objReport.PrintToPrinter(1, false, 0, 0);
                        //}
                        //else
                        //{
                        //    if (strPath != "")
                        //        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    else
                        //    {
                        //        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //        objViewer.ReportSource = objReport;
                        //        objViewer.ExportReport();
                        //    }
                        //}
                        //objReport.Close();
                        //objReport.Dispose();
                    }
                }
            }
            catch { }
        }
        private void FinallyPrint(int _printStatus, CrystalDecisions.CrystalReports.Engine.ReportClass objReport,string strPath)
        {
            if (_printStatus == 0)
            {
                Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();
            }
            else if (_printStatus == 1)
            {
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objReport);
                else
                {
                    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    objReport.PrintToPrinter(1, false, 0, 0);
                }
            }
            else
            {
                if (strPath != "")
                    objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                else
                {
                    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                    objViewer.ReportSource = objReport;
                    objViewer.ExportReport();
                }
            }
            objReport.Close();
            objReport.Dispose();
        }

        private DataTable CreatePrintDataTable(bool wStatus, bool cStatus, ref DataTable dtAdvance)
        {
            DataTable myDataTable = new DataTable();
            try
            {

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("GraceDays", typeof(String));
                myDataTable.Columns.Add("Rate", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Account", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("IDays", typeof(String));
                myDataTable.Columns.Add("Desc", typeof(String));
                myDataTable.Columns.Add("DebitInt", typeof(String));
                myDataTable.Columns.Add("CreditInt", typeof(String));
                myDataTable.Columns.Add("BalanceAmt", typeof(String));
                myDataTable.Columns.Add("FinalAmt", typeof(String));
                myDataTable.Columns.Add("WSR", typeof(String));
                myDataTable.Columns.Add("CD", typeof(String));
                myDataTable.Columns.Add("TotalDebitAmt", typeof(String));
                myDataTable.Columns.Add("TotalCreditAmt", typeof(String));
                myDataTable.Columns.Add("TotalDebitInt", typeof(String));
                myDataTable.Columns.Add("TotalCreditInt", typeof(String));
                myDataTable.Columns.Add("TotalBalanceAmt", typeof(String));
                myDataTable.Columns.Add("TotalWSR", typeof(String));
                myDataTable.Columns.Add("TotalCD", typeof(String));
                myDataTable.Columns.Add("TotalInt", typeof(String));
                myDataTable.Columns.Add("AvgDays", typeof(String));
                myDataTable.Columns.Add("BalanceWithInt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("OnAccount", typeof(String));
                myDataTable.Columns.Add("BalanceWithoutAmt", typeof(String));
                myDataTable.Columns.Add("DiscountName", typeof(String));
                myDataTable.Columns.Add("BalanceWithAmt", typeof(String));
                myDataTable.Columns.Add("TaxAmt", typeof(String));
                myDataTable.Columns.Add("BankName", typeof(String));
                myDataTable.Columns.Add("BranchName", typeof(String));
                myDataTable.Columns.Add("BankAccountNo", typeof(String));
                myDataTable.Columns.Add("IFSCCode", typeof(String));
                myDataTable.Columns.Add("Address", typeof(String));
                myDataTable.Columns.Add("PhoneNo", typeof(String));
                myDataTable.Columns.Add("Other", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));

                string strDate = "From " + txtFromDate.Text + " To " + txtLastDate.Text, strUserName = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                if (!chkDate.Checked)
                    strDate = "From " + MainPage.multiQSDate.ToString("dd/MM/yyyy") + " To " + txtLastDate.Text;
                if (rdoYes.Checked)
                    strDate += "  Grace Days : " + txtGraceDays.Text + "  Int. Rate : DR : " + txtRateDr.Text + "%  CR : " + txtRateCr.Text + "%";
                double dExtra = dba.ConvertObjectToDouble(txtExtraDays.Text);

                foreach (DataGridViewRow dr in dgrdInterest.Rows)
                {
                    if (Convert.ToString(dr.Cells["onaccountStatus"].Value) != "COST")
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = "INTEREST STATEMENT";
                        row["PartyName"] = "INTEREST STATEMENT OF M/S : " + txtParty.Text;
                        row["DatePeriod"] = strDate;
                        row["GraceDays"] = strDiscountName;

                        row["Date"] = Convert.ToDateTime(dr.Cells["bDate"].Value).ToString("dd/MM/yy");
                        row["Account"] = dr.Cells["particulars"].Value;
                        row["DebitAmt"] = dr.Cells["amountDr"].Value;
                        row["CreditAmt"] = dr.Cells["amountCr"].Value;
                        row["Desc"] = dr.Cells["desc"].Value;
                        row["BalanceAmt"] = dr.Cells["bal"].Value;
                        row["FinalAmt"] = dr.Cells["final"].Value;
                        row["WSR"] = dr.Cells["wsd"].Value;
                        row["OnAccount"] = dr.Cells["onaccountStatus"].Value;

                        row["TotalDebitAmt"] = lblDrAmt.Text;
                        row["TotalCreditAmt"] = lblCrAmt.Text;
                        row["TotalBalanceAmt"] = lblBalance.Text;
                        row["AvgDays"] = lblAvgDays.Text;
                        row["BalanceWithInt"] = lblFinalBal.Text;
                        row["UserName"] = strUserName;
                        row["TaxAmt"] = lblTaxAmt.Text;

                        if (wStatus)
                        {
                            row["WSR"] = dr.Cells["wsd"].Value;
                            row["TotalWSR"] = lblWSR.Text;
                        }
                        else
                        {
                            if (dExtra > 0 && _bLockdownStatus)
                                row["WSR"] = dExtra.ToString();
                            else
                                row["WSR"] = "0";

                            row["TotalWSR"] = "0";
                        }
                        if (cStatus)
                        {
                            row["DebitInt"] = strDiscountName;

                            row["CD"] = dr.Cells["cd"].Value;
                            row["TotalCD"] = lblCDiscount.Text;
                            row["IDays"] = "0";
                            row["TotalInt"] = "0";
                            row["TotalDebitInt"] = "0.00";
                            row["TotalCreditInt"] = "0.00";
                            if (strCategoryName == "WHOLESALER")
                                row["CreditInt"] = "**";
                            else
                                row["CreditInt"] = "*";
                        }
                        else
                        {
                            row["IDays"] = dr.Cells["iDays"].Value;
                            row["DebitInt"] = dr.Cells["intDr"].Value;
                            row["CreditInt"] = dr.Cells["intCr"].Value;
                            row["TotalDebitInt"] = lblIntDr.Text;
                            row["TotalCreditInt"] = lblIntCr.Text;
                            row["TotalInt"] = lblInterest.Text;

                            if (strCategoryName == "WHOLESALER")
                                row["CD"] = "**";
                            else
                                row["CD"] = "*";
                        }

                        if (!wStatus && !cStatus)
                        {
                            if (lblInterest.Text.Contains("Cr"))
                            {
                                //GraceDays WSR TotalWSR
                                row["BalanceWithoutAmt"] = "Balance without T.Discount";
                                row["DiscountName"] = "T. Discount";
                                row["BalanceWithAmt"] = "Balance with T.Discount";
                            }
                            else
                            {
                                row["BalanceWithoutAmt"] = "Balance without interest";
                                row["DiscountName"] = "Interest Amount";
                                row["BalanceWithAmt"] = "Balance with interest";
                            }
                        }
                        else if (cStatus)
                        {
                            if (lblCDiscount.Text.Contains("Dr"))
                                row["DiscountName"] = "Reverse " + strDiscountName;
                            else
                                row["DiscountName"] = strDiscountName;
                        }
                        myDataTable.Rows.Add(row);
                    }
                }

                dtAdvance = CreateDataTableForPrint();

                if (dtAdvance.Rows.Count > 0 && myDataTable.Rows.Count == 0)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = "INTEREST STATEMENT";
                    row["PartyName"] = "INTEREST STATEMENT OF M/S : " + txtParty.Text;
                    row["DatePeriod"] = strDate;                  
                    row["GraceDays"] = strDiscountName;
                    row["HeaderImage"] = MainPage._headerImage;
                    row["BrandLogo"] = MainPage._brandLogo;
                    if (!wStatus && !cStatus)
                    {
                        if (lblInterest.Text.Contains("Cr"))
                        {
                            row["BalanceWithoutAmt"] = "Balance without T.Discount";
                            row["DiscountName"] = "T. Discount";
                            row["BalanceWithAmt"] = "Balance with T.Discount";
                        }
                        else
                        {
                            row["BalanceWithoutAmt"] = "Balance without interest";
                            row["DiscountName"] = "Interest Amount";
                            row["BalanceWithAmt"] = "Balance with interest";
                        }
                    }
                    else if (cStatus)
                    {
                        if (lblCDiscount.Text.Contains("Dr"))
                            row["DiscountName"] = "Reverse " + strDiscountName;
                        else
                            row["DiscountName"] = strDiscountName;
                    }

                    myDataTable.Rows.Add(row);
                }

                if (myDataTable.Rows.Count > 0)
                {
                    DataTable dt = dba.GetDataTable("Select (SM.Address + ', '+SM.Station+', '+SM.State+'-'+SM.PinCode)Address,(SM.MobileNo+ ' '+SM.PhoneNo)PhoneNo,SM.AccountNo,CD.* from SupplierMaster SM Outer Apply (Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber,BankName,BranchName,IFSCCode,AccountName from CompanyDetails CD  Order by CD.ID asc) CD Where (SM.AreaCode+SM.AccountNo+' '+SM.Name)='" + txtParty.Text + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow _row = dt.Rows[0];
                        myDataTable.Rows[0]["Address"] = _row["Address"];
                        myDataTable.Rows[0]["PhoneNo"] = _row["PhoneNo"];
                        myDataTable.Rows[0]["Other"] = txtParty.Text;
                        myDataTable.Rows[0]["HeaderImage"] = MainPage._headerImage;
                        myDataTable.Rows[0]["BrandLogo"] = MainPage._brandLogo;

                        myDataTable.Rows[0]["CompanyAddress"] = _row["CompanyAddress"];
                        myDataTable.Rows[0]["CompanyEmail"] = _row["CompanyPhoneNo"];
                        myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                        myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];

                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            myDataTable.Rows[0]["BankName"] = "ICICI BANK";
                            myDataTable.Rows[0]["BranchName"] = "DELHI";
                            myDataTable.Rows[0]["BankAccountNo"] = "SASUSP" + dba.ConvertObjectToDouble(_row["AccountNo"]).ToString("000000");
                            myDataTable.Rows[0]["IFSCCode"] = "ICIC0000106";
                        }
                        else
                        {
                            myDataTable.Rows[0]["BankName"] = _row["BankName"];
                            myDataTable.Rows[0]["BranchName"] = _row["BranchName"];
                            myDataTable.Rows[0]["BankAccountNo"] = _row["AccountName"];
                            myDataTable.Rows[0]["IFSCCode"] = _row["IFSCCode"];
                        }

                        if (dtAdvance.Rows.Count > 0)
                            myDataTable.Rows[myDataTable.Rows.Count - 1]["GraceDays"] = "ADVANCE";
                    }
                    else
                        myDataTable.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }

        private DataTable CreateDataTableForPrint()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("Address", typeof(String));
                myDataTable.Columns.Add("PostOffice", typeof(String));
                myDataTable.Columns.Add("PhoneNo", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Account", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("Balance", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("TotalDebit", typeof(String));
                myDataTable.Columns.Add("TotalCredit", typeof(String));
                myDataTable.Columns.Add("TotalBalance", typeof(String));
                myDataTable.Columns.Add("AmountInWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("OnAccount", typeof(String));
                myDataTable.Columns.Add("BankName", typeof(String));
                myDataTable.Columns.Add("BranchName", typeof(String));
                myDataTable.Columns.Add("AccountNo", typeof(String));
                myDataTable.Columns.Add("IFSCCode", typeof(String));
                myDataTable.Columns.Add("FirmName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));

                double dADebitAmt = 0, dACreditAmt = 0, dDAmt = 0, dCAmt = 0, dTAmt = 0;

                foreach (DataGridViewRow row in dgrdInterest.Rows)
                {
                    try
                    {
                        if (Convert.ToString(row.Cells["onaccountStatus"].Value) == "COST")
                        {
                            DataRow dRow = myDataTable.NewRow();
                            dRow["CompanyName"] = MainPage.strPrintComapanyName;
                            dRow["PartyName"] = "Advance Details";

                            dADebitAmt += dDAmt = dba.ConvertObjectToDouble(row.Cells["amountDr"].Value);
                            dACreditAmt += dCAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);

                            dRow["DatePeriod"] = "";
                            dRow["Date"] = Convert.ToDateTime(row.Cells["bDate"].Value).ToString("dd/MM/yyyy");
                            dRow["Account"] = row.Cells["particulars"].Value;
                            dRow["DebitAmt"] = row.Cells["amountDr"].Value;
                            dRow["CreditAmt"] = row.Cells["amountCr"].Value;
                            //dRow["Balance"] = row.Cells["bal"].Value;
                            dRow["Description"] = row.Cells["desc"].Value;

                            dTAmt = dDAmt - dCAmt;
                            if (dTAmt >= 0)
                                dRow["Balance"] = dTAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else
                                dRow["Balance"] = Math.Abs(dTAmt).ToString("N2", MainPage.indianCurancy) + " Cr";


                            dRow["OnAccount"] = "0";
                            dRow["TotalDebit"] = dADebitAmt.ToString("N2", MainPage.indianCurancy);
                            dRow["TotalCredit"] = dACreditAmt.ToString("N2", MainPage.indianCurancy);

                            double _dAmt = dADebitAmt - dACreditAmt;
                            if (_dAmt >= 0)
                                dRow["TotalBalance"] = _dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else
                                dRow["TotalBalance"] = Math.Abs(_dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";

                            dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                            myDataTable.Rows.Add(dRow);
                        }
                        //}
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }


        #region Multi Company

        private void GetMultiQuarterName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";
                dgrdCompany.Rows.Clear();
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    GetMultiCompanyData(strPath, "CURRENT");
                    if (!MainPage._bTaxStatus)
                    {
                        strPath = MainPage.strOldServerPath + "\\Data";
                        folder = new DirectoryInfo(strPath);
                        if (folder.Exists)
                            GetMultiCompanyData(strPath, "OLD");
                    }
                }

                dgrdCompany.Sort(dgrdCompany.Columns["startDate"], ListSortDirection.Ascending);
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

                MainPage.con.Close();
                MainPage.OpenConnection();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in MultiLedger Merging ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void GetMultiCompanyData(string strPath, string strDataType)
        {
            int rowIndex = dgrdCompany.Rows.Count;
            string[] sFolder = Directory.GetDirectories(strPath);
            string strDBName = "";
            DateTime sDate = DateTime.Today, eDate = DateTime.Today;
            foreach (string folderName in sFolder)
            {
                string[] strFile = Directory.GetFiles(folderName, "*.syber");
                if (strFile.Length > 0)
                {
                    FileInfo objFile = new FileInfo(folderName);
                    strDBName = objFile.Name;
                    DataTable dt;
                    if (strDataType == "OLD")
                    {
                        dt = SearchDataOther.GetDataTable("Select ('A'+(CASE WHEN CompanyName Like('%STYLO%') then '0' else '' end)+CAST(CompanyID as varchar)) as CCode,CompanyName,Convert(varchar,Fin_Y_Starts,103) SDate,Convert(varchar,Fin_Y_Ends,103)EDate from Company ", "A" + strDBName);
                    }
                    else
                        dt = dba.GetMultiCompanyNameAndFinDate(strDBName);
                    if (dt.Rows.Count > 0)
                    {
                        dgrdCompany.Rows.Add();
                        sDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["SDate"]));
                        eDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["EDate"]));
                        dgrdCompany.Rows[rowIndex].Cells["companyCheck"].Value = (Boolean)true;
                        dgrdCompany.Rows[rowIndex].Cells["code"].Value = dt.Rows[0]["CCode"];
                        dgrdCompany.Rows[rowIndex].Cells["companyName"].Value = dt.Rows[0]["CompanyName"];
                        dgrdCompany.Rows[rowIndex].Cells["sTextDate"].Value = dt.Rows[0]["SDate"];
                        dgrdCompany.Rows[rowIndex].Cells["eTextDate"].Value = dt.Rows[0]["EDate"];
                        dgrdCompany.Rows[rowIndex].Cells["startDate"].Value = sDate;
                        dgrdCompany.Rows[rowIndex].Cells["endDate"].Value = eDate;
                        dgrdCompany.Rows[rowIndex].Cells["dataType"].Value = strDataType;

                        if (MainPage.multiQSDate > sDate)
                            MainPage.multiQSDate = sDate;
                        if (MainPage.multiQEDate < eDate)
                            MainPage.multiQEDate = eDate;
                        rowIndex++;
                    }
                }
            }
        }



        private void btnSelectCompany_Click(object sender, EventArgs e)
        {
            if (!panelCompany.Visible)
            {
                panelCompany.Visible = true;
                dgrdCompany.Focus();
            }
            else
            {
                panelCompany.Visible = false;
            }
        }

        private void dgrdCompany_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                GetSelectedQuarterDate();
            }
        }

        private void GetSelectedQuarterDate()
        {
            try
            {
                DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                MainPage.multiQSDate = DateTime.Today;
                MainPage.multiQEDate = DateTime.Today;
                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["sTextDate"].Value));
                        eDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["eTextDate"].Value));
                        if (rowCount == 0)
                        {
                            MainPage.multiQSDate = sDate;
                            MainPage.multiQEDate = eDate;
                        }
                        else
                        {
                            if (MainPage.multiQSDate > sDate)
                                MainPage.multiQSDate = sDate;
                            if (MainPage.multiQEDate < eDate)
                                MainPage.multiQEDate = eDate;
                        }
                        rowCount++;
                    }
                }
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public void GetMultiQuarterDetails()
        {
            ClearRecord();
            try
            {
                string strPartyID="", strFirstQuery = "", strOtherQuery = "", strSubQuery = CreateQuery(ref strPartyID,true), strOpeningQuery = "", strCompanyCode = "";
                strOpeningQuery = " Select 0 as ID,AccountID as PartyName,Date,AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,BA.FinalAmount,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,0 as GDays from BalanceAmount BA left join AdjustedIds AID on BA.BalanceID=AID.BalanceID  and AID.MultiCompanyNo=0 Where AccountStatus='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " Union All  ";

                DataTable table = null;//, dtLastInt = null;

                strFirstQuery += " Select Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,FinalAmount,AdjustedNumber,MultiCompanyNo,UserName,(Select Category from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))=PartyName) CategoryName,0 as Onaccount,GDays from ( "
                              + strOpeningQuery
                              + " Select 1 as ID,AccountID as PartyName, (CASE WHEN (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) then ChqDate else Date end) as Date,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' OR AccountStatus='TCS CREDIT NOTE' OR AccountStatus='TCS DEBIT NOTE' OR AccountStatus='DUTIES & TAXES' then AccountStatus else AccountStatus end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, " // dbo.GetFullName(AccountStatusID)
                              + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,BA.FinalAmount,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,(CASE WHEN AccountStatus='SALES A/C' then ISNULL((Select Top 1 GDM.BuyerDays as GDays from SalesRecord SR CROSS APPLY (Select GRSNO from SalesEntry SE Where SR.BIllCode=SE.BillCode and SR.BillNo=SE.BillNo)SE Cross Apply(Select OrderNo,SalePartyID from GoodsReceive GR  Where SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)))GR Cross Apply (Select OfferName,SalePartyID as OBSalePartyID from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo)OB Cross Apply (Select BuyerDays from GraceDaysMaster GDM Where GDM.OfferName=OB.OfferName) GDM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BA.Description and GR.SalePartyID=BA.AccountID and OB.OBSalePartyID=BA.AccountID and SR.SalePartyID=BA.AccountID),0) else 0 end) as GDays from BalanceAmount BA left join AdjustedIds AID on BA.BalanceID=AID.BalanceID  and AID.MultiCompanyNo=0  Where AccountStatus!='OPENING' and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 and CAST(Amount as Money)>0 " + strSubQuery + " ) Balance Order By ID,Date "
                              + " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LedgerAccessDetails]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[LedgerAccessDetails]([ID] [bigint] IDENTITY(1,1) NOT NULL,[AccountType] [nvarchar](250) NULL,[AccountID] [nvarchar](250) NULL,[UserName] [nvarchar](250) NULL,[ComputerName] [nvarchar](250) NULL,[Date] [datetime] NULL,[InsertStatus] [bit] NULL,[UpdateStatus] [bit] NULL) ON [PRIMARY] end "
                              + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('INTEREST','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName.Replace("'", "") + "/" + Environment.UserName.Replace("'", "")).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";

                strOtherQuery += " Select Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,FinalAmount,AdjustedNumber,MultiCompanyNo,UserName,(Select Category from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))=PartyName) CategoryName,0 as Onaccount,GDays from ( "
                             + " Select 1 as ID,AccountID as PartyName, (CASE WHEN (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) then ChqDate else Date end) as Date,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' OR AccountStatus='TCS CREDIT NOTE' OR AccountStatus='TCS DEBIT NOTE' OR AccountStatus='DUTIES & TAXES' then AccountStatus else AccountStatus end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, "
                             + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,BA.FinalAmount,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,(CASE WHEN AccountStatus='SALES A/C' then ISNULL((Select Top 1 GDM.BuyerDays as GDays from SalesRecord SR CROSS APPLY (Select GRSNO from SalesEntry SE Where SR.BIllCode=SE.BillCode and SR.BillNo=SE.BillNo)SE Cross Apply(Select OrderNo,SalePartyID from GoodsReceive GR  Where SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)))GR Cross Apply (Select OfferName,SalePartyID as OBSalePartyID from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo)OB Cross Apply (Select BuyerDays from GraceDaysMaster GDM Where GDM.OfferName=OB.OfferName) GDM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BA.Description and GR.SalePartyID=BA.AccountID and OB.OBSalePartyID=BA.AccountID and SR.SalePartyID=BA.AccountID),0) else 0 end) as GDays from BalanceAmount BA left join AdjustedIds AID on BA.BalanceID=AID.BalanceID  and AID.MultiCompanyNo=0 Where AccountStatus!='OPENING' and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 and CAST(Amount as Money)>0 " + strSubQuery + " ) Balance Order By ID,Date "
                             + " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LedgerAccessDetails]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[LedgerAccessDetails]([ID] [bigint] IDENTITY(1,1) NOT NULL,[AccountType] [nvarchar](250) NULL,[AccountID] [nvarchar](250) NULL,[UserName] [nvarchar](250) NULL,[ComputerName] [nvarchar](250) NULL,[Date] [datetime] NULL,[InsertStatus] [bit] NULL,[UpdateStatus] [bit] NULL) ON [PRIMARY] end "
                             + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('INTEREST','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName.Replace("'", "") + "/" + Environment.UserName.Replace("'", "")).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";

                int rowPCount = 0, rowCCount = 0;
                string strDataType = "";

                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                        strDataType = Convert.ToString(row.Cells["dataType"].Value);
                        if (strCompanyCode != "")
                        {
                            DataTable dt = null;
                            if (strDataType != "OLD")
                            {
                                if (rowPCount == 0)
                                    dt = dba.GetMultiQuarterDataTable(strFirstQuery, strCompanyCode);
                                else                               
                                    dt = dba.GetMultiQuarterDataTable(strOtherQuery, strCompanyCode);
                                   
                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);

                                rowPCount++;
                            }
                            else
                            {
                                if (rowCCount == 0)
                                    dt = SearchDataOther.GetDataTable(strFirstQuery, strCompanyCode);
                                else                                
                                    dt = SearchDataOther.GetDataTable(strOtherQuery, strCompanyCode);
                                   
                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);
                                rowCCount++;
                            }
                        }
                    }
                }

                SetNewAndOldScheme(table);
                               
                //if (dtLastInt != null)
                //    SetIntDiscountValue(dtLastInt);

                string strQuery = " Select CONVERT(varchar,Date,103)BDate,(CASE When AccountStatus='JOURNAL A/C' then AccountStatus else (AccountID+' '+Name) end +(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and SM.TINNumber='COST CENTRE') SM Where AccountID!=''  " + strSubQuery.Replace(" AccountID=", " CostCentreAccountID=") + " Order By Date  ";
                DataTable _dt = dba.GetDataTable(strQuery);
                SetCostCenterDetails(_dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void SetCostCenterDetails(DataTable table)
        {
            if (table != null)
            {
                int rowIndex = dgrdInterest.Rows.Count;

                if (table.Rows.Count > 0)
                    dgrdInterest.Rows.Add(table.Rows.Count);
                double dDebitAmt = 0, dCreditAmt = 0, dTotalAmt = 0;
                foreach (DataRow row in table.Rows)
                {
                    dDebitAmt = dba.ConvertObjectToDouble(row["DebitAmt"]);
                    dCreditAmt = dba.ConvertObjectToDouble(row["CreditAmt"]);
                    dTotalAmt += dDebitAmt - dCreditAmt;

                    dgrdInterest.Rows[rowIndex].Cells["chk"].Value = false;
                    dgrdInterest.Rows[rowIndex].Cells["bDate"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["BDate"]));
                    dgrdInterest.Rows[rowIndex].Cells["particulars"].Value = row["AccountStatus"];
                    dgrdInterest.Rows[rowIndex].Cells["desc"].Value = row["Description"];
                    if (dDebitAmt > 0)
                        dgrdInterest.Rows[rowIndex].Cells["amountDr"].Value = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        dgrdInterest.Rows[rowIndex].Cells["amountCr"].Value = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                    if (dTotalAmt >= 0)
                        dgrdInterest.Rows[rowIndex].Cells["bal"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        dgrdInterest.Rows[rowIndex].Cells["bal"].Value = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy);

                    dgrdInterest.Rows[rowIndex].Cells["iDays"].Value = "0";
                    dgrdInterest.Rows[rowIndex].Cells["intDr"].Value = "";
                    dgrdInterest.Rows[rowIndex].Cells["intcr"].Value = "";
                    dgrdInterest.Rows[rowIndex].Cells["final"].Value = "0";
                    dgrdInterest.Rows[rowIndex].Cells["tick"].Value = row["Tick"];
                    dgrdInterest.Rows[rowIndex].Cells["adjustedNo"].Value = "";
                    dgrdInterest.Rows[rowIndex].Cells["adjuster"].Value = "";
                    dgrdInterest.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    dgrdInterest.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdInterest.Rows[rowIndex].Cells["wsd"].Value = "0";
                    dgrdInterest.Rows[rowIndex].Cells["cd"].Value = "0";
                    dgrdInterest.Rows[rowIndex].Cells["onaccountStatus"].Value = "COST";

                    dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                    rowIndex++;
                }
            }
        }

        private void dgrdInterest_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdInterest.CurrentRow.DefaultCellStyle.BackColor.Name != "Gold")
                    {
                        int rowIndex = dgrdInterest.CurrentRow.Index;
                        if (dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                        {
                            dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdInterest.CurrentCell.ColumnIndex == 2 && dgrdInterest.CurrentCell.RowIndex >= 0)
                    {
                        ShowDetails();
                    }
                }
            }
            catch
            {
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            picPleasewait.Visible = true;
            try
            {
                if (txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    string strPath = "";
                    PrintPreviewExport(2, ref strPath);
                }
            }
            catch
            {
            }
            btnExport.Enabled = true;
            picPleasewait.Visible = false;
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            picPleasewait.Visible = true;
            try
            {
                if (txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    string strPath = "";
                    PrintPreviewExport(1, ref strPath);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
            picPleasewait.Visible = false;
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                btnPartyName.Enabled = pnlRelatedParty.Visible = false;
                if (objSearch == null)
                {
                    objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS NAME", Keys.Space);
                }
                else
                {
                    objSearch.txtSearch.Text = "";
                }
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                {
                    txtParty.Text = objSearch.strSelectedData;
                    GetPartyDueDaysAndCDDays();
                }
                ClearRecord();
                GetRelatedpartyDetails();
            }
            catch { }
            btnPartyName.Enabled = true;
        }

        private void txtParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtParty.Text);
        }



        private void lblOpenMaster_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtParty.Text);
        }

        private void lnkHint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pnlColor.Visible)
                pnlColor.Visible = false;
            else
                pnlColor.Visible = true;
        }

        private void lnkShowIntDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //try
            //{
            //    if (_dtIntDiscDetails != null && _dtIntDiscDetails.Rows.Count > 0)
            //    {
            //        LastIntDiscountDetails objLast = new LastIntDiscountDetails(_dtIntDiscDetails);
            //        objLast.ShowDialog();
            //    }
            //}
            //catch { }
        }

        private void chkTax_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalAmount();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                foreach (DataGridViewRow row in dgrdInterest.Rows)
                {
                    row.Cells["chk"].Value = chkAll.Checked;
                }
            }
            catch { }
        }

        private void lnkShowMasterSummary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if(txtParty.Text!="")
                {
                    ShowPartyMasterSummary objSummary = new ShowPartyMasterSummary(txtParty.Text);
                    objSummary.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummary.ShowInTaskbar = true;
                    objSummary.Show();
                }
            }
            catch { }
        }

        private void txtExtraDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (MainPage.strUserRole.Contains("ADMIN"))
                dba.KeyHandlerPoint(sender, e, 0);
            else
                e.Handled = true;
        }

        private void txtExtraDays_Leave(object sender, EventArgs e)
        {
            
        }

        private void dgrdInterest_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkAll.Visible = false;
                    else
                        chkAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendEmail.Enabled = false;
                picPleasewait.Visible = true;
                System.Threading.Thread.Sleep(5);
                SendEmailAndWhatsAppToParty();
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
            picPleasewait.Visible = false;
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtParty.Text, strNewSecurityDate = "", strNewGrade = "", strNewMobileNo = "", strNewCategory = "", strNewLimit = "", strNewBlackList = "", strNewTransactionLock = "";//, strNewLastPaymentDate = "", strNewLastPaymentAmt = "";
                    if (strParty != "")
                    {
                        txtParty.Text = strParty;
                        strNewSecurityDate = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["securityChqDate"].Value);
                        strNewGrade = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["Grade"].Value);
                        strNewMobileNo = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["MobileNo"].Value);
                        strNewCategory = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["Category"].Value);
                        strNewLimit = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["amtLimit"].Value);

                        strNewBlackList = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["blackListed"].Value);
                        strNewTransactionLock = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["transactionLock"].Value);
                        //strNewLastPaymentDate = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["lastPaymentDate"].Value);
                        //strNewLastPaymentAmt = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["lastPaymentAmt"].Value);


                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                        dgrdRelatedParty.CurrentRow.Cells["securityChqDate"].Value = _STRChqDate;
                        dgrdRelatedParty.CurrentRow.Cells["Grade"].Value = _STRGrade;
                        dgrdRelatedParty.CurrentRow.Cells["MobileNo"].Value = _STRMobileNo;
                        dgrdRelatedParty.CurrentRow.Cells["Category"].Value = _STRCategory;
                        dgrdRelatedParty.CurrentRow.Cells["amtLimit"].Value = _STRAmtLimit;

                        dgrdRelatedParty.CurrentRow.Cells["blackListed"].Value = _STRBlackList;
                        dgrdRelatedParty.CurrentRow.Cells["transactionLock"].Value = _STRTransasactionLock;
                        //dgrdRelatedParty.CurrentRow.Cells["lastPaymentDate"].Value = _STRLastPaymentDate;
                        //dgrdRelatedParty.CurrentRow.Cells["lastPaymentAmt"].Value = _STRLastPaymentAmt;


                        _STRChqDate = strNewSecurityDate;
                        _STRBlackList = strNewBlackList;
                        _STRTransasactionLock = strNewTransactionLock;
                        //_STRLastPaymentDate = strNewLastPaymentDate;
                        //_STRLastPaymentAmt = strNewLastPaymentAmt;

                        _STRGrade = strNewGrade;
                        _STRCategory = strNewCategory;
                        _STRMobileNo = strNewMobileNo;
                        _STRAmtLimit = strNewLimit;

                        if (!_STRGrade.Contains("GRADE") && _STRGrade != "")
                            _STRGrade = "GRADE : " + _STRGrade;
                        if (_STRGrade != "" && !_STRGrade.Contains(","))
                            _STRGrade += ", ";

                        //lblGradeCategory.Text = _STRGrade + " " + _STRCategory;// + _STRLastPaymentAmt + _STRLastPaymentDate;
                        //lblMobileNoLimit.Text = "MOBILE No : " + _STRMobileNo + ", LIMIT : " + dba.ConvertObjectToDouble(_STRAmtLimit).ToString("N2", MainPage.indianCurancy);// + _STRBlackList;

                        //lblGradeCategory.Text = _STRGrade + "CATEGORY : " + _STRCategory + " " + _STRLastPaymentDate + _STRLastPaymentAmt;
                        //lblMobileNoLimit.Text = "MOBILE No : " + _STRMobileNo + ", AMT LIMIT : " + dba.ConvertObjectToDouble(_STRAmtLimit).ToString("N2", MainPage.indianCurancy) + " " + _STRBlackList;

                    }
                    txtParty.Focus();
                }

                bool _bTransactionLock = Convert.ToBoolean(_STRTransasactionLock), _bBlackListed = false, _bSecurityChq = false;
                if (_STRChqDate != "")
                    _bSecurityChq = true;
                if (_STRBlackList != "")
                    _bBlackListed = true;

                dba.SetPinColorInPictureBox(txtParty, picBoxPin, _bTransactionLock, _bBlackListed, _bSecurityChq);

                //if (_STRChqDate != "")
                //{
                //   // lblChequeStatus.Text = "SECURITY CHQ RECEIVED ON DATE : " + _STRChqDate;
                //    txtParty.BackColor = Color.LightGreen;
                //}
                //if (_STRBlackList != "")
                //    txtParty.BackColor = Color.Gold;
                //else if (Convert.ToBoolean(_STRTransasactionLock))
                //    txtParty.BackColor = Color.Tomato;
                //else
                //{
                //    lblChequeStatus.Text = "";
                //    txtParty.BackColor = Color.White;
                //}
            }
            catch { }
        }

        private void txtParty_Enter(object sender, EventArgs e)
        {
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
            else
                pnlRelatedParty.Visible = false;
        }

        private void txtParty_Leave(object sender, EventArgs e)
        {
            pnlRelatedParty.Visible = false;
        }

        private void lblChequeStatus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblChequeStatus.Text != "" && txtParty.Text != "")
                {
                    ChequeDetailRegister objChequeDetailRegister = new ChequeDetailRegister(txtParty.Text, "SECURITY");
                    objChequeDetailRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objChequeDetailRegister.ShowDialog();
                }
            }
            catch { }
        }

        private void dgrdCompany_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        //private void SendEmail()
        //{
        //    if (txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
        //    {
        //        string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
        //        //if (strEmailID != "")
        //        //{
        //         string strEmailID = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select EmailID from SupplierMaster where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + txtParty.Text + "' and GroupName!='SUB PARTY' "));
        //         if (strEmailID != "")
        //         {
        //             strPath = CreatePDFFile();
        //             if (strPath != "")
        //             {
        //                 strSubject = "INTEREST STATEMENT  FROM " + MainPage.strGRCompanyName;
        //                 strBody = "We are sending Int. Statement , which is Attached with this mail, Please Find it.";
        //                 bool bStatus= DataBaseAccess.SendEmail(strEmailID, strSubject, strBody, strPath,"", "");
        //                 if(bStatus)
        //                     MessageBox.Show("Thank you ! Email sent successfully !!","Email Sent",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
        //                 else
        //                     MessageBox.Show("Sorry ! Unable to send right now", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //             }
        //         }
        //         else
        //             MessageBox.Show("Sorry ! Please enter email id in party master after that you can send email !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        //}
        //    }
        //    else
        //    {
        //        MessageBox.Show("Sorry ! Party Name can't be blank ", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        txtParty.Focus();
        //    }
        //}

        private void SendEmailAndWhatsAppToParty()
        {
            try
            {
                string[] strParty = txtParty.Text.Split(' ');
                if (strParty.Length > 0)
                {
                    string strOrginalFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    string strPartyID = strParty[0], strFileName = "", strPath = "";
                    strFileName = strPartyID + "_" + strOrginalFileName + ".pdf";

                    string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Interest_Statement";
                    strPath = strNewPath + "\\" + strFileName;

                    if (File.Exists(strPath))
                        File.Delete(strPath);

                    Directory.CreateDirectory(strNewPath);

                    PrintPreviewExport(2, ref strPath);

                    // strPath = CreatePDFFile(strPath);

                    string strEmailID = "", strWhatsAppNo = "";
                    if (strPath != "")
                    {
                        string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strPartyID + "' ";
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                            strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                            if (strEmailID != "")
                            {
                                string strMessage = "A/c : " + txtParty.Text + ", we are sending interest statement which is attached with this mail, Please find attachment.";
                                string strSub = "INTEREST STATEMENT";

                                DataBaseAccess.SendEmail(strEmailID, strSub, strMessage, strPath, "", "INTEREST STATEMENT",true);
                            }
                            if (strWhatsAppNo != "")
                            {
                                SendWhatsappMessage(strWhatsAppNo, strPath, strFileName);
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

        private void SendWhatsappMessage(string strMobileNo, string strPath, string strFileName)
        {
            string strMessage = "";

            string strFilePath = MainPage.strHttpPath + "/Interest_Statement/" + strFileName, strName = dba.GetSafePartyName(txtParty.Text);

            //strMessage = "M/S : " + strName + ", WE ARE SENDING INTEREST STATEMENT, PLEASE FIND ATTACHMENT.";
            bool _bStatus = dba.UploadLedgerInterestStatementPDFFile(strPath, strFileName, "Interest_Statement");
            if (_bStatus)
            {
                strMessage = "\"variable1\": \"" + strName + "\",";
                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, "interest_pdf", strMessage, "", strFilePath);

                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }


        //private string CreatePDFFile(string strFileName)
        //{
        //    string strPath = "", strPartyName = txtParty.Text;
        //    try
        //    {
        //        //strPartyName = strPartyName.Replace(" ", "_").Replace(".", "_").Replace("/", "_");
        //        //strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Int_Statement";
        //        //if (!Directory.Exists(strPath))
        //        //    Directory.CreateDirectory(strPath);

        //        //strFileName = strPath + "\\Int_Statement.pdf";

        //        bool wStatus = dgrdInterest.Columns["wsd"].Visible, cStatus = dgrdInterest.Columns["cd"].Visible;
        //        DataTable dt = CreatePrintDataTable(wStatus, cStatus);

        //        if (dt.Rows.Count > 0)
        //        {
        //            if (File.Exists(strFileName))
        //                File.Delete(strFileName);

        //            if (!wStatus && !cStatus)
        //            {
        //                Reporting.InterestReport objReport = new SSS.Reporting.InterestReport();
        //                objReport.SetDataSource(dt);
        //                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
        //            }
        //            else if (wStatus && !cStatus)
        //            {
        //                Reporting.WSRInterestReport objReport = new SSS.Reporting.WSRInterestReport();
        //                objReport.SetDataSource(dt);
        //                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
        //            }
        //            else if (!wStatus && cStatus)
        //            {
        //                Reporting.CDInterestReport objReport = new SSS.Reporting.CDInterestReport();
        //                objReport.SetDataSource(dt);
        //                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
        //            }
        //            else if (wStatus && cStatus)
        //            {
        //                Reporting.WSRCDReport objReport = new SSS.Reporting.WSRCDReport();
        //                objReport.SetDataSource(dt);
        //                objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
        //            }                  
        //        }
        //        else
        //            strFileName = "";
        //    }
        //    catch
        //    {
        //        strFileName = "";
        //    }
        //    return strFileName;
        //}

        private void GetRelatedpartyDetails()
        {
            try
            {
                pnlRelatedParty.Visible = picBoxPin.Visible = lblChequeStatus.Visible = false;//lblGradeCategory.Visible = lblMobileNoLimit.Visible = 
                dgrdRelatedParty.Rows.Clear();
                _STRChqDate = "";// lblGradeCategory.Text = lblMobileNoLimit.Text = "";

                if (txtParty.Text != "")
                {
                    DataSet _ds = dba.GetRelatedPartyDetailsWithChequeDate(txtParty.Text);
                    if (_ds.Tables.Count > 0)
                    {
                        DataTable dt = _ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            dgrdRelatedParty.Rows.Add(dt.Rows.Count);
                            int _index = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                _STRBlackList = Convert.ToString(row["BlackListReason"]);
                                _STRTransasactionLock = Convert.ToString(row["TransactionLock"]);
                                //_STRLastPaymentDate = Convert.ToString(row["LastPaymentDate"]);
                                //_STRLastPaymentAmt = Convert.ToString(row["LastPayment"]);

                                if (Convert.ToBoolean(row["BlackList"]))
                                    _STRBlackList = ", Blacklisted : " + _STRBlackList;
                                //if (_STRLastPaymentAmt != "")
                                //    _STRLastPaymentAmt = ", Last Payment: " + dba.ConvertObjectToDouble(_STRLastPaymentAmt).ToString("N2", MainPage.indianCurancy);
                                //if (_STRLastPaymentDate != "")
                                //    _STRLastPaymentDate = ", Date : " + _STRLastPaymentDate;

                                dgrdRelatedParty.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                                dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["Name"];
                                dgrdRelatedParty.Rows[_index].Cells["securityChqDate"].Value = row["ChqDate"];
                                dgrdRelatedParty.Rows[_index].Cells["Grade"].Value = row["Grade"];
                                dgrdRelatedParty.Rows[_index].Cells["category"].Value = row["Category"];
                                dgrdRelatedParty.Rows[_index].Cells["amtLimit"].Value = row["AmountLimit"];
                                dgrdRelatedParty.Rows[_index].Cells["mobileNo"].Value = row["MobileNo"];

                                dgrdRelatedParty.Rows[_index].Cells["blackListed"].Value = _STRBlackList;
                                dgrdRelatedParty.Rows[_index].Cells["transactionLock"].Value = _STRTransasactionLock;


                                //dgrdRelatedParty.Rows[_index].Cells["lastPaymentDate"].Value = _STRLastPaymentDate;
                                //dgrdRelatedParty.Rows[_index].Cells["lastPaymentAmt"].Value = _STRLastPaymentAmt;

                                _index++;
                            }
                        }
                        dt = _ds.Tables[1];
                        if (dt.Rows.Count > 0)
                        {
                            DataRow _row = dt.Rows[0];
                            _STRChqDate = Convert.ToString(_row["_ChqDate"]);
                            _STRGrade = Convert.ToString(_row["Grade"]);
                            _STRCategory = Convert.ToString(_row["Category"]);
                            _STRMobileNo = Convert.ToString(_row["MobileNo"]);
                            _STRAmtLimit = Convert.ToString(_row["AmountLimit"]);
                            _STRBlackList = Convert.ToString(_row["BlackListReason"]);
                            _STRTransasactionLock = Convert.ToString(_row["TransactionLock"]);
                            //_STRLastPaymentDate = Convert.ToString(_row["LastPaymentDate"]);
                            //_STRLastPaymentAmt = Convert.ToString(_row["LastPayment"]);

                            if (Convert.ToBoolean(_row["BlackList"]))
                                _STRBlackList = ", Blacklisted : " + _STRBlackList;

                            //if (_STRLastPaymentAmt != "")
                            //    _STRLastPaymentAmt = ", Last Payment: " + dba.ConvertObjectToDouble(_STRLastPaymentAmt).ToString("N2", MainPage.indianCurancy);
                            //if (_STRLastPaymentDate != "")
                            //    _STRLastPaymentDate = ", Date : " + _STRLastPaymentDate;

                            if (!_STRGrade.Contains("GRADE") && _STRGrade != "")
                                _STRGrade = "GRADE : " + _STRGrade;
                            if (_STRGrade != "" && !_STRGrade.Contains(","))
                                _STRGrade += ", ";

                            //lblGradeCategory.Text = _STRGrade + " " + _STRCategory;// + _STRLastPaymentAmt + _STRLastPaymentDate;
                            //lblMobileNoLimit.Text = "MOBILE No : " + _STRMobileNo + ", LIMIT : " + dba.ConvertObjectToDouble(_STRAmtLimit).ToString("N2", MainPage.indianCurancy);// + _STRBlackList;
                            picBoxPin.Visible = true;// lblGradeCategory.Visible = lblMobileNoLimit.Visible = lblChequeStatus.Visible = true;
                        }
                    }
                }

                if (dgrdRelatedParty.Rows.Count > 0)
                    pnlRelatedParty.Visible = true;

                bool _bTransactionLock = Convert.ToBoolean(_STRTransasactionLock), _bBlackListed = false, _bSecurityChq = false;
                if (_STRChqDate != "")
                    _bSecurityChq = true;
                if (_STRBlackList != "")
                    _bBlackListed = true;

                dba.SetPinColorInPictureBox(txtParty, picBoxPin, _bTransactionLock, _bBlackListed, _bSecurityChq);

                //if (_STRChqDate != "")
                //{
                // //   lblChequeStatus.Text = "SECURITY CHQ RECEIVED ON DATE : " + _STRChqDate;
                //    txtParty.BackColor = Color.LightGreen;
                //}
                //if (_STRBlackList != "")
                //    txtParty.BackColor = Color.Gold;
                //else if (Convert.ToBoolean(_STRTransasactionLock))
                //    txtParty.BackColor = Color.Tomato;
                //else
                //{
                //    lblChequeStatus.Text = "";
                //    txtParty.BackColor = Color.White;
                //}
            }
            catch { }
        }

    }
}
