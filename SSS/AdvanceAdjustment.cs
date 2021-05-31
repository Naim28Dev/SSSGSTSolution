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
    public partial class AdvanceAdjustment : Form
    {
        DataBaseAccess dba;
        internal static string strAdvAdjCode = "";
        string strAdvAdjType = "";
        public AdvanceAdjustment()
        {
            InitializeComponent();
            try
            {
                dba = new DataBaseAccess();
                strAdvAdjCode = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select distinct AdvanceVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'"));
                txtSRCODE.Text = strAdvAdjCode;
                SetSerialNo();
                FormSetUp();
                BindLastRecord();
            }
            catch (Exception ex)
            { }
        }

        public AdvanceAdjustment(string strCode, string strNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtSRCODE.Text = strCode;
            SetSerialNo();
            FormSetUp();
            BindAllDataWithControl(txtSRCODE.Text, strNo);
        }

        private void FormSetUp()
        {
            if (rdoAdvRec.Checked)
            {
                label1.Visible = label5.Visible = label13.Visible = false;
                txtAdjustedNo.Visible = txtRefundableAmt.Visible = txtReturnedAmt.Visible = false;
                txtAdvAmt.Enabled = txtCardAmt.Enabled = true;


                if (btnAdd.Text == "&Save")
                {
                    ClearAllText();
                    EnableAllControls();
                }
            }
            else
            {
                label1.Visible = label5.Visible = label13.Visible = label18.Visible = true;
                txtAdjustedNo.Visible = txtAdjustedAmt.Visible = txtRefundableAmt.Visible = txtReturnedAmt.Visible = true;

                //txtRefundableAmt.Enabled = 
                txtAdjustedNo.Focus();
                dgrdCardDetail.Enabled = false;
                DisableAllControls();
                if (btnAdd.Text == "&Save")
                {
                    ClearAllText();
                    txtAdvAmt.Enabled = txtCardAmt.Enabled = false;
                    txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = false;
                }
            }

        }

        private void SetSerialNo()
        {
            try
            {
                DataTable _dt = dba.GetDataTable("Select (ISNULL(MAX(BillNo),0)+1)BillNo from AdvanceAdjustment where BillCode='" + txtSRCODE.Text + "' ");
                if (_dt.Rows.Count > 0)
                {
                    txtSNo.Text = Convert.ToString(_dt.Rows[0]["BillNo"]);
                }
            }
            catch
            {
            }
        }

        private void CalculateAmount()
        {
            double dNetAmt = 0, dTotalAmt = 0, dAdjustedAmt = 0, dReturnedAmt = 0, dRefundableAmt = 0;

            dTotalAmt = ConvertObjectToDouble(txtTotalAmt.Text);
            dAdjustedAmt = ConvertObjectToDouble(txtAdjustedAmt.Text);
            dReturnedAmt = ConvertObjectToDouble(txtReturnedAmt.Text);
            dRefundableAmt = ConvertObjectToDouble(txtRefundableAmt.Text);

            if (rdoAdvReturn.Checked)
                dReturnedAmt += dRefundableAmt;

            dNetAmt = dTotalAmt - dAdjustedAmt - dReturnedAmt;
            txtReturnedAmt.Text = dReturnedAmt.ToString("N2", MainPage.indianCurancy);
            txtRefundableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void BindLastRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from AdvanceAdjustment Where BillNo!=0 and BillCode='" + txtSRCODE.Text + "'");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void BindAllDataWithControl(object objSerialCode, object objSerialNo)
        {
           
            string strQuery = "  select *,ID as SID,Convert(varchar,Date,103)_Date,Convert(varchar,DelDate,103)_DDate from AdvanceAdjustment  where BillCode ='" + objSerialCode + "' and BillNo='" + objSerialNo + "'"
                            + " Select * from dbo.[CardDetails] Where BillCode='" + objSerialCode + "' and BillNo=" + objSerialNo;
            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            txtSNo.ReadOnly = false;
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                DataTable _dt = ds.Tables[1];
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    strAdvAdjType = Convert.ToString(row["AdvAdjType"]);

                    txtSRCODE.Text = Convert.ToString(row["BillCode"]);
                    txtSNo.Text = Convert.ToString(row["BillNo"]);
                    txtDate.Text = Convert.ToString(row["_Date"]);
                    txtCustomerName.Text = Convert.ToString(row["CustomerName"]);
                    txtMobile.Text = Convert.ToString(row["MobileNo"]);
                    txtRemark.Text = Convert.ToString(row["Remarks"]);
                    txtAdvAmt.Text = Convert.ToString(row["CashAmt"]);
                    txtCardAmt.Text = Convert.ToString(row["CardAmt"]);
                    txtTotalAmt.Text = Convert.ToString(row["TotalAmt"]);
                    txtAdjustedAmt.Text = Convert.ToString(row["AdjustedAmt"]);
                    txtReturnedAmt.Text = Convert.ToString(row["ReturnedAmt"]);
                    txtRefundableAmt.Text = Convert.ToString(row["RefundableAmt"]);
                    txtAdjustedNo.Text = Convert.ToString(row["AdjustedNumber"]);

                }


                if (_dt.Rows.Count > 0)
                {
                    DataRow drow = _dt.Rows[0];
                    dgrdCardDetail.Rows.Clear();
                    dgrdCardDetail.Rows.Add(_dt.Rows.Count);
                    int indexer = 0;
                    foreach (DataRow rows in _dt.Rows)
                    {
                        dgrdCardDetail.Rows[indexer].Cells["cSNo"].Value = (indexer + 1) + ".";
                        dgrdCardDetail.Rows[indexer].Cells["bank"].Value = rows["BankName"];
                        dgrdCardDetail.Rows[indexer].Cells["cCardType"].Value = rows["CardType"];
                        dgrdCardDetail.Rows[indexer].Cells["cCardNo"].Value = rows["CardNo"];
                        dgrdCardDetail.Rows[indexer].Cells["cAmt"].Value = rows["CardAmount"];

                        indexer++;
                    }
                }
            }
            else
            {
                ClearAllText();
            }
            if (strAdvAdjType == "ADVANCE RECEIVE")
            {
                label1.Visible = label5.Visible = label13.Visible = false;
                txtAdjustedNo.Visible = txtRefundableAmt.Visible = txtReturnedAmt.Visible = false;
                //txtAdvAmt.Enabled = txtCardAmt.Enabled = true;
                rdoAdvRec.Checked = true; rdoAdvReturn.Checked = false;

                if (btnAdd.Text == "&Save")
                {
                    ClearAllText();
                    EnableAllControls();
                }
            }
            if (strAdvAdjType == "ADVANCE RETURN")
            {
                label1.Visible = label5.Visible = label13.Visible = label18.Visible = true;
                txtAdjustedNo.Visible = txtAdjustedAmt.Visible = txtRefundableAmt.Visible = txtReturnedAmt.Visible = true;
                //txtRefundableAmt.Enabled = txtAdjustedNo.Enabled = true;
                rdoAdvReturn.Checked = true; rdoAdvRec.Checked = false;

                dgrdCardDetail.Enabled = false;
                DisableAllControls();
                if (btnAdd.Text == "&Save")
                {
                    ClearAllText();
                    txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = false;
                }
            }
            DisableAllControls();
            CalculateAllAmount();
        }

        private void BindRecordForReturn(object objSerialCode, object objSerialNo)
        {
            DataTable dt = dba.GetDataTable("  select *,ID as SID,Convert(varchar,Date,103)_Date,Convert(varchar,DelDate,103)_DDate from AdvanceAdjustment where BillCode ='" + objSerialCode + "' and BillNo='" + objSerialNo + "'");
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                txtCustomerName.Text = Convert.ToString(row["CustomerName"]);
                txtMobile.Text = Convert.ToString(row["MobileNo"]);
                txtRemark.Text = Convert.ToString(row["Remarks"]);
                txtAdvAmt.Text = Convert.ToString(row["CashAmt"]);
                txtCardAmt.Text = Convert.ToString(row["CardAmt"]);
                txtTotalAmt.Text = Convert.ToString(row["TotalAmt"]);
                txtAdjustedAmt.Text = Convert.ToString(row["AdjustedAmt"]);
                txtRefundableAmt.Text = Convert.ToString(row["RefundableAmt"]);
                txtReturnedAmt.Text = Convert.ToString(row["ReturnedAmt"]);

            }
            else
            {
                ClearAllText();
            }
            DisableAllControls();
            CalculateAllAmount();
        }

        private void ClearAllText()
        {
            try
            {
                //txtAltNo.Clear();
                //txtSNo.Clear();
                txtDate.Focus();
                txtMobile.Clear();
                txtAdvAmt.Clear();
                txtAdjustedAmt.Clear();
                txtTotalAmt.Clear();
                txtCardAmt.Clear();
                txtRefundableAmt.Clear();
                txtRemark.Clear();
                txtAdjustedNo.Clear();
                txtCustomerName.Clear();
                txtCardAmt.Clear();
                txtTotalAmt.Clear();
                txtReturnedAmt.Clear();

                if (DateTime.Today > MainPage.startFinDate)
                {
                    txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                }

                dgrdCardDetail.Rows.Clear();
                dgrdCardDetail.Rows.Add();
                dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
            }
            catch
            {
            }
        }

        private void dgrdCardDetail_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                    e.Cancel = true;
                else if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 3)
                    {
                        SearchData objSearch = new SearchData("CARDTYPE", "Search Card Type", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSearchData != "")
                        {
                            dgrdCardDetail.CurrentCell.Value = objSearch.strSelectedData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        SearchData objSearch = new SearchData("BANKPARTY", "Search Bank Name", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSearchData != "")
                        {
                            dgrdCardDetail.CurrentCell.Value = objSearch.strSelectedData;
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

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            Char pressedKey = e.KeyChar;
            if (txtCustomerName.Text == "" && Char.IsWhiteSpace(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtAdjustedAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtRefundableAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtDelivery_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }


        private void dgrdCardDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 6)
                {
                    double dAmt = 0;
                    foreach (DataGridViewRow row in dgrdCardDetail.Rows)
                        dAmt += ConvertObjectToDouble(row.Cells["cAmt"].Value);
                    //chkCardAmt.Checked = dAmt > 0 ? true : false;

                    txtCardAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                }
                CalculateAllAmount();
            }
            catch { }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "")
            {
                try
                {
                    dValue = Convert.ToDouble(objValue);
                }
                catch
                {
                }
            }
            return dValue;
        }

        private void dgrdCardDetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdCardDetail.CurrentCell.ColumnIndex;
                if (columnIndex == 6 || columnIndex == 5)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_Card_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_Card_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdCardDetail.CurrentCell.ColumnIndex;
                if (columnIndex == 3 || columnIndex == 5)
                {
                    dba.ValidateSpace(sender, e);
                }
                else if (columnIndex == 6)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
        }

        private void dgrdCardDetail_KeyDown(object sender, KeyEventArgs e)
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
                        Index = dgrdCardDetail.CurrentCell.RowIndex;
                        IndexColmn = dgrdCardDetail.CurrentCell.ColumnIndex;
                        if (Index < dgrdCardDetail.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdCardDetail.ColumnCount - 1)
                        {
                            IndexColmn += 1;
                            if (!dgrdCardDetail.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdCardDetail.RowCount - 1)
                        {
                            string strCardType = Convert.ToString(dgrdCardDetail.Rows[CurrentRow].Cells["cCardType"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdCardDetail.Rows[CurrentRow].Cells["cAmt"].Value);

                            if (strCardType != "" && dAmt > 0)
                            {
                                dgrdCardDetail.Rows.Add(1);
                                dgrdCardDetail.Rows[dgrdCardDetail.RowCount - 1].Cells["cSNo"].Value = dgrdCardDetail.Rows.Count;
                                dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[dgrdCardDetail.RowCount - 1].Cells["bank"];
                                dgrdCardDetail.Focus();
                            }
                            else
                            {
                                dgrdCardDetail.Focus();
                                CalculateCardAmount();
                                txtAdvAmt.Focus();
                                SelectNextControl(dgrdCardDetail, true, true, true, true);
                            }

                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdCardDetail.Rows.RemoveAt(dgrdCardDetail.CurrentRow.Index);
                        if (dgrdCardDetail.Rows.Count == 0)
                        {
                            dgrdCardDetail.Rows.Add(1);
                            dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                            dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells[0];
                            dgrdCardDetail.Enabled = true;
                        }
                        else
                        {
                            ArrangeCardSerialNo();
                        }
                        CalculateCardAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {

                        dgrdCardDetail.Rows.RemoveAt(dgrdCardDetail.CurrentRow.Index);
                        if (dgrdCardDetail.Rows.Count == 0)
                        {
                            dgrdCardDetail.Rows.Add(1);
                            dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                            dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells[2];
                            dgrdCardDetail.Enabled = true;
                        }
                        else
                        {
                            ArrangeCardSerialNo();
                        }
                        CalculateCardAmount();

                    }

                }
            }
            catch (Exception ex)
            { }
        }

        private void ArrangeCardSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdCardDetail.Rows)
            {//cSNo
                row.Cells["cSNo"].Value = serialNo;
                serialNo++;
            }
        }

        private void CalculateCardAmount()
        {
            try
            {
                double dAmt = 0;
                foreach (DataGridViewRow row in dgrdCardDetail.Rows)
                    dAmt += ConvertObjectToDouble(row.Cells["cAmt"].Value);
                //chkCardAmt.Checked = dAmt > 0 ? true : false;

                txtCardAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);

            }
            catch { }
        }

        private void EnableAllControls()
        {
            foreach (Control txt in panel3.Controls)
            {
                if (txt is TextBox)
                {
                    ((TextBox)txt).ReadOnly = false;
                }
            }
            txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = false;
            txtSRCODE.ReadOnly = true;
        }

        private void DisableAllControls()
        {
            foreach (Control txt in panel3.Controls)
            {
                if (txt is TextBox)
                {
                    (txt as TextBox).ReadOnly = true;
                }
                txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = true;
                dgrdCardDetail.Enabled = false;
            }

            //dgrdAlteration.Enabled = false;
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

                        btnEdit.Text = "&Edit";
                    }
                    btnAdd.Text = "&Save";
                    txtSNo.ReadOnly = true;
                    ClearAllText();
                    grpAdvAdjType.Enabled = true;
                    if (rdoAdvReturn.Checked)
                    {
                        txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = false;
                        SetSerialNo();
                        // txtAdjustedNo.Focus();
                        btnEdit.Enabled = btnDelete.Enabled = btnPreview.Enabled = btnPrint.Enabled = false;
                    }
                    else
                    {
                        EnableAllControls();
                        SetSerialNo();
                        // txtCustomerName.Focus();
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    }
                }
                else
                {
                    double dCardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                    if (txtCustomerName.Text == "")
                    {
                        MessageBox.Show("Please select Customer Name");
                        txtCustomerName.Focus();
                    }
                    else if (txtTotalAmt.Text == "" || txtTotalAmt.Text == "0" || txtTotalAmt.Text == "0.00")
                    {
                        MessageBox.Show("Please fill Amount", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtAdvAmt.Focus();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to save this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRecord();
                            grpAdvAdjType.Enabled = false;
                        }
                    }

                }

            }
            catch (Exception ex)
            { }
        }

        private void CalculateAllAmount()
        {
            double dNetAmt = 0, dCardAmt = 0, dCashAmt = 0, dAdjustedAmt = 0, dRefundable = 0, dReturnedAmt = 0, dQty = 0;
            dCashAmt = ConvertObjectToDouble(txtAdvAmt.Text);
            dCardAmt = ConvertObjectToDouble(txtCardAmt.Text);
            dAdjustedAmt = ConvertObjectToDouble(txtAdjustedAmt.Text);
            dReturnedAmt = ConvertObjectToDouble(txtReturnedAmt.Text);
            dNetAmt = dCashAmt + dCardAmt;
            dRefundable = dCashAmt + dCardAmt - dAdjustedAmt - dReturnedAmt;
            txtRefundableAmt.Text = dRefundable.ToString("N2", MainPage.indianCurancy);
            txtTotalAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

        }

        private void SaveRecord()
        {
            string strTotalQty = "0";
            try
            {
                CalculateAmount();
                string strQuery = "", status = "", strSalePartyID = "", strSaleParty = "";
                double dNetAmt = 0, dCashAmt = 0, dCardAmt = 0, dReturnedAmt = 0;
                dNetAmt = ConvertObjectToDouble(txtRefundableAmt.Text);
                dCashAmt = ConvertObjectToDouble(txtAdvAmt.Text);
                dCardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                dReturnedAmt = ConvertObjectToDouble(txtReturnedAmt.Text);

                string[] strFullName = txtCustomerName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                }


                if (dNetAmt > 0)
                    status = "0";
                else
                    status = "1";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (rdoAdvRec.Checked)
                {
                    strQuery = " INSERT INTO AdvanceAdjustment ([BillCode],[BillNo],[Date],[AdvAdjType],[CustomerName],[MobileNo],[Remarks],[DelDate],[TotalQty],[TotalAmt],[CashAmt],[CardAmt],[AdjustedAmt],[AdjustedNumber],[ReturnedAmt],[RefundableAmt],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES"
                                      + "('" + txtSRCODE.Text + "','" + txtSNo.Text + "','" + sDate + "','ADVANCE RECEIVE','" + txtCustomerName.Text + "','" + txtMobile.Text + "','" + txtRemark.Text + "','', " + ConvertObjectToDouble(strTotalQty) + "," + ConvertObjectToDouble(txtTotalAmt.Text) + "," + ConvertObjectToDouble(txtAdvAmt.Text) + "," + ConvertObjectToDouble(txtCardAmt.Text) + ",0,0,0," + ConvertObjectToDouble(txtRefundableAmt.Text) + ",'" + status + "','" + MainPage.strLoginName + "','','1','0');";
                    if (dCashAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + sDate + "',@CashName,'CASH RECEIVE','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCashAmt + "','DR','" + dCashAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + sDate + "','" + strSaleParty + "','CASH RECEIVE','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCashAmt + "','CR','" + dNetAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName)  ";
                    }
                    if (dCardAmt > 0)
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CARD RECEIVE' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "') begin "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "',@CardName,'CARD RECEIVE','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'" + strSalePartyID + "')  "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "','" + strSaleParty + "','CARD RECEIVE','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCardAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CardName) end else begin "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]=@CardName,[Amount]=" + dCardAmt + ",[FinalAmount]='" + dCardAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CardName Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='DEBIT' "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCardAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='CREDIT' end ";

                    }
                }

                if (rdoAdvReturn.Checked)
                {
                    strQuery = " INSERT INTO AdvanceAdjustment ([BillCode],[BillNo],[Date],[AdvAdjType],[CustomerName],[MobileNo],[Remarks],[DelDate],[TotalQty],[TotalAmt],[CashAmt],[CardAmt],[AdjustedAmt],[AdjustedNumber],[ReturnedAmt],[RefundableAmt],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES"
                                      + "('" + txtSRCODE.Text + "','" + txtSNo.Text + "','" + sDate + "','ADVANCE RETURN','" + txtCustomerName.Text + "','" + txtMobile.Text + "','" + txtRemark.Text + "','', " + strTotalQty + "," + dba.ConvertObjectToDouble(txtTotalAmt.Text) + "," + dba.ConvertObjectToDouble(txtAdvAmt.Text) + "," + dba.ConvertObjectToDouble(txtCardAmt.Text) + "," + dba.ConvertObjectToDouble(txtAdjustedAmt.Text) + ","
                                      + "'" + txtAdjustedNo.Text + "'," + dba.ConvertObjectToDouble(txtReturnedAmt.Text) + "," + dba.ConvertObjectToDouble(txtRefundableAmt.Text) + ",'" + status + "','" + MainPage.strLoginName + "','',1,0);";
                    strQuery += " UPDATE Adv SET ReturnedAmt = "+dba.ConvertObjectToDouble(txtReturnedAmt.Text) + " FROM AdvanceAdjustment Adv WHERE  BillCode +' '+ Cast(BillNo as varchar(20))= '" + txtAdjustedNo.Text + "'";

                    if (dReturnedAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE' "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + sDate + "',@CashName,'CASH PAYMENT','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dReturnedAmt + "','CR','" + dReturnedAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                    + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                    + " ('" + sDate + "','" + strSaleParty + "','CASH PAYMENT','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dReturnedAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName)  ";
                    }
                }

                foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                {
                    if (dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) > 0)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails]([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                       + " ('" + txtSRCODE.Text + "'," + txtSNo.Text + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";
                    }
                    else
                    {
                        dgrdCardDetail.Rows.Remove(rows);
                    }
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                 + "('ADVANCE ADJUATMENT','" + txtSRCODE.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(txtAdvAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                if (strQuery != "")
                {
                    int Count = dba.ExecuteMyQuery(strQuery);
                    if (Count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                }

            }
            catch (Exception ex)
            { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdvanceAdjustment_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void BindFirstRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MIN(BillNo) from AdvanceAdjustment Where BillNo!=0 and BillCode='" + txtSRCODE.Text + "'");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void BindNextRecord()
        {
            if (txtSNo.Text != "")
            {
                object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select Min(BillNo) from AdvanceAdjustment Where BillNo>" + txtSNo.Text + "  and BillCode='" + txtSRCODE.Text + "'");

                if (Convert.ToString(objSerialNo) != "")
                {
                    BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
                }
                else
                {
                    BindLastRecord();
                }
            }
        }

        private void BindPreviousRecord()
        {
            if (txtSNo.Text != "")
            {
                object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select Max(BillNo) from AdvanceAdjustment Where BillNo<" + txtSNo.Text + "  and BillCode='" + txtSRCODE.Text + "'");
                if (Convert.ToString(objSerialNo) != "")
                {
                    BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
                }
                else
                {
                    BindFirstRecord();
                }
            }
        }

        private void AdvanceAdjustment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdCardDetail.Focus())
            {
                SendKeys.Send("{TAB}");
            }
            else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (e.KeyCode == Keys.Home)
                {
                    BindFirstRecord();
                }
                else if (e.KeyCode == Keys.End)
                {
                    BindLastRecord();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    BindNextRecord();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    BindPreviousRecord();
                }
            }
        }

        private void rdoAdvRec_CheckedChanged(object sender, EventArgs e)
        {
            FormSetUp();
        }

        private void rdoReturn_CheckedChanged(object sender, EventArgs e)
        {
            FormSetUp();
        }



        private void txtAdjustedNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtAdjustedNo_Leave(object sender, EventArgs e)
        {
            CalculateAllAmount();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                grpAdvAdjType.Enabled = false;
                btnAdd.Text = "&Add";
                btnEdit.Text = "&Edit";
                //BindLastRecord();
                btnEdit.Enabled = btnDelete.Enabled = btnPrint.Enabled = btnPreview.Enabled = true;

                try
                {
                    AdvanceAdjustmentRegister objAdvanceAdjustment = new AdvanceAdjustmentRegister();
                    objAdvanceAdjustment.MdiParent = MainPage.mymainObject;
                    objAdvanceAdjustment.Show();
                }
                catch (Exception ex)
                {
                    string[] strReport = { "Exception occurred in Advance Adjustment in Main Page", ex.Message };
                    dba.CreateErrorReports(strReport);
                }
            }
            catch { }
        }

        private void txtSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtSNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select BillNo from AdvanceAdjustment Where BillNo!=0 and BillNo='" + txtSNo.Text + "' and BillCode='" + txtSRCODE.Text + "' ");

                if (Convert.ToString(objSerialNo) != "")
                {
                    BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
                }
                else
                {
                    ClearAllText();
                }
            }
        }

        private void txtCardAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtCardAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
        }

        private void txtCardAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    TextBox txtNew = sender as TextBox;
                    if (txtNew.Text == "")
                        txtNew.Text = "0.00";
                    double dcardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                    if (dcardAmt > 0)
                    {
                        dgrdCardDetail.ReadOnly = false;
                        dgrdCardDetail.Enabled = true;
                        dgrdCardDetail.Focus();
                        dgrdCardDetail.CurrentCell = dgrdCardDetail.Rows[0].Cells["bank"];
                        dgrdCardDetail.Rows[0].Cells["cAmt"].Value = Convert.ToString(txtCardAmt.Text);
                    }
                    else
                    {
                        dgrdCardDetail.ReadOnly = true;
                        dgrdCardDetail.Rows.Clear();
                        dgrdCardDetail.Rows.Add();
                        dgrdCardDetail.Rows[0].Cells["cSNo"].Value = 1;
                    }
                    CalculateAllAmount();
                }
            }
            catch (Exception ex)
            { }
        }

        private void txtAdvAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
        }

        private void txtAdvAmt_Leave(object sender, EventArgs e)
        {
            CalculateAllAmount();
        }

        private void UpdateRecord()
        {
            try
            {
                CalculateAmount();
                string strQuery = "", status = "", strSalePartyID = "", strSaleParty = "";
                double dNetAmt = 0, dCashAmt = 0, dCardAmt = 0, dReturnedAmt = 0;
                dNetAmt = ConvertObjectToDouble(txtRefundableAmt.Text);
                dCashAmt = ConvertObjectToDouble(txtAdvAmt.Text);
                dCardAmt = ConvertObjectToDouble(txtCardAmt.Text);
                dReturnedAmt = ConvertObjectToDouble(txtReturnedAmt.Text);

                string[] strFullName = txtCustomerName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtCustomerName.Text.Replace(strSalePartyID + " ", "");
                }

                if (dNetAmt > 0)
                    status = "0";
                else
                    status = "1";

                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strQuery = " Update [dbo].[AdvanceAdjustment] Set [Date]='" + sDate + "', [CustomerName]='" + txtCustomerName.Text + "',[MobileNo]='" + txtMobile.Text + "',[Remarks]= '" + txtRemark.Text + "',[TotalQty]=0,[TotalAmt]=" + ConvertObjectToDouble(txtTotalAmt.Text) + ",[CashAmt]=" + dCashAmt + ",[CardAmt]=" + dCardAmt + ",[AdjustedAmt]=" + ConvertObjectToDouble(txtAdjustedAmt.Text) + ", "
                                      + " [ReturnedAmt]=" + ConvertObjectToDouble(txtReturnedAmt.Text) + ",[RefundableAmt]=" + ConvertObjectToDouble(txtRefundableAmt.Text) + ",[Status]=" + status + ",[UpdatedBy]='" + MainPage.strLoginName + "' Where BillCode='" + txtSRCODE.Text + "' and BillNo=" + txtSNo.Text + " ";

                if (rdoAdvRec.Checked)
                {
                    if (dCashAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CASH RECEIVE' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "') begin "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "',@CashName,'CASH RECEIVE','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "'," + dCashAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "','" + strSaleParty + "','CASH RECEIVE','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "'," + dCashAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName) end else begin "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]=@CashName,[Amount]=" + dCashAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='DEBIT' "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCashAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='CREDIT' end ";
                    }
                    else
                        strQuery += " Delete from BalanceAmount Where [AccountStatus]='CASH RECEIVE' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "' ";

                    if (dCardAmt > 0)
                    {
                        strQuery += " Declare @CardName nvarchar(250); Select Top 1 @CardName=(AreaCode+AccountNo) from SupplierMaster Where Category='CARD SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CARD RECEIVE' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "') begin "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "',@CardName,'CARD RECEIVE','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCardAmt + "','DR','" + dCardAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CardName,'" + strSalePartyID + "')  "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "','" + strSaleParty + "','CARD RECEIVE','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dCardAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CardName) end else begin "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]=@CardName,[Amount]=" + dCardAmt + ",[FinalAmount]='" + dCardAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CardName Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='DEBIT' "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dCardAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CARD RECEIVE'  and Status='CREDIT' end ";

                    }
                    else
                        strQuery += " Delete from BalanceAmount Where [AccountStatus]='CARD RECEIVE' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "' ";

                }

                if (rdoAdvReturn.Checked)
                {
                    strQuery += " UPDATE Adv SET ReturnedAmt = " + dba.ConvertObjectToDouble(txtReturnedAmt.Text) + " FROM AdvanceAdjustment Adv WHERE  BillCode +' '+ Cast(BillNo as varchar(20))= '" + txtAdjustedNo.Text + "'";

                    if (dReturnedAmt > 0)
                    {
                        strQuery += " Declare @CashName nvarchar(250); Select Top 1 @CashName=(AreaCode+AccountNo) from SupplierMaster Where Category='CASH SALE'; if not exists (Select PartyName from BalanceAmount Where [AccountStatus]='CASH RETURN' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "') begin "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "',@CashName,'CASH RETURN','DEBIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dReturnedAmt + "','DR','" + dReturnedAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@CashName,'" + strSalePartyID + "')  "
                                        + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID]) VALUES  "
                                        + " ('" + sDate + "','" + strSaleParty + "','CASH RETURN','CREDIT','" + txtSRCODE.Text + " " + txtSNo.Text + "','" + dReturnedAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "',@CashName) end else begin "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]=@CashName,[Amount]=" + dReturnedAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]=@CashName Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='DEBIT' "
                                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + sDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dReturnedAmt + ",[FinalAmount]='0',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "'  Where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "' and [AccountStatus]='CASH RECEIVE'  and Status='CREDIT' end ";
                    }
                    else
                        strQuery += " Delete from BalanceAmount Where [AccountStatus]='CASH RETURN' AND [Description]='" + txtSRCODE.Text + " " + txtSNo.Text + "' ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                 + "('ADVANCE ADJUATMENT','" + txtSRCODE.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(txtAdvAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') "
                 + " Delete from [dbo].[CardDetails]Where [BillCode]='" + txtSRCODE.Text + "' and [BillNo]=" + txtSNo.Text + " ";

                foreach (DataGridViewRow rows in dgrdCardDetail.Rows)
                {
                    if (dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) > 0)
                    {
                        strQuery += " INSERT INTO [dbo].[CardDetails]([BillCode],[BillNo],[CardType],[CardNo],[ExpiryDate],[CardAmount],[InsertStatus],[UpdateStatus],[BankName])VALUES "
                                   + " ('" + txtSRCODE.Text + "'," + txtSNo.Text + ",'" + rows.Cells["cCardType"].Value + "','" + rows.Cells["cCardNo"].Value + "','" + rows.Cells["cExpiryDate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["cAmt"].Value) + ",1,0,'" + rows.Cells["bank"].Value + "') ";
                    }
                    else
                    {
                        dgrdCardDetail.Rows.Remove(rows);
                    }
                }
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record updated successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    DisableAllControls();
                    BindAllDataWithControl(txtSRCODE.Text, txtSNo.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    txtSNo.ReadOnly = true;
                    btnEdit.Text = "&Update";
                    EnableAllControls();

                    if (strAdvAdjType == "ADVANCE RETURN")
                    {
                        txtAdvAmt.ReadOnly = txtCardAmt.ReadOnly = true;
                    }

                }
                else
                {
                    if (txtAdjustedNo.Text == "")
                    {
                        MessageBox.Show("Sorry! Adjested No. can't be blank");
                        txtAdjustedNo.Focus();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to update this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && txtSNo.Text != "")
                {
                    panelSMS.Visible = true;
                    txtReason.Focus();
                    btnDelete.Enabled = false;

                }

            }
            catch
            {
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (txtReason.Text != "")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Delete record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    string strQuery = "";
                    if (rdoAdvReturn.Checked)
                        strQuery = " UPDATE Adv SET ReturnedAmt = 0 FROM AdvanceAdjustment Adv WHERE  BillCode +' '+ Cast(BillNo as varchar(20))= '" + txtAdjustedNo.Text + "'";
                    else
                        strQuery = "if((SELECT ISNULL(AdjustedinSaleBillNo,'') FROM AdvanceAdjustment WHERE BillCode = '" + txtSRCODE.Text + "' and BillNo=" + txtSNo.Text+ ")= '') BEGIN ";

                    strQuery += " Delete from AdvanceAdjustment Where BillCode='" + txtSRCODE.Text + "' and BillNo=" + txtSNo.Text
                        + " Delete from CardDetails Where BillCode='" + txtSRCODE.Text + "' and BillNo=" + txtSNo.Text + " "
                        + " Delete from BalanceAmount where Description='" + txtSRCODE.Text + " " + txtSNo.Text + "'"
                        + " INSERT INTO RemovalReason VALUES('ADVANCE ADJUATMENT','" + txtSRCODE.Text + "','" + txtSNo.Text + "','" + txtReason.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                    if (!rdoAdvReturn.Checked)
                        strQuery += " END ELSE BEGIN SELECT 0 END";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Record Delete successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        BindLastRecord();
                        panelSMS.Visible = false;
                        txtReason.Clear();
                        btnDelete.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Unable to Delete, Please try after some time ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Reason should not be blank...Please Fill the Valid Reason...");
                txtReason.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelSMS.Visible = false;
            txtReason.Clear();
            btnDelete.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = new DataTable();
                CreateDataTable(ref dt);
                if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (strAdvAdjType == "ADVANCE RECEIVE")
                    {
                        Reporting.CryAdvAdjustmentSlip objReport = new Reporting.CryAdvAdjustmentSlip();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("ADVANCE BOOK SLIP PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                    else if (strAdvAdjType == "ADVANCE RETURN")
                    {
                        Reporting.CryAdvReturnSlip objReport = new Reporting.CryAdvReturnSlip();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("ADVANCE RETURN SLIP PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                }
                else
                    MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void CreateDataTable(ref DataTable myDataTable)
        {
            try
            {
                myDataTable.Columns.Add("CompanyImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmailID", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("CustomerName", typeof(String));
                myDataTable.Columns.Add("MobileNo", typeof(String));
                myDataTable.Columns.Add("DDate", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));
                myDataTable.Columns.Add("CashAmt", typeof(String));
                myDataTable.Columns.Add("CardAmt", typeof(String));
                myDataTable.Columns.Add("TotalAmt", typeof(String));
                myDataTable.Columns.Add("AdvAdjustedNo", typeof(String));
                myDataTable.Columns.Add("AdjustedAmt", typeof(String));
                myDataTable.Columns.Add("RefundableAmt", typeof(String));
                myDataTable.Columns.Add("ReturnedAmt", typeof(String));
                myDataTable.Columns.Add("Remarks", typeof(String));
                myDataTable.Columns.Add("ItemName", typeof(String));
                myDataTable.Columns.Add("Variant1", typeof(String));
                myDataTable.Columns.Add("Variant2", typeof(String));
                myDataTable.Columns.Add("Variant3", typeof(String));
                myDataTable.Columns.Add("Variant4", typeof(String));
                myDataTable.Columns.Add("Variant5", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("SalesManName", typeof(String));
                myDataTable.Columns.Add("ItemStatus", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));

                DataRow row = myDataTable.NewRow();
                row["CompanyImage"] = MainPage._headerImage;
                row["Brandlogo"] = MainPage._brandLogo;
                string strSRCode = Convert.ToString(txtSRCODE.Text) + " " + Convert.ToString(txtSNo.Text);
                string strMOBNo = Convert.ToString(txtMobile.Text);

                row["Date"] = txtDate.Text;
                row["BillNo"] = strSRCode;
                row["CustomerName"] = txtCustomerName.Text;
                row["MobileNo"] = strMOBNo;
                row["Remarks"] = txtRemark.Text;
                row["CashAmt"] = txtAdvAmt.Text;
                row["CardAmt"] = txtCardAmt.Text;
                row["TotalAmt"] = txtTotalAmt.Text;
                row["AdvAdjustedNo"] = txtAdjustedNo.Text;
                row["AdjustedAmt"] = txtAdjustedAmt.Text;
                row["RefundableAmt"] = txtRefundableAmt.Text;
                row["ReturnedAmt"] = txtReturnedAmt.Text;
                row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                if (strAdvAdjType == "ADVANCE RECEIVE")
                    row["Headername"] = "Advance Book Slip";
                else if (strAdvAdjType == "ADVANCE RETURN")
                    row["Headername"] = "Advance Return Slip";

                DataTable dt = dba.GetDataTable("Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD CROSS APPLY (Select AdvanceVCode from CompanySetting CS Where CS.CompanyName=CD.Other) CS Where AdvanceVCode='" + txtSRCODE.Text + "' Order by CD.ID asc ");
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = new DataTable();
                CreateDataTable(ref dt);
                if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                    defS.Copies = 1;
                    defS.Collate = false;
                    defS.FromPage = 0;
                    defS.ToPage = 0;

                    Reporting.CryAdvAdjustmentSlip objReport = new Reporting.CryAdvAdjustmentSlip();
                    objReport.SetDataSource(dt);

                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                        objReport.PrintToPrinter(defS,defS.DefaultPageSettings,false);

                    objReport.Close();
                    objReport.Dispose();
                    btnPrint.Enabled = true;
                }
                else
                    MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnPrint.Enabled = true;
            }
            catch
            {
            }
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtCustomerName.Text = objSearch.strSelectedData;
                            string strMobileNo = "", strStation = "";
                            dba.CheckTransactionLockWithMobileNoStation(txtCustomerName.Text, ref strMobileNo, ref strStation);

                            if (strMobileNo != "" || strStation != "")
                            {
                                txtMobile.Text = strMobileNo;
                            }
                        }
                    }
                    else if (value != 8)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }


        private void GetAdvanceDtails()
        {
            string strBillCode = "", strBillNo = "";
            string[] strAdjustedNo = txtAdjustedNo.Text.Split(' ');
            if (strAdjustedNo.Length > 1)
            {
                strBillCode = strAdjustedNo[0].Trim();
                strBillNo = strAdjustedNo[1].Trim();
            }
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select BillNo from AdvanceAdjustment Where BillNo!=0 and BillNo='" + strBillNo + "' and BillCode='" + strBillCode + "' and AdvAdjType='ADVANCE RECEIVE'");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindRecordForReturn(txtSRCODE.Text, objSerialNo);
                txtAdjustedNo.ReadOnly = txtRefundableAmt.ReadOnly = false;
            }
            else
            {
                ClearAllText();
            }
        }
        private void txtAdjustedNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ADVANCEBILLNO", "SELECT ADVANCE BILLNO", e.KeyCode);
                    objSearch.ShowDialog();
                    txtAdjustedNo.Text = objSearch.strSelectedData;
                    GetAdvanceDtails();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void AdvanceAdjustment_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    //if (_cashStatus > 0)
                    //{
                    //    btnAdd.PerformClick();                       
                    //    txtDate.Focus();
                    //}
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
                    txtSNo.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
                return false;
            }
        }

        private void dgrdCardDetail_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                {
                    int Date = 0;
                    Date = Convert.ToInt32(dgrdCardDetail.CurrentRow.Cells["cExpiryDate"].Value);

                    if (Convert.ToBoolean(Date))
                    {
                        string strDate = Convert.ToString(dgrdCardDetail.CurrentCell.EditedFormattedValue);
                        if (strDate != "")
                        {
                            strDate = strDate.Replace("/", "");
                            if (strDate.Length == 4)
                            {
                                TextBox txtDate = new TextBox();
                                //txtDate.Text = strDate;
                                //dba.GetStringFromDateForCompany(txtDate);
                                Double dMonth = Convert.ToDouble(strDate.Substring(0, 2)), dYear = Convert.ToDouble(strDate.Substring(2, 2));
                                if (dMonth < 1 || dMonth > 12)
                                {
                                    MessageBox.Show("Month is not valid : " + dMonth, "Invalid Month ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdCardDetail.CurrentCell = dgrdCardDetail.CurrentRow.Cells["cExpiryDate"];
                                    e.Cancel = true;
                                    dgrdCardDetail.Focus();
                                    return;
                                }
                                if (dYear < 20)
                                {
                                    MessageBox.Show("Year is not valid : " + dYear, "Invalid Year ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdCardDetail.CurrentCell = dgrdCardDetail.CurrentRow.Cells["cExpiryDate"];
                                    dgrdCardDetail.Focus();
                                    return;
                                }
                                string strMon = Convert.ToString(dMonth);
                                if (strMon.Length < 2)
                                    txtDate.Text = "0" + Convert.ToString(dMonth) + "/" + Convert.ToString(dYear);
                                else
                                    txtDate.Text = Convert.ToString(dMonth) + "/" + Convert.ToString(dYear);
                                try
                                {
                                    if (!txtDate.Text.Contains("/"))
                                    {
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        if (e.RowIndex != dgrdCardDetail.Rows.Count - 1)
                                        {
                                            dgrdCardDetail.EndEdit();
                                        }
                                    }
                                    dgrdCardDetail.CurrentCell.Value = txtDate.Text;
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                MessageBox.Show("Date format is not valid ! Please Specify in MMyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                e.Cancel = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtRefundableAmt_Leave(object sender, EventArgs e)
        {
        }
    }
}
