using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;


namespace SSS
{
    public partial class ViewPaymentRequest : Form
    {
        DataBaseAccess dba;
        public ViewPaymentRequest()
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();

                btnSendRequest.Enabled = MainPage.mymainObject.bSendRequest;

                if (MainPage.strServerPath.Contains("NET"))
                {
                    btnDownload.Enabled =btnUpload.Enabled= MainPage.mymainObject.bDownloadRequest;
                    btnChangeStatus.Enabled = txtStatusChanged.Enabled = btnStatusChanged.Enabled = MainPage.mymainObject.bChangeStatus;

                    dgrdDetails.Columns["filepath"].Visible = false;
                }

                //if (MainPage.mymainObject.bSendRequest)
                //    txtStatus.Text = "APPROVAL PENDING";
                //if (MainPage.mymainObject.bDownloadRequest)
                //    txtStatus.Text = "APPROVED";
                //GetDataFromDB();
            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewPaymentRequest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
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

        private string CreateQuery()
        {
            string strQuery = "";
            if (txtPartyName.Text != "")
            {
                string[] strFullName = txtPartyName.Text.Split(' ');
                if (strFullName.Length > 1)
                    strQuery += " and PartyID='" + strFullName[0].Trim() + "' ";
            }

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }
            if (chkPaidDate.Checked && txtFromPaidDate.Text.Length == 10 && txtToPaidDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromPaidDate.Text), eDate = dba.ConvertDateInExactFormat(txtToPaidDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and  (PaidDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and PaidDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }


            if (txtStatus.Text!="")
                strQuery += " and [RequestStatus]='"+ txtStatus.Text+"' ";

            if (txtBranchCode.Text != "")
                strQuery += " and [BranchCode]='" + txtBranchCode.Text + "' ";

            if (txtPriority.Text != "")
                strQuery += " and [ReqPriority]='" + txtPriority.Text + "' ";

            if (txtRemark.Text != "")
                strQuery += " and [Remark] Like('%" + txtRemark.Text + "%') ";

            return strQuery;
        }


        private void GetDataFromDB()
        {
            if (txtBranchCode.Text != "" || MainPage.mymainObject.bChangeStatus)
            {
                if (!MainPage.mymainObject.bPrivilegeAccount && (txtStatus.Text == "DOWNLOADED" || txtStatus.Text == ""))
                {
                    MessageBox.Show("Sorry ! Unable to show the downloaded/All record for security reason !! ", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    dgrdDetails.Rows.Clear();
                    lblNetAmt.Text = "0.00";
                    lblNetStatus.Text = "Dr";
                    string strQuery = "", strSubQuery = CreateQuery();
                    strQuery += " Select  *,(CASE WHEN AccountNumber Like('0%') then ''''+AccountNumber else AccountNumber end) as BankAccountNo,SUBSTRING(PartyID+'APIPAY'+REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(AccountName,' ',''),'&',''),':',''),',',''),'/',''),'-',''),'.',''),'(',''),')',''),0,30) Final_Account from [PaymentRequest] Where ID!=0  " + strSubQuery + " Order By Date desc,ReqPriority asc ";

                    DataTable dt = dba.GetDataTable(strQuery);
                    BindDataWithGrid(dt);
                }
            }
            else
            {
                MessageBox.Show("Sorry ! Please select branch code !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBranchCode.Focus();
            }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            double dNetAmt = 0, dAmt = 0;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    string strStatus = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["chkValue"].Value = true;
                        dgrdDetails.Rows[_rowIndex].Cells["Date"].Value = row["Date"];
                        dgrdDetails.Rows[_rowIndex].Cells["partyName"].Value = row["PartyID"] + " " + row["PartyName"];
                        dgrdDetails.Rows[_rowIndex].Cells["cashAmt"].Value = dba.ConvertObjectToDouble(row["CashAmt"]).ToString("N2", MainPage.indianCurancy) + " " + row["CashStatus"];
                        dgrdDetails.Rows[_rowIndex].Cells["purchaseAmt"].Value = dba.ConvertObjectToDouble(row["PurchaseAmt"]).ToString("N2", MainPage.indianCurancy) + " " + row["PurchaseStatus"];
                        dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = row["NetAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["netStatus"].Value = row["NetStatus"];
                        dgrdDetails.Rows[_rowIndex].Cells["filepath"].Value = row["FilePath"];
                        dgrdDetails.Rows[_rowIndex].Cells["bankName"].Value = row["BankName"];
                        dgrdDetails.Rows[_rowIndex].Cells["branchName"].Value = row["BranchName"];
                        dgrdDetails.Rows[_rowIndex].Cells["accountNo"].Value = row["BankAccountNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["ifscCode"].Value = row["IFSCCode"];
                        dgrdDetails.Rows[_rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                        dgrdDetails.Rows[_rowIndex].Cells["requestStatus"].Value = row["RequestStatus"];
                        dgrdDetails.Rows[_rowIndex].Cells["accountName"].Value = row["AccountName"];
                        dgrdDetails.Rows[_rowIndex].Cells["id"].Value = row["ID"];
                        dgrdDetails.Rows[_rowIndex].Cells["beniID"].Value = row["BeniID"];
                        dgrdDetails.Rows[_rowIndex].Cells["finalPartyName"].Value = row["Final_Account"];
                        dgrdDetails.Rows[_rowIndex].Cells["branchCode"].Value = row["BranchCode"];
                        dgrdDetails.Rows[_rowIndex].Cells["remark"].Value = Convert.ToString(row["Remark"]);
                        dgrdDetails.Rows[_rowIndex].Cells["priority"].Value = row["ReqPriority"];
                        dgrdDetails.Rows[_rowIndex].Cells["paidDate"].Value = row["PaidDate"];

                        dAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                        if (Convert.ToString(row["NetStatus"]).Trim().ToUpper() == "DR")
                            dAmt = dAmt * -1;
                        dNetAmt += dAmt;

                        //"ADDED","APPROVAL PENDING", "APPROVED", "DOWNLOADED", "REJECT","REQUESTED", "RESCHEDULED", "PAID", "STOP PAYMENT"
                        strStatus = Convert.ToString(row["requestStatus"]);
                        if (strStatus == "ADDED")
                            dgrdDetails.Rows[_rowIndex].Cells["deleteButton"].Value = "Delete";
                        else if (strStatus == "APPROVED")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        else if (strStatus == "REJECT" || strStatus == "STOP PAYMENT")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                        else if (strStatus == "APPROVAL PENDING")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                        else if (strStatus == "PAID")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.SkyBlue;
                        else if (strStatus == "DOWNLOADED")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                        else if (strStatus == "REQUESTED")
                            dgrdDetails.Rows[_rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;

                        _rowIndex++;
                    }
                }
            }
            catch { }

            lblNetAmt.Text = Math.Abs(dNetAmt).ToString("N2", MainPage.indianCurancy);
            if (dNetAmt >= 0)
                lblNetStatus.Text = "CR";
            else
                lblNetStatus.Text = "DR";
            btnGo.Focus();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            chkAll.Checked = true;
            GetDataFromDB();
            btnGo.Enabled = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 5)
                        OpenEditTrail();
                    else if (e.ColumnIndex == 10)
                        DeleteRecord();
                    else if(e.ColumnIndex == 16)
                        OpenFile(Convert.ToString(dgrdDetails.CurrentCell.Value));  
                }
            }
            catch { }
        }

