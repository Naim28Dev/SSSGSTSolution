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
    public partial class NewSubParty : Form
    {
        DataBaseAccess dba;     
        public string strNewAddedSubParty = "",_strSubParty="", _strSalesParty="";
        bool _newStatus = false;
        int id = 0;
        DataTable dtSubParty;
        ToolTip tt;

        public NewSubParty(int chk)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetAreaCode();
            BindSearchListData();
            id = chk;
        }

        public NewSubParty(bool _nStatus, string strParty,string strSParty)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _strSalesParty = strParty;
            _strSubParty = strSParty;
            _nStatus = _newStatus;
            GetAreaCode();
            txtSubParty.Focus();
        }

        public NewSubParty(string strParty,string strSubParty)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _strSalesParty = strParty;
            GetAreaCode();
            BindSearchListData();
            lboxSubParty.SelectedItem = strSubParty;         
            txtSubParty.Focus();
        }

        private void NewSubParty_Load(object sender, EventArgs e)
        {
            try
            {
                bool _bStatus=EditOption();
                if (_bStatus)
                {
                    if (_newStatus)
                    {
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        txtSearchSalesParty.TabStop = txtSearchSubParty.TabStop = lboxSubParty.TabStop = false;
                        btnAdd.PerformClick();
                        txtSalesParty.Text = _strSalesParty;
                        txtSubParty.Text = _strSubParty;
                        txtSalesParty.Enabled = false;
                        txtSubParty.Focus();
                    }
                    if (MainPage.strSoftwareType == "AGENT" && MainPage.strCompanyName.Contains("SARAOGI"))
                        picOrange.Visible = chkOrangeZone.Visible = true;
                    else
                        picOrange.Visible = chkOrangeZone.Visible = false;
                }
                else
                    this.Close();

            }
            catch { }
        }

        private bool GetAreaCode()
        {
            DataSet ds = dba.GetSubPartyName();
            if (ds.Tables.Count > 1)
            {
                dtSubParty = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                if (dt.Rows.Count > 0)
                    txtAreaCode.Text = Convert.ToString(dt.Rows[0][0]);

            }
            if (txtAreaCode.Text == "")
            {
                MessageBox.Show("Sorry ! Area Code can't be blank, Please fill area code in company master after that you can add party name", "Worning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = false;
                return false;
            }
            return true;
        }

        private void BindListData()
        {
            try
            {
                lboxSubParty.Items.Clear();
                if (txtSubParty.Text == "")
                {
                    foreach (DataRow dr in dtSubParty.Rows)
                    {
                        lboxSubParty.Items.Add(dr["FullName"]);
                    }
                }
                else
                {
                    DataRow[] filteredRows = dtSubParty.Select(string.Format("{0} LIKE '%{1}%'", "Name", txtSubParty.Text));
                    if (filteredRows.Length > 0)
                    {
                        foreach (DataRow dr in filteredRows)
                        {
                            lboxSubParty.Items.Add(dr["FullName"]);
                        }
                    }

                }
                if (lboxSubParty.Items.Count > 0)
                    lboxSubParty.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding List Data in New Sub Party ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void EnabaleControl()
        {
            txtNickName.ReadOnly= txtSubParty.ReadOnly = txtAddress.ReadOnly = txtPinCode.ReadOnly = txtGSTNo.ReadOnly = txtPANNumber.ReadOnly = txtMobileNo.ReadOnly = txtPhoneNo.ReadOnly = txtMarka.ReadOnly =  txtRemark.ReadOnly = txtPostage.ReadOnly = txtDate.ReadOnly = txtBlackList.ReadOnly= false;
               

            if (btnAdd.Text == "&Save")
            {
                txtAmtLimit.ReadOnly = txtAddress.ReadOnly = txtGSTNo.ReadOnly = false;
                chkTransaction.Enabled = chkBlackList.Enabled = chkOrangeZone.Enabled = true;
            }
            else
            {
                txtGSTNo.ReadOnly = (MainPage.mymainObject.bChangeCustomerDetail && MainPage.mymainObject.bGSTMasterEditDelete) ? false : true;
                txtSubParty.ReadOnly = txtAddress.ReadOnly = txtMobileNo.ReadOnly = !MainPage.mymainObject.bChangeCustomerDetail;
                txtAmtLimit.ReadOnly = !MainPage.mymainObject.bChangeCustomerLimit;
                chkTransaction.Enabled = chkBlackList.Enabled = chkOrangeZone.Enabled = MainPage.mymainObject.bLockUnlockCustomer;
            }
        }

        private void DisableControl()
        {
            txtNickName.ReadOnly = txtSubParty.ReadOnly = txtAddress.ReadOnly = txtPinCode.ReadOnly = txtGSTNo.ReadOnly = txtPANNumber.ReadOnly = txtAmtLimit.ReadOnly = txtMobileNo.ReadOnly = txtPhoneNo.ReadOnly = txtMarka.ReadOnly = txtRemark.ReadOnly = txtPostage.ReadOnly = txtDate.ReadOnly = txtBlackList.ReadOnly = true;
            chkTransaction.Enabled = chkBlackList.Enabled = txtBlackList.Enabled = chkOrangeZone.Enabled = false;
        }

        private void SaveData()
        {
            try
            {
                string[] record = new string[26];

                record[0] = txtSubParty.Text;
                record[1] = txtAddress.Text;
                record[2]=txtState.Text;
                record[3] = txtTransport.Text;
                record[4] = txtDistrict.Text;
                record[5] = txtBookingStation.Text;
                record[6] = txtMobileNo.Text;              
                record[8] = txtMarka.Text;
                record[9] = txtPostage.Text;
                record[10] = txtGroup.Text;
                record[11] = txtAreaCode.Text;
                record[12] = txtAccountNo.Text;
                record[13] = txtGSTNo.Text;
                record[14] = txtPANNumber.Text;
                record[15] = txtPinCode.Text;
                record[16] = txtPhoneNo.Text;
                record[17] = txtRemark.Text;
                
                if (txtAmtLimit.Text != "")
                    record[18] = txtAmtLimit.Text;
                else
                    record[18] = "0";
                record[19] = dba.ConvertDateInExactFormat(txtDate.Text).ToString("MM/dd/yyyy h:mm:ss tt");               
                record[20] = chkBlackList.Checked.ToString();
                if (chkBlackList.Checked)
                    record[21] = txtBlackList.Text;
                record[22] = chkTransaction.Checked.ToString();
                record[23] = txtNickName.Text;
                record[24] = chkOrangeZone.Checked.ToString();

                string[] strSalesParty = txtSalesParty.Text.Split(' ');
                if (strSalesParty.Length > 1)
                {
                    record[7] = strSalesParty[0];
                    int count = 0;
                    if (MainPage.strOnlineDataBaseName != "")
                    {
                        string strResult = dba.SaveSubPartyNameInOnline(record, MainPage.strOnlineDataBaseName);
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
                        count = dba.SaveSubSalesParty(record);

                    if (count > 0)
                    {
                        MessageBox.Show("Thank You ! Record saved successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        if (_newStatus || id == 0)
                        {
                            strNewAddedSubParty = txtSubParty.Text;
                            this.Close();
                            return;
                        }
                        else
                        {
                            ClearAllText();
                            BindListData();
                        }
                    }
                    else
                    {
                        if (count != -2)
                            MessageBox.Show("Sorry ! Record not  Saved ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Sundry Debtors name is not valid please try again ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Saving Data in New Sub Party ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void UpdateData()
        {
            try
            {
                string[] record = new string[27];

                record[0] = txtSubParty.Text;
                record[1] = txtAddress.Text;
                record[2] = txtState.Text;
                record[3] = txtTransport.Text;
                record[4] = txtDistrict.Text;
                record[5] = txtBookingStation.Text;
                record[6] = txtMobileNo.Text;
                record[8] = txtMarka.Text;
                record[9] = txtPostage.Text;
                record[10] = txtGroup.Text;
                record[11] = txtAreaCode.Text;
                record[12] = txtAccountNo.Text;
                record[13] = Convert.ToString(lboxSubParty.SelectedItem);
                record[14] = txtGSTNo.Text;
                record[15] = txtPANNumber.Text;
                record[16] = txtPinCode.Text;

                record[17] = txtPhoneNo.Text;
                record[18] = txtRemark.Text;

                if (txtAmtLimit.Text != "")
                    record[19] = txtAmtLimit.Text;
                else
                    record[19] = "0";
                record[20] = dba.ConvertDateInExactFormat(txtDate.Text).ToString("MM/dd/yyyy h:mm:ss tt");              
                record[21] = chkBlackList.Checked.ToString();
                if (chkBlackList.Checked)
                    record[22] = txtBlackList.Text;
                record[23] = chkTransaction.Checked.ToString();
                record[24] = txtNickName.Text;
                record[25] = chkOrangeZone.Checked.ToString();

                string[] strSalesParty = txtSalesParty.Text.Split(' ');
                if (strSalesParty.Length > 1)
                {
                    record[7] = strSalesParty[0];
                    int count = dba.UpdateSubSalesParty(record);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank You ! Record Successfully Updated ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        _strSubParty = txtSubParty.Text;
                        btnEdit.Text = "&Edit";
                        int _index = lboxSubParty.SelectedIndex;
                        lboxSubParty.Enabled=true;
                        BindSearchListData();
                        if (lboxSubParty.Items.Count > 0)
                            lboxSubParty.SelectedIndex = _index;
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Record not  Update ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Sundry Debtors name is not valid please try again ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Updating Data in New Sub Party ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtSubParty_TextChanged(object sender, EventArgs e)
        {
            lboxSubParty.Visible = true;
            if (btnAdd.Text == "&Save")
            {
                BindListData();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void GetMaxAccountNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(CAST(AccountNo as int)),0)+1) from SupplierMaster ");
            txtAccountNo.Text = Convert.ToString(objValue);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
              try
            {
                if (btnAdd.Text == "&Add")
                {                   
                        btnAdd.Text = "&Save";
                        btnEdit.Text = "&Edit";
                        txtGroup.Enabled = true;
                        ClearAllText();
                        EnabaleControl();
                        GetAreaCode();
                        GetMaxAccountNo();
                        txtAmtLimit.ReadOnly = false;
                        txtSalesParty.Focus();                   
                }
                else
                {
                    btnAdd.Enabled = false;
                    if (ValidateControl())
                    {
                        if (CheckAvailability())
                        {
                            DialogResult dr = MessageBox.Show(" Are you sure want to save data .....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {

                                if (btnAdd.Text == "&Save")
                                {
                                    SaveData();
                                }
                                    //else if (btnAdd.Text == "Up&date")
                                    //{
                                    //    UpdateData();
                                    //}
                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("Sorry ! Require Fields can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private bool ValidateControl()
        {
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors can't be blank !!", "Sundry Debtors required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (txtSubParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sub party can't be blank !!", "Sub party required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }
            if (txtState.Text == "")
            {
                MessageBox.Show("Sorry ! State name can't be blank !!", "State name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtState.Focus();
                return false;
            }
            if (txtPinCode.Text == "")
            {
                MessageBox.Show("Sorry ! Pin Code can't be blank !!", "Pin Code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPinCode.Focus();
                return false;
            }

            return true;
        }

        private void NewSubParty_KeyDown(object sender, KeyEventArgs e)
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

        private void lboxSubParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text=="&Edit")
            {
                string strParty = Convert.ToString(lboxSubParty.SelectedItem);
                if (strParty != "")
                    BindDataWithControl(strParty);
            }
        }

        private void BindDataWithControl(string strFullName)
        {
            try
            {
                DisableControl();

                DataTable dt = dba.GetDataTable("Select *,Convert(varchar,Date,103) SDate,(Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')=SM.HasteSale)) as SalesPartyName from SupplierMaster SM Where GroupName='SUB PARTY' and (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + strFullName + "'");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtAreaCode.Text = Convert.ToString(row["AreaCode"]);
                    txtAccountNo.Text = Convert.ToString(row["AccountNo"]);
                    txtSalesParty.Text = Convert.ToString(row["SalesPartyName"]);
                    txtSubParty.Text = _strSubParty= Convert.ToString(row["Name"]);
                    txtTransport.Text = Convert.ToString(row["Transport"]);
                    txtDistrict.Text = Convert.ToString(row["Station"]);
                    txtBookingStation.Text = Convert.ToString(row["BookingStation"]);
                    txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                    txtAddress.Text = Convert.ToString(row["Address"]);
                    txtMarka.Text = Convert.ToString(row["PvtMarka"]);
                    txtPostage.Text = Convert.ToString(row["Postage"]);
                    txtState.Text = Convert.ToString(row["State"]);
                    txtGroup.Text = Convert.ToString(row["GroupII"]);
                    txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                    txtPANNumber.Text = Convert.ToString(row["PANNumber"]);
                    txtPinCode.Text = Convert.ToString(row["PinCode"]);
                    txtAmtLimit.Text = Convert.ToString(row["AmountLimit"]);
                    txtDate.Text = Convert.ToString(row["SDate"]);
                    txtPhoneNo.Text = Convert.ToString(row["PhoneNo"]);
                    txtNickName.Text = Convert.ToString(row["Other"]);
                    chkTransaction.Checked = Convert.ToBoolean(row["TransactionLock"]);
                    chkBlackList.Checked = Convert.ToBoolean(row["BlackList"]);
                    txtBlackList.Text = Convert.ToString(row["BlackListReason"]);

                    string strOrange = Convert.ToString(row["Other1"]);
                    if (strOrange.ToUpper().Contains("TRUE"))
                        chkOrangeZone.Checked = true;
                    else
                        chkOrangeZone.Checked = false;

                    lblCreatedBy.Text = "";
                    string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                    if (MainPage.strUserRole != "ADMIN" && MainPage.strUserRole != "SUPERADMIN" && MainPage.strLoginName != "RAJESH")
                        txtAmtLimit.ReadOnly = true;
                    else
                        txtAmtLimit.ReadOnly = false;

                }
            }
            catch
            {
            }
        }

        private void txtSubParty_Leave(object sender, EventArgs e)
        {
            CheckAvailability();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("Are you sure want to delete sub party....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int count = dba.DeleteSubParty(txtSubParty.Text);
                    if (count > 0)
                    {
                        MessageBox.Show("Sub Party deleted successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dtSubParty = dba.GetSubPartyNameWithFullName();
                        BindSearchListData();
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Sub Party is not Deleted ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
        }

        private void ClearAllText()
        {
           txtNickName.Text= txtSubParty.Text = txtMobileNo.Text = txtAddress.Text = txtMarka.Text = txtPostage.Text = txtSearchSubParty.Text =txtGSTNo.Text=txtPinCode.Text= "";
            txtPANNumber.Text= txtSalesParty.Text = txtState.Text = txtDistrict.Text = txtTransport.Text = txtBookingStation.Text = txtGroup.Text =txtPhoneNo.Text=txtRemark.Text= "";
            txtAmtLimit.Text = "0";
            chkOrangeZone.Checked = chkTransaction.Checked = chkBlackList.Checked = false;
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void BindSearchListData()
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text=="&Edit")
                {
                    lboxSubParty.Items.Clear();
                    string strSalesParty = txtSearchSalesParty.Text;
                    if (dtSubParty != null)
                    {
                        lboxSubParty.Items.Clear();
                        if (strSalesParty == "")
                        {
                            if (txtSearchSubParty.Text == "")
                            {
                                foreach (DataRow dr in dtSubParty.Rows)
                                {
                                    lboxSubParty.Items.Add(dr["FullName"]);
                                }
                            }
                            else
                            {

                                DataRow[] filteredRows = dtSubParty.Select(string.Format("{0} LIKE '%{1}%'", "FullName", txtSearchSubParty.Text));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxSubParty.Items.Add(dr["FullName"]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (txtSearchSalesParty.Text == "")
                            {
                                DataRow[] filteredRows = dtSubParty.Select(string.Format("HasteSale='" + strSalesParty + "'"));
                                foreach (DataRow dr in filteredRows)
                                {
                                    lboxSubParty.Items.Add(dr["FullName"]);
                                }
                                if (lboxSubParty.Items.Count > 0)
                                    lboxSubParty.SelectedIndex = 0;
                            }
                            else
                            {

                                DataRow[] filteredRows = dtSubParty.Select(string.Format("HasteSale='" + strSalesParty + "' and FullName LIKE ('%" + txtSearchSalesParty.Text + "%')"));
                                if (filteredRows.Length > 0)
                                {
                                    foreach (DataRow dr in filteredRows)
                                    {
                                        lboxSubParty.Items.Add(dr["FullName"]);
                                    }
                                    lboxSubParty.SelectedIndex = 0;
                                }
                            }
                        }
                    }
                    if (lboxSubParty.Items.Count > 0)
                        lboxSubParty.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding List Data in New Sub Party ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
                
        private void txtSearchSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    lboxSubParty.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lboxSubParty.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtSearchSubParty_TextChanged(object sender, EventArgs e)
        {
            BindSearchListData();
        }

        private void lboxSubParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                txtSearchSubParty.Text += e.KeyChar.ToString();
                txtSearchSubParty.Focus();
                txtSearchSubParty.Select(txtSearchSubParty.Text.Length, 0);
            }
            else if (e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Space))
            {
                txtSearchSubParty.Focus();
                txtSearchSubParty.Select(txtSearchSubParty.Text.Length, 0);
            }
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch 
            {               
            }
        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtAddress.SelectionStart < 2)
            {
                e.SuppressKeyPress = true;
            }
        }

        private bool CheckAvailability()
        {
            bool checkStatus = true;
            if (btnAdd.Text=="&Save" || (btnEdit.Text=="&Update" && _strSubParty != txtSubParty.Text))
            {
                try
                {
                    if (txtSubParty.Text != "")
                    {
                        DataRow[] filteredRows = dtSubParty.Select(string.Format("Name='" + txtSubParty.Text + "' and HasteSale='" + txtSalesParty.Text + "'"));
                        if (filteredRows.Length > 0)
                        {
                            lblMsg.Text = txtSubParty.Text + "  is already exists ! Please choose another Name..";
                            lblMsg.ForeColor = Color.Red;
                            lblMsg.Visible = true;
                            checkStatus = false;
                            txtSubParty.Focus();
                        }
                        else
                        {
                            lblMsg.Text = txtSubParty.Text + "  is available ........";
                            lblMsg.ForeColor = Color.Green;
                            lblMsg.Visible = true;
                            checkStatus = true;
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please choose sub party name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        checkStatus = false;
                        txtSubParty.Focus();

                    }
                }
                catch (Exception ex)
                {
                    string[] strReport = { "Exception occurred in Leave Event of Sub Party TextBox in sub Party", ex.Message };
                    dba.CreateErrorReports(strReport);
                }
            }
            else
            {
                lblMsg.Visible = false;
            }
            return checkStatus;
        }

        private bool EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bSubPartyAdd || MainPage.mymainObject.bSubPartyEdit || MainPage.mymainObject.bSubPartyView)
                {
                    if (!MainPage.mymainObject.bAddNewCustomer || !MainPage.mymainObject.bSubPartyAdd)
                        btnAdd.Enabled = false;
                    if (!(MainPage.mymainObject.bSubPartyEdit))
                        btnEdit.Enabled = btnDelete.Enabled = false;

                    if (!(MainPage.mymainObject.bSubPartyView))
                    {
                        lboxSubParty.Enabled = false;
                    }
                    return true;                
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.Close();
                }
            }
            catch
            {
            }
            return false;
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("SALESPARTY");
                        if (strData != "")
                            txtSalesParty.Text = strData;
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                                txtSalesParty.Text = strData;
                        }
                    }
                }
                e.Handled = true;
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
                        SearchData objSearch = new SearchData("OTHERGROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtGroup.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
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
                }
                e.Handled = true;
            }
            catch
            {
                e.Handled = true;
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
                        SearchData objSearch = new SearchData("DISTRICTNAME", txtState.Text, "SEARCH DISTRICT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtDistrict.Text = objSearch.strSelectedData;
                    }
                }
                    e.Handled = true;
            }
            catch
            {
                e.Handled = true;
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
                }
                e.Handled = true;
            }
            catch
            {
                e.Handled = true;
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
                            txtDistrict.Text = "";
                    }
                }
                    e.Handled = true;
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void txtSearchParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSearchSalesParty.Text = objSearch.strSelectedData;
                    GetPartyNameByGroup();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetPartyNameByGroup()
        {
            try
            {
                txtSearchSubParty.Clear();
                lboxSubParty.Items.Clear();
                if (dtSubParty != null)
                {
                    if (txtSearchSalesParty.Text != "")
                    {
                        string[] strSplitName = txtSearchSalesParty.Text.Split(' ');
                        if (strSplitName.Length > 0)
                        {
                            DataRow[] dr = dtSubParty.Select(String.Format("HasteSale='" + strSplitName[0] + "'"));
                            foreach (DataRow row in dr)                            
                                lboxSubParty.Items.Add(row["FullName"]);                            
                        }
                        else
                        {
                            foreach (DataRow row in dtSubParty.Rows)
                                lboxSubParty.Items.Add(row["FullName"]);
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dtSubParty.Rows)
                        {
                            lboxSubParty.Items.Add(dr["FullName"]);
                        }                      
                    }
                }
                if (lboxSubParty.Items.Count > 0)
                    lboxSubParty.SelectedIndex = 0;
            }
            catch
            {
            }
        }

        private void txtSubParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
            else
                e.Handled = true;
        }

        private void txtSearchSubParty_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    btnAdd.Text = "&Add";
                    btnEdit.Text = "&Update";
                    EnabaleControl();
                    txtSalesParty.Focus();
                    lboxSubParty.Enabled = false;                    
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateControl())
                    {
                        if (CheckAvailability())
                        {
                            DialogResult dr = MessageBox.Show(" Are you sure want to save data .....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                UpdateData();
                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("Sorry ! Require Fields can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click Event of Submit Button in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnEdit.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                if (btnAdd.Text == "&Save")
                {
                    btnAdd.Text = "&Add";
                    BindListData();
                }
                lboxSubParty.Enabled = true;
                txtSearchSalesParty.Visible = txtSearchSubParty.Visible = true;
                if (lboxSubParty.Items.Count > 0)
                    lboxSubParty.SelectedIndex = 0;
            }
            catch
            {
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download Sub Party ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadSubPartyMaster(MainPage.strOnlineDataBaseName);
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! Sub party master downloaded successfully... ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                        SendSMSPage objSMS = new SSS.SendSMSPage(txtMobileNo.Text, strMessage);
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
                        PrintShippingDetails(true);
                    else
                        PrintShippingDetails(true);
                }
            }
            catch { }
            btnPrintDetails.Enabled = true;
        }

        private void PrintShippingDetails(bool _bStatus)
        {
            DataTable _dt = CreateShippingDataTable();
            if (_dt.Rows.Count > 0)
            {
                Reporting.PartyShippingDetails objReport = new Reporting.PartyShippingDetails();
                objReport.SetDataSource(_dt);
                objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                if (_bStatus)
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
            row["PartyName"] = txtAreaCode.Text + txtAccountNo.Text + " " + txtSubParty.Text;
            row["NickName"] = txtAddress.Text.Replace("\n", " ").Replace("\r", " ") + " " + txtDistrict.Text + " " + txtState.Text+"-"+txtPinCode.Text;
            row["BankName"] = txtState.Text;
            row["BranchName"] = txtGSTNo.Text;
            row["IFSCCode"] = txtTransport.Text;
            row["BankAccountName"] = txtDistrict.Text;
            row["BankAccountNo"] = txtMobileNo.Text;
            row["PrintedBy"] = "PRINTED BY : " + MainPage.strLoginName + ", Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
            table.Rows.Add(row);

            return table;
        }

        private string GetShippingDetailsSMS()
        {
            string strMessage = "";
            strMessage += "SHIPPING DETAILS\n"
                       + "PARTY NAME : " + txtAreaCode.Text + txtAccountNo.Text + " " + txtSubParty.Text + "\n"
                       + "ADDRESS : " + txtAddress.Text.Replace("\n", " ").Replace("\r", " ") + " " + txtDistrict.Text + " " + txtState.Text + "-" + txtPinCode.Text + "\n"
                       + "GST No : " + txtGSTNo.Text + "\n"
                       + "TRANSPORT : " + txtTransport.Text + "\n"
                       + "STATION : " + txtDistrict.Text + "\n"
                       + "PHONE No : " + txtMobileNo.Text;
            return strMessage;
        }

        private void txtGSTNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtGSTNo.Text != "")
                {
                    txtPANNumber.Text = txtGSTNo.Text.Substring(2, 10);
                }
            }
        }

        private void txtPinCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {

        }

        private void chkBlackList_CheckedChanged(object sender, EventArgs e)
        {

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

        private void txtMobileNo_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            try
            {               

                if (txtMobileNo.Text.Length == 10 && !MainPage.strLoginName.Contains("RAJESH") && !MainPage.strUserRole.Contains("ADMIN") && !MainPage.strLoginName.Contains("SADHANA") && !MainPage.strLoginName.Contains("MANMOHAN") && !MainPage.strLoginName.Contains("CHHOTUSINGH") && btnAdd.Text != "&Save")
                    txtMobileNo.ReadOnly = true;
                else
                    txtMobileNo.ReadOnly = false;

                dba.KeyHandlerPoint(sender,e,0);
            }
            catch { }
        }

        private void txtNickName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
    }
}
