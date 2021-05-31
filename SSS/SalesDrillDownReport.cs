using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class SalesDrillDownReport : Form
    {
        DataBaseAccess dba;

        static string strDepartment = "", strCategory = "", strBrand = "", strItem = "", strCat1 = "", strCat2 = "", strCat3 = "", strCat4 = "", strCat5 = "", strBillCodeNo = "";
        string State = "DEPARTMENT";
        string[] StateList = { "DEPARTMENT", "CATEGORY", "BRAND", "ITEM", MainPage.StrCategory1.ToUpper(), MainPage.StrCategory2.ToUpper(), MainPage.StrCategory3.ToUpper(), MainPage.StrCategory4.ToUpper(), MainPage.StrCategory5.ToUpper()
               , "INVOICE NO" };
        bool IsInvoiceShown = false,isLastCategory=false;
        TextInfo textInfo = new CultureInfo("hi-IN", false).TextInfo;

        public SalesDrillDownReport()
        {
            InitializeComponent();
            dba = new DataBaseAccess();

            SetReportState(0);
            GetDataFromDataBase(0);
        }

        public SalesDrillDownReport(int _Index)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetReportState(_Index);
            GetDataFromDataBase(_Index);
        }

        private void SalesDrillDownReport_Load(object sender, EventArgs e)
        {

        }

        private void SetReportState(int _index)
        {
            State = StateList[_index];
        }

        private void SetLinkName(int index)
        {
            lblCategory1.Text = textInfo.ToTitleCase(MainPage.StrCategory1) + " >";
            lblCategory2.Text = textInfo.ToTitleCase(MainPage.StrCategory2) + " >";
            lblCategory3.Text = textInfo.ToTitleCase(MainPage.StrCategory3) + " >";
            lblCategory4.Text = textInfo.ToTitleCase(MainPage.StrCategory4) + " >";
            lblCategory5.Text = textInfo.ToTitleCase(MainPage.StrCategory5) + " >";
            switch (index)
            {
                case 0:
                    lblDepartment.Visible = false;
                    lblCategory.Visible = false;
                    lblBrand.Visible = false;
                    lblItem.Visible = false;
                    lblCategory1.Visible = false;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 1:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Visible = false;
                    lblBrand.Visible = false;
                    lblItem.Visible = false;
                    lblCategory1.Visible = false;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 2:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Visible = false;
                    lblItem.Visible = false;
                    lblCategory1.Visible = false;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 3:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Visible = false;
                    lblCategory1.Visible = false;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 4:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Visible = false;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 5:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Text = strCat1 + " >";
                    lblCategory1.Visible = true;
                    lblCategory2.Visible = false;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 6:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Text = strCat1 + " >";
                    lblCategory1.Visible = true;
                    lblCategory2.Text = strCat2 + " >";
                    lblCategory2.Visible = true;
                    lblCategory3.Visible = false;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 7:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Text = strCat1 + " >";
                    lblCategory1.Visible = true;
                    lblCategory2.Text = strCat2 + " >";
                    lblCategory2.Visible = true;
                    lblCategory3.Text = strCat3 + " >";
                    lblCategory3.Visible = true;
                    lblCategory4.Visible = false;
                    lblCategory5.Visible = false;
                    break;
                case 8:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Text = strCat1 + " >";
                    lblCategory1.Visible = true;
                    lblCategory2.Text = strCat2 + " >";
                    lblCategory2.Visible = true;
                    lblCategory3.Text = strCat3 + " >";
                    lblCategory3.Visible = true;
                    lblCategory4.Text = strCat4 + " >";
                    lblCategory4.Visible = true;
                    lblCategory5.Visible = false;
                    break;
                case 9:
                    lblDepartment.Text = strDepartment + " >";
                    lblDepartment.Visible = true;
                    lblCategory.Text = strCategory + " >";
                    lblCategory.Visible = true;
                    lblBrand.Text = strBrand + " >";
                    lblBrand.Visible = true;
                    lblItem.Text = strItem + " >";
                    lblItem.Visible = true;
                    lblCategory1.Text = strCat1 + " >";
                    lblCategory1.Visible = true;
                    lblCategory2.Text = strCat2 + " >";
                    lblCategory2.Visible = true;
                    lblCategory3.Text = strCat3 + " >";
                    lblCategory3.Visible = true;
                    lblCategory4.Text = strCat4 + " >";
                    lblCategory4.Visible = true;
                    lblCategory5.Text = strCat5 + " >";
                    lblCategory5.Visible = true;
                    break;
            }
            lblCategory.Left = lblDepartment.Right + 3;
            lblBrand.Left = lblCategory.Right + 3;
            lblItem.Left = lblBrand.Right + 3;
            lblCategory1.Left = lblItem.Right + 3;
            lblCategory2.Left = lblCategory1.Right + 3;
            lblCategory3.Left = lblCategory2.Right + 3;
            lblCategory4.Left = lblCategory3.Right + 3;
            lblCategory5.Left = lblCategory4.Right + 3;
        }

        private void OpenNewDrill(int _index)
        {
            string CState = StateList[_index];
            try
            {
                SetReportState(_index);
                GetDataFromDataBase(_index);
            }
            catch { }
            //string CState = StateList[_index];
            //if (State != CState)
            //{
            //    try
            //    {
            //        SalesDrillDownReport objDrill = new SalesDrillDownReport(_index);
            //        objDrill.MdiParent = MainPage.mymainObject;
            //        objDrill.Show();
            //    }
            //    catch { }
            //}
        }

        private void clearFilters(int index)
        {
            if (index >= 0)
            {
                strCat5 = index < 8 ? "" : strCat5;
                strCat4 = index < 7 ? "" : strCat4;
                strCat3 = index < 6 ? "" : strCat3;
                strCat2 = index < 5 ? "" : strCat2;
                strCat1 = index < 4 ? "" : strCat1;
                strItem = index < 3 ? "" : strItem;
                strBrand = index < 2 ? "" : strBrand;
                strCategory = index < 1 ? "" : strCategory;
                strDepartment = index < 0 ? "" : strDepartment;
            }
            SetLinkName(index);
        }

        private string CreateQuery(int _index)
        {
            clearFilters(_index);
            string strQuery = "", strWhereQuery = "", strColumnQuery = "", strColumnFinal = "", strGroupByQuery = "";
            bool _isCat = false;

            if (_index >= 0)
            {
                strColumnFinal = " Department";
                strColumnQuery = " Isnull(Depo.Department,'') as Department";
                strGroupByQuery = " Group By Depo.Department";
            }

            if (_index >= 1)
            {
                strColumnFinal += ", Category";
                strColumnQuery += ", Isnull(Category,'') as Category";
                strWhereQuery = " Where Isnull(Department,'') in ('" + strDepartment + "') ";
                strGroupByQuery += ", Category";
            }

            if (_index >= 2)
            {
                strColumnFinal += ", Brand";
                strColumnQuery += ", Isnull(BrandName,'') as Brand";
                strWhereQuery += " and Isnull(Category,'') in ('" + strCategory + "') ";
                strGroupByQuery += ", BrandName";
            }

            if (_index >= 3)
            {
                strColumnFinal += ", Item";
                strColumnQuery += ", Isnull(ItemName,'') as Item";
                strWhereQuery += " and Isnull(BrandName,'') in ('" + strBrand + "') ";
                strGroupByQuery += ", ItemName ";
            }

            if (_index >= 4)
            {
                if (MainPage.StrCategory1 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory1;
                    strColumnQuery += ", Isnull(Variant1,'') as " + MainPage.StrCategory1;
                    strGroupByQuery += ", Variant1 ";
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 5)
            {
                if (MainPage.StrCategory2 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory2;
                    strColumnQuery += ", Isnull(Variant2,'') as " + MainPage.StrCategory2;
                    if (MainPage.StrCategory1 != "")
                        strWhereQuery += " and Isnull(Variant1,'') in ('" + strCat1 + "') ";
                    strGroupByQuery += ", Variant2 ";
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 6)
            {
                if (MainPage.StrCategory3 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory3;
                    strColumnQuery += ", Isnull(Variant3,'') as " + MainPage.StrCategory3;
                    if (MainPage.StrCategory2 != "")
                        strWhereQuery += " and Isnull(Variant2,'') in ('" + strCat2 + "') ";
                    strGroupByQuery += ", Variant3 ";
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 7)
            {
                if (MainPage.StrCategory4 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory4;
                    strColumnQuery += ", Isnull(Variant4,'') as  " + MainPage.StrCategory4;
                    if (MainPage.StrCategory3 != "")
                        strWhereQuery += " and Isnull(Variant3,'') in ('" + strCat3 + "') ";
                    strGroupByQuery += ", Variant4 ";
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 8)
            {
                if (MainPage.StrCategory5 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory5;
                    strColumnQuery += ", Isnull(Variant5,'') as " + MainPage.StrCategory5;
                    if (MainPage.StrCategory4 != "")
                        strWhereQuery += " and Isnull(Variant4,'') in ('" + strCat4 + "') ";
                    strGroupByQuery += ", Variant5 ";
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 9)
            {
                strColumnFinal += ", InvoiceNo";
                strColumnQuery += ", Isnull(SBS.BillCode+ ' ' + Convert(varchar(20),SBS.BillNo),'') as InvoiceNo";
                strGroupByQuery += ", (SBS.BillCode+ ' ' + Convert(varchar(20),SBS.BillNo))";
                if (MainPage.StrCategory5 != "")
                    strWhereQuery += " and Isnull(Variant5,'') in ('" + strCat5 + "') ";
                _isCat = true;
            }

            if (_isCat)
            {
                strWhereQuery += " and Isnull(ItemName,'') in ('" + strItem + "') ";
            }

            strQuery =
                        "SELECT " + strColumnFinal + ", Sum(Qty) Qty,Sum(Amount) Amount from ("
                        + "Select " + strColumnQuery
                        + ", Sum(Qty) Qty,Sum(Amount) Amount FROM SalesBookSecondary as SBS left Join SalesBook as SB on SBS.BillCode = SB.BillCode and Sbs.BillNo = SB.BillNo OUTER APPLY (SELECT MakeName as Department , Other as Category FROM Items _im  where _im.ItemName = SBS.ItemName ) Depo "
                        + strWhereQuery
                        + strGroupByQuery
                        + " ) as SalesRecords Group By "
                        + strColumnFinal + " Order By "
                        + strColumnFinal;
            return strQuery;
        }

        private void GetDataFromDataBase(int _Index)
        {
            btnGO.Enabled = false;
            try
            {
                State = StateList[_Index];
                string strQuery = CreateQuery(_Index);

                DataTable table = new DataTable();
                table = dba.GetDataTable(strQuery);

                BindColumn(table);
                BindDataTable(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Error Occured that is - " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            btnGO.Enabled = true;
        }

        private void setFilters()
        {
            strCat5 = strCat4 = strCat3 = strCat2 = strCat1 = strItem = strBrand = strCategory = strDepartment = "";

            int ColCount = dgrdDetails.Columns.Count;

            strDepartment = dgrdDetails.CurrentRow.Cells["Department"].Value.ToString();
            if (ColCount >= 5)
                strCategory = dgrdDetails.CurrentRow.Cells["Category"].Value.ToString();
            if (ColCount >= 6)
                strBrand = dgrdDetails.CurrentRow.Cells["Brand"].Value.ToString();
            if (ColCount >= 7)
                strItem = dgrdDetails.CurrentRow.Cells["Item"].Value.ToString();
            if (ColCount >= 8)
                strCat1 = MainPage.StrCategory1 == "" ? "" : dgrdDetails.CurrentRow.Cells[MainPage.StrCategory1].Value.ToString();
            if (ColCount >= 9)
                strCat2 = MainPage.StrCategory2 == "" ? "" : dgrdDetails.CurrentRow.Cells[MainPage.StrCategory2].Value.ToString();
            if (ColCount >= 10)
                strCat3 = MainPage.StrCategory3 == "" ? "" : dgrdDetails.CurrentRow.Cells[MainPage.StrCategory3].Value.ToString();
            if (ColCount >= 11)
                strCat4 = MainPage.StrCategory4 == "" ? "" : dgrdDetails.CurrentRow.Cells[MainPage.StrCategory4].Value.ToString();
            if (ColCount >= 12)
                strCat5 = MainPage.StrCategory5 == "" ? "" : dgrdDetails.CurrentRow.Cells[MainPage.StrCategory5].Value.ToString();
            if (ColCount >= 13)
                strBillCodeNo = dgrdDetails.CurrentRow.Cells["InvoiceNo"].Value.ToString();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    string HeaderText = dgrdDetails.Columns[e.ColumnIndex].HeaderText;

                    if (HeaderText.ToUpper().Contains("INVOICE"))
                    {
                        strBillCodeNo = dgrdDetails.CurrentCell.Value.ToString();
                        string[] strBillNo = strBillCodeNo.Split(' ');
                        ShowSaleBook(strBillNo[0], strBillNo[1]);
                    }
                }
            }
            catch
            {

            }
        }
        private void ShowSaleBook(string strCode, string strBillNo)
        {
            dba.ShowTransactionBook("SALES", strCode, strBillNo);
        }

        private void dgrdDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    int StateIndex = Array.IndexOf(StateList, State);
                    int count = this.StateList.Count();
                    string HeaderText = dgrdDetails.Columns[e.ColumnIndex].HeaderText;

                    setFilters();
                    if (StateIndex < count)
                    {
                        if (IsInvoiceShown)
                        {
                            strBillCodeNo = dgrdDetails.Rows[e.RowIndex].Cells["InvoiceNo"].Value.ToString();
                            string[] strBillNo = strBillCodeNo.Split(' ');
                            ShowSaleBook(strBillNo[0], strBillNo[1]);
                        }
                        else
                        {
                            OpenNewDrill(StateIndex + 1);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void BindColumn(DataTable _dt)
        {
            IsInvoiceShown = false;
            dgrdDetails.Columns.Clear();
            CreateGridviewColumn("SNo", "S.No", "RIGHT", 50);

            foreach (DataColumn Dtc in _dt.Columns)
            {
                string align = "LEFT", columnName = Dtc.ColumnName.ToString();
                int width = 150;
                if (columnName.ToUpper().Contains("ITEM"))
                {
                    width = 200;
                }
                if (columnName.ToUpper().Contains("QTY"))
                {
                    align = "RIGHT"; width = 70;
                }
                if (columnName.ToUpper().Contains("AMOUNT"))
                {
                    align = "RIGHT"; width = 100;
                }
                if (columnName.ToUpper().Contains("SIZE"))
                {
                    width = 70;
                }
                if (columnName.ToString().ToUpper().Contains("INVOICE"))
                {
                    CreateGridviewLinkColumn(columnName, columnName, align, width);
                    IsInvoiceShown = true;
                }
                else
                {
                    CreateGridviewColumn(columnName, columnName, align, width);
                }
            }
        }

        private void CreateGridviewColumn(string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
                _column.Name = strColName;
                _column.HeaderText = textInfo.ToTitleCase(strColHeader);
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
            catch { }
        }

        private void CreateGridviewLinkColumn(string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewLinkCell dataGridViewCell = new DataGridViewLinkCell();

                _column.Name = strColName;
                _column.HeaderText = textInfo.ToTitleCase(strColHeader);
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
                dataGridViewCell.LinkColor = Color.FromArgb(64, 64, 0);
                dataGridViewCell.LinkBehavior = LinkBehavior.HoverUnderline;
                dataGridViewCell.ActiveLinkColor = Color.Red;

                _column.CellTemplate = dataGridViewCell;
                dgrdDetails.Columns.Add(_column);
            }
            catch { }
        }

        private void BindDataTable(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            if (table.Rows.Count > 0)
                dgrdDetails.Rows.Add(table.Rows.Count);

            int _rowIndex = 0;
            double dAmount = 0, dQty = 0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["SNo"].Value = (_rowIndex + 1);

                    foreach (DataColumn column in table.Columns)
                    {
                        string columnName = column.ColumnName.ToString();
                        dgrdDetails.Rows[_rowIndex].Cells[columnName].Value = row[columnName];
                    }
                    _rowIndex++;
                    dAmount += dAmount = dba.ConvertObjectToDouble(row["Amount"]);
                    dQty += dQty = dba.ConvertObjectToDouble(row["Qty"]);
                }
                lblTotalAmt.Text = dAmount.ToString("N2", MainPage.indianCurancy);
                lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[dgrdDetails.ColumnCount - 1];
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblDepartment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           // clearFilters(0);
            OpenNewDrill(1);
        }

        private void lblCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(2);
        }

        private void lblBrand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(3);
        }

        private void lblItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(4);
        }

        private void lblCategory1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(5);
        }

        private void lblCategory2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(6);
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            GetDataFromDataBase(0);
        }

        private void lblCategory3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(7);
        }

        private void lblCategory4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(8);
        }

        private void lblCategory5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(9);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
                {

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

                            ExcelApp.Cells[2, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[2, j - _skipColumn].Font.Bold = true;

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
                                    ExcelApp.Cells[k + 3, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                            }
                            _skipColumn = 0;
                        }
                        Microsoft.Office.Interop.Excel.Range range = xlWorksheet.UsedRange;

                        string address = range.get_Address();
                        string[] cells = address.Split(new char[] { ':' });
                        string endCell = cells[1].Replace("$", "");

                        ExcelApp.Cells[1, 1] = "Sales Drill Down Report";
                        ExcelApp.Cells[1, 1].Font.Bold = true;

                        xlWorksheet.get_Range("A1:" + endCell.Substring(0, 1) + "1").Merge();
                        //ExcelApp.Cells[1, 1].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                        ExcelApp.Columns.AutoFit();

                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Sales_Report";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        }

                        xlWorkbook.Close(0);
                        ExcelApp.Quit();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);

                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int StateIndex = Array.IndexOf(StateList, State);
                    int colIndex = dgrdDetails.CurrentCell.ColumnIndex;

                    int count = this.StateList.Count();

                    string HeaderText = dgrdDetails.Columns[colIndex].HeaderText;
                    setFilters();

                    if (StateIndex < count)
                    {
                        if (IsInvoiceShown)
                        {
                            int rowIndex = dgrdDetails.SelectedCells[0].OwningRow.Index;
                            strBillCodeNo = strBillCodeNo = dgrdDetails.Rows[rowIndex].Cells["InvoiceNo"].Value.ToString();
                            string[] strBillNo = strBillCodeNo.Split(' ');
                            ShowSaleBook(strBillNo[0], strBillNo[1]);
                        }
                        else
                        {
                            OpenNewDrill(StateIndex + 1);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SalesDrillDownReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                int stateID = Array.IndexOf(StateList, State);
                if (stateID > 0)
                    State = StateList[stateID - 1];
                else
                    State = StateList[0];

                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["SNo"].Value = _index++;

            }
            catch { }
        }
    }
}
