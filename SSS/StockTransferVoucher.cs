using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace SSS
{
    public partial class StockTransferVoucher : Form
    {
        DataBaseAccess dba;  
        public string strAddedOrderDetails = "", _strMainOrderCode = "", _strCurrentOrderCode = "", _strOrderNo_Update, _STRMasterTransportName = "";
        bool qtyAdjustStatus = false;
        SearchData _objData;
        SearchCategory _objSearch;
        SearchCategory_Custom _objSearch_Custom;
        bool  _bVariant1 = false, _bVariant2 = false, _bRoundTo5 = false, _bRoundToU5 = false, _bMUAfterDisc = false, _bMUAfterTax = false, _bMarginIncludeTax=false;

        public StockTransferVoucher()
        {
            InitializeComponent();
            dba = new DataBaseAccess();            
            SetCategory();
            GetStartupData(true);
            txtSourceStSerialDate.Text = "";
        }

        public StockTransferVoucher(string strBillCode, string strBillNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();           
            SetCategory();
            GetStartupData(false);
            txtBillCode.Text = strBillCode;
            txtBillNo.Text = strBillNo;
            BindRecordWithControl(txtBillNo.Text);
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
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


        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from STOCKTRANSFER Where BillCode='" + txtBillCode.Text.Trim() + "'   ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }


        private void GetStartupData(bool _bstatus)
        {
            try
            {
                string strLastSerialNo = "", strQuery = "Select STCode, (Select ISNULL(MAX(BillNo),0) from STOCKTRANSFER Where BillCode=STCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'  Select * from [dbo].[Purchase_Setup] Where CompanyID='" + MainPage.strDataBaseFile + "'  ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtBillCode.Text = txtBillCode.Text = Convert.ToString(dt.Rows[0]["STCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }

                    dt = ds.Tables[1];
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        chkVariant1.Checked = _bVariant1 = Convert.ToBoolean(row["Data1"]);
                        chkVariant2.Checked = _bVariant2 = Convert.ToBoolean(row["Data2"]);
                        chkRoundTo5.Checked = _bRoundTo5 = Convert.ToBoolean(row["Data3"]);
                        chkRoundToU5.Checked = _bRoundToU5 = Convert.ToBoolean(row["Data4"]);
                        chkMUAfterDisc.Checked = _bMUAfterDisc = Convert.ToBoolean(row["Data5"]);
                        chkMuAfterTax.Checked = _bMUAfterTax = Convert.ToBoolean(row["Data6"]);
                        if (Convert.ToString(row["Data7"]) != "")
                            chkMarginIncludeTax.Checked = _bMarginIncludeTax = Convert.ToBoolean(row["Data7"]);
                    }

                    if (strLastSerialNo != "" && strLastSerialNo != "0" && _bstatus)
                        BindRecordWithControl(strLastSerialNo);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Sale Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo  from [STOCKTRANSFER] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SNo"]);
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "",strLRDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                string strNetQuery = "", strUnit = "", strItemName = "", strStockType = "OUT", strSourceBillNo = "", strSourceDate = "NULL";
                if (rdbStockIn.Checked)
                {
                    strStockType = "IN";
                    strSourceBillNo = txtSourceStSerialNo.Text;
                    if (txtSourceStSerialDate.Text.Length==10)
                    {
                        DateTime _sDate = dba.ConvertDateInExactFormat(txtSourceStSerialDate.Text);
                        strSourceDate = "'" + _sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                    }
                }
                else
                {
                    strSourceBillNo = "";
                    strSourceDate = "'" + strDate + "'";
                }

                double dQty = 0, dPrice = 0, dMRP = 0, dAmt = 0,dNetAmt=dba.ConvertObjectToDouble(lblAmt.Text);
                string strQuery = "";

                strQuery = "if not exists(Select Billcode from StockTransfer Where BillCode = '" + txtBillCode.Text + "' and BillNo='" + txtBillNo.Text + "') Begin "
                    + " INSERT INTO [dbo].[STOCKTRANSFER] ([BillCode],[BillNo],[Date],[FromMCentre],[ToMCentre],[Remark],[StockType],[SourceBillNo],[SourceDate],[TotalQty],[TotalAmt],[WaybillNo],[WayBillDate] "
                            + " ,[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Transport],[LRNumber],[LRDate],[Station]) VALUES "
                 + "('" + txtBillCode.Text.Trim() + "', '" + txtBillNo.Text.Trim() + "','" + strDate + "','" + txtStockFrom.Text.Trim() + "','" + txtStockTo.Text.Trim() + "', "
                 + " '" + txtRemark.Text.Trim() + "', '" + strStockType + "','" + strSourceBillNo + "', " + strSourceDate + " ,'" + Convert.ToDouble(lblQty.Text) + "'," + dNetAmt + ",'" + txtWayBillNo.Text + "','" + txtWayBillDate.Text + "','" + MainPage.strLoginName + "','',1,0, '"+txtTransportName.Text.Trim()+"', '"+txtLRNumber.Text.Trim()+ "', " + strLRDate + ", '" + txtBookingStation.Text.Trim() + "' ); ";

                strQuery += " Declare @BillCode Varchar(50),@BillNo Bigint,@BarcodingType varchar(20)='UNIQUE_BARCODE'" ;

                double dSaleMargin = 0, dSaleMRP = 0, dDisPer = 0, dSaleDis = 0, dSaleRate = 0, dCompanyMargin = 0, dCompanyMRP = 0;
                int _index = 1;
                if (MainPage._bPurchaseBillWiseMargin)
                    dSaleMargin = MainPage.dPurchaseBillMargin;
                else if (MainPage._bFixedMargin)
                    dSaleMargin = MainPage.dFixedMargin;
                string strBarCode, strCompanyCode = MainPage.strDataBaseFile,strBarCode_s,strGroupName;

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    strBarCode = Convert.ToString(rows.Cells["barCode"].Value);
                    dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dPrice = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                    dMRP = dba.ConvertObjectToDouble(rows.Cells["mrp"].Value);
                    dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);


                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin)
                        dSaleMargin = dba.ConvertObjectToDouble(rows.Cells["saleMargin"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(rows.Cells["saleMRP"].Value);
                    dDisPer = dba.ConvertObjectToDouble(rows.Cells["disPer"].Value);
                    dSaleDis = dba.ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(rows.Cells["saleRate"].Value);
                    dCompanyMargin = dba.ConvertObjectToDouble(rows.Cells["cMargin"].Value);
                    dCompanyMRP = dba.ConvertObjectToDouble(rows.Cells["cMrp"].Value);

                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);
                    strUnit = Convert.ToString(rows.Cells["unitName"].Value);
                    strBarCode_s = Convert.ToString(rows.Cells["barcode_s"].Value);

                    if (strBarCode == "")
                    {
                        if (MainPage._bBarCodeStatus)
                            strBarCode = dba.GetBarCode(txtBillNo.Text, _index, strCompanyCode);
                        else
                            strBarCode = "";

                        //if (strCompanyCode != "" && strBarCode != "")
                        //    strBarCode = strCompanyCode + "-" + strBarCode;

                        if (MainPage._bCustomPurchase && strBarCode == "")
                            strBarCode = strCompanyCode;
                    }

                    strQuery += " INSERT INTO [dbo].[StockTransferSecondary]([RemoteID],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Unit],[MRP],[Rate],[Amount],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[SDisPer],[MarginType],[SaleMargin],[CompanyMarginType],[CompanyMargin],[CompanyMRP],[SaleMRP],[SaleDis],[SaleRate],[BarCode_S]) VALUES "
                                + " ('0', '" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strItemName + "', '" + rows.Cells["Variant1"].Value + "', '" + rows.Cells["Variant2"].Value + "', "
                                + " '" + rows.Cells["Variant3"].Value + "','" + rows.Cells["Variant4"].Value + "','" + rows.Cells["Variant5"].Value + "'," + dQty + ",'" + strUnit + "', '" + dMRP + "'," + dPrice + ",  " + dAmt + ", '" + MainPage.strLoginName + "','',1,0,'" + strBarCode + "','" + rows.Cells["brandName"].Value + "','" + rows.Cells["styleName"].Value + "'," + dDisPer + ",'" + rows.Cells["marginType"].Value + "'," + dSaleMargin + ",'" + rows.Cells["cmarginType"].Value + "'," + dCompanyMargin + "," + dCompanyMRP + "," + dSaleMRP + "," + dSaleDis + "," + dSaleRate + ",'" + rows.Cells["barcode_s"].Value + "') ";

                    strQuery += "INSERT INTO StockMaster ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) "
                                + "VALUES ('STOCK" + strStockType + "','" + txtBillCode.Text + "','" + txtBillNo.Text.Trim() + "','" + strItemName + "','" + rows.Cells["variant1"].Value + "', "
                                + " '" + rows.Cells["variant2"].Value + "','" + rows.Cells["variant3"].Value + "','" + rows.Cells["variant4"].Value + "', "
                                + " '" + rows.Cells["variant5"].Value + "','" + dQty + "','" + dPrice + "','','" + MainPage.strLoginName + "','',1,0,'" + ConvertObjectToDouble(rows.Cells["mrp"].Value) + "', '" + strDate + "','" + strBarCode + "','" + rows.Cells["brandName"].Value + "','" + rows.Cells["styleName"].Value + "');";
                    if (rdbStockIn.Checked)
                    {
                        strGroupName = Convert.ToString(rows.Cells["groupname"].Value);

                        strQuery += " Select @BillCode = FChallanCode ,@BillNo = (Select (ISNULL(MAX(BillNo),0)) from Items Where BillCode=FChallanCode) from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' SELECT top 1 @BarcodingType = BarcodingType FROM CompanySetting "
                            + " IF Not Exists(SELECT * FROM Items Im WHERE IM.ItemName = '" + strItemName + "') BEGIN "
                            + " INSERT INTO [dbo].[Items] ([ItemName],[GroupName],[Date],[UnitName],[QtyRatio],[StockUnitName],[BuyerDesignName],[DisStatus],[SubGroupName],[BillCode],[BillNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BrandName],[BarcodingType]) "
                            + " VALUES ('" + strItemName + "','" + strGroupName + "','" + strDate + "','" + strUnit + "',1,'" + strUnit + "','" + rows.Cells["styleName"].Value + "',0,'PURCHASE', @BillCode, @BillNo+1,'" + MainPage.strLoginName + "','',0,0,'" + rows.Cells["brandName"].Value + "',@BarcodingType) "
                            + " END Select @BillNo = MAX(BillNo) from Items Where ItemName = '" + strItemName + "'";

                        strQuery += " IF Not Exists(SELECT * FROM ItemSecondary IMS LEFT JOIN Items Im on IMS.BillCode = IM.BillCode AND IMs.BillNo = IM.BillNo WHERE IM.ItemName = '" + strItemName + "' AND Ims.Variant1 = '" + rows.Cells["Variant1"].Value + "' AND Ims.Variant2 = '" + rows.Cells["Variant2"].Value + "') BEGIN "
                                 + " INSERT INTO[dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OpeningQty],[ActiveStatus] ,[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PurchaseRate],[Margin],[Reorder],[SaleMRP],[SaleRate],[OpeningRate],[Brand],[DesignName])"
                                 + " SELECT 0,@BillCode,@BillNo,'" + rows.Cells["variant1"].Value + "' ,'" + rows.Cells["variant2"].Value + "','" + rows.Cells["variant3"].Value + "','" + rows.Cells["variant4"].Value + "','" + rows.Cells["variant5"].Value + "',0,1,'" + strBarCode + "','" + MainPage.strLoginName + "','',0,0," + dPrice + ",0,0," + dSaleMRP + "," + dSaleRate + ",0,'" + rows.Cells["brandName"].Value + "','" + rows.Cells["styleName"].Value + "' END ";
                    }

                    if (rdbStockIn.Checked && strBarCode!="" && strBarCode_s != "")
                    {
                        strQuery += " INSERT INTO [dbo].[BarcodeDetails]([BillCode],[BillNo],[ParentBarCode],[BarCode],[NetQty],[SetQty],[LastPrintNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[InStock]) "
                                 + " SELECT '" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strBarCode + "','" + strBarCode_s + "',"+ dQty+","+ dQty+",1,'" + MainPage.strLoginName + "','',1,0,1 ";
                    }
                    _index++;
                }

                strQuery += " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('STOCKTRANSFER','" + txtBillCode.Text.Trim() + "','" + txtBillNo.Text.Trim() + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dNetAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION'); ";

                strQuery += "  End";


                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (strNetQuery != "")
                        DataBaseAccess.CreateDeleteQuery(strNetQuery);

                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";

                    AskForPrint(true);                  
                    ClearAllText();
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool AskForPrint(bool _pstatus)
        {
            string strValue, strPrintValues = "1";
            if (_pstatus)
            {
                strValue = "0";
                strPrintValues = MainPage.strNoofCopy;
                //if (strPrintValues == "")
                {
                    strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", strPrintValues, 400, 300);
                    if (strValue == "" || strValue == "0")
                    {
                        return false;
                    }
                }
                //else
                  //  strValue = strPrintValues;
            }
            else
                strValue = "1";

            int _printNo = dba.ConvertObjectToInt(strValue);
            if (_printNo > 0)
            {
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    Reporting.CryStockTransferReport objReport = new Reporting.CryStockTransferReport();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport,true,(short)_printNo);
                    else
                        objReport.PrintToPrinter(_printNo, false, 0, 0);
                }
                else
                    MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return true;
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                string strQuery = " SELECT *, Convert(varchar(100), Date, 103) As SDate, Convert(varchar(100), LRDate, 103) As LDate, Convert(varchar(100), SourceDate, 103) As SSourceDate FROM [STOCKTRANSFER] "
                                + " Where BillCode = '" + txtBillCode.Text.Trim() + "' and BillNo = '" + strSerialNo + "'; "
                                + " SELECT * FROM [STOCKTRANSFERSECONDARY]  Where BillCode = '" + txtBillCode.Text.Trim() + "' and BillNo = '" + strSerialNo + "' ORDER BY BillNo ASC; ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                DisableAllControls();
                txtReason.Text = "";
                pnlDeletionConfirmation.Visible = false;
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = "";
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtBillNo.Text = strSerialNo;

                            txtDate.Text = Convert.ToString(row["SDate"]);
                            txtStockFrom.Text = Convert.ToString(row["FromMCentre"]);
                            txtStockTo.Text = Convert.ToString(row["ToMCentre"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtTransportName.Text= Convert.ToString(row["Transport"]);
                            txtBookingStation.Text = Convert.ToString(row["Station"]);
                            txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            txtWayBillNo.Text = Convert.ToString(row["WaybillNo"]);
                            txtWayBillDate.Text = Convert.ToString(row["WayBillDate"]);
                            if (txtLRNumber.Text != "")
                                txtLRDate.Text = Convert.ToString(row["LDate"]);
                            else
                                txtLRDate.Text = txtDate.Text;

                           string StockType = Convert.ToString(row["StockType"]);
                            if (StockType == "IN")                           
                                rdbStockIn.Checked = true;                           
                            else
                            {
                                rdbStockOut.Checked = true;
                            }

                            txtSourceStSerialNo.Text = Convert.ToString(row["SourceBillNo"]);
                            txtSourceStSerialDate.Text = Convert.ToString(row["SSourceDate"]);
                            lblQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            lblAmt.Text = Convert.ToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                        }
                    }

                    BindSalesBookDetails(ds.Tables[1]);
                    //BindGSTDetailsWithControl(ds.Tables[2]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void BindSalesBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["ID"].Value = row["ID"];
                    dgrdDetails.Rows[rowIndex].Cells["Barcode"].Value = row["barcode"];
                    dgrdDetails.Rows[rowIndex].Cells["BrandName"].Value = row["brandName"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["Unit"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["saleMargin"].Value = ConvertObjectToDouble(row["SaleMargin"]);
                    dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = ConvertObjectToDouble(row["SaleMRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["marginType"].Value = row["marginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cmarginType"].Value = row["CompanyMarginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cMargin"].Value = row["CompanyMargin"];
                    dgrdDetails.Rows[rowIndex].Cells["cMrp"].Value = row["CompanyMRP"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["saleDis"].Value = row["SaleDis"];
                    dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];

                    rowIndex++;
                }
            }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
        private bool CheckBillNoAndSuggest()
        {
            bool chkStatus = true;
            try
            {
                if (txtBillNo.Text != "")
                {
                    int check = dba.CheckStockTransferAvailability(txtBillCode.Text, txtBillNo.Text, txtSourceStSerialNo.Text);
                    if (check > 0)
                    {
                        string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(BillNo)+1,1)BillNo from StockTransfer where BillCode='" + txtBillCode.Text + "' "));
                        MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Text = strBillNo;
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bill No can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnEdit.Text = "&Edit";

                    EnableAllControls();
                    ClearAllText();
                    SetSerialNo();
                    txtDate.Focus();
                    btnAdd.Text = "&Save";

                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else if (ValidateControls() && ValidateOtherValidation(false) && CheckBillNoAndSuggest())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }            
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtStockFrom.Text == "")
            {
                MessageBox.Show("Sorry ! From material centre can't be blank !!", "SUNDRY CREDITOR required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStockFrom.Focus();
                return false;
            }

            if (txtStockTo.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Sorry ! To material centre can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStockTo.Focus();
                return false;
            }

            double dQty = 0, dTotalAmt = 0;
            string strItem = "";

            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strItem = Convert.ToString(row.Cells["itemName"].Value);
                dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                dTotalAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);

                if (strItem == "" && dQty == 0 && dTotalAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }

                    if (dTotalAmt == 0)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["rate"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add();
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                dgrdDetails.Focus();
                return false;
            }
            return ValidateOtherValidation(false);
        }

        private void AddStockTransferVoucher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panalColumnSetting.Visible)
                    panalColumnSetting.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        BindNextRecord();
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        BindPreviousRecord();
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        BindFirstRecord();
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        BindLastRecord();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.E)
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                }
            }
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from StockTransfer Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            if (txtBillNo.Text != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from StockTransfer Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
                else
                    BindLastRecord();
            }
            else
                ClearAllText();
        }

        private void BindPreviousRecord()
        {
            if (txtBillNo.Text != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from StockTransfer Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void AddStockTransferVoucher_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult dar = MessageBox.Show("Are you sure you want to close? ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dar == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtStockFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH MATERIAL CENTER", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            if (strData != txtStockTo.Text.Trim())
                            {
                                //dgrdDetails.Rows.Clear();
                                txtStockFrom.Text = strData;
                                if (dgrdDetails.Rows.Count == 0)
                                    dgrdDetails.Rows.Add();
                            }
                            else
                            {
                                MessageBox.Show("You can't select same Material Center into Both Side. Please choose anothers..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtStockFrom.Clear();
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtStockTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH MATERIAL CENTER", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            if (txtStockFrom.Text.Trim() != strData)
                            {
                                //dgrdDetails.Rows.Clear();
                                txtStockTo.Text = strData;
                                if (dgrdDetails.Rows.Count == 0)
                                    dgrdDetails.Rows.Add();
                            }
                            else
                            {
                                MessageBox.Show("You can't select same Material Center into Both Side. Please choose anothers..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtStockTo.Clear();
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            if (btnAdd.Text == "&Save")
            {
                DataTable dt = dba.GetDataTable("Select BillNo from STOCKTRANSFER Where BillCode = '" + txtBillCode.Text + "' And BillNo = '" + txtBillNo.Text + "' ");
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Sorry ! Already This Serial No has found. ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            if (!_bUpdateStatus)
                return ValidateStock();
            else
                return true;

        }

        private DataTable GenerateDistinctItemName()
        {
            DataTable _dt = new DataTable();
            try
            {
                _dt.Columns.Add("ItemName", typeof(String));
                _dt.Columns.Add("Variant1", typeof(String));
                _dt.Columns.Add("Variant2", typeof(String));
                _dt.Columns.Add("Variant3", typeof(String));
                _dt.Columns.Add("Variant4", typeof(String));
                _dt.Columns.Add("Variant5", typeof(String));
                _dt.Columns.Add("BarCode", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("ItemName='" + row.Cells["itemName"].Value + "' and Variant1='" + row.Cells["variant1"].Value + "' and Variant2='" + row.Cells["variant2"].Value + "' and ISNULL(Variant3,'')='" + row.Cells["variant3"].Value + "' and ISNULL(Variant4,'')='" + row.Cells["variant4"].Value + "' and ISNULL(Variant5,'')='" + row.Cells["variant5"].Value + "' and BarCode='" + row.Cells["BarCode"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]), dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                        _rows[0]["Qty"] = dOQty + dQty;
                    }
                    else
                    {
                        DataRow _row = _dt.NewRow();
                        _row["ItemName"] = row.Cells["itemName"].Value;
                        _row["Variant1"] = row.Cells["variant1"].Value;
                        _row["Variant2"] = row.Cells["variant2"].Value;
                        _row["Variant3"] = row.Cells["variant3"].Value;
                        _row["Variant4"] = row.Cells["variant4"].Value;
                        _row["Variant5"] = row.Cells["variant5"].Value;
                        _row["BarCode"] = row.Cells["barCode"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
        }

        private bool ValidateStock()
        {
            if (rdbStockOut.Checked)
            {
                DataTable _dt = GenerateDistinctItemName();
                bool _bStatus = dba.CheckQtyAvalability(_dt, txtBillCode.Text, txtBillNo.Text, dgrdDetails, lblMsg);
                if (!_bStatus && MainPage.strUserRole.Contains("SUPERADMIN"))
                    _bStatus = true;
                return _bStatus;
            }
            else
                return true;
        }

        private void EnableAllControls()
        {
            txtWayBillDate.ReadOnly = txtWayBillNo.ReadOnly = txtBillCode.ReadOnly = txtDate.ReadOnly = txtRemark.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtTransportName.ReadOnly = txtBookingStation.ReadOnly = false;
            rdbStockIn.Enabled = rdbStockOut.Enabled = true;          
        }

        private void DisableAllControls()
        {
            txtWayBillDate.ReadOnly = txtWayBillNo.ReadOnly = txtBillCode.ReadOnly = txtDate.ReadOnly = txtStockFrom.ReadOnly = txtStockTo.ReadOnly = txtRemark.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtTransportName.ReadOnly = txtBookingStation.ReadOnly = true;
            rdbStockIn.Enabled = rdbStockOut.Enabled = false;
            lblMsg.Text = lblCreatedBy.Text = "";
        }

        private void ClearAllText()
        {
            txtWayBillDate.Text = txtWayBillNo.Text= txtStockFrom.Text = txtStockTo.Text = lblMsg.Text = txtRemark.Text = txtLRNumber.Text = txtLRDate.Text = txtTransportName.Text = txtBookingStation.Text = "";

            lblQty.Text = lblAmt.Text = "0.00";

            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            lblCreatedBy.Text = "";

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = txtSourceStSerialDate.Text= DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtSourceStSerialDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int CurrentRow = 0;
                    int IndexColmn = 0;
                    int Index;
                    if (e.KeyCode == Keys.Enter)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                        if (Index < dgrdDetails.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdDetails.ColumnCount - 6)
                        {
                            IndexColmn += 1;
                            if (!dgrdDetails.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                            }

                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            double dAmount = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "" && dAmount > 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                if (dgrdDetails.RowCount > 1)
                                {
                                    DataGridViewRow row = dgrdDetails.Rows[dgrdDetails.RowCount - 2];

                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["saleMargin"].Value = row.Cells["saleMargin"].Value;
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["disPer"].Value = row.Cells["disPer"].Value;                                   
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["marginType"].Value = row.Cells["marginType"].Value;
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["cmarginType"].Value = row.Cells["cmarginType"].Value;
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["cMargin"].Value = row.Cells["cMargin"].Value;
                                }
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[2];
                                dgrdDetails.Focus();

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barcode"];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                btnAdd.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["ID"].Value);
                        if (strID == "")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                                dgrdDetails.Enabled = true;
                            }
                            else
                            {
                                ArrangeSerialNo();
                            }
                            CalculateAllAmount();
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                DeleteOneRow(strID);
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 3)// && colIndex != 10 && colIndex != 13 && colIndex != 15 && colIndex != 18)
                            dgrdDetails.CurrentCell.Value = "";                     
                    }
                }
            }
            catch { }
        }
        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "")
            {
                try
                {
                    dValue = Convert.ToDouble(objValue);
                }
                catch
                {
                }
            }
            return dValue;
        }


        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                        e.Cancel = true;

                    else if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 ||  e.ColumnIndex == 7 || e.ColumnIndex == 8)
                    {
                        if (rdbStockOut.Checked)
                        {
                            string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "", strFrom = e.ColumnIndex == 1? "BarCode" : "ItemName";
                            bool _bChkStatus = false;

                            //_objSearch = new SearchCategory("", "ITEM_NAME_ST", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, true, _bChkStatus);
                            //_objSearch.ShowDialog();

                            _objSearch_Custom = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_RETAIL", "", "", "", "", "", "", "", Keys.Space, false, false, strFrom);
                            _objSearch_Custom.ShowDialog();
                            GetAllDesignSizeColor(_objSearch_Custom, dgrdDetails.CurrentRow.Index);
                            e.Cancel = true;
                        }
                        else
                        {
                            if(e.ColumnIndex == 2)
                            {
                                string strValue = Convert.ToString(dgrdDetails.CurrentCell.FormattedValue);
                                _objData = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                                _objData.ShowDialog();
                                dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                                if (MainPage._bBrandWiseMargin)
                                    BindBrandMargin(_objData.strSelectedData);
                                e.Cancel = true;
                            }
                            else if (e.ColumnIndex == 1 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8)
                            {
                                _objSearch = new SearchCategory("", "DESIGNNAME", "", "", "", "", "", "", Keys.Space, false, "");
                                _objSearch.ShowDialog();
                                GetAllDesignSizeColor_IN(_objSearch, dgrdDetails.CurrentRow.Index);
                                e.Cancel = true;
                            }
                        }
                       
                    }
                    else if (e.ColumnIndex == 16 || e.ColumnIndex == 19)
                    {
                        _objData = new SearchData("MARGINTYPE", "SELECT MARGIN TYPE", Keys.Space);
                        _objData.ShowDialog();
                        if (_objData.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = _objData.strSelectedData;
                            CalculateSaleMarginWithMargins(dgrdDetails.CurrentRow);
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 13)
                    {
                        if (!MainPage.strUserRole.Contains("ADMIN"))
                            e.Cancel = true;
                    }
                }
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void BindBrandMargin(string strBrandName)
        {
            try
            {
                double dValue = 0;
                if (strBrandName != "")
                {
                    string strQuery = "Select Margin from BrandMaster Where BrandName='" + strBrandName + "' ";
                    object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                    dValue = ConvertObjectToDouble(objValue);
                    if (dValue == 0)
                        dValue = MainPage.dBrandwiseMargin;
                }
                dgrdDetails.CurrentRow.Cells["saleMargin"].Value = dValue.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }


        private void CalculateSaleMarginWithMargins(DataGridViewRow rows)
        {
            try
            {
                double dMRP = 0, dSaleMargin = 0, dSaleDis = 0, dItemTaxAmt = 0, dSaleRate = 0, dComMargin = 0, dCompanyMRP = 0, dSaleMRP = 0, dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dAgentCommAmt = 0, dRoundOff = 0;
                string strMarginType = "", strCMarginType = "";
                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);

                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";

                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);

                //dItemTaxAmt = ConvertObjectToDouble(rows.Cells["gstAmt"].Value);
                //if (_bMUAfterTax)
                //    dMRP += dItemTaxAmt;

                if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin || MainPage._bDesignMasterMargin)
                {
                    dSaleMargin = ConvertObjectToDouble(rows.Cells["saleMargin"].Value);
                    if (dSaleMargin == 0)
                    {
                        if (MainPage._bItemWiseMargin)
                            dSaleMargin = MainPage.dItemwiseMargin;
                        if (MainPage._bBrandWiseMargin)
                            dSaleMargin = MainPage.dBrandwiseMargin;
                    }
                }

                if (strMarginType == "MARKUP")
                    dSaleMRP = Math.Round((dMRP * (100.00 + dSaleMargin) / 100.00), 2);
                else
                    dSaleMRP = Math.Round((dMRP / (100.00 - dSaleMargin) * 100.00), 2);

                if (_bRoundTo5)
                    dSaleMRP = dba.RoundOffNearest(dSaleMRP, 5);

                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                if (strCMarginType == "MARKUP")
                    dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                else
                    dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100.00 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

            }
            catch
            {
            }
        }

        private void GetAllDesignSizeColor_IN(SearchCategory objCategory, int rowIndex)
        {
            try
            {
                bool firstRow = false;
                if (objCategory != null)
                {
                    if (objCategory.lbSearchBox.Items.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData == "")
                        {
                            foreach (string strItem in objCategory.lbSearchBox.Items)
                            {
                                if (strItem != "ADD NEW DESIGNNAME NAME" && strItem != "ADD NEW ITEM NAME")
                                {
                                    string[] strAllItem = strItem.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                        //if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                                        //    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[0];

                                        if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                            GetPurchaseRate(dgrdDetails.Rows[rowIndex]);

                                        SetUnitName_IN(strAllItem[0], rowIndex);

                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowIndex > 0)
                                rowIndex--;
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                if (strAllItem[0] != "ADD NEW DESIGNNAME NAME" && strAllItem[0] != "ADD NEW ITEM NAME")
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                    //if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                                    //    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = strAllItem[0];

                                    if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                    if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                    if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                    if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                    if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                        GetPurchaseRate(dgrdDetails.Rows[rowIndex]);
                                    SetUnitName_IN(strAllItem[0], rowIndex);
                                }
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (btnAdd.Text == "&Save")
                        {
                            if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex== dgrdDetails.RowCount-1)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                                dgrdDetails.Focus();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void GetPurchaseRate(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0,  dSaleRate = 0, dRate = 0;
                if (row != null)
                {
                    object objDisPer = 0, objSaleRate = 0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        dSaleRate = ConvertObjectToDouble(objSaleRate);
                        row.Cells["mrp"].Value =  dMRP;
                        if (dDisPer != 0)
                            dDisPer = dDisPer * -1;
                    }
                }           

                dDisPer = Math.Abs(dDisPer);
                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;

                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                dAmt = dQty * dRate;
                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                if (MainPage._bDesignMasterMargin)
                {
                    row.Cells["saleMRP"].Value = row.Cells["saleRate"].Value = dSaleRate;
                    CalculateSaleMarginWithSaleMRP_PRate(row, dMRP, dSaleRate);
                }

            }
            catch
            {
            }
        }

        private void CalculateSaleMarginWithSaleMRP_PRate(DataGridViewRow row, double dMRP, double dSaleMRP)
        {
            try
            {
                double dSaleMargin = 0;

                if (dSaleMRP != 0 && dMRP != 0)
                    dSaleMargin = ((dSaleMRP * 100.00) / dMRP) - 100.00;
                row.Cells["saleMargin"].Value = dSaleMargin;
            }
            catch { }
        }



        private void GetAllDesignSizeColor(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {
                if (objCategory != null)
                {
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData != "")
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                string[] str = strAllItem[0].Split('.');

                                dgrdDetails.Rows[rowIndex].Cells["barcode"].Value = str[0];
                                dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = strAllItem[1];
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2];
                                if (MainPage.StrCategory1!="" && strAllItem.Length > 5)
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3];
                                if (MainPage.StrCategory2 != "" && strAllItem.Length > 6)
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4];
                                if (MainPage.StrCategory3 != "" && strAllItem.Length > 7)
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[5];
                                if (MainPage.StrCategory4 != "" && strAllItem.Length > 8)
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[6];
                                if (MainPage.StrCategory5 != "" && strAllItem.Length > 9)
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[7];

                                if (str.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = strAllItem[0];

                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = 0;
                                dgrdDetails.Rows[rowIndex].Cells["rate"].Value = 0;
                                dgrdDetails.Rows[rowIndex].Cells["amount"].Value = 0;

                                SetUnitName(strAllItem[2], rowIndex);
                            }
                        }

                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex == dgrdDetails.Rows.Count - 1)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                            int _colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                            if (_colIndex < 0 || _colIndex > 4)
                                _colIndex = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[_colIndex];
                            dgrdDetails.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetUnitName_IN(string strDesignName, int rowIndex)
        {
            if (strDesignName != "")
            {
                DataTable table = dba.GetDataTable("Select BrandName,StockUnitName UnitName,BuyerDesignName from Items IM Where ItemName='" + strDesignName + "' ");
                if (table.Rows.Count > 0)
                {
                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                        dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = table.Rows[0]["BuyerDesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void SetUnitName(string strDesignName, int rowIndex)
        {
            if (strDesignName != "")
            {
                string strMarginType, strBarCode = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["barcode"].Value), strVariant1 = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["variant1"].Value), strVariant2 = Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["variant2"].Value);
                string strQuery = "";// " Select UnitName,Rate,MRP,BuyerDesignName,DesignName from Items _IM left join(Select Top 1 ItemName,Rate,MRP,DesignName from (Select Top 1 ItemName,Rate,MRP,0 ID,DesignName from StockMaster Where BillType='PURCHASE' and ItemName='" + strDesignName + "' and Variant1='" + strVariant1 + "' and Variant2='" + strVariant2 + "' Order by BillNo desc UNION ALL Select Top 1 ItemName,Rate,MRP,1 ID,DesignName from StockMaster Where BillType='OPENING' and ItemName='" + strDesignName + "' and Variant1='" + strVariant1 + "' and Variant2='" + strVariant2 + "')_Stock Order by ID)Stock on Stock.ItemName=_Im.ItemName Where Stock.ItemName='" + strDesignName + "' ";
                strQuery += " Select 0 ID,MRP,Rate,SaleMRP,SaleRate,DesignName,UnitName,DesignName as BuyerDesignName from PurchaseBookSecondary Where BarCode='" + strBarCode+"' and ItemName ='"+strDesignName+"' and Variant1='"+ strVariant1+ "' and Variant2='" + strVariant2 + "' UNION ALL Select 1 ID,PurchaseRate as MRP,PurchaseRate as Rate,SaleMRP,SaleRate,DesignName,UnitName,DesignName as BuyerDesignName from Items _im inner join ItemSecondary _ISS on _Im.BillCode=_ISS.BillCode and _Im.BillNo=_ISS.BillNo Where Description='" + strBarCode + "' and ItemName='" + strDesignName + "' and Variant1='" + strVariant1 + "' and Variant2='" + strVariant2 + "' ";
                DataTable table = dba.GetDataTable(strQuery);             
                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];                  
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = dgrdDetails.Rows[rowIndex].Cells["amount"].Value = row["Rate"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = row["MRP"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                    dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = row["SaleMRP"];
                    dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];

                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["styleName"].Value) == "")
                    {
                        if (Convert.ToString(row["DesignName"]) != "")
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = Convert.ToString(row["DesignName"]);
                        else
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["BuyerDesignName"];
                    }                    

                    CalculateDisWithRate(dgrdDetails.Rows[rowIndex]);
                }
            }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {//
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
            }
        }
        private void DeleteOneRow(string strID)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {

                    string strQuery = " Delete from [STOCKTRANSFERSECONDARY] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and [ID]=" + strID + " ";
                    int _index = dgrdDetails.CurrentRow.Index;
                    dgrdDetails.Rows.RemoveAt(_index);
                    CalculateAllAmount();
                    // if (ValidateControls())
                    {
                        int result = UpdateRecord(strQuery);
                        if (result < 1)
                            BindRecordWithControl(txtBillNo.Text);
                        else
                        {
                            strQuery = " Delete from [STOCKTRANSFERSECONDARY] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID=" + strID + " ";

                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                                dgrdDetails.Enabled = true;
                            }
                            else
                                ArrangeSerialNo();
                        }

                        dgrdDetails.ReadOnly = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Enabled = false;
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    dgrdDetails.ReadOnly = qtyAdjustStatus;
                    txtBillNo.ReadOnly = true;
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                    if (txtSourceStSerialDate.Text != "")
                    {
                        if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                            txtSourceStSerialDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                        else
                            txtSourceStSerialDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                    }

                    txtDate.Focus();
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateRecord("");
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            BindRecordWithControl(txtBillNo.Text);
                        }
                        else
                            MessageBox.Show("Sorry ! Record not updated, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
            btnEdit.Enabled = true;
        }

        private int UpdateRecord(string strSubQuery)
        {
            int result = 0;
            try
            {
                string strDate = "", strLRDate = "NULL", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");
                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";


                double dNetAmt = ConvertObjectToDouble(lblAmt.Text);


                string strStockType = "OUT", strSourceBillNo = "", strSourceDate = "NULL";
                if (rdbStockIn.Checked)
                {
                    strStockType = "IN";
                    strSourceBillNo = txtSourceStSerialNo.Text;
                    if (txtSourceStSerialDate.Text.Length==10)
                    {
                        DateTime _sDate = dba.ConvertDateInExactFormat(txtSourceStSerialDate.Text);
                        strSourceDate = "'" + _sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                    }
                }
                else
                {
                    strSourceBillNo = "";
                    strSourceDate = "'" + strDate + "'";
                }


                string strQuery = " Update [STOCKTRANSFER] SET [BillCode]='" + txtBillCode.Text.Trim() + "',[BillNo]='" + txtBillNo.Text.Trim() + "',[Date]='" + strDate + "',[WaybillNo]='" + txtWayBillNo.Text + "',[WayBillDate]='" + txtWayBillDate.Text + "', "
                    + " [FromMCentre]='" + txtStockFrom.Text.Trim() + "',[ToMCentre]='" + txtStockTo.Text.Trim() + "',[Remark]='" + txtRemark.Text.Trim() + "', StockType='" + strStockType + "', "
                    + " [TotalQty]='" + Convert.ToDouble(lblQty.Text.Trim()) + "',[TotalAmt]='" + dNetAmt + "',SourceBillNo='" + strSourceBillNo + "', SourceDate=" + strSourceDate + ", [UpdatedBy]='" + MainPage.strLoginName + "',[Transport]='" + txtTransportName.Text.Trim() + "',[LRNumber]='" + txtLRNumber.Text.Trim() + "',[LRDate]=" + strLRDate + ",[Station]='" + txtBookingStation.Text.Trim() + "',[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text.Trim() + " ; "
                    + " Delete from StockMaster Where BillType='STOCK" + strStockType + "' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "; ";

                strQuery += " Declare @BillCode Varchar(50),@BillNo Bigint,@BarcodingType varchar(20)='UNIQUE_BARCODE'";

                string strID = "";
                double dQty = 0, dPrice = 0, dMRP = 0;

                double dSaleMargin = 0, dSaleMRP = 0, dDisPer = 0, dSaleDis = 0, dSaleRate = 0, dCompanyMargin = 0, dCompanyMRP = 0;
                if (MainPage._bPurchaseBillWiseMargin)
                    dSaleMargin = MainPage.dPurchaseBillMargin;
                else if (MainPage._bFixedMargin)
                    dSaleMargin = MainPage.dFixedMargin;
                int _index = 1;
                string strBarCode, strCompanyCode = MainPage.strDataBaseFile, strGroupName;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strBarCode = Convert.ToString(row.Cells["barCode"].Value);                  
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dMRP = ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dPrice = ConvertObjectToDouble(row.Cells["rate"].Value);
                    strID = Convert.ToString(row.Cells["ID"].Value);
                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin)
                        dSaleMargin = dba.ConvertObjectToDouble(row.Cells["saleMargin"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dSaleDis = dba.ConvertObjectToDouble(row.Cells["saleDis"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                    dCompanyMargin = dba.ConvertObjectToDouble(row.Cells["cMargin"].Value);
                    dCompanyMRP = dba.ConvertObjectToDouble(row.Cells["cMrp"].Value);

                    if (strBarCode == "")
                    {
                        if (MainPage._bBarCodeStatus)
                            strBarCode = dba.GetBarCode(txtBillNo.Text, _index,"");
                        else
                            strBarCode = "";

                        //if (strCompanyCode != "" && strBarCode != "")
                        //    strBarCode = strCompanyCode + "-" + strBarCode;

                        if (MainPage._bCustomPurchase && strBarCode == "")
                            strBarCode = strCompanyCode;
                    }

                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockTransferSecondary]([RemoteID],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Unit],[MRP],[Rate],[Amount],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BarCode],[BrandName],[DesignName],[SDisPer],[MarginType],[SaleMargin],[CompanyMarginType],[CompanyMargin],[CompanyMRP],[SaleMRP],[SaleDis],[SaleRate],[BarCode_S]) VALUES "
                               + " ('0', '" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + row.Cells["itemName"].Value + "', '" + row.Cells["Variant1"].Value + "', '" + row.Cells["Variant2"].Value + "', "
                               + " '" + row.Cells["Variant3"].Value + "','" + row.Cells["Variant4"].Value + "','" + row.Cells["Variant5"].Value + "'," + dQty + ",'" + row.Cells["unitName"].Value + "', '" + dMRP + "'," + dPrice + ",  " + ConvertObjectToDouble(row.Cells["amount"].Value) + ", '" + MainPage.strLoginName + "','',1,0,'" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "'," + dDisPer + ",'" + row.Cells["marginType"].Value + "'," + dSaleMargin + ",'" + row.Cells["cmarginType"].Value + "'," + dCompanyMargin + "," + dCompanyMRP + "," + dSaleMRP + "," + dSaleDis + "," + dSaleRate + ",'" + row.Cells["barcode_s"].Value + "'); ";

                    }
                    else
                        strQuery += " Update [dbo].[STOCKTRANSFERSECONDARY] SET [ItemName]='" + row.Cells["itemName"].Value + "', "
                            + " [Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "', "
                            + " [Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "', "
                            + " [Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ", MRP=" + dMRP + ", "
                            + " [Rate]=" + dPrice + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ", "
                            + " [Unit]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[BarCode]='" + strBarCode + "',[BrandName]='" + row.Cells["brandName"].Value + "',[UpdateStatus]=1,[DesignName]='" + row.Cells["styleName"].Value + "',[SDisPer]=" + dDisPer + ",[MarginType]='" + row.Cells["marginType"].Value + "',[SaleMargin]=" + dSaleMargin + ",[CompanyMarginType]='" + row.Cells["cmarginType"].Value + "',[CompanyMargin]=" + dCompanyMargin + ",[CompanyMRP]=" + dCompanyMRP + ",[SaleMRP]=" + dSaleMRP + ",[SaleDis]=" + dSaleDis + ",[SaleRate]=" + dSaleRate + ",[BarCode_S]='" + row.Cells["barcode_s"].Value + "' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + "  ";

                    strQuery += "INSERT INTO StockMaster ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) "
                        + " VALUES ('STOCK" + strStockType + "','" + txtBillCode.Text + "','" + txtBillNo.Text.Trim() + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "', "
                        + " '" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "', "
                        + " '" + row.Cells["variant5"].Value + "','" + dQty + "','" + dPrice + "','','" + MainPage.strLoginName + "','',1,0,'" + ConvertObjectToDouble(row.Cells["mrp"].Value) + "','" + strDate + "','" + strBarCode + "','" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "');";

                    if (rdbStockIn.Checked)
                    {
                        strGroupName = Convert.ToString(row.Cells["groupname"].Value);
                        
                        strQuery += " Select @BillCode = FChallanCode ,@BillNo = (Select (ISNULL(MAX(BillNo),0)) from Items Where BillCode=FChallanCode) from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'  SELECT top 1 @BarcodingType = BarcodingType FROM CompanySetting "
                                + " IF Not Exists(SELECT * FROM Items Im WHERE IM.ItemName = '" + row.Cells["itemName"].Value + "') BEGIN "
                                + " INSERT INTO [dbo].[Items] ([ItemName],[GroupName],[Date],[UnitName],[QtyRatio],[StockUnitName],[BuyerDesignName],[DisStatus],[SubGroupName],[BillCode],[BillNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[BrandName],[BarcodingType]) "
                                + " VALUES ('" + row.Cells["itemName"].Value + ",'" + strGroupName + "'','" + strDate + "','" + row.Cells["unitName"].Value + "',1,'" + row.Cells["unitName"].Value + "','" + row.Cells["styleName"].Value + "',0,'PURCHASE', @BillCode, @BillNo+1,'" + MainPage.strLoginName + "','',0,0,'" + row.Cells["brandName"].Value + "',@BarcodingType) "
                                + " END Select @BillNo = MAX(BillNo) from Items Where ItemName = '" + row.Cells["itemName"].Value + "'";

                        strQuery += " IF Not Exists(SELECT * FROM ItemSecondary IMS LEFT JOIN Items Im on IMS.BillCode = IM.BillCode AND IMs.BillNo = IM.BillNo WHERE IM.ItemName = '" + row.Cells["itemName"].Value + "' AND Ims.Variant1 = '" + row.Cells["Variant1"].Value + "' AND Ims.Variant2 = '" + row.Cells["Variant2"].Value + "') BEGIN "
                                 + " INSERT INTO[dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OpeningQty],[ActiveStatus] ,[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PurchaseRate],[Margin],[Reorder],[SaleMRP],[SaleRate],[OpeningRate],[Brand],[DesignName])"
                                 + " SELECT 0,@BillCode,@BillNo,'" + row.Cells["variant1"].Value + "' ,'" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "',0,1,'" + row.Cells["barcode"].Value + "','" + MainPage.strLoginName + "','',0,0," + dPrice + ",0,0," + dSaleMRP + "," + dSaleRate + ",0,'" + row.Cells["brandName"].Value + "','" + row.Cells["styleName"].Value + "' END";
                    }
                    _index++;
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('STOCKTRANSFER','" + txtBillCode.Text + "','" + txtBillNo.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dNetAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                result = dba.ExecuteMyQuery(strQuery);
            }
            catch
            {
            }
            return result;
        }

        private void CalculateAllAmount()
        {
            try
            {

                double dFinalAmt = 0, dQty = 0, dSaleMargin = 0, dMRP, dSaleMRP, dComMargin, dCompanyMRP, dSaleDis, dSaleRate ;
                if (MainPage._bPurchaseBillWiseMargin)
                    dSaleMargin = MainPage.dPurchaseBillMargin;
                else if (MainPage._bFixedMargin)
                    dSaleMargin = MainPage.dFixedMargin;
                string strMarginType = "", strCMarginType = "";

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                    strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                    if (strMarginType == "")
                        rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                    if (strCMarginType == "")
                        rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";

                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dFinalAmt += ConvertObjectToDouble(rows.Cells["amount"].Value);

                    if (_bMUAfterDisc)
                        dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                    else
                        dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);

                    if (MainPage._bItemWiseMargin || MainPage._bBrandWiseMargin || MainPage._bDesignMasterMargin)
                    {
                        dSaleMargin = ConvertObjectToDouble(rows.Cells["saleMargin"].Value);
                        if (dSaleMargin == 0)
                        {
                            if (MainPage._bItemWiseMargin)
                                dSaleMargin = MainPage.dItemwiseMargin;
                            if (MainPage._bBrandWiseMargin)
                                dSaleMargin = MainPage.dBrandwiseMargin;
                        }
                    }

                    if (strMarginType == "MARKUP")
                        dSaleMRP = Math.Round((dMRP * (100.00 + dSaleMargin) / 100.00), 2);
                    else
                        dSaleMRP = Math.Round((dMRP / (100.00 - dSaleMargin) * 100.00), 2);

                    if (_bRoundTo5)
                        dSaleMRP = dba.RoundOffNearest(dSaleMRP, 5);

                    dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                    if (strCMarginType == "MARKUP")
                        dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                    else
                        dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                    rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                    rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                    dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                    dSaleRate = (dSaleMRP * (100.00 - dSaleDis)) / 100.00;
                    rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

                }

                lblAmt.Text = dFinalAmt.ToString("N2", MainPage.indianCurancy);
                lblQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private void rdbStockIn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdbStockIn.Checked == true)
                {
                    grpStockInOut.Visible = true;
                }
                else
                {
                    grpStockInOut.Visible = false;
                }

                //if (dgrdDetails.Rows.Count > 1)
                //{
                //    DialogResult _result = MessageBox.Show("If you change the stock type, Data entered datawill be loss.\n Are you want to continue ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //    if (_result == DialogResult.Yes)
                //    {
                //        dgrdDetails.Rows.Clear();
                //        dgrdDetails.Rows.Add(1);
                //        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                //    }
                //    else
                //        grpStockInOut.Visible = !grpStockInOut.Visible;
                //}
            }
            catch
            {
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (ValidateOtherValidation(true))
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillCode.Text != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strStockType = "STOCKOUT";
                                if (rdbStockIn.Checked)                               
                                    strStockType = "STOCKIN";
                               
                                string strQuery = " Delete from STOCKTRANSFER Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from STOCKTRANSFERSECONDARY Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from StockMaster Where BillType='" + strStockType + "' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('STOCKTRANSFER','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                                int count = dba.ExecuteMyQuery(strQuery);
                                if (count > 0)
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);

                                    MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    txtReason.Text = "";
                                    pnlDeletionConfirmation.Visible = false;
                                    BindNextRecord();
                                }
                                else
                                    MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReason.Focus();
                }
            }
            catch
            {
            }
            btnFinalDelete.Enabled = true;
        }



        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 10)
                        CalculateAmountWithQty(dgrdDetails.Rows[e.RowIndex]);
                    if (e.ColumnIndex == 13 || e.ColumnIndex == 12)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);                  
                    else if (e.ColumnIndex == 14)
                        CalculateDisWithRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 17 || e.ColumnIndex == 20)
                        CalculateSaleMarginWithMargins(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 18)
                        CalculateSaleMarginWithSaleMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 21)
                        CalculateSaleMarginWithCompanyMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 23)
                        CalculateSaleDisWithSaleRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 22)
                        CalculateSaleDisWithSaleDisc(dgrdDetails.Rows[e.RowIndex]);

                }
            }
            catch
            {
            }
        }

        private void CalculateSaleDisWithSaleRate(DataGridViewRow row)
        {
            try
            {
                double dSaleRate = 0, dSaleMRP = 0, dSaleDis = 0;
                dSaleRate = ConvertObjectToDouble(row.Cells["saleRate"].Value);
                dSaleMRP = ConvertObjectToDouble(row.Cells["saleMRP"].Value);

                if (dSaleMRP != 0 && dSaleRate != 0)
                    dSaleDis = ((dSaleMRP - dSaleRate) / dSaleMRP) * 100.00;

                row.Cells["saleDis"].Value = dSaleDis;                
            }
            catch { }
        }

        private void CalculateSaleDisWithSaleDisc(DataGridViewRow row)
        {
            try
            {
                double dSaleRate = 0, dSaleMRP = 0, dSaleDis = 0;
                dSaleDis = ConvertObjectToDouble(row.Cells["saleDis"].Value);
                dSaleMRP = ConvertObjectToDouble(row.Cells["saleMRP"].Value);

                if (dSaleMRP != 0)
                    dSaleRate = (dSaleMRP - ((dSaleDis * dSaleMRP) / 100.00));

                row.Cells["saleRate"].Value = dSaleRate;

                // CalculateAllAmount();
            }
            catch { }
        }

        private void CalculateSaleMarginWithCompanyMRP(DataGridViewRow rows)
        {
            try
            {
                string strMarginType = "", strCMarginType = "";
                double dSaleMRP = 0, dMRP = 0, dSaleMargin = 0, dComMargin = 0, dCompanyMRP = 0, dSaleDis = 0, dSaleRate = 0;

                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                dCompanyMRP = ConvertObjectToDouble(rows.Cells["cMrp"].Value);
                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);
                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";

                if (strCMarginType == "MARKUP")
                    dSaleMRP = Math.Round(((dCompanyMRP * 100.00) / (100.00 + dComMargin)), 2);
                else
                    dSaleMRP = Math.Round(((dCompanyMRP * (100.00 - dComMargin)) / 100.00), 2);

                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);

                //if (_bMUAfterTax)
                //    dMRP += ConvertObjectToDouble(rows.Cells["gstAmt"].Value);


                if (strMarginType == "MARKUP")
                    dSaleMargin = Math.Round(((dSaleMRP * 100.00 / dMRP) - 100.00), 4);
                else
                    dSaleMargin = Math.Round((100.00 - dMRP * 100 / dSaleMRP), 4);


                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
                rows.Cells["saleMargin"].Value = dSaleMargin;
                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N2", MainPage.indianCurancy);

            }
            catch { }
        }


        private void CalculateSaleMarginWithSaleMRP(DataGridViewRow rows)
        {
            try
            {
                string strMarginType = "", strCMarginType = "";
                double dSaleMRP = 0, dMRP = 0, dSaleMargin = 0, dComMargin = 0, dCompanyMRP = 0, dSaleDis = 0, dSaleRate = 0;
                strMarginType = Convert.ToString(rows.Cells["marginType"].Value);
                strCMarginType = Convert.ToString(rows.Cells["cmarginType"].Value);
                if (_bMUAfterDisc)
                    dMRP = ConvertObjectToDouble(rows.Cells["rate"].Value);
                else
                    dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);

                dSaleMRP = ConvertObjectToDouble(rows.Cells["saleMRP"].Value);

                //if (_bMUAfterTax)
                //    dMRP += ConvertObjectToDouble(rows.Cells["gstAmt"].Value);

                if (strMarginType == "")
                    rows.Cells["marginType"].Value = strMarginType = "MARKUP";
                if (strCMarginType == "")
                    rows.Cells["cmarginType"].Value = strCMarginType = "MARKUP";


                if (strMarginType == "MARKUP")
                    dSaleMargin = Math.Round(((dSaleMRP * 100.00 / dMRP) - 100.00), 4);
                else
                    dSaleMargin = Math.Round((100.00 - dMRP * 100 / dSaleMRP), 4);

                dComMargin = ConvertObjectToDouble(rows.Cells["cMargin"].Value);

                if (strCMarginType == "MARKUP")
                    dCompanyMRP = Math.Round((dSaleMRP * (100.00 + dComMargin) / 100.00), 2);
                else
                    dCompanyMRP = Math.Round((dSaleMRP / (100.00 - dComMargin) * 100.00), 2);

                rows.Cells["saleMargin"].Value = dSaleMargin.ToString("N4", MainPage.indianCurancy);
                rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N0", MainPage.indianCurancy);
                rows.Cells["cMrp"].Value = dCompanyMRP.ToString("N2", MainPage.indianCurancy);

                dSaleDis = ConvertObjectToDouble(rows.Cells["saleDis"].Value);
                dSaleRate = (dSaleMRP * (100 - dSaleDis)) / 100.00;
                rows.Cells["saleRate"].Value = dSaleRate.ToString("N0", MainPage.indianCurancy);

            }
            catch { }
        }

        private void CalculateDisWithRate(DataGridViewRow rows)
        {
            double dDisPer = 0, dRate = 0, dMRP = 0, dSaleMRP, dSaleMargin;
            string strMarginType;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);
                dSaleMRP = ConvertObjectToDouble(rows.Cells["saleMRP"].Value);
                if (dSaleMRP != 0 && dMRP == 0 && dRate == 0)
                {
                    rows.Cells["amount"].Value = rows.Cells["rate"].Value = rows.Cells["mrp"].Value = dSaleMRP;
                    rows.Cells["disPer"].Value = 0;
                }
                else { 
                    if (dMRP != 0 && dRate != 0)
                        dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                    rows.Cells["disPer"].Value = dDisPer;
                    double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dAmt = dQty * dRate;
                    rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    strMarginType = Convert.ToString(rows.Cells["marginType"].Value);

                    if (strMarginType == "MARKUP")
                        dSaleMargin = Math.Round(((dSaleMRP * 100.00 / dMRP) - 100.00), 4);
                    else
                        dSaleMargin = Math.Round((100.00 - dMRP * 100 / dSaleMRP), 4);

                    rows.Cells["saleMargin"].Value = dSaleMargin;
                }
                CalculateAllAmount();
            }
        }


        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value), dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
            dDisPer = Math.Abs(dDisPer);
            if (dDisPer != 0 && dMRP != 0)
            {
                dRate = dMRP * (100.00 - dDisPer) / 100.00;
                dRate = Math.Round(dRate, 2);
            }
            if (dRate == 0)
                dRate = dMRP;

            dAmt = dQty * dRate;
            rows.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);          
            CalculateAllAmount();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();

                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Copies = (short)MainPage.iNCopyStockTrans;
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;

                        Reporting.CryStockTransferReport objReport = new Reporting.CryStockTransferReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport,false,MainPage.iNCopyStockTrans);
                        else
                            objReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
            }
            btnPrint.Enabled = true;
        }
        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("CompanyAddress", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("SNo", typeof(String));
                myDataTable.Columns.Add("SourceMC", typeof(String));
                myDataTable.Columns.Add("TargetMC", typeof(String));
                myDataTable.Columns.Add("S.N.", typeof(String));
                myDataTable.Columns.Add("Goods", typeof(String));
                myDataTable.Columns.Add("Quantity", typeof(String));
                myDataTable.Columns.Add("Unit", typeof(String));
                myDataTable.Columns.Add("Price", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("TotalQuantity", typeof(String));
                myDataTable.Columns.Add("TotalAmount", typeof(String));
                myDataTable.Columns.Add("AmountInWord", typeof(String));
                myDataTable.Columns.Add("AuthorisedSignature", typeof(byte[]));
                myDataTable.Columns.Add("HeaderLogo", typeof(byte[]));
                myDataTable.Columns.Add("CompanyEmail", typeof(String));
                myDataTable.Columns.Add("Remarks", typeof(String));
                myDataTable.Columns.Add("CompanyGSTIN", typeof(String));
                myDataTable.Columns.Add("CompanyCIN", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();
                string strTotalAmt = objCurrency.changeCurrencyToWords(Convert.ToDouble(lblAmt.Text));

                string strQuery = "Select Top 1 CompanyName, Address As CompanyAddress, ('Ph. : ' + PhoneNo + ', Email ID : ' + EmailID + ', Website : ' + Website) As CompanyEmail, GSTNo As CompanyGSTIN,CINNumber As CompanyCIN from CompanyDetails Where Other='" + MainPage.strCompanyName + "'";
                DataTable dt = DataBaseAccess.GetDataTableRecord(strQuery);
                int _index = 1;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = "FOR " + MainPage.strCompanyName;
                    row["HeaderName"] = "STOCK TRANSFER";
                    row["HeaderImage"] = MainPage._headerImage;
                    row["CompanyAddress"] = dt.Rows[0]["CompanyAddress"];
                    row["Date"] = txtDate.Text;
                    row["SNo"] = txtBillCode.Text + " " + txtBillNo.Text;
                    row["SourceMC"] = txtStockFrom.Text;
                    row["TargetMC"] = txtStockTo.Text;
                    row["S.N."] = _index;// dr.Cells["srNo"].Value;
                    row["Goods"] = dr.Cells["itemName"].Value;

                    if (Convert.ToString(dr.Cells["Variant1"].Value) != "")
                    {
                        row["Goods"] += " " + dr.Cells["Variant1"].Value;
                    }
                    if (Convert.ToString(dr.Cells["Variant2"].Value) != "")
                    {
                        row["Goods"] += " " + dr.Cells["Variant2"].Value;
                    }
                    if (Convert.ToString(dr.Cells["Variant3"].Value) != "")
                    {
                        row["Goods"] += " " + dr.Cells["Variant3"].Value;
                    }
                    if (Convert.ToString(dr.Cells["Variant4"].Value) != "")
                    {
                        row["Goods"] += " " + dr.Cells["Variant4"].Value;
                    }
                    if (Convert.ToString(dr.Cells["Variant5"].Value) != "")
                    {
                        row["Goods"] += " " + dr.Cells["Variant5"].Value;
                    }

                    row["Quantity"] = dr.Cells["qty"].Value;
                    row["Unit"] = dr.Cells["unitName"].Value;
                    row["Price"] = dr.Cells["rate"].Value;
                    row["Amount"] = dr.Cells["amount"].Value;
                    row["TotalQuantity"] = lblQty.Text;
                    row["TotalAmount"] = lblAmt.Text;
                    row["AmountInWord"] = strTotalAmt;
                    row["AuthorisedSignature"] = MainPage._signatureImage;
                    row["HeaderLogo"] = MainPage._brandLogo;
                    row["CompanyEmail"] = dt.Rows[0]["CompanyEmail"];
                    row["Remarks"] = txtRemark.Text;
                    row["CompanyGSTIN"] = dt.Rows[0]["CompanyGSTIN"];
                    row["CompanyCIN"] = Convert.ToString(dt.Rows[0]["CompanyCIN"]);
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);
                    _index++;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();

                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        Reporting.CryStockTransferReport objReport = new Reporting.CryStockTransferReport();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("STOCK TRANSFER PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();
                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnCreatePDF.Enabled = false;
                    DataTable dt = CreateDataTable();
                    string strFileName = "", strPath = "";

                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        Reporting.CryStockTransferReport objReport = new Reporting.CryStockTransferReport();
                        objReport.SetDataSource(dt);

                        SaveFileDialog _browser = new SaveFileDialog();
                        _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                        strFileName = _browser.FileName = txtBillNo.Text + ".pdf";
                        _browser.ShowDialog();
                     
                        if (_browser.FileName != "")
                            strPath = _browser.FileName;
                        if (strPath != "")
                        {
                            if (System.IO.File.Exists(strPath))
                                System.IO.File.Delete(strPath);
                            objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, _browser.FileName);

                            MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("STOCKTRANSFER", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtStockCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STOCKCODE", "SEARCH STOCK TR. CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBillCode.Text = objSearch.strSelectedData;
                       
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSourceStSerialDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtSourceStSerialDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtLRDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, true);
        }

        private void txtTransportName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransportName.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = !panalColumnSetting.Visible;
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panalColumnSetting.Visible = false;
        }

        private void btnBarCodePrint_Click(object sender, EventArgs e)
        {
            btnBarCodePrint.Enabled = false;
            try
            {

                BarCode_Printing objBarCode = new BarCode_Printing("","", txtBillCode.Text, txtBillNo.Text, txtDate.Text, dgrdDetails);
                objBarCode.MdiParent = MainPage.mymainObject;
                objBarCode.Show();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bar code in Purchase Book Retail", ex.Message };
                dba.CreateErrorReports(strReport);
            }

            btnBarCodePrint.Enabled = true;
        }

        private void txtBookingStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBookingStation.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void rdbStockIn_Click(object sender, EventArgs e)
        {

        }

        private void btnWayBillNo_Click(object sender, EventArgs e)
        {
            btnWayBillNo.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "" && !txtTransportName.Text.Contains("BY HAND"))
                {
                    if (txtTransportName.Text != "")
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want generate JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";

                                var _success = dba.GenerateEWayBillJSON(strBillNo, "STOCKTRANSFER");
                                if (_success)
                                {
                                    DialogResult _result = MessageBox.Show("Are you want to open eway bill site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                        System.Diagnostics.Process.Start("https://ewaybillgst.gov.in/BillGeneration/BulkUploadEwayBill.aspx");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Transport Name can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTransportName.Focus();
                    }
                }
            }
            catch { }
            btnWayBillNo.Enabled = true;
        }

        private void btnPrintWayBill_Click(object sender, EventArgs e)
        {
            btnPrintWayBill.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        if (txtWayBillNo.Text != "" && txtWayBillDate.Text != "")
                        {
                            if (txtWayBillDate.Text.Length == 19)
                            {
                                DataTable _dt = dba.CreateStockTransferWayBillDataTable(txtBillCode.Text, txtBillNo.Text);
                                if (_dt.Rows.Count > 0)
                                {
                                    Reporting.WayBillReport objReport = new Reporting.WayBillReport();
                                    objReport.SetDataSource(_dt);

                                    if (MainPage._PrintWithDialog)
                                        dba.PrintWithDialog(objReport);
                                    else
                                    {
                                        Reporting.ShowReport objShow = new Reporting.ShowReport("WAY BILL PREVIEW");
                                        objShow.myPreview.ReportSource = objReport;
                                        objShow.myPreview.ShowPrintButton = true;
                                        objShow.myPreview.ShowExportButton = true;
                                        objShow.ShowDialog();
                                    }

                                    objReport.Close();
                                    objReport.Dispose();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please enter valid way bill date (dd/MM/yyyy hh:mm tt).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtWayBillDate.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Way bill no. and Way bill date can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtWayBillNo.Focus();
                        }
                    }
                }
            }
            catch(Exception ex) { }
            btnPrintWayBill.Enabled = true;
        }

        private void txtWayBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtWayBillDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex > 11)
                    {
                        if (_objSearch_Custom != null)
                            _objSearch_Custom.Close();
                    }
                }
                else
                {
                    if (_objSearch_Custom != null)
                    {
                        _objSearch_Custom.txtSearch.Text = e.KeyChar.ToString().Trim();
                        _objSearch_Custom.txtSearch.SelectionStart = 1;
                    }
                }
            }
            catch { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string strData1 = chkVariant1.Checked.ToString(), strData2 = chkVariant2.Checked.ToString(), strData3 = chkRoundTo5.Checked.ToString(), strData4 = chkRoundToU5.Checked.ToString(), strData5 = chkMUAfterDisc.Checked.ToString(), strData6 = chkMuAfterTax.Checked.ToString(), strData7 = chkMarginIncludeTax.Checked.ToString(), strData8 = "0", strData9 = "0", strData10 = "0", strOther1 = "", strOther2 = "", strOther3 = "";
                int _count = dba.SavePurchaseSetup(strData1, strData2, strData3, strData4, strData5, strData6, strData7, strData8, strData9, strData10, strOther1, strOther2, strOther3);
                if (_count > 0)
                {
                    _bVariant1 = chkVariant1.Checked;
                    _bVariant2 = chkVariant2.Checked;
                    _bRoundTo5 = chkRoundTo5.Checked;
                    _bRoundToU5 = chkRoundToU5.Checked;
                    _bMUAfterDisc = chkMUAfterDisc.Checked;
                    _bMUAfterTax = chkMuAfterTax.Checked;
                    _bMarginIncludeTax = chkMarginIncludeTax.Checked;
                    panalColumnSetting.Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void txtSourceStSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchDataOther objSearch = new SearchDataOther("STOCKOUTBILLNO", "", "SEARCH STOCK BILL NO", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtSourceStSerialNo.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
                        else
                            txtSourceStSerialNo.Text = "";
                    }
                    else
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetDataFromLocal()
        {
            if (txtSourceStSerialNo.Text != "" && (btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
            {
                BindRecordWithControlWithImport();
            }
        }

        private void BindRecordWithControlWithImport()
        {
            try
            {                
                string strQuery = " SELECT *, Convert(varchar(100), Date, 103) As SDate, Convert(varchar(100), LRDate, 103) As LDate, Convert(varchar(100), SourceDate, 103) As SSourceDate FROM [STOCKTRANSFER] "
                               + " Where (BillCode+' '+CAST(BillNo as varchar))='" + txtSourceStSerialNo.Text + "'   "
                               + "  SELECT st.*,GroupName FROM [STOCKTRANSFERSECONDARY] st left join Items im on st.ItemName=im.Itemname  Where (st.BillCode+' '+CAST(st.BillNo as varchar))='" + txtSourceStSerialNo.Text + "'  ORDER BY st.ID ASC; ";

                DataSet ds = NetDBAccess.GetDataSetRecord(strQuery);
                txtReason.Text = "";
                pnlDeletionConfirmation.Visible = false;
                lblCreatedBy.Text = "";
                DataTable dt = null;
                if (ds.Tables.Count > 1)
                {
                    if (dt != null)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtSourceStSerialDate.Text = Convert.ToString(row["SDate"]);
                            txtStockFrom.Text = Convert.ToString(row["FromMCentre"]);
                            txtStockTo.Text = Convert.ToString(row["ToMCentre"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtTransportName.Text = Convert.ToString(row["Transport"]);
                            txtBookingStation.Text = Convert.ToString(row["Station"]);
                            txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            if (txtLRNumber.Text != "")
                                txtLRDate.Text = Convert.ToString(row["LDate"]);
                            else
                                txtLRDate.Text = txtDate.Text;
                        }
                    }
                    BindSalesBookDetails_Import(ds.Tables[1]);
                }
                CalculateAllAmount();
            }
            catch { }
        }


        private void BindSalesBookDetails_Import(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["ID"].Value = "";// row["ID"];
                    dgrdDetails.Rows[rowIndex].Cells["Barcode"].Value = row["barcode"];
                    dgrdDetails.Rows[rowIndex].Cells["BrandName"].Value = row["brandName"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["Unit"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["saleMargin"].Value = ConvertObjectToDouble(row["SaleMargin"]);
                    dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = ConvertObjectToDouble(row["SaleMRP"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                    dgrdDetails.Rows[rowIndex].Cells["marginType"].Value = row["marginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cmarginType"].Value = row["CompanyMarginType"];
                    dgrdDetails.Rows[rowIndex].Cells["cMargin"].Value = row["CompanyMargin"];
                    dgrdDetails.Rows[rowIndex].Cells["cMrp"].Value = row["CompanyMRP"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["saleDis"].Value = row["SaleDis"];
                    dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];
                    dgrdDetails.Rows[rowIndex].Cells["barcode_s"].Value = row["BarCode_S"];
                    dgrdDetails.Rows[rowIndex].Cells["groupname"].Value = row["GroupName"];
                    rowIndex++;
                }
            }
        }

        private void CalculateAmountWithQty(DataGridViewRow rows)
        {
            double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);
            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if ( columnIndex > 8 && (columnIndex != 15 || columnIndex != 18))
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
                else if (columnIndex == 1 || columnIndex == 9)
                {
                    TextBox txt = (TextBox)e.Control;
                    txt.CharacterCasing = CharacterCasing.Upper;
                    txt.KeyPress += new KeyPressEventHandler(txtBoxbarCode_KeyPress);
                }

            }
            catch
            { }
        }

        private void txtBoxbarCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 1 || columnIndex == 9)
                    dba.ValidateSpace(sender, e);
            }
            catch { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex > 8 && (columnIndex != 15 || columnIndex != 18))
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearAllText();
            }            
        }

    }
}
