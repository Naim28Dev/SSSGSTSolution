using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SSS
{
    public partial class PartyWiseLedgerSummary : Form
    {
        DataBaseAccess dba;
        string strMonthName = "",_strGroupName="",_strCategoryName="";
        DateTime startDate = MainPage.startFinDate, endDate = MainPage.endFinDate;

        public PartyWiseLedgerSummary(string strGroupName, DateTime sDate, DateTime eDate, int _month)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            lblLedger.Text = _strGroupName=strGroupName;
            startDate = sDate;
            endDate = eDate;
            strMonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(_month).ToUpper();
            lblDate.Text = "Month : " + strMonthName;

            GetAllPartyNameWithBalance(strGroupName, sDate, eDate, _month);
        }

        public PartyWiseLedgerSummary(string strGroupName,string strCategory, DateTime sDate, DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _strGroupName = strGroupName;
            _strCategoryName = strCategory;

            lblLedger.Text = strGroupName + " (" + _strCategoryName + ")" ;
                  
            startDate = sDate;
            endDate = eDate;
            lblDate.Text = "Date : " + sDate.ToString("dd/MM/yy") + " To " + eDate.ToString("dd/MM/yy");
            GetDetailsofGroupName();
        }

        private void GetDetailsofGroupName()
        {
            try
            {
                dgrdDetails.Rows.Clear();
                DateTime _eDate = endDate.AddDays(1);

                string strQuery = " Select PartyName,SUM(Amount) Amt from (Select (AccountID+' '+Name) PartyName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA Cross APPLY (SELECT Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and GroupName in ('" + _strGroupName + "') and Category in ('" + _strCategoryName + "')) SM Where Status='DEBIT' and Date>='" + startDate.ToString("MM/dd/yyyy") + "'  and Date<'" + _eDate.ToString("MM/dd/yyyy") + "' Group by AccountID,Name UNION ALL   "
                         + " Select(AccountID + ' ' + Name) PartyName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA Cross APPLY (SELECT Name from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID and GroupName  in ('" + _strGroupName + "') and Category  in ('" + _strCategoryName + "')) SM Where Status = 'CREDIT' and Date>='" + startDate.ToString("MM/dd/yyyy") + "'  and Date<'" + _eDate.ToString("MM/dd/yyyy") + "' Group by AccountID,Name )Sales Group by PartyName Order by PartyName ";
                
                DataTable _dt = dba.GetDataTable(strQuery);
                double dAmt = 0, dTotalAmt = 0;
                if (_dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(_dt.Rows.Count);
                    int _index = 0;
                    foreach (DataRow row in _dt.Rows)
                    {
                        dTotalAmt += dAmt = dba.ConvertObjectToDouble(row["Amt"]);
                        dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1) + ".";
                        dgrdDetails.Rows[_index].Cells["partyName"].Value = row["PartyName"];
                        if (dAmt >= 0)
                            dgrdDetails.Rows[_index].Cells["debitAmt"].Value = dAmt;
                        else
                            dgrdDetails.Rows[_index].Cells["creditAmt"].Value = Math.Abs(dAmt);

                        _index++;
                    }
                }

                if (dTotalAmt >= 0)
                    lblTAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Debit";
                else
                    lblTAmount.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Credit";
            }
            catch
            {
            }
        }


        private void GetAllPartyNameWithBalance(string strGroupName,DateTime sDate, DateTime eDate,int _month)
        {
            try
            {
                DataTable dt = dba.GetAllDetailsByGroupName(strGroupName,sDate, eDate, _month);
                BindRecordWithControl(dt);
            }
            catch
            {
            }
        }

        private void BindRecordWithControl(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            double dAmt = 0, dTotalAmt = 0;
            try
            {
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count);
                        int rowIndex = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            dTotalAmt += dAmt = Convert.ToDouble(row["Amount"]);
                            dgrdDetails.Rows[rowIndex].Cells["sno"].Value = (rowIndex + 1) + ".";
                            dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["Name"];
                            if (dAmt >= 0)
                                dgrdDetails.Rows[rowIndex].Cells["debitAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            else
                                dgrdDetails.Rows[rowIndex].Cells["creditAmt"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                            rowIndex++;
                        }
                    }
                }
            }
            catch
            {
            }

            lblTAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy);
            if (dTotalAmt > 0)
                lblTAmount.Text += " Dr";
            else
                lblTAmount.Text += " Cr";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PartyWiseLedgerSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (lblLedger.Text != "" && e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    ShowDetailPage();
                }
            }
            catch
            {
            }
        }

        private void ShowDetailPage()
        {
            //int rowIndex = dgrdDetails.CurrentRow.Index;

            string strPartyName = Convert.ToString(dgrdDetails.CurrentRow.Cells["partyName"].Value);
            LedgerAccount objLedger = new LedgerAccount();
            objLedger.txtParty.Text = strPartyName;
            objLedger.txtMonthName.Text = strMonthName;
            objLedger.GetCurrentQuarterDetails();
            objLedger.ShowInTaskbar = true;
            objLedger.chkDate.Checked = true;
            objLedger.txtFromDate.Text = startDate.ToString("dd/MM/yyyy");
            objLedger.txtToDate.Text = endDate.ToString("dd/MM/yyyy");
            objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objLedger.TopLevel = true;
            objLedger.BringToFront();
            objLedger.Focus();
            objLedger.GetCurrentQuarterDetails();
            objLedger.Show();
        }
    }
}
