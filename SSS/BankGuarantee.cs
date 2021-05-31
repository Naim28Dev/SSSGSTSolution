using System;
using System.Data;
using System.Windows.Forms;

namespace SSS
{
    public partial class BankGuarantee : Form
    {
        DataBaseAccess dba;
        public BankGuarantee()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtBillCode.Text = MainPage.strBranchCode + "BG";
            BindLastRecord();
        }
        public BankGuarantee(string strCode, string strSerial)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            if (strCode == "")
                txtBillCode.Text = MainPage.strBranchCode + "BG";
            else
                txtBillCode.Text = strCode;

            BindAllDetails(strSerial);
        }

        private void BankGuarantee_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void ClearAllFields()
        {         
            lblCreatedBy.Text = "";
          
            txtBillCode.Text = MainPage.strBranchCode+"BG";
            txtCustomerName.Text = "";
            txtBankGuaranteeNo.Text = "";
            txtBankName.Text = "";
            txtReason.Text = "";
            txtAmount.Text = "";

            if (DateTime.Today > MainPage.startFinDate)
            {
                txtDate.Text = MainPage.strCurrentDate;
                txtValidUptoDate.Text = txtDate.Text;
            }
            else
            {
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtValidUptoDate.Text = txtDate.Text;
            }
        }
      

        private void SetSerialNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(BillNo),0)+1) BillNo from BankGuarantee Where BillCode='" + txtBillCode.Text + "' ");
            txtBillNo.Text = Convert.ToString(objValue);
        }
        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from BankGuarantee Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDetails(strSerialNo);
            }
            else
                ClearAllFields();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from BankGuarantee Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDetails(strSerialNo);
            }
            else
                ClearAllFields();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from BankGuarantee Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDetails(strSerialNo);
            }
            else
            {
                BindLastRecord();
            }
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from  BankGuarantee Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDetails(strSerialNo);
            }
        }
        private void BindAllDetails(string strSerialNo)
        {
            try
            {
                string strQuery = " Select Top 1 [BillCode],[BillNo],Convert(varchar,[Date],103) Date,dbo.GetFullName(CustomerName) as CustomerName,[BankGuaranteeNo],[Amount],[BankName],Convert(varchar,[ValidUpto],103) ValidUpto,[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus] FROM BankGuarantee WHERE BillCode = '"+ txtBillCode.Text +"' and BillNo= " + strSerialNo;
                DataTable dt = dba.GetDataTable(strQuery);

                ClearAllFields();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtBillNo.Text = Convert.ToString(row["BillNo"]);
                    txtBillCode.Text = Convert.ToString(row["BillCode"]);
                    txtDate.Text = Convert.ToString(row["Date"]);
                    txtAmount.Text = Convert.ToDouble(row["Amount"]).ToString("N2",MainPage.indianCurancy);
                    txtCustomerName.Text = Convert.ToString(row["CustomerName"]);
                    txtBankGuaranteeNo.Text = Convert.ToString(row["BankGuaranteeNo"]);
                    txtBankName.Text = Convert.ToString(row["BankName"]);
                    txtValidUptoDate.Text = Convert.ToString(row["ValidUpto"]);

                    string strCreatedBy = Convert.ToString(dt.Rows[0]["CreatedBy"]), strUpdatedBy = Convert.ToString(dt.Rows[0]["UpdatedBy"]);
                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;

                    DisableAllControl();
                }
                txtBillNo.ReadOnly = false;
            }
            catch { }
        }
        private void SaveRecord()
        {
            try
            {
                string[] strFullCustomer = txtCustomerName.Text.Split(' ');
                if (strFullCustomer.Length > 1)
                {
                    string strCustomerName = "";
                    strCustomerName = strFullCustomer[0];
                    DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    DateTime strValid = dba.ConvertDateInExactFormat(txtValidUptoDate.Text);
                    double dAmt = dba.ConvertObjectToDouble(txtAmount.Text);
                    string strQuery = " Declare @SerialNo int ";

                    strQuery += " Select @SerialNo=(ISNULL(MAX(BillNo),0)+1) from dbo.[BankGuarantee] Where BillCode='" + txtBillCode.Text + "' ";

                    strQuery += " if not exists (Select BillNo from [dbo].[BankGuarantee] Where BillCode + ' ' + Cast(BillNo as Nvarchar(50)) = '" + txtBillCode.Text + " " + txtBillNo.Text
                                    + "') begin INSERT INTO [dbo].[BankGuarantee] ([BillCode],[BillNo],[Date],[CustomerName],[BankGuaranteeNo],[Amount],[BankName],[ValidUpto],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                    + "('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strCustomerName + "','" + txtBankGuaranteeNo.Text + "',"
                                    + dAmt + ",'" + txtBankName.Text + "','" + strValid.ToString("MM/dd/yyyy") + "','" + MainPage.strLoginName + "','',1,0) ";

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                    + "('BANKGUARANTEE','" + txtBillCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION')  end ";

                    int _count = dba.ExecuteMyQuery(strQuery);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you..! Record Saved successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnAdd.Text = "&Add";
                    }
                }
            }
            catch
            {
                MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    EnableAllControl();
                    ClearAllFields();
                    SetSerialNo();
                    txtDate.Focus();
                    if (!MainPage.mymainObject.bCashEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else
                {
                    if (ValidateControls())
                    {
                        DialogResult dar = MessageBox.Show("Are you sure you want to Save Record ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dar == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
            }
            catch { }
        }

        private void EnableAllControl()
        {
            txtAmount.ReadOnly = txtBankGuaranteeNo.ReadOnly = txtDate.ReadOnly = txtValidUptoDate.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtAmount.ReadOnly = txtBankGuaranteeNo.ReadOnly = txtDate.ReadOnly = txtValidUptoDate.ReadOnly = true;
        }
        private void btnEdit_Click(object sender, EventArgs e)
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

                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    if (btnEdit.Enabled)
                    {
                        EnableAllControl();
                        pnlDeletionConfirmation.Visible = false;
                        btnEdit.Text = "&Update";
                        txtDate.Focus();
                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateControls())
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch { }
            btnEdit.Enabled = true;
        }


        private void UpdateRecord()
        {
            try
            {
                string[] strFullCustomer = txtCustomerName.Text.Split(' ');
                if (strFullCustomer.Length > 1)
                {
                    string strCustomerName = "";
                    strCustomerName = strFullCustomer[0];
                    DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    DateTime strValid = dba.ConvertDateInExactFormat(txtValidUptoDate.Text);
                    double dAmt = dba.ConvertObjectToDouble(txtAmount.Text);
                    string strQuery = " Update [dbo].[BankGuarantee] Set [BillCode]='" + txtBillCode.Text + "',[Date]='" + strDate.ToString("MM/dd/yyyy")
                                + "',[CustomerName]='" + strCustomerName + "',[BankGuaranteeNo]='" + txtBankGuaranteeNo.Text
                                + "',[Amount]=" + dAmt + ",[BankName]='" + txtBankName.Text + "',[ValidUpto]='" + strValid.ToString("MM/dd/yyyy")
                                + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 WHERE BillCode + ' ' + Cast(BillNo as Nvarchar(50)) = '" + txtBillCode.Text + " " + txtBillNo.Text + "'";

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                              + " ('BANKGUARANTEE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                    int _count = dba.ExecuteMyQuery(strQuery);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you..! Record Updated successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnEdit.Text = "&Edit";
                        BindAllDetails(txtBillNo.Text);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (btnAdd.Text != "&Save" && txtBillNo.Text != "" && txtBillCode.Text != "" && dba.ValidateBackDateEntry(txtDate.Text))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = ValidateInsertStatus();
                            string strQuery = " Delete from BankGuarantee Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('BANKGUARANTEE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtAmount.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!_bStatus)
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                pnlDeletionConfirmation.Visible = false;
                                MessageBox.Show("Thank you ! Record delete successfully ", "Record Delete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindNextRecord();
                            }
                            else
                                MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReason.Focus();
                }
            }
            catch
            {
            }
            btnFinalDelete.Enabled = true;
        }
        private bool ValidateInsertStatus()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from BankGuarantee Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
            return Convert.ToBoolean(objValue);
        }
        private void BankGuarantee_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlDeletionConfirmation.Visible)
                        pnlDeletionConfirmation.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bCashView)
                    {
                        if (e.KeyCode == Keys.PageUp)
                        {
                            BindNextRecord();
                        }
                        else if (e.KeyCode == Keys.PageDown)
                        {
                            BindPreviousRecord();
                        }
                        else if (e.KeyCode == Keys.Home)
                        {
                            BindFirstRecord();
                        }
                        else if (e.KeyCode == Keys.End)
                        {
                            BindLastRecord();
                        }
                        else if (e.Control && e.Shift && e.KeyCode == Keys.D)
                        {
                            if (btnAdd.Enabled)
                            {
                                btnAdd.Text = "&Save";
                                SetSerialNo();
                                txtDate.Focus();
                            }
                        }
                    }
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
            dba.GetDateInExactFormat(sender, false, true, true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtValidUptoDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, false, false);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }


        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private bool ValidateControls()
        {
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill No. can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtDate.TextLength != 10)
            {
                MessageBox.Show("Sorry ! Date can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtCustomerName.Text == "")
            {
                MessageBox.Show("Sorry ! Customer Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }
            if (txtBankGuaranteeNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bank Guarantee No. can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBankGuaranteeNo.Focus();
                return false;
            }
            if (txtAmount.Text == "")
            {
                MessageBox.Show("Sorry ! Amount can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return false;
            }
            if (txtBankName.Text == "")
            {
                MessageBox.Show("Sorry ! Bank can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBankName.Focus();
                return false;
            }
            if (txtValidUptoDate.TextLength != 10)
            {
                MessageBox.Show("Sorry ! Valid Upto Date can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValidUptoDate.Focus();
                return false;
            }

            return true;
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtCustomerName.Text = objSearch.strSelectedData;                           
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("BGBANKNAME", "SEARCH BANK NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtBankName.Text = objSearch.strSelectedData;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("BANKGUARANTEE", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtBankGuaranteeNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bCashAdd || MainPage.mymainObject.bCashEdit || MainPage.mymainObject.bCashView)
            {
                if (!MainPage.mymainObject.bCashAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bCashEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bCashView)
                    txtBillNo.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(Close));
                return false;
            }
        }

        private void txtBillNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindAllDetails(txtBillNo.Text);
                    }
                }
                else
                {
                    txtBillNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("BGBILLCODE", "SEARCH BILL CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtBillCode.Text = objSearch.strSelectedData;
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

        private void txtAmount_Leave(object sender, EventArgs e)
        {
            if(txtAmount.Text != "")
            txtAmount.Text = Convert.ToDouble(txtAmount.Text).ToString("N2", MainPage.indianCurancy);
        }
    }
}
