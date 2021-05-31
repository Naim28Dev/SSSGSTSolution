using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace SSS
{
    public class SendPDFToParty
    {
        DataBaseAccess dba;
        public SendPDFToParty()
        {
            dba = new SSS.DataBaseAccess();
        }

        protected internal bool GenerateTableForSending()
        { 
            DataTable myDataTable = CreateDataTable();
            try
            {  
                string strQuery = "", strDate = "", strPartyName = "";

                strQuery += " Select AccountID,ID,Convert(varchar,Date,103) Date, CONVERT(Date,Date,103)BDate,ISNULL(UPPER(AccountStatus),'') AccountStatus,Description,DebitAmt,CreditAmt from (  "
                         + " Select 0 as ID,BA.BalanceID,AccountID,Date,AccountStatus,Description,(Case when Status = 'Debit' then Amount else '' end) DebitAmt,(Case when Status = 'Credit' then Amount else '' end) CreditAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster SM Where (AreaCode + AccountNo) = AccountID and GroupName = 'SUNDRY DEBTORS') SM Where AccountStatus = 'OPENING' and CAST(Amount as Money) > 0 and Tick = 'FALSE' and AccountID not in (Select BLP.AccountID from [dbo].[BulkLedgerPosting] BLP Where CONVERT(varchar,Date,103)=Convert(varchar,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),103)) Union All "
                         + " Select 1 as ID, BA.BalanceID, AccountID, Date, (CASE When AccountStatus = 'SALES A/C' OR AccountStatus = 'PURCHASE A/C' OR AccountStatus = 'SALE RETURN' OR AccountStatus = 'PURCHASE RETURN' OR AccountStatus = 'JOURNAL A/C' OR AccountStatus = 'SALE SERVICE' OR AccountStatus = 'CREDIT NOTE' OR AccountStatus = 'DEBIT NOTE' then AccountStatus else dbo.GetFullName(AccountStatusID) end + (CASE When VoucherCode != '' then ' | ' + VoucherCode + ' ' + CAST(VoucherNo as varchar) else '' end)) AccountStatus,Description,(Case when Status = 'Debit' then Amount else '' end) DebitAmt,(Case when Status = 'Credit' then Amount else '' end) CreditAmt from BalanceAmount BA  CROSS APPLY (Select Name from SupplierMaster SM Where (AreaCode + AccountNo) = AccountID and GroupName = 'SUNDRY DEBTORS') SM OUTER APPLY(Select BA1.Tick as _Tick from BalanceAmount BA1 Where BA1.AccountStatus = 'OPENING' and CAST(AMount as Money) > 0 and BA1.AccountID = BA.AccountID) BA1 Where AccountStatus != 'OPENING' and CAST(Amount as Money) > 0 and AccountID not in (Select BLP.AccountID from [dbo].[BulkLedgerPosting] BLP Where CONVERT(varchar,Date,103)=Convert(varchar,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),103)) and (ISNULL(_Tick, 'True') != BA.Tick OR 'FALSE' = BA.Tick) "
                         + " ) Balance Order By AccountID, ID, Balance.Date "
                         + " Select AccountNo,EmailID,WhatsappNo,(AreaCode+AccountNo)AccountID,Name,(SM.Address + ', '+SM.Station+', '+SM.State+'-'+SM.PinCode)Address,(SM.MobileNo+ ' '+SM.PhoneNo)PhoneNo,SM.AccountNo,CD.* from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SOD.AreaCode=SM.AreaCode and SOD.AccountNo=SM.AccountNo)SOD Outer Apply (Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD  Order by CD.ID asc) CD Where GroupName='SUNDRY DEBTORS'  and (AreaCode+AccountNo) not in (Select AccountID from [dbo].[BulkLedgerPosting] Where CONVERT(varchar,Date,103)=Convert(varchar,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),103)) ";
                
                DataSet _ds = DataBaseAccess.GetDataSetRecord(strQuery);

                if (_ds.Tables.Count > 0)
                {
                    strDate = "Date Period : From " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                    DataTable _dtBalance = _ds.Tables[0], _dtPartyDetail = _ds.Tables[1];

                    DataTable _dtParty = _dtBalance.DefaultView.ToTable(true, "AccountID");
                    //int _count = 1;
                    foreach (DataRow row in _dtParty.Rows)
                    {
                        //if (_count > 1545)
                        {
                            myDataTable.Rows.Clear();
                            DataRow[] _rows = _dtBalance.Select("AccountID='" + row["AccountID"] + "' ");
                            if (_rows.Length > 0)
                            {
                                DataTable _dtDetail = _rows.CopyToDataTable();
                                DataView _dv = _dtDetail.DefaultView;
                                _dv.Sort = "ID,BDate";
                                _dtDetail = _dv.ToTable();

                                DataRow[] _drPartyDetails = _dtPartyDetail.Select("AccountID='" + row["AccountID"] + "' ");
                                if (_drPartyDetails.Length > 0)
                                {
                                    string strEmailID = Convert.ToString(_drPartyDetails[0]["EmailID"]), strWhatsappNo = Convert.ToString(_drPartyDetails[0]["WhatsappNo"]);
                                    if (strEmailID != "" || strWhatsappNo != "")
                                    {
                                        myDataTable = SetValueinDataTable(myDataTable, _dtDetail, _drPartyDetails[0], strDate);

                                        if (myDataTable.Rows.Count > 0)
                                        {
                                            string strPartyID= Convert.ToString(row["AccountID"]), strFileName = "", strPath = GetFileName(strPartyID, ref strFileName);
                                            strPath = ExportPDFFile(strPath, myDataTable);
                                            bool _bSentStatus = false;
                                            if (strPath != "")
                                            {
                                                string strMessage = "A/c : " + (strPartyID + " " + _drPartyDetails[0]["Name"]) + ", we are sending ledger statement which is attached with this mail, Please find attachment.";
                                                string strSub = "LEDGER STATEMENT";
                                                bool _bEmailStatus = false;
                                                if (strEmailID != "")
                                                    _bEmailStatus = DataBaseAccess.SendEmail(strEmailID, strSub, strMessage, strPath, "", "LEDGER STATEMENT",false);

                                                if (strWhatsappNo != "")
                                                {
                                                    _bSentStatus =SendWhatsappMessage(strWhatsappNo, strPath, strFileName, _bEmailStatus, strEmailID, Convert.ToString(_drPartyDetails[0]["Name"]));
                                                }
                                            }

                                            string strStatusQuery = " if not exists (Select [AccountID] from [dbo].[BulkLedgerPosting] Where [AccountID]='"+ strPartyID+ "' and CONVERT(varchar,Date,103)=Convert(varchar,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),103)) begin "
                                                                 + " INSERT INTO[dbo].[BulkLedgerPosting] ([AccountID],[Date],[Status],[SentBy])VALUES('"+ strPartyID+"',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'"+_bSentStatus+"','"+MainPage.strLoginName+"') end ";
                                            DataBaseAccess.ExecuteMyNonQuery(strStatusQuery);
                                        }
                                    }
                                }
                            }
                        }
                        //_count++;
                    }
                }
            }
            catch
            {
                throw;
            }
            return true;
        }

        private bool SendWhatsappMessage(string strMobileNo, string strPath, string strFileName, bool _bEmailStatus, string strEmailID, string strName)
        {
            try
            {
                string strMessage = "";
                string strFilePath = MainPage.strHttpPath + "/Ledger_Statement/" + strFileName;

               // strMessage = "M/S : " + strName + ", WE ARE SENDING LEDGER STATEMENT, PLEASE FIND ATTACHMENT.";              

                bool  _bStatus = dba.UploadLedgerInterestStatementPDFFile(strPath, strFileName, "Ledger_Statement");
                if (_bStatus)
                {
                    strMessage = "\"variable1\": \"" + strName + "\",";
                    string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, "ledger_pdf", strMessage, "", strFilePath);

                    if (strResult.Contains("SENT"))
                    {                       
                        _bEmailStatus = false;
                        return true;                   
                    }
                }
            }
            catch { }
            return false;
        }

        private DataTable SetValueinDataTable(DataTable myDataTable, DataTable _dtDetail,DataRow _drPartyDetails,string strDate)
        {
            ChangeCurrencyToWord currency = new ChangeCurrencyToWord();

            string strPartyName = (_drPartyDetails["AccountID"] + " " + _drPartyDetails["Name"]);
            double dDAmt = 0, dCAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dBalanceAmt = 0, dNetBalanceAmt = 0;
            foreach (DataRow rowDetails in _dtDetail.Rows)
            {
                try
                {
                    DataRow dRow = myDataTable.NewRow();

                    dRow["CompanyName"] = MainPage.strPrintComapanyName;
                    dRow["PartyName"] = strPartyName;

                    dRow["DatePeriod"] = strDate;
                    dRow["Date"] = rowDetails["Date"];
                    dRow["Account"] = rowDetails["AccountStatus"];

                    if (Convert.ToString(rowDetails["DebitAmt"]) != "")
                    {
                        dDAmt = ConvertObjectToDouble(rowDetails["DebitAmt"]);
                        dDebitAmt += dDAmt;
                        dBalanceAmt += dDAmt;

                        dRow["DebitAmt"] = dDAmt.ToString("N2", MainPage.indianCurancy);
                        dRow["CreditAmt"] = "";
                    }
                    else
                    {
                        dCAmt = ConvertObjectToDouble(rowDetails["CreditAmt"]);
                        dCreditAmt += dCAmt;
                        dBalanceAmt -= dCAmt;

                        dRow["CreditAmt"] = dCAmt.ToString("N2", MainPage.indianCurancy);
                        dRow["DebitAmt"] = "";
                    }
                    if (dBalanceAmt > 0)
                        dRow["Balance"] = dBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else if (dBalanceAmt < 0)
                        dRow["Balance"] = Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                    else
                        dRow["Balance"] = "0.00";

                    dRow["Description"] = rowDetails["Description"];
                    dRow["OnAccount"] = "0";

                    dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    myDataTable.Rows.Add(dRow);
                }
                catch
                {
                }
            }

            myDataTable.Rows[0]["Address"] = _drPartyDetails["Address"];         
            myDataTable.Rows[0]["PhoneNo"] = _drPartyDetails["PhoneNo"];
            myDataTable.Rows[0]["FirmName"] = "SUNDRY DEBTORS";

            myDataTable.Rows[0]["CompanyAddress"] = _drPartyDetails["CompanyAddress"];
            myDataTable.Rows[0]["CompanyEmail"] = _drPartyDetails["CompanyPhoneNo"];
            myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _drPartyDetails["GSTNo"];
            myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _drPartyDetails["CINNumber"];

            myDataTable.Rows[0]["BankName"] = "ICICI BANK";
            myDataTable.Rows[0]["BranchName"] = "DELHI";
            myDataTable.Rows[0]["AccountNo"] = "SASUSP" + ConvertObjectToDouble(_drPartyDetails["AccountNo"]).ToString("000000");
            myDataTable.Rows[0]["IFSCCode"] = "ICIC0000106";

            int _rCount = myDataTable.Rows.Count - 1;
            string strNumeric = "Zero";

            myDataTable.Rows[_rCount]["TotalDebit"] = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            myDataTable.Rows[_rCount]["TotalCredit"] = dCreditAmt.ToString("N2", MainPage.indianCurancy);

            dNetBalanceAmt = Math.Abs(dBalanceAmt);

            if (dBalanceAmt > 0)
            {
                strNumeric = currency.changeCurrencyToWords(dNetBalanceAmt);
                myDataTable.Rows[_rCount]["TotalBalance"] = dBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                strNumeric += " Debit";
            }
            else if(dBalanceAmt < 0)
            {
                strNumeric = currency.changeCurrencyToWords(dNetBalanceAmt);
                myDataTable.Rows[_rCount]["TotalBalance"] = Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                strNumeric += " Credit";
            }

            myDataTable.Rows[_rCount]["AmountInWord"] = strNumeric;

            return myDataTable;
        }


        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            myDataTable.Columns.Add("CompanyName", typeof(String));
            myDataTable.Columns.Add("PartyName", typeof(String));
            myDataTable.Columns.Add("Address", typeof(String));
            myDataTable.Columns.Add("PostOffice", typeof(String));
            myDataTable.Columns.Add("PhoneNo", typeof(String));
            myDataTable.Columns.Add("DatePeriod", typeof(String));
            myDataTable.Columns.Add("Date", typeof(String));
            myDataTable.Columns.Add("Account", typeof(String));
            myDataTable.Columns.Add("DebitAmt", typeof(String));
            myDataTable.Columns.Add("CreditAmt", typeof(String));
            myDataTable.Columns.Add("Balance", typeof(String));
            myDataTable.Columns.Add("Description", typeof(String));
            myDataTable.Columns.Add("TotalDebit", typeof(String));
            myDataTable.Columns.Add("TotalCredit", typeof(String));
            myDataTable.Columns.Add("TotalBalance", typeof(String));
            myDataTable.Columns.Add("AmountInWord", typeof(String));
            myDataTable.Columns.Add("UserName", typeof(String));
            myDataTable.Columns.Add("OnAccount", typeof(String));
            myDataTable.Columns.Add("BankName", typeof(String));
            myDataTable.Columns.Add("BranchName", typeof(String));
            myDataTable.Columns.Add("AccountNo", typeof(String));
            myDataTable.Columns.Add("IFSCCode", typeof(String));
            myDataTable.Columns.Add("CHQAccountNo", typeof(String));
            myDataTable.Columns.Add("FirmName", typeof(String));
            myDataTable.Columns.Add("CompanyAddress", typeof(String));
            myDataTable.Columns.Add("CompanyEmail", typeof(String));
            myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
            myDataTable.Columns.Add("CompanyCINNo", typeof(String));

            return myDataTable;
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

        private string GetFileName(string strAccountID,ref string _srtFileName)
        {            
            string strOrginalFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string strPartyID = strAccountID, strFileName = "", strPath = "";
            _srtFileName= strFileName = strPartyID + "_" + strOrginalFileName + ".pdf";

            string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Ledger_Statement";
            strPath = strNewPath + "\\" + strFileName;

            if (File.Exists(strPath))
                File.Delete(strPath);

            Directory.CreateDirectory(strNewPath);
            return strPath;
        }

        private string ExportPDFFile(string strPath, DataTable dt)
        {
            try
            {
                DataTable _dtAdvance = dt.Clone();
                if (dt.Rows.Count > 0)
                {
                    using (Reporting.LedgerReport_New report = new SSS.Reporting.LedgerReport_New())
                    {
                        report.SetDataSource(dt);
                        report.Subreports[0].SetDataSource(_dtAdvance);

                        if (strPath != "" && strPath.Contains("\\"))
                        {
                            report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        }
                        report.Dispose();
                    }
                }
            }
            catch(Exception ex)
            {
                strPath = "";
                throw ex;
            }
            return strPath;
        }


    }
}
