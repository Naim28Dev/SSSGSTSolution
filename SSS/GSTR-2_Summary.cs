using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class GSTR_2_Summary : Form
    {
        DataBaseAccess dba;
        public GSTR_2_Summary()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            GetDataTableFromDB();
            btnGo.Enabled = true;
        }

        private string CreateSubQuery()
        {
            string strSubQuery = "";
            if (chkDate.Checked)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strSubQuery += " and SR.BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }
            if (txtMonth.Text != "")
            {
                strSubQuery += " and UPPER(DATENAME(MM,SR.BillDate))='" + txtMonth.Text + "' ";
            }
            return strSubQuery;
        }

        private void GetDataTableFromDB()
        {
            string strQuery = "", strSubQuery = CreateSubQuery();
            ClearAllRecord();

            //if (rdoReturnFormat.Checked)
            //{
            strQuery = " Select 'B2B Invoices - 3' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money)-CAST(SR.Tax as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = (SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from PurchaseBook SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " UNION ALL  "
                     //+ " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " UNION ALL "
                     + " Select (JVD.VoucherCode+' '+ CAST(JVD.VoucherNo as varchar)) as BillNo,BA.Date as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) NetAmt  from JournalVoucherDetails JVD left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA   Where SM.GroupII!='UNAUTHORISED' and SM.GroupII!='' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + " )_Sales Union ALL "
                     + " Select 'B2BUR (4B)' as BillType,COUNT(*) VchCount,ISNULL(SUM(TaxableAmt), 0) as TaxableAmt,ISNULL(SUM(IGSTAmt), 0) as IGSTAmt,ISNULL(SUM(CGSTAmt), 0) as CGSTAmt,ISNULL(SUM(SGSTAmt), 0) as SGSTAmt,ISNULL(SUM(IGSTAmt + CGSTAmt + SGSTAmt), 0) TaxAmt,ISNULL(SUM(NetAmt),0) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = (SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED'  " + strSubQuery + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from PurchaseBook SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED'  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " UNION ALL  "
                     // + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " UNION ALL "
                     + " Select (JVD.VoucherCode+' '+ CAST(JVD.VoucherNo as varchar)) as BillNo,BA.Date as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) NetAmt  from JournalVoucherDetails JVD left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where SM.GroupII='UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + " )_Sales Union ALL "
                     + " Select 'Credit/Debit Notes(Registered) - 9B' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,PR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,((CAST(PR.NetAmt as money) - PR.TaxAmount)*CreditNoteStatus) TaxableAmt,CAST((GD.IGSTAmt*CreditNoteStatus) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*CreditNoteStatus) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*CreditNoteStatus) as numeric(18, 2)) SGSTAmt,CAST((PR.NetAmt*CreditNoteStatus) as Money) NetAmt from PurchaseReturn PR inner join SupplierMaster SM on PR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN BillType='CREDITNOTE' then -1 else 1 end)CreditNoteStatus,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = PR.BillCode and GD.BillNo = PR.BillNo Group by BillType,TaxType) GD Where SM.GroupII != 'UNAUTHORISED'  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + " UNION ALL "
                     + " Select (JVD.VoucherCode+' '+ CAST(JVD.VoucherNo as varchar)) as BillNo,BA.Date as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) NetAmt  from JournalVoucherDetails JVD left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "BA.Date") + " )SaleReturn UNION ALL "
                     + " Select 'Credit/Debit Notes(Unregistered) - 9B' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,PR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,((CAST(PR.NetAmt as money) - PR.TaxAmount)*CreditNoteStatus) TaxableAmt,CAST((GD.IGSTAmt*CreditNoteStatus) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*CreditNoteStatus) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*CreditNoteStatus) as numeric(18, 2)) SGSTAmt,CAST((PR.NetAmt*CreditNoteStatus) as Money) NetAmt from PurchaseReturn PR inner join SupplierMaster SM on PR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN BillType='CREDITNOTE' then -1 else 1 end)CreditNoteStatus,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = PR.BillCode and GD.BillNo = PR.BillNo Group by BillType,TaxType) GD Where SM.GroupII = 'UNAUTHORISED'  " + strSubQuery.Replace("SR.BillDate", "PR.Date") +  " UNION ALL "
                     + " Select (JVD.VoucherCode+' '+ CAST(JVD.VoucherNo as varchar)) as BillNo,BA.Date as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) NetAmt  from JournalVoucherDetails JVD left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') and SM.GroupII = 'UNAUTHORISED'  " + strSubQuery.Replace("SR.BillDate", "BA.Date") + "  )SaleReturn UNION ALL "
                     + " Select 'Nil Rated Invoices - 8A, 8B, 8C, 8D' BillType,COUNT(*) VchCount,ISNULL(SUM(TaxableAmt),0) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,ISNULL(SUM(IGSTAmt+CGSTAmt+SGSTAmt),0) TaxAmt,ISNULL(SUM(NetAmt),0) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = (SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType) GD Where SR.TaxAmount = 0 " + strSubQuery + " )_Sales ";
            //}
            //else
            //{
            //    strQuery += " Select Region,ROUND(SUM((TaxAmt/TaxRate)*100),2) TaxableAmt,TaxRate,ROUND(SUM(IGSTAmt),2) IGSTAmt,ROUND(SUM(CGSTAmt/2),2) CGSTAmt,ROUND(SUM(CGSTAmt/2),2) SGSTAmt ,ROUND(SUM(TaxAmt),2)TaxAmt from ( "
            //             + " Select SMN.Region,GD.TaxRate,(CASE WHEN SMN.Region = 'INTERSTATE' then SUM(GD.TaxAmount) else 0 end)IGSTAmt,(CASE WHEN SMN.Region = 'LOCAL' then SUM(GD.TaxAmount) else 0 end)CGSTAmt,  SUM(GD.TaxAmount) TaxAmt from SalesRecord SR inner join SaleTypeMaster SMN on SR.SalesType = SMN.TaxName and SMN.SaleType = 'SALES' inner join GSTDetails GD on SR.BillCode = GD.BillCode and SR.BillNo = GD.BillNo Where SR.BillNo>0 " + strSubQuery + "  Group by SMN.Region,GD.GSTAccount,GD.TaxRate )_Tax Group by Region, TaxRate Order by Region,TaxRate ";
            //}

            //strQuery += " SELECT 'Invoice for Inword supply (Purchase)' BillType, MIN(BillNo) MINBillNo,MAX(BIllNo) MAXBillNo, (MAX(BillNo)-MIN(BIllNo)+1) NoOfBIll FROM SalesRecord SR Where SR.BIllNo>0 " + strSubQuery + " "
            //              + " ;WITH Missing (missnum, maxid) AS ( SELECT 1 AS missnum, (Select max(BillNo) from SalesRecord SR  Where SR.BIllNo>0 " + strSubQuery + ")   UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) SELECT 'SALES' as BillType,COUNT(Missnum) Missnum FROM Missing LEFT OUTER JOIN SalesRecord SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL OPTION (MAXRECURSION 0); ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 0)
            {
                BindRecordWithControl(ds.Tables[0]);                
            }
        }
        
        private void BindRecordWithControl(DataTable dt)
        {
            try
            {
                double dVch_Count = 0, dTaxableAmt = 0, dIGSTAmt = 0, dCGSTAmt = 0, dSGSTAmt = 0, dTotalTax = 0, dInvAmt = 0;

                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dVch_Count += dba.ConvertObjectToDouble(row["VchCount"]);
                        dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
                        dIGSTAmt += dba.ConvertObjectToDouble(row["IGSTAmt"]);
                        dCGSTAmt += dba.ConvertObjectToDouble(row["cgstAmt"]);
                        dSGSTAmt += dba.ConvertObjectToDouble(row["sgstAmt"]);
                        dTotalTax += dba.ConvertObjectToDouble(row["TaxAmt"]);
                        dInvAmt += dba.ConvertObjectToDouble(row["InvoiceAmt"]);

                        dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1) + " .";
                        dgrdDetails.Rows[_rowIndex].Cells["Particulars"].Value = row["BillTYpe"];
                        dgrdDetails.Rows[_rowIndex].Cells["voucherCount"].Value = row["VchCount"];
                        dgrdDetails.Rows[_rowIndex].Cells["taxableValue"].Value = row["TaxableAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["igstAmt"].Value = row["IGSTAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = row["cgstAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = row["sgstAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["totalTaxAmt"].Value = row["TaxAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["invoiceAmt"].Value = row["InvoiceAmt"];

                      
                        _rowIndex++;
                    }
                }

                lblVchCount.Text = dVch_Count.ToString("N0", MainPage.indianCurancy);
                lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                lblIGSTAmt.Text = dIGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblCGSTAmt.Text = dCGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblSGSTAmt.Text = dSGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblTotalTax.Text = dTotalTax.ToString("N2", MainPage.indianCurancy);
                lblInvoiceAmt.Text = dInvAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            ClearAllRecord();
        }

        private void ClearAllRecord()
        {
            dgrdDetails.Rows.Clear();
            dgrdDocSummary.Rows.Clear();
            lblCGSTAmt.Text = lblIGSTAmt.Text = lblInvoiceAmt.Text = lblSGSTAmt.Text = lblTaxableAmt.Text = lblTotalTax.Text = "0.00";
            lblVchCount.Text = "0";
            pnlDocSummary.Visible = false;
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
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
                    ClearAllRecord();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GSTR_1_Summary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlDocSummary.Visible)
                    pnlDocSummary.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    string strTaxvalue = Convert.ToString(dgrdDetails.CurrentRow.Cells["taxableValue"].Value);
                    if (strTaxvalue != "" && strTaxvalue != "0.00")
                    {
                        OpenGstDetails();
                    }
                }
            }
            catch { }
        }

        private void OpenGstDetails()
        {
            string strParticular = Convert.ToString(dgrdDetails.CurrentRow.Cells["Particulars"].Value);
            if (strParticular != "")
            {
                GST_VoucherRegister objGST = new SSS.GST_VoucherRegister();
                if (chkDate.Checked)
                {
                    objGST.strFromDate = txtFromDate.Text;
                    objGST.strToDate = txtToDate.Text;
                }
                objGST.strTaxType = strParticular;
                objGST.strMonthName = txtMonth.Text;
                objGST.strSummaryType = "GSTR2";
                objGST.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objGST.GetDataTableFromDB();
                objGST.ShowDialog();
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetails.CurrentRow.Index >= 0 && dgrdDetails.CurrentCell.ColumnIndex >= 0)
                    {
                        string strTaxvalue = Convert.ToString(dgrdDetails.CurrentRow.Cells["taxableValue"].Value);
                        if (strTaxvalue != "" && strTaxvalue != "0.00")
                        {
                            OpenGstDetails();
                        }
                    }
                }
            }
            catch { }
        }

        private void btnDocumentSummmary_Click(object sender, EventArgs e)
        {
            if (pnlDocSummary.Visible)
                pnlDocSummary.Visible = false;
            else
                pnlDocSummary.Visible = true;
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlDocSummary.Visible = false;
        }

        private void GetDatForExport()
        {
            string strQuery = "", strSubQuery = CreateSubQuery();
            strQuery += " Select * from (Select SM.GSTNo,(SR.InvoiceNo) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,(CAST(SR.NetAmt as Money)) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'Regular' InvoiceType,GD.TaxRate,ROUND((( CASE WHEN GD.TaxRate>0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,'' CessAmt_ITC from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode =(SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery + " Union ALL "
                     + " Select SM.GSTNo,(SR.InvoiceNo) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,(CAST(SR.NetAmt as Money)) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'Regular' InvoiceType,GD.TaxRate,ROUND((( CASE WHEN GD.TaxRate>0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,'' CessAmt_ITC from PurchaseBook SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode =SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " Union ALL  "
                     + " Select SM.GSTNo,(JVD.OriginalInvoiceNo) as BillNo,BA.Date as BillDate,(DiffAmt + IGSTAmt + CGSTAmt + SGSTAmt) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'Regular' InvoiceType,JVD.GSTPer,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,'' CessPaid,'Inputs' EligbleITC,IGSTAmt as IGSTAmt_ITC,CGSTAmt as CGSTAmt_ITC,SGSTAmt as SGSTAmt_ITC,'' CessAmt_ITC from JournalVoucherDetails JVD left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) left join StateMaster STM on SM.State = STM.StateName Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "BA.Date") + ") SR Order by BillNo "
                     + " Select * from (Select SM.Name,(SR.InvoiceNo) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) InvoiceType,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC, SR.BillNo as SBillNo from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode =(SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery + " UNION ALL "
                     + " Select SM.Name,(SR.InvoiceNo) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) InvoiceType,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC, SR.BillNo as SBillNo from PurchaseBook SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode =SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " )_Purchase Order by SBillNo "
                     //+ " Select SM.GSTNo,(SR.PurchaseBillCode + CAST(SR.PurchaseBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SR.PurchaseBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,'N' PreGST,(CASE WHEN BillType='CREDITNOTE' then 'D' else 'C' end) DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) SupplyType,NetAmt as VoucherValue,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select BillType,SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by BillType,TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("BillDate","Date") + " Order by SR.BillNo "
                     + " Select * from (Select SM.GSTNo,(SR.PurchaseBillCode + CAST(SR.PurchaseBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SR.PurchaseBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,'N' PreGST,(CASE WHEN BillType='CREDITNOTE' then 'D' else 'C' end) DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) SupplyType,NetAmt as VoucherValue,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select BillType,SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by BillType,TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("BillDate", "Date") + " UNION ALL "
                     + " Select SM.GSTNo,'' as SaleBillNo,NULL as SaleBillDate,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,BA.Date,6),' ','-') as BillDate,'N' PreGST,'D' DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN SUM(IGSTAmt)>0 then 'Inter State' When SUM(CGSTAmt)>0 then 'Intra State' else '' end) SupplyType,SUM(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) as VoucherValue,JVD.GSTPer,SUM(ROUND(((JVD.DiffAmt*100)/JVD.GSTPer),2)) TaxableAmt,SUM(CAST(JVD.IGSTAmt as numeric(18, 2)))IGSTAmt,SUM(CAST(JVD.CGSTAmt as numeric(18, 2)))CGSTAmt,SUM(CAST(JVD.CGSTAmt as numeric(18, 2))) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,SUM(CAST(JVD.IGSTAmt as numeric(18, 2)))IGSTAmt_ITC,SUM(CAST(JVD.CGSTAmt as numeric(18, 2)))CGSTAmt_ITC,SUM(CAST(JVD.CGSTAmt as numeric(18, 2))) SGSTAmt_ITC,0 CessAmt_ITC  from JournalVoucherDetails JVD left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) left join StateMaster STM on SM.State = STM.StateName Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "BA.Date") + " Group by SM.GSTNo,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)),JVD.GSTPer,BA.Date)_PurchaseReturn Order by BillNo "
                     //+ " Select (SR.PurchaseBillCode + CAST(SR.PurchaseBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SR.PurchaseBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,'N' PreGST,(CASE WHEN BillType='CREDITNOTE' then 'D' else 'C' end) DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) SupplyType,NetAmt as VoucherValue,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select BillType,SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by BillType,TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("BillDate", "Date") + " Order by SR.BillNo "
                     + " Select * from (Select (SR.PurchaseBillCode + CAST(SR.PurchaseBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SR.PurchaseBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,'N' PreGST,(CASE WHEN BillType='CREDITNOTE' then 'D' else 'C' end) DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN IGSTAmt>0 then 'Inter State' When CGSTAmt>0 then 'Intra State' else '' end) SupplyType,NetAmt as VoucherValue,GD.TaxRate,ROUND(((GD.TaxAmt*100)/GD.TaxRate),2) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt_ITC,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt_ITC,0 CessAmt_ITC from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select BillType,SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 4) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 4) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('PURCHASERETURN','CREDITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by BillType,TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' UNION ALL  "
                     + " Select '' as SaleBillNo,NULL as SaleBillDate,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,BA.Date,6),' ','-') as BillDate,'N' PreGST,'D' DocType,'01-Sales Return' ReasonForDoc,(CASE WHEN SUM(IGSTAmt)>0 then 'Inter State' When SUM(CGSTAmt)>0 then 'Intra State' else '' end) SupplyType,SUM(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) as VoucherValue,JVD.GSTPer,SUM(ROUND(((JVD.DiffAmt*100)/JVD.GSTPer),2)) TaxableAmt,SUM(CAST(JVD.IGSTAmt as numeric(18, 2)))IGSTAmt,SUM(CAST(JVD.CGSTAmt as numeric(18, 2)))CGSTAmt,SUM(CAST(JVD.CGSTAmt as numeric(18, 2))) SGSTAmt,'' CessPaid,'Inputs' EligbleITC,SUM(CAST(JVD.IGSTAmt as numeric(18, 2)))IGSTAmt_ITC,SUM(CAST(JVD.CGSTAmt as numeric(18, 2)))CGSTAmt_ITC,SUM(CAST(JVD.CGSTAmt as numeric(18, 2))) SGSTAmt_ITC,0 CessAmt_ITC  from JournalVoucherDetails JVD left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) left join StateMaster STM on SM.State = STM.StateName Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('CR. NOTE RECEIVED AGAINST PURCHASE') and SM.GroupII = 'UNAUTHORISED' Group by SM.GSTNo,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as nvarchar)),JVD.GSTPer,BA.Date)_PurchaseReturn Order by BillNo "
                     + " Select (CASE WHEN SLTM.Region = 'INTERSTATE' and SM.GroupII != 'UNAUTHORISED' then 'Inter-State supplies to registered persons' WHEN SLTM.Region = 'LOCAL' and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' then 'Intra-State supplies to registered persons' WHEN SLTM.Region = 'INTERSTATE' and SM.GroupII = 'UNAUTHORISED' then 'Inter-State supplies to unregistered persons' WHEN SLTM.Region = 'LOCAL' and SM.GroupII = 'UNAUTHORISED' then 'Intra-State supplies to unregistered persons' end) SaleDescription, (CASE WHEN SLTM.TaxationType = 'ZERORATED' then(SUM(CAST(SR.NetAmt as money))) else 0 end) NilRatedAmt,(CASE WHEN SLTM.TaxationType = 'EXEMPT' then(SUM(CAST(SR.NetAmt as money))) else 0 end) ExemptAmt,(CASE WHEN SLTM.TaxationType = 'NONGST' then(SUM(CAST(SR.NetAmt as money))) else 0 end) NonGSTAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) Outer Apply (Select TOP 1 StateName from CompanyDetails) CD Outer Apply(Select TOP 1 SLTM.Region, SLTM.TaxationType from SaleTypeMaster SLTM Where SLTM.SaleType = 'PURCHASE' and SLTM.TaxName = SR.TaxLedger) SLTM Where SR.TaxAmount = 0  " + strSubQuery + "  Group by SLTM.Region,SM.GroupII,SLTM.TaxationType "
                     + " Select HSNCode,ItemName,UQC,SUM(Qty) Quantity,SUM(TotalValue)TotalValue,SUM(NetAMt)NetAmt,SUM(IGSTAmt) IGSTAmt,SUM(SGSTAmt) SGSTAmt,SUM(SGSTAmt) SGSTAmt from ( Select BillNo,HSNCode,ItemName,ISNULL((Select Top 1 UM.FormalName from Items _IM left join UnitMaster UM on UM.UnitName=_IM.UnitName Where _IM.ItemName=_Purchase.ItemName),'')UQC,Qty,(NetAmt+TaxAmt) TotalValue,NetAmt,(CASE WHEN Region='INTERSTATE' then TaxAmt else 0 end) IGSTAmt,(CASE WHEN Region='LOCAL' then (TaxAmt/2) else 0 end) CGSTAmt,(CASE WHEN Region='LOCAL' then (TaxAmt/2) else 0 end) SGSTAmt from ( "
                     + " Select BillNo,Region, HSNCode, ItemName, SUM(Amount)NetAmt, ROUND(SUM((Amount * TaxRate) / 100), 2) TaxAmt, SUM(Quantity) Qty from( "
                     + " Select BillNo,SMN.Region, (GM.HSNCode) as HSNCode,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(GRD.ItemName,':',''),',',''),'/',''),'-',''),'.','')ItemName, GRD.Quantity, ROUND(((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)) * (100 + (CAST((PR.DiscountStatus + PR.Discount) as Money) - (CASE WHEN (SM.Category = 'CASH PURCHASE' OR TINNumber = 'CASH PURCHASE') then 5 else 3 end)+(CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end))))/ 100.00),2)Amount,GM.TaxRate from PurchaseRecord PR inner join  GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((PR.DiscountStatus + PR.Discount) as Money) - (CASE WHEN (SM.Category='CASH PURCHASE' OR SM.TINNumber = 'CASH PURCHASE') then 5 else 3 end)+(CASE WHEN (Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end))) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(CAST((PR.DiscountStatus + PR.Discount) as Money) - (CASE WHEN (SM.Category='CASH PURCHASE' OR SM.TINNumber = 'CASH PURCHASE') then 5 else 3 end)+(CASE WHEN (SM.Category='CLOTH PURCHASE' OR BillCode Like('%SRT%')  OR BillCode Like('%CCK%')) then 1 else 0 end))) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0  " + strSubQuery.Replace("SR.","PR.") + " Union All "
                     + " Select PR.BillNo,SMN.Region, (GM.HSNCode) as HSNCode,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(GRD.ItemName,':',''),',',''),'/',''),'-',''),'.','')ItemName, GRD.Qty as Quantity,  ROUND((((GRD.Amount*(100.00- PR.DiscPer))/ 100.00)* (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,GM.TaxRate from PurchaseBook PR inner join  PurchaseBookSecondary GRD on PR.BillCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100) / (100 + TaxRate)) else GRD.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GRD.SDisPer-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-PR.DiscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.MRP * 100) / (100 + TaxRate)) else GRD.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-GRD.SDisPer-SpecialDscPer-DiscPer) / 100.00) else 1.00 end)*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00-PR.DiscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where GRD.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + " Union All "
                     + " Select BillNo,SMN.Region,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'Service Charge' as ItemName,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseRecord PR inner join GoodsReceiveDetails GRD on PR.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where (GRD.PackingAmt + GRD.FreightAmt) > 0 " + strSubQuery.Replace("SR.", "PR.") + "  Union All "
                     + " Select PR.BillNo,SMN.Region,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'Service Charge' as ItemName,0 as Quantity,ROUND(((GRD.OCharges-GRD.Discount) * (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)),2) Amount,PR.TaxPer as TaxRate from PurchaseBook PR inner join PurchaseBookSecondary GRD on PR.BillCode=GRD.BillCode and PR.BillNo=GRD.BillNo left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar))  Where (GRD.OCharges-GRD.Discount) > 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + "  Union All "
                     + " SELECT BillNo,SMN.Region,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'Service Charge' as ItemName,0 as Quantity,(CAST(OtherPer as Money) + CAST(Others as MOney)) as Amount,PR.TaxPer as TaxRate  from PurchaseRecord PR left join SaleTypeMaster SMN On PR.TaxLedger = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Where PR.BillNo != 0 " + strSubQuery.Replace("SR.", "PR.") + " UNION ALL "
                     + " SELECT BillNo,SMN.Region,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode != ''),'') as HSNCode,'Service Charge' as ItemName,0 as Quantity,((PR.PackingAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)))* (CASE WHEN SMN.TaxIncluded = 1 then((100) / (100 + PR.TaxPer)) else 1 end)) as Amount,PR.TaxPer as TaxRate  from PurchaseBook PR left join SaleTypeMaster SMN On PR.PurchaseType = SMN.TaxName  and SMN.SaleType = 'PURCHASE' left Join SupplierMaster SM on PR.PurchasePartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Where PR.BillNo != 0 " + strSubQuery.Replace("SR.BillDate", "PR.Date") + " UNION ALL "
                     + " Select JVD.VoucherNo as BillNo, Region,ISNULL(GM.HSNCode,'') HSNCode,JVD.Other as ItemName,0 as Quantity,DiffAmt as Amount,GSTPer as TaxRate from JournalVoucherDetails JVD left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) OUTER APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode from Items _IM inner join ItemGroupMaster _IGm on _IM.GroupName=_IGM.GroupName Where _IM.ItemName=JVD.Other) GM Cross Apply (Select  TOP 1 BA.GSTNature,BA.Date from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where BA.GSTNature in ('REGISTERED EXPENSE (B2B)','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "BA.Date") + " "
                     + " )_Purchase Group by BillNo,Region,HSNCode,ItemName)_Purchase)_Purchase Group by HSNCode, ItemName, UQC Order by HSNCode,ItemName ";
                    

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if(ds.Tables.Count>0)
            CreateExcelSheet(ds);
        }


        private string CreateExcelSheet(DataSet ds)
        {
            string[] strSheet = { "b2b", "b2bur", "imps","impg", "cdnr", "cdnur", "at", "atadj", "exemp","itcr", "hsnsum"};
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            if (strFileName != "")
            {
                try
                {
                    object misValue = System.Reflection.Missing.Value;
                    ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                    ExcelWorkBook.Worksheets.Add(misValue, misValue, strSheet.Length, NewExcel.XlSheetType.xlWorksheet);
                    int sheetIndex = 1;
                    foreach (string strName in strSheet)
                    {

                        ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[sheetIndex];
                        var range = ExcelWorkSheet.get_Range("A1", "Z10000");
                        range = range.EntireRow;
                        range.Font.Name = "Times New Roman";

                        SetColumnName(ref ExcelWorkSheet, strName, ds);
                        sheetIndex++;
                    }

                    ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    ExcelWorkBook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    Marshal.ReleaseComObject(ExcelWorkSheet);
                    Marshal.ReleaseComObject(ExcelWorkBook);
                    Marshal.ReleaseComObject(ExcelApp);

                    MessageBox.Show("Thanks ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    strFileName = ex.Message;
                }
                finally
                {
                    //foreach (Process process in Process.GetProcessesByName("Excel"))
                    //    process.Kill();
                }
            }
            return strFileName;
        }

        private void SetColumnName(ref NewExcel.Worksheet ExcelWorkSheet, string strSheetName,DataSet ds)
        {
            if (strSheetName == "b2b")
            {
                var range = ExcelWorkSheet.get_Range("D1", "D10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("H1", "M10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("O1", "R10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "GSTIN of Supplier", "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Reverse Charge", "Invoice Type", "Rate", "Taxable Value", "Integrated Tax Paid", "Central Tax Paid", "State/UT Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Central Tax", "Availed ITC State/UT Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary Of Supplies From Registered Suppliers B2B(3)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[0]);
            }
            else if (strSheetName == "b2bur")
            {
                var range = ExcelWorkSheet.get_Range("D1", "D10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("G1", "L10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("N1", "Q10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "Supplier Name", "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Supply Type", "Rate", "Taxable Value", "Integrated Tax Paid", "Central Tax Paid", "State/UT Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Central Tax", "Availed ITC State/UT Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary Of Supplies From Unregistered Suppliers B2BUR(4B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[1]);
            }
            else if (strSheetName == "imps")
            {
                var range = ExcelWorkSheet.get_Range("C1", "C10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("E1", "H10000");
                range.NumberFormat = "#######.00";             

                string[] strColumn = { "Invoice Number of Reg Recipient", "Invoice Date", "Invoice Value", "Place Of Supply", "Rate", "Taxable Value", "Integrated Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For IMPS (4C)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
            }
            else if (strSheetName == "impg")
            {
                string[] strColumn = { "Port Code", "Bill Of Entry Number", "Bill Of Entry Date", "Bill Of Entry Value", "Document type", "GSTIN Of SEZ Supplier", "Rate", "Taxable Value", "Integrated Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For IMPG (5)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
            }
            else if (strSheetName == "cdnr")
            {
                var range = ExcelWorkSheet.get_Range("J1", "U10000");
                range.NumberFormat = "#######.00";               

                string[] strColumn = { "GSTIN of Supplier", "Note/Refund Voucher Number", "Note/Refund Voucher date", "Invoice/Advance Payment Voucher Number", "Invoice/Advance Payment Voucher date", "Pre GST", "Document Type", "Reason For Issuing document", "Supply Type", "Note/Refund Voucher Value", "Rate", "Taxable Value", "Integrated Tax Paid", "Central Tax Paid", "State/UT Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Central Tax", "Availed ITC State/UT Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For CDNR(6C)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[2]);
            }
            else if (strSheetName == "cdnur")
            {
                var range = ExcelWorkSheet.get_Range("I1", "T10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "Note/Voucher Number", "Note/Voucher date", "Invoice/Advance Payment Voucher number", "Invoice/Advance Payment Voucher date", "Pre GST", "Document Type", "Reason For Issuing document", "Supply Type", "Note/Voucher Value", "Rate", "Taxable Value", "Integrated Tax Paid", "Central Tax Paid", "State/UT Tax Paid", "Cess Paid", "Eligibility For ITC", "Availed ITC Integrated Tax", "Availed ITC Central Tax", "Availed ITC State/UT Tax", "Availed ITC Cess" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[3]);
            }           
            else if (strSheetName == "at")
            {
                string[] strColumn = { "Place Of Supply", "Rate", "Gross Advance Received", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For  Tax Liability on Advance Paid  under reverse charge(10 A)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
            }
            else if (strSheetName == "atadj")
            {
                string[] strColumn = { "Place Of Supply", "Rate", "Gross Advance Adjusted", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Adjustment of advance tax paid earlier for reverse charge supplies (10 B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
            }            
            else if (strSheetName == "exemp")
            {
                var range = ExcelWorkSheet.get_Range("B1", "E10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "Description", "Composition taxable person", "Nil Rated Supplies", "Exempted (other than nil rated/non GST supply )", "Non-GST supplies" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Composition, Nil rated, exempted and non GST inward supplies (7)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[4]);
            }
            else if (strSheetName == "itcr")
            {
                string[] strColumn = { "To be added or reduced from output liability", "ITC Integrated Tax Amount", "ITC Central Tax Amount", "ITC State/UT Tax Amount", "ITC Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Composition, Nil rated, exempted and non GST inward supplies (7)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
            }
            else if (strSheetName == "hsnsum")
            {
                var range = ExcelWorkSheet.get_Range("D1", "J10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN","Description", "UQC", "Total Quantity", "Total Value", "Taxable Value", "Integrated Tax Amount", "Central Tax Amount", "State/UT Tax Amount", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For HSN(12)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[5]);
            }            
        }

        private void AddColumnsName(ref NewExcel.Worksheet ExcelWorkSheet, string[] strColumn)
        {
            int colIndex = 1, colNewIndex = 0;
            foreach (string strName in strColumn)
            {
                if (colIndex > 0)
                    ExcelWorkSheet.Cells[4, colIndex] = strName;
                colIndex++;
            }

            foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
            {
                column.ColumnWidth = (double)column.ColumnWidth + 10;
                if (colNewIndex >= colIndex)
                    break;
                colNewIndex++;
            }

            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 1];
            if (ExcelWorkSheet.Name != "itcr" && ExcelWorkSheet.Name != "hsnsum")
            {
                objRange.Font.ColorIndex = 2;// = Color.FromArgb(255, 255, 255); ;// ColorTranslator.ToOle((Color)cc.ConvertFromString("#FFFFFF"));
                objRange.Interior.ColorIndex = 49;//  Color.FromArgb(0, 112, 192);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#0070C0"));
                objRange.Cells.BorderAround();
                objRange.RowHeight = 32;
                objRange.WrapText = true;
            }

            for (int cIndex = 1; cIndex <= strColumn.Length; cIndex++)
            {
                objRange = (NewExcel.Range)ExcelWorkSheet.Cells[2, cIndex];
                objRange.Font.ColorIndex = 2;// = Color.FromArgb(255, 255, 255); ;// ColorTranslator.ToOle((Color)cc.ConvertFromString("#FFFFFF"));
                objRange.Interior.ColorIndex = 49;//  Color.FromArgb(0, 112, 192);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#0070C0"));
                objRange.Cells.BorderAround();

                objRange = (NewExcel.Range)ExcelWorkSheet.Cells[4, cIndex];
                objRange.Interior.ColorIndex = 40;// Color.FromArgb(248,203,173);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#F8CBAD"));
                objRange.Cells.BorderAround();
            }
           
        }

        private void SetDataInSheet(ref NewExcel.Worksheet ExcelWorkSheet, DataTable dt)
        {
            int rowIndex = 5,colIndex=1;
            foreach (DataRow row in dt.Rows)
            {
                colIndex = 1;
                for (; colIndex <= dt.Columns.Count; colIndex++)
                    ExcelWorkSheet.Cells[rowIndex, colIndex] = row[colIndex - 1];

                rowIndex++;
            }

            //for (int rIndex = 1; rIndex <= rowIndex; rIndex++)
            //{
            //    for (int cIndex = 1; cIndex < colIndex; cIndex++)
            //    {
            //        NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
            //        objRange.Cells.BorderAround();
            //    }
            //}
        }

        private void SetDataInSheetINDoc(ref NewExcel.Worksheet ExcelWorkSheet, DataSet ds)
        {
            int rowIndex = 5;

            DataTable dt = ds.Tables[5];
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                double dNoOfBill = dba.ConvertObjectToDouble(row["NoOfBIll"]), dCancelBillNo = 0;
                if (ds.Tables[6].Rows.Count > 0)
                    dCancelBillNo = dba.ConvertObjectToDouble(ds.Tables[6].Rows[0][1]);


                ExcelWorkSheet.Cells[rowIndex, 1] = row["BillType"];
                ExcelWorkSheet.Cells[rowIndex, 2] = row["MINBillNo"];
                ExcelWorkSheet.Cells[rowIndex, 3] = row["MAXBillNo"];
                ExcelWorkSheet.Cells[rowIndex, 4] = row["NoOfBIll"];
                ExcelWorkSheet.Cells[rowIndex, 5] = dCancelBillNo;
               
            }

            //for (int rIndex = 1; rIndex < rowIndex; rIndex++)
            //{
            //    for (int cIndex = 1; cIndex <= 5; cIndex++)
            //    {
            //        NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
            //        objRange.Cells.BorderAround();
            //    }
            //}
        }
        
        private string GetFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
            _browser.FileName = "GSTR-2.xls";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;
            
            return strPath;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    GetDatForExport();
                }
            }
            catch { }
            btnExport.Enabled = true;
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", Keys.Space);
                objSearch.ShowDialog();
                txtMonth.Text = objSearch.strSelectedData;
                ClearAllRecord();

            }
            catch
            {
            }
        }

        private void GSTR_2_Summary_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }
    }

}
