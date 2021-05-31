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
    public partial class MonthlyStockRegister : Form
    {
        DataBaseAccess dba;
        string strSupplierName="",strBarCode="",strBrandName="",strGroupName="",strCategoryName="", strItemName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "",strDecimal="0",strUnitName="",strGodownName="";
        string[] strMonth = { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        double dInQty = 0, dOutQty = 0, dInAmount = 0, dOutAmount = 0, dNetRate = 0, dNetQty = 0, dNetAmt = 0;
        DateTime _fromInDate = MainPage.startFinDate, _fromOutDate = MainPage.startFinDate,_toInDate=  MainPage.endFinDate,_toOutDate=  MainPage.endFinDate;              
        int month;

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkOutDate.Checked, false, true);
        }

        bool _detailStatus = false;

        public MonthlyStockRegister(DataGridViewRow row, bool _status)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _detailStatus = _status;
            BindInitialData(row);           
            GetAllDataWithDetails();
        }

        public MonthlyStockRegister(DataGridViewRow row, bool _status,DateTime _fromIDate,DateTime _toIDate,DateTime _fromODate,DateTime _toODate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();            
            _detailStatus = _status;
            _fromInDate = _fromIDate;
            _fromOutDate = _fromODate;
            _toInDate = _toIDate;
            _toOutDate = _toODate;

            BindInitialData(row);
            GetAllDataWithDetails();
        }

        private void BindInitialData(DataGridViewRow row)
        {
            try
            {
                if (row.DataGridView.Columns.Contains("PParty"))
                    strSupplierName = Convert.ToString(row.Cells["PParty"].Value);
                if (row.DataGridView.Columns.Contains("Category"))
                    strCategoryName = Convert.ToString(row.Cells["Category"].Value);
                if (row.DataGridView.Columns.Contains("BrandName"))
                    strBrandName = Convert.ToString(row.Cells["BrandName"].Value);
                if (row.DataGridView.Columns.Contains("GroupName"))
                    strGroupName = Convert.ToString(row.Cells["GroupName"].Value);

                if (row.DataGridView.Columns.Contains("itemname"))
                    strItemName = Convert.ToString(row.Cells["itemName"].Value);
                if (row.DataGridView.Columns.Contains("Variant1"))
                    strCategory1 = Convert.ToString(row.Cells["Variant1"].Value);
                if (row.DataGridView.Columns.Contains("Variant2"))
                    strCategory2 = Convert.ToString(row.Cells["Variant2"].Value);
                if (row.DataGridView.Columns.Contains("Variant3"))
                    strCategory3 = Convert.ToString(row.Cells["Variant3"].Value);
                if (row.DataGridView.Columns.Contains("Variant4"))
                    strCategory4 = Convert.ToString(row.Cells["Variant4"].Value);
                if (row.DataGridView.Columns.Contains("Variant5"))
                    strCategory5 = Convert.ToString(row.Cells["Variant5"].Value);
                if (row.DataGridView.Columns.Contains("barCode"))
                    strBarCode = Convert.ToString(row.Cells["barCode"].Value);
                

                string strFullItemName = strItemName;
                if (strCategoryName != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                   strFullItemName +=  strCategoryName;
                }
                if (strBrandName != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strBrandName;
                }
                if (strBarCode != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strBarCode;
                }
                if (strCategory1 != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strCategory1;
                }
                if (strCategory2 != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strCategory2;
                }
                if (strCategory3 != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strCategory3;
                }
                if (strCategory4 != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strCategory4;
                }
                if (strCategory5 != "")
                {
                    if (strFullItemName != "")
                        strFullItemName += " / ";
                    strFullItemName += strCategory5;
                }

                lblName.Text = strFullItemName;
            }
            catch
            {
            }
        }
        
        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }


        private void StockRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void GetAllDataWithDetails()
        {
            try
            {
                dgrdDetails.Rows.Clear();
                dgrdDetails.Rows.Add(13);

                DataRow row = GetOpeningRecord();
                BindDataWithGrid(row, 0);

                month = MainPage.startFinDate.Month;
                for (int i = 1; i < 13; i++)
                {
                    row = GetDataFromDataBase(month);
                    BindDataWithGrid(row, i);
                    if (month == 12)
                        month = 0;
                    month++;
                }

                lblInQty.Text = dInQty.ToString("N" + strDecimal, MainPage.indianCurancy);
               // lblIAmount.Text = dInAmount.ToString("N2", MainPage.indianCurancy);
                lblOutQty.Text = dOutQty.ToString("N" + strDecimal, MainPage.indianCurancy);
               // lblOutAmt.Text = dOutAmount.ToString("N2", MainPage.indianCurancy);
                lblNetQty.Text = dNetQty.ToString("N" + strDecimal, MainPage.indianCurancy);
              //  lblNetAmt.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);
                if (dNetQty < 0)
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkRed;
                else
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkGreen;
            }
            catch
            {
            }
        }

        private void BindDataWithGrid(DataRow row, int rowIndex)
        {
            if (row != null)
            {
                double dIQty = 0, dOQty = 0, dIAmt = 0, dOAmt = 0;
                dInQty += dIQty = dba.ConvertObjectToDouble(row["InQty"]);
                dOutQty += dOQty = dba.ConvertObjectToDouble(row["OutQty"]);
                dInAmount += dIAmt = dba.ConvertObjectToDouble(row["IAmount"]);
                dOutAmount += dOAmt = dba.ConvertObjectToDouble(row["OAmount"]);
                dNetQty += dIQty - dOQty;
                if (dInQty != 0)
                    dNetRate = dInAmount / dInQty;
                else
                    dNetRate = 0;
                dNetAmt = dNetQty * dNetRate;
                dgrdDetails.Rows[rowIndex].Cells["monthName"].Value = row["MonthName"];
                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = strUnitName;
                dgrdDetails.Rows[rowIndex].Cells["inQty"].Value = dIQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["outQty"].Value = dOQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["iAmount"].Value = dIAmt.ToString("N2", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["oAmount"].Value = dOAmt.ToString("N2", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["netQty"].Value = dNetQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["netRate"].Value = dNetRate.ToString("N0", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["netAmount"].Value = dNetAmt.ToString("N0", MainPage.indianCurancy);
                if(dNetQty<0)
                    dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
            }
            else
            {
                dgrdDetails.Rows[rowIndex].Cells["monthName"].Value = strMonth[month-1];
                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = strUnitName;
                dgrdDetails.Rows[rowIndex].Cells["inQty"].Value = "--";
                dgrdDetails.Rows[rowIndex].Cells["outQty"].Value = "--";
                dgrdDetails.Rows[rowIndex].Cells["iAmount"].Value = "--";
                dgrdDetails.Rows[rowIndex].Cells["oAmount"].Value = "--";
                dgrdDetails.Rows[rowIndex].Cells["netQty"].Value = dNetQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["netRate"].Value = dNetRate.ToString("N0", MainPage.indianCurancy);
                dgrdDetails.Rows[rowIndex].Cells["netAmount"].Value = dNetAmt.ToString("N0", MainPage.indianCurancy);
            }

        }

        private DataRow GetDataFromDataBase(int month)
        {
            DataRow row = null;
            try
            {
                string strSubQuery = "", strQuery = "", strInDateQuery = "", strOutDateQuery = "";

                strInDateQuery = " and SM.Date>='" + _fromInDate.ToString("MM/dd/yyyy") + "' and SM.Date<'" + _toInDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                strOutDateQuery = " and SM.Date>='" + _fromOutDate.ToString("MM/dd/yyyy") + "' and SM.Date<'" + _toOutDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";


                strSubQuery = " and DATEPART(MM,SM.Date)=" + month + " ";

                if (strGroupName != "")
                    strSubQuery += " and GroupName='" + strGroupName + "' ";
                if (strCategoryName != "")
                    strSubQuery += " and Category='" + strCategoryName + "' ";
                if (strBrandName != "")
                    strSubQuery += " and BrandName='" + strBrandName + "' ";
                if (strBarCode != "")
                    strSubQuery += " and BarCode='" + strBarCode + "' ";
                if (strItemName != "")
                    strSubQuery += " and ItemName='" + strItemName + "' ";
                if (strCategory1 != "")
                    strSubQuery += " and Variant1='" + strCategory1 + "' ";
                if (strCategory2 != "")
                    strSubQuery += " and Variant2='" + strCategory2 + "' ";

                strQuery += " Select MonthName,SUM(IQty) InQty,SUM(IAmt)IAmount,SUM(OQty) OutQty,SUM(OAmt)OAmount from ( "
                              + " Select UPPER(MName)MonthName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(IQty) IQty,SUM(OQty) OQty,Rate,(SUM(IQty)*Rate) IAmt,(SUM(OQty)*Rate) OAmt from ( "
                              + " Select DATENAME(MONTH, SM.Date) MName,GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) IQty,0 as OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM Where BillType in ('PURCHASE') " + strSubQuery + strInDateQuery+ " Group By DATENAME(MONTH, SM.Date),GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate  Union All "
                              + " Select DATENAME(MONTH, SM.Date) MName,GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) IQty,0 as OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM  Where BillType in ('SALERETURN') " + strSubQuery + strInDateQuery+ " Group By DATENAME(MONTH, SM.Date),GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate  Union All "
                              + " Select DATENAME(MONTH, SM.Date) MName,GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,0 as IQty,SUM(Qty) OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM Where  BillType in ('SALES') " + strSubQuery +strOutDateQuery+ " Group By DATENAME(MONTH, SM.Date),GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate  Union All "
                              + " Select DATENAME(MONTH, SM.Date) MName,GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,(CASE WHEN SM.BillType='STOCKIN' THEN SUM(Qty) else 0 end) as IQty,(CASE WHEN SM.BillType='STOCKOUT' THEN SUM(Qty) else 0 end) OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM  Where  BillType in ('STOCKOUT','STOCKIN') " + strSubQuery + strInDateQuery + strOutDateQuery + " Group By DATENAME(MONTH, SM.Date),GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate,BillType UNION ALL "
                              + " Select DATENAME(MONTH, SM.Date) MName,GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,0 as IQty,SUM(Qty) OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM Where  BillType in ('PURCHASERETURN') " + strSubQuery +strOutDateQuery+ " Group By DATENAME(MONTH, SM.Date),GroupName,Category,BrandName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate "
                              + " ) Stock Where (IQty!=0 OR OQty!=0)  Group By MName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate "
                              + " ) Stock Group By MonthName ";

                DataTable table = dba.GetDataTable(strQuery);
                if (table.Rows.Count > 0)
                    row = table.Rows[0];
            }
            catch
            {
            }
            return row;
        }

        private DataRow GetOpeningRecord()
        {
            DataRow row = null;
            try
            {
                string strSubQuery = "", strQuery = "", strInDateQuery="";
                //// strSubQuery = " and DesignNo='" + strItemName + "'  ";
                //if (_detailStatus)
                //    strSubQuery += " and ItemName in ('" + strItemName + "')  and Variant1 in ('" + strCategory1 + "') and Variant2 in ('" + strCategory2 + "') ";
                //else
                //    strSubQuery += " and ItemName in (Select ItemName from Items Where GroupName in ('" + strItemName + "')) ";
                strInDateQuery = " and Date>='" + _fromInDate.ToString("MM/dd/yyyy") + "' and Date<'" + _toInDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";

                if (strGroupName != "")
                    strSubQuery += " and GroupName='" + strGroupName + "' ";
                if (strCategoryName != "")
                    strSubQuery += " and Category='" + strCategoryName + "' ";
                if (strBrandName != "")
                    strSubQuery += " and BrandName='" + strBrandName + "' ";
                if (strBarCode != "")
                    strSubQuery += " and BarCode='" + strBarCode + "' ";
                if (strItemName != "")
                    strSubQuery += " and ItemName='" + strItemName + "' ";
                if (strCategory1 != "")
                    strSubQuery += " and Variant1='" + strCategory1 + "' ";
                if (strCategory2 != "")
                    strSubQuery += " and Variant2='" + strCategory2 + "' ";


                strQuery = "Select 'OPENING' as MonthName, SUM(IQty) InQty,SUM(IAmt)IAmount,SUM(OQty) OutQty,SUM(OAmt)OAmount from ( "
                              + " Select GroupName,Category,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(IQty) IQty,SUM(OQty) OQty,Rate,(SUM(IQty)*Rate) IAmt,(SUM(OQty)*Rate) OAmt from (  "
                              + " Select GroupName,Category,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,SUM(Qty) IQty,0 as OQty,Rate from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM Where BillType='OPENING' " + strSubQuery+ strInDateQuery+ " Group By GroupName,Category,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate "
                              + " ) Stock Where (IQty!=0 OR OQty!=0)  Group By GroupName,Category,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5,Rate) Stock  ";

                DataTable table = dba.GetDataTable(strQuery);
                if (table.Rows.Count > 0)
                    row = table.Rows[0];
            }
            catch
            {
            }
            return row;
        }             
     
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.StockRegisterReport objReport = new Reporting.StockRegisterReport();
                        objReport.SetDataSource(dt);
                        string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strPath += "\\Stock_Details.pdf";
                        System.IO.FileInfo objFile = new System.IO.FileInfo(strPath);
                        if (objFile.Exists)
                            objFile.Delete();
                        strPath = strPath.Replace('/', '_');
                        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);                      

                        MessageBox.Show("Thank you ! File has been saved on Desktop with the name of Stock_Details", "Record Exported", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        objReport.Close();
                        objReport.Dispose();
                    }
                }
            }
            catch
            {
            }
            btnExport.Enabled = true ;
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CompanyName", typeof(String));
            table.Columns.Add("HeaderName", typeof(String));
            table.Columns.Add("SNo", typeof(String));
            table.Columns.Add("ItemName", typeof(String));
            table.Columns.Add("InQty", typeof(String));
            table.Columns.Add("InAmt", typeof(String));
            table.Columns.Add("OutQty", typeof(String));
            table.Columns.Add("OutAmt", typeof(String));
            table.Columns.Add("NetQty", typeof(Double));
            table.Columns.Add("NetRate", typeof(String));
            table.Columns.Add("NetAmt", typeof(String));
            table.Columns.Add("Unit", typeof(String));
            table.Columns.Add("TotalInQty", typeof(String));
            table.Columns.Add("TotalInAmt", typeof(String));
            table.Columns.Add("TotalOutQty", typeof(String));
            table.Columns.Add("TotalOutAmt", typeof(String));
            table.Columns.Add("TotalNetQty", typeof(String));
            table.Columns.Add("TotalNetAmt", typeof(String));

            int rowIndex=1;
            string strItem="", strCategory1 = "", strCategory2 = "";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strItem = Convert.ToString(row.Cells["itemName"].Value);
                strCategory1 = Convert.ToString(row.Cells["category1"].Value);
                strCategory2 = Convert.ToString(row.Cells["category2"].Value);
                if (strCategory2 != "")
                    strItem += " / " + strCategory2;
                if (strCategory1 != "")
                    strItem += " / " + strCategory1;
               
                DataRow dRow = table.NewRow();
                dRow["CompanyName"] = MainPage.strPrintComapanyName;
                dRow["HeaderName"] = "Sock Register";
                dRow["SNo"] = rowIndex + ".";
                dRow["ItemName"] = strItem + " " + row.Cells["category3"].Value;
                dRow["InQty"] = row.Cells["inQty"].Value;
                dRow["OutQty"] = row.Cells["outQty"].Value;
                dRow["InAmt"] = row.Cells["iAmount"].Value;
                dRow["OutAmt"] = row.Cells["oAmount"].Value;
                dRow["NetQty"] = row.Cells["netQty"].Value;
                dRow["NetRate"] = row.Cells["netRate"].Value;
                dRow["NetAmt"] = row.Cells["netAmount"].Value;
                dRow["Unit"] = row.Cells["unitName"].Value;
                table.Rows.Add(dRow);
                rowIndex++;
            }

            if (table.Rows.Count > 0)
            {
                rowIndex = table.Rows.Count - 1;
                table.Rows[rowIndex]["TotalInQty"] = lblInQty.Text;
                table.Rows[rowIndex]["TotalInAmt"] = lblIAmount.Text;
                table.Rows[rowIndex]["TotalOutQty"] = lblOutQty.Text;
                table.Rows[rowIndex]["TotalOutAmt"] = lblOutAmt.Text;
                table.Rows[rowIndex]["TotalNetQty"] = lblNetQty.Text;
                table.Rows[rowIndex]["TotalNetQty"] = lblNetAmt.Text;
            }

            return table;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.RowIndex > 0)
                {
                    ShowSummaryInvoiceWise();
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetails.CurrentRow.Index > 0 && dgrdDetails.CurrentCell.ColumnIndex >= 0)
                    {
                        ShowSummaryInvoiceWise();
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowSummaryInvoiceWise()
        {
            System.Collections.Hashtable objHash = new System.Collections.Hashtable();
            objHash["GroupName"] = strGroupName;
            objHash["CategoryName"] = strCategoryName;
            objHash["BrandName"] = strBrandName;
            objHash["BarCode"] = strBarCode;
            objHash["ItemName"] = strItemName;
            objHash["Category1"] = strCategory1;
            objHash["Category2"] = strCategory2;
            objHash["Category3"] = strCategory3;
            objHash["Category4"] = strCategory4;
            objHash["Category5"] = strCategory5;
            objHash["Decimal"] = strDecimal;
            objHash["UnitName"] = strUnitName;
            objHash["MonthName"] = dgrdDetails.CurrentRow.Cells["monthName"].Value;
            objHash["GodownName"] = strGodownName;

            StockSummaryVoucherWise objSummary = new StockSummaryVoucherWise(objHash, _detailStatus, _fromInDate, _toInDate, _fromOutDate, _toOutDate);
            objSummary.MdiParent = MainPage.mymainObject;        
            objSummary.txtFromDate.Text = txtFromDate.Text;
            objSummary.txtToDate.Text = txtToDate.Text;
            objSummary.txtOFromDate.Text = txtOFromDate.Text;
            objSummary.txtOToDate.Text = txtOToDate.Text;
            objSummary.Show();
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkOutDate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.ReadOnly = txtOToDate.ReadOnly = !chkOutDate.Checked;
            txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

     
        private void btnGO_Click(object sender, EventArgs e)
        {
            btnGO.Enabled = false;
            try
            {
                if ((chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10)))
                    MessageBox.Show("Sorry ! Please enter In date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if ((chkOutDate.Checked && (txtOFromDate.Text.Length != 10 || txtOToDate.Text.Length != 10)))
                    MessageBox.Show("Sorry ! Please enter Out date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                    {
                        _fromInDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                        _toInDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    }

                    if (chkOutDate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
                    {
                        _fromOutDate = dba.ConvertDateInExactFormat(txtOFromDate.Text);
                        _toOutDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
                    }
                    GetAllDataWithDetails();
                }
            }
            catch { }
            btnGO.Enabled = true;
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void MonthlyStockRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

    }
}
