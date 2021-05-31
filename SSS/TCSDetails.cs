using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class TCSDetails : Form
    {
        DataBaseAccess dba;
        bool newStatus = false;
        string strLastSerialNo = "",strInvoiceType="";
        public TCSDetails(string strInType)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strInvoiceType = strInType;

            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }
        public TCSDetails(string strInType, string strBillCode, string strBillNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strInvoiceType = strInType;
            GetStartupData();
            if (strBillCode != "")
                txtBillCode.Text = strBillCode;

            BindRecordWithControl(strBillNo);
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = "";
                if (strInvoiceType == "DEBITNOTE")
                    strQuery = " Select TCSDNCode as BillCode,(Select ISNULL(MAX(BillNo),0) from TCSDetails Where BillCode=TCSDNCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                else
                    strQuery = " Select TCSCNCode as BillCode,(Select ISNULL(MAX(BillNo),0) from TCSDetails Where BillCode=TCSCNCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["BillCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from TCSDetails Where InvoiceType='"+strInvoiceType+"' and BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from TCSDetails Where InvoiceType='" + strInvoiceType + "' and BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from TCSDetails Where InvoiceType='" + strInvoiceType + "' and BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from TCSDetails Where InvoiceType='" + strInvoiceType + "'and BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo from [TCSDetails] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {                    
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SNo"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Set Bill No in TCS Details", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ClearAllText()
        {
            lblCreatedBy.Text = txtVoucherCode.Text = txtVoucherNo.Text = txtAccountID.Text = txtTCSAccount.Text = txtRemark.Text = "";
            txtAmount.Text = txtTCSPer.Text = txtTCSAmt.Text = "0.00";
            txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");
            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void EnableAllControls()
        {
            txtDate.ReadOnly = txtAmount.ReadOnly = txtRemark.ReadOnly =  false;
        }

        private void DisableAllControls()
        {
            txtDate.ReadOnly = txtAmount.ReadOnly = txtRemark.ReadOnly = true;
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                if (strSerialNo != "")
                {
                    DisableAllControls();

                    string strQuery = " Select *,Convert(varchar,Date,103)BDate,dbo.GetFullName(AccountID) PartyName,ISNULL(dbo.GetFullName(TCSAccountID),'') TCSParty  from TCSDetails SR Where InvoiceType='"+strInvoiceType+"' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;


                    pnlDeletionConfirmation.Visible = false;
                    txtReason.Text = "";
                    DataTable _dt = dba.GetDataTable(strQuery);

                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];

                        txtBillNo.Text = strSerialNo;
                        txtDate.Text = Convert.ToString(row["BDate"]);
                        txtVoucherCode.Text = Convert.ToString(row["VoucherCode"]);
                        txtVoucherNo.Text = Convert.ToString(row["VoucherNo"]);
                        txtAccountID.Text = Convert.ToString(row["PartyName"]);
                        txtTCSAccount.Text = Convert.ToString(row["TCSParty"]);
                        txtRemark.Text = Convert.ToString(row["Remark"]);
                        txtAmount.Text = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2",MainPage.indianCurancy);
                        txtTCSPer.Text = Convert.ToString(row["TCSPer"]);
                        txtTCSAmt.Text = Convert.ToString(row["TCSAmt"]);

                        string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                        if (strCreatedBy != "")
                            lblCreatedBy.Text = "Created By : " + strCreatedBy;
                        if (strUpdatedBy != "")
                            lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                    }
                    EditOption();
                }
            }
            catch
            {
            }
        }

        private void TCSDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
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
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.E)
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                }
            }
        }

        private void TCSDetails_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (EditOption())
                {
                    if (newStatus)
                    {
                        btnAdd.PerformClick();
                        txtBillNo.Focus();
                    }
                    if (strInvoiceType == "CREDITNOTE")
                        lblHeader.Text = "TCS DETAILS (CREDIT NOTE)";
                }
            }
            catch
            {
            }
        }

        private bool EditOption()
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
                this.Close();
                return false;
            }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    double dAmt = dba.ConvertObjectToDouble(txtAmount.Text),dTCSPer=dba.ConvertObjectToDouble(txtTCSPer.Text);
                    double dTcsAmt = ((dAmt * dTCSPer) / 100);
                    dTcsAmt = Convert.ToDouble(dTcsAmt.ToString("0"));
                    txtTCSAmt.Text = dTcsAmt.ToString("N2", MainPage.indianCurancy);
                }
            }catch { }
        }

        private void txtAccountID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strType = "SALESPARTY";
                        if (strInvoiceType == "CREDITNOTE")
                            strType = "PURCHASEPARTY";

                        SearchData objSearch = new SearchData(strType, "SEARCH PARTY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            txtAccountID.Text = strData;
                            if (dba.CheckTransactionLock(txtAccountID.Text))
                            {
                                MessageBox.Show("Transaction has been locked on this party ! Please select different Sundry Debtors ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtAccountID.Text = "";
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTCSAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strType = "TCSDNACCOUNT";
                        if (strInvoiceType == "CREDITNOTE")
                            strType = "TCSCNACCOUNT";

                        SearchData objSearch = new SearchData(strType, "SEARCH TCS ACCOUNT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            txtTCSAccount.Text = strData;
                            if (dba.CheckTransactionLock(txtTCSAccount.Text))
                            {
                                MessageBox.Show("Transaction has been locked on this party ! Please select different Sundry Debtors ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtTCSAccount.Text = "";
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtVoucherCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strType = "DNBANKVCODE";
                        if (strInvoiceType == "CREDITNOTE")
                            strType = "CNBANKVCODE";

                        SearchData objSearch = new SearchData(strType, "SEARCH BANK VOUCHER CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            string[] str = strData.Split('|');
                            if (str.Length > 2)
                            {
                                string[] _str = str[0].Split(' ');
                                if (_str.Length > 1)
                                {
                                    txtVoucherCode.Text = _str[0];
                                    txtVoucherNo.Text = _str[1];
                                }
                                txtAccountID.Text = str[1];
                                txtAmount.Text = str[2];
                            }
                        }
                    }
                }
                e.Handled = true;
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
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }           
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtVoucherCode.Text=="")
            {
                MessageBox.Show("Sorry ! Voucher Code can't be blank !!", "Voucher Code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtVoucherNo.Text == "")
            {
                MessageBox.Show("Sorry ! Voucher no can't be blank !!", "Voucher no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                return false;
            }
            if (txtAccountID.Text == "")
            {
                MessageBox.Show("Sorry ! Account name can't be blank !!", "Account name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccountID.Focus();
                return false;
            }
            if (txtTCSAccount.Text.Length == 10)
            {
                MessageBox.Show("Sorry ! TCS Account can't be blank  !!", "TCS Account name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTCSAccount.Focus();
                return false;
            }
            double dAmt = dba.ConvertObjectToDouble(txtAmount.Text), dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
            if (dAmt == 0)
            {
                MessageBox.Show("Sorry ! Amount can't be blank !!", "Amount required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return false;
            }
            if (dTCSAmt == 0)
            {
                MessageBox.Show("Sorry ! TCS amt can't be blank !!", "TCS Amount required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTCSAmt.Focus();
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();

                    txtDate.Focus();
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strPartyName = "", strAccountID = "", strTCSAccountID = "", strTCSAccountName = "";
                string[] strFullName = txtAccountID.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strAccountID = strFullName[0].Trim();
                    strPartyName = txtAccountID.Text.Replace(strAccountID + " ", "");
                }
                strFullName = txtTCSAccount.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strTCSAccountID = strFullName[0].Trim();
                    strTCSAccountName = txtTCSAccount.Text.Replace(strTCSAccountID + " ", "");
                }


                double dAmt = dba.ConvertObjectToDouble(txtAmount.Text), dTCSPer = dba.ConvertObjectToDouble(txtTCSPer.Text), dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                string strQuery = "", strAccountStatus = "TCS DEBIT NOTE",strPStatus="DEBIT",strTStatus= "CREDIT";
                if (strInvoiceType == "CREDITNOTE")
                {
                    strAccountStatus = "TCS CREDIT NOTE";
                    strPStatus = "CREDIT";
                    strTStatus = "DEBIT";
                }

                strQuery += " Declare @BillNo bigint; Select @BillNo=(ISNULL(MAX(BillNo),0)+1) from TCSDetails Where BillCode='" + txtBillCode.Text + "' "
                         + " if not exists(Select [BillCode] from [dbo].[TCSDetails] Where (([BillCode]='" + txtBillCode.Text + "' and BIllNo=@BillNo) OR ([VoucherCode]='"+txtVoucherCode.Text+"' and [VoucherNo]="+txtVoucherNo.Text+"))) begin "
                         + " INSERT INTO [dbo].[TCSDetails]([BillCode],[BillNo],[Date],[AccountID],[TCSAccountID],[VoucherCode],[VoucherNo],[Amount],[TCSPer],[TCSAmt],[Remark],[InvoiceType],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) OUTPUT INSERTED.BillNo Values "
                         + " ('" + txtBillCode.Text + "',@BillNo,'" + strDate + "','" + strAccountID + "','" + strTCSAccountID + "','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + "," + dAmt + "," + dTCSPer + "," + dTCSAmt + ",'" + txtRemark.Text + "','" + strInvoiceType + "','','" + MainPage.strLoginName + "','',1,0) "
                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                         + " ('" + strDate + "','" + strPartyName + "','"+ strAccountStatus+"','"+ strPStatus+"','" + txtBillCode.Text + " '+CAST(@BillNo as varchar)," + dTCSAmt + ",'CR','0','0','False','',0,'"+MainPage.strLoginName+"','',0,0,0,'" + strAccountID + "') "
                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                         + " ('" + strDate + "','" + strTCSAccountName + "','"+ strAccountStatus+ "','" + strTStatus + "','" + txtBillCode.Text + " '+CAST(@BillNo as varchar)," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strTCSAccountID + "')  "
                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('TCS" + strInvoiceType + "','" + txtBillCode.Text + "',@BillNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTCSAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                string strBillNo = Convert.ToString(objValue);
                //int count = dba.ExecuteMyQuery(strQuery);
                if (strBillNo!="")
                {
                    txtBillNo.Text = strBillNo;
                    SendEmailWhastapp();
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    ClearAllText();
                    BindRecordWithControl(strBillNo);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in TCS Details", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SendEmailWhastapp()
        {
            if(chkEmail.Checked || chkWhatsapp.Checked || chkSendSMS.Checked)
            {
                dba.SendTCSEmailWhatsapp(strInvoiceType, txtBillCode.Text, txtBillNo.Text, true, chkEmail.Checked, chkWhatsapp.Checked, chkSendSMS.Checked);
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
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        BindLastRecord();
                        btnAdd.Text = "&Add";
                    }
                    if (btnEdit.Enabled)
                    {
                        btnEdit.Text = "&Update";
                        EnableAllControls();
                        txtBillNo.ReadOnly = true;
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
            catch
            {
            }
            btnEdit.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
        }

        private void UpdateRecord()
        {
            try
            {
                string strDate = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");

                string strPartyName = "", strAccountID = "", strTCSAccountID = "", strTCSAccountName = "";
                string[] strFullName = txtAccountID.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strAccountID = strFullName[0].Trim();
                    strPartyName = txtAccountID.Text.Replace(strAccountID + " ", "");
                }
                strFullName = txtTCSAccount.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strTCSAccountID = strFullName[0].Trim();
                    strTCSAccountName = txtTCSAccount.Text.Replace(strTCSAccountID + " ", "");
                }


                double dAmt = dba.ConvertObjectToDouble(txtAmount.Text), dTCSPer = dba.ConvertObjectToDouble(txtTCSPer.Text), dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                string strQuery = "", strAccountStatus = "TCS DEBIT NOTE", strPStatus = "DEBIT", strTStatus = "CREDIT";
                if (strInvoiceType == "CREDITNOTE")
                {
                    strAccountStatus = "TCS CREDIT NOTE";
                    strPStatus = "CREDIT";
                    strTStatus = "DEBIT";
                }

                strQuery += " if exists(Select [BillCode] from [dbo].[TCSDetails] Where [BillCode]='" + txtBillCode.Text + "' and BIllNo=" + txtBillNo.Text + ") begin "
                         + " UPDATE [dbo].[TCSDetails] SET [Date]='" + strDate + "',[AccountID]='" + strAccountID + "',[TCSAccountID]='" + strTCSAccountID + "',[VoucherCode]='" + txtVoucherCode.Text + "',[VoucherNo]=" + txtVoucherNo.Text + ",[Amount]=" + dAmt + ",[TCSPer]=" + dTCSPer + ",[TCSAmt]=" + dTCSAmt + ",[Remark]='" + txtRemark.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " UPDATE [dbo].[BalanceAmount]  Set [Date]='" + strDate + "',[PartyName]='" + strPartyName + "',[Amount]=" + dTCSAmt + ",[AccountID]='" + strAccountID + "' Where [AccountStatus]='" + strAccountStatus + "' and [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [Status]='"+ strPStatus+"'  "
                         + " UPDATE [dbo].[BalanceAmount]  Set [Date]='" + strDate + "',[PartyName]='" + strTCSAccountName + "',[Amount]=" + dTCSAmt + ",[AccountID]='" + strTCSAccountID + "' Where [AccountStatus]='" + strAccountStatus + "' and [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [Status]='" + strTStatus + "'  "
                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('TCS" + strInvoiceType + "','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTCSAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    SendEmailWhastapp();
                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in updating Record in TCS Details", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
            {
                pnlDeletionConfirmation.Visible = true;
                txtReason.Focus();
            }
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TCSDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("TCS" + strInvoiceType, txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }
               
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
               
                btnPreview.Enabled = false;
                string strEmailID = "", strMobileNo = "", strWhatsappNo = "", strAmount="";
                DataTable dt = dba.CreateDataTable_TCS(strInvoiceType, txtBillCode.Text, txtBillNo.Text, ref strEmailID, ref strMobileNo, ref strWhatsappNo,ref strAmount);

                if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    Reporting.CryTCSDetailNote objReport = new Reporting.CryTCSDetailNote();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("TCS Details");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();

                    objReport.Close();
                    objReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
            { }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                string strEmailID = "", strMobileNo = "", strWhatsappNo = "", strAmount="";
                DataTable dt = dba.CreateDataTable_TCS(strInvoiceType, txtBillCode.Text, txtBillNo.Text, ref strEmailID, ref strMobileNo, ref strWhatsappNo, ref strAmount);

                if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    Reporting.CryTCSDetailNote objReport = new Reporting.CryTCSDetailNote();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                    {
                        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;
                        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        objReport.PrintToPrinter(1, false, 0, 0);
                    }
                    btnPreview.Enabled = true;
                    objReport.Close();
                    objReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnPrint.Enabled = true;
            }
            catch
            {
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            CreatePDF();
            btnExport.Enabled = true;
        }

        private void CreatePDF()
        {
            try
            {
               
                string strFileName = "", strPath = "";
                strFileName = txtBillCode.Text + "_" + txtBillNo.Text;

                SaveFileDialog _browser = new SaveFileDialog();
                _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                _browser.FileName = strFileName + ".pdf";
                _browser.ShowDialog();

                if (_browser.FileName != "")
                    strPath = _browser.FileName;

                strPath = ExportPDFFile(strPath);
                if (strPath != "")
                    MessageBox.Show("Thanks ! File exported successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

         

        private string ExportPDFFile(string strPath)
        {
            string strEmailID = "", strMobileNo = "", strWhatsappNo = "", strAmount="";
            DataTable dt = dba.CreateDataTable_TCS(strInvoiceType, txtBillCode.Text, txtBillNo.Text, ref strEmailID,ref  strMobileNo, ref strWhatsappNo, ref strAmount);
            if (dt.Rows.Count > 0)
            {              
                if (dt.Rows.Count > 0)
                {
                    Reporting.CryTCSDetailNote report = new SSS.Reporting.CryTCSDetailNote();
                    report.SetDataSource(dt);                   

                    if (strPath != "" && strPath.Contains("\\"))
                    {
                        report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                    }
                    report.Close();
                    report.Dispose();
                }
            }
            else
            {
                strPath = "";
               // MessageBox.Show("There is no record for Exporting ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return strPath;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            chkWhatsapp.Checked = chkEmail.Checked = chkSendSMS.Checked = chkAll.Checked;
        }

        private void btnSendEmailAndWhatsapp_Click(object sender, EventArgs e)
        {
            try
            {
                if(chkEmail.Checked || chkWhatsapp.Checked || chkSendSMS.Checked)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to send ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SendEmailWhastapp();
                    }
                }
            }
            catch { }
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && dba.ValidateBackDateEntry(txtDate.Text))
                {
                    if (txtReason.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strQuery = "", strAccountStatus = "TCS DEBIT NOTE";
                            if (strInvoiceType == "CREDITNOTE")
                                strAccountStatus = "TCS CREDIT NOTE";

                            strQuery += "DELETE [dbo].[TCSDetails] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " DELETE [dbo].[BalanceAmount] Where [AccountStatus]='" + strAccountStatus + "' and [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('TCS" + strInvoiceType + "','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtTCSAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from TCSDetails Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {   
                                if (!Convert.ToBoolean(objStatus))
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                pnlDeletionConfirmation.Visible = false;
                                MessageBox.Show("Thank You ! Record deleted successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindNextRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReason.Focus();
                    }
                }
            }
            catch
            {
            }
        }

       
    }
}
