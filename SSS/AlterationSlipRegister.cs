using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace SSS
{
    public partial class AlterationSlipRegister : Form
    {
        DataBaseAccess DbAcess;
        SendSMS sendMessage;
        DataTable dtReport = null, dtMaster = null;
        int masterRowIndex = 0, masterColumnIndex = 0;
        MaskedTextBox maskedTxtBox;
        //string strAltSerialCode = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select distinct AltrationCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'"));

        ArrayList aList;
        public AlterationSlipRegister()
        {

            InitializeComponent();
            DbAcess = new DataBaseAccess();
            sendMessage = new SendSMS();
            SetDatePeriod();
            aList = new ArrayList();
        }

        private void SetDatePeriod()
        {
            try
            {
                txtDStartDate.Text = txtStartDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtDEndDate.Text = txtEndDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

                dtMaster = DbAcess.GetDataTable("Select (AreaCode+AccountNo+' '+Name)MASTERNAME from SupplierMaster Where TINNumber in ('MASTER') OR Category in ('MASTER') Order by Name");
                if (dtMaster.Rows.Count > 0)
                {
                    dgrdMaster.Rows.Add(dtMaster.Rows.Count);
                    for (int i = 0; i < dtMaster.Rows.Count; ++i)
                    {
                        dgrdMaster.Rows[i].Cells[0].Value = (Boolean)false;
                        dgrdMaster.Rows[i].Cells[1].Value = dtMaster.Rows[i]["Name"];
                    }
                }
            }
            catch
            {
            }
        }

        private void SearchQueryData()
        {
            string strQuery = "";
            try
            {
                strQuery = "Select *,ass.ID as SID,ass.Remark as OldRemark from AlterationSlip aSlip inner join AlterationSlipSecondary ass on aSlip.SerialNo=ass.SerialNo and aSlip.SerialCode=ass.SerialCode  Where aSlip.SerialNo!=0  "; // and aSlip.SerialCode='"+ strAltSerialCode + "'

                string strSubQuery = CreateQuery();
                if (strQuery != "")
                {
                    strQuery += strSubQuery;
                }
                strQuery += "  Order by aSlip.Date, aSlip.SerialNo";
                dtReport = DbAcess.GetDataTable(strQuery);
                BindGridViewData(dtReport);
                //CalculateTotal();
            }
            catch { }
        }

        private void SearchQuickSearchData(string strHead)
        {
            try
            {
                if (dtReport != null)
                {
                    //if (strHead=="MOBILE" && txtMobileNo.Text != "")
                    //{
                    //    string strQuery = " MobileNoI Like ('%" + txtMobileNo.Text + "%') Or MobileNoII Like ('%" + txtMobileNo.Text + "%') ";

                    //    DataRow[] rows = dtReport.Select(String.Format(strQuery));
                    //    BindGridViewDataWithDataRow(rows);
                    //}
                    //else if (strHead == "ALTERATIONNO" && txtAltNo.Text != "")
                    //{
                    //    string strQuery = " AltNo >=" + txtAltNo.Text + "";

                    //    DataRow[] rows = dtReport.Select(String.Format(strQuery));
                    //    BindGridViewDataWithDataRow(rows);
                    //}
                    //else
                    //{
                    //    BindGridViewData(dtReport);
                    //}
                }
            }
            catch
            {
            }
        }

        //using statment Bind Grid View Data According to Table Result...
        private void BindGridViewData(DataTable dtRecord)
        {
            try
            {
                double dTQty = 0;
                dgrdStockReport.Rows.Clear();
                if (dtRecord.Rows.Count > 0)
                {
                    int sno = 0;
                    dgrdStockReport.Rows.Add(dtRecord.Rows.Count);
                    foreach (DataRow drow in dtRecord.Rows)
                    {
                        double dQty = Convert.ToDouble(drow["Qty"]), dPAmt = ConvertObjectToDouble(drow["PendingAmt"]);
                        string strSerialNo = Convert.ToString(drow["SerialCode"]) + " " + Convert.ToString(drow["SerialNo"]);
                        dTQty += dQty;
                        dgrdStockReport.Rows[sno].Cells["chkStatus"].Value = false;
                        dgrdStockReport.Rows[sno].Cells["SNo"].Value = drow["SerialNo"];
                        dgrdStockReport.Rows[sno].Cells["SCode"].Value = drow["SerialCode"];
                        dgrdStockReport.Rows[sno].Cells["altID"].Value = drow["SID"];
                        dgrdStockReport.Rows[sno].Cells["SerialNo"].Value = strSerialNo;
                        dgrdStockReport.Rows[sno].Cells["SrDate"].Value = Convert.ToDateTime(drow["Date"]).ToString("dd/MM/yyyy");
                        dgrdStockReport.Rows[sno].Cells["altCode"].Value = drow["AltCode"];
                        dgrdStockReport.Rows[sno].Cells["altNo"].Value = drow["AltNo"];
                        dgrdStockReport.Rows[sno].Cells["billNo"].Value = drow["BillNo"];
                        dgrdStockReport.Rows[sno].Cells["orderNo"].Value = drow["OrderNo"];
                        dgrdStockReport.Rows[sno].Cells["mobileNoI"].Value = drow["MobileNoI"];
                        dgrdStockReport.Rows[sno].Cells["dDate"].Value = Convert.ToDateTime(drow["DDate"]).ToString("dd/MM/yyyy");
                        dgrdStockReport.Rows[sno].Cells["ItemName"].Value = drow["ItemName"];
                        dgrdStockReport.Rows[sno].Cells["Quantity"].Value = dQty;
                        dgrdStockReport.Rows[sno].Cells["pendingAmt"].Value = dPAmt.ToString("N0", MainPage.indianCurancy);
                        string strStatus = Convert.ToString(drow["ItemStatus"]);
                        dgrdStockReport.Rows[sno].Cells["altStatus"].Value = drow["ItemStatus"];
                        if (strStatus == "READY")
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                        else if (strStatus == "DELIVERED")
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                        }
                        else if (strStatus == "REALTER")
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.Tomato;
                            dgrdStockReport.Rows[sno].Cells[0].ReadOnly = true;
                        }
                        else if (strStatus == "PENDING" && Convert.ToString(drow["AltType"]) == "FINISHING")
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.Turquoise;
                        }
                        string strRemark = Convert.ToString(drow["OldRemark"]);
                        if (strRemark.Contains("HOLD") || strStatus == "HOLD")
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.Gold;
                        }

                        if (dPAmt > 0)
                        {
                            dgrdStockReport.Rows[sno].DefaultCellStyle.BackColor = Color.Violet;
                        }

                        dgrdStockReport.Rows[sno].Cells["remark"].Value = strRemark;
                        dgrdStockReport.Rows[sno].Cells["mobileNoII"].Value = drow["MobileNoII"];
                        dgrdStockReport.Rows[sno].Cells["gatePassNo"].Value = drow["GatePassNo"];
                        dgrdStockReport.Rows[sno].Cells["name"].Value = drow["PersonName"];
                        dgrdStockReport.Rows[sno].Cells["mobileNo"].Value = drow["MobileNo"];
                        dgrdStockReport.Rows[sno].Cells["measurementMaster"].Value = drow["MasterName"];
                        dgrdStockReport.Rows[sno].Cells["salesMan"].Value = drow["SalesManName"];
                        dgrdStockReport.Rows[sno].Cells["altType"].Value = drow["AltType"];
                        dgrdStockReport.Rows[sno].Cells["timing"].Value = drow["Timing"];
                        dgrdStockReport.Rows[sno].Cells["customerName"].Value = drow["CustomerName"];
                        string strReadyDate = Convert.ToString(drow["ReadyDate"]);
                        if (strReadyDate != "")
                        {
                            dgrdStockReport.Rows[sno].Cells["readyDate"].Value = Convert.ToDateTime(strReadyDate).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dgrdStockReport.Rows[sno].Cells["readyDate"].Value = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                        if (Convert.ToString(drow["Pcs"]) != "0")
                        {
                            dgrdStockReport.Rows[sno].Cells["pcs"].Value = drow["Pcs"];
                        }


                        sno++;
                    }
                }
                lblQty.Text = dTQty.ToString("0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //using statment Create Query 
        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                //searching qury using Date Wise..
                if (chkDate.Checked && txtStartDate.Text.Length > 9 && txtEndDate.Text.Length > 9)
                {
                    DateTime strDate, endDate;
                    strDate = DbAcess.ConvertDateInExactFormat(txtStartDate.Text);
                    endDate = DbAcess.ConvertDateInExactFormat(txtEndDate.Text).AddDays(1);
                    strQuery = " and (aSlip.Date >='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and aSlip.Date<'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "')  ";

                }

                if (chkDDate.Checked && txtDStartDate.Text.Length > 9 && txtDEndDate.Text.Length > 9)
                {
                    DateTime strDate, endDate;
                    strDate = DbAcess.ConvertDateInExactFormat(txtDStartDate.Text);
                    endDate = DbAcess.ConvertDateInExactFormat(txtDEndDate.Text).AddDays(1);
                    strQuery = " and (aSlip.DDate >='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and aSlip.DDate<'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "')  ";
                }

                if (chkSNo.Checked && txtFromSNo.Text != "" && txtToSNo.Text != "")
                    strQuery = " and (aSlip.SerialNo >=" + txtFromSNo.Text + " and aSlip.SerialNo<=" + txtToSNo.Text + ")  ";

                if (txtAltCode.Text != "")
                {
                    strQuery += " and aSlip.AltCode='" + txtAltCode.Text + "' ";
                }
                if (txtBillCode.Text != "")
                {
                    strQuery += " and aSlip.SerialCode='" + txtBillCode.Text + "' ";
                }

                if (txtAltNo.Text != "")
                {

                    strQuery += " and aSlip.AltNo='" + txtAltNo.Text + "' ";
                }

                if (txtBillNo.Text != "")
                {
                    strQuery += " and aSlip.BillNo Like('" + txtBillNo.Text + "%') ";
                }

                if (txtItem.Text != "")
                {
                    strQuery += " and ass.ItemName='" + txtItem.Text + "' ";
                }

                if (txtOrderNo.Text != "")
                {
                    strQuery += " and aSlip.OrderNo Like ('" + txtOrderNo.Text + "%') ";
                }

                if (txtMobileNo.Text != "")
                {
                    strQuery += " and (aSlip.MobileNoI Like ('%" + txtMobileNo.Text + "%') Or aSlip.MobileNoII Like ('%" + txtMobileNo.Text + "%')) ";
                }

                if (rdoPending.Checked)
                {
                    strQuery += " and ass.ItemStatus='PENDING' ";
                }
                else if (rdoReady.Checked)
                {
                    strQuery += " and ass.ItemStatus='READY' ";
                }
                else if (rdoDelivered.Checked)
                {
                    strQuery += " and ass.ItemStatus='DELIVERED' ";
                }
                else if (rdoReAlter.Checked)
                {
                    strQuery += " and ass.ItemStatus='REALTER' ";
                }
                else if (rdoCancel.Checked)
                {
                    strQuery += " and ass.ItemStatus='CANCEL' ";
                }
                else if (rdoExchange.Checked)
                {
                    strQuery += " and ass.ItemStatus='EXCHANGE' ";
                }
                else if (rdoHold.Checked)
                {
                    strQuery += " and (ass.Remark Like ('%HOLD%') OR ass.ItemStatus='HOLD') ";
                }
                else if (rdoPaymentPending.Checked)
                {
                    strQuery += " and aSlip.PendingAmt>0 ";
                }

                if (rdoTypeAlteration.Checked)
                {
                    strQuery += " and ass.AltType='ALTERATION' ";
                }
                else if (rdoTypeFinish.Checked)
                {
                    strQuery += " and ass.AltType='FINISHING' ";
                }
                else if (rdoTypeReady.Checked)
                {
                    strQuery += " and ass.AltType='READY' ";
                }

            }
            catch { }

            return strQuery;
        }

        private void CalculateTotal()
        {
            try
            {
                double DTotalQnty = 0;

                foreach (DataGridViewRow dr in dgrdStockReport.Rows)
                {
                    try
                    {
                        DTotalQnty += Convert.ToDouble(dr.Cells["Quantity"].Value);
                    }
                    catch { }
                }

                lblQty.Text = DTotalQnty.ToString("0.00");

            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Calculate value in DataGridView", ex.Message };
                //DataBaseObj.ErrorReport(StrReport);
            }
        }


        //using statment Clear Controls Value..
        private void ClearRecord()
        {
            try
            {
                txtAltCode.Clear();
                txtAltNo.Clear();
                txtOrderNo.Clear();
                txtBillNo.Clear();
                txtMobileNo.Clear();
                txtItem.Clear();
                //rdoAll.Checked = true;
                //chkDate.Checked = false;
                //txtStartDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                //txtEndDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (chkDate.Checked && (txtStartDate.Text.Length != 10 || txtEndDate.Text.Length != 10))
            {
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (chkDDate.Checked && (txtDStartDate.Text.Length != 10 || txtDEndDate.Text.Length != 10))
            {
                MessageBox.Show(" Sorry ! Please fill Delivary Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                btnGo.Enabled = false;
                SearchQueryData();
                btnGo.Enabled = true;
                ClearRecord();
            }
        }

        private void StockTransferRegister_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlMaster.Visible)
                    {
                        pnlMaster.Visible = false;
                    }
                    else if (panelSMS.Visible)
                    {
                        panelSMS.Visible = false;
                    }
                    else if (pnlColor.Visible)
                    {
                        pnlColor.Visible = false;
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else if (e.KeyCode == Keys.Enter && !dgrdStockReport.Focused)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch { }
        }

        //using statment Create Data Table for Reporting..
        private DataTable CreateDatatable()
        {

            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));
                myDataTable.Columns.Add("SerialNo", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("AltNo", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("OrderNo", typeof(String));
                myDataTable.Columns.Add("DDate", typeof(String));
                myDataTable.Columns.Add("ItemName", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("Status", typeof(String));
                myDataTable.Columns.Add("MobileNo", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));


                foreach (DataGridViewRow row in dgrdStockReport.Rows)
                {
                    DataRow dRow = myDataTable.NewRow();



                    dRow["CompanyName"] = MainPage.strCompanyName;
                    dRow["HeaderName"] = "ALTERATION REPORT";
                    dRow["SerialNo"] = row.Cells["SerialNo"].Value;
                    dRow["Date"] = row.Cells["SrDate"].Value;
                    dRow["AltNo"] = row.Cells["altCode"].Value + " " + row.Cells["altNo"].Value;
                    dRow["BillNo"] = row.Cells["billNo"].Value;
                    dRow["OrderNo"] = row.Cells["orderNo"].Value;
                    dRow["DDate"] = row.Cells["dDate"].Value;
                    dRow["ItemName"] = row.Cells["ItemName"].Value;
                    dRow["Qty"] = row.Cells["Quantity"].Value;
                    dRow["Status"] = row.Cells["altStatus"].Value;
                    dRow["MobileNo"] = row.Cells["mobileNoI"].Value;
                    dRow["TotalQty"] = lblTotalQnty.Text;

                    myDataTable.Rows.Add(dRow);
                }
                if (myDataTable.Rows.Count > 0)
                {
                    myDataTable.Rows[0]["CompanyName"] = MainPage.strCompanyName;
                    if (rdoPending.Checked)
                    {
                        myDataTable.Rows[0]["HeaderName"] = " PENDING ALTERATION REPORT";
                    }
                    else if (rdoReady.Checked)
                    {
                        myDataTable.Rows[0]["HeaderName"] = " READY ALTERATION REPORT";
                    }
                    else if (rdoReAlter.Checked)
                    {
                        myDataTable.Rows[0]["HeaderName"] = " RE-ALTER ALTERATION REPORT";
                    }
                    else if (rdoDelivered.Checked)
                    {
                        myDataTable.Rows[0]["HeaderName"] = " DELIVERED ALTERATION REPORT";
                    }
                    else if (rdoExchange.Checked)
                    {
                        myDataTable.Rows[0]["HeaderName"] = " EXCHANGE ALTERATION REPORT";
                    }
                    else
                    {
                        myDataTable.Rows[0]["HeaderName"] = " ALTERATION REPORT";
                    }
                    myDataTable.Rows[myDataTable.Rows.Count - 1]["TotalQty"] = lblQty.Text;
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        //using statment Print Stock Report..
        private void PrintReport()
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dtCrystalReport = CreateDatatable();
                if (dtCrystalReport.Rows.Count > 0)
                {
                    Reporting.AlterationReport objAlterationReport = new Reporting.AlterationReport();
                    objAlterationReport.SetDataSource(dtCrystalReport);
                   
                    if (MainPage._PrintWithDialog)
                        DbAcess.PrintWithDialog(objAlterationReport);
                    else
                    {
                        objAlterationReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        objAlterationReport.PrintToPrinter(1, false, 0, 0);
                    }
                    objAlterationReport.Close();
                    objAlterationReport.Dispose();
                }
                else
                {
                    MessageBox.Show("Sorry ! No record found to Print... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                picBox.Visible = true;
                PrintReport();
            }
            catch { }
            btnPrint.Enabled = true;
            picBox.Visible = false;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                picBox.Visible = true;

                DataTable dt = CreateDatatable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("Alteration Report");
                    Reporting.AlterationReport objAlterationReport = new Reporting.AlterationReport();
                    objAlterationReport.SetDataSource(dt);
                    objReport.myPreview.ReportSource = objAlterationReport;
                    objReport.Show();

                    objAlterationReport.Close();
                    objAlterationReport.Dispose();
                }
                else
                {
                    MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
            picBox.Visible = false;
        }

        //private void btnCreatepdf_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgrdStockReport.RowCount > 0)
        //        {                   
        //           // btnCreatepdf.Enabled = false;

        //            string strRoot = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString(), strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PDF Files\\" + strRoot, strFileName = strPath + "\\" + strRoot + ".pdf";
        //            Directory.CreateDirectory(strPath);
        //            FileInfo file = new FileInfo(strFileName);
        //            if (file.Exists)
        //            {
        //                file.Delete();
        //            }
        //            DataTable table = CreateDatatable();
        //            Reporting.StockTransferRegister objTrnasfer = new global::SSS.Reporting.StockTransferRegister();
        //            objTrnasfer.SetDataSource(table);
        //            objTrnasfer.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);                 
        //            MessageBox.Show("PDF  Created Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Sorry ! An Error occurred Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //   // btnCreatepdf.Enabled = true;
        //}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdStockReport_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 17 && e.RowIndex >= 0)
                {
                    if (!Convert.ToBoolean(dgrdStockReport.CurrentRow.Cells[0].Value))
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        Rectangle rect = dgrdStockReport.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                        maskedTxtBox.Visible = true;

                        maskedTxtBox.Mask = "00/00/0000";
                        maskedTxtBox.Location = rect.Location;
                        maskedTxtBox.Size = rect.Size;
                        maskedTxtBox.Font = new Font("Arial", 10);
                        maskedTxtBox.Text = "";

                        var cellValue = dgrdStockReport[e.ColumnIndex, e.RowIndex].Value;
                        if (cellValue != null)
                        {
                            maskedTxtBox.Text = cellValue.ToString();
                            //maskedTxtBox.Select();
                        }
                    }
                    maskedTxtBox.Focus();
                }
                else if (e.ColumnIndex > 13 && e.ColumnIndex != 16 && e.ColumnIndex < dgrdStockReport.ColumnCount - 5)
                {
                    if (!Convert.ToBoolean(dgrdStockReport.CurrentRow.Cells[0].Value))
                    {
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 14)
                    {
                        SearchData objSearch = new SearchData("ALTSTATUS", "SELECT STATUS", Keys.Space);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            dgrdStockReport.CurrentRow.Cells["altStatus"].Value = strData;
                        }
                        e.Cancel = true;
                    }

                }
                else if (e.ColumnIndex == 9 || e.ColumnIndex == 10)
                {
                    e.Cancel = false;
                }
                else if (e.ColumnIndex != 0)
                {
                    e.Cancel = true;
                }

            }
            catch
            {
            }
        }

        private void dgrdStockReport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdStockReport.CurrentCell.ColumnIndex == 1 && dgrdStockReport.CurrentRow.Index >= 0)
                    {
                        string strSerialNo = Convert.ToString(dgrdStockReport.CurrentRow.Cells["SNo"].Value);
                        string strSerialCode = Convert.ToString(dgrdStockReport.CurrentRow.Cells["SCode"].Value);
                        if (strSerialNo != "")
                        {
                            AlterationSlip objAlterationSlip = new AlterationSlip(strSerialCode, strSerialNo);
                            objAlterationSlip.ShowDialog();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdStockReport.CurrentRow.Index;
                    if (dgrdStockReport.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdStockReport.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdStockReport.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    int columnIndex = dgrdStockReport.CurrentCell.ColumnIndex;
                    if (columnIndex > 13 && columnIndex != 18 && columnIndex < dgrdStockReport.ColumnCount - 5)
                    {
                        if (Convert.ToBoolean(dgrdStockReport.CurrentRow.Cells[0].Value))
                        {
                            dgrdStockReport.CurrentCell.Value = "";
                        }
                    }
                }
            }
            catch { }
        }

        private void dgrdStockReport_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3 && e.RowIndex >= 0)
                {
                    string strSerialNo = Convert.ToString(dgrdStockReport.CurrentRow.Cells["SNo"].Value);
                    string strSerialCode = Convert.ToString(dgrdStockReport.CurrentRow.Cells["SCode"].Value);
                    if (strSerialNo != "")
                    {
                        AlterationSlip objAlterationSlip = new AlterationSlip(strSerialCode, strSerialNo);
                        objAlterationSlip.FormBorderStyle = FormBorderStyle.FixedDialog;
                        objAlterationSlip.ShowDialog();
                    }
                }
                if (e.ColumnIndex == 7 && e.RowIndex >= 0)
                {
                    string str = Convert.ToString(dgrdStockReport.CurrentRow.Cells["billNo"].Value);
                    string[] strCodeNo = str.Split(' ');
                    if (strCodeNo.Length > 1)
                    {
                        DbAcess.ShowTransactionBook("SALES", strCodeNo[0], strCodeNo[1]);
                    }
                }
            }
            catch { }
        }

        //using statment Validation on Text Box..
        private void KeyPointHandler(KeyPressEventArgs e)
        {
            try
            {
                Char PressedKey = e.KeyChar;
                if (Char.IsLetter(PressedKey) || Char.IsSeparator(PressedKey) || Char.IsPunctuation(PressedKey) || Char.IsSymbol(PressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            catch { }
        }

        private void txtSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            DbAcess.KeyHandlerPoint(sender, e, 0);
        }

        private void txtSerialNo1_KeyPress(object sender, KeyPressEventArgs e)
        {
            DbAcess.KeyHandlerPoint(sender, e, 0);
        }

        private void txtTFSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt.Text.Length == 0 && Char.IsWhiteSpace(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch { }
        }

        private bool ValidationControl()
        {
            bool status = true;
            foreach (DataGridViewRow rows in dgrdStockReport.Rows)
            {
                double dqty, dPCS;
                dqty = ConvertObjectToDouble(rows.Cells["Quantity"].Value);
                dPCS = ConvertObjectToDouble(rows.Cells["pcs"].Value);
                if (dPCS > dqty)
                {
                    MessageBox.Show("Sorry ! PCSs can't be greater than Quantity ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdStockReport.CurrentCell = rows.Cells["pcs"];
                    dgrdStockReport.Focus();
                    return false;

                }

            }
            return status;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            dgrdStockReport.EndEdit();
            if (ValidationControl())
            {
                DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int count = UpdateRecord();
                    if (count > 0)
                    {
                        //DialogResult dResult = MessageBox.Show("Are you want to send SMS for updates ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        //if (dResult == DialogResult.Yes)
                        //{
                        //    SendSMSForUpdate();
                        //}
                        MessageBox.Show("Thank you ! Record updated successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        SearchQueryData();
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Record not updated , Please try again ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private double ConvertObjectToDouble(object objAmt)
        {
            double dAmount = 0;
            try
            {
                if (objAmt != null)
                {
                    dAmount = Convert.ToDouble(objAmt);
                }
            }
            catch
            {
            }
            return dAmount;
        }

        private int UpdateRecord()
        {
            string strQuery = "";
            aList.Clear();
            foreach (DataGridViewRow row in dgrdStockReport.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    string strID = Convert.ToString(row.Cells["altID"].Value), strGatePassNo = Convert.ToString(row.Cells["gatePassNo"].Value), strName = Convert.ToString(row.Cells["name"].Value), strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value), strPcs = Convert.ToString(row.Cells["pcs"].Value), strRemark = Convert.ToString(row.Cells["remark"].Value), strStatus = Convert.ToString(row.Cells["altStatus"].Value);
                    string strSubQuery = "", strCustMo = Convert.ToString(row.Cells["mobileNoI"].Value), strReadyDate = Convert.ToString(row.Cells["readyDate"].Value);
                    if (strGatePassNo.Trim() != "")
                    {
                        if (strName.Trim() == "" || strMobileNo.Trim() == "" || strPcs.Trim() == "" || strPcs.Trim() == "0")
                        {
                            MessageBox.Show("Sorry ! Please fill Name,Mobile No & pcs in Gate pass Delievery ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                    }

                    if (strReadyDate.Length==10)
                    {
                        DateTime dDate = DbAcess.ConvertDateInExactFormat(strReadyDate);
                        strSubQuery = " , ReadyDate='" + dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
                    }
                    else
                    {
                        strSubQuery = " , ReadyDate=NULL ";
                    }
                    strQuery += " Update AlterationSlip Set MobileNoI='" + strCustMo + "', PendingAmt=" + ConvertObjectToDouble(row.Cells["pendingAmt"].Value) + " Where SerialNo=" + row.Cells["SNo"].Value + " and SerialCode='" + row.Cells["SCode"].Value + "'";
                    strQuery += " Update AlterationSlipSecondary Set ItemStatus='" + strStatus + "',Remark='" + strRemark + "',GatePassNo='" + strGatePassNo + "',PersonName='" + strName + "',MobileNo='" + strMobileNo + "',Pcs='" + strPcs + "' " + strSubQuery + "  Where ID=" + strID + " and SerialNo=" + row.Cells["SNo"].Value + "  and SerialCode='" + row.Cells["SCode"].Value + "'";
                    CreateSMSForUpdate(strID, strStatus, strCustMo);
                }
            }
            int count = 0;
            if (strQuery != "")
            {
                count = DbAcess.ExecuteMyQuery(strQuery);
            }
            return count;
        }

        private void CreateSMSForUpdate(string strID, string strStatus, string strMobileNo)
        {
            try
            {
                if (strID != "" && strStatus != "DELIVERED")
                {
                    DataRow[] row = dtReport.Select(String.Format(" SID=" + strID));
                    if (row.Length > 0)
                    {
                        string strOldStatus = Convert.ToString(row[0]["ItemStatus"]);
                        string[] strData = { "", "" };
                        if (strStatus != strOldStatus)
                        {
                            SMS();
                            if (txtSMS.Text == "")
                            {
                                MessageBox.Show("Please type SMS...");
                                txtSMS.Focus();
                            }
                            else
                            {
                                if (strStatus == "READY")
                                {
                                    object objSMS = Convert.ToString(txtSMS.Text);
                                    strData[0] = strMobileNo;
                                    strData[1] = Convert.ToString(objSMS);

                                }
                                else if (strStatus == "PENDING")
                                {
                                    object objSMS = Convert.ToString(txtSMS.Text);
                                    strData[0] = strMobileNo;
                                    strData[1] = Convert.ToString(objSMS);
                                }
                            }
                        }

                        if (strData[0] != "" && strData[1] != "")
                        {
                            aList.Add(strData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendSMSForUpdate()
        {
            try
            {
                int count = 0;
                string strMobileNo = "";
                for (int i = 0; i < aList.Count; i++)
                {
                    string[] strData = aList[i] as string[];
                    if (strData.Length > 0)
                    {
                        if (!strMobileNo.Contains(strData[0]))
                        {
                            string strResult = sendMessage.SendSingleSMS(strData[1], strData[0]);
                            if (strResult != "")
                            {
                                count++;
                                strMobileNo += strData[0];
                            }
                        }
                    }
                }

                if (count > 0)
                {
                    MessageBox.Show("Thanks you ! SMS send successfully ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    aList.Clear();
                }
            }
            catch
            {
            }
        }

        private void dgrdStockReport_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int columnIndex = dgrdStockReport.CurrentCell.ColumnIndex;

            if (columnIndex == 9 || columnIndex == 10 || columnIndex == 13 || columnIndex == 14 || columnIndex == 17 || columnIndex == 20 || columnIndex == 21 || columnIndex == 22 || columnIndex == 23)
            {
                TextBox txtBox = e.Control as TextBox;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int columnIndex = dgrdStockReport.CurrentCell.ColumnIndex;
            if (columnIndex == 13 || columnIndex == 20)
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            else if (columnIndex == 9 || columnIndex == 10 || columnIndex == 14 || columnIndex == 17 || columnIndex == 18 || columnIndex == 21 || columnIndex == 22 || columnIndex == 23)
            {
                Char pressedKey = e.KeyChar;
                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            DbAcess.GetDateInExactFormat(sender, false, false, true);
        }

        private void txtMobileNo_TextChanged(object sender, EventArgs e)
        {
            //if (txtMobileNo.Text.Length > 3)
            //{
            SearchQueryData();
            //}           
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32)
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItem.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void dgrdMaster_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = true;
            }
        }

        private void dgrdMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgrdMaster.CurrentCell = dgrdMaster.CurrentRow.Cells[dgrdMaster.CurrentCell.ColumnIndex + 1];
                    AddMasterToSelectedGrid();
                }
                else if (Char.IsLetter((char)e.KeyCode))
                {
                    if (dtMaster != null)
                    {
                        SearchAltItemByKey(e.KeyCode.ToString());
                    }
                }
            }
            catch
            {
            }
        }

        private void SearchAltItemByKey(string strKey)
        {
            try
            {
                DataRow[] fileterrow = dtMaster.Select(String.Format("Name Like('" + strKey + "%') "));
                if (fileterrow.Length > 0)
                {
                    int index = dtMaster.Rows.IndexOf(fileterrow[0]);
                    dgrdMaster.CurrentCell = dgrdMaster.Rows[index].Cells[0];
                    dgrdMaster.FirstDisplayedCell = dgrdMaster.CurrentCell;
                }
                else
                {
                    dgrdMaster.CurrentCell = dgrdMaster.Rows[0].Cells[0];
                }
            }
            catch
            {
            }
        }

        private void AddMasterToSelectedGrid()
        {
            try
            {
                pnlMaster.Visible = false;
                dgrdMaster.EndEdit();
                string strMaster = "";
                int count = 0;
                foreach (DataGridViewRow dr in dgrdMaster.Rows)
                {
                    Boolean chk = Convert.ToBoolean(dr.Cells[0].Value);

                    if (chk)
                    {
                        if (strMaster == "")
                        {
                            strMaster = Convert.ToString(dr.Cells[1].Value);
                        }
                        else
                        {
                            strMaster += "," + Convert.ToString(dr.Cells[1].Value);
                        }
                        count++;
                    }
                    dr.Cells[0].Value = false;
                }
                if (strMaster == "")
                {
                    pnlMaster.Visible = true;
                    dgrdMaster.Focus();
                    MessageBox.Show("Please select Atleast 1 Master Name  !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (count > 1)
                {
                    pnlMaster.Visible = true;
                    dgrdMaster.Focus();
                    MessageBox.Show("Please select only one Master Name at a time !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    dgrdStockReport.Rows[masterRowIndex].Cells[masterColumnIndex].Value = strMaster;
                    dgrdStockReport.Focus();
                }
            }
            catch
            {
            }
        }

        private void btnMasterAdd_Click(object sender, EventArgs e)
        {
            AddMasterToSelectedGrid();
        }

        private void dgrdStockReport_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnPDF.Enabled = false;
                picBox.Visible = true;
                DataTable dt = CreateDatatable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("Alteration Report");
                    Reporting.AlterationReport objAlterationReport = new global::SSS.Reporting.AlterationReport();
                    objAlterationReport.SetDataSource(dt);

                    string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }
                    if (rdoPending.Checked)
                    {
                        strPath += "\\Pending-Alteration.pdf";
                    }
                    else if (rdoReady.Checked)
                    {
                        strPath += "\\Ready-Alteration.pdf";
                    }
                    else if (rdoReAlter.Checked)
                    {
                        strPath += "\\Realter-Alteration.pdf";
                    }
                    else if (rdoDelivered.Checked)
                    {
                        strPath += "\\Delivered-Alteration.pdf";
                    }
                    else if (rdoExchange.Checked)
                    {
                        strPath += "\\Exchange-Alteration.pdf";
                    }
                    else
                    {
                        strPath += "\\Alteration.pdf";
                    }

                    if (File.Exists(strPath))
                    {
                        File.Delete(strPath);
                    }
                    objAlterationReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);

                    MessageBox.Show("Thank you ! PDF created successfully ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    objAlterationReport.Close();
                    objAlterationReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            btnPDF.Enabled = true;
            picBox.Visible = false;
        }

        //private void SetRealterGrid()
        //{
        //    try
        //    {
        //        ReAlterSlip objSlip = new ReAlterSlip();
        //        int index = 0;
        //        foreach (DataGridViewRow row in dgrdStockReport.Rows)
        //        {
        //            if (Convert.ToBoolean(row.Cells[0].Value))
        //            {
        //                objSlip.dgrdStockReport.Rows.Add(1);
        //                for (int i = 0; i < 13; i++)
        //                {
        //                    objSlip.dgrdStockReport.Rows[index].Cells[i].Value = row.Cells[i].Value;
        //                }
        //                objSlip.dgrdStockReport.Rows[index].Cells["altID"].Value = row.Cells["altID"].Value;
        //                index++;
        //            }
        //        }
        //        if (objSlip.dgrdStockReport.Rows.Count > 0)
        //        {
        //            objSlip.ShowDialog();
        //        }
        //    }
        //    catch 
        //    {               
        //    }
        //}

        private void btnRealter_Click(object sender, EventArgs e)
        {
            //SetRealterGrid();
        }

        private void btnSendReadySMS_Click(object sender, EventArgs e)
        {
            try
            {
                //object objSMS = DataBaseAccess.ExecuteMyScalar("Select ReadySMS from CompanyDetails ");
                txtSelectedMobileNo.Text = GetSelectedMobileNo();
                txtSMS.Text = "";//Convert.ToString(objSMS);
                txtSMS.Text += GetSelectedItem();
                SMSCount();
                panelSMS.Visible = true;
                txtSMS.Focus();
            }
            catch
            {
            }
        }

        #region Send SMS

        private string GetSelectedMobileNo()
        {
            string strAllNo = "";
            int count = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdStockReport.Rows)
                {
                    bool chkStatus = Convert.ToBoolean(row.Cells[0].Value);
                    if (chkStatus)
                    {
                        string strMobileNo = Convert.ToString(row.Cells["mobileNoI"].Value), strAltNo = Convert.ToString(row.Cells["altNo"].Value);
                        if (strMobileNo != "")
                        {
                            if (strAllNo == "")
                            {
                                strAllNo = "" + strMobileNo;
                                count++;
                            }
                            else
                            {
                                if (!strAllNo.Contains("," + strMobileNo) && (strAllNo != strMobileNo))
                                {
                                    strAllNo += "," + strMobileNo;
                                    count++;
                                }
                            }

                            //if (strAlterationNo == "")
                            //{
                            //    strAlterationNo = strAltNo;
                            //}
                            //else
                            //{
                            //    if (!strAlterationNo.Contains("," + strAltNo) && (strAlterationNo!=strAltNo))
                            //    {
                            //        strAlterationNo += "," + strAltNo;
                            //    }
                            //}
                        }
                    }
                }
            }
            catch
            {
            }

            lblCount.Text = "Mobile No. Count : " + count.ToString("0");
            return strAllNo;
        }

        private string GetSelectedItem()
        {
            string strAlterationNo = "", strItem = "", strAltNo = "";
            int count = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdStockReport.Rows)
                {
                    bool chkStatus = Convert.ToBoolean(row.Cells[0].Value);
                    if (chkStatus)
                    {
                        strAltNo = Convert.ToString(row.Cells["altNo"].Value); strItem = Convert.ToString(row.Cells["ItemName"].Value);
                        if (strItem != "")
                        {
                            if (strAlterationNo == "")
                            {
                                strAlterationNo = "AltNo.: " + strAltNo + " and Item Name: " + strItem;
                                count++;
                            }
                            else
                            {
                                if (!strAlterationNo.Contains("AltNo.," + strAltNo + "and Item Name " + strItem) && (strAlterationNo != strAltNo))
                                {
                                    strAlterationNo += "," + strAltNo + "and Item Name " + strItem;
                                    count++;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return strAlterationNo;
        }

        private void txtSMS_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void btnSMS_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtSMS.Text != "" && txtSelectedMobileNo.Text.Length > 9)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Send SMS ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strResult = sendMessage.SendSingleSMS(txtSMS.Text, txtSelectedMobileNo.Text);
                        if (strResult != "")
                        {
                            MessageBox.Show("Message Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            txtSMS.Clear();
                            panelSMS.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Please Try Again  ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please fill the message box and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSMS.Visible = false;
            txtSMS.Clear();
        }

        #endregion     

        private void txtSMS_TextChanged(object sender, EventArgs e)
        {
            SMSCount();
        }

        private void SMSCount()
        {
            try
            {
                lblCharCount.Text = "Char Count : " + txtSMS.Text.Length.ToString();
                if (txtSMS.Text.Length % 160 != 0)
                {
                    lblSMSCount.Text = "SMS Count : " + ((txtSMS.Text.Length / 160) + 1).ToString();
                }
                else
                {
                    lblSMSCount.Text = "SMS Count : " + (txtSMS.Text.Length / 160).ToString();
                }
            }
            catch
            {
                lblSMSCount.Text = "1";
            }
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            //object objSMS = DataBaseAccess.ExecuteMyScalar("Select OtherSMS from CompanyDetails ");
            txtSelectedMobileNo.Text = GetSelectedMobileNo();
            txtSMS.Text = "";
            txtSMS.Text += GetSelectedItem();
            SMSCount();
            panelSMS.Visible = true;
            txtSMS.Focus();
        }

        private void SMS()
        {
            txtSelectedMobileNo.Text = GetSelectedMobileNo();
            txtSMS.Text = "";
            panelSMS.Visible = true;
            txtSMS.Focus();
        }

        private void dgrdStockReport_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 17 && e.RowIndex >= 0)
                {
                    if (Convert.ToBoolean(dgrdStockReport.CurrentRow.Cells[0].Value))
                    {

                        //string strDate = Convert.ToString(dgrdStockReport.CurrentCell.EditedFormattedValue);
                        ////if (strDate != "")
                        ////{
                        ////    strDate = strDate.Replace("/", "");
                        ////    if (strDate.Length == 8)
                        ////    {

                       

                        //try
                        //{
                        //    if (!maskedTxtBox.Text.Contains("/"))
                        //    {
                        //        e.Cancel = true;
                        //    }
                        //    else
                        //    {
                        //        if (e.RowIndex != dgrdStockReport.Rows.Count - 1)
                        //        {
                        //            dgrdStockReport.EndEdit();
                        //        }
                        //    }
                        //    dgrdStockReport.CurrentCell.Value = txtDate.Text;
                        //}
                        //catch
                        //{
                        //}
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    e.Cancel = true;
                        //}
                        //}
                    }
                }
            }
            catch
            {
            }
        }

        private void chkHeader_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgrdStockReport.Rows)
            {
                row.Cells[0].Value = chkHeader.Checked;
            }
        }

        private void txtDStartDate_Leave(object sender, EventArgs e)
        {
            DbAcess.GetDateInExactFormat(sender, false, false, true);
        }

        private void rdoPending_CheckedChanged(object sender, EventArgs e)
        {
            dgrdStockReport.Rows.Clear();
            lblQty.Text = "0";
        }

        private void lnkColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pnlColor.Visible)
                pnlColor.Visible = false;
            else
                pnlColor.Visible = true;

        }

        private void btnDatewise_Click(object sender, EventArgs e)
        {
            try
            {
                btnDatewise.Enabled = false;
                picBox.Visible = true;

                DataTable dt = CreateDatewiseDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("Alteration Report Datewise");
                    Reporting.DatewiseAlerationReport objAlterationReport = new global::SSS.Reporting.DatewiseAlerationReport();
                    objAlterationReport.SetDataSource(dt);
                    objReport.myPreview.ReportSource = objAlterationReport;
                    objReport.Show();

                    objAlterationReport.Close();
                    objAlterationReport.Dispose();
                }
                else
                    MessageBox.Show("Sorry ! No record found ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            btnDatewise.Enabled = true;
            picBox.Visible = false;
        }

        private DataTable CreateDatewiseDataTable()
        {
            string strQuery = "";
            DataTable myTable = new DataTable();
            try
            {
                strQuery = "Select *,ass.ID as SID,ass.Remark as OldRemark from AlterationSlip aSlip inner join AlterationSlipSecondary ass on aSlip.SerialNo=ass.SerialNo and aSlip.SerialCode=ass.SerialCode  Where aSlip.SerialNo!=0  "; //and aSlip.SerialCode='" + strAltSerialCode + "'

                string strSubQuery = CreateQuery();
                if (strQuery != "")
                {
                    strQuery += strSubQuery;
                }
                strQuery += "  Order by aSlip.Date, aSlip.SerialNo";
                DataTable dt = DbAcess.GetDataTable(strQuery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    myTable.Columns.Add("CompanyHeader", typeof(String));
                    myTable.Columns.Add("HeaderName", typeof(String));
                    myTable.Columns.Add("SerialNo", typeof(String));
                    myTable.Columns.Add("DatePeriod", typeof(String));
                    myTable.Columns.Add("Date", typeof(String));
                    myTable.Columns.Add("AltNo", typeof(String));
                    myTable.Columns.Add("BillNo", typeof(String));
                    myTable.Columns.Add("OrderNo", typeof(String));
                    myTable.Columns.Add("DDate", typeof(String));
                    myTable.Columns.Add("ItemName", typeof(String));
                    myTable.Columns.Add("Qty", typeof(String));
                    myTable.Columns.Add("Status", typeof(String));
                    myTable.Columns.Add("MobileNo", typeof(String));
                    myTable.Columns.Add("TotalQty", typeof(String));
                    myTable.Columns.Add("TQty", typeof(String));
                    string strDate = "", strNewDate = "";                    

                    foreach (DataGridViewRow row in dgrdStockReport.Rows)
                    {
                        DataRow dRow = myTable.NewRow();



                        dRow["HeaderName"] = "ALTERATION REPORT";
                        dRow["SerialNo"] = row.Cells["SerialNo"].Value;
                        dRow["Date"] = row.Cells["SrDate"].Value;
                        dRow["AltNo"] = row.Cells["altCode"].Value + " " + row.Cells["altNo"].Value;
                        dRow["BillNo"] = row.Cells["billNo"].Value;
                        dRow["OrderNo"] = row.Cells["orderNo"].Value;
                        dRow["DDate"] = row.Cells["dDate"].Value;
                        dRow["ItemName"] = row.Cells["ItemName"].Value;
                        dRow["Qty"] = row.Cells["Quantity"].Value;
                        dRow["Status"] = row.Cells["altStatus"].Value;
                        dRow["MobileNo"] = row.Cells["mobileNoI"].Value;
                        dRow["TotalQty"] = lblQty.Text;
                        dRow["TQty"] = row.Cells["Quantity"].Value;

                        myTable.Rows.Add(dRow);
                    }



                    if (chkDDate.Checked && txtDStartDate.Text != "" && txtDEndDate.Text != "")
                        myTable.Rows[0]["DatePeriod"] = "Date Period : " + txtDStartDate.Text + " To " + txtDEndDate.Text;
                    else
                        myTable.Rows[0]["DatePeriod"] = "Date Period : " + Convert.ToDateTime(dt.Rows[0]["Date"]).ToString("dd/MM/yyyy") + " To " + Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["Date"]).ToString("dd/MM/yyyy");

                    strDate = strNewDate;
                   
                    myTable.Rows[0]["HeaderName"] = "Alteration Slip";

                }
                if (myTable.Rows.Count > 0)
                {
                    myTable.Rows[0]["CompanyHeader"] = MainPage.strCompanyName;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myTable;
        }

        private void btnExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnExcelFile.Enabled = false;
                picBox.Visible = true;
                if (dgrdStockReport.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to create excel ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //CreateNormalExcel(dt);

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
                        for (int j = 1; j < dgrdStockReport.Columns.Count + 1; j++)
                        {
                            strHeader = dgrdStockReport.Columns[j - 1].HeaderText;
                            if (strHeader == "" || !dgrdStockReport.Columns[j - 1].Visible)
                            {
                                _skipColumn++;
                                j++;
                            }

                            ExcelApp.Cells[1, j - _skipColumn] = dgrdStockReport.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                        }
                        _skipColumn = 0;
                        // Storing Each row and column value to excel sheet
                        for (int k = 0; k < dgrdStockReport.Rows.Count; k++)
                        {
                            for (int l = 0; l < dgrdStockReport.Columns.Count; l++)
                            {
                                if (dgrdStockReport.Columns[l].HeaderText == "" || !dgrdStockReport.Columns[l].Visible)
                                {
                                    _skipColumn++;
                                    l++;
                                }
                                if (l < dgrdStockReport.Columns.Count)
                                    ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdStockReport.Rows[k].Cells[l].Value.ToString();
                            }
                            _skipColumn = 0;
                        }
                        ExcelApp.Columns.AutoFit();


                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Alteration_Slip_Register";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        }
                        xlWorkbook.Close(true, misValue, misValue);
                        ExcelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                    MessageBox.Show("Sorry ! No record found. Please select atleast 1 record... ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            picBox.Visible = false;
            btnExcelFile.Enabled = true;
        }

        private void panCommand_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtStartDate.ReadOnly = txtEndDate.ReadOnly = !chkDate.Checked;
            txtStartDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtEndDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtAltCode_TextChanged(object sender, EventArgs e)
        {
            SearchQueryData();
        }

        private void chkDDate_CheckedChanged(object sender, EventArgs e)
        {
            txtDStartDate.ReadOnly = txtDEndDate.ReadOnly = !chkDDate.Checked;
            txtDStartDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtDEndDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSNo.ReadOnly = txtToSNo.ReadOnly = !chkSNo.Checked;
            txtFromSNo.Text = txtToSNo.Text = "";
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALTERATIONCODE", "SEARCH ALT. SERIAL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdStockReport_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 22)
            {
                double dQty, dPCS;
                dQty = ConvertObjectToDouble(dgrdStockReport.CurrentRow.Cells["Quantity"].Value);
                dPCS = ConvertObjectToDouble(dgrdStockReport.CurrentRow.Cells["PCS"].Value);
                if (dPCS > dQty)
                {
                    MessageBox.Show("Sorry ! PCSs can't be greater than Quantity ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdStockReport.CurrentCell = dgrdStockReport.CurrentRow.Cells["pcs"];
                    dgrdStockReport.Focus();
                }
            }
            if (maskedTxtBox.Visible && e.ColumnIndex == 17)
            {
                var ValidDate = DbAcess.GetDateInExactFormat(maskedTxtBox, true, false, false);
                if (ValidDate)
                {
                    dgrdStockReport.CurrentCell.Value = maskedTxtBox.Text;
                    maskedTxtBox.Visible = false;
                }
                else
                {
                    dgrdStockReport.CurrentCell.Value = maskedTxtBox.Text;
                    maskedTxtBox.Focus();
                    maskedTxtBox.Select();
                }
            }
        }

        private void AlterationSlipRegister_Load(object sender, EventArgs e)
        {
            btnExcelFile.Enabled = MainPage.mymainObject.bExport;

            maskedTxtBox = new MaskedTextBox();
            maskedTxtBox.Visible = false;
            dgrdStockReport.Controls.Add(maskedTxtBox);

            //dgrdStockReport.CellBeginEdit += new DataGridViewCellCancelEventHandler(dgrdStockReport_CellBeginEdit);
            //dgrdStockReport.CellEndEdit += new DataGridViewCellEventHandler(dgrdStockReport_CellEndEdit);
            //dgrdStockReport.Scroll += new ScrollEventHandler(dgrdStockReport_Scroll);

            try
            {
                if (MainPage.mymainObject.bSaleReport)
                    DbAcess.EnableCopyOnClipBoard(dgrdStockReport);
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }

        }

        private void dgrdStockReport_Scroll(object sender, ScrollEventArgs e)
        {
            if (maskedTxtBox.Visible)
            {
                Rectangle rect = dgrdStockReport.GetCellDisplayRectangle(dgrdStockReport.CurrentCell.ColumnIndex, dgrdStockReport.CurrentCell.RowIndex, true);
                maskedTxtBox.Location = rect.Location;
            }
        }

        private void dgrdStockReport_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgrdStockReport.Columns[e.ColumnIndex].Name == "billNo")
                dgrdStockReport.Cursor = Cursors.Hand;
            else
                dgrdStockReport.Cursor = Cursors.Arrow;
        }

        private void CreateNormalExcel(DataTable table)
        {
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            try
            {
                object misValue = System.Reflection.Missing.Value;
                ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
                ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
                ExcelWorkSheet.Name = "ALTERATION DETAILS";

                ExcelWorkSheet.Cells[1, 1] = "SERIAL NO";
                ExcelWorkSheet.Cells[1, 2] = "DATE";
                ExcelWorkSheet.Cells[1, 3] = "ALT. No.";
                ExcelWorkSheet.Cells[1, 4] = "BILL No.";
                ExcelWorkSheet.Cells[1, 5] = "CUSTOMER NAME";
                ExcelWorkSheet.Cells[1, 6] = "OrderNO";
                ExcelWorkSheet.Cells[1, 7] = "DDate";
                ExcelWorkSheet.Cells[1, 8] = "Item Name";
                ExcelWorkSheet.Cells[1, 9] = "Qty.";
                ExcelWorkSheet.Cells[1, 10] = "Status";
                ExcelWorkSheet.Cells[1, 11] = "Mobile No.";
                ExcelWorkSheet.Cells[1, 12] = "Total Qty.";
                ExcelWorkSheet.Cells[1, 13] = "Old Remarks.";

                int columnIndex = 1;
                foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
                {
                    if (columnIndex == 1)
                        column.ColumnWidth = (double)column.ColumnWidth + 7;
                    else if (columnIndex == 2)
                        column.ColumnWidth = (double)column.ColumnWidth + 4;
                    else if (columnIndex == 4)
                        column.ColumnWidth = (double)column.ColumnWidth + 9;
                    else if (columnIndex == 5)
                        column.ColumnWidth = (double)column.ColumnWidth + 8;
                    else if (columnIndex == 11)
                        column.ColumnWidth = (double)column.ColumnWidth + 3;
                    else if (columnIndex == 6)
                        column.ColumnWidth = (double)column.ColumnWidth + 9;
                    else if (columnIndex == 7)
                        column.ColumnWidth = (double)column.ColumnWidth + 9;
                    else if (columnIndex == 8)
                        column.ColumnWidth = (double)column.ColumnWidth + 8;
                    else if (columnIndex == 10)
                        column.ColumnWidth = (double)column.ColumnWidth + 8;
                    else if (columnIndex == 13)
                        column.ColumnWidth = (double)column.ColumnWidth + 10;
                    else if (columnIndex > 14)
                        break;
                    columnIndex++;
                }

                int rowIndex = 2;
                foreach (DataRow row in table.Rows)
                {
                    for (int col = 1; col < 14; col++)
                    {
                        ExcelWorkSheet.Cells[rowIndex, col] = row[col - 1];
                    }
                    rowIndex++;
                }

                NewExcel.Range objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 1];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 2];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 3];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 4];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 5];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 6];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 7];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 8];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 9];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 10];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 11];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 12];
                objQRange.Font.Bold = true;
                objQRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, 13];
                objQRange.Font.Bold = true;

                ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                ExcelWorkBook.Close(true, misValue, misValue);
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);

                MessageBox.Show("Thank you ! Excel created successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch
            {
            }
            finally
            {
                //foreach (Process process in Process.GetProcessesByName("Excel"))
                //    process.Kill();
            }
        }

        private string GetFileName()
        {
            string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Excel File";
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            strPath += "\\CUSTOMERDETAILS.xls";

            try
            {
                FileInfo file = new FileInfo(strPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch
            {
            }
            return strPath;
        }

    }
}
