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
    public partial class ChequeDetails : Form
    {
        DataBaseAccess dba;
        ChangeCurrencyToWord objCurrency;
        string strOldPartyName = "",_strCustomerName="";
        double dOldAmount = 0;
        string strSingleRowID = "";
        public double _chqAmount = 0;
        public string StrChqSrNo = "";

        public ChequeDetails()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            GetVoucherCode();
            BindLastRecord();
        }

        public ChequeDetails(double _Amount, string strCName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            GetVoucherCode();
            _strCustomerName = strCName;
            _chqAmount = _Amount;           
        }

        public ChequeDetails(string strCode, string strSerial)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            if (strCode == "")
                GetVoucherCode();
            else
                txtVoucherCode.Text = strCode;

            BindRecordWithControl(strSerial);
        }

        private void GetVoucherCode()
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
                else
                    txtVoucherCode.Text += "CHQ";
            }
            catch
            {
            }
        }

        private void ChequeDetails_KeyDown(object sender, KeyEventArgs e)
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

        private void SetSerialNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(BillNo),0)+1)VoucherNo from [ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' ");
            txtVoucherNo.Text = Convert.ToString(objValue);           
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from [ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from [ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from [ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' and BillNo>" + txtVoucherNo.Text + " ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from  dbo.[ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' and BillNo<" + txtVoucherNo.Text + " ");
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
                string strQuery = "Select *,Convert(varchar,Date,103) SDate,Convert(varchar,DepositeDate,103) DDate,dbo.GetFullName(DebitAccountID)DebitParty,dbo.GetFullName(CreditAccountID)CreditParty from [ChequeDetails] BA CROSS APPLY (Select Top 1 GroupName from SupplierMaster SM Where BA.DebitAccountID=(SM.AreaCode+SM.AccountNo))SM  Where BillCode='" + txtVoucherCode.Text + "' and BillNo=" + strSerialNo + " Order by ID ";

                DataTable dt = dba.GetDataTable(strQuery);
                DisableAllControls();
                dgrdDetails.Rows.Clear();
                string strGroupName = "",strDDate="";
                double dAmt = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    DataRow dRow = dt.Rows[0];
                    txtVoucherNo.Text = strSerialNo;
                    txtDate.Text = Convert.ToString(dRow["SDate"]);
                    strGroupName = Convert.ToString(dRow["GroupName"]);
                    strDDate = Convert.ToString(dRow["DepositeDate"]);
                    if (strDDate != "" && !strDDate.Contains("1900"))
                        txtDepositeDate.Text = Convert.ToString(dRow["DDate"]);
                    else
                        txtDepositeDate.Text = "";

                    if (Convert.ToString(dRow["ChequeType"]) == "PDC")
                        rdoPDCCheque.Checked = true;
                    else
                        rdoSecurityChq.Checked = true;

                    if (strGroupName == "BANK A/C")
                        rdoDebit.Checked = true;
                    else
                        rdoCredit.Checked = true;
                    if (rdoDebit.Checked)
                        txtBankAccount.Text = Convert.ToString(dRow["DebitParty"]);
                    else
                        txtBankAccount.Text = Convert.ToString(dRow["CreditParty"]);

                    int _index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dOldAmount += dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                        dgrdDetails.Rows[_index].Cells["SrNo"].Value = (_index + 1);
                        if (rdoDebit.Checked)
                            dgrdDetails.Rows[_index].Cells["accountName"].Value = row["CreditParty"];
                        else
                            dgrdDetails.Rows[_index].Cells["accountName"].Value = row["DebitParty"];

                        dgrdDetails.Rows[_index].Cells["bankName"].Value = row["BankName"];
                        dgrdDetails.Rows[_index].Cells["branchName"].Value = row["BranchName"];
                        dgrdDetails.Rows[_index].Cells["firmName"].Value = row["FirmName"];
                        dgrdDetails.Rows[_index].Cells["chequeNo"].Value = row["ChequeNo"];
                        dgrdDetails.Rows[_index].Cells["particular"].Value = row["Description"];
                        dgrdDetails.Rows[_index].Cells["gridID"].Value = row["ID"];
                        dgrdDetails.Rows[_index].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                        _index++;
                    }

                    string strCreatedBy = Convert.ToString(dt.Rows[0]["CreatedBy"]), strUpdatedBy = Convert.ToString(dt.Rows[0]["UpdatedBy"]);
                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;

                }
                lblTotalAmt.Text = dOldAmount.ToString("N2", MainPage.indianCurancy);
                txtVoucherNo.ReadOnly = false;
            }
            catch
            {
            }
        }

        private void EnableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = txtDepositeDate.ReadOnly = false;
            txtDepositeDate.Enabled = rdoPDCCheque.Checked;
        }

        private void DisableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = txtDepositeDate.ReadOnly = true;
        }

        private void ClearAllText()
        {
            //  txtBankAccount.Text =
            strOldPartyName = lblCreatedBy.Text = "";
            lblTotalAmt.Text = "0.00";
            rdoPDCCheque.Checked = rdoDebit.Checked = true;
            chkSendSMS.Checked = false;
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
            dOldAmount = 0;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtDepositeDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtDepositeDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void rdoDebit_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDebit.Checked)
            {
                lblBankAccount.Text = "Debit Back A/c :";
                dgrdDetails.Columns["accountName"].HeaderText = "CREDIT ACCOUNT NAME";
            }
            else
            {
                lblBankAccount.Text = "Credit Back A/c :";
                dgrdDetails.Columns["accountName"].HeaderText = "DEBIT ACCOUNT NAME";
            }
        }

        private void txtVoucherNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDepositeDate_Leave(object sender, EventArgs e)
        {
            if (rdoPDCCheque.Checked)
                dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtBankAccount_KeyDown(object sender, KeyEventArgs e)
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
                            txtBankAccount.Text = objSearch.strSelectedData;
                            if (dba.CheckTransactionLock(txtBankAccount.Text))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtBankAccount.Text = "";
                            }
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
                        txtBankAccount.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtBankAccount.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBankAccount.Text = "";
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void rdoSecurityChq_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtDepositeDate.Enabled = rdoPDCCheque.Checked;
                if (rdoSecurityChq.Checked)
                    txtDepositeDate.Clear();
            }
            catch { }
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
                        SearchData objSearch = new SearchData("ALLPARTY", "Search Account Name", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSearchData != "")
                        {
                            if (txtBankAccount.Text != objSearch.strSelectedData)
                            {
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                if (dba.CheckTransactionLock(objSearch.strSelectedData))
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

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
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
                    
                    if (dgrdDetails.CurrentCell.ColumnIndex >1 && dgrdDetails.CurrentCell.RowIndex>=0)
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
            int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (colIndex >1 && colIndex<7)
                dba.ValidateSpace(sender, e);
            else if (colIndex == 7)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentCell.ColumnIndex == 7)
                {
                    TextBox txt = (TextBox)sender;
                    if (txt.Text != "")
                        lblCurrentAmount.Text = objCurrency.changeCurrencyToWords(dba.ConvertObjectToDouble(txt.Text));
                    else
                        lblCurrentAmount.Text = "";
                }
            }
            catch { }
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

            //if (rdoPDCCheque.Checked && (txtDepositeDate.Text.Length != 10))
            //{
            //    MessageBox.Show("Sorry ! Please enter valid deposite date.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtDepositeDate.Focus();
            //    return false;
            //}

            if (txtBankAccount.Text == "")
            {
                MessageBox.Show("Sorry ! Bank Account can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBankAccount.Focus();
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
            string strChqNo = "";
            double dAmt = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
               // strName = Convert.ToString(row.Cells["accountName"].Value);
                strChqNo = Convert.ToString(row.Cells["chequeNo"].Value);
                dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);

                //if (strName == "" && dAmt>0)
                //{
                //    MessageBox.Show("Sorry ! Account name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    dgrdDetails.CurrentCell = row.Cells["accountName"];
                //    dgrdDetails.Focus();
                //    return false;
                //}                
                if (dAmt == 0 && strChqNo=="")
                    dgrdDetails.Rows.Remove(row);
                else if (strChqNo == "")
                {
                    MessageBox.Show("Sorry ! Cheque No can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["chequeNo"];
                    dgrdDetails.Focus();
                    return false;
                }

                //else if (dAmt == 0)
                //{
                //    MessageBox.Show("Sorry ! Amount can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    dgrdDetails.CurrentCell = row.Cells["amount"];
                //    dgrdDetails.Focus();
                //    return false;
                //}
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
                            if (IndexColmn < dgrdDetails.ColumnCount - 2)
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

                                if (strAccountName != "")
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["SrNo"].Value = dgrdDetails.Rows.Count;
                                   // dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["accountName"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["accountName"].Value;

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
                    else if (e.KeyCode == Keys.F)
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["srNo"];
                                dgrdDetails.Enabled = true;
                            }
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["gridID"].Value);
                            if (strID != "")
                            {
                                if (strSingleRowID != "")
                                    strSingleRowID += ",";
                                strSingleRowID += strID;
                            }
                            //else
                            //{
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
                            //}
                        }

                        ArrangeSerialNo();

                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 1 || colIndex == 2 || colIndex == 3 || colIndex == 4 || colIndex == 5 || colIndex == 6)
                            dgrdDetails.CurrentCell.Value = "";
                        CalculateAllAmount();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                    {
                        int _rowCount = dgrdDetails.Rows.Count;
                        dgrdDetails.Rows.Add(1);

                        dgrdDetails.Rows[_rowCount].Cells["srNo"].Value = (_rowCount + 1) + ".";
                        dgrdDetails.Rows[_rowCount].Cells["accountName"].Value = dgrdDetails.CurrentRow.Cells["accountName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["particular"].Value = dgrdDetails.CurrentRow.Cells["particular"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["amount"].Value = dgrdDetails.CurrentRow.Cells["amount"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["branchName"].Value = dgrdDetails.CurrentRow.Cells["branchName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["bankName"].Value = dgrdDetails.CurrentRow.Cells["bankName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["firmName"].Value = dgrdDetails.CurrentRow.Cells["firmName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["chequeNo"].Value = dgrdDetails.CurrentRow.Cells["chequeNo"].Value;
                        
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[_rowCount].Cells["amount"];
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
                string[] strFullParty = txtBankAccount.Text.Split(' ');
                if (strFullParty.Length > 1)
                {
                    string strQuery = " Declare @SerialNo int ", strDepositeDate = "NULL";
                    string strDate = "", strDescription = "", strBankAccountID = "", strAccountID = "", strAccountName = "", strChequeType = "PDC";
                    strBankAccountID = strFullParty[0];

                    DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");



                    if (rdoSecurityChq.Checked)
                        strChequeType = "SECURITY";
                    else if (txtDepositeDate.Text.Length==10)
                    {
                        DateTime _dDate = dba.ConvertDateInExactFormat(txtDepositeDate.Text);
                        strDepositeDate = "'" + _dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                    }
                    double dAmt = 0, dTAmt=0;

                    strQuery += " Select @SerialNo=(ISNULL(MAX(BillNo),0)+1) from dbo.[ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' ";

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                        strAccountName = Convert.ToString(row.Cells["accountName"].Value);
                        strDescription = Convert.ToString(row.Cells["particular"].Value);

                        strFullParty = strAccountName.Split(' ');
                        strAccountID = "";
                        if (strFullParty.Length > 1)
                            strAccountID = strFullParty[0];
                        else if (strAccountName != "")
                            strDescription += " " + strAccountName;

                        strQuery += " INSERT INTO [dbo].[ChequeDetails] ([BillCode],[BillNo],[Date],[DebitAccountID],[CreditAccountID],[ChequeType],[DepositeDate],[Description],[Amount],[Status],[ActiveDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BankName],[BranchName],[FirmName],[ChequeNo]) VALUES ";
                        if (rdoDebit.Checked)
                            strQuery += "('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strBankAccountID + "','" + strAccountID + "','" + strChequeType + "'," + strDepositeDate + ",'" + strDescription.Trim() + "'," + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",'PENDING',NULL,'" + MainPage.strLoginName + "','',1,0,'" + row.Cells["bankName"].Value + "','" + row.Cells["branchName"].Value + "','" + row.Cells["firmName"].Value + "','" + row.Cells["chequeNo"].Value + "') ";
                        else
                            strQuery += "('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strAccountID + "','" + strBankAccountID + "','" + strChequeType + "'," + strDepositeDate + ",'" + strDescription.Trim() + "'," + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",'PENDING',NULL,'" + MainPage.strLoginName + "','',1,0,'" + row.Cells["bankName"].Value + "','" + row.Cells["branchName"].Value + "','" + row.Cells["firmName"].Value + "','" + row.Cells["chequeNo"].Value + "') ";
                        strQuery += "select (billcode+' '+cast(billno as varchar)) as ChqSrNo from ChequeDetails where BillCode='" + txtVoucherCode.Text + "' and BillNo=@SerialNo";
                    }
                    

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + "('CHQDETAIL','" + txtVoucherCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        //DataTable _dt = ds.Tables[1];

                        //int count = dba.ExecuteMyQuery(strQuery);
                        if (dt.Rows.Count > 0)
                        {
                            //SendSMSToParty();
                            MessageBox.Show("Thank you ! Record saved successfully ", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnAdd.Text = "&Add";
                            if (_chqAmount > 0)
                            {
                                _chqAmount = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["amount"].Value);
                                DataRow row = dt.Rows[0];
                                StrChqSrNo = Convert.ToString(row["ChqSrNo"]);                                
                                this.Close();
                            }
                            else
                            {
                                BindLastRecord();
                                AskForPrint();
                            }
                        }
                        else
                            MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }

        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strMessage = "", strWhastappMessage="", strPartyID = "", strNetBalance = "", strPartyName = "", strMobileNo = "", strBankName = "", strGroupName = "", strWhatsappNo = "";
                    double dNetAmt = 0;
                    strBankName = dba.GetSafePartyName(txtBankAccount.Text);

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage= strNetBalance = strPartyID = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        string[] strFullName = strPartyName.Split(' ');
                        if (strFullName.Length > 1)
                            strPartyID = strFullName[0].Trim();

                        DataTable dt = DataBaseAccess.GetDataTableRecord("Select MobileNo,UPPER(GroupName) GroupName,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,''))  ='" + strPartyID + "' ");
                        if (dt.Rows.Count > 0)
                        {
                            strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                            strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                            strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);
                            strPartyName=dba.GetSafePartyName(strPartyName);
                            if (strMobileNo.Length == 10)
                            {
                                dNetAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);

                                if (rdoDebit.Checked)
                                {
                                    strMessage = "M/S : " + strPartyName + ", We have received your amt with cheque Rs. " + dNetAmt.ToString("N2", MainPage.indianCurancy) + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                    strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                }
                                else
                                {
                                    strMessage = "M/S : " + strPartyName + ", We have paid your amt with cheque Rs. " + dNetAmt.ToString("N2", MainPage.indianCurancy) + " THRU " + strBankName + " " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                    strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strBankName + " " + row.Cells["particular"].Value + "\" },{\"default\": \"" + txtDate.Text + "\" }";
                                }

                                    SendSMS objSMS = new SendSMS();
                                objSMS.SendSingleSMS(strMessage, strMobileNo);

                                if (strWhatsappNo != "")
                                    WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "chq_entry", strWhastappMessage, "", "");
                                //WhatsappClass.SendWhatsAppMessage(strWhatsappNo, strMessage, "", "BANK", "", "TEXT");
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

        private bool ValidateControlONEditDelete(bool _bStatus)
        {
            //if (dOldAmount != dba.ConvertObjectToDouble(lblTotalAmt.Text) || strOldPartyName != Convert.ToString(dgrdDetails.Rows[0].Cells["accountName"].Value) || _bStatus)
            //{
            //    if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
            //    {
            //        return DataBaseAccess.CheckPartyAdjustedAmount(txtVoucherCode.Text, txtVoucherNo.Text);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }
            //}
            return true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    strSingleRowID = "";
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
                string[] strFullParty = txtBankAccount.Text.Split(' ');
                if (strFullParty.Length > 1)
                {
                    string strQuery = " Declare @SerialNo int ", strDepositeDate = "NULL";
                    string strDate = "", strDescription = "", strBankAccountID = "", strAccountID = "", strAccountName = "", strChequeType = "PDC";
                    strBankAccountID = strFullParty[0];

                    DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                    strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");



                    if (rdoSecurityChq.Checked)
                        strChequeType = "SECURITY";
                    else if (txtDepositeDate.Text.Length==10)
                    {
                        DateTime _dDate = dba.ConvertDateInExactFormat(txtDepositeDate.Text);
                        strDepositeDate = "'" + _dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                    }
                    double dAmt = 0, dTAmt = 0 ;

                    
                    string strID = "", strNetQuery="";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strID = Convert.ToString(row.Cells["gridID"].Value);

                        dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                        strAccountName = Convert.ToString(row.Cells["accountName"].Value);
                        strDescription = Convert.ToString(row.Cells["particular"].Value);

                        strFullParty = strAccountName.Split(' ');
                        if (strFullParty.Length > 1)
                            strAccountID = strFullParty[0];
                        else
                            strAccountID = "";

                        if (strID == "")
                        {
                            strQuery += " INSERT INTO [dbo].[ChequeDetails] ([BillCode],[BillNo],[Date],[DebitAccountID],[CreditAccountID],[ChequeType],[DepositeDate],[Description],[Amount],[Status],[ActiveDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BankName],[BranchName],[FirmName],[ChequeNo]) VALUES ";
                            if (rdoDebit.Checked)
                                strQuery += "('" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + strDate + "','" + strBankAccountID + "','" + strAccountID + "','" + strChequeType + "'," + strDepositeDate + ",'" + row.Cells["particular"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",'PENDING',NULL,'" + MainPage.strLoginName + "','',1,0,'" + row.Cells["bankName"].Value + "','" + row.Cells["branchName"].Value + "','" + row.Cells["firmName"].Value + "','" + row.Cells["chequeNo"].Value + "') ";
                            else
                                strQuery += "('" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + strDate + "','" + strAccountID + "','" + strBankAccountID + "','" + strChequeType + "'," + strDepositeDate + ",'" + row.Cells["particular"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",'PENDING',NULL,'" + MainPage.strLoginName + "','',1,0,'" + row.Cells["bankName"].Value + "','" + row.Cells["branchName"].Value + "','" + row.Cells["firmName"].Value + "','" + row.Cells["chequeNo"].Value + "') ";
                        }
                        else
                        {
                            if (rdoDebit.Checked)
                                strQuery += " UPDATE [dbo].[ChequeDetails] Set [Date]='" + strDate + "',[DebitAccountID]='" + strBankAccountID + "',[CreditAccountID]='" + strAccountID + "',[ChequeType]='" + strChequeType + "',[DepositeDate]=" + strDepositeDate + ",[Description]='" + row.Cells["particular"].Value + "',[Amount]=" + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BankName]='" + row.Cells["bankName"].Value + "',[BranchName]='" + row.Cells["branchName"].Value + "',[FirmName]='" + row.Cells["firmName"].Value + "',[ChequeNo]='" + row.Cells["chequeNo"].Value + "' Where [BillCode]='" + txtVoucherCode.Text + "' and [BillNo]=" + txtVoucherNo.Text + " and ID=" + strID;
                            else
                                strQuery += " UPDATE [dbo].[ChequeDetails] Set [Date]='" + strDate + "',[DebitAccountID]='" + strAccountID + "',[CreditAccountID]='" + strBankAccountID + "',[ChequeType]='" + strChequeType + "',[DepositeDate]=" + strDepositeDate + ",[Description]='" + row.Cells["particular"].Value + "',[Amount]=" + dba.ConvertObjectToDouble(row.Cells["amount"].Value) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[BankName]='" + row.Cells["bankName"].Value + "',[BranchName]='" + row.Cells["branchName"].Value + "',[FirmName]='" + row.Cells["firmName"].Value + "',[ChequeNo]='" + row.Cells["chequeNo"].Value + "' Where [BillCode]='" + txtVoucherCode.Text + "' and [BillNo]=" + txtVoucherNo.Text + " and ID=" + strID;
                        }

                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + " ('CHQDETAIL','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                    bool _bStatus = ValidateInsertStatus();
                    if (strSingleRowID != "")
                    {
                        strQuery += " Delete from [dbo].[ChequeDetails] Where BillCode = '" + txtVoucherCode.Text + "' and BillNo = " + txtVoucherNo.Text + " and ID in (" + strSingleRowID + ") ";
                        if (!_bStatus)
                            strNetQuery = " Delete from [dbo].[ChequeDetails] Where BillCode = '" + txtVoucherCode.Text + "' and BillNo = " + txtVoucherNo.Text + " and ID in (" + strSingleRowID + ") ";
                    }

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        //SendSMSToParty();
                        if (!_bStatus && MainPage.strOnlineDataBaseName != "" && strNetQuery != "")
                            DataBaseAccess.CreateDeleteQuery(strNetQuery);
                        strSingleRowID = "";
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

                    if (btnAdd.Text != "&Save" && txtVoucherNo.Text != "" && txtVoucherCode.Text != "" && dba.ValidateBackDateEntry(txtDate.Text) && ValidateControlONEditDelete(true))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = ValidateInsertStatus();
                            string strQuery = " Delete from [ChequeDetails] Where BillCode='" + txtVoucherCode.Text + "' and BillNo=" + txtVoucherNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('CHQDETAIL','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblTotalAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!_bStatus)
                                    DataBaseAccess.CreateDeleteQuery(strQuery);

                                txtReason.Text = "";
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from ChequeDetails Where BillCode='" + txtVoucherCode.Text + "' and BillNo=" + txtVoucherNo.Text + " ");
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

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtVoucherCode.Text != "" && txtVoucherNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("CHQDETAIL", txtVoucherCode.Text, txtVoucherNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
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
                        SearchData objSearch = new SearchData("CHEQUEBOOKVOUCHERCODE", "SEARCH VOUCHER CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtVoucherCode.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ChequeDetails_Load(object sender, EventArgs e)
        {
            SetPermission();
            if (_chqAmount > 0)
            {
                btnAdd.PerformClick();
                btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;

                if (dgrdDetails.Rows.Count > 0)
                {
                    dgrdDetails.Rows[0].Cells["amount"].Value = _chqAmount;
                    dgrdDetails.Rows[0].Cells["accountName"].Value = _strCustomerName;
                }
            }
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            PrintChequeDetails(false);
            btnPreview.Enabled = true;
        }

        private void AskForPrint()
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you want to print cheque details ?", "Print cheque details", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        PrintChequeDetails(true);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }


        private void PrintChequeDetails(bool _bPrint)
        {
            try
            {
                string strQuery = " Select BillCode,BillNo,ChequeType,CONVERT(varchar,Date,103) Date,(CASE WHEN ISNULL(DepositeDate,'')='' then '' else CONVERT(varchar,DepositeDate,103) end) DDate,CDs.[BankName],CDs.[BranchName],[FirmName],[ChequeNo],Description,Amount,(CreditAccountID+' '+Name) PartyName,SM.PartyAddress,SM.PartyState,MobileNo,SM.PartyGSTNo, CD.FullCompanyName,CD.Address,CD.PhoneNo,CD.GSTNo,CD.CINNumber from ChequeDetails CDs OUTER APPLY (Select SM.Name,SM.Address as PartyAddress,(SM.State+' '+SM.PINCode) PartyState,SM.GSTNo as PartyGSTNo,SM.MobileNo from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=CreditAccountID)SM OUTER APPLY (Select TOP 1 * from (Select 0 as ID, CD.FullCompanyName,'H.O. Address : '+TINNo as HOAddress,'Place of Supply : '+(CD.StateName+' ('+(Select Top 1 StateCode from StateMaster SM Where SM.StateName=CD.StateName)+')')PlaceOfSupply,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))Address, ('Ph.  : '+CD.STDNo+'-'+CD.PhoneNo +', Email ID : '+CD.EmailId) PhoneNo,'' as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.SignaturePath,CD.BankName,CD.BranchName,CD.AccountName,CD.IFSCCode,CD.CINNumber from CompanyDetails CD Where Other in (Select CompanyName from CompanySetting Where BankVCode+'CHQ'='" + txtVoucherCode.Text + "') Union ALL  Select 1 as ID, CD.FullCompanyName,'H.O. Address : '+CD.TINNo as HOAddress,'Place of Supply : '+(CD.StateName+' ('+(Select Top 1 StateCode from StateMaster SM Where SM.StateName=CD.StateName)+')')PlaceOfSupply,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))Address, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email ID : '+CD.EmailId) PhoneNo,'' as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.SignaturePath,CD.BankName,CD.BranchName,CD.AccountName,CD.IFSCCode,CD.CINNumber from CompanyDetails CD )_CD)CD Where BillCode='" + txtVoucherCode.Text+ "' and BillNo=" + txtVoucherNo.Text + " Order by CreditAccountID,CDs.ID ";

                DataTable dt = dba.GetDataTable(strQuery);
                DataTable _dtAccount = dt.DefaultView.ToTable(true, "PartyName");

                int _index = 1;
                foreach (DataRow row in _dtAccount.Rows)
                {
                    DataTable _table = CreateDataTable();
                    DataRow[] _rows = dt.Select("PartyName='" + row["PartyName"] + "' ");

                    double dAmt = 0;
                    _index = 1;
                    string strChequeType = "";
                    foreach (DataRow _dr in _rows)
                    {
                        DataRow _row = _table.NewRow();

                        _row["CompanyName"] = _dr["FullCompanyName"];
                        _row["Address"] = _dr["Address"];
                        _row["PhoneNo"] = _dr["PhoneNo"];
                        _row["GSTIN"] = "GSTIN : " + _dr["GSTNo"];
                        _row["CINNumber"] = "CIN No : " + _dr["CINNumber"];
                        strChequeType = Convert.ToString(_dr["ChequeType"]);
                        if (strChequeType == "PDC")
                            _row["HeaderName"] = "Acknowledgement of Cheque Received";
                        else
                            _row["HeaderName"] = "Acknowledgement of Security Cheque Received";

                        _row["Date"] = "Ref. no.: "+txtVoucherCode.Text+" "+txtVoucherNo.Text+", Dt: "+ _dr["Date"];
                        _row["PartyName"] = _dr["PartyName"];
                        _row["PartyAddress"] = _dr["PartyAddress"];
                        _row["PartyAddressII"] = _dr["PartyState"];

                        if (Convert.ToString(_dr["PartyGSTNo"]) != "")
                            _row["PartyGSTIN"] = "GSTIN : " + _dr["PartyGSTNo"];

                        dAmt = dba.ConvertObjectToDouble(_dr["Amount"]);
                        if (rdoSecurityChq.Checked)
                        {
                            _row["ReferenceNo"] = "Dear Sir/Madam,\nWe would like to inform you that we  have received your  Cheques against security  for selling goods in credit  by our company.";// vide  reference no : " + _dr["BillCode"] + " " + _dr["BillNo"];
                        }
                        else
                        {
                            _row["ReferenceNo"] = "Dear Sir/Madam,\nWe would like to inform you that we  have received your  Cheques against payment.";// with reference no : " + _dr["BillCode"] + " " + _dr["BillNo"];
                        }
                        _row["SerialNo"] = _index+".";
                        _row["ChequeDetails"] = _dr["Description"];
                        _row["BankName"] = _dr["BankName"];
                        _row["BranchName"] = _dr["BranchName"];
                        _row["FirmName"] = _dr["FirmName"];
                        _row["ChequeNo"] = _dr["ChequeNo"];                    

                        if (Convert.ToString(_dr["DDate"]) == "")
                            _row["DepositeDate"] = "---";
                        else
                            _row["DepositeDate"] = _dr["DDate"];

                        if (dAmt > 0)
                            _row["Amount"] = dAmt.ToString("N2", MainPage.indianCurancy);
                        else
                            _row["Amount"] = "---";

                        _row["PrintedBy"] = "PRINTED BY : " + MainPage.strLoginName + ", Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            _row["SignatureHeader"] = "Signature valid";
                            _row["SignatureText"] = "Digitally signed by DS SARAOGI SUPER SALES PRIVATE LIMITED 2"
                                                 + "\nDate: " + MainPage.currentDate.ToString("yyyy.MM.dd") + " " + DateTime.Now.ToString("HH:mm:ss") + " +05:30"
                                                 + "\nLocation: IN";
                        }

                        _row["HeaderImage"] = MainPage._headerImage;
                        _row["SignatureImage"] = MainPage._signatureImage;
                        _row["BrandLogo"] = MainPage._brandLogo;                     

                        _index++;
                        _table.Rows.Add(_row);
                    }

                    if (_table.Rows.Count > 0)
                    {
                        if(strChequeType=="PDC")
                        {
                            Reporting.PDCChequeReport _objReport = new Reporting.PDCChequeReport();
                            _objReport.SetDataSource(_table);
                            if (_bPrint)
                            {
                                if (MainPage._PrintWithDialog)
                                    dba.PrintWithDialog(_objReport);
                                else
                                {
                                    _objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                                    _objReport.PrintToPrinter(1, false, 0, 1);
                                }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("CHEQUE DETAILS PREVIEW");
                                objReport.myPreview.ReportSource = _objReport;
                                objReport.ShowDialog();
                            }
                            _objReport.Close();
                            _objReport.Dispose();
                        }
                        else if (rdoSecurityChq.Checked)
                        {
                            Reporting.SecurityChequeReport _objReport = new Reporting.SecurityChequeReport();
                            _objReport.SetDataSource(_table);
                            if (_bPrint)
                            {
                                if (MainPage._PrintWithDialog)
                                    dba.PrintWithDialog(_objReport);
                                else
                                {
                                    _objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                                    _objReport.PrintToPrinter(1, false, 0, 1);
                                }
                            }
                            else
                            {
                                Reporting.ShowReport objReport = new Reporting.ShowReport("CHEQUE DETAILS PREVIEW");
                                objReport.myPreview.ReportSource = _objReport;
                                objReport.ShowDialog();
                            }
                            _objReport.Close();
                            _objReport.Dispose();
                        }
                    }
                }
            }
            catch { }
        }

        private DataTable CreateDataTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("CompanyName", typeof(string));
            _dt.Columns.Add("Address", typeof(string));
            _dt.Columns.Add("PhoneNo", typeof(string));
            _dt.Columns.Add("GSTIN", typeof(string));
            _dt.Columns.Add("CINNumber", typeof(string));
            _dt.Columns.Add("HeaderName", typeof(string));
            _dt.Columns.Add("Date", typeof(string));
            _dt.Columns.Add("PartyName", typeof(string));
            _dt.Columns.Add("PartyAddress", typeof(string));
            _dt.Columns.Add("PartyAddressII", typeof(string));
            _dt.Columns.Add("PartyGSTIN", typeof(string));
            _dt.Columns.Add("ReferenceNo", typeof(string));
            _dt.Columns.Add("SerialNo", typeof(string));
            _dt.Columns.Add("ChequeDetails", typeof(string));
            _dt.Columns.Add("DepositeDate", typeof(string));
            _dt.Columns.Add("Amount", typeof(string));
            _dt.Columns.Add("PrintedBy", typeof(string));
            _dt.Columns.Add("SignatureHeader", typeof(string));
            _dt.Columns.Add("SignatureText", typeof(string));
            _dt.Columns.Add("BankName", typeof(string));
            _dt.Columns.Add("BranchName", typeof(string));
            _dt.Columns.Add("FirmName", typeof(string));
            _dt.Columns.Add("ChequeNo", typeof(string));

            _dt.Columns.Add("HeaderImage", typeof(byte[]));
            _dt.Columns.Add("SignatureImage", typeof(byte[]));
            _dt.Columns.Add("BrandLogo", typeof(byte[]));

            return _dt;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            PrintChequeDetails(true);
            btnPrint.Enabled = true;
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
    }
}
