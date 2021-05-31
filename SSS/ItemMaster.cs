using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace SSS
{
    public partial class ItemMaster : Form
    {
        DataBaseAccess dba;
        public string StrAddedDesignName = "", strGroupName = "", strItemName = "", strUnit = "", strLastSerialNo = "", strCreateStatus = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "",strDPurchaseRate="0",_strBarCode="";
        string strOldDesignName="",strOldUnitName="";
        bool newDesignStatus = false;
        DataTable dtDesignName = null;

        
        private delegate void SetTextDeleg(string text);

        public ItemMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            GetStartupData();
            if (strLastSerialNo != "0" && strLastSerialNo!="")
                BindRecordWithControl(strLastSerialNo);
            BindDesignNameWithList();           
        }

        public ItemMaster(string strSerialCode, string strSerialNo, bool _bDStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            GetStartupData();
            if (strSerialCode != "")
                txtSerialCode.Text = strSerialCode;
            if (_bDStatus)
            {
                newDesignStatus = true;
                if (strSerialNo == "" || strSerialNo == "0")
                {
                    strCreateStatus = "CREATE";
                    BindRecordWithControl(strLastSerialNo);
                }
                else
                {
                    strCreateStatus = "UPDATE";
                    BindRecordWithControl(strSerialNo);
                }
            }
            else
                BindRecordWithControl(strSerialNo);

            BindDesignNameWithList();
        }

        public ItemMaster(bool nStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            newDesignStatus = nStatus;
            GetStartupData();
            SetCategory();
            BindLastRecord();
            BindDesignNameWithList();            
           
            dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text,1,"");
        }

        public ItemMaster(string strDesignName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            GetStartupData();
            BindDesignWithSelectedDesign(strDesignName);
            BindDesignNameWithList();
        }


        private void GetStartupData()
        {
            try
            {
                string strQuery = "Select FChallanCode as SerialCode,(Select ISNULL(MAX(BillNo),'') from Items Where BillCode=FChallanCode COLLATE SQL_Latin1_General_CP1_CI_AI)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select Distinct BillCode,BillNo,ItemName,BuyerDesignName from Items Order by ItemName";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0 && txtSerialCode.Text == "")
                    {
                        txtSerialCode.Text = Convert.ToString(dt.Rows[0]["SerialCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                    dtDesignName = ds.Tables[1];
                }
            }
            catch
            {
            }
        }

        //private void SetDesignSerialCode()
        //{
        //    object objValue = DataBaseAccess.ExecuteMyScalar("Select FChallanCode as SerialCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ");
        //    txtSerialCode.Text = Convert.ToString(objValue);
        //}

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["category1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["category1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category1"].Visible = false;
               
                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["category2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["category2"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category2"].Visible = false;
               
                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["category3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["category3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["category4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["category4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["category5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["category5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category5"].Visible = false;
            }
            catch
            {
            }
        }

        private void BindDesignNameWithList()
        {
            lBoxDesignName.Items.Clear();
            if (dtDesignName != null)
            {
                foreach (DataRow row in dtDesignName.Rows)
                {
                    lBoxDesignName.Items.Add(row["ItemName"]);
                }
            }
            if (lBoxDesignName.Items.Count > 0)
                lBoxDesignName.SelectedIndex = 0;
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from Items Where BillCode='" + txtSerialCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from Items Where BillCode='" + txtSerialCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from Items Where BillCode='" + txtSerialCode.Text + "' and BillNo>" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
            {
                BindLastRecord();
            }
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from Items Where BillCode='"+ txtSerialCode.Text+"' and BillNo<" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                chkPick.Checked = false;
                string strQuery = "Select *,Convert(varchar,Date,103) BDate from Items Where  BillCode='" + txtSerialCode.Text + "' and  BillNo=" + strSerialNo + "  Select *,dbo.GetFullName(PurchasePartyID) SupplierName from ItemSecondary Where BillCode='" + txtSerialCode.Text + "' and BillNo=" + strSerialNo + " order by ID ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                DisableAllControls();
                txtSerialNo.ReadOnly = false;
                //newImageStatus = false;
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtSerialNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["BDate"]);
                            txtBuyerDesignNo.Text = Convert.ToString(row["BuyerDesignName"]);
                            txtItemName.Text =strOldDesignName= Convert.ToString(row["ItemName"]);
                            txtGroupName.Text = Convert.ToString(row["GroupName"]);
                            txtPurchaseUnit.Text = Convert.ToString(row["StockUnitName"]);
                            txtUnitName.Text = strOldUnitName = Convert.ToString(row["UnitName"]);
                            txtQtyRatio.Text = Convert.ToString(row["QtyRatio"]);
                            txtDRemark.Text = Convert.ToString(row["DisRemark"]);
                            txtCategory.Text = Convert.ToString(row["Other"]);
                                                        
                            string strCodingType = Convert.ToString(row["BarcodingType"]);
                            // strCodingType = strCodingType == "" ? MainPage.strBarCodingType : Convert.ToString(row["BarcodingType"]);

                            if (strCodingType == "UNIQUE_BARCODE")
                                rdoUniqueBarCode.Checked = true;
                            else if (strCodingType == "DESIGNMASTER_WISE")
                                rdoSameBarcodeYes.Checked = true;
                            else if (strCodingType == "ITEM_WISE")
                                rdoItemwise.Checked = true;
                            else
                                rdoItemwise.Checked = rdoSameBarcodeYes.Checked = rdoUniqueBarCode.Checked = false;

                            if (dt.Columns.Contains("BrandName"))
                            {
                                txtBrandName.Text = Convert.ToString(row["BrandName"]);
                                txtDepartmentName.Text = Convert.ToString(row["MakeName"]);
                            }
                            string strItemType = Convert.ToString(row["SubGroupName"]);
                            if (strItemType == "JOURNAL")
                                rdoForJournal.Checked = true;
                            else
                                rdoForPurchase.Checked = true;

                            if (txtPurchaseUnit.Text != txtUnitName.Text)
                                txtQtyRatio.ReadOnly = false;
                            else
                                txtQtyRatio.ReadOnly = true;
                            if(Convert.ToString(row["DisStatus"])!="")
                            {
                                if (Convert.ToBoolean(row["DisStatus"]))
                                {
                                    chkDiscontinue.Checked = true;
                                    txtItemName.BackColor = Color.Red;
                                }
                                else
                                {
                                    chkDiscontinue.Checked = false;
                                    txtItemName.BackColor = Color.White;
                                }
                            }

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            lblCreatedBy.Text = "";
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                        }
                    }
                    dt.Rows.Clear();
                    dgrdDetails.Rows.Clear();
                    dt = ds.Tables[1];
                    int rowIndex = 0;
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count);
                        foreach (DataRow row in dt.Rows)
                        {
                            dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                            dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                            dgrdDetails.Rows[rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                            dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["Brand"];
                            dgrdDetails.Rows[rowIndex].Cells["purchaseRate"].Value = row["PurchaseRate"];
                            dgrdDetails.Rows[rowIndex].Cells["openingRate"].Value = ConvertObjectToDouble(row["OpeningRate"]).ToString("N2",MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["saleMRP"].Value = row["SaleMRP"];
                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["disPer"];
                            dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];
                            dgrdDetails.Rows[rowIndex].Cells["reOrder"].Value = row["Reorder"];
                            dgrdDetails.Rows[rowIndex].Cells["openingQty"].Value = row["OpeningQty"];
                            dgrdDetails.Rows[rowIndex].Cells["Chk"].Value = Convert.ToBoolean(row["ActiveStatus"]);
                            dgrdDetails.Rows[rowIndex].Cells["category1"].Value = row["Variant1"];
                            dgrdDetails.Rows[rowIndex].Cells["category2"].Value = row["Variant2"];
                            dgrdDetails.Rows[rowIndex].Cells["category3"].Value = row["Variant3"];
                            dgrdDetails.Rows[rowIndex].Cells["category4"].Value = row["Variant4"];
                            dgrdDetails.Rows[rowIndex].Cells["category5"].Value = row["Variant5"];
                            dgrdDetails.Rows[rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                            dgrdDetails.Rows[rowIndex].Cells["margin"].Value = row["Margin"];
                            dgrdDetails.Rows[rowIndex].Cells["godownName"].Value = row["GodownName"];
                            dgrdDetails.Rows[rowIndex].Cells["barCodeID"].Value = dgrdDetails.Rows[rowIndex].Cells["oldBarCode"].Value = row["Description"];
                            dgrdDetails.Rows[rowIndex].Cells["printCheck"].Value = true;
                            dgrdDetails.Rows[rowIndex].Cells["PRate"].Value = row["PurchaseRate"];
                            dgrdDetails.Rows[rowIndex].Cells["Smrp"].Value = row["SaleMRP"];
                            dgrdDetails.Rows[rowIndex].Cells["SRate"].Value = row["SaleRate"];
                            dgrdDetails.Rows[rowIndex].Cells["OQty"].Value = row["OpeningQty"];
                            dgrdDetails.Rows[rowIndex].Cells["ORate"].Value = row["OpeningRate"];

                            
                          rowIndex++;
                        }
                    }
                    else
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.Rows[0].Cells["Chk"].Value = dgrdDetails.Rows[0].Cells["printCheck"].Value = true;
                        dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text,1,"");
                    }      
                }

            }
            catch
            {
            }
        }


        private void BindRecordWithControlWithImport()
        {
            try
            {
                string strQuery = "Select *,Convert(varchar,Date,103) BDate from Items Where  ItemName='" + txtImportData.Text + "' Select _IS.*,'' as SupplierName from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where SubGroupName='PURCHASE' and ItemName='" + txtImportData.Text+"' Order by Variant1,Variant2 ";

                DataSet ds = SearchDataOther.GetDataSet(strQuery);               
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];  

                            txtBuyerDesignNo.Text = Convert.ToString(row["BuyerDesignName"]);
                            txtItemName.Text = strOldDesignName = Convert.ToString(row["ItemName"]);                          
                            txtPurchaseUnit.Text = Convert.ToString(row["StockUnitName"]);
                            txtUnitName.Text = strOldUnitName = Convert.ToString(row["UnitName"]);
                            txtQtyRatio.Text = Convert.ToString(row["QtyRatio"]);
                            txtDRemark.Text = Convert.ToString(row["DisRemark"]);
                            txtDepartmentName.Text = Convert.ToString(row["MakeName"]);
                            txtBrandName.Text = Convert.ToString(row["BrandName"]);

                            if (MainPage._bBarCodeStatus)
                            {
                                if (Convert.ToString(row["BarcodingType"]) != "")
                                {
                                    string strCodingType = Convert.ToString(row["BarcodingType"]);
                                    if (strCodingType == "UNIQUE_BARCODE")
                                        rdoUniqueBarCode.Checked = true;
                                    if (strCodingType == "DESIGNMASTER_WISE")
                                        rdoSameBarcodeYes.Checked = true;
                                    if (strCodingType == "ITEM_WISE")
                                        rdoItemwise.Checked = true;
                                }
                            }

                            string strItemType = Convert.ToString(row["SubGroupName"]);
                            if (strItemType == "JOURNAL")
                                rdoForJournal.Checked = true;
                            else
                                rdoForPurchase.Checked = true;

                            if (txtPurchaseUnit.Text != txtUnitName.Text)
                                txtQtyRatio.ReadOnly = false;
                            else
                                txtQtyRatio.ReadOnly = true;
                            if (Convert.ToString(row["DisStatus"]) != "")
                            {
                                if (Convert.ToBoolean(row["DisStatus"]))
                                {
                                    chkDiscontinue.Checked = true;
                                    txtItemName.BackColor = Color.Red;
                                }
                                else
                                {
                                    chkDiscontinue.Checked = false;
                                    txtItemName.BackColor = Color.White;
                                }
                            }                         
                        }
                    }

                    dt.Rows.Clear();
                    dgrdDetails.Rows.Clear();
                    dt = ds.Tables[1];
                    int rowIndex = 0;
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count);
                        foreach (DataRow row in dt.Rows)
                        {
                            dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                            //  dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                            dgrdDetails.Rows[rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                            dgrdDetails.Rows[rowIndex].Cells["styleName"].Value = row["DesignName"];
                            dgrdDetails.Rows[rowIndex].Cells["brandName"].Value = row["Brand"];

                            dgrdDetails.Rows[rowIndex].Cells["purchaseRate"].Value = row["PurchaseRate"];
                            dgrdDetails.Rows[rowIndex].Cells["openingRate"].Value = ConvertObjectToDouble(row["OpeningRate"]).ToString("N2", MainPage.indianCurancy);
                            dgrdDetails.Rows[rowIndex].Cells["saleRate"].Value = row["SaleRate"];
                            dgrdDetails.Rows[rowIndex].Cells["reOrder"].Value = row["Reorder"];
                            dgrdDetails.Rows[rowIndex].Cells["openingQty"].Value = row["OpeningQty"];
                            dgrdDetails.Rows[rowIndex].Cells["Chk"].Value = Convert.ToBoolean(row["ActiveStatus"]);
                            dgrdDetails.Rows[rowIndex].Cells["category1"].Value = row["Variant1"];
                            dgrdDetails.Rows[rowIndex].Cells["category2"].Value = row["Variant2"];
                            dgrdDetails.Rows[rowIndex].Cells["category3"].Value = row["Variant3"];
                            dgrdDetails.Rows[rowIndex].Cells["category4"].Value = row["Variant4"];
                            dgrdDetails.Rows[rowIndex].Cells["category5"].Value = row["Variant5"];
                            dgrdDetails.Rows[rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                            dgrdDetails.Rows[rowIndex].Cells["margin"].Value = row["Margin"];
                            dgrdDetails.Rows[rowIndex].Cells["godownName"].Value = row["GodownName"];
                            dgrdDetails.Rows[rowIndex].Cells["barCodeID"].Value = dgrdDetails.Rows[rowIndex].Cells["oldBarCode"].Value = row["Description"];
                            dgrdDetails.Rows[rowIndex].Cells["printCheck"].Value = true;

                            rowIndex++;
                        }
                    }
                    else
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.Rows[0].Cells["Chk"].Value = dgrdDetails.Rows[0].Cells["printCheck"].Value = true;
                        dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
                    }              
                }
            }
            catch
            {
            }
        }

        private void DesignMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;  
                else if(pnlDeleteAllItems.Visible)
                    pnlDeleteAllItems.Visible = false;                         
                else if (pnlSearch.Visible)
                    pnlSearch.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bAccountMasterView)
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
                }
            }
        }

        private void txtSerialNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }  

        private void txtUnitName_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("UNITNAME", "SEARCH UNIT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtUnitName.Text = objSearch.strSelectedData;
                    if (txtPurchaseUnit.Text == "")
                        txtPurchaseUnit.Text = txtUnitName.Text;
                    if (txtUnitName.Text == txtPurchaseUnit.Text)
                    {
                        txtQtyRatio.Text = "1";
                      //  txtQtyRatio.ReadOnly = true;
                    }
                    else
                    {
                      //  txtQtyRatio.Text = "1";
                       // txtQtyRatio.ReadOnly = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value) != "" && Convert.ToString(dgrdDetails.CurrentCell.Value) != "" && e.ColumnIndex !=2)
                    {
                        if (Convert.ToString(dgrdDetails.CurrentCell.Value) != "" && !MainPage.strUserRole.Contains("ADMIN"))
                        {
                            e.Cancel = true;
                            return;
                        }
                        else if (!MainPage.strUserRole.Contains("ADMIN"))
                        {
                            e.Cancel = true;
                            return;
                        }
                    }

                    if (e.ColumnIndex == 1)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 3)
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4)
                    {
                        SearchData objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 6)
                    {
                        SearchCategory objSearch = new SearchCategory("1", MainPage.StrCategory1, Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex ==7)
                    {
                        SearchCategory objSearch = new SearchCategory("2", MainPage.StrCategory2, Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 8)
                    {
                        SearchCategory objSearch = new SearchCategory("3", MainPage.StrCategory3, Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 9)
                    {
                        SearchCategory objSearch = new SearchCategory("4", MainPage.StrCategory4, Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex ==10)
                    {
                        SearchCategory objSearch = new SearchCategory("5", MainPage.StrCategory5, Keys.Space);
                        objSearch.ShowDialog();
                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                }
                else if(e.ColumnIndex!=18)
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                    if(colIndex==2 || colIndex == 5)
                    {
                        TextBox txtBox = (TextBox)e.Control;
                        if (txtBox != null)
                        {
                            txtBox.CharacterCasing = CharacterCasing.Upper;
                            txtBox.KeyPress += new KeyPressEventHandler(txtBox_Text_KeyPress);
                        }
                    }
                    else if (colIndex == 11 || colIndex== 12 || colIndex == 13 || colIndex == 14 || colIndex == 15 || colIndex == 16 || colIndex == 17 || colIndex == 18)
                    {
                        TextBox txtBox = (TextBox)e.Control;
                        if (txtBox != null)                        
                            txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);                        
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
             int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if(colIndex==16)
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    dba.KeyHandlerPoint(sender, e, 2);
            }
             else if (colIndex == 11 || colIndex == 12 || colIndex == 13 || colIndex == 14 || colIndex == 15 || colIndex == 16 || colIndex == 18)
             {
                 dba.KeyHandlerPoint(sender, e, 2);
             }
        }

        private void txtBox_Text_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 2 || dgrdDetails.CurrentCell.ColumnIndex == 5)
            {
                dba.ValidateSpace(sender, e);
            }
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
                            if (IndexColmn < dgrdDetails.ColumnCount - 10)
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
                                    if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                        IndexColmn++;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                                }
                            }
                            else if (Index == dgrdDetails.RowCount - 1)
                            {
                                string strPRate = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["purchaseRate"].Value), strSRate = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["saleRate"].Value), strQty = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["openingQty"].Value);

                                if ((strPRate != "" && strPRate != "0") || (strSRate != "" && strSRate != "0") || (strQty != "" && strQty != "0") || Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["category1"].Value) != "" || Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["category2"].Value) != "" || Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["category3"].Value) != "" || Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["category4"].Value) != "" || Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["category5"].Value) != "")
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[2];
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["Chk"].Value = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["printCheck"].Value = true;
                                    dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, dgrdDetails.Rows.Count,"",true);
                                }
                                else
                                {
                                    if (btnAdd.Text == "&Save")
                                        btnAdd.Focus();
                                    else
                                        btnEdit.Focus();
                                }
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
                            dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[2];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        if (strID == "" || MainPage.strUserRole.Contains("ADMIN"))
                        {
                            if (strID == "")
                            {
                                dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                                if (dgrdDetails.Rows.Count == 0)
                                {
                                    dgrdDetails.Rows.Add(1);
                                    dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                    dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
                                    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[2];
                                    dgrdDetails.Enabled = true;
                                }
                                else
                                {
                                    ArrangeSerialNo();
                                }
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
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                    {
                        int _rowCount = dgrdDetails.Rows.Count;
                        dgrdDetails.Rows.Add(1);
                        
                        dgrdDetails.Rows[_rowCount].Cells["srNo"].Value = (_rowCount + 1) + ".";
                        dgrdDetails.Rows[_rowCount].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, (_rowCount + 1), "");
                        dgrdDetails.Rows[_rowCount].Cells["supplierName"].Value = dgrdDetails.CurrentRow.Cells["supplierName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["styleName"].Value = dgrdDetails.CurrentRow.Cells["styleName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["brandName"].Value = dgrdDetails.CurrentRow.Cells["brandName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["category1"].Value = dgrdDetails.CurrentRow.Cells["category1"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["category2"].Value = dgrdDetails.CurrentRow.Cells["category2"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["category3"].Value = dgrdDetails.CurrentRow.Cells["category3"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["category4"].Value = dgrdDetails.CurrentRow.Cells["category4"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["category5"].Value = dgrdDetails.CurrentRow.Cells["category5"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["purchaseRate"].Value = dgrdDetails.CurrentRow.Cells["purchaseRate"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["margin"].Value = dgrdDetails.CurrentRow.Cells["margin"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["saleMRP"].Value = dgrdDetails.CurrentRow.Cells["saleMRP"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["disPer"].Value = dgrdDetails.CurrentRow.Cells["disPer"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["saleRate"].Value = dgrdDetails.CurrentRow.Cells["saleRate"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["margin"].Value = dgrdDetails.CurrentRow.Cells["margin"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["Chk"].Value = dgrdDetails.CurrentRow.Cells["Chk"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["godownName"].Value = dgrdDetails.CurrentRow.Cells["godownName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["openingQty"].Value = dgrdDetails.CurrentRow.Cells["openingQty"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["openingRate"].Value = dgrdDetails.CurrentRow.Cells["openingRate"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["printCheck"].Value = dgrdDetails.CurrentRow.Cells["printCheck"].Value;
                        
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[_rowCount].Cells["category1"];
                    }
                }
            }
            catch { }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
            }
        }

        private void DeleteOneRow(string strID)
        {
            try
            {
                string strQuery = "", strBarCodeQuery = "", strBarCode = "";
                 strQuery = " Delete from ItemSecondary Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " and ID=" + strID + " ";

                if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                {
                    strBarCode = Convert.ToString(dgrdDetails.CurrentRow.Cells["oldBarCode"].Value);
                    if (strBarCode != "")
                    {
                        strBarCodeQuery = " Delete from ItemSecondary Where [BillCode]=@SerialCode and [BillNo]=@SerialNo and [Description]='" + strBarCode + "' and [Description] Like('" + MainPage.strDataBaseFile + "%') ";
                    }
                }
                dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                int result = UpdateDesignMaster(strQuery, strBarCodeQuery);
                if (result < 1)
                {
                    strQuery = " Delete from ItemSecondary Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " and RemoteID=" + strID + " ";
                    DataBaseAccess.CreateDeleteQuery(strQuery);

                    BindRecordWithControl(txtSerialNo.Text);
                    btnEdit.Text = "&Update";
                }
                else
                {
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[0];
                        dgrdDetails.Enabled = true;
                    }
                    else
                        ArrangeSerialNo();
                }

            }
            catch
            {
            }
        }

        private void EnableAllControls()
        {
            txtBrandName.ReadOnly = txtDepartmentName.ReadOnly = txtDate.ReadOnly = txtBuyerDesignNo.ReadOnly = txtItemName.ReadOnly = txtQtyRatio.ReadOnly = txtDRemark.ReadOnly = false;
            chkDiscontinue.Enabled = txtGroupName.Enabled = true;

            if (btnEdit.Text == "&Update")
            {
                txtGroupName.Enabled = false;
                if (MainPage.strUserRole.Contains("ADMIN"))
                    txtGroupName.Enabled = true;
            }
        }

        private void DisableAllControls()
        {
            txtBrandName.ReadOnly = txtDepartmentName.ReadOnly = txtDate.ReadOnly = txtBuyerDesignNo.ReadOnly = txtItemName.ReadOnly = txtQtyRatio.ReadOnly = txtDRemark.ReadOnly= true;
            chkDiscontinue.Enabled = false; //dgrdDetails.Enabled =
            lblMsg.Text = "";
        }

        private void ClearAllText()
        {
            txtBrandName.Text = txtDepartmentName.Text = txtImportData.Text= txtCategory.Text= lblCreatedBy.Text = txtDRemark.Text=  txtBuyerDesignNo.Text = txtItemName.Text = txtGroupName.Text = txtPurchaseUnit.Text = txtUnitName.Text = "";
            txtQtyRatio.Text = "1";
            chkDiscontinue.Checked = false;
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
            dgrdDetails.Rows[0].Cells["Chk"].Value = dgrdDetails.Rows[0].Cells["printCheck"].Value = true;
            chkPick.Checked = false;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

            if (MainPage._bBarCodeStatus)
            {
                if (MainPage.strBarCodingType == "UNIQUE_BARCODE")
                    rdoUniqueBarCode.Checked = true;
                if (MainPage.strBarCodingType == "DESIGNMASTER_WISE")
                    rdoSameBarcodeYes.Checked = true;
                if (MainPage.strBarCodingType == "ITEM_WISE")
                    rdoItemwise.Checked = true;
            }
            else
            {
                rdoItemwise.Checked = rdoSameBarcodeYes.Checked = rdoUniqueBarCode.Checked = false;                
            }
            // SetDesignSerialNo();
        }

        private void SetDesignSerialNo()
        {
            DataTable dt = dba.GetDataTable("Select FChallanCode SerialCode,(Select (ISNULL(MAX(BillNo),0)+1) from Items Where BillCode=FChallanCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ");
            if (dt.Rows.Count > 0)
            {
                txtSerialCode.Text = Convert.ToString(dt.Rows[0]["SerialCode"]);
                txtSerialNo.Text = Convert.ToString(dt.Rows[0]["SerialNo"]);
            }
        }

        private bool ValidateControls()
        {
            if (txtSerialCode.Text == "")
            {
                MessageBox.Show("Sorry ! Serial code can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSerialCode.Focus();
                return false;
            }
            if (txtSerialNo.Text == "")
            {
                MessageBox.Show("Sorry ! Serial No can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSerialNo.Focus();
                return false;
            }
            //if (txtBuyerDesignNo.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Buyer Design Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtBuyerDesignNo.Focus();
            //    return false;
            //}
            if (txtItemName.Text == "")
            {
                MessageBox.Show("Sorry ! Design Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemName.Focus();
                return false;
            }
            if (txtGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! Group Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupName.Focus();
                return false;
            }
            if (txtPurchaseUnit.Text == "")
            {
                MessageBox.Show("Sorry ! Purchase Unit Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseUnit.Focus();
                return false;
            }
            if (txtUnitName.Text == "")
            {
                MessageBox.Show("Sorry ! Unit Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitName.Focus();
                return false;
            }
            //if (txtCategory.Text == "")
            //{
            //    txtCategory.Text = "STD";
            //}
            if (txtQtyRatio.Text == "")
            {
                MessageBox.Show("Sorry ! Qty Ratio can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQtyRatio.Focus();
                return false;
            }
            if (chkDiscontinue.Checked && txtDRemark.Text == "")
            {
                MessageBox.Show("Sorry ! Discontinue remark can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDRemark.Focus();
                return false;
            }
            string strStyle ;
            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                double dOpeningQty = dba.ConvertObjectToDouble(rows.Cells["openingQty"].Value), dOpeningRate = dba.ConvertObjectToDouble(rows.Cells["openingRate"].Value);
                string strBarCode = Convert.ToString(rows.Cells["barCodeID"].Value), strPRate = Convert.ToString(rows.Cells["purchaseRate"].Value), strSRate = Convert.ToString(rows.Cells["saleRate"].Value);
                strStyle= Convert.ToString(rows.Cells["styleName"].Value);
                if (Convert.ToString(rows.Cells["supplierName"].Value) =="" && strStyle == ""&& (strPRate == "" || strPRate == "0") && (strSRate == "" || strSRate == "0") && dOpeningQty == 0 && (Convert.ToString(rows.Cells["category1"].Value) == "" && Convert.ToString(rows.Cells["category2"].Value) == "" && Convert.ToString(rows.Cells["category3"].Value) == "" && Convert.ToString(rows.Cells["category4"].Value) == "" && Convert.ToString(rows.Cells["category5"].Value) == ""))
                {
                    dgrdDetails.Rows.Remove(rows);
                }
                else if (dOpeningQty != 0 && dOpeningRate == 0)
                {
                    MessageBox.Show("Sorry ! Please qty or rate ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdDetails.CurrentCell = rows.Cells["openingQty"];
                    dgrdDetails.Focus();
                    return false;
                }
                else if(strStyle=="")
                {
                    rows.Cells["styleName"].Value = txtItemName.Text;
                }              
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                dgrdDetails.Rows[0].Cells["Chk"].Value = dgrdDetails.Rows[0].Cells["printCheck"].Value = true;
                dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"");
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    btnAdd.Text = "&Save";
                    EnableAllControls();
                    txtSerialNo.ReadOnly = true;
                    SetDesignSerialNo();
                    ClearAllText();
                    pnlSearch.Visible = false;                  
                   
                    txtDate.Focus();
                }
                else if (CheckAvailability() && CheckBarCodeAvailability() && ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch { }
        }

        private void SaveRecord()
        {            
            DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
            string strItemType = "PURCHASE" ,strCodingType = "";
            if (rdoForJournal.Checked)
                strItemType = "JOURNAL";
            if (MainPage._bBarCodeStatus)
            {
                if (rdoUniqueBarCode.Checked)
                    strCodingType = "UNIQUE_BARCODE";
                else if (rdoSameBarcodeYes.Checked)
                    strCodingType = "DESIGNMASTER_WISE";
                else if (rdoItemwise.Checked)
                    strCodingType = "ITEM_WISE";
            }

            if (dgrdDetails.Rows.Count > 0)
            {
                txtBrandName.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["brandName"].Value);
                txtBuyerDesignNo.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["styleName"].Value);
            }
            else
                txtBrandName.Text = txtBuyerDesignNo.Text = "";

            string strQuery = " Declare @SerialNo bigint; Select @SerialNo=(ISNULL(MAX(BillNo),0)+1) from [dbo].[Items] Where [BillCode]='"+txtSerialCode.Text+"' if(@SerialNo='') Set @SerialNo=1; ";
         
            strQuery += " if not exists (Select [ItemName] from [dbo].[Items] Where (([BillCode]='" + txtSerialCode.Text + "' and [BillNo]=@SerialNo)  OR ([ItemName]='" + txtItemName.Text + "'))) begin  INSERT INTO [dbo].[Items] ([ItemName],[Date],[InsertStatus],[UpdateStatus],[GroupName],[SubGroupName],[UnitName],[BillCode],[BillNo],[BuyerDesignName],[QtyRatio],[StockUnitName],[DisStatus],[DisRemark],[Other],[CreatedBy],[UpdatedBy],[BrandName],[MakeName],[BarcodingType]) VALUES "
                     + " ('"+txtItemName.Text+"','"+strDate.ToString("MM/dd/yyyy")+"',1,0,'"+txtGroupName.Text+ "','" + strItemType + "','" + txtUnitName.Text + "','" + txtSerialCode.Text + "',@SerialNo,'" + txtBuyerDesignNo.Text + "',"+txtQtyRatio.Text+ ",'" + txtPurchaseUnit.Text + "','" + chkDiscontinue.Checked.ToString() + "','" + txtDRemark.Text + "','"+txtCategory.Text+"','" + MainPage.strLoginName + "','','"+txtBrandName.Text+"','"+txtDepartmentName.Text + "','" + strCodingType + "') ";

            double dORate = 0, dOQty = 0, dMargin=0, dSaleMRP = 0, dDisPer = 0, dSaleRate =0, dPurchaseRate=0, dReorder=0, dTOpeningQty = 0;
            string[] strFullParty;
            string strPurchasePartyID = "",strBarCode="",strUpdateStockQuery="", strStockMQuery="", strInnerQuery ="",strOuterQuery="",strBrandName,strStyleName;
            int _Index = 1;
            if (dgrdDetails.Rows.Count > 0)
            {
                strStockMQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName])";
            }
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                dMargin = dba.ConvertObjectToDouble(row.Cells["margin"].Value);
                dORate = dba.ConvertObjectToDouble(row.Cells["openingRate"].Value);
                dTOpeningQty += dOQty = dba.ConvertObjectToDouble(row.Cells["openingQty"].Value);
                dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                dDisPer = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                dPurchaseRate = dba.ConvertObjectToDouble(row.Cells["purchaseRate"].Value);
                dReorder = dba.ConvertObjectToDouble(row.Cells["reOrder"].Value);
                strBarCode = Convert.ToString(row.Cells["barCodeID"].Value);
                strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                strStyleName = Convert.ToString(row.Cells["styleName"].Value);

                if (dSaleRate != 0 && dSaleMRP == 0)
                    dSaleMRP = dSaleRate;
                if (dSaleRate == 0 && dSaleMRP != 0)
                    dSaleRate = dSaleMRP;

                //txtSerialNo.Text + "" + _Index + "" + dba.Random_No
                if (strBarCode == "")
                    strBarCode = dba.GetBarCode(txtSerialNo.Text, _Index, "",true);

                strFullParty = Convert.ToString(row.Cells["supplierName"].Value).Split(' ');
                if (strFullParty.Length > 1)
                    strPurchasePartyID = strFullParty[0];
                else
                    strPurchasePartyID = "";

                strInnerQuery += " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                              + " (0,'" + txtSerialCode.Text + "',@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + ",'" + dSaleMRP + "','" + dDisPer + "'," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate
                              + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',1,0)";
                if (_Index > 1)
                    strStockMQuery += " UNION ALL ";
                // strInnerQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                strStockMQuery += " SELECT 'OPENING','" + txtSerialCode.Text + "',@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "'";

                strUpdateStockQuery += " if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                      + " (0,'" + txtSerialCode.Text + "',@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate + " "
                      + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                      + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                      + " ('OPENING','" + txtSerialCode.Text + "',@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                      + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                      + " (0,'" + txtSerialCode.Text + "',@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + ",'" + dSaleMRP + "','" + dDisPer + "'," + dSaleRate + "," + dReorder + ",0,0 "
                      + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                      + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                      + " ('OPENING','" + txtSerialCode.Text + "',@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end ";

                _Index++;

            }

            strOuterQuery = " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                      + "('DESIGNMASTER','" + txtSerialCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),"+ dTOpeningQty+",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

            strUpdateStockQuery = strQuery + strUpdateStockQuery + strOuterQuery + " end ";
            strQuery = strQuery + strInnerQuery + strStockMQuery + strOuterQuery + " end ";
                      
            if (strQuery != "")
            {
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)                                         
                        dba.DataMirroringInCurrentFinYear(strUpdateStockQuery);                    

                    MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);                   
                    btnAdd.Text = "&Add";
                    if (newDesignStatus)
                    {
                        StrAddedDesignName = txtItemName.Text;
                        if (dgrdDetails.Rows.Count > 0)
                        {
                            StrAddedDesignName += "|" + dgrdDetails.Rows[0].Cells["category1"].Value + "|" + dgrdDetails.Rows[0].Cells["category2"].Value;
                        }                       
                        this.Close();
                    }
                    else
                    {
                        btnAdd.Text = "&Add";
                        dtDesignName = dba.GetDataTable("Select Distinct BillCode,BillNo,ItemName,BuyerDesignName from Items Order by ItemName");
                        BindRecordWithControl(txtSerialNo.Text);
                        BindDesignNameWithList();
                    }
                }
                else
                    MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetMappingQuery()
        {
            DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
            string strItemType = "PURCHASE";
            if (rdoForJournal.Checked)
                strItemType = "JOURNAL";

            if (dgrdDetails.Rows.Count > 0)
            {
                txtBrandName.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["brandName"].Value);
                txtBuyerDesignNo.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["styleName"].Value);
            }
            else
                txtBrandName.Text = txtBuyerDesignNo.Text = "";

            string strQuery = "", strUpdateQuery = GetUpdateQuery();
            strQuery += " Declare @SerialCode nvarchar(250),@SerialNo bigint; if not exists (Select [ItemName] from [dbo].[Items] Where [ItemName]='" + txtItemName.Text + "' ) begin Select @SerialNo=(ISNULL(MAX(BillNo),0)+1) from [dbo].[Items] Where [BillCode]='" + txtSerialCode.Text + "' if(@SerialNo='') Set @SerialNo=1;  INSERT INTO [dbo].[Items] ([ItemName],[Date],[InsertStatus],[UpdateStatus],[GroupName],[SubGroupName],[UnitName],[BillCode],[BillNo],[BuyerDesignName],[QtyRatio],[StockUnitName],[DisStatus],[DisRemark],[Other],[CreatedBy],[UpdatedBy],[BrandName],[MakeName]) VALUES "
                     + " ('" + txtItemName.Text + "','" + strDate.ToString("MM/dd/yyyy") + "',1,0,'" + txtGroupName.Text + "','" + strItemType + "','" + txtUnitName.Text + "','" + txtSerialCode.Text + "',@SerialNo,'" + txtBuyerDesignNo.Text + "'," + txtQtyRatio.Text + ",'" + txtPurchaseUnit.Text + "','" + chkDiscontinue.Checked.ToString() + "','" + txtDRemark.Text + "','" + txtCategory.Text + "','" + MainPage.strLoginName + "','','" + txtBrandName.Text + "','" + txtDepartmentName.Text + "') ";

            double dORate = 0, dOQty = 0, dMargin = 0, dSaleRate = 0, dPurchaseRate = 0, dReorder = 0, dTOpeningQty = 0;
            string[] strFullParty;
            string strPurchasePartyID = "", strBarCode = "", strUpdateStockQuery = "", strOuterQuery = "", strBrandName, strStyleName;
            int _Index = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                dMargin = dba.ConvertObjectToDouble(row.Cells["margin"].Value);
                dORate = dba.ConvertObjectToDouble(row.Cells["openingRate"].Value);
                dTOpeningQty += dOQty = dba.ConvertObjectToDouble(row.Cells["openingQty"].Value);
                dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                dPurchaseRate = dba.ConvertObjectToDouble(row.Cells["purchaseRate"].Value);
                dReorder = dba.ConvertObjectToDouble(row.Cells["reOrder"].Value);
                strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                strStyleName = Convert.ToString(row.Cells["styleName"].Value);

                strBarCode = Convert.ToString(row.Cells["barCodeID"].Value);// dba.GetBarCode(strSerialNo, _Index);
                strFullParty = Convert.ToString(row.Cells["supplierName"].Value).Split(' ');
                if (strFullParty.Length > 1)
                    strPurchasePartyID = strFullParty[0];
                else
                    strPurchasePartyID = "";
                strUpdateStockQuery += " if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                      + " (0,'" + txtSerialCode.Text + "',@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate + " "
                      + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                      + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                      + " ('OPENING','" + txtSerialCode.Text + "',@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                      + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                      + " (0,'" + txtSerialCode.Text + "',@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleRate + "," + dReorder + ",0,0 "
                      + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                      + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                      + " ('OPENING','" + txtSerialCode.Text + "',@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end ";

                _Index++;
            }

            strOuterQuery = " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                      + "('DESIGNMASTER','" + txtSerialCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTOpeningQty + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

            strUpdateStockQuery = strQuery + strUpdateStockQuery + strOuterQuery + " end else begin " + strUpdateQuery + " end ";

            return strUpdateStockQuery;
        }

        private int DataMapping(bool _mStatus)
        {
            string strUpdateStockQuery = GetMappingQuery();
            int _count = dba.DataMirroringInCurrentFinYear(strUpdateStockQuery);
            if (_mStatus)
            {
                if (_count > 0)
                    MessageBox.Show("Thank you ! Record saved successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                else
                    MessageBox.Show("Sorry ! Unable to mirror data right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return _count;
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void  txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Enabled = false;
                if (btnEdit.Text == "&Edit")
                {
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    txtSerialNo.ReadOnly = true;
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.Rows[0].Cells["Chk"].Value = true;
                        dgrdDetails.Rows[0].Cells["barCodeID"].Value = dba.GetBarCode(txtSerialNo.Text, 1,"") ;
                    }                  
                    pnlSearch.Visible =  false;
                    txtDate.Focus();
                }
                else if (CheckAvailability() && CheckBarCodeAvailability() &&  ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateDesignMaster("","");
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            if (strCreateStatus == "UPDATE")
                            {
                                StrAddedDesignName = txtItemName.Text;
                                if (dgrdDetails.Rows.Count > 0)
                                {
                                    StrAddedDesignName += "|" + dgrdDetails.Rows[dgrdDetails.Rows.Count-1].Cells["category1"].Value + "|" + dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["category2"].Value;
                                }
                                this.Close();
                            }
                            else
                            {
                                btnEdit.Text = "&Edit";                             
                                BindRecordWithControl(txtSerialNo.Text);
                                BindDesignNameWithList();
                            }
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

        private int UpdateDesignMaster(string strSubQuery,string strBarCodeQuery)
        {
            int result = 0;
            try
            {
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string strItemType = "PURCHASE", strNetQuery = "", strOuterQuery = "", strMainQuery = "", strCodingType="",strStockQuery="";
                if (rdoForJournal.Checked)
                    strItemType = "JOURNAL";
                if (MainPage._bBarCodeStatus)
                {
                    if (rdoUniqueBarCode.Checked)
                        strCodingType = "UNIQUE_BARCODE";
                    else if (rdoSameBarcodeYes.Checked)
                        strCodingType = "DESIGNMASTER_WISE";
                    else if (rdoItemwise.Checked)
                        strCodingType = "ITEM_WISE";
                }

                if (dgrdDetails.Rows.Count > 0)
                {
                    txtBrandName.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["brandName"].Value);
                    txtBuyerDesignNo.Text = Convert.ToString(dgrdDetails.Rows[0].Cells["styleName"].Value);
                }
                else
                    txtBrandName.Text = txtBuyerDesignNo.Text = "";


                string strQuery = "";                
                strMainQuery += " UPDATE [dbo].[Items] SET [ItemName]='" + txtItemName.Text + "',[Date]='" + strDate.ToString("MM/dd/yyyy") + "',[UpdateStatus]=1,[GroupName]='" + txtGroupName.Text + "',[SubGroupName]='" + strItemType + "',[UnitName]='" + txtUnitName.Text + "',[BuyerDesignName]='" + txtBuyerDesignNo.Text + "',[QtyRatio]=" + txtQtyRatio.Text + ",[StockUnitName]='" + txtPurchaseUnit.Text + "',[DisStatus]='" + chkDiscontinue.Checked.ToString() + "',[DisRemark]='" + txtDRemark.Text + "',[Other]='" + txtCategory.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[BrandName]='" + txtBrandName.Text + "',[MakeName]='" + txtDepartmentName.Text + "',[BarcodingType]='" + strCodingType + "' Where [BillCode]=@SerialCode and [BillNo]=@SerialNo ";


                if (strOldDesignName != txtItemName.Text)
                {
                    strQuery += " Update GoodsReceiveDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update PurchaseBookSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SalesBookSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update StockMaster Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update OrderBooking Set Items='" + txtItemName.Text + "' Where Items='" + strOldDesignName + "' "
                           + " Update PurchaseReturnDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SaleReturnDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SaleServiceDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update JournalVoucherDetails Set Other='" + txtItemName.Text + "' Where Other='" + strOldDesignName + "' "
                           + " Update StockTransferSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' ";
                }

                strMainQuery += strQuery;
                strQuery += " UPDATE [dbo].[Items] SET [ItemName]='" + txtItemName.Text + "',[Date]='" + strDate.ToString("MM/dd/yyyy") + "',[UpdateStatus]=1,[GroupName]='" + txtGroupName.Text + "',[SubGroupName]='" + strItemType + "',[UnitName]='" + txtUnitName.Text + "',[BuyerDesignName]='" + txtBuyerDesignNo.Text + "',[QtyRatio]=" + txtQtyRatio.Text + ",[StockUnitName]='" + txtPurchaseUnit.Text + "',[DisStatus]='" + chkDiscontinue.Checked.ToString() + "',[DisRemark]='" + txtDRemark.Text + "',[Other]='" + txtCategory.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[BrandName]='" + txtBrandName.Text + "',[MakeName]='" + txtDepartmentName.Text + "',[BarcodingType]='" + strCodingType + "' Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " ";

                double dORate = 0, dOQty =0,dMargin=0, dSaleMRP = 0, dDisPer = 0, dSaleRate =0,dPurchaseRate=0,dReorder=0, dTOpeningQty=0, dOldPRate=0, dOldSRate=0, dOldSmrp = 0, dOldOQty =0, dOldORate=0;
                string[] strFullParty;
                string strPurchasePartyID = "",strBarCode="",strBrandName, strStyleName, strUpdateStockQuery ="",strStockMQuery = "",strInnerQuery ="",strOldBarCode="", strChangeDetailsUpdate="",strDBCode= MainPage.strDataBaseFile;
                int _index = 1;
                if(dgrdDetails.Rows.Count > 0)
                {
                    strStockMQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) ";
                }
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dMargin = dba.ConvertObjectToDouble(row.Cells["margin"].Value);
                    dORate = dba.ConvertObjectToDouble(row.Cells["openingRate"].Value);
                    dTOpeningQty += dOQty = dba.ConvertObjectToDouble(row.Cells["openingQty"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dPurchaseRate = dba.ConvertObjectToDouble(row.Cells["purchaseRate"].Value);
                    dReorder = dba.ConvertObjectToDouble(row.Cells["reOrder"].Value);
                    strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                    strStyleName = Convert.ToString(row.Cells["styleName"].Value);

                    dOldPRate = dba.ConvertObjectToDouble(row.Cells["PRate"].Value);
                    dOldSRate = dba.ConvertObjectToDouble(row.Cells["SRate"].Value);
                    dOldSmrp = dba.ConvertObjectToDouble(row.Cells["Smrp"].Value);
                    dOldOQty = dba.ConvertObjectToDouble(row.Cells["OQty"].Value);
                    dOldORate = dba.ConvertObjectToDouble(row.Cells["ORate"].Value);

                    if (dPurchaseRate != dOldPRate && _index == 1)
                    {
                        strChangeDetailsUpdate += " P.Rate=" + dOldPRate; 
                    }
                    if (dSaleRate != dOldSRate && _index ==1)
                    {
                        strChangeDetailsUpdate += " S.Rate=" + dOldSRate;
                    }
                    if (dTOpeningQty != dOldOQty && _index == 1)
                    {
                        strChangeDetailsUpdate += " O.Qty=" + dOldOQty;
                    }
                    if (dORate != dOldORate && _index == 1)
                    {
                        strChangeDetailsUpdate += " O.Rate=" + dOldORate;
                    }

                    strBarCode = Convert.ToString(row.Cells["barCodeID"].Value);
                    strOldBarCode = Convert.ToString(row.Cells["oldBarCode"].Value);

                    strFullParty = Convert.ToString(row.Cells["supplierName"].Value).Split(' ');
                    if (strFullParty.Length > 1)
                        strPurchasePartyID = strFullParty[0];
                    else
                        strPurchasePartyID = "";
                    if(strBarCode=="")
                        strBarCode= dba.GetBarCode(txtSerialNo.Text, _index,"",true);

                    string strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID != "" && strID != "0")
                    {
                        strInnerQuery += " UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                        + " [Margin]=" + dMargin + ",[SaleMRP]=" + dSaleMRP + ",[DisPer]=" + dDisPer + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[OpeningQty]=" + dOQty + ",[OpeningRate]=" + dORate + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [ID]=" + strID + " and [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " ";

                        strNetQuery += " UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                    + " [Margin]=" + dMargin + ",[SaleMRP]=" + dOldSmrp + ",[DisPer]=" + dDisPer + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[OpeningQty]=" + dOQty + ",[OpeningRate]=" + dORate + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "' Where [RemoteID]=" + strID + " and [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " ";

                        if (strOldBarCode != "")
                        {
                            //strUpdateStockQuery += " UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                            //                    + " [Margin]=" + dMargin + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "-%') and [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " ";

                            strUpdateStockQuery += " if exists (Select BillCode from [dbo].[ItemSecondary] Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo) begin if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                                + " [Margin]=" + dMargin + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[OpeningQty]=" + dOQty + ",[OpeningRate]=" + dORate + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                                + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                                + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                                                + " UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                                + " [Margin]=" + dMargin + ",[SaleMRP]=" + dSaleMRP + ",[DisPer]=" + dDisPer + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                                + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                                + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end end ";
                        }
                    }
                    else
                    {
                        if(strBarCode=="")
                        strBarCode = dba.GetBarCode(txtSerialNo.Text, _index,"");
                     
                        strInnerQuery += " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                 + " (0,'" + txtSerialCode.Text + "'," + txtSerialNo.Text + ",'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleMRP + "," + dDisPer + "," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate
                                 + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',1,0)";

                        strUpdateStockQuery += " if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                            + " (0,@SerialCode,@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleMRP + "," + dDisPer + "," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate + " "
                                            + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                                            + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                            + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                                            + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                            + " (0,@SerialCode,@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleMRP + "," + dDisPer + "," + dSaleRate + "," + dReorder + ",0,0 "
                                            + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                                            + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                            + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end ";
                    }

                    if (_index > 1)
                        strStockMQuery += " UNION ALL ";
                        //strInnerQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                        strStockMQuery += "SELECT 'OPENING','" + txtSerialCode.Text + "'," + txtSerialNo.Text + ", '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "'";

                    _index++;
                }

                strOuterQuery = " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason]) VALUES "
                             + "('DESIGNMASTER','" + txtSerialCode.Text + "'," + txtSerialNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),"+ dTOpeningQty+",'" + MainPage.strLoginName + "',1,0,'UPDATION','" + strChangeDetailsUpdate + "') ";

                strMainQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason]) VALUES "
                            + "('DESIGNMASTER',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),"+ dTOpeningQty+",'" + MainPage.strLoginName + "',1,0,'UPDATION','" + strChangeDetailsUpdate + "') ";


                strUpdateStockQuery = " Declare @SerialCode nvarchar(250),@SerialNo bigint; Select @SerialCode=BillCode,@SerialNo=BillNo from [dbo].[Items] Where ItemName='" + strOldDesignName + "'  "
                                    + " Delete from [dbo].[StockMaster] Where BillType='OPENING' and [BarCode] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                    + strMainQuery +strBarCodeQuery+ strUpdateStockQuery;

                strNetQuery = strQuery+ strNetQuery+strOuterQuery;

                strQuery = " Delete from [dbo].[StockMaster] Where BillType='OPENING' and BillCode='" + txtSerialCode.Text + "' and BillNo=" + txtSerialNo.Text + " "
                        + strQuery + strSubQuery + strInnerQuery + strStockMQuery + strOuterQuery;
               
                result = dba.ExecuteMyQuery(strQuery);
                if (result > 0)
                {
                    if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                    { 
                       int _mCount= dba.DataMirroringInCurrentFinYear(strUpdateStockQuery);
                        if (_mCount < 1)
                            DataMapping(false);

                    }

                    DataBaseAccess.CreateDeleteQuery(strNetQuery);
                }
            }
            catch
            {
            }
            return result;
        }

        private string GetUpdateQuery()
        {
            string strQuery = "", strUpdateStockQuery = "";
            try
            {
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                string strItemType = "PURCHASE",strMainQuery = "", strCodingType="";
                if (rdoForJournal.Checked)
                    strItemType = "JOURNAL";
                if (MainPage._bBarCodeStatus)
                {
                    if (rdoUniqueBarCode.Checked)
                        strCodingType = "UNIQUE_BARCODE";
                    else if (rdoSameBarcodeYes.Checked)
                        strCodingType = "DESIGNMASTER_WISE";
                    else if (rdoItemwise.Checked)
                        strCodingType = "ITEM_WISE";
                }

                strMainQuery += " UPDATE [dbo].[Items] SET [ItemName]='" + txtItemName.Text + "',[Date]='" + strDate.ToString("MM/dd/yyyy") + "',[UpdateStatus]=1,[GroupName]='" + txtGroupName.Text + "',[SubGroupName]='" + strItemType + "',[UnitName]='" + txtUnitName.Text + "',[BuyerDesignName]='" + txtBuyerDesignNo.Text + "',[QtyRatio]=" + txtQtyRatio.Text + ",[StockUnitName]='" + txtPurchaseUnit.Text + "',[DisStatus]='" + chkDiscontinue.Checked.ToString() + "',[DisRemark]='" + txtDRemark.Text + "',[Other]='" + txtCategory.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[BrandName]='" + txtBrandName.Text + "',[MakeName]='" + txtDepartmentName.Text + "',[BarcodingType]='" + strCodingType + "' Where [BillCode]=@SerialCode and [BillNo]=@SerialNo ";


                if (strOldDesignName != txtItemName.Text)
                {
                    strQuery += " Update GoodsReceiveDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update PurchaseBookSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SalesBookSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update StockMaster Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update OrderBooking Set Items='" + txtItemName.Text + "' Where Items='" + strOldDesignName + "' "
                           + " Update PurchaseReturnDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SaleReturnDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update SaleServiceDetails Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' "
                           + " Update JournalVoucherDetails Set Other='" + txtItemName.Text + "' Where Other='" + strOldDesignName + "' "
                           + " Update StockTransferSecondary Set ItemName='" + txtItemName.Text + "' Where ItemName='" + strOldDesignName + "' ";
                }

                strMainQuery += strQuery;
                strQuery += " UPDATE [dbo].[Items] SET [ItemName]='" + txtItemName.Text + "',[Date]='" + strDate.ToString("MM/dd/yyyy") + "',[UpdateStatus]=1,[GroupName]='" + txtGroupName.Text + "',[SubGroupName]='" + strItemType + "',[UnitName]='" + txtUnitName.Text + "',[BuyerDesignName]='" + txtBuyerDesignNo.Text + "',[QtyRatio]=" + txtQtyRatio.Text + ",[StockUnitName]='" + txtPurchaseUnit.Text + "',[DisStatus]='" + chkDiscontinue.Checked.ToString() + "',[DisRemark]='" + txtDRemark.Text + "',[Other]='" + txtCategory.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[BrandName]='" + txtBrandName.Text + "',[MakeName]='" + txtDepartmentName.Text + "',[BarcodingType]='" + strCodingType + "' Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text + " ";

                double dORate = 0, dOQty = 0, dMargin = 0, dSaleMRP = 0, dDisPer = 0, dSaleRate = 0, dPurchaseRate = 0, dReorder = 0, dTOpeningQty = 0;
                string[] strFullParty;
                string strPurchasePartyID = "", strBarCode = "", strOldBarCode = "", strStyleName, strBrandName, strDBCode = MainPage.strDataBaseFile;
                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dMargin = dba.ConvertObjectToDouble(row.Cells["margin"].Value);
                    dORate = dba.ConvertObjectToDouble(row.Cells["openingRate"].Value);
                    dTOpeningQty += dOQty = dba.ConvertObjectToDouble(row.Cells["openingQty"].Value);
                    dSaleRate = dba.ConvertObjectToDouble(row.Cells["saleRate"].Value);
                    dSaleMRP = dba.ConvertObjectToDouble(row.Cells["saleMRP"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dPurchaseRate = dba.ConvertObjectToDouble(row.Cells["purchaseRate"].Value);
                    dReorder = dba.ConvertObjectToDouble(row.Cells["reOrder"].Value);
                    strBrandName = Convert.ToString(row.Cells["brandName"].Value);
                    strStyleName = Convert.ToString(row.Cells["styleName"].Value);

                    strBarCode = Convert.ToString(row.Cells["barCodeID"].Value);
                    strOldBarCode = Convert.ToString(row.Cells["oldBarCode"].Value);

                    strFullParty = Convert.ToString(row.Cells["supplierName"].Value).Split(' ');
                    if (strFullParty.Length > 1)
                        strPurchasePartyID = strFullParty[0];
                    else
                        strPurchasePartyID = "";

                    string strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID == "" || strID == "0")
                        strBarCode = dba.GetBarCode(txtSerialNo.Text, _index,"");


                    strUpdateStockQuery += " if exists (Select BillCode from [dbo].[ItemSecondary] Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo) begin if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                        + " [Margin]=" + dMargin + ",[SaleMRp]=" + dSaleMRP + ",[DisPer]=" + dDisPer + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[OpeningQty]=" + dOQty + ",[OpeningRate]=" + dORate + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                        + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                        + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                                        + " UPDATE [dbo].[ItemSecondary] SET [PurchasePartyID]='" + strPurchasePartyID + "',[Brand]='" + strBrandName + "',[DesignName]='" + strStyleName + "',[Variant1]='" + row.Cells["category1"].Value + "',[Variant2]='" + row.Cells["category2"].Value + "',[Variant3]='" + row.Cells["category3"].Value + "',[Variant4]='" + row.Cells["category4"].Value + "',[Variant5]='" + row.Cells["category5"].Value + "',[PurchaseRate]=" + dPurchaseRate + ",[Description]='" + strBarCode + "',"
                                        + " [Margin]=" + dMargin + ",[SaleMRP]=" + dSaleMRP + ",[DisPer]=" + dDisPer + ",[SaleRate]=" + dSaleRate + ",[Reorder]=" + dReorder + ",[ActiveStatus]='" + Convert.ToBoolean(row.Cells["Chk"].Value) + "',[GodownName]='" + row.Cells["godownName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [Description]='" + strOldBarCode + "' and [Description] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                        + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                        + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end end else begin "
                                        + " if exists (Select CompanyName from CompanyDetails Where TAXEnabled=0) begin INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                        + " (0,@SerialCode,@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleRate + "," + dReorder + "," + dOQty + "," + dORate + " "
                                        + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                                        + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                        + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dOQty + "," + dORate + " ,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end else begin "
                                        + " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Brand],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleMRP],[DisPer],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                        + " (0,@SerialCode,@SerialNo,'" + strPurchasePartyID + "','" + strBrandName + "','" + strStyleName + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "'," + dPurchaseRate + "," + dMargin + "," + dSaleMRP + "," + dDisPer + "," + dSaleRate + "," + dReorder + ",0,0 "
                                        + ",'" + Convert.ToBoolean(row.Cells["Chk"].Value) + "','" + row.Cells["godownName"].Value + "','" + strBarCode + "','" + MainPage.strLoginName + "','',0,0) "
                                        + " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                        + " ('OPENING',@SerialCode,@SerialNo, '" + txtItemName.Text + "','" + row.Cells["category1"].Value + "','" + row.Cells["category2"].Value + "','" + row.Cells["category3"].Value + "','" + row.Cells["category4"].Value + "','" + row.Cells["category5"].Value + "',0,0,'" + row.Cells["godownName"].Value + "','" + MainPage.strLoginName + "','',1,0," + dORate + ",'" + strDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strStyleName + "') end end ";
                    _index++;
                }


                strMainQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                            + "('DESIGNMASTER',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTOpeningQty + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                strUpdateStockQuery = " Select @SerialCode=BillCode,@SerialNo=BillNo from [dbo].[Items] Where ItemName='" + strOldDesignName + "'  "
                                    + " Delete from [dbo].[StockMaster] Where BillType='OPENING' and [BarCode] Like('" + strDBCode + "%') and [BillCode]=@SerialCode and [BillNo]=@SerialNo "
                                    + strMainQuery +  strUpdateStockQuery;

            }
            catch
            {
            }
            return strUpdateStockQuery;
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private bool CheckDesignNameExistence()
        {
            string strQuery = "Select ISNULL(SUM(Qty),0) Qty from (Select ISNULL(Count(*),0) Qty from SaleReturnDetails Where ItemName='" + txtItemName.Text + "' UNION ALL Select ISNULL(Count(*),0) Qty from PurchaseReturnDetails Where ItemName='" + txtItemName.Text + "'  UNION ALL Select ISNULL(Count(*),0) Qty from GoodsReceiveDetails Where ItemName='" + txtItemName.Text + "'  UNION ALL Select ISNULL(Count(*),0) Qty from JournalVoucherDetails Where Other='" + txtItemName.Text + "'  UNION ALL Select ISNULL(Count(*),0) Qty from SaleServiceDetails Where ItemName='" + txtItemName.Text+ "' UNION ALL Select ISNULL(Count(*),0) Qty from StockMaster Where BillType!='OPENING' and Qty>0 and ItemName='" + txtItemName.Text + "' ) Design ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            double dCount = ConvertObjectToDouble(objValue);
            if (dCount > 0)
            {
                MessageBox.Show("Sorry ! This item Name has been used some where " + dCount + " times that's why unable to delete ! ", "Can't Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
                return true;
        }

        private void txtDesignNo_Leave(object sender, EventArgs e)
        {
            bool chk = CheckAvailability();
            if (chk)
                txtBuyerDesignNo.Text = txtItemName.Text;
        }

        private bool CheckAvailability()
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtItemName.Text != "")
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Count(*),0) from Items Where ItemName='" + txtItemName.Text + "'");
                            if (Convert.ToInt32(objValue) > 0)
                            {
                                lblMsg.Text = txtItemName.Text + " is already exist ! Please try with different ";
                                lblMsg.ForeColor = System.Drawing.Color.Red;
                                txtItemName.Focus();
                                return false;
                            }
                            else
                            {
                                lblMsg.Text = txtItemName.Text + " is available ! Please proceed";
                                lblMsg.ForeColor = System.Drawing.Color.DarkGreen;
                                return true;
                            }
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Count(*),0) from Items Where ItemName='" + txtItemName.Text + "' and  [BillCode]='" + txtSerialCode.Text + "' and [BillNo]!=" + txtSerialNo.Text + " ");
                            if (Convert.ToInt32(objValue) > 0)
                            {
                                lblMsg.Text = txtItemName.Text + " is already exist ! Please try with different ";
                                lblMsg.ForeColor = System.Drawing.Color.Red;
                                txtItemName.Focus();
                                return false;
                            }
                            else
                            {
                                lblMsg.Text = txtItemName.Text + " is available ! Please proceed";
                                lblMsg.ForeColor = System.Drawing.Color.DarkGreen;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Items Name can't be blank ";
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                        txtItemName.Focus();
                        return false;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private bool CheckBarCodeAvailability()
        {
            if (MainPage._bBarCodeStatus)
            {
                try
                {
                    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    {
                        string strBarCode = "", strAllBarCode = "";
                        foreach (DataGridViewRow row in dgrdDetails.Rows)
                        {
                            strBarCode = Convert.ToString(row.Cells["barCodeID"].Value);
                            if (strBarCode != "")
                            {
                                if (strAllBarCode.Contains(strBarCode))
                                {
                                    row.Cells["barCodeID"].Value = "";
                                    MessageBox.Show("Sorry !! Bar code No : " + strBarCode + " can't be duplicate !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                                else
                                {
                                    if (strAllBarCode != "")
                                        strAllBarCode += ",";
                                    strAllBarCode += "'" + strBarCode + "'";
                                }
                            }
                        }
                        if (strAllBarCode == "")
                            strAllBarCode += "''";

                        object objValue = DataBaseAccess.ExecuteMyScalar("Select CAST(BillNo as varchar)+':'+Description from ItemSecondary Where Description!='' and Description in (" + strAllBarCode + ") and BillCode in ('" + txtSerialCode.Text + "') and BillNo not in (" + txtSerialNo.Text + ") ");
                        if (Convert.ToString(objValue) != "")
                        {
                            MessageBox.Show("Sorry !!  Bar code already exists in Serial no : " + objValue + " !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else
                            return true;
                    }
                }
                catch
                {
                }
            }
            else
                return true;
            return false;
        }        


        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                //if (e.ColumnIndex == 11 || e.ColumnIndex == 12)               
                //    CalculateSaleRate(dgrdDetails.Rows[e.RowIndex]);
                //else if(e.ColumnIndex == 13 || e.ColumnIndex == 14)
                //    CalculateSaleMarginWithSaleMRP(dgrdDetails.Rows[e.RowIndex]);

                if (e.ColumnIndex == 11 || e.ColumnIndex == 12)
                    CalculateSaleMRP(dgrdDetails.Rows[e.RowIndex]);
                else if (e.ColumnIndex == 14)
                    CalculateSaleRate(dgrdDetails.Rows[e.RowIndex]);
                else if (e.ColumnIndex == 13)
                {
                    CalculateSaleMargin(dgrdDetails.Rows[e.RowIndex]);
                    CalculateSaleRate(dgrdDetails.Rows[e.RowIndex]);
                }
            }
        }

        private void CalculateSaleMRP(DataGridViewRow rows)
        {
            double dSaleMRP = 0, dPRate = ConvertObjectToDouble(rows.Cells["purchaseRate"].Value)
                            , dMargin = ConvertObjectToDouble(rows.Cells["margin"].Value);
            dSaleMRP = Math.Round(dPRate + ((dPRate * dMargin) / 100), 0);

            rows.Cells["saleMRP"].Value = dSaleMRP.ToString("N2", MainPage.indianCurancy);
            CalculateSaleRate(rows);
        }
        private void CalculateSaleRate(DataGridViewRow rows)
        {
            double dSRate = 0, dMRP = ConvertObjectToDouble(rows.Cells["saleMRP"].Value)
                            , dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);
            dSRate = Math.Round(dMRP - ((dMRP * dDisPer) / 100), 0);

            rows.Cells["saleRate"].Value = dSRate.ToString("N2", MainPage.indianCurancy);
        }

        private void CalculateSaleMargin(DataGridViewRow rows)
        {
            double dMargin = 0, dPRate = ConvertObjectToDouble(rows.Cells["purchaseRate"].Value)
                              , dSRate = ConvertObjectToDouble(rows.Cells["saleMRP"].Value);
            if (dSRate != 0 && dPRate != 0)
                dMargin = ((dSRate * 100) / dPRate) - 100.00;

            rows.Cells["margin"].Value = dMargin.ToString("N2", MainPage.indianCurancy);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtSerialNo.ReadOnly = false;
            pnlSearch.Visible = true;
            txtSearchDesign.Clear();
            txtSearchDesign.Focus();
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {

                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMGROUPNAME", "SEARCH HSN GROUP", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtGroupName.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void DesignMaster_Load(object sender, EventArgs e)
        {
            try
            {
                btnDataMirror.Enabled = MainPage._bItemMirroring && MainPage.bHSNWisePurchase ? false : true;
                btnRemoveAll.Enabled= MainPage.strUserRole=="SUPERADMIN" ? true : false;

                btnPrint.Enabled = MainPage.mymainObject.bBarcodePrint;
                grpBarCodingType.Enabled = MainPage._bBarCodeStatus;
                if (SetPermission())
                {
                    if (strCreateStatus == "UPDATE")
                    {
                        btnEdit.PerformClick();
                        int _rowIndex = dgrdDetails.Rows.Count;
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_rowIndex].Cells["barCodeID"].Value = _strBarCode;
                        dgrdDetails.Rows[_rowIndex].Cells["category1"].Value = strVariant1;
                        dgrdDetails.Rows[_rowIndex].Cells["category2"].Value = strVariant2;
                        dgrdDetails.Rows[_rowIndex].Cells["purchaseRate"].Value = strDPurchaseRate;

                        btnEdit.Focus();
                    }
                    else if (newDesignStatus || strCreateStatus == "CREATE")
                    {
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        txtSerialCode.TabStop = txtSerialNo.TabStop = txtDate.TabStop = false;
                        btnAdd.PerformClick();
                        txtItemName.Focus();
                        txtGroupName.Text = strGroupName;
                        txtItemName.Text = txtBuyerDesignNo.Text = strItemName;
                        txtPurchaseUnit.Text = txtUnitName.Text = strUnit;
                        if(dgrdDetails.Rows.Count==0)
                            dgrdDetails.Rows.Add();
                        if (_strBarCode == "")
                            _strBarCode = dba.GetBarCode(txtSerialNo.Text, 1,"");

                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                        dgrdDetails.Rows[0].Cells["barCodeID"].Value = _strBarCode;
                        dgrdDetails.Rows[0].Cells["category1"].Value = strVariant1;
                        dgrdDetails.Rows[0].Cells["category2"].Value = strVariant2;
                        dgrdDetails.Rows[0].Cells["purchaseRate"].Value = strDPurchaseRate;
                        if (strCreateStatus == "CREATE")
                            txtGroupName.Focus();
                    }                     
                }
            }
            catch { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void txtPurchaseUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("UNITNAME", "SEARCH UNIT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseUnit.Text = objSearch.strSelectedData;
                    if (txtUnitName.Text == "")
                        txtUnitName.Text = txtPurchaseUnit.Text;
                    if (txtUnitName.Text == txtPurchaseUnit.Text)
                    {
                        txtQtyRatio.Text = "1";
                        //txtQtyRatio.ReadOnly = true;
                    }
                    else
                    {
                        txtQtyRatio.Text = "1";
                       // txtQtyRatio.ReadOnly = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
              
        private void txtSearchDesign_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lBoxDesignName.Items.Clear();
                if (dtDesignName != null)
                {
                    if (txtSearchDesign.Text != "")
                    {
                        DataRow[] rows = dtDesignName.Select(String.Format("ItemName Like ('%" + txtSearchDesign.Text + "%') OR BuyerDesignName Like ('%" + txtSearchDesign.Text + "%')"));
                        foreach (DataRow row in rows)
                        {
                            lBoxDesignName.Items.Add(row["ItemName"]);
                        }
                        if (lBoxDesignName.Items.Count > 0)
                            lBoxDesignName.SelectedIndex = 0;
                    }
                    else
                        BindDesignNameWithList();
                }
            }
            catch
            {
            }
        }

        private void txtSearchDesign_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GetSelectDesignFromList();
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                lBoxDesignName.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    string strStockUnit = txtQtyRatio.Text + " " + txtUnitName.Text, strCodingType="";
                    if (MainPage._bBarCodeStatus)
                    {
                        if (rdoUniqueBarCode.Checked)
                            strCodingType = "UNIQUE_BARCODE";
                        else if (rdoSameBarcodeYes.Checked)
                            strCodingType = "DESIGNMASTER_WISE";
                        else if (rdoItemwise.Checked)
                            strCodingType = "ITEM_WISE";
                    }

                    BarCode_Printing objBarCode = new BarCode_Printing(txtSerialCode.Text, txtSerialNo.Text, txtDate.Text, dgrdDetails, txtItemName.Text, txtBuyerDesignNo.Text, txtBrandName.Text, txtPurchaseUnit.Text, strStockUnit, strCodingType);
                    objBarCode.MdiParent = MainPage.mymainObject;
                    objBarCode.Show();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bar code in Purchase Book Retail", ex.Message };
                dba.CreateErrorReports(strReport);
            }

            btnPrint.Enabled = true;

            //try
            //{
            //    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            //    {
            //        btnPrint.Enabled = false;
            //        DataTable dt = CreateDataTable();
            //        if (dt.Rows.Count > 0)
            //        {
            //            string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
            //            if (strValue != "" && strValue != "0")
            //            {
            //                Reporting.BarCodeReport objReport = new Reporting.BarCodeReport();
            //                objReport.SetDataSource(dt);
            //               // objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            //                objReport.PrintToPrinter(Convert.ToInt32(strValue), false, 0, 0);

            //                objReport.Close();
            //                objReport.Dispose();
            //            }
            //        }
            //        else
            //            MessageBox.Show("Sorry ! No reocrd for printing", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //catch
            //{
            //}
            //btnPrint.Enabled = true;
        }

        private void txtImportData_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {

                        SearchDataOther objSearch = new SearchDataOther("DESIGNNAME","", "SEARCH DESIGN NAME", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportData.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
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
            if (txtImportData.Text != "" && btnAdd.Text == "&Save")
            {
                BindRecordWithControlWithImport();
            }

        }

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
            {
                txtImportData.Enabled = chkPick.Checked;
                txtImportData.Clear();
            }
            else
            {
                txtImportData.Enabled = false;
                txtImportData.Clear();
            }
        }

        private void txtMakeName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtBrandName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtDepartmentName_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DEPARTMENTNAME", "SEARCH DEPARTMENT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtDepartmentName.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void txtBrandName_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBrandName.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        private void lBoxDesignName_DoubleClick(object sender, EventArgs e)
        {
            GetSelectDesignFromList();
        }

        private void DesignMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            }
            catch { }
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
            txtReason.Text = "";
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            pnlDeleteAllItems.Visible = true;
            txtRemoveAllItemReason.Focus();
            
        }

        private void btnAllItemFinalDelete_Click(object sender, EventArgs e)
        {
            btnAllItemFinalDelete.Enabled = btnRemoveAll.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.strUserRole.Contains("SUPERADMIN"))
                {
                    if (txtRemoveAllItemReason.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to DELETE ALL ITEMS WITH OPENING QTY ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " Delete from Items " 
                                            + " Delete from ItemSecondary "
                                            + " Delete from StockMaster Where BillType='OPENING' "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('DESIGNMASTER','" + txtSerialCode.Text + "'," + txtSerialNo.Text + ",'" + txtRemoveAllItemReason.Text + ".',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                txtRemoveAllItemReason.Text = "";
                                pnlDeletionConfirmation.Visible = false;
                                MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindNextRecord();
                                BindDesignNameWithList();
                            }
                            else
                                MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtRemoveAllItemReason.Focus();
                    }
                }
            }
            catch
            {
            }
            btnAllItemFinalDelete.Enabled = btnRemoveAll.Enabled = false;
        }

        private void btnRemoveAllClose_Click(object sender, EventArgs e)
        {
            pnlDeleteAllItems.Visible = false;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = btnFinalDelete.Enabled= false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtSerialNo.Text != "")
                {
                    if (txtReason.Text != "")
                    {
                        if (CheckDesignNameExistence())
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strQuery = " Delete from Items Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text
                                                + " Delete from ItemSecondary Where [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text
                                                + " Delete from StockMaster Where BillType='OPENING' and [BillCode]='" + txtSerialCode.Text + "' and [BillNo]=" + txtSerialNo.Text
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('DESIGNMASTER','" + txtSerialCode.Text + "'," + txtSerialNo.Text + ",'" + txtReason.Text + ".',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                                int count = dba.ExecuteMyQuery(strQuery);
                                if (count > 0)
                                {
                                    txtReason.Text = "";
                                    pnlDeletionConfirmation.Visible = false;
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                    MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    BindNextRecord();
                                    BindDesignNameWithList();
                                }
                                else
                                    MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReason.Focus();
                    }
                }
            }
            catch
            {
            }
            btnDelete.Enabled = btnFinalDelete.Enabled = true;
        }

        private void btnDataMirror_Click(object sender, EventArgs e)
        {
            btnDataMirror.Enabled = false;
            try
            {
                if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to data mapping ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        DataMapping(true);
                    }
                }
            }
            catch { }
            btnDataMirror.Enabled = true;
        }

        private void txtBuyerDesignNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
        }

        private void lBoxDesignName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GetSelectDesignFromList();
        }

        private void GetSelectDesignFromList()
        {
            try
            {
                if (lBoxDesignName.SelectedIndex >= 0)
                {
                    string strDesignName = Convert.ToString(lBoxDesignName.SelectedItem) ;
                    if (strDesignName != "" && dtDesignName!=null)
                    {
                        BindDesignWithSelectedDesign(strDesignName);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindDesignWithSelectedDesign(string strDesign)
        {
            string strSerialNo = "";
            DataRow[] rows = dtDesignName.Select("ItemName='" + strDesign + "'");
            if (rows.Length > 0)
            {
                strSerialNo = Convert.ToString(rows[0]["BillNo"]);
                txtSerialCode.Text = Convert.ToString(rows[0]["BillCode"]);
                BindRecordWithControl(strSerialNo);
                pnlSearch.Visible = false;
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtSerialCode.Text != "" && txtSerialNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("DESIGNMASTER", txtSerialCode.Text, txtSerialNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtSerialNo.Text != "")
                {
                   
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControl(txtSerialNo.Text);
                    }
                }
                else
                {
                    txtSerialNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMCATEGORYNAME", "SEARCH ITEM CATEGORY", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtCategory.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;
        }

        private void txtQtyRatio_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }
        
        private void txtDesignNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpecialChar(sender, e);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView)
            {
                if (!MainPage.mymainObject.bAccountMasterAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterView)
                    txtSerialNo.Enabled = false;

                btnPrint.Enabled = MainPage.mymainObject.bBarcodePrint;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(Close));
                return false;
            }
        }        

    }
}

