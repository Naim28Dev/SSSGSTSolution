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
    public partial class AdvanceAdjustmentRegister : Form
    {
        DataBaseAccess dba;
        public AdvanceAdjustmentRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCustomerName.Text = objSearch.strSelectedData;
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

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDDateFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDDateTo_KeyPress(object sender, KeyPressEventArgs e)
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
                    SearchData objSearch = new SearchData("ADVADJUSTMENTCODE", "SEARCH ADV. ADJUSTMENT CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSerialNo.ReadOnly = txtToSerialNo.ReadOnly = !chkSerial.Checked;
            txtFromSerialNo.Text = txtToSerialNo.Text = "";
        }

        private void txtFromSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtToSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = false;

                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
                else if (chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill serial no or uncheck serial no box ! ", "Serial no Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetSearchedRecord();

            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                if (table.Rows.Count > 0)
                {
                    double dTotalAmt = 0, dOutStandingAmt = 0, dReturnedAmt = 0, dAdjustedAmt = 0;
                    dgrdDetails.Rows.Clear();
                    dgrdDetails.Rows.Add(table.Rows.Count);
                    int rowIndex = 0;
                    foreach (DataRow dr in table.Rows)
                    {
                        DataGridViewRow row = dgrdDetails.Rows[rowIndex];
                        string strBillNo = Convert.ToString(dr["BillCode"]) + " " + Convert.ToString(dr["BillNo"]);
                        row.Cells["srBillNo"].Value = strBillNo;
                        row.Cells["date"].Value = dr["BDate"];
                        row.Cells["customerName"].Value = dr["CustomerName"];
                        row.Cells["mobileNo"].Value = dr["MobileNo"];
                        row.Cells["remarks"].Value = dr["Remarks"];
                        row.Cells["cashAmt"].Value = dr["CashAmt"];
                        row.Cells["cardamt"].Value = dr["CardAmt"];
                        row.Cells["totalAmt"].Value = dr["TotalAmt"];
                        row.Cells["advadjustedNo"].Value = dr["AdjustedNumber"];
                        row.Cells["adjustedAmt"].Value = dr["AdjustedAmt"];
                        row.Cells["refundableAmt"].Value = dr["RefundableAmt"];
                        row.Cells["returnedAmt"].Value = dr["ReturnedAmt"];
                        row.Cells["adjustedSaleBill"].Value = dr["AdjustedInSaleBillNo"];
                        row.Cells["advAdjType"].Value = dr["AdvAdjType"];
                        row.Cells["createdBy"].Value = dr["CreatedBy"];
                        row.Cells["updatedBy"].Value = dr["UpdatedBy"];
                        row.Cells["SID"].Value = dr["ID"];

                        dTotalAmt += ConvertObjectToDouble(dr["TotalAmt"]);
                        dOutStandingAmt += ConvertObjectToDouble(dr["RefundableAmt"]);
                        dReturnedAmt += ConvertObjectToDouble(dr["ReturnedAmt"]);
                        dAdjustedAmt += ConvertObjectToDouble(dr["AdjustedAmt"]);

                        rowIndex++;
                    }
                    lblTotalAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                    lblOutstandingAmt.Text = dOutStandingAmt.ToString("N2", MainPage.indianCurancy);
                    lblReturnedAmt.Text = dReturnedAmt.ToString("N2", MainPage.indianCurancy);
                    lblAdjustedAmt.Text = dAdjustedAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    dgrdDetails.Rows.Clear();                    
                }
            }
            catch (Exception ex)
            { }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "")
            {
                try
                {
                    dValue = Convert.ToDouble(objValue);
                }
                catch
                {
                }
            }
            return dValue;
        }

        private void GetSearchedRecord()
        {
            try
            {
                string strQuery = "", strSubQuery = CreateQuery();
                strQuery = "select *,CONVERT(VARCHAR(10), Date, 103)BDate,CONVERT(VARCHAR(10), DelDate, 103)DDate from AdvanceAdjustment where BillNo!='' " + strSubQuery + " order by BillNo,Date";

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataWithGrid(dt);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in  Advance Adjustment Register", ex.Message };
                dba.CreateErrorReports(strReport);
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
                              

                if (chkSerial.Checked)
                    strQuery += " and (BillNo >= " + txtFromSerialNo.Text + " and BillNo <=" + txtToSerialNo.Text + ") ";

                //if (chkSaleBillNo.Checked)
                //    strQuery += " and (SR.SaleBillNo >= " + txtSaleRBillNoFrom.Text + " and SR.SaleBillNo <=" + txtSaleRBillNoTo.Text + ") ";

                if (txtBillCode.Text != "")
                    strQuery += " and BillCode Like('" + txtBillCode.Text + "') ";

                string[] strFullName;
                if (txtCustomerName.Text != "")
                {
                    strFullName = txtCustomerName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and  CustomerName = '" + strFullName[0].Trim() + "'  ";

                }                
                if (rdoAdvRec.Checked)
                    strQuery += " and AdvAdjType ='ADVANCE RECEIVE' ";
                else if (rdoReturn.Checked)
                    strQuery += " and AdvAdjType ='ADVANCE RETURN' ";
                

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in  Advance Adjustment Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdvanceAdjustmentRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            txtCustomerName.Clear();
            txtBillCode.Clear();
            chkDate.Checked = chkSerial.Checked = false;
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
                    saveFileDialog.FileName = "Advance_Adjustment_Report";
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
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);

                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        AdvanceAdjustment objPurchase = new AdvanceAdjustment(strNumber[0], strNumber[1]);
                        objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurchase.Show();
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void AdvanceAdjustmentRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
