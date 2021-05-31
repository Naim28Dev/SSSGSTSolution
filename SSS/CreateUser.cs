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
    public partial class CreateUser : Form
    {
        DataBaseAccess dba;
        DataTable table;
        string strUser = "";

        public CreateUser()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            table = dba.GetLoginName();
            txtLoginName.Focus();
            rdoNone.Checked = true;

            if (MainPage.strUserRole == "SUPERADMIN")
                grpSuperAdmin.Enabled = true;// chkCrediterDebiter.Enabled = chkFASReport.Enabled = chkPrintMultyLedger.Enabled = rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible = false;
            if (MainPage.strLoginName != "ADMIN" && MainPage.strLoginName != "SUPERADMIN")
                rdoMakeAsAdmin.Enabled = rdoMakeasSuperAdmin.Enabled = rdoNone.Enabled = grpPanel.Visible = lblUserType.Visible = false;
            else if (MainPage.strLoginName != "SUPERADMIN")
                rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible = false;

        }

        public CreateUser(string strUpdate)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                table = dba.GetLoginName();
                btnSubmit.Text = "Up&date";
                BindListData();
                txtLoginName.ReadOnly = true;
                btnDelete.Enabled = true;

                if (MainPage.strUserRole == "SUPERADMIN")
                    grpSuperAdmin.Enabled = true;// chkCrediterDebiter.Enabled = chkFASReport.Enabled = chkPrintMultyLedger.Enabled = rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible = false;
                if (MainPage.strLoginName != "ADMIN" && MainPage.strLoginName != "SUPERADMIN")
                    rdoMakeAsAdmin.Enabled = rdoMakeasSuperAdmin.Enabled = rdoNone.Enabled = grpPanel.Visible = lblUserType.Visible = false;
                else if (MainPage.strLoginName != "SUPERADMIN")
                    rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible = false;
                
                //if (MainPage.strUserRole != "SUPERADMIN")
                //    grpSuperAdmin.Enabled = rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible = false;
                //if (MainPage.strLoginName != "ADMIN" && MainPage.strLoginName != "SUPERADMIN")
                //    rdoMakeAsAdmin.Enabled = rdoMakeasSuperAdmin.Enabled = rdoNone.Enabled = grpPanel.Visible = lblUserType.Visible = false;
                //else if (MainPage.strLoginName != "SUPERADMIN")
                //    rdoMakeasSuperAdmin.Enabled = grpPanel.Visible = lblUserType.Visible= false;

            }
            catch
            {
            }
        }

        private void BindListData()
        {
            try
            {

                lboxUser.Items.Clear();

                if (txtLoginName.Text == "")
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        lboxUser.Items.Add(dr[0].ToString());
                    }
                    if (lboxUser.Items.Count > 0)
                    {
                        lboxUser.SelectedIndex = 0;
                    }

                }
                else
                {
                    DataRow[] filteredRows = table.Select(string.Format("{0} LIKE '%{1}%'", "LoginName", txtLoginName.Text));
                    if (filteredRows.Length > 0)
                    {
                        foreach (DataRow dr in filteredRows)
                        {
                            lboxUser.Items.Add(dr[0]);
                        }
                        lboxUser.SelectedIndex = 0;
                        lboxUser.Visible = true;
                    }
                    else
                    {
                        lboxUser.Visible = true;
                    }
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding List of Data in Create User Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CheckAllControls()
        {
            try
            {

                foreach (Control ctrl in panelPermission.Controls)
                {
                    if (ctrl is GroupBox)
                    {
                        if (ctrl.Enabled)
                        {
                            foreach (Control _ctrl in ctrl.Controls)
                            {
                                if (_ctrl is CheckBox)
                                {
                                    if (_ctrl.Enabled)
                                        ((CheckBox)_ctrl).Checked = true;
                                }
                            }
                        }
                    }
                }               
            }
            catch
            {
            }
        }

        private void UncheckAllControls()
        {
            try
            {
                foreach (Control ctrl in panelPermission.Controls)
                {
                    if (ctrl is GroupBox)
                    {
                        foreach (Control _ctrl in ctrl.Controls)
                        {
                            if (_ctrl is CheckBox)
                            {
                                ((CheckBox)_ctrl).Checked = false;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Checked)
            {
                CheckAllControls();
               // chkInternet.Checked = false;
                btnSubmit.Focus();
            }
            else
            {
                UncheckAllControls();
                chkJournalAll.Focus();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtLoginName.Text != "" && txtPassword.Text != "" && txtConfirmPassword.Text != "" && (txtPassword.Text==txtConfirmPassword.Text))
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Save Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        if (btnSubmit.Text == "&Submit")
                        {
                            bool _bStatus=SaveRecord();
                            if (_bStatus)
                            {
                                table = dba.GetLoginName();
                                BindListData();
                            }
                        }
                        else if (btnSubmit.Text == "Up&date")
                        {
                            UpdateRecord();                           
                        }
                      
                    }
                }
                else
                {
                    MessageBox.Show("Login Name and Password cann't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Submit Button in Create User Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string GetUserType()
        {
            if (rdoMakeAsAdmin.Checked)
                return "ADMIN";
            else if (rdoMakeasSuperAdmin.Checked)
                return "SUPERADMIN";
            else if (rdoManager.Checked)
                return "MANAGER";
            else
                return "NONE";
        }

        private bool SaveRecord()
        {
            string[] record = new string[94];

            record[0] = chkAll.Checked.ToString();
            record[1] = txtLoginName.Text;
            record[2] = txtPassword.Text;
            record[3] = txtName.Text;
            record[4] = txtMobileNo.Text;
            record[5] = GetUserType();
            record[6] = chkJournalEntry.Checked.ToString();
            record[7] = chkJournalView.Checked.ToString();
            record[8] = chkJournalEditDelete.Checked.ToString();
            record[9] = chkCashEntry.Checked.ToString();
            record[10] = chkCashView.Checked.ToString();
            record[11] = chkCashEditDelete.Checked.ToString();
            record[12] = chkOrderEntry.Checked.ToString();
            record[13] = chkOrderView.Checked.ToString();
            record[14] = chkOrderSlipEditDelete.Checked.ToString();
            record[15] = chkFullEditControl.Checked.ToString();
            record[16] = chkBulkUpload.Checked.ToString();
            record[17] = chkAddPaymentReq.Checked.ToString();
            record[18] = chkSaleEntry.Checked.ToString();
            record[19] = chkSaleView.Checked.ToString();
            record[20] = chkSaleEditDelete.Checked.ToString();
            record[21] = chkPurchaseEntry.Checked.ToString();
            record[22] = chkPurchaseView.Checked.ToString();
            record[23] = chkPurchaseEditDelete.Checked.ToString();
            record[24] = chkDownloadRequest.Checked.ToString();
            record[25] = chkSendRequest.Checked.ToString();
            record[26] = chkChangePaymentStatus.Checked.ToString();
            record[27] = chkCourierEntry.Checked.ToString();
            record[28] = chkCourierView.Checked.ToString();
            record[29] = chkCourierEditDelete.Checked.ToString();
            record[30] = chkPartyEntry.Checked.ToString();
            record[31] = chkPartyView.Checked.ToString();
            record[32] = chkPartyEditDelete.Checked.ToString();
            record[33] = chkSubPartyEntry.Checked.ToString();
            record[34] = chkSubPartyView.Checked.ToString();
            record[35] = chkSubPartyEditDelete.Checked.ToString();
            record[36] = chkAddAccountMasterEntry.Checked.ToString();
            record[37] = chkAccountMasterView.Checked.ToString();
            record[38] = chkAccountMasterEditDelete.Checked.ToString();
            record[39] = chkMergeEntry.Checked.ToString();
            record[40] = chkCompanyInfo.Checked.ToString();
            record[41] = chkAccessories.Checked.ToString();
            record[42] = chkBackup.Checked.ToString();
            record[43] = chkOrderSilpView.Checked.ToString();
            record[44] = chkFASReport.Checked.ToString();
            record[45] = chkGoodsRecivedReportViewView.Checked.ToString();
            record[46] = chkCourierReoprtView.Checked.ToString();
            record[47] = chkSalesReportView.Checked.ToString();
            record[48] = chkReportSummeryView.Checked.ToString();
            record[49] = chkPurchaseReportView.Checked.ToString();
            record[50] = chkMultiCompanyReportView.Checked.ToString();
            record[51] = chkForwardingReportViewAdd.Checked.ToString();
            record[52] = chkLedgerInterest.Checked.ToString();
            record[53] = chkPrintMultyLedger.Checked.ToString();
            record[54] = chkPurchaseoutStanding.Checked.ToString();
            record[55] = chkCrediterDebiter.Checked.ToString();
            record[56] = chkShowAmontLimit.Checked.ToString();
            record[57] = chkPartyLedger.Checked.ToString();
            record[58] = DateTime.Now.ToString("MM/dd/yyyy");
            record[59] = chkBackDateEntry.Checked.ToString();
            record[60] = chkSMS.Checked.ToString();
            record[61] = chkInternet.Checked.ToString();
            record[62] = chkReminder.Checked.ToString();
            record[63] = chkPrevilegeAccount.Checked.ToString();
            record[64] = chkOnAccountEditDelete.Checked.ToString();
            record[65] = chkSupplierOtherDetails.Checked.ToString();
            record[66] = txtBranchCode.Text;
            record[67] = chkGSTMasterEntry.Checked.ToString();
            record[68] = chkGSTMasterView.Checked.ToString();
            record[69] = chkGSTMasterEditDelete.Checked.ToString();
            record[70] = chkRefrenceMasterEntry.Checked.ToString();
            record[71] = chkRefrenceMasterView.Checked.ToString();
            record[72] = chkRefrenceMasterEditDelete.Checked.ToString();
            record[73] = chkLockUnCustomer.Checked.ToString();
            record[74] = chkSecurityChqPermision.Checked.ToString();
            record[75] = chkAdminPanel.Checked.ToString();
            record[76] = chkChangeSupplierDisc.Checked.ToString();
            record[77] = chkCustmorLimit.Checked.ToString();
            record[78] = chkDashboard.Checked.ToString();
            record[79] = chkBankDetailApprove.Checked.ToString();
            record[80] = chkPartyWiseSP.Checked.ToString();
            record[81] = chkChangeBankDetail.Checked.ToString();
            record[82] = chkBranchWiseSP.Checked.ToString();
            record[83] = chkChangeCustmorDetail.Checked.ToString();
            record[84] = chkShowbankLedger.Checked.ToString();
            record[85] = chkPartyMasterReg.Checked.ToString();
            record[86] = chkGraficalSummry.Checked.ToString();
            record[87] = chkSchemeMaster.Checked.ToString();
            record[88] = chkShowPartyLimit.Checked.ToString();
            record[89] = chkShowAllRecord.Checked.ToString();
            record[90] = chkGSTReport.Checked.ToString();
            record[91] = chkShowEmailReg.Checked.ToString();
            record[92] = chkShowWhatsappReg.Checked.ToString();
            record[93] = chkAddCustomer.Checked.ToString();


            int count = dba.SaveUserAccount(record);
            if (count > 0)
            {
                MessageBox.Show("Thank you ! Record saved successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ClearAllText();
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! We are unable to save record !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void UpdateRecord()
        {
            try
            {
                string[] record = new string[100];

                record[0] = chkAll.Checked.ToString();
                record[1] = txtLoginName.Text;
                record[2] = txtPassword.Text;
                record[3] = txtName.Text;
                record[4] = txtMobileNo.Text;
                record[5] = GetUserType();
                record[6] = chkJournalEntry.Checked.ToString();
                record[7] = chkJournalView.Checked.ToString();
                record[8] = chkJournalEditDelete.Checked.ToString();
                record[9] = chkCashEntry.Checked.ToString();
                record[10] = chkCashView.Checked.ToString();
                record[11] = chkCashEditDelete.Checked.ToString();
                record[12] = chkOrderEntry.Checked.ToString();
                record[13] = chkOrderView.Checked.ToString();
                record[14] = chkOrderSlipEditDelete.Checked.ToString();
                record[15] = chkFullEditControl.Checked.ToString();
                record[16] = chkBulkUpload.Checked.ToString();
                record[17] = chkAddPaymentReq.Checked.ToString();
                record[18] = chkSaleEntry.Checked.ToString();
                record[19] = chkSaleView.Checked.ToString();
                record[20] = chkSaleEditDelete.Checked.ToString();
                record[21] = chkPurchaseEntry.Checked.ToString();
                record[22] = chkPurchaseView.Checked.ToString();
                record[23] = chkPurchaseEditDelete.Checked.ToString();
                record[24] = chkDownloadRequest.Checked.ToString();
                record[25] = chkSendRequest.Checked.ToString();
                record[26] = chkChangePaymentStatus.Checked.ToString();
                record[27] = chkCourierEntry.Checked.ToString();
                record[28] = chkCourierView.Checked.ToString();
                record[29] = chkCourierEditDelete.Checked.ToString();
                record[30] = chkPartyEntry.Checked.ToString();
                record[31] = chkPartyView.Checked.ToString();
                record[32] = chkPartyEditDelete.Checked.ToString();
                record[33] = chkSubPartyEntry.Checked.ToString();
                record[34] = chkSubPartyView.Checked.ToString();
                record[35] = chkSubPartyEditDelete.Checked.ToString();
                record[36] = chkAddAccountMasterEntry.Checked.ToString();
                record[37] = chkAccountMasterView.Checked.ToString();
                record[38] = chkAccountMasterEditDelete.Checked.ToString();
                record[39] = chkMergeEntry.Checked.ToString();
                record[40] = chkCompanyInfo.Checked.ToString();
                record[41] = chkAccessories.Checked.ToString();
                record[42] = chkBackup.Checked.ToString();
                record[43] = chkOrderSilpView.Checked.ToString();
                record[44] = chkFASReport.Checked.ToString();
                record[45] = chkGoodsRecivedReportViewView.Checked.ToString();
                record[46] = chkCourierReoprtView.Checked.ToString();
                record[47] = chkSalesReportView.Checked.ToString();
                record[48] = chkReportSummeryView.Checked.ToString();
                record[49] = chkPurchaseReportView.Checked.ToString();
                record[50] = chkMultiCompanyReportView.Checked.ToString();
                record[51] = chkForwardingReportViewAdd.Checked.ToString();
                record[52] = chkLedgerInterest.Checked.ToString();
                record[53] = chkPrintMultyLedger.Checked.ToString();
                record[54] = chkPurchaseoutStanding.Checked.ToString();
                record[55] = chkCrediterDebiter.Checked.ToString();
                record[56] = chkShowAmontLimit.Checked.ToString();
                record[57] = chkPartyLedger.Checked.ToString();
                record[58] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                record[59] = chkBackDateEntry.Checked.ToString();
                record[60] = chkSMS.Checked.ToString();
                record[61] = chkInternet.Checked.ToString();
                record[62] = chkReminder.Checked.ToString();
                record[63] = chkPrevilegeAccount.Checked.ToString();
                record[64] = chkOnAccountEditDelete.Checked.ToString();
                record[65] = chkSupplierOtherDetails.Checked.ToString();
                record[66] = txtBranchCode.Text;
                record[67] = chkGSTMasterEntry.Checked.ToString();
                record[68] = chkGSTMasterView.Checked.ToString();
                record[69] = chkGSTMasterEditDelete.Checked.ToString();
                record[70] = chkRefrenceMasterEntry.Checked.ToString();
                record[71] = chkRefrenceMasterView.Checked.ToString();
                record[72] = chkRefrenceMasterEditDelete.Checked.ToString();
                record[73] = chkLockUnCustomer.Checked.ToString();
                record[74] = chkSecurityChqPermision.Checked.ToString();
                record[75] = chkAdminPanel.Checked.ToString();
                record[76] = chkChangeSupplierDisc.Checked.ToString();
                record[77] = chkCustmorLimit.Checked.ToString();
                record[78] = chkDashboard.Checked.ToString();
                record[79] = chkBankDetailApprove.Checked.ToString();
                record[80] = chkPartyWiseSP.Checked.ToString();
                record[81] = chkChangeBankDetail.Checked.ToString();
                record[82] = chkBranchWiseSP.Checked.ToString();
                record[83] = chkChangeCustmorDetail.Checked.ToString();
                record[84] = chkShowbankLedger.Checked.ToString();
                record[85] = chkPartyMasterReg.Checked.ToString();
                record[86] = chkGraficalSummry.Checked.ToString();
                record[87] = chkSchemeMaster.Checked.ToString();
                record[88] = chkShowPartyLimit.Checked.ToString();
                record[89] = chkShowAllRecord.Checked.ToString();
                record[90] = chkGSTReport.Checked.ToString();
                record[91] = chkShowEmailReg.Checked.ToString();
                record[92] = chkShowWhatsappReg.Checked.ToString();
                record[93] = chkAddCustomer.Checked.ToString();

                int count = dba.UpdateUserAccount(record);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record Saved Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //ClearAllText();
                }
                else
                {
                    MessageBox.Show("Sorry ! We are unable to Save Record !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding List of Data in Create User Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindDatawithControls(string strUser)
        {
            try
            {
                chkSAALL.Checked = chkAll.Checked = false;
                DataTable dt = dba.GetUserDetails(strUser);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    txtLoginName.Text = Convert.ToString(dr["LoginName"]);
                    txtPassword.Text = Convert.ToString(dr["Password"]);
                    txtConfirmPassword.Text = txtPassword.Text;
                    txtName.Text = Convert.ToString(dr["Name"]);
                    txtMobileNo.Text = Convert.ToString(dr["MobileNo"]);
                    txtBranchCode.Text = Convert.ToString(dr["BranchCode"]);
                    string strUserType = "";
                    if (dt.Columns.Contains("UserType"))
                        strUserType = Convert.ToString(dr["UserType"]);
                    else
                        strUserType = Convert.ToString(dr["Address"]);

                    if (strUserType == "ADMIN")
                        rdoMakeAsAdmin.Checked = true;
                    else if (strUserType == "SUPERADMIN")
                        rdoMakeasSuperAdmin.Checked = true;
                    else if (strUserType == "MANAGER")
                        rdoManager.Checked = true;
                    else
                        rdoNone.Checked = true;

                    chkJournalEntry.Checked = Convert.ToBoolean(dr["JournalEntry"]);
                    chkJournalView.Checked = Convert.ToBoolean(dr["JournalView"]);
                    chkJournalEditDelete.Checked = Convert.ToBoolean(dr["JournalEdit"]);
                    chkCashEntry.Checked = Convert.ToBoolean(dr["CashEntry"]);
                    chkCashView.Checked = Convert.ToBoolean(dr["CashView"]);
                    chkCashEditDelete.Checked = Convert.ToBoolean(dr["CashEdit"]);
                    chkOrderEntry.Checked = Convert.ToBoolean(dr["OrderEntry"]);
                    chkOrderView.Checked = Convert.ToBoolean(dr["OrderView"]);
                    chkOrderSlipEditDelete.Checked = Convert.ToBoolean(dr["OrderEdit"]);
                    chkFullEditControl.Checked = Convert.ToBoolean(dr["GoodsEntry"]);
                    chkBulkUpload.Checked = Convert.ToBoolean(dr["GoodsView"]);
                    chkAddPaymentReq.Checked = Convert.ToBoolean(dr["GoodsEdit"]);
                    chkSaleEntry.Checked = Convert.ToBoolean(dr["SaleEntry"]);
                    chkSaleView.Checked = Convert.ToBoolean(dr["SaleView"]);
                    chkSaleEditDelete.Checked = Convert.ToBoolean(dr["SaleEdit"]);
                    chkPurchaseEntry.Checked = Convert.ToBoolean(dr["PurchaseEntry"]);
                    chkPurchaseView.Checked = Convert.ToBoolean(dr["PurchaseView"]);
                    chkPurchaseEditDelete.Checked = Convert.ToBoolean(dr["PurchaseEdit"]);
                    chkDownloadRequest.Checked = Convert.ToBoolean(dr["ForwardingEntry"]);
                    chkSendRequest.Checked = Convert.ToBoolean(dr["ForwardingView"]);
                    chkChangePaymentStatus.Checked = Convert.ToBoolean(dr["ForwardingEdit"]);
                    chkCourierEntry.Checked = Convert.ToBoolean(dr["CourierEntry"]);
                    chkCourierView.Checked = Convert.ToBoolean(dr["CourierView"]);
                    chkCourierEditDelete.Checked = Convert.ToBoolean(dr["CourierEdit"]);
                    chkPartyEntry.Checked = Convert.ToBoolean(dr["NewParty"]);
                    chkPartyView.Checked = Convert.ToBoolean(dr["PartyView"]);
                    chkPartyEditDelete.Checked = Convert.ToBoolean(dr["NewPartyEdit"]);
                    chkSubPartyEntry.Checked = Convert.ToBoolean(dr["NewSubParty"]);
                    chkSubPartyView.Checked = Convert.ToBoolean(dr["SubPartyView"]);
                    chkSubPartyEditDelete.Checked = Convert.ToBoolean(dr["SubPartyEdit"]);
                    chkAddAccountMasterEntry.Checked = Convert.ToBoolean(dr["NewAccountMaster"]);
                    chkAccountMasterView.Checked = Convert.ToBoolean(dr["AccountMasterView"]);
                    chkAccountMasterEditDelete.Checked = Convert.ToBoolean(dr["AccountMasterEdit"]);
                    chkMergeEntry.Checked = Convert.ToBoolean(dr["Merging"]);
                    chkCompanyInfo.Checked = Convert.ToBoolean(dr["CompanyInfo"]);
                    chkSalesReportView.Checked = Convert.ToBoolean(dr["SalesReportView"]);
                    chkAccessories.Checked = Convert.ToBoolean(dr["Accessories"]);
                    chkBackup.Checked = Convert.ToBoolean(dr["BackupRestore"]);
                    chkPurchaseReportView.Checked = Convert.ToBoolean(dr["PurchaseReport"]);
                    chkOrderSilpView.Checked = Convert.ToBoolean(dr["OrderSlipView"]);
                    chkFASReport.Checked = Convert.ToBoolean(dr["FASReport"]);
                    chkForwardingReportViewAdd.Checked = Convert.ToBoolean(dr["ForwardingReport"]);
                    chkGoodsRecivedReportViewView.Checked = Convert.ToBoolean(dr["GoodsRecivedView"]);
                    chkCourierReoprtView.Checked = Convert.ToBoolean(dr["Reportview"]);
                    chkReportSummeryView.Checked = Convert.ToBoolean(dr["ReportSummeryView"]);
                    chkMultiCompanyReportView.Checked = Convert.ToBoolean(dr["MultiCmpReportview"]);
                    chkPartyLedger.Checked = Convert.ToBoolean(dr["PartyLedger"]);
                    chkPrintMultyLedger.Checked = Convert.ToBoolean(dr["PrintMultiLedger"]);
                    chkLedgerInterest.Checked = Convert.ToBoolean(dr["LedgerInterest"]);
                    chkCrediterDebiter.Checked = Convert.ToBoolean(dr["CrediterDebter"]);
                    chkShowAmontLimit.Checked = Convert.ToBoolean(dr["ShowAmountLimit"]);
                    chkPurchaseoutStanding.Checked = Convert.ToBoolean(dr["PurchaseOutStanding"]);
                    chkBackDateEntry.Checked = Convert.ToBoolean(dr["BackDateEntry"]);
                    chkSMS.Checked = Convert.ToBoolean(dr["SMS"]);
                    chkInternet.Checked = Convert.ToBoolean(dr["Other"]);
                    chkReminder.Checked = Convert.ToBoolean(dr["Reminder"]);
                    chkPrevilegeAccount.Checked = Convert.ToBoolean(dr["DayBook"]);
                    chkOnAccountEditDelete.Checked = Convert.ToBoolean(dr["OnAccount"]);
                    chkSupplierOtherDetails.Checked = Convert.ToBoolean(dr["Extra"]);

                    chkGSTMasterEntry.Checked = chkGSTMasterView.Checked = chkGSTMasterEditDelete.Checked = chkRefrenceMasterEntry.Checked = chkRefrenceMasterView.Checked = chkRefrenceMasterEditDelete.Checked = chkGSTReport.Checked = false;

                    if (dt.Columns.Contains("GSTMasterEntry"))
                    {
                        if (Convert.ToString(dr["GSTMasterEntry"]) != "")
                        {
                            chkGSTMasterEntry.Checked = Convert.ToBoolean(dr["GSTMasterEntry"]);
                            chkGSTMasterView.Checked = Convert.ToBoolean(dr["GSTMasterView"]);
                            chkGSTMasterEditDelete.Checked = Convert.ToBoolean(dr["GSTMasterEditDelete"]);
                            chkRefrenceMasterEntry.Checked = Convert.ToBoolean(dr["RefrenceMasterEntry"]);
                            chkRefrenceMasterView.Checked = Convert.ToBoolean(dr["RefrenceMasterView"]);
                            chkRefrenceMasterEditDelete.Checked = Convert.ToBoolean(dr["RefrenceMasterEditDelete"]);
                            chkLockUnCustomer.Checked = Convert.ToBoolean(dr["LockunLockCustomer"]);
                            chkSecurityChqPermision.Checked = Convert.ToBoolean(dr["SecurityChequePermission"]);
                            chkAdminPanel.Checked = Convert.ToBoolean(dr["AdminPanel"]);
                            chkChangeSupplierDisc.Checked = Convert.ToBoolean(dr["ChangeSupplierDisc"]);
                            chkCustmorLimit.Checked = Convert.ToBoolean(dr["ChangeCustomerLimit"]);
                            chkDashboard.Checked = Convert.ToBoolean(dr["Dashboard"]);
                            chkBankDetailApprove.Checked = Convert.ToBoolean(dr["BankDetailApprove"]);
                            chkPartyWiseSP.Checked = Convert.ToBoolean(dr["PartyWiseSP"]);
                            chkChangeBankDetail.Checked = Convert.ToBoolean(dr["ChangeBankDetail"]);
                            chkBranchWiseSP.Checked = Convert.ToBoolean(dr["BranchWiseSP"]);
                            chkChangeCustmorDetail.Checked = Convert.ToBoolean(dr["ChangeCustomerDetail"]);
                            chkShowbankLedger.Checked = Convert.ToBoolean(dr["ShowBankLedger"]);
                            chkPartyMasterReg.Checked = Convert.ToBoolean(dr["PartyMasterRegister"]);
                            chkGraficalSummry.Checked = Convert.ToBoolean(dr["GraphicalSummary"]);
                            chkSchemeMaster.Checked = Convert.ToBoolean(dr["SchemeMaster"]);
                            chkShowPartyLimit.Checked = Convert.ToBoolean(dr["ShowPartyLimit"]);
                            chkShowAllRecord.Checked = Convert.ToBoolean(dr["ShowAllRecord"]);
                            chkGSTReport.Checked = Convert.ToBoolean(dr["GSTReport"]);
                            chkShowEmailReg.Checked = Convert.ToBoolean(dr["ShowEmailReg"]);
                            chkShowWhatsappReg.Checked = Convert.ToBoolean(dr["ShowWhatsAppReg"]);
                            chkAddCustomer.Checked = Convert.ToBoolean(dr["AddNewCustomer"]);
                        }
                        else
                            UncheckAllControls_SA();
                    }
                    else
                        UncheckAllControls_SA();
                }
                else
                {
                    ClearAllText();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Data with Controls  in Create User Account", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }

        private void ClearAllText()
        {            
            txtConfirmPassword.Clear();
            txtLoginName.Clear();
            txtMobileNo.Clear();
            txtName.Clear();
            txtPassword.Clear();
            txtBranchCode.Clear();
            chkAll.Checked =chkSAALL.Checked= rdoNone.Checked= false;
        }

        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password is not Match", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               // txtConfirmPassword.Focus();
            }
        }

        private void txtLoginName_TextChanged(object sender, EventArgs e)
        {
            if (btnSubmit.Text == "&Submit")
            {
                BindListData();
            }
        }

        private void txtLoginName_Leave(object sender, EventArgs e)
        {
            if (strUser != txtLoginName.Text)
            {
                try
                {
                    if (txtLoginName.Text != "")
                    {
                        DataRow[] filteredRows = table.Select(string.Format("{0} LIKE '{1}'", "LoginName", txtLoginName.Text));
                        if (filteredRows.Length > 0)
                        {
                            lblMsg.Text = txtLoginName.Text + "  is already exists ! Please choose another Name..";
                            lblMsg.ForeColor = Color.Red;
                            lblMsg.Visible = true;
                            txtLoginName.Focus();
                        }
                        else
                        {
                            lblMsg.Text = txtLoginName.Text + "  is Available ........";
                            lblMsg.ForeColor = Color.Green;
                            lblMsg.Visible = true;
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Another Name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        txtLoginName.Focus();
                    }
                }
                catch (Exception ex)
                {
                    string[] strReport = { "Exception occurred in Leave Event of Login Name TextBox in Create User Account", ex.Message };
                    dba.CreateErrorReports(strReport);
                }
            }

        }

        private void lboxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnSubmit.Text == "Up&date")
                {
                    strUser = Convert.ToString(lboxUser.SelectedItem);
                    BindDatawithControls(strUser);
                }
            }
            catch
            {
           }
        }      

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSubmit.Text != "&Submit")
                {
                    DialogResult dr = MessageBox.Show("Are you Sure want to Delete User " + txtLoginName.Text, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strQuery = "Delete from UserAccount where LoginName='" + txtLoginName.Text + "'";

                        int result = dba.ExecuteMyQuery(strQuery);
                        if (result > 0)
                        {
                            MessageBox.Show("User Deleted Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            ClearAllText();
                            table = dba.GetLoginName();
                            BindListData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred on Click Event of Delete Button in Create New User", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Enter && !chkAll.Focused)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void chkCashView_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkJournalAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkJournalAll.Checked == true)
            {
                chkJournalEntry.Checked = true;
                chkJournalView.Checked = true;
                chkJournalEditDelete.Checked = true;
                chkCaseAll.Focus(); 
            }
            if (chkJournalAll.Checked == false)
            {
                chkJournalEntry.Checked = false;
                chkJournalView.Checked = false;
                chkJournalEditDelete.Checked = false;
            }

        }

        private void chkCaseAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCaseAll.Checked == true)
            {
                chkCashEntry.Checked = true;
                chkCashView.Checked = true;
                chkCashEditDelete.Checked = true;
                chkOrderAll.Focus();
            }
            if (chkCaseAll.Checked == false)
            {
                chkCashEntry.Checked = false;
                chkCashView.Checked = false;
                chkCashEditDelete.Checked = false;
            }
        }

        private void chkOrderAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOrderAll.Checked == true)
            {
                chkOrderEntry.Checked = true;
                chkOrderView.Checked = true;
                chkOrderSlipEditDelete.Checked = true;
                chkPurchaseEntryAll.Focus();
            }
            if (chkOrderAll.Checked == false)
            {
                chkOrderEntry.Checked = false;
                chkOrderView.Checked = false;
                chkOrderSlipEditDelete.Checked = false;
            }
        }

        private void chkGoodsAll_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkGoodsAll.Checked == true)
            //{
            //    chkFullEditControl.Checked = true;
            //    chkAddPaymentReq.Checked = true;
            //    chkBulkUpload.Checked = true;
            //    chkSaleEntryAll.Focus();
            //}
            //if (chkGoodsAll.Checked == false)
            //{
            //    chkFullEditControl.Checked = false;
            //    chkAddPaymentReq.Checked = false;
            //    chkBulkUpload.Checked = false;
            //}
        }

        private void chkSaleEntryAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSaleEntryAll.Checked == true)
            {
                chkSaleEntry.Checked = true;
                chkSaleView.Checked = true;
                chkSaleEditDelete.Checked = true;
                chkPurchaseEntryAll.Focus();
            }
            if (chkSaleEntryAll.Checked == false)
            {
                chkSaleEntry.Checked = false;
                chkSaleView.Checked = false;
                chkSaleEditDelete.Checked = false;
            }
        }

        private void chkPurchaseEntryAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPurchaseEntryAll.Checked == true)
            {
                chkPurchaseEntry.Checked = true;
                chkPurchaseView.Checked = true;
                chkPurchaseEditDelete.Checked = true;
                //chkForwardingAll.Focus();
            }
            if (chkPurchaseEntryAll.Checked == false)
            {
                chkPurchaseEntry.Checked = false;
                chkPurchaseView.Checked = false;
                chkPurchaseEditDelete.Checked = false;
            }
        }

        //private void chkForwardingAll_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkForwardingAll.Checked == true)
        //    {
        //        chkDownloadRequest.Checked = true;
        //        chkSendRequest.Checked = true;
        //        chkChangePaymentStatus.Checked = true;
        //        chkCourierAll.Focus();
        //    }
        //    if (chkForwardingAll.Checked == false)
        //    {
        //        chkDownloadRequest.Checked = false;
        //        chkSendRequest.Checked = false;
        //        chkChangePaymentStatus.Checked = false;
        //    }

        //}

        private void chkCourierAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCourierAll.Checked == true)
            {
                chkCourierEntry.Checked = true;
                chkCourierView.Checked = true;
                chkCourierEditDelete.Checked = true;
                chkNewPartyAll.Focus();
            }
            if (chkCourierAll.Checked == false)
            {
                chkCourierEntry.Checked = false;
                chkCourierView.Checked = false;
                chkCourierEditDelete.Checked = false;
            }
        }

        private void chkNewPartyAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNewPartyAll.Checked == true)
            {
                chkPartyEntry.Checked = true;
                chkPartyView.Checked = true;
                chkPartyEditDelete.Checked = true;
                chkSubPartyAll.Focus();
            }
            if (chkNewPartyAll.Checked == false)
            {
                chkPartyEntry.Checked = false;
                chkPartyView.Checked = false;
                chkPartyEditDelete.Checked = false;
            }

        }

        private void chkSubPartyAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSubPartyAll.Checked == true)
            {
                chkSubPartyEntry.Checked = true;
                chkSubPartyView.Checked = true;
                chkSubPartyEditDelete.Checked = true;
                chkAccountMasterAll.Focus();
            }
            if (chkSubPartyAll.Checked == false)
            {
                chkSubPartyEntry.Checked = false;
                chkSubPartyView.Checked = false;
                chkSubPartyEditDelete.Checked = false;
            }
        }

        private void chkAccountMasterAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAccountMasterAll.Checked == true)
            {
                chkAddAccountMasterEntry.Checked = true;
                chkAccountMasterView.Checked = true;
                chkAccountMasterEditDelete.Checked = true;
                chkMergeEntry.Focus();
            }
            if (chkAccountMasterAll.Checked == false)
            {
                chkAddAccountMasterEntry.Checked = false;
                chkAccountMasterView.Checked = false;
                chkAccountMasterEditDelete.Checked = false;
            }
        }

        private void chkAll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                panelPermission.Focus();
                chkJournalAll.Focus();
            }
        }

        private void chkJournalEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(chkJournalEditDelete.Checked))
            chkJournalView.Checked = true;
        }

        private void chkCashEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(chkCashEditDelete.Checked))
            chkCashView.Checked = true;
        }

        private void chkOrderSlipEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if(Convert.ToBoolean(chkOrderSlipEditDelete.Checked))
            chkOrderView.Checked=true;
        }

        private void chkGoodsReceiveEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            //if (Convert.ToBoolean(chkGoodsReceiveEditDelete.Checked))
            //chkGoodsView.Checked = true;
        }

        private void chkSaleEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(chkSaleEditDelete.Checked))
            chkSaleView.Checked = true;
        }

        private void chkPurchaseEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(chkPurchaseEditDelete.Checked))
            chkPurchaseView.Checked = true;
        }

        private void chkForwardingEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            //if (Convert.ToBoolean(chkChangePaymentStatus.Checked))
            //chkSendRequest.Checked = true;
        }

        private void chkCourierEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if(Convert.ToBoolean(chkCourierEditDelete.Checked))
            chkCourierView.Checked = true;
        }

        private void chkPartyEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if(Convert.ToBoolean(chkPartyEditDelete.Checked))
            chkPartyView.Checked = true;
        }

        private void chkSubPartyEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(chkSubPartyEditDelete.Checked))
            chkSubPartyView.Checked = true;
        }

        private void chkAccountMasterEditDelete_CheckedChanged(object sender, EventArgs e)
        {
            if(Convert.ToBoolean(chkAccountMasterEditDelete.Checked))
            chkAccountMasterView.Checked = true;
        }

        private void chkDebitCreditAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDebitCreditAll.Checked == true)
            {
                chkForwardingReportViewAdd.Checked = true;
                chkGoodsRecivedReportViewView.Checked = true;
                chkOnAccountEditDelete.Checked = true;
                chkPurchaseEntryAll.Focus();
            }
            if (chkDebitCreditAll.Checked == false)
            {
                chkForwardingReportViewAdd.Checked = false;
                chkGoodsRecivedReportViewView.Checked = false;
                chkOnAccountEditDelete.Checked = false;
            }
        }

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "SELECT BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void CheckAllControls_SA()
        {
            try
            {
                if (MainPage.strUserRole == "SUPERADMIN")
                {
                    foreach (Control _ctrl in grpSuperAdmin.Controls)
                    {
                        if (_ctrl is CheckBox)
                        {
                            ((CheckBox)_ctrl).Checked = true;
                        }

                    }
                }
            }
            catch
            {
            }
        }

        private void UncheckAllControls_SA()
        {
            try
            {
                foreach (Control _ctrl in grpSuperAdmin.Controls)
                {
                    if (_ctrl is CheckBox)
                    {
                        ((CheckBox)_ctrl).Checked = false;
                    }

                }
            }
            catch
            {
            }
        }

        private void chkSAALL_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSAALL.Checked)
                CheckAllControls_SA();
            else
                UncheckAllControls_SA();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            chkGSTMasterEntry.Checked = chkGSTMasterView.Checked = chkGSTMasterEditDelete.Checked = chkGSTMasterAll.Checked;

        }

        private void chkReferenceAll_CheckedChanged(object sender, EventArgs e)
        {
            chkRefrenceMasterEntry.Checked = chkRefrenceMasterView.Checked = chkRefrenceMasterEditDelete.Checked = chkReferenceAll.Checked;
        }

        private void CreateUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (MainPage.strSoftwareType != "AGENT")
                    grpPayment.Enabled = false;
            }
            catch { }
        }
    }
}
