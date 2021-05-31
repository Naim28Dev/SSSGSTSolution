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
    public partial class UserLogin : Form
    {
        DataBaseAccess dba;
        public static string strUserName = "";
        MainPage mainPage;
        DataTable table;

        public UserLogin()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                mainPage = MainPage.mymainObject;// as MainPage;
                MainPage.strLoginName = MainPage.strUserRole = "";
               

                if (MainPage.strFolderName.Contains("LOCAL") || MainPage.strFolderName.Contains("SSS"))
                {
                    txtUserType.Text = "NORMAL";
                    txtUserType.Enabled = txtUserType.TabStop = false;
                    //txtLogin.Text = MainPage.strOldUserName;
                    //txtPassword.Text = MainPage.strOldPassword;
                    //if (txtLogin.Text != "" && txtPassword.Text != "")
                    //    VerifyUser();
                }
                if(MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO"))
                {
                    txtUserType.TabStop = false;
                    txtLogin.Focus();
                }
                table = DataBaseAccess.GetDataTableRecord("Select * from UserAccount ");
                //lblHeader.Text = MainPage.strCompanyName + " LOGIN";
                //SoftVersionSetup();
            }
            catch
            {
            }
        }

        private void SoftVersionSetup()
        {
            try
            {
                string strQuery = "";
                if (MainPage.strPlanType == "SILVER" || MainPage.strPlanType == "GOLD")
                {
                    MainPage.mymainObject.bDrillDownReport = false;
                    strQuery = " Select Modules,Access from Version_Control Where PlanType='" + MainPage.strPlanType + "' order by ID";
                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        if (MainPage.strPlanType == "SILVER")
                        {
                            MainPage.mymainObject.bDigitallySignedInvoice = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DigitallySignedInvoice" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bE_InvoicingFacility = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "E_InvoicingFacility" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bBranchWiseBalanceSheet = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "BranchWiseBalanceSheet" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bAdjustUnadjustAccount = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "LedgerStatementAdjustement" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bRefrenceMaster = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "RefrenceMaster" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bCreditLimitmanagement = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "CreditLimitManagement" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bSchemeMaster = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "SchemeManagement" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bLockUnlockCustomer = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "TransectionLockUnlock" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bLoyalityCardSystem = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "LoyalityCardSystem" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bPayRoll = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "PayRoll" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bBankWayBill = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Import_Bank_Way_Bill" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bRemovalReason = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "RemovalReason" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bReminder = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "ReminderSetUp" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage._PrintWithDialog = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "PrintingSetting" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bInterestStatement = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "InterestStatement" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bDueDateWisereport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DueDateWisereport" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bPartyBalanceSlabWise = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "PartyBalanceSlabWise" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bPrintMultiParty = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "PrintMultiLedger" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bBlackListTransectionReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "BlackListTransectionReport" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bUnmovedItemReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "UnmovedItemReport" select (Boolean)dr["Access"]).FirstOrDefault();
                            //MainPage.mymainObject.bDayBookRegister = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DayBookRegister" select (Boolean)dr["Access"]).FirstOrDefault();
                            //MainPage.mymainObject.bDebtorCreditorReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DebitNoteRegister" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bAmendedBillReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "AmendedBillReport" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bDashboard = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Dashboard" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bImport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Import" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bExport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Export" select (Boolean)dr["Access"]).FirstOrDefault();
                            

                        }
                        else if (MainPage.strPlanType == "GOLD")
                        {

                            //MainPage.mymainObject.bBarcodePrint = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "BarcodePrint" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bDigitallySignedInvoice = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DigitallySignedInvoice" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bE_InvoicingFacility = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "E_InvoicingFacility" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bLoyalityCardSystem = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "LoyalityCardSystem" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bBankWayBill = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Import_Bank_Way_Bill" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bRemovalReason = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "RemovalReason" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage._PrintWithDialog = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "PrintingSetting" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bUnmovedItemReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "UnmovedItemReport" select (Boolean)dr["Access"]).FirstOrDefault();
                            //MainPage.mymainObject.bDebtorCreditorReport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "DebitNoteRegister" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bDashboard = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Dashboard" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bImport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Import" select (Boolean)dr["Access"]).FirstOrDefault();
                            MainPage.mymainObject.bExport = (from DataRow dr in dt.Rows where (string)dr["Modules"] == "Export" select (Boolean)dr["Access"]).FirstOrDefault();

                        }
                    }
                }
                else
                    MainPage.mymainObject.bDrillDownReport = true;
            }
            catch (Exception ex)
            { }
        }

        private bool VerifyUserName(string strUName, string strPwd)
        {
            bool status = false;
            try
            {
                DataRow[] rows = table.Select(String.Format(" LoginName='" + strUName + "' and Password='" + strPwd + "'"));
                if (rows.Length > 0)
                {
                    DataRow row = rows[0];
                    strUserName = Convert.ToString(row["LoginName"]);
                    if (strUName == strUserName)
                    {
                        mainPage.adminPToolStripMenuItem.Visible = false;
                        //MainPage.mymainObject.bCashAdd = MainPage.mymainObject.bCashEdit = MainPage.mymainObject.bCashView = MainPage.mymainObject.bBankAdd = MainPage.mymainObject.bBankEdit = MainPage.mymainObject.bBankView = MainPage.mymainObject.bJournalAdd = MainPage.mymainObject.bJournalEdit = MainPage.mymainObject.bJournalView = true;
                        
                        MainPage.mymainObject.bJournalAdd = Convert.ToBoolean(row["JournalEntry"]);
                        MainPage.mymainObject.bJournalEdit = Convert.ToBoolean(row["JournalEdit"]);
                        MainPage.mymainObject.bJournalView = Convert.ToBoolean(row["JournalView"]);

                        MainPage.mymainObject.bCashAdd = Convert.ToBoolean(row["CashEntry"]);
                        MainPage.mymainObject.bCashEdit = Convert.ToBoolean(row["CashEdit"]);
                        MainPage.mymainObject.bCashView = Convert.ToBoolean(row["CashView"]);

                        MainPage.mymainObject.bOrderAdd = Convert.ToBoolean(row["OrderEntry"]);
                        MainPage.mymainObject.bOrderEdit = Convert.ToBoolean(row["OrderEdit"]);
                        MainPage.mymainObject.bOrderView = Convert.ToBoolean(row["OrderView"]);

                        MainPage.mymainObject.bFullEditControl = Convert.ToBoolean(row["GoodsEntry"]);
                        MainPage.mymainObject.bAddPaymentRequest = Convert.ToBoolean(row["GoodsEdit"]);
                        MainPage.mymainObject.bOtherExtraControl = Convert.ToBoolean(row["GoodsView"]);

                        MainPage.mymainObject.bSaleAdd = Convert.ToBoolean(row["SaleEntry"]);
                        MainPage.mymainObject.bSaleEdit = Convert.ToBoolean(row["SaleEdit"]);
                        MainPage.mymainObject.bSaleView = Convert.ToBoolean(row["SaleView"]);

                        MainPage.mymainObject.bPurchaseAdd = Convert.ToBoolean(row["PurchaseEntry"]);
                        MainPage.mymainObject.bPurchaseEdit = Convert.ToBoolean(row["PurchaseEdit"]);
                        MainPage.mymainObject.bPurchaseView = Convert.ToBoolean(row["PurchaseView"]);

                        MainPage.mymainObject.bDownloadRequest = Convert.ToBoolean(row["ForwardingEntry"]); //bForwardingAdd
                        MainPage.mymainObject.bSendRequest = Convert.ToBoolean(row["ForwardingView"]); //bForwardingEdit
                        MainPage.mymainObject.bChangeStatus = Convert.ToBoolean(row["ForwardingEdit"]); //bForwardingView

                        MainPage.mymainObject.bCourierAdd = Convert.ToBoolean(row["CourierEntry"]);
                        MainPage.mymainObject.bCourierEdit = Convert.ToBoolean(row["CourierEdit"]);
                        MainPage.mymainObject.bCourierView = Convert.ToBoolean(row["CourierView"]);

                        MainPage.mymainObject.bPartyMasterAdd = Convert.ToBoolean(row["NewParty"]);
                        MainPage.mymainObject.bPartyMasterEdit = Convert.ToBoolean(row["NewPartyEdit"]);
                        MainPage.mymainObject.bPartyMasterView = Convert.ToBoolean(row["PartyView"]);

                        MainPage.mymainObject.bSubPartyAdd = Convert.ToBoolean(row["NewSubParty"]);
                        MainPage.mymainObject.bSubPartyEdit = Convert.ToBoolean(row["SubPartyEdit"]);
                        MainPage.mymainObject.bSubPartyView = Convert.ToBoolean(row["SubPartyView"]);

                        MainPage.mymainObject.bAccountMasterAdd = Convert.ToBoolean(row["NewAccountMaster"]);
                        MainPage.mymainObject.bAccountMasterEdit = Convert.ToBoolean(row["AccountMasterEdit"]);
                        MainPage.mymainObject.bAccountMasterView = Convert.ToBoolean(row["AccountMasterView"]);

                        MainPage.mymainObject.bLedgerReport = Convert.ToBoolean(row["LedgerInterest"]);
                        MainPage.mymainObject.bPrintMultiParty = Convert.ToBoolean(row["PrintMultiLedger"]);
                        MainPage.mymainObject.bPurchaseSlip = Convert.ToBoolean(row["PurchaseOutStanding"]);
                        MainPage.mymainObject.bDebtorCreditorReport = Convert.ToBoolean(row["CrediterDebter"]);
                        MainPage.mymainObject.bAdjustUnadjustAccount = Convert.ToBoolean(row["ShowAmountLimit"]);
                        MainPage.mymainObject.bFASReport = Convert.ToBoolean(row["FASReport"]);
                        MainPage.mymainObject.bReportSummary = Convert.ToBoolean(row["ReportSummeryView"]);
                        MainPage.mymainObject.bBackDayEntry = Convert.ToBoolean(row["BackDateEntry"]);
                        MainPage.mymainObject.bSMSReport = Convert.ToBoolean(row["SMS"]);
                        MainPage.mymainObject.bSendToInternet = Convert.ToBoolean(row["Other"]);
                        MainPage.mymainObject.bCashAdd = Convert.ToBoolean(row["CashEntry"]);
                        MainPage.mymainObject.bPrivilegeAccount = Convert.ToBoolean(row["DayBook"]);
                        MainPage.mymainObject.bReminder = Convert.ToBoolean(row["Reminder"]);
                        MainPage.mymainObject.bF5Report = Convert.ToBoolean(row["PartyLedger"]);
                        MainPage.mymainObject.bOrderSlip = Convert.ToBoolean(row["OrderSlipView"]);                        
                        MainPage.mymainObject.bSaleReport = Convert.ToBoolean(row["SalesReportView"]);
                        MainPage.mymainObject.bPurchaseReport = Convert.ToBoolean(row["PurchaseReport"]);                        
                        MainPage.mymainObject.bCourierReport = Convert.ToBoolean(row["Reportview"]);
                        MainPage.mymainObject.bCompanyInfo = Convert.ToBoolean(row["CompanyInfo"]); 

                        MainPage.mymainObject.bDrCrNoteAdd = Convert.ToBoolean(row["ForwardingReport"]);
                        MainPage.mymainObject.bDrCrNoteView = Convert.ToBoolean(row["GoodsRecivedView"]);
                        MainPage.mymainObject.bDrCrNoteEdit = Convert.ToBoolean(row["OnAccount"]);

                        MainPage.mymainObject.bAccessories = Convert.ToBoolean(row["Accessories"]);
                        MainPage.mymainObject.bBckupRestore = Convert.ToBoolean(row["BackupRestore"]);
                        MainPage.mymainObject.bMergingParty = Convert.ToBoolean(row["Merging"]);
                        MainPage.mymainObject.bMultiCompany = Convert.ToBoolean(row["MultiCmpReportview"]);
                        MainPage.mymainObject.bSupplierOtherDetails = Convert.ToBoolean(row["Extra"]);

                        MainPage.mymainObject.bGSTMasterEntry =  MainPage.mymainObject.bGSTMasterView = MainPage.mymainObject.bGSTMasterEditDelete =MainPage.mymainObject.bRefrenceMasterEntry =MainPage.mymainObject.bRefrenceMasterView =MainPage.mymainObject.bRefrenceMasterEditDelete =MainPage.mymainObject.bLockUnlockCustomer =MainPage.mymainObject.bSecurityChequePermission =MainPage.mymainObject.bAdminPanel =MainPage.mymainObject.bChangeSuplierDisc =MainPage.mymainObject.bChangeCustomerLimit =MainPage.mymainObject.bDashboard =MainPage.mymainObject.bBankDetailApprove =MainPage.mymainObject.bPartyWiseSP =MainPage.mymainObject.bChangeBankDetail =MainPage.mymainObject.bBranchWiseSP =MainPage.mymainObject.bChangeCustomerDetail =MainPage.mymainObject.bShowBankLedger =MainPage.mymainObject.bPartymasterRegister =MainPage.mymainObject.bGraphicalSummary = MainPage.mymainObject.bSchemeMaster =MainPage.mymainObject.bShowPartyLimit =MainPage.mymainObject.bShowAllRecord =MainPage.mymainObject.bGSTReport =MainPage.mymainObject.bShowEmailReg =MainPage.mymainObject.bShowWhatsAppReg =MainPage.mymainObject.bAddNewCustomer = false;
                        MainPage.strUserRole = Convert.ToString(row["Address"]);
                        if (table.Columns.Contains("GSTMasterEntry"))
                        {
                            if (Convert.ToString(row["GSTMasterEntry"]) != "")
                            {                                
                                MainPage.strUserRole = Convert.ToString(row["UserType"]);
                                MainPage.strUserBranchCode= Convert.ToString(row["BranchCode"]);
                                MainPage.mymainObject.bGSTMasterEntry = Convert.ToBoolean(row["GSTMasterEntry"]);
                                MainPage.mymainObject.bGSTMasterView = Convert.ToBoolean(row["GSTMasterView"]);
                                MainPage.mymainObject.bGSTMasterEditDelete = Convert.ToBoolean(row["GSTMasterEditDelete"]);
                                MainPage.mymainObject.bRefrenceMasterEntry = Convert.ToBoolean(row["RefrenceMasterEntry"]);
                                MainPage.mymainObject.bRefrenceMasterView = Convert.ToBoolean(row["RefrenceMasterView"]);
                                MainPage.mymainObject.bRefrenceMasterEditDelete = Convert.ToBoolean(row["RefrenceMasterEditDelete"]);
                                MainPage.mymainObject.bLockUnlockCustomer = Convert.ToBoolean(row["LockunLockCustomer"]);
                                MainPage.mymainObject.bSecurityChequePermission = Convert.ToBoolean(row["SecurityChequePermission"]);
                                MainPage.mymainObject.bAdminPanel = Convert.ToBoolean(row["AdminPanel"]);
                                MainPage.mymainObject.bChangeSuplierDisc = Convert.ToBoolean(row["ChangeSupplierDisc"]);
                                MainPage.mymainObject.bChangeCustomerLimit = Convert.ToBoolean(row["ChangeCustomerLimit"]);
                                MainPage.mymainObject.bDashboard = Convert.ToBoolean(row["Dashboard"]);
                                MainPage.mymainObject.bBankDetailApprove = Convert.ToBoolean(row["BankDetailApprove"]);
                                MainPage.mymainObject.bPartyWiseSP = Convert.ToBoolean(row["PartyWiseSP"]);
                                MainPage.mymainObject.bChangeBankDetail = Convert.ToBoolean(row["ChangeBankDetail"]);
                                MainPage.mymainObject.bBranchWiseSP = Convert.ToBoolean(row["BranchWiseSP"]);
                                MainPage.mymainObject.bChangeCustomerDetail = Convert.ToBoolean(row["ChangeCustomerDetail"]);
                                MainPage.mymainObject.bShowBankLedger = Convert.ToBoolean(row["ShowBankLedger"]);
                                MainPage.mymainObject.bPartymasterRegister = Convert.ToBoolean(row["PartyMasterRegister"]);

                                MainPage.mymainObject.bGraphicalSummary = Convert.ToBoolean(row["GraphicalSummary"]);
                                MainPage.mymainObject.bSchemeMaster = Convert.ToBoolean(row["SchemeMaster"]);
                                MainPage.mymainObject.bShowPartyLimit = Convert.ToBoolean(row["ShowPartyLimit"]);

                                MainPage.mymainObject.bShowAllRecord = Convert.ToBoolean(row["ShowAllRecord"]);
                                MainPage.mymainObject.bGSTReport = Convert.ToBoolean(row["GSTReport"]);
                                MainPage.mymainObject.bShowEmailReg = Convert.ToBoolean(row["ShowEmailReg"]);
                                MainPage.mymainObject.bShowWhatsAppReg = Convert.ToBoolean(row["ShowWhatsAppReg"]);
                                MainPage.mymainObject.bAddNewCustomer = Convert.ToBoolean(row["AddNewCustomer"]);
                            }
                        }                       

                        if (Convert.ToBoolean(row["PartyLedger"]))                       
                            MainPage.strPartyLeder = "YES";                        
                        else                        
                            MainPage.strPartyLeder = "NO";
                        
                        status = true;
                    }
                }
                else
                {
                    status = false;
                    MessageBox.Show("Sorry ! User Name and Password doesn't Match", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLogin.Focus();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Verifying User Name and Password in User Login", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return status;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            VerifyUser();
        }

        private bool OpenOldData()
        {
            if (txtUserType.Enabled && txtUserType.Text == "NORMAL" && !MainPage.strCompanyName.Contains("SARAOGI") && !MainPage.strCompanyName.Contains("STYLO"))
            {
                MainPage.strOldUserName = txtLogin.Text;
                MainPage.strOldPassword = txtPassword.Text;

                string strFName = MainPage.strFolderName;
                MainPage.strFolderName = MainPage.strOldData;
                MainPage.strOldData = strFName;

                MainPage.strServerPath = @"\\"+MainPage.strComputerName+@"\" + MainPage.strFolderName;
                MainPage.strOldServerPath = @"\\" + MainPage.strComputerName +@"\" + MainPage.strOldData;

                if (!System.IO.Directory.Exists(MainPage.strServerPath))
                    MainPage.strServerPath = @"\\" + MainPage.strComputerName + @"\" + MainPage.strFolderName;

                if (!System.IO.Directory.Exists(MainPage.strOldServerPath))
                    MainPage.strOldServerPath = @"\\" + MainPage.strComputerName + @"\" + MainPage.strOldData;

                this.Hide();
                SelectCompany();

                return false;
            }
            else
            {
                MainPage.strOldUserName = MainPage.strOldPassword = "";
                return true;
            }
        }

        private void SelectCompany()
        {
            try
            {
                SelectCompany sc = new SelectCompany();
                sc.ShowDialog();
                if (sc.strCompCode == "" || MainPage.strDataBaseFile == "")
                {
                    this.Close();
                }
            }
            catch
            {
            }
        }


        private void VerifyUser()
        {
            try
            {
                MainPage.mymainObject.adminPToolStripMenuItem.Visible = false;
                string strUName = txtLogin.Text, strPwd = txtPassword.Text;
                if (strUName != "")
                {
                    strUName = strUName.Replace("'", "");
                    strPwd = strPwd.Replace("'", "");

                    if (txtLogin.Text == "ADMIN" || txtLogin.Text == "SUPERADMIN")
                    {
                        string strPassword = dba.GetAdminPassword(strUName);
                        if (strPwd == strPassword)
                        {
                            if (OpenOldData())
                            {
                                MainPage.strLoginName = MainPage.strUserRole = strUName;
                                bool _bStatus = WriteLoginDetails();
                                if (_bStatus)
                                {
                                    MainPage.mymainObject.bCashAdd = MainPage.mymainObject.bCashEdit = MainPage.mymainObject.bCashView = MainPage.mymainObject.bJournalAdd = MainPage.mymainObject.bJournalEdit = MainPage.mymainObject.bJournalView = MainPage.mymainObject.bOrderAdd = MainPage.mymainObject.bOrderEdit = MainPage.mymainObject.bOrderView = MainPage.mymainObject.bFullEditControl = MainPage.mymainObject.bAddPaymentRequest = MainPage.mymainObject.bOtherExtraControl = MainPage.mymainObject.bSaleAdd = MainPage.mymainObject.bSaleEdit = MainPage.mymainObject.bSaleView = MainPage.mymainObject.bPurchaseAdd = MainPage.mymainObject.bPurchaseEdit = MainPage.mymainObject.bPurchaseView = MainPage.mymainObject.bDownloadRequest = MainPage.mymainObject.bSendRequest = MainPage.mymainObject.bChangeStatus = MainPage.mymainObject.bCourierAdd = MainPage.mymainObject.bCourierEdit = MainPage.mymainObject.bCourierView = MainPage.mymainObject.bPrivilegeAccount = true; //MainPage.mymainObject.bForwardingAdd = MainPage.mymainObject.bForwardingEdit = MainPage.mymainObject.bForwardingView =
                                    MainPage.mymainObject.bPartyMasterAdd = MainPage.mymainObject.bPartyMasterEdit = MainPage.mymainObject.bPartyMasterView = MainPage.mymainObject.bAccountMasterAdd = MainPage.mymainObject.bAccountMasterEdit = MainPage.mymainObject.bAccountMasterView = MainPage.mymainObject.bSubPartyAdd = MainPage.mymainObject.bSubPartyEdit = MainPage.mymainObject.bSubPartyView = MainPage.mymainObject.bMergingParty = MainPage.mymainObject.bDrCrNoteAdd = MainPage.mymainObject.bFASReport = MainPage.mymainObject.bOrderSlip = MainPage.mymainObject.bPrintMultiParty = MainPage.mymainObject.bCompanyInfo = MainPage.mymainObject.bLedgerReport = MainPage.mymainObject.bReportSummary = MainPage.mymainObject.bDrCrNoteEdit = MainPage.mymainObject.bSaleReport = MainPage.mymainObject.bPurchaseReport = MainPage.mymainObject.bCourierReport = MainPage.mymainObject.bDebtorCreditorReport = MainPage.mymainObject.bAccessories = MainPage.mymainObject.bSMSReport = MainPage.mymainObject.bF5Report = MainPage.mymainObject.bPurchaseSlip = MainPage.mymainObject.bBackDayEntry = MainPage.mymainObject.bDayBook = MainPage.mymainObject.bReminder = MainPage.mymainObject.bBckupRestore = MainPage.mymainObject.bMultiCompany = MainPage.mymainObject.bDrCrNoteView = MainPage.mymainObject.bSendToInternet = true;
                                    MainPage.mymainObject.adminPToolStripMenuItem.Visible = MainPage.mymainObject.bSupplierOtherDetails = true;
                                    MainPage.mymainObject.bAdjustUnadjustAccount = true;

                                    MainPage.strPartyLeder = "YES";

                                    SetStatusOnMenuControl();
                                   // if (txtLogin.Text == "ADMIN")
                                     //   MainPage.mymainObject.bAdjustUnadjustAccount = false;

                                }
                                else
                                    mainPage.Close();
                            }
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Worng Password ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        bool status = VerifyUserName(strUName, strPwd);
                        if (status && strUserName != "")
                        {
                            if (OpenOldData())
                            {
                                MainPage.strLoginName = strUName;
                                bool _bStatus = WriteLoginDetails();
                                if (_bStatus)
                                {
                                    SetStatusOnMenuControl();
                                    MainPage.mymainObject.adminPToolStripMenuItem.Visible = MainPage.mymainObject.bAdminPanel;// (MainPage.strUserRole.Contains("ADMIN") && !MainPage.strLoginName.Contains("AUDIT") && !MainPage.strLoginName.Contains("BANKING")) ? true : false;
                                }
                                else
                                    mainPage.Close();
                            }
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Login name can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLogin.Focus();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Login Button in User Login", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool WriteLoginDetails()
        {
            string strComputerName = (Environment.MachineName.Replace("'", "") + "/" + Environment.UserName.Replace("'","")).ToUpper(), strValue = "";
            string strQuery = " Select ISNULL((Select TOP 1 UPPER(ComputerName) as ComputerName from LoginDetails Where Remark='' and UserName='" + MainPage.strLoginName + "' and Date>CONVERT(date,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),100) Order by Date desc),'')+'|'+ISNULL((Select Other from SupplierOtherDetails Where Other='" + MainPage.strLoginName + "' and DATEPART(MM,DOB)=DATEPART(MM,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) and DATEPART(dd,DOB)=DATEPART(dd,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())))),'')+'|'+ISNULL((Select TOP 1 CAST(COUNT(*) as varchar)_Count from LoginDetails Where UserName='" + MainPage.strLoginName + "' and Date>CONVERT(date,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),100) Group by UserName),'') "
                            + " INSERT INTO [dbo].[LoginDetails] ([UserName],[ComputerName],[Date],[Remark]) VALUES ('" + MainPage.strLoginName + "','" + strComputerName + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'') ";

            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            strValue = Convert.ToString(objValue);
            MainPage.strBirthDayName = "";
            string[] strValues = strValue.Split('|');
            if (strValues.Length > 1)
            {
                if (strValues[0] != strComputerName && strValues[0]!="")
                {
                    DialogResult result= MessageBox.Show("Warning ! You have already logged in on " + strValues[0] + ", Are you want to logout from other computer ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(result!=DialogResult.Yes)
                    {
                        DataBaseAccess.ExecuteMyNonQuery("Delete from LoginDetails Where ID in (Select MAX(ID) from LoginDetails Where UserName='" + MainPage.strLoginName + "') ");
                        return false;
                    }
                }
                else
                {
                    if (strValues[1] == MainPage.strLoginName)
                    {
                        double dValue = dba.ConvertObjectToDouble(strValues[2]);
                        if (dValue == 0)
                        {
                            this.Hide();
                            if (MainPage.strCompanyName.Contains("SARAOGI"))
                            {
                                Happy_Birthday _obj = new Happy_Birthday(MainPage.strLoginName);
                                _obj.ShowDialog();
                            }
                        }
                        MainPage.strBirthDayName = MainPage.strLoginName;
                        //   mainPage.BackgroundImage = global::SSS.Properties.Resources.BD_Image;
                    }
                    //else
                    //    mainPage.BackgroundImage = global::SSS.Properties.Resources.Top;
                }
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            mainPage.Close();
        }

        private void UserLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                mainPage.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                VerifyUser();
            }
        }

        private void UserLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MainPage.strLoginName == "")
                {
                    MainPage.strCompanyName = "";
                    mainPage.Close();
                }
            }
            catch
            {
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtLogin.Text != "" && txtPassword.Text != "")
                {
                    DataRow[] row = table.Select(" LoginName='" + txtLogin.Text.Replace("'", "") + "' and Password='" + txtPassword.Text.Replace("'", "") + "' ");
                    if (row.Length > 0)
                    {
                        VerifyUser();
                    }
                }
            }
            catch
            {
            }
        }

        private void txtLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;

                if (Convert.ToChar(39) == pressedKey || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                    dba.ValidateSpace(sender, e);
            }
            catch
            {
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;

                if (Convert.ToChar(39) == pressedKey || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    dba.ValidateSpace(sender, e);
                }
            }
            catch
            {
            }
        }

        private void SetStatusOnMenuControl()
        {

            SoftVersionSetup();   

            //MainPage.mymainObject.bSendToInternet = false;
            MainPage.mymainObject.graphicalSummaryToolStripMenuItem.Visible = MainPage.mymainObject.showWhatsappNoDetailsToolStripMenuItem.Visible =MainPage.mymainObject.purchasedToolStripMenuItem.Visible = false;

            MainPage.mymainObject.journalRegisterToolStripMenuItem.Visible = MainPage.mymainObject.jounralToolStripMenuItem.Visible = (MainPage.mymainObject.bJournalAdd || MainPage.mymainObject.bJournalEdit || MainPage.mymainObject.bJournalView) ? true : false;
            MainPage.mymainObject.bankGuaranteeToolStripMenuItem.Visible = ((MainPage.mymainObject.bCashAdd || MainPage.mymainObject.bCashEdit || MainPage.mymainObject.bCashView) && MainPage.mymainObject.bSecurityChequePermission) ? true : false;
            MainPage.mymainObject.bankGuaranteeRegtoolStripMenuItem.Visible = MainPage.mymainObject.bSecurityChequePermission;
            MainPage.mymainObject.tcsRegisterToolStripMenuItem.Visible= MainPage.mymainObject.tcsBookToolStripMenuItem.Visible = MainPage.mymainObject.chequeDetailsToolStripMenuItem.Visible = MainPage.mymainObject.bankEntryToolStripMenuItem.Visible = MainPage.mymainObject.cashEntryToolStripMenuItem.Visible = (MainPage.mymainObject.bCashAdd || MainPage.mymainObject.bCashEdit || MainPage.mymainObject.bCashView) ? true : false;
           MainPage.mymainObject.fairOrderToolStripMenuItem.Visible= MainPage.mymainObject.orderBookToolStripMenuItem.Visible = (MainPage.mymainObject.bOrderAdd || MainPage.mymainObject.bOrderEdit || MainPage.mymainObject.bOrderView) ? true : false;
            MainPage.mymainObject.creditNoteToolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.goodscumPurchaseToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView) ? true : false;//MainPage.mymainObject.goodsRecieveToolStripMenuItem.Visible =
            MainPage.mymainObject.pincodeDistanceToolStripMenuItem.Visible= MainPage.mymainObject.retailSaleBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingSalesBookToolStripMenuItem.Visible = MainPage.mymainObject.saleServiceBookToolStripMenuItem.Visible = MainPage.mymainObject.saleReturnToolStripMenuItem.Visible = MainPage.mymainObject.salesBookToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView) ? true : false;
            MainPage.mymainObject.purchaseReturnToolStripMenuItem.Visible = MainPage.mymainObject.saleReturnToolStripMenuItem.Visible = (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;
            MainPage.mymainObject.courierRegisterToolStripMenuItem.Visible = (MainPage.mymainObject.bCourierAdd || MainPage.mymainObject.bCourierEdit || MainPage.mymainObject.bCourierView) ? true : false;
            MainPage.mymainObject.stockTransferToolStripMenuItem.Visible = MainPage.mymainObject.debitNoteToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView || MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;
            //MainPage.mymainObject.tradinglBookToolStripMenuItem.Visible =
            MainPage.mymainObject.groupCategoryMastertoolStripMenuItem.Visible = MainPage.mymainObject.addressBookToolStripMenuItem.Visible = MainPage.mymainObject.accountToolStripMenuItem.Visible = (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bPartyMasterView) ? true : false;
            MainPage.mymainObject.subPartyMasterToolStripMenuItem.Visible = (MainPage.mymainObject.bSubPartyAdd || MainPage.mymainObject.bSubPartyEdit || MainPage.mymainObject.bSubPartyView) ? true : false;
            MainPage.mymainObject.purchaeTypeMasterToolStripMenuItem.Visible = MainPage.mymainObject.saleTypeMasterToolStripMenuItem.Visible = MainPage.mymainObject.taxCategoryToolStripMenuItem.Visible = MainPage.mymainObject.partyGroupToolStripMenuItem.Visible = MainPage.mymainObject.unitMasterToolStripMenuItem.Visible = MainPage.mymainObject.marketerMasterToolStripMenuItem.Visible = MainPage.mymainObject.courierMasterToolStripMenuItem.Visible = MainPage.mymainObject.trasnportMasterToolStripMenuItem.Visible = MainPage.mymainObject.stationMasterToolStripMenuItem.Visible = MainPage.mymainObject.cartonMasterToolStripMenuItem.Visible = MainPage.mymainObject.itemGroupMasterToolStripMenuItem.Visible = (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView) ? true : false;
            MainPage.mymainObject.materialCentreToolStripMenuItem.Visible = MainPage.mymainObject.brandMasterToolStripMenuItem.Visible = MainPage.mymainObject.itemCategoryMasterToolStripMenuItem.Visible = MainPage.mymainObject.designMasterToolStripMenuItem.Visible = MainPage.mymainObject.variant1ToolStripMenuItem.Visible = MainPage.mymainObject.variant2ToolStripMenuItem.Visible = MainPage.mymainObject.variant3ToolStripMenuItem.Visible = MainPage.mymainObject.variant4ToolStripMenuItem.Visible = MainPage.mymainObject.variant5ToolStripMenuItem.Visible = (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView) ? true : false;
            MainPage.mymainObject.referenceBookToolStripMenuItem.Visible = (MainPage.mymainObject.bRefrenceMaster && (MainPage.mymainObject.bRefrenceMasterEntry || MainPage.mymainObject.bRefrenceMasterView || MainPage.mymainObject.bRefrenceMasterEditDelete)) ? true : false;

            MainPage.mymainObject.orderSlipToolStripMenuItem.Visible = MainPage.mymainObject.bOrderSlip;
            MainPage.mymainObject.stockTransferRegisterToolStripMenuItem.Visible = MainPage.mymainObject.petiDispatchRegToolStripMenuItem.Visible = MainPage.mymainObject.debitNoteRegisterToolStripMenuItem.Visible = MainPage.mymainObject.saleServiceBookRegistertoolStripMenuItem.Visible = MainPage.mymainObject.retailSalesRegisterToolStripMenuItem.Visible = MainPage.mymainObject.salesReportToolStripMenuItem1.Visible =MainPage.mymainObject.customSaleReportToolStripMenuItem.Visible= MainPage.mymainObject.bSaleReport;
            MainPage.mymainObject.creditNoteRegisterToolStripMenuItem.Visible = MainPage.mymainObject.goodsReceivedToolStripMenuItem.Visible = MainPage.mymainObject.retailPurchaseBoolRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bPurchaseReport;

            MainPage.mymainObject.courierRegisterReportToolStripMenuItem.Visible = MainPage.mymainObject.bCourierReport;
            MainPage.mymainObject.schemeDetailsToolStripMenuItem.Visible = MainPage.mymainObject.reportSummeryToolStripMenuItem.Visible = MainPage.mymainObject.bReportSummary;
            MainPage.mymainObject.customPurchaseReturnRegToolStripMenuItem.Visible=MainPage.mymainObject.customSaleReturnRegToolStripMenuItem.Visible= MainPage.mymainObject.purchaseReturnRegisterToolStripMenuItem.Visible = MainPage.mymainObject.saleReturnRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bDrCrNoteView ? true : false;
            MainPage.mymainObject.goodsReceiveAdjustmentToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bOrderEdit) ? true : false;
            MainPage.mymainObject.chequeDetailRegisterToolStripMenuItem.Visible = MainPage.mymainObject.ledgerAccountToolStripMenuItem.Visible = MainPage.mymainObject.bLedgerReport;
            MainPage.mymainObject.interestStatementToolStripMenuItem.Visible = (MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bInterestStatement) ? true : false;
            MainPage.mymainObject.partyBalanceToolStripMenuItem.Visible = (MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bPartyBalanceSlabWise) ? true : false;
            MainPage.mymainObject.dueDaysWiseToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseSlip && MainPage.mymainObject.bDueDateWisereport) ? true : false;
            MainPage.mymainObject.showBlackListTransactionLockToolStripMenuItem.Visible = (MainPage.mymainObject.bPartymasterRegister && MainPage.mymainObject.bBlackListTransectionReport) ? true : false;
            MainPage.mymainObject.salesManToolStripMenuItem.Visible = MainPage.mymainObject.profitDetailsToolStripMenuItem.Visible = MainPage.mymainObject.depreciationChartToolStripMenuItem.Visible = MainPage.mymainObject.profitandLossToolStripMenuItem.Visible = MainPage.mymainObject.balanceSheetToolStripMenuItem.Visible = MainPage.mymainObject.trialBalanceToolStripMenuItem.Visible = MainPage.mymainObject.bFASReport;
             MainPage.mymainObject.amendedBillRegToolStripMenuItem.Visible = (MainPage.mymainObject.bAmendedBillReport && MainPage.mymainObject.bGSTReport) ? true : false;
            MainPage.mymainObject.gSTReportToolStripMenuItem.Visible = MainPage.mymainObject.gSTR2ToolStripMenuItem.Visible = MainPage.mymainObject.bGSTReport;
            MainPage.mymainObject.printMultiPartyLedgerToolStripMenuItem.Visible = MainPage.mymainObject.bPrintMultiParty;
            MainPage.mymainObject.purchaseSlipToolStripMenuItem.Visible = MainPage.mymainObject.paymentRequestToolStripMenuItem.Visible = MainPage.mymainObject.bPurchaseSlip;
            MainPage.mymainObject.showWhatsappNoDetailsToolStripMenuItem.Visible = MainPage.mymainObject.showSMSReportToolStripMenuItem.Visible = MainPage.mymainObject.bShowWhatsAppReg;
            MainPage.mymainObject.showEmailRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bShowEmailReg;
            MainPage.mymainObject.variantDetailsToolStripMenuItem.Visible = MainPage.mymainObject.bAccessories;
            MainPage.mymainObject.multitoolStripMenuItem.Visible = MainPage.mymainObject.bMultiCompany;
            MainPage.mymainObject.showAmountLimitToolStripMenuItem.Visible = MainPage.mymainObject.bShowPartyLimit;
            MainPage.mymainObject.adjustMultiCompanyLedgerToolStripMenuItem.Visible = MainPage.mymainObject.multiLedgerAccountToolStripMenuItem.Visible = (MainPage.mymainObject.bMultiCompany && MainPage.mymainObject.bLedgerReport) ? true : false;
            MainPage.mymainObject.multiGeneralInterestToolStripMenuItem.Visible = MainPage.mymainObject.lnkGInerest_A.Visible = (MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bInterestStatement && MainPage.mymainObject.bMultiCompany) ? true : false;
            MainPage.mymainObject.multiPurchaseOutstandingToolStripMenuItem.Visible = (MainPage.mymainObject.bMultiCompany && MainPage.mymainObject.bPurchaseSlip) ? true : false;
            MainPage.mymainObject.multiCreditorsDebitorsAcToolStripMenuItem.Visible = (MainPage.mymainObject.bMultiCompany && MainPage.mymainObject.bDebtorCreditorReport) ? true : false;
            MainPage.mymainObject.dashboardToolStripMenuItem.Visible = MainPage.mymainObject.lnkDashBoard_R.Visible=MainPage.mymainObject.lnkDashBoard_A.Visible= MainPage.mymainObject.bDashboard;
            MainPage.mymainObject.removalReasonToolStripMenuItem.Visible = MainPage.mymainObject.editLogReportToolStripMenuItem.Visible = MainPage.mymainObject.lnkEditLogReport_R.Visible = (MainPage.mymainObject.bAccessories && MainPage.mymainObject.bRemovalReason) ? true : false;
            MainPage.mymainObject.profitMargintoolStripMenuItem.Visible = MainPage.mymainObject.accessories.Visible =  MainPage.mymainObject.bAccessories;
            MainPage.mymainObject.sendDataToInternetToolStripMenuItem.Visible = MainPage.mymainObject.bSendToInternet;
            MainPage.mymainObject.backupRestoreToolStripMenuItem1.Visible = MainPage.mymainObject.bBckupRestore;
            MainPage.mymainObject.partyMergeToolStripMenuItem.Visible = MainPage.mymainObject.bMergingParty;

            MainPage.mymainObject.companyDetailToolStripMenuItem.Visible = MainPage.mymainObject.settingToolStripMenuItem.Visible = MainPage.mymainObject.companyMasterToolStripMenuItem.Visible = MainPage.mymainObject.bCompanyInfo;
            MainPage.mymainObject.templateSettingToolStripMenuItem.Visible= MainPage.mymainObject.importRetailPurchaseBillToolStripMenuItem.Visible = MainPage.mymainObject.importPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.importExcelSheetToolStripMenuItem.Visible = (MainPage.mymainObject.bOtherExtraControl && MainPage.mymainObject.bImport)?true:false;
            MainPage.mymainObject.biltyAndWayBilltoolStripMenuItem.Visible = MainPage.mymainObject.bSaleEdit;
            MainPage.mymainObject.stockAuditToolStripMenuItem.Visible = (MainPage.mymainObject.bAccountMasterAdd && MainPage.strUserRole.Contains("ADMIN") && MainPage.mymainObject.bSaleReport && MainPage.mymainObject.bPurchaseReport) ? true : false;
            MainPage.mymainObject.customPurchaseReportToolStripMenuItem.Visible = MainPage.mymainObject.stockAgeingToolStripMenuItem.Visible = MainPage.mymainObject.stockMasterToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleReport && MainPage.mymainObject.bPurchaseReport) ? true : false;
            MainPage.mymainObject.creditorsOrDebitorsAccountToolStripMenuItem.Visible = MainPage.mymainObject.bDebtorCreditorReport;
            MainPage.mymainObject.graphicalSummaryToolStripMenuItem.Visible = MainPage.mymainObject.bGraphicalSummary;
            MainPage.mymainObject.partyRecordToolStripMenuItem.Visible = MainPage.mymainObject.lnk_AccountMaster_A.Visible=MainPage.mymainObject.lnk_AccountMaster_R.Visible= MainPage.mymainObject.bPartymasterRegister;
            MainPage.mymainObject.partyWiseSalePurchaseToolStripMenuItem.Visible = MainPage.mymainObject.bPartyWiseSP;
            MainPage.mymainObject.showCurrentBalanceToolStripMenuItem.Visible = MainPage.mymainObject.bShowPartyLimit;
            MainPage.mymainObject.branchesSalesDetailToolStripMenuItem.Visible = MainPage.mymainObject.bBranchWiseSP;
            MainPage.mymainObject.schemeMasterToolStripMenuItem.Visible = MainPage.mymainObject.schemeDetailMasterToolStripMenuItem.Visible = MainPage.mymainObject.graceMasterToolStripMenuItem.Visible = MainPage.mymainObject.bSchemeMaster;
            MainPage.mymainObject.designMasterRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bAccountMasterView;
            MainPage.mymainObject.dayBookToolStripMenuItem.Visible = MainPage.mymainObject.bDayBook = (MainPage.mymainObject.bSaleReport && MainPage.mymainObject.bPurchaseReport && MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bDayBookRegister) ? true : false;

            MainPage.mymainObject.stockDrillDownReportToolStripMenuItem.Visible = (MainPage.mymainObject.bDrillDownReport && MainPage.mymainObject.bSaleReport && MainPage.mymainObject.bPurchaseReport) ? true : false;
            MainPage.mymainObject.saleDrillDownReportToolStripMenuItem.Visible= (MainPage.mymainObject.bDrillDownReport && MainPage.mymainObject.bSaleReport) ? true : false;

            if (txtLogin.Text == "A")
                 MainPage.mymainObject.purchasedToolStripMenuItem.Visible = true;              

            if (MainPage.strUserRole.Contains("ADMIN"))                         
                MainPage.mymainObject.birthdayAnniversaryToolStripMenuItem.Visible = MainPage.mymainObject.pringtingSettingToolStripMenuItem.Visible = MainPage.mymainObject.layoutdesignToolStripMenuItem.Visible = true;            
            else
                MainPage.mymainObject.birthdayAnniversaryToolStripMenuItem.Visible = MainPage.mymainObject.pringtingSettingToolStripMenuItem.Visible = MainPage.mymainObject.layoutdesignToolStripMenuItem.Visible = false;
            
            DataBaseAccess.SetCategoryData();
            if (!MainPage.mymainObject.bAccountMasterAdd && !MainPage.mymainObject.bAccountMasterEdit && !MainPage.mymainObject.bAccountMasterView)
                MainPage.mymainObject.variant1ToolStripMenuItem.Visible = MainPage.mymainObject.variant2ToolStripMenuItem.Visible = MainPage.mymainObject.variant3ToolStripMenuItem.Visible = MainPage.mymainObject.variant4ToolStripMenuItem.Visible = MainPage.mymainObject.variant5ToolStripMenuItem.Visible = false;


            if (!MainPage.strCompanyName.Contains("SARAOGI") && !MainPage.strCompanyName.Contains("STYLO"))
            {
                MainPage.mymainObject.graceMasterToolStripMenuItem.Visible = MainPage.mymainObject.schemeMasterToolStripMenuItem.Visible = MainPage.mymainObject.schemeDetailMasterToolStripMenuItem.Visible = MainPage.mymainObject.supplierMappingToolStripMenuItem.Visible = MainPage.mymainObject.supplierMappedRToolStripMenuItem.Visible = MainPage.mymainObject.importRetailPurchaseBillToolStripMenuItem.Visible = MainPage.mymainObject.importPurchaseBookToolStripMenuItem.Visible = false;

                MainPage.mymainObject.marketerMasterToolStripMenuItem.Visible = MainPage.mymainObject.purchaseReturnToolStripMenuItem.Visible = MainPage.mymainObject.saleReturnToolStripMenuItem.Visible = MainPage.mymainObject.salesBookToolStripMenuItem.Visible = MainPage.mymainObject.goodscumPurchaseToolStripMenuItem.Visible = MainPage.mymainObject.goodsReceiveAdjustmentToolStripMenuItem.Visible = MainPage.mymainObject.biltyAndWayBilltoolStripMenuItem.Visible = MainPage.mymainObject.schemeDetailsToolStripMenuItem.Visible = false;
                MainPage.mymainObject.goodsReceivedToolStripMenuItem.Visible = MainPage.mymainObject.salesReportToolStripMenuItem1.Visible = MainPage.mymainObject.purchasedToolStripMenuItem.Visible = MainPage.mymainObject.reportSummeryToolStripMenuItem.Visible = false;
                MainPage.mymainObject.orderBookToolStripMenuItem.Visible = MainPage.mymainObject.orderSlipToolStripMenuItem.Visible = MainPage.mymainObject.petiDispatchRegToolStripMenuItem.Visible = false;
                MainPage.mymainObject.referenceBookToolStripMenuItem.Visible = (MainPage.mymainObject.bRefrenceMasterEntry || MainPage.mymainObject.bRefrenceMasterView || MainPage.mymainObject.bRefrenceMasterEditDelete) ? true : false;
                MainPage.mymainObject.paymentRequestToolStripMenuItem.Visible = false;
                if (MainPage.strFolderName == "DEMO" || MainPage.strOldData == "DEMO")
                    MainPage.mymainObject.interestStatementToolStripMenuItem.Visible = MainPage.mymainObject.multiGeneralInterestToolStripMenuItem.Visible = true;
            }
            else
            {
                MainPage.mymainObject.supplierMappingToolStripMenuItem.Visible = MainPage.mymainObject.supplierMappedRToolStripMenuItem.Visible = (MainPage.strServerPath.Contains("NET") && (MainPage.strUserRole.Contains("ADMIN") || MainPage.strUserRole.Contains("MANAGER"))) ? true : false;
                MainPage.mymainObject.stockTransferToolStripMenuItem.Visible = MainPage.mymainObject.stockTransferRegisterToolStripMenuItem.Visible = false;
                if (!MainPage.strCompanyName.Contains("PTN"))
                    MainPage.mymainObject.customPurchaseReportToolStripMenuItem.Visible = MainPage.mymainObject.customSaleReportToolStripMenuItem.Visible = false;
            }

            if (MainPage.strSoftwareType.Contains("RETAIL"))
            {
                MainPage.mymainObject.advanceRegisterToolStripMenuItem.Visible = MainPage.mymainObject.advanceBookToolStripMenuItem.Visible = (MainPage.mymainObject.bCashAdd || MainPage.mymainObject.bCashEdit || MainPage.mymainObject.bCashView) ? true : false;
                MainPage.mymainObject.retailPurchaseBookToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView) ? true : false;
                MainPage.mymainObject.retailSaleBookToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView) ? true : false;
                MainPage.mymainObject.retailPurchaseReturnToolStripMenuItem.Visible = MainPage.mymainObject.retailSaleReturntoolStripMenuItem.Visible = (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;

                MainPage.mymainObject.alterationslipRegisterToolStripMenuItem.Visible = MainPage.mymainObject.alterationSlipToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView) ? true : false;              
                MainPage.mymainObject.subPartyMasterToolStripMenuItem.Visible = MainPage.mymainObject.tradingSalesBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingSaleReturnToolStripMenuItem.Visible = false;
                MainPage.mymainObject.cartonMasterToolStripMenuItem.Visible = false;
                if (MainPage.strUserRole.Contains("ADMIN") || txtLogin.Text.Contains("ADMIN"))
                    MainPage.mymainObject.incentiveToolStripMenuItem.Visible = MainPage.mymainObject.discountDetailsToolStripMenuItem.Visible = true;
            }
            else
            {
                MainPage.mymainObject.tradingSalesBookToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView) ? true : false;
                MainPage.mymainObject.tradingSaleReturnToolStripMenuItem.Visible = (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;
                if (MainPage.strCompanyName.Contains("SARAOGI"))
                {
                    MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView) ? true : false;
                    MainPage.mymainObject.retailPurchaseBookToolStripMenuItem.Visible =   MainPage.mymainObject.retailSaleReturntoolStripMenuItem.Visible = MainPage.mymainObject.retailPurchaseReturnToolStripMenuItem.Visible = false;
                    MainPage.mymainObject.tradingSaleReturnToolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseReturnToolStripMenuItem.Visible = (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;
                }
                else
                {
                    MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingSaleReturnToolStripMenuItem.Visible = false;
                    MainPage.mymainObject.stockTransferRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bSaleReport;
                    MainPage.mymainObject.retailPurchaseBookToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView) ? true : false; 
                    MainPage.mymainObject.retailSaleReturntoolStripMenuItem.Visible =MainPage.mymainObject.retailPurchaseReturnToolStripMenuItem.Visible=(MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView) ? true : false;
                }
                MainPage.mymainObject.alterationslipRegisterToolStripMenuItem.Visible = MainPage.mymainObject.alterationSlipToolStripMenuItem.Visible = false;
                MainPage.mymainObject.retailSaleBookToolStripMenuItem.Visible =  false;
               
            }

            if (MainPage.strSoftwareType == "TRADING" && MainPage._bCustomPurchase)
            {
                MainPage.mymainObject.mergePurchaseBookToolStripMenuItem.Visible = (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView) ? true : false;
                MainPage.mymainObject.mergeSalesBookToolStripMenuItem.Visible = (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView) ? true : false;
                MainPage.mymainObject.tradingSalesBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.retailPurchaseBookToolStripMenuItem.Visible = false;

               // MainPage.mymainObject.graphicalSummaryToolStripMenuItem.Visible = MainPage.mymainObject.birthdayAnniversaryToolStripMenuItem.Visible = MainPage.mymainObject.partyWiseSalePurchaseToolStripMenuItem.Visible = MainPage.mymainObject.showCurrentBalanceToolStripMenuItem.Visible = MainPage.mymainObject.branchesSalesDetailToolStripMenuItem.Visible = MainPage.mymainObject.partyRecordToolStripMenuItem.Visible = true;
            }

            if (MainPage.strSoftwareType == "RES_RETAIL")
            {
                MainPage.mymainObject.chequeDetailsToolStripMenuItem.Visible = MainPage.mymainObject.bankGuaranteeToolStripMenuItem.Visible = MainPage.mymainObject.tcsBookToolStripMenuItem.Visible = MainPage.mymainObject.advanceBookToolStripMenuItem.Visible = MainPage.mymainObject.orderBookToolStripMenuItem.Visible = MainPage.mymainObject.goodscumPurchaseToolStripMenuItem.Visible = MainPage.mymainObject.salesBookToolStripMenuItem.Visible = MainPage.mymainObject.saleReturnToolStripMenuItem.Visible = MainPage.mymainObject.purchaseReturnToolStripMenuItem.Visible = MainPage.mymainObject.debitNoteToolStripMenuItem.Visible = MainPage.mymainObject.creditNoteToolStripMenuItem.Visible = MainPage.mymainObject.courierRegisterToolStripMenuItem.Visible = MainPage.mymainObject.goodsReceiveAdjustmentToolStripMenuItem.Visible = MainPage.mymainObject.biltyAndWayBilltoolStripMenuItem.Visible = MainPage.mymainObject.purchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingSalesBookToolStripMenuItem.Visible = MainPage.mymainObject.retailSaleBookToolStripMenuItem.Visible = MainPage.mymainObject.tradingSaleReturnToolStripMenuItem.Visible = MainPage.mymainObject.retailSaleReturntoolStripMenuItem.Visible = MainPage.mymainObject.tradingPurchaseReturnToolStripMenuItem.Visible = MainPage.mymainObject.mergePurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.mergeSalesBookToolStripMenuItem.Visible = MainPage.mymainObject.alterationSlipToolStripMenuItem.Visible = MainPage.mymainObject.stockTransferToolStripMenuItem.Visible = MainPage.mymainObject.subPartyMasterToolStripMenuItem.Visible = MainPage.mymainObject.addressBookToolStripMenuItem.Visible = MainPage.mymainObject.referenceBookToolStripMenuItem.Visible = MainPage.mymainObject.cartonMasterToolStripMenuItem.Visible = MainPage.mymainObject.courierMasterToolStripMenuItem.Visible = MainPage.mymainObject.marketerMasterToolStripMenuItem.Visible = MainPage.mymainObject.incentiveToolStripMenuItem.Visible = MainPage.mymainObject.discountDetailsToolStripMenuItem.Visible = MainPage.mymainObject.materialCentreToolStripMenuItem.Visible = MainPage.mymainObject.branchesSalesDetailToolStripMenuItem.Visible = MainPage.mymainObject.goodsReceivedToolStripMenuItem.Visible = MainPage.mymainObject.orderSlipToolStripMenuItem.Visible = MainPage.mymainObject.salesReportToolStripMenuItem1.Visible = MainPage.mymainObject.saleReturnRegisterToolStripMenuItem.Visible = MainPage.mymainObject.customSaleReturnRegToolStripMenuItem.Visible = MainPage.mymainObject.creditNoteRegisterToolStripMenuItem.Visible = MainPage.mymainObject.debitNoteRegisterToolStripMenuItem.Visible = MainPage.mymainObject.reportSummeryToolStripMenuItem.Visible = MainPage.mymainObject.courierRegisterReportToolStripMenuItem.Visible = MainPage.mymainObject.petiDispatchRegToolStripMenuItem.Visible = MainPage.mymainObject.schemeDetailsToolStripMenuItem.Visible = MainPage.mymainObject.amendedBillRegToolStripMenuItem.Visible = MainPage.mymainObject.alterationslipRegisterToolStripMenuItem.Visible = MainPage.mymainObject.stockTransferRegisterToolStripMenuItem.Visible = MainPage.mymainObject.tcsRegisterToolStripMenuItem.Visible = MainPage.mymainObject.advanceRegisterToolStripMenuItem.Visible = MainPage.mymainObject.pincodeDistanceToolStripMenuItem.Visible = MainPage.mymainObject.interestStatementToolStripMenuItem.Visible = MainPage.mymainObject.profitDetailsToolStripMenuItem.Visible = MainPage.mymainObject.chequeDetailRegisterToolStripMenuItem.Visible = MainPage.mymainObject.bankGuaranteeRegtoolStripMenuItem.Visible = MainPage.mymainObject.purchaseSlipToolStripMenuItem.Visible = MainPage.mymainObject.paymentRequestToolStripMenuItem.Visible = MainPage.mymainObject.showAmountLimitToolStripMenuItem.Visible = MainPage.mymainObject.showBlackListTransactionLockToolStripMenuItem.Visible = MainPage.mymainObject.graphicalSummaryToolStripMenuItem.Visible = MainPage.mymainObject.importExcelSheetToolStripMenuItem.Visible = MainPage.mymainObject.importRetailPurchaseBillToolStripMenuItem.Visible = MainPage.mymainObject.importPurchaseBookToolStripMenuItem.Visible = MainPage.mymainObject.birthdayAnniversaryToolStripMenuItem.Visible = MainPage.mymainObject.profitMargintoolStripMenuItem.Visible = MainPage.mymainObject.graceMasterToolStripMenuItem.Visible = MainPage.mymainObject.schemeMasterToolStripMenuItem.Visible = MainPage.mymainObject.schemeDetailMasterToolStripMenuItem.Visible = MainPage.mymainObject.supplierMappingToolStripMenuItem.Visible = MainPage.mymainObject.supplierMappedRToolStripMenuItem.Visible = MainPage.mymainObject.retailSalesRegisterToolStripMenuItem.Visible= MainPage.mymainObject.brandMasterToolStripMenuItem.Visible= false;
                MainPage.mymainObject.resInvoiceToolStripMenuItem.Visible = true;
                MainPage.mymainObject.designMasterToolStripMenuItem.Text = "&Item Master";
            }

            //stockTransferRegisterToolStripMenuItem
        }

        private void txtUserType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("USERTYPE", "SEARCH USER TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtUserType.Text = objSearch.strSelectedData;
                    }
                }
            }
            catch { }
        }
    }
}
