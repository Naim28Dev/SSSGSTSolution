using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Excel;
using System.Collections;

namespace SSS
{
    public partial class ImportDataFromExcel_RetailPurchase : Form
    {
        DataBaseAccess dba;
        DataTable _dataTable = null;

        public ImportDataFromExcel_RetailPurchase()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtDateFormat.Text = "dd/MM/yyyy";
        }

        private void ImportDataFromExcel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
                else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                    SendKeys.Send("{TAB}");
            }
            catch { }
        }        

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                    txtFilePath.Text = _browser.FileName;
            }
            catch
            {
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            btnShow.Enabled = false;
            try
            {
                dgrdDetails.DataSource = null;
                if (txtSupplier.Text != "")
                {
                    if (txtFilePath.Text != "")
                    {
                        DataSet ds = GetDataFromExcel();
                        if (ds.Tables.Count > 0)
                        {
                            _dataTable = ds.Tables[0];
                            dgrdDetails.DataSource = _dataTable;
                            //dgrdDetails.Columns[0].DefaultCellStyle.Format = "MM/dd/yyyy";

                            CheckItemNameExistence();
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please enter file path after than you can view the records !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                    MessageBox.Show("Sorry ! Please enter template name after that you can view the records !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnShow.Enabled = true;
        }

        private void SetSerialNo(ref DataTable dt)
        {
            int _index = 1;
            foreach (DataRow row in dt.Rows)
            {
                row["Column1"] = _index;
                _index++;
            }
        }     

        private DataSet GetDataFromExcel()
        {
            DataSet ds = null;
            try
            {
                if (txtFilePath.Text != "")
                {
                    if (txtFilePath.Text.Contains(".XLS"))
                    {
                        FileStream stream = new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read);
                        IExcelDataReader excelReader = null;

                        if (txtFilePath.Text.Contains(".XLSX"))
                        {
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        excelReader.IsFirstRowAsColumnNames = true;

                        ds = excelReader.AsDataSet();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ds;
        }              

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
        private bool ValidateControl()
        {
            //if (txtDate.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Date can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtDate.Focus();
            //    return false;
            //}
            //if (txtDate.Text.Length != 10)
            //{
            //    MessageBox.Show("Sorry ! Date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtDate.Focus();
            //    return false;
            //}
            if (txtPurchaseType.Text == "")
            {
                MessageBox.Show("Sorry ! Purchase type can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseType.Focus();
                return false;
            }
            return true;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnImport.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    if (ValidateControl())
                    {
                        DialogResult reuslt = MessageBox.Show("Are you sure you want to import details !!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (DialogResult.Yes == reuslt)
                        {
                            int count = GenerateQueryForSaving();
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                dgrdDetails.DataSource = null;
                            }
                            else
                                MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                   
                }
            }
            catch (Exception ex){ MessageBox.Show("Sorry !! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnImport.Enabled = true;
        }

        private bool ConvertDateTime(ref DateTime _date,string strDate)
        {
            try
            {                
                    double dDate = dba.ConvertObjectToDouble(strDate);
                    if (dDate > 0)
                        _date = DateTime.FromOADate(dDate);
                    else
                    {                        
                        try
                        {
                            char split = '/';
                            if (strDate.Contains("-"))
                                split = '-';
                            string[] strNDate = strDate.Split(' ');
                            string[] strAllDate = strNDate[0].Split(split);
                            string strMonth = strAllDate[0], strFormat = "dd/MM/yyyy";
                            if (strMonth.Length == 1)
                                strFormat = "d/M/yyyy";

                            if (dba.ConvertObjectToInt(strMonth) == MainPage.currentDate.Month)
                            {
                                strFormat = "MM/dd/yyyy";
                                if (strMonth.Length == 1)
                                    strFormat = "M/d/yyyy";
                            }
                            if(strAllDate.Length>2)
                            {
                                if (strAllDate[2].Length == 2)
                                    strFormat = strFormat.Replace("yyyy", "yy");
                            }

                            if (strDate.Contains("-"))
                                strFormat = strFormat.Replace("/", "-");

                            if (strDate.Length > 10)
                            {
                                string strTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;                             
                                if (strDate.Contains("AM") || strDate.Contains("PM"))
                                    strFormat += " " + strTimeFormat;// " hh:mm:ss tt";//
                                else
                                {
                                    string[] strTime = strDate.Split(':');
                                    if (strTime.Length > 2)
                                        strFormat += " HH:mm:ss";
                                    else
                                        strFormat += " HH:mm";
                                }
                            }

                            _date = dba.ConvertDateInExactFormat(strDate, strFormat);
                        }
                        catch
                        {
                            _date = Convert.ToDateTime(strDate);
                        }
                    }               
                return true;
            }
            catch(Exception ex) { MessageBox.Show("Sorry !! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }        

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.F1)
            //    {
            //        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);                  
            //    }
            //}
            //catch { }
        }
               
        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
           
        }     

        private void txtDateFormat_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY ACCOUNT", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtSupplier.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtSupplier.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSupplier.Text = "";
                        }
                    }
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

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY ACCOUNT", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                {
                    txtSupplier.Text = objSearch.strSelectedData;
                    if (dba.CheckTransactionLock(txtSupplier.Text))
                    {
                        MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSupplier.Text = "";
                    }
                }
            }
            catch
            {
            }
        }

        private void txtPurchaseType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseType.Text = objSearch.strSelectedData;
                    //CalculateNetAmount();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnPurchaseType_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                    txtPurchaseType.Text = objSearch.strSelectedData;
                //CalculateNetAmount();

            }
            catch
            {
            }
        }

        private bool CheckItemNameExistence()
        {
            btnImport.Enabled = false;
            try
            {
                DataTable _dt = _dataTable.DefaultView.ToTable(true, "ITEM_NAME", "Variant1", "Variant2");
                string strItemName = "", strVariant1 = "", strVariant2 = "",strBarCode="";
                foreach (DataRow row in _dt.Rows)
                {
                    strItemName = Convert.ToString(row["ITEM_NAME"]);
                    strVariant1 = Convert.ToString(row["Variant1"]);
                    strVariant2 = Convert.ToString(row["Variant2"]);
                    if (strItemName != "")
                    {
                        string strPurchaseRate = GetPurchaseRate(strItemName, strVariant1, strVariant2, _dataTable, ref strBarCode);
                        if (!CheckItemName(strItemName, strVariant1, strVariant2))
                        {
                            string strSerialNo = GetItemSerialNo(strItemName);
                            if (strSerialNo != "")
                            {
                                int _count = SaveDesignMaster(strSerialNo, strVariant1, strVariant2, "", "", "", strPurchaseRate, strBarCode);
                                if (_count == 0)
                                {
                                    MessageBox.Show("Sorry ! Item name : " + strItemName + " " + strVariant1 + " " + strVariant2 + " not created yet !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                            else
                            {
                                DialogResult result = MessageBox.Show("Item Name : " + strItemName + " " + strVariant1 + " " + strVariant2 + " does not exists, Are you want to create new items ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    if (MainPage.bArticlewiseOpening)
                                    {
                                        ItemMaster objItemMaster = new ItemMaster("", strSerialNo, true);
                                        objItemMaster.ShowInTaskbar = true;
                                        objItemMaster.strItemName = strItemName;
                                        objItemMaster.strVariant1 = strVariant1;
                                        objItemMaster.strVariant2 = strVariant2;
                                        objItemMaster.strUnit = "PCS";
                                        objItemMaster.strDPurchaseRate = strPurchaseRate;
                                        objItemMaster._strBarCode = strBarCode;

                                        objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                        objItemMaster.ShowDialog();

                                        if (objItemMaster.StrAddedDesignName == "")
                                        {
                                            MessageBox.Show("Sorry ! Item name not created yet !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        DesignMaster objDesignMaster = new DesignMaster("", strSerialNo, true);
                                        objDesignMaster.strItemName = strItemName;
                                        objDesignMaster.strVariant1 = strVariant1;
                                        objDesignMaster.strVariant2 = strVariant2;
                                        objDesignMaster.strUnit = "PCS";
                                        objDesignMaster.strDPurchaseRate = strPurchaseRate;
                                        objDesignMaster._strBarCode = strBarCode;

                                        objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                        objDesignMaster.ShowDialog();

                                        if (objDesignMaster.StrAddedDesignName == "")
                                        {
                                            MessageBox.Show("Sorry ! Item name not created yet !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return false;
                                        }
                                    }
                                }
                                else
                                    return false;
                            }

                        }
                        else
                        {
                            double dDBPRate = GetPurchaseRateFromDB(strItemName, strVariant1, strVariant2), dRate = dba.ConvertObjectToDouble(strPurchaseRate);
                            if(dDBPRate!=dRate)
                            {
                                MessageBox.Show("Purchase Rate in Master and Purchase Rate in sheet, Both are different !! Plz match them !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }

                btnImport.Enabled = true;
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return true;
        }

        private int SaveDesignMaster(string strSerialNo, string strVariant1, string strVariant2,string strVariant3, string strVariant4, string strVariant5,string strPurchaseRate,string strBarCode)
        {
            int _count = 0;
            try
            {
                string strQuery = "Declare @BillCode varchar(250);    Select Top 1 @BillCode= FChallanCode from CompanySetting "
                               + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                               + " (0, @BillCode, "+ strSerialNo+", '', '" + strVariant1 + "', '" + strVariant2 + "', '" + strVariant3 + "', '" + strVariant4 + "', '" + strVariant5 + "', " + dba.ConvertObjectToDouble(strPurchaseRate) + ", 0, 0, 0, 0, 0, 1, '', '" + strBarCode + "', '" + MainPage.strLoginName + "', '', 1, 0) ";

                _count = dba.ExecuteMyQuery(strQuery);
            }
            catch { }
            return _count;
        }

        private string GetItemSerialNo(string strItemName)
        {
            string strQuery = " Select BillNo from Items Where ItemName='" + strItemName + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        private string GetPurchaseRate(string strItemName,string strVariant1,string strVariant2,DataTable dt,ref string strBarCode)
        {
            DataRow[] rows = dt.Select("ITEM_NAME='" + strItemName + "' and ISNULL(Variant1,'')='" + strVariant1 + "' and ISNULL(Variant2,'')='" + strVariant2 + "' ");
            if (rows.Length > 0)
            {
                if (dt.Columns.Contains("BarCode"))
                    strBarCode = Convert.ToString(rows[0]["BarCode"]);
                return Convert.ToString(rows[0]["MRP"]);
            }
            else
                return "";
        }

        private bool CheckItemName(string strItemName,string strVariant1,string strVariant2)
        {
            try
            {
                string strQuery = "Select ItemName from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo WHere ItemName='"+strItemName+"' and Variant1='"+ strVariant1+ "' and Variant2='" + strVariant2 + "' ";

               object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (Convert.ToString(objValue) != "")
                    return true;
                else
                    return false;
            }
            catch { }
            return false;
        }

        private double GetPurchaseRateFromDB(string strItemName, string strVariant1, string strVariant2)
        {
            double dRate = 0;
            try
            {
                string strQuery = " Select PurchaseRate from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo WHere ItemName='" + strItemName + "' and Variant1='" + strVariant1 + "' and Variant2='" + strVariant2 + "' ";

                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (Convert.ToString(objValue) != "")
                    dRate = dba.ConvertObjectToDouble(objValue);
            }
            catch { }
            return dRate;
        }

        private int GenerateQueryForSaving()
        {
            int count = 0;
            try
            {
                DateTime strDate = DateTime.Now;
                if (txtDate.Text.Length==10)
                    strDate = dba.ConvertDateInExactFormat(txtDate.Text);

                string strPurchaseParty = "", strPurchasePartyID = "";
                string[] strFullName = txtSupplier.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplier.Text.Replace(strPurchasePartyID + " ", "");
                }

                DataTable _dt = _dataTable.DefaultView.ToTable(true, "BILL_NO");
                string strBillNo = "", strQuery = "", strIDate = "", strMainQuery = "", strTaxQuery = "", strROSign = "", strROAmt = "";
                double dRate = 0, dQty = 0, dTQty = 0, dMRP = 0, dAmt = 0, dOtherCharges = 0, dTaxPer = 0, dTotalAmt = 0, dTaxAmt = 0, dOtherAmt = 0, dGrossAmt = 0, dRoundOff = 0, dNetAmt = 0, dDisPer = 0, dSplDisPer = 0, dSplDisAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strBillNo = Convert.ToString(row["BILL_NO"]);
                    if (strBillNo != "")
                    {
                        DataRow[] rows = _dataTable.Select("BILL_NO='" + strBillNo + "' ");
                        if (rows.Length > 0)
                        {
                            strIDate = Convert.ToString(rows[0]["BILL_DATE"]);

                            DateTime _iDate = DateTime.Now;
                            dOtherAmt = dTaxPer = dTaxAmt = dTQty = dSplDisAmt = dTotalAmt = 0;
                            if (ConvertDateTime(ref _iDate, strIDate))
                            {
                                if (txtDate.Text.Length != 10)
                                    strDate = _iDate;

                                strIDate = "'" + _iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                                foreach (DataRow _row in rows)
                                {
                                    dTQty += dQty = dba.ConvertObjectToDouble(_row["QTY"]);
                                    dRate = dba.ConvertObjectToDouble(_row["RATE"]);
                                    dMRP = dba.ConvertObjectToDouble(_row["MRP"]);
                                    dOtherAmt += dOtherCharges = dba.ConvertObjectToDouble(_row["SP_CD"]);
                                    dSplDisPer = dba.ConvertObjectToDouble(_row["Special_Dis"]);
                                    dAmt = dQty * dRate;

                                    dSplDisAmt += (dAmt * dSplDisPer) / 100;

                                    dDisPer = Math.Round((100 - ((dRate / dMRP) * 100)),0);

                                    dTotalAmt += dAmt;//= dQty * dRate;
                                    strQuery += " Select Top 1 @UnitName=UnitName from Items Where ItemName='" + _row["ITEM_NAME"] + "' "
                                             + " INSERT INTO [dbo].[PurchaseBookSecondary] ([BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Discount],[OCharges],[BasicAmt],[UnitName],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                             + " (@BillCode,@BillNo,'" + _row["ITEM_NAME"] + "','" + _row["Variant1"] + "','" + _row["Variant2"] + "','','',''," + dQty + "," + dMRP + "," + dDisPer + "," + dRate + ","
                                             + " " + dAmt + ",0," + dOtherCharges + ", " + (dAmt + dOtherCharges) + ",@UnitName,0,'" + MainPage.strLoginName + "','',1,0)";

                                    strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                                             + " ('PURCHASE',@BillCode,@BillNo, '" + _row["ITEM_NAME"] + "','" + _row["Variant1"] + "','" + _row["Variant2"] + "','','',''," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "') ";

                                }
                                DataTable _dTaxTable = new DataTable();
                                DataRow purchaseTypeRow = null;

                                GetTaxAmount(dOtherAmt, rows, ref _dTaxTable, ref purchaseTypeRow);
                                strTaxQuery = GetTaxQuery(_dTaxTable, purchaseTypeRow, ref dTaxPer, ref dTaxAmt);

                                strMainQuery = "Declare @BillCode nvarchar(250), @BillNo bigint, @UnitName nvarchar(250) ;"
                                           + " Select @BillCode = BillCode, @BillNo = (MAX(BillNo)+1)  from(Select BillCode, MAX(BillNo) as BillNo from PurchaseRecord Group by BillCode UNION ALL Select BillCode, MAX(BillNo)  from PurchaseBook Group by BillCode UNION ALL Select Top 1 PBillCode, 0 as BillNo from CompanySetting )_Purchase Group by BillCode ";

                                dGrossAmt = dTotalAmt + dOtherAmt - dSplDisAmt;
                                if (Convert.ToBoolean(purchaseTypeRow["TaxIncluded"]))
                                    dNetAmt = dGrossAmt;
                                else
                                    dNetAmt = dGrossAmt + dTaxAmt;
                                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); // Math.Round(dNetAmt, 0);
                                dRoundOff = dNNetAmt - dNetAmt;
                                if (dRoundOff >= 0)
                                {
                                    strROSign = "+";
                                    strROAmt = dRoundOff.ToString("0.00");
                                }
                                else
                                {
                                    strROSign = "-";
                                    strROAmt = Math.Abs(dRoundOff).ToString("0.00");
                                }

                                strMainQuery += " if not exists(Select ReceiptCode from GoodsReceive Where [PurchasePartyID]='" + strPurchasePartyID + "' and InvoiceNo='" + row["BILL_NO"] + "' UNION ALL Select BillCode from PurchaseBook Where [PurchasePartyID]='" + strPurchasePartyID + "' and [InvoiceNo]='" + row["BILL_NO"] + "' ) begin "
                                             + " INSERT INTO [dbo].[PurchaseBook] ([BillCode],[BillNo],[Date],[InvoiceNo],[InvoiceDate],[PurchasePartyID],[PurchaseParty],[PurchaseType],[TransportName],[GodownName],[Remark],[Description],[Other],[PackingAmt],[OtherSign],[OtherAmt],[DiscPer],[DiscAmt],[TaxPer],[TaxAmt],[TotalQty],[GrossAmt],[NetAmt],[ROSign],[RoundOff],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                             + " (@BillCode,@BillNo,'" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + row["BILL_NO"] + "'," + strIDate + ",'" + strPurchasePartyID + "','" + strPurchaseParty + "','" + txtPurchaseType.Text + "','','','','','-',0,'+',0," + dSplDisPer + "," + dSplDisAmt + "," + dTaxPer + "," + dTaxAmt + "," + dTQty + "," + dGrossAmt + "," + dNNetAmt + ",'" + strROSign + "','" + strROAmt + "','" + MainPage.strLoginName + "','',1,0)  "
                                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dNNetAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "')  ";

                                strMainQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                                if (dTaxAmt > 0 && txtPurchaseType.Text != "")
                                {
                                    strMainQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end ";
                                }

                                strMainQuery += strQuery + strTaxQuery;

                                strMainQuery += "  Update SM Set SM.BarCode=_IM.BarCode from StockMaster SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode=@BillCode and SM.BillNo=@BillNo"
                                             + " Update SM Set SM.BarCode=_IM.BarCode from PurchaseBookSecondary SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode=@BillCode and SM.BillNo=@BillNo  end end ";


                                count += dba.ExecuteMyQuery(strMainQuery);

                                strMainQuery = strQuery = strTaxQuery = "";
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Date : " + strIDate + " is invalid, Please enter correct date format ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return count;
        }

        private void GetTaxAmount(double dOtherAmt, DataRow[] _rows ,ref DataTable _dTable,ref DataRow _dPurchaseTypeRow)
        {
            //double dTaxAmt = 0, dTaxPer = 0;
            string _strTaxType = "";
            try
            {
                if (MainPage._bTaxStatus && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtPurchaseType.Text, "PURCHASE");
                    if (_dt.Rows.Count > 0)
                    {
                        _dPurchaseTypeRow = _dt.Rows[0];
                        string strTaxationType = Convert.ToString(_dPurchaseTypeRow["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(_dPurchaseTypeRow["TaxIncluded"]))
                                _strTaxType = "INCLUDED";

                            string strQuery = "", strSubQuery = "", strItemName = "";
                            double dDisStatus = 0;

                            double dRate = 0, dPacking = 0,dQty=0,dAmt=0;
                            foreach (DataRow rows in _rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows["Rate"]);
                                dQty = dba.ConvertObjectToDouble(rows["Qty"]);
                                dDisStatus = dba.ConvertObjectToDouble(rows["Special_Dis"]);
                                strItemName = Convert.ToString(rows["ITEM_NAME"]);
                                dAmt = dQty * dRate;

                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode,"+dQty+" as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dRate + ">0  ";
                                }
                            }

                            dPacking += dOtherAmt;
                            if (dPacking != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,12 as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                                strQuery += strSubQuery;

                                _dTable = dba.GetDataTable(strQuery);
                                //if (dt.Rows.Count > 0)
                                //{
                                //    double dMaxRate = 0, dTTaxAmt = 0;
                                    
                                //    BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt);

                                //    dTaxAmt = dTTaxAmt;
                                //    dTaxPer = dMaxRate;
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //if (!rdoSummary.Checked)
            //{
            //    txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            //    txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);
            //}

            //if (_strTaxType == "INCLUDED")
            //    dTaxAmt = 0;
            //return dTaxAmt;
        }

        private string GetTaxQuery(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt)
        {
            string strTaxQuery = "";
            try
            {               
                if (_dt.Rows.Count > 0)
                {                 
                  
                    string strRegion = Convert.ToString(_row["Region"]), strIGST = Convert.ToString(_row["IGSTName"]), strSGST = Convert.ToString(_row["SGSTName"]); ;
                
                    double dTaxRate = 0, dTaxAmt = 0;
                    string strTaxAccountID = "";                   
                    foreach (DataRow row in _dt.Rows)
                    {
                        dTaxRate = dba.ConvertObjectToDouble(row["TaxRate"]);
                        dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);
                        dTTaxAmt += Convert.ToDouble(dTaxAmt.ToString("0.00"));

                        if (dTaxRate > dMaxRate)
                            dMaxRate = dTaxRate;

                        strTaxAccountID = "";
                        string[] strFullName = strIGST.Split(' ');
                        if (strFullName.Length > 0)                       
                            strTaxAccountID = strFullName[0].Trim();                        
                      
                        if (strRegion == "LOCAL")
                        {
                            strTaxQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                        + " ('PURCHASE',@BillCode,@BillNo,'" + strTaxAccountID + "','" + (dTaxRate / 2) + "'," + Math.Round((dTaxAmt / 2),4) + ",'" + strRegion + "','',1) ";


                            strFullName = strSGST.Split(' ');
                            if (strFullName.Length > 0)
                                strTaxAccountID = strFullName[0].Trim();

                            strTaxQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                        + " ('PURCHASE',@BillCode,@BillNo,'" + strTaxAccountID + "','" + (dTaxRate / 2) + "'," + Math.Round((dTaxAmt / 2), 4) + ",'" + strRegion + "','',1) ";

                        }
                        else
                        {
                            strTaxQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                        + " ('PURCHASE',@BillCode,@BillNo,'" + strTaxAccountID + "','" + dTaxRate + "'," + Math.Round(dTaxAmt,4) + ",'" + strRegion + "','',1) ";                        
                        }                        
                    }
                }
            }
            catch { }
            return strTaxQuery;
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
           // if(txtDate.Text!="")
           // dba.GetStringFromDate(txtDate);
            dba.GetDateInExactFormat(sender, false, true, true);
        }
        
    }
}
