using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class StockSummaryVoucherWise : Form
    {
        DataBaseAccess dba;
        string strMonthName="",strGroupName="",strCategoryName="",strBrandName="",strBarCode="", strItemName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "",strDecimal="0",strUnitName="",strGodownName="";
        double dInQty = 0, dOutQty = 0, dInAmount = 0, dOutAmount = 0, dNetRate = 0, dNetQty = 0, dNetAmt = 0;
        int month;
        DateTime _fromInDate = MainPage.startFinDate, _fromOutDate = MainPage.startFinDate, _toInDate = MainPage.endFinDate, _toOutDate = MainPage.endFinDate;

        private void chkOutDate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.ReadOnly = txtOToDate.ReadOnly = !chkOutDate.Checked;
            txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkOutDate.Checked, false, true);
        }

        bool _detailStatus = false;
        public StockSummaryVoucherWise(Hashtable ht, bool _status, DateTime _fromIDate, DateTime _toIDate, DateTime _fromODate, DateTime _toODate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            _detailStatus = _status;
            chkDate.Checked = chkOutDate.Checked = true;
            _fromInDate = _fromIDate;
            _fromOutDate = _fromODate;
            _toInDate = _toIDate;
            _toOutDate = _toODate;

            BindInitialData(ht);
            GetAllDataWithDetails();
        }

        private void StockSummaryVoucherWise_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

        private void BindInitialData(Hashtable hTable)
        {
            string strItem = Convert.ToString(hTable["ItemName"]), strCat1 = Convert.ToString(hTable["Category1"]), strCat2 = Convert.ToString(hTable["Category2"]), strCat3 = Convert.ToString(hTable["Category3"]), strCat4 = Convert.ToString(hTable["Category4"]), strCat5 = Convert.ToString(hTable["Category5"]), strDecPoint = Convert.ToString(hTable["Decimal"]), strUnit = Convert.ToString(hTable["UnitName"]), strGodown = Convert.ToString(hTable["GodownName"]);
            strGroupName= Convert.ToString(hTable["GroupName"]);
            strBarCode = Convert.ToString(hTable["BarCode"]);
            strBrandName = Convert.ToString(hTable["BrandName"]);
            strCategoryName = Convert.ToString(hTable["CategoryName"]);
            strItemName = strItem;
            strMonthName = Convert.ToString(hTable["MonthName"]);
            txtMonth.Text = strMonthName.ToUpper();
            strCategory1 = strCat1;
            strCategory2 = strCat2;
            strCategory3 = strCat3;
            strCategory4 = strCat4;
            strCategory5 = strCat5;
            strDecimal = strDecPoint;
            strUnitName = strUnit;
            strGodownName = strGodown;

            if (strCat1 != "")
                strItem += " / " + strCat1;
            if (strCat2 != "")
                strItem += " / " + strCat2;
            if (strCat3 != "")
                strItem += " / " + strCat3;
            if (strCat4 != "")
                strItem += " / " + strCat4;
            if (strCat5 != "")
                strItem += " / " + strCat5;
            if (strGodown != "")
                strItem += " / " + strGodown;

            lblName.Text = strItem;
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
                //GetOpeningDataFromDataBase();
                GetDataFromDataBase();
               
                lblInQty.Text = dInQty.ToString("N" + strDecimal, MainPage.indianCurancy);
               // lblIAmount.Text = dInAmount.ToString("N2", MainPage.indianCurancy);
                lblOutQty.Text = dOutQty.ToString("N" + strDecimal, MainPage.indianCurancy);
               // lblOutAmt.Text = dOutAmount.ToString("N2", MainPage.indianCurancy);
                lblNetQty.Text = dNetQty.ToString("N" + strDecimal, MainPage.indianCurancy);
               // lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                if (dNetQty < 0)
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkRed;
                else
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkGreen;

            }
            catch
            {
            }
        }

        private void GetDataFromDataBase()
        {
            try
            {
                string strSubQuery = "", strQuery = "",strOpeningQuery="", strInDateQuery="", strOutDateQuery="";
                strInDateQuery = " and Date>='" + _fromInDate.ToString("MM/dd/yyyy") + "' and Date<'" + _toInDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                strOutDateQuery = " and Date>='" + _fromOutDate.ToString("MM/dd/yyyy") + "' and Date<'" + _toOutDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";

                if (txtMonth.Text != "")
                    strSubQuery += " and DATEPART(MM,PB.PDate)=DATEPART(MM,'" + strMonthName + " 01 2019') ";
               // if (chkDate.Checked && txtFromDate.Text.Length==10 && txtToDate.Text.Length==10)
               // {
                 //   DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDate.Text), toDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                 //   strSubQuery += " and PB.Date>='" + fromDate + "' and PB.Date<'" + toDate.AddDays(1) + "' ";
               // }
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

                if (_detailStatus)
                    strOpeningQuery = "Select 'OPENING' Particulars,'' VType,'' as VNo, PB.PDate,Qty as IQty,0 as OQty,Rate,0 ID from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select PB.Date as PDate from Items PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB Where BillType='OPENING' " + strSubQuery ;
                else
                    strOpeningQuery = "Select 'OPENING' Particulars,'' VType,'' as VNo, " + MainPage.startFinDate.ToString("dd/MM/yyyy") + " Date,SUM(Qty) as IQty,0 as OQty,AVG(Rate)Rate,0 ID from StockMaster SM OUTER APPLY (Select Date from Items PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB Where BillType='OPENING' and Rate!=0 " + strSubQuery;

                strQuery += "Select Convert(varchar,PDate,103) BDate,(CASE WHEN Particulars!='OPENING' then ISNULL(dbo.GetFullName(Particulars),Particulars) else Particulars end)Particulars ,VType,VNo,SUM(IQty) InQty,(SUM(IQty)*Rate)InAmt,SUM(OQty) OutQty,(SUM(OQty)*Rate) OutAmt from ( "
                              + strOpeningQuery+"  Union All "
                              + " Select PurchasePartyID as Particulars,'Purchase' as VType,(BillCode+' '+CAST(BillNo as varchar)) as VNo,PDate,Qty IQty,0 as OQty,Rate,1 ID from StockMaster SM  OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select Date as PDate,PurchasePartyID from PurchaseBook PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB Where BillType='PURCHASE' " + strSubQuery + strInDateQuery+ " Union All "
                              + " Select SalePartyID as Particulars,'Sales Return' as VType,(BillCode+' '+CAST(BillNo as varchar)) as VNo,PDate, Qty IQty,0 as OQty,Rate,1 ID from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select Date as PDate,SalePartyID from SaleReturn PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB  Where BillType='SALERETURN' " + strSubQuery + strInDateQuery+ " Union All "
                              + " Select PurchasePartyID as Particulars,'Purchase Return' as VType,(BillCode+' '+CAST(BillNo as varchar)) as VNo,PDate,0 IQty,Qty as OQty,Rate,1 ID from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select Date as PDate,PurchasePartyID from PurchaseReturn PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB  Where BillType='PURCHASERETURN' " + strSubQuery + strOutDateQuery+ " Union All "                              
                              + " Select PurchasePartyID as Particulars,'Stock Transfer' as VType,(BillCode+' '+CAST(BillNo as varchar)) as VNo,PDate,0 IQty,Qty as OQty,Rate,1 ID from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select Date as PDate,(FromMCentre+'-'+ToMCentre) as PurchasePartyID from StockTransfer PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB  Where BillType in ('STOCKOUT','STOCKIN') " + strSubQuery +strInDateQuery+strOutDateQuery+ " Union All "
                              + " Select SalePartyID as Particulars,'Sales' as VType,(BillCode+' '+CAST(BillNo as varchar)) as VNo,PDate, 0 IQty,Qty as OQty,Rate,1 ID from StockMaster SM OUTER APPLY(Select GroupName,_IM.Other as Category from Items _IM Where _im.ItemName=SM.ItemName)_IM OUTER APPLY (Select Date as PDate,SalePartyID from SalesBook PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB Where BillType='SALES' " + strSubQuery +strOutDateQuery
                              + " ) Stock Group by ID,PDate,Particulars,VType,VNo,Rate Order by ID,PDate,VType,VNo ";


                DataTable table = dba.GetDataTable(strQuery);
                BindDataWithGrid(table);

            }
            catch
            {
            }
        }
        
        private void BindDataWithGrid(DataTable dt)
        {
            int rowIndex = dgrdDetails.Rows.Count;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);

                foreach (DataRow row in dt.Rows)
                {
                    double dIQty = 0, dOQty = 0, dIAmt = 0, dOAmt = 0;
                    dInQty += dIQty = dba.ConvertObjectToDouble(row["InQty"]);
                    dOutQty += dOQty = dba.ConvertObjectToDouble(row["OutQty"]);
                    dInAmount += dIAmt = dba.ConvertObjectToDouble(row["InAmt"]);
                    dOutAmount += dOAmt = dba.ConvertObjectToDouble(row["OutAmt"]);
                    dNetQty += dIQty - dOQty;
                    if (dInQty != 0)
                        dNetRate = dInAmount / dInQty;
                    else
                        dNetRate = 0;
                    dNetAmt = dNetQty * dNetRate;

                    dgrdDetails.Rows[rowIndex].Cells["sNo"].Value = (rowIndex+1)+".";
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                    dgrdDetails.Rows[rowIndex].Cells["particulars"].Value = row["Particulars"];
                    dgrdDetails.Rows[rowIndex].Cells["voucherType"].Value = row["VType"];
                    dgrdDetails.Rows[rowIndex].Cells["voucherNo"].Value = row["VNo"];
                    dgrdDetails.Rows[rowIndex].Cells["inQty"].Value = dIQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["outQty"].Value = dOQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["iAmount"].Value = dIAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["oAmount"].Value = dOAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netQty"].Value = dNetQty.ToString("N" + strDecimal, MainPage.indianCurancy);
                    //dgrdDetails.Rows[rowIndex].Cells["netRate"].Value = dNetRate.ToString("N2", DataBaseAccess.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netAmount"].Value = dNetAmt.ToString("N2", MainPage.indianCurancy);
                    rowIndex++;
                }
            }
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
            ShowVoucherDetails();
        }

        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonth.Text = objSearch.strSelectedData;
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

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
            {
                txtFromDate.Enabled = txtToDate.Enabled = true;
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");                
            }
            else
                txtFromDate.Enabled = txtToDate.Enabled = false;
            ClearAllRecord();
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
            ClearAllRecord();
        }

        public void ClearAllRecord()
        {
            lblInQty.Text = lblIAmount.Text = lblOutQty.Text = lblOutAmt.Text = lblNetQty.Text = lblNetAmt.Text = "0.00";
            dgrdDetails.Rows.Clear();
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                if ((chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10)))
                    MessageBox.Show("Sorry ! Please enter In date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if ((chkOutDate.Checked && (txtOFromDate.Text.Length != 10 || txtOToDate.Text.Length != 10)))
                    MessageBox.Show("Sorry ! Please enter Out date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    dInQty = dOutQty = dInAmount = dOutAmount = dNetRate = dNetQty = dNetAmt = 0;
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
            catch
            {
            }
            btnGO.Enabled = true;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowVoucherDetails();
            }
        }

        private void ShowVoucherDetails()
        {
            try
            {
                if (dgrdDetails.CurrentRow.Index>=0 && dgrdDetails.CurrentCell.ColumnIndex == 4)
                {
                    string strVType = Convert.ToString(dgrdDetails.CurrentRow.Cells["voucherType"].Value), strVNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["voucherNo"].Value), strParticular=Convert.ToString(dgrdDetails.CurrentRow.Cells["particulars"].Value);
                    string[] strVoucherNo = strVNo.Split(' ');
                    if (strVType == "Purchase")
                    {
                        if (strVoucherNo.Length > 1)
                        {
                            if (MainPage.strSoftwareType == "RETAIL")
                            {
                                PurchaseBook_Retail_Merge objPurchase = new PurchaseBook_Retail_Merge(strVoucherNo[0], strVoucherNo[1]);
                                objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objPurchase.ShowInTaskbar = true;
                                objPurchase.Show();
                            }
                            else
                            {
                                if (MainPage._bCustomPurchase)
                                {
                                    PurchaseBook_Retail_Custom objPurchase = new PurchaseBook_Retail_Custom(strVoucherNo[0], strVoucherNo[1]);
                                    objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objPurchase.ShowInTaskbar = true;
                                    objPurchase.Show();
                                }
                                else
                                {
                                    PurchaseBook_Trading objPurchase = new PurchaseBook_Trading(strVoucherNo[0], strVoucherNo[1]);
                                    objPurchase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                    objPurchase.ShowInTaskbar = true;
                                    objPurchase.Show();
                                }
                            }
                        }
                    }
                    else if (strVType == "Sales Return")
                    {
                        if (strVoucherNo.Length>1)
                        {
                            if (MainPage.strSoftwareType == "AGENT")
                            {
                                if (Control.ModifierKeys == Keys.Control)
                                {
                                    SaleReturn objSalesReturn = new SaleReturn(strVoucherNo[0], strVoucherNo[1]);
                                    objSalesReturn.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                    objSalesReturn.ShowInTaskbar = true;
                                    objSalesReturn.Show();
                                }
                                else
                                {
                                    SaleReturn_Trading objSalesReturn = new SaleReturn_Trading(strVoucherNo[0], strVoucherNo[1]);
                                    objSalesReturn.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                    objSalesReturn.ShowInTaskbar = true;
                                    objSalesReturn.Show();
                                }
                            }                           
                            else if (MainPage.strSoftwareType == "RETAIL" || MainPage.strSoftwareType == "TRADING")
                            {
                                SaleReturn_Retail objSalesReturn = new SaleReturn_Retail(strVoucherNo[0], strVoucherNo[1]);
                                objSalesReturn.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                objSalesReturn.ShowInTaskbar = true;
                                objSalesReturn.Show();
                            }
                        }                        
                    }                   
                    else if (strVType == "Purchase Return")
                    {
                        if (strVoucherNo.Length > 1)
                        {
                            PurchaseReturn_Trading objPurchase = new PurchaseReturn_Trading(strVoucherNo[0], strVoucherNo[1]);
                            objPurchase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                            objPurchase.ShowInTaskbar = true;
                            objPurchase.Show();
                        }
                    }                   
                    else if (strVType == "Sales")
                    {
                        if (strVoucherNo.Length > 1)
                        {
                            if (MainPage.strSoftwareType == "RETAIL")
                            {
                                SaleBook_Retail objSale = new SaleBook_Retail(strVoucherNo[0], strVoucherNo[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                            else
                            {
                                if (MainPage._bCustomPurchase)
                                {
                                    SaleBook_Retail_Custom objSale = new SaleBook_Retail_Custom(strVoucherNo[0], strVoucherNo[1]);
                                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objSale.ShowInTaskbar = true;
                                    objSale.Show();
                                }
                                else
                                {
                                    SaleBook_Trading objSale = new SaleBook_Trading(strVoucherNo[0], strVoucherNo[1]);
                                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                    objSale.ShowInTaskbar = true;
                                    objSale.Show();
                                }
                            }
                        }                       
                    }
                    else if (strVType == "Stock Transfer")
                    {
                        if (strVoucherNo.Length > 1)
                        {
                            StockTransferVoucher objPurchase = new StockTransferVoucher(strVoucherNo[0], strVoucherNo[1]);
                            objPurchase.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                            objPurchase.ShowInTaskbar = true;
                            objPurchase.Show();
                        }
                    }
                }
            }
            catch
            {
            }
        }
      
    }
}
