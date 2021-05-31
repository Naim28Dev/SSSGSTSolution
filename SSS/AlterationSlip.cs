using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class AlterationSlip : Form
    {
        DataBaseAccess dba;        
        public Color Green { get; private set; }
        bool _bNewStatus = false;
        string strInvoiceCode = "", strInvoiceNo = "";
        public AlterationSlip()
        {
            InitializeComponent();
            try
            {
                dba = new DataBaseAccess();
                txtSRCODE.Text = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select distinct AltrationCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'"));
                SetSerialNo();
                SetCategory();             
                BindLastRecord();
            }
            catch { }
        }

        public AlterationSlip(bool _Status, string InvoiceCode, string InvoiceNo)
        {
            InitializeComponent();
            try
            {
                dba = new DataBaseAccess();
                txtSRCODE.Text = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select distinct AltrationCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'"));
                SetSerialNo();
                SetCategory();
                _bNewStatus = _Status;
                strInvoiceCode = InvoiceCode;
                strInvoiceNo = InvoiceNo;
                GetSaleDetail();
            }
            catch { }
        }

        public AlterationSlip(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            BindAllDataWithControl(strSerialCode, strSerialNo);
        }

        private void GetSaleDetail()
        {
            try
            {
                string strQuery = " Select *,ISNULL((SalePartyID+' '+SName),SalePartyID) SParty,CONVERT(varchar,Date,103)BDate,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SB.Date))) LockType from SalesBook SB OUTER APPLY (Select Top 1 SM.Name as SName,NormalDhara from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.SalePartyID)SM1    Where BillCode='" + strInvoiceCode + "' and BillNo=" + strInvoiceNo + "    Select SBS.*,(StockQty+Qty)StockQty from SalesBookSecondary SBS OUTER APPLY (Select SUM(Qty)StockQty from (Select SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASE','OPENING','SALERETURN','STOCKIN') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select -SUM(Qty) Qty from StockMaster SM Where SM.BillType in ('PURCHASERETURN','SALES','STOCKOUT') and ISNULL(SM.BarCode,'')=ISNULL(SBS.BarCode,'') and ISNULL(SM.BrandName,'')=ISNULL(SBS.BrandName,'') and SM.ItemName=SBS.ItemName and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 UNION ALL Select 1000 Qty from Items _IM left join ItemSecondary _IS on _IM.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo Where _IM.Other='WITHOUT STOCK' and _IM.DisStatus=0  and ISNULL(_IS.Description,'')=ISNULL(SBS.BarCode,'') and _IM.ItemName=SBS.ItemName and _IS.Variant1=SBS.Variant1 and _IS.Variant2=SBS.Variant2)Stock)Stock Where BillCode='" + strInvoiceCode + "' and BillNo=" + strInvoiceNo + " order by SID ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    DataTable _dt = ds.Tables[1];
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        txtBillNo.Text = strInvoiceCode + " " + strInvoiceNo;
                        txtCustomerName.Text = Convert.ToString(row["SParty"]);
                        txtMobile.Text = Convert.ToString(row["MobileNo"]);
                        txtPendingAmt.Text = Convert.ToString(row["CreditAmt"]);
                    }

                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        dgrdAlteration.Rows.Clear();
                        int rowIndex = 0;
                        dgrdAlteration.Rows.Add(_dt.Rows.Count);
                        foreach (DataRow drow in _dt.Rows)
                        {
                            dgrdAlteration.Rows[rowIndex].Cells["SNo"].Value = rowIndex + 1;
                            dgrdAlteration.Rows[rowIndex].Cells["itemName"].Value = Convert.ToString(drow["ItemName"]);
                            dgrdAlteration.Rows[rowIndex].Cells["variant1"].Value = Convert.ToString(drow["Variant1"]);
                            dgrdAlteration.Rows[rowIndex].Cells["variant2"].Value = Convert.ToString(drow["Variant2"]);
                            dgrdAlteration.Rows[rowIndex].Cells["variant3"].Value = Convert.ToString(drow["Variant3"]);
                            dgrdAlteration.Rows[rowIndex].Cells["variant4"].Value = Convert.ToString(drow["Variant4"]);
                            dgrdAlteration.Rows[rowIndex].Cells["variant5"].Value = Convert.ToString(drow["Variant5"]);
                            dgrdAlteration.Rows[rowIndex].Cells["qty"].Value = Convert.ToString(drow["Qty"]);
                            dgrdAlteration.Rows[rowIndex].Cells["salesman"].Value = Convert.ToString(drow["SONumber"]);

                            rowIndex++;
                        }
                    }
                }
                SendKeys.Send("{TAB}");
            }
           catch(Exception ex)
            { }
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdAlteration.Columns["variant1"].HeaderText  = MainPage.StrCategory1;
                    dgrdAlteration.Columns["variant1"].Visible = true;
                }
                else
                    dgrdAlteration.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdAlteration.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdAlteration.Columns["variant2"].Visible =  true;
                }
                else
                    dgrdAlteration.Columns["variant2"].Visible =  false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdAlteration.Columns["variant3"].HeaderText =  MainPage.StrCategory3;
                    dgrdAlteration.Columns["variant3"].Visible =  true;
                }
                else
                    dgrdAlteration.Columns["variant3"].Visible =  false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdAlteration.Columns["variant4"].HeaderText =  MainPage.StrCategory4;
                    dgrdAlteration.Columns["variant4"].Visible =  true;
                }
                else
                    dgrdAlteration.Columns["variant4"].Visible =  false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdAlteration.Columns["variant5"].HeaderText =  MainPage.StrCategory5;
                    dgrdAlteration.Columns["variant5"].Visible =  true;
                }
                else
                    dgrdAlteration.Columns["variant5"].Visible =  false;
            }
            catch
            {
            }
        }


        private void BindLastRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MAX(SerialNo) from AlterationSlip Where SerialNo!=0 and SerialCode='" + txtSRCODE.Text + "'");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void BindFirstRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MIN(SerialNo) from AlterationSlip Where SerialNo!=0 and SerialCode='" + txtSRCODE.Text + "'");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void AlterationSlip_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
            {    if (panelSMS.Visible)
                     panelSMS.Visible = false;
                else
                    this.Close();                
            }
            else if (e.KeyCode == Keys.Enter && !dgrdAlteration.Focused)
            {
                SendKeys.Send("{TAB}");
            }
            else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (e.KeyCode == Keys.Home)
                {
                    BindFirstRecord();
                }
                else if (e.KeyCode == Keys.End)
                {
                    BindLastRecord();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    BindNextRecord();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    BindPreviousRecord();
                }
            }
        }

        private void BindNextRecord()
        {
            if (txtSNo.Text != "")
            {
                object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select Min(SerialNo) from AlterationSlip Where SerialNo>" + txtSNo.Text + "  and SerialCode='" + txtSRCODE.Text + "'");
                
                if (Convert.ToString(objSerialNo) != "")
                {
                    BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
                }
                else
                {
                    BindLastRecord();
                }
            }
        }

        private void BindPreviousRecord()
        {
            if (txtSNo.Text != "")
            {
                object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select Max(SerialNo) from AlterationSlip Where SerialNo<" + txtSNo.Text + "  and SerialCode='" + txtSRCODE.Text + "'");
                if (Convert.ToString(objSerialNo) != "")
                {
                    BindAllDataWithControl(txtSRCODE.Text, objSerialNo);
                }
                else
                {
                    BindFirstRecord();
                }
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("ID", typeof(String));
                myDataTable.Columns.Add("SerialCode", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("AltCode", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("OrderNO", typeof(String));
                myDataTable.Columns.Add("MobileNo", typeof(String));
                myDataTable.Columns.Add("DDate", typeof(String));
                myDataTable.Columns.Add("TotalQty", typeof(String));
                myDataTable.Columns.Add("Timing", typeof(String));
                myDataTable.Columns.Add("PendingAmt", typeof(String));
                myDataTable.Columns.Add("Remarks", typeof(String));
                myDataTable.Columns.Add("CustomerName", typeof(String));
                myDataTable.Columns.Add("ASSId", typeof(String));
                myDataTable.Columns.Add("ASSSerialNo", typeof(String));
                myDataTable.Columns.Add("ItemName", typeof(String));
                myDataTable.Columns.Add("Variant1", typeof(String));
                myDataTable.Columns.Add("Variant2", typeof(String));
                myDataTable.Columns.Add("Variant3", typeof(String));
                myDataTable.Columns.Add("Variant4", typeof(String));
                myDataTable.Columns.Add("Variant5", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("MasterName", typeof(String));
                myDataTable.Columns.Add("SalesManName", typeof(String));
                myDataTable.Columns.Add("AltType", typeof(String));
                myDataTable.Columns.Add("ItemStatus", typeof(String));
                myDataTable.Columns.Add("sID", typeof(String));                
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("HeaderName", typeof(String));

                foreach (DataGridViewRow dr in dgrdAlteration.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strCompanyName;
                    string strSRCode = Convert.ToString(txtSRCODE.Text) + " " + Convert.ToString(txtSNo.Text);
                    string strAltCode = Convert.ToString(txtAltCode.Text) + " " + Convert.ToString(txtAltNo.Text);
                    string strMOBNo = Convert.ToString(txtMobile.Text) + "," + Convert.ToString(txtMobileII.Text);
                    //row["ID"] = dr.Cells["ID"].Value;
                    row["SerialCode"] = strSRCode;
                    row["Date"] = txtDate.Text; 
                    row["AltCode"] = strAltCode;
                    row["BillNo"] = txtBillNo.Text;
                    row["OrderNo"] = txtOrderNo.Text;
                    row["MobileNo"] = strMOBNo;
                    row["DDate"] = txtDelivery.Text;
                    row["Timing"] = txtTime.Text;
                    row["TotalQty"] = lbltotalQty.Text;
                    row["PendingAmt"] = txtPendingAmt.Text;
                    row["Remarks"] = txtRemark.Text;
                    row["CustomerName"] = txtCustomerName.Text;
                    row["ASSID"] = dr.Cells["SNO"].Value;
                    row["ItemName"] = dr.Cells["itemName"].Value;
                    row["Variant1"] = dr.Cells["Variant1"].Value;
                    row["Variant2"] = dr.Cells["Variant2"].Value;
                    row["Variant3"] = dr.Cells["Variant3"].Value;
                    row["Variant4"] = dr.Cells["Variant4"].Value;
                    row["Variant5"] = dr.Cells["Variant5"].Value;
                    row["Qty"] = dr.Cells["qty"].Value;
                    row["MasterName"] = dr.Cells["mastername"].Value;
                    row["SalesManName"] = dr.Cells["salesman"].Value;
                    row["AltType"] = dr.Cells["alterationtype"].Value;
                    row["ItemStatus"] = dr.Cells["status"].Value;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                    row["Headername"] = "Alteration Slip";

                    myDataTable.Rows.Add(row);
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }

        private void BindAllDataWithControl(object objSerialCode, object objSerialNo)
        {
            try
            {
                lblCreatedBy.Text = "";
                dgrdAlteration.Rows.Clear();
                DataTable dt = dba.GetDataTable("Select *,ass.ID as SID,Convert(varchar,Date,103)_Date,Convert(varchar,DDate,103)_DDate from AlterationSlip aSlip inner join AlterationSlipSecondary ass on aSlip.SerialNo=ass.SerialNo and aSlip.SerialCode=ass.SerialCode  Where aSlip.SerialCode='" + objSerialCode + "' and aSlip.SerialNo=" + objSerialNo + " ");
                if (dt.Rows.Count > 0)
                {
                    DisableAllControls();

                    DataRow row = dt.Rows[0];
                    txtSRCODE.Text = Convert.ToString(row["SerialCode"]);
                    txtSNo.Text = Convert.ToString(row["SerialNo"]);
                    txtDate.Text = Convert.ToString(row["_Date"]);
                    txtAltCode.Text = Convert.ToString(row["AltCode"]);
                    txtAltNo.Text = Convert.ToString(row["AltNo"]);
                    txtCustomerName.Text = Convert.ToString(row["CustomerName"]);
                    txtBillNo.Text = Convert.ToString(row["BillNo"]);
                    txtOrderNo.Text = Convert.ToString(row["OrderNo"]);
                    txtMobile.Text = Convert.ToString(row["MobileNoI"]);
                    txtMobileII.Text = Convert.ToString(row["MobileNoII"]);
                    txtTime.Text = Convert.ToString(row["Timing"]);
                    txtPendingAmt.Text = Convert.ToString(row["PendingAmt"]);
                    txtRemark.Text = Convert.ToString(row["Remark"]);
                    lbltotalQty.Text = Convert.ToString(row["TotalQty"]);
                    txtDelivery.Text = Convert.ToString(row["_DDate"]);
                    if (txtAltNo.Text == "0")
                        txtAltNo.Clear();

                    DataRow[] rows = dt.Select(String.Format("AltType='ALTERATION' OR AltType='FINISHING' OR AltType='READY' "));
                    if (rows.Length > 0)
                    {
                        dgrdAlteration.Rows.Add(rows.Length);
                        int index = 0;
                        foreach (DataRow dr in rows)
                        {
                            dgrdAlteration.Rows[index].Cells["SNo"].Value = index + 1;
                            dgrdAlteration.Rows[index].Cells["barCode"].Value = dr["Barcode"];
                            dgrdAlteration.Rows[index].Cells["itemName"].Value = dr["ItemName"];
                            dgrdAlteration.Rows[index].Cells["variant1"].Value = dr["Variant1"];
                            dgrdAlteration.Rows[index].Cells["variant2"].Value = dr["Variant2"];
                            dgrdAlteration.Rows[index].Cells["variant3"].Value = dr["Variant3"];
                            dgrdAlteration.Rows[index].Cells["variant4"].Value = dr["Variant4"];
                            dgrdAlteration.Rows[index].Cells["variant5"].Value = dr["Variant5"];
                            dgrdAlteration.Rows[index].Cells["qty"].Value = dr["Qty"];
                            dgrdAlteration.Rows[index].Cells["masterName"].Value = dr["MasterName"];
                            dgrdAlteration.Rows[index].Cells["salesMan"].Value = dr["SalesManName"];
                            dgrdAlteration.Rows[index].Cells["alterationType"].Value = dr["AltType"];
                            dgrdAlteration.Rows[index].Cells["status"].Value = dr["Itemstatus"];
                            dgrdAlteration.Rows[index].Cells["altID"].Value = dr["SID"];
                            index++;
                        }
                    }

                    string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                    if (strCreatedBy != "")
                        lblCreatedBy.Text = "Created By : " + strCreatedBy;
                    if (strUpdatedBy != "")
                        lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;


                }
                else
                {
                    ClearAllText();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSNo_KeyPress(object sender, KeyPressEventArgs e)
        {

            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey)  || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
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

        private void txtDelivery_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
                DateTime _dDate = dba.ConvertDateInExactFormat(txtDelivery.Text), _date = dba.ConvertDateInExactFormat(txtDate.Text);
                if (_dDate < _date)
                {
                    MessageBox.Show("Delivery date must be greater than alteration date", "Date out of Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDelivery.Focus();
                }
            }
        }       

        private void dgrdAlteration_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0)
                    {
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
                    {
                        SearchCategory_Custom objData = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE", "", "", "", "","","","",Keys.Space,true,false,"ItemName");
                        objData.ShowDialog();
                        GetAllDesignSizeColor(objData, e.RowIndex);
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 9)
                    {
                        SearchData objData = new SearchData("MASTERNAME", "SELECT MASTER NAME",Keys.Space);
                        objData.ShowDialog();
                        dgrdAlteration.CurrentCell.Value = objData.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 10)
                    {
                        SearchData objSearch = new SearchData("SALESMANNAME", "SEARCH SALES MAN", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdAlteration.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 11)
                    {
                        SearchData objData = new SearchData("ALTTYPE", "SELECT ALTERATION TYPE",Keys.Space);
                        objData.ShowDialog();
                        string strData = objData.strSelectedData;
                        if (strData != "")
                        {
                            dgrdAlteration.CurrentCell.Value = strData;
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 12)
                    {
                        SearchData objData = new SearchData("ALTSTATUS", "SELECT ALTERATION STATUS", Keys.Space);
                        objData.ShowDialog();
                        string strData = objData.strSelectedData;
                        if (strData != "")
                        {
                            dgrdAlteration.CurrentCell.Value = strData;
                        }
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void GetAllDesignSizeColor(SearchCategory_Custom objCategory, int rowIndex)
        {
            try
            {
                bool firstRow=false;
                if (objCategory != null)
                {
                    if (objCategory.dgrdDetails.Rows.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData == "")
                        {
                            foreach (DataGridViewRow dr in objCategory.dgrdDetails.Rows)
                            {
                                if (strData != "ADD NEW DESIGNNAME NAME")
                                {
                                    string selDesc = Convert.ToString(dr.Cells["DESIGNNAMEWITHBARCODE"].Value);
                                    string[] strAllItem = selDesc.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdAlteration.Rows.Add();
                                        else
                                            firstRow = true;
                                        dgrdAlteration.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                        dgrdAlteration.Rows[rowIndex].Cells["itemName"].Value = strAllItem[1];

                                        if (strAllItem.Length > 1)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant1"].Value = strAllItem[2];
                                        if (strAllItem.Length > 2)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant2"].Value = strAllItem[3];
                                        if (strAllItem.Length > 3)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                                        if (strAllItem.Length > 4)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                                        if (strAllItem.Length > 5)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];

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
                            dgrdAlteration.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                            dgrdAlteration.Rows[rowIndex].Cells["itemName"].Value = strAllItem[1];
                            if (strAllItem.Length > 2)
                                dgrdAlteration.Rows[rowIndex].Cells["variant1"].Value = strAllItem[2];
                            if (strAllItem.Length > 3)
                                dgrdAlteration.Rows[rowIndex].Cells["variant2"].Value = strAllItem[3];
                            if (strAllItem.Length > 4)
                                dgrdAlteration.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                            if (strAllItem.Length > 5)
                                dgrdAlteration.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                            if (strAllItem.Length > 6)
                                dgrdAlteration.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];
                       }
                    }

                    ArrangeSerialNo();

                    //if (Convert.ToString(dgrdAlteration.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex == dgrdAlteration.Rows.Count - 1)
                    //{
                    //    dgrdAlteration.Rows.Add(1);
                    //    dgrdAlteration.Rows[dgrdAlteration.RowCount - 1].Cells["SNo"].Value = dgrdAlteration.Rows.Count;
                    //    dgrdAlteration.CurrentCell = dgrdAlteration.Rows[dgrdAlteration.RowCount - 1].Cells["itemName"];
                    //    dgrdAlteration.Focus();
                    //}
                }
            }
            }
            catch(Exception ex)
            {
            }
        }

    private void GetAllReadyDesignSizeColor(SearchCategory objCategory, int rowIndex)
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
                                string[] strAllItem = strItem.Split('|');
                                if (strItem != "ADD NEW DESIGNNAME NAME")
                                {
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdAlteration.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdAlteration.Rows[rowIndex].Cells["ItemName"].Value = strAllItem[0];

                                        if (strAllItem.Length > 1)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (strAllItem.Length > 2)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (strAllItem.Length > 3)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (strAllItem.Length > 4)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (strAllItem.Length > 5)
                                            dgrdAlteration.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

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
                                dgrdAlteration.Rows[rowIndex].Cells["ItemName"].Value = strAllItem[0];
                                if (strAllItem.Length > 1)
                                    dgrdAlteration.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                if (strAllItem.Length > 2)
                                    dgrdAlteration.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                if (strAllItem.Length > 3)
                                    dgrdAlteration.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                if (strAllItem.Length > 4)
                                    dgrdAlteration.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                if (strAllItem.Length > 5)
                                    dgrdAlteration.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];
                            }
                        }

                        ArrangeSerialNo();

                    //    if (Convert.ToString(dgrdAlteration.Rows[rowIndex].Cells["ItemName"].Value) != "" && rowIndex == dgrdAlteration.Rows.Count - 1)
                    //    {
                    //        dgrdAlteration.Rows.Add(1);
                    //        dgrdAlteration.Rows[dgrdAlteration.RowCount - 1].Cells["Sno"].Value = dgrdAlteration.Rows.Count;
                    //        dgrdAlteration.CurrentCell = dgrdAlteration.Rows[dgrdAlteration.RowCount - 1].Cells["ItemName"];
                    //        dgrdAlteration.Focus();
                    //    }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetItemType(object objItem, DataGridViewRow row,string strLowerColumn, string strCName)
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(COUNT(*),0)  from ItemMaster Where Category not in ('LOWER','OTHER') and ItemName='" + objItem + "' ");
                if (Convert.ToDouble(objValue) > 0)
                {
                    row.Cells[strLowerColumn].Value = "";
                    row.Cells[strCName].Value = true;
                }
                else
                {
                    row.Cells[strLowerColumn].Value = "N/A";
                    row.Cells[strCName].Value = false;
                }
            }
            catch
            {
            }
        }

        private void EnableAllControls()
        {
            foreach (Control txt in grpBasic.Controls)
            {
                if (txt is TextBox)
                {
                    ((TextBox)txt).ReadOnly = false;
                }
            }
            txtAltCode.ReadOnly = txtAltNo.ReadOnly = txtBillNo.ReadOnly = txtDelivery.ReadOnly = txtTime.ReadOnly = dgrdAlteration.ReadOnly= false;
            txtSRCODE.ReadOnly = txtSNo.ReadOnly = true;
            dgrdAlteration.Enabled = true;
            
        }

        private void DisableAllControls()
        {
            foreach (Control txt in grpBasic.Controls)
            {
                if (txt is TextBox)
                {
                    (txt as TextBox).ReadOnly = true;
                }
            }
           
            //dgrdAlteration.Enabled = false;
        }

        private void ClearAllText()
        {
            try
            {
                //txtAltNo.Clear();
                txtBillNo.Clear();
                txtMobile.Clear();
                txtMobileII.Clear();
                txtOrderNo.Clear();
                txtPendingAmt.Clear();
                txtRemark.Clear();
                txtCustomerName.Clear();
                txtTime.Text = "00";
                lbltotalQty.Text = "0";

                if (DateTime.Today > MainPage.startFinDate)
                {
                    txtDate.Text = txtDelivery.Text = DateTime.Today.ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDate.Text = txtDelivery.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                }
                dgrdAlteration.Rows.Clear();
                dgrdAlteration.Rows.Add();
                dgrdAlteration.Rows[0].Cells[0].Value = 1;
                //dgrdAlteration.Rows[0].Cells["qty"].Value = 1;
                
            }
            catch
            {
            }
        }

        private bool ValidateControls()
        {
            if (txtSNo.Text == "")
            {
                MessageBox.Show(" Sorry ! Serial no can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSNo.Focus();
                return false;
            }
            if (txtAltCode.Text == "")
            {
                MessageBox.Show(" Sorry ! Alteration code can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAltCode.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show(" Sorry ! Date can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtDelivery.Text.Length!=10)
            {
                MessageBox.Show(" Sorry ! Delivery Date can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDelivery.Focus();
                return false;
            }
            if (txtMobile.Text.Length != 10 && txtCustomerName.Text=="")
            {
                MessageBox.Show(" Sorry ! Customer name or mobile no can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            foreach (DataGridViewRow row in dgrdAlteration.Rows)
            {
                if (Convert.ToString(row.Cells["itemName"].Value) == "")
                {
                    dgrdAlteration.Rows.RemoveAt(row.Index);
                }
                else if (Convert.ToString(row.Cells["qty"].Value) == "")
                {
                    MessageBox.Show(" Sorry ! Qty can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdAlteration.CurrentCell = row.Cells["qty"];
                    dgrdAlteration.Focus();
                    return false;
                }
                else if (Convert.ToString(row.Cells["alterationType"].Value) == "")
                {
                    MessageBox.Show(" Sorry ! Alteration type can't be blank ....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdAlteration.CurrentCell = row.Cells["alterationType"];
                    dgrdAlteration.Focus();
                    return false;
                }
            }

            if (dgrdAlteration.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Please fill atleast one row for saving ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdAlteration.Rows.Add();
                dgrdAlteration.Rows[0].Cells[0].Value = 1;
                dgrdAlteration.Focus();
                return false;
            }
            else
                ArrangeSerialNo();

            return true;
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtAltCode.Text == "")
                    txtAltCode.Text = "A";

                DataTable _dt = dba.GetDataTable("Select (ISNULL(MAX(SerialNo),0)+1)SerialNo,(Select (ISNULL(MAX(_ALT.AltNo),0)+1)AltNo from AlterationSlip _ALT Where _ALT.AltCode='" + txtAltCode.Text + "') AltNo from AlterationSlip   ");
                if (_dt.Rows.Count > 0)
                {
                    txtSNo.Text = Convert.ToString(_dt.Rows[0]["SerialNo"]);
                    txtAltNo.Text = Convert.ToString(_dt.Rows[0]["AltNo"]);
                }
            }
            catch
            {
            }
        }

        private bool CheckAltNoAvailability()
        {
            if (txtAltCode.Text != "" && txtAltNo.Text != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(COUNT(*),0) from AlterationSlip Where AltCode='" + txtAltCode.Text + "' and AltNo=" + txtAltNo.Text + " and SerialNo!=" + txtSNo.Text + "");
                int result = Convert.ToInt32(objValue);
                if (result > 0)
                {
                    MessageBox.Show("This alteration no is already exist ! ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAltNo.Focus();
                    return false;
                }
            }
            //else
            //{
            //    MessageBox.Show("Sorry ! Alteration no can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            return true;
        }

        private void txtAltCode_Leave(object sender, EventArgs e)
        {
            //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //{
            //    CheckAltNoAvailability();
            //}
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        btnEdit.Text = "&Edit";
                    }
                    btnAdd.Text = "&Save";
                    ClearAllText();
                    EnableAllControls();
                    SetSerialNo();
                    txtDate.Focus();
                    dgrdAlteration.Columns["status"].Visible = false;
                    txtSNo.ReadOnly = false;
                    btnEdit.Enabled = btnDelete.Enabled = false;
                }
                else if (ValidateControls() && CheckAltNoAvailability())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to save this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                    else
                    {
                       
                        if (dgrdAlteration.Rows.Count == 0)
                        {
                            dgrdAlteration.Rows.Add();
                            dgrdAlteration.Rows[0].Cells[0].Value = 1;
                           // dgrdAlteration.Rows[0].Cells["qty"].Value = 1;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveRecord()
        {
            try
            {
                
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text), dDate = dba.ConvertDateInExactFormat(txtDelivery.Text);
                string strQuery = " INSERT INTO [dbo].[AlterationSlip]([SerialCode],[SerialNo],[Date],[AltCode],[AltNo],[BillNo],[OrderNo],[MobileNoI],[MobileNoII],[DDate],[TotalQty],[Timing],[PendingAmt],[Remark],[CustomerName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus])VALUES "
                                      + " ('" + txtSRCODE.Text + "'," + txtSNo.Text + ",'" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + txtAltCode.Text + "','" + txtAltNo.Text + "','" + txtBillNo.Text + "','" + txtOrderNo.Text + "', '" + txtMobile.Text + "', " 
                                      + " '" + txtMobileII.Text + "','" + dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'," + dba.ConvertObjectToDouble(lbltotalQty.Text) + ",'"+txtTime.Text +"',"+ConvertObjectToDouble(txtPendingAmt.Text)+",'"+txtRemark.Text+"','"+txtCustomerName.Text+"','" + MainPage.strLoginName + "','',1,0);";

                foreach (DataGridViewRow row in dgrdAlteration.Rows)
                {
                    strQuery += " INSERT INTO [dbo].[AlterationSlipSecondary]([SerialCode],[SerialNo],[Barcode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MasterName],[SalesManName],[AltType],[ItemStatus]) VALUES "
                                  + " ('" + txtSRCODE.Text + "'," + txtSNo.Text + ",'" + row.Cells["barCode"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["qty"].Value) + ",'" + row.Cells["masterName"].Value + "','" + row.Cells["salesMan"].Value + "', "
                                  + " '" + row.Cells["alterationType"].Value + "','PENDING') ";
                }

               

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('ALTERATIONSLIP','" + txtSRCODE.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),"+dba.ConvertObjectToDouble(lbltotalQty.Text)+",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

               

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record saved successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    BindLastRecord();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
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
                    
                    if (dgrdAlteration.Rows.Count == 0)
                    {
                        dgrdAlteration.Rows.Add();
                        dgrdAlteration.Rows[0].Cells[0].Value = 1;
                      //  dgrdAlteration.Rows[0].Cells["qty"].Value = 1;
                    }
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to update this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        UpdateRecord();
                    }
                    else
                    {
                       
                        if (dgrdAlteration.Rows.Count == 0)
                        {
                            dgrdAlteration.Rows.Add();
                            dgrdAlteration.Rows[0].Cells[0].Value = 1;
                          //  dgrdAlteration.Rows[0].Cells["qty"].Value = 1;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdateRecord()
        {
            try
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text), dDate = dba.ConvertDateInExactFormat(txtDelivery.Text);
                string strQuery = " Update [dbo].[AlterationSlip] Set  [Date]='" + sDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[AltCode]='" + txtAltCode.Text + "',[AltNo]='" + txtAltNo.Text + "',[BillNo]='" + txtBillNo.Text + "',[OrderNo]='" + txtOrderNo.Text + "',[MobileNoI]='" + txtMobile.Text + "',[MobileNoII]='" + txtMobileII.Text + "', "
                                       + " [DDate]='" + dDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[TotalQty]='" + lbltotalQty.Text + "',[Timing]='" + txtTime.Text + "',[PendingAmt]=" + ConvertObjectToDouble(txtPendingAmt.Text) + ",[Remark]='" + txtRemark.Text + "',[CustomerName]='"+txtCustomerName.Text+ "',[UpdatedBy]='" + MainPage.strLoginName + "' Where SerialNo=" + txtSNo.Text + " ";

                foreach (DataGridViewRow row in dgrdAlteration.Rows)
                {
                    string strAID=Convert.ToString(row.Cells["altID"].Value);
                    if (strAID != "")
                    {
                        strQuery += " Update [dbo].[AlterationSlipSecondary] SET [Barcode]='" + row.Cells["barCode"].Value + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Qty]='" + row.Cells["qty"].Value + "',[MasterName]='" + row.Cells["masterName"].Value + "',[SalesManName]='" + row.Cells["salesMan"].Value + "',[AltType]='" + row.Cells["alterationType"].Value + "', " + "[ItemStatus]='" + row.Cells["status"].Value + "', "
                                      + " [Variant1]='" + row.Cells["variant1"].Value + "',[variant2]='" + row.Cells["variant2"].Value + "',[variant3]='" + row.Cells["variant3"].Value + "',[variant4]='" + row.Cells["variant4"].Value + "',[variant5]='" + row.Cells["variant5"].Value + "' Where SerialNo='" + txtSNo.Text + "' and ID=" + strAID + " ";
                    }
                    else
                    {
                        strQuery += " INSERT INTO [dbo].[AlterationSlipSecondary]([SerialNo],[ItemName],[Qty],[MasterName],[SalesManName],[AltType],[ItemStatus],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5]) VALUES "
                                    + " (" + txtSNo.Text + ",'" + row.Cells["itemName"].Value + "','" + row.Cells["qty"].Value + "','" + row.Cells["masterName"].Value + "','" + row.Cells["salesMan"].Value + "','" + row.Cells["alterationType"].Value + "','" + row.Cells["status"].Value + "', '" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "') ";
                    }
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('ALTERATIONSLIP','" + txtSRCODE.Text + "'," + txtSNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lbltotalQty.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record updated successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    DisableAllControls();
                    BindAllDataWithControl(txtSRCODE.Text, txtSNo.Text);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgrdAlteration_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int currentRow = 0;
                int indexColumn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdAlteration.CurrentCell.RowIndex;
                    indexColumn = dgrdAlteration.CurrentCell.ColumnIndex;
                    if (Index < dgrdAlteration.RowCount - 1)
                        currentRow = Index - 1;
                    else
                        currentRow = Index;
                    if (indexColumn < dgrdAlteration.ColumnCount - 4)
                    {
                        indexColumn += 1;
                        if (currentRow >= 0)
                        {
                            if (!dgrdAlteration.Columns[indexColumn].Visible && indexColumn < dgrdAlteration.ColumnCount - 1)
                                indexColumn++;
                            if (!dgrdAlteration.Columns[indexColumn].Visible && indexColumn < dgrdAlteration.ColumnCount - 1)
                                indexColumn++;
                            if (!dgrdAlteration.Columns[indexColumn].Visible && indexColumn < dgrdAlteration.ColumnCount - 1)
                                indexColumn++;
                            if (!dgrdAlteration.Columns[indexColumn].Visible && indexColumn < dgrdAlteration.ColumnCount - 1)
                                indexColumn++;
                            if (!dgrdAlteration.Columns[indexColumn].Visible && indexColumn < dgrdAlteration.ColumnCount - 1)
                                indexColumn++;
                           

                            dgrdAlteration.CurrentCell = dgrdAlteration.Rows[currentRow].Cells[indexColumn];
                        }
                    }
                    else if (Index == dgrdAlteration.RowCount - 1)
                    {
                        string strItem = Convert.ToString(dgrdAlteration.Rows[currentRow].Cells["itemName"].Value);//strQty = Convert.ToString(dgrdAlteration.Rows[currentRow].Cells["qty"].Value);

                        if (strItem != "" )//&& strQty != "")
                        {
                            dgrdAlteration.Rows.Add(1);
                            int rowIndex = dgrdAlteration.Rows.Count;
                            dgrdAlteration.Rows[rowIndex - 1].Cells["SNo"].Value = dgrdAlteration.Rows.Count;
                            dgrdAlteration.Rows[rowIndex - 1].Cells["masterName"].Value = dgrdAlteration.Rows[rowIndex - 2].Cells["masterName"].Value;
                            dgrdAlteration.Rows[rowIndex - 1].Cells["salesMan"].Value = dgrdAlteration.Rows[rowIndex - 2].Cells["salesMan"].Value;
                            dgrdAlteration.Rows[rowIndex - 1].Cells["alterationType"].Value = dgrdAlteration.Rows[rowIndex - 2].Cells["alterationType"].Value;
                           // dgrdAlteration.Rows[rowIndex - 1].Cells["qty"].Value = 1;

                            dgrdAlteration.CurrentCell = dgrdAlteration.Rows[currentRow + 1].Cells[1];
                            dgrdAlteration.Focus();
                        }
                       
                    }
                    else
                    {
                        dgrdAlteration.CurrentCell = dgrdAlteration.Rows[dgrdAlteration.RowCount - 1].Cells[1];
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    if (btnAdd.Text == "&Save")
                    {
                        //if (Convert.ToString(dgrdAlteration.CurrentRow.Cells["itemName"].Value) =="" || Convert.ToString(dgrdAlteration.CurrentRow.Cells["qty"].Value) == "")
                        //{ dgrdAlteration.Rows.RemoveAt(dgrdAlteration.CurrentRow.Index); }
                        dgrdAlteration.Rows.RemoveAt(dgrdAlteration.CurrentRow.Index);
                        ArrangeSerialNo();
                        if (dgrdAlteration.RowCount == 0)
                        {
                            dgrdAlteration.Rows.Add(1);
                            dgrdAlteration.Rows[0].Cells[0].Value = 1;
                        }
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdAlteration.CurrentRow.Cells["altID"].Value);
                        if (strID == "")
                        {
                            dgrdAlteration.Rows.RemoveAt(dgrdAlteration.CurrentRow.Index);
                            ArrangeSerialNo();
                            if (dgrdAlteration.RowCount == 0)
                            {
                                dgrdAlteration.Rows.Add(1);
                                dgrdAlteration.Rows[0].Cells[0].Value = 1;
                            }
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                DeleteAltCurrentRow(strID);
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    {
                        if (dgrdAlteration.CurrentCell.ColumnIndex != 0 && dgrdAlteration.CurrentCell.ColumnIndex != 8)
                        {
                            dgrdAlteration.CurrentCell.Value = "";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteAltCurrentRow(string strID)
        {
            string strQuery = " Delete from AlterationSlipSecondary Where SerialNo=" + txtSNo.Text + " and ID=" + strID + " ";
            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                dgrdAlteration.Rows.RemoveAt(dgrdAlteration.CurrentRow.Index);
                ArrangeSerialNo();
                if (dgrdAlteration.RowCount == 0)
                {
                    dgrdAlteration.Rows.Add(1);
                    dgrdAlteration.Rows[0].Cells[0].Value = 1;
                }
            }
        }

       



        private void ArrangeSerialNo()
        {
            try
            {
                int rowIndex = 1;
                double dQty = 0;
                foreach (DataGridViewRow row in dgrdAlteration.Rows)
                {
                    row.Cells[0].Value = rowIndex;
                    dQty += ConvertObjectToDouble(row.Cells["qty"].Value);
                    rowIndex++;
                }
                rowIndex = 1;
               
                lbltotalQty.Text = dQty.ToString("0");
            }
            catch
            {
            }
        }


        private void CalculateTotalQty()
        {
            try
            {
                double dQty = 0;
                foreach (DataGridViewRow row in dgrdAlteration.Rows)
                {
                    dQty += ConvertObjectToDouble(row.Cells["qty"].Value);
                }
               
                lbltotalQty.Text = dQty.ToString("0");
            }
            catch
            {
            }
        }


        private double ConvertObjectToDouble(object objAmt)
        {
            double dAmount = 0;
            try
            {
                if (objAmt != null)
                {
                    dAmount = Convert.ToDouble(objAmt);
                }
            }
            catch
            {
            }
            return dAmount;
        }

       

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && txtSNo.Text != "")
                {
                        panelSMS.Visible = true;
                        txtReason.Focus();
                        btnDelete.Enabled = false;
                        //string strQuery = " Delete from AlterationSlip Where SerialNo=" + txtSNo.Text + "   Delete from AlterationSlipSecondary Where SerialNo=" + txtSNo.Text + " ";
                        //int count = dba.ExecuteMyQuery(strQuery);
                        //if (count > 0)
                        //{
                        //    MessageBox.Show("Thank you ! Record Delete successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        //    BindNextRecord();
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Sorry ! Unable to Delete, Please try after some time ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //}
                    }
                
            }
            catch
            {
            }
        }

        private void dgrdAlteration_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                e.CellStyle.BackColor = Color.White;
                if (dgrdAlteration.CurrentCell.ColumnIndex == 5 || dgrdAlteration.CurrentCell.ColumnIndex == 12 || dgrdAlteration.CurrentCell.ColumnIndex == 8)
                {
                    TextBox txtBox = e.Control as TextBox;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdAlteration.CurrentCell.ColumnIndex == 5 || dgrdAlteration.CurrentCell.ColumnIndex == 8 )
            {
                Char pressedKey = e.KeyChar;
                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else if ( dgrdAlteration.CurrentCell.ColumnIndex == 12)
            {
                Char pressedKey = e.KeyChar;
                if (Convert.ToString(dgrdAlteration.CurrentCell.Value) == "" && Char.IsWhiteSpace(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

     

        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdAlteration.Rows.Count > 0 )
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        Reporting.CryAlterationSlip objReport = new Reporting.CryAlterationSlip();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                        {
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;
                            objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objReport.PrintToPrinter(1, false, 0, 0);
                        }
                        objReport.Close();
                        objReport.Dispose();
                        btnPreview.Enabled = true;
                        
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnPreview.Enabled = true;
                }
            }
            catch
            {
            }
        }   

        private void txtAltCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text.Length == 0 && Char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTime_Enter(object sender, EventArgs e)
        {
            //if (txtTime.Text == "00" || !txtTime.ReadOnly)
            //{
            //    txtTime.Clear();
            //}
        }

        private void txtTime_Leave(object sender, EventArgs e)
        {
            if (txtTime.Text == "" || txtTime.Text == "0")
            {
                txtTime.Text="";
            }            
        }

        private void AlterationSlip_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (_bNewStatus)
                    {
                        btnAdd.PerformClick();
                        if (strInvoiceCode != "" && strInvoiceNo != "")                      
                            GetSaleDetail();                        
                        txtAltNo.Focus();

                    }
                }
               
            }
            catch { }

        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView)
            {
                if (!MainPage.mymainObject.bSaleAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bSaleEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bSaleView)
                    txtBillNo.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return false;
            }
        }

        private void txtPendingAmt_Leave(object sender, EventArgs e)
        {
           // dgrdAlteration.Focus();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgrdAlteration.Rows.Count > 0 )
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0 && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        Reporting.CryAlterationSlip objReport = new Reporting.CryAlterationSlip();
                        objReport.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("ALTERATION SLIP PREVIEW");
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void txtSNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (txtSNo.Text != "")
                    BindAllDataWithControl(txtSRCODE.Text,txtSNo.Text);
                else
                    ClearAllText();
            }
            else if (txtSNo.Text != "")
                CheckAvailability();
        }

        public int CheckAlterationSlipAvailability(string strSerialCode, string strSerialNo)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select Count(*) from AlterationSlip where SerialCode='" + txtSRCODE.Text + "' and SerialNo=" + txtSNo.Text + "", MainPage.con);
            adap.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0][0]) != "")
                {
                    count = Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            return count;
        }

        private bool CheckAvailability()
        {
            bool chk = true;
            try
            {
                if (btnAdd.Text == "&Save" && !txtSNo.ReadOnly)
                {
                    if (txtSNo.Text != "")
                    {
                        int check = CheckAlterationSlipAvailability(txtSRCODE.Text, txtSNo.Text);
                        if (check < 1)
                        {
                            lblserial.Text = txtSNo.Text + "  S.R. No is Available ........";
                            lblserial.ForeColor = Color.Green;
                            lblserial.Visible = true;
                            chk = true;
                        }
                        else
                        {
                            lblserial.Text = txtSNo.Text + " S.R. No is Already exist ! ";
                            lblserial.ForeColor = Color.Red;
                            lblserial.Visible = true;
                            txtSNo.Focus();
                            chk = false;
                        }
                    }

                    else
                    {
                        lblserial.Text = "Please Choose S.R. Number .......";
                        lblserial.ForeColor = Color.Red;
                        lblserial.Visible = true;
                        txtSNo.Focus();
                        chk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtBillNo.Focus();
            }
            return chk;
        }

        private bool CheckAvailabilityAltNo()
        {
            bool chk = true;
            try
            {
                if (btnAdd.Text == "&Save" && !txtAltNo.ReadOnly)
                {
                    if (txtAltNo.Text != "")
                    {
                        int count = 0;
                        DataTable dt = new DataTable();
                        SqlDataAdapter adap = new SqlDataAdapter("Select Count(*) from AlterationSlip where AltCode='" + txtAltCode.Text + "' and AltNo=" + txtAltNo.Text + "", MainPage.con);
                        adap.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (Convert.ToString(dt.Rows[0][0]) != "")
                            {
                                count = Convert.ToInt32(dt.Rows[0][0]);
                            }
                        }
                        if (count < 1)
                        {
                            lblserial.Text = txtAltNo.Text + "  Alt. No is Available ........";
                            lblserial.ForeColor = Color.Green;
                            lblserial.Visible = true;
                            chk = true;
                        }
                        else
                        {
                            lblserial.Text = txtAltNo.Text + " Alt. No is Already exist ! ";
                            lblserial.ForeColor = Color.Red;
                            lblserial.Visible = true;
                            txtAltNo.Focus();
                            chk = false;
                        }
                    }

                    else
                    {
                        lblserial.Text = "Please Choose Alt. Number .......";
                        lblserial.ForeColor = Color.Red;
                        lblserial.Visible = true;
                        txtAltNo.Focus();
                        chk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtBillNo.Focus();
            }
            return chk;
        }

        private bool CheckAvailabilityBillNo()
        {
            bool chk = true;
            try
            {
                if (btnAdd.Text == "&Save" && !txtBillNo.ReadOnly)
                {
                    if (txtBillNo.Text != "")
                    {
                        int count = 0;
                        DataTable dt = new DataTable();
                        SqlDataAdapter adap = new SqlDataAdapter("Select Count(*) from AlterationSlip where BillNo='" + txtBillNo.Text + "' and SerialCode='" + txtSRCODE.Text + "'", MainPage.con);
                        adap.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (Convert.ToString(dt.Rows[0][0]) != "")
                            {
                                count = Convert.ToInt32(dt.Rows[0][0]);
                            }
                        }
                        if (count < 1)
                        {
                            lblserial.Text = txtBillNo.Text + "  Bill No is Available ........";
                            lblserial.ForeColor = Color.Green;
                            lblserial.Visible = true;
                            chk = true;
                        }
                        else
                        {
                            lblserial.Text = txtBillNo.Text + " Bill. No is Already exist ! ";
                            lblserial.ForeColor = Color.Red;
                            lblserial.Visible = true;
                            txtBillNo.Focus();
                            chk = false;
                        }
                    }

                    else
                    {
                        lblserial.Text = "Please Choose Bill Number .......";
                        lblserial.ForeColor = Color.Red;
                        lblserial.Visible = true;
                        txtBillNo.Focus();
                        chk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtBillNo.Focus();
            }
            return chk;
        }

        private bool CheckAvailabilityOrderNo()
        {
            bool chk = true;
            try
            {
                if (btnAdd.Text == "&Save" && !txtOrderNo.ReadOnly)
                {
                    if (txtOrderNo.Text != "")
                    {
                        int count = 0;
                        DataTable dt = new DataTable();
                        SqlDataAdapter adap = new SqlDataAdapter("Select Count(*) from AlterationSlip where OrderNo='" + txtOrderNo.Text + "' and SerialCode='" + txtSRCODE.Text + "'", MainPage.con);
                        adap.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (Convert.ToString(dt.Rows[0][0]) != "")
                            {
                                count = Convert.ToInt32(dt.Rows[0][0]);
                            }
                        }
                        if (count < 1)
                        {
                            lblserial.Text = txtOrderNo.Text + "  Order No is Available ........";
                            lblserial.ForeColor = Color.Green;
                            lblserial.Visible = true;
                            chk = true;
                        }
                        else
                        {
                            lblserial.Text = txtOrderNo.Text + " Order No is Already exist ! ";
                            lblserial.ForeColor = Color.Red;
                            lblserial.Visible = true;
                            txtOrderNo.Focus();
                            chk = false;
                        }
                    }

                    else
                    {
                        lblserial.Text = "Please Choose Order Number .......";
                        lblserial.ForeColor = Color.Red;
                        lblserial.Visible = true;
                        txtOrderNo.Focus();
                        chk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtOrderNo.Focus();
            }
            return chk;
        }

       

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (txtReason.Text != "")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Delete record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                        string strQuery = " Delete from AlterationSlip Where SerialNo=" + txtSNo.Text + "   Delete from AlterationSlipSecondary Where SerialNo=" + txtSNo.Text
                            + " INSERT INTO RemovalReason VALUES('ALTERATIONSLIP','" + txtSRCODE.Text + "','" + txtSNo.Text + "','" + txtReason.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record Delete successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            BindLastRecord();
                            panelSMS.Visible = false;
                            txtReason.Clear();
                            btnDelete.Enabled = true;
                    }
                        else
                        {
                            MessageBox.Show("Sorry ! Unable to Delete, Please try after some time ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                }
            }
            else
            {
                MessageBox.Show("Reason should not be blank...Please Fill the Valid Reason...");
                txtReason.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelSMS.Visible = false;
            txtReason.Clear();
            btnDelete.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtSNo.ReadOnly = txtSRCODE.ReadOnly = false;
            txtSRCODE.Enabled = true;
            BindLastRecord();
            dgrdAlteration.Enabled = dgrdAlteration.ReadOnly= true;
            btnEdit.Enabled = btnDelete.Enabled = true;

        }

        private void txtSRCODE_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALTERATIONCODE", "SEARCH ALT. SERIAL CODE", e.KeyCode);
                        objSearch.ShowDialog();

                        if (objSearch.strSelectedData != "")
                            txtSRCODE.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void AlterationSlip_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            Char pressedKey = e.KeyChar;
            if (txtCustomerName.Text == "" && Char.IsWhiteSpace(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            Char pressedKey = e.KeyChar;
            
            if (txtRemark.Text == "" && Char.IsWhiteSpace(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void dgrdAlteration_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    dba.ClearTextBoxOnKeyDown(sender, e);
                    string strCName = txtCustomerName.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("CUSTOMERNAME", "SEARCH CUSTOMER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtCustomerName.Text = objSearch.strSelectedData;
                    }
                    else if (value != 8)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (_bNewStatus)
                {
                    txtBillNo.ReadOnly = true;
                    e.Handled = true;
                }
                else if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    dba.ClearTextBoxOnKeyDown(sender, e);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALEBILLCODENO", "SEARCH SALE BILL NO", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            string[] str = objSearch.strSelectedData.Split('|');
                            txtBillNo.Text = str[0];
                            txtCustomerName.Text = str[1];
                            txtMobile.Text = str[2];
                        }
                    }
                    else if (value != 8)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtSRCODE.Text != "" && txtSNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("ALTERATIONSLIP", txtSRCODE.Text, txtSNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }
    }
}
