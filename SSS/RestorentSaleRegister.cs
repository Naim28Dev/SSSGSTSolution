using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class RestorentSaleRegister : Form
    {
        DataBaseAccess dba;
        DataTable BindedDT;
        int currentPageNum = 0, pageSize = 1, maxPageNum = 0;
        protected internal bool _bSearchStatus = false;
        public RestorentSaleRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtRowsPerPage.Text = "15";
            lblCurrentPage.Text = "1";
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            try
            {
                btnSearch.Enabled = btnSearch2.Enabled = false;
                if (txtSalesParty.Text != "" || MainPage.mymainObject.bShowAllRecord)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                        MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        GetDataFromDB();
                        // SetColounCategory();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch
            {
            }
            btnSearch.Enabled = btnSearch2.Enabled = true;
            pnlSearch.Visible = false;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                //DataTable _dt = dba.GetDataTable(strQuery);
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable _dt = ds.Tables[0];
                    DataTable dt = ds.Tables[1];
                    DataTable dt1 = ds.Tables[2];

                    BindColumn(_dt);
                    SetPagesForFirst(_dt);
                    BindDataWithLabel(dt);
                    BindTotalQty(dt1);
                }
            }
            catch (Exception ex)
            { }
        }
        private void SetPagesForFirst(DataTable table)
        {
            BindedDT = table.Clone();
            BindedDT = table;

            currentPageNum = 0;
            if (txtRowsPerPage.Text=="")
                pageSize = 1;
            pageSize = (int)dba.ConvertObjectToDouble(txtRowsPerPage.Text);
            if (pageSize > BindedDT.Rows.Count)
            {
                maxPageNum = 0;
                pageSize = BindedDT.Rows.Count;
            }
            else
            {
                double max = (double)BindedDT.Rows.Count / (double)pageSize;
                if ((max - (int)max) > 0)
                    maxPageNum = (int)max;
                else
                    maxPageNum = (int)max - 1;
            }
            try
            {
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = maxPageNum > 0;
            btnPrev.Visible = currentPageNum > 0;
            lblCurrentPage.Text = (currentPageNum + 1).ToString(); lblPages.Text = (maxPageNum + 1).ToString();
        }
        private void BindNextInGV()
        {
            try
            {
                if (currentPageNum < maxPageNum)
                    currentPageNum += 1;
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
            lblCurrentPage.Text = (currentPageNum+1).ToString(); lblPages.Text = (maxPageNum+1).ToString();
        }
        private void BindPrevInGV()
        {
            try
            {
                if (currentPageNum > 0)
                    currentPageNum -= 1;
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
            lblCurrentPage.Text = (currentPageNum + 1).ToString(); lblPages.Text = (maxPageNum + 1).ToString();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            BindNextInGV();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            BindPrevInGV();
        }

        private void BindDataWithLabel(DataTable dt)
        {
            double dNetAmt = 0, dGrossAmt = 0, dQty = 0, dTaxAmt = 0, dTaxableAmt = 0;
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                dNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                dTaxAmt = dba.ConvertObjectToDouble(row["TaxAmt"]);
                dTaxableAmt = dba.ConvertObjectToDouble(row["NetTaxableAmt"]);
                dGrossAmt = dba.ConvertObjectToDouble(row["GrossAmt"]);

                lblGrossAmt.Text = dGrossAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
            }
        }

        private void BindTotalQty(DataTable dt1)
        {
            double dQty = 0;
            if (dt1.Rows.Count > 0)
            {
                DataRow row = dt1.Rows[0];

                if (chkQty.Checked)
                    dQty += dba.ConvertObjectToDouble(row["TotalQty"]);

                lblTotQty.Text = Convert.ToString(dQty);

            }
        }
        //private void SetCategory()
        //{
        //    try
        //    {
        //        if (MainPage.StrCategory1 != "")
        //        {
        //            chkSize.Text = MainPage.StrCategory1;
        //        }
        //        else
        //        {
        //            chkSize.Enabled = false;
        //        }

        //        if (MainPage.StrCategory2 != "")
        //        {

        //            chkColour.Text = MainPage.StrCategory2;
        //        }
        //        else
        //        {
        //            chkColour.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void SetColounCategory()
        //{
        //    try
        //    {
        //        if (chkSize.Checked)
        //        {
        //            if (MainPage.StrCategory1 != "")
        //            {
        //                dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
        //                dgrdDetails.Columns["variant1"].Visible = true;
        //                chkSize.Text = MainPage.StrCategory1;
        //            }
        //            else
        //                dgrdDetails.Columns["variant1"].Visible = false;
        //        }

        //        if (chkColour.Checked)
        //        {
        //            if (MainPage.StrCategory2 != "")
        //            {
        //                dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
        //                dgrdDetails.Columns["variant2"].Visible = true;
        //                chkColour.Text = MainPage.StrCategory2;
        //            }
        //            else
        //                dgrdDetails.Columns["variant2"].Visible = false;
        //        }

        //        if (MainPage.StrCategory3 != "")
        //        {
        //            dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
        //            dgrdDetails.Columns["variant3"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant3"].Visible = false;

        //        if (MainPage.StrCategory4 != "")
        //        {
        //            dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
        //            dgrdDetails.Columns["variant4"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant4"].Visible = false;

        //        if (MainPage.StrCategory5 != "")
        //        {
        //            dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
        //            dgrdDetails.Columns["variant5"].Visible = true;
        //        }
        //        else
        //            dgrdDetails.Columns["variant5"].Visible = false;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        private void BindDataWithGrid(DataTable _dt)
        {
            double dNetAmt = 0, dAmt = 0, dGrossAmt = 0, dQty = 0;
            try
            {
                dgrdDetails.Rows.Clear();
                if (_dt.Rows.Count > 0)
                    dgrdDetails.Rows.Add(_dt.Rows.Count);
                int _rowIndex = 0;

                foreach (DataRow row in _dt.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = row["SNo"];

                    if (chkBillNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["billno"].Value = row["BillNo"];
                    if (chkSalesParty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salesParty"].Value = row["SalePartyID"];
                    if (chkNDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = Convert.ToDateTime(Convert.ToString(row["Date"])).ToString("dd/MM/yyyy");
                    if (chkItemName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["itemname"].Value = row["ItemName"];
                    if (chkQty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value = row["Qty"];
                    if (chkTaxAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["taxamt"].Value = row["TaxAmt"];
                    if (chkGrossAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["grossamt"].Value = row["GrossAmt"];
                    if (chkCardAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["cardamt"].Value = row["CardAmt"];
                    if (chkCashAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["cashamt"].Value = row["CashAmt"];
                    if (chkChqAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["ChequeAmt"].Value = row["ChequeAmt"];
                    if (chkTaxableAmt.Checked)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["STaxableAmt"].Value = row["STaxableAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["NetTaxableAmt"].Value = row["NetTaxableAmt"];
                    }
                    if (chkBrandName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["brandname"].Value = row["BrandName"];
                    if (chkSalesMan.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salesman"].Value = row["SalesMan"];
                    if (chkRemarks.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["remarks"].Value = row["Remark"];
                    if (chkDesignName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["designname"].Value = row["DesignName"];
                    if (chkRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["rate"].Value = row["Rate"];
                    if (chkAmount.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value = row["Amount"];
                    if (chkDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disper"].Value = row["DisPer"];
                    if (chkDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disamt"].Value = row["DisAmt"];
                    if (chkOtherAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Otheramt"].Value = row["OtherAmt"];
                    if (chkCreditAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["creditamt"].Value = row["CreditAmt"];
                    if (chkMob.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["mob"].Value = row["MobileNo"];
                    if (chkReturnAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["returnamt"].Value = row["ReturnAmt"];
                    if (chkAdvAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["advamt"].Value = row["AdvanceAmt"];
                    if (chkCreatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["createdby"].Value = row["CreatedBy"];
                    if (chkUpdatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["updatedby"].Value = row["UpdatedBy"];
                    if (chkSpcDicPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdscper"].Value = row["SpecialDscPer"];
                    if (chkSpcDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdisamt"].Value = row["SpecialDscAmt"];
                    if (chkFinalAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["finalamt"].Value = row["FinalAmt"];
                    if (chkSDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["sdisper"].Value = row["SDisPer"];
                    if (chkRoundOfAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["roundofamt"].Value = row["RoundOffAmt"];
                    if (chkSalesType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salestype"].Value = row["SalesType"];
                    if (chkUnitName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["unitname"].Value = row["UnitName"];

                    dgrdDetails.Rows[_rowIndex].Cells["netamt"].Value = dba.ConvertObjectToDouble(row["NetAmt"]).ToString("N2", MainPage.indianCurancy);
                    _rowIndex++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void CreateGridviewColumn(string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
                _column.Name = strColName;
                _column.HeaderText = strColHeader;
                _column.Width = _width;

                _column.SortMode = DataGridViewColumnSortMode.Automatic;

                if (strAlign == "LEFT")
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                    _column.HeaderCell.Style.Font = new Font("Arial", 10F, System.Drawing.FontStyle.Bold);

                }
                else
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                    _column.HeaderCell.Style.Font = new Font("Arial", 10F, System.Drawing.FontStyle.Bold);

                    if (_width != 50)
                        _column.DefaultCellStyle.Format = "N2";
                }
                _column.CellTemplate = dataGridViewCell;
                dgrdDetails.Columns.Add(_column);
            }
            catch { }
        }

        private void BindColumn(DataTable _dt)
        {
            dgrdDetails.Columns.Clear();

            CreateGridviewColumn("sno", "S.No", "RIGHT", 50);

            if (chkBillNo.Checked)
                CreateGridviewColumn("billno", "BillNo", "LEFT", 120);
            if (chkNDate.Checked)
                CreateGridviewColumn("Date", "Date", "LEFT", 100);
            if (chkSalesParty.Checked)
                CreateGridviewColumn("salesParty", "Customer", "LEFT", 180);
            if (chkBrandName.Checked)
                CreateGridviewColumn("brandname", "Table No.", "LEFT", 80);
            if (chkItemName.Checked)
                CreateGridviewColumn("ItemName", "Item", "LEFT", 180);
            if (chkQty.Checked)
                CreateGridviewColumn("qty", "Qty", "RIGHT", 80);
            if (chkUnitName.Checked)
                CreateGridviewColumn("unitname", "Unit Name", "LEFT", 90);
            if (chkRate.Checked)
                CreateGridviewColumn("rate", "Rate", "RIGHT", 100);
            if (chkAmount.Checked)
                CreateGridviewColumn("amount", "Amount", "RIGHT", 100);
            if (chkSalesMan.Checked)
                CreateGridviewColumn("salesman", "Waiter", "LEFT", 120);
            if (chkDesignName.Checked)
                CreateGridviewColumn("designname", "Description", "LEFT", 150);
            if (chkCreditAmt.Checked)
                CreateGridviewColumn("creditamt", "Credit Amt", "RIGHT", 100);
            if (chkCardAmt.Checked)
                CreateGridviewColumn("cardamt", "Card Amt", "RIGHT", 80);
            if (chkCashAmt.Checked)
                CreateGridviewColumn("cashamt", "Cash Amt", "RIGHT", 80);
            if (chkChqAmt.Checked)
                CreateGridviewColumn("ChequeAmt", "Chq Amt", "RIGHT", 80);
            if (chkRemarks.Checked)
                CreateGridviewColumn("remarks", "Remarks", "LEFT", 180);
            if (chkSpcDicPer.Checked)
                CreateGridviewColumn("spcdscper", "Spcl Disc%", "RIGHT", 100);
            if (chkSalesType.Checked)
                CreateGridviewColumn("salestype", "Sales Type", "LEFT", 180);
            if (chkMob.Checked)
                CreateGridviewColumn("mob", "Mobile No", "LEFT", 100);
            if (chkSpcDisAmt.Checked)
                CreateGridviewColumn("spcdisamt", "Spcl DisAmt", "RIGHT", 120);
            if (chkOtherAmt.Checked)
                CreateGridviewColumn("otheramt", "Other Amt", "RIGHT", 100);
            if (chkGrossAmt.Checked)
                CreateGridviewColumn("grossamt", "Gross Amt", "RIGHT", 80);
            if (chkTaxAmt.Checked)
                CreateGridviewColumn("taxamt", "Tax Amt", "RIGHT", 80);
            if (chkDisPer.Checked)
                CreateGridviewColumn("disper", "Dis(%)", "RIGHT", 100);
            if (chkTaxableAmt.Checked)
            {
                CreateGridviewColumn("STaxableAmt", "Taxable Amt", "RIGHT", 110);
                CreateGridviewColumn("NetTaxableAmt", "Net Taxable Amt", "RIGHT", 130);
            }
            if (chkReturnAmt.Checked)
                CreateGridviewColumn("returnamt", "Return Amt", "RIGHT", 100);
            if (chkAdvAmt.Checked)
                CreateGridviewColumn("advamt", "Advance Amt", "RIGHT", 100);
            if (chkSDisPer.Checked)
                CreateGridviewColumn("sdisper", "S.Dis(%)", "RIGHT", 80);   
            if (chkDisAmt.Checked)
                CreateGridviewColumn("disamt", "Dis Amt", "RIGHT", 100);
            if (chkRoundOfAmt.Checked)
                CreateGridviewColumn("roundofamt", "RO Amt", "RIGHT", 70);
            if (chkCreatedBy.Checked)
                CreateGridviewColumn("createdby", "Created By", "LEFT", 100);
            if (chkUpdatedBy.Checked)
                CreateGridviewColumn("updatedby", "Updated By", "LEFT", 100);
            if (chkFinalAmt.Checked)
                CreateGridviewColumn("finalamt", "Final Amt", "RIGHT", 100);
            CreateGridviewColumn("netamt", "Net Amt", "RIGHT", 80);
        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strColumnOther = "", strColumnName = "", strGroupBy = "", strOrderBy = "", strOrderByText = " Order by ", strGroupByText = " Group by BillNo,NetAmt", strGroupBycase = " Group by BillNo,NetAmt", strGroupByOther = " Group by ", strDepartmentQuery = "", strDepartName = "";
            //if (chkDepartment.Checked || txtGroupName.Text != "" || txtDepartment.Text != "")
            //{
            //    strDepartmentQuery = " OUTER APPLY (Select Top 1 ISNULL(_Im.MakeName,'') as Department,GroupName from Items _IM Where _IM.ItemName=SBS.ItemName)_IM ";
            //    strDepartName = ",_Im.Department,_Im.GroupName";
            //}
            if (txtSalesParty.Text != "")
                strSubQuery += " and ISNULL((SalePartyId+' '+Name),SalePartyId) = '" + txtSalesParty.Text + "' ";

            //if (txtTransportName.Text != "")
            //    strSubQuery += " and TransportName='" + txtTransportName.Text + "' ";

            if (txtItemName.Text != "")
                strSubQuery += " and ItemName='" + txtItemName.Text + "' ";

            if (txtBillCode.Text != "")
                strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

            //if (txtStation.Text != "")
            //    strSubQuery += " and Station='" + txtStation.Text + "' ";

            if (txtBrand.Text != "")
                strSubQuery += " and BrandName='" + txtBrand.Text + "' ";

            if (txtNetAmt.Text != "")
                strSubQuery += " and NetAmt='" + txtNetAmt.Text + "'";

            //if (txtDepartment.Text != "")
            //    strSubQuery += " and Department='" + txtDepartment.Text + "' ";

            //if (txtGroupName.Text != "")
            //    strSubQuery += " and GroupName='" + txtGroupName.Text + "' ";
            //if (txtLocation.Text != "")
            //    strSubQuery += " and MaterialLocation='" + txtLocation.Text + "' ";
            if (txtRemark.Text != "")
                strSubQuery += " and Remark LIKE('%" + txtRemark.Text + "%') ";
            if (txtSalesMan.Text != "")
                strSubQuery += " and [SalesMan] LIKE('" + txtSalesMan.Text + "') ";
            if (txtMobileNo.Text != "")
                strSubQuery += " and MobileNo LIKE('%" + txtMobileNo.Text + "%') ";
            //if (txtBarCode.Text != "")
            //    strSubQuery += " and BarCode LIKE('%" + txtBarCode.Text + "%') ";

            if (rdoCashAmt.Checked)
                strSubQuery += " and CashAmt>0 ";
            else if (rdoCardAmt.Checked)
                strSubQuery += " and CardAmt>0 ";
            else if (rdoChequeAmt.Checked)
                strSubQuery += " and ChequeAmt>0 ";
            else if (rdoCreditAmt.Checked)
                strSubQuery += " and CreditAmt>0 ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strSubQuery += " and (Date >='" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";

            }

            if (chkPSNo.Checked && txtPFromSNo.Text != "" && txtPToSNo.Text != "")
            {
                strSubQuery += " and (BillNo Between '" + txtPFromSNo.Text + "' and '" + txtPToSNo.Text + "') ";

            }

            if (chkBillNo.Checked)
            {
                strColumnName += "(billCode +' '+cast(billNo as nvarchar)) BillNo, BillNo as _BillNo,";
                strColumnOther = "BillNo,_BillNo,";
                strGroupBy += ",(billCode +' '+cast(billNo as nvarchar)),BillNo";
                strGroupByOther += "BillNo,_BillNo,";
                strOrderBy += ",_BillNo";
            }
            if (chkNDate.Checked)
            {
                strColumnName += "Date,"; strColumnOther += "Date,"; strGroupBy += ",Date"; strGroupByOther += "Date,"; strOrderBy += ",Date";
            }
            if (chkSalesParty.Checked)
            {
                strColumnName += "ISNULL((SalePartyId+' '+Name),SalePartyId) SalePartyId,"; strColumnOther += "SalePartyId,"; strGroupBy += ",SalePartyId,Name"; strGroupByOther += "SalePartyId,"; strOrderBy += ",SalePartyId";
            }
            if (chkItemName.Checked)
            {
                strColumnName += "ItemName,"; strColumnOther += "ItemName,"; strGroupBy += ",ItemName"; strGroupByOther += "ItemName,"; strOrderBy += ",ItemName";
            }
            //if (chkSize.Checked)
            //{
            //    strColumnName += "Variant1,"; strColumnOther += "Variant1,"; strGroupBy += ",Variant1"; strGroupByOther += "Variant1,"; strOrderBy += ",Variant1";
            //}
            //if (chkColour.Checked)
            //{
            //    strColumnName += "Variant2,"; strColumnOther += "Variant2,"; strGroupBy += ",Variant2"; strGroupByOther += "Variant2,"; strOrderBy += ",Variant2";
            //}
            if (chkQty.Checked)
            {
                strColumnName += "sum(Qty) Qty,"; strColumnOther += "sum(Qty) Qty,";// strGroupByOther += "Qty,"; strOrderBy += ",Qty";
            }
            if (chkGrossAmt.Checked)
            {
                strColumnName += "(GrossAmt) GrossAmt,"; strColumnOther += "SUM(GrossAmt) GrossAmt,"; strGroupBy += ",GrossAmt";// strGroupByOther += "GrossAmt,"; strOrderBy += ",GrossAmt";
            }
            if (chkTaxAmt.Checked)
            {
                strColumnName += "TaxAmt,"; strColumnOther += "SUM(TaxAmt) TaxAmt,"; strGroupBy += ",TaxAmt";// strGroupByOther += "TaxAmt,"; strOrderBy += ",TaxAmt";
            }
            if (chkTaxableAmt.Checked)
            {
                strColumnName += "SUM(STaxableAmt) STaxableAmt,(NetTaxableAmt) NetTaxableAmt,"; strColumnOther += "sum(STaxableAmt) STaxableAmt,sum(NetTaxableAmt) NetTaxableAmt,"; strGroupBy += ",NetTaxableAmt";// strGroupByOther += "GrossAmt,"; strOrderBy += ",GrossAmt";
            }
            if (chkCardAmt.Checked)
            {
                strColumnName += "(CardAmt) CardAmt,"; strColumnOther += "sum(CardAmt) CardAmt,"; strGroupBy += ",CardAmt"; //strGroupByOther += "CardAmt,"; strOrderBy += ",CardAmt";
            }
            if (chkCashAmt.Checked)
            {
                strColumnName += "(CashAmt) CashAmt,"; strColumnOther += "sum(CashAmt) CashAmt,"; strGroupBy += ",CashAmt"; //strGroupByOther += "CashAmt,"; strOrderBy += ",CashAmt";
            }
            //if (chkBarCode.Checked)
            //{
            //    strColumnName += "BarCode,BarCode_S,"; strColumnOther += "BarCode,BarCode_S,"; strGroupBy += ",BarCode,BarCode_S"; strGroupByOther += "BarCode,BarCode_S,"; strOrderBy += ",BarCode,BarCode_S";
            //}
            if (chkBrandName.Checked)
            {
                strColumnName += "BrandName,"; strColumnOther += "BrandName,"; strGroupBy += ",BrandName"; strGroupByOther += "BrandName,"; strOrderBy += ",BrandName";
            }
            //if (chkDepartment.Checked)
            //{
            //    strColumnName += "Department,"; strColumnOther += "Department,"; strGroupBy += ",Department"; strGroupByOther += "Department,"; strOrderBy += ",Department";
            //}
            //if (chkSaleIncentives.Checked)
            //{
            //    strColumnName += "Other2 as SalesIncentive,"; strColumnOther += "SalesIncentive,"; strGroupBy += ",Other2"; strGroupByOther += "SalesIncentive,"; strOrderBy += ",SalesIncentive";
            //}
            //if (chkSaleIncentives.Checked)
            //{
            //    strColumnName += @"SUM((CASE WHEN SaleBillType='RETAIL' and (SaleIncentive LIKE '%\%%' ESCAPE '\') then ((Amount*CAST(Replace(SaleIncentive,'%','') as Money))/100) WHEN SaleBillType='RETAIL' then (Qty*CAST(SaleIncentive as Money)) else 0 end)) SalesIncentive,"; strColumnOther += " CAST(SUM(SalesIncentive) as numeric(18,2)) SalesIncentive,";
            //}

            if (chkSalesMan.Checked)
            {
                strColumnName += "SalesMan as SalesMan ,"; strColumnOther += "SalesMan ,"; strGroupBy += ",SalesMan"; strGroupByOther += "SalesMan,"; strOrderBy += ",SalesMan";
            }
            //if (chkSubParty.Checked)
            //{
            //    strColumnName += "SubPartyID,"; strColumnOther += "SubPartyID,"; strGroupBy += ",SubPartyID"; strGroupByOther += "SubPartyID,"; strOrderBy += ",SubPartyID";
            //}
            //if (chkStation.Checked)
            //{
            //    strColumnName += "Station,"; strColumnOther += "Station,"; strGroupBy += ",Station"; strGroupByOther += "Station,"; strOrderBy += ",Station";
            //}
            //if (chkTransport.Checked)
            //{
            //    strColumnName += "Transportname,"; strColumnOther += "Transportname,"; strGroupBy += ",Transportname"; strGroupByOther += "Transportname,"; strOrderBy += ",Transportname";
            //}
            if (chkRemarks.Checked)
            {
                strColumnName += "Remark,"; strColumnOther += "Remark,"; strGroupBy += ",Remark"; strGroupByOther += "Remark,"; strOrderBy += ",Remark";
            }
            //if (chkLRNo.Checked)
            //{
            //    strColumnName += "LRNumber,"; strColumnOther += "LRNumber,"; strGroupBy += ",LRNumber"; strGroupByOther += "LRNumber,"; strOrderBy += ",LRNumber";
            //}
            //if (chkLRDate.Checked)
            //{
            //    strColumnName += "LRDate,"; strColumnOther += "LRDate,"; strGroupBy += ",LRDate"; strGroupByOther += "LRDate,"; strOrderBy += ",LRDate";
            //}
            if (chkDesignName.Checked)
            {
                strColumnName += "DesignName,"; strColumnOther += "DesignName,"; strGroupBy += ",DesignName"; strGroupByOther += "DesignName,"; strOrderBy += ",DesignName";
            }
            if (chkRate.Checked)
            {
                strColumnName += "Rate,"; strColumnOther += "Rate,"; strGroupBy += ",Rate"; strGroupByOther += "Rate,"; strOrderBy += ",Rate";
            }
            //if (chkMRP.Checked)
            //{
            //    strColumnName += "MRP,"; strColumnOther += "MRP,"; strGroupBy += ",MRP"; strGroupByOther += "MRP,"; strOrderBy += ",MRP";
            //}
            if (chkAmount.Checked)
            {
                strColumnName += "sum(Amount) Amount,"; strColumnOther += "sum(Amount) Amount,"; //strGroupBy += ",Amount"; strGroupByOther += "Amount,"; strOrderBy += ",Amount";
            }
            if (chkDisPer.Checked)
            {
                strColumnName += "DisPer,"; strColumnOther += "DisPer,"; strGroupBy += ",DisPer"; strGroupByOther += "DisPer,"; strOrderBy += ",DisPer";
            }
            if (chkDisAmt.Checked)
            {
                strColumnName += "(DisAmt) DisAmt,"; strColumnOther += "sum(DisAmt) DisAmt,"; strGroupBy += ",DisAmt";// strGroupByOther += "DisAmt,"; strOrderBy += ",DisAmt";
            }
            //if (chkPostageAmt.Checked)
            //{
            //    strColumnName += "(PostageAmt) PostageAmt,"; strColumnOther += "sum(PostageAmt) PostageAmt,"; strGroupBy += ",PostageAmt"; // strGroupByOther += "PostageAmt,"; strOrderBy += ",PostageAmt";
            //}
            if (chkOtherAmt.Checked)
            {
                strColumnName += "othersign,sum(otherAmt) OtherAmt,"; strColumnOther += "(cast(othersign as nvarchar)+' '+cast(sum(otherAmt) as nvarchar)) OtherAmt,"; strGroupByOther += "othersign,OtherAmt,";
                strGroupBy += ",othersign,otherAmt";// strOrderBy += ",OtherAmt";
            }
            if (chkCreditAmt.Checked)
            {
                strColumnName += "(CreditAmt) CreditAmt,"; strColumnOther += "sum(CreditAmt) CreditAmt,"; strGroupBy += ",CreditAmt"; // strGroupByOther += "CreditAmt,"; strOrderBy += ",CreditAmt";
            }
            if (chkChqAmt.Checked)
            {
                strColumnName += "(ChequeAmt) ChequeAmt,"; strColumnOther += "sum(ChequeAmt) ChequeAmt,"; strGroupBy += ",ChequeAmt";// strGroupByOther += "CreditAmt,"; strOrderBy += ",CreditAmt";
            }
            if (chkMob.Checked)
            {
                strColumnName += "MobileNo,"; strColumnOther += "MobileNo,"; strGroupBy += ",MobileNo"; strGroupByOther += "MobileNo,"; strOrderBy += ",MobileNo";
            }
            //if (chkReturnSlipNo.Checked)
            //{
            //    strColumnName += "ReturnSlipNo,"; strColumnOther += "ReturnSlipNo,"; strGroupBy += ",ReturnSlipNo"; strGroupByOther += "ReturnSlipNo,"; strOrderBy += ",ReturnSlipNo";
            //}
            if (chkReturnAmt.Checked)
            {
                strColumnName += "(ReturnAmt) ReturnAmt,"; strColumnOther += "sum(ReturnAmt) ReturnAmt,"; strGroupBy += ",ReturnAmt";// strGroupByOther += "ReturnAmt,"; strOrderBy += ",ReturnAmt";
            }
            //if (chkAdvSlipNo.Checked)
            //{
            //    strColumnName += "AdvanceSlipNo,"; strColumnOther += "AdvanceSlipNo,"; strGroupBy += ",AdvanceSlipNo"; strGroupByOther += "AdvanceSlipNo,"; strOrderBy += ",AdvanceSlipNo";
            //}
            if (chkAdvAmt.Checked)
            {
                strColumnName += "(AdvanceAmt) AdvanceAmt,"; strColumnOther += "sum(AdvanceAmt) AdvanceAmt,"; strGroupBy += ",AdvanceAmt";// strGroupByOther += "AdvanceAmt,"; strOrderBy += ",AdvanceAmt";
            }
            if (chkCreatedBy.Checked)
            {
                strColumnName += "CreatedBy,"; strColumnOther += "CreatedBy,"; strGroupBy += ",CreatedBy"; strGroupByOther += "CreatedBy,"; strOrderBy += ",CreatedBy";
            }
            if (chkUpdatedBy.Checked)
            {
                strColumnName += "Updatedby,"; strColumnOther += "Updatedby,"; strGroupBy += ",Updatedby"; strGroupByOther += "Updatedby,"; strOrderBy += ",Updatedby";
            }
            if (chkSpcDicPer.Checked)
            {
                strColumnName += "SpecialDscPer,"; strColumnOther += "SpecialDscPer,"; strGroupBy += ",SpecialDscPer"; strGroupByOther += "SpecialDscPer,"; strOrderBy += ",SpecialDscPer";
            }
            if (chkSpcDisAmt.Checked)
            {
                strColumnName += "(SpecialDscAmt) SpecialDscAmt,"; strColumnOther += "sum(SpecialDscAmt) SpecialDscAmt,"; strGroupBy += ",SpecialDscAmt"; //strGroupByOther += "SpecialDscAmt,"; strOrderBy += ",SpecialDscAmt";
            }
            if (chkFinalAmt.Checked)
            {
                strColumnName += "(FinalAmt) FinalAmt,"; strColumnOther += "sum(FinalAmt) FinalAmt,"; strGroupBy += ",FinalAmt"; //strGroupByOther += "FinalAmt,"; strOrderBy += ",FinalAmt";
            }
            if (chkSDisPer.Checked)
            {
                strColumnName += "SDisPer,"; strColumnOther += "SDisPer,"; strGroupBy += ",SDisPer"; strGroupByOther += "SDisPer,"; strOrderBy += ",SDisPer";
            }
            if (chkRoundOfAmt.Checked)
            {
                strColumnName += "RoundOffSign,(RoundOffAmt) RoundoffAmt,"; strColumnOther += "(cast(RoundOffSign as nvarchar)+' '+cast((RoundOffAmt) as nvarchar)) RoundoffAmt,"; strGroupByOther += "RoundOffSign,RoundoffAmt,";
                strGroupBy += ",RoundOffSign ,RoundOffAmt"; // strOrderBy += ",RoundoffAmt";
            }
            if (chkSalesType.Checked)
            {
                strColumnName += "SalesType,"; strColumnOther += "SalesType,"; strGroupBy += ",SalesType"; strGroupByOther += "SalesType,"; strOrderBy += ",SalesType";
            }

            //if (chkPackingAmt.Checked)
            //{
            //    strColumnName += "(PackingAmt) PackingAmt,"; strColumnOther += "sum(PackingAmt) PackingAmt,"; strGroupBy += ",PackingAmt";// strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            //}

            //if (chkWayBillNo.Checked)
            //{
            //    strColumnName += "WaybillNo,"; strColumnOther += "WaybillNo,"; strGroupBy += " ,WaybillNo"; strGroupByOther += " WaybillNo,"; strOrderBy += ",WaybillNo";
            //}

            if (chkUnitName.Checked)
            {
                strColumnName += "UnitName,"; strColumnOther += "UnitName,"; strGroupBy += " ,UnitName"; strGroupByOther += " UnitName,"; strOrderBy += ",UnitName";
            }

            //if (chkSaleBillType.Checked)
            //{
            //    strColumnName += "SaleBillType,"; strColumnOther += "SaleBillType,"; strGroupBy += " ,SaleBillType"; strGroupByOther += " SaleBillType,"; strOrderBy += ",SaleBillType";
            //}
            //if (chkPackerName.Checked)
            //{
            //    strColumnName += "Other1 as PackerName,"; strColumnOther += "PackerName,"; strGroupBy += " ,Other1"; strGroupByOther += " PackerName,"; strOrderBy += ",PackerName";
            //}

            //if (strGroupBy != "")
            //    strGroupBy = strGroupBy.Substring(1);

            if (strOrderBy != "")
                strOrderBy = strOrderBy.Substring(1);
            else
                strOrderByText = "";

            if (strGroupBy == "")
                strGroupByText = "";
            else
                strGroupBycase = "";

            if (strColumnOther != "")
                strGroupByOther = strGroupByOther.Remove(strGroupByOther.Length - 1);
            else
                strGroupByOther = "";
            if (strGroupByOther.Trim() == "Group by")
                strGroupByOther = "";

            //strQuery = " Select *,(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt) NetTaxableAmt,CAST((CASE WHEN TaxIncluded=1 then((Rate* 100) / (100 + TaxRate)) else Rate end) as Numeric(18,2))TaxableAmt,Rate from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2,SBS.SalesMan,SBS.SaleIncentive,TaxIncluded" + strDepartName + ",(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SBS.MRP * 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded= 1 then((SBS.MRP* 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName=SBS.ItemName)TaxRate from salesbook SB left join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno left join SaleTypeMaster STM on STM.TaxName=SB.SalesType and STM.SaleType='SALES' " + strDepartmentQuery + " )_Sale ";

            strQuery = "select SNo = Row_Number() over(Order By " + (strOrderBy != "" ? strOrderBy : "SUM(netAmt)") + " )," + strColumnOther + " sum(netAmt) NetAmt from (Select " + strColumnName + " NetAmt from (Select *,(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt) NetTaxableAmt,CAST((CASE WHEN TaxIncluded=1 then((Amount* 100) / (100 + TaxRate)) else Amount end) as Numeric(18,2))STaxableAmt from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2,SBS.SalesMan,SBS.SaleIncentive,TaxIncluded" + strDepartName + ",(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SBS.MRP * 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded= 1 then((SBS.MRP* 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName=SBS.ItemName)TaxRate from salesbook SB left join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno left join SaleTypeMaster STM on STM.TaxName=SB.SalesType and STM.SaleType='SALES' " + strDepartmentQuery + " )_Sale )__Sale OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID)_SM Where BillNo!=0 " + strSubQuery + strGroupByText + strGroupBy + strGroupBycase + ")sales " + strGroupByOther + strOrderByText + strOrderBy + " "
                     + " select SUM(NetAmt) NetAmt,SUM(GrossAmt) GrossAmt,SUM(TaxAmt)TaxAmt,SUM(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt) NetTaxableAmt from (select NetAmt,GrossAmt,TaxAmt,RoundOffSign,RoundOffAmt from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2" + strDepartName + ",SBS.SalesMan,SBS.SaleIncentive from salesbook SB inner join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno  " + strDepartmentQuery + ")_sales OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID)_SM Where BillNo!=0 " + strSubQuery + "Group by BillNo,NetAmt,GrossAmt,TaxAmt,RoundOffSign,RoundOffAmt)Sales"
                     + "  select sum(Qty) TotalQty from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2" + strDepartName + ",SBS.SalesMan,SBS.SaleIncentive from salesbook SB inner join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno " + strDepartmentQuery + ")_sales OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID)_SM Where BillNo!=0  " + strSubQuery + " ";

            return strQuery;
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();

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
                    // txtTransportName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void RestorentSaleRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlSearch.Visible)
                    pnlSearch.Visible = false;
                else
                    this.Close();
            }
            else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.Right)
            {
                BindNextInGV();
            }
            else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.Left)
            {
                BindPrevInGV();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
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

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMRESTORENT", "SEARCH ITEM", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
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

        private void txtStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION ", e.KeyCode);
                    objSearch.ShowDialog();
                    // txtStation.Text = objSearch.strSelectedData;
                    ClearAll();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }



        private void btnSearch2_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch2.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    GetDataFromDB();
                    //  SetColounCategory();
                }

            }
            catch
            {
            }
            btnSearch2.Enabled = true;
            pnlSearch.Visible = false;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            txtSalesParty.Text = txtFromDate.Text = txtToDate.Text = txtItemName.Text = txtBillCode.Text = txtNetAmt.Text = txtPFromSNo.Text = txtPToSNo.Text = "";
            pnlSearch.Visible = false;
        }

        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = true;

        }

        private void txtNetAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                foreach (CheckBox c in groupBox2.Controls.OfType<CheckBox>())
                {
                    c.Checked = true;
                }
                foreach (CheckBox c in groupBox3.Controls.OfType<CheckBox>())
                {
                    c.Checked = true;
                }
            }
            else
            {
                foreach (CheckBox c in groupBox2.Controls.OfType<CheckBox>())
                {
                    c.Checked = false;
                }
                foreach (CheckBox c in groupBox3.Controls.OfType<CheckBox>())
                {
                    c.Checked = false;
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
                    saveFileDialog.FileName = "Custom_Sale_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    }
                    //xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);



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

        private void RestorentSaleRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (_bSearchStatus)
            {
                SearchRecord();
            }
        }

        private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DEPARTMENTNAME", "SEARCH DEPARTMENT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    // txtDepartment.Text = objSearch.strSelectedData;
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

        private void txtBrand_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDRESTORENT", "SELECT TABLE NO", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBrand.Text = objSearch.strSelectedData;
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

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMGROUPNAME", "GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    // txtGroupName.Text = objSearch.strSelectedData;
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

        private void txtLocation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                {
                    SearchData objSearch = new SearchData("MATERIALCENTER", "SELECT LOCATION", e.KeyCode);
                    objSearch.ShowDialog();
                    // txtLocation.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtSalesMan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                {
                    SearchData objSearch = new SearchData("WAITER", "SELECT WAITER", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesMan.Text = objSearch.strSelectedData;

                }
                e.Handled = true;
            }
            catch
            {
            }
        }



        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
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
        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "BillNo")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            dba.ShowTransactionBook("SALES", strNumber[0], strNumber[1]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Sales Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtRowsPerPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtRowsPerPage_Validating(object sender, CancelEventArgs e)
        {
            if (dba.ConvertObjectToDouble(txtRowsPerPage.Text) <= 0)
            {
                txtRowsPerPage.Text = "1";
            }
        }

        private void lblCurrentPage_Leave(object sender, EventArgs e)
        {
            if (dba.ConvertObjectToDouble(lblCurrentPage.Text) <= 0)
                lblCurrentPage.Text = "1";
            if (dba.ConvertObjectToDouble(lblCurrentPage.Text) > dba.ConvertObjectToDouble(lblPages.Text))
                lblCurrentPage.Text = lblPages.Text;

            try
            {
                currentPageNum = (int)dba.ConvertObjectToDouble(lblCurrentPage.Text)-1;
                pageSize = (int)dba.ConvertObjectToDouble(txtRowsPerPage.Text);
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
            lblCurrentPage.Text = (currentPageNum + 1).ToString(); lblPages.Text = (maxPageNum + 1).ToString();
            btnExport.Focus();
        }

        private void txtRowsPerPage_Leave(object sender, EventArgs e)
        {
            try
            {
                SetPagesForFirst(BindedDT);
            }
            catch { }
        }

        private void lblPages_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void lblPages_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void lblPages_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                currentPageNum = maxPageNum;
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
            lblCurrentPage.Text = (currentPageNum + 1).ToString(); lblPages.Text = (maxPageNum + 1).ToString();
        }

        private void lblCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            int _rowIndex = 0;
            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                _rowIndex++;
            }
        }
    }
}
