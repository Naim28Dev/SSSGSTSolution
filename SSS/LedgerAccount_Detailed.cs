using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class LedgerAccount_Detailed : Form
    {
        DataBaseAccess dba;
        string[] strColor = { "LightSteelBlue", "PeachPuff", "Thistle", "Lavender", "LightSalmon", "LightCoral", "ButtonShadow", "BurlyWood", "Gainsboro", "Beige" };
        int index = 0;
        ChangeCurrencyToWord currency;
        string[] strAllParty, strPartyStatus;
        List<string> displaydClms = new List<String>();

        public LedgerAccount_Detailed()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
           // btnSendSMS.Enabled = MainPage.mymainObject.bSMSReport;
        }

        public LedgerAccount_Detailed(string strPartyName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
            txtParty.Text = strPartyName;           
           // btnSendSMS.Enabled = MainPage.mymainObject.bSMSReport;
        }


        private void LedgerAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (panelCompany.Visible)
                        panelCompany.Visible = false;
                    else if (panelSearch.Visible)
                        panelSearch.Visible = false;
                    else if (panalColumnSetting.Visible)
                        panalColumnSetting.Visible = false;
                    else
                    {
                        this.Close();
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
                //else if (e.KeyCode == Keys.F7)
                //{
                //    if (btnPrint.Enabled)
                //        PrintLedger();
                //}
            }
            catch
            {
            }
        }

        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    //ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    //string strData = objRead.ReadDataFromCard("ALLPARTY");
                    //if (strData != "")
                    //    txtParty.Text = strData;
                    //ClearRecord();
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtParty.Text = objSearch.strSelectedData;
                        ClearRecord();
                    }
                    else
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
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
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

        private string CreateQuery(ref string strChqStatus, ref string strInvQuery, ref string _strAccountID)
        {
            string strQuery = "";
            try
            {
                if (txtParty.Text != "")
                {
                    string[] strFullName = txtParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        _strAccountID = strFullName[0].Trim();
                        strQuery += " and AccountID='" + strFullName[0].Trim() + "' ";
                    }
                }

                if (chkDate.Checked && txtFromDate.Text.Length==10 && txtToDate.Text.Length==10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                    strQuery += " and Date >='" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
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
                    strInvQuery = " OUTER APPLY (Select Date as BillDate) PR ";

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
                    //if (Convert.ToString(comboAccount.SelectedItem) == "JOURNAL A/C")
                    //{
                    //    strQuery += " and  AccountStatus not in ('CASH A/C','SALES A/C','PURCHASE A/C')  ";
                    //}
                    //else
                    //{
                    strQuery += " and  AccountStatus Like('%" + comboAccount.SelectedItem + "%')  ";
                    //}
                }

                string strStatus = GetStatus();
                if (strStatus != "")
                    strQuery += " and Tick='" + strStatus + "' ";

                if (rdoStatusDR.Checked)
                    strQuery += " and Status='DEBIT' ";
                else if (rdoStatusCr.Checked)
                    strQuery += " and Status='CREDIT' ";

                if (rdoCHQClear.Checked)
                    strChqStatus = " Where CHQStatus='CLEAR' ";
                else if (rdoCHQUnclear.Checked)
                    strChqStatus = " Where CHQStatus='UNCLEAR' ";

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
            lblBalAmount.Text = lblCredit.Text = lblDebit.Text = "0.00";
        }

        private string GetCommaSeperateColumn(DataTable _dt, ref bool _bPurchase, ref string strColumnQuery)
        {
            string strColumnName = "",strName="";
            foreach (DataRow row in _dt.Rows)
            {
                strName = Convert.ToString(row["PartyName"]);
                if (strColumnName != "")
                    strColumnName += ",";
                strColumnName += "[" + strName + "]";

                if (strColumnQuery != "")
                    strColumnQuery += " OR ";

                strColumnQuery += " CAST([" + strName + "] as Money)!=0 ";

                if (strName=="PURCHASE A/C" || strName=="SALES A/C") 
                    _bPurchase = true;                
            }
            if (strColumnQuery != "")
                strColumnQuery = "(" + strColumnQuery + ")";

            return strColumnName;
        }

        public void GetCurrentQuarterDetails()
        {
            ClearRecord();
            string strQuery = "", strChqStatus = "", strInvQuery = "", _strMainQuery = "", strAccountID = "", strSubQuery = CreateQuery(ref strChqStatus, ref strInvQuery, ref strAccountID),strPColumnQuery="",strPOuterQuery="";

            _strMainQuery = "Select Distinct PartyName,ID from ( "
                          + " Select Distinct 0 ID,'OPENING' as PartyName from BalanceAmount Where AccountID='" + strAccountID + "' and  AccountStatus='OPENING' and CAST(AMount as Money)>0  "
                          + " UNION ALL  Select Distinct 1 ID,AccountStatus from BalanceAmount Where (AccountID='" + strAccountID + "' " + strSubQuery + " and  (VoucherCode+' '+CAST(VoucherNo as varchar))in (select (VoucherCode+' '+CAST(VoucherNo as varchar)) from BalanceAmount Where CAST(Amount as Money)>0 and VoucherCode!='' and AccountID in ('" + strAccountID + "') )) UNION ALL "
                          + " Select Distinct 1 ID,AccountStatus as PartyName from BalanceAmount Where AccountStatus != 'OPENING' and CAST(Amount as Money)>0  " + strSubQuery + " and (AccountID = '" + strAccountID + "' and Description in (select Description from BalanceAmount Where VoucherCode='' and AccountID in ('" + strAccountID + "'))))_Balance order by ID,PartyName ";

            DataTable _dt = dba.GetDataTable(_strMainQuery);
            bool _bPStatus = false;
            string strColumnQuery="", strColumnName = GetCommaSeperateColumn(_dt, ref _bPStatus, ref strColumnQuery);

            if (_bPStatus)
            {
                strPColumnQuery = ",[InvoiceNo],[InvoiceDate],[Remark],[GrossAmount],[TaxAmount],[DisAmount],[OtherAmt],[NetAmount]";
                //  strPOuterQuery = " OUTER APPLY (Select InvoiceNo,Convert(varchar,InvoiceDate,103)InvoiceDate,[Remark],GrossAmount,TaxAmount,DisAmount,(CAST(Packing as Money)+CAST(Freight as Money)+CAST(Tax as Money)+(CAST(OtherSign+CAST(OtherAmount as varchar) as money)+TCSAmt))OtherAmt,NetAmount from GoodsReceive WHere (ReceiptCode+' '+CAST(ReceiptNo as varchar))=Balance.VoucherCode and AccountStatus='PURCHASE A/C' UNION ALL Select InvoiceNo,Convert(varchar,InvoiceDate,103)InvoiceDate,[Remark],GrossAmt as GrossAmount,TaxAmt as TaxAmount,DiscAmt as DisAmount,(PackingAmt+(CAST(OtherSign+CAST(OtherAmt as varchar) as money)+TaxFree+TCSAmt))OtherAmt,NetAmt as NetAmount  from PurchaseBook WHere (BillCode+' '+CAST(BillNo as varchar))=Balance.VoucherCode and AccountStatus='PURCHASE A/C' )Purchase ";
                strPOuterQuery = " OUTER APPLY (Select InvoiceNo,Convert(varchar,InvoiceDate,103)InvoiceDate,[Remark],GrossAmount,TaxAmount,DisAmount,(CAST(Packing as Money)+CAST(Freight as Money)+CAST(Tax as Money)+(CAST(OtherSign+CAST(OtherAmount as varchar) as money)+TCSAmt))OtherAmt,NetAmount from GoodsReceive WHere (ReceiptCode+' '+CAST(ReceiptNo as varchar))=Balance.VoucherCode and AccountStatus='PURCHASE A/C' UNION ALL "
                                + " Select InvoiceNo, Convert(varchar, InvoiceDate,103)InvoiceDate,[Remark],GrossAmt as GrossAmount,TaxAmt as TaxAmount,DiscAmt as DisAmount,(PackingAmt+(CAST(OtherSign+CAST(OtherAmt as varchar) as money)+TaxFree+TCSAmt))OtherAmt,NetAmt as NetAmount from PurchaseBook WHere(BillCode+' '+CAST(BillNo as varchar))=Balance.VoucherCode and AccountStatus='PURCHASE A/C' UNION ALL "
                                + " Select (BillCode+' '+CAST(BillNo as varchar)) InvoiceNo,Convert(varchar, BillDate,103)InvoiceDate,[Remark],GrossAmt as GrossAmount,TaxAmount,CAST(NetAddLs as money) as DisAmount,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)),2) OtherAmt,NetAmt as NetAmount from SalesRecord SR WHere(BillCode+' '+CAST(BillNo as varchar))=Balance.VoucherCode and AccountStatus='SALES A/C' UNION ALL "
                                + " Select (BillCode+' '+CAST(BillNo as varchar)) InvoiceNo,Convert(varchar, Date,103)InvoiceDate,[Remark],GrossAmt as GrossAmount,TaxAmt as TaxAmount,0.00 as DisAmount,ROUND(PackingAmt + GreenTax+PostageAmt + ((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+CAST((SR.Description + CAST(SR.DisAmt as varchar)) as Money))), 2)OtherAmt,NetAmt as NetAmount from SalesBook SR WHere(BillCode+' '+CAST(BillNo as varchar))=Balance.VoucherCode and AccountStatus='SALES A/C' )Purchase ";
            }

            strQuery += "Select BDate,VoucherCode,DDays," + strColumnName + strPColumnQuery+ ",CreatedBy,UpdatedBy,JournalID,CHQStatus,ChequeDate from ( "
                     + " Select ID, DDays,Date, CONVERT(varchar, Date,103)BDate,ISNULL(UPPER(AccountStatus), '') AccountStatus,VoucherCode,VoucherType,CAST(Amount as money)_Amount,CreatedBy,UpdatedBy,JournalID,CHQStatus,0 Onaccount,ISNULL(ChqDate, '') ChqDate,(CASE WHEN ChqDate is NULL or ChqDate = '' then Date else Convert(datetime, ChqDate, 103) end)ChequeDate" + strPColumnQuery+" from ( "
                     + " Select 0 as ID, Date,DATEDIFF(DD,Date,GETDATE()) DDays, 'OPENING' as AccountStatus, '' as VoucherCode, 'OPENING' as VoucherType, (Case when Status = 'Debit' then Amount else '-' + Amount end) Amount,BA.Tick,BA.UserName CreatedBy, UpdatedBy, JournalID,'' as CHQStatus, '' ChqDate,BillDate from BalanceAmount BA  OUTER APPLY (Select Date as BillDate)PR Where AccountStatus = 'OPENING' and CAST(Amount as Money) > 0  and AccountID in ('" + strAccountID + "') " + strSubQuery + " Union All "
                     + " Select 1 as ID, Date,DATEDIFF(DD,Date,GETDATE()) DDays, PartyName as AccountStatus, (VoucherCode + ' ' + CAST(VoucherNo as varchar)) VoucherCode, (CASE WHEN AccountStatus='JOURNAL A/C' then AccountStatus when VoucherCode in (Select CashVCOde from CompanySetting) then 'CASH A/C' else 'BANK A/C' end) VoucherType, (Case when Status = 'Debit' then '-' +Amount else Amount end) Amount,BA.Tick,BA.UserName CreatedBy, UpdatedBy, JournalID,(CASE WHEN((VoucherCode != '' OR JournalID != '')  and(Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus, 0) = 1) then 'CLEAR' WHEN((VoucherCode != '' OR JournalID != '')  and(Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)= 0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar, ChqDate, 103) ChqDate,BillDate from BalanceAmount BA OUTER APPLY (Select Date as BillDate)PR Where AccountStatus != 'OPENING' and VoucherCode!= '' and CAST(Amount as Money) > 0 and AccountID not in ('" + strAccountID + "') and(VoucherCode + ' ' + CAST(VoucherNo as varchar))in (select(VoucherCode + ' ' + CAST(VoucherNo as varchar)) from BalanceAmount Where VoucherCode != '' and AccountID in ( '" + strAccountID + "') " + strSubQuery + " )   UNION ALL "
                     + " Select 1 as ID, Date,DATEDIFF(DD,Date,GETDATE()) DDays,AccountStatus, Description as VoucherCode,AccountStatus as VoucherType,(Case when Status = 'Debit' then Amount else '-' + Amount end) Amount,BA.Tick,BA.UserName CreatedBy, UpdatedBy, JournalID,(CASE WHEN((VoucherCode != '' OR JournalID != '')  and(Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus, 0) = 1) then 'CLEAR' WHEN((VoucherCode != '' OR JournalID != '')  and(Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)= 0) Then 'UNCLEAR' else '' end) CHQStatus,CONVERT(varchar, ChqDate, 103) ChqDate,BillDate from BalanceAmount BA OUTER APPLY (Select Date as BillDate)PR Where AccountStatus != 'OPENING' and VoucherCode = '' and(AccountID in ('" + strAccountID + "') and Description in (select Description from BalanceAmount Where VoucherCode = '' and AccountID in ('" + strAccountID + "') " + strSubQuery + " ))) Balance"+ strPOuterQuery+")_Balance "
                     + " Pivot (SUM(_Amount) "
                     + " for VoucherType in (" + strColumnName + ") "
                     + " ) as piv Where BDate!='' and " + strColumnQuery+" Order by ID, Date; ";

            DataTable dt = dba.GetDataTable(strQuery);
            BindDataWithGrid(dt, _dt, _bPStatus);
        }

        private void SetRecordWithDataTable(DataTable dt)
        {
            DataTable _datatable = CreateDataTable();
            string strAdjustedNo = "", strAdjuster = "";
            if (dt != null)
            {
                double dDebitAmt = 0, dCreditAmt = 0, dAmt = 0, dTotalAmt = 0;
                int rowLength = 0, colorIndex = 0;

                for (; rowLength < dt.Rows.Count; rowLength++)
                {
                    DataRow row = dt.Rows[rowLength];
                    DataRow dRow = _datatable.NewRow();
                    dRow["Date"] = row["BDate"];
                    dRow["AccountStatus"] = row["AccountStatus"];
                    dRow["Description"] = row["Description"];
                    dRow["CreatedBy"] = row["CreatedBy"];
                    dRow["UpdatedBy"] = row["UpdatedBy"];
                    dRow["journalID"] = row["JournalID"];
                    //dRow["ChqDate"] = row["ChqDate"];
                    //dRow["ChqStatus"] = row["CHQStatus"];
                    //dRow["Tick"] = false;

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

                //BindDataWithGrid(_datatable);

                lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                if (dTotalAmt > 0)
                    lblBalAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                     lblBalAmount.Text = "0";
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
            return _datatable;
        }

        private void AddGridColumn(DataTable _dt,bool _bPStatus)
        {
            dgrdLedger.Columns.Clear();
            DataGridViewColumn col;
            DataGridViewCell cell;

            col = new DataGridViewColumn();
            cell = new DataGridViewTextBoxCell();
            col.CellTemplate = cell;
            col.HeaderText = "Date";
            col.Name = "date";
            col.Visible = true;
            col.Width = 90;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgrdLedger.Columns.Add(col);

            DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
            linkCol.LinkColor = Color.Black;
            linkCol.LinkBehavior = LinkBehavior.HoverUnderline;
            linkCol.HeaderText = "Vch. Details";
            linkCol.Name = "voucherCode";
            linkCol.Visible = true;
            linkCol.Width = 110;
            linkCol.SortMode = DataGridViewColumnSortMode.Automatic;
            dgrdLedger.Columns.Add(linkCol);

            //col = new DataGridViewColumn();
            //cell = new DataGridViewTextBoxCell();
            //col.CellTemplate = cell;
            //col.HeaderText = "Voucher Type";
            //col.Name = "voucherType";
            //col.Visible = true;
            //col.Width = 110;
            //col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dgrdLedger.Columns.Add(col);

            string strColumnName = "";
            foreach (DataRow row in _dt.Rows)
            {
                strColumnName = Convert.ToString(row["PartyName"]);
                col = new DataGridViewColumn();
                cell = new DataGridViewTextBoxCell();
                col.CellTemplate = cell;
                col.HeaderText = strColumnName;
                col.Name = strColumnName;
                col.Visible = true;
                if (strColumnName.Length > 30)
                    col.Width = 180;
                else if (strColumnName.Length > 20)
                    col.Width = 150;
                else if (strColumnName.Length > 10)
                    col.Width = 130;
                else
                    col.Width = 100;

                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
                col.DefaultCellStyle.Format = "N2";
                dgrdLedger.Columns.Add(col);
            }

            if (_bPStatus)
            {
                string[] str = { "InvoiceNo", "InvoiceDate", "NetAmount", "Remark", "GrossAmount", "TaxAmount", "DisAmount", "OtherAmt", "DDays" };
                string[] strHeader = { "Invoice No", "Invoice Date", "Net Amt", "Remark", "Gross Amt", "Tax Amt", "Dis Amt", "Other Amt", "D.Days" };
                int _index = 0;
                foreach (string _strColumnName in str)
                {
                    col = new DataGridViewColumn();
                    cell = new DataGridViewTextBoxCell();
                    col.CellTemplate = cell;
                    col.HeaderText = strHeader[_index];
                    col.Name = _strColumnName;
                    col.Visible = true;
                    if (_strColumnName.Length > 30)
                        col.Width = 180;
                    else if (_strColumnName.Length > 20)
                        col.Width = 150;
                    else if (_strColumnName.Length > 12)
                        col.Width = 130;
                    else
                        col.Width = 100;
                    if (strHeader[_index].Contains("Remark"))
                        col.Width = 180;
                    if (str[_index].Contains("DDays"))
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        col.Width = 50;
                    }

                    if (strHeader[_index].Contains("Amt"))
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        col.DefaultCellStyle.Format = "N2";
                    }
                    col.SortMode = DataGridViewColumnSortMode.Automatic;

                    dgrdLedger.Columns.Add(col);
                    _index++;
                }
            }

            col = new DataGridViewColumn();
            cell = new DataGridViewTextBoxCell();
            col.CellTemplate = cell;
            col.HeaderText = "Balance Amt";
            col.Name = "balanceAmt";
            col.Visible = true;
            col.Width = 140;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgrdLedger.Columns.Add(col);

            col = new DataGridViewColumn();
            cell = new DataGridViewTextBoxCell();
            col.CellTemplate = cell;
            col.HeaderText = "Created By";
            col.Name = "createdBy";
            col.Visible = true;
            col.Width = 100;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgrdLedger.Columns.Add(col);

            col = new DataGridViewColumn();
            cell = new DataGridViewTextBoxCell();
            col.CellTemplate = cell;
            col.HeaderText = "Updated By";
            col.Name = "updatedBy";
            col.Visible = true;
            col.Width = 100;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgrdLedger.Columns.Add(col);

            BindColumnSettingData();

        }

        private void BindDataWithGrid(DataTable table, DataTable _dtColumn,bool _bPStatus)
        {
            int rowIndex = 0, colorIndex = 0;
            string strCIndex = "", strColumnName = "";
            AddGridColumn(_dtColumn, _bPStatus);
            if (table.Rows.Count > 0)
                dgrdLedger.Rows.Add(table.Rows.Count);
            double dAmt = 0, dBalanceAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dTotalAmt = 0, dNetBalanceAmt = 0;
            foreach (DataRow row in table.Rows)
            {
                dgrdLedger.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                dgrdLedger.Rows[rowIndex].Cells["voucherCode"].Value = row["VoucherCode"];
                //dgrdLedger.Rows[rowIndex].Cells["voucherType"].Value = row["VoucherType"];
                dgrdLedger.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                dgrdLedger.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];

                foreach (DataRow _row in _dtColumn.Rows)
                {
                    strColumnName = Convert.ToString(_row["PartyName"]);
                    dBalanceAmt += dAmt = dba.ConvertObjectToDouble(row[strColumnName]);
                    if (dAmt >= 0)
                        dgrdLedger.Rows[rowIndex].Cells[strColumnName].Value = dAmt.ToString("N2",MainPage.indianCurancy);
                    else
                        dgrdLedger.Rows[rowIndex].Cells[strColumnName].Value = "(" + Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + ")";
                }
                if(_bPStatus)
                {
                    string[] str = { "InvoiceNo", "InvoiceDate","DDays","Remark", "GrossAmount", "TaxAmount", "DisAmount", "OtherAmt", "NetAmount" };
                    foreach (string _str in str)
                    {
                        if (_str.Contains("Amt") || _str.Contains("Amount"))
                            dgrdLedger.Rows[rowIndex].Cells[_str].Value = dba.ConvertObjectToDouble(row[_str]).ToString("N2", MainPage.indianCurancy);
                        else
                            dgrdLedger.Rows[rowIndex].Cells[_str].Value = row[_str];
                    }

                }
                if (dBalanceAmt >= 0)
                    dDebitAmt += dBalanceAmt;
                else
                    dCreditAmt += dBalanceAmt;

                dNetBalanceAmt += dBalanceAmt;

                if (dNetBalanceAmt >= 0)
                    dgrdLedger.Rows[rowIndex].Cells["balanceAmt"].Value = dNetBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else
                    dgrdLedger.Rows[rowIndex].Cells["balanceAmt"].Value = Math.Abs(dNetBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";


                dBalanceAmt = 0;
                rowIndex++;
            }


            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = Math.Abs(dCreditAmt).ToString("N2", MainPage.indianCurancy);
            dTotalAmt = dDebitAmt + dCreditAmt;

            if (dTotalAmt > 0)
               lblBalAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else if (dTotalAmt < 0)
                lblBalAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            else
                 lblBalAmount.Text = "0";
        }

        private void LedgerAccount_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                dba.EnableCopyOnClipBoard(dgrdLedger);

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
                    if(txtParty.Text!="")
                    {
                        SearchRecord();
                    }
                }
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

        #region Delete Ledger Entry

        private bool ValidateAdjustementOfBalanceID(string strVCode, bool iStatus)
        {
            if (!iStatus)
            {
                iStatus = DataBaseAccess.CheckAmountAdjustmentByVCode(strVCode);
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
                    lblBalAmount.Text =  lblCredit.Text = lblDebit.Text = "0.00";

                    if (btnSelectCompany.Enabled)
                        GetMultiQuarterDetails();
                    else
                        GetCurrentQuarterDetails();
                    panelCompany.Visible = panelSearch.Visible = false;
                }
            }
            catch
            {
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
                    int rowIndex = 0;
                    string[] sFolder = Directory.GetDirectories(strPath);
                    DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                    foreach (string folderName in sFolder)
                    {
                        string[] strFile = Directory.GetFiles(folderName, "*.syber");
                        if (strFile.Length > 0)
                        {
                            FileInfo objFile = new FileInfo(folderName);
                            DataTable dt = dba.GetMultiCompanyNameAndFinDate(objFile.Name);
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
                                if (MainPage.multiQSDate > sDate)
                                    MainPage.multiQSDate = sDate;
                                if (MainPage.multiQEDate < eDate)
                                    MainPage.multiQEDate = eDate;
                                rowIndex++;
                            }
                        }
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
                string strFirstQuery = "", strOtherQuery = "", strChqStatus = "", strInvQuery = "", strAccountID = "", strSubQuery = CreateQuery(ref strChqStatus, ref strInvQuery, ref strAccountID), strOpeningQuery = "", strCompanyCode = "";
                strOpeningQuery = " Select 0 as ID,'' as BalanceID,Date,AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt,BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,'' as CHQStatus,NULL as ChqDate, BA.Date as BillDate from BalanceAmount BA OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID OUTER APPLY (Select Date as BillDate)PR  Where AccountStatus='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " Union All  ";

                DataTable table = null;

                strFirstQuery += "Select BalanceID,Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,AdjustedNumber,MultiCompanyNo,UserName,JournalID,CHQStatus,0 Onaccount,ISNULL(ChqDate,'')ChqDate,(CASE WHEN ChqDate is NULL or ChqDate='' then Date else Convert(datetime,ChqDate,103) end)ChequeDate from ( "
                              + strOpeningQuery
                              + " Select 1 as ID,'' as BalanceID, Date,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' then AccountStatus else dbo.GetFullName(AccountStatusID) end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end))AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, " //(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' then AccountStatus else dbo.GetFullName(AccountStatusID) end
                              + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN (VoucherCode!='' and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN (VoucherCode!='' and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,Convert(varchar,ChqDate,103) ChqDate,BillDate from BalanceAmount BA OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID " + strInvQuery + " Where AccountStatus!='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " ) Balance Order By ID,Date";

                strOtherQuery += "Select BalanceID,Date, CONVERT(varchar,Date,103)BDate,UPPER(AccountStatus)AccountStatus,Description,DebitAmt,CreditAmt,Tick,CreatedBy,UpdatedBy,AdjustedNumber,MultiCompanyNo,UserName,JournalID,CHQStatus, 0 as Onaccount,ISNULL(ChqDate,'')ChqDate,(CASE WHEN ChqDate is NULL or ChqDate='' then Date else Convert(datetime,ChqDate,103) end)ChequeDate from ( "
                             + " Select 1 as ID,'' as BalanceID, Date,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' then AccountStatus else dbo.GetFullName(AccountStatusID) end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) else '' end))  AccountStatus,Description,(Case when Status='Debit' then Amount else '' end) DebitAmt,(Case when Status='Credit' then Amount else '' end) CreditAmt, "//(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' then AccountStatus else dbo.GetFullName(AccountStatusID) end
                             + " BA.Tick,BA.UserName CreatedBy,UpdatedBy,AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName,JournalID,(CASE WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=1) then 'CLEAR' WHEN ((VoucherCode!='' OR JournalID!='')  and (Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ISNULL(ChequeStatus,0)=0) Then 'UNCLEAR' else '' end) CHQStatus,Convert(varchar,ChqDate,103) ChqDate,BillDate from BalanceAmount BA OUTER APPLY (Select TOP 1 AID.AdjustedNumber,AID.MultiCompanyNo,AID.UserName from AdjustedIds AID Where BA.BalanceID=AID.BalanceID and AID.DataBaseName='[DBNAME]')AID " + strInvQuery + " Where AccountStatus!='OPENING' and CAST(Amount as Money)>0 " + strSubQuery + " ) Balance Order By ID,Date";

                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                        if (strCompanyCode != "")
                        {
                            DataTable dt = null;
                            if (rowCount == 0)
                                table = dba.GetMultiQuarterDataTable(strFirstQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);
                            else
                            {
                                dt = dba.GetMultiQuarterDataTable(strOtherQuery.Replace("[DBNAME]", strCompanyCode), strCompanyCode);
                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);
                            }
                            rowCount++;
                        }
                    }
                }

                if (table != null)
                    SetRecordWithDataTable(table);
            }
            catch
            {
            }
        }

        #endregion

        private void dgrdLedger_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
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
            DateTime ledgerDate = dba.ConvertDateInExactFormat(Convert.ToString(dgrdLedger.CurrentRow.Cells["date"].Value));
            if (ledgerDate >= MainPage.startFinDate && ledgerDate < MainPage.endFinDate)
            {
                string strAccount = Convert.ToString(dgrdLedger.CurrentRow.Cells["voucherType"].Value).ToUpper(), strInvoiceNo = Convert.ToString(dgrdLedger.CurrentRow.Cells["voucherCode"].Value);
                string[] strNumber = strInvoiceNo.Split(' ');
                if (strNumber.Length > 1)
                {
                    if (strAccount == "PURCHASE A/C")
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
                    else if (strAccount == "SALES A/C")
                    {
                        string str = dba.GetSalesRecordType(strNumber[0], strNumber[1]);
                        if (str == "")
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                dba.ShowSaleBookPrint(strNumber[0], strNumber[1],false, false);
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
                    else if (strAccount == "SALE RETURN")
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
                    else if (strAccount == "PURCHASE RETURN")
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
                    else if (strAccount == "SALE SERVICE")
                    {
                        SaleServiceBook objSale = new SaleServiceBook(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                    else if (strAccount == "CREDIT NOTE")
                    {
                        CreditNote_Supplier objSale = new CreditNote_Supplier(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                    else if (strAccount == "DEBIT NOTE")
                    {
                        DebitNote_Customer objDebitNote = new DebitNote_Customer(strNumber[0], strNumber[1]);
                        objDebitNote.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objDebitNote.ShowInTaskbar = true;
                        objDebitNote.Show();
                    }
                    else if (strAccount == "JOURNAL A/C")
                    {
                        JournalEntry_New objJournalEntry = new JournalEntry_New(strNumber[0], strNumber[1]);
                        objJournalEntry.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        objJournalEntry.ShowInTaskbar = true;
                        objJournalEntry.Show();
                    }
                    else if (strAccount == "CASH A/C")
                    {
                        CashBook objCashBook = new CashBook(strNumber[0].Trim(), strNumber[1].Trim());
                        objCashBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        objCashBook.ShowInTaskbar = true;
                        objCashBook.Show();
                    }
                    else if (strAccount == "BANK A/C")
                    {
                        BankBook objBankBook = new BankBook(strNumber[0].Trim(), strNumber[1].Trim());
                        objBankBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        objBankBook.ShowInTaskbar = true;
                        objBankBook.Show();
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
                    if (dgrdLedger.CurrentCell.ColumnIndex == 3 && dgrdLedger.CurrentCell.RowIndex>=0)
                    {
                        ShowDetails();
                    }
                }
            }
            catch
            {
            }
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    PrintLedger();
        //}

        //private void PrintLedger()
        //{
        //    try
        //    {
        //        if (dgrdLedger.Rows.Count > 0)
        //        {
        //            DataTable dt = CreateDataTableForPrint();
        //            //if (rdoNo.Checked)
        //            //{
        //            //    Reporting.LedgerReport_New report = new SSS.Reporting.LedgerReport_New();
        //            //    report.SetDataSource(dt);
        //            //    report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        //            //    report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //            //    report.PrintToPrinter(1, false, 0, 0);
        //            //}
        //            //else
        //            //{
        //            if (dt.Rows.Count > 0)
        //            {
        //                Reporting.LedgerReport_New report = new SSS.Reporting.LedgerReport_New();
        //                report.SetDataSource(dt);
        //                if (MainPage._PrintWithDialog)
        //                    dba.PrintWithDialog(report);
        //                else
        //                {
        //                    report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        //                    report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                    report.PrintToPrinter(1, false, 0, 0);
        //                }

        //                report.Close();
        //                report.Dispose();
        //            }
        //            //}
        //            //if (index > -1)
        //            //{
        //            //    if (index == strAllParty.Length)
        //            //    {
        //            //        btnPrint.Enabled = false;
        //            //    }
        //            //    else
        //            //    {
        //            //        BindMultiLedgerAccount();
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindMultiLedgerAccount()
        //{
        //    if (strAllParty.Length > 0 && index<strPartyStatus.Length)
        //    {
        //        txtParty.Text = strAllParty[index];
        //        SetStatus();
        //        index++;
        //        GetCurrentQuarterDetails();
        //    }
        //}

        //private void SetStatus()
        //{
        //    if (strPartyStatus[index] == "All")
        //        rdoAll.Checked = true;
        //    else if (strPartyStatus[index] == "True")
        //        rdoTick.Checked = true;
        //    else if (strPartyStatus[index] == "False")
        //        rdoUnTick.Checked = true;
        //}

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdLedger.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTableForPrint();
                    if (dt.Rows.Count > 0)
                    {
                        SSS.Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Detailed Ledger Report Preview");
                        SSS.Reporting.LedgerReport_Detailed objReport = new Reporting.LedgerReport_Detailed();
                        objReport.SetDataSource(dt);
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }

                }
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTableForPrint()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("FirmName", typeof(String));
                myDataTable.Columns.Add("OnAccount", typeof(String));

                myDataTable.Columns.Add("Column1", typeof(String));
                myDataTable.Columns.Add("Column2", typeof(String));
                myDataTable.Columns.Add("Column3", typeof(String));
                myDataTable.Columns.Add("Column4", typeof(String));
                myDataTable.Columns.Add("Column5", typeof(String));
                myDataTable.Columns.Add("Column6", typeof(String));
                myDataTable.Columns.Add("Column7", typeof(String));

                myDataTable.Columns.Add("Column1V", typeof(String));
                myDataTable.Columns.Add("Column2V", typeof(String));
                myDataTable.Columns.Add("Column3V", typeof(String));
                myDataTable.Columns.Add("Column4V", typeof(String));
                myDataTable.Columns.Add("Column5V", typeof(String));
                myDataTable.Columns.Add("Column6V", typeof(String));
                myDataTable.Columns.Add("Column7V", typeof(String));

                myDataTable.Columns.Add("TotalDebit", typeof(String));
                myDataTable.Columns.Add("TotalCredit", typeof(String));
                myDataTable.Columns.Add("TotalBalance", typeof(String));

                myDataTable.Columns.Add("AmountInWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));

                myDataTable.Columns.Add("Column8", typeof(String));
                myDataTable.Columns.Add("Column9", typeof(String));
                myDataTable.Columns.Add("Column10", typeof(String));
                myDataTable.Columns.Add("Column11", typeof(String));
                myDataTable.Columns.Add("Column8V", typeof(String));
                myDataTable.Columns.Add("Column9V", typeof(String));
                myDataTable.Columns.Add("Column10V", typeof(String));
                myDataTable.Columns.Add("Column11V", typeof(String));


                string strNumeric = "Zero",strDate="";
                if (lblBalAmount.Text.Contains("Cr") || lblBalAmount.Text.Contains("Dr"))                
                    strNumeric = currency.changeCurrencyToWords(Convert.ToDouble(lblBalAmount.Text.Substring(0, lblBalAmount.Text.Length - 3)));
                if (chkDate.Checked && txtFromDate.Text != "" && txtToDate.Text != "")
                    strDate = "Date Period : From "+txtFromDate.Text + " To " + txtToDate.Text;
                else
                    strDate = "Date Period : From " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    try
                    {
                        DataRow dRow = myDataTable.NewRow();

                        dRow["CompanyName"] = MainPage.strPrintComapanyName;
                        dRow["BrandLogo"] = MainPage._brandLogo;
                        dRow["HeaderImage"] = MainPage._headerImage;
                        dRow["PartyName"] = txtParty.Text;
                        dRow["OnAccount"] = "0";
                        dRow["DatePeriod"] = strDate;

                        int index = 1;
                        foreach (string c in displaydClms)
                        {
                            if (index <= 11)
                            {
                                string[] clm = c.Split('>');

                                dRow["Column" + index] = clm[1];
                                if (clm[0].ToLower().Contains("date"))
                                    dRow["Column" + index + "V"] = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells[clm[0]].Value)).ToString("dd/MM/yyyy");
                                else
                                    dRow["Column" + index + "V"] = row.Cells[clm[0]].Value;
                            }
                            else
                            {
                                break;
                                //dRow["Column" + index] = "NA";
                                //dRow["Column" + index + "V"] = "";
                            }
                            index++;
                        }
                        
                        dRow["TotalDebit"] = lblDebit.Text;
                        dRow["TotalCredit"] = lblCredit.Text;
                        dRow["TotalBalance"] = lblBalAmount.Text;
                        dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        if (lblBalAmount.Text.Contains("Cr"))
                        {
                            dRow["AmountInWord"] = strNumeric + " Credit";
                        }
                        else if (lblBalAmount.Text.Contains("Dr"))
                        {
                            dRow["AmountInWord"] = strNumeric + " Debit";
                        }
                        else
                            dRow["AmountInWord"] = strNumeric;

                        myDataTable.Rows.Add(dRow);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Sorry ! " + ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    }
                }

                if (myDataTable.Rows.Count > 0)
                {
                    DataTable dt = dba.GetDataTable("Select SM.GroupName, (SM.Address + ', '+SM.Station+', '+SM.State+'-'+SM.PinCode)Address,(SM.MobileNo+ ' '+SM.PhoneNo)PhoneNo,SM.AccountNo,CD.* from SupplierMaster SM Outer Apply (Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD  Order by CD.ID asc) CD Where (SM.AreaCode+SM.AccountNo+' '+SM.Name)='" + txtParty.Text + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow _row = dt.Rows[0];
                        //myDataTable.Rows[0]["Address"] = _row["Address"];
                        //myDataTable.Rows[0]["PhoneNo"] = _row["PhoneNo"];
                        myDataTable.Rows[0]["FirmName"] = _row["GroupName"];

                        myDataTable.Rows[0]["CompanyAddress"] = _row["CompanyAddress"];
                        myDataTable.Rows[0]["CompanyEmail"] = _row["CompanyPhoneNo"];
                        myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                        if (Convert.ToString(_row["CINNumber"]) != "")
                            myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];
                        //if (Convert.ToString(_row["GroupName"]) == "SUNDRY DEBTORS")
                        //{
                        //    myDataTable.Rows[0]["BankName"] = "ICICI BANK";
                        //    myDataTable.Rows[0]["BranchName"] = "DELHI";
                        //    myDataTable.Rows[0]["AccountNo"] = "SASUSP" + dba.ConvertObjectToDouble(_row["AccountNo"]).ToString("000000");
                        //    myDataTable.Rows[0]["IFSCCode"] = "ICIC0000106";
                        //}
                        //else
                        //{

                        //    myDataTable.Rows[0]["BankName"] = myDataTable.Rows[0]["BranchName"] = myDataTable.Rows[0]["AccountNo"] = myDataTable.Rows[0]["IFSCCode"] = "N/A";
                        //}
                    }
                    else
                        myDataTable.Rows.Clear();

                    //int _rowIndex = myDataTable.Rows.Count - 1;
                    //if (strPartyDetail[7] == "SUNDRY DEBTORS" && strPartyDetail[8] != "")
                    //{
                    //    myDataTable.Rows[_rowIndex]["BankName"] = "BANK NAME : ICICI BANK";
                    //    myDataTable.Rows[_rowIndex]["BranchName"] = "BRANCH NAME : DELHI,                    IFSC CODE : ICIC0000106";
                    //    myDataTable.Rows[_rowIndex]["AccountNo"] = "BANK ACCOUNT NO. : SASUSP" + dba.ConvertObjectToDouble(strPartyDetail[8]).ToString("000000");
                    //    myDataTable.Rows[_rowIndex]["IFSCCode"] = "ACCOUNT NAME : SARAOGI SUPER SALES PVT LTD";
                    //    myDataTable.Rows[_rowIndex]["FirmName"] = "(This account no. is only for you, Each party have their unique account no.)";
                    //}
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
                    string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT, 1 OR 2 ! ", "Number of Copies", "2", 400, 300);
                    if (strValue != "" && strValue != "0")
                    {
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
                                            Reporting.CashReceiptReport report = new SSS.Reporting.CashReceiptReport();
                                            report.SetDataSource(dt);
                                            if (MainPage._PrintWithDialog)
                                                dba.PrintWithDialog(report);
                                            else
                                            {
                                                report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                                                report.PrintToPrinter(1, false, 0, 1);
                                            }
                                            if (strValue == "2")
                                            {
                                                dt.Clear();
                                                dt = CreateOfficeReceiptDataTable(rows, strVoucherCode);
                                                report.SetDataSource(dt);
                                                if (MainPage._PrintWithDialog)
                                                    dba.PrintWithDialog(report,true);
                                                else
                                                {
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


                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = MainPage.strGRCompanyName;
                row["VoucherNo"] = strName[1] + "/D";
                row["Date"] = Convert.ToDateTime(dgRow.Cells["date"].Value).ToString("dd/MM/yy");
                row["Description"] = dgRow.Cells[4].Value;
                row["CastType"] = "CONSIGNEE COPY";
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


                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = MainPage.strGRCompanyName;
                row["VoucherNo"] = strName[1] + "/D";
                row["Date"] = Convert.ToDateTime(dgRow.Cells["date"].Value).ToString("dd/MM/yy");
                row["Description"] = dgRow.Cells["desc"].Value;
                row["CastType"] = "OFFICE COPY";
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
                row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                if (dgrdLedger.Rows.Count > 0)
                {
                    CreateNormalExcel();
                }
                else
                {
                    MessageBox.Show("There is no record for Exporting ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
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
                double dDebitAmt = 0, dCreditAmt = 0, dTotalAmt=0,dAmt=0;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
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

                lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                if (dTotalAmt > 0)
                    lblBalAmount.Text = lblBalAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dTotalAmt < 0)
                    lblBalAmount.Text = lblBalAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalAmount.Text = lblBalAmount.Text = "0";
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
        
        private void txtVCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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
               

        private void dgrdLedger_Scroll(object sender, ScrollEventArgs e)
        {
           
        }
        
        private void txtMonthName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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
            //try
            //{
            //    if (e.ColumnIndex == 10)
            //    {
            //        string strDate = Convert.ToString(dgrdLedger.CurrentCell.EditedFormattedValue);
            //        if (strDate != "")
            //        {
            //            strDate = strDate.Replace("/", "");
            //            if (strDate.Length == 8)
            //            {
            //                TextBox txtDate = new TextBox();
            //                txtDate.Text = strDate;
            //                dba.GetStringFromDateForReporting(txtDate,false);
            //                if (!txtDate.Text.Contains("/"))
            //                {
            //                    e.Cancel = true;
            //                }
            //                else
            //                {
            //                    if (e.RowIndex < dgrdLedger.Rows.Count - 1)
            //                    {
            //                        dgrdLedger.EndEdit();
            //                    }
            //                }
            //                dgrdLedger.CurrentCell.Value = txtDate.Text;
            //            }
            //            else
            //            {
            //                MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                e.Cancel = true;
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //}
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
                    if (strDate.Length==8)
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate,false);
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
                btnPartyName.Enabled = false;
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                    txtParty.Text = objSearch.strSelectedData;
                ClearRecord();
            }
            catch { }
            btnPartyName.Enabled = true;
        }

        private void txtParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtParty.Text);
        }
        
        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtParty.Text != "")
                {
                    string strMessage = "", strMobileNo = "", strBalance = lblBalAmount.Text.Replace(",", ""), strAccountNo = "" ;
                    string[] strFullName = txtParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strAccountNo = strFullName[0];
                        object objMobile = DataBaseAccess.ExecuteMyScalar("Select  MobileNo from SupplierMaster Where (AreaCode+AccountNo)='" + strAccountNo + "' ");
                        strMobileNo = Convert.ToString(objMobile);
                        if (strMobileNo != "")
                        {
                            strAccountNo= System.Text.RegularExpressions.Regex.Replace(strAccountNo, "[^0-9]", "");

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

        private string CreateNormalExcel()
        {
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            try
            {
                object misValue = System.Reflection.Missing.Value;
                ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
                ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
                ExcelWorkSheet.Name = "LEDGER_DETAILS";
                string strDate = "";
                if (chkDate.Checked && txtFromDate.Text != "" && txtToDate.Text != "")
                    strDate = "Date Period : From " + txtFromDate.Text + " To " + txtToDate.Text;
                else
                    strDate = "Date Period : From " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");


                ExcelWorkSheet.Cells[1, 1] = "LEDGER DETAILS OF "+txtParty.Text+", "+ strDate;
                ExcelWorkSheet.Range[ExcelWorkSheet.Cells[1, 1], ExcelWorkSheet.Cells[1, dgrdLedger.Columns.Count-2]].Merge();
                NewExcel.Range _objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 1];
                _objRange.Font.Bold = true;
                _objRange.HorizontalAlignment = HorizontalAlignment.Center;
                _objRange.RowHeight = 18;

                int colIndex = 1;
                foreach (DataGridViewColumn column in dgrdLedger.Columns)
                {
                    if (colIndex < dgrdLedger.Columns.Count - 1)
                    {                        
                            ExcelWorkSheet.Cells[2, colIndex] = column.HeaderText;
                        colIndex++;
                    }
                    else
                        break;
                }

                int _colWidth = 0;
                int columnIndex = 1;
                foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
                {
                    column.HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;
                    _colWidth = dgrdLedger.Columns[columnIndex + 1].Width;
                    if (_colWidth > 149)
                        column.ColumnWidth = (double)column.ColumnWidth + 16;
                    else if (_colWidth > 119)
                        column.ColumnWidth = (double)column.ColumnWidth + 10;
                    else if (_colWidth > 99)
                        column.ColumnWidth = (double)column.ColumnWidth + 7;
                    else if (_colWidth > 50)
                        column.ColumnWidth = (double)column.ColumnWidth;
                    else
                        column.ColumnWidth = (double)column.ColumnWidth - 2;
                    column.RowHeight = 15;

                    if (columnIndex + 1 > colIndex - 1)
                        break;
                    columnIndex++;
                }

                int rowIndex = 3;
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    for (int col = 0; col < dgrdLedger.Columns.Count-2; col++)
                    {
                        ExcelWorkSheet.Cells[rowIndex, col+1] = row.Cells[col].Value;
                    }
                    rowIndex++;
                }

                for (int cIndex = 1; cIndex < dgrdLedger.Columns.Count - 1; cIndex++)
                {
                    NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[2, cIndex];
                    objRange.Font.Bold = true;
                    objRange.Interior.ColorIndex = 22;                   
                }              

                for (int rIndex = 2; rIndex < rowIndex; rIndex++)
                {
                    for (int cIndex = 1; cIndex < dgrdLedger.Columns.Count - 1; cIndex++)
                    {
                        NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
                        objRange.Cells.BorderAround();
                    }
                }

                ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                ExcelWorkBook.Close(true, misValue, misValue);
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);

                MessageBox.Show("Thank you ! Details exported successfully !! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                strFileName = ex.Message;
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //foreach (Process process in Process.GetProcessesByName("Excel"))
                //    process.Kill();
            }
            return strFileName;
        }

        private void BindColumnSettingData()
        {
            try
            {
                int _rowIndex = 0;
                dgrdColumnSetting.Rows.Clear();
                if (dgrdLedger.Columns.Count > 0)
                {
                    dgrdColumnSetting.Rows.Add(dgrdLedger.Columns.Count);
                    //string strColumnName = "", strHeaderName = "";
                    foreach (DataGridViewColumn _column in dgrdLedger.Columns)
                    {
                        dgrdColumnSetting.Rows[_rowIndex].Cells["columnName"].Value = _column.HeaderText;
                        dgrdColumnSetting.Rows[_rowIndex].Cells["colName"].Value = _column.Name;
                        dgrdColumnSetting.Rows[_rowIndex].Cells["colIndex"].Value = _rowIndex + 1;
                        _rowIndex++;
                    }
                }
                RearrangeColumn();
            }
            catch { }
        }

        private void btnColumnSetting_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = !panalColumnSetting.Visible ;
        }

        private void dgrdColumnSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                RearrangeColumn();
            }
        }

        private void RearrangeColumn()
        {
            try
            {
                int _index = 0, dIndex = 1;
                string strColumn = "";
                DataTable _dt = new DataTable();
                _dt.Columns.Add("ColumnName", typeof(string));
                _dt.Columns.Add("ColumnIndex", typeof(Int32));

                foreach (DataGridViewRow row in dgrdColumnSetting.Rows)
                {
                    DataRow _dr = _dt.NewRow();
                    _dr["ColumnName"] = Convert.ToString(row.Cells["colName"].Value);
                    _dr["ColumnIndex"] = dba.ConvertObjectToInt(row.Cells["colIndex"].Value);
                    _dt.Rows.Add(_dr);
                }
                DataView dv = _dt.DefaultView;
                dv.Sort = "ColumnIndex asc";
                _dt = dv.ToTable();

                displaydClms.Clear();
                dIndex = 1;// dgrdLedger.Columns[dgrdLedger.Columns.Count - 1].DisplayIndex;
                foreach (DataRow row in _dt.Rows)
                {
                    _index = dba.ConvertObjectToInt(row["ColumnIndex"]);
                    strColumn = Convert.ToString(row["ColumnName"]);
                    if (_index == 0)
                    {
                        dgrdLedger.Columns[strColumn].Visible = false;
                    }
                    else
                    {
                        dgrdLedger.Columns[strColumn].Visible = true;
                        dgrdLedger.Columns[strColumn].DisplayIndex = dIndex;
                        displaydClms.Add(dgrdLedger.Columns[strColumn].Name+">"+ dgrdLedger.Columns[strColumn].HeaderText);

                        dIndex++;
                    }
                  
                }
            }
            catch { }
        }

        private void dgrdColumnSetting_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                e.Cancel = true;
            }
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdLedger.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTableForPrint();
                    if (dt.Rows.Count > 0)
                    {
                        SSS.Reporting.LedgerReport_Detailed objReport = new Reporting.LedgerReport_Detailed();
                        objReport.SetDataSource(dt);
                        objReport.PrintToPrinter(1, false, 0, 0);

                        objReport.Close();
                        objReport.Dispose();
                    }

                }
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
            btnPrint.Enabled = true;
        }

        private string GetFileName()
        {           
            string strPath = "", strFileName = "";
            if (txtParty.Text != "")
                strFileName = txtParty.Text.Replace(" ", "_").Replace(".", "_").Replace("(", "_").Replace(")", "_").Replace(":", "").Replace("-", "_").Replace("&", "AND").Replace(",", "").Replace("/", "_");
            else
                strFileName = "Ledger_Detail_Statement";

            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xls)|*.xls;";
            _browser.FileName = strFileName + ".xls";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;
            try
            {
                FileInfo file = new FileInfo(strPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch
            {
            }
            return strPath;
        }

    }
}
