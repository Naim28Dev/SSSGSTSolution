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
    public partial class ShowPartyDetailsWithDueDays : Form
    {
        DataBaseAccess dba;
        public ShowPartyDetailsWithDueDays()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtLastDate.Text = MainPage.currentDate.ToString("dd/MM/yyy");
        }

        private void ShowPartyDetailsWithDueDays_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGroupArrow_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", Keys.Space);
                objSearch.ShowDialog();
                txtGroupName.Text = objSearch.strSelectedData;
            }
            catch { }
        }

        private void btnPartyType_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("CASHTYPE", "SEARCH PARTY TYPE", Keys.Space);
                objSearch.ShowDialog();
                txtPartyType.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void txtPartyType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CASHTYPE", "SEARCH PARTY TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtCategory.Text = objSearch.strSelectedData;
            }
            catch { }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", txtGroupName.Text, "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("ALLPARTY", txtGroupName.Text, "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;

            }
            catch
            {
            }
        }

        private void txtState_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtState.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnStateName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", Keys.Space);
                objSearch.ShowDialog();
                txtState.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = true;
        }

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", txtGroupName.Text, "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnBranchCode_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("BRANCHCODE", txtGroupName.Text, "SEARCH BRANCH CODE", Keys.Space);
                objSearch.ShowDialog();
                txtBranchCode.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            try
            {
                GetDataFromDB();
            }
            catch { }
            btnGo.Enabled = true;
            panelSearch.Visible = false;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (txtGroupName.Text != "")
                strQuery += " and GroupName='" + txtGroupName.Text + "' ";
            if (txtBranchCode.Text != "")
                strQuery += " and AreaCode='" + txtBranchCode.Text + "' ";
            if (txtCategory.Text != "")
                strQuery += " and Category ='" + txtCategory.Text + "' ";
            if (txtState.Text != "")
                strQuery += " and State ='" + txtState.Text + "' ";
            if (txtPartyName.Text != "")
                strQuery += " and (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) ='" + txtPartyName.Text + "' ";
            if (txtPartyType.Text != "")
                strQuery += " and TINNumber ='" + txtPartyType.Text + "' ";

            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = "", strSubQuery = CreateQuery();
                DateTime _date = dba.ConvertDateInExactFormat(txtLastDate.Text);
                if (rdoPurchase.Checked)
                {
                    strQuery += " Select * from (Select *, (SUM(Amount) OVER(PARTITION BY Name))  AS PAmt from (Select (AccountID+' '+Name)Name,DueDays,DATEDIFF(dd,(DATEADD(dd,CAST(DueDays as int),Date)),'" + _date.ToString("MM/dd/yyyy") + "')_DueDays,(Convert(varchar,Date,103))_Date,Description,AccountStatus,CAST(Amount as Money)Amount,Status, "
                             + " (Select SUM(_Amt)Amt from(Select SUM(CAST(_BA.Amount as money))_Amt from BalanceAMount _BA Where Tick = 'FALSE' and _BA.Status = 'DEBIT' and _BA.AccountID = BA.AccountID UNION ALL "
                             + " Select -SUM(CAST(_BA.Amount as money))_Amt from BalanceAMount _BA Where Tick = 'FALSE' and _BA.AccountStatus != BA.AccountStatus and _BA.Description != BA.Description and _BA.Status = 'CREDIT' and _BA.AccountID = BA.AccountID)_Balance)Amt "
                             + " from BalanceAmount BA Cross APPLY (Select Name, DueDays from SupplierMaster SM Where (SM.AreaCode + AccountNo) = BA.AccountID  " + strSubQuery + " ) SM Where Tick = 'FALSE' and AccountStatus = 'PURCHASE A/C' and Status = 'CREDIT' "
                             + " )Balance)Balance  Where Amt<PAmt Order by Name,_DueDays,Description";
                }
                else
                {
                    strQuery += " Select * from (Select *, (SUM(Amount) OVER(PARTITION BY Name))  AS PAmt from (Select (AccountID+' '+Name)Name,DueDays,DATEDIFF(dd,(DATEADD(dd,CAST(DueDays as int),Date)),'" + _date.ToString("MM/dd/yyyy") + "')_DueDays,(Convert(varchar,Date,103))_Date,Description,AccountStatus,CAST(Amount as Money)Amount,Status, "
                           + " (Select SUM(_Amt)Amt from(Select SUM(CAST(_BA.Amount as money))_Amt from BalanceAMount _BA Where Tick = 'FALSE' and _BA.Status = 'DEBIT' and _BA.AccountID = BA.AccountID UNION ALL "
                           + " Select -SUM(CAST(_BA.Amount as money))_Amt from BalanceAMount _BA Where Tick = 'FALSE' and _BA.AccountStatus != BA.AccountStatus and _BA.Description != BA.Description and _BA.Status = 'CREDIT' and _BA.AccountID = BA.AccountID)_Balance)Amt "
                           + " from BalanceAmount BA Cross APPLY (Select Name, DueDays from SupplierMaster SM Where (SM.AreaCode + AccountNo) = BA.AccountID  " + strSubQuery + " ) SM Where Tick = 'FALSE' and AccountStatus = 'SALES A/C' and Status = 'DEBIT' "
                           + " )Balance)Balance  Where Amt<PAmt Order by Name,_DueDays,Description";
                }
                DataTable dt = dba.GetDataTable(strQuery);

                BindRecordWithData(dt);
            }
            catch { }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            try
            {
                GetDataFromDB();
            }
            catch { }
            btnSearch.Enabled = true;
            panelSearch.Visible = false;
        }

        private DataTable GenrateDataTable(DataTable _dt)
        {
            DataTable _dTable = new DataTable();
            try
            {
                DataTable dt = _dt.DefaultView.ToTable(true, "Name", "Amt");
                double dPurchaseAmt = 0, dNetAmt = 0;
                foreach(DataRow row in dt.Rows)
                {
                    dNetAmt = dba.ConvertObjectToDouble(row["Amt"]);
                    DataRow[] _rows = _dt.Select("Name='" + row["Name"] + "' and Amt=" + dNetAmt);
                    DataTable _table = _rows.CopyToDataTable();
                    DataView _dv = _table.DefaultView;
                    _dv.Sort = "_DueDays desc,Description desc";

                    _table = _dv.ToTable();

                    foreach (DataRow _dr in _table.Rows)
                    {
                        dPurchaseAmt = dba.ConvertObjectToDouble(_dr["Amount"]);
                        dNetAmt = dPurchaseAmt- dNetAmt;
                        DataRow[] __dr = _dt.Select("Description='" + _dr["Description"] + "' and Name='" + _dr["Name"] + "' ");
                        if(__dr.Length>0)
                             __dr[0]["Amount"] = dNetAmt;
                        //_dr["Amount"] = dNetAmt;
                        if (dNetAmt >= 0)
                            break;
                        dNetAmt = Math.Abs(dNetAmt);
                    }
                }

                double _dDueDays = dba.ConvertObjectToDouble(txtDaysSlab.Text);

                DataRow[] _dRow = _dt.Select("Amount>0 and _DueDays >"+_dDueDays);
                _dTable = _dRow.CopyToDataTable();
                DataView dv = _dTable.AsDataView();
                dv.Sort = "Name asc,_DueDays desc,Description desc";

                _dTable = dv.ToTable();

            }
            catch { }
            return _dTable;
        }

        private void BindRecordWithData(DataTable _dt)
        {
            double dAmt = 0, dTAmt = 0;
            try
            {
                dgrdDetails.Rows.Clear();

                DataTable _table = GenrateDataTable(_dt);

                if (_table.Rows.Count > 0)
                {
                    int _index = 0;
                    dgrdDetails.Rows.Add(_table.Rows.Count);
                    foreach (DataRow row in _table.Rows)
                    {
                        dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                        dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1);
                        dgrdDetails.Rows[_index].Cells["partyName"].Value = row["Name"];
                        dgrdDetails.Rows[_index].Cells["dueDays"].Value = row["_DueDays"];
                        dgrdDetails.Rows[_index].Cells["purchaseBIll"].Value = row["Description"];
                        dgrdDetails.Rows[_index].Cells["billAmt"].Value = dAmt;
                        dgrdDetails.Rows[_index].Cells["billDate"].Value = row["_Date"];
                        dgrdDetails.Rows[_index].Cells["graceDays"].Value = row["DueDays"];
                        _index++;
                    }
                }
            }
            catch { }
            if (rdoSales.Checked)
            {
                lblDebit.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblBalAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            }
            else
            {
                lblCredit.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblBalAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy) + " Cr";
            }
        }

        private void txtDaysSlab_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    dba.KeyHandlerPoint(sender, e, 0);
            }
            catch { }
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 2)
                    {
                        string strValue = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strValue != "")
                        {
                            PurchaseOutstandingSlip _slip = new PurchaseOutstandingSlip(strValue);
                            _slip.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            _slip.ShowDialog();
                        }
                    }
                    else if (e.ColumnIndex == 5)
                    {
                        string strValue = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strValue != "")
                        {
                            ShowBillDetails(strValue);
                        }
                    }
                }
            }
            catch { }
        }

        private void ShowBillDetails(string strValue)
        {
            try
            {
                string[] strInvoice = strValue.Split(' ');
                if (strInvoice.Length > 1)
                {
                    if (strInvoice[0].Contains("PTN"))
                    {
                        PurchaseBook_Trading objPurchase = new PurchaseBook_Trading(strInvoice[0], strInvoice[1]);
                        objPurchase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                        objPurchase.Show();
                    }
                    else
                    {
                        GoodscumPurchase objGoods = new GoodscumPurchase(strInvoice[0], strInvoice[1]);
                        objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                        objGoods.Show();
                    }
                }
            }
            catch { }
        }

        private void txtLastDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtLastDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, false);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            int _index = 1;
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["sNo"].Value = _index;
                    _index++;
                }
            }
            catch { }
        }

        private void ShowPartyDetailsWithDueDays_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }
    }
}
