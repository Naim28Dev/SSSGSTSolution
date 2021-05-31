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
    public partial class JournalEntry_New : Form
    {
        DataBaseAccess dba;
        ChangeCurrencyToWord objCurrency;
        string strOldPartyName = "", strDetailQuery = "";
        string strSingleRowID = "";
        SearchData _objSearch;

        public object dDebitAmt { get; private set; }

        public JournalEntry_New()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            GetJournalVoucherCode();
            BindLastRecord();
        }

        public JournalEntry_New(string strCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objCurrency = new ChangeCurrencyToWord();
            if (strCode == "")
                GetJournalVoucherCode();
            else
                txtVoucherCode.Text = strCode;

            BindRecordWithControl(strSerialNo);
        }

        private void JournalEntry_KeyDown(object sender, KeyEventArgs e)
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
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bJournalView)
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
                            BindRecordWithControl(txtVoucherNo.Text);
                        }
                        else if (e.Control && e.Shift && e.KeyCode == Keys.D)
                        {
                            if (btnAdd.Enabled && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
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

        private void GetJournalVoucherCode()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select JournalVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ");
                txtVoucherCode.Text = Convert.ToString(objValue);
                if (txtVoucherCode.Text == "" || txtVoucherCode.Text == "0")
                {
                    MessageBox.Show("Sorry ! Please enter journal voucher code in company setting !", "Waring", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = btnDelete.Enabled = false;
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

                string strQuery = " Select *,Convert(varchar,Date,103) SDate,(AccountID+' '+SM.Name) AccountName,SM.MobileNo,PartyType,Category,(CASE WHEN ISNULL(CostCentreAccountID,'')!='' then dbo.GetFullName(CostCentreAccountID) else '' end) NCostCentreName,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,BA.Date))) LockType from BalanceAmount BA OUTER APPLY (Select Name,MobileNo,SM.TinNumber as PartyType,Category from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID)SM Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=" + strSerialNo + " ";
                DataTable dt = dba.GetDataTable(strQuery);
                DisableAllControls();
                dgrdDetails.Rows.Clear();
                double dAmt = 0;
                double dDebitAmt = 0, dCreditAmt = 0;
                strOldPartyName = "";
                if (dt != null && dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);

                    txtVoucherNo.Text = strSerialNo;
                    txtDate.Text = Convert.ToString(dt.Rows[0]["SDate"]);
                    txtGSTNature.Text = Convert.ToString(dt.Rows[0]["GSTNature"]);
                    strOldPartyName = Convert.ToString(dt.Rows[0]["AccountName"]);
                    int _rowIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        dAmt = Convert.ToDouble(row["Amount"]);

                        dgrdDetails.Rows[_rowIndex].Cells["SrNo"].Value = _rowIndex + 1;
                        dgrdDetails.Rows[_rowIndex].Cells["accountName"].Value = Convert.ToString(row["AccountName"]);
                        dgrdDetails.Rows[_rowIndex].Cells["particular"].Value = row["Description"];
                        dgrdDetails.Rows[_rowIndex].Cells["gridID"].Value = row["BalanceID"]; 
                        dgrdDetails.Rows[_rowIndex].Cells["mobileNo"].Value = row["MobileNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["partyType"].Value = row["PartyType"];
                        dgrdDetails.Rows[_rowIndex].Cells["category"].Value = row["Category"];
                        dgrdDetails.Rows[_rowIndex].Cells["costcentreAccount"].Value = row["NCostCentreName"];

                        if (Convert.ToString(row["Status"]).ToUpper() == "DEBIT")
                        {
                            dDebitAmt += dAmt;
                            dgrdDetails.Rows[_rowIndex].Cells["debitAMt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                        }
                        else
                        {
                            dCreditAmt += dAmt;
                            dgrdDetails.Rows[_rowIndex].Cells["creditAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                        }
                        _rowIndex++;
                    }

                    string strCreatedBy = Convert.ToString(dt.Rows[0]["UserName"]), strUpdatedBy = Convert.ToString(dt.Rows[0]["UpdatedBy"]);

                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;

                    if (txtGSTNature.Text== "REGISTERED EXPENSE (B2B)" && Convert.ToString(dt.Rows[0]["LockType"]) == "LOCK" && MainPage.strUserRole != "SUPERADMIN" && MainPage.strUserRole != "ADMIN")
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                    {
                        if (!MainPage.mymainObject.bJournalEdit)
                            btnEdit.Enabled = btnDelete.Enabled = false;
                        else
                            btnEdit.Enabled = btnDelete.Enabled = true;
                    }

                }
                CheckPartyTypeForCostCentre();

                txtTotalDebitAmt.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                txtTotalCreditAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                txtVoucherNo.ReadOnly = false;
            }
            catch
            {
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
                    dgrdDetails.Columns["particular"].Width = 140;
                    dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells["costcentreAccount"];
                }
                else
                {
                    dgrdDetails.Columns["costcentreAccount"].Visible = _bStatus;
                    dgrdDetails.Columns["particular"].Width = 350;
                }
            }
            catch { }
        }

        private void EnableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = false; //    rdoDebit.Enabled = rdoCredit.Enabled = 
            txtVoucherNo.ReadOnly = true;
        }

        private void DisableAllControls()
        {
            dgrdDetails.ReadOnly = txtDate.ReadOnly = true;
            txtVoucherNo.ReadOnly = false; //rdoDebit.Enabled = rdoCredit.Enabled = 
        }

        private void ClearAllText()
        {
            strOldPartyName = strSingleRowID = lblCreatedBy.Text = "";          
            txtTotalDebitAmt.Text = txtTotalCreditAmt.Text = "0.00";
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
            pnlDeletionConfirmation.Visible =chkSendSMS.Checked= false;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private bool GetPartyGroupName()
        {
            string strQuery = "", strName = Convert.ToString(dgrdDetails.Rows[0].Cells["accountName"].Value);
            string[] strFullParty = strName.Split(' ');
            if (strFullParty.Length > 1)
            {
                strName = strFullParty[0];
                strQuery = " Select Name from SupplierMaster Where  GroupName like('%DIRECT EXPENSE A/C') and (AreaCode+CAST(AccountNo as nvarchar))='" + strName + "' ";
                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (Convert.ToString(objValue) != "")
                    return true;
            }
            return false;
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
                        bool _bStatus = false;
                        if (dgrdDetails.CurrentRow.Index == 1)
                        {
                            if (Convert.ToString(dgrdDetails.CurrentCell.EditedFormattedValue) == "")
                            {
                                if (GetPartyGroupName())
                                {
                                    DialogResult _result = MessageBox.Show("Are you want to book TDS payable ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                        _bStatus = true;
                                }
                            }
                        }
                        if (_bStatus)
                        {
                            _objSearch = new SearchData("TDSPAYABLEPARTYNAME", "Search TDS Payable Name", Keys.Space);
                            _objSearch.ShowDialog();
                            if (_objSearch.strSelectedData != "")
                            {
                                string[] strData = _objSearch.strSelectedData.Split('|');
                                if (strData.Length > 1)
                                {
                                    dgrdDetails.CurrentCell.Value = strData[1].Trim();
                                    dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells["creditAmt"];
                                }
                            }
                            else
                                dgrdDetails.CurrentCell.Value = "";
                        }
                        else
                        {

                            _objSearch = new SearchData("JOURNALPARTYNAME", "Search Account Name", Keys.Space);
                            _objSearch.ShowDialog();
                            string strMobileNo = "", strPartyType = "",strCategory="";
                            if (_objSearch.strSelectedData != "")
                            {
                                if (dba.CheckTransactionLockWithMobileNo(_objSearch.strSelectedData, ref strMobileNo, ref strPartyType,ref strCategory))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdDetails.CurrentCell.Value = "";
                                }
                                else
                                {
                                    dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                                    dgrdDetails.CurrentRow.Cells["mobileNo"].Value = strMobileNo;
                                    dgrdDetails.CurrentRow.Cells["category"].Value = strCategory;
                                    
                                    if (strPartyType == "COST CENTRE")
                                    {                                       
                                        dgrdDetails.CurrentRow.Cells["partyType"].Value = strPartyType;                                        
                                    }
                                    else
                                    {
                                        dgrdDetails.CurrentRow.Cells["costcentreAccount"].Value = dgrdDetails.CurrentRow.Cells["partyType"].Value = "";
                                        dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells["particular"];
                                    }

                                    CheckPartyTypeForCostCentre();
                                }
                            }
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
                            dgrdDetails.CurrentCell.Value = _objSearch.strSelectedData;
                            if (dba.CheckTransactionLock(_objSearch.strSelectedData))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdDetails.CurrentCell.Value = "";
                            }
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                        dgrdDetails.CurrentRow.Cells["creditAmt"].Value = "";
                    else if (e.ColumnIndex == 5)
                        dgrdDetails.CurrentRow.Cells["debitAMt"].Value = "";
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 || e.ColumnIndex == 4)
                CalculateAllAmount();
        }

        private void SetDifferenceAmount()
        {
            try
            {
                double dDAmt = 0, dCAmt = 0, dAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                        dDAmt += dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                    else if (Convert.ToString(row.Cells["creditAmt"].Value) != "")
                        dCAmt += dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                }

                dAmt = dDAmt - dCAmt;
                int _rowindex = dgrdDetails.Rows.Count - 1;
                if (dAmt > 0)
                {
                    dCAmt += dAmt;
                    dgrdDetails.Rows[_rowindex].Cells["creditAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (dAmt < 0)
                {
                    dDAmt += Math.Abs(dAmt);
                    dgrdDetails.Rows[_rowindex].Cells["debitAMt"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                }

                txtTotalDebitAmt.Text = dDAmt.ToString("N2", MainPage.indianCurancy);
                txtTotalCreditAmt.Text = dCAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private void CalculateAllAmount()
        {
            try
            {
                double dDAmt = 0, dCAmt = 0, dTaxPer = 0, dTaxAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                        dDAmt += dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                    else
                        dCAmt += dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                }
                txtTotalDebitAmt.Text = dDAmt.ToString("N2", MainPage.indianCurancy);
                txtTotalCreditAmt.Text = dCAmt.ToString("N2", MainPage.indianCurancy);
                lblCurrentAmount.Text = "";

                //dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);
                //dTaxAmt = ((dCAmt * dTaxPer) / 100);
                //txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);

                //CalculatNetAmount(dCAmt, dTaxAmt, false);

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
                    if (dgrdDetails.CurrentCell.ColumnIndex == 3 || dgrdDetails.CurrentCell.ColumnIndex == 4 || dgrdDetails.CurrentCell.ColumnIndex == 5)
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
            else if (dgrdDetails.CurrentCell.ColumnIndex == 5 || dgrdDetails.CurrentCell.ColumnIndex == 4)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 5 || dgrdDetails.CurrentCell.ColumnIndex == 4)
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text != "")
                    lblCurrentAmount.Text = objCurrency.changeCurrencyToWords(txt.Text);
                else
                    lblCurrentAmount.Text = "";
            }
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
                            if (IndexColmn < dgrdDetails.ColumnCount - 5)
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
                                string strAccountName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["accountName"].Value), strDAmt = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["debitAMt"].Value), strCAmt = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["creditAmt"].Value);

                                if (strAccountName != "" && (strDAmt != "" || strCAmt != ""))
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["SrNo"].Value = dgrdDetails.Rows.Count;
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["particular"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 2].Cells["particular"].Value;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["accountName"];
                                    SetDifferenceAmount();
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
                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 2 || colIndex == 3 || colIndex == 4)
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(VoucherNo),0)+1)VoucherNo from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "'  ");
            txtVoucherNo.Text = Convert.ToString(objValue);
        }

        private bool ValidateDate(DateTime date)
        {
            if (!(MainPage.mymainObject.bBackDayEntry))
            {
                if (Convert.ToDateTime(date.AddDays(3).ToString("MM/dd/yyyy")) < MainPage.currentDate)
                {
                    MessageBox.Show("Back Date Entry is not Allowed  !  Please Contact to Administrator ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            return true;
        }

        //private void ValidationForDeletion(string strID)
        //{
        //    if (strID != "" && strJournalCode.Length > 1)
        //    {
        //        DataTable table = dba.GetDataTable("Select TransactionLock from SupplierMaster Where (ISNULL(AreaCode,'')+ISNULL(AccountNo,'') in (Select DebitParty from JournalAccount Where VoucherCode='" + strJournalCode[0] + "' and VoucherNo=" + strJournalCode[1] + " ) Or (ISNULL(AreaCode,'')+ISNULL(AccountNo,'') in (Select CreditParty from JournalAccount Where VoucherCode='" + strJournalCode[0] + "' and VoucherNo=" + strJournalCode[1] + " ) ");
        //        if (table.Rows.Count > 0)
        //        {
        //            if (!Convert.ToBoolean(table.Rows[0][0]) && !Convert.ToBoolean(table.Rows[1][0]))
        //            {
        //                GetBalanceIDFromJournalID(strVoucherNo);

        //                strQuery += " Delete From JournalAccount where ID=" + strID + " and RemoteCode=0 and VoucherCode='" + strJournalCode[0] + "' and VoucherNo=" + strJournalCode[1] + "";
        //                strQuery += " Delete from BalanceAmount where JournalID='" + strVoucherNo + "'  and RemoteCode=0 ";
        //                strQuery += " Delete from CostCentreAccount where VoucherCode='" + strJournalCode[0] + "' and BalanceID='" + strJournalCode[1] + "'  and RemoteCode=0 ";

        //                object objValue = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from JournalAccount Where ID=" + strID + " and RemoteCode=0 and VoucherCode='" + strJournalCode[0] + "'  ");
        //                if (objValue != null)
        //                {
        //                    if (!Convert.ToBoolean(objValue))
        //                    {
        //                        bool partyAdjustStatus = DataBaseAccess.CheckJournalPartyAdjustedAmount(strVoucherNo, MainPage.strOnlineDataBaseName);
        //                        if (partyAdjustStatus)
        //                        {
        //                            strRemoteQuery += " Delete From JournalAccount where RemoteCode=" + strID + " and VoucherCode='" + strJournalCode[0] + "' and VoucherNo=" + strJournalCode[1] + " ";
        //                            strRemoteQuery += " Delete from BalanceAmount where JournalID='" + strVoucherNo + "'  and RemoteCode!=0 ";
        //                        }
        //                        else
        //                        {
        //                            strQuery = "";
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            }
        //        }
        //    }
        //}

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
            bool _dStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_dStatus)
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
            if (txtGSTNature.Text == "")
            {
                MessageBox.Show("Sorry ! GST Nature can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGSTNature.Focus();
                return false;
            }
            if (dba.ConvertObjectToDouble(txtTotalCreditAmt.Text) != dba.ConvertObjectToDouble(txtTotalDebitAmt.Text))
            {
                MessageBox.Show("Sorry ! Debit amt and Credit amt didn't match !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["debitAMt"];
                return false;
            }
            string strAllPartyName = "",strCategory="",strGST="";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                string strName = Convert.ToString(row.Cells["accountName"].Value);
                strCategory= Convert.ToString(row.Cells["category"].Value);

                double dDAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value), dCAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                if (strName == "" && dDAmt == 0 && dCAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else if (strName == "")
                {
                    MessageBox.Show("Sorry ! Account name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["accountName"];
                    dgrdDetails.Focus();
                    return false;
                }
                else if (dDAmt == 0 && dCAmt == 0)
                {
                    MessageBox.Show("Sorry ! Amount can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = row.Cells["debitAMt"];
                    dgrdDetails.Focus();
                    return false;
                }
                else
                {
                    if (strAllPartyName != "")
                        strAllPartyName += ",";
                    strAllPartyName += "'" + strName + "'";

                    if (strCategory == "SALARIES & WAGES")
                        txtGSTNature.Text = "SALARY";
                    else if (strCategory == "INTER BRANCH ACCOUNT")
                        txtGSTNature.Text = "INTER BRANCH";
                    else if (strCategory == "ADVANCE TO STAFF")
                        txtGSTNature.Text = "ADVANCE";
                    else
                    {
                        if (strCategory == "STATUTORY DUES")
                            strGST = "REGISTERED EXPENSE (B2B)";
                    }
                }
            }

            if(txtGSTNature.Text== "REGISTERED EXPENSE (B2B)" && txtGSTNature.Text != strGST)
            {
                MessageBox.Show("Sorry ! Registered expense must have the 'STATUTORY DUES' category account in this entry.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Please enter atleast one entry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.Rows.Add();
                dgrdDetails.Rows[0].Cells["SrNo"].Value = 1;
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["accountName"];
                return false;
            }

            if (txtGSTNature.Text == "REGISTERED EXPENSE (B2B)" || txtGSTNature.Text == "CR. NOTE RECEIVED AGAINST PURCHASE" || txtGSTNature.Text == "DR. NOTE RECEIVED AGAINST PURCHASE" || txtGSTNature.Text == "CONSOLIDATED RCM PAYBLE")
            {
                string strPartyName = "", strExpenseName = "";
                double dTaxAmt = 0, dPartyAmt = 0;
                bool _bStatus = GetAccountName(strAllPartyName, ref dTaxAmt, ref strPartyName, ref strExpenseName);
                if (_bStatus)
                {
                    dPartyAmt = GetPartyAmountFromGrid(strPartyName);
                    JournalVoucherDetails _objJVD = new JournalVoucherDetails(strPartyName, strExpenseName, txtVoucherCode.Text, txtVoucherNo.Text);
                    _objJVD.lblTotalAmt.Text = dPartyAmt.ToString("N2", MainPage.indianCurancy);
                    _objJVD.dTotalTaxAmt = dTaxAmt;
                    if (_objJVD.txtInvDate.Text.Length != 10)
                        _objJVD.txtInvDate.Text = txtDate.Text;
                    if (txtGSTNature.Text != "REGISTERED EXPENSE (B2B)")
                        _objJVD.txtReverseCharge.Enabled = false;
                    _objJVD.ShowDialog();
                    strDetailQuery = _objJVD.strDataQuery;
                    if (strDetailQuery == "")
                        return false;
                }
                else
                    return false;
            }
            else
            {
                if (btnAdd.Text == "&Save")
                    strDetailQuery = "";
                else
                    strDetailQuery = " Delete from [dbo].[JournalVoucherDetails] Where [VoucherCode]='" + txtVoucherCode.Text + "' and [VoucherNo]=" + txtVoucherNo.Text + " ";
            }

            return true;
        }

        private bool GetAccountName(string strAccountName, ref double dTaxAmt, ref string strPartyAccount, ref string strExpenseAccount)
        {
            string strTaxAccount = "";
            DataTable dt = dba.GetDataTable("Select (AreaCode+CAST(AccountNo as nvarchar)+' '+Name) Name,GroupName from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar)+' '+Name) in (" + strAccountName + ") and Name not like('%ROUND%') ");
            if (dt.Rows.Count > 0)
            {
                DataRow[] row = dt.Select("GroupName not Like('%COST OF MATERIAL TRADED%')  and GroupName not Like('%DIRECT EXPENSE%') AND GroupName not Like('OTHER CURRENT LIABILITIES') AND GroupName not Like('OTHER EXPENSES') ");
                if (row.Length > 0)
                    strPartyAccount = Convert.ToString(row[0]["Name"]);

                if (txtGSTNature.Text == "REGISTERED EXPENSE (B2B)")
                {
                    row = dt.Select("GroupName Like('%DIRECT EXPENSE%')  OR GroupName Like('%COST OF MATERIAL TRADED%') OR GroupName Like('%OTHER EXPENSES%') ");
                    if (row.Length > 0)
                        strExpenseAccount = Convert.ToString(row[0]["Name"]);
                }
                row = dt.Select("GroupName Like('OTHER CURRENT LIABILITIES') ");
                if (row.Length > 0)
                {
                    foreach (DataRow _row in row)
                    {
                        strTaxAccount = Convert.ToString(_row["Name"]);
                        dTaxAmt += GetPartyTaxAmount(strTaxAccount);
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("No Tax account found ! Do you want to continue ?", "No Tax Applied", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        return true;
                    else
                        return false;
                }
            }
            else
                return false;
            return true;
        }

        private double GetPartyTaxAmount(string strName)
        {
            double dDAmt = 0, dCAmt = 0, dAmt = 0;
            string strPartyName = "";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                dDAmt = dCAmt = 0;
                if (strPartyName == strName)
                {
                    if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                        dDAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                    //else
                    //    dCAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                    dAmt += dDAmt + dCAmt;
                }

            }
            return dAmt;
        }

        private double GetPartyAmountFromGrid(string strName)
        {
            double dDAmt = 0, dCAmt = 0, dAmt = 0;
            string strPartyName = "";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                dDAmt = dCAmt = 0;
                if (strPartyName == strName)
                {
                    if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                        dDAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                    else
                        dCAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                    dAmt += dDAmt + dCAmt;
                }

            }
            return dAmt;
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
                    EnableAllControls();
                    ClearAllText();
                    SetSerialNo();
                    txtDate.Focus();
                    chkSendSMS.Checked = false;
                    if (!MainPage.mymainObject.bJournalEdit)
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
            catch
            {
            }
        }

        //INSERT INTO[JournalAccount] ([Date],[DebitParty],[CreditParty],[Description],[Amount],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[DebitPartyID],[CreditPartyID]) VALUES "
        //                              + " ('" + strDate + "','" + strAccountName + "','" + strAccountStatus + "','" + row.Cells["particular"].Value + "'," + dAmt + ",'" + txtVoucherCode.Text + "',@SerialNo,'" + MainPage.strLoginName + "','',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "') "


        private void SaveRecord()
        {
            try
            {
                string[] strFullParty;

                string strQuery = " Declare @SerialNo int ", strDate = "";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy h:mm:ss tt");
                double dAmt = 0;
                string strAccountName = "", strAccountID = "", strDescription = "", strStatus = "", strCostCentreAccount="";

                int _chqStatus = 0;
                strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' if not exists (Select VoucherNo from BalanceAmount Where VoucherCode='" + txtVoucherCode.Text + "' and VoucherNo=@SerialNo) begin ";


                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strAccountName = Convert.ToString(row.Cells["accountName"].Value);
                    strDescription = Convert.ToString(row.Cells["particular"].Value);
                    strCostCentreAccount = Convert.ToString(row.Cells["costcentreAccount"].Value);

                    //if (strDescription.Contains("CHQ") || strDescription.Contains("CHEQUE"))
                    //    _chqStatus = 0;

                    strFullParty = strAccountName.Split(' ');
                    if (strFullParty.Length > 1)
                    {
                        strAccountID = strFullParty[0];
                        strAccountName = strAccountName.Replace(strAccountID + " ", "");
                        strStatus = "DEBIT";

                        if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                            dAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                        else
                        {
                            strStatus = "CREDIT";
                            dAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                        }

                        if (strCostCentreAccount != "")
                        {
                            strFullParty = strCostCentreAccount.Split(' ');
                            if (strFullParty.Length > 1)
                                strCostCentreAccount = strFullParty[0];
                        }

                        strQuery += "   INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature],[CostCentreAccountID]) VALUES "
                                      + " ('" + txtVoucherCode.Text + "',@SerialNo,'" + strDate + "','" + strAccountName + "','JOURNAL A/C','" + strStatus + "','" + row.Cells["particular"].Value + "'," + dAmt + ",'0','" + MainPage.strLoginName + "','','False',1,0,'" + strAccountID + "',''," + _chqStatus + ",'" + txtGSTNature.Text + "','" + strCostCentreAccount + "')  ";
                    }

                }
                strQuery += strDetailQuery;

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                   + "('JOURNAL','" + txtVoucherCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(txtTotalCreditAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                strQuery += " end ";
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    SendSMSToParty();
                    MessageBox.Show("Thank you ! Record saved successfully ", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    BindLastRecord();
                    // AskForPrint();
                }
                else
                    MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                        txtVoucherNo.ReadOnly = true;
                        pnlDeletionConfirmation.Visible = false;
                        btnEdit.Text = "&Update";
                        strSingleRowID = "";
                        txtDate.Focus();

                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateAllControl() && DataBaseAccess.CheckPartyAdjustedAmount(txtVoucherCode.Text, txtVoucherNo.Text))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch
            { }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord()
        {
            try
            {
                string strQuery = "", strDate = "", strNetQuery = "";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
                double dAmt = 0;
                string[] strFullParty = txtGSTNature.Text.Split(' ');
                string strAccountName = "", strStatus = "", strAccountID = "", strBalanceID = "", strDescription = "", strCostCentreAccount="";


                int _chqStatus = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strBalanceID = Convert.ToString(row.Cells["gridID"].Value);
                    strDescription = Convert.ToString(row.Cells["particular"].Value);
                    strAccountName = Convert.ToString(row.Cells["accountName"].Value);
                    strCostCentreAccount = Convert.ToString(row.Cells["costcentreAccount"].Value);

                    _chqStatus = 0;
                    strFullParty = strAccountName.Split(' ');
                    if (strFullParty.Length > 0)
                    {
                        strAccountID = strFullParty[0];
                        strAccountName = strAccountName.Replace(strAccountID + " ", "");
                        strStatus = "DEBIT";

                        if (strDescription.Contains("CHQ") || strDescription.Contains("CHEQUE"))
                            _chqStatus = 0;


                        if (Convert.ToString(row.Cells["debitAMt"].Value) != "")
                            dAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                        else
                        {
                            strStatus = "CREDIT";
                            dAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                        }

                        if (strCostCentreAccount != "")
                        {
                            strFullParty = strCostCentreAccount.Split(' ');
                            if (strFullParty.Length > 1)
                                strCostCentreAccount = strFullParty[0];
                        }

                        if (strBalanceID != "")
                        {
                            strQuery += " Update BalanceAmount Set Date = '" + strDate + "',[PartyName] = '" + strAccountName + "',[AccountID] = '" + strAccountID + "',[AccountStatusID] = '',[Status] = '" + strStatus + "', Amount = " + dAmt + ", Description = '" + strDescription + "',[GSTNature]='" + txtGSTNature.Text + "',[CostCentreAccountID]='"+strCostCentreAccount+"', UpdatedBy = '" + MainPage.strLoginName + "',[UpdateStatus] = 1 Where VoucherCode = '" + txtVoucherCode.Text + "' and VoucherNo = " + txtVoucherNo.Text + " and BalanceID=" + strBalanceID + " ";
                        }
                        else
                        {
                            strQuery += "   INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature],[CostCentreAccountID]) VALUES "
                                     + " ('" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + strDate + "','" + strAccountName + "','JOURNAL A/C','" + strStatus + "','" + row.Cells["particular"].Value + "'," + dAmt + ",'0','" + MainPage.strLoginName + "','','False',1,0,'" + strAccountID + "',''," + _chqStatus + ",'" + txtGSTNature.Text + "','" + strCostCentreAccount + "')  ";

                        }
                    }
                }

                if (strQuery != "")
                {
                    strDetailQuery = strDetailQuery.Replace("@SerialNo", txtVoucherNo.Text);
                    if (strDetailQuery != "")
                    {
                        strDetailQuery = " Delete from [dbo].[JournalVoucherDetails] Where [VoucherCode]='" + txtVoucherCode.Text + "' and [VoucherNo]=" + txtVoucherNo.Text + " "
                                       + strDetailQuery;
                    }
                    strQuery += strDetailQuery;

                    bool _bStatus = ValidateInsertStatus();
                    if (strSingleRowID != "")
                    {
                        strQuery += " Delete from [dbo].[BalanceAmount] Where VoucherCode = '" + txtVoucherCode.Text + "' and VoucherNo = " + txtVoucherNo.Text + " and BalanceID in (" + strSingleRowID + ") ";
                        if (!_bStatus)
                            strNetQuery = " Delete from [dbo].[BalanceAmount] Where VoucherCode = '" + txtVoucherCode.Text + "' and VoucherNo = " + txtVoucherNo.Text + " and RemoteCode in (" + strSingleRowID + ") ";
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('JOURNAL','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(txtTotalDebitAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        if (!_bStatus && MainPage.strOnlineDataBaseName != "" && strNetQuery != "")
                            DataBaseAccess.CreateDeleteQuery(strNetQuery);
                        SendSMSToPartyForUpdate();
                        MessageBox.Show("Thank you ! Record updated successfully ", "Record Updated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnEdit.Text = "&Edit";
                        BindRecordWithControl(txtVoucherNo.Text);
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

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtVoucherNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtVoucherNo.Text != "")
                BindRecordWithControl(txtVoucherNo.Text);
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

        private void JournalEntry_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bJournalAdd || MainPage.mymainObject.bJournalEdit || MainPage.mymainObject.bJournalView)
            {
                if (!MainPage.mymainObject.bJournalAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bJournalEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bJournalView)
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

        private void JournalEntry_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
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

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (btnAdd.Text != "&Save" && txtVoucherNo.Text != "" && dba.ValidateBackDateEntry(txtDate.Text) && DataBaseAccess.CheckPartyAdjustedAmount(txtVoucherCode.Text, txtVoucherNo.Text))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = ValidateInsertStatus();
                            string strQuery = " Delete from BalanceAmount Where VoucherNo=" + txtVoucherNo.Text + " and VoucherCode='" + txtVoucherCode.Text + "' "
                                            + " Delete from [dbo].[JournalVoucherDetails] Where [VoucherCode]='" + txtVoucherCode.Text + "' and [VoucherNo]=" + txtVoucherNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('JOURNAL','" + txtVoucherCode.Text + "'," + txtVoucherNo.Text + ",'" + txtReason.Text + ", With Amt : " + txtTotalDebitAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

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

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtVoucherNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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
                        SearchData objSearch = new SearchData("JOURNALVCODE", "SEARCH VOUCHER CODE", e.KeyCode);
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
                    EditTrailDetails objEdit = new EditTrailDetails("JOURNAL", txtVoucherCode.Text, txtVoucherNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.JournalReport_A5 report = new Reporting.JournalReport_A5();
                        report.SetDataSource(dt);
                        Reporting.ShowReport objReport = new Reporting.ShowReport("JOURNAL ENTRY PREVIEW");
                        objReport.myPreview.ReportSource = report;
                        objReport.ShowDialog();
                        report.Close();
                        report.Dispose();
                    }
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                    defS.Copies = (short)MainPage.iNCopyJournal;
                    defS.Collate = false;
                    defS.FromPage = 0;
                    defS.ToPage = 0;

                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.JournalReport_A5 report = new Reporting.JournalReport_A5();
                        report.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(report,false, MainPage.iNCopyJournal);
                        else
                        {
                            report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;
                            report.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        }

                        report.Close();
                        report.Dispose();
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnDetailView_Click(object sender, EventArgs e)
        {
            btnDetailView.Enabled = false;
            try
            {
                if (txtGSTNature.Text == "REGISTERED EXPENSE (B2B)" || txtGSTNature.Text == "CR. NOTE RECEIVED AGAINST PURCHASE" || txtGSTNature.Text == "DR. NOTE RECEIVED AGAINST PURCHASE" || txtGSTNature.Text == "CONSOLIDATED RCM PAYBLE")
                {
                    string strAllPartyName = "";

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        string strName = Convert.ToString(row.Cells["accountName"].Value);
                        if (strAllPartyName != "")
                            strAllPartyName += ",";
                        strAllPartyName += "'" + strName + "'";
                    }
                    if (strAllPartyName != "")
                    {
                        JournalVoucherDetails _objJVD = new JournalVoucherDetails(strAllPartyName, txtVoucherCode.Text, txtVoucherNo.Text);
                        if (_objJVD.txtInvDate.Text.Length != 10)
                            _objJVD.txtInvDate.Text = txtDate.Text;
                        _objJVD.btnSubmit.Enabled = false;
                        if (txtGSTNature.Text != "REGISTERED EXPENSE (B2B)")
                            _objJVD.txtReverseCharge.Enabled = false;
                        _objJVD.ShowDialog();
                        strDetailQuery = _objJVD.strDataQuery;
                    }
                }
            }
            catch
            {

            }
            btnDetailView.Enabled = true;
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
                myDataTable.Columns.Add("HeaderName", typeof(String));
                myDataTable.Columns.Add("VoucherNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("GSTNature", typeof(String));
                myDataTable.Columns.Add("SNo", typeof(String));
                myDataTable.Columns.Add("Particulars", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("TotalDebitAmt", typeof(String));
                myDataTable.Columns.Add("TotalCreditAmt", typeof(String));
                myDataTable.Columns.Add("AmtInWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                string strUserName = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");
                double dDebitAmt = dba.ConvertObjectToDouble(txtTotalCreditAmt.Text);
                int _index = 1;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    row["VoucherNo"] = txtVoucherCode.Text + " " + txtVoucherNo.Text;
                    row["Date"] = txtDate.Text;
                    row["GSTNature"] = txtGSTNature.Text;
                    row["SNo"] = _index+".";
                    row["Particulars"] = rows.Cells["accountName"].Value;
                    row["Description"] = rows.Cells["particular"].Value;
                    row["DebitAmt"] = rows.Cells["debitAmt"].Value;
                    row["CreditAmt"] = rows.Cells["creditAmt"].Value;

                    row["UserName"] = strUserName;
                    row["HeaderImage"] = MainPage._headerImage;
                    row["BrandLogo"] = MainPage._brandLogo;
                    row["SignatureImage"] = MainPage._signatureImage;

                    myDataTable.Rows.Add(row);
                    _index++;
                }

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select JournalVCode,BankVCode,CashVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where JournalVCode='"+txtVoucherCode.Text+"' Order by CD.ID asc ");
                if (dt.Rows.Count > 0)
                {
                    DataRow _row = dt.Rows[0];

                    myDataTable.Rows[0]["CompanyAddress"] = _row["CompanyAddress"];
                    myDataTable.Rows[0]["CompanyEmailID"] = _row["CompanyPhoneNo"];
                    myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                    myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];


                    _index = myDataTable.Rows.Count - 1;
                    myDataTable.Rows[_index]["TotalDebitAmt"] = myDataTable.Rows[_index]["TotalCreditAmt"] = txtTotalCreditAmt.Text;
                    ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();
                    myDataTable.Rows[_index]["AmtInWord"] = objCurrency.changeNumericToWords(dDebitAmt);
                }

            }
            catch
            {
            }
            return myDataTable;
        }

        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    string strMessage = "", strNetBalance = "", strPartyName = "", strMobileNo = "", strGroupName = "";
                    double dNetAmt = 0;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strNetBalance = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value);
                        if (strMobileNo.Length == 10)
                        {
                            //strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                            //if (strGroupName == "SUNDRY DEBTORS")
                            //{
                            //    // strNetBalance = dba.CalculateNetBalance(strPartyName);
                            //}

                            if (Convert.ToString(row.Cells["creditAmt"].Value) != "")
                            {
                                dNetAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                                strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ", We have received your amt Rs. " + dNetAmt + "," + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                            }
                            else
                            {
                                dNetAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                                strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ", We have paid your amt Rs. " + dNetAmt + ", " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                            }

                            SendSMS objSMS = new SendSMS();
                            objSMS.SendSingleSMS(strMessage, strMobileNo);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex < 3)
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

        private void SendSMSToPartyForUpdate()
        {
            try
            {
                if (chkSendSMS.Checked && strOldPartyName != "")
                {
                    string strMessage = "", strNetBalance = "", strPartyName = "", strMobileNo = "";
                    double dNetAmt = 0;

                    SendSMS objSMS = new SendSMS();
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strNetBalance = "";
                        strPartyName = Convert.ToString(row.Cells["accountName"].Value);
                        strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value);
                        if (strMobileNo.Length == 10)
                        {
                            if (strPartyName == strOldPartyName)
                            {
                                if (Convert.ToString(row.Cells["creditAmt"].Value) != "")
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                                    strMessage = "ALERT ! M/S : " + dba.GetSafePartyName(strPartyName) + ", We have received your amt Rs. " + dNetAmt + "," + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                }
                                else
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                                    strMessage = "ALERT ! M/S : " + dba.GetSafePartyName(strPartyName) + ", We have paid your amt Rs. " + dNetAmt + ", " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                }

                                objSMS.SendSingleSMS(strMessage, strMobileNo);
                            }
                            else
                            {
                                if (Convert.ToString(row.Cells["creditAmt"].Value) != "")
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);
                                    strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ", We have received your amt Rs. " + dNetAmt + "," + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                }
                                else
                                {
                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells["debitAMt"].Value);
                                    strMessage = "M/S : " + dba.GetSafePartyName(strPartyName) + ", We have paid your amt Rs. " + dNetAmt + ", " + row.Cells["particular"].Value + " DT : " + txtDate.Text + strNetBalance + ".";
                                }
                                objSMS.SendSingleSMS(strMessage, strMobileNo);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

    }
}
