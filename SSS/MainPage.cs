using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Reflection;
using DBConnection;

namespace SSS
{
    public partial class MainPage : Form                       
    {
        DataBaseAccess dba;
        public static MainPage mymainObject = null;
        public static string strProductType = "DEMO_RETAIL", strProductVersion = "27.5.21", strFolderName = "DEMO", strOldData = "LOCAL", strSoftwareType = "RETAIL", strPlanType = "DIAMOND";
       public static LoadingForm objLoading = new LoadingForm();

        public static string strSytemType = "SINGLE", strUserBranchCode = "", strBirthDayName = "", strUserRole = "", strDataBaseFile = "", strPreviousDataBase = "", strCompanyName = "", strLoginName = "", strPartyLeder = "No", strCurrentDate = "", strServerPath = "", strOldServerPath = "", strLocation = "", strGRCompanyName = "", strOnlineDataBaseName = "", strServerDataBaseName = "", strSendBalanceInSMS = "NO", strSenderEmailID = "", strSenderPassword = "", strSMTPServer = "", strComputerName = "", strPrintComapanyName = "", strSenderID = "", strBranchCode = "", strSalesReportTitle = "", strTitleofDocument = "", strSubTitle = "", strJurisdiction = "", strGeneratedBy = "", strDeclaration = "", strTermsofDelivery = "", strNoofCopy = "1", strCompanyStateName = "", strUpdateQuery = "", strVersionUpdateQuery = "", strLiveDataBaseIP = "", StrCategory1 = "", StrCategory2 = "", StrCategory3 = "", StrCategory4 = "", StrCategory5 = "", strLiveDBPassword, strDBPwd, strNetImagePath = "", strHttpPath = "", strFTPPath = "", strFTPUserName = "", strFTPPassword = "", strHeadOfficeBankAccountNo = "777705000285", strSSSDataBaseIP, strDataBaseIP, strLocalDBIP = "", strLocalDBPwd = "", __strLoginName = "", strOldUserName = "", strOldPassword = "", strSMSURL = "", strSMSUser = "", strSMSPassword = "", strMessageType = "", strClientName = "", strPrintLayout = "", strStockAsPer = "", strBarCodingType = "", strMonthLockPeriod = "", strMonthLockDate = "", strSaleRtnDeclaration = "", strSaleServDeclaration = "", strPurchaseRtnDeclaration = "";
        public static double dTCSPer = 0.1, dPackingDhara = 0, dFreightDhara = 0, dTaxDhara = 0, dPackingAmount = 0, dPostageAmount = 0, dFixedMargin = 0, dItemwiseMargin = 0, dPurchaseBillMargin = 0, dBrandwiseMargin = 0;
        public static int _SMTPPORTNo = 0, iNCopyPurchase = 1, iNCopySaleRtn = 1, iNCopyPurRtn = 1, iNCopyCash = 1, iNCopyBank = 1, iNCopyJournal = 1, iNCopySServ = 1, iNCopyStockTrans=1;
        public static byte[] _headerImage = null, _signatureImage = null, _brandLogo = null;
        public static SqlConnection con;
        public static DateTime startFinDate, endFinDate, currentDate, multiQSDate, multiQEDate;
        public static CultureInfo indianCurancy = new CultureInfo("hi-IN");
        DataTable dtPartyBalanceTable = null, dtPartyName = null, dtReminder = null;
        public bool bCashAdd = false, bCashEdit = false, bCashView = false, bJournalAdd = false, bJournalEdit = false, bJournalView = false, bOrderAdd = false, bOrderEdit = false, bOrderView = false, bSaleAdd = false, bSaleEdit = false, bSaleView = false, bPurchaseAdd = false, bPurchaseEdit = false, bPurchaseView = false, bCourierAdd = false, bCourierEdit = false, bCourierView = false, bSupplierOtherDetails = false, bDownloadRequest = false, bSendRequest = false, bChangeStatus = false, bFullEditControl = false, bAddPaymentRequest = false, bOtherExtraControl = false, bCategoryView = false, bDrCrNoteAdd = false, bDrCrNoteView = false, bDrCrNoteEdit = false, bPrivilegeAccount = false;
        public bool bPartyMasterAdd = false, bPartyMasterEdit = false, bPartyMasterView = false, bAccountMasterAdd = false, bAccountMasterEdit = false, bAccountMasterView = false, bSubPartyAdd = false, bSubPartyEdit = false, bSubPartyView = false, bMergingParty = false, bFASReport = false, bOrderSlip = false, bPrintMultiParty = false, bCompanyInfo = false, bLedgerReport = false, bReportSummary = false, bSaleReport = false, bPurchaseReport = false, bCourierReport = false, bDebtorCreditorReport = false, bAccessories = false, bSMSReport = false, bF5Report = false, bPurchaseSlip = false, bAdjustUnadjustAccount = false, bBackDayEntry = false, bDayBook = false, bReminder = false, bBckupRestore = false, bMultiCompany = false, bSendToInternet = false;
        public bool bGSTMasterEntry = false, bGSTMasterView = false, bGSTMasterEditDelete = false, bRefrenceMasterEntry = false, bRefrenceMasterView = false, bRefrenceMasterEditDelete = false, bLockUnlockCustomer = false, bSecurityChequePermission = false, bAdminPanel = false, bChangeSuplierDisc = false, bChangeCustomerLimit = false, bDashboard = false, bBankDetailApprove = false, bPartyWiseSP = false, bChangeBankDetail = false, bBranchWiseSP = false, bChangeCustomerDetail = false, bShowBankLedger = false, bPartymasterRegister = false, bGraphicalSummary = false, bSchemeMaster = false, bShowPartyLimit = false, bShowAllRecord = false, bGSTReport = false, bShowEmailReg = false, bShowWhatsAppReg = false, bAddNewCustomer = false;
        public static bool bDBOnNet = false, bArticlewiseOpening = false, _PrintWithDialog = true, _bItemMirroring = false, _bCustomPurchase = false, _localonLocal = false, bHSNWisePurchase = false, pCompanyName = false, pCompanyAddress = false, pBuyerName = false, pBuyerAddress = false, pCompTaxRegNo = false, pBuyerTaxRegNo = false, pOrderDetails = false, pSuppDesign = false, pManfDesign = false, pQty = false, pRate = false, pAmount = false, pAgentName = false, pCategory1 = false, pCategory2 = false, pCategory3 = false, pCategory4 = false, pCategory5 = false, pTaxPer = false, _bTaxStatus = true, _bPaidStatus = false, _bFixedMargin = false, _bItemWiseMargin = false, _bPurchaseBillWiseMargin = false, _bBrandWiseMargin = false, _bDesignMasterMargin = true, _bBarCodeStatus = false, _TaxStatusPurchase = true, bPurchaseSetWise = true;
        public bool bMultiBranch=true, bExport = true,bImport=true, bBarcodePrint = true, bDigitallySignedInvoice = false, bE_InvoicingFacility = false, bBranchWiseBalanceSheet = false, bCreditLimitmanagement = true, bLoyalityCardSystem = false, bPayRoll = false, bRemovalReason = true, bInterestStatement = true, bDueDateWisereport = true, bPartyBalanceSlabWise = true, bBlackListTransectionReport = true, bUnmovedItemReport = false, bDayBookRegister = true, bAmendedBillReport = true, bRefrenceMaster = true, bBankWayBill = false,bDrillDownReport=false;
        int _escCount = 0;
        protected internal MenuItem fairOrderToolStripMenuItem = new MenuItem();

