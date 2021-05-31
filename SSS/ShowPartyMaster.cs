using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class ShowPartyMaster : Form
    {
        DataBaseAccess dba;
        public ShowPartyMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            rdoActiveTranasaction.Checked = true;
            dgrdParty.Columns["bankName"].Visible = dgrdParty.Columns["branchName"].Visible = dgrdParty.Columns["ifscCode"].Visible = dgrdParty.Columns["bankAccountName"].Visible = dgrdParty.Columns["accountNo"].Visible = dgrdParty.Columns["verifyStatus"].Visible = dgrdParty.Columns["beniID"].Visible = MainPage.mymainObject.bSupplierOtherDetails;
        }

        private void ShowPartyMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else if (panelSMS.Visible)
                    panelSMS.Visible = false;
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
                    SearchData objSearch = new SearchData("GROUPNAMEWITHSUBPARTY", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupName.Text = objSearch.strSelectedData;
                    dgrdParty.Rows.Clear();
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

        private void txtGroupII_KeyDown(object sender, KeyEventArgs e)
        {

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

        private void txtSTation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStation.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = btnSearch.Enabled = false;
            GetAllData();
            btnGo.Enabled = btnSearch.Enabled = true;
        }

        private string CreateQuery(ref string strAddressQuery)
        {
            string strQuery = "";
            if (txtGroupName.Text != "")
            {
                strQuery += " and GroupName='" + txtGroupName.Text + "' ";
                strAddressQuery += " and GroupName='" + txtGroupName.Text + "' ";
            }
            if (txtBranchCode.Text != "")
            {
                strQuery += " and AreaCode='" + txtBranchCode.Text + "' ";
                strAddressQuery += " and AreaCode Like('" + txtBranchCode.Text + "%') ";
            }
            if (txtCategory.Text != "")
                strQuery += " and Category ='" + txtCategory.Text + "' ";
            if (txtState.Text != "")
            {
                strQuery += " and State ='" + txtState.Text + "' ";
                strAddressQuery += " and State='" + txtState.Text + "' ";
            }
            if (txtStation.Text != "")
            {
                strQuery += " and Station ='" + txtStation.Text + "' ";
                strAddressQuery += " and City='" + txtStation.Text + "' ";
            }
            if (txtPartyName.Text != "")
            {
                strQuery += " and (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) ='" + txtPartyName.Text + "' ";
                strAddressQuery += " and (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name) ='" + txtPartyName.Text + "' ";
            }
            if (txtMobileNo.Text != "")
            {
                strQuery += " and MobileNo Like ('%" + txtMobileNo.Text + "%') ";
                strAddressQuery += " and MobileNo Like ('%" + txtMobileNo.Text + "%') ";
            }
            if (txtGSTNo.Text != "")
            {
                strQuery += " and GSTNo Like ('%" + txtGSTNo.Text + "%') ";
                strAddressQuery += " and GSTNo Like ('%" + txtGSTNo.Text + "%') ";
            }
            if (txtContactPerson.Text != "")
            {
                strQuery += " and ContactPerson Like ('%" + txtContactPerson.Text + "%') ";
                strAddressQuery += " and NickName Like ('%" + txtContactPerson.Text + "%') ";
            }
            if (txtReference.Text != "")
            {
                strQuery += " and Reference Like ('" + txtReference.Text + "') ";
                strAddressQuery += " and Reference Like ('" + txtReference.Text + "') ";
            }
            if (txtTransport.Text != "")
                strQuery += " and Transport ='" + txtTransport.Text + "' ";

            if (txtPartyType.Text != "")
            {
                strQuery += " and TINNumber ='" + txtPartyType.Text + "' ";
                strAddressQuery += " and GroupName Like ('" + txtPartyType.Text + "') ";
            }

            if (rdoBeniIDExists.Checked)
                strQuery += " and ISNULL(BeniID,'')!='' ";
            else if (rdoBeniIDNotExists.Checked)
                strQuery += " and ISNULL(BeniID,'')='' ";

            if (rdoBankVerify.Checked)
                strQuery += " and ISNULL(VStatus,'')='VERIFIED' ";
            else if (rdoBankNotVerified.Checked)
                strQuery += " and ISNULL(VStatus,'')='NOT VERIFIED' ";

            if (txtWhatsappNo.Text != "")
            {
                strQuery += " and ISNULL(WhatsappNo,'') Like('%" + txtWhatsappNo.Text + "%') ";
                strAddressQuery += " and WhatsappNo Like ('%" + txtWhatsappNo.Text + "%') ";
            }

            if (rdoActiveTranasaction.Checked)
                strQuery += " and TransactionLock =0 ";
            else if (rdoLockTransaction.Checked)
                strQuery += " and TransactionLock =1 ";

            if (rdoChqReceived.Checked)
                strQuery += " and ISNULL(ChqDate,'') !='' ";
            else if (rdoChqPending.Checked)
                strQuery += " and ISNULL(ChqDate,'') ='' ";


            if (rdoBlackList.Checked)
                strQuery += " and BlackList =1 ";
            else if (rdoActive.Checked)
                strQuery += " and BlackList =0 and ISNULL(Other1,'FALSE') ='FALSE' ";
            else if (rdoOrange.Checked)
                strQuery += " and Other1 ='TRUE' ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and SM.Date>='" + sDate.ToString("MM/dd/yyyy") + "' and SM.Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }


            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strAddressQuery = "", strSubQuery = CreateQuery(ref strAddressQuery);
                strQuery = " Select * from (";
                if (rdoBookAll.Checked || rdoAccountMaster.Checked)
                    strQuery += " Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)PartyName,Name,Reference,(Address+' '+Station+' '+ PINCode)Address,MobileNo,Station,GroupName,ISNULL(GSTNo,'')GSTNo,ISNULL(EmailID,'') EmailID,ContactPerson,Other as NickName,TINNumber as PartyType,_CD.ChqDate,ISNULL(SBD.BankName,'')BankName,ISNULL(SBD.BranchName,'')BranchName,ISNULL(SBD.BankIFSCCode,'')BankIFSCCode,ISNULL(SBD.BankAccountNo,'')BankAccountNo,ISNULL(SBD.BankAccountName,'')BankAccountName,ISNULL(VStatus,'')VStatus,ISNULL(SBD.BeniID,'')BeniID,SUBSTRING(ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+ REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(ISNULL(SBD.BankAccountName,''),' ',''),'&',''),':',''),',',''),'/',''),'-',''),'.',''),0,30) Final_Account,ISNULL(SOD.WhatsappNo,'')WhatsappNo,(Select SUM(Amt) from (Select ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='DEBIT' and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')) Union All Select -ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='CREDIT' and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,''))) Balance) BalanceAmt,AmountLimit,ExtendedAmt,Convert(varchar,SM.Date,103)Date from SupplierMaster SM  OUTER APPLY (Select TOP 1 SBD.BankName,SBD.BranchName,SBD.BankIFSCCode,(CASE WHEN SBD.BankAccountNo Like('0%') then ''''+SBD.BankAccountNo else SBD.BankAccountNo end) as BankAccountNo,SBD.BankAccountName,(CASE WHEN VerifiedStatus=1 then 'VERIFIED' else 'NOT VERIFIED' end) VStatus,SBD.BeniID from SupplierBankDetails SBD Where SM.AreaCode=SBD.AreaCode and SM.AccountnO=SBD.AccountNo Order by VerifiedStatus desc,ID desc) SBD  OUTER APPLY (Select TOP 1 WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountnO=SOD.AccountNo) SOD OUTER APPLY (Select TOP 1 CONVERT(varchar,MAX(CD.Date),103) ChqDate from ChequeDetails CD Where CD.CreditAccountID=(SM.AreaCode+SM.AccountNo) and ChequeType='SECURITY' and Status='PENDING')_CD Where Name!='' " + strSubQuery;
                if (rdoBookAll.Checked)
                    strQuery += "  UNION ALL ";
                if (rdoBookAll.Checked || rdoAddressBook.Checked)
                    strQuery += " Select (ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)PartyName,Name,Reference,(Address+' '+City+' '+ PINCode)Address,MobileNo,City as Station,'ADDRESS BOOK' as GroupName,ISNULL(GSTNo,'')GSTNo,ISNULL(EmailID,'') EmailID,NickName as ContactPerson,'' as NickName,GroupName as PartyType,'' as ChqDate,'' BankName,'' BranchName,'' as BankIFSCCode,'' as BankAccountNo,'' as BankAccountName,'' as VStatus,'' as BeniID,'' as Final_Account,ISNULL(AB.WhatsappNo,'')WhatsappNo,0 as BalanceAmt,0 as AmountLimit,'0' as ExtendedAmt,Convert(varchar,VisitedDate,103)Date from AddressBook AB  Where Name!='' " + strAddressQuery;

                strQuery += " )_Supplier Order by Name ";

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataTable(dt);
                panelSearch.Visible = false;
            }
            catch { }
        }

        private void BindDataTable(DataTable dt)
        {
            try
            {
                dgrdParty.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    chkAll.Checked = true;
                    dgrdParty.Rows.Add(dt.Rows.Count);
                    int rowIndex = 0;
                    double dAmt = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dAmt = dba.ConvertObjectToDouble(row["BalanceAmt"]);
                        dgrdParty.Rows[rowIndex].Cells["chk"].Value = true;
                        dgrdParty.Rows[rowIndex].Cells["sNo"].Value = (rowIndex + 1);
                        dgrdParty.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                        dgrdParty.Rows[rowIndex].Cells["address"].Value = Convert.ToString(row["Address"]).Replace("\n", " ").Replace("\r", " ");
                        dgrdParty.Rows[rowIndex].Cells["phone"].Value = row["MobileNo"];
                        dgrdParty.Rows[rowIndex].Cells["cityName"].Value = row["Station"];
                        dgrdParty.Rows[rowIndex].Cells["groupName"].Value = row["GroupName"];
                        dgrdParty.Rows[rowIndex].Cells["gstNo"].Value = row["GSTNo"];
                        dgrdParty.Rows[rowIndex].Cells["contactPerson"].Value = row["ContactPerson"];
                        if (dAmt > 0)
                            dgrdParty.Rows[rowIndex].Cells["balance"].Value = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else if (dAmt < 0)
                            dgrdParty.Rows[rowIndex].Cells["balance"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                        else
                            dgrdParty.Rows[rowIndex].Cells["balance"].Value = "0.00";

                        dgrdParty.Rows[rowIndex].Cells["bankName"].Value = row["BankName"];
                        dgrdParty.Rows[rowIndex].Cells["branchName"].Value = row["BranchName"];
                        dgrdParty.Rows[rowIndex].Cells["ifscCode"].Value = row["BankIFSCCode"];
                        dgrdParty.Rows[rowIndex].Cells["accountNo"].Value = row["BankAccountNo"];
                        dgrdParty.Rows[rowIndex].Cells["bankAccountName"].Value = row["BankAccountName"];
                        dgrdParty.Rows[rowIndex].Cells["verifyStatus"].Value = row["VStatus"];
                        dgrdParty.Rows[rowIndex].Cells["beniID"].Value = row["BeniID"];
                        dgrdParty.Rows[rowIndex].Cells["finalPartyName"].Value = row["Final_Account"];
                        dgrdParty.Rows[rowIndex].Cells["nickName"].Value = row["NickName"];
                        dgrdParty.Rows[rowIndex].Cells["reference"].Value = row["Reference"];
                        dgrdParty.Rows[rowIndex].Cells["partyType"].Value = row["PartyType"];
                        dgrdParty.Rows[rowIndex].Cells["whatsappNo"].Value = row["WhatsappNo"];
                        dgrdParty.Rows[rowIndex].Cells["securityCheque"].Value = row["ChqDate"];
                        dgrdParty.Rows[rowIndex].Cells["amtLimit"].Value = row["AmountLimit"];
                        dgrdParty.Rows[rowIndex].Cells["addLimit"].Value = row["ExtendedAmt"];
                        dgrdParty.Rows[rowIndex].Cells["date"].Value = row["date"];

                        if (Convert.ToString(row["VStatus"]) == "NOT VERIFIED")
                            dgrdParty.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                        else if (Convert.ToString(row["VStatus"]) == "VERIFIED")
                            dgrdParty.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;

                        if (Convert.ToString(row["GroupName"]) == "ADDRESS BOOK")
                            dgrdParty.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;
                        rowIndex++;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdParty_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.ColumnIndex != 5)
                e.Cancel = true;
        }

        private void dgrdParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2 && e.RowIndex >= 0)
                {
                    string strParty = Convert.ToString(dgrdParty.Rows[e.RowIndex].Cells["partyName"].Value);
                    string strGroup = Convert.ToString(dgrdParty.Rows[e.RowIndex].Cells["groupName"].Value);
                    if (strGroup == "ADDRESS BOOK")
                        ShowAddressBook(strParty);
                    else
                        ShowLedgerAccount(strParty);
                }
            }
            catch
            {
            }
        }

        private void ShowAddressBook(string strParty)
        {
            try
            {
                if (strParty != "")
                {

                    AddressBook objAddressBook = new AddressBook(strParty);
                    objAddressBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                    objAddressBook.ShowDialog();
                }
            }
            catch
            {
            }
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

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgrdParty.Rows)
                row.Cells["chk"].Value = chkAll.Checked;
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();

            try
            {
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("GroupName", typeof(String));
                table.Columns.Add("CityName", typeof(String));
                table.Columns.Add("Name", typeof(String));
                table.Columns.Add("Address", typeof(String));
                table.Columns.Add("PhoneNo", typeof(String));
                table.Columns.Add("Balance", typeof(String));
                table.Columns.Add("LoginName", typeof(String));

                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        DataRow dr = table.NewRow();
                        dr["CompanyName"] = MainPage.strPrintComapanyName;
                        if (txtGroupName.Text != "")
                            dr["GroupName"] = txtGroupName.Text + " DETAILS REGISTER";
                        else
                            dr["GroupName"] = "PARTY DETAILS REGISTER";
                        dr["CityName"] = "";
                        dr["Name"] = row.Cells["partyName"].Value;
                        dr["Address"] = row.Cells["address"].Value;
                        dr["PhoneNo"] = row.Cells["phone"].Value;
                        if (chkBalance.Checked)
                            dr["Balance"] = row.Cells["balance"].Value;
                        else
                            dr["Balance"] = "---------";
                        dr["LoginName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        table.Rows.Add(dr);
                    }
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
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PartyMasterReport objReport = new Reporting.PartyMasterReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PARTY REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PartyMasterReport objReport = new Reporting.PartyMasterReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnPrintPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPrintPartyName.Enabled = false;
                    DataTable dt = CreatePartyNameDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PartyNameReport objReport = new Reporting.PartyNameReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PARTY REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPrintPartyName.Enabled = true;
        }

        private DataTable CreatePartyNameDataTable()
        {
            DataTable table = new DataTable();
            int i = 0, j = 0, index = 1;
            DataRow dataRow = null;
            try
            {
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("LeftSNo", typeof(String));
                table.Columns.Add("LeftColumn", typeof(String));
                table.Columns.Add("RightSNo", typeof(String));
                table.Columns.Add("RightColumn", typeof(String));
                table.Columns.Add("UserName", typeof(String));
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    bool chkStatus = Convert.ToBoolean(row.Cells[0].Value);
                    if (chkStatus)
                    {
                        if (i == 0)
                        {
                            DataRow dr = table.NewRow();
                            dr["CompanyName"] = MainPage.strPrintComapanyName;
                            dr["LeftSNo"] = index + ".";
                            dr["LeftColumn"] = row.Cells["partyName"].Value;
                            dr["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                            dataRow = dr;
                            i++;
                            index++;
                        }
                        else
                        {
                            dataRow["RightSNo"] = index + ".";
                            dataRow["RightColumn"] = row.Cells["partyName"].Value;
                            table.Rows.Add(dataRow);
                            index++;
                            i = 0;
                        }
                    }
                }
                if (i != 0)
                {
                    table.Rows.Add(dataRow);
                }
            }
            catch
            {
            }
            return table;
        }
        private string GetSelectedPartyName()
        {
            string strParty = "";
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        string strName = Convert.ToString(row.Cells["partyName"].Value);
                        string[] strSafeName = strName.Split(' ');
                        if (strSafeName.Length > 1)
                        {
                            if (strParty != "")
                                strParty += ",";
                            strParty += "'" + strSafeName[0] + "' ";
                        }
                    }
                }
            }
            catch
            {
            }
            return strParty;
        }

        private DataTable CreateOfficeAddressDataTable()
        {
            DataTable newDataTable = new DataTable();
            try
            {
                string strQuery = "";

                strQuery = " Select Top 1 CompanyName as Name,Address,(StateName+'-'+PinCode)Station,(StdNo+'-'+PhoneNo)MobileNo from CompanyDetails  ";
                DataTable table = dba.GetDataTable(strQuery);

                newDataTable.Columns.Add("LeftColumn", typeof(String));
                newDataTable.Columns.Add("RightColumn", typeof(String));


                int index = 0;
                for (int _index = 0; _index < 16; _index++)
                {
                    DataRow row = newDataTable.NewRow();
                    row["LeftColumn"] = table.Rows[index]["Name"] + "\n" + Convert.ToString(table.Rows[index]["Address"]).ToUpper() + "\n" + table.Rows[index]["Station"] + "\nPhone No. : " + table.Rows[index]["MobileNo"];
                    //if (index + 1 < table.Rows.Count)
                    //{
                    row["RightColumn"] = table.Rows[index]["Name"] + "\n" + Convert.ToString(table.Rows[index]["Address"]).ToUpper() + "\n" + table.Rows[index]["Station"] + "\nPhone No. : " + table.Rows[index]["MobileNo"];
                    //  }
                    newDataTable.Rows.Add(row);
                    _index++;
                }
            }
            catch
            {
            }

            return newDataTable;
        }

        private DataTable CreateAddressDataTable()
        {
            DataTable newDataTable = new DataTable();
            try
            {
                string strQuery = "", strPartyName = GetSelectedPartyName();
                if (strPartyName != "")
                {
                    strQuery = " Select SM.* from (Select Distinct Other as SSSName from SupplierMaster Where (AreaCode+AccountNo) in (" + strPartyName + ") Group by Other)Supplier Outer Apply ( Select Top 1 ( CASE WHEN Name Like('M/S %') then Name else 'M/S '+Name end)as Name, Address, (Station+ '  '+PinCode) as Station ,MobileNo from SupplierMaster Where Other=SSSName  Order by Date desc )SM  UNION ALL Select  ( CASE WHEN Name Like('M/S %') then Name else 'M/S '+Name end)as Name, Address, (City+ '  '+PinCode) as Station ,MobileNo from AddressBook Where (AreaCode+AccountNo ) in (" + strPartyName + ") Order by Name ";
                    DataTable table = dba.GetDataTable(strQuery);

                    newDataTable.Columns.Add("LeftColumn", typeof(String));
                    newDataTable.Columns.Add("RightColumn", typeof(String));

                    for (int index = 0; index < table.Rows.Count; index++)
                    {
                        DataRow row = newDataTable.NewRow();
                        row["LeftColumn"] = table.Rows[index]["Name"] + "\n" + Convert.ToString(table.Rows[index]["Address"]).ToUpper() + "\nPO. : " + table.Rows[index]["Station"] + "\nMob. : " + table.Rows[index]["MobileNo"];
                        if (index + 1 < table.Rows.Count)
                        {
                            row["RightColumn"] = table.Rows[index + 1]["Name"] + "\n" + Convert.ToString(table.Rows[index + 1]["Address"]).ToUpper() + "\nPO. : " + table.Rows[index + 1]["Station"] + "\nMob. : " + table.Rows[index + 1]["MobileNo"];
                        }
                        newDataTable.Rows.Add(row);
                        index++;
                    }
                }
            }
            catch
            {
            }

            return newDataTable;
        }


        private void btnPreviewAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnPreviewAddress.Enabled = false;
                    DataTable dt = CreateAddressDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.StickerReport objReport = new Reporting.StickerReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PARTY ADDRESS PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPreviewAddress.Enabled = true;
        }

        private void btnPrintAddress_Click(object sender, EventArgs e)
        {
            btnPrintAddress.Enabled = false;
            try
            {
                //if (dgrdParty.Rows.Count > 0)
                //{

                DataTable dt = CreateOfficeAddressDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.StickerReport objReport = new Reporting.StickerReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("OFFICE ADDRESS PREVIEW");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();

                    objReport.Close();
                    objReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //}
            }
            catch
            {
            }
            btnPrintAddress.Enabled = true;

            //try
            //{
            //    if (dgrdParty.Rows.Count > 0)
            //    {
            //        btnPrintAddress.Enabled = false;
            //        DataTable dt = CreateAddressDataTable();
            //        if (dt.Rows.Count > 0)
            //        {
            //            Reporting.StickerReport objReport = new Reporting.StickerReport();
            //            objReport.SetDataSource(dt);
            //            PrintDialog dialog = new PrintDialog();
            //            dialog.AllowSomePages = true;
            //            dialog.PrinterSettings.FromPage = 1;
            //            dialog.PrinterSettings.ToPage = 1;
            //            DialogResult result = dialog.ShowDialog();
            //            if (result == DialogResult.OK)                        
            //                objReport.PrintToPrinter(1, false, dialog.PrinterSettings.FromPage, dialog.PrinterSettings.ToPage);                       
            //        }
            //        else
            //            MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //    }
            //}
            //catch
            //{
            //}
            //btnPrintAddress.Enabled = true;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            txtSMSMobileNo.Text = GetSelectedMobileNo();
            lblCharCount.Text = "Char Count : 0";
            lblSMSCount.Text = "SMS Count  : 0";
            panelSMS.Visible = true;
            btnSMS.Text = "&Send SMS";
            txtSMSMobileNo.Enabled = true;
            txtSMS.Clear();
            txtSMS.Focus();
        }

        private string GetSelectedMobileNo()
        {
            string strMobileNo = "";
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        string strMNo = Convert.ToString(row.Cells["phone"].Value);
                        if (strMNo != "")
                        {
                            if (!strMobileNo.Contains(strMNo))
                            {
                                if (strMobileNo == "")
                                    strMobileNo = "" + strMNo;
                                else
                                    strMobileNo += "," + strMNo;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return strMobileNo;
        }


        private void txtSMS_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lblCharCount.Text = "Char Count : " + txtSMS.Text.Length.ToString();
                if (txtSMS.Text.Length % 160 != 0)
                {
                    lblSMSCount.Text = "SMS Count : " + ((txtSMS.Text.Length / 160) + 1).ToString();
                }
                else
                {
                    lblSMSCount.Text = "SMS Count : " + (txtSMS.Text.Length / 160).ToString();
                }
            }
            catch
            {
                lblSMSCount.Text = "1";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSMS.Visible = false;
        }

        private void btnSMS_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendSMS.Enabled = btnSMS.Enabled = btnSendWhatsappMessage.Enabled = false;

                if (!txtSMSMobileNo.Enabled)
                {
                    SendWhatsappMessage();
                }
                else
                {
                    if (txtSMS.Text != "" && txtSMSMobileNo.Text.Length > 9)
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to Send SMS ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SendSMS sendMessage = new SendSMS();
                            string strResult = sendMessage.SendSingleSMS(txtSMS.Text, txtSMSMobileNo.Text);
                            if (strResult.Contains("success"))
                            {
                                MessageBox.Show("Message Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtSMS.Clear();
                                panelSMS.Visible = false;
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please Try Again  ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please fill the message box and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
            }
            btnSendSMS.Enabled = btnSMS.Enabled = btnSendWhatsappMessage.Enabled = true;
        }

        private void txtSMSMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == (Char)44)
                {
                    e.Handled = false;
                }
                else
                {
                    if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            catch
            {
            }
        }

        private void txtSMS_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        //private void chkAllScheme_CheckedChanged(object sender, EventArgs e)
        //{
        //    foreach (DataGridViewRow row in dgrdParty.Rows)
        //        row.Cells["chkScheme"].Value = chkAllScheme.Checked;
        //}

        //private void btnUpdate_Click(object sender, EventArgs e)
        //{
        //    btnUpdate.Enabled = false;
        //    try
        //    {
        //        if (dgrdParty.Rows.Count > 0)
        //        {
        //            DialogResult result = MessageBox.Show("Are you sure you want to update records ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //            if (result == DialogResult.Yes)
        //            {
        //                UpdateRecords();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string[] strReport = { "Exception occurred in update scheme in Show Party Master", ex.Message };
        //        dba.CreateErrorReports(strReport);
        //    }
        //    btnUpdate.Enabled = true;
        //}

        private void UpdateRecords()
        {
            string strDParty = "", strSelectedParty = GetSelectedPartyNameForUpdate(ref strDParty), strQuery = "";
            if (strSelectedParty != "")
                strQuery = " Update SupplierMaster Set Other='SCHEME' Where (AreaCode+CAST(AccountNo as varchar)+' '+Name) in (" + strSelectedParty + ") ";
            if (strDParty != "")
                strQuery += " Update SupplierMaster Set Other='' Where (AreaCode+CAST(AccountNo as varchar)+' '+Name) in (" + strDParty + ") ";

            if (strQuery != "")
            {
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record updated successfully ! ", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    ResetAfterUpdation();
                }
                else
                    MessageBox.Show("Sorry ! Please after some time ! ", "Record not updated", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSelectedPartyNameForUpdate(ref string strDeselectedParty)
        {
            string strSelectedParty = "";
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    string strName = Convert.ToString(row.Cells["partyName"].Value), strGroup = Convert.ToString(row.Cells["groupName"].Value);
                    if (strName != "" && strGroup != "RELATION")
                    {
                        if (Convert.ToBoolean(row.Cells["chkScheme"].Value))
                        {
                            if (strSelectedParty == "")
                                strSelectedParty = "'" + strName + "' ";
                            else
                                strSelectedParty += ",'" + strName + "' ";
                        }
                        else
                        {
                            if (strDeselectedParty == "")
                                strDeselectedParty = "'" + strName + "' ";
                            else
                                strDeselectedParty += ",'" + strName + "' ";
                        }
                    }
                }
            }
            catch
            {
            }
            return strSelectedParty;
        }

        private void ResetAfterUpdation()
        {
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkScheme"].Value))
                        row.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                    else
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                }
            }
            catch
            {
            }
        }

        private void txtGSTNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdParty_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkAll.Visible = false;
                    else
                        chkAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void txtContactPerson_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void ShowPartyMaster_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdParty);
        }

        private void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            //btnDownloadExcel.Enabled = false;
            //try
            //{
            //    DialogResult result = MessageBox.Show("Are you sure want to download ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (result == DialogResult.Yes)
            //    {
            //        WriteInExistingFile();
            //    }
            //}
            //catch { }
            //btnDownloadExcel.Enabled = true;
        }

        //private string CreateNormalExcel()
        //{
        //    NewExcel.Application ExcelApp = new NewExcel.Application();
        //    NewExcel.Workbook ExcelWorkBook = null;
        //    NewExcel.Worksheet ExcelWorkSheet = null;
        //    string strFileName = GetFileName();
        //    try
        //    {
        //        object misValue = System.Reflection.Missing.Value;
        //        ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
        //        ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
        //        ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
        //        ExcelWorkSheet.Name = "MASTER_DETAILS";

        //        ExcelWorkSheet.Cells[1, 1] = "Master Name";
        //        ExcelWorkSheet.Cells[1, 2] = "Bank Name";
        //        ExcelWorkSheet.Cells[1, 3] = "Branch Name";
        //        ExcelWorkSheet.Cells[1, 4] = "IFSC Code";
        //        ExcelWorkSheet.Cells[1, 5] = "Account No";
        //        ExcelWorkSheet.Cells[1, 6] = "Account Holder";
        //        ExcelWorkSheet.Cells[1, 7] = "Beni ID";
        //        ExcelWorkSheet.Cells[1, 8] = "Amount";
        //        ExcelWorkSheet.Cells[1, 9] = "Status";

        //        int columnIndex = 1;
        //        foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
        //        {
        //            column.HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;
        //            column.NumberFormat = "@";
        //            if (columnIndex == 1)
        //                column.ColumnWidth = (double)column.ColumnWidth + 16;
        //            else if (columnIndex == 2 || columnIndex == 3 || columnIndex == 4 || columnIndex == 5 || columnIndex == 6)
        //                column.ColumnWidth = (double)column.ColumnWidth + 12;
        //            else if (columnIndex == 7 || columnIndex == 8)
        //                column.ColumnWidth = (double)column.ColumnWidth + 8;

        //            if (columnIndex > 7)
        //                break;
        //            columnIndex++;
        //        }

        //        int rowIndex = 2;
        //        string strBalance = "";
        //        foreach (DataGridViewRow row in dgrdParty.Rows)
        //        {
        //            if (Convert.ToBoolean(row.Cells["chk"].Value))
        //            {
        //                ExcelWorkSheet.Cells[rowIndex, 1] = row.Cells["partyName"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 2] = row.Cells["bankName"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 3] = row.Cells["branchName"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 4] = row.Cells["ifscCode"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 5] = row.Cells["accountNo"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 6] = row.Cells["bankAccountName"].Value;
        //                ExcelWorkSheet.Cells[rowIndex, 7] = row.Cells["beniID"].Value;

        //                strBalance = Convert.ToString(row.Cells["balance"].Value);
        //                string[] strBal = strBalance.Split(' ');
        //                ExcelWorkSheet.Cells[rowIndex, 8] = strBal[0].Trim();
        //                if (strBal.Length > 1)
        //                    ExcelWorkSheet.Cells[rowIndex, 9] = strBal[1].Trim();

        //                rowIndex++;
        //            }
        //        }

        //        for (int cIndex = 1; cIndex < 10; cIndex++)
        //        {
        //            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, cIndex];
        //            objRange.Font.Bold = true;
        //            objRange.Interior.ColorIndex = 22;
        //        }

        //        for (int rIndex = 2; rIndex < rowIndex; rIndex++)
        //        {
        //            for (int cIndex = 1; cIndex < 10; cIndex++)
        //            {
        //                NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
        //                objRange.Cells.BorderAround();
        //            }
        //        }

        //        ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //        ExcelWorkBook.Close(true, misValue, misValue);
        //        ExcelApp.Quit();

        //        Marshal.ReleaseComObject(ExcelWorkSheet);
        //        Marshal.ReleaseComObject(ExcelWorkBook);
        //        Marshal.ReleaseComObject(ExcelApp);

        //        MessageBox.Show("Thanks ! Excel generated successfully ? ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //    }
        //    catch (Exception ex)
        //    {
        //        strFileName = ex.Message;
        //    }
        //    finally
        //    {
        //        foreach (Process process in Process.GetProcessesByName("Excel"))
        //            process.Kill();
        //    }
        //    return strFileName;
        //}

        private string WriteInExistingFile()
        {
            NewExcel.Application myExcelApplication;
            NewExcel.Workbook myExcelWorkbook;
            NewExcel.Worksheet myExcelWorkSheet;
            myExcelApplication = null;
            string excelFilePath = "";
            try
            {
                string strFileName = GetFileName();
                if (strFileName != "")
                {
                    myExcelApplication = new NewExcel.Application(); // create Excell App
                    myExcelApplication.DisplayAlerts = false; // turn off alerts

                    excelFilePath = GetPaymentFileFromServer();// MainPage.strServerPath + "\\Excel_File\\GSTR1_Template.xlsx";

                    myExcelWorkbook = (NewExcel.Workbook)(myExcelApplication.Workbooks._Open(excelFilePath, System.Reflection.Missing.Value,
                       System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                       System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                       System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                       System.Reflection.Missing.Value, System.Reflection.Missing.Value)); // open the existing excel file

                    int numberOfWorkbooks = myExcelApplication.Workbooks.Count; // get number of workbooks (optional)

                    myExcelWorkSheet = (NewExcel.Worksheet)myExcelWorkbook.Worksheets[2];

                    int _rowIndex = 5;
                    string strBankName = "", strTransType = "", strAmount = "";
                    double dAmount = 0;

                    myExcelWorkSheet.Cells[1, 3] = MainPage.strHeadOfficeBankAccountNo;
                    myExcelWorkSheet.Cells[2, 3] = MainPage.currentDate.ToString("dd/MM/yyyy");
                    foreach (DataGridViewRow row in dgrdParty.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["chk"].Value) && Convert.ToString(row.Cells["verifyStatus"].Value) == "VERIFIED")
                        {
                            strBankName = Convert.ToString(row.Cells["bankName"].Value);
                            strAmount = Convert.ToString(row.Cells["balance"].Value);
                            if (strAmount.Contains("Cr"))
                            {
                                string[] strAmt = strAmount.Split(' ');
                                if (strAmt.Length > 1)
                                    dAmount = dba.ConvertObjectToDouble(strAmt[0].Trim());
                            }

                            if (strBankName.Contains("ICICI"))
                                strTransType = "WIB";
                            else if (dAmount >= 200000)
                                strTransType = "RTG";
                            else
                                strTransType = "NFT";

                            myExcelWorkSheet.Cells[_rowIndex, 1] = strTransType;
                            myExcelWorkSheet.Cells[_rowIndex, 2] = MainPage.strHeadOfficeBankAccountNo;
                            myExcelWorkSheet.Cells[_rowIndex, 3] = row.Cells["beniID"].Value;
                            myExcelWorkSheet.Cells[_rowIndex, 4] = dAmount;
                            myExcelWorkSheet.Cells[_rowIndex, 5] = row.Cells["finalPartyName"].Value;

                            _rowIndex++;
                        }
                    }


                    //for (i = 0; i < Percentage.Count; i++)
                    //{
                    //    oSheet.Cells[i + 2, 2] = Percentage[i];
                    //}

                    myExcelWorkbook.SaveAs(strFileName, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                                  System.Reflection.Missing.Value, System.Reflection.Missing.Value, NewExcel.XlSaveAsAccessMode.xlNoChange,
                                                  System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                                  System.Reflection.Missing.Value, System.Reflection.Missing.Value); // Save data in excel


                    myExcelWorkbook.Close(true, excelFilePath, System.Reflection.Missing.Value);
                    myExcelWorkbook.Close(true, strFileName, System.Reflection.Missing.Value); // close the worksh

                    MessageBox.Show("Thanks ! Excel generated successfully ? ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            { excelFilePath = ""; }
            finally
            {
                if (myExcelApplication != null)
                {
                    myExcelApplication.Quit(); // close the excel application
                }
            }
            return excelFilePath;
        }

        private string GetFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xlsx)|*.xlsx";
            _browser.FileName = "Master_details.xlsx";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;

            return strPath;
        }

        private void dgrdParty_Sorted(object sender, EventArgs e)
        {
            int _index = 1;
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    row.Cells["sNo"].Value = _index;
                    _index++;
                }
            }
            catch { }
        }

        private string GetPaymentFileFromServer()
        {
            string strExePath = MainPage.strServerPath.Replace(@"\NET", "") + @"\Excel_File\payment_request.xlsx";
            try
            {
                if (!File.Exists(strExePath))
                {
                    string strPath = DataBaseAccess.DownloadFileFromServer(strExePath, "payment_request.xlsx");
                    System.Diagnostics.Process.Start(strPath);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Downloading file from Server", ex.Message };
                dba.CreateErrorReports(strReport);
                strExePath = "";
            }
            return strExePath;
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

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransport.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnSearchCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = panelSearch.Visible ? false : true;
            txtCategory.Focus();
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

        private void btnGroupArrow_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", Keys.Space);
                objSearch.ShowDialog();
                txtGroupName.Text = objSearch.strSelectedData;
                dgrdParty.Rows.Clear();
            }
            catch
            {
            }
        }

        private void btnSendWhatsappMessage_Click(object sender, EventArgs e)
        {
            //lblCharCount.Text = "Char Count : 0";
            //lblSMSCount.Text = "SMS Count  : 0";
            //panelSMS.Visible = true;
            //btnSMS.Text = "&Send Whatsapp";
            //txtSMSMobileNo.Enabled = false;
            //txtSMS.Clear();
            //txtSMS.Focus();
            try
            {
                string strWhatsappNo = GetSelectedWhatsappNo();
                SendWhatsappPage objMessage = new SendWhatsappPage(strWhatsappNo, "");
                objMessage.ShowDialog();
            }
            catch { }
        }

        private string GetSelectedWhatsappNo()
        {
            string strMobileNo = "";
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        string strMNo = Convert.ToString(row.Cells["whatsappNo"].Value);
                        if (strMNo != "")
                        {
                            if (!strMobileNo.Contains(strMNo))
                            {
                                if (strMobileNo == "")
                                    strMobileNo = "" + strMNo;
                                else
                                    strMobileNo += "," + strMNo;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return strMobileNo;
        }


        private void SendWhatsappMessage()
        {
            string strMobileNo = "", strMessage = "", strPartyName = "", strResult = "";
            int _count = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        string strMNo = Convert.ToString(row.Cells["whatsappNo"].Value);
                        if (strMNo != "")
                        {
                            strMessage = txtSMS.Text;
                            strPartyName = Convert.ToString(row.Cells["partyName"].Value);
                            string[] str = strPartyName.Split(' ');
                            if (str.Length > 1)
                            {
                                strPartyName = strPartyName.Replace("M/S", "").Trim();//.Replace(str[0], "")
                                strMessage = strMessage.Replace("[PARTYNAME]", strPartyName);

                                strMessage = "{\"default\": \"" + strPartyName + "\" }";
                                strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMNo, "welcome_msg", strMessage, "", "");
                                //strResult = WhatsappClass.SendWhatsAppMessage(strMNo, strMessage, "", "OTHER", "","TEXT");
                                if (strResult != "")
                                    _count++;
                            }
                        }
                    }
                }

                if (_count > 0)
                {
                    MessageBox.Show("Thank you ! " + _count + " message sent successfully !! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtSMS.Clear();
                    panelSMS.Visible = false;
                }
                else
                    MessageBox.Show("Unable to send message right now !! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }
        }

        private void SendWelcomeWhatsappMessage()
        {
            string strMobileNo = "", strMessage = "", strPartyName = "", strResult = "";
            int _count = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        strMobileNo = Convert.ToString(row.Cells["whatsappNo"].Value);
                        if (strMobileNo != "")
                        {
                            strPartyName = Convert.ToString(row.Cells["partyName"].Value);
                            string[] str = strPartyName.Split(' ');
                            if (str.Length > 1)
                            {
                                strPartyName = strPartyName.Replace(str[0], "").Replace("M/S", "").Trim();
                                strMessage = "{\"default\": \"" + strPartyName + "\" }";

                                strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, "sss_welcome", strMessage, "", "");

                                //strResult = WhatsappClass.SendWhatsAppMessage(strMNo, strMessage, "", "OTHER", "","TEXT");
                                if (strResult != "")
                                    _count++;
                            }
                        }
                    }
                }

                if (_count > 0)
                {
                    MessageBox.Show("Thank you ! " + _count + " message sent successfully !! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtSMS.Clear();
                    panelSMS.Visible = false;
                }
                else
                    MessageBox.Show("Unable to send message right now !! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnExport.Enabled = false;

                    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    object misValue = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;

                    //Create Excel Sheets
                    xlSheets = ExcelApp.Sheets;
                    xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
                                   Type.Missing, Type.Missing, Type.Missing);

                    int _skipColumn = 0;
                    string strHeader = "";
                    for (int j = 1; j < dgrdParty.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdParty.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdParty.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdParty.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdParty.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdParty.Columns.Count; l++)
                        {
                            if (dgrdParty.Columns[l].HeaderText == "" || !dgrdParty.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdParty.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdParty.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Show_Party_Master";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                }
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private void txtReference_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("REFERENCENAME", "SEARCH REFERENCE NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtReference.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;

            }
            catch
            {
            }
        }

        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, false);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }
    }
}
