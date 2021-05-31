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
using System.Reflection;

namespace SSS
{
    public partial class GSTR_1_Summary : Form
    {
        DataBaseAccess dba;
        public GSTR_1_Summary()
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

        private string CreateSubQuery(ref string strSBillCode, ref string strSRBillCode, ref string strSaleServiceVCode,ref string strDNBillCode)
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

            if (txtStateName.Text != "")
            {
                strSBillCode = " and SR.BillCode in (Select SBillCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSRBillCode = " and SR.BillCode in (Select GReturnCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strDNBillCode = " and SR.BillCode in (Select DebitNoteCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
                strSaleServiceVCode = " and SR.BillCode in (Select SaleServiceCode from CompanySetting CS INNER JOIN CompanyDetails CD on CS.CompanyName=CD.Other Where CD.StateName='" + txtStateName.Text + "') ";
            }
            
            return strSubQuery;
        }

        private void GetDataTableFromDB()
        {
            string strQuery = "", strSBillCode="", strSRBillCode="",strDNBillCode="", strSaleServiceVCode="", strSubQuery = CreateSubQuery(ref strSBillCode,ref strSRBillCode, ref strSaleServiceVCode, ref strDNBillCode);
            ClearAllRecord();


            //if (rdoReturnFormat.Checked)
            //{
            strQuery = " Select 'B2B Invoices - 4A, 4B, 4C, 6B, 6C' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery + strSBillCode+ " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt-CAST((SR.RoundOffSign+CAST(ISNULL(SR.RoundOffAmt,0) as varchar)) as money)) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.Date BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII != 'UNAUTHORISED'and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " )_Sales Union ALL "
                     + " Select 'B2BUR Invoices - 4B' as BillType,COUNT(*) VchCount,ISNULL(SUM(TaxableAmt), 0) as TaxableAmt,ISNULL(SUM(IGSTAmt), 0) as IGSTAmt,ISNULL(SUM(CGSTAmt), 0) as CGSTAmt,ISNULL(SUM(SGSTAmt), 0) as SGSTAmt,ISNULL(SUM(IGSTAmt + CGSTAmt + SGSTAmt), 0) TaxAmt,ISNULL(SUM(NetAmt),0) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery + strSBillCode +" UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmt-CAST((SR.RoundOffSign+CAST(ISNULL(SR.RoundOffAmt,0) as varchar)) as money)) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.Date BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ")_Sales Union ALL "
                     + " Select 'B2C(Small) Invoices - 7' as BillType,COUNT(*) VchCount,ISNULL(SUM(TaxableAmt), 0) as TaxableAmt,ISNULL(SUM(IGSTAmt), 0) as IGSTAmt,ISNULL(SUM(CGSTAmt), 0) as CGSTAmt,ISNULL(SUM(SGSTAmt), 0) as SGSTAmt,ISNULL(SUM(IGSTAmt + CGSTAmt + SGSTAmt), 0) TaxAmt,ISNULL(SUM(NetAmt),0) InvoiceAmt from ( "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery + strSBillCode + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.Date as BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmt-CAST((SR.RoundOffSign+CAST(ISNULL(SR.RoundOffAmt,0) as varchar)) as money)) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " UNION ALL "
                     + " Select (SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo, SR.Date BillDate, (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty, SM.GSTNo, (CAST(SR.NetAmt as money) - SR.TaxAmt) TaxableAmt, CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ")_Sales UNION ALL "
                     + " Select 'Credit/Debit Notes(Registered) - 9B' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,PR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,((CAST(PR.NetAmt as money) - PR.TaxAmount)*DebitNoteStatus) TaxableAmt,CAST((GD.IGSTAmt*DebitNoteStatus) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2)) SGSTAmt,(CAST(PR.NetAmt as Money)*DebitNoteStatus) NetAmt from SaleReturn PR inner join SupplierMaster SM on PR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN BillType='DEBITNOTE' then -1 else 1 end)DebitNoteStatus,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = PR.BillCode and GD.BillNo = PR.BillNo Group by BillType,TaxType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate","PR.Date") + strSRBillCode.Replace("SR.", "PR.") + ")SaleReturn UNION ALL "
                     + " Select 'Credit/Debit Notes(Unregistered) - 9B' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,PR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,((CAST(PR.NetAmt as money) - PR.TaxAmount)*DebitNoteStatus) TaxableAmt,CAST((GD.IGSTAmt*DebitNoteStatus) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2)) SGSTAmt,(CAST(PR.NetAmt as Money)*DebitNoteStatus) NetAmt from SaleReturn PR inner join SupplierMaster SM on PR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN BillType='DEBITNOTE' then -1 else 1 end)DebitNoteStatus,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = PR.BillCode and GD.BillNo = PR.BillNo Group by TaxType,BillType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(PR.NetAmt as money) > 250000  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + ")SaleReturn UNION ALL "
                     + " Select 'Credit/Debit Notes(Unregistered-Small)-9B' BillType,COUNT(*) VchCount,SUM(TaxableAmt) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,SUM(IGSTAmt+CGSTAmt+SGSTAmt) TaxAmt,SUM(NetAmt) InvoiceAmt from ( "
                     + " Select (PR.BillCode + ' ' + CAST(PR.BillNo as nvarchar)) BillNo,PR.Date as BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,((CAST(PR.NetAmt as money) - PR.TaxAmount)*DebitNoteStatus) TaxableAmt,CAST((GD.IGSTAmt*DebitNoteStatus) as numeric(18, 2))IGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2))CGSTAmt,CAST((GD.CGSTAmt*DebitNoteStatus) as numeric(18, 2)) SGSTAmt,(CAST(PR.NetAmt as Money)*DebitNoteStatus) NetAmt from SaleReturn PR inner join SupplierMaster SM on PR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select (CASE WHEN BillType='DEBITNOTE' then -1 else 1 end)DebitNoteStatus,(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = PR.BillCode and GD.BillNo = PR.BillNo Group by TaxType,BillType) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(PR.NetAmt as money) <= 250000  " + strSubQuery.Replace("SR.BillDate", "PR.Date") + strSRBillCode.Replace("SR.", "PR.") + ")SaleReturn UNION ALL "
                     + " Select 'Exports Invoices - 6A' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt UNION ALL "
                     + " Select 'Tax Liability(Advances received) - 11A(1), 11A(2)' as BillType,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt UNION ALL "
                     + " Select 'Adjustment of Advances - 11B(1), 11B(2)' as BillTYpe,0 VchCount,0 as TaxableAmt,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,0 TaxAmt,0 InvoiceAmt UNION ALL "
                     + " Select 'Nil Rated Invoices - 8A, 8B, 8C, 8D' BillType,COUNT(*) VchCount,ISNULL(SUM(TaxableAmt),0) as TaxableAmt,ISNULL(SUM(IGSTAmt),0) as IGSTAmt,ISNULL(SUM(CGSTAmt),0) as CGSTAmt,ISNULL(SUM(SGSTAmt),0) as SGSTAmt,ISNULL(SUM(IGSTAmt+CGSTAmt+SGSTAmt),0) TaxAmt,ISNULL(SUM(NetAmt),0) InvoiceAmt from ( "
                     + " Select(SR.BillCode + ' ' + CAST(SR.BillNo as nvarchar)) BillNo,SR.BillDate,(SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + Name) SalesParty,SM.GSTNo,(CAST(SR.NetAmt as money) - SR.TaxAmount) TaxableAmt,CAST(GD.IGSTAmt as numeric(18, 2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18, 2)) SGSTAmt,CAST(SR.NetAmt as Money) NetAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select(CASE WHEN GD.TaxType = 'LOCAL' then ROUND((SUM(GD.TaxAmount) / 2), 2) else 0 end) CGSTAmt, (CASE WHEN GD.TaxType = 'INTERSTATE' then ROUND(SUM(GD.TaxAmount), 2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType) GD Where SR.TaxAmount = 0 " + strSubQuery + strSBillCode+" )_Sales ";

            strQuery += ";WITH Missing (missnum, maxid) AS (Select  MIN(missnum) AS missnum, MAX(maxnum) maxnum from( Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "')  UNION ALL Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "')  )SaleRecord UNION ALL   SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid )     Select BillType,BCode,MinBillNo,MaxBillNo,MissNum,((MaxBillNo-MinBillNo)+1)TotalBill,((MaxBillNo-MinBillNo)-MissNum+1)NetBill from(SELECT 'SALES' as BillType,(Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') BCode,(Select MIN(MinBillNo)MinBillNo from (Select MIN(BillNo) MinBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " UNION ALL  Select MIN(BillNo) MinBillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_Sales) MinBillNo,COUNT(*) MissNum,( Select MAX(MaxBillNo)MaxBillNo from (Select MAX(BillNo) MaxBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + "UNION ALL Select MAX(BillNo) MaxBillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_Sales) MaxBillNo FROM Missing LEFT OUTER JOIN (Select BillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " UNION ALL Select BillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_SR on _SR.BillNo = Missing.missnum  WHERE _SR.BillNo is NULL) _Miss OPTION(MAXRECURSION 0);  "
                    + " ;WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " and BillCode in (Select TOP 1 SaleServiceCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                    + " Select BillType,BCode,MinBillNo,MaxBillNo,MissNum,((MaxBillNo-MinBillNo)+1)TotalBill,((MaxBillNo-MinBillNo)-MissNum+1)NetBill from(SELECT 'SALESERVICE' as BillType,(Select TOP 1 SaleServiceCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ) BCode,(Select MIN(BillNo) MinBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SaleServiceBook SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); "
                    + " ;WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleReturn SR Where EntryType!='DEBITNOTE' and BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + "and BillCode in (Select TOP 1 GReturnCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                    + " Select BillType, BCode, MinBillNo, MaxBillNo, MissNum,((MaxBillNo - MinBillNo) + 1)TotalBill, ((MaxBillNo - MinBillNo) - MissNum + 1)NetBill from(SELECT 'SALERETURN' as BillType, (Select TOP 1 GReturnCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' ) BCode, (Select MIN(BillNo) MinBillNo from SaleReturn SR Where EntryType != 'DEBITNOTE' and BillNo >0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleReturn SR Where EntryType!= 'DEBITNOTE' and BillNo> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SaleReturn SR on SR.BillNo = Missing.missnum " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " WHERE EntryType!= 'DEBITNOTE' and SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); "
                    + ";WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo > 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " and BillCode in (Select TOP 1 DebitNoteCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                    + " Select BillType, BCode, MinBillNo, MaxBillNo, MissNum,((MaxBillNo - MinBillNo) + 1)TotalBill, ((MaxBillNo - MinBillNo) - MissNum + 1)NetBill from(SELECT 'DEBITNOTE' as BillType, (Select TOP 1 DebitNoteCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' ) BCode, (Select MIN(BillNo) MinBillNo from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo >0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + " ) MaxBillNo FROM Missing LEFT OUTER JOIN SaleReturn SR on SR.BillNo = Missing.missnum " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + " WHERE EntryType = 'DEBITNOTE' and SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); ";



            strQuery += " ;WITH Missing (missnum, maxid) AS (Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid )  "
                     + " SELECT 'SALES' as BillType,(Select MIN(BillNo) MinBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SalesRecord SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL  OPTION(MAXRECURSION 0); "
                     + " ; WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " and BillCode in (Select TOP 1 SaleServiceCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                     + " SELECT 'SALESERVICE' as BillType,(Select MIN(BillNo) MinBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SaleServiceBook SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL  OPTION(MAXRECURSION 0); ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 0)
            {
                BindRecordWithControl(ds.Tables[0]);
                BindDocumentSummary(ds);
            }
        }

        private void BindSummaryWithGrid(DataTable dt)
        {
            if(dt.Rows.Count>0)
            {
                DataRow row = dt.Rows[0];
                double dMinBillNo = 0, dMaxBillNo = 0, dMissingBillNo = 0;
                dMinBillNo = dba.ConvertObjectToDouble(row["MinBillNo"]);
                dMaxBillNo = dba.ConvertObjectToDouble(row["MaxBillNo"]);
                dMissingBillNo = dba.ConvertObjectToDouble(row["MissNum"]);

                int _rowIndex = dgrdDocSummary.Rows.Count;

                dgrdDocSummary.Rows.Add(1);

                dgrdDocSummary.Rows[_rowIndex].Cells["natureofDoc"].Value = "Invoices for outward supply (" + row["BillType"] + ")";
                dgrdDocSummary.Rows[_rowIndex].Cells["fromVchNo"].Value = dMinBillNo;
                dgrdDocSummary.Rows[_rowIndex].Cells["toVchNo"].Value = dMaxBillNo;
                dgrdDocSummary.Rows[_rowIndex].Cells["noOfVch"].Value = (dMaxBillNo - dMinBillNo) + 1;
                dgrdDocSummary.Rows[_rowIndex].Cells["cancelVch"].Value = dMissingBillNo;
                dgrdDocSummary.Rows[_rowIndex].Cells["totalNoOfVch"].Value = (dMaxBillNo - dMinBillNo - dMissingBillNo) + 1;
            }
        }

        private void BindDocumentSummary(DataSet ds)
        {
            try
            {
                dgrdDocSummary.Rows.Clear();
                DataTable dt = ds.Tables[1];
                BindSummaryWithGrid(dt);
                dt = ds.Tables[2];
                BindSummaryWithGrid(dt);
                dt = ds.Tables[3];
                BindSummaryWithGrid(dt);
                dt = ds.Tables[4];
                BindSummaryWithGrid(dt);
            }
            catch { }
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

                        if (Convert.ToString(row["BillType"]) == "Credit/Debit Notes(Unregistered-Small)-9B")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
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
                objGST.strStateName = txtStateName.Text;
                objGST.strSummaryType = "GSTR1";
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

        private void GetDataForExport()
        {
            DataSet ds = GetExportedDataFromDB();
            if (ds.Tables.Count > 0)
            {
                CreateExcelSheet(ds);
            }
        }

        private DataSet GetExportedDataFromDB()
        {
            string strQuery = "", strSBillCode = "", strSRBillCode = "",strDNBillCode="", strSaleServiceVCode = "", strSubQuery = CreateSubQuery(ref strSBillCode, ref strSRBillCode, ref strSaleServiceVCode, ref strDNBillCode);

            strQuery += " Select GSTNo,ReceiverName,BillNo,BillDate,InvoiceAmt,PlaceOfSupply,ReverseCharge,ApplicableTaxRate,InvoiceType,EcommGSTNo,TaxRate,TaxableAmt,CessAmount from (Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode  + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmount,SR.BillNo as _BillNo from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery + strSBillCode + "  Union ALL "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode  + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),4) TaxableAmt,'' as CessAmount,SR.BillNo as _BillNo from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + "  Union ALL "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode  + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2)  TaxableAmt,'' as CessAmount,SR.BillNo as _BillNo from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union ALL "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR, SR.BillDate, 6), ' ', '-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,0 as TaxRate,CAST(SE.Tax as Money) TaxableAmt,'' as CessAmount,SR.BillNo as _BillNo from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName  Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' and CAST(SE.Tax as Money) != 0 " + strSubQuery + strSBillCode + " UNION ALL "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR, SR.BillDate, 6), ' ', '-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,0 as TaxRate, CAST((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)) as numeric(18,2)) TaxableAmt,'' as CessAmount,SR.BillNo as _BillNo from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo=(GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) inner join Items _IM on GRD.ItemName=_IM.ItemName inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName  Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' and _IGM.TaxCategoryName='0%' " + strSubQuery + strSBillCode + " ) SR Order by SR._BillNo "
                     + " Select * from (Select(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end), 2) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery + strSBillCode + " Union ALL "
                     + " Select (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate,(CAST(SR.NetAmt as money) - SR.TaxAmt-CAST((SR.RoundOffSign+CAST(ISNULL(SR.RoundOffAmt,0) as varchar)) as money)) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union ALL "
                     + " Select (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end), 2) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + "  Union ALL "
                     + " Select (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR, SR.BillDate, 6), ' ', '-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,0 as TaxRate,CAST(SE.Tax as Money) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName  Where SM.GroupII = 'UNAUTHORISED' and CAST(SE.Tax as Money) != 0 " + strSubQuery + strSBillCode + " UNION ALL "
                     + " Select (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate, 6), ' ', '-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,0 as TaxRate, CAST((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100.00)) as numeric(18,4)) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo=(GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) inner join Items _IM on GRD.ItemName=_IM.ItemName inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName  Where SM.GroupII = 'UNAUTHORISED' and _IGM.TaxCategoryName='0%'  " + strSubQuery + strSBillCode + " ) SR Order by SR.BillNo "
                     + " Select * from (Select 'OE' EcommType, (STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate, ROUND(SUM((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)), 4) TaxableAmt, '' as CessAmount, '' EcommGSTNo from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery + strSBillCode + "  Group by GD.TaxRate, STM.StateCode, STM.StateName UNION ALL "
                     + " Select 'OE' EcommType, (STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate, ROUND(SUM((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)), 4) TaxableAmt, '' as CessAmount, '' EcommGSTNo from SalesBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Group by GD.TaxRate, STM.StateCode, STM.StateName UNION ALL "
                     + " Select 'OE' EcommType, (STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'' as ApplicableTaxRate,GD.TaxRate, ROUND(SUM((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)), 4) TaxableAmt, '' as CessAmount, '' EcommGSTNo from SaleServiceBook SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALESERVICE' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + "  Group by GD.TaxRate, STM.StateCode, STM.StateName )_Sale Order by TaxRate "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,(CASE WHEN BillType='DEBITNOTE' then 'D' else 'C' end) DocType,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'Regular' SupplyType,CAST(SR.NetAmt as Money) NetAmt,'' as ApplicableTaxRate,GD.TaxRate, ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmt from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar))  left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select BillType,SUM(GD.TaxRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate,BillType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!=''  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + "  Order by SR.Date "
                     //+ " Select SM.GSTNo,SM.Name as ReceiverName,(SR.SaleBillCode + CAST(SR.SaleBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SaleBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,(CASE WHEN BillType='DEBITNOTE' then 'D' else 'C' end) DocType,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,CAST(SR.NetAmt as Money) NetAmt,'' as ApplicableTaxRate,GD.TaxRate, ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmt,'N' as PreGST from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar))  left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select BillType,SUM(GD.TaxRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate,BillType) GD Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + "  Order by SR.Date "
                     + " Select 'B2CL' URType,(SR.SaleBillCode + CAST(SR.SaleBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SaleBillDate,6),' ','-') as SaleBillDate, (SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,'C' DocType,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,CAST(SR.NetAmt as Money) NetAmt,'' as ApplicableTaxRate,GD.TaxRate, ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmt,'N' as PreGST from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar))  left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TaxRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Where SM.GroupII = 'UNAUTHORISED' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " and SR.NetAmt>250000  Order by SR.Date "
                     + " Select (CASE WHEN SLTM.Region = 'INTERSTATE' and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' then 'Inter-State supplies to registered persons' WHEN SLTM.Region = 'LOCAL' and SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' then 'Intra-State supplies to registered persons' WHEN SLTM.Region = 'INTERSTATE' and SM.GroupII = 'UNAUTHORISED' then 'Inter-State supplies to unregistered persons' WHEN SLTM.Region = 'LOCAL' and SM.GroupII = 'UNAUTHORISED' then 'Intra-State supplies to unregistered persons' end) SaleDescription, (CASE WHEN SLTM.TaxationType = 'ZERORATED' then(SUM(CAST(SR.NetAmt as money))) else 0 end) NilRatedAmt,(CASE WHEN SLTM.TaxationType = 'EXEMPT' then(SUM(CAST(SR.NetAmt as money))) else 0 end) ExemptAmt,(CASE WHEN SLTM.TaxationType = 'NONGST' then(SUM(CAST(SR.NetAmt as money))) else 0 end) NonGSTAmt from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) Outer Apply (Select TOP 1 StateName from CompanyDetails) CD Outer Apply(Select TOP 1 SLTM.Region, SLTM.TaxationType from SaleTypeMaster SLTM Where SLTM.SaleType = 'SALES' and SLTM.TaxName = SR.SalesType) SLTM Where SR.TaxAmount = 0 " + strSubQuery + strSBillCode + " Group by SLTM.Region,SM.GroupII,SLTM.TaxationType "
                     + " Select HSNCode,ItemName,UnitName,Qty,(TaxableAmt + IGSTAmt + CGSTAmt + SGSTAmt) TotalValue,TaxableAmt,IGSTAmt,CGSTAmt,SGSTAmt,'' CessAmt from (Select HSNCode, ItemName, UnitName, SUM(Quantity)Qty, SUM(TaxableAmt) TaxableAmt, SUM((CASE WHEN Region = 'INTERSTATE' then Amount else 0 end)) as IGSTAmt,SUM((CASE WHEN Region = 'LOCAL' and Amount != 0 then Amount / 2 else 0 end)) as CGSTAmt,SUM((CASE WHEN Region = 'LOCAL' and Amount != 0 then Amount / 2 else 0 end)) as SGSTAmt from(Select BillNo,HSNCode, ItemName, UnitName, SUM(Qty)Quantity, TaxRate,SUM(CASE WHEN TaxType = 1 and TaxRate>0 then ((Amount * 100) / (100+TaxRate)) else Amount end) TaxableAmt, SUM((((CASE WHEN TaxType = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end) * TaxRate) / 100.00)) Amount,Region from (Select BillNo,HSNCode, ItemName, UnitName, SUM(Amount)Amount, SUM(Quantity) Qty, TaxRate, TaxType, Region from( "
                     + " Select SR.BillNo,(GM.HSNCode) as HSNCode, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(GRD.ItemName,':',''),',',''),'/',''),'-',''),'.','')ItemName, GM.UnitName, GRD.Quantity, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100.00) / (100.00 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate,_IM.ItemName,UM.FormalName as UnitName from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join UnitMaster UM on UM.UnitName=_IM.UnitName left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != ''  and GRD.Amount > 0 " + strSubQuery + strSBillCode + "  Union All "
                     + " Select SR.BillNo,(GM.HSNCode) as HSNCode,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(SE.ItemName,':',''),',',''),'/',''),'-',''),'.','')ItemName, GM.UnitName, SE.Qty as Quantity,SE.Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SR.SpecialDscPer) / 100.00) else 1.00 end))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.MRP * 100) / (100 + TaxRate)) else SE.MRP end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SE.SDisPer-SR.SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate,_IM.ItemName,UM.FormalName as UnitName from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join UnitMaster UM on UM.UnitName=_IM.UnitName left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != ''  and SE.MRP > 0  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + "  Union All "
                     + " Select SR.BillNo,(GM.HSNCode) as HSNCode, REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(SE.ItemName,':',''),',',''),'/',''),'-',''),'.','')ItemName, GM.UnitName, 0 Quantity, (SE.Amount)Amount, GM.TaxRate, SMN.TaxIncluded as TaxType, SMN.Region from SaleServiceBook SR inner join SaleServiceDetails SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end))))) > _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SE.Amount * 100) / (100 + TaxRate)) else SE.Amount end)))))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate,_IM.ItemName,UM.FormalName as UnitName from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join UnitMaster UM on UM.UnitName=_IM.UnitName left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SE.ItemName = _IM.ItemName ) as GM  Where SR.BillCode != '' and SE.Amount > 0  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + "  UNION ALL "
                     + " Select BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName, 0 as Quantity, ROUND((((((Amount + (Amount * DisStatus) / 100) * GM.TaxRate) / 100) * CS.TaxDhara) / 100) * (CASE WHen TaxType = 1 then((100 + TaxPer) / 100) else 1 end),2)Amount, TaxPer TaxRate, TaxType, Region from(Select SR.BillNo,ROUND((CASE WHEN SMN.TaxIncluded = 1  then((GRD.Amount * 100) / (100 + GM.TaxRate)) else GRD.Amount end), 2) Amount, (SE.DiscountStatus + SE.Discount) DisStatus, SMN.TaxIncluded as TaxType, GRD.ItemName,(CASE WHEN GRD.Quantity =0 then 1 else GRD.Quantity end) as Qty,SR.TaxPer,SMN.Region,SR.BillCode,SR.BillDate from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((GRD.Rate * 100) / (100 + TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where SR.BillCode != '' " + strSubQuery + strSBillCode + " and SR.ServiceAmount!=0 and GRD.Amount > 0 )_SAles OUTER APPLY(Select CAST(TaxDhara as bigint)+(CASE WHEN _Sales.BillCode like('%CCK%') and BillDate<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere SBillCode=_Sales.BillCode) CS Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end)) / _SAles.Qty)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN _SAles.TaxType = 1 then((_SAles.Amount * 100) / (100 + TaxRate)) else _SAles.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(_SAles.DisStatus)) / 100.00) else 1.00 end))/ _SAles.Qty)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _SAles.ItemName = _IM.ItemName ) as GM  Union All "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) + ((GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) * CS.FreightDhara / 100)* (CASE WHen SMN.TaxIncluded = 1 then((100 + TaxPer) / 100) else 1 end)), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer Apply (Select CAST(FreightDhara as bigint)+(CASE WHEN SR.BillCode like('%CCK%') and SR.BillDate<'09/01/2019' then 1 else 0 end) FreightDhara from CompanySetting WHere SBillCode=SR.BillCode) CS Where SR.BillCode != '' and(GRD.PackingAmt + GRD.FreightAmt + GRD.TaxAmt) > 0 " + strSubQuery + strSBillCode + " Union All "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND((SE.OCharges -SE.Disc) * (CASE WHen SMN.TaxIncluded = 1 then((100 + TaxPer) / 100) else 1 end), 2) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesBook SR inner join SalesBookSecondary SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' and (SE.OCharges -SE.Disc)!= 0  " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " UNION ALL "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND(GRD.TaxAmt, 2) Amount,0 as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) Outer Apply (Select CAST(TaxDhara as bigint)+(CASE WHEN SR.BillCode like('%CCK%') and SR.BillDate<'09/01/2019' then 1 else 0 end) TaxDhara from CompanySetting WHere SBillCode=SR.BillCode) CS Where SR.BillCode != ''  and GRD.TaxAmt > 0 " + strSubQuery + strSBillCode + " Union All "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND((CAST(SR.Postage as Money) + CAST(OtherPacking as Money) + CAST(OtherPer as Money) + CAST(Others as Money) + CAST(ISNULL(GreenTaxAmt, 0) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + TaxPer) / 100) else 1 end) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesRecord SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' " + strSubQuery + strSBillCode + " UNION ALL "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND((SR.PackingAmt + SR.PostageAmt +(CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)) + (CAST((Other+CAST(DisAmt as varchar)) as Money)) + CAST(ISNULL(SR.GreenTax, 0) as money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + TaxPer) / 100) else 1 end) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SalesBook SR  left join SaleTypeMaster SMN On SR.SalesType = SMN.TaxName  and SMN.SaleType = 'SALES' Where SR.BillCode != '' " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " UNION ALL "
                     + " Select SR.BillNo,ISNULL((Select Top 1 SACCode from CompanyDetails Where SACCode!=''),'') as HSNCode,'Service Charge' ItemName,'OTH-OTHERS' as UnitName,0 as Quantity,ROUND((CAST((OtherSign+CAST(OtherAmt as varchar)) as Money)), 2) * (CASE WHen SMN.TaxIncluded = 1 then((100 + TaxPer) / 100) else 1 end) Amount,TaxPer as TaxRate,SMN.TaxIncluded as TaxType,SMN.Region from SaleServiceBook SR  left join SaleTypeMaster SMN On SR.SaleType = SMN.TaxName  and SMN.SaleType = 'SALES' Where OtherAmt!=0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " )_Sales Group by BillNo,HSNCode, ItemName, UnitName, TaxRate, TaxType, Region )_Sales Group by BillNo,HSNCode, ItemName, UnitName, Region, TaxRate )_Sales Group by HSNCode, ItemName, UnitName)Sales Order by ItemName, UnitName "
                     //+ " SELECT 'Invoice for outword supply (SALES)' BillType, MIN(BillNo) MINBillNo,MAX(BIllNo) MAXBillNo, (MAX(BillNo)-MIN(BIllNo)+1) NoOfBIll FROM SalesRecord SR Where SR.BIllNo>0 " + strSubQuery + strSBillCode + " "
                     //+" ;WITH Missing (missnum, maxid) AS ( SELECT 1 AS missnum, (Select max(BillNo) from SalesRecord SR  Where SR.BIllNo>0 " + strSubQuery + strSBillCode + ")   UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) SELECT 'SALES' as BillType,COUNT(Missnum) Missnum FROM Missing LEFT OUTER JOIN SalesRecord SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL OPTION (MAXRECURSION 0); "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,(SR.BillCode + CAST(SR.BillNo as nvarchar)) OBillNo,AB.OBillDate,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,(STM.StateCode+'-'+STM.StateName) PlaceOfSupply,'N' as ReverseCharge,'' as ApplicableTaxRate,'Regular' InvoiceType,'' EcommGSTNo,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmount from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State=STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate,SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Cross APPLY (Select TOP 1 REPLACE(CONVERT(VARCHAR,AD.ODate,6),' ','-') as OBillDate,AD.Date from AmendmentDetails AD Where AD.BillType='SALES' and OBillCode=SR.BillCode and OBillNo=SR.BillNo) AB Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "AB.Date") + strSBillCode
                     + " Select (SR.BillCode + CAST(SR.BillNo as nvarchar)) OBillNo,AB.OBillDate,(STM.StateCode + '-' + STM.StateName) PlaceOfSupply,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.BillDate,6),' ','-') as BillDate,CAST(SR.NetAmt as Money) InvoiceAmt,'' as ApplicableTaxRate,GD.TaxRate,ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end), 2) TaxableAmt,'' as CessAmount,'' EcommGSTNo from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Cross APPLY (Select TOP 1 REPLACE(CONVERT(VARCHAR,AD.ODate,6),' ','-') as OBillDate,AD.Date from AmendmentDetails AD Where BillType='SALES' and OBillCode=SR.BillCode and OBillNo=SR.BillNo " + strSubQuery.Replace("SR.BillDate", "AD.Date") + " ) AB Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) > 250000 " + strSubQuery.Replace("SR.BillDate", "AB.Date") + strSBillCode + " Order by SR.BillNo "
                     + " Select (CASE WHEN (DATEPART(mm,AB.OBillDate))<4 then CAST(DATEPART(YY,AB.OBillDate)-1 as varchar)+'-'+SUBSTRING(CAST(DATEPART(YY,AB.OBillDate) as varchar),3,2) else CAST(DATEPART(YY,AB.OBillDate) as varchar)+'-'+SUBSTRING(CAST(DATEPART(YY,AB.OBillDate) as varchar),3,2) end) FYear, UPPER(DATENAME(MM,AB.OBillDate))_MonthName, (STM.StateCode + '-' + STM.StateName) OPlaceOfSupply, (STM.StateCode + '-' + STM.StateName) PlaceOfSupply,'OE' EcommType,'' as ApplicableTaxRate,GD.TaxRate, ROUND(SUM((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end)), 2) TaxableAmt, '' as CessAmount, '' EcommGSTNo from SalesRecord SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) left join StateMaster STM on SM.State = STM.StateName OUTER APPLY(Select SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate) GD Cross APPLY (Select TOP 1 REPLACE(CONVERT(VARCHAR,AD.ODate,6),' ','-') as OBillDate,AD.Date from AmendmentDetails AD Where BillType='SALES' and OBillCode=SR.BillCode and OBillNo=SR.BillNo) AB Where SM.GroupII = 'UNAUTHORISED' and CAST(SR.NetAmt as money) <= 250000 " + strSubQuery.Replace("SR.BillDate", "AB.Date") + strSBillCode + "  Group by GD.TaxRate,STM.StateCode,STM.StateName,(CASE WHEN (DATEPART(mm,AB.OBillDate))<4 then CAST(DATEPART(YY,AB.OBillDate)-1 as varchar)+'-'+SUBSTRING(CAST(DATEPART(YY,AB.OBillDate) as varchar),3,2) else CAST(DATEPART(YY,AB.OBillDate) as varchar)+'-'+SUBSTRING(CAST(DATEPART(YY,AB.OBillDate) as varchar),3,2) end), UPPER(DATENAME(MM,AB.OBillDate)) Order by GD.TaxRate "
                     + " Select SM.GSTNo,SM.Name as ReceiverName,AB.ORBillNo,AB.ORDate,(SR.SaleBillCode + CAST(SR.SaleBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,ISNULL((Select TOP 1 SRD.BillDate from SalesRecord SRD WHere SRD.BillCode = SR.SaleBillCode and SRD.BillNo = SR.SaleBillNo), ''),6),' ','-') as SaleBillDate,(SR.BillCode + CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,(CASE WHEN BillType='DEBITNOTE' then 'D' else 'C' end) DocType,(CASE WHEN GD.TaxType='LOCAL' then 'Intra State' else 'Inter State' end)PlaceOfSupply,CAST(SR.NetAmt as Money) NetAmt,'' as ApplicableTaxRate,GD.TaxRate, ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmt,'N' as PreGST from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar))   OUTER APPLY(Select BillType,SUM(GD.TaxRate) TaxRate,SUM(TaxAmount) TaxAmt,GD.TaxType from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by TaxType,TaxRate,BillType) GD CROSS APPLY (Select TOP 1 (ORBillCode+CAST(ORBillNo as nvarchar))ORBillNo, REPLACE(CONVERT(VARCHAR,AD.ORDate,6),' ','-') as ORDate,AD.Date from AmendmentDetails AD Where BillType in ('SALERETURN','DEBITNOTE') and OBillCode=SR.BillCode and OBillNo=SR.BillNo) AB Where SM.GroupII != 'UNAUTHORISED' and SM.GroupII!='' " + strSubQuery.Replace("SR.BillDate", "AB.Date") + strSRBillCode + "  Order by SR.Date "
                     + " Select 'B2CL' URType,AB.ORBillNo,AB.ORDate,(SR.SaleBillCode + CAST(SR.SaleBillNo as nvarchar)) SaleBillNo,REPLACE(CONVERT(VARCHAR,SR.SaleBillDate,6),' ','-') as SaleBillDate, (SR.BillCode+ CAST(SR.BillNo as nvarchar)) BillNo,REPLACE(CONVERT(VARCHAR,SR.Date,6),' ','-') as BillDate,(CASE WHEN BillType='DEBITNOTE' then 'D' else 'C' end) DocType,(CASE WHEN GD.TaxType='LOCAL' then 'Intra State' else 'Inter State' end)PlaceOfSupply, CAST(SR.NetAmt as Money) NetAmt,'' as ApplicableTaxRate,GD.TaxRate, ROUND((CASE WHEN GD.TaxRate!=0 then (GD.TaxAmt*100)/GD.TaxRate else GD.TaxAmt end),2) TaxableAmt,'' as CessAmt,'N' as PreGST from SaleReturn SR inner join SupplierMaster SM on SR.SalePartyID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY(Select BillType,SUM(GD.TaxRate) TaxRate,SUM(TaxAmount) TaxAmt,TaxType from GSTDetails GD WHere BillType in ('SALERETURN','DEBITNOTE') and GD.BillCode = SR.BillCode and GD.BillNo = SR.BillNo Group by BillType,TaxType,TaxRate) GD CROSS APPLY (Select TOP 1 (ORBillCode+CAST(ORBillNo as nvarchar))ORBillNo, REPLACE(CONVERT(VARCHAR,AD.ORDate,6),' ','-') as ORDate,AD.Date from AmendmentDetails AD Where BillType in ('SALERETURN','DEBITNOTE') and OBillCode=SR.BillCode and OBillNo=SR.BillNo)AB Where SM.GroupII = 'UNAUTHORISED'  and SR.NetAmt>250000 " + strSubQuery.Replace("SR.BillDate", "AB.Date") + strSRBillCode + "  Order by SR.Date "
                     + " ;WITH Missing (missnum, maxid) AS (Select  MIN(missnum) AS missnum, MAX(maxnum) maxnum from( Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "')  UNION ALL Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "')  )SaleRecord UNION ALL   SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid )     Select BillType,BCode,MinBillNo,MaxBillNo,MissNum,((MaxBillNo-MinBillNo)+1)TotalBill,((MaxBillNo-MinBillNo)-MissNum+1)NetBill from(SELECT 'SALES' as BillType,(Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') BCode,(Select MIN(MinBillNo)MinBillNo from (Select MIN(BillNo) MinBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " UNION ALL  Select MIN(BillNo) MinBillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_Sales) MinBillNo,COUNT(*) MissNum,( Select MAX(MaxBillNo)MaxBillNo from (Select MAX(BillNo) MaxBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + "UNION ALL Select MAX(BillNo) MaxBillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_Sales) MaxBillNo FROM Missing LEFT OUTER JOIN (Select BillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " UNION ALL Select BillNo from SalesBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSBillCode + ")_SR on _SR.BillNo = Missing.missnum  WHERE _SR.BillNo is NULL) _Miss OPTION(MAXRECURSION 0);  "
                     //+ " ;WITH Missing (missnum, maxid) AS (Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + " and BillCode in (Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid )  "
                     // + " Select BillType,BCode,MinBillNo,MaxBillNo,MissNum,((MaxBillNo-MinBillNo)+1)TotalBill,((MaxBillNo-MinBillNo)-MissNum+1)NetBill from(SELECT 'SALES' as BillType,(Select TOP 1 SBillCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ) BCode,(Select MIN(BillNo) MinBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SalesRecord SR Where BillNo>0 " + strSubQuery + strSBillCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SalesRecord SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL)_Miss  OPTION(MAXRECURSION 0); "
                     + " ;WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + " and BillCode in (Select TOP 1 SaleServiceCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                     + " Select BillType,BCode,MinBillNo,MaxBillNo,MissNum,((MaxBillNo-MinBillNo)+1)TotalBill,((MaxBillNo-MinBillNo)-MissNum+1)NetBill from(SELECT 'SALESERVICE' as BillType,(Select TOP 1 SaleServiceCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ) BCode,(Select MIN(BillNo) MinBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleServiceBook SR Where BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSaleServiceVCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SaleServiceBook SR on SR.BillNo = Missing.missnum WHERE SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); "
                     + " ;WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleReturn SR Where EntryType!='DEBITNOTE' and BillNo>0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + "and BillCode in (Select TOP 1 GReturnCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                     + " Select BillType, BCode, MinBillNo, MaxBillNo, MissNum,((MaxBillNo - MinBillNo) + 1)TotalBill, ((MaxBillNo - MinBillNo) - MissNum + 1)NetBill from(SELECT 'SALERETURN' as BillType, (Select TOP 1 GReturnCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' ) BCode, (Select MIN(BillNo) MinBillNo from SaleReturn SR Where EntryType != 'DEBITNOTE' and BillNo >0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleReturn SR Where EntryType!= 'DEBITNOTE' and BillNo> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + ") MaxBillNo FROM Missing LEFT OUTER JOIN SaleReturn SR on SR.BillNo = Missing.missnum " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strSRBillCode + " WHERE EntryType!= 'DEBITNOTE' and SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); "
                     + ";WITH Missing (missnum, maxid) AS(Select MIN(BillNo) AS missnum, MAX(BillNo) maxnum from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo > 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + " and BillCode in (Select TOP 1 DebitNoteCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' )  UNION ALL  SELECT missnum + 1, maxid FROM Missing WHERE missnum < maxid ) "
                     + " Select BillType, BCode, MinBillNo, MaxBillNo, MissNum,((MaxBillNo - MinBillNo) + 1)TotalBill, ((MaxBillNo - MinBillNo) - MissNum + 1)NetBill from(SELECT 'DEBITNOTE' as BillType, (Select TOP 1 DebitNoteCode from CompanySetting Where CompanyName = '" + MainPage.strCompanyName + "' ) BCode, (Select MIN(BillNo) MinBillNo from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo >0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + ") MinBillNo,COUNT(*) MissNum,(Select MAX(BillNo) MaxBillNo from SaleReturn SR Where EntryType = 'DEBITNOTE' and BillNo> 0 " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + " ) MaxBillNo FROM Missing LEFT OUTER JOIN SaleReturn SR on SR.BillNo = Missing.missnum " + strSubQuery.Replace("SR.BillDate", "SR.Date") + strDNBillCode + " WHERE EntryType = 'DEBITNOTE' and SR.BillNo is NULL )_Miss OPTION(MAXRECURSION 0); ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            return ds;
        }

        //private void WriteInExistingFile(DataSet ds)
        //{
        //    NewExcel.Application myExcelApplication;
        //    NewExcel.Workbook myExcelWorkbook;
        //    NewExcel.Worksheet myExcelWorkSheet;
        //    myExcelApplication = null;
        //    try
        //    {  
        //        myExcelApplication = new NewExcel.Application(); // create Excell App
        //        myExcelApplication.DisplayAlerts = false; // turn off alerts

        //        string excelFilePath = MainPage.strServerPath + "\\Excel_File\\GSTR1_Template.xlsx";

        //        myExcelWorkbook = (NewExcel.Workbook)(myExcelApplication.Workbooks._Open(excelFilePath, System.Reflection.Missing.Value,
        //           System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //           System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //           System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //           System.Reflection.Missing.Value, System.Reflection.Missing.Value)); // open the existing excel file

        //        int numberOfWorkbooks = myExcelApplication.Workbooks.Count; // get number of workbooks (optional)

        //        myExcelWorkSheet = (NewExcel.Worksheet)myExcelWorkbook.Worksheets[2];


        //        DataTable dt = ds.Tables[0];
        //        int i = 0;
        //        for (i = 0; i < 10; i++)
        //        {
        //            for (int j = 1; j <= dt.Columns.Count; j++)
        //            {
        //                myExcelWorkSheet.Cells[i + 5, j] = dt.Rows[i][j - 1];
        //            }
        //        }


        //        //for (i = 0; i < Percentage.Count; i++)
        //        //{
        //        //    oSheet.Cells[i + 2, 2] = Percentage[i];
        //        //}

        //        myExcelWorkbook.SaveAs(excelFilePath, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //                                      System.Reflection.Missing.Value, System.Reflection.Missing.Value, NewExcel.XlSaveAsAccessMode.xlNoChange,
        //                                      System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
        //                                      System.Reflection.Missing.Value, System.Reflection.Missing.Value); // Save data in excel


        //        myExcelWorkbook.Close(true, excelFilePath, System.Reflection.Missing.Value); // close the worksh

        //    }
        //    catch
        //    { }
        //    finally
        //    {
        //        if (myExcelApplication != null)
        //        {
        //            myExcelApplication.Quit(); // close the excel application
        //        }
        //    }
        //}


        private string CreateExcelSheet(DataSet ds)
        {
            string[] strSheet = { "b2b", "b2ba", "b2cl","b2cla", "b2cs","b2csa", "cdnr","cdnra", "cdnur","cdnura", "exp", "expa", "at", "ata", "atadj", "atadja", "exemp", "hsn", "docs" };
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

        private void SetColumnName(ref NewExcel.Worksheet ExcelWorkSheet, string strSheetName, DataSet ds)
        {

            if (strSheetName == "b2b")
            {
                var range = ExcelWorkSheet.get_Range("E3", "E10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("K3", "M10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "GSTIN/UIN of Recipient", "Receiver Name", "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Reverse Charge","Applicable % of Tax Rate", "Invoice Type", "E-Commerce GSTIN", "Rate", "Taxable Value", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2B(4)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[0]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[0].Rows.Count);
            }
            else if (strSheetName == "b2ba")
            {
                var range = ExcelWorkSheet.get_Range("G3", "G10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("M3", "O10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "GSTIN/UIN of Recipient","Receiver Name","Original Invoice Number","Original Invoice date","Revised Invoice Number","Revised Invoice date","Invoice Value","Place Of Supply","Reverse Charge", "Applicable % of Tax Rate", "Invoice Type","E-Commerce GSTIN","Rate","Taxable Value","Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2BA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[7]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[7].Rows.Count);
            }
            else if (strSheetName == "b2cl")
            {
                var range = ExcelWorkSheet.get_Range("C3", "C10000");
                range.NumberFormat = "#######.00";
                range = ExcelWorkSheet.get_Range("F3", "H10000");
                range.NumberFormat = "#######.00";


                string[] strColumn = { "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN","Sale from Bonded WH" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2CL(5)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[1]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[1].Rows.Count);
            }
            else if (strSheetName == "b2cla")
            {
                var range = ExcelWorkSheet.get_Range("F3", "J10000");
                range.NumberFormat = "#######.00";
                
                string[] strColumn = { "Original Invoice date","Original Place Of Supply","Revised Invoice Number","Revised Invoice date", "Invoice Value", "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN", "Sale from Bonded WH" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2CLA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[8]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[8].Rows.Count);
            }
            else if (strSheetName == "b2cs")
            {
                var range = ExcelWorkSheet.get_Range("D3", "F10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "Type", "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2CS(7)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[2]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[2].Rows.Count);
            }
            else if (strSheetName == "b2csa")
            {
                var range = ExcelWorkSheet.get_Range("F3", "H10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = {"Financial Year","Original Month","Original Place Of Supply","Revised Place Of Supply", "Type", "Applicable % of Tax Rate", "Place Of Supply", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For B2CSA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[9]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[9].Rows.Count);
            }
            else if (strSheetName == "cdnr")
            {
                var range = ExcelWorkSheet.get_Range("I3", "M10000");
                range.NumberFormat = "#######.00";

               // string[] strColumn = { "GSTIN/UIN of Recipient","Receiver Name", "Invoice/Advance Receipt Number", "Invoice/Advance Receipt date", "Note/Refund Voucher Number", "Note/Refund Voucher date", "Document Type", "Place Of Supply", "Note/Refund Voucher Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "Pre GST" };
                string[] strColumn = {"GSTIN/UIN of Recipient","Receiver Name","Note Number","Note Date","Note Type","Place Of Supply","Reverse Charge","Note Supply Type","Note Value","Applicable % of Tax Rate","Rate","Taxable Value","Cess Amount"};
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For CDNR(9B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[3]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[3].Rows.Count);
            }
            else if (strSheetName == "cdnra")
            {
                var range = ExcelWorkSheet.get_Range("K3", "O10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "GSTIN/UIN of Recipient","Receiver Name","Original Note/Refund Voucher Number","Original Note/Refund Voucher date","Original Invoice/Advance Receipt Number","Original Invoice/Advance Receipt date","Revised Note/Refund Voucher Number","Revised Note/Refund Voucher date","Document Type","Supply Type","Note/Refund Voucher Value", "Applicable % of Tax Rate", "Rate","Taxable Value","Cess Amount","Pre GST" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For CDNRA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[10]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[10].Rows.Count);
            }
            else if (strSheetName == "cdnur")
            {
                var range = ExcelWorkSheet.get_Range("H3", "L10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "UR Type", "Note/Refund Voucher Number", "Note/Refund Voucher date", "Document Type", "Invoice/Advance Receipt Number", "Invoice/Advance Receipt date",  "Place Of Supply", "Note/Refund Voucher Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "Pre GST" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For CDNUR(9B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[4]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[4].Rows.Count);
            }
            else if (strSheetName == "cdnura")
            {
                var range = ExcelWorkSheet.get_Range("J3", "N10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "UR Type","Original Note/Refund Voucher Number","Original Note/Refund Voucher date","Original Invoice/Advance Receipt Number","Original Invoice/Advance Receipt date","Revised Note/Refund Voucher Number","Revised Note/Refund Voucher date","Document Type","Supply Type","Note/Refund Voucher Value", "Applicable % of Tax Rate","Rate","Taxable Value","Cess Amount","Pre GST" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For CDNURA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[11]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[11].Rows.Count);
            }
            else if (strSheetName == "exp")
            {
                string[] strColumn = { "Export Type", "Invoice Number", "Invoice date", "Invoice Value", "Port Code", "Shipping Bill Number", "Shipping Bill Date", "Applicable % of Tax Rate", "Rate", "Taxable Value" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For EXP(6)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "expa")
            {
                string[] strColumn = {"Export Type","Original Invoice Number","Original Invoice date","Revised Invoice Number","Revised Invoice date","Invoice Value","Port Code","Shipping Bill Number","Shipping Bill Date","Applicable % of Tax Rate","Rate","Taxable Value","Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For EXPA";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "at")
            {
                string[] strColumn = { "Place Of Supply","Applicable % of Tax Rate", "Rate", "Gross Advance Received", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Advance Received (11B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "at")
            {
                string[] strColumn = { "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Received", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Advance Received (11B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "ata")
            {
                string[] strColumn = { "Financial Year","Original Month","Original Place Of Supply","Applicable % of Tax Rate","Rate","Gross Advance Received","Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Amended Tax Liability(Advance Received)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "atadj")
            {
                string[] strColumn = { "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Adjusted", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Advance Adjusted (11B)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "atadja")
            {
                string[] strColumn = { "Financial Year","Original Month","Original Place Of Supply","Applicable % of Tax Rate","Rate","Gross Advance Adjusted","Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Amendement Of Adjustment Advances";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, new DataTable());
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
            }
            else if (strSheetName == "exemp")
            {
                var range = ExcelWorkSheet.get_Range("B3", "D10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "Description", "Nil Rated Supplies", "Exempted (other than nil rated/non GST supply )", "Non-GST supplies" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For Nil rated, exempted and non GST outward supplies (8)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[5]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[5].Rows.Count);
            }
            else if (strSheetName == "hsn")
            {
                var range = ExcelWorkSheet.get_Range("D3", "J10000");
                range.NumberFormat = "#######.00";

                string[] strColumn = { "HSN", "Description", "UQC", "Total Quantity", "Total Value", "Taxable Value", "Integrated Tax Amount", "Central Tax Amount", "State/UT Tax Amount", "Cess Amount" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary For HSN(12)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheet(ref ExcelWorkSheet, ds.Tables[6]);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, ds.Tables[6].Rows.Count);
            }
            else if (strSheetName == "docs")
            {
                string[] strColumn = { "Nature  of Document", "Sr. No. From", "Sr. No. To", "Total Number", "Cancelled" };
                ExcelWorkSheet.Name = strSheetName;
                ExcelWorkSheet.Cells[1, 1] = "Summary of documents issued during the tax period (13)";
                AddColumnsName(ref ExcelWorkSheet, strColumn);
                SetDataInSheetINDoc(ref ExcelWorkSheet, ds);
                SetSheetSummary(ref ExcelWorkSheet, strSheetName, 1);
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

            ColorConverter cc = new ColorConverter();

            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 1];
            if (ExcelWorkSheet.Name != "cdnr" && ExcelWorkSheet.Name != "cdnur")
            {
                objRange.Font.ColorIndex = 2;// = Color.FromArgb(255, 255, 255); ;// ColorTranslator.ToOle((Color)cc.ConvertFromString("#FFFFFF"));
                objRange.Interior.ColorIndex = 49;//  Color.FromArgb(0, 112, 192);// ColorTranslator.ToOle((Color)cc.ConvertFromString("#0070C0"));
                objRange.Cells.BorderAround();
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
            int rowIndex = 5, colIndex = 1;
            foreach (DataRow row in dt.Rows)
            {
                colIndex = 1;
                for (; colIndex <= dt.Columns.Count; colIndex++)
                    ExcelWorkSheet.Cells[rowIndex, colIndex] = row[colIndex - 1];

                rowIndex++;
            }         
        }

        private void BindSummary_Data(ref NewExcel.Worksheet ExcelWorkSheet, DataTable dt,ref int rowIndex)
        {
            DataRow row = dt.Rows[0];
            double _dCount = dba.ConvertObjectToDouble(row["MaxBillNo"]);
            if (_dCount > 0)
            {
                ExcelWorkSheet.Cells[rowIndex, 1] = "Invoices for outward supply (" + row["BillType"] + ")";
                ExcelWorkSheet.Cells[rowIndex, 2] = row["BCode"] + "" + row["MinBillNo"];
                ExcelWorkSheet.Cells[rowIndex, 3] = row["BCode"] + "" + row["MaxBillNo"];
                ExcelWorkSheet.Cells[rowIndex, 4] = (_dCount - dba.ConvertObjectToDouble(row["MinBillNo"])) + 1;
                ExcelWorkSheet.Cells[rowIndex, 5] = row["MissNum"];
                rowIndex++;
            }
        }

        private void SetDataInSheetINDoc(ref NewExcel.Worksheet ExcelWorkSheet, DataSet ds)
        {
            int rowIndex = 5;

            DataTable dt = ds.Tables[12];
            if (dt.Rows.Count > 0)
                BindSummary_Data(ref ExcelWorkSheet, dt, ref rowIndex);
            dt = ds.Tables[13];
            if (dt.Rows.Count > 0)
                BindSummary_Data(ref ExcelWorkSheet, dt, ref rowIndex);
            dt = ds.Tables[14];
            if (dt.Rows.Count > 0)
                BindSummary_Data(ref ExcelWorkSheet, dt, ref rowIndex);
            dt = ds.Tables[15];
            if (dt.Rows.Count > 0)
                BindSummary_Data(ref ExcelWorkSheet, dt, ref rowIndex);
        }

        private string GetFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
            _browser.FileName = "GSTR-1.xls";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;

            return strPath;
        }

        private string GetJSONFileName()
        {
            string strPath = "";
            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "JSON Files (*.json)|*.json;";
            _browser.FileName = "GSTR-1.json";
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strPath = _browser.FileName;

            return strPath;
        }

        private void SetSheetSummary(ref NewExcel.Worksheet ExcelWorkSheet, string strSheetName,int _count)
        {
            _count += 5;
            if (strSheetName == "b2b")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Recipients";
                ExcelWorkSheet.Cells[2, 3] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 5] = "Total Invoice Value";
                ExcelWorkSheet.Cells[2, 12] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 13] = "Total Cess";         
                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E"+ _count+")";
                ExcelWorkSheet.Cells[3, 12] = "=SUM(L5:L"+ _count+")";
            }
            else if (strSheetName == "b2ba")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Recipients";
                ExcelWorkSheet.Cells[2, 3] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 7] = "Total Invoice Value";
                ExcelWorkSheet.Cells[2, 14] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 15] = "Total Cess";

                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G" + _count + ")";
                ExcelWorkSheet.Cells[3, 14] = "=SUM(N5:N" + _count + ")";
            }
            else if (strSheetName == "b2cl")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 3] = "Total Inv Value";
                ExcelWorkSheet.Cells[2, 7] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 8] = "Total Cess";
                
                ExcelWorkSheet.Cells[3, 3] = "=SUM(C5:C"+ _count+")";
                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G"+ _count+")";
                ExcelWorkSheet.Cells[3, 8] = "=SUM(H5:H"+ _count+")";
            }
            else if (strSheetName == "b2cla")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 6] = "Total Inv Value";
                ExcelWorkSheet.Cells[2, 9] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 10] = "Total Cess";

                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F" + _count + ")";
                ExcelWorkSheet.Cells[3, 9] = "=SUM(I5:I" + _count + ")";
                ExcelWorkSheet.Cells[3, 10] = "=SUM(J5:J" + _count + ")";
            }
            else if (strSheetName == "b2cs")
            {
                ExcelWorkSheet.Cells[2, 5] = "Total Taxable  Value";
                ExcelWorkSheet.Cells[2, 6] = "Total Cess";

                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E" + _count + ")";
                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F"+ _count+")";
            }
            else if (strSheetName == "b2csa")
            {
                ExcelWorkSheet.Cells[2, 7] = "Total Taxable  Value";
                ExcelWorkSheet.Cells[2, 8] = "Total Cess";

                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G" + _count + ")";
                ExcelWorkSheet.Cells[3, 8] = "=SUM(H5:H" + _count + ")";
            }
            else if (strSheetName == "cdnr")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Recipients";
                ExcelWorkSheet.Cells[2, 3] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 5] = "No. of Notes/Vouchers";
                ExcelWorkSheet.Cells[2, 9] = "Total Note/Refund Voucher Value";
                ExcelWorkSheet.Cells[2, 12] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 13] = "Total Cess";                
                //ExcelWorkSheet.Cells[3, 9] = "=SUM(I5:I"+ _count+")";
                //ExcelWorkSheet.Cells[3, 12] = "=SUM(L5:L"+ _count+")";
                //ExcelWorkSheet.Cells[3, 13] = "=SUM(M5:M"+ _count+")";

                ExcelWorkSheet.Cells[3, 9] = "=(SUMIF($E$5:$E$" + _count + ",\"C\",$I$5:$I$" + _count + ")-SUMIF($E$5:$E$" + _count + ",\"D\",$I$5:$I$" + _count + "))";
                ExcelWorkSheet.Cells[3, 12] = "=(SUMIF($E$5:$E$" + _count + ",\"C\",$L$5:$L$" + _count + ")-SUMIF($E$5:$E$" + _count + ",\"D\",$L$5:$L$" + _count + "))";
                ExcelWorkSheet.Cells[3, 13] = "=(SUMIF($E$5:$E$" + _count + ",\"C\",$M$5:$M$" + _count + ")-SUMIF($E$5:$E$" + _count + ",\"D\",$M$5:$M$" + _count + "))";

            }
            else if (strSheetName == "cdnra")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of Recipients";
                ExcelWorkSheet.Cells[2, 3] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 5] = "No. of Notes/Vouchers";
                ExcelWorkSheet.Cells[2, 11] = "Total Note/Refund Voucher Value";
                ExcelWorkSheet.Cells[2, 14] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 15] = "Total Cess";

                //ExcelWorkSheet.Cells[3, 11] = "=SUM(K5:K" + _count + ")";
                //ExcelWorkSheet.Cells[3, 14] = "=SUM(N5:N" + _count + ")";
                //ExcelWorkSheet.Cells[3, 15] = "=SUM(O5:O" + _count + ")";

                ExcelWorkSheet.Cells[3, 11] = "=(SUMIF($I$5:$I$" + _count + ",\"C\",$K$5:$K$" + _count + ")-SUMIF($I$5:$I$" + _count + ",\"D\",$K$5:$K$" + _count + "))";
                ExcelWorkSheet.Cells[3, 14] = "=(SUMIF($I$5:$I$" + _count + ",\"C\",$N$5:$N$" + _count + ")-SUMIF($I$5:$I$" + _count + ",\"D\",$N$5:$N$" + _count + "))";
                ExcelWorkSheet.Cells[3, 15] = "=(SUMIF($I$5:$I$" + _count + ",\"C\",$O$5:$O$" + _count + ")-SUMIF($I$5:$I$" + _count + ",\"D\",$O$5:$O$" + _count + "))";

            }
            else if (strSheetName == "cdnur")
            {
                ExcelWorkSheet.Cells[2, 2] = "No. of Notes/Vouchers";
                ExcelWorkSheet.Cells[2, 5] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 8] = "Total Note Value";
                ExcelWorkSheet.Cells[2, 11] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 12] = "Total Cess";
                
                //ExcelWorkSheet.Cells[3, 8] = "=SUM(H5:H"+ _count+")";
                //ExcelWorkSheet.Cells[3, 11] = "=SUM(K5:K"+ _count+")";
                //ExcelWorkSheet.Cells[3, 12] = "=SUM(L5:L"+ _count+")";

                ExcelWorkSheet.Cells[3, 8] = "=(SUMIF($D$5:$D$" + _count + ",\"C\",$H$5:$H$" + _count + ")-SUMIF($D$5:$D$" + _count + ",\"D\",$H$5:$H$" + _count + "))";
                ExcelWorkSheet.Cells[3, 11] = "=(SUMIF($D$5:$D$" + _count + ",\"C\",$K$5:$K$" + _count + ")-SUMIF($D$5:$D$" + _count + ",\"D\",$K$5:$K$" + _count + "))";
                ExcelWorkSheet.Cells[3, 12] = "=(SUMIF($D$5:$D$" + _count + ",\"C\",$L$5:$L$" + _count + ")-SUMIF($D$5:$D$" + _count + ",\"D\",$L$5:$L$" + _count + "))";
            }
            else if (strSheetName == "cdnura")
            {
                ExcelWorkSheet.Cells[2, 2] = "No. of Notes/Vouchers";
                ExcelWorkSheet.Cells[2, 4] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 10] = "Total Note Value";
                ExcelWorkSheet.Cells[2, 13] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 14] = "Total Cess";

                //ExcelWorkSheet.Cells[3, 10] = "=SUM(J5:J" + _count + ")";
                //ExcelWorkSheet.Cells[3, 13] = "=SUM(M5:M" + _count + ")";
                //ExcelWorkSheet.Cells[3, 14] = "=SUM(N5:N" + _count + ")";

                ExcelWorkSheet.Cells[3, 10] = "=(SUMIF($H$5:$H$" + _count + ",\"C\",$J$5:$J$" + _count + ")-SUMIF($H$5:$H$" + _count + ",\"D\",$J$5:$J$" + _count + "))";
                ExcelWorkSheet.Cells[3, 13] = "=(SUMIF($H$5:$H$" + _count + ",\"C\",$M$5:$M$" + _count + ")-SUMIF($H$5:$H$" + _count + ",\"D\",$M$5:$M$" + _count + "))";
                ExcelWorkSheet.Cells[3, 14] = "=(SUMIF($H$5:$H$" + _count + ",\"C\",$N$5:$N$" + _count + ")-SUMIF($H$5:$H$" + _count + ",\"D\",$N$5:$N$" + _count + "))";
            }
            else if (strSheetName == "exp")
            {
                ExcelWorkSheet.Cells[2, 2] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 4] = "Total Invoice Value";
                ExcelWorkSheet.Cells[2, 6] = "No. of Shipping Bill";
                ExcelWorkSheet.Cells[2, 11] = "Total Taxable Value";

              //  ExcelWorkSheet.Cells[3, 2] = "=SUMPRODUCT(1/COUNTIF(B5:B" + _count + ", B5:B" + _count + "))";
                ExcelWorkSheet.Cells[3, 4] = "=SUM(D5:D"+ _count+")";
                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F"+ _count+")";
                ExcelWorkSheet.Cells[3, 11] = "=SUM(J5:J"+ _count+")";
            }
            else if (strSheetName == "expa")
            {
                ExcelWorkSheet.Cells[2, 2] = "No. of Invoices";
                ExcelWorkSheet.Cells[2, 6] = "Total Invoice Value";
                ExcelWorkSheet.Cells[2, 8] = "No. of Shipping Bill";
                ExcelWorkSheet.Cells[2, 13] = "Total Taxable Value";

                //  ExcelWorkSheet.Cells[3, 2] = "=SUMPRODUCT(1/COUNTIF(B5:B" + _count + ", B5:B" + _count + "))";
                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F" + _count + ")";
                ExcelWorkSheet.Cells[3, 8] = "=SUM(H5:H" + _count + ")";
                ExcelWorkSheet.Cells[3, 13] = "=SUM(L5:L" + _count + ")";
            }
            else if (strSheetName == "at")
            {
                ExcelWorkSheet.Cells[2, 4] = "Total Advance Received";
                ExcelWorkSheet.Cells[2, 5] = "Total Cess";

                ExcelWorkSheet.Cells[3, 4] = "=SUM(D5:D"+ _count+")";
                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E"+ _count+")";                
            }
            else if (strSheetName == "ata")
            {
                ExcelWorkSheet.Cells[2, 6] = "Total Advance Received";
                ExcelWorkSheet.Cells[2, 7] = "Total Cess";

                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F" + _count + ")";
                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G" + _count + ")";
            }
            else if (strSheetName == "atadj")
            {
                ExcelWorkSheet.Cells[2, 4] = "Total Advance Adjusted";
                ExcelWorkSheet.Cells[2, 5] = "Total Cess";

                ExcelWorkSheet.Cells[3, 4] = "=SUM(D5:D"+ _count+")";
                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E"+ _count+")";
            }
            else if (strSheetName == "atadja")
            {
                ExcelWorkSheet.Cells[2, 6] = "Total Advance Adjusted";
                ExcelWorkSheet.Cells[2, 7] = "Total Cess";

                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F" + _count + ")";
                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G" + _count + ")";
            }
            else if (strSheetName == "exemp")
            {
                ExcelWorkSheet.Cells[2, 2] = "Total Nil Rated Supplies";
                ExcelWorkSheet.Cells[2, 3] = "Total Exempted Supplies";
                ExcelWorkSheet.Cells[2, 4] = "Total Non-GST Supplies";

                ExcelWorkSheet.Cells[3, 2] = "=SUM(B5:B"+ _count+")";
                ExcelWorkSheet.Cells[3, 3] = "=SUM(C5:C"+ _count+")";
                ExcelWorkSheet.Cells[3, 4] = "=SUM(D5:D"+ _count+")";
            }
            else if (strSheetName == "hsn")
            {
                ExcelWorkSheet.Cells[2, 1] = "No. of HSN";
                ExcelWorkSheet.Cells[2, 5] = "Total Value";
                ExcelWorkSheet.Cells[2, 6] = "Total Taxable Value";
                ExcelWorkSheet.Cells[2, 7] = "Total Integrated Tax";
                ExcelWorkSheet.Cells[2, 8] = "Total Central Tax";
                ExcelWorkSheet.Cells[2, 9] = "Total State/UT Tax";
                ExcelWorkSheet.Cells[2, 10] = "Total Cess";

                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E"+ _count+")";
                ExcelWorkSheet.Cells[3, 6] = "=SUM(F5:F"+ _count+")";
                ExcelWorkSheet.Cells[3, 7] = "=SUM(G5:G"+ _count+")";
                ExcelWorkSheet.Cells[3, 8] = "=SUM(H5:H"+ _count+")";
                ExcelWorkSheet.Cells[3, 9] = "=SUM(I5:I"+ _count+")";
                ExcelWorkSheet.Cells[3, 10] = "=SUM(J5:J"+ _count+")";
            }
            else if (strSheetName == "docs")
            {
                ExcelWorkSheet.Cells[2, 4] = "Total Number";
                ExcelWorkSheet.Cells[2, 5] = "Total Cancelled";

                ExcelWorkSheet.Cells[3, 4] = "=SUM(D5:D"+ _count+")";
                ExcelWorkSheet.Cells[3, 5] = "=SUM(E5:E"+ _count+")";
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    GetDataForExport();
                }
            }
            catch(Exception ex) { MessageBox.Show("Sorry " + ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning); }
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
                txtStateName.Focus();

            }
            catch
            {
            }
        }

        private void txtStateName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStateName.Text = objSearch.strSelectedData;
                    ClearAllRecord();
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
                txtStateName.Text = objSearch.strSelectedData;
                ClearAllRecord();
                btnGo.Focus();
            }
            catch
            {
            }
        }

        private void GSTR_1_Summary_Load(object sender, EventArgs e)
        {

        }

        private void btnExportJSON_Click(object sender, EventArgs e)
        {
            btnExportJSON.Enabled = false;
            try
            {
                DialogResult dir = MessageBox.Show("Are you want to export record in JSON", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dir == DialogResult.Yes)
                {
                    GetAndSaveJSONFile();
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExportJSON.Enabled = true;
        }

        private void GetAndSaveJSONFile()
        {
            DataSet ds = GetExportedDataFromDB();
            if (ds.Tables.Count > 0)
            {
                object objJson = DataBaseAccess.ExecuteMyScalar("Select GSTNo from CompanyDetails Where  Other='" + MainPage.strCompanyName + "' ");
                string strFileName = GetJSONFileName(), strJSON = "",strFinYear= GetFinYear();
                strJSON = PrepareJSON.GetGSTR1_JSON(ds, Convert.ToString(objJson), strFinYear);
              bool _bStatus=  DataBaseAccess.SaveFile(strJSON,strFileName);
                if(_bStatus)
                {
                    MessageBox.Show("Thank You ! JSON File Imported successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }         
            }
        }

        private string GetFinYear()
        {
            string strFinYear = "";
            int _month = 0;
            //"", "", "", "", "", "", "", "", "", "", "", ""
            if (txtMonth.Text == "JANUARY")
                _month = 1;
            else if (txtMonth.Text == "FEBRUARY")
                _month = 2;
            else if (txtMonth.Text == "MARCH")
                _month = 3;
            else if (txtMonth.Text == "APRIL")
                _month = 4;
            else if (txtMonth.Text == "MAY")
                _month = 5;
            else if (txtMonth.Text == "JUNE")
                _month = 6;
            else if (txtMonth.Text == "JULY")
                _month = 7;
            else if (txtMonth.Text == "AUGUST")
                _month = 8;
            else if (txtMonth.Text == "SEPTEMBER")
                _month = 9;
            else if (txtMonth.Text == "OCTOBER")
                _month = 10;
            else if (txtMonth.Text == "NOVEMBER")
                _month = 11;
            else if (txtMonth.Text == "DECEMBER")
                _month = 12;
            if (_month < 4)
                strFinYear = _month.ToString("00") + MainPage.endFinDate.Year.ToString();
            else
                strFinYear = _month.ToString("00") + MainPage.startFinDate.Year.ToString();
            return strFinYear;
        }

        private void GSTR_1_Summary_Load_1(object sender, EventArgs e)
        {
            btnExport.Enabled = btnExportJSON.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }
    }

}
