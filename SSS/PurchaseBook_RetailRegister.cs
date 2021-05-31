using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;


namespace SSS
{
    public partial class PurchaseBook_RetailRegister : Form
    {
        DataBaseAccess dba;    
        DataTable dtOrder = null, dtDetails = null;      
        public PurchaseBook_RetailRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();                   
        }

        public PurchaseBook_RetailRegister(string strPName)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();                          
                txtPurchaseParty.Text = strPName;
                GetAllData();
            }
            catch
            {
            }
        }

        private void PurchaseBookRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;           
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;
                    ClearAll();
                }
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

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }     

        private void txtPFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
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
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkPSNo.Focus();
                }                
                else
                    GetAllData();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkPSNo.Checked && (txtPFromSNo.Text == "" || txtPToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkPSNo.Focus();
                }               
                else
                    GetAllData();
            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            panelSearch.Visible=false;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkPSNo.Checked && txtPFromSNo.Text!="" && txtPToSNo.Text!="")
                    strQuery += " and (BillNo >= " + txtPFromSNo.Text + " and BillNo <=" + txtPToSNo.Text + ") ";
              
                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                }             

                if (txtInvoiceNo.Text != "")
                    strQuery += " and InvoiceNo Like ('%" + txtInvoiceNo.Text + "%') ";
                        
                if (txtBillCode.Text != "")
                    strQuery += " and BillCode='" + txtBillCode.Text + "' ";

                if (txtTransport.Text != "")
                    strQuery += " and TransportName='" + txtTransport.Text + "' ";

                if (txtGodown.Text != "")
                    strQuery += " and GodownName='" + txtGodown.Text + "' ";

                if (txtStockStatus.Text != "")
                    strQuery += " and StockStatus='" + txtStockStatus.Text + "' ";

                if (txtNetAmt.Text != "")
                    strQuery += " and NetAmt = " + txtNetAmt.Text + " ";

                if (txtScheme.Text != "")
                    strQuery += " and ISNULL(SchemeName,'')='" + txtScheme.Text + "' ";


                //if (chkWithScheme.Checked)
                //{
                //    strQuery += " and PurchasePartyID in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Other!='') ";
                //}
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Purchase Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strSubQuery = "";
                strSubQuery = CreateQuery();

                strQuery = " Select PR.*,(PurchasePartyID+' '+SM.Name) PartyName,CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt,SchemeName from PurchaseBook PR OUTER APPLY (Select SchemeName from PurchaseBookSecondary PBS OUTER APPLY (Select SchemeName from OrderBooking WHere (OrderCode+' '+CAST(OrderNo as varchar))=PBS.PONumber Group by SchemeName)OB Where PR.BillCode=PBS.BillCode and PR.BillNo=PBS.BillNo Group by SchemeName) PBS OUTER APPLY (Select Name from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=PR.PurchasePartyID) SM  OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='PURCHASE' and GD.BillCode=PR.BillCode and GD.BillNo=PR.BillNo Group by TaxType) GD Where PR.BillNo!=0 " + strSubQuery+" Order by BillNo,Date  ";

                dtDetails = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dtDetails);
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Gettting data in Purchase register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            double dGAmt = 0, dNetAmt = 0, dTGrossAmt = 0, dTNetAmt = 0;
            //chkAll.Checked = true;
            if (table.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(table.Rows.Count);
                int rowIndex = 0;
                string strSchemeName = "", strStatus = "" ;
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["chkID"].Value = true;
                    dGAmt = Convert.ToDouble(row["GrossAmt"]);
                    dNetAmt = Convert.ToDouble(row["NetAmt"]);
                    strSchemeName = Convert.ToString(row["SchemeName"]);
                    strStatus = Convert.ToString(row["StockStatus"]);
                    dTGrossAmt += dGAmt;
                    dTNetAmt += dNetAmt;

                    dgrdDetails.Rows[rowIndex].Cells["chkID"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["Date"];
                    dgrdDetails.Rows[rowIndex].Cells["billNo"].Value = row["BillCode"]+" "+ row["BillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                    dgrdDetails.Rows[rowIndex].Cells["invoiceNo"].Value = row["InvoiceNo"];
                    dgrdDetails.Rows[rowIndex].Cells["invoiceDate"].Value = row["InvoiceDate"];
                    dgrdDetails.Rows[rowIndex].Cells["purchaseType"].Value = row["PurchaseType"];
                    dgrdDetails.Rows[rowIndex].Cells["grossAmt"].Value = dGAmt;
                    dgrdDetails.Rows[rowIndex].Cells["igstAmt"].Value = row["IgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["cgstAmt"].Value = row["cgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["sgstAmt"].Value = row["sgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dNetAmt;
                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["schemeName"].Value = strSchemeName;
                    dgrdDetails.Rows[rowIndex].Cells["transport"].Value = row["TransportName"];
                    dgrdDetails.Rows[rowIndex].Cells["godown"].Value = row["GodownName"];
                    dgrdDetails.Rows[rowIndex].Cells["purchaseStatus"].Value = strStatus;

                    if (strSchemeName != "")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                    else  if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;

                    if(strStatus=="HOLD")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                    else if (strStatus == "PURCHASE IN")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;


                    rowIndex++;
                }                
            }

            lblGrossAmt.Text = dTGrossAmt.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dTNetAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            if (panelSearch.Visible)
                panelSearch.Visible = false;
            else
                panelSearch.Visible = true;
        }

        private void dgrdDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        
        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2 && e.RowIndex >= 0)
                {

                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowPurchaseBook(strNumber[0], strNumber[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Purchase Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowPurchaseBook(string strCode, string strBillNo)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                PurchaseBook_Retail_Merge objPurchase = new PurchaseBook_Retail_Merge(strCode, strBillNo);
                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchase.ShowInTaskbar = true;
                objPurchase.Show();
            }
            else if (MainPage._bCustomPurchase)
            {
                PurchaseBook_Retail_Custom objPurchase = new PurchaseBook_Retail_Custom(strCode, strBillNo);
                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchase.ShowInTaskbar = true;
                objPurchase.Show();
            }
            else
            {

                PurchaseBook_Trading objPurchaseBook = new PurchaseBook_Trading(strCode, strBillNo);
                objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchaseBook.ShowInTaskbar = true;
                objPurchaseBook.Show();
            }       
        }
              
        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentRow.Index >= 0)
                {
                    if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                        else
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (columnIndex == 2)
                        {
                            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            string[] strNumber = strInvoiceNo.Split(' ');
                            if (strNumber.Length > 1)
                            {
                                ShowPurchaseBook(strNumber[0], strNumber[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Down Event of Purchase Grid view  in Show Purchase Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PURCHASE REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objSales;
                        objShow.ShowDialog();

                        objSales.Close();
                        objSales.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
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
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("Party", typeof(String));
                myDataTable.Columns.Add("IColumn", typeof(String));
                myDataTable.Columns.Add("IIColumn", typeof(String));
                myDataTable.Columns.Add("IIIColumn", typeof(String));
                myDataTable.Columns.Add("IVColumn", typeof(String));
                myDataTable.Columns.Add("VColumn", typeof(String));
                myDataTable.Columns.Add("VIColumn", typeof(String));
                myDataTable.Columns.Add("VIIColumn", typeof(String));
                myDataTable.Columns.Add("VIIIColumn", typeof(String));
                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalGrossAmt", typeof(String));
                myDataTable.Columns.Add("TotalNetAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["chkID"].Value))
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = MainPage.strGRCompanyName;
                        if (chkDate.Checked)
                            row["DatePeriod"] = "From " + txtFromDate.Text + "   To   " + txtToDate.Text;
                        else
                            row["DatePeriod"] = "";
                      
                            row["Party"] = "PURCHASE REGISTER";


                        for (int colIndex = 2; colIndex < dgrdDetails.Columns.Count; colIndex++)
                        {
                            row[colIndex + 1] = dgrdDetails.Columns[colIndex].HeaderText;
                            row[colIndex + 9] = dr.Cells[colIndex].Value;
                            if (colIndex == 9)
                                break;
                        }

                        row["TotalGrossAmt"] = lblGrossAmt.Text;
                        row["TotalNetAmt"] = lblNetAmt.Text;
                        row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        myDataTable.Rows.Add(row);
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["chkID"].Value = chkAll.Checked;
            }
            catch
            {
            }
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
            lblGrossAmt.Text = lblNetAmt.Text = "0.00";
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
            ClearAll();
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
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
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objSales);
                        else
                        {
                            objSales.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objSales.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objSales.PrintToPrinter(1, false, 0, 0);
                        }
                        objSales.Close();
                        objSales.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }



        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
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
                    saveFileDialog.FileName = "PurchaseBook_Retail";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                        MessageBox.Show("Export Cancled...");

                    //((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }



        //private void ExportDataSetToExcel(DataSet ds)
        //{
        //    try
        //    {
        //        Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
        //        Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

        //        // Loop over DataTables in DataSet.
        //        DataTableCollection collection = ds.Tables;

        //        for (int i = dgrdDetails.Rows.Count; i > 0; i--)
        //        {
        //            Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
        //            Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;
        //            //Create Excel Sheets
        //            xlSheets = ExcelApp.Sheets;
        //            xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
        //                           Type.Missing, Type.Missing, Type.Missing);

        //            System.Data.DataTable table = collection[i - 1];
        //            xlWorksheet.Name = table.TableName;

        //            for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
        //            {
        //                ExcelApp.Cells[1, j] = dgrdDetails.Columns[j - 1].HeaderText;
        //            }

        //            // Storing Each row and column value to excel sheet
        //            for (int k = 0; k < dgrdDetails.Rows.Count; k++)
        //            {
        //                for (int l = 0; l < dgrdDetails.Columns.Count; l++)
        //                {
        //                    ExcelApp.Cells[k + 2, l + 1] =
        //                    dgrdDetails.Rows[k].Cells[l].Value.ToString();
        //                }
        //            }
        //            ExcelApp.Columns.AutoFit();
        //        }
        //        ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
        //        ExcelApp.Visible = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Purchase Bill";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\PurchaseRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.SaleRegister objRegister = new Reporting.SaleRegister();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
                    objRegister.Close();
                    objRegister.Dispose();
                }
                else
                    strFileName = "";
            }
            catch
            {
                strFileName = "";
            }
            return strFileName;
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtScheme.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void PurchaseBook_RetailRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bPurchaseReport)
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

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransport.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtStockStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASESTATUS", "SEARCH PURCHASE STATUS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStockStatus.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtGodown_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH GODOWN NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGodown.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
        
    }
}
