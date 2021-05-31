using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class DepreciationChart : Form
    {
        DataBaseAccess dba;
        public DepreciationChart()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void DepreciationChart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtAccountName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtAccountName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("FIXASSETSCATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtVCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("JVCODE", "SEARCH VOUCHER CODE", e.KeyCode);
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
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            try
            {
                SearchData();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "";

            if (txtAccountName.Text != "")
            {
                string[] strFullName = txtAccountName.Text.Split(' ');
                if (strFullName.Length > 0)
                    strQuery += " and BA.AccountID='" + strFullName[0].Trim() + "' ";
            }

            if (chkDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                strQuery += " and BA.Date>='" + sDate.ToString("MM/dd/yyyy") + "' and BA.Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
            }

            if (txtVCode.Text != "")
                strQuery += " and BA.VoucherCode ='" + txtVCode.Text + "' ";

            if (txtCategory.Text != "")
                strQuery += " and SM.Category ='" + txtCategory.Text + "' ";

            return strQuery;
        }

        private void SearchData()
        {
            string strQuery = "", strSubQuery = CreateQuery();

            strQuery = "Select Convert(varchar,BA.Date,103)BDate,(VoucherCode+' '+CAST(VoucherNo as varchar)) VNo,Category,(AccountID+' '+Name)AccountName,Description,Amount,DepreciationPer as DepPer,((CAST(Amount as money)*ISNULL(DepreciationPer,0))/100)DepAmt from BalanceAmount BA inner join SupplierMaster SM on BA.AccountID=SM.AreaCode+SM.AccountNo inner join Category _CT on SM.GroupName=_CT.GroupName Where SM.GroupName in ('FIXED ASSETS','FURNITURE / OFFICE ASSETS')  and CAST(BA.Amount as money)>0 "

                           + strSubQuery + " Order By  BA.Date, VoucherCode,VoucherNo  ";

            DataTable _dt = dba.GetDataTable(strQuery);
            BindRecord(_dt);
        }

        private void BindRecord(DataTable _dt)
        {
            dgrdDetails.Rows.Clear();
            double dAmt = 0, dTAmt = 0, dDepAmt = 0, dTDepAmt = 0;
            if (_dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(_dt.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    dTDepAmt += dDepAmt = dba.ConvertObjectToDouble(row["DepAmt"]);
                    dgrdDetails.Rows[_index].Cells["date"].Value = row["BDate"];
                    dgrdDetails.Rows[_index].Cells["voucherNo"].Value = row["VNo"];
                    dgrdDetails.Rows[_index].Cells["categoryName"].Value = row["Category"];
                    dgrdDetails.Rows[_index].Cells["accountName"].Value = row["AccountName"];
                    dgrdDetails.Rows[_index].Cells["description"].Value = row["Description"];
                    dgrdDetails.Rows[_index].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy); // row["Amount"];
                    dgrdDetails.Rows[_index].Cells["depreciaationPer"].Value = row["DepPer"];
                    dgrdDetails.Rows[_index].Cells["depAmt"].Value = dDepAmt.ToString("N2", MainPage.indianCurancy); // row["DepAmt"];

                    _index++;
                }
            }

            lblTotalAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
            lblDepAmt.Text = dDepAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    string strVoucherNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strVoucher = strVoucherNo.Trim().Split(' ');
                    if (strVoucher.Length > 0)
                    {
                        JournalEntry_New objJournalEntry = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                        objJournalEntry.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        objJournalEntry.ShowDialog();
                    }
                }
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        SSS.Reporting.DepreciationReport objReport = new Reporting.DepreciationReport();
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
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        SSS.Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Depreciation Report Preview");
                        SSS.Reporting.DepreciationReport objReport = new Reporting.DepreciationReport();
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

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            try
            {
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("HeaderName", typeof(String));
                table.Columns.Add("Date", typeof(String));
                table.Columns.Add("VoucherNo", typeof(String));
                table.Columns.Add("CategoryName", typeof(String));
                table.Columns.Add("AccountName", typeof(String));
                table.Columns.Add("Amount", typeof(String));
                table.Columns.Add("DepPer", typeof(String));
                table.Columns.Add("DepAmt", typeof(String));
                table.Columns.Add("TotalAmt", typeof(String));
                table.Columns.Add("TotalDepAmt", typeof(String));
                table.Columns.Add("PrintedBy", typeof(String));
                
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow dRow = table.NewRow();
                    dRow["CompanyName"] = MainPage.strCompanyName;
                    dRow["HeaderName"] = "DEPRECIATION CHART";
                    dRow["Date"] = row.Cells["date"].Value;
                    dRow["VoucherNo"] = row.Cells["voucherNo"].Value;
                    dRow["CategoryName"] = row.Cells["categoryName"].Value;
                    dRow["AccountName"] = row.Cells["accountName"].Value;
                    dRow["Amount"] = row.Cells["amount"].Value;
                    dRow["DepPer"] = row.Cells["depreciaationPer"].Value;
                    dRow["DepAmt"] = row.Cells["depAmt"].Value;
                  
                    dRow["PrintedBy"] ="Printed By : "+ MainPage.strLoginName + ",  " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    
                    table.Rows.Add(dRow);
                }

                if (table.Rows.Count > 0)
                {
                
                    table.Rows[table.Rows.Count - 1]["TotalAmt"] = lblTotalAmt.Text;
                    table.Rows[table.Rows.Count - 1]["TotalDepAmt"] = lblDepAmt.Text;
                }
            }
            catch
            {
            }
            return table;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
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
                        for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                        {
                            strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                            if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                            {
                                _skipColumn++;
                                j++;
                            }

                            ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                        }
                        _skipColumn = 0;
                        // Storing Each row and column value to excel sheet
                        for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                        {
                            for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                            {
                                if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                                {
                                    _skipColumn++;
                                    l++;
                                }
                                if (l < dgrdDetails.Columns.Count)
                                    ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                            }
                            _skipColumn = 0;
                        }
                        ExcelApp.Columns.AutoFit();


                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Description_Chart";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                            MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                        }
                        else
                            MessageBox.Show("Export Cancled...");

                    ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    //xlWorkbook.Close(true, misValue, misValue);
                    //ExcelApp.Quit();
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);


                }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnExport.Enabled = true;
            }
            catch
            { }
        }

        private void DepreciationChart_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bFASReport)
                    dba.EnableCopyOnClipBoard(dgrdDetails);
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }
        }
    }
}
