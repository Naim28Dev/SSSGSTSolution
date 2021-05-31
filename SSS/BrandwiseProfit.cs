using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class BrandwiseProfit : Form
    {
        DataBaseAccess dba;
        string LocalMode = "HIGH";
        public BrandwiseProfit()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        public BrandwiseProfit(string _Mode)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            LocalMode = _Mode;
            GetRecord();
        }

        private void BrandwiseProfit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void GetRecord()
        {
            try
            {
                string strSubQuery="",strBrandQuery="";
                if (txtBrandName.Text != "" && rdoBrandWise.Checked)
                    strBrandQuery = " and ISNULL(BrandName,'')='" + txtBrandName.Text + "' ";

                if (txtItemName.Text != "" && rdoItemName.Checked)
                {
                    strSubQuery += " and _IM.ItemName='" + txtItemName.Text + "' ";
                }
                if (txtBranch.Text != "" && rdoBranch.Checked)
                    strSubQuery += " and SB.BillCode Like('%" + txtBranch.Text + "%') ";

                //if (chkDate.Checked && txtFromDate.TextLength>9 && txtToDate.TextLength>9)
                //    strSubQuery += " and SB.Date>='" + dba.ConvertDateInExactFormat(txtFromDate.Text).ToString("MM/dd/yyyy") + "' and SB.Date<'" + dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1).ToString("MM/dd/yyyy") + "' ";

                ////" + strSubQuery.Replace("SB.Date", "SM.Date").Replace("SB.BillCode", "SM.BillCode") + "

                ////Sale Ratio and Brand wise profit
                //if (rdoBrandWise.Checked)
                //    strQuery += " Select BrandName,SUM(NetAmt)NetAmt from(Select BrandName,(Amt-(ISNULL(PRate,0)*SQty))NetAmt from ( Select BrandName,BarCode,ItemName,Variant1,Variant2,SUM(SAmt)Amt,SQty from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,SBS.BarCode,SBS.ItemName,SBS.Variant1,SBS.Variant2,(SBS.Amount) SAmt,SBS.Qty as SQty from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName Where SB.BillNo>0 " + strSubQuery + ") Sales Group by BrandName,BarCode,ItemName,Variant1,Variant2,SQty)_Sales OUTER APPLY (Select MAX(PRate)PRate from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Rate) PRate from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(_Sales.BarCode,'') and SM.ItemName=_Sales.ItemName and SM.Variant1=_Sales.Variant1 and SM.Variant2=_Sales.Variant2 and Qty!=0 )_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!=''  )Purchase  UNION ALL Select BrandName,SUM(SAmt-PAmt)NetAmt from ( Select ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName Where SE.BillNo>0 " + strSubQuery.Replace("SB.Date", "SE.BillDate").Replace("SB.BillCode", "SE.BillCode") + ")Sale Group by BrandName )_Sale Where BrandName!='' " + strBrandQuery + " Group by BrandName   ";//Order by SUM(NetAmt) desc
                //else if (rdoItemName.Checked)
                //{
                //    strQuery += " Select ItemName as BrandName,SUM(NetAmt)NetAmt from(Select ItemName,BrandName,(Amt-(ISNULL(PRate,0)*SQty))NetAmt from (Select BrandName,BarCode,ItemName,Variant1,Variant2,SUM(SAmt)Amt,SQty from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,SBS.BarCode,SBS.ItemName,SBS.Variant1,SBS.Variant2,(SBS.Amount) SAmt,SBS.Qty as SQty from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName Where SB.BillNo>0 " + strSubQuery + " ) Sales Group by ItemName,BrandName,BarCode,ItemName,Variant1,Variant2,SQty "
                //             + " )_Sales OUTER APPLY (Select MAX(PRate)PRate from (Select _IM.ItemName,(CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Rate) PRate from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(_Sales.BarCode,'') and SM.ItemName=_Sales.ItemName and SM.Variant1=_Sales.Variant1 and SM.Variant2=_Sales.Variant2  and Qty!=0)_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!=''  "
                //             + " )Purchase  UNION ALL Select ItemName,BrandName,SUM(SAmt-PAmt)NetAmt from ( Select _IM.ItemName ,ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName Where SE.BillNo>0  " + strSubQuery.Replace("SB.Date", "SE.BillDate").Replace("SB.BillCode", "SE.BillCode") + ")Sale Group by  ItemName,BrandName )_Sale Where BrandName!='' " + strBrandQuery + " Group by ItemName  ";
                //}
                //else
                //{
                //    strQuery = " Select REPLACE(REPLACE(REPLACE(BillCode,'20-21/',''),'21-22/',''),'22-23/','') as BrandName,SUM(NetAmt)NetAmt from(Select BillCode,BrandName,(Amt-(ISNULL(PRate,0)*SQty))NetAmt from ( Select BillCode,BrandName,BarCode,ItemName,Variant1,Variant2,SUM(SAmt)Amt,SQty from (Select SB.BillCode,(CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,SBS.BarCode,SBS.ItemName,SBS.Variant1,SBS.Variant2,(SBS.Amount) SAmt,SBS.Qty as SQty from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName Where SB.BillNo>0 " + strSubQuery + " ) Sales Group by BillCode,BrandName,BarCode,ItemName,Variant1,Variant2,SQty  "
                //             + " )_Sales OUTER APPLY (Select MAX(PRate)PRate from (Select SM.BillCode,(CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Rate) PRate from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(_Sales.BarCode,'') and SM.ItemName=_Sales.ItemName and SM.Variant1=_Sales.Variant1 and SM.Variant2=_Sales.Variant2  and Qty!=0)_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!='' "
                //             + " )Purchase  UNION ALL Select BillCode,BrandName,SUM(SAmt-PAmt)NetAmt from ( Select SE.BillCode ,ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName Where SE.BillNo>0 " + strSubQuery.Replace("SB.Date", "SE.BillDate").Replace("SB.BillCode", "SE.BillCode") + " )Sale Group by  BillCode,BrandName )_Sale Where BrandName!='' " + strBrandQuery + " Group by BillCode ";

                //}

                string strQuery = "";

                string FromDt = "", ToDt = "";
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                if (chkDate.Checked)
                {
                    FromDt = (txtFromDate.Text.Trim().Length == 10 )? txtFromDate.Text : "";
                    ToDt = (txtToDate.Text.Trim().Length == 10) ? txtToDate.Text : "";
                    FromDate =  dba.ConvertDateInExactFormat(FromDt);
                    ToDate = dba.ConvertDateInExactFormat(ToDt);
                }

                strQuery = " declare @InBarcode nvarchar(100), @OutBarcode nvarchar(100), @InQty numeric(18, 2), @OutQty numeric(18, 2), @InBrand nvarchar(max), @OutBrand nvarchar(max), @InDate DateTime, @OutDate DateTime, @InRate numeric(18, 2), @OutRate numeric(18, 2)"
                + " create table #InStock (In_ID int identity not null,In_Barcode nvarchar(100),In_Qty numeric(18,2),In_Rate numeric(18,2),In_Brand nvarchar(50),In_Date date) "
                + " create table #OutStock (Out_ID int identity not null,Out_Barcode nvarchar(100),Out_Qty numeric(18,2),Out_Rate numeric(18,2),Out_Brand nvarchar(50),Out_Date date) "
                + " declare TempCursor Cursor for "
                + " select Barcode, sum(Qty)Qty, Rate, BrandName, CONVERT(Date, Date, 103)Date  from StockMaster where BillType in ('Purchase', 'Opening', 'StockIn', 'SaleReturn') "
                + " and(Case When isnull('"+ (FromDt != "" ? FromDate.ToString("yyyy-MM-dd"):"") + "', '') = '' Then 1 "
                + " when isnull('" + (FromDt != "" ? FromDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) >= Convert(Date,'" + (FromDt != "" ? FromDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                + " and(Case When isnull('"+ (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') = '' Then 1 "
                + " when isnull('" + (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) <= Convert(Date,'" + (ToDt != "" ? ToDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 " + strSubQuery + strBrandQuery
                + " group by Barcode,BrandName,Rate,CONVERT(Date, Date, 103) order by BrandName "
                + " open TempCursor "
                + " FETCH NEXT FROM TempCursor INTO @InBarcode,@InQty,@InRate,@InBrand,@InDate "
                + " WHILE @@FETCH_STATUS = 0 "
                + " BEGIN insert into #InStock(In_Barcode,In_Qty,In_Rate,In_Brand,In_Date) values(@InBarcode,@InQty,@InRate,@InBrand,@InDate) "
                + " FETCH NEXT FROM TempCursor INTO @InBarcode,@InQty,@InRate,@InBrand,@InDate END "
                + " CLOSE TempCursor DEALLOCATE TempCursor "
                + " declare TempCursor1 Cursor for "
                + " select Barcode, sum(Qty)Qty, Rate, BrandName, CONVERT(Date, Date, 103)Date from StockMaster where BillType in ('Sales', 'PurchaseReturn', 'StockOut') "
                + " and(Case When  isnull('" + (FromDt != "" ? FromDate.ToString("yyyy-MM-dd") : "") + "', '') = '' Then 1 "
                + " when  isnull('" + (FromDt != "" ? FromDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) >= Convert(Date,'" + (FromDt != "" ? FromDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                + " and(Case When isnull('" + (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') = '' Then 1 "
                + " when isnull('" + (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) <= Convert(Date,'" + (ToDt != "" ? ToDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 " + strSubQuery + strBrandQuery
                + " group by Barcode,BrandName,Rate,CONVERT(Date, Date, 103) order by BrandName "
                + " open TempCursor1 "
                + " FETCH NEXT FROM TempCursor1 INTO @OutBarcode,@OutQty,@OutRate,@OutBrand,@OutDate "
                + " WHILE @@FETCH_STATUS = 0 "
                + " BEGIN "
                + " insert into #OutStock(Out_Barcode,Out_Qty,Out_Rate,Out_Brand,Out_Date) values(@OutBarcode,@OutQty,@OutRate,@OutBrand,@OutDate) "
                + " FETCH NEXT FROM TempCursor1 INTO @OutBarcode,@OutQty,@OutRate,@OutBrand,@OutDate "
                + " END "
                + " CLOSE TempCursor1 "
                + " DEALLOCATE TempCursor1 "
                + " select top 10 Out_Brand,AVG(Profit) as Profit from( "
                + " select OS.Out_Brand, OS.Out_Barcode, OS.Out_Qty, ((OS.Out_Rate - INS.In_Rate) * OS.Out_Qty) as Profit "
                + " from #InStock INS  "
                + " Right join #OutStock OS  on INS.In_Barcode=OS.Out_Barcode and INS.In_Brand=os.Out_Brand "
                + " )Brand "
                + " group by Out_Brand "
                + " Having(isnull(Out_Brand,'') <> '' OR AVG(Profit) > 0) "
                + " order by Profit desc, Out_Brand asc "
                + " select top 10 Out_Brand,AVG(Profit) as Profit from( "
                + " select OS.Out_Brand, OS.Out_Barcode, OS.Out_Qty, ((OS.Out_Rate - INS.In_Rate) * OS.Out_Qty) as Profit "
                + " from #InStock INS  "
                + " Right join #OutStock OS  on INS.In_Barcode=OS.Out_Barcode and INS.In_Brand=os.Out_Brand  "
                + " )Brand "
                + " group by Out_Brand "
                + " Having(isnull(Out_Brand,'') <> '' OR AVG(Profit) > 0) "
                + " order by Profit Asc, Out_Brand desc drop table #InStock drop table #OutStock";

                 //string ProcName = "SP_GetBrandDetailsProfitWise";


                //List<SqlParameter> paramss = new List<SqlParameter>();
                //paramss.Add(new SqlParameter("@StartDate", FromDt));
                //paramss.Add(new SqlParameter("@EndDate", ToDt));

                DataSet _ds = dba.GetDataSet(strQuery);
                BindBrandwithControl(_ds);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BindBrandwithControl(DataSet _ds)
        {
            dgrdProfitDetails.Rows.Clear();
            dgrdLossDetails.Rows.Clear();
            int _index = 0;
            DataTable DT = new DataTable();
            if (_ds.Tables.Count > 0)
            {
                DT = _ds.Tables[0];
                if (DT.Rows.Count > 0 && LocalMode == "HIGH")
                {
                    //DataRow[] ValidRows = DT.Select("Out_Brand not is null");
                    //DT = ValidRows.CopyToDataTable();
                    //DataView _dv = DT.DefaultView;
                    //_dv.Sort = " Profit desc, Out_Brand asc";
                    //DT = _dv.ToTable();
                    dgrdProfitDetails.Rows.Add(DT.Rows.Count);
                    foreach (DataRow row in DT.Rows)
                    {
                        dgrdProfitDetails.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                        dgrdProfitDetails.Rows[_index].Cells["brandName"].Value = row["Out_Brand"];
                        dgrdProfitDetails.Rows[_index].Cells["saleAmt"].Value = dba.ConvertObjectToDouble(row["Profit"]);
                        _index++;
                    }
                }
            }
            if (_ds.Tables.Count > 1)
            {
                DT = _ds.Tables[1];
                if (DT.Rows.Count > 0 && LocalMode == "LOW")
                {
                    _index = 0;
                    dgrdLossDetails.Rows.Add(DT.Rows.Count);
                    foreach (DataRow row in DT.Rows)
                    {
                        if (Convert.ToString(row["Out_Brand"]) != "")
                        {
                            dgrdLossDetails.Rows[_index].Cells["lSNo"].Value = (_index + 1) + ".";
                            dgrdLossDetails.Rows[_index].Cells["lBrandName"].Value = row["Out_Brand"];
                            dgrdLossDetails.Rows[_index].Cells["lSaleAmt"].Value = dba.ConvertObjectToDouble(row["Profit"]);
                        }
                        _index++;
                    }
                }
            }
            if(LocalMode == "HIGH")
            {
                lblHeader.Text = "BRAND WISE HIGH PROFIT SUMMARY";
                pnlProfitDetails.Visible = true;
                pnlLossDetails.Visible = false;
            }
            else
            {
                lblHeader.Text = "BRAND WISE LOW PROFIT SUMMARY";
                pnlProfitDetails.Visible = false;
                pnlLossDetails.Visible = true;
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBrandName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBrandName.Text = objSearch.strSelectedData;
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
            dba.GetDateInExactFormat(sender, true, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            GetRecord();
            btnGo.Enabled = true;
        }

        private void rdoBrandWise_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoBrandWise.Checked)
                {
                    lblHeader.Text = "BRAND WISE " + LocalMode.ToUpper() + " PROFIT SUMMARY";
                    dgrdProfitDetails.Columns["brandName"].HeaderText = dgrdLossDetails.Columns["lBrandName"].HeaderText = "Brand Name";

                    dgrdProfitDetails.Rows.Clear();
                    dgrdLossDetails.Rows.Clear();
                }
            }
            catch { }
        }


        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", "", "", "", "", "", e.KeyCode, false, "");
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void txtBranch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranch.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void rdoItemName_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoItemName.Checked)
                {
                    lblHeader.Text = "ITEM WISE " + LocalMode.ToUpper() + " PROFIT SUMMARY";
                    dgrdProfitDetails.Columns["brandName"].HeaderText = dgrdLossDetails.Columns["lBrandName"].HeaderText = "Item Name";

                    dgrdProfitDetails.Rows.Clear();
                    dgrdLossDetails.Rows.Clear();
                }
            }
            catch { }
        }

        private void rdoBranch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoBranch.Checked)
                {
                    lblHeader.Text = "BRANCH WISE " + LocalMode.ToUpper() + " PROFIT SUMMARY";
                    dgrdProfitDetails.Columns["brandName"].HeaderText = dgrdLossDetails.Columns["lBrandName"].HeaderText = "Branch Name";

                    dgrdProfitDetails.Rows.Clear();
                    dgrdLossDetails.Rows.Clear();
                }
            }
            catch { }
        }

        private void dgrdProfitDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    string strValue = Convert.ToString(dgrdProfitDetails.CurrentRow.Cells["brandName"].Value);
                    ShowDetails(strValue);
                }
            }
            catch
            {
            }
        }

        private void ShowDetails(string strValue)
        {
            try
            {
                DataGridView dgrd = new DataGridView();
                if (rdoBrandWise.Checked)
                {
                    dgrd.Columns.Add("brandName", "BrandName");
                    dgrd.Rows.Add();
                    dgrd.Rows[0].Cells["brandName"].Value = strValue;
                }
                else if (rdoItemName.Checked)
                {
                    dgrd.Columns.Add("itemName", "ItemName");
                    dgrd.Rows.Add();
                    dgrd.Rows[0].Cells["itemName"].Value = strValue;
                }
                else
                {
                    dgrd.Columns.Add("sno", "sno");
                    dgrd.Rows.Add();
                    dgrd.Rows[0].Cells["sno"].Value = strValue;
                }

                MonthlyStockRegister objMonthly = new MonthlyStockRegister(dgrd.Rows[0], true); //rdoDetail.Checked
                objMonthly.MdiParent = MainPage.mymainObject;
                objMonthly.Show();
            }
            catch { }
        }

        private void dgrdLossDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {
                    string strValue = Convert.ToString(dgrdLossDetails.CurrentRow.Cells["lBrandName"].Value);
                    ShowDetails(strValue);
                }
            }
            catch
            {
            }
        }
    }
}
