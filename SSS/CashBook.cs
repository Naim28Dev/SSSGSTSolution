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
    public partial class CashBook : Form
    {
        DataBaseAccess dba;
        ChangeCurrencyToWord objCurrency;
        string strOldPartyName = "";
        int _cashStatus = 0;
        double dOldAmount = 0;
        SearchData _objSearch;
        public CashBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetCashVoucherCode();
            objCurrency = new ChangeCurrencyToWord();
            BindLastRecord();
        }

        public CashBook(int _status)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetCashVoucherCode();
            objCurrency = new ChangeCurrencyToWord();
            _cashStatus = _status;          
        }

        public CashBook(string strCode, string strSerial)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            if (strCode == "")
                GetCashVoucherCode();
            else
                txtVoucherCode.Text = strCode;

            objCurrency = new ChangeCurrencyToWord();
            BindRecordWithControl(strSerial);
        }

        private void CashBook_KeyDown(object sender, KeyEventArgs e)
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
                else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                    SendKeys.Send("{TAB}");
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
                                EnableAllControls();
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

        private void GetCashVoucherCode()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select CashVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ");
            txtVoucherCode.Text = Convert.ToString(objValue);
            if (txtVoucherCode.Text == "" || txtVoucherCode.Text == "0")
            {
                MessageBox.Show("Sorry ! Please enter cash voucher code in company setting !", "Waring", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = btnDelete.Enabled = false;
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(VoucherNo),'') from BalanceAmount Where VoucherCode='"+txtVoucherCode.Text+"' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(VoucherNo),'') from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(VoucherNo),'') from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo>" + txtVoucherNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
            {
                BindLastRecord();
            }
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(VoucherNo),'') from  BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo<" + txtVoucherNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                ClearAllText();
                string strQuery = "Select Top 1 *,Convert(varchar,Date,103) SDate,dbo.GetFullName(AccountID)NPartyName,(AccountStatusID+' '+SM.Name)NAccountStatus,GroupName,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,BA.Date))) LockType,(Select TOP 1 BillNo from TCSDetails TCS Where TCS.VoucherCode=BA.VoucherCode and TCS.VoucherNo=BA.VoucherNo)TCS from BalanceAmount BA OUTER APPLY (Select SM.Name,SM.TinNumber as PartyType,GroupName from SupplierMaster SM Where AccountStatusID=(SM.AreaCode+SM.AccountNo)) SM Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + strSerialNo + " and AccountID in (Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')) from SupplierMaster Where GroupName='CASH A/C') ";
                DataTable dt = dba.GetDataTable(strQuery);
                DisableAllControls();
                dgrdDetails.Rows.Clear();
                if (dt != null && dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add();
                    DataRow dRow = dt.Rows[0];
                    txtVoucherNo.Text = strSerialNo;
                    txtDate.Text = Convert.ToString(dRow["SDate"]);
                    txtCashAccount.Text = Convert.ToString(dRow["NPartyName"]);
                    txtGSTNature.Text = Convert.ToString(dt.Rows[0]["GSTNature"]);
                    if (Convert.ToString(dRow["Status"]).ToUpper() == "DEBIT")
                        rdoReceipt.Checked = true;
                    else
                        rdoPayment.Checked = true;

                    dOldAmount = Convert.ToDouble(dRow["Amount"]);

                    dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
                    dgrdDetails.Rows[0].Cells["accountName"].Value = strOldPartyName=Convert.ToString(dRow["NAccountStatus"]);
                    dgrdDetails.Rows[0].Cells["particular"].Value = dRow["Description"];
                    dgrdDetails.Rows[0].Cells["amount"].Value = lblTotalAmt.Text= dOldAmount.ToString("N2",MainPage.indianCurancy);
                    dgrdDetails.Rows[0].Cells["groupName"].Value = dRow["groupName"];

                    //if (Convert.ToString(dRow["LockType"]) == "LOCK" && MainPage.strUserRole != "SUPERADMIN" && MainPage.strUserRole != "ADMIN")
                    //    btnEdit.Enabled = btnDelete.Enabled = false;
                    //else
                    {
                        if (!MainPage.mymainObject.bCashEdit)
                            btnEdit.Enabled = btnDelete.Enabled = false;
                        else
                            btnEdit.Enabled = btnDelete.Enabled = true;
                    }
                    if(Convert.ToString(dRow["TCS"])!="")                    
                        btnGenerateTCS.BackColor = Color.LightGreen;
                    else
                        btnGenerateTCS.BackColor = Color.FromArgb(185, 30, 12);

                    string strCreatedBy = Convert.ToString(dt.Rows[0]["UserName"]), strUpdatedBy = Convert.ToString(dt.Rows[0]["UpdatedBy"]);

                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                    GetCashBalance();

                    if (strOldPartyName != "")
                    {
                        double dLedgerAmt = dba.GetPartyAmountFromQuery(strOldPartyName);
                        if (dLedgerAmt > 0)
                            lblLedgerBal.Text = dLedgerAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dLedgerAmt < 0)
                            lblLedgerBal.Text = Math.Abs(dLedgerAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            lblLedgerBal.Text = dLedgerAmt.ToString("0.00");
                    }
                    else
                        lblLedgerBal.Text = "0.00";
                }
                txtVoucherNo.ReadOnly = false;
            }
            catch
            {
            }
        }

        private void EnableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = true;
        }

        private void ClearAllText()
        {
            strOldPartyName = "";
            txtCashAccount.Text =lblCreatedBy.Text= "";
            lblCashBalance.Text = lblTotalAmt.Text = lblLedgerBal.Text = "0.00";
            txtGSTNature.Text = "NOT APPLICABLE/NON-GST";
            rdoReceipt.Checked = true;
            chkSendSMS.Checked = false;
            dOldAmount = 0;
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
            pnlDeletionConfirmation.Visible = false;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                    e.Cancel = true;
                else if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1)
                    {
                        _objSearch = new SearchData("ALLPARTY", "Search Account Name", Keys.Space);
                        _objSearch.ShowDialog();
                        if (_objSearch.strSearchData != "")
                        {
                            if (txtCashAccount.Text != _objSearch.strSelectedData)
                            {
                                dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                                string strPartyType = "", strGroupName = "";
                                if (dba.CheckTransactionLockWithPartyType(_objSearch.strSelectedData, ref strPartyType, ref strGroupName))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdDetails.CurrentCell.Value = "";
                                }
                                else
                                {
                                    if (_objSearch.strSelectedData != "")
                                    {
                                        double dLedgerAmt = dba.GetPartyAmountFromQuery(_objSearch.strSelectedData);
                                        if (dLedgerAmt > 0)
                                            lblLedgerBal.Text = dLedgerAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                                        else if (dLedgerAmt < 0)
                                            lblLedgerBal.Text = Math.Abs(dLedgerAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                                        else
                                            lblLedgerBal.Text = dLedgerAmt.ToString("0.00");
                                    }
                                    else
                                        lblLedgerBal.Text = "0.00";

                                    dgrdDetails.CurrentRow.Cells["groupName"].Value = strGroupName;                                   
                                }
                            }
                            else
                                MessageBox.Show("Sorry ! Both account name can't be same ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        e.Cancel = true;
                    }
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
                CalculateAllAmount();
        }

        private void CalculateAllAmount()
        {
            try
            {
                double dAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dAmt += dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                }
                lblTotalAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                lblCurrentAmount.Text = "";
            }
            catch
            {
            }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex == 2 || dgrdDetails.CurrentCell.ColumnIndex == 3)
                    {
                        TextBox txtBox = (TextBox)e.Control;
                        txtBox.CharacterCasing = CharacterCasing.Upper;
                        txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                        txtBox.TextChanged += new EventHandler(txtBox_TextChanged);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 2)
                dba.ValidateSpace(sender, e);
            else if (dgrdDetails.CurrentCell.ColumnIndex == 3)
            {
                if (MainPage.strSoftwareType == "AGENT")
                    dba.KeyHandlerPoint(sender, e, 0);
                else
                    dba.KeyHandlerPoint(sender, e, 2);
            }
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 3)
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text != "")
                    lblCurrentAmount.Text = objCurrency.changeCurrencyToWords(dba.ConvertObjectToDouble(txt.Text));
                else
                    lblCurrentAmount.Text = "";
            }
        }

        private void txtCash_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CASHPARTY", "SEARCH CASH A/C", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtCashAccount.Text = objSearch.strSelectedData;
                            if (dba.CheckTransactionLock(txtCashAccount.Text))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtCashAccount.Text = "";
                            }
                            else
                                GetCashBalance();
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

        private bool ValidateAllControl()
        {
            if (txtVoucherCode.Text == "")
            {
                MessageBox.Show("Sorry ! Voucher code can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVoucherCode.Focus();
                return false;
            }
            if (txtVoucherNo.Text == "")
            {
                MessageBox.Show("Sorry ! Voucher no can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                return false;
            }           
            if (txtDate.Text.Length!=10)
            {
                MessageBox.Show("Sorry ! Please enter valid date.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtCashAccount.Text == "")
            {
                MessageBox.Show("Sorry ! Cash Account can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCashAccount.Focus();
                return false;
            }
            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;
            //if (!MainPage.mymainObject.bBackDayEntry)
            //{
            //    if (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) < MainPage.currentDate)
            //    {
            //        MessageBox.Show("Back Date Entry is not Allowed in your Login ? Please Contact to Administrator ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        txtDate.Focus();
            //        return false;
            //    }
            //}

            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                string strName = Convert.ToString(row.Cells["accountName"].Value);
                double dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                if (strName == "" && dAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else if(strName=="")
                {
                    MessageBox.Show("Sorry ! Account name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["accountName"];
                    dgrdDetails.Focus();
                    return false;
                }
                else if (dAmt==0)
                {
                    MessageBox.Show("Sorry ! Amount can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["amount"];
                    dgrdDetails.Focus();
                    return false;
                }
                else if(MainPage._bTaxStatus && MainPage.mymainObject.bCreditLimitmanagement)
                {
                    bool __bStatus = ValidateCashLimit(strName, dAmt);
                    if (!__bStatus)
                        return __bStatus;

                }
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Please enter atleast one entry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.Rows.Add();
                dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["accountName"];
                return false;
            }

            return true;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
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
                            Index = dgrdDetails.CurrentCell.RowIndex;
                            IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                            if (Index < dgrdDetails.RowCount - 1)
                            {
                                CurrentRow = Index - 1;
                            }
                            else
                            {
                                CurrentRow = Index;
                            }
                            if (IndexColmn < dgrdDetails.ColumnCount - 3)
                            {
                                IndexColmn += 1;                              
                                if (CurrentRow >= 0)
                                {                                  
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                                }
                            }
                            else if (Index == dgrdDetails.RowCount - 1)
                            {
                                string strAccountName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["accountName"].Value), strAmt = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                                if (strAccountName != "" && strAmt != "" && btnAdd.Text=="&Save")
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["SrNo"].Value = dgrdDetails.Rows.Count;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["accountName"];
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
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["srNo"];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                        CalculateAllAmount();
                    }                    
                    else if (e.KeyCode == Keys.Delete)
                    {                       
                            int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                            if (colIndex ==1 || colIndex == 2)
                                dgrdDetails.CurrentCell.Value = "";
                            CalculateAllAmount();
                    }
                    else if (e.KeyValue == 96)
                        e.Handled = true;
                }
            }
            catch { }
        }       

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells["SrNo"].Value = serialNo;
                serialNo++;
            }
        }

        private void SetSerialNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(VoucherNo),0)+1)VoucherNo from BalanceAmount Where VoucherCode='"+txtVoucherCode.Text+"' ");
            txtVoucherNo.Text = Convert.ToString(objValue);
        }

        private void btnAdd_Click(object sender, EventArgs e)
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
                EnableAllControls();                
                ClearAllText();
                chkSendSMS.Checked = true;
                SetSerialNo();
                txtDate.Focus();
                if (!MainPage.mymainObject.bCashEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                    btnEdit.Enabled = btnDelete.Enabled = true;
            }
            else if (ValidateAllControl())
            {               
                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
        }

        private void SaveRecord()
        {
            try
            {
                string[] strFullParty = txtCashAccount.Text.Split(' ');
                if (strFullParty.Length > 1)
                {
                    string strQuery = " Declare @SerialNo int ;", strGroupName = "";//,@BillCode nvarchar(250),@BillNo bigint, @TCSAccount nvarchar(250), @TCSPer numeric(18,4),@TCSAmt numeric(18,2), @Amt numeric(18,2),@NetAmt numeric(18,2); ";
                    string strCashAccount ="", strSecondParty = "", strDate = "", strAccountID = "", strAccountStatusID = "", strDescription="";
                    strAccountID = strFullParty[0];
                    strCashAccount = txtCashAccount.Text.Replace(strAccountID + " ", "");

                    DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");
                    double dAmt = 0;
                    int _chqStatus = 0;
                    bool _bSDebtorStatus = false;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                        strSecondParty = Convert.ToString(row.Cells["accountName"].Value);
                        strDescription = Convert.ToString(row.Cells["particular"].Value);
                        strGroupName = Convert.ToString(row.Cells["groupName"].Value);

                        strFullParty = strSecondParty.Split(' ');
                         if (strFullParty.Length > 1)
                         {
                             strAccountStatusID = strFullParty[0];
                             strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");

                             strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' ";
                             if (rdoReceipt.Checked)
                             {
                                 strQuery += " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature]) VALUES "
                                               + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strCashAccount + "','" + strSecondParty + "','DEBIT','" + row.Cells["particular"].Value + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "'," + _chqStatus + ",'" + txtGSTNature.Text + "') "
                                               + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature]) VALUES "
                                               + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strSecondParty + "','" + strCashAccount + "','CREDIT','" + row.Cells["particular"].Value + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountStatusID + "','" + strAccountID + "'," + _chqStatus + ",'" + txtGSTNature.Text + "') ";
                             }
                             else
                             {
                                 strQuery += " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature]) VALUES "
                                               + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strCashAccount + "','" + strSecondParty + "','CREDIT','" + row.Cells["particular"].Value + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "'," + _chqStatus + ",'" + txtGSTNature.Text + "') "
                                               + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature]) VALUES "
                                               + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strSecondParty + "','" + strCashAccount + "','DEBIT','" + row.Cells["particular"].Value + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountStatusID + "','" + strAccountID + "'," + _chqStatus + ",'" + txtGSTNature.Text + "') ";
                             }

                            strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                     + "('CASH','" + txtVoucherCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                            if (strGroupName == "SUNDRY DEBTORS" && rdoReceipt.Checked)
                                _bSDebtorStatus = false;

                        }
                    }

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        SendSMSToParty();
                        if (_bSDebtorStatus)
                        {
                            double dVNo = dba.ConvertObjectToDouble(txtVoucherNo.Text);
                            count = dba.SaveTCSDetails(txtVoucherCode.Text, dVNo, dgrdDetails.RowCount);
                        }

                        MessageBox.Show("Thank you ! Record saved successfully ", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        BindLastRecord();
                        AskForPrint();
                    }
                    else
                        MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
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
                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    pnlDeletionConfirmation.Visible = false;
                    if (btnEdit.Enabled)
                    {
                        EnableAllControls();
                        chkSendSMS.Checked = true;
                        btnEdit.Text = "&Update";
                        txtDate.Focus();
                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateAllControl() && ValidateControlONEditDelete(false))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                string[] strFullParty = txtCashAccount.Text.Split(' ');
                if (strFullParty.Length > 1)
                {
                    string strQuery = "", strGroupName = "";// " Declare @BillCode nvarchar(250),@BillNo bigint, @TCSAccount nvarchar(250), @TCSPer numeric(18,4),@TCSAmt numeric(18,2), @Amt numeric(18,2),@NetAmt numeric(18,2); ", strGroupName = "";

                    string strCashAccount = "", strSecondParty = "", strDate = "", strAccountID = "", strAccountStatusID = "";
                    strAccountID = strFullParty[0];
                    strCashAccount = txtCashAccount.Text.Replace(strAccountID + " ", "");

                    DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");

                    double dAmt = 0;
                    bool _bSDebtorStatus = false;

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSecondParty = Convert.ToString(row.Cells["accountName"].Value);
                        strGroupName = Convert.ToString(row.Cells["groupName"].Value);

                        strFullParty = strSecondParty.Split(' ');
                        if (strFullParty.Length > 1)
                        {
                            strAccountStatusID = strFullParty[0];
                            strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");

                            dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                            if (rdoReceipt.Checked)
                            {
                                strQuery += " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strCashAccount + "',[AccountStatus]='" + strSecondParty + "',[AccountID]='" + strAccountID + "',[AccountStatusID]='" + strAccountStatusID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[GSTNature]='" + txtGSTNature.Text + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' "
                                         + " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strSecondParty + "',[AccountStatus]='" + strCashAccount + "',[AccountID]='" + strAccountStatusID + "',[AccountStatusID]='" + strAccountID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[GSTNature]='" + txtGSTNature.Text + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' ";
                            }
                            else
                            {
                                strQuery += " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strCashAccount + "',[AccountStatus]='" + strSecondParty + "',[AccountID]='" + strAccountID + "',[AccountStatusID]='" + strAccountStatusID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[GSTNature]='" + txtGSTNature.Text + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' "
                                         + " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strSecondParty + "',[AccountStatus]='" + strCashAccount + "',[AccountID]='" + strAccountStatusID + "',[AccountStatusID]='" + strAccountID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[GSTNature]='" + txtGSTNature.Text + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' ";
                            }

                            if (strGroupName == "SUNDRY DEBTORS" && rdoReceipt.Checked)
                                _bSDebtorStatus = true;// strQuery += dba.GetTCSquery(strAccountStatusID, dAmt, txtVoucherCode.Text, txtVoucherNo.Text, strDate, false);

                        }
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                              + "('CASH','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblTotalAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        SendSMSToPartyForUpdate();
                        if (_bSDebtorStatus && sDate >= Convert.ToDateTime("10/01/2020"))
                        {
                            double dVNo = dba.ConvertObjectToDouble(txtVoucherNo.Text);
                            count = dba.SaveTCSDetails(txtVoucherCode.Text, dVNo, dgrdDetails.RowCount);
                        }
                        MessageBox.Show("Thank you ! Record updated successfully ", "Record Updated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnEdit.Text = "&Edit";
                        BindRecordWithControl(txtVoucherNo.Text);
                        AskForPrint();
                    }
                    else
                        MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
            }
        }

        private bool ValidateCashLimit(string strParty, double dAmt)
        {
            try
            {
                double dDebitAmt = 0, dCreditAmt = 0;
                string strStatus = "DEBIT", strPartyCode = "";
                if (rdoReceipt.Checked)
                    strStatus = "CREDIT";
                //foreach (DataGridViewRow row in dgrdDetails.Rows)
                //{
                //    dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                //    strParty = Convert.ToString(row.Cells["accountName"].Value);

                string[] str = strParty.Split(' ');
                if (str.Length > 1)
                    strPartyCode = str[0];

                string strQuery = " Select SUM(ISNULL((CASE WHEN BA.Status='DEBIT' Then (CAST(Amount as Money)) end),0)) DebitAmt, SUM(ISNULL((CASE WHEN BA.Status='CREDIT' Then (CAST(Amount as Money)) end),0)) CreditAmt from BalanceAmount BA  CROSS APPLY (Select TOP 1 Name from SupplierMaster SM Where GroupName in ('SUNDRY DEBTORS','SUNDRY CREDITOR') and SM.AreaCode+SM.AccountNo= AccountID)SM CROSS APPLY(Select CashVCode from CompanySetting CS Where BA.VoucherCode=CS.CashVCode)_CS Where BA.AccountID='" + strPartyCode + "' and (VoucherCode+' '+CAST(VoucherNo as varchar)) !='" + txtVoucherCode.Text + " " + txtVoucherNo.Text + "'  ";

                DataTable dt = null;
                if(MainPage.strOnlineDataBaseName!="")
                dt =NetDBAccess.GetDataTableRecord(strQuery);
                else
                    dt = dba.GetDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    dDebitAmt = dba.ConvertObjectToDouble(dt.Rows[0]["DebitAmt"]);
                    dCreditAmt = dba.ConvertObjectToDouble(dt.Rows[0]["CreditAmt"]);
                    strQuery += " and InsertStatus=1 ";

                    DataTable _dt = dba.GetDataTable(strQuery);
                    if(_dt.Rows.Count>0)
                    {
                        dDebitAmt += dba.ConvertObjectToDouble(_dt.Rows[0]["DebitAmt"]);
                        dCreditAmt += dba.ConvertObjectToDouble(_dt.Rows[0]["CreditAmt"]);
                    }
                    if (dDebitAmt != 0 || dCreditAmt != 0)
                    {
                        if (strStatus == "DEBIT")
                            dDebitAmt += dAmt;
                        else
                            dCreditAmt += dAmt;

                        if (dDebitAmt > 200000 && (dDebitAmt - dAmt) != 0)
                        {
                            MessageBox.Show("Sorry ! We have already paid " + (dDebitAmt - dAmt).ToString("N2", MainPage.indianCurancy) + " amount to party name : " + strParty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dCreditAmt > 200000 && (dCreditAmt - dAmt) != 0)
                        {
                            MessageBox.Show("Sorry ! We have already received " + (dCreditAmt - dAmt).ToString("N2", MainPage.indianCurancy) + " amount from party name : " + strParty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
                //}
            }
            catch { return false; }
            return true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private bool ValidateInsertStatus()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " ");
            return Convert.ToBoolean(objValue);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            
            BindLastRecord();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    Reporting.CashReceiptReport report = new Reporting.CashReceiptReport();
                    report.SetDataSource(dt);
                    Reporting.ShowReport objReport = new Reporting.ShowReport("CASH RECEIPT PREVIEW");
                    objReport.myPreview.ReportSource = report;
                    objReport.ShowDialog();

                    report.Close();
                    report.Dispose();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }


        private void AskForPrint()
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you want to print office receipt ?", "Print Cash Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    if (dgrdDetails.Rows.Count > 0)
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Copies = (short)MainPage.iNCopyCash;
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;

                        DataTable dt = CreateOfficeDataTable();
                        Reporting.CashReceiptReport report = new Reporting.CashReceiptReport();
                        report.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(report,false,MainPage.iNCopyCash);
                        else
                        {
                            report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                            report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        }
                        result = MessageBox.Show("Are you also want to print cash receipt ?", "Print Cash Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                        {
                            dt.Clear();
                            dt = CreateDataTable();
                            report.SetDataSource(dt);
                            if (MainPage._PrintWithDialog)
                                dba.PrintWithDialog(report,true, MainPage.iNCopyCash);
                            else
                            {
                                report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                            }
                            report.Close();
                            report.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmailID", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("VoucherNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("CashAccount", typeof(String));
                myDataTable.Columns.Add("CashStatus", typeof(String));
                myDataTable.Columns.Add("CastType", typeof(String));
                myDataTable.Columns.Add("AccountName", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("AmountinWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = "FOR " + MainPage.strPrintComapanyName;
                row["VoucherNo"] = txtVoucherCode.Text+" "+ txtVoucherNo.Text;
                row["Date"] = txtDate.Text;
                
                if (rdoReceipt.Checked)
                {
                    row["CashAccount"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["AccountName"] = txtCashAccount.Text;
                    row["CashStatus"] = "Receipt Voucher";
                }
                else if (rdoPayment.Checked)
                {
                    row["CashAccount"] = txtCashAccount.Text;
                    row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["CashStatus"] = "Payment Voucher";
                }
                row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                row["Description"] = dgrdDetails.Rows[0].Cells["particular"].Value;
                double dAmount = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["amount"].Value);
                row["Amount"] = dAmount.ToString("N2", MainPage.indianCurancy);
                row["AmountinWord"] = objCurrency.changeCurrencyToWords(dAmount);

                row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where CashVCode='" + txtVoucherCode.Text + "' Order by CD.ID asc ");
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];
                    row["CompanyAddress"] = _row["CompanyAddress"];
                    row["CompanyEmailID"] = _row["CompanyPhoneNo"];
                    row["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                    row["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];
                }
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private DataTable CreateOfficeDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmailID", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("VoucherNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("CashAccount", typeof(String));
                myDataTable.Columns.Add("CashStatus", typeof(String));
                myDataTable.Columns.Add("CastType", typeof(String));
                myDataTable.Columns.Add("AccountName", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("AmountinWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = "FOR "+ MainPage.strPrintComapanyName;
                row["VoucherNo"] = txtVoucherCode.Text + " " + txtVoucherNo.Text;
                row["Date"] = txtDate.Text;
                //row["CastType"] = "OFFICE COPY";
                if (rdoReceipt.Checked)
                {
                    row["CashAccount"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["AccountName"] = txtCashAccount.Text;
                    row["CashStatus"] = "Receipt Voucher";
                }
                else if (rdoPayment.Checked)
                {
                    row["CashAccount"] = txtCashAccount.Text;
                    row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["CashStatus"] = "Payment Voucher";
                }
               // row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                row["Description"] = dgrdDetails.Rows[0].Cells["particular"].Value;
                double dAmount = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["amount"].Value);
                row["Amount"] = dAmount.ToString("N2", MainPage.indianCurancy);
                row["AmountinWord"] = objCurrency.changeCurrencyToWords(dAmount);

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");
                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where CashVCode='" + txtVoucherCode.Text + "' Order by CD.ID asc ");
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];
                    row["CompanyAddress"] = _row["CompanyAddress"];
                    row["CompanyEmailID"] = _row["CompanyPhoneNo"];
                    row["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                    row["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];
                }
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            AskForPrint();
            btnPrint.Enabled = true;
        }

        private void GetCashBalance()
        {
            if (txtCashAccount.Text != "")
            {
                double dAmt = dba.GetPartyAmountFromQuery(txtCashAccount.Text);
                if (dAmt > 0)
                    lblCashBalance.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else if (dAmt < 0)
                    lblCashBalance.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                else
                    lblCashBalance.Text = dAmt.ToString("0.00");
            }
            else
                lblCashBalance.Text = "0.00";
        }

        
        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strWhastappMessage="", strMessage = "", strNetBalance = "", strPartyName = "",strMobileNo="", strGroupName="", strCashName="", strPartyID="",strWhatsappNo="";
                    double dNetAmt = 0;
                    strCashName = dba.GetSafePartyName(txtCashAccount.Text);
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage= strNetBalance = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        string[] strFullName = strPartyName.Split(' ');
                        if (strFullName.Length > 1)
                            strPartyID = strFullName[0].Trim();
                        strWhatsappNo = "";
                        DataTable dt = DataBaseAccess.GetDataTableRecord("Select MobileNo,UPPER(GroupName) GroupName ,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) ='" + strPartyName + "' ");
                        if (dt.Rows.Count > 0)
                        {
                            strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                            strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);
                            strPartyName= dba.GetSafePartyName(strPartyName);

                            dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                            strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                            
                            if (strMobileNo.Length == 10)
                            {
                                if (rdoReceipt.Checked)
                                {
                                    strMessage = "M/S : " + strPartyName + ", We have received your amt Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                    strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strCashName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                }
                                else
                                {
                                    strMessage = "M/S : " + strPartyName + ", We have paid your amt Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                    strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strCashName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                }

                                SendSMS objSMS = new SendSMS();
                                objSMS.SendSingleSMS(strMessage, strMobileNo);
                            }

                            if (MainPage.strSoftwareType == "AGENT")
                            {
                                if (rdoReceipt.Checked)
                                    NotificationClass.SetNotification("RECEIPT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);
                                else
                                    NotificationClass.SetNotification("PAYMENT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);

                                if (strWhatsappNo != "")
                                    WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "cash_bank", strWhastappMessage, "", "");
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SendSMSToPartyForUpdate()
        {
            try
            {
                if (chkSendSMS.Checked && strOldPartyName != "")
                {
                    string strMessage = "", strWhastappMessage="", strNetBalance = "", strPartyName = "", strMobileNo = "", strBankName = "", strPartyID = "", strWhatsappNo = "";
                    double dNetAmt = 0;
                    strBankName = dba.GetSafePartyName(txtCashAccount.Text);
                    SendSMS objSMS = new SendSMS();
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage= strNetBalance = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        string[] strFullName = strPartyName.Split(' ');
                        if (strFullName.Length > 1)
                            strPartyID = strFullName[0].Trim();

                        if (strPartyName == strOldPartyName)
                        {
                            DataTable _dt = dba.GetDataTable("Select MobileNo,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD  Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')) ='" + strPartyID + "' ");
                            if (_dt.Rows.Count > 0)
                            {
                                strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                                strWhatsappNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);
                                dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                                strPartyName = dba.GetSafePartyName(strPartyName);
                                if (strMobileNo.Length == 10)
                                {
                                    if (rdoReceipt.Checked)
                                    {
                                        strMessage = "M/S : " + strPartyName + ", We have received your amt Rs. " + dNetAmt + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                        strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                    }
                                    else
                                    {
                                        strMessage = "M/S : " + strPartyName + ", We have paid your amt Rs. " + dNetAmt + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                        strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                    }

                                    //objSMS.SendSingleSMS(strMessage, strMobileNo);
                                }

                                if (MainPage.strSoftwareType == "AGENT")
                                {
                                    if (rdoReceipt.Checked)
                                        NotificationClass.SetNotification("RECEIPT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);
                                    else
                                        NotificationClass.SetNotification("PAYMENT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);

                                    if (strWhatsappNo != "")
                                        WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "cash_bank", strWhastappMessage, "", "");
                                }
                            }
                        }
                        else
                        {
                            strWhatsappNo = "";
                            string strGroupName = "", strQuery = "Select MobileNo,(Select MobileNo from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + strOldPartyName + "') MobileNoII,GroupName,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))='" + strPartyID + "' ";
                            DataTable dt = dba.GetDataTable(strQuery);
                            if (dt.Rows.Count > 0)
                            {
                                strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                                strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);
                                strPartyName = dba.GetSafePartyName(strPartyName);

                                if (strMobileNo.Length == 10)
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                                    strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);

                                    if (rdoReceipt.Checked)
                                    {
                                        strMessage = "M/S : " + strPartyName + ", We have received your amt Rs. " + dNetAmt + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                        strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                    }
                                    else
                                    {
                                        strMessage = "M/S : " + strPartyName + ", We have paid your amt Rs. " + dNetAmt + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                        strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                    }

                                    objSMS.SendSingleSMS(strMessage, strMobileNo);
                                    if (MainPage.strSoftwareType == "AGENT")
                                    {
                                        if (rdoReceipt.Checked)
                                            NotificationClass.SetNotification("RECEIPT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);
                                        else
                                            NotificationClass.SetNotification("PAYMENT", strPartyID, dNetAmt, txtVoucherCode + " " + txtVoucherNo.Text);

                                        if (strWhatsappNo != "")
                                            WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "cash_bank", strWhastappMessage, "", "");
                                    }
                                }

                                strMobileNo = Convert.ToString(dt.Rows[0]["MobileNoII"]);
                                if (strMobileNo.Length == 10)
                                {
                                    strMessage = "A/c : " + strOldPartyName + ", Sorry ! We have passed worng entry in your account on the date : " + txtDate.Text + ".";

                                    objSMS.SendSingleSMS(strMessage, strMobileNo);
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


        //private void SendSMSToPartyForUpdate()
        //{
        //    try
        //    {
        //        if (chkSendSMS.Checked && strOldPartyName != "")
        //        {
        //            string strMessage = "", strNetBalance = "", strPartyName = "", strMobileNo = "", strCashName="";
        //            double dNetAmt = 0;
        //            strCashName = dba.GetSafePartyName(txtCashAccount.Text);
        //            SendSMS objSMS = new SendSMS();
        //            foreach (DataGridViewRow row in dgrdDetails.Rows)
        //            {
        //                strMessage = strNetBalance = "";
        //                strPartyName = Convert.ToString(row.Cells["accountName"].Value);
        //                if (strPartyName == strOldPartyName)
        //                {
        //                    object objMobile = DataBaseAccess.ExecuteMyScalar("Select MobileNo from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) ='" + strPartyName + "' ");
        //                    strMobileNo = Convert.ToString(objMobile);
        //                    if (strMobileNo.Length == 10)
        //                    {
        //                        dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
        //                        //if (MainPage.strSendBalanceInSMS == "YES")
        //                        //{
        //                        //    double dAmt = dba.GetPartyAmountFromQuery(strPartyName);
        //                        //    if (dAmt > 0)
        //                        //        strNetBalance = " BAL : " + dAmt.ToString("0") + " Dr";
        //                        //    else if (dAmt < 0)
        //                        //        strNetBalance = " BAL : " + Math.Abs(dAmt).ToString("0") + " Cr";
        //                        //    else
        //                        //        strNetBalance = " BAL : 0";
        //                        //}

        //                        //if (rdoReceipt.Checked)
        //                        //    strMessage = "Correction in A/c : " + strPartyName + ", your account has credited with amount rs. " + dNetAmt.ToString("0") + ", " + row.Cells["particular"].Value + " on date : " + txtDate.Text + strNetBalance + ".";
        //                        //else
        //                        //    strMessage = "Correction in A/c : " + strPartyName + ", your account has debited with amount rs. " + dNetAmt.ToString("0") + ", " + row.Cells["particular"].Value + " on date : " + txtDate.Text + strNetBalance + ".";
        //                        if (rdoReceipt.Checked)
        //                            strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ",WE HAVE RECEIPT YOUR AMT. Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
        //                        else
        //                            strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ",We have paid your amt Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";


        //                        objSMS.SendSingleSMS(strMessage, strMobileNo);
        //                    }
        //                }
        //                else
        //                {
        //                    string strGroupName = "", strQuery = "Select MobileNo,(Select MobileNo from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + strOldPartyName + "') MobileNoII,GroupName from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + strPartyName + "' ";
        //                    DataTable dt = dba.GetDataTable(strQuery);
        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
        //                        if (strMobileNo.Length == 10)
        //                        {
        //                            dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
        //                            strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
        //                            //if (MainPage.strSendBalanceInSMS == "YES")
        //                            //{
        //                            //    double dAmt = dba.GetPartyAmountFromQuery(strPartyName);
        //                            //    if (dAmt > 0)
        //                            //        strNetBalance = " BAL : " + dAmt.ToString("0") + " Dr";
        //                            //    else if (dAmt < 0)
        //                            //        strNetBalance = " BAL : " + Math.Abs(dAmt).ToString("0") + " Cr";
        //                            //    else
        //                            //        strNetBalance = " BAL : 0";
        //                            //}
        //                            //if (strGroupName == "SUNDRY DEBTORS")// (MainPage.strSendBalanceInSMS == "YES" )
        //                            //{
        //                            //    strNetBalance = dba.CalculateNetBalance(strPartyName);
        //                            //}

        //                            if (rdoReceipt.Checked)
        //                                strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ",WE HAVE RECEIPT YOUR AMT. Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
        //                            else
        //                                strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ",We have paid your amt Rs. " + dNetAmt + " THRU " + strCashName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";

        //                            objSMS.SendSingleSMS(strMessage, strMobileNo);
        //                        }
        //                        strMobileNo = Convert.ToString(dt.Rows[0]["MobileNoII"]);
        //                        if (strMobileNo.Length == 10)
        //                        {
        //                            strMessage =  "A/c : "+strOldPartyName + ", Sorry ! We have passed worng entry in your account on the date : " + txtDate.Text + ".";

        //                            objSMS.SendSingleSMS(strMessage, strMobileNo);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void SendSMSToPartyForDelete()
        {
            try
            {
                if (chkSendSMS.Checked && strOldPartyName != "")
                {
                    SendSMS objSMS = new SendSMS();
                    string strMessage = "", strMobileNo = "";
                    object objMobile = DataBaseAccess.ExecuteMyScalar("Select MobileNo from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)='" + strOldPartyName + "' ");
                    strMobileNo = Convert.ToString(objMobile);
                    if (strMobileNo.Length == 10)
                    {
                        strMessage = "A/c : " + strOldPartyName + ", Sorry ! We have passed worng entry in your account on the date : " + txtDate.Text + ".";

                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void CashBook_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (_cashStatus > 0)
                    {
                        btnAdd.PerformClick();
                        if (_cashStatus == 1)
                            rdoReceipt.Checked = true;
                        else if (_cashStatus == 2)
                            rdoPayment.Checked = true;
                        txtDate.Focus();
                    }
                }
            }
            catch { }
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
                    txtVoucherNo.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                this.Close();
                return false;
            }
        }

        private void CashBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private bool ValidateControlONEditDelete(bool _bStatus)
        {
            if (dOldAmount != dba.ConvertObjectToDouble(lblTotalAmt.Text) || strOldPartyName != Convert.ToString(dgrdDetails.Rows[0].Cells["accountName"].Value) || _bStatus)
            {
                if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                {
                    return DataBaseAccess.CheckPartyAdjustedAmount(txtVoucherCode.Text, txtVoucherNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (btnAdd.Text != "&Save" && txtVoucherNo.Text != "" && dba.ValidateBackDateEntry(txtDate.Text) && ValidateControlONEditDelete(true))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = ValidateInsertStatus();

                            string strQuery = " Delete from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('CASH','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblTotalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);

                            if (count > 0)
                            {
                                if (!_bStatus)
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                pnlDeletionConfirmation.Visible = false;
                                txtReason.Text = "";
                                SendSMSToPartyForDelete();
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

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtVoucherNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtVoucherNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControl(txtVoucherNo.Text);
                    }
                }
                else
                {
                    txtVoucherNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtVoucherNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void rdoReceipt_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoReceipt.Checked)
            {
              //  lblCashAccount.Text = "Debit Cash A/c :";
                dgrdDetails.Columns["accountName"].HeaderText = "CREDIT ACCOUNT NAME";
            }
            else
            {
               // lblCashAccount.Text = "Credit Cash A/c :";
                dgrdDetails.Columns["accountName"].HeaderText = "DEBIT ACCOUNT NAME";
            }
        }

        private void txtCashAccount_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtCashAccount.Text);
        }

        private void txtVoucherCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CASHVOUCHERCODE", "SEARCH VOUCHER CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtVoucherCode.Text = objSearch.strSelectedData;
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

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtVoucherCode.Text != "" && txtVoucherNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("CASH", txtVoucherCode.Text, txtVoucherNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnName_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    SearchData objSearch = new SearchData("CASHPARTY", "SEARCH CASH A/C", Keys.Space);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtCashAccount.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtCashAccount.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtCashAccount.Text = "";
                        }
                        else
                            GetCashBalance();
                    }
                }
            }
            catch
            {
            }
        }

        private void txtGSTNature_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("JOURNALGSTNATURE", "SEARCH GST NATURE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtGSTNature.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGenerateTCS_Click(object sender, EventArgs e)
        {
            try
            {
                btnGenerateTCS.Enabled = false;
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && dgrdDetails.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to generate tcs debit note ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strGroupName = Convert.ToString(dgrdDetails.Rows[0].Cells["groupName"].Value);
                        if (strGroupName == "SUNDRY DEBTORS" && rdoReceipt.Checked)
                        {
                            double dVNo = dba.ConvertObjectToDouble(txtVoucherNo.Text);
                            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                            if (_date >= Convert.ToDateTime("10/01/2020"))
                            {
                                int count = dba.SaveTCSDetails(txtVoucherCode.Text, dVNo, dgrdDetails.RowCount);
                                if (count > 0)
                                {
                                    MessageBox.Show("Thank you ! Record TCS debit note generated successfully ", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                }
                                else
                                    MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnGenerateTCS.Enabled = true;
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex < 2)
                    {
                        if (_objSearch != null)
                            _objSearch.Close();
                    }
                }
                else
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex < 2)
                    {
                        if (_objSearch != null)
                        {
                            _objSearch.txtSearch.Text = e.KeyChar.ToString().Trim();
                            _objSearch.txtSearch.SelectionStart = 1;
                        }
                    }
                }
            }
            catch { }
        }
    }

}
