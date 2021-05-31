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
    public partial class AddressBook : Form
    {
        DataBaseAccess dba;
        DataTable _dtTable = null;
        protected internal string strSelectedName = "";
        bool _bNewStatus = false;
        public AddressBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetAreaCode();
            txtName.Focus();
            GetAllData();           
        }

        public AddressBook(bool _nStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _bNewStatus = _nStatus;
            GetAreaCode();
            txtName.Focus();
            GetAllData();
        }

        public AddressBook(string strParty)
        {
            InitializeComponent();
            dba = new DataBaseAccess();  
            GetAllData();
            lboxName.SelectedItem = strParty;
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
                txtAreaCode.Text += "A";
                btnAdd.Enabled = btnEdit.Enabled = true;
            }
            return true;
        }

        private void GetAllData()
        {
            _dtTable = dba.GetDataTable("Select (AreaCode+AccountNo+' '+Name)PartyName from AddressBook Where AreaCode like('%A') Order By Name ");
            BindSearchListData();
        }

        private void BindSearchListData()
        {
            try
            {
                lboxName.Visible = true;
                lboxName.Items.Clear();
                if (txtSearchName.Text == "")
                {
                    foreach (DataRow dr in _dtTable.Rows)
                    {
                        lboxName.Items.Add(dr["PartyName"]);
                    }
                }
                else
                {
                    DataRow[] filteredRows = _dtTable.Select(string.Format("{0} LIKE '%{1}%'", "PartyName", txtSearchName.Text));
                    if (filteredRows.Length > 0)
                    {
                        foreach (DataRow dr in filteredRows)
                        {
                            lboxName.Items.Add(dr["PartyName"]);
                        }
                    }
                }
                if (lboxName.Items.Count > 0)
                {
                    lboxName.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding List Data in Address Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool CheckDuplicacy()
        {
            bool check = true;
            string _strName = txtAreaCode.Text + txtAccountNo.Text + " " + txtName.Text;
            if (strSelectedName != _strName)
            {
                DataRow[] filteredRows = _dtTable.Select(string.Format("{0} LIKE '{1}'", "Name", txtName.Text));
                if (filteredRows.Length > 0)
                {
                    lblMsg.Text = txtName.Text + "  already exist ! Please choose another Name..";
                    lblMsg.ForeColor = Color.Red;
                    lblMsg.Visible = true;
                    txtName.Focus();
                    check = false;
                }
                else
                {
                    lblMsg.Text = txtName.Text + "  is Available ........";
                    lblMsg.ForeColor = Color.Green;
                    lblMsg.Visible = true;
                    check = true;
                }
            }
            return check;
        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtAddress.SelectionStart < 2)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void txtPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void AddressBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !txtAddress.Focused)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetMaxAccountNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(CAST(AccountNo as int)),0)+1) from AddressBook Where AreaCode like('%A') ");
            txtAccountNo.Text = Convert.ToString(objValue);
        }

        private void ClearAllText()
        {
            try
            {
                lblMsg.Text = "";
                txtAddress.Clear();
                txtMobileNo.Clear();
                txtNickName.Clear();
                txtPhone.Clear();
                txtName.Clear();
                txtSTD.Clear();
                txtPINCode.Clear();
                txtGradeType.Clear();
                txtStationName.Clear();
                txtStateName.Clear();
                txtWhatsappNo.Clear();
                txtEmailID.Clear();
                txtVisitedBy.Clear();
                txtGSTNo.Clear();
                txtReference.Clear();
                txtRemark.Clear();
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strParty = Convert.ToString(lboxName.SelectedItem);
                string[] strSplitName = strParty.Split(' ');
                if (strSplitName.Length > 0)
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to delete address book", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strQuery = " Delete from [AddressBook] Where (AreaCode+AccountNo)='" + strSplitName[0] + "' ";
                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count > 0)
                        {
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            GetAllData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Delete Button in Address Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void lboxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strParty = Convert.ToString(lboxName.SelectedItem);
                    if (strParty != "")
                    {
                        strSelectedName = strParty;
                        BindRecordwithControls(strParty);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindRecordwithControls(string strFullName)
        {
            try
            {
                string[] strName = strFullName.Split(' ');
                if (strName.Length > 1)
                {
                    DataTable _dt = dba.GetDataTable("Select *,CONVERT(varchar,VisitedDate,103)VDate from [AddressBook] Where (AreaCode+AccountNo)='" + strName[0] + "' ");

                    if (_dt.Rows.Count > 0)
                    {
                        DataRow dr = _dt.Rows[0];
                        txtAreaCode.Text = Convert.ToString(dr["AreaCode"]);
                        txtAccountNo.Text = Convert.ToString(dr["AccountNo"]);
                        txtName.Text = Convert.ToString(dr["Name"]);
                        txtNickName.Text = Convert.ToString(dr["NickName"]);
                        txtMobileNo.Text = Convert.ToString(dr["MobileNo"]);
                        txtSTD.Text = Convert.ToString(dr["PhoneNoCode"]);
                        txtPhone.Text = Convert.ToString(dr["PhoneNo"]);
                        txtAddress.Text = Convert.ToString(dr["Address"]);
                        txtPINCode.Text = Convert.ToString(dr["PinCode"]);
                        txtGradeType.Text = Convert.ToString(dr["GroupName"]);
                        txtStationName.Text = Convert.ToString(dr["City"]);
                        txtStateName.Text = Convert.ToString(dr["State"]);

                        txtWhatsappNo.Text = Convert.ToString(dr["WhatsappNo"]);
                        txtEmailID.Text = Convert.ToString(dr["EmailID"]);
                        txtVisitedBy.Text = Convert.ToString(dr["VisitedBy"]);
                        txtDate.Text = Convert.ToString(dr["VDate"]);
                        txtGSTNo.Text = Convert.ToString(dr["GSTNo"]);
                        txtReference.Text = Convert.ToString(dr["Reference"]);
                        txtRemark.Text = Convert.ToString(dr["Remark"]);

                        lblCreatedBy.Text = "";
                        string strCreatedBy = Convert.ToString(dr["CreatedBy"]), strUpdatedBy = Convert.ToString(dr["UpdatedBy"]);
                        if (strCreatedBy != "")
                            lblCreatedBy.Text = "Created By : " + strCreatedBy;
                        if (strUpdatedBy != "")
                            lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            //if (btnSubmit.Text == "&Submit")
            //{
            //    BindListData();
            //}
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (strSelectedName != txtName.Text)
                {
                    if (txtName.Text != "")
                    {
                        CheckDuplicacy();
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        txtName.Focus();
                    }
                }
                else
                {
                    lblMsg.Visible = false;
                }
            }
            catch
            {
            }
        }

        private void AddressBook_Load(object sender, EventArgs e)
        {
            EditOption();
            if (_bNewStatus)
            {
                btnDelete.Enabled = btnEdit.Enabled = btnSearch.Enabled = false;
                btnAdd.PerformClick();
                txtName.Focus();
            }
        }

        private void EditOption()
        {
            try
            {
                try
                {
                    if (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bPartyMasterView)
                    {
                        if (!(MainPage.mymainObject.bPartyMasterAdd))
                            btnAdd.Enabled = false;
                        if (!(MainPage.mymainObject.bPartyMasterEdit))
                            btnEdit.Enabled = false;

                        if (!(MainPage.mymainObject.bPartyMasterEdit) || !MainPage.strUserRole.Contains("ADMIN"))
                            btnDelete.Enabled = false;

                        if (!(MainPage.mymainObject.bPartyMasterView))
                        {
                            lboxName.Enabled = false;
                        }
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
            }
            catch
            {
            }
        }

        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            BindSearchListData();
        }


        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CASHTYPESALE", "SEARCH PARTY TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGradeType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCityName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH CITY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStationName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtStateName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStateName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void EnabaleControl()
        {
            txtName.ReadOnly = txtEmailID.ReadOnly = txtAddress.ReadOnly = txtPINCode.ReadOnly = txtMobileNo.ReadOnly = txtSTD.ReadOnly = txtPhone.ReadOnly = txtNickName.ReadOnly = txtVisitedBy.ReadOnly = txtReference.ReadOnly = txtRemark.ReadOnly = txtGSTNo.ReadOnly = false;
        }

        private void DisableControl()
        {
            txtName.ReadOnly = txtEmailID.ReadOnly = txtAddress.ReadOnly = txtPINCode.ReadOnly = txtMobileNo.ReadOnly = txtSTD.ReadOnly = txtPhone.ReadOnly = txtNickName.ReadOnly = txtVisitedBy.ReadOnly = txtReference.ReadOnly = txtRemark.ReadOnly = txtGSTNo.ReadOnly = true;
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
                MessageBox.Show("Name can't be blank !  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Date is not valid, Please enter valid date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }

            if (txtGSTNo.Text != "" && txtGSTNo.Text.Length != 15)
            {
                MessageBox.Show("GST No is not valid, Please enter valid GST no.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGSTNo.Focus();
                return false;
            }
            else if (txtGSTNo.Text != "")
            {
                if (txtStateName.Text == "")
                {
                    MessageBox.Show("State name can't be Blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStateName.Focus();
                    return false;
                }
                else
                {

                    string strStateCode = txtGSTNo.Text.Substring(0, 2);
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select StateName+'|'+ISNULL((Select Top 1 Name from [dbo].[AddressBook] Where (AreaCode+AccountNo)!='" + txtAreaCode.Text + txtAccountNo.Text + "' and GSTNo='" + txtGSTNo.Text + "'),'') PartyName from StateMaster Where StateCode='" + strStateCode + "' ");
                    string strValues = Convert.ToString(objValue);
                    string[] strValue = strValues.Split('|');
                    if (strValue.Length > 1)
                    {
                        if (strValue[0] != txtStateName.Text)
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
            return true;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Save";
                    btnEdit.Text = "&Edit";
                    ClearAllText();
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
                        if (dr == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click Event of Save Button in Address Book Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            string strQuery = "";

            strQuery += " if not exists (Select Name from [dbo].[AddressBook] Where AreaCode like('%A') and Name='" + txtName.Text+ "') begin Declare @ID varchar(20) Select @ID=(ISNULL(MAX(CAST(AccountNo as int)),0)+1) from [dbo].[AddressBook] Where AreaCode like('%A') "
                     + " INSERT INTO [dbo].[AddressBook] ([Name],[NickName],[GroupName],[MobileNo],[PhoneNoCode],[PhoneNo],[Address],[PinCode],[City],[State],[InsertStatus],[UpdateStatus],[AreaCode],[AccountNo],[WhatsappNo],[EmailID],[VisitedBy],[VisitedDate],[GSTNo],[Reference],[Remark],[CreatedBy],[UpdatedBy])VALUES "
                     + " ('" + txtName.Text + "','" + txtNickName.Text + "','" + txtGradeType.Text + "','" + txtMobileNo.Text + "','" + txtSTD.Text + "','" + txtPhone.Text + "','" + txtAddress.Text + "','" + txtPINCode.Text + "','" + txtStationName.Text + "','" + txtStateName.Text + "',1,0,'" + txtAreaCode.Text + "',@ID,'" + txtWhatsappNo.Text + "','" + txtEmailID.Text + "','" + txtVisitedBy.Text + "','" + _date.ToString("MM/dd/yyyy") + "','" + txtGSTNo.Text + "','" + txtReference.Text + "','" + txtRemark.Text + "','" + MainPage.strLoginName + "','') "
                     + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + " ('ADDRESSBOOK','" + txtAreaCode.Text+ "',@ID,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                MessageBox.Show("Thank You ! Record saved successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                btnAdd.Text = "&Add";
                if (_bNewStatus)
                {
                    strSelectedName = txtName.Text;
                    this.Close();
                }
                else
                {
                    ClearAllText();
                    GetAllData();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Add";
                    btnEdit.Text = "&Update";
                    EnabaleControl();
                    txtName.Focus();
                    lboxName.Enabled = false;
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateControls())
                    {
                        DialogResult dr = MessageBox.Show("Are you sure want to update record .....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
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

        private void UpdateRecord()
        {
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            string strParty = Convert.ToString(lboxName.SelectedItem),strAccountID="";
            if (strParty != "")
            {
                string[] strName = strParty.Split(' ');
                if (strName.Length > 1)
                {
                    strAccountID = strName[0];
                    if (strAccountID != "")
                    {
                        string strQuery = "";

                        strQuery += " if exists (Select Name from [dbo].[AddressBook] Where ([AreaCode]+[AccountNo])='" + strAccountID + "' ) begin "
                                 + " UPDATE [dbo].[AddressBook] SET [Name]='" + txtName.Text + "',[NickName]='" + txtNickName.Text + "',[GroupName]='" + txtGradeType.Text + "',[MobileNo]='" + txtMobileNo.Text + "',[PhoneNoCode]='" + txtSTD.Text + "',[PhoneNo]='" + txtPhone.Text + "',[Address]='" + txtAddress.Text + "',[PinCode]='" + txtPINCode.Text + "',[City]='" + txtStationName.Text + "',[State]='" + txtStateName.Text + "',"
                                 + " [WhatsappNo]='" + txtWhatsappNo.Text + "',[EmailID]='" + txtEmailID.Text + "',[VisitedBy]='" + txtVisitedBy.Text + "',[VisitedDate]='" + _date.ToString("MM/dd/yyyy") + "',[GSTNo]='" + txtGSTNo.Text + "',[Reference]='" + txtReference.Text + "',[Remark]='" + txtRemark.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where ([AreaCode]+[AccountNo])='" + strAccountID + "' "
                                 + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + " ('ADDRESSBOOK','" + txtAreaCode.Text + "',"+txtAccountNo.Text+",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'UPDATION') end ";
                        
                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count > 0)
                        {
                            MessageBox.Show("Thank You ! Record updated successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            lboxName.Enabled = true;
                            ClearAllText();
                            GetAllData();
                        }
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                if (btnAdd.Text == "&Save")
                {
                    btnAdd.Text = "&Add";
                    GetAllData();
                }
                lboxName.Enabled = true;                
                if (lboxName.Items.Count > 0)
                    lboxName.SelectedIndex = 0;
            }
            catch
            {
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtAreaCode.Text != "" && txtAccountNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("ADDRESSBOOK", txtAreaCode.Text, txtAccountNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void AddressBook_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtSearchName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    lboxName.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lboxName.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender,true,true,true);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
    }
}
