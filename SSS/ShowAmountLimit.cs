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
    public partial class ShowAmountLimit : Form
    {
        DataBaseAccess dba;       
     
        public ShowAmountLimit()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetData();
        }
      
        public void GetData()
        {
            try
            {
                string strQuery = " Select * from ( Select *,(CASE WHEN AMountLimit>0 then (BalanceAmt*100)/AmountLimit else 0 end)UsedPer from ( "
                                       + " Select (ISNULL(AreaCode,'')+ISNULL(CAST(AccountNo as varchar),'')+' '+Name)PartyName,GroupName,AmountLimit,ExtendedAmt, "
                                       + " (Select SUM(Amt) from (Select ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='DEBIT' and SM.GroupName not in ('SUB PARTY') and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')) Union All Select -ISNULL(SUM(CAST(ISNULL(Amount,0) as Money)),0)Amt from BalanceAmount Where Status='CREDIT' and SM.GroupName not in ('CAPITAL ACCOUNT','SUB PARTY') and AccountID=(ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,''))) Balance) BalanceAmt  "
                                       + " from SupplierMaster SM Where Name!=''  and SM.GroupName in ('SUNDRY DEBTORS'))Supplier) NewSupplier Order by UsedPer desc ";
                DataTable table = dba.GetDataTable(strQuery);
                if (table.Rows.Count > 0)
                {
                    int rowIndex = 0;
                    dgrdPartyDetails.Rows.Add(table.Rows.Count);
                    double dAmtLimit = 0, dAddAmtLimit = 0, dBalanceAmt = 0,dLimit=0;
                    foreach (DataRow row in table.Rows)
                    {
                        dAmtLimit = dba.ConvertObjectToDouble(row["AmountLimit"]);
                        dAddAmtLimit = dba.ConvertObjectToDouble(row["ExtendedAmt"]);
                        dBalanceAmt = dba.ConvertObjectToDouble(row["BalanceAmt"]);
                        dLimit = dba.ConvertObjectToDouble(row["UsedPer"]);

                        dgrdPartyDetails.Rows[rowIndex].Cells["PartyName"].Value = row["PartyName"];
                        dgrdPartyDetails.Rows[rowIndex].Cells["groupName"].Value = row["GroupName"];
                        dgrdPartyDetails.Rows[rowIndex].Cells["AmountInUse"].Value = dAmtLimit.ToString("N2", MainPage.indianCurancy);
                        dgrdPartyDetails.Rows[rowIndex].Cells["additionalAmt"].Value = dAddAmtLimit.ToString("N2", MainPage.indianCurancy);
                        dgrdPartyDetails.Rows[rowIndex].Cells["UsedAmount"].Value = dLimit.ToString("0.00") + " %" ;
                        if(dBalanceAmt>=0)
                            dgrdPartyDetails.Rows[rowIndex].Cells["CurrentBalance"].Value = dBalanceAmt.ToString("N2", MainPage.indianCurancy)+ " Dr";
                        else
                            dgrdPartyDetails.Rows[rowIndex].Cells["CurrentBalance"].Value = Math.Abs(dBalanceAmt).ToString("N2", MainPage.indianCurancy)+ " Cr";
                        rowIndex++;
                    }
                }               
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Get Data in Show Amount Limit ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }        

        private void ShowAmountLimit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdPartyDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;           
        }

        private void ShowAmountLimit_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdPartyDetails);
        }
    }
}
