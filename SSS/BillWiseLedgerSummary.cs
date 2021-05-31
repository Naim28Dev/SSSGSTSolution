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
    public partial class BillWiseLedgerSummary : Form
    {
        DataBaseAccess odba;     

        public BillWiseLedgerSummary(string strAccountType,DateTime sDate,DateTime eDate, int rowIndex)
        {
            InitializeComponent();
            odba = new DataBaseAccess();
            lblLedger.Text = strAccountType;
            GetDataFromDataBase(rowIndex, sDate, eDate);
        }

        private void GetDataFromDataBase(int month, DateTime sDate, DateTime eDate)
        {
            try
            {
                DataTable dt = null;
                string strQuery = "";
                DateTime _eDate = eDate.AddDays(1);

                if (lblLedger.Text.ToUpper() == "SALES A/C")
                    strQuery = "Select Convert(varchar,BillDate,103) BillDate,(BillCode+' '+CAST(BillNo as varchar)) BillNo,dbo.GetFullName(SalePartyID) SalesParty,'----' as SupplierName,(CAST(NetAmt as Money)-TaxAmount) NetAmt from SalesRecord Where DatePart(MM,BillDate)=" + month+ " and BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and BillDate<'" + _eDate.ToString("MM/dd/yyyy") + "'  order by BillNo ";
                else if (lblLedger.Text.ToUpper() == "PURCHASE A/C")
                    strQuery = "Select Convert(varchar,BillDate,103) BillDate, (BillCode+' '+CAST(BillNo as varchar)) BillNo,dbo.GetFullName(SalePartyID) SalesParty,dbo.GetFullName(PurchasePartyID) AS SupplierName,(CAST(NetAmt as Money)-TaxAmount) NetAmt from PurchaseRecord Where DatePart(MM,BillDate)=" + month + "  and BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and BillDate<'" + _eDate.ToString("MM/dd/yyyy") + "' order by BillNo ";
                else if (lblLedger.Text.ToUpper() == "SALE RETURN")
                    strQuery = "Select Convert(varchar,Date,103) BillDate,(BillCode+' '+CAST(BillNo as varchar)) BillNo,dbo.GetFullName(SalePartyID) SalesParty,'----' as SupplierName,(NetAmt-TaxAmount)NetAmt from SaleReturn Where DatePart(MM,Date)=" + month + "  and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + _eDate.ToString("MM/dd/yyyy") + "' order by BillNo ";
                else if (lblLedger.Text.ToUpper() == "PURCHASE RETURN")
                    strQuery = "Select Convert(varchar,Date,103) BillDate, (BillCode+' '+CAST(BillNo as varchar)) BillNo,'----' SalesParty,dbo.GetFullName(PurchasePartyID) SupplierName,(NetAmt-TaxAmount)NetAmt from PurchaseReturn Where DatePart(MM,Date)=" + month + "  and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + _eDate.ToString("MM/dd/yyyy") + "' order by BillNo ";
                else if (lblLedger.Text.ToUpper() == "SALE SERVICE")
                    strQuery = "Select Convert(varchar,Date,103) BillDate, (BillCode+' '+CAST(BillNo as varchar)) BillNo,dbo.GetFullName(SalePartyID) SalesParty,'-----' SupplierName,(NetAmt-TaxAmount) NetAmt from SaleServiceBook Where DatePart(MM,Date)=" + month + "  and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + _eDate.ToString("MM/dd/yyyy") + "' order by BillNo ";
                if (strQuery != "")
                {
                    dt = odba.GetDataTable(strQuery);
                    BindRecordWithGrid(dt);
                }
                if (dgrdLedgerSummery.Rows.Count < 1)
                {
                    this.Close();
                }
            }
            catch
            {
            }
        }

        private void AddSaleRecord()
        {
          
        }

        private void AddPurchaseRecord()
        {
           
        }

        private void BindRecordWithGrid(DataTable dt)
        {
            try
            {
                double dAmt = 0, dTotalAmt = 0;
                int rowIndex = 0;
                dgrdLedgerSummery.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdLedgerSummery.Rows.Add(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    {
                        dTotalAmt += dAmt = ConvertObjectToDouble(dr["NetAmt"]);
                        dgrdLedgerSummery.Rows[rowIndex].Cells["date"].Value = dr["BillDate"];
                        dgrdLedgerSummery.Rows[rowIndex].Cells["billNo"].Value = dr["BillNo"];
                        dgrdLedgerSummery.Rows[rowIndex].Cells["SaleParty"].Value = dr["SalesParty"];
                        dgrdLedgerSummery.Rows[rowIndex].Cells["purchaseParty"].Value = dr["SupplierName"];
                        dgrdLedgerSummery.Rows[rowIndex].Cells["Amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                        rowIndex++;
                    }
                }
                if (lblLedger.Text == "SALES A/C" || lblLedger.Text == "PURCHASE RETURN" || lblLedger.Text == "SALE SERVICE")
                    lblTAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                else
                    lblTAmount.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Cr"; ;

            }
            catch
            {
            }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dAmount = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dAmount = Convert.ToDouble(objValue);
            }
            catch
            {
            }
            return dAmount;
        }

        private void PartyWiseLedgerSummery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        

        #region Printing
         

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
               
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        

        private void btnPrint_Click(object sender, EventArgs e)
        {
           
        }       

        #endregion

        private void dgrdLedgerSummery_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    string strBillNo = Convert.ToString(dgrdLedgerSummery.CurrentRow.Cells["billNo"].Value);
                    string[] strNo = strBillNo.Split(' ');
                    if (strNo.Length > 1)
                    {
                        ShowDetail(strNo[0], strNo[1]);
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdLedgerSummery_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdLedgerSummery.CurrentRow.Index;
                    if (dgrdLedgerSummery.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdLedgerSummery.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdLedgerSummery.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdLedgerSummery.CurrentRow.Index >= 0 && dgrdLedgerSummery.CurrentCell.ColumnIndex == 1)
                    {
                        string strBillNo = Convert.ToString(dgrdLedgerSummery.CurrentRow.Cells[1].Value);
                        string[] strNo = strBillNo.Split(' ');
                        if (strNo.Length > 1)
                        {
                            ShowDetail(strNo[0], strNo[1]);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowDetail(string strCode,string strBillNo)
        {
            string strLedger = lblLedger.Text.ToUpper();
            if (strLedger == "SALES A/C")
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    odba.ShowSaleBookPrint(strCode, strBillNo,false, false);
                }
                else
                {
                    SaleBook objSaleBook = new SaleBook(strCode, strBillNo);
                    objSaleBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSaleBook.ShowInTaskbar = true;
                    objSaleBook.Show();
                }
            }
            else if (strLedger == "PURCHASE A/C")
            {
                PurchaseBook objPurchaseBook = new PurchaseBook(strCode, strBillNo);
                objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchaseBook.ShowInTaskbar = true;
                objPurchaseBook.Show();
            }
            else if (strLedger == "SALE RETURN")
            {
                SaleReturn objSaleReturn = new SaleReturn(strCode, strBillNo);
                objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSaleReturn.ShowInTaskbar = true;
                objSaleReturn.Show();
            }
            else if (strLedger == "PURCHASE RETURN")
            {
                PurchaseReturn objPurchaseReturn = new PurchaseReturn(strCode, strBillNo);
                objPurchaseReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchaseReturn.ShowInTaskbar = true;
                objPurchaseReturn.Show();
            }
            else if (strLedger == "SALE SERVICE")
            {
                SaleServiceBook objSaleServiceBook = new SaleServiceBook(strCode, strBillNo);
                objSaleServiceBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSaleServiceBook.ShowInTaskbar = true;
                objSaleServiceBook.Show();
            }
        }      

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
