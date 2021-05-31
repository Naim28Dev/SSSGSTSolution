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
    public partial class AmendedBillRegister : Form
    {
        DataTable table = null;
        DataBaseAccess dba;
        SendSMS objSMS;
        string strOldQuery = "";

        public AmendedBillRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            if (MainPage.strUserRole != "ADMIN")
                btnDelete.Enabled = false;           
        }               

        private void GetAdvanceSearchedRecord()
        {
            try
            {
                string strQuery = "Select * from AmendmentDetails Where ID!=0 ";
                string query = CreateQuery();
                if (query != "")
                {
                    strQuery = strQuery + " " + query;
                }
                strQuery += " Order By Date desc ";
                strOldQuery = strQuery;
                table = dba.GetDataTable(strQuery);
                BindDataWithGrid();                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Advance searched Record in Show Amended Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                
                if (txtBillCode.Text != "")
                {
                    strQuery = " and OBillCode ='" + txtBillCode.Text + "' ";
                }
              
                if (chkDate.Checked)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1) ;
                    strQuery += " and (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
            
                string strBillType = GetBillType();
                if (strBillType != "")
                {
                    strQuery += " and  BillType ='" + strBillType + "'  ";
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Amended Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private string GetBillType()
        {
            string strStatus = "";
            if (rdoSales.Checked)            
                strStatus = "SALES";            
            else if (rdoPurchase.Checked)            
                strStatus = "PURCHASE";
            else if (rdoSaleReturn.Checked)
                strStatus = "SALERETURN";
            else if (rdoPurchaseReturn.Checked)
                strStatus = "PURCHASERETURN";
            return strStatus;
        }

        private void BindDataWithGrid()
        {
            try
            {                
                int rowIndex = 0,k=0;
                dgrdDetails.Rows.Clear();
                if (table.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(table.Rows.Count);
                    foreach (DataRow dr in table.Rows)
                    {                       
                        dgrdDetails.Rows[rowIndex].Cells["chkStatus"].Value = false;

                        dgrdDetails.Rows[rowIndex].Cells["billType"].Value = dr["BillType"];
                        dgrdDetails.Rows[rowIndex].Cells["date"].Value = dr["Date"]; 
                        dgrdDetails.Rows[rowIndex].Cells["oBillCode"].Value = dr["OBillCode"]+" "+ dr["OBillNo"];
                        dgrdDetails.Rows[rowIndex].Cells["oRBillCode"].Value = dr["ORBillCode"] + " " + dr["ORBillNo"]; 
                        dgrdDetails.Rows[rowIndex].Cells["oBillDate"].Value = dr["ODate"];
                        dgrdDetails.Rows[rowIndex].Cells["oRBillDate"].Value = dr["ORDate"];
                        dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = dr["CreatedBy"];
                        dgrdDetails.Rows[rowIndex].Cells["description1"].Value = dr["Columnof1"];
                        dgrdDetails.Rows[rowIndex].Cells["description2"].Value = dr["Columnof2"];
                        dgrdDetails.Rows[rowIndex].Cells["description3"].Value = dr["Columnof3"];

                        rowIndex++;
                    }
                }              
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Data with Gridview in Show Amended Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            picPleasewait.Visible = true;
            btnGo.Enabled = false;
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                GetAdvanceSearchedRecord();
            btnGo.Enabled = true;
            picPleasewait.Visible = false;
        }
             
        private void dgrdSMS_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                e.Cancel = true;
            }
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SMSReportRegister_KeyDown(object sender, KeyEventArgs e)
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

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells[0].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdSMS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                int rowIndex = dgrdDetails.CurrentRow.Index;
                if (rowIndex >= 0)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Delete these bill...? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strID = "";
                        foreach (DataGridViewRow row in dgrdDetails.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
                            {
                                if (strID != "")
                                    strID += ",";
                                strID += "'" + row.Cells["oBillCode"].Value + "'";
                            }
                        }

                        if (strID != "")
                        {
                            string strQuery = "  Delete from AmendmentDetails Where (OBillCode+' '+CAST(OBillNo as nvarchar))  in (" + strID + ") ";
                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! Selected record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                GetAdvanceSearchedRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Unable to Record ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
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
        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLBILLCODE", "SEARCH BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
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

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3 && e.RowIndex>=0)
                {
                    OpenBillDetails();
                }
            }
            catch { }
        }

        private void OpenBillDetails()
        {
            string strBillType = Convert.ToString(dgrdDetails.CurrentRow.Cells["billType"].Value), strBillCode = Convert.ToString(dgrdDetails.CurrentRow.Cells["oBillCode"].Value);
            string[] strBillNo = strBillCode.Split(' ');
            if (strBillNo.Length > 1)
            {
                if (strBillType == "SALES")
                {
                    SaleBook objSaleBook = new SSS.SaleBook(strBillNo[0], strBillNo[1]);
                    objSaleBook.FormBorderStyle = FormBorderStyle.FixedDialog;
                    objSaleBook.Show();
                }
                else if (strBillType == "PURCHASE")
                {
                    PurchaseBook objPurchaseBook = new SSS.PurchaseBook(strBillNo[0], strBillNo[1]);
                    objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedDialog;
                    objPurchaseBook.Show();
                }
                else if (strBillType == "SALERETURN")
                {
                    SaleReturn objSaleReturn = new SSS.SaleReturn(strBillNo[0], strBillNo[1]);
                    objSaleReturn.FormBorderStyle = FormBorderStyle.FixedDialog;
                    objSaleReturn.Show();
                }
                else if (strBillType == "PURCHASERETURN")
                {
                    PurchaseReturn objPurchaseReturn = new SSS.PurchaseReturn(strBillNo[0], strBillNo[1]);
                    objPurchaseReturn.FormBorderStyle = FormBorderStyle.FixedDialog;
                    objPurchaseReturn.Show();
                }
            }
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
                    saveFileDialog.FileName = "Amended_Bill_Register";
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

        private void AmendedBillRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
