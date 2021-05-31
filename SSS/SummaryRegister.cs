using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class SummaryRegister : Form
    {
        DataBaseAccess dba;
        protected internal bool _bSearchStatus = false;
        string Mode = "SALES";
        public SummaryRegister(string _mode = "SALES")
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            Mode = _mode;
            SetMode();
        }
        private void SetMode()
        {
            LabelHeader.Text = toTitleCase(Mode) + " Summary Report";
            this.Text = toTitleCase(Mode) + " Summary Report";

            lableAmt.Text = lableAmt.Text + char.ConvertFromUtf32(8595);
            labelIGST.Text = labelIGST.Text + char.ConvertFromUtf32(8595);
            labelCGST.Text = labelCGST.Text + char.ConvertFromUtf32(8595);
            labelSGST.Text = labelSGST.Text + char.ConvertFromUtf32(8595);
            labelROOffGST.Text = labelROOffGST.Text + char.ConvertFromUtf32(8595);
            labelTotInv.Text = labelTotInv.Text + char.ConvertFromUtf32(8595);
            labelTCS.Text = labelTCS.Text + char.ConvertFromUtf32(8595);
            labelTaxFree.Text = labelTaxFree.Text + char.ConvertFromUtf32(8595);

            if (Mode.Contains("SALE"))
                lblPartyHeader.Text = "Sundry Debitors  :";
            else
                lblPartyHeader.Text = "Sundry Creditors  :";

            if (Mode != "PURCHASE")
            {
                rdoAll.Checked = true;
                grpJournal.Enabled = false;

                labelTCS.Visible = labelTCSAmt.Visible = false;
                labelTaxFree.Left = labelTaxFree.Left - 80;
                labelTaxFreeAmt.Left = labelTaxFree.Left;
                labelTotInv.Left = labelTotInv.Left - 80;
                lblTotalInvValue.Left = labelTotInv.Left ;
            }
        }
        private string toTitleCase(string str)
        {
            string[] strs = str.Split(' ');
            str = "";
            foreach (string s in strs)
            {
                string st = "";
                st = s.Substring(0, 1).ToUpper() + s.Substring(1, s.Length - 1).ToLower();
                str += " " + st;
            }
            return str.TrimStart().TrimEnd();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            btnSearch.Enabled = false;
            try
            {
                if (txtSalesParty.Text != "" || MainPage.mymainObject.bShowAllRecord)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                        MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        GetDataFromDB();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Party Name !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }
        private string CreateSummaryQuery()
        {
            string strQuery = "", strCodeQuery = "", strCodeBAQuery = "", strIfDate = "", strSubQuery = "", strOuterWhereQry = "";

            switch (Mode)
            {
                case "PURCHASE":
                    if (txtSalesParty.Text != "")
                    {
                        strCodeQuery = " and ISNULL((PurchasePartyID+' '+SM.Name),PurchasePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strIfDate += " and (PB.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and PB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                    {
                        strCodeQuery += " and BillCode='" + txtBillCode.Text + "' ";
                        strCodeBAQuery += " and VoucherCode='" + txtBillCode.Text + "' ";
                    }

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                    {
                        strCodeQuery += " and (BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                        strCodeBAQuery += " and (VoucherNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    }
                    if (rdoGSTRegular.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY Sales.Party_Name) "
                           + ", Party_Name "
                           + ", Party_GST,PARTY_GROUP "
                           + ", Taxable_Value = SUM(ISNULL(GST.TaxableAmt + isnull(Sales.TaxFree, 0) ,Sales.TaxableAmt))"
                           + ", IGST_Amt = SUM(ISNULL(Cast(GST.IGSTAmt as Numeric(18, 4)),Sales.IGSTAmt)) "
                           + ", CGST_Amt = SUM(ISNULL(Cast(GST.CGSTAmt as Numeric(18, 4)) ,Sales.CGSTAmt))"
                           + ", SGST_Amt = SUM(ISNULL(Cast(GST.CGSTAmt as Numeric(18, 4)),Sales.CGSTAmt)) "
                           + ", Total_Invoice_Value = SUM(ISNULL(NetAmt,0)) "
                           + ", Tax_Free_Amt = SUM(ISNULL(Sales.TaxFree, 0)) "
                           + " FROM ( ";
                    if (rdoAll.Checked || rdoNonJournal.Checked)
                    {
                        strQuery += " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''), PARTY_GROUP = ISNULL(SM.GroupName, ''), PB.PurchasePartyID "
                            + ", BillDate = Convert(nVarchar(20), PB.DATE, 103), INVOICE_Code = PB.BillCode, INVOICE_No = Convert(nvarchar(20), PB.BillNo), NetAmt "
                            + ", (Isnull(NetAmt, 0) - ISNULL(TaxAmt, 0))TaxableAmt, ROSign + Convert(Varchar(10), RoundOff)RoundOffAmt,ISNULL(TCSAmt,0) TCSAmt ,ISNULL(TaxFree,0)TaxFree, IGSTAmt = 0,  CGSTAmt = 0 "
                            + " FROM PurchaseBook PB "
                            + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PB.PurchasePartyID WHERE 1 = 1 " + strCodeQuery + strIfDate
                            + " UNION ALL "
                            + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''),PARTY_GROUP = ISNULL(SM.GroupName, ''), PR.PurchasePartyID "
                            + ", BillDate = Convert(nVarchar(20), PR.BillDate, 103), INVOICE_Code = PR.BillCode, INVOICE_No = Convert(nvarchar(20), PR.BillNo) "
                            + ", CAST(NetAmt as Money) as NetAmt, (Isnull(CAST(NetAmt as Money), 0) - ISNULL(TaxAmount, 0))TaxableAmt, RoundOffSign + Convert(Varchar(10), RoundOffAmt)RoundOffAmt,ISNULL(TCSAmt,0) TCSAmt , TaxFree = ISNULL(Tax,0), IGSTAmt = 0,  CGSTAmt = 0  "
                            + " FROM PurchaseRecord PR "
                            + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PR.PurchasePartyID WHERE 1 = 1 " + strCodeQuery + strIfDate.Replace("PB.Date", "PR.BillDate");
                    }
                    if (rdoAll.Checked || rdoJournal.Checked)
                    {
                        if (strQuery.Contains("UNION ALL"))
                            strQuery += " UNION ALL ";

                        strQuery += " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''),PARTY_GROUP = ISNULL(SM1.GroupName, '') "
                        + ", _BA.PartyID PurchasePartyID, BillDate = Convert(nVarchar(20), _Ba.Date, 103), INVOICE_Code = _BA.VoucherCode, INVOICE_No = Convert(nvarchar(20), _BA.VoucherNo), NetAmt = (_BA.Amount+IGSTAmt+CGSTAmt) "
                        + ", TaxableAmt = (_BA.Amount), RoundOffAmt = '0', TCSAmt = 0 , TaxFree = 0 , IGSTAmt ,  CGSTAmt"
                        + " FROM( "
                        + " Select * from (SELECT  JVD.VoucherCode, JVD.VoucherNo, SUM(Cast(isnull(JVD.DiffAmt, 0) as Money))Amount"
                        + ",(Select Top 1 BA.DATE from BalanceAmount BA WHere BA.VoucherCode = JVD.VoucherCode AND BA.VoucherNo = JVD.VoucherNo)Date"
                        + ", SUM(isnull(JVD.IGSTAmt, 0))IGSTAmt, SUM(isnull(JVD.CGSTAmt, 0))CGSTAmt, (JVD.PartyID)PartyID,JVD.AccountID as PartyAccountID  FROM JournalVoucherDetails JVD GROUP BY JVD.VoucherCode, JVD.VoucherNo, JVD.PartyID,JVD.AccountID)_BA Where VoucherCode!='' " + strCodeBAQuery + strIfDate.Replace("PB.", "")
                        + " )_BA "
                        + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = _BA.PartyID "
                        + " LEFT JOIN SupplierMaster SM1 ON(SM1.AreaCode + SM1.AccountNo) = _BA.PartyAccountID ";
                    }
                    strQuery += " ) Sales  LEFT JOIN( "
                    + " SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                    + " Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmount, (SUM(ISNULL(GD.TaxAmount, 0))  * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                    + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt "
                    + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                    + " from GSTDetails GD  WHere BillType IN('PURCHASE') "
                    + " AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate "
                    + " )GST2 GROUP BY BillCode, BillNo "
                    + " ) as GST ON GST.BillCode = Sales.INVOICE_Code AND GST.BillNo = Sales.INVOICE_No " + strOuterWhereQry
                    + " GROUP BY Party_Name,Party_GST,PARTY_GROUP "
                    + " ORDER BY Party_Name ";
                    break;

                case "SALES":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((SalePartyId+' '+SM.Name),SalePartyId) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strIfDate += " and (SB.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and SB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    if (rdoGSTRegular.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY Sales.Party_Name), Party_Name, Party_GST"
                        + ", Taxable_Value = CAST(SUM(GST.TaxableAmt) + SUM(TaxFree) as Numeric(18,2))"
                        + ", CAST(SUM(TaxFree) as Numeric(18,2)) Tax_Free_Amt"
                        + ", IGST_Amt = Cast(Sum(ISNULL(GST.IGSTAmt, 0)) as Numeric(18, 4))"
                        + ", CGST_Amt = Cast(Sum(ISNULL(GST.CGSTAmt, 0)) as Numeric(18, 4))"
                        + ", SGST_Amt = Cast(Sum(ISNULL(GST.CGSTAmt, 0)) as Numeric(18, 4))"
                        + ", Total_Invoice_Value = Sum(ISNULL(NetAmt, 0))"
                        + " FROM("
                        + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, '')"
                        + " , BillDate = Convert(nVarchar(20), SB.DATE, 103), INVOICE_Code = SB.BillCode, INVOICE_No = Convert(nvarchar(20), SB.BillNo), NetAmt, (Isnull(NetAmt, 0) - ISNULL(TaxAmt, 0))TaxableAmt"
                        + " ,0 TaxFree, RoundOffSign + Convert(Varchar(10), RoundOffAmt)RoundOffAmt"
                        + " FROM SalesBook SB"
                        + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = SB.SalePartyID"
                        + " WHERE 1 = 1" + strSubQuery + strIfDate
                        + " UNION ALL"
                        + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, '')"
                        + " , BillDate = Convert(nVarchar(20), SR.BillDate, 103), INVOICE_Code = SR.BillCode, INVOICE_No = Convert(nvarchar(20), SR.BillNo), CAST(NetAmt as Money) NetAmt"
                        + " , (Isnull(CAST(NetAmt as Money), 0) - ISNULL(TaxAmount, 0))TaxableAmt , TaxFree, RoundOffSign + Convert(Varchar(10), RoundOffAmt)RoundOffAmt"
                        + " FROM SalesRecord SR"
                        + " LEFT JOIN (SELECT SUM(Cast(Tax as Numeric(18,4))) TaxFree, SE.BillCode,SE.BillNo FROM SalesEntry SE GROUP BY SE.BillCode,SE.BillNo)SEE on SR.BillCode = SEE.BillCode AND SR.BillNo = SEE.BillNo "
                        + "  LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = SR.SalePartyID"
                        + " WHERE 1 = 1" + strSubQuery + strIfDate.Replace("SB.", "SR.")
                        + " UNION ALL"
                        + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, '')"
                        + " , BillDate = Convert(nVarchar(20), SSB.Date, 103), INVOICE_Code = SSB.BillCode, INVOICE_No = Convert(nvarchar(20), SSB.BillNo), NetAmt"
                        + " , (Isnull(NetAmt, 0) - ISNULL(TaxAmt, 0))TaxableAmt, 0 TaxFree, RoundOffSign + Convert(Varchar(10), RoundOffAmt)RoundOffAmt FROM SaleServiceBook SSB"
                        + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = SSB.SalePartyID WHERE 1 = 1" + strSubQuery + strIfDate.Replace("SB.", "SSB.")
                        + " ) Sales"
                        + " LEFT JOIN("
                        + " SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt"
                        + " FROM(Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmount, (SUM(ISNULL(GD.TaxAmount, 0)) * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt"
                        + " , (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt"
                        + " , (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt"
                        + "  from GSTDetails GD WHere BillType IN('SALESERVICE', 'SALES') AND ISNULL(TaxRate,0) > 0"
                        + " Group by BillCode,BillNo,TaxType,TaxRate"
                        + " )GST2 GROUP BY BillCode, BillNo )"
                        + "  as GST ON GST.BillCode = Sales.INVOICE_Code AND GST.BillNo = Sales.INVOICE_No " + strOuterWhereQry
                        + " GROUP BY  Party_Name, Party_GST ORDER BY Party_Name";
                    break;

                case "PURCHASE RETURN":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((PRTN.PurchasePartyID+' '+SM.Name),PRTN.PurchasePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strSubQuery += " and (PRTN.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and PRTN.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and PRTN.BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (PRTN.BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    if (rdoGSTRegular.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') = '' ";

                    strQuery = "SELECT S_No = ROW_NUMBER() Over(ORDER BY PARTY_NAME),PARTY_NAME,GSTNo PARTY_GST,SUM(TaxableAmt)Taxable_Value "
                                + ",Cast(SUM(ISNULL(IGSTAmt, 0)) as Numeric(18, 2))IGST_Amt,Cast(SUM(ISNULL(CGSTAmt, 0)) as Numeric(18, 2))CGST_Amt "
                                + ",Cast(SUM(ISNULL(CGSTAmt, 0)) as Numeric(18, 2))SGST_Amt,SUM(ISNULL(NetAmt, 0)) Total_Invoice_Value FROM ("
                                + "SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), SM.GSTNo, GST.TaxableAmt, GST.IGSTAmt, GST.CGSTAmt, NetAmt "
                                + "FROM PurchaseReturn PRTN "
                                + "LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PRTN.PurchasePartyID "
                                + "LEFT JOIN(SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                                + "Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmount, (SUM(ISNULL(GD.TaxAmount, 0)) * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                                + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt "
                                + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                                + "from GSTDetails GD  WHere BillType IN('PURCHASERETURN') AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate "
                                + ")GST2 GROUP BY BillCode, BillNo   ) as GST ON GST.BillCode = PRTN.BillCode AND GST.BillNo = PRTN.BillNo WHERE 1 = 1 " + strSubQuery
                                + ") PurC GROUP BY PARTY_NAME, GSTNo";
                    break;

                case "SALE RETURN":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((SRTN.SalePartyID+' '+SM.Name),SRTN.SalePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strSubQuery += " and (SRTN.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and SRTN.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and SRTN.BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (SRTN.BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";

                    if (rdoGSTRegular.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') = '' ";

                     strQuery = "SELECT S_No = ROW_NUMBER() Over(ORDER BY PARTY_NAME) , PARTY_NAME,GSTNo PARTY_GST, SUM(TaxableAmt)Taxable_Value"
                                + ", CAST(SUM(ISNULL(IGSTAmt,0)) as Numeric(18,2))IGST_Amt, CAST(SUM(ISNULL(CGSTAmt,0)) as Numeric(18,2))CGST_Amt, CAST(SUM(ISNULL(CGSTAmt,0)) as Numeric(18,2))SGST_Amt, SUM(Isnull(NetAmt,0))Total_Invoice_Value FROM("
                                + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), SM.GSTNo, GST.TaxableAmt, GST.IGSTAmt, GST.CGSTAmt, NetAmt FROM"
                                + " SaleReturn SRTN "
                                + "LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = SRTN.SalePartyID "
                                + " LEFT JOIN( "
                                + "SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                                + "Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmount, (SUM(ISNULL(GD.TaxAmount, 0)) * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                                + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt "
                                + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                                + "from GSTDetails GD "
                                + "WHere BillType IN('SALERETURN') AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate "
                                + ")GST2 "
                                + "GROUP BY BillCode, BillNo "
                                + ") as GST ON GST.BillCode = SRTN.BillCode AND GST.BillNo = SRTN.BillNo WHERE 1 = 1 " + strSubQuery
                                + ") SaleR GROUP BY PARTY_NAME, GSTNo ORDER BY PARTY_NAME";
                    break;
            }
            return strQuery;
        }

        private string CreateDetailQuery()
        {
            string strQuery = "", strCodeQuery = "", strCodeBAQuery = "", strIfDate = "", strIfSRDate = "", strIfBDate = "", strSubQuery = "",strOuterWhereQry = "";

            switch (Mode)
            {
                case "PURCHASE":
                    if (txtSalesParty.Text != "")
                    {
                        strCodeQuery = " and ISNULL((PurchasePartyID+' '+SM.Name),PurchasePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strIfDate += " and (PB.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and PB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                        strIfSRDate += " and (PR.BillDate >='" + sDate.ToString("MM/dd/yyyy") + "' and PR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                        strIfBDate += " and (Date >='" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                    {
                        strCodeQuery += " and BillCode='" + txtBillCode.Text + "' ";
                        strCodeBAQuery += " and VoucherCode='" + txtBillCode.Text + "' ";
                    }

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                    {
                        strCodeQuery += " and (BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                        strCodeBAQuery += " and (VoucherNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    }
                    if(rdoGSTRegular.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') != '' ";
                    else if(rdoGSTUnAutho.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY Sales.BillDate) "
                            + ", BillDate as Date "
                            + ", Party_Name "
                            + ", Party_GST,PARTY_GROUP "
                            + ", Invoice_No = (INVOICE_Code + ' ' + INVOICE_No) "
                            + ", Taxable_Value = ISNULL(GST.TaxableAmt + isnull(Sales.TaxFree, 0) ,Sales.TaxableAmt) "
                            + ", IGST_Amt = ISNULL(Cast(GST.IGSTAmt as Numeric(18, 4)),Sales.IGSTAmt) "
                            + ", CGST_Amt = ISNULL(Cast(GST.CGSTAmt as Numeric(18, 4)) ,Sales.CGSTAmt)"
                            + ", SGST_Amt = ISNULL(Cast(GST.CGSTAmt as Numeric(18, 4)),Sales.CGSTAmt) "
                            + ", Total_Invoice_Value = NetAmt "
                            + ", Taxable_Amt = isnull(Sales.TaxableAmt, 0) + isnull(Sales.TaxFree, 0) "
                            + ", TCS_Amt = isnull(Sales.TCSAmt, 0) "
                            + ", Tax_Free_Amt = isnull(Sales.TaxFree, 0) "
                            + ", [Difference_Amt] = (ISNULL(GST.TaxableAmt + isnull(Sales.TaxFree, 0) ,Sales.TaxableAmt) - isnull(Sales.TaxableAmt,0) + Cast(RoundOffAmt as Money)) "
                            + ", RoundOff_Amt = Cast(RoundOffAmt as Money) "
                            + " FROM( ";
                            if (rdoAll.Checked || rdoNonJournal.Checked)
                            {
                                strQuery += " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''), PARTY_GROUP = ISNULL(SM.GroupName, ''), PB.PurchasePartyID "
                                    + ", BillDate = Convert(nVarchar(20), PB.DATE, 103), INVOICE_Code = PB.BillCode, INVOICE_No = Convert(nvarchar(20), PB.BillNo), NetAmt "
                                    + ", (Isnull(NetAmt, 0) - ISNULL(TaxAmt, 0))TaxableAmt, ROSign + Convert(Varchar(10), RoundOff)RoundOffAmt,ISNULL(TCSAmt,0) TCSAmt ,ISNULL(TaxFree,0)TaxFree, IGSTAmt = 0,  CGSTAmt = 0"
                                    + " FROM PurchaseBook PB "
                                    + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PB.PurchasePartyID WHERE 1 = 1 " + strCodeQuery + strIfDate
                                    + " UNION ALL "
                                    + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''), PARTY_GROUP = ISNULL(SM.GroupName, ''), PR.PurchasePartyID "
                                    + ", BillDate = Convert(nVarchar(20), PR.BillDate, 103), INVOICE_Code = PR.BillCode, INVOICE_No = Convert(nvarchar(20), PR.BillNo) "
                                    + ", CAST(NetAmt as Money) as NetAmt, (Isnull(CAST(NetAmt as Money), 0) - ISNULL(TaxAmount, 0))TaxableAmt, RoundOffSign + Convert(Varchar(10), RoundOffAmt)RoundOffAmt,ISNULL(TCSAmt,0) TCSAmt , TaxFree = ISNULL(Tax,0), IGSTAmt = 0,  CGSTAmt = 0  "
                                    + " FROM PurchaseRecord PR "
                                    + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PR.PurchasePartyID WHERE 1 = 1 " + strCodeQuery + strIfSRDate;
                            }
                            if (rdoAll.Checked || rdoJournal.Checked)
                            {
                                if(strQuery.Contains("UNION ALL"))
                                    strQuery += " UNION ALL ";

                                strQuery += " SELECT PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name), PARTY_GST = ISNULL(SM.GSTNo, ''), PARTY_GROUP = ISNULL(SM1.GroupName, '') "
                                + ", _BA.PartyID PurchasePartyID, BillDate = Convert(nVarchar(20), _Ba.Date, 103), INVOICE_Code = _BA.VoucherCode, INVOICE_No = Convert(nvarchar(20), _BA.VoucherNo), NetAmt = (_BA.Amount+IGSTAmt+CGSTAmt) "
                                + ", TaxableAmt = (_BA.Amount), RoundOffAmt = '0', TCSAmt = 0 , TaxFree = 0 , IGSTAmt ,  CGSTAmt"
                                + " FROM( "
                                + " Select * from (SELECT  JVD.VoucherCode, JVD.VoucherNo, SUM(Cast(isnull(JVD.DiffAmt, 0) as Money))Amount"
                                + ",(Select Top 1 BA.DATE from BalanceAmount BA WHere BA.VoucherCode = JVD.VoucherCode AND BA.VoucherNo = JVD.VoucherNo)Date"
                                + ", SUM(isnull(JVD.IGSTAmt, 0))IGSTAmt, SUM(isnull(JVD.CGSTAmt, 0))CGSTAmt , (JVD.PartyID)PartyID,JVD.AccountID as PartyAccountID FROM JournalVoucherDetails JVD GROUP BY JVD.VoucherCode, JVD.VoucherNo, JVD.PartyID,JVD.AccountID)_BA Where VoucherCode!='' " + strCodeBAQuery + strIfBDate
                                + " )_BA "
                                + " LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = _BA.PartyID "
                                + " LEFT JOIN SupplierMaster SM1 ON(SM1.AreaCode + SM1.AccountNo) = _BA.PartyAccountID ";
                    }
                            strQuery += " ) Sales  LEFT JOIN( "
                            + " SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                            + " Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate, SUM(TaxAmount) TaxAmount, (SUM(ISNULL(GD.TaxAmount, 0))  * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                            + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt "
                            + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                            + " from GSTDetails GD  WHere BillType IN('PURCHASE') "
                            + " AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate "
                            + " )GST2 GROUP BY BillCode, BillNo "
                            + " ) as GST ON GST.BillCode = Sales.INVOICE_Code AND GST.BillNo = Sales.INVOICE_No "
                            + strOuterWhereQry
                            + "  ORDER BY Sales.BillDate ";
                    break;

                case "SALES":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((SalePartyId+' '+SM.Name),SalePartyId) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strIfDate += " and (SB.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and SB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                        strIfSRDate += " and (SR.BillDate >='" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                        strIfBDate += " and (SSB.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and SSB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    if (rdoGSTRegular.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strOuterWhereQry = " WHERE ISNULL(Party_GST,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY Sales.BillDate)  "
                    + ", BillDate as Date "
                    + ", Party_Name "
                    + ", Party_GST "
                    + ", Invoice_No = (INVOICE_Code + ' ' + INVOICE_No) "
                    + ", Taxable_Value = ROUND(GST.TaxableAmt, 2) + ISNULL(TaxFree, 0) "
                    + ", IGST_Amt = Cast(GST.IGSTAmt as Numeric(18, 4)) "
                    + ", CGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4)) "
                    + ", SGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4)) "
                    + ", Total_Invoice_Value = NetAmt "
                    + ", CAST(TaxFree as Numeric(18,2)) Tax_Free_Amt"
                    + ", (Sales.TaxableAmt + Cast(RoundOffAmt as Money)) Taxable_Amt"
                    + ", [Difference_Amt] = Round(((GST.TaxableAmt - Sales.TaxableAmt) + Cast(RoundOffAmt as Money) ),2) "
                    + ", Cast(RoundOffAmt as Money) RoundOff_Amt "
                    + " FROM( "
                    + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')+' '+SM.Name), PARTY_GST = ISNULL(SM.GSTNo,''), SB.SalePartyID, BillDate = Convert(nVarchar(20), SB.DATE, 103), INVOICE_Code = SB.BillCode, INVOICE_No = Convert(nvarchar(20), SB.BillNo), NetAmt,0 TaxFree, (Isnull(NetAmt,0)-ISNULL(TaxAmt,0))TaxableAmt,RoundOffSign + Convert(Varchar(10),RoundOffAmt )RoundOffAmt"
                    + " FROM SalesBook SB LEFT JOIN SupplierMaster SM ON (SM.AreaCode + SM.AccountNo) = SB.SalePartyID WHERE 1 = 1 " + strSubQuery + strIfDate

                    + " UNION ALL "

                    + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')+' '+SM.Name), PARTY_GST = ISNULL(SM.GSTNo,''), SR.SalePartyID, BillDate = Convert(nVarchar(20), SR.BillDate, 103), INVOICE_Code = SR.BillCode, INVOICE_No = Convert(nvarchar(20), SR.BillNo),CAST(NetAmt as Money) NetAmt, TaxFree ,(Isnull(CAST(NetAmt as Money),0)-ISNULL(TaxAmount,0))TaxableAmt ,RoundOffSign + Convert(Varchar(10),RoundOffAmt )RoundOffAmt "
                    + " FROM SalesRecord SR "
                    + " LEFT JOIN (SELECT SUM(Cast(Tax as Numeric(18,4))) TaxFree, SE.BillCode,SE.BillNo FROM SalesEntry SE GROUP BY SE.BillCode,SE.BillNo)SEE on SR.BillCode = SEE.BillCode AND SR.BillNo = SEE.BillNo "
                    + " LEFT JOIN SupplierMaster SM ON (SM.AreaCode + SM.AccountNo) = SR.SalePartyID WHERE 1 = 1 " + strSubQuery + strIfSRDate

                    + " UNION ALL "

                    + " SELECT PARTY_NAME = (ISNULL(SM.AreaCode,'')+ISNULL(SM.AccountNo,'')+' '+SM.Name), PARTY_GST = ISNULL(SM.GSTNo,''), SSB.SalePartyID, BillDate = Convert(nVarchar(20), SSB.Date, 103), INVOICE_Code = SSB.BillCode, INVOICE_No = Convert(nvarchar(20), SSB.BillNo), NetAmt,0 TaxFree ,(Isnull(NetAmt,0)-ISNULL(TaxAmt,0))TaxableAmt,RoundOffSign + Convert(Varchar(10),RoundOffAmt )RoundOffAmt "
                    + " FROM SaleServiceBook SSB LEFT JOIN SupplierMaster SM ON (SM.AreaCode + SM.AccountNo) = SSB.SalePartyID WHERE 1 = 1 " + strSubQuery + strIfBDate
                    + " ) Sales "
                    + " LEFT JOIN "
                    + " ( "
                    + " SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM "
                    + " ( "
                    + " Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate "
                    + ", SUM(TaxAmount) TaxAmount "
                    + ", (SUM(ISNULL(GD.TaxAmount, 0))  * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                    + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt "
                    + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                    + " from GSTDetails GD "
                    + " WHere BillType IN('SALESERVICE', 'SALES') AND ISNULL(TaxRate, 0) > 0 "
                    + " Group by BillCode, BillNo, TaxType, TaxRate "
                    + " )GST2 GROUP BY BillCode, BillNo "
                    + " ) as GST ON GST.BillCode = Sales.INVOICE_Code AND GST.BillNo = Sales.INVOICE_No "+ strOuterWhereQry + " ORDER BY Sales.BillDate ";
                    break;

                case "PURCHASE RETURN":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((PRTN.PurchasePartyID+' '+SM.Name),PRTN.PurchasePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strSubQuery += " and (PRTN.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and PRTN.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and PRTN.BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (PRTN.BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";
                    if (rdoGSTRegular.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY PRTN.DATE) "
                                + ",PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name) "
                                + ", PARTY_GST = ISNULL(SM.GSTNo, '') "
                                + ", Date = Convert(nVarchar(20), PRTN.DATE, 103) "
                                + ", Invoice_No = (PRTN.BillCode + ' ' + Convert(nvarchar(20), PRTN.BillNo)) "
                                + ", Taxable_Value = ROUND(GST.TaxableAmt, 2) "
                                + ", IGST_Amt = Cast(GST.IGSTAmt as Numeric(18, 4))  "
                                + ", CGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4))  "
                                + ", SGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4))  "
                                + ", Total_Invoice_Value = NetAmt "
                                + ", Taxable_Amt = (Isnull(PRTN.NetAmt, 0) - ISNULL(PRTN.TaxAmount, 0)) "
                                + ", [Difference_Amt] = Round((GST.TaxableAmt-(Isnull(PRTN.NetAmt,0)-ISNULL(PRTN.TaxAmount,0))),2)  "
                                + ", RoundOff_Amt = Cast(RoundOffSign+ Convert(Varchar(10),RoundOffAmt ) as Money) "
                                + "FROM PurchaseReturn PRTN "
                                + "LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = PRTN.PurchasePartyID "
                                + "LEFT JOIN( "
                                + "SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                                + "Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate  , SUM(TaxAmount) TaxAmount  , (SUM(ISNULL(GD.TaxAmount, 0))  * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt  "
                                + ", (CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt   "
                                + ", (CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                                + "from GSTDetails GD  WHere BillType IN('PURCHASERETURN') "
                                + "AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate   "
                                + ")GST2 GROUP BY BillCode, BillNo   "
                                + ") as GST ON GST.BillCode = PRTN.BillCode AND GST.BillNo = PRTN.BillNo WHERE 1=1 " + strSubQuery + " ORDER BY PRTN.Date ";
                    break;

                case "SALE RETURN":
                    if (txtSalesParty.Text != "")
                    {
                        strSubQuery = " and ISNULL((SRTN.SalePartyID+' '+SM.Name),SRTN.SalePartyID) = '" + txtSalesParty.Text + "' ";
                    }
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text)
                                , eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        eDate = eDate.AddDays(1);
                        strSubQuery += " and (SRTN.Date >='" + sDate.ToString("MM/dd/yyyy") + "' and SRTN.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    }
                    if (txtBillCode.Text != "")
                        strSubQuery += " and SRTN.BillCode='" + txtBillCode.Text + "' ";

                    if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                        strSubQuery += " and (SRTN.BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";

                    if (rdoGSTRegular.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') != '' ";
                    else if (rdoGSTUnAutho.Checked)
                        strSubQuery = " AND ISNULL(SM.GSTNo,'') = '' ";

                    strQuery = " SELECT S_No = ROW_NUMBER() Over(ORDER BY SRTN.DATE) "
                                + ",PARTY_NAME = (ISNULL(SM.AreaCode, '') + ISNULL(SM.AccountNo, '') + ' ' + SM.Name) "
                                + ", PARTY_GST = ISNULL(SM.GSTNo, '') "
                                + ", Date = Convert(nVarchar(20), SRTN.DATE, 103) "
                                + ", Invoice_No = (SRTN.BillCode + ' ' + Convert(nvarchar(20), SRTN.BillNo)) "
                                + ", Taxable_Value = ROUND(GST.TaxableAmt, 2) "
                                + ", IGST_Amt = Cast(GST.IGSTAmt as Numeric(18, 4))  "
                                + ", CGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4))  "
                                + ", SGST_Amt = Cast(GST.CGSTAmt as Numeric(18, 4))  "
                                + ", Total_Invoice_Value = NetAmt "
                                + ", Taxable_Amt = (Isnull(SRTN.NetAmt, 0) - ISNULL(SRTN.TaxAmount, 0)) "
                                + ", [Difference_Amt] = Round((GST.TaxableAmt-(Isnull(SRTN.NetAmt,0)-ISNULL(SRTN.TaxAmount,0))),2)  "
                                + ", RoundOff_Amt = Cast(RoundOffSign+ Convert(Varchar(10),RoundOffAmt ) as Money) "
                                + "FROM SaleReturn SRTN "
                                + "LEFT JOIN SupplierMaster SM ON(SM.AreaCode + SM.AccountNo) = SRTN.SalePartyID "
                                + "LEFT JOIN( "
                                + "SELECT BillCode, BillNo, SUM(TaxAmount)TaxAmount, SUM(TaxableAmt)TaxableAmt, SUM(CGSTAmt)CGSTAmt, SUM(IGSTAmt)IGSTAmt FROM( "
                                + "Select BillCode, BillNo, SUM(GD.TAXRate) TaxRate  , SUM(TaxAmount) TaxAmount  , (SUM(ISNULL(GD.TaxAmount, 0))  * 100) / SUM(ISNULL(GD.TAXRate, 0)) TaxableAmt "
                                + ",(CASE WHEN GD.TaxType = 'LOCAL' then(SUM(ISNULL(GD.TaxAmount, 0)) / 2) else 0 end) CGSTAmt   "
                                + ",(CASE WHEN GD.TaxType = 'INTERSTATE' then SUM(ISNULL(GD.TaxAmount, 0)) else 0 end) IGSTAmt "
                                + "from GSTDetails GD  WHere BillType IN('SALERETURN') "
                                + "AND ISNULL(TaxRate, 0) > 0  Group by BillCode, BillNo, TaxType, TaxRate   "
                                + ")GST2 GROUP BY BillCode, BillNo   "
                                + ") as GST ON GST.BillCode = SRTN.BillCode AND GST.BillNo = SRTN.BillNo WHERE 1=1 "
                                + strSubQuery
                                + " ORDER BY SRTN.Date ";
                    break;
            }
            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = "";
                if (rdoDetail.Checked)
                    strQuery = CreateDetailQuery();
                else
                    strQuery = CreateSummaryQuery();

                if (strQuery != "")
                {
                    DataTable DT = DataBaseAccess.GetDataTableRecord(strQuery);
                    if (rdoDetail.Checked && !MainPage.strUserRole.Contains("SUPERADMIN"))
                    {
                        DT.Columns.RemoveAt(DT.Columns.Count - 1);
                        DT.Columns.RemoveAt(DT.Columns.Count - 1);
                        DT.Columns.RemoveAt(DT.Columns.Count - 1);
                    }
                    BindDataWithGrid(DT);
                    BindDataWithLabel(DT);
                }
            }
            catch
            { }
        }

        private void SetColumnStyle()
        {
            for (int i = 0; i < dgrdDetails.Columns.Count; i++)
            {
                try
                {
                    DataGridViewCellStyle cellStyle = dgrdDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdDetails.Columns[i];

                    string strAlign = "LEFT";
                    int _width = 120;
                    _column.Width = _width;

                    _column.SortMode = DataGridViewColumnSortMode.Automatic;
                    if (_column.Name.ToUpper().Contains("S_NO"))
                    {
                        strAlign = "MIDDLE";
                        _width = 50;
                    }
                    if (_column.Name.ToUpper().Contains("DATE"))
                        _width = 80;
                    if (_column.Name.ToUpper().Contains("PARTY_NAME"))
                        _width = 270;
                    if (_column.Name.ToUpper().Contains("PARTY_GST"))
                        _width = 150;
                    if (_column.Name.ToUpper().Contains("AMT") || _column.Name.ToUpper().Contains("VALUE"))
                    {
                        strAlign = "RIGHT";
                        cellStyle.Format = "N2";
                    }
                    if (_column.Name.ToUpper().Contains("VALUE"))
                        _width = 170;
                    if (_column.Name.ToUpper().Contains("ROUND"))
                        _width = 140;

                    if (_column.Name.ToUpper().Contains("INVOICE_NO"))
                    {
                        _width = 120;
                        cellStyle.ForeColor = Color.FromArgb(64, 64, 0);
                        cellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Underline);
                    }
                    else
                        cellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);

                    if (strAlign == "LEFT")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (strAlign == "MIDDLE")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgrdDetails.Columns[i].DefaultCellStyle = cellStyle;
                    dgrdDetails.Columns[i].HeaderText = (dgrdDetails.Columns[i].HeaderText).Replace("_", " ");
                    dgrdDetails.Columns[i].HeaderCell.Style.Font = new Font("Arial", 10F, System.Drawing.FontStyle.Bold);
                    dgrdDetails.Columns[i].Width = _width;

                }
                catch  { }
            }
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                dgrdDetails.DataSource = null;
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        DataView dataView = new DataView(table);
                        dgrdDetails.DataSource = dataView;
                        SetColumnStyle();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in  Sale Return Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindDataWithLabel(DataTable dt)
        {
            double dTaxableAmt = 0, dIGSTAmt = 0, dCGSTAmt = 0, dSGSTAmt = 0, dTotalAmt = 0, dRoundOff = 0, dTCSAmt = 0, dTaxFree = 0;
            try
            {
                if (dt.Columns.Contains("TAXABLE_VALUE"))
                    dTaxableAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TAXABLE_VALUE)", "ISNULL(TAXABLE_VALUE,0) <> 0"));
                if (dt.Columns.Contains("IGST_AMT"))
                    dIGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(IGST_AMT)", "ISNULL(IGST_AMT,0) <> 0"));
                if (dt.Columns.Contains("CGST_AMT"))
                    dCGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(CGST_AMT)", "ISNULL(CGST_AMT,0) <> 0"));
                if (dt.Columns.Contains("SGST_AMT"))
                    dSGSTAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(SGST_AMT)", "ISNULL(SGST_AMT,0) <> 0"));
                if (dt.Columns.Contains("TOTAL_INVOICE_VALUE"))
                    dTotalAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TOTAL_INVOICE_VALUE)", "ISNULL(TOTAL_INVOICE_VALUE,0) <> 0"));
                if (dt.Columns.Contains("RoundOff_Amt"))
                    dRoundOff = dba.ConvertObjectToDouble(dt.Compute("SUM(RoundOff_Amt)", "ISNULL(RoundOff_Amt,0) <> 0"));
                if (dt.Columns.Contains("TCS_Amt"))
                    dTCSAmt = dba.ConvertObjectToDouble(dt.Compute("SUM(TCS_Amt)", "ISNULL(TCS_Amt,0) <> 0"));
                if (dt.Columns.Contains("Tax_Free_Amt"))
                    dTaxFree = dba.ConvertObjectToDouble(dt.Compute("SUM(Tax_Free_Amt)", "ISNULL(Tax_Free_Amt,0) <> 0"));

                lblTaxableAmt.Text = (dTaxableAmt != 0) ? dTaxableAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
                lbliGST.Text = (dIGSTAmt != 0) ? dIGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
                lblCGST.Text = (dCGSTAmt != 0) ? dCGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
                lblSGST.Text = (dSGSTAmt != 0) ? dSGSTAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
                lblRoundOff.Text = (dRoundOff != 0) ? dRoundOff.ToString("N2", MainPage.indianCurancy) : "0.00";
                labelTCSAmt.Text = (dTCSAmt != 0) ? dTCSAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
                labelTaxFreeAmt.Text = (dTaxFree != 0) ? dTaxFree.ToString("N2", MainPage.indianCurancy) : "0.00";
                lblTotalInvValue.Text = (dTotalAmt != 0) ? dTotalAmt.ToString("N2", MainPage.indianCurancy) : "0.00";
            }
            catch { }
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                    switch (Mode)
                    {
                        case "PURCHASE":
                            objSearch = new SearchData("PURCHASEPARTY", "SEARCH PURCHASE PARTY", e.KeyCode);
                            break;
                        case "SALES":
                            objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                            break;
                        case "PURCHASE RETURN":
                            objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                            break;
                        case "SALE RETURN":
                            objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                            break;
                    }
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
        }

        private void SummaryRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                    switch (Mode)
                    {
                        case "PURCHASE":
                            objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
                            break;
                        case "SALES":
                            objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                            break;
                        case "PURCHASE RETURN":
                            objSearch = new SearchData("PURCHASERETURNCODE", "SEARCH PURCHASE RETURN CODE", e.KeyCode);
                            break;
                        case "SALE RETURN":
                            objSearch = new SearchData("PURCHASERETURNCODE", "SEARCH PURCHASE RETURN CODE", e.KeyCode);
                            break;
                    }
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportToExcel(DataGridView DGrv, string FileName = "", string Header = "")
        {
            try
            {
                if (DGrv.Rows.Count > 0)
                {
                    DGrv.SelectAll();
                    DataObject dataObj = DGrv.GetClipboardContent();
                    if (dataObj != null)
                        Clipboard.SetDataObject(dataObj);
                    DGrv.ClearSelection();

                    object misValue = System.Reflection.Missing.Value;
                    NewExcel.Application excelApp = new NewExcel.Application();
                    NewExcel.Workbook excelWorkBook = excelApp.Workbooks.Add(misValue);
                    NewExcel.Worksheet excelWorkSheet = (NewExcel.Worksheet)excelWorkBook.Worksheets.get_Item(1);

                    var saveFileDialog = new SaveFileDialog();
                    if (FileName != "")
                        saveFileDialog.FileName = FileName;
                    else
                        saveFileDialog.FileName = "Exported_Data";

                    saveFileDialog.DefaultExt = ".xls";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        NewExcel.Range CR = (NewExcel.Range)excelWorkSheet.Cells[1, 1];

                        CR.Select();
                        excelWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
                       
                        NewExcel.Range line = (NewExcel.Range)excelWorkSheet.Rows[1];
                        line.Insert();

                        string strHeader = "";
                        for (int j = 1; j < DGrv.Columns.Count + 1; j++)
                        {
                            strHeader = DGrv.Columns[j - 1].HeaderText;
                            excelApp.Cells[1, j] = dgrdDetails.Columns[j - 1].HeaderText;
                            excelApp.Cells[1, j].Font.Bold = true;
                        }
                        if (Header != "")
                        {
                            NewExcel.Range Newline = (NewExcel.Range)excelWorkSheet.Rows[1];
                            Newline.Insert();
                            NewExcel.Range range = excelWorkSheet.UsedRange;
                            string address = range.get_Address();
                            string[] cells = address.Split(new char[] { ':' });
                            string endCell = cells[1].Replace("$", "");

                            excelApp.Cells[1, 1] = Header;
                            excelApp.Cells[1, 1].Font.Bold = true;
                            excelWorkSheet.get_Range("A1:" + endCell.Substring(0, 1) + "1").Merge();
                        }
                        excelApp.Columns.AutoFit();
                        CR.Select();

                        excelWorkBook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    excelWorkBook.Close(true, misValue, misValue);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkBook);
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }
            }
            catch (Exception ex) { throw ex; }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                ExportToExcel(dgrdDetails, toTitleCase(Mode) + "_Summary", toTitleCase(Mode) + " Summary Report");
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExport.Enabled = true;
        }

        private void SummaryRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (_bSearchStatus)
            {
                SearchRecord();
            }
        }
        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
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
        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Invoice No")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            switch (Mode)
                            {
                                case "PURCHASE":
                                    dba.ShowTransactionBook("PURCHASE", strNumber[0], strNumber[1]);
                                    break;
                                case "SALES":
                                    dba.ShowTransactionBook("SALES", strNumber[0], strNumber[1]);
                                    break;
                                case "PURCHASE RETURN":
                                    dba.ShowTransactionBook("PURCHASE RETURN", strNumber[0], strNumber[1]);
                                    break;
                                case "SALE RETURN":
                                    dba.ShowTransactionBook("SALE RETURN", strNumber[0], strNumber[1]);
                                    break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Grid view in Show " + toTitleCase(Mode) + " Summary Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _rowIndex = 0;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["S_No"].Value = (_rowIndex + 1);
                    _rowIndex++;
                }
            }
            catch { }
        }

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }

        private void dgrdDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Invoice No")
                dgrdDetails.Cursor = Cursors.Hand;
            else
                dgrdDetails.Cursor = Cursors.Arrow;
        }
    }
}
