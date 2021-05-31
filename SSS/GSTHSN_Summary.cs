using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace SSS
{
    public partial class GSTHSN_Summary : Form
    {
        DataBaseAccess dba;
        public GSTHSN_Summary()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            UpdateBlankDetails();
        }

        private void UpdateBlankDetails()
        {
            try
            {
                string strQuery = "";

                strQuery += " Update GR Set GR.OtherSign='+',GR.OtherAmount='0',GR.TaxPer=PR.TaxPer,GR.PurchaseType=PR.TaxLedger, GR.DisPer=(CASE WHEN CAST(PR.NetDiscount as money)>0 then (CAST(PR.NetDiscount as money)/PR.Amount)*100 else 0 end) from GoodsReceive GR Inner join PurchaseRecord PR on PR.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) Where GR.DisPer is NULL and GR.PurchaseType is NULL ";

                int count = dba.ExecuteMyQuery(strQuery);
            }
            catch { }
        }

        private void GSTHSN_Summary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private string CreateQuery(ref string strPurchaseQuery, ref string strJournalQuery, ref string strPurchaseReturnQuery, ref string strSaleQuery, ref string strSaleServiceQuery, ref string strSaleReturnQuery, ref string strPurchaseReturnItemWise, ref string strSaleReturnItemWise)
        {
            string strQuery = "";
            //Purchase
            strPurchaseQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty)Qty,TaxRate,ROUND(SUM(Amt),2) Amt,ROUND((SUM(CGST)/2),2) CGST,ROUND((SUM(CGST)/2),2) SGST,ROUND(SUM(IGST),2) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,SUM(Qty)Qty, TaxRate, SUM(Amount)Amt, (CASE WHEN PurchaseType like('L/%') then SUM((Amount * TaxRate) / 100) else 0 end) CGST,(CASE WHEN PurchaseType like('I/%') then SUM((Amount * TaxRate) / 100) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName, NAmount,Qty,(NAmount * (CASE WHen PurchaseType Like('%INCLUDE%') then(100 / (100 + TaxRate)) else 1 end))Amount,TaxRate,PurchaseType from ( "
                     + " Select HSNType, HSNCode,GRD.ItemName,GQty as Qty, NAmount, TaxRate, PurchaseType from GoodsReceive GR Cross Apply (Select ItemName, GRD.Quantity as GQty, GRD.Amount as GAmt, GRD.Rate as GNetRate from GoodsReceiveDetails GRD Where GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo)GRD OUTER APPLY(Select GM.Other as HSNType, GM.HSNCode, ROUND((((GAmt) * (100 - GR.DisPer)) / 100.00), 2)NAmount, GM.TaxRate from Items _IM Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN GR.PurchaseType Like('%INCLUDE%') then((GRD.GNetRate * 100) / (100 + TaxRate)) else GNetRate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - GR.DisPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN GR.PurchaseType Like('%INCLUDE%') then((GNetRate * 100) / (100 + TaxRate)) else GNetRate end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - GRD.GNetRate) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName = GRD.ItemName and GAmt> 0 )Sales UNION ALL "
                     + " Select 'SAC' as HSNTYpe,'' as HSNCode,'' as ItemName,0 Qty,((CAST(Packing as Money) + CAST(Freight as money) + CAST((OtherSign + CAST(OtherAmount as varchar)) as money))) OAmt,TaxPer,PurchaseType from  GoodsReceive UNION ALL "
                     + " Select 'SAC' as HSNTYpe,'' as HSNCode,'' as ItemName, 0 Qty,((CAST(Tax as Money))) OAmt,0 as TaxPer,'' PurchaseType from  GoodsReceive Where CAST(Tax as Money) > 0 "
                     + " )_Goods Where NAmount != 0 )_Sales Group by HSNType, HSNCode, TaxRate, PurchaseType,ItemName )Purchase OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=Purchase.ItemName)_IM Group by HSNType,TaxRate,HSNCode,ItemName,UnitName Order by HSNType,HSNCode ";

            // Journal
            strJournalQuery += " Select HSNType,HSNCode,Other as ItemName,UnitName,1 Qty,GSTPer as TaxRate,ROUND(SUM(DiffAmt),2)Amt,ROUND(SUM(CGSTAmt),2)CGSTAmt,ROUND(SUM(SGSTAmt),2)SGSTAmt,ROUND(SUM(IGSTAMt),2) IGSTAmt,SM.GroupName from JournalVoucherDetails JVD  Outer APPLY (Select GroupName from SupplierMaster Where AreaCode+AccountNo=AccountID) SM OUTER APPLY (SELECT TOp 1 _IGM.Other as HSNType,_IGM.HSNCode,UnitName from Items _IM inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where JVD.Other=_IM.ItemName) _IM Group by HSNType,HSNCode,SM.GroupName,GSTPer,Other,UnitName Order by HSNType,HSNCode,GSTPer ";

            //Purchase Return
            strPurchaseReturnQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) as Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN PurchaseType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN PurchaseType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,PurchaseType from ( "
                     + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, PurchaseType from( "
                     + " Select GM.Other as HSNType,SRD.ItemName, (GM.HSNCode) as HSNCode, SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
                     + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
                     + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where  SRD.TaxFree > 0  Union All "
                     + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR  left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' "
                     + ")_Sales Group by HSNCode, TaxRate, TaxType,HSNType,PurchaseType,ItemName )_Sales )_Sales )_Sales OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,TaxRate,ItemName,UnitName Order by HSNType,HSNCode,TaxRate asc ";

            //Sale
            strSaleQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) as Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN SalesType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SalesType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SalesType from ( "
                     + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SalesType from( "
                     + " Select GM.Other as HSNType, GM.HSNCode,GRD.ItemName, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where GRD.Amount > 0   Union All "
                     + " Select 'SAC' HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName, 0 as Quantity,ROUND(((((((Amount * (CASE WHen TaxType = 1 then(100.00 / (100.00 + TaxRate)) else 1 end))*(100.00 + DisStatus) / 100.00) *TaxRate) / 100.00) *CS.TaxDhara) / 100.00),2)Amount, TaxPer TaxRate, TaxType, SalesType from(Select ROUND(GRD.Amount, 2) Amount, GM.TaxRate, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty, TaxPer, SR.BillCode, SR.BillNo, SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting Where SBillCode = _SAles.BillCode) CS Union All "
                     + " Select 'SAC' HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)) +((GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where SBillCode = SR.BillCode) CS Where (GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) > 0  Union All "
                     + " Select 'SAC' HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(GRD.TaxAmt, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where SBillCode = SR.BillCode) CS Where  GRD.TaxAmt > 0  Union All "
                     + " Select 'SAC' HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2) Amount,sr.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' "
                     + " )_Sales Group by HSNType,HSNCode, TaxRate, TaxType,SalesType,ItemName  )_Sales  )_Sales  )_Sales OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,ItemName,TaxRate,UnitName Order by HSNType,HSNCode,TaxRate asc ";

            //Sale Service
            strSaleServiceQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty)Qty,TaxRate,ROUND(SUM(Amount),2) Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN SaleType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SaleType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from (Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SaleType from (Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SaleType from( "
                     + " Select GM.Other as HSNType, GM.HSNCode,SE.ItemName, 1 as Quantity, SE.Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SaleType from SaleServiceBook SR inner join SaleServiceDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))) ))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SE.Amount > 0   Union All "
                     + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleServiceBook SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' "
                     + " )_Sales Group by HSNCode, TaxRate, TaxType,SaleType,HSNType,ItemName )_Sales Where Amount!=0 )_Sales "
                     + " )_Sales OUTER APPLY(Select Top 1 UnitName from Items _IM Where _IM.ItemName = _Sales.ItemName)_IM Group by HSNType, HSNCode, UnitName, TaxRate, ItemName  Order by HSNType,HSNCode,TaxRate ";

            //Sale Return
            strSaleReturnQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                    + " Select HSNType, HSNCode,ItemName,Qty, Amount, TaxRate,(CASE WHEN SaleType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SaleType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                    + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SaleType from ( "
                    + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SaleType from( "
                    + " Select GM.Other as HSNType, (GM.HSNCode) as HSNCode,SRD.ItemName,SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
                    + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName, 0 as Quantity,ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer as TaxRate, TaxType,SaleType from (Select ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2) Amount, (SRD.DisStatus + CAST(SRD.Discount as varchar)) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty,SR.TaxPer,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where SRD.Amount > 0 and SR.ServiceAmt != 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                    + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (GReturnCode = SR.BillCode OR DebitNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
                    + " Select 'SAC' as HSNType, ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer Apply (Select TOP 1 TaxDhara from CompanySetting) CS Where SRD.TaxFree > 0  Union All "
                    + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money) + PackingAmt), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES')_Sales Group by HSNType,HSNCode, TaxRate, TaxType,SaleType,ItemName )_Sales Where Amount!=0 )_Sales   )_Sales  OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,UnitName,TaxRate,ItemName Order by HSNType,HSNCode,TaxRate asc ";

            //Purchase Return Item wise
            //strPurchaseReturnItemWise += " Select HSNType,HSNCode,ItemName,TaxRate,ROUND(SUM(Amount),2) Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
            //           + " Select HSNType, HSNCode, ItemName, Qty, Amount, TaxRate,(CASE WHEN PurchaseType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN PurchaseType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
            //           + " Select HSNType, HSNCode, ItemName,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,PurchaseType from ( "
            //           + " Select HSNType, HSNCode, ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, PurchaseType from( "
            //           + " Select GM.Other as HSNType, (GM.HSNCode) as HSNCode, SRD.ItemName, SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
            //           + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
            //           + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where  SRD.TaxFree > 0  Union All "
            //           + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR  left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' "
            //           + " )_Sales Group by HSNCode, TaxRate, TaxType, HSNType, PurchaseType, ItemName )_Sales )_Sales  )_Sales Group by HSNType, HSNCode, ItemName, TaxRate Order by HSNType,HSNCode,ItemName,TaxRate asc ";

            ////Sale Return Item wise
            //strSaleReturnItemWise += "  Select HSNType,HSNCode,Itemname,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
            //            + " Select HSNType, HSNCode, Itemname, Qty, Amount, TaxRate,(CASE WHEN SaleType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SaleType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
            //            + " Select HSNType, HSNCode, Itemname,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SaleType from ( "
            //            + " Select HSNType, HSNCode, Itemname, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SaleType from( "
            //            + " Select GM.Other as HSNType, (GM.HSNCode) as HSNCode, SRD.ItemName, SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
            //            + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as Itemname, 0 as Quantity,ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer as TaxRate, TaxType,SaleType from (Select ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2) Amount, (SRD.DisStatus + CAST(SRD.Discount as varchar)) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty,SR.TaxPer,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where SRD.Amount > 0 and SR.ServiceAmt != 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
            //            + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as Itemname,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (GReturnCode = SR.BillCode OR DebitNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
            //            + " Select 'SAC' as HSNType, ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as Itemname,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer Apply (Select TOP 1 TaxDhara from CompanySetting) CS Where SRD.TaxFree > 0  Union All "
            //            + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as Itemname,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money) + PackingAmt), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' )_Sales Group by HSNType, HSNCode, TaxRate, TaxType, SaleType, Itemname )_Sales Where Amount != 0 )_Sales  )_Sales Group by HSNType, HSNCode, Itemname, TaxRate  Order by HSNType,HSNCode,ItemName,TaxRate ";


            return strQuery;
        }

        private void btnShowSummary_Click(object sender, EventArgs e)
        {
            btnShowSummary.Enabled = false;
            try
            {               
                GetDataFromDB();
            }
            catch { }
            btnShowSummary.Enabled = true;
        }

        private void GetDataFromDB()
        {
            string strPurchaseQuery = "", strJournalQuery = "", strPurchaseReturnQuery = "", strSaleQuery = "", strSaleServiceQuery = "", strSaleReturnQuery = "", strPurchaseReturnItemWise = "", strSaleReturnItemWise = "";
            string strQuery = CreateQuery(ref strPurchaseQuery, ref strJournalQuery, ref strPurchaseReturnQuery, ref strSaleQuery, ref strSaleServiceQuery, ref strSaleReturnQuery, ref strPurchaseReturnItemWise, ref strSaleReturnItemWise);

            dgrdPurchase.DataSource = dba.GetDataTable(strPurchaseQuery);            

            dgrdJournal.DataSource = dba.GetDataTable(strJournalQuery);
           
            dgrdPurchaseReturn.DataSource = dba.GetDataTable(strPurchaseReturnQuery);
          
            dgrdSales.DataSource = dba.GetDataTable(strSaleQuery);           

            dgrdSaleService.DataSource = dba.GetDataTable(strSaleServiceQuery);         

            dgrdSaleReturn.DataSource = dba.GetDataTable(strSaleReturnQuery);         

            //dgrdPurchaseReturnItemWise.DataSource = dba.GetDataTable(strPurchaseReturnItemWise);           

            //dgrdSaleReturnItemWise.DataSource = dba.GetDataTable(strSaleReturnItemWise);               
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    GetDataForExport();
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExport.Enabled = true;
        }

        private void GetDataForExport()
        {
            DataSet ds = GetExportedDataFromDB();
            if (ds.Tables.Count > 0)
            {
                CreateExcelSheet(ds);
            }
        }

        private DataSet GetExportedDataFromDB()
        {
            string strQuery = "";

            //Purchase
            strQuery += " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty)Qty,TaxRate,ROUND(SUM(Amt),2) Amt,ROUND((SUM(CGST)/2),2) CGST,ROUND((SUM(CGST)/2),2) SGST,ROUND(SUM(IGST),2) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,SUM(Qty)Qty, TaxRate, SUM(Amount)Amt, (CASE WHEN PurchaseType like('L/%') then SUM((Amount * TaxRate) / 100) else 0 end) CGST,(CASE WHEN PurchaseType like('I/%') then SUM((Amount * TaxRate) / 100) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName, NAmount,Qty,(NAmount * (CASE WHen PurchaseType Like('%INCLUDE%') then(100 / (100 + TaxRate)) else 1 end))Amount,TaxRate,PurchaseType from ( "
                     + " Select HSNType, HSNCode,GRD.ItemName,GQty as Qty, NAmount, TaxRate, PurchaseType from GoodsReceive GR Cross Apply (Select ItemName, GRD.Quantity as GQty, GRD.Amount as GAmt, GRD.Rate as GNetRate from GoodsReceiveDetails GRD Where GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo)GRD OUTER APPLY(Select GM.Other as HSNType, GM.HSNCode, ROUND((((GAmt) * (100 - GR.DisPer)) / 100.00), 2)NAmount, GM.TaxRate from Items _IM Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN GR.PurchaseType Like('%INCLUDE%') then((GRD.GNetRate * 100) / (100 + TaxRate)) else GNetRate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - GR.DisPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN GR.PurchaseType Like('%INCLUDE%') then((GNetRate * 100) / (100 + TaxRate)) else GNetRate end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - GRD.GNetRate) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName = GRD.ItemName and GAmt> 0 )Sales UNION ALL "
                     + " Select 'SAC' as HSNTYpe,'' as HSNCode,'' as ItemName,0 Qty,((CAST(Packing as Money) + CAST(Freight as money) + CAST((OtherSign + CAST(OtherAmount as varchar)) as money))) OAmt,TaxPer,PurchaseType from  GoodsReceive UNION ALL "
                     + " Select 'SAC' as HSNTYpe,'' as HSNCode,'' as ItemName, 0 Qty,((CAST(Tax as Money))) OAmt,0 as TaxPer,'' PurchaseType from  GoodsReceive Where CAST(Tax as Money) > 0 "
                     + " )_Goods Where NAmount != 0 )_Sales Group by HSNType, HSNCode, TaxRate, PurchaseType,ItemName )Purchase OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=Purchase.ItemName)_IM Group by HSNType,TaxRate,HSNCode,ItemName,UnitName Order by HSNType,HSNCode ;"

            // Journal
                     +" Select HSNType,HSNCode,Other as ItemName,UnitName,1 Qty,GSTPer as TaxRate,ROUND(SUM(DiffAmt),2)Amt,ROUND(SUM(CGSTAmt),2)CGSTAmt,ROUND(SUM(SGSTAmt),2)SGSTAmt,ROUND(SUM(IGSTAMt),2) IGSTAmt,SM.GroupName from JournalVoucherDetails JVD  Outer APPLY (Select GroupName from SupplierMaster Where AreaCode+AccountNo=AccountID) SM OUTER APPLY (SELECT TOp 1 _IGM.Other as HSNType,_IGM.HSNCode,UnitName from Items _IM inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where JVD.Other=_IM.ItemName) _IM Group by HSNType,HSNCode,SM.GroupName,GSTPer,Other,UnitName Order by HSNType,HSNCode,GSTPer ;"

            //Purchase Return
                     + " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) as Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN PurchaseType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN PurchaseType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,PurchaseType from ( "
                     + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, PurchaseType from( "
                     + " Select GM.Other as HSNType,SRD.ItemName, (GM.HSNCode) as HSNCode, SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
                     + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
                     + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR inner join PurchaseReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where (PurchaseReturnCode = SR.BillCode OR CreditNoteCode = SR.BillCode)) CS Where  SRD.TaxFree > 0  Union All "
                     + " Select 'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,PurchaseType from PurchaseReturn SR  left join SaleTypeMaster SMN On SR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' "
                     + ")_Sales Group by HSNCode, TaxRate, TaxType,HSNType,PurchaseType,ItemName )_Sales )_Sales )_Sales OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,TaxRate,ItemName,UnitName Order by HSNType,HSNCode,TaxRate asc ;"

            //Sale
                     + " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) as Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN SalesType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SalesType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                     + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SalesType from ( "
                     + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SalesType from( "
                     + " Select GM.Other as HSNType, GM.HSNCode,GRD.ItemName, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where GRD.Amount > 0   Union All "
                     + " Select 'SAC' HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName, 0 as Quantity,ROUND(((((((Amount * (CASE WHen TaxType = 1 then(100.00 / (100.00 + TaxRate)) else 1 end))*(100.00 + DisStatus) / 100.00) *TaxRate) / 100.00) *CS.TaxDhara) / 100.00),2)Amount, TaxPer TaxRate, TaxType, SalesType from(Select ROUND(GRD.Amount, 2) Amount, GM.TaxRate, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty, TaxPer, SR.BillCode, SR.BillNo, SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting Where SBillCode = _SAles.BillCode) CS Union All "
                     + " Select 'SAC' HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)) +((GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where SBillCode = SR.BillCode) CS Where (GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) > 0  Union All "
                     + " Select 'SAC' HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(GRD.TaxAmt, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where SBillCode = SR.BillCode) CS Where  GRD.TaxAmt > 0  Union All "
                     + " Select 'SAC' HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2) Amount,sr.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SalesType from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' "
                     + " )_Sales Group by HSNType,HSNCode, TaxRate, TaxType,SalesType,ItemName  )_Sales  )_Sales  )_Sales OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,ItemName,TaxRate,UnitName Order by HSNType,HSNCode,TaxRate asc ;"

            //Sale Service
                     + " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty)Qty,TaxRate,ROUND(SUM(Amount),2) Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                     + " Select HSNType, HSNCode,ItemName, Qty, Amount, TaxRate,(CASE WHEN SaleType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SaleType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from (Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SaleType from (Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SaleType from( "
                     + " Select GM.Other as HSNType, GM.HSNCode,SE.ItemName, 1 as Quantity, SE.Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SaleType from SaleServiceBook SR inner join SaleServiceDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))) ))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SE.Amount > 0   Union All "
                     + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleServiceBook SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' "
                     + " )_Sales Group by HSNCode, TaxRate, TaxType,SaleType,HSNType,ItemName )_Sales Where Amount!=0 )_Sales "
                     + " )_Sales OUTER APPLY(Select Top 1 UnitName from Items _IM Where _IM.ItemName = _Sales.ItemName)_IM Group by HSNType, HSNCode, UnitName, TaxRate, ItemName  Order by HSNType,HSNCode,TaxRate ;"

            //Sale Return
                    + " Select HSNType,HSNCode,ItemName,UnitName,SUM(Qty) Qty,TaxRate,ROUND(SUM(Amount),2)Amt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt,ROUND(SUM(IGST),2) as IGSTAmt from ( "
                    + " Select HSNType, HSNCode,ItemName,Qty, Amount, TaxRate,(CASE WHEN SaleType Like('L/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) CGSTAmt,(CASE WHEN SaleType Like('I/%') then ROUND(((Amount * TaxRate) / 100.00), 2) else 0 end) IGST from ( "
                    + " Select HSNType, HSNCode,ItemName,Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) Amount,SaleType from ( "
                    + " Select HSNType, HSNCode,ItemName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, SaleType from( "
                    + " Select GM.Other as HSNType, (GM.HSNCode) as HSNCode,SRD.ItemName,SRD.Qty as Quantity, (SRD.Amount + ((SRD.Amount * (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end)) / SRD.Qty) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM  Where SRD.Amount > 0   Union All "
                    + " Select 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName, 0 as Quantity,ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer as TaxRate, TaxType,SaleType from (Select ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SRD.Amount * 100) / (100 + GM.TaxRate)) else SRD.Amount end), 2) Amount, (SRD.DisStatus + CAST(SRD.Discount as varchar)) DisStatus, SMN.TaxIncluded as TaxType, SRD.ItemName, SRD.Qty as Qty,SR.TaxPer,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SRD.Amount * 100) / (100 + TaxRate)) else SRD.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SRD.DisStatus + CAST(SRD.Discount as varchar))) / 100.00) else 1.00 end))/ SRD.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SRD.ItemName = _IM.ItemName ) as GM Where SRD.Amount > 0 and SR.ServiceAmt != 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                    + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(((SRD.Packing + SRD.Freight) + ((SRD.Packing + SRD.Freight + SRD.TaxFree) * CS.FreightDhara / 100)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where (GReturnCode = SR.BillCode OR DebitNoteCode = SR.BillCode)) CS Where (SRD.Packing + SRD.Freight) > 0  Union All "
                    + " Select 'SAC' as HSNType, ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND(SRD.TaxFree, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR inner join SaleReturnDetails SRD  on SR.BillCode = SRD.BillCode and SR.BillNo = SRD.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer Apply (Select TOP 1 TaxDhara from CompanySetting) CS Where SRD.TaxFree > 0  Union All "
                    + " Select 'SAC' as HSNType,ISNULL((Select Top 1  SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money) + PackingAmt), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + SR.TaxPer) / 100) else 1 end) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SR.SaleType from SaleReturn SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES')_Sales Group by HSNType,HSNCode, TaxRate, TaxType,SaleType,ItemName )_Sales Where Amount!=0 )_Sales   )_Sales  OUTER APPLY (Select Top 1 UnitName from Items _IM Where _IM.ItemName=_Sales.ItemName)_IM Group by HSNType,HSNCode,UnitName,TaxRate,ItemName Order by HSNType,HSNCode,TaxRate asc ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            return ds;
        }

        private string CreateExcelSheet(DataSet ds)
        {
            string[] strSheet = { "PURCHASE", "JOURNAL", "PURCHASE_RETURN", "SALES", "SALES_SERVICE", "SALES_RETURN"};
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            if (strFileName != "")
            {
                try
                {
                    object misValue = System.Reflection.Missing.Value;
                    ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                    ExcelWorkBook.Worksheets.Add(misValue, misValue, strSheet.Length, NewExcel.XlSheetType.xlWorksheet);
                    int sheetIndex = 1;
                    foreach (string strName in strSheet)
                    {

                        ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[sheetIndex];
                        var range = ExcelWorkSheet.get_Range("A1", "Z10000");
                        range = range.EntireRow;
                        range.Font.Name = "Times New Roman";

                        SetColumnName(ref ExcelWorkSheet, strName, ds);
                        sheetIndex++;
                    }

                    ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    ExcelWorkBook.Close(true, misValue, misValue);
                    ExcelApp.Quit();


                    Marshal.ReleaseComObject(ExcelWorkSheet);
                    Marshal.ReleaseComObject(ExcelWorkBook);
                    Marshal.ReleaseComObject(ExcelApp);

                    MessageBox.Show("Thanks ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    strFileName = ex.Message;
                }
                finally
                {
                    //foreach (Process process in Process.GetProcessesByName("Excel"))
                    //    process.Kill();
                }
            }
            return strFileName;
        }

        private void SetColumnName(ref NewExcel.Worksheet ExcelWorkSheet, string strSheetName, DataSet ds)
        {

            if (strSheetName == "PURCHASE")
            {
                var range = ExcelWorkSheet.get_Range("E3", "E10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("K3", "M10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount"};
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For PURCHASE";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[0]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[0].Rows.Count);
            }
            else if (strSheetName == "JOURNAL")
            {
                var range = ExcelWorkSheet.get_Range("G3", "G10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("M3", "O10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = {  "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount","Group Name" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For JOURNAL";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[1]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[7].Rows.Count);
            }
            else if (strSheetName == "PURCHASE_RETURN")
            {
                var range = ExcelWorkSheet.get_Range("C3", "C10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("F3", "H10000");
                range.NumberFormat = "#######.00";


                string[] strColumn = { "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For PURCHASE_RETURN";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[2]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[1].Rows.Count);
            }
            else if (strSheetName == "SALES")
            {
                var range = ExcelWorkSheet.get_Range("F3", "J10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For SALES";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[3]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[8].Rows.Count);
            }
            else if (strSheetName == "SALES_SERVICE")
            {
                var range = ExcelWorkSheet.get_Range("D3", "F10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For SALES_SERVICE";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[4]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[2].Rows.Count);
            }
            else if (strSheetName == "SALES_RETURN")
            {
                var range = ExcelWorkSheet.get_Range("F3", "H10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN Type", "HSN Code", "Item Name", "Unit Name", "Qty.", "Tax Rate", "Amount", "CGST Amount", "SGST Amount", "IGST Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For SALES_RETURN";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[5]);
                //SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[9].Rows.Count);
            }
            
        }

        private void SetDataInSheet(ref NewExcel.Worksheet ExcelWorkSheet, DataTable dt)
        {
            int rowIndex = 5, colIndex = 1;
            foreach (DataRow row in dt.Rows)
            {
                colIndex = 1;
                for (; colIndex <= dt.Columns.Count; colIndex++)
                    ExcelWorkSheet.Cells[rowIndex, colIndex] = row[colIndex - 1];

                rowIndex++;
            }
        }

        private void AddColumnsName(ref NewExcel.Worksheet ExcelWorkSheet, string[] strColumn)
        {
            int colIndex = 1, colNewIndex = 0;
            foreach (string strName in strColumn)
            {
                if (colIndex > 0)
                    ExcelWorkSheet.Cells[4, colIndex] = strName;
                colIndex++;
            }

            foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
            {
                column.ColumnWidth = (double)column.ColumnWidth + 10;
                if (colNewIndex >= colIndex)
                    break;
                colNewIndex++;
            }

            ColorConverter cc = new ColorConverter();

            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 1];
            if (ExcelWorkSheet.Name != "cdnr" && ExcelWorkSheet.Name != "cdnur")
            {
                objRange.Font.ColorIndex = 2;// = Color.FromArgb(255, 255, 255); ;// ColorTranslator.ToOle((Color)cc.ConvertFromString("#FFFFFF"));
                objRange.Interior.ColorIndex = 49;//  Color.FromArgb(0, 112, 192);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#0070C0"));
                objRange.Cells.BorderAround();
            }

            for (int cIndex = 1; cIndex <= strColumn.Length; cIndex++)
            {
                objRange = (NewExcel.Range)ExcelWorkSheet.Cells[2, cIndex];
                objRange.Font.ColorIndex = 2;// = Color.FromArgb(255, 255, 255); ;// ColorTranslator.ToOle((Color)cc.ConvertFromString("#FFFFFF"));
                objRange.Interior.ColorIndex = 49;//  Color.FromArgb(0, 112, 192);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#0070C0"));
                objRange.Cells.BorderAround();

                objRange = (NewExcel.Range)ExcelWorkSheet.Cells[4, cIndex];
                objRange.Interior.ColorIndex = 40;// Color.FromArgb(248,203,173);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#F8CBAD"));
                objRange.Cells.BorderAround();
            }
        }

        private string GetFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
            _browser.FileName = "GST_HSN_Summary.xls";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;

            return strPath;
        }

        private void GSTHSN_Summary_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                dba.EnableCopyOnClipBoard(dgrdJournal);
                dba.EnableCopyOnClipBoard(dgrdPurchase);
                dba.EnableCopyOnClipBoard(dgrdPurchaseReturn);
                dba.EnableCopyOnClipBoard(dgrdSaleReturn);
                dba.EnableCopyOnClipBoard(dgrdSales);
                dba.EnableCopyOnClipBoard(dgrdSaleService);
            }
            catch { }
        }
    }
}
