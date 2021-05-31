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
    public partial class CustomSaleRegister : Form
    {
        DataBaseAccess dba;
        DataTable BindedDT = new DataTable();
        int currentPageNum = 0, pageSize = 1, maxPageNum = 0;
        protected internal bool _bSearchStatus = false;
        List<CheckBox> arrPrint = new List<CheckBox>();
        public CustomSaleRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            SetCategory();
            if (txtRowsPerPage.Text == "")
                txtRowsPerPage.Text = "1";
            GetChkSetting("SALES");
        }
        public CustomSaleRegister(string CustomerName, string FromDate, string ToDate)
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            SetCategory();
            if (txtRowsPerPage.Text == "")
                txtRowsPerPage.Text = "1";
            GetChkSetting("SALES");
            SetPreviewData(CustomerName, FromDate, ToDate);
            SearchRecord();
        }

        private void SetPreviewData(string CustomerName, string FromDate, string ToDate)
        {
            if (!MainPage.mymainObject.bShowAllRecord && CustomerName != "")
                txtSalesParty.Text = CustomerName;

            chkDate.Checked = true;
            txtFromDate.Text = FromDate;
            txtToDate.Text = ToDate;
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
                        SetColounCategory();
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
                    lblSTaxableAmt.Text = dba.ConvertObjectToDouble(BindedDT.Compute("SUM(STaxableAmt)", "")).ToString("N2", MainPage.indianCurancy);
                    LabelSTaxbleAmt.Visible = lblSTaxableAmt.Visible = true;
                }
                else
                    LabelSTaxbleAmt.Visible = lblSTaxableAmt.Visible = false;
            }
            catch(Exception ex) { }
            btnNext.Visible = maxPageNum > 0;
            btnPrev.Visible = currentPageNum > 0;
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
            LabelQty.Visible = lblTotQty.Visible = chkQty.Checked;
            LabelGrossAmt.Visible = lblGrossAmt.Visible = chkGrossAmt.Checked;
            LabelNetTaxable.Visible = lblNetTaxable.Visible = chkNetTaxable.Checked;
            LabelTaxAmt.Visible = lblTaxAmt.Visible = chkTaxAmt.Checked;
            LabelNetAmt.Visible = lblNetAmt.Visible = chkNetAmt.Checked;

            double dNetAmt = 0, dGrossAmt = 0, dQty = 0, dTaxAmt = 0, dTaxableAmt = 0;
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                dNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                if (chkGrossAmt.Checked)
                    dGrossAmt = dba.ConvertObjectToDouble(row["GrossAmt"]);
                dTaxAmt = dba.ConvertObjectToDouble(row["TaxAmt"]);
                dTaxableAmt = dba.ConvertObjectToDouble(row["NetTaxableAmt"]);

                //if (chkQty.Checked)
                //    dQty += dba.ConvertObjectToDouble( row["TotalQty"]);


                lblGrossAmt.Text = dGrossAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                lblNetTaxable.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
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
        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    lblVar1Var2.Text = chkSize.Text = MainPage.StrCategory1;
                    txtCategory1.Enabled = true;
                    txtCategory1.Width = 170;
                }
                else
                {
                    txtCategory1.Enabled = chkSize.Enabled = false;
                    txtCategory1.Width = 67;
                }

                if (MainPage.StrCategory2 != "")
                {
                    chkColour.Text = MainPage.StrCategory2;
                    lblVar1Var2.Text += "/" + MainPage.StrCategory2;
                    txtCategory2.Enabled = true;
                    txtCategory1.Width = 67;
                }
                else
                {
                    txtCategory2.Enabled = chkColour.Enabled = false;
                    txtCategory1.Width = 170;
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
                   // chkVariant5.Text = MainPage.StrCategory5;
                    lblCategory5.Text = MainPage.StrCategory5 + " :";
                    txtCategory5.Enabled = true;
                }
                else
                {
                    lblCategory5.Enabled = txtCategory5.Enabled = false;
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
                if (chkSize.Checked && chkSize.Enabled)
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

                if (chkColour.Checked && chkColour.Enabled)
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

        private void BindDataWithGrid(DataTable _dt)
        {
            double dNetAmt = 0, dAmt = 0, dGrossAmt = 0, dQty = 0;
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
                    if (chkSalesParty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salesParty"].Value = row["SalePartyID"];
                    if (chkNDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = Convert.ToDateTime(Convert.ToString(row["Date"])).ToString("dd/MM/yyyy");
                    if (chkHSN.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["HSN"].Value = row["HSN"];
                    if (chkItemName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["itemname"].Value = row["ItemName"];
                    if (chkSize.Checked && chkSize.Enabled)
                        dgrdDetails.Rows[_rowIndex].Cells["variant1"].Value = row["variant1"];
                    if (chkColour.Checked && chkColour.Enabled)
                        dgrdDetails.Rows[_rowIndex].Cells["variant2"].Value = row["variant2"];
                    if (chkVariant3.Checked && chkVariant3.Enabled)
                        dgrdDetails.Rows[_rowIndex].Cells["variant3"].Value = row["variant3"];
                    if (chkVariant4.Checked && chkVariant4.Enabled)
                        dgrdDetails.Rows[_rowIndex].Cells["variant4"].Value = row["variant4"];
                    if (chkQty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value =ConvertObjToStringN2(row["Qty"]);
                    if (chkTaxAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["taxamt"].Value =ConvertObjToStringN2(row["TaxAmt"]);
                    if (chkGrossAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["grossamt"].Value =ConvertObjToStringN2(row["GrossAmt"]);
                    if (chkCardAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["cardamt"].Value =ConvertObjToStringN2(row["CardAmt"]);
                    if (chkCashAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["cashamt"].Value =ConvertObjToStringN2(row["CashAmt"]);
                    if (chkChqAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["ChequeAmt"].Value =ConvertObjToStringN2(row["ChequeAmt"]);
                    if (chkTaxableAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["STaxableAmt"].Value =ConvertObjToStringN2(row["STaxableAmt"]);
                    if (chkNetTaxable.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["NetTaxableAmt"].Value =ConvertObjToStringN2(row["NetTaxableAmt"]);
                    if (chkBarCode.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["barcode"].Value = row["BarCode"];
                    if (chkBarCode_S.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];
                    if (chkBrandName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["brandname"].Value = row["BrandName"];
                    if (chkDepartment.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Department"].Value = row["Department"];
                    if (chkSaleIncentives.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleincentive"].Value =ConvertObjToStringN2(row["SalesIncentive"]);
                    if (chkSalesMan.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salesman"].Value = row["SalesMan"];
                    if (chkSubParty.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["subparty"].Value = row["SubPartyID"];
                    if (chkStation.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["station"].Value = row["Station"];
                    if (chkTransport.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Transport"].Value = row["TransportName"];
                    if (chkRemarks.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["remarks"].Value = row["Remark"];
                    //if (chkCartoonType.Checked)
                    //    dgrdDetails.Rows[_rowIndex].Cells["cartontype"].Value = row["CartonType"];
                    if (chkLRNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRNo"].Value = row["LRNumber"];
                    if (chkLRDate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["LRDate"].Value = Convert.ToString(row["LRDate"]);
                    if (chkDesignName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["designname"].Value = row["DesignName"];
                    if (chkRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["rate"].Value =ConvertObjToStringN2(row["Rate"]);
                    if (chkMRP.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["mrp"].Value =ConvertObjToStringN2(row["MRP"]);
                    if (chkAmount.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["amount"].Value =ConvertObjToStringN2(row["Amount"]);
                    if (chkDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disper"].Value =ConvertObjToStringN2(row["DisPer"]);
                    if (chkDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["disamt"].Value =ConvertObjToStringN2(row["DisAmt"]);
                    if (chkPostageAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["postageamt"].Value =ConvertObjToStringN2(row["PostageAmt"]);
                    if (chkOtherAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Otheramt"].Value =ConvertObjToStringN2(row["OtherAmt"]);
                    if (chkCreditAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["creditamt"].Value =ConvertObjToStringN2(row["CreditAmt"]);
                    if (chkMob.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["mob"].Value = row["MobileNo"];
                    if (chkReturnSlipNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["returnslipno"].Value = row["ReturnSlipNo"];
                    if (chkReturnAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["returnamt"].Value =ConvertObjToStringN2(row["ReturnAmt"]);
                    if (chkAdvSlipNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["advslipno"].Value = row["AdvanceSlipNo"];
                    if (chkAdvAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["advamt"].Value =ConvertObjToStringN2(row["AdvanceAmt"]);
                    if (chkCreatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["createdby"].Value = row["CreatedBy"];
                    if (chkUpdatedBy.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["updatedby"].Value = row["UpdatedBy"];
                    if (chkSpcDicPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdscper"].Value =ConvertObjToStringN2(row["SpecialDscPer"]);
                    if (chkSpcDisAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["spcdisamt"].Value =ConvertObjToStringN2(row["SpecialDscAmt"]);
                    if (chkFinalAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["finalamt"].Value =ConvertObjToStringN2(row["FinalAmt"]);
                    if (chkSDisPer.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["sdisper"].Value =ConvertObjToStringN2(row["SDisPer"]);
                    if (chkRoundOfAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["roundofamt"].Value = row["RoundOffAmt"];
                    if (chkSalesType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["salestype"].Value = row["SalesType"];
                    if (chkPackingAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["packingamt"].Value =ConvertObjToStringN2(row["PackingAmt"]);
                    if (chkWayBillNo.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["waybillno"].Value = row["WayBillNo"];
                    if (chkUnitName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["unitname"].Value = row["UnitName"];
                    if (chkSaleBillType.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["saleBillType"].Value = row["SaleBillType"];
                    if (chkDepartment.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["department"].Value = row["department"];
                    if (chkNetAmt.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["netamt"].Value =ConvertObjToStringN2(row["NetAmt"]);
                    _rowIndex++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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
                CreateGridviewColumn("salesParty", "Sundry Debtors", "LEFT", 180);
            if (chkBrandName.Checked)
                CreateGridviewColumn("brandname", "Brand Name", "LEFT", 150);
            if (chkItemName.Checked)
                CreateGridviewColumn("ItemName", "Item Name", "LEFT", 180);
            if (chkSize.Checked && chkSize.Enabled)
                CreateGridviewColumn("variant1", MainPage.StrCategory1, "LEFT", 50);
            if (chkColour.Checked && chkColour.Enabled)
                CreateGridviewColumn("variant2", MainPage.StrCategory2, "LEFT", 50);
            if (chkVariant3.Checked && chkVariant3.Enabled)
                CreateGridviewColumn("variant3", MainPage.StrCategory3, "LEFT", 50);
            if (chkVariant4.Checked && chkVariant4.Enabled)
                CreateGridviewColumn("variant4", MainPage.StrCategory4, "LEFT", 50);
            if (chkQty.Checked)
                CreateGridviewColumn("qty", "Qty", "RIGHT", 80);
            if (chkRate.Checked)
                CreateGridviewColumn("rate", "Rate", "RIGHT", 100);
            if (chkAmount.Checked)
                CreateGridviewColumn("amount", "Amount", "RIGHT", 100);
            if (chkSalesMan.Checked)
                CreateGridviewColumn("salesman", "Sales Man", "LEFT", 120);
            if (chkSaleIncentives.Checked)
                CreateGridviewColumn("saleincentive", "Sale Inc.", "RIGHT", 90);
            if (chkGrossAmt.Checked)
                CreateGridviewColumn("grossamt", "Gross Amt", "RIGHT", 100);
            if (chkBarCode.Checked)
                CreateGridviewColumn("barcode", "Bar Code", "LEFT", 120);
            if (chkBarCode_S.Checked)
                CreateGridviewColumn("barcode_s", "Bar Code II", "LEFT", 120);
            if (chkCreditAmt.Checked)
                CreateGridviewColumn("creditamt", "Credit Amt", "RIGHT", 100);
            if (chkCardAmt.Checked)
                CreateGridviewColumn("cardamt", "Card Amt", "RIGHT", 80);
            if (chkCashAmt.Checked)
                CreateGridviewColumn("cashamt", "Cash Amt", "RIGHT", 80);
            if (chkChqAmt.Checked)
                CreateGridviewColumn("ChequeAmt", "Chq Amt", "RIGHT", 80);
            if (chkTransport.Checked)
                CreateGridviewColumn("transport", "Transport", "LEFT", 150);
            if (chkStation.Checked)
                CreateGridviewColumn("station", "Station", "LEFT", 150);
            if (chkLRNo.Checked)
                CreateGridviewColumn("LRNo", "LRNo", "LEFT", 100);
            if (chkLRDate.Checked)
                CreateGridviewColumn("LRDate", "LRDate", "LEFT", 100);
            if (chkSpcDicPer.Checked)
                CreateGridviewColumn("spcdscper", "Spcl Disc%", "RIGHT", 100);
            if (chkWayBillNo.Checked)
                CreateGridviewColumn("waybillno", "Way Bill No", "LEFT", 130);
            if (chkDesignName.Checked)
                CreateGridviewColumn("designname", "Design Name", "LEFT", 150);
            if (chkDepartment.Checked)
                CreateGridviewColumn("department", "Department", "LEFT", 90);
            if (chkMRP.Checked)
                CreateGridviewColumn("mrp", "MRP", "RIGHT", 100);
            if (chkTaxAmt.Checked)
                CreateGridviewColumn("taxamt", "Tax Amt", "RIGHT", 80);
            if (chkTaxableAmt.Checked)
                CreateGridviewColumn("STaxableAmt", "Taxable Amt", "RIGHT", 110);
            if (chkNetTaxable.Checked)
                CreateGridviewColumn("NetTaxableAmt", "Net Taxable Amt", "RIGHT", 130);
            if (chkDisPer.Checked)
                CreateGridviewColumn("disper", "Dis(%)", "RIGHT", 100);
            if (chkPostageAmt.Checked)
                CreateGridviewColumn("Postageamt", "Postage", "RIGHT", 75);
            if (chkOtherAmt.Checked)
                CreateGridviewColumn("otheramt", "Other Amt", "RIGHT", 100);
            if (chkSubParty.Checked)
                CreateGridviewColumn("subParty", "Sub Party", "LEFT", 150);
            if (chkMob.Checked)
                CreateGridviewColumn("mob", "Mobile No", "LEFT", 100);
            if (chkReturnSlipNo.Checked)
                CreateGridviewColumn("returnslipno", "Return SlipNo", "LEFT", 100);
            if (chkReturnAmt.Checked)
                CreateGridviewColumn("returnamt", "Return Amt", "RIGHT", 100);
            if (chkAdvSlipNo.Checked)
                CreateGridviewColumn("advslipno", "Advance SlipNo", "LEFT", 120);
            if (chkAdvAmt.Checked)
                CreateGridviewColumn("advamt", "Advance Amt", "RIGHT", 120);
            if (chkSDisPer.Checked)
                CreateGridviewColumn("sdisper", "S.Dis(%)", "RIGHT", 80);
            if (chkDisAmt.Checked)
                CreateGridviewColumn("disamt", "Dis Amt", "RIGHT", 100);
            if (chkRoundOfAmt.Checked)
                CreateGridviewColumn("roundofamt", "RO Amt", "RIGHT", 70);
            if (chkPackingAmt.Checked)
                CreateGridviewColumn("packingamt", "Packing", "RIGHT", 90);
            if (chkSalesType.Checked)
                CreateGridviewColumn("salestype", "Sales Type", "LEFT", 180);
            if (chkUnitName.Checked)
                CreateGridviewColumn("unitname", "Unit Name", "LEFT", 90);
            if (chkCreatedBy.Checked)
                CreateGridviewColumn("createdby", "Created By", "LEFT", 100);
            if (chkUpdatedBy.Checked)
                CreateGridviewColumn("updatedby", "Updated By", "LEFT", 100);
            if (chkRemarks.Checked)
                CreateGridviewColumn("remarks", "Remarks", "LEFT", 180);
            if (chkSaleBillType.Checked)
                CreateGridviewColumn("saleBillType", "Sale Bill Type", "LEFT", 130);
            if (chkHSN.Checked)
                CreateGridviewColumn("HSN", "HSN Code", "LEFT", 140);
            if (chkFinalAmt.Checked)
                CreateGridviewColumn("finalamt", "Final Amt", "RIGHT", 100);
            if (chkSpcDisAmt.Checked)
                CreateGridviewColumn("spcdisamt", "Spcl DisAmt", "RIGHT", 120);
            //if (chkPackerName.Checked)
            //    CreateGridviewColumn("packername", "Packer Name", "LEFT", 110);
            if (chkNetAmt.Checked)
                CreateGridviewColumn("netamt", "Net Amt", "RIGHT", 80);
        }

        private string CreateQuery()
        {
            string strQuery = "", strSubQuery = "", strColumnOther = "", strColumnName = "", strGroupBy = "", strOrderBy = "", strOrderByText = " Order by ", strGroupByText = " Group by BillNo,NetAmt", strGroupBycase = " Group by BillNo,NetAmt", strGroupByOther = " Group by ", strDepartmentQuery = "", strDepartName = "";
            if (chkDepartment.Checked || txtGroupName.Text != "" || txtDepartment.Text != "")
            {
                strDepartmentQuery = " OUTER APPLY (Select Top 1 ISNULL(_Im.MakeName,'') as Department,GroupName from Items _IM Where _IM.ItemName=SBS.ItemName)_IM ";
                strDepartName = ",_Im.Department,_Im.GroupName";
            }
            if (txtSalesParty.Text != "")
                strSubQuery += " and ISNULL((SalePartyId+' '+Name),SalePartyId) = '" + txtSalesParty.Text + "' ";

            if (txtTransportName.Text != "")
                strSubQuery += " and TransportName='" + txtTransportName.Text + "' ";

            if (txtItemName.Text != "")
                strSubQuery += " and ItemName='" + txtItemName.Text + "' ";

            if (txtBillCode.Text != "")
                strSubQuery += " and BillCode='" + txtBillCode.Text + "' ";

            if (txtStation.Text != "")
                strSubQuery += " and Station='" + txtStation.Text + "' ";

            if (txtBrand.Text != "")
                strSubQuery += " and BrandName='" + txtBrand.Text + "' ";

            if (txtNetAmt.Text != "")
                strSubQuery += " and NetAmt='" + txtNetAmt.Text + "'";

            if (txtDepartment.Text != "")
                strSubQuery += " and Department='" + txtDepartment.Text + "' ";

            if (MinRate.Text != "")
                strSubQuery += " and Rate>=" + MinRate.Text + " ";

            if (MaxRate.Text != "")
                strSubQuery += " and Rate<=" + MaxRate.Text + " ";

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

            if (txtGroupName.Text != "")
                strSubQuery += " and GroupName='" + txtGroupName.Text + "' ";
            if (txtLocation.Text != "")
                strSubQuery += " and MaterialLocation='" + txtLocation.Text + "' ";
            if (txtRemark.Text != "")
                strSubQuery += " and Remark LIKE('%" + txtRemark.Text + "%') ";
            if (txtSalesMan.Text != "")
                strSubQuery += " and [SalesMan] LIKE('" + txtSalesMan.Text + "') ";
            if (txtMobileNo.Text != "")
                strSubQuery += " and MobileNo LIKE('%" + txtMobileNo.Text + "%') ";
            if (txtBarCode.Text != "")
                strSubQuery += " and BarCode LIKE('%" + txtBarCode.Text + "%') ";
            if (txtBarCode_S.Text != "")
                strSubQuery += " and BarCode_S LIKE('%" + txtBarCode_S.Text + "%') ";

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
            if (chkHSN.Checked)
            {
                strColumnName += "HSN,"; strColumnOther += "HSN,"; strGroupBy += ",HSN"; strGroupByOther += "HSN,"; strOrderBy += ",HSN";
            }
            if (chkSize.Checked && chkSize.Enabled)
            {
                strColumnName += "Variant1,"; strColumnOther += "Variant1,"; strGroupBy += ",Variant1"; strGroupByOther += "Variant1,"; strOrderBy += ",Variant1";
            }
            if (chkColour.Checked && chkColour.Enabled)
            {
                strColumnName += "Variant2,"; strColumnOther += "Variant2,"; strGroupBy += ",Variant2"; strGroupByOther += "Variant2,"; strOrderBy += ",Variant2";
            }
            if (chkVariant3.Checked && chkVariant3.Enabled)
            {
                strColumnName += "Variant3,"; strColumnOther += "Variant3,"; strGroupBy += ",Variant3"; strGroupByOther += "Variant3,"; strOrderBy += ",Variant3";
            }
            if (chkVariant4.Checked && chkVariant4.Enabled)
            {
                strColumnName += "Variant4,"; strColumnOther += "Variant4,"; strGroupBy += ",Variant4"; strGroupByOther += "Variant4,"; strOrderBy += ",Variant4";
            }
            if (chkQty.Checked)
            {
                strColumnName += "sum(Qty) Qty,"; strColumnOther += "sum(Qty) Qty,";// strGroupByOther += "Qty,"; strOrderBy += ",Qty";
            }
            if (chkGrossAmt.Checked)
            {
                strColumnName += "(GrossAmt) GrossAmt,"; strColumnOther += "SUM(GrossAmt) GrossAmt,"; strGroupBy += ",GrossAmt";// strGroupByOther += "GrossAmt,"; strOrderBy += ",GrossAmt";
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
            if (chkCardAmt.Checked)
            {
                strColumnName += "(CardAmt) CardAmt,"; strColumnOther += "sum(CardAmt) CardAmt,"; strGroupBy += ",CardAmt"; //strGroupByOther += "CardAmt,"; strOrderBy += ",CardAmt";
            }
            if (chkCashAmt.Checked)
            {
                strColumnName += "(CashAmt) CashAmt,"; strColumnOther += "sum(CashAmt) CashAmt,"; strGroupBy += ",CashAmt"; //strGroupByOther += "CashAmt,"; strOrderBy += ",CashAmt";
            }
            if (chkBarCode.Checked)
            {
                strColumnName += "BarCode,"; strColumnOther += "BarCode,"; strGroupBy += ",BarCode"; strGroupByOther += "BarCode,"; strOrderBy += ",BarCode";
            }
            if (chkBarCode_S.Checked)
            {
                strColumnName += "BarCode_S,"; strColumnOther += "ISNULL(BarCode_S,'')BarCode_S,"; strGroupBy += ",BarCode_S"; strGroupByOther += "BarCode_S,"; strOrderBy += ",BarCode_S";
            }
            if (chkBrandName.Checked)
            {
                strColumnName += "BrandName,"; strColumnOther += "BrandName,"; strGroupBy += ",BrandName"; strGroupByOther += "BrandName,"; strOrderBy += ",BrandName";
            }
            if (chkDepartment.Checked)
            {
                strColumnName += "Department,"; strColumnOther += "Department,"; strGroupBy += ",Department"; strGroupByOther += "Department,"; strOrderBy += ",Department";
            }
            //if (chkSaleIncentives.Checked)
            //{
            //    strColumnName += "Other2 as SalesIncentive,"; strColumnOther += "SalesIncentive,"; strGroupBy += ",Other2"; strGroupByOther += "SalesIncentive,"; strOrderBy += ",SalesIncentive";
            //}
            if (chkSaleIncentives.Checked)
            {
                strColumnName += @"SUM((CASE WHEN SaleBillType='RETAIL' and (SaleIncentive LIKE '%\%%' ESCAPE '\') then ((Amount*CAST(Replace(SaleIncentive,'%','') as Money))/100) WHEN SaleBillType='RETAIL' then (Qty*CAST(SaleIncentive as Money)) else 0 end)) SalesIncentive,"; strColumnOther += " CAST(SUM(SalesIncentive) as numeric(18,2)) SalesIncentive,";
            }

            if (chkSalesMan.Checked)
            {
                strColumnName += "SalesMan ,"; strColumnOther += "SalesMan ,"; strGroupBy += ",SalesMan"; strGroupByOther += "SalesMan,"; strOrderBy += ",SalesMan";
            }
            if (chkSubParty.Checked)
            {
                strColumnName += "SubPartyID,"; strColumnOther += "SubPartyID,"; strGroupBy += ",SubPartyID"; strGroupByOther += "SubPartyID,"; strOrderBy += ",SubPartyID";
            }
            if (chkStation.Checked)
            {
                strColumnName += "Station,"; strColumnOther += "Station,"; strGroupBy += ",Station"; strGroupByOther += "Station,"; strOrderBy += ",Station";
            }
            if (chkTransport.Checked)
            {
                strColumnName += "Transportname,"; strColumnOther += "Transportname,"; strGroupBy += ",Transportname"; strGroupByOther += "Transportname,"; strOrderBy += ",Transportname";
            }
            if (chkRemarks.Checked)
            {
                strColumnName += "Remark,"; strColumnOther += "Remark,"; strGroupBy += ",Remark"; strGroupByOther += "Remark,"; strOrderBy += ",Remark";
            }
            if (chkLRNo.Checked)
            {
                strColumnName += "LRNumber,"; strColumnOther += "LRNumber,"; strGroupBy += ",LRNumber"; strGroupByOther += "LRNumber,"; strOrderBy += ",LRNumber";
            }
            if (chkLRDate.Checked)
            {
                strColumnName += "LRDate,"; strColumnOther += "LRDate,"; strGroupBy += ",LRDate"; strGroupByOther += "LRDate,"; strOrderBy += ",LRDate";
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
            if (chkDisPer.Checked)
            {
                strColumnName += "DisPer,"; strColumnOther += "DisPer,"; strGroupBy += ",DisPer"; strGroupByOther += "DisPer,"; strOrderBy += ",DisPer";
            }
            if (chkDisAmt.Checked)
            {
                strColumnName += "(DisAmt) DisAmt,"; strColumnOther += "sum(DisAmt) DisAmt,"; strGroupBy += ",DisAmt";// strGroupByOther += "DisAmt,"; strOrderBy += ",DisAmt";
            }
            if (chkPostageAmt.Checked)
            {
                strColumnName += "(PostageAmt) PostageAmt,"; strColumnOther += "sum(PostageAmt) PostageAmt,"; strGroupBy += ",PostageAmt"; // strGroupByOther += "PostageAmt,"; strOrderBy += ",PostageAmt";
            }
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
            if (chkReturnSlipNo.Checked)
            {
                strColumnName += "ReturnSlipNo,"; strColumnOther += "ReturnSlipNo,"; strGroupBy += ",ReturnSlipNo"; strGroupByOther += "ReturnSlipNo,"; strOrderBy += ",ReturnSlipNo";
            }
            if (chkReturnAmt.Checked)
            {
                strColumnName += "(ReturnAmt) ReturnAmt,"; strColumnOther += "sum(ReturnAmt) ReturnAmt,"; strGroupBy += ",ReturnAmt";// strGroupByOther += "ReturnAmt,"; strOrderBy += ",ReturnAmt";
            }
            if (chkAdvSlipNo.Checked)
            {
                strColumnName += "AdvanceSlipNo,"; strColumnOther += "AdvanceSlipNo,"; strGroupBy += ",AdvanceSlipNo"; strGroupByOther += "AdvanceSlipNo,"; strOrderBy += ",AdvanceSlipNo";
            }
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

            if (chkPackingAmt.Checked)
            {
                strColumnName += "(PackingAmt) PackingAmt,"; strColumnOther += "sum(PackingAmt) PackingAmt,"; strGroupBy += ",PackingAmt";// strGroupByOther += "PackingAmt,"; strOrderBy += ",PackingAmt";
            }

            if (chkWayBillNo.Checked)
            {
                strColumnName += "WaybillNo,"; strColumnOther += "WaybillNo,"; strGroupBy += " ,WaybillNo"; strGroupByOther += " WaybillNo,"; strOrderBy += ",WaybillNo";
            }

            if (chkUnitName.Checked)
            {
                strColumnName += "UnitName,"; strColumnOther += "UnitName,"; strGroupBy += " ,UnitName"; strGroupByOther += " UnitName,"; strOrderBy += ",UnitName";
            }

            if (chkSaleBillType.Checked)
            {
                strColumnName += "SaleBillType,"; strColumnOther += "SaleBillType,"; strGroupBy += " ,SaleBillType"; strGroupByOther += " SaleBillType,"; strOrderBy += ",SaleBillType";
            }
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

            if (strColumnName != "")
                strColumnName = strColumnName.Remove(strColumnName.Length - 1);
            if (strColumnOther != "")
                strColumnOther = strColumnOther.Remove(strColumnOther.Length - 1);
            if (strGroupByOther != "")
                strGroupByOther = strGroupByOther.Remove(strGroupByOther.Length - 1);

            if (strGroupByOther.Trim() == "Group by")
                strGroupByOther = "";

            //strQuery = " Select *,(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt) NetTaxableAmt,CAST((CASE WHEN TaxIncluded=1 then((Rate* 100) / (100 + TaxRate)) else Rate end) as Numeric(18,2))TaxableAmt,Rate from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2,SBS.SalesMan,SBS.SaleIncentive,TaxIncluded" + strDepartName + ",(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SBS.MRP * 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded= 1 then((SBS.MRP* 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName=SBS.ItemName)TaxRate from salesbook SB left join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno left join SaleTypeMaster STM on STM.TaxName=SB.SalesType and STM.SaleType='SALES' " + strDepartmentQuery + " )_Sale ";
            if (strColumnOther == "")
                strQuery = "SELECT 1 as SNo SELECT 2 as SNo SELECT 3 as SNo ";
            else
                strQuery = " select " + strColumnOther + " from (Select " + strColumnName + " from (Select *,(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt) NetTaxableAmt,CAST((CASE WHEN TaxIncluded=1 then((Amount* 100) / (100 + TaxRate)) else Amount end) as Numeric(18,2))STaxableAmt from (select SB.*,SBS.BarCode,SBS.BarCode_s,SBS.BrandName,SBS.DesignName,IMH.GroupName HSN,SBS.ItemName,SBS.Variant1,SBS.Variant2,SBS.Variant3,SBS.Variant4,SBS.Variant5,SBS.Qty,SBS.UnitName,SBS.MRP,SBS.SDisPer,SBS.Rate,SBS.Amount,SBS.SONumber,SBS.Other1,SBS.Other2,dbo.GetFullName(SBS.SalesMan)SalesMan,SBS.SaleIncentive,TaxIncluded" + strDepartName + ",(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SBS.MRP * 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded= 1 then((SBS.MRP* 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName=SBS.ItemName)TaxRate from salesbook SB left join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno LEFT JOIN Items IMH on SBS.ItemName = IMH.ItemName left join SaleTypeMaster STM on STM.TaxName=SB.SalesType and STM.SaleType='SALES' " + strDepartmentQuery + " )_Sale )__Sale OUTER APPLY (Select Name from SupplierMaster Where (AreaCode+AccountNo)=SalePartyID)_SM Where BillNo!=0 " + strSubQuery + strGroupByText + strGroupBy + strGroupBycase + ")sales " + strGroupByOther + strOrderByText + strOrderBy + " "
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
                if(chkGroup2.Visible)
                    chkGroup2.Visible = false;
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
                    txtStation.Text = objSearch.strSelectedData;
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
                    SetColounCategory();
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
            txtCategory1.Text = txtCategory2.Text = txtCategory3.Text = txtCategory4.Text = txtCategory5.Text = txtSalesParty.Text = txtFromDate.Text = txtToDate.Text = txtItemName.Text = txtStation.Text = txtBillCode.Text = txtTransportName.Text = txtNetAmt.Text = txtPFromSNo.Text = txtPToSNo.Text = "";
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
                //DataView dataView = new DataView(BindedDT);
                //VirtualDV.DataSource = dataView;
                //dba.ExportToExcel(VirtualDV, "Custom_Sale_Register", "Custom Sale Register");
                if (BindedDT.Rows.Count > 0)
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
                    for (int j = 1; j < BindedDT.Columns.Count + 1; j++)
                    {
                        strHeader = BindedDT.Columns[j - 1].ColumnName;
                        //if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                        //{
                        //    _skipColumn++;
                        //    j++;
                        //}

                        ExcelApp.Cells[1, j - _skipColumn] = BindedDT.Columns[j - 1].ColumnName;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < BindedDT.Rows.Count; k++)
                    {
                        for (int l = 0; l < BindedDT.Columns.Count; l++)
                        {
                            //if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                            //{
                            //    _skipColumn++;
                            //    l++;
                            //}
                            if (l < BindedDT.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = Convert.ToString(BindedDT.Rows[k][l]);
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

        private void CustomSaleRegister_Load(object sender, EventArgs e)
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

        private void txtSalesMan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESMAN", "SELECT SALES MAN", e.KeyCode);
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

        private void lblCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtRowsPerPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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

        private void txtBarCode_S_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "BillNo")
                dgrdDetails.Cursor = Cursors.Hand;
            else
                dgrdDetails.Cursor = Cursors.Arrow;
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
                    UpdateSetting("SALES");
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
                            case "SaleIncentives":
                                chkSaleIncentives.Checked = true; break;
                            case "SalesMan":
                                chkSalesMan.Checked = true; break;
                            case "Amount":
                                chkAmount.Checked = true; break;
                            case "Rate":
                                chkRate.Checked = true; break;
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
                            case "ItemName":
                                chkItemName.Checked = true; break;
                            case "BrandName":
                                chkBrandName.Checked = true; break;
                            case "BillNo":
                                chkBillNo.Checked = true; break;
                            case "NDate":
                                chkNDate.Checked = true; break;
                            case "SalesParty":
                                chkSalesParty.Checked = true; break;
                            case "HSN":
                                chkHSN.Checked = true; break;
                            case "BarCode_S":
                                chkBarCode_S.Checked = true; break;
                            case "SaleBillType":
                                chkSaleBillType.Checked = true; break;
                            case "LRDate":
                                chkLRDate.Checked = true; break;
                            case "LRNo":
                                chkLRNo.Checked = true; break;
                            case "Transport":
                                chkTransport.Checked = true; break;
                            case "ChqAmt":
                                chkChqAmt.Checked = true; break;
                            case "AdvSlipNo":
                                chkAdvSlipNo.Checked = true; break;
                            case "CashAmt":
                                chkCashAmt.Checked = true; break;
                            case "CardAmt":
                                chkCardAmt.Checked = true; break;
                            case "AdvAmt":
                                chkAdvAmt.Checked = true; break;
                            case "ReturnAmt":
                                chkReturnAmt.Checked = true; break;
                            case "ReturnSlipNo":
                                chkReturnSlipNo.Checked = true; break;
                            case "UpdatedBy":
                                chkUpdatedBy.Checked = true; break;
                            case "FinalAmt":
                                chkFinalAmt.Checked = true; break;
                            case "CreatedBy":
                                chkCreatedBy.Checked = true; break;
                            case "SDisPer":
                                chkSDisPer.Checked = true; break;
                            case "WayBillNo":
                                chkWayBillNo.Checked = true; break;
                            case "RoundOfAmt":
                                chkRoundOfAmt.Checked = true; break;
                            case "SubParty":
                                chkSubParty.Checked = true; break;
                            case "UnitName":
                                chkUnitName.Checked = true; break;
                            case "PackingAmt":
                                chkPackingAmt.Checked = true; break;
                            case "OtherAmt":
                                chkOtherAmt.Checked = true; break;
                            case "SpcDisAmt":
                                chkSpcDisAmt.Checked = true; break;
                            case "SalesType":
                                chkSalesType.Checked = true; break;
                            case "SpcDicPer":
                                chkSpcDicPer.Checked = true; break;
                            case "CreditAmt":
                                chkCreditAmt.Checked = true; break;
                            case "Mob":
                                chkMob.Checked = true; break;
                            case "Remarks":
                                chkRemarks.Checked = true; break;
                            case "BarCode":
                                chkBarCode.Checked = true; break;
                            case "DisAmt":
                                chkDisAmt.Checked = true; break;
                            case "Station":
                                chkStation.Checked = true; break;
                            case "PostageAmt":
                                chkPostageAmt.Checked = true; break;
                            case "TaxableAmt":
                                chkTaxableAmt.Checked = true; break;
                            case "DisPer":
                                chkDisPer.Checked = true; break;
                            case "TaxAmt":
                                chkTaxAmt.Checked = true; break;
                            case "MRP":
                                chkMRP.Checked = true; break;
                            case "Department":
                                chkDepartment.Checked = true; break;
                            case "DesignName":
                                chkDesignName.Checked = true; break;
                            case "NetTaxable":
                                chkNetTaxable.Checked = true; break;
                            case "NetAmt":
                                chkNetAmt.Checked = true; break;
                            case "RowPerPage":
                                txtRowsPerPage.Text = Value; break;
                        }
                    }
                }
                if (txtRowsPerPage.Text == "")
                    txtRowsPerPage.Text = "15";
            }
            catch { }
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

        private void getClmNames(int index, ref string DTClmName,ref string RptDispClm)
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
                        RptDispClm = "Gross Amt";
                        DTClmName = "GrossAmt";
                        break;
                    case "chkSaleIncentives":
                        RptDispClm = "Sale Incentive";
                        DTClmName = "SalesIncentive";
                        break;
                    case "chkSalesMan":
                        RptDispClm = "Sales Man";
                        DTClmName = "SalesMan";
                        break;
                    case "chkAmount":
                        RptDispClm = "Amount";
                        DTClmName = "Amount";
                        break;
                    case "chkRate":
                        RptDispClm = "Rate";
                        DTClmName = "Rate";
                        break;
                    case "chkQty":
                        RptDispClm = "Qty";
                        DTClmName = "qty";
                        break;
                    case "chkSize":
                        if (chkSize.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory1;
                            DTClmName = "variant1";
                        }
                        break;
                    case "chkColour":
                        if (chkColour.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory2;
                            DTClmName = "variant2";
                        }
                        break;
                    case "chkVariant3":
                        if (chkVariant3.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory3;
                            DTClmName = "variant3";
                        }
                        break;
                    case "chkVariant4":
                        if (chkVariant4.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory4;
                            DTClmName = "variant4";
                        }
                        break;
                    case "chkItemName":
                        RptDispClm = "Item Name";
                        DTClmName = "ItemName";
                        break;
                    case "chkBrandName":
                        RptDispClm = "Brand Name";
                        DTClmName = "BrandName";
                        break;
                    case "chkBillNo":
                        RptDispClm = "Bill No.";
                        DTClmName = "BillNo";
                        break;
                    case "chkNDate":
                        RptDispClm = "Date";
                        DTClmName = "Date";
                        break;
                    case "chkSalesParty":
                        RptDispClm = "Sundry Debtor";
                        DTClmName = "SalePartyID";
                        break;
                    case "chkHSN":
                        RptDispClm = "HSN";
                        DTClmName = "HSN";
                        break;
                    case "chkBarCode_S":
                        RptDispClm = "Barcode Sec.";
                        DTClmName = "BarCode_S";
                        break;
                    case "chkSaleBillType":
                        RptDispClm = "Sale Bill Type";
                        DTClmName = "SaleBillType";
                        break;
                    case "chkLRDate":
                        RptDispClm = "LR Date";
                        DTClmName = "LRDate";
                        break;
                    case "chkLRNo":
                        RptDispClm = "LR No.";
                        DTClmName = "LRNumber";
                        break;
                    case "chkTransport":
                        RptDispClm = "Transport Name";
                        DTClmName = "TransportName";
                        break;
                    case "chkChqAmt":
                        RptDispClm = "Cheque Amt";
                        DTClmName = "ChequeAmt";
                        break;
                    case "chkAdvSlipNo":
                        RptDispClm = "Adv. Slip.";
                        DTClmName = "AdvanceSlipNo";
                        break;
                    case "chkCashAmt":
                        RptDispClm = "Cash Amt";
                        DTClmName = "CashAmt";
                        break;
                    case "chkCardAmt":
                        RptDispClm = "Card Amt";
                        DTClmName = "CardAmt";
                        break;
                    case "chkAdvAmt":
                        RptDispClm = "Adv. Amt";
                        DTClmName = "AdvanceAmt";
                        break;
                    case "chkReturnAmt":
                        RptDispClm = "Return Amt";
                        DTClmName = "ReturnAmt";
                        break;
                    case "chkReturnSlipNo":
                        RptDispClm = "Return Slip";
                        DTClmName = "ReturnSlipNo";
                        break;
                    case "chkUpdatedBy":
                        RptDispClm = "Updated By";
                        DTClmName = "UpdatedBy";
                        break;
                    case "chkFinalAmt":
                        RptDispClm = "Final Amt";
                        DTClmName = "FinalAmt";
                        break;
                    case "chkCreatedBy":
                        RptDispClm = "Created By";
                        DTClmName = "CreatedBy";
                        break;
                    case "chkSDisPer":
                        RptDispClm = "Dis. %";
                        DTClmName = "SDisPer";
                        break;
                    case "chkWayBillNo":
                        RptDispClm = "WayBill No.";
                        DTClmName = "WayBillNo";
                        break;
                    case "chkRoundOfAmt":
                        RptDispClm = "Round Off";
                        DTClmName = "RoundOffAmt";
                        break;
                    case "chkSubParty":
                        RptDispClm = "Sub Party";
                        DTClmName = "SubPartyID";
                        break;
                    case "chkUnitName":
                        RptDispClm = "Unit Name";
                        DTClmName = "UnitName";
                        break;
                    case "chkPackingAmt":
                        RptDispClm = "Packing Amt";
                        DTClmName = "PackingAmt";
                        break;
                    case "chkOtherAmt":
                        RptDispClm = "Other Amt";
                        DTClmName = "OtherAmt";
                        break;
                    case "chkSpcDisAmt":
                        RptDispClm = "Spl.Dis Amt";
                        DTClmName = "SpecialDscAmt";
                        break;
                    case "chkSalesType":
                        RptDispClm = "Sales Type";
                        DTClmName = "SalesType";
                        break;
                    case "chkSpcDicPer":
                        RptDispClm = "Spl.Dis%";
                        DTClmName = "SpecialDscPer";
                        break;
                    case "chkCreditAmt":
                        RptDispClm = "Credit Amt";
                        DTClmName = "CreditAmt";
                        break;
                    case "chkMob":
                        RptDispClm = "Mobile";
                        DTClmName = "MobileNo";
                        break;
                    case "chkRemarks":
                        RptDispClm = "Remarks";
                        DTClmName = "Remark";
                        break;
                    case "chkBarCode":
                        RptDispClm = "BarCode";
                        DTClmName = "BarCode";
                        break;
                    case "chkDisAmt":
                        RptDispClm = "Dis. Amt";
                        DTClmName = "DisAmt";
                        break;
                    case "chkStation":
                        RptDispClm = "Station";
                        DTClmName = "Station";
                        break;
                    case "chkPostageAmt":
                        RptDispClm = "Postage Amt";
                        DTClmName = "PostageAmt";
                        break;
                    case "chkTaxableAmt":
                        RptDispClm = "Taxable Amt";
                        DTClmName = "STaxableAmt";
                        break;
                    case "chkNetTaxable":
                        RptDispClm = "Net Taxable";
                        DTClmName = "NetTaxableAmt";
                        break;
                    case "chkDisPer":
                        RptDispClm = "Dis. %";
                        DTClmName = "DisPer";
                        break;
                    case "chkTaxAmt":
                        RptDispClm = "Tax Amt";
                        DTClmName = "TaxAmt";
                        break;
                    case "chkMRP":
                        RptDispClm = "MRP";
                        DTClmName = "MRP";
                        break;
                    case "chkDepartment":
                        RptDispClm = "Department";
                        DTClmName = "Department";
                        break;
                    case "chkDesignName":
                        RptDispClm = "Design Name";
                        DTClmName = "DesignName";
                        break;
                    case "chkNetAmt":
                        RptDispClm = "Net Amt";
                        DTClmName = "NetAmt";
                        break;
                }
            }
            else
            {
                RptDispClm = "";
                DTClmName = "";
            }
        }
        private string getDateString(DataRow dr,string clm)
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
                        _row["TQty"] = "Total Qty : "+lblTotQty.Text;
                    if (chkGrossAmt.Checked)
                        _row["TGross"] = "Gross Amt : " + lblGrossAmt.Text;
                    if (chkTaxableAmt.Checked)
                        _row["TTaxable"] = "Taxable Amt : " + lblSTaxableAmt.Text;
                    if (chkTaxAmt.Checked)
                        _row["TTax"] = "Tax Amt : " + lblTaxAmt.Text;

                    _row["TNet"] = "Net Amt : " + lblNetAmt.Text;

                    if(txtSalesParty.Text!= "")
                    _row["CustomerName"] = "Customer : " + txtSalesParty.Text;
                    _row["HeaderName"] = "Custom Sale Report";
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
                Reporting.ShowReport objReport = new Reporting.ShowReport("CUSTOM SALES REPORT PREVIEW");
                objReport.myPreview.ReportSource = objSalesManReport;
                objReport.ShowDialog();

            }
            objSalesManReport.Close();
            objSalesManReport.Dispose();
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
                string[] strReport = { "Exception occurred in Preview in Custom Sales Report", ex.Message };
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
                string[] strReport = { "Exception occurred in Preview  in Sales Man Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }

        private void chkBarCode_CheckedChanged(object sender, EventArgs e)
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
                    SearchCategory objSearch = new SearchCategory(txt.Name.Substring(txt.Name.Length-1,1), VarName, "", "", "", "", "", "", e.KeyCode, false, "");
                    objSearch.ShowDialog();
                    txt.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void MinRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            chkGroup2.Visible = !chkGroup2.Visible;
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
