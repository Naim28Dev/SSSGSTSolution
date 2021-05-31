using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class GoodsReceipt : Form
    {
        DataBaseAccess dba;
        DataTable dtItemName;
        string strFullOrderNo = "",strLastSerialNo="",strOldPartyName="";
        public bool saleStatus = false,updateStatus=false,newStatus=false;
        public GoodsReceipt()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public GoodsReceipt(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            newStatus = bStatus;          
        }

        public GoodsReceipt(string strCode,string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtReceiptCode.Text = strCode;
            BindRecordWithControl(strSNo);            
        }

        public GoodsReceipt(string strCode, string strSNo,bool sStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtReceiptCode.Text = strCode;
            saleStatus = sStatus;
            BindRecordWithControl(strSNo);
            EnableAllControls();
        }

        private void GoodsReciept_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (pnlItems.Visible)
                //    pnlItems.Visible = false;
                //else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdItem.Focused && !txtRemark.Focused)
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
                        if ( btnAdd.Text == "&Add" && btnEdit.Text== "&Edit" && txtReceiptNo.Text != "")
                        {
                            BindRecordWithControl(txtReceiptNo.Text);
                        }
                    }
                }
            }
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = "Select GReceiveCode,(Select ISNULL(MAX(ReceiptNo),0) from GoodsReceive Where ReceiptCode=GReceiveCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";//Select Distinct ItemName from Items  Where ItemName!='' order by ItemName 
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtReceiptCode.Text = Convert.ToString(dt.Rows[0]["GReceiveCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                   // dtItemName = ds.Tables[1];
                    BindItemWithGrid();
                    
                }
            }
            catch
            {
            }
        }

        private void BindItemWithGrid()
        {
            try
            {
                //if (dtItemName.Rows.Count > 0)
                //{
                //    dgrdItem.Rows.Add(dtItemName.Rows.Count);
                //    for (int i = 0; i < dtItemName.Rows.Count; ++i)
                //    {
                //        dgrdItem.Rows[i].Cells["chkItem"].Value = (Boolean)false;
                //        dgrdItem.Rows[i].Cells["itemName"].Value = dtItemName.Rows[i]["ItemName"];
                //    }
                //}
            }
            catch
            {
            }
        }


        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ReceiptNo),'') from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ReceiptNo),'') from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ReceiptNo),'') from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo>" + txtReceiptNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ReceiptNo),'') from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo<" + txtReceiptNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                if (strSerialNo != "")
                {
                    DisableAllControls();
                    dgrdItem.Rows.Clear();
                    string strQuery = " Select *,(CONVERT(varchar,ReceivingDate,103)) RDate,(CONVERT(varchar,OrderDate,103)) ODate,dbo.GetFullName(SalePartyID) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') HParty,ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') PParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,ReceivingDate))) LockType from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' And ReceiptNo=" + strSerialNo
                                          + " Select (OB.OrderCode+' '+CAST(OB.SerialNo as varchar)) ID,(CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)FullOrder,(CASE When OB.PurchasePartyID='' then OB.Personal else dbo.GetFullName(OB.PurchasePartyID) end) PParty,OB.Items,OB.Pieces,OB.Quantity,OB.Amount,(Convert(varchar,OB.Date,103))Date from OrderBooking OB inner Join GoodsReceive GR on OB.SalePartyID=GR.SalePartyID and OB.SubPartyID=GR.SubPartyID Where OB.Status='PENDING' and GR.ReceiptCode='" + txtReceiptCode.Text + "' and GR.ReceiptNo=" + strSerialNo
                                          + " Select TransactionLock,GroupII,BlackList from SupplierMaster SM inner Join GoodsReceive GR on (SM.AreaCode+CAST(SM.AccountNo as varchar))=GR.SalesParty Where SM.GroupName !='SUB PARTY' and GR.ReceiptCode='" + txtReceiptCode.Text + "' and GR.ReceiptNo=" + strSerialNo
                                          + " Select * from dbo.[GoodsReceiveDetails] Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + strSerialNo;
                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        BindGoodsReceiveDetails(ds.Tables[0], ds.Tables[3]);
                        BindDataWithControlUsingDataTable(ds.Tables[0]);
                        BindPendingOrderWithGrid(ds.Tables[1]);
                        
                        DataTable dt = ds.Tables[2];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                                txtSalesParty.BackColor = Color.IndianRed;
                            else
                                txtSalesParty.BackColor = Color.White;
                            if (Convert.ToString(dt.Rows[0]["GroupII"]) == "CASH PARTY")
                                pnlCash.Visible = true;
                            else
                                pnlCash.Visible = false;
                        }
                    }
                    
                }
            }
            catch
            {
            }
        }

        private void BindDataWithControlUsingDataTable(DataTable dt)
        {
            DisableAllControls();
            if (dt != null && dt.Rows.Count > 0)
            {
                
                DataRow dr = dt.Rows[0];
                lblID.Text = "";
                txtReceiptCode.Text = Convert.ToString(dr["ReceiptCode"]);
                txtReceiptNo.Text = Convert.ToString(dr["ReceiptNo"]);
                txtDate.Text = Convert.ToString(dr["RDate"]);
                txtSalesParty.Text =  Convert.ToString(dr["SParty"]);
                txtSubParty.Text = Convert.ToString(dr["HParty"]);
                txtPurchaseParty.Text = strOldPartyName = Convert.ToString(dr["PParty"]);
           
                txtBox.Text = Convert.ToString(dr["Box"]);
                txtRemark.Text = Convert.ToString(dr["Remark"]);
                strFullOrderNo = txtOrderNo.Text = Convert.ToString(dr["OrderNo"]);
                txtOrderDate.Text = Convert.ToString(dr["ODate"]);
                if (strFullOrderNo == "")
                    rdoNew.Checked = true;
                else
                    rdoPending.Checked = true;
                string strPStatus=Convert.ToString(dr["PackingStatus"]).ToUpper();
                if (strPStatus == "DIRECT")
                    rdoDirect.Checked = true;
                else if (strPStatus == "PACKED")
                    rdoPacked.Checked = true;
                else
                    rdoOnAccount.Checked = true;

                if (txtPurchaseParty.Text == "")
                    txtPurchaseParty.Text = "PERSONAL";
                if (txtSubParty.Text == "")
                    txtSubParty.Text = "SELF";
                string strCreatedBy = Convert.ToString(dr["CreatedBy"]), strUpdatedBy = Convert.ToString(dr["UpdatedBy"]);
                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                EnableAndDisableBySale(dr["SaleBill"]);

                if (Convert.ToString(dr["LockType"]) == "LOCK" && MainPage.strUserRole != "SUPERADMIN")
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                //    EnableAllControls();
                CalculateTotalAmt();                
            }
        }

        private void BindGoodsReceiveDetails(DataTable _dtMain, DataTable _dtDetails)
        {
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdItem.Rows.Add(_dtDetails.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dgrdItem.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    dgrdItem.Rows[_index].Cells["gPcsType"].Value = row["PcsType"];
                    dgrdItem.Rows[_index].Cells["gQty"].Value = row["Quantity"];
                    dgrdItem.Rows[_index].Cells["gAmount"].Value = row["Amount"];
                    dgrdItem.Rows[_index].Cells["gPacking"].Value = row["PackingAmt"];
                    dgrdItem.Rows[_index].Cells["gFreight"].Value = row["FreightAmt"];
                    dgrdItem.Rows[_index].Cells["gTax"].Value = row["TaxAmt"];

                    _index++;
                }
            }
            else
            {
                dgrdItem.Rows.Add();
                DataRow row = _dtMain.Rows[0];

                dgrdItem.Rows[0].Cells["itemName"].Value = row["Item"];
                dgrdItem.Rows[0].Cells["gPcsType"].Value = row["Pieces"];
                dgrdItem.Rows[0].Cells["gQty"].Value = row["Quantity"];
                dgrdItem.Rows[0].Cells["gAmount"].Value = row["Amount"];
                dgrdItem.Rows[0].Cells["gPacking"].Value = row["Packing"];
                dgrdItem.Rows[0].Cells["gFreight"].Value = row["Freight"];
                dgrdItem.Rows[0].Cells["gTax"].Value = row["Tax"];

                //txtPiecesType.Text = Convert.ToString(dr["Pieces"]);
                //txtAmount.Text = dba.ConvertObjectToDouble(dr["Amount"]).ToString("N2", MainPage.indianCurancy);
                //txtQuantity.Text = Convert.ToString(dr["Quantity"]);
                //txtFreight.Text = Convert.ToString(dr["Freight"]);
                //txtPacking.Text = Convert.ToString(dr["Packing"]);
                //txtTax.Text = Convert.ToString(dr["Tax"]);
                //txtItemName.Text = Convert.ToString(dr["Item"]);
            }
        }

        private void EnableAllControls()
        {
            txtDate.ReadOnly = txtBox.ReadOnly = txtRemark.ReadOnly = false;// txtQuantity.ReadOnly = txtAmount.ReadOnly = txtPacking.ReadOnly = txtFreight.ReadOnly = txtTax.ReadOnly =
            rdoDirect.Enabled = rdoPacked.Enabled = rdoNew.Enabled = rdoPending.Enabled = btnOrderClear.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtDate.ReadOnly = txtBox.ReadOnly = txtRemark.ReadOnly = true;// txtQuantity.ReadOnly = txtAmount.ReadOnly = txtPacking.ReadOnly = txtFreight.ReadOnly = txtTax.ReadOnly =
            txtReceiptNo.ReadOnly = rdoDirect.Enabled = rdoPacked.Enabled = rdoNew.Enabled = rdoPending.Enabled = btnOrderClear.Enabled = false;
        }

        private void EnableAndDisableBySale(object objValue)
        {
            if (Convert.ToString(objValue).ToUpper() == "CLEAR")
            {
                txtSalesParty.Enabled = txtSubParty.Enabled = btnDelete.Enabled = false;
                if (saleStatus)
                {
                    txtPurchaseParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = dgrdItem.Enabled = true;// txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
                    btnAdd.Enabled =btnSearch.Enabled= false;
                    btnEdit.Text = "&Update";
                    txtDate.Focus();
                }
                else
                    txtPurchaseParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = dgrdItem.Enabled = false;// txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
            }
            else
                txtSalesParty.Enabled = txtSubParty.Enabled = txtPurchaseParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = btnDelete.Enabled = dgrdItem.Enabled = true;// txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
        }

        private void EnableForAdding()
        {
            txtSalesParty.Enabled = txtSubParty.Enabled = btnDelete.Enabled = txtPurchaseParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = dgrdItem.Enabled= true;//txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
        }

        private void ClearAllText()
        {
            txtSalesParty.Text = txtSubParty.Text = txtPurchaseParty.Text = txtBox.Text =txtOrderNo.Text = txtOrderDate.Text = lblID.Text = lblCreatedBy.Text =txtRemark.Text= "";
            //txtItemName.Text = txtPiecesType.Text = //txtQuantity.Text = txtAmount.Text = txtPacking.Text = txtFreight.Text = txtTax.Text = lblTotalAmt.Text = "0.00";
            lblTotalAmt.Text = "0.00";
            txtSalesParty.BackColor = Color.White;
            dgrdPending.Rows.Clear();
            chkSendSMS.Checked = false;
            rdoPacked.Checked = true;

            dgrdItem.Rows.Clear();
            dgrdItem.Rows.Add();

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtReceiptCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(ReceiptNo)+1,1)SNo,(Select ISNULL(Max(GoodsReceiveNo)+1,1) from MaxSerialNo)ReceiptNo  from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {
                        int receiptNo = Convert.ToInt32(table.Rows[0]["SNo"]), maxReceiptNo = Convert.ToInt32(table.Rows[0]["ReceiptNo"]);
                        if (receiptNo > maxReceiptNo)
                            txtReceiptNo.Text = Convert.ToString(receiptNo);
                        else
                            txtReceiptNo.Text = Convert.ToString(maxReceiptNo);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Set Goods Receipt No in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtReceiptNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("SALESPARTY");
                        if (strData != "")
                        {
                            dgrdPending.Rows.Clear();
                            txtSalesParty.Text = strData;
                            txtSubParty.Text = "SELF";
                            GetPendingOrderAndOther();
                        }
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                dgrdPending.Rows.Clear();
                                txtSalesParty.Text = strData;
                                txtSubParty.Text = "SELF";
                                GetPendingOrderAndOther();
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

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtSalesParty.Text != "")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtSubParty.Text = objSearch.strSelectedData;
                            GetPendingOrderAndOther();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearFewRecordOnPartyChange()
        {
            txtSalesParty.BackColor = Color.White;
            pnlCash.Visible = false;
            dgrdPending.Rows.Clear();
            if (txtOrderNo.Text != "")
            {
                lblTotalAmt.Text = "0.00";//txtAmount.Text = txtPacking.Text = txtFreight.Text = txtTax.Text =
                txtPurchaseParty.Text = txtOrderNo.Text = txtOrderDate.Text = lblID.Text = "";//txtItemName.Text = txtPiecesType.Text =
            }
        }

        private void GetPendingOrderAndOther()
        {
            try
            {
                ClearFewRecordOnPartyChange();
                if (txtSalesParty.Text != "" && txtSubParty.Text != "")
                {
                    bool tStatus = true;
                    string strSaleParty = "", strSubParty = "";
                    string[] strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strSaleParty = strFullName[0].Trim();
                    strFullName = txtSubParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                        strSubParty = strFullName[0].Trim();

                    if (strSaleParty != "" && strSubParty != "")
                    {
                        string strQuery = " Select TransactionLock,GroupII,BlackList from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' "
                                               + " Select (OrderCode+' '+CAST(SerialNo as varchar)) ID,(CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)FullOrder,(CASE When P_Party='' then Personal else dbo.GetFullName(P_Party) end) PParty,Items,Pieces,Quantity,Amount,(Convert(varchar,Date,103))Date from OrderBooking Where Status='PENDING' and S_Party='" + strSaleParty + "' and Haste='" + strSubParty + "' ";

                        DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSalesParty.Clear();
                                    txtSubParty.Clear();
                                    tStatus = false;
                                }
                                if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                                    txtSalesParty.BackColor = Color.IndianRed;
                                if (Convert.ToString(dt.Rows[0]["GroupII"]) == "CASH PARTY")
                                    pnlCash.Visible = true;
                            }
                            if (tStatus)
                                BindPendingOrderWithGrid(ds.Tables[1]);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void BindPendingOrderWithGrid(DataTable dt)
        {
            int rowIndex = 0;
            dgrdPending.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdPending.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdPending.Rows[rowIndex].Cells["chk"].Value = false;
                    dgrdPending.Rows[rowIndex].Cells["id"].Value = row["ID"];
                    dgrdPending.Rows[rowIndex].Cells["date"].Value = row["Date"];
                    dgrdPending.Rows[rowIndex].Cells["order"].Value = row["FullOrder"];
                    dgrdPending.Rows[rowIndex].Cells["party"].Value = row["PParty"];
                    dgrdPending.Rows[rowIndex].Cells["item"].Value = row["Items"];
                    dgrdPending.Rows[rowIndex].Cells["qty"].Value = row["Quantity"];
                    dgrdPending.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdPending.Rows[rowIndex].Cells["pcsType"].Value = row["Pieces"];
                    rowIndex++;
                }
            }
        }

        private void dgrdPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 0)
                    {
                        bool chkCheckStatus = Convert.ToBoolean(dgrdPending.CurrentCell.Value);
                        foreach (DataGridViewRow row in dgrdPending.Rows)
                            row.Cells["chk"].Value = false;

                        dgrdPending.CurrentCell.Value = chkCheckStatus;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        ShowOrderBookingPage();
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowOrderBookingPage()
        {
            string strOrderNo = Convert.ToString(dgrdPending.CurrentRow.Cells["id"].Value);
            string[] strOrder = strOrderNo.Split(' ');
            if (strOrder.Length > 1)
            {
                if (strOrder[0] != "" && strOrder[1] != "")
                {
                    OrderBooking objOrderBooking = new OrderBooking(strOrder[0], strOrder[1]);
                    objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objOrderBooking.Show();
                }
            }
        }

        private void btnPAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bool aStatus = true;
                if (btnEdit.Text == "&Update" && strFullOrderNo != "" && lblID.Text == "")
                {
                    DialogResult result = MessageBox.Show("An Order No is already adjusted with this goods receipt !! Are you still want to adjust new order no ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        aStatus = false;
                }
                if (aStatus)
                {
                    foreach (DataGridViewRow row in dgrdPending.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["chk"].Value))
                        {
                            BindGRPendingData(row);
                            row.Visible = false;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ReverseEntry()
        {
            if (lblID.Text != "")
            {
                foreach (DataGridViewRow row in dgrdPending.Rows)
                {
                    if (lblID.Text == Convert.ToString(row.Cells["order"].Value))
                    {
                        row.Visible = true;
                        lblID.Text = "";
                        break;
                    }
                }
            }
        }

        private void BindGRPendingData(DataGridViewRow row)
        {
            if (row != null)
            {
                ReverseEntry();
                lblID.Text = Convert.ToString(row.Cells["order"].Value);
                txtOrderNo.Text = Convert.ToString(row.Cells["order"].Value);
                txtOrderDate.Text = Convert.ToString(row.Cells["Date"].Value);
                txtPurchaseParty.Text = Convert.ToString(row.Cells["party"].Value);

                dgrdItem.Rows.Clear();
                dgrdItem.Rows[0].Cells["itemName"].Value = row.Cells["item"].Value;
                dgrdItem.Rows[0].Cells["gQty"].Value = row.Cells["qty"].Value;
                dgrdItem.Rows[0].Cells["gAmount"].Value = row.Cells["amount"].Value;
                dgrdItem.Rows[0].Cells["gPcsType"].Value = row.Cells["pcsType"].Value;

                //txtItemName.Text = Convert.ToString(row.Cells["item"].Value);
                //txtQuantity.Text = Convert.ToString(row.Cells["qty"].Value);
                //txtAmount.Text = Convert.ToString(row.Cells["amount"].Value);
                //txtPiecesType.Text = Convert.ToString(row.Cells["pcsType"].Value);
                CalculateTotalAmt();
            }
        }

        private bool ValidateControls()
        {
            if (txtReceiptCode.Text == "")
            {
                MessageBox.Show("Sorry ! Receipt code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReceiptCode.Focus();
                return false;
            }
            if (txtReceiptNo.Text == "")
            {
                MessageBox.Show("Sorry ! Receipt no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReceiptNo.Focus();
                return false;
            }           
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors can't be blank !!", "Sundry Debtors required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (txtSubParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sub party can't be blank !!", "Sub party required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Creditor can't be blank !!", "Sundry Creditor required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }
            //if (txtItemName.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Item name can't be blank !!", "Item name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtItemName.Focus();
            //    return false;
            //}
            //if (txtQuantity.Text == "" || txtQuantity.Text == "0")
            //{
            //    MessageBox.Show("Sorry ! Qty can't be blank !!", "Qty required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtQuantity.Focus();
            //    return false;
            //}
            //if (dba.ConvertObjectToDouble(txtAmount.Text) == 0 && txtPurchaseParty.Text != "PERSONAL")
            //{
            //    MessageBox.Show("Sorry ! Amount can't be blank !!", "Amount required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtAmount.Focus();
            //    return false;
            //}
            //if (txtAmount.Text == "")
            //    txtAmount.Text = "0.00";
            foreach (DataGridViewRow row in dgrdItem.Rows)
            {
                string strItem = Convert.ToString(row.Cells["itemName"].Value), strQty = Convert.ToString(row.Cells["gQty"].Value), strAmount = Convert.ToString(row.Cells["gAmount"].Value);
                if (strItem == "" && strQty == "" && strAmount == "")
                    dgrdItem.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter order no", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdItem.CurrentCell = row.Cells["itemName"];
                        dgrdItem.Focus();
                        return false;
                    }
                    if (strQty == "")
                    {
                        MessageBox.Show("Sorry ! Quantity can't be blank", "Enter Sundry Creditor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdItem.CurrentCell = row.Cells["gQty"];
                        dgrdItem.Focus();
                        return false;
                    }
                    if (strAmount == "" && txtPurchaseParty.Text != "PERSONAL")
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdItem.CurrentCell = row.Cells["gAmount"];
                        dgrdItem.Focus();
                        return false;
                    }
                }
            }
            if (dgrdItem.Rows.Count == 0)
            {
                dgrdItem.Rows.Add();
                MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdItem.CurrentCell = dgrdItem.Rows[0].Cells["itemName"];
                dgrdItem.Focus();
                return false;
            }
            return true;
        }

        private bool ValidateFromPrevoisBill()
        {
            string strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "";
            string[] strFullName = txtSalesParty.Text.Split(' ');
            if (strFullName.Length > 1)
                strSalePartyID = strFullName[0].Trim();

            strFullName = txtSubParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strSubPartyID = strFullName[0].Trim();

            if (txtPurchaseParty.Text != "PERSONAL")
            {
                strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 0)
                    strPurchasePartyID = strFullName[0].Trim();
            }

            double dAmt = dba.ConvertObjectToDouble(lblTotalAmt.Text);
            string strQuery = "Select ReceiptNo from GoodsReceive Where ReceiptNo!=" + txtReceiptNo.Text + " and SalePartyID='" + strSalePartyID + "' and SubPartyID='" + strSubPartyID + "' and PurchasePartyID='" + strPurchasePartyID + "' and (CAST(Amount as money)+CAST(Freight as money)+CAST(Packing as Money)+CAST(Tax as Money))=" + dAmt + " ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (Convert.ToString(objValue) != "")
            {
                DialogResult result = MessageBox.Show("Sorry ! This detail might be saved in receipt no : " + objValue + " ! Are you want to continue ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }



        private bool CheckBillNoAndSuggest()
        {
            bool chkStatus = true;
            try
            {
                if (txtReceiptNo.Text != "")
                {
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtReceiptNo.Text))
                    {
                        int check = dba.CheckGoodsReceiptAvailability(txtReceiptCode.Text, txtReceiptNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(ReceiptNo)+1 from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' "));
                            MessageBox.Show("Sorry ! This Receipt No is already Exist ! you are Late,  Receipt Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            chkStatus = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Receipt No is already in used please Choose Different Receipt No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReceiptNo.Focus();
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Receipt No can't be blank  ..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReceiptNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }

        private string GetPackingStatus()
        {
            if (rdoDirect.Checked)
                return "DIRECT";
            else if (rdoPacked.Checked)
                return "PACKED";
            else
                return "ONACCOUNT";
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
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();
                    EnableForAdding();
                    txtDate.Focus();
                }
                else if (ValidateControls() && CheckBillNoAndSuggest() && ValidateFromPrevoisBill())
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
            btnAdd.Enabled = true ;
        }

        private void SaveRecord()
        {
            try
            {
                string strODate = "NULL", strNCode = "";
                DateTime rDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtOrderNo.Text != "" && txtOrderDate.Text.Length == 10)
                {
                    DateTime oDate = dba.ConvertDateInExactFormat(txtOrderDate.Text);
                    strODate = "'" + oDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strPerosnal = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                if (txtPurchaseParty.Text == "PERSONAL")
                    strPerosnal = "PERSONAL";
                else
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                    }
                }
                if (txtBox.Text == "")
                    txtBox.Text = "0";
                double dAmt = 0,dTAmt=0,dQty=0,dTQty=0,dPacking=0,dTPackingAmt=0,dFreightAmt=0,dTFreightAmt=0,dTaxAmt=0,dTTaxAmt=0;
                string strQuery = "",strPcsType="",strItemName="";
                foreach (DataGridViewRow rows in dgrdItem.Rows)
                {
                    dTQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dTPackingAmt += dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dTFreightAmt += dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    if (strPcsType == "")
                        strPcsType = Convert.ToString(rows.Cells["gPcsType"].Value);
                    if (strItemName != "")
                        strItemName += ",";
                    strItemName += Convert.ToString(rows.Cells["itemName"].Value);

                    strQuery += ""// if not exists (Select * from GoodsReceiveDetails Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ) begin  " 
                                   + " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus]) VALUES "
                                   + " ('" + txtReceiptCode.Text + "'," + txtReceiptNo.Text + ",'" + rows.Cells["itemName"].Value + "','" + rows.Cells["gPcsType"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxAmt + " ,1,0) ";// end ";
                }

                strQuery += "if not exists (Select * from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[GoodsReceive] ([ReceiptCode],[ReceiptNo],[OrderNo],[OrderDate],[SalesParty],[SubSalesParty],[PurchaseParty],[ReceivingDate],[Pieces],[Quantity],[Amount],[Freight],[Tax],[Packing],[Item],[Personal],[SaleBill],[PackingStatus],[CreatedBy],[PrintedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Box],[Remark],[SalePartyID],[SubPartyID],[PurchasePartyID]) Values "
                               + " ('" + txtReceiptCode.Text + "'," + txtReceiptNo.Text + ",'" + txtOrderNo.Text + "'," + strODate + ",'" + strSaleParty + "','" + strSubParty + "','" + strPurchaseParty + "','" + rDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strPcsType + "','" + dTQty + "'," + dTAmt + ",'" + dTFreightAmt + "', "
                               + " '" + dTTaxAmt + "','" + dTPackingAmt + "','" + strItemName + "','" + strPerosnal + "','PENDING','" + GetPackingStatus() + "','" + MainPage.strLoginName + "','','',1,0," + txtBox.Text + ",'" + txtRemark.Text + "','" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "')  ";



                if (txtOrderNo.Text != "")
                {
                    string[] strOrderNo = txtOrderNo.Text.Split(' ');
                    if (strOrderNo.Length > 2)
                        strNCode = strOrderNo[2];
                    strQuery += " Update OrderBooking set Status='CLEAR', Pieces='" + strPcsType + "', Quantity='" + dTQty + "', Amount=" + dTAmt + ",UpdateStatus=1 where OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                }

                strQuery += " end ";



                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    AskForPrint();
                    if (chkSendSMS.Checked)
                        SendSMSToParty();
                    btnAdd.Text = "&Add";
                    ClearAllText();
                    BindRecordWithControl(txtReceiptNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
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

                        BindLastRecord();
                        btnAdd.Text = "&Add";
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    txtReceiptNo.ReadOnly = true;
                    chkSendSMS.Checked = false;
                    txtDate.Focus();
                }
                else if (ValidateControls() && ValidatePurchaseAndSaleStatus())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        UpdateRecord();
                    }
                }
            }
            catch
            {
            }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord()
        {
            try
            {
                string strODate = "NULL", strNCode = "";
                DateTime rDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtOrderNo.Text != "" && txtOrderDate.Text.Length == 10)
                {
                    DateTime oDate = dba.ConvertDateInExactFormat(txtOrderDate.Text);
                    strODate = "'" + oDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strPerosnal = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                if (txtPurchaseParty.Text == "PERSONAL")
                    strPerosnal = "PERSONAL";
                else
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                    }
                }
                if (txtBox.Text == "")
                    txtBox.Text = "0";

                double dAmt = 0, dTAmt = 0, dQty = 0, dTQty = 0, dPacking = 0, dTPackingAmt = 0, dFreightAmt = 0, dTFreightAmt = 0, dTaxAmt = 0, dTTaxAmt = 0;
                string strQuery = "", strPcsType = "", strItemName = "";
                strQuery = " Delete from GoodsReceiveDetails Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ";
                foreach (DataGridViewRow rows in dgrdItem.Rows)
                {
                    dTQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dTPackingAmt += dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dTFreightAmt += dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                    if (strPcsType == "")
                        strPcsType = Convert.ToString(rows.Cells["gPcsType"].Value);
                    if (strItemName != "")
                        strItemName += ",";
                    strItemName += Convert.ToString(rows.Cells["itemName"].Value);

                    strQuery += " "//if not exists (Select * from GoodsReceiveDetails Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ) begin  "
                                   + " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus]) VALUES "
                                   + " ('" + txtReceiptCode.Text + "'," + txtReceiptNo.Text + ",'" + rows.Cells["itemName"].Value + "','" + rows.Cells["gPcsType"].Value + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxAmt + " ,1,0) ";//end ";
                }

                if (strFullOrderNo != "" || (txtOrderNo.Text != strFullOrderNo))
                {
                    strQuery += " Update OrderBooking Set Status='PENDING' Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end) in (Select OrderNo from GoodsReceive Where OrderNo!='' and ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + ") ";
                    if (txtOrderNo.Text != "")
                    {
                        string[] strOrderNo = txtOrderNo.Text.Split(' ');
                        if (strOrderNo.Length > 2)
                            strNCode = strOrderNo[2];
                        strQuery += " Update OrderBooking set Status='CLEAR', Pieces='" + strPcsType + "', Quantity='" + dTQty + "',UpdateStatus=1 where OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                    }
                }

                strQuery += " UPDATE [dbo].[GoodsReceive] SET [OrderNo]='" + txtOrderNo.Text + "',[OrderDate]=" + strODate + ",[SalesParty]='" + strSaleParty + "',[SubSalesParty]='" + strSubParty + "',[PurchaseParty]='" + strPurchaseParty + "',[ReceivingDate]='" + rDate.ToString("MM/dd/yyyy hh:mm:ss") + "',[Pieces]='" +strPcsType + "',[Quantity]='" +  dTQty + "', [Amount]=" + dTAmt + ", "
                                  + " [Freight]='" + dTFreightAmt + "',[Tax]='" + dTTaxAmt + "',[Packing]='" + dTPackingAmt + "',[Item]='" + strItemName + "',[Personal]='" + strPerosnal + "',[PackingStatus]='" + GetPackingStatus() + "',[Box]=" + txtBox.Text + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Remark]='" + txtRemark.Text + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "' Where [ReceiptCode]='" + txtReceiptCode.Text + "' and [ReceiptNo]=" + txtReceiptNo.Text + " ";


               // object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ");

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    //if (!Convert.ToBoolean(objValue))
                       // DataBaseAccess.CreateDeleteQuery(strQuery);
                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    if (chkSendSMS.Checked)
                        SendSMSToParty();
                    btnEdit.Text = "&Edit";
                    updateStatus = true;
                    if (saleStatus)
                        this.Close();
                    else
                    {
                        AskForPrint();
                        ClearAllText();
                        BindRecordWithControl(txtReceiptNo.Text);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Updating Record in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnOrderClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to clear this order no ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {                       
                            ClearOrderFilledData();
                            rdoNew.Checked = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && ValidatePurchaseAndSaleStatus())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes && txtReceiptCode.Text != "" && txtReceiptNo.Text != "")
                    {
                        string strQuery = "";
                        if (strFullOrderNo != "")
                            strQuery += " Update OrderBooking Set Status='PENDING' Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end) in (Select OrderNo from GoodsReceive Where OrderNo!='' and ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + ") ";
                        strQuery += " Delete from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text
                                       +  "Delete from GoodsReceiveDetails Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ";

                        object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from GoodsReceive Where  ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + " ");

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            if (!Convert.ToBoolean(objStatus))
                                DataBaseAccess.CreateDeleteQuery(strQuery);
                            MessageBox.Show("Thank You ! Record deleted successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            BindNextRecord();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void txtAmount_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0.00")
                        txt.Clear();
                }
            }
        }

        private void txtAmount_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";
                    CalculateTotalAmt();
                }
            }
        }
        
        private void txtBox_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0")
                        txt.Text = "";
                }
            }
        }

        private void txtBox_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0";
                }
            }
        }

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                            txtPurchaseParty.Text = strData;
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("PURCHASEPERSONALPARTY", "SEARCH Sundry Creditor", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                txtPurchaseParty.Text = strData;
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

        private void txtPiecesType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {                       
                        SearchData objSearch = new SearchData("PIECESTYPE", "SEARCH PIECES TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        //if (strData != "")
                        //    txtPiecesType.Text = strData;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        //pnlItems.Visible = true;
                        //SetItemToGrid(txtItemName.Text);
                        //txtSearchItem.Focus();
                    }
                }
            }
            catch
            {
            }
        }

        private void SetItemToGrid(string strItemName)
        {
            try
            {
                if (strItemName != "")
                {
                    string[] strAllItem = strItemName.Split(',');
                    foreach (string strID in strAllItem)
                    {
                        string strItem = strID.Trim();
                        if (strItem != "")
                        {
                            DataRow[] fileterrow = dtItemName.Select(String.Format(" ItemName Like ('" + strItem + "') "));
                            if (fileterrow.Length > 0 && dgrdItem.Rows.Count > 0)
                            {
                                int index = dtItemName.Rows.IndexOf(fileterrow[0]);
                                dgrdItem.Rows[index].Cells[0].Value = true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnEmailAdd_Click(object sender, EventArgs e)
        {
            AddSelectedItems();
        }

        //private void btnEmailCancel_Click(object sender, EventArgs e)
        //{
        //    pnlItems.Visible = false;
        //    txtPiecesType.Focus();
        //}

        private void AddSelectedItems()
        {
            try
            {
                string strItemName = "";
                foreach (DataGridViewRow row in dgrdItem.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkItem"].Value))
                    {
                        if (strItemName == "")
                            strItemName = Convert.ToString(row.Cells["itemName"].Value);
                        else
                            strItemName += "," + row.Cells["itemName"].Value;
                        row.Cells["chkItem"].Value = false;
                    }
                }
                //txtItemName.Text = strItemName;
                //txtPiecesType.Focus();
                //pnlItems.Visible = false;
            }
            catch
            {
            }
        }

        private void SearchItemByKey()
        {
            try
            {
                //DataRow[] fileterrow = dtItemName.Select(String.Format("ItemName Like('" + txtSearchItem.Text + "%') "));
                //if (fileterrow.Length > 0)
                //{
                //    string strName = Convert.ToString(fileterrow[0]["ItemName"]);
                //    int index = 0;
                //    foreach (DataGridViewRow row in dgrdItem.Rows)
                //    {
                //        if (Convert.ToString(row.Cells["itemName"].Value) == strName)
                //            break;
                //        index++;
                //    }
                //    dgrdItem.CurrentCell = dgrdItem.Rows[index].Cells[0];
                //    dgrdItem.FirstDisplayedCell = dgrdItem.CurrentCell;
                //}
            }
            catch
            {
            }
        }

        private void txtSearchItem_TextChanged(object sender, EventArgs e)
        {
            SearchItemByKey();
        }

        //private void dgrdItem_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        //{
        //    if (e.ColumnIndex > 0)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        //private void dgrdItem_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dgrdItem.CurrentCell = dgrdItem.CurrentRow.Cells[dgrdItem.CurrentCell.ColumnIndex + 1];
        //            AddSelectedItems();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void rdoNew_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (rdoNew.Checked)
                {
                    txtPurchaseParty.Enabled = dgrdPending.ReadOnly = true;
                    btnPAdd.Enabled = false;
                    ClearOrderFilledData();
                }
            }
        }

        private void ClearOrderFilledData()
        {
            if (lblID.Text != "0" && lblID.Text != "")
            {
                ReverseEntry();
                dgrdItem.Rows.Clear();
                dgrdItem.Rows.Add();
                txtPurchaseParty.Text = "";// txtItemName.Text = txtPiecesType.Text =
                txtBox.Text = "0";// txtQuantity.Text =
                //txtAmount.Text = txtPacking.Text = txtFreight.Text = txtTax.Text = "0.00";
            }
            txtOrderNo.Text = txtOrderDate.Text = "";
        }

        private void rdoPending_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (rdoPending.Checked)
                {
                    txtPurchaseParty.Enabled = dgrdPending.ReadOnly = false;
                    btnPAdd.Enabled = true;
                }
            }
        }

        private void CheckAvailability()
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (txtReceiptNo.Text != "")
                    {
                        object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),0) from MaxSerialNo");
                        int maxBillNo = Convert.ToInt32(objMax);
                        if (maxBillNo < Convert.ToInt32(txtReceiptNo.Text))
                        {
                            int check = dba.CheckGoodsReceiptAvailability(txtReceiptCode.Text, txtReceiptNo.Text);
                            if (check < 1)
                            {
                                lblMsg.Text = txtReceiptNo.Text + "  Receipt No is Available ........";
                                lblMsg.ForeColor = Color.Green;
                                lblMsg.Visible = true;

                            }
                            else
                            {
                                lblMsg.Text = txtReceiptNo.Text + " Receipt No is already exist ! ";
                                lblMsg.ForeColor = Color.Red;
                                lblMsg.Visible = true;
                                txtReceiptNo.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Receipt No is already in used please Choose Different Receipt No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtReceiptNo.Focus();

                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Receipt Number .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        txtReceiptNo.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability in Goods Receive ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtReceiptNo.Focus();
            }
        }

        private void txtReceiptNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtReceiptNo.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        CheckAvailability();
                    }
                    else if (btnAdd.Text=="&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControl(txtReceiptNo.Text);
                    }
                }
                else
                {
                    txtReceiptNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void GoodsReciept_Load(object sender, EventArgs e)
        {
            try
            {
                EditOption();
                //if (MainPage.strGRCompanyName != "SSS")
                //{
                //    txtBox.Enabled = false;
                //    lblBox.Enabled = false;
                //}
                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtReceiptNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView)
                {
                    if (!MainPage.mymainObject.bPurchaseAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    if (!MainPage.mymainObject.bPurchaseView)
                        txtReceiptNo.Focus();
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.Close();
                }
                
            }
            catch
            {
            }
        }

        private void AskForPrint()
        {
            try
            {
                DialogResult result = MessageBox.Show("ARE YOU WANT TO PRINT GOODS RECEIPT  ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        DataTable dt = CreateDataTable();
                        Reporting.GRSNoReport objReport = new Reporting.GRSNoReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport);
                        else
                            objReport.PrintToPrinter(1, false, 0, 1);

                        objReport.Close();
                        objReport.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("SerialNo", typeof(String));
                myDataTable.Columns.Add("PParty", typeof(String));
                myDataTable.Columns.Add("SParty", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("NetAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("SupplierHead", typeof(String));
                myDataTable.Columns.Add("SubParty", typeof(String));
                myDataTable.Columns.Add("Freight", typeof(String));
                myDataTable.Columns.Add("Tax", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("Packing", typeof(String));
                myDataTable.Columns.Add("Remark", typeof(String));
                DataRow row = myDataTable.NewRow();

              //  row["CompanyName"] = "RUPEE "+ objCurrency.changeCurrencyToWords(dba.ConvertObjectToDouble(lblTotalAmt.Text)).ToUpper();
                row["CompanyName"] = MainPage.strGRCompanyName;
                //if (btnPrint.Enabled)
                //    row["SerialNo"] = txtReceiptCode.Text + " " + txtReceiptNo.Text + " / D";
                //else
                    row["SerialNo"] = txtReceiptCode.Text + " " + txtReceiptNo.Text;

                row["SupplierHead"] = "SUPPLIER";
                row["PParty"] = txtPurchaseParty.Text;

                row["SParty"] = txtSalesParty.Text;
                row["SubParty"] = txtSubParty.Text;
                row["Date"] = txtDate.Text;
                double dTQty=0,dTAmt = 0,dTPackingAmt = 0,  dTFreightAmt  = 0, dTTaxAmt = 0;
                foreach (DataGridViewRow rows in dgrdItem.Rows)
                {
                    dTQty += dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dTAmt += dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dTPackingAmt += dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    dTFreightAmt += dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    dTTaxAmt += dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
                }

                if (dba.ConvertObjectToDouble(txtBox.Text) > 0)
                    row["Qty"] = dTQty + "  Pcs (" + txtBox.Text + " Box(s))";
                else
                    row["Qty"] = dTQty + " Pcs";// dba.ConvertObjectToDouble(txtQuantity.Text).ToString("N2", MainPage.indianCurancy) + "  Pcs";

                row["Amount"] = dTAmt.ToString("N2", MainPage.indianCurancy);
                row["Tax"] = dTTaxAmt.ToString("N2",MainPage.indianCurancy) ;
                row["Freight"] = dTFreightAmt.ToString("N2",MainPage.indianCurancy) ;
                row["Packing"] = dTPackingAmt.ToString("N2", MainPage.indianCurancy); 

                    row["NetAmount"] =  lblTotalAmt.Text;
                    row["Remark"] = txtRemark.Text; 

                row["UserName"] = MainPage.strLoginName + " ,  Date & Time : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void CalculateTotalAmt()
        {
            //double dAmt = dba.ConvertObjectToDouble(txtAmount.Text) + dba.ConvertObjectToDouble(txtFreight.Text) + dba.ConvertObjectToDouble(txtPacking.Text) + dba.ConvertObjectToDouble(txtTax.Text);
            double dTAmt = 0, dTPackingAmt = 0, dTFreightAmt = 0, dTTaxAmt = 0;
            foreach (DataGridViewRow rows in dgrdItem.Rows)
            {
                //dTQty += dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                dTAmt += dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                dTPackingAmt += dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                dTFreightAmt += dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                dTTaxAmt += dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);
            }
            lblTotalAmt.Text = (dTAmt+dTPackingAmt+dTFreightAmt+dTTaxAmt).ToString("N2", MainPage.indianCurancy);        
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "Update GoodsReceive set PrintedBy='" + MainPage.strLoginName + "',UpdateStatus=1 where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + "";
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    btnPrint.Enabled = false;   
                    DataTable dt = CreateDataTable();
                    Reporting.GRSNoReport objReport = new Reporting.GRSNoReport();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                        objReport.PrintToPrinter(1, false, 0, 0);
                        // objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                    btnPrint.Enabled = true;
                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Printing in Goods Receive ", ex.Message };
                dba.CreateErrorReports(strReport);              
            }
            btnPrint.Enabled = true;      
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {                
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.GRSNoReport objReport = new Reporting.GRSNoReport();
                objReport.SetDataSource(dt);
                Reporting.ShowReport objShow = new Reporting.ShowReport("Goods Receipt Slip");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();

                objReport.Close();
                objReport.Dispose();
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnOpenFile.Enabled = false;               

                if (txtReceiptCode.Text != "" && txtReceiptNo.Text != "" && btnAdd.Text != "&Save")
                {
                    DataBaseAccess.ShowPDFFiles(txtReceiptCode.Text, txtReceiptNo.Text);
                }
            }
            catch
            {
            }
            btnOpenFile.Enabled = true;            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidatePurchaseAndSaleStatus()
        {
            string strQuery = "", strSubQuery = "'PENDING'";

            if (!saleStatus)
                strSubQuery = " ISNULL((Select UPPER(SaleBill) from GoodsReceive Where ReceiptCode='" + txtReceiptCode.Text + "' and ReceiptNo=" + txtReceiptNo.Text + "),'PENDING') ";
            
            strQuery = "Select ISNULL(Count(*),0) PStatus, "+strSubQuery+" SStatus from PurchaseRecord Where GRSNO='"+txtReceiptCode.Text+" "+txtReceiptNo.Text+"' ";
            DataTable dt = dba.GetDataTable(strQuery);
            if (dt.Rows.Count > 0)
            {
                if (dba.ConvertObjectToDouble(dt.Rows[0]["PStatus"]) > 0 && txtPurchaseParty.Text!="PERSONAL")
                {
                    MessageBox.Show("Sorry ! Purchase bill has been made of this serial no, Please remove purchase bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (Convert.ToString(dt.Rows[0]["SStatus"])== "CLEAR")
                {
                    MessageBox.Show("Sorry ! Sale bill has been made of this serial no, Please update from sales bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        private void SendSMSToParty()
        {
            try
            {
                string strMessage = "",strMobileNo="",strOldMobileNo="";
                if (btnEdit.Text == "&Update" && txtPurchaseParty.Text != strOldPartyName && strOldPartyName != "")
                {
                    DataTable dt = dba.GetDataTable("Select MobileNo,(Select MobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strOldPartyName + "') OldMobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "'");
                    if (dt.Rows.Count > 0)
                    {
                        strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                        strOldMobileNo = Convert.ToString(dt.Rows[0]["OldMobileNo"]);
                    }
                }
                else
                {
                    object objMobile = DataBaseAccess.ExecuteMyScalar("Select MobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "'");
                    strMobileNo = Convert.ToString(objMobile);
                }
                if (strMobileNo != "" && txtPurchaseParty.Text!="PERSONAL")
                {
                    if (btnAdd.Text == "&Save")
                        strMessage = "M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblTotalAmt.Text + " with Receipt no : " + txtReceiptCode.Text + " " + txtReceiptNo.Text + ", on the date : " + txtDate.Text + ".";
                    else
                    {
                        if (txtSalesParty.Text != strOldPartyName)
                            strMessage = "M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblTotalAmt.Text + " with receipt no : " + txtReceiptCode.Text + " " + txtReceiptNo.Text + ", on the date : " + txtDate.Text + ".";
                        else
                            strMessage = "Updation in Receipt : " + txtReceiptCode.Text + " " + txtReceiptNo.Text + ", M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblTotalAmt.Text + ", on the date : " + txtDate.Text + ".";
                    }
                }
                SendSMS objSMS = new SendSMS();
                if (strMessage != "" && strMobileNo != "")
                    objSMS.SendSingleSMS(strMessage, strMobileNo);
                if (strOldMobileNo != "" && strOldPartyName!="PERSONAL")
                {
                    strMessage = "Sorry ! Wrong  entry on your account on Dt : " + txtDate.Text + ", Please avoid it.";
                    objSMS.SendSingleSMS(strMessage, strOldMobileNo);
                }
            }
            catch
            {
            }
        }

        private void lnkOrderNo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (txtOrderNo.Text != "")
                {
                    string strQuery = "Select OrderCode,SerialNo from OrderBooking Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)='"+txtOrderNo.Text+"' ";
                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        string strOrderCode = Convert.ToString(dt.Rows[0]["OrderCode"]), strSerialNo= Convert.ToString(dt.Rows[0]["SerialNo"]);
                        if (strOrderCode != "" && strSerialNo != "")
                        {
                            OrderBooking objOrder = new OrderBooking(strOrderCode, strSerialNo);
                            objOrder.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                            objOrder.ShowDialog();
                            objOrder.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";
                    
                }
            }
        }

        private void txtQuantity_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0.00")
                        txt.Clear();
                }
            }
        }

        private void txtRemark_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                if (btnAdd.Text == "&Save")
                    btnAdd.Focus();
                else if (btnEdit.Text == "&Update")
                    btnEdit.Focus();
            }
        }

        private void dgrdItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdItem.CurrentCell.RowIndex;
                    IndexColmn = dgrdItem.CurrentCell.ColumnIndex;
                    if (Index < dgrdItem.RowCount - 1)
                        CurrentRow = Index - 1;
                    else
                        CurrentRow = Index;

                    if (IndexColmn < dgrdItem.ColumnCount - 1)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                            dgrdItem.CurrentCell = dgrdItem.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdItem.RowCount - 1)
                    {
                        if (Convert.ToString(dgrdItem.Rows[CurrentRow].Cells["itemName"].Value) != "" && Convert.ToString(dgrdItem.Rows[CurrentRow].Cells["gAmount"].Value) != "")
                        {
                            dgrdItem.Rows.Add(1);
                            dgrdItem.CurrentCell = dgrdItem.Rows[CurrentRow + 1].Cells["itemName"];
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
                else if (e.KeyCode == Keys.F1)
                {
                    Index = dgrdItem.CurrentCell.RowIndex;
                    if (btnAdd.Text == "&Save")
                    {
                        dgrdItem.Rows.RemoveAt(Index);
                        CalculateTotalAmt();
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            dgrdItem.Rows.RemoveAt(Index);
                            CalculateTotalAmt();
                        }
                    }
                   
                }               
            }
            catch
            {
            }
        }

        private void dgrdItem_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0)
                    {
                        SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            string[] strItem = objSearch.strSelectedData.Split('|');
                            if(strItem.Length>0)
                                dgrdItem.CurrentCell.Value = strItem[0];
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("PIECESTYPE", "SEARCH PIECES TYPE", Keys.Space);
                        objSearch.ShowDialog();
                       // if (objSearch.strSelectedData != "")
                            dgrdItem.CurrentCell.Value = objSearch.strSelectedData;
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

        private void dgrdItem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int cIndex = dgrdItem.CurrentCell.ColumnIndex;
            if (cIndex == 2 || cIndex == 3 || cIndex == 4 || cIndex == 5 || cIndex == 6)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int cIndex = dgrdItem.CurrentCell.ColumnIndex;
            if (cIndex == 2)
                dba.KeyHandlerPoint(sender, e, 0);
            else if (cIndex == 3 || cIndex == 4 || cIndex == 5 || cIndex == 6)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void dgrdItem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 1)
                {
                    CalculateTotalAmt();
                }
            }
            catch
            {
            }
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

    }
}
