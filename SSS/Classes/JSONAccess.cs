using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using System.Windows.Forms;


namespace SSS
{
    class JSONAccess
    {
        public static string GetJSONFromSaleBillNo(string strBillNo)
        {
            string strJSONString = "";
            try
            {
                List<BillDetail> _billDetails = new List<BillDetail>();
                string strQuery = " Select CD.GSTNo as userGstin,'O' supplyType,'1' subSupplyType,'INV' docType,(SR.BillCode+CAST(SR.BIllNo as varchar)) as docNo,CONVERT(varchar,BillDate,103) as docDate,CD.GSTNo as fromGstin,CD.CompanyName as fromTrdName,CD.Address as fromAddr1,'' as fromAddr2,CD.StateName as fromPlace,CD.PinCode as fromPincode,CD.StateCode as fromStateCode,CD.StateCode as actualFromStateCode,SM.GSTNo as toGstin,SM.Name as toTrdName,SM.Address as toAddr1,'' as toAddr2,(CASE WHEN SR.SubPartyID='SELF' then SM.Station else SM2.Address end) as toPlace,(CASE WHEN SR.SubPartyID='SELF' then SM.PINCode else SM2.PINCode end)  as toPincode,(CASE WHEN SR.SubPartyID='SELF' then SM.StateCode else SM2._StateCode end) as toStateCode,(CASE WHEN SR.SubPartyID='SELF' then SM.StateCode else SM2._StateCode end) as actualToStateCode,(CAST(SR.NetAmt as Money)-SR.TaxAmount) as totalValue,CGSTAmt as cgstValue,CGSTAmt as sgstValue,IGSTAmt as igstValue,0 as cessValue,1 as transMode,0 as transDistance,Transport as transporterName,TR.TransportGSTNo transporterId,'' as transDocNo,''transDocDate,'' as vehicleNo,'R' as vehicleType,CAST(NetAmt as Money) as totInvValue,'' mainHsnCode,ISNULL(PD.Distance,0)Distance,SR.SubPartyID from SalesRecord SR Outer APPLY (Select TOP 1 GSTNo as TransportGSTNo from Transport Where TransportName=SR.Transport)TR Outer APPLY (Select Name,GSTNo,Address,Station,PinCode,ST.StateCode from SupplierMaster SM Left join StateMaster ST on SM.State=ST.StateName Where (SM.AreaCode+SM.AccountNo)=SR.SalePartyID) SM OUTER APPLY (Select Top 1 SM2.Address,SM2.PINCode,_ST.StateCode _StateCode from SupplierMaster SM2  Left join StateMaster _ST on SM2.State=_ST.StateName Where (SM2.AreaCode+SM2.AccountNo)=SR.SubPartyID) SM2 Outer APPLY (Select CD.CompanyName,CD.GSTNo,CD.Address,CD.StateName,CD.PinCode,ST.StateCode from CompanyDetails CD Left join StateMaster ST on CD.StateName=ST.StateName WHERE CD.Other = '" + MainPage.strCompanyName + "') CD OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD OUTER APPLY (Select Top 1 Distance from PinCodeDistance  Where FromPinCOde=CD.PinCode and ToPinCode=(CASE WHEN SR.SubPartyID='SELF' then SM.PINCode else SM2.PINCode end)) PD Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") Order by SR.BillNo"
                                + "  Select BillNo,ItemName as productName,'READYMADE GARMENT' productDesc, HSNCode as hsnCode,Qty as quantity,UnitName as qtyUnit,ROUND((CASE WHEN TaxType = 1 and Qty!=0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) as taxableAmount,(Case When Region='INTERSTATE' then TaxRate else 0 end) igstRate,(Case When Region='LOCAL' then TaxRate/2 else 0 end) cgstRate from (  Select BillNo,Region,ItemName, HSNCode, SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate, TaxType,UnitName from( "
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,LTRIM(RTRIM(REPLACE(REPLACE(ItemName, HSNCode, ''), ':', ''))) ItemName, GM.HSNCode as HSNCode, (CASE WHEN ISNULL(GRD.MTR,0)>0 THEN GRD.Mtr else GRD.Quantity end) Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType,(CASE WHEN ISNULL(GRD.Mtr,0)>0 then 'MTR' else (Select TOP 1 UnitName from Items Where ItemName = GRD.ItemName) end) UnitName from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where GRD.Amount > 0  and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ")  Union All "
                                + " Select BillNo,Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * TaxPer) / 100) * CS.TaxDhara) / 100),2)Amount, TaxPer TaxRate, TaxType,'' as UnitName from (Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo, SR.TaxPer, SMN.Region, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((GRD.Amount * 100) / (100 + GM.TaxRate)) else GRD.Amount end), 2) Amount, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' and SR.BillNo != 0  and GRD.Amount > 0 and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS  Union All " // Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _Sales.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,0 as Quantity,ROUND((((GRD.PackingAmt + GRD.FreightAmt)* (CASE WHen TaxIncluded = 1 then(100/(100 + SR.TaxPer)) else 1 end)) + ((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara / 100)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,'' UnitName from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select TOP 1 FreightDhara from CompanySetting) CS Where SR.BillCode != '' and SR.BillNo != 0 and(GRD.PackingAmt + GRD.FreightAmt) > 0 and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") Union All "
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,0 as Quantity,ROUND(GRD.TaxAmt, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,'' as UnitName from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Where SR.BillCode != '' and GRD.TaxAmt > 0  and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") Union All "
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,'' as UnitName from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") "
                                + " )_Sales Group by BillNo,ItemName, Region, HSNCode, TaxRate, TaxType, UnitName)_Sales Order by Qty desc "
                                + " Insert Into EditTrailDetails ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                + " Select 'SALES' as BillType,BillCode,BillNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) Date,CAST(NetAmt as Money) NetAmt,'" + MainPage.strLoginName + "' as UpdatedBy,1,0,'WAYBILL_GENERTAED' from SalesRecord SR Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0], dtDetails = ds.Tables[1];
                    string strDistance = "", strSaleBillNo = "", strTransportID = "", strFromPinCode = "", strPinCodeMessage = "", strToPinCode = "";
                    double dDistance = 0;
                    int _index = 0, strTransType = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dDistance = ConvertObjectToDouble(row["Distance"]);
                        strToPinCode = Convert.ToString(row["toPincode"]);
                        strFromPinCode = Convert.ToString(row["fromPincode"]);
                        strSaleBillNo = Convert.ToString(row["docNo"]);
                        strTransType = 1;
                        if (Convert.ToString(row["SubPartyID"]) != "SELF")
                            strTransType = 2;

