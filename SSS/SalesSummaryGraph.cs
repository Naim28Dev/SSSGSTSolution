using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SSS
{
    public partial class SalesSummaryGraph : Form
    {
        DataBaseAccess dba;
        public SalesSummaryGraph()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void SalesOrderGraph_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (MinimizeChart())
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void ClearAllRecord()
        {

            //soChart.ChartAreas.Clear();
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                    ClearAllRecord();
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonth.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }


        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                if (rdoUser.Checked)
                {
                    grpMonth.Text = " Cash/Journal wise ";
                    grpBranch.Text = " Purchase wise ";
                    grpParty.Text = " Sale wise ";
                    grpQuarter.Text = " Bank wise ";
                }
                else if (rdoOther.Checked)
                {
                    grpMonth.Text = " Transport wise ";
                    grpBranch.Text = " State wise ";
                    grpParty.Text = " Courier wise ";
                    grpQuarter.Text = " Station wise ";
                }
                else
                {
                    grpMonth.Text = " Month wise ";
                    grpBranch.Text = " Branch wise ";
                    grpParty.Text = " Party wise ";
                    grpQuarter.Text = " Quarter wise ";
                }

                GetAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnGO.Enabled = true;
        }

        private void GetAllData()
        {
            string strQuery = "", strUserQuery = "", strSubQuery = CreateQuery(ref strUserQuery);
            if (rdoSale.Checked || rdoPurchase.Checked)
            {
                strQuery += "Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/10000000),2) as NetAmt,Quarter_Name as Item_Name,Quarter_Name from (Select (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) Quarter_Name,(CAST(Amount as Money)-ISNULL((Select SUM(CAST(BA1.Amount as Money)) TaxAmt from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)) NetAmt from BalanceAmount BA Where CAST(BA.Amount as Money)>0  " + strSubQuery + ") Sales Group by Quarter_Name)_Sales Order by Quarter_Name "
                         + " Select  TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,PName as Item_Name from (Select AccountID,(CAST(Amount as Money)-ISNULL((Select SUM(CAST(BA1.Amount as Money)) TaxAmt from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)) NetAmt from BalanceAmount BA Where CAST(BA.Amount as Money)>0  " + strSubQuery + ") Sales OUTER APPLY (SELECT TOP 1 (SUBSTRING(Name,0,20))PName from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar)=AccountID))SM Group by PName)_Sales Order by NetAmt desc  "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/10000000),2) as NetAmt,_MonthNo,_Month as Item_Name from (Select SUBSTRING(DATENAME(mm,BA.Date),1,3) _Month,DATEPART(mm,BA.Date) _MonthNo,(CAST(Amount as Money)-ISNULL((Select SUM(CAST(BA1.Amount as Money)) TaxAmt from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)) NetAmt from BalanceAmount BA Where CAST(BA.Amount as Money)>0  " + strSubQuery + ") Sales Group by _MonthNo,_Month)_Sales Order by (Case When _MonthNo<4 then _MonthNo+12 else _MonthNo end) "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/10000000),2) as NetAmt,BillCode as Item_Name,BillCode from (Select SUBSTRING(Description,(CHARINDEX('/',Description,0)+1),(CHARINDEX(' ',Description,0)-(CHARINDEX('/',Description,0))-2)) BillCode,(CAST(Amount as Money)-ISNULL((Select SUM(CAST(BA1.Amount as Money)) TaxAmt from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)) NetAmt from BalanceAmount BA Where CAST(BA.Amount as Money)>0  " + strSubQuery + ") Sales Group by BillCode)_Sales Order by NetAmt desc ";
            }
            else if (rdoCash.Checked)
            {
                strQuery += " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,Quarter_Name as Item_Name,Quarter_Name from (Select (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) Quarter_Name,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')  " + strSubQuery + " Group by (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) UNION ALL Select (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) Quarter_Name,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' and (VoucherCode!='' OR JournalID!='0')  " + strSubQuery + " Group by (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) ) Sales Group by Quarter_Name )_Sales Order by Quarter_Name "
                         + " Select TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,Name as Item_Name,Name from (Select SM.Name,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select (SUBSTRING(Name,0,20)) as Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')   " + strSubQuery + " Group by Name UNION ALL Select SM.Name,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select (SUBSTRING(Name,0,20)) as Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' " + strSubQuery + " and (VoucherCode!='' OR JournalID!='0')  Group by Name) Sales Group by Name )_Sales Order by NetAmt desc  "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,_Month as Item_Name,_MonthNo from (Select SUBSTRING(DATENAME(mm,BA.Date),1,3) _Month,DATEPART(mm,BA.Date) _MonthNo,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')   " + strSubQuery + " Group by SUBSTRING(DATENAME(mm,BA.Date),1,3),DATEPART(mm,BA.Date)UNION ALL Select SUBSTRING(DATENAME(mm,BA.Date),1,3) _Month,DATEPART(mm,BA.Date) _MonthNo,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' and (VoucherCode!='' OR JournalID!='0')  " + strSubQuery + " Group by SUBSTRING(DATENAME(mm,BA.Date),1,3),DATEPART(mm,BA.Date)) Sales Group by _Month,_MonthNo)_Sales Order by (Case When _MonthNo<4 then _MonthNo+12 else _MonthNo end)  "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,AreaCode as Item_Name,AreaCode from (Select AreaCode,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select AreaCode from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')   " + strSubQuery + " Group by AreaCode UNION ALL Select AreaCode,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select AreaCode from SupplierMaster Where GroupName='CASH A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' " + strSubQuery + " and (VoucherCode!='' OR JournalID!='0')  Group by AreaCode) Sales Group by AreaCode)_Sales Order by NetAmt desc";
            }
            else if (rdoBank.Checked)
            {
                strQuery += " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,Quarter_Name as Item_Name,Quarter_Name from (Select (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) Quarter_Name,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end)=1 " + strSubQuery + " Group by (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) UNION ALL Select (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) Quarter_Name,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 " + strSubQuery + " Group by (CASE  WHEN DATEPART(mm,BA.Date) in (4,5,6) Then 'QUARTER 1' WHEN DATEPART(mm,BA.Date) in (7,8,9) Then 'QUARTER 2' WHEN DATEPART(mm,BA.Date) in (10,11,12) Then 'QUARTER 3' ELSE 'QUARTER 4' End) ) Sales Group by Quarter_Name )_Sales Order by Quarter_Name "
                         + " Select TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,Name as Item_Name,Name from (Select SM.Name,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select (SUBSTRING(Name,0,20)) as Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0') and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 " + strSubQuery + " Group by Name UNION ALL Select SM.Name,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select (SUBSTRING(Name,0,20)) as Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' " + strSubQuery + " and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 Group by Name) Sales Group by Name )_Sales Order by NetAmt desc  "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,_Month as Item_Name,_MonthNo from (Select SUBSTRING(DATENAME(mm,BA.Date),1,3) _Month,DATEPART(mm,BA.Date) _MonthNo,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 " + strSubQuery + " Group by SUBSTRING(DATENAME(mm,BA.Date),1,3),DATEPART(mm,BA.Date)UNION ALL Select SUBSTRING(DATENAME(mm,BA.Date),1,3) _Month,DATEPART(mm,BA.Date) _MonthNo,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select Name from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 " + strSubQuery + " Group by SUBSTRING(DATENAME(mm,BA.Date),1,3),DATEPART(mm,BA.Date)) Sales Group by _Month,_MonthNo)_Sales Order by (Case When _MonthNo<4 then _MonthNo+12 else _MonthNo end)  "
                         + " Select (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select ROUND((SUM(NetAmt)/100000),2) as NetAmt,AreaCode as Item_Name,AreaCode from (Select AreaCode,SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select AreaCode from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0 and Status='DEBIT' and (VoucherCode!='' OR JournalID!='0') and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 " + strSubQuery + " Group by AreaCode UNION ALL Select AreaCode,-SUM(CAST(Amount as Money)) NetAmt from BalanceAmount BA CROSS APPLY (Select AreaCode from SupplierMaster Where GroupName='BANK A/C' and (AreaCode+CAST(AccountNo as nvarchar))=BA.AccountID) SM Where CAST(BA.Amount as Money)>0  and Status='CREDIT' " + strSubQuery + " and (VoucherCode!='' OR JournalID!='0')  and (CASE WHEN (Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) =1 Group by AreaCode) Sales Group by AreaCode)_Sales Order by NetAmt desc";
            }
            else if (rdoUser.Checked)
            {
                strQuery += " Select TOP 20 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select UpdatedBy  as Item_Name,Count(*) NetAmt from EditTrailDetails Where BillType='BANK' and EditStatus ='CREATION' " + strUserQuery + " Group by UpdatedBy )_Sales Order by  NetAmt desc "
                         + " Select TOP 20 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select UpdatedBy as Item_Name, Count(*) NetAmt from EditTrailDetails Where BillType in ('SALES') and EditStatus = 'CREATION' " + strUserQuery + " Group by UpdatedBy )_Sales Order by NetAmt desc "
                         + " Select TOP 20 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select UpdatedBy as Item_Name, Count(*) NetAmt from EditTrailDetails Where BillType in ('CASH', 'JOURNAL') and EditStatus = 'CREATION' " + strUserQuery + " Group by UpdatedBy )_Sales Order by NetAmt desc "
                         + " Select TOP 20 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select UpdatedBy as Item_Name, Count(*) NetAmt from EditTrailDetails Where BillType = 'GOODSPURCHASE' and EditStatus = 'CREATION' " + strUserQuery + " Group by UpdatedBy )_Sales Order by NetAmt desc ";
            }
            else if (rdoOther.Checked)
            {
                strQuery += " Select TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from(Select SUBSTRING(Station, 0, 20) as Item_Name, Count(*) NetAmt from SalesRecord Where Station != '' " + strUserQuery.Replace("Date", "BillDate") + "  Group by Station)_Sales Order by NetAmt desc "
                         + " Select  TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select SUBSTRING(CourierName, 0, 20) as Item_Name, Count(*) NetAmt from CourierRegister Where CourierName != '' " + strUserQuery + "  Group by CourierName)_Sales Order by NetAmt desc "
                         + " Select TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select SUBSTRING(Transport,0,20)  as Item_Name,Count(*) NetAmt from SalesRecord Where Transport!='' " + strUserQuery.Replace("Date", "BilLDate") + " Group by Transport )_Sales Order by  NetAmt desc  "
                         + " Select TOP 15 (Item_Name+'\n'+CAST(NetAmt as nvarchar)) as Item_Name, NetAmt from (Select SUBSTRING(SM.State, 0, 20) as Item_Name, Count(*) NetAmt from SalesRecord SR CROSS APPLY (Select State from SupplierMaster Where (AreaCode + CAST(AccountNo as nvarchar)) = SalePartyID) SM Where SM.State != '' " + strUserQuery.Replace("Date", "BilLDate") + " Group by SM.State)_Sales Order by NetAmt desc  ";
            }


            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            int _index = 0, _count = ds.Tables.Count;

            Chart[] _chart = { qtrChart, partyChart, monthChart, branchChart };

            for (; _index < _count; _index++)
            {
                DataTable table = ds.Tables[0];

                ds.Tables.RemoveAt(0);
                DataSet _ds = new DataSet();
                _ds.Tables.Add(table);
                BindRecordWithGrid(_ds, _chart[_index], _index);
            }
        }


        private string CreateQuery(ref string strUserQuery)
        {
            string strQuery = "", strGroupName = "";
            strUserQuery = "";
            if (txtBranchCode.Text != "")
            {
                strQuery += " and BA.Description Like('%" + txtBranchCode.Text + "%') ";
                strUserQuery += " and BillCode Like('%" + txtBranchCode.Text + "%')  ";
            }
            
            if (txtMonth.Text != "")
            {
                strQuery += " and DATENAME(mm,BA.Date)='" + txtMonth.Text + "' ";
                strUserQuery += " and DATENAME(mm,Date)='" + txtMonth.Text + "' ";
            }

            if (txtPartyName.Text != "")
            {
                string[] strPName = txtPartyName.Text.Split(' ');
                if (strPName.Length > 0)
                    strQuery += " and AccountID='" + strPName[0] + "' ";
            }

            if (rdoSale.Checked)
                strGroupName = "SALES A/C";
            else if (rdoPurchase.Checked)
                strGroupName = "PURCHASE A/C";
            if (strGroupName != "")
                strQuery += " and AccountStatus in ('" + strGroupName + "') ";

            return strQuery;
        }

        private void BindRecordWithGrid(DataSet _ds, Chart _Chart, int _index)
        {
            _Chart.DataSource = _ds;
            _Chart.Series["Month"].XValueMember = "Item_Name";
            _Chart.Series["Month"].YValueMembers = "NetAmt";

            _Chart.ChartAreas[0].AxisX.Interval = 1;
            _Chart.Series["Month"].ChartType = SeriesChartType.Column;
            if (_Chart.Titles.Count > 0)
                _Chart.Titles.RemoveAt(0);
            if (rdoSale.Checked || rdoPurchase.Checked)
            {
                if (_index == 1)
                    _Chart.Titles.Add("Amount in Lakh");
                else
                    _Chart.Titles.Add("Amount in Crore");
            }
            else if (!rdoOther.Checked && !rdoUser.Checked)
            {
                _Chart.Titles.Add("Amount in Lakh");
            }
            _Chart.DataBind();
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
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void btnBranchCode_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BILL CODE", Keys.Space);
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
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", Keys.Space);
                objSearch.ShowDialog();
                txtMonth.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void monthChart_Click(object sender, EventArgs e)
        {
            if (monthChart.Width != 485)
            {
                monthChart.Location = new System.Drawing.Point(32, 113);
                monthChart.Height = 250;
                monthChart.Width = 485;
                //monthChart.SendToBack();
            }
            else
            {
                monthChart.Location = new System.Drawing.Point(10, 105);
                monthChart.Height = 540;
                monthChart.Width = 1025;
                monthChart.BringToFront();

            }

        }

        private void qtrChart_Click(object sender, EventArgs e)
        {
            if (qtrChart.Width != 485)
            {
                qtrChart.Height = 250;
                qtrChart.Width = 485;
                qtrChart.Location = new System.Drawing.Point(540, 115);

                // qtrChart.SendToBack();
            }
            else
            {
                qtrChart.Location = new System.Drawing.Point(10, 105);
                qtrChart.Height = 540;
                qtrChart.Width = 1025;
                qtrChart.BringToFront();

            }
        }

        private void branchChart_Click(object sender, EventArgs e)
        {
            if (branchChart.Width != 485)
            {
                branchChart.Height = 250;
                branchChart.Width = 485;
                branchChart.Location = new System.Drawing.Point(32, 388);

                //  branchChart.SendToBack();
            }
            else
            {
                branchChart.Height = 540;
                branchChart.Width = 1025;
                branchChart.Location = new System.Drawing.Point(10, 105);
                branchChart.BringToFront();

            }
        }

        private void partyChart_Click(object sender, EventArgs e)
        {
            if (partyChart.Width != 485)
            {
                partyChart.Height = 250;
                partyChart.Width = 485;
                partyChart.Location = new System.Drawing.Point(540, 388);

                // partyChart.SendToBack();
            }
            else
            {
                partyChart.Location = new System.Drawing.Point(10, 105);
                partyChart.Height = 540;
                partyChart.Width = 1025;
                partyChart.BringToFront();

            }
        }

        private bool MinimizeChart()
        {
            if (monthChart.Width != 485)
            {
                monthChart.Location = new System.Drawing.Point(32, 113);
                monthChart.Height = 250;
                monthChart.Width = 485;
                return false;
            }
            else if (qtrChart.Width != 485)
            {
                qtrChart.Height = 250;
                qtrChart.Width = 485;
                qtrChart.Location = new System.Drawing.Point(540, 115);
                return false;
            }
            else if (branchChart.Width != 485)
            {
                branchChart.Height = 250;
                branchChart.Width = 485;
                branchChart.Location = new System.Drawing.Point(32, 388);
                return false;
            }
            else if (partyChart.Width != 485)
            {
                partyChart.Height = 250;
                partyChart.Width = 485;
                partyChart.Location = new System.Drawing.Point(540, 388);
                return false;
            }
            return true;
        }
    }
}
