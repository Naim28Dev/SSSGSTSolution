using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Excel;
using System.Collections;

namespace SSS
{
    public partial class ImportDataFromExcel_Purchase : Form
    {
        DataBaseAccess dba;
        DataTable _dataTable = null;
        double _dDiscountPer = 0,_dSheetDiscount_Per=0;
        public ImportDataFromExcel_Purchase()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtDateFormat.Text = "dd/MM/yyyy";
            txtDate.Text = MainPage.strCurrentDate;           
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
                lblBillCount.Text = "0";
                dgrdDetails.DataSource = null;
                rdoDirect.Checked = true;
                if (txtFilePath.Text != "")
                {
                    _dataTable = GenerateDataTable();
                    dgrdDetails.DataSource = _dataTable;

                    //dgrdDetails.Columns[0].DefaultCellStyle.Format = "MM/dd/yyyy";

                    CheckItemNameExistence();

                    DataTable _dt = _dataTable.DefaultView.ToTable(true, "BILL_NO");

                    lblBillCount.Text = _dt.Rows.Count.ToString();
                    
                    foreach (DataGridViewColumn dgvc in dgrdDetails.Columns)                   
                        dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;

                    dgrdDetails.Columns["BILL_NO"].Width = 120;
                    dgrdDetails.Columns["SALES_PARTY"].Width = 250;
                    dgrdDetails.Columns["ITEM_NAME"].Width = 150;
                    dgrdDetails.Columns["SSSItemName"].Width = 180;                

                    dgrdDetails.Columns["PackingType"].Visible = dgrdDetails.Columns["CGST"].Visible = dgrdDetails.Columns["SGST"].Visible = dgrdDetails.Columns["IGST"].Visible = dgrdDetails.Columns["Net_Amt"].Visible = false;
                  
                }
                else
                    MessageBox.Show("Sorry ! Please enter file path after than you can view the records !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnShow.Enabled = true;
        }

