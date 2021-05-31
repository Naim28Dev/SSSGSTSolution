using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class AdjustMultiFinancialYear : Form
    {
        DataBaseAccess dba;
        string[] strColor = { "LightSteelBlue", "PeachPuff", "Thistle", "Lavender", "LightSalmon", "LightCoral", "ButtonShadow", "BurlyWood", "Gainsboro", "Beige" };

        public AdjustMultiFinancialYear()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            btnSelectCompany.Enabled = true;
            GetMultiQuarterName();
        }


        #region Multi Company

        private void GetMultiQuarterName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";
                dgrdCompany.Rows.Clear();
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    int rowIndex = 0;
                    string[] sFolder = Directory.GetDirectories(strPath);
                    DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                    foreach (string folderName in sFolder)
                    {
                        string[] strFile = Directory.GetFiles(folderName, "*.syber");
                        if (strFile.Length > 0)
                        {
                            FileInfo objFile = new FileInfo(folderName);
                            DataTable dt = dba.GetMultiCompanyNameAndFinDate(objFile.Name);
                            if (dt.Rows.Count > 0)
                            {
                                dgrdCompany.Rows.Add();
                                sDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["SDate"]));
                                eDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["EDate"]));
                                dgrdCompany.Rows[rowIndex].Cells["companyCheck"].Value = (Boolean)true;
                                dgrdCompany.Rows[rowIndex].Cells["code"].Value = dt.Rows[0]["CCode"];
                                dgrdCompany.Rows[rowIndex].Cells["companyName"].Value = dt.Rows[0]["CompanyName"];
                                dgrdCompany.Rows[rowIndex].Cells["sTextDate"].Value = dt.Rows[0]["SDate"];
                                dgrdCompany.Rows[rowIndex].Cells["eTextDate"].Value = dt.Rows[0]["EDate"];
                                dgrdCompany.Rows[rowIndex].Cells["startDate"].Value = sDate;
                                dgrdCompany.Rows[rowIndex].Cells["endDate"].Value = eDate;
                                if (MainPage.multiQSDate > sDate)
                                    MainPage.multiQSDate = sDate;
                                if (MainPage.multiQEDate < eDate)
                                    MainPage.multiQEDate = eDate;
                                rowIndex++;
                            }
                        }
                    }
                }
                dgrdCompany.Sort(dgrdCompany.Columns["startDate"], ListSortDirection.Ascending);
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

                MainPage.con.Close();
                MainPage.OpenConnection();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in MultiLedger Merging ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnSelectCompany_Click(object sender, EventArgs e)
        {
            if (!panelCompany.Visible)
            {
                panelCompany.Visible = true;
                dgrdCompany.Focus();
            }
            else
            {
                panelCompany.Visible = false;
            }
        }

        private void dgrdCompany_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                GetSelectedQuarterDate();
            }
        }

        private void GetSelectedQuarterDate()
        {
            try
            {
                DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                MainPage.multiQSDate = DateTime.Today;
                MainPage.multiQEDate = DateTime.Today;
                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["sTextDate"].Value));
                        eDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["eTextDate"].Value));
                        if (rowCount == 0)
                        {
                            MainPage.multiQSDate = sDate;
                            MainPage.multiQEDate = eDate;
                        }
                        else
                        {
                            if (MainPage.multiQSDate > sDate)
                                MainPage.multiQSDate = sDate;
                            if (MainPage.multiQEDate < eDate)
                                MainPage.multiQEDate = eDate;
                        }
                        rowCount++;
                    }
                }
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public void GetMultiQuarterDetails()
        {
            //   ClearRecord();
            try
            {
                string strPartyID = "", strQuery = "", strCompanyCode = "";

                string[] str = txtParty.Text.Split(' ');
                if (str.Length > 1)
                    strPartyID = str[0];

                DataTable table = null;
                DateTime dSDate = MainPage.multiQSDate, dEDate = MainPage.multiQEDate.AddDays(1);
                if (chkDate.Checked)
                {
                    dSDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    dEDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                }

                strQuery = "Select '[CCODE]' as CCode,BalanceID,CONVERT(varchar,Date,103)_Date,Date,Description,Status,Amount,(CASE When AccountStatus='SALES A/C' OR AccountStatus='PURCHASE A/C' OR AccountStatus='SALE RETURN' OR AccountStatus='PURCHASE RETURN' OR AccountStatus='JOURNAL A/C' OR AccountStatus='SALE SERVICE' OR AccountStatus='CREDIT NOTE' OR AccountStatus='DEBIT NOTE' OR AccountStatus='TCS CREDIT NOTE' OR AccountStatus='TCS DEBIT NOTE' OR AccountStatus='DUTIES & TAXES' OR AccountStatus='OPENING' then AccountStatus else dbo.GetFullName(AccountStatusID) end+(CASE When VoucherCode!='' then ' | '+VoucherCode+' '+CAST(VoucherNo as varchar) WHEN AccountStatus='SALE SERVICE' then ' | '+Description else '' end)) NAccountStatus from BalanceAmount where AccountID='" + strPartyID + "' and Tick='False' and Cast(Amount as Money)>0 and Date >='" + dSDate.ToString("MM/dd/yyyy") + "' and Date <'" + dEDate.ToString("MM/dd/yyyy") + "' order by Date  ";

                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                        if (strCompanyCode != "")
                        {
                            DataTable dt = dba.GetMultiQuarterDataTable(strQuery.Replace("[CCODE]", strCompanyCode), strCompanyCode);

                            if (table == null)
                                table = dt;
                            else if (dt != null)
                                table.Merge(dt, true);

                        }
                    }
                }

                if (table != null)
                    SetRecordWithDataTable(table);
            }
            catch
            {
            }
        }

        #endregion

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            SearchRecord();
            btnGo.Enabled = true;
        }

        private void SearchRecord()
        {
            try
            {
                if (txtParty.Text == "")
                {
                    MessageBox.Show("Sorry ! Party name can't be blank ! ", "Party name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParty.Focus();
                }
                else if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else
                {
                    dgrdLedger.Rows.Clear();
                    lblCredit.Text = lblDebit.Text =lblBalanceAmt.Text= "0.00";
                    chkAll.Checked = false;
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select TINNumber as Category from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtParty.Text + "' ");
                    bool _bPrevilegeAccount = false;
                    if (Convert.ToString(objValue) == "PREVILEGE ACCOUNT")
                        _bPrevilegeAccount = true;

                    if (_bPrevilegeAccount && !MainPage.mymainObject.bPrivilegeAccount)
                    {
                        MessageBox.Show("Sorry ! This account is in previlege category.\nContact to administrator.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtParty.Focus();
                    }
                    else
                    {
                        GetMultiQuarterDetails();
                    }
                    panelCompany.Visible = false;
                }
            }
            catch
            {
            }
        }

        private void SetRecordWithDataTable(DataTable dt)
        {
            double dDebitAmt = 0, dCreditAmt = 0, dBalanceAmt = 0, dAmt = 0;

            if (dt.Rows.Count > 0)
            {
                DataView _dv = dt.DefaultView;
                _dv.Sort = "Date asc";
                dt = _dv.ToTable();

                dgrdLedger.Rows.Add(dt.Rows.Count);
                int _index = 0,_colIndex=strColor.Length-1;
                string strStatus = "",strCCode="",strOldCode="";
                foreach (DataRow row in dt.Rows)
                {
                    dgrdLedger.Rows[_index].Cells["sno"].Value = (_index + 1);
                    dgrdLedger.Rows[_index].Cells["id"].Value = row["BalanceID"];
                    dgrdLedger.Rows[_index].Cells["date"].Value = row["_Date"];
                    dgrdLedger.Rows[_index].Cells["account"].Value = row["NAccountStatus"];
                    dgrdLedger.Rows[_index].Cells["desc"].Value = row["Description"];
                    dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                    strStatus = Convert.ToString(row["Status"]).ToUpper();
                    if (strStatus == "DEBIT")
                    {
                        dDebitAmt += dAmt;
                        dgrdLedger.Rows[_index].Cells["debit"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    }
                    else
                    {
                        dCreditAmt += dAmt;
                        dgrdLedger.Rows[_index].Cells["credit"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    }
                    dBalanceAmt = dDebitAmt - dCreditAmt;
                    if (dBalanceAmt >= 0)
                        dgrdLedger.Rows[_index].Cells["balance"].Value = dBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else
                        dgrdLedger.Rows[_index].Cells["balance"].Value = Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                    dgrdLedger.Rows[_index].Cells["tick"].Value = false;
                    strCCode = Convert.ToString(row["CCode"]);
                    dgrdLedger.Rows[_index].Cells["cCode"].Value = strCCode;
                    
                    if (strOldCode == "")
                        strOldCode = strCCode;

                    if (strCCode != strOldCode)
                    {
                        strOldCode = strCCode;                       
                        _colIndex--;
                    }
                    dgrdLedger.Rows[_index].DefaultCellStyle.BackColor = Color.FromName(strColor[_colIndex]);

                    _index++;
                }
            }
            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
            if (dBalanceAmt >= 0)
                lblBalanceAmt.Text = dBalanceAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else
                lblBalanceAmt.Text = Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    row.Cells["tick"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private DataTable CalculateDebitCreditAmt()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("CCode", typeof(String));
            
            try
            {
                double dDebitAmt = 0, dCreditAmt = 0,dAmt=0;
                int _index = 0,_tickIndex=0;
                string strAccountStatus = "",strCCode="",strBalanceID="";
                dgrdLedger.EndEdit();

                foreach (DataGridViewRow row in dgrdLedger.Rows)
                {
                    strAccountStatus = Convert.ToString(row.Cells["account"].Value);
                    strCCode = Convert.ToString(row.Cells["cCode"].Value);
                    strBalanceID = Convert.ToString(row.Cells["id"].Value);

                    if (strAccountStatus != "OPENING" || _index == 0)
                    {
                        if (Convert.ToBoolean(row.Cells["tick"].Value))
                        {
                            if (Convert.ToString(row.Cells["debit"].Value) != "")
                                dDebitAmt += dba.ConvertObjectToDouble(row.Cells["debit"].Value);
                            else
                                dCreditAmt += dba.ConvertObjectToDouble(row.Cells["credit"].Value);

                            DataRow _row = dt.NewRow();
                            _row["id"] = strBalanceID;
                            _row["CCode"] = strCCode;
                            dt.Rows.Add(_row);

                        }
                        _tickIndex++;
                    }
                    else if (_tickIndex == _index)
                    {
                        row.Cells["tick"].Value = true;
                        DataRow _row = dt.NewRow();
                        _row["id"] = strBalanceID;
                        _row["CCode"] = strCCode;
                        dt.Rows.Add(_row);
                    }
                    else
                        row.Cells["tick"].Value = false;
                    _index++;
                }

                dDebitAmt = Math.Round(dDebitAmt, 2);
                dCreditAmt = Math.Round(dCreditAmt, 2);
                dAmt = dDebitAmt - dCreditAmt;

                if (dAmt != 0)
                {
                    MessageBox.Show("Sorry ! Debit/Credit amt not matched. The amt diff : " + dAmt.ToString("N2", MainPage.indianCurancy), "Amt not matched", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dt = new DataTable();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dt = new DataTable();
            }
            return dt;
        }

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            try {
                DataTable dt = CalculateDebitCreditAmt();
                if (dt.Rows.Count > 0)
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to Adjust account ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        AdjustLedgerEntry(dt);
                    }
                }
            }
            catch { }
        }


        private void AdjustLedgerEntry(DataTable _dt)
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(MultiCompanyNo),0)+1)MNO from AdjustedIds ");
            int count = 0, result = 0;
            string strMultiNo = Convert.ToString(objValue);
            if (strMultiNo != "" && strMultiNo != "0")
            {
                DataTable table = _dt.DefaultView.ToTable(true, "CCode");
                string strBalanceID = "", strCCode = "", strAllBalanceID = "", strQuery = "", strAdjustedQuery = "";
                foreach (DataRow row in table.Rows)
                {
                    strCCode = Convert.ToString(row["CCode"]);
                    DataRow[] rows = _dt.Select("CCode='" + strCCode + "' ");
                    if (rows.Length > 0)
                    {
                        foreach (DataRow _dr in rows)
                        {
                            strBalanceID = Convert.ToString(_dr["id"]);
                            if (strBalanceID != "")
                            {
                                if (strAllBalanceID != "")
                                    strAllBalanceID += ",";
                                strAllBalanceID += strBalanceID;
                            }
                        }
                    }

                    strAdjustedQuery += CreateQueryForAdjustedID(strAllBalanceID, strCCode, strMultiNo);
                    strQuery += " Update BalanceAmount set Tick='True',UpdateStatus=1 where BalanceID in (" + strAllBalanceID + ") ";
                    result += count = dba.ExecuteQueryWithDataBase(strQuery + strAdjustedQuery, strCCode);
                    if (count > 0)
                        strQuery = strAllBalanceID = "";
                }
            }
            
            if (result > 0)
            {
                MessageBox.Show("Thanks ! Ledger adjusted successfully ! ", "Adjusted Successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                btnGo.PerformClick();
            }
            else
                MessageBox.Show("Sorry ! Record not adjusted, Please try again ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private string CreateQueryForAdjustedID(string strAdjustedID,string strDBName,string strNumber)
        {
            string strQuery = "";
            string[] strAllIDs = strAdjustedID.Split(',');
            if (strAllIDs.Length > 0)
            {
                foreach (string strID in strAllIDs)
                {
                    strQuery += " if not exists (Select ID from AdjustedIds Where DatabaseName='" + strDBName + "' and BalanceID=" + strID + ") begin "
                             + " Insert into AdjustedIds values('0'," + strID + ",'" + strDBName + "'," + strNumber + ",'" + MainPage.strLoginName + "',1,0) end ";
                }
            }
            return strQuery;
        }

        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);

                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtParty.Text = objSearch.strSelectedData;

                    dgrdLedger.Rows.Clear();
                    lblDebit.Text = lblCredit.Text = lblBalanceAmt.Text = "0.00";
                    txtParty.Focus();
                }
                else
                e.Handled = true;
            }
            catch
            {
            }

        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void dgrdCompany_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
                e.Cancel = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdjustMultiFinancialYear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {

            try
            {
                btnPartyName.Enabled = false;
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                    txtParty.Text = objSearch.strSelectedData;

                dgrdLedger.Rows.Clear();
                lblDebit.Text = lblCredit.Text = lblBalanceAmt.Text = "0.00";
                txtParty.Focus();
            }
            catch { }
            btnPartyName.Enabled = true;
        }
    }
}
