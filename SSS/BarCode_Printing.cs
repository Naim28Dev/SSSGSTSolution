using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing.Printing;

namespace SSS
{
    public partial class BarCode_Printing : Form
    {
        DataBaseAccess dba;
        string strSerialCode = "", strSerialNo = "", _strBarCodingType = MainPage.strBarCodingType;
        DataTable dtUnitName = new DataTable();
        public BarCode_Printing(string strSupplierName, string strAgentName, string strSCode, string strSNo, string strDate, DataGridView dgrd)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strSerialCode = strSCode;
            strSerialNo = strSNo;
            SetCategory();
            dtUnitName.Columns.Add("ItemName", typeof(String));
            dtUnitName.Columns.Add("UnitName", typeof(String));
            dtUnitName.Columns.Add("BarcodingType", typeof(String));
            SetPrintFormat();
            BindData(strSupplierName, strAgentName, strDate, dgrd);
        }

        public BarCode_Printing(string strSCode, string strSNo, string strDate, DataGridView dgrd, string strItemName, string strDesignName, string strBrandName, string strUnitName, string strSUnit, string strCodingType)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strSerialCode = strSCode;
            strSerialNo = strSNo;
            _strBarCodingType = strCodingType;
            SetCategory();
            SetPrintFormat();
            BindDataWithDesign(strDate, dgrd, strItemName, strDesignName, strBrandName, strUnitName, strSUnit);
        }

        private void SetPrintFormat()
        {
            try
            {
                cmbPrintFormat.DisplayMember = "Text";
                cmbPrintFormat.ValueMember = "Value";
                var items = new[]{
                new {Text="40x25",Value="40x25"},
                new {Text="38x38_2Column",Value="38x38_2Column"},
                new {Text="40x25_2Column",Value="40x25_2Column"},
                new {Text="43x25_2Column",Value="43x25_2Column"},
                new {Text="50x25",Value="50x25"},
                new {Text="50x38",Value="50x38"},
                new {Text="70x50",Value="70x50"},
                new {Text="70x50_2",Value="70x50_2"},
            };
                cmbPrintFormat.DataSource = items;
                cmbPrintFormat.SelectedValue = "50x38";
                cmbPrintFormat.Text = "50x38";
            }
            catch { }
        }
        private void SetCategory()
        {
            chkVariant1.Enabled = chkVariant2.Enabled = false;
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = chkVariant1.Text = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = chkVariant1.Enabled = true;

                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = chkVariant2.Text = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = chkVariant2.Enabled = true;
                }
                else
                    dgrdDetails.Columns["variant2"].Visible = false;

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
            catch
            {
            }
        }

        private void BindDataWithDesign(string strDate, DataGridView dgrd, string strItemName, string strDesignName, string strBrandName, string strUnitName, string strSUnit)
        {
            try
            {
                txtPurchaseParty.Text = "";
                DateTime _date = dba.ConvertDateInExactFormat(strDate);
                txtDate.Text = _date.ToString("yyMMdd");
                chkAll.Checked = true;
                if (dgrd.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dgrd.Rows.Count);
                    int _rowIndex = 0;
                    double dMRP = 0;
                    if (dgrd.Columns.Contains("supplierName"))
                    {
                        txtPurchaseParty.Text = Convert.ToString(dgrd.Rows[dgrd.Rows.Count - 1].Cells["supplierName"].Value);
                    }

                    foreach (DataGridViewRow row in dgrd.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["chkTick"].Value = true;
                        dgrdDetails.Rows[_rowIndex].Cells["id"].Value = "0";
                        dgrdDetails.Rows[_rowIndex].Cells["srNo"].Value = row.Cells["srNo"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["barCode"].Value = row.Cells["barCodeID"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant1"].Value = row.Cells["category1"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant2"].Value = row.Cells["category2"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant3"].Value = row.Cells["category3"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant4"].Value = row.Cells["category4"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant5"].Value = row.Cells["category5"].Value;

                        dgrdDetails.Rows[_rowIndex].Cells["pMRP"].Value = row.Cells["purchaseRate"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["pRate"].Value = row.Cells["openingRate"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value = row.Cells["openingQty"].Value;
                        if (_strBarCodingType == "UNIQUE_BARCODE")
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["printQty"].Value = row.Cells["openingQty"].Value;
                            dgrdDetails.Rows[_rowIndex].Cells["setQty"].Value = 1;
                        }
                        else
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["printQty"].Value = 1;
                            dgrdDetails.Rows[_rowIndex].Cells["setQty"].Value = row.Cells["openingQty"].Value;
                        }

                        if (dgrd.Columns.Contains("brandName") && dgrd.Columns.Contains("styleName"))
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["brandName"].Value = row.Cells["brandName"].Value;
                            dgrdDetails.Rows[_rowIndex].Cells["designName"].Value = row.Cells["styleName"].Value;
                        }
                        else
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["brandName"].Value = strBrandName;
                            dgrdDetails.Rows[_rowIndex].Cells["designName"].Value = strDesignName;
                        }

                        dgrdDetails.Rows[_rowIndex].Cells["itemName"].Value = strItemName;
                        dgrdDetails.Rows[_rowIndex].Cells["unitName"].Value = strUnitName;
                        dgrdDetails.Rows[_rowIndex].Cells["stockUnitName"].Value = strSUnit;
                        dgrdDetails.Rows[_rowIndex].Cells["sMRP"].Value = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                        dgrdDetails.Rows[_rowIndex].Cells["SRate"].Value = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                        dgrdDetails.Rows[_rowIndex].Cells["BarCodingType"].Value = _strBarCodingType;

                        _rowIndex++;
                    }
                }
                GetPartyNickName(txtPurchaseParty.Text, "");
            }
            catch { }
        }

        private void BindData(string strSupplierName, string strAgentName, string strDate, DataGridView dgrd)
        {
            try
            {
                txtPurchaseParty.Text = strSupplierName;
                DateTime _date = dba.ConvertDateInExactFormat(strDate);
                txtDate.Text = _date.ToString("yyMMdd");
                chkAll.Checked = true;
                if (dgrd.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dgrd.Rows.Count);
                    int _rowIndex = 0;
                    double dMRP = 0;
                    string strUnitName = "", strCodingType = "";
                    foreach (DataGridViewRow row in dgrd.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["chkTick"].Value = true;
                        dgrdDetails.Rows[_rowIndex].Cells["id"].Value = row.Cells["id"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["srNo"].Value = row.Cells["srNo"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["barCode"].Value = row.Cells["barCode"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["brandName"].Value = row.Cells["brandName"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant1"].Value = row.Cells["variant1"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant2"].Value = row.Cells["variant2"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant3"].Value = row.Cells["variant3"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant4"].Value = row.Cells["variant4"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["variant5"].Value = row.Cells["variant5"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["designName"].Value = row.Cells["styleName"].Value;
                        if (dgrd.Columns.Contains("wsMRP"))
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["pMRP"].Value = row.Cells["wsMRP"].Value;
                        }
                        else
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["pMRP"].Value = row.Cells["mrp"].Value;
                        }
                        dgrdDetails.Rows[_rowIndex].Cells["pRate"].Value = row.Cells["rate"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["qty"].Value = row.Cells["qty"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["setQty"].Value = 1;

                        dgrdDetails.Rows[_rowIndex].Cells["itemName"].Value = row.Cells["itemName"].Value;
                        dgrdDetails.Rows[_rowIndex].Cells["unitName"].Value = row.Cells["unitName"].Value;
                        if (row.DataGridView.Columns.Contains("cMrp"))
                            dMRP = dba.ConvertObjectToDouble(row.Cells["cMrp"].Value);
                        else
                            dMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);

                        dgrdDetails.Rows[_rowIndex].Cells["sMRP"].Value = dMRP;
                        dgrdDetails.Rows[_rowIndex].Cells["SRate"].Value = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);

                        //double dMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                        //GetSaleRate(dgrdDetails.Rows[_rowIndex], dMRP);
                        strUnitName = GetStockUnitName(row.Cells["itemName"].Value, ref strCodingType);
                        if (strUnitName != "")
                            dgrdDetails.Rows[_rowIndex].Cells["stockUnitName"].Value = strUnitName;
                        else
                            dgrdDetails.Rows[_rowIndex].Cells["stockUnitName"].Value = row.Cells["unitName"].Value;

                        if (strCodingType == "UNIQUE_BARCODE")
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["printQty"].Value = row.Cells["qty"].Value;
                            dgrdDetails.Rows[_rowIndex].Cells["setQty"].Value = 1;

                        }
                        else
                        {
                            dgrdDetails.Rows[_rowIndex].Cells["printQty"].Value = 1;
                            dgrdDetails.Rows[_rowIndex].Cells["setQty"].Value = row.Cells["qty"].Value;
                        }

                        dgrdDetails.Rows[_rowIndex].Cells["BarCodingType"].Value = strCodingType;

                        _rowIndex++;
                    }
                }

                GetPartyNickName(strSupplierName, strAgentName);
            }
            catch (Exception ex) { }
        }

        private string GetStockUnitName(object objItemName, ref string strCodingType)
        {
            string strUnitName = "";
            DataRow[] rows = dtUnitName.Select("ItemName='" + objItemName + "' ");
            if (rows.Length > 0)
            {
                strUnitName = Convert.ToString(rows[0]["UnitName"]);
                strCodingType = Convert.ToString(rows[0]["BarcodingType"]);
            }
            else
            {
                DataTable _dt = dba.GetDataTable("Select (CAST(QtyRatio as varchar)+' '+StockUnitName)S,isnull(BarcodingType,'') BarcodingType from Items WHERE ItemName = '" + objItemName + "'");
                DataRow row = dtUnitName.NewRow();
                row["ItemName"] = objItemName;
                row["UnitName"] = Convert.ToString(_dt.Rows[0]["S"]);
                row["BarcodingType"] = strCodingType = Convert.ToString(_dt.Rows[0]["BarcodingType"]);
                dtUnitName.Rows.Add(row);
                strUnitName = Convert.ToString(_dt.Rows[0]["S"]);
            }
            return strUnitName;
        }

        private void GetPartyNickName(string strSupplierName, string strAgentName)
        {
            string[] str = strSupplierName.Split(' ');
            if (str.Length > 0)
            {
                string[] strAgent = strAgentName.Split(' ');

                string strQuery = "Select Other,Station,PvtMarka from SupplierMaster Where (AreaCode+AccountNo)='" + str[0] + "' "
                                + " Select * from BarcodeSetting ";
                if (strAgentName != "")
                    strQuery += " Select PvtMarka as AgentCode from SupplierMaster Where (AreaCode+AccountNo)='" + strAgent[0] + "' ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    DataTable dt2 = new DataTable();
                    if (ds.Tables.Count > 2)
                        dt2 = ds.Tables[2];
                    if (dt2.Rows.Count > 0)
                        txtAgentCode.Text = Convert.ToString(dt2.Rows[0]["AgentCode"]);
                    else
                        txtAgentCode.Text = "";
                    if (dt.Rows.Count > 0)
                    {
                        txtSupplierCode.Text = Convert.ToString(dt.Rows[0]["PvtMarka"]);
                        txtRatePrefix.Text = MainPage.strBranchCode;
                    }
                    DataTable _dtBarCode = ds.Tables[1];
                    if (_dtBarCode.Rows.Count > 0)
                    {
                        DataRow row = _dtBarCode.Rows[0];
                        txtRatePrefix.Text = Convert.ToString(row["RatePrefix"]);
                        chkSupplier.Checked = Convert.ToBoolean(row["SupplierCode"]);
                        chkDate.Checked = Convert.ToBoolean(row["PurchaseDate"]);
                        chkMRP.Checked = Convert.ToBoolean(row["MRP"]);
                        chkRate.Checked = Convert.ToBoolean(row["Rate"]);
                        chkBarCode.Checked = Convert.ToBoolean(row["Barcode"]);
                        chkBrandName.Checked = Convert.ToBoolean(row["Brand"]);
                        chkDesign.Checked = Convert.ToBoolean(row["DesignName"]);
                        chkVariant1.Checked = Convert.ToBoolean(row["Size"]);
                        chkVariant2.Checked = Convert.ToBoolean(row["Color"]);
                        chkQty.Checked = Convert.ToBoolean(row["Qty"]);
                        chkPCity.Checked = Convert.ToBoolean(row["PurchaseCity"]);
                        chkPurchaseRate.Checked = Convert.ToBoolean(row["PurchaseRate"]);

                        if (Convert.ToString(row["Remark"]) != "")
                            cmbPrintFormat.SelectedValue = Convert.ToString(row["Remark"]);
                    }
                }
            }
        }

        // private void GetSaleRate(DataGridViewRow row,double  dMRP)
        //{
        //    try
        //    {
        //        double dDisPer = 0, dRate = 0;

        //        if (row != null)
        //        {
        //            object objDisPer = 0;
        //            if (Convert.ToString(row.Cells["itemName"].Value) != "")
        //            {
        //                string strQuery = " Select Top 1 ISNULL(_ICM.DisPer,0) DisPer from Items _IM inner join ItemCategoryMaster  _ICM on _IM.Other=_ICM.CategoryName Where _Im.ItemName='"+ row.Cells["itemName"].Value + "' and "+ dMRP +"> _ICM.FromRange and "+ dMRP +"< _ICM.ToRange ";
        //                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);

        //                dDisPer = dba.ConvertObjectToDouble(objValue) * -1;
        //                //dMRP = dba.ConvertObjectToDouble(objValue);
        //            }
        //        }             

        //        if (dDisPer != 0 && dMRP != 0)
        //            dRate = dMRP * (100.00 + dDisPer) / 100.00;
        //        if (dRate == 0)
        //            dRate = dMRP;

        //        row.Cells["sMRP"].Value = Math.Round(dMRP,0);
        //        row.Cells["SRate"].Value = Math.Round(dRate, 0);               
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

        private void BarCode_Printing_KeyDown(object sender, KeyEventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSupplierCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
        private DataTable GetTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("ItemName", typeof(string));
            _dt.Columns.Add("Variant1", typeof(string));
            _dt.Columns.Add("Variant2", typeof(string));
            _dt.Columns.Add("Variant3", typeof(string));
            _dt.Columns.Add("Variant4", typeof(string));
            _dt.Columns.Add("Variant5", typeof(string));
            _dt.Columns.Add("MRP", typeof(string));
            _dt.Columns.Add("BarCode", typeof(string));
            _dt.Columns.Add("BarCodeID", typeof(string));
            _dt.Columns.Add("Qty", typeof(string));
            _dt.Columns.Add("SetNo", typeof(string));
            _dt.Columns.Add("Date", typeof(string));
            _dt.Columns.Add("HeaderImage", typeof(byte[]));
            _dt.Columns.Add("HeaderName", typeof(string));

            return _dt;
        }
        private DataTable CreateDataTable(DataGridViewRow row)
        {
            DataTable _dt = GetTable();

            string strDesignName = "", strVariant1 = "", strVariant2 = "", strFullItemName = "", strMRP = "", strQty = "", strBrandName = "", selFormat = "Format1", strSetNo = "";
            if (MainPage.strCompanyName.Contains("FIVE") || MainPage.strCompanyName.Contains("FIVE"))
                strSetNo = "LOTUS";
            else
                strSetNo = "SETNO";

            double dSaleRate = 0;
            DateTime _pDate = MainPage.currentDate;
            if (txtDate.Text.Length == 10)
                _pDate = dba.ConvertDateInExactFormat(txtDate.Text);

            dSaleRate = dba.ConvertObjectToDouble(row.Cells["SRate"].Value);
            if (dSaleRate > 0)
            {
                selFormat = Convert.ToString(cmbPrintFormat.SelectedValue);
                DataRow _row = _dt.NewRow();
                if (selFormat == "40x25_2Column")
                {
                    if (chkDesign.Checked)
                    {
                        strDesignName = Convert.ToString(row.Cells["designName"].Value);
                        if (strDesignName == "")
                            strDesignName = Convert.ToString(row.Cells["itemName"].Value);
                    }
                    if (chkVariant2.Checked)
                        strVariant2 = Convert.ToString(row.Cells["variant2"].Value);
                    strDesignName = strDesignName + " " + strVariant2;

                    _row["ItemName"] = strDesignName;

                    if (chkVariant1.Checked)
                        strVariant1 = Convert.ToString(row.Cells["variant1"].Value);
                    if (strVariant1 != "")
                        strVariant1 = MainPage.StrCategory1.ToUpper() + "- " + strVariant1 + " ";
                    _row["Variant1"] = strVariant1;

                    if (chkBrandName.Checked)
                        strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                    if (strBrandName != "")
                        _row["Variant1"] = strVariant1 + "BRAND: " + strBrandName;

                    if (chkMRP.Checked)
                        strMRP = "MRP: " + dba.ConvertObjectToDouble(row.Cells["sMRP"].Value).ToString("N2", MainPage.indianCurancy);
                    _row["MRP"] = strMRP.Trim();
                    if (chkDate.Checked)
                        _row["Date"] = GetDateCode();

                    if (chkSupplier.Checked)
                        _row["SetNo"] = txtSupplierCode.Text + "." + txtAgentCode.Text;

                    _row["HeaderImage"] = MainPage._headerImage;
                    _row["HeaderName"] = MainPage.strPrintComapanyName;
                }
                else if (selFormat == "38x38_2Column")
                {
                    strDesignName = Convert.ToString(row.Cells["itemName"].Value);
                    if (chkDesign.Checked)                   
                        strDesignName += " "+Convert.ToString(row.Cells["designName"].Value);                      
                   
                    strDesignName = strDesignName.Trim();

                    _row["ItemName"] = strDesignName;

                    if (chkVariant1.Checked)
                        strVariant1 = Convert.ToString(row.Cells["variant1"].Value);
                    if (strVariant1 != "")
                        strVariant1 = MainPage.StrCategory1.ToUpper() + "- " + strVariant1 + " ";
                    _row["Variant1"] = strVariant1;

                    if (chkBrandName.Checked)
                        strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                    if (strBrandName != "")
                        _row["Variant1"] = strVariant1 + "BRAND: " + strBrandName;

                    if (chkMRP.Checked)
                        strMRP = "MRP: " + dba.ConvertObjectToDouble(row.Cells["sMRP"].Value).ToString("N2", MainPage.indianCurancy);
                    _row["MRP"] = strMRP.Trim();
                    if (chkDate.Checked)
                        _row["Date"] = GetDateCode();

                    if (chkSupplier.Checked)
                        _row["SetNo"] = txtSupplierCode.Text + "." + txtAgentCode.Text;

                    _row["HeaderImage"] = MainPage._headerImage;
                    _row["HeaderName"] = MainPage.strPrintComapanyName;
                }
                else if (selFormat == "43x25_2Column")
                {
                    if (chkDesign.Checked)
                    {
                        strDesignName = Convert.ToString(row.Cells["designName"].Value);
                        if (strDesignName == "")
                            strDesignName = Convert.ToString(row.Cells["itemName"].Value);
                    }
                    if (chkVariant2.Checked)
                        strVariant2 = Convert.ToString(row.Cells["variant2"].Value);
                    strDesignName = strDesignName + " " + strVariant2;

                    _row["ItemName"] = strDesignName;

                    if (chkVariant1.Checked)
                        strVariant1 = Convert.ToString(row.Cells["variant1"].Value);
                    if (strVariant1 != "")
                        strVariant1 = MainPage.StrCategory1.ToUpper() + " - " + strVariant1 + " ";

                    _row["Variant1"] = strVariant1;
                    if (chkBrandName.Checked)
                        strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                    if (strBrandName != "")
                        _row["Variant1"] = strVariant1 + " - " + strBrandName;

                    if (chkMRP.Checked)
                        strMRP ="` "+ dba.ConvertObjectToDouble(row.Cells["sMRP"].Value).ToString("N0", MainPage.indianCurancy)+"/-";// "MRP: " + dba.ConvertObjectToDouble(row.Cells["sMRP"].Value).ToString("N0", MainPage.indianCurancy) + "/-";
                    _row["MRP"] = strMRP.Trim();

                    if (chkSupplier.Checked)
                        _row["SetNo"] = txtSupplierCode.Text + "." + txtAgentCode.Text;

                    _row["HeaderImage"] = MainPage._headerImage;
                    _row["HeaderName"] = MainPage.strPrintComapanyName;
                }
                else
                {
                    if (chkDesign.Checked)
                    {
                        strDesignName = Convert.ToString(row.Cells["designName"].Value);
                        if (strDesignName == "")
                            strDesignName = Convert.ToString(row.Cells["itemName"].Value);
                    }
                    if (chkVariant1.Checked)
                        strVariant1 = Convert.ToString(row.Cells["variant1"].Value);

                    if (Convert.ToString(row.Cells["variant2"].Value) != "" && chkVariant2.Checked)
                    {
                        if (strVariant1 != "")
                            strVariant1 += " (";
                        strVariant1 += Convert.ToString(row.Cells["variant2"].Value);

                        if (strVariant1.Contains("("))
                            strVariant1 += ")";
                    }

                    strFullItemName = (strDesignName + " " + strVariant1).Trim();

                    if (chkBrandName.Checked)
                        _row["Variant2"] = strBrandName = Convert.ToString(row.Cells["brandName"].Value);

                    if (selFormat == "70x50_2")
                    {
                        if (strBrandName != "" && !strDesignName.Contains("/"))
                            strBrandName = "/" + strBrandName;
                        _row["ItemName"] = strDesignName + strBrandName;
                        //_row["Variant1"] = "SIZE - " + strVariant1;
                    }
                    else
                    {
                        _row["ItemName"] = "D.No. " + strDesignName;
                        //_row["Variant1"] = strVariant1;
                    }

                    if (chkSupplier.Checked)
                        _row["Variant3"] = txtSupplierCode.Text;
                    if (chkPCity.Checked)
                        _row["Variant4"] = txtAgentCode.Text;
                    if (chkPurchaseRate.Checked)
                        _row["Variant5"] = GetPartyCode(row.Cells["pRate"].Value);
                    if (chkQty.Checked)
                        _row["Qty"] = "QTY - " + row.Cells["stockUnitName"].Value;

                    if (strVariant1 != "")
                        _row["Variant1"] = "SIZE - " + strVariant1;

                    if (selFormat == "50x38")
                    {
                        if (strVariant1 != "")
                            _row["Variant1"] = Convert.ToString(_row["Variant1"]) + " " + Convert.ToString(_row["Qty"]);
                        else
                            _row["Variant1"] = Convert.ToString(_row["Qty"]);
                    }

                    _row["SetNo"] = strSetNo + _pDate.Month.ToString("00") + "" + Convert.ToDouble(dSaleRate.ToString("0")) + "" + _pDate.ToString("yy");

                    if (chkMRP.Checked)
                        strMRP = "MRP: " + dba.ConvertObjectToDouble(row.Cells["sMRP"].Value).ToString("N0", MainPage.indianCurancy) + "/-";
                    if (chkRate.Checked)
                    {
                        if (strMRP != "")
                            strMRP += "  ";
                        strMRP += (txtRatePrefix.Text + "PRICE: " + dSaleRate.ToString("N0", MainPage.indianCurancy)).Trim() + "/-";
                    }
                    _row["MRP"] = strMRP.Trim();

                    if (chkDate.Checked)
                        _row["Date"] = txtDate.Text;
                    _row["HeaderImage"] = MainPage._headerImage;
                    _row["HeaderName"] = MainPage.strPrintComapanyName;
                }
                _dt.Rows.Add(_row);
            }
            return _dt;
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                // if (MainPage.bUniqueBarCode)
                //e.Cancel = true;
            }
            else if (e.ColumnIndex != 1 && e.ColumnIndex != 14)
                e.Cancel = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["chkTick"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;


                int _printQty = 0, _setQty = 0, _lastPrintNo = 0;
                string strParentBarCode = "", strBarCode = "", strPBarCode = "", _strShortBarCode, BarCodingType = "";
                ReportClass ReportToPrint = new ReportClass();
                PrinterSettings _DefPrS = new PrinterSettings();
                PageSettings _DefPgS = _DefPrS.DefaultPageSettings;
                DataTable dt = GetTable().Clone();
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkTick"].Value))
                    {
                        BarCodingType = Convert.ToString(row.Cells["BarCodingType"].Value);

                        _printQty = dba.ConvertObjectToInt(row.Cells["printQty"].Value);
                        _setQty = dba.ConvertObjectToInt(row.Cells["setQty"].Value);
                        strParentBarCode = Convert.ToString(row.Cells["barCode"].Value);
                        if (_printQty > 0 && _setQty > 0)
                        {
                            if (BarCodingType == "UNIQUE_BARCODE")
                            {
                                if (chkPrintFromStart.Checked)
                                    _lastPrintNo = 0;
                                else
                                    _lastPrintNo = GetLastPrintNo(strParentBarCode);
                            }

                            string[] strBCode = strParentBarCode.Split('-');
                            if (strBCode.Length > 1)
                                strPBarCode = strBCode[1];
                            else
                                strPBarCode = strBCode[0];

                            try
                            {
                                for (int _i = 0; _i < _printQty; _i++)
                                {
                                    _lastPrintNo++;
                                    DataRowCollection dtrC = CreateDataTable(row).Rows;
                                    if (dtrC.Count > 0)
                                    {
                                        DataRow dtr = dtrC[0];
                                        string BarcodeID = "";
                                        if (BarCodingType != "")
                                        {
                                            if (BarCodingType == "UNIQUE_BARCODE")
                                                BarcodeID = "." + _lastPrintNo;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please select barcoding type in Company Setting or Design Master.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            break;
                                        }

                                        strBarCode = strParentBarCode + BarcodeID;
                                        _strShortBarCode = strPBarCode + BarcodeID;
                                        if (chkBarCode.Checked)
                                        {
                                            dtr["BarCode"] = "*" + _strShortBarCode + "*";
                                            dtr["BarCodeID"] = _strShortBarCode;
                                        }
                                        try
                                        {
                                            for (int _j = 0; _j < _setQty; _j++)
                                            {
                                                dt.ImportRow(dtr);
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex; //MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objShow = new Reporting.ShowReport("BAR CODE PREVIEW");
                    GetReportFormat(ref ReportToPrint);
                    ReportToPrint.SetDataSource(dt);
                    objShow.myPreview.ReportSource = ReportToPrint;
                    objShow.ShowDialog();
                }
                ReportToPrint.Close();
                ReportToPrint.Dispose();
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnPreview.Enabled = true;
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void UpdateSetting()
        {
            string strQuery = " if exists (Select SupplierCode from [dbo].[BarcodeSetting]) begin "
                            + " Update [dbo].[BarcodeSetting] Set [RatePrefix]='" + txtRatePrefix.Text + "',[SupplierCode]='" + chkSupplier.Checked + "',[PurchaseDate]='" + chkDate.Checked + "',[MRP]='" + chkMRP.Checked + "',[Rate]='" + chkRate.Checked + "',[Barcode]='" + chkBarCode.Checked + "',[Brand]='" + chkBrandName.Checked + "',[DesignName]='" + chkDesign.Checked + "',[Size]='" + chkVariant1.Checked + "',[Color]='" + chkVariant2.Checked + "',[Qty]='" + chkQty.Checked + "',[PurchaseCity]='" + chkPCity.Checked + "',[PurchaseRate]='" + chkPurchaseRate.Checked + "',[Remark]='" + cmbPrintFormat.SelectedValue + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 end else begin "
                            + " INSERT INTO [dbo].[BarcodeSetting] ([RatePrefix],[SupplierCode],[PurchaseDate],[MRP],[Rate],[Barcode],[Brand],[DesignName],[Size],[Color],[Qty],[PurchaseCity],[PurchaseRate],[Remark],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES ('" + txtRatePrefix.Text + "','" + chkSupplier.Checked + "','" + chkDate.Checked + "','" + chkMRP.Checked + "','" + chkRate.Checked + "','" + chkBarCode.Checked + "','" + chkBrandName.Checked + "','" + chkDesign.Checked + "','" + chkVariant1.Checked + "','" + chkVariant2.Checked + "','" + chkQty.Checked + "','" + chkPCity.Checked + "','" + chkPurchaseRate.Checked + "','" + cmbPrintFormat.SelectedValue + "','" + MainPage.strLoginName + "','',1,0) end ";

            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                MessageBox.Show("Thank you ! Bar code setting updated successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                MessageBox.Show("Sorry ! Unable to update right now!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                PrintBarCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnPrint.Enabled = true;
        }

        private int GetLastPrintNo(string strParentBarCode)
        {
            int dNo = 0;
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX([LastPrintNo]),0) from [dbo].[BarcodeDetails] Where BillCode='" + strSerialCode + "' and BillNo=" + strSerialNo + " and [ParentBarCode]='" + strParentBarCode + "' ");
            dNo = dba.ConvertObjectToInt(objValue);
            return dNo;
        }

        private void PrintBarCode()
        {
            int _printQty = 0, _setQty = 0, _lastPrintNo = 0;
            string strQuery = "", strDelQuery = "", strParentBarCode = "", strBarCode = "", strPBarCode = "", _strShortBarCode, BarCodingType = "";
            bool isPrinted = false;

            ReportClass ReportToPrint = new ReportClass();
            PrinterSettings _DefPrS = new PrinterSettings();
            PageSettings _DefPgS = _DefPrS.DefaultPageSettings;
            DataTable dt = GetTable().Clone();
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkTick"].Value))
                {
                    BarCodingType = Convert.ToString(row.Cells["BarCodingType"].Value);

                    _printQty = dba.ConvertObjectToInt(row.Cells["printQty"].Value);
                    _setQty = dba.ConvertObjectToInt(row.Cells["setQty"].Value);
                    strParentBarCode = Convert.ToString(row.Cells["barCode"].Value);
                    if (_printQty > 0 && _setQty > 0)
                    {
                        if (BarCodingType == "UNIQUE_BARCODE")
                        {
                            if (chkPrintFromStart.Checked)
                                _lastPrintNo = 0;
                            else
                                _lastPrintNo = GetLastPrintNo(strParentBarCode);
                        }

                        string[] strBCode = strParentBarCode.Split('-');
                        if (strBCode.Length > 1)
                            strPBarCode = strBCode[1];
                        else
                            strPBarCode = strBCode[0];

                        try
                        {
                            for (int _i = 0; _i < _printQty; _i++)
                            {
                                _lastPrintNo++;
                                DataRowCollection dtrC = CreateDataTable(row).Rows;
                                if (dtrC.Count > 0)
                                {
                                    DataRow dtr = dtrC[0];
                                    string BarcodeID = "";
                                    if (BarCodingType != "")
                                    {
                                        if (BarCodingType == "UNIQUE_BARCODE")
                                            BarcodeID = "." + _lastPrintNo;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please select barcoding type in Company Setting or Design Master.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        break;
                                    }

                                    strBarCode = strParentBarCode + BarcodeID;
                                    _strShortBarCode = strPBarCode + BarcodeID;
                                    if (chkBarCode.Checked)
                                    {
                                        dtr["BarCode"] = "*" + _strShortBarCode + "*";
                                        dtr["BarCodeID"] = _strShortBarCode;
                                    }
                                    try
                                    {
                                        for (int _j = 0; _j < _setQty; _j++)
                                        {
                                            dt.ImportRow(dtr);
                                        }

                                        if (dt.Rows.Count >= 50)
                                        {
                                            GetReportFormat(ref ReportToPrint);
                                            ReportToPrint.SetDataSource(dt);
                                            if (MainPage._PrintWithDialog)
                                            {
                                                dba.PrintWithDialog(ReportToPrint, isPrinted, 1);
                                                isPrinted = true;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    _DefPrS.Copies = 1;
                                                    ReportToPrint.PrintToPrinter(_DefPrS,_DefPgS,false);
                                                    isPrinted = true;
                                                }
                                                catch { isPrinted = false; }
                                            }
                                            dt.Rows.Clear();
                                        }
                                    }
                                    catch { }
                                    strQuery += " INSERT INTO [dbo].[BarcodeDetails]([BillCode],[BillNo],[ParentBarCode],[BarCode],[NetQty],[SetQty],[LastPrintNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[InStock]) "
                                             + " SELECT '" + strSerialCode + "'," + strSerialNo + ",'" + strParentBarCode + "','" + strBarCode + "'," + _printQty + "," + _setQty + "," + _lastPrintNo + ",'" + MainPage.strLoginName + "','',1,0,1 ";
                                }
                            }
                            if (chkPrintFromStart.Checked)
                                strDelQuery += " DELETE FROM BarcodeDetails WHERE BillCode = '" + strSerialCode + "' AND BillNo = " + strSerialNo + " AND ParentBarcode = '" + strParentBarCode + "'";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            strQuery = "";
                        }
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                GetReportFormat(ref ReportToPrint);
                ReportToPrint.SetDataSource(dt);
                if (MainPage._PrintWithDialog)
                {
                    isPrinted = dba.PrintWithDialog(ReportToPrint, false, 1);
                }
                else
                {
                    try
                    {
                        _DefPrS.Copies = 1;
                        ReportToPrint.PrintToPrinter(_DefPrS, _DefPgS, false);
                        isPrinted = true;
                    }
                    catch { isPrinted = false; }
                }
            }
            ReportToPrint.Close();
            ReportToPrint.Dispose();

            if (strQuery != "")// && isPrinted
            {
                strQuery = strDelQuery + strQuery;

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count <= 0)
                {
                    DialogResult _updateResult = MessageBox.Show("Sorry ! Not able to save the printing records, Please retry to save record.", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                    if (_updateResult == DialogResult.Retry)
                    {
                        _count = dba.ExecuteMyQuery(strQuery);
                        if (_count <= 0)
                            dba.ExecuteMyQuery(strQuery);
                    }
                }
            }
        }

        private void GetReportFormat(ref ReportClass report)
        {
            string selFormat = Convert.ToString(cmbPrintFormat.SelectedValue);
            if (selFormat == "40x25")
            {
                Reporting.BarCode_Print40x25 objReport = new Reporting.BarCode_Print40x25();
                report = objReport;
            }
            else if (selFormat == "38x38_2Column")
            {
                Reporting.BarCode_Print38x38_2Column objReport = new Reporting.BarCode_Print38x38_2Column();
                report = objReport;
            }
            else if (selFormat == "40x25_2Column")
            {
                Reporting.BarCode_Print40x25_2Column objReport = new Reporting.BarCode_Print40x25_2Column();
                report = objReport;
            }
            else if (selFormat == "43x25_2Column")
            {
                Reporting.BarCode_Print43x25_2Column objReport = new Reporting.BarCode_Print43x25_2Column();
                report = objReport;
            }
            else if (selFormat == "50x25")
            {
                Reporting.BarCode_Print50x25 objReport = new Reporting.BarCode_Print50x25();
                report = objReport;
            }
            else if (selFormat == "50x38")
            {
                Reporting.BarCode_Print50x38 objReport = new Reporting.BarCode_Print50x38();
                report = objReport;
            }
            else if (selFormat == "70x50")
            {
                Reporting.BarCode_Print70x50 objReport = new Reporting.BarCode_Print70x50();
                report = objReport;
            }
            else if (selFormat == "70x50_2")
            {
                Reporting.BarCode_Print70x50_2 objReport = new Reporting.BarCode_Print70x50_2();
                report = objReport;
            }
            else
            {
                MessageBox.Show("Please select any format to print. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbPrintFormat.Focus();
                cmbPrintFormat.DroppedDown = true;
            }
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void btnUpdateSetting_Click(object sender, EventArgs e)
        {
            btnUpdateSetting.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to update barcode settings ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    UpdateSetting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnUpdateSetting.Enabled = true;
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 13 || columnIndex == 14)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 13 || columnIndex == 14)
                {
                    dba.KeyHandlerPoint(sender, e, 0);
                }
            }
            catch { }
        }

        private void cmbPrintFormat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BarCode_Printing_Load(object sender, EventArgs e)
        {
            //SetPrintFormat();
        }

        private void chkPAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (Control ctrl in grpCheckbox.Controls)
                {
                    if (ctrl is CheckBox)
                    {
                        if (ctrl != chkPrintFromStart)
                            ((CheckBox)ctrl).Checked = chkPAll.Checked;
                    }
                }

            }
            catch { }
        }

        private string GetPartyCode(object _objPRate)
        {
            double _dRate = dba.ConvertObjectToDouble(_objPRate);
            string strPCode = "", strRate = _dRate.ToString("0");
            char cCode;
            //XEDUCATION
            for (int _index = 0; _index < strRate.Length; _index++)
            {
                cCode = strRate[_index];
                if (cCode == '0')
                    strPCode += "X";
                else if (cCode == '1')
                    strPCode += "E";
                else if (cCode == '2')
                    strPCode += "D";
                else if (cCode == '3')
                    strPCode += "U";
                else if (cCode == '4')
                    strPCode += "C";
                else if (cCode == '5')
                    strPCode += "A";
                else if (cCode == '6')
                    strPCode += "T";
                else if (cCode == '7')
                    strPCode += "I";
                else if (cCode == '8')
                    strPCode += "O";
                else if (cCode == '9')
                    strPCode += "N";
            }
            return strPCode;
        }

        private string GetDateCode()
        {
            string strMCode = "", strYCode = "", strCode = "";
            if (txtDate.TextLength == 6)
            {
                strYCode = txtDate.Text.Substring(0, 2);
                strMCode = txtDate.Text.Substring(2, 2);
                if (dba.ConvertObjectToDouble(strMCode) > 9)
                {
                    strCode = getCode(strMCode.Substring(0, 1));
                    strCode = strCode + getCode(strMCode.Substring(1, 1));
                }
                else
                    strCode = getCode(strMCode.Substring(1, 1));

                if (dba.ConvertObjectToDouble(strYCode) > 9)
                {
                    strCode = strCode + getCode(strYCode.Substring(0, 1));
                    strCode = strCode + getCode(strYCode.Substring(1, 1));
                }
                else
                    strCode = strCode + getCode(strMCode.Substring(1, 1));
            }
            return strCode;
        }
        private string getCode(string strCode)
        {
            //SUMITGOENK
            string Code = "";
            switch (strCode)
            {
                case "1":
                    Code = "S";
                    break;
                case "2":
                    Code = "U";
                    break;
                case "3":
                    Code = "M";
                    break;
                case "4":
                    Code = "I";
                    break;
                case "5":
                    Code = "T";
                    break;
                case "6":
                    Code = "G";
                    break;
                case "7":
                    Code = "O";
                    break;
                case "8":
                    Code = "E";
                    break;
                case "9":
                    Code = "N";
                    break;
                case "0":
                    Code = "K";
                    break;
                default:
                    Code = "";
                    break;
            }
            return Code;
        }
    }
}
