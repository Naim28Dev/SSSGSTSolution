using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

namespace SSS
{
    public partial class BankBook : Form
    {
        DataBaseAccess dba;
        ChangeCurrencyToWord objCurrency;
        string strOldPartyName = "";
        double dOldAmount = 0;
        SearchData _objSearch;
        public BankBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            GetBankVoucherCode();
            BindLastRecord();
        }

        public BankBook(string strCode, string strSerial)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            if (strCode == "")
                GetBankVoucherCode();
            else
                txtVoucherCode.Text = strCode;
            BindRecordWithControl(strSerial);
        }

        private void GetBankVoucherCode()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select BankVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ");
                txtVoucherCode.Text = Convert.ToString(objValue);
                if (txtVoucherCode.Text == "" || txtVoucherCode.Text == "0")
                {
                    MessageBox.Show("Sorry ! Please enter bank voucher code in company setting !", "Waring", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = btnDelete.Enabled = false;
                }
            }
            catch
            {
            }
        }

        private void BankBook_KeyDown(object sender, KeyEventArgs e)
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

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(VoucherNo),'') from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' ");
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
                string strQuery = " Select Top 1 *,Convert(varchar,Date,103) SDate,dbo.GetFullName(AccountID)NPartyName,PartyType,(AccountStatusID+' '+SM.Name)NAccountStatus,GroupName,(CASE WHEN ISNULL(CostCentreAccountID,'')!='' then dbo.GetFullName(CostCentreAccountID) else '' end) NCostCentreName,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,BA.Date))) LockType,(Select TOP 1 BillNo from TCSDetails TCS Where TCS.VoucherCode=BA.VoucherCode and TCS.VoucherNo=BA.VoucherNo)TCS from BalanceAmount BA OUTER APPLY (Select SM.Name,SM.TinNumber as PartyType,GroupName from SupplierMaster SM Where AccountStatusID=(SM.AreaCode+SM.AccountNo)) SM Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + strSerialNo + " and AccountID in (Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')) from SupplierMaster Where GroupName='BANK A/C') ";
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

                    if (Convert.ToString(dRow["Status"]).ToUpper() == "DEBIT")
                        rdoReceipt.Checked = true;
                    else
                        rdoPayment.Checked = true;

                    dOldAmount = Convert.ToDouble(dRow["Amount"]);

                    dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
                    dgrdDetails.Rows[0].Cells["accountName"].Value = strOldPartyName = Convert.ToString(dRow["NAccountStatus"]);
                    dgrdDetails.Rows[0].Cells["particular"].Value = dRow["Description"];
                    dgrdDetails.Rows[0].Cells["costcentreAccount"].Value = dRow["NCostCentreName"];
                    dgrdDetails.Rows[0].Cells["partyType"].Value = dRow["PartyType"];
                    dgrdDetails.Rows[0].Cells["amount"].Value = lblTotalAmt.Text = dOldAmount.ToString("N2", MainPage.indianCurancy);
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
                    if (Convert.ToString(dRow["TCS"]) != "")
                        btnGenerateTCS.BackColor = Color.DarkGreen;
                    else
                        btnGenerateTCS.BackColor = Color.FromArgb(185, 30, 12);
                    string strCreatedBy = Convert.ToString(dt.Rows[0]["UserName"]), strUpdatedBy = Convert.ToString(dt.Rows[0]["UpdatedBy"]);

                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;


                    GetCashBalance();
                    CheckPartyTypeForCostCentre();
                }
                txtVoucherNo.ReadOnly = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            txtCashAccount.Text = strOldPartyName = lblCreatedBy.Text = "";
            lblCashBalance.Text = lblTotalAmt.Text = "0.00";
            rdoReceipt.Checked = true;
            chkSendSMS.Checked = false;
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
            dOldAmount = 0;
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
                                    dgrdDetails.CurrentRow.Cells["groupName"].Value = strGroupName;
                                    if (strPartyType == "COST CENTRE")
                                        dgrdDetails.CurrentRow.Cells["partyType"].Value = strPartyType;
                                    else
                                        dgrdDetails.CurrentRow.Cells["partyType"].Value = "";
                                }
                                CheckPartyTypeForCostCentre();
                            }
                            else
                                MessageBox.Show("Sorry ! Both account name can't be same ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        string strType = "ALLPARTY", strParty = Convert.ToString(dgrdDetails.CurrentRow.Cells["accountName"].Value);
                        if (strParty.Contains("CUSTOMER"))
                            strType = "SALESPARTY";
                        else if (strParty.Contains("SUPPLIER"))
                            strType = "PURCHASEPARTY";

                        _objSearch = new SearchData(strType, "Search Account Name", Keys.Space);
                        _objSearch.ShowDialog();
                        if (_objSearch.strSearchData != "")
                        {
                            if (txtCashAccount.Text != _objSearch.strSelectedData)
                            {
                                dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                                if (dba.CheckTransactionLock(_objSearch.strSelectedData))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdDetails.CurrentCell.Value = "";
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

        private void CheckPartyTypeForCostCentre()
        {
            try
            {
                bool _bStatus = false;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["partyType"].Value) == "COST CENTRE")
                    {
                        _bStatus = true;
                        break;
                    }
                }
                if (_bStatus)
                {
                    dgrdDetails.Columns["costcentreAccount"].Visible = _bStatus;
                    dgrdDetails.Columns["particular"].Width = 200;
                }
                else
                {
                    dgrdDetails.Columns["costcentreAccount"].Visible = _bStatus;
                    dgrdDetails.Columns["particular"].Width = 375;
                }
            }
            catch { }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
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
                    if (dgrdDetails.CurrentCell.ColumnIndex == 4 || dgrdDetails.CurrentCell.ColumnIndex == 3)
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
            if (dgrdDetails.CurrentCell.ColumnIndex == 3)
                dba.ValidateSpace(sender, e);
            else if (dgrdDetails.CurrentCell.ColumnIndex == 4)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 4)
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
                        SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", e.KeyCode);
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
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtCashAccount.Text == "")
            {
                MessageBox.Show("Sorry ! Bank Account can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                else if (strName == "")
                {
                    MessageBox.Show("Sorry ! Account name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["accountName"];
                    dgrdDetails.Focus();
                    return false;
                }
                else if (dAmt == 0)
                {
                    MessageBox.Show("Sorry ! Amount can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["amount"];
                    dgrdDetails.Focus();
                    return false;
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
                            if (IndexColmn < dgrdDetails.ColumnCount - 4)
                            {
                                IndexColmn += 1;
                                if (CurrentRow >= 0)
                                {
                                    if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                        IndexColmn++;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                                }
                            }
                            else if (Index == dgrdDetails.RowCount - 1)
                            {
                                string strAccountName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["accountName"].Value), strAmt = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                                if (strAccountName != "" && strAmt != "" && btnAdd.Text == "&Save")
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
                        if (colIndex == 1 || colIndex == 2 || colIndex == 3)
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(VoucherNo),0)+1)VoucherNo from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' ");
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
                SetSerialNo();
                chkSendSMS.Checked = true;
                txtDate.Focus();
                if (!MainPage.mymainObject.bCashEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                    btnEdit.Enabled = btnDelete.Enabled = true;
            }
            else if (ValidateAllControl())
            {
                btnAdd.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
            btnAdd.Enabled = true;
        }



        private void SaveRecord()
        {
            try
            {
                string[] strFullParty = txtCashAccount.Text.Split(' ');
                if (strFullParty.Length > 1)
                {
                    string strQuery = " Declare @SerialNo int;", strGroupName = "";//,@BillCode nvarchar(250),@BillNo bigint, @TCSAccount nvarchar(250), @TCSPer numeric(18,4),@TCSAmt numeric(18,2), @Amt numeric(18,2),@NetAmt numeric(18,2); ", strGroupName="";
                    string strCashAccount = "", strSecondParty = "", strDate = "", strAccountID = "", strAccountStatusID = "", strDescription = "", strCostCentreAccount = "";
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

                        //if (strDescription.Contains("CHQ") || strDescription.Contains("CHEQUE"))
                        //    _chqStatus = 0;
                        strCostCentreAccount = Convert.ToString(row.Cells["costcentreAccount"].Value);
                        if (strCostCentreAccount != "")
                        {
                            strFullParty = strCostCentreAccount.Split(' ');
                            if (strFullParty.Length > 1)
                                strCostCentreAccount = strFullParty[0];
                        }

                        strFullParty = strSecondParty.Split(' ');
                        if (strFullParty.Length > 1)
                        {
                            strAccountStatusID = strFullParty[0];
                            strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");

                            strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' ";
                            if (rdoReceipt.Checked)
                            {
                                strQuery += " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                              + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strCashAccount + "','" + strSecondParty + "','DEBIT','" + strDescription + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "'," + _chqStatus + ",'" + strCostCentreAccount + "') "
                                              + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                              + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strSecondParty + "','" + strCashAccount + "','CREDIT','" + strDescription + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountStatusID + "','" + strAccountID + "'," + _chqStatus + ",'" + strCostCentreAccount + "') ";
                            }
                            else
                            {
                                strQuery += " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                              + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strCashAccount + "','" + strSecondParty + "','CREDIT','" + strDescription + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "'," + _chqStatus + ",'" + strCostCentreAccount + "') "
                                              + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                              + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strSecondParty + "','" + strCashAccount + "','DEBIT','" + strDescription + "'," + dAmt + ",'','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountStatusID + "','" + strAccountID + "'," + _chqStatus + ",'" + strCostCentreAccount + "') ";
                            }

                            strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                     + "('BANK','" + txtVoucherCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                            if (strGroupName == "SUNDRY DEBTORS" && rdoReceipt.Checked)
                                _bSDebtorStatus = true;
                        }

                    }

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        SendSMSToParty();
                        if (_bSDebtorStatus && sDate >= Convert.ToDateTime("10/01/2020"))
                        {
                            double dVNo = dba.ConvertObjectToDouble(txtVoucherNo.Text);
                            count = dba.SaveTCSDetails(txtVoucherCode.Text, dVNo, dgrdDetails.RowCount);
                        }

                        MessageBox.Show("Thank you ! Record saved successfully.", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                    if (btnEdit.Enabled)
                    {
                        EnableAllControls();
                        chkSendSMS.Checked = true;
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
                    if (ValidateAllControl() && ValidateControlONEditDelete(false))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    string strCashAccount = "", strSecondParty = "", strDate = "", strAccountID = "", strAccountStatusID = "", strCostCentreAccount = "";
                    strAccountID = strFullParty[0];
                    strCashAccount = txtCashAccount.Text.Replace(strAccountID + " ", "");

                    DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");

                    double dAmt = 0;                    
                    bool _bSDebtorStatus = false;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSecondParty = Convert.ToString(row.Cells["accountName"].Value);
                        strCostCentreAccount = Convert.ToString(row.Cells["costcentreAccount"].Value);
                        strGroupName = Convert.ToString(row.Cells["groupName"].Value);

                        if (strCostCentreAccount != "")
                        {
                            strFullParty = strCostCentreAccount.Split(' ');
                            if (strFullParty.Length > 1)
                                strCostCentreAccount = strFullParty[0];
                        }

                        strFullParty = strSecondParty.Split(' ');
                        if (strFullParty.Length > 1)
                        {
                            strAccountStatusID = strFullParty[0];
                            strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");

                            dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                            if (rdoReceipt.Checked)
                            {
                                strQuery += " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strCashAccount + "',[AccountStatus]='" + strSecondParty + "',[AccountID]='" + strAccountID + "',[AccountStatusID]='" + strAccountStatusID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[CostCentreAccountID]='" + strCostCentreAccount + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' "
                                         + " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strSecondParty + "',[AccountStatus]='" + strCashAccount + "',[AccountID]='" + strAccountStatusID + "',[AccountStatusID]='" + strAccountID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[CostCentreAccountID]='" + strCostCentreAccount + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' ";
                            }
                            else
                            {
                                strQuery += " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strCashAccount + "',[AccountStatus]='" + strSecondParty + "',[AccountID]='" + strAccountID + "',[AccountStatusID]='" + strAccountStatusID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[CostCentreAccountID]='" + strCostCentreAccount + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' "
                                         + " Update BalanceAmount Set Date='" + strDate + "',[PartyName]='" + strSecondParty + "',[AccountStatus]='" + strCashAccount + "',[AccountID]='" + strAccountStatusID + "',[AccountStatusID]='" + strAccountID + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "',[UpdateStatus]=1,[CostCentreAccountID]='" + strCostCentreAccount + "' Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' ";
                            }
                        }

                        if (strGroupName == "SUNDRY DEBTORS" && rdoReceipt.Checked)
                            _bSDebtorStatus = true;
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('BANK','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblTotalAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

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

        private bool ValidateControlONEditDelete(bool _bStatus)
        {
            if (dOldAmount != dba.ConvertObjectToDouble(lblTotalAmt.Text) || strOldPartyName != Convert.ToString(dgrdDetails.Rows[0].Cells["accountName"].Value) || _bStatus)
            {
                if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                {
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select (BillCode+' '+CAST(BillNo as varchar)) from TCSDetails Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + "");
                    if (Convert.ToString(objValue) != "")
                    {
                        MessageBox.Show("Sorry ! This voucher has been linked with TCS Debit note/Credit Note with Serial no " + objValue + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
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


        //private void UpdateRecord()
        //{
        //    try
        //    {
        //        string strQuery = "";
        //        DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
        //        double dAmt = 0;
        //        foreach (DataGridViewRow row in dgrdDetails.Rows)
        //        {
        //            dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);                   
        //            if (rdoReceipt.Checked)
        //            {
        //                strQuery += " Update BalanceAmount Set Date='" + sDate.ToString("MM/dd/yyyy") + "',[PartyName]='" + txtCashAccount.Text + "',[AccountStatus]='" + row.Cells["accountName"].Value + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "' Where VoucherCode='"+txtVoucherCode.Text+"' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' "
        //                              + " Update BalanceAmount Set Date='" + sDate.ToString("MM/dd/yyyy") + "',[PartyName]='" + row.Cells["accountName"].Value + "',[AccountStatus]='" + txtCashAccount.Text + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "' Where VoucherCode='"+txtVoucherCode.Text+"' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' ";
        //            }
        //            else
        //            {
        //                strQuery += " Update BalanceAmount Set Date='" + sDate.ToString("MM/dd/yyyy") + "',[PartyName]='" + txtCashAccount.Text + "',[AccountStatus]='" + row.Cells["accountName"].Value + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "' Where VoucherCode='"+txtVoucherCode.Text+"' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='CREDIT' "
        //                               + " Update BalanceAmount Set Date='" + sDate.ToString("MM/dd/yyyy") + "',[PartyName]='" + row.Cells["accountName"].Value + "',[AccountStatus]='" + txtCashAccount.Text + "',Amount=" + dAmt + ",Description='" + row.Cells["particular"].Value + "',UpdatedBy='" + MainPage.strLoginName + "' Where VoucherCode='"+txtVoucherCode.Text+"' and VoucherNo=" + txtVoucherNo.Text + " and [Status]='DEBIT' ";
        //            }
        //        }

        //        int count = dba.ExecuteMyQuery(strQuery);
        //        if (count > 0)
        //        {
        //            SendSMSToPartyForUpdate();
        //            MessageBox.Show("Thank you ! Record updated successfully ", "Record Updated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //            btnEdit.Text = "&Edit";
        //            BindRecordWithControl(txtVoucherNo.Text);
        //            AskForPrint();
        //        }
        //        else
        //            MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch
        //    {
        //    }
        //}

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
                    Reporting.ShowReport objReport = new Reporting.ShowReport("BANK RECEIPT PREVIEW");
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
                //if (MainPage._showPrintDialog)
                //{
                    DialogResult result = MessageBox.Show("Are you want to print office receipt ?", "Print Cash Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                    {
                        DataTable dt = CreateOfficeDataTable();
                        Reporting.CashReceiptReport report = new Reporting.CashReceiptReport();
                        report.SetDataSource(dt);

                        if (dgrdDetails.Rows.Count > 0)
                        {
                            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                            defS.Copies = (short)MainPage.iNCopyBank;
                            defS.Collate = false;
                            defS.FromPage = 0;
                            defS.ToPage = 0;

                            report.SetDataSource(dt);
                            if (MainPage._PrintWithDialog)
                                dba.PrintWithDialog(report,false, MainPage.iNCopyBank);
                            else
                            {
                                report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                                report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        }

                            result = MessageBox.Show("Are you also want to print bank receipt ?", "Print Cash Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes)
                            {
                                dt.Clear();
                                dt = CreateDataTable();
                                report.SetDataSource(dt);
                                if (MainPage._PrintWithDialog)
                                    dba.PrintWithDialog(report,true, MainPage.iNCopyBank);
                                else
                                    report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        }
                        report.Close();
                        report.Dispose();
                        }
                    }
                //}
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
                row["VoucherNo"] = txtVoucherCode.Text + " " + txtVoucherNo.Text; ;
                row["Date"] = txtDate.Text;
                row["CastType"] = "CONSIGNEE COPY";
                if (rdoReceipt.Checked)
                {
                    row["CashAccount"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["AccountName"] = txtCashAccount.Text;
                    row["CashStatus"] = "Bank Receipt";
                }
                else if (rdoPayment.Checked)
                {
                    row["CashAccount"] = txtCashAccount.Text;
                    row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["CashStatus"] = "Payment Receipt";
                }

                row["Description"] = dgrdDetails.Rows[0].Cells["particular"].Value;
                double dAmount = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["amount"].Value);
                row["Amount"] = dAmount.ToString("N2", MainPage.indianCurancy);
                row["AmountinWord"] = objCurrency.changeCurrencyToWords(dAmount);

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where BankVCode='" + txtVoucherCode.Text + "' Order by CD.ID asc ");
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
                row["CompanyName"] = "FOR " + MainPage.strPrintComapanyName;
                row["VoucherNo"] = txtVoucherCode.Text + " " + txtVoucherNo.Text; ;
                row["Date"] = txtDate.Text;
                row["CastType"] = "OFFICE COPY";
                if (rdoReceipt.Checked)
                {
                    row["CashAccount"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["AccountName"] = txtCashAccount.Text;
                    row["CashStatus"] = "Bank Receipt";
                }
                else if (rdoPayment.Checked)
                {
                    row["CashAccount"] = txtCashAccount.Text;
                    row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                    row["CashStatus"] = "Payment Receipt";
                }
                //  row["AccountName"] = dgrdDetails.Rows[0].Cells["accountName"].Value;
                row["Description"] = dgrdDetails.Rows[0].Cells["particular"].Value;
                double dAmount = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["amount"].Value);
                row["Amount"] = dAmount.ToString("N2", MainPage.indianCurancy);
                row["AmountinWord"] = objCurrency.changeCurrencyToWords(dAmount);

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;
                row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where BankVCode='" + txtVoucherCode.Text + "' Order by CD.ID asc ");
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

        //private string GetNetBalance(string strPartyName, double dEntryAmt)
        //{
        //    string strNetBalance = "";

        //    string[] strParty = strPartyName.Split(' ');
        //    if (strParty[0] != "")
        //    {
        //        if (rdoPayment.Checked)
        //            dEntryAmt = dEntryAmt * -1;
        //        double dAmt = NetDBAccess.GetPartyAmountFromQueryFromNet(strParty[0]);
        //        dAmt += dEntryAmt;

        //        if (dAmt > 0)
        //            strNetBalance = ", CURRENT BAL : " + dAmt.ToString("0") + " Dr";
        //        else if (dAmt < 0)
        //            strNetBalance = ", CURRENT BAL : " + Math.Abs(dAmt).ToString("0") + " Cr";
        //        else
        //            strNetBalance = ", CURRENT BAL : 0";
        //    }
        //    return strNetBalance;
        //}

        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strMessage = "", strWhastappMessage = "", strPartyID = "", strNetBalance = "", strPartyName = "", strMobileNo = "", strBankName = "", strGroupName = "", strWhatsappNo = "";
                    double dNetAmt = 0;
                    strBankName = dba.GetSafePartyName(txtCashAccount.Text);

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage = strNetBalance = strPartyID = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        string[] strFullName = strPartyName.Split(' ');
                        if (strFullName.Length > 1)
                            strPartyID = strFullName[0].Trim();

                        DataTable dt = DataBaseAccess.GetDataTableRecord("Select MobileNo,UPPER(GroupName) GroupName,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select Top 1 WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))  ='" + strPartyID + "' ");
                        if (dt.Rows.Count > 0)
                        {
                            strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                            strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                            strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void SendSMSToPartyForUpdate()
        {
            try
            {
                if (chkSendSMS.Checked && strOldPartyName != "")
                {
                    string strMessage = "", strWhastappMessage = "", strNetBalance = "", strPartyName = "", strMobileNo = "", strBankName = "", strPartyID = "", strWhatsappNo = "";
                    double dNetAmt = 0;
                    strBankName = dba.GetSafePartyName(txtCashAccount.Text);
                    SendSMS objSMS = new SendSMS();
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage = strNetBalance = "";
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
                                strPartyName = dba.GetSafePartyName(strPartyName);

                                if (strMobileNo.Length == 10)
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);

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
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender);
        }

        private void BankBook_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bCashAdd || MainPage.mymainObject.bCashEdit || MainPage.mymainObject.bCashView)
            {
                if (!MainPage.mymainObject.bCashAdd)
                    btnAdd.Enabled = btnGenerateTCS.Enabled = false;
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

        private void BankBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {

                    if (btnAdd.Text != "&Save" && txtVoucherNo.Text != "" && txtVoucherCode.Text != "" && dba.ValidateBackDateEntry(txtDate.Text) && ValidateControlONEditDelete(true))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = ValidateInsertStatus();
                            string strQuery = " Delete from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + txtVoucherNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('BANK','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblTotalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!_bStatus)
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                txtReason.Text = "";
                                pnlDeletionConfirmation.Visible = false;
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
                lblBankAccount.Text = "Debit Bank A/c :";
                dgrdDetails.Columns["accountName"].HeaderText = "CREDIT ACCOUNT NAME";
            }
            else
            {
                lblBankAccount.Text = "Credit Bank A/c :";
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
                        SearchData objSearch = new SearchData("BANKVOUCHERCODE", "SEARCH VOUCHER CODE", e.KeyCode);
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
                    EditTrailDetails objEdit = new EditTrailDetails("BANK", txtVoucherCode.Text, txtVoucherNo.Text);

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
                    SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", Keys.Space);
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
                                int count = 0;
                                //if (MainPage.strLoginName == "A")
                                //    count = dba.SaveTCSDetails_All(txtVoucherCode.Text, dVNo, 100000);
                                //else
                                    count = dba.SaveTCSDetails(txtVoucherCode.Text, dVNo, dgrdDetails.RowCount);
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
                    if (dgrdDetails.CurrentCell.ColumnIndex <3)
                    {                      
                        if (_objSearch != null)
                            _objSearch.Close();
                    }
                }
                else
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex < 3)
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
