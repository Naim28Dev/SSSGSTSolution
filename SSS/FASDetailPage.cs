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
    public partial class FASDetailPage : Form
    {
        DataBaseAccess dba;
        int leftRowIndex = 0, rightRowIndex = 0;
        double dTotalDebitAmt = 0, dTotalCreditAmt = 0,_dOpeningStockAmt=0, _dClosingStockAmt = 0;
    //    bool bProfitStatus = false;
        string strDetailType = "";
        public FASDetailPage()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
             GetAllData();
        }

        public FASDetailPage(string  strDType, DateTime sDate,DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            chkDate.Checked = true;
            strDetailType = strDType;
            txtFromDate.Text = sDate.ToString("dd/MM/yyyy");
            txtToDate.Text = eDate.ToString("dd/MM/yyyy");
            GetAllData();
        }

        private void ProfitAndLossDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
            {
                txtFromDate.ReadOnly = txtToDate.ReadOnly = false;
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
            else
                txtFromDate.ReadOnly = txtToDate.ReadOnly = true;
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
        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
            {
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDate.Focus();
            }
            else
                GetAllData();
            btnGo.Enabled = true;
        }

        private void GetAllData()
        {
            try
            {

                dTotalDebitAmt = dTotalCreditAmt = 0;
                leftRowIndex = rightRowIndex = 0;
                dgrdDetails.Rows.Clear();

                if (strDetailType == "PROFIT")
                {
                    SetOpeningStockDetails("OPENING STOCK");
                    GetLeftSideDetails("PURCHASE A/C", false, 1);
                    GetLeftSideDetails("SALE RETURN", false, 1);
                    GetLeftSideDetails("CREDIT NOTE", false, 1);
                    GetLeftSideDetails("DIRECT EXPENSE A/C", false, 1);
                    GetLeftSideDetails("INDIRECT EXPENSE A/C", false, 1);
                    GetLeftSideDetails("COST OF MATERIAL TRADED", false, 1);
                    GetLeftSideDetails("EMPLOYEE BENEFIT EXPENSE", false, 1);
                    GetLeftSideDetails("SELLING & DISTRIBUTION EXPENSES", false, 1);
                    GetLeftSideDetails("OTHER EXPENSES", false, 1);
                    GetLeftSideDetails("DEPRECIATION", false, 1);

                    GetRightSideDetails("SALES A/C", true, 1);
                    GetRightSideDetails("SALE SERVICE", true, 1);
                    GetRightSideDetails("DEBIT NOTE", true, 1);
                    GetRightSideDetails("PURCHASE RETURN", true, 1);
                    GetRightSideDetails("DIRECT INCOME A/C", true, 1);
                    GetRightSideDetails("INDIRECT INCOME A/C", true, 1);
                    GetRightSideDetails("REVENUE FROM OPERATIONS", true, 1);
                    GetRightSideDetails("OTHER INCOME", true, 1);
                    SetClosingStockDetails("CLOSING STOCK", strDetailType);
                }
                else if (strDetailType == "TRIAL")
                {
                    SetOpeningStockDetails("OPENING STOCK");
                    GetLeftSideDetails("PURCHASE A/C", false, 0);
                    GetLeftSideDetails("SALE RETURN", false, 0);
                    GetLeftSideDetails("CREDIT NOTE", false, 0);
                    GetLeftSideDetails("DIRECT EXPENSE A/C", false, 0);
                    GetLeftSideDetails("INDIRECT EXPENSE A/C", false, 0);
                    GetLeftSideDetails("COST OF MATERIAL TRADED", false, 0);
                    GetLeftSideDetails("EMPLOYEE BENEFIT EXPENSE", false, 0);
                    GetLeftSideDetails("SELLING & DISTRIBUTION EXPENSES", false, 0);
                    GetLeftSideDetails("OTHER EXPENSES", false, 0);
                    GetLeftSideDetails("DEPRECIATION", false, 0);

                    GetRightSideDetails("SALES A/C", true, 0);
                    GetRightSideDetails("SALE SERVICE", true, 0);
                    GetRightSideDetails("DEBIT NOTE", true, 0);
                    GetRightSideDetails("PURCHASE RETURN", true, 0);
                    GetRightSideDetails("DIRECT INCOME A/C", true, 0);
                    GetRightSideDetails("INDIRECT INCOME A/C", true, 0);
                    GetRightSideDetails("REVENUE FROM OPERATIONS", true, 0);
                    GetRightSideDetails("OTHER INCOME", true, 0);
                    SetClosingStockDetails("CLOSING STOCK", strDetailType);
                }
                //else if (strDetailType == "TRIAL")
                //{
                //   // SetOpeningStockDetails("OPENING STOCK");
                //    GetRightSideDetails("PURCHASE A/C", false);
                //    GetRightSideDetails("SALE RETURN", false);                  
                //    GetLeftSideDetails("DIRECT INCOME A/C", true);
                //    GetLeftSideDetails("INDIRECT INCOME A/C", true);
                //    GetLeftSideDetails("REVENUE FROM OPERATIONS", true);
                //    GetLeftSideDetails("OTHER INCOME", true);

                //    GetLeftSideDetails("SALES A/C", true);
                //    GetLeftSideDetails("SALE SERVICE", true);
                //    GetLeftSideDetails("PURCHASE RETURN", true);
                //    GetRightSideDetails("DIRECT EXPENSE A/C", false);
                //    GetRightSideDetails("INDIRECT EXPENSE A/C", false);
                //    GetRightSideDetails("DEPRECIATION", false);
                //    GetRightSideDetails("COST OF MATERIAL TRADED", false);
                //    GetRightSideDetails("EMPLOYEE BENEFIT EXPENSE", false);
                //    GetRightSideDetails("OTHER EXPENSES", false);
                //    GetRightSideDetails("SELLING & DISTRIBUTION EXPENSES", false);

                //}
                if (strDetailType == "BALANCE" || strDetailType == "TRIAL")
                {

                    GetLeftSideDetails("CAPITAL ACCOUNT", true, 1);
                    GetLeftSideDetails("CAPITAL WORK IN PROGRESS", true, 1);
                    GetLeftSideDetails("SUNDRY CREDITOR", true, 1);
                    GetLeftSideDetails("DUTIES & TAXES", true, 1);
                    GetLeftSideDetails("PROVISIONS", true, 1);
                    GetLeftSideDetails("RETAINED EARNINGS", true, 1);
                    GetLeftSideDetails("CREDITOR / MISCELLANEOUS", true, 1);
                    GetLeftSideDetails("CREDITOR EXPENSE", true, 1);                                   
                    GetLeftSideDetails("DEFERRED TAX LIABILITIES", true, 1);
                    GetLeftSideDetails("LOAN (LIABILITY)", true, 1);
                    GetLeftSideDetails("SECURED LOANS", true, 1);
                    GetLeftSideDetails("UNSECURED LOANS", true, 1);                  
                    GetLeftSideDetails("LONG-TERM BORROWINGS", true, 1);
                    GetLeftSideDetails("SHORT TERM BORROWINGS", true, 1);
                    GetLeftSideDetails("LONG-TERM PROVISIONS", true, 1);
                    GetLeftSideDetails("SHORT TERM PROVISIONS", true, 1);
                    GetLeftSideDetails("OTHER CURRENT LIABILITIES", true, 1);
                    GetLeftSideDetails("OTHER LONG TERM LIABILITIES", true, 1);
                    GetLeftSideDetails("TRADE PAYABLES", true, 1);
                    GetLeftSideDetails("RESERVES & SURPLUSES", true, 1);
                    GetLeftSideDetails("PROFIT & LOSS A/C", true, 1);

                    GetRightSideDetails("SUNDRY DEBTORS", false, 1);
                    GetRightSideDetails("DEBTOR / MISCELLANEOUS", false, 1);
                    GetRightSideDetails("BANK A/C", false, 1);
                    GetRightSideDetails("CASH A/C", false, 1);
                    GetRightSideDetails("CASH IN HAND", false, 1);
                    GetRightSideDetails("DEPOSITS (ASSET)", false, 1);                   
                    GetRightSideDetails("FIXED ASSETS", false, 1);
                    GetRightSideDetails("FURNITURE/OFFICE ASSETS", false, 1);
                    GetRightSideDetails("LOAN (ASSETS)", false, 1);
                    GetRightSideDetails("LAND / BUILDING", false, 1);
                    GetRightSideDetails("INVESTMENTS", false, 1);
                    GetRightSideDetails("BRANCH / DIVISIONS", false, 1);
                    GetRightSideDetails("NON CURRENT INVESTMENTS", false, 1);
                    GetRightSideDetails("CURRENT INVESTMENTS", false, 1);
                    GetRightSideDetails("DEFERRED TAX ASSETS(NET)", false, 1);
                    GetRightSideDetails("DEPOSITS (ASSET)", false, 1);
                    GetRightSideDetails("INTANGIBLE ASSETS", false, 1);
                    GetRightSideDetails("INTANGIBLE ASSETS UNDER DEVELOPMENT", false, 1);
                    GetRightSideDetails("LONG TERM LOANS AND ADVANCES", false, 1);
                    GetRightSideDetails("SHORT-TERM LOANS AND ADVANCES", false, 1);
                    GetRightSideDetails("OTHER CURRENT ASSETS", false, 1);
                    GetRightSideDetails("SUSPENCES A/C", false, 1);                    

                    if (strDetailType == "BALANCE")
                    {
                        SetClosingStockDetails("CLOSING STOCK", strDetailType);

                        GetLeftSideDetails("PROFIT & LOSS", true, 1);
                    }
                }
                SetTotalAmount();
            }
            catch
            {
            }
        }

        private void SetOpeningStockDetails(string strData)
        {
            try
            {
                int rowIndex = 0;
                DateTime sDate = MainPage.startFinDate;
                if (chkDate.Checked)
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);

                _dOpeningStockAmt = dba.GetOpeningStockAmount(sDate);
                if (_dOpeningStockAmt > 0)
                {
                    AddBlankRow(false);
                    rowIndex = rightRowIndex;
                    SetAmountAndDataWithControlsInLeft(strData, _dOpeningStockAmt, false);
                    AddBlankRow(false);
                    dTotalDebitAmt += _dOpeningStockAmt;
                }
            }
            catch
            {
            }
        }

        private void SetClosingStockDetails(string strData, string strDetailType)
        {
            try
            {
                int rowIndex = 0;
                DateTime eDate = MainPage.endFinDate;
                if (chkDate.Checked)                
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);

                eDate = eDate.AddDays(1);

                _dClosingStockAmt  = dba.GetClosingStockAmount(eDate);
                if (_dClosingStockAmt > 0)
                {

                    AddBlankRow(false);
                    rowIndex = rightRowIndex;
                    SetAmountAndDataWithControlsInRight(strData, _dClosingStockAmt);
                    dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Style.Font = dgrdDetails.Rows[rowIndex].Cells["rightData"].Style.Font = new Font(dgrdDetails.Font, FontStyle.Bold);

                    AddBlankRow(false);
                   // if (strDetailType == "PROFIT" || strDetailType == "BALANCE")
                    dTotalCreditAmt += _dClosingStockAmt;
                }
            }
            catch
            {
            }
        }


        private void SetTotalAmount()
        {
            dgrdDetails.Rows.Add(3);
            int rowIndex = dgrdDetails.Rows.Count - 1;
            double dDiff = Math.Round(dTotalCreditAmt,0) - Math.Round(dTotalDebitAmt,0);
            // dDiff = dDiff * -1;
            if (strDetailType == "PROFIT")
            {
                if (dDiff > 0)
                {
                    if (strDetailType == "PROFIT")
                        dgrdDetails.Rows[rowIndex - 1].Cells["leftData"].Value = "NET PROFIT";
                    else
                        dgrdDetails.Rows[rowIndex - 1].Cells["leftData"].Value = "OPENING DIFF.";
                    dgrdDetails.Rows[rowIndex - 1].Cells["leftAmt"].Value = dDiff.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex - 1].DefaultCellStyle.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                }
                else if (dDiff < 0)
                {
                    if (strDetailType == "PROFIT")
                        dgrdDetails.Rows[rowIndex - 1].Cells["rightData"].Value = "NET LOSS";
                    else
                        dgrdDetails.Rows[rowIndex - 1].Cells["rightData"].Value = "OPENING DIFF.";
                    dgrdDetails.Rows[rowIndex - 1].Cells["rightAmt"].Value = Math.Abs(dDiff).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex - 1].DefaultCellStyle.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                }
            }
            dgrdDetails.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
            dgrdDetails.Rows[rowIndex].Cells["leftData"].Value = dgrdDetails.Rows[rowIndex].Cells["rightData"].Value = "TOTAL BALANCE";
            dgrdDetails.Rows[rowIndex].Cells["leftAmt"].Value = dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Value = Math.Abs(dTotalDebitAmt).ToString("N2", MainPage.indianCurancy); ;

        }
       
        private void AddBlankRow(bool leftStatus)
        {
            if (leftStatus)
            {
                if (leftRowIndex == dgrdDetails.Rows.Count)
                    dgrdDetails.Rows.Add();
                dgrdDetails.Rows[leftRowIndex].Cells["leftAmt"].Value = "-----------------";
                leftRowIndex++;
            }
            else
            {
                if (rightRowIndex == dgrdDetails.Rows.Count)
                    dgrdDetails.Rows.Add();
                dgrdDetails.Rows[rightRowIndex].Cells["rightAmt"].Value = "-----------------";
                rightRowIndex++;
            }
        }
             

        private void GetLeftSideDetails(string strGroupName,bool _bStatus, int _status)
        {
            int rowIndex = 0;
             DataTable dt =null;
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked)
            {
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }
            if (strGroupName == "PROFIT & LOSS")
            {
                SetProfitAndLoss(strGroupName, sDate,eDate);
            }
            else
            {                

                dt = dba.GetAllDetailsByGroupName(strGroupName, sDate, eDate,0);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        double dAmt = 0, dTotalAmt = 0;
                        AddBlankRow(true);
                        rowIndex = leftRowIndex;
                        SetAmountAndDataWithControlsInLeft(strGroupName, 1, _bStatus);
                        AddBlankRow(true);

                        string strName = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            strName = Convert.ToString(row["Name"]);
                            dAmt = Convert.ToDouble(row["Amount"]);
                            if (_bStatus)
                                dAmt = dAmt * -1;
                            dTotalAmt += dAmt;
                            SetAmountAndDataWithControlsInLeft(strName, dAmt, _bStatus);
                        }

                        dgrdDetails.Rows[rowIndex].Cells["leftAmt"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["leftAmt"].Style.Font = dgrdDetails.Rows[rowIndex].Cells["leftData"].Style.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                        if (_status == 0)
                            dTotalAmt = dTotalAmt * -1;
                        dTotalDebitAmt += dTotalAmt;
                    }
                }
            }
        }

        private void GetRightSideDetails(string strGroupName,bool _bStatus, int _status)
        {
            int rowIndex = rightRowIndex;
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked)
            {
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }

            DataTable dt = dba.GetAllDetailsByGroupName(strGroupName, sDate, eDate,0);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    double dAmt = 0, dTotalAmt = 0;
                    AddBlankRow(false);
                    rowIndex = rightRowIndex;
                    SetAmountAndDataWithControlsInRight(strGroupName, 1);
                    AddBlankRow(false);

                    string strName = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        strName = Convert.ToString(row["Name"]);
                        dAmt = Convert.ToDouble(row["Amount"]);
                        if (_bStatus)
                            dAmt = dAmt * -1;
                        dTotalAmt += dAmt;
                        SetAmountAndDataWithControlsInRight(strName, dAmt);
                    }

                    dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Style.Font = dgrdDetails.Rows[rowIndex].Cells["rightData"].Style.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                    if (_status == 0)
                        dTotalAmt = dTotalAmt * -1;
                    dTotalCreditAmt += dTotalAmt;
                }
            }
        }

        private void SetProfitAndLoss(string strGroupName, DateTime sDate, DateTime eDate)
        {
            int rowIndex = 0;
           
            double dAmount = dba.GetNetProfitAndLoss(sDate,eDate,0);
           // _dClosingStockAmt = dba.GetClosingStockAmount(eDate);
            dAmount -= _dClosingStockAmt;
          
            if (dAmount < 0)
            {
                AddBlankRow(true);
                rowIndex = leftRowIndex;
                SetAmountAndDataWithControlsInLeft(strGroupName, 1,false);
                AddBlankRow(true);
                dAmount = dAmount * -1;
                SetAmountAndDataWithControlsInLeft(strGroupName, dAmount,false);

                dgrdDetails.Rows[rowIndex].Cells["leftAmt"].Value = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["leftAmt"].Style.Font = dgrdDetails.Rows[rowIndex].Cells["leftData"].Style.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                dTotalDebitAmt += Math.Abs(dAmount);
            }
            else if (dAmount > 0)
            {
                AddBlankRow(false);
                rowIndex = rightRowIndex;
                SetAmountAndDataWithControlsInRight(strGroupName, 1);
                AddBlankRow(false);

                SetAmountAndDataWithControlsInRight(strGroupName, dAmount);

                dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["rightAmt"].Style.Font = dgrdDetails.Rows[rowIndex].Cells["rightData"].Style.Font = new Font(dgrdDetails.Font, FontStyle.Bold);
                dTotalCreditAmt += dAmount;
            }
        }

        private void SetAmountAndDataWithControlsInLeft(string strData, double dAmount,bool _bStatus)
        {
            if (dAmount != 0)
            {
                if (leftRowIndex == dgrdDetails.Rows.Count)
                    dgrdDetails.Rows.Add();
                //if (_bStatus)
                //    dAmount = dAmount * -1;
                dgrdDetails.Rows[leftRowIndex].Cells["leftData"].Value = strData;
                dgrdDetails.Rows[leftRowIndex].Cells["leftAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                leftRowIndex++;
            }

        }
        
        private void SetAmountAndDataWithControlsInRight(string strData, double dAmount)
        {
            if (dAmount != 0)
            {
                if (rightRowIndex == dgrdDetails.Rows.Count)
                    dgrdDetails.Rows.Add();
                dgrdDetails.Rows[rightRowIndex].Cells["rightData"].Value = strData;
                dgrdDetails.Rows[rightRowIndex].Cells["rightAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                rightRowIndex++;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objShowReport = new Reporting.ShowReport("FAS Details Preview");
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
                string strDetails = "";
                if (strDetailType == "PROFIT")
                    strDetails = "PROFIT & LOSS DETAILS";
                else if (strDetailType == "BALANCE")
                    strDetails = "BALANCE DETAILS";
                else if (strDetailType == "TRIAL")
                    strDetails = "TRIAL BALANCE DETAILS";

                string strDate = "";
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    strDate = " Date period from " + txtFromDate.Text + " to " + txtToDate.Text;
                else
                    strDate = " Date period from " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " to " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                
                //ASSIGN VALUES IN DATA TABLE
                for (int index = 0; index < dgrdDetails.Rows.Count; ++index)
                {
                    DataGridViewRow row = dgrdDetails.Rows[index];
                    DataRow drow = myTable.NewRow();
                    drow["ReportHeader"] = "DETAILS REPORT OF "+ strDetails+ " "+ strDate;
                    drow["CompanyName"] = MainPage.strPrintComapanyName;
                    if (index < dgrdDetails.Rows.Count - 2)
                    {
                        drow["Particulars"] = row.Cells["leftData"].Value;
                        drow["Amount"] = row.Cells["leftAmt"].Value;
                        drow["Particulars1"] = row.Cells["rightData"].Value;
                        drow["Amount1"] = row.Cells["rightAmt"].Value;
                    }
                    else
                    {
                        drow["FooterParticulars"] = row.Cells["leftData"].Value;
                        drow["FooterAmount"] = row.Cells["leftAmt"].Value;
                        drow["FooterParticulars1"] = row.Cells["rightData"].Value;
                        drow["FooterAmount1"] = row.Cells["rightAmt"].Value;
                    }
                    myTable.Rows.Add(drow);

                }
            }
            catch { }
            //if (myTable.Rows.Count > 0)
            //{
            //   // myTable.Rows[0]["CompanyAddress"] = DataBaseAccess.ExecuteMyScalar("Select (Address+' '+City+' '+CAST(PinCode as varchar))Address from CompanyDetails");
            //}
            return myTable;
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
                        objReport.PrintToPrinter(1, false, 0, 0);

                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }
    }
}
