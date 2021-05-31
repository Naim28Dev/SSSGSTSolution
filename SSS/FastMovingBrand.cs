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
    public partial class FastMovingBrand : Form
    {
        DataBaseAccess dba;
        string LocalMode = "FAST";
        public FastMovingBrand(string _Mode)
        {
            LocalMode = _Mode;
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            GetRecord();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FastMovingBrand_KeyDown(object sender, KeyEventArgs e)
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
                string strBrandQuery = "";
                if (txtBrandName.Text != "")
                    strBrandQuery = " and ISNULL(BrandName,'')='" + txtBrandName.Text + "' ";
                //if (chkDate.Checked && txtFromDate.TextLength > 9 && txtToDate.TextLength > 9)
                //    strSubQuery += " and SB.Date>='" + dba.ConvertDateInExactFormat(txtFromDate.Text).ToString("MM/dd/yyyy") + "' and SB.Date<'" + dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1).ToString("MM/dd/yyyy") + "' ";


                ////Sale Ratio and Brand wise profit
                ////strQuery += " Select BrandName,SUM(NetAmt)NetAmt from(Select BrandName,(Amt-ISNULL(PAmt,0))NetAmt from ( Select BrandName,SUM(SAmt)Amt from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,(SBS.Amount) SAmt from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode=SBS.BillCode and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName Where SB.BillNo>0 " + strSubQuery + ") Sales Group by BrandName)_Sales OUTER APPLY (Select SUM(PAMt)PAmt from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Qty*Rate) PAmt from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN') and Qty!=0 " + strSubQuery.Replace("SB.Date", "SM.Date") + ")_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!=''  )Purchase  UNION ALL Select BrandName,SUM(SAmt-PAmt)NetAmt from ( Select ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName Where SE.BillNo>0 " + strSubQuery.Replace("SB.Date", "SE.BillDate") + ")Sale Group by BrandName )_Sale Where BrandName!='' " + strBrandQuery + " Group by BrandName   ";//Order by SUM(NetAmt) desc

                //strQuery += " Select * from (Select BrandName,(CASE WHEN _Qty!=0 then ((SQTy*100)/_Qty) else 0 end) as SaleRatio from ( "
                //         + " Select(CASE When ISNULL(PBS.BrandName, '') = '' then ISNULL(_IM.BrandName, '') else ISNULL(PBS.BrandName, '') end) BrandName,ISNULL(PBS.BarCode, '') as BarCode,PBS.ItemName,PBS.Variant1,PBS.Variant2, SUM(Qty)_Qty from PurchaseBook PB inner join PurchaseBookSecondary PBS on PB.BillCode = PBS.BillCode and PB.BillNo = PBS.BillNo inner join Items _Im on PBS.ItemName = _Im.ItemName Group by ISNULL(PBS.BarCode, ''),PBS.ItemName,PBS.Variant1,PBS.Variant2,(CASE When ISNULL(PBS.BrandName, '')= '' then ISNULL(_IM.BrandName, '') else ISNULL(PBS.BrandName, '') end))PBS OUTER APPLY( "
                //         + " Select SUM(SBS.Qty)SQty from SalesBook SB inner join SalesBookSecondary SBS on SB.BillCode = SBS.BillCode and SB.BillNo = SBS.BillNo Where PBS.BarCode=SBS.BarCode and PBS.ItemName = SBS.ItemName and PBS.Variant1 = SBS.Variant1 and PBS.Variant2 = SBS.Variant2 and ISNULL(PBS.BarCode, '') = ISNULL(SBS.BarCode, '') "
                //         + " ) SBS )_SaleRatio Where SaleRatio is not NULL "+ strBrandQuery+" ";

                //DataTable _dt = dba.GetDataTable(strQuery);
                //BindFastMovingBrandwithControl(_dt);

                // string ProcName = "SP_APP_GetTop10Brand";

                string FromDt = "", ToDt = "";
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                if (chkDate.Checked)
                {
                    FromDt = (txtFromDate.Text.Trim().Length == 10) ? txtFromDate.Text : "";
                    ToDt = (txtToDate.Text.Trim().Length == 10) ? txtToDate.Text : "";
                    FromDate = dba.ConvertDateInExactFormat(FromDt);
                    ToDate = dba.ConvertDateInExactFormat(ToDt);
                }

                string strQuery = " declare @InBarcode nvarchar(100),@InItem nvarchar(max),@InVariant1 nvarchar(50),@InVariant2 nvarchar(50),@InMRP numeric(18,2),@InQty int,@InBrand nvarchar(max),@InDesign nvarchar(50),@InDate DateTime"
                                    + " declare @OutBarcode nvarchar(100),@OutItem nvarchar(max),@OutVariant1 nvarchar(50),@OutVariant2 nvarchar(50),@OutMRP numeric(18,2),@OutQty int,@OutBrand nvarchar(max),@OutDesign nvarchar(50),@OutDate DateTime "
                                    + " create table #InStock (In_ID int identity not null,In_Barcode nvarchar(100),In_Item nvarchar(max),In_Variant1 nvarchar(50),In_Variant2 nvarchar(50),In_MRP numeric(18,2),In_Qty bigint,In_Brand nvarchar(250),In_DesignName nvarchar(250),In_Date date) "
                                    + " create table #OutStock (Out_ID int identity not null,Out_Barcode nvarchar(100),Out_Item nvarchar(max),Out_Variant1 nvarchar(50),Out_Variant2 nvarchar(50),Out_MRP numeric(18,2),Out_Qty bigint,Out_Brand nvarchar(250),Out_DesignName nvarchar(250),Out_Date date) "
                                    + " declare TempCursor Cursor for    	select Barcode,ItemName,Variant1,Variant2,MRP,sum(Qty)Qty,BrandName,DesignName,CONVERT(Date,Date,103)Date  from StockMaster where BillType in ('Purchase','Opening','StockIn','SaleReturn') " + strBrandQuery
                                    + " and(Case When isnull('"+ (FromDt != "" ? FromDate.ToString("yyyy-MM-dd"):"") + "', '')  = '' Then 1    when isnull('"+ (FromDt != "" ? FromDate.ToString("yyyy-MM-dd"):"") + "', '')  <> '' AND CONVERT(date, Date, 103) >= Convert(Date,'" + (FromDt != "" ? FromDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                                    + " and(Case When isnull('"+ (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') = '' Then 1    when isnull('"+ (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) <= Convert(Date,'" + (ToDt != "" ? ToDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                                    + " group by Barcode,ItemName,Variant1,Variant2,MRP,BrandName,DesignName,CONVERT(Date,Date,103)  order by BrandName    open TempCursor    FETCH NEXT FROM TempCursor INTO @InBarcode,@InItem,@InVariant1,@InVariant2,@InMRP,@InQty,@InBrand,@inDesign,@InDate "
                                    + " WHILE @@FETCH_STATUS = 0    BEGIN    insert into #InStock(In_Barcode,In_Item,In_Variant1,In_Variant2,In_MRP,In_Qty,In_Brand,In_DesignName,In_Date) values(@InBarcode,@InItem,@InVariant1,@InVariant2,@InMRP,@InQty,@InBrand,@inDesign,@InDate) "
                                    + " FETCH NEXT FROM TempCursor INTO @InBarcode,@InItem,@InVariant1,@InVariant2,@InMRP,@InQty,@InBrand,@inDesign,@InDate    END    CLOSE TempCursor    DEALLOCATE TempCursor    declare TempCursor1 Cursor for "
                                    + " select Barcode,ItemName,Variant1,Variant2,MRP,sum(Qty)Qty,BrandName,DesignName,CONVERT(Date,Date,103)Date from StockMaster where BillType in ('Sales','PurchaseReturn','StockOut')  " + strBrandQuery
                                    + " and(Case When isnull('"+ (FromDt != "" ? FromDate.ToString("yyyy-MM-dd"):"") + "', '')  = '' Then 1    when isnull('"+ (FromDt != "" ? FromDate.ToString("yyyy-MM-dd"):"") + "', '')  <> '' AND CONVERT(date, Date, 103) >= Convert(Date,'" + (FromDt != "" ? FromDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                                    + " and(Case When isnull('"+ (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') = '' Then 1    when isnull('"+ (ToDt != "" ? ToDate.ToString("yyyy-MM-dd") : "") + "', '') <> '' AND CONVERT(date, Date, 103) <= Convert(Date,'" + (ToDt != "" ? ToDate.ToString("dd/MM/yyyy") : "") + "',103) then 1 else 0 end) = 1 "
                                    + " group by Barcode,ItemName,Variant1,Variant2,MRP,BrandName,DesignName,CONVERT(Date,Date,103)  order by BrandName    open TempCursor1 "
                                    + " FETCH NEXT FROM TempCursor1 INTO @OutBarcode,@OutItem,@OutVariant1,@OutVariant2,@OutMRP,@OutQty,@OutBrand,@OutDesign,@OutDate  WHILE @@FETCH_STATUS = 0    BEGIN "
                                    + " insert into #OutStock(Out_Barcode,Out_Item,Out_Variant1,Out_Variant2,Out_MRP,Out_Qty,Out_Brand,Out_DesignName,Out_Date) values(@OutBarcode,@OutItem,@OutVariant1,@OutVariant2,@OutMRP,@OutQty,@OutBrand,@OutDesign,@OutDate) "
                                    + " FETCH NEXT FROM TempCursor1 INTO @OutBarcode,@OutItem,@OutVariant1,@OutVariant2,@OutMRP,@OutQty,@OutBrand,@OutDesign,@OutDate  END    CLOSE TempCursor1    DEALLOCATE TempCursor1 "

                                    + " select top 10 In_Brand,AVG(Ratio) as Ratio from( "
                                    + " select INS.In_Brand, INS.In_Barcode, ((cast(OS.Out_Qty as decimal(18, 4)) * 100) / (CASE WHEN (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18, 4)) * cast(INS.In_Qty  as decimal(18, 4)))!=0 then (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18, 4)) * cast(INS.In_Qty  as decimal(18, 4))) else 1 end)) as Ratio    from #InStock INS     Left join #OutStock OS  on INS.In_Barcode=OS.Out_Barcode and INS.In_Item=os.Out_Item and INS.In_Variant1=os.Out_Variant1 and  INS.In_Variant2=os.Out_Variant2 and INS.In_MRP=os.Out_MRP  and INS.In_Brand=os.Out_Brand and INS.In_DesignName=os.Out_DesignName where In_Qty > 0 and (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18,2))) is not null and (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18,2)))>0 )Brand    group by In_Brand    Having(isnull(In_Brand,'') <> '' OR AVG(Ratio) > 0)    order by Ratio desc, In_Brand asc "

                                    + " select top 10 In_Brand,AVG(Ratio) as Ratio from(    "
                                    + " select INS.In_Brand, INS.In_Barcode,((cast(OS.Out_Qty as decimal(18, 4)) * 100) / (CASE WHEN (cast((DATEDIFF(day, INS.In_Date,DATEADD(dd,1,OS.Out_Date))) as decimal(18, 4)) * cast(INS.In_Qty as decimal(18, 4)))!=0 then (cast((DATEDIFF(day, INS.In_Date,DATEADD(dd,1,OS.Out_Date))) as decimal(18, 4)) * cast(INS.In_Qty as decimal(18, 4))) else 1 end )) as Ratio from #InStock INS Left join #OutStock OS  on INS.In_Barcode=OS.Out_Barcode and INS.In_Item=os.Out_Item and INS.In_Variant1=os.Out_Variant1 and  INS.In_Variant2=os.Out_Variant2 and INS.In_MRP=os.Out_MRP  and INS.In_Brand=os.Out_Brand and INS.In_DesignName=os.Out_DesignName where In_Qty > 0 and (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18,2))) is not null and (cast((DATEDIFF(day, INS.In_Date, DATEADD(dd,1,OS.Out_Date))) as decimal(18,2)))>0  )Brand    group by In_Brand    Having(isnull(In_Brand,'') <> '' OR AVG(Ratio) > 0)    order by Ratio Asc, In_Brand desc drop table #InStock drop table #OutStock ";


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

        private void BindFastMovingBrandwithControl(DataTable dt)
        {
            dgrdFastMoving.Rows.Clear();
            int _index = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdFastMoving.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdFastMoving.Rows[_index].Cells["fSNo"].Value = (_index + 1) + ".";
                    dgrdFastMoving.Rows[_index].Cells["fBrandName"].Value = row["brandName"];
                    dgrdFastMoving.Rows[_index].Cells["fSaleAmt"].Value = dba.ConvertObjectToDouble(row["SaleRatio"]);

                    _index++;
                }
            }
        }


        private void BindBrandwithControl(DataSet _ds)
        {
            dgrdFastMoving.Rows.Clear();
            dgrdSlowMoving.Rows.Clear();
            int _index = 0;
            DataTable DT = new DataTable();
            DataRow[] rows;
            if (_ds.Tables.Count > 0)
            {
                DT = _ds.Tables[0];
                if (DT.Rows.Count > 0 && LocalMode == "FAST")
                {
                    //rows = DT.Select("In_Brand not is null");
                    //if (rows.Length > 0)
                    //{
                        //DataTable _dt = rows.CopyToDataTable();
                        //DataView _dv = _dt.DefaultView;
                        //_dv.Sort = " Ratio desc,In_Brand asc";
                        //_dt = _dv.ToTable();

                    dgrdFastMoving.Rows.Add(DT.Rows.Count);
                    foreach (DataRow row in DT.Rows)
                    {
                        dgrdFastMoving.Rows[_index].Cells["fSNo"].Value = (_index + 1) + ".";
                        dgrdFastMoving.Rows[_index].Cells["fBrandName"].Value = row["In_Brand"];
                        dgrdFastMoving.Rows[_index].Cells["fSaleAmt"].Value = dba.ConvertObjectToDouble(row["Ratio"]);

                        _index++;
                    }
                }
            }
            if (_ds.Tables.Count > 1)
            {
                DT = _ds.Tables[1];
                if (DT.Rows.Count > 0 && LocalMode == "SLOW")
                {
                    //rows = DT.Select("In_Brand not is null");
                    //if (rows.Length > 0)
                    //{
                    //    DataTable _dt = rows.CopyToDataTable();
                    //    DataView _dv = _dt.DefaultView;
                    //    _dv.Sort = " Ratio asc,In_Brand desc";
                    //    _dt = _dv.ToTable();
                    _index = 0;
                    dgrdSlowMoving.Rows.Add(DT.Rows.Count);
                    foreach (DataRow row in DT.Rows)
                    {
                        dgrdSlowMoving.Rows[_index].Cells["sSno"].Value = (_index + 1) + ".";
                        dgrdSlowMoving.Rows[_index].Cells["sBrandName"].Value = row["In_Brand"];
                        dgrdSlowMoving.Rows[_index].Cells["sSaleRatio"].Value = dba.ConvertObjectToDouble(row["Ratio"]);

                        _index++;
                    }
                }
            }

            if (LocalMode == "FAST")
            {
                lblHeader.Text = "BRAND WISE FAST MOVING DETAILS";
                pnlFastMoving.Visible = true;
                pnlSlowMoving.Visible = false;
            }
            else
            {
                lblHeader.Text = "BRAND WISE SLOW MOVING DETAILS";
                pnlFastMoving.Visible = false;
                pnlSlowMoving.Visible = true;
            }
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
    }
}
