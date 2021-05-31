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
    public partial class ShowBalanceSheetDetails : Form
    {
        DataBaseAccess dba;
        double dTotalAmt = 0;
        DateTime startDate = MainPage.startFinDate, endDate = MainPage.endFinDate;
        public ShowBalanceSheetDetails(string strCategory,DateTime sDate,DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            lblGroupName.Text = strCategory;
            startDate = sDate;
            endDate = eDate;
            GetDetailsofCategory(strCategory);
            
            lblDate.Text = "Date : " + sDate.ToString("dd/MM/yy") + " To " + eDate.ToString("dd/MM/yy");
        }

        private void GetDetailsofCategory(string strCategory)
        {
            try
            {
                if (strCategory == "CURRENT ASSETS")
                {
                    string[] strName = { "SUNDRY DEBTORS", "DEBTOR / MISCELLANEOUS", "BANK A/C", "CASH A/C", "CASH IN HAND", "DEPOSITS (ASSET)", "RESERVES & SURPLUSES", "CLOSING STOCK" };
                    GetDataFromDataBase(strName);
                }
                else if (strCategory == "CURRENT LIABILITY")
                {
                    string[] strName = { "SUNDRY CREDITOR", "DUTIES & TAXES","PROVISIONS", "RETAINED EARNINGS", "CREDITOR / MISCELLANEOUS", "TAX COLLECTION","CREDITOR EXPENSE" };
                    GetDataFromDataBase(strName);
                }
                else if (strCategory == "FIXED ASSETS")
                {
                    string[] strName = { "FIXED ASSETS", "FURNITURE/OFFICE ASSETS", "LAND / BUILDING", "INVESTMENTS" };
                    GetDataFromDataBase(strName);
                }
                else if (strCategory == "LOAN (LIABILITY)")
                {
                    string[] strName = { "LOAN (LIABILITY)", "SECURED LOANS", "UNSECURED LOANS" };
                    GetDataFromDataBase(strName);
                }
                else if (strCategory == "RESERVES & SURPLUSE")
                {
                    string[] strName = { "RESERVES & SURPLUSE", "PROFIT & LOSS A/C" };
                    GetDataFromDataBase(strName);
                }
            }
            catch
            {
            }
        }

        private void GetDataFromDataBase(string[] strData)
        {
            double dAmt = 0;

            foreach (string strName in strData)
            {
                if (strName == "CLOSING STOCK")
                    dAmt += dba.GetClosingStockAmount(endDate);
                else
                    dAmt = dba.GetGroupAmountFromQuery(strName, startDate, endDate, 0);
                if (dAmt != 0)
                    SetAmountAndDataWithControls(strName, dAmt);
            }

            if (dTotalAmt > 0)
                lblTotalAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else if (dTotalAmt < 0)
                lblTotalAmt.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            else
                lblTotalAmt.Text = "0.00";
        }

        private void SetAmountAndDataWithControls(string strData, double dAmount)
        {
            dTotalAmt += dAmount;
                int rowIndex = dgrdDetails.Rows.Count;
                dgrdDetails.Rows.Add();
                dgrdDetails.Rows[rowIndex].Cells["sNo"].Value = (rowIndex + 1) + ".";
                dgrdDetails.Rows[rowIndex].Cells["name"].Value = strData;
                if (dAmount > 0)
                    dgrdDetails.Rows[rowIndex].Cells["debitAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                else
                    dgrdDetails.Rows[rowIndex].Cells["creditAmt"].Value = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy);
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
            if (dgrdDetails.CurrentRow != null)
            {
                string strName = Convert.ToString(dgrdDetails.CurrentRow.Cells["name"].Value);
                if (strName != "")
                {
                    if (strName == "OPENING STOCK" || strName == "CLOSING STOCK")
                    {
                        StockRegister objStock = new StockRegister(endDate);
                        objStock.MdiParent = MainPage.mymainObject;
                        objStock.Show();
                    }
                    else
                    {
                        LedgerMonthlySummary objSummary = new LedgerMonthlySummary(strName, startDate, endDate);
                        objSummary.MdiParent = MainPage.mymainObject;
                        objSummary.Show();
                    }
                }
            }
        }

        private void ShowBalanceSheetDetails_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdDetails);
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
