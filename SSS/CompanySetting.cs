using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class CompanySetting : Form
    {
        DataBaseAccess dba;
        string strFinYear = "";
        bool isReminder = false;
        public CompanySetting()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            hideOpen();
            BindDataWithControls();
            BindRecord();
            GetReminderSchedule();
            // GetMonthLockSchedule();
        }

        private void hideOpen()
        {
            lblsalesHeadr.Visible = lbl1.Visible = lbl2.Visible = lbl3.Visible = lbl4.Visible = txtFreightDhara.Visible = txtTaxDhara.Visible = txtPostage.Visible = txtPacking.Visible  = (MainPage.strSoftwareType == "AGENT");
        }

        private void UpdateMySetting()
        {
            try
            {
                string[] data = new string[40];

                data[0] = "Yes";
                data[1] = "No";
                data[2] = "5";
                data[3] = txtMobileNo.Text;
                data[4] = txtEmailID.Text;
                data[5] = MainPage.strCompanyName;
                data[6] = txtGRCode.Text;
                data[7] = txtSaleBillCode.Text;
                data[8] = txtPBillCOde.Text;
                data[9] = txtFChallanCode.Text;
                data[10] = txtGReturnCode.Text;
                data[11] = txtCashVCode.Text;
                data[12] = txtJVCode.Text;
                data[13] = txtCourierCode.Text;
                data[14] = txtOrderCode.Text;
                data[15] = txtBankVCode.Text;
                data[16] = txtPassword.Text;
                data[17] = txtPurchaseReturnCode.Text;
                data[18] = txtSaleServiceCode.Text;
                data[19] = txtDebitNoteCode.Text;
                data[20] = txtCreditNoteCode.Text;
                data[21] = txtRCMVoucherCode.Text;
                data[22] = txtSMTPServer.Text;
                data[23] = txtPortNo.Text;
                data[24] = txtAltCode.Text;
                data[25] = txtSTCode.Text;
                data[26] = "";
                data[27] = rdoWithBarCode.Checked.ToString();
                data[28] = txtTCSDNCode.Text;
                data[29] = txtTCSCNCode.Text;
                data[30] = txtAdvanceVCode.Text;
                // data[31] = rdoUniqueBarCode.Checked.ToString();
                data[31] = rdoWithPrintDialog.Checked.ToString();
                data[32] = (rdoDesignMaster.Checked ? "DesignMaster" : "AvgRate");
                data[33] = (rdoHSNWise.Checked ? "HSN_WISE" : "ARTICLE_WISE");
                string barCodingType = "";
                if (rdoWithBarCode.Checked)
                {
                    if (rdoUniqueBarCode.Checked)
                        barCodingType = "UNIQUE_BARCODE";
                    else if (rdoAsPerDesignMaster.Checked)
                        barCodingType = "DESIGNMASTER_WISE";
                    else if (rdoItemwise.Checked)
                        barCodingType = "ITEM_WISE";
                }

                data[34] = barCodingType;
                data[35] = (rdoOpeningArticleWise.Checked ? "ARTICLE_WISE" : "ITEM_WISE");

                string strLockPeriod = "";
                if (rdoMLMonthly.Checked)
                    strLockPeriod = "MONTHLY";
                else if (rdoMLQuarterly.Checked)
                    strLockPeriod = "QUARTERLY";
                else if (rdoMLHalfYearly.Checked)
                    strLockPeriod = "HALFYEARLY";
                else if (rdoMLYearly.Checked)
                    strLockPeriod = "YEARLY";
                data[36] = strLockPeriod;
                data[37] = cmbMLDate.Text;
                data[38] = (rdoSetWise.Checked ? "SET_WISE" : "SINGLE_ITEM_WISE");

                int count = dba.UpdateCompanyMySetting(data);
                if (count > 0)
                {
                    MainPage.strSenderEmailID = txtEmailID.Text;
                    MainPage.strSenderPassword = txtPassword.Text;
                    MainPage.strSMTPServer = txtSMTPServer.Text;
                    MainPage._SMTPPORTNo = dba.ConvertObjectToInt(txtPortNo.Text);
                    MainPage._bBarCodeStatus = rdoWithBarCode.Checked;
                    MainPage._PrintWithDialog = rdoWithPrintDialog.Checked;
                    MainPage.strStockAsPer = (rdoDesignMaster.Checked ? "DesignMaster" : "AvgRate");
                    MainPage.bHSNWisePurchase = rdoHSNWise.Checked;
                    MainPage.strBarCodingType = barCodingType;
                    MainPage.bArticlewiseOpening = rdoOpeningArticleWise.Checked;
                    MainPage.strMonthLockPeriod = strLockPeriod;
                    MainPage.strMonthLockDate = cmbMLDate.Text;
                    MainPage.bPurchaseSetWise = rdoSetWise.Checked;

                    MessageBox.Show("Thank ! Record Updated Successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    BindDataWithControls();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on update MySetting in Company Setting ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void UpdateApplicationSetting()
        {
            try
            {
                string[] data = new string[21];
                data[0] = txtDays.Text;
                data[1] = txtGraceDays.Text;
                data[2] = txtCashDiscDays.Text;
                data[3] = txtCashDiscRate.Text;
                data[4] = txtDrInterest.Text;
                data[5] = txtCrInterest.Text;

                data[6] = txtFreightDhara.Text;
                data[7] = txtTaxDhara.Text;
                data[8] = txtPostage.Text;
                data[9] = txtPacking.Text;
                data[10] = ""; //txtVat.Text;
                data[11] = "";// txtWholeSale.Text;
                data[12] = MainPage.strCompanyName;
                data[13] = txtHTTPPath.Text;
                data[14] = txtFTPPath.Text;
                data[15] = txtFTPUserName.Text;
                data[16] = txtFTPPassword.Text;

                int count = dba.UpdateCompanyApplicationSetting(data);
                if (count > 0)
                {
                    MessageBox.Show("Record Updated Successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    BindDataWithControls();

                    MainPage.strHttpPath = txtHTTPPath.Text;
                    MainPage.strFTPPath = txtFTPPath.Text;
                    MainPage.strFTPUserName = txtFTPUserName.Text;
                    MainPage.strFTPPassword = txtFTPPassword.Text;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Update Application Setting in Company Setting ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnAppSubmit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure want to Save Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                UpdateApplicationSetting();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (CheckCodeValidation())
            {
                DialogResult dr = MessageBox.Show("Are you sure want to Save Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    UpdateMySetting();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAppCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void KeyHandler(KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;

                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                    lblMsg.Text = "Only Numeric value Allowed";
                    lblMsg.Visible = true;
                }
                else
                {
                    e.Handled = false;
                    lblMsg.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Key Handler in Company Setting ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void txtLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void BindDataWithControls()
        {
            try
            {
                DataTable table = dba.GetComSetting(MainPage.strCompanyName);
                //lblManageItemMapping.Visible = !MainPage._bTaxStatus;
                //grpBoxItemMapping.Visible = !MainPage._bTaxStatus;

                // MainPage._bBarCodeStatus = false;
                if (table.Rows.Count > 0)
                {
                    DataRow dr = table.Rows[0];

                    txtMobileNo.Text = Convert.ToString(dr["MobileNo"]);
                    txtEmailID.Text = Convert.ToString(dr["EmailID"]);
                    txtPassword.Text = txtConfirmPassword.Text = Convert.ToString(dr["Password"]);
                    txtGRCode.Text = Convert.ToString(dr["GReceiveCode"]);
                    txtSaleBillCode.Text = Convert.ToString(dr["SBillCode"]);
                    txtPBillCOde.Text = Convert.ToString(dr["PBillCode"]);
                    txtFChallanCode.Text = Convert.ToString(dr["FChallanCode"]);
                    txtGReturnCode.Text = Convert.ToString(dr["GReturnCode"]);
                    txtCashVCode.Text = Convert.ToString(dr["CashVCode"]);
                    txtJVCode.Text = Convert.ToString(dr["JournalVCode"]);
                    txtCourierCode.Text = Convert.ToString(dr["CourierCode"]);
                    txtOrderCode.Text = Convert.ToString(dr["OrderCode"]);
                    txtBankVCode.Text = Convert.ToString(dr["BankVCode"]);
                    txtPurchaseReturnCode.Text = Convert.ToString(dr["PurchaseReturnCode"]);
                    txtSaleServiceCode.Text = Convert.ToString(dr["SaleServiceCode"]);
                    txtTCSDNCode.Text = Convert.ToString(dr["TCSDNCode"]);
                    txtTCSCNCode.Text = Convert.ToString(dr["TCSCNCode"]);
                    txtAdvanceVCode.Text = Convert.ToString(dr["AdvanceVCode"]);

                    txtDays.Text = Convert.ToString(dr["DaysInYear"]);
                    txtGraceDays.Text = Convert.ToString(dr["GraceDays"]);
                    txtCashDiscDays.Text = Convert.ToString(dr["CashDiscDays"]);
                    txtCashDiscRate.Text = Convert.ToString(dr["CashDiscRate"]);
                    txtDrInterest.Text = Convert.ToString(dr["DrInterest"]);
                    txtCrInterest.Text = Convert.ToString(dr["CrInterest"]);

                    txtFreightDhara.Text = Convert.ToString(dr["FreightDhara"]);
                    txtTaxDhara.Text = Convert.ToString(dr["TaxDhara"]);
                    txtPostage.Text = Convert.ToString(dr["Postage"]);
                    txtPacking.Text = Convert.ToString(dr["Packing"]);
                    string strUnique = Convert.ToString(dr["Vat"]).ToUpper();
                    if (strUnique.Contains("TRUE"))
                        rdoUniqueBarCode.Checked = true;
                    else
                        rdoItemwise.Checked = true;

                    string strPrintOn = Convert.ToString(dr["Rebate"]).ToUpper();
                    if (strPrintOn.Contains("TRUE"))
                        rdoWithPrintDialog.Checked = true;
                    else
                        rdoWithoutPrintDialog.Checked = true;

                    //txtWholeSale.Text = Convert.ToString(dr["Rebate"]);

                    txtDebitNoteCode.Text = Convert.ToString(dr["DebitNoteCode"]);
                    txtCreditNoteCode.Text = Convert.ToString(dr["CreditNoteCode"]);
                    txtRCMVoucherCode.Text = Convert.ToString(dr["RCMVCode"]);
                    txtSMTPServer.Text = Convert.ToString(dr["SMTPServer"]);
                    txtPortNo.Text = Convert.ToString(dr["SMTPPort"]);

                    lblDate.Text = Convert.ToString(dr["CDate"]);
                    lblDate.Visible = true;
                    rdoWithoutBarcode.Checked = true;
                    if (table.Columns.Contains("AltrationCode"))
                    {
                        txtAltCode.Text = Convert.ToString(dr["AltrationCode"]);
                        txtSTCode.Text = Convert.ToString(dr["STCode"]);

                        string strbarCode = Convert.ToString(dr["OtherCode"]).ToUpper();
                        if (strbarCode.Contains("TRUE"))
                            rdoWithBarCode.Checked = true;
                    }

                    txtHTTPPath.Text = Convert.ToString(dr["HTTPPath"]);
                    txtFTPPath.Text = Convert.ToString(dr["FTPPath"]);
                    txtFTPUserName.Text = Convert.ToString(dr["FTPUserName"]);
                    txtFTPPassword.Text = Convert.ToString(dr["FTPPassword"]);
                    if (table.Columns.Contains("StockAsPer"))
                    {
                        string strStockAsPer = Convert.ToString(dr["StockAsPer"]).ToUpper();
                        if (strStockAsPer.Contains("DESIGNMASTER"))
                            rdoDesignMaster.Checked = true;
                        else
                            rdoAvgRate.Checked = true;
                    }
                    if (table.Columns.Contains("ItemMapping"))
                    {
                        string str = Convert.ToString(dr["ItemMapping"]).ToUpper();
                        if (str.Contains("HSN_WISE"))
                            rdoHSNWise.Checked = true;
                        else
                            rdoArticleWise.Checked = true;
                    }
                    if (table.Columns.Contains("BarcodingType"))
                    {
                        string str = Convert.ToString(dr["BarcodingType"]).ToUpper();
                        if (str == "UNIQUE_BARCODE")
                            rdoUniqueBarCode.Checked = true;
                        if (str == "DESIGNMASTER_WISE")
                            rdoAsPerDesignMaster.Checked = true;
                        if (str == "ITEM_WISE")
                            rdoItemwise.Checked = true;
                    }

                    rdoOpeningItemWise.Checked = true;
                    if (table.Columns.Contains("ItemOpening"))
                        rdoOpeningArticleWise.Checked = Convert.ToString(dr["ItemOpening"]).ToUpper() == "ARTICLE_WISE";

                    if (table.Columns.Contains("MonthLockPeriod"))
                    {
                        string LockPeriod = Convert.ToString(dr["MonthLockPeriod"]);
                        rdoMLMonthly.Checked = (LockPeriod == "MONTHLY");
                        rdoMLQuarterly.Checked = (LockPeriod == "QUARTERLY");
                        rdoMLHalfYearly.Checked = (LockPeriod == "HALFYEARLY");
                        rdoMLYearly.Checked = (LockPeriod == "YEARLY");
                        cmbMLDate.Text = Convert.ToString(dr["MonthLockDate"]);
                        lblMonthLocMsg.Visible = false;
                    }
                    else
                        lblMonthLocMsg.Visible = true;

                    rdoSingleItemWise.Checked = true;
                    if (table.Columns.Contains("PurchaseSetWise"))
                        rdoSetWise.Checked = Convert.ToString(dr["PurchaseSetWise"]).ToUpper() == "SET_WISE";
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Bind Data With Controls in Company Setting ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CompanySetting_KeyDown(object sender, KeyEventArgs e)
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

        private void txtCourierCode_Leave(object sender, EventArgs e)
        {
            bool chk = CheckCodeValidation();
            if (!chk)
            {
                TextBox txt = sender as TextBox;
                txt.Focus();
            }
        }

        private bool CheckCodeValidation()
        {
            bool chkValidation = true;

            if ((txtGRCode.Text == txtSaleBillCode.Text || txtGRCode.Text == txtFChallanCode.Text || txtGRCode.Text == txtGReturnCode.Text || txtGRCode.Text == txtCashVCode.Text || txtGRCode.Text == txtJVCode.Text || txtGRCode.Text == txtCourierCode.Text || txtGRCode.Text == txtOrderCode.Text || txtGRCode.Text == txtBankVCode.Text) && txtGRCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtPBillCOde.Text == txtSaleBillCode.Text || txtPBillCOde.Text == txtGReturnCode.Text || txtPBillCOde.Text == txtFChallanCode.Text || txtPBillCOde.Text == txtCashVCode.Text || txtPBillCOde.Text == txtJVCode.Text || txtPBillCOde.Text == txtCourierCode.Text || txtPBillCOde.Text == txtOrderCode.Text || txtPBillCOde.Text == txtBankVCode.Text) && txtPBillCOde.Text != "")
            {
                chkValidation = false;
            }
            if ((txtSaleBillCode.Text == txtGReturnCode.Text || txtSaleBillCode.Text == txtFChallanCode.Text || txtSaleBillCode.Text == txtCashVCode.Text || txtSaleBillCode.Text == txtJVCode.Text || txtSaleBillCode.Text == txtCourierCode.Text || txtSaleBillCode.Text == txtOrderCode.Text || txtSaleBillCode.Text == txtBankVCode.Text) && txtSaleBillCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtGReturnCode.Text == txtFChallanCode.Text || txtGReturnCode.Text == txtCashVCode.Text || txtGReturnCode.Text == txtJVCode.Text || txtGReturnCode.Text == txtCourierCode.Text || txtGReturnCode.Text == txtOrderCode.Text || txtGReturnCode.Text == txtBankVCode.Text) && txtGReturnCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtFChallanCode.Text == txtCashVCode.Text || txtFChallanCode.Text == txtJVCode.Text || txtFChallanCode.Text == txtCourierCode.Text || txtFChallanCode.Text == txtOrderCode.Text || txtFChallanCode.Text == txtBankVCode.Text) && txtFChallanCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtCashVCode.Text == txtJVCode.Text || txtCashVCode.Text == txtCourierCode.Text || txtCashVCode.Text == txtOrderCode.Text || txtCashVCode.Text == txtBankVCode.Text) && txtCashVCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtJVCode.Text == txtCourierCode.Text || txtJVCode.Text == txtOrderCode.Text || txtJVCode.Text == txtBankVCode.Text) && txtJVCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtCourierCode.Text == txtOrderCode.Text || txtCourierCode.Text == txtBankVCode.Text) && txtCourierCode.Text != "")
            {
                chkValidation = false;
            }
            if ((txtOrderCode.Text == txtBankVCode.Text) && txtBankVCode.Text != "")
            {
                chkValidation = false;
            }

            if (!chkValidation)
            {
                MessageBox.Show("Serial Code can not be dupliacate ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return chkValidation;
        }

        private void txtVat_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password doesn't matched ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtPurchaseReturnCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void lnkShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtFTPPassword.UseSystemPasswordChar = false;
        }

        private void lnkShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtFTPPassword.UseSystemPasswordChar = true;
        }

        private void txtFTPPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateAllSpace(sender, e);
        }

        private void rdoWithBarCode_CheckedChanged(object sender, EventArgs e)
        {
            rdoItemwise.Checked = rdoUniqueBarCode.Checked = rdoAsPerDesignMaster.Checked = false;
            grpBarCodingType.Enabled = rdoWithBarCode.Checked;
        }

        private void SetDefaultValue(object sender, string strSuffix)
        {
            if (sender is TextBox)
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text == "")
                    txt.Text = strFinYear + MainPage.strBranchCode + strSuffix;
            }
        }

        private void txtCashVCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "C");
        }

        private void txtJVCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "J");
        }

        private void txtBankVCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "B");
        }

        private void txtDebitNoteCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "DN");
        }

        private void txtCreditNoteCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "CN");
        }

        private void txtRCMVoucherCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "RCM");
        }

        private void txtTCSDNCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "TDN");
        }

        private void txtTCSCNCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "TCN");
        }

        private void txtAdvanceVCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "ADV");
        }

        private void txtOrderCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "O");
        }

        private void txtSaleBillCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "S");
        }

        private void txtPBillCOde_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "P");
        }

        private void txtFChallanCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "D");
        }

        private void txtGReturnCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "SR");
        }

        private void txtPurchaseReturnCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "PR");
        }

        private void txtCourierCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "CC");
        }

        private void txtSaleServiceCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "SS");
        }

        private void txtAltCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "ALT");
        }

        private void txtSTCode_Enter(object sender, EventArgs e)
        {
            SetDefaultValue(sender, "ST");
        }

        private void CompanySetting_Load(object sender, EventArgs e)
        {
            string strStart = MainPage.startFinDate.Year.ToString("00"), strEnd = MainPage.endFinDate.Year.ToString("00");
            if (strStart.Length == 4)
                strStart = strStart.Substring(2, 2);
            if (strEnd.Length == 4)
                strEnd = strEnd.Substring(2, 2);
            if (strStart != strEnd)
                strFinYear = strStart + "-" + strEnd + "/";
            else
                strFinYear = strStart + "/";
        }

        private void tabChanged()
        {
            if (tabSetting.SelectedIndex == 0 || tabSetting.SelectedIndex == 2 || tabSetting.SelectedIndex == 4 || tabSetting.SelectedIndex == 6)
            {
                btnSubmit.Visible = true;
                btnCancel.Visible = true;
                btnAppSubmit.Visible = false;
                btnAppCancel.Visible = false;
            }
            else
            {
                btnSubmit.Visible = false;
                btnCancel.Visible = false;
                btnAppSubmit.Visible = true;
                btnAppCancel.Visible = true;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Add")
                {
                    btnEdit.Text = "&Save";
                    btnDelete.Enabled = false;
                    EnableAllControl();
                }
                else if (btnEdit.Text == "&Save")
                {
                    if (txtURL.Text != "" && txtSenderId.Text != "")
                    {
                        string strQuery = "Insert into MessageMaster values ('" + txtURL.Text + "','" + txtSenderId.Text + "','" + txtUserName.Text + "','" + txtSMSPassword.Text + "','" + txtMessageType.Text + "')";
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            btnDelete.Enabled = true;
                            BindRecord();
                            UpdateMainPageSMSPath();
                            DisableAllControl();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill the Mendatory Fields...");
                    }
                }
                else if (btnEdit.Text == "&Edit")
                {
                    btnEdit.Text = "&Update";
                    btnDelete.Enabled = false;
                    EnableAllControl();

                }
                else if (btnEdit.Text == "&Update")
                {
                    if (txtURL.Text != "" && txtSenderId.Text != "")
                    {
                        string strQuery = "update MessageMaster set URL='" + txtURL.Text + "',SenderId='" + txtSenderId.Text + "',UserName='" + txtUserName.Text + "',Password='" + txtSMSPassword.Text + "',MessageType='" + txtMessageType.Text + "'";
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            btnDelete.Enabled = true;
                            BindRecord();
                            UpdateMainPageSMSPath();
                            DisableAllControl();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill the Mendatory Fields...");
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        private void UpdateMainPageSMSPath()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from MessageMaster");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    MainPage.strSMSURL = Convert.ToString(row["URL"]);
                    MainPage.strSenderID = Convert.ToString(row["SenderID"]);
                    MainPage.strSMSUser = Convert.ToString(row["UserName"]);
                    MainPage.strSMSPassword = Convert.ToString(row["Password"]);
                    MainPage.strMessageType = Convert.ToString(row["MessageType"]);
                }
            }
            catch (Exception ex)
            { }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "&Edit")
            {
                string strQuery = "Delete from MessageMaster";
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record Deleted successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Add";
                    BindRecord();
                    DisableAllControl();
                }
            }
        }

        private void BindRecord()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from MessageMaster");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtURL.Text = Convert.ToString(row["URL"]);
                    txtSenderId.Text = Convert.ToString(row["SenderID"]);
                    txtUserName.Text = Convert.ToString(row["UserName"]);
                    txtSMSPassword.Text = Convert.ToString(row["Password"]);
                    txtMessageType.Text = Convert.ToString(row["MessageType"]);
                }
                else
                {
                    btnEdit.Text = "&Add";
                    ClearTextBox();
                }
            }
            catch (Exception ex)
            { }
        }
        private void EnableAllControl()
        {
            txtURL.ReadOnly = txtSenderId.ReadOnly = txtUserName.ReadOnly = txtSMSPassword.ReadOnly = txtMessageType.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtURL.ReadOnly = txtSenderId.ReadOnly = txtUserName.ReadOnly = txtSMSPassword.ReadOnly = txtMessageType.ReadOnly = true;
        }
        private void ClearTextBox()
        {
            txtURL.Text = txtSenderId.Text = txtUserName.Text = txtSMSPassword.Text = txtMessageType.Text = "";
        }

        private void txtURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtSenderId_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtSMSPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtMessageType_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0)
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtGroupType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupType.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void GetReminderSchedule()
        {
            try
            {
                string strQuery = "Select top 1 * from Reminder_Schedular where CompanyName='" + MainPage.strCompanyName + "'";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    btnSchUpdate.Text = "Update";
                    DataRow dr = dt.Rows[0];
                    txtGroupType.Text = Convert.ToString(dr["GroupName"]);
                    string strAccType = Convert.ToString(dr["AccountType"]);
                    if (strAccType == "ALL")
                        rdoAll.Checked = true;
                    else if (strAccType == "CREDIT")
                        rdoCredit.Checked = true;
                    else
                        rdoDebit.Checked = true;

                    txtAmt.Text = Convert.ToString(dr["Amount"]);
                    if (txtAmt.Text != "")
                        txtAmt.Text = dba.ConvertObjectToDouble(txtAmt.Text).ToString("N2", MainPage.indianCurancy);

                    chkSMS.Checked = Convert.ToBoolean(dr["TextSMS"]);
                    chkGmail.Checked = Convert.ToBoolean(dr["Email"]);
                    chkWhatsApp.Checked = Convert.ToBoolean(dr["WhatsApp"]);
                    rdoDaily.Checked = Convert.ToBoolean(dr["Daily"]);
                    rdoWeekly.Checked = Convert.ToBoolean(dr["Weekly"]);
                    rdoHalfMonthly.Checked = Convert.ToBoolean(dr["HalfMonthly"]);
                    rdoMonthly.Checked = Convert.ToBoolean(dr["Monthly"]);
                    rdoQuarterly.Checked = Convert.ToBoolean(dr["Quarterly"]);
                    rdoYearly.Checked = Convert.ToBoolean(dr["Yearly"]);

                    txtTime.Text = Convert.ToString(dr["ReminderTime"]);
                    cmbTime.Text = Convert.ToString(dr["TimeType"]);
                    cmbDay.Text = Convert.ToString(dr["ReminderDay"]);
                    cmbDate.Text = Convert.ToString(dr["ReminderDate"]);
                    cmbDate2.Text = Convert.ToString(dr["ReminderDate2"]);
                    cmbMonth.Text = Convert.ToString(dr["ReminderMonth"]);
                    txtMessage.Text = Convert.ToString(dr["ReminderMessage"]);
                    if (Convert.ToString(dr["Status"]).ToUpper() == "TRUE")
                        rdoScheduleEnabled.Checked = true;
                    else
                        rdoScheduleDisabled.Checked = true;


                    txtMessage.Text = txtMessage.Text == "" ? "Message ..." : txtMessage.Text;
                    isReminder = true;
                    lblReminderMsg.Visible = false;
                }
                else
                {
                    lblReminderMsg.Visible = true;
                    btnSchUpdate.Text = "Save";
                    txtGroupType.Text = "";
                    rdoAll.Checked = true;
                    txtAmt.Text = "";
                    chkSMS.Checked = true;
                    chkGmail.Checked = false;
                    chkWhatsApp.Checked = false;
                    rdoDaily.Checked = true;
                    txtTime.Text = "12";
                    cmbTime.Text = "AM";
                    cmbDay.Text = "Monday";
                    cmbDate.Text = "";
                    cmbDate2.Text = "1";
                    cmbMonth.Text = "January";
                    txtMessage.Text = "Message ...";
                    isReminder = false;
                    rdoScheduleEnabled.Checked = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private void txtAmt_Leave(object sender, EventArgs e)
        {
            if (txtAmt.Text != "")
                txtAmt.Text = dba.ConvertObjectToDouble(txtAmt.Text).ToString("N2", MainPage.indianCurancy);
        }

        private void chkSMS_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkGmail.Checked && !chkWhatsApp.Checked)
                chkSMS.Checked = true;
        }

        private void rdoDaily_CheckedChanged(object sender, EventArgs e)
        {
            CheckedUncheked();
        }
        private void CheckedUncheked()
        {
            lblDay.Visible = cmbDay.Visible = lblDate1.Visible = cmbDate.Visible = lblDate2.Visible = cmbDate2.Visible = lblMonth.Visible = cmbMonth.Visible = false;

            if (rdoDaily.Checked)
            {
                adjustElements(0);
            }
            if (rdoWeekly.Checked)
            {
                ShowIt(1);
                adjustElements(1);
            }
            if (rdoMonthly.Checked)
            {
                ShowIt(2);
                adjustElements(2);
            }
            if (rdoHalfMonthly.Checked)
            {
                ShowIt(2);
                ShowIt(3);
                adjustElements(3);
            }
            if (rdoQuarterly.Checked)
            {
                ShowIt(2);
                ShowIt(4);
                adjustElements(4);
            }
            if (rdoYearly.Checked)
            {
                ShowIt(2);
                ShowIt(4);
                adjustElements(5);
            }
        }
        private void adjustElements(int _index)
        {
            lblMonth.Text = "Month :";
            cmbMonth.Width = 170;
            cmbMonth.Left = 558;
            cmbDate.Width = 170;
            switch (_index)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    cmbDate.Width = 170;
                    break;
                case 3:
                    cmbDate.Width = 52;
                    cmbDate.DropDownWidth = 52;
                    break;
                case 4:
                    lblMonth.Text = "Start Month :";
                    cmbMonth.Width = 160;
                    cmbMonth.Left = 593;
                    break;
                case 5:
                    lblMonth.Text = "Month :";
                    break;
            }

        }

        private void ShowIt(int index)
        {
            if (index == 1)
                lblDay.Visible = cmbDay.Visible = true;
            if (index == 2)
                lblDate1.Visible = cmbDate.Visible = true;
            if (index == 3)
                lblDate2.Visible = cmbDate2.Visible = true;
            if (index == 4)
                lblMonth.Visible = cmbMonth.Visible = true;
        }

        private void txtTime_Leave(object sender, EventArgs e)
        {
            if (txtTime.Text != "")
            {
                double dhh = dba.ConvertObjectToDouble(txtTime.Text);

                if (dhh < 1 || dhh > 12)
                    txtTime.Text = "12";

                if (Convert.ToString(dhh).Length < 2)
                    txtTime.Text = "0" + Convert.ToString(dhh);
            }
            else
                txtTime.Text = "12";
        }

        private void cmbDate_TextChanged(object sender, EventArgs e)
        {
            if (cmbDate.Text == cmbDate2.Text && cmbDate.Text != "" && cmbDate2.Visible)
            {
                MessageBox.Show("Date and Date2 can't be same.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbDate.Focus();
                cmbDate.DroppedDown = true;
            }
        }

        private void cmbDate2_TextChanged(object sender, EventArgs e)
        {
            if (cmbDate.Text == cmbDate2.Text && cmbDate2.Text != "")
            {
                MessageBox.Show("Date and Date2 can't be same.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbDate2.Focus();
                cmbDate2.DroppedDown = true;
            }
        }

        private void txtMessage_Enter(object sender, EventArgs e)
        {
            if (txtMessage.Text == "Message ...")
                txtMessage.Text = "";
        }

        private void txtMessage_Leave(object sender, EventArgs e)
        {
            if (txtMessage.Text == "")
                txtMessage.Text = "Message ...";
        }

        private void btnSchUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdoHalfMonthly.Checked && (cmbDate.Text == cmbDate2.Text))
                {
                    MessageBox.Show("Sorry ! Date and Date2 can't be same ! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbDate2.Focus();
                    cmbDate2.DroppedDown = true;
                }
                else
                {
                    string strAccType = "";
                    string time = txtTime.Text == "" ? "12" : txtTime.Text;
                    string timetype = cmbTime.Text == "" ? "AM" : cmbTime.Text;
                    string strAmt = "";
                    double amt = dba.ConvertObjectToDouble(txtAmt.Text);
                    if (txtAmt.Text == "")
                        strAmt = "";
                    else if (amt > 0)
                        strAmt = amt.ToString();
                    else if (amt == 0)
                        strAmt = "0";

                    if (rdoAll.Checked)
                        strAccType = "ALL";
                    else if (rdoCredit.Checked)
                        strAccType = "CREDIT";
                    else
                        strAccType = "DEBIT";

                    DialogResult result = MessageBox.Show("Are you want to update Reminder Schedular  ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string sts = "1";
                        sts = rdoScheduleDisabled.Checked ? "0" : "1";
                        string msg = (txtMessage.Text == "Message ...") ? "" : txtMessage.Text;

                        string strQuery = " Update Reminder_Schedular Set GroupName ='" + txtGroupType.Text + "', AccountType='" + strAccType + "',Amount='" + strAmt + "',TextSMS='" + chkSMS.Checked + "',Email='" + chkGmail.Checked + "',WhatsApp='" + chkWhatsApp.Checked + "',Daily='" + rdoDaily.Checked + "',Weekly='" + rdoWeekly.Checked + "',HalfMonthly='" + rdoHalfMonthly.Checked + "',Monthly='" + rdoMonthly.Checked + "',Quarterly='" + rdoQuarterly.Checked + "',Yearly='" + rdoYearly.Checked + "',ReminderTime='" + time + "',TimeType='" + timetype
                                            + "',ReminderDay='" + cmbDay.Text + "',ReminderDate='" + cmbDate.Text + "',ReminderDate2='" + cmbDate2.Text + "',ReminderMonth='" + cmbMonth.Text + "',ReminderMessage='" + msg + "',Status = '" + sts + "',UpdateStatus=1 Where CompanyName='" + MainPage.strCompanyName + "' ";

                        if (btnSchUpdate.Text == "Save")
                        {
                            strQuery = " Insert into Reminder_Schedular values ('" + MainPage.strCompanyName + "','" + txtGroupType.Text + "','" + strAccType + "','" + txtAmt.Text + "','" + chkSMS.Checked + "','" + chkGmail.Checked + "','" + chkWhatsApp.Checked + "','" + rdoDaily.Checked + "','" + rdoWeekly.Checked + "','" + rdoHalfMonthly.Checked + "','" + rdoMonthly.Checked + "','" + rdoQuarterly.Checked + "','" + rdoYearly.Checked + "','" + time + "','" + timetype + "',1,0,'" + cmbDay.Text + "','" + cmbDate.Text + "','" + cmbDate2.Text + "','" + cmbMonth.Text + "','" + msg + "','" + sts + "' )";
                        }
                        int count = dba.ExecuteMyQuery(strQuery);

                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Reminder Schedular updated successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            lblReminderMsg.Visible = false;
                            isReminder = true;
                            btnSchUpdate.Text = "Update";
                            btnClearReminder.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnClearReminder_Click(object sender, EventArgs e)
        {
            if (btnSchUpdate.Text == "Update")
            {
                if (isReminder)
                {
                    try
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete Reminder Schedule? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " if exists ( select * from Reminder_Schedular where CompanyName='" + MainPage.strCompanyName + "')  BEGIN  Delete from Reminder_Schedular where CompanyName='" + MainPage.strCompanyName + "' END";
                            int isCleared = dba.ExecuteMyQuery(strQuery);
                            if (isCleared > 0)
                            {
                                lblReminderMsg.Visible = true;
                                txtGroupType.Text = "";
                                rdoAll.Checked = true;
                                txtAmt.Text = "";
                                chkSMS.Checked = true;
                                chkGmail.Checked = false;
                                chkWhatsApp.Checked = false;
                                rdoDaily.Checked = true;
                                txtTime.Text = "12";
                                cmbTime.Text = "AM";
                                cmbDay.Text = "";
                                cmbDate.Text = "";
                                cmbDate2.Text = "";
                                cmbMonth.Text = "";
                                txtMessage.Text = "Message ...";
                                isReminder = false;
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Some Error occurred while removing reminder !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            btnSchUpdate.Text = "Save";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }


        private void btnColse2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMLClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMLDelete_Click(object sender, EventArgs e)
        {
            if (!lblMonthLocMsg.Visible)
            {
                try
                {
                    DialogResult result = MessageBox.Show("Do you want to delete auto lock Schedule? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Update CompanySetting set MonthLockPeriod = '', MonthLockDate = '' WHERE CompanyName='" + MainPage.strCompanyName + "' ";
                        int isCleared = dba.ExecuteMyQuery(strQuery);
                        if (isCleared > 0)
                        {
                            rdoMLMonthly.Checked = rdoMLQuarterly.Checked = rdoMLHalfYearly.Checked = rdoMLYearly.Checked = false;
                            cmbMLDate.Text = "";
                            lblMonthLocMsg.Visible = true;
                            MainPage.strMonthLockPeriod = MainPage.strMonthLockDate = "";
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Some Error occurred while removing reminder !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //btnMLSave.Text = "Add";
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void tabSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabChanged();
        }

        private void btnUpdateAutoSaleSMS_Click(object sender, EventArgs e)
        {
            string strSMSPeriod = "";
            if (rdPerHr.Checked)
                strSMSPeriod = "HOUR";
            if (rdPerDay.Checked)
                strSMSPeriod = "DAILY";
            if (rdTwiceW.Checked)
                strSMSPeriod = "W2";
            if (rdWeekly.Checked)
                strSMSPeriod = "WEEKLY";
            if (txtMobile.Text =="" || txtMobile.Text == null)
            {
                MessageBox.Show("Mobile No. can't be Blank");
                txtMobile.Focus();
            }
            if ((rdWeekly.Checked || rdTwiceW.Checked) && cmbDay1.SelectedText != "")
            {
                MessageBox.Show("Day1 Can't be blank.Please select Day1", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDay1.Focus();
            }
            if (rdTwiceW.Checked && cmbDay2.SelectedText != "")
            {
                MessageBox.Show("Day2 Can't be blank.Please select Day2", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDay2.Focus();
            }
            DialogResult result = MessageBox.Show("Are you want to update Auto Sale SMS ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string msg = (txtMsg.Text == "Message ...") ? "" : txtMsg.Text;

                string strQuery = " If not Exists (Select * from AutoSaleSMS) Insert Into Auto Sale SMS values ('" + txtMobile.Text + "','" +strSMSPeriod + "','" + msg + "','" + cmbDay1.SelectedText + "','" + cmbDay2.SelectedText + "') Else Update AutoSaleSMS set MobileNo = '" + txtMobile.Text + "',SMSPeriod='" + strSMSPeriod + "',Message='" + msg + "',Day1='" + cmbDay1.SelectedText + "',Day2='" + cmbDay2.SelectedText + "'";
                int count = dba.ExecuteMyQuery(strQuery);

                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Auto Sale SMS updated successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    BindAutoSaleSMS();
                }
            }

        }

        private void BindAutoSaleSMS()
        {
            string strSMSPeriod = "";
            DataTable dt = dba.GetDataTable("Select * from AutoSaleSMS");
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                txtMobile.Text = Convert.ToString(dr["MobileNo"]);
                strSMSPeriod = Convert.ToString(dr["SMSPeriod"]);
                txtMsg.Text = Convert.ToString(dr["Message"]);
                cmbDay1.Text = Convert.ToString(dr["Day1"]);
                cmbDay2.Text = Convert.ToString(dr["Day1"]);
                if (strSMSPeriod == "HOUR")
                    rdPerHr.Checked = true;
                if (strSMSPeriod == "DAILY")
                    rdPerDay.Checked = true;
                if (strSMSPeriod == "W2")
                    rdTwiceW.Checked = true;
                if (strSMSPeriod == "WEEKLY")
                    rdWeekly.Checked = true;

            }
        }
    }
}
