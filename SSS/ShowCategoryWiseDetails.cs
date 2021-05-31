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
    public partial class ShowCategoryWiseDetails : Form
    {
        DataBaseAccess dba;
        double dTotalAmt = 0;
        DateTime startDate = MainPage.startFinDate, endDate = MainPage.endFinDate;
        string _strGroupName = "";
        public ShowCategoryWiseDetails(string strGroupName,DateTime sDate,DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            lblGroupName.Text = _strGroupName= strGroupName;
            startDate = sDate;
            endDate = eDate;
            GetDetailsofGroupName(strGroupName);
            
            lblDate.Text = "Date : " + sDate.ToString("dd/MM/yy") + " To " + eDate.ToString("dd/MM/yy");
        }

        private void GetDetailsofGroupName(string strGroupName)
        {
            try
            {
                dgrdDetails.Rows.Clear();
              DateTime _eDate = endDate.AddDays(1);

                string strQuery = " Select Category,SUM(Amount) Amt from (Select Category, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA Cross APPLY (SELECT Category from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=BA.AccountID and GroupName in ('" + strGroupName + "')) SM Where Status='DEBIT' and  Date>='" + startDate.ToString("MM/dd/yyyy") + "'  and Date<'" + _eDate.ToString("MM/dd/yyyy") + "'  Group by Category UNION ALL  "
                                + " Select Category, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA Cross APPLY (SELECT Category from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID and GroupName in ('" + strGroupName + "') ) SM Where Status = 'CREDIT'  and  Date>='" + startDate.ToString("MM/dd/yyyy") + "'  and Date<'" + _eDate.ToString("MM/dd/yyyy") + "'  Group by Category )Sales Group by Category Order by Category ";

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
                        dgrdDetails.Rows[_index].Cells["particulars"].Value = row["Category"];
                        if (dAmt >= 0)
                            dgrdDetails.Rows[_index].Cells["debitAmt"].Value = dAmt;
                        else
                            dgrdDetails.Rows[_index].Cells["creditAmt"].Value = Math.Abs(dAmt);

                        _index++;
                    }
                }

                if (dTotalAmt >= 0)
                    lblTotalAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Debit";
                else
                    lblTotalAmt.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Credit";
            }
            catch
            {
            }
        }

          
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowBalanceSheetDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                    ShowVoucherDetails();
            }
            catch
            {
            }
        }

        private void ShowVoucherDetails()
        {
            string strValue = Convert.ToString(dgrdDetails.CurrentCell.Value);           

            if (strValue == "CLOSING STOCK")
            {
                StockRegister objStock = new StockRegister(endDate);
                objStock.MdiParent = MainPage.mymainObject;
                objStock.Show();
            }
            else if (strValue != "")
            {
                PartyWiseLedgerSummary objPartyWiseLedgerSummary = new PartyWiseLedgerSummary(_strGroupName,strValue, startDate, endDate);
                objPartyWiseLedgerSummary.MdiParent = MainPage.mymainObject;
                objPartyWiseLedgerSummary.ShowInTaskbar = true;
                objPartyWiseLedgerSummary.Show();
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentCell.ColumnIndex==1 && dgrdDetails.CurrentCell.RowIndex >= 0)
                    ShowVoucherDetails();
            }
            catch
            {
            }
        }
    }
}
