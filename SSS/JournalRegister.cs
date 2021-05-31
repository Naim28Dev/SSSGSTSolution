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
    public partial class JournalRegister : Form
    {
        DataBaseAccess dba;
        public JournalRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void JournalRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;                   
                }
                else
                    e.Handled = true;
            }
            catch { }
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

        private void chkVchNo_CheckedChanged(object sender, EventArgs e)
        {
            txtFromVNo.ReadOnly = txtToVNo.ReadOnly = !chkVchNo.Checked;
            txtFromVNo.Text = txtToVNo.Text = "";
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

        private void txtGSTNature_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("JOURNALGSTNATURE", "SEARCH GST NATURE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGSTNature.Text = objSearch.strSelectedData;
                }

                e.Handled = true;
            }
            catch
            {
            }
        }
        
        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if (chkVchNo.Checked && (txtFromVNo.Text == "" || txtToVNo.Text == ""))
                {
                    MessageBox.Show("Sorry ! Please enter voucher no ! or uncheck voucher No box ! ", "Voucher No Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkVchNo.Focus();
                }
                else
                {
                    GetDataFromQuery();
                }
            }
            catch(Exception ex) { MessageBox.Show("Sorry " + ex.Message); }
            btnGo.Enabled = true;
        }



        private string CreateQuery()
        {
            string strQuery = "";
            if (txtPartyName.Text != "")
            {
                string[] strFullName = txtPartyName.Text.Split(' ');
                if (strFullName.Length > 0)
                    strQuery += " and AccountID='" + strFullName[0].Trim() + "' ";
            }

            if (chkDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text),endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                strQuery += " and Date>='"+ sDate.ToString("MM/dd/yyyy")+"' and Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
            }

            if (chkVchNo.Checked)
            {
                strQuery += " and VoucherNo>= " + txtFromVNo.Text + " and VoucherNo <=" + txtToVNo.Text + " ";
            }

            if (txtAmount.Text != "")
                strQuery += " and Cast(Amount as Money) = " + Convert.ToDouble(txtAmount.Text) + "  ";

            if (txtDescription.Text != "")
                strQuery += " and Description Like('%" + txtDescription.Text + "%') ";

            if (txtVCode.Text != "")
                strQuery += " and VoucherCode ='" + txtVCode.Text + "' ";

            if (txtGSTNature.Text != "")
                strQuery += " and GSTNature ='" + txtGSTNature.Text + "' ";
            return strQuery;
        }

        private void GetDataFromQuery()
        {
            string strQuery = "", strSubQuery =CreateQuery();
            strQuery += " Select Date,(VoucherCode+' '+CAST(VoucherNo as varchar)) VoucherNo,GSTNature,Amt as Amount,PName PartyName,GroupName,SUM(ISNULL(CGSTAmt,0))CGSTAmt,SUM(ISNULL(IGSTAmt,0))IGSTAmt from BalanceAmount BA OUTER APPLY (Select TOP 1 (CAST(BA4.AMount as Money)) Amt,(AccountID+' '+Name) PName,GroupName from BalanceAmount BA4 inner join SupplierMaster SM on BA4.AccountID=AreaCode+AccountNo and BA4.VoucherCode=BA.VoucherCode and BA.VoucherNo=BA4.VoucherNo and BA4.Status='DEBIT' and BA4.PartyName not Like('%CGST%') and BA4.PartyName not Like('%SGST%') and BA4.PartyName not Like('%IGST%') and BA4.PartyName not Like('%ROUND%')) SM OUTER APPLY (Select MAX(CAST(BA1.Amount as money)) CGSTAmt from BalanceAmount BA1 Where BA1.VoucherCode!='' and BA1.VoucherCode=BA.VoucherCode and BA1.VoucherNo=BA.VoucherNo and BA1.PartyName Like('%CGST%')) BA1 OUTER APPLY (Select MAX(CAST(BA2.Amount as money)) IGSTAmt from BalanceAmount BA2 Where  BA2.VoucherCode!='' and BA2.VoucherCode=BA.VoucherCode and BA2.VoucherNo=BA.VoucherNo and BA2.PartyName Like('%IGST%')) BA2 Where VoucherCode!='' and Status='DEBIT' and GSTNature!='' " + strSubQuery+ "  Group by Date,VoucherCode,VoucherNo,GSTNature,Amt,PName,GroupName Order by BA.Date ";

            DataTable dt= dba.GetDataTable(strQuery);
            BindDataWithGrid(dt);
        }

        private void BindDataWithGrid(DataTable dt)
        {
            double dAmt = 0, dCGST = 0, dIGST = 0, dTotalAamt = 0, dTAmt = 0, dTCGST = 0, dTIGST = 0;
            try
            {
                dgrdDetails.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                   
                    foreach (DataRow row in dt.Rows)
                    {
                        dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                        dTCGST += dCGST = dba.ConvertObjectToDouble(row["CGSTAmt"]);
                        dTIGST += dIGST = dba.ConvertObjectToDouble(row["IGSTAmt"]);
                        dTotalAamt = (dAmt + dCGST + dCGST + dIGST);

                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = row["Date"];
                        dgrdDetails.Rows[_rowIndex].Cells["voucherNo"].Value = row["VoucherNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["partyName"].Value = row["PartyName"];
                        dgrdDetails.Rows[_rowIndex].Cells["gstNature"].Value = row["GSTNature"];
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value = dAmt;
                        dgrdDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = dCGST;
                        dgrdDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = dCGST;
                        dgrdDetails.Rows[_rowIndex].Cells["igstAmt"].Value = dIGST;
                        dgrdDetails.Rows[_rowIndex].Cells["totalAmt"].Value =dTotalAamt;
                        dgrdDetails.Rows[_rowIndex].Cells["groupName"].Value = row["GroupName"];
                        _rowIndex++;
                    }
                }
            }
            catch { }

            lblTotalAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
            lblCGSTAmt.Text = dTCGST.ToString("N2", MainPage.indianCurancy);
            lblSGSTAmt.Text = dTCGST.ToString("N2", MainPage.indianCurancy);
            lblIGSTAmt.Text = dTIGST.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = (dTAmt+ dTCGST+ dTCGST+ dTIGST).ToString("N2", MainPage.indianCurancy);

        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex>=0)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    saveFileDialog.FileName = "Journal_Register";
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
               
            }
            catch
            { }
            btnExport.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("Journal Register");
                    Reporting.CryJournalRegisterReport objJournalRegisterReport = new Reporting.CryJournalRegisterReport();
                    objJournalRegisterReport.SetDataSource(dt);
                    objReport.myPreview.ReportSource = objJournalRegisterReport;
                    objReport.Show();

                    objReport.Close();
                    objReport.Dispose();
                }
                else
                {
                    MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            { }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable JournalRegister = new DataTable();

            try
            {
                JournalRegister.Columns.Add("CompanyName", typeof(String));
                JournalRegister.Columns.Add("HeaderImage", typeof(byte[]));
                JournalRegister.Columns.Add("BrandLogo", typeof(byte[]));
                JournalRegister.Columns.Add("HeaderName", typeof(String));
                JournalRegister.Columns.Add("Date", typeof(String));
                JournalRegister.Columns.Add("VoucherNo", typeof(String));
                JournalRegister.Columns.Add("PartyName", typeof(String));
                JournalRegister.Columns.Add("Amount", typeof(String));
                JournalRegister.Columns.Add("CGSTAmount", typeof(String));
                JournalRegister.Columns.Add("SGSTAmount", typeof(String));
                JournalRegister.Columns.Add("IGSTAmount", typeof(String));
                JournalRegister.Columns.Add("TotalAmt", typeof(String));
                JournalRegister.Columns.Add("GroupName", typeof(String));
                JournalRegister.Columns.Add("GSTNature", typeof(String));
                JournalRegister.Columns.Add("User", typeof(String));
                JournalRegister.Columns.Add("DateFrom", typeof(String));

                string strDate = "";
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    strDate = " Date period from " + txtFromDate.Text + " to " + txtToDate.Text;
                else
                    strDate = " Date period from " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " to " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow dRow = JournalRegister.NewRow();

                    dRow["CompanyName"] = MainPage.strCompanyName;
                    dRow["Headerimage"] = MainPage._headerImage;
                    dRow["BrandLogo"] = MainPage._brandLogo;
                    dRow["Headername"] = "Journal Register Report";
                    dRow["Date"] = Convert.ToDateTime(row.Cells["date"].Value).ToString("dd/MM/yyyy");
                    dRow["VoucherNo"] = row.Cells["voucherNo"].Value;
                    dRow["PartyName"] = row.Cells["partyName"].Value;
                    dRow["Amount"] = row.Cells["amount"].Value;
                    dRow["CGSTAmount"] = row.Cells["cgstAmt"].Value;
                    dRow["SGSTAmount"] = row.Cells["sgstAmt"].Value;
                    dRow["IGStAmount"] = row.Cells["igstAmt"].Value;
                    dRow["TotalAmt"] = row.Cells["totalAmt"].Value;
                    dRow["GroupName"] = row.Cells["groupName"].Value;
                    dRow["GSTNature"] = row.Cells["gstNature"].Value;
                    dRow["User"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    dRow["DateFrom"] = strDate;


                    JournalRegister.Rows.Add(dRow);
                   
                }
            }
            catch(Exception ex)
            { }

            return JournalRegister;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    
                    Reporting.CryJournalRegisterReport objJournalRegisterReport = new Reporting.CryJournalRegisterReport();
                    objJournalRegisterReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objJournalRegisterReport);
                    else
                    {
                        objJournalRegisterReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        objJournalRegisterReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                        objJournalRegisterReport.PrintToPrinter(1, false, 0, 0);
                    }

                    objJournalRegisterReport.Close();
                    objJournalRegisterReport.Dispose();
                } 
                else
                {
                    MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            { }
            btnPrint.Enabled = true;
        }

        private void JournalRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
