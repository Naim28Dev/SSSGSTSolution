using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class DebitorsCreditorsAccount : Form
    {
        DataBaseAccess dba;
        public DebitorsCreditorsAccount()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            MainPage.multiQSDate = MainPage.startFinDate;
            MainPage.multiQSDate = MainPage.endFinDate;

        }

        public DebitorsCreditorsAccount(bool mStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            if (mStatus)
            {
                btnSelect.Enabled = true;
                GetMultiQuarterName();
            }
        }

        private void txtBalance_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void DebitorsCreditorsAccount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelCompany.Visible)
                    panelCompany.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void chkDDate_CheckedChanged(object sender, EventArgs e)
        {
            txtDFromDate.Enabled = txtDToDate.Enabled = chkDDate.Checked;
            txtDFromDate.Text =  MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtDToDate.Text =  MainPage.multiQEDate.ToString("dd/MM/yyyy");
            ClearRecord();
        }

        private void chkCDate_CheckedChanged(object sender, EventArgs e)
        {
            txtCFromDate.Enabled = txtCToDate.Enabled = chkCDate.Checked;
            txtCFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtCToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            ClearRecord();
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
                    ClearRecord();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

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
                if (chkDDate.Checked && (txtDFromDate.Text.Length!=10 || txtDToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Debit Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDDate.Focus();
                }
                else if (chkCDate.Checked && (txtCFromDate.Text.Length != 10 || txtCToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Credit Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkCDate.Focus();
                }
                else
                {
                    if (btnSelect.Enabled)
                        GetMultiQuarterDetails();
                    else
                        GetCurrentQuarterDetails();
                    panelCompany.Visible = false;
                }
            }
            catch
            {
            }
        }

        private string CreateQuery(ref string strDQuery, ref string strCQuery, ref string strSQuery)
        {
            string strQuery = "";
            if (txtGroupName.Text!="")
                strQuery += " and SM.GroupName='"+ txtGroupName.Text+"' ";
            if (txtCategory.Text != "")
                strQuery += " and SM.Category='" + txtCategory.Text + "' ";
            if (txtBranchCode.Text != "")
                strQuery += " and SM.AreaCode='" + txtBranchCode.Text + "' ";

            if (txtCityName.Text != "")
                strQuery += " and SM.Station='" + txtCityName.Text + "' ";

            if (chkDDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtDFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtDToDate.Text).AddDays(1);
                strDQuery += " and (BA.Status='DEBIT' and BA.Date>='" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and  BA.Date<'" + eDate.ToString("MM/dd/yyyy h:mm:ss tt") + "') ";
            }
            if (chkCDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtCFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtCToDate.Text).AddDays(1);
                strCQuery += " and (BA.Status='CREDIT' and BA.Date>='" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and  BA.Date<'" + eDate.ToString("MM/dd/yyyy h:mm:ss tt") + "') ";
            }

            //if (txtAmount.Text != "")
            //{
            //    if (rdoDr.Checked)
            //        strSQuery = " Where Amount>" + txtAmount.Text;
            //    else if (rdoCr.Checked)
            //        strSQuery = " Where (Amount*-1)>" + txtAmount.Text;
            //    else
            //        strSQuery = " Where (Amount>" + txtAmount.Text + " OR (Amount*-1)>" + txtAmount.Text + " ) ";
            //}

            return strQuery;
        }

        private void ClearRecord()
        {
            dgrdAccount.Rows.Clear();
            lblBalAmount.Text = lblCredit.Text = lblDebit.Text = "0.00";
        }

        public void GetCurrentQuarterDetails()
        {
            ClearRecord();
            string strQuery = "", strDQuery = "", strCQuery = "", strSQuery = "", strSubQuery = CreateQuery(ref strDQuery, ref strCQuery, ref strSQuery);

            strQuery = " Select PartyName,MobileNo,Station,Reference,Amount,Name,AccountID from ( Select AccountID,PartyName,Name,MobileNo,Station,Reference, ISNULL(Sum(Amt),0) Amount from ("
                          + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name,Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BA.Status='DEBIT' " + strSubQuery + strDQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name Union ALL "
                          + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name,-Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BA.Status='CREDIT' " + strSubQuery + strCQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name) Balance "
                          + " Group By PartyName,MobileNo,Station,Reference,Name,AccountID) Bal " + strSQuery + " Order by Name";

            DataTable dt = dba.GetDataTable(strQuery);
            BindDetailRecords(dt);
        }

        private DataRow GetDataRowFromTable(DataTable dt, string strName)
        {
            DataRow row = null;
            DataRow[] rows = dt.Select("AccountID='" + strName + "'");
            if (rows.Length > 0)
                row = rows[0];
            return row;
        }

        private void BindDetailRecords(DataTable dt)
        {           

            dgrdAccount.Rows.Clear();
            lblBalAmount.Text = lblCredit.Text = lblDebit.Text = "";
            double dCreditAmt = 0, dDebitAmt = 0, dAmt = 0;
            int rowIndex = 0;
            if (dt.Rows.Count > 0)
            {                
                DataTable _dt = dt.DefaultView.ToTable(true, "AccountID");
                if (_dt.Rows.Count > 0)
                {
                    DataView _dv = _dt.DefaultView;
                    _dv.Sort = "AccountID";
                    _dt = _dv.ToTable();
                    foreach (DataRow rows in _dt.Rows)
                    {

                        dAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(Amount)", "AccountID='" + rows["AccountID"] + "' "));
                        if (ValidateAmt(dAmt))
                        {
                            DataRow row = GetDataRowFromTable(dt, Convert.ToString(rows["AccountID"]));
                            if (row != null)
                            {
                                dgrdAccount.Rows.Add();
                                if (dAmt >= 0)
                                {
                                    dgrdAccount.Rows[rowIndex].Cells["debit"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                    dDebitAmt += dAmt;
                                }
                                else if (dAmt < 0)
                                {
                                    dgrdAccount.Rows[rowIndex].Cells["credit"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                                    dCreditAmt += dAmt;
                                }

                                dgrdAccount.Rows[rowIndex].Cells["sno"].Value = rowIndex + 1 + ".";
                                dgrdAccount.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                                dgrdAccount.Rows[rowIndex].Cells["mobileNo"].Value = row["MobileNo"];
                                dgrdAccount.Rows[rowIndex].Cells["city"].Value = row["Station"];
                                dgrdAccount.Rows[rowIndex].Cells["reference"].Value = row["Reference"];
                                rowIndex++;
                            }
                        }
                    }
                }
            }
            dAmt = dDebitAmt + dCreditAmt;
            if (dAmt >= 0)
                lblBalAmount.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else
                lblBalAmount.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";

            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = Math.Abs(dCreditAmt).ToString("N2", MainPage.indianCurancy);
        }

        private bool ValidateAmt(double dAmt)
        {
            if (txtAmount.Text != "")
            {
                double dValue = dba.ConvertObjectToDouble(txtAmount.Text);
                if (rdoAll.Checked)
                {
                    if (dAmt > dValue || dAmt < (dValue * -1))
                        return true;
                }
                else if (rdoDr.Checked)
                {
                    if (dAmt > dValue)
                        return true;
                }
                else
                {
                    if (dAmt < (dValue * -1))
                        return true;
                }
            }
            else
            {
                if (rdoDr.Checked)
                {
                    if (dAmt > 0)
                        return true;
                }
                else if (rdoCr.Checked)
                {
                    if (dAmt < 0)
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        private void DebitorsCreditorsAccount_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdAccount);
            if (MainPage.strUserRole.Contains("ADMIN"))            
                dgrdAccount.Columns["mobileNo"].Visible = dgrdAccount.Columns["reference"].Visible = dgrdAccount.Columns["city"].Visible = true;
                     
            if (!btnSelect.Enabled)
            {
                MainPage.multiQSDate = MainPage.startFinDate;
                MainPage.multiQEDate = MainPage.endFinDate;
            }
            chkDDate.Checked = chkCDate.Checked = true;
            txtDFromDate.Text = txtCFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtDToDate.Text = txtCToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

            if (txtGroupName.Text != "")
                btnGo.PerformClick();
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
                txtDFromDate.Text = txtCFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtDToDate.Text = txtCToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

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
                txtDFromDate.Text = txtCFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtDToDate.Text = txtCToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public void GetMultiQuarterDetails()
        {
            ClearRecord();
            try
            {
                string strQuery = "", strDQuery = "", strCQuery = "", strSQuery = "", strSubQuery = CreateQuery(ref strDQuery, ref strCQuery, ref strSQuery), strOpeningQuery = "", strCompanyCode = "";

                strQuery = " Select PartyName,MobileNo,Station,Reference, Amount,Name,AccountID from ( Select AccountID,PartyName,Name,MobileNo,Station,Reference, ISNULL(Sum(Amt),0) Amount from ("
                              + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name, Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where AccountStatus !='OPENING' and BA.Status='DEBIT' " + strSubQuery + strDQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name Union ALL "
                              + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name,-Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where AccountStatus !='OPENING' and BA.Status='CREDIT' " + strSubQuery + strCQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name) Balance "
                              + " Group By PartyName,MobileNo,Station,Reference,Name,AccountID) Bal " + strSQuery + " Order by Name";


                strOpeningQuery = " Select PartyName,MobileNo,Station,Reference, Amount,Name,AccountID from ( Select AccountID,PartyName,Name,MobileNo,Station,Reference, ISNULL(Sum(Amt),0) Amount from ("
                              + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name,Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BA.Status='DEBIT' " + strSubQuery + strDQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name Union ALL "
                              + " Select BA.AccountID,(BA.AccountID+' '+SM.Name) as PartyName,SM.Name,-Sum(Cast(Amount as Money)) Amt,SM.MobileNo,SM.Station,SM.Reference from BalanceAmount BA inner Join SupplierMaster SM on BA.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BA.Status='CREDIT' " + strSubQuery + strCQuery + " Group By BA.AccountID,SM.MobileNo,SM.Station,SM.Reference,SM.Name) Balance "
                              + " Group By PartyName,MobileNo,Station,Reference,Name,AccountID) Bal " + strSQuery + " Order by Name";

                DataTable table = null;

                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                        if (strCompanyCode != "")
                        {
                            DataTable dt = null;
                            if (rowCount == 0)
                                table = dba.GetMultiQuarterDataTable(strOpeningQuery, strCompanyCode);
                            else
                            {
                                dt = dba.GetMultiQuarterDataTable(strQuery, strCompanyCode);
                                if (table == null)
                                    table = dt;
                                else if (dt != null)
                                    table.Merge(dt, true);
                            }
                            rowCount++;
                        }
                    }
                }

                if (table != null)
                    BindDetailRecords(table);
            }
            catch
            {
            }
        }

        #endregion

        private void txtDFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDDate.Checked, false, false, true);
        }

        private void txtCFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkCDate.Checked, false, false, true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("PartyType", typeof(String));
                myDataTable.Columns.Add("DebitorsDate", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("MobileNo", typeof(String));
                myDataTable.Columns.Add("City", typeof(String));
                myDataTable.Columns.Add("TotalDebitAmt", typeof(String));
                myDataTable.Columns.Add("TotalCreditAmt", typeof(String));
                myDataTable.Columns.Add("TotalAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("Reference", typeof(String));

                string strAccount = "", strDate = "";
                if (txtGroupName.Text!="")
                    strAccount = txtGroupName.Text+" BALANCE DETAIL";
                else
                    strAccount = "PARTY BALANCE DETAIL";

                if (txtCityName.Text != "")
                    strAccount += "  " + txtCityName.Text;
                if (chkDDate.Checked)
                    strDate = "Debit Date From : " + txtDFromDate.Text + " To " + txtDToDate.Text;
                if (chkCDate.Checked)
                {
                    if (strDate != "")
                        strDate += " & ";
                    strDate += " Credit Date From : " + txtCFromDate.Text + " To " + txtCToDate.Text;
                }

                foreach (DataGridViewRow row in dgrdAccount.Rows)
                {
                    DataRow dRow = myDataTable.NewRow();

                    dRow["CompanyName"] = MainPage.strPrintComapanyName;
                    dRow["PartyType"] = strAccount;
                    dRow["DebitorsDate"] = strDate;
                    dRow["PartyName"] = row.Cells["partyName"].Value;
                    dRow["DebitAmt"] = row.Cells["debit"].Value;
                    dRow["CreditAmt"] = row.Cells["credit"].Value;
                    dRow["MobileNo"] = row.Cells["mobileNo"].Value;
                    dRow["City"] = row.Cells["city"].Value;
                    if (chkReference.Checked)
                        dRow["Reference"] = row.Cells["reference"].Value;
                    dRow["TotalDebitAmt"] = lblDebit.Text;
                    dRow["TotalCreditAmt"] = lblCredit.Text;
                    dRow["TotalAmt"] = lblBalAmount.Text;
                    dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(dRow);
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdAccount.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    Reporting.CreditorsReport objReport = new Reporting.CreditorsReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("Debitors / Creditors Detail Preview");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();

                    objReport.Close();
                    objReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! No record found ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.CreditorsReport objReport = new Reporting.CreditorsReport();
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
                else
                    MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.CreditorsReport objReport = new Reporting.CreditorsReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.myPreview.ExportReport();

                    objReport.Close();
                    objReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\CreditorsAndDebtors";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\CreditorsAndDebtors.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.CreditorsReport objRegister = new Reporting.CreditorsReport();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                    objRegister.Close();
                    objRegister.Dispose();
                }
                else
                    strFileName = "";
            }
            catch
            {
                strFileName = "";
            }
            return strFileName;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (dgrdAccount.Rows.Count > 0)
                {
                    string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    //if (strEmailID != "")
                    //{
                    strPath = CreatePDFFile();
                    if (strPath != "")
                    {
                        strSubject = "CREDITORS AND DEBTORS ACCOUNT FROM " + MainPage.strCompanyName;
                        strBody = "We are sending Creditors and debtors account details, which is Attached with this mail, Please Find it.";
                        SendingEmailPage objEmail = new SendingEmailPage(true, "", "", strSubject, strBody, strPath, "", "CREDITORS AND DEBTORS ACCOUNT");
                        objEmail.ShowDialog();
                    }
                    //}
                }
                else
                    MessageBox.Show("Sorry ! No record found", "Items required", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }

        private void rdoDebtors_CheckedChanged(object sender, EventArgs e)
        {
            ClearRecord();
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

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnBranch_Click(object sender, EventArgs e)
        {
            try
            {
                btnBranch.Enabled = false;
                   SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", Keys.Space);
                objSearch.ShowDialog();
                txtBranchCode.Text = objSearch.strSelectedData;
                btnBranch.Enabled = true;
            }
            catch
            {
            }
        }

        private void dgrdAccount_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdAccount.Rows)
                    row.Cells["sno"].Value = _index++;

            }
            catch { }
        }
    }
}
