using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class SupplierMaster : Form
    {
        DataBaseAccess dba = null;
        PartyTransport pt;
        int check = 0, update = 0, newPartStatus = 0;
        DataTable dtable;
        public string strSelectedName = "", strMobileNumber = "", strAccountName = "", strAreaCode = "", strOldMobileNumber = "", strOldEmailID = "", strOldStation = "", strOldStateName = "";
        ToolTip tt;
        object ObjDummyProfile = SSS.Properties.Resources.profile;
        public SupplierMaster()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                pt = new PartyTransport();
                dtable = dba.GetPartyNameGroupFullNameRecord();

                GetAreaCode();
                BindListData();

            }
            catch
            {
            }
        }

        public SupplierMaster(int sStatus, string strGroup, string strCategory)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                pt = new PartyTransport();
                dtable = dba.GetPartyNameGroupFullNameRecord();
                btnDelete.Enabled = btnEdit.Enabled = false;
                txtSearchGroup.TabStop = txtSearchParty.TabStop = txtSearchSSSName.TabStop = lboxParty.TabStop = false;
                btnAdd.Text = "&Save";
                newPartStatus = sStatus;

                if (sStatus > 0 && strGroup != "")
                {
                    txtGroup.Text = strGroup;
                    txtGroup.Enabled = false;
                }
                if (sStatus > 0 && strCategory != "")
                {
                    txtCategory.Text = txtPartyType.Text= strCategory;
                    txtCategory.Enabled = txtPartyType.Enabled = false;
                }
                GetAreaCode();
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public SupplierMaster(string sParty)
        {
            InitializeData(sParty);
        }

        public SupplierMaster(string sParty, int status)
        {
            InitializeData(sParty);
            update = status;
        }

        private void InitializeData(string sParty)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                pt = new PartyTransport();

                BindPartyData();
                lboxParty.SelectedItem = sParty;
                txtSearchParty.Visible = btnDelete.Enabled = txtSearchGroup.Enabled = true;
                EditOption();
            }
            catch
            {
            }
        }

        private void GetMaxAccountNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(CAST(AccountNo as int)),0)+1) from SupplierMaster ");
            txtAccountNo.Text = Convert.ToString(objValue);
        }

        private bool GetAreaCode()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select CurrencyBase from Company Where CompanyName='" + MainPage.strCompanyName + "'");
            txtAreaCode.Text = Convert.ToString(objValue);
            if (txtAreaCode.Text == "")
            {
                MessageBox.Show("Sorry ! Area Code can't be blank, Please fill area code in company master after that you can add party name", "Worning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = false;
                return false;
            }
            else
            {
                btnAdd.Enabled = btnEdit.Enabled = true;
            }
            return true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Add";
                    btnEdit.Text = "&Update";
                    EnabaleControl();
                    txtName.Focus();
                    //lboxParty.Enabled = false;                    
                }
                else
                {
                    btnEdit.Enabled = false;
                    pnlDetails.Visible = false;

                    if (ValidateControls())
                    {
                        DialogResult dr = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes && check < 1)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click Event of Submit Button in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnEdit.Enabled = true;
        }

        private bool ValidateControls()
        {
            if (txtAreaCode.Text == "")
            {
                if (!GetAreaCode())
                    return false;
            }
            if (txtName.Text == "")
            {
                MessageBox.Show(" Name can't be blank !  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            else if (txtGroup.Text == "")
            {
                MessageBox.Show("Group Name can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroup.Focus();
                return false;
            }
            else if ((txtOtherGroup.Text == "REGULAR" || txtOtherGroup.Text == "COMPOSITE") && txtGSTNo.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("GST No can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGSTNo.Focus();
                return false;
            }
            else if (txtOtherGroup.Text == "" && txtGSTNo.Text != "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Dealer Type can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOtherGroup.Focus();
                return false;
            }
            else if ((txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR") && txtState.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("State Name can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtState.Focus();
                return false;
            }
            else if ((txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR" || txtGroup.Text == "CREDITOR EXPENSE") && txtOtherGroup.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Dealer Type can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOtherGroup.Focus();
                return false;
            }
            else if ((txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR") && txtSSSName.Text == "" && MainPage.strSoftwareType == "AGENT")
            {
                MessageBox.Show("Nick Name can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSSSName.Focus();
                return false;
            }
            if (txtGroup.Text == "CREDITOR EXPENSE" || txtGroup.Text == "SUNDRY DEBTORS")
            {
                if (txtMobile.Text.Length < 10 && txtPhone.Text.Length < 5)
                {
                    MessageBox.Show("Mobile No or Phone No can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMobile.Focus();
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }
            if (chkBlackList.Checked && txtBlackList.Text == "")
            {
                MessageBox.Show("Black List Reason can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBlackList.Focus();
                return false;
            }
            if (txtPANNumber.Text != "" && txtPANNumber.Text.Length != 10)
            {
                MessageBox.Show("PAN No is not valid, Please enter valid PAN no ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPANNumber.Focus();
                return false;
            }
            else if (txtGSTNo.Text != "" && txtGSTNo.Text.Length != 15)
            {
                MessageBox.Show("GST No is not valid, Please enter valid GST no ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGSTNo.Focus();
                return false;
            }
            else if (txtGSTNo.Text != "" && txtPANNumber.Text != "")
            {
                if (txtState.Text == "")
                {
                    MessageBox.Show("State name can't be Blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtState.Focus();
                    return false;
                }
                else
                {
                    if (!txtGSTNo.Text.Contains(txtPANNumber.Text))
                    {
                        MessageBox.Show("Sorry either your Pan number is wrong or your gst no is wrong, Please enter valid PAN & GST no ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtGSTNo.Focus();
                        return false;
                    }
                    string strStateCode = txtGSTNo.Text.Substring(0, 2);
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select StateName+'|'+ISNULL((Select Top 1 Name from SupplierMaster Where AreaCode+AccountNo!='" + txtAreaCode.Text + txtAccountNo.Text + "' and GSTNo='" + txtGSTNo.Text + "'),'') PartyName from StateMaster Where StateCode='" + strStateCode + "' ");
                    string strValues = Convert.ToString(objValue);
                    string[] strValue = strValues.Split('|');
                    if (strValue.Length > 1)
                    {
                        if (strValue[0] != txtState.Text)
                        {
                            MessageBox.Show("Sorry ! State name and GST no doesn't match, please select correct GSTNo and State Name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtGSTNo.Focus();
                            return false;
                        }
                        if (strValue[1] != "")
                        {
                            DialogResult result = MessageBox.Show("Sorry ! This gst no is aleardy linked with party name : " + strValue[1] + "\nAre you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result != DialogResult.Yes)
                            {
                                txtGSTNo.Focus();
                                return false;
                            }
                        }
                    }
                }
            }

            string strBankName = "", strAccountNo = "", strIFSCCode = "", strBrandName = "", strProductType = "";
            foreach (DataGridViewRow row in dgrdBank.Rows)
            {
                strBankName = Convert.ToString(row.Cells["bankName"].Value);
                strIFSCCode = Convert.ToString(row.Cells["ifscCode"].Value);
                strAccountNo = Convert.ToString(row.Cells["accountNo"].Value);

                if (strBankName == "" && strIFSCCode == "" && strAccountNo == "")
                    dgrdBank.Rows.Remove(row);
                else if (strBankName == "")
                {
                    MessageBox.Show("Sorry ! Bank name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdBank.CurrentCell = row.Cells["bankName"];
                    dgrdBank.Focus();
                    return false;
                }
                else if (strIFSCCode == "")
                {
                    MessageBox.Show("Sorry ! IFSC code can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdBank.CurrentCell = row.Cells["ifscCode"];
                    dgrdBank.Focus();
                    return false;
                }
                else if (strAccountNo == "")
                {
                    MessageBox.Show("Sorry ! Account no can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdBank.CurrentCell = row.Cells["accountNo"];
                    dgrdBank.Focus();
                    return false;
                }
            }

            foreach (DataGridViewRow row in dgrdBrandName.Rows)
            {
                strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                strProductType = Convert.ToString(row.Cells["productType"].Value);

                if (strBrandName == "" && strProductType == "")
                    dgrdBrandName.Rows.Remove(row);
            }

            if (MainPage.strSoftwareType == "AGENT")
            {
                if (txtCategory.Text == "")
                {
                    MessageBox.Show("Category Name can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCategory.Focus();
                    return false;
                }
                else if (txtGroup.Text == "SUNDRY DEBTORS")
                {
                    if (txtMobile.Text == "")
                    {
                        MessageBox.Show("Mobile No can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMobile.Focus();
                        return false;
                    }
                    if (txtWhatsappNo.Text == "" && btnAdd.Text == "&Save")
                    {
                        MessageBox.Show("Whatsapp No can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtWhatsappNo.Focus();
                        return false;
                    }
                    if (txtReference.Text == "")
                    {
                        MessageBox.Show("Reference can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReference.Focus();
                        return false;
                    }
                    if (txtPartyType.Text == "")
                    {
                        MessageBox.Show("Party Type can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPartyType.Focus();
                        return false;
                    }
                    if ((txtAmountLimit.Text == "" || txtAmountLimit.Text == "0"))
                    {
                        MessageBox.Show("Amount limit can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtAmountLimit.Focus();
                        return false;
                    }
                    if (txtPIN.Text == "")
                    {
                        MessageBox.Show("Pin code can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPIN.Focus();
                        return false;
                    }
                    if (txtPANNumber.Text == "")
                    {
                        MessageBox.Show("PAN Number can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPANNumber.Focus();
                        return false;
                    }
                }
            }
            if (txtPIN.Text != "")
            {
                if (btnAdd.Text == "&Save" || MainPage.mymainObject.bChangeCustomerDetail)
                {
                    //if(txtadd)
                }
            }

            if (txtMSMENo.Text == "" && chkMSMENo.Checked)
            {
                MessageBox.Show("MSME no can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMSMENo.Focus();
                return false;
            }
            return true;
        }


        private void SaveRecord()
        {
            try
            {
                string[] record = new string[55];

                record[0] = txtName.Text.Trim();
                record[1] = txtCategory.Text.Trim();
                record[2] = txtGroup.Text;
                record[3] = txtOpening.Text;
                if (rdoDebit.Checked)
                {
                    record[4] = "Debit";
                    record[5] = "Dr";
                }
                else if (rdoCredit.Checked)
                {
                    record[4] = "Credit";
                    record[5] = "Cr";
                }
                else
                {
                    record[4] = "";
                    record[5] = "";
                }
                record[6] = txtAddress.Text.Replace("\n", " ").Replace("\r", " ").Replace("  ", " ").Trim();
                record[7] = txtState.Text;
                record[8] = txtPIN.Text;
                record[9] = txtTransport.Text;
                record[10] = txtStation.Text;
                record[11] = txtBookingStation.Text;
                record[12] = txtPartyType.Text;
                record[13] = txtNormalDhara.Text;
                record[14] = txtSNetDhara.Text;
                record[15] = txtContactPer.Text;
                record[16] = txtPhone.Text;
                record[17] = txtMobile.Text;
                record[18] = txtPvtMarka.Text;
                record[19] = txtReference.Text;
                record[20] = txtEmailID.Text;
                record[21] = txtDueDays.Text;
                record[22] = dba.ConvertDateInExactFormat(txtDate.Text).ToString("MM/dd/yyyy h:mm:ss tt");
                record[23] = txtPremiumDhara.Text;

                if (txtAmountLimit.Text != "")
                    record[24] = dba.ConvertObjectToDouble(txtAmountLimit.Text).ToString("0");
                else
                    record[24] = "0";

                record[25] = txtPerAddress.Text.Replace("\n", " ").Replace("\r", " ").Trim(); ;
                record[26] = pt.strFirstTransport;
                record[27] = pt.strSecondTransport;
                record[28] = chkPostage.Checked.ToString();
                record[29] = txtRemark.Text.Trim();
                record[30] = txtSchemedhara.Text;
                record[31] = txtExtendedAmt.Text;
                record[32] = txtPostage.Text;
                record[33] = chkTransaction.Checked.ToString();
                record[34] = chkBlackList.Checked.ToString();
                record[35] = txtBlackList.Text;
                record[36] = txtOtherGroup.Text;
                record[37] = txtAreaCode.Text;
                record[38] = "";
                record[39] = txtAadharNumber.Text;
                record[40] = txtSSSName.Text.Trim();
                record[41] = txtSaleIncentive.Text;
                record[42] = txtGSTNo.Text.Trim();
                record[43] = txtPANNumber.Text;
                record[44] = TaxStatus();
                record[45] = txtAccountentMobileNo.Text;
                //record[46] = txtMainPartyName.Text;
                record[47] = txtCourierName.Text;
                record[48] = txtDistrictName.Text;
                if (txtOrderAmt.Text != "" && txtOrderAmt.Text != "0")
                    record[49] = dba.ConvertObjectToDouble(txtOrderAmt.Text).ToString("0");
                else
                    record[49] = "0";
                record[50] = chkOrangeZone.Checked.ToString();
                record[51] = txtMSMENo.Text;
                record[52] = rdoActive.Checked.ToString();

                if (txtGroup.Text == "SUNDRY DEBTORS" && (MainPage.strCompanyName.Contains("STYLO") || MainPage.strCompanyName.Contains("SARAOGI")) && MainPage.strSoftwareType == "AGENT")
                {
                    record[33] = "True";
                }

                if (txtMainPartyName.Text != "")
                {
                    string[] strFullName = txtMainPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        record[46] = strFullName[0].Trim();
                }

                int count = 0;
                string strAccountNo = "", strOtherQuery = GetOtherDetailsQuery();
                if (MainPage.strOnlineDataBaseName != "" && MainPage.mymainObject.bMultiBranch)
                {
                    string strResult = dba.SaveSupplierNameInOnline(record, MainPage.strOnlineDataBaseName, ref strAccountNo, strOtherQuery);
                    if (strResult == "net")
                    {
                        MessageBox.Show("Sorry ! An error occured, Please try again later", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        count = -2;
                    }
                    else if (strResult == "error")
                        count = 0;
                    else if (strResult == "success")
                        count = 2;
                }
                else
                    count = dba.SaveSupplierMaster(record, ref strAccountNo, strOtherQuery);

                if (count > 0 & strAccountNo != "")
                {
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        string strUserRole = "CUSTOMER", strUserType = "1";
                        if (txtGroup.Text == "SUNDRY CREDITOR")
                        {
                            strUserRole = "SUPPLIER";
                            strUserType = "2";
                        }
                        else if (txtGroup.Text != "SUNDRY DEBTORS")
                        {
                            strUserRole = "EMPLOYEE";
                            strUserType = "3";
                        }
                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            AppAPI.AddNewUserinApp(txtName.Text, txtEmailID.Text, txtMobile.Text, strUserType, txtAreaCode.Text + strAccountNo, strUserRole);
                           // string strName = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
                           // AppAPI.AddNewUserinSSSAddaApp(strName, txtEmailID.Text, txtMobile.Text, strUserType, txtAreaCode.Text + strAccountNo, strUserRole, txtName.Text, txtDistrictName.Text, txtState.Text, txtGSTNo.Text, "");
                        }
                    }

                    SendSMSToParty(strAccountNo);
                    SaveAllImages(strAccountNo);
                    MessageBox.Show("Thank You ! Record saved successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    if (newPartStatus > 0)
                    {
                        strAccountName = txtAreaCode.Text + strAccountNo + " " + txtName.Text;
                        this.Close();
                        return;
                    }
                    else
                    {
                        ClearAllData();
                        BindPartyData();
                    }
                }
                else
                {
                    if (count != -2)
                        MessageBox.Show("Sorry ! An error occured, Please try again later", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Saving Record in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void UpdateRecord()
        {
            try
            {
                string[] record = new string[55];
                record[0] = txtName.Text.Trim();
                record[1] = txtCategory.Text.Trim();
                record[2] = txtGroup.Text;
                if (txtOpening.Text == "")
                    record[3] = "0.00";
                else
                    record[3] = txtOpening.Text;
                if (rdoDebit.Checked)
                {
                    record[4] = "Debit";
                    record[5] = "Dr";
                }
                else if (rdoCredit.Checked)
                {
                    record[4] = "Credit";
                    record[5] = "Cr";
                }
                else
                {
                    record[4] = "";
                    record[5] = "";
                }
                record[6] = txtAddress.Text.Replace("\n", " ").Replace("\r", " ").Replace("  ", " ").Trim(); 
                record[7] = txtState.Text;
                record[8] = txtPIN.Text;
                record[9] = txtTransport.Text;
                record[10] = txtStation.Text;
                record[11] = txtBookingStation.Text;
                record[12] = txtPartyType.Text;
                record[13] = txtNormalDhara.Text;
                record[14] = txtSNetDhara.Text;
                record[15] = txtContactPer.Text;
                record[16] = txtPhone.Text;
                record[17] = txtMobile.Text;
                record[18] = txtPvtMarka.Text;
                record[19] = txtReference.Text;
                record[20] = txtEmailID.Text;
                record[21] = txtDueDays.Text;
                record[22] = dba.ConvertDateInExactFormat(txtDate.Text).ToString("MM/dd/yyyy h:mm:ss tt");
                record[23] = txtPremiumDhara.Text;

                if (txtAmountLimit.Text != "")
                    record[24] = dba.ConvertObjectToDouble(txtAmountLimit.Text).ToString("0");
                else
                    record[24] = "0";

                record[25] = txtPerAddress.Text.Replace("\n", " ").Replace("\r", " ").Replace("  "," ").Trim();
                record[26] = pt.strFirstTransport;
                record[27] = pt.strSecondTransport;
                record[28] = chkPostage.Checked.ToString();
                record[29] = txtRemark.Text.Trim();
                record[30] = txtSchemedhara.Text;
                record[31] = txtExtendedAmt.Text;
                record[32] = txtPostage.Text;
                record[33] = chkTransaction.Checked.ToString();
                record[34] = chkBlackList.Checked.ToString();
                record[35] = txtBlackList.Text;
                record[36] = txtOtherGroup.Text;
                record[37] = txtAreaCode.Text;
                record[38] = txtAccountNo.Text;
                record[39] = txtAadharNumber.Text;
                record[40] = txtSSSName.Text.Trim();
                record[41] = txtSaleIncentive.Text;
                record[42] = txtGSTNo.Text;
                record[43] = txtPANNumber.Text;
                record[44] = TaxStatus();
                record[45] = txtAccountentMobileNo.Text;
                //record[46] = txtMainPartyName.Text;
                record[47] = txtCourierName.Text;
                record[48] = txtDistrictName.Text;
                if (txtOrderAmt.Text != "" && txtOrderAmt.Text != "0")
                    record[49] = dba.ConvertObjectToDouble(txtOrderAmt.Text).ToString("0");
                else
                    record[49] = "0";
                record[50] = chkOrangeZone.Checked.ToString();
                record[51] = txtMSMENo.Text;
                record[52] = rdoActive.Checked.ToString();

                if (txtMainPartyName.Text != "")
                {
                    string[] strFullName = txtMainPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        record[46] = strFullName[0].Trim();
                }

                //if (txtGroup.Text == "SUNDRY DEBTORS" && txtGroup.Enabled)
                //{
                //    if (MainPage.strCompanyName.Contains("STYLO") || MainPage.strCompanyName.Contains("SARAOGI"))
                //    {
                //        record[33] = "True";
                //    }
                //}

                SaveAllImages(txtAccountNo.Text);
                string strParty = Convert.ToString(lboxParty.SelectedItem), strOtherQuery = GetOtherDetailsQuery();

                if (strParty != "" && txtAccountNo.Text != "")
                {
                    strOtherQuery = strOtherQuery.Replace("@ID", txtAccountNo.Text);

                    int count = dba.UpdateSupplierMaster(record, strParty, strOtherQuery);
                    if (count > 0)
                    {
                        if (strOldMobileNumber != txtMobile.Text && txtMobile.Text != "")
                        {
                            if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                            {
                                AppAPI _app = new SSS.AppAPI();
                                bool _status = AppAPI.UpdateMobileNoInApp(txtAreaCode.Text + txtAccountNo.Text, strOldMobileNumber, txtMobile.Text);
                                if (!_status)
                                {
                                    DialogResult _updateResult = MessageBox.Show("Sorry ! Mobile no not updated right now !! ", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                                    if (_updateResult == DialogResult.Retry)
                                    {
                                        _status = AppAPI.UpdateMobileNoInApp(txtAreaCode.Text + txtAccountNo.Text, strOldMobileNumber, txtMobile.Text);
                                        if (!_status)
                                            MessageBox.Show("Sorry ! Mobile no not updated right now, Please try later !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }

                        //if ((strOldMobileNumber != txtMobile.Text && txtMobile.Text != "") || (strOldEmailID != txtEmailID.Text && txtEmailID.Text != "") || (strOldStateName != txtState.Text && txtState.Text != "") || (strOldStation != txtStation.Text && txtStation.Text != "") || (strParty != (txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text)))
                        //{
                        //    if (txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR")
                        //    {
                        //        string strUserRole = "CUSTOMER", strUserType = "1";
                        //        if (txtGroup.Text == "SUNDRY CREDITOR")
                        //        {
                        //            strUserRole = "SUPPLIER";
                        //            strUserType = "2";
                        //        }
                        //        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        //        {
                        //            string strID = AppAPI.GetSSSAddaID(strUserRole, strOldMobileNumber);
                        //            string strName = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
                        //            string strResponse = AppAPI.AddNewUserinSSSAddaApp(strName, txtEmailID.Text, txtMobile.Text, strUserType, txtAreaCode.Text + txtAccountNo.Text, strUserRole, txtName.Text, txtDistrictName.Text, txtState.Text, txtGSTNo.Text, strID);
                        //            if (strResponse != "")
                        //                MessageBox.Show(strResponse, "SSS Adda update response", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        //        }
                        //    }
                        //}
                       
                        MessageBox.Show("Thank you ! Record updated successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dtable = dba.GetPartyNameGroupFullNameRecord();
                        // ClearAllData();
                        if (update > 0)
                        {
                            strMobileNumber = txtMobile.Text;
                            this.Close();
                        }
                        btnEdit.Text = "&Edit";
                        lboxParty.Enabled = true;
                        pnlDetails.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Try again, Party name or account name not found ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Updating Record in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ClearAllData()
        {
            try
            {

                lblChequeStatus.Text = strOldMobileNumber = lblCreatedBy.Text = txtSSSName.Text = txtNormalDhara.Text = txtPremiumDhara.Text = txtAddress.Text = txtExtendedAmt.Text = txtSchemedhara.Text = txtContactPer.Text = txtAmountLimit.Text = txtPvtMarka.Text = txtDueDays.Text = txtOrderAmt.Text = "";
                txtStation.Text = txtEmailID.Text = txtMobile.Text = txtWhatsappNo.Text = txtName.Text = txtOpening.Text = txtPhone.Text = txtPIN.Text = txtReference.Text = txtPartyType.Text = txtSNetDhara.Text = "";
                txtPerAddress.Text = txtRemark.Text = txtPostage.Text = txtBlackList.Text = txtBookingStation.Text = txtDistrictName.Text = txtTransport.Text = txtCategory.Text = "";
                txtGroup.Text = txtState.Text = txtOtherGroup.Text = pt.txtTransportI.Text = pt.txtTransportII.Text = txtAadharNumber.Text = txtSaleIncentive.Text = txtGSTNo.Text = txtPANNumber.Text = "";
                txtMainPartyName.Text = txtCourierName.Text = txtAccountentMobileNo.Text = txtImportParty.Text = txtCompanyRegNo.Text = txtReligion.Text = txtNameOfFirm.Text = "";
                chkAgent.Checked = chkAssembler.Checked = chkDealer.Checked = chkManf.Checked = chkPartnership.Checked = chkPrivate.Checked = chkProprietary.Checked = chkPublic.Checked = chkSoleAgent.Checked = chkTrader.Checked = false;
                chkPick.Checked = txtImportParty.Enabled = chkOrangeZone.Checked =chkMSMENo.Checked= false;

                picProfile1.ImageLocation = picProfile2.ImageLocation = picProfile3.ImageLocation = "";
                picProfile1.Image = picProfile2.Image = picProfile3.Image = (Image)ObjDummyProfile;
                btnRemovePic1.Visible = btnRemovePic2.Visible = btnRemovePic2.Visible = false;
                //bPic1Ch = bPic2Ch = bPic3Ch = false;

                chkPostage.Checked = true;
                lblMsg.Visible = chkBlackList.Checked = chkTransaction.Checked = chkWithScheme.Checked = false;
                txtPartyBankAccountNo.Text = txtDOA.Text = txtDOB.Text = txtSpouse.Text = "";
                dgrdBank.Rows.Clear();
                dgrdBrandName.Rows.Clear();

                dgrdBank.Rows.Add();
                dgrdBrandName.Rows.Add();
                txtName.BackColor = Color.White;
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            }
            catch
            {
            }
        }

        private void txtMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (txtMobile.Text.Length == 10 && !MainPage.mymainObject.bChangeCustomerDetail && txtGroup.Text == "SUNDRY DEBTORS" && btnAdd.Text != "&Save")
                    txtMobile.ReadOnly = true;
                else
                    txtMobile.ReadOnly = false;

                KeyHandler(e);
            }
            catch { }
        }

        private void txtPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void KeyHandler(KeyPressEventArgs e)
        {
            try
            {
                //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                //{
                Char pressedKey = e.KeyChar;
                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
                //}
                //else
                //    e.Handled = true;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Key Handler in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void SupplierMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (picProfile1.Width == 360)
                        ZoomOut(picProfile1, grpPic1);
                    else if (picProfile2.Width == 360)
                        ZoomOut(picProfile2, grpPic2);
                    else if (picProfile3.Width == 360)
                        ZoomOut(picProfile3, grpPic3);

                    else if (pnlDetails.Visible)
                        pnlDetails.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !txtAddress.Focused && !txtPerAddress.Focused && !txtBlackList.Focused && !dgrdBank.Focused && !dgrdBrandName.Focused)
                {
                    SendKeys.Send("{Tab}");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Key Down Event of Form in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void rdoDebit_Leave(object sender, EventArgs e)
        {
            if (!rdoDebit.Checked && !rdoCredit.Checked && txtOpening.Text != "")
            {
                MessageBox.Show("Amount status can't be Blank  ");
                rdoDebit.Focus();
            }
        }

        private void BindDataWithControls(string strParty)
        {
            try
            {
                DataSet ds = dba.GetPartyInfo(strParty);
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                        BindPartyRecordFromDataRow(dt.Rows[0]);
                    if (ds.Tables[1].Rows.Count > 0)
                        SupplierOtherDetails(ds.Tables[1].Rows[0]);

                    SupplierBankDetails(ds.Tables[2]);
                    SupplierBrandDetails(ds.Tables[3]);

                    DisableControl();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding All Data with Controls  in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindPartyRecordFromDataRow(DataRow dr)
        {
            ClearAllData();
            txtAreaCode.Text = Convert.ToString(dr["AreaCode"]);
            txtAccountNo.Text = Convert.ToString(dr["AccountNo"]);
            txtAadharNumber.Text = Convert.ToString(dr["CardNumber"]);
            txtName.Text = Convert.ToString(dr["Name"]);
            txtCategory.Text = Convert.ToString(dr["Category"]);
            txtGroup.Text = Convert.ToString(dr["GroupName"]).ToUpper();
            txtOtherGroup.Text = Convert.ToString(dr["GroupII"]);

            txtOpening.Text = dba.ConvertObjectToDouble(dr["OpeningBal"]).ToString("0.00");
            if (Convert.ToString(dr["Status"]).ToUpper() == "DEBIT")
                rdoDebit.Checked = true;
            else
                rdoCredit.Checked = true;

            txtAddress.Text = Convert.ToString(dr["Address"]);
            strOldStateName = txtState.Text = Convert.ToString(dr["State"]);
            txtPIN.Text = Convert.ToString(dr["PINCode"]);
            txtTransport.Text = Convert.ToString(dr["Transport"]);
            txtDistrictName.Text = Convert.ToString(dr["DistrictName"]);
            strOldStation = txtStation.Text = Convert.ToString(dr["Station"]);
            txtBookingStation.Text = Convert.ToString(dr["BookingStation"]);
            txtPartyType.Text = Convert.ToString(dr["TINNumber"]);
            txtNormalDhara.Text = Convert.ToString(dr["NormalDhara"]);
            txtSNetDhara.Text = Convert.ToString(dr["SNDhara"]);
            txtPremiumDhara.Text = Convert.ToString(dr["CFormApply"]);
            txtContactPer.Text = Convert.ToString(dr["ContactPerson"]);
            txtPhone.Text = Convert.ToString(dr["PhoneNo"]);
            strOldMobileNumber = txtMobile.Text = Convert.ToString(dr["MobileNo"]);
            txtPvtMarka.Text = Convert.ToString(dr["PvtMarka"]);
            txtReference.Text = Convert.ToString(dr["Reference"]);
            strOldEmailID = txtEmailID.Text = Convert.ToString(dr["EmailID"]);
            txtDueDays.Text = Convert.ToString(dr["DueDays"]);
            txtDate.Text = Convert.ToDateTime(dr["Date"]).ToString("dd/MM/yyyy");
            txtAmountLimit.Text = dba.ConvertObjectToDouble(dr["AmountLimit"]).ToString("N0", MainPage.indianCurancy);
            txtExtendedAmt.Text = dba.ConvertObjectToDouble(dr["ExtendedAmt"]).ToString("N0", MainPage.indianCurancy);
            txtPerAddress.Text = Convert.ToString(dr["PermanentAddress"]);
            pt.txtTransportI.Text = Convert.ToString(dr["SecondTransport"]);
            pt.txtTransportII.Text = Convert.ToString(dr["ThirdTransport"]);
            string str = Convert.ToString(dr["FourthTransport"]).ToUpper();
            if (str == "FALSE")
                chkPostage.Checked = false;
            else
                chkPostage.Checked = true;

            //pt.txtTransportIII.Text = Convert.ToString(dr["FourthTransport"]);
            txtRemark.Text = Convert.ToString(dr["Remark"]);
            txtSchemedhara.Text = Convert.ToString(dr["CDDays"]);
            txtPostage.Text = Convert.ToString(dr["Postage"]);
            chkTransaction.Checked = Convert.ToBoolean(dr["TransactionLock"]);
            chkBlackList.Checked = Convert.ToBoolean(dr["BlackList"]);
            txtBlackList.Text = Convert.ToString(dr["BlackListReason"]);
            txtSaleIncentive.Text = Convert.ToString(dr["SaleIncentive"]);
            txtGSTNo.Text = Convert.ToString(dr["GSTNo"]);
            txtPANNumber.Text = Convert.ToString(dr["PANNumber"]);
            txtSSSName.Text = Convert.ToString(dr["Other"]);

            txtAccountentMobileNo.Text = Convert.ToString(dr["AccountantMobileNo"]);
            txtCourierName.Text = Convert.ToString(dr["CourierName"]);
            txtMainPartyName.Text = Convert.ToString(dr["_MainPartyName"]);
            txtMSMENo.Text = Convert.ToString(dr["Other2"]);
            string strOrange = Convert.ToString(dr["Other1"]);
            if (strOrange.ToUpper().Contains("TRUE"))
                chkOrangeZone.Checked = true;
            else
                chkOrangeZone.Checked = false;

            string strActive = Convert.ToString(dr["Other3"]).ToUpper();
            if (strActive.Contains("FALSE"))
                rdoInactive.Checked = true;
            else
                rdoActive.Checked = true;

            if (txtGroup.Text == "SUNDRY DEBTORS")
            {
                txtPartyBankAccountNo.Text = "SASUSP" + dba.ConvertObjectToDouble(dr["AccountNo"]).ToString("000000");
                btnPrintDetails.Enabled = btnPrint.Enabled = true;
                btnSendSMS.Enabled = btnSendShippingDetail.Enabled = MainPage.mymainObject.bSMSReport;
                lblPvtMarka.Text = "Pvt Marka :";
            }
            else
            {
                txtPartyBankAccountNo.Text = "";
                btnPrintDetails.Enabled = btnSendShippingDetail.Enabled = btnPrint.Enabled = btnSendSMS.Enabled = false;
                lblPvtMarka.Text = "Short Name :";
            }
         
         
            if (txtReligion.Text != "")
            {
                if (txtReligion.Text != "HINDU" && txtReligion.Text != "MUSLIM" && txtReligion.Text != "CHRISTIAN" && txtReligion.Text != "SIKH" && txtReligion.Text != "BUDDHIST" && txtReligion.Text != "JAIN" && txtReligion.Text != "OTHER")
                    txtReligion.Text = "";
            }

            lblCreatedBy.Text = "";
            string strCreatedBy = Convert.ToString(dr["CreatedBy"]), strUpdatedBy = Convert.ToString(dr["UpdatedBy"]);
            if (strCreatedBy != "")
                lblCreatedBy.Text = "Created By : " + strCreatedBy;
            if (strUpdatedBy != "")
                lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;

            if (Convert.ToString(dr["TaxType"]) == "EXCLUDED")
                rdoExcluded.Checked = true;
            else if (Convert.ToString(dr["TaxType"]) == "INCLUDED")
                rdoIncluded.Checked = true;
            else
                rdoNone.Checked = true;

            if ((txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR") && MainPage.strUserRole != "SUPERADMIN")
                txtGroup.Enabled = false;
            else
                txtGroup.Enabled = true;
            if (Convert.ToString(dr["ChqDate"]) != "")
                lblChequeStatus.Text = "Security cheque received on Date: " + dr["ChqDate"];
            else
                lblChequeStatus.Text = "";

            chkMSMENo.Checked = txtMSMENo.Text == "" ? false : true;


            if (chkTransaction.Checked)
                txtName.BackColor = Color.Gold;
            else if (chkBlackList.Checked)
                txtName.BackColor = Color.Tomato;
            else if (lblChequeStatus.Text != "")
                txtName.BackColor = Color.LightGreen;
            else
                txtName.BackColor = Color.White;

            if (txtGroup.Text == "SUNDRY DEBTORS")
                chkPostage.Enabled = txtPostage.Enabled = true;
            else
                chkPostage.Enabled = txtPostage.Enabled = false;

            if (dr.Table.Columns.Contains("OrderAmtLimit"))
                txtOrderAmt.Text = dba.ConvertObjectToDouble(dr["OrderAmtLimit"]).ToString("N0", MainPage.indianCurancy);
        }

        private void SupplierOtherDetails(DataRow row)
        {
            try
            {
                txtWhatsappNo.Text = Convert.ToString(row["WaybillUserName"]);
                // txtWayBillPwd.Text = Convert.ToString(row["WaybillPassword"]);
                txtCompanyRegNo.Text = Convert.ToString(row["CompanyRegNo"]);
                txtNameOfFirm.Text = Convert.ToString(row["NameOfFirm"]);
                txtReligion.Text = Convert.ToString(row["OtherDetails"]);
                txtDOA.Text = Convert.ToString(row["NDOA"]);
                txtDOB.Text = Convert.ToString(row["NDOB"]);
                txtSpouse.Text = Convert.ToString(row["SpouseName"]);

                chkManf.Checked = Convert.ToBoolean(row["NB_Manufacturing"]);
                chkSoleAgent.Checked = Convert.ToBoolean(row["NB_SoleSellingAgent"]);
                chkDealer.Checked = Convert.ToBoolean(row["NB_Dealer"]);
                chkAgent.Checked = Convert.ToBoolean(row["NB_Agent"]);
                chkAssembler.Checked = Convert.ToBoolean(row["NB_Assembler"]);
                chkTrader.Checked = Convert.ToBoolean(row["NB_Trader"]);
                chkProprietary.Checked = Convert.ToBoolean(row["NC_Proprietary"]);
                chkPartnership.Checked = Convert.ToBoolean(row["NC_Partnership"]);
                chkPrivate.Checked = Convert.ToBoolean(row["NC_Private"]);
                chkPublic.Checked = Convert.ToBoolean(row["NC_Public"]);

                if (txtDOB.Text == "01/01/1900")
                    txtDOB.Clear();
                if (txtDOA.Text == "01/01/1900")
                    txtDOA.Clear();
                ShowSavedProfiles(row);
            }
            catch { }
        }

        private void SupplierBankDetails(DataTable dt)
        {
            try
            {
                dgrdBank.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    dgrdBank.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    bool _bVStatus = false;
                    foreach (DataRow row in dt.Rows)
                    {
                        _bVStatus = Convert.ToBoolean(row["VerifiedStatus"]);
                        dgrdBank.Rows[_rowIndex].Cells["id"].Value = row["ID"];
                        dgrdBank.Rows[_rowIndex].Cells["bankName"].Value = row["BankName"];
                        dgrdBank.Rows[_rowIndex].Cells["branchName"].Value = row["BranchName"];
                        dgrdBank.Rows[_rowIndex].Cells["accountNo"].Value = row["BankAccountNo"];
                        dgrdBank.Rows[_rowIndex].Cells["ifscCode"].Value = row["BankIFSCCode"];
                        dgrdBank.Rows[_rowIndex].Cells["accountName"].Value = row["BankAccountName"];
                        dgrdBank.Rows[_rowIndex].Cells["verifiedDate"].Value = row["VDate"];
                        dgrdBank.Rows[_rowIndex].Cells["beniID"].Value = row["BeniID"];
                        dgrdBank.Rows[_rowIndex].Cells["accountVerified"].Value = _bVStatus;
                        if (_bVStatus)
                        {
                            dgrdBank.Rows[_rowIndex].Cells["verifyButton"].Value = "Unverify";
                            dgrdBank.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                        else
                            dgrdBank.Rows[_rowIndex].Cells["verifyButton"].Value = "Verify";
                        _rowIndex++;
                    }
                }
                dgrdBank.Rows.Add();
            }
            catch { }
        }

        private void SupplierBrandDetails(DataTable dt)
        {
            try
            {
                dgrdBrandName.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    dgrdBrandName.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdBrandName.Rows[_rowIndex].Cells["brandName"].Value = row["BrandName"];
                        dgrdBrandName.Rows[_rowIndex].Cells["productType"].Value = row["ProductType"];
                        dgrdBrandName.Rows[_rowIndex].Cells["range"].Value = row["Range"];
                        _rowIndex++;
                    }
                }
                dgrdBrandName.Rows.Add();
            }
            catch { }
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (txtName.Text != "")
                    {
                        check = dba.CheckPartyAvailability(txtName.Text);
                        if (check < 1)
                        {
                            lblMsg.Text = txtName.Text + "  is Available ........";
                            lblMsg.ForeColor = Color.Green;
                            lblMsg.Visible = true;
                            //lboxParty.Visible = false;
                        }
                        else
                        {
                            lblMsg.Text = txtName.Text + "  is Already exist  ! Please choose another Name..";
                            lblMsg.ForeColor = Color.Red;
                            lblMsg.Visible = true;
                            // txtName.Focus();
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Party Name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        //  txtName.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtName.Text != "" && MainPage.strUserRole.Contains("ADMIN"))
                {
                    string strParty = Convert.ToString(lboxParty.SelectedItem);
                    string[] strSplitName = strParty.Split(' ');
                    if (strSplitName.Length > 0)
                    {
                        if (CheckBalanceofName(strSplitName[0]))
                        {
                            DialogResult dr = MessageBox.Show("Are you sure  want to  Delete : " + strParty + "  .....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                int count = dba.DeletePartyRecord(strSplitName[0]);
                                if (count > 0)
                                {
                                    MessageBox.Show(txtName.Text + " is successfully Deleted ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    ClearAllData();
                                    txtSearchParty.Clear();
                                    dtable = dba.GetPartyNameGroupFullNameRecord();
                                    BindPartyData();
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! Unable to Delete : " + txtName.Text, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! This Account have Some Record,  and can't be Deleted , Please Remove All reference of this Account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Account Name nis not in correct format !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Please provide Name for Deletion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Deletion of Party in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool CheckBalanceofName(string strParty)
        {
            bool amountStatus = false;
            try
            {
                string strQuery = " Select ISNULL(SUM(result),0)Result from (Select Count(*) result from GoodsReceive Where PurchasePartyID='" + strParty + "' or SalePartyID ='" + strParty + "' union all Select Count(*) result from SalesRecord Where SalePartyID='" + strParty + "' union all Select Count(*) result from SalesEntry Where PurchasePartyID='" + strParty + "' union all Select Count(*) result from PurchaseRecord Where SalePartyID='" + strParty + "' or PurchasePartyID='" + strParty + "' union all Select Count(*) result from BalanceAmount Where AccountID='" + strParty + "' and CAST(Amount as Money)>0) AccountMaster ";
                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (objValue != null)
                {
                    double dBalance = Convert.ToDouble(objValue);
                    if (dBalance <= 0)
                        amountStatus = true;
                }
            }
            catch
            {
            }
            return amountStatus;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
            {
                BindListData();
            }
        }

        private void BindListData()
        {
            try
            {
                if (dtable != null)
                {
                    if (txtName.Text == "")
                    {
                        lboxParty.Items.Clear();
                        foreach (DataRow dr in dtable.Rows)
                        {
                            lboxParty.Items.Add(dr["FullName"]);
                        }
                    }
                    else
                    {

                        DataRow[] filteredRows = dtable.Select(string.Format("{0} LIKE '%{1}%'", "Name", txtName.Text));
                        if (filteredRows.Length > 0)
                        {
                            lboxParty.Items.Clear();
                            foreach (DataRow dr in filteredRows)
                            {
                                lboxParty.Items.Add(dr["FullName"]);
                            }
                        }
                    }
                }
                if (lboxParty.Items.Count > 0)
                {
                    lboxParty.SelectedIndex = 0;
                    //lboxParty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bindning List data  in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void txtAddress_Leave(object sender, EventArgs e)
        {
            if (txtPerAddress.Text == "")
            {
                txtPerAddress.Text = txtAddress.Text;
            }
        }

        private void BindPartyData()
        {
            dtable = dba.GetPartyNameGroupFullNameRecord();
            if (dtable != null)
            {
                lboxParty.Items.Clear();
                foreach (DataRow dr in dtable.Rows)
                {
                    lboxParty.Items.Add(dr["FullName"]);
                }
                if (lboxParty.Items.Count > 0)
                {
                    lboxParty.SelectedIndex = 0;
                }
                //lboxParty.Visible = true;
            }
        }

        private void lboxParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strParty = Convert.ToString(lboxParty.SelectedItem);
                    if (strParty != "")
                    {
                        strSelectedName = strParty;
                        BindDataWithControls(strParty);
                    }
                }
                else if (btnEdit.Text == "&Update")
                    lboxParty.SelectedItem = strSelectedName;
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void txtAmountLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAdat_Enter(object sender, EventArgs e)
        {
            if (txtNormalDhara.Text == "0")
            {
                txtNormalDhara.Clear();
            }
        }

        private void txtAdat_Leave(object sender, EventArgs e)
        {
            if (txtNormalDhara.Text == "")
            {
                txtNormalDhara.Text = "0";
            }
        }

        private void txtSNetDhara_Enter(object sender, EventArgs e)
        {
            if (txtSNetDhara.Text == "0")
            {
                txtSNetDhara.Clear();
            }
        }

        private void txtSNetDhara_Leave(object sender, EventArgs e)
        {
            if (txtSNetDhara.Text == "")
            {
                txtSNetDhara.Text = "0";
            }
        }

        private void txtSNetDhara_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPointHandler(e);
        }

        private void txtAdat_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPointHandler(e);
        }

        private void KeyPointHandler(KeyPressEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    Char pressedKey = e.KeyChar;
                    if (pressedKey == (Char)46)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                }
                else
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Handler in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void lnkTransport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pt.ShowDialog();
        }

        private void txtSearchParty_TextChanged(object sender, EventArgs e)
        {
            BindSearchListData();
        }

        private void BindSearchListData()
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strGroup = txtSearchGroup.Text;
                    if (dtable != null)
                    {
                        lboxParty.Items.Clear();
                        if (strGroup == "")
                        {
                            if (txtSearchParty.Text == "")
                            {
                                foreach (DataRow dr in dtable.Rows)
                                {
                                    lboxParty.Items.Add(dr["FullName"]);
                                }
                                if (lboxParty.Items.Count > 0)
                                    lboxParty.SelectedIndex = 0;
                            }
                            else
                            {

                                DataRow[] filteredRows = dtable.Select(string.Format("{0} LIKE '%{1}%'", "FullName", txtSearchParty.Text));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxParty.Items.Add(dr["FullName"]);
                                    }
                                    lboxParty.SelectedIndex = 0;
                                }
                            }
                        }
                        else
                        {
                            if (txtSearchParty.Text == "")
                            {
                                DataRow[] filteredRows = dtable.Select(string.Format("GroupName='" + strGroup + "'"));
                                foreach (DataRow dr in filteredRows)
                                {
                                    lboxParty.Items.Add(dr["FullName"]);
                                }
                                if (lboxParty.Items.Count > 0)
                                    lboxParty.SelectedIndex = 0;
                            }
                            else
                            {

                                DataRow[] filteredRows = dtable.Select(string.Format("GroupName='" + strGroup + "' and FullName LIKE ('%" + txtSearchParty.Text + "%')"));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxParty.Items.Add(dr["FullName"]);
                                    }
                                    lboxParty.SelectedIndex = 0;
                                }
                            }
                        }
                        //lboxParty.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bindning Search List data  in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtSearchParty_KeyDown(object sender, KeyEventArgs e)
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
            }
            catch
            {
            }
        }

        private void lboxParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                txtSearchParty.Text += e.KeyChar.ToString();
                txtSearchParty.Focus();
                txtSearchParty.Select(txtSearchParty.Text.Length, 0);
            }
            else if (e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Space))
            {
                txtSearchParty.Focus();
                txtSearchParty.Select(txtSearchParty.Text.Length, 0);
            }
        }

        private void txtCDDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler(e);
        }


        private void GetPartyNameByGroup()
        {
            try
            {
                txtSearchParty.Clear();
                lboxParty.Items.Clear();
                if (dtable != null)
                {
                    if (txtSearchGroup.Text != "")
                    {
                        DataRow[] dr = dtable.Select(String.Format("GroupName='" + txtSearchGroup.Text + "'"));
                        foreach (DataRow row in dr)
                        {
                            lboxParty.Items.Add(row["FullName"]);
                        }
                        if (lboxParty.Items.Count > 0)
                            lboxParty.SelectedIndex = 0;
                    }
                    else
                    {
                        foreach (DataRow dr in dtable.Rows)
                        {
                            lboxParty.Items.Add(dr["FullName"]);
                        }
                        if (lboxParty.Items.Count > 0)
                            lboxParty.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            dba.ValidateSpace(sender, e);
            // else
            //   e.Handled = true;
        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtAddress.SelectionStart < 2)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void txtPerAddress_KeyDown(object sender, KeyEventArgs e)
        {
            // if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            // {
            if (e.KeyCode == Keys.Enter && txtPerAddress.SelectionStart < 2)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{TAB}");
            }
            //  }
            // else
            //    e.Handled = true;
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void EditOption()
        {
            try
            {
                btnDelete.Enabled = false;
                if (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bPartyMasterView)
                {
                    if (!(MainPage.mymainObject.bPartyMasterAdd))
                        btnAdd.Enabled = false;
                    if (!(MainPage.mymainObject.bPartyMasterEdit))
                        btnEdit.Enabled = false;

                    if ((MainPage.mymainObject.bPartyMasterEdit) && MainPage.strUserRole.Contains("ADMIN"))
                        btnDelete.Enabled = true;

                    if (!(MainPage.mymainObject.bPartyMasterView))
                    {
                        lboxParty.Enabled = false;
                    }
                    btnOtherDetails.Enabled = MainPage.mymainObject.bSupplierOtherDetails;

                    if (MainPage.mymainObject.bPartyMasterEdit && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strUserRole.Contains("ADMIN") && MainPage.strSoftwareType == "AGENT")
                        btnSendtoApp.Enabled = true;
                    else
                        btnSendtoApp.Enabled = false;

                    btnRemovePic1.Enabled = btnRemovePic2.Enabled = btnRemovePic3.Enabled = btnUpload1.Enabled = btnUpload2.Enabled = btnUpload3.Enabled = (MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bPartyMasterAdd) ? true : false;
                    btnSendSMS.Enabled = btnSendShippingDetail.Enabled = MainPage.mymainObject.bSMSReport;

                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }

                if (!MainPage.mymainObject.bPartyMasterAdd && !MainPage.mymainObject.bPartyMasterEdit)
                    btnDownload.Enabled = false;
            }
            catch
            {
            }
        }

        private void txtEmailID_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (txtEmailID.Text != "")
                {

                    bool chk = System.Text.RegularExpressions.Regex.IsMatch(txtEmailID.Text, @"^[a-zA-Z0-9][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
                    if (!chk)
                    {
                        txtEmailID.ForeColor = Color.Red;
                        MessageBox.Show("Sorry ! Email id is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("ADMIN"))
                            e.Cancel = true;
                    }
                    else
                        txtEmailID.ForeColor = Color.Black;
                }
                else
                    txtEmailID.ForeColor = Color.Black;
            }
            catch
            {
            }
        }

        private void chkBlackList_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkBlackList.Checked || chkTransaction.Checked || chkOrangeZone.Checked)
            //    txtBlackList.Enabled = true;
            //else
            //    txtBlackList.Enabled = false;
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CATEGORYNAME", txtGroup.Text, "SEARCH CATEGORY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtCategory.Text = objSearch.strSelectedData;
                            if (txtGroup.Text == "SUNDRY DEBTORS" && MainPage.strSoftwareType == "AGENT")
                            {
                                if (txtCategory.Text == "WHOLESALER")
                                    txtDueDays.Text = "60";
                                else
                                    txtDueDays.Text = "45";
                            }
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtGroup.Text = objSearch.strSelectedData;

                        if (txtGroup.Text == "SUNDRY DEBTORS")
                        {
                            chkPostage.Enabled = txtPostage.Enabled = true;
                            if (!MainPage.mymainObject.bAddNewCustomer)
                            {
                                MessageBox.Show("Sorry ! You don't have sufficient permission to create customer account.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtGroup.Text = "";
                            }
                            lblPvtMarka.Text = "Pvt Marka :";
                        }
                        else
                        {
                            if (txtGroup.Text == "SUNDRY CREDITOR")
                                chkMSMENo.Checked = true;

                            chkPostage.Enabled = txtPostage.Enabled = false;
                            lblPvtMarka.Text = "Short Name :";
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtOtherGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("OTHERGROUPNAME", "SEARCH DEALER TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtOtherGroup.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtStation.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtState_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string _strState = txtState.Text;
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtState.Text = objSearch.strSelectedData;
                        if (_strState != txtState.Text)
                            txtDistrictName.Text = "";
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransport.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtBookingStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBookingStation.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtSearchGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSearchGroup.Text = objSearch.strSelectedData;
                        GetPartyNameByGroup();
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, false);
        }

        private void txtSaleIncentive_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            dba.KeyHandlerPoint(sender, e, 2);
            // else
            //   e.Handled = true;
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            // if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            dba.ValidateSpace(sender, e);
            // else
            //   e.Handled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    txtName.Text = "";
                    btnAdd.Text = "&Save";
                    btnEdit.Text = "&Edit";
                    txtGroup.Enabled = true;
                    ClearAllData();
                    GetAreaCode();
                    GetMaxAccountNo();
                    EnabaleControl();
                    txtName.Focus();
                }
                else
                {
                    btnAdd.Enabled = false;
                    if (ValidateControls())
                    {
                        DialogResult dr = MessageBox.Show("Are you sure you want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes && check < 1)
                        {
                            int check1 = dba.CheckPartyAvailability(txtName.Text);
                            if (check1 < 1)
                            {
                                SaveRecord();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                if (btnAdd.Text == "&Save")
                {
                    btnAdd.Text = "&Add";
                    BindPartyData();
                }

                lboxParty.Enabled = true;
                txtSearchGroup.Visible = txtSearchParty.Visible = true;
                if (lboxParty.Items.Count > 0)
                    lboxParty.SelectedIndex = 0;
            }
            catch
            {
            }
        }

        private void SupplierMaster_Load(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save")
                    txtSearchParty.Focus();
                EditOption();

                if (MainPage.strSoftwareType == "AGENT" && MainPage.strCompanyName.Contains("SARAOGI"))
                {
                    lblAddaDhara.Text = "Adda Dhara :";
                    picOrange.Visible = chkOrangeZone.Visible = true;
                }
                else
                {
                    lblMobileNoText.Text = "Mobile No :";
                    lblAddaDhara.Text = "Scheme Dhara:";
                    picOrange.Visible = chkOrangeZone.Visible = false;
                }
                
            }
            catch { }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download SUNDRY DEBTORS ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadMaster(MainPage.strOnlineDataBaseName);
                        count += dba.DownloadMasterPurchaseParty(MainPage.strOnlineDataBaseName);
                        count += dba.DownloadMasterOtherParty(MainPage.strOnlineDataBaseName);
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! New party master downloaded successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("No master found  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please enter online databse name and Live IP in company master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnDownload.Enabled = true;
        }

        private string TaxStatus()
        {
            if (rdoExcluded.Checked)
                return "EXCLUDED";
            else if (rdoIncluded.Checked)
                return "INCLUDED";
            else
                return "NONE";
        }

        private void txtImportParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {

                        SearchDataOnOld objSearch = new SearchDataOnOld("ALLPARTY", "", "SEARCH PARTY NAME", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportParty.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
                    }
                    else
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetDataFromLocal()
        {
            if (txtImportParty.Text != "" && btnAdd.Text == "&Save")
            {
                string strQuery = "Select * from SupplierMaster Where Name='" + txtImportParty.Text + "' ";
                SearchDataOnOld _Search = new SearchDataOnOld(false);
                DataTable dt = _Search.GetDataTableFromMDB(strQuery);
                if (dt.Rows.Count > 0)
                {
                    BindImportedData(dt.Rows[0]);
                }
            }

        }

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
            {
                txtImportParty.Enabled = chkPick.Checked;
                txtImportParty.Clear();
            }
            else
            {
                txtImportParty.Enabled = false;
                txtImportParty.Clear();
            }
        }

        private void BindImportedData(DataRow dr)
        {
            txtName.Text = txtSSSName.Text = Convert.ToString(dr["Name"]);
            txtCategory.Text = Convert.ToString(dr["Category"]);
            // txtGroup.Text = Convert.ToString(dr["GroupName"]).ToUpper();
            // txtOtherGroup.Text = Convert.ToString(dr["GroupII"]);

            txtAddress.Text = Convert.ToString(dr["Address"]);
            txtState.Text = Convert.ToString(dr["State"]);
            txtPIN.Text = Convert.ToString(dr["PINCode"]);
            txtTransport.Text = Convert.ToString(dr["Transport"]);
            txtDistrictName.Text = Convert.ToString(dr["Station"]);
            txtBookingStation.Text = Convert.ToString(dr["BookingStation"]);
            // txtPartyType.Text = Convert.ToString(dr["TINNumber"]);
            txtNormalDhara.Text = Convert.ToString(dr["NormalDhara"]);
            txtSNetDhara.Text = Convert.ToString(dr["SNDhara"]);
            txtContactPer.Text = Convert.ToString(dr["ContactPerson"]);
            txtPhone.Text = Convert.ToString(dr["PhoneNo"]);
            txtMobile.Text = Convert.ToString(dr["MobileNo"]);
            txtPvtMarka.Text = Convert.ToString(dr["PvtMarka"]);
            //txtReference.Text = Convert.ToString(dr["Reference"]);
            txtEmailID.Text = Convert.ToString(dr["EmailID"]);
            txtDueDays.Text = Convert.ToString(dr["DueDays"]);
            //txtAmountLimit.Text = Convert.ToString(dr["AmountLimit"]);
            //txtExtendedAmt.Text = Convert.ToString(dr["ExtendedAmt"]);
            txtPerAddress.Text = Convert.ToString(dr["PermanentAddress"]);
            //pt.txtTransportI.Text = Convert.ToString(dr["SecondTransport"]);
            //pt.txtTransportII.Text = Convert.ToString(dr["ThirdTransport"]);
            //pt.txtTransportIII.Text = Convert.ToString(dr["FourthTransport"]);
            //txtRemark.Text = Convert.ToString(dr["Remark"]);
            //txtSchemedhara.Text = Convert.ToString(dr["CDDays"]);
            txtPostage.Text = Convert.ToString(dr["Postage"]);
            chkTransaction.Checked = Convert.ToBoolean(dr["TransactionLock"]);
            chkBlackList.Checked = Convert.ToBoolean(dr["BlackList"]);
            txtBlackList.Text = Convert.ToString(dr["BlackListReason"]);
        }


        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
        }

        private void dgrdBank_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                e.Cancel = true;
            }
            else if (btnEdit.Text == "&Update")
            {
                if (Convert.ToString(dgrdBank.CurrentRow.Cells["id"].Value) != "" && Convert.ToString(dgrdBank.CurrentCell.Value) != "")
                {
                    if (!MainPage.mymainObject.bChangeBankDetail)
                        e.Cancel = true;
                }
            }
        }

        private void dgrdBank_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    TextBox txtBox = (TextBox)e.Control;

                    if (dgrdBank.CurrentCell.ColumnIndex < 6 && dgrdBank.CurrentCell.RowIndex >= 0)
                    {
                        txtBox.CharacterCasing = CharacterCasing.Upper;
                        txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (dgrdBank.CurrentCell.RowIndex >= 0)
                {
                    if (dgrdBank.CurrentCell.ColumnIndex == 2)
                        dba.KeyHandlerPoint(sender, e, 0);
                    else if (dgrdBank.CurrentCell.ColumnIndex < 6)
                        dba.ValidateSpace(sender, e);
                }
            }
            catch { }
        }

        private void dgrdBank_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 7)
            {
                ChangeVerifyStatus();
            }
        }

        private void ChangeVerifyStatus()
        {
            if (MainPage.mymainObject.bBankDetailApprove)
            {
                bool _bStatus = Convert.ToBoolean(dgrdBank.CurrentRow.Cells["accountVerified"].Value);
                dgrdBank.CurrentRow.Cells["accountVerified"].Value = !_bStatus;
                if (!_bStatus)
                {
                    dgrdBank.CurrentRow.Cells["verifyButton"].Value = "Unverify";
                    dgrdBank.CurrentRow.DefaultCellStyle.BackColor = Color.LightGreen;
                    dgrdBank.CurrentRow.Cells["verifiedDate"].Value = "";
                }
                else
                {
                    dgrdBank.CurrentRow.Cells["verifyButton"].Value = "Verify";
                    dgrdBank.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void btnOtherDetails_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = true;
            txtCompanyRegNo.Focus();
        }

        private void txtOLDName_TextChanged(object sender, EventArgs e)
        {
            BindSearchSSSNameListData();
        }

        private void dgrdBank_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int CurrentRow = 0;
                    int IndexColmn = 0;
                    int Index;
                    if (e.KeyCode == Keys.Enter)
                    {
                        {
                            Index = dgrdBank.CurrentCell.RowIndex;
                            IndexColmn = dgrdBank.CurrentCell.ColumnIndex;
                            if (Index < dgrdBank.RowCount - 1)
                            {
                                CurrentRow = Index - 1;
                            }
                            else
                            {
                                CurrentRow = Index;
                            }
                            if (IndexColmn < dgrdBank.ColumnCount - 5)
                            {
                                IndexColmn += 1;
                                if (CurrentRow >= 0)
                                {
                                    dgrdBank.CurrentCell = dgrdBank.Rows[CurrentRow].Cells[IndexColmn];
                                }
                            }
                            else if (Index == dgrdBank.RowCount - 1)
                            {
                                string strBankName = Convert.ToString(dgrdBank.Rows[CurrentRow].Cells["bankName"].Value), strAccountNo = Convert.ToString(dgrdBank.Rows[CurrentRow].Cells["accountNo"].Value);

                                if (strBankName != "" && strAccountNo != "")
                                {
                                    dgrdBank.Rows.Add(1);
                                    dgrdBank.CurrentCell = dgrdBank.Rows[dgrdBank.RowCount - 1].Cells["bankName"];
                                }
                                else
                                {
                                    if (btnAdd.Text == "&Save")
                                        btnAdd.Focus();
                                    else
                                        btnEdit.Focus();
                                }
                            }

                        }
                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            dgrdBank.Rows.RemoveAt(dgrdBank.CurrentRow.Index);
                            if (dgrdBank.Rows.Count == 0)
                            {
                                dgrdBank.Rows.Add(1);
                                dgrdBank.CurrentCell = dgrdBank.Rows[0].Cells["bankName"];
                                dgrdBank.Enabled = true;
                            }
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            //else
                            //{
                            if (Convert.ToString(dgrdBank.CurrentRow.Cells["id"].Value) == "" || MainPage.mymainObject.bChangeBankDetail)
                            {
                                dgrdBank.Rows.RemoveAt(dgrdBank.CurrentRow.Index);
                            }
                            if (dgrdBank.Rows.Count == 0)
                            {
                                dgrdBank.Rows.Add(1);
                                dgrdBank.CurrentCell = dgrdBank.Rows[0].Cells["bankName"];
                                dgrdBank.Enabled = true;
                            }
                            //}
                        }
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdBank.CurrentCell.ColumnIndex;
                        if (colIndex == 1 || colIndex == 2)
                            dgrdBank.CurrentCell.Value = "";
                    }
                    else if (e.KeyValue == 96)
                        e.Handled = true;
                }
            }
            catch { }
        }

        private void txtSearchSSSName_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox _txt = (TextBox)sender;
                tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ShowAlways = true;
                tt.SetToolTip(_txt, "Please enter sss name");
            }
            catch { }
        }

        private void txtSearchParty_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox _txt = (TextBox)sender;
                tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ShowAlways = true;
                tt.SetToolTip(_txt, "Please enter account name");
            }
            catch { }
        }

        private void txtSearchSSSName_Leave(object sender, EventArgs e)
        {
            if (tt != null)
                tt.Dispose();
        }

        private void lnkCheck_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkCheck.Enabled = false;
            try
            {
                System.Diagnostics.Process.Start("https://services.gst.gov.in/services/searchtp");
            }
            catch { }
            lnkCheck.Enabled = true;
        }

        private void txtGSTNo_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (txtGSTNo.Text != "")
                {
                    bool chk = System.Text.RegularExpressions.Regex.IsMatch(txtGSTNo.Text, @"\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}");
                    if (!chk)
                    {
                        txtGSTNo.ForeColor = Color.Red;
                        MessageBox.Show("Sorry ! GST Number not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    else
                        txtGSTNo.ForeColor = Color.Black;

                }
                else
                    txtGSTNo.ForeColor = Color.Black;
            }
            catch
            {
            }
        }

        private void dgrdBrandName_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    e.Cancel = true;
                }
                else if (e.ColumnIndex == 1)
                {
                    SearchData objSearch = new SearchData("ITEMCATEGORYNAME", "SEARCH CATEGORY NAME", Keys.Space);
                    objSearch.ShowDialog();
                    dgrdBrandName.CurrentCell.Value = objSearch.strSelectedData;
                    e.Cancel = true;
                }
            }
            catch { }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtAreaCode.Text != "" && txtAccountNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("PARTYMASTER", txtAreaCode.Text, txtAccountNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnoClose_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                if (txtPartyBankAccountNo.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to print bank detail ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        PrintBankDetails(true);
                    }
                    else
                        PrintBankDetails(false);
                }
            }
            catch { }
            btnPrint.Enabled = true;
        }

        private void dgrdBrandName_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                }
            }
            catch
            {
            }
        }

        private void dgrdBrandName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int CurrentRow = 0;
                    int IndexColmn = 0;
                    int Index;
                    if (e.KeyCode == Keys.Enter)
                    {
                        Index = dgrdBrandName.CurrentCell.RowIndex;
                        IndexColmn = dgrdBrandName.CurrentCell.ColumnIndex;
                        if (Index < dgrdBrandName.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdBrandName.ColumnCount - 1)
                        {
                            IndexColmn += 1;
                            if (CurrentRow >= 0)
                            {
                                dgrdBrandName.CurrentCell = dgrdBrandName.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdBrandName.RowCount - 1)
                        {
                            string strBankName = Convert.ToString(dgrdBrandName.Rows[CurrentRow].Cells["brandName"].Value);

                            if (strBankName != "")
                            {
                                dgrdBrandName.Rows.Add(1);
                                dgrdBrandName.CurrentCell = dgrdBrandName.Rows[dgrdBrandName.RowCount - 1].Cells["brandName"];
                            }
                            else
                            {
                                if (btnAdd.Text == "&Save")
                                    btnAdd.Focus();
                                else
                                    btnEdit.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            dgrdBrandName.Rows.RemoveAt(dgrdBrandName.CurrentRow.Index);
                            if (dgrdBrandName.Rows.Count == 0)
                            {
                                dgrdBrandName.Rows.Add(1);
                                dgrdBrandName.CurrentCell = dgrdBrandName.Rows[0].Cells["brandName"];
                                dgrdBrandName.Enabled = true;
                            }
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            //else
                            //{
                            dgrdBrandName.Rows.RemoveAt(dgrdBrandName.CurrentRow.Index);
                            if (dgrdBrandName.Rows.Count == 0)
                            {
                                dgrdBrandName.Rows.Add(1);
                                dgrdBrandName.CurrentCell = dgrdBrandName.Rows[0].Cells["brandName"];
                                dgrdBrandName.Enabled = true;
                            }
                            //}
                        }
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdBrandName.CurrentCell.ColumnIndex;
                        if (colIndex == 1 || colIndex == 2)
                            dgrdBrandName.CurrentCell.Value = "";
                    }
                    else if (e.KeyValue == 96)
                        e.Handled = true;
                }
            }
            catch { }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtGSTNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtGSTNo.Text != "" && txtGSTNo.Text.Length == 15)
                    {
                        txtPANNumber.Text = txtGSTNo.Text.Substring(2, 10);
                    }
                }
            }
            catch { }
        }

        private void btnSendShippingDetail_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendShippingDetail.Enabled = false;
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strMessage = GetShippingDetailsSMS();
                    if (strMessage != "")
                    {
                        SendSMSPage objSMS = new SSS.SendSMSPage(txtMobile.Text, strMessage);
                        objSMS.ShowDialog();
                    }
                }
            }
            catch { }
            btnSendShippingDetail.Enabled = true;
        }

        private void btnPrintDetails_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrintDetails.Enabled = false;
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to print shipping detail ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        PrintShippingDetails(true);
                    }
                    else
                        PrintShippingDetails(false);
                }
            }
            catch { }
            btnPrintDetails.Enabled = true;
        }

        private string GetProfilePicPath(string strLocation, string strNo)
        {
            string strPath = "";
            if (strLocation != "")
            {
                string strFileName = txtAreaCode.Text + "'+CAST(@ID as varchar)+'_" + strNo;
                string strExtension = Path.GetExtension(strLocation);
                strFileName = strFileName + strExtension;

                if (MainPage.mymainObject.bMultiBranch && MainPage.strFTPPath != "")
                {                   
                    strPath = MainPage.strHttpPath + "/ProfilePic/" + strFileName;
                }
                else
                {
                    strPath = MainPage.strServerPath + "/ProfilePic/" + strFileName;
                }
            }
            return strPath;
        }

        private string GetOtherDetailsQuery()
        {           
            string strQuery = "", strDOB = "NULL", strDOA = "NULL";
            if (txtDOA.Text.Length == 10)
                strDOA = "'" + dba.ConvertDateInExactFormat(txtDOA.Text).ToString("MM/dd/yyyy") + "'";
            if (txtDOB.Text.Length == 10)
                strDOB = "'" + dba.ConvertDateInExactFormat(txtDOB.Text).ToString("MM/dd/yyyy") + "'";

            double dLimitAmt = dba.ConvertObjectToDouble(txtAmountLimit.Text) + dba.ConvertObjectToDouble(txtExtendedAmt.Text);

            if (btnAdd.Text == "&Save")
            {
                strQuery += " INSERT INTO [dbo].[SupplierOtherDetails] ([AreaCode],[AccountNo],[WaybillUserName],[WaybillPassword],[CompanyRegNo],[NameOfFirm],[OtherDetails],[NB_Manufacturing],[NB_SoleSellingAgent],[NB_Dealer],[NB_Agent],[NB_Assembler],[NB_Trader],[NC_Proprietary],[NC_Partnership],[NC_Private],[NC_Public],[Other],[ProfilePic1],[ProfilePic2],[ProfilePic3],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[DOB],[DOA],[SpouseName],[Description]) VALUES "
                         + " ('" + txtAreaCode.Text + "',@ID,'" + txtWhatsappNo.Text + "','','" + txtCompanyRegNo.Text + "','" + txtNameOfFirm.Text + "','" + txtReligion.Text + "','" + chkManf.Checked + "','" + chkSoleAgent.Checked + "','" + chkDealer.Checked + "','" + chkAgent.Checked + "','" + chkAssembler.Checked + "','" + chkTrader.Checked + "','" + chkProprietary.Checked + "','" + chkPartnership.Checked + "','" + chkPrivate.Checked + "','" + chkPublic.Checked + "','','" + GetProfilePicPath(picProfile1.ImageLocation,"1") + "','" + GetProfilePicPath(picProfile2.ImageLocation,"2") + "','" + GetProfilePicPath(picProfile3.ImageLocation,"3") + "','" + MainPage.strLoginName + "','',1,0," + strDOB + "," + strDOA + ",'" + txtSpouse.Text + "','') "
                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PARTYMASTER','" + txtAreaCode.Text + "',@ID,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dLimitAmt + ",'" + MainPage.strLoginName + "',0,0,'CREATION') ";
            }
            else
            {
                strQuery += " if not exists (Select AreaCode from SupplierOtherDetails Where [AreaCode]='" + txtAreaCode.Text + "' and [AccountNo]='" + txtAccountNo.Text + "') begin "
                         + " INSERT INTO [dbo].[SupplierOtherDetails] ([AreaCode],[AccountNo],[WaybillUserName],[WaybillPassword],[CompanyRegNo],[NameOfFirm],[OtherDetails],[NB_Manufacturing],[NB_SoleSellingAgent],[NB_Dealer],[NB_Agent],[NB_Assembler],[NB_Trader],[NC_Proprietary],[NC_Partnership],[NC_Private],[NC_Public],[Other],[ProfilePic1],[ProfilePic2],[ProfilePic3],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                         + " ('" + txtAreaCode.Text + "','" + txtAccountNo.Text + "','" + txtWhatsappNo.Text + "','','" + txtCompanyRegNo.Text + "','" + txtNameOfFirm.Text + "','" + txtReligion.Text + "','" + chkManf.Checked + "','" + chkSoleAgent.Checked + "','" + chkDealer.Checked + "','" + chkAgent.Checked + "','" + chkAssembler.Checked + "','" + chkTrader.Checked + "','" + chkProprietary.Checked + "','" + chkPartnership.Checked + "','" + chkPrivate.Checked + "','" + chkPublic.Checked + "','" + picProfile1.ImageLocation + "','" + picProfile2.ImageLocation + "','" + picProfile3.ImageLocation + "','','" + MainPage.strLoginName + "','',1,0) end else begin "
                         + " UPDATE [dbo].[SupplierOtherDetails] SET [WaybillUserName]='" + txtWhatsappNo.Text + "',[CompanyRegNo]='" + txtCompanyRegNo.Text + "',[NameOfFirm]='" + txtNameOfFirm.Text + "',[OtherDetails]='" + txtReligion.Text + "',[NB_Manufacturing]='" + chkManf.Checked + "',[NB_SoleSellingAgent]='" + chkSoleAgent.Checked + "',[NB_Dealer]='" + chkDealer.Checked + "',[NB_Agent]='" + chkAgent.Checked + "',[NB_Assembler]='" + chkAssembler.Checked + "',[NB_Trader]='" + chkTrader.Checked + "',[NC_Proprietary]='" + chkProprietary.Checked + "',[NC_Partnership]='" + chkPartnership.Checked + "',[NC_Private]='" + chkPrivate.Checked + "',[NC_Public]='" + chkPublic.Checked + "',[ProfilePic1]='" + picProfile1.ImageLocation + "',[ProfilePic2]='" + picProfile2.ImageLocation + "',[ProfilePic3]='" + picProfile3.ImageLocation + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdatedDate]=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),[DOB]=" + strDOB + ",[DOA]=" + strDOA + ",[SpouseName]='" + txtSpouse.Text + "' Where [AreaCode]='" + txtAreaCode.Text + "' and [AccountNo]='" + txtAccountNo.Text + "' end "
                         + " Delete from [dbo].[SupplierBankDetails] Where [AreaCode]='" + txtAreaCode.Text + "' and [AccountNo]='" + txtAccountNo.Text + "' "
                         + " Delete from[dbo].[SupplierBrandDetails] Where[AreaCode]='" + txtAreaCode.Text + "' and[AccountNo]='" + txtAccountNo.Text + "' "
                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PARTYMASTER','" + txtAreaCode.Text + "','" + txtAccountNo.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dLimitAmt + ",'" + MainPage.strLoginName + "',0,0,'UPDATION') ";

            }

            bool _bStatus = false;
            string strDate = "";
            foreach (DataGridViewRow row in dgrdBank.Rows)
            {
                _bStatus = Convert.ToBoolean(row.Cells["accountVerified"].Value);
                strDate = Convert.ToString(row.Cells["verifiedDate"].Value);
                if (_bStatus)
                {
                    if (strDate.Length == 10)
                        strDate = "'" + dba.ConvertDateInExactFormat(strDate).ToString("MM/dd/yyyy") + "'";
                    else
                        strDate = "DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))";
                }
                else
                    strDate = "NULL";

                strQuery += " if not exists (Select AreaCode from [SupplierBankDetails] Where [AreaCode]='" + txtAreaCode.Text + "' and [AccountNo]=@ID and [BankAccountNo]='" + row.Cells["accountNo"].Value + "') begin "
                         + " INSERT INTO [dbo].[SupplierBankDetails] ([AreaCode],[AccountNo],[BankName],[BranchName],[BankAccountNo],[BankIFSCCode],[BankAccountName],[VerifiedStatus],[VerifiedDate],[CreatedBy],[BeniID],[InsertStatus],[UpdateStatus]) VALUES "
                         + " ('" + txtAreaCode.Text + "',@ID,'" + row.Cells["bankName"].Value + "','" + row.Cells["branchName"].Value + "','" + row.Cells["accountNo"].Value + "','" + row.Cells["ifscCode"].Value + "','" + row.Cells["accountName"].Value + "','" + _bStatus + "'," + strDate + ",'" + MainPage.strLoginName + "','" + row.Cells["beniID"].Value + "',1,0) end ";
            }

            foreach (DataGridViewRow row in dgrdBrandName.Rows)
            {
                strQuery += " if not exists (Select AreaCode from [SupplierBrandDetails] Where [AreaCode]='" + txtAreaCode.Text + "' and [AccountNo]=@ID and [Range]='" + row.Cells["range"].Value + "' and [BrandName]='" + row.Cells["brandName"].Value + "' and [ProductType]='" + row.Cells["productType"].Value + "') begin "
                         + " INSERT INTO [dbo].[SupplierBrandDetails] ([AreaCode],[AccountNo],[BrandName],[ProductType],[Range],[HSNCode],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                         + " ('" + txtAreaCode.Text + "',@ID,'" + row.Cells["brandName"].Value + "','" + row.Cells["productType"].Value + "','" + row.Cells["range"].Value + "','','" + MainPage.strLoginName + "','',1,0) end ";
            }

            return strQuery;
        }

        private void BindSearchSSSNameListData()
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strGroup = txtSearchGroup.Text;
                    if (dtable != null)
                    {
                        lboxParty.Items.Clear();
                        if (strGroup == "")
                        {
                            if (txtSearchSSSName.Text == "")
                            {
                                foreach (DataRow dr in dtable.Rows)
                                {
                                    lboxParty.Items.Add(dr["FullName"]);
                                }
                                if (lboxParty.Items.Count > 0)
                                    lboxParty.SelectedIndex = 0;
                            }
                            else
                            {

                                DataRow[] filteredRows = dtable.Select(string.Format("{0} LIKE '%{1}%'", "Other", txtSearchSSSName.Text));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxParty.Items.Add(dr["FullName"]);
                                    }
                                    lboxParty.SelectedIndex = 0;
                                }
                            }
                        }
                        else
                        {
                            if (txtSearchSSSName.Text == "")
                            {
                                DataRow[] filteredRows = dtable.Select(string.Format("GroupName='" + strGroup + "'"));
                                foreach (DataRow dr in filteredRows)
                                {
                                    lboxParty.Items.Add(dr["FullName"]);
                                }
                                if (lboxParty.Items.Count > 0)
                                    lboxParty.SelectedIndex = 0;
                            }
                            else
                            {

                                DataRow[] filteredRows = dtable.Select(string.Format("GroupName='" + strGroup + "' and Other LIKE ('%" + txtSearchSSSName.Text + "%')"));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxParty.Items.Add(dr["FullName"]);
                                    }
                                    lboxParty.SelectedIndex = 0;
                                }
                            }
                        }
                        //lboxParty.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding Search List data with sss in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void PrintBankDetails(bool _bStatus)
        {
            DataTable _dt = CreateBankDataTable();
            if (_dt.Rows.Count > 0)
            {
                Reporting.PartyBankDetails objReport = new Reporting.PartyBankDetails();
                objReport.SetDataSource(_dt);
                objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                if (_bStatus)
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                        objReport.PrintToPrinter(1, false, 1, 1);
                else
                {
                    Reporting.ShowReport objShow = new Reporting.ShowReport("BANK DETAILS PREVIEW");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();
                }
            }
        }

        private void txtMainPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                        objSearch.ShowDialog();
                        txtMainPartyName.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCourierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("COURIERNAME", "SEARCH COURIER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCourierName.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSearchParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            // if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            dba.ValidateSpace(sender, e);
            // else
            //    e.Handled = true;
        }

        private void txtAccountentMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtAccountentMobileNo.Text.Length == 10 && !MainPage.strUserRole.Contains("ADMIN") && btnAdd.Text != "&Save")
            //    txtAccountentMobileNo.ReadOnly = true;
            //else
            //    txtAccountentMobileNo.ReadOnly = false;

            KeyHandler(e);
        }

        private DataTable CreateBankDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("HeaderName", typeof(String));
            table.Columns.Add("PartyName", typeof(String));
            table.Columns.Add("NickName", typeof(String));
            table.Columns.Add("BankName", typeof(String));
            table.Columns.Add("BranchName", typeof(String));
            table.Columns.Add("IFSCCode", typeof(String));
            table.Columns.Add("BankAccountNo", typeof(String));
            table.Columns.Add("BankAccountName", typeof(String));
            table.Columns.Add("PrintedBy", typeof(String));

            DataRow row = table.NewRow();
            row["HeaderName"] = "BANK DETAIL";
            row["PartyName"] = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
            row["NickName"] = txtSSSName.Text;
            row["BankName"] = "ICICI BANK";
            row["BranchName"] = "DELHI";
            row["IFSCCode"] = "ICIC0000106";
            row["BankAccountNo"] = txtPartyBankAccountNo.Text;
            row["BankAccountName"] = "SARAOGI SUPER SALES PVT LTD";
            row["PrintedBy"] = "PRINTED BY : " + MainPage.strLoginName + ", Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
            table.Rows.Add(row);

            return table;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPartyBankAccountNo.Text != "")
                {
                    string strMessage = "FOR A/C : " + txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text + "\nBANK: ICICI BANK,\nBRANCH: DELHI,\nIFSC CODE: ICIC0000106,\nBANK A/C NO.: " + txtPartyBankAccountNo.Text + "\nA/C NAME: SARAOGI SUPER SALES PVT LTD.";
                    SendSMSPage objSMS = new SSS.SendSMSPage(txtMobile.Text, strMessage);
                    objSMS.ShowDialog();
                }
            }
            catch { }
        }

        private void PrintShippingDetails(bool _bPrintStatus)
        {
            DataTable _dt = CreateShippingDataTable();
            if (_dt.Rows.Count > 0)
            {
                Reporting.PartyShippingDetails objReport = new Reporting.PartyShippingDetails();
                objReport.SetDataSource(_dt);
                objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                if (_bPrintStatus)
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                        objReport.PrintToPrinter(1, false, 1, 1);
                else
                {
                    Reporting.ShowReport objShow = new Reporting.ShowReport("SHIPPING DETAILS PREVIEW");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();
                }
            }
        }

        private void txtPartyType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (txtGroup.Text == "SUNDRY CREDITOR" || txtGroup.Text == "SUNDRY DEBTORS")
                        {
                            string _partyType = "CASHTYPESALE";
                            if (txtGroup.Text == "SUNDRY CREDITOR")
                                _partyType = "CASHTYPEPURCHASE";

                            SearchData objSearch = new SearchData(_partyType, "SEARCH PARTY TYPE", e.KeyCode);
                            objSearch.ShowDialog();
                            txtPartyType.Text = objSearch.strSelectedData;

                            // txtAmountLimit.ReadOnly = txtPartyType.Text == "CASH PARTY" ? true : false;
                            if (txtPartyType.Text == "CASH PARTY" && txtAmountLimit.Text == "")
                                txtAmountLimit.Text = "1";
                        }
                        else
                        {
                            string _partyType = "CASHTYPEOTHER";

                            SearchData objSearch = new SearchData(_partyType, "SEARCH PARTY TYPE", e.KeyCode);
                            objSearch.ShowDialog();
                            txtPartyType.Text = objSearch.strSelectedData;
                        }
                    }
                    else
                        e.Handled = true;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSpouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtDOB_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, false, false, false);
        }

        private void txtTransport_DoubleClick(object sender, EventArgs e)
        {
            if (txtTransport.Text != "")
                DataBaseAccess.OpenTransportMaster(txtTransport.Text);
        }

        private void txtSchemedhara_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtWhatsappNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (txtWhatsappNo.Text.Length == 10 && !MainPage.mymainObject.bChangeCustomerDetail && txtGroup.Text == "SUNDRY DEBTORS" && btnAdd.Text != "&Save")
                    txtWhatsappNo.ReadOnly = true;
                else
                    txtWhatsappNo.ReadOnly = false;

                KeyHandler(e);
            }
            catch { }
        }

        private void txtPremiumDhara_Leave(object sender, EventArgs e)
        {
            if (txtPremiumDhara.Text == "")
            {
                txtPremiumDhara.Text = "0";
            }
        }

        private void lblChequeStatus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblChequeStatus.Text != "" && txtAreaCode.Text != "" && txtAccountNo.Text != "")
                {
                    string _strName = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
                    ChequeDetailRegister objChequeDetailRegister = new ChequeDetailRegister(_strName, "SECURITY");
                    objChequeDetailRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objChequeDetailRegister.ShowDialog();
                }
            }
            catch { }
        }

        private void txtAmountLimit_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    txtAmountLimit.Text = dba.ConvertObjectToDouble(txtAmountLimit.Text).ToString("N0", MainPage.indianCurancy);
            }
            catch { }
        }

        private void txtExtendedAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    txtExtendedAmt.Text = dba.ConvertObjectToDouble(txtExtendedAmt.Text).ToString("N0", MainPage.indianCurancy);
            }
            catch { }
        }

        private void txtSearchGroup_TextChanged(object sender, EventArgs e)
        {

        }

        private void lnkShowMasterSummary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strSelectedName != "")
                {
                    ShowPartyMasterSummary objSummary = new ShowPartyMasterSummary(strSelectedName);
                    objSummary.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSummary.ShowInTaskbar = true;
                    objSummary.Show();
                }
            }
            catch { }
        }

        private void txtOrderAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    txtOrderAmt.Text = dba.ConvertObjectToDouble(txtOrderAmt.Text).ToString("N0", MainPage.indianCurancy);
            }
            catch { }
        }

        private void txtReligion_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("RELIGION", "SEARCH RELIGION", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtReligion.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch { }
        }

        private void SupplierMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            }
            catch { }
        }

        private void txtImportParty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtOpening_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void chkBlackList_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                CheckBox _txt = (CheckBox)sender;
                tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ShowAlways = true;
                tt.SetToolTip(_txt, "Black list");
            }
            catch { }
        }

        private void chkBlackList_MouseLeave(object sender, EventArgs e)
        {
            if (tt != null)
                tt.Dispose();
        }

        private void chkTransaction_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                CheckBox _txt = (CheckBox)sender;
                tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ShowAlways = true;
                tt.SetToolTip(_txt, "Transaction Lock");
            }
            catch { }
        }

        private void chkOrangeZone_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                CheckBox _txt = (CheckBox)sender;
                tt = new ToolTip();
                tt.IsBalloon = true;
                tt.ShowAlways = true;
                tt.SetToolTip(_txt, "Orange list");
            }
            catch { }
        }



        private void txtCardNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDistrictName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("DISTRICTNAME", txtState.Text, "SEARCH DISTRICT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtDistrictName.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }



        private void btnSendtoApp_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    btnSendtoApp.Enabled = false;
                    DialogResult result = MessageBox.Show("Are you sure you want to create user in mobile app ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strUserRole = "CUSTOMER", strUserType = "1";
                        if (txtGroup.Text == "SUNDRY CREDITOR")
                        {
                            strUserRole = "SUPPLIER";
                            strUserType = "2";
                        }
                        else if (txtGroup.Text != "SUNDRY DEBTORS")
                        {
                            strUserRole = "EMPLOYEE";
                            strUserType = "3";
                        }
                        bool _bStatus = AppAPI.AddNewUserinApp(txtName.Text, txtEmailID.Text, txtMobile.Text, strUserType, txtAreaCode.Text + txtAccountNo.Text, strUserRole);
                        string strName = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
                        string strResponse = AppAPI.AddNewUserinSSSAddaApp(strName, txtEmailID.Text, txtMobile.Text, strUserType, txtAreaCode.Text + txtAccountNo.Text, strUserRole, txtName.Text, txtDistrictName.Text, txtState.Text, txtGSTNo.Text, "");
                        if (strResponse != "")
                            MessageBox.Show(strResponse, "SSS Adda update response", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        if (_bStatus)
                            MessageBox.Show("Thank you ! User successfully created on mobile app.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnSendtoApp.Enabled = true;
        }



        private DataTable CreateShippingDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("HeaderName", typeof(String));
            table.Columns.Add("PartyName", typeof(String));
            table.Columns.Add("NickName", typeof(String));
            table.Columns.Add("BankName", typeof(String));
            table.Columns.Add("BranchName", typeof(String));
            table.Columns.Add("IFSCCode", typeof(String));
            table.Columns.Add("BankAccountNo", typeof(String));
            table.Columns.Add("BankAccountName", typeof(String));
            table.Columns.Add("PrintedBy", typeof(String));

            DataRow row = table.NewRow();

            row["HeaderName"] = "SHIPPING DETAIL";
            row["PartyName"] = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
            row["NickName"] = txtAddress.Text.Replace("\n", " ").Replace("\r", " ") + " " + txtDistrictName.Text + " " + txtState.Text + "-" + txtPIN.Text;
            row["BankName"] = txtState.Text;
            row["BranchName"] = txtGSTNo.Text;
            row["IFSCCode"] = txtTransport.Text;
            row["BankAccountName"] = txtBookingStation.Text;
            row["BankAccountNo"] = txtMobile.Text + " " + txtPhone.Text;
            row["PrintedBy"] = "PRINTED BY : " + MainPage.strLoginName + ", Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
            table.Rows.Add(row);

            return table;
        }



        private string GetShippingDetailsSMS()
        {
            string strMessage = "";
            strMessage += "SHIPPING DETAILS\n"
                       + "PARTY NAME : " + txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text + "\n"
                       + "ADDRESS : " + txtAddress.Text.Replace("\n", " ").Replace("\r", " ") + " " + txtDistrictName.Text + " " + txtState.Text + "-" + txtPIN.Text + "\n"
                       + "GST No : " + txtGSTNo.Text + "\n"
                       + "TRANSPORT : " + txtTransport.Text + "\n"
                       + "STATION : " + txtBookingStation.Text + "\n"
                       + "PHONE No : " + txtMobile.Text;
            return strMessage;
        }


        private void SendSMSToParty(string strAccountNo)
        {
            try
            {
                if (txtGroup.Text == "SUNDRY DEBTORS" && txtMobile.Text != "")
                {
                    string strCreditLimit = "", strBankDetails = "", strWhastappMessage = "";
                    double dLimit = dba.ConvertObjectToDouble(txtAmountLimit.Text);

                    if (dLimit > 1)
                        strCreditLimit = " with credit limit : " + dLimit.ToString("N2", MainPage.indianCurancy) + "/-.";

                    if (MainPage.strCompanyName.Contains("SARAOGI") && txtGroup.Text == "SUNDRY DEBTORS" && MainPage.strSoftwareType == "AGENT")
                        strBankDetails = "\n\nThe Bank detail for A/C : " + txtAreaCode.Text + strAccountNo + " " + txtName.Text + "\nBANK: ICICI BANK,\nBRANCH: DELHI,\nIFSC CODE: ICIC0000106,\nBANK A/C NO.: SASUSP" + dba.ConvertObjectToDouble(strAccountNo).ToString("000000") + "\nA/C NAME: SARAOGI SUPER SALES PVT LTD.";

                    string strMessage = "Thank you !! You are now registered with us, with the name of : " + txtAreaCode.Text + strAccountNo + " " + txtName.Text + strCreditLimit + ",\nWe are honor to welcome you in our family.";

                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        strMessage += "\nPlease download our mobile app :\nclick here for Android : https://play.google.com/store/apps/details?id=com.syber.ssspltd";

                    strWhastappMessage = "{\"default\": \"" + txtAreaCode.Text + strAccountNo + " " + txtName.Text + strCreditLimit + "\" },{\"default\": \"https://play.google.com/store/apps/details?id=com.syber.ssspltd" + strBankDetails + "\" }";

                    if (txtGroup.Text == "SUNDRY DEBTORS")
                        strMessage += strBankDetails;

                    SendSMS objSMS = new SendSMS();
                    objSMS.SendSingleSMS(strMessage, txtMobile.Text);

                    if (txtWhatsappNo.Text != "" && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        WhatsappClass.SendWhatsappWithIMIMobile(txtWhatsappNo.Text, "registration", strWhastappMessage, "", "");

                    // WhatsappClass.SendWhatsAppMessage(txtWhatsappNo.Text, strMessage, "", "NEW MASTER", "", "TEXT");
                }
            }
            catch
            {
            }
        }



        private void EnabaleControl()
        {
            txtDOA.ReadOnly = txtDOB.ReadOnly = txtSpouse.ReadOnly = txtName.ReadOnly = txtAddress.ReadOnly = txtPerAddress.ReadOnly = txtPIN.ReadOnly = txtGSTNo.ReadOnly = txtPANNumber.ReadOnly = txtOpening.ReadOnly = txtContactPer.ReadOnly = txtMobile.ReadOnly = txtAccountentMobileNo.ReadOnly = txtPhone.ReadOnly = txtSSSName.ReadOnly = txtPvtMarka.ReadOnly = txtEmailID.ReadOnly = txtAadharNumber.ReadOnly = txtRemark.ReadOnly = txtNormalDhara.ReadOnly = txtSNetDhara.ReadOnly = txtPremiumDhara.ReadOnly = txtDueDays.ReadOnly = txtSchemedhara.ReadOnly = txtPremiumDhara.ReadOnly = txtPostage.ReadOnly = txtDate.ReadOnly = txtSaleIncentive.ReadOnly = txtBlackList.ReadOnly = txtAccountentMobileNo.ReadOnly = false;
            txtAmountLimit.ReadOnly = txtExtendedAmt.ReadOnly =  false;//txtOrderAmt.ReadOnly =
            chkTransaction.Enabled = chkBlackList.Enabled = chkOrangeZone.Enabled = chkPostage.Enabled =chkMSMENo.Enabled= grpBox.Enabled= true;
            txtCategory.Enabled = txtPartyType.Enabled = txtOtherGroup.Enabled = txtReference.Enabled = txtPostage.Enabled = txtGroup.Enabled = true;
            rdoDebit.Enabled = rdoCredit.Enabled = true;
            txtMSMENo.ReadOnly = !chkMSMENo.Checked;

            if (btnEdit.Text == "&Update")
            {
                if (txtGroup.Text == "SUNDRY DEBTORS" || txtGroup.Text == "SUNDRY CREDITOR")
                {
                    txtPartyType.Enabled = txtReference.Enabled = txtGroup.Enabled = MainPage.mymainObject.bChangeCustomerDetail;
                    txtName.ReadOnly = txtSSSName.ReadOnly = txtOpening.ReadOnly = txtDueDays.ReadOnly = txtAddress.ReadOnly = txtPerAddress.ReadOnly = !MainPage.mymainObject.bChangeCustomerDetail;
                    txtGSTNo.ReadOnly = txtPANNumber.ReadOnly = (MainPage.mymainObject.bChangeCustomerDetail && MainPage.mymainObject.bGSTMasterEditDelete) ? false : true;
                    txtOtherGroup.Enabled = (MainPage.mymainObject.bChangeCustomerDetail && MainPage.mymainObject.bGSTMasterEditDelete) ? true : false;
                    txtNormalDhara.ReadOnly = txtSNetDhara.ReadOnly = !MainPage.mymainObject.bChangeSuplierDisc;
                    txtAmountLimit.ReadOnly = txtExtendedAmt.ReadOnly = !MainPage.mymainObject.bChangeCustomerLimit;
                    txtSchemedhara.ReadOnly = !MainPage.mymainObject.bSchemeMaster;
                    chkTransaction.Enabled = chkBlackList.Enabled = chkOrangeZone.Enabled = grpBox.Enabled = MainPage.mymainObject.bLockUnlockCustomer;
                    rdoDebit.Enabled = rdoCredit.Enabled = MainPage.mymainObject.bChangeCustomerDetail;

                    //txtOrderAmt.ReadOnly =
                }

                if (txtGroup.Text == "SUNDRY CREDITOR")
                {
                    if (MainPage.strUserRole == "MANAGER")
                        txtPartyType.Enabled = txtCategory.Enabled = grpBox.Enabled = true;
                }
            }
        }



        private void DisableControl()
        {
            txtDOA.ReadOnly = txtDOB.ReadOnly = txtSpouse.ReadOnly = txtName.ReadOnly = txtAddress.ReadOnly = txtPerAddress.ReadOnly = txtPIN.ReadOnly = txtGSTNo.ReadOnly = txtPANNumber.ReadOnly = txtOpening.ReadOnly = txtAmountLimit.ReadOnly = txtExtendedAmt.ReadOnly = txtContactPer.ReadOnly = txtMobile.ReadOnly = txtAccountentMobileNo.ReadOnly = txtPhone.ReadOnly = txtSSSName.ReadOnly = txtPvtMarka.ReadOnly = txtEmailID.ReadOnly = txtAadharNumber.ReadOnly = txtRemark.ReadOnly = txtNormalDhara.ReadOnly = txtSNetDhara.ReadOnly = txtPremiumDhara.ReadOnly = txtDueDays.ReadOnly = txtSchemedhara.ReadOnly = txtPostage.ReadOnly = txtDate.ReadOnly = txtSaleIncentive.ReadOnly = txtOrderAmt.ReadOnly = txtMobile.ReadOnly = txtAccountentMobileNo.ReadOnly = txtReference.ReadOnly = txtBlackList.ReadOnly =txtMSMENo.ReadOnly= true;
            chkMSMENo.Enabled= chkTransaction.Enabled = chkBlackList.Enabled = chkOrangeZone.Enabled = rdoDebit.Enabled = rdoCredit.Enabled =grpBox.Enabled= false;
        }

        private void txtReference_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ACTIVEREFERENCENAME", "SEARCH REFERENCE NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtReference.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        #region Profile Pics

        //bool bPic1Ch = false, bPic2Ch = false, bPic3Ch = false;
        //private void SaveProfilePic()
        //{
        //    try
        //    {
        //        string AppPath = Application.StartupPath;
        //        if(Directory.Exists(AppPath + "\\ProfilePic\\") == false)
        //        {
        //            Directory.CreateDirectory(AppPath + "\\ProfilePic\\");
        //        }
        //        string FileName = txtAreaCode.Text + txtAccountNo.Text;
        //        if (bPic1Ch)
        //        {
        //            string strExtension1 = Path.GetExtension(picProfile1.ImageLocation), strNewImage1 = FileName + "_1" + strExtension1;
        //            picProfile1.Image.Save(AppPath + "\\ProfilePic\\" + strNewImage1);
        //            picProfile1.ImageLocation = AppPath + "\\ProfilePic\\" + strNewImage1;
        //        }
        //        if (bPic2Ch)
        //        {
        //            string strExtension2 = Path.GetExtension(picProfile2.ImageLocation), strNewImage2 = FileName + "_2" + strExtension2;
        //            picProfile2.Image.Save(AppPath + "\\ProfilePic\\" + strNewImage2);
        //            picProfile2.ImageLocation = AppPath + "\\ProfilePic\\" + strNewImage2;
        //        }
        //        if (bPic3Ch)
        //        {
        //            string strExtension3 = Path.GetExtension(picProfile3.ImageLocation), strNewImage3 = FileName + "_3" + strExtension3;
        //            picProfile3.Image.Save(AppPath + "\\ProfilePic\\" + strNewImage3);
        //            picProfile3.ImageLocation = AppPath + "\\ProfilePic\\" + strNewImage3;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Unable to open file : " + ex.Message);
        //    }
        //}

        private void ShowSavedProfiles(DataRow Row)
        {
            string strPathPic1 = "", strPathPic2 = "", strPathPic3 = "";
           
            strPathPic1 = Convert.ToString(Row["ProfilePic1"]);
            strPathPic2 = Convert.ToString(Row["ProfilePic2"]);
            strPathPic3 = Convert.ToString(Row["ProfilePic3"]);

            // Pic 1 Try
            try
            {
                if (strPathPic1.Contains("http") || File.Exists(strPathPic1))
                {
                    picProfile1.ImageLocation = strPathPic1;
                    btnRemovePic1.Visible = true;
                }
                else
                    picProfile1.Image = (Image)ObjDummyProfile; 
            }
            catch
            {
                picProfile1.Image = (Image)ObjDummyProfile; 
            }

            // Pic 2 Try
            try
            {
                if (strPathPic2.Contains("http") || File.Exists(strPathPic2))
                {
                    picProfile2.ImageLocation = strPathPic2;
                    btnRemovePic2.Visible = true;
                }
                else
                    picProfile2.Image = (Image)ObjDummyProfile; 
            }
            catch
            {
                picProfile2.Image = (Image)ObjDummyProfile; 
            }

            // Pic 3 Try
            try
            {
                if (strPathPic3.Contains("http") || File.Exists(strPathPic3))
                {
                    picProfile3.ImageLocation = strPathPic3;
                    btnRemovePic3.Visible = true;
                }
                else
                    picProfile3.Image = (Image)ObjDummyProfile; 
            }
            catch
            {
                picProfile3.Image = (Image)ObjDummyProfile; 
            }
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            OpenFileDialog objDialog = new OpenFileDialog();
            try
            {
                objDialog.Filter = "Image File|*.jpg;*.png;*.jpeg;*.gif;*.bmp";
                objDialog.ShowDialog();
                if (objDialog.FileName != "")
                {
                    //bPic1Ch = true;
                    picProfile1.ImageLocation = objDialog.FileName;
                    btnRemovePic1.Visible = true;
                }
                else
                {
                    picProfile1.ImageLocation = "";
                    picProfile1.Image = (Image)ObjDummyProfile;
                }
            }
            catch
            {
            }
            finally
            {
                objDialog.Dispose();
            }
        }
      
        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            OpenFileDialog objDialog = new OpenFileDialog();
            try
            {
                objDialog.Filter = "Image File|*.jpg;*.png;*.jpeg;*.gif;*.bmp";
                objDialog.ShowDialog();
                if (objDialog.FileName != "")
                {
                 //   bPic2Ch = true;
                    picProfile2.ImageLocation = objDialog.FileName;
                    btnRemovePic2.Visible = true;
                }
                else
                {
                    picProfile2.ImageLocation = "";
                    picProfile2.Image = (Image)ObjDummyProfile;
                }
            }
            catch
            {
            }
            finally
            {
                objDialog.Dispose();
            }
        }

        private void btnBrowse3_Click(object sender, EventArgs e)
        {
            OpenFileDialog objDialog = new OpenFileDialog();
            try
            {
                objDialog.Filter = "Image File|*.jpg;*.png;*.jpeg;*.gif;*.bmp";
                objDialog.ShowDialog();
                if (objDialog.FileName != "")
                {
                   // bPic3Ch = true;
                    picProfile3.ImageLocation = objDialog.FileName;
                    btnRemovePic3.Visible = true;
                }
                else
                {
                    picProfile3.ImageLocation = "";
                    picProfile3.Image = (Image)ObjDummyProfile;
                }
                   
            }
            catch
            {
            }
            finally
            {
                objDialog.Dispose();
            }
        }

        private void btnUpload1_Click(object sender, EventArgs e)
        {
            if(btnAdd.Text!="&Save")
            SaveImage(btnUpload1, picProfile1);
        }

        private void btnUpload2_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
                SaveImage(btnUpload2, picProfile2);
        }

        private void btnUpload3_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
                SaveImage(btnUpload3, picProfile3);
        }

        private void SaveImage_Local(PictureBox picBox,string strAccountNo)
        {
            try
            {
                if (picBox.ImageLocation == "")
                {
                    string PicNumber = picBox.Name.Substring(picBox.Name.Length - 1, 1);
                    string strImagePath = picBox.ImageLocation;
                    if (strImagePath != "")
                    {
                        string strFileName = txtAreaCode.Text + strAccountNo + "_" + PicNumber;
                        string strExtension = Path.GetExtension(strImagePath);
                        strFileName = strFileName + strExtension;

                        string strFilePath = dba.SaveProfilePic_Local(strImagePath, strFileName);
                        if (strFilePath != "")
                        {
                            picBox.ImageLocation = strFilePath;
                            if (PicNumber == "1")
                                btnRemovePic1.Visible = true;
                            if (PicNumber == "2")
                                btnRemovePic2.Visible = true;
                            if (PicNumber == "3")
                                btnRemovePic3.Visible = true;
                        }
                        else
                        {
                            picBox.ImageLocation = "";
                            picBox.Image = (Image)ObjDummyProfile;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveAllImages(string strAccountNo)
        {
            try
            {
                if(MainPage.mymainObject.bMultiBranch && MainPage.strFTPPath!="")
                {
                    SaveImage_Bulk(picProfile1, strAccountNo);
                    SaveImage_Bulk(picProfile2, strAccountNo);
                    SaveImage_Bulk(picProfile3, strAccountNo);
                }
                else
                {
                    SaveImage_Local(picProfile1, strAccountNo);
                    SaveImage_Local(picProfile2, strAccountNo);
                    SaveImage_Local(picProfile3, strAccountNo);
                }
            }
            catch { }
        }

        private void txtAreaCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (btnAdd.Text == "&Save")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtAreaCode.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SaveImage_Bulk(PictureBox picBox, string strAccountNo)
        {
            try
            {
                string PicNumber = picBox.Name.Substring(picBox.Name.Length - 1, 1);

                string strImagePath = picBox.ImageLocation;
                if (strImagePath != "" && !strImagePath.Contains("http"))
                {
                    string strFileName = txtAreaCode.Text + strAccountNo + "_" + PicNumber;
                    string strExtension = Path.GetExtension(strImagePath);
                    strFileName = strFileName + strExtension;

                    bool isUploaded = dba.UploadProfilePic(strImagePath, strFileName);
                    if (isUploaded)
                    {
                        picBox.ImageLocation =MainPage.strHttpPath+ "/ProfilePic/" + strFileName;
                        if (PicNumber == "1")
                            btnRemovePic1.Visible = true;
                        if (PicNumber == "2")
                            btnRemovePic2.Visible = true;
                        if (PicNumber == "3")
                            btnRemovePic3.Visible = true;
                    }
                    else
                    {
                        picBox.ImageLocation = "";
                        picBox.Image = (Image)ObjDummyProfile;
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveImage(Button btn, PictureBox picBox)
        {
            try
            {
                btn.Text = "Please wait..";
                btn.Enabled = false;

                string PicNumber = picBox.Name.Substring(picBox.Name.Length-1, 1);

                string strImagePath = picBox.ImageLocation;
                if (strImagePath != "")
                {
                    string strFileName = txtAreaCode.Text + txtAccountNo.Text + "_" + PicNumber;
                    string strExtension = Path.GetExtension(strImagePath);
                    strFileName = strFileName + strExtension;

                    DialogResult result = MessageBox.Show("Are you sure want to upload image on net ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        bool isUploaded = dba.UploadProfilePic(strImagePath, strFileName);
                        if (isUploaded)
                        {
                            picBox.ImageLocation = MainPage.strHttpPath+"/ProfilePic/" + strFileName;
                            if (PicNumber == "1")
                                btnRemovePic1.Visible = true;
                            if (PicNumber == "2")
                                btnRemovePic2.Visible = true;
                            if (PicNumber == "3")
                                btnRemovePic3.Visible = true;

                            MessageBox.Show("Thank you ! Profile pic uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            picBox.ImageLocation = "";
                            picBox.Image = (Image)ObjDummyProfile;
                            MessageBox.Show("Sorry ! Somthing went wrong, Please try later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch
            {
            }
            btn.Text = "Upload Image";
            btn.Enabled = true;
        }

        private bool RemoveImage(PictureBox picBox)
        {
            try
            {
                string PicNumber = picBox.Name.Substring(picBox.Name.Length - 1, 1);

                string strImagePath = picBox.ImageLocation;
                if (strImagePath != "")
                {
                    string strFileName = txtAreaCode.Text + txtAccountNo.Text + "_" + PicNumber;
                    string strExtension = Path.GetExtension(strImagePath);
                    strFileName = strFileName + strExtension;

                    DialogResult result = MessageBox.Show("Are you sure want to remove image from net ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        bool isRemoved = dba.DeleteProfilePic(strFileName);
                        if (isRemoved)
                        {
                            UpdateRecord();
                            picBox.ImageLocation = "";
                            picBox.Image = (Image)ObjDummyProfile;
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Somthing went wrong, Please try later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch
            {
            }
            return false;
        }

        private void chkMSMENo_CheckedChanged(object sender, EventArgs e)
        {
            if(btnAdd.Text=="&Save" || btnEdit.Text=="&Update")
            {
                txtMSMENo.ReadOnly = !chkMSMENo.Checked;
            }
        }

        private void txtBlackList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void txtBlackList_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtBlackList.Text = txtBlackList.Text.Replace("\n", " ").Replace("\r", " ").Trim();
            }
        }

        private bool RemoveImage_local(PictureBox picBox)
        {
            try
            {
                string strImagePath = picBox.ImageLocation;
                if (strImagePath != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to remove image?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(strImagePath))
                        {
                            File.Delete(strImagePath);
                            UpdateRecord();
                        }
                    }
                    else
                        return false;
                }
            }
            catch
            {
            }
            return false;
        }

        private void btnRemovePic1_Click(object sender, EventArgs e)
        {
            try
            {
                if (picProfile1.ImageLocation.Contains("http"))
                {
                    if (RemoveImage(picProfile1))
                    {
                        picProfile1.Image = (Image)ObjDummyProfile;
                        picProfile1.ImageLocation = "";
                        btnRemovePic1.Visible = false;
                    }
                }
                else
                {
                    if (RemoveImage_local(picProfile1))
                    {
                        picProfile1.Image = (Image)ObjDummyProfile;
                        picProfile1.ImageLocation = "";
                        btnRemovePic1.Visible = false;
                    }
                }
            }
            catch { }
        }

        private void btnRemovePic2_Click(object sender, EventArgs e)
        {
            try
            {
                if (picProfile2.ImageLocation.Contains("http"))
                {
                    if (RemoveImage(picProfile2))
                    {
                        picProfile2.Image = (Image)ObjDummyProfile;
                        picProfile2.ImageLocation = "";
                        btnRemovePic2.Visible = false;
                    }
                }
                else if (RemoveImage_local(picProfile1))
                {                 
                    picProfile2.Image = (Image)ObjDummyProfile;
                    picProfile2.ImageLocation = "";
                    btnRemovePic2.Visible = false;
                }
            }
            catch { }
        }

        private void btnRemovePic3_Click(object sender, EventArgs e)
        {
            try
            {
                if (picProfile3.ImageLocation.Contains("http"))
                {
                    if (RemoveImage(picProfile3))
                    {
                        picProfile3.Image = (Image)ObjDummyProfile;
                        picProfile3.ImageLocation = "";
                        btnRemovePic3.Visible = false;
                    }
                }
                else if (RemoveImage_local(picProfile1))
                {
                    picProfile3.Image = (Image)ObjDummyProfile;
                    picProfile3.ImageLocation = "";
                    btnRemovePic3.Visible = false;
                }
            }
            catch { }
        }

        private void picProfile1_Click(object sender, EventArgs e)
        {
            ZoomOut(picProfile3, grpPic3);
            ZoomOut(picProfile2, grpPic2);
            if (picProfile1.ImageLocation != "")
                ZoomIn(picProfile1, grpPic1);

        }

        private void picProfile2_Click(object sender, EventArgs e)
        {
            ZoomOut(picProfile1, grpPic1);
            ZoomOut(picProfile3, grpPic3);
            if (picProfile2.ImageLocation != "")
                ZoomIn(picProfile2, grpPic2);
        }

        private void picProfile3_Click(object sender, EventArgs e)
        {
            ZoomOut(picProfile1, grpPic1);
            ZoomOut(picProfile2, grpPic2);
            if (picProfile3.ImageLocation != "")
                ZoomIn(picProfile3, grpPic3);
        }

        private void ZoomIn(object sender, GroupBox panelParent)
        {
            PictureBox picBox = sender as PictureBox;
            if (picBox.Width == 360)
                ZoomOut(sender, panelParent);
            else
                try
                {
                    picBox.Width = 360;
                    picBox.Height = 400;
                    picBox.Parent = pnlDetails;
                    picBox.BringToFront();
                    if (panelParent.Left > 850)
                        picBox.Left = panelParent.Left - 200;
                    else
                        picBox.Left = panelParent.Left-50;
                    picBox.Top = panelParent.Top + 10;
                    picBox.BorderStyle = BorderStyle.FixedSingle;
                }
                catch
                {
                }
        }

        private void ZoomOut(object sender, GroupBox panelParent)
        {
            PictureBox picBox = sender as PictureBox;

            picBox.Width = 120;
            picBox.Height = 140;
            picBox.Parent = panelParent;
            picBox.Left = 7;
            picBox.Top = 17;
            picBox.BorderStyle = BorderStyle.None;
        }

        #endregion
    }
}
