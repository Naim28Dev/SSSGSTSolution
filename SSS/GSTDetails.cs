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
    public partial class GSTDetails : Form
    {
        public string strRegion = "", strTaxPer = "", strTaxType = "",strDealerType="";
        DataBaseAccess dba;
        public GSTDetails()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
           // GetDataTableFromDB();
           

        }

        public void ShowRecord()
        {
            string strType = "SALES";
            if (strTaxType == "INPUT")
                strType = "PURCHASE";
            lblDetails.Text = strRegion + " " + strType + " @ " + strTaxPer;
            GetDataTableFromDB();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GSTDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strSBillCOde = "", strPBillCode = "", strSRBillCode = "", strPRBillCode = "", strJournalVCode = "", strSaleServiceVCode="";

            if (chkDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strSubQuery += " and SR.BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }
            if (txtMonth.Text != "")
            {
                strSubQuery += " and UPPER(DATENAME(MM,SR.BillDate))='" + txtMonth.Text + "' ";
            }
            if (txtStateName.Text != "")
            {
                strSBillCOde = " and SR.BillCode in (Select SBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPBillCode = " and SR.BillCode in (Select PBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSRBillCode = " and SR.BillCode in (Select GReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPRBillCode = " and SR.BillCode in (Select PurchaseReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strJournalVCode = " and SR.VoucherCode in (Select JournalVCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSaleServiceVCode = " and SR.BillCode in (Select SaleServiceCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";

            }

            if (strDealerType == "UN-REGISTERED DEALER")
            {
                strQuery += " Select BillType,BillDate,BillNo,PartyName,'LOCAL' as Region,Taxrate,ROUND(SUM(Amount),2) Amount,0 as TaxAmt from ( Select BillType,BillDate,BillNo,PartyName, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                         + " Select 'PURCHASE' BillType,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) as BillNo,PR.BillDate,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName, '' as HSNCode, GRD.Quantity, ROUND(((((GRD.Amount) * (100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - 3))) / 100.00) + (GRD.PackingAmt + GRD.FreightAmt)), 2)Amount,0 as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                         + " SELECT 'PURCHASE' BillType,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) as BillNo,PR.BillDate,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName,'' as HSNCode,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as MOney)) as Amount,0 as TaxRate  from PurchaseRecord PR left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))   Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                         + " Select Distinct 'JOURNAL' BillType,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)) as BillNo,InvoiceDate as BillDate,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName,'' HSNCode,0 as Quantity,DiffAmt as Amount,0 as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where SM.GroupII = 'UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','CR. NOTE RECEIVED AGAINST PURCHASE','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.", "InvoiceDate") + strJournalVCode.Replace("SR.", "JVD.") + "  "
                         + " )_Sales Group by BillType, BillNo, HSNCode, TaxRate, BillDate, PartyName )_Sales Group by BillType, BillNo, Taxrate, PartyName, BillDate Order by BillDate  ";
            }
            else if (strTaxType== "AGAINSTRCM")
            {
                strQuery += " Select BillType,BillDate,BillNo,PartyName,'LOCAL' as Region,Taxrate,ROUND(SUM(Amount),2) Amount,0 as TaxAmt from ( Select BillType, BillDate, BillNo, PartyName, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                         + " Select Distinct 'JOURNAL' BillType, (JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)) as BillNo, InvoiceDate as BillDate,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName, '' HSNCode, 0 as Quantity, DiffAmt as Amount, GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where  BA.GSTNature in ('CONSOLIDATED RCM PAYBLE') " + strSubQuery.Replace("SR.", "InvoiceDate") + strJournalVCode.Replace("SR.", "JVD.") + ")_RCM Group by BillType, BillNo, HSNCode, BillDate, PartyName, TaxRate )_RCM Group by BillType, BillNo, BillDate, PartyName, TaxRate ";
            }
            else if (strTaxType == "INPUT")
            {
                strQuery += " Select BillType,BillDate,BillNo,PartyName,Region,Taxrate,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from (  "
                         + " Select BillType,BillDate,BillNo,PartyName, Region, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                         + " Select 'PURCHASE' BillType,PR.BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - 3)))/ 100.00),2)Amount,GM.TaxRate from PurchaseRecord PR inner join  GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE'  left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - 3))) / 100.00)/ GRD.Quantity)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end))*(100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - 3)))/ 100.00)/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='UNAUTHORISED' and GRD.Amount > 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                         + " Select 'PURCHASE' BillType, PR.BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Where SM.GroupII!='UNAUTHORISED' and (GRD.PackingAmt + GRD.FreightAmt) > 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + "  Union All "
                         + " Select 'PURCHASE' BillType,PR.BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as MOney)) as Amount,PR.TaxPer as TaxRate  from PurchaseRecord PR left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE'  left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Where SM.GroupII!='UNAUTHORISED' and  PR.BillNo != 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " UNION ALL "
                         + " Select 'SALERETURN' as BillType,PR.Date as BillDate, (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))))/ 100.00),2)Amount,GM.TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)))) / 100.00)/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end))*(100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))))/ 100.00)/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='UNAUTHORISED' and GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  Union All   "
                         + " Select 'SALERETURN' as BillType,BillDate, BillNo,PartyName,Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer TaxRate from (Select PR.Date as BillDate, (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,dbo.GetFullName(PR.SalePartyID) PartyName,Region,PR.TaxPer, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2) Amount, (SRD.DisStatus + CAST(SRD.Discount as varchar)) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty from SaleReturn PR inner join SaleReturnDetails SRD on PR.BillCode = SRD.BillCode and PR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)) * (100 + (SRD.DisStatus + CAST(SRD.Discount as varchar)))) / 100)/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))*(100 + (SRD.DisStatus + CAST(SRD.Discount as varchar))))/ 100)/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where PR.BillCode != '' and(SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00)) > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)) * (100 + (_SAles.DisStatus))) / 100) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))*(100 + (_SAles.DisStatus)))/ 100)/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  UNION ALL   "
                         + " Select 'SALERETURN' as BillType,PR.Date as BillDate, (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,dbo.GetFullName(PR.SalePartyID) PartyName,Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where SM.GroupII!='UNAUTHORISED' and (GRD.Packing + GRD.Freight) > 0   " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  UNION  ALL "
                         + " Select Distinct 'JOURNAL' BillType,InvoiceDate,(JVD.VoucherCode+' '+CAST(JVD.VoucherNo as varchar)) BillNo,(SM.AreaCode + CAST(SM.Accountno as varchar)+' '+SM.Name) PartyName,Region,'' HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Where SM.GroupII!='UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','CR. NOTE RECEIVED AGAINST PURCHASE','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "InvoiceDate") + strJournalVCode.Replace("SR.", "JVD.") + " "
                         + " )_Sales Group by BillType,BillDate,BillNo,PartyName, Region, HSNCode, TaxRate )_Sales Where TaxRate =" + strTaxPer + " and Region = '" + strRegion + "' Group by BillType,BillNo,Taxrate,PartyName, Region,BillDate Order by BillDate";
            }
            //else if (strTaxType == "NOPURCHASE")
            //{
            //    strQuery += " Select 'NOPURCHASE' BillType,BillDate,BillNo,PartyName,Region,Taxrate,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from ( " 
            //             + " Select GR.ReceivingDate as BillDate,SE.GRSNo BIllNo, dbo.GetFullName(GR.PurchasePartyID) PartyName,CS.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) GrossAmt, (ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) +((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) *(CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo Left Join SupplierMaster SM on SE.PurchasePartyID = (SM.AreaCode + SM.AccountNo) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  inner join GoodsReceive GR on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo Outer APPLY (Select TOP 1 (CASE WHEN SM.State = CD.StateName then 'LOCAL' else 'INTERSTATE' end)Region from CompanyDetails CD) CS  Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) * (CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100)) / GRD.Quantity) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) *(CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100))/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0   and GRD.Amount > 0  and SE.PurchaseBill = 'PENDING' " + strSubQuery.Replace("SR.BillDate", "GR.ReceivingDate") + " Union ALL "
            //             + " Select GR.ReceivingDate as BillDate,SE.GRSNo BIllNo, dbo.GetFullName(GR.PurchasePartyID) PartyName,CS.Region, '' as HSNCode, (GRD.PackingAmt)GrossAmt, (GRD.PackingAmt* (CASE WHen SMN.TaxIncluded = 1 then((100 + GM.TaxRate) / 100) else 1 end)) Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo Left Join SupplierMaster SM on SE.PurchasePartyID = (SM.AreaCode + SM.AccountNo) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  inner join GoodsReceive GR on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo Outer Apply (Select TOP 1 (CASE WHEN SM.State = CD.StateName then 'LOCAL' else 'INTERSTATE' end)Region from CompanyDetails CD) CS Outer APPLY(Select MAX(TaxRate) TaxRate from (Select ((CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) * (CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100)) / GRD.Quantity) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) *(CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100))/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName)_TAX ) as GM Where SR.BillCode != '' and SR.BillNo != 0 and GRD.PackingAmt > 0 and SE.PurchaseBill = 'PENDING' " + strSubQuery.Replace("SR.BillDate", "GR.ReceivingDate") + " UNION ALL "
            //             + " Select GR.ReceivingDate as BillDate,SE.GRSNo BIllNo, dbo.GetFullName(GR.PurchasePartyID) PartyName,CS.Region, '' as HSNCode, (GRD.FreightAmt)GrossAmt, (GRD.FreightAmt* (CASE WHen SMN.TaxIncluded = 1 then((100 + GM.TaxRate) / 100) else 1 end)) Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo Left Join SupplierMaster SM on SE.PurchasePartyID = (SM.AreaCode + SM.AccountNo) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  inner join GoodsReceive GR on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo Outer Apply (Select TOP 1 (CASE WHEN SM.State = CD.StateName then 'LOCAL' else 'INTERSTATE' end)Region from CompanyDetails CD) CS Outer APPLY(Select MAX(TaxRate) TaxRate from (Select ((CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) * (CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100)) / GRD.Quantity) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN(((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end), 2) + ((ROUND((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end),2) *(CAST((SE.DiscountStatus + SE.Discount) as Money) - 3)) / 100))/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName)_TAX ) as GM Where SR.BillCode != '' and SR.BillNo != 0 and GRD.FreightAmt > 0 and SE.PurchaseBill = 'PENDING' " + strSubQuery.Replace("SR.BillDate", "GR.ReceivingDate") + " "
            //             + " )_SAle Where TaxRate =" + strTaxPer + " and Region = '" + strRegion + "' Group by BillDate,Region,BIllNo,PartyName,TaxRate Order by BillDate, BillNo ";
            //}
            else
            {
                strQuery += " Select BillType,BillDate, BillNo,PartyName,Region,Taxrate,ROUND(SUM(Amount), 2) Amount,SUM(ROUND(((Amount * TaxRate) / 100.00), 2)) TaxAmt from ( "
                         + " Select  BillType,BillDate,BillNo,PartyName, Region, HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount from ( "
                         + " Select BillType,BillDate,BillNo,PartyName, Region, HSNCode, SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate, TaxType from(Select 'SALES' BillType,SR.BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,dbo.GetFullName(SR.SalePartyID) PartyName, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (SE.DiscountStatus + SE.Discount))) / 100) / GRD.Quantity) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end))*(100 + (SE.DiscountStatus + SE.Discount)))/ 100)/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0  and GRD.Amount > 0 " + strSubQuery + strSBillCOde + " Union All "
                         + " Select 'SALES' BillType,BillDate,BillNo,PartyName,Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer TaxRate, TaxType from(Select SR.BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,dbo.GetFullName(SR.SalePartyID) PartyName, SR.TaxPer, SMN.Region, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((GRD.Amount * 100) / (100 + GM.TaxRate)) else GRD.Amount end), 2) Amount, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (SE.DiscountStatus + SE.Discount))) / 100)/ GRD.Quantity)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end))*(100 + (SE.DiscountStatus + SE.Discount)))/ 100)/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' and SR.BillNo != 0  and GRD.Amount > 0 " + strSubQuery + strSBillCOde + " )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)) * (100 + (_SAles.DisStatus))) / 100) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))*(100 + (_SAles.DisStatus)))/ 100)/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                         + " Select 'SALES' BillType,SR.BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,dbo.GetFullName(SR.SalePartyID) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) + ((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara / 100)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select TOP 1 FreightDhara from CompanySetting) CS Where SR.BillCode != '' and SR.BillNo != 0 and (GRD.PackingAmt + GRD.FreightAmt) > 0 " + strSubQuery + strSBillCOde + "  Union All "
                         + " Select 'SALES' BillType,SR.BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,dbo.GetFullName(SR.SalePartyID) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and SR.BillNo != 0 " + strSubQuery + strSBillCOde + "  UNION ALL "
                         + " Select 'PURCHASERETURN' BillType,BillDate,BillNo,PartyName, Region, HSNCode, SUM(Quantity) Qty,SUM(Amount)Amount, TaxRate,TaxType  from ( Select PR.Date as BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar))BillNo,(PR.PurchasePartyID+' '+SM.Name) PartyName, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, ROUND((((GRD.Amount) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))))/ 100.00),2)Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)))) / 100.00)/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end))*(100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))))/ 100.00)/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='UNAUTHORISED' and GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.","PR.") + " )_PR Group by BillDate,BillNo,PartyName, Region, HSNCode,TaxRate,TaxType Union All   "
                         + " Select 'PURCHASERETURN' BillType,PR.Date as BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar))BillNo,(PR.PurchasePartyID+' '+SM.Name) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where SM.GroupII!='UNAUTHORISED' and (GRD.Packing + GRD.Freight) > 0   " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + "   Union All    "
                         + " SELECT 'PURCHASERETURN' BillType,PR.Date as BillDate,(PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar))BillNo,(PR.PurchasePartyID+' '+SM.Name) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)) as Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType  from PurchaseReturn PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   Where SM.GroupII!='UNAUTHORISED' and PR.BillNo!=0   " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + " UNION ALL "
                         + " Select 'SALESERVICE' BillType,BillDate,BillNo,PartyName, Region, HSNCode, SUM(Quantity) Qty,SUM(Amount)Amount, TaxRate,TaxType  from ( "
                         + " Select SR.Date as BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,(SR.SalePartyID + ' ' + SM.Name) PartyName, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, 1 as Quantity, ROUND((((SRD.Amount))), 2)Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from SaleServiceBook SR inner join SaleServiceDetails SRD on SR.BIllCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII != 'UNAUTHORISED' and SRD.Amount > 0  " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + "  )_PR Group by BillDate, BillNo, PartyName, Region, HSNCode, TaxRate, TaxType Union All "
                         + " SELECT 'SALESERVICE' BillType,SR.Date as BillDate,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar))BillNo,(SR.SalePartyID + ' ' + SM.Name) PartyName,SMN.Region,'' as HSNCode,0 as Quantity,(CAST((OtherSign + CAST(OtherAmt as varchar)) as Money)) as Amount,SR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType  from SaleServiceBook SR left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))   Where SM.GroupII != 'UNAUTHORISED' and SR.BillNo != 0   " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + " "
                         + " )_Sales Group by BillType,BillDate,HSNCode,PartyName, TaxRate, TaxType, BillNo, Region)_Sales )_Sales Where TaxRate =" + strTaxPer + " and Region = '" + strRegion + "' Group by  BillType,BillDate,BillNo,PartyName,TaxRate, Region Order by BillDate ";

            }
            return strQuery;
        }

        private void GetDataTableFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                DataTable dt = dba.GetDataTable(strQuery);
                int _rowIndex = 0;
                dgrdDetails.Rows.Clear();
                double dAmt = 0, dTAmt = 0, dTaxAmt = 0, dTTaxAmt = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    if (strRegion == "LOCAL")
                    {
                        dgrdDetails.Columns["igstAmt"].Visible = false;
                        dgrdDetails.Columns["cgstAmt"].Visible = dgrdDetails.Columns["sgstAmt"].Visible = true;
                    }
                    else
                    {
                        dgrdDetails.Columns["igstAmt"].Visible = true;
                        dgrdDetails.Columns["cgstAmt"].Visible = dgrdDetails.Columns["sgstAmt"].Visible = false;
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                        dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(row["TaxAmt"]);

                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = row["BillDate"];
                        dgrdDetails.Rows[_rowIndex].Cells["vchType"].Value = row["BillType"];
                        dgrdDetails.Rows[_rowIndex].Cells["serialNo"].Value = row["BillNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["partyName"].Value = row["PartyName"];
                        dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                        if (strRegion == "LOCAL")
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                        }
                        else
                            dgrdDetails.Rows[_rowIndex].Cells["igstAmt"].Value = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                        _rowIndex++;
                    }
                }

                lblNetAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblTotalTaxAmt.Text = dTTaxAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }


        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    ShowDetails();
                }
            }
            catch { }
        }

        private void ShowDetails()
        {
            try
            {
                string strVchType = Convert.ToString(dgrdDetails.CurrentRow.Cells["vchType"].Value);
                string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["serialNo"].Value);
                string[] strNumber = strInvoiceNo.Split(' ');
                if (strNumber.Length > 1)
                {
                    if (strVchType == "PURCHASE")
                    {
                        ShowPurchaseBook(strNumber[0], strNumber[1]);
                    }
                    else if (strVchType == "SALES")
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                    else if (strVchType == "NOPURCHASE")
                    {
                        ShowGoodsReceive(strNumber[0], strNumber[1]);
                    }
                    else if (strVchType == "JOURNAL")
                    {
                        ShowJournalDetails(strNumber[0], strNumber[1]);
                    }
                    else if (strVchType == "SALERETURN")
                    {
                        ShowSaleReturnBook(strNumber[0], strNumber[1]);
                    }
                    else if (strVchType == "PURCHASERETURN")
                    {
                        ShowPurchaseReturn(strNumber[0], strNumber[1]);
                    }
                    else if(strVchType== "SALESERVICE")
                    {
                        ShowSaleServiceBook(strNumber[0], strNumber[1]);
                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetails.CurrentRow.Index >= 0 && dgrdDetails.CurrentCell.ColumnIndex >= 0)
                    {
                        ShowDetails();
                    }
                }
            }
            catch { }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            GetDataTableFromDB();
            btnGo.Enabled = true;
        }

        private void ShowPurchaseBook(string strCode, string strBillNo)
        {
            PurchaseBook objPurchaseBook = new PurchaseBook(strCode, strBillNo);
            objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurchaseBook.ShowInTaskbar = true;
            objPurchaseBook.Show();
        }
        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            dgrdDetails.Rows.Clear();
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
        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonth.Text = objSearch.strSelectedData;
                    ClearAllRecord();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                dba.ShowSaleBookPrint(strCode, strBillNo,false, false);
            }
            else
            {
                SaleBook objSale = new SaleBook(strCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.Show();
            }
        }

        private void ShowSaleServiceBook(string strCode, string strBillNo)
        {
            SaleServiceBook objSale = new SaleServiceBook(strCode, strBillNo);
            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSale.ShowInTaskbar = true;
            objSale.Show();
        }

        private void ShowSaleReturnBook(string strCode, string strBillNo)
        {
            SaleReturn objSale = new SaleReturn(strCode, strBillNo);
            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSale.ShowInTaskbar = true;
            objSale.Show();
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", Keys.Space);
                objSearch.ShowDialog();
                txtMonth.Text = objSearch.strSelectedData;
                ClearAllRecord();

            }
            catch
            {
            }
        }

        private void txtStateName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStateName.Text = objSearch.strSelectedData;
                    ClearAllRecord();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearAllRecord()
        {
            dgrdDetails.Rows.Clear();
            lblNetAmt.Text = lblTotalTaxAmt.Text = "0.00";
        }

        private void btnStateName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", Keys.Space);
                objSearch.ShowDialog();
                txtStateName.Text = objSearch.strSelectedData;
                ClearAllRecord();
            }
            catch
            {
            }
        }

        private void ShowGoodsReceive(string strCode, string strBillNo)
        {
            GoodsReceipt objGoodsReciept = new GoodsReceipt(strCode, strBillNo);
            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objGoodsReciept.ShowInTaskbar = true;
            objGoodsReciept.Show();
        }

        private void ShowJournalDetails(string strCode, string strBillNo)
        {
            JournalEntry_New objGoodsReciept = new JournalEntry_New(strCode, strBillNo);
            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objGoodsReciept.ShowInTaskbar = true;
            objGoodsReciept.Show();
        }

        private void ShowPurchaseReturn(string strCode, string strBillNo)
        {
            PurchaseReturn objPurchaseReturn = new PurchaseReturn(strCode, strBillNo);
            objPurchaseReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurchaseReturn.ShowInTaskbar = true;
            objPurchaseReturn.Show();
        }

    }
}
