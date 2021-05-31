using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace SSS
{
    public partial class ShowPartyBalanceSlabwise : Form
    {
        DataBaseAccess dba;
        public ShowPartyBalanceSlabwise()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindColumnSettingData();
            btnSelectCompany.Enabled = true;
            GetMultiQuarterName();

            txtDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
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
                    string strFName = "", strFullCompanyName = "", strFilePath="";
                    foreach (string folderName in sFolder)
                    {
                        string[] strFile = Directory.GetFiles(folderName, "*.syber");
                        if (strFile.Length > 0)
                        {
                            FileInfo objFile = new FileInfo(folderName);
                            strFName = objFile.Name;
                            strFilePath = strPath + "\\" + strFName + "\\" + strFName + ".syber";
                            using (StreamReader sr = new StreamReader(strFilePath))
                            {
                                strFullCompanyName = sr.ReadToEnd().Trim();
                                dgrdCompany.Rows.Add();
                                dgrdCompany.Rows[rowIndex].Cells["companyCheck"].Value = true;
                                dgrdCompany.Rows[rowIndex].Cells["code"].Value = "A"+strFName;
                                dgrdCompany.Rows[rowIndex].Cells["companyName"].Value = strFullCompanyName;
                            }

                            rowIndex++;
                        }
                    }
                }   
                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name", ex.Message };
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


        #endregion

        private void PartyBalanceDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else if (panelCompany.Visible)
                    panelCompany.Visible = false;
                else if (panalColumnSetting.Visible)
                    panalColumnSetting.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnCancel_Click(object sender, EventArgs e)
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

        private void txtStation_KeyDown(object sender, KeyEventArgs e)
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
            dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = btnSearch.Enabled = false;
            try
            {
                if (txtDaysSlab.Text != "")
                {
                    GetDataFromDataBase();
                    chkAll.Checked = true;
                    panelSearch.Visible = false;
                }
                else
                {
                    MessageBox.Show("Sorry ! Days slab can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnGo.Enabled = btnSearch.Enabled = true;
        }

        private void CreateColumn(int _slab)
        {
            if (_slab > 0)
            {
                dgrdDetails.Columns["fAmt"].HeaderText = 0 + " - " + _slab;
                dgrdDetails.Columns["sAmt"].HeaderText = (_slab + 1) + " - " + (_slab * 2);
                dgrdDetails.Columns["tAmt"].HeaderText = ((_slab * 2) + 1) + " - " + (_slab * 3);
                dgrdDetails.Columns["frAmt"].HeaderText = ((_slab * 3) + 1) + " > ";
            }
        }

        //public string CreateQuery(int _slab)
        //{
        //    string strQuery = "", strSubQuery = "", strDate = "", strLQuery = "";

        //    // strSubQuery += " and BA.AccountID='DL418' ";
        //    if (txtGroupName.Text != "")
        //        strSubQuery += " and SM.GroupName='" + txtGroupName.Text + "' ";
        //    if (txtCategory.Text != "")
        //        strSubQuery += " and SM.Category='" + txtCategory.Text + "' ";
        //    if (txtPartyType.Text != "")
        //        strSubQuery += " and SM.TINNumber='" + txtPartyType.Text + "' ";
        //    if (txtState.Text != "")
        //        strSubQuery += " and SM.State='" + txtState.Text + "' ";

        //    if (txtPartyName.Text != "")
        //    {
        //        string[] strFullName = txtPartyName.Text.Split(' ');
        //        if (strFullName.Length > 1)
        //            strSubQuery += " and (ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')) ='" + strFullName[0] + "' ";
        //    }

        //    DateTime _date = DateTime.Now;

        //    if (txtDate.Text.Length == 10)
        //        _date = dba.ConvertDateInExactFormat(txtDate.Text);
        //    strDate = _date.ToString("MM/dd/yyyy");

        //    if (txtAmount.Text != "")
        //        strLQuery += " and (BAmt>" + txtAmount.Text + " OR (-1*BAmt)>" + txtAmount.Text + ") ";

        //    if (txtFourthSlab.Text != "")
        //        strLQuery += " and (((RAmt+FAmt)>" + txtFourthSlab.Text + " AND FAmt>" + txtFourthSlab.Text + ") OR ((-1*(RAmt+FAmt))>" + txtFourthSlab.Text + " AND (-1*FAmt)>" + txtFourthSlab.Text + ")) ";
        //    if (txtThirdSlab.Text != "")
        //        strLQuery += " and (((RAmt+FAmt+TAmt)>" + txtThirdSlab.Text + " AND TAmt>" + txtThirdSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt))>" + txtThirdSlab.Text + " AND (-1*TAmt)>" + txtThirdSlab.Text + ")) ";
        //    if (txtSecondSlab.Text != "")
        //        strLQuery += " and (((RAmt+FAmt+TAmt+SAmt)>" + txtSecondSlab.Text + " AND SAmt>" + txtSecondSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt+SAmt))>" + txtSecondSlab.Text + " AND (-1*SAmt)>" + txtSecondSlab.Text + ")) ";
        //    if (txtFirstSlab.Text != "")
        //        strLQuery += " and (((RAmt+FAmt+TAmt+SAmt+FRAmt)>" + txtFirstSlab.Text + " AND FRAmt>" + txtFirstSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt+SAmt+FRAmt))>" + txtFirstSlab.Text + " AND (-1*FRAmt)>" + txtFirstSlab.Text + ")) ";


        //    if (rdoDebit.Checked)
        //        strLQuery += " and BAmt >0 ";
        //    else if (rdoCredit.Checked)
        //        strLQuery += " and BAmt<0 ";


        //    //DATEADD(dd, (CASE WHEN CAST(SM.DueDays as int)>0 then CAST(SM.DueDays as int) else (Select TOP 1 CAST(CS.GraceDays as int) from CompanySetting CS Where CompanyName='"+MainPage.strCompanyName+"') end), 

        //    strQuery += "Declare @_Date datetime; "
        //                   + " Set @_Date='" + strDate + "'; "
        //                   + " Select * from (Select PartyName,GroupName,Category,GradeName,MobileNo, SUM(FRAmt) FRAmt, SUM(SAmt) SAmt,SUM(TAmt) TAmt,SUM(FAmt) FAmt,SUM(RAmt) as RAmt,SUM(BAmt) BAmt from ( "
        //                   + " Select PartyName, GroupName, Category,GradeName,MobileNo, SUM((CASE WHEN  _Days < " + _slab + " then BAmt else 0 end)) FRAmt,SUM((CASE WHEN _Days < (" + _slab + " * 2) and _Days >= (" + _slab + ")then(BAmt) else 0 end)) SAmt,SUM((CASE WHEN _Days < (" + _slab + " * 3) and _Days >= (" + _slab + " * 2) then(BAmt) else 0 end)) TAmt,SUM((CASE WHEN _Days >= (" + _slab + " * 3) then(BAmt) else 0 end)) FAmt,0 as RAmt,SUM(BAmt) BAmt from ( "
        //                   + " Select (BA.AccountID + ' ' + SM.Name) PartyName, SM.GroupName, SM.Category,SM.TinNumber as GradeName,MobileNo, SUM(CAST(BA.Amount as Money)) BAmt, DATEDIFF(dd,BA.Date, @_Date) _Days from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'DEBIT' and AccountStatus in ('PURCHASE A/C', 'SALES A/C')  " + strSubQuery + "   Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo, SM.DueDays,BA.Date UNION ALL "
        //                   + " Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, -SUM(CAST(BA.Amount as Money)) BAmt, DATEDIFF(dd,BA.Date, @_Date) _Days from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'CREDIT' and AccountStatus in ('PURCHASE A/C','SALES A/C') " + strSubQuery + "   Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo, SM.DueDays,BA.Date)_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo UNION ALL "
        //                   + " Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,SUM(CAST(BA.Amount as Money)) RAmt,SUM(CAST(BA.Amount as Money)) BAmt from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'DEBIT' and AccountStatus not in ('PURCHASE A/C','SALES A/C') " + strSubQuery + "  Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo UNION ALL "
        //                   + " Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,-SUM(CAST(BA.Amount as Money)) RAmt,-SUM(CAST(BA.Amount as Money)) BAmt from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'CREDIT' and AccountStatus not in ('PURCHASE A/C','SALES A/C') " + strSubQuery + "  Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo)_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo )_Balance Where PartyName!='' " + strLQuery + " Order by GroupName,Category,PartyName";

        //    return strQuery;
        //}


        //private void GetDataFromDataBase()
        //{
        //    double dSlab = dba.ConvertObjectToDouble(txtDaysSlab.Text);
        //    int _slab = Convert.ToInt32(dSlab);
        //    string strQuery = CreateQuery(_slab);
        //    DataTable _dt = dba.GetDataTable(strQuery);

        //    CreateColumn(_slab);
        //    BindDataWithControl(_dt, _slab);
        //}

        public string CreateQuery(int _slab)
        {
            string strQuery = "", strSubQuery = "", strDate = "", strLQuery = "";

            if (txtGroupName.Text != "")
            {
                string strGroupName = txtGroupName.Text;
                if (txtGroupName.Text == "PURCHASE PARTY")
                    strGroupName = "SUNDRY CREDITOR";
                else if (txtGroupName.Text == "SALES PARTY")
                    strGroupName = "SUNDRY DEBTORS";

                strSubQuery += " and SM.GroupName='" + strGroupName + "' ";
            }

            if (txtCategory.Text != "")
                strSubQuery += " and SM.Category='" + txtCategory.Text + "' ";
            if (txtPartyType.Text != "")
                strSubQuery += " and SM.TINNumber='" + txtPartyType.Text + "' ";
            if (txtState.Text != "")
                strSubQuery += " and SM.State='" + txtState.Text + "' ";
            if (txtBranchCode.Text != "")
                strSubQuery += " and SM.AreaCode='" + txtBranchCode.Text + "' ";

            if (txtPartyName.Text != "")
            {
                string[] strFullName = txtPartyName.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSubQuery += " and (ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')) ='" + strFullName[0] + "' ";
            }
       
            if (chkDate.Checked && txtToDate.Text.Length == 10)
            {
                DateTime eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strSubQuery += " and BA.Date <'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }


            DateTime _date = DateTime.Now;
            if (txtDate.Text.Length == 10)
                _date = dba.ConvertDateInExactFormat(txtDate.Text);
            strDate = _date.ToString("MM/dd/yyyy");

            if (txtFourthSlab.Text != "")
                strLQuery += " and (((RAmt+FAmt)>" + txtFourthSlab.Text + " AND FAmt>" + txtFourthSlab.Text + ") OR ((-1*(RAmt+FAmt))>" + txtFourthSlab.Text + " AND (-1*FAmt)>" + txtFourthSlab.Text + ")) ";
            if (txtThirdSlab.Text != "")
                strLQuery += " and (((RAmt+FAmt+TAmt)>" + txtThirdSlab.Text + " AND TAmt>" + txtThirdSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt))>" + txtThirdSlab.Text + " AND (-1*TAmt)>" + txtThirdSlab.Text + ")) ";
            if (txtSecondSlab.Text != "")
                strLQuery += " and (((RAmt+FAmt+TAmt+SAmt)>" + txtSecondSlab.Text + " AND SAmt>" + txtSecondSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt+SAmt))>" + txtSecondSlab.Text + " AND (-1*SAmt)>" + txtSecondSlab.Text + ")) ";
            if (txtFirstSlab.Text != "")
                strLQuery += " and (((RAmt+FAmt+TAmt+SAmt+FRAmt)>" + txtFirstSlab.Text + " AND FRAmt>" + txtFirstSlab.Text + ") OR ((-1*(RAmt+FAmt+TAmt+SAmt+FRAmt))>" + txtFirstSlab.Text + " AND (-1*FRAmt)>" + txtFirstSlab.Text + ")) ";
            
            strQuery += "Declare @_Date datetime; "
                           + " Set @_Date='" + strDate + "'; "
                           + " Select * from (Select PartyName,GroupName,Category,GradeName,MobileNo, SUM(FRAmt) FRAmt, SUM(SAmt) SAmt,SUM(TAmt) TAmt,SUM(FAmt) FAmt,SUM(RAmt) as RAmt,SUM(BAmt) BAmt,NickName from ( "
                           + " Select PartyName, GroupName, Category,GradeName,MobileNo, SUM((CASE WHEN  _Days < " + _slab + " then BAmt else 0 end)) FRAmt,SUM((CASE WHEN _Days < (" + _slab + " * 2) and _Days >= (" + _slab + ")then(BAmt) else 0 end)) SAmt,SUM((CASE WHEN _Days < (" + _slab + " * 3) and _Days >= (" + _slab + " * 2) then(BAmt) else 0 end)) TAmt,SUM((CASE WHEN _Days >= (" + _slab + " * 3) then(BAmt) else 0 end)) FAmt,0 as RAmt,SUM(BAmt) BAmt,NickName from ( "
                           + " Select (BA.AccountID + ' ' + SM.Name) PartyName, SM.GroupName, SM.Category,SM.TinNumber as GradeName,MobileNo, SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) BAmt, DATEDIFF(dd,BA.Date, @_Date) _Days,Other as NickName from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) Where AccountStatus in ('PURCHASE A/C', 'SALES A/C')  " + strSubQuery + "   Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo, SM.DueDays,BA.Date,Other "
                           //+ "  UNION ALL Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, -SUM(CAST(BA.Amount as Money)) BAmt, DATEDIFF(dd,BA.Date, @_Date) _Days,Other as NickName from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'CREDIT' and AccountStatus in ('PURCHASE A/C','SALES A/C') " + strSubQuery + "   Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo, SM.DueDays,BA.Date,Other "
                           + " )_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo,NickName UNION ALL "
                           + " Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) RAmt,SUM(CAST(BA.Amount as Money)*(CASE WHEN BA.Status = 'CREDIT' then -1 else 1 end)) BAmt,Other as NickName from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere AccountStatus not in ('PURCHASE A/C','SALES A/C') " + strSubQuery + " [OPENING] Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo,Other "
                           //+ " UNION ALL Select (BA.AccountID + ' ' + SM.Name) PartyName,SM.GroupName,SM.Category,SM.TinNumber as GradeName,MobileNo, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,-SUM(CAST(BA.Amount as Money)) RAmt,-SUM(CAST(BA.Amount as Money)) BAmt,Other as NickName from BalanceAMount BA inner join SupplierMaster SM on BA.AccountID = (SM.AreaCode + SM.AccountNo) WHere BA.Status = 'CREDIT' and AccountStatus not in ('PURCHASE A/C','SALES A/C') " + strSubQuery + " [OPENING] Group by BA.AccountID,SM.GroupName,SM.Name,SM.Category,SM.TinNumber,MobileNo,Other"
                           + " )_Balance Group by PartyName, GroupName, Category,GradeName,MobileNo,NickName)_Balance Where PartyName!='' " + strLQuery + " Order by NickName ";

            return strQuery;
        }


        private void GetDataFromDataBase()
        {
            double dSlab = dba.ConvertObjectToDouble(txtDaysSlab.Text);
            int _slab = Convert.ToInt32(dSlab);
            string strQuery = CreateQuery(_slab), strCompanyCode = "";
            DataTable _dt = null, table = null;
                                               
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
                            table = dba.GetMultiQuarterDataTable(strQuery.Replace("[OPENING]", ""), strCompanyCode);
                        else
                        {
                            dt = dba.GetMultiQuarterDataTable(strQuery.Replace("[OPENING]", " and AccountStatus!='OPENING' "), strCompanyCode);
                            if (table == null)
                                table = dt;
                            else if (dt != null)
                                table.Merge(dt, true);
                        }
                        if (table.Rows.Count > 0)
                            rowCount++;
                    }
                }
            }
            
            CreateColumn(_slab);         
            BindDataWithControl(table, _slab);
        }

        private void BindDataWithControl(DataTable _dTable, int _slab)
        {
            dgrdDetails.Rows.Clear();

            double _dBalanceAmt = 0;
            if (txtAmount.Text != "")
                _dBalanceAmt = dba.ConvertObjectToDouble(txtAmount.Text);
            
            string strOLDNickName = "", strNickName = "";
            double dAmt = 0, dBAmt = 0, dFAmt = 0, dRAmt = 0, dSAmt = 0, dTAmt = 0, dFRAmt = 0, dTBAmt = 0, dTRAmt = 0, dTFAmt = 0, dTSAmt = 0, dTTAmt = 0, dTFRAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dSSSAmt = 0;

            if (_dTable.Rows.Count > 0)
            {
                int _index = 0, _groupID = 0;         
                DataTable _dtNewTable = _dTable.DefaultView.ToTable(true, "PartyName", "GroupName", "Category", "GradeName", "MobileNo", "NickName");

                foreach (DataRow row in _dtNewTable.Rows)
                {
                    dBAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(BAmt)", "PartyName='" + row["partyName"] + "' "));

                    if (txtAmount.Text == "" || (dBAmt > _dBalanceAmt || (-1 * dBAmt) > _dBalanceAmt))
                    {
                        if (rdoAll.Checked || (rdoDebit.Checked && dBAmt > 0) || (rdoCredit.Checked && dBAmt < 0))
                        {
                            dgrdDetails.Rows.Add(1);
                            dTBAmt += dBAmt;
                            dTFAmt += dFAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(FAmt)", "partyName='" + row["partyName"] + "' "));// dba.ConvertObjectToDouble(row["FAmt"]);
                            dTSAmt += dSAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(SAmt)", "partyName='" + row["partyName"] + "' "));//dba.ConvertObjectToDouble(row["SAmt"]);
                            dTTAmt += dTAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(TAmt)", "partyName='" + row["partyName"] + "' "));// dba.ConvertObjectToDouble(row["TAmt"]);
                            dTFRAmt += dFRAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(FRAmt)", "partyName='" + row["partyName"] + "' "));// dba.ConvertObjectToDouble(row["FRAmt"]);
                            dTRAmt += dRAmt = dba.ConvertObjectToDouble(_dTable.Compute("SUM(RAmt)", "partyName='" + row["partyName"] + "' "));// dba.ConvertObjectToDouble(row["RAmt"]);
                            strNickName = Convert.ToString(row["NickName"]);

                            if (strNickName != strOLDNickName)
                            {
                                _groupID++;
                                strOLDNickName = strNickName;
                            }
                            else if (strNickName == "")
                                _groupID++;

                            dgrdDetails.Rows[_index].Cells["chkStatus"].Value = true;
                            dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1) + ".";
                            dgrdDetails.Rows[_index].Cells["partyName"].Value = row["partyName"];
                            dgrdDetails.Rows[_index].Cells["groupName"].Value = row["GroupName"];
                            dgrdDetails.Rows[_index].Cells["categoryName"].Value = row["Category"];
                            dgrdDetails.Rows[_index].Cells["grade"].Value = row["GradeName"];
                            dgrdDetails.Rows[_index].Cells["mobileNo"].Value = row["MobileNo"];
                     
                            dgrdDetails.Rows[_index].Cells["nickName"].Value = strNickName;
                            dgrdDetails.Rows[_index].Cells["frAmt"].Value = dgrdDetails.Rows[_index].Cells["tAmt"].Value = dgrdDetails.Rows[_index].Cells["sAmt"].Value = dgrdDetails.Rows[_index].Cells["fAmt"].Value = dgrdDetails.Rows[_index].Cells["netBalance"].Value = 0;
                            dgrdDetails.Rows[_index].Cells["nick_ID"].Value = _groupID % 2;

                            if ((_groupID % 2) == 0)
                                dgrdDetails.Rows[_index].DefaultCellStyle.BackColor = Color.LightGray;
                            else
                                dgrdDetails.Rows[_index].DefaultCellStyle.BackColor = Color.White;                          

                            dAmt = dFAmt + dRAmt;
                            if ((dAmt > 0 && dFAmt > 0) || (dAmt < 0 && dFAmt < 0))
                            {
                                if (dAmt >= 0)
                                {
                                    dgrdDetails.Rows[_index].Cells["frAmt"].Value = dAmt;
                                    dgrdDetails.Rows[_index].Cells["frStatus"].Value = "DR";
                                }
                                else
                                {
                                    dgrdDetails.Rows[_index].Cells["frAmt"].Value = Math.Abs(dAmt);
                                    dgrdDetails.Rows[_index].Cells["frStatus"].Value = "CR";
                                }
                                dAmt = 0;
                            }
                            else
                            {
                                dgrdDetails.Rows[_index].Cells["frAmt"].Value = 0.00;
                                dgrdDetails.Rows[_index].Cells["frStatus"].Value = "DR";
                            }
                            dAmt += dTAmt;

                            if ((dAmt > 0 && dTAmt > 0) || (dAmt < 0 && dTAmt < 0))
                            {
                                if (dAmt >= 0)
                                {
                                    dgrdDetails.Rows[_index].Cells["tAmt"].Value = dAmt;
                                    dgrdDetails.Rows[_index].Cells["tStatus"].Value = "DR";
                                }
                                else
                                {
                                    dgrdDetails.Rows[_index].Cells["tAmt"].Value = Math.Abs(dAmt);
                                    dgrdDetails.Rows[_index].Cells["tStatus"].Value = "CR";
                                }
                                dAmt = 0;
                            }
                            else
                            {
                                dgrdDetails.Rows[_index].Cells["tAmt"].Value = 0.00;
                                dgrdDetails.Rows[_index].Cells["tStatus"].Value = "DR";
                            }
                            dAmt += dSAmt;

                            if ((dAmt > 0 && dSAmt > 0) || (dAmt < 0 && dSAmt < 0))
                            {
                                if (dAmt >= 0)
                                {
                                    dgrdDetails.Rows[_index].Cells["sAmt"].Value = dAmt;
                                    dgrdDetails.Rows[_index].Cells["sStatus"].Value = "DR";
                                }
                                else
                                {
                                    dgrdDetails.Rows[_index].Cells["sAmt"].Value = Math.Abs(dAmt);
                                    dgrdDetails.Rows[_index].Cells["sStatus"].Value = "CR";
                                }
                                dAmt = 0;
                            }
                            else
                            {
                                dgrdDetails.Rows[_index].Cells["sAmt"].Value = 0.00;
                                dgrdDetails.Rows[_index].Cells["sStatus"].Value = "DR";
                            }
                            dAmt += dFRAmt;

                            if (dAmt >= 0)
                            {
                                dgrdDetails.Rows[_index].Cells["fAmt"].Value = dAmt;//.ToString("N2", MainPage.indianCurancy)
                                dgrdDetails.Rows[_index].Cells["fStatus"].Value = "DR";
                            }
                            else
                            {
                                dgrdDetails.Rows[_index].Cells["fAmt"].Value = Math.Abs(dAmt);
                                dgrdDetails.Rows[_index].Cells["fStatus"].Value = "CR";
                            }

                            if (dBAmt >= 0)
                            {
                                dDebitAmt += dBAmt;
                                dgrdDetails.Rows[_index].Cells["netBalance"].Value = dBAmt;
                                dgrdDetails.Rows[_index].Cells["netStatus"].Value = "DR";
                            }
                            else
                            {
                                dCreditAmt += Math.Abs(dBAmt);
                                dgrdDetails.Rows[_index].Cells["netBalance"].Value = Math.Abs(dBAmt);
                                dgrdDetails.Rows[_index].Cells["netStatus"].Value = "CR";
                            }
                            _index++;
                        }
                    }

                }
            }

            dAmt = dDebitAmt - dCreditAmt;
            if (dAmt >= 0)
            {
                lblBalAmount.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            }
            else
            {
                lblBalAmount.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            }

            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
        }

        //private void BindDataWithControl(DataTable _dt, int _slab)
        //{
        //    dgrdDetails.Rows.Clear();
        //    double dAmt = 0, dBAmt = 0, dFAmt = 0, dRAmt = 0, dSAmt = 0, dTAmt = 0, dFRAmt = 0, dTBAmt = 0, dTRAmt = 0, dTFAmt = 0, dTSAmt = 0, dTTAmt = 0, dTFRAmt = 0, dDebitAmt = 0, dCreditAmt = 0;
        //    if (_dt.Rows.Count > 0)
        //    {
        //        dgrdDetails.Rows.Add(_dt.Rows.Count);

        //        int _index = 0;
        //        foreach (DataRow row in _dt.Rows)
        //        {
        //            dTBAmt += dBAmt = dba.ConvertObjectToDouble(row["BAmt"]);
        //            dTFAmt += dFAmt = dba.ConvertObjectToDouble(row["FAmt"]);
        //            dTSAmt += dSAmt = dba.ConvertObjectToDouble(row["SAmt"]);
        //            dTTAmt += dTAmt = dba.ConvertObjectToDouble(row["TAmt"]);
        //            dTFRAmt += dFRAmt = dba.ConvertObjectToDouble(row["FRAmt"]);
        //            dTRAmt += dRAmt = dba.ConvertObjectToDouble(row["RAmt"]);

        //            dgrdDetails.Rows[_index].Cells["chkStatus"].Value = true;
        //            dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1) + ".";
        //            dgrdDetails.Rows[_index].Cells["partyName"].Value = row["partyName"];
        //            dgrdDetails.Rows[_index].Cells["groupName"].Value = row["GroupName"];
        //            dgrdDetails.Rows[_index].Cells["categoryName"].Value = row["Category"];
        //            dgrdDetails.Rows[_index].Cells["grade"].Value = row["GradeName"];
        //            dgrdDetails.Rows[_index].Cells["mobileNo"].Value = row["MobileNo"];

        //            dAmt = dFAmt + dRAmt;
        //            if ((dAmt >= 0 && dFAmt >= 0) || (dAmt < 0 && dFAmt < 0))
        //            {
        //                if (dAmt >= 0)
        //                {
        //                    dgrdDetails.Rows[_index].Cells["frAmt"].Value = dAmt;
        //                    dgrdDetails.Rows[_index].Cells["frStatus"].Value = "DR";
        //                }
        //                else
        //                {
        //                    dgrdDetails.Rows[_index].Cells["frAmt"].Value = Math.Abs(dAmt);
        //                    dgrdDetails.Rows[_index].Cells["frStatus"].Value = "CR";
        //                }
        //                dAmt = 0;
        //            }
        //            else
        //            {
        //                dgrdDetails.Rows[_index].Cells["frAmt"].Value = 0;
        //                dgrdDetails.Rows[_index].Cells["frStatus"].Value = "DR";
        //            }
        //            dAmt += dTAmt;

        //            if ((dAmt >= 0 && dTAmt > 0) || (dAmt < 0 && dTAmt < 0))
        //            {
        //                if (dAmt >= 0)
        //                {
        //                    dgrdDetails.Rows[_index].Cells["tAmt"].Value = dAmt;
        //                    dgrdDetails.Rows[_index].Cells["tStatus"].Value = "DR";
        //                }
        //                else
        //                {
        //                    dgrdDetails.Rows[_index].Cells["tAmt"].Value = Math.Abs(dAmt);
        //                    dgrdDetails.Rows[_index].Cells["tStatus"].Value = "CR";
        //                }
        //                dAmt = 0;
        //            }
        //            else
        //            {
        //                dgrdDetails.Rows[_index].Cells["tAmt"].Value = 0;
        //                dgrdDetails.Rows[_index].Cells["tStatus"].Value = "DR";
        //            }
        //            dAmt += dSAmt;

        //            if ((dAmt >= 0 && dSAmt > 0) || (dAmt < 0 && dSAmt < 0))
        //            {
        //                if (dAmt >= 0)
        //                {
        //                    dgrdDetails.Rows[_index].Cells["sAmt"].Value = dAmt;
        //                    dgrdDetails.Rows[_index].Cells["sStatus"].Value = "DR";
        //                }
        //                else
        //                {
        //                    dgrdDetails.Rows[_index].Cells["sAmt"].Value = Math.Abs(dAmt);
        //                    dgrdDetails.Rows[_index].Cells["sStatus"].Value = "CR";
        //                }
        //                dAmt = 0;
        //            }
        //            else
        //            {
        //                dgrdDetails.Rows[_index].Cells["sAmt"].Value = 0;
        //                dgrdDetails.Rows[_index].Cells["sStatus"].Value = "DR";
        //            }
        //            dAmt += dFRAmt;

        //            if (dAmt >= 0)
        //            {
        //                dgrdDetails.Rows[_index].Cells["fAmt"].Value = dAmt;//.ToString("N2", MainPage.indianCurancy)
        //                dgrdDetails.Rows[_index].Cells["fStatus"].Value = "DR";
        //            }
        //            else
        //            {
        //                dgrdDetails.Rows[_index].Cells["fAmt"].Value = Math.Abs(dAmt);
        //                dgrdDetails.Rows[_index].Cells["fStatus"].Value = "CR";
        //            }

        //            if (dBAmt >= 0)
        //            {
        //                dDebitAmt += dBAmt;
        //                dgrdDetails.Rows[_index].Cells["netBalance"].Value = dBAmt;
        //                dgrdDetails.Rows[_index].Cells["netStatus"].Value = "DR";
        //            }
        //            else
        //            {
        //                dCreditAmt += Math.Abs(dBAmt);
        //                dgrdDetails.Rows[_index].Cells["netBalance"].Value = Math.Abs(dBAmt);
        //                dgrdDetails.Rows[_index].Cells["netStatus"].Value = "CR";
        //            }

        //            _index++;
        //        }
        //    }

        //    dAmt = dDebitAmt - dCreditAmt;
        //    if (dAmt >= 0)
        //    {
        //        lblBalAmount.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
        //    }
        //    else
        //    {
        //        lblBalAmount.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
        //    }

        //    lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
        //    lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
        //}

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                }
                else if (e.KeyCode == Keys.Enter && dgrdDetails.CurrentRow.Index >= 0 && dgrdDetails.CurrentCell.ColumnIndex == 1)
                {
                    string strName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    if (strName != "")
                    {
                        ShowPartyLedger(strName);
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowPartyLedger(string strName)
        {
            LedgerAccount _obj = new LedgerAccount(strName);
            _obj.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            _obj.ShowInTaskbar = true;
            _obj.ShowDialog();
        }


        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();

            try
            {
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("HeaderText", typeof(String));
                table.Columns.Add("SNo", typeof(String));
                table.Columns.Add("PartyName", typeof(String));
                table.Columns.Add("GroupName", typeof(String));
                table.Columns.Add("FAmtHeader", typeof(String));
                table.Columns.Add("SAmtHeader", typeof(String));
                table.Columns.Add("TAmtHeader", typeof(String));
                table.Columns.Add("FRAmtHeader", typeof(String));
                table.Columns.Add("FAmt", typeof(String));
                table.Columns.Add("SAmt", typeof(String));
                table.Columns.Add("TAmt", typeof(String));
                table.Columns.Add("FRAmt", typeof(String));
                table.Columns.Add("BalanceAmt", typeof(String));
                table.Columns.Add("UserName", typeof(String));

                int _index = 1;
                string strHeader = "PARTY BALANCE SLAB WISE";
                if (txtGroupName.Text != "")
                    strHeader += " OF " + txtGroupName.Text;
                if (txtStation.Text != "")
                    strHeader += " FROM " + txtStation.Text;


                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
                    {
                        DataRow dr = table.NewRow();
                        dr["CompanyName"] = MainPage.strPrintComapanyName;
                        dr["SNo"] = _index + ".";
                        dr["HeaderText"] = strHeader;
                        dr["PartyName"] = row.Cells["partyName"].Value;
                        if (dgrdDetails.Columns["fAmt"].Visible)
                        {
                            dr["FAmtHeader"] = dgrdDetails.Columns["fAmt"].HeaderText;
                            dr["FAmt"] = dba.ConvertObjectToDouble(row.Cells["fAmt"].Value).ToString("N2", MainPage.indianCurancy) + " " + row.Cells["fStatus"].Value;
                        }
                        else
                            dr["FAmtHeader"] = "----";
                        if (dgrdDetails.Columns["sAmt"].Visible)
                        {
                            dr["SAmtHeader"] = dgrdDetails.Columns["sAmt"].HeaderText;
                            dr["SAmt"] = dba.ConvertObjectToDouble(row.Cells["sAmt"].Value).ToString("N2", MainPage.indianCurancy) + " " + row.Cells["sStatus"].Value;
                        }
                        else
                            dr["SAmtHeader"] = "----";
                        if (dgrdDetails.Columns["tAmt"].Visible)
                        {
                            dr["TAmtHeader"] = dgrdDetails.Columns["tAmt"].HeaderText;
                            dr["TAmt"] = dba.ConvertObjectToDouble(row.Cells["tAmt"].Value).ToString("N2", MainPage.indianCurancy) + " " + row.Cells["tStatus"].Value;
                        }
                        else
                            dr["TAmtHeader"] = "----";
                        if (dgrdDetails.Columns["frAmt"].Visible)
                        {
                            dr["FRAmtHeader"] = dgrdDetails.Columns["frAmt"].HeaderText;
                            dr["FRAmt"] = dba.ConvertObjectToDouble(row.Cells["frAmt"].Value).ToString("N2", MainPage.indianCurancy) + " " + row.Cells["frStatus"].Value;
                        }
                        else
                            dr["FRAmtHeader"] = "----";

                        if (dgrdDetails.Columns["netBalance"].Visible)
                        {
                            dr["BalanceAmt"] = dba.ConvertObjectToDouble(row.Cells["netBalance"].Value).ToString("N2", MainPage.indianCurancy) + " " + row.Cells["netStatus"].Value;
                        }

                        dr["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        table.Rows.Add(dr);
                        _index++;
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
                Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("Show Party Record");
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.PartyBalanceSlabReport objReport = new Reporting.PartyBalanceSlabReport();
                objReport.SetDataSource(dt);
                objShowReport.myPreview.ReportSource = objReport;
                objShowReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["chkStatus"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_Scroll(object sender, ScrollEventArgs e)
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

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.PartyBalanceSlabReport objReport = new Reporting.PartyBalanceSlabReport();
                objReport.SetDataSource(dt);
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objReport);
                else
                    objReport.PrintToPrinter(1, false, 0, 0);
            }
            catch
            {
            }
            btnPrint.Enabled = true;
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

        private void BindColumnSettingData()
        {
            try
            {
                string[] strHeader = { "Party Name", "Group Name", "First Slab", "Second Slab", "Third Slab", "Fourth Slab", "Net Balance", "Category Name" };
                string[] strName = { "partyName", "groupName", "fAmt", "sAmt", "tAmt", "frAmt", "netBalance", "categoryName" };
                int _rowIndex = 0;
                dgrdColumnSetting.Rows.Clear();
                dgrdColumnSetting.Rows.Add(strHeader.Length);
                foreach (string strData in strHeader)
                {
                    dgrdColumnSetting.Rows[_rowIndex].Cells["columnName"].Value = strData;
                    dgrdColumnSetting.Rows[_rowIndex].Cells["colName"].Value = strName[_rowIndex];
                    dgrdColumnSetting.Rows[_rowIndex].Cells["colIndex"].Value = _rowIndex + 1;
                    _rowIndex++;
                }
            }
            catch { }
        }

        private void dgrdColumnSetting_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgrdColumnSetting.CurrentCell.ColumnIndex == 1)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }


        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (dgrdColumnSetting.CurrentCell.ColumnIndex == 1)
                    dba.KeyHandlerPoint(sender, e, 0);
            }
            catch { }
        }

        private void RearrangeColumn()
        {
            try
            {
                int _index = 0, dIndex = 1;
                string strColumn = "";
                foreach (DataGridViewRow row in dgrdColumnSetting.Rows)
                {
                    _index = dba.ConvertObjectToInt(row.Cells["colIndex"].Value);
                    strColumn = Convert.ToString(row.Cells["colName"].Value);
                    if (_index == 0)
                    {
                        dgrdDetails.Columns[strColumn].Visible = false;
                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].Visible = false;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].Visible = false;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].Visible = false;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].Visible = false;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].Visible = false;
                    }
                    else
                    {
                        dgrdDetails.Columns[strColumn].Visible = true;
                        dgrdDetails.Columns[strColumn].DisplayIndex = dIndex;
                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].DisplayIndex = ++dIndex;

                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].Visible = true;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].Visible = true;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].Visible = true;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].Visible = true;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].Visible = true;

                        dIndex++;
                    }
                }
            }
            catch { }
        }

        private void dgrdColumnSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                RearrangeColumn();
            }
        }

        private void btnColumnSetting_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = !panalColumnSetting.Visible;
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = false;
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

        private void btnStation_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", Keys.Space);
                objSearch.ShowDialog();
                txtStation.Text = objSearch.strSelectedData;
            }
            catch { }
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
            catch { }
        }

        private void btnSearchCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = panelSearch.Visible ? false : true;
            txtFirstSlab.Focus();
        }

        private void btnPrintMultiLedger_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrintMultiLedger.Enabled = false;
                string[] strStatus = null, strPartyName = GetSelectedPartyNameAndStatus(ref strStatus);
                if (strPartyName.Length > 0)
                {
                    DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                    LedgerAccount objLedger = new LedgerAccount(strPartyName, strStatus, sDate, eDate);
                    objLedger.MdiParent = MainPage.mymainObject;
                    objLedger.Show();
                }
            }
            catch
            {
            }
            btnPrintMultiLedger.Enabled = true;
        }

        private string[] GetSelectedPartyNameAndStatus(ref string[] strStatus)
        {
            List<string> lstPartyName = new List<string>();
            List<string> lstStatus = new List<string>();
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
                    {
                        lstPartyName.Add(Convert.ToString(row.Cells["partyName"].Value));
                        lstStatus.Add("False");
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetails.Columns[j - 1].Name;
                        if (strHeader == "chkStatus" || !dgrdDetails.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            //j++;
                            continue;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                        {
                            if (dgrdDetails.Columns[l].Name == "chkStatus" || !dgrdDetails.Columns[l].Visible)
                            {
                                _skipColumn++;
                                //l++;
                                continue;
                            }
                            if (l < dgrdDetails.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Party_Balance_Slab_Wise";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                    }
                    else
                        MessageBox.Show("Export Cancled...");

                    ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    //xlWorkbook.Close(true, misValue, misValue);
                    //ExcelApp.Quit();
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);




                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void ShowPartyBalanceSlabwise_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

        private void txtToDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtToDate.ReadOnly = !chkDate.Checked;
            txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
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
    }
}
