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
    public partial class BankGuaranteeRegister : Form
    {
        DataBaseAccess dba;
        public BankGuaranteeRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtFromDate.Text = txtFromValidUpto.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = txtToValidUpto.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        public BankGuaranteeRegister(string strCustomerName)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();
                txtFromDate.Text = txtFromValidUpto.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = txtToValidUpto.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

                txtCustomerName.Text = strCustomerName;
                GetAllData();
            }
            catch { }
        }


        private void BankGuaranteeRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlColor.Visible)
                {
                    pnlColor.Visible = false;
                }
                else
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
          
        }

        private void txtBankAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCustomerName.Text = objSearch.strSelectedData;
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

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkDepositeDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromValidUpto.Enabled = txtToValidUpto.Enabled = chkValidUptoDate.Checked;
            txtFromValidUpto.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToValidUpto.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
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

        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                GetAllData();
            }
            catch
            {
            }
            btnGO.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (chkDate.Checked)
            {
                DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDate.Text), toDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strQuery += " and Date>='" + fromDate + "' and Date<'" + toDate.AddDays(1) + "' ";
            }
            if (chkValidUptoDate.Checked)
            {
                DateTime fromDate = dba.ConvertDateInExactFormat(txtFromValidUpto.Text), toDate = dba.ConvertDateInExactFormat(txtToValidUpto.Text);
                strQuery += " and ValidUpto >='" + fromDate + "' and ValidUpto <'" + toDate.AddDays(1) + "' ";
            }

            string[] strFullName;
            if (txtCustomerName.Text != "")
            {
                strFullName = txtCustomerName.Text.Split(' ');
                if (strFullName.Length > 1)
                    strQuery += " and (CustomerName='" + strFullName[0].Trim() + "') ";
            }
            if (txtBankName.Text != "")
            {
                if (txtBankName.Text.Length > 1)
                    strQuery += " and (BankName='" + txtBankName.Text + "') ";
            }

            if (txtBGNo.Text != "")
                strQuery += " and [BankGuaranteeNo] Like('%" + txtBGNo.Text + "%')  ";

            if (txtBillCode.Text != "")
                strQuery += " and [BillCode] Like('%" + txtBillCode.Text + "%')  ";

            return strQuery;
        }

        private void GetAllData()
        {
            //if (txtBGNo.Text.Length > 3)
            //{
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (chkValidUptoDate.Checked && (txtFromValidUpto.Text == "" || txtToValidUpto.Text == ""))
                MessageBox.Show("Sorry ! Please enter Valid Upto date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                string strQuery = "", strSubQuery = CreateQuery();
                strQuery = " Select [ID],[BillCode],[BillNo],Convert(varchar,[Date],103) BDate,dbo.GetFullName(CustomerName) as CustomerName,[BankGuaranteeNo],[Amount],[BankName] ,Convert(varchar, [ValidUpto],103) ValidUpto,[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]  FROM BankGuarantee WHERE BillCode != '' " + strSubQuery + " Order by Date,BillNo desc ";

                DataTable dt = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dt);
            }
            //else
            //{
            //    MessageBox.Show("Sorry ! Please  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }

        private void BindRecordWithGrid(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            chkAll.Checked = false;
            lblTotalAmt.Text = "0";
            double dAmt = 0, dTotalAmt = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                string strVUDate = "";
                DateTime VUDate;
                foreach (DataRow row in dt.Rows)
                {
                    dTotalAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    VUDate = dba.ConvertDateInExactFormat(row["ValidUpto"].ToString());
                    strVUDate = Convert.ToString(row["ValidUpto"]);

                    dgrdDetails.Rows[rowIndex].Cells["chkTick"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["billno"].Value = row["BillCode"] + " " + row["BillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                    dgrdDetails.Rows[rowIndex].Cells["customername"].Value = row["CustomerName"];
                    dgrdDetails.Rows[rowIndex].Cells["bankName"].Value = row["BankName"];
                    if (Convert.ToString(row["ValidUpto"]) != "" && !strVUDate.Contains("1900"))
                        dgrdDetails.Rows[rowIndex].Cells["validupToDate"].Value = strVUDate;
                    dgrdDetails.Rows[rowIndex].Cells["bgno"].Value = row["BankGuaranteeNo"];
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dAmt;

                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];

                    if (VUDate < MainPage.currentDate.AddDays(1))
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                    else if (VUDate < MainPage.currentDate.AddDays(30) && VUDate > MainPage.currentDate)
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                 
                    rowIndex++;
                }
            }
            lblTotalAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }
       
        private void BankGuaranteeRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);

            //if (!MainPage.mymainObject.bCashView)
            //{
            //    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    this.Close();
            //}
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
                        BankGuarantee objBG = new BankGuarantee(strNumber[0], strNumber[1]);
                        objBG.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objBG.ShowInTaskbar = true;
                        objBG.Show();
                    }
                }
            }
            catch { }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                int _count = 0;
                for (int k = 0; k < dgrdDetails.RowCount; k++)
                {
                    _count += Convert.ToBoolean(dgrdDetails.Rows[k].Cells[0].Value) ? 1 : 0;
                }
                if (_count > 0 )
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

                    int _skipColumn = 0, _skipRow = 0;
                    string strHeader = "";
                    for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            //j++;
                            continue;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                    {
                        if (Convert.ToBoolean(dgrdDetails.Rows[k].Cells[0].Value) == false)
                        {
                            _skipRow++;
                            continue;
                        }
                        for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                        {
                            if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                            {
                                _skipColumn++;
                                //l++;
                                continue;
                            }
                            if (l < dgrdDetails.Columns.Count)
                            {
                                int RowId = k - _skipRow + 2;
                                ExcelApp.Cells[RowId, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                            }
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Bank_Guarantee_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                       
                    }
                    else
                        MessageBox.Show("Export Cancled...");
                    ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
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

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BGBANKNAME", "SEARCH BANK NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBankName.Text = objSearch.strSelectedData;
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
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void chkAll_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                for (int k = 0; k < dgrdDetails.RowCount; k++)
                {
                    dgrdDetails.Rows[k].Cells[0].Value = chkAll.Checked;
                }
            }
            catch { }
        }

        private void txtBGNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnCustomerName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", Keys.Space);
                objSearch.ShowDialog();
                txtCustomerName.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnBankName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("BGBANKNAME", "SEARCH BANK NAME", Keys.Space);
                objSearch.ShowDialog();
                txtBankName.Text = objSearch.strSelectedData;
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
                    SearchData objSearch = new SearchData("BGBILLCODE", "SEARCH BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;

            }
            catch
            {
            }
        }

        private void txtBillCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtFromValidUpto_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkValidUptoDate.Checked, false, false);
        }

        private void lnkColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
             pnlColor.Visible = !pnlColor.Visible;
        }
    }

}