                        if (dDistance == 0)
                        {
                            strPinCodeMessage = "enter pin code in master or ";

                            if (strDistance == "")
                            {
                                string strValue = Microsoft.VisualBasic.Interaction.InputBox("Please " + strPinCodeMessage + "enter Distance of " + row["toPlace"] + " ! ", "Enter distance manually", "", 400, 300);
                                if (strValue != "" && strValue != "0")
                                {
                                    strDistance = strValue;
                                }
                            }
                            dDistance = ConvertObjectToDouble(strDistance);
                            if (dDistance > 0)
                            {
                                if (dDistance > 3000)
                                {
                                    MessageBox.Show("Sorry ! Distance is invalid, Please enter correct distance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dDistance = 0;
                                }
                                else
                                {
                                    DataBaseAccess.SaveDistanceFromPinCode(strFromPinCode, strToPinCode, dDistance);
                                    if (dt.Rows.Count - 1 > _index)
                                    {
                                        SetDistanceInTable(ref dt, strFromPinCode, strToPinCode, dDistance);
                                    }
                                }
                            }
                        }

                        strTransportID = Convert.ToString(row["transporterId"]);
                        if (strTransportID != "")
                        {
                            if (dDistance > 0)
                            {
                                //  dDistance += 10; 
                                DataRow[] rows = dtDetails.Select("BillNo in ('" + strSaleBillNo + "') ");
                                if (rows.Length > 0)
                                {
                                    string strToGST = Convert.ToString(row["toGstin"]);
                                    if (strToGST == "")
                                        strToGST = "URP";
                                    BillDetail _bill = new BillDetail
                                    {
                                        userGstin = Convert.ToString(row["userGstin"]),
                                        supplyType = Convert.ToString(row["supplyType"]),
                                        subSupplyType = Convert.ToInt32(row["subSupplyType"]),
                                        docType = Convert.ToString(row["docType"]),
                                        docNo = Convert.ToString(row["docNo"]),
                                        docDate = Convert.ToString(row["docDate"]),
                                        transType = strTransType,
                                        fromGstin = Convert.ToString(row["fromGstin"]),
                                        fromTrdName = Convert.ToString(row["fromTrdName"]),
                                        fromAddr1 = Convert.ToString(row["fromAddr1"]),
                                        fromAddr2 = "",
                                        fromPlace = Convert.ToString(row["fromPlace"]),
                                        fromPincode = ConvertObjectToInt(row["fromPincode"]),
                                        fromStateCode = ConvertObjectToInt(row["fromStateCode"]),
                                        actualFromStateCode = ConvertObjectToInt(row["actualFromStateCode"]),
                                        toGstin = strToGST,//Convert.ToString(row["toGstin"]),
                                        toTrdName = Convert.ToString(row["toTrdName"]),
                                        toAddr1 = Convert.ToString(row["toAddr1"]),
                                        toAddr2 = "",
                                        toPlace = Convert.ToString(row["toPlace"]),
                                        toPincode = ConvertObjectToInt(row["toPincode"]),
                                        toStateCode = ConvertObjectToInt(row["toStateCode"]),
                                        actualToStateCode = ConvertObjectToInt(row["actualToStateCode"]),
                                        totalValue = ConvertObjectToDouble(row["totalValue"]),
                                        cgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        sgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        igstValue = ConvertObjectToDouble(row["igstValue"]),
                                        cessValue = 0,
                                        TotNonAdvolVal = 0,
                                        OthValue = 0,
                                        totInvValue = ConvertObjectToDouble(row["totInvValue"]),
                                        transMode = Convert.ToString(row["transMode"]),
                                        transDistance = dDistance,// ConvertObjectToDouble(strDistance),
                                        transporterName = Convert.ToString(row["transporterName"]),
                                        transporterId = Convert.ToString(row["transporterId"]),
                                        transDocNo = "",
                                        transDocDate = "",
                                        vehicleNo = "",
                                        vehicleType = "",
                                        mainHsnCode = ConvertObjectToInt(rows[0]["hsnCode"]),
                                        itemList = GetItemDetails(rows)
                                    };
                                    _billDetails.Add(_bill);
                                }
                            }
                            else { MessageBox.Show("Sorry ! Unable to calculate distance right now  in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { MessageBox.Show("Sorry ! Transporter ID can't be blank in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                        _index++;
                    }
                }
                if (_billDetails.Count > 0)
                {
                    var json = new JavaScriptSerializer().Serialize(_billDetails);
                    strJSONString = json.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message + " ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return strJSONString;
        }

        public static string GetJSONFromTradingSaleBillNo(string strBillNo)
        {
            string strJSONString = "";
            try
            {
                List<BillDetail> _billDetails = new List<BillDetail>();
                string strQuery = " Select CD.GSTNo as userGstin,'O' supplyType,'1' subSupplyType,'INV' docType,(SR.BillCode+CAST(SR.BIllNo as varchar)) as docNo,CONVERT(varchar,Date,103) as docDate,CD.GSTNo as fromGstin,CD.CompanyName as fromTrdName,CD.Address as fromAddr1,'' as fromAddr2,CD.StateName as fromPlace,CD.PinCode as fromPincode,CD.StateCode as fromStateCode,CD.StateCode as actualFromStateCode,SM.GSTNo as toGstin,SM.Name as toTrdName,SM.Address as toAddr1,'' as toAddr2,(CASE WHEN SR.SubPartyID='SELF' then SM.Station else SM2.Address end) as toPlace,(CASE WHEN SR.SubPartyID='SELF' then SM.PINCode else SM2.PINCode end)  as toPincode,(CASE WHEN SR.SubPartyID='SELF' then SM.StateCode else SM2._StateCode end) as toStateCode,(CASE WHEN SR.SubPartyID='SELF' then SM.StateCode else SM2._StateCode end) as actualToStateCode,(CAST(SR.NetAmt as Money)-SR.TaxAmt) as totalValue,CGSTAmt as cgstValue,CGSTAmt as sgstValue,IGSTAmt as igstValue,0 as cessValue,1 as transMode,0 as transDistance,TransportName as transporterName,TR.TransportGSTNo transporterId,'' as transDocNo,''transDocDate,'' as vehicleNo,'' as vehicleType,CAST(NetAmt as Money) as totInvValue,'' mainHsnCode,ISNULL(PD.Distance,0)Distance,SR.SubPartyID from SalesBook SR Outer APPLY (Select TOP 1 GSTNo as TransportGSTNo from Transport Where TransportName=SR.TransportName)TR Outer APPLY (Select Name,GSTNo,Address,Station,PinCode,ST.StateCode from SupplierMaster SM Left join StateMaster ST on SM.State=ST.StateName Where (SM.AreaCode+SM.AccountNo)=SR.SalePartyID) SM OUTER APPLY (Select Top 1 SM2.Address,SM2.PINCode,_ST.StateCode _StateCode from SupplierMaster SM2  Left join StateMaster _ST on SM2.State=_ST.StateName Where (SM2.AreaCode+SM2.AccountNo)=SR.SubPartyID) SM2 Outer APPLY (Select CD.CompanyName,CD.GSTNo,CD.Address,CD.StateName,CD.PinCode,ST.StateCode from CompanyDetails CD Left join StateMaster ST on CD.StateName=ST.StateName WHERE CD.Other = '" + MainPage.strCompanyName + "') CD OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD OUTER APPLY (Select Top 1 Distance from PinCodeDistance  Where FromPinCOde=CD.PinCode and ToPinCode=(CASE WHEN SR.SubPartyID='SELF' then SM.PINCode else SM2.PINCode end)) PD Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") Order by SR.BillNo"
                                + "  Select BillNo,ItemName as productName,'READYMADE GARMENT' productDesc, HSNCode as hsnCode,Qty as quantity,UnitName as qtyUnit,ROUND((CASE WHEN TaxType = 1 and Qty!=0 then((Amount * 100) / (100 + TaxRate)) else Amount end),2) as taxableAmount,(Case When Region='INTERSTATE' then TaxRate else 0 end) igstRate,(Case When Region='LOCAL' then TaxRate/2 else 0 end) cgstRate from (  Select BillNo,Region,ItemName, HSNCode, SUM(Quantity) Qty, SUM(Amount)Amount, TaxRate, TaxType,UnitName from( "
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,LTRIM(RTRIM(REPLACE(REPLACE(ItemName, HSNCode, ''), ':', ''))) ItemName, GM.HSNCode as HSNCode, SE.Qty Quantity, (SE.Amount + ((SE.Amount * (SE.SDisPer)) / 100)) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType,SE.UnitName UnitName from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.SDisPer)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.SDisPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SE.Amount > 0  and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ")  Union All "
                                //+ " Select BillNo,Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * TaxPer) / 100) * CS.TaxDhara) / 100),2)Amount, TaxPer TaxRate, TaxType,'' as UnitName from (Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo, SR.TaxPer, SMN.Region, ROUND((CASE WHEN SMN.TaxIncluded = 1  then((SE.Amount * 100) / (100 + GM.TaxRate)) else SE.Amount end), 2) Amount, (SE.SDisPer) DisStatus, SMN.TaxIncluded as TaxType, SE.ItemName, SE.Qty as Qty from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.SDisPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.SDisPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' and SR.BillNo != 0  and SE.Amount > 0 and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS  Union All " // Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (_SAles.DisStatus)) / 100.00) else 1.00 end))/ _Sales.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM
                                //+ " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,0 as Quantity,ROUND((((SE.Amount)* (CASE WHen TaxIncluded = 1 then(100/(100 + SR.TaxPer)) else 1 end)) + ((SE.Amount) * CS.FreightDhara / 100)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,'' UnitName from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES'  Outer Apply (Select TOP 1 FreightDhara from CompanySetting) CS Where SR.BillCode != '' and SR.BillNo != 0 and(SE.Amount) > 0 and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") Union All "
                                + " Select (SR.BillCode+CAST(SR.BillNo as varchar)) BillNo,SMN.Region,'SERVICE CHARGE' as ItemName,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,0 as Quantity,ROUND((CAST(SR.PostageAmt as Money) + CAST(PackingAmt as Money) + CAST([Description]+Convert(Varchar,DisAmt)as Money) + CAST(OtherSign + CAST(OtherAmt as Varchar) as Money) + CAST(ISNULL(GreenTax, 0) as money)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,'' as UnitName from SalesBook SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") "
                                + " )_Sales Group by BillNo,ItemName, Region, HSNCode, TaxRate, TaxType, UnitName)_Sales Order by Qty desc "
                                + " Insert Into EditTrailDetails ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                + " Select 'SALES' as BillType,BillCode,BillNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) Date,CAST(NetAmt as Money) NetAmt,'" + MainPage.strLoginName + "' as UpdatedBy,1,0,'WAYBILL_GENERTAED' from SalesBook SR Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strBillNo + ") ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0], dtDetails = ds.Tables[1];
                    string strDistance = "", strSaleBillNo = "", strTransportID = "", strFromPinCode = "", strPinCodeMessage = "", strToPinCode = "";
                    double dDistance = 0;
                    int _index = 0, strTransType = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dDistance = ConvertObjectToDouble(row["Distance"]);
                        strToPinCode = Convert.ToString(row["toPincode"]);
                        strFromPinCode = Convert.ToString(row["fromPincode"]);
                        strSaleBillNo = Convert.ToString(row["docNo"]);
                        strTransType = 1;
                        if (Convert.ToString(row["SubPartyID"]) != "SELF")
                            strTransType = 2;

                        if (dDistance == 0)
                        {
                            //if (strFromPinCode != "")
                            //{
                            //    strDistance = DataBaseAccess.GetDistanceBetween(strToPinCode, strFromPinCode);
                            //}
                            //else
                            strPinCodeMessage = "enter pin code in master or ";

                            if (strDistance == "")
                            {
                                string strValue = Microsoft.VisualBasic.Interaction.InputBox("Please " + strPinCodeMessage + "enter Distance of " + row["toPlace"] + " ! ", "Enter distance manually", "", 400, 300);
                                if (strValue != "" && strValue != "0")
                                {
                                    strDistance = strValue;
                                }
                            }
                            dDistance = ConvertObjectToDouble(strDistance);
                            if (dDistance > 0)
                            {
                                if (dDistance > 3000)
                                {
                                    MessageBox.Show("Sorry ! Distance is invalid, Please enter correct distance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dDistance = 0;
                                }
                                else
                                {
                                    DataBaseAccess.SaveDistanceFromPinCode(strFromPinCode, strToPinCode, dDistance);
                                    if (dt.Rows.Count - 1 > _index)
                                    {
                                        SetDistanceInTable(ref dt, strFromPinCode, strToPinCode, dDistance);
                                    }
                                }
                            }
                        }

                        strTransportID = Convert.ToString(row["transporterId"]);
                        if (strTransportID != "")
                        {
                            if (dDistance > 0)
                            {
                                //  dDistance += 10; 
                                DataRow[] rows = dtDetails.Select("BillNo in ('" + strSaleBillNo + "') ");
                                if (rows.Length > 0)
                                {
                                    string strToGST = Convert.ToString(row["toGstin"]);
                                    if (strToGST == "")
                                        strToGST = "URP";
                                    BillDetail _bill = new BillDetail
                                    {
                                        userGstin = Convert.ToString(row["userGstin"]),
                                        supplyType = Convert.ToString(row["supplyType"]),
                                        subSupplyType = Convert.ToInt32(row["subSupplyType"]),
                                        docType = Convert.ToString(row["docType"]),
                                        docNo = Convert.ToString(row["docNo"]),
                                        docDate = Convert.ToString(row["docDate"]),
                                        transType = strTransType,
                                        fromGstin = Convert.ToString(row["fromGstin"]),
                                        fromTrdName = Convert.ToString(row["fromTrdName"]),
                                        fromAddr1 = Convert.ToString(row["fromAddr1"]),
                                        fromAddr2 = "",
                                        fromPlace = Convert.ToString(row["fromPlace"]),
                                        fromPincode = ConvertObjectToInt(row["fromPincode"]),
                                        fromStateCode = ConvertObjectToInt(row["fromStateCode"]),
                                        actualFromStateCode = ConvertObjectToInt(row["actualFromStateCode"]),
                                        toGstin = strToGST,//Convert.ToString(row["toGstin"]),
                                        toTrdName = Convert.ToString(row["toTrdName"]),
                                        toAddr1 = Convert.ToString(row["toAddr1"]),
                                        toAddr2 = "",
                                        toPlace = Convert.ToString(row["toPlace"]),
                                        toPincode = ConvertObjectToInt(row["toPincode"]),
                                        toStateCode = ConvertObjectToInt(row["toStateCode"]),
                                        actualToStateCode = ConvertObjectToInt(row["actualToStateCode"]),
                                        totalValue = ConvertObjectToDouble(row["totalValue"],2),
                                        cgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        sgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        igstValue = ConvertObjectToDouble(row["igstValue"]),
                                        cessValue = 0,
                                        TotNonAdvolVal = 0,
                                        OthValue = 0,
                                        totInvValue = ConvertObjectToDouble(row["totInvValue"]),
                                        transMode = Convert.ToString(row["transMode"]),
                                        transDistance = dDistance,// ConvertObjectToDouble(strDistance),
                                        transporterName = Convert.ToString(row["transporterName"]),
                                        transporterId = Convert.ToString(row["transporterId"]),
                                        transDocNo = Convert.ToString(row["transDocNo"]),
                                        transDocDate = Convert.ToString(row["transDocDate"]),
                                        vehicleNo = Convert.ToString(row["vehicleNo"]),
                                        vehicleType = Convert.ToString(row["vehicleType"]),
                                        mainHsnCode = ConvertObjectToInt(rows[0]["hsnCode"]),
                                        itemList = GetItemDetails(rows)
                                    };
                                    _billDetails.Add(_bill);
                                }
                            }
                            else { MessageBox.Show("Sorry ! Unable to calculate distance right now  in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { MessageBox.Show("Sorry ! Transporter ID can't be blank in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                        _index++;
                    }
                }
                if (_billDetails.Count > 0)
                {
                    var json = new JavaScriptSerializer().Serialize(_billDetails);
                    strJSONString = json.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message + " ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return strJSONString;
        }

        public static List<ItemList> GetItemDetails(DataRow[] rows)
        {
            List<ItemList> _itemList = new List<ItemList>();
            int _index = 1;
            foreach (DataRow row in rows)
            {
                _itemList.Add(new ItemList
                {
                    itemNo = _index,
                    productName = Convert.ToString(row["productName"]),
                    productDesc = Convert.ToString(row["productDesc"]),
                    hsnCode = ConvertObjectToDouble(row["hsnCode"]),
                    quantity = ConvertObjectToDouble(row["quantity"]),
                    qtyUnit = Convert.ToString(row["qtyUnit"]),
                    taxableAmount = ConvertObjectToDouble(row["taxableAmount"],2),
                    sgstRate = ConvertObjectToDouble(row["cgstRate"]),
                    cgstRate = ConvertObjectToDouble(row["cgstRate"]),
                    igstRate = ConvertObjectToDouble(row["igstRate"]),
                    cessRate = 0,
                    cessNonAdvol = 0
                });
                _index++;
            }
            return _itemList;

        }

        public static double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToDouble(objValue);

            }
            catch
            {
            }
            return dValue;
        }

