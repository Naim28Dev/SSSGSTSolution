using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class ShowCurrentLedgerBalance : Form
    {
        SendSMS objSMS;
        DataBaseAccess dba;

        public ShowCurrentLedgerBalance()
        {
            InitializeComponent();
            objSMS = new SendSMS();
            dba = new DataBaseAccess();
            txtLastDate.Text = MainPage.strCurrentDate;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                dgrdParty.Rows.Clear();
                btnGo.Enabled = false;
                GetPartyRecord();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery(ref string strSQuery)
        {
            string strQuery = "";

            if (txtGroupName.Text != "")
                strQuery += " and GroupName='" + txtGroupName.Text + "' ";
            if (txtAmount.Text != "")
            {
                if (rdoDebit.Checked)
                    strSQuery += " and Amount>" + txtAmount.Text;
                else if (rdoCredit.Checked)
                    strSQuery += " and (Amount*-1)>" + txtAmount.Text;
                else
                    strSQuery += " and (Amount>" + txtAmount.Text + " OR (Amount*-1)>" + txtAmount.Text+")";
            }
            if(txtDueAmt.Text!="")
            {
                double dDAmt = dba.ConvertObjectToDouble(txtDueAmt.Text);
                strSQuery += " and DAmt>" + dDAmt;
            }
            if (txtLastDate.Text.Length == 10)
            {
                DateTime eDate = dba.ConvertDateInExactFormat(txtLastDate.Text);
                strQuery += " and BA.Date<='" + eDate.ToString("MM/dd/yyyy") + "' ";
            }
            return strQuery;
        }

        //private string GetAvgDaysQuery()
        //{
        //    string strQuery = "0";
        //    if (chkShowAvgDays.Checked)
        //    {
        //        strQuery = " (Select (CASE WHEN SaleAmt>0 and GroupName='SUNDRY DEBTORS' then ((IntAmt*DaysInYear*100)/(SaleAmt*DrInterest)) else 0 end) AvgDays from ( "
        //                    + " Select SUM((CASE WHEN AccountStatus = 'SALES A/C' then Amt else 0 end)) SaleAmt,SUM(IntAmt) IntAmt,AVG(CAST(DrInterest as int))DrInterest,AVG(DaysInYear) DaysInYear from ( "
        //                    + " Select *, (((Duedays * Amt) * DrInterest) / (DaysInYear * 100)) IntAmt from( "
        //                    + " Select Date, AccountStatus, Amt, (DATEDIFF(dd, (CASE WHEN AccountStatus = 'SALES A/C' then DATEADD(dd, GraceDays, Date) else Date end), DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) + 1) DueDays,Category,DaysInYear,DrInterest,CrInterest,CashDiscRate,CashDiscDays from ( "
        //                    + " Select DATEADD(dd, GDays, Date) Date, AccountStatus, Amt, (Case When(DueDays != '' AND DueDays != '0') then DueDays else GraceDays end) GraceDays,Category,DaysInYear,DrInterest,CrInterest,CashDiscRate,CashDiscDays from ( "
        //                    + " Select  Date,AccountStatus,(ISNULL(CAST(Amount as Money),0))Amt,(CASE WHEN AccountStatus='SALES A/C' then ISNULL((Select Top 1 GDM.BuyerDays as GDays from SalesRecord SR CROSS APPLY (Select GRSNO from SalesEntry SE Where SR.BIllCode=SE.BillCode and SR.BillNo=SE.BillNo)SE Cross Apply(Select OrderNo,SalePartyID from GoodsReceive GR  Where SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)))GR Cross Apply (Select OfferName,SalePartyID as OBSalePartyID from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo)OB Cross Apply (Select BuyerDays from GraceDaysMaster GDM Where GDM.OfferName=OB.OfferName) GDM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BA.Description and GR.SalePartyID=Supplier.AccountID and OB.OBSalePartyID=Supplier.AccountID and SR.SalePartyID=Supplier.AccountID),0) else 0 end) GDays  from BalanceAmount BA  Where Status='DEBIT' and Tick='FALSE' and BA.AccountID=Supplier.AccountID  Union All "
        //                    + " Select Date, AccountStatus,-(ISNULL(CAST(Amount as Money), 0))Amt,0 GDays from BalanceAmount BA  Where Status = 'Credit' and Tick = 'FALSE'  and BA.AccountID = Supplier.AccountID "
        //                    + " )Balance Outer Apply(Select Category, DueDays from SupplierMaster Where (ISNULL(AreaCode, '') + ISNULL(AccountNo, '')) = Supplier.AccountID ) SM OUTER APPLY(Select Top 1 DaysInYear, DrInterest, CrInterest, CashDiscRate, CashDiscDays, GraceDays from CompanySetting) CS "
        //                    + " )_Balance)_Balance)_Balance )_Balance) ";
        //    }

        //    return strQuery;
        //}

        private void GetPartyRecord()
        {
            try
            {
                string strQuery = "", strSQuery = "", strSubQuery = CreateQuery(ref strSQuery);
              
                //strQuery = "Select (AccountID+ ' '+Name) Name,GroupName,MobileNo,Station,Amount," + GetAvgDaysQuery() + " as AvgDays from (Select (AreaCode+AccountNo) as AccountID ,Name,GroupName,MobileNo,Station,"
                //             + " (Select SUM(Amt) Amount from (Select ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='DEBIT' and Date<='" + eDate.ToString("MM/dd/yyyy") + "'  and AccountID=(AreaCode+CAST(AccountNo as varchar)) Union All  "
                //             + " Select -ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='CREDIT' and Date<='" + eDate.ToString("MM/dd/yyyy") + "' and AccountID=(AreaCode+CAST(AccountNo as varchar)))Bal ) Amount"
                //             + " from SupplierMaster Where GroupName!='SUB PARTY' " + strSubQuery + ") Supplier " + strSQuery + " Order By Name";

                strQuery = " Select * from ( Select PartyName,GroupName,Category,GradeName,MobileNo,MAX(DueDays)_Days, SUM(RAmt+DAmt) DAmt,SUM(BAmt) Amount,NickName from (  "
                         + " Select PartyName, GroupName, Category,GradeName,MobileNo,MAX(DueDays)DueDays, SUM((CASE WHEN  _Days >= DueDays then BAmt else 0 end)) DAmt,0 as RAmt,SUM(BAmt) BAmt,NickName from (  "
                         + " Select (BA.AccountID + ' ' + SM.Name) PartyName, SM.GroupName, SM.Category,SM.TinNumber as GradeName,MobileNo, SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) BAmt, DATEDIFF(dd,BA.Date, GetDate()) _Days,Other as NickName,(CASE WHEN DueDays!='' then DueDays  When Category='WHOLESALER' then 60 else 45 end)DueDays from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) Where AccountStatus in ('PURCHASE A/C', 'SALES A/C') "+ strSubQuery
                         + " Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo, SM.DueDays,BA.Date,Other,(CASE WHEN DueDays!='' then DueDays  When Category='WHOLESALER' then 60 else 45 end))_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo,NickName UNION ALL  "
                         + " Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo,MAX(CASE WHEN DueDays!='' then DueDays  When Category='WHOLESALER' then 60 else 45 end) _Days, 0 as DAmt,SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) RAmt,SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) BAmt,Other as NickName from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere AccountStatus not in ('PURCHASE A/C','SALES A/C') "+ strSubQuery+ " Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo,Other)_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo,NickName)_Bal Where PartyName!='' " + strSQuery+ " Order by PartyName ";
                
                DataTable dt = DataBaseAccess.GetDataTableRecord(strQuery);

                BindDataWithGrid(dt);
            }
            catch
            {
            }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            dgrdParty.Rows.Clear();
            chkAll.Checked = true;
            if (dt.Rows.Count > 0)
            {
                dgrdParty.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                double dAmt = 0,dDays=0,dDueAmt=0;
                foreach (DataRow row in dt.Rows)
                {
                    dAmt = Convert.ToDouble(row["Amount"]);
                    dDueAmt = dba.ConvertObjectToDouble(row["DAmt"]);

                    dgrdParty.Rows[rowIndex].Cells["sno"].Value = (rowIndex+1);
                    dgrdParty.Rows[rowIndex].Cells["chk"].Value = true;
                    dgrdParty.Rows[rowIndex].Cells["partyName"].Value = row["partyName"];
                    dgrdParty.Rows[rowIndex].Cells["groupName"].Value = row["GroupName"];
                    dgrdParty.Rows[rowIndex].Cells["mobileNo"].Value = row["MobileNo"];
                    dgrdParty.Rows[rowIndex].Cells["balance"].Value = dAmt;
                    if (dAmt > 0)
                    {
                        dgrdParty.Rows[rowIndex].Cells["balance"].Value = dAmt;
                        dgrdParty.Rows[rowIndex].Cells["status"].Value = "Debit";
                    }
                    else if (dAmt < 0)
                    {
                        dgrdParty.Rows[rowIndex].Cells["balance"].Value = Math.Abs(dAmt);
                        dgrdParty.Rows[rowIndex].Cells["status"].Value = "Credit";
                    }
                    else
                        dgrdParty.Rows[rowIndex].Cells["balance"].Value = dAmt;
                    dgrdParty.Rows[rowIndex].Cells["grade"].Value = row["GradeName"];
                    dgrdParty.Rows[rowIndex].Cells["dueDays"].Value = row["_Days"];
                    if (dDueAmt > 0)
                        dgrdParty.Rows[rowIndex].Cells["dueBalance"].Value = dDueAmt;
                    else
                        dgrdParty.Rows[rowIndex].Cells["dueBalance"].Value = 0.00;
                    rowIndex++;
                }
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void ShowCurrentLedgerBalance_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            //{
            //    if (dgrdParty.Rows.Count > 0)
            //    {
            //        partyPanel.Visible = true;
            //        txtName.Focus();
            //    }
            //    else
            //    {
            //        MessageBox.Show("No record found ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //else if (e.KeyCode == Keys.Escape && partyPanel.Visible)
            //{
            //    partyPanel.Visible = false;
            //    txtName.Clear();
            //}
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dgrdParty_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                e.Cancel = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendSMS.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure want to Send SMS  ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    double dDAmt = 0;
                    string strGroupName = "";
                    
                    foreach (DataGridViewRow row in dgrdParty.Rows)
                    {
                        bool status = Convert.ToBoolean(row.Cells["chk"].Value);
                        if (status)
                        {
                            strGroupName = Convert.ToString(row.Cells["groupName"].Value);
                            string strName = Convert.ToString(row.Cells["partyName"].Value), strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value), strBalance = dba.ConvertObjectToDouble(row.Cells["balance"].Value).ToString("N2",MainPage.indianCurancy)+ " " + Convert.ToString(row.Cells["status"].Value), strAvgDay = "";
                            if (strMobileNo != "" && strMobileNo.Length == 10)
                            {
                                strAvgDay = "";
                                dDAmt = dba.ConvertObjectToDouble(row.Cells["dueBalance"].Value);
                                if (dDAmt > 10000 && strGroupName == "SUNDRY DEBTORS")
                                {
                                    strAvgDay = "\nand " + row.Cells["dueDays"].Value + " days Overdue Balance is : " + dDAmt.ToString("N2", MainPage.indianCurancy) + "\nPlease keep your payment on time to avoid interest charges.";
                                }
                                string strMessage = "DEAR CUSTOMER!\nM/S " + strName + ", as on Date " + txtLastDate.Text + ", your net balance is : " + strBalance + "." + strAvgDay;
                                if (MainPage.strSoftwareType == "AGENT" && MainPage.strCompanyName.Contains("SARAOGI"))
                                    strMessage += "\nFor further query\ncontact: 7290097992, 9650064285";

                                strMessage += "\n\n" + MainPage.strPrintComapanyName;
                                objSMS.SendSingleSMS(strMessage, strMobileNo);
                            }
                        }
                    }
                    MessageBox.Show("Message sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
                MessageBox.Show("Sorry ! Please try again later ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnSendSMS.Enabled = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    row.Cells["chk"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int count = CheckPartyAvailability(txtName.Text);
                    if (count > 0)
                    {
                        foreach (DataGridViewRow row in dgrdParty.Rows)
                        {
                            string strName = Convert.ToString(row.Cells["partyName"].Value);
                            if (txtName.Text == strName)
                            {
                                dgrdParty.CurrentCell = row.Cells[0];
                                dgrdParty.FirstDisplayedCell = dgrdParty.CurrentCell;
                                row.Cells[0].Value = true;
                                txtName.Clear();
                                partyPanel.Visible = false;
                                dgrdParty.Focus();
                                break;
                            }
                        }
                        txtName.Clear();
                        partyPanel.Visible = false;
                        dgrdParty.Focus();
                    }
                }
            }
            catch
            {
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsWhiteSpace(e.KeyChar) && txtName.Text.Length < 1)
            {
                e.Handled = true;
            }
        }

        public int CheckPartyAvailability(string strParty)
        {
            int count = 0;
            try
            {
                //if (myTable != null)
                //{
                //    DataRow[] rows = myTable.Select(String.Format("Name='" + strParty + "'"));
                //    count = rows.Length;
                //}
            }
            catch
            {
            }
            return count;
        }

        private void txtLastDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, true);
        }

        private DataTable CreateDatatable()
        {
            DataTable cryDataTable = new DataTable();
            try
            {
                cryDataTable.Columns.Add("CompanyName", typeof(String));
                cryDataTable.Columns.Add("HeaderName", typeof(String));
                cryDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                cryDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                cryDataTable.Columns.Add("BalanceAmt", typeof(String));
                cryDataTable.Columns.Add("LastDate", typeof(String));
                cryDataTable.Columns.Add("PartyName", typeof(String));
                cryDataTable.Columns.Add("MobNo", typeof(String));
                cryDataTable.Columns.Add("NetBalance", typeof(String));
                cryDataTable.Columns.Add("Status", typeof(String));
                cryDataTable.Columns.Add("Station", typeof(String));
                cryDataTable.Columns.Add("AvgDays", typeof(String));
                cryDataTable.Columns.Add("GroupName", typeof(String));
                cryDataTable.Columns.Add("User", typeof(String));
                cryDataTable.Columns.Add("Address", typeof(String));
                cryDataTable.Columns.Add("Email", typeof(String));
                cryDataTable.Columns.Add("GSTNo", typeof(String));
                cryDataTable.Columns.Add("CINNo", typeof(String));

                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        DataRow dRow = cryDataTable.NewRow();
                        dRow["CompanyName"] = MainPage.strCompanyName;
                        dRow["HeaderName"] = "Current Balance Report";
                        dRow["HeaderImage"] = MainPage._headerImage;
                        dRow["BrandLogo"] = MainPage._brandLogo;
                        double dBalance = dba.ConvertObjectToDouble(txtAmount.Text);
                        if (dBalance > 0)
                            dRow["BalanceAmt"] = dBalance.ToString("N2", MainPage.indianCurancy);
                        else
                            dRow["BalanceAmt"] = "0.00";
                        dRow["LastDate"] = txtLastDate.Text;
                        dRow["PartyName"] = row.Cells["partyName"].Value;
                        dRow["MobNo"] = row.Cells["mobileNo"].Value;
                        dRow["NetBalance"] = dba.ConvertObjectToDouble(row.Cells["balance"].Value).ToString("N2",MainPage.indianCurancy);
                        dRow["Status"] = row.Cells["status"].Value;
                        dRow["Station"] = row.Cells["grade"].Value;
                        dRow["AvgDays"] = dba.ConvertObjectToDouble(row.Cells["dueBalance"].Value).ToString("N2", MainPage.indianCurancy);
                        dRow["GroupName"] = txtGroupName.Text;
                        dRow["User"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        cryDataTable.Rows.Add(dRow);
                    }
                }

                DataTable dt = dba.GetDataTable("Select TOP 1 FullCompanyName,(Address+'\n'+StateName+'-'+CAST(PinCode as varchar))CompanyAddress, ('Ph. : '+STDNo+'-'+PhoneNo +', Email : '+EmailId) CompanyPhoneNo,TinNo as CompanyTIN,StateName,('GSTIN : '+GSTNo) GSTNO,PANNo,('CIN No.: '+CINNumber) CINNO from CompanyDetails where Other='" + MainPage.strCompanyName + "'  Order by ID asc");
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    cryDataTable.Rows[0]["Address"] = dr["CompanyAddress"];
                    cryDataTable.Rows[0]["Email"] = dr["CompanyPhoneNo"];
                    cryDataTable.Rows[0]["GSTNo"] = dr["GSTNO"];
                    cryDataTable.Rows[0]["CINNo"] = dr["CINNO"];
                }

            }
            catch (Exception ex)
            { }
            return cryDataTable;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDatatable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.ShowReport objReport = new Reporting.ShowReport("Current Balance Report");
                        Reporting.CryCurrentBalanceReport objCurrentBalance = new Reporting.CryCurrentBalanceReport();
                        objCurrentBalance.SetDataSource(dt);
                        objReport.myPreview.ReportSource = objCurrentBalance;
                        objReport.Show();

                        //objCurrentBalance.Close();
                        //objCurrentBalance.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDatatable();
                    if (dt.Rows.Count > 0)
                    {                        
                        Reporting.CryCurrentBalanceReport objCurrentBalance = new global::SSS.Reporting.CryCurrentBalanceReport();
                        objCurrentBalance.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objCurrentBalance);
                        else
                        {
                            objCurrentBalance.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objCurrentBalance.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objCurrentBalance.PrintToPrinter(1, false, 0, 0);
                        }
                        btnPrint.Enabled = true;
                        objCurrentBalance.Close();
                        objCurrentBalance.Dispose();

                    }
                    else
                    {
                        MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPrint.Enabled = true;
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdParty_Scroll(object sender, ScrollEventArgs e)
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

        private void btnGroupArrow_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", Keys.Space);
                objSearch.ShowDialog();
                txtGroupName.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void ShowCurrentLedgerBalance_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdParty);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {

                    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    object misValue = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;

                    //Create Excel Sheets
                    xlSheets = ExcelApp.Sheets;
                    xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
                                   Type.Missing, Type.Missing, Type.Missing);

                    int _skipColumn = 0;
                    string strHeader = "";
                    for (int j = 1; j < dgrdParty.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdParty.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdParty.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdParty.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdParty.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdParty.Columns.Count; l++)
                        {
                            if (dgrdParty.Columns[l].HeaderText == "" || !dgrdParty.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdParty.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdParty.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Current_Balance_Report";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void dgrdParty_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdParty.Rows)
                    row.Cells["sno"].Value = _index++;

            }
            catch { }
        }
    }
}
