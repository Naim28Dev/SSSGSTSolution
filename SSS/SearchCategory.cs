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
    public partial class SearchCategory : Form
    {
        public string strSearchData = "", strSelectedData = "", strSubQuery = "", strOrderStatus = "",strCompanyCode="";
        DataTable table = null;
        bool chkStatus = false, boxStatus = false,closeStatus=false;
       public string strFullOrderNumber = "", strFullDesignNo = "",strPartyName="",strCartonStatus="";
        string strCatNo = "1", strCatName = "", strPONumber = "", strSONumber = "", strDesignName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";
         
        public SearchCategory(string strNo, string strName, Keys objKey)
        {
            InitializeComponent();
            strCatNo = strNo;
            strCatName = strName.ToUpper() ;
            lblHeader.Text = "Search "+strName+" Name";
            strSearchData = "Variant" + strNo;
            SetKeyInTextBox(objKey);
            GetDataAndBind();
        }

        public SearchCategory(string strData, string strHeader, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5,Keys objKey,bool bStatus, string strCCode)
        {
            InitializeComponent();
            strCatNo = strData;
            boxStatus = bStatus;
            strCatName = strHeader.ToUpper();
            lblHeader.Text = "SEARCH " + strHeader.ToUpper();
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
            strCompanyCode = strCCode;

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

        public SearchCategory(string strHeader, string strData, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5,string strPName, Keys objKey, bool bStatus)
        {
            InitializeComponent();
            strCatNo = strData;
            boxStatus = bStatus;
            strPartyName = strPName;        
            strSearchData = strCatName= strData;

            lblHeader.Text = strHeader;       
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

        public SearchCategory(string strData, string strHeader, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5, Keys objKey, bool bStatus,bool _bchkStatus)
        {
            InitializeComponent();
            strCatNo = strData;
            boxStatus = bStatus;
            chkStatus = _bchkStatus;
            strCatName = strHeader.ToUpper();
            lblHeader.Text = "SEARCH " + strHeader.ToUpper();
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

        private void SetKeyInTextBox(Keys objKey)
        {
            if (Keys.Space != objKey && objKey!=Keys.F2)
            {
                string strKey = objKey.ToString();
                if (strKey.Contains("NumPad"))
                    strKey = strKey.Replace("NumPad", "");
                if (strKey.Length == 2)
                    strKey = strKey.Replace("D", "");
                txtSearch.Text = strKey;
                txtSearch.SelectionStart = 1;
            }
        }

        private void GetDataAndBind()
        {
            try
            {
                lbSearchBox.Items.Clear();

                string strAllVariant = "ItemName",strAllVariant_Order = "ItemName";
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

                if (MainPage.StrCategory2 != "")
                    strAllVariant_Order += "+'|'+ISNULL(Variant2,'')";
                if (MainPage.StrCategory1 != "")
                    strAllVariant_Order += "+'|'+ISNULL(Variant1,'')";              
                if (MainPage.StrCategory3 != "")
                    strAllVariant_Order += "+'|'+ISNULL(Variant3,'')";
                if (MainPage.StrCategory4 != "")
                    strAllVariant_Order += "+'|'+ISNULL(Variant4,'')";
                if (MainPage.StrCategory5 != "")
                    strAllVariant_Order += "+'|'+ISNULL(Variant5,'')";


                if (boxStatus)
                {
                    if (strSearchData == "SONUMBER")
                    {
                        string strOrderVariant = "";
                        if (MainPage.StrCategory1 != "")
                            strOrderVariant += "+'|'+ISNULL(OB.Variant1,'')";
                        if (MainPage.StrCategory2 != "")
                            strOrderVariant += "+'|'+ISNULL(OB.Variant2,'')";
                        strFullOrderNumber = "  (ISNULL(_IS.Description,'')+'|'+(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)+'|'+OB.Items"+ strOrderVariant +"+ '|'+CAST((CAST(Quantity as float)-(ISNULL(AdjustedQty,0)+ISNULL(CancelQty,0))) as varchar)) ";
                    }
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
                        //strFullOrderNumber = "  ((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)) ";
                        strFullOrderNumber = "  (ISNULL(_IS.Description,'')+'|'+(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)+'|'+OB.Items + '|'+ISNULL(OB.Variant1,'')+'|'+ISNULL(OB.Variant2,'')+'|'+CAST((CAST(Quantity as float)-(ISNULL(AdjustedQty,0)+ISNULL(CancelQty,0))) as varchar)+'|'+(AreaCode+AccountNo+' '+Name)) ";
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
                    // strQuery = "Select Distinct " + strFullOrderNumber + " as  SONUMBER,SO.SONumber as SNumber from from OrderBooking OB Where OB.Status='PENDING  " +strSubQuery + " Order by OB.OrderNo ";
                    strQuery = " Select  " + strFullOrderNumber + " as  SONUMBER,OB.Date,OB.Items from OrderBooking OB left join SupplierMaster SM on SM.AreaCode+AccountNo=OB.PurchasePartyID  left join Items _IM on _IM.ItemName=OB.Items left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo  and _IS.Variant1=OB.Variant1 and _IS.Variant2=OB.Variant2 and [ActiveStatus]=1 Where OB.Status='PENDING' and Pieces='LOOSE' " + strSubQuery.Replace("ItemName", "Items") + " Order by OB.Date Desc,OB.Items asc  ";
                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else if (strSearchData == "DESIGNNAME")
                {
                    if (chkStatus)
                    {
                        //"Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST(MRP as numeric(18,2)) as varchar)+'|'+CAST(Disc as varchar)+'|'+CAST(CAST((MRP*(100+Disc)/100) as numeric(18,2)) as varchar))DESIGNNAME,'' as OtherDetails  from (Select (ItemName+'|'+ISNULL(Variant1,'')+'|'+ISNULL(Variant2,'')+'|'+ISNULL(Variant3,'')+'|'+ISNULL(Variant4,'')+'|'+ISNULL(Variant5,'')) as DESIGNNAME,(Qty+ISNULL(_Qty,0)) Qty,ISNULL(MRP,0)MRP,(CAST(GodownName as money)+(CASE WHEN Category='RETAIL PURCHASE' Then 0 WHEN (Category='CASH PURCHASE' OR PartyType='CASH PURCHASE') Then 5 else 3 end)) Disc,ISNULL(_Qty,0)_Qty from StockMaster STM OUTER APPLY (Select -SUM(Qty) _Qty from StockMaster _STM Where STM.ItemName=_STM.ItemName and STM.Variant1=_STM.Variant1 and STM.Variant2=_STM.Variant2 and STM.Variant3=_STM.Variant3 and STM.Variant4=_STM.Variant4 and STM.Variant5=_STM.Variant5 and ISNULL(STM.MRP,0)=ISNULL(_STM.MRP,0) and _STM.BillType in ('SALES','PURCHASERETURN')) _STM OUTER APPLY (Select Top 1 Category,TINNumber as PartyType from SupplierMaster Where GroupName='Sundry Creditor' and Name=Variant4)SM Where ItemName !='' and BillType in ('OPENING','PURCHASE','SALERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1,''),ISNULL(Variant2,''),ISNULL(Variant3,''),ISNULL(Variant4,''),ISNULL(Variant5,''),Qty,_Qty,MRP,GodownName,Category,PartyType  )_Sales  Group by DESIGNNAME,MRP,Disc having(SUM(Qty)>0) Order by DESIGNNAME "
                        string strQuery = " Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST((MRP) as numeric(18,2)) as varchar)+'|'+CAST(MAX(Disc) as varchar)+'|'+CAST(CAST((MAX(MRP)*(100+MAX(Disc))/100) as numeric(18,2)) as varchar))DESIGNNAME,'' as OtherDetails  from ( "
                                       + " Select (ItemName + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME, SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,(CAST(GodownName as money) + (CASE WHEN Category = 'RETAIL PURCHASE' Then 0 WHEN(Category = 'CASH PURCHASE' OR PartyType = 'CASH PURCHASE') then 5 else 3 end)- (CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)) Disc from StockMaster STM OUTER APPLY (Select Top 1 Category, TINNumber as PartyType from SupplierMaster Where GroupName = 'SUNDRY CREDITOR' and Name = Variant4)SM Where ItemName != '' and BillType in ('OPENING','PURCHASE','SALERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP,GodownName,Category,PartyType,BillCode UNION ALL "
                                       + " Select (ItemName + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME,-SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,(CAST(_SM2._Disc as money) + (CASE WHEN Category = 'RETAIL PURCHASE' Then 0 WHEN(Category = 'CASH PURCHASE' OR PartyType = 'CASH PURCHASE') then 5 else 3 end)- (CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)) Disc from StockMaster STM OUTER APPLY (Select Top 1 Category, TINNumber as PartyType from SupplierMaster Where GroupName = 'SUNDRY CREDITOR' and Name = Variant4)SM OUTER APPLY (Select Top 1 _SM2.GodownName as _Disc from StockMaster _SM2 Where  _SM2.BillType in ('OPENING','PURCHASE','SALERETURN') and _SM2.ItemName=STM.ItemName and _SM2.Variant1=STM.Variant1 and _SM2.Variant2=STM.Variant2 and _SM2.Variant3=STM.Variant3 and _SM2.Variant4=STM.Variant4 and _SM2.Variant5=STM.Variant5)_SM2  Where ItemName != '' and BillType in ('SALES','PURCHASERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP,_SM2._Disc,Category,PartyType,BillCode "
                                       + " )_Sales Group by DESIGNNAME,MRP having(SUM(Qty) > 0) Order by DESIGNNAME ";

                        table = DataBaseAccess.GetDataTableRecord(strQuery);
                    }
                    else if (boxStatus)
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct (" + strAllVariant + ") as DESIGNNAME,DSM.Description as OtherDetails,Variant2,Variant1 from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 " + strSubQuery + " Order By DesignName,Variant2,Variant1");
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct ItemName as DESIGNNAME,'' as OtherDetails from Items DM left join ItemSecondary DSM on DM.BillNo=DSM.BillNo Where DM.ItemName!='' " + strSubQuery + " Order By ItemName ");
                }
                else if (strSearchData == "ORDERDESIGNNAME")
                {
                    if (chkStatus)
                    {
                        //"Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST(MRP as numeric(18,2)) as varchar)+'|'+CAST(Disc as varchar)+'|'+CAST(CAST((MRP*(100+Disc)/100) as numeric(18,2)) as varchar))DESIGNNAME,'' as OtherDetails  from (Select (ItemName+'|'+ISNULL(Variant1,'')+'|'+ISNULL(Variant2,'')+'|'+ISNULL(Variant3,'')+'|'+ISNULL(Variant4,'')+'|'+ISNULL(Variant5,'')) as DESIGNNAME,(Qty+ISNULL(_Qty,0)) Qty,ISNULL(MRP,0)MRP,(CAST(GodownName as money)+(CASE WHEN Category='RETAIL PURCHASE' Then 0 WHEN (Category='CASH PURCHASE' OR PartyType='CASH PURCHASE') Then 5 else 3 end)) Disc,ISNULL(_Qty,0)_Qty from StockMaster STM OUTER APPLY (Select -SUM(Qty) _Qty from StockMaster _STM Where STM.ItemName=_STM.ItemName and STM.Variant1=_STM.Variant1 and STM.Variant2=_STM.Variant2 and STM.Variant3=_STM.Variant3 and STM.Variant4=_STM.Variant4 and STM.Variant5=_STM.Variant5 and ISNULL(STM.MRP,0)=ISNULL(_STM.MRP,0) and _STM.BillType in ('SALES','PURCHASERETURN')) _STM OUTER APPLY (Select Top 1 Category,TINNumber as PartyType from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Name=Variant4)SM Where ItemName !='' and BillType in ('OPENING','PURCHASE','SALERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1,''),ISNULL(Variant2,''),ISNULL(Variant3,''),ISNULL(Variant4,''),ISNULL(Variant5,''),Qty,_Qty,MRP,GodownName,Category,PartyType  )_Sales  Group by DESIGNNAME,MRP,Disc having(SUM(Qty)>0) Order by DESIGNNAME "
                        //string strQuery = " Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST(MRP as numeric(18,2)) as varchar)+'|'+CAST(MAX(Disc) as varchar)+'|'+CAST(CAST((MRP*(100+MAX(Disc))/100) as numeric(18,2)) as varchar))ORDERDESIGNNAME,'' as OtherDetails  from ( "
                        //               + " Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME, SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,(CAST(GodownName as money) + (CASE WHEN Category = 'RETAIL PURCHASE' Then 0 WHEN(Category = 'CASH PURCHASE' OR PartyType = 'CASH PURCHASE') then 5 else 3 end)- (CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)) Disc from StockMaster STM OUTER APPLY (Select Top 1 Category, TINNumber as PartyType from SupplierMaster Where GroupName = 'SUNDRY CREDITOR' and Name = Variant4)SM Where ItemName != '' and BillType in ('OPENING','PURCHASE','SALERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP,GodownName,Category,PartyType,BillCode UNION ALL "
                        //               + " Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME,-SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,(CAST(_SM2._Disc as money) + (CASE WHEN Category = 'RETAIL PURCHASE' Then 0 WHEN(Category = 'CASH PURCHASE' OR PartyType = 'CASH PURCHASE') then 5 else 3 end)- (CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)) Disc from StockMaster STM OUTER APPLY (Select Top 1 Category, TINNumber as PartyType from SupplierMaster Where GroupName = 'SUNDRY CREDITOR' and Name = Variant4)SM OUTER APPLY (Select Top 1 _SM2.GodownName as _Disc from StockMaster _SM2 Where  _SM2.BillType in ('OPENING','PURCHASE','SALERETURN') and _SM2.ItemName=STM.ItemName and _SM2.Variant1=STM.Variant1 and _SM2.Variant2=STM.Variant2 and _SM2.Variant3=STM.Variant3 and _SM2.Variant4=STM.Variant4 and _SM2.Variant5=STM.Variant5)_SM2  Where ItemName != '' and BillType in ('SALES','PURCHASERETURN') " + strSubQuery + " Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP,_SM2._Disc,Category,PartyType,BillCode "
                        //               + " )_Sales Group by DESIGNNAME, MRP  Order by ORDERDESIGNNAME ";

                        string strQuery = "";// Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST((MRP*(100-MAX(ISNULL(Disc,0)))/100) as numeric(18,2)) as varchar)+'|'+CAST(CAST(MRP as numeric(18,2)) as varchar))ORDERDESIGNNAME,'' as OtherDetails from ( Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME, SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,ItemName from StockMaster STM Where ItemName != '' and BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN')  Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP UNION ALL Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME,-SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,ItemName from StockMaster STM Where ItemName != '' and BillType in ('SALES','PURCHASERETURN','STOCKOUT') Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP )_Sales OUTER APPLY (Select ((CASE WHEN _ICM.DisPer=0 and _ICM.Margin>0 then -1 else 1 end)*(ISNULL(_ICM.DisPer,0)-ISNULL((CASE WHEN _ICM.CategoryName Like('SHIYARAM%') and _ICM.Margin=1 then 0 else _ICM.Margin end),0))) Disc from Items _IM inner join ItemCategoryMaster  _ICM on _IM.Other=_ICM.CategoryName Where _Im.ItemName=_Sales.ItemName and MRP>_ICM.FromRange and MRP<_ICM.ToRange) _IM Group by DESIGNNAME, MRP having(SUM(Qty)>0)  Order by ORDERDESIGNNAME ";
                        strQuery = "Select (DESIGNNAME+'|'+CAST(SUM(Qty) as varchar)+'|'+CAST(CAST((MRP*(100-MAX(ISNULL(Disc,0)))/100) as numeric(18,2)) as varchar)+'|'+CAST(CAST(MRP as numeric(18,2)) as varchar))ORDERDESIGNNAME,BarCode as OtherDetails from ( Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME, SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,ItemName,ISNULL(Variant1, '')Variant1,ISNULL(Variant2, '')Variant2 from StockMaster STM Where ItemName != '' and BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN')  Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP UNION ALL Select (ItemName + '|' + ISNULL(Variant2, '') + '|' + ISNULL(Variant1, '') + '|' + ISNULL(Variant3, '') + '|' + ISNULL(Variant4, '') + '|' + ISNULL(Variant5, '')) as DESIGNNAME,-SUM(Qty) as Qty,ISNULL(MRP, 0)MRP,ItemName,ISNULL(Variant1, '')Variant1,ISNULL(Variant2, '')Variant2 from StockMaster STM Where ItemName != '' and BillType in ('SALES','PURCHASERETURN','STOCKOUT') Group By ItemName,ISNULL(Variant1, ''),ISNULL(Variant2, ''),ISNULL(Variant3, ''),ISNULL(Variant4, ''),ISNULL(Variant5, ''),Qty,MRP )_Sales OUTER APPLY (Select ((CASE WHEN _ICM.DisPer=0 and _ICM.Margin>0 then -1 else 1 end)*(ISNULL(_ICM.DisPer,0)-ISNULL((CASE WHEN _ICM.CategoryName Like('SHIYARAM%') and _ICM.Margin=1 then 0 else _ICM.Margin end),0))) Disc,_IS.Description as BarCode from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo left join ItemCategoryMaster  _ICM on _IM.Other=_ICM.CategoryName Where _Im.ItemName=_Sales.ItemName and _IS.Variant1=_Sales.Variant1 and _IS.Variant2=_Sales.Variant2 and MRP>_ICM.FromRange and MRP<_ICM.ToRange) _IM Group by DESIGNNAME,BarCode, MRP having(SUM(Qty)>0)  Order by ORDERDESIGNNAME ";
                        table = DataBaseAccess.GetDataTableRecord(strQuery);
                    }
                    else if (boxStatus)
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct (" + strAllVariant_Order + ") as ORDERDESIGNNAME,DSM.Description as OtherDetails,Variant2,Variant1 from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 and DM.SubGroupName='PURCHASE' " + strSubQuery + " Order By ORDERDESIGNNAME,Variant2,Variant1");
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct ItemName as ORDERDESIGNNAME,'' as OtherDetails from Items DM left join ItemSecondary DSM on DM.BillNo=DSM.BillNo Where DM.ItemName!='' and DM.SubGroupName='PURCHASE' " + strSubQuery + " Order By ORDERDESIGNNAME ");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (DSM.Description+'|'+" + strAllVariant + ") as DESIGNNAMEWITHBARCODE,DSM.Description as OtherDetails,Variant2,Variant1 from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 and DM.SubGroupName='PURCHASE' and DSM.Description!='' and [ActiveStatus]=1 " + strSubQuery + " Order By DESIGNNAMEWITHBARCODE,Variant2,Variant1");
                }
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALEMERGE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BarCode+'|'+" + strAllVariant + "+'|'+CAST(SUM(Qty)as varchar)) as DESIGNNAMEWITHBARCODE_SALEMERGE from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select Distinct ISNULL(_IS.Description,'')BarCode,_IM.BrandName,BuyerDesignName as DesignName,ItemName,ISNULL(Variant1,'')Variant1,ISNULL(Variant2,'')Variant2,ISNULL(Variant3,'')Variant3,ISNULL(Variant4,'')Variant4,ISNULL(Variant5,'')Variant5,1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and DisStatus=0  UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )_Stock Group by (BarCode+'|'+" + strAllVariant+ ") having(SUM(Qty)>0) ");
                 
                    //string strQuery = "Select Distinct (BarCode+'|'+ItemName+'|'+CAST(Qty as varchar)) as DESIGNNAMEWITHBARCODE_SALEMERGE from ( Select Distinct ISNULL(_IS.Description,'')BarCode,ItemName,ISNULL(Variant1,'')Variant1,ISNULL(Variant2,'')Variant2,ISNULL(Variant3,'')Variant3,ISNULL(Variant4,'')Variant4,ISNULL(Variant5,'')Variant5,0 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where DisStatus=0)_Stock ";// Select Distinct (BarCode+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)) as DESIGNNAMEWITHBARCODE_SALEMERGE from (Select BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN') Group by BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select ISNULL(BarCode,'')BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES') Group by BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select Distinct ISNULL(_IS.Description,'')BarCode,ItemName,ISNULL(Variant1,'')Variant1,ISNULL(Variant2,'')Variant2,ISNULL(Variant3,'')Variant3,ISNULL(Variant4,'')Variant4,ISNULL(Variant5,'')Variant5,0 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and DisStatus=0 )Stock Group by BarCode,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5)_Stock ";

                    //table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                //else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALE")
                //{
                //    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BarCode+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)) as DESIGNNAMEWITHBARCODE_SALE from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 having(SUM(Qty)>0))_Stock ");
                //}
                else if (strSearchData == "DESIGNNAMEWITHBARCODE_SALERETURN")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BarCode+'|'+BrandName+'|'+DesignName+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)+'|'+CAST(Rate as varchar)+'|'+CAST(MRP as varchar)+'|'+UnitName) as DESIGNNAMEWITHBARCODE_SALERETURN from ( Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty,Rate,MRP,UnitName from ( Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty,Rate,MRP,UnitName from SalesBookSecondary Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty,Rate,MRP,UnitName from SaleReturnDetails Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,MRP,UnitName having(SUM(Qty)>0))_Stock  ");
                }
                else if (strSearchData == "ITEM_NAME_ST")
                {
                    //                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct (BarCode+'|'+BrandName+'|'+" + strAllVariant + "+'|'+CAST(Qty as varchar)) as ITEM_NAME_ST from (Select BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty)Qty from (Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 UNION ALL Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'')BrandName,ISNULL(DesignName,'')DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,-SUM(Qty) Qty from StockMaster Where BillType in ('PURCHASERETURN','SALES','STOCKOUT') Group by BarCode,ISNULL(BrandName,''),ISNULL(DesignName,''),ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 )Stock Group by BarCode,BrandName,DesignName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5 having(SUM(Qty)>0))_Stock ");
                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct (ST.BarCode+'|'+BrandName+'|'+ItemName+'|'+Variant1+'|'+Variant2+'|1') as ITEM_NAME_ST FROM ItemStock ST LEFT JOIN (Select ParentBarCode,Barcode,SUM(SetQty) SetQty,ISNULL(InStock,0)InStock from BarcodeDetails Group by ParentBarCode,Barcode,InStock)BCD On ST.Barcode = BCD.ParentBarCode  WHERE (IsWithoutStock = 1 OR StockQty > 0) AND isnull(BCD.InStock,0) = 1 ");
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
                if (strSearchData != "ORDERDESIGNNAME")
                {
                    if (table != null)
                    {
                        foreach (DataRow row in table.Rows)
                            lbSearchBox.Items.Add(row[0]);
                    }
                    AddNewData();
                }
                else
                {
                    this.Text = "Please type atleast 3 character";
                }
            }
            catch
            {
            }
        }

        private void AddNewData()
        {
            if (strSONumber == "" && strPONumber == "" && !chkStatus && !boxStatus && !strSearchData.Contains("BARCODE") && !strSearchData.Contains("SONUMBER"))
                lbSearchBox.Items.Add("ADD NEW " + strCatName + " NAME");
            else if (strSearchData == "DESIGNNAME")
                lbSearchBox.Items.Add("ADD NEW ITEM NAME");

            if (lbSearchBox.Items.Count > 0 && !boxStatus)
                lbSearchBox.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            try
            {
                lbSearchBox.Items.Clear();
                if (table != null)
                {
                    string strLikeKey = "";
                    //  if (boxStatus)
                    strLikeKey = "%";
                    if (txtSearch.Text == "")
                    {
                        if (strSearchData != "ORDERDESIGNNAME")
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                lbSearchBox.Items.Add(row[0]);
                            }
                        }
                    }
                    else
                    {
                        DataRow[] rows = null;
                        if (strSearchData == "DESIGNNAME")
                            rows = table.Select(String.Format(strSearchData + " Like('" + strLikeKey + txtSearch.Text + "%') OR OtherDetails Like('" + strLikeKey + txtSearch.Text + "%') "));
                        else if (strSearchData == "ORDERDESIGNNAME")
                        {
                            if (txtSearch.Text.Length > 2)
                                rows = table.Select(String.Format(strSearchData + " Like('%"+ txtSearch.Text + "%') OR OtherDetails Like('%" + txtSearch.Text + "%') "));
                        }
                        else
                            rows = table.Select(String.Format(strSearchData + " Like('" + strLikeKey + txtSearch.Text + "%') "));

                        if (rows.Length > 0)
                        {
                            foreach (DataRow row in rows)
                            {
                                lbSearchBox.Items.Add(row[0]);
                            }
                        }
                    }
                    AddNewData();
                }
                else
                {
                    GetDataAndBind();
                }
            }
            catch { }
        }


        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                    //if (txtSearch.Text != "" || strSelectedData!="")
                        closeStatus = true;
                    //else 
                    //    closeStatus = false;
                    this.Close();
                }                
                else if (e.KeyCode == Keys.Up)
                {
                    int index = lbSearchBox.SelectedIndex;
                    if (index > 0)
                    {
                        lbSearchBox.SelectedIndex = index - 1;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int index = lbSearchBox.SelectedIndex;
                    if (index < lbSearchBox.Items.Count - 1)
                    {
                        lbSearchBox.SelectedIndex = index + 1;
                    }
                }
            }
            catch
            {
            }
        }

        private void lbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                    closeStatus = true;
                    this.Close();
                }
            }
            catch
            {
            }
        }

        private void lbSearchBox_Click(object sender, EventArgs e)
        {
            try
            {                
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                    closeStatus = true;
                    this.Close();               
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
                lbSearchBox.Items.Clear();
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
                    if (strSelectedData == "ADD NEW ITEM NAME" || strSelectedData == "ADD NEW DESIGNNAME NAME" || strSelectedData == "ADD NEW ORDERDESIGNNAME NAME")
                    {
                        if (MainPage.bArticlewiseOpening)
                        {
                            ItemMaster objItemMaster = new ItemMaster(true);
                            objItemMaster.strItemName = strText;
                            objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objItemMaster.ShowInTaskbar = true;
                            objItemMaster.ShowDialog();
                            strSelectedData = objItemMaster.StrAddedDesignName;
                        }
                        else
                        {
                            DesignMaster objDesignMaster = new DesignMaster(true);
                            objDesignMaster.strItemName = strText;
                            objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objDesignMaster.ShowInTaskbar = true;
                            objDesignMaster.ShowDialog();
                            strSelectedData = objDesignMaster.StrAddedDesignName;
                        }
                        if (strSelectedData == "")
                            e.Cancel = true;
                    }
                    else if (strSelectedData == "ADD NEW " + strCatName + " NAME")
                    {
                        VariantMaster objVariantMaster = new VariantMaster(strCatNo, strCatName,true, strText);
                        objVariantMaster.ShowDialog();
                        strSelectedData = objVariantMaster.StrAddedCategory;
                        if (strSelectedData == "")
                            e.Cancel = true;
                    }
                }
                
                if (!closeStatus)
                    lbSearchBox.Items.Clear();
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
           // GetDataAndBind();
        }
    }
}
