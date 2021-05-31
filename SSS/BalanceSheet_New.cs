using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class BalanceSheet_New : Form
    {
        DataBaseAccess dba;
        double _dClosingStockAmt = 0, _dOpeningStockAmt = 0;
        public BalanceSheet_New()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            SetDataWithGrid();
        }

        private void SetDataWithGrid()
        {
            try
            {
              
                dgrdDetails.Rows.Clear();
               
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                eDate = eDate.AddDays(1);

                string strQuery = "Select GroupName,SUM(Amount) Amt from (Select GroupName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY (SELECT GroupName from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID) SM Where Status='DEBIT' and  Date>='" + sDate.ToString("MM/dd/yyyy") + "'  and Date<'" + eDate.ToString("MM/dd/yyyy") + "'  Group by GroupName UNION ALL  "
                                + " Select GroupName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY (SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and  Date>='" + sDate.ToString("MM/dd/yyyy") + "'  and Date<'" + eDate.ToString("MM/dd/yyyy") + "'  Group by GroupName )Sales Group by GroupName ";

                DataTable _dt = dba.GetDataTable(strQuery);
                GetClosingStockAmt();

                double dAmt = 0;
                int _index = 0;
                string[] strGroupName = { "CAPITAL ACCOUNT", "CAPITAL WORK IN PROGRESS", "CREDITOR EXPENSE", "CREDITOR / MISCELLANEOUS", "DEFERRED TAX LIABILITIES", "DUTIES & TAXES", "LOAN (LIABILITY)", "LONG-TERM BORROWINGS",  "LONG-TERM PROVISIONS", "OTHER CURRENT LIABILITIES", "OTHER LONG TERM LIABILITIES", "PROVISIONS", "SUNDRY CREDITOR", "PROFIT & LOSS A/C", "RESERVES & SURPLUSES", "RETAINED EARNINGS", "SHORT TERM BORROWINGS", "SHORT TERM PROVISIONS", "SUSPENCES A/C", "TRADE PAYABLES" };
                string[] _strGroupName = { "BANK A/C", "BRANCH / DIVISIONS", "CASH A/C", "CASH IN HAND", "CURRENT INVESTMENTS", "DEBTOR / MISCELLANEOUS", "DEFERRED TAX ASSETS(NET)", "DEPOSITS (ASSET)", "FIXED ASSETS", "FURNITURE/OFFICE ASSETS", "INTANGIBLE ASSETS", "INTANGIBLE ASSETS UNDER DEVELOPMENT", "LAND / BUILDING", "LOAN (ASSETS)", "LONG TERM LOANS AND ADVANCES", "NON CURRENT INVESTMENTS", "OTHER CURRENT ASSETS","SUNDRY DEBTORS", "SECURED LOANS", "SHORT TERM BORROWINGS",  "SHORT-TERM LOANS AND ADVANCES", "UNSECURED LOANS", "VEHICLE A/C" };
                foreach (string strGroup in strGroupName)
                {
                    dAmt = GetAmtFromDataTable(_dt, strGroup);
                    if(dAmt!=0)
                    {
                        dAmt = dAmt * -1;
                        _index = dgrdDetails.Rows.Count;
                        dgrdDetails.Rows.Add();                     
                        dgrdDetails.Rows[_index].Cells["liability"].Value = strGroup;                       
                            dgrdDetails.Rows[_index].Cells["debitAmt"].Value = dAmt.ToString("N2",MainPage.indianCurancy);                       
                           
                    }
                }

                _index = 0;
                foreach (string strGroup in _strGroupName)
                {
                    dAmt = GetAmtFromDataTable(_dt, strGroup);
                    if (dAmt != 0)
                    {
                        
                        if (_index >= dgrdDetails.Rows.Count)
                            dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_index].Cells["assests"].Value = strGroup;
                        dgrdDetails.Rows[_index].Cells["creditAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                        _index++;
                    }

                }

                if (_dClosingStockAmt != 0)
                {
                    if (_index >= dgrdDetails.Rows.Count)
                        dgrdDetails.Rows.Add();

                    dgrdDetails.Rows[_index].Cells["assests"].Value = "CLOSING STOCK";
                    dgrdDetails.Rows[_index].Cells["creditAmt"].Value = _dClosingStockAmt.ToString("N2", MainPage.indianCurancy);
                }

                CalculateProfitLoss();
                CalculateTotalBalance();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error in try to set Data in Gridview in Balance Sheet", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private double GetAmtFromDataTable(DataTable _dt,string strGroupName)
        {
            double dAmt = 0;
            try
            {
                DataRow[] rows = _dt.Select(" GroupName='" + strGroupName + "' ");
                if (rows.Length > 0)
                {
                    dAmt = dba.ConvertObjectToDouble(rows[0]["Amt"]);
                }
            }
            catch { }
            return dAmt;
        }
      
        private void GetClosingStockAmt()
        {
            DateTime sDate=MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked)
            {
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
            }

            _dOpeningStockAmt = dba.GetOpeningStockAmount(sDate);
            _dClosingStockAmt = dba.GetClosingStockAmount(eDate.AddDays(1));
        }

        private void CalculateProfitLoss()
        {
            try
            {
              
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }

                double dAmount = 0;// dba.GetNetProfitAndLoss(sDate,eDate,0);
                DataTable _table;// = dba.GetNetProfitAndLossDataTable(sDate, eDate.AddDays(1), 0, true);
                if (MainPage.strSoftwareType == "AGENT")
                    _table = dba.GetNetProfitAndLossDataTable_Agent(sDate, eDate, 0, true);
                else
                    _table = dba.GetNetProfitAndLossDataTable(sDate, eDate, 0, true);

                if (_table.Rows.Count > 0)
                    dAmount = dba.ConvertObjectToDouble(_table.Rows[0]["Amt"]);
                dAmount -= (_dClosingStockAmt- _dOpeningStockAmt);

                if (dAmount != 0)
                {
                    int _index = dgrdDetails.Rows.Count;
                    dgrdDetails.Rows.Add();
               
                    if (dAmount > 0)
                    {                      
                        dgrdDetails.Rows[_index].Cells["assests"].Value = "NET LOSS";
                        dgrdDetails.Rows[_index].Cells["creditAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                    }
                    else if (dAmount < 0)
                    {
                        dgrdDetails.Rows[_index].Cells["liability"].Value = "NET PROFIT";
                        dgrdDetails.Rows[_index].Cells["debitAmt"].Value = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Calculation of Profit & loss in Balance Sheet", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }        

        private void CalculateTotalBalance()
        {
            try
            {               

                double dDebitAmt = 0, dCreditAmt = 0;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToString(dr.Cells["debitAmt"].Value) != "")
                        dCreditAmt += dba.ConvertObjectToDouble(dr.Cells["debitAmt"].Value);
                    if (Convert.ToString(dr.Cells["creditAmt"].Value) != "")
                        dDebitAmt += dba.ConvertObjectToDouble(dr.Cells["creditAmt"].Value);
                }

                double fDiff = Convert.ToDouble(dDebitAmt.ToString("0.00")) - Convert.ToDouble(dCreditAmt.ToString("0.00"));


               // string strDiff = (dDebitAmt - dCreditAmt).ToString("N2", MainPage.indianCurancy);

                //double fDiff = double.Parse(strDiff);
                if (fDiff < 0)
                {
                    dgrdDetails.Rows.Add();
                    dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["assests"].Value = "OPENING DIFFERENCE ";
                    dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["creditAmt"].Value = Math.Abs(fDiff).ToString("N2", MainPage.indianCurancy); 
                }
                else if (fDiff > 0)
                {
                    dgrdDetails.Rows.Add();
                    dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["liability"].Value = "OPENING DIFFERENCE";
                    dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["debitAmt"].Value = fDiff.ToString("N2", MainPage.indianCurancy);                  
                }

                dgrdDetails.Rows.Add(2);
                dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["liability"].Value = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["assests"].Value = "TOTAL BALANCE :";
                dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["creditAmt"].Value = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["debitAmt"].Value = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                //dgrdDetails.Rows[dgrdDetails.Rows.Count - 2].Height = 10;

            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Calculate Total Amount in Balance Sheet", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BalanceSheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");            
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
            {
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDate.Focus();
            }
            else
                SetDataWithGrid();
            btnGo.Enabled = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.ColumnIndex == 0 || e.ColumnIndex == 2) && e.RowIndex>=0)
                {
                    ShowDetails(Convert.ToString(dgrdDetails.CurrentCell.Value));
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if ((dgrdDetails.CurrentCell.ColumnIndex == 0 || dgrdDetails.CurrentCell.ColumnIndex == 2) && dgrdDetails.CurrentCell.RowIndex >= 0)
                    {
                        ShowDetails(Convert.ToString(dgrdDetails.CurrentCell.Value));
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowDetails(string strValue)
        {
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }

            if(strValue== "CLOSING STOCK")
            {
                StockRegister objStock = new StockRegister(eDate);
                objStock.MdiParent = MainPage.mymainObject;
                objStock.Show();
            }
            else if (strValue !="")
            {
                ShowCategoryWiseDetails objBalance = new ShowCategoryWiseDetails(strValue, sDate, eDate);
                objBalance.MdiParent = MainPage.mymainObject;
                objBalance.ShowInTaskbar = true;
                objBalance.Show();
            }
        }

        public DataTable CreateDataTable()
        {
            DataTable myTable = new DataTable();
            try
            {
                myTable.Columns.Add("ReportHeader", typeof(string));
                myTable.Columns.Add("CompanyName", typeof(string));
                myTable.Columns.Add("Particulars", typeof(string));
                myTable.Columns.Add("Amount", typeof(string));
                myTable.Columns.Add("Particulars1", typeof(string));
                myTable.Columns.Add("Amount1", typeof(string));
                myTable.Columns.Add("FooterParticulars", typeof(string));
                myTable.Columns.Add("FooterAmount", typeof(string));
                myTable.Columns.Add("FooterParticulars1", typeof(string));
                myTable.Columns.Add("FooterAmount1", typeof(string));
                myTable.Columns.Add("CompanyAddress", typeof(string));

                string strDate = "";
                if(chkDate.Checked && txtFromDate.Text.Length==10 && txtToDate.Text.Length == 10)                
                    strDate = " Date period from " + txtFromDate.Text + " to " + txtToDate.Text;
                else
                    strDate = " Date period from " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " to " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                for (int index = 0; index < dgrdDetails.Rows.Count; ++index)
                {
                    DataGridViewRow row = dgrdDetails.Rows[index];
                    DataRow drow = myTable.NewRow();
                    drow["ReportHeader"] = "BALANCE SHEET SUMMARY : " + strDate;
                    
                    drow["CompanyName"] = MainPage.strPrintComapanyName;

                    if (index < dgrdDetails.Rows.Count - 1)
                    {
                        drow["Particulars"] = row.Cells["liability"].Value;
                        drow["Amount"] = row.Cells["debitAmt"].Value;
                        drow["Particulars1"] = row.Cells["assests"].Value;
                        drow["Amount1"] = row.Cells["creditAmt"].Value;
                    }
                    else
                    {
                        drow["FooterParticulars"] = row.Cells["liability"].Value;
                        drow["FooterAmount"] = row.Cells["debitAmt"].Value;
                        drow["FooterParticulars1"] = row.Cells["assests"].Value;
                        drow["FooterAmount1"] = row.Cells["creditAmt"].Value;
                    }
                    myTable.Rows.Add(drow);

                }
            }
            catch { }
            if (myTable.Rows.Count > 0)
            {
                // myTable.Rows[0]["CompanyAddress"] =DataBaseAccess.ExecuteMyScalar("Select (Address+' '+City+' '+CAST(PinCode as varchar))Address from CompanyDetails");
            }
            return myTable;
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objShowReport = new Reporting.ShowReport("Balance Sheet Preview");
                    Reporting.ProfitLossReport objReport = new Reporting.ProfitLossReport();
                    objReport.SetDataSource(dt);
                    objShowReport.myPreview.ReportSource = objReport;
                    objShowReport.Show();
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
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ProfitLossReport objReport = new Reporting.ProfitLossReport();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                    { 
                        objReport.PrintToPrinter(1, false, 0, 0);
                    }
                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void BalanceSheet_New_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

        private void txtToDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void btnDetailView_Click(object sender, EventArgs e)
        {
            btnDetailView.Enabled = false;
            try
            {
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                FASDetailPage objFASDetailPage = new FASDetailPage("BALANCE", sDate, eDate);
                objFASDetailPage.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objFASDetailPage.ShowInTaskbar = true;
                objFASDetailPage.Show();
            }
            catch
            {
            }
            btnDetailView.Enabled = true;
        }
    }
}
