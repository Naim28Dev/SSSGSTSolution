using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class DayBookRegister : Form
    {
        DataBaseAccess dba;
        bool _loadStatus = false;
        string strAmtStatus = "";    
        public DayBookRegister()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();               
                chkAll.Checked = true;
                if(DateTime.Now>=MainPage.startFinDate && DateTime.Now<=MainPage.endFinDate)
                txtFromDate.Text = txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                else
                    txtFromDate.Text = txtToDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public DayBookRegister(DateTime sDate, DateTime eDate, string strBillType,string str)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                if (strBillType == "CASH A/C")
                    chkCash.Checked = true;
                else if (strBillType == "BANK A/C")
                    chkBank.Checked = true;
                txtFromDate.Text = sDate.ToString("dd/MM/yyyy");
                txtToDate.Text = eDate.ToString("dd/MM/yyyy");
                strAmtStatus = str;
                _loadStatus = true;
            }
            catch
            {
            }
        }

        private bool ValidateDate()
        {
            if (MainPage.mymainObject.bShowAllRecord)
                return true;
            else
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                TimeSpan _Ts = eDate.Subtract(sDate);
                if (_Ts.Days > 4 || _Ts.Days < 0)
                {
                    MessageBox.Show("Sorry ! You are not authorized to show to daybook more than 3 days.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                    return true;
            }
        }
        
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkAll.Checked || chkCash.Checked || chkBank.Checked || chkJournal.Checked || chkSale.Checked || chkPurchase.Checked || chkSaleReturn.Checked || chkPurchaseReturn.Checked || chkSaleService.Checked)
                {
                    if (ValidateDate())
                    {
                        GetAllRecord();
                    }
                }
                else
                {
                    MessageBox.Show(" Please select atleast one selection ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! "+ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void GetAllRecord()
        {

            try
            {
                dgrdDayBook.Rows.Clear();
                string strQuery = "", strSubQuery = "",strSQuery="";// " Select BA.*,Convert(varchar,BA.Date,103)_Date,PName,ISNULL(SMS.AName,AccountStatus) AStatus,(Select CashVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') CASHVCode,(Select BankVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') BANKVCode from BalanceAmount BA OUTER APPLY (Select (AreaCode+AccountNo+' '+Name)PName from SupplierMaster Where (AreaCode+AccountNo)=AccountID) SM OUTER APPLY (Select (AreaCode+AccountNo+' '+Name)AName from SupplierMaster Where (AreaCode+AccountNo)=AccountStatusID) SMS Where AccountStatus!='OPENING'  ";
                strSubQuery = GetQuery(ref strSQuery);

                strQuery += " Select *,CONVERT(varchar,_Balance.Date,103)_Date from ( "
                         + " Select GroupName, BA.Date,(AccountID + ' ' + Name)PName,RTRIM(LTRIM(ISNULL(AccountStatusID, '') + ' ' + AccountStatus))AccountStatus,Description,Amount,BA.Status,BA.UserName,BA.UpdatedBy,LTRIM((VoucherCode + ' ' + CAST(VoucherNo as varchar)))VCode from BalanceAmount BA inner join SupplierMaster SM on BA.AccountID = SM.AreaCode + SM.AccountNo and GroupName in ('BANK A/C','CASH A/C') Where AccountStatus!= 'OPENING' "+strSQuery+" UNION ALL "
                         + " Select AccountStatus as GroupName,BA.Date,(AccountID + ' ' + Name)PName,'' as AccountStatus,Description,Amount,BA.Status,BA.UserName,BA.UpdatedBy,LTRIM((VoucherCode + ' ' + CAST(VoucherNo as varchar)))VCode from BalanceAmount BA inner join SupplierMaster SM on BA.AccountID = SM.AreaCode + SM.AccountNo Where AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE','JOURNAL A/C') " + strSQuery + "   "
                         + " )_Balance WHere GroupName != '' " + strSubQuery + "  Order by _Balance.Date,GroupName ";


                DataTable dt = dba.GetDataTable(strQuery);
                double dAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dBalance = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdDayBook.Rows.Add(dt.Rows.Count);
                    int i = 0;
                    string strAccount = "", strStatus = "", strDescription = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        strAccount = Convert.ToString(dr["PName"]).ToUpper();
                        strStatus = Convert.ToString(dr["Status"]).ToUpper();
                        strDescription = Convert.ToString(dr["Description"]);
                        dgrdDayBook.Rows[i].Cells["date"].Value = dba.ConvertDateInExactFormat(Convert.ToString(dr["_Date"]));

                        dgrdDayBook.Rows[i].Cells["partyName"].Value = strAccount;
                        dgrdDayBook.Rows[i].Cells["accountStatus"].Value = Convert.ToString(dr["AccountStatus"]).ToUpper();
                        dgrdDayBook.Rows[i].Cells["description"].Value = strDescription;
                        dgrdDayBook.Rows[i].Cells["createdBy"].Value = dr["UserName"];
                        dgrdDayBook.Rows[i].Cells["updatedBy"].Value = dr["UpdatedBy"];
                        dgrdDayBook.Rows[i].Cells["billType"].Value = dr["GroupName"];
                        dAmt = dba.ConvertObjectToDouble(dr["Amount"]);

                        if (strStatus == "DEBIT")
                        {
                            dDebitAmt += dAmt;
                            dgrdDayBook.Rows[i].Cells["debitAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            dgrdDayBook.Rows[i].Cells["creditAmt"].Value = "";
                        }
                        else
                        {
                            dCreditAmt += dAmt;
                            dgrdDayBook.Rows[i].Cells["creditAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            dgrdDayBook.Rows[i].Cells["debitAmt"].Value = "";
                        }

                        string strVCode = Convert.ToString(dr["VCode"]);
                        if (strVCode == "" || strVCode == "0")
                            dgrdDayBook.Rows[i].Cells["voucherNo"].Value = strDescription;
                        else
                            dgrdDayBook.Rows[i].Cells["voucherNo"].Value = strVCode;

                        i++;
                    }
                }

                lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                dBalance = dDebitAmt - dCreditAmt;

                if (dBalance > 0)
                    lblBalance.Text = dBalance.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dBalance < 0)
                    lblBalance.Text = Math.Abs(dBalance).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblBalance.Text = "0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetQuery(ref string strSQuery)
        {
            string strQuery = "";          

            if (chkCash.Checked)            
                strQuery += "'CASH A/C'";            
            if (chkBank.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'BANK A/C'";
            }
            if (chkJournal.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'JOURNAL A/C'";
            }
            if (chkSale.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'SALES A/C' ";
            }
            if (chkPurchase.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'PURCHASE A/C' ";
            }
            if (chkPurchaseReturn.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'PURCHASE RETURN','CREDIT NOTE' ";
            }
            if (chkSaleReturn.Checked)
            {
                if (strQuery != "")
                    strQuery += ",";
                strQuery += "'SALE RETURN','DEBIT NOTE' ";
            }
            if (strQuery != "")
            {
                strQuery = " and GroupName in (" + strQuery + " ) ";
            }

            if (txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                strSQuery += " and (BA.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and BA.Date<'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }
            if(strAmtStatus!="")
            {
                strSQuery += " and BA.Status='" + strAmtStatus + "' ";
            }

            return strQuery;
        }

        private void DayBookRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                DataTable dt = CreateDataTable();
                Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("Day Book Register");
                Reporting.DayBookCrystal objBookReport = new SSS.Reporting.DayBookCrystal();
                objBookReport.SetDataSource(dt);
                objShowReport.myPreview.ReportSource = objBookReport;
                objShowReport.ShowDialog();

                objBookReport.Close();
                objBookReport.Dispose();
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(string));
                myDataTable.Columns.Add("DatePeriod", typeof(string));
                myDataTable.Columns.Add("Date", typeof(string));
                myDataTable.Columns.Add("PartyName", typeof(string));
                myDataTable.Columns.Add("AccountStatus", typeof(string));
                myDataTable.Columns.Add("Description", typeof(string));
                myDataTable.Columns.Add("DebitAmt", typeof(string));
                myDataTable.Columns.Add("CreditAmt", typeof(string));
                myDataTable.Columns.Add("BalanceAmt", typeof(string));
                myDataTable.Columns.Add("TotalDebitAmt", typeof(string));
                myDataTable.Columns.Add("TotalCreditAmt", typeof(string));
                myDataTable.Columns.Add("TotalBalance", typeof(string));
                myDataTable.Columns.Add("UserName", typeof(string));
                myDataTable.Columns.Add("GroupName", typeof(string));
               
                foreach (DataGridViewRow row in dgrdDayBook.Rows)
                {
                    DataRow drow = myDataTable.NewRow();
                    drow["CompanyName"] = MainPage.strPrintComapanyName;
                    drow["DatePeriod"] = row.Cells["voucherNo"].Value;
                    drow["Date"] = Convert.ToDateTime(row.Cells["date"].Value).ToString("dd/MM/yy");
                    drow["PartyName"] = row.Cells["partyName"].Value;
                    drow["AccountStatus"] = row.Cells["accountStatus"].Value;
                    drow["Description"] = row.Cells["description"].Value;
                    drow["DebitAmt"] = row.Cells["debitAmt"].Value;
                    drow["CreditAmt"] = row.Cells["creditAmt"].Value;
                    //drow["BalanceAmt"] = row.Cells["balanceAmt"].Value;
                    drow["TotalDebitAmt"] = lblDebit.Text;
                    drow["TotalCreditAmt"] = lblCredit.Text;
                    drow["TotalBalance"] = lblBalance.Text;
                    drow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    drow["GroupName"] = row.Cells["billType"].Value;
                   
                    myDataTable.Rows.Add(drow);
                }
            }
            catch { }
            return myDataTable;
        }
           

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAll.Checked)
                {
                    chkCash.Checked = chkBank.Checked =chkJournal.Checked =chkPurchase.Checked = chkSale.Checked = chkPurchaseReturn.Checked = chkSaleReturn.Checked =chkSaleService.Checked= true;
                }
                else if (chkSaleReturn.Checked && chkPurchaseReturn.Checked && chkSale.Checked && chkPurchase.Checked && chkJournal.Checked && chkCash.Checked && chkBank.Checked)
                {
                    chkCash.Checked = chkBank.Checked = chkJournal.Checked = chkPurchase.Checked = chkSale.Checked = chkPurchaseReturn.Checked = chkSaleReturn.Checked = chkSaleService.Checked = false;
                }
            }
            catch
            {
            }
        }

        private void chkCash_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSaleReturn.Checked && chkPurchaseReturn.Checked && chkSale.Checked && chkPurchase.Checked && chkJournal.Checked && chkCash.Checked && chkBank.Checked)
            {
                chkAll.Checked = true;
            }
            else
            {
                chkAll.Checked = false;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to print this Day book ! ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                btnPrint.Enabled = false;
                try
                {
                    DataTable dt = CreateDataTable();
                    Reporting.DayBookCrystal objBookReport = new SSS.Reporting.DayBookCrystal();
                    objBookReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objBookReport);
                    else
                    {
                        objBookReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        objBookReport.PrintToPrinter(1, false, 0, 0);
                    }

                    objBookReport.Close();
                    objBookReport.Dispose();
                }
                catch
                {
                }
                btnPrint.Enabled = true;
            }
        }

        private void dgrdDayBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdDayBook.CurrentRow.Index;
                    if (dgrdDayBook.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDayBook.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDayBook.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDayBook.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdDayBook.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDayBook.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDayBook.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDayBook.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDayBook.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDayBook.Columns.Count; l++)
                        {
                            if (dgrdDayBook.Columns[l].HeaderText == "" || !dgrdDayBook.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdDayBook.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDayBook.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Day_Book_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);


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

        private void DayBookRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            if(_loadStatus)
            {
                GetAllRecord();
            }
        }
    }
}
