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
    public partial class HideRecords : Form
    {
        DataBaseAccess dba;
        string strDiscountName = "";
        public InterestStatement _objInterestStatement = null;
        public HideRecords()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            btnActive.Enabled = false;
            ShowSelectedEntry();
            btnActive.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void HideRecords_KeyDown(object sender, KeyEventArgs e)
        {
             try
            {
                if (e.KeyCode==Keys.Escape)
                {
                    this.Hide();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
             catch (Exception ex)
             {
                 string[] strReport = { "Exception occurred in Form's Key Down  in Hide Record", ex.Message };
                 dba.CreateErrorReports(strReport);
             }
        }      
    
        private void ShowSelectedEntry()
        {
            try
            {
                dgrdInterest.Sort(dgrdInterest.Columns["bDate"], ListSortDirection.Ascending);
                dgrdInterest.Columns["bDate"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                if (dgrdInterest.Rows.Count > 0)
                {
                    InterestStatement objInt = InterestStatement.objInterest;

                    DataGridViewRow[] rows = new DataGridViewRow[dgrdInterest.Rows.Count];
                    int rowIndex = objInt.dgrdInterest.Rows.Count;
                    for (int rIndex = 0; rIndex < dgrdInterest.Rows.Count; rIndex++)
                    {
                        DataGridViewRow dr = dgrdInterest.Rows[rIndex];
                        if (Convert.ToBoolean(dr.Cells["chk"].Value))
                        {
                            objInt.dgrdInterest.Rows.Add();
                            for (int colIndex = 0; colIndex < dgrdInterest.ColumnCount; colIndex++)
                            {
                                objInt.dgrdInterest.Rows[rowIndex].Cells[colIndex].Value = dr.Cells[colIndex].Value;                              
                            }
                            if (Convert.ToString(dr.Cells["onaccountStatus"].Value) == "COST")
                                objInt.dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;

                            rowIndex++;
                            dgrdInterest.Rows.RemoveAt(rIndex);
                            rIndex--;
                        }
                    }

                    objInt.dgrdInterest.Sort(objInt.dgrdInterest.Columns["bDate"], ListSortDirection.Ascending);
                    objInt.dgrdInterest.Columns["bDate"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    CalculateTotalAmount();
                    this.Hide();
                    InterestStatement.objInterest.Focus();
                    objInt.CalculateTotalAmount();    
                              
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Active Entries in Hide Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void HideRecords_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShowSelectedEntry();
        }

        double dNetSaleAmt = 0;
        public void CalculateTotalAmount()
        {
            double dDAmt = 0, dCAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dIntDAmt = 0, dIntCAmt = 0, dTotalAmt = 0, dIntAmt = 0;
            dNetSaleAmt =  0;
            lblTaxAmt.Text = "0.00";

            foreach (DataGridViewRow row in dgrdInterest.Rows)
            {
                if (row.DefaultCellStyle.BackColor.Name != "Gold")
                {
                    dDAmt = dba.ConvertObjectToDouble(row.Cells["amountDr"].Value);
                    if (dDAmt != 0)
                    {
                        dDebitAmt += dDAmt;
                        dTotalAmt += dDAmt;
                        if (Convert.ToString(row.Cells["particulars"].Value).ToUpper() == "SALES A/C")
                            dNetSaleAmt += dDAmt;
                    }
                    else
                    {
                        dCAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);
                        dCreditAmt += dCAmt;
                        dTotalAmt -= dCAmt;
                        //if (Convert.ToString(row.Cells["particulars"].Value).ToUpper() == "PURCHASE A/C")
                        //    dNetPurchaseAmt += dCAmt;
                    }

                    dIntDAmt += dIntAmt = dba.ConvertObjectToDouble(row.Cells["intDr"].Value);
                    if (dIntAmt == 0)
                        dIntCAmt += dba.ConvertObjectToDouble(row.Cells["intCr"].Value);
                    row.Cells["wsd"].Value = 0;
                    row.Cells["cd"].Value = 0;
                    if (dTotalAmt > 0)
                        row.Cells["bal"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                    else if (dTotalAmt < 0)
                        row.Cells["bal"].Value = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
                    else
                        row.Cells["bal"].Value = "0.00";
                }
            }

            lblDrAmt.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCrAmt.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
            lblIntCr.Text = dIntCAmt.ToString("N2", MainPage.indianCurancy);
            lblIntDr.Text = dIntDAmt.ToString("N2", MainPage.indianCurancy);

            if (dTotalAmt > 0)
                lblBalance.Text = lblGrossAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else if (dTotalAmt < 0)
                lblBalance.Text = lblGrossAmt.Text = Math.Abs(dTotalAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            else
                lblBalance.Text = lblGrossAmt.Text = "0";
            dIntAmt = dIntDAmt - dIntCAmt;
            if (dIntAmt > 0)
                lblInterest.Text = dIntAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
            else if (dIntAmt < 0)
                lblInterest.Text = Math.Abs(dIntAmt).ToString("N2", MainPage.indianCurancy) + " Cr";
            else
                lblInterest.Text = "0";

            CalculateAvgDays(dNetSaleAmt, dDebitAmt, dIntAmt);

        }

        private void CalculateAvgDays(double dSaleAmt, double dDebitAmt, double dIntAmt)
        {
            try
            {
                double dAmt = 0, avgDays = 0, dCDDays = 0, dGraceDays = 0, dDrRate = 0;
                dDrRate = dba.ConvertObjectToDouble(InterestStatement.objInterest.txtRateDr.Text);
                dAmt = (dSaleAmt * dDrRate) / 36000;
                //dCDDays = Convert.ToDouble(txtCDDays.Text);
                dGraceDays = Convert.ToDouble(InterestStatement.objInterest.txtGraceDays.Text);

                // if (dIntAmt>=0)
                avgDays = Math.Round((dIntAmt / dAmt), 2);
                //else
                //    avgDays = (dIntAmt / dAmt)*-1;

                if (dSaleAmt < 1)
                    lblAvgDays.Text = "0";
                else
                    lblAvgDays.Text = avgDays.ToString("N2", MainPage.indianCurancy);

                // Calculate Cash Discount
                bool bCDStatus = false;
                if (dSaleAmt > 0)
                    bCDStatus = CheckCDStatus(dGraceDays, avgDays);

                //if ((dCDDays - dGraceDays) >= avgDays && dSaleAmt > 0)                
                //    CalculateWSRAndCD(true);               

                CalculateWSRAndCD(bCDStatus);

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Calculation of Average Day in General Interest ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private bool CheckCDStatus(double dGraceDays, double avgDays)
        {
            bool bStatus = false;
            strDiscountName = "";
            if (InterestStatement.objInterest.dtDiscountDetails != null)
            {
                string strCategoryID = "2";
                if (InterestStatement.objInterest.strCategoryName == "WHOLESALER")
                    strCategoryID = "1";
                DataRow[] rows = InterestStatement.objInterest.dtDiscountDetails.Select("CategoryID=" + strCategoryID);
                if (rows.Length > 0)
                {
                    DataTable dtCash = rows.CopyToDataTable();
                    DataView dv = dtCash.DefaultView;
                    dv.Sort = "CDDays asc";
                    dtCash = dv.ToTable();
                    double dCDDays = 0;
                    foreach (DataRow row in dtCash.Rows)
                    {
                        dCDDays = dba.ConvertObjectToDouble(row["CDDays"]);
                        if ((dCDDays - dGraceDays) >= avgDays)
                        {
                            txtCD.Text = Convert.ToString(row["DiscountPer"]);
                            strDiscountName = Convert.ToString(row["DiscountName"]);
                            bStatus = true;
                            break;
                        }
                    }
                }
            }
            return bStatus;
        }

        private void CalculateWSRAndCD(bool cdStatus)
        {
            try
            {
                double dFinalAmt = 0, dWSRRate = 0, dCDRate = 0, dNetCDAmt = 0, dNetWSR = 0, dAmt = 0;

                if (cdStatus)
                {
                    dgrdInterest.Columns["cd"].Visible = true;
                    dgrdInterest.Columns["iDays"].Visible = dgrdInterest.Columns["intDr"].Visible = dgrdInterest.Columns["intcr"].Visible = false;
                }
                else
                {
                    dgrdInterest.Columns["cd"].Visible = false;
                    dgrdInterest.Columns["iDays"].Visible = dgrdInterest.Columns["intDr"].Visible = dgrdInterest.Columns["intcr"].Visible = true;
                }


                double dDebitAmt = 0, dCreditAmt = 0, dIntDAmt = 0, dIntCAmt = 0, dTaxAmt = 0, _dNetIntAmt = 0;
                dDebitAmt = dba.ConvertObjectToDouble(lblDrAmt.Text);
                dCreditAmt = dba.ConvertObjectToDouble(lblCrAmt.Text);
                dIntDAmt = dba.ConvertObjectToDouble(lblIntDr.Text);
                dIntCAmt = dba.ConvertObjectToDouble(lblIntCr.Text);
                if (InterestStatement.objInterest.strCategoryName == "WHOLESALER" || cdStatus)
                {
                    lblFinalBal.Text = "00";
                    dWSRRate = dba.ConvertObjectToDouble(InterestStatement.objInterest.txtWSR.Text);
                    dCDRate = dba.ConvertObjectToDouble(txtCD.Text);

                    string strAccount = "";
                    foreach (DataGridViewRow row in dgrdInterest.Rows)
                    {
                        strAccount = Convert.ToString(row.Cells["particulars"].Value);
                        if (strAccount == "SALES A/C" || strAccount == "PURCHASE A/C")
                        {
                            dFinalAmt = dba.ConvertObjectToDouble(row.Cells["final"].Value);
                            if (dFinalAmt != 0)
                            {
                                if (InterestStatement.objInterest.strCategoryName == "WHOLESALER")
                                {
                                    dAmt = (dFinalAmt * dWSRRate) / 100;
                                    dNetWSR += dAmt;
                                    row.Cells["wsd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }
                                if (cdStatus)
                                {
                                    dAmt = (dFinalAmt * dCDRate) / 100;
                                    dNetCDAmt += dAmt;
                                    row.Cells["cd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }
                            }
                        }
                        else if (strAccount == "SALE RETURN")
                        {
                            if (cdStatus)
                            {
                                dFinalAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);
                                dAmt = Math.Round(((dFinalAmt * dCDRate) / 100), 2) * -1;
                                dNetCDAmt += dAmt;
                                row.Cells["cd"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                            }
                        }
                    }
                }

                if (InterestStatement.objInterest.strCategoryName == "WHOLESALER" && dWSRRate != 0)
                    dgrdInterest.Columns["wsd"].Visible = true;
                else
                    dgrdInterest.Columns["wsd"].Visible = false;

                if (cdStatus)
                {
                    dCreditAmt += dNetWSR + dNetCDAmt;
                    lblIntCr.Text = lblIntDr.Text = lblInterest.Text = "0.00";
                }
                else
                {
                    _dNetIntAmt = dIntDAmt - dIntCAmt;
                    if (_dNetIntAmt > 0)
                    {
                        if (chkTax.Checked)
                            dTaxAmt = Math.Round((_dNetIntAmt * 18) / 100, 2);
                        else
                            dTaxAmt = (_dNetIntAmt - Math.Round((_dNetIntAmt / 118) * 100, 2));

                        lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy) + " Dr";

                        if (!chkTax.Checked)
                            dTaxAmt = 0;
                    }

                    dNetCDAmt = 0;
                    dDebitAmt += dIntDAmt + dTaxAmt;
                    dCreditAmt += dNetWSR + dIntCAmt;
                }

                dAmt = dDebitAmt - dCreditAmt;

                lblWSR.Text = dNetWSR.ToString("N2", MainPage.indianCurancy);
                if (dNetCDAmt >= 0)
                    lblCDiscount.Text = dNetCDAmt.ToString("N2", MainPage.indianCurancy) + " Cr";
                else if (dNetCDAmt < 0)
                    lblCDiscount.Text = Math.Abs(dNetCDAmt).ToString("N2", MainPage.indianCurancy) + " Dr";

                if (dAmt > 0)
                    lblFinalBal.Text = dAmt.ToString("N0", MainPage.indianCurancy) + " Dr";
                else if (dAmt < 0)
                    lblFinalBal.Text = Math.Abs(dAmt).ToString("N0", MainPage.indianCurancy) + " Cr";
            }
            catch
            {
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;          
            try
            {
                if (InterestStatement.objInterest.txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    PrintPreviewExport(0);
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;          
        }


        private void PrintPreviewExport(int _printStatus)
        {
            try
            {
                bool wStatus = dgrdInterest.Columns["wsd"].Visible, cStatus = dgrdInterest.Columns["cd"].Visible;
                DataTable _dtAdvance = new DataTable();
                DataTable dt = CreatePrintDataTable(wStatus, cStatus, ref _dtAdvance);
                if (dt.Rows.Count > 0)
                {
                    if (!wStatus && !cStatus)
                    {
                        if (chkTax.Checked && dba.ConvertObjectToDouble(lblTaxAmt.Text.Replace(" Dr", "").Replace(" Cr", "")) > 0)
                        {
                            Reporting.InterestReport_WithAddress objReport = new SSS.Reporting.InterestReport_WithAddress();
                            objReport.SetDataSource(dt);
                            objReport.Subreports[0].SetDataSource(_dtAdvance);
                            FinallyPrint(_printStatus, objReport);
                            objReport.Close();
                            objReport.Dispose();
                            //if (_printStatus == 0)
                            //{
                            //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                            //    objShow.myPreview.ReportSource = objReport;
                            //    objShow.ShowDialog();
                            //}
                            //else if (_printStatus == 1)
                            //{
                            //    if (MainPage._PrintWithDialog)
                            //        dba.PrintWithDialog(objReport);
                            //    else
                            //    {
                            //        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            //        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            //        objReport.PrintToPrinter(1, false, 0, 0);
                            //    }
                            //}
                            //else
                            //{
                            //    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                            //    objViewer.ReportSource = objReport;
                            //    objViewer.ExportReport();
                            //}

                            //objReport.Close();
                            //objReport.Dispose();
                        }
                        else
                        {
                            Reporting.InterestReport_WithoutTax objReport = new SSS.Reporting.InterestReport_WithoutTax();
                            objReport.SetDataSource(dt);
                            objReport.Subreports[0].SetDataSource(_dtAdvance);
                            FinallyPrint(_printStatus, objReport);
                            objReport.Close();
                            objReport.Dispose();
                            //if (_printStatus == 0)
                            //{
                            //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                            //    objShow.myPreview.ReportSource = objReport;
                            //    objShow.ShowDialog();
                            //}
                            //else if (_printStatus == 1)
                            //{
                            //    if (MainPage._PrintWithDialog)
                            //        dba.PrintWithDialog(objReport);
                            //    else
                            //    {
                            //        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            //        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            //        objReport.PrintToPrinter(1, false, 0, 0);
                            //    }
                            //}
                            //else
                            //{
                            //    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                            //    objViewer.ReportSource = objReport;
                            //    objViewer.ExportReport();
                            //}
                            //objReport.Close();
                            //objReport.Dispose();
                        }

                    }
                    else if (wStatus && !cStatus)
                    {
                        Reporting.WSRInterestReport objReport = new SSS.Reporting.WSRInterestReport();
                        objReport.SetDataSource(dt);
                        FinallyPrint(_printStatus, objReport);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    if (MainPage._PrintWithDialog)
                        //        dba.PrintWithDialog(objReport);
                        //    else
                        //    {
                        //        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //        objReport.PrintToPrinter(1, false, 0, 0);
                        //    }
                        //}
                        //else
                        //{
                        //    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //    objViewer.ReportSource = objReport;
                        //    objViewer.ExportReport();
                        //}
                        //objReport.Close();
                        //objReport.Dispose();
                    }
                    else if (!wStatus && cStatus)
                    {
                        Reporting.CDInterestReport objReport = new SSS.Reporting.CDInterestReport();
                        objReport.SetDataSource(dt);
                        objReport.Subreports[0].SetDataSource(_dtAdvance);
                        FinallyPrint(_printStatus, objReport);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    if (MainPage._PrintWithDialog)
                        //        dba.PrintWithDialog(objReport);
                        //    else
                        //    {
                        //        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //        objReport.PrintToPrinter(1, false, 0, 0);
                        //    }
                        //}
                        //else
                        //{
                        //    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //    objViewer.ReportSource = objReport;
                        //    objViewer.ExportReport();
                        //}
                        //objReport.Close();
                        //objReport.Dispose();
                    }
                    else if (wStatus && cStatus)
                    {
                        Reporting.WSRCDReport objReport = new SSS.Reporting.WSRCDReport();
                        objReport.SetDataSource(dt);
                        FinallyPrint(_printStatus, objReport);
                        objReport.Close();
                        objReport.Dispose();
                        //if (_printStatus == 0)
                        //{
                        //    Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                        //    objShow.myPreview.ReportSource = objReport;
                        //    objShow.ShowDialog();
                        //}
                        //else if (_printStatus == 1)
                        //{
                        //    if (MainPage._PrintWithDialog)
                        //        dba.PrintWithDialog(objReport);
                        //    else
                        //    {
                        //        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                        //        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //        objReport.PrintToPrinter(1, false, 0, 0);
                        //    }
                        //}
                        //else
                        //{
                        //    CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        //    objViewer.ReportSource = objReport;
                        //    objViewer.ExportReport();
                        //}
                        //objReport.Close();
                        //objReport.Dispose();
                    }
                }
            }
            catch { }
        }

        private void FinallyPrint(int _printStatus, CrystalDecisions.CrystalReports.Engine.ReportClass objReport)
        {
            if (_printStatus == 0)
            {
                Reporting.ShowReport objShow = new Reporting.ShowReport("SHOW INTEREST STATEMENT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();
            }
            else if (_printStatus == 1)
            {
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objReport);
                else
                {
                    objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    objReport.PrintToPrinter(1, false, 0, 0);
                }
            }
            else
            {
                CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                objViewer.ReportSource = objReport;
                objViewer.ExportReport();
            }
            objReport.Close();
            objReport.Dispose();
        }

        private DataTable CreatePrintDataTable(bool wStatus, bool cStatus, ref DataTable dtAdvance)
        {
            DataTable myDataTable = new DataTable();
            try
            {

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("GraceDays", typeof(String));
                myDataTable.Columns.Add("Rate", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Account", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("IDays", typeof(String));
                myDataTable.Columns.Add("Desc", typeof(String));
                myDataTable.Columns.Add("DebitInt", typeof(String));
                myDataTable.Columns.Add("CreditInt", typeof(String));
                myDataTable.Columns.Add("BalanceAmt", typeof(String));
                myDataTable.Columns.Add("FinalAmt", typeof(String));
                myDataTable.Columns.Add("WSR", typeof(String));
                myDataTable.Columns.Add("CD", typeof(String));
                myDataTable.Columns.Add("TotalDebitAmt", typeof(String));
                myDataTable.Columns.Add("TotalCreditAmt", typeof(String));
                myDataTable.Columns.Add("TotalDebitInt", typeof(String));
                myDataTable.Columns.Add("TotalCreditInt", typeof(String));
                myDataTable.Columns.Add("TotalBalanceAmt", typeof(String));
                myDataTable.Columns.Add("TotalWSR", typeof(String));
                myDataTable.Columns.Add("TotalCD", typeof(String));
                myDataTable.Columns.Add("TotalInt", typeof(String));
                myDataTable.Columns.Add("AvgDays", typeof(String));
                myDataTable.Columns.Add("BalanceWithInt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("OnAccount", typeof(String));
                myDataTable.Columns.Add("BalanceWithoutAmt", typeof(String));
                myDataTable.Columns.Add("DiscountName", typeof(String));
                myDataTable.Columns.Add("BalanceWithAmt", typeof(String));
                myDataTable.Columns.Add("TaxAmt", typeof(String));
                myDataTable.Columns.Add("BankName", typeof(String));
                myDataTable.Columns.Add("BranchName", typeof(String));
                myDataTable.Columns.Add("BankAccountNo", typeof(String));
                myDataTable.Columns.Add("IFSCCode", typeof(String));
                myDataTable.Columns.Add("Address", typeof(String));
                myDataTable.Columns.Add("PhoneNo", typeof(String));
                myDataTable.Columns.Add("Other", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));

                string strDate = "From " + InterestStatement.objInterest.txtFromDate.Text + " To " + InterestStatement.objInterest.txtLastDate.Text, strUserName = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                if (!InterestStatement.objInterest.chkDate.Checked)
                    strDate = "From " + MainPage.multiQSDate.ToString("dd/MM/yyyy") + " To " + InterestStatement.objInterest.txtLastDate.Text;
                if (InterestStatement.objInterest.rdoYes.Checked)
                    strDate += "  Grace Days : " + InterestStatement.objInterest.txtGraceDays.Text + "  Int. Rate : DR : " + InterestStatement.objInterest.txtRateDr.Text + "%  CR : " + InterestStatement.objInterest.txtRateCr.Text + "%";

                foreach (DataGridViewRow dr in dgrdInterest.Rows)
                {
                    if (Convert.ToString(dr.Cells["onaccountStatus"].Value) != "COST")
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = "UNCLEAR INTEREST STATEMENT";
                        row["PartyName"] = "UNCLEAR INTEREST STATEMENT OF M/S : " + InterestStatement.objInterest.txtParty.Text;
                        row["DatePeriod"] = strDate;
                        row["GraceDays"] = strDiscountName;

                        row["Date"] = Convert.ToDateTime(dr.Cells["bDate"].Value).ToString("dd/MM/yy");
                        row["Account"] = dr.Cells["particulars"].Value;
                        row["DebitAmt"] = dr.Cells["amountDr"].Value;
                        row["CreditAmt"] = dr.Cells["amountCr"].Value;
                        row["Desc"] = dr.Cells["desc"].Value;
                        row["BalanceAmt"] = dr.Cells["bal"].Value;
                        row["FinalAmt"] = dr.Cells["final"].Value;
                        row["WSR"] = dr.Cells["wsd"].Value;
                        row["OnAccount"] = dr.Cells["onaccountStatus"].Value;

                        row["TotalDebitAmt"] = lblDrAmt.Text;
                        row["TotalCreditAmt"] = lblCrAmt.Text;
                        row["TotalBalanceAmt"] = lblBalance.Text;
                        row["AvgDays"] = lblAvgDays.Text;
                        row["BalanceWithInt"] = lblFinalBal.Text;
                        row["UserName"] = strUserName;
                        row["TaxAmt"] = lblTaxAmt.Text;

                        if (wStatus)
                        {
                            row["WSR"] = dr.Cells["wsd"].Value;
                            row["TotalWSR"] = lblWSR.Text;
                        }
                        else
                        {
                            row["WSR"] = "0";
                            row["TotalWSR"] = "0";
                        }
                        if (cStatus)
                        {
                            row["DebitInt"] = strDiscountName;

                            row["CD"] = dr.Cells["cd"].Value;
                            row["TotalCD"] = lblCDiscount.Text;
                            row["IDays"] = "0";
                            row["TotalInt"] = "0";
                            row["TotalDebitInt"] = "0.00";
                            row["TotalCreditInt"] = "0.00";
                            if (InterestStatement.objInterest.strCategoryName == "WHOLESALER")
                                row["CreditInt"] = "**";
                            else
                                row["CreditInt"] = "*";
                        }
                        else
                        {
                            row["IDays"] = dr.Cells["iDays"].Value;
                            row["DebitInt"] = dr.Cells["intDr"].Value;
                            row["CreditInt"] = dr.Cells["intCr"].Value;
                            row["TotalDebitInt"] = lblIntDr.Text;
                            row["TotalCreditInt"] = lblIntCr.Text;
                            row["TotalInt"] = lblInterest.Text;

                            if (InterestStatement.objInterest.strCategoryName == "WHOLESALER")
                                row["CD"] = "**";
                            else
                                row["CD"] = "*";
                        }

                        if (!wStatus && !cStatus)
                        {
                            if (lblInterest.Text.Contains("Cr"))
                            {
                                //GraceDays WSR TotalWSR
                                row["BalanceWithoutAmt"] = "Balance without T.Discount";
                                row["DiscountName"] = "T. Discount";
                                row["BalanceWithAmt"] = "Balance with T.Discount";
                            }
                            else
                            {
                                row["BalanceWithoutAmt"] = "Balance without interest";
                                row["DiscountName"] = "Interest Amount";
                                row["BalanceWithAmt"] = "Balance with interest";
                            }
                        }
                        else if (cStatus)
                        {
                            row["DiscountName"] = strDiscountName;
                        }
                        myDataTable.Rows.Add(row);
                    }
                }

                dtAdvance = CreateDataTableForPrint();

                if (dtAdvance.Rows.Count > 0 && myDataTable.Rows.Count == 0)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = "UNCLEAR INTEREST STATEMENT";
                    row["PartyName"] = "UNCLEAR INTEREST STATEMENT OF M/S : " + InterestStatement.objInterest.txtParty.Text;
                    row["DatePeriod"] = strDate;
                    row["GraceDays"] = strDiscountName;

                    if (!wStatus && !cStatus)
                    {
                        if (lblInterest.Text.Contains("Cr"))
                        {
                            row["BalanceWithoutAmt"] = "Balance without T.Discount";
                            row["DiscountName"] = "T. Discount";
                            row["BalanceWithAmt"] = "Balance with T.Discount";
                        }
                        else
                        {
                            row["BalanceWithoutAmt"] = "Balance without interest";
                            row["DiscountName"] = "Interest Amount";
                            row["BalanceWithAmt"] = "Balance with interest";
                        }
                    }
                    else if (cStatus)
                    {
                        if (lblCDiscount.Text.Contains("Dr"))
                            row["DiscountName"] = "Reverse " + strDiscountName;
                        else
                            row["DiscountName"] = strDiscountName;
                    }

                    myDataTable.Rows.Add(row);
                }


                if (myDataTable.Rows.Count > 0)
                {
                    DataTable dt = dba.GetDataTable("Select (SM.Address + ', '+SM.Station+', '+SM.State+'-'+SM.PinCode)Address,(SM.MobileNo+ ' '+SM.PhoneNo)PhoneNo,SM.AccountNo,CD.* from SupplierMaster SM Outer Apply (Select TOP 1 CD.FullCompanyName,(Address+'\n'+CD.StateName+'-'+CAST(CD.PinCode as varchar))CompanyAddress, ('Ph. : '+CD.STDNo+'-'+CD.PhoneNo +', Email : '+CD.EmailId) CompanyPhoneNo,CD.TinNo as CompanyTIN,CD.StateName,CD.GSTNo,CD.PANNo,CD.CINNumber from CompanyDetails CD  Order by CD.ID asc) CD Where (SM.AreaCode+SM.AccountNo+' '+SM.Name)='" + InterestStatement.objInterest.txtParty.Text + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow _row = dt.Rows[0];
                        myDataTable.Rows[0]["Address"] = _row["Address"];
                        myDataTable.Rows[0]["PhoneNo"] = _row["PhoneNo"];
                        myDataTable.Rows[0]["Other"] = InterestStatement.objInterest.txtParty.Text;
                        // myDataTable.Rows[0]["PartyName"] = "";

                        myDataTable.Rows[0]["CompanyAddress"] = _row["CompanyAddress"];
                        myDataTable.Rows[0]["CompanyEmail"] = _row["CompanyPhoneNo"];
                        myDataTable.Rows[0]["CompanyGSTNo"] = "GSTIN : " + _row["GSTNo"];
                        myDataTable.Rows[0]["CompanyCINNo"] = "CIN No : " + _row["CINNumber"];

                        myDataTable.Rows[0]["BankName"] = "ICICI BANK";
                        myDataTable.Rows[0]["BranchName"] = "DELHI";
                        myDataTable.Rows[0]["BankAccountNo"] = "SASUSP" + dba.ConvertObjectToDouble(_row["AccountNo"]).ToString("000000");
                        myDataTable.Rows[0]["IFSCCode"] = "ICIC0000106";
                    }
                    else
                        myDataTable.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }

        private DataTable CreateDataTableForPrint()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("Address", typeof(String));
                myDataTable.Columns.Add("PostOffice", typeof(String));
                myDataTable.Columns.Add("PhoneNo", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Account", typeof(String));
                myDataTable.Columns.Add("DebitAmt", typeof(String));
                myDataTable.Columns.Add("CreditAmt", typeof(String));
                myDataTable.Columns.Add("Balance", typeof(String));
                myDataTable.Columns.Add("Description", typeof(String));
                myDataTable.Columns.Add("TotalDebit", typeof(String));
                myDataTable.Columns.Add("TotalCredit", typeof(String));
                myDataTable.Columns.Add("TotalBalance", typeof(String));
                myDataTable.Columns.Add("AmountInWord", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("OnAccount", typeof(String));
                myDataTable.Columns.Add("BankName", typeof(String));
                myDataTable.Columns.Add("BranchName", typeof(String));
                myDataTable.Columns.Add("AccountNo", typeof(String));
                myDataTable.Columns.Add("IFSCCode", typeof(String));
                myDataTable.Columns.Add("FirmName", typeof(String));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("CompanyGSTNo", typeof(String));
                myDataTable.Columns.Add("CompanyCINNo", typeof(String));

                double dADebitAmt = 0, dACreditAmt = 0, dDAmt = 0, dCAmt = 0, dTAmt = 0;

                foreach (DataGridViewRow row in dgrdInterest.Rows)
                {
                    try
                    {
                        if (Convert.ToString(row.Cells["onaccountStatus"].Value) == "COST")
                        {
                            DataRow dRow = myDataTable.NewRow();
                            dRow["CompanyName"] = MainPage.strPrintComapanyName;
                            dRow["PartyName"] = "Advance Details";

                            dADebitAmt += dDAmt = dba.ConvertObjectToDouble(row.Cells["amountDr"].Value);
                            dACreditAmt += dCAmt = dba.ConvertObjectToDouble(row.Cells["amountCr"].Value);

                            dRow["DatePeriod"] = "";
                            dRow["Date"] = Convert.ToDateTime(row.Cells["bDate"].Value).ToString("dd/MM/yyyy");
                            dRow["Account"] = row.Cells["particulars"].Value;
                            dRow["DebitAmt"] = row.Cells["amountDr"].Value;
                            dRow["CreditAmt"] = row.Cells["amountCr"].Value;
                          //  dRow["Balance"] = row.Cells["bal"].Value;
                            dRow["Description"] = row.Cells["desc"].Value;

                            dTAmt = dDAmt - dCAmt;
                            if (dTAmt >= 0)
                                dRow["Balance"] = dTAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else
                                dRow["Balance"] = Math.Abs(dTAmt).ToString("N2", MainPage.indianCurancy) + " Cr";


                            dRow["OnAccount"] = "0";
                            dRow["TotalDebit"] = dADebitAmt.ToString("N2", MainPage.indianCurancy);
                            dRow["TotalCredit"] = dACreditAmt.ToString("N2", MainPage.indianCurancy);

                            double _dAmt = dADebitAmt - dACreditAmt;
                            if (_dAmt >= 0)
                                dRow["TotalBalance"] = _dAmt.ToString("N2", MainPage.indianCurancy) + " Dr";
                            else
                                dRow["TotalBalance"] = Math.Abs(_dAmt).ToString("N2", MainPage.indianCurancy) + " Cr";

                            dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                            myDataTable.Rows.Add(dRow);
                        }
                        //}
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;           
            try
            {
                if (InterestStatement.objInterest.txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    PrintPreviewExport(1);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdInterest.Rows)
                {
                    row.Cells["chk"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;           
            try
            {
                if (InterestStatement.objInterest.txtParty.Text != "" && dgrdInterest.Rows.Count > 0)
                {
                    PrintPreviewExport(2);
                }
            }
            catch
            {
            }
            btnExport.Enabled = true;

        }

        private void dgrdInterest_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkAll.Visible = false;
                    else
                        chkAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void chkTax_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalAmount();
        }

        private void dgrdInterest_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 0)
                    {
                        if (dgrdInterest.CurrentRow.DefaultCellStyle.BackColor.Name != "Gold")
                        {
                            if (Convert.ToBoolean(dgrdInterest.CurrentCell.EditedFormattedValue))
                                dgrdInterest.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                            else
                                dgrdInterest.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                    else if (e.ColumnIndex == 2)
                        ShowDetails();
                }
            }
            catch
            {
            }
        }

        private void ShowDetails()
        {
            DateTime ledgerDate = Convert.ToDateTime(dgrdInterest.CurrentRow.Cells["bDate"].Value);// dba.ConvertDateInExactFormat(Convert.ToString(dgrdLedger.CurrentRow.Cells["date"].Value));
            if (ledgerDate >= MainPage.startFinDate && ledgerDate < MainPage.endFinDate)
            {
                string strAccount = Convert.ToString(dgrdInterest.CurrentRow.Cells["particulars"].Value).ToUpper();
                if (strAccount == "PURCHASE A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        if (dba.GetPurchaseRecordType(strNumber[0], strNumber[1]))
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                PurchaseBook objPurchase = new PurchaseBook(strNumber[0], strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                            else
                            {
                                string strCode = strNumber[0].Replace("PB", "GB");
                                GoodscumPurchase objPurchase = new GoodscumPurchase(strCode, strNumber[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                        }
                        else
                        {
                            PurchaseBook_Trading objPurchase = new PurchaseBook_Trading(strNumber[0], strNumber[1]);
                            objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objPurchase.ShowInTaskbar = true;
                            objPurchase.Show();
                        }
                    }
                }
                else if (strAccount == "SALES A/C")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        string str = dba.GetSalesRecordType(strNumber[0], strNumber[1]);
                        if (str=="")
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                dba.ShowSaleBookPrint(strNumber[0], strNumber[1],false, false);
                            }
                            else
                            {
                                SaleBook objSale = new SaleBook(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }
                        else
                        {
                            if (str == "RETAIL")
                            {
                                SaleBook_Retail objSale = new SaleBook_Retail(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                            else
                            {
                                SaleBook_Trading objSale = new SaleBook_Trading(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }

                    }
                }
                else if (strAccount == "SALE RETURN")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        SaleReturn objSale = new SaleReturn(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "PURCHASE RETURN")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        PurchaseReturn objSale = new PurchaseReturn(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "SALE SERVICE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        SaleServiceBook objSale = new SaleServiceBook(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "CREDIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        CreditNote_Supplier objSale = new CreditNote_Supplier(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
                else if (strAccount == "DEBIT NOTE")
                {
                    string strInvoiceNo = Convert.ToString(dgrdInterest.CurrentRow.Cells["desc"].Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        DebitNote_Customer objDebitNote = new DebitNote_Customer(strNumber[0], strNumber[1]);
                        objDebitNote.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objDebitNote.ShowInTaskbar = true;
                        objDebitNote.Show();
                    }
                }
                else
                {
                    string[] strName = strAccount.Split('|');
                    if (strName.Length > 1)
                    {
                        string[] strVoucher = strName[1].Trim().Split(' ');
                        if (strVoucher.Length > 0)
                        {
                            if (strName[0].Trim() == "JOURNAL A/C")
                            {
                                JournalEntry_New objJournalEntry = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                                objJournalEntry.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                objJournalEntry.ShowInTaskbar = true;
                                objJournalEntry.Show();
                            }
                            else
                            {
                                object objCode = DataBaseAccess.ExecuteMyScalar("Select CashVCode from CompanySetting Where CashVCode='" + strVoucher[0] + "'");
                                if (Convert.ToString(objCode) != "")
                                {
                                    CashBook objCashBook = new CashBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                                    objCashBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                    objCashBook.ShowInTaskbar = true;
                                    objCashBook.Show();
                                }
                                else
                                {
                                    BankBook objBankBook = new BankBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                                    objBankBook.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                                    objBankBook.ShowInTaskbar = true;
                                    objBankBook.Show();
                                }
                            }
                        }
                    }
                    else
                    {
                        //string strJournal = Convert.ToString(dgrdInterest.CurrentRow.Cells["journalID"].Value);
                        //string[] strVoucher = strJournal.Split(' ');
                        //if (strVoucher.Length > 0)
                        //{
                        //    JournalEntry_New objJournal = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                        //    objJournal.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                        //    objJournal.ShowInTaskbar = true;
                        //    objJournal.Show();
                        //}
                    }
                }
            }
        }

        private void dgrdInterest_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdInterest.CurrentRow.Index;
                    if (dgrdInterest.CurrentRow.DefaultCellStyle.BackColor.Name != "Gold")
                    {
                        if (dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                        {
                            dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dgrdInterest.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdInterest.CurrentCell.ColumnIndex == 2 && dgrdInterest.CurrentCell.RowIndex >= 0)
                    {
                        ShowDetails();
                    }
                }
            }
            catch
            {
            }
        }

        private void HideRecords_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            // dba.EnableCopyOnClipBoard(dgrdInterest);
        }
    }
}
