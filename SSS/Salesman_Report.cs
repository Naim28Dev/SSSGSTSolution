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
    public partial class Salesman_Report : Form
    {
        DataBaseAccess dba;
        public Salesman_Report()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void Agent_Report_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {                
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }              

        private void txtSalesMan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESMANMARKETERNAME", "SEARCH SALES MAN/MARKETER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesMan.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }
     
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtPFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;

                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkPSNo.Checked && (txtPFromSNo.Text == "" || txtPToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter  serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkPSNo.Focus();
                }
                else
                    GetAllData();

            }
            catch { }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "",strSubQuery="";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strSubQuery += " and  (SB.Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and SB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                    strSubQuery += " and (SB.BillNo >= " + txtPFromSNo.Text + " and SB.BillNo <=" + txtPToSNo.Text + ") ";

                if (txtSalesMan.Text != "")
                {
                    string[] str = txtSalesMan.Text.Split(' ');
                    strSubQuery += " and SalesMan = '" + str[0] + "'  ";
                }          

                if (txtBillCode.Text != "")
                    strSubQuery += " and SB.BillCode='" + txtBillCode.Text + "' ";

                string strColumName = "", strGroupBy = "",strOrderBy = "";

                if (chkBillCode.Checked)
                    strColumName += "BillCode,";
                if (chkBillNo.Checked)
                    strColumName += "BillNo,";
                if (chkSalesMan.Checked)
                    strColumName += "SalesMan,";
                if (chkBillDate.Checked)
                    strColumName += "BillDate,";
                if (strColumName != "")
                {
                    strGroupBy = strColumName.Substring(0, strColumName.Length - 1);
                    strOrderBy = " ORDER BY " + strGroupBy;
                    strGroupBy = " GROUP BY " + strGroupBy;
                }

                strQuery = "Select " + strColumName + @" SUM(TotalQty)Qty,SUM(NetAmt)Amount,SUM(SaleInc)SaleInc from ( ";
                if (rdoAll.Checked || rdoSale.Checked)
                    strQuery += @" Select SB.BillCode,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) as BillNo,CONVERT(varchar,SB.Date,103)BillDate,dbo.GetFullName(SBS.SalesMan) SalesMan,SUM(SBS.Qty) as TotalQty,SUM(SBS.Amount) NetAmt,SUM((CASE WHEN (SBS.SaleIncentive LIKE '%\%%' ESCAPE '\') then ((SBS.Amount*CAST(Replace(SBS.SaleIncentive,'%','') as Money))/100) else (SBS.Qty*CAST(SBS.SaleIncentive as Money)) end))SaleInc from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo WHere ISNULL(SBS.SalesMan,'DIRECT')!='DIRECT' AND ISNULL(SBS.SalesMan,'')!=''" + strSubQuery + " Group by SB.BillCode,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) ,CONVERT(varchar,SB.Date,103),SBS.SalesMan ";
                if (rdoAll.Checked)
                    strQuery += " UNION ALL ";
                if (rdoAll.Checked || rdoSaleReturn.Checked)
                    strQuery += @" Select SB.BillCode,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) as BillNo,CONVERT(varchar,SB.Date,103)BillDate,dbo.GetFullName(SBS.SalesMan) SalesMan,-SUM(SBS.Qty) as TotalQty,-SUM(SBS.Amount) NetAmt,-SUM((CASE WHEN (ISNULL(_SBS.SaleIncentive,0) LIKE '%\%%' ESCAPE '\') then ((SBS.Amount*CAST(Replace(ISNULL(_SBS.SaleIncentive,0),'%','') as Money))/100) else (SBS.Qty*CAST(ISNULL(_SBS.SaleIncentive,0) as Money)) end))SaleInc from SaleReturn SB inner join SaleReturnDetails SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo OUTER APPLY (Select Top 1 _SBS.SaleIncentive from SalesBookSecondary _SBS Where _SBS.BillNo=SB.SaleBillNo and _SBS.BillCode=SB.SaleBillCode and _SBS.BarCode=SBS.BarCode and _SBS.ItemName=SBS.ItemName and _SBS.Variant1=SBS.Variant1 and _SBS.Variant2=SBS.Variant2)_SBS WHere SB.ReturnType='RETAIL' and EntryType!='DEBITNOTE' and ISNULL(_SBS.SaleIncentive,'')!='' AND ISNULL(SBS.SalesMan,'DIRECT')!='DIRECT' AND ISNULL(SBS.SalesMan,'')!='' " + strSubQuery + " Group by SB.BillCode,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) ,CONVERT(varchar,SB.Date,103),SalesMan ";
                strQuery += " )_Sales " + strGroupBy;

                if (txtSalesMan.Text != "")
                {
                    strSubQuery = strSubQuery.Replace("and SalesMan =", "and SM.SalesMan =");
                }
                if (MainPage.strSoftwareType != "RETAIL")
                {
                    strQuery += " UNION ALL  "
                                + " SELECT " + strColumName + "SUM(Qty)Qty,SUM(Amount)Amount,SUM(SaleInc)SaleInc FROM ("
                                + " SELECT SB.BillCode BillCode,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) as BillNo,CONVERT(varchar,SB.Date,103) BillDate,SM.SaleManID +' '+SM.SalesMan SalesMan,"
                                + " (ISNULL(SB.TotalQty,0)- ISNULL(SR.TotalQty,0)) Qty ,(ISNULL(SB.NetAmt,0) - ISNULL(SR.NetAmt,0)) Amount,CAST((CASE WHEN ISNULL(SM.IncentivePer,0) > 0 "
                                + " then ((ISNULL(SB.NetAmt, 0) - ISNULL(SR.NetAmt, 0)) * ISNULL(SM.IncentivePer, 0)) / 100    "
                                + "  else 0 end) as Numeric(18,2)) SaleInc "
                                + " FROM SalesBook SB LEFT JOIN SaleReturn SR on SB.BillCode = SR.SaleBillCode AND SR.SaleBillNo = SB.BillNo "
                                + " OUTER APPLY (SELECT(CASE WHEN ISNULL(SaleIncentive, '') != '' then SaleIncentive else 0 end) IncentivePer "
                                + " , (SS.AreaCode + SS.AccountNo) SaleManID, Name SalesMan FROM SupplierMaster as SS "
                                + " WHERE(SS.AreaCode + SS.AccountNo) = SB.Description_1 ) SM "
                                + " WHERE ISNULL(SM.SalesMan, '') != '' " + strSubQuery.Replace("SM.SalesMan =", "SB.Description_1 =")
                                + " )Recs " + strGroupBy;
                }
                strQuery = "SELECT "+ strColumName +"SUM(Qty)Qty,SUM(Amount)Amount,SUM(SaleInc)SaleInc FROM ( " + strQuery + " ) Final "+ strGroupBy + strOrderBy;
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = CreateQuery();
                DataTable dtDetails = dba.GetDataTable(strQuery);
                BindColumn();
                BindDataTable(dtDetails);

            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void BindColumn()
        {

            dgrdDetails.Columns.Clear();

            CreateGridviewColumn("sno", "S.No", "RIGHT", 50);           
            if (chkBillCode.Checked)
                CreateGridviewColumn("BillCode", "Bill Code", "LEFT", 100);
            if (chkBillNo.Checked)
                CreateGridviewColumn("BillNo", "Bill No", "LEFT", 130);
            if (chkBillDate.Checked)
                CreateGridviewColumn("BillDate", "Bill Date", "LEFT", 100);
            if (chkSalesMan.Checked)
                CreateGridviewColumn("SalesMan", "Sales Man", "LEFT", 170);
           
            CreateGridviewColumn("Qty", "Net  Qty", "RIGHT", 80);
            CreateGridviewColumn("Amount", "Net Amt", "RIGHT", 120);
            CreateGridviewColumn("SaleInc", "Sale Inc.", "RIGHT", 100);
        }

        private void BindDataTable(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            if (table.Rows.Count > 0)
                dgrdDetails.Rows.Add(table.Rows.Count);

            int _rowIndex = 0;
            double dQty = 0, dTQty = 0, dTAmt = 0, dAmt = 0,dSaleInc=0,dTSaleInc=0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                    if (chkBillCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BillCode"].Value = row["BillCode"];
                    if (chkBillNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BillNo"].Value = row["BillNo"];
                    if (chkSalesMan.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["SalesMan"].Value = row["SalesMan"];
                    if (chkBillDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BillDate"].Value = row["BillDate"];

                    dTQty += dQty = dba.ConvertObjectToDouble(row["Qty"]);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    dTSaleInc += dSaleInc = dba.ConvertObjectToDouble(row["SaleInc"]);

                    dgrdDetails.Rows[_rowIndex].Cells["Qty"].Value = dQty;
                    dgrdDetails.Rows[_rowIndex].Cells["Amount"].Value = dAmt;
                    dgrdDetails.Rows[_rowIndex].Cells["SaleInc"].Value = (dSaleInc > 0) ? dSaleInc.ToString("N2",MainPage.indianCurancy) :"";
                    if (dSaleInc < 0)
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                    _rowIndex++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            lblTotalQty.Text = dTQty.ToString("N2", MainPage.indianCurancy);
            lblTotalAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
            lblTotalSalesInc.Text = dTSaleInc.ToString("N2", MainPage.indianCurancy);
        }

        private void CreateGridviewColumn(string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
                _column.Name = strColName;
                _column.HeaderText = strColHeader;
                _column.Width = _width;
                _column.SortMode = DataGridViewColumnSortMode.Automatic;
                if (strAlign == "LEFT")
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                }
                else
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
                    if (_width != 50)
                        _column.DefaultCellStyle.Format = "N2";
                }
                _column.CellTemplate = dataGridViewCell;
                dgrdDetails.Columns.Add(_column);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
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
                    saveFileDialog.FileName = "Salesman_Report";
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Bill No")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            dba.ShowTransactionBook("SALES", strNumber[0], strNumber[1]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of SalesMan_Report Grid view  in Show Sales Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void Salesman_Report_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                dba.EnableCopyOnClipBoard(dgrdDetails);

            }
            catch { }
        }

        private DataTable GetTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("CompanyName", typeof(string));
            _dt.Columns.Add("lblSalesMan", typeof(string));
            _dt.Columns.Add("SalesMan", typeof(string));
            _dt.Columns.Add("lblBillCode", typeof(string));
            _dt.Columns.Add("BillCode", typeof(string));
            _dt.Columns.Add("lblBillNo", typeof(string));
            _dt.Columns.Add("BillNo", typeof(string));
            _dt.Columns.Add("lblBillDate", typeof(string));
            _dt.Columns.Add("BillDate", typeof(string));
            _dt.Columns.Add("NetQty", typeof(string));
            _dt.Columns.Add("NetAmount", typeof(string));
            _dt.Columns.Add("Incentive", typeof(string));
            _dt.Columns.Add("SNo", typeof(string));
            _dt.Columns.Add("TotalQty", typeof(string));
            _dt.Columns.Add("TotalAmount", typeof(string));
            _dt.Columns.Add("TotalIncentive", typeof(string));
            _dt.Columns.Add("UserName", typeof(string));

            return _dt;
        }
        private DataTable CreateDataTable()
        {
            DataTable _dt = GetTable();
            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                DataRow _row = _dt.NewRow();
                _row["CompanyName"] = MainPage.strPrintComapanyName;

                if (dgrdDetails.Columns.Contains("SalesMan"))
                {
                    _row["lblSalesMan"] = "SalesMan";
                    _row["SalesMan"] = Convert.ToString(dr.Cells["SalesMan"].Value);
                }
                if (dgrdDetails.Columns.Contains("BillCode"))
                {
                    _row["lblBillCode"] = "BillCode";
                    _row["BillCode"] = Convert.ToString(dr.Cells["BillCode"].Value);
                }
                if (dgrdDetails.Columns.Contains("BillNo"))
                {
                    string[] str = Convert.ToString(dr.Cells["BillNo"].Value).Split(' ');
                    _row["lblBillNo"] = "BillNo";
                    _row["BillNo"] = str[1];
                }
                if (dgrdDetails.Columns.Contains("BillDate"))
                {
                    _row["lblBillDate"] = "BillDate";
                    _row["BillDate"] = Convert.ToString(dr.Cells["BillDate"].Value);
                }
                _row["NetQty"] = Convert.ToString(dr.Cells["Qty"].Value);
                _row["NetAmount"] = Convert.ToString(dr.Cells["Amount"].Value);
                _row["Incentive"] = Convert.ToString(dr.Cells["SaleInc"].Value);
                _row["SNo"] = dr.Index + 1;
                _row["TotalQty"] = dba.ConvertObjectToDouble(lblTotalQty.Text).ToString("N2");
                _row["TotalAmount"] = dba.ConvertObjectToDouble(lblTotalAmt.Text).ToString("N2");
                _row["TotalIncentive"] = dba.ConvertObjectToDouble(lblTotalSalesInc.Text).ToString("N2");
                _row["UserName"] = "Printed By : " + MainPage.strLoginName;

                _dt.Rows.Add(_row);
            }
            return _dt;
        }
        private void PrintPreviewReport(bool bPrint)
        {
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = 1;
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;

            Reporting.SalesManReport objSalesManReport = new Reporting.SalesManReport();
            objSalesManReport.SetDataSource(CreateDataTable());
            if (bPrint)
            {
                if(MainPage._PrintWithDialog)
                dba.PrintWithDialog(objSalesManReport, false);
                else
                    objSalesManReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
            }
            else
            {
                Reporting.ShowReport objReport = new Reporting.ShowReport("SALES MAN REPORT PREVIEW");
                objReport.myPreview.ReportSource = objSalesManReport;
                objReport.ShowDialog();

            }
            objSalesManReport.Close();
            objSalesManReport.Dispose();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(false);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Man Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(true);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Man Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach(CheckBox c in grpBoxChk.Controls.OfType<CheckBox>())
            {
                c.Checked = chkAll.Checked;
            }
        }
    }
}