        public MainPage()
        {
            try
            {
                AddAsseblies();
                InitializeComponent();
                dba = new DataBaseAccess();
                con = new SqlConnection();
                SetDetails();

               bDBOnNet = true;
             strDataBaseIP = strLiveDataBaseIP = "103.21.58.193";

                _bItemMirroring = true;
                _bCustomPurchase = true;
                bMultiBranch = false;
                //// _localonLocal = true;

                SetServerPath();

                mymainObject = this;
                myTimer.Interval = 1000;
                myTimer.Enabled = true;

                if (CheckVersionStatus())
                    SelectCompany();

                if (strDataBaseFile == "" || strLoginName == "")
                {
                    this.BeginInvoke(new MethodInvoker(Close));
                    this.Close();
                }
                else
                    SetCustomerData();

                grpPrint.Left = this.Width - 150;
                rdoWith.Checked = _PrintWithDialog;
                rdoWithout.Checked = !_PrintWithDialog;
            }
            catch (Exception ex)
            {
                this.Close();
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }

        private void SetDetails()
        {
            DBCon.LiveDBUserName = "sqlserver";
            strLiveDBPassword = DBCon.LiveDBUserPassword;
            strDBPwd = DBCon.DBUserPassword;
            strSSSDataBaseIP = DBCon.LiveDBSSSIP;
            strDataBaseIP = DBCon.LiveDBIP;
        }

        private void SetCustomerData()
        {
            if (strCompanyName.Contains("FIVE") || strCompanyName.Contains("SHIKHAR") || strCompanyName.ToUpper().Contains("MART"))
            {
                strClientName = "LOTUS";
            }
            else if (strSoftwareType == "RETAIL")
                bArticlewiseOpening = true;

            if (strLiveDataBaseIP == "148.66.132.75" || strLiveDataBaseIP == "103.21.58.193")
                DBCon.LiveDBUserName = "";

            if (strSoftwareType == "AGENT")
            {
                bMultiBranch = true;
                strFTPPath = "ftp://pdffiles.ssspltd.com";
                strHttpPath = "http://pdffiles.ssspltd.com";
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

        private void SetServerPath()
        {           
            strSenderID = "SSSGST";

            strComputerName = "CYBER";
            if (strFolderName == "DEMO" || strSytemType == "SINGLE")
                strComputerName = Environment.MachineName;

            strServerPath = "\\\\" + strComputerName + "\\" + strFolderName;
            strOldServerPath = "\\\\" + strComputerName + "\\" + strOldData;


            if (!Directory.Exists(strServerPath) && strSoftwareType=="RETAIL")
            {
                strComputerName = Environment.MachineName;
                strServerPath = "\\\\" + strComputerName + "\\" + strFolderName;
            }           

            if (!Directory.Exists(strServerPath))
            {
                strComputerName = "GOENKASSERVER";
                strServerPath = @"\\\\GOENKASSERVER\\" + strFolderName;
               // bArticlewiseOpening = true;
                //_bItemMirroring = false;
            }

            if (!Directory.Exists(strServerPath))
            {
                strComputerName = "SERVER";
                strServerPath = @"\\\\SERVER\\" + strFolderName;
            }

            if (!Directory.Exists(strServerPath) || strProductType == "SYBER")
            {
                strComputerName = Environment.MachineName;
                strServerPath = "\\\\" + strComputerName + "\\" + strFolderName;
                strOldServerPath = "\\\\" + strComputerName + "\\" + strOldData;
            }


            //strComputerName = "192.168.1.56";
            //if (!Directory.Exists(strServerPath))
            //    strServerPath = @"\\192.168.1.56\" + strFolderName;

            // strServerPath = @"\\CYBER\NC";          
            //    strServerPath = Application.StartupPath;


            //if (!Directory.Exists(strServerPath))
            //{
            //    strComputerName = ".";
            //    strServerPath = Application.StartupPath;
            //    strOldServerPath = Application.StartupPath + "\\" + strOldData;
            //}         

            strBranchCode = "DL";
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            var workingArea = Screen.FromHandle(Handle).WorkingArea;
            this.MaximizedBounds = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
            this.WindowState = FormWindowState.Maximized;

            startFinDate = multiQSDate = DateTime.Now.Date;
            endFinDate = multiQEDate = DateTime.Now.Date;
            int pageWidth = this.Width, pageHeight = this.Height;
            if (pageHeight > 0 && pageWidth > 0)
            {
                ledgerPanel.Location = partyPanel.Location = new Point(pageWidth - (482+176), 30);
                ledgerPanel.Height = partyPanel.Height = pageHeight - 35;
                datePanel.Location = new Point((pageWidth / 2) - 200, (pageHeight / 2) - 90);
                restorePanel.Location = new Point((pageWidth / 2) - 320, (pageHeight / 2) - 180);
            }
            if (pageWidth > 1300)
            {
                if (MainPage.strSoftwareType == "AGENT")
                    pnlShortCutAgent.Visible = true;
                else
                    pnlShortCut.Visible = true;
            }

            if (MainPage.strSoftwareType == "RETAIL")
                marketerMasterToolStripMenuItem.Text = "Salesman Master";
            else
                marketerMasterToolStripMenuItem.Text = "&Marketer Master";

        }

        public static void OpenConnection()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                if (MainPage.strDataBaseFile != MainPage.con.Database)
                {
                    ChangeDataBase(strDataBaseFile);
                }
            }
            catch
            {
            }
        }


        private void AddAsseblies()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
        public static bool ChangeDataBase(string strDatabaseCode)
        {
            try
            {
                if (strDatabaseCode != "")
                {
                    if (bDBOnNet || MainPage.strFolderName == "DEMO" || MainPage.strOldData == "DEMO" || MainPage.strProductType == "RES_RETAIL")
                    {
                        if (con.Database != strDatabaseCode && strDatabaseCode!="")
                        {
                            con.Close();
                            con.ConnectionString = "Data Source=" + MainPage.strDataBaseIP + ";Initial Catalog=" + strDatabaseCode + "; User Id=" + strDatabaseCode + ";Password=" + MainPage.strLiveDBPassword + ";";
                            con.Open();
                        }
                        else if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                    }
                    else
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        if (con.Database != strDatabaseCode)
                            con.ChangeDatabase(strDatabaseCode);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckVersionStatus()
        {
            bool _bStatus = true;
            try
            {
                if (IsConnectedToInternet())
                {
                    _bStatus = DataBaseAccess.CheckSoftwareVersion();
                    if (!_bStatus)
                    {
                        Application.Exit();
                        this.BeginInvoke(new MethodInvoker(Close));
                    }
                }
            }
            catch { }
            return _bStatus;
        }

        public static bool IsConnectedToInternet()
        {
            string host = "www.google.com";
           // bool result = true;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 5000);
               // if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { return false; }
           // return result;
        }

        private void GetMSChartFromServer()
        {
            try
            {
                string strExePath = @"C:\Program Files (x86)\Microsoft Chart Controls\Assemblies\System.Windows.Forms.DataVisualization.dll";
                if (!File.Exists(strExePath))
                {
                   string strPath= DataBaseAccess.DownloadChartSetup();
                    System.Diagnostics.Process.Start(strPath);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Downloading config file", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public static void CloseConnection()
        {
            if (con != null)
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
     
        //private void ValidateUser()
        //{
        //    try
        //    {
        //        string[] strData = dba.IncreaseCounter();

        //        int counter = Int32.Parse(strData[0]);
        //        if (counter > 30 && strData[1] != "PAID")
        //        {
        //            MessageBox.Show("Validity has been Expired ! Please Purchase Lincense version of this Software", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            this.Close();
        //        }
        //    }
        //    catch
        //    {
        //        this.Close();
        //    }
        //}

        private void logoutToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int _count = DataBaseAccess.LogoutFromThisComputerName();
            if (_count > 0)
            {
                CloseAllOpenedForms();
                CloseFormsOnLogout();
                strUserRole = strLoginName = "";
                UserLogin ul = new UserLogin();
                ul.ShowDialog();
            }
        }

       private void addToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CompanyMaster newComp = new CompanyMaster();
                newComp.MdiParent = this;
                newComp.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of New Company in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CloseAllOpenForms()
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    //foreach (Form childForm in MdiChildren)
                    //{
                    //    childForm.Close();
                    //}
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Closing All opened Forms in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CloseAllOpenedForms()
        {
            try
            {
                foreach (Form childForm in MdiChildren)
                {
                    childForm.Close();
                }
            }
            catch
            {              
            }
        }

        private void CloseFormsOnLogout()
        {
            try
            {
                foreach (Form childForm in MdiChildren)
                {
                    childForm.Close();
                }
            }
            catch
            {
            }
        }

        private void stopExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }      

        private void interestStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                InterestStatement objInterestStatement = new InterestStatement();
                objInterestStatement.MdiParent = this;
                objInterestStatement.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of General Interest in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ledgerAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                if (strClientName == "LOTUS")
                {
                    LedgerAccount_Remark objLedgerAccount = new LedgerAccount_Remark();
                    objLedgerAccount.MdiParent = this;
                    objLedgerAccount.Show();
                }
                else
                {
                    LedgerAccount objLedgerAccount = new LedgerAccount();
                    objLedgerAccount.MdiParent = this;
                    objLedgerAccount.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Ledger Account Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void pToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ProfitandLoss pl = new ProfitandLoss();
                pl.MdiParent = this;
                pl.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Profit and Loss Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variant1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantMaster objVariantMaster = new VariantMaster("1", StrCategory1);
                objVariantMaster.MdiParent = this;
                objVariantMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variant2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantMaster objVariantMaster = new VariantMaster("2", StrCategory2);
                objVariantMaster.MdiParent = this;
                objVariantMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variant3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantMaster objVariantMaster = new VariantMaster("3", StrCategory3);
                objVariantMaster.MdiParent = this;
                objVariantMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variant4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantMaster objVariantMaster = new VariantMaster("4", StrCategory4);
                objVariantMaster.MdiParent = this;
                objVariantMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variant5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantMaster objVariantMaster = new VariantMaster("5", StrCategory5);
                objVariantMaster.MdiParent = this;
                objVariantMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void variantDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                VariantDetails objVariantDetails = new VariantDetails();
                objVariantDetails.MdiParent = this;
                objVariantDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Variant Details in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void designMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                if (MainPage.bArticlewiseOpening)
                {
                    ItemMaster objDesignMaster = new ItemMaster();
                    objDesignMaster.MdiParent = this;
                    objDesignMaster.Show();
                }
                else
                {
                    DesignMaster objDesignMaster = new DesignMaster();
                    objDesignMaster.MdiParent = this;
                    objDesignMaster.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Design Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailPurchaseBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseBook_Trading objPurchaseBook_Retail = new PurchaseBook_Trading();
                objPurchaseBook_Retail.MdiParent = this;
                objPurchaseBook_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Book Retail in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailPurchaseBoolRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    PurchaseBook_RetailRegister objPurchaseBook_Retail = new PurchaseBook_RetailRegister();
                    objPurchaseBook_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchaseBook_Retail.ShowInTaskbar = true;
                    objPurchaseBook_Retail.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Book Retail Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void stockMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                StockRegister objStockRegister = new StockRegister();
                objStockRegister.MdiParent = this;
                objStockRegister.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Stock Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailSalesBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                SaleBook_Trading objSaleBook_Retail = new SaleBook_Trading();
                objSaleBook_Retail.MdiParent = this;
                objSaleBook_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in sale book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailSalesRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SaleBook_TradingRegister objSaleBook_Retail = new SaleBook_TradingRegister();
                    objSaleBook_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleBook_Retail.ShowInTaskbar = true;
                    objSaleBook_Retail.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in retail sale book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void schemeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SchemeMaster objSchemeMaster = new SchemeMaster();
                objSchemeMaster.MdiParent = this;
                objSchemeMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in scheme master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void graceMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GraceDaysMaster objGraceDaysMaster = new GraceDaysMaster();
                objGraceDaysMaster.MdiParent = this;
                objGraceDaysMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in grace master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void importPurchaseBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ImportDataFromExcel_RetailPurchase objImport = new ImportDataFromExcel_RetailPurchase();
                objImport.MdiParent = this;
                objImport.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in  Import Retail Purchase Book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void importPurchaseBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ImportDataFromExcel_Purchase objImport = new ImportDataFromExcel_Purchase();
                objImport.MdiParent = this;
                objImport.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Import Purchase Book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void schemeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                FairDetails objFairDetails = new FairDetails();
                objFairDetails.MdiParent = this;
                objFairDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Fair Details in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void showWhatsappNoDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ShowWhatsappNoRegister objShowWhatsappNoRegister = new ShowWhatsappNoRegister();
                objShowWhatsappNoRegister.MdiParent = this;
                objShowWhatsappNoRegister.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Whatsapp No Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailSaleReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SaleReturn_Trading objSaleReturn_Retail = new SaleReturn_Trading();
                objSaleReturn_Retail.MdiParent = this;
                objSaleReturn_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Retail Sale return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void designMasterRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                DesignRegister objDesignRegister = new DesignRegister();
                objDesignRegister.MdiParent = this;
                objDesignRegister.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Design Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void referenceBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ReferenceBook objReferenceBook = new ReferenceBook();
                objReferenceBook.MdiParent = this;
                objReferenceBook.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void debitNoteTCSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TCSDetails objTCSDetails = new TCSDetails("DEBITNOTE");
                objTCSDetails.MdiParent = this;
                objTCSDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in TCS Details in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void tcsRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TCSRegister objTCSDetails = new TCSRegister();
                objTCSDetails.MdiParent = this;
                objTCSDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in TCS Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void advanceBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                AdvanceAdjustment objAdvanceAdjustment = new AdvanceAdjustment();
                objAdvanceAdjustment.MdiParent = this;
                objAdvanceAdjustment.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Advance Adjustment in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void advanceRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                AdvanceAdjustmentRegister objAdvanceAdjustment = new AdvanceAdjustmentRegister();
                objAdvanceAdjustment.MdiParent = this;
                objAdvanceAdjustment.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Advance Adjustment in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CashBook objCashBook = new CashBook();
            objCashBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objCashBook.ShowInTaskbar = true;
            objCashBook.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BankBook objBankBook = new SSS.BankBook();
            objBankBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objBankBook.ShowInTaskbar = true;
            objBankBook.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SupplierMaster objAccountMaster = new SupplierMaster();
            objAccountMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objAccountMaster.ShowInTaskbar = true;
            objAccountMaster.Show();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowPartyMasterSummary objPartyMaster = new ShowPartyMasterSummary();
            objPartyMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPartyMaster.ShowInTaskbar = true;
            objPartyMaster.Show();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strPartyLeder == "YES")
            {

                if (!ledgerPanel.Visible)
                {
                    ShowPartyNameWithBalance();
                }
                else
                {
                    ledgerPanel.Visible = false;
                    if (this.HasChildren)
                    {
                        this.MdiChildren[this.MdiChildren.Length - 1].Controls[0].Focus();
                    }
                }
            }
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (bLedgerReport)
            {
                if (strClientName == "LOTUS")
                {
                    LedgerAccount_Remark objLedger = new LedgerAccount_Remark(true);
                    objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objLedger.ShowInTaskbar = true;
                    objLedger.Focus();
                    objLedger.Show();
                }
                else
                {
                    LedgerAccount objLedger = new LedgerAccount(true);
                    objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objLedger.ShowInTaskbar = true;
                    objLedger.Focus();
                    objLedger.Show();
                }
            }
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (bCourierAdd)
            {
                CourierBookIN objCourier = new CourierBookIN(true);
                objCourier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objCourier.Focus();
                objCourier.Show();
            }
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((MainPage.mymainObject.bReportSummary))
            {
                if (partyPanel.Visible)
                {
                    ShowReportSummary();
                }
                else
                {
                    ShowReportSummary objshowAllReport = new ShowReportSummary();
                    objshowAllReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objshowAllReport.TopLevel = true;
                    objshowAllReport.Show();
                }
            }
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.mymainObject.bPurchaseSlip && MainPage.mymainObject.bMultiCompany)
            {
                PurchaseOutstandingSlip objPurchase = new PurchaseOutstandingSlip(true);
                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchase.TopLevel = true;
                objPurchase.Show();
            }
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((MainPage.mymainObject.bPurchaseSlip))
            {
                PurchaseOutstandingSlip objPurchase = new PurchaseOutstandingSlip();
                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchase.TopLevel = true;
                objPurchase.Show();
            }
        }

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("calc.exe");
        }

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bMultiCompany)
            {
                InterestStatement objInterestStatement = new InterestStatement(true);
                objInterestStatement.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objInterestStatement.TopLevel = true;
                objInterestStatement.Show();
            }
        }

        private void discountDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                Discount_Offer objDiscount_Offer = new Discount_Offer();
                objDiscount_Offer.MdiParent = this;
                objDiscount_Offer.Show();

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Discount in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void rdoWith_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MainPage._PrintWithDialog = rdoWith.Checked;
            }
            catch { }
        }

        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OrderBooking objOrderBook = new OrderBooking();
            objOrderBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objOrderBook.ShowInTaskbar = true;
            objOrderBook.Show();
        }

        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GoodscumPurchase objPurchaseBook = new GoodscumPurchase();
            objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurchaseBook.ShowInTaskbar = true;
            objPurchaseBook.Show();
        }

        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaleBook objSaleBook = new SaleBook();
            objSaleBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleBook.ShowInTaskbar = true;
            objSaleBook.Show();
        }

        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GoodsReceiveAdjustment objOrderAdjustment = new GoodsReceiveAdjustment();
            objOrderAdjustment.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objOrderAdjustment.ShowInTaskbar = true;
            objOrderAdjustment.Show();
        }

        private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MarketerMaster objSalesManMaster = new MarketerMaster();
            objSalesManMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSalesManMaster.ShowInTaskbar = true;
            objSalesManMaster.Show();
        }

        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SalesBookRegisters objSaleRegister = new SalesBookRegisters();
            objSaleRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleRegister.ShowInTaskbar = true;
            objSaleRegister.Show();
        }

        private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GoodsReceiveRegister objPurchaseRegister = new GoodsReceiveRegister();
            objPurchaseRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurchaseRegister.ShowInTaskbar = true;
            objPurchaseRegister.Show();
        }       

        private void bgRegisterToolStripItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    BankGuaranteeRegister objChequeDetailRegister = new BankGuaranteeRegister();
                    objChequeDetailRegister.MdiParent = this;
                    objChequeDetailRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Cheque Detail Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void bankGuaranteeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    BankGuarantee objBankGuaranteeRegister = new BankGuarantee();
                    objBankGuaranteeRegister.MdiParent = this;
                    objBankGuaranteeRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bank Gaurantee in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailPurchaseReturnToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseReturn_Retail objPurchaseReturn_Retail = new PurchaseReturn_Retail();
                objPurchaseReturn_Retail.MdiParent = this;
                objPurchaseReturn_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void bankGuaranteeRegtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    BankGuaranteeRegister objBankGuaranteeRegister = new BankGuaranteeRegister();
                    objBankGuaranteeRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objBankGuaranteeRegister.ShowInTaskbar = true;
                    objBankGuaranteeRegister.Show();
                }
            }
            catch { }
        }

        private void saleSummaryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SummaryRegister objSummaryRegister = new SummaryRegister("SALES");
                    objSummaryRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummaryRegister.ShowInTaskbar = true;
                    objSummaryRegister.Show();
                }
            }
            catch { }
        }

        private void resInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SaleBook_Restorent objSaleBook_Restorent = new SaleBook_Restorent();
                    //objSaleBook_Restorent.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleBook_Restorent.MdiParent = this;
                    objSaleBook_Restorent.Show();
                }
            }
            catch { }
        }

        private void purchaseSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SummaryRegister objSummaryRegister = new SummaryRegister("PURCHASE");
                    objSummaryRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummaryRegister.ShowInTaskbar = true;
                    objSummaryRegister.Show();
                }
            }
            catch { }
        }

        private void stockAuditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    StockAudit objStockAudit = new StockAudit();
                    objStockAudit.MdiParent = this;
                   // objStockAudit.ShowInTaskbar = true;
                    objStockAudit.Show();
                }
            }
            catch { }
        }

        private void stockDrillDownReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    StockDrillDownReport objStockDrillDownReport = new StockDrillDownReport();
                    objStockDrillDownReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objStockDrillDownReport.ShowInTaskbar = true;
                    objStockDrillDownReport.Show();
                }
            }
            catch { }
        }

        private void saleReturnSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SummaryRegister objSummaryRegister = new SummaryRegister("SALE RETURN");
                    objSummaryRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummaryRegister.ShowInTaskbar = true;
                    objSummaryRegister.Show();
                }
            }
            catch { }
        }

        private void supplierMappedRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SupplierMapingRegister objSupplierMapingRegister = new SupplierMapingRegister();
                    objSupplierMapingRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSupplierMapingRegister.ShowInTaskbar = true;
                    objSupplierMapingRegister.Show();
                }
            }
            catch { }
        }

        private void supplierMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SupplierMapping objSummaryRegister = new SupplierMapping();
                    objSummaryRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummaryRegister.ShowInTaskbar = true;
                    objSummaryRegister.Show();
                }
            }
            catch { }
        }

        private void purchaseReturnSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SummaryRegister objSummaryRegister = new SummaryRegister("PURCHASE RETURN");
                    objSummaryRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummaryRegister.ShowInTaskbar = true;
                    objSummaryRegister.Show();
                }
            }
            catch { }
        }

        private void pincodeDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PinCodeDistanceRegister objPinCodeDistanceRegister = new PinCodeDistanceRegister();
                objPinCodeDistanceRegister.MdiParent = this;
                objPinCodeDistanceRegister.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in PinCode Distance Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OrderBookingRegister objOrderRegister = new OrderBookingRegister();
            objOrderRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objOrderRegister.ShowInTaskbar = true;
            objOrderRegister.Show();
        }

        private void customSaleReturnRegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CustomSaleReturnRegister objSaleReurnRegister = new CustomSaleReturnRegister();
                    objSaleReurnRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleReurnRegister.ShowInTaskbar = true;
                    objSaleReurnRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sale Return Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void customPurchaseReturnRegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CustomPurchaseReturnRegister objPurchaseReurnRegister = new CustomPurchaseReturnRegister();
                    objPurchaseReurnRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchaseReurnRegister.ShowInTaskbar = true;
                    objPurchaseReurnRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Return Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void saleDrillDownReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SalesDrillDownReport objPurchaseReurnRegister = new SalesDrillDownReport();
                    objPurchaseReurnRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchaseReurnRegister.ShowInTaskbar = true;
                    objPurchaseReurnRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Return Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void smsSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                MessageMaster objMessageMaster = new MessageMaster();
                objMessageMaster.MdiParent = this;
                objMessageMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Message Master  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SMSReportRegister objSMSReport = new SMSReportRegister();
            objSMSReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSMSReport.ShowInTaskbar = true;
            objSMSReport.Show();
        }

        private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EmailRegister objEmailRegister = new EmailRegister();
            objEmailRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objEmailRegister.ShowInTaskbar = true;
            objEmailRegister.Show();
        }

        private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StockRegister objStockRegister = new StockRegister();
            objStockRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStockRegister.ShowInTaskbar = true;
            objStockRegister.Show();
        }

        private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ItemGroupMaster objItemGroupMaster = new ItemGroupMaster();
            objItemGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objItemGroupMaster.ShowInTaskbar = true;
            objItemGroupMaster.Show();
        }

        private void linkLabel26_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (bArticlewiseOpening)
            {
                ItemMaster objItemMaster = new ItemMaster();
                objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objItemMaster.ShowInTaskbar = true;
                objItemMaster.Show();
            }
            else
            {
                DesignMaster objDesignMaster = new DesignMaster();
                objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objDesignMaster.ShowInTaskbar = true;
                objDesignMaster.Show();
            }
        }

        private void linkLabel27_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PurchaseReturnRegister objPurReturnReg = new PurchaseReturnRegister();
            objPurReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurReturnReg.ShowInTaskbar = true;
            objPurReturnReg.Show();
        }

        private void linkLabel28_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StationMaster objStationMaster = new StationMaster();
            objStationMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStationMaster.ShowInTaskbar = true;
            objStationMaster.Show();
        }

        private void linkLabel29_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TransportMaster objTransportMaster = new TransportMaster();
            objTransportMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objTransportMaster.ShowInTaskbar = true;
            objTransportMaster.Show();
        }

        private void linkLabel30_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            JournalEntry_New objJournal = new JournalEntry_New();
            objJournal.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objJournal.ShowInTaskbar = true;
            objJournal.Show();
        }

        private void linkLabel31_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaleReturn objSaleReturn = new SaleReturn();
            objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturn.ShowInTaskbar = true;
            objSaleReturn.Show();
        }

        private void linkLabel32_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PurchaseReturn_Trading objSaleReturn = new PurchaseReturn_Trading();
            objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturn.ShowInTaskbar = true;
            objSaleReturn.Show();
        }

        private void linkLabel33_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaleReturnRegister objSaleReturnReg = new SaleReturnRegister();
            objSaleReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturnReg.ShowInTaskbar = true;
            objSaleReturnReg.Show();
        }

        private void linkLabel34_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.mymainObject.bDashboard)
            {
                FormDashboard objDashBoard = new FormDashboard();
                objDashBoard.MdiParent = this;
                objDashBoard.Show();
            }
            else
            { MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        }

        private void linkLabel35_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MdiChildren.Length == 0 && !partyPanel.Visible && !ledgerPanel.Visible)
            {
                if ((MainPage.mymainObject.bCashAdd))
                {
                    CashBook objCash = new CashBook();
                    objCash.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objCash.ShowInTaskbar = true;
                    objCash.Show();
                }
            }
        }

        private void linkLabel36_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BankBook objBank = new BankBook();
            objBank.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objBank.ShowInTaskbar = true;
            objBank.Show();
        }

        private void linkLabel37_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SupplierMaster objSupplierMaster = new SupplierMaster();
            objSupplierMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSupplierMaster.ShowInTaskbar = true;
            objSupplierMaster.Show();
        }

        private void linkLabel38_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UnitMaster objUnitMaster = new UnitMaster();
            objUnitMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objUnitMaster.ShowInTaskbar = true;
            objUnitMaster.Show();
        }

        private void linkLabel39_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                //if (Screen.PrimaryScreen.Bounds.Width < 1100)
                //{
                //    SaleBook_Retail_POS objSaleBill_Retail = new SaleBook_Retail_POS();
                //    objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                //    objSaleBill_Retail.ShowInTaskbar = true;
                //    objSaleBill_Retail.Show();
                //}
                //else
                {
                    SaleBook_Retail objSaleBill_Retail = new SaleBook_Retail();
                    objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleBill_Retail.ShowInTaskbar = true;
                    objSaleBill_Retail.Show();
                }
            }
            else if (MainPage.strSoftwareType == "TRADING")
            {
                if (MainPage._bCustomPurchase)
                {
                    SaleBook_Retail_Custom objSale = new SaleBook_Retail_Custom();
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
                else
                {
                    SaleBook_Trading objSale = new SaleBook_Trading();
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
            }
        }

        private void linkLabel40_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BrandMaster objBrandMaster = new BrandMaster();
            objBrandMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objBrandMaster.ShowInTaskbar = true;
            objBrandMaster.Show();
        }

        private void linkLabel41_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                PurchaseBook_Retail_Merge objPurchaseBill_Retail = new PurchaseBook_Retail_Merge();
                objPurchaseBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchaseBill_Retail.ShowInTaskbar = true;
                objPurchaseBill_Retail.Show();
            }
            else if (MainPage.strSoftwareType == "TRADING")
            {
                if (MainPage._bCustomPurchase)
                {
                    PurchaseBook_Retail_Custom objPurchase = new PurchaseBook_Retail_Custom();
                    objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchase.ShowInTaskbar = true;
                    objPurchase.Show();
                }
                else
                {
                    PurchaseBook_Trading objPurchase = new PurchaseBook_Trading();
                    objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchase.ShowInTaskbar = true;
                    objPurchase.Show();
                }
            }
        }

        private void mergeReferenceNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                MergingReference objMerging = new MergingReference();
                objMerging.MdiParent = this;
                objMerging.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Merging REFERENCE Name in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void templateSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TemplateSetting objTemplateSetting = new TemplateSetting();
                objTemplateSetting.MdiParent = this;
                objTemplateSetting.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening Template Setting in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void linkLabel42_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomPurchaseReport objCustomPurReport = new CustomPurchaseReport();
            objCustomPurReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objCustomPurReport.ShowInTaskbar = true;
            objCustomPurReport.Show();
        }

        private void linkLabel43_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomSaleRegister objCustomSaleReg = new CustomSaleRegister();
            objCustomSaleReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objCustomSaleReg.ShowInTaskbar = true;
            objCustomSaleReg.Show();
        }

        private void linkLabel44_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StockRegister objStockReg = new StockRegister();
            objStockReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStockReg.ShowInTaskbar = true;
            objStockReg.Show();
        }

        private void linkLabel45_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VariantMaster objVariantMaster = new VariantMaster("1", StrCategory1);
            objVariantMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objVariantMaster.ShowInTaskbar = true;
            objVariantMaster.Show();
        }

        private void linkLabel46_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VariantMaster objVariantMaster = new VariantMaster("2", StrCategory2);
            objVariantMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objVariantMaster.ShowInTaskbar = true;
            objVariantMaster.Show();
        }

        private void linkLabel47_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                AlterationSlip objAltSLip = new AlterationSlip();
                objAltSLip.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objAltSLip.ShowInTaskbar = true;
                objAltSLip.Show();
            }
        }

        private void linkLabel48_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ItemGroupMaster objItemGroupMaster = new ItemGroupMaster();
            objItemGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objItemGroupMaster.ShowInTaskbar = true;
            objItemGroupMaster.Show();
        }

        private void linkLabel49_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StockTransferVoucher objStockTrnsfer = new StockTransferVoucher();
            objStockTrnsfer.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStockTrnsfer.ShowInTaskbar = true;
            objStockTrnsfer.Show();
        }

        private void linkLabel50_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                SalesManMaster objSalesManMaster = new SalesManMaster();
                objSalesManMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSalesManMaster.ShowInTaskbar = true;
                objSalesManMaster.Show();
            }
        }

        private void linkLabel51_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (bArticlewiseOpening)
            {
                ItemMaster objItemMaster = new ItemMaster();
                objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objItemMaster.ShowInTaskbar = true;
                objItemMaster.Show();
            }
            else
            {
                DesignMaster objDesignMaster = new DesignMaster();
                objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objDesignMaster.ShowInTaskbar = true;
                objDesignMaster.Show();
            }
        }

        private void linkLabel52_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MaterialCenterMaster objMaterialCenter = new MaterialCenterMaster();
            objMaterialCenter.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objMaterialCenter.ShowInTaskbar = true;
            objMaterialCenter.Show();
        }

        private void linkLabel53_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StationMaster objStationMaster = new StationMaster();
            objStationMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStationMaster.ShowInTaskbar = true;
            objStationMaster.Show();
        }

        private void linkLabel54_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TransportMaster objTransportMaster = new TransportMaster();
            objTransportMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objTransportMaster.ShowInTaskbar = true;
            objTransportMaster.Show();
        }

        private void linkLabel55_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            JournalEntry_New objJournal = new JournalEntry_New();
            objJournal.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objJournal.ShowInTaskbar = true;
            objJournal.Show();
        }

        private void linkLabel56_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaleReturn_Retail objSaleReturn_Retail = new SaleReturn_Retail();
            objSaleReturn_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturn_Retail.ShowInTaskbar = true;
            objSaleReturn_Retail.Show();
        }

        private void linkLabel57_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PurchaseReturn_Trading objSaleReturn_Retail = new PurchaseReturn_Trading();
            objSaleReturn_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturn_Retail.ShowInTaskbar = true;
            objSaleReturn_Retail.Show();
        }

        private void linkLabel58_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaleReturnRegister objSaleReturnReg = new SaleReturnRegister();
            objSaleReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSaleReturnReg.ShowInTaskbar = true;
            objSaleReturnReg.Show();
        }

        private void linkLabel62_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PurchaseReturnRegister objPurReturnReg = new PurchaseReturnRegister();
            objPurReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objPurReturnReg.ShowInTaskbar = true;
            objPurReturnReg.Show();
        }

        private void linkLabel63_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                Salesman_Report objSalesManReport = new Salesman_Report();
                objSalesManReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSalesManReport.ShowInTaskbar = true;
                objSalesManReport.Show();
            }
        }

        private void linkLabel59_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strSoftwareType == "RETAIL")
            {
                AlterationSlipRegister objAltSlipReg = new AlterationSlipRegister();
                objAltSlipReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objAltSlipReg.ShowInTaskbar = true;
                objAltSlipReg.Show();
            }
        }

        private void linkLabel60_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StockTransferRegister objStockTransReg = new StockTransferRegister();
            objStockTransReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objStockTransReg.ShowInTaskbar = true;
            objStockTransReg.Show();
        }

        private void linkLabel61_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.mymainObject.bAccessories)
            {
                EditLogReport objEditLogReport = new EditLogReport();
                objEditLogReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objEditLogReport.ShowInTaskbar = true;
                objEditLogReport.Show();
            }
            else
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void linkLabel64_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.mymainObject.bDashboard)
            {
                FormDashboard objDashBoard = new FormDashboard();
                objDashBoard.MdiParent = this;
                objDashBoard.Show();
            }
            else
            { MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        }

        private void linkLabel65_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MainPage.strPartyLeder == "YES")
            {

                if (!ledgerPanel.Visible)
                {
                    ShowPartyNameWithBalance();
                }
                else
                {
                    ledgerPanel.Visible = false;
                    if (this.HasChildren)
                    {
                        this.MdiChildren[this.MdiChildren.Length - 1].Controls[0].Focus();
                    }
                }
            }
        }

        private void linkLabel66_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (bLedgerReport)
            {
                if (strClientName == "LOTUS")
                {
                    LedgerAccount_Remark objLedger = new LedgerAccount_Remark();
                    objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objLedger.ShowInTaskbar = true;
                    objLedger.Focus();
                    objLedger.Show();
                }
                else
                {
                    LedgerAccount objLedger = new LedgerAccount();
                    objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objLedger.ShowInTaskbar = true;
                    objLedger.Focus();
                    objLedger.Show();
                }
            }
        }

        private void creditNoteTCSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TCSDetails objTCSDetails = new TCSDetails("CREDITNOTE");
                objTCSDetails.MdiParent = this;
                objTCSDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in TCS Details in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void retailPurchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseReturn_Trading objPurchaseReturn_Retail = new PurchaseReturn_Trading();
                objPurchaseReturn_Retail.MdiParent = this;
                objPurchaseReturn_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void saleServiceBookRegistertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SaleServiceRegister objSaleServiceRegister = new SaleServiceRegister();
                    objSaleServiceRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleServiceRegister.ShowInTaskbar = true;
                    objSaleServiceRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sale Service Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void creditNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CreditNote_Supplier objCreditNote_Supplier = new CreditNote_Supplier();                   
                    objCreditNote_Supplier.MdiParent = this;
                    objCreditNote_Supplier.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Credit Note in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void MainPage_Load(object sender, EventArgs e)
        {

        }

        private void customSaleReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    if (MainPage.strSoftwareType == "RES_RETAIL")
                    {
                        RestorentSaleRegister objCustomSaleRegister = new RestorentSaleRegister();
                        objCustomSaleRegister.MdiParent = this;
                        objCustomSaleRegister.Show();
                    }
                    else
                    {
                        CustomSaleRegister objCustomSaleRegister = new CustomSaleRegister();
                        objCustomSaleRegister.MdiParent = this;
                        objCustomSaleRegister.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Custom sale register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void customPurchaseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CustomPurchaseReport objCustomPurchaseReport = new CustomPurchaseReport();
                    objCustomPurchaseReport.MdiParent = this;
                    objCustomPurchaseReport.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Custom purchase register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void profitDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    BrandwiseProfit objBrandwiseProfit = new BrandwiseProfit();
                    objBrandwiseProfit.MdiParent = this;
                    objBrandwiseProfit.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Brand wise Profit in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void stockTransferRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    StockTransferRegister objStockTransferRegister = new StockTransferRegister();
                    objStockTransferRegister.MdiParent = this;
                    objStockTransferRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Stock Transfer Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void materialCentreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    MaterialCenterMaster objMaterialCenterMaster = new MaterialCenterMaster();
                    objMaterialCenterMaster.MdiParent = this;
                    objMaterialCenterMaster.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Material Cente rMaster in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void stockTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    StockTransferVoucher objAddStockTransferVoucher = new StockTransferVoucher();
                    objAddStockTransferVoucher.MdiParent = this;
                    objAddStockTransferVoucher.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Add Stock Transfer Voucher in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void layoutdesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    LayoutMaster objLayoutMaster = new LayoutMaster();
                    objLayoutMaster.MdiParent = this;
                    objLayoutMaster.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Layout design in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void creditNoteRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CreditNoteRegister objCreditNote = new CreditNoteRegister();
                    objCreditNote.MdiParent = this;
                    objCreditNote.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Credit Note Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void debitNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    DebitNote_Customer objDebitNote_Customer = new DebitNote_Customer();
                    objDebitNote_Customer.MdiParent = this;
                    objDebitNote_Customer.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Credit Note in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void calcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("calc.exe");
            }
            catch { }
        }

        private void groupCategoryMastertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CategoryMaster objCategoryMaster = new CategoryMaster();
                    objCategoryMaster.MdiParent = this;
                    objCategoryMaster.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Group Category Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void customPurchaseBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                PurchaseBook_Retail_Custom objPurchaseBook = new PurchaseBook_Retail_Custom();
                objPurchaseBook.MdiParent = this;
                objPurchaseBook.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in purchase book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailPurchaseBookToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseBook_Retail_Merge objPurchaseBook_Retail = new PurchaseBook_Retail_Merge();
                objPurchaseBook_Retail.MdiParent = this;
                objPurchaseBook_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Book Retail in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void incentiveAndDiscToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                Incentive_Target objIncentive_Target = new Incentive_Target();
                objIncentive_Target.MdiParent = this;
                objIncentive_Target.Show();

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Discount And Incentive in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void mergeSalesBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                SaleBook_Retail_Custom objSaleBook_Retail_Custom = new SaleBook_Retail_Custom();
                objSaleBook_Retail_Custom.MdiParent = this;
                objSaleBook_Retail_Custom.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in purchase book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailSaleBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                if (Screen.PrimaryScreen.Bounds.Width < 1100)
                {
                    SaleBook_Retail_POS objSaleBill_Retail = new SaleBook_Retail_POS();
                    objSaleBill_Retail.MdiParent = this;
                    objSaleBill_Retail.Show();
                }
                else
                {
                    SaleBook_Retail objSaleBill_Retail = new SaleBook_Retail();
                    objSaleBill_Retail.MdiParent = this;
                    objSaleBill_Retail.Show();
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in sale book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void alterationSlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                AlterationSlip objAlterationSlip = new AlterationSlip();
                objAlterationSlip.MdiParent = this;
                objAlterationSlip.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Alteration slip in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void alterationslipRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                AlterationSlipRegister objAlterationSlip = new AlterationSlipRegister();
                objAlterationSlip.MdiParent = this;
                objAlterationSlip.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Alteration slip Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void profitMargintoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                Profit_Margin objProfit_Margin = new Profit_Margin();
                objProfit_Margin.MdiParent = this;
                objProfit_Margin.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Profit Margin in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void brandMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                BrandMaster objBrandMaster = new BrandMaster();
                objBrandMaster.MdiParent = this;
                objBrandMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Brand master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                FormDashboard objFormDashboard = new FormDashboard();
                objFormDashboard.MdiParent = this;
                objFormDashboard.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in dash board in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void editLogReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    EditLogReport objEditLogReport = new EditLogReport();
                    objEditLogReport.MdiParent = this;
                    objEditLogReport.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Edit Log in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void retailSaleReturntoolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();

                SaleReturn_Retail objSaleReturn_Retail = new SaleReturn_Retail();
                objSaleReturn_Retail.MdiParent = this;
                objSaleReturn_Retail.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sale Return Retail in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void depreciationChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    DepreciationChart objDepreciationChart = new DepreciationChart();
                    objDepreciationChart.MdiParent = this;
                    objDepreciationChart.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Depreciation Chart in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void salesManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                   // if (MainPage.strSoftwareType == "RETAIL")
                    {
                        Salesman_Report objAgent_Report = new Salesman_Report();
                        objAgent_Report.MdiParent = this;
                        objAgent_Report.Show();
                    }
                    //else
                    //{
                    //    Agent_Report objAgent_Report = new Agent_Report();
                    //    objAgent_Report.MdiParent = this;
                    //    objAgent_Report.Show();
                    //}
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Agent Report in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void debitNoteRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    DebitNoteRegister objDebitNote = new DebitNoteRegister();
                    objDebitNote.MdiParent = this;
                    objDebitNote.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Debit Note Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void schemeDetailMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SchemeDetailMaster objTourMaster = new SchemeDetailMaster();
                    objTourMaster.MdiParent = this;
                    objTourMaster.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Tour Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void itemCategoryMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ItemCategoryMaster objItemGroupMaster = new ItemCategoryMaster();
                objItemGroupMaster.MdiParent = this;
                objItemGroupMaster.Show();
            }
            catch
            {
            }
        }

        private void chequeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                
                    CloseAllOpenForms();
                    ChequeDetails objChequeDetails = new ChequeDetails();
                    objChequeDetails.MdiParent = this;
                    objChequeDetails.Show();
                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Cheque Details Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void chequeDetailRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    ChequeDetailRegister objChequeDetailRegister = new ChequeDetailRegister();
                    objChequeDetailRegister.MdiParent = this;
                    objChequeDetailRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Cheque Detail Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void addressBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                AddressBook objAddressBook = new AddressBook();
                objAddressBook.MdiParent = this;
                objAddressBook.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Address book in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void stockAgeingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    ShowStockSlabwise objShowPartyDetails = new ShowStockSlabwise();
                    objShowPartyDetails.MdiParent = this;
                    objShowPartyDetails.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Party Details Due Days Wise in Main Page.", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void birthdayAnniversaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BirthdayDetails objBirthdayDetails = new BirthdayDetails();
                objBirthdayDetails.MdiParent = this;
                objBirthdayDetails.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Birthday Details in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void gSTHSNSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    GSTHSN_Summary objGSTHSN_Summary = new GSTHSN_Summary();
                    objGSTHSN_Summary.MdiParent = this;
                    objGSTHSN_Summary.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Sales Summary in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void petiDispatchRegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                   // CloseAllOpenForms();
                    ShowPetiDetails objShowPetiDetails = new ShowPetiDetails();
                    objShowPetiDetails.MdiParent = this;
                    objShowPetiDetails.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Peti Agent Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void saleSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BranchesSalesDetail objSalesSummary = new BranchesSalesDetail();
                objSalesSummary.MdiParent = this;
                objSalesSummary.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Sales Summary in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dueDaysWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    ShowPartyDetailsWithDueDays objShowPartyDetails = new ShowPartyDetailsWithDueDays();
                    objShowPartyDetails.MdiParent = this;
                    objShowPartyDetails.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Show Party Details Due Days Wise in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void downloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {                
                string strExePath = @"C:\Windows\Fonts\IDAutomationHC39M.ttf";
                if (!File.Exists(strExePath))
                {
                    string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\IDAutomationHC39M.ttf";
                    strPath = DataBaseAccess.DownloadFileFromServer("IDAutomationHC39M.ttf", strPath);
                    if (strPath != "")
                        System.Diagnostics.Process.Start(strPath);
                }
            }
            catch { }
        }

        private void balanceSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BalanceSheet_New objBalanceSheet = new BalanceSheet_New();
                objBalanceSheet.MdiParent = this;
                objBalanceSheet.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Balance Sheet  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            
        }

        private void gSTR2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GSTR_2_Summary objGSTR_2_Summary = new GSTR_2_Summary();
                objGSTR_2_Summary.MdiParent = this;
                objGSTR_2_Summary.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GSTR_2 Summary in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void partyBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                ShowPartyBalanceSlabwise _obj = new ShowPartyBalanceSlabwise();
                _obj.MdiParent = this;
                _obj.Show();
            }
            catch { }
        }

        private void journalRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                JournalRegister _obj = new JournalRegister();
                _obj.MdiParent = this;
                _obj.Show();
            }
            catch { }
        }

        private void removalReasonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                RemovalReason _obj = new RemovalReason();
                _obj.MdiParent = this;
                _obj.Show();
            }
            catch { }
        }

        private void graphicalSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GetMSChartFromServer();
                SalesSummaryGraph objSalesSummaryGraph = new SalesSummaryGraph();
                objSalesSummaryGraph.MdiParent = this;
                objSalesSummaryGraph.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sales Summary Graph in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void saleServiceBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SaleServiceBook objSaleServiceBook = new SaleServiceBook();
                objSaleServiceBook.MdiParent = this;
                objSaleServiceBook.Show();
            }
            catch
            {
            }
        }

        private void importExcelSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ImportDataFromExcel objImportDataFromExcel = new ImportDataFromExcel();
                objImportDataFromExcel.MdiParent = this;
                objImportDataFromExcel.Show();
            }
            catch
            {
            }
        }

        private void biltyAndWayBilltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BiltyDetails objBiltyDetails = new BiltyDetails();
                objBiltyDetails.MdiParent = this;
                objBiltyDetails.Show();
            }
            catch
            {
            }
        }

        private void amendedBillRegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                AmendedBillRegister objAmendedBillRegister = new AmendedBillRegister();
                objAmendedBillRegister.MdiParent = this;
                objAmendedBillRegister.Show();
            }
            catch
            {
            }
        }

        private void paymentRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ViewPaymentRequest objViewPaymentRequest = new ViewPaymentRequest();
                objViewPaymentRequest.MdiParent = this;
                objViewPaymentRequest.Show();
            }
            catch 
            {                
            }
        }

        private void mergePartyMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                MergingParty objMergingParty = new MergingParty();
                objMergingParty.MdiParent = this;
                objMergingParty.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Merger Accounts in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void mergeTransportStationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                MergingTransport objMergingTransport = new MergingTransport();
                objMergingTransport.MdiParent = this;
                objMergingTransport.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Merger Transport in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void mergeGroupItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                MergingItems objMerging = new MergingItems();
                objMerging.MdiParent = this;
                objMerging.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Merging Group Name in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void trialBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TrialBalance objTrialBalance = new TrialBalance();
                objTrialBalance.MdiParent = this;
                objTrialBalance.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Trial Balance  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        private void salesReportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SalesBookRegisters objRegister = new SalesBookRegisters();
                    objRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objRegister.ShowInTaskbar = true;
                    objRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of  Show Sales Record in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CompanySetting objCompanySetting = new CompanySetting();
                objCompanySetting.MdiParent = this;
                objCompanySetting.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Company Setting  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        private void orderSlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //CloseAllOpenForms();
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    OrderBookingRegister objSlip = new OrderBookingRegister();
                    objSlip.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSlip.ShowInTaskbar = true;
                    objSlip.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Show Pending Order Slip in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void MainPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.Dispose();
            //  ChangeOtherPath();
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateBackupFile(true);
        }

        private void CreateBackupFile(bool _bStatus)
        {
            try
            {
                //DialogResult dr = MessageBox.Show("Are you Want to Create Backup ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //if (dr == DialogResult.Yes)
                //{
                string strPath = MainPage.strServerPath + "\\" + strCompanyName + " Backup\\" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString();

                Directory.CreateDirectory(strPath);

                dba.CreateBackupWithCommand(strPath);
                if (_bStatus)
                    MessageBox.Show("Thanks ! Backup Generated Successfully ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                // MessageBox.Show(" Please send PDF files on Internet ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Backup not Created " + ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private bool CheckTimeSlot()
        //{

        //}

        private void myTimer_Tick(object sender, EventArgs e)
        {
            if (strBirthDayName == "")
                lblDate.Text = "!! USER NAME  : " + strLoginName + " !!";
            else
                lblDate.Text = "!! HAPPY BIRTHDAY TO " + strLoginName + " !!";

            this.Text = strCompanyName + "   " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString() + " : GST VERSION : " + strProductVersion;
            string strDateTime = DateTime.Now.ToLongTimeString(), strMachineName = System.Environment.MachineName, strAccountName = System.Environment.UserName.Replace("'", "").ToUpper();

            if ((strDateTime.Contains(":45:00") || strDateTime.Contains(":15:00")) && strSoftwareType == "AGENT" && bReminder)
            {
                if (MainPage.strCompanyName.Contains("SARAOGI SUPER"))
                {
                    if (strBranchCode.Contains("DL") && MainPage.mymainObject.bCashAdd && bOtherExtraControl)
                    {
                        string strDate = MainPage.currentDate.ToString("MM/dd/yyyy " + DateTime.Now.Hour + ":15:00");
                        dba.ImportBankStatement(strDate);
                    }
                    if (bOrderAdd)
                        dba.DownloadOrderDetails();
                }
            }

            if (strDateTime.Contains(":00:00"))
            {                
                if ((strMachineName.ToUpper() == strComputerName.ToUpper() && (strAccountName.Contains("ADMIN") || !MainPage.strCompanyName.Contains("SARAOGI SUPER"))) || bReminder)
                {
                    if (strOnlineDataBaseName != "")
                    {
                        if (dba.GetDownloadStatus(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"))>0)
                        {
                            if (bMultiBranch)
                            {
                                dba.DownloadMaster(MainPage.strOnlineDataBaseName);
                                dba.DownloadStationMaster(MainPage.strOnlineDataBaseName);
                                dba.DownloadReferenceMaster(MainPage.strOnlineDataBaseName);
                            }

                            //if (bSendToInternet)
                            //{
                            //    DialogResult _result = MessageBox.Show("Are you want to send data to cloud ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            //    if (_result == DialogResult.Yes)
                            //    {
                            //        SendInternet objSend = new SendInternet(1);
                            //        objSend.SendData();
                            //    }
                            //}
                        }                        
                    }

                    if (strDateTime == "7:00:00" || strDateTime == "11:00:00")
                        CreateBackupFile(false);
                }
            }

            if (strServerPath.Contains("NET"))
            {
                if (strLoginName == "BACKUP" && (strMachineName.ToUpper() == strComputerName.ToUpper() && strDateTime == "12:00:00"))
                {
                    dba.UpdateExtendedLimitAutomatically();
                }
            }

            //if (bReminder)
            //{
            //    if (strDateTime.Contains(":00:00 ") && strLoginName!="")
            //    {
            //        if (bSMSReport)
            //        {
            //            dba.CheckFailedSMS();
            //        }
            //        if (bSendToInternet)
            //        {
            //            DialogResult result = MessageBox.Show("Are you want to send data to live server ?", "Send data to live server", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            //            if (result == DialogResult.Yes)
            //            {                           
            //                SendInternet objSend = new SendInternet(1);
            //                objSend.SendData();
            //            }
            //        }
            //    }
            //}

        }

        private void GetReminderMessage()
        {
            dtReminder = DataBaseAccess.GetDataTableRecord("Select ID,ReminderTime,Message,CreatedBy from Reminder Where UserName Like('" + MainPage.strLoginName + "') and Status=1 and ReminderTime<=DATEADD(minute,10,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) and ReminderTime>DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) ");
        }

        private void ReminderSet()
        {
            try
            {
                if (dtReminder != null)
                {
                    foreach (DataRow row in dtReminder.Rows)
                    {
                        DateTime date = Convert.ToDateTime(row["ReminderTime"]);
                        if (date.ToString() == DateTime.Now.ToString())
                        {
                            string strMessage = Convert.ToString(row["Message"]), strID = Convert.ToString(row["ID"]), strUser = Convert.ToString(row["CreatedBy"]);
                            strMessage += " . Regards  :  " + strUser;
                            DialogResult result = MessageBox.Show(strMessage, "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                            if (result == DialogResult.OK)
                            {
                                SetReminderDeactive(strID, 1);
                            }
                            else
                            {
                                SetReminderDeactive(strID, 0);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SetReminderDeactive(string strID, int sStatus)
        {
            try
            {
                string strQuery = "";
                if (sStatus == 0)
                {
                    strQuery = " Update Reminder Set  ReminderTime=DATEADD(minute,10,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) Where ID=" + strID + " ";
                }
                else
                {
                    strQuery = " Update Reminder Set Status=0 Where ID=" + strID + " ";
                }
                DataBaseAccess.ExecuteMyNonQuery(strQuery);
            }
            catch
            {
            }
        }
        
        private void goodsReceivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    GoodsReceiveRegister objGRRegister = new GoodsReceiveRegister();
                    objGRRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objGRRegister.ShowInTaskbar = true;
                    objGRRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Goods Receive in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        private void partyMergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void viewEditToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CompanyMaster newComp = new CompanyMaster("Update");
                newComp.MdiParent = this;
                newComp.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of New Company for Updation in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void changeCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CloseAllOpenedForms();
                SelectCompany objSelect = new SelectCompany();
                objSelect.ShowDialog();
                if (objSelect.strCompCode != "")
                {
                    //strDataBaseFile = "A" + objSelect.strCompCode;
                    if (strDataBaseFile != "")
                    {
                        MainPage.ChangeDataBase(strDataBaseFile);
                    }
                }
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        private void accessories_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TransferRecord objTransfer = new TransferRecord();
                objTransfer.MdiParent = this;
                objTransfer.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Transfer Record in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
                       
        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllOpenForms();
        }
               
        private void printMultiPartyLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                PrintMultiLedger objPrintMultiLedger = new PrintMultiLedger();
                objPrintMultiLedger.MdiParent = this;
                objPrintMultiLedger.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening for Print Multi Ledger  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        
        private void purchaseSlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseOutstandingSlip objPOSlip = new PurchaseOutstandingSlip();
                objPOSlip.MdiParent = this;
                objPOSlip.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of  Purchase Slip  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void creditorsOrDebitorsAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                DebitorsCreditorsAccount objAccount = new DebitorsCreditorsAccount();
                objAccount.MdiParent = this;
                objAccount.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Debtors and Creditors for Goods in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        
        private void adminPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                AdminPanel aPanel = new AdminPanel();
                aPanel.MdiParent = this;
                aPanel.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of  Admin Panel  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        //private void courierRegisterReportToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}
        
        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ChangePassword password = new ChangePassword();
                password.MdiParent = this;
                password.Show();
            }
            catch
            {

            }
        }

        private void ledgerAccountToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                if (strClientName == "LOTUS")
                {
                    LedgerAccount_Remark objLedgerAccount = new LedgerAccount_Remark(true);
                    objLedgerAccount.MdiParent = this;
                    objLedgerAccount.Show();
                }
                else
                {
                    LedgerAccount mlm = new LedgerAccount(true);
                    mlm.MdiParent = this;
                    mlm.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Multiple Company Ledger  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void generalInterestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                InterestStatement objStatement = new InterestStatement(true);
                objStatement.MdiParent = this;
                objStatement.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Multi Company Interest  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }              

        private void purchaseOutstandingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseOutstandingSlip objPOSlip = new PurchaseOutstandingSlip(true);
                objPOSlip.MdiParent = this;
                objPOSlip.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of Multiple Purchase outstanding  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void adjustMultiCompanyLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                if (MainPage.strUserRole.Contains("ADMIN") || MainPage.strLoginName == "MANMOHAN" || MainPage.strLoginName == "TITOO")
                {
                    AdjustMultiFinancialYear adjustLedger = new AdjustMultiFinancialYear();
                    adjustLedger.MdiParent = this;
                    adjustLedger.Show();
                }
                else
                {
                    AdjustMultiCompanyLedger adjustLedger = new AdjustMultiCompanyLedger();
                    adjustLedger.MdiParent = this;
                    adjustLedger.Show();
                }
            }
            catch
            {

            }
        }

        private void reportSummeryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    ShowReportSummary objSummary = new ShowReportSummary();
                    objSummary.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummary.ShowInTaskbar = true;
                    objSummary.Show();
                }
            }
            catch
            {
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ShowAmountLimit showAmount = new ShowAmountLimit();
                showAmount.MdiParent = this;
                showAmount.Show();
            }
            catch
            {

            }
        }

        private void partyRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ShowPartyMaster partyMaster = new ShowPartyMaster();
                partyMaster.MdiParent = this;
                partyMaster.Show();
            }
            catch
            {
            }
        }

        private void partyWiseSalePurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PartyWiseSalePurchase objParty = new PartyWiseSalePurchase();
                objParty.MdiParent = this;
                objParty.Show();
            }
            catch
            {
            }
        }

        private void newEntryToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CourierBookIN objCourierBookIN = new CourierBookIN();
                objCourierBookIN.MdiParent = this;
                objCourierBookIN.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of  Courier Register In  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CourierBookOut objCourierBookOut = new CourierBookOut();
                objCourierBookOut.MdiParent = this;
                objCourierBookOut.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Opening of  Courier Register  out in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

       

        private void showSMSReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SMSReportRegister objSMS = new SMSReportRegister();
                objSMS.MdiParent = this;
                objSMS.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in SMS Report Register  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void creditorsDebitorsAcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                DebitorsCreditorsAccount objDebitors = new DebitorsCreditorsAccount(true);
                objDebitors.MdiParent = this;
                objDebitors.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Debitors Creditor sAccount  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void showCurrentBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ShowCurrentLedgerBalance objBalance = new ShowCurrentLedgerBalance();
                objBalance.MdiParent = this;
                objBalance.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Debitors Creditor sAccount  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void mStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void goodscumPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GoodscumPurchase objGoodscumPurchase = new GoodscumPurchase();
                objGoodscumPurchase.MdiParent = this;
                objGoodscumPurchase.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Goods cum Purchase in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void gstSummaryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GSTSummary objGSTSummary = new GSTSummary();
                objGSTSummary.MdiParent = this;
                objGSTSummary.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GST Summary in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void gstr1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GSTR_1_Summary objGSTSummary = new GSTR_1_Summary();
                objGSTSummary.MdiParent = this;
                objGSTSummary.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GST Summary in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void sendDataToInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SendInternet objSendInternet = new SendInternet();
                objSendInternet.MdiParent = this;
                objSendInternet.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in send Internet  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void showBlackListTransactionLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BlackListReport objBlackListReport = new BlackListReport();
                objBlackListReport.MdiParent = this;
                objBlackListReport.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in send Internet  in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

       
        private void dayBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                DayBookRegister objDayBook = new DayBookRegister();
                objDayBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objDayBook.ShowInTaskbar = true;
                objDayBook.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Day Book Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        private void receiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void MainPageKeyDownEvent(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape)
                _escCount = 0;
            if (e.KeyCode == Keys.Enter && ledgerPanel.Visible)
                SendKeys.Send("{TAB}");
            else if (e.KeyCode == Keys.Escape)
            {
                if (ledgerPanel.Visible || partyPanel.Visible || datePanel.Visible)
                {
                    if (datePanel.Visible)
                    {
                        datePanel.Visible = false;
                    }
                    else
                    {
                        ledgerPanel.Visible = false;
                        partyPanel.Visible = false;

                        if (this.HasChildren)
                        {
                            this.MdiChildren[this.MdiChildren.Length - 1].Controls[0].Focus();
                        }
                    }
                }
                else
                {
                    Form[] objChildForm = this.MdiChildren;
                    if (objChildForm.Length == 0)
                    {
                        if (_escCount == 2)
                            this.Close();
                        _escCount++;
                    }
                }
            }
            else
            {                

                if (MainPage.strSoftwareType == "AGENT")
                {                  
                     if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F4)
                    {
                        ItemGroupMaster objItemGroupMaster = new ItemGroupMaster();
                        objItemGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objItemGroupMaster.ShowInTaskbar = true;
                        objItemGroupMaster.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.J)
                    {
                        SalesBookRegisters objSaleRegister = new SalesBookRegisters();
                        objSaleRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleRegister.ShowInTaskbar = true;
                        objSaleRegister.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.K)
                    {
                        GoodsReceiveRegister objPurchaseRegister = new GoodsReceiveRegister();
                        objPurchaseRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurchaseRegister.ShowInTaskbar = true;
                        objPurchaseRegister.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
                    {
                        OrderBookingRegister objOrderRegister = new OrderBookingRegister();
                        objOrderRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderRegister.ShowInTaskbar = true;
                        objOrderRegister.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
                    {
                        StockRegister objStockRegister = new StockRegister();
                        objStockRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStockRegister.ShowInTaskbar = true;
                        objStockRegister.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.M)
                    {
                        SMSReportRegister objSMSReport = new SMSReportRegister();
                        objSMSReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSMSReport.ShowInTaskbar = true;
                        objSMSReport.Show();
                    }
                     else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
                    {
                        EmailRegister objEmailRegister = new EmailRegister();
                        objEmailRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objEmailRegister.ShowInTaskbar = true;
                        objEmailRegister.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F10)
                    {
                        MarketerMaster objSalesManMaster = new MarketerMaster();
                        objSalesManMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSalesManMaster.ShowInTaskbar = true;
                        objSalesManMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F7)
                    {
                        if (bArticlewiseOpening)
                        {
                            ItemMaster objItemMaster = new ItemMaster();
                            objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objItemMaster.ShowInTaskbar = true;
                            objItemMaster.Show();
                        }
                        else
                        {
                            DesignMaster objDesignMaster = new DesignMaster();
                            objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objDesignMaster.ShowInTaskbar = true;
                            objDesignMaster.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F7)
                    {
                        GoodscumPurchase objPurchaseBook = new GoodscumPurchase();
                        objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurchaseBook.ShowInTaskbar = true;
                        objPurchaseBook.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F8)
                    {
                        SaleBook objSaleBook = new SaleBook();
                        objSaleBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleBook.ShowInTaskbar = true;
                        objSaleBook.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F8)
                    {
                        PurchaseReturnRegister objPurReturnReg = new PurchaseReturnRegister();
                        objPurReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurReturnReg.ShowInTaskbar = true;
                        objPurReturnReg.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F9)
                    {
                        GoodsReceiveAdjustment objOrderAdjustment = new GoodsReceiveAdjustment();
                        objOrderAdjustment.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderAdjustment.ShowInTaskbar = true;
                        objOrderAdjustment.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F9)
                    {
                        StationMaster objStationMaster = new StationMaster();
                        objStationMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStationMaster.ShowInTaskbar = true;
                        objStationMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F10)
                    {
                        TransportMaster objTransportMaster = new TransportMaster();
                        objTransportMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTransportMaster.ShowInTaskbar = true;
                        objTransportMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F11)
                    {
                        JournalEntry_New objJournal = new JournalEntry_New();
                        objJournal.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objJournal.ShowInTaskbar = true;
                        objJournal.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F12)
                    {
                        SaleReturn objSaleReturn = new SaleReturn();
                        objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleReturn.ShowInTaskbar = true;
                        objSaleReturn.Show();
                    }
                    else if (e.KeyCode == Keys.F9)
                    {
                        if (MainPage.mymainObject.bPurchaseSlip && MainPage.mymainObject.bMultiCompany)
                        {
                            PurchaseOutstandingSlip objPurchase = new PurchaseOutstandingSlip(true);
                            objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objPurchase.TopLevel = true;
                            objPurchase.Show();
                        }                     

                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        if (MdiChildren.Length == 0 && !partyPanel.Visible && !ledgerPanel.Visible)
                        {
                            if ((MainPage.mymainObject.bCashView))
                            {
                                CashBook objCash = new CashBook();
                                objCash.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objCash.ShowInTaskbar = true;
                                objCash.Show();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F2)
                    {
                        if (MdiChildren.Length == 0 && !partyPanel.Visible && !ledgerPanel.Visible)
                        {
                            if ((MainPage.mymainObject.bCashView))
                            {
                                BankBook objBank = new BankBook();
                                objBank.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objBank.ShowInTaskbar = true;
                                objBank.Show();
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F6)
                    {
                        OrderBooking objOrderBook = new OrderBooking();
                        objOrderBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderBook.ShowInTaskbar = true;
                        objOrderBook.Show();
                    }
                    else if (e.KeyCode == Keys.F3)
                    {
                        if (partyPanel.Visible || ledgerPanel.Visible)
                        {
                            ShowSupplierMaster();
                        }
                        else
                        {
                            if ((MainPage.mymainObject.bPartyMasterView))
                            {
                                SupplierMaster objSupplier = new SupplierMaster("Update");
                                objSupplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSupplier.TopLevel = true;
                                objSupplier.ShowInTaskbar = true;
                                objSupplier.Show();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F4)
                    {
                        if (MainPage.strPartyLeder == "YES")
                        {
                            ShowPartyMasterSummary objSummary = new ShowPartyMasterSummary();
                            objSummary.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSummary.TopLevel = true;
                            objSummary.ShowInTaskbar = true;
                            objSummary.Show();
                        }
                    }                   
                    else if (e.KeyCode == Keys.F7)
                    {
                        if (bCourierAdd)
                        {
                            CourierBookIN objCourier = new CourierBookIN(true);
                            objCourier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objCourier.Focus();
                            objCourier.Show();
                        }
                    }
                    else if (e.KeyCode == Keys.F8)
                    {
                        if ((MainPage.mymainObject.bReportSummary))
                        {
                            if (partyPanel.Visible)
                            {
                                ShowReportSummary();
                            }
                            else
                            {
                                ShowReportSummary objshowAllReport = new ShowReportSummary();
                                objshowAllReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objshowAllReport.TopLevel = true;
                                objshowAllReport.Show();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F10)
                    {
                        if ((MainPage.mymainObject.bPurchaseSlip))
                        {
                            PurchaseOutstandingSlip objPurchase = new PurchaseOutstandingSlip();
                            objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objPurchase.TopLevel = true;
                            objPurchase.Show();
                        }
                    }
                    else if (e.KeyCode == Keys.F11)
                    {
                        System.Diagnostics.Process.Start("calc.exe");
                    }
                    else if (e.KeyCode == Keys.F12)
                    {
                        if (MainPage.mymainObject.bLedgerReport && MainPage.mymainObject.bMultiCompany)
                        {
                            InterestStatement objInterestStatement = new InterestStatement(true);
                            objInterestStatement.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objInterestStatement.TopLevel = true;
                            objInterestStatement.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.W)
                    {
                        PurchaseReturn_Trading objSaleReturn = new PurchaseReturn_Trading();
                        objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleReturn.ShowInTaskbar = true;
                        objSaleReturn.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Y)
                    {
                        SaleReturnRegister objSaleReturnReg = new SaleReturnRegister();
                        objSaleReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleReturnReg.ShowInTaskbar = true;
                        objSaleReturnReg.Show();
                    }                    
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.N)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            Salesman_Report objSalesManReport = new Salesman_Report();
                            objSalesManReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSalesManReport.ShowInTaskbar = true;
                            objSalesManReport.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.G)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            AlterationSlipRegister objAltSlipReg = new AlterationSlipRegister();
                            objAltSlipReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objAltSlipReg.ShowInTaskbar = true;
                            objAltSlipReg.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.J)
                    {
                        StockTransferRegister objStockTransReg = new StockTransferRegister();
                        objStockTransReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStockTransReg.ShowInTaskbar = true;
                        objStockTransReg.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.L)
                    {
                        EditLogReport objEditLogReport = new EditLogReport();
                        objEditLogReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objEditLogReport.ShowInTaskbar = true;
                        objEditLogReport.Show();
                    }
                    else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F1)
                    {
                        if (MainPage.mymainObject.bAccessories)
                        {
                            FormDashboard objDashBoard = new FormDashboard();
                            objDashBoard.MdiParent = this;
                            objDashBoard.Show();
                        }
                        else
                        { MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }
                    if (e.KeyCode == Keys.F5)
                    {
                        if (MainPage.strPartyLeder == "YES")
                        {

                            if (!ledgerPanel.Visible)
                            {
                                ShowPartyNameWithBalance();
                            }
                            else
                            {
                                ledgerPanel.Visible = false;
                                if (this.HasChildren)
                                {
                                    this.MdiChildren[this.MdiChildren.Length - 1].Controls[0].Focus();
                                }
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F6)
                    {
                        if (bLedgerReport)
                        {
                            LedgerAccount objLedger = new LedgerAccount(true);
                            objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objLedger.ShowInTaskbar = true;
                            objLedger.Focus();
                            objLedger.Show();
                        }
                    }
                }
                else
                {
                    if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F1)
                    {
                        if (MdiChildren.Length == 0 && !partyPanel.Visible && !ledgerPanel.Visible)
                        {
                            if ((MainPage.mymainObject.bCashAdd))
                            {
                                CashBook objCash = new CashBook(1);
                                objCash.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objCash.ShowInTaskbar = true;
                                objCash.Show();
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F2)
                    {
                        BankBook objBank = new BankBook();
                        objBank.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objBank.ShowInTaskbar = true;
                        objBank.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F3)
                    {
                        SupplierMaster objSupplierMaster = new SupplierMaster();
                        objSupplierMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSupplierMaster.ShowInTaskbar = true;
                        objSupplierMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F4)
                    {
                        UnitMaster objUnitMaster = new UnitMaster();
                        objUnitMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objUnitMaster.ShowInTaskbar = true;
                        objUnitMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F5)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            //if (Screen.PrimaryScreen.Bounds.Width < 1100)
                            //{
                            //    SaleBook_Retail_POS objSaleBill_Retail = new SaleBook_Retail_POS();
                            //    objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            //    objSaleBill_Retail.ShowInTaskbar = true;
                            //    objSaleBill_Retail.Show();
                            //}
                            //else
                            {
                                SaleBook_Retail objSaleBill_Retail = new SaleBook_Retail();
                                objSaleBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSaleBill_Retail.ShowInTaskbar = true;
                                objSaleBill_Retail.Show();
                            }
                        }
                        else if (MainPage.strSoftwareType == "TRADING")
                        {
                            if (MainPage._bCustomPurchase)
                            {
                                SaleBook_Retail_Custom objSale = new SaleBook_Retail_Custom();
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                            else
                            {
                                SaleBook_Trading objSale = new SaleBook_Trading();
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F7)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            PurchaseBook_Retail_Merge objPurchaseBill_Retail = new PurchaseBook_Retail_Merge();
                            objPurchaseBill_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objPurchaseBill_Retail.ShowInTaskbar = true;
                            objPurchaseBill_Retail.Show();
                        }
                        else if (MainPage.strSoftwareType == "TRADING")
                        {
                            if (MainPage._bCustomPurchase)
                            {
                                PurchaseBook_Retail_Custom objPurchase = new PurchaseBook_Retail_Custom();
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                            else
                            {
                                PurchaseBook_Trading objPurchase = new PurchaseBook_Trading();
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F6)
                    {
                        BrandMaster objBrandMaster = new BrandMaster();
                        objBrandMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objBrandMaster.ShowInTaskbar = true;
                        objBrandMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F8)
                    {
                        CustomPurchaseReport objCustomPurReport = new CustomPurchaseReport();
                        objCustomPurReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCustomPurReport.ShowInTaskbar = true;
                        objCustomPurReport.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F9)
                    {
                        CustomSaleRegister objCustomSaleReg = new CustomSaleRegister();
                        objCustomSaleReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objCustomSaleReg.ShowInTaskbar = true;
                        objCustomSaleReg.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F10)
                    {
                        StockRegister objStockReg = new StockRegister();
                        objStockReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStockReg.ShowInTaskbar = true;
                        objStockReg.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F11)
                    {
                        VariantMaster objVariantMaster = new VariantMaster("1", StrCategory1);
                        objVariantMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objVariantMaster.ShowInTaskbar = true;
                        objVariantMaster.Show();

                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F12)
                    {
                        VariantMaster objVariantMaster = new VariantMaster("2", StrCategory2);
                        objVariantMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objVariantMaster.ShowInTaskbar = true;
                        objVariantMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F4)
                    {
                        ItemGroupMaster objItemGroupMaster = new ItemGroupMaster();
                        objItemGroupMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objItemGroupMaster.ShowInTaskbar = true;
                        objItemGroupMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F6)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            SalesManMaster objSalesManMaster = new SalesManMaster();
                            objSalesManMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSalesManMaster.ShowInTaskbar = true;
                            objSalesManMaster.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F7)
                    {
                        if (bArticlewiseOpening)
                        {
                            ItemMaster objItemMaster = new ItemMaster();
                            objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objItemMaster.ShowInTaskbar = true;
                            objItemMaster.Show();
                        }
                        else
                        {
                            DesignMaster objDesignMaster = new DesignMaster();
                            objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objDesignMaster.ShowInTaskbar = true;
                            objDesignMaster.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F8)
                    {
                        MaterialCenterMaster objMaterialCenter = new MaterialCenterMaster();
                        objMaterialCenter.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objMaterialCenter.ShowInTaskbar = true;
                        objMaterialCenter.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F9)
                    {
                        StationMaster objStationMaster = new StationMaster();
                        objStationMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStationMaster.ShowInTaskbar = true;
                        objStationMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F10)
                    {
                        TransportMaster objTransportMaster = new TransportMaster();
                        objTransportMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objTransportMaster.ShowInTaskbar = true;
                        objTransportMaster.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F11)
                    {
                        JournalEntry_New objJournal = new JournalEntry_New();
                        objJournal.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objJournal.ShowInTaskbar = true;
                        objJournal.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F12)
                    {                       
                            SaleReturn_Retail objSaleReturn_Retail = new SaleReturn_Retail();
                            objSaleReturn_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSaleReturn_Retail.ShowInTaskbar = true;
                            objSaleReturn_Retail.Show();                        
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.W)
                    {
                        PurchaseReturn_Trading objSaleReturn_Retail = new PurchaseReturn_Trading();
                        objSaleReturn_Retail.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleReturn_Retail.ShowInTaskbar = true;
                        objSaleReturn_Retail.Show();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            AlterationSlip objAltSLip = new AlterationSlip();
                            objAltSLip.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objAltSLip.ShowInTaskbar = true;
                            objAltSLip.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F3)
                    {
                        StockTransferVoucher objStockTrnsfer = new StockTransferVoucher();
                        objStockTrnsfer.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStockTrnsfer.ShowInTaskbar = true;
                        objStockTrnsfer.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Y)
                    {
                        SaleReturnRegister objSaleReturnReg = new SaleReturnRegister();
                        objSaleReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleReturnReg.ShowInTaskbar = true;
                        objSaleReturnReg.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.V)
                    {
                        PurchaseReturnRegister objPurReturnReg = new PurchaseReturnRegister();
                        objPurReturnReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objPurReturnReg.ShowInTaskbar = true;
                        objPurReturnReg.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.N)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            Salesman_Report objSalesManReport = new Salesman_Report();
                            objSalesManReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSalesManReport.ShowInTaskbar = true;
                            objSalesManReport.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.I)
                    {
                        if (MainPage.strSoftwareType == "RETAIL")
                        {
                            AlterationSlipRegister objAltSlipReg = new AlterationSlipRegister();
                            objAltSlipReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objAltSlipReg.ShowInTaskbar = true;
                            objAltSlipReg.Show();
                        }
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.J)
                    {
                        StockTransferRegister objStockTransReg = new StockTransferRegister();
                        objStockTransReg.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objStockTransReg.ShowInTaskbar = true;
                        objStockTransReg.Show();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.L)
                    {
                        if (MainPage.mymainObject.bAccessories)
                        {
                            EditLogReport objEditLogReport = new EditLogReport();
                            objEditLogReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objEditLogReport.ShowInTaskbar = true;
                            objEditLogReport.Show();
                        }
                        else
                            MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F1)
                    {
                        if (MainPage.mymainObject.bAccessories)
                        {
                            FormDashboard objDashBoard = new FormDashboard();
                            objDashBoard.MdiParent = this;
                            objDashBoard.Show();
                        }
                        else
                        { MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }
                    if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F5)
                    {
                        if (MainPage.strPartyLeder == "YES")
                        {

                            if (!ledgerPanel.Visible)
                            {
                                ShowPartyNameWithBalance();
                            }
                            else
                            {
                                ledgerPanel.Visible = false;
                                if (this.HasChildren)
                                {
                                    this.MdiChildren[this.MdiChildren.Length - 1].Controls[0].Focus();
                                }
                            }
                        }
                    }
                    else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F6)
                    {
                        if (bLedgerReport)
                        {
                            LedgerAccount objLedger = new LedgerAccount(true);
                            objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objLedger.ShowInTaskbar = true;
                            objLedger.Focus();
                            objLedger.Show();
                        }
                    }                   

                }
            }
        }

       
        private void MainPage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                MainPageKeyDownEvent(e);
            }
            catch
            {
            }
        }

        private string GetPartyName()
        {
            string strParty = "";
            if (ledgerPanel.Visible)
            {
                strParty = Convert.ToString(lboxParty.SelectedItem);
            }
            else if (partyPanel.Visible)
            {
                strParty = Convert.ToString(partyBox.SelectedItem);
            }
            return strParty;
        }

        private void GetBalanceAmount()
        {
            try
            {
                string strQuery = "Select (AreaCode+CAST(AccountNo as varchar)+' '+Name) Name,GroupName,Category,MobileNo,Address,PinCode,Station,BlackList,(Select SUM(Amt)  from ( "
                                      + " Select ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='DEBIT' and AccountID=(AreaCode+CAST(AccountNo as varchar))  Union All  "
                                      + " Select -ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='CREDIT'  and AccountID=(AreaCode+CAST(AccountNo as varchar)) "
                                      + " )Bal) Amount  from SupplierMaster where GroupName!='SUB PARTY' order by Name ";

                dtPartyBalanceTable = dba.GetDataTable(strQuery);
            }
            catch
            {
            }
        }

        private void ShowPartyNameWithBalance()
        {
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            ledgerPanel.Visible = true;
            txtParty.Clear();
            txtParty.Focus();
            GetBalanceAmount();
            BindSearchListData();
        }

        private void BindSearchListData()
        {
            try
            {
                if (dtPartyBalanceTable != null)
                {
                    if (txtParty.Text == "")
                    {
                        lboxParty.Items.Clear();
                        foreach (DataRow dr in dtPartyBalanceTable.Rows)
                        {
                            lboxParty.Items.Add(Convert.ToString(dr["Name"]));
                        }
                        if (lboxParty.Items.Count > 0)
                        {
                            lboxParty.SelectedIndex = 0;
                        }
                    }
                    else
                    {

                        DataRow[] filteredRows = dtPartyBalanceTable.Select("Name LIKE ('%"+txtParty.Text+"%')");
                        if (filteredRows.Length > 0)
                        {
                            lboxParty.Items.Clear();
                            foreach (DataRow dr in filteredRows)
                            {                                
                                    lboxParty.Items.Add(Convert.ToString(dr["Name"]));
                            }
                            if (lboxParty.Items.Count > 0)
                            {
                                lboxParty.SelectedIndex = 0;
                            }

                        }
                    }
                    lboxParty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bindning Search List data  in Main Page ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
             

        private void GetPartyRecordWithoutBalance()
        {
            try
            {
                dtPartyName = dba.GetDataTable("Select (AreaCode+CAST(AccountNo as varchar)+' '+Name) Name,GroupName,BlackList from SupplierMaster where GroupName!='SUB PARTY' order by Name");
            }
            catch
            {
            }
        }


        private void BindSearchListWithPartyData()
        {
            try
            {
                if (dtPartyName != null)
                {
                    if (txtPartyName.Text == "")
                    {
                        partyBox.Items.Clear();
                        foreach (DataRow dr in dtPartyName.Rows)
                        {
                            partyBox.Items.Add(dr["Name"]);
                        }
                        if (partyBox.Items.Count > 0)
                        {
                            partyBox.SelectedIndex = 0;
                        }
                    }
                    else
                    {

                        DataRow[] filteredRows = dtPartyName.Select("Name LIKE ('%" + txtPartyName.Text + "%')");
                        if (filteredRows.Length > 0)
                        {
                            partyBox.Items.Clear();
                            foreach (DataRow dr in filteredRows)
                            {
                                partyBox.Items.Add(dr["Name"]);
                            }
                            if (partyBox.Items.Count > 0)
                            {
                                partyBox.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bindning Search List data  in Main Page ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtParty_TextChanged(object sender, EventArgs e)
        {
            BindSearchListData();
        }

        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    lboxParty.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lboxParty.Focus();
                }
                else if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("ALLPARTY");
                    if (strData != "")
                        txtParty.Text = strData;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    datePanel.Visible = true;
                    datePanel.BringToFront();
                    txtFromDate.Focus();
                }
            }
            catch
            {
            }
        }

        private void lboxParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strParty = Convert.ToString(lboxParty.SelectedItem);
                if (strParty != "")
                {
                    DataRow[] dr = dtPartyBalanceTable.Select(string.Format("Name='" + strParty + "'"));
                    if (dr.Length > 0)
                    {
                        BindPartyRecordFromDataRow(dr[0]);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindPartyRecordFromDataRow(DataRow dr)
        {
            try
            {
                string strMobileNo = Convert.ToString(dr["MobileNo"]), strPinCode = Convert.ToString(dr["PINCode"]), strStation = Convert.ToString(dr["Station"]);
                double dAmt = dba.ConvertObjectToDouble(dr["Amount"]);
                lblGroup.Text = dr["GroupName"].ToString().ToUpper();
                lblAddress1.Text = Convert.ToString(dr["Address"]);
                lblLedgerHeader.Text = "SELECT PARTY NAME ("+ Convert.ToString(dr["Category"])+")";

                if (Convert.ToBoolean(dr["BlackList"]))
                    lblAmountHeader.ForeColor=lblAmount.ForeColor = Color.Red;
                else
                    lblAmountHeader.ForeColor = lblAmount.ForeColor = Color.DarkGreen;

                if (lblAddress1.Text == "")
                {
                    if (strMobileNo == "")                    
                        lblAddress1.Text = strStation + " " + strPinCode;
                     else                   
                        lblAddress1.Text = strStation + " " + strPinCode + " Mob : " + strMobileNo;                    
                    lblAddress2.Text = "";
                }
                else
                {
                    if (strMobileNo == "")                    
                        lblAddress2.Text = strStation + " " + strPinCode;                    
                    else                    
                        lblAddress2.Text = strStation + " " + strPinCode + " Mob : " + strMobileNo;                    
                }
                if (dAmt >= 0)
                    lblAmount.Text = dAmt.ToString("N2", indianCurancy) + " Dr";
                else
                    lblAmount.Text = Math.Abs(dAmt).ToString("N2", indianCurancy) + " Cr";
            }
            catch
            {
            }
        }

        private void lboxParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsLetter(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Space))
                {
                    txtParty.Text += e.KeyChar.ToString();
                    txtParty.Focus();
                    txtParty.Select(txtParty.Text.Length, 0);
                }
                else if (e.KeyChar == Convert.ToChar(Keys.Back))
                {
                    txtParty.Focus();
                    txtParty.Select(txtParty.Text.Length, 0);
                }
            }
            catch
            {
            }
        }

        private void lboxParty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                datePanel.Visible = true;
                datePanel.BringToFront();
                txtFromDate.Focus();
            }
        }

        private void ShowSupplierMaster()
        {
            try
            {
                string strParty = "";
                if (ledgerPanel.Visible)
                {
                    strParty = Convert.ToString(lboxParty.SelectedItem);
                }
                else if (partyPanel.Visible)
                {
                    strParty = Convert.ToString(partyBox.SelectedItem);
                }

                if (strParty != "")
                {
                    SupplierMaster supplier = new SupplierMaster(strParty);
                    supplier.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    supplier.ShowInTaskbar = true;
                    supplier.Show();
                }
            }
            catch
            {
            }
        }

        private void ShowLedgerAccount()
        {
            try
            {
                string strParty = "";
                try
                {
                    if (ledgerPanel.Visible)
                    {
                        strParty = Convert.ToString(lboxParty.SelectedItem);
                    }
                    else if (partyPanel.Visible)
                    {
                        strParty = Convert.ToString(partyBox.SelectedItem);
                    }
                }
                catch
                {
                }
                if (strParty != "")
                {
                    LedgerAccount objLedger = new LedgerAccount(true);
                    objLedger.txtParty.Text = strParty;
                    objLedger.GetRelatedpartyDetails();
                    if (objLedger._bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                    {
                        MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objLedger.ShowInTaskbar = true;
                        objLedger.TopLevel = true;
                        objLedger.Focus();
                        objLedger.BringToFront();
                        objLedger.GetMultiQuarterDetails();
                        objLedger.Show();
                    }
                }
            }
            catch
            {
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Backup File (*.bak)|*.bak";
            openFile.ShowDialog();
            txtFile.Text = openFile.FileName;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllOpenForms();
            btnRestore.Enabled = true;
            restorePanel.Visible = true;
            txtPassword.Clear();
            txtFile.Clear();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (txtFile.Text != "" && txtFile.Text.Contains(".BAK"))
                    {
                        if (txtPassword.Text == "SSS@321")
                        {
                            btnRestore.Enabled = false;
                            bool restoreStatus = dba.RestoreBackupWithCommand(txtFile.Text);
                            if (restoreStatus)
                            {
                                MessageBox.Show("Thank you ! Backup Restored Successfully  !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                restorePanel.Visible = false;
                                txtPassword.Clear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Wrong Password ,Please Choose valid Password !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Choose Backup File name !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    DataBaseAccess.SetMultiUserDataBase();
                    MessageBox.Show(ex.Message);
                    // MainPage.con.ChangeDatabase(MainPage.strDataBaseFile);
                    MainPage.ChangeDataBase(MainPage.strDataBaseFile);
                }
            }
            catch
            {
            }
            btnRestore.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            restorePanel.Visible = false;
            txtPassword.Clear();
        }


        private void partyBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtPartyName != null)
                {
                    string strName = Convert.ToString(partyBox.SelectedItem);
                    DataRow[] row = dtPartyName.Select(String.Format("Name='" + strName + "'"));
                    if (row.Length > 0)
                    {
                        lblPartyGroupName.Text = Convert.ToString(row[0]["GroupName"]);
                        if (Convert.ToBoolean(row[0]["BlackList"]))
                            lblPartyGroupName.ForeColor = Color.Red;
                        else
                            lblPartyGroupName.ForeColor = Color.Black;
                    }
                }
            }
            catch
            {
            }
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            btnGO.Enabled = false;
            GetLedgerDateWise();
            btnGO.Enabled = true;
        }

        private void GetLedgerDateWise()
        {
            try
            {
                string strName = "";
                if (ledgerPanel.Visible)
                {
                    strName = Convert.ToString(lboxParty.SelectedItem);
                }
                else if (partyPanel.Visible)
                {
                    strName = Convert.ToString(partyBox.SelectedItem);
                }
                if (strName != ""  && DataBaseAccess.GetLastLoginComputerName())
                {
                    datePanel.Visible = false;
                    if (strClientName == "LOTUS")
                    {
                        LedgerAccount_Remark objLedger = new LedgerAccount_Remark();
                        objLedger.txtParty.Text = strName;
                        objLedger.chkDate.Checked = true;
                        objLedger.txtFromDate.Text = txtFromDate.Text;
                        objLedger.txtToDate.Text = txtToDate.Text;
                        objLedger.GetRelatedpartyDetails();
                        if (objLedger._bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                        {
                            MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            objLedger.GetCurrentQuarterDetails();
                            objLedger.ShowInTaskbar = true;
                            objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objLedger.TopLevel = true;
                            objLedger.BringToFront();
                            objLedger.Focus();
                            objLedger.Show();
                            btnGO.GetNextControl(objLedger, true);
                        }
                    }
                    else
                    {
                        LedgerAccount objLedger = new LedgerAccount();
                        objLedger.txtParty.Text = strName;
                        objLedger.chkDate.Checked = true;
                        objLedger.txtFromDate.Text = txtFromDate.Text;
                        objLedger.txtToDate.Text = txtToDate.Text;
                        objLedger.GetRelatedpartyDetails();
                        if (objLedger._bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                        {
                            MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            objLedger.GetCurrentQuarterDetails();
                            objLedger.ShowInTaskbar = true;
                            objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objLedger.TopLevel = true;
                            objLedger.BringToFront();
                            objLedger.Focus();
                            objLedger.Show();
                            btnGO.GetNextControl(objLedger, true);
                        }
                    }
                }
            }
            catch
            {
                datePanel.Visible = true;
            }

        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    if (partyBox.SelectedIndex > 0)
                    {
                        partyBox.SelectedIndex = partyBox.SelectedIndex - 1;
                    }
                    txtPartyName.SelectionStart = txtPartyName.Text.Length + 1;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (partyBox.SelectedIndex < partyBox.Items.Count - 1)
                    {
                        partyBox.SelectedIndex = partyBox.SelectedIndex + 1;
                    }
                    txtPartyName.SelectionStart = txtPartyName.Text.Length;
                }
                else if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("ALLPARTY");
                    if (strData != "")
                        txtPartyName.Text = strData;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    ShowLedgerAccount();
                }
            }
            catch
            {
            }
        }

        private void txtPartyName_TextChanged(object sender, EventArgs e)
        {
            BindSearchListWithPartyData();
        }

        private void partyBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowLedgerAccount();
            }
        }

        private void datePanel_VisibleChanged(object sender, EventArgs e)
        {
            if (datePanel.Visible)
            {
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
            else
            {
                if (ledgerPanel.Visible)
                {
                    txtParty.Focus();
                }
            }
        }

        private void ShowReportSummary()
        {
            try
            {
                string strName = "", strGroupName = "";
                if (ledgerPanel.Visible)
                {
                    strGroupName = lblGroup.Text;
                    strName = Convert.ToString(lboxParty.SelectedItem);
                }
                else if (partyPanel.Visible)
                {
                    strGroupName = lblPartyGroupName.Text;
                    strName = Convert.ToString(partyBox.SelectedItem);
                }
                if (strGroupName.ToUpper() == "SUNDRY DEBTORS")
                {
                    //ShowReportSummary objReport = new ShowReportSummary(strName);
                    //objReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    //objReport.TopLevel = true;
                    //objReport.ShowInTaskbar = true;
                    //objReport.Show();
                }

            }
            catch
            {
            }
        }
        
        private void purchaseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    PurchaseBookRegister objRegister = new PurchaseBookRegister();
                    objRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objRegister.ShowInTaskbar = true;
                    objRegister.Show();
                }
            }
            catch
            {
            }
        }

        private void courierRegisterReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    CourierBookRegister objRegister = new CourierBookRegister();
                    objRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objRegister.ShowInTaskbar = true;
                    objRegister.Show();
                }
            }
            catch
            {
            }
        }
        

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, true);
        }

        private void showEmailRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                EmailRegister objRegister = new EmailRegister();
                objRegister.MdiParent = this;
                objRegister.Show();
            }
            catch
            {
            }
        }

        private void goodsReceiveAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GoodsReceiveAdjustment objGRAdjustment = new GoodsReceiveAdjustment();
                objGRAdjustment.MdiParent = this;
                objGRAdjustment.Show();
            }
            catch
            {
            }
        }
        
        private void mStrip_MouseEnter(object sender, EventArgs e)
        {
            mStrip.ForeColor = Color.Black;
        }

        private void mStrip_MouseLeave(object sender, EventArgs e)
        {
            mStrip.ForeColor = Color.White;
        }

        private void jounralToolStripMenuItem_Click(object sender, EventArgs e)
        {
             try
            {
                CloseAllOpenForms();
                JournalEntry_New objEntry = new JournalEntry_New();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch 
            {               
            }
        }

        private void cashEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CashBook objEntry = new CashBook();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }

        }

        private void bankEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                BankBook objEntry = new BankBook();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void orderBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                OrderBooking objEntry = new OrderBooking();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void goodsRecieveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GoodsReceipt objEntry = new GoodsReceipt();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void salesBookToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                SaleBook objEntry = new SaleBook();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void purchaseBookToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                PurchaseBook objEntry = new PurchaseBook();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }        

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();               
                SupplierMaster objEntry = new SupplierMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void subPartyMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                NewSubParty objEntry = new NewSubParty(1);
                objEntry.MdiParent = this;
                objEntry.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void cartonMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    CloseAllOpenForms();
            //    CartoneMaster objEntry = new CartoneMaster();
            //    objEntry.MdiParent = this;
            //    objEntry.Show();
            //}
            //catch
            //{
            //}

        }

        private void courierMasterToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                CourierMaster objEntry = new CourierMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

      

        private void marketerMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                if (MainPage.strSoftwareType == "RETAIL")
                {
                    SalesManMaster objSalesManMaster = new SalesManMaster();
                    objSalesManMaster.MdiParent = this;
                    objSalesManMaster.Show();
                }
                else
                {
                    MarketerMaster objEntry = new MarketerMaster();
                    objEntry.MdiParent = this;
                    objEntry.Show();
                }
            }
            catch
            {
            }
        }

        private void stationMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                StationMaster objEntry = new StationMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void trasnportMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                CloseAllOpenForms();
                TransportMaster objEntry = new TransportMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void cartoneTypeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CartonTypeMaster objEntry = new CartonTypeMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void cartoneSizeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CartonSizeMaster objEntry = new CartonSizeMaster();
                objEntry.MdiParent = this;
                objEntry.Show();
            }
            catch
            {
            }
        }

        private void itemGroupMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                ItemGroupMaster objItemGroupMaster = new ItemGroupMaster();
                objItemGroupMaster.MdiParent = this;
                objItemGroupMaster.Show();
            }
            catch
            {
            }
        }

        private void companyDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                CompanyDetails objCompanyDetails = new CompanyDetails();
                objCompanyDetails.MdiParent = this;
                objCompanyDetails.Show();
            }
            catch
            {
            }
        }

        private void pringtingSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                InvoicePrintingConfiguration objInvoicePrintingConfiguration = new InvoicePrintingConfiguration();
                objInvoicePrintingConfiguration.MdiParent = this;
                objInvoicePrintingConfiguration.Show();
            }
            catch
            {
            }
        }

        private void unitMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                UnitMaster objUnitMaster = new UnitMaster();
                objUnitMaster.MdiParent = this;
                objUnitMaster.Show();
            }
            catch
            {
            }
        }

        private void partyGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                GroupMaster objGroupMaster = new GroupMaster();
                objGroupMaster.MdiParent = this;
                objGroupMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Group Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void saleTypeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SalesTypeMaster objSalesTypeMaster = new SalesTypeMaster();
                objSalesTypeMaster.MdiParent = this;
                objSalesTypeMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sales Type Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void purchaeTypeMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseTypeMaster objPurchaseTypeMaster = new PurchaseTypeMaster();
                objPurchaseTypeMaster.MdiParent = this;
                objPurchaseTypeMaster.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Type Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void taxCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                TaxCategory objTaxCategory = new TaxCategory();
                objTaxCategory.MdiParent = this;
                objTaxCategory.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Tax Category Master in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

      

        private void saleReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                SaleReturn objSaleReturn = new SaleReturn();
                objSaleReturn.MdiParent = this;
                objSaleReturn.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sale Return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }//

        private void saleReturnRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    SaleReturnRegister objSaleReurnRegister = new SaleReturnRegister();
                    objSaleReurnRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleReurnRegister.ShowInTaskbar = true;                  
                    objSaleReurnRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Sale Return Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void purchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllOpenForms();
                PurchaseReturn objPurchaseReturn = new PurchaseReturn();
                objPurchaseReturn.MdiParent = this;
                objPurchaseReturn.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Return in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void purchaseReturnRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaseAccess.GetLastLoginComputerName())
                {
                    PurchaseReturnRegister objPurchaseReurnRegister = new PurchaseReturnRegister();
                    objPurchaseReurnRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchaseReurnRegister.ShowInTaskbar = true;
                    objPurchaseReurnRegister.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Purchase Return Register in Main Page", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
      
    }
}

