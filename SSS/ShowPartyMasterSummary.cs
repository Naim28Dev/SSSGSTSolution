using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class ShowPartyMasterSummary : Form
    {
        DataBaseAccess dba;
        ArrayList FolderName;
        string strPartyName = "",strFullPartyName="",strGroupName="";
        #region Variables
        public static int g_rHandle, g_retCode;
        public static bool g_isConnected = false, _cardStatus = false;
        public byte g_Sec;
        public static byte[] g_pKey = new byte[6];
        ComboBox cbPort = new ComboBox();
        string TidNumber, dstrpass1, CardBalance;
        string WriteValue;
        byte Blck2;
        DataTable dtmiss = new DataTable();
        DataRow drmiss;
        #endregion

        public ShowPartyMasterSummary()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            FolderName = new ArrayList();
            GetAllQuarterName();
            SetPermission();
        }

        public ShowPartyMasterSummary(string strParty)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            FolderName = new ArrayList();
            txtPartyName.Text = strParty;
            GetAllQuarterName();
            SetPermission();
            ShowPartyDetails();
        }

        private bool ReadCardData()
        {
            try
            {
                g_Sec = Convert.ToByte("1");
                Blck2 = Convert.ToByte("1");
                string strCardName = SelectCard();
                if (strCardName != "")
                {
                    string strData = ReadBalance();// "DLH9";//
                    if (strData != "" && strData != "0")
                    {
                        strData = strData.Replace("\0", "");
                        strPartyName = strData;
                        GetRecordFromDataBase(strData);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Data not found, your card is blank or put the card on writer on right way  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                    MessageBox.Show("Sorry ! Card not found, Please Put card on writer  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            return false;
        }

        #region Read Value
        private string ReadBalance()
        {
            byte[] PassRead = new byte[16];
            long sto = 0;
            byte vKeyType = 0x00;
            int PhysicalSector = 0;
            int ctr, tmpInt2 = 2;

            vKeyType = ACR120U.ACR120_LOGIN_KEYTYPE_A;
            PhysicalSector = Convert.ToInt16(g_Sec);
            tmpInt2 = Convert.ToInt16(Blck2);
            sto = 30;
            for (ctr = 0; ctr < 6; ctr++)
                g_pKey[ctr] = 0xFF;
            g_retCode = ACR120U.ACR120_Login(g_rHandle, Convert.ToByte(PhysicalSector), Convert.ToInt16(vKeyType),
                                             Convert.ToByte(sto), ref g_pKey[0]);
            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
            tmpInt2 = tmpInt2 + Convert.ToInt16(g_Sec) * 4;

            Blck2 = Convert.ToByte(tmpInt2);

            g_retCode = ACR120U.ACR120_Read(g_rHandle, Blck2, ref PassRead[0]);
            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
            else
            {
                dstrpass1 = "";
                for (ctr = 0; ctr < 16; ctr++)
                {
                    dstrpass1 = dstrpass1 + char.ToString((char)(PassRead[ctr]));
                }
                CardBalance = Convert.ToString(dstrpass1);
                if (CardBalance == "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                    CardBalance = "0";
            }
            return CardBalance;
        }
        #endregion

        #region For Connect
        private void Connect()
        {
            int ctr = 0;
            byte[] FirmwareVer = new byte[31];
            byte[] FirmwareVer1 = new byte[20];
            byte infolen = 0x00;
            string FirmStr;
            ACR120U.tReaderStatus ReaderStat = new ACR120U.tReaderStatus();

            if (g_isConnected)
            {
                //   MessageBox.Show("Device is already connected.");
                return;
            }

            g_rHandle = ACR120U.ACR120_Open(0);
            if (g_rHandle != 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_rHandle));
            else
            {
                g_isConnected = true;
                //Get the DLL version the program is using
                g_retCode = ACR120U.ACR120_RequestDLLVersion(ref infolen, ref FirmwareVer[0]);
                if (g_retCode < 0)
                    MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen) - 1; ctr++)
                        FirmStr = FirmStr + char.ToString((char)(FirmwareVer[ctr]));
                }

                g_retCode = ACR120U.ACR120_Status(g_rHandle, ref FirmwareVer1[0], ref ReaderStat);
                if (g_retCode < 0)
                    MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen); ctr++)
                        if ((FirmwareVer1[ctr] != 0x00) && (FirmwareVer1[ctr] != 0xFF))
                            FirmStr = FirmStr + char.ToString((char)(FirmwareVer1[ctr]));
                }

            }
        }
        #endregion

        #region Select
        private string SelectCard()
        {
            //Variable Declarations
            byte[] ResultSN = new byte[11];
            byte ResultTag = 0x00;
            byte[] TagType = new byte[51];
            int ctr = 0;
            string SN = "";

            g_retCode = ACR120U.ACR120_Select(g_rHandle, ref TagType[0], ref ResultTag, ref ResultSN[0]);
            if (g_retCode < 0)

                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));

            else
            {
                if ((TagType[0] == 4) || (TagType[0] == 5))
                {

                    SN = "";
                    for (ctr = 0; ctr < 7; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }

                }
                else
                {

                    SN = "";
                    for (ctr = 0; ctr < ResultTag; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }

                }
            }
            return SN.Trim();
        }
        #endregion

        private void PartySummaryFromCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void PartySummaryFromCard_Load(object sender, EventArgs e)
        {
            try
            {

                if (_cardStatus)
                {                   
                    grpManual.Visible = false;
                    cbPort.Items.Add("USB1");
                    cbPort.SelectedIndex = 0;
                    Connect();
                    if (!ReadCardData())
                        this.Close();
                }
                else
                {                   
                    grpManual.Visible = true;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void GetRecordFromDataBase(string strPartyName)
        {
            try
            {
                txtNickName.Text = txtGroupName.Text = txtCategory.Text = txtPartyType.Text = txtMobileNo.Text = txtPhoneNo.Text = txtEmailID.Text = txtWhatsappNo.Text = txtReference.Text = txtTransport.Text = txtAmtLimit.Text = txtAdditionalLimit.Text = txtLastSaleAmt.Text = txtSaleDate.Text = txtLastPaymentDetails.Text = txtLastPaymentDate.Text = txtTotalSaleAmt.Text = txtOrderAmt.Text = txtBlackListed.Text = txtTransactionLock.Text = txtSecurityChq.Text = "";

                string strQuery = "";
                strQuery += " Select GroupName,Other as NickName,Category,TINNumber as PartyType,Transport,AmountLimit,ExtendedAmt,MobileNo,PhoneNo,EmailID,Reference,TransactionLock,BlackList,BlackListReason,WhatsappNo,ISNULL(CONVERT(varchar,_BA.Date,103),'') LastPaymentDate,ISNULL(_BA.Amount,'') as LastPayment,CONVERT(varchar,Sale._Date,103)LastSaleDate,(Select MAX(CAST(__BA.Amount as Money)) LastSaleAmt from BalanceAmount __BA Where __BA.Date=Sale._Date and __BA.AccountStatus in ('SALES A/C','PURCHASE A/C') and __BA.AccountID=(SM.AreaCode+SM.AccountNo)) LastSaleAmt,TotalSaleAmount,ISNULL(PendingSaleAmt,0) as PendingSaleAmt, (Select TOP 1 CONVERT(varchar,MAX(CD.Date),103) _ChqDate from ChequeDetails CD Where CD.CreditAccountID=(AreaCode+AccountNo) and ChequeType='SECURITY' and Status='PENDING') _ChqDate,(Select SUM(Amt) from (Select ISNULL(SUM(CAST(Amount as Money)),0)Amt from BalanceAmount BA Where BA.Status='DEBIT' and BA.AccountID=(SM.AreaCode+SM.AccountNo) Union All Select -ISNULL(SUM(CAST(Amount as Money)),0)Amt from BalanceAmount BA Where BA.Status='CREDIT' and (CASE WHEN (BA.Description Not  Like('%CHQ%') AND BA.Description Not Like('%CHEQUE%')) then 1 else BA.ChequeStatus end) =1 and BA.AccountID=(SM.AreaCode+SM.AccountNo)) Balance) BalanceAmt,PendingOrderAmt from SupplierMaster SM OUter Apply (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SOD.AreaCode=SM.AreaCode and SOD.AccountNo=SM.AccountNo) SOD OUTER APPLY (Select VoucherCode,MAX(VoucherNo)VoucherNo from BalanceAmount BA Where VoucherCode Like('%B') and AccountID=(SM.AreaCode+SM.AccountNo) Group by VoucherCode) BA left join BalanceAmount _BA on _BA.VoucherCode=BA.VoucherCode and _BA.VoucherNo=BA.VoucherNo and _BA.AccountID=(SM.AreaCode+SM.AccountNo) OUTER APPLY (Select MAX(Date)_Date,SUM(CAST(Amount as Money))TotalSaleAmount from BalanceAmount Where AccountStatus in ('SALES A/C','PURCHASE A/C') And AccountID=(AreaCode+AccountNo))Sale OUTER APPLY (Select SUM(((CAST(Quantity as Money)-(AdjustedQty+CancelQty))*(Amount/CAST(Quantity as money)))) PendingOrderAmt from OrderBooking Where Status='PENDING' and SalePartyID=(AreaCode+AccountNo))_Order Outer Apply (Select SUM(NetAmount)PendingSaleAmt from GoodsReceive Where SaleBill='PENDING' and SalePartyID=(AreaCode+AccountNo))PendingSale Where (AreaCode+AccountNo)='" + strPartyName + "' "
                         + " Select(AreaCode + AccountNo + ' ' + Name)RelatedPartyName from SupplierMaster SM  Where(SM.AreaCode + SM.AccountNo) <> '" + strPartyName + "' and Other!= '' and Other in (Select SM1.Other from SupplierMaster SM1 WHere(SM1.AreaCode + SM1.AccountNo) = '" + strPartyName + "') "
                         + " Select CONVERT(varchar, Date,103)_Date,VoucherNo,NetAmt,Status,BillType,Date from (Select Date,(BillCode + ' ' + CAST(BIllNo as varchar))VoucherNo, NetAmt, 'Dr' as Status, 'SALESERVICE' as BillType from SaleServiceBook SSB Cross APPLY(Select Top 1 ItemName from SaleServiceDetails SSD Where SSB.BillCode = SSD.BillCode and SSB.BillNo = SSD.BillNo) SSD Where ItemName Like('%INT%') and SalePartyID = '" + strPartyName + "' UNION ALL "
                         + " Select Date, (VoucherCode + ' ' + CAST(VoucherNo as varchar))VoucherNo, Amount, 'Cr' as Status, 'JOURNAL' as BillType from BalanceAmount Where GSTNature = 'OFFICE DISCOUNT' and AccountID = '" + strPartyName + "' )Balance Order by Date desc "
                         + " Select Branch,SUM(NetAmt) _NetAmt from ( "
                         + " Select SubString(SR.BillCode, CHARINDEX('/', SR.BillCode) + 1, (LEN(SR.BillCode) - CHARINDEX('/', SR.BillCode) - 1)) Branch, SUM(CAST(NetAmt as Money)) NetAmt from SalesRecord SR Where SR.SalePartyID = '" + strPartyName + "' Group by BillCode UNION ALL "
                         + " Select SubString(SR.BillCode, CHARINDEX('/', SR.BillCode) + 1, (LEN(SR.BillCode) - CHARINDEX('/', SR.BillCode) - 1)) Branch, SUM(CAST(NetAmt as Money)) NetAmt from SalesBook SR Where SR.SalePartyID = '" + strPartyName + "' Group by BillCode "
                         + " )_Sales Group by Branch Order by _NetAmt desc "
                         + " Select Distinct Replace(Replace(OrderCode,'OD',''),'O','') BranchCode,Marketer from OrderBooking OB Outer APPLY (Select Distinct _OB.OrderCode as OCode ,MAX(_OB.SerialNo)_SerialNo from OrderBooking _OB Where _OB.Marketer!='' and _OB.Marketer not Like('%9TH%')and _OB.SalePartyID='" + strPartyName + "' Group by OrderCode)_OB Where OrderCode=OCode and SerialNo=_SerialNo Order by BranchCode "
                         + " Select BranchCode,SUM(PendingOrderAmt) PendingOrderAmt,SUM(PendignStockAmt) PendignStockAmt from ( "
                         + " Select Replace(Replace(OrderCode, 'OD', ''), 'O', '') BranchCode, SUM(((CAST(Quantity as Money) - (AdjustedQty + CancelQty)) * (Amount / CAST(Quantity as money)))) PendingOrderAmt,0 as PendignStockAmt from OrderBooking OB  Where Status = 'PENDING' and SalePartyID = '" + strPartyName + "' Group by OrderCode UNION ALL "
                         + " Select  SUBSTRING(ReceiptCode, 0, Len(ReceiptCode)) BranchCode,0 PendingOrderAmt, SUM(NetAmount)PendignStockAmt from GoodsReceive GR Where SaleBill = 'PENDING' and SalePartyID = '" + strPartyName + "' Group by ReceiptCode "
                         + " )_Order Group by BranchCode  Order by BranchCode ";

                DataSet _ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (_ds.Tables.Count > 0)
                {
                    BindDataWithGrid(_ds.Tables[0]);
                    BindRelatedParty(_ds.Tables[1]);
                    BindIntDiscDataWithTable(_ds.Tables[2]);
                    BindBranchSaleDataWithTable(_ds.Tables[3]);
                    BindBranchMarketerDataWithTable(_ds.Tables[4]);
                    BindBranchPendingOrderDataWithTable(_ds.Tables[5]);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            strGroupName = "";
            try
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strFullPartyName = txtPartyName.Text;
                    strGroupName = Convert.ToString(row["GroupName"]);
                    txtNickName.Text = Convert.ToString(row["NickName"]);
                    txtGroupName.Text = strGroupName;
                    txtCategory.Text = Convert.ToString(row["Category"]);
                    txtPartyType.Text = Convert.ToString(row["PartyType"]);
                    txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                    txtPhoneNo.Text = Convert.ToString(row["PhoneNo"]);
                    txtEmailID.Text = Convert.ToString(row["EmailID"]);
                    txtWhatsappNo.Text = Convert.ToString(row["WhatsappNo"]);
                    txtReference.Text = Convert.ToString(row["Reference"]);
                    txtTransport.Text = Convert.ToString(row["Transport"]);
                    txtAmtLimit.Text = dba.ConvertObjectToDouble(row["AmountLimit"]).ToString("N2", MainPage.indianCurancy);
                    txtAdditionalLimit.Text = dba.ConvertObjectToDouble(row["ExtendedAmt"]).ToString("N2", MainPage.indianCurancy);
                    txtLastSaleAmt.Text = dba.ConvertObjectToDouble(row["LastSaleAmt"]).ToString("N2", MainPage.indianCurancy);
                    txtSaleDate.Text = Convert.ToString(row["LastSaleDate"]);
                    txtLastPaymentDetails.Text = dba.ConvertObjectToDouble(row["LastPayment"]).ToString("N2", MainPage.indianCurancy);
                    txtLastPaymentDate.Text = Convert.ToString(row["LastPaymentDate"]);
                    txtTotalSaleAmt.Text = dba.ConvertObjectToDouble(row["TotalSaleAmount"]).ToString("N2", MainPage.indianCurancy);
                    txtOrderAmt.Text = dba.ConvertObjectToDouble(row["PendingOrderAmt"]).ToString("N2", MainPage.indianCurancy);
                    txtStockAmt.Text = dba.ConvertObjectToDouble(row["PendingSaleAmt"]).ToString("N2", MainPage.indianCurancy);

                    if (Convert.ToBoolean(row["TransactionLock"]))
                    {
                        txtPartyName.BackColor = txtTransactionLock.BackColor = Color.Gold;
                        txtTransactionLock.Text = "LOCKED";
                    }
                    else
                    {
                        txtTransactionLock.BackColor = Color.White;
                        txtTransactionLock.Text = "";// "ACTIVE";
                        if (txtPartyName.BackColor == Color.Gold)
                            txtPartyName.BackColor = Color.White;
                    }
                    if (Convert.ToBoolean(row["BlackList"]))
                    {
                        txtPartyName.BackColor = txtBlackListed.BackColor = Color.Tomato;
                        txtBlackListed.Text = Convert.ToString(row["BlackListReason"]);
                    }
                    else
                    {
                        txtBlackListed.BackColor = Color.White;
                        txtBlackListed.Text = "";// "ACTIVE";
                        if (txtPartyName.BackColor == Color.Tomato)
                            txtPartyName.BackColor = Color.White;
                    }

                    txtSecurityChq.Text = Convert.ToString(row["_ChqDate"]);
                    if (txtSecurityChq.Text != "")
                        txtPartyName.BackColor = txtSecurityChq.BackColor = Color.LightGreen;
                    else
                    {
                        txtSecurityChq.BackColor = Color.White;
                        if (txtPartyName.BackColor == Color.LightGreen)
                            txtPartyName.BackColor = Color.White;
                    }

                    double dBalanceAmt = dBalanceAmt = dba.ConvertObjectToDouble(row["BalanceAmt"]);
                    if (dBalanceAmt >= 0)
                        txtBalanceAmount.Text = "Balance Amount : " + dBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else
                        txtBalanceAmount.Text = "Balance Amount : " + Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                }
            }
            catch { }
            SetPermission();
        
            if (strGroupName == "SUNDRY DEBTORS")
                btnPurchaseSlip.Enabled = false;
            else if (strGroupName == "SUNDRY CREDITOR")
                btnStatement.Enabled = false;
            else if(strGroupName=="SUB PARTY")
                btnPurchaseSlip.Enabled = btnStatement.Enabled = false;

        }

        private void BindRelatedParty(DataTable dt)
        {
            try
            {
                dgrdRelatedParty.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdRelatedParty.Rows.Add(dt.Rows.Count);
                    int _index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdRelatedParty.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                        dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["RelatedPartyName"];
                        _index++;
                    }
                }
            }
            catch { }
        }

        private void BindIntDiscDataWithTable(DataTable dt)
        {
            try
            {
                dgrdIntDiscount.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdIntDiscount.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdIntDiscount.Rows[_rowIndex].Cells["lddate"].Value = row["_Date"];
                        dgrdIntDiscount.Rows[_rowIndex].Cells["ldvoucherNo"].Value = row["VoucherNo"];
                        dgrdIntDiscount.Rows[_rowIndex].Cells["ldnetAmt"].Value = row["NetAmt"];
                        dgrdIntDiscount.Rows[_rowIndex].Cells["ldstatus"].Value = row["Status"];
                        dgrdIntDiscount.Rows[_rowIndex].Cells["ldBillType"].Value = row["BillType"];
                        _rowIndex++;
                    }
                }               
            }
            catch { }
        }

        private void BindBranchSaleDataWithTable(DataTable dt)
        {
            try
            {
                dgrdBranchSale.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdBranchSale.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdBranchSale.Rows[_rowIndex].Cells["serialNo"].Value = (_rowIndex + 1) + ".";
                        dgrdBranchSale.Rows[_rowIndex].Cells["branchName"].Value = row["Branch"];
                        dgrdBranchSale.Rows[_rowIndex].Cells["saleAmt"].Value = row["_NetAmt"];                     
                        _rowIndex++;
                    }
                }
            }
            catch { }
        }

        private void BindBranchPendingOrderDataWithTable(DataTable dt)
        {
            try
            {
                dgrdOrderPendingAmt.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdOrderPendingAmt.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdOrderPendingAmt.Rows[_rowIndex].Cells["oSNo"].Value = (_rowIndex + 1) + ".";
                        dgrdOrderPendingAmt.Rows[_rowIndex].Cells["oBranchName"].Value = row["BranchCode"];
                        dgrdOrderPendingAmt.Rows[_rowIndex].Cells["oPendingAmt"].Value = row["PendingOrderAmt"];
                        dgrdOrderPendingAmt.Rows[_rowIndex].Cells["stockAmt"].Value = row["PendignStockAmt"];
                        _rowIndex++;
                    }
                }
            }
            catch { }
        }

        private void BindBranchMarketerDataWithTable(DataTable dt)
        {
            try
            {
                dgrdMarketer.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdMarketer.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdMarketer.Rows[_rowIndex].Cells["mSNo"].Value = (_rowIndex + 1) + ".";
                        dgrdMarketer.Rows[_rowIndex].Cells["mBranchName"].Value = row["BranchCode"];
                        dgrdMarketer.Rows[_rowIndex].Cells["mMarketerName"].Value = row["Marketer"];
                        _rowIndex++;
                    }
                }
            }
            catch { }
        }

        private void GetAllQuarterName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";

                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    string[] Folder;
                    Folder = Directory.GetDirectories(strPath);
                    foreach (string folderName in Folder)
                    {
                        FileInfo fi = new FileInfo(folderName);
                        FolderName.Add(fi.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in Analysis Report ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        } 


        private void btnMasterDetails_Click(object sender, EventArgs e)
        {
            btnMasterDetails.Enabled = false;
            try
            {
                SupplierMaster objMaster = new SupplierMaster(strFullPartyName);
                objMaster.FormBorderStyle = FormBorderStyle.FixedDialog;
                objMaster.TopLevel = true;
                objMaster.ShowInTaskbar = true;
                objMaster.Show();
            }
            catch
            {
            }
            btnMasterDetails.Enabled = true;
        }

        private void btnLedgerAccount_Click(object sender, EventArgs e)
        {
            btnLedgerAccount.Enabled = false;
            try
            {
                LedgerAccount objLedgerAccount = new LedgerAccount(strFullPartyName);
                objLedgerAccount.FormBorderStyle = FormBorderStyle.FixedDialog;//.MdiParent = MainPage.mymainObject;
                objLedgerAccount.TopLevel = true;
                objLedgerAccount.ShowInTaskbar = true;
                objLedgerAccount.Show();
            }
            catch
            {
            }
            btnLedgerAccount.Enabled = true;
        }

        private void btnName_Click(object sender, EventArgs e)
        {
            btnName.Enabled = false;
            try
            {
                SearchData objSearch = new SearchData("ALLPARTYNAME", "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                {
                    txtPartyName.Text = objSearch.strSelectedData;
                    string[] strName = objSearch.strSelectedData.Split(' ');
                    if (strName.Length > 0)
                    {
                        strPartyName = strName[0];
                        GetRecordFromDataBase(strPartyName);
                    }
                }
            }
            catch
            {
            }
            btnName.Enabled = true;
        }

        private void ShowPartyDetails()
        {
            if (txtPartyName.Text != "")
            {
                string[] strName = txtPartyName.Text.Split(' ');
                if (strName.Length > 0)
                {
                    strPartyName = strName[0];
                    GetRecordFromDataBase(strPartyName);
                }
            }
        }

        private void txtCashAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTYNAME", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtPartyName.Text = objSearch.strSelectedData;
                        ShowPartyDetails();
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void btnGetOtherDetails_Click(object sender, EventArgs e)
        {
            //btnGetOtherDetails.Enabled = false;
            //btnGetOtherDetails.Text = "Please wait ....";
            //try
            //{
            //    System.Threading.Thread.Sleep(100);
            //    GetOtherDetails();
            //}
            //catch { }
            //btnGetOtherDetails.Enabled = true;
            //btnGetOtherDetails.Text = "&Get Other Details";
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtPartyName.Text;
                    if (strParty != "")
                    {
                        txtPartyName.Text = strParty;
                        dgrdRelatedParty.CurrentCell.Value = strOldParty;

                        string[] strName = txtPartyName.Text.Split(' ');
                        if (strName.Length > 0)
                        {
                            strPartyName = strName[0];
                            GetRecordFromDataBase(strPartyName);
                        }

                    }
                    txtPartyName.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void dgrdIntDiscount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                    ShowDetails();
            }
            catch { }
        }

        private void ShowDetails()
        {

            string strAccount = Convert.ToString(dgrdIntDiscount.CurrentRow.Cells["ldbillType"].Value).ToUpper(), strVoucherNo = Convert.ToString(dgrdIntDiscount.CurrentRow.Cells["ldvoucherNo"].Value).ToUpper();

            if (strAccount != "" && strVoucherNo != "")
            {
                string[] strVoucher = strVoucherNo.Trim().Split(' ');
                if (strVoucher.Length > 0)
                {
                    if (strAccount == "SALESERVICE")
                    {
                        if (strVoucher.Length > 1)
                        {
                            SaleServiceBook objSale = new SaleServiceBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.TopLevel = true;
                            objSale.Show();
                        }
                    }
                    else
                    {
                        if (strVoucher.Length > 1)
                        {
                            JournalEntry_New objJournal = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                            objJournal.FormBorderStyle = FormBorderStyle.FixedSingle;
                            objJournal.ShowInTaskbar = true;
                            objJournal.TopLevel = true;
                            objJournal.Show();
                        }
                    }
                }
            }
        }

        private void txtPartyName_DoubleClick(object sender, EventArgs e)
        {
            if (txtPartyName.Text != "")
                DataBaseAccess.OpenPartyMaster(txtPartyName.Text);
        }

        private void txtAmtLimit_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (txtPartyName.Text!="")
                {
                    string[] strParty = txtPartyName.Text.Split(' ');
                    if (strParty.Length > 1)
                    {
                       string strBranchCode = System.Text.RegularExpressions.Regex.Replace(strParty[0], @"[\d-]", string.Empty);
                        string strAccountNo = strParty[0].Replace(strBranchCode, "");

                        EditTrailDetails objEdit = new EditTrailDetails("PARTYMASTER", strBranchCode, strAccountNo);
                        objEdit.ShowDialog();
                    }
                }
            }
            catch { }
        }

        private void btnPurchaseSlip_Click(object sender, EventArgs e)
        {
            btnPurchaseSlip.Enabled = false;
            try
            {
                PurchaseOutstandingSlip objPOSlip = new PurchaseOutstandingSlip(true);
                objPOSlip.FormBorderStyle = FormBorderStyle.FixedDialog;//.MdiParent = MainPage.mymainObject;
                objPOSlip.TopLevel = true;
                objPOSlip.ShowInTaskbar = true;
                objPOSlip.Show();
            }
            catch
            {
            }
            btnPurchaseSlip.Enabled = true;
        }

        private void btnStatement_Click(object sender, EventArgs e)
        {
            btnStatement.Enabled = false;
            try
            {
                InterestStatement objInterest = new InterestStatement(strFullPartyName, true);
                objInterest.FormBorderStyle = FormBorderStyle.FixedDialog;// .MdiParent = MainPage.mymainObject;
                objInterest.TopLevel = true;
                objInterest.ShowInTaskbar = true;
                objInterest.Show();
            }
            catch
            {
            }
            btnStatement.Enabled = true;
        }

        private void btnSalesReport_Click(object sender, EventArgs e)
        {
            btnSalesReport.Enabled = false;
            try
            {
                if (strFullPartyName != "")
                {
                    SalesBookRegisters objSRegister = new SalesBookRegisters(strFullPartyName);
                    objSRegister.FormBorderStyle = FormBorderStyle.FixedDialog;//.MdiParent = MainPage.mymainObject;
                    objSRegister.TopLevel = true;
                    objSRegister.ShowInTaskbar = true;
                    objSRegister.Show();
                }
                else
                {
                    MessageBox.Show("Sorry ! Please select account name !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnSalesReport.Enabled = true;
        }

        private void btnPurchaseReport_Click(object sender, EventArgs e)
        {
            btnPurchaseReport.Enabled = false;
            try
            {
                if (strFullPartyName != "")
                {
                    PurchaseBookRegister objPRegister = new PurchaseBookRegister(strFullPartyName);
                    objPRegister.FormBorderStyle = FormBorderStyle.FixedDialog;//.MdiParent = MainPage.mymainObject;
                    objPRegister.TopLevel = true;
                    objPRegister.ShowInTaskbar = true;
                    objPRegister.Show();
                }
            }
            catch
            {
            }
            btnPurchaseReport.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetPermission()
        {
            btnMasterDetails.Enabled = MainPage.mymainObject.bPartyMasterView;
            btnLedgerAccount.Enabled = btnStatement.Enabled= MainPage.mymainObject.bLedgerReport;
            btnSalesReport.Enabled = MainPage.mymainObject.bSaleReport;
            btnPurchaseReport.Enabled = MainPage.mymainObject.bPurchaseReport;
            btnPurchaseSlip.Enabled = MainPage.mymainObject.bPurchaseSlip;
        }

    }
}

