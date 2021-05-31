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
    public partial class SupplierMapingRegister : Form
    {
        DataBaseAccess dba;
        protected internal bool _bSearchStatus = false;
        public SupplierMapingRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            btnSearch.Enabled = false;
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    GetDataFromDB();
                }
            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }
        private string CreateQuery()
        {
            string strQuery = "",strWhereQry = "";
            // AND MarketerName = '' AND SupplierName = '' AND StartDate >= '' AND EndDate <= '' AND SerialCode = '' AND SerialNo = 1
            if (txtSupplier.Text != "")
            {
                strWhereQry = " AND SupplierName = '" + txtSupplier.Text + "' ";
            }
            if (txtMarketer.Text != "")
            {
                strWhereQry = " AND MarketerName = '" + txtMarketer.Text + "' ";
            }
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                        , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);

                strWhereQry += " AND (StartDate <= '" + sDate.ToString("MM/dd/yyyy") + "' AND EndDate >= '" + eDate.ToString("MM/dd/yyyy") + "')";
                //strWhereQry += " and (('" + sDate.ToString("MM/dd/yyyy") + "' >= StartDate OR '" + sDate.ToString("MM/dd/yyyy") + "' <= EndDate) "
                //            + " AND ('" + eDate.ToString("MM/dd/yyyy") + "' > StartDate OR '" + eDate.ToString("MM/dd/yyyy") + "' < EndDate ))";

            }
            if (txtSerialCode.Text != "")
            {
                strWhereQry += " and SerialCode='" + txtSerialCode.Text + "' ";
            }

            if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
            {
                strWhereQry += " and (SerialNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
            }
            strWhereQry += " AND BranchCode = '"+MainPage.strUserBranchCode+"'";

            strQuery = " SELECT S_NO = Row_Number() Over (Order by StartDate)"
                        + " ,SerialCode + ' ' + Convert(varchar(20),SerialNo) Serial_Code_No, MarketerName Marketer_Name,SupplierName Supplier_Name,Convert(Varchar(12),StartDate ,103) Start_Date,Convert(Varchar(12),EndDate ,103)  End_Date,(Case WHEN ActiveStatus = 1 then 'Active' else 'Non-Active' end) Status,CreatedBy Created_By,UpdatedBy Updated_By "
                        + " FROM SupplierMapping WHERE 1 = 1 " + strWhereQry 
                        + "  ORDER BY StartDate ";
            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                DataTable DT = DataBaseAccess.GetDataTableRecord(strQuery);
                
                BindDataWithGrid(DT);
            }
            catch
            { }
        }

        private void SetColumnStyle()
        {
            for (int i = 0; i < dgrdDetails.Columns.Count; i++)
            {
                try
                {
                    DataGridViewCellStyle cellStyle = dgrdDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdDetails.Columns[i];

                    string strAlign = "LEFT";
                    int _width = 120;
                    _column.Width = _width;

                    _column.SortMode = DataGridViewColumnSortMode.Automatic;
                    if (_column.Name.ToUpper().Contains("S_NO"))
                    {
                        strAlign = "MIDDLE";
                        _width = 50;
                    }
                    if (_column.Name.ToUpper().Contains("STATUS") || _column.Name.ToUpper().Contains("DATE"))
                    {
                        strAlign = "MIDDLE";
                        _width = 90;
                    }
                    if (_column.Name.ToUpper().Contains("SUPP"))
                        _width = 270;
                    if (_column.Name.ToUpper().Contains("MARK"))
                        _width = 170;
                    if (_column.Name.ToUpper().Contains("AMT") || _column.Name.ToUpper().Contains("VALUE"))
                    {
                        strAlign = "RIGHT";
                        cellStyle.Format = "N2";
                    }
                    if (_column.Name.ToUpper().Contains("SERIAL"))
                    {
                        strAlign = "MIDDLE";
                        _width = 125;
                        cellStyle.ForeColor = Color.FromArgb(64, 64, 0);
                        cellStyle.Font = new Font("Arial", 9F, System.Drawing.FontStyle.Underline);
                    }
                    else
                        cellStyle.Font = new Font("Arial", 9F, System.Drawing.FontStyle.Regular);

                    if (strAlign == "LEFT")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (strAlign == "MIDDLE")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgrdDetails.Columns[i].DefaultCellStyle = cellStyle;
                    dgrdDetails.Columns[i].HeaderText = (dgrdDetails.Columns[i].HeaderText).Replace("_", " ");
                    dgrdDetails.Columns[i].HeaderCell.Style.Font = new Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
                    dgrdDetails.Columns[i].Width = _width;

                }
                catch  { }
            }
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                dgrdDetails.DataSource = null;
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        DataView dataView = new DataView(table);
                        dgrdDetails.DataSource = dataView;
                        SetColumnStyle();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in Supplier Marketer Mapping Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        //private void BindDataWithLabel(DataTable dt)
        //{
        //    double dTaxableAmt = 0, dIGSTAmt = 0, dCGSTAmt = 0, dSGSTAmt = 0, dTotalAmt = 0, dRoundOff = 0, dTCSAmt = 0, dTaxFree = 0;
        //    try
        //    {
        //        if (dt.Columns.Contains("TAXABLE_VALUE"))
        //            dTaxableAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TAXABLE_VALUE)", "ISNULL(TAXABLE_VALUE,0) <> 0"));
        //        if (dt.Columns.Contains("IGST_AMT"))
        //            dIGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(IGST_AMT)", "ISNULL(IGST_AMT,0) <> 0"));
        //        if (dt.Columns.Contains("CGST_AMT"))
        //            dCGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(CGST_AMT)", "ISNULL(CGST_AMT,0) <> 0"));
        //        if (dt.Columns.Contains("SGST_AMT"))
        //            dSGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(SGST_AMT)", "ISNULL(SGST_AMT,0) <> 0"));
        //        if (dt.Columns.Contains("TOTAL_INVOICE_VALUE"))
        //            dTotalAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TOTAL_INVOICE_VALUE)", "ISNULL(TOTAL_INVOICE_VALUE,0) <> 0"));
        //        if (dt.Columns.Contains("RoundOff_Amt"))
        //            dRoundOff = dba.ConvertObjectToDouble(dt.Compute("SUM(RoundOff_Amt)", "ISNULL(RoundOff_Amt,0) <> 0"));
        //        if (dt.Columns.Contains("TCS_Amt"))
        //            dTCSAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TCS_Amt)", "ISNULL(TCS_Amt,0) <> 0"));
        //        if (dt.Columns.Contains("Tax_Free_Amt"))
        //            dTaxFree = dba.ConvertObjectToDouble(dt.Compute("SUM(Tax_Free_Amt)", "ISNULL(Tax_Free_Amt,0) <> 0"));

        //        lblTaxableAmt.Text = (dTaxableAmt != 0) ? dTaxableAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        lbliGST.Text = (dIGSTAmt != 0) ? dIGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        lblCGST.Text = (dCGSTAmt != 0) ? dCGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        lblSGST.Text = (dSGSTAmt != 0) ? dSGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        lblRoundOff.Text = (dRoundOff != 0) ? dRoundOff.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        labelTCSAmt.Text = (dTCSAmt != 0) ? dTCSAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        labelTaxFreeAmt.Text = (dTaxFree != 0) ? dTaxFree.ToString("N2", MainPage.indianCurancy) : "0.00";
        //        lblTotalInvValue.Text = (dTotalAmt != 0) ? dTotalAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
        //    }
        //    catch { }
        //}

        private void txtSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTYNICKNAME_MAPPING", "Search Supplier Name", Keys.Space);
                    objSearch.ShowDialog();
                    txtSupplier.Text = objSearch.strSelectedData;
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
        }

        private void SupplierMapingRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtSerialCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MARKETERMAPPINGCODE", "SEARCH MAPPING CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSerialCode.Text = objSearch.strSelectedData;
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


        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                dba.ExportToExcel(dgrdDetails, "Supplier_Marketer_Register", "Marketers Mapped to Supplier");
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExport.Enabled = true;
        }

        private void SupplierMapingRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (_bSearchStatus)
            {
                SearchRecord();
            }
        }
        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
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
        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string hdr = dgrdDetails.Columns[e.ColumnIndex].HeaderText;
                    if (hdr.Contains("Serial"))
                    {
                        string str = Convert.ToString(dgrdDetails.Rows[e.RowIndex].Cells["Serial_Code_No"].Value);
                        string[] strSerialCodeNo = str.Split(' ');
                        if (strSerialCodeNo.Length > 1)
                        {
                            SupplierMapping objSupplierMapping = new SupplierMapping(strSerialCodeNo[0], strSerialCodeNo[1]);
                            objSupplierMapping.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSupplierMapping.ShowInTaskbar = true;
                            objSupplierMapping.Show();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Grid view in Show Supplier Marketer Mapping Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _rowIndex = 0;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["S_No"].Value = (_rowIndex + 1);
                    _rowIndex++;
                }
            }
            catch { }
        }

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }

        private void txtMarketer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MARKETERNAMEONBRANCH", "Search Marketer Name", Keys.Space);
                    objSearch.ShowDialog();
                    txtMarketer.Text = objSearch.strSelectedData;
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
                dgrdDetails.Cursor = Cursors.Hand;
            else
                dgrdDetails.Cursor = Cursors.Arrow;
        }

        private void txtPFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
    }
}
