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
    public partial class ProfitandLoss : Form
    {             
        DataBaseAccess dba;
        int rowIndex = 0;
        MainPage mainObj = MainPage.mymainObject as MainPage;
        double dGrossProfit = 0;
        public ProfitandLoss()
        {
            InitializeComponent();         
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

            //SetDataWithGrid();            
        }

        private void SetDataWithGrid()
        {
            try
            {
                rowIndex = 0;
                dgrdPL.Rows.Clear();
                dgrdPL.Rows.Add(11);
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                eDate = eDate.AddDays(1);

                DataTable _dt;
                if (MainPage.strSoftwareType == "AGENT")
                    _dt = dba.GetNetProfitAndLossDataTable_Agent(sDate, eDate, 0, false);
                else
                    _dt = dba.GetNetProfitAndLossDataTable(sDate, eDate, 0, false);

                GetOpeningAndClosingStock("Opening Stock");
                SetRightSideData("Sales A/c", _dt);
                SetLeftSideData("Purchase A/c", _dt);
                SetRightSideData("Purchase Return", _dt);
                SetRightSideData("Credit Note", _dt);
                SetLeftSideData("Sale Return", _dt);
                SetRightSideData("Sale Service", _dt);
                SetRightSideData("Debit Note", _dt);
                //SetLeftSideData("Creditor Expense", 4); 
                SetRightSideData("Direct Income A/c", _dt);
                SetRightSideData("Revenue From Operations", _dt);
                SetLeftSideData("Cost Of Material Traded", _dt);
                SetLeftSideData("Direct Expense A/c", _dt);
                GetOpeningAndClosingStock("Closing Stock");
                //SetLeftSideData("Profit & Loss A/c", 10);
                rowIndex++;
                CalculateGrossProfit();
                SetRightSideData("Indirect Income A/c", _dt);
                SetRightSideData("Other Income", _dt);
                SetLeftSideData("Indirect Expense A/c", _dt);
                SetLeftSideData("Employee Benefit Expense", _dt);
                SetLeftSideData("Selling & Distribution Expenses", _dt);
                SetLeftSideData("Other Expenses", _dt);
                SetLeftSideData("Depreciation", _dt);

                // dgrdPL.Rows.Add();
                CalculateTotalBalance();

            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Set Data with Gridview in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void GetOpeningAndClosingStock(string strGroupName)
        {
            try
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

                if (dAmt != 0)
                {
                    if (dAmt > 0)
                    {
                        if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value) != "")
                            rowIndex++;
                        dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value = strGroupName;
                        dgrdPL.Rows[rowIndex].Cells["leftAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    }
                    else if (dAmt < 0)
                    {
                        if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value) != "")
                            rowIndex++;
                        dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value = strGroupName;
                        dgrdPL.Rows[rowIndex].Cells["rightAmt"].Value = Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy);
                        rowIndex++;
                    }
                }
            }
            catch { }
        }

        private double GetAmtFromDataTable(DataTable _dt, string strGroupName)
        {
            double dAmt = 0;
            try
            {
                DataRow[] rows = _dt.Select(" GroupName='" + strGroupName + "' ");
                if (rows.Length > 0)
                {
                    dAmt = dba.ConvertObjectToDouble(rows[0]["Amt"]);
                }
            }
            catch { }
            return dAmt;
        }

        private void SetLeftSideData(string strGroup,DataTable _dt)
        {
            try
            {
                //DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                //if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                //{
                //    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                //    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                //}
                double dAmount = GetAmtFromDataTable(_dt, strGroup);// dba.GetGroupAmountFromQuery(strGroup, sDate, eDate,0);
            
                if (dAmount>0)
                {
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();
                    if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value) != "")
                        rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value = strGroup;                  
                    dgrdPL.Rows[rowIndex].Cells["leftAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);
                    
                }
                else if (dAmount<0)
                {
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();

                    if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value) != "")
                        rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value = strGroup;               
                    dgrdPL.Rows[rowIndex].Cells["rightAmt"].Value = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                }
         
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Set left side Data  in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
       
        private void SetRightSideData(string strGroup, DataTable _dt)
        {
            try
            {
                //DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                //if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                //{
                //    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                //    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                //}
                double dAmount = GetAmtFromDataTable(_dt, strGroup); //dba.GetGroupAmountFromQuery(strGroup, sDate, eDate,0);

                if (dAmount > 0)
                {
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();
                    if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value) != "")
                        rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value = strGroup;                
                    dgrdPL.Rows[rowIndex].Cells["leftAmt"].Value = dAmount.ToString("N2", MainPage.indianCurancy);                  
                }
                else if (dAmount<0)
                {
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();
                    if (Convert.ToString(dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value) != "")
                        rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value = strGroup;                  
                    dgrdPL.Rows[rowIndex].Cells["rightAmt"].Value = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy);                   
                }
               // rowIndex++;           
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Set Right side Data  in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CalculateGrossProfit()
        {
            try
            {
                double debitAmount = 0, creditAmount = 0;
                foreach (DataGridViewRow dr in dgrdPL.Rows)
                {
                    if (Convert.ToString(dr.Cells["rightAmt"].Value) != "")
                        debitAmount += dba.ConvertObjectToDouble(dr.Cells["rightAmt"].Value);
                    if (Convert.ToString(dr.Cells["leftAmt"].Value) != "")
                        creditAmount += dba.ConvertObjectToDouble(dr.Cells["leftAmt"].Value);
                }


                double dNetAmt = debitAmount - creditAmount;              
                string strDiff = (dNetAmt).ToString("0.00");
                double fDiff = double.Parse(strDiff);
                dGrossProfit = fDiff;
                if (fDiff >= 0)
                {                   
                    dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value = "Gross Profit c/o ";
                    dgrdPL.Rows[rowIndex].Cells["leftAmt"].Value = fDiff.ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value = "Gross Profit b/ f";
                    dgrdPL.Rows[rowIndex].Cells["rightAmt"].Value = fDiff.ToString("N2", MainPage.indianCurancy);
                    rowIndex++;

                }
                else if (fDiff < 0)
                {                   
                    dgrdPL.Rows[rowIndex].Cells["rightParticulars"].Value = "Gross  Loss c/o";
                    dgrdPL.Rows[rowIndex].Cells["rightAmt"].Value = Math.Abs(fDiff).ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                    dgrdPL.Rows[rowIndex].Cells["leftParticulars"].Value = "Gross Profit c/o ";
                    dgrdPL.Rows[rowIndex].Cells["leftAmt"].Value = Math.Abs(fDiff).ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Calculate Total Balance in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private void CalculateTotalBalance()
        {
            try
            {
                double debitAmount = 0, creditAmount = 0;
                for (int _index = 0; _index < dgrdPL.Rows.Count; _index++)
                {
                    DataGridViewRow dr = dgrdPL.Rows[_index];

                    if (Convert.ToString(dr.Cells["rightAmt"].Value) != "")
                        debitAmount += dba.ConvertObjectToDouble(dr.Cells["rightAmt"].Value);
                    if (Convert.ToString(dr.Cells["leftAmt"].Value) != "")
                        creditAmount += dba.ConvertObjectToDouble(dr.Cells["leftAmt"].Value);
                }
               

               // string strDiff = (debitAmount - creditAmount).ToString("N2", MainPage.indianCurancy);
                double fDiff = Convert.ToDouble(debitAmount.ToString("0.00"))- Convert.ToDouble(creditAmount.ToString("0.00"));

                if (fDiff>0)
                {
                    // rowIndex++;
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();

                    creditAmount += fDiff;
                    dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["leftParticulars"].Value = "Net Profit";
                    dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["leftAmt"].Value = fDiff.ToString("N2", MainPage.indianCurancy);
                    //CalculateTotalBalance();
                    //return;
                }
                else if(fDiff<0)
                {
                    //rowIndex++;
                    if (rowIndex > dgrdPL.Rows.Count - 2)
                        dgrdPL.Rows.Add();

                    debitAmount += Math.Abs(fDiff);
                    dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["rightParticulars"].Value = "Net Loss";
                    dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["rightAmt"].Value = Math.Abs(fDiff).ToString("N2", MainPage.indianCurancy);
                    //CalculateTotalBalance();
                    //return;
                }

                dgrdPL.Rows.Add(2);
                dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["leftParticulars"].Value = "Total Balance";
                dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["rightAmt"].Value = debitAmount.ToString("N2", MainPage.indianCurancy);
                dgrdPL.Rows[dgrdPL.Rows.Count - 1].Cells["leftAmt"].Value = creditAmount.ToString("N2", MainPage.indianCurancy);

                dgrdPL.Rows[dgrdPL.Rows.Count - 2].DefaultCellStyle.BackColor = Color.MistyRose;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Calculate Total Balance in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ProfitandLoss_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            else if (e.KeyCode==Keys.Escape)
            {
                this.Close();
            }
        }

        private void dgrdPL_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                picPleasewait.Visible = true;               
                if (e.ColumnIndex == 0 || e.ColumnIndex == 2)
                {
                    ShowDetailPage();
                }              
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Content Click Event on Gridview  in Profit and Loss A/c", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            picPleasewait.Visible = false;
        }

        // Printing.................
    
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.ShowReport showReport = new SSS.Reporting.ShowReport(" Profit && Loss Preview");
                Reporting.ProfitLossReport report = new SSS.Reporting.ProfitLossReport();
                report.SetDataSource(dt);
                showReport.myPreview.ReportSource = report;
                showReport.Show();

                report.Close();
                report.Dispose();
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        public DataTable CreateDataTable()
        {
            DataTable myTable = new DataTable();
            try
            {
                myTable.Columns.Add("ReportHeader", typeof(string));
                myTable.Columns.Add("CompanyName", typeof(string));
                myTable.Columns.Add("Particulars", typeof(string));
                myTable.Columns.Add("Amount", typeof(string));
                myTable.Columns.Add("Particulars1", typeof(string));
                myTable.Columns.Add("Amount1", typeof(string));
                myTable.Columns.Add("FooterParticulars", typeof(string));
                myTable.Columns.Add("FooterAmount", typeof(string));
                myTable.Columns.Add("FooterParticulars1", typeof(string));
                myTable.Columns.Add("FooterAmount1", typeof(string));
                myTable.Columns.Add("UserName", typeof(string));

                string strDate = "";
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    strDate = " Date period from " + txtFromDate.Text + " to " + txtToDate.Text;
                else
                    strDate = " Date period from " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " to " + MainPage.endFinDate.ToString("dd/MM/yyyy");


                //ASSIGN VALUES IN DATA TABLE
                for (int index = 0; index < dgrdPL.Rows.Count; ++index)
                {
                    DataGridViewRow row = dgrdPL.Rows[index];
                    DataRow drow = myTable.NewRow();
                    drow["ReportHeader"] = "PROFIT & LOSS DETAILS "+ strDate;
                    drow["CompanyName"] = MainPage.strPrintComapanyName;
                    if (index < dgrdPL.Rows.Count - 1)
                    {
                        drow["Particulars"] = row.Cells["leftParticulars"].Value;
                        drow["Amount"] = row.Cells["leftAmt"].Value;
                        drow["Particulars1"] = row.Cells["rightParticulars"].Value;
                        drow["Amount1"] = row.Cells["rightAmt"].Value;
                    }
                    else
                    {
                        drow["FooterParticulars"] = row.Cells["leftParticulars"].Value;
                        drow["FooterAmount"] = row.Cells["leftAmt"].Value;
                        drow["FooterParticulars1"] = "Total Balance :";
                        drow["FooterAmount1"] = row.Cells["rightAmt"].Value;
                    }
                    drow["UserName"] = MainPage.strLoginName + " , Date : " + MainPage.strCurrentDate ;
                    myTable.Rows.Add(drow);

                }
            }
            catch { }
            return myTable;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdPL.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    Reporting.ProfitLossReport report = new SSS.Reporting.ProfitLossReport();
                    report.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(report);
                    else
                    {
                        report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        report.PrintToPrinter(1, false, 0, 0);
                    }

                    report.Close();
                    report.Dispose();
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }
      
        private void dgrdPL_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdPL_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    int columnIndex = dgrdPL.CurrentCell.ColumnIndex, rowIndex = dgrdPL.CurrentRow.Index;
                    picPleasewait.Visible = true;
                    if (columnIndex == 0 || columnIndex == 2)
                    {
                        ShowDetailPage();
                    }
                    picPleasewait.Visible = false;
                }
            }
            catch
            {
            }
        }

        private void ShowDetailPage()
        {
            string strParty = strParty = Convert.ToString(dgrdPL.CurrentCell.Value);
            if (strParty != "" && strParty != "Total Balance" && strParty != "Net Profit" && strParty != "Net Loss")
            {               
                if (strParty == "Opening Stock" || strParty == "Closing Stock")
                {
                    DateTime eDate = MainPage.endFinDate;
                    if (strParty == "Opening Stock")
                    {
                        if (chkDate.Checked)
                            eDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                        else
                            eDate = MainPage.startFinDate;
                    }
                    else if (strParty == "Closing Stock")
                    {
                        if (chkDate.Checked)
                            eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                        else                             
                            eDate = MainPage.endFinDate;
                    }

                    StockRegister objStock = new StockRegister(eDate);
                    objStock.MdiParent = mainObj;
                    objStock.Show();
                }
                else
                {
                    DateTime _sdate = MainPage.startFinDate, _edate = MainPage.endFinDate;
                    if (chkDate.Checked)
                    {
                        _sdate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                        _edate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    }
                    ShowCategoryWiseDetails objBalance = new ShowCategoryWiseDetails(strParty, _sdate, _edate);
                    objBalance.MdiParent = MainPage.mymainObject;
                    objBalance.ShowInTaskbar = true;
                    objBalance.Show();
                }
            }          
        }


        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
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
                SetDataWithGrid();
            btnGo.Enabled = true;
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
                FASDetailPage objFASDetailPage = new FASDetailPage("PROFIT", sDate, eDate);
                //objFASDetailPage.MdiParent = MainPage.mymainObject;
                objFASDetailPage.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objFASDetailPage.ShowInTaskbar = true;
                objFASDetailPage.Show();
            }
            catch
            {
            }
            btnDetailView.Enabled = true;
        }
    }
}
