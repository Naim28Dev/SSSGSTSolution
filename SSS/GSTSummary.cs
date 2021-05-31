using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SSS
{
    public partial class GSTSummary : Form
    {
        DataBaseAccess dba;
        public GSTSummary()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
           
            //GetDataTableFromDB();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GSTSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");

        }

        private string CreateQuery_New()
        {
            string strQuery = "", strSubQuery = "", strSBillCode = "", strPBillCode = "", strSRBillCode = "", strPRBillCode = "", strJournalVCode = "", strSaleServiceVCode = "";

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
                strSBillCode = " and SR.BillCode in (Select SBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPBillCode = " and SR.BillCode in (Select PBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSRBillCode = " and SR.BillCode in (Select GReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPRBillCode = " and SR.BillCode in (Select PurchaseReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strJournalVCode = " and SR.VoucherCode in (Select JournalVCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSaleServiceVCode = " and SR.BillCode in (Select SaleServiceCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
            }

            strQuery = " "
                       //+ " Select * from (Select 0 as BillType,Region,Taxrate,('@ '+CAST(Taxrate as nvarchar)+'% ' + Region) Detail,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from ( "

                       + " Select  'PURCHASE' as BillType,SUM(Amount)Amount,SUM(CASE WHEN Region='INTERSTATE' then ((Amount*TaxRate)/100.00) else 0 end) IGST,SUM(CASE WHEN Region<>'INTERSTATE' then ((Amount*TaxRate)/100.00)/2 else 0 end) CGST from(  "
                       + " Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100.00- GR.DisPer))/ 100.00),2)Amount,GM.TaxRate from PurchaseRecord PR inner join  GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  inner join GoodsReceive GR on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GR.DisPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GR.DisPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "   // SM.GroupII!='UNAUTHORISED' and
                       + " Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100.00- PR.DiscPer))/ 100.00),2)Amount,GM.TaxRate from PurchaseBook PR inner join  PurchaseBookSecondary GRD on PR.BillCode = GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100.00) / (100.00 + TaxRate)) else GRD.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((ISNULL(DisStatus,'-')+CAST(SDisPer as varchar)) as Money))-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-(PR.DiscPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100) / (100 + TaxRate)) else GRD.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((ISNULL(DisStatus,'-')+CAST(SDisPer as varchar)) as Money))-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-(PR.DiscPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All"  // SM.GroupII!='UNAUTHORISED' and
                       + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where  (GRD.PackingAmt + GRD.FreightAmt) > 0  " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All " //SM.GroupII!='UNAUTHORISED' and
                       + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.OCharges-GRD.Discount) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseBook PR inner join PurchaseBookSecondary GRD on PR.BillCode = GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType =  SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where (GRD.OCharges-GRD.Discount) != 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All " // SM.GroupII!='UNAUTHORISED' and
                       + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as MOney)) as Amount,PR.TaxPer as TaxRate  from PurchaseRecord PR left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   Where  PR.BillNo!=0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All " // SM.GroupII!='UNAUTHORISED' and
                       + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(PR.PackingAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money))) as Amount,PR.TaxPer as TaxRate  from PurchaseBook PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   Where PR.BillNo!=0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All " // SM.GroupII!='UNAUTHORISED' and
                       + " Select Distinct JVD.VoucherNo as BillNo,Region,ISNULL(GM.HSNCode,'') HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM  Where  BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + " )Purchase" // SM.GroupII!='UNAUTHORISED' and

                      + " Select  'PURCHASERTURN' as BillType,SUM(Amount)Amount,SUM(CASE WHEN Region='INTERSTATE' then ((Amount*TaxRate)/100.00) else 0 end) IGST,SUM(CASE WHEN Region<>'INTERSTATE' then ((Amount*TaxRate)/100.00)/2 else 0 end) CGST from( Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, (ROUND((((GRD.Amount) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))+(CAST(OtherValue as Money))))/ 100.00),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)+(CAST(OtherValue as Money)))) / 100.00) else 1.00 end))/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)+(CAST(OtherValue as Money)))) / 100.00) else 1.00 end))/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS Where GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + "  Union All  " // SM.GroupII!='UNAUTHORISED' and
                      + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS Where  (GRD.Packing + GRD.Freight) <> 0   " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + " Union All  "  // SM.GroupII!='UNAUTHORISED' and
                      + " Select Distinct JVD.VoucherNo as BillNo,Region,ISNULL(GM.HSNCode,'') HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate,0 as TaxType from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM  Where  BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + "  Union All " // SM.GroupII!='UNAUTHORISED' and
                      + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,((CAST((OtherSign+CAST(OtherAmt as varchar)) as Money))*ISNULL(CS.DebitNoteStatus,1)) as Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType  from PurchaseReturn PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS  Where PR.OtherAmt!=0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + ")_SaleReturn " // SM.GroupII!='UNAUTHORISED' and

                      + " Select  'RCM' as BillType,SUM(Amount)Amount,SUM(CASE WHEN Region='INTERSTATE' then ((Amount*TaxRate)/100.00) else 0 end) IGST,SUM(CASE WHEN Region<>'INTERSTATE' then ((Amount*TaxRate)/100.00)/2 else 0 end) CGST from(  "
                      + " Select Distinct JVD.VoucherNo as BillNo,Region,'' HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where BA.GSTNature in ('CONSOLIDATED RCM PAYBLE')  " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + " )_RCM "//Group by BillNo, Region, HSNCode, TaxRate )_RCM Group by Taxrate, Region 

                      + " Select  'SALES' as BillType,SUM(Amount)Amount,SUM(CASE WHEN Region='INTERSTATE' then ((Amount*TaxRate)/100.00) else 0 end) IGST,SUM(CASE WHEN Region<>'INTERSTATE' then ((Amount*TaxRate)/100.00)/2 else 0 end) CGST from(  "
                      + " Select BillNo, Region, HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1  and Qty!=0  then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount from ( "
                      + " Select BillNo, Region, HSNCode, SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate, TaxType from ( Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0  and GRD.Amount > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, SE.Qty as Quantity, SE.Amount as Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SpecialDscPer) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0  and SE.MRP>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union All "
                      + " Select BillNo, Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100),2)Amount, TaxPer TaxRate, TaxType from(Select SR.BillNo, SR.TaxPer, SMN.Region, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((GRD.Amount * 100) / (100 + GM.TaxRate)) else GRD.Amount end), 2) Amount, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty,SR.BillCode,SR.BillDate from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then ((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' and SR.BillNo != 0 and SR.ServiceAmount!=0 and GRD.Amount > 0 " + strSubQuery + strSBillCode + "  )_SAles OUTER APPLY(Select CAST(TaxDhara as bigint)+(CASE WHEN _Sales.BillCode like('%CCK%') and BillDate<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere SBillCode=_Sales.BillCode) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)) + ((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara / 100)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select CAST(FreightDhara as bigint)+(CASE WHEN SR.BillCode like('%CCK%') and SR.BillDate<'09/01/2019' then 1 else 0 end) FreightDhara from CompanySetting WHere SBillCode=SR.BillCode) CS Where SR.BillCode != '' and SR.BillNo != 0 and (GRD.PackingAmt + GRD.FreightAmt) > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((((SE.OCharges-SE.Disc) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end))), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Where SR.BillCode != '' and SR.BillNo != 0 and (SE.Disc+SE.OCharges) != 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(GRD.TaxAmt,2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Where SR.BillCode != '' and SR.BillNo != 0 and GRD.TaxAmt > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2)  Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and SR.BillNo != 0 " + strSubQuery + strSBillCode + "  UNION ALL "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((SR.PackingAmt + SR.PostageAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)) + (CAST((Description+CAST(DisAmt as varchar)) as Money)) + CAST(ISNULL(SR.GreenTax, 0) as money)), 2)  Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesBook SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and SR.BillNo != 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  UNION ALL "
                      + " Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, 1 as Quantity, ROUND((SRD.Amount), 2)Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SaleServiceBook SR inner join SaleServiceDetails SRD on SR.BIllCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where  SRD.Amount > 0  " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + "  Union All "
                      + " Select SR.BillNo, SMN.Region, '' as HSNCode, 0 as Quantity, ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)),2) Amount,SR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType from SaleServiceBook SR left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money) > 0 " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + ")_Sales Group by BillNo,Region, HSNCode,TaxRate,TaxType)_Sales UNION ALL"

                      + " Select BillNo,Region, HSNCode, SUM(Quantity) Qty,-SUM(Amount)Amount,  TaxRate from(Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, (ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)))/ 100.00),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,GM.TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName) as GM OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS Where GRD.Amount > 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  Union All  "
                      + " Select BillNo,Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer TaxRate from(Select PR.BillNo, PR.TaxPer, SMN.Region, (ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2)*ISNULL(CS.DebitNoteStatus,1)) Amount, (CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty,PR.BillCode,PR.Date from SaleReturn PR inner join SaleReturnDetails SRD on PR.BillCode = SRD.BillCode and PR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS Where PR.BillCode != '' and SRD.Amount > 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  and PR.ServiceAmt>0  )_SAles OUTER APPLY(Select CAST(TaxDhara as bigint)+(CASE WHEN _Sales.BillCode like('%CCK%') and _Sales.Date<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere GReturnCode=_Sales.BillCode) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + (_SAles.DisStatus) / 100.00)) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + (_SAles.DisStatus) / 100.00)) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  UNION ALL  "
                      + " Select SR.BillNo, SMN.Region, '' as HSNCode, 0 as Quantity, (ROUND(((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+PackingAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,SR.TaxPer as TaxRate from SaleReturn SR left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=SR.BillCode) CS  Where (CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+PackingAmt) <> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode.Replace("SR.", "SR.") + "   UNION ALL  "
                      + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,PR.TaxPer as TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS  Where  (GRD.Packing + GRD.Freight) <> 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + " )_SaleReturn Group by  BillNo,Region, HSNCode,TaxRate )_Sale  ";
                     
                   

            return strQuery;
        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strSBillCode = "", strPBillCode = "", strSRBillCode = "", strPRBillCode = "", strJournalVCode = "", strSaleServiceVCode = "";

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
                strSBillCode = " and SR.BillCode in (Select SBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPBillCode = " and SR.BillCode in (Select PBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSRBillCode = " and SR.BillCode in (Select GReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strPRBillCode = " and SR.BillCode in (Select PurchaseReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strJournalVCode = " and SR.VoucherCode in (Select JournalVCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSaleServiceVCode = " and SR.BillCode in (Select SaleServiceCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
            }

            strQuery = " "
                       + " Select * from (Select 0 as BillType,Region,Taxrate,('@ '+CAST(Taxrate as nvarchar)+'% ' + Region) Detail,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from ( "
                       + " Select BillNo, Region, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                       + " Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100.00- GR.DisPer))/ 100.00),2)Amount,GM.TaxRate from PurchaseRecord PR inner join  GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  inner join GoodsReceive GR on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GR.DisPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GR.DisPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and GRD.Amount > 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100.00- PR.DiscPer))/ 100.00),2)Amount,GM.TaxRate from PurchaseBook PR inner join  PurchaseBookSecondary GRD on PR.BillCode = GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100.00) / (100.00 + TaxRate)) else GRD.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((ISNULL(GRD.DisStatus,'-')+CAST(SDisPer as varchar)) as Money))-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-(PR.DiscPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100) / (100 + TaxRate)) else GRD.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((ISNULL(GRD.DisStatus,'-')+CAST(SDisPer as varchar)) as Money))-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-(PR.DiscPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All"
                       + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and (GRD.PackingAmt + GRD.FreightAmt) > 0  " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.OCharges-GRD.Discount) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseBook PR inner join PurchaseBookSecondary GRD on PR.BillCode = GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType =  SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and (GRD.OCharges-GRD.Discount) != 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as MOney)) as Amount,PR.TaxPer as TaxRate  from PurchaseRecord PR left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!=''and PR.BillNo!=0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,((PR.PackingAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)))* (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)) as Amount,PR.TaxPer as TaxRate  from PurchaseBook PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and PR.BillNo!=0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " Select BillNo,Region, HSNCode, SUM(Quantity) Qty,SUM(Amount)Amount,  TaxRate from(Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, (ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)))/ 100.00),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,GM.TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (GRD.DisStatus + CAST(GRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName) as GM OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and GRD.Amount > 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  Union All  "
                       + " Select BillNo,Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer TaxRate from(Select PR.BillNo, PR.TaxPer, SMN.Region, (ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2)*ISNULL(CS.DebitNoteStatus,1)) Amount, (CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty,PR.BillCode,PR.Date from SaleReturn PR inner join SaleReturnDetails SRD on PR.BillCode = SRD.BillCode and PR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CASE WHEN ISNULL(Dhara,'')!='' then (SRD.DisStatus + CAST(SRD.Discount as varchar)) else 0 end)) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS Where PR.BillCode != '' and SRD.Amount > 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "  and PR.ServiceAmt>0  )_SAles OUTER APPLY(Select CAST(TaxDhara as bigint)+(CASE WHEN _Sales.BillCode like('%CCK%') and _Sales.Date<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere GReturnCode=_Sales.BillCode) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + (_SAles.DisStatus) / 100.00)) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + (_SAles.DisStatus) / 100.00)) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  UNION ALL  "
                       //+ " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  Where SM.GroupII!='UNAUTHORISED' and (GRD.Packing + GRD.Freight) > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + "   UNION ALL  "
                       + " Select SR.BillNo, SMN.Region, '' as HSNCode, 0 as Quantity, (ROUND(((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+PackingAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,SR.TaxPer as TaxRate from SaleReturn SR left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=SR.BillCode) CS  Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' and (CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+PackingAmt) <> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode.Replace("SR.", "SR.") + "   UNION ALL  "
                       + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,PR.TaxPer as TaxRate from SaleReturn PR inner join  SaleReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  PR.SalePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where DebitNoteCode=PR.BillCode) CS  Where SM.GroupII!='UNAUTHORISED'and SM.GroupII!='' and (GRD.Packing + GRD.Freight) <> 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + " )_SaleReturn Group by BillNo, Region, HSNCode, TaxRate UNION ALL"
                       + " Select Distinct JVD.VoucherNo as BillNo,Region,ISNULL(GM.HSNCode,'') HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM  Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + " "
                       + " )_Sales Group by BillNo, Region, HSNCode, TaxRate )_Sales Group by Taxrate, Region UNION ALL "
                       + " Select 1 as BillType,'LOCAL' as Region,Taxrate,'UN-REGISTERED DEALER' as Detail,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from (Select BillNo,HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                       + " Select PR.BillNo, '' as HSNCode, GRD.Quantity, ROUND(((((GRD.Amount) * (100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - 3))) / 100.00) + (GRD.PackingAmt + GRD.FreightAmt)), 2)Amount,0 as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " Select PR.BillNo, '' as HSNCode, GRD.Qty as Quantity, ROUND(((((GRD.Amount) * (100 - PR.DiscPer)) / 100.00) + (GRD.OCharges -GRD.Discount)), 2)Amount,0 as TaxRate from PurchaseBook PR inner join PurchaseBookSecondary GRD on PR.BillCode = GRD.BillCode and PR.BillNo=GRD.BillNo left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where SM.GroupII = 'UNAUTHORISED'  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " SELECT PR.BillNo,'' as HSNCode,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as Money)) as Amount,0 as TaxRate  from PurchaseRecord PR left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))   Where SM.GroupII = 'UNAUTHORISED' and PR.BillNo != 0 " + strSubQuery.Replace("SR.", "PR.") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " SELECT PR.BillNo,'' as HSNCode,0 as Quantity,(PR.PackingAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money))) as Amount,0 as TaxRate  from PurchaseBook PR left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))   Where SM.GroupII = 'UNAUTHORISED' and PR.BillNo != 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPBillCode.Replace("SR.", "PR.") + " Union All "
                       + " Select Distinct JVD.VoucherNo as BillNo,ISNULL(GM.HSNCode,'') HSNCode,0 as Quantity,DiffAmt as Amount,0 as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  OUTER APPLY (Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM  Where SM.GroupII = 'UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + ")_Sales Group by BillNo, HSNCode, TaxRate  )_Sales Group by Taxrate)_Purchase Order by  BillType,Region,Taxrate "

                      + " Select Region,Taxrate,('@ '+CAST(Taxrate as nvarchar)+'% ' + Region) Detail,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount*TaxRate)/100),2)) as TaxAmt from (Select BillNo, Region, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate from( "
                      + " Select Distinct JVD.VoucherNo as BillNo,Region,'' HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where BA.GSTNature in ('CONSOLIDATED RCM PAYBLE')  " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + " )_RCM Group by BillNo, Region, HSNCode, TaxRate )_RCM Group by Taxrate, Region "

                      + " Select Region,Taxrate,('@ ' + CAST(Taxrate as nvarchar) + '% ' + Region) Detail,ROUND(SUM(Amount),2) Amount,SUM(ROUND(((Amount * TaxRate) / 100.00), 2)) TaxAmt from ( "
                      + " Select BillNo, Region, HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1  and Qty!=0  then((Amount * 100) / (100 + TaxRate)) else Amount end),4) Amount from ( "
                      + " Select BillNo, Region, HSNCode, SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate, TaxType from ( Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0  and GRD.Amount > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, SE.Qty as Quantity, SE.Amount as Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SpecialDscPer) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SR.BillNo != 0  and SE.MRP>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union All "
                      + " Select BillNo, Region,'' as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100),2)Amount, TaxPer TaxRate, TaxType from(Select SR.BillNo, SR.TaxPer, SMN.Region, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((GRD.Amount * 100) / (100 + GM.TaxRate)) else GRD.Amount end), 2) Amount, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty,SR.BillCode,SR.BillDate from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then ((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' and SR.BillNo != 0 and SR.ServiceAmount!=0  and GRD.Amount > 0 " + strSubQuery + strSBillCode + "  )_SAles OUTER APPLY(Select CAST(TaxDhara as bigint)+(CASE WHEN _Sales.BillCode like('%CCK%') and BillDate<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere SBillCode=_Sales.BillCode) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)) + ((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara / 100)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select CAST(FreightDhara as bigint)+(CASE WHEN SR.BillCode like('%CCK%') and SR.BillDate<'09/01/2019' then 1 else 0 end) FreightDhara from CompanySetting WHere SBillCode=SR.BillCode) CS Where SR.BillCode != '' and SR.BillNo != 0 and (GRD.PackingAmt + GRD.FreightAmt) > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((((SE.OCharges-SE.Disc) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end))), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Where SR.BillCode != '' and SR.BillNo != 0 and (SE.Disc+SE.OCharges) != 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND(GRD.TaxAmt,2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Where SR.BillCode != '' and SR.BillNo != 0 and GRD.TaxAmt > 0 " + strSubQuery + strSBillCode + "  Union All "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2)  Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and SR.BillNo != 0 " + strSubQuery + strSBillCode + "  UNION ALL "
                      + " Select SR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,ROUND((SR.PackingAmt + SR.PostageAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)) + (CAST((Description+CAST(DisAmt as varchar)) as Money)) + CAST(ISNULL(SR.GreenTax, 0) as money)), 2)  Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType from SalesBook SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and SR.BillNo != 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  UNION ALL "
                      + " Select BillNo, Region, HSNCode,SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate,TaxType  from ( Select PR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, GRD.Qty as Quantity, (ROUND((((GRD.Amount) * (100 + (CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money))+(CAST(OtherValue as Money))))/ 100.00),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,GM.TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)+(CAST(OtherValue as Money)))) / 100.00) else 1.00 end))/ GRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((GRD.DisStatus + CAST(GRD.Discount as varchar)) as Money)+(CAST(OtherValue as Money)))) / 100.00) else 1.00 end))/ GRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + "  Union All  "
                      + " Select PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,(ROUND(((GRD.Packing + GRD.Freight) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2)*ISNULL(CS.DebitNoteStatus,1)) Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType from PurchaseReturn PR inner join  PurchaseReturnDetails GRD on PR.BIllCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))   OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and (GRD.Packing + GRD.Freight) <> 0   " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + " Union All  "
                      + " SELECT PR.BillNo,SMN.Region,'' as HSNCode,0 as Quantity,((CAST((OtherSign+CAST(OtherAmt as varchar)) as Money))*ISNULL(CS.DebitNoteStatus,1)) as Amount,PR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType  from PurchaseReturn PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on  PR.PurchasePartyID in (SM.AreaCode+CAST(SM.Accountno as varchar))  OUTER APPLY (Select Top 1 -1 as DebitNoteStatus from CompanySetting Where CreditNoteCode=PR.BillCode) CS  Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and PR.OtherAmt!=0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strPRBillCode.Replace("SR.", "PR.") + " )_Sales Group by BillNo, Region, HSNCode, TaxRate,TaxType UNION ALL "
                      + " Select Distinct JVD.VoucherNo as BillNo,Region,ISNULL(GM.HSNCode,'') HSNCode,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate,0 as TaxType from JournalVoucherDetails JVD Left join BalanceAmount BA on JVD.VoucherCode=BA.VoucherCode and JVD.VoucherNo=BA.VoucherNo left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select TOP 1 _IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM  Where  BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') and  SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "BA.Date") + strJournalVCode.Replace("SR.", "JVD.") + "  Union All " // SM.GroupII!='UNAUTHORISED' and
                      + " Select BillNo, Region, HSNCode, SUM(Quantity)Qty, SUM(Amount)Amount, TaxRate, TaxType  from( "
                      + " Select SR.BillNo, SMN.Region, (GM.Other + ' : ' + GM.HSNCode) as HSNCode, 1 as Quantity, ROUND((SRD.Amount), 2)Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from SaleServiceBook SR inner join SaleServiceDetails SRD on SR.BIllCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on  SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where SM.GroupII!='' and SRD.Amount > 0  " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + "  Union All "
                      + " Select SR.BillNo, SMN.Region, '' as HSNCode, 0 as Quantity, ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + SR.TaxPer)) else 1 end)),2) Amount,SR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType from SaleServiceBook SR left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' left Join SupplierMaster SM on SR.SalePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where  SM.GroupII!='' and CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money) > 0 " + strSubQuery.Replace("BillDate", "Date") + strSaleServiceVCode + "  )_Sales Group by BillNo, Region, HSNCode, TaxRate, TaxType  "
                      + " )_Sales Group by HSNCode, TaxRate, TaxType, BillNo, Region)_Sales )_Sales Group by TaxRate, Region Order by Taxrate ";

            return strQuery;
        }

        private void GetDataTableFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                double dTaxableValue = 0, dTTaxableValue = 0, dNOIGST = 0, dTNOIGST = 0, dTNOCGST = 0, dIGST = 0, dTIGST = 0, dTCGST = 0, dTIIGST = 0, dTICGST, dNetIGST = 0, dNetCGST = 0, dRCMIGST = 0, dTRCMIGST = 0, dTRCMCGST = 0;
                string strRegion = "";
                int _rowIndex = 0;
                if (ds.Tables.Count > 0)
                {
                    dgrdDetails.Rows.Clear();
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count + 2);
                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "INPUT GST";
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                        _rowIndex++;
                        foreach (DataRow row in dt.Rows)
                        {
                            strRegion = Convert.ToString(row["Region"]);
                            dTTaxableValue += dTaxableValue = dba.ConvertObjectToDouble(row["Amount"]);
                            if (strRegion != "" && dTaxableValue !=0)
                            {
                                dIGST = dba.ConvertObjectToDouble(row["TaxAmt"]);
                                dgrdDetails.Rows[_rowIndex].Cells["details"].Value = row["Detail"];
                                dgrdDetails.Rows[_rowIndex].Cells["taxPer"].Value = row["Taxrate"];
                                dgrdDetails.Rows[_rowIndex].Cells["region"].Value = row["Region"];
                                dgrdDetails.Rows[_rowIndex].Cells["taxType"].Value = "INPUT";
                                dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                                if (strRegion == "LOCAL")
                                {
                                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = "0.00";
                                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                    dTCGST += dIGST;
                                }
                                else
                                {
                                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = "0.00";
                                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = "0.00";

                                    dTIGST += dIGST;
                                }
                                _rowIndex++;
                            }
                        }
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Total";
                        dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTIGST;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = (dTIGST + dTCGST);//.ToString("N2", MainPage.indianCurancy);
                        _rowIndex++;
                    }

                    //dt = ds.Tables[1];
                    //dTTaxableValue = 0;
                    //if (dt.Rows.Count > 0)
                    //{
                    //    dgrdDetails.Rows.Add(dt.Rows.Count + 2);
                    //    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "NO PURCHASE DETAILS ";
                    //    dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                    //    _rowIndex++;
                    //    foreach (DataRow row in dt.Rows)
                    //    {
                    //        strRegion = Convert.ToString(row["Region"]);
                    //        dTTaxableValue += dTaxableValue = dba.ConvertObjectToDouble(row["Amount"]);
                    //        if (strRegion != "" && dTaxableValue > 0)
                    //        {
                    //            dIGST = dba.ConvertObjectToDouble(row["TaxAmt"]);
                    //            dgrdDetails.Rows[_rowIndex].Cells["details"].Value = row["Detail"];
                    //            dgrdDetails.Rows[_rowIndex].Cells["taxPer"].Value = row["Taxrate"];
                    //            dgrdDetails.Rows[_rowIndex].Cells["region"].Value = row["Region"];
                    //            dgrdDetails.Rows[_rowIndex].Cells["taxType"].Value = "NOPURCHASE";
                    //            dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTaxableValue.ToString("N2", MainPage.indianCurancy);
                    //            dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = dIGST.ToString("N2", MainPage.indianCurancy);
                    //            if (strRegion == "LOCAL")
                    //            {
                    //                dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = "0.00";
                    //                dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dIGST / 2).ToString("N2", MainPage.indianCurancy);
                    //                dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dIGST / 2).ToString("N2", MainPage.indianCurancy);
                    //                dTNOCGST += dIGST;
                    //            }
                    //            else
                    //            {
                    //                dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dIGST.ToString("N2", MainPage.indianCurancy);
                    //                dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = "0.00";
                    //                dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = "0.00";

                    //                dTNOIGST += dIGST;
                    //            }
                    //            _rowIndex++;
                    //        }
                    //    }

                    //    dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    //    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Total";
                    //    dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTTaxableValue.ToString("N2", MainPage.indianCurancy);
                    //    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTNOIGST.ToString("N2", MainPage.indianCurancy);
                    //    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTNOCGST / 2).ToString("N2", MainPage.indianCurancy);
                    //    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTNOCGST / 2).ToString("N2", MainPage.indianCurancy);
                    //    dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = (dTNOIGST + dTNOCGST).ToString("N2", MainPage.indianCurancy);
                    //    _rowIndex++;
                    //}

                    dt = ds.Tables[1];
                    dTTaxableValue = 0;
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count + 2);
                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "INPUT AVAILABLE AGAINST RCM ";
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                        _rowIndex++;
                        foreach (DataRow row in dt.Rows)
                        {
                            strRegion = Convert.ToString(row["Region"]);
                            dTTaxableValue += dTaxableValue = dba.ConvertObjectToDouble(row["Amount"]);
                            if (strRegion != "" && dTaxableValue !=0)
                            {
                                dIGST = dba.ConvertObjectToDouble(row["TaxAmt"]);
                                dgrdDetails.Rows[_rowIndex].Cells["details"].Value = row["Detail"];
                                dgrdDetails.Rows[_rowIndex].Cells["taxPer"].Value = row["Taxrate"];
                                dgrdDetails.Rows[_rowIndex].Cells["region"].Value = row["Region"];
                                dgrdDetails.Rows[_rowIndex].Cells["taxType"].Value = "AGAINSTRCM";
                                dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                                if (strRegion == "LOCAL")
                                {
                                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = "0.00";
                                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                    dTRCMCGST += dIGST;
                                }
                                else
                                {
                                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = "0.00";
                                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = "0.00";

                                    dTRCMIGST += dIGST;
                                }
                                _rowIndex++;
                            }
                        }

                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Total";
                        dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTRCMIGST;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTRCMCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTRCMCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = (dTRCMIGST + dTRCMCGST);//.ToString("N2", MainPage.indianCurancy);
                        _rowIndex++;
                    }

                    dt = ds.Tables[2];
                    dTICGST = dTCGST+ dTNOCGST+dTRCMCGST;
                    dTIIGST = dTIGST+ dTNOIGST+ dTRCMIGST;
                    dTCGST = dTIGST = dTTaxableValue = 0;

                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count + 2);

                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "OUTPUT GST";
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                        _rowIndex++;
                        foreach (DataRow row in dt.Rows)
                        {
                            strRegion = Convert.ToString(row["Region"]);
                            dTTaxableValue += dTaxableValue = dba.ConvertObjectToDouble(row["Amount"]);
                            dIGST = dba.ConvertObjectToDouble(row["TaxAmt"]);
                            dgrdDetails.Rows[_rowIndex].Cells["details"].Value = row["Detail"];
                            dgrdDetails.Rows[_rowIndex].Cells["taxPer"].Value = row["Taxrate"];
                            dgrdDetails.Rows[_rowIndex].Cells["region"].Value = row["Region"];
                            dgrdDetails.Rows[_rowIndex].Cells["taxType"].Value = "OUTPUT";
                            dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                            if (strRegion == "LOCAL")
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = "0.00";
                                dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dIGST / 2);//.ToString("N2", MainPage.indianCurancy);
                                dTCGST += dIGST;
                            }
                            else
                            {
                                dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dIGST;//.ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = "0.00";
                                dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = "0.00";

                                dTIGST += dIGST;
                            }
                            _rowIndex++;
                        }
                        dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Total";
                        dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dTTaxableValue;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTIGST;//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = (dTIGST + dTCGST);//.ToString("N2", MainPage.indianCurancy);

                    }

                    dgrdDetails.Rows.Add(8);
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Tax Calculation";
                    dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "INPUT GST";
                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTIIGST;//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTICGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTICGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "OUTPUT GST";
                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dTIGST;//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dTCGST / 2);//.ToString("N2", MainPage.indianCurancy);

                    _rowIndex++;
                    dNetIGST = dTIGST - dTIIGST;
                    dNetCGST = dTCGST - dTICGST;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "GST Payable";
                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = dNetIGST;//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);

                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "Tax Adjustment (Suggested)";
                    dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = "Tax Payable";
                    dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "IGST";
                    dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = dNetIGST;//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["CGST"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["SGST"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["totalTax"].Value = (dNetIGST + dNetCGST);//.ToString("N2", MainPage.indianCurancy);
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "CGST";
                    dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = ((dNetCGST / 2) * -1);//.ToString("N2", MainPage.indianCurancy);
                    _rowIndex++;
                    dgrdDetails.Rows[_rowIndex].Cells["details"].Value = "SGST/UTGST";
                    dgrdDetails.Rows[_rowIndex].Cells["taxableAmt"].Value = (dNetCGST / 2);//.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_rowIndex].Cells["IGST"].Value = ((dNetCGST / 2) * -1);//.ToString("N2", MainPage.indianCurancy);
                }
            }
            catch { }
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
                    dgrdDetails.Rows.Clear();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            GetDataTableFromDB();
            btnGo.Enabled = true;
        }

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    string strTaxPer = Convert.ToString(dgrdDetails.CurrentRow.Cells["taxPer"].Value);
                    if (strTaxPer!="")
                    {
                        OpenGstDetails(strTaxPer);
                    }
                }
            }
            catch { }
        }

        private void OpenGstDetails(string strTaxPer)
        {
            string strRegion = Convert.ToString(dgrdDetails.CurrentRow.Cells["Region"].Value),strTaxType= Convert.ToString(dgrdDetails.CurrentRow.Cells["taxType"].Value),strDetail=Convert.ToString(dgrdDetails.CurrentRow.Cells["details"].Value);
            if (strRegion != "" && strTaxType != "")
            {
                GSTDetails objGST = new SSS.GSTDetails();
                objGST.chkDate.Checked = chkDate.Checked;
                objGST.txtFromDate.Text = txtFromDate.Text;
                objGST.txtToDate.Text = txtToDate.Text;
                objGST.txtMonth.Text = txtMonth.Text;
                objGST.strTaxPer = strTaxPer;
                objGST.strRegion = strRegion;
                objGST.strTaxType = strTaxType;
                objGST.strDealerType = strDetail;
                objGST.txtStateName.Text = txtStateName.Text;
                objGST.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objGST.ShowRecord();
                objGST.ShowDialog();
            }

        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode==Keys.Enter)
                {
                    if(dgrdDetails.CurrentRow.Index>=0 && dgrdDetails.CurrentCell.ColumnIndex>=0)
                    {
                        string strTaxPer = Convert.ToString(dgrdDetails.CurrentRow.Cells["taxPer"].Value);
                        if (strTaxPer != "")
                        {
                            OpenGstDetails(strTaxPer);
                        }
                    }
                }
            }
            catch { }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    CreateNormalExcel();
                }
            }
            catch { }
            btnExport.Enabled = true;
        }

        private string CreateNormalExcel()
        {
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            try
            {
                object misValue = System.Reflection.Missing.Value;
                ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
                ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
                ExcelWorkSheet.Name = "GST_SUMMARY";

                int colIndex = 1;

                ExcelWorkSheet.Cells[1, colIndex] = "GST SUMMARY OF THE MONTH OF : " + txtMonth.Text;
                ExcelWorkSheet.Range["A1:F1"].Merge();
                ExcelWorkSheet.Range["A1:F1"].HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;

                foreach (DataGridViewColumn column in dgrdDetails.Columns)
                {
                    if (colIndex < 7)
                        ExcelWorkSheet.Cells[2, colIndex] = column.HeaderText;
                    else
                        break;
                    colIndex++;
                }

                int _colWidth = 0;
                int columnIndex = 1;
                foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
                {
                    column.HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;
                    _colWidth = dgrdDetails.Columns[columnIndex + 1].Width;
                    if (_colWidth > 149)
                        column.ColumnWidth = (double)column.ColumnWidth + 16;
                    else if (_colWidth > 119)
                        column.ColumnWidth = (double)column.ColumnWidth + 10;
                    else if (_colWidth > 99)
                        column.ColumnWidth = (double)column.ColumnWidth + 7;
                    else if (_colWidth > 50)
                        column.ColumnWidth = (double)column.ColumnWidth;
                    else
                        column.ColumnWidth = (double)column.ColumnWidth - 2;
                    column.RowHeight = 15;

                    if (columnIndex + 1 > colIndex - 1)
                        break;
                    columnIndex++;
                }

                int rowIndex = 3;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    for (int col = 1; col < dgrdDetails.Columns.Count - 2; col++)
                    {
                        ExcelWorkSheet.Cells[rowIndex, col] = row.Cells[col - 1].Value;
                        if (Convert.ToString(row.Cells["taxableAmt"].Value) == "")
                        {
                            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rowIndex, col];
                            objRange.Font.Bold = true;
                            objRange.Interior.ColorIndex = 22;
                        }
                    }
                    rowIndex++;
                }


                for (int col = 1; col < dgrdDetails.Columns.Count - 2; col++)
                {
                    NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, col];
                    objRange.Font.Bold = true;
                    //objRange.HorizontalAlignment = HorizontalAlignment.Center;

                    objRange = (NewExcel.Range)ExcelWorkSheet.Cells[2, col];
                    objRange.Font.Bold = true;
                    objRange.Interior.ColorIndex = 2;
                }

                for (int rIndex = 1; rIndex < rowIndex; rIndex++)
                {
                    for (int cIndex = 1; cIndex < dgrdDetails.Columns.Count - 2; cIndex++)
                    {
                        NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
                        objRange.NumberFormat = "@";
                        objRange.Cells.BorderAround();
                    }
                }

                ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                ExcelWorkBook.Close(true, misValue, misValue);
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);

                MessageBox.Show("Thank you ! Summary exported successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            catch (Exception ex)
            {
                strFileName = ex.Message;
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //foreach (Process process in Process.GetProcessesByName("Excel"))
                //    process.Kill();
            }
            return strFileName;
        }

        private string GetFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
            _browser.FileName = "GSTR-Summary.xls";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;
            return strPath;
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", Keys.Space);
                objSearch.ShowDialog();
                txtMonth.Text = objSearch.strSelectedData;
                dgrdDetails.Rows.Clear();

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
                    dgrdDetails.Rows.Clear();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnStateName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", Keys.Space);
                objSearch.ShowDialog();
                txtStateName.Text = objSearch.strSelectedData;
                dgrdDetails.Rows.Clear();
            }
            catch
            {
            }
        }

        private void btnExportJSON_Click(object sender, EventArgs e)
        {
            btnExportJSON.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export json ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    GetData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExportJSON.Enabled = true;
        }


        private void GetData()
        {
            string strQuery = CreateQuery_New();
            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 0)
            {
                object objJson = DataBaseAccess.ExecuteMyScalar("Select GSTNo from CompanyDetails Where  Other='" + MainPage.strCompanyName + "' ");
                string strFileName = GetJSONFileName(), strJSON = "", strFinYear = GetFinYear();
                strJSON = PrepareJSON.GetGSTR3B_JSON(ds, Convert.ToString(objJson), strFinYear);
                bool _bStatus = DataBaseAccess.SaveFile(strJSON, strFileName);
                if (_bStatus)
                {
                    MessageBox.Show("Thank You ! JSON File Imported successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private string GetFinYear()
        {
            string strFinYear = "";
            int _month = 0;
            //"", "", "", "", "", "", "", "", "", "", "", ""
            if (txtMonth.Text == "JANUARY")
                _month = 1;
            else if (txtMonth.Text == "FEBRUARY")
                _month = 2;
            else if (txtMonth.Text == "MARCH")
                _month = 3;
            else if (txtMonth.Text == "APRIL")
                _month = 4;
            else if (txtMonth.Text == "MAY")
                _month = 5;
            else if (txtMonth.Text == "JUNE")
                _month = 6;
            else if (txtMonth.Text == "JULY")
                _month = 7;
            else if (txtMonth.Text == "AUGUST")
                _month = 8;
            else if (txtMonth.Text == "SEPTEMBER")
                _month = 9;
            else if (txtMonth.Text == "OCTOBER")
                _month = 10;
            else if (txtMonth.Text == "NOVEMBER")
                _month = 11;
            else if (txtMonth.Text == "DECEMBER")
                _month = 12;
            if (_month < 4)
                strFinYear = _month.ToString("00") + MainPage.endFinDate.Year.ToString();
            else
                strFinYear = _month.ToString("00") + MainPage.startFinDate.Year.ToString();
            return strFinYear;
        }

        private string GetJSONFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "JSON Files (*.json)|*.json;";
            _browser.FileName = "GSTR-3B.json";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;

            return strPath;
        }

        private void GSTSummary_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }
    }


}
