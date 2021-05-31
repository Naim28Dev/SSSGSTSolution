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
    public partial class FairDetails : Form
    {
        DataBaseAccess dba;
        public FairDetails()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();           
        }
        private void FairDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {              
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
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

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {           

            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GOODSRCODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
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

        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtScheme.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            lblNetDiscAmt.Text = lblNetDiscAmt.Text = "0.00";

            if (txtScheme.Text != "")
            {
               // if (txtScheme.Text.Contains("TOUR") || txtScheme.Text.Contains("FAIR"))
                {
                    dgrdSummary.Columns["disPer"].Visible = dgrdSummary.Columns["disAmt"].Visible = false;
                    dgrdSummary.Columns["expectedONo"].Visible = dgrdSummary.Columns["targetAmt"].Visible = dgrdSummary.Columns["netSaleAmt"].Visible = true;
                    dgrdDetailView.Columns["dDisPer"].Visible = dgrdDetailView.Columns["dDisAmt"].Visible = true;
                    dgrdDetailView.Columns["billValue"].Visible = true;
                    SearchTourData();
                }
                //else
                //{
                //    SearchData();
                //    dgrdSummary.Columns["disPer"].Visible = dgrdSummary.Columns["disAmt"].Visible = true;                    
                //    dgrdDetailView.Columns["dDisPer"].Visible = dgrdDetailView.Columns["dDisAmt"].Visible = true;
                //    dgrdDetailView.Columns["billValue"].Visible = false;
                //    //if(txtScheme.Text.Contains("9TH"))
                //    //    dgrdSummary.Columns["expectedONo"].Visible = dgrdSummary.Columns["targetAmt"].Visible = dgrdSummary.Columns["netSaleAmt"].Visible = true;
                //}
            }
            else
            {
                MessageBox.Show("Sorry ! Scheme name can't be blank !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtScheme.Focus();
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery(ref string strNickName,ref string strOtherName, ref string strNQuery,ref string strCustomerSchemeQuery, ref string strRetailQuery)
        {
            string strQuery = "";
            if (txtSalesParty.Text != "")
            {
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {                  
                    strQuery += " and GR.SalePartyID='" + strFullName[0].Trim() + "'";
                    strOtherName += " and OB.SalePartyID='" + strFullName[0].Trim() + "'";
                    strCustomerSchemeQuery += " and CustomerName in (Select SM.Other from SupplierMaster SM Where (AreaCode+AccountNo)='" + strFullName[0].Trim() + "') ";
                }
            }

            if (txtNickName.Text != "")
            {
                strNickName = " Where SalesParty='" + txtNickName.Text + "'";
                strOtherName += " and Other='" + txtNickName.Text + "'";
                strNQuery += " and SM.SalesParty='" + txtNickName.Text + "' ";
                strCustomerSchemeQuery += " and CustomerName='" + txtNickName.Text + "' ";
            }

            if (txtSupplierName.Text != "")
            {               
                strNQuery += " and SM1.PurchaseParty='" + txtSupplierName.Text + "' ";
                strCustomerSchemeQuery += " and CustomerName='" + txtSupplierName.Text + "' ";
            }

            if (txtScheme.Text != "")
                strQuery += " and OB.SchemeName = '" + txtScheme.Text + "' ";

            if (txtScheme.Text.Contains("9TH"))
                strQuery += " and ISNULL(GR.PurchasePartyID,'') not in ('DL149','DL5255','TKR9363','LDH1156') ";

            strRetailQuery = strQuery;

            if (txtPurchaseParty.Text != "")
            {
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {                   
                    strQuery += " and GR.PurchasePartyID='" + strFullName[0].Trim() + "'";
                    strOtherName += " and OB.PurchasePartyID='" + strFullName[0].Trim() + "'";
                    strCustomerSchemeQuery += " and CustomerName='" + strFullName[0].Trim() + "' ";
                    strRetailQuery += " and PurchasePartyID='" + strFullName[0].Trim() + "'";
                }
            }

            if (txtBillCode.Text != "")
            {
                strRetailQuery += " and GR.BillCode in (Select Top 1 SBillCode from CompanySetting Where PBillCode in  ('" + txtBillCode.Text + "') ) ";
                strQuery += " and GR.ReceiptCode in  ('" + txtBillCode.Text + "') ";
            }
            return strQuery;
        }

        private string CreateQuery_Fair(ref string strNickName, ref string strOtherName, ref string strNQuery, ref string strCustomerSchemeQuery, ref string strRetailQuery)
        {
            string strQuery = "";
            if (txtSalesParty.Text != "")
            {
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strQuery += " and GR.SalePartyID='" + strFullName[0].Trim() + "'";
                    strOtherName += " and OB.SalePartyID='" + strFullName[0].Trim() + "'";
                    strCustomerSchemeQuery += " and CustomerName in (Select SM.Other from SupplierMaster SM Where (AreaCode+AccountNo)='" + strFullName[0].Trim() + "') ";
                }
            }

            if (txtNickName.Text != "")
            {
                strNickName = " Where SalesParty='" + txtNickName.Text + "'";
                strOtherName += " and Other='" + txtNickName.Text + "'";
                strNQuery += " and SMS.OTHER='" + txtNickName.Text + "' ";
                strCustomerSchemeQuery += " and CustomerName='" + txtNickName.Text + "' ";
            }

            if (txtSupplierName.Text != "")
            {
                strNQuery += " and SMP.Other='" + txtSupplierName.Text + "' ";
                strCustomerSchemeQuery += " and CustomerName='" + txtSupplierName.Text + "' ";
            }

            //if (txtScheme.Text != "")
            //    strQuery += " and OB.SchemeName = '" + txtScheme.Text + "' ";

            if (txtScheme.Text.Contains("9TH"))
                strQuery += " and ISNULL(GR.PurchasePartyID,'') not in ('DL149','DL5255','TKR9363','LDH1156') ";

            strRetailQuery = strQuery;

            if (txtPurchaseParty.Text != "")
            {
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strQuery += " and GR.PurchasePartyID='" + strFullName[0].Trim() + "'";
                    strOtherName += " and OB.PurchasePartyID='" + strFullName[0].Trim() + "'";
                    strCustomerSchemeQuery += " and CustomerName='" + strFullName[0].Trim() + "' ";
                    strRetailQuery += " and PurchasePartyID='" + strFullName[0].Trim() + "'";
                }
            }

            if (txtBillCode.Text != "")
            {
                strRetailQuery += " and GR.BillCode in (Select Top 1 SBillCode from CompanySetting Where PBillCode in  ('" + txtBillCode.Text + "') ) ";
                strQuery += " and GR.ReceiptCode in  ('" + txtBillCode.Text + "') ";
            }
            return strQuery;
        }


        //strOtherQuery = " Select SalesParty,PurchaseParty,ReceiptCode,ReceiptNo,SalesAmt as Amt from ( "
        //         + " Select Distinct SM.Other as OSalesParty from OrderBooking OB CROSS APPLY(Select Other from SupplierMaster SM Where AreaCode + AccountNo = OB.SalePartyID) SM Where OB.SchemeName = '"+txtScheme.Text+"'  and OB.PurchasePartyID in ('DL149','DL5255') "+ strOtherName
        //         + " )_Order Outer APPLY( "
        //         + " Select Top 4 RCode as ReceiptCode,RNo as ReceiptNo , PurchaseParty, Other as SalesParty, SalesAmt from( "
        //         + " Select Top 2 *, (Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) Amount from(Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from(Select(GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (RCode + ' ' + CAST(RNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType )_Sales)SalesAmt from( "
        //         + " Select  GR.ReceiptCode RCode, MIN(GR.ReceiptNo) as RNo, (GR.PurchasePartyID + ' ' + GR.PurchaseParty)PurchaseParty, SM.Other from GoodsReceive GR  inner join OrderBooking OB on (CASE When OB.NumberCode != '' then(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) else OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) end) = GR.OrderNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo CROSS APPLY (Select Other From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM Where GR.PurchasePartyID = 'DL5255' and SM.Other = _Order.OSalesParty " + strSubQuery + strOtherName+ " Group by REPLACE(GRD.ItemName, 'JEANS', 'PANT'), SM.Other, GR.ReceiptCode, GR.PurchasePartyID,GR.PurchaseParty "
        //         + " )Purchase UNION ALL "
        //         + " Select *,(Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) Amount from (Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from( "
        //         + " Select(GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) and GRD.ReceiptCode = _Sale.ReceiptCode and GRD.ReceiptNo = _SALE.ReceiptNo Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (_Sale.ReceiptCode + ' ' + CAST(_Sale.ReceiptNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType )_Sales)SalesAmt from ( "
        //         + " Select Top 2 GR.ReceiptCode, (GR.ReceiptNo) as ReceiptNo, (GR.PurchasePartyID + ' ' + GR.PurchaseParty)PurchaseParty, SM.Other "
        //         + " from GoodsReceive GR  inner join OrderBooking OB on (CASE When OB.NumberCode != '' then(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) else OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) end) = GR.OrderNo CROSS APPLY (Select Other From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM Where GR.PurchasePartyID = 'DL149' and SM.Other = _Order.OSalesParty "+ strSubQuery+ strOtherName+"  Order by ReceiptNo asc "
        //         + " )_Sale)_Sales) _Sales Where SalesParty is not NULL";

        private void SearchData()
        {
            try
            {
                string strQuery = "", strNickNameQuery = "", strOtherName="", strNQuery="",strCustomerSchemeQuery="",strRetailQuery="", strSubQuery = CreateQuery(ref strNickNameQuery, ref strOtherName, ref strNQuery, ref strCustomerSchemeQuery, ref strRetailQuery),strOtherQuery="";

                strQuery += " Select _GR.ReceiptCode,_GR.ReceiptNo,_GR.SalePartyID,(_GR.PurchasePartyID + ' ' + _GR.PurchaseParty) as PurchaseParty, "
                         + "  SaleAmt from GoodsReceive _GR  inner join OrderBooking OB on(CASE When OB.NumberCode != '' then(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) else OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) end) = _GR.OrderNo CROSS APPLY (Select Other From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = _GR.SalePartyID) SM OUTER APPLY (Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SaleAmt from (Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from( "
                         + " Select (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' OUTER APPLY (Select GRD.ItemName,GRD.Quantity,GRD.Rate,GRD.Amount from GoodsReceiveDetails GRD Where GRD.ReceiptCode = _GR.ReceiptCode and GRD.ReceiptNo = _GR.ReceiptNo and SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)))GRD Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (_GR.ReceiptCode + ' ' + CAST(_GR.ReceiptNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType )_Sales "
                         + " )_SaleAmt Where (_GR.PurchasePartyID not in ('DL149','DL5255','CCK453','LDH1156') " + strSubQuery.Replace("GR.", "_GR.") + strNickNameQuery.Replace("Where SalesParty", "and Other")  + ") ";

                if (txtScheme.Text.Contains("8TH"))
                {
                    strOtherQuery = " Select SalesParty,PurchaseParty,ReceiptCode,ReceiptNo,SalesAmt from (Select Distinct SM.Other as OSalesParty from OrderBooking OB CROSS APPLY (Select Other from SupplierMaster SM Where AreaCode+AccountNo=OB.SalePartyID) SM Where OB.PurchasePartyID in ('DL149','DL5255') " + strOtherName + "  "
                             + " )_Order Outer APPLY(Select Top 4 RCode as ReceiptCode, RNo as ReceiptNo, PurchaseParty, Other as SalesParty, SalesAmt from( "
                             + " Select Top 2 * from(Select  GR.ReceiptCode RCode, MIN(GR.ReceiptNo) as RNo, (GR.PurchasePartyID + ' ' + GR.PurchaseParty)PurchaseParty, SM.Other from GoodsReceive GR  inner join OrderBooking OB on (CASE When OB.NumberCode != '' then(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) else OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) end) = GR.OrderNo CROSS APPLY(Select GRD.ItemName from GoodsReceiveDetails GRD Where GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo) GRD CROSS APPLY(Select Other From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM Where GR.PurchasePartyID = 'DL5255' and SM.Other = _Order.OSalesParty " + strSubQuery + strOtherName + "  Group by REPLACE(GRD.ItemName, 'JEANS', 'PANT'), SM.Other, GR.ReceiptCode, GR.PurchasePartyID, GR.PurchaseParty "
                             + " )Purchase OUTER APPLY(Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SalesAmt from(Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from(Select(GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' OUTER APPLY (Select GRD.ItemName, GRD.Quantity, GRD.Rate, GRD.Amount from  GoodsReceiveDetails GRD Where SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))) GRD Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then ((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (RCode + ' ' + CAST(RNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType )_Sales)_SalesAmt UNION ALL "
                             + " Select * from (Select Top 2 GR.ReceiptCode, (GR.ReceiptNo) as ReceiptNo, (GR.PurchasePartyID + ' ' + GR.PurchaseParty)PurchaseParty, SM.Other, SalesAmt "
                             + " from GoodsReceive GR  inner join OrderBooking OB on (CASE When OB.NumberCode != '' then(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) else OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) end) = GR.OrderNo CROSS APPLY (Select Other From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM OUTER APPLY((Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SalesAmt from (Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from( "
                             + " Select (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, ((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))/(CAST(OtherField as Money)))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' OUTER APPLY(Select GRD.ItemName, GRD.Quantity, GRD.Rate, GRD.Amount from GoodsReceiveDetails GRD Where SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) and GRD.ReceiptCode = GR.ReceiptCode and GRD.ReceiptNo = GR.ReceiptNo)GRD Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (GR.ReceiptCode + ' ' + CAST(GR.ReceiptNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType )_Sales))_SAmtAmt Where GR.PurchasePartyID = 'DL149' and SM.Other = _Order.OSalesParty " + strSubQuery + strOtherName + "  Order by ReceiptNo asc )_Sale)_Sales) _Sales Where SalesParty is not NULL ";
                }
                else
                {
                    strOtherQuery = " Select '' as SalesParty,0 SalesAmt";
                }

                if (rdoSummary.Checked)
                {
                    strQuery = " Select SalesParty,SUM(Amt) as Amt from ( Select SM.SalesParty,SUM(Amt) Amt  from ( Select SalePartyID,ROUND(SUM(SaleAmt),0) as Amt from ( "
                             + strQuery + "  )_Sales Group by SalePartyID )Sale OUTER Apply (Select TOP 1 Other as  SalesParty from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID) SM " + strNickNameQuery + " Group by SM.SalesParty "
                             + " UNION ALL Select SalesParty,SUM(SalesAmt) Amt from (" + strOtherQuery + ")_Sales Group by SalesParty "
                             + " )_Sales Where SalesParty!='' Group by SalesParty  Order by SalesParty ";
                }
                else
                {
                    strQuery = "Select SalesParty,PurchaseParty,ReceiptCode,ReceiptNo, Amt from ( Select SM.SalesParty,PurchaseParty,ReceiptCode,ReceiptNo,SUM(SaleAmt) Amt from ( "
                             + strQuery + " )Sale Outer Apply (Select TOP 1 Other as  SalesParty from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID) SM " + strNickNameQuery + " Group by SM.SalesParty,PurchaseParty,ReceiptCode,ReceiptNo "
                             + " UNION ALL "+ strOtherQuery 
                             + " ) _Sales  Where SalesParty!='' Order by SalesParty,ReceiptNo ";
                }

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataWithGrid(dt);
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void SearchTourData()
        {
            try
            {
                string strQuery = "",strOrderQuery="",strMKTRQuery="", strNickNameQuery = "", strOtherName = "",strNQuery="",strCustomerSchemeQuery="",strRetailQuery="", strSubQuery = CreateQuery_Fair(ref strNickNameQuery, ref strOtherName, ref strNQuery, ref strCustomerSchemeQuery,ref strRetailQuery),strTargetQuery="";
                if (rdoWithTarget.Checked)
                    strTargetQuery = " and ISNULL(TargetValue,0)!=0 ";
                else  if (rdoWithoutTarget.Checked)
                    strTargetQuery = " and ISNULL(TargetValue,0)=0 ";

                if (rdoWithAdda.Checked)
                    strMKTRQuery = " and Marketer Like('%ADDA%')";
                else if (rdoWithoutAdda.Checked)
                    strMKTRQuery = " and Marketer NOT Like('%ADDA%')";
                //strOrderQuery = " (Select SUM(((Amount/(CAST(Quantity as Money))*(CAST(Quantity as Money)-((AdjustedQty+CancelQty))))*ISNULL(_BillValue,0.5))) OrderAmt from OrderBooking OB CROSS APPLY (Select Other from SupplierMaster Where GroupName='SUNDRY DEBTORS' and AreaCode+AccountNo=OB.SalePartyID) SM CROSS APPLY (Select _BillValue from SupplierMaster _SMP CROSS APPLY (Select BillValue _BillValue from Scheme_SupplierDetails SSD Where SSD.SchemeName=OB.SchemeName and SupplierName=_SMP.Other) _SSD Where GroupName='SUNDRY CREDITOR' and ((AreaCode+AccountNo)=OB.PurchasePartyID OR OrderCode Like('PTN%'))) SMP Where OB.SchemeName = '" + txtScheme.Text+"' and OB.Status='PENDING' and ((CAST(Quantity as Money)-(AdjustedQty+CancelQty))*100)/CAST(Quantity as Money)>0 and SM.Other=SalesParty)  ";

                strOrderQuery = " 0 ";

                strQuery += " Select _Sales.*, (SaleAmt * ISNULL((BillValue), 1))_NetSaleAmt, ISNULL(TargetValue, 0)TargetValue, (ISNULL(BillValue, 0)) as BillValue from( "
                         + "Select GR.ReceiptCode, GR.ReceiptNo, (GR.SalePartyID + ' ' + SMS.Name)SalePartyID, (GR.PurchasePartyID + ' ' + SMP.Name) as PurchasePartyID, SMS.Other as SalesParty, SMP.Other as PurchaseParty, SUM((GRD.Quantity * ((GRD.Rate * ((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00)) * (CASE WHEN TaxIncluded = 0 then 1 else 100.00 / (100.00 + (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) end))))SaleAmt, Dhara,GR.PurchasePartyID as _PurchasePartyID,SchemeName from GoodsReceive GR "
                         + "left join SaleTypeMaster SMN On GR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' "
                         + "left join OrderBooking OB on OB.SalePartyID = GR.SalePartyID and RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) = GR.OrderNo "+ strMKTRQuery
                         + "inner join SupplierMaster SMS on SMS.AreaCode + SMS.AccountNo = GR.SalePartyID "
                         + "left join SupplierMaster SMP on SMP.AreaCode + SMP.AccountNo = GR.PurchasePartyID "
                         + "left join SalesEntry SE on SE.GRSNo = (GR.ReceiptCode + ' ' + CAST(GR.ReceiptNo as varchar)) "
                         + "left join GoodsReceiveDetails GRD on GRD.ReceiptCode = GR.ReceiptCode and GRD.ReceiptNo = GR.ReceiptNo "
                         + "left join Items _IM on GRD.ItemName = _IM.ItemName "
                         + "left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName "
                         + "left join TaxCategory _TC on _TC.CategoryName = _IGM.TaxCategoryName "
                         + "Where OB.SchemeName ='" + txtScheme.Text + "'  " + strSubQuery + strNQuery + " GROUP by GR.ReceiptCode, GR.ReceiptNo, GR.SalePartyID, SMS.Name, GR.PurchasePartyID, SMP.Name, SMS.Other, SMP.Other, Dhara, GR.PurchasePartyID, SchemeName UNION ALL "
                         + "Select GR.BillCode as ReceiptCode, GR.BillNo as ReceiptNo, (GR.SalePartyID + ' ' + SMS.Name)SalePartyID, (PurchasePartyID + ' ' + SMP.Name) as PurchasePartyID, SMS.Other as SalesParty, SMP.Other as PurchaseParty, SUM(_SBS.Amount)SaleAmt, 'NORMAL' Dhara, PurchasePartyID as _PurchasePartyID, SchemeName from SalesBook GR inner join SalesBookSecondary _SBS  on GR.BillCode = _SBS.BillCode and GR.BillNo = _SBS.BillNo "
                         + "left join(Select RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode)SOrderNo, SalePartyID, SchemeName from OrderBooking OB Where OB.OrderNo>0 "+ strMKTRQuery+")OB on OB.SOrderNo = _SBS.SONumber and OB.SalePartyID = GR.SalePartyID "
                         + "left join(Select MAX(PB.PurchasePartyID)PurchasePartyID, ItemName from PurchaseBook PB inner join PurchaseBookSecondary PBS ON PB.BillCode = PBS.BillCode and PB.BillNo = PBS.BillNo Group by PBS.ItemName)PBS ON PBS.ItemName = _SBS.ItemName "
                         + "inner join SupplierMaster SMS on SMS.AreaCode + SMS.AccountNo = GR.SalePartyID "
                         + "left join SupplierMaster SMP on SMP.AreaCode + SMP.AccountNo = PBS.PurchasePartyID "
                         + "Where SchemeName = '" + txtScheme.Text + "' " + strRetailQuery + strNQuery+" Group by GR.BillCode, GR.BillNo, GR.SalePartyID, SMS.Name, PurchasePartyID, SMP.Name, SMS.Other, SMP.Other, SchemeName)_Sales "
                         + "left join Scheme_SupplierDetails SSD on ISNULL(SSD.SupplierName, '') = ISNULL(_Sales.PurchaseParty, '') and SSD.SchemeName = _Sales.SchemeName "
                         + "left join  Scheme_CustomerDetails SCD  on SCD.CustomerName = SalesParty and SCD.SchemeName = _Sales.SchemeName "
                         + "Where (SaleAmt != 0 OR ISNULL(TargetValue, 0) != 0) " + strTargetQuery;


                //strQuery += "  Select _Sales.*,(SaleAmt*ISNULL(BillValue,1))_NetSaleAmt,ISNULL(TargetValue,0)TargetValue,BillValue from ( "
                //         + " Select  GR.ReceiptCode,GR.ReceiptNo,(GR.SalePartyID + ' ' + SName)SalePartyID,(GR.PurchasePartyID + ' ' + PName) as PurchasePartyID,SM.SalesParty,SM1.PurchaseParty, SaleAmt,Dhara,PurchasePartyID as _PurchasePartyID  from GoodsReceive GR OUTER APPLY (Select SchemeName from OrderBooking OB Where  OB.SalePartyID=GR.SalePartyID and  RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) = GR.OrderNo "+ strMKTRQuery+") OB OUTER APPLY(Select Name as SName, Other as SalesParty From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM OUTER APPLY(Select Name as PName, Other as PurchaseParty From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.PurchasePartyID) SM1 OUTER APPLY(Select SUM(ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SaleAmt from (Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType from(Select(GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesEntry SE left join SaleTypeMaster SMN On GR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' OUTER APPLY(Select GRD.ItemName, GRD.Quantity, GRD.Rate, GRD.Amount from GoodsReceiveDetails GRD Where GRD.ReceiptCode = GR.ReceiptCode and GRD.ReceiptNo = GR.ReceiptNo)GRD Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SE.GRSNo = (GR.ReceiptCode + ' ' + CAST(GR.ReceiptNo as varchar)) and GRD.Amount > 0 )_Sales Group by HSNCode, TaxRate, TaxType)_Sales "
                //         + " )_SaleAmt Where GR.ReceiptCode!='' " + strSubQuery+ strNQuery 
                //         + " UNION ALL Select  GR.BillCode as ReceiptCode,GR.BillNo as ReceiptNo,(GR.SalePartyID + ' ' + SName)SalePartyID,(PurchasePartyID+' '+PurchaseParty) as PurchasePartyID,SM.SalesParty,PName as PurchaseParty, SaleAmt,'NORMAL' Dhara,PurchasePartyID as _PurchasePartyID  from SalesBook GR  OUTER APPLY(Select Name as SName, Other as SalesParty From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = GR.SalePartyID) SM OUTER APPLY(Select SchemeName,SUM(ROUND((CASE WHEN SMN.TaxIncluded = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SaleAmt,PurchasePartyID,PName,PurchaseParty from  SalesBookSecondary SBS OUTER APPLY (Select Top 1 PB.PurchasePartyID,PB.PurchaseParty from PurchaseBook PB inner join PurchaseBookSecondary PBS ON PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo Where PBS.ItemName=SBS.ItemName and PBS.Variant1=SBS.Variant1 and PBS.Variant2=SBS.Variant2)PBS OUTER APPLY(Select Other as PName From SupplierMaster SM Where  SM.AreaCode + SM.AccountNo = PBS.PurchasePartyID) SM1 OUTER APPLY (Select SchemeName from OrderBooking OB Where OB.SalePartyID=GR.SalePartyID and RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) = SBS.SONumber " + strMKTRQuery + ") OB left join SaleTypeMaster SMN On GR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SBS.Rate * 100) / (100 + TaxRate)) else SBS.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (sbs.SDisPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SBS.Rate * 100) / (100 + TaxRate)) else SBS.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SBS.SDisPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SBS.ItemName = _IM.ItemName ) as GM Where GR.BillCode=SBS.BillCode and GR.BillNo=SBS.BillNo Group by SchemeName,PurchasePartyID,PurchaseParty,PName)_Sales Where GR.BillNo!='' " + strRetailQuery.Replace("GR.","").Replace("OB.", "") + strNQuery.Replace("SM1.", "").Replace("SM.", "")
                //         + " )_Sales Outer Apply(Select Top 1 (CASE WHEN (_PurchasePartyID in ('DL312','DL745') and Dhara='NORMAL') then 1 else BillValue end) as BillValue from Scheme_SupplierDetails Where ISNULL(SupplierName,'') = ISNULL(PurchaseParty,'') and SchemeName = '" + txtScheme.Text+ "')SSD Outer Apply(Select TargetValue from Scheme_CustomerDetails Where CustomerName = SalesParty and SchemeName = '" + txtScheme.Text + "')SCD Where (SaleAmt!=0 OR ISNULL(TargetValue,0)!=0)  " + strTargetQuery;
         
                if (rdoSummary.Checked)
                {
                    strQuery = " Select SalePartyID,SalesParty,SUM(Amt)Amt,AVG(TargetValue)TargetValue,SUM(OAmt)OAmt from (Select SalePartyID,SalesParty,SUM(_NetSaleAmt) as Amt,TargetValue," + strOrderQuery+" as OAmt from ( "
                             + strQuery + ")Sales Group by SalePartyID,SalesParty,TargetValue "
                             + " UNION ALL  Select SalesPartyID,CustomerName as SalesParty,0 as Amt,TargetValue,0 as OAmt from Scheme_CustomerDetails Where SchemeName = '" + txtScheme.Text+"' "+ strCustomerSchemeQuery+strTargetQuery + ")_Sales  Group by SalePartyID,SalesParty  Order by SalesParty ";
                }
                else
                {
                    strQuery = " Select SalePartyID,SalesParty as SalesParty,PurchasePartyID as PurchaseParty,ReceiptCode,ReceiptNo, _NetSaleAmt as Amt,TargetValue,SalePartyID,PurchasePartyID,BillValue from ( "
                             + strQuery + "  )Sales Order by SalesParty,ReceiptNo ";                           
                }

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataWithGrid(dt);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private double GetDisPer(double dAmt)
        {
            double dDis = 0;
            if (txtScheme.Text.Contains("9TH"))
                dDis = 3;
            else if (!txtScheme.Text.Contains("TOUR"))
            {
                if (dAmt >= 500000 && dAmt < 1000000)
                    dDis = 2;
                else if (dAmt >= 1000000)
                    dDis = 3;
            }
            return dDis;
        }


        private void BindDataWithGrid(DataTable dt)
        {
            try
            {
                dgrdDetailView.Rows.Clear();
                dgrdSummary.Rows.Clear();
                double dTSaleAmt = 0, dNetDiscountAmt = 0, dOAmt = 0, dTOAmt = 0, dTNetSaleAmt = 0;
                if (dt.Rows.Count > 0)
                {
                    if (rdoSummary.Checked)
                    {
                        dgrdSummary.Rows.Add(dt.Rows.Count);
                        int _rowIndex = 0;
                        double dSAmt = 0, dDiscPer = 0, dDiscAmt = 0, dTargetAmt = 0, _dNSaleAmt = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            dTSaleAmt += dSAmt = dba.ConvertObjectToDouble(row["Amt"]);
                            if (dt.Columns.Contains("TargetValue"))
                                dTargetAmt = dba.ConvertObjectToDouble(row["TargetValue"]);
                            if (dt.Columns.Contains("OAmt"))
                                dTOAmt += dOAmt = dba.ConvertObjectToDouble(row["OAmt"]);
                            dDiscPer = GetDisPer(dSAmt);

                            dgrdSummary.Rows[_rowIndex].Cells["sSNo"].Value = (_rowIndex + 1) + ".";
                            dgrdSummary.Rows[_rowIndex].Cells["salesParty"].Value = row["SalesParty"];
                            dgrdSummary.Rows[_rowIndex].Cells["saleAmt"].Value = dSAmt;//.ToString("N2", MainPage.indianCurancy); ;
                            dgrdSummary.Rows[_rowIndex].Cells["disPer"].Value = dDiscPer;
                            dgrdSummary.Rows[_rowIndex].Cells["expectedONo"].Value = dOAmt;

                            if (txtScheme.Text.Contains("TOUR") || txtScheme.Text.Contains("FAIR") || txtScheme.Text.Contains("9TH"))
                            {
                                if (chkConsiderValue.Checked)
                                    dSAmt = (dSAmt + dOAmt);
                                _dNSaleAmt = (dSAmt - dTargetAmt);

                                dgrdSummary.Rows[_rowIndex].Cells["targetAmt"].Value = dTargetAmt;//.ToString("N2",MainPage.indianCurancy);
                                dgrdSummary.Rows[_rowIndex].Cells["netSaleAmt"].Value = _dNSaleAmt;//.ToString("N2", MainPage.indianCurancy);
                            }
                            else
                                _dNSaleAmt = (dSAmt - dTargetAmt);
                            if (dt.Columns.Contains("SalePartyID"))
                                dgrdSummary.Rows[_rowIndex].Cells["SalePartyID"].Value = row["SalePartyID"];

                            dDiscAmt = Math.Round(((_dNSaleAmt * dDiscPer) / 100), 0);
                            dgrdSummary.Rows[_rowIndex].Cells["disAmt"].Value = dDiscAmt;

                            if (dDiscAmt > 0)
                                dNetDiscountAmt += dDiscAmt;

                            dTNetSaleAmt += dSAmt;

                            _rowIndex++;
                        }
                    }
                    else
                    {
                        dgrdDetailView.Rows.Add(dt.Rows.Count);
                        double dSAmt = 0, dDiscPer = 0, dDiscAmt = 0, dTotalSaleAmt = 0, dBillValue = 0;
                        int _rowIndex = 0;
                        string strNickName = "", strOldNickName = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            dTSaleAmt += dSAmt = dba.ConvertObjectToDouble(row["Amt"]);
                            strNickName = Convert.ToString(row["SalesParty"]);
                            if (strNickName != strOldNickName && txtScheme.Text != "TOUR")
                            {
                                dTotalSaleAmt = SumSaleAmt(dt, strNickName);
                                dDiscPer = GetDisPer(dTotalSaleAmt);
                                strOldNickName = strNickName;
                            }

                            dDiscAmt = (dSAmt * dDiscPer) / 100;
                            if (dDiscAmt > 0)
                                dNetDiscountAmt += dDiscAmt;

                            dgrdDetailView.Rows[_rowIndex].Cells["dSNo"].Value = (_rowIndex + 1) + ".";
                            dgrdDetailView.Rows[_rowIndex].Cells["dSalesParty"].Value = row["SalesParty"];
                            dgrdDetailView.Rows[_rowIndex].Cells["dPurchaseParty"].Value = row["PurchaseParty"];
                            dgrdDetailView.Rows[_rowIndex].Cells["dPurchaseSNo"].Value = row["ReceiptCode"] + " " + row["ReceiptNo"];
                            dgrdDetailView.Rows[_rowIndex].Cells["dSaleAmt"].Value = dSAmt;
                            dgrdDetailView.Rows[_rowIndex].Cells["dDisPer"].Value = dDiscPer;
                            dgrdDetailView.Rows[_rowIndex].Cells["dDisAmt"].Value = dDiscAmt;

                            if (dt.Columns.Contains("SalePartyID"))
                                dgrdDetailView.Rows[_rowIndex].Cells["dSalePartyID"].Value = row["salePartyID"];

                            if (txtScheme.Text.Contains("TOUR"))
                            {
                                dBillValue = dba.ConvertObjectToDouble(row["BillValue"]);
                                dgrdDetailView.Rows[_rowIndex].Cells["billValue"].Value = dBillValue;

                                if (dBillValue == 1)
                                    dgrdDetailView.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                            }

                            _rowIndex++;
                        }
                        dTNetSaleAmt = dTSaleAmt;

                    }
                }

                if (!txtScheme.Text.Contains("TOUR"))
                    dgrdDetailView.Columns["billValue"].Visible = false;

                lblSaleAmt.Text = dTSaleAmt.ToString("N2", MainPage.indianCurancy);
                lblOrderAmt.Text = dTOAmt.ToString("N2", MainPage.indianCurancy);
                lblNetSaleAmt.Text = dTNetSaleAmt.ToString("N2", MainPage.indianCurancy);
                lblNetDiscAmt.Text = dNetDiscountAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double SumSaleAmt(DataTable _dt, string strName)
        {
            object objSaleAmt = _dt.Compute("SUM(Amt)", "SalesParty='"+ strName+"' ");
            return dba.ConvertObjectToDouble(objSaleAmt);
        }

        private void rdoSummary_CheckedChanged(object sender, EventArgs e)
        {
            dgrdSummary.Visible = rdoSummary.Checked;
        }

        private void rdoDetailView_CheckedChanged(object sender, EventArgs e)
        {
            dgrdDetailView.Visible = rdoDetailView.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetailView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex==4)
                {
                    ShowGoodsReceivePage();
                }
            }
            catch { }
        }

        private void dgrdDetailView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetailView.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetailView.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetailView.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetailView.CurrentCell.ColumnIndex == 2 && dgrdDetailView.CurrentCell.RowIndex >= 0)
                    {
                        ShowGoodsReceivePage();
                    }
                }
            }
            catch { }
        }

        private void ShowGoodsReceivePage()
        {
            string strAllGRSNo = Convert.ToString(dgrdDetailView.CurrentRow.Cells["dPurchaseSNo"].Value);
            string[] strGRSNo = strAllGRSNo.Split(' ');
            if (strGRSNo.Length > 1)
            {
                if (strGRSNo[0] != "" && strGRSNo[1] != "")
                {
                    if (strGRSNo[0].Contains("PTN") || strGRSNo[0].Contains("19-20") || strGRSNo[0].Contains("20-21"))
                    {
                        SaleBook_Trading objSaleBook_Retail = new SaleBook_Trading(strGRSNo[0], strGRSNo[1]);
                        objSaleBook_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleBook_Retail.ShowInTaskbar = true;
                        objSaleBook_Retail.Show();
                    }
                    else
                    {
                        GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strGRSNo[0], strGRSNo[1]);
                        objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objGoodsReciept.ShowInTaskbar = true;
                        objGoodsReciept.Show();
                    }
                }
            }
        }

        private void txtSupplierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTYNICKNAME", "SEARCH SUPPLIER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSupplierName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdDetailView_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetailView.Rows)
                {
                    row.Cells["dSNo"].Value = _index + ".";
                    _index++;
                }
            }
            catch { }
        }

        private void dgrdSummary_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdSummary.Rows)
                {
                    row.Cells["sSNo"].Value = _index + ".";
                    _index++;
                }
            }
            catch { }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetailView.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdDetailView.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetailView.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDetailView.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetailView.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetailView.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDetailView.Columns.Count; l++)
                        {
                            if (dgrdDetailView.Columns[l].HeaderText == "" || !dgrdDetailView.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdDetailView.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetailView.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Fair_Details";
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

        private void FairDetails_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetailView);
            dba.EnableCopyOnClipBoard(dgrdSummary);
        }
    }
}
