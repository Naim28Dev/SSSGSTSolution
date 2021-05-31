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
    public partial class StockDrillDownReport : Form
    {
        DataBaseAccess dba;

        static string strDepartment = "", strCategory = "", strBrand = "", strItem = "", strCat1 = "", strCat2 = "", strCat3 = "", strCat4 = "", strCat5 = "", strBillCodeNo = "";
        string State = "DEPARTMENT";
        string[] StateList = { "DEPARTMENT", "CATEGORY", "BRAND", "ITEM", MainPage.StrCategory1.ToUpper(), MainPage.StrCategory2.ToUpper(), MainPage.StrCategory3.ToUpper(), MainPage.StrCategory4.ToUpper(), MainPage.StrCategory5.ToUpper()
               , "INVOICE NO" };
        bool IsInvoiceShown = false, isLastCategory = false;
        TextInfo textInfo = new CultureInfo("hi-IN", false).TextInfo;
        DataTable BindedDT;
        public StockDrillDownReport()
        {
            InitializeComponent();
            dba = new DataBaseAccess();

            SetReportState(0);
            GetDataFromDataBase(0);
        }

        //public StockDrillDownReport(int _Index)
        //{
        //    InitializeComponent();
        //    dba = new DataBaseAccess();
        //    SetReportState(_Index);
        //    GetDataFromDataBase(_Index);
        //}

        private void StockDrillDownReport_Load(object sender, EventArgs e)
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
            string strQuery = "", strWhereQuery = "", strWhereItem = "", strColumnQuery = "", strColumnFinal = "", strGroupByQuery = "";
            bool _isCat = false;

            if (_index >= 0)
            {
                strColumnFinal = " Department";
                strColumnQuery = " Isnull(IM.MakeName,'') as Department";
                //    strGroupByQuery = " Group By Department";
            }
            if (_index >= 1)
            {
                strColumnFinal += ", Category";
                strColumnQuery += ", Isnull(IM.Other,'') as Category";
                strWhereQuery = " Where Isnull(IM.MakeName,'') in ('" + strDepartment + "') ";
                //   strGroupByQuery += ", Category"; 
            }

            if (_index >= 2)
            {
                strColumnFinal += ", Brand";
                strColumnQuery += ", Isnull(ST.BrandName,'') as Brand";
                strWhereQuery += " and Isnull(IM.Other,'') in ('" + strCategory + "') ";
                //   strGroupByQuery += ", Brand";
            }

            if (_index >= 3)
            {
                strColumnFinal += ", Item";
                strColumnQuery += ", Isnull(ST.ItemName,'') as Item";
                strWhereQuery += " and Isnull(ST.BrandName,'') in ('" + strBrand + "') ";
                strWhereItem += " AND SM.BrandName IN ('" + strBrand + "')";
                //   strGroupByQuery += ", Item ";
            }

            if (_index >= 4)
            {
                if (MainPage.StrCategory1 != "")
                {
                    strColumnFinal += ", " + MainPage.StrCategory1;
                    strColumnQuery += ", Isnull(ST.Variant1,'') as " + MainPage.StrCategory1;
                    strWhereItem += " AND SM.Variant1 IN ('" + strCat1 + "')";
                    //    strGroupByQuery += ", " + MainPage.StrCategory1;
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
                    strColumnQuery += ", Isnull(ST.Variant2,'') as " + MainPage.StrCategory2;
                    strWhereItem += " AND SM.Variant2 IN ('" + strCat2 + "')";
                    if (MainPage.StrCategory1 != "")
                        strWhereQuery += " and Isnull(ST.Variant1,'') in ('" + strCat1 + "') ";
                    //  strGroupByQuery += ", " + MainPage.StrCategory2;
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
                    strColumnQuery += ", Isnull(ST.Variant3,'') as " + MainPage.StrCategory3;
                    if (MainPage.StrCategory2 != "")
                        strWhereQuery += " and Isnull(ST.Variant2,'') in ('" + strCat2 + "') ";
                    // strGroupByQuery += ", " + MainPage.StrCategory3;
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
                    strColumnQuery += ", Isnull(ST.Variant4,'') as  " + MainPage.StrCategory4;
                    if (MainPage.StrCategory3 != "")
                        strWhereQuery += " and Isnull(ST.Variant3,'') in ('" + strCat3 + "') ";
                    //  strGroupByQuery += ", " + MainPage.StrCategory4;
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
                    strColumnQuery += ", Isnull(ST.Variant5,'') as " + MainPage.StrCategory5;
                    if (MainPage.StrCategory4 != "")
                        strWhereQuery += " and Isnull(ST.Variant4,'') in ('" + strCat4 + "') ";
                    //  strGroupByQuery += ", " + MainPage.StrCategory5;
                    _isCat = true;
                }
                else
                    _index++;
            }

            if (_index >= 9)
            {
                strColumnFinal += ", Invoice_No";
                strColumnQuery += ", Isnull(PBS.BillCode+ ' ' + Convert(varchar(20),PBS.BillNo),'') as Invoice_No";
                // strGroupByQuery += ", InvoiceNo";
                if (MainPage.StrCategory5 != "")
                    strWhereQuery += " and Isnull(Variant5,'') in ('" + strCat5 + "') ";
                _isCat = true;
            }

            if (_isCat)
            {
                strWhereQuery += " and Isnull(ST.ItemName,'') in ('" + strItem + "') ";
                strWhereItem += " AND SM.ItemName IN ('" + strItem + "')";
            }

            strQuery = "SELECT SNo = Row_Number() Over(Order by " + strColumnFinal + ")," + strColumnFinal + ",SUM(Qty)Qty" + (_index >= 9 ? " ,Max(Rate) Rate " : "") + ",Cast(SUM(Amount) as Numeric(18,2))Amount" + (_index >= 9 ? " ,BillType " : "") + " FROM ("
                    + " SELECT " + strColumnQuery + ", Qty = " + (_index >= 9 ? "PBS.Qty" : "StockQty") + ", Purc.AvgRate Rate,(" + (_index >= 9 ? " PBS.Qty " : " StockQty ") + " * Purc.AvgRate) Amount" + (_index >= 9 ? " , BillType " : "")
                    + " FROM ItemStock ST"
                    + " LEFT JOIN Items IM ON ST.ItemName = IM.ItemName";
            if (_index >= 9)
            {
                string str = " Declare @StockQty Numeric(18, 2) SELECT @StockQty = StockQty FROM ItemStock SM Where 1=1 " + strWhereItem + " SELECT * INTO #temp FROM (SELECT ID = ROW_NUMBER() OVER(Order By Date Desc),SM.BillCode,SM.BillNo,SM.BillType, Date,ItemName,BrandName,Barcode,Variant1,Variant2,Variant3,Variant4,Variant5,Sum(Qty)Qty FROM StockMaster SM Where SM.BillType IN ('PURCHASE', 'SALERETURN', 'STOCKIN', 'OPENING') " + strWhereItem + " GROUP BY Date,SM.BillCode,SM.BillNo,SM.BillType,ItemName,BrandName,Barcode,Variant1,Variant2,Variant3,Variant4,Variant5 )Qry ORDER BY Date Desc ";
                str += " SELECT * INTO #temp2 FROM ( SELECT Date, BillCode, BillNo, BillType, ItemName, BrandName, Barcode, Variant1, Variant2, Variant3, Variant4, Variant5, a.Qty ,isnull((SELECT Sum(b.Qty) FROM #temp b WHERE b.ID <= a.ID AND b.ItemName=a.ItemName AND b.BrandName = a.BrandName AND b.Barcode=a.Barcode  AND b.Variant1 = a.Variant1 AND b.Variant2 = a.Variant2 AND b.Variant3 = a.Variant3 AND b.Variant4 = a.Variant4 AND b.Variant5 = a.Variant5) ,0) RTQty  FROM #temp a  )PBS2  Where 1=1 " + strWhereItem.Replace("SM.", "PBS2.") + " ORDER BY ItemName,BrandName,Barcode,Variant1,Variant2,Variant3,Variant4,Variant5,Date Desc ";
                str += " SELECT * INTO #temp3 FROM ( SELECT BillCode, BillNo, BillType, ItemName, BrandName, Barcode, Variant1, Variant2, Variant3, Variant4, Variant5, Qty FROM #temp2 WHERE RTQty <= @StockQty UNION ALL SELECT top 1 BillCode,BillNo, BillType,ItemName,BrandName,Barcode,Variant1,Variant2,Variant3,Variant4,Variant5,Qty = (Qty - (RTQty - @StockQty)) FROM #temp2 WHERE RTQty > @StockQty  )Final ";
                strQuery = str + strQuery;
                strQuery += " LEFT JOIN ( SELECT * FROM #temp3 ) PBS ON ST.ItemName = PBS.ItemName AND ST.BarCode = PBS.BarCode AND ST.BrandName = PBS.BrandName  ";
            }

            strQuery += " LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY BarCode, BrandName, ItemName, Variant1, Variant2 ORDER BY ID ASC) AS RNo, BrandName, BarCode, ItemName, Variant1, Variant2, SUM(PQty)TPQty, CAST((SUM(PAmt) / SUM(PQty)) as Numeric(18, 4))AvgRate FROM ("
                   + " SELECT(CASE WHEN(SELECT TOP 1 StockAsPer FROM CompanySetting) = 'AvgRate' then 2 else 0 end) ID, IM.BrandName, Description BarCode, IM.ItemName, Variant1, Variant2, 1 PQty, IMS.PurchaseRate PAmt FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE IMS.PurchaseRate != 0 " + strWhereItem.Replace("SM.BrandName", "IM.BrandName").Replace("SM.ItemName", "IM.ItemName").Replace("SM.", "IMS.")
                   + " UNION ALL"
                   + "  SELECT(CASE WHEN(SELECT TOP 1 StockAsPer FROM CompanySetting) = 'AvgRate' then 0 else 1 end) ID, BrandName, BarCode, ItemName, Variant1, Variant2, (CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE' WHERE BillType IN('OPENING','PURCHASE','STOCKIN')" + strWhereItem
                   + " UNION ALL"
                   + " SELECT(CASE WHEN(SELECT TOP 1 StockAsPer FROM CompanySetting) = 'AvgRate' then 1 else 2 end) ID ,BrandName,BarCode,ItemName,Variant1,Variant2,(CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE'    WHERE BillType IN('SALERETURN')" + strWhereItem
                   + " )Ratt GROUP BY BrandName, BarCode, ItemName, Variant1, Variant2, ID"
                   + " )Purc on ST.BrandName = Purc.BrandName and ISNULL(ST.BarCode, '') = ISNULL(Purc.BarCode, '') and ST.ItemName = Purc.ItemName and ST.Variant1 = Purc.Variant1 and ST.Variant2 = Purc.Variant2 AND RNo = 1"
                   + strWhereQuery
                   + " )Final GROUP BY " + strColumnFinal + (_index >= 9 ? " ,BillType" : "") + " Order By " + strColumnFinal;
            if (_index >= 9)
                strQuery += "  DROP table #temp DROP table #temp2 DROP table #temp3";

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
                strBillCodeNo = dgrdDetails.CurrentRow.Cells["Invoice_No"].Value.ToString();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    string HeaderText = dgrdDetails.Columns[e.ColumnIndex].HeaderText;

                    if (HeaderText.ToUpper().Contains("INVOICE NO"))
                    {
                        string BillType = "";
                        strBillCodeNo = dgrdDetails.CurrentCell.Value.ToString();
                        BillType = dgrdDetails.CurrentRow.Cells["BillType"].Value.ToString();
                        string[] strBillNo = strBillCodeNo.Split(' ');
                        dba.ShowTransactionBook(BillType, strBillNo[0], strBillNo[1]);
                    }
                }
            }
            catch
            {

            }
        }
        private void ShowPurchaseBook(string strCode, string strBillNo)
        {
            dba.ShowTransactionBook("PURCHASE", strCode, strBillNo);
        }

        private void dgrdDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    int StateIndex = Array.IndexOf(StateList, State);
                    int count = this.StateList.Count();
                    string HeaderText = dgrdDetails.Columns[e.ColumnIndex].HeaderText, BillType = "";

                    setFilters();
                    if (StateIndex < count)
                    {
                        if (IsInvoiceShown)
                        {
                            strBillCodeNo = dgrdDetails.Rows[e.RowIndex].Cells["Invoice_No"].Value.ToString();
                            BillType = dgrdDetails.Rows[e.RowIndex].Cells["BillType"].Value.ToString();
                            string[] strBillNo = strBillCodeNo.Split(' ');

                            dba.ShowTransactionBook(BillType, strBillNo[0], strBillNo[1]);
                            //ShowPurchaseBook(strBillNo[0], strBillNo[1]);
                        }
                        else
                        {
                            OpenNewDrill(StateIndex + 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void BindDataTable(DataTable table)
        {
            try
            {
                IsInvoiceShown = false;
                dgrdDetails.DataSource = null;
                if (table != null && table.Rows.Count > 0)
                {
                    DataView dataView = new DataView(table);
                    dgrdDetails.DataSource = dataView;
                    SetColumnStyle();
                    GetSum();
                    if (dgrdDetails.Rows.Count > 0)
                    {
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["Amount"];
                        dgrdDetails.Focus();
                    }
                    //BindedDT = table.Clone();
                    //BindedDT = table;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Bind Data To Grid.", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        private void GetSum()
        {
            try
            {
                double totalAmt = 0, totalQty = 0;
                totalQty = dgrdDetails.Rows.Cast<DataGridViewRow>()
                    .Sum(t => dba.ConvertObjectToDouble(t.Cells["Qty"].Value));
                totalAmt = dgrdDetails.Rows.Cast<DataGridViewRow>()
                    .Sum(t => dba.ConvertObjectToDouble(t.Cells["Amount"].Value));

                lblTotalAmt.Text = totalAmt.ToString("N2", MainPage.indianCurancy);
                lblTotalQty.Text = totalQty.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }
        private void SetColumnStyle()
        {
            for (int i = 0; i < dgrdDetails.Columns.Count; i++)
            {
                try
                {
                    DataGridViewCellStyle cellStyle = dgrdDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdDetails.Columns[i];

                    string strAlign = "LEFT", clmname = _column.Name.ToUpper();
                    int _width = 150;
                    _column.Width = _width;
                    if (clmname == "BILLTYPE")
                        _column.Visible = false;
                    if (clmname == "SNO")
                        _width = 50;
                    if (clmname == "ITEM")
                        _width = 200;
                    if (clmname == "QTY")
                    {
                        strAlign = "RIGHT"; _width = 100;
                        cellStyle.Format = "N2";
                    }
                    if (clmname == "AMOUNT" || clmname == "RATE")
                    {
                        strAlign = "RIGHT"; _width = 120;
                        cellStyle.Format = "N2";
                    }
                    if (clmname == MainPage.StrCategory1.ToUpper() || clmname == MainPage.StrCategory2.ToUpper() || clmname == MainPage.StrCategory3.ToUpper() || clmname == MainPage.StrCategory4.ToUpper() || clmname == MainPage.StrCategory5.ToUpper())
                        _width = 80;
                    if (clmname.Contains("INVOICE_NO"))
                    {
                        _width = 120;
                        cellStyle.ForeColor = Color.FromArgb(64, 64, 0);
                        cellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Underline);
                        IsInvoiceShown = true;
                    }
                    else
                        cellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);

                    if (clmname == "VARIANT1")
                        _column.HeaderText = MainPage.StrCategory1;
                    if (clmname == "VARIANT2")
                        _column.HeaderText = MainPage.StrCategory2;
                    if (clmname == "VARIANT3")
                        _column.HeaderText = MainPage.StrCategory3;
                    if (clmname == "VARIANT4")
                        _column.HeaderText = MainPage.StrCategory4;
                    if (clmname == "VARIANT5")
                        _column.HeaderText = MainPage.StrCategory5;

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
                catch { }
            }
        }
        //private void BindColumn(DataTable _dt)
        //{
        //    IsInvoiceShown = false;
        //    dgrdDetails.Columns.Clear();
        //    CreateGridviewColumn("SNo", "S.No", "RIGHT", 50);

        //    foreach (DataColumn Dtc in _dt.Columns)
        //    {
        //        string align = "LEFT", columnName = Dtc.ColumnName.ToString();
        //        int width = 150;
        //        if (columnName.ToUpper().Contains("ITEM"))
        //        {
        //            width = 200;
        //        }
        //        if (columnName.ToUpper().Contains("QTY"))
        //        {
        //            align = "RIGHT"; width = 70;
        //        }
        //        if (columnName.ToUpper().Contains("AMOUNT"))
        //        {
        //            align = "RIGHT"; width = 100;
        //        }
        //        if (columnName.ToUpper().Contains("SIZE"))
        //        {
        //            width = 70;
        //        }
        //        if (columnName.ToString().ToUpper().Contains("INVOICE_NO"))
        //        {
        //            CreateGridviewLinkColumn(columnName, columnName, align, width);
        //            IsInvoiceShown = true;
        //        }
        //        else
        //        {
        //            CreateGridviewColumn(columnName, columnName, align, width);
        //        }
        //    }
        //}

        //private void CreateGridviewColumn(string strColName, string strColHeader, string strAlign, int _width)
        //{
        //    try
        //    {
        //        DataGridViewColumn _column = new DataGridViewColumn();
        //        DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
        //        _column.Name = strColName;
        //        _column.HeaderText = textInfo.ToTitleCase(strColHeader);
        //        _column.Width = _width;
        //        _column.SortMode = DataGridViewColumnSortMode.Automatic;
        //        if (strAlign == "LEFT")
        //        {
        //            _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //            _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
        //        }
        //        else
        //        {
        //            _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //            _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
        //            if (_width != 50)
        //                _column.DefaultCellStyle.Format = "N2";
        //        }
        //        _column.CellTemplate = dataGridViewCell;
        //        dgrdDetails.Columns.Add(_column);
        //    }
        //    catch { }
        //}

        //private void CreateGridviewLinkColumn(string strColName, string strColHeader, string strAlign, int _width)
        //{
        //    try
        //    {
        //        DataGridViewColumn _column = new DataGridViewColumn();
        //        DataGridViewLinkCell dataGridViewCell = new DataGridViewLinkCell();

        //        _column.Name = strColName;
        //        _column.HeaderText = textInfo.ToTitleCase(strColHeader);
        //        _column.Width = _width;
        //        _column.SortMode = DataGridViewColumnSortMode.Automatic;
        //        if (strAlign == "LEFT")
        //        {
        //            _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //            _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);

        //        }
        //        else
        //        {
        //            _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //            _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
        //            if (_width != 50)
        //                _column.DefaultCellStyle.Format = "N2";
        //        }
        //        dataGridViewCell.LinkColor = Color.FromArgb(64, 64, 0);
        //        dataGridViewCell.LinkBehavior = LinkBehavior.HoverUnderline;
        //        dataGridViewCell.ActiveLinkColor = Color.Red;

        //        _column.CellTemplate = dataGridViewCell;
        //        dgrdDetails.Columns.Add(_column);
        //    }
        //    catch { }
        //}


        //private void BindDataTable(DataTable table)
        //{
        //    dgrdDetails.Rows.Clear();
        //    if (table.Rows.Count > 0)
        //        dgrdDetails.Rows.Add(table.Rows.Count);

        //    int _rowIndex = 0;
        //    double dAmount = 0, dQty = 0;
        //    try
        //    {
        //        foreach (DataRow row in table.Rows)
        //        {
        //            dgrdDetails.Rows[_rowIndex].Cells["SNo"].Value = (_rowIndex + 1);

        //            foreach (DataColumn column in table.Columns)
        //            {
        //                string columnName = column.ColumnName.ToString();
        //                dgrdDetails.Rows[_rowIndex].Cells[columnName].Value = row[columnName];
        //            }
        //            _rowIndex++;
        //            dAmount += dAmount = dba.ConvertObjectToDouble(row["Amount"]);
        //            dQty += dQty = dba.ConvertObjectToDouble(row["Qty"]);
        //        }
        //        lblTotalAmt.Text = dAmount.ToString("N2", MainPage.indianCurancy);
        //        lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
        //    }
        //    catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        //    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[dgrdDetails.ColumnCount - 1];
        //}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblDepartment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // clearFilters(0);
            OpenNewDrill(0);
        }

        private void lblCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(1);
        }

        private void lblBrand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(2);
        }

        private void lblItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(3);
        }

        private void dgrdDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Invoice No")
                dgrdDetails.Cursor = Cursors.Hand;
            else
                dgrdDetails.Cursor = Cursors.Arrow;
        }

        private void dgrdDetails_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgrdDetails.Cursor = Cursors.Arrow;
        }

        private void lblCategory1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(4);
        }

        private void lblCategory2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(5);
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            GetDataFromDataBase(0);
        }

        private void lblCategory3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(6);
        }

        private void lblCategory4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(7);
        }

        private void lblCategory5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenNewDrill(8);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                dba.ExportToExcel(dgrdDetails, "Stock_DrillDown_Report", "Stock Report");
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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

                    string HeaderText = dgrdDetails.Columns[colIndex].HeaderText, BillType = "";
                    setFilters();

                    if (StateIndex < count)
                    {
                        if (IsInvoiceShown)
                        {
                            int rowIndex = dgrdDetails.SelectedCells[0].OwningRow.Index;
                            strBillCodeNo = dgrdDetails.Rows[rowIndex].Cells["Invoice_No"].Value.ToString();
                            BillType = dgrdDetails.Rows[rowIndex].Cells["BillType"].Value.ToString();
                            string[] strBillNo = strBillCodeNo.Split(' ');

                            dba.ShowTransactionBook(BillType, strBillNo[0], strBillNo[1]);
                            //ShowPurchaseBook(strBillNo[0], strBillNo[1]);
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

        private void StockDrillDownReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                int stateID = Array.IndexOf(StateList, State);
                if (stateID > 0)
                    State = StateList[stateID - 1];
                else
                    State = StateList[0];
                if (stateID == 0)
                    this.Close();
                else
                    OpenNewDrill(stateID - 1);
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
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
