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
    public partial class CourierBookRegister : Form
    {
        DataBaseAccess dba;
        public CourierBookRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void CourierBookRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else
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
                    dgrdDetails.Rows.Clear();
                    lblCount.Text = "0";
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

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkSSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtSFromSNo.ReadOnly = txtSToSNo.ReadOnly = !chkSSNo.Checked;
            txtSFromSNo.Text = txtSToSNo.Text = "";
        }

        private void chkSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtCFromSNo.ReadOnly = txtCToSNo.ReadOnly = !chkSNo.Checked;
            txtCFromSNo.Text = txtCToSNo.Text = "";
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
            dgrdDetails.Rows.Clear();
            lblCount.Text = "0";
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtCourierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("COURIERNAME", "SEARCH COURIER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCourierName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtDocType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DOCUMENTTYPE", "SEARCH DOC TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtDocType.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("COURIERCODE", "SEARCH COURIER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCourierNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            SearchCourierData();
            btnGo.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            SearchCourierData();
            btnSearch.Enabled = true;
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = true;
            txtCourierName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void SearchCourierData()
        {
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkSNo.Checked && (txtCFromSNo.Text == "" || txtCToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSNo.Focus();
                }
                else if ((chkSSNo.Checked && (txtSFromSNo.Text == "" || txtSToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter sales serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSSNo.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
        }

        private string CreateQuery(ref string strInQuery, ref string strOutQuery)
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

                if (chkSNo.Checked && txtCFromSNo.Text != "" && txtCToSNo.Text != "")
                    strQuery += " and (SNo >= " + txtCFromSNo.Text + " and SNo <=" + txtCToSNo.Text + ") ";

                if (chkSSNo.Checked && txtSFromSNo.Text != "" && txtSToSNo.Text != "")
                    strOutQuery += " and (SaleBillNo >= " + txtSFromSNo.Text + " and SaleBillNo <=" + txtSToSNo.Text + ") ";


                if (txtPartyName.Text != "")
                {
                    string[] strFullName = txtPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strInQuery += " and SalePartyID='" + strFullName[0].Trim() + "'";
                        strOutQuery += " and SalePartyID='" + strFullName[0].Trim() + "'";
                    }
                }

                if (txtCourierName.Text != "")
                    strQuery += " and CourierName='" + txtCourierName.Text + "' ";

                if (txtCourierNo.Text != "")
                    strQuery += " and CourierNo Like('%" + txtCourierNo.Text + "%') ";

                if (txtDocType.Text != "")
                    strQuery += " and DocType ='" + txtDocType.Text + "' ";

                if (txtBillCode.Text != "")
                {
                    strInQuery += " and CourierCode='" + txtBillCode.Text + "' ";
                    strOutQuery += " and SCode='" + txtBillCode.Text + "' ";
                }

                if (rdoWithCNo.Checked)
                    strQuery += " and CourierNo!='' ";
                else if (rdoWithoutCNo.Checked)
                    strQuery += " and CourierNo='' ";

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Courier Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strSubQuery = "", strInQuery = "", strOutQuery = "";
                strSubQuery = CreateQuery(ref strInQuery, ref strOutQuery);

                strQuery = " Select ID,CStatus,SCode,CDate,SerialNo,SBillNo,PartyName,CourierNo,CourierName,DocType,Station,Remark,CreatedBy,SNo from ( ";
                if (rdoAll.Checked || rdoOut.Checked)
                    strQuery += " Select ID,'OUT' CStatus,(SCode+' '+CAST(SNo as varchar))SCode,CONVERT(varchar,Date,103) CDate,(SCode+' '+CAST(SNo as varchar)+' '+SerialCode) SerialNo,(CASE When SaleBillNo>0 then SaleBillCode+' '+CAST(SaleBillNo as varchar) else '' end) SBillNo,(SalePartyID+' '+Name) PartyName,CourierNo,CourierName,DocType,Station,Remark,UserName as CreatedBy,Date,SNo from CourierRegister CR Outer Apply (Select Name from SupplierMaster SM Where AreaCode+AccountNo=SalePartyID) SM  Where SNo!=0 " + strSubQuery + strOutQuery;
                if (rdoAll.Checked)
                    strQuery += " Union All ";
                if (rdoAll.Checked || rdoIN.Checked)
                    strQuery += " Select ID,'IN' CStatus,(CourierCode+' '+CAST(SNo as varchar))SCode,CONVERT(varchar,Date,103) CDate,(CourierCode+' '+CAST(SNo as varchar)+' '+SCode) SerialNo,'' SBillNo,(SalePartyID+' '+Name) PartyName,CourierNo,CourierName,DocType,Station,Remark,CreatedBy,Date,SNo from CourierRegisterIn CR Outer Apply (Select Name from SupplierMaster SM Where AreaCode+AccountNo=SalePartyID) SM Where SNo!=0 " + strSubQuery + strInQuery;

                strQuery += " ) Courier Order By CStatus,SNo,SCode ";

                DataTable dt = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dt);              
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Gettting data in Courier register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            lblCount.Text = "0";
            double dCount = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                string strOldID = "", strNewID = "",strStatus="",strOldStatus="";
                foreach (DataRow row in dt.Rows)
                {
                    strNewID = Convert.ToString(row["SCode"]);
                    strStatus = Convert.ToString(row["CStatus"]);

                    if (strNewID != strOldID)
                    {
                        dCount++;
                        strOldID = strNewID;
                        strOldStatus = strStatus;
                    }
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                    dgrdDetails.Rows[rowIndex].Cells["cStatus"].Value = strStatus;
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["CDate"];
                    dgrdDetails.Rows[rowIndex].Cells["serialNo"].Value = row["SerialNo"] ;
                    dgrdDetails.Rows[rowIndex].Cells["saleBillNo"].Value = row["SBillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                    dgrdDetails.Rows[rowIndex].Cells["courierNo"].Value = row["CourierNo"];
                    dgrdDetails.Rows[rowIndex].Cells["courierName"].Value = row["CourierName"];
                    dgrdDetails.Rows[rowIndex].Cells["docType"].Value = row["DocType"];
                    dgrdDetails.Rows[rowIndex].Cells["station"].Value = row["Station"];
                    dgrdDetails.Rows[rowIndex].Cells["remark"].Value = row["Remark"];
                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    if (strStatus == "IN")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.BurlyWood;
                 
                    rowIndex++;
                }
            }

            lblCount.Text = dCount.ToString("N0", MainPage.indianCurancy);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 3)
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value),strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value), strStatus = Convert.ToString(dgrdDetails.CurrentRow.Cells["cStatus"].Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            if(strStatus=="IN")
                                ShowCourierIN(strNumber[0], strID);
                            else
                                ShowCourierOUT(strNumber[0], strID);
                        }
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strInvoiceNo != "")
                        {
                            string[] strNumber = strInvoiceNo.Split(' ');
                            if (strNumber.Length > 1)
                            {
                                ShowSaleBook(strNumber[0], strNumber[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Courier Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowCourierIN(string strCode, string strID)
        {
            CourierBookIN objCourierBookIN = new CourierBookIN(strCode, strID);
            objCourierBookIN.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objCourierBookIN.ShowInTaskbar = true;
            objCourierBookIN.Show();
        }

        private void ShowCourierOUT(string strCode, string strID)
        {
            CourierBookOut objCourierBookOut = new CourierBookOut(strCode, strID);
            objCourierBookOut.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objCourierBookOut.ShowInTaskbar = true;
            objCourierBookOut.Show();
        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                dba.ShowSaleBookPrint(strCode, strBillNo,false, false);
            }
            else
            {
                SaleBook objSale = new SaleBook(strCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.Show();
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int colIndex = dgrdDetails.CurrentCell.ColumnIndex, rowIndex = dgrdDetails.CurrentRow.Index;
                if (colIndex >= 0 && rowIndex >= 0)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (colIndex == 3)
                        {
                            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value), strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value), strStatus = Convert.ToString(dgrdDetails.CurrentRow.Cells["cStatus"].Value);
                            string[] strNumber = strInvoiceNo.Split(' ');
                            if (strNumber.Length > 1)
                            {
                                if (strStatus == "IN")
                                    ShowCourierIN(strNumber[0], strID);
                                else
                                    ShowCourierOUT(strNumber[0], strID);
                            }
                        }
                        else if (colIndex == 4)
                        {
                            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            if (strInvoiceNo != "")
                            {
                                string[] strNumber = strInvoiceNo.Split(' ');
                                if (strNumber.Length > 1)
                                {
                                    ShowSaleBook(strNumber[0], strNumber[1]);
                                }
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                        {
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Courier Record", ex.Message };
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
                        Reporting.CourierRegisterReport objReport = new Reporting.CourierRegisterReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("COURIER REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objReport;

                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
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
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    if (chkDate.Checked)
                        row["DatePeriod"] = "From " + txtFromDate.Text + "   To   " + txtToDate.Text;
                    else
                        row["DatePeriod"] = "";

                    if (txtPartyName.Text != "")
                        row["Party"] = "COURIER REGISTER OF  :  " + txtPartyName.Text;
                    else
                        row["Party"] = "COURIER REGISTER";

                    row["IColumnValue"] = dr.Cells["date"].Value;
                    row["IIColumnValue"] = dr.Cells["serialNo"].Value+" ("+dr.Cells["cStatus"].Value+")";
                    row["IIIColumnValue"] = dr.Cells["saleBillNo"].Value;
                    row["IVColumnValue"] = dr.Cells["partyName"].Value;
                    row["VColumnValue"] = dr.Cells["courierName"].Value + "(" + dr.Cells["courierNo"].Value + ")";
                    row["VIColumnValue"] = dr.Cells["docType"].Value;
                    row["VIIColumnValue"] = dr.Cells["station"].Value;
                    row["VIIIColumnValue"] = dr.Cells["remark"].Value;                   

                    row["TotalGrossAmt"] = lblCount.Text;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);
                }
            }
            catch
            {
            }
            return myDataTable;
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
                        Reporting.CourierRegisterReport objReport = new Reporting.CourierRegisterReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }
                        objReport.Close();
                        objReport.Dispose();
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
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnExport.Enabled = false;
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
                    saveFileDialog.FileName = "Courier_Book_Register";
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
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Courier_Data";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\CourierRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.CourierRegisterReport objReport = new Reporting.CourierRegisterReport();
                    objReport.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                    objReport.Close();
                    objReport.Dispose();
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

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (txtPartyName.Text != "")
                {
                    string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    //if (strEmailID != "")
                    //{
                    strPath = CreatePDFFile();
                    if (strPath != "")
                    {
                        strSubject = "COURIER REPORT REGISTER FROM " + MainPage.strCompanyName;
                        strBody = "We are sending Courier Register, which is attached with this mail, Please Find it.";
                        SendingEmailPage objEmail = new SendingEmailPage(true, txtPartyName.Text, "", strSubject, strBody, strPath,"","COURIER REPORT");
                        objEmail.ShowDialog();
                    }
                    //}
                }
                else
                {
                    MessageBox.Show("Sorry ! Party Name can't be blank ", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPartyName.Focus();
                }
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void CourierBookRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bCourierReport)
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {

                //SearchData objSearch = new SearchData("COURIERNAME", "SEARCH COURIER NAME", Keys.Space);
                //objSearch.ShowDialog();
                //txtCourierName.Text = objSearch.strSelectedData;
                e.Cancel = true;
            }
            catch { }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

            }
            catch { }
        }
    }
}
