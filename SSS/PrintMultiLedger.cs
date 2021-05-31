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
    public partial class PrintMultiLedger : Form
    {
        DataBaseAccess dba;
        public PrintMultiLedger()
        {
            InitializeComponent();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            dba = new DataBaseAccess();
        }

        public PrintMultiLedger(string strGroupName)
        {
            InitializeComponent();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            dba = new DataBaseAccess();
            txtGroupName.Text = strGroupName;
            GetAllData();
        }

        private void PrintMultiLedger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    txtCityName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            ClearRecord();
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
            ClearRecord();
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["check"].Value = chkCheckAll.Checked;
            }
            catch
            {
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["all"].Value = chkAll.Checked;
                    row.Cells["tick"].Value = row.Cells["unTick"].Value = chkTick.Checked = chkUntick.Checked = false;
                }
            }
            catch
            {
            }
        }

        private void chkTick_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["tick"].Value = chkTick.Checked;
                    row.Cells["all"].Value = row.Cells["unTick"].Value = chkUntick.Checked = chkAll.Checked = false;
                }
            }
            catch
            {
            }
        }

        private void chkUntick_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["unTick"].Value = chkUntick.Checked;
                    row.Cells["tick"].Value = row.Cells["all"].Value =chkTick.Checked=chkAll.Checked= false;
                }
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length!=10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetAllData();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery(ref string strSQuery, ref string strSupplierQuery)
        {
            string strQuery = "";
            if (chkDate.Checked)
            {
                DateTime endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                strQuery += " and Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
            }
            if (txtGroupName.Text != "")
            {
                strQuery += " and AccountID in (Select (AreaCode+CAST(AccountNo as varchar)) from SupplierMaster Where GroupName='" + txtGroupName.Text + "') ";
                strSupplierQuery += " and GroupName='" + txtGroupName.Text + "' ";
            }
            if (txtCityName.Text != "")
            {
                strQuery += " and AccountID in (Select (AreaCode+CAST(AccountNo as varchar)) from SupplierMaster Where Station='" + txtCityName.Text + "') ";
                strSupplierQuery += " and Station='" + txtCityName.Text + "' ";
            }
            if (txtBranchCode.Text != "")
            {
                strQuery += " and AccountID in (Select (AreaCode+CAST(AccountNo as varchar)) from SupplierMaster Where AreaCode='" + txtBranchCode.Text + "') ";
                strSupplierQuery += " and AreaCode='" + txtBranchCode.Text + "' ";
            }
            if(txtStateName.Text!="")
            {
                strSupplierQuery += " and State='" + txtStateName.Text + "' ";
            }
            if (txtNickName.Text != "")
            {
                strSupplierQuery += " and Other='" + txtNickName.Text + "' ";
            }

            if (txtAmount.Text != "")
            {
                if (rdoDebit.Checked)
                    strSQuery = " Where Amount>" + txtAmount.Text;
                else if (rdoCredit.Checked)
                    strSQuery = " Where (Amount*-1)>" + txtAmount.Text;
                else
                    strSQuery = " Where Amount>" + txtAmount.Text+" OR (Amount*-1)>" + txtAmount.Text;
            }
            return strQuery;
        }

        private void GetAllData()
        {
            string strQuery = "", strSQuery = "",strSupplierQuery="", strSubQuery = CreateQuery(ref strSQuery,ref strSupplierQuery);
            strQuery = "Select Name,GroupName, Amount from (Select (PartyName+' '+Name) Name,GroupName, SUM(Amt) Amount from ( "
                          + " Select AccountID as PartyName,ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='DEBIT' " + strSubQuery+ " Group By AccountID Union All "
                          + " Select AccountID as PartyName,-ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount  Where Status='CREDIT' " + strSubQuery + " Group By AccountID "
                          + " )Balance CROSS APPLY (Select Name,GroupName from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))=PartyName " + strSupplierQuery + ")SM Group By PartyName,Name,GroupName) Bal " + strSQuery+" Order By Name";

            DataTable dt = dba.GetDataTable(strQuery);
            BindDataWithGrid(dt);
        }

        private void BindDataWithGrid(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            double dAmt = 0, dDebitAmt = 0, dCreditAmt = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);                
                foreach (DataRow row in dt.Rows)
                {
                    dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    if (dAmt >= 0)
                        dDebitAmt += dAmt;
                    else
                        dCreditAmt += dAmt;
                    dgrdDetails.Rows[rowIndex].Cells["check"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["all"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["tick"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["unTick"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["Name"];
                    dgrdDetails.Rows[rowIndex].Cells["group"].Value = row["GroupName"];
                    if(dAmt>=0)
                        dgrdDetails.Rows[rowIndex].Cells["debitAmt"].Value = dAmt.ToString("N2",MainPage.indianCurancy);
                    else
                        dgrdDetails.Rows[rowIndex].Cells["creditAmt"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                }
            }

            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = Math.Abs(dCreditAmt).ToString("N2", MainPage.indianCurancy);
            dAmt = dDebitAmt + dCreditAmt;
            if(dAmt>=0)
                lblBalAmount.Text = dAmt.ToString("N2", MainPage.indianCurancy)+ " Dr";
            else
                lblBalAmount.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 3)
                e.Cancel = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            try
            {
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("DateRange", typeof(String));
                table.Columns.Add("SerialNo", typeof(String));
                table.Columns.Add("PartyName", typeof(String));
                table.Columns.Add("Category", typeof(String));
                table.Columns.Add("GroupName", typeof(String));
                table.Columns.Add("Amount", typeof(String));
                table.Columns.Add("Status", typeof(String));
                table.Columns.Add("DebitAmt", typeof(String));
                table.Columns.Add("CreditAmt", typeof(String));
                table.Columns.Add("UserName", typeof(String));
                int serialNo = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow dRow = table.NewRow();
                    dRow["SerialNo"] = serialNo+".";
                    dRow["PartyName"] = row.Cells["partyName"].Value;
                    dRow["GroupName"] = row.Cells["group"].Value;
                    dRow["Amount"] = row.Cells["debitAmt"].Value;
                    dRow["Status"] = row.Cells["creditAmt"].Value;
                    dRow["UserName"] = MainPage.strLoginName + ",  " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    serialNo++;
                    table.Rows.Add(dRow);
                }

                if (table.Rows.Count > 0)
                {
                    table.Rows[0]["CompanyName"] = MainPage.strPrintComapanyName;
                    table.Rows[table.Rows.Count - 1]["DebitAmt"] = lblDebit.Text;
                    table.Rows[table.Rows.Count - 1]["CreditAmt"] = lblCredit.Text;
                    table.Rows[table.Rows.Count - 1]["DateRange"] = lblBalAmount.Text;
                }
            }
            catch
            {
            }
            return table;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {                   
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        SSS.Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Multi Party Ledger Report Preview");
                        SSS.Reporting.MultiPartyLedgerReport objReport = new Reporting.MultiPartyLedgerReport();
                        objReport.SetDataSource(dt);
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {                     
                        SSS.Reporting.MultiPartyLedgerReport objReport = new Reporting.MultiPartyLedgerReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }

                        objReport.Close();
                        objReport.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("There is no record for printing ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }


        private string[] GetSelectedPartyNameAndStatus(ref string[] strStatus)
        {
            List<string> lstPartyName = new List<string>();
            List<string> lstStatus = new List<string>();
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["check"].Value))
                    {
                        lstPartyName.Add(Convert.ToString(row.Cells["partyName"].Value));
                        if (Convert.ToBoolean(row.Cells["tick"].Value))
                            lstStatus.Add("True");
                        else if (Convert.ToBoolean(row.Cells["unTick"].Value))
                            lstStatus.Add("False");
                        else
                            lstStatus.Add("All");
                    }
                }
            }
            catch
            {
            }
            strStatus = lstStatus.ToArray();
            string[] strParty = lstPartyName.ToArray();
            return strParty;
        }

        private void btnPrintMultiLdeger_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                string[] strStatus = null, strPartyName = GetSelectedPartyNameAndStatus(ref strStatus);
                if (strPartyName.Length > 0)
                {
                    DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                        eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    }
                    LedgerAccount objLedger = new LedgerAccount(strPartyName, strStatus,sDate,eDate);
                    objLedger.MdiParent = MainPage.mymainObject;
                    objLedger.Show();
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }
        
        private void ClearRecord()
        {
            dgrdDetails.Rows.Clear();
            lblBalAmount.Text = lblCredit.Text = lblDebit.Text = "0.00";
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                try
                {
                    if (e.ColumnIndex == 4 && e.RowIndex >= 0)
                    {
                        string strParty = Convert.ToString(dgrdDetails.Rows[e.RowIndex].Cells["partyName"].Value);
                        if (strParty != "")
                        {
                            ShowLedgerAccount(strParty);
                        }
                    }
                }
                catch
                {
                }
            }
            catch { }
        }

        private void ShowLedgerAccount(string strParty)
        {
            try
            {
                if (strParty != "")
                {
                    SupplierMaster objSupplier = new SupplierMaster(strParty);
                    objSupplier.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                    objSupplier.ShowDialog();
                }
            }
            catch
            {
            }
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

        private void txtNickName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTYNICKNAME", "SEARCH NICK NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtNickName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }
    }
}