        private DataTable GetDataTableFromExcel()
        {           
            DataSet ds = GetDataFromExcel();
            if (ds.Tables.Count > 0)
            {
                int _rowIndex = dba.ConvertObjectToInt(txtSheetNo.Text)-1;
                _dataTable = ds.Tables[_rowIndex];

                _dataTable.Columns.Add("SSSItemName", typeof(String)).SetOrdinal(6);
            }
            return _dataTable;
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
                            excelReader.IsFirstRowAsColumnNames = true;
                            ds = excelReader.AsDataSet();
                        }
                        else
                        {
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                            excelReader.IsFirstRowAsColumnNames = true;
                            ds = excelReader.AsDataSet();
                        }
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
            if (txtSupplier.Text == "")
            {
                MessageBox.Show("Sorry ! Supplier Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplier.Focus();
                return false;
            }
            //if (txtDate.Text.Length != 10)
            //{
            //    MessageBox.Show("Sorry ! Date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtDate.Focus();
            //    return false;
            //}

            if ((rdoNormalDhara.Checked && txtNormalDhara.Text == "") || (rdoSuperNet.Checked && txtSuperNetDhara.Text == ""))
            {
                MessageBox.Show("Sorry ! Nornal dhara and Supernet dhara of this party can't be blank !!", "Dhara required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplier.Focus();
                return false;
            }


            if (txtPurchaseType.Text == "")
            {
                MessageBox.Show("Sorry ! Purchase type can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseType.Focus();
                return false;
            }

            foreach(DataGridViewRow row in dgrdDetails.Rows)
            {
                if(Convert.ToString(row.Cells["SSSItemName"].Value)=="")
                {
                    MessageBox.Show("Sorry ! Item Name can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.Focus();
                    dgrdDetails.CurrentCell = row.Cells["SSSItemName"];
                    return false;
                }
                else if (txtSalesParty.Text == "" && Convert.ToString(row.Cells["SALES_PARTY"].Value) == "")
                {
                    MessageBox.Show("Sorry ! Sundry Debtors can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                    return false;
                }
                else if (Convert.ToString(row.Cells["ORDERNO"].Value) == "")
                {
                    MessageBox.Show("Sorry ! Order No can't be blank !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                    
                    return false;
                }
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
                                ShowSaleBillDetails();
                                
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

        private void ShowSaleBillDetails()
        {
            try
            {

                DialogResult _result = MessageBox.Show("Are you want to generate sale bill right now ?", "Sale Bill Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (_result == DialogResult.Yes)
                {
                    string strQuery = " Select Top 20 (ReceiptCode+' '+CAST(ReceiptNo as varchar)) _BillNo,(SalePartyID+' '+GR.SalesParty) as SalesParty,(CASE WHEN SubSalesParty!='SELF' Then (SubPartyID+' '+SubSalesParty) else SubSalesParty end) SubParty,PackingStatus  from GoodsReceive GR CROSS APPLY (Select  * from EditTrailDetails ETD Where ETD.BillType='GOODSPURCHASE' and ETD.BillCode=GR.ReceiptCode and ETD.BillNo=GR.ReceiptNo and ETD.UpdatedBy='" + MainPage.strLoginName + "') ETD  Where SaleBill='PENDING' and EditStatus='BULKCREATION' Group by ReceiptCode,ReceiptNo,SalePartyID,SalesParty,SubPartyID,SubSalesParty,PackingStatus Order  by ReceiptNo desc ";
                    DataTable _table = dba.GetDataTable(strQuery);
                    if (_table.Rows.Count > 0)
                    {
                       // DataTable _dtSalesparty = _table.DefaultView.ToTable(true, "SalesParty", "SubParty", "PackingStatus");
                        string strPSNO = "";
                        foreach (DataRow row in _table.Rows)
                        {
                           // strPSNO = GetPurchaseSNo(_table, row);
                            SaleBook _sale = new SSS.SaleBook(true);
                            _sale._strPSalesParty = Convert.ToString(row["SalesParty"]);
                            _sale._strPSubParty = Convert.ToString(row["SubParty"]);
                            _sale._strPackingType = Convert.ToString(row["PackingStatus"]);
                            _sale.strNewAddedGRSNO = Convert.ToString(row["_BillNo"]);
                            _sale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            _sale.ShowInTaskbar = true;
                           // _sale.TopMost = true;
                            _sale.ShowDialog();
                        }
                    }
                }
                else
                    MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch { }
        }

        private string GetPurchaseSNo(DataTable _dt,DataRow row)
        {
            string strPSNo = "";
            DataRow[] _rows = _dt.Select("SalesParty='" + row["SalesParty"] + "' and SubParty='" + row["SubParty"] + "' and PackingStatus='"+row["PackingStatus"]+"' ");
            foreach(DataRow dr in _rows)
            {
                if (strPSNo != "")
                    strPSNo += "','";
                strPSNo += Convert.ToString(dr["_BillNo"]);
            }
            return strPSNo;
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
            try
            {
                if (dgrdDetails.Columns[e.ColumnIndex].Name == "SSSItemName")
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                    objSearch.ShowDialog();
                    string _strItemName = objSearch.strSelectedData;
                    if (_strItemName != "")
                    {
                        string strNewHSNCode = "", strItemName = Convert.ToString(dgrdDetails.CurrentRow.Cells["ITEM_NAME"].Value), strHSNCode = Convert.ToString(dgrdDetails.CurrentRow.Cells["HSNCode"].Value);
                        if (strHSNCode == "" || !_strItemName.Contains(strHSNCode))
                            strNewHSNCode = GetHSNCodeFromItem(_strItemName);
                        else
                            strNewHSNCode = strHSNCode;

                        SetItemNameInAllRow(strItemName, _strItemName, strNewHSNCode);
                    }
                }
                else if (dgrdDetails.Columns[e.ColumnIndex].Name == "ORDERNO")
                {
                    if (txtSupplier.Text != "")
                    {
                        string[] strParty = txtSupplier.Text.Split(' ');
                        if (strParty.Length > 0)
                        {
                            SearchData objSearch = new SearchData("PENDINGORDERIMPORT", strParty[0], "SEARCH ORDER NUMBER", Keys.Space);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                strParty = objSearch.strSelectedData.Split('|');
                                if (strParty.Length > 0)
                                {
                                    SetOrderNoInAllRow(Convert.ToString(dgrdDetails.CurrentRow.Cells["BILL_NO"].Value), strParty[0], strParty[2], strParty[3],strParty[4]);
                                }
                            }
                        }
                    }
                    e.Cancel = true;
                }
                else if (dgrdDetails.Columns[e.ColumnIndex].Name == "SALES_PARTY")
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", Keys.Space);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                    {
                        bool _blackListed = false;
                        if (dba.CheckTransactionLockWithBlackList(strData, ref _blackListed))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                           
                        }
                        else if (_blackListed)
                        {
                            MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                           
                        }
                        else                                            
                            SetOrderNoInAllRow(Convert.ToString(dgrdDetails.CurrentRow.Cells["BILL_NO"].Value), Convert.ToString(dgrdDetails.CurrentRow.Cells["ORDERNO"].Value),strData,"SELF", Convert.ToString(dgrdDetails.CurrentRow.Cells["OrderDate"].Value));
                       
                        //if (!dba.CheckTransactionLock(strData))
                        //else
                        //{
                        //    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                            
                        //}
                    }
                    e.Cancel = true;
                }
                else if (dgrdDetails.Columns[e.ColumnIndex].Name == "SUBPARTY")
                {
                    SearchData objSearch = new SearchData("SUBPARTY", Convert.ToString(dgrdDetails.CurrentRow.Cells["SALES_PARTY"].Value), "SEARCH SUB PARTY", Keys.Space);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                    {
                        //dgrdDetails.CurrentCell.Value = strData;
                        SetOrderNoInAllRow(Convert.ToString(dgrdDetails.CurrentRow.Cells["BILL_NO"].Value), Convert.ToString(dgrdDetails.CurrentRow.Cells["ORDERNO"].Value), Convert.ToString(dgrdDetails.CurrentRow.Cells["SALES_PARTY"].Value), strData, Convert.ToString(dgrdDetails.CurrentRow.Cells["OrderDate"].Value));
                    }
                }
                e.Cancel = true;
            }
            catch { }
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
                    SearchData objSearch = new SearchData("PURCHASEPERSONALPARTY", "SEARCH SUPPLIER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        dgrdDetails.DataSource = null;
                        txtSupplier.Text = objSearch.strSelectedData;
                        bool _blackListed = false;
                        if (dba.CheckTransactionLockWithBlackList(txtSupplier.Text, ref _blackListed))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSupplier.Text = "";
                        }
                        else if (_blackListed)
                        {
                            MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSupplier.Text = "";
                        }
                        else
                            GetPartyDhara();
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

        private void GetPartyDhara()
        {
            try
            {
                string strPurchasePartyID, strQuery = "";
                if (txtSupplier.Text != "" && txtSupplier.Text != "PERSONAL")
                {
                    string[] strFullName = txtSupplier.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strQuery = "Select SM.NormalDhara,SM.SNDhara as SUPERDhara,SM.CFormApply as PremiumDhara,GSTNo,(Select Top 1 PurchaseType from GoodsReceive Where PurchasePartyID='" + strPurchasePartyID + "' Order by ReceiptNo Desc)PurchaseType  from SupplierMaster SM  Where  (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strPurchasePartyID + "' ";
                        DataTable dt = dba.GetDataTable(strQuery);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtNormalDhara.Text = Convert.ToString(row["NormalDhara"]);
                            txtSuperNetDhara.Text = Convert.ToString(row["SUPERDhara"]);
                            txtPremiumDhara.Text = Convert.ToString(row["PremiumDhara"]);
                            txtPurchaseType.Text = Convert.ToString(row["PurchaseType"]);
                            lblGSTNo.Text = "GST No : "+Convert.ToString(row["GSTNo"]);
                            rdoNormalDhara.Checked = true;
                        }
                        else
                            txtNormalDhara.Text = txtSuperNetDhara.Text =lblGSTNo.Text= "";                       
                    }
                    else
                        txtNormalDhara.Text = txtSuperNetDhara.Text = lblGSTNo.Text = "";
                }
                else
                {
                    txtNormalDhara.Text = txtSuperNetDhara.Text =  "0";
                    lblGSTNo.Text = "";
                }
            }
            catch
            { txtNormalDhara.Text = txtSuperNetDhara.Text =  ""; }
            rdoNormalDhara.Checked = true;
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
                    bool _blackListed = false;
                    if (dba.CheckTransactionLockWithBlackList(txtSupplier.Text, ref _blackListed))
                    {
                        MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSupplier.Text = "";
                    }
                    else if (_blackListed)
                    {
                        MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSupplier.Text = "";
                    }
                    else
                        GetPartyDhara();
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


                DataTable _dt = _dataTable.DefaultView.ToTable(true, "ITEM_NAME", "HSNCode");
                string strItemName = "", strHSNCode = "",strSSSItemName="",strNewHSNCode="";
                foreach (DataRow row in _dt.Rows)
                {
                    strItemName = Convert.ToString(row["ITEM_NAME"]);
                    strHSNCode = Convert.ToString(row["HSNCode"]);

                    if (strItemName != "")
                    {                       
                        strSSSItemName = CheckItemName(strItemName, strHSNCode);
                        if (strSSSItemName != "")
                        {
                            if (strHSNCode == "")
                                strNewHSNCode = GetHSNCodeFromItem(strSSSItemName);
                            else
                                strNewHSNCode = strHSNCode;
                            DataRow[] rows = _dataTable.Select("ITEM_NAME='" + strItemName + "' and ISNULL(HSNCode,'')='" + strHSNCode + "' ");
                            foreach (DataRow _row in rows)
                            {
                                _row["SSSItemName"] = strSSSItemName;
                                _row["HSNCode"] = strNewHSNCode;
                            }
                        }
                        else if (strHSNCode == "")
                        {
                            strNewHSNCode = GetHSNCodeFromItem(strSSSItemName);
                            DataRow[] rows = _dataTable.Select("ITEM_NAME='" + strItemName + "' and ISNULL(HSNCode,'')='" + strHSNCode + "' ");
                            foreach (DataRow _row in rows)
                            {
                                _row["HSNCode"] = strNewHSNCode;
                            }
                        }
                    }
                }

                btnImport.Enabled = true;
            }
            catch { }
            return true;
        }

        private string GetHSNCodeFromItem(string strItem)
        {
            string strQuery = "";
            strQuery = " Select _IGM.HSNCode from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where _IM.ItemName='" + strItem + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        //private bool CheckItemNameExistence()
        //{
        //    btnImport.Enabled = false;
        //    try
        //    {
        //        DataTable _dt = _dataTable.DefaultView.ToTable(true, "ITEM_NAME","Variant1","Variant2");
        //        string strItemName = "",strVariant1="",strVariant2="";
        //        foreach (DataRow row in _dt.Rows)
        //        {
        //            strItemName = Convert.ToString(row["ITEM_NAME"]);
        //            strVariant1= Convert.ToString(row["Variant1"]);
        //            strVariant2 = Convert.ToString(row["Variant2"]);
        //            if (strItemName != "")
        //            {
        //                if (!CheckItemName(strItemName, strVariant1, strVariant2))
        //                {
        //                    DialogResult result = MessageBox.Show("Item Name : " + strItemName + " " + strVariant1 + " " + strVariant2 + " does not exists, Are you want to create new items ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //                    if (result == DialogResult.Yes)
        //                    {
        //                        string strSerialNo = GetItemSerialNo(strItemName);
        //                        DesignMaster objDesignMaster = new DesignMaster("", strSerialNo);
        //                        objDesignMaster.strItemName = strItemName;
        //                        objDesignMaster.strVariant1 = strVariant1;
        //                        objDesignMaster.strVariant2 = strVariant2;
        //                        objDesignMaster.strUnit = "PCS";
        //                        objDesignMaster.strDPurchaseRate = GetPurchaseRate(strItemName, strVariant1, strVariant2, _dataTable);

        //                        objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        //                        objDesignMaster.ShowDialog();

        //                        if (objDesignMaster.StrAddedDesignName == "")
        //                        {
        //                            MessageBox.Show("Sorry ! Item name not created yet !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                            return false;
        //                        }
        //                    }
        //                    else
        //                        return false;
        //                }
        //            }
        //        }

        //        btnImport.Enabled = true;
        //    }
        //    catch { }
        //    return true;
        //}


        private string CheckItemName(string strItemName, string strHSNCode)
        {
            try
            {
                string strMainItemName = strItemName, strReplaceItemQuery = "";
                if (strHSNCode == "")
                {
                    string strValue = System.Text.RegularExpressions.Regex.Replace(strItemName, "[^0-9.]", "");
                    if (strValue.Length == 4)
                        strHSNCode = strValue;
                    else if (strItemName.Contains("("))
                    {
                        int _index = strItemName.IndexOf('(', 0);
                        if (strItemName.Length > (_index + 4))
                            strHSNCode = strItemName.Substring((_index + 1), 4).Replace(")", "");
                    }
                }

                if (strHSNCode.Length == 2)
                    strHSNCode = strHSNCode + "__";
                if (strHSNCode.Length == 3)
                    strHSNCode = strHSNCode + "_";
                if (strHSNCode.Length == 0)
                    strHSNCode = strHSNCode + "____";

                strItemName = strItemName.Replace("(", "").Replace(")", "").Replace("'", "");
                string strItemsQuery = "", strPurchasePartyID = "", strReplacedItemName = "", strFirstItemName = "";
                strReplacedItemName = System.Text.RegularExpressions.Regex.Replace(strItemName, @"[\d-]", string.Empty).Trim();
                string[] strFullName = txtSupplier.Text.Split(' ');
                if (strFullName.Length > 1)
                    strPurchasePartyID = strFullName[0].Trim();
                //strReplacedItemName = strReplacedItemName.Replace("'", "");

                string[] strPartItem = strReplacedItemName.Split(',');
                if (strPartItem.Length == 1)
                    strPartItem = strReplacedItemName.Split('-');
                if (strPartItem.Length == 1)
                    strPartItem = strReplacedItemName.Split(' ');


                if (strPartItem.Length > 1)
                {
                    strFirstItemName = strPartItem[0];
                    if (strFirstItemName.Length == 1)
                        strFirstItemName = strPartItem[1];
                    if (strFirstItemName.Length < 2 && strPartItem.Length > 2)
                        strFirstItemName = strPartItem[2];
                    strItemsQuery = " UNION ALL Select ItemName,5 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('" + strFirstItemName.Replace("/", "").Trim() + "%') and ItemName Like('%" + strHSNCode + "')  UNION ALL Select ItemName,9 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('" + strFirstItemName.Replace("/", "").Trim() + "%') ";
                }
                if (strReplacedItemName.Length > 2)
                {
                    strReplaceItemQuery = "UNION ALL Select ItemName,6 SerialNo from Items Where ItemName Like('%" + strReplacedItemName + "%') and ItemName Like('%" + strHSNCode + "')  UNION ALL  Select ItemName,7 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strReplacedItemName + "%')  ";
                }

                string strQuery = "Select TOP 1 * from ( Select ItemName,0 SerialNo from ItemMapping Where DesignName Like('" + strMainItemName + "') and UpdatedBy Like('" + strHSNCode + "') UNION ALL Select ItemName,1 SerialNo from ItemMapping Where DesignName Like('" + strMainItemName + "')  UNION ALL Select TOP 1 ItemName,1 SerialNo from GoodsReceive GR CROSS APPLY (Select ItemName,DesignName from GoodsReceiveDetails GRD Where GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo)GRD Where DesignName='" + strItemName + "'  and ItemName Like('%" + strHSNCode + "') and PurchasePartyID='" + strPurchasePartyID + "'   UNION ALL Select ItemName,3 SerialNo from ItemMapping Where DesignName Like('%" + strItemName + "%') and DesignName Like('%" + strHSNCode + "%') and UpdatedBy Like('" + strHSNCode + "') UNION ALL "
                            + " Select ItemName,4 SerialNo from Items Where SubGroupName='PURCHASE' and  ItemName Like('%" + strItemName + "%') and ItemName Like('%" + strHSNCode + "') " + strItemsQuery + "  " + strReplaceItemQuery + " UNION ALL Select ItemName,8 SerialNo from Items Where SubGroupName='PURCHASE' and  (ItemName Like('%" + strItemName + "%') and ItemName Like('%" + strHSNCode + "%'))  "
                            + " )_Sale Order By SerialNo ";

                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                return Convert.ToString(objValue);
            }
            catch { }
            return "";
        }

        private string GetPackingStatus(ref string strPcsType)
        {
            if (rdoDirect.Checked)
            {
                strPcsType = "PETI";
                return "DIRECT";
            }
            else if (rdoPacked.Checked)
            {
                strPcsType = "LOOSE";
                return "PACKED";
            }
            else if (rdoCameOffice.Checked)
            {
                strPcsType = "PETI";
                return "CAMEOFFICE";
            }
            else
            {
                strPcsType = "PETI";
                return "SUMMARY";
            }
        }

        private int GenerateQueryForSaving()
        {
            int count = 0;
            try
            {
                DateTime strDate = DateTime.Now;
                if (txtDate.Text.Length==10)
                    strDate = dba.ConvertDateInExactFormat(txtDate.Text);

                string strPurchaseParty = "", strPurchasePartyID = "", strSalePartyID = "", strSubPartyID = "", strSaleParty = "", strSubParty = "";
                string[] strFullName = txtSupplier.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplier.Text.Replace(strPurchasePartyID + " ", "");
                }
                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strSalePartyID = strFullName[0].Trim();
                        strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                    }
                }
                if (txtSubParty.Text != "")
                {
                    strFullName = txtSubParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strSubPartyID = strFullName[0].Trim();
                        strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                    }
                }

                SetValueFromGridViewToDataTable(ref _dataTable);

                DataTable _dt = _dataTable.DefaultView.ToTable(true, "BILL_NO");
                string strBillNo = "", strQuery = "", strIDate = "", strInnerQuery = "", strMainQuery = "", strTaxQuery = "", strROSign = "", strROAmt = "", strItemName = "", strAllItemName = "", strItemDesc = "", strSalesPartyName = "", strSubPartyName = "", strOtherSign = "", strOrderNo = "", strExcelSalesParty = "", strExcelSubPrty = "", strNCode = "",strOrderDate="";
                double dNetRate = 0, dQty = 0, dTQty = 0, dMRP = 0, dAmt = 0, dTAmt = 0, dDisPer = 0, dSPDesPer = 0, dPackingAmt = 0, dFreight = 0, dTaxFree = 0, dTaxPer = 0, dTaxAmt = 0, dOtherCharges = 0, dOtherAmt = 0, dGrossAmt = 0, dNetAmt = 0, dDisAmt = 0, dSPDesAmt = 0, dFileDisPer = 0, _dPAmt=0,_dFAmt=0,_dTFAmt=0;
                string strPackingType = "", strPcsType = "", strDharaType = "NORMAL", _strItemName = "";
                strPackingType = GetPackingStatus(ref strPcsType);

                if (rdoSuperNet.Checked)
                {
                    strDharaType = "SUPER";
                    dDisPer = dba.ConvertObjectToDouble(txtSuperNetDhara.Text);
                }
                else if (rdoPremium.Checked)
                {
                    strDharaType = "PREMIUM";
                    dDisPer = dba.ConvertObjectToDouble(txtPremiumDhara.Text);
                }
                else
                    dDisPer = dba.ConvertObjectToDouble(txtNormalDhara.Text);


                foreach (DataRow _row in _dt.Rows)
                {
                    strBillNo = Convert.ToString(_row["BILL_NO"]);
                    if (strBillNo != "")
                    {
                        strAllItemName = strOrderNo = "";
                        DataRow[] _rows = _dataTable.Select("BILL_NO='" + strBillNo + "' ");
                        if (_rows.Length > 0)
                        {
                            strIDate = Convert.ToString(_rows[0]["BILL_DATE"]);

                            DateTime _iDate = DateTime.Now,dOrderDate=DateTime.Now;

                            if (ConvertDateTime(ref _iDate, strIDate))
                            {
                                if (txtDate.Text.Length != 10)
                                    strDate = _iDate;

                                strIDate = "'" + _iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                                dAmt = dNetRate = dTQty = dQty = dDisAmt = dSPDesAmt = dPackingAmt = dFreight = dTaxFree = dSPDesPer = dTaxPer = dTaxAmt = dTAmt = dOtherAmt = dOtherCharges = dGrossAmt = dNetAmt = 0;
                                strExcelSalesParty = strExcelSubPrty = strNCode = "";

                                _dPAmt= dPackingAmt = dba.ConvertObjectToDouble(_rows[0]["PACKING"]);
                                _dFAmt =  dFreight = dba.ConvertObjectToDouble(_rows[0]["FREIGHT"]);
                                _dTFAmt= dTaxFree = dba.ConvertObjectToDouble(_rows[0]["TaxFree"]);
                                dOtherCharges = dba.ConvertObjectToDouble(_rows[0]["OtherAmt"]);
                            
                                strOrderNo = Convert.ToString(_rows[0]["ORDERNO"]).Trim();
                                strOrderDate= Convert.ToString(_rows[0]["OrderDate"]).Trim();
                                if (strOrderDate.Length==10)
                                    dOrderDate = dba.ConvertDateInExactFormat(strOrderDate);

                                strExcelSalesParty = Convert.ToString(_rows[0]["SALES_PARTY"]).Trim();
                                strExcelSubPrty = Convert.ToString(_rows[0]["SUBPARTY"]).Trim();
                                foreach (DataRow row in _rows)
                                {
                                    dMRP = dba.ConvertObjectToDouble(row["MRP"]);

                                    dSPDesPer = Math.Round(dba.ConvertObjectToDouble(row["Special_Dis"]), 0);
                                    dTQty += dQty = dba.ConvertObjectToDouble(row["Qty"]);

                                    dNetRate = dMRP * (100 - dSPDesPer) / 100;
                                    dSPDesAmt += dQty * (dMRP * dSPDesPer) / 100;

                                    dTAmt += dAmt = Math.Round(dQty * dNetRate, 2);

                                    if (dFileDisPer == 0)
                                        dFileDisPer = Math.Round(dba.ConvertObjectToDouble(row["Dis"]), 0);
                                    if (dFileDisPer > 0)
                                    {
                                        if (dFileDisPer != dDisPer)
                                        {
                                            MessageBox.Show("Sorry ! Discount in sheet and discount in master doesn't match !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            break;
                                        }
                                    }

                                    //if (dPackingAmt==0)
                                    // dPackingAmt = dba.ConvertObjectToDouble(row["PACKING"]);
                                    //if (dFreight == 0)
                                    //     dFreight = dba.ConvertObjectToDouble(row["FREIGHT"]);
                                    //if (dTaxFree == 0)
                                    //    dTaxFree = dba.ConvertObjectToDouble(row["TaxFree"]);
                                    //if (dOtherCharges == 0)
                                    //    dOtherCharges = dba.ConvertObjectToDouble(row["OtherAmt"]);
                                    //if(strOrderNo=="")
                                    //    strOrderNo = Convert.ToString(row["ORDERNO"]).Trim();
                                    //if (strExcelSalesParty == "")
                                    //    strExcelSalesParty = Convert.ToString(row["SALES_PARTY"]).Trim();
                                    //if (strExcelSubPrty == "")
                                    //    strExcelSubPrty = Convert.ToString(row["SUBPARTY"]).Trim();

                                    strItemName = Convert.ToString(row["SSSItemName"]);
                                    strItemDesc = Convert.ToString(row["Item_Desc"]);
                                    _strItemName = Convert.ToString(row["ITEM_NAME"]);

                                    if (strAllItemName.Length < 200)
                                    {
                                        if (strAllItemName != "")
                                            strAllItemName += ",";
                                        strAllItemName += strItemName;
                                    }
                                    if (strItemDesc == "")
                                        strItemDesc = strItemName;
                                    else if (!strItemDesc.Contains(_strItemName))
                                        strItemDesc = _strItemName + " " + strItemDesc;

                                    //if (strInnerQuery == "")
                                    //    dTaxAmt = dTTaxAmt;
                                    //else
                                    //    dTaxAmt = 0;

                                    strInnerQuery += " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus],[Rate],[GRate],[DesignName]) VALUES "
                                                  + " (@BillCode,@BillNo,'" + strItemName + "','PETI'," + dQty + "," + dAmt + "," + _dPAmt + "," + _dFAmt + " ," + _dTFAmt + " ,1,0," + dNetRate + "," + dMRP + ",'" + strItemDesc.Trim().ToUpper() + "')  "// end ";
                                                  + " if not exists(Select ItemName from[dbo].[ItemMapping]  Where ItemName = '" + strItemName + "' and DesignName = '" + row["ITEM_NAME"] + "' and UpdatedBy = '" + row["HSNCode"] + "' ) begin "
                                                  + " INSERT INTO [dbo].[ItemMapping] ([ItemName],[DesignName],[Date],[CreatedBy],[UpdatedBy]) Values ('" + strItemName + "','" + row["ITEM_NAME"] + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','" + row["HSNCode"] + "') end";

                                    _dPAmt = _dFAmt = _dTFAmt = 0;
                                }

                                DataTable _dTaxTable = new DataTable();
                                DataRow purchaseTypeRow = null;

                                dSPDesAmt = Math.Round(dSPDesAmt, 2);
                                dOtherAmt = dPackingAmt + dFreight + dOtherCharges;

                                dDisAmt = (dTAmt * dDisPer) / 100;
                                dDisAmt = Math.Round(dDisAmt, 2);

                                GetTaxAmount(dOtherAmt, _rows, ref _dTaxTable, ref purchaseTypeRow);
                                strTaxQuery = GetTaxQuery(_dTaxTable, purchaseTypeRow, ref dTaxPer, ref dTaxAmt);

                                dGrossAmt = dTAmt + dOtherAmt - dOtherCharges;
                                if (Convert.ToBoolean(purchaseTypeRow["TaxIncluded"]))
                                    dNetAmt = dGrossAmt;
                                else
                                    dNetAmt = dGrossAmt + dTaxAmt;
                                dNetAmt += dOtherCharges - dDisAmt;
                                dNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);

                                if (txtSalesParty.Text == "")
                                {
                                    strSalePartyID = strSaleParty = strSalesPartyName =
                                    strSalesPartyName = strExcelSalesParty;// Convert.ToString(_rows[0]["SALES_PARTY"]);
                                    strFullName = strSalesPartyName.Split(' ');
                                    if (strFullName.Length > 1)
                                    {
                                        strSalePartyID = strFullName[0].Trim();
                                        strSaleParty = strSalesPartyName.Replace(strSalePartyID + " ", "");
                                    }
                                }
                                if (txtSubParty.Text == "")
                                {
                                    strSubPartyID = strSubParty = strSubPartyName = "";

                                    strSubPartyName = strExcelSubPrty;// Convert.ToString(_rows[0]["SUBPARTY"]);
                                    strFullName = strSubPartyName.Split(' ');
                                    if (strFullName.Length > 1)
                                    {
                                        strSubPartyID = strFullName[0].Trim();
                                        strSubParty = strSubPartyName.Replace(strSubPartyID + " ", "");
                                    }
                                    else
                                        strSubPartyID = strSubParty = "SELF";
                                }
                                if (dOtherCharges >= 0)
                                    strOtherSign = "+";
                                else
                                    strOtherSign = "-";

                                strMainQuery = "Declare @BillCode nvarchar(250), @BillNo bigint,@DisStatus nvarchar(20),@DisPer float ; "
                                        + " Select @BillCode = BillCode, @BillNo = (MAX(BillNo)+1)  from(Select BillCode, MAX(BillNo) as BillNo from PurchaseRecord Group by BillCode UNION ALL Select BillCode, MAX(BillNo)  from PurchaseBook Group by BillCode UNION ALL Select Top 1 PBillCode, 0 as BillNo from CompanySetting )_Purchase Group by BillCode ";

                                strQuery += " If not exists (Select ReceiptNo from GoodsReceive Where InvoiceNo='" + strBillNo + "' and PurchasePartyID='" + strPurchasePartyID + "') begin  if not exists (Select ReceiptCode from GoodsReceive Where ReceiptCode=@BillCode and ReceiptNo=@BillNo UNION ALL Select BillCode from PurchaseBook Where BillCode=@BillCode and BillNo=@BillNo ) begin  "
                                         + " INSERT INTO [dbo].[GoodsReceive] ([ReceiptCode],[ReceiptNo],[OrderNo],[OrderDate],[SalesParty],[SubSalesParty],[PurchaseParty],[ReceivingDate],[Pieces],[Quantity],[Amount],[Freight],[Tax],[Packing],[Item],[Personal],[SaleBill],[PackingStatus],[CreatedBy],[PrintedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Box],[Remark],[SalePartyID],[SubPartyID],[PurchasePartyID],[InvoiceNo],[InvoiceDate],[PurchaseType],[ReverseCharge],[Dhara],[GrossAmount],[OtherSign],[OtherAmount],[DisPer],[DisAmount],[TaxPer],[TaxAmount],[NetAmount],[PurchaseStatus],[SpecialDscPer],[SpecialDscAmt],[PcsRateAmt],[NoOfCase]) Values "
                                         + " (@BillCode,@BillNo,'" + strOrderNo + "','"+ dOrderDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strSaleParty + "','" + strSubParty + "','" + strPurchaseParty + "','" + strDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strPcsType + "','" + dTQty + "'," + dTAmt + ",'" + dFreight + "', '" + dTaxFree + "','" + dPackingAmt + "','" + strAllItemName + "','','PENDING',"
                                         + " '" + strPackingType + "','" + MainPage.strLoginName + "','','',1,0,0,'','" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + strBillNo + "'," + strIDate + ",'" + txtPurchaseType.Text + "','NOT APPLICABLE','" + strDharaType + "'," + dGrossAmt + ",'" + strOtherSign + "'," + dOtherCharges + "," + dDisPer + "," + dDisAmt + "," + dTaxPer + "," + dTaxAmt + "," + dNetAmt + ",1," + dSPDesPer + "," + dSPDesAmt + ",0,1)  "
                                         + strInnerQuery;


                                strQuery += " Select @DisPer = ((" + dDisPer + " * -1) + (CASE WHEN Category = 'CASH PURCHASE' then 5 else 3 end)) from SupplierMaster Where (AreaCode + AccountNo) = '" + strPurchasePartyID + "' "
                                         + " Set @DisStatus = '+'; if (@DisPer < 0) begin Set @DisStatus = '-'; end Set @DisPer = ABS(@DisPer); ";

                                strQuery += " if not exists (Select * from [PurchaseRecord] Where [BillCode]=@BillCode and [BillNo]=@BillNo ) begin "
                                         + " INSERT INTO [dbo].[PurchaseRecord] ([BillCode],[BillNo],[GRSNo],[DueDays],[SupplierName],[SaleBillNo],[SalesParty],[Pieces],[Item],[Discount],[DiscountStatus],[Amount],[Freight],[Tax],[Packing],[FreightDiscount],[TaxDiscount],[PackingDiscount],[NetDiscount],[Remark],[OtherPer],[Others],[GrossAmt],[NetAmt],[BillDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[PurchasePartyID],[TaxLedger],[TaxAmount],[TaxPer],[ReverseCharge],[Dhara],[InvoiceNo],[InvoiceDate],[CheckStatus],[CheckedBy],[PurchaseSource],[SpecialDscPer],[SpecialDscAmt],[PcsRateAmt]) VALUES "
                                         + " (@BillCode,@BillNo,@BillCode+' '+CAST(@BillNo as varchar),'30','" + strPurchaseParty + "','','" + strSaleParty + "','" + dTQty + "','" + strItemName + "',@DisPer,@DisStatus,'" + dTAmt + "'," + dFreight + "," + dTaxFree + "," + dPackingAmt + ","
                                         + " '0','0','0'," + dDisAmt + ",'','0','" + strOtherSign + Math.Abs(dOtherCharges) + "','" + dGrossAmt + "','" + dNetAmt + "','" + strDate + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strPurchasePartyID + "','" + txtPurchaseType.Text + "'," + dTaxAmt + "," + dTaxPer + ",'','" + strDharaType + "','" + strBillNo + "'," + strIDate + ",0,'','DIRECT'," + dSPDesPer + "," + dSPDesAmt + ",0) "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                         + " ('" + strDate + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dNetAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "') end "
                                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                          + "('GOODSPURCHASE',@BillCode,@BillNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dNetAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION') ";

                                if (strOrderNo != "")
                                {
                                    string[] _StrOrderNo = strOrderNo.Split(' ');
                                    if (_StrOrderNo.Length > 2)
                                        strNCode = _StrOrderNo[2];
                                    if (strPcsType == "PETI" && MainPage.strBranchCode.Contains("DL") && dOrderDate>Convert.ToDateTime("09/13/2019"))
                                        dTQty = 1;

                                    strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + dTQty + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dTQty + "), UpdateStatus=1 where OrderNo=" + _StrOrderNo[1] + " and OrderCode='" + _StrOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                                }

                                strMainQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                                if (dTaxAmt > 0 && txtPurchaseType.Text != "")
                                {
                                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                             + " if (@SGSTName != '') begin Select @SGSTFullName=Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                             + " else if (@IGSTName != '') begin Select @IGSTFullName=Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' '+CAST(@BillNo as varchar),'" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end ";
                                }

                                strMainQuery += strQuery + strTaxQuery + " end end end ";

                                count += dba.ExecuteMyQuery(strMainQuery);

                                strMainQuery = strQuery = strTaxQuery = strInnerQuery = "";
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return count;
        }

        private void GetTaxAmount(double dOtherAmt, DataRow[] _rows, ref DataTable _dTable, ref DataRow _dPurchaseTypeRow)
        {        
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
                            double dDiscStatus = 0;

                            double dMRP = 0, dNetRate=0, dPacking = 0, dQty = 0, dAmt = 0, dSPDesPer=0;
                           // dDiscStatus = Math.Round(dba.ConvertObjectToDouble(_rows[0]["Dis"]), 0);

                            if (rdoSuperNet.Checked)                          
                                dDiscStatus = dba.ConvertObjectToDouble(txtSuperNetDhara.Text);
                            else if (rdoPremium.Checked)
                                dDiscStatus = dba.ConvertObjectToDouble(txtPremiumDhara.Text);
                            else 
                                dDiscStatus = dba.ConvertObjectToDouble(txtNormalDhara.Text);

                            foreach (DataRow rows in _rows)
                            {
                                dMRP = dba.ConvertObjectToDouble(rows["MRP"]);
                                dQty = dba.ConvertObjectToDouble(rows["Qty"]);                              
                                strItemName = Convert.ToString(rows["SSSItemName"]);
                                dSPDesPer = Math.Round(dba.ConvertObjectToDouble(rows["Special_Dis"]), 0);
                                dQty = dba.ConvertObjectToDouble(rows["QTY"]);

                                dNetRate = dMRP * (100 - dSPDesPer) / 100;

                                dAmt = dQty * dNetRate;

                                if (dNetRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    //strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode,"+dQty+" as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),2)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dRate + ">0  ";
                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDiscStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dNetRate + " * 100) / (100 + TaxRate)) else " + dNetRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDiscStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dNetRate + "* 100) / (100 + TaxRate)) else " + dNetRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDiscStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dNetRate + ">0  ";
                                }
                            }

                            dPacking += dOtherAmt;
                            if (dPacking != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,0 as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                                strQuery += strSubQuery;

                                _dTable = dba.GetDataTable(strQuery);                             
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
        }

        //private void GetTaxAmount(double dOtherAmt, DataRow[] _rows ,ref DataTable _dTable,ref DataRow _dPurchaseTypeRow)
        //{
        //    //double dTaxAmt = 0, dTaxPer = 0;
        //    string _strTaxType = "";
        //    try
        //    {
        //        if (MainPage._bTaxStatus && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
        //        {
        //            DataTable _dt = dba.GetSaleTypeDetails(txtPurchaseType.Text, "PURCHASE");
        //            if (_dt.Rows.Count > 0)
        //            {
        //                _dPurchaseTypeRow = _dt.Rows[0];
        //                string strTaxationType = Convert.ToString(_dPurchaseTypeRow["TaxationType"]);
        //                _strTaxType = "EXCLUDED";
        //                if (strTaxationType == "ITEMWISE")
        //                {
        //                    if (Convert.ToBoolean(_dPurchaseTypeRow["TaxIncluded"]))
        //                        _strTaxType = "INCLUDED";

        //                    string strQuery = "", strSubQuery = "", strItemName = "";
        //                    double dDisStatus = 0;

        //                    double dRate = 0, dPacking = 0,dQty=0,dAmt=0;
        //                    foreach (DataRow rows in _rows)
        //                    {
        //                        dRate = dba.ConvertObjectToDouble(rows["Rate"]);
        //                        dQty = dba.ConvertObjectToDouble(rows["Qty"]);
        //                        dDisStatus = dba.ConvertObjectToDouble(rows["Special_Dis"]);
        //                        strItemName = Convert.ToString(rows["ITEM_NAME"]);
        //                        dAmt = dQty * dRate;

        //                        if (dRate > 0)
        //                        {
        //                            if (strQuery != "")
        //                                strQuery += " UNION ALL ";

        //                            strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode,"+dQty+" as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),2)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dRate + ">0  ";
        //                        }
        //                    }

        //                    dPacking += dOtherAmt;
        //                    if (dPacking != 0)
        //                    {
        //                        if (strQuery != "")
        //                            strQuery += " UNION ALL ";
        //                        strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,12 as TaxRate ";
        //                    }

        //                    if (strQuery != "")
        //                    {
        //                        strQuery = " Select SUM(ROUND(((Amount*TaxRate)/100.00),2)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
        //                                 + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

        //                        strQuery += strSubQuery;

        //                        _dTable = dba.GetDataTable(strQuery);
        //                        //if (dt.Rows.Count > 0)
        //                        //{
        //                        //    double dMaxRate = 0, dTTaxAmt = 0;

        //                        //    BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt);

        //                        //    dTaxAmt = dTTaxAmt;
        //                        //    dTaxPer = dMaxRate;
        //                        //}
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
        //        dba.CreateErrorReports(strReport);
        //        MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    //if (!rdoSummary.Checked)
        //    //{
        //    //    txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
        //    //    txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);
        //    //}

        //    //if (_strTaxType == "INCLUDED")
        //    //    dTaxAmt = 0;
        //    //return dTaxAmt;
        //}


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
                                        + " ('PURCHASE',@BillCode,@BillNo,'" + strTaxAccountID + "','" + (dTaxRate / 2) + "'," + Math.Round((dTaxAmt / 2),4) + ",'" + strRegion + "','',1) ";

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
            dba.GetDateInExactFormat(sender,false, true, true);
            
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SALE PARTY ACCOUNT", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSubParty.Text = "SELF";
                    if (objSearch.strSelectedData != "")
                    {
                        txtSalesParty.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtSalesParty.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSalesParty.Text = "";
                        }
                        
                    }
                    else
                        txtSalesParty.Text = "";
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

        private void btnSalesParty_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SALE PARTY ACCOUNT", Keys.Space);
                objSearch.ShowDialog();
                txtSubParty.Text = "SELF";
                if (objSearch.strSelectedData != "")
                {
                    txtSalesParty.Text = objSearch.strSelectedData;
                    if (dba.CheckTransactionLock(txtSalesParty.Text))
                    {
                        MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSalesParty.Text = "";
                    }
                   
                }
                else
                    txtSalesParty.Text = "";
            }
            catch
            {
            }
        }

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtSalesParty.Text != "")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY ACCOUNT", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtSubParty.Text = objSearch.strSelectedData;
                            //if (dba.CheckTransactionLock(txtSupplier.Text))
                            //{
                            //    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    txtSupplier.Text = "";
                            //}
                        }
                        else
                            txtSubParty.Text = "SELF";
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnSubParty_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSalesParty.Text != "")
                {
                    SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY ACCOUNT", Keys.Space);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtSubParty.Text = objSearch.strSelectedData;
                        //if (dba.CheckTransactionLock(txtSupplier.Text))
                        //{
                        //    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    txtSupplier.Text = "";
                        //}
                    }
                    else
                        txtSubParty.Text = "SELF";
                }
            }
            catch
            {
            }
        }

        private void txtSupplier_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSupplier.Text);
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSubParty.Text);
        }
     

        private DataTable CreateMasterDataTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("BILL_DATE", typeof(String));
            _dt.Columns.Add("BILL_NO", typeof(String));
            _dt.Columns.Add("ORDERNO", typeof(String));
            _dt.Columns.Add("SALES_PARTY", typeof(String));
            _dt.Columns.Add("SUBPARTY", typeof(String));
            _dt.Columns.Add("ITEM_NAME", typeof(String));
            _dt.Columns.Add("SSSItemName", typeof(String));
            _dt.Columns.Add("Item_Desc", typeof(String));
            _dt.Columns.Add("HSNCode", typeof(String));
            _dt.Columns.Add("QTY", typeof(String));
            _dt.Columns.Add("MRP", typeof(String));
            _dt.Columns.Add("Dis", typeof(String));
            _dt.Columns.Add("Special_Dis", typeof(String));
            _dt.Columns.Add("PACKING", typeof(String));
            _dt.Columns.Add("FREIGHT", typeof(String));
            _dt.Columns.Add("TaxFree", typeof(String));
            _dt.Columns.Add("OtherAmt", typeof(String));
            _dt.Columns.Add("PackingType", typeof(String));
            _dt.Columns.Add("CGST", typeof(String));
            _dt.Columns.Add("SGST", typeof(String));
            _dt.Columns.Add("IGST", typeof(String));
            _dt.Columns.Add("Net_Amt", typeof(String));
            _dt.Columns.Add("OrderDate", typeof(String));
            return _dt;           	
        }


        private string GetColumnValue(DataTable dTable, DataRow row,string strDataType)
        {
            string strValue = "";
            if (strDataType == "BILL_DATE")
            {
                if (dTable.Columns.Contains("Date"))
                    strValue = Convert.ToString(row["Date"]);
                else if (dTable.Columns.Contains("Bill Date"))
                    strValue = Convert.ToString(row["Bill Date"]);
            }           
            else if (strDataType == "BILL_NO")
            {
                if (dTable.Columns.Contains("Voucher No."))// Vch No.
                    strValue = Convert.ToString(row["Voucher No."]);
                else if (dTable.Columns.Contains("Vch No."))
                    strValue = Convert.ToString(row["Vch No."]);
                else if (dTable.Columns.Contains("VOCH. NO."))
                    strValue = Convert.ToString(row["VOCH. NO."]);
                else if (dTable.Columns.Contains("Bill No."))
                    strValue = Convert.ToString(row["Bill No."]);
            }
            else if (strDataType == "ITEM_NAME")
            {
                if (dTable.Columns.Contains("Particulars"))
                    strValue = Convert.ToString(row["Particulars"]);
                else if (dTable.Columns.Contains("Particular"))
                    strValue = Convert.ToString(row["Particular"]);
                else if (dTable.Columns.Contains("Particuler"))
                    strValue = Convert.ToString(row["Particuler"]);
                else if (dTable.Columns.Contains("Item Name"))
                    strValue = Convert.ToString(row["Item Name"]);
                strValue = strValue.Replace("'", "");
            }
            else if (strDataType == "Item_DESC")
            {
                if (dTable.Columns.Contains("Narration"))
                    strValue = Convert.ToString(row["Narration"]);
                else if (dTable.Columns.Contains("Item Description"))
                    strValue = Convert.ToString(row["Item Description"]);
                strValue = strValue.Replace("'", "");
            }
            else if (strDataType == "HSNCODE")
            {
                if (dTable.Columns.Contains("HSN Code"))
                    strValue = Convert.ToString(row["HSN Code"]);
            }
            else if (strDataType.ToUpper() == "QTY")
            {
                if (dTable.Columns.Contains("Quantity"))
                    strValue = Convert.ToString(row["Quantity"]);
                else if (dTable.Columns.Contains("QTY"))
                    strValue = Convert.ToString(row["QTY"]);

                strValue = System.Text.RegularExpressions.Regex.Replace(strValue, "[^0-9.]", "");// System.Text.RegularExpressions.Regex.Match(strValue, @"\d+").Value;
            }
            else if (strDataType.ToUpper() == "MRP")
            {
                if (dTable.Columns.Contains("Rate"))
                    strValue = Convert.ToString(row["Rate"]);
                else if (dTable.Columns.Contains("Price"))
                    strValue = Convert.ToString(row["Price"]);
                string[] str = strValue.Split('.');
                strValue = System.Text.RegularExpressions.Regex.Replace(strValue, "[^0-9.]", "");// Replace(str[0], @"[^[1-9]\d*(\.\d+)?$", string.Empty); // System.Text.RegularExpressions.Regex.Match(strValue, @"\d+").Value;
                if (dTable.Columns.Contains("Price") && _dSheetDiscount_Per>0)
                {
                    double dValue = dba.ConvertObjectToDouble(strValue);
                    strValue = ((dValue * 100) / (100 - _dSheetDiscount_Per)).ToString("0.00");
                }
            }
            else if (strDataType.ToUpper() == "DIS")
            {
                if (dTable.Columns.Contains("Discount Per"))
                    strValue = Convert.ToString(row["Discount Per"]);
                if (dTable.Columns.Contains("Discount Percent"))
                    strValue = Convert.ToString(row["Discount Percent"]);
                _dSheetDiscount_Per = dba.ConvertObjectToDouble(strValue);
            }
            else if (strDataType.ToUpper() == "SPECIAL_DIS")
            {
                if (dTable.Columns.Contains("Special_Dis"))
                    strValue = Convert.ToString(row["Special_Dis"]);
            }
            else if (strDataType.ToUpper() == "PACKING")
            {
                if (dTable.Columns.Contains("Packing"))
                    strValue = Convert.ToString(row["Packing"]);
                else if (dTable.Columns.Contains("Estimated Value"))
                    strValue = Convert.ToString(row["Estimated Value"]);
            }
            else if (strDataType.ToUpper() == "FREIGHT")
            {
                if (dTable.Columns.Contains("Freight"))
                    strValue = Convert.ToString(row["Freight"]);
                else if (dTable.Columns.Contains("Other Charge"))
                    strValue = Convert.ToString(row["Other Charge"]);
            }
            else if (strDataType.ToUpper() == "TAXFREE")
            {
                if (dTable.Columns.Contains("TaxFree"))
                    strValue = Convert.ToString(row["TaxFree"]);
            }
            //else if (strDataType.ToUpper() == "OTHERAMT")
            //{
            //    if (dTable.Columns.Contains("Other Amt"))
            //        strValue = Convert.ToString(row["Other Amt"]);
            //    else if (dTable.Columns.Contains("OTHERAMT"))
            //        strValue = Convert.ToString(row["OTHERAMT"]);
            //}
            else if (strDataType.ToUpper() == "OTHERAMT")
            {
                if (dTable.Columns.Contains("Other Amt"))
                    strValue = Convert.ToString(row["Other Amt"]);
                else if (dTable.Columns.Contains("OTHERAMT"))
                    strValue = Convert.ToString(row["OTHERAMT"]);              
            }
            else if (strDataType.ToUpper() == "SALES_PARTY")
            {
                if (dTable.Columns.Contains("SALES_PARTY"))
                    strValue = Convert.ToString(row["SALES_PARTY"]);
            }
            else if (strDataType.ToUpper() == "SUBPARTY")
            {
                if (dTable.Columns.Contains("SUBPARTY"))
                    strValue = Convert.ToString(row["SUBPARTY"]);
            }
            return strValue.Trim();
        } 

        private DataTable GenerateDataTable()
        {
            DataTable _dt = CreateMasterDataTable(), dTable = GetDataTableFromExcel();
            string strVNo = "", strVDate = "", strVoucherNo = "",strNPacking="",strNFreight="",strNTaxFree="", strVoucherDate = "", strItemName = "",strItemDescription="" ,strHSNCode="",strQty="",strMRP="",strDesc="",strSpecialDisc="",strPacking="",strFreight="",strTaxFree="",strOtherAmt="",strSalesParty="",strSubParty="";

            if (rdoSuperNet.Checked)          
                _dDiscountPer = dba.ConvertObjectToDouble(txtSuperNetDhara.Text);         
            else
                _dDiscountPer = dba.ConvertObjectToDouble(txtNormalDhara.Text);
            _dSheetDiscount_Per = 0;

            foreach (DataRow row in dTable.Rows)
            {                
                strVDate = GetColumnValue(dTable, row, "BILL_DATE");
                strVNo = GetColumnValue(dTable, row, "BILL_NO"); 
                strItemName = GetColumnValue(dTable, row, "ITEM_NAME");
                strItemDescription = GetColumnValue(dTable, row, "Item_DESC");
                strHSNCode = GetColumnValue(dTable, row, "HSNCODE");
                strQty = GetColumnValue(dTable, row, "QTY");
                strDesc = GetColumnValue(dTable, row, "DIS");                
                strMRP = GetColumnValue(dTable, row, "MRP");               
                strSpecialDisc = GetColumnValue(dTable, row, "SPECIAL_DIS");
                strPacking = GetColumnValue(dTable, row, "PACKING");
                strFreight = GetColumnValue(dTable, row, "FREIGHT");
                strTaxFree = GetColumnValue(dTable, row, "TAXFREE");
                strOtherAmt = GetColumnValue(dTable, row, "OTHERAMT");
                strSalesParty = GetColumnValue(dTable, row, "SALES_PARTY");
                strSubParty = GetColumnValue(dTable, row, "SUBPARTY");
                if (strVNo != "")
                    strVoucherNo = strVNo;
                if (strVDate != "")
                    strVoucherDate = strVDate;
                if (strPacking != "")
                    strNPacking = strPacking;
                if (strFreight != "")
                    strNFreight = strFreight;
                if (strTaxFree != "")
                    strNTaxFree = strTaxFree;

                if (strItemDescription == "")
                    strItemDescription = strItemName;

                if (strItemName!="" && strQty !="" && strMRP!="" && !strItemName.ToUpper().Contains("SARAOGI"))
                {                 
                    DataRow _row = _dt.NewRow();
                    _row["BILL_DATE"] = strVoucherDate;
                    _row["BILL_NO"] = strVoucherNo;
                    _row["ITEM_NAME"] = strItemName;
                    _row["Item_DESC"] = strItemDescription;
                    _row["HSNCODE"] =strHSNCode ;
                    _row["QTY"] = strQty;
                    _row["MRP"] = strMRP;
                    _row["DIS"] = strDesc;
                    _row["SPECIAL_DIS"] =strSpecialDisc ;
                    _row["PACKING"] =strNPacking ;
                    _row["FREIGHT"] =strNFreight ;
                    _row["TAXFREE"] = strNTaxFree;
                    _row["OTHERAMT"] = strOtherAmt;
                    _row["SALES_PARTY"] = strSalesParty;
                    _row["SUBPARTY"] =strSubParty ;
                    _row["OrderDate"] = "";
                    _dt.Rows.Add(_row);
                }
            }
            return _dt;
        }

        private void rdoNormalDhara_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNormalDhara.Checked)
                dgrdDetails.DataSource = null;
        }

        private void rdoSuperNet_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSuperNet.Checked)
                dgrdDetails.DataSource = null;
        }

        private void SetValueFromGridViewToDataTable(ref DataTable _dTable)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    _dTable.Rows[row.Index]["ORDERNO"] = row.Cells["ORDERNO"].Value;
                    _dTable.Rows[row.Index]["SALES_PARTY"] = row.Cells["SALES_PARTY"].Value;
                    _dTable.Rows[row.Index]["SUBPARTY"] = row.Cells["SUBPARTY"].Value;
                    _dTable.Rows[row.Index]["SSSItemName"] = row.Cells["SSSItemName"].Value;
                    _dTable.Rows[row.Index]["HSNCode"] = row.Cells["HSNCode"].Value;
                    _dTable.Rows[row.Index]["OrderDate"] = row.Cells["OrderDate"].Value;

                }
            }
            catch { }
        }

        private void SetOrderNoInAllRow(string strBillNo, string strOrderNo, string strSalesParty, string strSubParty,string strOrderDate)
        {
            try
            {
              
                DataRow[] _rows = _dataTable.Select("BILL_NO='"+strBillNo+"' ");
                int _index = 0;
                foreach (DataRow _row in _rows )
                {
                    _index =_dataTable.Rows.IndexOf(_row);
                    dgrdDetails.Rows[_index].Cells["ORDERNO"].Value = strOrderNo;
                    dgrdDetails.Rows[_index].Cells["SALES_PARTY"].Value = strSalesParty;
                    dgrdDetails.Rows[_index].Cells["SUBPARTY"].Value = strSubParty;
                    dgrdDetails.Rows[_index].Cells["OrderDate"].Value = strOrderDate;

                }
            }
            catch { }
        }

        private void txtSheetNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtSheetNo_Leave(object sender, EventArgs e)
        {
            if (txtSheetNo.Text == "")
                txtSheetNo.Text = "1";
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgrdDetails.Columns[e.ColumnIndex].Name == "ORDERNO")
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string strSerialCode = "", strSerialNo = "";
                    dba.GetOrderSerialCodeAndSerialNo(strInvoiceNo, ref strSerialCode, ref strSerialNo);
                    if (strSerialCode != "" && strSerialNo != "")
                        ShowOrderDetails(strSerialCode, strSerialNo);
                }
            }
            catch { }
        }

        private void ShowOrderDetails(string strSerialCode, string strSerialNo)
        {
            try
            {
                OrderBooking objOrderBooking = new OrderBooking(strSerialCode, strSerialNo);
                objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objOrderBooking.ShowInTaskbar = true;
                objOrderBooking.Show();
            }
            catch { }
        }


        private void SetItemNameInAllRow(string strItemName, string strSSSItemName,string strHSNCode)
        {
            try
            {

                DataRow[] _rows = _dataTable.Select("ITEM_NAME='" + strItemName + "' ");
                int _index = 0;
                foreach (DataRow _row in _rows)
                {
                    _index = _dataTable.Rows.IndexOf(_row);
                    dgrdDetails.Rows[_index].Cells["SSSItemName"].Value = strSSSItemName;
                    dgrdDetails.Rows[_index].Cells["HSNCode"].Value = strHSNCode;
                }

                DialogResult result = MessageBox.Show("Are you want to set this item in all blank items", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow _row in dgrdDetails.Rows)
                    {
                        if (Convert.ToString(_row.Cells["SSSItemName"].Value) == "")
                        {
                            _row.Cells["SSSItemName"].Value = strSSSItemName;
                            _row.Cells["HSNCode"].Value = strHSNCode;
                        }
                    }
                }
            }
            catch { }
        }

    }
}
