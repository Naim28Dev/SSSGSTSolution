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
    public partial class LedgerMonthlySummary : Form
    {
        DataBaseAccess dba;
        DateTime startDate = MainPage.startFinDate,endDate = MainPage.endFinDate;
        public LedgerMonthlySummary(string strAccount,DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();       
            lblAccount.Text = strAccount.ToUpper();
            endDate = eDate;
        }
        public LedgerMonthlySummary(string strAccount,DateTime sDate, DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            lblAccount.Text = strAccount.ToUpper();
            endDate = eDate;
            startDate = sDate;
        }

        private void LedgerMonthlySummery_Load(object sender, EventArgs e)
        {
            BindMonthlySummeryInGridview();
        }

        private int ConverTointeger(int month)
        {
            month++;
            return month > 12 ? 12 : month;
        }

        public static int MonthDifference(DateTime sDate, DateTime eDate)
        {
            int monthsApart = 12 * (sDate.Year - eDate.Year) + sDate.Month - eDate.Month;
            return Math.Abs(monthsApart);
        }


        private void BindMonthlySummeryInGridview()
        {
            try
            {
                string[] strMonth = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

             //   int startMonth = MainPage.startFinDate.Month, endMonth = MainPage.endFinDate.Month;

                int startMonth = startDate.Month - 1, month = ConverTointeger(MonthDifference(startDate, endDate));

                int j = 0;
                double dDebitAmt = 0, dCreditAmt = 0, dAmt = 0;
                do
                {
                    dgrdItemSummery.Rows.Add(1);
                    int rowNo = dgrdItemSummery.Rows.Count;
                    dgrdItemSummery.Rows[rowNo - 1].Cells["month"].Value = strMonth[startMonth];

                    dgrdItemSummery.Rows[j].Cells["monthId"].Value = startMonth + 1; 
                    dAmt = dba.GetGroupAmountFromQuery(lblAccount.Text,startDate, endDate, startMonth+1);// GetDataFromMonth(i + 1);
                    if (dAmt >= 0)
                    {
                        dDebitAmt += dAmt;
                        dgrdItemSummery.Rows[j].Cells["debit"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    }
                    else
                    {
                        dCreditAmt +=Math.Abs(dAmt);
                        dgrdItemSummery.Rows[j].Cells["credit"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                    }
                    dAmt = dDebitAmt - dCreditAmt;
                    if (dAmt >= 0)
                        dgrdItemSummery.Rows[j].Cells["totalAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else
                        dgrdItemSummery.Rows[j].Cells["totalAmt"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                    j++;

                    if (startMonth == 11)
                        startMonth = 0;
                    else
                        startMonth++;
                    month--;
                } while (month != 0);

                lblDebitAmt.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCreditAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                
                dAmt = dDebitAmt-dCreditAmt;
                if (dAmt >= 0)
                    lblTotalAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy)+" Dr";
                else
                    lblTotalAmt.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            }
            catch
            {
            }
        }

        //private double GetDataFromMonth(int month)
        //{
        //    string strDate = month.ToString("00") + "/01/" + MainPage.startFinDate.Year;

        //    DateTime sDate = DateTime.Parse(strDate);
        //    DateTime eDate = sDate.AddMonths(1);
        //    double dAmount = GetNetAmount(sDate, eDate);

        //    return dAmount;
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LedgerMonthlySummery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void dgrdItemSummery_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (lblAccount.Text != "" && e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    ShowDetailPage();
                }
            }
            catch
            {
            }
        }

        private int GetMonth(int rowIndex)
        {
            int monthID = 0;
            try
            {
                monthID = Convert.ToInt32(dgrdItemSummery.Rows[rowIndex].Cells["monthId"].Value);
            }
            catch
            {
            }
            return monthID;
        }

        private void dgrdItemSummery_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdItemSummery.CurrentRow.Index;
                    if (dgrdItemSummery.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdItemSummery.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdItemSummery.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Enter && lblAccount.Text != "")
                {
                    if (dgrdItemSummery.CurrentRow.Index >= 0 && dgrdItemSummery.CurrentCell.ColumnIndex == 0)
                    {
                        ShowDetailPage();
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowDetailPage()
        {
            int rowIndex = dgrdItemSummery.CurrentRow.Index;
            if (lblAccount.Text == "SALES A/C" || lblAccount.Text == "PURCHASE A/C" || lblAccount.Text == "SALE RETURN" || lblAccount.Text == "PURCHASE RETURN" || lblAccount.Text == "SALE SERVICE")
            {
                BillWiseLedgerSummary objSummary = new BillWiseLedgerSummary(lblAccount.Text,startDate,endDate, GetMonth(rowIndex));
                objSummary.MdiParent = MainPage.mymainObject;
                objSummary.Show();
            }
            else
            {
                int _month=Convert.ToInt32(dgrdItemSummery.Rows[rowIndex].Cells["monthId"].Value);
                PartyWiseLedgerSummary objSummary = new PartyWiseLedgerSummary(lblAccount.Text,startDate, endDate, _month);
                objSummary.MdiParent = MainPage.mymainObject;
                objSummary.Show();
            }
        }

        #region Printing
               
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport showReport = new Reporting.ShowReport("Ledger Monthly Summary Preview");
                    Reporting.ShowMonthlyCrystal objReport = new Reporting.ShowMonthlyCrystal();
                    objReport.SetDataSource(dt);
                    showReport.myPreview.ReportSource = objReport;
                    showReport.Show();

                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        public DataTable CreateDataTable()
        {
            DataTable myTable = new DataTable();
            try
            {
                myTable.Columns.Add("CompanyName", typeof(string));
                myTable.Columns.Add("ReportHeader", typeof(string));
                myTable.Columns.Add("Month", typeof(string));
                myTable.Columns.Add("Debit", typeof(string));
                myTable.Columns.Add("Credit", typeof(string));
                myTable.Columns.Add("ClosingAmont", typeof(string));
                myTable.Columns.Add("TotalClosing", typeof(string));
                myTable.Columns.Add("TotalCr", typeof(string));
                myTable.Columns.Add("TotalDr", typeof(string));
                myTable.Columns.Add("LblAccountdetails", typeof(string));
                myTable.Columns.Add("CompanyAddress", typeof(string));
                myTable.Columns.Add("DatePeriod", typeof(string));

                //ASSIGN VALUES IN DATA TABLE
                foreach (DataGridViewRow row in dgrdItemSummery.Rows)
                {
                    if (row.Visible)
                    {
                        DataRow drow = myTable.NewRow();
                        drow["CompanyName"] = MainPage.strGRCompanyName;
                        drow["ReportHeader"] = "LEDGER REPORT MONTHLY SUMMARY";
                        drow["Month"] = row.Cells["month"].Value;
                        drow["Debit"] = row.Cells["debit"].Value;
                        drow["Credit"] = row.Cells["credit"].Value;
                        drow["ClosingAmont"] = row.Cells["totalAmt"].Value;
                        drow["TotalClosing"] = lblTotalAmt.Text;
                        drow["TotalCr"] = "";
                        drow["TotalDr"] = "";
                        drow["LblAccountdetails"] = "Monthly Summary of "+ lblAccount.Text;
                        myTable.Rows.Add(drow);
                    }
                }
            }
            catch { }
            if (myTable.Rows.Count > 0)
            {
                myTable.Rows[0]["DatePeriod"] = "Date Period : " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy"); ;
            }
            return myTable;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdItemSummery.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.ShowMonthlyCrystal objReport = new Reporting.ShowMonthlyCrystal();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }

                        objReport.Close();
                        objReport.Dispose();
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }
    
        #endregion

    }
}
