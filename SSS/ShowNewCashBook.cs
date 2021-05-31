using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class ShowNewCashBook : Form
    {
        DataBaseAccess dba; 
        MainPage mainObj;
       
        public ShowNewCashBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess(); 
            mainObj = MainPage.mymainObject as MainPage;
            SetCashData();    
        }

        private void SetCashData()
        {
            try
            {
                dgrdCashBook.Rows.Clear();
                lblBalAmount.Text = "0.00";
                string strQuery = "";
                strQuery = "Select dbo.GetFullName(PartyName) PartyName, ISNULL(Sum(Amount),0) Amt from ( Select PartyName, Sum(Cast(Amount as Money)) Amount from BalanceAmount Where Status='DEBIT' and PartyName in (Select (AreaCode+AccountNo) from SupplierMaster Where GroupName='CASH A/C') Group By PartyName Union All "
                              + " Select PartyName, -Sum(Cast(Amount as Money)) Amount from BalanceAmount Where Status='CREDIT'  and PartyName in (Select (AreaCode+AccountNo) from SupplierMaster Where GroupName='CASH A/C') Group By PartyName) Balance Group By PartyName";
                DataTable dt = dba.GetDataTable(strQuery);
                double dAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dBalance = 0;
                if (dt.Rows.Count > 0)
                {
                    int rowIndex = 0;
                    dgrdCashBook.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dAmt = Convert.ToDouble(row["Amt"]);

                        dgrdCashBook.Rows[rowIndex].Cells["chk"].Value = true;
                        dgrdCashBook.Rows[rowIndex].Cells["account"].Value = row["PartyName"];
                        if (dAmt >= 0)
                        {
                            dgrdCashBook.Rows[rowIndex].Cells["debit"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            dDebitAmt += dAmt;
                        }
                        else
                        {
                            dCreditAmt += dAmt;
                            dgrdCashBook.Rows[rowIndex].Cells["credit"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                        }
                        dBalance = dDebitAmt - dCreditAmt;
                        if (dBalance >= 0)
                            dgrdCashBook.Rows[rowIndex].Cells["balance"].Value = dBalance.ToString("N2", MainPage.indianCurancy) + " Dr";
                        else
                            dgrdCashBook.Rows[rowIndex].Cells["balance"].Value = Math.Abs(dBalance).ToString("N2", MainPage.indianCurancy) + " Cr";
                        rowIndex++;
                    }
                }
                dBalance = dDebitAmt - dCreditAmt;
                if (dBalance >= 0)
                    lblBalAmount.Text = dBalance.ToString("N2", MainPage.indianCurancy) + " Dr";
                else
                    lblBalAmount.Text = Math.Abs(dBalance).ToString("N2", MainPage.indianCurancy) + " Cr";
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Setting Party Data in Show Account Details", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowCashBook_KeyDown(object sender, KeyEventArgs e)
        {
           if (e.KeyValue == 27)
            {
                this.Close();
            }  
        }              
        
        private void btnPrint_Click(object sender, EventArgs e)
        {          
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {           
        }
                   
        private void dgrdCashBook_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                e.Cancel = true;
            }
        }
    
        private void dgrdCashBook_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string strCashName = Convert.ToString(dgrdCashBook.CurrentRow.Cells["account"].Value);               
                if (strCashName != "")
                {
                    LedgerAccount objLedger = new LedgerAccount(strCashName);
                    objLedger.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objLedger.Show();
                }
            }
            catch
            {
            }
        }

        private void dgrdCashBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string strCashName = Convert.ToString(dgrdCashBook.CurrentRow.Cells["account"].Value);
                if (strCashName != "")
                {
                    LedgerAccount objLedger = new LedgerAccount(strCashName);
                    objLedger.MdiParent = mainObj;
                    objLedger.Show();                  
                }
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
