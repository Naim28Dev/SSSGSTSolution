using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class StockTransferRegister : Form
    {
        DataBaseAccess dba;
        public StockTransferRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            SetCategory();
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant2"].Visible = false;
            }
            catch
            {
            }
        }

        private void StockTransferRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void txtMaterialCenter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH MATERIAL CENTER", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMaterialCenter.Text = objSearch.strSelectedData;
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
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
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
                    SearchData objSearch = new SearchData("STOCKCODE", "SEARCH STOCK TR. CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSourceBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void chkSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSNo.ReadOnly = txtToSNo.ReadOnly = !chkSNo.Checked;
            txtFromSNo.Text = txtToSNo.Text = "";
        }

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
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
            Search_Data();
            btnGo.Enabled = true;
        }

        private void Search_Data()
        {
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkSNo.Checked && (txtFromSNo.Text == "" || txtToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSNo.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
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

                if (chkSNo.Checked && txtFromSNo.Text != "" && txtToSNo.Text != "")
                    strQuery += " and (ST.BillNo >= " + txtFromSNo.Text + " and ST.BillNo <=" + txtToSNo.Text + ") ";

                if (txtMaterialCenter.Text != "")
                    strQuery += " and ([FromMCentre]='" + txtMaterialCenter.Text + "' OR [ToMCentre]='" + txtMaterialCenter.Text + "')  ";

                if (txtBillCode.Text != "")
                    strQuery += " and ST.BillCode='" + txtBillCode.Text + "' ";

                if (txtItemName.Text != "")
                    strQuery += " and ItemName='" + txtItemName.Text + "' ";

                if (txtRemark.Text != "")
                    strQuery += " and Remark Like('%" + txtRemark.Text + "%') ";

                if (txtSourceBillNo.Text != "")
                    strQuery += " and [SourceBillNo] Like('%" + txtSourceBillNo.Text + "%') ";

                if (rdoStockIn.Checked)
                    strQuery += " and StockType='IN' ";
                else if (rdoStockOut.Checked)
                    strQuery += " and StockType='OUT' ";
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Stock Transfer Register", ex.Message };
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

                strQuery = " Select ST.*,Convert(varchar,ST.Date,103)SDate,STS.[ItemName],STS.[Variant1],STS.[Variant2],STS.[Variant3],STS.[Variant4],STS.[Variant5],STS.[Qty],STS.[Unit],STS.[Rate],STS.[Amount] from StockTransfer ST inner join StockTransferSecondary STS on ST.BillCOde=STS.BillCode and ST.BillNo=STS.BIllNo Where ST.BillNo>0  " + strSubQuery + " Order by ST.BillNo,ST.Date  ";

                DataTable dtDetails = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dtDetails);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in gettting data in stock transfer register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            double dAmt = 0, dQty = 0, dTQty = 0, dTAmt = 0;
            if (table.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(table.Rows.Count);
                int rowIndex = 0;
                string strID = "", strNewID = "";

                foreach (DataRow row in table.Rows)
                {
                    strNewID = Convert.ToString(row["ID"]);

                    dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    dTQty += dQty = Convert.ToDouble(row["Qty"]);
                    if (strID != strNewID)
                    {
                        strID = strNewID;
                    }
                    else
                        dgrdDetails.Rows[rowIndex].Visible = false;

                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = strNewID;
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["SDate"];
                    dgrdDetails.Rows[rowIndex].Cells["billNo"].Value = row["BillCode"] + " " + row["BillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["fromMC"].Value = row["FromMCentre"];
                    dgrdDetails.Rows[rowIndex].Cells["toMC"].Value = row["ToMCentre"];
                    dgrdDetails.Rows[rowIndex].Cells["stockType"].Value = row["StockType"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = dQty;
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = row["Rate"];
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["remark"].Value = row["Remark"];
                    dgrdDetails.Rows[rowIndex].Cells["sourceBillNo"].Value = row["SourceBillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["createdby"].Value = row["Createdby"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedby"].Value = row["Updatedby"];

                    rowIndex++;
                }
            }

            lblQty.Text = dTQty.ToString("N2", MainPage.indianCurancy);
            lblAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);

        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            btnExpand.Enabled = false;
            try
            {
                if (btnExpand.Text == "Expand")
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        if (!row.Visible)
                            row.Visible = true;
                    }
                    btnExpand.Text = "Collapse";
                }
                else
                {
                    string strID = "", strNewID = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strNewID != "")
                        {
                            if (strID == strNewID)
                                row.Visible = false;
                        }
                        strNewID = strID;
                    }
                    btnExpand.Text = "Expand";
                }
            }
            catch
            {
            }
            btnExpand.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowStockTransfer(strNumber[0], strNumber[1]);
                    }
                }
            }
            catch { }
        }

        private void ShowStockTransfer(string strCode, string strBillNo)
        {
            try
            {
                StockTransferVoucher objSale = new StockTransferVoucher(strCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.Show();

            }
            catch { }
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
                        Reporting.CryStockTransferRegister objStockTransferRegister = new Reporting.CryStockTransferRegister();
                        objStockTransferRegister.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("STOCK TRANSFER REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objStockTransferRegister;
                        objShow.ShowDialog();

                        objStockTransferRegister.Close();
                        objStockTransferRegister.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! No record found!! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Sorry ! No record found!! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                myDataTable.Columns.Add("HeaderName", typeof(String));
                myDataTable.Columns.Add("SNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("FromMCenter", typeof(String));
                myDataTable.Columns.Add("ToMCenter", typeof(String));
                myDataTable.Columns.Add("StockType", typeof(String));
                myDataTable.Columns.Add("ItemName", typeof(String));
                myDataTable.Columns.Add("Variant1", typeof(String));
                myDataTable.Columns.Add("Variant2", typeof(String));
                myDataTable.Columns.Add("Variant3", typeof(String));
                myDataTable.Columns.Add("Variant4", typeof(String));
                myDataTable.Columns.Add("Variant5", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("Rate", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("Remark", typeof(String));
                myDataTable.Columns.Add("SourceBillNo", typeof(String));
                myDataTable.Columns.Add("CreatedBy", typeof(String));
                myDataTable.Columns.Add("UpdatedBy", typeof(String));
                myDataTable.Columns.Add("ID", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));
                myDataTable.Columns.Add("TotalAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                int SNo = 1;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (dr.Visible == true)
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = MainPage.strCompanyName;
                        row["HeaderName"] = "STOCK TRANSFER REGISTER";
                        row["Date"] = dr.Cells["date"].Value;
                        row["SNo"] = SNo;
                        row["BillNo"] = dr.Cells["billNo"].Value;
                        row["FromMCenter"] = dr.Cells["fromMC"].Value;
                        row["ToMCenter"] = dr.Cells["toMC"].Value;
                        row["StockType"] = dr.Cells["stockType"].Value;
                        row["ItemName"] = dr.Cells["itemName"].Value;
                        row["Variant1"] = dr.Cells["variant1"].Value;
                        row["Variant2"] = dr.Cells["variant2"].Value;
                        row["Qty"] = dr.Cells["qty"].Value;
                        row["Rate"] = dr.Cells["rate"].Value;
                        row["Amount"] = dr.Cells["amount"].Value;
                        row["Remark"] = dr.Cells["remark"].Value;
                        row["SourceBillNo"] = dr.Cells["sourceBillNo"].Value;
                        row["TotalQty"] = lblQty.Text;
                        row["TotalAmt"] = lblAmt.Text;
                        row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        myDataTable.Rows.Add(row);
                        SNo++;
                    }
                }
            }
            catch (Exception ex)
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
                        Reporting.CryStockTransferRegister objStockTransferRegister = new Reporting.CryStockTransferRegister();
                        objStockTransferRegister.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objStockTransferRegister);
                        else
                            objStockTransferRegister.PrintToPrinter(1, false, 0, 0);
                    }
                    else
                        MessageBox.Show("Sorry ! No record found!! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Sorry ! No record found!! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    saveFileDialog.FileName = "Stock_Transfer_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
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

        private void StockTransferRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }
    }
}
