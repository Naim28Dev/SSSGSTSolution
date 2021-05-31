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
    public partial class Agent_Report : Form
    {
        DataBaseAccess dba;
        public Agent_Report()
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

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESANDCASHPARTY", "SEARCH Sundry Debtors", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
                    
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPetiAgent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PETIAGENT", "SEARCH SALES MAN", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPetiAgent.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPacker_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PACKERNAME", "SEARCH PACKER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPacker.Text = objSearch.strSelectedData;
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
            try
            {
                btnGo.Enabled = false;
                if (txtSalesParty.Text != "" || txtPacker.Text != "" || txtPetiAgent.Text != "" || MainPage.mymainObject.bShowAllRecord || MainPage.strSoftwareType != "AGENT")
                {
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
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch { }
            btnGo.Enabled = true;
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

                if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                    strQuery += " and (BillNo >= " + txtPFromSNo.Text + " and BillNo <=" + txtPToSNo.Text + ") ";

                string[] strFullName;
                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and SalePartyID = '" + strFullName[0].Trim() + "'  ";
                }
                if (txtPetiAgent.Text != "")
                {
                    strFullName = txtPetiAgent.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and Description_1 = '" + strFullName[0].Trim() + "'  ";
                }

                if (txtPacker.Text != "")
                    strQuery += " and PackerName='" + txtPacker.Text + "' ";
                if (txtBillCode.Text != "")
                    strQuery += " and BillCode='" + txtBillCode.Text + "' ";

                string strColumName = "", strGroupBy = "";
                if (chkSundryDebtors.Checked)
                    strColumName += "SalesParty,";
                if (chkBillCode.Checked)
                    strColumName += "BillCode,";
                if (chkBillNo.Checked)
                    strColumName += "BillNo,";
                if (chkSalesMan.Checked)
                    strColumName += "SalesMan,";
                if (chkPackerName.Checked)
                    strColumName += "PackerName,";
                if (strColumName != "")
                {
                    strGroupBy = strColumName.Substring(0, strColumName.Length - 1);
                    strGroupBy = "GROUP BY " + strGroupBy + " Order By " + strGroupBy;
                }

                strQuery = "Select " + strColumName + " SUM(TotalQty)Qty,SUM(NetAmt)Amount from (Select BillCode,(BillCode+' '+CAST(BillNo as varchar)) as BillNo,(SalePartyID+' '+Name)SalesParty,PackerName,SalesMan,TotalQty,NetAmt from SalesBook SB OUTER APPLY (Select Name from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=SalePartyID)SM OUTER APPLY (Select (Description_1+''+Name)SalesMan from SupplierMaster SM1 Where SM1.AreaCode+SM1.AccountNo=SB.Description_1)SM1 WHere BillNo!=0 " + strQuery + ")_Sales " + strGroupBy;
            }
            catch (Exception ex){ MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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
            if (chkSundryDebtors.Checked)
                CreateGridviewColumn("SalesParty", "Sundry Debtors", "LEFT", 240);
            if (chkBillCode.Checked)
                CreateGridviewColumn("BillCode", "Bill Code", "LEFT", 100);
            if (chkBillNo.Checked)
                CreateGridviewColumn("BillNo", "Bill No", "LEFT", 150);
            if (chkPackerName.Checked)
                CreateGridviewColumn("PackerName", "Packer  Name", "LEFT", 170);
            if (chkSalesMan.Checked)
                CreateGridviewColumn("SalesMan", "Sales Man", "LEFT", 170);
           
            CreateGridviewColumn("Qty", "Net  Qty", "RIGHT", 100);
            CreateGridviewColumn("Amount", "Net Amt", "RIGHT", 120);

        }

        private void BindDataTable(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            if (table.Rows.Count > 0)
                dgrdDetails.Rows.Add(table.Rows.Count);

            int _rowIndex = 0;
            double dQty = 0, dTQty = 0, dTAmt = 0, dAmt = 0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                    if (chkSundryDebtors.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["SalesParty"].Value = row["SalesParty"];
                    if (chkBillCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BillCode"].Value = row["BillCode"];
                    if (chkBillNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BillNo"].Value = row["BillNo"];
                    if (chkSalesMan.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["SalesMan"].Value = row["SalesMan"];
                    if (chkPackerName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PackerName"].Value = row["PackerName"];

                    dTQty += dQty = dba.ConvertObjectToDouble(row["Qty"]);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                    dgrdDetails.Rows[_rowIndex].Cells["Qty"].Value = dQty;
                    dgrdDetails.Rows[_rowIndex].Cells["Amount"].Value = dAmt;
                    _rowIndex++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            lblTotalQty.Text = dTQty.ToString("N2", MainPage.indianCurancy);
            lblTotalAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
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
                    saveFileDialog.FileName = "Agent_Report";
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

        private void Agent_Report_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
