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
    public partial class PartyWiseSalePurchase : Form
    {
        DataBaseAccess dba;
        DataTable table;
        public PartyWiseSalePurchase()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
                if (MainPage.strUserRole.Contains("SUPERADMIN"))
                    txtBillCodeCondition.ReadOnly = false;
                else
                    txtBillCodeCondition.ReadOnly = true;
            }
            catch
            {
            }
        }

        private void PartyWiseSalePurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetDataFromDataBase()
        {
            try
            {
                string strQuery = "", strSubQuery = "", strGroupName = "" ;
                if (chkDate.Checked)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1) ;
                    strSubQuery = " and (BM.Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and BM.Date<'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
                if (rdoAll.Checked)
                    strGroupName = "SALES A/C','PURCHASE A/C','SALE RETURN','PURCHASE RETURN";
                else if (rdoSales.Checked)
                    strGroupName = "SALES A/C','SALE RETURN"; 
                else if (rdoPurchase.Checked)
                    strGroupName = "PURCHASE A/C','PURCHASE RETURN";
                //else if (rdoSaleReturn.Checked)
                //    strGroupName = "SALE RETURN";
                //else if (rdoPurchaseReturn.Checked)
                //    strGroupName = "PURCHASE RETURN";

                if (txtPartyName.Text != "")
                {
                    string[] strFullName = txtPartyName.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strSubQuery += " and (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strFullName[0].Trim() + "' ";
                }

                if (txtBillCode.Text != "" && txtBillCodeCondition.Text != "")
                {
                    string strCondition = " not Like ";
                    if (txtBillCodeCondition.Text == "=")
                        strCondition = " Like ";
                    string[] strBillCode = txtBillCode.Text.Split(',');
                    foreach (string strCode in strBillCode)
                    {
                        if (strCode != "")
                            strSubQuery += " and Description " + strCondition + " ('%" + strCode.Trim() + "%')";
                    }
                }        


                if (txtCategory.Text != "")
                    strSubQuery += " and Category ='" + txtCategory.Text + "' ";
                if (txtGroupName.Text != "")
                    strSubQuery += " and GroupName ='" + txtGroupName.Text + "' ";
                if (txtPartyType.Text != "")
                    strSubQuery += " and TINNumber ='" + txtPartyType.Text + "' ";
                if (txtNickName.Text != "")
                    strSubQuery += " and Other ='" + txtNickName.Text + "' ";

                if (rdoByAccountName.Checked)
                {
                    strQuery += " Select * from ( Select PartyName,'' AccountStatus,Category,GroupName,SUM(Amount) as Amount,NickName,SUM(TaxableAmt) as TaxableAmt from ( "
                             + " Select (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + SM.Name) PartyName,UPPER(BM.AccountStatus) as AccountStatus,UPPER(SM.Category) Category,UPPER(SM.GroupName)GroupName, ISNULL(Sum(Cast(BM.Amount as Money)), 0) Amount,SM.Other as NickName,SUM(_Sales.GAmt) as TaxableAmt from BalanceAmount BM inner join SupplierMaster SM  on BM.AccountID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY (Select SUM(GAmt) GAmt from (Select (CAST(SR.NetAmt as Money)-SR.TaxAmount-SR.ServiceAmount-SR.GreenTaxAmt-CAST(SR.OtherPacking as Money)-CAST(SR.Postage as Money)-CAST(SR.Others as Money)) GAmt from SalesRecord SR Where SR.SalePartyID=Bm.AccountID and (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BM.Description UNION ALL Select (SB.NetAmt-SB.TaxAmt-(CAST((SB.OtherSign+CAST(OtherAmt as varchar)) as Money))-(CAST((Description+CAST(DisAmt as varchar)) as Money))-GreenTax-PackingAmt-PostageAmt) GAmt from SalesBook SB Where SB.SalePartyID=Bm.AccountID and (SB.BillCode+' '+CAST(SB.BillNo as varchar))=BM.Description UNION ALL Select (CAST(PR.NetAmt as Money)-PR.TaxAmount-CAST(PR.OtherPer as Money)-CAST(PR.Others as Money)) GAmt from PurchaseRecord PR Where PR.PurchasePartyID=BM.AccountID and (PR.BillCode+' '+CAST(PR.BillNo as varchar))=BM.Description UNION ALL Select (PB.NetAmt-PB.TaxAmt-(CAST((PB.OtherSign+CAST(PB.OtherAmt as varchar)) as Money))+(CAST((PB.Other+CAST(PB.DiscAmt as varchar)) as Money))-PB.PackingAmt) GAmt from PurchaseBook PB Where PB.PurchasePartyID=BM.AccountID and (PB.BillCode+' '+CAST(PB.BillNo as varchar))=BM.Description )Sales)_Sales Where BM.AccountStatus in ('SALES A/C','PURCHASE A/C') and BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + " Group by SM.AreaCode,SM.AccountNo,SM.Name,SM.Category,SM.GroupName,BM.AccountStatus,SM.Other UNION ALL "
                             + " Select (SM.AreaCode + CAST(SM.AccountNo as varchar) + ' ' + SM.Name) PartyName,UPPER(BM.AccountStatus) as AccountStatus,UPPER(SM.Category) Category,UPPER(SM.GroupName)GroupName, -ISNULL(Sum(Cast(BM.Amount as Money)), 0) Amount,SM.Other as NickName,-SUM(_Sales.GAmt) as TaxableAmt  from BalanceAmount BM inner join SupplierMaster SM  on BM.AccountID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY (Select SUM(GAmt) GAmt from (Select (SR.NetAmt-SR.TaxAmount-SR.PackingAmt-SR.ServiceAmt-(CAST((SR.OtherSign+CAST(SR.OtherAmt as varchar)) as Money))) GAmt from SaleReturn SR Where  SR.SalePartyID=Bm.AccountID and (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BM.Description UNION ALL Select (PR.NetAmt-PR.TaxAmount-(CAST((PR.OtherSign+CAST(PR.OtherAmt as varchar)) as Money))) GAmt from PurchaseReturn PR Where PR.PurchasePartyID=BM.AccountID and (PR.BillCode+' '+CAST(PR.BillNo as varchar))=BM.Description )SaleReturn) _Sales Where BM.AccountStatus in ('SALE RETURN','PURCHASE RETURN') and BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + " Group by SM.AreaCode,SM.AccountNo,SM.Name,SM.Category,SM.GroupName,BM.AccountStatus,SM.Other)Sales Group by PartyName,Category,GroupName,NickName) Sale Where Amount!=0  Order by PartyName ";
                  
                    //strQuery = " Select (SM.AreaCode+CAST(SM.AccountNo as varchar)+' '+SM.Name) PartyName,UPPER(BM.AccountStatus) as AccountStatus,UPPER(SM.Category) Category,UPPER(SM.GroupName)GroupName, ISNULL(Sum(Cast(BM.Amount as Money)),0) Amount from BalanceAmount BM inner join SupplierMaster SM "
                    //         + " on BM.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + "  Group by SM.AreaCode,SM.AccountNo,SM.Name,SM.Category,SM.GroupName,BM.AccountStatus Order by SM.Name ";
                }
                else
                {
                    //strQuery = " Select Other as PartyName,UPPER(BM.AccountStatus) as AccountStatus,'' as Category,UPPER(SM.GroupName)GroupName, ISNULL(Sum(Cast(BM.Amount as Money)),0) Amount from BalanceAmount BM inner join SupplierMaster SM "
                    //          + " on BM.AccountID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Where BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + "  Group by SM.Other,SM.GroupName,BM.AccountStatus Order by SM.Other ";

                    strQuery += " Select * from ( Select PartyName,'' AccountStatus,'' as Category,GroupName,SUM(Amount) as Amount,PartyName as NickName,SUM(TaxableAmt) as TaxableAmt from ( "
                                + " Select (SM.Other) PartyName,UPPER(BM.AccountStatus) as AccountStatus,UPPER(SM.Category) Category,UPPER(SM.GroupName)GroupName, ISNULL(Sum(Cast(BM.Amount as Money)), 0) Amount,SUM(_Sales.GAmt) as TaxableAmt from BalanceAmount BM inner join SupplierMaster SM  on BM.AccountID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY (Select SUM(GAmt) GAmt from (Select (CAST(SR.NetAmt as Money)-SR.TaxAmount-SR.ServiceAmount-SR.GreenTaxAmt-CAST(SR.OtherPacking as Money)-CAST(SR.Postage as Money)-CAST(SR.Others as Money)) GAmt from SalesRecord SR Where SR.SalePartyID=Bm.AccountID and (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BM.Description UNION ALL Select (SB.NetAmt-SB.TaxAmt-(CAST((SB.OtherSign+CAST(OtherAmt as varchar)) as Money))-(CAST((Description+CAST(DisAmt as varchar)) as Money))-GreenTax-PackingAmt-PostageAmt) GAmt from SalesBook SB Where SB.SalePartyID=Bm.AccountID and (SB.BillCode+' '+CAST(SB.BillNo as varchar))=BM.Description UNION ALL Select (CAST(PR.NetAmt as Money)-PR.TaxAmount-CAST(PR.OtherPer as Money)-CAST(PR.Others as Money)) GAmt from PurchaseRecord PR Where PR.PurchasePartyID=BM.AccountID and (PR.BillCode+' '+CAST(PR.BillNo as varchar))=BM.Description UNION ALL Select (PB.NetAmt-PB.TaxAmt-(CAST((PB.OtherSign+CAST(PB.OtherAmt as varchar)) as Money))+(CAST((PB.Other+CAST(PB.DiscAmt as varchar)) as Money))-PB.PackingAmt) GAmt from PurchaseBook PB Where PB.PurchasePartyID=BM.AccountID and (PB.BillCode+' '+CAST(PB.BillNo as varchar))=BM.Description )Sales)_Sales Where BM.AccountStatus in ('SALES A/C','PURCHASE A/C') and BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + " Group by SM.Other,SM.Category,SM.GroupName,BM.AccountStatus UNION ALL "
                                + " Select (SM.Other) PartyName,UPPER(BM.AccountStatus) as AccountStatus,UPPER(SM.Category) Category,UPPER(SM.GroupName)GroupName, -ISNULL(Sum(Cast(BM.Amount as Money)), 0) Amount,-SUM(_Sales.GAmt) as TaxableAmt from BalanceAmount BM inner join SupplierMaster SM  on BM.AccountID = (SM.AreaCode + CAST(SM.AccountNo as varchar)) OUTER APPLY (Select SUM(GAmt) GAmt from (Select (SR.NetAmt-SR.TaxAmount-SR.PackingAmt-SR.ServiceAmt-(CAST((SR.OtherSign+CAST(SR.OtherAmt as varchar)) as Money))) GAmt from SaleReturn SR Where SR.SalePartyID=Bm.AccountID and (SR.BillCode+' '+CAST(SR.BillNo as varchar))=BM.Description UNION ALL Select (PR.NetAmt-PR.TaxAmount-(CAST((PR.OtherSign+CAST(PR.OtherAmt as varchar)) as Money))) GAmt from PurchaseReturn PR Where PR.PurchasePartyID=BM.AccountID and (PR.BillCode+' '+CAST(PR.BillNo as varchar))=BM.Description )SaleReturn)_Sales Where BM.AccountStatus in ('SALE RETURN','PURCHASE RETURN') and BM.AccountStatus in ('" + strGroupName + "') " + strSubQuery + " Group by SM.Other,SM.Category,SM.GroupName,BM.AccountStatus)Sales Group by PartyName,GroupName) Sale Where Amount!=0  Order by PartyName ";
                }
                table = dba.GetDataTable(strQuery);
                BindRecord();
            }
            catch
            {
            }
        }

        private void BindRecord()
        {
            try
            {
                double dDebitAmount = 0, dCreditAmount = 0, dAmount = 0;
                string strStatus = "", strGroupName = "";
                dgrdParty.Rows.Clear();
                if (table.Rows.Count > 0)
                {
                    int index = 0;
                    dgrdParty.Rows.Add(table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        strGroupName = Convert.ToString(row["GroupName"]);
                        dAmount = dba.ConvertObjectToDouble(row["Amount"]);
                        if (strGroupName == "SUNDRY DEBTORS")
                        {
                            if (dAmount >= 0)
                            {
                                strStatus = "DEBIT";
                                dDebitAmount += dAmount;
                            }
                            else
                            {
                                strStatus = "CREDIT";
                                dCreditAmount += Math.Abs(dAmount);
                            }
                        }
                        else
                        {                           
                            if (dAmount >= 0)
                            {
                                strStatus = "CREDIT";
                                dCreditAmount += dAmount;
                            }
                            else
                            {
                                strStatus = "DEBIT";
                                dDebitAmount += Math.Abs(dAmount);
                            }
                        }
                        dgrdParty.Rows[index].Cells["sNo"].Value = index + 1;
                        dgrdParty.Rows[index].Cells["partyName"].Value = row["PartyName"];
                        dgrdParty.Rows[index].Cells["category"].Value = row["Category"];
                        dgrdParty.Rows[index].Cells["billType"].Value = row["AccountStatus"];
                        dgrdParty.Rows[index].Cells["groupName"].Value = strGroupName;
                        dgrdParty.Rows[index].Cells["balance"].Value = Math.Abs(dAmount);
                        dgrdParty.Rows[index].Cells["status"].Value = strStatus;
                        dgrdParty.Rows[index].Cells["nickName"].Value = row["NickName"];
                        dgrdParty.Rows[index].Cells["taxableAmt"].Value = row["TaxableAmt"];
                        index++;
                    }
                }

                lblDebit.Text = dDebitAmount.ToString("N2", MainPage.indianCurancy);
                lblCredit.Text = dCreditAmount.ToString("N2", MainPage.indianCurancy);
                dAmount = dDebitAmount - dCreditAmount;
                if (dAmount > 0)
                    lblBalAmount.Text = dAmount.ToString("N2", MainPage.indianCurancy) + " Dr";
                else
                    lblBalAmount.Text = Math.Abs(dAmount).ToString("N2", MainPage.indianCurancy) + " Cr";
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetDataFromDataBase();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private void dgrdParty_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        public DataTable CreateDataTable()
        {
            DataTable myTable = new DataTable();
            try
            {
                myTable.Columns.Add("CompanyName", typeof(string));
                myTable.Columns.Add("DateRange", typeof(string));
                myTable.Columns.Add("SerialNo", typeof(string));
                myTable.Columns.Add("PartyName", typeof(string));
                myTable.Columns.Add("Category", typeof(string));
                myTable.Columns.Add("GroupName", typeof(string));
                myTable.Columns.Add("Amount", typeof(string));
                myTable.Columns.Add("Status", typeof(string));
                myTable.Columns.Add("DebitAmt", typeof(string));
                myTable.Columns.Add("CreditAmt", typeof(string));
                myTable.Columns.Add("UserName", typeof(string));
                //ASSIGN VALUES IN DATA TABLE
                foreach (DataGridViewRow row in dgrdParty.Rows)
                {
                    DataRow drow = myTable.NewRow();
                    drow["CompanyName"] = MainPage.strPrintComapanyName;
                    drow["DateRange"] = "Date Between "+txtFromDate.Text +" To "+txtToDate.Text;
                    drow["SerialNo"] = row.Cells["sNo"].Value+".";
                    drow["PartyName"] = row.Cells["partyName"].Value;
                    drow["Category"] = row.Cells["category"].Value;
                    drow["GroupName"] = row.Cells["groupName"].Value;
                    drow["Amount"] =row.Cells["balance"].Value;
                  //  drow["Status"] = row.Cells["status"].Value;
                    drow["DebitAmt"] = lblDebit.Text;
                    drow["CreditAmt"] = lblCredit.Text;
                    drow["UserName"] = MainPage.strLoginName + " , Date : " + MainPage.strCurrentDate;
                    myTable.Rows.Add(drow);
                }
            }
            catch { }
            return myTable;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                picPleasewait.Visible = true;
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("Party wise Sale && Purchase");
                Reporting.PartywiseDetails report = new SSS.Reporting.PartywiseDetails();
                report.SetDataSource(dt);
                objShowReport.myPreview.ReportSource = report;
                objShowReport.Show();

                report.Close();
                report.Dispose();
            }
            catch
            {
            }
            btnPreview.Enabled = true;
            picPleasewait.Visible = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            try
            {
                DialogResult result=MessageBox.Show("Are you sure you want to print ?","Question",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (dgrdParty.Rows.Count > 0)
                    {
                        picPleasewait.Visible = true;
                        btnPrint.Enabled = false;
                        DataTable dt = CreateDataTable();
                        Reporting.PartywiseDetails report = new SSS.Reporting.PartywiseDetails();
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
            }
            catch
            {
            }
            btnPrint.Enabled = true;
            picPleasewait.Visible = false;
        }              

        private void dgrdParty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0 && rdoByAccountName.Checked)
            {
                ShowRegister();
            }
        }

        private void dgrdParty_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Space)
            {
                if (dgrdParty.CurrentCell.RowIndex >= 0)
                {
                    if (dgrdParty.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                        dgrdParty.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        dgrdParty.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (dgrdParty.CurrentCell.ColumnIndex == 1 && dgrdParty.CurrentCell.RowIndex >= 0)
                    ShowRegister();
            }
        }

        private void ShowRegister()
        {
            try
            {
                string strPartyName = Convert.ToString(dgrdParty.CurrentRow.Cells["partyName"].Value), strGroupName = Convert.ToString(dgrdParty.CurrentRow.Cells["groupName"].Value);
                if (strGroupName == "SUNDRY DEBTORS")
                {
                    SalesBookRegisters objRecord = new SalesBookRegisters(strPartyName);
                    objRecord.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objRecord.ShowInTaskbar = true;
                    objRecord.Show();
                }
                else
                {
                    PurchaseBookRegister objRecord = new PurchaseBookRegister(strPartyName);
                    objRecord.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objRecord.ShowInTaskbar = true;
                    objRecord.Show();
                }
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
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
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdParty.Rows.Count > 0)
                {
                    btnExport.Enabled = false;
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
                    for (int j = 1; j < dgrdParty.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdParty.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdParty.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            //j++;
                            continue;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdParty.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdParty.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdParty.Columns.Count; l++)
                        {
                            if (dgrdParty.Columns[l].HeaderText == "" || !dgrdParty.Columns[l].Visible)
                            {
                                _skipColumn++;
                                //l++;
                                continue;
                            }
                            if (l < dgrdParty.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdParty.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Party_Wise_Sale&Purchase";
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
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPartyType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CASHTYPE", "SEARCH PARTY TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }
        

        private void txtNickName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTYNICKNAME", "SEARCH PARTY NICK NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtNickName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGroupName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtBillCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateAllSpace(sender, e);
        }

        private void PartyWiseSalePurchase_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdParty);
        }
    }
}