        public static double ConvertObjectToDouble(object objValue,int _dec)
        {
            double dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToDouble(objValue);

                if(_dec==2)
                    dValue = Convert.ToDouble(dValue.ToString("0.00"));
                else if (_dec == 3)                  
                    dValue = Convert.ToDouble(dValue.ToString("0.000"));

            }
            catch
            {
            }
            return dValue;
        }
        public static int ConvertObjectToInt(object objValue)
        {
            int dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToInt32(objValue);

            }
            catch
            {
            }
            return dValue;
        }

        public static void SetDistanceInTable(ref DataTable dt, string strfromPinCode, string strToPinCode, double dDistance)
        {
            try
            {
                DataRow[] rows = dt.Select("fromPincode='" + strfromPinCode + "' and toPincode='" + strToPinCode + "' ");
                foreach (DataRow row in rows)
                {
                    row["Distance"] = dDistance;
                }
            }
            catch { }
        }

        private static string ConvertStringToObject(object _obj)
        {
            if (Convert.ToString(_obj) == "")
                return null;
            else
                return Convert.ToString(_obj);
        }

        private static string ConvertStringToObject_HSN(object _obj)
        {
            if (Convert.ToString(_obj) == "")
                return null;
            else
            {
                string strHSN = Convert.ToString(_obj);
                //if (strHSN.Length == 6 && strHSN!= "998599")
                //    strHSN = strHSN+ "90";

                return strHSN;
            }
        }

        public static string GetJSONFrom_EInvoiceSaleBillNo(bool _bEway ,string strFullBillNo, string usedFor = null,string _strInvType=null)
        {
            string strJSONString = "";
            try
            {
                string strQuery = "", strBillCode = "";
                string[] arrFullBillNos = strFullBillNo.Split(',');
                foreach (string strFullBill in arrFullBillNos)
                {
                    var str = strFullBill.Split(' ')[0];
                    strBillCode += str + "',";
                }
                strBillCode = strBillCode.Substring(0, strBillCode.Length - 1);

                strQuery = " Select  CD.GSTNo,CD.FullCompanyName as LglNm,null as TrdNm,_SM.StateCode as Pos,CD.Address as Addr1,null as Addr2,CD.StateName as LOC,CD.PinCode as Pin,_SM.StateCOde as Stcd,STDNo+' '+CD.PhoneNo as Ph,CD.EmailID as Em from CompanyDetails CD left join StateMaster _SM on CD.StateName=_SM.StateName  Where Other in (Select CompanyName from CompanySetting Where SBillCode IN (" + strBillCode + ") OR SaleServiceCode IN (" + strBillCode + ") OR GReturnCode IN (" + strBillCode + ") OR DebitNoteCode IN (" + strBillCode + ") ) ";

                if (usedFor != null && usedFor == "TRADING")
                {
                    strQuery += "Select 'INV' as Typ,(SR.BillCode + CAST(SR.BillNo as varchar))No,CONVERT(varchar, SR.Date, 103)Dt,SM.GSTNo as Gstin, SM.Name as LglNm,null as TrdNm,_SM.StateCode as Pos,SM.Address as Addr1,null as Addr2,SR.Station as LOC, SM.PINCode as Pin,_SM.StateCOde as Stcd,SM.MobileNo as Ph,SM.EmailID as Em,SM1.GSTNo SGSTNo, SM1.Name,null as STrdNm,SM1.Address as SAddr1,null as SAddr2,SR.Station as SLOC,SM1.PINCode as SPin,_SM.StateCOde as SStcd,SR.TaxableAmt as AssVal,(CASE WHEN STM.Region = 'INTERSTATE' then SR.TaxAmt else 0 end)IgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmt / 2) else 0 end)CgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmt / 2) else 0 end)SgstVal,0 as CesVal,0 as StCesVal,0 as OthChrg,(RoundOffSign + CAST(RoundOffAmt as varchar))RndOffAmt,NetAmt as TotInvVal,0 as TotInvValFc "
                       + ",_TR.GSTNo as TransId,SR.TransportName as Transport,(CASE WHEN ISNULL(SM1.Name, '')= '' then PCD.Distance else _PCD.Distance end)Distance,SR.Remark as InvRm,CONVERT(varchar, SR.Date, 103)InvStDt,CONVERT(varchar, SR.Date, 103)InvEndDt,_BillNo as InvNo,_Date as InvDt from SalesBook SR inner join SupplierMaster SM On SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as nvarchar)) left join SupplierMaster SM1 On SR.SubPartyID = (SM1.AreaCode + CAST(SM1.AccountNo as nvarchar)) and SM1.GroupName = 'SUB PARTY' left join SaleTypeMaster STM on SR.SalesType = STM.TaxName and STM.SaleType = 'SALES' left join Transport _TR on SR.TransportName = _TR.TransportName left join StateMaster _SM on SM.State = _SM.StateName left join StateMaster __SM on SM1.State = __SM.StateName left join PinCodeDistance PCD on PCD.ToPinCode = SM.PINCode left join PinCodeDistance _PCD on _PCD.ToPinCode = SM.PINCode "
                       + "OUTER APPLY(Select CONVERT(varchar, MAX(SB.MDate), 103) as _Date, MAX(SB._BillNo)_BillNo from(Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SalesBook _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo UNION ALL Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SalesBook _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo)SB )_SR Where SM.GSTNo!='' and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") "
                       + "Select BillCode, BillNo, ItemName, HSNType,(CASE WHEN HSNType = 'SAC' then(Select Top 1 SACCode from CompanyDetails Where SACCode != '') else HSNCode end)HSNCode,Qty,Rate as UnitPrice ,Amount,TaxRate,ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,UQC from( "
                       + "Select BillCode,BillNo,HSNType,HSNCode,ItemName,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,(CASE WHEN TaxType = 1 and Qty > 0 then ((Rate * 100.00) / (100.00 + TaxRate)) else Rate end) as Rate,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100.00) / (100.00 + TaxRate)) else Amount end),2) Amount,Region,(Select Top 1 UnitName from Items _Im WHere _Im.ItemName = _Sales.ItemName)UQC from ("
                       + "Select BillCode,BillNo,HSNType,HSNCode, SUM(Amount)Amount,ItemName, SUM(Quantity) Qty,Rate, TaxRate, TaxType,Region from(   "
                       + "Select SR.BillCode,SR.BillNo,'HSN' as HSNType,(GM.HSNCode) as HSNCode,SE.ItemName, Qty as Quantity,SE.Rate, ROUND((((SE.MRP-((SE.MRP*ISNULL(SR.SpecialDscPer,0))/100.00))*SE.Qty) * (100.00 + SDisPer) / 100.00), 2) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType,SMN.Region from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - (SR.DisPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Rate * 100) / (100 + TaxRate)) else SE.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - (SR.DisPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName) as GM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and SE.Amount > 0 "
                       + "Union All Select SR.BillCode,SR.BillNo,'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND(PackingAmt + GreenTax+PostageAmt + ((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+CAST((SR.Description + CAST(SR.DisAmt as varchar)) as Money))* (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)), 2)Rate,ROUND(PackingAmt + GreenTax+PostageAmt + ((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)+CAST((SR.Description + CAST(SR.DisAmt as varchar)) as Money))* (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesBook SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and (PackingAmt + GreenTax+PostageAmt +SR.OtherAmt+SR.DisAmt) != 0 "
                       + ")_Sales Group by BillCode,BillNo,HSNCode, TaxRate,Rate,TaxType,HSNType,ItemName,Region)_Sales Where Amount != 0)_Sales ";

                }
                else if (usedFor != null && usedFor == "SERVICE")
                {
                    strQuery += " Select 'INV' as Typ,(SR.BillCode + CAST(SR.BillNo as varchar))No,CONVERT(varchar, SR.Date, 103)Dt,SM.GSTNo as Gstin, SM.Name as LglNm,null as TrdNm,_SM.StateCode as Pos,SM.Address as Addr1,null as Addr2,SM.Station as LOC, SM.PINCode as Pin,_SM.StateCOde as Stcd,SM.MobileNo as Ph,SM.EmailID as Em,SM1.GSTNo SGSTNo, SM1.Name,null as STrdNm,SM1.Address as SAddr1,null as SAddr2,SM.Station as SLOC,SM1.PINCode as SPin,_SM.StateCOde as SStcd,SR.TaxableAmt as AssVal,(CASE WHEN STM.Region = 'INTERSTATE' then SR.TaxAmt else 0 end)IgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmt / 2) else 0 end)CgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmt / 2) else 0 end)SgstVal,0 as CesVal,0 as StCesVal,0 as OthChrg,(RoundOffSign + CAST(RoundOffAmt as varchar))RndOffAmt,NetAmt as TotInvVal,0 as TotInvValFc "
                       + " ,NULL as TransId,NULL as Transport,(CASE WHEN ISNULL(SM1.Name, '')= '' then PCD.Distance else _PCD.Distance end)Distance,SR.Remark as InvRm,CONVERT(varchar, SR.Date, 103)InvStDt,CONVERT(varchar, SR.Date, 103)InvEndDt,_BillNo as InvNo,_Date as InvDt from SaleServiceBook SR inner join SupplierMaster SM On SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as nvarchar)) left join SupplierMaster SM1 On SR.SubPartyID = (SM1.AreaCode + CAST(SM1.AccountNo as nvarchar)) and SM1.GroupName = 'SUB PARTY' left join SaleTypeMaster STM on SR.SaleType = STM.TaxName and STM.SaleType = 'SALES' left join StateMaster _SM on SM.State = _SM.StateName left join StateMaster __SM on SM1.State = __SM.StateName left join PinCodeDistance PCD on PCD.ToPinCode = SM.PINCode left join PinCodeDistance _PCD on _PCD.ToPinCode = SM.PINCode "
                       + " OUTER APPLY(Select CONVERT(varchar, MAX(SB.MDate), 103) as _Date, MAX(SB._BillNo)_BillNo from(Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SaleServiceBook _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo UNION ALL Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SalesBook _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo)SB )_SR Where SM.GSTNo!='' and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") "
                       + " Select BillCode, BillNo, HSNType,HSNCode,ItemName,Qty,Amount as UnitPrice,Amount,TaxRate,ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,UQC from(Select BillCode,BillNo,ItemName,HSNType,HSNCode,Qty,Amount,TaxRate,ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,(Select Top 1 UnitName from Items _Im WHere _Im.ItemName = _Sales.ItemName)UQC from ("
                       + " Select BillCode,BillNo,HSNType,HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,ItemName,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100.00) / (100.00 + TaxRate)) else Amount end),2) Amount,Region from ( "
                       + " Select BillCode,BillNo,HSNType,HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,ItemName, TaxRate, TaxType,Region from( Select SR.BillCode,SR.BillNo, GM.Other as HSNType,(GM.HSNCode) as HSNCode,ItemName, 1 as Quantity, SE.Amount, GM.TaxRate, SMN.TaxIncluded as TaxType,SMN.Region from SaleServiceBook SR inner join SaleServiceDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))) ))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ")and SE.Amount>0 Union All  "
                       + " Select BillCode,BillNo,'SAC' as HSNType,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)* (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)), 2) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SaleServiceBook SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Where  ROUND((CAST((SR.OtherSign + CAST(SR.OtherAmt as varchar)) as Money)), 2)!=0 and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + "))_Sales Group by BillCode,BillNo,HSNCode,TaxRate,TaxType,HSNType,ItemName,Region )_Sales Where Amount!=0 )_Sales )_Sale ";

                }
                else if (usedFor != null && usedFor == "CREDITNOTE")
                {
                    strQuery += " Select '"+ _strInvType+"' as Typ,(SR.BillCode + CAST(SR.BillNo as varchar))No,CONVERT(varchar, SR.Date, 103)Dt,SM.GSTNo as Gstin, SM.Name as LglNm,null as TrdNm,_SM.StateCode as Pos,SM.Address as Addr1,null as Addr2,SM.Station as LOC, SM.PINCode as Pin,_SM.StateCOde as Stcd,SM.MobileNo as Ph,SM.EmailID as Em,SM1.GSTNo SGSTNo, SM1.Name,null as STrdNm,SM1.Address as SAddr1,null as SAddr2,SM.Station as SLOC,SM1.PINCode as SPin,_SM.StateCOde as SStcd,SR.TaxableAmt as AssVal,(CASE WHEN STM.Region = 'INTERSTATE' then SR.TaxAmount else 0 end)IgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmount / 2) else 0 end)CgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmount / 2) else 0 end)SgstVal,0 as CesVal,0 as StCesVal,0 as OthChrg,(RoundOffSign + CAST(RoundOffAmt as varchar))RndOffAmt,NetAmt as TotInvVal,0 as TotInvValFc "
                       + " ,NULL as TransId,NULL as Transport,(CASE WHEN ISNULL(SM1.Name, '')= '' then PCD.Distance else _PCD.Distance end)Distance,SR.Remark as InvRm,CONVERT(varchar, SR.Date, 103)InvStDt,CONVERT(varchar, SR.Date, 103)InvEndDt,_BillNo as InvNo,_Date as InvDt from SaleReturn SR inner join SupplierMaster SM On SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as nvarchar)) left join SupplierMaster SM1 On SR.SubPartyID = (SM1.AreaCode + CAST(SM1.AccountNo as nvarchar)) and SM1.GroupName = 'SUB PARTY' left join SaleTypeMaster STM on SR.SaleType = STM.TaxName and STM.SaleType = 'SALES' left join StateMaster _SM on SM.State = _SM.StateName left join StateMaster __SM on SM1.State = __SM.StateName left join PinCodeDistance PCD on PCD.ToPinCode = SM.PINCode left join PinCodeDistance _PCD on _PCD.ToPinCode = SM.PINCode "
                       + " OUTER APPLY(Select CONVERT(varchar, MAX(SB.MDate), 103) as _Date, MAX(SB._BillNo)_BillNo from(Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SaleReturn _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo)SB )_SR Where SM.GSTNo != '' and(SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ")"
                       + " Select BillCode, BillNo, HSNType, HSNCode, ItemName, Qty, Amount as UnitPrice,Amount,TaxRate,ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,UQC from("
                       + " Select BillCode, BillNo, ItemName, HSNType, HSNCode, Qty, Amount, TaxRate, ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,(Select Top 1 UnitName from Items _Im WHere _Im.ItemName = _Sales.ItemName)UQC from ( "
                       + " Select BillCode, BillNo, HSNType, HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,ItemName,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100.00) / (100.00 + TaxRate)) else Amount end),2) Amount,Region from ( "
                       + " Select BillCode, BillNo, HSNType, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, ItemName, TaxRate, TaxType, Region from( "
                       + " Select SR.BillCode, SR.BillNo, ItemName, GM.Other as HSNType, (GM.HSNCode) as HSNCode, Qty as Quantity,ROUND(((SE.Amount + ((SE.Amount *CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100))/(CASE WHEN Qty=0 then 1 else Qty end)), 2) as Rate,(SE.Amount + ((SE.Amount * CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100)) Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SaleReturn SR inner join SaleReturnDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((ROUND(((SE.Amount + ((SE.Amount *CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100))/(CASE WHEN Qty=0 then 1 else Qty end)), 2) * 100) / (100 + TaxRate)) else ROUND(((SE.Amount + ((SE.Amount *CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100))/(CASE WHEN Qty=0 then 1 else Qty end)), 2) end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((ROUND(((SE.Amount + ((SE.Amount *CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100))/(CASE WHEN Qty=0 then 1 else Qty end)), 2) * 100) / (100 + TaxRate)) else ROUND(((SE.Amount + ((SE.Amount *CAST((SE.DisStatus + CAST(SE.Discount as varchar)) as Money)) / 100))/(CASE WHEN Qty=0 then 1 else Qty end)), 2) end))) ))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where(SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and SE.Amount > 0 Union All "
                       + " Select SR.BillCode,SR.BillNo,'' as ItemName,'SAC' as HSNType,'' as HSNCode,0 as Quantity,0 as Rate,ROUND((((SE.Packing + SE.Freight) * (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)) +((SE.Packing + SE.Freight + SE.TaxFree) * CS.FreightDhara / 100)), 4) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SaleReturn SR inner join SaleReturnDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where SBillCode IN(" + strFullBillNo + ")) CS Where (SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and(SE.Packing + SE.Freight + SE.TaxFree) > 0 Union All "
                       + " Select SR.BillCode, SR.BillNo, '' as ItemName, 'SAC' as HSNType, '' as HSNCode, 0 as Quantity, 0 as Rate, ROUND(SE.TaxFree, 4) Amount, 0 as TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SaleReturn SR inner join SaleReturnDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer Apply(Select TOP 1 TaxDhara from CompanySetting Where SBillCode IN(" + strFullBillNo + ")) CS Where(SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and SE.TaxFree > 0 Union All "
                       + " Select SR.BillCode, SR.BillNo, '' as ItemName, 'SAC' as HSNType, '' as HSNCode, 0 as Quantity, 0 as Rate, ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money))* (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end), 4) Amount, SR.TaxPer as TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where(SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") UNION ALL "
                       + " Select BillCode, BillNo, '' as ItemName, 'SAC' as HSNType, ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''), '') as HSNCode,'' as ItemName,0 as Quantity,ROUND((CAST((OtherSign + CAST(OtherAmt as varchar)) as money) + PackingAmt)* (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end), 4)  Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SaleReturn SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Where(OtherAmt + PackingAmt) != 0 and(SR.BillCode + ' ' + CAST(SR.BillNo as varchar)) in (" + strFullBillNo + "))_Sales Group by BillCode, BillNo, HSNCode, TaxRate, TaxType, HSNType, ItemName, Region )_Sales Where Amount != 0 )_Sales )_Sale ";

                }
                else
                {
                    strQuery += " Select 'INV' as Typ,(SR.BillCode + CAST(SR.BillNo as varchar))No,CONVERT(varchar, SR.BilLDate, 103)Dt,SM.GSTNo as Gstin, SM.Name as LglNm,null as TrdNm,_SM.StateCode as Pos,SM.Address as Addr1,null as Addr2,SR.Station as LOC, SM.PINCode as Pin,_SM.StateCOde as Stcd,SM.MobileNo as Ph,SM.EmailID as Em,SM1.GSTNo SGSTNo, SM1.Name,null as STrdNm,SM1.Address as SAddr1,null as SAddr2,SR.Station as SLOC,SM1.PINCode as SPin,_SM.StateCOde as SStcd,SR.TaxableAmt as AssVal,(CASE WHEN STM.Region = 'INTERSTATE' then SR.TaxAmount else 0 end)IgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmount / 2) else 0 end)CgstVal,(CASE WHEN STM.Region != 'INTERSTATE' then(SR.TaxAmount / 2) else 0 end)SgstVal,0 as CesVal,0 as StCesVal,0 as OthChrg,(RoundOffSign + CAST(RoundOffAmt as varchar))RndOffAmt,NetAmt as TotInvVal,0 as TotInvValFc "
                        + ",_TR.GSTNo as TransId,SR.Transport,(CASE WHEN ISNULL(SM1.Name, '')= '' then PCD.Distance else _PCD.Distance end)Distance,SR.Remark as InvRm,CONVERT(varchar, SR.BilLDate, 103)InvStDt,CONVERT(varchar, SR.BilLDate, 103)InvEndDt,_BillNo as InvNo,_Date as InvDt from SalesRecord SR inner join SupplierMaster SM On SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as nvarchar)) left join SupplierMaster SM1 On SR.SubPartyID = (SM1.AreaCode + CAST(SM1.AccountNo as nvarchar)) and SM1.GroupName = 'SUB PARTY' left join SaleTypeMaster STM on SR.SalesType = STM.TaxName and STM.SaleType = 'SALES' left join Transport _TR on SR.Transport = _TR.TransportName left join StateMaster _SM on SM.State = _SM.StateName left join StateMaster __SM on SM1.State = __SM.StateName left join PinCodeDistance PCD on PCD.ToPinCode = SM.PINCode left join PinCodeDistance _PCD on _PCD.ToPinCode = SM.PINCode "
                        + "OUTER APPLY(Select CONVERT(varchar, MAX(SB.MDate), 103) as _Date, MAX(SB._BillNo)_BillNo from(Select MAX(_SR.BillDate) as MDate, MAX(_SR.BillNo)_BillNo from SalesRecord _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo UNION ALL Select MAX(_SR.Date) as MDate, MAX(_SR.BillNo)_BillNo from SalesBook _SR Where _SR.BillCode = SR.BillCode and _SR.BillNo < SR.BillNo)SB )_SR Where SM.GSTNo!='' and (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ")"
                        + "Select BillCode, BillNo, ItemName, HSNType,(CASE WHEN HSNType = 'SAC' then (Select Top 1 SACCode from CompanyDetails Where SACCode != '') else HSNCode end)HSNCode,Qty,Rate as UnitPrice ,Amount,TaxRate,ROUND(((Amount * TaxRate) / 100.00), 2) TaxAmt,Region,UQC from (Select BillCode, BillNo, ItemName, HSNType, HSNCode,(CASE WHEN Qty = 0 then '' else CAST(Qty as nvarchar) end)Qty,TaxRate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Rate * 100) / (100 + TaxRate)) else Rate end),3) Rate,ROUND((CASE WHEN TaxType = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end),4) Amount,Region,(Select Top 1 UnitName from Items _Im WHere _Im.ItemName = _Sales.ItemName)UQC from (Select BillCode, BillNo, ItemName, HSNType, HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty, Rate, TaxRate, TaxType, Region from(Select  SR.BillCode, SR.BillNo, GRD.ItemName, GM.Other as HSNType, GM.HSNCode as HSNCode, GRD.Quantity, (GRD.Rate + ((GRD.Rate * (SE.DiscountStatus + SE.Discount)) / 100))Rate, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and GRD.Amount > 0 "
                        + "Union All Select BillCode, BillNo,'' as ItemName,'SAC' as HSNType,'' HSNCode, 0 as Quantity,0 as Rate,ROUND(((((((Amount * (CASE WHen TaxType = 1 then(100.00 / (100.00 + TaxRate)) else 1 end))*(100.00 + DisStatus) / 100.00) *TaxRate) / 100.00) *CS.TaxDhara) / 100.00),4)Amount, TaxPer TaxRate, TaxType, Region from(Select SR.BillCode, SR.BillNo, ROUND(GRD.Amount, 2) Amount, GM.TaxRate, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName, GRD.Quantity as Qty, SR.TaxPer, SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and GRD.Amount > 0 )_SAles OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting Where SBillCode IN (" + strBillCode + ")) CS "
                        + " Union All Select SR.BillCode,SR.BillNo,'' as ItemName,'SAC' as HSNType,'' as HSNCode,0 as Quantity,0 as Rate,ROUND((((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHen TaxIncluded = 1 then(100 / (100 + SR.TaxPer)) else 1 end)) +((GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) * CS.FreightDhara / 100)), 4) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar))  Outer Apply (Select TOP 1 FreightDhara from CompanySetting Where SBillCode IN (" + strBillCode + ")) CS Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and(GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) > 0 "
                        + "Union All Select SR.BillCode,SR.BillNo,'' as ItemName,'SAC' as HSNType,'' as HSNCode,0 as Quantity,0 as Rate,ROUND(GRD.TaxAmt, 4) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer Apply (Select TOP 1 TaxDhara from CompanySetting Where SBillCode IN (" + strBillCode + ")) CS Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + ") and GRD.TaxAmt > 0 "
                        + "Union All Select SR.BillCode,SR.BillNo,'' as ItemName, 'SAC' as HSNType,'' as HSNCode,0 as Quantity,0 as Rate,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 4) Amount,SR.TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strFullBillNo + "))_Sales Group by ItemName, HSNType, HSNCode, TaxRate, TaxType, Region, Rate, BillCode, BillNo)_Sales )_Sales Order by ItemName, HSNType, HSNCode asc,TaxRate,Region, BillCode,BillNo desc ";
                }
              
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                List<EInvoice> _billDetails = new List<EInvoice>();

                if (ds.Tables.Count > 2)
                {
                    DataTable dtSeller = ds.Tables[0], dtBuyerDetails = ds.Tables[1], dtItems = ds.Tables[2];
                    string strDistance = "", strSaleBillNo = "", strFromPinCode = "", strToPinCode = "";
                    double dDistance = 0;
                    int _index = 0;
                    if (dtSeller.Rows.Count > 0)
                    {
                        DataRow SellerRec = dtSeller.Rows[0];

                        foreach (DataRow row in dtBuyerDetails.Rows)
                        {
                            DataRow BuyrRec = dtBuyerDetails.Rows[_index];

                            strSaleBillNo = ConvertStringToObject(BuyrRec["No"]);

                            dDistance = ConvertObjectToDouble(BuyrRec["Distance"], 2);
                            if (Convert.ToString(BuyrRec["Name"]) == "")
                            {
                                BuyrRec["SGSTNo"] = BuyrRec["Gstin"];
                                BuyrRec["Name"] = BuyrRec["LglNm"];
                                BuyrRec["STrdNm"] = BuyrRec["TrdNm"];
                                BuyrRec["SAddr1"] = BuyrRec["Addr1"];
                                BuyrRec["SAddr2"] = BuyrRec["Addr2"];
                                BuyrRec["SLOC"] = BuyrRec["LOC"];
                                BuyrRec["SPin"] = BuyrRec["Pin"];
                                BuyrRec["SStcd"] = BuyrRec["Stcd"];
                            }

                            if (dDistance == 0)
                            {
                                if (strDistance == "")
                                {
                                    string strValue = Microsoft.VisualBasic.Interaction.InputBox("Please enter pin code in master or enter Distance of transportation ! ", "Enter distance manually", "", 400, 300);
                                    if (strValue != "" && strValue != "0")
                                    {
                                        strDistance = strValue;
                                    }
                                }
                                dDistance = ConvertObjectToDouble(strDistance, 2);
                                if (dDistance > 0)
                                {
                                    if (dDistance > 5000 && MainPage.strUserRole.Contains("SUPERADMIN"))
                                    {
                                        MessageBox.Show("Sorry ! Distance is invalid, Please enter correct distance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        dDistance = 0;
                                    }
                                    else
                                    {
                                        BuyrRec["Distance"] = dDistance;
                                        strToPinCode = ConvertStringToObject(BuyrRec["SPin"]);
                                        strFromPinCode = ConvertStringToObject(SellerRec["Pin"]);
                                        DataBaseAccess.SaveDistanceFromPinCode(strFromPinCode, strToPinCode, dDistance);
                                    }
                                }
                            }
                            if (ConvertObjectToDouble(BuyrRec["Distance"], 2) > 0)
                            {
                                DataRow[] rows = dtItems.Select("BillCode + BillNo in ('" + strSaleBillNo + "')");
                                if (rows.Length > 0)
                                {
                                    EInvoice _Bill = new EInvoice();
                                    _Bill.Version = "1.1";

                                    TranDtls _TranDtls = new TranDtls();
                                    _TranDtls.TaxSch = "GST";
                                    _TranDtls.SupTyp = "B2B";
                                    _TranDtls.IgstOnIntra = null;
                                    _TranDtls.RegRev = null;
                                    _TranDtls.EcmGstin = null;
                                    _Bill.TranDtls = _TranDtls;

                                    DocDtls _DocDtls = new DocDtls();
                                    _DocDtls.Typ = ConvertStringToObject(BuyrRec["Typ"]);
                                    _DocDtls.No = ConvertStringToObject(BuyrRec["No"]);
                                    _DocDtls.Dt = ConvertStringToObject(BuyrRec["Dt"]);
                                    _Bill.DocDtls = _DocDtls;

                                    SellerDtls _SellerDtls = new SellerDtls();
                                    _SellerDtls.Gstin = ConvertStringToObject(SellerRec["GSTNo"]);
                                    _SellerDtls.LglNm = ConvertStringToObject(SellerRec["LglNm"]);
                                    _SellerDtls.TrdNm = null;
                                    _SellerDtls.Addr1 = ConvertStringToObject(SellerRec["Addr1"]);
                                    _SellerDtls.Addr2 = ConvertStringToObject(SellerRec["Addr2"]);
                                    _SellerDtls.Loc = ConvertStringToObject(SellerRec["LOC"]);
                                    _SellerDtls.Pin = ConvertObjectToDouble(SellerRec["Pin"]);
                                    _SellerDtls.Stcd = ConvertStringToObject(SellerRec["Stcd"]);
                                    string strPH = ConvertStringToObject(SellerRec["Ph"]);
                                    if(strPH!=null)
                                    _SellerDtls.Ph = strPH.Replace(" ", "");
                                    _SellerDtls.Em = ConvertStringToObject(SellerRec["Em"]);

                                    _Bill.SellerDtls = _SellerDtls;

                                    BuyerDtls _BuyerDtls = new BuyerDtls();
                                    _BuyerDtls.Gstin = ConvertStringToObject(BuyrRec["Gstin"]);
                                    _BuyerDtls.LglNm = ConvertStringToObject(BuyrRec["LglNm"]);
                                    _BuyerDtls.TrdNm = ConvertStringToObject(BuyrRec["TrdNm"]);
                                    _BuyerDtls.Pos = ConvertStringToObject(BuyrRec["Pos"]);
                                    _BuyerDtls.Addr1 = ConvertStringToObject(BuyrRec["Addr1"]);
                                    _BuyerDtls.Addr2 = ConvertStringToObject(BuyrRec["Addr2"]);
                                    _BuyerDtls.Loc = ConvertStringToObject(BuyrRec["LOC"]);
                                    _BuyerDtls.Pin = ConvertObjectToDouble(BuyrRec["Pin"]);
                                    _BuyerDtls.Stcd = ConvertStringToObject(BuyrRec["Stcd"]);
                                    strPH = ConvertStringToObject(SellerRec["Ph"]);
                                    if (strPH != null)
                                        _BuyerDtls.Ph = strPH.Replace(" ", "");
                                  //  _BuyerDtls.Ph = ConvertStringToObject(BuyrRec["Ph"]).Replace(" ", "");
                                    _BuyerDtls.Em = ConvertStringToObject(BuyrRec["Em"]);
                                    _Bill.BuyerDtls = _BuyerDtls;

                                    DispDtls _DispDtls = new DispDtls();
                                    _DispDtls.Nm = ConvertStringToObject(SellerRec["LglNm"]);
                                    _DispDtls.Addr1 = ConvertStringToObject(SellerRec["Addr1"]);
                                    _DispDtls.Addr2 = ConvertStringToObject(SellerRec["Addr2"]);
                                    _DispDtls.Loc = ConvertStringToObject(SellerRec["LOC"]);
                                    _DispDtls.Pin = ConvertObjectToDouble(SellerRec["Pin"]);
                                    _DispDtls.Stcd = ConvertStringToObject(SellerRec["Stcd"]);
                                    _Bill.DispDtls = _DispDtls;

                                    ShipDtls _ShipDtls = new ShipDtls();
                                    _ShipDtls.Gstin = ConvertStringToObject(BuyrRec["Gstin"]);
                                    _ShipDtls.LglNm = ConvertStringToObject(BuyrRec["LglNm"]);
                                    _ShipDtls.TrdNm = ConvertStringToObject(BuyrRec["TrdNm"]);
                                    _ShipDtls.Addr1 = ConvertStringToObject(BuyrRec["Addr1"]);
                                    _ShipDtls.Addr2 = ConvertStringToObject(BuyrRec["Addr2"]);
                                    _ShipDtls.Loc = ConvertStringToObject(BuyrRec["LOC"]);
                                    _ShipDtls.Pin = ConvertObjectToDouble(BuyrRec["Pin"]);
                                    _ShipDtls.Stcd = ConvertStringToObject(BuyrRec["Stcd"]);
                                    _Bill.ShipDtls = _ShipDtls;

                                    ValDtls _ValDtls = new ValDtls();
                                    _ValDtls.AssVal = ConvertObjectToDouble(BuyrRec["AssVal"], 2);
                                    _ValDtls.IgstVal = ConvertObjectToDouble(BuyrRec["IgstVal"], 2);
                                    _ValDtls.CgstVal = ConvertObjectToDouble(BuyrRec["CgstVal"], 2);
                                    _ValDtls.SgstVal = ConvertObjectToDouble(BuyrRec["SgstVal"], 2);
                                    _ValDtls.CesVal = ConvertObjectToDouble(BuyrRec["CesVal"], 2);
                                    _ValDtls.StCesVal = ConvertObjectToDouble(BuyrRec["StCesVal"], 2);
                                    _ValDtls.Discount = 0;
                                    _ValDtls.OthChrg = ConvertObjectToDouble(BuyrRec["OthChrg"], 2);
                                    _ValDtls.RndOffAmt = ConvertObjectToDouble(BuyrRec["RndOffAmt"], 2);
                                    _ValDtls.TotInvVal = ConvertObjectToDouble(BuyrRec["TotInvVal"], 2);
                                    _ValDtls.TotInvValFc = ConvertObjectToDouble(BuyrRec["TotInvValFc"], 2);
                                    _Bill.ValDtls = _ValDtls;

                                    ExpDtls _ExpDtls = new ExpDtls();
                                    _ExpDtls.ShipBNo = null;
                                    _ExpDtls.ShipBDt = null;
                                    _ExpDtls.Port = null;
                                    _ExpDtls.RefClm = null;
                                    _ExpDtls.ForCur = null;
                                    _ExpDtls.CntCode = null;
                                    _ExpDtls.ExpDuty = 0;
                                    _Bill.ExpDtls = _ExpDtls;

                                    EwbDtls _EwbDtls = new EwbDtls();
                                    if (_bEway)
                                    {
                                        _EwbDtls.TransId = ConvertStringToObject(BuyrRec["TransId"]);
                                        _EwbDtls.TransName = ConvertStringToObject(BuyrRec["Transport"]);
                                    }
                                    else
                                    {
                                        _EwbDtls.TransId =null;
                                        _EwbDtls.TransName = null;
                                    }
                                    _EwbDtls.TransMode = null;
                                    _EwbDtls.Distance = ConvertObjectToDouble(BuyrRec["Distance"], 2);
                                    _EwbDtls.TransDocNo = null;
                                    _EwbDtls.TransDocDt = null;
                                    _EwbDtls.VehNo = null;
                                    _EwbDtls.VehType = null;
                                    _Bill.EwbDtls = _EwbDtls;

                                    PayDtls _PayDtls = new PayDtls();
                                    _PayDtls.Nm = null;
                                    _PayDtls.AccDet = null;
                                    _PayDtls.Mode = null;
                                    _PayDtls.FinInsBr = null;
                                    _PayDtls.PayTerm = null;
                                    _PayDtls.PayInstr = null;
                                    _PayDtls.CrTrn = null;
                                    _PayDtls.DirDr = null;
                                    _PayDtls.CrDay = 0;
                                    _PayDtls.PaidAmt = 0;
                                    _PayDtls.PaymtDue = 0;
                                    _Bill.PayDtls = _PayDtls;

                                    RefDtls _RefDtls = new RefDtls();
                                    _RefDtls.InvRm = ConvertStringToObject(BuyrRec["InvRm"]);
                                    DocPerdDtls _DocPerdDtls = new DocPerdDtls();
                                    _DocPerdDtls.InvStDt = ConvertStringToObject(BuyrRec["InvStDt"]);
                                    _DocPerdDtls.InvEndDt = ConvertStringToObject(BuyrRec["InvEndDt"]);
                                    _RefDtls.DocPerdDtls = _DocPerdDtls;

                                    string strPrecInvoice= ConvertStringToObject(BuyrRec["InvNo"]);
                                    PrecDocDtls _PrecDocDtls = new PrecDocDtls();
                                    if (Convert.ToString(BuyrRec["InvNo"]) == "")
                                    {
                                        _PrecDocDtls.InvNo = "N/A";
                                        _PrecDocDtls.InvDt = ConvertStringToObject(BuyrRec["InvStDt"]);
                                    }
                                    else
                                    {
                                        _PrecDocDtls.InvNo = strPrecInvoice;
                                        _PrecDocDtls.InvDt = ConvertStringToObject(BuyrRec["InvDt"]);
                                    }
                                    _PrecDocDtls.OthRefNo = null;
                                    _RefDtls.PrecDocDtls.Add(_PrecDocDtls);

                                    ContrDtls _ContrDtls = new ContrDtls();
                                    _ContrDtls.RecAdvRefr = null;
                                    _ContrDtls.RecAdvDt = null;
                                    _ContrDtls.TendRefr = null;
                                    _ContrDtls.ContrRefr = null;
                                    _ContrDtls.ExtRefr = null;
                                    _ContrDtls.ProjRefr = null;
                                    _ContrDtls.PORefr = null;
                                    _ContrDtls.PORefDt = null;
                                    _RefDtls.ContrDtls.Add(_ContrDtls);
                                    _Bill.RefDtls = _RefDtls;

                                    AddlDocDtls _AddlDocDtls = new AddlDocDtls();
                                    _AddlDocDtls.Url = null;
                                    _AddlDocDtls.Docs = null;
                                    _AddlDocDtls.Info = null;
                                    _RefDtls.DocPerdDtls = _DocPerdDtls;
                                    List<AddlDocDtls> _AddlDocDtlsList = new List<AddlDocDtls>();
                                    _AddlDocDtlsList.Add(_AddlDocDtls);
                                    _Bill.AddlDocDtls = _AddlDocDtlsList;

                                    _Bill.ItemList = GetEInvoiceItemList(rows);
                                    _billDetails.Add(_Bill);

                                    _index++;
                                }
                            }
                        }
                    }
                    if (_billDetails.Count > 0)
                    {
                        var json = new JavaScriptSerializer().Serialize(_billDetails);
                        strJSONString = json.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message + " ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return strJSONString;
        }


        private static List<ItemsDtls> GetEInvoiceItemList(DataRow[] rows)
        {
            List<ItemsDtls> _ItemsDtlsList = new List<ItemsDtls>();
            int _index = 1;
            double dTaxAmt,dAmt;
            foreach (DataRow row in rows)
            {
                List<AttribDtls> _AttribDtlsList = new List<AttribDtls>();
                AttribDtls _AttribDtls = new AttribDtls();
                _AttribDtls.Nm = null;
                _AttribDtls.Val = null;
                _AttribDtlsList.Add(_AttribDtls);

                dTaxAmt = ConvertObjectToDouble(row["TaxAmt"]);
                dAmt = ConvertObjectToDouble(row["Amount"],2);       
                _ItemsDtlsList.Add(new ItemsDtls
                {
                    SlNo = ConvertStringToObject(_index),                    

                    PrdDesc = (ConvertStringToObject(row["HSNType"]) == "SAC") ? null : ConvertStringToObject(row["ItemName"]).Replace("*","X").Replace(":", " ").Replace(";", " ").Replace("/", " ").Replace("  "," "),
                    IsServc = (ConvertStringToObject(row["HSNType"]) == "SAC") ? "Y" : "N",

                    HsnCd = ConvertStringToObject_HSN(row["HSNCode"]),
                    Barcde = null,
                    Qty = ConvertObjectToDouble(row["Qty"], 3),
                    FreeQty = 0,
                    Unit = (ConvertStringToObject(row["UQC"]) != "") ? ConvertStringToObject(row["UQC"]) : null,
                    UnitPrice = ConvertObjectToDouble(row["UnitPrice"], 3),
                    TotAmt = dAmt,
                    Discount = 0,
                    PreTaxVal = 0,
                    AssAmt = dAmt,
                    GstRt = ConvertObjectToDouble(row["TaxRate"], 3),
                    IgstAmt = ((ConvertStringToObject(row["Region"]) == "INTERSTATE") ? ConvertObjectToDouble(dTaxAmt, 2) : 0),
                    CgstAmt = ((ConvertStringToObject(row["Region"]) != "INTERSTATE") ? ConvertObjectToDouble((dTaxAmt/2), 2) : 0),
                    SgstAmt = ((ConvertStringToObject(row["Region"]) != "INTERSTATE") ? ConvertObjectToDouble((dTaxAmt/2), 2) : 0),
                    CesRt = 0,
                    CesAmt = 0,
                    CesNonAdvlAmt = 0,
                    StateCesRt = 0,
                    StateCesAmt = 0,
                    StateCesNonAdvlAmt = 0,
                    OthChrg = 0,
                    TotItemVal = ConvertObjectToDouble((dAmt+dTaxAmt), 2),
                    OrdLineRef = null,
                    OrgCntry = null,
                    PrdSlNo = null,
                    BchDtls = null,
                    AttribDtls = _AttribDtlsList
                });
                _index++;
            }
            return _ItemsDtlsList;
        }

        public static string GetJSONFromStockTransfer(string strBillNo)
        {
            string strJSONString = "";
            try
            {
                List<BillDetail> _billDetails = new List<BillDetail>();

                string strQuery = " Select FCM.GSTNo as userGstin,'O' supplyType,'1' subSupplyType,'INV' docType,(ST.BillCode + CAST(ST.BIllNo as varchar)) as docNo "
                                + ", CONVERT(varchar, ST.Date, 103) as docDate,FCM.GSTNo as fromGstin,FCM.Name as fromTrdName,FCm.Address1 as fromAddr1,'' as fromAddr2,FCM.StateName as fromPlace "
                                + ", FCM.PinCode as fromPincode,FST.StateCode as fromStateCode,FST.StateCode as actualFromStateCode,TCM.GSTNo as toGstin,TCM.Name as toTrdName,TCM.Address1 as toAddr1 "
                                + ",'' as toAddr2,TCM.StateName as toPlace,TCM.PinCode as toPincode,TST.StateCode as toStateCode "
                                + ", TST.StateCode as actualToStateCode,CAST(ST.TotalAmt as Money) as totalValue "
                                + ",0 as cgstValue,0 as sgstValue,0 as igstValue,0 as cessValue,1 as transMode,0 as transDistance,Transport as transporterName "
                                + ",TR.GSTNo transporterId,'' as transDocNo,''transDocDate,'' as vehicleNo,'' as vehicleType,CAST(TotalAmt as Money) as totInvValue,'' mainHsnCode "
                                + ",ISNULL(PD.Distance, 0)Distance from StockTransfer ST "
                                + "LEFT JOIN Transport TR ON TransportName = ST.Transport "
                                + "LEFT JOIN MaterialCenterMaster FCM on FCM.Name = ST.FromMCentre "
                                + "LEFT JOIN MaterialCenterMaster TCM on TCM.Name = ST.ToMCentre "
                                + "LEFT JOIN StateMaster FST on FCM.StateName = FST.StateName "
                                + "LEFT JOIN StateMaster TST on TCM.StateName = TST.StateName "
                                + "LEFT JOIN PinCodeDistance PD on(PD.FromPinCode = FCM.PinCode AND PD.ToPinCode = TCM.PinCode) OR(PD.FromPinCode = TCM.PinCode AND PD.ToPinCode = FCM.PinCode) "
                                + "Where (ST.BillCode+' '+CAST(ST.BillNo as varchar)) in (" + strBillNo + ") Order by ST.BillNo "

                                + " Select BillNo, ItemName as productName,'READYMADE GARMENT' productDesc, HSNCode as hsnCode,Qty as quantity,UnitName as qtyUnit,ROUND(Amount, 2) as taxableAmount ,0 igstRate,0 cgstRate from (Select BillNo, Region, ItemName, HSNCode, SUM(Quantity)Qty, SUM(Amount)Amount, 0 TaxRate, TaxType, UnitName from(Select(ST.BillCode + CAST(ST.BillNo as varchar)) BillNo, SMN.Region, STS.ItemName, ISNULL(IGM.HSNCode,'') as HSNCode, STS.Qty Quantity, (STS.Amount + ((STS.Amount * (STS.SDisPer)) / 100)) Amount, 0 TaxRate, SMN.TaxIncluded as TaxType, STS.Unit UnitName from StockTransfer ST inner join StockTransferSecondary STS on ST.BillCode = STS.BillCode and ST.BillNo = STS.BillNo LEFT JOIN Items IM On STS.ItemName = IM.ItemName LEFT JOIN ItemGroupMaster IGM on IM.GroupName = IGM.GroupName left join SaleTypeMaster SMN On ST.StockType = SMN.TaxName  and SMN.SaleType = 'SALES' Where STS.Amount > 0  and(ST.BillCode + ' ' + CAST(ST.BillNo as varchar)) in (" + strBillNo + "))_Sales Group by BillNo, ItemName, Region, HSNCode, TaxType, UnitName)_Sales Order by Qty desc "

                                + "Insert Into EditTrailDetails ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) Select 'STOCKTRANSFER' as BillType,BillCode,BillNo,DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE())) Date,CAST(TotalAmt as Money) NetAmt,'A' as UpdatedBy,1,0,'WAYBILL_GENERTAED' from StockTransfer ST Where(ST.BillCode + ' ' + CAST(ST.BillNo as varchar)) in (" + strBillNo + ")";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0], dtDetails = ds.Tables[1];
                    string strDistance = "", strSaleBillNo = "", strTransportID = "", strFromPinCode = "", strPinCodeMessage = "", strToPinCode = "";
                    double dDistance = 0;
                    int _index = 0, strTransType = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dDistance = ConvertObjectToDouble(row["Distance"]);
                        strToPinCode = Convert.ToString(row["toPincode"]);
                        strFromPinCode = Convert.ToString(row["fromPincode"]);
                        strSaleBillNo = Convert.ToString(row["docNo"]);

                        if (dDistance == 0)
                        {
                            strPinCodeMessage = "enter pin code in master or ";

                            if (strDistance == "")
                            {
                                string strValue = Microsoft.VisualBasic.Interaction.InputBox("Please " + strPinCodeMessage + "enter Distance of " + row["toPlace"] + " ! ", "Enter distance manually", "", 400, 300);
                                if (strValue != "" && strValue != "0")
                                {
                                    strDistance = strValue;
                                }
                            }
                            dDistance = ConvertObjectToDouble(strDistance);
                            if (dDistance > 0)
                            {
                                if (dDistance > 3000)
                                {
                                    MessageBox.Show("Sorry ! Distance is invalid, Please enter correct distance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dDistance = 0;
                                }
                                else
                                {
                                    DataBaseAccess.SaveDistanceFromPinCode(strFromPinCode, strToPinCode, dDistance);
                                    if (dt.Rows.Count - 1 > _index)
                                    {
                                        SetDistanceInTable(ref dt, strFromPinCode, strToPinCode, dDistance);
                                    }
                                }
                            }
                        }

                        strTransportID = Convert.ToString(row["transporterId"]);
                        if (strTransportID != "")
                        {
                            if (dDistance > 0)
                            {
                                DataRow[] rows = dtDetails.Select("BillNo in ('" + strSaleBillNo + "') ");
                                if (rows.Length > 0)
                                {
                                    string strToGST = Convert.ToString(row["toGstin"]);
                                    if (strToGST == "")
                                        strToGST = "URP";
                                    BillDetail _bill = new BillDetail
                                    {
                                        userGstin = Convert.ToString(row["userGstin"]),
                                        supplyType = Convert.ToString(row["supplyType"]),
                                        subSupplyType = Convert.ToInt32(row["subSupplyType"]),
                                        docType = Convert.ToString(row["docType"]),
                                        docNo = Convert.ToString(row["docNo"]),
                                        docDate = Convert.ToString(row["docDate"]),
                                        transType = strTransType,
                                        fromGstin = Convert.ToString(row["fromGstin"]),
                                        fromTrdName = Convert.ToString(row["fromTrdName"]),
                                        fromAddr1 = Convert.ToString(row["fromAddr1"]),
                                        fromAddr2 = "",
                                        fromPlace = Convert.ToString(row["fromPlace"]),
                                        fromPincode = ConvertObjectToInt(row["fromPincode"]),
                                        fromStateCode = ConvertObjectToInt(row["fromStateCode"]),
                                        actualFromStateCode = ConvertObjectToInt(row["actualFromStateCode"]),
                                        toGstin = strToGST,//Convert.ToString(row["toGstin"]),
                                        toTrdName = Convert.ToString(row["toTrdName"]),
                                        toAddr1 = Convert.ToString(row["toAddr1"]),
                                        toAddr2 = "",
                                        toPlace = Convert.ToString(row["toPlace"]),
                                        toPincode = ConvertObjectToInt(row["toPincode"]),
                                        toStateCode = ConvertObjectToInt(row["toStateCode"]),
                                        actualToStateCode = ConvertObjectToInt(row["actualToStateCode"]),
                                        totalValue = ConvertObjectToDouble(row["totalValue"]),
                                        cgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        sgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        igstValue = ConvertObjectToDouble(row["igstValue"]),
                                        cessValue = 0,
                                        TotNonAdvolVal = 0,
                                        OthValue = 0,
                                        totInvValue = ConvertObjectToDouble(row["totInvValue"]),
                                        transMode = Convert.ToString(row["transMode"]),
                                        transDistance = dDistance,// ConvertObjectToDouble(strDistance),
                                        transporterName = Convert.ToString(row["transporterName"]),
                                        transporterId = Convert.ToString(row["transporterId"]),
                                        transDocNo = "",
                                        transDocDate = "",
                                        vehicleNo = "",
                                        vehicleType = "",
                                        mainHsnCode = ConvertObjectToInt(rows[0]["hsnCode"]),
                                        itemList = GetItemDetails(rows)
                                    };
                                    _billDetails.Add(_bill);
                                }
                            }
                            else { MessageBox.Show("Sorry ! Unable to calculate distance right now  in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { MessageBox.Show("Sorry ! Transporter ID can't be blank in Sale BIll No : " + strSaleBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                        _index++;
                    }
                }
                if (_billDetails.Count > 0)
                {
                    var json = new JavaScriptSerializer().Serialize(_billDetails);
                    strJSONString = json.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message + " ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return strJSONString;
        }

        public static string GetJSONFromPurchaseReturn(string strBillCodeNo)
        {
            string strJSONString = "";
            try
            {
                List<BillDetail> _billDetails = new List<BillDetail>();

                string strQuery = "Select  CD.GSTNo as userGstin ,'O' supplyType ,'1' subSupplyType ,'INV' docType ,(PR.BillCode + CAST(PR.BIllNo as varchar)) as docNo  , CONVERT(varchar, PR.Date, 103) as docDate ,CD.GSTNo as fromGstin ,CD.CompanyName as fromTrdName ,CD.Address as fromAddr1 ,'' as fromAddr2 ,CD.StateName as fromPlace  , CD.PinCode as fromPincode ,FST.StateCode as fromStateCode ,FST.StateCode as actualFromStateCode ,PP.GSTNo as toGstin ,PP.Name as toTrdName ,PP.Address as toAddr1  ,'' as toAddr2 ,PP.State as toPlace ,PP.PinCode as toPincode ,TST.StateCode as toStateCode  , TST.StateCode as actualToStateCode ,CAST(PR.NetAmt as Money) as totalValue  ,0 as cgstValue ,0 as sgstValue ,0 as igstValue ,0 as cessValue ,1 as transMode ,0 as transDistance ,PR.Transport as transporterName  ,TR.GSTNo transporterId,'' as transDocNo ,''transDocDate ,'' as vehicleNo ,'' as vehicleType ,CAST(PR.NetAmt as Money) as totInvValue ,'' mainHsnCode  ,ISNULL(PD.Distance, 0)Distance"
                                + " from PurchaseReturn PR"
                                + " LEFT JOIN Transport TR ON TransportName = PR.Transport"
                                + " LEFT JOIN CompanyDetails CD ON CD.Other = '" + MainPage.strCompanyName + "'"
                                + " LEFT JOIN SupplierMaster PP on(ISNULL(PP.AreaCode, '') + ISNULL(PP.AccountNo, '')) = PR.PurchasePartyID"
                                + " LEFT JOIN StateMaster FST on CD.StateName = FST.StateName"
                                + " LEFT JOIN StateMaster TST on PP.State = TST.StateName"
                                + " LEFT JOIN PinCodeDistance PD on(PD.FromPinCode = CD.PinCode AND PD.ToPinCode = PP.PinCode)"
                                + " OR(PD.FromPinCode = PP.PinCode AND PD.ToPinCode = CD.PinCode) Where(PR.BillCode + ' ' + CAST(PR.BillNo as varchar))"
                                + " in (" + strBillCodeNo + ")"
                                + " Order by PR.BillNo"

                                + " Select BillNo, ItemName as productName,'READYMADE GARMENT' productDesc, HSNCode as hsnCode,Qty as quantity,UnitName as qtyUnit,ROUND(Amount, 2) as taxableAmount,0 igstRate,0 cgstRate from ("
                                + " Select BillNo, Region, ItemName, HSNCode, SUM(Quantity)Qty, SUM(Amount)Amount, 0 TaxRate, TaxType, UnitName from("
                                + " Select(PR.BillCode + CAST(PR.BillNo as varchar)) BillNo, SMN.Region, PRD.ItemName, ISNULL(IGM.HSNCode, '') as HSNCode, PRD.Qty Quantity"
                                + " , (PRD.Amount + ((PRD.Amount * (PRD.SDisPer)) / 100)) Amount, 0 TaxRate, SMN.TaxIncluded as TaxType, PRD.UnitName"
                                + " from PurchaseReturn PR"
                                + " inner join PurchaseReturnDetails PRD on PR.BillCode = PRD.BillCode and PR.BillNo = PRD.BillNo"
                                + "  LEFT JOIN Items IM On PRD.ItemName = IM.ItemName"
                                + " LEFT JOIN ItemGroupMaster IGM on IM.GroupName = IGM.GroupName"
                                + "  left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE'"
                                + " Where PRD.Amount > 0  and(PR.BillCode + ' ' + CAST(PR.BillNo as varchar)) in (" + strBillCodeNo + ")"
                                + " )_Sales Group by BillNo, ItemName, Region, HSNCode, TaxType, UnitName"
                                + " )_Sales Order by Qty desc  "

                                + "Insert Into EditTrailDetails ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) Select 'PURCHASERETURN' as BillType,BillCode,BillNo,DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE())) Date,CAST(PR.NetAmt as Money) NetAmt,'" + MainPage.strLoginName + "' as UpdatedBy,1,0,'WAYBILL_GENERTAED' from PurchaseReturn PR Where(PR.BillCode + ' ' + CAST(PR.BillNo as varchar)) in (" + strBillCodeNo + ")";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0], dtDetails = ds.Tables[1];
                    string strDistance = "", strBillNo = "", strTransportID = "", strFromPinCode = "", strPinCodeMessage = "", strToPinCode = "";
                    double dDistance = 0;
                    int _index = 0, strTransType = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dDistance = ConvertObjectToDouble(row["Distance"]);
                        strToPinCode = Convert.ToString(row["toPincode"]);
                        strFromPinCode = Convert.ToString(row["fromPincode"]);
                        strBillNo = Convert.ToString(row["docNo"]);

                        if (dDistance == 0)
                        {
                            strPinCodeMessage = "enter pin code in master or ";

                            if (strDistance == "")
                            {
                                string strValue = Microsoft.VisualBasic.Interaction.InputBox("Please " + strPinCodeMessage + "enter Distance of " + row["toPlace"] + " ! ", "Enter distance manually", "", 400, 300);
                                if (strValue != "" && strValue != "0")
                                {
                                    strDistance = strValue;
                                }
                            }
                            dDistance = ConvertObjectToDouble(strDistance);
                            if (dDistance > 0)
                            {
                                if (dDistance > 3000)
                                {
                                    MessageBox.Show("Sorry ! Distance is invalid, Please enter correct distance.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dDistance = 0;
                                }
                                else
                                {
                                    DataBaseAccess.SaveDistanceFromPinCode(strFromPinCode, strToPinCode, dDistance);
                                    if (dt.Rows.Count - 1 > _index)
                                    {
                                        SetDistanceInTable(ref dt, strFromPinCode, strToPinCode, dDistance);
                                    }
                                }
                            }
                        }

                        strTransportID = Convert.ToString(row["transporterId"]);
                        if (strTransportID != "")
                        {
                            if (dDistance > 0)
                            {
                                DataRow[] rows = dtDetails.Select("BillNo in ('" + strBillNo + "') ");
                                if (rows.Length > 0)
                                {
                                    string strToGST = Convert.ToString(row["toGstin"]);
                                    if (strToGST == "")
                                        strToGST = "URP";
                                    BillDetail _bill = new BillDetail
                                    {
                                        userGstin = Convert.ToString(row["userGstin"]),
                                        supplyType = Convert.ToString(row["supplyType"]),
                                        subSupplyType = Convert.ToInt32(row["subSupplyType"]),
                                        docType = Convert.ToString(row["docType"]),
                                        docNo = Convert.ToString(row["docNo"]),
                                        docDate = Convert.ToString(row["docDate"]),
                                        transType = strTransType,
                                        fromGstin = Convert.ToString(row["fromGstin"]),
                                        fromTrdName = Convert.ToString(row["fromTrdName"]),
                                        fromAddr1 = Convert.ToString(row["fromAddr1"]),
                                        fromAddr2 = "",
                                        fromPlace = Convert.ToString(row["fromPlace"]),
                                        fromPincode = ConvertObjectToInt(row["fromPincode"]),
                                        fromStateCode = ConvertObjectToInt(row["fromStateCode"]),
                                        actualFromStateCode = ConvertObjectToInt(row["actualFromStateCode"]),
                                        toGstin = strToGST,//Convert.ToString(row["toGstin"]),
                                        toTrdName = Convert.ToString(row["toTrdName"]),
                                        toAddr1 = Convert.ToString(row["toAddr1"]),
                                        toAddr2 = "",
                                        toPlace = Convert.ToString(row["toPlace"]),
                                        toPincode = ConvertObjectToInt(row["toPincode"]),
                                        toStateCode = ConvertObjectToInt(row["toStateCode"]),
                                        actualToStateCode = ConvertObjectToInt(row["actualToStateCode"]),
                                        totalValue = ConvertObjectToDouble(row["totalValue"]),
                                        cgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        sgstValue = ConvertObjectToDouble(row["cgstValue"]),
                                        igstValue = ConvertObjectToDouble(row["igstValue"]),
                                        cessValue = 0,
                                        TotNonAdvolVal = 0,
                                        OthValue = 0,
                                        totInvValue = ConvertObjectToDouble(row["totInvValue"]),
                                        transMode = Convert.ToString(row["transMode"]),
                                        transDistance = dDistance,// ConvertObjectToDouble(strDistance),
                                        transporterName = Convert.ToString(row["transporterName"]),
                                        transporterId = Convert.ToString(row["transporterId"]),
                                        transDocNo = "",
                                        transDocDate = "",
                                        vehicleNo = "",
                                        vehicleType = "",
                                        mainHsnCode = ConvertObjectToInt(rows[0]["hsnCode"]),
                                        itemList = GetItemDetails(rows)
                                    };
                                    _billDetails.Add(_bill);
                                }
                            }
                            else { MessageBox.Show("Sorry ! Unable to calculate distance right now  in Purchase Return Bill No : " + strBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { MessageBox.Show("Sorry ! Transporter ID can't be blank in Purchase Return Bill No : " + strBillNo + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                        _index++;
                    }
                }
                if (_billDetails.Count > 0)
                {
                    var json = new JavaScriptSerializer().Serialize(_billDetails);
                    strJSONString = json.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message + " ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return strJSONString;
        }


    }
    class BillDetail
    {
        public string userGstin { get; set; }
        public string supplyType { get; set; }
        public int subSupplyType { get; set; }
        public string docType { get; set; }
        public string docNo { get; set; }
        public string docDate { get; set; }
        public int transType { get; set; }
        public string fromGstin { get; set; }
        public string fromTrdName { get; set; }
        public string fromAddr1 { get; set; }
        public string fromAddr2 { get; set; }
        public string fromPlace { get; set; }
        public int fromPincode { get; set; }
        public int fromStateCode { get; set; }
        public int actualFromStateCode { get; set; }
        public string toGstin { get; set; }
        public string toTrdName { get; set; }
        public string toAddr1 { get; set; }
        public string toAddr2 { get; set; }
        public string toPlace { get; set; }
        public double toPincode { get; set; }
        public double toStateCode { get; set; }
        public double actualToStateCode { get; set; }
        public double totalValue { get; set; }
        public double cgstValue { get; set; }
        public double sgstValue { get; set; }
        public double igstValue { get; set; }
        public double cessValue { get; set; }

        public double TotNonAdvolVal { get; set; }

        public double OthValue { get; set; }

        public double totInvValue { get; set; }

        public string transMode { get; set; }
        public double transDistance { get; set; }
        public string transporterName { get; set; }
        public string transporterId { get; set; }
        public string transDocNo { get; set; }
        public string transDocDate { get; set; }
        public string vehicleNo { get; set; }
        public string vehicleType { get; set; }     
        public double mainHsnCode { get; set; }
        public List<ItemList> itemList { get; set; }
    }
    
    class ItemList
    {
        public int itemNo { get; set; }
        public string productName { get; set; }
        public string productDesc { get; set; }
        public double hsnCode { get; set; }
        public double quantity { get; set; }
        public string qtyUnit { get; set; }
        public double taxableAmount { get; set; }
        public double sgstRate { get; set; }
        public double cgstRate { get; set; }
        public double igstRate { get; set; }
        public double cessRate { get; set; }

        public double cessNonAdvol { get; set; }

    }
    

}
