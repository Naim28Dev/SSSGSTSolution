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
    public partial class CustomPurchaseReturnRegister : Form
    {
        DataBaseAccess dba;
        int currentPageNum = 0, pageSize = 1, maxPageNum = 0;
        List<CheckBox> arrPrint = new List<CheckBox>();
        DataTable BindedDT = new DataTable();

        protected internal bool _bSearchStatus = false;
        public CustomPurchaseReturnRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            SetCategory();
            if (txtRowsPerPage.Text == "")
                txtRowsPerPage.Text = "1";
            GetChkSetting("PURCHASERETURN");
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
                if (txtPurchaseParty.Text != "" || MainPage.mymainObject.bShowAllRecord)
                {
                    if (chkDate.Checked && (txtFromDate.Text == "" || txtToDate.Text == ""))
                        MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        GetDataFromDB();
                        SetColounCategory();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Focus();
                }
            }
            catch
            {
            }
            btnSearch.Enabled = btnSearch2.Enabled = true;
            pnlSearch.Visible = false;
            chkGroup2.Visible = false;
        }

        private async void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                //DataTable _dt = dba.GetDataTable(strQuery);
                MainPage.objLoading.StartLoading();
                DataSet ds = await DataBaseAccess.GetDataSetRecordAsync(strQuery);
                MainPage.objLoading.StopLoading();
                if (ds.Tables.Count > 0)
                {
                    DataTable _dt = ds.Tables[0];
                    DataTable dt = ds.Tables[1];
                    DataTable dt1 = ds.Tables[2];

                    BindedDT = _dt.Clone();
                    BindedDT = _dt;

                    BindColumn(_dt);
                    SetPagesForFirst(_dt);
                    //BindDataWithGrid(_dt);
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
            lblTotalPages.Text = (maxPageNum + 1).ToString();
            lblCurrentPage.Text = "1";
            try
            {
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);

                if (chkTaxableAmt.Checked)
                {
                    lblTaxbleAmt.Text = dba.ConvertObjectToDouble(BindedDT.Compute("SUM(STaxableAmt)", "")).ToString("N2", MainPage.indianCurancy);
                    LableTaxbleAmt.Visible = lblTaxbleAmt.Visible = true;
                }
                else
                    LableTaxbleAmt.Visible = lblTaxbleAmt.Visible = false;
            }
            catch (Exception ex) { }
            btnNext.Visible = maxPageNum > 0;
            btnPrev.Visible = currentPageNum > 0;
        }
        private void BindDataWithLabel(DataTable dt)
        {
            LableGrossAmt.Visible = lblGrossAmt.Visible = chkGrossAmt.Checked;
            LableNetTaxbleAmt.Visible = lblNetTaxableAmt.Visible = chkNetTaxable.Checked;
            LableTaxAmt.Visible = lblTaxAmt.Visible = chkTaxAmt.Checked;
            LabelNetAmt.Visible = lblNetAmt.Visible = chkNetAmt.Checked;
            double dNetAmt = 0, dGrossAmt = 0, dTaxAmt, dTaxableAmt;
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                dNetAmt += dba.ConvertObjectToDouble(row["NetAmt"]);
                if (chkGrossAmt.Checked)
                    dGrossAmt += dba.ConvertObjectToDouble(row["GrossAmt"]);
                //if (chkQty.Checked)
                //    dQty += dba.ConvertObjectToDouble( row["TotalQty"]);
                dTaxAmt = dba.ConvertObjectToDouble(row["TaxAmount"]);
                dTaxableAmt = dba.ConvertObjectToDouble(row["NetTaxableAmt"]);

                //if (chkQty.Checked)
                //    dQty += dba.ConvertObjectToDouble( row["TotalQty"]);

                lblGrossAmt.Text = dGrossAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                lblNetTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                //lblTotQty.Text = Convert.ToString(dQty);

            }
        }

        private void BindTotalQty(DataTable dt1)
        {
            double dQty = 0;
            LableQty.Visible = lblTotQty.Visible = chkQty.Checked;
            if (dt1.Rows.Count > 0)
            {
                DataRow row = dt1.Rows[0];
                if (chkQty.Checked)
                    dQty += dba.ConvertObjectToDouble(row["TotalQty"]);
                lblTotQty.Text = Convert.ToString(dQty);
            }
        }
        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    lblVar1Var2.Text = chkSize.Text = MainPage.StrCategory1;
                    txtCategory1.Enabled = true;
                    txtCategory1.Width = 178;
                }
                else
                {
                    txtCategory1.Enabled = chkSize.Enabled = false;
                    txtCategory1.Width = 81;
                }

                if (MainPage.StrCategory2 != "")
                {
                    chkColour.Text = MainPage.StrCategory2;
                    lblVar1Var2.Text += "/" + MainPage.StrCategory2;
                    txtCategory2.Enabled = true;
                    txtCategory1.Width = 81;
                }
                else
                {
                    txtCategory2.Enabled = chkColour.Enabled = false;
                    txtCategory1.Width = 178;
                }

                if (MainPage.StrCategory3 != "")
                {
                    chkVariant3.Text = MainPage.StrCategory3;
                    lblCategory3.Text = MainPage.StrCategory3 + " :";
                    txtCategory3.Enabled = true;
                }
                else
                {
                    lblCategory3.Enabled = txtCategory3.Enabled = chkVariant3.Enabled = false;
                }
                if (MainPage.StrCategory4 != "")
                {
                    chkVariant4.Text = MainPage.StrCategory4;
                    lblCategory4.Text = MainPage.StrCategory4 + " :";
                    txtCategory4.Enabled = true;
                }
                else
                {
                    lblCategory4.Enabled = txtCategory4.Enabled = chkVariant4.Enabled = false;
                }
                if (MainPage.StrCategory5 != "")
                {
                    chkVariant5.Text = MainPage.StrCategory5;
                    lblCategory5.Text = MainPage.StrCategory5 + " :";
                    txtCategory5.Enabled = true;
                }
                else
                {
                    lblCategory5.Enabled = txtCategory5.Enabled = chkVariant5.Enabled = false;
                }
                lblVar1Var2.Text += " :";
            }
            catch (Exception ex)
            {
            }
        }


        private void SetColounCategory()
        {
            try
            {
                if (chkSize.Checked)
                {
                    if (MainPage.StrCategory1 != "")
                    {
                        dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                        dgrdDetails.Columns["variant1"].Visible = true;
                        chkSize.Text = MainPage.StrCategory1;
                    }
                    else
                        dgrdDetails.Columns["variant1"].Visible = false;
                }

                if (chkColour.Checked)
                {
                    if (MainPage.StrCategory2 != "")
                    {
                        dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                        dgrdDetails.Columns["variant2"].Visible = true;
                        chkColour.Text = MainPage.StrCategory2;
                    }
                    else
                        dgrdDetails.Columns["variant2"].Visible = false;
                }

                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["variant3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["variant4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["variant5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant5"].Visible = false;
            }
            catch (Exception ex)
            {
            }
        }
        private string ConvertObjToStringN2(object obj)
        {
            try
            {
                if (Convert.ToString(obj) != "")
                {
                    double d = Convert.ToDouble(obj);
                    return d.ToString("N2", MainPage.indianCurancy);
                }
                else
                    return "";
            }
            catch (Exception ex) { return ""; }
        }
        private void BindDataWithGrid(DataTable _dt)
        {
            // double dNetAmt = 0, dAmt = 0, dGrossAmt = 0, dQty = 0;
            try
            {
                dgrdDetails.Rows.Clear();
                if (_dt.Rows.Count > 0)
                    dgrdDetails.Rows.Add(_dt.Rows.Count);
                int _rowIndex = 0, SNo = (currentPageNum * pageSize);

                foreach (DataRow row in _dt.Rows)
                {
                    SNo++;
                    dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = SNo;

                    if (chkBillNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["billno"].Value = row["BillNo"];
                    if (chkNDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = Convert.ToDateTime(Convert.ToString(row["Date"])).ToString("dd/MM/yyyy");
                    if (chkItemName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["itemname"].Value = row["ItemName"];
                    if (chkSize.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["variant1"].Value = row["variant1"];
                    if (chkColour.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["variant2"].Value = row["variant2"];
                    if (chkVariant3.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["variant3"].Value = row["variant3"];
                    if (chkVariant4.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["variant4"].Value = row["variant4"];
                    if (chkVariant5.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["variant5"].Value = row["variant5"];
                    if (chkQty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value = ConvertObjToStringN2(row["Qty"]);
                    if (chkTaxPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TaxPer"].Value = ConvertObjToStringN2(row["TaxPer"]);
                    if (chkTaxAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TaxAmount"].Value = ConvertObjToStringN2(row["TaxAmount"]);
                    if (chkTaxableAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["STaxableAmt"].Value = ConvertObjToStringN2(row["STaxableAmt"]);
                    if (chkNetTaxable.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["NetTaxableAmt"].Value = ConvertObjToStringN2(row["NetTaxableAmt"]);
                    if (chkGrossAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["grossamt"].Value = ConvertObjToStringN2(row["GrossAmt"]);
                    if (chkBarCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["barcode"].Value = row["BarCode"];
                    if (chkBrandName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["brandname"].Value = row["BrandName"];
                    if (chkRemarks.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["remarks"].Value = row["Remark"];
                    if (chkDesignName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["designname"].Value = row["DesignName"];
                    if (chkRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["rate"].Value = ConvertObjToStringN2(row["Rate"]);
                    if (chkMRP.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["mrp"].Value = ConvertObjToStringN2(row["MRP"]);
                    if (chkAmount.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value = ConvertObjToStringN2(row["Amount"]);
                    if (chkDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["NetDiscount"].Value = ConvertObjToStringN2(row["NetDiscount"]);
                    if (chkOtherAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Otheramt"].Value = ConvertObjToStringN2(row["OtherAmt"]);
                    if (chkCreatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["createdby"].Value = row["CreatedBy"];
                    if (chkUpdatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["updatedby"].Value = row["UpdatedBy"];
                    if (chkSDisPer.Checked)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["sdisper"].Value = ConvertObjToStringN2(row["SDisPer"]);
                        dgrdDetails.Rows[_rowIndex].Cells["Discount"].Value = ConvertObjToStringN2(row["Discount"]);
                    }
                    if (chkPurchaseType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PurchaseType"].Value = row["PurchaseType"];
                    if (chkUnitName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["unitname"].Value = row["UnitName"];
                    if (chkEntryType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["EntryType"].Value = row["EntryType"];
                    //if (chkDhara.Checked)
                    //  dgrdDetails.Rows[_rowIndex].Cells["Dhara"].Value = row["Dhara"];
                    if (chkOtherText.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["OtherText"].Value = row["OtherText"];
                    if (chkPurchaseInvNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["ReverseCharge"].Value = ConvertObjToStringN2(row["ReverseCharge"]);
                    if (chkRoundOffAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["RoundOffAmt"].Value = ConvertObjToStringN2(row["RoundOffAmt"]);
                    if (chkPurchaseBillCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PurchaseBillNo"].Value = row["PurchaseBillNo"];
                    if (chkPurchaseBillDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PurchaseBillDate"].Value = Convert.ToString(row["PurchaseBillDate"]) == "" ? "" : Convert.ToDateTime(Convert.ToString(row["PurchaseBillDate"])).ToString("dd/MM/yyyy");
                    if (chkPurchaseParty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PurchaseParty"].Value = row["PurchaseParty"];
                    //if (chkSaleParty.Checked)
                    //    dgrdDetails.Rows[_rowIndex].Cells["SaleParty"].Value = row["SaleParty"];
                    if (chkFreight.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Freight"].Value = ConvertObjToStringN2(row["Freight"]);
                    if (chkTaxFree.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TaxFree"].Value = ConvertObjToStringN2(row["TaxFree"]);
                    if (chkTotalAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TotalAmt"].Value = ConvertObjToStringN2(row["TotalAmt"]);
                    if (chkOther1.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Other1"].Value = row["Other1"];
                    if (chkOther2.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Other2"].Value = row["SRD_Other2"];
                    if (chkDepartment.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["department"].Value = row["department"];
                    if (chkNetAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["netamt"].Value = ConvertObjToStringN2(row["NetAmt"]);

                    _rowIndex++;
                }
                if (chkTaxableAmt.Checked)
                {
                    lblTaxbleAmt.Text = dba.ConvertObjectToDouble(BindedDT.Compute("SUM(STaxableAmt)", "")).ToString("N2", MainPage.indianCurancy);
                    LableTaxbleAmt.Visible = lblTaxbleAmt.Visible = true;
                }
                else
                    LableTaxbleAmt.Visible = lblTaxbleAmt.Visible = false;
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
            if (chkPurchaseParty.Checked)
                CreateGridviewColumn("PurchaseParty", "Sundry Creditor", "LEFT", 180);
            //if (chkSaleParty.Checked)
            //    CreateGridviewColumn("SaleParty", "Sundry Debitor", "LEFT", 180);
            if (chkBrandName.Checked)
                CreateGridviewColumn("brandname", "Brand Name", "LEFT", 150);
            if (chkItemName.Checked)
                CreateGridviewColumn("ItemName", "Item Name", "LEFT", 180);
            if (chkSize.Checked)
                CreateGridviewColumn("variant1", MainPage.StrCategory1, "LEFT", 50);
            if (chkColour.Checked)
                CreateGridviewColumn("variant2", MainPage.StrCategory2, "LEFT", 50);
            if (chkVariant3.Checked)
                CreateGridviewColumn("variant3", MainPage.StrCategory3, "LEFT", 50);
            if (chkVariant4.Checked)
                CreateGridviewColumn("variant4", MainPage.StrCategory4, "LEFT", 50);
            if (chkVariant5.Checked)
                CreateGridviewColumn("variant5", MainPage.StrCategory5, "LEFT", 50);
            if (chkQty.Checked)
                CreateGridviewColumn("qty", "Qty", "RIGHT", 80);
            if (chkRate.Checked)
                CreateGridviewColumn("rate", "Rate", "RIGHT", 100);
            if (chkAmount.Checked)
                CreateGridviewColumn("amount", "Amount", "RIGHT", 100);
            if (chkGrossAmt.Checked)
                CreateGridviewColumn("grossamt", "Gross Amt", "RIGHT", 100);
            if (chkPurchaseBillCode.Checked)
                CreateGridviewColumn("PurchaseBillNo", "Purchase Bill No", "LEFT", 130);
            if (chkPurchaseBillDate.Checked)
                CreateGridviewColumn("PurchaseBillDate", "PurchaseBillDate", "LEFT", 130);
            if (chkBarCode.Checked)
                CreateGridviewColumn("barcode", "Bar Code", "LEFT", 120);

            if (chkEntryType.Checked)
                CreateGridviewColumn("EntryType", "Entry Type", "LEFT", 100);
            if (chkPurchaseType.Checked)
                CreateGridviewColumn("PurchaseType", "Purchase Type", "LEFT", 120);
            if (chkRemarks.Checked)
                CreateGridviewColumn("remarks", "Remarks", "LEFT", 180);
            if (chkOtherAmt.Checked)
                CreateGridviewColumn("otheramt", "Other Amt", "RIGHT", 100);
            if (chkRoundOffAmt.Checked)
                CreateGridviewColumn("RoundOffAmt", "Round Off Amt", "RIGHT", 120);
            if (chkDisAmt.Checked)
                CreateGridviewColumn("NetDiscount", "Discount Amt", "RIGHT", 110);
            //if (chkDhara.Checked)
            //    CreateGridviewColumn("Dhara", "Dhara", "RIGHT", 100);
            if (chkTaxPer.Checked)
                CreateGridviewColumn("TaxPer", "Tax Per", "RIGHT", 80);
            if (chkTaxAmt.Checked)
                CreateGridviewColumn("TaxAmount", "Tax Amt", "RIGHT", 80);
            if (chkTaxableAmt.Checked)
                CreateGridviewColumn("STaxableAmt", "S.Taxable Amt", "RIGHT", 120);
            if (chkNetTaxable.Checked)
                CreateGridviewColumn("NetTaxableAmt", "Net Taxable Amt", "RIGHT", 130);

            if (chkDesignName.Checked)
                CreateGridviewColumn("designname", "Design Name", "LEFT", 150);
            if (chkOtherText.Checked)
                CreateGridviewColumn("OtherText", "Year", "LEFT", 80);
            if (chkPurchaseInvNo.Checked)
                CreateGridviewColumn("ReverseCharge", "Purchase Inv No", "LEFT", 130);

            if (chkSDisPer.Checked)
            {
                CreateGridviewColumn("sdisper", "S.Dis(%)", "RIGHT", 80);
                CreateGridviewColumn("Discount", "Dis(%)", "RIGHT", 80);
            }
            if (chkTaxFree.Checked)
            {
                //CreateGridviewColumn("STaxFree", "S.TaxFree", "RIGHT", 100);
                CreateGridviewColumn("TaxFree", "TaxFree", "RIGHT", 100);
            }
            if (chkTotalAmt.Checked)
                CreateGridviewColumn("TotalAmt", "TotalAmt", "RIGHT", 100);
            if (chkMRP.Checked)
                CreateGridviewColumn("mrp", "MRP", "RIGHT", 100);
            if (chkUnitName.Checked)
                CreateGridviewColumn("unitname", "Unit Name", "LEFT", 90);
            if (chkOther1.Checked)
                CreateGridviewColumn("Other1", "Other1", "LEFT", 100);
            if (chkOther2.Checked)
                CreateGridviewColumn("Other2", "Other2", "LEFT", 100);
            if (chkDepartment.Checked)
                CreateGridviewColumn("department", "Department", "LEFT", 90);
            if (chkFreight.Checked)
                CreateGridviewColumn("Freight", "Freight", "RIGHT", 100);
            if (chkCreatedBy.Checked)
                CreateGridviewColumn("createdby", "Created By", "LEFT", 100);
            if (chkUpdatedBy.Checked)
                CreateGridviewColumn("updatedby", "Updated By", "LEFT", 100);
            if (chkNetAmt.Checked)
                CreateGridviewColumn("netamt", "Net Amt", "RIGHT", 80);

        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strColumnOther = "", strColumnName = "", strGroupBy = "", strOrderBy = "", strOrderByText = " Order by ", strGroupByText = " Group by BillNo,NetAmt", strGroupBycase = " Group by BillNo,NetAmt", strGroupByOther = " Group by ", strDepartmentQuery = "", strDepartName = "";
            if (chkDepartment.Checked || txtGroupName.Text != "" || txtDepartment.Text != "")
            {
                strDepartmentQuery = " OUTER APPLY (Select Top 1 ISNULL(_Im.MakeName,'') as Department,GroupName from Items _IM Where _IM.ItemName=SRD.ItemName)_IM ";
                strDepartName = ",_Im.Department,_Im.GroupName";
            }
            if (txtPurchaseParty.Text != "")
                strSubQuery += " and ISNULL((PurchasePartyID+' '+Name),PurchasePartyID) = '" + txtPurchaseParty.Text + "' ";

            //if (txtSalesParty.Text != "")
            //{
            //    string[] strFullName = txtSalesParty.Text.Split(' ');
            //    if (strFullName.Length > 1)
            //        strSubQuery += " and SalePartyID='" + strFullName[0].Trim() + "' ";
            //}

            if (txtItemName.Text != "")
                strSubQuery += " and ItemName='" + txtItemName.Text + "' ";
            if (txtCategory1.Text != "")
                strSubQuery += " and Variant1='" + txtCategory1.Text + "' ";
            if (txtCategory2.Text != "")
                strSubQuery += " and Variant2='" + txtCategory2.Text + "' ";
            if (txtCategory3.Text != "")
                strSubQuery += " and Variant3='" + txtCategory3.Text + "' ";
            if (txtCategory4.Text != "")
                strSubQuery += " and Variant4='" + txtCategory4.Text + "' ";
            if (txtCategory5.Text != "")
                strSubQuery += " and Variant5='" + txtCategory5.Text + "' ";
            if (txtBillCode.Text != "")
                strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

            if (txtBrand.Text != "")
                strSubQuery += " and BrandName='" + txtBrand.Text + "' ";

            if (txtNetAmt.Text != "")
                strSubQuery += " and NetAmt='" + txtNetAmt.Text + "'";

            if (txtDepartment.Text != "")
                strSubQuery += " and Department='" + txtDepartment.Text + "' ";

            if (txtGroupName.Text != "")
                strSubQuery += " and GroupName='" + txtGroupName.Text + "' ";
            if (txtLocation.Text != "")
                strSubQuery += " and MaterialLocation='" + txtLocation.Text + "' ";
            if (txtRemark.Text != "")
                strSubQuery += " and Remark LIKE('%" + txtRemark.Text + "%') ";
            if (txtBarCode.Text != "")
                strSubQuery += " and BarCode LIKE('%" + txtBarCode.Text + "%') ";

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
            if (chkItemName.Checked)
            {
                strColumnName += "ItemName,"; strColumnOther += "ItemName,"; strGroupBy += ",ItemName"; strGroupByOther += "ItemName,"; strOrderBy += ",ItemName";
            }
            if (chkSize.Checked)
            {
                strColumnName += "Variant1,"; strColumnOther += "Variant1,"; strGroupBy += ",Variant1"; strGroupByOther += "Variant1,"; strOrderBy += ",Variant1";
            }
            if (chkColour.Checked)
            {
                strColumnName += "Variant2,"; strColumnOther += "Variant2,"; strGroupBy += ",Variant2"; strGroupByOther += "Variant2,"; strOrderBy += ",Variant2";
            }
            if (chkVariant3.Checked)
            {
                strColumnName += "Variant3,"; strColumnOther += "Variant3,"; strGroupBy += ",Variant3"; strGroupByOther += "Variant3,"; strOrderBy += ",Variant3";
            }
            if (chkVariant4.Checked)
            {
                strColumnName += "Variant4,"; strColumnOther += "Variant4,"; strGroupBy += ",Variant4"; strGroupByOther += "Variant4,"; strOrderBy += ",Variant4";
            }
            if (chkVariant5.Checked)
            {
                strColumnName += "Variant5,"; strColumnOther += "Variant5,"; strGroupBy += ",Variant5"; strGroupByOther += "Variant5,"; strOrderBy += ",Variant5";
            }
            if (chkQty.Checked)
            {
                strColumnName += "sum(Qty) Qty,"; strColumnOther += "sum(Qty) Qty,";// strGroupByOther += "Qty,"; strOrderBy += ",Qty";
            }
            if (chkGrossAmt.Checked)
            {
                strColumnName += "GrossAmt,"; strColumnOther += "SUM(GrossAmt) GrossAmt,"; strGroupBy += ",GrossAmt";// strGroupByOther += "GrossAmt,"; strOrderBy += ",GrossAmt";
            }
            if (chkEntryType.Checked)
            {
                strColumnName += "EntryType,"; strColumnOther += "EntryType,"; strGroupBy += ",EntryType"; strGroupByOther += "EntryType,"; strOrderBy += ",EntryType";
            }
            if (chkBarCode.Checked)
            {
                strColumnName += "BarCode,"; strColumnOther += "BarCode,"; strGroupBy += ",BarCode"; strGroupByOther += "BarCode,"; strOrderBy += ",BarCode";
            }
            if (chkBrandName.Checked)
            {
                strColumnName += "BrandName,"; strColumnOther += "BrandName,"; strGroupBy += ",BrandName"; strGroupByOther += "BrandName,"; strOrderBy += ",BrandName";
            }

            if (chkDepartment.Checked)
            {
                strColumnName += "Department,"; strColumnOther += "Department,"; strGroupBy += ",Department"; strGroupByOther += "Department,"; strOrderBy += ",Department";
            }

            if (chkRemarks.Checked)
            {
                strColumnName += "Remark,"; strColumnOther += "Remark,"; strGroupBy += ",Remark"; strGroupByOther += "Remark,"; strOrderBy += ",Remark";
            }

            if (chkDesignName.Checked)
            {
                strColumnName += "DesignName,"; strColumnOther += "DesignName,"; strGroupBy += ",DesignName"; strGroupByOther += "DesignName,"; strOrderBy += ",DesignName";
            }
            if (chkRate.Checked)
            {
                strColumnName += "Rate,"; strColumnOther += "Rate,"; strGroupBy += ",Rate"; strGroupByOther += "Rate,"; strOrderBy += ",Rate";
            }
            if (chkMRP.Checked)
            {
                strColumnName += "MRP,"; strColumnOther += "MRP,"; strGroupBy += ",MRP"; strGroupByOther += "MRP,"; strOrderBy += ",MRP";
            }
            if (chkAmount.Checked)
            {
                strColumnName += "sum(Amount) Amount,"; strColumnOther += "sum(Amount) Amount,"; //strGroupBy += ",Amount"; strGroupByOther += "Amount,"; strOrderBy += ",Amount";
            }
            if (chkDisAmt.Checked)
            {
                strColumnName += "sum(NetDiscount) NetDiscount,"; strColumnOther += "(NetDiscount) NetDiscount,"; strGroupByOther += "NetDiscount,"; //strGroupBy += ",NetDiscount"; strOrderBy += ",NetDiscount";
            }
            if (chkOtherAmt.Checked)
            {
                strColumnName += "othersign,sum(otherAmt) OtherAmt,"; strColumnOther += "(cast(othersign as nvarchar)+' '+cast(sum(otherAmt) as nvarchar)) OtherAmt,"; strGroupByOther += "othersign,OtherAmt,";
                strGroupBy += ",othersign,otherAmt"; strOrderBy += ",OtherAmt";
            }

            if (chkCreatedBy.Checked)
            {
                strColumnName += "CreatedBy,"; strColumnOther += "CreatedBy,"; strGroupBy += ",CreatedBy"; strGroupByOther += "CreatedBy,"; strOrderBy += ",CreatedBy";
            }
            if (chkUpdatedBy.Checked)
            {
                strColumnName += "Updatedby,"; strColumnOther += "Updatedby,"; strGroupBy += ",Updatedby"; strGroupByOther += "Updatedby,"; strOrderBy += ",Updatedby";
            }
            //if (chkDhara.Checked)
            //{
            //    strColumnName += "Dhara,"; strColumnOther += "Dhara,"; strGroupBy += ",Dhara"; strGroupByOther += "Dhara,"; strOrderBy += ",Dhara";
            //}
            if (chkSDisPer.Checked)
            {
                strColumnName += "SDisPer,Discount,"; strColumnOther += "SDisPer,Discount,"; strGroupBy += ",SDisPer,Discount"; strGroupByOther += "SDisPer,Discount,"; strOrderBy += ",SDisPer,Discount";
            }
            if (chkPurchaseType.Checked)
            {
                strColumnName += "PurchaseType,"; strColumnOther += "PurchaseType,"; strGroupBy += ",PurchaseType"; strGroupByOther += "PurchaseType,"; strOrderBy += ",PurchaseType";
            }
            if (chkFreight.Checked)
            {
                strColumnName += "sum(Freight) Freight,"; strColumnOther += "sum(Freight) Freight,"; //strGroupBy += ",PackingAmt"; strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            }
            if (chkTaxFree.Checked)
            {
                strColumnName += "SUM(STaxFree)STaxFree,TaxFree,"; strColumnOther += "sum(STaxFree) STaxFree,SUM(TaxFree) as TaxFree,"; strGroupBy += ",TaxFree"; //strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            }
            if (chkTotalAmt.Checked)
            {
                strColumnName += "sum(TotalAmt) TotalAmt,"; strColumnOther += "sum(TotalAmt) TotalAmt,"; //strGroupBy += ",PackingAmt"; strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            }
            if (chkUnitName.Checked)
            {
                strColumnName += "UnitName,"; strColumnOther += "UnitName,"; strGroupBy += " ,UnitName"; strGroupByOther += " UnitName,"; strOrderBy += ",UnitName";
            }
            if (chkTaxPer.Checked)
            {
                strColumnName += "TaxPer,"; strColumnOther += "TaxPer,"; strGroupBy += " ,TaxPer"; strGroupByOther += " TaxPer,"; strOrderBy += ",TaxPer";
            }
            if (chkOtherText.Checked)
            {
                strColumnName += "OtherText,"; strColumnOther += "OtherText,"; strGroupBy += " ,OtherText"; strGroupByOther += " OtherText,"; strOrderBy += ",OtherText";
            }
            if (chkRoundOffAmt.Checked)
            {
                strColumnName += "RoundOffAmt,"; strColumnOther += "RoundOffAmt,"; strGroupBy += " ,RoundOffAmt"; strGroupByOther += " RoundOffAmt,"; strOrderBy += ",RoundOffAmt";
            }
            if (chkPurchaseBillCode.Checked)
            {
                strColumnName += "PurchaseBillCode,PurchaseBillNo,"; strColumnOther += "PurchaseBillCode+' '+Convert(varchar(10),PurchaseBillNo) PurchaseBillNo,"; strGroupBy += " ,PurchaseBillCode,PurchaseBillNo"; strGroupByOther += " PurchaseBillCode,PurchaseBillNo,"; strOrderBy += ",PurchaseBillCode,PurchaseBillNo";
            }
            if (chkPurchaseBillDate.Checked)
            {
                strColumnName += "PurchaseBillDate,"; strColumnOther += "PurchaseBillDate,"; strGroupBy += " ,PurchaseBillDate"; strGroupByOther += " PurchaseBillDate,"; strOrderBy += ",PurchaseBillDate";
            }
            if (chkPurchaseInvNo.Checked)
            {
                strColumnName += "ReverseCharge,"; strColumnOther += "ReverseCharge,"; strGroupBy += " ,ReverseCharge"; strGroupByOther += " ReverseCharge,"; strOrderBy += ",ReverseCharge";
            }
            if (chkPurchaseParty.Checked)
            {
                strColumnName += "PurchasePartyID,"; strColumnOther += "dbo.GetFullName(PurchasePartyID) as PurchaseParty,"; strGroupBy += " ,PurchasePartyID"; strGroupByOther += " PurchasePartyID,"; strOrderBy += ",PurchaseParty";
            }
            //if (chkSaleParty.Checked)
            //{
            //    strColumnName += "SalePartyID,"; strColumnOther += "dbo.GetFullName(SalePartyID) as SaleParty,"; strGroupBy += " ,SalePartyID"; strGroupByOther += " SalePartyID,"; strOrderBy += ",SaleParty";
            //}
            if (chkOther1.Checked)
            {
                strColumnName += "Other1,"; strColumnOther += "Other1,"; strGroupBy += " ,Other1"; strGroupByOther += " Other1,"; strOrderBy += ",Other1";
            }
            if (chkOther2.Checked)
            {
                strColumnName += "SRD_Other2,"; strColumnOther += "SRD_Other2,"; strGroupBy += " ,SRD_Other2"; strGroupByOther += " SRD_Other2,"; strOrderBy += ",SRD_Other2";
            }
            if (chkTaxableAmt.Checked)
            {
                strColumnName += "SUM(STaxableAmt)STaxableAmt,"; strColumnOther += "SUM(STaxableAmt)STaxableAmt,";
            }
            if (chkNetTaxable.Checked)
            {
                strColumnName += "NetTaxableAmt,"; strColumnOther += "Sum(NetTaxableAmt)NetTaxableAmt,"; strGroupBy += ",NetTaxableAmt"; strOrderBy += ",NetTaxableAmt";
            }
            if (chkNetAmt.Checked)
            {
                strColumnName += "NetAmt,"; strColumnOther += "Sum(NetAmt)NetAmt,"; strGroupBy += ",NetAmt"; strOrderBy += ",NetAmt";
            }
            if (chkTaxAmt.Checked)
            {
                strColumnName += "TaxAmount,"; strColumnOther += "Sum(TaxAmount)TaxAmount,"; strGroupBy += ",TaxAmount"; strOrderBy += ",TaxAmount";
            }

            if (strColumnName != "")
                strColumnName = strColumnName.Remove(strColumnName.Length - 1);
            if (strColumnOther != "")
                strColumnOther = strColumnOther.Remove(strColumnOther.Length - 1);
            if (strGroupByOther != "")
                strGroupByOther = strGroupByOther.Remove(strGroupByOther.Length - 1);

            if (strOrderBy != "")
                strOrderBy = strOrderBy.Substring(1);
            else
                strOrderByText = "";

            if (strGroupBy == "")
                strGroupByText = "";
            else
                strGroupBycase = "";

            if (strGroupByOther.Trim() == "Group by")
                strGroupByOther = "";

            if (strColumnOther == "")
                strQuery = "SELECT 1 as SNo SELECT 2 as SNo SELECT 3 as SNo ";
            else
                strQuery = "select " + strColumnOther + " from (Select " + strColumnName + " from (Select *,(NetAmt - CAST((RoundOffSign + (CAST(RoundOffAmt as varchar))) as Money) - TaxAmount) NetTaxableAmt,CAST((CASE WHEN TaxIncluded = 1 then((Amount * 100) / (100 + TaxRate)) else Amount end) as Numeric(18,2))STaxableAmt from( select SR.*,SRD.BarCode,SRD.BrandName,SRD.DesignName,SRD.ItemName,SRD.SalePartyID,SRD.Variant1,SRD.Variant2,SRD.Variant3,SRD.Variant4,SRD.Variant5,SRD.Qty,SRD.UnitName,SRD.MRP,SRD.SDisPer,SRD.Rate,SRD.Amount,SRD.Other1,SRD.Other2 as SRD_Other2" + strDepartName + ",Convert(Money ,SRD.DisStatus + Convert(varchar(20) ,SRD.Discount)) as Discount,SRD.Dhara,SRD.Freight,SRD.TaxFree as STaxFree,SRD.TotalAmt,SRD.SRBillNo as SaleRetrunBillNo, TaxIncluded ,(Select TOP 1((CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SRD.MRP * 100) / (100 + TaxRate)) else SRD.MRP end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + SRD.SDisPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SRD.MRP * 100) / (100 + TaxRate)) else SRD.MRP end)))  *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + SRD.SDisPer) / 100.00) else 1.00 end))) < _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName = SRD.ItemName )TaxRate from PurchaseReturn SR inner join PurchaseReturnDetails SRD on SR.billcode = SRD.billcode and SR.billno = SRD.billno left join SaleTypeMaster STM on STM.TaxName = SR.PurchaseType and STM.SaleType = 'PURCHASE' " + strDepartmentQuery + " )_sales)__sales OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=PurchasePartyID)_SM Where BillNo!=0 " + strSubQuery + strGroupByText + strGroupBy + strGroupBycase + ")sales " + strGroupByOther + strOrderByText + strOrderBy + " "
                        + " select SUM(NetAmt) NetAmt,SUM(GrossAmt) GrossAmt,SUM(TaxAmount)TaxAmount,SUM(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmount) NetTaxableAmt from (select NetAmt,GrossAmt,TaxAmount,RoundOffSign,RoundOffAmt from (select SR.*,SRD.BarCode,SRD.BrandName,SRD.DesignName,SRD.ItemName,SRD.SalePartyID,SRD.Variant1,SRD.Variant2,SRD.Variant3,SRD.Variant4,SRD.Variant5,SRD.Qty,SRD.UnitName,SRD.MRP,SRD.SDisPer,SRD.Rate,SRD.Amount,SRD.Other1,SRD.Other2 as SRD_Other2" + strDepartName + " from PurchaseReturn SR inner join PurchaseReturnDetails SRD on SR.billcode=SRD.billcode and SR.billno=SRD.billno  " + strDepartmentQuery + ")_sales OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=PurchasePartyID)_SM Where BillNo!=0 " + strSubQuery + "Group by BillNo,NetAmt,GrossAmt,TaxAmount,RoundOffSign,RoundOffAmt)Sales"
                        + "  select sum(Qty) TotalQty from (select SR.*,SRD.BarCode,SRD.BrandName,SRD.DesignName,SRD.ItemName,SRD.SalePartyID,SRD.Variant1,SRD.Variant2,SRD.Variant3,SRD.Variant4,SRD.Variant5,SRD.Qty,SRD.UnitName,SRD.MRP,SRD.SDisPer,SRD.Rate,SRD.Amount,SRD.Other1,SRD.Other2 as SRD_Other2" + strDepartName + ",Convert(Money ,SRD.DisStatus + Convert(varchar(20) ,SRD.Discount)) as Discount,SRD.Dhara,SRD.Freight,SRD.TaxFree as STaxFree,SRD.TotalAmt,SRD.SRBillNo as SaleRetrunBillNo from PurchaseReturn SR inner join PurchaseReturnDetails SRD on SR.billcode=SRD.billcode and SR.billno=SRD.billno " + strDepartmentQuery + " )_sales OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=PurchasePartyID)_SM Where BillNo!=0  " + strSubQuery + " ";

            return strQuery;
        }

        //private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtSalesParty.Text = objSearch.strSelectedData;
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}
        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;
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

        //private void txtTransportName_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        dba.ClearTextBoxOnKeyDown(sender, e);
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtTransportName.Text = objSearch.strSelectedData;
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}

        private void CustomPurchaseReturnRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlSearch.Visible)
                    pnlSearch.Visible = false;
                if (chkGroup2.Visible)
                    chkGroup2.Visible = false;
                else
                    this.Close();
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

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASERETURNCODE", "SEARCH SALE RETURN BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtStation_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        dba.ClearTextBoxOnKeyDown(sender, e);
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION ", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtStation.Text = objSearch.strSelectedData;
        //            ClearAll();
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}



        private void btnSearch2_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch2.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text == "" || txtToDate.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    GetDataFromDB();
                    SetColounCategory();
                }

            }
            catch
            {
            }
            btnSearch2.Enabled = true;
            pnlSearch.Visible = false;
            chkGroup2.Visible = false;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            txtBarCode.Text = txtBrand.Text = txtDepartment.Text = txtItemName.Text = txtLocation.Text = txtPFromSNo.Text = txtPToSNo.Text = txtRemark.Text = "";
            pnlSearch.Visible = false;
            //SearchRecord();
        }

        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = chkGroup2.Visible = true;
        }

        private void txtNetAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                foreach (CheckBox c in chkGroup1.Controls.OfType<CheckBox>())
                {
                    c.Checked = true;
                }
                foreach (CheckBox c in chkGroup2.Controls.OfType<CheckBox>())
                {
                    c.Checked = true;
                }
            }
            else
            {
                foreach (CheckBox c in chkGroup1.Controls.OfType<CheckBox>())
                {
                    c.Checked = false;
                }
                foreach (CheckBox c in chkGroup2.Controls.OfType<CheckBox>())
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
                    saveFileDialog.FileName = "Custom_Purchase_Return_Register";
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

        private void CustomPurchaseReturnRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (_bSearchStatus)
            {
                SearchRecord();
            }
        }

        //private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("DEPARTMENTNAME", "SEARCH DEPARTMENT NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtDepartment.Text = objSearch.strSelectedData;
        //        }
        //        else
        //        {
        //            e.Handled = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void txtBrand_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDNAME", "SELECT BRAND NAME", e.KeyCode);
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

        //private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("ITEMGROUPNAME", "GROUP NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtGroupName.Text = objSearch.strSelectedData;
        //        }
        //        else
        //        {
        //            e.Handled = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

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
                    txtLocation.Text = objSearch.strSelectedData;
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
                if (e.RowIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "BillNo")
                        ShowDetails("PURCHASE RETURN");
                    else if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Purchase Bill No")
                        ShowDetails("PURCHASE");
                    //else if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "SaleBillCode")
                    //    ShowDetails("SALES");
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
                string[] strNumber = strInvoiceNo.Split(' ');
                if (strNumber.Length > 1)
                    dba.ShowTransactionBook(strAccount, strNumber[0], strNumber[1]);
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

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            int _rowIndex = 0;
            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                _rowIndex++;
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
                    txtDepartment.Text = objSearch.strSelectedData;
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
                    txtGroupName.Text = objSearch.strSelectedData;
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

        private void btnSetting_Click(object sender, EventArgs e)
        {
            btnSetting.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to update settings ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    pnlSearch.Visible = chkGroup2.Visible = false;
                    UpdateSetting("PURCHASERETURN");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnSetting.Enabled = true;
        }
        private void UpdateSetting(string BillType)
        {
            string strQuery = "", clmName = "";
            int showHide = 0;
            foreach (Control ctrl in chkGroup1.Controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox chk = (CheckBox)ctrl;
                    showHide = chk.Checked ? 1 : 0;
                    clmName = chk.Name.Substring(3, chk.Name.Length - 3);
                    strQuery += " Update CustomReportSetting set ShowHide = " + showHide + ", UpdateStatus = 1, UpdatedBy = '" + MainPage.strLoginName + "' WHERE BillType = '" + BillType + "' AND ColumnName = '" + clmName + "'"
                                    + " INSERT INTO CustomReportSetting(BillType, ColumnName, ShowHide, InsertStatus, CreatedBy)"
                                    + " SELECT '" + BillType + "','" + clmName + "'," + showHide + ",1,'" + MainPage.strLoginName + "' WHERE(SELECT COUNT(*) FROM CustomReportSetting WHERE BillType = '" + BillType + "' and ColumnName = '" + clmName + "') = 0 ";
                }
                clmName = "";
            }
            foreach (Control ctrl in chkGroup2.Controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox chk = (CheckBox)ctrl;
                    showHide = chk.Checked ? 1 : 0;
                    clmName = chk.Name.Substring(3, chk.Name.Length - 3);
                    strQuery += " Update CustomReportSetting set ShowHide = " + showHide + ", UpdateStatus = 1, UpdatedBy = '" + MainPage.strLoginName + "' WHERE BillType = '" + BillType + "' AND ColumnName = '" + clmName + "'"
                                    + " INSERT INTO CustomReportSetting(BillType, ColumnName, ShowHide, InsertStatus, CreatedBy)"
                                    + " SELECT '" + BillType + "','" + clmName + "'," + showHide + ",1,'" + MainPage.strLoginName + "' WHERE(SELECT COUNT(*) FROM CustomReportSetting WHERE BillType = '" + BillType + "' and ColumnName = '" + clmName + "') = 0 ";
                }
                clmName = "";
            }
            strQuery += " Update CustomReportSetting set ShowHide = " + Convert.ToInt32(txtRowsPerPage.Text) + ", UpdateStatus = 1, UpdatedBy = '" + MainPage.strLoginName + "' WHERE BillType = '" + BillType + "' AND ColumnName = 'RowPerPage'"
                   + " INSERT INTO CustomReportSetting(BillType, ColumnName, ShowHide, InsertStatus, CreatedBy)"
                   + " SELECT '" + BillType + "','RowPerPage'," + Convert.ToInt32(txtRowsPerPage.Text) + ",1,'" + MainPage.strLoginName + "' WHERE(SELECT COUNT(*) FROM CustomReportSetting WHERE BillType = '" + BillType + "' and ColumnName = 'RowPerPage') = 0 ";
            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                MessageBox.Show("Thank you ! " + BillType + " setting updated successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                MessageBox.Show("Sorry ! Unable to update right now!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void GetChkSetting(string BillType)
        {
            try
            {
                string strQuery = "  Select * from CustomReportSetting WHERE BillType = '" + BillType + "' AND ShowHide > 0";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    string clm = "", Value = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        clm = Convert.ToString(dr["ColumnName"]);
                        Value = Convert.ToString(dr["ShowHide"]);
                        switch (clm)
                        {
                            case "GrossAmt":
                                chkGrossAmt.Checked = true; break;
                            //case "SaleParty":
                            //    chkSaleParty.Checked = true; break;
                            case "Qty":
                                chkQty.Checked = true; break;
                            case "Colour":
                                chkColour.Checked = true; break;
                            case "Size":
                                chkSize.Checked = true; break;
                            case "Variant3":
                                chkVariant3.Checked = true; break;
                            case "Variant4":
                                chkVariant4.Checked = true; break;
                            case "Variant5":
                                chkVariant5.Checked = true; break;
                            case "ItemName":
                                chkItemName.Checked = true; break;
                            case "BrandName":
                                chkBrandName.Checked = true; break;
                            case "NDate":
                                chkNDate.Checked = true; break;
                            case "PurchaseParty":
                                chkPurchaseParty.Checked = true; break;
                            case "Amount":
                                chkAmount.Checked = true; break;
                            case "BillNo":
                                chkBillNo.Checked = true; break;
                            case "Rate":
                                chkRate.Checked = true; break;
                            case "PurchaseBillDate":
                                chkPurchaseBillDate.Checked = true; break;
                            case "PurchaseBillCode":
                                chkPurchaseBillCode.Checked = true; break;
                            case "UpdatedBy":
                                chkUpdatedBy.Checked = true; break;
                            case "CreatedBy":
                                chkCreatedBy.Checked = true; break;
                            case "OtherText":
                                chkOtherText.Checked = true; break;
                            case "DesignName":
                                chkDesignName.Checked = true; break;
                            case "TaxableAmt":
                                chkTaxableAmt.Checked = true; break;
                            case "NetTaxable":
                                chkNetTaxable.Checked = true; break;
                            case "Department":
                                chkDepartment.Checked = true; break;
                            case "DisAmt":
                                chkDisAmt.Checked = true; break;
                            case "Other2":
                                chkOther2.Checked = true; break;
                            case "Other1":
                                chkOther1.Checked = true; break;
                            case "TotalAmt":
                                chkTotalAmt.Checked = true; break;
                            case "TaxFree":
                                chkTaxFree.Checked = true; break;
                            case "Freight":
                                chkFreight.Checked = true; break;
                            //case "Dhara":
                            //    chkDhara.Checked = true; break;
                            case "PurchaseInvNo":
                                chkPurchaseInvNo.Checked = true; break;
                            case "RoundOffAmt":
                                chkRoundOffAmt.Checked = true; break;
                            case "TaxPer":
                                chkTaxPer.Checked = true; break;
                            case "TaxAmt":
                                chkTaxAmt.Checked = true; break;
                            case "Remarks":
                                chkRemarks.Checked = true; break;
                            case "PurchaseType":
                                chkPurchaseType.Checked = true; break;
                            case "BarCode":
                                chkBarCode.Checked = true; break;
                            case "EntryType":
                                chkEntryType.Checked = true; break;
                            case "MRP":
                                chkMRP.Checked = true; break;
                            case "SDisPer":
                                chkSDisPer.Checked = true; break;
                            case "UnitName":
                                chkUnitName.Checked = true; break;
                            case "OtherAmt":
                                chkOtherAmt.Checked = true; break;
                            case "NetAmt":
                                chkNetAmt.Checked = true; break;
                            case "RowPerPage":
                                txtRowsPerPage.Text = Value; break;
                        }
                    }
                }
            }
            catch { }
        }

        private void chkBillNo_CheckedChanged(object sender, EventArgs e)
        {
            SetColumnsIndex(sender);
        }
        private void SetColumnsIndex(object sender)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                arrPrint.Add(chk);
            else
                arrPrint.Remove(chk);
        }

        private void txtCategory1_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory1);
        }
        private void SelectVariants(object sender, KeyEventArgs e, string VarName)
        {
            try
            {
                TextBox txt = (TextBox)sender;
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory(txt.Name.Substring(txt.Name.Length - 1, 1), VarName, "", "", "", "", "", "", e.KeyCode, false, "");
                    objSearch.ShowDialog();
                    txt.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void txtCategory2_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory2);
        }

        private void txtCategory3_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory3);
        }

        private void txtCategory4_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory4);
        }

        private void txtCategory5_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory5);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(false);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Return Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(true);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Return Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }
        private void PrintPreviewReport(bool bPrint)
        {
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = 1;
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;
            CrystalDecisions.CrystalReports.Engine.ReportClass objSalesManReport = null;

            if (arrPrint.Count <= 7)
                objSalesManReport = new Reporting.CustomSalesReport();
            else
                objSalesManReport = new Reporting.CustomSalesReport_LandScape();

            objSalesManReport.SetDataSource(CreateDataTable());
            if (bPrint)
            {
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objSalesManReport, false);
                else
                    objSalesManReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
            }
            else
            {
                Reporting.ShowReport objReport = new Reporting.ShowReport("CUSTOM PURCHASE RETURN REPORT PREVIEW");
                objReport.myPreview.ReportSource = objSalesManReport;
                objReport.ShowDialog();

            }
            objSalesManReport.Close();
            objSalesManReport.Dispose();
        }
        private DataTable GetTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("CompanyName", typeof(string));
            _dt.Columns.Add("HeaderName", typeof(string));
            _dt.Columns.Add("CustomerName", typeof(string));
            _dt.Columns.Add("FromDate", typeof(string));
            _dt.Columns.Add("ToDate", typeof(string));

            _dt.Columns.Add("lblClm1", typeof(string));
            _dt.Columns.Add("Clm1", typeof(string));
            _dt.Columns.Add("lblClm2", typeof(string));
            _dt.Columns.Add("Clm2", typeof(string));
            _dt.Columns.Add("lblClm3", typeof(string));
            _dt.Columns.Add("Clm3", typeof(string));
            _dt.Columns.Add("lblClm4", typeof(string));
            _dt.Columns.Add("Clm4", typeof(string));
            _dt.Columns.Add("lblClm5", typeof(string));
            _dt.Columns.Add("Clm5", typeof(string));
            _dt.Columns.Add("lblClm6", typeof(string));
            _dt.Columns.Add("Clm6", typeof(string));
            _dt.Columns.Add("lblClm7", typeof(string));
            _dt.Columns.Add("Clm7", typeof(string));
            _dt.Columns.Add("lblClm8", typeof(string));
            _dt.Columns.Add("Clm8", typeof(string));
            _dt.Columns.Add("lblClm9", typeof(string));
            _dt.Columns.Add("Clm9", typeof(string));
            _dt.Columns.Add("lblClm10", typeof(string));
            _dt.Columns.Add("Clm10", typeof(string));

            _dt.Columns.Add("SNo", typeof(string));
            _dt.Columns.Add("TQty", typeof(string));
            _dt.Columns.Add("TGross", typeof(string));
            _dt.Columns.Add("TTaxable", typeof(string));
            _dt.Columns.Add("TTax", typeof(string));
            _dt.Columns.Add("TNet", typeof(string));
            _dt.Columns.Add("UserName", typeof(string));
            return _dt;
        }

        private void getClmNames(int index, ref string DTClmName, ref string RptDispClm)
        {
            string chkName = "";
            if (index < arrPrint.Count)
                chkName = arrPrint[index].Name;

            //if((arrPrint.Count <= 6 && index == arrPrint.Count) || (arrPrint.Count > 6 && index == arrPrint.Count) || (arrPrint.Count > 9 && index == 9))
            //{
            //    RptDispClm = "Net Amt";
            //    DTClmName = "NetAmt";
            //    return;
            //}
            if (chkName != "")
            {
                switch (chkName)
                {
                    case "chkGrossAmt":
                        RptDispClm = "GrossAmt"; DTClmName = "GrossAmt"; break;
                    case "chkSaleParty":
                        RptDispClm = "SaleParty"; DTClmName = "SaleParty"; break;
                    case "chkQty":
                        RptDispClm = "Qty"; DTClmName = "Qty"; break;
                    case "chkColour":
                        if (chkColour.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory2; DTClmName = "variant2";
                        }
                        break;
                    case "chkBillNo":
                        RptDispClm = "BillNo"; DTClmName = "BillNo"; break;
                    case "chkSize":
                        if (chkSize.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory1; DTClmName = "variant1";
                        }
                        break;
                    case "chkVariant3":
                        if (chkVariant3.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory3; DTClmName = "Variant3";
                        }
                        break;
                    case "chkVariant4":
                        if (chkVariant4.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory4; DTClmName = "Variant4";
                        }
                        break;
                    case "chkVariant5":
                        if (chkVariant5.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory5; DTClmName = "Variant5";
                        }
                        break;
                    case "chkItemName":
                        RptDispClm = "ItemName"; DTClmName = "ItemName"; break;
                    case "chkBrandName":
                        RptDispClm = "BrandName"; DTClmName = "BrandName"; break;
                    case "chkNDate":
                        RptDispClm = "NDate"; DTClmName = "Date"; break;
                    case "chkPurchaseParty":
                        RptDispClm = "PurchaseParty"; DTClmName = "PurchaseParty"; break;
                    case "chkAmount":
                        RptDispClm = "Amount"; DTClmName = "Amount"; break;
                    case "chkRate":
                        RptDispClm = "Rate"; DTClmName = "Rate"; break;
                    case "chkPurchaseBillDate":
                        RptDispClm = "PurchaseBillDate"; DTClmName = "PurchaseBillDate"; break;
                    case "chkPurchaseBillCode":
                        RptDispClm = "PurchaseBillCode"; DTClmName = "PurchaseBillNo"; break;
                    case "chkUpdatedBy":
                        RptDispClm = "UpdatedBy"; DTClmName = "UpdatedBy"; break;
                    case "chkCreatedBy":
                        RptDispClm = "CreatedBy"; DTClmName = "CreatedBy"; break;
                    case "chkOtherText":
                        RptDispClm = "OtherText"; DTClmName = "OtherText"; break;
                    case "chkDesignName":
                        RptDispClm = "DesignName"; DTClmName = "DesignName"; break;
                    case "chkTaxableAmt":
                        RptDispClm = "TaxableAmt"; DTClmName = "NetTaxableAmt"; break;
                    case "chkNetTaxable":
                        RptDispClm = "Net Taxable"; DTClmName = "NetTaxableAmt"; break;
                    case "chkDepartment":
                        RptDispClm = "Department"; DTClmName = "Department"; break;
                    case "chkDisAmt":
                        RptDispClm = "DisAmt"; DTClmName = "NetDiscount"; break;
                    case "chkOther2":
                        RptDispClm = "Other2"; DTClmName = "SRD_Other2"; break;
                    case "chkOther1":
                        RptDispClm = "Other1"; DTClmName = "Other1"; break;
                    case "chkTotalAmt":
                        RptDispClm = "TotalAmt"; DTClmName = "TotalAmt"; break;
                    case "chkTaxFree":
                        RptDispClm = "TaxFree"; DTClmName = "TaxFree"; break;
                    case "chkFreight":
                        RptDispClm = "Freight"; DTClmName = "Freight"; break;
                    //case "chkDhara":
                    //      RptDispClm = "Dhara"; DTClmName = "Dhara"; break;
                    case "chkPurchaseInvNo":
                        RptDispClm = "PurchaseInvNo"; DTClmName = "ReverseCharge"; break;
                    case "chkRoundOffAmt":
                        RptDispClm = "RoundOffAmt"; DTClmName = "RoundOffAmt"; break;
                    case "chkTaxPer":
                        RptDispClm = "TaxPer"; DTClmName = "TaxPer"; break;
                    case "chkTaxAmt":
                        RptDispClm = "TaxAmt"; DTClmName = "TaxAmount"; break;
                    case "chkRemarks":
                        RptDispClm = "Remarks"; DTClmName = "Remark"; break;
                    case "chkPurchaseType":
                        RptDispClm = "PurchaseType"; DTClmName = "PurchaseType"; break;
                    case "chkBarCode":
                        RptDispClm = "BarCode"; DTClmName = "BarCode"; break;
                    case "chkEntryType":
                        RptDispClm = "EntryType"; DTClmName = "EntryType"; break;
                    case "chkMRP":
                        RptDispClm = "MRP"; DTClmName = "MRP"; break;
                    case "chkSDisPer":
                        RptDispClm = "SDisPer"; DTClmName = "SDisPer"; break;
                    case "chkUnitName":
                        RptDispClm = "UnitName"; DTClmName = "UnitName"; break;
                    case "chkOtherAmt":
                        RptDispClm = "OtherAmt"; DTClmName = "OtherAmt"; break;
                    case "chkNetAmt":
                        RptDispClm = "Net Amt"; DTClmName = "NetAmt"; break;
                }
            }
            else
            {
                RptDispClm = "";
                DTClmName = "";
            }
        }
        private string getDateString(DataRow dr, string clm)
        {
            if (Convert.ToString(dr[clm]) != "")
            {
                if (clm.Contains("Date"))
                    return Convert.ToDateTime(dr[clm]).ToString("dd/MM/yyyy");
                else
                    return Convert.ToString(dr[clm]);
            }
            return "";
        }
        private DataTable CreateDataTable()
        {
            DataTable _dt = GetTable();
            if (arrPrint.Count > 0)
            {
                string DTClmName1 = "", RptDispClm1 = "";
                getClmNames(0, ref DTClmName1, ref RptDispClm1);
                string DTClmName2 = "", RptDispClm2 = "";
                getClmNames(1, ref DTClmName2, ref RptDispClm2);
                string DTClmName3 = "", RptDispClm3 = "";
                getClmNames(2, ref DTClmName3, ref RptDispClm3);
                string DTClmName4 = "", RptDispClm4 = "";
                getClmNames(3, ref DTClmName4, ref RptDispClm4);
                string DTClmName5 = "", RptDispClm5 = "";
                getClmNames(4, ref DTClmName5, ref RptDispClm5);
                string DTClmName6 = "", RptDispClm6 = "";
                getClmNames(5, ref DTClmName6, ref RptDispClm6);
                string DTClmName7 = "", RptDispClm7 = "";
                getClmNames(6, ref DTClmName7, ref RptDispClm7);
                string DTClmName8 = "", RptDispClm8 = "";
                getClmNames(7, ref DTClmName8, ref RptDispClm8);
                string DTClmName9 = "", RptDispClm9 = "";
                getClmNames(8, ref DTClmName9, ref RptDispClm9);
                string DTClmName10 = "", RptDispClm10 = "";
                getClmNames(9, ref DTClmName10, ref RptDispClm10);

                int index = 0;
                foreach (DataRow dr in BindedDT.Rows)
                {
                    DataRow _row = _dt.NewRow();
                    _row["SNo"] = index = index + 1;
                    _row["CompanyName"] = MainPage.strPrintComapanyName;

                    if (DTClmName1 != "")
                    {
                        _row["lblClm1"] = RptDispClm1;
                        _row["Clm1"] = getDateString(dr, DTClmName1);
                    }
                    if (DTClmName2 != "")
                    {
                        _row["lblClm2"] = RptDispClm2;
                        _row["Clm2"] = getDateString(dr, DTClmName2);
                    }
                    if (DTClmName3 != "")
                    {
                        _row["lblClm3"] = RptDispClm3;
                        _row["Clm3"] = getDateString(dr, DTClmName3);
                    }
                    if (DTClmName4 != "")
                    {
                        _row["lblClm4"] = RptDispClm4;
                        _row["Clm4"] = getDateString(dr, DTClmName4);
                    }
                    if (DTClmName5 != "")
                    {
                        _row["lblClm5"] = RptDispClm5;
                        _row["Clm5"] = getDateString(dr, DTClmName5);
                    }
                    if (DTClmName6 != "")
                    {
                        _row["lblClm6"] = RptDispClm6;
                        _row["Clm6"] = getDateString(dr, DTClmName6);
                    }
                    if (DTClmName7 != "")
                    {
                        _row["lblClm7"] = RptDispClm7;
                        _row["Clm7"] = getDateString(dr, DTClmName7);
                    }
                    if (DTClmName8 != "")
                    {
                        _row["lblClm8"] = RptDispClm8;
                        _row["Clm8"] = getDateString(dr, DTClmName8);
                    }
                    if (DTClmName9 != "")
                    {
                        _row["lblClm9"] = RptDispClm9;
                        _row["Clm9"] = getDateString(dr, DTClmName9);
                    }
                    if (DTClmName10 != "")
                    {
                        _row["lblClm10"] = RptDispClm10;
                        _row["Clm10"] = getDateString(dr, DTClmName10);
                    }

                    if (chkQty.Checked)
                        _row["TQty"] = "Total Qty : " + lblTotQty.Text;
                    if (chkGrossAmt.Checked)
                        _row["TGross"] = "Gross Amt : " + lblGrossAmt.Text;
                    if (chkTaxableAmt.Checked)
                        _row["TTaxable"] = "Taxable Amt : " + lblNetTaxableAmt.Text;
                    if (chkTaxAmt.Checked)
                        _row["TTax"] = "Tax Amt : " + lblTaxAmt.Text;

                    _row["TNet"] = "Net Amt : " + lblNetAmt.Text;

                    if (txtPurchaseParty.Text != "")
                        _row["CustomerName"] = "Customer : " + txtPurchaseParty.Text;

                    _row["HeaderName"] = "Custom Purchase Return Report";

                    if (txtFromDate.Text.Length == 10)
                    {
                        _row["FromDate"] = txtFromDate.Text;
                        _row["ToDate"] = "  To  " + txtToDate.Text;
                    }

                    _row["UserName"] = "Printed By : " + MainPage.strLoginName;

                    _dt.Rows.Add(_row);
                }
            }
            return _dt;
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            chkGroup2.Visible = !chkGroup2.Visible;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            BindPrevInGV();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            BindNextInGV();
        }
        private void BindNextInGV()
        {
            try
            {
                if (currentPageNum < maxPageNum)
                    currentPageNum += 1;
                pageSize = (int)dba.ConvertObjectToDouble(txtRowsPerPage.Text);
                lblCurrentPage.Text = (currentPageNum + 1).ToString();
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
        }
        private void BindPrevInGV()
        {
            try
            {
                if (currentPageNum > 0)
                    currentPageNum -= 1;
                pageSize = (int)dba.ConvertObjectToDouble(txtRowsPerPage.Text);
                lblCurrentPage.Text = (currentPageNum + 1).ToString();
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
        }

        private void txtRowsPerPage_Leave(object sender, EventArgs e)
        {
            if (dba.ConvertObjectToDouble(txtRowsPerPage.Text) <= 0)
            {
                txtRowsPerPage.Text = "1";
            }
            if (BindedDT.Rows.Count > 0)
            {
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
                lblTotalPages.Text = (maxPageNum + 1).ToString();
                lblCurrentPage.Text = "1";
                try
                {
                    currentPageNum = (int)dba.ConvertObjectToDouble(lblCurrentPage.Text) - 1;
                    DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                    BindDataWithGrid(dtPage);
                }
                catch (Exception ex) { }
            }
            btnNext.Visible = maxPageNum > 0;
            btnPrev.Visible = currentPageNum > 0;
        }

        private void lblCurrentPage_Leave(object sender, EventArgs e)
        {
            if (dba.ConvertObjectToDouble(lblCurrentPage.Text) <= 0)
            {
                lblCurrentPage.Text = "1";
            }
            else if (dba.ConvertObjectToDouble(lblCurrentPage.Text) > maxPageNum + 1)
            {
                lblCurrentPage.Text = (maxPageNum + 1).ToString();
            }

            try
            {
                currentPageNum = (int)dba.ConvertObjectToDouble(lblCurrentPage.Text) - 1;
                pageSize = (int)dba.ConvertObjectToDouble(txtRowsPerPage.Text);
                DataTable dtPage = BindedDT.Rows.Cast<DataRow>().Skip((currentPageNum) * pageSize).Take(pageSize).CopyToDataTable();
                BindDataWithGrid(dtPage);
            }
            catch { }
            btnNext.Visible = (currentPageNum < maxPageNum);
            btnPrev.Visible = (currentPageNum > 0);
            btnExport.Focus();
        }

        private void txtRowsPerPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void lblCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
    }
}
