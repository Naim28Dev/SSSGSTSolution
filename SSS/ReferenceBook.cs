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
    public partial class ReferenceBook : Form
    {
        DataBaseAccess dba;
        DataTable _dtTable = null;
        string strSelectedName = "";
        protected internal string strAddedName="",__strName="";
        bool _bNewStatus = false;
        public ReferenceBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetAreaCode();
            txtName.Focus();
            GetAllData();           
        }

        public ReferenceBook(bool _nStatus,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _bNewStatus = _nStatus;
            GetAreaCode();           
            GetAllData();
        }

        public ReferenceBook(string strParty)
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
                txtAreaCode.Text += "R";
               // btnAdd.Enabled = btnEdit.Enabled = true;
            }
            return true;
        }

        private void GetAllData()
        {
            _dtTable = dba.GetDataTable("Select (AreaCode+AccountNo+' '+Name)PartyName from AddressBook Where AreaCode like('%R')  Order By Name ");
            if (!_bNewStatus)
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
                    lblMsg.Text = txtName.Text + "  is available ........";
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(CAST(AccountNo as int)),0)+1) from AddressBook Where AreaCode like('%R') ");
            txtAccountNo.Text = Convert.ToString(objValue);
        }

        private void ClearAllText()
        {
            try
            {
                lblMsg.Text = "";
                txtAddress.Clear();
                txtMobileNo.Clear();
                //txtNickName.Clear();
                txtPhone.Clear();
                txtName.Clear();
                txtSTD.Clear();
                txtGradeType.Clear();
                txtStationName.Clear();
                txtStateName.Clear();
                txtWhatsappNo.Clear();
                txtEmailID.Clear();
                txtRemark.Clear();
                chkTransaction.Checked = false;
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
                       // txtNickName.Text = Convert.ToString(dr["NickName"]);
                        txtMobileNo.Text = Convert.ToString(dr["MobileNo"]);
                        txtSTD.Text = Convert.ToString(dr["PhoneNoCode"]);
                        txtPhone.Text = Convert.ToString(dr["PhoneNo"]);
                        txtAddress.Text = Convert.ToString(dr["Address"]);
                        txtGradeType.Text = Convert.ToString(dr["GroupName"]);
                        txtStationName.Text = Convert.ToString(dr["City"]);
                        txtStateName.Text = Convert.ToString(dr["State"]);

                        txtWhatsappNo.Text = Convert.ToString(dr["WhatsappNo"]);
                        txtEmailID.Text = Convert.ToString(dr["EmailID"]);                       
                        txtDate.Text = Convert.ToString(dr["VDate"]);                        
                        txtRemark.Text = Convert.ToString(dr["Remark"]);
                        string strReference = Convert.ToString(dr["Reference"]);
                        if (strReference.ToUpper().Contains("LOCKED"))
                            chkTransaction.Checked = true;
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
            if (btnAdd.Text == "&Save")
            {
                BindListData();
            }
        }

        private void BindListData()
        {
            try
            {
                if (_dtTable != null)
                {
                    if (txtName.Text == "")
                    {
                        lboxName.Items.Clear();
                        foreach (DataRow dr in _dtTable.Rows)
                        {
                            lboxName.Items.Add(dr["PartyName"]);
                        }
                    }
                    else
                    {

                        DataRow[] filteredRows = _dtTable.Select(string.Format("{0} LIKE '%{1}%'", "PartyName", txtName.Text));
                        if (filteredRows.Length > 0)
                        {
                            lboxName.Items.Clear();
                            foreach (DataRow dr in filteredRows)
                            {
                                lboxName.Items.Add(dr["PartyName"]);
                            }
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
                string[] strReport = { "Error occurred in Bindning List data  in Reference Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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
                        //txtName.Focus();
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
            try
            {
                if (EditOption())
                {
                    if (_bNewStatus)
                    {
                        btnAdd.PerformClick();
                        txtSearchName.TabStop = lboxName.TabStop = false;
                        btnDelete.Enabled = btnEdit.Enabled = btnSearch.Enabled = false;
                        txtName.Text = __strName;
                        txtName.Focus();
                        
                    }
                }
            }
            catch { }
        }

        private bool EditOption()
        {
            try
            {
                try
                {
                    if (MainPage.mymainObject.bRefrenceMasterEntry || MainPage.mymainObject.bRefrenceMasterView || MainPage.mymainObject.bRefrenceMasterEditDelete)
                    {
                        if (!(MainPage.mymainObject.bRefrenceMasterEntry))
                            btnAdd.Enabled = false;
                        if (!(MainPage.mymainObject.bRefrenceMasterEditDelete))
                            btnEdit.Enabled = false;

                        if (!(MainPage.mymainObject.bPartyMasterEdit))
                            btnDelete.Enabled = false;

                        if (!(MainPage.mymainObject.bRefrenceMasterView))                        
                            lboxName.Enabled = false;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();                       
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Close();
                }
            }
            catch
            {
            }
            return false;
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
            txtName.ReadOnly = txtEmailID.ReadOnly = txtAddress.ReadOnly = txtMobileNo.ReadOnly = txtSTD.ReadOnly = txtPhone.ReadOnly = txtRemark.ReadOnly =  false;
            chkTransaction.Enabled = true;
        }

        private void DisableControl()
        {
            txtName.ReadOnly = txtEmailID.ReadOnly = txtAddress.ReadOnly =  txtMobileNo.ReadOnly = txtSTD.ReadOnly = txtPhone.ReadOnly =  txtRemark.ReadOnly =  true;
            chkTransaction.Enabled = false;
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
            
           
            return true;
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
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
            string strReference = "";
            if (chkTransaction.Checked)
                strReference = "LOCKED";

            strQuery += " if not exists (Select Name from [dbo].[AddressBook] Where AreaCode like('%R') and Name='" + txtName.Text+ "') begin Declare @ID varchar(20) Select @ID=(ISNULL(MAX(CAST(AccountNo as int)),0)+1) from [dbo].[AddressBook] "
                     + " INSERT INTO [dbo].[AddressBook] ([Name],[NickName],[GroupName],[MobileNo],[PhoneNoCode],[PhoneNo],[Address],[PinCode],[City],[State],[InsertStatus],[UpdateStatus],[AreaCode],[AccountNo],[WhatsappNo],[EmailID],[VisitedBy],[VisitedDate],[GSTNo],[Reference],[Remark],[CreatedBy],[UpdatedBy])VALUES "
                     + " ('" + txtName.Text + "','','" + txtGradeType.Text + "','" + txtMobileNo.Text + "','" + txtSTD.Text + "','" + txtPhone.Text + "','" + txtAddress.Text + "','','" + txtStationName.Text + "','" + txtStateName.Text + "',1,0,'" + txtAreaCode.Text + "',@ID,'" + txtWhatsappNo.Text + "','" + txtEmailID.Text + "','','" + _date.ToString("MM/dd/yyyy") + "','','"+ strReference+"','" + txtRemark.Text + "','" + MainPage.strLoginName + "','') "
                     + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + " ('REFERENCEBOOK','" + txtAreaCode.Text+ "',@ID,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                MessageBox.Show("Thank You ! Record saved successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                btnAdd.Text = "&Add";
                if (_bNewStatus)
                {
                    strAddedName = txtName.Text;
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
                    DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;

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
                    string strReference = "";
                    if (chkTransaction.Checked)
                        strReference = "LOCKED";

                    strAccountID = strName[0];
                    if (strAccountID != "")
                    {
                        string strQuery = "";

                        strQuery += " if exists (Select Name from [dbo].[AddressBook] Where ([AreaCode]+[AccountNo])='" + strAccountID + "' ) begin "
                                 + " UPDATE [dbo].[AddressBook] SET [Name]='" + txtName.Text + "',[NickName]='',[GroupName]='" + txtGradeType.Text + "',[MobileNo]='" + txtMobileNo.Text + "',[PhoneNoCode]='" + txtSTD.Text + "',[PhoneNo]='" + txtPhone.Text + "',[Address]='" + txtAddress.Text + "',[PinCode]='',[City]='" + txtStationName.Text + "',[State]='" + txtStateName.Text + "',[UpdateStatus]=1,"
                                 + " [WhatsappNo]='" + txtWhatsappNo.Text + "',[EmailID]='" + txtEmailID.Text + "',[VisitedBy]='',[VisitedDate]='" + _date.ToString("MM/dd/yyyy") + "',[Reference]='" + strReference+"',[Remark]='" + txtRemark.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "' Where ([AreaCode]+[AccountNo])='" + strAccountID + "' "
                                 + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + " ('REFERENCEBOOK','" + txtAreaCode.Text + "',"+txtAccountNo.Text+",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'UPDATION') end ";
                        
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
                    EditTrailDetails objEdit = new EditTrailDetails("REFERENCEBOOK", txtAreaCode.Text, txtAccountNo.Text);
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

                        SearchData objSearch = new SearchData("ALLPARTYNICKNAME", "SEARCH NICK NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportParty.Text = objSearch.strSelectedData;
                            GetDataFromMaster();
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

        private void GetDataFromMaster()
        {
            try
            {
                string strQuery = " Select SM.Other as NickName,MobileNo,PhoneNo,SOD.WaybillUserName as WhatsappNo,Address,Station,State from SupplierMaster SM left join SupplierOtherDetails SOD on SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo WHere GroupName in ('SUNDRY CREDITOR','SUNDRY DEBTORS') and SM.Other='"+txtImportParty.Text+"' ";
                DataTable _dt = dba.GetDataTable(strQuery);
                if(_dt.Rows.Count>0)
                {
                    DataRow row = _dt.Rows[0];
                    txtName.Text = Convert.ToString(row["NickName"]);
                    txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                    txtPhone.Text = Convert.ToString(row["PhoneNo"]);
                    txtWhatsappNo.Text= Convert.ToString(row["WhatsappNo"]);
                    txtAddress.Text = Convert.ToString(row["Address"]);
                    txtStationName.Text = Convert.ToString(row["Station"]);
                    txtStateName.Text = Convert.ToString(row["State"]);
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download reference ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadReferenceMaster(MainPage.strOnlineDataBaseName);                    
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! New reference master downloaded successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("No master found  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please enter online database name and Live IP in company master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnDownload.Enabled = true;
        }
    }
}
