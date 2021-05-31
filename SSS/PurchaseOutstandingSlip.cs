using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class PurchaseOutstandingSlip : Form
    {
        DataBaseAccess dba;
        string _strSchemeDhara = "0";
        public PurchaseOutstandingSlip()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        public PurchaseOutstandingSlip(string strPartyName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtParty.Text = strPartyName;
            SearchRecord();
        }

        public PurchaseOutstandingSlip(bool mStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            if (mStatus)
            {
                btnSelectCompany.Enabled = true;              
                GetMultiQuarterName();
            }
        }

        private void PurchaseOutstandingSlip_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlRelatedParty.Visible)
                    pnlRelatedParty.Visible = false;
                else if (panelCompany.Visible)
                    panelCompany.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            if (txtParty.Text == "")
            {
                MessageBox.Show("Sorry ! Party name can't be blank ! ", "Party name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtParty.Focus();
            }
            else if ((chkDate.Checked || chkInvoiceDate.Checked) && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
            {
                MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDate.Focus();
            }
            else
                SearchRecord();
            panelCompany.Visible = false;
            btnGo.Enabled = true;
        }

        private void SearchRecord()
        {
            try
            {
                dgrdCash.Rows.Clear();
                dgrdPurchase.Rows.Clear();
                chkCashAll.Checked = chkPurchaseAll.Checked = true;
                if (btnSelectCompany.Enabled)
                {
                    GetMultiQuarterDetails();
                }
                else
                {
                    GetCurrentQuarterDetails();
                }               
            }
            catch
            {
            }
        }

        private string CreateQuery(ref string strDateQuery, ref string strPPartyID, ref string strIDateQuery)
        {
            string strQuery = "";
            if (txtParty.Text != "")
            {
                string[] strFullName = txtParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strPPartyID = strFullName[0].Trim();
                    strQuery += " and AccountID='" + strPPartyID + "' ";
                }
            }

            if ((chkDate.Checked || chkInvoiceDate.Checked) && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);

                if (chkDate.Checked)
                    strDateQuery = " and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + endDate.ToString("MM/dd/yyyy") + "' ";

                if (chkInvoiceDate.Checked)
                    strIDateQuery = "and InvoiceDate>='" + sDate.ToString("MM/dd/yyyy") + "' and InvoiceDate <'" + endDate.ToString("MM/dd/yyyy") + "'  ";

            }
            return strQuery;
        }

        public void GetCurrentQuarterDetails()
        {
            string strQuery = "", strDateQuery = "",strIDateQuery="",strPPartyID="", strSubQuery = CreateQuery(ref strDateQuery, ref strPPartyID,ref strIDateQuery);

            strQuery += " Select * from (Select GRSNo,(BillCode+' '+CAST(BillNo as varchar)) BillNo,SaleBillNo,(CONVERT(varchar,DATEADD(dd,ISNULL(GD.SupplierDays,0), BillDate),103)) BDate,(SalePartyID+' '+Name)SalesParty,CAST(Amount as Money) GrossAmt,(CAST(PACKING as Money)+CAST(FREIGHT AS Money)+CAST(TAX as Money)+CAST(Others as Money)+CAST(OtherPer as Money)) OtherAmt,-DisPer as _Discount,TaxAmount,CAST(NetAmt as Money)NetAmt,ISNULL(GR.SchemeName,'')SchemeName,ISNULL(GD.SupplierDays,0)SupplierDays,DATEADD(dd,ISNULL(GD.SupplierDays,0), BillDate)SDATE,PR.BillNo as SBILLNo,PcsType,Marketer,SSparty as NickName,PR.TCSAmt from PurchaseRecord PR OUTER APPLY (Select TOP 1 SchemeName,OfferName,GR.DisPer,GR.Pieces as PcsType,Marketer from GoodsReceive GR OUTER APPLY (Select Top 1 OB.SchemeName,OB.OfferName,Marketer from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo) OB Where GR.PurchasePartyID=PR.PurchasePartyID and (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=PR.GRSNo)GR OUTER APPLY (Select Top 1 _GD.SupplierDays from GraceDaysMaster _GD Where _GD.OfferName=GR.OfferName) GD  CROSS APPLY (Select Description from BalanceAmount Where AccountStatus='PURCHASE A/C' and Tick='False' and Description=(BillCode+' '+CAST(BillNo as varchar))  " + strSubQuery + strDateQuery + " ) BA   OUTER APPLY (Select Name,Other as SSparty from SupplierMaster  Where (AreaCode+AccountNo)=PR.SalePartyID)SM  Where PurchasePartyID='" + strPPartyID + "' " + strIDateQuery + "  "
                 + " UNION ALL Select InvoiceNo,(BillCode+' '+CAST(BillNo as varchar)) BillNo,InvoiceNo as SaleBillNo,(CONVERT(varchar,Date,103)) BDate,'' as SalesParty,CAST(GrossAmt as Money) GrossAmt,(CAST(PackingAmt as Money)+CAST((OtherSign + CAST(OtherAmt as varchar)) as Money)) OtherAmt,0 _Discount,TaxAmt,CAST(NetAmt as Money)NetAmt,ISNULL(SchemeName,'') SchemeName,0 as SupplierDays,Date as SDATE,PR.BillNo as SBILLNo,'LOOSE' as PcsType,Marketer,''as NickName,PR.TCSAmt from PurchaseBook PR OUTER APPLY (Select TOP 1 SchemeName,Marketer from PurchaseBookSecondary PBS OUTER APPLY (Select TOP 1 SchemeName,Marketer from OrderBooking WHere (OrderCode+' '+CAST(OrderNo as varchar))=PBS.PONumber)OB Where PR.BillCode=PBS.BillCode and PR.BillNo=PBS.BillNo) PBS CROSS APPLY (Select Description from BalanceAmount Where AccountStatus='PURCHASE A/C' and Tick='False' and Description=(BillCode+' '+CAST(BillNo as varchar)) " + strSubQuery + strDateQuery + " ) BA  Where PurchasePartyID='" + strPPartyID + "' "+strIDateQuery+" )_Purchase OUTER APPLY (Select TOP 1 ISNULL(SSD.Discount,0)Disc from Scheme_SupplierDetails SSD OUTER APPLY (Select Top 1 Other as PParty from SupplierMaster SM Where (SM.AreaCode+AccountNo)='" + strPPartyID + "')SM  Where SSD.SupplierName=SM.PParty and SSD.SchemeName=_Purchase.SchemeName) _SSD Order By SDATE,SBILLNo "
                 + " Select Convert(varchar,Date,103) BDate, AccountStatus,Description,Upper(Status)Status,Amount from BalanceAmount Where AccountStatus Not in ('PURCHASE A/C','SALES A/C') and Tick='False' and CAST(Amount as Money)>0 " + strSubQuery + " Order by Date ";
            
            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            if (ds.Tables.Count > 1)
            {
                BindPurchaseRecord(ds.Tables[0]);
                BindCashRecord(ds.Tables[1]);
            }
            dgrdCash.EndEdit();
            dgrdPurchase.EndEdit();
            CalculateTotalAmt();
        }

        private void BindPurchaseRecord(DataTable dt)
        {
            double dMaxSchemePer = 0, dPackingDhara = 0, dAddaAmt = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdPurchase.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                string strSchemeName = "", strOfferName = "";
                dPackingDhara = dba.ConvertObjectToDouble(_strSchemeDhara);
                double dSchemeDhara = 0, dSchemeDh = 0, dGrossAmt = 0, dOtherAmt = 0, dSchemeAmt = 0, dBSchemeDhara = 0,dTCSAmt=0;
                foreach (DataRow row in dt.Rows)
                {
                    dSchemeDh = dSchemeAmt = dAddaAmt = 0;
                    strSchemeName = Convert.ToString(row["SchemeName"]);
                    strOfferName = Convert.ToString(row["SupplierDays"]);
                    dGrossAmt = dba.ConvertObjectToDouble(row["GrossAmt"]);
                    dOtherAmt = dba.ConvertObjectToDouble(row["OtherAmt"]);
                    dTCSAmt = dba.ConvertObjectToDouble(row["TCSAmt"]);
                    dBSchemeDhara = dba.ConvertObjectToDouble(row["Disc"]);

                    dgrdPurchase.Rows[rowIndex].Cells["check"].Value = true;
                    dgrdPurchase.Rows[rowIndex].Cells["grsno"].Value = row["GRSNo"];
                    dgrdPurchase.Rows[rowIndex].Cells["billNo"].Value = row["BillNo"];
                    dgrdPurchase.Rows[rowIndex].Cells["saleBillNo"].Value = row["SaleBillNo"];
                    dgrdPurchase.Rows[rowIndex].Cells["date"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["BDate"]));
                    dgrdPurchase.Rows[rowIndex].Cells["grossAmt"].Value = dGrossAmt;// row["GrossAmt"];
                    dgrdPurchase.Rows[rowIndex].Cells["dhara"].Value = row["_Discount"];
                    dgrdPurchase.Rows[rowIndex].Cells["taxAmt"].Value = row["TaxAmount"];
                    dgrdPurchase.Rows[rowIndex].Cells["netAmt"].Value = row["NetAmt"];
                    dgrdPurchase.Rows[rowIndex].Cells["otherAmtColumn"].Value = dOtherAmt;// row["OtherAmt"];
                    dgrdPurchase.Rows[rowIndex].Cells["salesParty"].Value = Convert.ToString(row["SalesParty"]);
                    dgrdPurchase.Rows[rowIndex].Cells["nickName"].Value = Convert.ToString(row["NickName"]);
                    dgrdPurchase.Rows[rowIndex].Cells["tcsAmt"].Value = dTCSAmt.ToString("N2", MainPage.indianCurancy);

                    if (strSchemeName != "" && (dSchemeDhara > 0 || dBSchemeDhara > 0))
                    {
                        if ((txtParty.Text.Contains("DL312 ") || txtParty.Text.Contains("DL745 ")) && dba.ConvertObjectToDouble(row["_Discount"]) == -10)
                            dBSchemeDhara = 8;
                        else if ((txtParty.Text.Contains("DL7268 ")) && dba.ConvertObjectToDouble(row["_Discount"]) == -3)
                            dBSchemeDhara = 4;
                        else if ((txtParty.Text.Contains("DL169 ")) && dba.ConvertObjectToDouble(row["_Discount"]) == -3)
                            dBSchemeDhara = 4;
                        // dSchemeAmt = ((dGrossAmt - dOtherAmt) * dSchemeDhara / 100);
                        if (dBSchemeDhara > 0)
                            dMaxSchemePer = dSchemeDh = dBSchemeDhara;
                        else
                            dSchemeDh = dSchemeDhara;
                        dSchemeAmt = (dGrossAmt * dSchemeDh / 100);

                    }

                    if (dPackingDhara > 0)
                    {
                        if (Convert.ToString(row["Marketer"]).Contains("ADDA") && Convert.ToString(row["PcsType"]).ToUpper().Contains("LOOSE"))
                            dAddaAmt = (dGrossAmt * dPackingDhara / 100);
                    }

                    dgrdPurchase.Rows[rowIndex].Cells["schemeDhara"].Value = dSchemeDh;
                    dgrdPurchase.Rows[rowIndex].Cells["schemeAmt"].Value = dSchemeAmt;
                    dgrdPurchase.Rows[rowIndex].Cells["sssAddaAmt"].Value = dAddaAmt;

                    if (strSchemeName != "" && strOfferName != "" && strOfferName != "0")
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    else if (strSchemeName != "")
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                    else if (strOfferName != "" && strOfferName != "0")
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Thistle;

                    if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;
                    else if (strSchemeName.Contains("FAIR"))
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;


                    //if(dSchemeAmt>0 && dAddaAmt>0)
                    //    dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.PeachPuff;
                    //else if(dSchemeAmt>0)
                    //    dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                    //else if (dAddaAmt > 0)
                    //    dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                    rowIndex++;
                }
            }

            if (dMaxSchemePer > 0)
                txtSchemeDhara.Text = dMaxSchemePer.ToString("0.00");
            else
                txtSchemeDhara.Text = "0";

            txtAddaPer.Text = dPackingDhara.ToString("0.00");

        }

        private void BindCashRecord(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                dgrdCash.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                double dAmt = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    dgrdCash.Rows[rowIndex].Cells["chk"].Value = true;
                    dgrdCash.Rows[rowIndex].Cells["account"].Value = row["AccountStatus"];
                    dgrdCash.Rows[rowIndex].Cells["desc"].Value = row["Description"];
                    dgrdCash.Rows[rowIndex].Cells["cashDate"].Value = dba.ConvertDateInExactFormat(Convert.ToString(row["BDate"]));// row["BDate"];
                    if (Convert.ToString(row["Status"]) == "DEBIT")
                        dgrdCash.Rows[rowIndex].Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    else
                        dgrdCash.Rows[rowIndex].Cells["creditAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    rowIndex++;
                }
            }
        }

        private void CalculateTotalAmt()
        {
            double dTaxAmt = 0, dGrossAmt = 0, dNAmt = 0, dOAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dAmt = 0, dNetAmt = 0, dSIAmt = 0, dOtherAmt = 0, dSIPer = 0, dTSchemeAmt = 0, dAddaAmt = 0,dTCSAmt=0;
            foreach (DataGridViewRow row in dgrdPurchase.Rows)
            {
                if (Convert.ToBoolean(row.Cells["check"].EditedFormattedValue))
                {
                    dGrossAmt += Convert.ToDouble(row.Cells["grossAmt"].Value);
                    dNAmt += Convert.ToDouble(row.Cells["netAmt"].Value);
                    dOAmt += Convert.ToDouble(row.Cells["otherAmtColumn"].Value);
                    dTaxAmt += Convert.ToDouble(row.Cells["taxAmt"].Value);
                    dTSchemeAmt += dba.ConvertObjectToDouble(row.Cells["schemeAmt"].Value);
                    dAddaAmt += dba.ConvertObjectToDouble(row.Cells["sssAddaAmt"].Value);
                    dTCSAmt += dba.ConvertObjectToDouble(row.Cells["tcsAmt"].Value);
                }
            }
            foreach (DataGridViewRow row in dgrdCash.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chk"].EditedFormattedValue))
                {
                    if (Convert.ToString(row.Cells["amount"].Value) != "")
                        dDebitAmt += Convert.ToDouble(row.Cells["amount"].Value);
                    else
                        dCreditAmt += Convert.ToDouble(row.Cells["creditAmt"].Value);
                }
            }

            dSIPer = dba.ConvertObjectToDouble(txtSalePer.Text);
            dOtherAmt = dba.ConvertObjectToDouble(txtOther.Text);
            dSIAmt = (dSIPer * dGrossAmt) / 100;

            dOtherAmt = dba.ConvertObjectToDouble(txtOther.Text);
            if (txtSign.Text == "-")
                dOtherAmt = dOtherAmt * -1;
            dAmt = dDebitAmt - dCreditAmt;
            dNetAmt = ((dAmt - dNAmt) + dSIAmt) + dOtherAmt;

            dTSchemeAmt = Math.Round(dTSchemeAmt, 0);

            if (chkScheme.Checked)
                dNetAmt += dTSchemeAmt;
            if (chkAdda.Checked)
                dNetAmt += dAddaAmt;

            lblGrossAmt.Text = dGrossAmt.ToString("N2", MainPage.indianCurancy);
            lblNetPurchaseAmt.Text = dNAmt.ToString("N2", MainPage.indianCurancy);
            lblOtherAmt.Text = dOAmt.ToString("N2", MainPage.indianCurancy);
            txtSaleIncentive.Text = dSIAmt.ToString("N2", MainPage.indianCurancy);
            txtSchemeAmt.Text = dTSchemeAmt.ToString("N2", MainPage.indianCurancy);
            lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            txtAddaAmt.Text = dAddaAmt.ToString("N2", MainPage.indianCurancy);
            lblTCSAmt.Text= dTCSAmt.ToString("N2", MainPage.indianCurancy);

            if (dAmt > 0)
                lblCashAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else if (dAmt < 0)
                lblCashAmt.Text = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            else
                lblCashAmt.Text = "0.00";

            if (dNetAmt >= 0)
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else
                lblNetAmt.Text = Math.Abs(dNetAmt).ToString("N2", MainPage.indianCurancy) + " Cr";

            if (dAddaAmt > 0)
                dgrdPurchase.Columns["sssAddaAmt"].Visible = true;
            else
                dgrdPurchase.Columns["sssAddaAmt"].Visible = false;

            if (dTSchemeAmt > 0)
                dgrdPurchase.Columns["schemeDhara"].Visible = dgrdPurchase.Columns["schemeAmt"].Visible = true;
            else
                dgrdPurchase.Columns["schemeDhara"].Visible = dgrdPurchase.Columns["schemeAmt"].Visible = false;
        }

        private void CalculateOnlyAmount()
        {
            double dCashAmt = 0, dNAmt = 0, dNetAmt = 0, dOtherAmt = 0, dSIAmt=0, dTSchemeAmt=0,dAddaAmt=0;
            string strAmt = "";
            if (lblCashAmt.Text.Contains(" Dr"))
            {
                strAmt = lblCashAmt.Text.Replace(" Dr", "");
                dCashAmt = dba.ConvertObjectToDouble(strAmt);
            }
            else if (lblCashAmt.Text.Contains(" Cr"))
            {
                strAmt = lblCashAmt.Text.Replace(" Cr", "");
                dCashAmt = dba.ConvertObjectToDouble(strAmt)*-1;
            }
            dNAmt = dba.ConvertObjectToDouble(lblNetPurchaseAmt.Text);

            dOtherAmt = dba.ConvertObjectToDouble(txtOther.Text);
            dSIAmt = dba.ConvertObjectToDouble(txtSaleIncentive.Text);
            dTSchemeAmt = dba.ConvertObjectToDouble(txtSchemeAmt.Text);
            dAddaAmt=dba.ConvertObjectToDouble(txtAddaAmt.Text);

            if (txtSign.Text == "-")
                dOtherAmt = dOtherAmt * -1;
            
            dNetAmt = ((dCashAmt - dNAmt) + dSIAmt) + dOtherAmt;

            if (chkScheme.Checked)
                dNetAmt += dTSchemeAmt;
            if (chkAdda.Checked)
                dNetAmt += dAddaAmt;

            if (dNetAmt >= 0)
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else
                lblNetAmt.Text = Math.Abs(dNetAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
        }

        #region Multi Company

        private void GetMultiQuarterName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";
                dgrdCompany.Rows.Clear();
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    int rowIndex = 0;
                    string[] sFolder = Directory.GetDirectories(strPath);
                    DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                    foreach (string folderName in sFolder)
                    {
                        string[] strFile = Directory.GetFiles(folderName, "*.syber");
                        if (strFile.Length > 0)
                        {
                            FileInfo objFile = new FileInfo(folderName);
                            DataTable dt = dba.GetMultiCompanyNameAndFinDate(objFile.Name);
                            if (dt.Rows.Count > 0)
                            {
                                dgrdCompany.Rows.Add();
                                sDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["SDate"]));
                                eDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["EDate"]));
                                dgrdCompany.Rows[rowIndex].Cells["companyCheck"].Value = (Boolean)true;
                                dgrdCompany.Rows[rowIndex].Cells["code"].Value = dt.Rows[0]["CCode"];
                                dgrdCompany.Rows[rowIndex].Cells["companyName"].Value = dt.Rows[0]["CompanyName"];
                                dgrdCompany.Rows[rowIndex].Cells["sTextDate"].Value = dt.Rows[0]["SDate"];
                                dgrdCompany.Rows[rowIndex].Cells["eTextDate"].Value = dt.Rows[0]["EDate"];
                                dgrdCompany.Rows[rowIndex].Cells["startDate"].Value = sDate;
                                dgrdCompany.Rows[rowIndex].Cells["endDate"].Value = eDate;
                                if (MainPage.multiQSDate > sDate)
                                    MainPage.multiQSDate = sDate;
                                if (MainPage.multiQEDate < eDate)
                                    MainPage.multiQEDate = eDate;
                                rowIndex++;
                            }
                        }
                    }
                }
                dgrdCompany.Sort(dgrdCompany.Columns["startDate"], ListSortDirection.Ascending);
                chkDate.Checked = true;
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

                MainPage.con.Close();
                MainPage.OpenConnection();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in MultiLedger Merging ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnSelectCompany_Click(object sender, EventArgs e)
        {
            if (!panelCompany.Visible)
            {
                panelCompany.Visible = true;
                dgrdCompany.Focus();
            }
            else
            {
                panelCompany.Visible = false;
            }
        }

        private void dgrdCompany_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                GetSelectedQuarterDate();
            }
        }

        private void GetSelectedQuarterDate()
        {
            try
            {
                DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                MainPage.multiQSDate = DateTime.Today;
                MainPage.multiQEDate = DateTime.Today;
                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["sTextDate"].Value));
                        eDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["eTextDate"].Value));
                        if (rowCount == 0)
                        {
                            MainPage.multiQSDate = sDate;
                            MainPage.multiQEDate = eDate;
                        }
                        else
                        {
                            if (MainPage.multiQSDate > sDate)
                                MainPage.multiQSDate = sDate;
                            if (MainPage.multiQEDate < eDate)
                                MainPage.multiQEDate = eDate;
                        }
                        rowCount++;
                    }
                }
                txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        public void GetMultiQuarterDetails()
        {
            ClearRecord();
            try
            {
                string strPurchaseQuery = "", strCashQuery = "", strQuery = "", strDateQuery = "", strPPartyID = "", strIDateQuery="", strSubQuery = CreateQuery(ref strDateQuery, ref strPPartyID,ref strIDateQuery), strOpeningQuery = "", strCompanyCode = "";
                strOpeningQuery = " Select Convert(varchar,Date,103) BDate, AccountStatus,Description,Upper(Status)Status,Amount from BalanceAmount Where AccountStatus Not in ('PURCHASE A/C','SALES A/C') and Tick='False' and CAST(Amount as Money)>0 " + strSubQuery + " Order by Date ";

                DataTable dtPurchase = null, dtCash = null;
                strPurchaseQuery = "  Select * from (Select GRSNo,(BillCode+' '+CAST(BillNo as varchar)) BillNo,SaleBillNo,(CONVERT(varchar,DATEADD(dd,ISNULL(GD.SupplierDays,0), BillDate),103)) BDate,(SalePartyID+' '+Name)SalesParty,CAST(Amount as Money) GrossAmt,(CAST(PACKING as Money)+CAST(FREIGHT AS Money)+CAST(TAX as Money)+CAST(Others as Money)+CAST(OtherPer as Money)) OtherAmt,-DisPer as _Discount,TaxAmount,CAST(NetAmt as Money)NetAmt,ISNULL(GR.SchemeName,'')SchemeName,ISNULL(GD.SupplierDays,0)SupplierDays,DATEADD(dd,ISNULL(GD.SupplierDays,0), BillDate)SDATE,PR.BillNo as SBILLNo,PcsType,Marketer,SSparty as NickName,PR.TCSAmt from PurchaseRecord PR OUTER APPLY (Select TOP 1 SchemeName,OfferName,GR.DisPer,GR.Pieces as PcsType,Marketer from GoodsReceive GR OUTER APPLY (Select Top 1 OB.SchemeName,OB.OfferName,Marketer from OrderBooking OB Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo) OB Where GR.PurchasePartyID=PR.PurchasePartyID and (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=PR.GRSNo)GR OUTER APPLY (Select Top 1 _GD.SupplierDays from GraceDaysMaster _GD Where _GD.OfferName=GR.OfferName) GD  CROSS APPLY (Select Top 1 Description from BalanceAmount Where AccountStatus='PURCHASE A/C' and Tick='False' and Description=(BillCode+' '+CAST(BillNo as varchar))  " + strSubQuery + strDateQuery + " ) BA   OUTER APPLY (Select Top 1 Name,Other as SSparty from SupplierMaster  Where (AreaCode+AccountNo)=PR.SalePartyID)SM  Where PurchasePartyID='" + strPPartyID + "' " + strIDateQuery + "  "
                                   + " UNION ALL Select InvoiceNo,(BillCode+' '+CAST(BillNo as varchar)) BillNo,InvoiceNo as SaleBillNo,(CONVERT(varchar,Date,103)) BDate,'' as SalesParty,CAST(GrossAmt as Money) GrossAmt,(CAST(PackingAmt as Money)+CAST((OtherSign + CAST(OtherAmt as varchar)) as Money)) OtherAmt,0 _Discount,TaxAmt,CAST(NetAmt as Money)NetAmt,Isnull(SchemeName,'') AS SchemeName,0 as SupplierDays,Date as SDATE,PR.BillNo as SBILLNo,'LOOSE' as PcsType,Marketer,'' as NickName,PR.TCSAmt from PurchaseBook PR  OUTER APPLY (Select TOp 1 SchemeName,Marketer from PurchaseBookSecondary PBS OUTER APPLY (Select TOP 1 SchemeName,Marketer from OrderBooking WHere (OrderCode+' '+CAST(OrderNo as varchar))=PBS.PONumber )OB Where PR.BillCode=PBS.BillCode and PR.BillNo=PBS.BillNo) PBS CROSS APPLY (Select Top 1 Description from BalanceAmount Where AccountStatus='PURCHASE A/C' and Tick='False' and Description=(BillCode+' '+CAST(BillNo as varchar))  " + strSubQuery + strDateQuery + " ) BA  Where PurchasePartyID='" + strPPartyID + "' " + strIDateQuery + " )_Purchase OUTER APPLY (Select Top 1 ISNULL(SSD.Discount,0)Disc from Scheme_SupplierDetails SSD OUTER APPLY (Select Top 1 Other as PParty from SupplierMaster SM Where (SM.AreaCode+AccountNo)='" + strPPartyID + "')SM  Where SSD.SupplierName=SM.PParty and SSD.SchemeName=_Purchase.SchemeName) _SSD Order By SDATE,SBILLNo ";
                strCashQuery = " Select Convert(varchar,Date,103) BDate, AccountStatus,Description,Upper(Status)Status,Amount from BalanceAmount Where AccountStatus Not in ('PURCHASE A/C','SALES A/C') and Tick='False' and CAST(Amount as Money)>0  and AccountStatus !='OPENING' " + strSubQuery + " Order by Date ";


                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                        if (strCompanyCode != "")
                        {
                            DataTable dt = null;
                            strQuery = strPurchaseQuery;
                            if (rowCount == 0)
                                strQuery += strOpeningQuery;
                            else
                                strQuery += strCashQuery;

                            DataSet ds = dba.GetMultiQuarterDataSet(strQuery, strCompanyCode);
                            if (ds.Tables.Count > 1)
                            {
                                dt = ds.Tables[0];
                                if (dtPurchase == null)
                                    dtPurchase = dt;
                                else
                                    dtPurchase.Merge(dt, true);
                               // dt.Clear();
                                dt = ds.Tables[1];
                                if (dtCash == null)
                                    dtCash = dt;
                                else
                                    dtCash.Merge(dt, true);
                            }
                            rowCount++;
                        }
                    }
                }

                if (dtPurchase != null)
                    BindPurchaseRecord(dtPurchase);
                if (dtCash != null)
                    BindCashRecord(dtCash);
                
                dgrdCash.EndEdit();
                dgrdPurchase.EndEdit();
                CalculateTotalAmt();
            }
            catch(Exception Ex)
            {
                MessageBox.Show("Sorry ! " + Ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        private void ClearRecord()
        {
            dgrdCash.Rows.Clear();
            dgrdPurchase.Rows.Clear();
            txtSchemeDhara.Text = txtSalePer.Text = txtSaleIncentive.Text = txtOther.Text = lblNetAmt.Text = lblCashAmt.Text = lblGrossAmt.Text = lblNetPurchaseAmt.Text = lblOtherAmt.Text =lblTaxAmt.Text=lblTCSAmt.Text= "0.00";
            txtSign.Text = "+";
            chkScheme.Checked =chkAdda.Checked= true;
        }

        private void txtSalePer_TextChanged(object sender, EventArgs e)
        {
            double dAmt=0, dPer = dba.ConvertObjectToDouble(txtSalePer.Text), dGAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);
            dGAmt -= dba.ConvertObjectToDouble(lblOtherAmt.Text);

            dAmt = (dPer * dGAmt)/100;
            txtSaleIncentive.Text = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateOnlyAmount();
        }

        private void txtSalePer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtOther_TextChanged(object sender, EventArgs e)
        {
            CalculateOnlyAmount();
        }

        private void dgrdCash_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    CalculateTotalAmt();
                }
            }
            catch
            {
            }
        }

        private void dgrdCash_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdPurchase_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdPurchase_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    CalculateTotalAmt();
                }
                else if (e.ColumnIndex < 5)
                    ShowBillDetails(e.ColumnIndex);
            }
            catch
            {
            }
        }

        private void ShowBillDetails(int index)
        {
            string strValue = Convert.ToString(dgrdPurchase.CurrentCell.Value);
            if (strValue != "")
            {
                string[] strInvoice = strValue.Split(' ');
                if (strInvoice.Length > 1)
                {
                    if (index == 9)
                    {
                        GoodscumPurchase objGoods = new GoodscumPurchase(strInvoice[0], strInvoice[1]);
                        objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                        objGoods.Show();
                    }
                    else if (index == 2)
                    {
                        if (strInvoice[0].Contains("PTN"))
                        {
                            PurchaseBook_Trading objPurchase = new PurchaseBook_Trading(strInvoice[0], strInvoice[1]);
                            objPurchase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                            objPurchase.Show();
                        }
                        else
                        {
                            GoodscumPurchase objGoods = new GoodscumPurchase(strInvoice[0], strInvoice[1]);
                            objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                            objGoods.Show();
                        }
                    }
                    else if (index == 3)
                    {
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            dba.ShowSaleBookPrint(strInvoice[0], strInvoice[1],false, false);
                        }
                        else
                        {
                            SaleBook objSaleBook = new SaleBook(strInvoice[0], strInvoice[1]);
                            objSaleBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                            objSaleBook.ShowInTaskbar = true;
                            objSaleBook.Show();
                        }
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
            {              
                if (chkInvoiceDate.Checked)
                    chkInvoiceDate.Checked = false;
                else
                {
                    txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
                    txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                    txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
                }
                ClearRecord();
            }
            else
            {
                if(!chkInvoiceDate.Checked)
                {
                    txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
                    txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                    txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
                    ClearRecord();
                }              
            }
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                    if (strData != "")
                    {
                        txtParty.Text = strData;
                        ClearRecord();
                        GetSaleIncentive();
                        GetRelatedpartyDetails();
                    }
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            txtParty.Text = strData;
                            ClearRecord();
                            GetSaleIncentive();
                            GetRelatedpartyDetails();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void GetRelatedpartyDetails()
        {
            pnlRelatedParty.Visible = false;
            dgrdRelatedParty.Rows.Clear();

            if (txtParty.Text != "")
            {
                DataTable dt = dba.GetRelatedPartyDetails(txtParty.Text);
                if (dt.Rows.Count > 0)
                {
                    dgrdRelatedParty.Rows.Add(dt.Rows.Count);
                    int _index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdRelatedParty.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                        dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["Name"];
                        _index++;
                    }
                }
            }

            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
        }

        private void GetSaleIncentive()
        {
            txtSalePer.Text = _strSchemeDhara= "0.00";
            string strQuery = "Select SaleIncentive,CDDays from SupplierMaster Where (Areacode+CAST(AccountNo as varchar)+' '+Name)='" + txtParty.Text + "' ";
            DataTable dt = dba.GetDataTable(strQuery);
            if (dt.Rows.Count > 0)
            {
                double dValue = dba.ConvertObjectToDouble(dt.Rows[0]["SaleIncentive"]);//, dScheme = dba.ConvertObjectToDouble(dt.Rows[0]["CDDays"]);
                txtSalePer.Text = dValue.ToString("0.00");
                _strSchemeDhara = Convert.ToString(dt.Rows[0]["CDDays"]);// dScheme.ToString("0.00");
            }
        }

        private DataTable CreatePurchaseDataTable()
        {
            DataTable myDataTable = new DataTable("Purchase");
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("GRSNo", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("SBillNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("GrossAmt", typeof(String));
                myDataTable.Columns.Add("Dhara", typeof(String));
                myDataTable.Columns.Add("NetAmt", typeof(String));
                myDataTable.Columns.Add("SalesParty", typeof(String));
                myDataTable.Columns.Add("TotalGross", typeof(String));
                myDataTable.Columns.Add("TotalNet", typeof(String));
                myDataTable.Columns.Add("CashAmt", typeof(String));
                myDataTable.Columns.Add("GRStatus", typeof(String));
                myDataTable.Columns.Add("GRAmt", typeof(String));
                myDataTable.Columns.Add("FinalAmt", typeof(String));
                myDataTable.Columns.Add("Status", typeof(String));
                myDataTable.Columns.Add("OtherAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                string strSaleBillNo = "";
                int _startIndex = 0;
                foreach (DataGridViewRow dr in dgrdPurchase.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["check"].Value))
                    {
                        strSaleBillNo = Convert.ToString(dr.Cells["saleBillNo"].Value);

                        if (strSaleBillNo.Contains("/"))
                        {
                            _startIndex = strSaleBillNo.IndexOf('/') + 1;
                            strSaleBillNo = strSaleBillNo.Substring(_startIndex, strSaleBillNo.Length - _startIndex);

                        }
                        DataRow row = myDataTable.NewRow();

                        row["CompanyName"] = MainPage.strPrintComapanyName;
                        row["PartyName"] = "ACCOUNT NAME : " + txtParty.Text;
                        row["GRSNo"] = dba.ConvertObjectToDouble(dr.Cells["otherAmtColumn"].Value).ToString("N2", MainPage.indianCurancy);
                        row["BillNo"] = dr.Cells["billNo"].Value;
                        row["SBillNo"] = strSaleBillNo;
                        row["Date"] = Convert.ToDateTime(dr.Cells["date"].Value).ToString("dd/MM/yyyy");
                        row["GrossAmt"] = dba.ConvertObjectToDouble(dr.Cells["grossAmt"].Value).ToString("N2", MainPage.indianCurancy);
                        row["Dhara"] = Convert.ToDouble(dr.Cells["dhara"].Value).ToString("0");
                        row["GRAmt"] = dba.ConvertObjectToDouble(dr.Cells["taxAmt"].Value).ToString("N2",MainPage.indianCurancy);
                        row["NetAmt"] = dba.ConvertObjectToDouble(dr.Cells["netAmt"].Value).ToString("N2", MainPage.indianCurancy);
                        if (chkSaleParty.Checked)
                            row["SalesParty"] = dr.Cells["salesParty"].Value;
                        else
                            row["SalesParty"] = "-----------------------";
                        row["TotalGross"] = lblGrossAmt.Text;
                      
                        if (chkDate.Checked)
                            row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                        else
                            row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                        row["Status"] = "0";
                        //row["SBillNo"] = "";

                        row["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                        myDataTable.Rows.Add(row);
                    }
                }


                if (myDataTable.Rows.Count > 0)
                {
                    myDataTable.Rows[myDataTable.Rows.Count - 1]["Status"] = "2";
                    DataRow row1 = myDataTable.NewRow();
                    row1["Date"] = "------------";
                    row1["GrossAmt"] = "----------";
                    row1["NetAmt"] = "------------";
                    row1["GRSNo"] = "------------";
                    row1["GRAmt"] = "-----------";
                    row1["Status"] = "2";
                    row1["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    myDataTable.Rows.Add(row1);

                    DataRow row2 = myDataTable.NewRow();
                    row2["CompanyName"] = MainPage.strPrintComapanyName;
                    row2["Date"] = "Total : ";
                    row2["GrossAmt"] = lblGrossAmt.Text;
                    row2["GRSNo"] = lblOtherAmt.Text;
                    row2["GRAmt"] = lblTaxAmt.Text;
                    row2["NetAmt"] = lblNetPurchaseAmt.Text+" Cr";
                    row2["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    row2["SBillNo"] = "";
                    row2["Status"] = "2";
                    if (chkDate.Checked)
                        row2["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                    else
                        row2["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                    myDataTable.Rows.Add(row2);

                    row1 = myDataTable.NewRow();
                    row1["Date"] = "------------";
                    row1["GrossAmt"] = "----------";
                    row1["NetAmt"] = "------------";
                    row1["GRSNo"] = "------------";
                    row1["GRAmt"] = "-----------";
                    row1["Status"] = "2";
                    row1["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    if (chkDate.Checked)
                        row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                    else
                        row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                    myDataTable.Rows.Add(row1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.ToString (), "Error in Table", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
            return myDataTable;
        }

        private void CreateCashDataTable(DataTable myDataTable)
        {
            try
            {
                if (dgrdCash.Rows.Count > 0)
                {
                    AddCashSummaryRow(myDataTable);

                    foreach (DataGridViewRow dr in dgrdCash.Rows)
                    {
                        if (Convert.ToBoolean(dr.Cells["chk"].Value))
                        {
                            DataRow row = myDataTable.NewRow();
                            row["CompanyName"] = MainPage.strPrintComapanyName;
                            row["Date"] = Convert.ToDateTime(dr.Cells["cashDate"].Value).ToString("dd/MM/yyyy"); //dr.Cells["cashDate"].Value;
                            row["CashAmt"] = dr.Cells["desc"].Value;

                            if (Convert.ToString(dr.Cells["amount"].Value)!="")
                            {
                                row["GrossAmt"] = dr.Cells["amount"].Value + " Dr";
                            }
                            else if (Convert.ToString(dr.Cells["creditAMt"].Value) != "")
                            {
                                row["GrossAmt"] = dr.Cells["creditAMt"].Value + " Cr";
                            }
                            row["SBillNo"] = "";
                            row["Status"] = "0";
                            row["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                            if (chkDate.Checked)
                                row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                            else
                                row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                            myDataTable.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.ToString(), "Error in Cash", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddSalesIncentive(DataTable dt)
        {
            try
            {
                if (txtSaleIncentive.Text != "0.00" && txtSaleIncentive.Text != "")
                {
                    //DataRow row2 = dt.NewRow();
                    //dt.Rows.Add(row2);

                    DataRow row1 = dt.NewRow();
                    row1["GRStatus"] = "SALE INCENTIVE ("+txtSalePer.Text+"%) :";
                   // row1["GrossAmt"] = ;
                    row1["NetAmt"] = txtSaleIncentive.Text + " Dr";

                    row1["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    if (chkDate.Checked)
                        row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                    else
                        row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                    row1["SBillNo"] = "";
                    row1["Status"] = "0";
                    // row1["TotalNet"] = MainPage.strLoginName;
                    dt.Rows.Add(row1);

                }

                if (chkScheme.Checked)
                {
                    double dSchemeAmt = dba.ConvertObjectToDouble(txtSchemeAmt.Text);
                    if (dSchemeAmt > 0)
                    {
                        DataRow row1 = dt.NewRow();
                        row1["GRStatus"] = "SCHEME DISCOUNT (" + txtSchemeDhara.Text + "%) :";
                        row1["NetAmt"] = txtSchemeAmt.Text + " Dr";
                        row1["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                        if (chkDate.Checked)
                            row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                        else
                            row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                        row1["SBillNo"] = "";
                        row1["Status"] = "0";
                        dt.Rows.Add(row1);
                    }
                }

                if (chkAdda.Checked)
                {
                    double dAddaAmt = dba.ConvertObjectToDouble(txtAddaAmt.Text);
                    if (dAddaAmt > 0)
                    {
                        DataRow row1 = dt.NewRow();
                        row1["GRStatus"] = "ADDA DISCOUNT (" + txtAddaPer.Text + "%) :";
                        row1["NetAmt"] = txtAddaAmt.Text + " Dr";
                        row1["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                        if (chkDate.Checked)
                            row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                        else
                            row1["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                        row1["SBillNo"] = "";
                        row1["Status"] = "0";
                        dt.Rows.Add(row1);
                    }
                }

                double dOtherAmount = dba.ConvertObjectToDouble(txtOther.Text);
                if (dOtherAmount > 0)
                {
                    DataRow row3 = dt.NewRow();
                    row3["SBillNo"] = "";
                    row3["Status"] = "0";
                    if (txtSign.Text == "-")
                    {
                        row3["GRStatus"] = "ADD OTHER AMT :";
                        row3["NetAmt"] = dOtherAmount.ToString("N0", MainPage.indianCurancy) + " Cr";
                    }
                    else
                    {
                        row3["GRStatus"] = "LESS OTHER AMT :";
                        row3["NetAmt"] = dOtherAmount.ToString("N0", MainPage.indianCurancy) + " Dr";
                    }

                    if (chkDate.Checked)
                        row3["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                    else
                        row3["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");

                    row3["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    dt.Rows.Add(row3);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error in SI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddCashSummaryRow(DataTable dt)
        {
            try
            {
                if (lblCashAmt.Text.Contains("Cr") || lblCashAmt.Text.Contains("Dr"))
                {
                    DataRow row = dt.NewRow();

                    //row["Date"] = "----------------";
                    //row["GrossAmt"] = "----------------";
                    //row["NetAmt"] = "------------------";
                    //row["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    //dt.Rows.Add(row);

                    row = dt.NewRow();
                    row["SBillNo"] = "";
                    if (lblCashAmt.Text.Contains("Cr"))
                    {
                        row["GRStatus"] = "ADD PART AMT :";
                        row["NetAmt"] = lblCashAmt.Text;
                    }
                    else if (lblCashAmt.Text.Contains("Dr"))
                    {
                        row["GRStatus"] = "LESS PART AMT :";
                        row["NetAmt"] = lblCashAmt.Text;
                    }
                    if (chkDate.Checked)
                        row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + txtFromDate.Text + " To " + txtToDate.Text;
                    else
                        row["OtherAmount"] = "PAYMENT SLIP ON DATE  " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " To " + MainPage.endFinDate.ToString("dd/MM/yyyy");
                    row["Status"] = "1";
                    row["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    dt.Rows.Add(row);

                    //DataRow row1 = dt.NewRow();
                    //dt.Rows.Add(row1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error in Summary", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = CreatePurchaseDataTable();
            AddSalesIncentive(dt);
            CreateCashDataTable(dt);         
            if (dt.Rows.Count > 0)
            {
                dt.Rows[0]["PartyName"] = "ACCOUNT NAME : " + txtParty.Text;
                dt.Rows[dt.Rows.Count - 1]["FinalAmt"] = "NET AMOUNT : "+ lblNetAmt.Text;
                dt.Rows[dt.Rows.Count - 1]["Status"] = "1";
                dt.Rows[dt.Rows.Count - 1]["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
            }
            return dt;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdCash.Rows.Count != 0 || dgrdPurchase.Rows.Count != 0)
                {
                    Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Purchase Outstanding Slip");
                    CalculateTotalAmt();                  
                    btnPreview.Enabled = false;
                   
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PurchaseSlipReport objReport = new Reporting.PurchaseSlipReport();
                        objReport.SetDataSource(dt);
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdCash.Rows.Count != 0 || dgrdPurchase.Rows.Count != 0)
                {                  
                    CalculateTotalAmt();
                    btnPrint.Enabled = false;

                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PurchaseSlipReport objReport = new Reporting.PurchaseSlipReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void PurchaseOutstandingSlip_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.strSoftwareType!="AGENT")
                {
                    chkAdda.Visible = txtAddaPer.Visible = txtAddaAmt.Visible = chkScheme.Visible = txtSchemeDhara.Visible = txtSchemeAmt.Visible =btnAddPaymentRequest.Visible=btnViewAllRequest.Visible= false;
                }
                if (MainPage.mymainObject.bPurchaseSlip)
                {
                    if (!btnSelectCompany.Enabled)
                    {
                        MainPage.multiQSDate = MainPage.startFinDate;
                        MainPage.multiQEDate = MainPage.endFinDate;
                    }
                    btnAddPaymentRequest.Enabled = MainPage.mymainObject.bAddPaymentRequest;

                    txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                    txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
                    dba.EnableCopyOnClipBoard(dgrdPurchase);
                    dba.EnableCopyOnClipBoard(dgrdCash);
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }
           
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            CalculateOnlyAmount();
        }

        private void txtOther_Enter(object sender, EventArgs e)
        {
            if (txtOther.Text == "0.00")
                txtOther.Clear();
        }

        private void txtSalePer_Enter(object sender, EventArgs e)
        {
            if (txtSalePer.Text == "0.00")
                txtSalePer.Clear();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdCash.Rows.Count != 0 || dgrdPurchase.Rows.Count != 0)
                {
                    CalculateTotalAmt();
                    btnExport.Enabled = false;

                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PurchaseSlipReport objReport = new Reporting.PurchaseSlipReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.myPreview.ExportReport();

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private string CreatePDFFile(string strName)
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Purchase OutstandingSlip";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                if (strName == "")
                    strFileName = strPath + "\\PurchaseSlip.pdf";
                else
                    strFileName = strPath + "\\" + strName + ".pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.PurchaseSlipReport objRegister = new Reporting.PurchaseSlipReport();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                    objRegister.Close();
                    objRegister.Dispose();
                }
                else
                    strFileName = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                strFileName = "";
            }
            return strFileName;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (txtParty.Text != "")
                {
                    string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    //if (strEmailID != "")
                    //{
                    strPath = CreatePDFFile("");
                    if (strPath != "")
                    {
                        strSubject = "PURCHASE OUTSTANDING SLIP FROM " + MainPage.strCompanyName;
                        strBody = "We are sending Purchase Outstanding Slip , which is Attached with this mail, Please Find it.";
                        SendingEmailPage objEmail = new SendingEmailPage(true, txtParty.Text, "", strSubject, strBody, strPath, "", "PURCHASE OUTSTANDING SLIP");
                        objEmail.ShowDialog();
                    }
                    //}
                }
                else
                {
                    MessageBox.Show("Sorry ! Party Name can't be blank ", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParty.Focus();
                }
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }

        private void dgrdPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdPurchase.CurrentRow.Index;
                    if (dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdPurchase.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    int columnIndex = dgrdPurchase.CurrentCell.ColumnIndex, rowIndex = dgrdPurchase.CurrentRow.Index;

                    DateTime date = Convert.ToDateTime(dgrdPurchase.Rows[rowIndex].Cells["date"].Value);
                    if (date >= MainPage.startFinDate && date < MainPage.endFinDate)
                    {
                        if (columnIndex > 0 & columnIndex < 4)
                            ShowBillDetails(columnIndex);
                    }
                }
            }
            catch
            {
            }
        }

        private void chkCashAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdCash.Rows)
                {
                    row.Cells["chk"].Value = chkCashAll.Checked;
                }
                CalculateTotalAmt();
            }
            catch
            {
            }
        }

        private void chkPurchaseAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdPurchase.Rows)
                {
                    row.Cells["check"].Value = chkPurchaseAll.Checked;
                }
                CalculateTotalAmt();
            }
            catch
            {
            }
        }

        private void dgrdCash_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkCashAll.Visible = false;
                    else
                        chkCashAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void dgrdPurchase_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkPurchaseAll.Visible = false;
                    else
                        chkPurchaseAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void btnAddPaymentRequest_Click(object sender, EventArgs e)
        {
            btnAddPaymentRequest.Enabled = false;
            AddToRequest();
            btnAddPaymentRequest.Enabled = true;
        }

        private void GetAmountSeperate(string strAmt, ref double dAmt, ref string strStatus)
        {
            string[] strAll = strAmt.Trim().Split(' ');

            dAmt = dba.ConvertObjectToDouble(strAll[0].Trim());
            if (strAll.Length > 1)
            {
                strStatus = strAll[strAll.Length - 1];
            }
        }

        private void AddToRequest()
        {
            try
            {
                if (lblNetAmt.Text.Contains("Cr"))
                {
                    double dNAmt = dba.ConvertObjectToDouble(lblNetAmt.Text.Replace(" Dr", "").Replace(" Cr", ""));
                    if (txtParty.Text != "" && dNAmt > 0)
                    {
                        string[] strFullName = txtParty.Text.Split(' ');
                        if (strFullName.Length > 0)
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to add in payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strAccountID = strFullName[0].Trim(), strPath = "", strQuery = "", strPartyName = "", strFileName = "", strBranchCode = "" ;
                                strBranchCode= System.Text.RegularExpressions.Regex.Replace(strAccountID, @"[\d-]", string.Empty);
                                
                                //if (strAccountID.StartsWith(MainPage.strBranchCode))
                                //{
                                if (CheckPartyPendingExistence(strAccountID))
                                    {
                                        strQuery = " Select TOP 1 * from SupplierBankDetails Where (AreaCode+CAST(AccountNo as nvarchar))='" + strAccountID + "' and VerifiedStatus=1 and ISNULL(BeniID,'') !='' Order by ID asc";
                                        DataTable _dt = dba.GetDataTable(strQuery);
                                    if (_dt.Rows.Count > 0)
                                    {

                                        strFileName = strAccountID + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                                        strPath = CreatePDFFile(strFileName);
                                        strPartyName = txtParty.Text.Replace(strAccountID + " ", "");
                                        double dCashAmt = 0, dPurchaseAmt = 0, dNetAmt = 0;
                                        string strCashStatus = "", strPurchaseStatus = "CR", strNetStatus = "";
                                        GetAmountSeperate(lblCashAmt.Text, ref dCashAmt, ref strCashStatus);
                                        dPurchaseAmt = dba.ConvertObjectToDouble(lblNetPurchaseAmt.Text);
                                        //GetAmountSeperate(lblNetPurchaseAmt.Text, ref dPurchaseAmt, ref strPurchaseStatus);
                                        GetAmountSeperate(lblNetAmt.Text, ref dNetAmt, ref strNetStatus);
                                        dNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);

                                        DataRow row = _dt.Rows[0];
                                        strQuery = "INSERT INTO [dbo].[PaymentRequest] ([BranchCode],[PartyID],[PartyName],[CashAmt],[CashStatus],[PurchaseAmt],[PurchaseStatus],[NetAmt],[NetStatus],[Date],[FilePath],[BankName],[BranchName],[AccountNumber],[AccountName],[IFSCCode],[CreatedBy],[RequestStatus],[InsertStatus],[UpdateStatus],[BeniID],[ReqPriority]) VALUES "
                                                 + " ('" + strBranchCode + "','" + strAccountID + "','" + strPartyName + "'," + dCashAmt + ",'" + strCashStatus + "'," + dPurchaseAmt + ",'" + strPurchaseStatus + "'," + dNetAmt + ",'" + strNetStatus + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + strPath + "','" + row["BankName"] + "','" + row["BranchName"] + "','" + row["BankAccountNo"] + "','" + row["BankAccountName"] + "','" + row["BankIFSCCode"] + "','" + MainPage.strLoginName + "','ADDED',1,0,'" + row["BeniID"] + "','REGULAR') ";

                                        int _count = dba.ExecuteMyQuery(strQuery);
                                        if (_count > 0)
                                        {
                                            MessageBox.Show("Thanks you ! Payment request added successfully ! ", "Added", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                        }
                                        else
                                            MessageBox.Show("Sorry ! Unable to add right now", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sorry ! Please enter bank detail in account master and verify that account with Beni ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    }
                                //}
                                //else { MessageBox.Show("Sorry ! This account is not belong to your branch, please request only your branch account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please enter party name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }               
                else
                {
                    MessageBox.Show("Sorry ! There is no outstading bill for pay.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private bool CheckPartyPendingExistence(string strPartyID)
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select PartyID from PaymentRequest Where PartyID='" + strPartyID + "' and RequestStatus='ADDED' ");
            if (Convert.ToString(objValue) == strPartyID)
            {
                DialogResult result = MessageBox.Show("You have already added this party, Are you still want to add in payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        private void btnViewAllRequest_Click(object sender, EventArgs e)
        {
            try
            {
                ViewPaymentRequest objRequest = new ViewPaymentRequest();               
                objRequest.MdiParent = MainPage.mymainObject;
                objRequest.txtStatus.Text = "ADDED";
                objRequest.Show();
            }
            catch { }
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                btnPartyName.Enabled = false;
                SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", Keys.Space);
                objSearch.ShowDialog();
                string strData = objSearch.strSelectedData;
                if (strData != "")
                {
                    txtParty.Text = strData;
                    ClearRecord();
                    GetSaleIncentive();
                    GetRelatedpartyDetails();
                }
            }
            catch { }
            btnPartyName.Enabled = true;
        }

        private void txtParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtParty.Text);
        }

        private void chkScheme_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalAmt();
        }

        private void txtParty_Leave(object sender, EventArgs e)
        {
            pnlRelatedParty.Visible = false;
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtParty.Text;
                    if (strParty != "")
                    {
                        txtParty.Text = strParty;
                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                    }
                    txtParty.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void txtParty_Enter(object sender, EventArgs e)
        {
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
            else
                pnlRelatedParty.Visible = false;
        }

        private void chkAdda_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalAmt();
        }

        private void dgrdCompany_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void chkInvoiceDate_CheckedChanged(object sender, EventArgs e)
        {        
            if (chkInvoiceDate.Checked)
            {
                if (chkDate.Checked)
                    chkDate.Checked = false;
                else
                {
                    txtFromDate.Enabled = txtToDate.Enabled = chkInvoiceDate.Checked;
                    txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                    txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");                   
                }
                ClearRecord();
            }
            else
            {
                if (!chkDate.Checked)
                {
                    txtFromDate.Enabled = txtToDate.Enabled = chkInvoiceDate.Checked;
                    txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                    txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
                    ClearRecord();
                }
            }
        }

        private void btnSIExport_Click(object sender, EventArgs e)
        {
            btnSIExport.Enabled = false;
            try
            {
                DataTable dt = GenerateSaleIncentiveData();
                ExportSI(dt);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnSIExport.Enabled = true;
        }

        private DataTable GenerateSaleIncentiveData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SupplierName", typeof(String));
            dt.Columns.Add("NickName", typeof(String));
            dt.Columns.Add("Description", typeof(String));
            dt.Columns.Add("SIAmt", typeof(Double));
            try
            {
                
                double dSI = dba.ConvertObjectToDouble(txtSalePer.Text),dGrossAmt=0,dSIAmt=0,dAmt=0,dOtherAmt=0;
                if (dSI > 0)
                {
                    string[] str = txtParty.Text.Split(' ');

                    object obj = DataBaseAccess.ExecuteMyScalar("Select Other from SupplierMaster Where GroupName='SUNDRY CREDITOR' and (AreaCode+AccountNo)='"+ str[0]+"' ");
                    string strNickName = "",strSupplierName=Convert.ToString(obj);
                    foreach (DataGridViewRow row in dgrdPurchase.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["check"].Value))
                        {
                            strNickName = Convert.ToString(row.Cells["nickName"].Value);
                            dGrossAmt = dba.ConvertObjectToDouble(row.Cells["grossAmt"].Value);
                            //dOtherAmt = dba.ConvertObjectToDouble(row.Cells["otherAmtColumn"].Value);
                            //dGrossAmt -= dOtherAmt;
                            if (dGrossAmt != 0)
                            {
                                dSIAmt = Math.Round(((dGrossAmt * dSI) / 100.00), 0);

                                DataRow[] _row = dt.Select("NickName='" + strNickName + "' ");
                                if (_row.Length > 0)
                                {
                                    dAmt = dba.ConvertObjectToDouble(_row[0]["SIAmt"]);
                                    _row[0]["SIAmt"] = (dAmt + dSIAmt);
                                }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["SupplierName"] = strSupplierName;
                                    dr["NickName"] = strNickName;
                                    dr["SIAmt"] = dSIAmt;
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { throw ex; }
            return dt;
        }

        private void ExportSI(DataTable dt)
        {           
            try
            {
                if (dt.Rows.Count > 0)
                {

                    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    object misValue = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;

                    //Create Excel Sheets
                    xlSheets = ExcelApp.Sheets;
                    xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
                                   Type.Missing, Type.Missing, Type.Missing);


                    ExcelApp.Cells[1, 1] = "SUPPLIER_NAME";
                    ExcelApp.Cells[1, 2] ="NICK_NAME";
                    ExcelApp.Cells[1, 3] = "DESCRIPTION";
                    ExcelApp.Cells[1, 4] = "SI_Amt";
                    ExcelApp.Cells[1, 1].Font.Bold = true;
                    ExcelApp.Cells[1, 2].Font.Bold = true;
                    ExcelApp.Cells[1, 3].Font.Bold = true;
                    ExcelApp.Cells[1, 4].Font.Bold = true;

                    int _rowIndex = 2;
                    foreach(DataRow row in dt.Rows)
                    {
                        
                        ExcelApp.Cells[_rowIndex, 1] = row["SupplierName"];
                        ExcelApp.Cells[_rowIndex, 2] = row["NickName"];
                        ExcelApp.Cells[_rowIndex, 3] = "SWEET";
                        ExcelApp.Cells[_rowIndex, 4] = row["SIAmt"];
                        _rowIndex++;
                    }
                    ExcelApp.Columns.AutoFit();

                    string[] strParty = txtParty.Text.Split(' ');
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "SI_Report_"+ strParty[0];
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Sale Incentive Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                        MessageBox.Show("Warning !! Export cancelled","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                    ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            { throw ex; }
            
        }
    }
}
