using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SSS
{
    class ForwardingData
    {
        DataTable dtTransport,dtStation,dtMarketer,dtCartoneSize,dtCartoneType;
        SqlCommand cmd;

        public DataTable GetBalanceAmountMaster()
        {
            string strQuery = "";
            strQuery += " Select PartyName,(CASE When BalanceAmt>=0 then 'DEBIT' else 'CREDIT' end)Status, ABS(BalanceAmt) Amount from ( "
                     + " Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))PartyName,GroupName, "
                     + " (Select SUM(Amt) from (Select ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='DEBIT' and SM.GroupName not in ('SUB PARTY','DIRECT EXPENSE A/C','INDIRECT EXPENSE A/C','DIRECT INCOME A/C','INDIRECT INCOME A/C','PROFIT & LOSS A/C','Opening Stock','Closing Stock','Revenue From Operations','Revenue From Operations','Other Income','Employee Benefit Expense','DEPRECIATION','Selling & Distribution Expenses','Other Expenses','COST OF MATERIAL TRADED') and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')) Union All Select -ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='CREDIT' and SM.GroupName not in ('SUB PARTY','DIRECT EXPENSE A/C','INDIRECT EXPENSE A/C','DIRECT INCOME A/C','INDIRECT INCOME A/C','PROFIT & LOSS A/C','Opening Stock','Closing Stock','Revenue From Operations','Revenue From Operations','Other Income','Employee Benefit Expense','DEPRECIATION','Selling & Distribution Expenses','Other Expenses','COST OF MATERIAL TRADED') and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,''))) Balance) BalanceAmt "
                     + " from SupplierMaster SM Where SM.GroupName not in ('RESERVES & SURPLUSES')  Union All "
                     + " Select Top 1 (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))PartyName,GroupName, "
                     + " (Select SUM(Amt) from (Select ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount _BA Cross APPLY (SELECT GroupName from SupplierMaster _SM Where (_SM.AreaCode+_SM.AccountNo)=_BA.AccountID)_SM Where Status='DEBIT' and _SM.GroupName in ('RESERVES & SURPLUSES','DIRECT EXPENSE A/C','INDIRECT EXPENSE A/C','DIRECT INCOME A/C','INDIRECT INCOME A/C','PROFIT & LOSS A/C','Opening Stock','Closing Stock','Revenue From Operations','Revenue From Operations','Other Income','Employee Benefit Expense','DEPRECIATION','Selling & Distribution Expenses','Other Expenses','COST OF MATERIAL TRADED') Union All Select -ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount _BA Cross APPLY (SELECT GroupName from SupplierMaster _SM Where (_SM.AreaCode+_SM.AccountNo)=_BA.AccountID)_SM Where Status='CREDIT' and _SM.GroupName in ('RESERVES & SURPLUSES','DIRECT EXPENSE A/C','INDIRECT EXPENSE A/C','DIRECT INCOME A/C','INDIRECT INCOME A/C','PROFIT & LOSS A/C','Opening Stock','Closing Stock','Revenue From Operations','Revenue From Operations','Other Income','Employee Benefit Expense','DEPRECIATION','Selling & Distribution Expenses','Other Expenses','COST OF MATERIAL TRADED') Union All Select -ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0) Amt from BalanceAmount Where Status='DEBIT' and  AccountStatus in ('SALES A/C','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','SALE SERVICE','Credit Note','Debit Note','DUTIES & TAXES') Union All Select ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0) Amt from BalanceAmount Where Status='CREDIT' and  AccountStatus in('SALES A/C','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','SALE SERVICE','Credit Note','Debit Note','DUTIES & TAXES')) Balance) BalanceAmt "
                     + " from SupplierMaster SM Where SM.GroupName='RESERVES & SURPLUSES' ) Balance Order By PartyName";

            DataTable dt = DataBaseAccess.GetDataTableRecord(strQuery);
            return dt;
        }        

        public DataTable GetPartyData()
        {
            string strQuery = "Select * from SupplierMaster Order by Name ";
            DataTable dt = DataBaseAccess.GetDataTableRecord(strQuery);
            return dt;
        }

        public DataTable GetTransportMaster()
        {
            dtTransport = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select TransportName,ContactPersonI,ContactPersonII,PhoneNoI,PhoneNoII,MobileNo,City,Address,Date,[InsertStatus],[UpdateStatus],[GSTNo],[GreenTaxAmt],[CreatedBy],[UpdatedBy] from Transport Order by TransportName ", MainPage.con);
            adap.Fill(dtTransport);
             return dtTransport;
        }

        public DataTable GetStationMaster()
        {
            dtStation = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select StationName,Date,CreatedBy,UpdatedBy from Station Order by StationName", MainPage.con);
            adap.Fill(dtStation);    
            return dtStation;
        }

        public DataSet GetItemMaster()
        {
            DataSet _ds = new DataSet();
            SqlDataAdapter adap = new SqlDataAdapter(" Select * from Items Order By ItemName Select * from ItemSecondary Order By BillNo  Select * from ItemGroupMaster Order By GroupName ", MainPage.con);
            adap.Fill(_ds);
            return _ds;
        }

        public DataTable GetMarketerMaster()
        {
            dtMarketer = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select MarketerName,MobileNoI,MobileNoII,EmailID,Address,City,Date,OrderNoFrom,OrderNoTo from Marketer", MainPage.con);
            adap.Fill(dtMarketer);   
            return dtMarketer;
        }

        public DataTable GetCartoneSizeMaster()
        {
            dtCartoneSize = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select CSize,PackingAmt,Date from CartoneSize", MainPage.con);
            adap.Fill(dtCartoneSize);     
            return dtCartoneSize;
        }

        public DataTable GetCartoneTypeMaster()
        {
            dtCartoneType = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select Cartone,Date from CartoneType ", MainPage.con);
            adap.Fill(dtCartoneType);    
            return dtCartoneType;
        }

        public DataTable GetUserAccountMaster()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from UserAccount ", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetCategory()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from Category ", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetAdmin()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from Admin", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetCourierMaster()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from CourierMaster", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetCostMaster()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from CostMaster", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetAddressBook()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from AddressBook", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetGroupMaster()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from GroupMaster order by GroupName", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetOnAccountMaster()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from OnAccountParty order by PartyName", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetOnAccountPendingSalesRecord()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from OnAccountSalesRecord Where Status='PENDING'", MainPage.con);
            adap.Fill(dt);
            return dt;
        }

        public DataTable GetCompanySettingData()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from CompanySetting Where CompanyName='"+MainPage.strCompanyName+"' ", MainPage.con);
            adap.Fill(dt);
            return dt;
        } 
              
        public string GetNextYearDataBase()
        {
            string strName = "";
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select Next_Y_Path from Company Where CompanyName='"+MainPage.strCompanyName+"' ", MainPage.con);
            adap.Fill(dt);
            if(dt.Rows.Count>0)
            {
                string strpath = dt.Rows[0][0].ToString();
                if (strpath!="")
                {
                    int index = strpath.LastIndexOf(@"\");
                    strName = "A" +strpath.Substring(index + 1);

                }
            }
            return strName;
        }
       

        #region Forward Records        

        public int ForwardBalanceAmount(DataTable dt, string strDatabase)
        {
            int count = 0, rowCount=0,result=0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDatabase);
            if (strDatabase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if exists (Select AccountID from BalanceAmount Where  [AccountID]='" + row["PartyName"] + "' and AccountStatus='OPENING' ) begin "
                                  + " Update BalanceAmount set Status='" + row["Status"] + "',Amount='" + row["Amount"] + "',UpdateStatus=1 where [AccountID]='" + row["PartyName"] + "' and AccountStatus='OPENING'   end "
                                  + " else begin Insert into BalanceAmount ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) Values "
                                  + " ('" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["PartyName"] + "','OPENING','" + row["Status"] + "','FORWARDED','" + row["Amount"] + "','','0','0','False','',0,'" + MainPage.strLoginName + "','',0,1,0,'" + row["PartyName"] + "') end "
                                  + " Update SupplierMaster Set OpeningBal='" + row["Amount"] + "',Status='" + row["Status"] + "',UpdateStatus=1 Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))='" + row["PartyName"] + "' and GroupName!='SUB PARTY' ";

                    if (rowCount > 50)
                    {
                        strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;

                        // cmd = new SqlCommand(strQuery, MainPage.con);
                        // cmd.CommandTimeout = 1000000;
                        result += count = ExecuteQueryInNew(strQuery); //cmd.ExecuteNonQuery();
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;

                }
                if (strQuery != "")
                {
                    //cmd = new SqlCommand(strQuery, MainPage.con);
                    //count += cmd.ExecuteNonQuery();
                    result += count = ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        private int ExecuteQueryInNew(string strQuery)
        {
            int _count = 0;
            try
            {
                if (MainPage.con.State == ConnectionState.Closed)
                    MainPage.con.Open();
                strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;

                cmd = new SqlCommand(strQuery, MainPage.con);
                cmd.CommandTimeout = 1000000;
                _count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Sorry ! " + ex.Message);
                _count = -2;
            }
            return _count;
        }

        public int ForwardParty(DataTable dt, string strDatabase)
        {
            int count = 0,rowCount=0;
            string strQuery = "";           
            MainPage.ChangeDataBase(strDatabase);
            if (strDatabase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select AreaCode from SupplierMaster Where AreaCode='" + row["AreaCode"] + "' and AccountNo='" + row["AccountNo"] + "') begin "
                                 + " Insert into SupplierMaster([Name],[Category],[GroupName],[OpeningBal],[Status],[Address],[State],[PINCode],[Transport],[Station],[BookingStation],[TINNumber],[NormalDhara],[SNDhara],[ContactPerson],[PhoneNo],[MobileNo],[PvtMarka],[Reference],[EmailID],[DueDays],[Date],[CFormApply],[AmountLimit],[ExtendedAmt],[HasteSale],[PermanentAddress],[SecondTransport],[ThirdTransport],[FourthTransport],[Remark],[CDDays],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Postage],[TransactionLock],[BlackList],[BlackListReason],[GroupII],[Other],[AreaCode],[AccountNo],[CardNumber],[CardStatus],[SaleIncentive],[GSTNo],[PANNumber],[TaxType],[AccountantMobileNo],[MainPartyID],[CourierName],[DistrictName],[OrderAmtLimit],[Other1],[Other2],[Other3]) "
                                 + " Values('" + row["Name"] + "','" + row["Category"] + "','" + row["GroupName"] + "','0','DEBIT','" + row["Address"] + "','" + row["State"] + "','" + row["PINCode"] + "','" + row["Transport"] + "','" + row["Station"] + "','" + row["BookingStation"] + "','" + row["TINNumber"] + "','" + row["NormalDhara"] + "','" + row["SNDhara"] + "','" + row["ContactPerson"] + "', '" + row["PhoneNo"] + "','" + row["MobileNo"] + "','" + row["PvtMarka"] + "','" + row["Reference"] + "','" + row["EmailID"] + "','" + row["DueDays"] + "','" + row["Date"] + "',"
                                 + " '" + row["CFormApply"] + "','" + row["AmountLimit"] + "','" + row["ExtendedAmt"] + "','" + row["HasteSale"] + "','" + row["PermanentAddress"] + "','" + row["SecondTransport"] + "','" + row["ThirdTransport"] + "','" + row["FourthTransport"] + "','" + row["Remark"] + "','" + row["CDDays"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0,'" + row["Postage"] + "','" + row["TransactionLock"] + "','" + row["BlackList"] + "','" + row["BlackListReason"] + "','" + row["GroupII"] + "','" + row["Other"] + "','" + row["AreaCode"] + "','" + row["AccountNo"] + "','" + row["CardNumber"] + "','" + row["CardStatus"] + "','" + row["SaleIncentive"] + "','" + row["GSTNo"] + "','" + row["PANNumber"] + "','" + row["TaxType"] + "','" + row["AccountantMobileNo"] + "','" + row["MainPartyID"] + "','" + row["CourierName"] + "','" + row["DistrictName"] + "', "+ConvertObjectToDouble(row["OrderAmtLimit"])+",'" + row["Other1"] + "','" + row["Other2"] + "','" + row["Other3"] + "') end ";

                    if (rowCount > 50)
                    {
                        count = ExecuteQueryInNew(strQuery);
                        if (count!=-2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }

                if (strQuery != "")
                {                   
                    count = ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardPartyDetails(DataTable dt, string strDatabase)
        {
            int count = 0, rowCount = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDatabase);
            if (strDatabase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select [AreaCode] from [dbo].[SupplierOtherDetails] Where [AreaCode]='" + row["AreaCode"] + "' and [AccountNo]='" + row["AccountNo"] + "') begin INSERT INTO [dbo].[SupplierOtherDetails] ([AreaCode],[AccountNo],[WaybillUserName],[WaybillPassword],[CompanyRegNo],[NameOfFirm],[OtherDetails],[NB_Manufacturing],[NB_SoleSellingAgent],[NB_Dealer],[NB_Agent],[NB_Assembler],[NB_Trader],[NC_Proprietary],[NC_Partnership],[NC_Private],[NC_Public],[Other],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[DOB],[DOA],[SpouseName],[Description]) VALUES "
                                 + " ('" + row["AreaCode"] + "','" + row["AccountNo"] + "','" + row["WaybillUserName"] + "','" + row["WaybillPassword"] + "','" + row["CompanyRegNo"] + "','" + row["NameOfFirm"] + "','" + row["OtherDetails"] + "','" + row["NB_Manufacturing"] + "','" + row["NB_SoleSellingAgent"] + "','" + row["NB_Dealer"] + "','" + row["NB_Agent"] + "','" + row["NB_Assembler"] + "','" + row["NB_Trader"] + "','" + row["NC_Proprietary"] + "','" + row["NC_Partnership"] + "','" + row["NC_Private"] + "','" + row["NC_Public"] + "','" + row["Other"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0,'" + row["DOB"] + "','" + row["DOA"] + "','" + row["SpouseName"] + "','" + row["Description"] + "') end ";

                    if (rowCount == 50)
                    {
                        count = ExecuteQueryInNew(strQuery);
                        if (count !=-2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }

                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardPartyBankDetails(DataTable dt, string strDatabase)
        {
            int count = 0, rowCount = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDatabase);
            if (strDatabase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select AreaCode from [SupplierBankDetails] Where [AreaCode]='" + row["AreaCode"] + "' and [AccountNo]='" + row["AccountNo"] + "' and [BankAccountNo]='" + row["BankAccountNo"] + "') begin "
                       + " INSERT INTO [dbo].[SupplierBankDetails] ([AreaCode],[AccountNo],[BankName],[BranchName],[BankAccountNo],[BankIFSCCode],[BankAccountName],[VerifiedStatus],[VerifiedDate],[CreatedBy],[BeniID],[InsertStatus],[UpdateStatus]) VALUES "
                      + " ('" + row["AreaCode"] + "','" + row["AccountNo"] + "','" + row["BankName"] + "','" + row["BranchName"] + "','" + row["BankAccountNo"] + "','" + row["BankIFSCCode"] + "','" + row["BankAccountName"] + "','" + row["VerifiedStatus"] + "','" + row["VerifiedDate"] + "','" + row["CreatedBy"] + "','" + row["BeniID"] + "',0,0) end ";

                    if (rowCount == 50)
                    {
                        count = ExecuteQueryInNew(strQuery);
                        if (count !=-2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }

                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardPartyBrandDetails(DataTable dt, string strDatabase)
        {
            int count = 0, rowCount = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDatabase);
            if (strDatabase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select AreaCode from [SupplierBrandDetails] Where [AreaCode]='" + row["AreaCode"] + "' and [AccountNo]='" + row["AccountNo"] + "' and [Range]='" + row["Range"] + "' and [BrandName]='" + row["BrandName"] + "' and [ProductType]='" + row["ProductType"] + "') begin "
                             + " INSERT INTO [dbo].[SupplierBrandDetails] ([AreaCode],[AccountNo],[BrandName],[ProductType],[Range],[HSNCode],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                             + " ('" + row["AreaCode"] + "','" + row["AccountNo"] + "','" + row["BrandName"] + "','" + row["ProductType"] + "','" + row["Range"] + "','" + row["HSNCode"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0) end ";

                    if (rowCount == 50)
                    {
                        count= ExecuteQueryInNew(strQuery);
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }

                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardTransport(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
           
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from Transport Where TransportName='" + row["TransportName"] + "') begin "
                                   + " Insert into Transport ([TransportName],[ContactPersonI],[ContactPersonII],[PhoneNoI],[PhoneNoII],[MobileNo],[City],[Address],[Date],[InsertStatus],[UpdateStatus],[GSTNo],[GreenTaxAmt],[CreatedBy],[UpdatedBy]) Values "
                                   + " ('" + row["TransportName"] + "','" + row["ContactPersonI"] + "','" + row["ContactPersonII"] + "','" + row["PhoneNoI"] + "','" + row["PhoneNoII"] + "','" + row["MobileNo"] + "','" + row["City"] + "','" + row["Address"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0,'" + row["GSTNo"] + "'," + DataBaseAccess.ConvertObjectToDoubleStatic(row["GreenTaxAmt"]) + ",'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "') end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardMarketer(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";          
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from Marketer Where MarketerName='" + row["MarketerName"] + "' ) begin "
                                   + " Insert into Marketer ([MarketerName],[MobileNoI],[MobileNoII],[EmailID],[Address],[City],[Date],[OrderNoFrom],[OrderNoTo],[InsertStatus],[UpdateStatus]) Values "
                                   + " ('" + row["MarketerName"] + "','" + row["MobileNoI"] + "','" + row["MobileNoII"] + "','" + row["EmailID"] + "','" + row["Address"] + "','" + row["City"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["OrderNoFrom"] + "','" + row["OrderNoTo"] + "',1,0) end ";

                }
                if (strQuery != "")
                {
                    count = ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardStation(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";        
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from Station Where StationName='" + row["StationName"] + "') begin "
                              + " Insert into Station ([StationName],[Date],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy]) Values('" + row["StationName"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0,'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "')  end ";

                }
                if (strQuery != "")
                {
                    count = ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }
        public double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToDouble(objValue);

            }
            catch (Exception ex)
            {
            }
            return dValue;
        }

        public int ForwardPendingOrder(DataTable dt, string strDataBase)
        {
            int count = 0, rowCount=0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select OrderCode from OrderBooking Where OrderCode='" + row["OrderCode"] + "O' and OrderNo=" + row["OrderNo"] + " and NumberCode='" + row["NumberCode"] + "') begin "
                                + " Insert into OrderBooking ([BookingNo],[Marketer],[OrderNo],[P_Party],[S_Party],[Station],[Items],[Pieces],[Quantity],[Amount],[Transport],[Booking],[Marka],[Haste],[Date],[Personal],[Status],[OrderCode],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[NumberCode],[SerialNo],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory],[Imagepath1],[Imagepath2],[Imagepath3],[Imagepath4],[Imagepath5]) Values "
                                + " ('" + row["BookingNo"] + "','" + row["Marketer"] + "','" + row["OrderNo"] + "','" + row["P_Party"] + "','" + row["S_Party"] + "','" + row["Station"] + "','" + row["Items"] + "','" + row["Pieces"] + "','" + row["Quantity"] + "'," + ConvertObjectToDouble(row["Amount"]) + ",'" + row["Transport"] + "','" + row["Booking"] + "','" + row["Marka"] + "','" + row["Haste"] + "','" + row["Date"] + "','" + row["Personal"] + "','" + row["Status"] + "','" + row["OrderCode"] + "O','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',1,0,'" + row["NumberCode"] + "','" + row["SerialNo"] + "','" + row["SalePartyID"] + "','" + row["SubPartyID"] + "','" + row["PurchasePartyID"] + "','" + row["SchemeName"] + "','" + row["OfferName"] + "',N'" + row["Remark"] + "'," + ConvertObjectToDouble(row["AdjustedQty"]) + "," + ConvertObjectToDouble(row["CancelQty"]) + ",N'" + row["MRemark"] + "','" + row["DeliveryDate"] + "','" + row["Variant1"] + "','" + row["Variant2"] + "','" + row["Variant3"] + "','" + row["Variant4"] + "','" + row["Variant5"] + "','" + row["OrderType"] + "','" + row["OrderCategory"] + "','" + row["Imagepath1"] + "','" + row["Imagepath2"] + "','" + row["Imagepath3"] + "','" + row["Imagepath4"] + "','" + row["Imagepath5"] + "') end ";

                    if (rowCount == 100)
                    {
                        count = ExecuteQueryInNew(strQuery);
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }

                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }


        public int ForwardItems(DataSet _ds, string strDataBase)
        {
            int count = 0;
            try {

                string strQuery = "";
                MainPage.ChangeDataBase(strDataBase);
                if (strDataBase == MainPage.con.Database)
                {
                    DataTable dt = _ds.Tables[0];
                    int rowCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        strQuery += " if not exists(Select ItemName from Items Where ItemName='" + row["ItemName"] + "') begin "
                                      + " INSERT INTO [dbo].[Items] ([ItemName],[Date],[InsertStatus],[UpdateStatus],[GroupName],[SubGroupName],[UnitName],[BillCode],[BillNo],[BuyerDesignName],[QtyRatio],[StockUnitName],[DisStatus],[DisRemark],[Other],[CreatedBy],[UpdatedBy],[BrandName],[MakeName],[BarcodingType]) VALUES "
                                      + " ('" + row["ItemName"] + "','" + row["Date"] + "',0,0,'" + row["GroupName"] + "','" + row["SubGroupName"] + "','" + row["UnitName"] + "','" + row["BillCode"] + "','" + row["BillNo"] + "','" + row["BuyerDesignName"] + "','" + row["QtyRatio"] + "','" + row["StockUnitName"] + "','" + row["DisStatus"] + "','" + row["DisRemark"] + "','" + row["Other"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["BrandName"] + "','" + row["MakeName"] + "','" + row["BarcodingType"] + "')  end ";

                        if (rowCount == 100)
                        {
                            count = ExecuteQueryInNew(strQuery);
                            if (count != -2)
                            {
                                strQuery = "";
                                rowCount = 0;
                            }
                        }
                        rowCount++;
                    }
                    if (strQuery != "")
                    {
                        count = ExecuteQueryInNew(strQuery);
                        strQuery = "";
                    }

                    dt = _ds.Tables[1];
                    rowCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        strQuery += " if not exists (Select * from [ItemSecondary] Where BillCode='" + row["BillCode"] + "' and BillNo=" + row["BillNo"] + " and RemoteID=" + row["ID"] + ") begin "
                                 + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleMRP],[DesignName],[Brand]) Values "
                                 + " ('" + row["ID"] + "','" + row["BillCode"] + "','" + row["BillNo"] + "','" + row["PurchasePartyID"] + "','" + row["Variant1"] + "','" + row["Variant2"] + "','" + row["Variant3"] + "','" + row["Variant4"] + "','" + row["Variant5"] + "'," + row["PurchaseRate"] + "," + row["Margin"] + "," + row["SaleRate"] + ",'" + row["Reorder"] + "','" + row["OpeningQty"] + "','" + row["OpeningRate"] + "','" + row["ActiveStatus"] + "','" + row["GodownName"] + "','" + row["Description"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["InsertStatus"] + "','" + row["UpdateStatus"] + "',"+DataBaseAccess.ConvertObjectToDouble(row["SaleMRP"],2)+",'" + row["DesignName"] + "','" + row["Brand"] + "') end ";

                        if (rowCount == 100)
                        {
                            count = ExecuteQueryInNew(strQuery);
                            if (count != -2)
                            {
                                strQuery = "";
                                rowCount = 0;
                            }
                        }
                        rowCount++;
                    }


                    if (strQuery != "")
                    {
                        count = ExecuteQueryInNew(strQuery);
                        strQuery = "";
                    }

                    dt = _ds.Tables[2];
                    rowCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        strQuery += " if not exists(Select GroupName from ItemGroupMaster Where GroupName='" + row["GroupName"] + "' and HSNCode='" + row["HSNCode"] + "') begin "
                                 + "  INSERT INTO [dbo].[ItemGroupMaster] ([GroupName],[CategoryName],[ParentGroup],[HSNCode],[AmtRange],[TaxRate],[HSNType],[Other],[InsertStatus],[UpdateStatus],[TaxCategoryName]) Values "
                                 + " ('" + row["GroupName"] + "','" + row["CategoryName"] + "','" + row["ParentGroup"] + "','" + row["HSNCode"] + "'," + DataBaseAccess.ConvertObjectToDoubleStatic(row["AmtRange"]) + "," + DataBaseAccess.ConvertObjectToDoubleStatic(row["TaxRate"]) + ",'" + row["HSNType"] + "','" + row["Other"] + "',0,0 ,'" + row["TaxCategoryName"] + "') end ";

                        if (rowCount == 100)
                        {
                            count = ExecuteQueryInNew(strQuery);
                            if (count != -2)
                            {
                                strQuery = "";
                                rowCount = 0;
                            }
                        }
                        rowCount++;
                    }


                    if (strQuery != "")
                    {
                        count += ExecuteQueryInNew(strQuery);
                        strQuery = "";
                    }

                }
            }
            catch
            {
              
            }
            return count;
        }

        public int ForwardCartoneSize(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";           
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from CartoneSize Where CSize='" + row["CSize"] + "') begin "
                               + " Insert into CartoneSize ([CSize],[PackingAmt],[Date],[InsertStatus],[UpdateStatus]) Values('" + row["CSize"] + "','" + row["PackingAmt"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0) end ";

                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardCartoneType(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from CartoneType Where Cartone='" + row["Cartone"] + "') begin "
                               + " Insert into CartoneType ([Cartone],[Date],[InsertStatus],[UpdateStatus]) Values('" + row["Cartone"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0)  end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardUserAccount(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";        
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select * from UserAccount Where LoginName='" + row["LoginName"] + "') begin "
                                  + " Insert into UserAccount ([LoginName],[Password],[Name],[MobileNo],[UserType],[JournalEntry],[JournalView],[JournalEdit],[CashEntry],[CashView],[CashEdit],[OrderEntry],[OrderView],[OrderEdit],[GoodsEntry],[GoodsView],[GoodsEdit],[SaleEntry],[SaleView],[SaleEdit],[PurchaseEntry],[PurchaseView],[PurchaseEdit],[ForwardingEntry],[ForwardingView],[ForwardingEdit],[CourierEntry],[CourierView],[CourierEdit],[NewParty],[PartyView],[NewPartyEdit],[NewSubParty],[SubPartyView],[SubPartyEdit],"
                                  + " [NewAccountMaster],[AccountMasterView],[AccountMasterEdit],[Merging],[CompanyInfo],[Accessories],[BackupRestore],[OrderSlipView],[FASReport],[GoodsRecivedView],[Reportview],[SalesReportView],[ReportSummeryView],[PurchaseReport],[MultiCmpReportview],[ForwardingReport],[PartyLedger],[Date],[LedgerInterest],[PrintMultiLedger],[PurchaseOutStanding],[CrediterDebter],[ShowAmountLimit],[BackDateEntry],[SMS],[Other],[Reminder],[DayBook],[OnAccount],[Extra],BranchCode,GSTMasterEntry,GSTMasterView,GSTMasterEditDelete,RefrenceMasterEntry,RefrenceMasterView,RefrenceMasterEditDelete,LockunLockCustomer,SecurityChequePermission,AdminPanel,ChangeSupplierDisc,ChangeCustomerLimit,Dashboard,BankDetailApprove,PartyWiseSP,ChangeBankDetail,BranchWiseSP,ChangeCustomerDetail,ShowBankLedger,PartyMasterRegister,GraphicalSummary,SchemeMaster,ShowPartyLimit,ShowAllRecord,GSTReport,ShowEmailReg,showWhatsAppReg,Address,AddNewCustomer,[InsertStatus],[UpdateStatus]) Values "
                                  + " ('" + row["LoginName"] + "','" + row["Password"] + "','" + row["Name"] + "','" + row["MobileNo"] + "','" + row["UserType"] + "','" + row["JournalEntry"] + "','" + row["JournalView"] + "','" + row["JournalEdit"] + "','" + row["CashEntry"] + "','" + row["CashView"] + "','" + row["CashEdit"] + "','" + row["OrderEntry"] + "','" + row["OrderView"] + "','" + row["OrderEdit"] + "','" + row["GoodsEntry"] + "','" + row["GoodsView"] + "','" + row["GoodsEdit"] + "','" + row["SaleEntry"] + "','" + row["SaleView"] + "','" + row["SaleEdit"] + "','" + row["PurchaseEntry"] + "','" + row["PurchaseView"] + "','" + row["PurchaseEdit"] + "','" + row["ForwardingEntry"] + "','" + row["ForwardingView"] + "','" + row["ForwardingEdit"] + "','" + row["CourierEntry"] + "','" + row["CourierView"] + "',"
                                  + " '" + row["CourierEdit"] + "','" + row["NewParty"] + "','" + row["PartyView"] + "','" + row["NewPartyEdit"] + "','" + row["NewSubParty"] + "','" + row["SubPartyView"] + "','" + row["SubPartyEdit"] + "','" + row["NewAccountMaster"] + "','" + row["AccountMasterView"] + "','" + row["AccountMasterEdit"] + "','" + row["Merging"] + "','" + row["CompanyInfo"] + "','" + row["Accessories"] + "', '" + row["BackupRestore"] + "','" + row["OrderSlipView"] + "','" + row["FASReport"] + "','" + row["GoodsRecivedView"] + "','" + row["Reportview"] + "' ,'" + row["SalesReportView"] + "','" + row["ReportSummeryView"] + "','" + row["PurchaseReport"] + "','" + row["MultiCmpReportview"] + "','" + row["ForwardingReport"] + "','" + row["PartyLedger"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["LedgerInterest"] + "','" + row["PrintMultiLedger"] + "','" + row["PurchaseOutStanding"] + "','" + row["CrediterDebter"] + "','" + row["ShowAmountLimit"] + "','" + row["BackDateEntry"] + "','" + row["SMS"] + "','" + row["Other"] + "','" + row["Reminder"] + "','" + row["DayBook"] + "','" + row["OnAccount"] + "','" + row["Extra"] + "',"
                                  + " '" + row["BranchCode"] + "','" + row["GSTMasterEntry"] + "','" + row["GSTMasterView"] + "','" + row["GSTMasterEditDelete"] + "','" + row["RefrenceMasterEntry"] + "','" + row["RefrenceMasterView"] + "','" + row["RefrenceMasterEditDelete"] + "','" + row["LockunLockCustomer"] + "','" + row["SecurityChequePermission"] + "','" + row["AdminPanel"] + "','" + row["ChangeSupplierDisc"] + "','" + row["ChangeCustomerLimit"] + "','" + row["Dashboard"] + "','" + row["BankDetailApprove"] + "','" + row["PartyWiseSP"] + "','" + row["ChangeBankDetail"] + "','" + row["BranchWiseSP"] + "','" + row["ChangeCustomerDetail"] + "','" + row["ShowBankLedger"] + "','" + row["PartyMasterRegister"] + "','" + row["GraphicalSummary"] + "','" + row["SchemeMaster"] + "','" + row["ShowPartyLimit"] + "','" + row["ShowAllRecord"] + "','" + row["GSTReport"] + "','" + row["ShowEmailReg"] + "','" + row["showWhatsAppReg"] + "','" + row["Address"] + "','" + row["AddNewCustomer"] + "',1,0)  end ";

                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardCategory(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from Category Where CategoryName='" + row["CategoryName"] + "') begin "
                                   + " Insert into Category ([CategoryName],[GroupName],[DepreciationPer],[DiscountDr],[DiscountCr],[InsertStatus],[UpdateStatus]) Values ('" + row["CategoryName"] + "','" + row["GroupName"] + "'," + DataBaseAccess.ConvertObjectToDoubleStatic(row["DepreciationPer"]) + ",'" + row["DiscountDr"] + "','" + row["DiscountCr"] + "',1,0)  end ";

                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardAdmin(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " UPDATE [dbo].[Admin] SET [Password] ='"+row["Password"]+"' ,[Status] ='PAID'  ";
                    break;
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }         
        public int ForwardCompanyDetails(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach(DataRow row in dt.Rows)
                {
                    strQuery += " If not Exists(Select CompanyName from CompanyDetails where CompanyName='" + row["CompanyName"] + "')"
                             + " Begin Insert into CompanyDetails values ('" + row["CompanyName"] + "','" + row["FullCompanyName"] + "','" + row["Address"] + "','" + row["StateName"] + "',"
                             + " '" + row["PinCode"] + "','" + row["GSTNo"] + "','" + row["PANNo"] + "','" + row["TINNo"] + "','" + row["EmailId"] + "'," + row["STDNo"] + ",'" + row["PhoneNo"] + "',"
                             + " '" + row["MobileNo"] + "','" + row["SignaturePath"] + "','" + row["Other"] + "','" + row["TAXEnabled"] + "','" + row["CreatedBy"] + "','" + row["UpdateBy"] + "','" + row["InsertStatus"] + "',"
                             + " '" + row["UpdateStatus"] + "','" + row["BankName"] + "','" + row["AccountName"] + "','" + row["IFSCCode"] + "','" + row["BranchName"] + "','" + row["CINNumber"] + "','" + row["SACCode"] + "','" + row["HeaderImage"] + "','" + row["BrandLogo"] + "','" + row["SignatureImage"] + "','" + row["WebSite"] + "') end";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }
        public int ForwardBrandMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " If not Exists(Select BrandName from BrandMaster where BrandName='" + row["BrandName"] + "')"
                             + " Begin Insert into BrandMaster values ('" + row["BrandName"] + "','" + row["PurchasePartyID"] + "'," + row["MinStock"] + "," + row["MaxStock"] + ","
                             + " '" + row["Date"] + "','" + row["Remark"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["InsertStatus"] + "','" + row["UpdateStatus"] + "', " + NetDBAccess.ConvertObjectToDouble(row["Margin"]) + ") end";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }
        public int ForwardBarCodeSetting(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strQuery += " If not Exists (select * from BarCodeSetting) begin Insert into BarCodeSetting Values ('" + row["RatePrefix"] + "','" + row["SupplierCode"] + "','" + row["PurchaseDate"] + "','" + row["MRP"] + "','" + row["Rate"] + "','" + row["Barcode"] + "','" + row["Brand"] + "','" + row["DesignName"] + "','" + row["Size"] + "','" + row["Color"] + "','" + row["Qty"] + "','" + row["PurchaseCity"] + "','" + row["PurchaseRate"] + "','" + row["Remark"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["InsertStatus"] + "','" + row["UpdateStatus"] + "' ) end";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }
        public int ForwardPinCodeDistance(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        strQuery += " If not Exists(Select * from PinCodeDistance where FromPinCode='" + row["FromPinCode"] + "' and ToPinCode='" + row["ToPinCode"] + "')"
                                 + " Begin Insert into PinCodeDistance values ('" + row["FromPinCode"] + "','" + row["ToPinCode"] + "','" + row["Distance"] + "','" + row["Date"] + "') end";
                    }
                    if (strQuery != "")
                    {
                        count += ExecuteQueryInNew(strQuery);
                    }
                }
            }
            return count;
        }
        public int ForwardPrintingConfig(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strQuery += " If not Exists (select * from PrintingConfig) Begin"
                             + " Insert Into PrintingConfig Values ('" + row["TitleOfDocument"] + "','" + row["SubTitle"] + "','" + row["Jurisdiction"] + "','" + row["GeneratedBy"] + "','" + row["Declaration"] + "','" + row["CompanyName"] + "','" + row["CompanyAddress"] + "','" + row["BuyerName"] + "','" + row["BuyerAddress"] + "','" + row["CompTaxRegNo"] + "','" + row["BuyerTaxRegNo"] + "','" + row["OrderDetails"] + "','" + row["SupplierDesign"] + "',"
                             + " '" + row["ManfDesign"] + "','" + row["Qty"] + "','" + row["Rate"] + "','" + row["Amount"] + "','" + row["Category1"] + "','" + row["Category2"] + "','" + row["Category3"] + "','" + row["Category4"] + "','" + row["Category5"] + "','" + row["Other"] + "','" + row["OtherBit"] + "','" + row["AgentName"] + "','" + row["TermsOfDelivery"] + "','" + row["NoOfCopy"] + "') end";

                    if (strQuery != "")
                    {
                        count += ExecuteQueryInNew(strQuery);
                    }
                }
            }
            return count;
        }
        public int ForwardProfitMargin(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " If not Exists(Select CompanyName from ProfitMargin where CompanyName='" + row["CompanyName"] + "')"
                             + " Begin Insert into ProfitMargin values ('" + row["CompanyName"] + "','" + row["FixedProfit"] + "'," + row["FixedProfitRate"] + ",'" + row["PurchaseBill"] + "',"
                             + " " + row["PurchaseBillRate"] + ",'" + row["ItemWise"] + "'," + row["ItemWiseRate"] + ",'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["InsertStatus"] + "','" + row["UpdateStatus"] + "','" + row["BrandWise"] + "'," + row["BrandWiseRate"] + ") end";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardSMSMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " If not Exists(Select * from MessageMaster)"
                             + " Begin INSERT INTO [dbo].[MessageMaster]([URL],[SenderId],[UserName],[Password],[MessageType]) Values "
                             + " ('" + row["URL"] + "','" + row["SenderId"] + "','" + row["UserName"] + "','" + row["Password"] + "','" + row["MessageType"] + "') end";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardCourierMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";         
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from CourierMaster Where CourierName='" + row["CourierName"] + "') begin "
                                      + " Insert into CourierMaster ([CourierName],[MobileNo],[Address],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) Values('" + row["CourierName"] + "','" + row["MobileNo"] + "','" + row["Address"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',1,0) end ";

                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardCostMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";      
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from CostMaster Where CostType='" + row["CostType"] + "') begin "
                                          + " Insert into CostMaster ([CostType],[Date],[InsertStatus],[UpdateStatus]) Values('" + row["CostType"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0) end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }


        public int ForwardAddressBook(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";           
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select [Name] from AddressBook Where [AreaCode]='" + row["AreaCode"] + "' and [AccountNo]='" + row["AccountNo"] + "') begin "
                            + " INSERT INTO [dbo].[AddressBook] ([Name],[NickName],[GroupName],[MobileNo],[PhoneNoCode],[PhoneNo],[Address],[PinCode],[City],[State],[InsertStatus],[UpdateStatus],[AreaCode],[AccountNo],[WhatsappNo],[EmailID],[VisitedBy],[VisitedDate],[GSTNo],[Reference],[Remark],[CreatedBy],[UpdatedBy])VALUES "
                            + " ('" + row["Name"] + "','" + row["NickName"] + "','" + row["GroupName"] + "','" + row["MobileNo"] + "','" + row["PhoneNoCode"] + "','" + row["PhoneNo"] + "','" + row["Address"] + "','" + row["PinCode"] + "','" + row["City"] + "','" + row["State"] + "',0,0,'" + row["AreaCode"] + "','" + row["AccountNo"] + "','" + row["WhatsappNo"] + "','" + row["EmailID"] + "','" + row["VisitedBy"] + "','" + row["VisitedDate"] + "','" + row["GSTNo"] + "','" + row["Reference"] + "','" + row["Remark"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "') end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardGroupMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";           
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select * from GroupMaster Where GroupName='" + row["GroupName"] + "') begin "
                                  + "  Insert into GroupMaster ([GroupName],[Date],[InsertStatus],[UpdateStatus]) values('" + row["GroupName"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "',1,0) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardOnAccount(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select * from OnAccountParty Where PartyName='" + row["PartyName"] + "' and OnAccountName='" + row["OnAccountName"] + "' and SubPartyName='" + row["SubPartyName"] + "' ) begin "
                                  + "  Insert into OnAccountParty ([GroupName],[PartyName],[SubPartyName],[OnAccountName],[TINNo],[Address],[PinCode],[Station],[State],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) Values "
                                  + " ('" + row["GroupName"] + "','" + row["PartyName"] + "','" + row["SubPartyName"] + "','" + row["OnAccountName"] + "','" + row["TINNo"] + "','" + row["Address"] + "','" + row["PinCode"] + "','" + row["Station"] + "','" + row["State"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',1,0) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardOnAccountPendingSales(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (Select * from OnAccountSalesRecord Where SaleBillCode='" + row["SaleBillCode"] + "' and BillNo='" + row["BillNo"] + "') begin "
                                  + "  Insert into OnAccountSalesRecord ([BillNo],[BillDate],[SaleBillCode],[SaleBillNo],[SalesParty],[SubParty],[OnaccountName],[RoadPermitNo],[Transport],[Station],[TINNo],[LrNo],[LrDate],[Date],[BookNo],[PBillNo],[PurchaseParty],[Item],[Qty],[Amount],[TaxPer],[Tax],[OtherAmt],[FinalAmt],[Cash],[FormRequired],[FormType],[CreatedBy],[UpdatedBy],[ReceiveDate],[ReceiveAmount],[ChqDate],[ChqNo],[ChqAmt],[BankName],[Status],[CFormStatus],[InsertStatus],[UpdateStatus]) Values "
                                  + " ('" + row["BillNo"] + "','" + row["BillDate"] + "','" + row["SaleBillCode"] + "','" + row["SaleBillNo"] + "','" + row["SalesParty"] + "','" + row["SubParty"] + "','" + row["OnaccountName"] + "','" + row["RoadPermitNo"] + "','" + row["Transport"] + "','" + row["Station"] + "','" + row["TINNo"] + "','" + row["LrNo"] + "','" + row["LrDate"] + "','" + row["Date"] + "','" + row["BookNo"] + "','" + row["PBillNo"] + "','" + row["PurchaseParty"] + "','" + row["Item"] + "','" + row["Qty"] + "',"
                                  + " '" + row["Amount"] + "','" + row["TaxPer"] + "','" + row["Tax"] + "','" + row["OtherAmt"] + "','" + row["FinalAmt"] + "','" + row["Cash"] + "','" + row["FormRequired"] + "','" + row["FormType"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["ReceiveDate"] + "','" + row["ReceiveAmount"] + "','" + row["ChqDate"] + "','" + row["ChqNo"] + "','" + row["ChqAmt"] + "','" + row["BankName"] + "','" + row["Status"] + "','" + row["CFormStatus"] + "',1,0) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardCompanySettingData(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                strQuery="Declare @ComName varchar(500); Select Top 1 @ComName=CompanyName from Company ";
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select CompanyName from CompanySetting Where CompanyName=@ComName) begin "
                                  + " INSERT INTO [dbo].[CompanySetting] ([CompanyName],[StandardLogin],[ShowAccountCode],[PwdLimit],[MobileNo],[EmailID],[DaysInYear],[GraceDays],[CashDiscDays],[CashDiscRate],[DrInterest],[CrInterest],[FreightDhara],[TaxDhara],[Postage],[Packing],[Vat],[Rebate],[Date],[CreatedBy],[UpdatedBy],[UpdateStatus],[Password],[OtherCode],[SMTPServer],[SMTPPort],[HTTPPath],[FTPPath],[FTPUserName],[FTPPassword]) VALUES "
                                  + " (@ComName,'" + row["StandardLogin"] + "','" + row["ShowAccountCode"] + "','" + row["PwdLimit"] + "','" + row["MobileNo"] + "','" + row["EmailID"] + "','" + row["DaysInYear"] + "','" + row["GraceDays"] + "','" + row["CashDiscDays"] + "','" + row["CashDiscRate"] + "','" + row["DrInterest"] + "','" + row["CrInterest"] + "','" + row["FreightDhara"] + "','" + row["TaxDhara"] + "','" + row["Postage"] + "','" + row["Packing"] + "','" + row["Vat"] + "','" + row["Rebate"] + "','" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["UpdateStatus"] + "','" + row["Password"] + "','" + row["OtherCode"] + "','" + row["SMTPServer"] + "','" + row["SMTPPort"] + "','" + row["HTTPPath"] + "','" + row["FTPPath"] + "','" + row["FTPUserName"] + "','" + row["FTPPassword"] + "') end ";
                    break;
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardSaleTypeMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from SaleTypeMaster Where [TaxName]='" + row["TaxName"] + "' and [SaleType]='" + row["SaleType"] + "') begin "
                                 + " INSERT INTO [dbo].[SaleTypeMaster] ([TaxName],[Region],[TaxationType],[EcommType],[TaxOnMRP],[TaxIncluded],[EcommPortalName],[TaxAccountIGST],[TaxAccountSGST],[Other],[SkipGST],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy],[SaleType],[IGSTTaxRate],[SGSTTaxRate])  VALUES "
                                + " ('" + row["TaxName"] + "','" + row["Region"] + "','" + row["TaxationType"] + "','" + row["EcommType"] + "','" + row["TaxOnMRP"] + "','" + row["TaxIncluded"] + "','" + row["EcommPortalName"] + "','" + row["TaxAccountIGST"] + "','" + row["TaxAccountSGST"] + "','" + row["Other"] + "','" + row["SkipGST"] + "',0,0,'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','" + row["SaleType"] + "','" + row["IGSTTaxRate"] + "','" + row["SGSTTaxRate"] + "') end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardTaxCategoryMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from [dbo].[TaxCategory] Where [CategoryName]='" + row["CategoryName"] + "' ) begin "
                                 + " INSERT INTO [dbo].[TaxCategory] ([CategoryName],[TaxType],[TaxRateIGST],[TaxRateCGST],[TaxRateSGST],[TaxOnMRP],[CalculateTaxON],[TaxInclPrice],[ChangeTaxRate],[AmountType],[GreaterORSmaller],[ChangeAmt],[TaxChangeRateIGST],[TaxChangeRateCGST],[TaxChangeRateSGST],[Other],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                + " ('" + row["CategoryName"] + "','" + row["TaxType"] + "','" + row["TaxRateIGST"] + "','" + row["TaxRateCGST"] + "','" + row["TaxRateSGST"] + "','" + row["TaxOnMRP"] + "','" + row["CalculateTaxON"] + "','" + row["TaxInclPrice"] + "','" + row["ChangeTaxRate"] + "','" + row["AmountType"] + "','" + row["GreaterORSmaller"] + "','" + row["ChangeAmt"] + "','" + row["TaxChangeRateIGST"] + "','" + row["TaxChangeRateCGST"] + "','" + row["TaxChangeRateSGST"] + "','" + row["Other"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0) end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }


        public int ForwardUnitMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from UnitMaster Where UnitName='" + row["UnitName"] + "' ) begin "
                               + " INSERT INTO [dbo].[UnitMaster] ([UnitName],[FormalName],[DecimalPoint],[InsertStatus],[UpdateStatus]) VALUES "
                               + " ('" + row["UnitName"] + "','" + row["FormalName"] + "','" + row["DecimalPoint"] + "',1,0 ) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardVariantMaster1(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from VariantMaster1 Where Variant1='" + row["Variant1"] + "' ) begin "
                               + " INSERT INTO [dbo].[VariantMaster1]([Variant1],[Remark],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                               + " ('" + row["Variant1"] + "','" + row["Remark"] + "','" + row["Date"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0 ) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardVariantMaster2(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from VariantMaster2 Where Variant2='" + row["Variant2"] + "' ) begin "
                               + " INSERT INTO [dbo].[VariantMaster2]([Variant2],[Remark],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                               + " ('" + row["Variant2"] + "','" + row["Remark"] + "','" + row["Date"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0 ) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardVariantMaster3(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from VariantMaster3 Where Variant3='" + row["Variant3"] + "' ) begin "
                               + " INSERT INTO [dbo].[VariantMaster3]([Variant3],[Remark],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                               + " ('" + row["Variant3"] + "','" + row["Remark"] + "','" + row["Date"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0 ) end ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardItemCategoryMaster(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists(Select * from [dbo].[ItemCategoryMaster] Where [CategoryName]='" + row["CategoryName"] + "' ) begin "
                                 + " INSERT INTO [dbo].[ItemCategoryMaster] ([CategoryName],[FromRange],[ToRange],[DisPer],[Margin],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                + " ('" + row["CategoryName"] + "'," + DataBaseAccess.ConvertObjectToDoubleStatic(row["FromRange"]) + "," + DataBaseAccess.ConvertObjectToDoubleStatic(row["ToRange"]) + "," + DataBaseAccess.ConvertObjectToDoubleStatic(row["DisPer"]) + "," + DataBaseAccess.ConvertObjectToDoubleStatic(row["Margin"]) + ",GetDATE(),'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0) end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardChqDetailsMaster(DataTable dt, string strDataBase)
        {
            int count = 0, rowCount=0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            string strBillCode = "";
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strBillCode = Convert.ToString(row["BillCode"]);
                    if (!strBillCode.Contains("CHQO"))
                        strBillCode += "O";

                    strQuery += " if not exists (Select BillNo from [dbo].[ChequeDetails] Where BillCode='" + strBillCode + "' and BillNo=" + row["BillNo"] + " and RemoteID=" + row["ID"] + ") begin   "
                                + " INSERT INTO [dbo].[ChequeDetails] ([BillCode],[BillNo],[Date],[DebitAccountID],[CreditAccountID],[ChequeType],[DepositeDate],[Description],[Amount],[Status],[ActiveDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[RemoteID],[BankName],[BranchName],[FirmName],[ChequeNo]) VALUES "
                                + " ('" + strBillCode + "'," + row["BillNo"] + ",'" + row["Date"] + "','" + row["DebitAccountID"] + "','" + row["CreditAccountID"] + "','" + row["ChequeType"] + "','" + row["DepositeDate"] + "','" + row["Description"] + "'," + DataBaseAccess.ConvertObjectToDoubleStatic(row["Amount"]) + ",'" + row["Status"] + "','" + row["ActiveDate"] + "','" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "',0,0," + row["ID"] + ",'" + row["BankName"] + "','" + row["BranchName"] + "','" + row["FirmName"] + "','" + row["ChequeNo"] + "') end  ";

                    if (rowCount == 100)
                    {
                        count = ExecuteQueryInNew(strQuery);
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }


                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                    strQuery = "";
                }
            }
            return count;
        }

        public int ForwardMonthDetails(string strDataBase)
        {
            int count = 0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            
            if (strDataBase == MainPage.con.Database)
            {

                strQuery = "if not exists (Select [MonthName] from [dbo].[MonthLockDetails] Where [MonthName]='JANUARY') begin INSERT [dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES ( N'JANUARY', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails]([MonthName], [Status], [Date], [AllowUser]) VALUES(N'FEBRUARY', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'MARCH', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'APRIL', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'MAY', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'JUNE', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'JULY', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'AUGUST', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'SEPTEMBER', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'OCTOBER', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'NOVEMBER', N'UNLOCK', GETDATE(), N'') "
                         + " INSERT[dbo].[MonthLockDetails] ([MonthName], [Status], [Date], [AllowUser]) VALUES(N'DECEMBER', N'UNLOCK', GETDATE(), N'') end ";


                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                    strQuery = "";
                }
            }
            return count;
        }


        public int ForwardClosingStockBill(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "",strReceiptCode="";

            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strReceiptCode = Convert.ToString(row["ReceiptCode"])+"O";

                    strQuery += " if not exists(Select * from GoodsReceive Where ReceiptCode='" + strReceiptCode + "' and ReceiptNo='" + row["ReceiptNo"] + "' ) begin "
                                + " Insert into GoodsReceive ([ReceiptCode],[ReceiptNo],[OrderNo],[OrderDate],[SalesParty],[SubSalesParty],[PurchaseParty],[ReceivingDate],[Pieces],[Quantity],[Amount],[Freight],[Tax],[Item],[Packing],[Personal],[SaleBill],[PackingStatus],[CreatedBy],[PrintedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Box],[Remark],[SalePartyID],[SubPartyID],[PurchasePartyID],[InvoiceNo],[InvoiceDate],[PurchaseType],[ReverseCharge],[Dhara],[GrossAmount],[OtherSign],[OtherAmount],[DisPer],[DisAmount],[TaxPer],[TaxAmount],[NetAmount],[PurchaseStatus],[SpecialDscPer],[SpecialDscAmt],[PcsRateAmt]) Values "
                                + " ('" + strReceiptCode + "','" + row["ReceiptNo"] + "','" + row["OrderNo"] + "','" + row["OrderDate"] + "','" + row["SalesParty"] + "','" + row["SubSalesParty"] + "','" + row["PurchaseParty"] + "','" + row["ReceivingDate"] + "','" + row["Pieces"] + "','" + row["Quantity"] + "','" + row["Amount"] + "','" + row["Freight"] + "','" + row["Tax"] + "','" + row["Item"] + "','" + row["Packing"] + "','" + row["Personal"] + "','" + row["SaleBill"] + "','" + row["PackingStatus"] + "','" + row["CreatedBy"] + "','" + row["PrintedBy"] + "','" + row["UpdatedBy"] + "',1,0,'" + row["Box"] + "','" + row["Remark"] + "','" + row["SalePartyID"] + "','" + row["SubPartyID"] + "','" + row["PurchasePartyID"] + "','" + row["InvoiceNo"] + "','" + row["InvoiceDate"] + "','" + row["PurchaseType"] + "','" + row["ReverseCharge"] + "','" + row["Dhara"] + "'," + Convert.ToDouble(row["GrossAmount"]) + ",'" + row["OtherSign"] + "'," + Convert.ToDouble(row["OtherAmount"]) + "," + Convert.ToDouble(row["DisPer"]) + "," + Convert.ToDouble(row["DisAmount"]) + "," + Convert.ToDouble(row["TaxPer"]) + "," + Convert.ToDouble(row["TaxAmount"]) + "," + Convert.ToDouble(row["NetAmount"]) + ",'" + row["PurchaseStatus"] + "'," + Convert.ToDouble(row["SpecialDscPer"]) + "," + Convert.ToDouble(row["SpecialDscAmt"]) + "," + Convert.ToDouble(row["PcsRateAmt"]) + ") end ";//else begin 
                                
                                //+ " Update GoodsReceive Set [OrderNo]='" + row["OrderNo"] + "',[OrderDate]='" + row["OrderDate"] + "',[SalesParty]='" + row["SalesParty"] + "',[SubSalesParty]='" + row["SubSalesParty"] + "',[PurchaseParty]='" + row["PurchaseParty"] + "', [ReceivingDate]='" + row["ReceivingDate"] + "',[Pieces]='" + row["Pieces"] + "',[Quantity]='" + row["Quantity"] + "',[Amount]=" + row["Amount"] + ",[Freight]='" + row["Freight"] + "',[Tax]='" + row["Tax"] + "',[Item]='" + row["Item"] + "',[Packing]='" + row["Packing"] + "',[Personal]='" + row["Personal"] + "',[SaleBill]='" + row["SaleBill"] + "' ,"
                                //+ " [PackingStatus]='" + row["PackingStatus"] + "',[CreatedBy]='" + row["CreatedBy"] + "' ,[PrintedBy]='" + row["PrintedBy"] + "',[UpdatedBy]='" + row["UpdatedBy"] + "',[Box]='" + row["Box"] + "',[Remark]='" + row["Remark"] + "',[SalePartyID]='" + row["SalePartyID"] + "',[SubPartyID]='" + row["SubPartyID"] + "',[PurchasePartyID]='" + row["PurchasePartyID"] + "',[InvoiceNo]='" + row["InvoiceNo"] + "',[InvoiceDate]='" + row["InvoiceDate"] + "',[PurchaseType]='" + row["PurchaseType"] + "',[ReverseCharge]='" + row["ReverseCharge"] + "',[Dhara]='" + row["Dhara"] + "',"
                                //+ " [GrossAmount]=" + Convert.ToDouble(row["GrossAmount"]) + ",[OtherSign]='" + row["OtherSign"] + "',[OtherAmount]=" + Convert.ToDouble(row["OtherAmount"]) + ",[DisPer]=" + Convert.ToDouble(row["DisPer"]) + ",[DisAmount]=" + Convert.ToDouble(row["DisAmount"]) + ",[TaxPer]=" + Convert.ToDouble(row["TaxPer"]) + ",[TaxAmount]=" + Convert.ToDouble(row["TaxAmount"]) + ",[NetAmount]=" + Convert.ToDouble(row["NetAmount"]) + ",[SpecialDscPer]=" + Convert.ToDouble(row["SpecialDscPer"]) + ",[SpecialDscAmt]=" + Convert.ToDouble(row["SpecialDscAmt"]) + ",[PcsRateAmt]=" + Convert.ToDouble(row["PcsRateAmt"]) + "  Where [ReceiptCode]='" + row["ReceiptCode"] + "'  and [ReceiptNo] =" + row["ReceiptNo"] + " end  ";
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardClosingStockDetails(DataTable dt, string strDataBase)
        {
            int count = 0;
            string strQuery = "";

            string strReceiptCode = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                if (dt != null)
                {
                    DataTable _dt = dt.DefaultView.ToTable(true, "ReceiptCode", "ReceiptNo");
                    foreach (DataRow row in _dt.Rows)
                    {
                        strReceiptCode = Convert.ToString(row["ReceiptCode"]) + "O";

                        strQuery += " if exists(Select ReceiptCode from [dbo].[GoodsReceiveDetails] Where ReceiptCode='" + strReceiptCode + "' and ReceiptNo=" + row["ReceiptNo"] + ") begin "
                                 + " Delete from [dbo].[GoodsReceiveDetails] Where ReceiptCode='" + strReceiptCode + "' and ReceiptNo=" + row["ReceiptNo"] + " end ";
                    }


                    foreach (DataRow row in dt.Rows)
                    {
                        strReceiptCode = Convert.ToString(row["ReceiptCode"]) + "O";

                        strQuery += " if not exists(Select ReceiptCode from GoodsReceiveDetails Where ReceiptCode='" + strReceiptCode + "' and ReceiptNo=" + row["ReceiptNo"] + " and [RemoteID]=" + row["ID"] + " ) begin "
                                      + " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus],[RemoteID],[Rate],[GRate]) VALUES "
                                     + " ('" + strReceiptCode + "'," + row["ReceiptNo"] + ",'" + row["ItemName"] + "','" + row["PcsType"] + "'," + row["Quantity"] + "," + row["Amount"] + "," + row["PackingAmt"] + "," + row["FreightAmt"] + " ," + row["TaxAmt"] + " ,1,0," + row["ID"] + "," + Convert.ToDouble(row["Rate"]) + "," + Convert.ToDouble(row["GRate"]) + ") end  ";
                    }
                }

                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                }
            }
            return count;
        }

        public int ForwardBarCodeDetails(DataTable dt, string strDataBase)
        {
            int count = 0, rowCount=0;
            string strQuery = "";
            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strQuery += " if not exists (select ParentBarCode from BarCodedetails where ParentBarCode='" + row["ParentBarCode"] + "' and BarCode='" + row["BarCode"] + "') begin "
                                  + " INSERT INTO [dbo].[BarcodeDetails] ([BillCode],[BillNo],[ParentBarCode],[BarCode],[NetQty],[SetQty],[LastPrintNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[InStock]) values('FORWARDED','0','" + row["ParentBarCode"] + "','" + row["BarCode"] + "'," + NetDBAccess.ConvertObjectToDouble(row["NetQty"]) + "," + NetDBAccess.ConvertObjectToDouble(row["SetQty"]) + "," + NetDBAccess.ConvertObjectToDouble(row["LastPrintNo"]) + ",'" + row["CreatedBy"] + "','" + row["UpdatedBy"] + "','1','0','1') end ";

                    if (rowCount == 100)
                    {
                        //cmd = new SqlCommand(strQuery, MainPage.con);
                        count = ExecuteQueryInNew(strQuery);
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }
            }


            if (strQuery != "")
            {
                count += ExecuteQueryInNew(strQuery);
                strQuery = "";
            }
            return count;
        }

        public int ForwardClosingStock(DataTable dt, string strDataBase)
        {
            int count = 0, rowCount = 0;

            string strQuery = "";

            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                strQuery = "declare @BillCode nvarchar(50),@BillNo BigInt,@Billtype varchar(50),@Other2 varchar(50),@barcode varchar(50) declare TempCursor Cursor for 	"
                        + "select BillCode,BillNo,BillType,Other2,BarCode from StockMaster where BillType='OPENING' and ISNULL(Other2,'')='FORWARDED' order by BillCode,BillNo "
                        + "  open TempCursor   FETCH NEXT  FROM TempCursor  INTO @BillCode,@BillNo,@Billtype,@Other2,@Barcode WHILE @@FETCH_STATUS = 0 BEGIN	"
                        + "  update ItemSecondary set OpeningQty=0 where BillCode=@BillCode and BillNo=@BillNo and Description=@barcode "
                        + " FETCH NEXT  FROM TempCursor  INTO @BillCode,@BillNo,@Billtype,@Other2,@Barcode 	END CLOSE TempCursor DEALLOCATE TempCursor ;"
                        + " Delete from StockMaster Where BillType='OPENING' and ISNULL(Other2,'')='FORWARDED' ";

                foreach (DataRow row in dt.Rows)
                {
                    double dRate = NetDBAccess.ConvertObjectToDouble(row["Rate"]);
                    if (dRate <= 0)
                        dRate = 0;
                    double dMRP = NetDBAccess.ConvertObjectToDouble(row["MRP"]);
                    if (dMRP <= 0)
                        dMRP = 0;

                    strQuery += "  if not exists (Select ItemName from StockMaster Where BillType='OPENING' and BillCode='" + row["BCode"] + "' and BillNo=0 and ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' and Variant3='" + row["Variant3"] + "' and Variant4='" + row["Variant4"] + "' and Variant5='" + row["Variant5"] + "' and Rate=" + dRate + " and MRP=" + dMRP + " and ISNULL(BarCode,'') ='" + Convert.ToString(row["BarCode"]) + "') begin  "
                            + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[DesignName],[BrandName],[Other2]) VALUES "
                            + " ('OPENING','" + row["BCode"] + "','" + row["BillNo"] + "', '" + row["itemName"] + "','" + row["variant1"] + "','" + row["variant2"] + "','" + row["variant3"] + "','" + row["variant4"] + "','" + row["variant5"] + "'," + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + "," + dRate + " ,'','" + MainPage.strLoginName + "','',0,0," + dMRP + ",'" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["BarCode"] + "','" + row["DesignName"] + "','" + row["BrandName"] + "','FORWARDED')  end "
                            + " if not exists (Select Description from ItemSecondary where  BillCode='" + row["BCode"] + "' and BillNo='" + row["BillNo"] + "' and  Variant1='" + row["variant1"] + "' and Variant2='" + row["variant2"] + "' and Variant3='" + row["variant3"] + "' and Variant4='" + row["variant4"] + "' and Variant5='" + row["variant5"] + "' and ISNULL(Description,'') ='" + row["BarCode"] + "') begin"
                            + " insert into ItemSecondary ([RemoteID],BillCode,BillNo,Variant1,Variant2,Variant3,Variant4,Variant5,OpeningQty,OpeningRate,Description,SaleMRP,DesignName,Brand,[PurchaseRate],[Margin],[SaleRate],[Reorder],[ActiveStatus],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy]) "
                            + " Values (0,'" + row["BCode"] + "','" + row["BillNo"] + "','" + row["variant1"] + "','" + row["variant2"] + "','" + row["variant3"] + "','" + row["variant4"] + "','" + row["variant5"] + "'," + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + "," + dRate + ",'" + row["BarCode"] + "'," + row["SaleMRP"] + ",'" + row["DesignName"] + "','" + row["BrandName"] + "'," + row["PurchaseRate"] + ",0," + row["SaleRate"] + ",0,1,0,0,'','') end"
                            + " else begin update ItemSecondary set OpeningQty=" + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + " where BillCode='" + row["BCode"] + "' and BillNo='" + row["BillNo"] + "' and  Variant1='" + row["variant1"] + "' and Variant2='" + row["variant2"] + "' and Variant3='" + row["variant3"] + "' and Variant4='" + row["variant4"] + "' and Variant5='" + row["variant5"] + "'  and ISNULL(Description,'') ='" + row["BarCode"] + "' end";


                    //strQuery += "  if not exists (Select ItemName from StockMaster Where BillType='OPENING' and BillCode='" + row["BCode"] + "' and BillNo=0 and ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' and Variant3='" + row["Variant3"] + "' and Variant4='" + row["Variant4"] + "' and Variant5='" + row["Variant5"] + "' and Rate=" + dRate + " and MRP=" + dMRP + " and ISNULL(BarCode,'') ='" + Convert.ToString(row["BarCode"]) + "') begin  "
                    //         + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[DesignName],[BrandName],[Other2]) VALUES "
                    //         + " ('OPENING','" + row["BCode"] + "','" + row["BillNo"] + "', '" + row["itemName"] + "','" + row["variant1"] + "','" + row["variant2"] + "','" + row["variant3"] + "','" + row["variant4"] + "','" + row["variant5"] + "'," + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + "," + dRate + " ,'','" + MainPage.strLoginName + "','',0,0," + dMRP + ",'" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["BarCode"] + "','" + row["DesignName"] + "','" + row["BrandName"] + "','FORWARDED')  end "
                    //         + " if not exists (Select Description from ItemSecondary where  BillCode='" + row["BCode"] + "' and BillNo='" + row["BillNo"] + "' and  Variant1='" + row["variant1"] + "' and Variant2='" + row["variant2"] + "' and Variant3='" + row["variant3"] + "' and Variant4='" + row["variant4"] + "' and Variant5='" + row["variant5"] + "' and ISNULL(Description,'') ='" + row["BarCode"] + "') begin"
                    //         + " insert into ItemSecondary ([RemoteID],BillCode,BillNo,Variant1,Variant2,Variant3,Variant4,Variant5,OpeningQty,OpeningRate,Description,SaleMRP,DesignName,Brand,[PurchaseRate],[Margin],[SaleRate],[Reorder],[ActiveStatus],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy]) "
                    //         + " Values (0,'" + row["BCode"] + "','" + row["BillNo"] + "','" + row["variant1"] + "','" + row["variant2"] + "','" + row["variant3"] + "','" + row["variant4"] + "','" + row["variant5"] + "'," + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + "," + dRate + ",'" + row["BarCode"] + "',0,'" + row["DesignName"] + "','" + row["BrandName"] + "',0,0,0,0,1,0,0,'','') end"
                    //         + " else begin update ItemSecondary set OpeningQty=" + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + " where BillCode='" + row["BCode"] + "' and BillNo='" + row["BillNo"] + "' and  Variant1='" + row["variant1"] + "' and Variant2='" + row["variant2"] + "' and Variant3='" + row["variant3"] + "' and Variant4='" + row["variant4"] + "' and Variant5='" + row["variant5"] + "'  and ISNULL(Description,'') ='" + row["BarCode"] + "' end";

                    if (rowCount == 50)
                    {
                        //cmd = new SqlCommand(strQuery, MainPage.con);
                        count = ExecuteQueryInNew(strQuery);
                        if (count != -2)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }
                if (strQuery != "")
                {
                    count += ExecuteQueryInNew(strQuery);
                    strQuery = "";
                }

                //if (!MainPage._bBarCodeStatus)
                {
                    strQuery = " Update _92ist Set _92ist.SaleRate = PBS.SaleRate from " + strDataBase + ".dbo.Items _92IM left join " + strDataBase + ".dbo.ItemSecondary _92ist on _92IM.BillCode = _92ist.BillCode and _92IM.BillNo = _92ist.BillNo  left join " + MainPage.strDataBaseFile + ".dbo.Items _IM on _IM.ItemName = _92IM.ItemName left join " + MainPage.strDataBaseFile + ".dbo.ItemSecondary PBS on _Im.BillCode = PBS.BillCode and _IM.BillNo = PBS.BillNo and _92ist.Description = pbs.Description and _92ist.Variant1 = pbs.Variant1 WHere ISNULL(PBS.SaleRate, 0)!= 0 "
                             + " Update iss set iss.PurchaseRate=Rate,iss.SaleMRP=pbs.SaleMRP,iss.SaleRate=pbs.SaleRate from " + strDataBase + ".dbo.items im inner join " + strDataBase + ".dbo.ItemSecondary iss on im.BillCode=iss.BillCode and im.BillNo=iss.BillNo OUTER APPLY (Select Top 1 Rate,SaleRate,SaleMRP from " + MainPage.strDataBaseFile + ".dbo.PurchaseBookSecondary pbs WHere pbs.BarCode=iss.Description and pbs.ItemName=im.ItemName and pbs.Variant1=iss.Variant1 order by pbs.BillNo desc)pbs "
                             + " Update ist Set ist.SaleRate=PBS.SaleRate from " + strDataBase + ".dbo.ItemStock ist left join " + MainPage.strDataBaseFile + ".dbo.ItemStock PBS on ist.BarCode=pbs.BarCode and ist.ItemName=pbs.ItemName and ist.Variant1=pbs.Variant1 WHere ISNULL(pbs.SaleRate,0)!=0 "
                             + " Update _iss Set _iss.PurchaseRate = pbs.Rate,_iss.SaleMRP = pbs.SaleMRP, _iss.SaleRate = pbs.SaleRate from Items _is inner join ItemSecondary _iss on _is.billno = _iss.BillNo and _is.BillCode = _iss.BillCode Cross APPLY (Select top 1 Rate, SaleMRP, SaleRate from PurchaseBookSecondary pbs where _is.ItemName = pbs.ItemName and _iss.Description=pbs.barcode and _iss.Variant1 = pbs.Variant1 order by pbs.BillNo desc)PBS "
                             + " Update ist Set ist.SaleRate=PBS.SaleRate from " + strDataBase + ".dbo.ItemStock ist left join " + strDataBase + ".dbo.Items _IM on _IM.ItemName=IST.ItemName left join " + strDataBase + ".dbo.ItemSecondary PBS on _Im.BillCode=PBS.BillCode and _IM.BillNo=PBS.BillNo and ist.BarCode=pbs.Description and ist.Variant1=pbs.Variant1 WHere ISNULL(PBS.SaleRate,0)!=0 ";
                }
                count += ExecuteQueryInNew(strQuery);


            }
            return count;
        }

        public int ForwardClosingStock_PTN(DataTable dt, string strDataBase)
        {
            int count = 0, rowCount = 0;

            string strQuery = "";

            MainPage.ChangeDataBase(strDataBase);
            if (strDataBase == MainPage.con.Database)
            {
                strQuery = "Delete from StockMaster Where BillType='OPENING'and BillNo=0 ";

                foreach (DataRow row in dt.Rows)
                {
                    double dRate = NetDBAccess.ConvertObjectToDouble(row["Rate"]);
                    double dMRP = NetDBAccess.ConvertObjectToDouble(row["MRP"]);
                    strQuery += "  if not exists (Select ItemName from StockMaster Where BillType='OPENING' and BillCode='" + row["BCode"] + "' and BillNo=0 and ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' and Variant3='" + row["Variant3"] + "' and Variant4='" + row["Variant4"] + "' and Variant5='" + row["Variant5"] + "' and Rate=" + dRate + " and MRP=" + dMRP + " and ISNULL(BarCode,'') ='" + Convert.ToString(row["BarCode"]) + "') begin  "
                             + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[DesignName]) VALUES "
                             + " ('OPENING','" + row["BCode"] + "',0, '" + row["itemName"] + "','" + row["variant1"] + "','" + row["variant2"] + "','" + row["variant3"] + "','" + row["variant4"] + "','" + row["variant5"] + "'," + NetDBAccess.ConvertObjectToDouble(row["Qty"]) + "," + dRate + " ,'','" + MainPage.strLoginName + "','',0,0," + dMRP + ",'" + MainPage.endFinDate.AddSeconds(1).ToString("MM/dd/yyyy") + "','" + row["BarCode"] + "','" + row["DesignName"] + "')  end ";

                    if (rowCount == 50)
                    {
                        cmd = new SqlCommand(strQuery, MainPage.con);
                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            strQuery = "";
                            rowCount = 0;
                        }
                    }
                    rowCount++;
                }


                if (strQuery != "")
                {
                    cmd = new SqlCommand(strQuery, MainPage.con);
                    count = cmd.ExecuteNonQuery();
                    strQuery = "";
                }

                if (MainPage.strSoftwareType == "AGENT")
                    strQuery = " Update sm set sm.Rate=sm1.Rate,sm.MRP=sm1.MRP from " + strDataBase + ".dbo.StockMaster sm inner join " + MainPage.strDataBaseFile + ".dbo.StockMaster sm1 on sm.ItemName=sm1.ItemName and sm.Variant1=sm1.Variant1 and sm.Variant2=sm.Variant2  WHere Sm1.BillType not in ('SALES','SALERETURN') and ISNULL(sm1.Rate,0)!=0 and ISNULL(sm.Rate,0)=0 ";

                count += ExecuteQueryInNew(strQuery);
            }
            return count;
        }




        #endregion

    }
}
