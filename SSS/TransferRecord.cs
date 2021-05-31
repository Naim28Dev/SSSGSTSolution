using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SSS
{
    public partial class TransferRecord : Form
    {
        ForwardingData fd;
        DataBaseAccess dba;
        string strDataBase = "";
        DataSet _dsItemName = null, _dsStockItems = null;
        bool isReminder = false;
        DataTable dtBalance, dtParty, dtTransport, dtStation, dtMarketer, dtCartoneSize, dtCartoneType, dtUserAccount, dtCategory, dtAdmin, dtCourierMaster, dtCostMaster, dtAddressBook, dtGroupMaster, dtOnAccountParty, dtOnaccountSalesRecord, dtCompanySetting, dtGoodsReceive = null, dtGoodsReceiveDetails = null, _dtPartyDetails = null, _dtPartyBankDetails = null, _dtPartyBrandDetails = null, _dtSaleType = null, _dtTaxCategory = null, _dtUnitMaster = null, _dtItemCategoryMaster = null, _dtChqDetails = null, _dtClosingStock = null, dtCompanyDetails = null, dtBrandMaster = null, dtBarCodeSetting = null, dtPinCodeDistance = null, dtPrintingConfig = null, dtProfitMargin = null, dtReminder_Schedular = null, dtBarCodeDetails = null, dtSMSMaster = null, dtOrderDetails = null, dtVariant1 = null, dtVariant2 = null, dtVariant3 = null;

        
        private void TransferRecord_Load(object sender, EventArgs e)
        {
            if(MainPage.strSoftwareType=="AGENT")
            {
                if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                    chkItemMaster.Enabled = false;
                chkPendingOrder.Enabled = true;
            }
        }

        private void chkBalance_CheckedChanged(object sender, EventArgs e)
        {

        }

        public TransferRecord()
        {
            InitializeComponent();
            fd = new ForwardingData();
            dba = new DataBaseAccess();
            CheckNextYrDatabase();
           // BindUserName();
            //GetRemoteCompanyUpdatedDate();
        }

        private void CheckNextYrDatabase()
        {
            try
            {
                strDataBase = fd.GetNextYearDataBase();
                if (strDataBase == "")
                {
                    btnSubmit.Enabled = false;
                    //   MessageBox.Show("Please Update the Next Year Database Path in Company Master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }
       

        private void GetAllData()
        {
            try
            {
                if (chkBalance.Checked)
                {
                    dtBalance = fd.GetBalanceAmountMaster();
                }
                if (chkParty.Checked)
                {
                    dtParty = fd.GetPartyData();
                    _dtPartyDetails = dba.GetDataTable("Select * from SupplierOtherDetails");
                    _dtPartyBankDetails = dba.GetDataTable("Select * from SupplierBankDetails");
                    _dtPartyBrandDetails = dba.GetDataTable("Select * from SupplierBrandDetails");
                }
                if (chkItemMaster.Checked)
                {
                    _dsItemName = fd.GetItemMaster();
                }
                if (chkAccount.Checked)
                {
                    dtTransport = fd.GetTransportMaster();
                    dtStation = fd.GetStationMaster();
                    dtMarketer = fd.GetMarketerMaster();

                    dtCartoneSize = fd.GetCartoneSizeMaster();
                    dtCartoneType = fd.GetCartoneTypeMaster();
                    dtUserAccount = fd.GetUserAccountMaster();
                    dtCategory = fd.GetCategory();
                    dtAdmin = fd.GetAdmin();
                    dtCourierMaster = fd.GetCourierMaster();
                    dtCostMaster = fd.GetCostMaster();
                    dtAddressBook = fd.GetAddressBook();
                    dtGroupMaster = fd.GetGroupMaster();
                    dtCompanySetting = fd.GetCompanySettingData();
                    _dtSaleType = dba.GetDataTable("Select * from SaleTypeMaster Order by TaxName");
                    _dtTaxCategory = dba.GetDataTable("Select * from TaxCategory Order by CategoryName");
                    _dtUnitMaster = dba.GetDataTable("Select * from UnitMaster Order by UnitName");
                    _dtItemCategoryMaster = dba.GetDataTable("Select * from ItemCategoryMaster Order by CategoryName");
                    _dtChqDetails = dba.GetDataTable("Select * from ChequeDetails Where Status !='DEPOSITED' Order by BillNo");
                    dtCompanyDetails = dba.GetDataTable("select * from CompanyDetails");
                    dtBrandMaster = dba.GetDataTable("Select * from BrandMaster");
                    dtBarCodeSetting = dba.GetDataTable("Select * from BarCodeSetting");
                    dtPinCodeDistance = dba.GetDataTable("Select * from PinCodeDistance");
                    dtPrintingConfig = dba.GetDataTable("Select * from PrintingConfig");
                    dtProfitMargin = dba.GetDataTable("Select * from ProfitMargin");
                    dtSMSMaster = dba.GetDataTable("Select * from MessageMaster");

                    dtVariant1 = dba.GetDataTable("Select * from VariantMaster1 Order by Variant1");
                    dtVariant2 = dba.GetDataTable("Select * from VariantMaster2 Order by Variant2");
                    dtVariant3 = dba.GetDataTable("Select * from VariantMaster3 Order by Variant3");

                }

                if (chkStock.Checked)
                {
                    dtGoodsReceive = dba.GetDataTable(" Select * from GoodsReceive Where SaleBill='PENDING' Order by ReceiptNo ");
                    dtGoodsReceiveDetails = dba.GetDataTable(" Select GRD.* from GoodsReceiveDetails GRD inner join  GoodsReceive GR On GRD.ReceiptCode=GR.ReceiptCode and GRD.ReceiptNo=GR.ReceiptNo Where GR.SaleBill='PENDING' Order by GRD.ReceiptNo,GRD.ID");
                    string strQuery = GetClosingQuery();
                    _dtClosingStock = dba.GetDataTable(strQuery);
                    dtBarCodeDetails = dba.GetDataTable(" Select * from Barcodedetails where Instock=1");
                   // _dsStockItems = dba.GetDataSet("Select * from Items Where ItemName in (Select ItemName from (Select ItemName,SUM(Qty)Qty from StockMaster Where BilLType in ('SALERETURN', 'OPENING','STOCKIN','PURCHASE') GROUP by ItemName UNION ALL Select ItemName,-SUM(Qty)Qty from StockMaster Where BilLType in ('SALES','STOCKOUT','PURCHASERETURN') GROUP by ItemName)Stock Group by ItemName having(SUM(Qty)!=0)) Select _IS.* from Items IM inner join ItemSecondary _IS on IM.BillCode=_IS.BillCode and IM.BillNo=_IS.BillNo Where ItemName in (Select ItemName from (Select ItemName,SUM(Qty)Qty from StockMaster Where BilLType in ('SALERETURN', 'OPENING','STOCKIN','PURCHASE') GROUP by ItemName UNION ALL Select ItemName,-SUM(Qty)Qty from StockMaster Where BilLType in ('SALES','STOCKOUT','PURCHASERETURN') GROUP by ItemName)Stock Group by ItemName having(SUM(Qty)!=0)) Select Distinct _IGM.* from Items IM inner join ItemGroupMaster _IGM on IM.GroupName=_IGM.GroupName Where ItemName in (Select ItemName from (Select ItemName,SUM(Qty)Qty from StockMaster Where BilLType in ('SALERETURN', 'OPENING','STOCKIN','PURCHASE') GROUP by ItemName UNION ALL Select ItemName,-SUM(Qty)Qty from StockMaster Where BilLType in ('SALES','STOCKOUT','PURCHASERETURN') GROUP by ItemName)Stock Group by ItemName having(SUM(Qty)!=0)) ");
                }

                if(chkPendingOrder.Checked)
                {
                    dtOrderDetails = dba.GetDataTable("Select * from OrderBooking Where Status='PENDING'");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting All Data in Transfering Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string GetClosingQuery()
        {
            string strQuery = "";
            if (MainPage.strCompanyName.Contains("SARAOGI"))
            {
                if (!MainPage.strCompanyName.Contains("PTN"))
                {
                    strQuery += " SELECT IMM.BillCode BCode,Imm.BillNo,Recc.* FROM ("
                              + " Select ItemName, Variant1, Variant2,'' as Variant3,'' Variant4,'' as Variant5,(IQty - OQty)Qty,Rate,MRP,BarCode,DesignName,BrandName from ( Select Category, GroupName, BrandName, MakeName, BarCode, DesignName, ItemName, Variant1, Variant2, SUM(InQty)IQty, SUM(SQty) OQty, SUM(InQty * Rate) NetInAmt, SUM((InQty - SQty) * Rate) NetAmt, Rate, MRP from( Select _IM.Other as Category, BrandName, GroupName, MakeName, _Stock.BarCode, _Stock.DesignName, _Stock.ItemName, _Stock.Variant1, _Stock.Variant2, InQty, ISNULL(SQty, 0) SQty, ISNULL(_Stock.MRP, ISNULL(SMRP,SSMRP))MRP, PB.BillCode, PBillDate, PB.PurchasePartyID, ISNULL(Rate, ISNULL(SRate,SSRate)) Rate  from (Select BarCode, DesignName, ItemName, BrandName, Variant1, Variant2, SUM(InQty)InQty,_Stock.MRP from( "
                             + " Select ISNULL(BarCode, '') as BarCode,ISNULL(BrandName,'') BrandName, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty,MRP from StockMaster SM Where BillType in ('OPENING') Group by ISNULL(BarCode, ''),ISNULL(BrandName,''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 ,MRP "
                             + " UNION ALL Select ISNULL(BarCode, '') as BarCode,ISNULL(BrandName,'') BrandName, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty,MRP from StockMaster SM Where BillType in ('PURCHASE') Group by ISNULL(BarCode, ''),ISNULL(BrandName,''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 ,MRP "
                             + " UNION ALL Select ISNULL(BarCode, '') as BarCode,ISNULL(BrandName,'') BrandName, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty,MRP from StockMaster SM Where BillType in ('SALERETURN') Group by ISNULL(BarCode, ''),ISNULL(BrandName,''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 ,MRP "
                             + " )_Stock Group by BarCode,BrandName, DesignName, ItemName, Variant1, Variant2, _Stock.MRP)_Stock OUTER APPLY(Select _IM.Other,GroupName,MakeName from Items _IM Where _IM.ItemName = _Stock.ItemName)_IM "
                             + " OUTER APPLY(Select SUM(Qty)SQty from StockMaster SM Where ISNULL(SM.BarCode, '') = ISNULL(_Stock.BarCode, '') and ISNULL(SM.BrandName,'') = ISNULL(_Stock.BrandName,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.MRP = _Stock.MRP  and BillType in ('SALES', 'PURCHASERETURN','STOCKOUT') "
                             + " )_ST OUTER APPLY(Select Top 1 PB.BillCode, PB.Date as PBillDate, PB.PurchasePartyID, PBS.Rate, PBS.MRP from PurchaseBookSecondary PBS inner join PurchaseBook PB on PBS.BillCode = PB.BillCode and PBS.BillNO = PB.BillNo Where ISNULL(PBS.BarCode, '') = ISNULL(_Stock.BarCode, '') and ISNULL(PBS.BrandName,'') = ISNULL(_Stock.BrandName,'') and PBS.ItemName = _Stock.ItemName and PBS.Variant1 = _Stock.Variant1 and PBS.Variant2 = _Stock.Variant2 Order by PB.Date desc)PB OUTER APPLY(Select Top 1 SM.Rate as SRate,SM.MRP as SMRP from StockMaster SM Where BillType in ('SALERETURN', 'OPENING','STOCKIN') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and ISNULL(SM.BrandName,'') = ISNULL(_Stock.BrandName,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Rate!=0 and ISNULL(PB.Rate,0)=0)_STR OUTER APPLY(Select Top 1 SM.Rate as SSRate,SM.MRP as SSMRP from StockMaster SM Where BillType in ('SALES','STOCKOUT') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and ISNULL(SM.BrandName,'') = ISNULL(_Stock.BrandName,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Rate!=0 and ISNULL(_STR.SRate,0)=0 and ISNULL(PB.Rate,0)=0)_SSales Where (InQty > 0 OR SQty > 0) "
                             + " )Stock Group by Category, GroupName, BrandName, MakeName, BarCode, DesignName, ItemName, Variant1, Variant2, MRP, Rate "
                             + " )_Stock )Recc LEFT JOIN Items IMM ON Recc.ItemName = IMM.ItemName Where Qty != 0 ";
                }
                else
                {
                    strQuery += " Select BCode,ItemName, Variant1, Variant2,'' as Variant3,'' Variant4,'' Variant4,'' as Variant5,(IQty - OQty)Qty,Rate,MRP,BarCode,DesignName from ( "
                         + " Select Category, GroupName, MakeName, BarCode, DesignName, ItemName, Variant1, Variant2, SUM(InQty)IQty, SUM(SQty) OQty, SUM(InQty * Rate) NetInAmt, SUM((InQty - SQty) * Rate) NetAmt, Rate, MRP from( "
                         + " Select _IM.Other as Category, GroupName, MakeName, _Stock.BarCode, _Stock.DesignName, _Stock.ItemName, _Stock.Variant1, _Stock.Variant2, InQty, ISNULL(SQty, 0) SQty, ISNULL(MRP, ISNULL(SMRP,SSMRP))MRP, PB.BillCode, PBillDate, PB.PurchasePartyID, ISNULL(Rate,ISNULL(SRate,SSRate)) Rate  from ("
                         + "Select BarCode, DesignName, ItemName, Variant1, Variant2, SUM(InQty)InQty from(Select ISNULL(BarCode, '') as BarCode, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty from StockMaster SM Where BillType in ('OPENING') Group by ISNULL(BarCode, ''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 "
                         + "UNION ALL Select ISNULL(BarCode, '') as BarCode, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty from StockMaster SM Where BillType in ('PURCHASE') Group by ISNULL(BarCode, ''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 "
                         + "UNION ALL Select ISNULL(BarCode, '') as BarCode, ISNULL(DesignName, '')DesignName, ItemName, Variant1, Variant2, SUM(Qty)InQty from StockMaster SM Where BillType in ('SALERETURN') Group by ISNULL(BarCode, ''), ISNULL(DesignName, ''), ItemName, Variant1, Variant2 "
                         + ")_Stock Group by BarCode,DesignName, ItemName, Variant1, Variant2)_Stock OUTER APPLY(Select _IM.Other,GroupName,MakeName from Items _IM Where _IM.ItemName = _Stock.ItemName)_IM"
                         + " OUTER APPLY(Select SUM(Qty)SQty from StockMaster SM Where ISNULL(SM.BarCode, '') = ISNULL(_Stock.BarCode, '') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2  and BillType in ('SALES', 'PURCHASERETURN') "
                         + ")_ST  OUTER APPLY(Select Top 1 PB.BillCode, PB.Date as PBillDate, PB.PurchasePartyID, PBS.Rate, PBS.MRP from PurchaseBookSecondary PBS inner join PurchaseBook PB on PBS.BillCode = PB.BillCode and PBS.BillNO = PB.BillNo Where ISNULL(PBS.BarCode, '') = ISNULL(_Stock.BarCode, '') and PBS.ItemName = _Stock.ItemName and PBS.Variant1 = _Stock.Variant1 and PBS.Variant2 = _Stock.Variant2 Order by PB.Date desc)PB  OUTER APPLY(Select Top 1 SM.Rate as SRate,SM.MRP as SMRP from StockMaster SM Where BillType in ('SALERETURN', 'OPENING','STOCKIN') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Rate!=0 and ISNULL(PB.Rate,0)=0)_STR OUTER APPLY(Select Top 1 SM.Rate as SSRate,SM.MRP as SSMRP from StockMaster SM Where BillType in ('SALES','STOCKOUT') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Rate!=0 and ISNULL(_STR.SRate,0)=0 and ISNULL(PB.Rate,0)=0)_SSales Where (InQty > 0 OR SQty > 0) "
                         + " )Stock Group by Category, GroupName,MakeName, BarCode, DesignName, ItemName, Variant1, Variant2, MRP, Rate)_Stock OUTER APPLY(Select Top 1 FChallanCode as BCode from CompanySetting) CS Where (IQty - OQty) != 0 Order by BarCode, DesignName, ItemName, Variant1, Variant2 ";

                }
            }
            else
            {
                strQuery += " SELECT Recc.*,IMM.BillCode BCode, Imm.BillNo,(ISNULL(SRM.SaleMRP, 0) + ISNULL(IMSR.SaleMRP, 0))SaleMRP,(ISNULL(SRM.SaleRate, 0) + ISNULL(IMSR.SaleRate, 0))SaleRate,(ISNULL(SRM.PurchaseRate, 0) + ISNULL(IMSR.PurchaseRate, 0))PurchaseRate FROM (Select BrandName, BarCode, ItemName, Variant1, Variant2,'' Variant3, '' Variant4, '' as Variant5, Abs(Rate)Rate, (SUM(InQty) - SUM(SQty))Qty, Abs(Rate)MRP, '' DesignName from( Select SM.BrandName, SM.BarCode, SM.ItemName, SM.Variant1, SM.Variant2, INQty, ISNULL(OutQty, 0)SQty, (CASE WHEN ISNULL(Purc.AvgRate, 0) > 0 then ISNULL(Purc.AvgRate, 0) else ISNULL(SM.Rate, 0)end)Rate"
                         + " from(  Select SM.BrandName, ISNULL(SM.BarCode, '')BarCode, SM.ItemName, SM.Variant1, SM.Variant2, SUM(SM.Qty)INQty, 0 Rate from StockMaster SM  Where BillType in ('OPENING', 'PURCHASE', 'STOCKIN', 'SALERETURN')  Group by SM.BrandName, ISNULL(SM.BarCode, ''), SM.ItemName, SM.Variant1, SM.Variant2   UNION ALL SELECT BrandName, ISNULL(BarCode, '')BarCode, ItemName, Variant1, Variant2, 0 INQty, AvgRate FROM(SELECT BrandName, ISNULL(BarCode, '')BarCode, ItemName, Variant1, Variant2, SUM(Qty)TSQty, (SUM(SAmt) / SUM(Qty))AvgRate FROM( Select SO.BrandName, ISNULL(SO.BarCode, '')BarCode, SO.ItemName, SO.Variant1, SO.Variant2, Qty, (Qty) * (Rate)SAmt from StockMaster SO Where SO.BillType in ('SALES', 'PURCHASERETURN', 'STOCKOUT'))SOLD Group by SOLD.BrandName, ISNULL(SOLD.BarCode, ''), SOLD.ItemName, SOLD.Variant1, SOLD.Variant2 )OUTS WHERE (Select COUNT(*) from StockMaster ISM Where BillType in ('OPENING', 'PURCHASE', 'STOCKIN', 'SALERETURN') AND ISM.BrandName = OUTS.BrandName  and ISNULL(ISM.BarCode, '')= ISNULL(OUTS.BarCode, '') and ISM.ItemName = OUTS.ItemName and ISM.Variant1 = OUTS.Variant1 and ISM.Variant2 = OUTS.Variant2  ) = 0	)SM"
                         + " LEFT JOIN(SELECT ROW_NUMBER() OVER(PARTITION BY BarCode, BrandName, ItemName, Variant1, Variant2 ORDER BY ID ASC) AS RNo, BrandName, BarCode, ItemName, Variant1, Variant2, SUM(PQty)TPQty, CAST((SUM(PAmt) / SUM(PQty)) as Numeric(18, 4))AvgRate FROM (SELECT 2 ID, IM.BrandName, Description BarCode, IM.ItemName, Variant1, Variant2, 1 PQty, IMS.PurchaseRate PAmt FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE IMS.PurchaseRate != 0  UNION ALL SELECT 0 ID, BrandName, BarCode, ItemName, Variant1, Variant2, (CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE' WHERE BillType IN('OPENING', 'PURCHASE', 'STOCKIN') UNION ALL SELECT 1 ID ,BrandName,BarCode,ItemName,Variant1,Variant2,(CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE' WHERE BillType IN('SALERETURN')  )Ratt GROUP BY BrandName, BarCode, ItemName, Variant1, Variant2, ID)Purc on SM.BrandName = Purc.BrandName and ISNULL(SM.BarCode, '') = ISNULL(Purc.BarCode, '') and SM.ItemName = Purc.ItemName and SM.Variant1 = Purc.Variant1 and SM.Variant2 = Purc.Variant2 AND RNo = 1  left join (Select _SM.BrandName, ISNULL(_SM.BarCode, '')BarCode, _SM.ItemName, _SM.Variant1, _SM.Variant2, SUM(_SM.Qty)OutQty from StockMaster _SM  Where _SM.BillType in ('SALES', 'PURCHASERETURN', 'STOCKOUT')  Group by _SM.BrandName,ISNULL(_SM.BarCode, ''),_SM.ItemName,_SM.Variant1,_SM.Variant2  )_SM on SM.BrandName = _SM.BrandName and ISNULL(SM.BarCode, '')= ISNULL(_SM.BarCode, '') and SM.ItemName = _SM.ItemName and SM.Variant1 = _SM.Variant1 and SM.Variant2 = _SM.Variant2 "
                         + " Where(InQty != 0 OR ISNULL(OutQty, 0) != 0) )Stock Group by BrandName, BarCode, ItemName, Variant1, Variant2, Abs(Rate)	)Recc  LEFT JOIN Items IMM ON Recc.ItemName = IMM.ItemName  LEFT JOIN ( SELECT RN = ROW_NUMBER() Over(partition by BrandName, BarCode, ItemName, Variant1, Variant2 Order by ID), * FROM( SELECT 0 ID, *FROM( SELECT RowN = ROW_NUMBER() Over(partition by BrandName, BarCode, ItemName, Variant1, Variant2 Order by BillNo), SaleRate, SaleMRP, Rate PurchaseRate, BrandName, BarCode, ItemName, Variant1, Variant2  FROM PurchaseBookSecondary WHERE  SaleRate != 0 OR SaleMRP != 0 OR Rate != 0) PurRate WHERE RowN = 1 UNION ALL SELECT 1 ID, * FROM( SELECT RowN = ROW_NUMBER() Over(partition by BrandName, BarCode, ItemName, Variant1, Variant2 Order by BillNo), SaleRate, SaleMRP, Rate, BrandName, BarCode, ItemName, Variant1, Variant2 FROM StockTransferSecondary WHERE  SaleRate != 0 OR SaleMRP != 0 OR Rate != 0) StRate WHERE RowN = 1 UNION ALL   SELECT 2 ID, 1 RowN, SaleRate, SaleMRP, PurchaseRate, BrandName, Description, ItemName, Variant1, Variant2 FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE  SaleRate != 0 OR SaleMRP != 0 OR PurchaseRate != 0 ) INN	)SRM ON SRM.RN = 1 AND(SELECT TOP 1 OtherCode FROM CompanySetting) = 'True' AND Recc.ItemName = SRM.ItemName AND Recc.BrandName = SRM.BrandName AND Recc.BarCode = SRM.BarCode AND Recc.Variant1 = SRM.Variant1 AND  Recc.Variant2 = SRM.Variant2 LEFT JOIN ( SELECT Rown = ROW_NUMBER() Over(Partition by BrandName, ItemName, Variant1, Variant2 Order By IMS.BillNo), SaleRate, SaleMRP, PurchaseRate, BrandName, ItemName, Variant1, Variant2 FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE SaleRate != 0 OR SaleMRP != 0 OR PurchaseRate != 0 )IMSR ON IMSR.Rown = 1 AND(SELECT TOP 1 OtherCode FROM CompanySetting) = 'False' AND Recc.ItemName = IMSR.ItemName AND Recc.BrandName = IMSR.BrandName  AND Recc.Variant1 = IMSR.Variant1 AND Recc.Variant2 = IMSR.Variant2 where Qty <> 0";
            }
            return strQuery;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkAccount.Checked || chkBalance.Checked || chkParty.Checked || chkStock.Checked || chkItemMaster.Checked || chkPendingOrder.Checked)
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Forward Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        btnSubmit.Text = "Please wait .....";
                        btnSubmit.Enabled = false;
                        GetAllData();
                        ForwardAllData();
                        MainPage.ChangeDataBase(MainPage.strDataBaseFile);
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Atleast one Category for Forwarding !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnSubmit.Enabled = true;
            btnSubmit.Text = "&Forward";
        }

        private void ForwardAllData()
        {
            string strSuccess = "";
            try
            {
                int count = 0;

                if (chkBalance.Checked && dtBalance != null)
                {
                    count += fd.ForwardBalanceAmount(dtBalance, strDataBase);
                }

                if (chkParty.Checked && dtParty != null)
                {
                    count += fd.ForwardParty(dtParty, strDataBase);
                    count += fd.ForwardPartyDetails(_dtPartyDetails, strDataBase);
                    count += fd.ForwardPartyBankDetails(_dtPartyBankDetails, strDataBase);
                    count += fd.ForwardPartyBrandDetails(_dtPartyBrandDetails, strDataBase);
                }
                if(chkItemMaster.Checked)
                {
                    if (_dsItemName != null)
                    {
                        count += fd.ForwardItems(_dsItemName, strDataBase);
                        strSuccess += " ITEM NAME ";
                    }
                }
                if (chkPendingOrder.Checked)
                {
                    if (dtOrderDetails != null)
                    {
                        count += fd.ForwardPendingOrder(dtOrderDetails, strDataBase);
                        strSuccess += " Order Details ";
                    }
                }
                if (chkAccount.Checked)
                {

                    if (dtCartoneSize != null)
                    {
                        count += fd.ForwardCartoneSize(dtCartoneSize, strDataBase);
                        strSuccess += " CARTON SIZE ";
                    }
                    if (dtCartoneType != null)
                    {
                        count += fd.ForwardCartoneType(dtCartoneType, strDataBase);
                        strSuccess += " CARTON TYPE ";
                    }                  
                    if (dtMarketer != null)
                    {
                        count += fd.ForwardMarketer(dtMarketer, strDataBase);
                        strSuccess += " MARKTER NAME ";
                    }
                    if (dtStation != null)
                    {
                        count += fd.ForwardStation(dtStation, strDataBase);
                        strSuccess += " Station NAME ";
                    }
                    if (dtTransport != null)
                    {
                        count += fd.ForwardTransport(dtTransport, strDataBase);
                        strSuccess += " TRANSPORT NAME ";
                    }
                    if (dtUserAccount != null)
                    {
                        count += fd.ForwardUserAccount(dtUserAccount, strDataBase);
                        strSuccess += " User ACCOUNT ";
                    }
                    if (dtCategory != null)
                    {
                        count += fd.ForwardCategory(dtCategory, strDataBase);
                        strSuccess += " CATEGORY ";
                    }
                    if (dtCourierMaster != null)
                    {
                        count += fd.ForwardCourierMaster(dtCourierMaster, strDataBase);
                        strSuccess += " Courier Master ";
                    }
                    if (dtCostMaster != null)
                    {
                        count += fd.ForwardCostMaster(dtCostMaster, strDataBase);
                        strSuccess += " COST MASTER ";
                    }
                    if (dtAddressBook != null)
                    {
                        count += fd.ForwardAddressBook(dtAddressBook, strDataBase);
                        strSuccess += " ADRESS BOOK ";
                    }
                    if (dtGroupMaster != null)
                    {
                        count += fd.ForwardGroupMaster(dtGroupMaster, strDataBase);
                        strSuccess += " GROUP MASTER ";
                    }
                    if (dtCompanySetting != null)
                    {
                        count += fd.ForwardCompanySettingData(dtCompanySetting, strDataBase);
                        strSuccess += " COMPANY SETTING ";
                    }
                    if (_dtSaleType != null)
                    {
                        count += fd.ForwardSaleTypeMaster(_dtSaleType, strDataBase);
                        strSuccess += " SALE TYPE NAME ";
                    }
                    if (_dtTaxCategory != null)
                    {
                        count += fd.ForwardTaxCategoryMaster(_dtTaxCategory, strDataBase);
                        strSuccess += " TAX CATEGORY ";
                    }
                    if (_dtUnitMaster != null)
                    {
                        count += fd.ForwardUnitMaster(_dtUnitMaster, strDataBase);
                        strSuccess += " UNIT MASTER ";
                    }
                    if (_dtItemCategoryMaster != null)
                    {
                        count += fd.ForwardItemCategoryMaster(_dtItemCategoryMaster, strDataBase);
                        strSuccess += " ITEM CATEGORY NAME ";
                    }
                    if (_dtChqDetails != null)
                    {
                        count += fd.ForwardChqDetailsMaster(_dtChqDetails, strDataBase);
                        strSuccess += " CHQ DETAILS ";
                    }
                    if (dtAdmin != null)
                    {
                        count += fd.ForwardAdmin(dtAdmin, strDataBase);
                        strSuccess += " ADMIN MASTER ";
                    }
                    if (dtCompanyDetails != null)
                    {
                        count += fd.ForwardCompanyDetails(dtCompanyDetails, strDataBase);
                        strSuccess += " COMPANY DETAILS ";
                    }
                    if (dtBrandMaster != null)
                    {
                        count += fd.ForwardBrandMaster(dtBrandMaster, strDataBase);
                        strSuccess += " BRAND MASTER";
                    }
                    if (dtBarCodeSetting != null)
                    {
                        count += fd.ForwardBarCodeSetting(dtBarCodeSetting, strDataBase);
                        strSuccess += " BARCODE SETTING";
                    }
                    if (dtPinCodeDistance != null)
                    {
                        count += fd.ForwardPinCodeDistance(dtPinCodeDistance, strDataBase);
                        strSuccess += " PINCODE DISTANCE";
                    }
                    if (dtPrintingConfig != null)
                    {
                        count += fd.ForwardPrintingConfig(dtPrintingConfig, strDataBase);
                        strSuccess += " PRINTING CONFIG";
                    }
                    if (dtProfitMargin != null)
                    {
                        count += fd.ForwardProfitMargin(dtProfitMargin, strDataBase);
                        strSuccess += " PROFIT MARGIN";
                    }
                    if (dtSMSMaster != null)
                    {
                        count += fd.ForwardSMSMaster(dtSMSMaster, strDataBase);
                        strSuccess += " SMS MASTER";
                    }
                    if (dtVariant1 != null)
                    {
                        count += fd.ForwardVariantMaster1(dtVariant1, strDataBase);
                        strSuccess += " Variant MASTER 1";
                    }
                    if (dtVariant2 != null)
                    {
                        count += fd.ForwardVariantMaster2(dtVariant2, strDataBase);
                        strSuccess += " Variant MASTER 2";
                    }
                    if (dtVariant3 != null)
                    {
                        count += fd.ForwardVariantMaster3(dtVariant3, strDataBase);
                        strSuccess += " Variant MASTER 3";
                    }
                    count += fd.ForwardMonthDetails(strDataBase);
                    strSuccess += " MONTH DETAIL ";

                }

                if (chkStock.Checked)
                {
                   // strDataBase = "A902";
                    int __count = 0;
                    if (dtGoodsReceive != null)
                        count += __count = fd.ForwardClosingStockBill(dtGoodsReceive, strDataBase);
                    if (dtGoodsReceiveDetails != null && __count > 0)
                        count += fd.ForwardClosingStockDetails(dtGoodsReceiveDetails, strDataBase);
                    if (_dtClosingStock != null)
                    {
                        if(MainPage.strSoftwareType=="AGENT" && MainPage.strCompanyName.Contains("PTN"))
                            count += fd.ForwardClosingStock_PTN(_dtClosingStock, strDataBase); 
                        else
                        count += fd.ForwardClosingStock(_dtClosingStock, strDataBase);
                    }

                    if (dtBarCodeDetails != null)
                        count += fd.ForwardBarCodeDetails(dtBarCodeDetails, strDataBase);

                    //if (_dsItemName == null || _dsStockItems!=null)
                    //{
                    //    count += fd.ForwardItems(_dsStockItems, strDataBase);
                    //    strSuccess += " ITEM NAME ";
                    //}

                }

                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record Successfully forwarded to Next DataBase ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    chkAccount.Checked = chkParty.Checked = chkBalance.Checked = chkStock.Checked =chkItemMaster.Checked= chkPendingOrder.Checked=false;
                }
                else
                {
                    MessageBox.Show("Sorry ! We are Unable to Forward Record to Next DataBase at This Time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Forwarding Record to Next DataBase in Transfering Records " + strSuccess, ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TransferRecord_KeyDown(object sender, KeyEventArgs e)
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
        

    }
}

