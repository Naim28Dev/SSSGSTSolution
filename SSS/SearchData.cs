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
    public partial class SearchData : Form
    {
        public string strSearchData = "", strSelectedData = "", strDepartment = "", strBrand = "", strBarCode = "", strItem = "", strCustomerName = "";
        string strSalesParty = "", strPONumber = "", strSONumber = "", strDesignName = "", strSizeName = "", strColorName = "", strOrderStatus = "";
        double dItemAmt = 0, dTotalAmt = 0;
        public DataTable table = null;
        DateTime dtItemDate = DateTime.Now;
        int ItemQty = 0, TotalQty = 0;
        public bool boxStatus = false,_intStatus=false;
        public ListBox objListBox;
       

        public SearchData(string strData, string strHeader, Keys objKey)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            SetKeyInTextBox(objKey);
            GetDataAndBind();
            SearchRecord();
        }

        public SearchData(string strData, string strHasteParty, string strHeader, Keys objKey)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            strSalesParty = strHasteParty;
            SetKeyInTextBox(objKey);
            GetDataAndBind();
            SearchRecord();
            txtSearch.Focus();
        }

        public SearchData(string strData, string strHeader, DateTime dtItemArivalP, double dItemAmtP, double dTotalAmtP, string strDepartmentP, string strBrandP, string strItemP, string strCustomerNameP, int ItemQtyP, int TotalQtyP, Keys objKey)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            dtItemDate = dtItemArivalP;
            dItemAmt = dItemAmtP;
            dTotalAmt = dTotalAmtP;
            strDepartment = strDepartmentP;
            strBrand = strBrandP;
            strItem = strItemP;
            strCustomerName = strCustomerNameP;
            ItemQty = ItemQtyP;
            TotalQty = TotalQtyP;
            SetKeyInTextBox(objKey);
            GetDataAndBind();
            SearchRecord();
            txtSearch.Focus();
        }

        public SearchData(DataTable dtData, string strData, string strHeader, Keys objKey)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            SetKeyInTextBox(objKey);
            table = new DataTable();
            table = dtData;
            GetDataAndBind();
            SearchRecord();
            txtSearch.Focus();
        }


        private void SetKeyInTextBox(Keys objKey)
        {
            try
            {
                if (Keys.Space != objKey && objKey != Keys.F2)
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
            catch
            {
            }
        }

        private void GetDataAndBind()
        {
            try
            {
                lbSearchBox.Items.Clear();
                if (strSearchData == "SALESPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as SALESPARTY,AccountNo,Name from SupplierMaster where GroupName='SUNDRY DEBTORS' order by Name ");
                }
                else if (strSearchData == "SALESANDCASHPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as SALESANDCASHPARTY,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','CASH A/C') order by Name ");
                }
                else if (strSearchData == "SALESANDPURCHASEPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as SALESANDCASHPARTY,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','SUNDRY CREDITOR') order by Name ");
                }
                else if (strSearchData == "CUSTOMERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select * from (Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as CUSTOMERNAME,AccountNo,Name from SupplierMaster where GroupName in ('SUNDRY DEBTORS','CASH A/C')  UNION ALL Select Distinct SalePartyID as CUSTOMERNAME,0,SalePartyID as Name from SalesBook Where SalePartyID!='' and SalePartyID Not like ('%[0-9]%' ) )Sales Order by Name ");
                }
                else if (strSearchData == "SUBPARTY")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " and dbo.GetFullName(HasteSale)='" + strSalesParty + "' ";
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as SUBPARTY,AccountNo,Name from SupplierMaster where GroupName='SUB PARTY'  " + strSubQuery + " order by Name ");
                    DataRow row = table.NewRow();
                    row[0] = "SELF";
                    table.Rows.InsertAt(row, 0);
                }
                else if (strSearchData == "PURCHASEPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PURCHASEPARTY,AccountNo,Name from SupplierMaster where GroupName='SUNDRY CREDITOR' order by Name");
                }
                else if (strSearchData == "PURCHASEPERSONALPARTY")
                {
                    if (MainPage.strBranchCode == "LDH")
                        table = DataBaseAccess.GetDataTableRecord("Select PURCHASEPERSONALPARTY,GSTNo as OTHERDETAILS,Name,AccountNo from (Select Distinct (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PURCHASEPERSONALPARTY,GSTNo,Name,AccountNo from SupplierMaster where  GroupName='SUNDRY CREDITOR'  Union All Select 'PERSONAL' as PURCHASEPERSONALPARTY ,'' as GSTNo,'PERSONAL' as Name,'' as AccountNo)_Supplier Order by Name ");
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select PURCHASEPERSONALPARTY,GSTNo as OTHERDETAILS,Name,AccountNo from (Select Distinct (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PURCHASEPERSONALPARTY,GSTNo,Name,AccountNo from SupplierMaster where  AreaCode in ('" + MainPage.strBranchCode + "') and GroupName='SUNDRY CREDITOR'  Union All Select 'PERSONAL' as PURCHASEPERSONALPARTY ,'' as GSTNo,'PERSONAL' as Name,'' as AccountNo)_Supplier Order by Name ");
                }
                else if (strSearchData == "PURCHASEPARTYWITHGSTNO")
                {
                    if (MainPage.strBranchCode == "LDH")
                        table = DataBaseAccess.GetDataTableRecord("Select PURCHASEPARTYWITHGSTNO,GSTNo as OTHERDETAILS,Name,AccountNo  from (Select Distinct (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PURCHASEPARTYWITHGSTNO,GSTNo,Name,AccountNo from SupplierMaster Where GroupName='SUNDRY CREDITOR'  Union All Select 'PERSONAL' as PURCHASEPERSONALPARTY ,'' as GSTNo,'PERSONAL' as Name,'' as AccountNo)_Supplier Order by Name ");
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select PURCHASEPARTYWITHGSTNO,GSTNo as OTHERDETAILS,Name,AccountNo  from (Select Distinct (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PURCHASEPARTYWITHGSTNO,GSTNo,Name,AccountNo from SupplierMaster Where AreaCode in ('" + MainPage.strBranchCode + "') and GroupName='SUNDRY CREDITOR'  Union All Select 'PERSONAL' as PURCHASEPERSONALPARTY ,'' as GSTNo,'PERSONAL' as Name,'' as AccountNo)_Supplier Order by Name ");
                }
                else if (strSearchData == "ALLSUPPLIERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select ((AreaCode+AccountNo+' '+Name)+'|'+SM.Other)ALLSUPPLIERNAME,Name,SM.AccountNo from Scheme_SupplierDetails SSD inner join SupplierMaster SM on SM.GroupName='SUNDRY CREDITOR' and SM.Other=SSD.SupplierName inner join SchemeMaster _SM on SSD.SchemeName=_SM.SchemeName and _SM.ActiveStatus=1 Where SM.TransactionLock=0 and SM.BlackList=0 Order by AreaCode,CAST(SM.AccountNo as bigint) ");
                }
                else if (strSearchData == "CASHPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as CASHPARTY,AccountNo,Name from SupplierMaster where GroupName='CASH A/C' order by Name");
                }
                else if (strSearchData == "BANKPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as BANKPARTY,AccountNo,Name from SupplierMaster where GroupName='BANK A/C' order by Name");
                }
                else if (strSearchData == "ALLPARTY")
                {
                    string _sQuery = "";
                    if (strSalesParty != "")
                        _sQuery = " and GroupName='" + strSalesParty + "' ";
                    table = DataBaseAccess.GetDataTableRecord("Select ALLPARTY,Name,AccountNo from (Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as ALLPARTY,AccountNo,Name from SupplierMaster where GroupName !='SUB PARTY' " + _sQuery + ")_Sales order by Name");
                }
                else if (strSearchData == "ALLPARTYWITHADDRESSBOOK")
                {
                    string _sQuery = "";
                    if (strSalesParty != "")
                        _sQuery = " and GroupName='" + strSalesParty + "' ";
                    table = DataBaseAccess.GetDataTableRecord("Select ALLPARTY as ALLPARTYWITHADDRESSBOOK,Name,AccountNo from (Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as ALLPARTY,AccountNo,Name from SupplierMaster where GroupName !='SUB PARTY' " + _sQuery + " UNION ALL Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as ALLPARTY,AccountNo,Name from AddressBook where GroupName !='SUB PARTY' )_Sales order by Name");
                }
                else if (strSearchData == "ALLPARTYNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as ALLPARTYNAME,RTRIM((Other+GSTNo+MobileNo+ContactPerson)) OTHERDETAILS,AccountNo,Name from SupplierMaster order by Name");
                }
                else if (strSearchData == "PACKERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as PACKERNAME from SupplierMaster where Category='PACKER' order by Name");
                }
                else if (strSearchData == "AGENTNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as AGENTNAME from SupplierMaster where Category='AGENT' order by Name");
                }
                else if (strSearchData == "OTHERPARTY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as OTHERPARTY,AccountNo,Name from SupplierMaster where GroupName not in ('SUNDRY CREDITOR','SUNDRY DEBTORS','SUB PARTY') order by Name");
                }
                else if (strSearchData == "JOURNALPARTYNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) as JOURNALPARTYNAME,AccountNo,Name from SupplierMaster where GroupName not in ('CASH A/C','BANK A/C','SUB PARTY') order by Name");
                }
                else if (strSearchData == "MARKETERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct MarketerName from Marketer Where MarketerName!='' Order by MarketerName ");
                }
                else if (strSearchData == "FAIRMARKETERNAME")
                {
                    string _sQuery = "";
                    if (strSalesParty != "")
                        _sQuery = " and OrderCode in ('" + strSalesParty + "') ";
                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct MarketerName as FAIRMARKETERNAME from Marketer Where MarketerName!='' " + _sQuery + " Order by MarketerName ");
                }
                else if (strSearchData == "MARKETER")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)MARKETER from SupplierMaster Where TINNumber in ('MARKETER') OR  Category in ('MARKETER')  Order by Name ");
                    DataRow _row = table.NewRow();
                    _row[0] = "DIRECT";
                    table.Rows.InsertAt(_row, 0);
                }
                else if (strSearchData == "SALESMAN")
                {
                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct MarketerName as SALESMAN from Marketer Where MarketerName!='' Order by MarketerName ");
                }
                else if (strSearchData == "CATEGORYNAME")
                {
                    string _sQuery = "";
                    if (strSalesParty != "")
                        _sQuery = " and GroupName in ('" + strSalesParty + "') ";
                    table = DataBaseAccess.GetDataTableRecord("Select UPPER(CATEGORYNAME)CATEGORYNAME from Category Where (CategoryName!='' " + _sQuery + ") OR ISNULL(GroupName,'')=''  Order By CategoryName");
                }
                else if (strSearchData == "FIXASSETSCATEGORYNAME")
                {
                    string _sQuery = "";
                    if (strSalesParty != "")
                        _sQuery = " and GroupName in ('" + strSalesParty + "') ";
                    table = DataBaseAccess.GetDataTableRecord("Select UPPER(CATEGORYNAME)FIXASSETSCATEGORYNAME from Category Where DepreciationPer>0 " + _sQuery + " Order By CategoryName");
                }
                else if (strSearchData == "STATIONNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct STATIONNAME from Station Where StationName!='' order by StationName");
                }
                else if (strSearchData == "TRANSPORTNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT TRANSPORTNAME from Transport Where ISNULL(GSTNo,'')!=''  Order By TransportName");
                }
                else if (strSearchData == "ITEMNAME")
                {
                    string strSubQuery = "", strQuery = "";
                    if (strSalesParty == "")
                        strSalesParty = "PURCHASE";

                    strSubQuery = " and [SubGroupName]='" + strSalesParty + "' ";

                    if (MainPage.strSoftwareType == "AGENT" && MainPage.startFinDate >= Convert.ToDateTime("04/01/2021") && strSalesParty == "PURCHASE")
                        strQuery = "SELECT ITEMNAME FROM Items _Im inner join ItemGroupMaster IGM on _im.GroupName=IGM.GroupName Where LEN(IGM.HSNCode)>5 " + strSubQuery + " Order By ITEMNAME";
                    else
                        strQuery = "Select DISTINCT ITEMNAME from ITEMS WHere ITEMNAME!='' " + strSubQuery + " Order By ITEMNAME";

                    table = DataBaseAccess.GetDataTableRecord(strQuery);
                }
                else if (strSearchData == "FAIRITEMNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct Items as FAIRITEMNAME from AppOrderBooking Order by Items");
                }
                else if (strSearchData == "BUYERDESIGNNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BuyerDesignName as BUYERDESIGNNAME from ITEMS WHere BuyerDesignName!='' Order By BuyerDesignName ");
                }
                else if (strSearchData == "CASHVCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT VOUCHERCODE as CASHVCODE  from BalanceAmount Where VOUCHERCODE!='' Order By VOUCHERCODE");
                }
                else if (strSearchData == "JVCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT VOUCHERCODE as JVCODE  from BalanceAmount Where AccountStatus='JOURNAL A/C' and VOUCHERCODE!='' Order By VOUCHERCODE");
                }
                else if (strSearchData == "JOURNALVCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct JournalVCode as JOURNALVCODE from CompanySetting Where JournalVCode!='' Order by JournalVCode ");
                }
                else if (strSearchData == "CASHVOUCHERCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct CashVCode from CompanySetting Where CAshVCode!='' Order by CAshVCode");
                }
                else if (strSearchData == "BANKVOUCHERCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct BankVCode as BANKVOUCHERCODE from CompanySetting Where BankVCode!='' Order by BankVCode");
                }
                else if (strSearchData == "CHEQUEBOOKVOUCHERCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BankVCode+'CHQ') as CHEQUEBOOKVOUCHERCODE from CompanySetting Where BankVCode!='' Order by (BankVCode+'CHQ')");
                }
                else if (strSearchData == "ORDERCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct ORDERCODE from (Select Distinct ORDERCODE from OrderBooking  Where OrderCode!=''  UNION ALL Select (OrderCode+'D') as ORDERCODE from CompanySetting )OrderBooking Order by ORDERCODE ");
                }
                else if (strSearchData == "GOODSRCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT ReceiptCode as GOODSRCODE  from GoodsReceive Where ReceiptCode!='' Order By ReceiptCode");
                }
                else if (strSearchData == "SALECODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct SALECODE from (Select DISTINCT BillCode as SALECODE  from SalesRecord Where BillCode!='' UNION ALL Select DISTINCT BillCode as SALECODE  from SalesBook Where BillCode!='' )_Sales Order By SALECODE");
                }
                else if (strSearchData == "PURCHASECODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct * from (Select DISTINCT BillCode as PURCHASECODE  from PurchaseRecord Where BillCode!='' UNION ALL Select DISTINCT BillCode as PURCHASECODE  from PurchaseBook Where BillCode!='' )_Purchase Order By PURCHASECODE");
                }
                else if (strSearchData == "SALERETURNCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as SALERETURNCODE  from SaleReturn Where [EntryType]!='DEBITNOTE' and BillCode!='' Order By BillCode");
                }
                else if (strSearchData == "SALESERVICECODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as SALESERVICECODE  from SaleServiceBook Where BillCode!='' Order By BillCode");
                }
                else if (strSearchData == "PURCHASERETURNCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as PURCHASERETURNCODE  from PurchaseReturn Where [EntryType]!='CREDITNOTE' Order By BillCode");
                }
                else if (strSearchData == "CREDITNOTECODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as CREDITNOTECODE  from PurchaseReturn Where [EntryType]='CREDITNOTE' Order By BillCode");
                }
                else if (strSearchData == "DEBITNOTECODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as DEBITNOTECODE  from SaleReturn Where [EntryType]='DEBITNOTE' Order By BillCode");
                }
                else if (strSearchData == "DESIGNCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as DESIGNCODE  from ITEMS Order By BillCode");
                }
                else if (strSearchData == "FARWARDINGCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT FCode as FARWARDINGCODE  from ForwardingRecord Where FCode!='' Order By FCode");
                }
                else if (strSearchData == "GRETURNCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct GRCOde as GRETURNCODE from GRRecords Where GRCode!='' order by GRCode");
                }
                else if (strSearchData == "COURIERCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct SCode as COURIERCODE from (Select Distinct SCode from CourierRegister Where SCode!='' Union All Select Distinct CourierCode from CourierRegisterIn Where CourierCode!='' ) Courier Order by SCode");
                }
                else if (strSearchData == "ONACCOUNTSALESCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct SaleBillCode ONACCOUNTSALESCODE from OnAccountSalesRecord Order by SaleBillCode ");
                }
                else if (strSearchData == "STOCKCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT StockCode as STOCKCODE  from AddStockTransferVoucher Where StockCode!='' Order by StockCode ");
                }
                else if (strSearchData == "TCSBILLCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as TCSBILLCODE from TCSDetails Order by BillCode ");
                }
                else if (strSearchData == "BGBILLCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as BGBILLCODE from [dbo].[BankGuarantee] Order by BillCode ");
                }
                else if (strSearchData == "ITEM_CODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(ItemName,':',''),',',''),'/',''),'-',''),'.',''),'0',''),'1',''),'2',''),'3',''),'4',''),'5',''),'6',''),'7',''),'8',''),'9',''))  ITEM_CODE  from Items Where SubGroupName='PURCHASE' and DisStatus=0");
                }
                else if (strSearchData == "ONACCOUNTNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct ONACCOUNTNAME from OnAccountParty Order by OnAccountName ");
                }
                else if (strSearchData == "OTHERGROUPNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT GroupName as OTHERGROUPNAME from GroupMaster Order by GroupName");
                }
                else if (strSearchData == "CARTONSIZE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (CSize+'|'+ISNULL(PackingType,'')+'|'+PackingAmt)CARTONSIZE from CartoneSize Order by CSize");
                }
                else if (strSearchData == "CARTONTYPE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Cartone as CARTONTYPE from CartoneType Order by Cartone");
                }
                else if (strSearchData == "GRSNOFORPURCHASE")
                {
                    _intStatus = true;
                    table = DataBaseAccess.GetDataTableRecord("Select CAST(SUBSTRING(GRSNO,CHARINDEX(' ',GRSNo,0)+1,LEN(GRSNO)-CHARINDEX(' ',GRSNo,0)+1)as int)  GRSNOFORPURCHASE  from SalesEntry  Where PurchaseBill='PENDING' and GRSNo Like('" + strSalesParty + " %') Order by CAST(SUBSTRING(GRSNO,CHARINDEX(' ',GRSNo,0)+1,LEN(GRSNO)-CHARINDEX(' ',GRSNo,0)+1)as int) ");
                }
                else if (strSearchData == "SALEBILLNOFORWARDING")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct CAST(BillNo as varchar) as SALEBILLNOFORWARDING,BillNo from SalesRecord Where ForwardingChallan='PENDING' and BillCode='" + strSalesParty + "' Order by BillNo");
                }
                else if (strSearchData == "SALEBILLNOFORSERVICE")
                {
                    string strQuery = "";
                    if (strSalesParty != "")
                    {
                        string[] strFullName = strSalesParty.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery = " Where SalePartyID='" + strFullName[0] + "' ";
                    }
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (BillCode+' '+CAST(BillNo as varchar)) as SALEBILLNOFORSERVICE,BillNo from SalesRecord " + strQuery + " Order by BillNo");
                }
                else if (strSearchData == "SALEBILLNOWTCOURIER")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select * from (Select Distinct (CAST(BillNo as varchar)+'@'+SalePartyID+' '+SM.Name+'@'+ SM.Station) SALEBILLNOWTCOURIER,BillNo from SalesRecord CROSS Apply (Select Name,Station from SupplierMaster Where FourthTransport!='False' and (AreaCode+CAST(AccountNo as nvarchar))=SalePartyID) SM Where BillNo not in (Select SaleBillNo from CourierRegister " + strSalesParty + " UNION ALL Select Distinct (CAST(BillNo as varchar)+'@'+SalePartyID+' '+SM.Name+'@'+ SM.Station) SALEBILLNOWTCOURIER,BillNo from SalesBook CROSS APPLY (Select Name,Station from SupplierMaster Where FourthTransport!='False' and (AreaCode+CAST(AccountNo as nvarchar))=SalePartyID) SM Where BillNo not in (Select SaleBillNo from CourierRegister " + strSalesParty + ") _Sale Order by BillNo ");
                }
                else if (strSearchData == "SALEBILLNOFORRETURN")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,BillDate,103)) as SALEBILLNOFORRETURN,BillNo from SalesRecord " + strSalesParty + " UNION ALL  Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURN,BillNo from SalesBook " + strSalesParty);
                }
                else if (strSearchData == "SALEBILLNOFORRETURNRETAIL")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURNRETAIL,BillNo from SalesBook " + strSalesParty + " Order by BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLNOFORRETURN")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (SUBSTRING(PurchaseBillNo,CHARINDEX(' ',PurchaseBillNo,0)+1,LEN(PurchaseBillNo)-CHARINDEX(' ',PurchaseBillNo,0)+1)+'|'+ISNULL(CONVERT(varchar,(Select Top 1 BillDate from PurchaseRecord PR Where (PR.BillCode+' '+CAST(PR.BillNo as varchar))=SRD.PurchaseBillNo),103),'')) as PURCHASEBILLNOFORRETURN,BillNo from SaleReturnDetails  SRD " + strSalesParty + " Order by BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLNOFORMPURCHASE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct (ReceiptCode+'|'+(CAST(ReceiptNo as varchar))+'|'+ISNULL(CONVERT(varchar,ReceivingDate,103),'')) as PURCHASEBILLNOFORMPURCHASE,ReceiptNo from GoodsReceive " + strSalesParty + " and SaleBill='PENDING'  Order by ReceiptNo ");
                }
                else if (strSearchData == "PURCHASEBILLNOFORMPURCHASE_CREDITNOTE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct ((CAST(ReceiptNo as varchar))+'|'+ISNULL(CONVERT(varchar,ReceivingDate,103),'')) as PURCHASEBILLNOFORMPURCHASE_CREDITNOTE,ReceiptNo from GoodsReceive " + strSalesParty + "  UNION ALL Select Distinct(CAST(BillNo as varchar) + '|' + Convert(nvarchar, Date, 103)) as SALEBILLNOFORRETURN, BillNo from PurchaseBook " + strSalesParty);
                }
                else if (strSearchData == "PURCHASEBILLNOFORMPURCHASE_RETAIL")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct ((CAST(BillNo as varchar))+'|'+ISNULL(CONVERT(varchar,Date,103),'')) as PURCHASEBILLNOFORMPURCHASE_RETAIL,BillNo from PurchaseBook " + strSalesParty + " Order by BillNo ");
                }
                else if (strSearchData == "COURIERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct COURIERNAME from CourierMaster Where CourierName!='' Order By CourierName ");
                }
                else if (strSearchData == "ITEMGROUPNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct GROUPNAME as ITEMGROUPNAME from ItemGroupMaster Where ISNULL(ParentGroup,'')='' Order By GROUPNAME");
                }
                else if (strSearchData == "SUBGROUPNAME")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " and [ParentGroup]='" + strSalesParty + "' ";

                    table = DataBaseAccess.GetDataTableRecord("Select Distinct GROUPNAME as SUBGROUPNAME from GroupMaster  Where ISNULL(ParentGroup,'')!='' " + strSubQuery + " Order By GROUPNAME");
                }
                else if (strSearchData == "UNITNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct UNITNAME from UnitMaster Order By UNITNAME");
                }
                else if (strSearchData == "STATENAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select STATENAME from StateMaster Order by StateName");
                }
                else if (strSearchData == "SALESTYPE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct TaxName as SALESTYPE from SaleTypeMaster Where SaleType='SALES' Order by TaxName");
                }
                else if (strSearchData == "PURCHASETYPE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct TaxName as SALESTYPE from SaleTypeMaster Where SaleType='PURCHASE' Order by TaxName");
                }
                else if (strSearchData == "TAXCATEGORYNAME")
                {
                    string strSubQuery = "";
                    //if (strSalesParty != "")
                    //    strSubQuery = " Where [TaxType]='" + strSalesParty + "' ";

                    table = DataBaseAccess.GetDataTableRecord("Select CATEGORYNAME as TAXCATEGORYNAME from TaxCategory " + strSubQuery + " Order by CategoryName");
                }
                else if (strSearchData == "ALLGRSNO")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " Where dbo.GetFullName(PurchasePartyID)='" + strSalesParty + "'  ";

                    table = DataBaseAccess.GetDataTableRecord("Select (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) ALLGRSNO from GoodsReceive Order by ReceiptNo");
                }
                else if (strSearchData == "PARTYPURCHASEDBILLNO")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " Where PurchasePartyID='" + strSalesParty + "'  ";

                    table = DataBaseAccess.GetDataTableRecord("Select (BillCode+' '+CAST(BillNo as nvarchar)) PARTYPURCHASEDBILLNO from PurchaseRecord " + strSubQuery + " Order by BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLDETAIL")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select GR.ItemName+'|'+((PurchasePartyID+' '+SM.Name)+'|'+(BillCode+' '+CAST(BillNo as varchar))+'|'+PR.DiscountStatus+'|'+PR.Discount+'|'+PR.Dhara+'|'+SM.Category+'|'+GR.DesignName) PURCHASEBILLDETAIL from PurchaseRecord PR Left join GoodsReceiveDetails GR on PR.GRSNO=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) Outer Apply (Select TOP 1 Name,Category from SupplierMaster SM Where (SM.AreaCode+ CAST(SM.AccountNo as nvarchar))=PR.PurchasePartyID)SM   Where PR.BillNo!=0 " + strSubQuery + " Order by PR.BillNo ");
                }
                else if (strSearchData == "SALEBILLDETAILFORRETURN")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select (SBS.ItemName+'|'+SBS.Variant1+'|'+SBS.Variant2+'|'+(CAST(SBS.Qty as varchar))+'|'+(CAST(SBS.Rate as varchar))) SALEBILLDETAILFORRETURN from SalesBookSecondary SBS Where SBS.BillNo!=0 " + strSubQuery + " Order by SBS.BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLDETAILFORRETURN_TRADING")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select (SBS.ItemName+'|'+SBS.Variant1+'|'+SBS.Variant2+'|'+(CAST(SBS.Qty as varchar))+'|'+(CAST(SBS.Rate as varchar))) PURCHASEBILLDETAILFORRETURN_TRADING from PurchaseBookSecondary SBS Cross Apply (Select PurchasePartyID from PurchaseBook _PB Where SBS.BillCode=_PB.BillCode and SBS.BillNo=_PB.BillNo) _PB Where SBS.BillNo!=0 " + strSubQuery + " Order by SBS.BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLDETAILFORRETURN_RETAIL")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select (SBS.ItemName+'|'+SBS.Variant1+'|'+SBS.Variant2+'|'+(CAST(SBS.Qty as varchar))+'|'+(CAST(SBS.Rate as varchar))) PURCHASEBILLDETAILFORRETURN_RETAIL from PurchaseBookSecondary SBS Cross Apply (Select PurchasePartyID from PurchaseBook _PB Where SBS.BillCode=_PB.BillCode and SBS.BillNo=_PB.BillNo) _PB Where SBS.BillNo!=0 " + strSubQuery + " Order by SBS.BillNo ");
                }
                else if (strSearchData == "SALESRETURNBILLDETAILS")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select (SRD.ItemName+'|'+dbo.GetFullName(SR.SalePartyID)+'|'+(SR.BillCode+' '+CAST(SR.BillNo as varchar))+'|'+DisStatus+'|'+CAST(Discount as varchar) +'|'+Dhara+'|'+ CAST(SRD.Qty as varchar)+'|'+CAST(SRD.Amount as varchar)+'|'+CAST(SRD.Packing as varchar)+'|'+CAST(SRD.Freight as varchar)+'|'+CAST(SRD.TaxFree as varchar)+'|'+ CAST((SRD.Amount + SRD.Packing+SRD.Freight+SRD.TaxFree) as varchar)+'|'+SRD.DesignName+'|'+ISNULL((Select TOP 1 Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))=SRD.PurchasePartyID),'')+'|'+CAST(DiscountType as varchar)) SALESRETURNBILLDETAILS from SaleReturn SR inner join SaleReturnDetails SRD on SR.BillCode=SRD.BillCOde and SR.BillNo=SRD.BillNo  Where  PurchaseReturnStatus=0 " + strSubQuery + " Order by SR.BillNo ");
                }
                else if (strSearchData == "PURCHASEDETAILSFORPRETURN")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select (GRD.ItemName+'|'+dbo.GetFullName(GR.SalePartyID)+'|'+(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))+'|-|'+CAST(GR.DisPer as varchar) +'|'+GR.Dhara+'|'+ CAST(GRD.Quantity as varchar)+'|'+CAST(GRD.Amount as varchar)+'|'+CAST(GRD.PackingAmt as varchar)+'|'+CAST(GRD.FreightAmt as varchar)+'|'+CAST(GRD.TaxAmt as varchar)+'|'+ CAST((GRD.Amount + GRD.PackingAmt+GRD.FreightAmt+GRD.TaxAmt) as varchar)+'|'+GRD.DesignName+'|') PURCHASEDETAILSFORPRETURN from GoodsReceive GR inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo Where GR.SaleBill='PENDING' " + strSubQuery + " Order By GR.ReceiptNo ");
                }
                else if (strSearchData == "GROUPNAMEFORMERGE")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select Distinct GroupName as GROUPNAMEFORMERGE from ItemGroupMaster " + strSubQuery + " Order by GroupName ");
                }
                else if (strSearchData == "ITEMNAMEFORMERGE")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = strSalesParty;

                    table = DataBaseAccess.GetDataTableRecord("Select Distinct ItemName as ITEMNAMEFORMERGE from Items " + strSubQuery + " Order by ItemName ");
                }
                else if (strSearchData == "TDSPAYABLEPARTYNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (Reference+' |'+(AreaCode+CAST(AccountNo as varchar)+' '+Name)) TDSPAYABLEPARTYNAME from SupplierMaster Where Category='TDS PAYABLE' Order by Name");
                }
                else if (strSearchData == "ALLBILLCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct SUBSTRING(Description,1,CHARINDEX(' ',Description,0)-1)ALLBILLCODE  from BalanceAmount Where AccountStatus in ('SALES A/C','PURCHASE A/C','SALE RETURN','PURCHASE RETURN')  Order by ALLBILLCODE");
                }
                else if (strSearchData == "BRANCHCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct AreaCode as BRANCHCODE from SupplierMaster Order by AreaCode");
                }
                else if (strSearchData == "OFFERNAME")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = "Where (ActiveStatus=1 OR 'SUPERADMIN' Like('%" + MainPage.strUserRole + "'))  ";
                    table = DataBaseAccess.GetDataTableRecord("Select OFFERNAME from GraceDaysMaster " + strSubQuery + "  Order by OfferName");
                }
                else if (strSearchData == "SCHEMENAME")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " Where (ActiveStatus=1 OR 'SUPERADMIN' Like('%" + MainPage.strUserRole + "'))  ";

                    table = DataBaseAccess.GetDataTableRecord("Select SCHEMENAME from SchemeMaster " + strSubQuery + " Order by SCHEMENAME");
                }
                else if (strSearchData == "ITEMCATEGORYNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT CategoryName as ITEMCATEGORYNAME from ItemCategoryMaster Where CategoryName!='' Order By CategoryName ");
                }
                else if (strSearchData == "SALESPARTYNICKNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct Other as SALESPARTYNICKNAME from SupplierMaster Where GroupName='SUNDRY DEBTORS' and Other!='' Order by Other ");
                }
                else if (strSearchData == "PURCHASEPARTYNICKNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct Other as PURCHASEPARTYNICKNAME from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Other!='' Order by Other ");
                }
                else if (strSearchData == "PURCHASEPARTYNICKNAME_MAPPING")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct SM.Other as PURCHASEPARTYNICKNAME_MAPPING from SupplierMaster SM Where GroupName in ('SUNDRY CREDITOR') and SM.Other!='' and Category!='GRADE A' and AreaCode Like('" + MainPage.strUserBranchCode + "') Order by SM.Other ");
                }
                else if (strSearchData == "ALLPARTYNICKNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct Other as ALLPARTYNICKNAME from SupplierMaster Where GroupName in ('SUNDRY CREDITOR','SUNDRY DEBTORS') and Other!='' Order by Other ");
                }
                else if (strSearchData == "RETAILORDERNO")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select RETAILORDERNO from (Select Distinct (OrderCode+' '+CAST(OrderNo as varchar))RETAILORDERNO,Date from OrderBooking Where OrderType='RETAILORDER')_Order Where RETAILORDERNO not in (Select Distinct PONumber from PurchaseBookSecondary Where ISNULL(PONUMBER,'')!='') Order by Date desc ");
                }
                else if (strSearchData == "PENDINGORDERIMPORT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select ((CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)+'|'+CAST(CAST((CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0))) as numeric(10,0)) as varchar)+'|'+SalePartyID+' '+S_Party+'|'+CASE WHEN SubPartyID='SELF' then SubPartyID else SubPartyID+' '+Haste end+'|'+Convert(varchar,Date,103)) PENDINGORDERIMPORT,Status from OrderBooking OB Cross APPLY (Select Other OPName from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=OB.PurchasePartyID)_OB  Where OPName!='' and Status='PENDING' and OPName in (Select Other as PName from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)='" + strSalesParty + "')  Order by OB.Date,OB.OrderNo desc ");
                }
                else if (strSearchData == "MATERIALCENTER")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (Name)MATERIALCENTER from MaterialCenterMaster Where Name!='' Order by Name ");
                }
                else if (strSearchData == "BRANDNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BrandName from BrandMaster Where BrandName!='' Order by BrandName");
                }
                else if (strSearchData == "BARCODE")
                {
                    if (MainPage.strSoftwareType.Contains("RETAIL"))
                        table = DataBaseAccess.GetDataTableRecord(" Select Distinct BARCODE from PurchaseBookSecondary Where BarCode!='' Order by BarCode ");
                    else
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct Description as BARCODE from ItemSecondary Where Description!='' Order by Description ");
                }
                else if (strSearchData == "BARCODEDETAILS")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct BARCODE as BARCODEDETAILS from ItemStock Order by BarCode");
                }
                else if (strSearchData == "DISTRICTNAME")
                {
                    string strSubQuery = "";
                    if (strSalesParty != "")
                        strSubQuery = " and StateName='" + strSalesParty + "' ";

                    table = DataBaseAccess.GetDataTableRecord("Select Distinct DISTRICTNAME from DistrictDetails Where DistrictName !='' " + strSubQuery + " Order by DistrictName ");
                    if (table.Rows.Count == 0)
                        table = DataBaseAccess.GetDataTableRecord("Select Distinct STATIONNAME from Station Where StationName!='' order by StationName");
                }
                else if (strSearchData == "PETIAGENT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)PETIAGENT from SupplierMaster Where TINNumber in ('PETI AGENT') Order by Name ");
                    DataRow _row = table.NewRow();
                    _row[0] = "DIRECT";
                    table.Rows.InsertAt(_row, 0);
                }
                else if (strSearchData == "SALESMANNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)SALESMANNAME from SupplierMaster Where TINNumber in ('SALES MAN') OR  Category in ('SALES MAN')  Order by Name ");
                    DataRow _row = table.NewRow();
                    _row[0] = "DIRECT";
                    table.Rows.InsertAt(_row, 0);
                }
                else if (strSearchData == "SALESMANMARKETERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)SALESMANMARKETERNAME from SupplierMaster Where TINNumber in ('SALES MAN','MARKETER') OR  Category in ('SALES MAN','MARKETER')  Order by Name ");
                }
                else if (strSearchData == "WAITERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT WAITER WAITERNAME FROM Res_SalesStock WHERE Status != 'BILLED' Order by WAITER ");
                }
                else if (strSearchData == "TABLENO")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT TableNo TABLENO FROM Res_SalesStock WHERE Status != 'BILLED' Order by TableNo");
                }
                else if (strSearchData == "REFERENCENAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Name as REFERENCENAME from AddressBook Where AreaCode Like('%R') Order by Name");
                }
                else if (strSearchData == "ACTIVEREFERENCENAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Name as ACTIVEREFERENCENAME from AddressBook Where AreaCode Like('%R') and Reference!='LOCKED' Order by Name");
                }
                else if (strSearchData == "RECEIVEDBY")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select RECEIVEDBY from (Select ReceivedBy as RECEIVEDBY  from PurchaseBook Where ReceivedBy!='' Group by ReceivedBy UNION ALL Select CountedBy as RECEIVEDBY  from PurchaseBook Where CountedBy!='' Group by CountedBy UNION ALL Select BarCodedBy as RECEIVEDBY  from PurchaseBook Where BarCodedBy!='' Group by BarCodedBy)_Purchase Group by RECEIVEDBY Order by RECEIVEDBY");
                }
                else if (strSearchData == "MASTERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)MASTERNAME from SupplierMaster Where TINNumber in ('MASTER') OR Category in ('MASTER') Order by Name ");
                }
                else if (strSearchData == "BANKNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct BANKNAME  from CardDetails Where BankName!=''  Order by BankName");
                }
                else if (strSearchData == "BGBANKNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("select Distinct BankName from BankGuarantee  Where BankName!=''  Order by BankName");
                }
                else if (strSearchData == "CUSTOMERMOBILE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select * from (select MOBILENO AS CUSTOMERMOBILE from SupplierMaster where MOBILENO!='' and GroupName in ('SUNDRY DEBTORS','CASH A/C')  UNION ALL Select Distinct MobileNo AS CUSTOMERMOBILE from SalesBook Where ISNULL(MobileNo,'')!='' and SalePartyID Not like ('%[0-9]%' ) )Sales Order by CUSTOMERMOBILE ");
                }
                else if (strSearchData == "TCSDNACCOUNT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)TDSDNACCOUNT from SupplierMaster Where GroupName='OTHER CURRENT LIABILITIES' and Category='TCS PAYABLE' Order by Name ");
                }
                else if (strSearchData == "TCSCNACCOUNT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)TDSCNACCOUNT from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' Order by Name ");
                }
                else if (strSearchData == "TDSALLACCOUNT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+AccountNo+' '+Name)TDSCNACCOUNT from SupplierMaster Where GroupName in ('SHORT-TERM LOANS AND ADVANCES','OTHER CURRENT LIABILITIES') and Category in ('TCS RECEIVABLES','TCS PAYABLE' Order by Name ");
                }
                else if (strSearchData == "DNBANKVCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select ((VoucherCode+' '+CAST(VoucherNo as varchar))+'|'+(AccountID+' '+PartyName)+'|'+Amount) as DNBANKVCODE from BalanceAmount BA Where AccountStatus!='OPENING' and Status='CREDIT' and VoucherCode!='' and VoucherCode not in (Select JournalVCode from CompanySetting) and (VoucherCode+CAST(VoucherNo as varchar)) not in (Select (TD.VoucherCode+CAST(TD.VoucherNo as varchar)) from TCSDetails TD) and AccountID in (Select (AreaCode+AccountNo) from SupplierMaster SM Where GroupName='SUNDRY DEBTORS') Order by Date desc");
                }
                else if (strSearchData == "CNBANKVCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select ((VoucherCode+' '+CAST(VoucherNo as varchar))+'|'+(AccountID+' '+PartyName)+'|'+Amount) as CNBANKVCODE from BalanceAmount BA Where AccountStatus!='OPENING' and Status='DEBIT' and VoucherCode!='' and VoucherCode not in (Select JournalVCode from CompanySetting) and (VoucherCode+CAST(VoucherNo as varchar)) not in (Select (TD.VoucherCode+CAST(TD.VoucherNo as varchar)) from TCSDetails TD) and AccountID in (Select (AreaCode+AccountNo) from SupplierMaster SM Where GroupName='SUNDRY CREDITOR') Order by Date desc");
                }
                else if (strSearchData == "TRANSPORTMODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select Distinct TRANSPORTMODE  from PurchaseBook Where ISNULL(TransportMode,'')!=''");
                }
                else if (strSearchData == "ADVANCEBILLNO")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select  (BillCode+' '+CAST(BillNo AS VARCHAR)) as ADVANCEBILLNO  from AdvanceAdjustment Where BillCode!='' AND AdvAdjType='ADVANCE RECEIVE' Order By BillCode");
                }
                else if (strSearchData == "ADVADJUSTMENTCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT BillCode as ADVADJUSTMENTCODE  from AdvanceAdjustment Where BillCode!='' Order By BillCode");
                }
                else if (strSearchData == "ADVANCEBILLNO")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select  (BillCode+' '+CAST(BillNo AS VARCHAR)) as ADVANCEBILLNO  from AdvanceAdjustment Where BillCode!='' and BillCode='" + AdvanceAdjustment.strAdvAdjCode + "' AND AdvAdjType='ADVANCE RECEIVE' Order By BillCode");
                }
                else if (strSearchData == "OFFERAVAILABLE")
                {
                    table = DataBaseAccess.GetDataTableRecord(" exec [GetSaleOffer] '" + strBarCode + "', '" + strItem + "','" + strBrand + "'");
                }
                else if (strSearchData == "MARKETERMAPPINGCODE")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT SerialCode FROM SupplierMapping Order By SerialCode");
                }
                else if (strSearchData == "MARKETERNAMEONBRANCH")
                {
                    table = DataBaseAccess.GetDataTableRecord(" Select Distinct MarketerName as MARKETERNAMEONBRANCH from Marketer MK Where MK.OrderCode Like('" + MainPage.strUserBranchCode + "%')  Order by MarketerName ");
                }
                else if (strSearchData == "ITEMRESTORENT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT Item ITEMRESTORENT from Res_SalesStock Order By Item");
                }
                else if (strSearchData == "BRANDRESTORENT")
                {
                    table = DataBaseAccess.GetDataTableRecord("Select DISTINCT TableNo BRANDRESTORENT from Res_SalesStock Where TableNo!='' Order by TableNo");
                }
                else if (strSearchData == "WAITER")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT WAITER WAITER FROM Res_SalesStock Order by WAITER ");
                }
                else if (strSearchData == "WAITERNAME")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT WAITER WAITERNAME FROM Res_SalesStock WHERE Status != 'BILLED' Order by WAITER ");
                }
                else if (strSearchData == "TABLENO")
                {
                    table = DataBaseAccess.GetDataTableRecord("SELECT DISTINCT TableNo TABLENO FROM Res_SalesStock WHERE Status != 'BILLED' Order by TableNo");
                }
                else if (strSearchData == "SALEBILLCODENO")
                {
                    table = DataBaseAccess.GetDataTableRecord(" Select (BillCode + ' ' + Cast(BillNo as varchar(20)))+'|'+ISNULL(dbo.GetFullName(SalePartyID),'')+'|'+ISNULL(MobileNo,'') SALEBILLCODENO FROM SalesBook ");
                }

                else if (strSearchData == "MONTH" || strSearchData == "GROUPNAME" || strSearchData == "DEALERTYPE" || strSearchData == "FORMTYPE" || strSearchData == "PIECESTYPE" || strSearchData == "DHARA" || strSearchData == "DOCUMENTTYPE" || strSearchData == "PAYMENTTYPE" || strSearchData == "ONACCOUNTFORM" || strSearchData == "TAXTYPE" || strSearchData == "GREATERSMALLER" || strSearchData == "AMOUNTTYPE" || strSearchData == "REVERSECHARGES" || strSearchData == "JOURNALGSTNATURE" || strSearchData == "BILLTYPE" || strSearchData == "CHARTTYPE" || strSearchData == "REQUESTSTATUS" || strSearchData == "REQUESTPRIORITY" || strSearchData == "SALEBILLSTATUS" || strSearchData == "TEMPLATETYPE" || strSearchData == "CASHTYPEPURCHASE" || strSearchData == "CASHTYPESALE" || strSearchData == "CASHTYPE" || strSearchData == "CASHTYPEOTHER" || strSearchData == "GROUPNAMEWITHSUBPARTY" || strSearchData == "WHATSAPPBILLTYPE" || strSearchData == "PETITYPE" || strSearchData == "UNITFORMALNAME" || strSearchData == "CARDTYPE" || strSearchData == "DEPARTMENTNAME" || strSearchData == "USERTYPE" || strSearchData == "RELIGION" || strSearchData == "ALTTYPE" || strSearchData == "ALTSTATUS" || strSearchData == "PURCHASESTATUS" || strSearchData == "DESIGNTYPE" || strSearchData == "MARGINTYPE" || strSearchData == "ITEMSTATUS" || strSearchData == "TEMPLATENAME" || strSearchData == "SOFTWARETYPE" || strSearchData == "ORDERSTATUS")
                {
                    table = GetAdditionalDataTable(strSearchData);
                }

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        lbSearchBox.Items.Add(row[0]);
                    }
                }

                AddNewItem();               
            }
            catch
            {
            }
        }

        private void AddNewItem()
        {
            if (strSearchData == "SALESPARTY" || strSearchData == "SALESANDCASHPARTY" || strSearchData == "CUSTOMERNAME")
            {
                lbSearchBox.Items.Add("ADD NEW SUNDRY DEBTORS");
            }
            else if (strSearchData == "PURCHASEPARTY" || strSearchData == "PURCHASEPERSONALPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW SUNDRY CREDITOR");
            }
            else if (strSearchData == "CASHPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW CASH A/C");
            }
            else if (strSearchData == "BANKPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW BANK A/C");
            }
            else if (strSearchData == "ALLPARTY" || strSearchData== "JOURNALPARTYNAME")
            {
                lbSearchBox.Items.Add("ADD NEW PARTY NAME");
            }            
            else if (strSearchData == "PACKERNAME")
            {
                lbSearchBox.Items.Add("ADD NEW PACKER NAME");
            }
            else if (strSearchData == "AGENTNAME")
            {
                lbSearchBox.Items.Add("ADD NEW AGENT NAME");
            }
            else if (strSearchData == "CATEGORYNAME")
            {
                lbSearchBox.Items.Add("ADD NEW CATEGORY NAME");
            }
            else if (strSearchData == "STATIONNAME")
            {
                lbSearchBox.Items.Add("ADD NEW STATION NAME");
            }
            else if (strSearchData == "TRANSPORTNAME")
            {
                lbSearchBox.Items.Add("ADD NEW TRANSPORT NAME");
            }
            else if (strSearchData == "OTHERGROUPNAME")
            {
                lbSearchBox.Items.Add("ADD NEW OTHERGROUP NAME");
            }
            else if (strSearchData == "SUBPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW SUB PARTY");
            }            
            else if (strSearchData == "SALESMAN")
            {
                lbSearchBox.Items.Add("ADD NEW SALES MAN");
            }
            else if (strSearchData == "SALESMANNAME")
            {
                lbSearchBox.Items.Add("ADD NEW SALES MAN NAME");
            }
            else if (strSearchData == "MARKETERNAME")
            {
                lbSearchBox.Items.Add("ADD NEW MARKETER NAME");
            }
            else if (strSearchData == "MARKETER")
            {
                lbSearchBox.Items.Add("ADD NEW MARKETER");
            }
            else if (strSearchData == "ITEMNAME")
            {
                lbSearchBox.Items.Add("ADD NEW ITEM NAME");
            }
            else if (strSearchData == "ITEMCATEGORYNAME")
            {
                lbSearchBox.Items.Add("ADD NEW ITEM CATEGORY NAME");
            }
            else if (strSearchData == "CARTONSIZE")
            {
                lbSearchBox.Items.Add("ADD NEW CARTON SIZE");
            }
            else if (strSearchData == "CARTONTYPE")
            {
                lbSearchBox.Items.Add("ADD NEW CARTON TYPE");
            }
            else if (strSearchData == "COURIERNAME")
            {
                lbSearchBox.Items.Add("ADD NEW COURIER NAME");
            }
            else if (strSearchData == "ONACCOUNTSALESPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW ONACCOUNT SUNDRY DEBTORS");
            }
            else if (strSearchData == "ONACCOUNTPURCHASEPARTY")
            {
                lbSearchBox.Items.Add("ADD NEW ONACCOUNT SUNDRY CREDITOR");
            }
            else if (strSearchData == "ITEMGROUPNAME" || strSearchData == "ALLGROUPNAME")
            {
                lbSearchBox.Items.Add("ADD NEW GROUP NAME");
            }
            else if (strSearchData == "SUBGROUPNAME")
            {
                lbSearchBox.Items.Add("ADD NEW SUB GROUP NAME");
            }
            else if (strSearchData == "UNITNAME")
            {
                lbSearchBox.Items.Add("ADD NEW UNIT");
            }
            else if (strSearchData == "TAXCATEGORYNAME")
            {
                lbSearchBox.Items.Add("ADD NEW TAX CATEGORY");
            }
            else if (strSearchData == "SALESTYPE")
            {
                lbSearchBox.Items.Add("ADD NEW SALES TYPE");
            }
            else if (strSearchData == "PURCHASETYPE")
            {
                lbSearchBox.Items.Add("ADD NEW PURCHASE TYPE");
            }
            else if (strSearchData == "PENDINGORDERIMPORT")
            {
                lbSearchBox.Items.Add("ADD NEW ORDER DETAIL");
            }
            else if (strSearchData == "BRANDNAME")
            {
                lbSearchBox.Items.Add("ADD NEW BRAND NAME");
            }
            else if (strSearchData == "MATERIALCENTER")
            {
                lbSearchBox.Items.Add("ADD NEW MATERIAL CENTER");
            }
            else if (strSearchData == "MASTERNAME")
            {
                lbSearchBox.Items.Add("ADD NEW MASTER");
            }
            else if (strSearchData == "REFERENCENAME")
            {
                lbSearchBox.Items.Add("ADD NEW REFERENCE NAME");
            }
            if (lbSearchBox.Items.Count > 0 && !boxStatus && strSearchData != "CUSTOMERNAME")
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
                    if (txtSearch.Text == "")
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            lbSearchBox.Items.Add(row[0]);
                        }
                    }
                    else if (table.Rows.Count > 0)
                    {
                        DataRow[] rows = null;
                        if (_intStatus) 
                             rows = table.Select(String.Format(strSearchData + "=" + txtSearch.Text));
                        else if (strSearchData == "PURCHASEPARTYWITHGSTNO")
                            rows = table.Select(String.Format("OTHERDETAILS Like('%" + txtSearch.Text + "%') "));
                        else if (strSearchData == "SALESANDCASHPARTY" || strSearchData == "SALESPARTY" || strSearchData == "SUBPARTY" || strSearchData == "PURCHASEPARTY" || strSearchData == "PURCHASEPERSONALPARTY" || strSearchData == "CASHPARTY" || strSearchData == "BANKPARTY" || strSearchData == "ALLPARTY" || strSearchData == "ALLPARTYNAME" || strSearchData== "JOURNALPARTYNAME" || strSearchData == "OTHERPARTY" || strSearchData == "ALLPARTYWITHADDRESSBOOK" || strSearchData== "ALLSUPPLIERNAME")
                        {
                            string strAccountNo = System.Text.RegularExpressions.Regex.Replace(txtSearch.Text, "[^0-9]", "");
                            if (strAccountNo != "" && txtSearch.Text.Length<6)
                                rows = table.Select(String.Format(" AccountNo Like ('" + strAccountNo + "%') "));
                            else
                            {
                                if (strSearchData == "ALLPARTYNAME" || strSearchData == "PURCHASEPERSONALPARTY")
                                    rows = table.Select(String.Format(strSearchData + " Like ('%" + txtSearch.Text + "%') OR OTHERDETAILS Like('%" + txtSearch.Text + "%') "));
                                else
                                    rows = table.Select(String.Format(strSearchData + " Like ('%" + txtSearch.Text + "%') "));

                                if (rows.Length > 0)
                                {
                                    DataTable _dt = SetCharIndexinDataTable(rows.CopyToDataTable(), "Name");
                                    rows = _dt.Select();
                                }
                            }
                        }
                        else if (strSearchData == "SALEBILLNOWTCOURIER" || strSearchData== "FAIRITEMNAME")
                        {
                            rows = table.Select(String.Format(strSearchData + " Like ('" + txtSearch.Text + "%') "));
                        }
                        else if (strSearchData == "OFFERAVAILABLE")
                        {
                            rows = table.Select(String.Format("OfferName Like ('" + txtSearch.Text + "%') "));
                        }
                        else
                        {
                            rows = table.Select(String.Format(strSearchData + " Like ('%" + txtSearch.Text + "%') "));
                            if (strSearchData != "ITEMNAME")
                            {
                                if (rows.Length > 0)
                                {
                                    DataTable _dt = SetCharIndexinDataTable(rows.CopyToDataTable(), strSearchData);
                                    rows = _dt.Select();
                                }
                            }
                        }

                        if (rows.Length > 0)
                        {
                            foreach (DataRow row in rows)
                            {
                                lbSearchBox.Items.Add(row[0]);
                            }
                        }
                    }
                    AddNewItem();
                }
                else
                {
                    GetDataAndBind();
                }
            }
            catch
            {
            }
        }

        private DataTable SetCharIndexinDataTable(DataTable _dt, string strColumnName)
        {
            string _strSearch = txtSearch.Text, strValue = "";
            if (_strSearch != "")
            {
                _dt.Columns.Add("CharIndex", typeof(Int64));
                int _charIndex = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strValue = Convert.ToString(row[strColumnName]);
                    _charIndex = strValue.IndexOf(_strSearch);
                    row["CharIndex"] = _charIndex;
                }

                DataView _dv = _dt.DefaultView;
                _dv.Sort = "CharIndex,"+ strSearchData+" asc ";

                return _dv.ToTable();
            }
            else
                return _dt;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                    if (strSelectedData == "" && txtSearch.Text != "")
                    {
                        if (strSearchData == "CUSTOMERNAME" || strSearchData == "RECEIVEDBY" || strSearchData == "BANKNAME" || strSearchData == "BGBANKNAME" || strSearchData == "CUSTOMERMOBILE" || strSearchData == "TRANSPORTMODE" || strSearchData== "PURCHASEBILLNOFORMPURCHASE_CREDITNOTE")
                            strSelectedData = txtSearch.Text.Trim();

                        if (strSearchData == "CUSTOMERNAME")
                            strSelectedData = System.Text.RegularExpressions.Regex.Replace(strSelectedData, @"[\d-]", string.Empty);
                    }

                    if (strSelectedData != "" || boxStatus)
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
                    if (strSelectedData == "")
                    {
                        if (boxStatus)
                            objListBox = lbSearchBox;
                        else
                            strSelectedData = txtSearch.Text;
                    }

                    if (strSearchData != "" || boxStatus)
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
                if (strSelectedData != "")
                {
                    this.Close();
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
                this.Close();
            }

        }

        private void SearchData_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bool closeStatus = true;
                string strNewSelectedName = strSelectedData,strText=GetNewTypedText;
                if (strSelectedData == "ADD NEW SUNDRY DEBTORS")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "SUNDRY DEBTORS", "");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW SUNDRY CREDITOR")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "SUNDRY CREDITOR", "");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW CASH A/C")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "CASH A/C", "");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW BANK A/C")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "BANK A/C", "");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW PARTY NAME")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, strSalesParty, "");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }                
                else if (strSelectedData == "ADD NEW PACKER NAME")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "PACKER");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }              
                else if (strSelectedData == "ADD NEW AGENT NAME")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "AGENT");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW SALES MAN NAME")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "SALES MAN");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }               
                else if (strSelectedData == "ADD NEW SUB PARTY")
                {
                    if (SubPartyAddPermission)
                    {
                        NewSubParty objNsp = new NewSubParty(true,strSalesParty,strText);
                        objNsp.ShowInTaskbar = true;
                        objNsp.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objNsp.ShowDialog();
                        strNewSelectedName = objNsp.strNewAddedSubParty;
                    }                   
                    closeStatus = false;
                }                
                else if (strSelectedData == "ADD NEW CATEGORY NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        CategoryMaster objCategory = new CategoryMaster(true, strSalesParty, strText);
                        objCategory.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCategory.ShowDialog();
                        strNewSelectedName = objCategory.StrAddedCategory;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW STATION NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        StationMaster objStation = new StationMaster(true, strText);
                        objStation.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStation.ShowDialog();
                        strNewSelectedName = objStation.StrAddedName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW TRANSPORT NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        TransportMaster objTransport = new TransportMaster(true, strText);
                        objTransport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTransport.ShowDialog();
                        strNewSelectedName = objTransport.StrAddedTransport;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW OTHERGROUP NAME")
                {
                    if (GSTMasterAddPermission)
                    {
                        GroupMaster objGroupMaster = new GroupMaster(true, strText);
                        objGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objGroupMaster.ShowDialog();
                        strNewSelectedName = objGroupMaster.StrAddedMaster;
                    }
                    closeStatus = false;
                }                
                else if (strSelectedData == "ADD NEW SALES MAN")
                {
                    if (OtherMasterAddPermission)
                    {
                        SalesManMaster objNewMarketer = new SalesManMaster(1, strText);
                        objNewMarketer.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objNewMarketer.ShowDialog();
                        strNewSelectedName = objNewMarketer.StrSalesManName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW MARKETER NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "SALES MAN");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                        //MarketerMaster objNewMarketer = new MarketerMaster(1, strText);
                        //objNewMarketer.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        //objNewMarketer.ShowDialog();
                        //strNewSelectedName = objNewMarketer.StrAgentName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW MARKETER")
                {
                    if (OtherMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "MARKETER");
                        objSupplier.txtName.Text = strText;
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                        //MarketerMaster objNewMarketer = new MarketerMaster(1, strText);
                        //objNewMarketer.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        //objNewMarketer.ShowDialog();
                        //strNewSelectedName = objNewMarketer.StrAgentName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW ITEM NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        if (MainPage.bArticlewiseOpening)
                        {
                            ItemMaster objItemMaster = new ItemMaster(true);
                            objItemMaster.strItemName = strText;
                            objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objItemMaster.ShowInTaskbar = true;
                            objItemMaster.ShowDialog();
                            strNewSelectedName = objItemMaster.StrAddedDesignName;
                        }
                        else
                        {
                            DesignMaster objItemMaster = new DesignMaster(true);
                            objItemMaster.strItemName = strText;
                            objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objItemMaster.ShowDialog();
                            strNewSelectedName = objItemMaster.StrAddedDesignName;
                        }
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW CARTON SIZE")
                {
                    if (OtherMasterAddPermission)
                    {
                        CartonSizeMaster objCartoneMaster = new CartonSizeMaster(true, strText);
                        objCartoneMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCartoneMaster.ShowDialog();
                        strNewSelectedName = objCartoneMaster.StrAddedName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW CARTON TYPE")
                {
                    if (OtherMasterAddPermission)
                    {
                        CartonTypeMaster objCartoneMaster = new CartonTypeMaster(true, strText);
                        objCartoneMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCartoneMaster.ShowDialog();
                        strNewSelectedName = objCartoneMaster.StrAddedName;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW COURIER NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        CourierMaster objCourierMaster = new CourierMaster(1, strText);
                        objCourierMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCourierMaster.ShowDialog();
                        strNewSelectedName = CourierMaster.strAddedCourier;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW GROUP NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        ItemGroupMaster ObjGroupMaster = new ItemGroupMaster(true, strText);
                        ObjGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        ObjGroupMaster.ShowDialog();
                        strSelectedData = ObjGroupMaster.StrAddedGroup;
                    }
                    closeStatus = false;
                }             
                else if (strSelectedData == "ADD NEW UNIT")
                {
                    if (OtherMasterAddPermission)
                    {
                        UnitMaster objUnitMaster = new UnitMaster(true, strText);
                        objUnitMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objUnitMaster.ShowDialog();
                        strSelectedData = objUnitMaster.StrAddedUnit;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW TAX CATEGORY")
                {
                    if (GSTMasterAddPermission)
                    {
                        TaxCategory objTaxCategory = new TaxCategory(true, strText);
                        objTaxCategory.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTaxCategory.ShowDialog();
                        strSelectedData = objTaxCategory.StrAddedTax;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW SALES TYPE")
                {
                    if (GSTMasterAddPermission)
                    {
                        SalesTypeMaster objSalesTypeMaster = new SalesTypeMaster(true, strText);
                        objSalesTypeMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSalesTypeMaster.ShowDialog();
                        strSelectedData = objSalesTypeMaster.StrAddedSaleType;
                    }
                    closeStatus = false;
                }
               
                else if (strSelectedData == "ADD NEW PURCHASE TYPE")
                {
                    if (GSTMasterAddPermission)
                    {
                        PurchaseTypeMaster objPurchaseTypeMaster = new PurchaseTypeMaster(true, strText);
                        objPurchaseTypeMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurchaseTypeMaster.ShowDialog();
                        strSelectedData = objPurchaseTypeMaster.StrAddedTax;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW ORDER DETAIL")
                {
                    OrderBooking objOrderBooking = new OrderBooking(true);
                    objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objOrderBooking.ShowDialog();
                    strSelectedData = objOrderBooking.strAddedOrderDetails;
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW ITEM CATEGORY NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        ItemCategoryMaster objItemCategoryMaster = new ItemCategoryMaster(true, strText);
                        objItemCategoryMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objItemCategoryMaster.ShowDialog();
                        strSelectedData = objItemCategoryMaster.StrAddedName;
                    }
                    closeStatus = false;
                }
                else if(strSelectedData== "ADD NEW BRAND NAME")
                {
                    if (OtherMasterAddPermission)
                    {
                        BrandMaster objBrandMaster = new BrandMaster(true, strText);
                        objBrandMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objBrandMaster.ShowDialog();
                        strSelectedData = objBrandMaster.StrAddedBrand;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW MATERIAL CENTER")
                {
                    if (OtherMasterAddPermission)
                    {
                        MaterialCenterMaster objMaterialCenterMaster = new MaterialCenterMaster(true, strText);
                        objMaterialCenterMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objMaterialCenterMaster.ShowDialog();
                        strSelectedData = objMaterialCenterMaster.strAddedMCentre;
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW MASTER")
                {
                    if (AccountMasterAddPermission)
                    {
                        SupplierMaster objSupplier = new SupplierMaster(1, "", "MASTER");
                        objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplier.txtName.Text = strText;
                        objSupplier.ShowDialog();
                        strNewSelectedName = objSupplier.strAccountName;
                        GetDataAndBind();
                    }
                    closeStatus = false;
                }
                else if (strSelectedData == "ADD NEW REFERENCE NAME")
                {
                    if (AccountMasterAddPermission)
                    {
                        ReferenceBook objAddress = new ReferenceBook(true, strText);
                        objAddress.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objAddress.ShowDialog();
                        strSelectedData = objAddress.strAddedName;
                    }
                    closeStatus = false;
                }

                if (strSelectedData.Contains("ADD NEW "))
                {
                    if (strNewSelectedName != strSelectedData)
                        strSelectedData = strNewSelectedName;
                    else
                        strSelectedData = "";
                }

                if (strSelectedData == "" && !closeStatus)
                {
                    e.Cancel = true;
                }
            }
            catch
            {
            }
        }

        private string GetNewTypedText
        {
            get
            {
                if (!txtSearch.Text.Contains("ADD") && !txtSearch.Text.Contains("NEW"))
                    return txtSearch.Text.Trim();
                    return "";
            }
        }

        private bool OtherMasterAddPermission
        {
            get
            {
                if (MainPage.mymainObject.bAccountMasterAdd)
                    return true;
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private bool GSTMasterAddPermission
        {
            get
            {
                if (MainPage.mymainObject.bGSTMasterEntry)
                    return true;
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private bool AccountMasterAddPermission
        {
            get
            {
                if (MainPage.mymainObject.bPartyMasterAdd)
                    return true;
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private bool SubPartyAddPermission
        {
            get
            {
                if (MainPage.mymainObject.bSubPartyAdd)
                    return true;
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
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

        private DataTable GetAdditionalDataTable(string strName)
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
            else if (strName == "GROUPNAME")
            {
                dt.Columns.Add("GROUPNAME", typeof(String));
                string[] strGroupName = { "BANK A/C", "BRANCH / DIVISIONS", "CAPITAL ACCOUNT", "CAPITAL WORK IN PROGRESS", "CASH A/C", "CASH IN HAND", "COST OF MATERIAL TRADED", "CREDITOR EXPENSE", "CREDITOR / MISCELLANEOUS", "CURRENT INVESTMENTS", "DEBTOR / MISCELLANEOUS", "DEFERRED TAX ASSETS(NET)", "DEFERRED TAX LIABILITIES", "DIRECT EXPENSE A/C", "DIRECT INCOME A/C", "DEPOSITS (ASSET)", "DUTIES & TAXES", "EMPLOYEE BENEFIT EXPENSE", "DEPRECIATION", "FIXED ASSETS", "FURNITURE/OFFICE ASSETS", "INDIRECT EXPENSE A/C", "INDIRECT INCOME A/C", "INTANGIBLE ASSETS", "INTANGIBLE ASSETS UNDER DEVELOPMENT", "LAND / BUILDING", "LOAN (ASSETS)", "LOAN (LIABILITY)", "LONG-TERM BORROWINGS", "LONG TERM LOANS AND ADVANCES", "LONG-TERM PROVISIONS", "NON CURRENT INVESTMENTS", "OTHER CURRENT ASSETS", "OTHER CURRENT LIABILITIES", "OTHER EXPENSES", "OTHER INCOME", "OTHER LONG TERM LIABILITIES", "PROVISIONS", "SUNDRY CREDITOR", "PROFIT & LOSS A/C", "RESERVES & SURPLUSES", "RETAINED EARNINGS", "REVENUE FROM OPERATIONS", "SUNDRY DEBTORS", "SECURED LOANS", "SELLING & DISTRIBUTION EXPENSES", "SHORT TERM BORROWINGS", "SHORT TERM PROVISIONS", "SHORT-TERM LOANS AND ADVANCES", "SUSPENCES A/C", "TRADE PAYABLES", "UNSECURED LOANS", "VEHICLE A/C" };
                foreach (string strGroup in strGroupName)
                {
                    DataRow row = dt.NewRow();
                    row["GROUPNAME"] = strGroup;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "GROUPNAMEWITHSUBPARTY")
            {
                dt.Columns.Add("GROUPNAMEWITHSUBPARTY", typeof(String));
                string[] strGroupName = { "BANK A/C", "BRANCH / DIVISIONS", "CAPITAL ACCOUNT", "CAPITAL WORK IN PROGRESS", "CASH A/C", "CASH IN HAND", "CREDITOR EXPENSE", "CREDITOR / MISCELLANEOUS", "CURRENT INVESTMENTS", "DEBTOR / MISCELLANEOUS", "DEFERRED TAX ASSETS(NET)", "DEFERRED TAX LIABILITIES", "DIRECT EXPENSE A/C", "DIRECT INCOME A/C", "DEPOSITS (ASSET)", "DUTIES & TAXES", "FIXED ASSETS", "FURNITURE/OFFICE ASSETS", "INDIRECT EXPENSE A/C", "INDIRECT INCOME A/C", "INTANGIBLE ASSETS", "INTANGIBLE ASSETS UNDER DEVELOPMENT", "LAND / BUILDING", "LOAN (ASSETS)", "LOAN (LIABILITY)", "LONG-TERM BORROWINGS", "LONG-TERM PROVISIONS", "NON CURRENT INVESTMENTS", "OTHER LONG TERM LIABILITIES", "PROVISIONS", "SUNDRY CREDITOR", "PROFIT & LOSS A/C", "RESERVES & SURPLUSES", "RETAINED EARNINGS", "SUNDRY DEBTORS", "SECURED LOANS", "SHORT TERM BORROWINGS", "SUSPENCES A/C", "SUB PARTY", "UNSECURED LOANS", "VEHICLE A/C" };
                foreach (string strGroup in strGroupName)
                {
                    DataRow row = dt.NewRow();
                    row["GROUPNAMEWITHSUBPARTY"] = strGroup;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "STATENAME")
            {
                dt.Columns.Add("STATENAME", typeof(String));
                string[] strGroupName = { "ANDHRA PRADESH", "ARUNACHAL PRADESH", "ASSAM", "BIHAR", "CHHATTISGARH", "DELHI", "GOA", "GUJARAT", "HARYANA", "HIMACHAL PRADESH", "JAMMU AND KASHMIR", "JHARKHAND", "KARNATAKA", "KERALA", "MADHYA PRADESH", "MANIPUR", "MAHARASHTRA", "MEGHALAYA", "MIZORAM", "NAGALAND", "ORISSA", "PUNJAB", "RAJASTHAN", "SIKKIM", "TAMIL NADU", "TRIPURA", "UTTAR PRADESH", "UTTARAKHAND", "WEST BENGAL" };
                foreach (string strGroup in strGroupName)
                {
                    DataRow row = dt.NewRow();
                    row["STATENAME"] = strGroup;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "DEALERTYPE")
            {
                dt.Columns.Add("DEALERTYPE", typeof(String));
                string[] strDealerTypes = { "COMPOSITION DEALER", "CONSUMER", "INTER STATE DEALER", "REGISTERED DEALER", "UNREGISTERED DEALER" };
                foreach (string strType in strDealerTypes)
                {
                    DataRow row = dt.NewRow();
                    row["DEALERTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "FORMTYPE")
            {
                dt.Columns.Add("FORMTYPE", typeof(String));
                string[] strFormTypes = { "C FORM", "F FORM", "H FORM", "I FORM", "J FORM" };
                foreach (string strType in strFormTypes)
                {
                    DataRow row = dt.NewRow();
                    row["FORMTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "PIECESTYPE")
            {
                dt.Columns.Add("PIECESTYPE", typeof(String));
                string[] strTypes = { "LOOSE", "PETI", "PARCEL" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["PIECESTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "DOCUMENTTYPE")
            {
                dt.Columns.Add("DOCUMENTTYPE", typeof(String));
                string[] strTypes = { "BILL", "CHEQUE", "DOCUMENTS", "INVITATION", "LEDGER", "OTHER", "STATEMENT" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["DOCUMENTTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "PAYMENTTYPE")
            {
                dt.Columns.Add("PAYMENTTYPE", typeof(String));
                string[] strTypes = { "BOTH", "CASH", "CREDIT", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["PAYMENTTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "ONACCOUNTFORM")
            {
                dt.Columns.Add("ONACCOUNTFORM", typeof(String));
                string[] strTypes = { "BOTH", "C FORM", "38 FORM", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["ONACCOUNTFORM"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "DHARA")
            {
                dt.Columns.Add("DHARA", typeof(String));
                string[] strTypes = { "NORMAL", "SNDHARA", "PREMIUM" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["DHARA"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "TAXTYPE")
            {
                dt.Columns.Add("TAXTYPE", typeof(String));
                string[] strTypes = { "GOODS", "SERVICES" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["TAXTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "GREATERSMALLER")
            {
                dt.Columns.Add("GREATERSMALLER", typeof(String));
                string[] strTypes = { ">", "<" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["GREATERSMALLER"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "AMOUNTTYPE")
            {

                dt.Columns.Add("AMOUNTTYPE", typeof(String));
                string[] strTypes = { "NET PRICE", "MRP" };//
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["AMOUNTTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "REVERSECHARGES")
            {
                dt.Columns.Add("REVERSECHARGES", typeof(String));
                string[] strTypes = { "BASAED ON DAILY LIMIT", "COMPULSORY (REG. DEALER)", "COMPULSORY (UNREG. DEALER)", "SERVICE IMPORT", "NOT APPLICABLE" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["REVERSECHARGES"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "JOURNALGSTNATURE")
            {
                dt.Columns.Add("JOURNALGSTNATURE", typeof(String));
                string[] strTypes = { "NOT APPLICABLE/NON-GST", "RCM/UNREG. EXPENSE", "REGISTERED EXPENSE (B2B)", "CR. NOTE RECEIVED AGAINST PURCHASE", "DR. NOTE RECEIVED AGAINST PURCHASE", "ADJUSTMENT", "SALARY", "OFFICE DISCOUNT", "SUPPLIER DISCOUNT", "INTER BRANCH", "ADVANCE" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["JOURNALGSTNATURE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "BILLTYPE")
            {
                dt.Columns.Add("BILLTYPE", typeof(String));
                string[] strTypes = { "BANK", "CASH", "CREDITNOTE", "DEBITNOTE", "JOURNAL", "ORDER", "PURCHASE", "PURCHASERETURN", "SALERETURN", "SALES", "SALESENTRY", "SALESERVICE" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["BILLTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CHARTTYPE")
            {
                dt.Columns.Add("CHARTTYPE", typeof(String));
                string[] strTypes = { "AREA", "COLUMN", "DOUGHNUT", "PIE", "RANGECOLUMN", "STACKEDCOLUMN" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CHARTTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "REQUESTSTATUS")
            {
                dt.Columns.Add("REQUESTSTATUS", typeof(String));
                string[] strTypes = { "ADDED", "APPROVAL PENDING", "APPROVED", "DOWNLOADED", "REJECT", "REQUESTED", "RESCHEDULED", "PAID", "STOP PAYMENT", "UPLOADED" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["REQUESTSTATUS"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "REQUESTPRIORITY")
            {
                dt.Columns.Add("REQUESTPRIORITY", typeof(String));
                string[] strTypes = { "HIGH", "MEDIUM", "REGULAR" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["REQUESTPRIORITY"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "SALEBILLSTATUS")
            {
                dt.Columns.Add("SALEBILLSTATUS", typeof(String));
                string[] strTypes = { "BILLED", "SHIPPED", "STOCK" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["SALEBILLSTATUS"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "TEMPLATETYPE")
            {
                dt.Columns.Add("TEMPLATETYPE", typeof(String));
                string[] strTypes = { "BANK", "WAYBILL","EINVOICE", "CASH", "JOURNAL", "ACCOUNTMASTER", "DESIGNMASTER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["TEMPLATETYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CASHTYPE")
            {
                dt.Columns.Add("CASHTYPE", typeof(String));
                string[] strTypes = { "CASH PARTY", "CASH PURCHASE", "COST CENTRE", "CREDIT PURCHASE", "GRADE A", "GRADE B", "GRADE C", "GRADE R", "GRADE Z", "DISPUTE", "PETI AGENT", "SALES MAN", "MARKETER", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CASHTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CASHTYPEPURCHASE")
            {
                dt.Columns.Add("CASHTYPEPURCHASE", typeof(String));
                string[] strTypes = { "CASH PURCHASE", "CREDIT PURCHASE", "GRADE A" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CASHTYPEPURCHASE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CASHTYPESALE")
            {
                dt.Columns.Add("CASHTYPESALE", typeof(String));
                string[] strTypes = { "CASH PARTY", "GRADE A", "GRADE B", "GRADE C", "GRADE R", "GRADE Z", "DISPUTE" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CASHTYPESALE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CASHTYPEOTHER")
            {
                dt.Columns.Add("CASHTYPEOTHER", typeof(String));
                string[] strTypes = { "COST CENTRE", "PETI AGENT", "PREVILEGE ACCOUNT", "OTHER", "SALES MAN", "MARKETER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CASHTYPEOTHER"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "WHATSAPPBILLTYPE")
            {
                dt.Columns.Add("WHATSAPPBILLTYPE", typeof(String));
                string[] strTypes = { "SALEBILL", "LEDGER", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["WHATSAPPBILLTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "WHATSAPPBILLTYPE")
            {
                dt.Columns.Add("WHATSAPPBILLTYPE", typeof(String));
                string[] strTypes = { "SALEBILL", "LEDGER", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["WHATSAPPBILLTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "PETITYPE")
            {
                dt.Columns.Add("PETITYPE", typeof(String));
                string[] strTypes = { "20MM PARCEL", "30MM PARCEL", "40MM PARCEL", "DOUBLE", "SINGLE", "PARCEL", "OTHER", "DOUBLE/SINGLE" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["PETITYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "UNITFORMALNAME")
            {
                dt.Columns.Add("UNITFORMALNAME", typeof(String));
                string[] strTypes = { "BAG-BAGS", "BAL-BALE", "BDL-BUNDLES", "BKL-BUCKLES", "BOU-BILLION OF UNITS", "BOX-BOX", "BTL-BOTTLES", "BUN-BUNCHES", "CAN-CANS", "CBM-CUBIC METERS", "CCM-CUBIC CENTIMETERS", "CMS-CENTIMETERS", "CTN-CARTONS", "DOZ-DOZENS", "DRM-DRUMS", "GGK-GREAT GROSS", "GMS-GRAMMES", "GRS-GROSS", "GYD-GROSS YARDS", "KGS-KILOGRAMS", "KLR-KILOLITRE", "KME-KILOMETRE", "MLT-MILILITRE", "MTR-METERS", "MTS-METRIC TON", "NOS-NUMBERS", "PAC-PACKS", "PCS-PIECES", "PRS-PAIRS", "QTL-QUINTAL", "ROL-ROLLS", "SET-SETS", "SQF-SQUARE FEET", "SQM-SQUARE METERS", "SQY-SQUARE YARDS", "TBS-TABLETS", "TGM-TEN GROSS", "THD-THOUSANDS", "TON-TONNES", "TUB-TUBES", "UGS-US GALLONS", "UNT-UNITS", "YDS-YARDS", "OTH-OTHERS" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["UNITFORMALNAME"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "CARDTYPE")
            {
                dt.Columns.Add("CARDTYPE", typeof(String));
                string[] strTypes = { "DEBIT CARD", "CREDIT CARD", "WALLET", "UPI" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["CARDTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "DEPARTMENTNAME")
            {
                dt.Columns.Add("DEPARTMENTNAME", typeof(String));
                string[] strTypes = { "MENS", "WOMENS", "KIDS", "ACCESSORIES", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["DEPARTMENTNAME"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "USERTYPE")
            {
                dt.Columns.Add("USERTYPE", typeof(String));
                string[] strTypes = { "ADMINISTRATOR", "NORMAL" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["USERTYPE"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "RELIGION")
            {
                dt.Columns.Add("RELIGION", typeof(String));
                string[] strTypes = { "HINDU", "MUSLIM", "CHRISTIAN", "SIKH", "BUDDHIST", "JAIN", "OTHER" };
                foreach (string strType in strTypes)
                {
                    DataRow row = dt.NewRow();
                    row["RELIGION"] = strType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "ORDERSTATUS")
            {
                dt.Columns.Add("ORDERSTATUS", typeof(String));
                string[] strStatus = { "PENDING", "HOLD" };
                foreach (string strMonth in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["ORDERSTATUS"] = strMonth;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "ALTSTATUS")
            {
                dt.Columns.Add("ALTSTATUS", typeof(String));
                string[] strStatus = { "CANCEL", "DELIVERED", "EXCHANGE", "HOLD", "PENDING", "READY" };
                foreach (string strMonth in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["ALTSTATUS"] = strMonth;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "ALTTYPE")
            {
                dt.Columns.Add("ALTTYPE", typeof(String));
                string[] strStatus = { "ALTERATION", "FINISHING", "READY" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["ALTTYPE"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "PURCHASESTATUS")
            {
                dt.Columns.Add("PURCHASESTATUS", typeof(String));
                string[] strStatus = { "PURCHASE IN", "STOCK IN", "HOLD" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["PURCHASESTATUS"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "DESIGNTYPE")
            {
                dt.Columns.Add("DESIGNTYPE", typeof(String));
                string[] strStatus = { "PURCHASE", "JOURNAL" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["DESIGNTYPE"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "MARGINTYPE")
            {
                dt.Columns.Add("MARGINTYPE", typeof(String));
                string[] strStatus = { "MARKUP", "MARKDOWN" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["MARGINTYPE"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "TRANSPORTMODE")
            {
                dt.Columns.Add("TRANSPORTMODE", typeof(String));
                string[] strStatus = { "AIR", "BY HAND", "COURIER", "ROAD", "RAILWAY" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["TRANSPORTMODE"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "ITEMSTATUS")
            {
                dt.Columns.Add("ITEMSTATUS", typeof(String));
                string[] strStatus = { "STOCK IN", "OUT OF STOCK" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["ITEMSTATUS"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "SOFTWARETYPE")
            {
                dt.Columns.Add("SOFTWARETYPE", typeof(String));
                string[] strStatus = { "TRADING", "RETAIL", "WHOLESALE", "MANUFACTURING" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["SOFTWARETYPE"] = strAltType;
                    dt.Rows.Add(row);
                }
            }
            else if (strName == "TEMPLATENAME")
            {
                dt.Columns.Add("TEMPLATENAME", typeof(String));
                string[] strStatus = { "CASH", "BANK", "JOURNAL", "ACCOUNTMASTER", "DESIGNMASTER" };
                foreach (string strAltType in strStatus)
                {
                    DataRow row = dt.NewRow();
                    row["TEMPLATENAME"] = strAltType;
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
    }
}
