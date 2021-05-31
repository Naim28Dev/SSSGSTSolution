using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class TrialBalance : Form
    {
        DataBaseAccess dba;
        public TrialBalance()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            //GetAllData();
        }

        private void TrialBalance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !dgrdTrial.Focused)
                SendKeys.Send("{TAB}");
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {          
                txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
            {
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDate.Focus();
            }
            else
                GetAllData();
            btnGo.Enabled = true;
        }

        private void GetAllData()
        {
            try
            {
                dgrdTrial.Rows.Clear();

                if (!chkGroup.Checked && !chkCategory.Checked && !chkPartyName.Checked)
                    chkGroup.Checked = true;

                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                eDate = eDate.AddDays(1);

                string strColumnName = "", strAccountStatusColumn = "", strGroupType = " '' ";
                if (chkGroup.Checked)
                {
                    strColumnName = "GroupName";
                    strAccountStatusColumn += "AccountStatus  as GroupName";
                }
                else
                    strGroupType = "AccountStatus";
                if (chkCategory.Checked)
                {
                    if (strColumnName != "")
                    {
                        strColumnName += ",";
                        strAccountStatusColumn += ",";
                    }
                    strColumnName += "Category";

                    strAccountStatusColumn += strGroupType + " as Category";
                }
                if (chkPartyName.Checked)
                {
                    if (strColumnName != "")
                    {
                        strColumnName += ",";
                        strAccountStatusColumn += ",";
                    }
                    strColumnName += "_PartyName";
                    strAccountStatusColumn += strGroupType + " as _PartyName";
                }

                if (strColumnName == "")
                {
                    strColumnName += "GroupName";
                    strAccountStatusColumn += "AccountStatus  as GroupName";
                }

                string strOSubQuery = " and (AccountStatus='OPENING' OR Date< '" + sDate.ToString("MM/dd/yyyy") + "') ", strSubQuery = " and AccountStatus<>'OPENING' and Date>= '" + sDate.ToString("MM/dd/yyyy") + "'  and Date< '" + eDate.ToString("MM/dd/yyyy") + "' ";

                string strQuery = " Select " + strColumnName + ",SUM(OpeningAmt) OAmt,SUM(DAmt) DAmt,SUM(CAmt) CAmt,SUM(DAmt-CAmt) Amount from (Select " + strColumnName + ",SUM(ISNULL(Amt,0)) OpeningAmt,0 DAmt,0 CAmt From(Select " + strAccountStatusColumn + ", -ISNULL((CAST(BA.Amount as Money)-ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)),0) Amt from BalanceAmount BA  Where BA.Status='DEBIT' " + strOSubQuery + " and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE')   Union All   "
                               + " Select " + strAccountStatusColumn + ",ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) Amt from BalanceAmount BA Where BA.Status = 'CREDIT' " + strOSubQuery + " and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE')   )Balance Group by " + strColumnName + " UNION ALL "
                               + " Select " + strColumnName + ", SUM(Amount)OpeningAmt,0 DAmt,0 CAmt from(Select " + strColumnName + ", SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and CAST(Amount as Money) != 0 " + strOSubQuery + " Group by " + strColumnName + " UNION ALL "
                               + " Select " + strColumnName + ", -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT'  and CAST(Amount as Money) != 0 " + strOSubQuery + " Group by " + strColumnName + ")Sales Group by " + strColumnName + " UNION ALL "

                               + " Select " + strColumnName + ", 0 as OpeningAmt, SUM(ISNULL(DAmt, 0)) DAmt,SUM(ISNULL(CAmt, 0)) CAmt From(Select " + strAccountStatusColumn + ",0 as DAmt, ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)), 0) CAmt from BalanceAmount BA  Where BA.Status = 'DEBIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE') " + strSubQuery + "  Union All "
                               + " Select " + strAccountStatusColumn + ",ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) Dmt,0 as CAmt from BalanceAmount BA Where BA.Status = 'CREDIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE') " + strSubQuery + "  )Balance Group by " + strColumnName + " UNION ALL "
                               + " Select " + strColumnName + ", 0 as OpeningAmt,SUM(DAmt)DAmt,SUM(CAmt)CAmt from(Select " + strColumnName + ", SUM(CAST(Amount as Money)) DAmt,0 as CAmt from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and CAST(Amount as Money) != 0 " + strSubQuery + " Group by " + strColumnName + " UNION ALL "
                               + " Select " + strColumnName + ", 0 DAmt, SUM(CAST(Amount as Money)) CAmt from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT'  and CAST(Amount as Money) != 0 " + strSubQuery + " Group by " + strColumnName + " )Sales Group by " + strColumnName + ")Balance Group by " + strColumnName + " Order by " + strColumnName;


                DataTable _dt = dba.GetDataTable(strQuery);
                SetDataWithGrid(_dt);
            }
            catch
            {
            }
        }


        //string strQuery = " Select " + strColumnName + ",SUM(OpeningAmt) OAmt,SUM(Amt)Amount from (Select " + strColumnName + ",SUM(ISNULL(Amt,0)) OpeningAmt,0 Amt From(Select " + strAccountStatusColumn + ", -ISNULL((CAST(BA.Amount as Money)-ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description=BA.Description and BA1.AccountStatus='DUTIES & TAXES'),0)),0) Amt from BalanceAmount BA  Where BA.Status='DEBIT' " + strOSubQuery + " and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE')   Union All   "
        //                      + " Select " + strAccountStatusColumn + ",ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) Amt from BalanceAmount BA Where BA.Status = 'CREDIT' " + strOSubQuery + " and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE')   )Balance Group by " + strColumnName + " UNION ALL "
        //                      + " Select " + strColumnName + ", SUM(Amount)OpeningAmt, 0 as Amt from(Select " + strColumnName + ", SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and CAST(Amount as Money) != 0 " + strOSubQuery + " Group by " + strColumnName + " UNION ALL "
        //                      + " Select " + strColumnName + ", -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT'  and CAST(Amount as Money) != 0 " + strOSubQuery + " Group by " + strColumnName + ")Sales Group by " + strColumnName + " UNION ALL "

        //                      + " Select " + strColumnName + ", 0 as OpeningAmt, SUM(ISNULL(Amt, 0)) Amt From(Select " + strAccountStatusColumn + ", -ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)), 0) Amt from BalanceAmount BA  Where BA.Status = 'DEBIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE') " + strSubQuery + "  Union All "
        //                      + " Select " + strAccountStatusColumn + ",ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) Amt from BalanceAmount BA Where BA.Status = 'CREDIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C','SALE RETURN','PURCHASE RETURN','DEBIT NOTE','CREDIT NOTE') " + strSubQuery + "  )Balance Group by " + strColumnName + " UNION ALL "
        //                      + " Select " + strColumnName + ", 0 as OpeningAmt,SUM(Amount)Amt from(Select " + strColumnName + ", SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and CAST(Amount as Money) != 0 " + strSubQuery + " Group by " + strColumnName + " UNION ALL "
        //                      + " Select " + strColumnName + ", -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName,Category,(AreaCode+AccountNo+' '+Name) as _PartyName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT'  and CAST(Amount as Money) != 0 " + strSubQuery + " Group by " + strColumnName + " )Sales Group by " + strColumnName + ")Balance Group by " + strColumnName + " Order by " + strColumnName;


        private void SetDataWithGrid(DataTable _dt)
        {
            try
            {
                int _index = 0;
                if (_dt.Rows.Count > 0)
                {
                    dgrdTrial.Rows.Add(_dt.Rows.Count);
                    double dOpeningAmt = 0,dDAmt=0,dCAmt=0, dNetAmt = 0;//, dAmt = 0


                    if (chkGroup.Checked)
                        dgrdTrial.Columns["name"].Visible = true;
                    else
                        dgrdTrial.Columns["name"].Visible = false;
                    if (chkCategory.Checked)
                        dgrdTrial.Columns["category"].Visible = true;
                    else
                        dgrdTrial.Columns["category"].Visible = false;
                    if (chkPartyName.Checked)
                        dgrdTrial.Columns["partyName"].Visible = true;
                    else
                        dgrdTrial.Columns["partyName"].Visible = false;

                    foreach (DataRow row in _dt.Rows)
                    {
                        dOpeningAmt = dba.ConvertObjectToDouble(row["OAmt"]);
                        dDAmt = dba.ConvertObjectToDouble(row["DAmt"]);
                        dCAmt = dba.ConvertObjectToDouble(row["CAmt"]);
                        //dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                        dNetAmt = dOpeningAmt + (dDAmt-dCAmt);

                        dgrdTrial.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                        if (chkGroup.Checked)
                            dgrdTrial.Rows[_index].Cells["name"].Value = row["GroupName"];
                        if (chkCategory.Checked)
                            dgrdTrial.Rows[_index].Cells["category"].Value = row["Category"];
                        if (chkPartyName.Checked)
                            dgrdTrial.Rows[_index].Cells["partyName"].Value = row["_PartyName"];

                        dgrdTrial.Rows[_index].Cells["openingAmt"].Value = dOpeningAmt;
                        dgrdTrial.Rows[_index].Cells["debitAmt"].Value = dDAmt;
                        dgrdTrial.Rows[_index].Cells["creditAmt"].Value = dCAmt;

                        //if (dNetAmt > 0)
                        //    dgrdTrial.Rows[_index].Cells["closingAmt"].Value = dNetAmt.ToString("N2", MainPage.indianCurancy);// + " Dr";
                        //else
                            dgrdTrial.Rows[_index].Cells["closingAmt"].Value = dNetAmt.ToString("N2", MainPage.indianCurancy);// + " Cr";

                        _index++;

                        //if (dAmt > 0)
                        //    dgrdTrial.Rows[_index].Cells["debitAmt"].Value = dAmt;
                        //else
                        //    dgrdTrial.Rows[_index].Cells["creditAmt"].Value = Math.Abs(dAmt);

                    }
                }
           
                //if (_dClosingStockAmt != 0)
                //{
                //    _index = dgrdDetails.Rows.Count;
                //    dgrdDetails.Rows.Add();
                //    dgrdDetails.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                //    dgrdDetails.Rows[_index].Cells["particulars"].Value = "CLOSING STOCK";
                //    if (dAmt > 0)
                //        dgrdDetails.Rows[_index].Cells["creditAmt"].Value = _dClosingStockAmt;
                //    else
                //        dgrdDetails.Rows[_index].Cells["debitAmt"].Value = Math.Abs(_dClosingStockAmt);

                //}

                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error in try to set Data in Gridview in Balance Sheet", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private double GetOpeningAndClosingAmt(string strGroupName)
        {
            double dAmt = 0;
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (strGroupName == "Opening Stock")
            {
                if (chkDate.Checked)
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                dAmt = dba.GetOpeningStockAmount(sDate);
            }
            else if (strGroupName == "Closing Stock")
            {
                if (chkDate.Checked)
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                dAmt = dba.GetClosingStockAmount(eDate) * -1;
            }
            return dAmt;
        }
       
        private void CalculateTotalAmount()
        {
            try
            {

                double dDAmt = 0, dCAmt = 0, dDiff = 0, dTotalAmt = 0; ;
                int rowIndex = 0;
                
                foreach (DataGridViewRow row in dgrdTrial.Rows)
                {                  
                        if (Convert.ToString(row.Cells["debitAmt"].Value) != "")
                            dDAmt += dba.ConvertObjectToDouble(row.Cells["debitAmt"].Value);
                        if (Convert.ToString(row.Cells["creditAmt"].Value) != "")
                            dCAmt += dba.ConvertObjectToDouble(row.Cells["creditAmt"].Value);                  
                }

                dDAmt = Convert.ToDouble(dDAmt.ToString("0.00"));
                dCAmt= Convert.ToDouble(dCAmt.ToString("0.00"));

                dDiff = dCAmt - dDAmt;
                dTotalAmt = dDAmt;
                if (dDiff != 0)
                {
                    rowIndex = dgrdTrial.Rows.Count;
                    dgrdTrial.Rows.Add();
                    dgrdTrial.Rows[rowIndex].Cells["name"].Value = "OPENING DIFF";
                    if (dDiff > 0)
                    {
                        dgrdTrial.Rows[rowIndex].Cells["debitAmt"].Value = dDiff.ToString("N2", MainPage.indianCurancy);
                        dTotalAmt +=  dDiff;
                    }
                    else
                    {
                        dgrdTrial.Rows[rowIndex].Cells["creditAmt"].Value = Math.Abs(dDiff).ToString("N2", MainPage.indianCurancy);
                    }
                    dgrdTrial.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgrdTrial.Font, FontStyle.Italic);    
                }
                dgrdTrial.Rows.Add(2);
                rowIndex= dgrdTrial.Rows.Count-1;
                dgrdTrial.Rows[rowIndex].Cells["name"].Value = "TOTAL BALANCE";
                dgrdTrial.Rows[rowIndex].Cells["debitAmt"].Value = dgrdTrial.Rows[rowIndex].Cells["creditAmt"].Value = dTotalAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private void dgrdTrial_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    string strName = Convert.ToString(dgrdTrial.CurrentCell.Value);
                    ShowDetailsByGroupName(strName);
                }
            }
            catch
            {
            }
        }

        private void ShowDetailsByGroupName(string strName)
        {

            if (strName != "" && strName != "TOTAL BALANCE" && strName != "OPENING DIFF")
            {
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }

                if (strName == "CLOSING STOCK")
                {
                    StockRegister objStock = new StockRegister(eDate);
                    objStock.MdiParent = MainPage.mymainObject;
                    objStock.Show();
                }
                else if (strName != "")
                {
                    ShowCategoryWiseDetails objBalance = new ShowCategoryWiseDetails(strName, sDate, eDate);
                    objBalance.MdiParent = MainPage.mymainObject;
                    objBalance.ShowInTaskbar = true;
                    objBalance.Show();
                }
            }
        }

        private void dgrdTrial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdTrial.CurrentCell.ColumnIndex == 1 && dgrdTrial.CurrentCell.RowIndex >= 0)
                    {
                        string strName = Convert.ToString(dgrdTrial.CurrentCell.Value);
                        ShowDetailsByGroupName(strName);
                    }
                }
            }
            catch
            {
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            try
            {
                table.Columns.Add("AccountType", typeof(String));
                table.Columns.Add("Debit", typeof(String));
                table.Columns.Add("Credit", typeof(String));
                table.Columns.Add("FooterAccountType", typeof(String));
                table.Columns.Add("FooterDebit", typeof(String));
                table.Columns.Add("FooterCredit", typeof(String));
                table.Columns.Add("CompanyName", typeof(String));
                table.Columns.Add("ReportHeader", typeof(String));
                table.Columns.Add("UserName", typeof(String));
                table.Columns.Add("Category", typeof(String));
                table.Columns.Add("PartyName", typeof(String));
                table.Columns.Add("OpeningAmount", typeof(String));
                table.Columns.Add("ClosingAmount", typeof(String));
                table.Columns.Add("HeaderImage", typeof(byte[]));
                table.Columns.Add("BrandLogo", typeof(byte[]));

                int rowIndex=0;
                string strDate = "";
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    strDate = " Date period from " + txtFromDate.Text + " to " + txtToDate.Text;
                else
                    strDate = " Date period from " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " to " + MainPage.endFinDate.ToString("dd/MM/yyyy");


                foreach (DataGridViewRow row in dgrdTrial.Rows)
                {
                    DataRow dRow = table.NewRow();
                    dRow["ReportHeader"] = "TRIAL BALANCE REPORT : "+ strDate;
                    dRow["HeaderImage"] = MainPage._headerImage;
                    dRow["BrandLogo"] = MainPage._brandLogo;
                    if (rowIndex < dgrdTrial.Rows.Count - 1)
                    {
                        dRow["AccountType"] = row.Cells["name"].Value;
                        dRow["Category"] = row.Cells["category"].Value;
                        dRow["PartyName"] = row.Cells["partyname"].Value;
                        dRow["Debit"] = row.Cells["debitAmt"].Value;
                        dRow["Credit"] = row.Cells["creditAmt"].Value;
                        dRow["OpeningAmount"] = row.Cells["openingAmt"].Value;
                        dRow["ClosingAmount"] = row.Cells["closingAmt"].Value;
                    }
                    else
                    {
                        dRow["FooterAccountType"] = row.Cells["name"].Value;
                        dRow["FooterDebit"] = row.Cells["debitAmt"].Value;
                        dRow["FooterCredit"] = row.Cells["creditAmt"].Value;
                    }
                    dRow["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    
                    table.Rows.Add(dRow);
                    rowIndex++;
                }

                if (table.Rows.Count > 0)
                {
                    table.Rows[0]["CompanyName"] = MainPage.strPrintComapanyName;
                }

            }
            catch
            {
            }
            return table;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objShowReport = new Reporting.ShowReport("Trial Balance Preview");
                    Reporting.TrialBalanceCrystal objReport = new Reporting.TrialBalanceCrystal();
                    objReport.SetDataSource(dt);
                    objShowReport.myPreview.ReportSource = objReport;
                    objShowReport.Show();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.TrialBalanceCrystal objReport = new Reporting.TrialBalanceCrystal();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                    {
                        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                        objReport.PrintToPrinter(1, false, 0, 0);
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnDetailView_Click(object sender, EventArgs e)
        {
            btnDetailView.Enabled = false;
            try
            {
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                FASDetailPage objFASDetailPage = new FASDetailPage("TRIAL", sDate, eDate);
                objFASDetailPage.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objFASDetailPage.ShowInTaskbar = true;
                objFASDetailPage.Show();
            }
            catch
            {
            }
            btnDetailView.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                if (dgrdTrial.Rows.Count > 0)
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


                        int _skipColumn = 0;
                        string strHeader = "";
                        for (int j = 1; j < dgrdTrial.Columns.Count + 1; j++)
                        {
                            strHeader = dgrdTrial.Columns[j - 1].HeaderText;
                            if (strHeader == "" || !dgrdTrial.Columns[j - 1].Visible)
                            {
                                _skipColumn++;
                                j++;
                            }

                        strHeader = dgrdTrial.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdTrial.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdTrial.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                        }
                        _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdTrial.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdTrial.Columns.Count; l++)
                        {
                            if (dgrdTrial.Columns[l].HeaderText == "" || !dgrdTrial.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }

                            if (dgrdTrial.Columns[l].HeaderText == "" || !dgrdTrial.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }

                            if (l < dgrdTrial.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdTrial.Rows[k].Cells[l].Value;
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Trial_Balance";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                            MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                        }
                        else
                            MessageBox.Show("Export Cancled...");

                    ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                    //xlWorkbook.Close(true, misValue, misValue);
                    //ExcelApp.Quit();
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
            catch(Exception ex)
            { }
            btnExport.Enabled = true;
        }

        private void TrialBalance_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
