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
    public partial class GST_VoucherRegister : Form
    {
        public string strFromDate = "", strToDate = "", strMonthName = "",strTaxType="",strSalesParty="",strSummaryType="",strStateName="";
        DataBaseAccess dba;

        public GST_VoucherRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GST_VoucherRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }            
        }

        private void rdoVoucherWise_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoVoucherWise.Checked)
            GetDataTableFromDB();
        }

        private void rdoPartyWise_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPartyWise.Checked)
                GetDataTableFromDB();
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetails.CurrentRow.Index >= 0 && dgrdDetails.CurrentCell.ColumnIndex >= 0)
                    {
                        OpenVoucherDetails();
                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                OpenVoucherDetails();
            }
        }

        public void GetDataTableFromDB()
        {
            lblVoucherOF.Text = strTaxType;
            
            string strQuery = "", strSubQuery = "", strSBillCOde = "", strPBillCode = "", strSRBillCode = "", strPRBillCode = "", strJournalVCode = "", strSaleServiceVCode="";
            dgrdDetails.Rows.Clear();
            lblCGSTAmt.Text = lblIGSTAmt.Text = lblInvoiceAmt.Text = lblSGSTAmt.Text = lblTaxableAmt.Text = lblTotalTax.Text = "0.00";
            lblVchCount.Text = "0";

            if (strFromDate.Length==10 && strToDate.Length==10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(strFromDate), eDate = dba.ConvertDateInExactFormat(strToDate);
                strSubQuery += " and SR.BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (strMonthName != "")
            {
                strSubQuery += " and UPPER(DATENAME(MM,SR.BillDate))='" + strMonthName + "' ";
            }
            if (strSalesParty != "")
            {
                string[] strFullName = strSalesParty.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubQuery += " and SR.SalePartyID='" + strFullName[0] + "' ";
                }
            }
            if (strStateName != "")
            {
                strSBillCOde = " and SR.BillCode in (Select SBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
                strPBillCode = " and SR.BillCode in (Select PBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
                strSRBillCode = " and SR.BillCode in (Select GReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
                strPRBillCode = " and SR.BillCode in (Select PurchaseReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
                strJournalVCode = " and SR.VoucherCode in (Select JournalVCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
                strSaleServiceVCode = " and SR.BillCode in (Select SaleServiceCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + strStateName + "') ";
            }


            if (strTaxType == "B2B Invoices - 4A, 4B, 4C, 6B, 6C")
            {
                strQuery = " Select * from (Select 'SALES' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery + strSBillCOde + " UNION ALL "
                         + " Select 'SALESERVICE' as BillType, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo,SR.Date BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " )SR ";
            }
            else if (strTaxType == "B2B Invoices - 3")
            {
                strQuery = " Select * from (Select 'PURCHASE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - CAST(SR.Tax as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = (SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPBillCode + " Union ALL "
                         + " Select 'JOURNAL' BillType,(JVD.VoucherCode + ' ' + CAST(JVD.VoucherNo as varchar)) as BillNo,JVD.VoucherNo as _BillNo,JVD.InvoiceDate as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt + IGSTAmt + CGSTAmt + SGSTAmt) InvoiceAmt from JournalVoucherDetails JVD left Join SupplierMaster SM on JVD.PartyID in (SM.AreaCode + CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where SM.GroupII != 'UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','CR. NOTE RECEIVED AGAINST PURCHASE','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "InvoiceDate").Replace("SR.SalePartyID", "JVD.PartyID") + strJournalVCode.Replace("SR.","JVD.") + ")SR  "; 
            }
            else if (strTaxType == "B2BUR (4B)")
            {
                strQuery = " Select * from (Select 'PURCHASE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money)-CAST(SR.Tax as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseRecord SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASE' and GD.BillCode = (SUBSTRING(SR.GRSNO,0,CHARINDEX(' ',SR.GRSNo,0))) and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPBillCode 
                          + " Select 'JOURNAL' BillType,(JVD.VoucherCode+' '+ CAST(JVD.VoucherNo as varchar)) as BillNo,InvoiceDate as BillDate,PartyID+' '+SM.Name as SalesParty ,SM.GSTNo,DiffAmt as TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,(DiffAmt+IGSTAmt+CGSTAmt+SGSTAmt) InvoiceAmt  from JournalVoucherDetails JVD left Join SupplierMaster SM on  JVD.PartyID in (SM.AreaCode+CAST(SM.Accountno as varchar)) Cross Apply (Select  TOP 1 BA.GSTNature from BalanceAmount BA Where JVD.VoucherCode = BA.VoucherCode and JVD.VoucherNo = BA.VoucherNo) BA  Where SM.GroupII='UNAUTHORISED' and BA.GSTNature in ('REGISTERED EXPENSE (B2B)','CR. NOTE RECEIVED AGAINST PURCHASE','DR. NOTE RECEIVED AGAINST PURCHASE') " + strSubQuery.Replace("SR.BillDate", "InvoiceDate").Replace("SR.SalePartyID", "JVD.PartyID") + strJournalVCode.Replace("SR.", "JVD.") + ")SR ";
            }
            else if (strTaxType == "B2C(Large) Invoices - 5A, 5B")
            {
                strQuery = " Select * from (Select 'SALES' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery + strSBillCOde + " UNION ALL "
                         + " Select 'SALESERVICE' as BillType, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo,SR.Date BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ")SR  "; 
            }
            else if (strTaxType == "B2C(Small) Invoices - 7")
            {
                strQuery = " Select * from ( Select 'SALES' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery + strSBillCOde + " UNION ALL "
                         + " Select 'SALESERVICE' as BillType, (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo,SR.Date BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " )SR ";
            }
            else if (strTaxType == "Credit/Debit Notes(Registered) - 9B")
            {
                  //strQuery = "  Select 'SALERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " ";
                if (strSummaryType == "GSTR1")
                {
                    strQuery = "Select 'SALERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + "  UNION ALL "
                             + " Select 'DEBITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, ((CAST(SR.NetAmt as money) - SR.TaxAmount)*-1) TaxableAmt, CAST((GD.IGSTAmt*-1) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*-1) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*-1) as numeric(18, 2)) SGSTAmt,CAST((SR.NetAmt*-1) as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'DEBITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + "  ";
                }
                else
                {
                    strQuery = "  Select 'PURCHASERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " UNION ALL "
                             + " Select 'CREDITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'CREDITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " ";
                }
                strQuery = "Select * from (" + strQuery + ")SR ";
            }
            else if (strTaxType == "Credit/Debit Notes(Unregistered) - 9B")
            {
                if (strSummaryType == "GSTR1")
                {
                    strQuery = "  Select 'SALERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " UNION ALL "
                             + "  Select 'DEBITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'DEBITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED'  and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " ";
                }
                else
                {
                    strQuery = "  Select 'PURCHASERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED'  and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " UNION ALL "
                             + "  Select 'CREDITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'CREDITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " ";
                }

                strQuery = "Select * from (" + strQuery + ")SR ";
            }
            else if (strTaxType == "Credit/Debit Notes(Unregistered-Small)-9B")
            {
                if (strSummaryType == "GSTR1")
                {
                    strQuery = "  Select 'SALERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " UNION ALL "
                             + "  Select 'DEBITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'DEBITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " ";
                }
                else
                {
                    strQuery = "  Select 'PURCHASERETURN' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'PURCHASERETURN' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " UNION ALL "
                             + "  Select 'CREDITNOTE' BillType,(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillNo as _BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) InvoiceAmt from PurchaseReturn SR inner join SupplierMaster SM on SR.PurchasePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) CROSS APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'CREDITNOTE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date").Replace("SR.SalePartyID", "SR.PurchasePartyID") + strPRBillCode + " ";
                }

                strQuery = "Select * from (" + strQuery + ")SR ";
            }
            //if (strTaxType == "Exports Invoices - 6A")
            //{
            //    strQuery = " Select 'Exports Invoices - 6A' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt ";
            //}
            //if (strTaxType == "Tax Liability(Advances received) - 11A(1), 11A(2)")
            //{
            //    strQuery = " Select 'Tax Liability(Advances received) - 11A(1), 11A(2)' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt ";
            //}
            //if (strTaxType == "Adjustment of Advances - 11B(1), 11B(2)")
            //{
            //    strQuery = " Select 'Adjustment of Advances - 11B(1), 11B(2)' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt ";
            //}
            //if (strTaxType == "Nil Rated Invoices - 8A, 8B, 8C, 8D")
            //{
            //    strQuery = " Select 'Nil Rated Invoices - 8A, 8B, 8C, 8D' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt ";
            //}

            if (rdoPartyWise.Checked)
            {
                strQuery = " Select BillType,SalesParty,GSTNo,Count(*) Vch_Count,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(InvoiceAmt) InvoiceAmt from ( "
                         + strQuery + " )_Sales  Group by BillType,SalesParty,GSTNo Order by SalesParty ";
            }
            else
                strQuery += " Order by BillDate,SR._BillNo ";

            DataTable dt = dba.GetDataTable(strQuery);
            BindRecordWithControl(dt);
        }

        private void BindRecordWithControl(DataTable dt)
        {
            try
            {
                double dVch_Count = 0, dTaxableAmt = 0, dIGSTAmt = 0, dCGSTAmt = 0, dSGSTAmt = 0, dTotalTax = 0, dInvAmt = 0, dTIGSTAmt = 0, dTCGSTAmt = 0, dTSGSTAmt = 0;
               
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        
                        dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
                        dIGSTAmt += dTIGSTAmt= dba.ConvertObjectToDouble(row["IGSTAmt"]);
                        dCGSTAmt += dTCGSTAmt= dba.ConvertObjectToDouble(row["cgstAmt"]);
                        dSGSTAmt += dTSGSTAmt= dba.ConvertObjectToDouble(row["sgstAmt"]);
                        
                        dInvAmt += dba.ConvertObjectToDouble(row["InvoiceAmt"]);
                        if (rdoPartyWise.Checked)
                        {
                            dVch_Count += dba.ConvertObjectToDouble(row["Vch_Count"]);
                            dTotalTax += dba.ConvertObjectToDouble(row["TaxAmt"]);

                            dgrdDetails.Rows[_rowIndex].Cells["Particulars"].Value = row["SalesParty"];
                            dgrdDetails.Rows[_rowIndex].Cells["gstNo"].Value = row["GSTNo"];
                            dgrdDetails.Rows[_rowIndex].Cells["voucherCount"].Value = row["Vch_Count"];
                            dgrdDetails.Rows[_rowIndex].Cells["taxableValue"].Value = row["TaxableAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["igstAmt"].Value = row["IGSTAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = row["cgstAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = row["sgstAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["totalTaxAmt"].Value = row["TaxAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["invoiceAmt"].Value = row["InvoiceAmt"];
                        }
                        else
                        {
                            dTotalTax += dTIGSTAmt + dTCGSTAmt + dTSGSTAmt;
                            dgrdDetails.Rows[_rowIndex].Cells["date"].Value = row["BillDate"];
                            dgrdDetails.Rows[_rowIndex].Cells["Particulars"].Value = row["SalesParty"];
                            dgrdDetails.Rows[_rowIndex].Cells["gstNo"].Value = row["GSTNo"];
                            dgrdDetails.Rows[_rowIndex].Cells["voucherCount"].Value = row["BillNo"];
                            dgrdDetails.Rows[_rowIndex].Cells["taxableValue"].Value = row["TaxableAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["igstAmt"].Value = row["IGSTAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = row["cgstAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = row["sgstAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["totalTaxAmt"].Value = (dTIGSTAmt+ dTCGSTAmt+ dTSGSTAmt);
                            dgrdDetails.Rows[_rowIndex].Cells["invoiceAmt"].Value = row["InvoiceAmt"];
                        }
                        dgrdDetails.Rows[_rowIndex].Cells["billType"].Value = row["BillType"];
                        _rowIndex++;
                    }
                }

                if (rdoPartyWise.Checked)
                {
                    lblVchCount.Text = dVch_Count.ToString("N0", MainPage.indianCurancy);
                    dgrdDetails.Columns["Particulars"].Width = 270;
                    dgrdDetails.Columns["gstNo"].Width = 125;
                    dgrdDetails.Columns["date"].Visible = false;
                    dgrdDetails.Columns["voucherCount"].HeaderText = "Vch Count";
                }
                else
                {
                    lblVchCount.Text = "";
                    dgrdDetails.Columns["Particulars"].Width = 185;
                    dgrdDetails.Columns["gstNo"].Width = 115;
                    dgrdDetails.Columns["date"].Visible = true;
                    dgrdDetails.Columns["voucherCount"].HeaderText = "Inv.No";
                }
               
                lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                lblIGSTAmt.Text = dIGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblCGSTAmt.Text = dCGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblSGSTAmt.Text = dSGSTAmt.ToString("N2", MainPage.indianCurancy);
                lblTotalTax.Text = dTotalTax.ToString("N2", MainPage.indianCurancy);
                lblInvoiceAmt.Text = dInvAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }

        private void OpenVoucherDetails()
        {
            try
            {
                if (rdoVoucherWise.Checked)
                {
                    string strVoucherNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["voucherCount"].Value), strBillType = Convert.ToString(dgrdDetails.CurrentRow.Cells["billType"].Value);
                    if (strVoucherNo != "")
                    {
                        string[] strNumber = strVoucherNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            if (strBillType == "SALES")
                            {
                                if (Control.ModifierKeys == Keys.Control)
                                {
                                    dba.ShowSaleBookPrint(strNumber[0], strNumber[1],false, false);
                                }
                                else
                                {
                                    dba.ShowTransactionBook("SALES", strNumber[0], strNumber[1]);
                                    //SaleBook objSale = new SaleBook(strNumber[0], strNumber[1]);
                                    //objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    //objSale.ShowInTaskbar = true;
                                    //objSale.Show();
                                }
                            }
                            else if (strBillType == "PURCHASE")
                            {
                                dba.ShowTransactionBook("PURCHASE", strNumber[0], strNumber[1]);
                                //PurchaseBook objPurchaseBook = new PurchaseBook(strNumber[0], strNumber[1]);
                                //objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                //objPurchaseBook.ShowInTaskbar = true;
                                //objPurchaseBook.Show();
                            }
                            else if (strBillType == "SALERETURN")
                            {
                                dba.ShowTransactionBook("SALE RETURN", strNumber[0], strNumber[1]);
                                //SaleReturn objSaleReturn = new SaleReturn(strNumber[0], strNumber[1]);
                                //objSaleReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                //objSaleReturn.ShowInTaskbar = true;
                                //objSaleReturn.Show();
                            }
                            else
                            {
                                dba.ShowTransactionBook("PURCHASE RETURN", strNumber[0], strNumber[1]);
                                //PurchaseReturn objPurchaseReturn = new PurchaseReturn(strNumber[0], strNumber[1]);
                                //objPurchaseReturn.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                //objPurchaseReturn.ShowInTaskbar = true;
                                //objPurchaseReturn.Show();
                            }
                        }
                    }
                }
                else
                {
                    string strParticular = Convert.ToString(dgrdDetails.CurrentRow.Cells["Particulars"].Value);
                    if (strParticular != "")
                    {
                        GST_VoucherRegister objGST = new SSS.GST_VoucherRegister();
                        if (strFromDate != "" && strToDate != "")
                        {
                            objGST.strFromDate = strFromDate;
                            objGST.strToDate = strToDate;
                        }
                        objGST.strSalesParty = strParticular;
                        objGST.strTaxType = strTaxType;
                        objGST.strMonthName = strMonthName;
                        objGST.strSummaryType = this.strSummaryType;
                        objGST.rdoPartyWise.Enabled = false;
                        objGST.rdoVoucherWise.Checked = true;
                        objGST.strStateName = this.strStateName;
                        objGST.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        //objGST.GetDataTableFromDB();
                        objGST.ShowDialog();
                    }
                }
            }
            catch { }
        }


    }
}
