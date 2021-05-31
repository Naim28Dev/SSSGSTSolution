using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace SSS
{
    public partial class LedgerAccount_Remark : Form
    {
        DataBaseAccess dba;
        string[] strColor = { "LightSteelBlue", "PeachPuff", "Thistle", "Lavender", "LightSalmon", "LightCoral", "ButtonShadow", "BurlyWood", "Gainsboro", "Beige" };
        int index = 0;
        //DataTable _dtIntDiscDetails = null;
        protected internal bool _bPrevilegeAccount = false;
        ChangeCurrencyToWord currency;
        string[] strAllParty, strPartyStatus;
        string _STRChqDate = "",_STRGrade="",_STRCategory="",_STRAmtLimit="",_STRMobileNo="", _STRBlackList = "", _STRTransasactionLock = "",_STROrangeList="";
        PrinterSettings PS = new PrinterSettings();
        public LedgerAccount_Remark()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
            btnSendSMS.Enabled = btnSendWhatsapp.Enabled = MainPage.mymainObject.bSMSReport;
        }

        public LedgerAccount_Remark(string strPartyName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
            txtParty.Text = strPartyName;
            GetRelatedpartyDetails();
            SearchRecord();
            btnSendSMS.Enabled = btnSendWhatsapp.Enabled = MainPage.mymainObject.bSMSReport;
        }

        public LedgerAccount_Remark(string[] strPName, string[] strPStatus, DateTime sDate, DateTime eDate)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                currency = new ChangeCurrencyToWord();
                strAllParty = strPName;
                strPartyStatus = strPStatus;
                chkDate.Checked = true;
                txtFromDate.Text = sDate.ToString("dd/MM/yyyy");
                txtToDate.Text = eDate.ToString("dd/MM/yyyy");
                btnPrint.Enabled = false;
                btnSelectCompany.Enabled = true;
                GetMultiQuarterName();

                BindMultiLedgerAccount();
                btnSendSMS.Enabled = btnSendWhatsapp.Enabled = MainPage.mymainObject.bSMSReport;
            }
            catch
            {
            }
        }


        public LedgerAccount_Remark(bool mStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
            btnSendSMS.Enabled = btnSendWhatsapp.Enabled = MainPage.mymainObject.bSMSReport;
            if (mStatus)
            {
                btnSelectCompany.Enabled = true;
                GetMultiQuarterName();
            }
        }

        private void LedgerAccount_KeyDown(object sender, KeyEventArgs e)
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
                    else if (panelSearch.Visible)
                        panelSearch.Visible = false;
                    else if (index > 0)
                    {
                        if (!btnPrint.Enabled)
                        {
                           // index++;
                            BindMultiLedgerAccount();
                        }
                        else
                            this.Close();
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
                else if (e.KeyCode == Keys.F7)
                {
                    if (!btnPrint.Enabled)
                        PrintLedger();
                }
            }
            catch
            {
            }
        }

        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender,e,dgrdRelatedParty);

                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtParty.Text = objSearch.strSelectedData;

                    GetRelatedpartyDetails();
                    ClearRecord();
                    txtParty.Focus();
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

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            string strDate = txtFromDate.Text;
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, false, true);
            if (strDate != txtFromDate.Text)
                ClearRecord();

            dba.ChangeLeaveColor(sender, e);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            ClearRecord();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            SearchRecord();
            btnGo.Enabled = true;
        }

        private string CreateQuery(ref string strChqStatus, ref string strInvQuery,ref string strPartyID,bool _bStatus)
        {
            string strQuery = "";
            try
            {
                if (txtAccountID.Text != "" && !MainPage._bTaxStatus)
                {
                    string[] strFullName = txtAccountID.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strQuery += " and AccountID='" + strFullName[0].Trim() + "' ";
                    }
                }
                else if (txtParty.Text != "")
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
                if (chkChqDate.Checked)
                {
                    DateTime startChqDate = dba.ConvertDateInExactFormat(txtClearFromDate.Text), endChqDate = dba.ConvertDateInExactFormat(txtClearToDate.Text).AddDays(1);
                    strQuery += " and ((CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then Date else CHqDate end) <'" + endChqDate.ToString("MM/dd/yyyy h:mm:ss tt") + "') ";

                    strQuery += " and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 ";
                }
                if (chkInvoiceDate.Checked)
                {
                    DateTime _InvDate = dba.ConvertDateInExactFormat(txtInvFromDate.Text), endInvDate = dba.ConvertDateInExactFormat(txtInvToDate.Text).AddDays(1);
                    strInvQuery = " OUTER APPLY (Select InvoiceDate as BillDate from PurchaseRecord Where AccountStatus='PURCHASE A/C' and (BillCode+' '+CAST(BillNo as varchar))=Description)PR ";

                    strQuery += " and ISNULL(BillDate,Date) >='" + _InvDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and ISNULL(BillDate,Date) <'" + endInvDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
                }
                else
                    strInvQuery = " OUTER APPLY (Select Date as BillDate)PR ";

                if (txtAmount.Text != "")
                    strQuery += " and Cast(Amount as Money) = " + Convert.ToDouble(txtAmount.Text) + "  ";

                if (txtDescription.Text != "")
                    strQuery += " and Description Like('%" + txtDescription.Text + "%') ";

                if (txtVCode.Text != "")
                    strQuery += " and VoucherCode ='" + txtVCode.Text + "' ";

                if (txtVNo.Text != "")
                    strQuery += " and VoucherNo =" + txtVNo.Text + " ";

                if (txtMonthName.Text != "")
                {
                    strQuery += " and UPPER(DATENAME(MM,Date))='" + txtMonthName.Text + "'  ";
                }

                if (comboAccount.SelectedIndex > 0)
                {
                    string strAccountStatus = Convert.ToString(comboAccount.SelectedItem);
                    if (strAccountStatus == "CASH A/C")
                        strQuery += " and  AccountStatus in (Select CASHVCode from CompanySetting) ";
                    else if (strAccountStatus == "BANK A/C")
                        strQuery += " and  AccountStatus in  (Select BankVCode from CompanySetting) ";
                    else
                        strQuery += " and  AccountStatus Like('%" + strAccountStatus + "%')  ";
                }

                string strStatus = GetStatus();
                if (strStatus != "")
                    strQuery += " and Tick='" + strStatus + "' ";

                if (rdoStatusDR.Checked)
                    strQuery += " and Status='DEBIT' ";
                else if (rdoStatusCr.Checked)
                    strQuery += " and Status='CREDIT' ";

                if (rdoCHQClear.Checked)
                    strChqStatus += " and CHQStatus='CLEAR' ";
                else if (rdoCHQUnclear.Checked)
                    strChqStatus += " and CHQStatus='UNCLEAR' ";

                if (rdoAdvance.Checked)
                    strChqStatus += " and ID=2 ";
                else if (rdoWithoutAdvance.Checked)
                    strChqStatus += " and ID<>2 ";



            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Ledger Account", ex.Message };
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

        private void ClearRecord()
        {
            dgrdLedger.Rows.Clear();
            lblBalAmount.Text = lblBalance.Text = lblCredit.Text = lblDebit.Text = "0.00";
        }

        public void GetCurrentQuarterDetails()
        {
            ClearRecord();
            string strQuery = "",strPartyID="", strChqStatus = "", strInvQuery = "", strSubQuery = CreateQuery(ref strChqStatus, ref strInvQuery, ref strPartyID,false), strOuterQuery = "" ;
           
            strQuery += " Select BalanceID,Date, CONVERT(varchar,Date,103)BDate,ISNULL(UPPER(AccountStatus),'') AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,AdjustedNumber,MultiCompanyNo,JournalID,UserName,CHQStatus,0 Onaccount,ISNULL(ChqDate,'') ChqDate,(CASE WHEN ChqDate is NULL or ChqDate='' then Date else Convert(datetime,ChqDate,103) end)ChequeDate,ID,Remark from ( "
                         + " Select 0 as ID,BA.BalanceID,Date,AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,'' as CHQStatus, '' ChqDate,BillDate,'' as Remark from BalanceAmount BA  OUTER APPLY (Select TOP 1  AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='" + MainPage.strDataBaseFile + "')AID OUTER APPLY (Select Date as BillDate)PR  Where AccountStatus='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " Union All  "
                         + " Select 1 as ID,BA.BalanceID,Date,(CASE When VoucherCode!='' then AccountStatus+' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else AccountStatus end)AccountStatus,(CASE WHEN AccountStatus='SALE SERVICE' then Description+'|'+SDescription WHEN ISNULL(CostCentreAccountID,'') !='' then (dbo.GetFullName(CostCentreAccountID)) else Description end) Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar,ChqDate,103) ChqDate,BillDate,ISNULL(Remark,'')Remark "
                         + " from BalanceAmount BA OUTER APPLY (Select Top 1 (ItemName+' : '+ SAC) as SDescription from SaleServiceDetails Where (BillCode+' '+CAST(BillNo as varchar))=Description and AccountStatus='SALE SERVICE') SSD  OUTER APPLY (Select TOP 1  AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='" + MainPage.strDataBaseFile + "')AID " + strInvQuery + " Left join (Select BillNo,Remark from (Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseBook Where PurchasePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SalesBook Where SalePartyID='" + strPartyID+ "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SaleReturn Where SalePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseReturn Where PurchasePartyID='" + strPartyID + "')_Sales)Sales on BA.Description=Sales.BillNo and BA.AccountStatus in ('PURCHASE A/C','SALES A/C') Where AccountStatus !='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " UNION ALL "
                         + " Select 2 as ID,BA.BalanceID,Date,(CASE When AccountStatus='JOURNAL A/C' then AccountStatus else (AccountID+' '+Name) end +(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar,ChqDate,103) ChqDate,'' as BillDate,'' as Remark from BalanceAmount BA OUTER APPLY (Select TOP 1  AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='" + MainPage.strDataBaseFile + "')AID CROSS APPLY (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and SM.TINNumber='COST CENTRE') SM Where AccountID!='' " + strSubQuery.Replace(" AccountID=", " CostCentreAccountID=") + ") Balance Where ID>=0 " + strChqStatus + " Order By ID,Date "
                         + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('LEDGER','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName.Replace("'", "") + "/" + Environment.UserName.Replace("'", "")).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";
            
            DataTable _dt = dba.GetDataTable(strQuery);
            SetRecordWithDataTable(_dt);
            
        }
       
        private void SetRecordWithDataTable(DataTable dt)
        {
            DataTable _datatable = CreateDataTable();
            string strAdjustedNo = "", strAdjuster = "",strTick="";
            if (dt != null)
            {
                double dDebitAmt = 0, dCreditAmt = 0, dAmt = 0, dTotalAmt = 0;
                int rowLength = 0, colorIndex = 0;
                if (chkDate.Checked || chkChqDate.Checked)
                {
                    DateTime sDate = MainPage.startFinDate;
                    DataRow[] rows;
                    if (chkDate.Checked && txtFromDate.Text.Length == 10)
                    {
                        sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                        rows = dt.Select("Date<'" + sDate.ToString("MM/dd/yyyy") + "' and ID<>2  ");
                    }
                    else if (chkChqDate.Checked && txtClearFromDate.Text.Length == 10)
                    {
                        sDate = dba.ConvertDateInExactFormat(txtClearFromDate.Text);
                        DataView dv = dt.DefaultView;
                        dv.Sort = "ChequeDate";
                        dt = dv.ToTable();
                        rows = dt.Select("ChequeDate<'" + sDate.ToString("MM/dd/yyyy") + "'  and ID<>2 ");
                    }
                    else
                        rows = dt.Select("Date<'" + sDate.ToString("MM/dd/yyyy") + "'  and ID<>2 ");

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
                                if (Convert.ToString(row["Tick"]) == "")
                                    row["Tick"] = "False";
                                tickStatus = Convert.ToBoolean(row["Tick"]);
                                strAdjustedNo = Convert.ToString(row["AdjustedNumber"]);
                                strAdjuster = Convert.ToString(row["UserName"]);
                            }
                        }
                        dTotalAmt = dDebitAmt - dCreditAmt;
                        if (dTotalAmt > 0)
                        {
                            DataRow dRow = _datatable.NewRow();
                            dRow["Date"] = sDate.ToString("dd/MM/yyyy");
                            dRow["AccountStatus"] = "OPENING";
                            dRow["DebitAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                            dRow["BalanceAmt"] = dRow["DebitAmt"] + " Dr";
                            dRow["Tick"] = tickStatus;
                            dRow["AdjusterName"] = strAdjuster;
                            dRow["ChqDate"] = dRow["Remark"] = "";
                            dRow["ID"] = "0";
                            if (tickStatus)
                            {
                                dRow["ColorIndex"] = 0;
                                if (strAdjustedNo == "")
                                    strAdjustedNo = "0";
                            }

                            dRow["AdjustedNo"] = strAdjustedNo;
                            dDebitAmt = dTotalAmt;
                            dCreditAmt = 0;
                            _datatable.Rows.Add(dRow);
                        }
                        else if (dTotalAmt < 0)
                        {
                            DataRow dRow = _datatable.NewRow();
                            dRow["Date"] = sDate.ToString("dd/MM/yyyy");
                            dRow["AccountStatus"] = "OPENING";
                            dRow["CreditAmt"] = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy);
                            dRow["BalanceAmt"] = dRow["CreditAmt"] + " Cr";
                            dRow["Tick"] = tickStatus;
                            dRow["AdjusterName"] = strAdjuster;
                            dRow["ChqDate"] = dRow["Remark"] = "";
                            dRow["ID"] = "0";
                            if (tickStatus)
                            {
                                dRow["ColorIndex"] = 0;
                                if (strAdjustedNo == "")
                                    strAdjustedNo = "0";
                            }

                            dRow["AdjustedNo"] = strAdjustedNo;
                            _datatable.Rows.Add(dRow);
                            dCreditAmt = Math.Abs(dTotalAmt);
                            dDebitAmt = 0;
                        }
                    }
                }


                for (; rowLength < dt.Rows.Count; rowLength++)
                {
                    DataRow row = dt.Rows[rowLength];
                    if (Convert.ToString(row["ID"]) != "2")
                    {
                        DataRow dRow = _datatable.NewRow();
                        dRow["BalanceID"] = row["BalanceID"];
                        dRow["Date"] = row["BDate"];
                        dRow["AccountStatus"] = row["AccountStatus"];
                        dRow["Description"] = row["Description"];
                        dRow["CreatedBy"] = row["CreatedBy"];
                        dRow["UpdatedBy"] = row["UpdatedBy"];
                        dRow["journalID"] = row["JournalID"];
                        dRow["ChqDate"] = row["ChqDate"];
                        dRow["ChqStatus"] = row["CHQStatus"];
                        dRow["Remark"] = Convert.ToString(row["Remark"]);
                        dRow["Tick"] = false;
                        dRow["ID"] = row["ID"];

                        if (Convert.ToString(row["Tick"]) == "")
                            row["Tick"] = "False";

                        if (Convert.ToBoolean(row["Tick"]))
                        {
                            dRow["Tick"] = true;
                            dRow["AdjusterName"] = row["UserName"];
                            strAdjustedNo = Convert.ToString(row["AdjustedNumber"]);
                            if (strAdjustedNo == "" || strAdjustedNo == "0")
                                strAdjustedNo = Convert.ToString(row["MultiCompanyNo"]);

                            if (strAdjustedNo == "")
                                strAdjustedNo = "0";

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
                        else
                        {
                            dRow["Tick"] = false;
                            dRow["AdjusterName"] = "";
                        }

                        if (Convert.ToString(row["DebitAmt"]) != "")
                        {
                            dDebitAmt += dAmt = Convert.ToDouble(row["DebitAmt"]);
                            dTotalAmt += dAmt;
                            dRow["DebitAmt"] = dAmt.ToString("N2", MainPage.indianCurancy);
                            if (dTotalAmt > 0)
                                dRow["BalanceAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else if (dTotalAmt < 0)
                                dRow["BalanceAmt"] = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                            else
                                dRow["BalanceAmt"] = "0.00";
                            dRow["CreditAmt"] = "";
                        }
                        else if (Convert.ToString(row["CreditAmt"]) != "")
                        {
                            dCreditAmt += dAmt = Convert.ToDouble(row["CreditAmt"]);
                            dRow["CreditAmt"] = dAmt.ToString("N2", MainPage.indianCurancy);
                            dTotalAmt -= dAmt;
                            if (dTotalAmt > 0)
                                dRow["BalanceAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else if (dTotalAmt < 0)
                                dRow["BalanceAmt"] = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                            else
                                dRow["BalanceAmt"] = "0.00";
                            dRow["DebitAmt"] = "";
                        }
                        _datatable.Rows.Add(dRow);
                    }


                }

                double dADebitAmt = 0, dACreditAmt = 0, dAAmt = 0, dATotalAmt = 0;
                DataRow[] _rows = dt.Select("ID=2");
                foreach(DataRow row in _rows)
                {
                    DataRow dRow = _datatable.NewRow();
                    dRow["BalanceID"] = row["BalanceID"];
                    dRow["Date"] = row["BDate"];
                    dRow["AccountStatus"] = row["AccountStatus"];
                    dRow["Description"] = row["Description"];
                    dRow["CreatedBy"] = row["CreatedBy"];
                    dRow["UpdatedBy"] = row["UpdatedBy"];
                    dRow["journalID"] = row["JournalID"];
                    dRow["ChqDate"] = row["ChqDate"];
                    dRow["ChqStatus"] = row["CHQStatus"];
                    dRow["Tick"] = false;
                    dRow["ID"] = row["ID"];
                    dRow["Remark"] = Convert.ToString(row["Remark"]);

                    if (Convert.ToBoolean(row["Tick"]))
                    {
                        dRow["Tick"] = true;
                        dRow["AdjusterName"] = row["UserName"];
                        strAdjustedNo = Convert.ToString(row["AdjustedNumber"]);
                        if (strAdjustedNo == "" || strAdjustedNo == "0")
                            strAdjustedNo = Convert.ToString(row["MultiCompanyNo"]);

                        if (strAdjustedNo == "")
                            strAdjustedNo = "0";

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
                    else
                    {
                        dRow["Tick"] = false;
                        dRow["AdjusterName"] = "";
                    }

                    if (Convert.ToString(row["DebitAmt"]) != "")
                    {
                        dADebitAmt += dAAmt = Convert.ToDouble(row["DebitAmt"]);
                        dATotalAmt += dAAmt;
                        dRow["DebitAmt"] = dAAmt.ToString("N2", MainPage.indianCurancy);
                        if (dATotalAmt > 0)
                            dRow["BalanceAmt"] = dATotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dATotalAmt < 0)
                            dRow["BalanceAmt"] = Math.Abs(dATotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            dRow["BalanceAmt"] = "0.00";
                        dRow["CreditAmt"] = "";
                    }
                    else if (Convert.ToString(row["CreditAmt"]) != "")
                    {
                        dACreditAmt += dAAmt = Convert.ToDouble(row["CreditAmt"]);
                        dRow["CreditAmt"] = dAAmt.ToString("N2", MainPage.indianCurancy);
                        dATotalAmt -= dAAmt;
                        if (dATotalAmt > 0)
                            dRow["BalanceAmt"] = dATotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dATotalAmt < 0)
                            dRow["BalanceAmt"] = Math.Abs(dATotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            dRow["BalanceAmt"] = "0.00";
                        dRow["DebitAmt"] = "";
                    }
                    _datatable.Rows.Add(dRow);
                }


                BindDataWithGrid(_datatable);

                lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                if (dTotalAmt > 0)
                    lblBalance.Text = lblBalAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalance.Text = lblBalAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalance.Text = lblBalAmount.Text = "0";
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
            _datatable.Columns.Add("BalanceAmt", typeof(String));
            _datatable.Columns.Add("Tick", typeof(Boolean));
            _datatable.Columns.Add("AdjusterName", typeof(String));
            _datatable.Columns.Add("AdjustedNo", typeof(String));
            _datatable.Columns.Add("CreatedBy", typeof(String));
            _datatable.Columns.Add("UpdatedBy", typeof(String));
            _datatable.Columns.Add("ColorIndex", typeof(String));
            _datatable.Columns.Add("JournalID", typeof(String));
            _datatable.Columns.Add("ChqStatus", typeof(String));
            _datatable.Columns.Add("ChqDate", typeof(String));
            _datatable.Columns.Add("Remark", typeof(String));
            _datatable.Columns.Add("ID", typeof(String));
            return _datatable;
        }

        private void BindDataWithGrid(DataTable table)
        {
            int rowIndex = 0, colorIndex = 0;
            string strCIndex = "";

            DataRow[] rows = table.Select("ID<>2");

            if (rows.Length > 0)
                dgrdLedger.Rows.Add(rows.Length);

            foreach (DataRow row in rows)
            {
                dgrdLedger.Rows[rowIndex].Cells["id"].Value = row["BalanceID"];
                dgrdLedger.Rows[rowIndex].Cells["chkCheck"].Value = false;
                dgrdLedger.Rows[rowIndex].Cells["date"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["Date"]));// row["Date"];
                dgrdLedger.Rows[rowIndex].Cells["account"].Value = Convert.ToString(row["AccountStatus"]);
                dgrdLedger.Rows[rowIndex].Cells["desc"].Value = Convert.ToString(row["Description"]);
                dgrdLedger.Rows[rowIndex].Cells["debit"].Value = row["DebitAmt"];
                dgrdLedger.Rows[rowIndex].Cells["credit"].Value = row["CreditAmt"];
                dgrdLedger.Rows[rowIndex].Cells["balance"].Value = row["BalanceAmt"];
                dgrdLedger.Rows[rowIndex].Cells["tick"].Value = row["Tick"];
                dgrdLedger.Rows[rowIndex].Cells["adjustedNo"].Value = row["AdjustedNo"];
                dgrdLedger.Rows[rowIndex].Cells["adjuster"].Value = row["AdjusterName"];
                dgrdLedger.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                dgrdLedger.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                dgrdLedger.Rows[rowIndex].Cells["journalID"].Value = row["JournalID"];
                dgrdLedger.Rows[rowIndex].Cells["ChqDate"].Value = row["ChqDate"];
                dgrdLedger.Rows[rowIndex].Cells["chqStatus"].Value = row["ChqStatus"];
                dgrdLedger.Rows[rowIndex].Cells["remark"].Value = row["remark"];

                strCIndex = Convert.ToString(row["ColorIndex"]);
                if (strCIndex != "")
                {
                    colorIndex = Convert.ToInt32(strCIndex);
                    dgrdLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromName(strColor[colorIndex]);
                }
                //if (Convert.ToString(row["Onaccount"]) == "1")
                //{
                //    dgrdLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                //}
                if (Convert.ToString(row["ChqStatus"]) == "UNCLEAR")
                {
                    dgrdLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                }
                rowIndex++;
            }


            rows = table.Select("ID=2");
            if (rows.Length > 0)
                dgrdLedger.Rows.Add(rows.Length);
            foreach (DataRow row in rows)
            {
                dgrdLedger.Rows[rowIndex].Cells["id"].Value = row["BalanceID"];
                dgrdLedger.Rows[rowIndex].Cells["chkCheck"].Value = false;
                dgrdLedger.Rows[rowIndex].Cells["date"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["Date"]));// row["Date"];
                dgrdLedger.Rows[rowIndex].Cells["account"].Value = Convert.ToString(row["AccountStatus"]);
                dgrdLedger.Rows[rowIndex].Cells["desc"].Value = Convert.ToString(row["Description"]);
                dgrdLedger.Rows[rowIndex].Cells["debit"].Value = row["DebitAmt"];
                dgrdLedger.Rows[rowIndex].Cells["credit"].Value = row["CreditAmt"];
                dgrdLedger.Rows[rowIndex].Cells["balance"].Value = row["BalanceAmt"];
                dgrdLedger.Rows[rowIndex].Cells["tick"].Value = row["Tick"];
                dgrdLedger.Rows[rowIndex].Cells["adjustedNo"].Value = row["AdjustedNo"];
                dgrdLedger.Rows[rowIndex].Cells["adjuster"].Value = row["AdjusterName"];
                dgrdLedger.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                dgrdLedger.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                dgrdLedger.Rows[rowIndex].Cells["journalID"].Value = row["JournalID"];
                dgrdLedger.Rows[rowIndex].Cells["ChqDate"].Value = row["ChqDate"];
                dgrdLedger.Rows[rowIndex].Cells["chqStatus"].Value = row["ChqStatus"];

                dgrdLedger.Rows[rowIndex].Cells["costcentre"].Value = "COST CENTRE";

                strCIndex = Convert.ToString(row["ColorIndex"]);
                if (strCIndex != "")
                {
                    colorIndex = Convert.ToInt32(strCIndex);
                    dgrdLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromName(strColor[colorIndex]);
                }
                
                dgrdLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                rowIndex++;
            }


        }

        private void btnAdjustBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdLedger.Rows.Count > 0)
                {
                    if (CheckAdjustAmountForOpening() && CalculateDebitAndCreditBalance())
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to Adjust these entry ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            AdjustLedgerEntry();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool CalculateDebitAndCreditBalance()
        {
            try
            {
                double dDebitAmt = 0, dCreditAmt = 0;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    if (Convert.ToString(row.Cells["tick"].Value) != "")
                    {
                        if (Convert.ToBoolean(row.Cells["tick"].Value))
                        {
                            if (Convert.ToString(row.Cells["debit"].Value) != "")
                                dDebitAmt += Convert.ToDouble(row.Cells["debit"].Value);
                            else if (Convert.ToString(row.Cells["credit"].Value) != "")
                                dCreditAmt += Convert.ToDouble(row.Cells["credit"].Value);
                        }
                    }
                }

                dDebitAmt = Math.Round(dDebitAmt, 2);
                dCreditAmt = Math.Round(dCreditAmt, 2);

                if (dDebitAmt != dCreditAmt)
                {
                    MessageBox.Show(" Sorry ! Debit and Credit Entry Not matched ! Diffrence is " + (dDebitAmt - dCreditAmt), "Mismatched", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool CheckAdjustAmountForOpening()
        {
            try
            {
                if (Convert.ToString(dgrdLedger.Rows[0].Cells["account"].Value) == "OPENING")
                {
                    if (Convert.ToBoolean(dgrdLedger.Rows[0].Cells["tick"].Value))
                    {
                        if (MainPage.strPreviousDataBase != "")
                        {
                            string[] strFullName = txtParty.Text.Split(' ');
                            if (strFullName.Length > 0)
                            {
                                int count = dba.GetUnAdjustedEntryFromPreviousDataBase(strFullName[0], MainPage.strPreviousDataBase);
                                if (count > 0)
                                {
                                    MessageBox.Show("Please Firstly Adjust in Previous Quarter after that you can adjust Opening Amount ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                                        return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch { }
            return false;
        }
        


        private void AdjustLedgerEntry()
        {
            string strAdjuster="", strAdjustedID = "", strUnAdjustedID = "", strAllAdjustedID = "", strAllUnAdjustedID = "", strQuery = "", strMultiQuery = "", strBalanceID = "", strAdjustedNo = "", strPartyName = "", strMultDataBase = "", strMultiBalanceID = "";
            string[] strParty = txtParty.Text.Split(' ');
            if (strParty.Length > 0)
            {
                strPartyName = strParty[0];
                bool chkStatus;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    strBalanceID = Convert.ToString(row.Cells["id"].Value);
                    strAdjustedNo = Convert.ToString(row.Cells["adjustedNo"].Value);
                    chkStatus = Convert.ToBoolean(row.Cells["tick"].Value);
                    strAdjuster = Convert.ToString(row.Cells["adjuster"].Value);

                    strAdjustedID = strUnAdjustedID = "";
                    if (strBalanceID == "")
                    {
                        if (chkStatus)
                        {
                            DateTime sDate = Convert.ToDateTime(row.Cells["date"].Value);// dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["date"].Value));
                            strAdjustedID = GetOpeningBalanceID(strPartyName, sDate, "False");
                        }
                        else if (strAdjustedNo != "")
                        {
                            DateTime sDate = Convert.ToDateTime(row.Cells["date"].Value); // dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["date"].Value));
                            strUnAdjustedID += GetOpeningBalanceIDUnAdjusment(strPartyName, sDate, "True", ref strMultiBalanceID, ref strMultDataBase);
                        }
                    }
                    else
                    {
                        if (chkStatus)
                        {
                            if (strAdjustedNo == "")
                                strAdjustedID = strBalanceID;
                        }
                        else
                        {
                            if (strAdjustedNo != "")
                            {
                                bool _bUnadjustStatus = dba.ValidateFormAccountUnadjust(_STRCategory);

                                if (strAdjuster == MainPage.strLoginName || strAdjuster=="" || _bUnadjustStatus)
                                    strUnAdjustedID = GetAdjustedIDByBalanceID(strBalanceID, ref strMultiBalanceID, ref strMultDataBase);
                                else
                                {
                                    strUnAdjustedID= strAdjustedID = strAllAdjustedID = strAllUnAdjustedID = strMultiBalanceID = strQuery = strMultiQuery = "";
                                    MessageBox.Show("Sorry ! You are not authorized to unadjust these enties, Please contact to admin.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                        }
                    }

                    if (strAdjustedID != "")
                    {
                        if (strAllAdjustedID == "")
                            strAllAdjustedID = strAdjustedID;
                        else
                            strAllAdjustedID += "," + strAdjustedID;
                    }
                    if (strUnAdjustedID != "")
                    {
                        if (strAllUnAdjustedID == "")
                            strAllUnAdjustedID = strUnAdjustedID;
                        else
                            strAllUnAdjustedID += "," + strUnAdjustedID;
                    }
                }

                if (strAllAdjustedID != "")
                {
                    strQuery += " Update BalanceAmount Set Tick='True' Where BalanceID in (" + strAllAdjustedID + ") ";
                    strQuery += CreateQueryForAdjustedID(strAllAdjustedID);
                }

                if (strAllUnAdjustedID != "")
                {
                    strQuery += " Update BalanceAmount Set Tick='False' Where BalanceID in (" + strAllUnAdjustedID + ") "
                                  + " Delete from AdjustedIds Where BalanceID in (" + strAllUnAdjustedID + ") and DataBaseName='" + MainPage.strDataBaseFile + "' ";
                }
                if (strMultiBalanceID != "" && strMultDataBase != "")
                {
                    strQuery += " Delete from AdjustedIds Where BalanceID in (" + strMultiBalanceID + ") and DataBaseName='" + strMultDataBase + "' ";
                    strMultiQuery = " Update BalanceAmount Set Tick='False' Where BalanceID in (" + strMultiBalanceID + ") ";
                }

                if (strQuery != "" || strMultiQuery != "")
                {
                    int count = dba.AdjustAllDataByLedger(strQuery, strMultiQuery, strMultDataBase);
                    if (count > 0)
                    {
                        MessageBox.Show("Thanks ! Ledger adjusted successfully ! ", "Adjusted Successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnGo.PerformClick();
                    }
                    else
                        MessageBox.Show("Sorry ! Record not adjusted, Please try again ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string CreateQueryForAdjustedID(string strAdjustedID)
        {
            string strQuery = "";
            string[] strAllIDs = strAdjustedID.Split(',');
            if (strAllIDs.Length > 0)
            {
                strQuery = " Declare @ID varchar(20) Select @ID= ISNULL(Max(Cast(AdjustedNumber as int))+1,1) from AdjustedIds ";
                foreach (string strID in strAllIDs)
                {
                    strQuery += " if not exists (Select ID from [dbo].[AdjustedIds] Where BalanceID in (" + strID + ") and DatabaseName in ('" + MainPage.strDataBaseFile + "')) begin  INSERT INTO [dbo].[AdjustedIds]([AdjustedNumber],[BalanceID],[DataBaseName],[MultiCompanyNo],[UserName],[InsertStatus],[UpdateStatus]) VALUES "
                                  + " (@ID," + strID + ",'" + MainPage.strDataBaseFile + "',0,'" + MainPage.strLoginName + "',1,0) end ";
                }
            }
            return strQuery;
        }

        private string GetOpeningBalanceID(string strPartyName, DateTime sDate, string strStatus)
        {
            string strBalanceID = "";
            DataTable dt = dba.GetDataTable("Select BalanceID from BalanceAmount Where PartyName='" + strPartyName + "' and Date<'" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and Tick='" + strStatus + "' ");
            foreach (DataRow row in dt.Rows)
            {
                if (strBalanceID == "")
                    strBalanceID = Convert.ToString(row["BalanceID"]);
                else
                    strBalanceID += "," + row["BalanceID"];
            }
            return strBalanceID;
        }

        private string GetOpeningBalanceIDUnAdjusment(string strPartyName, DateTime sDate, string strStatus, ref string strMultiID, ref string strMultiDB)
        {
            string strBalanceID = "", strQuery = "", strDataBase = "", strMultiQID = "", strMDB = "";
            strQuery = "Select BalanceID,DataBaseName from AdjustedIDs Where (AdjustedNumber!=0 and AdjustedNumber in (Select AdjustedNumber from AdjustedIds Where DataBaseName='" + MainPage.strDataBaseFile + "' and BalanceID in (Select BalanceID from BalanceAmount Where PartyName='" + strPartyName + "' and Date<'" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and Tick='True' ))) "
                     + " OR (MultiCompanyNo!=0 and MultiCompanyNo in (Select MultiCompanyNo from AdjustedIds Where DataBaseName='" + MainPage.strDataBaseFile + "' and BalanceID in (Select BalanceID from BalanceAmount Where PartyName='" + strPartyName + "' and Date<'" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and Tick='True' ))) ";

            DataTable dt = dba.GetDataTable(strQuery);
            foreach (DataRow row in dt.Rows)
            {
                strDataBase = Convert.ToString(row["DataBaseName"]);
                if (MainPage.strDataBaseFile != strDataBase)
                {
                    if (strMultiQID == "")
                        strMultiQID = Convert.ToString(row["BalanceID"]);
                    else
                        strMultiQID += "," + row["BalanceID"];
                    strMDB = strDataBase;
                }
                else
                {
                    if (strBalanceID == "")
                        strBalanceID = Convert.ToString(row["BalanceID"]);
                    else
                        strBalanceID += "," + row["BalanceID"];
                }
            }
            if (strMultiQID != "")
            {
                if (strMultiID == "")
                    strMultiID = strMultiQID;
                else
                    strMultiID += "," + strMultiQID;
                if (strMultiDB == "")
                    strMultiDB = strMDB;
                //else
                //  strMultiDB += "," + strMDB;
            }

            return strBalanceID;
        }

        private string GetAdjustedIDByBalanceID(string strBID, ref string strMultiID, ref string strMultiDB)
        {
            string strBalanceID = "", strQuery = "", strDataBase = "", strMultiQID = "", strMDB = "";
            strQuery = "Select BalanceID,DataBaseName from AdjustedIDs Where (AdjustedNumber!=0 and AdjustedNumber in (Select AdjustedNumber from AdjustedIds Where DataBaseName='" + MainPage.strDataBaseFile + "' and BalanceID in (" + strBID + ")) "
                     + " OR (MultiCompanyNo!=0 and MultiCompanyNo in (Select MultiCompanyNo from AdjustedIds Where DataBaseName='" + MainPage.strDataBaseFile + "' and BalanceID in (" + strBID + ")))) ";

            DataTable dt = dba.GetDataTable(strQuery);
            foreach (DataRow row in dt.Rows)
            {
                strDataBase = Convert.ToString(row["DataBaseName"]);
                if (MainPage.strDataBaseFile != strDataBase)
                {
                    if (strMultiQID == "")
                        strMultiQID = Convert.ToString(row["BalanceID"]);
                    else
                        strMultiQID += "," + row["BalanceID"];
                    strMDB = strDataBase;
                }
                else
                {
                    if (strBalanceID == "")
                        strBalanceID = Convert.ToString(row["BalanceID"]);
                    else
                        strBalanceID += "," + row["BalanceID"];
                }
            }
            if (strMultiQID != "")
            {
                if (strMultiID == "")
                    strMultiID = strMultiQID;
                else
                    strMultiID += "," + strMultiQID;
                if (strMultiDB == "")
                    strMultiDB = strMDB;
                //  else
                //    strMultiDB += "," + strMDB;
            }
            if (strBID != "" && strBalanceID == "" && strMultiID == "" && strMultiDB == "" && MainPage.strUserRole.Contains("ADMIN"))
                strBalanceID = strBID;

            return strBalanceID;
        }

        private void LedgerAccount_Load(object sender, EventArgs e)
        {
            try
            {                
                btnExport.Enabled = MainPage.mymainObject.bExport;
                btnAdjustBill.Enabled = (MainPage.mymainObject.bAdjustUnadjustAccount && !btnSelectCompany.Enabled) ? true : false;

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
                }

                if (btnSelectCompany.Enabled)
                    lblAccountID.Visible = txtAccountID.Visible = !MainPage._bTaxStatus;
            }
            catch
            { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgrdLedger.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete these items ?", "Confirmation For Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DeleteLedgerEntry();
                }
            }
        }

        #region Delete Ledger Entry

        private void DeleteLedgerEntry()
        {
            try
            {
                string strID = "", strAccount = "", strBalanceID = "", strJournalID = "", strNetBalanceID = "", strNetJournalID = "";
                DateTime sDate;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkCheck"].Value))
                    {
                        if (Convert.ToString(row.Cells["adjustedNo"].Value) == "")
                        {
                            strAccount = Convert.ToString(row.Cells["account"].Value);
                            if (strAccount != "OPENING" && strAccount != "SALES A/C" && strAccount != "PURCHASE A/C")
                            {
                                strID = Convert.ToString(row.Cells["id"].Value);
                                sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["date"].Value));
                                if (ValidateDate(sDate))
                                {
                                    if (strID != "")
                                    {
                                        bool insertStatus = true;
                                        string[] strVoucherCode = strAccount.Split('|');
                                        if (strAccount.Contains("|") && strVoucherCode.Length > 1)
                                        {
                                            strAccount = strVoucherCode[0].Trim();
                                            if (!ValidatePartyAndStatus(strID, strAccount, ref insertStatus))
                                            {
                                                if (ValidateAdjustementOfBalanceID(strVoucherCode[1].Trim(), insertStatus))
                                                {
                                                    if (strBalanceID == "")
                                                        strBalanceID = strID;
                                                    else
                                                        strBalanceID += "," + strID;
                                                    if (!insertStatus)
                                                    {
                                                        if (strNetBalanceID == "")
                                                            strNetBalanceID = "'" + strVoucherCode[1].Trim() + "'";
                                                        else
                                                            strNetBalanceID += ",'" + strVoucherCode[1].Trim() + "'";
                                                    }
                                                }
                                            }
                                            else
                                                MessageBox.Show("Transaction has been locked on Account : " + strAccount + "/" + txtParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                        else
                                        {
                                            if (!ValidatePartyAndStatus(strID, strAccount, ref insertStatus))
                                            {
                                                string strJVCode = "";
                                                if (ValidateAdjustementOfJournalID(strID, insertStatus, ref strJVCode))
                                                {
                                                    if (strJournalID == "")
                                                        strJournalID = strID;
                                                    else
                                                        strJournalID += "," + strID;

                                                    if (!insertStatus && strJVCode != "")
                                                    {
                                                        if (strNetJournalID == "")
                                                            strNetJournalID = "'" + strJVCode + "'";
                                                        else
                                                            strNetJournalID += ",'" + strJVCode + "'";
                                                    }
                                                }
                                            }
                                            else
                                                MessageBox.Show("Transaction has been locked on Account : " + strAccount + "/" + txtParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Adjusted entry can't be adjusted ", "Amount Adjusted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                if (strBalanceID != "" || strJournalID != "")
                {
                    int cout = DeleteFromBalanceAmount(strBalanceID, strJournalID, strNetBalanceID, strNetJournalID);
                    if (cout > 0)
                    {
                        MessageBox.Show("Thank you ! Entry deleted successfully ! ", "Entry Deleted", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        GetCurrentQuarterDetails();
                    }
                    else
                        MessageBox.Show("Sorry ! Problem may occured, These entry may be adjusted by some other entry !", "Please Ununadjust Adjusted Entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Sorry ! No entry found for deletetion", "Entry not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Deletion of Ledger Entry in Ledger Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool ValidateAdjustementOfBalanceID(string strVCode, bool iStatus)
        {
            if (!iStatus)
            {
                iStatus = DataBaseAccess.CheckAmountAdjustmentByVCode(strVCode);
            }
            return iStatus;
        }

        private bool ValidateAdjustementOfJournalID(string strID, bool iStatus, ref string strVCode)
        {
            if (!iStatus)
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select JournalID from BalanceAmount Where JournalID!='0' and BalanceID=" + strID);
                strVCode = Convert.ToString(objValue);
                if (strVCode != "")
                    iStatus = DataBaseAccess.CheckAmountAdjustmentByJournalID(strVCode);
            }
            return iStatus;
        }

        private bool ValidatePartyAndStatus(string strID, string strAccount, ref bool insertStatus)
        {
            bool tStatus = false;
            DataTable dt = dba.GetDataTable("Select TransactionLock,(Select InsertStatus from BalanceAmount Where BalanceID=" + strID + ") InsertStatus from SupplierMaster Where GroupName!='SUB PARTY' and (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) in ('" + strAccount + "','" + txtParty.Text + "') ");
            if (dt.Rows.Count > 0)
            {
                tStatus = Convert.ToBoolean(dt.Rows[0]["TransactionLock"]);
                insertStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);
                if (!tStatus && dt.Rows.Count > 1)
                    tStatus = Convert.ToBoolean(dt.Rows[1]["TransactionLock"]);
            }
            return tStatus;
        }

        private int DeleteFromBalanceAmount(string strBalanceID, string strJournalID, string strNetBalanceID, string strNetJID)
        {
            int count = 0;
            string strQuery = "", strNetQuery = "";
            if (strBalanceID != "")
                strQuery = " Delete from BalanceAmount Where VoucherCode!='' and (VoucherCode+' '+CAST(VoucherNo as varchar)) Not in (Select (VoucherCode+' '+CAST(VoucherNo as varchar)) from BalanceAmount Where Tick='True') and (VoucherCode+' '+CAST(VoucherNo as varchar)) in (Select (VoucherCode+' '+CAST(VoucherNo as varchar)) from BalanceAmount Where BalanceID in (" + strBalanceID + "))";
            if (strJournalID != "")
            {
                strQuery += " Delete from BalanceAmount Where JournalID!='0' and BalanceID in (" + strJournalID + ") and JournalID not in (Select JournalID from BalanceAmount Where JournalID!='0' and Tick='True') "
                              + " Delete from JournalAccount Where (VoucherCode+' '+CAST(VoucherNo as varchar)) in (Select JournalID from BalanceAmount Where JournalID!='0' and BalanceID in (" + strJournalID + ")) and (VoucherCode+' '+CAST(VoucherNo as varchar)) not in (Select JournalID from BalanceAmount Where JournalID!='0' and Tick='True') ";
            }
            if (strNetBalanceID != "")
                strNetQuery = " Delete from BalanceAmount Where (VoucherCode+' '+CAST(VoucherNo as varchar)) in (" + strNetBalanceID + ") ";
            if (strNetJID != "")
                strNetQuery = " Delete from BalanceAmount Where JournalID!='0' and JournalID in (" + strNetJID + ") ";

            count = dba.ExecuteMyQuery(strQuery);
            if (count > 0 && strNetQuery != "")
                DataBaseAccess.CreateDeleteQuery(strNetQuery);
            return count;
        }

        private bool ValidateDate(DateTime date)
        {
            if (!(MainPage.mymainObject.bBackDayEntry))
            {
                if (Convert.ToDateTime(date.AddDays(3).ToString("MM/dd/yyyy")) < MainPage.currentDate)
                {
                    MessageBox.Show("Back Date Entry is not Allowed  !  Please Contact to Administrator ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            return true;
        }

        #endregion

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = true;
            txtAmount.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            SearchRecord();
            btnSearch.Enabled = true;
        }

        private void SearchRecord()
        {
            try
            {
                if (txtParty.Text == "")
                {
                    MessageBox.Show("Sorry ! Party name can't be blank ! ", "Party name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParty.Focus();
                }
                else if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if (chkChqDate.Checked && (txtClearFromDate.Text.Length != 10 || txtClearToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else
                {
                    dgrdLedger.Rows.Clear();
                    lblBalAmount.Text = lblBalance.Text = lblCredit.Text = lblDebit.Text = "0.00";
                    chkTickAll.Checked = chkAll.Checked = false;
                    if(txtParty.Text.Contains("BANK") && _bPrevilegeAccount && !MainPage.mymainObject.bShowBankLedger)
                    {
                        MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtParty.Focus();
                    }
                    else if (_bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                    {
                        MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtParty.Focus();
                    }
                    else
                    {
                        if (btnSelectCompany.Enabled)
                            GetMultiQuarterDetails();
                        else
                            GetCurrentQuarterDetails();
                    }
                    panelCompany.Visible = panelSearch.Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                    GetMultiCompanyData(strPath,"CURRENT");
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

        private void GetMultiCompanyData(string strPath,string strDataType)
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
                    if (strDataType=="OLD")
                    {
                        dt = SearchDataOther.GetDataTable("Select ('A'+(CASE WHEN CompanyName Like('%STYLO%') then '0' else '' end)+CAST(CompanyID as varchar)) as CCode,CompanyName,Convert(varchar,Fin_Y_Starts,103) SDate,Convert(varchar,Fin_Y_Ends,103)EDate from Company", "A" + strDBName);
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
                string strFirstQuery = "", strPartyID = "", strOtherQuery = "", strChqStatus = "", strInvQuery = "", strSubQuery = CreateQuery(ref strChqStatus, ref strInvQuery, ref strPartyID,true), strOpeningQuery = "", strCompanyCode = "";
                
                strOpeningQuery = " Select 0 as ID,'' as BalanceID,Date,AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,'' as CHQStatus,NULL as ChqDate, BA.Date as BillDate,'' as Remark from BalanceAmount BA OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID OUTER APPLY (Select Date as BillDate)PR  Where AccountStatus='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " Union All  ";

                DataTable table = null;//, dtLastInt = null;

                strFirstQuery += "Select BalanceID,Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,AdjustedNumber,MultiCompanyNo,UserName,JournalID,CHQStatus,0 Onaccount,ISNULL(ChqDate,'')ChqDate,(CASE WHEN ChqDate is NULL or ChqDate='' then Date else Convert(datetime,ChqDate,103) end)ChequeDate,ID,Remark from ( "
                              + strOpeningQuery
                              + " Select 1 as ID,'' as BalanceID, Date,(CASE When VoucherCode!='' then AccountStatus+' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else AccountStatus end) AccountStatus,(CASE WHEN AccountStatus='SALE SERVICE' then (Description+'|'+SDescription) WHEN ISNULL(CostCentreAccountID,'') !='' then (dbo.GetFullName(CostCentreAccountID)) else Description end) Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, "
                              + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN (VoucherCode!='' and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN (VoucherCode!='' and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,Convert(varchar,ChqDate,103) ChqDate,BillDate,Remark from BalanceAmount BA OUTER APPLY (Select Top 1 (ItemName+' : '+ SAC) as SDescription from SaleServiceDetails Where (BillCode+' '+CAST(BillNo as varchar))=Description and AccountStatus='SALE SERVICE') SSD OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID " + strInvQuery + " Left join (Select BillNo,Remark from (Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseBook Where PurchasePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SalesBook Where SalePartyID='" + strPartyID+ "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SaleReturn Where SalePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseReturn Where PurchasePartyID='" + strPartyID + "')_Sales)Sales on BA.Description=Sales.BillNo and BA.AccountStatus in ('PURCHASE A/C','SALES A/C') Where AccountStatus!='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " UNION ALL "
                              + " Select 2 as ID,BA.BalanceID,Date,((AccountID+' '+Name)+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar,ChqDate,103) ChqDate,'' as BillDate,'' as Remark from BalanceAmount BA OUTER APPLY (Select TOP 1  AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID CROSS APPLY (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and SM.TINNumber='COST CENTRE') SM Where AccountID!='' " + strSubQuery.Replace(" AccountID=", " CostCentreAccountID=") + " ) Balance Where ID>=0 " + strChqStatus + " Order By ID,Date";
                              ///+ " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LedgerAccessDetails]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[LedgerAccessDetails]([ID] [bigint] IDENTITY(1,1) NOT NULL,[AccountType] [nvarchar](250) NULL,[AccountID] [nvarchar](250) NULL,[UserName] [nvarchar](250) NULL,[ComputerName] [nvarchar](250) NULL,[Date] [datetime] NULL,[InsertStatus] [bit] NULL,[UpdateStatus] [bit] NULL) ON [PRIMARY] end "
                             // + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('LEDGER','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName + "/" + Environment.UserName).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";

                strOtherQuery += "Select BalanceID,Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,AdjustedNumber,MultiCompanyNo,UserName,JournalID,CHQStatus, 0 as Onaccount,ISNULL(ChqDate,'')ChqDate,(CASE WHEN ChqDate is NULL or ChqDate='' then Date else Convert(datetime,ChqDate,103) end)ChequeDate,ID,Remark from ( "
                             + " Select 1 as ID,'' as BalanceID, Date,(CASE When VoucherCode!='' then AccountStatus+' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else AccountStatus end)AccountStatus,(CASE WHEN AccountStatus='SALE SERVICE' then (Description+'|'+SDescription) WHEN ISNULL(CostCentreAccountID,'') !='' then (dbo.GetFullName(CostCentreAccountID)) else Description end) Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, "
                             + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,Convert(varchar,ChqDate,103) ChqDate,BillDate,Remark from BalanceAmount BA OUTER APPLY (Select Top 1 (ItemName+' : '+ SAC) as SDescription from SaleServiceDetails Where (BillCode+' '+CAST(BillNo as varchar))=Description and AccountStatus='SALE SERVICE') SSD OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID " + strInvQuery + " Left join (Select BillNo,Remark from (Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseBook Where PurchasePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SalesBook Where SalePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from SaleReturn Where SalePartyID='" + strPartyID + "' UNION ALL Select (BillCode+' '+Cast(BillNo as varchar))BillNo, Remark from PurchaseReturn Where PurchasePartyID='" + strPartyID + "')_Sales)Sales on BA.Description=Sales.BillNo and BA.AccountStatus in ('PURCHASE A/C','SALES A/C') Where AccountStatus!='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " UNION ALL "
                             + " Select 2 as ID,BA.BalanceID,Date,((AccountID+' '+Name)+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar,ChqDate,103) ChqDate,'' as BillDate,'' as Remark from BalanceAmount BA OUTER APPLY (Select TOP 1  AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID CROSS APPLY (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and SM.TINNumber='COST CENTRE') SM Where AccountID!='' " + strSubQuery.Replace(" AccountID=", " CostCentreAccountID=") + ") Balance  Where ID>=0 " + strChqStatus + " Order By ID,Date" ;
                           //  + " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LedgerAccessDetails]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[LedgerAccessDetails]([ID] [bigint] IDENTITY(1,1) NOT NULL,[AccountType] [nvarchar](250) NULL,[AccountID] [nvarchar](250) NULL,[UserName] [nvarchar](250) NULL,[ComputerName] [nvarchar](250) NULL,[Date] [datetime] NULL,[InsertStatus] [bit] NULL,[UpdateStatus] [bit] NULL) ON [PRIMARY] end "
                            // + " INSERT INTO [dbo].[LedgerAccessDetails] ([AccountType],[AccountID],[UserName],[ComputerName],[Date],[InsertStatus],[UpdateStatus]) VALUES ('LEDGER','" + strPartyID + "','" + MainPage.strLoginName + "','" + (Environment.MachineName + "/" + Environment.UserName).ToUpper() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),1,0) ";


                int rowPCount = 0,rowKCount=0;
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
                                    dt = dba.GetMultiQuarterDataTable(strFirstQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);
                                else
                                    dt = dba.GetMultiQuarterDataTable(strOtherQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);

                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);
                                rowPCount++;
                            }
                            else
                            {
                                if (rowKCount == 0)
                                    dt = SearchDataOther.GetDataTable(strFirstQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);
                                else
                                    dt = SearchDataOther.GetDataTable(strOtherQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);

                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);
                                rowKCount++;
                            }
                        }
                    }
                }

                if (table != null)
                    SetRecordWithDataTable(table);               
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        private void dgrdLedger_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3 && e.RowIndex >= 0)
                {
                    ShowDetails();
                }
            }
            catch
            {
            }
        }

        private void ShowDetails()
        {
            DateTime ledgerDate = Convert.ToDateTime(dgrdLedger.CurrentRow.Cells["date"].Value);// dba.ConvertDateInExactFormat(Convert.ToString(dgrdLedger.CurrentRow.Cells["date"].Value));
            if (ledgerDate >= MainPage.startFinDate && ledgerDate < MainPage.endFinDate)
            {
                string strAccount = Convert.ToString(dgrdLedger.CurrentRow.Cells["account"].Value).ToUpper();
                if (strAccount == "PURCHASE A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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

                            if (MainPage.strSoftwareType == "RETAIL")
                            {
                                PurchaseBook_Retail_Merge objPurchase = new PurchaseBook_Retail_Merge(strNumber[0], strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                            else if (MainPage._bCustomPurchase)
                            {
                                PurchaseBook_Retail_Custom objPurchase = new PurchaseBook_Retail_Custom(strNumber[0], strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
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
                }
                else if (strAccount == "SALES A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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
                                if (Screen.PrimaryScreen.Bounds.Width < 1100)
                                {
                                    SaleBook_Retail_POS objSaleBill_Retail = new SaleBook_Retail_POS(strNumber[0], strNumber[1]);
                                    objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objSaleBill_Retail.ShowInTaskbar = true;
                                    objSaleBill_Retail.Show();
                                }
                                else
                                {
                                    SaleBook_Retail objSaleBill_Retail = new SaleBook_Retail(strNumber[0], strNumber[1]);
                                    objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objSaleBill_Retail.ShowInTaskbar = true;
                                    objSaleBill_Retail.Show();
                                }

                                //SaleBook_Retail objSale = new SaleBook_Retail(strNumber[0], strNumber[1]);
                                //objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                //objSale.ShowInTaskbar = true;
                                //objSale.Show();
                            }
                            else
                            {
                                if (MainPage._bCustomPurchase)
                                {
                                    SaleBook_Retail_Custom objSale = new SaleBook_Retail_Custom(strNumber[0], strNumber[1]);
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
                }
                else if (strAccount == "SALE SERVICE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split('|');
                    strNumber = strNumber[0].Split(' ');
                    if (strNumber.Length > 1)
                    {
                        SaleServiceBook objSale = new SaleServiceBook(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }

                }
                else if (strAccount == "SALE RETURN" || strAccount == "PURCHASE RETURN")
                {
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        dba.ShowTransactionBook(strAccount, strNumber[0], strNumber[1]);
                    }

                }
                else if (strAccount == "CREDIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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
                    string strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["desc"].Value);
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
                            if (strName[0].Trim() == "SALE SERVICE")
                            {
                                if (strVoucher.Length > 1)
                                {
                                    SaleServiceBook objSale = new SaleServiceBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objSale.ShowInTaskbar = true;
                                    objSale.Show();
                                }
                            }
                            else if (strName[0].Trim() == "JOURNAL A/C")
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
                        string strJournal = Convert.ToString(dgrdLedger.CurrentRow.Cells["journalID"].Value);
                        string[] strVoucher = strJournal.Split(' ');
                        if (strVoucher.Length > 0)
                        {
                            JournalEntry_New objJournal = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                            objJournal.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                            objJournal.ShowInTaskbar = true;
                            objJournal.Show();
                        }
                    }
                }
            }
        }

        private void dgrdLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdLedger.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdLedger.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdLedger.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdLedger.CurrentCell.ColumnIndex == 3 && dgrdLedger.CurrentCell.RowIndex >= 0)
                    {
                        ShowDetails();
                    }
                }
            }
            catch
            {
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintLedger();
        }

        private void PrintLedger()
        {
            try
            {
                if (dgrdLedger.Rows.Count > 0)
                {
                    DataTable _dtAdvance = null;
                    DataTable dt = CreateDataTableForPrint(ref _dtAdvance);
                    //if (rdoNo.Checked)
                    //{
                    //    Reporting.LedgerReport_Remark report = new SSS.Reporting.LedgerReport_Remark();
                    //    report.SetDataSource(dt);
                    //    report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                    //    report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    //    report.PrintToPrinter(1, false, 0, 0);
                    //}
                    //else
                    //{
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.LedgerReport_Remark report = new SSS.Reporting.LedgerReport_Remark();
                        report.SetDataSource(dt);
                        report.Subreports[0].SetDataSource(_dtAdvance);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(report);
                        else
                        {
                            report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            report.PrintToPrinter(1, false, 0, 0);
                        }
                        report.Close();
                        report.Dispose();
                    }
                    //}
                    if (index > -1)
                    {
                        if (index == strAllParty.Length)
                        {
                            btnPrint.Enabled = false;
                        }
                        else
                        {
                            BindMultiLedgerAccount();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }

        private void BindMultiLedgerAccount()
        {
            if (strAllParty.Length > 0 && index < strPartyStatus.Length)
            {
                txtParty.Text = strAllParty[index];
                GetRelatedpartyDetails();
                if (_bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                {
                    MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearRecord();
                }
                else
                {
                    SetStatus();                  
                    if (btnSelectCompany.Enabled)
                        GetMultiQuarterDetails();
                    else
                        GetCurrentQuarterDetails();
                }
                index++;
            }
            else
                this.Close();
        }

        private void SetStatus()
        {
            if (strPartyStatus[index] == "All")
                rdoAll.Checked = true;
            else if (strPartyStatus[index] == "True")
                rdoTick.Checked = true;
            else if (strPartyStatus[index] == "False")
                rdoUnTick.Checked = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (dgrdLedger.Rows.Count > 0)
            {                
                btnPreview.Enabled = false;
                DataTable _dtTable = null;
                DataTable dt = CreateDataTableForPrint(ref _dtTable);
                if (dt.Rows.Count > 0)
                {
                    if(_dtTable == null)
                        _dtTable = new DataTable();
                    SSS.Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Ledger Report Preview");
                    SSS.Reporting.LedgerReport_Remark objReport = new Reporting.LedgerReport_Remark();
                    objReport.SetDataSource(dt);
                    objReport.Subreports[0].SetDataSource(_dtTable);
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();

                    objReport.Close();
                    objReport.Dispose();

                }               
                btnPreview.Enabled = true;
            }
            else
            {
                MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataTable CreateDataTableForPrint(ref DataTable _dtTable)
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
                myDataTable.Columns.Add("CHQAccountNo", typeof(String));
                myDataTable.Columns.Add("FirmName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));

                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));


                // string[] strPartyDetail = dba.GetPartyAddress(txtParty.Text);

                string strNumeric = "Zero", strDate = "";
                if (lblBalance.Text.Contains("Cr") || lblBalance.Text.Contains("Dr"))
                    strNumeric = currency.changeCurrencyToWords(Convert.ToDouble(lblBalance.Text.Substring(0, lblBalance.Text.Length - 3)));
                if (chkDate.Checked && txtFromDate.Text != "" && txtToDate.Text != "")
                    strDate = "Date Period : From " + txtFromDate.Text + " To " + txtToDate.Text;
                else
                    strDate = "Date Period : From " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                _dtTable = myDataTable.Clone();

                double dADebitAmt = 0, dACreditAmt = 0;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    try
                    {
                        if (Convert.ToString(row.Cells["costcentre"].Value) == "")
                        {
                            DataRow dRow = myDataTable.NewRow();
                            dRow["CompanyName"] = MainPage.strPrintComapanyName;
                            dRow["PartyName"] = txtParty.Text;  
                            dRow["DatePeriod"] = strDate;
                            dRow["Date"] = Convert.ToDateTime(row.Cells["date"].Value).ToString("dd/MM/yyyy");
                            dRow["Account"] = row.Cells["account"].Value;
                            dRow["DebitAmt"] = row.Cells["debit"].Value;
                            dRow["CreditAmt"] = row.Cells["credit"].Value;
                            dRow["Balance"] = row.Cells["balance"].Value;
                            dRow["Description"] = row.Cells["desc"].Value;
                            dRow["OnAccount"] = "0";// row.Cells["onaccountStatus"].Value;
                            dRow["PostOffice"] = row.Cells["remark"].Value;
                            
                            dRow["TotalDebit"] = lblDebit.Text;
                            dRow["TotalCredit"] = lblCredit.Text;
                            dRow["TotalBalance"] = lblBalAmount.Text;
                            dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                            if (lblBalance.Text.Contains("Cr"))
                            {
                                dRow["AmountInWord"] = strNumeric + " Credit";
                            }
                            else if (lblBalance.Text.Contains("Dr"))
                            {
                                dRow["AmountInWord"] = strNumeric + " Debit";
                            }
                            else
                                dRow["AmountInWord"] = strNumeric;

                            myDataTable.Rows.Add(dRow);
                        }
                        else
                        {
                            DataRow dRow = _dtTable.NewRow();
                            dRow["CompanyName"] = MainPage.strPrintComapanyName;
                            dRow["PartyName"] = "Advance Details";

                            dADebitAmt += dba.ConvertObjectToDouble(row.Cells["debit"].Value);
                            dACreditAmt += dba.ConvertObjectToDouble(row.Cells["credit"].Value);

                            dRow["DatePeriod"] = strDate;
                            dRow["Date"] = Convert.ToDateTime(row.Cells["date"].Value).ToString("dd/MM/yyyy");
                            dRow["Account"] = row.Cells["account"].Value;
                            dRow["DebitAmt"] = row.Cells["debit"].Value;
                            dRow["CreditAmt"] = row.Cells["credit"].Value;
                            dRow["Balance"] = row.Cells["balance"].Value;
                            dRow["Description"] = row.Cells["desc"].Value;
                            dRow["PostOffice"] = row.Cells["remark"].Value;
                            dRow["OnAccount"] = "0";

                            dRow["TotalDebit"] = dADebitAmt.ToString("N2", MainPage.indianCurancy);
                            dRow["TotalCredit"] = dACreditAmt.ToString("N2", MainPage.indianCurancy);
                            double _dAmt = dADebitAmt - dACreditAmt;
                            if (_dAmt >= 0)
                                dRow["TotalBalance"] = _dAmt.ToString("N2", MainPage.indianCurancy)+" Dr";
                            else
                                dRow["TotalBalance"] = Math.Abs(_dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";

                            dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                            _dtTable.Rows.Add(dRow);
                        }
                        //}
                    }
                    catch
                    {
                    }
                }
                if (_dtTable.Rows.Count > 0 && myDataTable.Rows.Count == 0)
                {
                    DataRow dRow = myDataTable.NewRow();
                    dRow["CompanyName"] = MainPage.strPrintComapanyName;
                    dRow["PartyName"] = txtParty.Text; 
                    dRow["DatePeriod"] = strDate;                  

                    myDataTable.Rows.Add(dRow);
                }

                if (myDataTable.Rows.Count > 0)
                {
                    DataTable dt = dba.GetDataTable("Select SM.GroupName, (SM.Address + ', '+SM.Station+', '+SM.State+'-'+SM.PinCode)Address,(SM.MobileNo+ ' '+SM.PhoneNo)PhoneNo,SM.AccountNo,CD.* from SupplierMaster SM Outer Apply (Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber,CD.SignatureImage,CD.HeaderImage,CD.BrandLogo from CompanyDetails CD  Order by CD.ID asc) CD Where (SM.AreaCode+SM.AccountNo+' '+SM.Name)='" + txtParty.Text + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow _row = dt.Rows[0];
                        myDataTable.Rows[0]["Address"] = _row["Address"];
                        myDataTable.Rows[0]["PhoneNo"] = _row["PhoneNo"];
                        myDataTable.Rows[0]["FirmName"] = _row["GroupName"];

                        myDataTable.Rows[0]["HeaderImage"] = _row["HeaderImage"];
                        myDataTable.Rows[0]["BrandLogo"] = _row["BrandLogo"];
                        myDataTable.Rows[0]["SignatureImage"] = _row["SignatureImage"];

                        myDataTable.Rows[0]["CompanyAddress"] = _row["CompanyAddress"];
                        myDataTable.Rows[0]["CompanyEmail"] = _row["CompanyPhoneNo"];

                        if (Convert.ToString(_row["GSTNo"])!="")
                        myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                        if (Convert.ToString(_row["CINNumber"]) != "")
                            myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];

                        if (Convert.ToString(_row["GroupName"]) == "SUNDRY DEBTORS" && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            myDataTable.Rows[0]["BankName"] = "ICICI BANK";
                            myDataTable.Rows[0]["BranchName"] = "DELHI";
                            myDataTable.Rows[0]["AccountNo"] = "SASUSP" + dba.ConvertObjectToDouble(_row["AccountNo"]).ToString("000000");
                            myDataTable.Rows[0]["IFSCCode"] = "ICIC0000106";
                        }
                        else
                        {
                            myDataTable.Rows[0]["BankName"] = myDataTable.Rows[0]["BranchName"] = myDataTable.Rows[0]["AccountNo"] = myDataTable.Rows[0]["IFSCCode"] = "N/A";
                        }

                        if (_dtTable.Rows.Count > 0)
                            myDataTable.Rows[myDataTable.Rows.Count-1]["CHQAccountNo"] = "Adv";
                    }
                    else
                        myDataTable.Rows.Clear();
                  
                }
            }
            catch
            {
            }
            return myDataTable;
        }


        private void btnReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("ARE YOU SURE YOU  WANT TO PRINT CASH RECEIPT  ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    Reporting.CashReceiptReport report = new SSS.Reporting.CashReceiptReport();
                    string strValue="1";
                    if (MainPage._PrintWithDialog)
                    {
                        try
                        {
                            PrintDialog printDlg = new PrintDialog();
                            printDlg.AllowSelection = false;
                            printDlg.AllowSomePages = false;
                            printDlg.AllowCurrentPage = false;

                            var MaxPages = report.FormatEngine.GetLastPageNumber(new CrystalDecisions.Shared.ReportPageRequestContext());
                            printDlg.PrinterSettings.MaximumPage = MaxPages;
                            printDlg.PrinterSettings.Copies = 2;

                            report.PrintOptions.CopyTo(printDlg.PrinterSettings, printDlg.PrinterSettings.DefaultPageSettings);

                            if (printDlg.ShowDialog() == DialogResult.OK)
                            {
                                PS = printDlg.PrinterSettings;
                                report.PrintOptions.PrinterName = PS.PrinterName;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Sorry ! Error occured while printing, that is _ " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT, 1 OR 2 ! ", "Number of Copies", "2", 400, 300);
                    }

                    int count = 0;
                    foreach (DataGridViewRow rows in dgrdLedger.Rows)
                    {
                        if (Convert.ToBoolean(rows.Cells["chkCheck"].Value))
                        {
                            string strAccountName = Convert.ToString(rows.Cells["account"].Value);
                            if (strAccountName != "OPENING" && strAccountName != "SALES A/C" && strAccountName != "PURCHASE A/C" && strAccountName != "SALE RETURN" && strAccountName != "PURCHASE RETURN" && strAccountName != "SALE SERVICE")
                            {
                                string[] strVoucherCode = strAccountName.Split('|');
                                if (strAccountName.Contains("|") && strVoucherCode.Length > 1)
                                {
                                    DataTable dt = CreateReceiptDataTable(rows, strVoucherCode);
                                    if (dt.Rows.Count > 0)
                                    {
                                        //Reporting.CashReceiptReport report = new SSS.Reporting.CashReceiptReport();
                                        report.SetDataSource(dt);
                                        if (MainPage._PrintWithDialog)
                                        {
                                            if (report.PrintOptions.PrinterName == "")
                                            {
                                                MessageBox.Show("Sorry ! Somthing went wrong,.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                report.PrintOptions.PrinterName = "Microsoft Print to PDF";
                                                return;
                                            }
                                            report.PrintToPrinter(1, PS.Collate, PS.FromPage, PS.ToPage);
                                            if (PS.Copies > 1)
                                            {
                                                dt.Clear();
                                                dt = CreateOfficeReceiptDataTable(rows, strVoucherCode);
                                                report.SetDataSource(dt);
                                                report.PrintToPrinter(1, PS.Collate, PS.FromPage, PS.ToPage);
                                            }
                                        }
                                        else
                                        {
                                            report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                                            report.PrintToPrinter(1, false, 0, 1);
                                            if (strValue == "2")
                                            {
                                                dt.Clear();
                                                dt = CreateOfficeReceiptDataTable(rows, strVoucherCode);
                                                report.SetDataSource(dt);
                                                report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                                                report.PrintToPrinter(1, false, 0, 1);
                                            }
                                        }
                                        report.Close();
                                        report.Dispose();
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                    if (count == 0)
                    {
                        MessageBox.Show("Please Select at least one Cash Entry for Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
            }
        }


        private DataTable CreateReceiptDataTable(DataGridViewRow dgRow, string[] strName)
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmailID", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("VoucherNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("CashAccount", typeof(String));
                myDataTable.Columns.Add("CashStatus", typeof(String));
                myDataTable.Columns.Add("CastType", typeof(String));
                myDataTable.Columns.Add("AccountName", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("AmountinWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                string strVCode = "";
                string[] str = strName[1].Split(' ');
                if (str.Length > 1)
                    strVCode = str[0];

                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = "FOR "+ MainPage.strGRCompanyName;
                row["VoucherNo"] = strName[1] + "/D";
                row["Date"] = Convert.ToDateTime(dgRow.Cells["date"].Value).ToString("dd/MM/yy");
                row["Description"] = dgRow.Cells[4].Value;
                row["CastType"] = "CONSIGNEE COPY";
                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                string strPartyName = "", strPGroupName = dba.GetGroupNameFromFullName(txtParty.Text);
                if (strPGroupName == "CASH A/C" || strPGroupName == "BANK A/C")
                {
                    strPartyName = strName[0];
                    string strAmount = Convert.ToString(dgRow.Cells["debit"].Value);
                    if (strAmount != "")
                    {
                        row["AccountName"] = txtParty.Text;
                        row["CashAccount"] = strPartyName;
                        row["CashStatus"] = "Receipt/Payment";
                        row["Amount"] = strAmount;
                        row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                    }
                    else
                    {
                        strAmount = Convert.ToString(dgRow.Cells["credit"].Value);
                        row["AccountName"] = strPartyName;
                        row["CashAccount"] = txtParty.Text;
                        row["CashStatus"] = "Receipt/Payment";
                        row["Amount"] = strAmount;
                        row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                    }                
                }
                else// if (dba.GetGroupNameFromFullName() == "CASH A/C")
                {
                    strPartyName = strName[0];
                    strPGroupName = dba.GetGroupNameFromFullName(strPartyName);
                    if (strPGroupName == "CASH A/C" || strPGroupName == "BANK A/C")
                    {
                        string strAmount = Convert.ToString(dgRow.Cells["debit"].Value);
                        if (strAmount != "")
                        {
                            row["AccountName"] = strPartyName;
                            row["CashAccount"] = txtParty.Text;
                            row["CashStatus"] = "Receipt/Payment";
                            row["Amount"] = strAmount;
                            row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                        }
                        else
                        {
                            strAmount = Convert.ToString(dgRow.Cells["credit"].Value);
                            row["AccountName"] = txtParty.Text;
                            row["CashAccount"] = strPartyName;
                            row["CashStatus"] = "Receipt/Payment";
                            row["Amount"] = strAmount;
                            row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                        }
                    }
                }
                row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where (CashVCode='" + strVCode + "' OR BankVCode='" + strVCode + "') Order by CD.ID asc ");
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];
                    row["CompanyAddress"] = _row["CompanyAddress"];
                    row["CompanyEmailID"] = _row["CompanyPhoneNo"];
                    row["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                    row["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];
                }

                myDataTable.Rows.Add(row);              

            }
            catch
            {
            }
            return myDataTable;
        }

        private DataTable CreateOfficeReceiptDataTable(DataGridViewRow dgRow, string[] strName)
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmailID", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("VoucherNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("CashAccount", typeof(String));
                myDataTable.Columns.Add("CashStatus", typeof(String));
                myDataTable.Columns.Add("CastType", typeof(String));
                myDataTable.Columns.Add("AccountName", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("AmountinWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                string strVCode = "";
                string[] str = strName[1].Split(' ');
                if (str.Length > 1)
                    strVCode = str[0];
                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = "FOR " + MainPage.strGRCompanyName;
                row["VoucherNo"] = strName[1] + "/D";
                row["Date"] = Convert.ToDateTime(dgRow.Cells["date"].Value).ToString("dd/MM/yy");
                row["Description"] = dgRow.Cells["desc"].Value;
                row["CastType"] = "OFFICE COPY";
                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                string strPartyName = "", strPGroupName = dba.GetGroupNameFromFullName(txtParty.Text);

                if (strPGroupName == "CASH A/C" || strPGroupName == "BANK A/C")
                {
                    strPartyName = strName[0];
                    string strAmount = Convert.ToString(dgRow.Cells["debit"].Value);
                    if (strAmount != "")
                    {
                        row["AccountName"] = txtParty.Text;
                        row["CashAccount"] = strPartyName;
                        row["CashStatus"] = "Receipt/Payment";
                        row["Amount"] = strAmount;
                        row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                    }
                    else
                    {
                        strAmount = Convert.ToString(dgRow.Cells["credit"].Value);
                        row["AccountName"] = strPartyName;
                        row["CashAccount"] = txtParty.Text;
                        row["CashStatus"] = "Receipt/Payment";
                        row["Amount"] = strAmount;
                        row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                    }
                }
                else
                {
                    strPartyName = strName[0];
                    strPGroupName = dba.GetGroupNameFromFullName(strPartyName);

                    if (strPGroupName == "CASH A/C" || strPGroupName == "BANK A/C")
                    {
                        string strAmount = Convert.ToString(dgRow.Cells["debit"].Value);
                        if (strAmount != "")
                        {
                            row["AccountName"] = strPartyName;
                            row["CashAccount"] = txtParty.Text;
                            row["CashStatus"] = "Receipt/Payment";
                            row["Amount"] = strAmount;
                            row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                        }
                        else
                        {
                            strAmount = Convert.ToString(dgRow.Cells["credit"].Value);
                            row["AccountName"] = txtParty.Text;
                            row["CashAccount"] = strPartyName;
                            row["CashStatus"] = "Receipt/Payment";
                            row["Amount"] = strAmount;
                            row["AmountinWord"] = currency.changeCurrencyToWords(Convert.ToDouble(strAmount).ToString("0"));
                        }
                    }
                }
                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where  (CashVCode='" + strVCode + "' OR BankVCode='" + strVCode + "') Order by CD.ID asc ");
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];
                    row["CompanyAddress"] = _row["CompanyAddress"];
                    row["CompanyEmailID"] = _row["CompanyPhoneNo"];
                    row["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                    row["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];
                }

                row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private string ExportPDFFile(string strPath)
        {
            // string strFileName = "";
            if (dgrdLedger.Rows.Count > 0)
            {
                DataTable _dtAdvance = null;
                DataTable dt = CreateDataTableForPrint( ref _dtAdvance);
                if (dt.Rows.Count > 0)
                {
                    Reporting.LedgerReport_Remark report = new SSS.Reporting.LedgerReport_Remark();
                    report.SetDataSource(dt);
                    report.Subreports[0].SetDataSource(_dtAdvance);

                    if (strPath != "" && strPath.Contains("\\"))
                    {
                        report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                    }
                    report.Close();
                    report.Dispose();
                }

            }
            else
            {
                strPath = "";
                MessageBox.Show("There is no record for Exporting ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return strPath;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                string strFileName = "", strPath = "";

                if (txtParty.Text != "")
                    strFileName = txtParty.Text.Replace(" ", "_").Replace(".", "_").Replace("(", "_").Replace(")", "_").Replace(":", "").Replace("-", "_").Replace("&", "AND").Replace(",", "").Replace("/", "_");
                else
                    strFileName = "Ledger_Statement";

                SaveFileDialog _browser = new SaveFileDialog();
                _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                _browser.FileName = strFileName + ".pdf";
                _browser.ShowDialog();

                if (_browser.FileName != "")
                    strPath = _browser.FileName;

                strPath = ExportPDFFile(strPath);
                if (strPath != "")
                    MessageBox.Show("Thanks ! File exported successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                for (int _index = 0; _index < dgrdLedger.Rows.Count; _index++)
                {
                    if (Convert.ToBoolean(dgrdLedger.Rows[_index].Cells["chkCheck"].EditedFormattedValue))
                    {
                        dgrdLedger.Rows.RemoveAt(_index);
                        _index--;
                        count++;
                    }
                }

                if (count > 0)
                {
                    MessageBox.Show(count + " records successfully hidden.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    CalculateBalanceAmount();
                }
            }
            catch
            {
            }
        }


        private bool CalculateBalanceAmount()
        {
            try
            {
                double dDebitAmt = 0, dCreditAmt = 0, dTotalAmt = 0, dAmt = 0;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    if (Convert.ToString(row.Cells["costcentre"].Value) == "")
                    {
                        if (Convert.ToString(row.Cells["debit"].Value) != "")
                        {
                            dDebitAmt += dAmt = Convert.ToDouble(row.Cells["debit"].Value);
                            dTotalAmt += dAmt;
                        }
                        else if (Convert.ToString(row.Cells["credit"].Value) != "")
                        {
                            dCreditAmt += dAmt = Convert.ToDouble(row.Cells["credit"].Value);
                            dTotalAmt -= dAmt;
                        }

                        // dTotalAmt += dAmt;

                        if (dTotalAmt > 0)
                            row.Cells["balance"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dTotalAmt < 0)
                            row.Cells["balance"].Value = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            row.Cells["balance"].Value = "0.00";
                    }
                }

                lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                if (dTotalAmt > 0)
                    lblBalance.Text = lblBalAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalance.Text = lblBalAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalance.Text = lblBalAmount.Text = "0";


                dDebitAmt =dCreditAmt =dTotalAmt = dAmt = 0;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    if (Convert.ToString(row.Cells["costcentre"].Value) != "")
                    {
                        if (Convert.ToString(row.Cells["debit"].Value) != "")
                        {
                            dDebitAmt += dAmt = Convert.ToDouble(row.Cells["debit"].Value);
                            dTotalAmt += dAmt;
                        }
                        else if (Convert.ToString(row.Cells["credit"].Value) != "")
                        {
                            dCreditAmt += dAmt = Convert.ToDouble(row.Cells["credit"].Value);
                            dTotalAmt -= dAmt;
                        }

                        // dTotalAmt += dAmt;

                        if (dTotalAmt > 0)
                            row.Cells["balance"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dTotalAmt < 0)
                            row.Cells["balance"].Value = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            row.Cells["balance"].Value = "0.00";
                    }
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnCheckStatus_Click(object sender, EventArgs e)
        {
            try
            {
                btnCheckStatus.Enabled = false;
                if (dgrdLedger.Rows.Count > 0)
                {

                    DialogResult result = MessageBox.Show("Are you sure you want to change cheque status ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ChangeChequeStatus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCheckStatus.Enabled = true;
        }

        private void txtVCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CASHVCODE", "SEARCH VOUCHER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtVCode.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    row.Cells["chkCheck"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdLedger_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkTickAll.Visible = false;
                    else
                        chkTickAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void ChangeChequeStatus()
        {
            string strBID = "", strQuery = "", strDate = "";

            foreach (DataGridViewRow row in dgrdLedger.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkCheck"].Value))
                {
                    strBID = Convert.ToString(row.Cells["id"].Value);
                    // strStatus = Convert.ToString(row.Cells["chqStatus"].Value);
                    strDate = Convert.ToString(row.Cells["chqDate"].Value);
                    if (strDate.Length==10)
                    {
                        strDate = dba.ConvertDateInExactFormat(strDate).ToString("MM/dd/yyyy");
                        strQuery += " Update BalanceAmount Set [ChqDate]='" + strDate + "', [ChequeStatus]=1,[UpdateStatus]=1,[CHQStatusChangedBy]='" + MainPage.strLoginName + "' Where  VoucherCode!='' and  (VoucherCode+CAST(VoucherNo as varchar)+JournalID) in (Select (VoucherCode+CAST(VoucherNo as varchar)+JournalID) from BalanceAmount Where  VoucherCode!='' and  BalanceID in (" + strBID + ")) ";
                    }
                    else
                        strQuery += " Update BalanceAmount Set [ChqDate]=NULL, [ChequeStatus]=0,[UpdateStatus]=1,[CHQStatusChangedBy]='" + MainPage.strLoginName + "' Where VoucherCode!='' and (VoucherCode+CAST(VoucherNo as varchar)+JournalID) in (Select (VoucherCode+CAST(VoucherNo as varchar)+JournalID) from BalanceAmount Where  VoucherCode!='' and  BalanceID in (" + strBID + ")) ";
                }
            }

            if (strQuery == "")
            {
                MessageBox.Show("Sorry ! Please select atleast one entry for change status.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //if (strClearID != "")
                //    strQuery = "Update BalanceAmount Set [ChequeStatus]=0,[UpdateStatus]=1,[CHQStatusChangedBy]='" + MainPage.strLoginName + "' Where (VoucherCode+' '+CAST(VoucherNo as varchar)) in (Select (VoucherCode+' '+CAST(VoucherNo as varchar)) from BalanceAMount Where BalanceID in (" + strClearID + ")) ";
                //else if (strUnclearID != "")
                //    strQuery = "Update BalanceAmount Set [ChequeStatus]=1,[UpdateStatus]=1,[CHQStatusChangedBy]='" + MainPage.strLoginName + "' Where (VoucherCode+' '+CAST(VoucherNo as varchar)) in (Select (VoucherCode+' '+CAST(VoucherNo as varchar)) from BalanceAMount Where BalanceID in (" + strUnclearID + ")) ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thanks ! Status changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    chkAll.Checked = false;
                    this.SearchRecord();
                }
                else
                {
                    MessageBox.Show("Sorry ! Unable to change status.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void txtMonthName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonthName.Text = objSearch.strSelectedData;

                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdLedger_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int cIndex = dgrdLedger.CurrentCell.ColumnIndex;
                if (dgrdLedger.CurrentCell.ColumnIndex == 10)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (dgrdLedger.CurrentCell.ColumnIndex == 10)
                {
                    if (Convert.ToBoolean(dgrdLedger.CurrentRow.Cells["chkCheck"].EditedFormattedValue))
                    {
                        Char pressedKey = e.KeyChar;
                        if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                            e.Handled = false;
                        else
                        {
                            dba.KeyHandlerPoint(sender, e, 0);
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
            catch { }

        }

        private void dgrdLedger_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 10)
                {
                    string strDate = Convert.ToString(dgrdLedger.CurrentCell.EditedFormattedValue);
                    if (strDate != "")
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate, false);
                            if (!txtDate.Text.Contains("/"))
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                if (e.RowIndex < dgrdLedger.Rows.Count - 1)
                                {
                                    dgrdLedger.EndEdit();
                                }
                            }
                            dgrdLedger.CurrentCell.Value = txtDate.Text;
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdLedger_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 10)
                {
                    dgrdLedger.CurrentRow.Cells["chkCheck"].Value = true;
                }
            }
            catch
            {
            }
        }

        private void dgrdLedger_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 10)
                {
                    string strDate = Convert.ToString(dgrdLedger.CurrentCell.EditedFormattedValue);
                    if (strDate.Length == 8)
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate, false);
                            if (!txtDate.Text.Contains("/"))
                            {

                            }
                            else
                            {
                                if (e.RowIndex < dgrdLedger.Rows.Count - 1)
                                {
                                    dgrdLedger.EndEdit();
                                }
                            }
                            dgrdLedger.CurrentCell.Value = txtDate.Text;
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                }
            }
            catch { }
        }

        private void chkChqDate_CheckedChanged(object sender, EventArgs e)
        {
            txtClearFromDate.Enabled = txtClearToDate.Enabled = chkChqDate.Checked;
            txtClearFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtClearToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            ClearRecord();
        }

        private void txtClearFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkChqDate.Checked, false, false, true);
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                btnPartyName.Enabled = pnlRelatedParty.Visible = false;
                //if (dgrdRelatedParty.Rows.Count > 0)
                //    pnlRelatedParty.Visible = true;
                //else
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", Keys.Space);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtParty.Text = objSearch.strSelectedData;
                    GetRelatedpartyDetails();
                    ClearRecord();
                }
            }
            catch { }
            btnPartyName.Enabled = true;
        }

        private void txtParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtParty.Text);
        }

        private void chkTickAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    row.Cells["tick"].Value = chkTickAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtParty.Text != "" && dgrdLedger.Rows.Count>0)
                {
                    string strMessage = "", strMobileNo = "", strBalance = lblBalAmount.Text.Replace(",", ""), strAccountNo = "";
                    string[] strFullName = txtParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strAccountNo = strFullName[0];
                        object objMobile = DataBaseAccess.ExecuteMyScalar("Select  MobileNo from SupplierMaster Where (AreaCode+AccountNo)='" + strAccountNo + "' ");
                        strMobileNo = Convert.ToString(objMobile);
                        if (strMobileNo != "")
                        {
                            strAccountNo = System.Text.RegularExpressions.Regex.Replace(strAccountNo, "[^0-9]", "");

                            strMessage = "M/S : " + txtParty.Text + " ! YOUR CURRENT BALANCE : " + strBalance + ".\nBANK DETAIL FOR NEFT/RTGS AS FOLLOWS :\nBANK: ICICI BANK,\nBRANCH: DELHI,\nIFSC CODE: ICIC0000106,\nBANK A/C NO.: SASUSP" + dba.ConvertObjectToDouble(strAccountNo).ToString("000000") + "\nA/C NAME: SARAOGI SUPER SALES PVT LTD.";
                            SendSMSPage objSMS = new SSS.SendSMSPage(strMobileNo, strMessage);
                            objSMS.ShowDialog();
                        }
                    }
                }
            }
            catch { }
        }

        private void chkInvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            txtInvFromDate.Enabled = txtInvToDate.Enabled = chkInvoiceDate.Checked;
            txtInvFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtInvToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
        }

        private void txtInvFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkInvoiceDate.Checked, false, false, true);
        }

        private void btnDetailedLedger_Click(object sender, EventArgs e)
        {
            try
            {
                LedgerAccount_Detailed _objLedger = new SSS.LedgerAccount_Detailed(txtParty.Text);
                _objLedger.MdiParent = MainPage.mymainObject;
                if(chkDate.Checked)
                {
                    _objLedger.chkDate.Checked = chkDate.Checked;
                    _objLedger.txtFromDate.Text = txtFromDate.Text;
                    _objLedger.txtToDate.Text = txtToDate.Text;
                }
                _objLedger.txtAmount.Text = txtAmount.Text;
                _objLedger.txtDescription.Text = txtDescription.Text;
                _objLedger.txtMonthName.Text = txtMonthName.Text;
                _objLedger.txtVCode.Text = txtVCode.Text;
                _objLedger.txtVNo.Text = txtVNo.Text;
                _objLedger.rdoStatusAll.Checked = rdoStatusAll.Checked;
                _objLedger.rdoStatusDR.Checked = rdoStatusDR.Checked;
                _objLedger.rdoStatusCr.Checked = rdoStatusCr.Checked;
                _objLedger.rdoAll.Checked = rdoAll.Checked;
                _objLedger.rdoTick.Checked = rdoTick.Checked;
                _objLedger.rdoUnTick.Checked = rdoUnTick.Checked;
                _objLedger.rdoCHQAll.Checked = rdoCHQAll.Checked;
                _objLedger.rdoCHQClear.Checked = rdoCHQClear.Checked;
                _objLedger.rdoCHQUnclear.Checked = rdoCHQUnclear.Checked;

                _objLedger.chkChqDate.Checked = chkChqDate.Checked;
                _objLedger.chkInvoiceDate.Checked = chkInvoiceDate.Checked;

                _objLedger.txtClearFromDate.Text = txtClearFromDate.Text;
                _objLedger.txtClearToDate.Text = txtClearToDate.Text;
                _objLedger.txtInvFromDate.Text = txtInvFromDate.Text;
                _objLedger.txtInvToDate.Text = txtInvToDate.Text;
                _objLedger.comboAccount.SelectedIndex = _objLedger.comboAccount.SelectedIndex;

                _objLedger.Show();
            }
            catch { }
        }

        private void btnSendWhatsapp_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtParty.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to send email and whatsapp ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SendEmailAndWhatsAppToParty();
                    }
                }
            }
            catch { }
        }

        private void lnkHint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pnlColor.Visible)
                pnlColor.Visible = false;
            else
                pnlColor.Visible = true;
        }

        private void btnSendSMS_Enter(object sender, EventArgs e)
        {
            if (dgrdLedger.Rows.Count == 0)
                btnGo.Focus();
        }

        private void lnkShowIntDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //try
            //{
            //    if (_dtIntDiscDetails!= null && _dtIntDiscDetails.Rows.Count>0)
            //    {
            //        LastIntDiscountDetails objLast = new LastIntDiscountDetails(_dtIntDiscDetails);
            //        objLast.ShowDialog();
            //    }
            //}
            //catch { }
        }
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

                    string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Ledger_Statement";
                    strPath = strNewPath + "\\" + strFileName;

                    if (File.Exists(strPath))
                        File.Delete(strPath);

                    Directory.CreateDirectory(strNewPath);

                    strPath = ExportPDFFile(strPath);

                    string strEmailID = "", strWhatsAppNo = "";
                    if (strPath != "")
                    {
                        string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strPartyID + "' ";
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                            strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);
                            bool _bEmailStatus = false;
                            if (strEmailID != "")
                            {
                                string strMessage = "A/c : " + txtParty.Text + ", we are sending ledger statement which is attached with this mail, Please find attachment.";
                                string strSub = "LEDGER STATEMENT";

                               _bEmailStatus= DataBaseAccess.SendEmail(strEmailID, strSub, strMessage, strPath, "", "LEDGER STATEMENT",true);                                                             
                            }
                            if (strWhatsAppNo != "")
                            {
                                SendWhatsappMessage(strWhatsAppNo, strPath, strFileName, _bEmailStatus, strEmailID);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lnkShowMasterSummary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (txtParty.Text != "")
                {
                    ShowPartyMasterSummary objSummary = new ShowPartyMasterSummary(txtParty.Text);
                    objSummary.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummary.ShowInTaskbar = true;
                    objSummary.Show();
                }
            }
            catch { }
        }

        private void txtAccountID_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;

                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    string strPParty = "", strAccountID = txtAccountID.Text;
                    if (txtParty.Text != "")
                    {
                        string[] str = txtParty.Text.Split(' ');
                        if (str.Length > 1)
                            strPParty = str[0];
                    }

                    SearchDataOther objSearch = new SearchDataOther("ALLACCOUNTID", strPParty, "SEARCH ACCOUNT NAME", e.KeyCode, false);
                    objSearch.ShowDialog();
                    if (txtParty.Text == "" && objSearch.strSelectedData!="")
                    {
                        string[] str = objSearch.strSelectedData.Split('|');
                        txtAccountID.Text = str[0];
                        if (str.Length > 1)
                            txtParty.Text = str[1];
                    }
                    else
                        txtAccountID.Text = objSearch.strSelectedData;

                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFromDate_Enter(object sender, EventArgs e)
        {
           dba.SelectInTextBox(sender, 0, 0);
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                _bPrevilegeAccount = false;
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtParty.Text, strNewSecurityDate = "", strNewGrade = "", strNewMobileNo = "", strNewCategory = "", strNewLimit = "", strNewBlackList = "", strNewTransactionLock = "", strNewOrangeList = "", strNewLastPaymentAmt = "";
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
                        strNewOrangeList = Convert.ToString(dgrdRelatedParty.CurrentRow.Cells["orangeListed"].Value);


                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                        dgrdRelatedParty.CurrentRow.Cells["securityChqDate"].Value = _STRChqDate;
                        dgrdRelatedParty.CurrentRow.Cells["Grade"].Value = _STRGrade;
                        dgrdRelatedParty.CurrentRow.Cells["MobileNo"].Value = _STRMobileNo;
                        dgrdRelatedParty.CurrentRow.Cells["Category"].Value = _STRCategory;
                        dgrdRelatedParty.CurrentRow.Cells["amtLimit"].Value = _STRAmtLimit;

                        dgrdRelatedParty.CurrentRow.Cells["blackListed"].Value = _STRBlackList;
                        dgrdRelatedParty.CurrentRow.Cells["transactionLock"].Value = _STRTransasactionLock;
                        dgrdRelatedParty.CurrentRow.Cells["orangeListed"].Value = _STROrangeList;
                        
                        _STRChqDate = strNewSecurityDate;
                        _STRBlackList = strNewBlackList;
                        _STRTransasactionLock = strNewTransactionLock;                    

                        _STRGrade = strNewGrade;
                        _STRCategory = strNewCategory;
                        _STRMobileNo = strNewMobileNo;
                        _STRAmtLimit = strNewLimit;

                        if (_STRGrade.Contains("PREVILEGE ACCOUNT"))
                            _bPrevilegeAccount = true;

                        if (!_STRGrade.Contains("GRADE") && _STRGrade != "")
                            _STRGrade = "GRADE : " + _STRGrade;
                        if (_STRGrade != "" && !_STRGrade.Contains(","))
                            _STRGrade += ", ";                        
                    }
                    txtParty.Focus();
                }

                bool _bTransactionLock = Convert.ToBoolean(_STRTransasactionLock), _bBlackListed = Convert.ToBoolean(_STRBlackList), _bOrangeListed = false, _bSecurityChq = false;
                if (_STRChqDate != "")
                    _bSecurityChq = true;
                if (_STROrangeList.Contains("TRUE"))
                    _bOrangeListed = Convert.ToBoolean(_STROrangeList);

                picBoxBlack.Visible = _bBlackListed;
                picBoxYellow.Visible = _bTransactionLock;
                picBoxOrange.Visible = _bOrangeListed;
                picBoxGreen.Visible = _bSecurityChq;
            }
            catch { }
        }

        private void SendWhatsappMessage(string strMobileNo, string strPath, string strFileName, bool _bEmailStatus,string strEmailID)
        {
            try
            {
                string strMessage = "",strEmailMessage="";
                string strFilePath = "http://pdffiles.ssspltd.com/Ledger_Statement/" + strFileName, strName = dba.GetSafePartyName(txtParty.Text);

                bool _bStatus = dba.UploadLedgerInterestStatementPDFFile(strPath, strFileName, "Ledger_Statement");
                if (!_bStatus)
                {
                    DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                    if (_updateResult == DialogResult.Retry)
                        _bStatus = dba.UploadSaleBillPDFFile(strPath, strFileName, "Ledger_Statement");
                }
                if (_bStatus)
                {
                  //  strMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + strFilePath + "\"}";
                    strMessage = "\"variable1\": \"" + strName + "\",";
                    string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, "ledger_pdf", strMessage, "", strFilePath);
                    //string strResult = WhatsappClass.SendWhatsAppMessage(strMobileNo, strMessage, strFilePath, "LEDGER_STATEMENT", "", "PDF");
                    if (strResult != "")
                    {
                        if (_bEmailStatus)
                            strEmailMessage = " & Email sent on " + strEmailID;
                        _bEmailStatus = false;
                        MessageBox.Show("Thank you ! Whatsapp messsage sent on : " + strMobileNo + strEmailMessage + " ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }           
            }
            catch { }
            if (_bEmailStatus)            
                MessageBox.Show("Thank you ! Email sent on " + strEmailID+" ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            
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
            if (e.ColumnIndex > 0)
                e.Cancel = true;
        }

        public void GetRelatedpartyDetails()
        {
            try
            {
                picBoxBlack.Visible = picBoxBlue.Visible = picBoxGreen.Visible = picBoxPurple.Visible = picBoxYellow.Visible = picBoxOrange.Visible = false;
                pnlRelatedParty.Visible = lblChequeStatus.Visible = _bPrevilegeAccount = false;//lblGradeCategory.Visible = lblMobileNoLimit.Visible =
                dgrdRelatedParty.Rows.Clear();
                _STRChqDate = "";// lblGradeCategory.Text = lblMobileNoLimit.Text = "";
                //lnkShowIntDetails.Visible = false;
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
                                _STRTransasactionLock = Convert.ToString(row["TransactionLock"]);
                                _STRBlackList = Convert.ToString(row["BlackList"]);
                                _STROrangeList = Convert.ToString(row["OrangeList"]);
                                dgrdRelatedParty.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                                dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["Name"];
                                dgrdRelatedParty.Rows[_index].Cells["securityChqDate"].Value = row["ChqDate"];
                                dgrdRelatedParty.Rows[_index].Cells["Grade"].Value = row["Grade"];
                                dgrdRelatedParty.Rows[_index].Cells["category"].Value = row["Category"];
                                dgrdRelatedParty.Rows[_index].Cells["mobileNo"].Value = row["MobileNo"];

                                dgrdRelatedParty.Rows[_index].Cells["blackListed"].Value = _STRBlackList;
                                dgrdRelatedParty.Rows[_index].Cells["transactionLock"].Value = _STRTransasactionLock;
                                dgrdRelatedParty.Rows[_index].Cells["orangeListed"].Value = _STROrangeList;
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
                            _STRBlackList = Convert.ToString(_row["BlackList"]);
                            _STRTransasactionLock = Convert.ToString(_row["TransactionLock"]);
                            _STROrangeList = Convert.ToString(_row["OrangeList"]);

                            if (Convert.ToString(_row["Grade"]) == "PREVILEGE ACCOUNT")
                                _bPrevilegeAccount = true;

                            if (!_STRGrade.Contains("GRADE") && _STRGrade != "")
                                _STRGrade = "GRADE : " + _STRGrade;
                            if (_STRGrade != "" && !_STRGrade.Contains(","))
                                _STRGrade += ", ";

                        }
                    }
                }

                if (dgrdRelatedParty.Rows.Count > 0)
                    pnlRelatedParty.Visible = true;

                bool _bTransactionLock = Convert.ToBoolean(_STRTransasactionLock), _bBlackListed = Convert.ToBoolean(_STRBlackList), _bOrangeListed = false, _bSecurityChq = false;
                if (_STRChqDate != "")
                    _bSecurityChq = true;
                if (_STROrangeList.Contains("TRUE"))
                    _bOrangeListed= Convert.ToBoolean(_STROrangeList);

                picBoxBlack.Visible = _bBlackListed;
                picBoxYellow.Visible = _bTransactionLock;
                picBoxOrange.Visible = _bOrangeListed;
                picBoxGreen.Visible = _bSecurityChq;
            }
            catch { }
        }

    }

}

