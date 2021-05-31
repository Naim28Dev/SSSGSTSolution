using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class SearchCategory_Custom : Form
    {
        public string strSearchData = "", strSelectedData = "", strSubQuery = "", strOrderStatus = "", strCompanyCode = "";
        public List<string> strSelectedRows = new List<string>();
        DataTable table = null, dtSaleRate = null;
        bool chkStatus = false, boxStatus = false, closeStatus = false;
        public string strFullOrderNumber = "", strFullDesignNo = "", strPartyName = "", strCartonStatus = "";
        string strDBName = "", strCatNo = "1", strCatName = "", strPONumber = "", strSONumber = "", strBrandName, strDesignName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "", strInvokedFrom = "";

        // int MinIndex = 1, MaxIndex = 50;


        public SearchCategory_Custom(string DBName, string strData, string strHeader, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5, Keys objKey, bool bStatus, bool _bchkStatus)
        {
            InitializeComponent();
            strDBName = DBName;
            strCatNo = strData;
            boxStatus = bStatus;
            chkStatus = _bchkStatus;
            strCatName = strHeader.ToUpper();
            lblHeader.Text = "SEARCH ITEM DETAILS";// + strHeader.ToUpper();
            if (strData == "")
                strSearchData = strHeader;
            else
                strSearchData = "Variant" + strCatNo;
            strDesignName = strDesign;
            strCategory1 = strCat1;
            strCategory2 = strCat2;
            strCategory3 = strCat3;
            strCategory4 = strCat4;
            strCategory5 = strCat5;

            if (strDesignName != "")
                strSubQuery += " and ItemName='" + strDesignName + "' ";
            if (strCategory1 != "")
                strSubQuery += " and Variant1='" + strCategory1 + "' ";
            if (strCategory2 != "")
                strSubQuery += " and Variant2='" + strCategory2 + "' ";
            if (strCategory3 != "")
                strSubQuery += " and Variant3='" + strCategory3 + "' ";
            if (strCategory4 != "")
                strSubQuery += " and Variant4='" + strCategory4 + "' ";
            if (strCategory5 != "")
                strSubQuery += " and Variant5='" + strCategory5 + "' ";
            SetKeyInTextBox(objKey);
            GetDataAndBind();
        }

        public SearchCategory_Custom(string strData, string strHeader, string strBrand, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5, Keys objKey, bool bStatus, bool _bchkStatus, string strInvokedFrom)
        {
            InitializeComponent();
            strCatNo = strData;
            boxStatus = bStatus;
            chkStatus = _bchkStatus;
            this.strInvokedFrom = strInvokedFrom;
            strCatName = strHeader.ToUpper();
            lblHeader.Text = "SEARCH ITEM DETAILS";// + strHeader.ToUpper();
            if (strData == "")
                strSearchData = strHeader;
            else
                strSearchData = "Variant" + strCatNo;
            strBrandName = strBrand;
            strDesignName = strDesign;
            strCategory1 = strCat1;
            strCategory2 = strCat2;
            strCategory3 = strCat3;
            strCategory4 = strCat4;
            strCategory5 = strCat5;

            if (strBrand != "")
                strSubQuery += " and BrandName='" + strBrand + "' ";
            if (strDesignName != "")
                strSubQuery += " and ItemName='" + strDesignName + "' ";
            if (strCategory1 != "")
                strSubQuery += " and Variant1='" + strCategory1 + "' ";
            if (strCategory2 != "")
                strSubQuery += " and Variant2='" + strCategory2 + "' ";
            if (strCategory3 != "")
                strSubQuery += " and Variant3='" + strCategory3 + "' ";
            if (strCategory4 != "")
                strSubQuery += " and Variant4='" + strCategory4 + "' ";
            if (strCategory5 != "")
                strSubQuery += " and Variant5='" + strCategory5 + "' ";
            SetKeyInTextBox(objKey);
            GetDataAndBind();
        }

        private void SetKeyInTextBox(Keys objKey)
        {
            if (Keys.Space != objKey && objKey != Keys.F2)
            {
                string strKey = objKey.ToString();
                if (strKey.Contains("NumPad"))
                    strKey = strKey.Replace("NumPad", "");
                if (strKey.Length == 2)
                    strKey = strKey.Replace("D", "");
                txtSearch.Text += strKey;
                txtSearch.SelectionStart = txtSearch.Text.Length;
            }
        }

        private void GetDataAndBind()
        {
            try
            {
                //lbSearchBox.Items.Clear();
                string strAllVariant = "ItemName", strAllVariant_Order = ",ItemName";
                if (MainPage.StrCategory1 != "")
                    strAllVariant += "+'|'+ISNULL(Variant1,'')";
                if (MainPage.StrCategory2 != "")
                    strAllVariant += "+'|'+ISNULL(Variant2,'')";
                if (MainPage.StrCategory3 != "")
                    strAllVariant += "+'|'+ISNULL(Variant3,'')";
                if (MainPage.StrCategory4 != "")
                    strAllVariant += "+'|'+ISNULL(Variant4,'')";
                if (MainPage.StrCategory5 != "")
                    strAllVariant += "+'|'+ISNULL(Variant5,'')";

                if (MainPage.StrCategory1 != "")
                    strAllVariant_Order += "ISNULL(Variant1,'')Variant1";
                if (MainPage.StrCategory2 != "")
                    strAllVariant_Order += "ISNULL(Variant2,'')Variant2";
                if (MainPage.StrCategory3 != "")
                    strAllVariant_Order += "ISNULL(Variant3,'')Variant3";
                if (MainPage.StrCategory4 != "")
                    strAllVariant_Order += "ISNULL(Variant4,'')Variant4";
                if (MainPage.StrCategory5 != "")
                    strAllVariant_Order += "ISNULL(Variant5,'')Variant5";

                if(strAllVariant_Order != "")
                    strAllVariant_Order = strAllVariant_Order.Replace("ISNULL", ",ISNULL");

                if (boxStatus)
                {
                    if (strSearchData == "SONUMBER")
                        strFullOrderNumber = "  (ISNULL(_IS.Description,'')+'|'+(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)+'|'+OB.Items + '|'+ISNULL(OB.Variant1,'')+'|'+ISNULL(OB.Variant2,'')+'|'+CAST((CAST(Quantity as float)-(ISNULL(AdjustedQty,0)+ISNULL(CancelQty,0))) as varchar)) ";
                    else
                        strFullOrderNumber = "(" + strSearchData + "+'|'+ItemName + '|'+Variant1+'|'+Variant2+'|'+CAST((CAST(Quantity as float)-(AdjustedQty+CancelQty)) as varchar)) ";

                    strFullDesignNo = "(ItemName + '|'+Category2+'|'+Category1) ";

                    if (strPartyName != "" && (strSearchData == "SONUMBER" || strSearchData == "PONUMBER"))
                    {
                        string[] strParty = strPartyName.Split(' ');
                        if (strParty.Length > 1)
                            strSubQuery += " and SalePartyID='" + strParty[0] + "' ";
                    }
                }
                else
                {
                    if (strSearchData == "SONUMBER")
                    {
                        strFullOrderNumber = "  (ISNULL(_IS.Description,'')+'|'+(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)+'|'+OB.Items + '|'+ISNULL(OB.Variant1,'')+'|'+ISNULL(OB.Variant2,'')+'|'+CAST((CAST(Quantity as float)-(ISNULL(AdjustedQty,0)+ISNULL(CancelQty,0))) as varchar)) ";
                        if (strPartyName != "" && (strSearchData == "SONUMBER" || strSearchData == "PONUMBER"))
                        {
                            string[] strParty = strPartyName.Split(' ');
                            if (strParty.Length > 1)
                                strSubQuery += " and SalePartyID='" + strParty[0] + "' ";
                        }
                    }
                    else
                        strFullOrderNumber = strFullDesignNo = strSearchData;
                }

                if (strSearchData == "SONUMBER")
                {
                    string strQuery = "";
                    strQuery = " Select  " + strFullOrderNumber + " as  SONUMBER,OB.Date,OB.Items from OrderBooking OB inner join Items _IM on _IM.ItemName=OB.Items left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo  and _IS.Variant1=OB.Variant1 and _IS.Variant2=OB.Variant2 and [ActiveStatus]=1 Where OB.Status='PENDING' and Pieces='LOOSE' " + strSubQuery.Replace("ItemName", "Items") + " Order by OB.Date Desc,OB.Items asc  ";
                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (DSM.Description+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE,DSM.Description BarCode,DM.BrandName" + strAllVariant_Order + " from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 and DM.SubGroupName='PURCHASE' and DSM.Description!='' and [ActiveStatus]=1 " + strSubQuery + " Order By DESIGNNAMEWITHBARCODE ");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALEMERGE")
                {
                    string strQuery = "", strColQuery = "", strOrderBy = "";
                    if (MainPage.StrCategory1 != "")
                    {
                        strColQuery = ",Variant1";                      
                    }
                    if (MainPage.StrCategory2 != "" && MainPage.StrCategory3 != "")
                    {
                        strColQuery += ",Variant2";
                    }
                    if (strInvokedFrom == "BarCode")
                        strOrderBy = " Order by LEN(SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20)),SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20) ";
                    else
                        strOrderBy = " Order by ItemName";
                    /// OLD Concept
                    //strQuery = " Select *,'' SaleRate from ( Select Distinct BarCode,BrandName," + strAllVariant + " as DESIGNNAMEWITHBARCODE_SALEMERGE,SUM(Qty)Qty,ItemName " + strColQuery + " from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN')  Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2 UNION ALL Select Distinct ISNULL(_IS.Description,'')BarCode,_IM.BrandName,BuyerDesignName as DesignName,ItemName,ISNULL(Variant1,'')Variant1,ISNULL(Variant2,'')Variant2,1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and DisStatus=0  UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT')  Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2 )Stock  Group by  BarCode,BrandName,ItemName,ItemName,Variant1,Variant2 having(SUM(Qty)>0) )_Stock " + strOrderBy
                    //         + " ;WITH Purchase AS(Select BarCode,BrandName,ItemName,Variant1,SaleRate,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1 ORDER BY ID ASC) AS RNo from (Select * from (SELECT 0 ID,BarCode,BrandName,ItemName,Variant1,SaleRate,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1 ORDER BY BillNo DESC) AS RNumber FROM PurchaseBookSecondary)PBS Where RNumber=1 UNION ALL Select 1 ID,Description BarCode,BrandName,ItemName,Variant1,SaleRate,1 as RNumber from Items _Im inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo and _IS.ActiveStatus=1 and SaleRate>0)_Purchase) SELECT * FROM Purchase WHERE RNo = 1 ";

                    strQuery = " SELECT BarCode,BrandName," + strAllVariant + " as DESIGNNAMEWITHBARCODE_SALEMERGE,StockQty Qty,ItemName " + strColQuery + ", CAST(SaleRate as Numeric(18,2))SaleRate FROM ItemStock ST WHERE IsWithoutStock = 1 OR StockQty > 0 " + strOrderBy;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 0)
                    {
                        table = ds.Tables[0];
                        //  dtSaleRate = ds.Tables[1];
                    }
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_RETAIL")
                {
                    string strQuery = "", strOrderBy = "", strColQuery = "", strColumnQuery = "";
                    if (MainPage.StrCategory1 != "")
                    {
                        strColQuery = ",Variant1";                    
                        strColumnQuery = "+'|'+Variant1";
                    }
                    if (MainPage.StrCategory2 != "")
                        strColumnQuery += "+'|'+Variant2";

                    if (strInvokedFrom == "BarCode")
                        strOrderBy = " Order by LEN(SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20)),SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20) ";
                    else
                        strOrderBy = " Order by ItemName";
                    /// OLD Concept
                    //strQuery = " Select BCD.BarCode,BrandName,DESIGNNAMEWITHBARCODE_RETAIL,Qty,ItemName " + strColQuery + ",'' as SaleRate from ( Select Distinct BarCode as _BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_RETAIL,SUM(Qty)Qty,ItemName " + strColQuery + " from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN')  Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select Distinct ISNULL(_IS.Description,'')BarCode,_IM.BrandName,BuyerDesignName as DesignName,ItemName,ISNULL(Variant1,'')Variant1,ISNULL(Variant2,'')Variant2,ISNULL(Variant3,'')Variant3,ISNULL(Variant4,'')Variant4,ISNULL(Variant5,'')Variant5,1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and DisStatus=0  UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT')  Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock  Group by  BarCode,BrandName,ItemName,ItemName,Variant1,Variant2 having(SUM(Qty)>0) )_Stock left join BarCodeDetails BCD on _Stock._BarCode=BCD.ParentBarCode WHERE BCD.BarCode is not NULL " + strOrderBy
                    //                + " ;WITH Purchase AS(Select BarCode,BrandName,ItemName,Variant1,SaleRate,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1 ORDER BY ID ASC) AS RNo from (Select * from (SELECT 0 ID,BarCode,BrandName,ItemName,Variant1,SaleRate,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1 ORDER BY BillNo DESC) AS RNumber FROM PurchaseBookSecondary)PBS Where RNumber=1 UNION ALL Select 1 ID,Description BarCode,BrandName,ItemName,Variant1,SaleRate,1 as RNumber from Items _Im inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo and _IS.ActiveStatus=1 and SaleRate>0)_Purchase) SELECT * FROM Purchase WHERE RNo = 1 ";

                    //if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                    //    strQuery = " SELECT BCD.BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_RETAIL ,1 Qty,ItemName" + strColQuery + ", CAST(SaleRate as Numeric(18,2))SaleRate FROM ItemStock ST LEFT JOIN BarcodeDetails BCD On ST.Barcode = BCD.ParentBarCode  WHERE (IsWithoutStock = 1 OR StockQty > 0) AND BCD.BarCode is not null and BCD.BarCode not in (Select BarCode_S from (Select BarCode_S,SUM(Qty)Qty from SalesBookSecondary Group by BarCode_S UNION  ALL Select BarCode_S,-SUM(Qty)Qty from SaleReturnDetails Group by BarCode_S)_Sales Group by BarCode_S having(SUM(Qty)>0)) " + strOrderBy;
                    //else
                    //    strQuery = " SELECT BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_RETAIL,StockQty Qty,ItemName " + strColQuery + ", CAST(SaleRate as Numeric(18,2))SaleRate FROM ItemStock ST WHERE IsWithoutStock = 1 OR StockQty > 0 " + strOrderBy;

                    if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                        strQuery = " SELECT ISNULL(BCD.BarCode,ST.BarCode)BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_RETAIL ,1 Qty,ItemName" + strColQuery + ", CAST(SaleRate as Numeric(18,2))SaleRate FROM ItemStock ST LEFT JOIN (Select ParentBarCode,Barcode,SUM(SetQty) SetQty,ISNULL(InStock,0)InStock from BarcodeDetails Group by ParentBarCode,Barcode,InStock)BCD On ST.Barcode = BCD.ParentBarCode  WHERE (IsWithoutStock = 1 OR (StockQty > 0 AND isnull(BCD.InStock,0) = 1)) " + strOrderBy;
                    else
                        strQuery = " SELECT BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_RETAIL,StockQty Qty,ItemName " + strColQuery + ", CAST(SaleRate as Numeric(18,2))SaleRate FROM ItemStock ST WHERE IsWithoutStock = 1 OR StockQty > 0 " + strOrderBy;


                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 0)
                    {
                        table = ds.Tables[0];
                        //dtSaleRate = ds.Tables[1];
                    }
                }
                else if (strSearchData == "ITEMNAME_PURCHASERETURN_RETAIL")
                {
                    string strQuery = " Select BarCode,BrandName,(DesignName+'|'+" + strAllVariant + ") as ITEMNAME_PURCHASERETURN_RETAIL,(Qty-SaleQty)Qty,Rate as SaleRate,ItemName from ( Select ISNULL(BrandName,'')BrandName,ISNULL(BarCode,'')BarCode,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate,ISNULL((Select SUM(_SM.Qty) from StockMaster _SM Where _SM.BillType in ('SALES','PURCHASERETURN','STOCKOUT') and _SM.BarCode=SM.BarCode and _SM.BrandName=SM.BrandName and _SM.ItemName=SM.ItemName and _SM.DesignName=SM.DesignName and _SM.Variant1=SM.Variant1),0)SaleQty from StockMaster SM Where BillType in ('OPENING','PURCHASE','STOCKIN','SALERETURN') " + strSubQuery + " Group by BrandName,BarCode,DesignName,ItemName,Variant1,Variant2,Variant3,Variant3,Variant4,Variant5,Rate )_Stock Where (Qty-SaleQty)>0 Order by BarCode";
                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BarCode+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)) as DESIGNNAMEWITHBARCODE_SALE from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 having(SUM(Qty)>0))_Stock ");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN")
                {
                    if (strDBName != "")
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN,Qty,Rate as SaleRate,ItemName from ( Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate from ( Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty,Rate,MRP,UnitName from " + strDBName + ".dbo.SalesBookSecondary Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty,Rate,MRP,UnitName from " + strDBName + ".dbo.SaleReturnDetails Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate )_Stock Order by ItemName "); //having(SUM(Qty)>0)
                    else
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN,Qty,Rate as SaleRate,ItemName from ( Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate from ( Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty,Rate,MRP,UnitName from SalesBookSecondary Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty,Rate,MRP,UnitName from SaleReturnDetails Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate )_Stock Order by ItemName "); //having(SUM(Qty)>0)
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN_RETAIL")
                {
                    if (strDBName != "")
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN_RETAIL,UnitName ,Qty,Rate as SaleRate,ItemName from ( Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate,Max(UnitName)UnitName from ( Select ISNULL(BarCode_S,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty,Rate,MRP,UnitName from " + strDBName + ".dbo.SalesBookSecondary Group by BarCode_S,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName UNION ALL Select ISNULL(BarCode_S,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty,Rate,MRP,UnitName from " + strDBName + ".dbo.SaleReturnDetails Group by BarCode_S,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate having(SUM(Qty)>0))_Stock Order by BarCode");
                    else
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN_RETAIL,UnitName ,Qty,Rate as SaleRate,ItemName from ( Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate,Max(UnitName)UnitName from ( Select ISNULL(BarCode_S,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty,Rate,MRP,UnitName from SalesBookSecondary Group by BarCode_S,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName UNION ALL Select ISNULL(BarCode_S,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty,Rate,MRP,UnitName from SaleReturnDetails Group by BarCode_S,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate having(SUM(Qty)>0))_Stock Order by BarCode");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN_MANUAL")
                {
                    if (strDBName != "")
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName + '|' + " + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN_MANUAL,UnitName,Qty,SaleRate,ItemName from (Select BRD.BarCode as BarCode, BrandName, BuyerDesignName as DesignName, ItemName, UnitName, Variant1, Variant2, Variant3, Variant4, Variant5, 1 as Qty, SaleRate from " + strDBName + ".dbo.Items IM inner join " + strDBName + ".dbo.ItemSecondary IMS on im.BillCode = Ims.BillCode and IM.BillNo = IMS.BillNo LEFT JOIN " + strDBName + ".dbo.BarcodeDetails BRD on IMS.Description = BRD.ParentBarCode )_Stock Order by BarCode ");
                    else
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BarCode,BrandName,(DesignName + '|' + " + strAllVariant + ") as DESIGNNAMEWITHBARCODE_SALERETURN_MANUAL,UnitName,Qty,SaleRate,ItemName from (Select BRD.BarCode as BarCode, BrandName, BuyerDesignName as DesignName, ItemName, UnitName, Variant1, Variant2, Variant3, Variant4, Variant5, 1 as Qty, SaleRate from Items IM inner join ItemSecondary IMS on im.BillCode = Ims.BillCode and IM.BillNo = IMS.BillNo LEFT JOIN BarcodeDetails BRD on IMS.Description = BRD.ParentBarCode )_Stock Order by BarCode ");
                }
                else if (strSearchData == "ITEM_NAME_ST")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BarCode+'|'+BrandName+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)) as ITEM_NAME_ST from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 having(SUM(Qty)>0))_Stock ");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_AUDITSTOCK")
                {
                    string strQuery = "", strOrderBy = "", strColQuery = "", strColumnQuery = "";
                    if (MainPage.StrCategory1 != "")
                    {
                        strColQuery = ",Variant1";
                        strColumnQuery = "+'|'+Variant1";
                    }
                    if (MainPage.StrCategory2 != "")
                        strColumnQuery += "+'|'+Variant2";

                    if (strInvokedFrom == "BarCode")
                        strOrderBy = " Order by LEN(SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20)),SUBSTRING(ST.BarCode, CHARINDEX('-', ST.BarCode)+1,20) ";
                    else
                        strOrderBy = " Order by ItemName";

                    if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                        strQuery = " SELECT BCD.BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_AUDITSTOCK ,1 Qty,ItemName" + strColQuery + ",'' SaleRate FROM ItemStock ST LEFT JOIN (Select ParentBarCode,Barcode,SUM(SetQty) SetQty,ISNULL(InStock,0)InStock from BarcodeDetails Group by ParentBarCode,Barcode,InStock)BCD On ST.Barcode = BCD.ParentBarCode  WHERE BCD.BarCode is not null " + strOrderBy;
                    else
                        strQuery = " SELECT BarCode,BrandName,ItemName" + strColumnQuery + " as DESIGNNAMEWITHBARCODE_AUDITSTOCK,StockQty Qty,ItemName " + strColQuery + ",'' SaleRate FROM ItemStock ST " + strOrderBy;

                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else if(strSearchData == "SALEITEMS_RESTO")
                {
                    string strQuery = "SELECT OrderNo,Waiter,TableNo, Item SALEITEMS_RESTO,Description,Qty,Unit,Rate,Amount FROM Res_SalesStock WHERE Status != 'BILLED' " + ((strBrandName != "")? " And TableNo = '"+ strBrandName+"'" : "") + " Order by Item";
                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else
                {
                    if (chkStatus)
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct " + strSearchData + " from StockMaster Where  " + strSearchData + " !='' " + strSubQuery + "  Group By " + strSearchData + " having (Sum(StockQuantity))>0 Order by " + strSearchData + " ");
                    else if (strSubQuery != "")
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct " + strSearchData + " from DesignMaster DM inner join DesignSecondaryMaster DSM on DM.SerialNo=DSM.SerialNo Where " + strSearchData + " !='' " + strSubQuery + " Order By " + strSearchData);
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select " + strSearchData + " from VariantMaster" + strCatNo + " Where " + strSearchData + " !='' Order by " + strSearchData);
                }
                BindData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void BindData(DataTable table = null)
        {
            dgrdDetails.DataSource = null;
            if (table != null)
            {
                DataView dataView = new DataView(table);
                dgrdDetails.DataSource = dataView;
                SetColumnStyle();
            }
            else
            {
                DataView dataView = new DataView(this.table);
                dgrdDetails.DataSource = dataView;
                SetColumnStyle();
            }
            if (dgrdDetails.Rows.Count > 0)
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
            //if(strSearchData == "SALEITEMS_RESTO")
            //{
            //    dgrdDetails.CurrentRow.Selected = false;
            //    dgrdDetails.CurrentCell = null;
            //}
        }
        private void SetColumnStyle()
        {
            try
            {
                int ic = 0;
                for (int i = 0; i < dgrdDetails.Columns.Count; i++)
                {
                    DataGridViewCellStyle cellStyle = dgrdDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdDetails.Columns[i];

                    string strAlign = "LEFT", clmName = _column.Name.ToUpper();
                    int _width = 100;
                    _column.Width = _width;

                    _column.SortMode = DataGridViewColumnSortMode.Automatic;

                    if (!strSearchData.Contains("DESIGNNAMEWITHBARCODE") || strSearchData== "DESIGNNAMEWITHBARCODE_RETAIL")
                    {
                        if (clmName == "ITEMNAME" || clmName == "VARIANT1" || clmName == "VARIANT2" || clmName == "VARIANT3" || clmName == "VARIANT4" || clmName == "VARIANT5")
                            _column.Visible = false;
                    }

                    //if (clmName == "BARCODE" && ic > 0)
                    //    _column.Visible = false;

                    //if (clmName.Contains("BARCODE"))
                    //{
                    //    _width = 150;
                    //    ic = 1;
                    //}

                    if (clmName.Contains("BRAND"))
                        _width = 180;
                    if (clmName.Contains("RATE"))
                    {
                        _width = 100;
                        strAlign = "RIGHT";
                    }
                    if (strSearchData == "ITEMNAME_PURCHASERETURN_RETAIL" && clmName.Contains("RATE"))
                        _column.HeaderText = "RATE";

                    if (clmName.Contains("VARIANT1"))
                        _column.HeaderText = MainPage.StrCategory1;
                    if (clmName.Contains("VARIANT2"))
                        _column.HeaderText = MainPage.StrCategory2;
                    if (clmName.Contains("VARIANT3"))
                        _column.HeaderText = MainPage.StrCategory3;
                    if (clmName.Contains("VARIANT4"))
                        _column.HeaderText = MainPage.StrCategory4;
                    if (clmName.Contains("VARIANT5"))
                        _column.HeaderText = MainPage.StrCategory5;

                    if (clmName.Contains(strSearchData.ToUpper()))
                    {
                        _width = 280;
                        if (strSearchData == "SALEITEMS_RESTO")
                            _width = 120;
                        _column.HeaderText = "Item";
                        //else
                        //{
                        //    _column.HeaderText = "Description";
                          //  _width = 120;
                        //}
                    }
                    if (clmName.Contains("QTY"))
                    {
                        _width = 80;
                        strAlign = "RIGHT";
                    }

                    cellStyle.Font = new Font("Arial", 8F, System.Drawing.FontStyle.Regular);

                    if (strAlign == "LEFT")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (strAlign == "MIDDLE")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgrdDetails.Columns[i].DefaultCellStyle = cellStyle;
                    dgrdDetails.Columns[i].HeaderText = (dgrdDetails.Columns[i].HeaderText).Replace("_", " ");
                    dgrdDetails.Columns[i].HeaderCell.Style.Font = new Font("Arial", 8.5F, System.Drawing.FontStyle.Bold);
                    dgrdDetails.Columns[i].Width = _width;
                }
            }
            catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            try
            {
                if (table != null)
                {
                    string strLikeKey = "%";
                    if (txtSearch.Text == "")
                    {
                        BindData();
                    }
                    else
                    {
                        DataRow[] rows = null;
                        if (strSearchData == "DESIGNNAME")
                            rows = table.Select(String.Format(strSearchData + " Like('" + strLikeKey + txtSearch.Text + "%') OR OtherDetails Like('" + strLikeKey + txtSearch.Text + "%') "));
                        else if (strSearchData == "ORDERDESIGNNAME" && txtSearch.Text.Length > 5)
                            rows = table.Select(String.Format(strSearchData + " Like('" + txtSearch.Text + "%') OR OtherDetails Like('%" + txtSearch.Text + "%') "));
                        else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALEMERGE" || strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN" || strSearchData == "ITEMNAME_PURCHASERETURN_RETAIL" || strSearchData == "DESIGNNAMEWITHBARCODE_AUDITSTOCK")
                        {
                            string strSearchKey = "";
                            if (strInvokedFrom.ToUpper() == "BARCODE")
                                strSearchKey = " BarCode Like('%" + txtSearch.Text + "%') ";
                            else
                                strSearchKey = strSearchData + " Like('" + strLikeKey + txtSearch.Text + "%') OR BarCode Like('%" + txtSearch.Text + "%')";

                            rows = table.Select(String.Format(strSearchKey + " OR BrandName Like('%" + txtSearch.Text + "%') "));
                        }
                        else if (strSearchData == "SALEITEMS_RESTO")
                        {// OrderNo,Waiter,
                            rows = table.Select(String.Format(strSearchData + " Like('%" + txtSearch.Text + "%') OR OrderNo Like('%" + txtSearch.Text + "%') OR TableNo Like('%" + txtSearch.Text + "%') "));
                        }
                        else
                        {
                            string strSearchKey = "";
                            if (strInvokedFrom.ToUpper() == "BARCODE")
                                strSearchKey = "BarCode Like('%" + txtSearch.Text + "%') OR BarCode LIKE ('%" + txtSearch.Text + "%')";
                            else
                                strSearchKey = strSearchData + " Like('" + strLikeKey + txtSearch.Text + "%') OR BarCode Like('%" + txtSearch.Text + "%')";

                            rows = table.Select(String.Format(strSearchKey + " OR BrandName Like('%" + txtSearch.Text + "%') "));
                        }

                        dgrdDetails.DataSource = null;
                        if (rows.Length > 0)
                        {
                            DataTable __dt = rows.CopyToDataTable();
                            if (strSearchData == "DESIGNNAMEWITHBARCODE_SALEMERGE" || strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN")
                            {
                                if (strInvokedFrom.ToUpper() != "BARCODE")
                                {
                                    DataView dv = __dt.DefaultView;
                                    dv.Sort = "ItemName";
                                    __dt = dv.ToTable();
                                }
                            }
                            BindData(__dt);
                        }
                    }
                }
                else
                {
                    GetDataAndBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgrdDetails_SelectionChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (strSearchData == "DESIGNNAMEWITHBARCODE_RETAIL" || strSearchData == "DESIGNNAMEWITHBARCODE_SALEMERGE")
            //    {
            //        DataGridViewRow dr = dgrdDetails.CurrentRow;
            //        if (dr != null)
            //        {
            //            if (Convert.ToString(dr.Cells["SaleRate"].Value) == "")
            //            {
            //                string strAllVariant = "ItemName";
            //                if (MainPage.StrCategory1 != "")
            //                    strAllVariant += "+'|'+ISNULL(Variant1,'')";
            //                if (MainPage.StrCategory2 != "")
            //                    strAllVariant += "+'|'+ISNULL(Variant2,'')";

            //                string selBarcode = Convert.ToString(dr.Cells["BarCode"].Value);
            //                string selBrandName = Convert.ToString(dr.Cells["BrandName"].Value);
            //                string selDesc = Convert.ToString(dr.Cells[strSearchData].Value);
            //                string selSaleRate = Convert.ToString(dr.Cells["SaleRate"].Value);
            //                string selQty = Convert.ToString(dr.Cells["Qty"].Value);

            //                strSelectedData = selBarcode + "|" + selBrandName + "|" + selDesc + "|" + selSaleRate + "|" + selQty;

            //                DataRow[] row = dtSaleRate.Select("BarCode='" + selBarcode + "' and BrandName ='" + selBrandName + "' and  " + strAllVariant + "= '" + selDesc + "' ");
            //                if (row.Length > 0)
            //                {
            //                    dr.Cells["SaleRate"].Value = Convert.ToString(row[0]["SaleRate"]);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch { }
        }

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow dr = dgrdDetails.Rows[e.RowIndex];
                if (dr != null)
                {
                    if (strSearchData == "SALEITEMS_RESTO")
                    {
                        string orderno = "", waiter = "", tableNo = "", item = "", Desc = "", qty = "", rate = "", unit = "", amount = "";
                        foreach (DataGridViewRow row in dgrdDetails.Rows)
                        {
                            orderno = Convert.ToString(row.Cells["OrderNo"].Value);
                            waiter = Convert.ToString(row.Cells["Waiter"].Value);
                            tableNo = Convert.ToString(row.Cells["TableNo"].Value);
                            item = Convert.ToString(row.Cells[strSearchData].Value);
                            Desc = Convert.ToString(row.Cells["Description"].Value);
                            qty = Convert.ToString(row.Cells["Qty"].Value);
                            rate = Convert.ToString(row.Cells["Rate"].Value);
                            unit = Convert.ToString(row.Cells["Unit"].Value);
                            amount = Convert.ToString(row.Cells["Amount"].Value);
                           
                            strSelectedRows.Add(orderno + "|" + waiter + "|" + tableNo + "|" + item + "|" + Desc + "|" + qty + "|" + rate + "|" + unit + "|" + amount);
                        }
                        closeStatus = true;
                    }
                    else if(strSearchData == "DESIGNNAMEWITHBARCODE")
                    {
                        strSelectedData = Convert.ToString(dr.Cells[strSearchData].Value);
                        closeStatus = true;
                    }
                    else
                    {
                        string selBarcode = Convert.ToString(dr.Cells["BarCode"].Value);
                        string selBrandName = Convert.ToString(dr.Cells["BrandName"].Value);
                        string selDesc = Convert.ToString(dr.Cells[strSearchData].Value);
                        string selSaleRate = Convert.ToString(dr.Cells["SaleRate"].Value);
                        string selQty = Convert.ToString(dr.Cells["Qty"].Value);
                        string UnitName = "";
                        if (dgrdDetails.Columns.Contains("UnitName"))
                            UnitName = Convert.ToString(dr.Cells["UnitName"].Value);

                        strSelectedData = selBarcode + "|" + selBrandName + "|" + selDesc + "|" + UnitName + "|" + selSaleRate + "|" + selQty ;
                        closeStatus = true;
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DataGridViewRow dr = dgrdDetails.CurrentRow;
                    if (dr != null)
                    {
                        if (strSearchData == "SALEITEMS_RESTO")
                        {
                            string orderno = "", waiter = "", tableNo = "", item = "", Desc = "", qty = "", rate = "", unit = "", amount = "";
                            foreach (DataGridViewRow row in dgrdDetails.Rows)
                            {
                                orderno = Convert.ToString(row.Cells["OrderNo"].Value);
                                waiter = Convert.ToString(row.Cells["Waiter"].Value);
                                tableNo = Convert.ToString(row.Cells["TableNo"].Value);
                                item = Convert.ToString(row.Cells[strSearchData].Value);
                                Desc = Convert.ToString(row.Cells["Description"].Value);
                                qty = Convert.ToString(row.Cells["Qty"].Value);
                                rate = Convert.ToString(row.Cells["Rate"].Value);
                                unit = Convert.ToString(row.Cells["Unit"].Value);
                                amount = Convert.ToString(row.Cells["Amount"].Value);
                                strSelectedRows.Add(orderno + "|" + waiter + "|" + tableNo + "|" + item + "|" + Desc + "|" + qty + "|" + rate + "|" + unit + "|" + amount);
                            }
                            closeStatus = true;
                        }
                        else if (strSearchData == "DESIGNNAMEWITHBARCODE")
                        {
                            strSelectedData = Convert.ToString(dr.Cells[strSearchData].Value);
                            closeStatus = true;
                        }
                        else
                        {
                            string selBarcode = Convert.ToString(dr.Cells["BarCode"].Value);
                            string selBrandName = Convert.ToString(dr.Cells["BrandName"].Value);
                            string selDesc = Convert.ToString(dr.Cells[strSearchData].Value);
                            string selSaleRate = Convert.ToString(dr.Cells["SaleRate"].Value);
                            string selQty = Convert.ToString(dr.Cells["Qty"].Value);
                            string UnitName = "";
                            if (dgrdDetails.Columns.Contains("UnitName"))
                                UnitName = Convert.ToString(dr.Cells["UnitName"].Value);

                            strSelectedData = selBarcode + "|" + selBrandName + "|" + selDesc + "|" + UnitName + "|" + selSaleRate + "|" + selQty;
                            closeStatus = true;
                        }
                        this.Close();
                    }
                }
                else if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
                {
                    char pressedCharacter = (char)e.KeyValue;
                    if (Char.IsLetter(pressedCharacter) || Char.IsNumber(pressedCharacter))
                    {
                        SetKeyInTextBox(e.KeyCode);
                    }
                    txtSearch.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DataGridViewRow dr = dgrdDetails.CurrentRow;
                    if (dr != null)
                    {
                        if (strSearchData == "SALEITEMS_RESTO")
                        {
                            string orderno="",waiter = "", tableNo = "", item = "", Desc = "", qty = "", rate = "", unit = "", amount = "";
                            foreach (DataGridViewRow row in dgrdDetails.Rows)
                            {
                                orderno = Convert.ToString(row.Cells["OrderNo"].Value);
                                waiter = Convert.ToString(row.Cells["Waiter"].Value);
                                tableNo = Convert.ToString(row.Cells["TableNo"].Value);
                                item = Convert.ToString(row.Cells[strSearchData].Value);
                                Desc = Convert.ToString(row.Cells["Description"].Value);
                                qty = Convert.ToString(row.Cells["Qty"].Value);
                                rate = Convert.ToString(row.Cells["Rate"].Value);
                                unit = Convert.ToString(row.Cells["Unit"].Value);
                                amount = Convert.ToString(row.Cells["Amount"].Value);
                                strSelectedRows.Add(orderno + "|" + waiter + "|" + tableNo + "|" + item + "|" + Desc + "|" + qty + "|" + rate + "|" + unit + "|" + amount);
                            }
                            closeStatus = true;
                        }
                        else if (strSearchData == "DESIGNNAMEWITHBARCODE")
                        {
                            strSelectedData = Convert.ToString(dr.Cells[strSearchData].Value);
                            closeStatus = true;
                        }
                        else
                        {
                            string selBarcode = Convert.ToString(dr.Cells["BarCode"].Value);
                            string selBrandName = Convert.ToString(dr.Cells["BrandName"].Value);
                            string selDesc = Convert.ToString(dr.Cells[strSearchData].Value);
                            string selSaleRate = Convert.ToString(dr.Cells["SaleRate"].Value);
                            string selQty = Convert.ToString(dr.Cells["Qty"].Value);
                            string UnitName = "";
                            if (dgrdDetails.Columns.Contains("UnitName"))
                                UnitName = Convert.ToString(dr.Cells["UnitName"].Value);

                            strSelectedData = selBarcode + "|" + selBrandName + "|" + selDesc + "|" + UnitName + "|" + selSaleRate + "|" + selQty;
                            closeStatus = true;
                        }
                        this.Close();
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    if (dgrdDetails.CurrentRow != null)
                    {
                        int _index = dgrdDetails.CurrentRow.Index;
                        if (_index > 0)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[_index - 1].Cells[1];
                        }
                    }
                    else
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[1];
                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (dgrdDetails.CurrentRow != null)
                    {
                        int _index = dgrdDetails.CurrentRow.Index;
                        if (_index < dgrdDetails.Rows.Count - 1)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[_index + 1].Cells[1];
                        }
                    }
                    else
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells[1];
                    dgrdDetails.Focus();
                }

            }
            catch
            {
            }
        }

        private void SearchData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                strSelectedData = "";
                closeStatus = false;
                dgrdDetails.DataSource = null;
                this.Close();
            }

        }

        private string GetNewTypedText
        {
            get
            {
                if (!txtSearch.Text.Contains("ADD") && !txtSearch.Text.Contains("NEW"))
                    return txtSearch.Text.Trim();
                else
                    return "";
            }
        }


        private void SearchData_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string strText = GetNewTypedText;
                if (strSONumber == "" && strPONumber == "")
                {
                    if (strSelectedData == "ADD NEW DESIGNNAME NAME" || strSelectedData == "ADD NEW ORDERDESIGNNAME NAME")
                    {
                        DesignMaster objDesignMaster = new DesignMaster(true);
                        objDesignMaster.txtItemName.Text = strText;
                        objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objDesignMaster.ShowDialog();
                        strSelectedData = objDesignMaster.StrAddedDesignName;
                        if (strSelectedData == "")
                            e.Cancel = true;
                    }
                    else if (strSelectedData == "ADD NEW " + strCatName + " NAME")
                    {
                        VariantMaster objVariantMaster = new VariantMaster(strCatNo, strCatName, true, strText);
                        objVariantMaster.ShowDialog();
                        strSelectedData = objVariantMaster.StrAddedCategory;
                        if (strSelectedData == "")
                            e.Cancel = true;
                    }
                }
                if (!closeStatus)
                    dgrdDetails.DataSource = null;
            }
            catch
            {
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSearch.Text.Length == 0)
            {
                if (Char.IsWhiteSpace(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private DataTable GetMonthDataTable(string strName)
        {
            DataTable dt = new DataTable();
            if (strName == "MONTH")
            {
                dt.Columns.Add("Month", typeof(String));
                string[] strMonths = { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
                foreach (string strMonth in strMonths)
                {
                    DataRow row = dt.NewRow();
                    row["Month"] = strMonth;
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        private void SearchCategory_Load(object sender, EventArgs e)
        {
            //GetDataAndBind();
        }
        private void dgrdDetails_Scroll(object sender, ScrollEventArgs e)
        {
            //try
            //{
            //    if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            //    {
            //        int rowCount = dgrdDetails.Rows.Count;
            //        if (e.NewValue > rowCount - 15)
            //        {
            //            DataTable temp = table.Clone();
            //            foreach (DataRow dr in table.Rows)
            //            {
            //                temp.ImportRow(dr);
            //            }
            //            MinIndex = MaxIndex + 1;
            //            MaxIndex = MaxIndex + 30;
            //            GetDataAndBind(true);
            //            if (table.Rows.Count > 0)
            //            {
            //                foreach (DataRow dr in temp.Rows)
            //                {
            //                    table.ImportRow(dr);
            //                }
            //                BindData();
            //                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 16].Cells[2];
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex) { }
        }

    }
}
