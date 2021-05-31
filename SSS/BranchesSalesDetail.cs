using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel=Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class BranchesSalesDetail : Form
    {
        DataBaseAccess dba;
        public BranchesSalesDetail()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }

        private void SalesSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {              
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }

        }

        private void txtNickName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTYNICKNAME", "SEARCH NICK NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtNickName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
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
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPartyType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CASHTYPE", "SEARCH PARTY TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtMarketer_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("MARKETERNAME", "SEARCH MARKETER NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtMarketer.Text = objSearch.strSelectedData;
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALECODE", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
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
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetDataFromDB();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery , strSubQuery = "", strColumnName = "", strGroupBy ,strMainColumn, strMainGroup = "",strOrderByText=" Order by ",strGroupByText=" Group by ";

            if(txtSalesParty.Text!="")
            {
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSubQuery += " and SalePartyID='" + strFullName[0] + "' ";
            }
            if (txtPurchaseParty.Text != "")
            {
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSubQuery += " and PurchasePartyID='" + strFullName[0] + "' ";
            }

            if(txtNickName.Text!="")
                strSubQuery += " and NickName='" + txtNickName.Text + "' ";
            if (txtCategory.Text != "")
                strSubQuery += " and Category='" + txtCategory.Text + "' ";
            if (txtPartyType.Text != "")
                strSubQuery += " and Grade='" + txtPartyType.Text + "' ";
            if (txtSalesMan.Text != "")
                strSubQuery += " and Marketer='" + txtSalesMan.Text + "' ";
            if (txtSchemeName.Text != "")
                strSubQuery += " and SchemeName='" + txtSchemeName.Text + "' ";
            if (txtBillCode.Text != "")
                strSubQuery += " and SR.BillCode='" + txtBillCode.Text + "' ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strSubQuery += " and  (SR.BillDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            //(CASE When (DATEPART(mm,SR.BillDate))<4 then (DATEPART(mm,SR.BillDate)+12) else DATEPART(mm,SR.BillDate) END) _MonthNo
           
            if (chkNickName.Checked)            
                strColumnName += "SM1.NickName,";                
            if (chkSupplier.Checked)            
                strColumnName += "SM2.SupplierName,";
            if (chkSalesMan.Checked)
                strColumnName += "Marketer,";
            if (chkCategory.Checked)
                strColumnName += "Category,";
            if (chkGrade.Checked)
                strColumnName += "Grade,";

            strMainColumn=strGroupBy = strColumnName;

            if (chkSalesParty.Checked)
            {
                strColumnName += "(SalePartyID+' '+SM1.Name) as SalesParty,";
                strGroupBy += "(SalePartyID+' '+SM1.Name),";
                strMainColumn += "SalesParty,";
            }
            if(chkBranch.Checked || rdoBranchWise.Checked)
            {
                strColumnName += " (SubString(SR.BillCode,CHARINDEX('/', SR.BillCode)+1,(LEN(SR.BillCode)-CHARINDEX('/', SR.BillCode)-1))) as Branch,";
                strGroupBy += " (SubString(SR.BillCode,CHARINDEX('/', SR.BillCode)+1,(LEN(SR.BillCode)-CHARINDEX('/', SR.BillCode)-1))),";
                strMainColumn += "Branch,";
            }

            if(rdoMonthwise.Checked)
            {
                strColumnName += "DATENAME(mm,SR.BillDate) _Month,(CASE When (DATEPART(mm,SR.BillDate))<4 then (DATEPART(mm,SR.BillDate)+12) else DATEPART(mm,SR.BillDate) END) _MonthNo,";
                strGroupBy += "(CASE When (DATEPART(mm,SR.BillDate))<4 then (DATEPART(mm,SR.BillDate)+12) else DATEPART(mm,SR.BillDate) END),DATENAME(mm,SR.BillDate),";

                strMainColumn += "_Month,_MonthNo,";
            }
            
            if (strGroupBy != "")
                strGroupBy = strGroupBy.Substring(0, (strGroupBy.Length - 1));

            strMainColumn = strMainColumn.Replace("SM1.", "").Replace("SM2.", "");

            if (strMainColumn != "")
                strMainGroup = strMainColumn.Substring(0, (strMainColumn.Length - 1));

            if (strGroupBy == "")
                strOrderByText = strGroupByText = "";

            strColumnName = strColumnName.Replace("Marketer", "ISNULL(Marketer,'') as Marketer");

            strQuery = " Select "+ strMainColumn+ " SUM(Amount) Amount from (Select " + strColumnName + "SUM((((CAST(SE.Amount as Money)-TAmount)*((100+(CAST((SE.DiscountStatus+SE.Discount) as money)))/100)))) Amount from SalesRecord SR inner join SalesEntry SE ON SR.BillCode=SE.BillCode and SR.BillNo=SE.BIllNo OUTER APPLY (Select Name,SM.Other as NickName,Category,TINNumber as Grade from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SR.SalePartyID) SM1 OUTER APPLY (Select SM.Other as SupplierName from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SE.PurchasePartyID) SM2 OUTER APPLY (Select OB.Marketer,(CASE WHEN PurchaseType Like('%INCLUDE%') then TaxAmount else 0 end) TAmount,SchemeName from GoodsReceive GR OUTER APPLY (Select Marketer,SchemeName from OrderBooking OB Where GR.SalePartyID=OB.SalePartyID and RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) = GR.OrderNo)OB  Where GR.SalePartyID=SR.SalePartyID and GR.PurchasePartyID=SE.PurchasePartyID and (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo) GR Where SR.BillNo>0 " + strSubQuery + strGroupByText + strGroupBy + " UNION ALL "
                     + " Select " + strColumnName.Replace("SR.BillDate", "SR.Date").Replace("SM2.SupplierName", "SE.SupplierName") + " SUM(SE.Amount) Amount from SalesBook SR OUTER APPLY (Select 'RETAIL' as PurchasePartyID,'RETAIL' as SupplierName,(SBS.BasicAmt) Amount,Marketer,SchemeName from SalesBookSecondary SBS OUTER APPLY (Select Marketer,SchemeName from OrderBooking _OB Where _OB.SalePartyID=SR.SalePartyID and SBS.SONumber=(_OB.OrderCode+' '+CAST(_OB.OrderNo as varchar)+' '+_OB.NumberCode))OB Where SR.BillCode=SBS.BillCode and SR.BillNo=SBS.BillNo)SE Cross Apply (Select Name,SM.Other as NickName,Category,TINNumber as Grade from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SalePartyID)SM1 Where SR.BillNo!=0 " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SM2.SupplierName", "SE.SupplierName") + strGroupByText+ strGroupBy.Replace("SR.BillDate", "SR.Date").Replace("SM2.SupplierName", "SE.SupplierName") + ")_Sales " + strGroupByText+ strMainGroup + strOrderByText+ strMainGroup;


            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                DataTable _dt = dba.GetDataTable(strQuery);
                BindColumn(_dt);
                BindDataWithGrid(_dt);
            }
            catch { }
        }

        private string[] GetColumnName()
        {
            string strColumn = "";
                      
            if (chkSalesParty.Checked)           
                strColumn= "SalesParty";

            if (chkNickName.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn+= "NickName";
            }
            if (chkSupplier.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn += "SupplierName";                
            }
            if (chkSalesMan.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn += "Marketer";                
            }
            if (chkCategory.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn += "Category";               
            }
            if (chkGrade.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn += "Grade";               
            }
            if (chkBranch.Checked)
            {
                if (strColumn != "")
                    strColumn += ",";
                strColumn += "Branch";
                
            }

            string[] str = strColumn.Split(',');
            return str;
        }

        private string GetFilterQuery(DataRow row)
        {
            string strQuery = "";
            if (chkSalesParty.Checked)
                strQuery = "SalesParty='"+row["SalesParty"] +"' ";

            if (chkNickName.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "NickName='" + row["NickName"] + "' ";
            }
            if (chkSupplier.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "SupplierName='" + row["SupplierName"] + "' ";
            }
            if (chkSalesMan.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "Marketer='" + row["Marketer"] + "' ";
            }
            if (chkCategory.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "Category='" + row["Category"] + "' ";
            }
            if (chkGrade.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "Grade='" + row["Grade"] + "' ";
            }
            if (chkBranch.Checked)
            {
                if (strQuery != "")
                    strQuery += " and ";
                strQuery += "Branch='" + row["Branch"] + "' ";
            }
            return strQuery;
        }

        private void BindDataWithGrid(DataTable _dt)
        {
            double dNetAmt = 0,dAmt=0;
            try
            {
                if (rdoNet.Checked)
                {
                    if (_dt.Rows.Count > 0)
                        dgrdDetails.Rows.Add(_dt.Rows.Count);
                    int _rowIndex = 0;

                    foreach (DataRow row in _dt.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                        if (chkSalesParty.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["salesParty"].Value = row["SalesParty"];
                        if (chkNickName.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["nickName"].Value = row["NickName"];
                        if (chkSupplier.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                        if (chkSalesMan.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["marketer"].Value = row["Marketer"];
                        if (chkCategory.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["category"].Value = row["Category"];
                        if (chkGrade.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["Grade"].Value = row["Grade"];
                        if (chkBranch.Checked)
                            dgrdDetails.Rows[_rowIndex].Cells["branchName"].Value = row["Branch"];

                        dNetAmt += dAmt = dba.ConvertObjectToDouble(row["amount"]);
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value = Math.Round(dAmt, 2);

                        _rowIndex++;
                    }
                }
                else
                {
                    int _rowIndex = 0;
                    bool _bStatus = false;
                    string[] strColumn = GetColumnName();
                    if (strColumn.Length > 0)
                    {
                        DataTable dt = null;
                        if (strColumn.Length > 0)
                        {
                            if (strColumn.Length == 1 && strColumn[0] == "")
                                dt = _dt;
                            else
                            {
                                dt = _dt.DefaultView.ToTable(true, strColumn);
                                _bStatus = true;
                            }
                        }
                        else
                        {
                            dt = _dt;
                            _bStatus = true;
                        }
                        if (!_bStatus)
                            dgrdDetails.Rows.Add();

                        string strFilterSearch = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            if (_bStatus)
                                dgrdDetails.Rows.Add();
                            strFilterSearch = GetFilterQuery(row);
                            dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                            if (chkSalesParty.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["salesParty"].Value = row["SalesParty"];
                            if (chkNickName.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["nickName"].Value = row["NickName"];
                            if (chkSupplier.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                            if (chkSalesMan.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["marketer"].Value = row["Marketer"];
                            if (chkCategory.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["category"].Value = row["Category"];
                            if (chkGrade.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["Grade"].Value = row["Grade"];
                            if (chkBranch.Checked)
                                dgrdDetails.Rows[_rowIndex].Cells["branchName"].Value = row["Branch"];

                            string strColumnName = "_Month", strColumnSearch = "";
                            if (rdoBranchWise.Checked)
                                strColumnName = "Branch";

                            if (_dt.Columns.Contains(strColumnName))
                            {
                                DataTable dtColumn = _dt.DefaultView.ToTable(true, strColumnName);

                                string _strColumnName = "";
                                foreach (DataRow _row in dtColumn.Rows)
                                {
                                    _strColumnName = Convert.ToString(_row[strColumnName]);
                                    strColumnSearch = strColumnName + "='" + _strColumnName + "' ";

                                    if (strFilterSearch != "")
                                        strColumnSearch = " and " + strColumnSearch;

                                    object objValue = _dt.Compute("Sum(Amount)", strFilterSearch + strColumnSearch);

                                    dNetAmt += dAmt = dba.ConvertObjectToDouble(objValue);

                                    dgrdDetails.Rows[_rowIndex].Cells[_strColumnName].Value = Math.Round(dAmt, 2);
                                }
                            }

                            _rowIndex++;
                            if (!_bStatus)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void BindColumn(DataTable _dt)
        {

            dgrdDetails.Columns.Clear();

            CreateGridviewColumn("sno", "S.No", "RIGHT", 50);
            if(chkSalesParty.Checked)
                CreateGridviewColumn("salesParty", "Sundry Debtors", "LEFT", 180);
            if (chkNickName.Checked)
                CreateGridviewColumn("nickName", "Nick Name", "LEFT", 150);
            if (chkSupplier.Checked)
                CreateGridviewColumn("supplierName", "Sundry Creditor", "LEFT", 150);
            if (chkSalesMan.Checked)
                CreateGridviewColumn("marketer", "Sales Man", "LEFT", 100);
            if (chkCategory.Checked)
                CreateGridviewColumn("category", "Category", "LEFT", 100);
            if (chkGrade.Checked)
                CreateGridviewColumn("Grade", "grade", "LEFT", 100);
            if (chkBranch.Checked)
                CreateGridviewColumn("branchName", "Branch", "LEFT", 100);

            if (rdoNet.Checked)
            {
                CreateGridviewColumn("Amount", "Amount", "RIGHT", 120);
            }
            else
            {
                string strColName = "_Month,_MonthNo",strCName= "_MonthNo";
                if (rdoBranchWise.Checked)                
                    strCName = strColName = "Branch";                   
                
                string[] strColumnName = strColName.Split(',');
                if (strColumnName.Length>0)
                {
                    DataTable dt = _dt.DefaultView.ToTable(true, strColumnName);
                    DataView _dv = dt.DefaultView;
                    _dv.Sort = strCName;
                    dt = _dv.ToTable();

                    string _strColumnName = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        _strColumnName = Convert.ToString(row[0]);
                        CreateGridviewColumn(_strColumnName, _strColumnName, "RIGHT", 110);
                    }
                }              
            }
        }

        private void CreateGridviewColumn(string strColName, string strColHeader,string strAlign, int _width)
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
                    if(_width!=50)
                    _column.DefaultCellStyle.Format = "N2";
                }
                _column.CellTemplate = dataGridViewCell;
                dgrdDetails.Columns.Add(_column);
            }
            catch { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["sno"].Value = _index;
                    _index++;
                }
            }
            catch { }
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
                    saveFileDialog.FileName = "Branches_Sale_Detail";
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

        private void txtSchemeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSchemeName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void BranchesSalesDetail_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (Control ctrl in grpChkBox1.Controls)
                {
                    if (ctrl is CheckBox)
                    {
                        ((CheckBox)ctrl).Checked = chkAll.Checked;
                    }
                }

            }
            catch { }
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
    }
}
