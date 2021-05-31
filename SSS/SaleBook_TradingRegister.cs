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
    public partial class SaleBook_TradingRegister : Form
    {
        DataBaseAccess dba;
        DataTable dtOrder = null, dtDetails = null;
        public SaleBook_TradingRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        public SaleBook_TradingRegister(string strPName)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                txtSalesParty.Text = strPName;
                GetAllData();
            }
            catch
            {
            }
        }

        private void PurchaseBookRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelMissingSNo.Visible)
                    panelMissingSNo.Visible = false;
                else if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    if (MainPage.strSoftwareType == "RETAIL")
                    {
                        SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSalesParty.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        SearchData objSearch = new SearchData("SALESANDCASHPARTY", "SEARCH Sundry Debtors", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSalesParty.Text = objSearch.strSelectedData;
                    }
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }

        private void txtPFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            Search_Data();
            btnGo.Enabled = true;
        }

        private void Search_Data()
        {
            try
            {
                 if (rdoWithoutLR.Checked || txtSalesParty.Text != "" || MainPage.mymainObject.bShowAllRecord || txtLRNumber.Text.Length > 3)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    {
                        MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        chkDate.Focus();
                    }
                    else if ((chkPSNo.Checked && (txtPFromSNo.Text == "" || txtPToSNo.Text == "")))
                    {
                        MessageBox.Show("Sorry ! Please enter serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        chkPSNo.Focus();
                    }
                    else
                        GetAllData();
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            Search_Data();
            btnSearch.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            panelSearch.Visible = false;
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

                if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
                    strQuery += " and (BillNo >= " + txtPFromSNo.Text + " and BillNo <=" + txtPToSNo.Text + ") ";

                string[] strFullName;
                if (txtSalesParty.Text != "")
                {
                    if (MainPage.strSoftwareType == "RETAIL")
                    {
                        string strCustomer = System.Text.RegularExpressions.Regex.Replace(txtSalesParty.Text, "[^0-9.]", "");
                        if (strCustomer != "")
                        {
                            string[] _strFullName = txtSalesParty.Text.Split(' ');
                            if (_strFullName.Length > 1)
                                strQuery += " and SalePartyID = '" + _strFullName[0].Trim() + "'  ";
                        }
                        else
                            strQuery += " and SalePartyID = '" + txtSalesParty.Text.Trim() + "'  ";
                    }
                    else
                    {
                        strFullName = txtSalesParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery += " and SalePartyID = '" + strFullName[0].Trim() + "'  ";
                    }
                }
                if (txtPetiAgent.Text != "")
                {
                    strFullName = txtPetiAgent.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and Description_1 = '" + strFullName[0].Trim() + "'  ";
                }

                if (txtLRNumber.Text != "")
                    strQuery += " and LRNumber Like ('%" + txtLRNumber.Text + "%') ";

                if (txtBillCode.Text != "")
                    strQuery += " and BillCode='" + txtBillCode.Text + "' ";


                if (txtNetAmt.Text != "")
                    strQuery += " and NetAmt = " + txtNetAmt.Text + " ";

                if (txtScheme.Text != "")
                    strQuery += " and SchemeName='" + txtScheme.Text + "' ";

                if (txtItemGroupName.Text != "")
                    strQuery += " and GroupName='" + txtItemGroupName.Text + "' ";

                if (txtMarketerName.Text != "")
                    strQuery += " and Marketer='" + txtMarketerName.Text + "' ";

                if (txtTransportName.Text != "")
                    strQuery += " and TransportName='" + txtTransportName.Text + "' ";

                if (rdoWithLR.Checked)
                    strQuery += " and LrNumber!='' ";
                else if (rdoWithoutLR.Checked)
                    strQuery += " and LrNumber='' ";

                if (rdoMens.Checked)
                    strQuery += " and DepartmentName='MENS' ";
                else if (rdoWomens.Checked)
                    strQuery += " and DepartmentName='WOMENS' ";
                else if (rdoKids.Checked)
                    strQuery += " and DepartmentName='KIDS' ";
                else if (rdoAccessories.Checked)
                    strQuery += " and DepartmentName='ACCESSORIES' ";
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Sales Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strSubQuery = "";
                strSubQuery = CreateQuery();

                strQuery = " Select PR.*,_SBS.*,Marketer,SchemeAmt,(SalePartyID+' '+Name) PartyName,AgentName,CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt,ISNULL(EDate,'') EDate from SalesBook PR  OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID)_SM  OUTER APPLY (Select (AreaCode+AccountNo+' '+Name)AgentName from SupplierMaster Where ISNULL(Description_1,'') !='' and Description_1!='DIRECT' and (AreaCode+AccountNo)=Description_1)_SM4 OUTER APPLY (Select _Im.GroupName,SUM(Qty) Qty,SUM(SBS.Rate*SBS.Qty) Amt,DepartmentName from SalesBookSecondary SBS OUTER APPLY (Select (CASE WHEN ISNULL(_IM.Other,'')!='' then _Im.Other else _Im.GroupName end) as GroupName,MakeName as DepartmentName from Items _IM Where SBS.ItemName=_IM.ItemName) _Im Where SBS.BillCode=PR.BillCode and SBS.BillNo=PR.BillNo Group by GroupName,DepartmentName) _SBS OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='SALES' and GD.BillCode=PR.BillCode and GD.BillNo=PR.BillNo Group by TaxType) GD  "
                           + " OUTER APPLY(Select Marketer,SchemeName,SUM(ROUND((CASE WHEN SMN.TaxIncluded = 1 and Qty > 0 then((Amount * 100) / (100 + TaxRate)) else Amount end), 2)) SchemeAmt from  SalesBookSecondary SBS CROSS APPLY (Select Top 1 Marketer,SchemeName from OrderBooking OB Where OB.SalePartyID=PR.SalePartyID and RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode) = SBS.SONumber) OB left join SaleTypeMaster SMN On PR.SalesType  = SMN.TaxName  and SMN.SaleType = 'SALES' Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SBS.Rate * 100) / (100 + TaxRate)) else SBS.Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (sbs.SDisPer)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((SBS.Rate * 100) / (100 + TaxRate)) else SBS.Rate end)))*(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + (SBS.SDisPer)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where SBS.ItemName = _IM.ItemName) as GM Where PR.BillCode=SBS.BillCode and PR.BillNo=SBS.BillNo Group by SchemeName,Marketer)_Sales OUTER APPLY (Select TOP 1 LTRIM(RIGHT(CONVERT(VARCHAR(20), MAX(ETD.Date), 100), 7)) as EDate from EditTrailDetails ETD Where ETD.BillType='SALES' and ETD.BillCode=PR.BillCode and ETD.BillNo=PR.BillNo and EditStatus='CREATION')ETD Where PR.BillNo!=0 " + strSubQuery + " Order by BillNo,Date  ";

                dtDetails = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dtDetails);
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in gettting data in SALES register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            double dGAmt = 0, dNetAmt = 0, dTGrossAmt = 0, dTNetAmt = 0, dTaxAmt = 0, dSchemeAmt = 0, dTSchemeAmt = 0, dAmt = 0, dTAmt = 0;
            chkAll.Checked = true;
            if (table.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(table.Rows.Count);
                int rowIndex = 0;
                string strID = "", strNewID = "";

                foreach (DataRow row in table.Rows)
                {
                    strNewID = Convert.ToString(row["ID"]);

                    dGAmt = Convert.ToDouble(row["GrossAmt"]);
                    dNetAmt = Convert.ToDouble(row["NetAmt"]);
                    dSchemeAmt = dba.ConvertObjectToDouble(row["SchemeAmt"]);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row["Amt"]);
                    if (strID != strNewID)
                    {
                        dTGrossAmt += dGAmt;
                        dTNetAmt += dNetAmt;
                        dTSchemeAmt += dSchemeAmt;
                        dTaxAmt += Convert.ToDouble(row["TaxAmt"]);
                        strID = strNewID;
                    }
                    else
                        dgrdDetails.Rows[rowIndex].Visible = false;

                    dgrdDetails.Rows[rowIndex].Cells["chkID"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = strNewID;
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["Date"];
                    dgrdDetails.Rows[rowIndex].Cells["stime"].Value = row["EDate"];                    
                    dgrdDetails.Rows[rowIndex].Cells["billNo"].Value = row["BillCode"] + " " + row["BillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                    dgrdDetails.Rows[rowIndex].Cells["transportName"].Value = row["TransportName"];
                    dgrdDetails.Rows[rowIndex].Cells["station"].Value = row["Station"];
                    dgrdDetails.Rows[rowIndex].Cells["lrNumber"].Value = row["LRNumber"];
                    dgrdDetails.Rows[rowIndex].Cells["waybillNo"].Value = row["WaybillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["lrDate"].Value = row["LRDate"];
                    dgrdDetails.Rows[rowIndex].Cells["salesType"].Value = row["SalesType"];
                    dgrdDetails.Rows[rowIndex].Cells["grossAmt"].Value = dGAmt;
                    dgrdDetails.Rows[rowIndex].Cells["igstAmt"].Value = row["IgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["cgstAmt"].Value = row["cgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["sgstAmt"].Value = row["sgstAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dNetAmt;
                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["schemeAmt"].Value = dSchemeAmt;
                    dgrdDetails.Rows[rowIndex].Cells["itemGroupName"].Value = row["GroupName"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["amt"].Value = dAmt;
                    dgrdDetails.Rows[rowIndex].Cells["marketer"].Value = row["Marketer"];
                    dgrdDetails.Rows[rowIndex].Cells["agentName"].Value = Convert.ToString(row["AgentName"]);
                    dgrdDetails.Rows[rowIndex].Cells["saleBillType"].Value = Convert.ToString(row["SaleBillType"]);
                    dgrdDetails.Rows[rowIndex].Cells["remark"].Value = row["Remark"];

                    rowIndex++;
                }
            }

            lblGrossAmt.Text = dTGrossAmt.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dTNetAmt.ToString("N2", MainPage.indianCurancy);
            lblSchemeAmt.Text = dTSchemeAmt.ToString("N2", MainPage.indianCurancy);
            lblTaxableAmt.Text = (dTNetAmt - dTaxAmt).ToString("N2", MainPage.indianCurancy);
            lblItemAmt.Text = (dTAmt).ToString("N2", MainPage.indianCurancy);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            if (panelSearch.Visible)
                panelSearch.Visible = false;
            else
                panelSearch.Visible = true;
        }

        private void dgrdDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3 && e.RowIndex >= 0)
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        string strType = Convert.ToString(dgrdDetails.CurrentRow.Cells["saleBillType"].Value);
                        ShowSaleBook(strNumber[0], strNumber[1], strType);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Sales Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowSaleBook(string strCode, string strBillNo, string strBillType)
        {
            try
            {
                if (strBillType == "RETAIL")
                {
                    SaleBook_Retail objSale = new SaleBook_Retail(strCode, strBillNo);
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
                else
                {
                    if (MainPage._bCustomPurchase)
                    {
                        SaleBook_Retail_Custom objSale = new SaleBook_Retail_Custom(strCode, strBillNo);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                    else
                    {
                        SaleBook_Trading objSale = new SaleBook_Trading(strCode, strBillNo);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentRow.Index >= 0)
                {
                    if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                        else
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (columnIndex == 3)
                        {
                            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            string[] strNumber = strInvoiceNo.Split(' ');
                            if (strNumber.Length > 1)
                            {
                                string strType = Convert.ToString(dgrdDetails.CurrentRow.Cells["saleBillType"].Value);
                                ShowSaleBook(strNumber[0], strNumber[1], strType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Down Event of Purchase Grid view  in Show Purchase Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("SALES REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objSales;
                        objShow.ShowDialog();

                        objSales.Close();
                        objSales.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

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
                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalGrossAmt", typeof(String));
                myDataTable.Columns.Add("TotalNetAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["chkID"].Value))
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = MainPage.strGRCompanyName;
                        if (chkDate.Checked)
                            row["DatePeriod"] = "From " + txtFromDate.Text + "   To   " + txtToDate.Text;
                        else
                            row["DatePeriod"] = "";

                        row["Party"] = "SALES REGISTER";


                        for (int colIndex = 2; colIndex < dgrdDetails.Columns.Count; colIndex++)
                        {
                            row[colIndex + 1] = dgrdDetails.Columns[colIndex].HeaderText;
                            row[colIndex + 9] = dr.Cells[colIndex].Value;
                            if (colIndex == 9)
                                break;
                        }

                        row["TotalGrossAmt"] = lblGrossAmt.Text;
                        row["TotalNetAmt"] = lblNetAmt.Text;
                        row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        myDataTable.Rows.Add(row);
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["chkID"].Value = chkAll.Checked;
            }
            catch
            {
            }
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
            lblGrossAmt.Text = lblNetAmt.Text = "0.00";
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
            ClearAll();
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objSales);
                        else
                        {
                            objSales.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objSales.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objSales.PrintToPrinter(1, false, 0, 0);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                int rowIndexer = 0;
                if (dgrdDetails.Rows.Count > 0 )
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
                            if ((dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible))
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (Convert.ToBoolean(dgrdDetails.Rows[k].Cells["chkID"].EditedFormattedValue) == true && dgrdDetails.Rows[rowIndexer].Visible)
                            {
                                if (l < dgrdDetails.Columns.Count)
                                    ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                            }

                        }
                        _skipColumn = 0;
                    }


                    ExcelApp.Columns.AutoFit();

                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "SaleBook_Trading";
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

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Purchase Bill";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\PurchaseRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.SaleRegister objRegister = new Reporting.SaleRegister();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
                }
                else
                    strFileName = "";
            }
            catch
            {
                strFileName = "";
            }
            return strFileName;
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void btnMissing_Click(object sender, EventArgs e)
        {
            btnMissing.Enabled = false;
            try
            {
                if (!panelMissingSNo.Visible)
                    ShowMissingSerials();
                else
                    panelMissingSNo.Visible = false;
            }
            catch
            {
            }
            btnMissing.Enabled = true;
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtSalesType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESTYPE", "SEARCH SALES TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesType.Text = objSearch.strSelectedData;
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
                dba.ClearTextBoxOnKeyDown(sender, e);
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

        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtScheme.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtItemGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMCATEGORYNAME", "SEARCH ITEM GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemGroupName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
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
                        strID = Convert.ToString(row.Cells["id"].Value);
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

        private void txtMarketerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MARKETERNAME", "SEARCH MARKETER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMarketerName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTransportName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransportName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPetiAgent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PETIAGENT", "SEARCH PETI AGENT", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPetiAgent.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SaleBook_RetailRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bSaleReport)
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

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panelMissingSNo.Visible = false;
        }

        private void ShowMissingSerials()
        {
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked)
            {
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }
            eDate = eDate.AddDays(1);
            DataTable dt = dba.GetMissingSaleRetailBillNo(sDate, eDate);
            panelMissingSNo.Visible = true;
            dgrdMissingSNo.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdMissingSNo.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdMissingSNo.Rows[rowIndex].Cells["sSno"].Value = (rowIndex + 1) + ".";
                    dgrdMissingSNo.Rows[rowIndex].Cells["missingSNo"].Value = row[0];
                    rowIndex++;
                }
            }
        }
    }
}
