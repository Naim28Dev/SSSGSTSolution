using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class CreditNoteRegister : Form
    {
        DataBaseAccess dba;
        public CreditNoteRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void GoodsReceiveRegister_KeyDown(object sender, KeyEventArgs e)
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

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }       

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH Sundry Creditor", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }
        
        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtGRCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CREDITNOTECODE", "SEARCH CREDIT NOTE CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }    

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill serial no or uncheck serial no box ! ", "Serial no Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);             
                else
                    GetAdvanceSearchedRecord();               
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }


        private void GetAdvanceSearchedRecord()
        {
            try
            {
                string strQuery = "  ", strSubQuery =CreateQuery();
                strQuery = " Select SR.BillCode,SR.BIllNo, (PurchaseBillCode+' '+PurchaseBillNo) as PurchaseBillNo,SR.ID,Convert(varchar,Date,103)BDate,dbo.GetFullName(PurchasePartyID) PurchaseParty,ItemName,Variant1,Amount,(OtherSign+CAST(otherAmt as varchar)) as OtherAmt,taxPer,CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt,TaxAmount as taxAmt,grossAmt,netAmt,createdBy,updatedBy from PurchaseReturn SR inner join PurchaseReturnDetails SRD on SR.BillCode=SRD.BillCode and SR.BillNo=SRD.BillNo OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='CREDITNOTE' and GD.BillCode=SR.BillCode and GD.BillNo=SR.BillNo Group by TaxType) GD Where [EntryType]='CREDITNOTE' and SR.BillCode!='' " + strSubQuery + " Order by SR.BillNo,SR.Date ";

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataWithGrid(dt);                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Advance Searched Record in Purchase Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkSerial.Checked)
                    strQuery += " and (SR.BillNo >= " + txtFromSerialNo.Text + " and SR.BillNo <=" + txtToSerialNo.Text + ") ";

                //if (chkSaleBillNo.Checked)
                //    strQuery += " and (SR.SaleBillNo >= " + txtSaleRBillNoFrom.Text + " and SR.SaleBillNo <=" + txtSaleRBillNoTo.Text + ") ";

                if (txtBillCode.Text != "")
                    strQuery += " and SR.BillCode Like('" + txtBillCode.Text + "') ";

                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    if (txtPurchaseParty.Text != "PERSONAL")
                    {
                        strFullName = txtPurchaseParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery += " and  PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                    }
                    else
                        strQuery += " and  Personal!='' ";
                }               
          
                if (txtItemName.Text != "")
                    strQuery += " and ItemName Like('" + txtItemName.Text + "') ";

                if (txtPurchaseInvoiceNo.Text != "")
                    strQuery += " and ReverseCharge Like('%" + txtPurchaseInvoiceNo.Text + "%') ";
                if (txtRemark.Text != "")
                    strQuery += " and Remark Like('%" + txtRemark.Text + "%') ";

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in  Sale Return Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                double dGrossAmt = 0, dNetAmt = 0;
                dgrdDetails.Rows.Clear();
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(table.Rows.Count);
                        int rowIndex = 0;
                        string strID = "", strOldID = "";
                        foreach (DataRow dr in table.Rows)
                        {
                            strID = Convert.ToString(dr["ID"]);
                            DataGridViewRow row = dgrdDetails.Rows[rowIndex];

                            if (strID != strOldID)
                            {
                                dGrossAmt += dba.ConvertObjectToDouble(dr["GrossAmt"]);
                                dNetAmt += dba.ConvertObjectToDouble(dr["NetAmt"]);

                                row.Cells["date"].Value = dr["Bdate"];                           
                                row.Cells["otherAmt"].Value = dr["otherAmt"];                                                      
                                row.Cells["taxPer"].Value = dr["taxPer"];
                                row.Cells["igstAmt"].Value = dr["igstAmt"];
                                row.Cells["cgstAmt"].Value = dr["cgstAmt"];
                                row.Cells["sgstAmt"].Value = dr["sgstAmt"];
                                row.Cells["taxAmt"].Value = dr["taxAmt"];
                                row.Cells["netAmt"].Value = dr["netAmt"];
                                row.Cells["grossAmt"].Value = dr["grossAmt"];
                                row.Cells["createdBy"].Value = dr["createdBy"];
                                row.Cells["updatedBy"].Value = dr["updatedBy"];
                                strOldID = strID;
                            }
                            else
                                row.Visible = false;
                                            
                            row.Cells["SID"].Value = strID;
                            row.Cells["srBillNo"].Value = dr["BillCode"]+" " + dr["BillNo"];
                            row.Cells["itemName"].Value = dr["itemName"];
                            row.Cells["description"].Value = dr["Variant1"];
                            row.Cells["amount"].Value = dba.ConvertObjectToDouble(dr["Amount"]).ToString("N2",MainPage.indianCurancy); 
                            row.Cells["purchaseParty"].Value = dr["PurchaseParty"];
                            row.Cells["purchaseSno"].Value = dr["PurchaseBillNo"];

                            rowIndex++;
                        }
                    }
                }

                lblGrossAmt.Text = dGrossAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in  Sale Return Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dgrdGoods_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex ==0)                    
                        ShowDetails("CREDITNOTE");
                    else if (e.ColumnIndex == 3)
                        ShowDetails("PURCHASE");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Data Grid View in Show SALES RETURN Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowDetails(string strAccount)
        {
            try
            {
                string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);               
                if (strAccount == "CREDITNOTE")
                {
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {                       
                            CreditNote_Supplier objSale = new CreditNote_Supplier(strNumber[0], strNumber[1]);
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();                        
                    }
                }
                else if (strAccount == "PURCHASE")
                {
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        GoodscumPurchase objGoodscumPurchase = new GoodscumPurchase(strNumber[0], strNumber[1]);
                        objGoodscumPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objGoodscumPurchase.ShowInTaskbar = true;
                        objGoodscumPurchase.Show();
                    }
                }
            }
            catch
            {
            }
        }


        private void dgrdGoods_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdGoods_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdDetails.CurrentRow.Index;
                    if (dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    int columnIndex = dgrdDetails.CurrentCell.ColumnIndex, rowIndex = dgrdDetails.CurrentRow.Index;
                    if (rowIndex >= 0)
                    {
                        if (columnIndex == 0)
                            ShowDetails("CREDITNOTE");
                        else if (columnIndex == 3)
                            ShowDetails("PURCHASE");
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Data Grid View in Show Goods Received Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        btnPrint.Enabled = false;
        //        DataTable dt = CreateDataTable();
        //        if (dt.Rows.Count > 0)
        //        {
        //            Reporting.GoodsReceiveReport report = new Reporting.GoodsReceiveReport();
        //            report.SetDataSource(dt);
        //            report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
        //            report.PrintToPrinter(1, false, 0, 0);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnPrint.Enabled = true;
        //}

        //private void btnPreview_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        btnPreview.Enabled = false;
        //        if (dgrdDetails.Rows.Count > 0)
        //        {
        //            DataTable dt = CreateDataTable();
        //            if (dt.Rows.Count > 0)
        //            {
        //                Reporting.ShowReport objShow = new Reporting.ShowReport("Report Summary Preview");
        //                Reporting.GoodsReceiveReport report = new Reporting.GoodsReceiveReport();
        //                report.SetDataSource(dt);
        //                objShow.myPreview.ReportSource = report;
        //                objShow.Show();
        //            }

        //        }
        //        else
        //        {
        //            MessageBox.Show("No Record for Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnPreview.Enabled = true;
        //}

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("Party", typeof(String));
                myDataTable.Columns.Add("IColumn", typeof(String));
                myDataTable.Columns.Add("IIColumn", typeof(String));
                myDataTable.Columns.Add("IIIColumn", typeof(String));
                myDataTable.Columns.Add("IVColumn", typeof(String));
                myDataTable.Columns.Add("VColumn", typeof(String));
                myDataTable.Columns.Add("VIColumn", typeof(String));
                myDataTable.Columns.Add("VIIColumn", typeof(String));
                myDataTable.Columns.Add("VIIIColumn", typeof(String));
                myDataTable.Columns.Add("IXColumn", typeof(String));
                myDataTable.Columns.Add("XColumn", typeof(String));
                myDataTable.Columns.Add("XIColumn", typeof(String));
                myDataTable.Columns.Add("XIIColumn", typeof(String));
                myDataTable.Columns.Add("XIIIColumn", typeof(String));
                myDataTable.Columns.Add("XIVColumn", typeof(String));
                myDataTable.Columns.Add("XVColumn", typeof(String));
                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IXColumnValue", typeof(String));
                myDataTable.Columns.Add("XColumnValue", typeof(String));
                myDataTable.Columns.Add("XIColumnValue", typeof(String));
                myDataTable.Columns.Add("XIIColumnValue", typeof(String));
                myDataTable.Columns.Add("XIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("XIVColumnValue", typeof(String));
                myDataTable.Columns.Add("XVColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPeti", typeof(String));
                myDataTable.Columns.Add("TotalCartoon", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    row["Party"] = "";

                    for (int i = 0; i < dgrdDetails.Columns.Count; i++)
                    {
                        row[i + 3] = dgrdDetails.Columns[i].HeaderText;
                        row[i + 18] = dr.Cells[i].Value;
                        if (i == 14)
                        {
                            i = dgrdDetails.Columns.Count;
                        }
                    }

                    row["TotalPeti"] = "";
                    row["TotalCartoon"] = "";
                    //row["TotalPieces"] = lblPcs.Text;
                    //row["TotalAmount"] = lblAmount.Text;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        //private void btnExport_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgrdDetails.Rows.Count > 0)
        //        {
        //            btnExport.Enabled = false;
        //            DataTable dt = CreateDataTable();
        //            if (dt.Rows.Count > 0)
        //            {
        //                Reporting.GoodsReceiveReport report = new Reporting.GoodsReceiveReport();
        //                report.SetDataSource(dt);
        //                CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
        //                objViewer.ReportSource = report;
        //                objViewer.ExportReport();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnExport.Enabled = true;
        //}       

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSerialNo.ReadOnly = txtToSerialNo.ReadOnly = !chkSerial.Checked;
            txtFromSerialNo.Text = txtToSerialNo.Text = "";
        }
        
        private void btnExpand_Click(object sender, EventArgs e)
        {
            btnExpand.Enabled = false;
            try
            {
                if (btnExpand.Text == "Expand")
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        if (!row.Visible)
                            row.Visible = true;
                    }
                    btnExpand.Text = "Collapse";
                }
                else
                {
                    string strID = "", strNewID = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strID = Convert.ToString(row.Cells["SID"].Value);
                        if (strNewID != "")
                        {
                            if (strID == strNewID)
                                row.Visible = false;
                        }
                        strNewID = strID;
                    }
                    btnExpand.Text = "Expand";
                }
            }
            catch
            {
            }
            btnExpand.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                        {
                            if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdDetails.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Credit_Note_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);


                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void CreditNoteRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bPurchaseReport)
                    dba.EnableCopyOnClipBoard(dgrdDetails);
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }
        }
    }
}