        private void OpenEditTrail()
        {
            string strBranchCode = Convert.ToString(dgrdDetails.CurrentRow.Cells["branchCode"].Value), strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
            if (strID != "")
            {
                EditTrailDetails objEdit = new EditTrailDetails("PAYMENTREQUEST", strBranchCode, strID);
                objEdit.ShowDialog();
            }
        }

        private void DeleteRecord()
        {
            try
            {
                string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                if (strID != "" && Convert.ToString(dgrdDetails.CurrentCell.Value) != "")
                {
                    dgrdDetails.EndEdit();
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Delete from [dbo].[PaymentRequest] Where [RequestStatus] in ('ADDED') and ID=" + strID;
                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count > 0)
                        {
                            int _rowIndex = dgrdDetails.CurrentRow.Index;
                            dgrdDetails.Rows.RemoveAt(_rowIndex);
                        }
                    }
                }
            }
            catch { }
        }

        private void OpenFile(string strPath)
        {
            System.Diagnostics.Process.Start(strPath);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["chkValue"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        //private string CreateNormalExcel()
        //{
        //    NewExcel.Application ExcelApp = new NewExcel.Application();
        //    NewExcel.Workbook ExcelWorkBook = null;
        //    NewExcel.Worksheet ExcelWorkSheet = null;
        //    string strFileName = GetFileName();
        //    try
        //    {
        //        object misValue = System.Reflection.Missing.Value;
        //        ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
        //        ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
        //        ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
        //        ExcelWorkSheet.Name = "Payment_Request";

        //        int colIndex = 1;

        //        ExcelWorkSheet.Cells[1, colIndex] = "PAYMENT REQUEST FROM " + MainPage.strCompanyName;
        //        ExcelWorkSheet.Range["A1:H1"].Merge();
        //        ExcelWorkSheet.Range["A1:H1"].HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;

        //        ExcelWorkSheet.Cells[2, 1] = "Party Name";
        //        ExcelWorkSheet.Cells[2, 2] = "Bank Name";
        //        ExcelWorkSheet.Cells[2, 3] = "Branch Name";
        //        ExcelWorkSheet.Cells[2, 4] = "Account No";
        //        ExcelWorkSheet.Cells[2, 5] = "Account Name";
        //        ExcelWorkSheet.Cells[2, 6] = "IFSC Code";
        //        ExcelWorkSheet.Cells[2, 7] = "Amount";
        //        ExcelWorkSheet.Cells[2, 8] = "Status";
        //        ExcelWorkSheet.Cells[2, 9] = "Beni ID";

        //        int columnIndex = 1;
        //        foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
        //        {
        //            column.HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;
        //            column.RowHeight = 15;
        //            if (columnIndex == 1)
        //                column.ColumnWidth = (double)column.ColumnWidth + 10;
        //            else if (columnIndex == 7)
        //                column.ColumnWidth = (double)column.ColumnWidth + 5;
        //            else if (columnIndex == 8)
        //            {
        //                column.ColumnWidth = (double)column.ColumnWidth - 3;
        //            }
        //            else if (columnIndex == 9)
        //            {
        //                column.ColumnWidth = (double)column.ColumnWidth + 5;
        //                break;
        //            }
        //            else
        //            {
        //                column.ColumnWidth = (double)column.ColumnWidth + 10;
        //                if (columnIndex == 4 || columnIndex == 7)
        //                    column.NumberFormat = "@";
        //            }
        //            columnIndex++;
        //        }

        //        //NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rowIndex, col];
        //        //objRange.Font.Bold = true;
        //        //objRange.Interior.ColorIndex = 22;


        //        int rowIndex = 3;
        //        foreach (DataGridViewRow row in dgrdDetails.Rows)
        //        {
        //            ExcelWorkSheet.Cells[rowIndex, 1] = row.Cells["partyName"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 2] = row.Cells["bankName"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 3] = row.Cells["branchName"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 4] = row.Cells["accountNo"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 5] = row.Cells["accountName"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 6] = row.Cells["ifscCode"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 7] = row.Cells["netAmt"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 8] = row.Cells["netStatus"].Value;
        //            ExcelWorkSheet.Cells[rowIndex, 9] = row.Cells["beniID"].Value;

        //            rowIndex++;
        //        }


        //        for (int col = 1; col < 10; col++)
        //        {
        //            NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, col];
        //            objRange.Font.Bold = true;

        //            objRange = (NewExcel.Range)ExcelWorkSheet.Cells[2, col];
        //            objRange.Font.Bold = true;
        //            objRange.Interior.ColorIndex = 2;
        //        }


        //        ExcelWorkSheet.Cells[rowIndex, 6] = "Total Payable";
        //        ExcelWorkSheet.Cells[rowIndex, 7] = lblNetAmt.Text;
        //        ExcelWorkSheet.Cells[rowIndex, 8] = lblNetStatus.Text;
        //        rowIndex++;

        //        for (int rIndex = 1; rIndex < rowIndex; rIndex++)
        //        {
        //            for (int cIndex = 1; cIndex < 10; cIndex++)
        //            {
        //                NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
        //                objRange.NumberFormat = "@";
        //                objRange.Cells.BorderAround();
        //            }
        //        }

        //        ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //        ExcelWorkBook.Close(true, misValue, misValue);
        //        ExcelApp.Quit();
        //        Marshal.ReleaseComObject(ExcelWorkSheet);
        //        Marshal.ReleaseComObject(ExcelWorkBook);
        //        Marshal.ReleaseComObject(ExcelApp);

        //        //MessageBox.Show("Thank you ! Summary exported successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        //    }
        //    catch (Exception ex)
        //    {
        //        strFileName = ex.Message;
        //        MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        foreach (Process process in Process.GetProcessesByName("Excel"))
        //            process.Kill();
        //    }
        //    return strFileName;

        //}

        private string WriteInExistingFile(string strFileName)
        {
            NewExcel.Application myExcelApplication;
            NewExcel.Workbook myExcelWorkbook;
            NewExcel.Worksheet myExcelWorkSheet;
            myExcelApplication = null;
            string excelFilePath = "";
            try
            {
                myExcelApplication = new NewExcel.Application(); // create Excell App
                myExcelApplication.DisplayAlerts = false; // turn off alerts

                excelFilePath = GetPaymentFileFromServer();// MainPage.strServerPath + "\\Excel_File\\GSTR1_Template.xlsx";

                myExcelWorkbook = (NewExcel.Workbook)(myExcelApplication.Workbooks._Open(excelFilePath, System.Reflection.Missing.Value,
                   System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                   System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                   System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                   System.Reflection.Missing.Value, System.Reflection.Missing.Value)); // open the existing excel file

                int numberOfWorkbooks = myExcelApplication.Workbooks.Count; // get number of workbooks (optional)

                myExcelWorkSheet = (NewExcel.Worksheet)myExcelWorkbook.Worksheets[2];

                int _rowIndex = 5;
                string strBankName = "", strTransType = "", strStatus = "";
                double dAmount = 0;

                myExcelWorkSheet.Cells[1, 3] = MainPage.strHeadOfficeBankAccountNo;
                myExcelWorkSheet.Cells[2, 3] = "";// MainPage.currentDate.ToString("MMM dd, yyyy");
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkValue"].Value) && Convert.ToString(row.Cells["requestStatus"].Value) == "APPROVED")
                    {
                        strBankName = Convert.ToString(row.Cells["bankName"].Value);
                        dAmount = dba.ConvertObjectToDouble(row.Cells["netAmt"].Value);

                        if (strBankName.Contains("ICICI") || strBankName.Contains("ICIC"))
                            strTransType = "WIB";
                        else if (dAmount >= 200000)
                            strTransType = "RTG";
                        else
                            strTransType = "NFT";

                        myExcelWorkSheet.Cells[_rowIndex, 1] = strTransType;
                        myExcelWorkSheet.Cells[_rowIndex, 2] = MainPage.strHeadOfficeBankAccountNo;
                        myExcelWorkSheet.Cells[_rowIndex, 3] = row.Cells["beniID"].Value;
                        myExcelWorkSheet.Cells[_rowIndex, 4] = dAmount;
                        myExcelWorkSheet.Cells[_rowIndex, 5] = row.Cells["finalPartyName"].Value;

                        _rowIndex++;
                    }
                }


                //for (i = 0; i < Percentage.Count; i++)
                //{
                //    oSheet.Cells[i + 2, 2] = Percentage[i];
                //}

              

                myExcelWorkbook.SaveAs(strFileName, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                              System.Reflection.Missing.Value, System.Reflection.Missing.Value, NewExcel.XlSaveAsAccessMode.xlNoChange,
                                              System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                              System.Reflection.Missing.Value, System.Reflection.Missing.Value); // Save data in excel
                    
                MessageBox.Show("Thank you ! Request downloaded successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                try
                {
                    myExcelWorkbook.Close(true, excelFilePath, System.Reflection.Missing.Value);
                    myExcelWorkbook.Close(true, strFileName, System.Reflection.Missing.Value);
                }
                catch { }
            }
            catch
            { excelFilePath = ""; }
            finally
            {
                

                //foreach (Process process in Process.GetProcessesByName("Excel"))
                //    process.Kill();

                //if (myExcelApplication != null)
                //{
                //    myExcelApplication.Quit(); // close the excel application
                //}
            }
            return excelFilePath;
        }

        private string GetFileName()
        {
            string strFullPath = "", strFileName = "";            

            strFileName = "Payment_Request_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + ".xlsx";

            SaveFileDialog _browser = new SaveFileDialog();
            _browser.Filter = "Excel Files (*.xlsx)|*.xlsx;";
            _browser.FileName = strFileName;
            _browser.ShowDialog();

            if (_browser.FileName != "")
                strFullPath = _browser.FileName;


            if (File.Exists(strFullPath))
                File.Delete(strFullPath);

            return strFullPath;
        }
        
        private string GetAllFilePath()
        {
            string strPath = "";// WriteInExistingFile();// CreateNormalExcel();
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkValue"].Value) && (Convert.ToString(row.Cells["requestStatus"].Value) == "ADDED" || MainPage.strUserRole.Contains("SUPERADMIN")))
                {
                    if (strPath != "")
                        strPath += ","; 
                     strPath += Convert.ToString(row.Cells["filepath"].Value);
                }
            }
            return strPath;
        }

        //private void UpdateRequestStatus()
        //{
        //    string strID = "";
        //    foreach (DataGridViewRow row in dgrdDetails.Rows)
        //    {
        //        if (Convert.ToBoolean(row.Cells["chkValue"].Value))
        //        {
        //            if (strID != "")
        //                strID += ",";
        //            strID += Convert.ToString(row.Cells["id"].Value);
        //        }
        //    }

        //    if (strID != "")
        //    {
        //        string strQuery = " Update [PaymentRequest] Set [RequestStatus]='SENT' Where BranchCode='" + MainPage.strBranchCode + "' and ID in (" + strID + ") ";
        //        int count = dba.ExecuteMyQuery(strQuery);
        //        if (count > 0)
        //            GetDataFromDB();
        //    }
        //}

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count != 0)
                {
                    Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("Payment Request Slip");

                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PaymentRequestReport objReport = new Reporting.PaymentRequestReport();
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
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            ReCalculateAmount();
            DataTable myDataTable = new DataTable("Purchase");
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("PartyName", typeof(String));
                myDataTable.Columns.Add("NetAmt", typeof(String));
                myDataTable.Columns.Add("NetAmtStatus", typeof(String));
                myDataTable.Columns.Add("BankName", typeof(String));
                myDataTable.Columns.Add("BranchName", typeof(String));
                myDataTable.Columns.Add("AccountNo", typeof(String));
                myDataTable.Columns.Add("AccountName", typeof(String));
                myDataTable.Columns.Add("IFSCCode", typeof(String));
                myDataTable.Columns.Add("RequestStatus", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                int _index = 1;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["chkValue"].Value))
                    {
                        DataRow row = myDataTable.NewRow();

                        row["CompanyName"] = MainPage.strPrintComapanyName;
                        row["PartyName"] = dr.Cells["partyName"].Value;
                        row["Date"] = Convert.ToDateTime(dr.Cells["Date"].Value).ToString("dd/MM/yyyy");
                        row["NetAmt"] = dba.ConvertObjectToDouble(dr.Cells["netAmt"].Value).ToString("N2", MainPage.indianCurancy) + " " + dr.Cells["netStatus"].Value;
                        row["BankName"] = dr.Cells["remark"].Value;
                        row["BranchName"] = dr.Cells["branchName"].Value;
                        row["AccountNo"] = dr.Cells["accountNo"].Value;
                        row["AccountName"] = dr.Cells["accountName"].Value;
                        row["IFSCCode"] = _index + ".";// dr.Cells["ifscCode"].Value;
                        row["RequestStatus"] = dr.Cells["requestStatus"].Value;
                        row["UserName"] = MainPage.strLoginName + ", DATE : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                        row["NetAmtStatus"] = lblNetAmt.Text + " " + lblNetStatus.Text;

                        myDataTable.Rows.Add(row);
                        _index++;
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
            try
            {
                if (dgrdDetails.Rows.Count != 0)
                {
                    btnPrint.Enabled = false;

                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.PaymentRequestReport objReport = new Reporting.PaymentRequestReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (dgrdDetails.Rows.Count != 0)
            //    {
            //        btnExport.Enabled = false;

            //        DataTable dt = CreateDataTable();
            //        if (dt.Rows.Count > 0)
            //        {
            //            Reporting.PaymentRequestReport objReport = new Reporting.PaymentRequestReport();
            //            objReport.SetDataSource(dt);
            //            Reporting.ShowReport objShow = new SSS.Reporting.ShowReport("");
            //            objShow.myPreview.ReportSource = objReport;
            //            objShow.myPreview.ExportReport();
            //        }
            //        else
            //            MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //catch
            //{
            //}
            //btnExport.Enabled = true;
        }

        private string GetPaymentFileFromServer()
        {
            string strExePath = MainPage.strServerPath.Replace(@"\NET", "") + @"\Excel_File\payment_request.xlsx";
            try
            {
                if (!File.Exists(strExePath))
                {
                    string strPath = DataBaseAccess.DownloadFileFromServer(strExePath, "payment_request.xlsx");
                    System.Diagnostics.Process.Start(strPath);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Downloading file from Server", ex.Message };
                dba.CreateErrorReports(strReport);
                strExePath = "";
            }
            return strExePath;
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentCell.ColumnIndex == 3 || dgrdDetails.CurrentCell.ColumnIndex == 9)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
                if (dgrdDetails.CurrentCell.ColumnIndex == 8)
                {
                    TextBox txt = (TextBox)e.Control;
                    txt.KeyPress += new KeyPressEventHandler(txtRemark_KeyPress);
                }

            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 3 || dgrdDetails.CurrentCell.ColumnIndex == 9)
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 8)
                dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7)
                {
                    if (MainPage.mymainObject.bSendRequest)
                    {
                        SearchData objSearch = new SearchData("REQUESTPRIORITY", "SEARCH REQUEST PRIORITY", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            UpdateColumnDetails(e.RowIndex, "PRIORITY");
                            e.Cancel = true;
                        }
                    }
                    else
                        e.Cancel = true;
                }
                else if (e.ColumnIndex == 8)
                {
                    if (!MainPage.mymainObject.bChangeStatus)
                    {
                        if (MainPage.mymainObject.bSendRequest)
                        {
                            if (Convert.ToString(dgrdDetails.CurrentRow.Cells["requestStatus"].Value) != "ADDED")
                                e.Cancel = true;
                        }
                        else
                            e.Cancel = true;
                    }
                }
                else if (e.ColumnIndex == 9)
                {
                    if (!MainPage.mymainObject.bChangeStatus)
                        e.Cancel = true;
                }
                else if (e.ColumnIndex == 3)
                {
                    if (MainPage.mymainObject.bDownloadRequest)
                    {
                        string _str = Convert.ToString(dgrdDetails.CurrentRow.Cells["requestStatus"].Value);
                        if (_str != "APPROVAL PENDING")
                            e.Cancel = true;
                    }
                    else
                        e.Cancel = true;
                   
                    //if (MainPage.mymainObject.bSendRequest)
                    //{
                    //    string _str = Convert.ToString(dgrdDetails.CurrentRow.Cells["requestStatus"].Value);
                    //    if (_str != "ADDED" && _str != "APPROVAL PENDING")
                    //    {
                    //        e.Cancel = true;
                    //        MessageBox.Show("Sorry ! Amount can be change only in Added or Approval pending mode", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    }
                    //}
                    //else
                    //    e.Cancel = true;
                }
                else if ((e.ColumnIndex != 0 && e.ColumnIndex != 3))
                    e.Cancel = true;
            }
            catch { e.Cancel = true; }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 3)
                    {
                        if (MainPage.mymainObject.bSendRequest)
                        {
                            UpdateColumnDetails(e.RowIndex, "NETAMOUNT");
                        }
                    }
                    //else if (e.ColumnIndex == 7)
                    //{
                    //    if (MainPage.mymainObject.bSendRequest)
                    //    {
                    //        UpdateColumnDetails(e.RowIndex, "PRIORITY");
                    //    }
                        
                    //}
                    else if (e.ColumnIndex == 8)
                    {
                        if (MainPage.mymainObject.bSendRequest || MainPage.mymainObject.bChangeStatus)
                        {
                            UpdateColumnDetails(e.RowIndex, "REMARK");
                        }
                    }
                    else if (e.ColumnIndex == 9)
                    {
                        string strDate = Convert.ToString(dgrdDetails.CurrentCell.EditedFormattedValue);
                        if (strDate.Length == 8)
                        {
                            strDate = strDate.Replace("/", "");
                            if (strDate.Length == 8)
                            {
                                TextBox txtDate = new TextBox();
                                txtDate.Text = strDate;
                                dba.GetStringFromDateForReporting(txtDate, false);
                                if (!txtDate.Text.Contains("/"))
                                {

                                }
                                else
                                {
                                    if (e.RowIndex < dgrdDetails.Rows.Count - 1)
                                    {
                                        dgrdDetails.EndEdit();
                                    }
                                }
                                dgrdDetails.CurrentCell.Value = txtDate.Text;
                            }
                            else
                            {
                                MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        UpdateColumnDetails(e.RowIndex, "PAIDDATE");
                    }

                    if (e.ColumnIndex == 0 || e.ColumnIndex == 3)
                        ReCalculateAmount();
                }
            }
            catch { }
        }

        private void UpdateColumnDetails(int _rowIndex, string strEditType)
        {
            string strID = Convert.ToString(dgrdDetails.Rows[_rowIndex].Cells["id"].Value), strQuery = "";
            double dNetAmt = 0;
            if (strID != "")
            {
                if (strEditType == "NETAMOUNT")
                {
                    dNetAmt = dba.ConvertObjectToDouble(dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value);
                    if (dNetAmt > 0)
                        strQuery = "Update [PaymentRequest] Set NetAmt=" + dNetAmt + " Where ID=" + strID;
                }
                else if (strEditType == "REMARK")
                {
                    strQuery = "Update [PaymentRequest] Set [Remark]='" + dgrdDetails.Rows[_rowIndex].Cells["remark"].Value + "' Where ID=" + strID;
                }
                else if (strEditType == "PRIORITY")
                {
                    strQuery = "Update [PaymentRequest] Set [ReqPriority]='" + dgrdDetails.Rows[_rowIndex].Cells["priority"].Value + "' Where ID=" + strID;
                }
                else if (strEditType == "PAIDDATE")
                {
                    string strPaidDate = Convert.ToString(dgrdDetails.Rows[_rowIndex].Cells["paidDate"].Value);
                    if (strPaidDate.Length==10)
                        strPaidDate = "'" + dba.ConvertDateInExactFormat(strPaidDate).ToString("MM/dd/yyyy") + "' ";
                    else
                        strPaidDate = "NULL";

                    strQuery = "Update [PaymentRequest] Set [PaidDate]=" + strPaidDate + " Where ID=" + strID;
                }

                if (strQuery != "")
                {
                    int _count = dba.ExecuteMyQuery(strQuery);
                    if (_count <= 0)
                    {
                        MessageBox.Show("Sorry ! Net amt not updated right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (strEditType == "NETAMOUNT")
                            dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = dNetAmt;
                    }
                }
            }
        }

        private bool ValidateRequest()
        {
            try
            {
                string strPartyName="", strPartyID = "";
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["chkValue"].Value) && (Convert.ToString(dr.Cells["requestStatus"].Value) == "ADDED"))
                    {
                        strPartyName = Convert.ToString(dr.Cells["partyName"].Value);
                        string[] strParty = strPartyName.Split(' ');
                        if(strParty.Length>0)
                        {
                            if (strPartyID != "")
                                strPartyID += ",";
                            strPartyID += "'" + strParty[0] + "'";
                        }
                    }
                }
                if (strPartyID != "")
                {
                    string strPartyDetails = "";
                    string strQuery = "Select (PartyID+' '+PartyName)PName,RequestStatus from PaymentRequest Where PartyID in (" + strPartyID + ") and RequestStatus in ('APPROVAL PENDING', 'APPROVED', 'DOWNLOADED','REQUESTED', 'RESCHEDULED')  Order by RequestStatus ";
                    DataTable dt = null;
                    if (MainPage.strOnlineDataBaseName != "" && MainPage.strLiveDataBaseIP != "")
                        dt = NetDBAccess.GetDataTableRecord(strQuery);
                    else
                        dt = dba.GetDataTable(strQuery);

                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (strPartyDetails != "")
                                    strPartyDetails += ",";
                                strPartyDetails += Convert.ToString(row["PName"] + " (" + row["RequestStatus"] + ")");
                            }

                            if (strPartyDetails != "")
                            {
                                MessageBox.Show("Sorry ! These party (" + strPartyDetails + ") already have the active payment reqest,\nPlease wait for payment done or stop payment of these parties.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                            else
                                return true;

                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }


        private int SendRequest()
        {
            string strQuery = "",strLocalQuery="", strID = "",_strQry="";
            int count = 0;

            string strStatus = "REQUESTED";
            if (MainPage.strServerPath.Contains("NET"))
                strStatus = "APPROVAL PENDING";

            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(dr.Cells["chkValue"].Value) && (Convert.ToString(dr.Cells["requestStatus"].Value)=="ADDED" || MainPage.strUserRole.Contains("SUPERADMIN")))
                {
                    if (strID != "")
                        strID += ",";
                    strID += Convert.ToString(dr.Cells["id"].Value);
                    _strQry += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('PAYMENTREQUEST','" + dr.Cells["branchCode"].Value + "'," + dr.Cells["id"].Value + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(dr.Cells["netAmt"].Value) + ",'" + MainPage.strLoginName + "',1,0,'" + strStatus + "') ";
                }
            }

            if (strID != "")
            {
                strLocalQuery = " Update PaymentRequest Set InsertStatus=0,UpdateStatus=0,[RequestStatus]='"+ strStatus+"' Where ID in (" + strID + ")";

                if (MainPage.strOnlineDataBaseName != "")
                {
                    _strQry = "";
                    string strSelectQuery = "Select * from PaymentRequest Where  ID in (" + strID + ") ";
                    DataTable dt = dba.GetDataTable(strSelectQuery);

                    foreach (DataRow row in dt.Rows)
                    {
                        strQuery += "  INSERT INTO [dbo].[PaymentRequest] ([BranchCode],[PartyID],[PartyName],[CashAmt],[CashStatus],[PurchaseAmt],[PurchaseStatus],[NetAmt],[NetStatus],[Date],[FilePath],[BankName],[BranchName],[AccountNumber],[AccountName],[IFSCCode],[CreatedBy],[RequestStatus],[InsertStatus],[UpdateStatus],[BeniID],[Remark],[ReqPriority]) OUTPUT INSERTED.ID VALUES "
                                 + "  ('" + row["BranchCode"] + "','" + row["PartyID"] + "','" + row["PartyName"] + "','" + row["CashAmt"] + "','" + row["CashStatus"] + "','" + row["PurchaseAmt"] + "','" + row["PurchaseStatus"] + "','" + row["NetAmt"] + "','" + row["NetStatus"] + "','" + row["Date"] + "','" + row["FilePath"] + "','" + row["BankName"] + "','" + row["BranchName"] + "','" + row["AccountNumber"] + "','" + row["AccountName"] + "','" + row["IFSCCode"] + "','" + row["CreatedBy"] + "','APPROVAL PENDING',0,0,'" + row["BeniID"] + "','" + row["Remark"] + "','" + row["ReqPriority"] + "')   "
                                 + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + "('PAYMENTREQUEST','" + row["BranchCode"] + "', SCOPE_IDENTITY(),DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(row["NetAmt"]) + ",'" + MainPage.strLoginName + "',0,0,'APPROVAL PENDING') ";

                    }

                    count = DataBaseAccess.ExecuteQueryOnNet(strQuery, "", strLocalQuery, MainPage.strOnlineDataBaseName);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Payment request sent successfully ! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("Sorry !! Unable to send request right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                    count = dba.ExecuteMyQuery(strLocalQuery+_strQry);
            }
            else
            {
                MessageBox.Show("Sorry ! Please select atleast one pending request !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return count;
        }

        //private void btnAccept_Click(object sender, EventArgs e)
        //{
        //    btnAccept.Enabled = false;
        //    DialogResult result = MessageBox.Show("Are you sure you want to change request status ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //    if (result == DialogResult.Yes)
        //    {
        //        ChangeRequestStatus("ACCEPT");
        //    }
        //    btnAccept.Enabled = true;
        //}

        //private void btnIgnore_Click(object sender, EventArgs e)
        //{
        //    btnIgnore.Enabled = false;
        //    DialogResult result = MessageBox.Show("Are you sure you want to change request status ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //    if(result==DialogResult.Yes)
        //    {
        //        ChangeRequestStatus("IGNORE");
        //    }
        //    btnIgnore.Enabled = true;
        //}

        private int UpdatePaymentRequest(string strStatus)
        {
            int _count = 0;

            string strID = "", strQuery = "", strOldStatus = "", strReqStatus = "";
            if (strStatus == "APPROVED" || strStatus == "REJECT")
                strOldStatus = "'APPROVAL PENDING','RESCHEDULED'";
            else if (strStatus == "PAID")
                strOldStatus = "'UPLOADED','DOWNLOADED','RESCHEDULED','STOP PAYMENT'";
            else if (strStatus == "DOWNLOADED" || strStatus == "UPLOADED")
                strOldStatus = "'APPROVED'";
            else if (strStatus == "STOP PAYMENT")
                strOldStatus = "'DOWNLOADED','UPLOADED','PAID','RESCHEDULED'";
            else if (strStatus == "RESCHEDULED")
                strOldStatus = "'DOWNLOADED','UPLOADED','PAID','REJECT'";
            else if (strStatus == "APPROVAL PENDING")
                strOldStatus = "'APPROVED'";
            else if (strStatus == "ADDED")
                strOldStatus = "'APPROVAL PENDING'";

            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(dr.Cells["chkValue"].Value))
                {
                    strReqStatus = Convert.ToString(dr.Cells["requestStatus"].Value);
                    if ((strReqStatus == "PAID" && (MainPage.strUserRole == "SUPERADMIN" || MainPage.strLoginName == "SANJAY")) || strReqStatus != "PAID")
                    {
                        if ((strStatus == "APPROVED" && MainPage.mymainObject.bSendRequest) || strStatus != "APPROVED")
                        {
                            if (((strStatus == "DOWNLOADED" || strStatus == "UPLOADED") && MainPage.mymainObject.bDownloadRequest) || (strStatus != "DOWNLOADED" && strStatus != "UPLOADED"))
                            {
                                if (strID != "")
                                    strID += ",";
                                strID += Convert.ToString(dr.Cells["id"].Value);

                                strQuery += " if exists (Select RequestStatus from dbo.[PaymentRequest] Where [RequestStatus] in (" + strOldStatus + ") and ID in (" + dr.Cells["id"].Value + "))  begin INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                         + "('PAYMENTREQUEST','" + dr.Cells["branchCode"].Value + "'," + dr.Cells["id"].Value + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(dr.Cells["netAmt"].Value) + ",'" + MainPage.strLoginName + "',1,0,'" + strStatus + "') end ";
                            }
                        }
                    }
                }
            }

            if (strQuery != "" && strID!="")
            {
                strQuery += " Update PaymentRequest Set [RequestStatus]='" + strStatus + "'  Where [RequestStatus] in (" + strOldStatus + ") and ID in (" + strID + ") ";

                _count = dba.ExecuteMyQuery(strQuery);
            }
            return _count;
        }


        private void ChangeRequestStatus()
        {
            try
            {
                string strStatus = txtStatusChanged.Text;
                int _count = UpdatePaymentRequest(strStatus);
                if (_count > 0)
                {
                    MessageBox.Show("Thank you ! Status changes successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    GetDataFromDB();
                    txtStatusChanged.Clear();
                }
                else
                {
                    MessageBox.Show("Sorry !! Unable to changes status right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
        }

        private void txtStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REQUESTSTATUS", "SEARCH REQUEST STATUS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStatus.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("REQUESTSTATUS", "SEARCH REQUEST STATUS", Keys.Space);
                objSearch.ShowDialog();
                txtStatus.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnParty_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            btnSendRequest.Enabled = false;
            try
            {
                if (ValidateRequest())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to send payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = SendRequest();
                        if (count > 0)
                        {//
                            string strPath = GetAllFilePath(), strSubject = "", strBody = "";
                            strSubject = "PAYMENT REQUEST FROM " + MainPage.strCompanyName + " BY " + MainPage.strLoginName;
                            strBody = "Dear Sir ! \nWe are sending payment request of : " + MainPage.strCompanyName + ", which is attached with this mail, Please consider it.";
                            bool _bStatus = DataBaseAccess.SendEmail("ssspaymentdlh@gmail.com", strSubject, strBody, strPath, "", "PAYMENT REQUEST", true);
                            if (!_bStatus)
                            {
                                SendingEmailPage objEmail = new SendingEmailPage("ssspaymentdlh@gmail.com", strSubject, strBody, strPath, "PAYMENT REQUEST");
                                objEmail.ShowDialog();
                            }
                            GetDataFromDB();
                        }
                        else
                        {
                            MessageBox.Show("Sorry !! Unable to send request right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch { }
            btnSendRequest.Enabled = true;

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                if (CheckAcceptedEntry())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to download payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int _count = UpdatePaymentRequest("DOWNLOADED");
                        if (_count > 0)
                        {
                            string strFileName = GetFileName();
                            string strPath = WriteInExistingFile(strFileName);
                            if (strPath == "")
                                WriteInExistingFile(strFileName);
                            GetDataFromDB();
                        }
                    }
                }
            }
            catch { }
            btnDownload.Enabled = true;
        }

        private bool CheckAcceptedEntry()
        {
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkValue"].Value) && Convert.ToString(row.Cells["requestStatus"].Value) == "APPROVED")
                {
                    return true;
                }
            }
            MessageBox.Show("Sorry ! Only accepted request can be download !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void ReCalculateAmount()
        {
            double dNetAmt = 0, dAmt = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkValue"].EditedFormattedValue))
                    {
                        dAmt = dba.ConvertObjectToDouble(row.Cells["netAmt"].Value);
                        if (Convert.ToString(row.Cells["netStatus"].Value).Trim().ToUpper() == "DR")
                            dAmt = dAmt * -1;
                        dNetAmt += dAmt;
                    }
                }
            }
            catch { }

            lblNetAmt.Text = Math.Abs(dNetAmt).ToString("N2", MainPage.indianCurancy);
            if (dNetAmt >= 0)
                lblNetStatus.Text = "CR";
            else
                lblNetStatus.Text = "DR";
        }

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnBranch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", Keys.Space);
                objSearch.ShowDialog();
                txtBranchCode.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void btnChangeStatus_Click(object sender, EventArgs e)
        {
            try
            {
                btnChangeStatus.Enabled = false;
                if (txtStatusChanged.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to change request status ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ChangeRequestStatus();
                    }
                }
                else
                    MessageBox.Show("Sorry ! Please enter status for changing status !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch { }
            btnChangeStatus.Enabled = true;
        }

        private void txtStatusChanged_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REQUESTSTATUS", "SEARCH REQUEST STATUS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStatusChanged.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnStatusChanged_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("REQUESTSTATUS", "SEARCH REQUEST STATUS", Keys.Space);
                objSearch.ShowDialog();
                txtStatusChanged.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void dgrdDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 9)
                {
                    string strDate = Convert.ToString(dgrdDetails.CurrentCell.EditedFormattedValue);
                    if (strDate != "")
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate, false);
                            if (!txtDate.Text.Contains("/"))
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                if (e.RowIndex < dgrdDetails.Rows.Count - 1)
                                {
                                    dgrdDetails.EndEdit();
                                }
                            }
                            dgrdDetails.CurrentCell.Value = txtDate.Text;
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_Scroll(object sender, ScrollEventArgs e)
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

        private void txtPriority_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REQUESTPRIORITY", "SEARCH REQUEST PRIORITY", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPriority.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void chkPaidDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromPaidDate.Enabled = txtToPaidDate.Enabled = chkPaidDate.Checked;
            txtFromPaidDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToPaidDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtRemark_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnPriority_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("REQUESTPRIORITY", "SEARCH REQUEST PRIORITY", Keys.Space);
                objSearch.ShowDialog();
                txtPriority.Text = objSearch.strSelectedData;
            }
            catch
            {
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

        private void btnUpload_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            try
            {
                if (CheckAcceptedEntry())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to upload payment request ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        UploadEntries();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnUpload.Enabled = true;
        }

        private void UploadEntries()
        {
            int _allCount = 0, payCount = 0 ;
            string strID = "", strStatus = "UPLOADED", strReqStatus = "", strAccountName = "", strBankAccountNo = "", strIFSCCode = "", strTransactionType = "", strUID = "", strRemark = "", strQuery = "";
            double dAmt = 0;
            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(dr.Cells["chkValue"].Value))
                {
                    strReqStatus = Convert.ToString(dr.Cells["requestStatus"].Value);

                    if (((strStatus == "DOWNLOADED" || strStatus == "UPLOADED") && MainPage.mymainObject.bDownloadRequest) || (strStatus != "DOWNLOADED" && strStatus != "UPLOADED"))
                    {
                        strID = Convert.ToString(dr.Cells["id"].Value);

                        strQuery += " if exists (Select RequestStatus from dbo.[PaymentRequest] Where [RequestStatus] in (" + strReqStatus + ") and ID in (" + dr.Cells["id"].Value + "))  begin INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + "('PAYMENTREQUEST','" + dr.Cells["branchCode"].Value + "'," + dr.Cells["id"].Value + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(dr.Cells["netAmt"].Value) + ",'" + MainPage.strLoginName + "',1,0,'" + strStatus + "') "
                                 + " Update PaymentRequest Set [RequestStatus]='" + strStatus + "'  Where [RequestStatus] in (" + strReqStatus + ") and ID in (" + strID + ") end ";
                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count > 0)
                        {
                            _allCount += _count;
                            strUID = "DLH" + strID + MainPage.currentDate.Day.ToString();
                            strAccountName = Convert.ToString(dr.Cells["accountName"].Value);
                            strBankAccountNo = Convert.ToString(dr.Cells["accountNo"].Value).Replace("'", "");
                            strIFSCCode = Convert.ToString(dr.Cells["ifscCode"].Value);
                            strRemark = Convert.ToString(dr.Cells["finalPartyName"].Value);
                            dAmt = dba.ConvertObjectToDouble(dr.Cells["netAmt"]);
                            if (strIFSCCode.Contains("ICIC"))
                            {
                                strTransactionType = "TPA";
                                strIFSCCode = "ICIC0000011";
                            }
                            else if (dAmt < 200000)
                                strTransactionType = "RGS";
                            else
                                strTransactionType = "RTG";

                            string result = BankAPI.TransactionAPI(strAccountName,strBankAccountNo,strIFSCCode,strTransactionType,dAmt,strUID,strRemark);
                            if (result.Contains("succes"))
                                payCount++;
                        }
                    }
                }
            }
            if (_allCount > 0)
            {               
                GetDataFromDB();
            }
        }

        private void txtFromPaidDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkPaidDate.Checked, false, true);
        }
    }
}
