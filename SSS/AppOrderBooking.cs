using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;




namespace SSS
{
    public partial class AppOrderBooking : Form
    {
        DataBaseAccess dba;     
        bool newStatus = false, _bDeliveryDateReq = false, _bOrderStatus = false, _bSchemeActiveStatus = false;
        public bool updateStatus = false,bMasterUpdateStatus=false;
        public string strAddedOrderDetails = "", _strOrderNo_Update,_STRMasterTransportName="";
        public string _strPSalesParty = "", _strPSubParty = "", _strPackingType = "", _strPPurchaseParty = "",_strQty="0",_strAmount="0",_strItemName="",_strGRSNO="";
        double dOldNetAmt = 0;      
        public AppOrderBooking()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            BindLastRecord();
        }

        public AppOrderBooking(bool nStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtBillNo.TabStop = txtDate.TabStop = txtSalesParty.TabStop=txtSubParty.TabStop= false;
            btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
            GetStartupData();
            newStatus = nStatus;
        }

        public AppOrderBooking(string strCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            txtBillCode.Text = strCode;
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData()
        {
            try
            {
                string strLastSerialNo = "", strQuery = "Select OrderCode,(Select ISNULL(MAX(SerialNo),'') from AppOrderBooking)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "'  ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                       txtBillCode.Text = Convert.ToString(dt.Rows[0]["OrderCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                    //dtItemName = ds.Tables[1];
                    //BindItemWithGrid();
                }
                
            }
            catch
            {
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(SerialNo),'') from AppOrderBooking Where SchemeName!='' and OrderCode='" + txtBillCode.Text + "'  and InsertStatus=1");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(SerialNo),'') from AppOrderBooking Where SchemeName!='' and OrderCode='" + txtBillCode.Text + "'  and InsertStatus=1");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(SerialNo),'') from AppOrderBooking Where SchemeName!='' and OrderCode='" + txtBillCode.Text + "' and SerialNo>" + txtBillNo.Text + "  and InsertStatus=1");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(SerialNo),'') from AppOrderBooking Where SchemeName!='' and OrderCode='" + txtBillCode.Text + "' and SerialNo<" + txtBillNo.Text + " and InsertStatus=1 ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                string strQuery = "Select OB.*,(CONVERT(varchar,DeliveryDate,103)) DelDate,(CONVERT(varchar,Date,103)) BDate,(SalePartyID+' '+SM.Name) SParty,(CASE WHEN SubPartyID='SELF' then SubPartyID else SubPartyID+' '+SubName end) HParty,dbo.GetFullName(PurchasePartyID) PParty,(CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0))) PQty,PartyTransport,ISNULL(STransport,'')STransport,ISNULL(ActiveStatus,1) as ActiveStatus from AppOrderBooking OB left join (Select (SM.AreaCode+SM.AccountNo)SPartyID,Name,Transport as PartyTransport from SupplierMaster Sm WHere SM.GroupName='SUNDRY DEBTORS')SM on (SM.SPartyID)=OB.SalePartyID left join (Select (SM.AreaCode+SM.AccountNo)SBPARTYID,Name as SubName,Transport as STransport from SupplierMaster Sm WHere sm.GroupName='SUB PARTY')SMSub on SBPARTYID=OB.SubPartyID  left join (Select SM.SchemeName,ActiveStatus from SchemeMaster SM )_SM on OB.SchemeName=_SM.SchemeName Where OB.SchemeName!='' and SerialNo=" + strSerialNo + " and OrderCode='" + txtBillCode.Text + "' and OB.InsertStatus=1 Order by ID Select * from AppOrderDetails Where SerialNo=" + strSerialNo + " and OrderCode='" + txtBillCode.Text + "' Order by ID ";

                DataSet ds = dba.GetDataSet(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    lblCreatedBy.Text = txtReason.Text = _STRMasterTransportName = "";
                    pnlDeletionConfirmation.Visible = false;
                    DisableAllControls();
                    txtBillNo.ReadOnly = false;
                    _bDeliveryDateReq = true;
                    double dTAmt = 0, dAmt = 0;
                    bool _clearStatus = bMasterUpdateStatus = false;
                    _bOrderStatus = false;
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];

                           // _strCurrentOrderCode = txtCode.Text;
                            txtBillNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["BDate"]);
                            txtOrderNo.Text = Convert.ToString(row["OrderNo"]);
                            txtNumberCode.Text = Convert.ToString(row["NumberCode"]);
                            txtMarketer.Text = Convert.ToString(row["Marketer"]);
                            txtSalesParty.Text = Convert.ToString(row["SParty"]);
                            txtSubParty.Text = Convert.ToString(row["HParty"]);
                            txtSupplierName.Text = Convert.ToString(row["PParty"]);
                            txtTransport.Text = Convert.ToString(row["Transport"]);
                            txtStation.Text = Convert.ToString(row["Station"]);
                            txtScheme.Text = Convert.ToString(row["SchemeName"]);
                            txtRemark.Text = Convert.ToString(row["MRemark"]);
                            txtPvtMarka.Text = Convert.ToString(row["Marka"]);
                            txtOrderStatus.Text = Convert.ToString(row["Status"]);
                            lblQty.Text= Convert.ToString(row["Quantity"]);
                            _bSchemeActiveStatus = Convert.ToBoolean(row["ActiveStatus"]);

                            string strOrderCategory = Convert.ToString(row["OrderCategory"]);
                            if (strOrderCategory == "***")
                                rdoTriple.Checked = true;
                            else if (strOrderCategory == "**")
                                rdoDouble.Checked = true;
                            else
                                rdoSingle.Checked = true;

                            if (Convert.ToString(row["DelDate"]) == "")
                                _bDeliveryDateReq = false;

                            if (txtSubParty.Text == "")
                                txtSubParty.Text = "SELF";

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                            if (txtSubParty.Text == "SELF")
                                _STRMasterTransportName = Convert.ToString(row["STransport"]);
                            else
                                _STRMasterTransportName = Convert.ToString(row["PartyTransport"]);

                            dgrdOrder.Rows.Clear();


                            //dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                            //dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                            //dRate = dAmt / dQty;

                            //strQuery += " INSERT INTO [dbo].[AppOrderDetails] ([OrderCode],[SerialNo],[OrderNo],[NumberCode],[ItemName],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[Amount],[Remark],[RemoteID],[InsertStatus]) VALUES "
                            //         + " ('" + txtCode.Text + "',@SerialNo," + txtOrderNo.Text + ",'" + txtNumberCode.Text + "','" + row.Cells["item"].Value + "','" + row.Cells["designName"].Value + "','','','','',''," + dQty + "," + dRate + "," + dAmt + ",'" + dQty + "',0,1) ";


                            DataTable table = ds.Tables[1];

                          
                            int rowIndex = 0;
                            if (table.Rows.Count > 0)
                            {
                                dgrdOrder.Rows.Add(table.Rows.Count);
                                foreach (DataRow rows in table.Rows)
                                {
                                    dAmt = dba.ConvertObjectToDouble(rows["Amount"]);
                                    dgrdOrder.Rows[rowIndex].Cells["sno"].Value = (rowIndex+1);
                                    dgrdOrder.Rows[rowIndex].Cells["id"].Value = rows["ID"];
                                    dgrdOrder.Rows[rowIndex].Cells["item"].Value = rows["ItemName"];
                                    dgrdOrder.Rows[rowIndex].Cells["designName"].Value = rows["DesignName"];

                                    dgrdOrder.Rows[rowIndex].Cells["qtyType"].Value = row["Pieces"];
                                    dgrdOrder.Rows[rowIndex].Cells["qty"].Value = dgrdOrder.Rows[rowIndex].Cells["pendingQty"].Value = rows["Qty"];
                                    dgrdOrder.Rows[rowIndex].Cells["amt"].Value = dAmt;// rows["Amount"];
                                    dgrdOrder.Rows[rowIndex].Cells["remark"].Value = row["Remark"];
                                    dgrdOrder.Rows[rowIndex].Cells["deliveryDate"].Value = row["DelDate"];


                                    if (Convert.ToString(row["Status"]).ToUpper() == "CLEAR")
                                    {
                                        dgrdOrder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                        _clearStatus = true;
                                    }

                                    dTAmt += dAmt;

                                    rowIndex++;
                                }
                            }
                            else
                            {
                                dgrdOrder.Rows.Add(1);
                                dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                                dgrdOrder.Rows[rowIndex].Cells["sno"].Value = (rowIndex + 1);
                                dgrdOrder.Rows[rowIndex].Cells["id"].Value = row["ID"];
                                dgrdOrder.Rows[rowIndex].Cells["item"].Value = row["Items"];
                                dgrdOrder.Rows[rowIndex].Cells["designName"].Value = row["Items"];
                                dgrdOrder.Rows[rowIndex].Cells["qtyType"].Value = row["Pieces"];
                                dgrdOrder.Rows[rowIndex].Cells["qty"].Value = row["Quantity"];
                                dgrdOrder.Rows[rowIndex].Cells["pendingQty"].Value = row["PQty"];
                                dgrdOrder.Rows[rowIndex].Cells["amt"].Value = dAmt;// rows["Amount"];
                                dgrdOrder.Rows[rowIndex].Cells["remark"].Value = row["Remark"];
                                dgrdOrder.Rows[rowIndex].Cells["deliveryDate"].Value = row["DelDate"];


                                if (Convert.ToString(row["Status"]).ToUpper() == "CLEAR")
                                {
                                    dgrdOrder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                    _clearStatus = true;
                                }

                                dTAmt += dAmt;
                            }
                            dgrdOrder.FirstDisplayedCell = dgrdOrder.Rows[0].Cells[2];

                        }

                    }

                    if (dgrdOrder.Rows.Count == 0)
                    {
                        dgrdOrder.Rows.Add();
                    }


                    txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = !_clearStatus;

                    if (MainPage.strUserRole.Contains("ADMIN"))
                        txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = true;
                    if (txtScheme.Enabled && !_bSchemeActiveStatus)
                        txtScheme.Enabled = _bSchemeActiveStatus;
                    dOldNetAmt = dTAmt;
                    lblNetAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                }
            }
            catch
            {
            }
        }

        private void OrderBooking_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlDeletionConfirmation.Visible)
                        pnlDeletionConfirmation.Visible = false;                 
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !dgrdOrder.Focused)
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
                        else if (e.Control && e.Shift && e.KeyCode == Keys.D)
                        {
                            if (txtBillNo.Text != "")
                            {
                                btnAdd.Text = "&Save";
                                SetSerialNo();
                                EnableAllControls();
                                txtOrderNo.Focus();
                            }
                        }
                    }
                }
            }
            catch { }
        }


        private void EnableAllControls()
        {
            txtPvtMarka.ReadOnly = txtRemark.ReadOnly = txtDate.ReadOnly =txtOrderNo.ReadOnly= false;
            grpCategory.Enabled = true;
            if (dgrdOrder.Rows.Count < 2 && _bSchemeActiveStatus)
                txtScheme.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtPvtMarka.ReadOnly = txtRemark.ReadOnly= txtDate.ReadOnly = txtOrderNo.ReadOnly = true;
            grpCategory.Enabled = false;
        }

        private void ClearAllText()
        {
          txtOrderNo.Text=  txtSupplierName.Text = txtPvtMarka.Text = txtRemark.Text = lblCreatedBy.Text = txtMarketer.Text = txtSalesParty.Text = txtSubParty.Text = txtTransport.Text = txtStation.Text = txtScheme.Text = "";
            txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = true;
            dgrdOrder.Rows.Clear();
            dgrdOrder.Rows.Add(1);
            dgrdOrder.Rows[0].Cells["sno"].Value = 1;
            _STRMasterTransportName = "";
            bMasterUpdateStatus = _bOrderStatus = _bSchemeActiveStatus = false;
            lblNetAmt.Text = lblQty.Text = "0.00";
            rdoSingle.Checked = true;
            dOldNetAmt = 0;
            txtNumberCode.Text = "11F";
            txtOrderStatus.Text = "PENDING";
            pnlCash.Visible = false;
            chkEmail.Checked =  false;
            chkWhatsapp.Checked = true;
            DateTime _date = MainPage.startFinDate;
            if (MainPage.currentDate > MainPage.startFinDate && MainPage.currentDate <= MainPage.endFinDate)
                _date = MainPage.currentDate;
            txtDate.Text = _date.ToString("dd/MM/yyyy");
            if(dgrdOrder.Rows.Count>0)
                dgrdOrder.Rows[0].Cells["deliveryDate"].Value = _date.AddDays(2).ToString("dd/MM/yyyy");
           
            // SetDesignSerialNo();
        }

        private void SetSerialNo()
        {
            txtBillNo.Text = "";
               object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(SerialNo),0)+1)SerialNo from AppOrderBooking Where (OrderCode='" + txtBillCode.Text + "') ");
            txtBillNo.Text = Convert.ToString(objValue);
        }

        private void SetSerialNoOfDummy()
        {
            txtBillNo.Text = "";
            string strQuery = "Select (ISNULL(MAX(SerialNo),0)+1)SerialNo,(Select (ISNULL(MAX(OrderNo),0)+1)OrderNo from AppOrderBooking Where OrderCode='" + txtBillCode.Text + "')OrderNo from AppOrderBooking Where (OrderCode='" + txtBillCode.Text + "') ";
            DataTable _dt = dba.GetDataTable(strQuery);
            if (_dt.Rows.Count > 0)
            {
                txtBillNo.Text = Convert.ToString(_dt.Rows[0]["SerialNo"]);
                if (dgrdOrder.Rows.Count == 0)
                {
                    dgrdOrder.Rows.Add(1);
                    dgrdOrder.Rows[0].Cells["sno"].Value = 1;
                }

                txtOrderNo.Text= Convert.ToString(_dt.Rows[0]["OrderNo"]);
            }
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
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, false, true);   
                if(txtSupplierName.Text!="")
                {
                    SetSchemeName();
                }
            }
        }

        private void txtMarketer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("FAIRMARKETERNAME",txtBillCode.Text, "SEARCH MARKETER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                       // if (objSearch.strSelectedData != "")
                            txtMarketer.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
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
                            txtSalesParty.Text = strData;
                            txtSubParty.Text = "SELF";
                            GetPendingGRRecord();
                        }
                    }
                    else
                    {
                        if (CheckAdjustedQtyAll() ||  (MainPage.strUserRole.Contains("SUPERADMIN")))
                        {
                            char objChar = Convert.ToChar(e.KeyCode);
                            int value = e.KeyValue;
                            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                            {
                                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    txtSalesParty.Text = objSearch.strSelectedData;
                                    txtSubParty.Text = "SELF";
                                    GetPendingGRRecord();
                                }
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
                            GetPendingGRRecord();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
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
                        txtTransport.Text = objSearch.strSelectedData;
                    }
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
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtStation.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtBStation_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
        //        {
        //            char objChar = Convert.ToChar(e.KeyCode);
        //            int value = e.KeyValue;
        //            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //            {
        //                SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION NAME", e.KeyCode);
        //                objSearch.ShowDialog();
        //                txtBStation.Text = objSearch.strSelectedData;
        //            }
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}

        private void GetPendingGRRecord()
        {
            _bOrderStatus = false;
            txtTransport.Text =  txtStation.Text = "";//txtBStation.Text = 
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                string strSaleParty = "", strSubParty = "", strQuery="";
                bool tStatus = true;
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSaleParty = strFullName[0].Trim();
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                    strSubParty = strFullName[0].Trim();

                if (strSaleParty != "" && strSubParty != "")
                {
                    //string strQuery = " Select * from ( Select (ReceiptCode+' '+CAST(ReceiptNo as varchar)) RCode,Convert(varchar,ReceivingDate,103)RDate,(CASE When PurchasePartyID!='' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Item,Pieces,(CASE WHEN Pieces='PETI' then ISNULL(NoOfCase,1) else Quantity end)Quantity,Amount,'PURCHASE' as BillType,ReceivingDate as Date,0 as PendingQty,0 CancelQty,0 as ID,'' as Transport,'' as Station from GoodsReceive Where (OrderNo='0' OR OrderNo='') and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' UNION ALL "
                    //                + " Select (OrderCode + ' ' + CAST(SerialNo as varchar)) RCode,Convert(varchar, Date, 103)RDate,(CASE When PurchasePartyID!= '' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Items,Pieces,Quantity,Amount,'ORDER' as BillType,Date,CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)) as PendingQty,CancelQty,ID,Transport,Station from OrderBooking Where SalePartyID = '" + strSaleParty+"' and SubPartyID = '"+strSubParty+"' and OrderCode Like('%OD') )_Order Order by Date desc ";

                    if (strSubParty == "SELF")
                        strQuery += " Select Transport,PvtMarka,Station,BookingStation,TransactionLock,BlackList from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";
                    else
                        strQuery += " Select Transport,PvtMarka,Station,BookingStation,TransactionLock,BlackList from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSubParty + "' ";

                    strQuery += " Select TransactionLock,GroupII,BlackList,TINNumber,UPPER(Other1) as OrangeZone from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[1];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                txtSalesParty.Focus();
                                tStatus = false;
                            }
                            if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                            {
                                txtSalesParty.BackColor = Color.IndianRed;
                                MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSalesParty.Focus();
                            }                           
                            if (Convert.ToString(dt.Rows[0]["OrangeZone"])=="TRUE")
                            {
                                txtSalesParty.BackColor = Color.Orange;
                                MessageBox.Show("This Account is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSalesParty.Focus();
                            }
                            else
                                txtSalesParty.BackColor = Color.White;
                            if (Convert.ToString(dt.Rows[0]["GroupII"]) == "CASH PARTY" || Convert.ToString(dt.Rows[0]["TINNumber"]) == "CASH PARTY")
                                pnlCash.Visible = true;
                            else
                                pnlCash.Visible = false;
                        }

                        if (tStatus)
                        {
                            dt.Clear();
                            dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                if (row != null)
                                {
                                    _STRMasterTransportName = Convert.ToString(row["Transport"]);//txtTransport.Text
                                    txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                                    txtStation.Text = Convert.ToString(row["BookingStation"]);
                                    //  txtBStation.Text = Convert.ToString(row["BookingStation"]);
                                    if(txtSubParty.Text!="SELF")
                                    {
                                        if (Convert.ToBoolean(row["TransactionLock"]))
                                        {
                                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Sub Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                                           
                                            txtSubParty.Text="SELF";
                                            txtSubParty.Focus();
                                            tStatus = false;
                                        }
                                        if (Convert.ToBoolean(row["BlackList"]))
                                        {
                                            txtSubParty.BackColor = Color.IndianRed;
                                            MessageBox.Show("This Account is in blacklist ! Please Select Different Sub Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            txtSubParty.Text = "SELF";
                                            txtSubParty.Focus();
                                            tStatus = false;
                                        }
                                        else
                                            txtSubParty.BackColor = Color.White;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
               
        private bool CheckBillAdjustment(string strpBillNo)
        {
            if (txtScheme.Text != "")
            {
                bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(strpBillNo);
                return netStatus;
            }
            else
                return true;
        }

        private void dgrdOrder_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("FAIRITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdOrder.CurrentCell.Value = objSearch.strSelectedData;
                        if (Convert.ToString(dgrdOrder.CurrentRow.Cells["designName"].Value) == "" || (btnAdd.Text == "&Save" && (dgrdOrder.Rows.Count - 1) == e.RowIndex))
                        {
                            string[] strItem = objSearch.strSelectedData.Split(':');
                            dgrdOrder.CurrentRow.Cells["designName"].Value = strItem[0].Trim();
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 3)
                    {
                        if (dba.ConvertObjectToDouble(dgrdOrder.CurrentRow.Cells["adjustedQty"].Value) == 0)
                        {
                            SearchData objSearch = new SearchData("PIECESTYPE", "SEARCH PIECES TYPE", Keys.Space);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                                dgrdOrder.CurrentCell.Value = objSearch.strSelectedData;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! This order is adjusted with purchase bill.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
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

        private void dgrdOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
            if (cIndex != 3 || cIndex != 1)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.CharacterCasing = CharacterCasing.Upper;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
            if (cIndex != 1 && cIndex != 3)
            {
                if (cIndex == 2 || cIndex == 7)
                    dba.ValidateSpace(sender, e);
                else if (cIndex == 6)
                    dba.KeyHandlerPoint(sender, e, 0);
                else                 
                    dba.KeyHandlerPoint(sender, e, 2);

            }
        }

        private void dgrdOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdOrder.CurrentCell.RowIndex;
                    IndexColmn = dgrdOrder.CurrentCell.ColumnIndex;
                    if (Index < dgrdOrder.RowCount - 1)
                        CurrentRow = Index - 1;
                    else
                        CurrentRow = Index;

                    if (IndexColmn < dgrdOrder.ColumnCount - 5)
                    {
                        IndexColmn += 1;
                        if (IndexColmn == 2)
                            IndexColmn += 1;
                        if(Index>0)
                            if (IndexColmn == 3)
                                IndexColmn += 1;

                        if (CurrentRow >= 0)
                            dgrdOrder.CurrentCell = dgrdOrder.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdOrder.RowCount - 1)
                    {
                        if (!_bOrderStatus && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["item"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["qty"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["qtyType"].Value) != "")
                        {
                            dgrdOrder.Rows.Add(1);
                            dgrdOrder.Rows[CurrentRow + 1].Cells["sno"].Value = (CurrentRow + 2) + ".";
                            dgrdOrder.Rows[CurrentRow + 1].Cells["qtyType"].Value = dgrdOrder.Rows[CurrentRow].Cells["qtyType"].Value;
                            dgrdOrder.Rows[CurrentRow + 1].Cells["deliveryDate"].Value = dgrdOrder.Rows[CurrentRow].Cells["deliveryDate"].Value;
                            dgrdOrder.CurrentCell = dgrdOrder.Rows[CurrentRow + 1].Cells["item"];
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
                    RemoveSelectedData(dgrdOrder.CurrentRow);
                }
            }
            catch
            {
            }
        }


        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdOrder.Rows)
            {
                row.Cells["sno"].Value = serialNo;
                serialNo++;
            }
        }

        private void RemoveSelectedData(DataGridViewRow row)
        {
            int _index = row.Index;

            if (btnAdd.Text == "&Save")
            {
                dgrdOrder.Rows.RemoveAt(_index);
            }
            else if (btnEdit.Text == "&Update")
            {
                string strID = Convert.ToString(row.Cells["id"].Value);
                if (strID != "")
                {                   
                        DialogResult result = MessageBox.Show("Are you sure want to remove permanently ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            //if (DeleteSingleRow(strID))
                                dgrdOrder.Rows.Remove(row);
                        }
                    
                }
                else
                    dgrdOrder.Rows.Remove(row);
            }
            ArrangeSerialNo();
        }

        //private bool DeleteSingleRow(string strID)
        //{
        //    string strOrderNo = "", strQuery = "";
        //    object value = DataBaseAccess.ExecuteMyScalar("Select (CASE When OB.NumberCode!='' then(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end) OrderNo from AppOrderBooking Ob Where OrderCode='" + txtBillCode.Text + "' and ID=" + strID + " ");
        //    strOrderNo = Convert.ToString(value);
        //    if (strOrderNo != "")
        //    {
        //        strQuery += " Update AppOrderBooking Set UpdatedBy='" + MainPage.strLoginName+"' Where OrderCode='"+txtBillCode.Text+"' and SerialNo="+txtBillNo.Text+" "
        //                 + " Delete from AppOrderBooking Where (CASE When NumberCode!='' then(OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)='" + strOrderNo + "' "
        //                 + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
        //                 + " ('APPORDER','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strOrderNo + " NO DELETE FROM ORDER BOOKING, with Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";


        //        //+ " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
        //        //+ " ('ORDER','" + txtCode.Text + "'," + txtSerialNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

        //        int count = dba.ExecuteMyQuery(strQuery);
        //        if (count > 0)
        //        {
        //            DataBaseAccess.CreateDeleteQuery(strQuery);
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    else
        //        return false;
        //}

       

        private bool ValidateControls()
        {
            try
            {
                if (txtBillCode.Text == "")
                {
                    MessageBox.Show("Sorry ! Order code can't be blank !!", "Order code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillCode.Focus();
                    return false;
                }
                if (txtBillNo.Text == "")
                {
                    MessageBox.Show("Sorry ! Serial no can't be blank !!", "Serial no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    return false;
                }
                if (txtDate.Text.Length != 10)
                {
                    MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDate.Focus();
                    return false;
                }
                if (txtOrderNo.Text == "")
                {
                    MessageBox.Show("Sorry ! Order no can't be blank", "Enter order no", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOrderNo.Focus();
                    return false;
                }
                else
                {
                    string strOrderStatus = CheckOrderNoAvailability();
                    if (strOrderStatus != "")
                    {
                        MessageBox.Show("Sorry ! Order no " + txtOrderNo.Text + " is already exist in " + strOrderStatus + ",\nPlease try with different order no.", "Already exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtOrderNo.Focus();
                        return false;
                    }

                }
                if (txtSupplierName.Text == "")
                {
                    MessageBox.Show("Sorry ! Supplier name can't be blank !!", "Sundry Debtors required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSupplierName.Focus();
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
                //if (txtTransport.Text == "")
                //{
                //    MessageBox.Show("Sorry ! Transport name can't be blank !!", "Transport Name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    txtTransport.Focus();
                //    return false;
                //}
                if (txtStation.Text == "")
                {
                    MessageBox.Show("Sorry ! Station name can't be blank !!", "Station Name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStation.Focus();
                    return false;
                }

                if (btnAdd.Text == "&Save" && _STRMasterTransportName != txtTransport.Text)
                {
                    //DialogResult result = MessageBox.Show("Sorry ! Transport name in master and summary doesn't match,\nAre you want to update this transport in Party master ?  !!", "Transport name mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (result == DialogResult.Yes)
                        bMasterUpdateStatus = true;
                    //else
                    //    bMasterUpdateStatus = false;
                }

                double dAmt = 0, dQty = 0,dNetAmt=0;
                string strDeliveryDate = "";
                DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                string strSupplierName = "";
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    string strID = Convert.ToString(row.Cells["id"].Value), strItem = Convert.ToString(row.Cells["item"].Value);
                    strDeliveryDate = Convert.ToString(row.Cells["deliveryDate"].Value);

                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (strID == "" && strItem == "" && dQty == 0)
                        dgrdOrder.Rows.Remove(row);
                    else
                    {
                        dNetAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);                        
                        if (strItem == "")
                        {
                            MessageBox.Show("Sorry ! Item name can't be blank", "Enter Item name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["item"];
                            dgrdOrder.Focus();
                            return false;
                        }
                        if (dQty == 0)
                        {
                            MessageBox.Show("Sorry ! Qty can't be blank", "Enter Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["qty"];
                            dgrdOrder.Focus();
                            return false;
                        }
                        if (dAmt == 0)
                        {
                            MessageBox.Show("Sorry ! Amount can't be blank", "Enter Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["amt"];
                            dgrdOrder.Focus();
                            return false;
                        }
                        if (strDeliveryDate == "" && _bDeliveryDateReq && txtScheme.Text == "")
                        {
                            if (strDeliveryDate == "")
                            {
                                MessageBox.Show("Sorry ! Expected Delivery Date can't be blank !!", "Date required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdOrder.CurrentCell = row.Cells["deliveryDate"];
                                dgrdOrder.Focus();
                                return false;
                            }
                            if (strDeliveryDate.Length != 10)
                            {
                                MessageBox.Show("Sorry ! Please enter valid delivery date !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdOrder.CurrentCell = row.Cells["deliveryDate"];
                                dgrdOrder.Focus();
                                return false;
                            }
                        }
                        else if (strDeliveryDate.Length == 10)
                        {
                            DateTime _dDate = dba.ConvertDateInExactFormat(strDeliveryDate);
                            if (_dDate < _date)
                            {
                                MessageBox.Show("Sorry ! Delivery date can't be less than order date !!", "Invoice date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdOrder.CurrentCell = row.Cells["deliveryDate"];
                                dgrdOrder.Focus();
                                return false;
                            }
                        }                      
                    }
                }

                if (dgrdOrder.Rows.Count == 0)
                {
                    dgrdOrder.Rows.Add();
                    MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdOrder.CurrentCell = dgrdOrder.Rows[0].Cells["orderNo"];
                    dgrdOrder.Focus();
                    return false;
                }
                string[] str = txtSupplierName.Text.Split(' ');
                if(str.Length>1)
                strSupplierName = str[0];
                if (txtScheme.Text != "")
                {
                    string strQuery = "Select Convert(varchar,StartDate,103)+' - '+Convert(varchar,EndDate,103) _Date from SchemeMaster Where ActiveStatus=1 and (StartDate>'" + _date.ToString("MM/dd/yyyy") + "' OR EndDate<'" + _date.ToString("MM/dd/yyyy") + "') and SchemeName='" + txtScheme.Text + "' ";
                    if (strSupplierName != "")
                        strQuery += "Select (AreaCode+AccountNo+' '+Name) PartyName from SupplierMaster Where (AreaCode+AccountNo) in ('" + strSupplierName + "') and GroupName ='SUNDRY CREDITOR' and Other not in (Select SupplierName from Scheme_SupplierDetails Where SchemeName='" + txtScheme.Text + "') ";

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 0)
                    {
                        object objValue = "";
                        if (ds.Tables[0].Rows.Count > 0)
                            objValue = ds.Tables[0].Rows[0]["_Date"];

                        if (Convert.ToString(objValue) != "")
                        {
                            MessageBox.Show("Sorry ! This scheme is valid in the date period of : " + objValue, "Scheme not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtDate.Focus();
                            return false;
                        }
                        if (ds.Tables.Count > 1)
                        {
                            strSupplierName = "";
                            foreach (DataRow row in ds.Tables[1].Rows)
                                strSupplierName += "\n" + Convert.ToString(row["PartyName"]);


                            if (strSupplierName != "")
                            {
                                MessageBox.Show("Sorry ! The following supplier is not in scheme list, Please contact to concern department." + strSupplierName, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }

                    if (btnEdit.Text == "&Update")
                    {
                        if (!_bSchemeActiveStatus)
                        {
                            MessageBox.Show("Sorry ! This scheme is closed now, So that you are unable to update this order, Please contact to concern department.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("ADMIN"))
                                return false;
                        }
                    }
                }                
                string strTransaction = "false", strCategory = "*";
                if (!rdoSingle.Checked)
                    strCategory = "**";
                if (txtOrderStatus.Text != "HOLD")
                {
                    double dLimitAmt = dba.GetAmountLimitValidationFromNet(txtSalesParty.Text, strCategory, ref strTransaction);
                    dLimitAmt -= dNetAmt;
                    if (dLimitAmt < 0)
                    {
                        MessageBox.Show("Sorry ! Amount limit has been exceeded, Please extend amount limit : " + Math.Abs(dLimitAmt).ToString("N2", MainPage.indianCurancy) + " of  " + txtSalesParty.Text + " !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        DialogResult _result = MessageBox.Show("Are you want to hold this order ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (_result == DialogResult.Yes)
                            txtOrderStatus.Text = "HOLD";
                        else if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private string CheckOrderNoAvailability()
        {
            string strQuery = "Select (OrderCode+' '+CAST(SerialNo as varchar)+' : '+ PurchasepartyID+' '+ P_Party) Result from AppOrderBooking Where OrderCode='" + txtBillCode.Text + "' and SerialNo!=" + txtBillNo.Text + " and OrderNo=" + txtOrderNo.Text + " and NumberCode='" + txtNumberCode.Text + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
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
                    txtOrderNo.Focus();
                    _bDeliveryDateReq = true;
                }
                else if (ValidateControls() || MainPage.strUserRole.Contains("SUPERADMIN"))
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save order booking ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        

        private void SaveRecord()
        {
            try
            {
                string strQuery = "", strDate = "",strDDate="",strDeliveryDate="NULL", strPersonal = "", strOrderCategory="*";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
               string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "",strPurchaseParty="";
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

                strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }

                if (rdoDouble.Checked)
                    strOrderCategory = "**";      
                else if (rdoTriple.Checked)
                    strOrderCategory = "***";

                strQuery += "Declare @SerialNo bigint; Select @SerialNo = (ISNULL(MAX(SerialNo), 0) + 1)  from APPOrderBooking Where (OrderCode='" + txtBillCode.Text + "')"
                         + " if not exists (Select SerialNo from AppOrderBooking Where OrderCode='" + txtBillCode.Text + "'  and SerialNo=@SerialNo)  begin  ";
                double dTAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);

                if (dgrdOrder.Rows.Count>0)
                {
                    DataGridViewRow row = dgrdOrder.Rows[0];

                    strDDate = Convert.ToString(row.Cells["deliveryDate"].Value);
                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";
                   

                    strQuery += " if not exists (Select SerialNo from AppOrderBooking Where OrderCode='" + txtBillCode.Text + "'  and OrderNo=" + txtOrderNo.Text + " and NumberCode='" + txtNumberCode.Text + "') begin INSERT INTO [dbo].[AppOrderBooking] ([OrderCode],[SerialNo],[Date],[BookingNo],[Marketer],[S_Party],[Haste],[Transport],[Marka],[Station],[Booking],[OrderNo],[NumberCode],[P_Party],[Items],[Pieces],[Quantity],[Amount],[Personal],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory]) VALUES "
                             + " ('" + txtBillCode.Text + "',@SerialNo,'" + strDate + "','0','" + txtMarketer.Text + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','"+txtPvtMarka.Text+"','" + txtStation.Text + "','" + txtStation.Text + "','" + txtOrderNo.Text + "',"
                             + " '" + txtNumberCode.Text + "','" + strPurchaseParty + "','" + row.Cells["item"].Value + "','" + row.Cells["qtyType"].Value + "','" + dba.ConvertObjectToDouble(lblQty.Text) + "'," + dTAmt + ",'" + strPersonal + "','" + txtOrderStatus.Text + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtScheme.Text + "','',N'" + row.Cells["remark"].Value + "',0," + dba.ConvertObjectToDouble(row.Cells["cancelQty"].Value) + ",N'" + txtRemark.Text + "'," + strDeliveryDate + ",'"+row.Cells["designName"].Value+"','','','','','APPORDER','" + strOrderCategory + "') ";
                    
                }
                double dQty, dRate, dAmt;
                foreach(DataGridViewRow row in dgrdOrder.Rows)
                {
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                    dRate = dAmt / dQty;

                    strQuery += " INSERT INTO [dbo].[AppOrderDetails] ([OrderCode],[SerialNo],[OrderNo],[NumberCode],[ItemName],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[Amount],[Remark],[RemoteID],[InsertStatus]) VALUES "
                             + " ('" + txtBillCode.Text + "',@SerialNo," + txtOrderNo.Text + ",'" + txtNumberCode.Text + "','" + row.Cells["item"].Value + "','" +  row.Cells["designName"].Value + "','','','','','',"+dQty+"," + dRate + "," + dAmt + ",'" + dQty + "',0,1) ";

                }

                if (strQuery != "")
                {
                    strQuery += "  end if exists(Select SerialNo from AppOrderBooking Where OrderCode = '" + txtBillCode.Text + "'  and SerialNo = @SerialNo)  begin  ";

                    string strTransportReason = "";
                    if (bMasterUpdateStatus && _STRMasterTransportName!=txtTransport.Text)
                    {
                        if (strSubPartyID == "SELF")
                            strQuery += " Update SupplierMaster Set Transport='"+txtTransport.Text+"' Where (AreaCode+AccountNo)='"+strSalePartyID+"' ";
                        else
                            strQuery += " Update SupplierMaster Set Transport='" + txtTransport.Text + "' Where (AreaCode+AccountNo)='" + strSubPartyID + "' ";

                        strTransportReason = "TRP CHANGED " + _STRMasterTransportName + " TO " + txtTransport.Text;
                    }

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason],[ComputerName]) VALUES "
                              + "('APPORDER','" + txtBillCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION','" + strTransportReason + "','') ";

                    strQuery += "  end end ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {                      
                        MessageBox.Show("Thank you ! Record saved successfully !", "Saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        if (chkEmail.Checked || chkWhatsapp.Checked)
                            GeneratePDF(false);
                        if (newStatus)
                        {
                            strAddedOrderDetails = GetRecentOrderDetails();
                            this.Close();
                        }
                        else
                        {
                            btnAdd.Text = "&Add";
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save, Please try after some time", "Unable to save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }     

        private string GetRecentOrderDetails()
        {
            string strOrderDetails = "";
            if(dgrdOrder.Rows.Count==1)
            {
                strOrderDetails = (txtBillCode.Text + " " + Convert.ToString(dgrdOrder.Rows[0].Cells["orderNo"].Value) + " " + Convert.ToString(dgrdOrder.Rows[0].Cells["orderCode"].Value)).Trim();
                strOrderDetails += "|" + dgrdOrder.Rows[0].Cells["qty"].Value + "|" + txtSalesParty.Text + "|" + txtSubParty.Text+ "|" + txtDate.Text + "|" + dgrdOrder.Rows[0].Cells["pparty"].Value+"|"+ dgrdOrder.Rows[0].Cells["qtyType"].Value; 
            }
            return strOrderDetails;
        }

        private string GetRecentOrderDetails_Update()
        {
            string strOrderDetails = "";
            if (dgrdOrder.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    strOrderDetails = (txtBillCode.Text + " " + Convert.ToString(row.Cells["orderNo"].Value) + " " + Convert.ToString(row.Cells["orderCode"].Value)).Trim();
                    if (_strOrderNo_Update == strOrderDetails)
                    {
                        strOrderDetails += "|" + row.Cells["qty"].Value + "|" + txtSalesParty.Text + "|" + txtSubParty.Text + "|" + txtDate.Text + "|" + row.Cells["pparty"].Value + "|" + row.Cells["qtyType"].Value;
                        break;
                    }
                }
            }
            return strOrderDetails;
        }

        private void btnEdit_Click(object sender, EventArgs e)
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
                txtBillNo.ReadOnly = true;
                txtDate.Focus();
            }
            else if (ValidateControls() || MainPage.strUserRole.Contains("SUPERADMIN"))
            {
                DialogResult result = MessageBox.Show("Are you sure want to update order booking ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    UpdateRecord();
                }
            }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord()
        {
            try
            {
                string strQuery = "", strID = "", strDate = "", strDeliveryDate = "NULL", strDDate = "", strPParty = "", strPersonal = "", strOrderNo = "", strStatus = "", strOrderCategory = "*";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
                
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = ""; ;
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                if (rdoTriple.Checked)
                    strOrderCategory = "***";
                else if (rdoDouble.Checked)
                    strOrderCategory = "**";

                double dTAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);

                if (dgrdOrder.Rows.Count > 0)
                {
                    DataGridViewRow row = dgrdOrder.Rows[0];

                    strDDate = Convert.ToString(row.Cells["deliveryDate"].Value);
                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";
                    strStatus = "PENDING";

                    strQuery += "UPDATE AppOrderBooking SET [Date]='" + strDate + "',[Marketer]='" + txtMarketer.Text + "',[S_Party]='" + strSaleParty + "',[Haste]='" + strSubParty + "',[Transport]='" + txtTransport.Text + "',[Marka]='" + txtPvtMarka.Text + "',[Station]='" + txtStation.Text + "',[Booking]='" + txtStation.Text + "',[OrderNo]='" + txtOrderNo.Text + "',[NumberCode]='" + txtNumberCode.Text + "',[P_Party]='" + strPParty + "',[Items]='" + row.Cells["item"].Value + "',[Pieces]='" + row.Cells["qtyType"].Value + "',[Quantity]='" + dba.ConvertObjectToDouble(lblQty.Text) + "',[Amount]=" + dTAmt+","
                             + " [UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "',[SchemeName]='" + txtScheme.Text + "',[Remark]=N'" + row.Cells["remark"].Value + "',[CancelQty]=" + dba.ConvertObjectToDouble(row.Cells["cancelQty"].Value) + ",[MRemark]=N'" + txtRemark.Text + "',[DeliveryDate]=" + strDeliveryDate + ",[Variant1]='" + row.Cells["designName"].Value + "',[OrderCategory]='" + strOrderCategory + "',Status='"+txtOrderStatus.Text+"' Where OrderCode='" + txtBillCode.Text + "' and NumberCode='" + txtNumberCode.Text + "' and [SerialNo]=" + txtBillNo.Text;
                }

                strQuery += " DELETE from [dbo].[AppOrderDetails] WHERE [OrderCode]='" + txtBillCode.Text + "' and [SerialNo]=" + txtBillNo.Text;
                double dQty, dRate, dAmt;
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                    dRate = dAmt / dQty;

                    strQuery += " INSERT INTO [dbo].[AppOrderDetails] ([OrderCode],[SerialNo],[OrderNo],[NumberCode],[ItemName],[DesignName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[Amount],[Remark],[RemoteID],[InsertStatus]) VALUES "
                             + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + "," + txtOrderNo.Text + ",'" + txtNumberCode.Text + "','" + row.Cells["item"].Value + "','" + row.Cells["designName"].Value + "','','','','',''," + dQty + "," + dRate + "," + dAmt + ",'" + dQty + "',0,1) ";

                }

                if (strQuery != "")
                {
                  
                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason],[ComputerName]) VALUES "
                             + "('APPORDER','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION','','') ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        GeneratePDF(false);
                        MessageBox.Show("Thank you ! Record updated successfully !", "Updated successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        if (updateStatus)
                        {
                            strAddedOrderDetails = GetRecentOrderDetails_Update();
                            this.Close();
                        }
                        else
                        {
                            btnEdit.Text = "&Edit";
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to update, Please try after some time", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }     

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
            {
                pnlDeletionConfirmation.Visible = true;
                txtReason.Focus();
            }
        }

        private void OrderBooking_Load(object sender, EventArgs e)
        {
            try
            {
                EditOption();

                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtSalesParty.Text = _strPSalesParty;
                    txtSubParty.Text = _strPSubParty;
                   
                    GetPendingGRRecord();

                    if (dgrdOrder.Rows.Count > 0)
                    {
                        dgrdOrder.Rows[0].Cells["qty"].Value = _strQty;
                        dgrdOrder.Rows[0].Cells["amt"].Value = _strAmount;
                        dgrdOrder.Rows[0].Cells["qtyType"].Value = _strPackingType;
                        dgrdOrder.Rows[0].Cells["item"].Value = _strItemName;
                        dgrdOrder.Rows[0].Cells["deliveryDate"].Value = txtDate.Text;
                    }

                    txtOrderNo.Focus();
                }
                else if (updateStatus)
                {
                    txtBillNo.ReadOnly = true;
                    btnAdd.Enabled = btnDelete.Enabled = btnSearch.Enabled= false;
                    btnEdit.Text = "&Update";
                }
                   
            }
            catch { }            
        }

        private void EditOption()
        {
            try
            {
                if (!(MainPage.mymainObject.bOrderEdit))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bOrderAdd)
                    btnAdd.Enabled = false;
                if (!(MainPage.mymainObject.bOrderView))
                {
                    this.Close();
                    MessageBox.Show("Sorry ! You don't have sufficeint permission to Access this Page ! ", "Permission Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                BindRecordWithControl(txtBillNo.Text);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("APPORDER", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save")
                {
                    if (txtReason.Text != "" && CheckLinkedOrder())
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete order booking ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = "";
                            strQuery += " Delete from AppOrderBooking Where OrderCode='" + txtBillCode.Text + "' and SerialNo=" + txtBillNo.Text
                                     + "  Delete from AppOrderDetails Where OrderCode = '" + txtBillCode.Text + "' and SerialNo = " + txtBillNo.Text
                                       + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('APPORDER','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", Marketer : " + txtMarketer.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                           // object objValue = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from OrderBooking Where OrderCode='" + txtCode.Text + "' and SerialNo=" + txtSerialNo.Text);

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! Record delete successfully !", "Deleted successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                               // if (!Convert.ToBoolean(objValue))
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                txtReason.Text = "";
                                pnlDeletionConfirmation.Visible = false;
                                BindNextRecord();
                            }
                            else
                                MessageBox.Show("Sorry ! Unable to delete, Please try after some time", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtOrderNo_Leave(object sender, EventArgs e)
        {
            try {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOrderNo.Text != "")
                    {
                        string strOrderStatus = CheckOrderNoAvailability();
                        if (strOrderStatus != "")
                        {
                            MessageBox.Show("Sorry ! This Order no  is already exist in " + strOrderStatus + ",\nPlease try with different order no.", "Already exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtOrderNo.Focus();
                            txtOrderNo.Text = "";
                        }
                    }
                }
            }
            catch { }
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }



        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (CheckOrderClearingStatus())//MainPage.mymainObject.bFullEditControl &&
                    {
                        
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("SCHEMENAME", "ACTIVE", "SEARCH SCHEME NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            txtScheme.Text = objSearch.strSelectedData;
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private bool CheckOrderClearingStatus()
        {
            bool _bStatus = false;
            try
            {
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    if (dba.ConvertObjectToDouble(row.Cells["adjustedQty"].Value) > 0 && dgrdOrder.Rows.Count != 1)
                    {
                        MessageBox.Show("Sorry ! Order adjusted with purchase book ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("ADMIN"))
                            return false;
                    }

                }
            }
            catch { }
            return true;
        }
                
        private bool CheckAdjustedQtyAll()
        {
            foreach (DataGridViewRow row in dgrdOrder.Rows)
            {
                if (dba.ConvertObjectToDouble(row.Cells["adjustedQty"].Value) > 0)
                    return false;
            }
            return true;
        }

        private bool CheckLinkedOrder()
        {
            if (MainPage.strUserRole != "SUPERADMIN")
            {
                if (!MainPage.strUserRole.Contains("ADMIN") && !(dba.ConvertDateInExactFormat(txtDate.Text).AddDays(2) > MainPage.currentDate))
                {
                    if (!MainPage.mymainObject.bFullEditControl || !(dba.ConvertDateInExactFormat(txtDate.Text).AddDays(2) > MainPage.currentDate))
                    {
                        MessageBox.Show("Sorry ! You don't have sufficient permission to delete this Order ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                
            }          
            return true;
        }

        private void btnSendPDF_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure want to send email & whatsapp ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                GeneratePDF(true);
            }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                // {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                string strCode = txtBillCode.Text;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ORDERCODE", "SEARCH ORDER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtBillCode.Text = objSearch.strSelectedData;
                    if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtBillCode.Text == "DLOO")
                    {
                        MessageBox.Show("Sorry ! This code not allowed in order entry !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillCode.Text = strCode;
                    }
                    else
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            if (txtBillCode.Text.Contains("OD"))
                                SetSerialNoOfDummy();
                            else
                                SetSerialNo();
                        }
                    }
                }
                //}
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSupplierName_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSupplierName.Text);
        }

        private void rdoTriple_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTriple.Checked)
                txtPvtMarka.TabStop = true;
            else
                txtPvtMarka.TabStop = false;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                string strSEmailID = "", strSMobileNo = "", strPEmailID = "", strPMobileNo = "", strAddress = "";

                DataTable dt = CreateDataTable(ref strSEmailID, ref strSMobileNo, ref strPEmailID, ref strPMobileNo, ref strAddress);
                if (dt.Rows.Count > 0 && (strSEmailID != "" || strSMobileNo != "" || strPEmailID != "" || strPMobileNo != ""))
                {
                    string strPath = File_Path;
                    Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("ORDER FORM");
                    Reporting.OrderReport objReport = new Reporting.OrderReport();
                    objReport.SetDataSource(dt);
                    objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    objShowReport.myPreview.ReportSource = objReport;
                    objShowReport.Show();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;           
        }

        private void txtOrderStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOrderStatus.Text=="PENDING" || txtOrderStatus.Text == "HOLD")
                    {

                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("ORDERSTATUS", "SEARCH ORDER STATUS", e.KeyCode);
                            objSearch.ShowDialog();
                            if(objSearch.strSelectedData!="")
                            txtOrderStatus.Text = objSearch.strSelectedData;
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
        }

        private void txtOrderNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtTransport_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenTransportMaster(txtTransport.Text);
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    dba.ValidateSpace(sender, e);
                }
            }
            catch { }
        }

        private void txtRemark_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    if (rdoNew.Checked)
                //    {
                //        dgrdOrder.Focus();
                //        if (dgrdOrder.Rows.Count > 0)
                //        {
                //            dgrdOrder.CurrentCell= dgrdOrder.Rows[0].Cells["orderNo"];
                //            dgrdOrder.FirstDisplayedCell = dgrdOrder.Rows[0].Cells["orderNo"];
                //        }
                //    }
                //    else
                //        dgrdPending.Focus();
                //}
            }
            catch { }
        }

        private void dgrdOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5 || e.ColumnIndex == 4)
                {
                    CalculateNetAmt();
                }
                else if (e.ColumnIndex == 6)
                {
                    string strDate = Convert.ToString(dgrdOrder.CurrentCell.EditedFormattedValue);
                    if (strDate.Length == 8)
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate, false);
                            if (!txtDate.Text.Contains("/"))
                            {

                            }
                            else
                            {
                                if (e.RowIndex < dgrdOrder.Rows.Count - 1)
                                {
                                    dgrdOrder.EndEdit();
                                }
                            }
                            dgrdOrder.CurrentCell.Value = txtDate.Text;
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch { }
        }

        private void CalculateNetAmt()
        {
            double dTAmt = 0,dQty=0;
            try
            {
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    dQty += dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    dTAmt += dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                }
            }
            catch { }
            lblQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
        }
           

        private void txtSupplierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLSUPPLIERNAME", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                        {
                            string[] strSupplier = strData.Split('|');
                            if(strSupplier.Length>0)
                            txtSupplierName.Text = strSupplier[0];
                            GetOrderCode();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }        

        private void GetOrderCode()
        {
            string[] strSupplier = txtSupplierName.Text.Split(' ');
            if (strSupplier.Length > 1)
            {
                string strPartyCode = strSupplier[0],strCode="";
                if (strPartyCode != "")
                {
                    strCode = System.Text.RegularExpressions.Regex.Replace(strPartyCode, @"[\d-]", string.Empty);
                    if (strCode == "AH")
                        strCode = "AHD";
                    strCode += "O";
                    txtBillCode.Text = strCode;
                   // SetSerialNo();
                    SetSchemeName(strPartyCode);
                }
               
            }
        }

        private void SetSchemeName(string strPartyCode = "")
        {
            if (strPartyCode == "")
            {
                string[] strSupplier = txtSupplierName.Text.Split(' ');
                strPartyCode = strSupplier[0];
            }

            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            string strQuery = "Select SchemeName,(Select (ISNULL(MAX(SerialNo),0)+1)SerialNo from AppOrderBooking Where (OrderCode='" + txtBillCode.Text + "'))SerialNo from Scheme_SupplierDetails SSD inner join SupplierMaster SM on SM.GroupName='SUNDRY CREDITOR' and SM.Other=SSD.SupplierName Where StartDate<='" + _date.ToString("MM/dd/yyyy") + "' and EndDate>='" + _date.ToString("MM/dd/yyyy") + "' and AreaCode+AccountNo='" + strPartyCode + "' ";
            DataTable _dt = dba.GetDataTable(strQuery);
            if (_dt.Rows.Count > 0)
            {
                if (btnAdd.Text == "&Save")
                    txtBillNo.Text = Convert.ToString(_dt.Rows[0]["SerialNo"]);
                txtScheme.Text = Convert.ToString(_dt.Rows[0]["SchemeName"]);
            }
        }

        private void dgrdOrder_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 6)
                {
                    string strDate = Convert.ToString(dgrdOrder.CurrentCell.EditedFormattedValue);
                    if (strDate != "")
                    {
                        strDate = strDate.Replace("/", "");
                        if (strDate.Length == 8)
                        {
                            TextBox txtDate = new TextBox();
                            txtDate.Text = strDate;
                            dba.GetStringFromDateForReporting(txtDate, false);
                            if (!txtDate.Text.Contains("/"))
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                if (e.RowIndex < dgrdOrder.Rows.Count - 1)
                                {
                                    dgrdOrder.EndEdit();
                                }
                            }
                            dgrdOrder.CurrentCell.Value = txtDate.Text;
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private string File_Path
        {
            get
            {
                string strPath = "", _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\OrderPDF\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                _strPath += "\\" + _strFileName;

                strPath = _strPath + "\\" + txtOrderNo.Text + txtNumberCode.Text + ".pdf";
                if (System.IO.File.Exists(strPath))
                    System.IO.File.Delete(strPath);
                System.IO.Directory.CreateDirectory(_strPath);
                return strPath;
            }
        }

        private void GeneratePDF(bool _bStatus)
        {
            try {
                string strSEmailID = "", strSMobileNo = "", strPEmailID = "", strPMobileNo = "", strAddress = "";
                if (chkEmail.Checked || chkWhatsapp.Checked || _bStatus)
                {
                    DataTable dt = CreateDataTable(ref strSEmailID, ref strSMobileNo, ref strPEmailID, ref strPMobileNo, ref strAddress);
                    if (dt.Rows.Count > 0 && (strSEmailID != "" || strSMobileNo != "" || strPEmailID != "" || strPMobileNo != ""))
                    {
                        string strPath = File_Path;
                        Reporting.OrderReport objReport = new Reporting.OrderReport();
                        objReport.SetDataSource(dt);
                        objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        if (strPath != "")
                        {
                            objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                            objReport.Close();
                            objReport.Dispose();
                        }

                        if (chkEmail.Checked && (strSEmailID != "" || strPEmailID != ""))
                        {
                            CreateEmailBody(strSEmailID, strPEmailID, strPath, strPEmailID);
                        }
                        else if (_bStatus)
                            MessageBox.Show("Sorry ! Please enter mail id in party master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (chkWhatsapp.Checked && (strSMobileNo != "" || strPMobileNo != ""))
                        {
                            string _strFileName = txtOrderNo.Text + txtNumberCode.Text + ".pdf", strFilePath = MainPage.strFTPPath+ "/OrderPdf/" + txtBillCode.Text + "/" + _strFileName;
                            string strHttpPath = MainPage.strHttpPath+"/OrderPdf/" + txtBillCode.Text + "/" + _strFileName;
                            bool __bStatus = dba.UploadBillPDFFile(strPath, _strFileName, txtBillCode.Text, "ORDERPDF");

                            string strFullOrderNo = txtBillCode.Text + " " + txtOrderNo.Text + " " + txtNumberCode.Text;

                            string strMessage = "\"variable1\": \"" + strFullOrderNo + "\",\"variable2\": \"Saraogi Super Sales Pvt. Ltd.\",";
                            string strResult = "";
                            if (strSMobileNo != "")
                                strResult = WhatsappClass.SendWhatsappWithIMIMobile(strSMobileNo, "orderform", strMessage, "", strHttpPath);
                            if (strPMobileNo != "")
                                strResult = WhatsappClass.SendWhatsappWithIMIMobile(strSMobileNo, "orderform", strMessage, "", strHttpPath);
                        }
                    }
                }
            }
            catch { }
        }

        private void CreateEmailBody(string strEmail,string strPEmailID, string strpath,string strAddress)
        {
            try
            {
                string strSubject, strMsgBody, strFullOrderNo = txtBillCode.Text + " " + txtOrderNo.Text + " " + txtNumberCode.Text;
                strSubject = "Order No : " + strFullOrderNo + ", Your order has been placed with Saraogi Super Sales Pvt Ltd";
                strMsgBody = "Dear Sir/Madam<br />Your order (<b>" + strFullOrderNo + "</b>) has been placed.<br />Thank you for trusting us.<br />Team,<br />Saraogi Super Sales Pvt Ltd.<br />" + strAddress + ".";

                var task = SendMail.SendEmail(strEmail, strSubject, strMsgBody, strpath,"", "ORDER FORM", false, strPEmailID);
                bool bStatus = task.Result;
                
                if (bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
        }



        private DataTable CreateTableFormat()
        {
            DataTable _table = new DataTable();
            _table.Columns.Add("HeaderImage", typeof(byte[]));
            _table.Columns.Add("BrandImage", typeof(byte[]));
            _table.Columns.Add("GSTNo", typeof(string));
            _table.Columns.Add("CINNo", typeof(string));
            _table.Columns.Add("PhoneNo", typeof(string));
            _table.Columns.Add("EmailID", typeof(string));
            _table.Columns.Add("Address", typeof(string));
            _table.Columns.Add("SerialNo", typeof(string));
            _table.Columns.Add("OrderNo", typeof(string));
            _table.Columns.Add("Date", typeof(string));
            _table.Columns.Add("SalesParty", typeof(string));
            _table.Columns.Add("SubParty", typeof(string));
            _table.Columns.Add("Supplier", typeof(string));
            _table.Columns.Add("Transport", typeof(string));
            _table.Columns.Add("Station", typeof(string));
            _table.Columns.Add("OrderType", typeof(string));
            _table.Columns.Add("DispatchDate", typeof(string));
            _table.Columns.Add("SchemeName", typeof(string));
            _table.Columns.Add("ItemName", typeof(string));
            _table.Columns.Add("PcsType", typeof(string));
            _table.Columns.Add("Qty", typeof(string));
            _table.Columns.Add("Amount", typeof(string));
            _table.Columns.Add("MarketerName", typeof(string));
            _table.Columns.Add("MobileNo", typeof(string));
            _table.Columns.Add("SNo", typeof(string));
            _table.Columns.Add("Remark", typeof(string));
            return _table;
        }

        private DataTable CreateDataTable(ref string strSEmailID,ref string strSMobileNo,ref string strPEmailID,ref string strPMobileNo,ref string strAddress)
        {
            DataTable _table = CreateTableFormat();
            string strQuery = "";
            strQuery += "Select GSTNo,CINNumber,(PhoneNo) as PhoneNo,EmailID,Address,StateName,PinCode,CEmailID,Password,SMTPServer,SMTPPort from CompanyDetails CD CROSS APPLY (Select OrderCode,cs.EmailID as CEmailID,cs.Password,SMTPServer,SMTPPort from CompanySetting cs Where cs.CompanyName=CD.Other)CS Where CS.OrderCode='" + txtBillCode.Text + "'  "
                     + " Select (OB.OrderCode + ' ' + CAST(OB.SerialNo as varchar)) SerialNo,RTRIM(LTRIM((CAST(OB.OrderNo as varchar) + '-' + OB.NumberCode))) OrderNo, CONVERT(varchar, Date, 103) ODate, (SalePartyID + ' ' + SM.Name)SalesParty, ISNULL((OB.SubPartyID + ' ' + SM2.Name), 'SELF')SubParty, (PurchasePartyID + ' ' + PName)SupplierName, Items as Items, Variant1 as DesignName, Pieces PcsType, Quantity, Amount, Transport, Station, ISNULL(Remark, '')Remark, CONVERT(varchar, DeliveryDate, 103) DDate, Marketer, OrderCategory, SNickName, PNickName, ISNULL(SSNickName, 'SELF') as SSNickName, SEmailID, SWhatsappNo, PEmailID, PWhatsappNo, Marka,[SchemeName] from AppOrderBooking OB left join(Select SM.AreaCode + Sm.AccountNo as SPartyID, Name, SM.Other as SNickName, EmailID SEmailID, SOD.WaybillUserName as SWhatsappNo from SupplierMaster SM inner join SupplierOtherDetails SOD on SM.AreaCode = SOD.AreaCode and SM.AccountNo = SOD.AccountNo Where SM.GroupName = 'SUNDRY DEBTORS')SM on SPartyID = OB.SalePartyID  left join(Select Sm.AreaCode + Sm.AccountNo as PPartyID, Name as PName, SM.Other as PNickName, EmailID PEmailID, SOD.WaybillUserName as PWhatsappNo from SupplierMaster SM inner join SupplierOtherDetails SOD on SM.AreaCode = SOD.AreaCode and SM.AccountNo = SOD.AccountNo WHere SM.GroupName = 'SUNDRY CREDITOR')SM1 on SM1.PPartyID = OB.PurchasePartyID  left join(Select SM.AreaCode + SM.AccountNo as SBPartyID, SM.Name, SM.Other as SSNickName from SupplierMaster SM WHere SM.GroupName = 'SUB PARTY') SM2 on SBPartyID = OB.SubPartyID Where Status!= 'HOLD' and OrderCode = '" + txtBillCode.Text + "' and OrderNo = '" + txtOrderNo.Text + "' and NumberCode = '" + txtNumberCode.Text + "' "
                     + " Select * from AppOrderDetails Where OrderCode = '" + txtBillCode.Text + "' and SerialNo = '" + txtBillNo.Text + "'  ";

            DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
            string strRemark = "";
            if(ds.Tables.Count>0)
            {
                DataTable dt = ds.Tables[0];
                if(dt.Rows.Count>0)
                {
                    DataRow _row = _table.NewRow();

                    DataRow row = dt.Rows[0];
                    _row["HeaderImage"] = MainPage._headerImage;
                    _row["BrandImage"] = MainPage._brandLogo;
                    _row["GSTNo"] = row["GSTNo"];
                    _row["CINNo"] = row["CINNumber"];
                    _row["PhoneNo"] = row["PhoneNo"];
                    _row["EmailID"] = row["EmailID"];
                    _row["Address"] = strAddress=row["Address"] + ", " + row["StateName"] + "-" + row["PinCode"];

                    DataTable __dt = ds.Tables[1];
                    row = __dt.Rows[0];                 
                    _row["SerialNo"] = row["SerialNo"];
                    _row["OrderNo"] = row["OrderNo"];
                    _row["Date"] = row["ODate"];
                    _row["SalesParty"] = row["SalesParty"];
                    _row["SubParty"] = row["SubParty"];
                    _row["Supplier"] = row["SupplierName"];
                    _row["Transport"] = row["Transport"];
                    _row["Station"] = row["Station"];
                    _row["OrderType"] = row["OrderCategory"];
                    _row["DispatchDate"] = row["DDate"];
                    _row["SchemeName"] = row["SchemeName"];
                    _row["MarketerName"] = row["Marketer"];
                    _row["Remark"] = strRemark= Convert.ToString(row["Remark"]);
                    _row["MobileNo"] = row["Marka"];

                    strSEmailID = Convert.ToString(row["SEmailID"]);
                    strSMobileNo = Convert.ToString(row["SWhatsappNo"]);
                    strPEmailID = Convert.ToString(row["PEmailID"]);
                    strPMobileNo = Convert.ToString(row["PWhatsappNo"]);

                    _table.Rows.Add(_row);
                    int _count = 0;
                    DataTable _dTable = ds.Tables[2];
                    foreach (DataRow __row in _dTable.Rows)
                    {
                        if (_count > 0)
                        {
                            DataRow dr = _table.NewRow();
                            dr["SNo"] = (_count + 1) + ".";
                            dr["ItemName"] = __row["DesignName"];
                            dr["PcsType"] = row["PcsType"];
                            dr["Qty"] = __row["Qty"];
                            dr["Amount"] = __row["Amount"];
                            _table.Rows.Add(dr);
                        }
                        else
                        {
                            _table.Rows[0]["SNo"] = (_count + 1) + ".";
                            _table.Rows[0]["ItemName"] = __row["DesignName"];
                            _table.Rows[0]["PcsType"] = row["PcsType"];
                            _table.Rows[0]["Qty"] = __row["Qty"];
                            _table.Rows[0]["Amount"] = __row["Amount"];
                        }
                        _count++;
                    }                

                }
                if (_table.Rows.Count > 0)
                {
                    _table.Rows[_table.Rows.Count - 1]["Remark"] = strRemark;
                }
            }

            
            return _table;
        }


      
    }
}
