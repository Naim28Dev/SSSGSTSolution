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
    public partial class CustomPurchaseReport : Form
    {
        DataBaseAccess dba;
        int currentPageNum = 0, pageSize = 1, maxPageNum = 0;
        List<CheckBox> arrPrint = new List<CheckBox>();
        DataTable BindedDT = new DataTable();
        public CustomPurchaseReport()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            if (txtRowsPerPage.Text == "")
                txtRowsPerPage.Text = "1";
            GetChkSetting("PURCHASE");
            SetCategory();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = btnSearch2.Enabled = false;
                if (txtPurchaseParty.Text != "" || MainPage.mymainObject.bShowAllRecord)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                        MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        GetDataFromDB();
                        SetColounCategory();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Purchase Party !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MainPage.objLoading.StartLoading();
                DataSet ds = await DataBaseAccess.GetDataSetRecordAsync(strQuery);
                MainPage.objLoading.StopLoading();
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[1];
                    DataTable _dt = ds.Tables[0];

                    BindedDT = _dt.Clone();
                    BindedDT = _dt;

                    BindColumn(_dt);
                    SetPagesForFirst(_dt);
                    //BindDataWithGrid(_dt);
                    BindDataWithLabel(dt);
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
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(BindedDT.Compute("SUM(STaxableAmt)", "")).ToString("N2", MainPage.indianCurancy);
                    LableTaxbleAmt.Visible = lblTaxableAmt.Visible = true;
                }
                else
                    LableTaxbleAmt.Visible = lblTaxableAmt.Visible = false;
            }
            catch (Exception ex) { }
            btnNext.Visible = maxPageNum > 0;
            btnPrev.Visible = currentPageNum > 0;
        }
        private void BindDataWithLabel(DataTable dt)
        {
            LableQty.Visible = lblTotQty.Visible = chkQty.Checked;
            LableGrossAmt.Visible = lblGrossAmt.Visible = chkGrossAmt.Checked;
            LableNetTaxbleAmt.Visible = lblNetTaxableAmt.Visible = chkNetTaxable.Checked;
            LableTaxAmt.Visible = lblTaxAmt.Visible = chkTaxAmt.Checked;
            LableNetAmt.Visible = lblNetAmt.Visible = chkNetAmt.Checked;
            double dTaxAmt = 0, dTaxableAmt = 0, dNetAmt = 0, dGrossAmt = 0;
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                dNetAmt += dba.ConvertObjectToDouble(row["NetAmt"]);
                if (chkGrossAmt.Checked)
                    dGrossAmt += dba.ConvertObjectToDouble(row["GrossAmt"]);
                if (chkTaxAmt.Checked)
                    dTaxAmt = dba.ConvertObjectToDouble(row["TaxAmt"]);
                if (chkNetTaxable.Checked)
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

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    lblVar1Var2.Text = chkSize.Text = MainPage.StrCategory1;
                    txtCategory1.Enabled = true;
                    txtCategory1.Width = 184;
                }
                else
                {
                    txtCategory1.Enabled = chkSize.Enabled = false;
                    txtCategory1.Width = 79;
                }

                if (MainPage.StrCategory2 != "")
                {
                    chkColour.Text = MainPage.StrCategory2;
                    lblVar1Var2.Text += "/" + MainPage.StrCategory2;
                    txtCategory2.Enabled = true;
                    txtCategory1.Width = 79;
                }
                else
                {
                    txtCategory2.Enabled = chkColour.Enabled = false;
                    txtCategory1.Width = 184;
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
            double dNetAmt = 0, dGrossAmt = 0, dTotalQty = 0, dQty = 0;
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
                    if (chkPurchaseParty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["purchaseParty"].Value = row["PurchaseParty"];
                    if (chkNDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = ConvertDateString(row["Date"]);
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
                    {
                        dQty = Convert.ToDouble(row["Qty"]);
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value = ConvertObjToStringN2(row["Qty"]);
                        dTotalQty += dQty;
                    }
                    if (chkTaxAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["taxamt"].Value = ConvertObjToStringN2(row["TaxAmt"]);
                    if (chkGrossAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["grossamt"].Value = ConvertObjToStringN2(row["GrossAmt"]);
                    if (chkWSDis.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["WSDis"].Value = ConvertObjToStringN2(row["WSDis"]);
                    if (chkWSMRP.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["WSMRP"].Value = ConvertObjToStringN2(row["WSMRP"]);
                    if (chkBarCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["barcode"].Value = row["BarCode"];
                    if (chkBrandName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["brandname"].Value = row["BrandName"];
                    if (chkGodown.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Godown"].Value = row["GodownName"];
                    if (chkSaleMargin.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleMargin"].Value = ConvertObjToStringN2(row["SaleMargin"]);
                    if (chkSaleMRP.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleMRP"].Value = ConvertObjToStringN2(row["SaleMRP"]);

                    if (chkTransport.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Transport"].Value = row["TransportName"];
                    if (chkRemarks.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["remarks"].Value = row["Remark"];
                    if (chkAgent.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Agent"].Value = row["Agent"];
                    if (chkTransMode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TransportMode"].Value = row["TransportMode"];
                    if (chkWeight.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PackWeight"].Value = ConvertObjToStringN2(row["PackWeight"]);
                    if (chkNoofPackage.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["NoOfPacks"].Value = ConvertObjToStringN2(row["NoOfPacks"]);
                    if (chkReceivedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["ReceivedBy"].Value = row["ReceivedBy"];
                    if (chkCountedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["CountedBy"].Value = row["CountedBy"];
                    if (chkBarCodedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BarCodedBy"].Value = row["BarCodedBy"];
                    if (chkTaxFree.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TaxFree"].Value = ConvertObjToStringN2(row["TaxFree"]);
                    if (chkSaleDis.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleDis"].Value = ConvertObjToStringN2(row["SaleDis"]);
                    if (chkSaleRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleRate"].Value = ConvertObjToStringN2(row["saleRate"]);
                    if (chkDesignName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["designname"].Value = row["DesignName"];
                    if (chkRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["rate"].Value = ConvertObjToStringN2(row["Rate"]);
                    if (chkMRP.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["mrp"].Value = ConvertObjToStringN2(row["MRP"]);
                    if (chkAmount.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value = ConvertObjToStringN2(row["Amount"]);
                    if (chkDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disper"].Value = ConvertObjToStringN2(row["DiscPer"]);
                    if (chkDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disamt"].Value = ConvertObjToStringN2(row["DiscAmt"]);
                    if (chkInvoiceNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["InvoiceNo"].Value = row["Invoiceno"];
                    if (chkOtherAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Otheramt"].Value = ConvertObjToStringN2(row["OtherAmt"]);
                    if (chkInvoiceDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["InvoiceDate"].Value = ConvertDateString(row["InvoiceDate"]);

                    if (chkTaxableAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["STaxableAmt"].Value = ConvertObjToStringN2(row["STaxableAmt"]);
                    if (chkNetTaxable.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["NetTaxableAmt"].Value = ConvertObjToStringN2(row["NetTaxableAmt"]);
                    if (chkTCSAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TCSAmt"].Value = ConvertObjToStringN2(row["TCSAmt"]);

                    if (chkSpcDicPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdscper"].Value = ConvertObjToStringN2(row["SpecialDscPer"]);
                    if (chkSpcDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdisamt"].Value = ConvertObjToStringN2(row["SpecialDscAmt"]);
                    if (chkSDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["sdisper"].Value = ConvertObjToStringN2(row["SDisPer"]);
                    if (chkRoundOfAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["roundofamt"].Value = ConvertObjToStringN2(row["RoundOff"]);
                    if (chkPurchaseType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["purchasetype"].Value = row["PurchaseType"];
                    if (chkPackingAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["packingamt"].Value = ConvertObjToStringN2(row["PackingAmt"]);
                    if (chkUnitName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["unitname"].Value = row["UnitName"];
                    if (chkCreatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["createdby"].Value = row["CreatedBy"];
                    if (chkUpdatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["updatedby"].Value = row["UpdatedBy"];
                    if (chkPurchaseStatus.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["StockStatus"].Value = row["StockStatus"];
                    if (chkDepartment.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["department"].Value = row["Department"];
                    if (chkTransport2.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["TransportName2"].Value = row["TransportName2"];
                    if (chkLRNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRNumber"].Value = row["LRNumber"];
                    if (chkLRDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRDate"].Value = ConvertDateString(row["LRDate"]);
                    if (chkLRNo2.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRNumber2"].Value = row["LRNumber2"];
                    if (chkLRDate2.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRDate2"].Value = ConvertDateString(row["LRDate2"]);
                    if (chkAvailQty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["AvailQty"].Value = ConvertObjToStringN2(row["AvailQty"]);
                    if (chkNetAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["netamt"].Value = ConvertObjToStringN2(row["NetAmt"]);
                    
                    _rowIndex++;
                }
                lblTotQty.Text = dTotalQty.ToString("N2", MainPage.indianCurancy);

                if (chkTaxableAmt.Checked)
                {
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(BindedDT.Compute("SUM(STaxableAmt)", "")).ToString("N2", MainPage.indianCurancy);
                    LableTaxbleAmt.Visible = lblTaxableAmt.Visible = true;
                }
                else
                    LableTaxbleAmt.Visible = lblTaxableAmt.Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private string ConvertDateString(object strObj)
        {
            try
            {
                if (Convert.ToString(strObj) == "")
                    return "";
                else
                    return Convert.ToDateTime(Convert.ToString(strObj)).ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
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
                //if (_column.HeaderText == "S.No")
                //{
                //    _column.SortMode = DataGridViewColumnSortMode.Automatic;

                //}

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
            if (chkPurchaseParty.Checked)
                CreateGridviewColumn("PurchaseParty", "Purchase Party", "LEFT", 180);
            if (chkNDate.Checked)
                CreateGridviewColumn("Date", "Date", "LEFT", 100);
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
            if (chkAvailQty.Checked)
                CreateGridviewColumn("AvailQty", "Available Qty", "RIGHT", 120);
            if (chkTaxAmt.Checked)
                CreateGridviewColumn("taxamt", "Tax Amt", "RIGHT", 80);
            if (chkGrossAmt.Checked)
                CreateGridviewColumn("grossamt", "Gross Amt", "RIGHT", 100);
            if (chkWSDis.Checked)
                CreateGridviewColumn("WSDis", "WSDis", "LEFT", 80);
            if (chkWSMRP.Checked)
                CreateGridviewColumn("WSMRP", "WSMRP", "RIGHT", 80);

            if (chkBarCode.Checked)
                CreateGridviewColumn("barcode", "BarCode", "LEFT", 100);
            if (chkBrandName.Checked)
                CreateGridviewColumn("brandname", "Brand Name", "LEFT", 150);
            if (chkSaleMargin.Checked)
                CreateGridviewColumn("saleMargin", "Sale Margin", "RIGHT", 100);
            if (chkGodown.Checked)
                CreateGridviewColumn("Godown", "Godown", "LEFT", 150);
            if (chkSaleMRP.Checked)
                CreateGridviewColumn("saleMRP", "Sale MRP", "LEFT", 100);
            if (chkTransport.Checked)
                CreateGridviewColumn("transport", "Transport", "LEFT", 150);
            if (chkRemarks.Checked)
                CreateGridviewColumn("remarks", "Remark", "LEFT", 180);
            if (chkAgent.Checked)
                CreateGridviewColumn("Agent", "Agent", "LEFT", 150);
            if (chkTransMode.Checked)
                CreateGridviewColumn("TransportMode", "Transport Mode", "LEFT", 150);
            if (chkWeight.Checked)
                CreateGridviewColumn("PackWeight", "Weight", "RIGHT", 100);
            if (chkNoofPackage.Checked)
                CreateGridviewColumn("NoOfPacks", "No Of Packs", "RIGHT", 100);
            if (chkReceivedBy.Checked)
                CreateGridviewColumn("ReceivedBy", "Received By", "LEFT", 150);
            if (chkCountedBy.Checked)
                CreateGridviewColumn("CountedBy", "Counted By", "LEFT", 150);
            if (chkBarCodedBy.Checked)
                CreateGridviewColumn("BarCodedBy", "BarCoded By", "LEFT", 150);
            if (chkTaxFree.Checked)
                CreateGridviewColumn("TaxFree", "TaxFree", "LEFT", 100);
            if (chkSaleDis.Checked)
                CreateGridviewColumn("saleDis", "Sale Dis.", "LEFT", 100);
            if (chkSaleRate.Checked)
                CreateGridviewColumn("SaleRate", "SaleRate", "LEFT", 100);
            if (chkDesignName.Checked)
                CreateGridviewColumn("designname", "Style Name", "LEFT", 150);
            if (chkRate.Checked)
                CreateGridviewColumn("rate", "Rate", "RIGHT", 100);
            if (chkMRP.Checked)
                CreateGridviewColumn("mrp", "MRP", "RIGHT", 100);
            if (chkAmount.Checked)
                CreateGridviewColumn("amount", "Amount", "RIGHT", 100);
            if (chkDisPer.Checked)
                CreateGridviewColumn("disper", "DisPer", "RIGHT", 100);
            if (chkDisAmt.Checked)
                CreateGridviewColumn("disamt", "DisAmt", "RIGHT", 100);
            if (chkInvoiceNo.Checked)
                CreateGridviewColumn("InvoiceNo", "InvoiceNo", "RIGHT", 100);
            if (chkOtherAmt.Checked)
                CreateGridviewColumn("otheramt", "Other Amt", "RIGHT", 100);
            if (chkInvoiceDate.Checked)
                CreateGridviewColumn("InvoiceDate", "Invoice Date", "LEFT", 120);
            if (chkTaxableAmt.Checked)
                CreateGridviewColumn("STaxableAmt", "Taxable Amt", "RIGHT", 110);
            if (chkNetTaxable.Checked)
                CreateGridviewColumn("NetTaxableAmt", "Net Taxable Amt", "RIGHT", 130);
            if (chkTCSAmt.Checked)
                CreateGridviewColumn("TCSAmt", "TCS Amt", "RIGHT", 100);
            if (chkSpcDicPer.Checked)
                CreateGridviewColumn("spcdscper", "Spcl Disc%", "RIGHT", 100);
            if (chkSpcDisAmt.Checked)
                CreateGridviewColumn("spcdisamt", "Spcl DisAmt", "RIGHT", 120);
            if (chkSDisPer.Checked)
                CreateGridviewColumn("sdisper", "SDisPer", "RIGHT", 80);
            if (chkRoundOfAmt.Checked)
                CreateGridviewColumn("roundofamt", "RO Amt", "RIGHT", 70);
            if (chkPurchaseType.Checked)
                CreateGridviewColumn("purchasetype", "Purchase Type", "LEFT", 180);
            if (chkPackingAmt.Checked)
                CreateGridviewColumn("packingamt", "Packing Amt", "RIGHT", 120);
            if (chkUnitName.Checked)
                CreateGridviewColumn("unitname", "Unit Name", "LEFT", 90);
            if (chkCreatedBy.Checked)
                CreateGridviewColumn("createdby", "Created By", "LEFT", 100);
            if (chkUpdatedBy.Checked)
                CreateGridviewColumn("updatedby", "Updated By", "LEFT", 100);
            if (chkPurchaseStatus.Checked)
                CreateGridviewColumn("StockStatus", "P.Status", "LEFT", 100);
            if (chkDepartment.Checked)
                CreateGridviewColumn("department", "Department", "LEFT", 100);
            if (chkTransport2.Checked)
                CreateGridviewColumn("TransportName2", "Transport 2", "LEFT", 120);
            if (chkLRNo.Checked)
                CreateGridviewColumn("LRNumber", "LR No.", "LEFT", 90);
            if (chkLRDate.Checked)
                CreateGridviewColumn("LRDate", "LR Date", "LEFT", 90);
            if (chkLRNo2.Checked)
                CreateGridviewColumn("LRNumber2", "LR No. 2", "LEFT", 90);
            if (chkLRDate2.Checked)
                CreateGridviewColumn("LRDate2", "LR Date 2", "LEFT", 90);
            if (chkNetAmt.Checked)
                CreateGridviewColumn("netamt", "NetAmt", "RIGHT", 80);
        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strColumnOther = "", strColumnName = "", strGroupBy = "", strOrderBy = "", strOrderByText = " Order by ", strGroupByText = " Group by BillNo,NetAmt", strGroupBycase = " Group by BillNo,NetAmt", strGroupByOther = " Group by ";
            if (txtPurchaseParty.Text != "")
            {
                string[] strName = txtPurchaseParty.Text.Split(' ');
                strSubQuery += " and PurchasePartyId = '" + strName[0] + "' ";
            }
            if (txtTransportName.Text != "")
                strSubQuery += " and TransportName='" + txtTransportName.Text + "' ";

            if (txtAgent.Text != "")
                strSubQuery += " and Agent='" + txtAgent.Text + "' ";

            if (txtItemName.Text != "")
                strSubQuery += " and ItemName='" + txtItemName.Text + "' ";
            if (txtStyleName.Text != "")
                strSubQuery += " and DesignName LIKE '%" + txtStyleName.Text + "%' ";
            if (txtBarCode.Text != "")
                strSubQuery += " and BarCode like '%" + txtBarCode.Text + "%' ";

            if (txtBillCode.Text != "")
                strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

            if (txtDepartment.Text != "")
                strSubQuery += " and Department='" + txtDepartment.Text + "' ";

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

            if (txtGodown.Text != "")
                strSubQuery += " and GodownName='" + txtGodown.Text + "' ";
            if (txtInvoiceNo.Text != "")
                strSubQuery += " and InvoiceNo Like ('%" + txtInvoiceNo.Text + "%') ";

            if (txtStockStatus.Text != "")
                strSubQuery += " and ISNULL(StockStatus,'') in ('','" + txtStockStatus.Text + "') ";

            if (txtNetAmt.Text != "")
                strSubQuery += " and NetAmt='" + txtNetAmt.Text + "'";

            if (txtDepartment.Text != "")
                strSubQuery += " and Department='" + txtDepartment.Text + "'";

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
                strColumnName += "(cast(billCode as nvarchar)+' '+cast(billNo as nvarchar)) BillNo,";
                strColumnOther = "BillNo,";
                strGroupBy += ",(cast(billCode as nvarchar)+' '+cast(billNo as nvarchar))";
                strGroupByOther += "BillNo,";
                strOrderBy += ",BillNo";
            }
            if (chkNDate.Checked)
            {
                strColumnName += "Date,"; strColumnOther += "Date,"; strGroupBy += ",Date"; strGroupByOther += "Date,"; strOrderBy += ",Date";
            }
            if (chkPurchaseParty.Checked)
            {
                strColumnName += "ISNULL((PurchasePartyId+' '+Name),PurchasePartyId) PurchaseParty,"; strColumnOther += "PurchaseParty,"; strGroupBy += ",PurchasePartyId,Name"; strGroupByOther += "PurchaseParty,"; strOrderBy += ",PurchaseParty";
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
            if (chkTCSAmt.Checked)
            {
                strColumnName += "(TCSAmt) TCSAmt,"; strColumnOther += "SUM(TCSAmt) TCSAmt,"; strGroupBy += ",TCSAmt";// strGroupByOther += "GrossAmt,"; strOrderBy += ",GrossAmt";
            }
            if (chkGrossAmt.Checked)
            {
                strColumnName += "GrossAmt,"; strColumnOther += "SUM(GrossAmt) GrossAmt,"; strGroupBy += ",GrossAmt";
            }
            if (chkWSDis.Checked)
            {
                strColumnName += "sum(WSDis) WSDis,"; strColumnOther += "sum(WSDis) WSDis,"; //strGroupBy += ",CardAmt"; strGroupByOther += "CardAmt,"; strOrderBy += ",CardAmt";
            }
            if (chkWSMRP.Checked)
            {
                strColumnName += "sum(WSMRP) WSMRP,"; strColumnOther += "sum(WSMRP) WSMRP,";// strGroupBy += ",CashAmt"; strGroupByOther += "CashAmt,"; strOrderBy += ",CashAmt";
            }
            if (chkDepartment.Checked)
            {
                strColumnName += "Department,"; strColumnOther += "Department,"; strGroupBy += ",Department"; strGroupByOther += "Department,"; strOrderBy += ",Department";
            }
            if (chkBarCode.Checked)
            {
                strColumnName += "BarCode,"; strColumnOther += "BarCode,"; strGroupBy += ",BarCode"; strGroupByOther += "BarCode,"; strOrderBy += ",BarCode";
            }
            if (chkBrandName.Checked)
            {
                strColumnName += "BrandName,"; strColumnOther += "BrandName,"; strGroupBy += ",BrandName"; strGroupByOther += "BrandName,"; strOrderBy += ",BrandName";
            }
            if (chkSaleMargin.Checked)
            {
                strColumnName += "SaleMargin,"; strColumnOther += "SaleMargin,"; strGroupBy += ",SaleMargin"; strGroupByOther += "SaleMargin,"; strOrderBy += ",SaleMargin";
            }
            if (chkGodown.Checked)
            {
                strColumnName += "GodownName,"; strColumnOther += "GodownName ,"; strGroupBy += ",GodownName"; strGroupByOther += "GodownName,"; strOrderBy += ",GodownName";
            }
            if (chkSaleMRP.Checked)
            {
                strColumnName += "SaleMRP,"; strColumnOther += "SaleMRP,"; strGroupBy += ",SaleMRP"; strGroupByOther += "SaleMRP,"; strOrderBy += ",SaleMRP";
            }

            if (chkTransport.Checked)
            {
                strColumnName += "Transportname,"; strColumnOther += "Transportname,"; strGroupBy += ",Transportname"; strGroupByOther += "Transportname,"; strOrderBy += ",Transportname";
            }
            if (chkRemarks.Checked)
            {
                strColumnName += "Remark,"; strColumnOther += "Remark,"; strGroupBy += ",Remark"; strGroupByOther += "Remark,"; strOrderBy += ",Remark";
            }
            if (chkAgent.Checked)
            {
                strColumnName += "Agent,"; strColumnOther += "Agent,"; strGroupBy += ",Agent"; strGroupByOther += "Agent,"; strOrderBy += ",Agent";
            }
            if (chkTransMode.Checked)
            {
                strColumnName += "TransportMode,"; strColumnOther += "TransportMode,"; strGroupBy += ",TransportMode"; strGroupByOther += "TransportMode,"; strOrderBy += ",TransportMode";
            }
            if (chkNoofPackage.Checked)
            {
                strColumnName += "Sum(Cast(NoOfPacks as Money))NoOfPacks,"; strColumnOther += "Sum(Cast(NoOfPacks as Money))NoOfPacks,"; //strGroupBy += ",NoOfPacks"; strGroupByOther += "NoOfPacks,"; strOrderBy += ",NoOfPacks";
            }
            if (chkWeight.Checked)
            {
                strColumnName += "Sum(Cast(PackWeight as Money))PackWeight,"; strColumnOther += "Sum(Cast(PackWeight as Money))PackWeight,"; //strGroupBy += ",PackWeight"; strGroupByOther += "PackWeight,"; strOrderBy += ",PackWeight";
            }
            if (chkReceivedBy.Checked)
            {
                strColumnName += "ReceivedBy,"; strColumnOther += "ReceivedBy,"; strGroupBy += ",ReceivedBy"; strGroupByOther += "ReceivedBy,"; strOrderBy += ",ReceivedBy";
            }
            if (chkCountedBy.Checked)
            {
                strColumnName += "CountedBy,"; strColumnOther += "CountedBy,"; strGroupBy += ",CountedBy"; strGroupByOther += "CountedBy,"; strOrderBy += ",CountedBy";
            }
            if (chkBarCodedBy.Checked)
            {
                strColumnName += "BarCodedBy,"; strColumnOther += "BarCodedBy,"; strGroupBy += ",BarCodedBy"; strGroupByOther += "BarCodedBy,"; strOrderBy += ",BarCodedBy";
            }
            if (chkTaxFree.Checked)
            {
                strColumnName += "TaxFree,"; strColumnOther += "TaxFree,"; strGroupBy += ",TaxFree"; strGroupByOther += "TaxFree,"; strOrderBy += ",TaxFree";
            }
            if (chkSaleDis.Checked)
            {
                strColumnName += "SaleDis,"; strColumnOther += "SaleDis,"; strGroupBy += ",SaleDis"; strGroupByOther += "SaleDis,"; strOrderBy += ",SaleDis";
            }
            if (chkSaleRate.Checked)
            {
                strColumnName += "SaleRate,"; strColumnOther += "SaleRate,"; strGroupBy += ",SaleRate"; strGroupByOther += "SaleRate,"; strOrderBy += ",SaleRate";
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
                strColumnName += "sum(Amount) Amount,"; strColumnOther += "sum(Amount) Amount,"; strGroupBy += ",Amount"; //strGroupByOther += "Amount,"; strOrderBy += ",Amount";
            }
            if (chkDisPer.Checked)
            {
                strColumnName += "DiscPer,"; strColumnOther += "DiscPer,"; strGroupBy += ",DiscPer"; strGroupByOther += "DiscPer,"; strOrderBy += ",DiscPer";
            }
            if (chkDisAmt.Checked)
            {
                strColumnName += "sum(DiscAmt) DiscAmt,"; strColumnOther += "sum(DiscAmt) DiscAmt,"; strGroupBy += ",DiscAmt";// strGroupByOther += "DisAmt,"; strOrderBy += ",DisAmt";
            }
            if (chkInvoiceNo.Checked)
            {
                strColumnName += "InvoiceNo,"; strColumnOther += "InvoiceNo,"; strGroupBy += ",InvoiceNo"; strGroupByOther += "InvoiceNo,"; strOrderBy += ",InvoiceNo";
            }
            if (chkOtherAmt.Checked)
            {
                strColumnName += "othersign,sum(otherAmt) OtherAmt,"; strColumnOther += "(cast(othersign as nvarchar)+' '+cast(sum(otherAmt) as nvarchar)) OtherAmt,"; strGroupByOther += "othersign,OtherAmt,";
                strGroupBy += ",othersign,otherAmt"; strOrderBy += ",OtherAmt";
            }
            if (chkInvoiceDate.Checked)
            {
                strColumnName += "InvoiceDate,"; strColumnOther += "InvoiceDate,"; strGroupBy += ",InvoiceDate"; strGroupByOther += "InvoiceDate,"; strOrderBy += ",InvoiceDate";
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
                strColumnName += "sum(SpecialDscAmt) SpecialDscAmt,"; strColumnOther += "sum(SpecialDscAmt) SpecialDscAmt,"; //strGroupBy += ",SpecialDscAmt"; strGroupByOther += "SpecialDscAmt,"; strOrderBy += ",SpecialDscAmt";
            }

            if (chkSDisPer.Checked)
            {
                strColumnName += "SDisPer,"; strColumnOther += "SDisPer,"; strGroupBy += ",SDisPer"; strGroupByOther += "SDisPer,"; strOrderBy += ",SDisPer";
            }
            if (chkRoundOfAmt.Checked)
            {
                strColumnName += "ROSign,sum(RoundOff) Roundoff,"; strColumnOther += "(cast(ROSign as nvarchar)+' '+cast(sum(RoundOff) as nvarchar)) Roundoff,"; strGroupByOther += "ROSign,Roundoff,";
                strGroupBy += ",ROSign ,RoundOff"; strOrderBy += ",Roundoff";
            }
            if (chkPurchaseType.Checked)
            {
                strColumnName += "PurchaseType,"; strColumnOther += "PurchaseType,"; strGroupBy += ",PurchaseType"; strGroupByOther += "PurchaseType,"; strOrderBy += ",PurchaseType";
            }

            if (chkPackingAmt.Checked)
            {
                strColumnName += "sum(PackingAmt) PackingAmt,"; strColumnOther += "sum(PackingAmt) PackingAmt,"; //strGroupBy += ",PackingAmt"; strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            }

            if (chkUnitName.Checked)
            {
                strColumnName += "UnitName,"; strColumnOther += "UnitName,"; strGroupBy += " ,UnitName"; strGroupByOther += " UnitName,"; strOrderBy += ",UnitName";
            }
            if (chkPurchaseStatus.Checked)
            {
                strColumnName += "StockStatus,"; strColumnOther += "StockStatus,"; strGroupBy += " ,StockStatus"; strGroupByOther += " StockStatus,"; strOrderBy += ",StockStatus";
            }
            if (chkTransport2.Checked)
            {
                strColumnName += "TransportName2,"; strColumnOther += "isnull(TransportName2,'')TransportName2,"; strGroupBy += " ,TransportName2"; strGroupByOther += " TransportName2,"; strOrderBy += ",TransportName2";
            }
            if (chkLRNo.Checked)
            {
                strColumnName += "LRNumber,"; strColumnOther += "isnull(LRNumber,'')LRNumber,"; strGroupBy += " ,LRNumber"; strGroupByOther += " LRNumber,"; strOrderBy += ",LRNumber";
            }
            if (chkLRDate.Checked)
            {
                strColumnName += "LRDate,"; strColumnOther += "LRDate,"; strGroupBy += " ,LRDate"; strGroupByOther += " LRDate,"; strOrderBy += ",LRDate";
            }
            if (chkLRNo2.Checked)
            {
                strColumnName += "LRNumber2,"; strColumnOther += "isnull(LRNumber2,'')LRNumber2,"; strGroupBy += " ,LRNumber2"; strGroupByOther += " LRNumber2,"; strOrderBy += ",LRNumber2";
            }
            if (chkLRDate2.Checked)
            {
                strColumnName += "LRDate2,"; strColumnOther += "LRDate2,"; strGroupBy += " ,LRDate2"; strGroupByOther += " LRDate2,"; strOrderBy += ",LRDate2";
            }
            if (chkAvailQty.Checked)
            {
                strColumnName += "AvailQty,"; strColumnOther += "AvailQty,"; strGroupBy += " ,AvailQty"; strGroupByOther += " AvailQty,"; strOrderBy += ",AvailQty";
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
                strColumnName += "TaxAmt,"; strColumnOther += "Sum(TaxAmt)TaxAmt,"; strGroupBy += ",TaxAmt"; strOrderBy += ",TaxAmt";
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
                strQuery = "SELECT 1 as SNo SELECT 2 as SNo ";
            else
                strQuery = "select " + strColumnOther + " from (Select " + strColumnName + " from (Select *,(NetAmt-CAST((ROSign+(CAST(RoundOff as varchar))) as Money)-TaxAmt) NetTaxableAmt,CAST((CASE WHEN TaxIncluded=1 then((Amount* 100) / (100 + TaxRate)) else Amount end) as Numeric(18,2))STaxableAmt from (select PB.*,TaxIncluded,PBP.BarCode,PBP.BrandName,PBP.DesignName,PBP.ItemName,PBP.Variant1,PBP.Variant2,PBP.Variant3,PBP.Variant4,PBP.Variant5,PBP.Qty,ST.StockQty AvailQty,PBP.UnitName,PBP.MRP,PBP.SDisPer,PBP.Rate,PBP.Amount,PBP.PONumber,PBP.SaleMargin,PBP.SaleMRP,PBP.WSDis,PBP.WSMRP,PBP.SaleDis,PBP.SaleRate,_IM.Department"
                         + ",(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((PBP.MRP * 100)  / (100 + TaxRate)) else PBP.MRP end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + PBP.SDisPer - SpecialDscPer) / 100.00) else 1.00 end)))>  _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((PBP.MRP * 100) / (100 + TaxRate)) else PBP.MRP end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 + PBP.SDisPer - SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate  from Items _Im   inner join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName = PBP.ItemName)TaxRate"
                         + " from PurchaseBook PB inner join PurchaseBookSecondary PBP on PB.billcode=PBP.billcode and PB.billno=PBP.billno LEFT JOIN ItemStock ST ON ST.ItemName = PBP.ItemName AND ST.BarCode = PBP.BarCode AND ST.BrandName = PBP.BrandName AND ST.Variant1 = PBP.Variant1 AND ST.Variant2 = PBP.Variant2 left join SaleTypeMaster STM on STM.TaxName=PB.PurchaseType and STM.SaleType='PURCHASE'  OUTER APPLY (Select Top 1 ISNULL(_Im.MakeName,'') as Department,GroupName from Items _IM Where _IM.ItemName=PBP.ItemName)_IM )_Purchase)__Purchase left join (Select Name,(AreaCode+AccountNo) AccountID from SupplierMaster Where GroupName='SUNDRY CREDITOR')SM on AccountID=PurchasePartyID Where BillNo!=0 " + strSubQuery + strGroupByText + strGroupBy + strGroupBycase + ")Purchase " + strGroupByOther + strOrderByText + strOrderBy + " "
                         + " select SUM(NetAmt) NetAmt,SUM(GrossAmt) GrossAmt,SUM(TaxAmt)TaxAmt,SUM(NetAmt-CAST((ROSign+(CAST(RoundOff as varchar))) as Money)-TaxAmt) NetTaxableAmt from (select NetAmt,GrossAmt,TaxAmt,ROSign,RoundOff from (select PB.*,PBP.ItemName,PBP.DesignName,PBP.BarCode,PBP.BrandName,PBP.Variant1,PBP.Variant2,PBP.Variant3,PBP.Variant4,PBP.Variant5,_IM.Department from PurchaseBook PB inner join PurchaseBookSecondary PBP on Pb.billcode=PBP.billcode and PB.billno=PBP.billno  OUTER APPLY (Select Top 1 ISNULL(_Im.MakeName,'') as Department,GroupName from Items _IM Where _IM.ItemName=PBP.ItemName)_IM)_Purchase  Where BillNo!=0 " + strSubQuery + "Group by BillNo,NetAmt,GrossAmt,TaxAmt,ROSign,RoundOff)Purchase";

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
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PURCHASE PARTY", e.KeyCode);
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

        private void CustomSaleRegister_KeyDown(object sender, KeyEventArgs e)
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
                    SearchData objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
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
            //try
            //{
            //    char objChar = Convert.ToChar(e.KeyCode);
            //    int value = e.KeyValue;
            //    dba.ClearTextBoxOnKeyDown(sender, e);
            //    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            //    {
            //        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION ", e.KeyCode);
            //        objSearch.ShowDialog();
            //        txtGodown.Text = objSearch.strSelectedData;
            //        ClearAll();
            //    }
            //    e.Handled = true;
            //}
            //catch
            //{
            //}
        }



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
            catch  {  }
            btnSearch2.Enabled = true;
            pnlSearch.Visible = false;
            chkGroup2.Visible = false;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            txtPurchaseParty.Text = txtFromDate.Text = txtToDate.Text = txtItemName.Text = txtGodown.Text = txtBillCode.Text = txtTransportName.Text = txtNetAmt.Text = txtPFromSNo.Text = txtPToSNo.Text = txtDepartment.Text = "";
            pnlSearch.Visible = false;
        }

        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = chkGroup2.Visible = true;
        }

        private void txtNetAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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
                            dba.ShowTransactionBook("PURCHASE", strNumber[0], strNumber[1]);
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

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            int _rowIndex = 0;
            foreach (DataGridViewRow dr in dgrdDetails.Rows)
            {
                dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                _rowIndex++;
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
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


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

        private void CustomSaleRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
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
        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtStockStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASESTATUS", "SEARCH PURCHASE STATUS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtStockStatus.Text = objSearch.strSelectedData;
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

        private void txtGodown_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH GODOWN NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGodown.Text = objSearch.strSelectedData;
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

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateAllSpace(sender, e);
        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BARCODEDETAILS", "SEARCH BARCODE NO", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBarCode.Text = objSearch.strSelectedData;
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

        private void txtStyleName_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtAgent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("AGENTNAME", "SEARCH AGENT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtAgent.Text = objSearch.strSelectedData;
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
                    UpdateSetting("PURCHASE");
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
                    string clm = "", Value ="";
                    foreach (DataRow dr in dt.Rows)
                    {
                        clm = Convert.ToString(dr["ColumnName"]);
                        Value = Convert.ToString(dr["ShowHide"]);
                        switch (clm)
                        {
                            case "InvoiceDate":
                                chkInvoiceDate.Checked = true; break;
                            case "Colour":
                                chkColour.Checked = true; break;
                            case "BillNo":
                                chkBillNo.Checked = true; break;
                            case "Size":
                                chkSize.Checked = true; break;
                            case "Variant3":
                                chkVariant3.Checked = true; break;
                            case "Variant4":
                                chkVariant4.Checked = true; break;
                            case "Variant5":
                                chkVariant5.Checked = true; break;
                            case "NDate":
                                chkNDate.Checked = true; break;
                            case "PurchaseParty":
                                chkPurchaseParty.Checked = true; break;
                            case "Godown":
                                chkGodown.Checked = true; break;
                            case "ItemName":
                                chkItemName.Checked = true; break;
                            case "Qty":
                                chkQty.Checked = true; break;
                            case "TaxAmt":
                                chkTaxAmt.Checked = true; break;
                            case "GrossAmt":
                                chkGrossAmt.Checked = true; break;
                            case "InvoiceNo":
                                chkInvoiceNo.Checked = true; break;
                            case "TransMode":
                                chkTransMode.Checked = true; break;
                            case "Agent":
                                chkAgent.Checked = true; break;
                            case "BarCodedBy":
                                chkBarCodedBy.Checked = true; break;
                            case "CountedBy":
                                chkCountedBy.Checked = true; break;
                            case "ReceivedBy":
                                chkReceivedBy.Checked = true; break;
                            case "Weight":
                                chkWeight.Checked = true; break;
                            case "NoofPackage":
                                chkNoofPackage.Checked = true; break;
                            case "TCSAmt":
                                chkTCSAmt.Checked = true; break;
                            case "TaxableAmt":
                                chkTaxableAmt.Checked = true; break;
                            case "NetTaxable":
                                chkNetTaxable.Checked = true; break;
                            case "Department":
                                chkDepartment.Checked = true; break;
                            case "PurchaseStatus":
                                chkPurchaseStatus.Checked = true; break;
                            case "UpdatedBy":
                                chkUpdatedBy.Checked = true; break;
                            case "UnitName":
                                chkUnitName.Checked = true; break;
                            case "DisPer":
                                chkDisPer.Checked = true; break;
                            case "CreatedBy":
                                chkCreatedBy.Checked = true; break;
                            case "PurchaseType":
                                chkPurchaseType.Checked = true; break;
                            case "PackingAmt":
                                chkPackingAmt.Checked = true; break;
                            case "SDisPer":
                                chkSDisPer.Checked = true; break;
                            case "WSMRP":
                                chkWSMRP.Checked = true; break;
                            case "WSDis":
                                chkWSDis.Checked = true; break;
                            case "RoundOfAmt":
                                chkRoundOfAmt.Checked = true; break;
                            case "Amount":
                                chkAmount.Checked = true; break;
                            case "SaleMRP":
                                chkSaleMRP.Checked = true; break;
                            case "OtherAmt":
                                chkOtherAmt.Checked = true; break;
                            case "SpcDisAmt":
                                chkSpcDisAmt.Checked = true; break;
                            case "SpcDicPer":
                                chkSpcDicPer.Checked = true; break;
                            case "MRP":
                                chkMRP.Checked = true; break;
                            case "SaleMargin":
                                chkSaleMargin.Checked = true; break;
                            case "Rate":
                                chkRate.Checked = true; break;
                            case "DesignName":
                                chkDesignName.Checked = true; break;
                            case "BrandName":
                                chkBrandName.Checked = true; break;
                            case "Remarks":
                                chkRemarks.Checked = true; break;
                            case "BarCode":
                                chkBarCode.Checked = true; break;
                            case "SaleRate":
                                chkSaleRate.Checked = true; break;
                            case "SaleDis":
                                chkSaleDis.Checked = true; break;
                            case "DisAmt":
                                chkDisAmt.Checked = true; break;
                            case "Transport":
                                chkTransport.Checked = true; break;
                            case "TaxFree":
                                chkTaxFree.Checked = true; break;
                            case "Transport2":
                                chkTransport2.Checked = true; break;
                            case "LRNo":
                                chkLRNo.Checked = true; break;
                            case "LRDate":
                                chkLRDate.Checked = true; break;
                            case "LRNo2":
                                chkLRNo2.Checked = true; break;
                            case "LRDate2":
                                chkLRDate2.Checked = true; break;
                            case "AvailQty":
                                chkAvailQty.Checked = true; break;
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

        private void MinRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtCategory1_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory1);
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
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Report", ex.Message };
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
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Report", ex.Message };
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
                Reporting.ShowReport objReport = new Reporting.ShowReport("CUSTOM PURCHASE REPORT PREVIEW");
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

                    case "chkInvoiceDate":
                        RptDispClm = "InvoiceDate"; DTClmName = "InvoiceDate"; break;
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
                        } break;
                    case "chkVariant3":
                        if (chkVariant3.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory3; DTClmName = "Variant3";
                        } break;
                    case "chkVariant4":
                        if (chkVariant4.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory4; DTClmName = "Variant4";
                        } break;
                    case "chkVariant5":
                        if (chkVariant5.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory5; DTClmName = "Variant5";
                        } break;
                    case "chkNDate":
                        RptDispClm = "Date"; DTClmName = "Date"; break;
                    case "chkPurchaseParty":
                        RptDispClm = "PurchaseParty"; DTClmName = "PurchaseParty"; break;
                    case "chkGodown":
                        RptDispClm = "Godown"; DTClmName = "GodownName"; break;
                    case "chkItemName":
                        RptDispClm = "ItemName"; DTClmName = "ItemName"; break;
                    case "chkQty":
                        RptDispClm = "Qty"; DTClmName = "Qty"; break;
                    case "chkTaxAmt":
                        RptDispClm = "TaxAmt"; DTClmName = "TaxAmt"; break;
                    case "chkGrossAmt":
                        RptDispClm = "GrossAmt"; DTClmName = "GrossAmt"; break;
                    case "chkInvoiceNo":
                        RptDispClm = "InvoiceNo"; DTClmName = "Invoiceno"; break;
                    case "chkTransMode":
                        RptDispClm = "TransMode"; DTClmName = "TransportMode"; break;
                    case "chkAgent":
                        RptDispClm = "Agent"; DTClmName = "Agent"; break;
                    case "chkBarCodedBy":
                        RptDispClm = "BarCodedBy"; DTClmName = "BarCodedBy"; break;
                    case "chkCountedBy":
                        RptDispClm = "CountedBy"; DTClmName = "CountedBy"; break;
                    case "chkReceivedBy":
                        RptDispClm = "ReceivedBy"; DTClmName = "ReceivedBy"; break;
                    case "chkWeight":
                        RptDispClm = "Weight"; DTClmName = "PackWeight"; break;
                    case "chkNoofPackage":
                        RptDispClm = "NoofPackage"; DTClmName = "NoOfPacks"; break;
                    case "chkTCSAmt":
                        RptDispClm = "TCSAmt"; DTClmName = "TCSAmt"; break;
                    case "chkTaxableAmt":
                        RptDispClm = "TaxableAmt"; DTClmName = "STaxableAmt"; break;
                    case "chkNetTaxable":
                        RptDispClm = "Net Taxable"; DTClmName = "NetTaxableAmt"; break;
                    case "chkDepartment":
                        RptDispClm = "Department"; DTClmName = "Department"; break;
                    case "chkPurchaseStatus":
                        RptDispClm = "PurchaseStatus"; DTClmName = "StockStatus"; break;
                    case "chkUpdatedBy":
                        RptDispClm = "UpdatedBy"; DTClmName = "UpdatedBy"; break;
                    case "chkUnitName":
                        RptDispClm = "UnitName"; DTClmName = "UnitName"; break;
                    case "chkDisPer":
                        RptDispClm = "DisPer"; DTClmName = "DiscPer"; break;
                    case "chkCreatedBy":
                        RptDispClm = "CreatedBy"; DTClmName = "CreatedBy"; break;
                    case "chkPurchaseType":
                        RptDispClm = "PurchaseType"; DTClmName = "PurchaseType"; break;
                    case "chkPackingAmt":
                        RptDispClm = "PackingAmt"; DTClmName = "PackingAmt"; break;
                    case "chkSDisPer":
                        RptDispClm = "SDisPer"; DTClmName = "SDisPer"; break;
                    case "chkWSMRP":
                        RptDispClm = "WSMRP"; DTClmName = "WSMRP"; break;
                    case "chkWSDis":
                        RptDispClm = "WSDis"; DTClmName = "WSDis"; break;
                    case "chkRoundOfAmt":
                        RptDispClm = "RoundOfAmt"; DTClmName = "RoundOff"; break;
                    case "chkAmount":
                        RptDispClm = "Amount"; DTClmName = "Amount"; break;
                    case "chkSaleMRP":
                        RptDispClm = "SaleMRP"; DTClmName = "SaleMRP"; break;
                    case "chkOtherAmt":
                        RptDispClm = "OtherAmt"; DTClmName = "OtherAmt"; break;
                    case "chkSpcDisAmt":
                        RptDispClm = "SpcDisAmt"; DTClmName = "SpecialDscAmt"; break;
                    case "chkSpcDicPer":
                        RptDispClm = "SpcDicPer"; DTClmName = "SpecialDscPer"; break;
                    case "chkMRP":
                        RptDispClm = "MRP"; DTClmName = "MRP"; break;
                    case "chkSaleMargin":
                        RptDispClm = "SaleMargin"; DTClmName = "SaleMargin"; break;
                    case "chkRate":
                        RptDispClm = "Rate"; DTClmName = "Rate"; break;
                    case "chkDesignName":
                        RptDispClm = "DesignName"; DTClmName = "DesignName"; break;
                    case "chkBrandName":
                        RptDispClm = "BrandName"; DTClmName = "BrandName"; break;
                    case "chkRemarks":
                        RptDispClm = "Remarks"; DTClmName = "Remarks"; break;
                    case "chkBarCode":
                        RptDispClm = "BarCode"; DTClmName = "BarCode"; break;
                    case "chkSaleRate":
                        RptDispClm = "SaleRate"; DTClmName = "SaleRate"; break;
                    case "chkSaleDis":
                        RptDispClm = "SaleDis"; DTClmName = "SaleDis"; break;
                    case "chkDisAmt":
                        RptDispClm = "DisAmt"; DTClmName = "DisAmt"; break;
                    case "chkTransport":
                        RptDispClm = "Transport"; DTClmName = "Transport"; break;
                    case "chkTaxFree":
                        RptDispClm = "TaxFree"; DTClmName = "TaxFree"; break;
                    case "chkTransport2":
                        RptDispClm = "Transport2"; DTClmName = "Transport2"; break;
                    case "chkLRNo":
                        RptDispClm = "LRNo"; DTClmName = "LRNo"; break;
                    case "chkLRDate":
                        RptDispClm = "LRDate"; DTClmName = "LRDate"; break;
                    case "chkLRNo2":
                        RptDispClm = "LRNo2"; DTClmName = "LRNo2"; break;
                    case "chkLRDate2":
                        RptDispClm = "LRDate2"; DTClmName = "LRDate2"; break;
                    case "chkAvailQty":
                        RptDispClm = "AvailQty"; DTClmName = "AvailQty"; break;
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
                        _row["TTaxable"] = "Taxable Amt : " + lblTaxableAmt.Text;
                    if (chkTaxAmt.Checked)
                        _row["TTax"] = "Tax Amt : " + lblTaxAmt.Text;

                    _row["TNet"] = "Net Amt : " + lblNetAmt.Text;

                    if (txtPurchaseParty.Text != "")
                        _row["CustomerName"] = "Customer : " + txtPurchaseParty.Text;

                    _row["HeaderName"] = "Custom Purchase Report";

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

        private void chkBillNo_CheckedChanged(object sender, EventArgs e)
        {
            SetColumnsIndex(sender);
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
        private void btnPrev_Click(object sender, EventArgs e)
        {
            BindPrevInGV();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            BindNextInGV();
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
                    currentPageNum = (int)dba.ConvertObjectToDouble(lblCurrentPage.Text)-1;
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

        private void lblCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtRowsPerPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void SetColumnsIndex(object sender)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                arrPrint.Add(chk);
            else
                arrPrint.Remove(chk);
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            chkGroup2.Visible = !chkGroup2.Visible;
        }
    }
}
