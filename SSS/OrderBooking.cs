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
    public partial class OrderBooking : Form
    {
        DataBaseAccess dba;
        DataTable dtItemName;
        int itemRowIndex = 0;
        bool newStatus = false, _bDeliveryDateReq = false, _bOrderStatus = false, _bSchemeActiveStatus = false;
        public bool updateStatus = false,bMasterUpdateStatus=false;
        public string strAddedOrderDetails = "", _strMainOrderCode = "",_strCurrentOrderCode="",_strOrderNo_Update,_STRMasterTransportName="";
        public string _strPSalesParty = "", _strPSubParty = "", _strPackingType = "", _strPPurchaseParty = "",_strQty="0",_strAmount="0",_strItemName="",_strGRSNO="";

        string[] strAlphabate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" ,"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ" , "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ", "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ", "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ", "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ", "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ", "IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV", "IW", "IX", "IY", "IZ", "JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS", "JT", "JU", "JV", "JW", "JX", "JY", "JZ", "KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KI", "KJ", "KK", "KL", "KM", "KN", "KO", "KP", "KQ", "KR", "KS", "KT", "KU", "KV", "KW", "KX", "KY", "KZ", "LA", "LB", "LC", "LD", "LE", "LF", "LG", "LH", "LI", "LJ", "LK", "LL", "LM", "LN", "LO", "LP", "LQ", "LR", "LS", "LT", "LU", "LV", "LW", "LX", "LY", "LZ", "MA", "MB", "MC", "MD", "ME", "MF", "MG", "MH", "MI", "MJ", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NB", "NC", "ND", "NE", "NF", "NG", "NH", "NI", "NJ", "NK", "NL", "NM", "NN", "NO", "NP", "NQ", "NR", "NS", "NT", "NU", "NV", "NW", "NX", "NY", "NZ" };
        public OrderBooking()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            BindLastRecord();

            var arr =System.Text.RegularExpressions.Regex.Matches("", @"\b[A-Za-z-']+\b")
                     .Cast<System.Text.RegularExpressions.Match>()
                     .Select(m => m.Value)
                     .ToArray();
        }

        public OrderBooking(bool nStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtSerialNo.TabStop = txtDate.TabStop = txtSalesParty.TabStop=txtSubParty.TabStop= false;
            btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = btnGAdd.Enabled = btnRemove.Enabled = false;
            GetStartupData();
            newStatus = nStatus;
        }

        public OrderBooking(string strCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            txtCode.Text = strCode;
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData()
        {
            try
            {
                string strLastSerialNo = "", strQuery = "Select OrderCode,(Select ISNULL(MAX(SerialNo),'') from OrderBooking)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' Select Distinct ItemName from Items  Where ItemName!='' order by ItemName ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        _strMainOrderCode = txtCode.Text = Convert.ToString(dt.Rows[0]["OrderCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                    dtItemName = ds.Tables[1];
                    BindItemWithGrid();
                }
                SetCategory();
            }
            catch
            {
            }
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdRetailOrderBook.Columns["rt_Variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdRetailOrderBook.Columns["rt_Variant1"].Visible = true;
                }
                else
                    dgrdRetailOrderBook.Columns["rt_Variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdRetailOrderBook.Columns["rt_Variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdRetailOrderBook.Columns["rt_Variant2"].Visible = true;
                }
                else
                    dgrdRetailOrderBook.Columns["rt_Variant2"].Visible = false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdRetailOrderBook.Columns["rt_Variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdRetailOrderBook.Columns["rt_Variant3"].Visible = true;
                }
                else
                    dgrdRetailOrderBook.Columns["rt_Variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdRetailOrderBook.Columns["rt_Variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdRetailOrderBook.Columns["rt_Variant4"].Visible = true;
                }
                else
                    dgrdRetailOrderBook.Columns["rt_Variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdRetailOrderBook.Columns["rt_Variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdRetailOrderBook.Columns["rt_Variant5"].Visible = true;
                }
                else
                    dgrdRetailOrderBook.Columns["rt_Variant5"].Visible = false;
            }
            catch
            {
            }
        }

        private void BindItemWithGrid()
        {
            try
            {
                if (dtItemName.Rows.Count > 0)
                {
                    dgrdItem.Rows.Add(dtItemName.Rows.Count);
                    for (int i = 0; i < dtItemName.Rows.Count; ++i)
                    {
                        dgrdItem.Rows[i].Cells["chkItem"].Value = (Boolean)false;
                        dgrdItem.Rows[i].Cells["itemName"].Value = dtItemName.Rows[i]["ItemName"];
                    }
                }
            }
            catch
            {
            }
        }


        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(SerialNo),'') from OrderBooking Where OrderCode='" + txtCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(SerialNo),'') from OrderBooking Where OrderCode='" + txtCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(SerialNo),'') from OrderBooking Where OrderCode='" + txtCode.Text + "' and SerialNo>" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(SerialNo),'') from OrderBooking Where OrderCode='" + txtCode.Text + "' and SerialNo<" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                string strQuery = "Select OB.*,(CONVERT(varchar,DeliveryDate,103)) DelDate,(CONVERT(varchar,Date,103)) BDate,(SalePartyID+' '+SM.Name) SParty,(CASE WHEN SubPartyID='SELF' then SubPartyID else SubPartyID+' '+SubName end) HParty,dbo.GetFullName(PurchasePartyID) PParty,(CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0))) PQty,PartyTransport,ISNULL(STransport,'')STransport,ISNULL(ActiveStatus,1) as ActiveStatus from OrderBooking OB OUTER APPLY (Select Name,Transport as PartyTransport from SupplierMaster Sm WHere (SM.AreaCode+SM.AccountNo)=OB.SalePartyID)SM OUTER APPLY (Select Name as SubName,Transport as STransport from SupplierMaster Sm WHere (SM.AreaCode+SM.AccountNo)=OB.SubPartyID)SMSub  OUTER APPLY (Select ActiveStatus from SchemeMaster SM Where OB.SchemeName=SM.SchemeName)_SM Where SerialNo=" + strSerialNo + " and OrderCode='" + txtCode.Text + "' Order by ID";

                DataTable dt = dba.GetDataTable(strQuery);
                lblCreatedBy.Text = txtReason.Text = _STRMasterTransportName= "";
                pnlDeletionConfirmation.Visible = false;
                DisableAllControls();
                txtSerialNo.ReadOnly = false;
                _bDeliveryDateReq = true;
                double dTAmt = 0, dAmt = 0;
                bool _clearStatus = bMasterUpdateStatus= false;
                _bOrderStatus = false;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        _strCurrentOrderCode = txtCode.Text;
                        txtSerialNo.Text = strSerialNo;
                        txtDate.Text = Convert.ToString(row["BDate"]);
                        txtBookingNo.Text = Convert.ToString(row["BookingNo"]);
                        txtMarketer.Text = Convert.ToString(row["Marketer"]);
                        txtSalesParty.Text = Convert.ToString(row["SParty"]);
                        txtSubParty.Text = Convert.ToString(row["HParty"]);
                        txtTransport.Text = Convert.ToString(row["Transport"]);                      
                        txtStation.Text = Convert.ToString(row["Station"]);
                        txtScheme.Text = Convert.ToString(row["SchemeName"]);                       
                        txtRemark.Text = Convert.ToString(row["MRemark"]);
                        _bSchemeActiveStatus = Convert.ToBoolean(row["ActiveStatus"]);

                        string strOrderCategory = Convert.ToString(row["OrderCategory"]);
                        if (strOrderCategory == "**")
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
                        dgrdRetailOrderBook.Rows.Clear();

                        if (Convert.ToString(row["OrderType"]) != "RETAILORDER")
                        {
                            dgrdOrder.Rows.Add(dt.Rows.Count);
                            int rowIndex = 0;
                            foreach (DataRow rows in dt.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows["Amount"]);

                                dgrdOrder.Rows[rowIndex].Cells["id"].Value = rows["ID"];
                                dgrdOrder.Rows[rowIndex].Cells["chkDelete"].Value = false;
                                dgrdOrder.Rows[rowIndex].Cells["orderNo"].Value = rows["OrderNo"];
                                dgrdOrder.Rows[rowIndex].Cells["orderCode"].Value = rows["NumberCode"];
                                dgrdOrder.Rows[rowIndex].Cells["item"].Value = rows["Items"];
                                dgrdOrder.Rows[rowIndex].Cells["qtyType"].Value = rows["Pieces"];
                                dgrdOrder.Rows[rowIndex].Cells["qty"].Value = rows["Quantity"];
                                dgrdOrder.Rows[rowIndex].Cells["amt"].Value = dAmt;// rows["Amount"];
                                dgrdOrder.Rows[rowIndex].Cells["remark"].Value = rows["Remark"];
                                dgrdOrder.Rows[rowIndex].Cells["adjustedQty"].Value = rows["AdjustedQty"];
                                dgrdOrder.Rows[rowIndex].Cells["cancelQty"].Value = rows["CancelQty"];
                                dgrdOrder.Rows[rowIndex].Cells["graceDays"].Value = rows["OfferName"];
                                dgrdOrder.Rows[rowIndex].Cells["pendingQty"].Value = rows["PQty"];
                                dgrdOrder.Rows[rowIndex].Cells["deliveryDate"].Value = rows["DelDate"];

                                if (Convert.ToString(rows["PParty"]) == "")
                                    dgrdOrder.Rows[rowIndex].Cells["pparty"].Value = rows["Personal"];
                                else
                                    dgrdOrder.Rows[rowIndex].Cells["pparty"].Value = rows["PParty"];

                                if (Convert.ToString(rows["Status"]).ToUpper() == "CLEAR")
                                {
                                    dgrdOrder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                    _clearStatus = true;
                                }

                                dTAmt += dAmt;

                                rowIndex++;
                            }
                            dgrdOrder.FirstDisplayedCell = dgrdOrder.Rows[0].Cells[2];
                            _orderTab.SelectedIndex = 0;
                        }
                        else
                        {
                            dgrdRetailOrderBook.Rows.Add(dt.Rows.Count);
                            int rowIndex = 0;
                            txtBookingNumber.Text = Convert.ToString(row["OrderNo"]);
                            txtDeliveryDate.Text = Convert.ToString(row["DelDate"]);
                            double dQty = 0,dRate=0;

                            foreach (DataRow rows in dt.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows["Amount"]);
                                dQty = dba.ConvertObjectToDouble(rows["Quantity"]);
                                if (dQty != 0 && dAmt != 0)
                                    dRate = (dAmt / dQty);
                                else
                                    dRate = 0;

                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_SNo"].Value = rowIndex+1;
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_ID"].Value = rows["ID"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_ItemName"].Value = rows["Items"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant1"].Value = rows["Variant1"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant2"].Value = rows["Variant2"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant3"].Value = rows["Variant3"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant4"].Value = rows["Variant4"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant5"].Value = rows["Variant5"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"].Value = rows["Quantity"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Remark"].Value = rows["Remark"];
                                //dgrdRetailOrderBook.Rows[rowIndex].Cells["adjustedQty"].Value = rows["AdjustedQty"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_CancelQty"].Value = rows["CancelQty"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_PendingQty"].Value = rows["PQty"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_NumberCode"].Value = rows["NumberCode"];
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Amount"].Value = dAmt;
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Rate"].Value = dRate;

                                if (Convert.ToString(rows["Status"]).ToUpper() == "CLEAR")
                                {
                                    dgrdRetailOrderBook.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                    _clearStatus = true;
                                }                              

                                rowIndex++;
                            }


                            dgrdRetailOrderBook.FirstDisplayedCell = dgrdRetailOrderBook.Rows[0].Cells[1];
                            _orderTab.SelectedIndex = 1;
                        }
                    }
                }

                if (dgrdOrder.Rows.Count == 0)
                {
                    dgrdOrder.Rows.Add();                   
                }

                if (dgrdRetailOrderBook.Rows.Count == 0)
                {
                    dgrdRetailOrderBook.Rows.Add();
                    dgrdRetailOrderBook.Rows[0].Cells["rt_SNo"].Value = 1;
                }
                txtSalesParty.Enabled=txtSubParty.Enabled= txtScheme.Enabled = !_clearStatus;

                if (MainPage.strUserRole.Contains("ADMIN"))
                    txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = true;
                if (txtScheme.Enabled && !_bSchemeActiveStatus)
                    txtScheme.Enabled = _bSchemeActiveStatus;

               lblNetAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
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
                    else if (pnlItems.Visible)
                        pnlItems.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !dgrdItem.Focused && !dgrdOrder.Focused && !dgrdRetailOrderBook.Focused)
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
                            if (txtSerialNo.Text != "")
                            {
                                btnAdd.Text = "&Save";
                                SetSerialNo();
                                EnableAllControls();
                                txtMarketer.Focus();
                            }
                        }
                    }
                }
            }
            catch { }
        }


        private void EnableAllControls()
        {
            txtBookingNo.ReadOnly = txtDeliveryDate.ReadOnly = txtRemark.ReadOnly = txtDate.ReadOnly = txtBookingNo.ReadOnly = dgrdItem.ReadOnly = dgrdPending.ReadOnly = false;
            rdoNew.Enabled = rdoPending.Enabled = grpCategory.Enabled = true;
            if (dgrdOrder.Rows.Count < 2 && _bSchemeActiveStatus)
                txtScheme.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtBookingNo.ReadOnly = txtDeliveryDate.ReadOnly = txtRemark.ReadOnly= txtDate.ReadOnly = txtBookingNo.ReadOnly =  dgrdItem.ReadOnly = dgrdPending.ReadOnly = true;
            rdoNew.Enabled = rdoPending.Enabled = grpCategory.Enabled = false;
        }

        private void ClearAllText()
        {
            txtBookingNo.Text=txtDeliveryDate.Text= txtRemark.Text = lblCreatedBy.Text = txtBookingNo.Text = txtMarketer.Text = txtSalesParty.Text = txtSubParty.Text = txtTransport.Text = txtStation.Text = txtScheme.Text = "";
            rdoNew.Checked = txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = true;
            dgrdOrder.Rows.Clear();
            dgrdOrder.Rows.Add(1);
            dgrdRetailOrderBook.Rows.Clear();
            dgrdRetailOrderBook.Rows.Add(1);
            dgrdPending.Rows.Clear();
            dgrdRetailOrderBook.Rows[0].Cells["rt_SNo"].Value = 1;
            _strCurrentOrderCode = _STRMasterTransportName= "";
            bMasterUpdateStatus = _bOrderStatus = _bSchemeActiveStatus= false;
            lblNetAmt.Text = "0.00";
            rdoSingle.Checked = true;

            txtCode.Text = _strMainOrderCode;

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

            // SetDesignSerialNo();
        }

        private void SetSerialNo()
        {
            txtSerialNo.Text = "";
               object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(SerialNo),0)+1)SerialNo from OrderBooking Where (OrderCode='" + _strMainOrderCode + "' OR OrderCode Like('%OD')) ");
            txtSerialNo.Text = Convert.ToString(objValue);
        }

        private void SetSerialNoOfDummy()
        {
            txtSerialNo.Text = "";
            string strQuery = "Select (ISNULL(MAX(SerialNo),0)+1)SerialNo,(Select (ISNULL(MAX(OrderNo),0)+1)OrderNo from OrderBooking Where OrderCode='" + txtCode.Text + "')OrderNo from OrderBooking Where (OrderCode='" + _strMainOrderCode + "' OR OrderCode Like('%OD')) ";
            DataTable _dt = dba.GetDataTable(strQuery);
            if (_dt.Rows.Count > 0)
            {
                txtSerialNo.Text = Convert.ToString(_dt.Rows[0]["SerialNo"]);
                if (dgrdOrder.Rows.Count == 0)
                    dgrdOrder.Rows.Add(1);

                dgrdOrder.Rows[0].Cells["orderNo"].Value = Convert.ToString(_dt.Rows[0]["OrderNo"]);
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
                        SearchData objSearch = new SearchData("MARKETERNAME", "SEARCH MARKETER NAME", e.KeyCode);
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
            dgrdPending.Rows.Clear();
            _bOrderStatus = false;
            txtTransport.Text =  txtStation.Text = "";//txtBStation.Text = 
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                string strSaleParty = "", strSubParty = "";
                bool tStatus = true;
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSaleParty = strFullName[0].Trim();
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                    strSubParty = strFullName[0].Trim();

                if (strSaleParty != "" && strSubParty != "")
                {
                    string strQuery = " Select * from ( Select (ReceiptCode+' '+CAST(ReceiptNo as varchar)) RCode,Convert(varchar,ReceivingDate,103)RDate,(CASE When PurchasePartyID!='' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Item,Pieces,(CASE WHEN Pieces='PETI' then ISNULL(NoOfCase,1) else Quantity end)Quantity,Amount,'PURCHASE' as BillType,ReceivingDate as Date,0 as PendingQty,0 CancelQty,0 as ID,'' as Transport,'' as Station from GoodsReceive Where (OrderNo='0' OR OrderNo='') and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' UNION ALL "
                                    + " Select (OrderCode + ' ' + CAST(SerialNo as varchar)) RCode,Convert(varchar, Date, 103)RDate,(CASE When PurchasePartyID!= '' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Items,Pieces,Quantity,Amount,'ORDER' as BillType,Date,CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)) as PendingQty,CancelQty,ID,Transport,Station from OrderBooking Where SalePartyID = '" + strSaleParty+"' and SubPartyID = '"+strSubParty+"' and OrderCode Like('%OD') )_Order Order by Date desc ";

                    if (strSubParty == "SELF")
                        strQuery += " Select Transport,PvtMarka,Station,BookingStation,TransactionLock,BlackList from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";
                    else
                        strQuery += " Select Transport,PvtMarka,Station,BookingStation,TransactionLock,BlackList from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSubParty + "' ";

                    strQuery += " Select TransactionLock,GroupII,BlackList,TINNumber,UPPER(Other1) as OrangeZone from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[2];
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

                            if (dt.Rows.Count > 0)
                            {
                                int rowIndex = 0;
                                dgrdPending.Rows.Add(dt.Rows.Count);
                                foreach (DataRow row in dt.Rows)
                                {
                                    dgrdPending.Rows[rowIndex].Cells["grChk"].Value = false;
                                    dgrdPending.Rows[rowIndex].Cells["grsno"].Value = row["RCode"];
                                    dgrdPending.Rows[rowIndex].Cells["grDate"].Value = row["RDate"];
                                    dgrdPending.Rows[rowIndex].Cells["grPParty"].Value = row["PParty"];
                                    dgrdPending.Rows[rowIndex].Cells["grItem"].Value = row["Item"];
                                    dgrdPending.Rows[rowIndex].Cells["grQtyType"].Value = row["Pieces"];
                                    dgrdPending.Rows[rowIndex].Cells["grQty"].Value = row["Quantity"];
                                    dgrdPending.Rows[rowIndex].Cells["billType"].Value = row["BillType"];
                                    dgrdPending.Rows[rowIndex].Cells["gCancelQty"].Value = row["CancelQty"];
                                    dgrdPending.Rows[rowIndex].Cells["gPendingQty"].Value = row["PendingQty"];
                                    dgrdPending.Rows[rowIndex].Cells["gID"].Value = row["ID"];
                                    dgrdPending.Rows[rowIndex].Cells["gTransport"].Value = row["Transport"];
                                    dgrdPending.Rows[rowIndex].Cells["gStation"].Value = row["Station"];
                                    dgrdPending.Rows[rowIndex].Cells["grAmt"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                                    rowIndex++;
                                }
                            }
                            dt.Clear();
                            dt = ds.Tables[1];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                if (row != null)
                                {
                                    _STRMasterTransportName = Convert.ToString(row["Transport"]);//txtTransport.Text
                                    //txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
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

        private void btnGAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int rowIndex = 0;
                string strBillType = "";
                for (int _index = 0; _index < dgrdPending.RowCount; _index++)
                {
                    DataGridViewRow row = dgrdPending.Rows[_index];
                    if (Convert.ToBoolean(row.Cells["grChk"].EditedFormattedValue))
                    {
                        strBillType = Convert.ToString(row.Cells["billType"].Value);
                        if (strBillType == "PURCHASE")
                        {
                            if (CheckBillAdjustment(Convert.ToString(row.Cells["grsno"].Value)))
                            {
                                //if (txtScheme.Text == "" || !Convert.ToString(row.Cells["grPParty"].Value).Contains("DL5255"))
                                {

                                    rowIndex = dgrdOrder.Rows.Count;
                                    dgrdOrder.Rows.Add();
                                    dgrdOrder.Rows[rowIndex].Cells["chkDelete"].Value = false;
                                    dgrdOrder.Rows[rowIndex].Cells["orderNo"].Value = dgrdOrder.Rows[rowIndex].Cells["orderCode"].Value = "";
                                    dgrdOrder.Rows[rowIndex].Cells["pparty"].Value = row.Cells["grPParty"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["item"].Value = row.Cells["grItem"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["qtyType"].Value = row.Cells["grQtyType"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["qty"].Value = row.Cells["grQty"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["amt"].Value = row.Cells["grAmt"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["grSerial"].Value = row.Cells["grsno"].Value;
                                    dgrdOrder.Rows[rowIndex].Cells["deliveryDate"].Value = row.Cells["grDate"].Value;

                                    dgrdPending.Rows.RemoveAt(_index);
                                    _index--;
                                }
                            }
                        }
                        else
                        {
                            dgrdOrder.Rows.Clear();
                            dgrdOrder.Rows.Add(1);
                            string strOrder = Convert.ToString(row.Cells["grsno"].Value);
                            string[] _strOrderNo = strOrder.Split(' ');
                            if (_strOrderNo.Length > 1)
                            {
                                _strCurrentOrderCode = txtCode.Text = _strOrderNo[0];
                                txtSerialNo.Text = _strOrderNo[1];
                            }
                            _bOrderStatus = true;
                            dgrdOrder.Rows[rowIndex].Cells["chkDelete"].Value = false;
                            dgrdOrder.Rows[rowIndex].Cells["orderNo"].Value = dgrdOrder.Rows[rowIndex].Cells["orderCode"].Value = "";
                            dgrdOrder.Rows[rowIndex].Cells["pparty"].Value = row.Cells["grPParty"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["item"].Value = row.Cells["grItem"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["qtyType"].Value = row.Cells["grQtyType"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["adjustedQty"].Value = dgrdOrder.Rows[rowIndex].Cells["qty"].Value = row.Cells["grQty"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["amt"].Value = row.Cells["grAmt"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["grSerial"].Value = "";
                            dgrdOrder.Rows[rowIndex].Cells["deliveryDate"].Value = txtDate.Text = Convert.ToString(row.Cells["grDate"].Value);
                            dgrdOrder.Rows[rowIndex].Cells["cancelQty"].Value = row.Cells["gCancelQty"].Value;//,0 CancelQty
                            dgrdOrder.Rows[rowIndex].Cells["pendingQty"].Value = row.Cells["gPendingQty"].Value;
                            dgrdOrder.Rows[rowIndex].Cells["id"].Value = row.Cells["gID"].Value;
                            txtTransport.Text = Convert.ToString(row.Cells["gTransport"].Value);
                            txtStation.Text = Convert.ToString(row.Cells["gStation"].Value);

                            dgrdPending.Rows.RemoveAt(_index);
                            _index--;

                            btnEdit.Text = "&Update";
                            btnAdd.Text = "&Add";

                            txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = false;

                            if (MainPage.strUserRole.Contains("ADMIN"))
                                txtSalesParty.Enabled = txtSubParty.Enabled = txtScheme.Enabled = true;
                            txtCode.Focus();
                            break;
                        }
                    }
                }
            }
            catch
            {
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
                    if (e.ColumnIndex > 0 && e.ColumnIndex<12)
                    {
                        if (Convert.ToString(dgrdOrder.CurrentRow.Cells["grSerial"].Value) != "")
                        {
                            if (e.ColumnIndex != 2 && e.ColumnIndex != 3)
                                e.Cancel = true;
                        }
                        else if (e.ColumnIndex == 4)
                        {
                            if (dba.ConvertObjectToDouble(dgrdOrder.CurrentRow.Cells["adjustedQty"].Value) == 0)
                            {
                                SearchData objSearch = new SearchData("PURCHASEPERSONALPARTY", "SEARCH PARTY NAME", Keys.Space);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    dgrdOrder.CurrentCell.Value = objSearch.strSelectedData;
                                    if (objSearch.strSelectedData != "PERSONAL")
                                    {
                                        bool _blackListed = false;

                                        if (dba.CheckTransactionLockWithBlackList(objSearch.strSelectedData, ref _blackListed))
                                        {
                                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            dgrdOrder.CurrentCell.Value = "";
                                        }
                                        else if (_blackListed)
                                        {
                                            MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            dgrdOrder.CurrentCell.Value = "";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! This order is adjusted with purchase bill.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            e.Cancel = true;
                        }
                        else if (e.ColumnIndex == 5)
                        {
                            itemRowIndex = e.RowIndex;
                            pnlItems.Visible = true;
                            txtSearchItem.Focus();
                            SetItemToGrid(Convert.ToString(dgrdOrder.CurrentCell.Value));
                            txtSearchItem.Focus();
                            e.Cancel = true;
                        }
                        else if (e.ColumnIndex == 6)
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
                        else if (e.ColumnIndex == 12)
                        {
                            if (MainPage.mymainObject.bFullEditControl)
                            {
                                if (dba.ConvertObjectToDouble(dgrdOrder.CurrentRow.Cells["adjustedQty"].Value) == 0)
                                {
                                    SearchData objSearch = new SearchData("OFFERNAME", "ACTIVE", "SEARCH GRACE DAYS", Keys.Space);
                                    objSearch.ShowDialog();
                                    dgrdOrder.CurrentCell.Value = objSearch.strSelectedData;
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! This order is adjusted with purchase bill.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            e.Cancel = true;
                        }
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
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

        private void btnEmailCancel_Click(object sender, EventArgs e)
        {
            pnlItems.Visible = false;
        }

        private void btnEmailAdd_Click(object sender, EventArgs e)
        {
            AddSelectedItems();
        }

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
                dgrdOrder.Rows[itemRowIndex].Cells["item"].Value = strItemName;
                dgrdOrder.CurrentCell = dgrdOrder.Rows[itemRowIndex].Cells["qtyType"];
                dgrdOrder.Focus();
                pnlItems.Visible = false;
            }
            catch
            {
            }
        }

        private void SearchItemByKey()
        {
            try
            {
                DataRow[] fileterrow = dtItemName.Select(String.Format("ItemName Like('" + txtSearchItem.Text + "%') "));
                if (fileterrow.Length > 0)
                {
                    string strName = Convert.ToString(fileterrow[0]["ItemName"]);
                    int index = 0;
                    foreach (DataGridViewRow row in dgrdItem.Rows)
                    {
                        if (Convert.ToString(row.Cells["itemName"].Value) == strName)
                            break;
                        index++;
                    }
                    dgrdItem.CurrentCell = dgrdItem.Rows[index].Cells[0];
                    dgrdItem.FirstDisplayedCell = dgrdItem.CurrentCell;
                }
            }
            catch
            {
            }
        }

        private void txtSearchItem_TextChanged(object sender, EventArgs e)
        {
            SearchItemByKey();
        }

        private void dgrdItem_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = true;
            }
        }

        private void dgrdItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgrdItem.CurrentCell = dgrdItem.CurrentRow.Cells[dgrdItem.CurrentCell.ColumnIndex + 1];
                    AddSelectedItems();
                }
            }
            catch
            {
            }
        }

        private void dgrdOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
            if (cIndex == 2 || cIndex == 3 || cIndex == 7 || cIndex == 8 || cIndex == 9 || cIndex == 10 || cIndex == 11)//|| cIndex == 7
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.CharacterCasing = CharacterCasing.Upper;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
            if (cIndex == 2 || cIndex == 7 || cIndex == 8 || cIndex == 9 || cIndex == 10)//|| cIndex == 7
                dba.KeyHandlerPoint(sender, e, 0);
            else if (cIndex == 3 || cIndex == 11)
                dba.ValidateSpace(sender, e);
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

                    if (IndexColmn < dgrdOrder.ColumnCount - 4)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                            dgrdOrder.CurrentCell = dgrdOrder.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdOrder.RowCount - 1)
                    {
                        if (!_bOrderStatus && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["orderNo"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["pparty"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["item"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["qty"].Value) != "" && Convert.ToString(dgrdOrder.Rows[CurrentRow].Cells["qtyType"].Value) != "")
                        {
                            dgrdOrder.Rows.Add(1);
                            dgrdOrder.Rows[CurrentRow + 1].Cells["deliveryDate"].Value = dgrdOrder.Rows[CurrentRow].Cells["deliveryDate"].Value;
                            dgrdOrder.CurrentCell = dgrdOrder.Rows[CurrentRow + 1].Cells["orderNo"];
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
                    //if (dgrdOrder.CurrentCell.ColumnIndex == 4 && dgrdOrder.CurrentCell.RowIndex >= 0)
                    //{
                    //    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    //    string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                    //    if (strData != "")
                    //    {
                    //        dgrdOrder.CurrentCell.Value = strData;
                    //    }
                    //}
                    RemoveSelectedData(dgrdOrder.CurrentRow);
                }
            }
            catch
            {
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveSelectedData();
            }
            catch
            {
            }
        }

        private void RemoveSelectedData(DataGridViewRow row)
        {
            int rowIndex = 0, _index = row.Index;

            if (btnAdd.Text == "&Save")
            {


                if (Convert.ToString(row.Cells["grSerial"].Value) != "")
                {
                    rowIndex = dgrdPending.Rows.Count;
                    dgrdPending.Rows.Add();
                    dgrdPending.Rows[rowIndex].Cells["grChk"].Value = false;
                    dgrdPending.Rows[rowIndex].Cells["grPParty"].Value = row.Cells["pparty"].Value;
                    dgrdPending.Rows[rowIndex].Cells["grItem"].Value = row.Cells["item"].Value;
                    dgrdPending.Rows[rowIndex].Cells["grQtyType"].Value = row.Cells["qtyType"].Value;
                    dgrdPending.Rows[rowIndex].Cells["grQty"].Value = row.Cells["qty"].Value;
                    dgrdPending.Rows[rowIndex].Cells["grAmt"].Value = row.Cells["amt"].Value;
                    dgrdPending.Rows[rowIndex].Cells["grsno"].Value = row.Cells["grSerial"].Value;

                    dgrdOrder.Rows.RemoveAt(_index);
                    _index--;
                }
                else
                {
                    dgrdOrder.Rows.RemoveAt(_index);
                    _index--;
                }
            }
            else if (btnEdit.Text == "&Update")
            {
                string strID = Convert.ToString(row.Cells["id"].Value);
                if (strID != "")
                {
                    if (dgrdItem.Rows.Count > 1)
                    {
                        if (CheckAdjustedQty(row))
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to remove permanently ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (DeleteSingleRow(strID))
                                    dgrdOrder.Rows.Remove(row);
                            }
                        }
                    }
                }
                else
                    dgrdOrder.Rows.Remove(row);
            }
        }

        private void RemoveSelectedData()
        {
            int rowIndex = 0;
            for (int _index = 0; _index < dgrdOrder.RowCount; _index++)
            {
                DataGridViewRow row = dgrdOrder.Rows[_index];
                if (Convert.ToBoolean(row.Cells["chkDelete"].Value))
                {
                    if (btnAdd.Text == "&Save")
                    {
                        if (Convert.ToString(row.Cells["grSerial"].Value) != "")
                        {
                            rowIndex = dgrdPending.Rows.Count;
                            dgrdPending.Rows.Add();
                            dgrdPending.Rows[rowIndex].Cells["grChk"].Value = false;
                            dgrdPending.Rows[rowIndex].Cells["grPParty"].Value = row.Cells["pparty"].Value;
                            dgrdPending.Rows[rowIndex].Cells["grItem"].Value = row.Cells["item"].Value;
                            dgrdPending.Rows[rowIndex].Cells["grQtyType"].Value = row.Cells["qtyType"].Value;
                            dgrdPending.Rows[rowIndex].Cells["grQty"].Value = row.Cells["qty"].Value;
                            dgrdPending.Rows[rowIndex].Cells["grAmt"].Value = row.Cells["amt"].Value;
                            dgrdPending.Rows[rowIndex].Cells["grsno"].Value = row.Cells["grSerial"].Value;

                            dgrdOrder.Rows.RemoveAt(_index);
                            _index--;
                        }
                        else
                        {
                            dgrdOrder.Rows.RemoveAt(_index);
                            _index--;
                        }
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(row.Cells["id"].Value);
                        if (strID != "")
                        {
                            if (dgrdItem.Rows.Count > 1)
                            {
                                if (CheckAdjustedQty(row))
                                {
                                    DialogResult result = MessageBox.Show("Are you sure want to remove permanently ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (result == DialogResult.Yes)
                                    {
                                        if (DeleteSingleRow(strID))
                                            dgrdOrder.Rows.Remove(row);
                                    }
                                }
                            }
                        }
                        else
                            dgrdOrder.Rows.Remove(row);
                    }
                }
            }
        }

        private bool DeleteSingleRow(string strID)
        {
            string strOrderNo = "", strQuery = "";
            object value = DataBaseAccess.ExecuteMyScalar("Select (CASE When OB.NumberCode!='' then(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end) OrderNo from OrderBooking Ob Where OrderCode='" + txtCode.Text + "' and ID=" + strID + " ");
            strOrderNo = Convert.ToString(value);
            if (strOrderNo != "")
            {
                strQuery += " Update GoodsReceive Set OrderNo='' Where OrderNo='" + strOrderNo + "' "
                         + " Update OrderBooking Set UpdatedBy='"+MainPage.strLoginName+"' Where OrderCode='"+txtCode.Text+"' and SerialNo="+txtSerialNo.Text+" "
                         + " Delete from OrderBooking Where (CASE When NumberCode!='' then(OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)='" + strOrderNo + "' "
                         + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                         + " ('ORDER','" + txtCode.Text + "'," + txtSerialNo.Text + ",'" + strOrderNo + " NO DELETE FROM ORDER BOOKING, with Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";


                //+ " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                //+ " ('ORDER','" + txtCode.Text + "'," + txtSerialNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    DataBaseAccess.CreateDeleteQuery(strQuery);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        //private void EnableControl()
        //{
        //    txtBookingNo.ReadOnly = txtPvtMarka.ReadOnly = txtDate.ReadOnly = dgrdOrder.ReadOnly = false;
        //    rdoNew.Enabled = rdoPending.Enabled = true;
        //}

        //private void EnableControl()
        //{
        //    txtBookingNo.ReadOnly = txtPvtMarka.ReadOnly = txtDate.ReadOnly = dgrdOrder.ReadOnly = false;
        //    rdoNew.Enabled = rdoPending.Enabled = btnGAdd.Enabled = true;
        //}

        //private void DisableControl()
        //{
        //    txtBookingNo.ReadOnly = txtPvtMarka.ReadOnly = txtDate.ReadOnly = dgrdOrder.ReadOnly = true;
        //    rdoNew.Enabled = rdoPending.Enabled = btnGAdd.Enabled= false;
        //}

        //private void ClearAllText()
        //{
        //    try
        //    {
        //        txtBookingNo.Text = txtMarketer.Text = txtSalesParty.Text = txtSubParty.Text = txtTransport.Text = txtPvtMarka.Text = txtStation.Text = txtBStation.Text = "";
        //        dgrdPending.Rows.Clear();
        //        dgrdOrder.Rows.Clear();
        //        rdoNew.Checked = true;
        //        dgrdOrder.Rows.Add();
        //        if (DateTime.Now >= MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
        //            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        //        else
        //            txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        //    }
        //    catch
        //    {
        //    }
        //}

        private bool ValidateControls()
        {
            try
            {
                if (txtCode.Text == "")
                {
                    MessageBox.Show("Sorry ! Order code can't be blank !!", "Order code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCode.Focus();
                    return false;
                }
                if (txtSerialNo.Text == "")
                {
                    MessageBox.Show("Sorry ! Serial no can't be blank !!", "Serial no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSerialNo.Focus();
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
                if (txtTransport.Text == "")
                {
                    MessageBox.Show("Sorry ! Transport name can't be blank !!", "Transport Name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTransport.Focus();
                    return false;
                }
                if (txtStation.Text == "")
                {
                    MessageBox.Show("Sorry ! Station name can't be blank !!", "Station Name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStation.Focus();
                    return false;
                }
                if (txtMarketer.Text == "" && !txtCode.Text.Contains("OD"))
                {
                    MessageBox.Show("Sorry ! Marketer name can't be blank in original order !!", "Marketer Name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMarketer.Focus();
                    return false;
                }

                if (btnAdd.Text == "&Save" && _STRMasterTransportName !=txtTransport.Text)
                {
                    DialogResult result= MessageBox.Show("Sorry ! Transport name in master and summary doesn't match,\nAre you want to update this transport in Party master ?  !!", "Transport name mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        bMasterUpdateStatus = true;
                    else
                        bMasterUpdateStatus = false;
                }

                double dAmt = 0, dQty = 0;
                string strGraceDays = "", strDeliveryDate = "";
                DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                string strSupplierName = "";
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    string strID = Convert.ToString(row.Cells["id"].Value), strOrderNo = Convert.ToString(row.Cells["orderNo"].Value), strPParty = Convert.ToString(row.Cells["pparty"].Value), strItem = Convert.ToString(row.Cells["item"].Value);
                    strGraceDays = Convert.ToString(row.Cells["graceDays"].Value);
                    strDeliveryDate = Convert.ToString(row.Cells["deliveryDate"].Value);

                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (strID == "" && strOrderNo == "" && strPParty == "" && strItem == "" && dQty == 0)
                        dgrdOrder.Rows.Remove(row);
                    else
                    {
                        dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                        if (strOrderNo == "")
                        {
                            MessageBox.Show("Sorry ! Order no can't be blank", "Enter order no", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["orderNo"];
                            dgrdOrder.Focus();
                            return false;
                        }
                        if (strPParty == "")
                        {
                            MessageBox.Show("Sorry ! Sundry Creditor can't be blank", "Enter Sundry Creditor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["pparty"];
                            dgrdOrder.Focus();
                            return false;
                        }
                        else if (strPParty != "PERSONAL")
                        {
                            string[] strSPParty = strPParty.Split(' ');
                            if(strSPParty.Length>0)
                            {
                                if (strSupplierName != "")
                                    strSupplierName += ",";
                                strSupplierName += "'" + strSPParty[0] + "'";
                            }
                        }

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
                        if (strDeliveryDate == "" && _bDeliveryDateReq && txtScheme.Text=="")
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
                        string strOrderStatus = CheckOrderNoAvailability(row);
                        if (strOrderStatus!="")
                        {
                            MessageBox.Show("Sorry ! Order no " + strOrderNo + " is already exist in "+ strOrderStatus+",\nPlease try with different order no.", "Already exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdOrder.CurrentCell = row.Cells["orderNo"];
                            dgrdOrder.Focus();
                            return false;
                        }

                        if (strGraceDays != "")
                        {
                            object objValue = DataBaseAccess.ExecuteMyScalar("Select Convert(varchar,StartDate,103)+' - '+Convert(varchar,EndDate,103) from GraceDaysMaster Where ActiveStatus=1 and (StartDate>'" + _date.ToString("MM/dd/yyyy") + "' OR EndDate<'" + _date.ToString("MM/dd/yyyy") + "') and OfferName='" + strGraceDays + "' ");
                            if (Convert.ToString(objValue) != "")
                            {
                                MessageBox.Show("Sorry ! This grace days is valid in the date period of : " + objValue, "Grace days not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtDate.Focus();
                                return false;
                            }
                        }
                    }
                }


                foreach (DataGridViewRow row in dgrdRetailOrderBook.Rows)
                {
                    string strID = Convert.ToString(row.Cells["rt_ID"].Value), strItem = Convert.ToString(row.Cells["rt_ItemName"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["rt_Qty"].Value);
                    dAmt = dba.ConvertObjectToDouble(row.Cells["rt_Amount"].Value);
                    if (strID == "" && strItem == "" && dQty == 0 && dAmt==0)
                        dgrdRetailOrderBook.Rows.Remove(row);
                    else
                    {
                        if (strItem == "")
                        {
                            MessageBox.Show("Sorry ! Item name can't be blank", "Enter Item name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdRetailOrderBook.CurrentCell = row.Cells["rt_ItemName"];
                            dgrdRetailOrderBook.Focus();
                            return false;
                        }
                        if (dQty == 0)
                        {
                            MessageBox.Show("Sorry ! Qty can't be blank", "Enter Qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdRetailOrderBook.CurrentCell = row.Cells["rt_Qty"];
                            dgrdRetailOrderBook.Focus();
                            return false;
                        }
                        if (dAmt == 0)
                        {
                            MessageBox.Show("Sorry ! Amt can't be blank", "Enter Amt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdRetailOrderBook.CurrentCell = row.Cells["rt_Amount"];
                            dgrdRetailOrderBook.Focus();
                            return false;
                        }
                    }
                }

                if (dgrdOrder.Rows.Count == 0 && dgrdRetailOrderBook.Rows.Count == 0)
                {
                    dgrdOrder.Rows.Add();
                    MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdOrder.CurrentCell = dgrdOrder.Rows[0].Cells["orderNo"];
                    dgrdOrder.Focus();
                    return false;
                }

                if (txtScheme.Text != "")
                {
                    string strQuery = "Select Convert(varchar,StartDate,103)+' - '+Convert(varchar,EndDate,103) _Date from SchemeMaster Where ActiveStatus=1 and (StartDate>'" + _date.ToString("MM/dd/yyyy") + "' OR EndDate<'" + _date.ToString("MM/dd/yyyy") + "') and SchemeName='" + txtScheme.Text + "' ";
                    if (strSupplierName != "")
                        strQuery += "Select (AreaCode+AccountNo+' '+Name) PartyName from SupplierMaster Where (AreaCode+AccountNo) in (" + strSupplierName + ") and GroupName ='SUNDRY CREDITOR' and Other not in (Select SupplierName from Scheme_SupplierDetails Where SchemeName='" + txtScheme.Text + "') ";

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

                if (dgrdRetailOrderBook.Rows.Count > 0)
                {
                    if (txtBookingNumber.Text == "")
                    {
                        MessageBox.Show("Sorry ! Booking Number can't be blank !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBookingNumber.Focus();
                        return false;
                    }
                    else
                    {
                        bool _bBStatus = ChecBookingNoAvailability(txtBookingNumber.Text);
                        if (_bBStatus)
                        {
                            MessageBox.Show("Sorry ! This Order Number already exists !!", "Order No already exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBookingNumber.Focus();
                            return false;
                        }
                    }
                
                    if (txtDeliveryDate.Text.Length != 10)
                    {
                        MessageBox.Show("Sorry ! Please enter valid delivery date !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDeliveryDate.Focus();
                        return false;
                    }
                    else if (txtDeliveryDate.Text.Length == 10)
                    {
                        DateTime _dDate = dba.ConvertDateInExactFormat(txtDeliveryDate.Text);
                        if (_dDate < _date)
                        {
                            MessageBox.Show("Sorry ! Delivery date can't be less than order date !!", "Invoice date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtDeliveryDate.Focus();
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private string CheckOrderNoAvailability(DataGridViewRow row)
        {
            string strQuery = "Select (OrderCode+' '+CAST(SerialNo as varchar)+' : '+ PurchasepartyID+' '+ P_Party) Result from OrderBooking Where SerialNo!=" + txtSerialNo.Text + " and OrderNo=" + row.Cells["orderNo"].Value + " and NumberCode='" + row.Cells["orderCode"].Value + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        private bool ChecBookingNoAvailability(string strBookingNo)
        {       
            string strQuery = "Select ISNULL(Count(*),0) Result from OrderBooking Where OrderCode='" + txtCode.Text + "' and SerialNo!=" + txtSerialNo.Text + " and OrderNo=" + strBookingNo ;
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (dba.ConvertObjectToDouble(objValue) > 0)
                return true;
            else
                return false;
        }

        private void RemoveEmptyRow()
        {
            foreach (DataGridViewRow row in dgrdOrder.Rows)
            {
                if (Convert.ToString(row.Cells["id"].Value) == "" && Convert.ToString(row.Cells["orderNo"].Value) == "" && Convert.ToString(row.Cells["pparty"].Value) == "" && Convert.ToString(row.Cells["item"].Value) == "" && Convert.ToString(row.Cells["qty"].Value) == "")
                    dgrdOrder.Rows.Remove(row);
            }
        }

        private void rdoNew_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNew.Checked)
            {
                RemoveEmptyRow();
                dgrdOrder.Rows.Add();
                btnGAdd.Enabled = false;
                dgrdPending.ReadOnly = true;
            }
        }

        private void rdoPending_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoPending.Checked)
            {
                RemoveEmptyRow();
                btnGAdd.Enabled = true;
                dgrdPending.ReadOnly = false;
            }
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
                    }
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    btnAdd.Text = "&Save";                   
                    SetSerialNo();
                    EnableAllControls();
                    txtDate.Focus();
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
        }        

        private void SaveRecord()
        {
            try
            {
                string strQuery = "", strDate = "",strDDate="",strDeliveryDate="NULL", strGrSNo = "", strPParty = "", strPersonal = "", strOrderNo = "", strStatus = "",strOrderCategory="*";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
               string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = ""; ;
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

                if (rdoDouble.Checked)
                    strOrderCategory = "**";             

                strQuery += "Declare @SerialNo bigint; Select @SerialNo = (ISNULL(MAX(SerialNo), 0) + 1)  from OrderBooking Where (OrderCode='" + _strMainOrderCode + "' OR OrderCode Like('%OD'))"
                         + " if not exists (Select SerialNo from OrderBooking Where OrderCode='" + txtCode.Text + "'  and SerialNo=@SerialNo)  begin  ";

                double dAmt = 0, dTAmt = 0;
                int _orderNo = 0;
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    strGrSNo = Convert.ToString(row.Cells["grSerial"].Value);
                    strPParty = Convert.ToString(row.Cells["pparty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                    strDDate = Convert.ToString(row.Cells["deliveryDate"].Value);
                    _orderNo = dba.ConvertObjectToInt(row.Cells["orderNo"].Value);

                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";


                    strPurchasePartyID = "";
                    if (strPParty == "PERSONAL")
                    {
                        strPersonal = strPParty;
                        strPParty = "";
                    }
                    else
                    {
                        strPersonal = "";
                        strFullName = strPParty.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            strPurchasePartyID = strFullName[0].Trim();
                            strPParty = strPParty.Replace(strPurchasePartyID + " ", "");
                        }
                    }

                    if (strGrSNo != "")
                    {
                        if (Convert.ToString(row.Cells["orderCode"].Value) != "")
                            strOrderNo = _orderNo + " " + row.Cells["orderCode"].Value;
                        else
                            strOrderNo = _orderNo + "";
                        strOrderNo = txtCode.Text + " " + strOrderNo;
                    }
                    strStatus = "PENDING";

                    strQuery += " if not exists (Select SerialNo from OrderBooking Where OrderCode='" + txtCode.Text + "'  and OrderNo=" + _orderNo + " and NumberCode='" + row.Cells["orderCode"].Value + "') begin INSERT INTO [dbo].[OrderBooking] ([OrderCode],[SerialNo],[Date],[BookingNo],[Marketer],[S_Party],[Haste],[Transport],[Marka],[Station],[Booking],[OrderNo],[NumberCode],[P_Party],[Items],[Pieces],[Quantity],[Amount],[Personal],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory]) VALUES "
                              + " ('" + txtCode.Text + "',@SerialNo,'" + strDate + "','" + txtBookingNo.Text + "','" + txtMarketer.Text + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','','" + txtStation.Text + "','" + txtStation.Text + "','" + _orderNo + "',"
                              + " '" + row.Cells["orderCode"].Value + "','" + strPParty + "','" + row.Cells["item"].Value + "','" + row.Cells["qtyType"].Value + "','" + row.Cells["qty"].Value + "'," + dAmt + ",'" + strPersonal + "','" + strStatus + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtScheme.Text + "','" + row.Cells["graceDays"].Value + "',N'" + row.Cells["remark"].Value + "',0," + dba.ConvertObjectToDouble(row.Cells["cancelQty"].Value) + ",N'" + txtRemark.Text + "'," + strDeliveryDate + ",'','','','','','ORDER','"+strOrderCategory+"') ";

                    if (strGrSNo != "")
                    {
                        strQuery += " Update GoodsReceive Set OrderNo='" + strOrderNo + "',OrderDate='" + strDate + "',UpdateStatus=1 Where  (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGrSNo + "'  "
                                 + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)+ GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+GR.Qty),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + strGrSNo + "' ";
                    }
                    else
                        strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + _orderNo + " and NumberCode = '" + row.Cells["orderCode"].Value + "' ";
                    strQuery += "  end ";
                }

                if (dgrdRetailOrderBook.Rows.Count > 0)
                {
                    strPParty = strPurchasePartyID = "";
                    strDDate = Convert.ToString(txtDeliveryDate.Text);
                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";
                    dAmt = 0;

                    strQuery += " if not exists (Select SerialNo from OrderBooking Where OrderCode='" + txtCode.Text + "'  and OrderNo=" + txtBookingNumber.Text + ")  begin  ";

                    foreach (DataGridViewRow row in dgrdRetailOrderBook.Rows)
                    {
                        //strGrSNo = Convert.ToString(row.Cells["rt_SerialNo"].Value); 
                        //if (strGrSNo != "")
                        //{
                        //    if (Convert.ToString(row.Cells["orderCode"].Value) != "")
                        //        strOrderNo = row.Cells["orderNo"].Value + " " + row.Cells["orderCode"].Value;
                        //    else
                        //        strOrderNo = row.Cells["orderNo"].Value + "";
                        //    strOrderNo = txtCode.Text + " " + strOrderNo;
                        //}

                        dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["rt_Amount"].Value);

                        strStatus = "PENDING";

                        strQuery += " INSERT INTO [dbo].[OrderBooking] ([OrderCode],[SerialNo],[Date],[BookingNo],[Marketer],[S_Party],[Haste],[Transport],[Marka],[Station],[Booking],[OrderNo],[NumberCode],[P_Party],[Items],[Pieces],[Quantity],[Amount],[Personal],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory]) VALUES "
                                 + " ('" + txtCode.Text + "',@SerialNo,'" + strDate + "','" + txtBookingNo.Text + "','" + txtMarketer.Text + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','','" + txtStation.Text + "','" + txtStation.Text + "','" + txtBookingNumber.Text + "',"
                                 + " '" + strAlphabate[row.Index] + "','" + strPParty + "','" + row.Cells["rt_ItemName"].Value + "','LOOSE','" + row.Cells["rt_Qty"].Value + "'," + dAmt + ",'" + strPersonal + "','" + strStatus + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtScheme.Text + "','','" + row.Cells["rt_Remark"].Value + "',0," + dba.ConvertObjectToDouble(row.Cells["rt_CancelQty"].Value) + ",'" + txtRemark.Text + "'," + strDeliveryDate + ",'" + row.Cells["rt_Variant1"].Value + "','" + row.Cells["rt_Variant2"].Value + "','" + row.Cells["rt_Variant3"].Value + "','" + row.Cells["rt_Variant4"].Value + "','" + row.Cells["rt_Variant5"].Value + "','RETAILORDER','" + strOrderCategory + "') ";

                        //if (strGrSNo != "")
                        //{
                        //    strQuery += " Update GoodsReceive Set OrderNo='" + strOrderNo + "',OrderDate='" + strDate + "',UpdateStatus=1 Where  (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGrSNo + "'  "
                        //             + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)+ GR.Quantity))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+GR.Quantity),UpdateStatus=1  from OrderBooking OB inner join GoodsReceive GR on (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + strGrSNo + "' and GR.OrderNo!='' ";
                        //}
                        //else

                        strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + txtBookingNumber.Text + " and NumberCode = '" + strAlphabate[row.Index] + "' ";

                    }
                    strQuery += "  end ";
                }
                if (strQuery != "")
                {
                    strQuery += "  end if exists(Select SerialNo from OrderBooking Where OrderCode = '" + txtCode.Text + "'  and SerialNo = @SerialNo)  begin  ";

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
                              + "('ORDER','" + txtCode.Text + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'CREATION','" + strTransportReason + "','') ";

                    strQuery += "  end ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {                      

                        MessageBox.Show("Thank you ! Record saved successfully !", "Saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        if (newStatus)
                        {
                            strAddedOrderDetails = GetRecentOrderDetails();
                            this.Close();
                        }
                        else
                        {
                            btnAdd.Text = "&Add";
                            BindRecordWithControl(txtSerialNo.Text);
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
                strOrderDetails = (txtCode.Text + " " + Convert.ToString(dgrdOrder.Rows[0].Cells["orderNo"].Value) + " " + Convert.ToString(dgrdOrder.Rows[0].Cells["orderCode"].Value)).Trim();
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
                    strOrderDetails = (txtCode.Text + " " + Convert.ToString(row.Cells["orderNo"].Value) + " " + Convert.ToString(row.Cells["orderCode"].Value)).Trim();
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
                txtSerialNo.ReadOnly = true;
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
        }

        private void UpdateRecord()
        {
            try
            {
                string strQuery = "", strID = "", strDate = "", strDeliveryDate = "NULL", strGrSNo = "", strPParty = "", strPersonal = "", strOrderNo = "", strStatus = "", strOrderCategory = "*";
                DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = sDate.ToString("MM/dd/yyyy hh:mm:ss");
                
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = ""; ;
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
                if (rdoDouble.Checked)
                    strOrderCategory = "**";
              
                double dAmt = 0, dTAmt = 0;
                string strNetQuery = "", strDDate = "", strAlphaNCode = "" ;
                int _orderNo = 0;

                if (_strCurrentOrderCode != "" && _strCurrentOrderCode != txtCode.Text)
                {
                    strNetQuery = " Update OrderBooking Set OrderCode='" + txtCode.Text + "' Where OrderCode='" + _strCurrentOrderCode + "' and SerialNo=" + txtSerialNo.Text + "  "
                                + " Update [dbo].[EditTrailDetails] Set BillCode='" + txtCode.Text + "' Where BillType='ORDER' and BillCode='" + _strCurrentOrderCode + "' and BillNo=" + txtSerialNo.Text + " ";
                }

                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    strID = Convert.ToString(row.Cells["id"].Value);
                    strGrSNo = Convert.ToString(row.Cells["grSerial"].Value);
                    strPParty = Convert.ToString(row.Cells["pparty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                    strDDate = Convert.ToString(row.Cells["deliveryDate"].Value);
                    _orderNo = dba.ConvertObjectToInt(row.Cells["orderNo"].Value);
                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";

                    strPurchasePartyID = "";
                    if (strPParty == "PERSONAL")
                    {
                        strPersonal = strPParty;
                        strPParty = "";
                    }
                    else
                    {
                        strPersonal = "";
                        strFullName = strPParty.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            strPurchasePartyID = strFullName[0].Trim();
                            strPParty = strPParty.Replace(strPurchasePartyID + " ", "");
                        }
                    }

                    if (Convert.ToString(row.Cells["orderCode"].Value) != "")
                        strOrderNo = _orderNo + " " + row.Cells["orderCode"].Value;
                    else
                        strOrderNo = _orderNo + "";
                    strOrderNo = txtCode.Text + " " + strOrderNo;
                 
                    strStatus = "PENDING";

                    if (strID == "")
                    {
                        strQuery += " if not exists (Select SerialNo from OrderBooking Where OrderCode='" + txtCode.Text + "'  and OrderNo=" + _orderNo + " and NumberCode='" + row.Cells["orderCode"].Value + "') begin INSERT INTO [dbo].[OrderBooking] ([OrderCode],[SerialNo],[Date],[BookingNo],[Marketer],[S_Party],[Haste],[Transport],[Marka],[Station],[Booking],[OrderNo],[NumberCode],[P_Party],[Items],[Pieces],[Quantity],[Amount],[Personal],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory]) VALUES "
                            + " ('" + txtCode.Text + "'," + txtSerialNo.Text + ",'" + strDate + "','" + txtBookingNo.Text + "','" + txtMarketer.Text + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','','" + txtStation.Text + "','" + txtStation.Text + "','" + _orderNo + "',"
                            + " '" + row.Cells["orderCode"].Value + "','" + strPParty + "','" + row.Cells["item"].Value + "','" + row.Cells["qtyType"].Value + "','" + row.Cells["qty"].Value + "'," + dAmt + ",'" + strPersonal + "','" + strStatus + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtScheme.Text + "','" + row.Cells["graceDays"].Value + "',N'" + row.Cells["remark"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["adjustedQty"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["cancelQty"].Value) + ",N'" + txtRemark.Text + "',"+ strDeliveryDate+",'','','','','','ORDER','"+strOrderCategory+"') ";

                        if (strGrSNo != "")
                        {
                            strQuery += " Update GoodsReceive Set OrderNo='" + strOrderNo + "',OrderDate='" + strDate + "',UpdateStatus=1 Where  (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGrSNo + "' "
                                     + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)+ ISNULL(GR.Qty,0)))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+ISNULL(GR.Qty,0)),UpdateStatus=1  from OrderBooking OB  Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + strGrSNo + "' ";
                        }
                        else
                            strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + _orderNo + " and NumberCode = '" + row.Cells["orderCode"].Value + "' ";

                        strQuery += " end ";
                    }
                    else
                    { 
                           strQuery += " if not exists (Select SerialNo from OrderBooking Where ID!=" + strID + " and OrderCode='" + _strCurrentOrderCode + "'  and OrderNo=" + _orderNo + " and NumberCode='" + row.Cells["orderCode"].Value + "') begin  "
                                      + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+OB.CancelQty- ISNULL(GR.Qty,0)))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-ISNULL(GR.Qty,0))  from OrderBooking OB  Cross APPLY (Select SUM(CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where OB.OrderCode='" + _strCurrentOrderCode + "' and OB.ID=" + strID + " "
                                      + " Update GoodsReceive Set OrderNo='" + strOrderNo + "',OrderDate='" + strDate + "',UpdateStatus=1 Where OrderNo!='' and OrderNo in (Select (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else (OrderCode+' '+CAST(OrderNo as varchar)) end) OrderNo from OrderBooking  Where OrderCode='" + _strCurrentOrderCode + "' and ID=" + strID + ") "                                                                         
                                      + " Update [dbo].[OrderBooking] Set [Date]='" + strDate + "',[BookingNo]='" + txtBookingNo.Text + "',[Marketer]='" + txtMarketer.Text + "',[S_Party]='" + strSaleParty + "',[Haste]='" + strSubParty + "',[Transport]='" + txtTransport.Text + "',[Marka]='', "
                                      + " [Station]='" + txtStation.Text + "',[Booking]='" + txtStation.Text + "',[OrderNo]='" + _orderNo + "',[NumberCode]='" + row.Cells["orderCode"].Value + "',[P_Party]='" + strPParty + "',[Items]='" + row.Cells["item"].Value + "',[Pieces]='" + row.Cells["qtyType"].Value + "', "
                                      + " [Quantity]='" + row.Cells["qty"].Value + "',[Amount]=" + dAmt + ",[Personal]='" + strPersonal + "',[UpdatedBy]='" + MainPage.strLoginName + "',[InsertStatus]=1,[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "',[SchemeName]='" + txtScheme.Text + "',[OfferName]='" + row.Cells["graceDays"].Value + "',[Remark]='" + row.Cells["remark"].Value + "',[CancelQty]=" + dba.ConvertObjectToDouble(row.Cells["cancelQty"].Value) + ",[MRemark]='" + txtRemark.Text + "',[DeliveryDate]="+strDeliveryDate+ ",[OrderCategory]='" + strOrderCategory + "' Where [OrderCode]='" + _strCurrentOrderCode + "' and [SerialNo]=" + txtSerialNo.Text + " and ID=" + strID
                                      + strNetQuery
                                      + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)+ISNULL(GR.Qty,0)))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+ISNULL(GR.Qty,0)),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select SUM(CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where OB.OrderCode='" + txtCode.Text + "' and OB.ID=" + strID + " "
                                      + " end";

                        if (strGrSNo == "")
                            strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + _orderNo + " and NumberCode = '" + row.Cells["orderCode"].Value + "' ";
                    }
                }


                if (dgrdRetailOrderBook.Rows.Count > 0)
                {
                    strPParty = strPurchasePartyID = "";
                    strDDate = Convert.ToString(txtDeliveryDate.Text);
                    if (strDDate.Length == 10)
                        strDeliveryDate = "'" + dba.ConvertDateInExactFormat(strDDate).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                    else
                        strDeliveryDate = "NULL";
                    dAmt = 0;

                    foreach (DataGridViewRow row in dgrdRetailOrderBook.Rows)
                    {
                        strID = Convert.ToString(row.Cells["rt_ID"].Value);
                        dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["rt_Amount"].Value);

                        if (strID == "")
                        {
                            strAlphaNCode = GetNextAlphabate(strAlphaNCode);

                            strStatus = "PENDING";

                            strQuery += " INSERT INTO [dbo].[OrderBooking] ([OrderCode],[SerialNo],[Date],[BookingNo],[Marketer],[S_Party],[Haste],[Transport],[Marka],[Station],[Booking],[OrderNo],[NumberCode],[P_Party],[Items],[Pieces],[Quantity],[Amount],[Personal],[Status],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderType],[OrderCategory]) VALUES "
                                     + " ('" + txtCode.Text + "',"+txtSerialNo.Text+",'" + strDate + "','" + txtBookingNo.Text + "','" + txtMarketer.Text + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','','" + txtStation.Text + "','" + txtStation.Text + "','" + txtBookingNumber.Text + "',"
                                     + " '" + strAlphaNCode + "','" + strPParty + "','" + row.Cells["rt_ItemName"].Value + "','LOOSE','" + row.Cells["rt_Qty"].Value + "'," + dAmt + ",'" + strPersonal + "','" + strStatus + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtScheme.Text + "','','" + row.Cells["rt_Remark"].Value + "',0," + dba.ConvertObjectToDouble(row.Cells["rt_CancelQty"].Value) + ",'" + txtRemark.Text + "'," + strDeliveryDate + ",'" + row.Cells["rt_Variant1"].Value + "','" + row.Cells["rt_Variant2"].Value + "','" + row.Cells["rt_Variant3"].Value + "','" + row.Cells["rt_Variant4"].Value + "','" + row.Cells["rt_Variant5"].Value + "','RETAILORDER','" + strOrderCategory + "') ";

                            strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + txtBookingNumber.Text + " and NumberCode = '" + strAlphabate[row.Index] + "' ";
                            //strQuery += "  end ";
                        }
                        else
                        {
                            strAlphaNCode = Convert.ToString(row.Cells["rt_NumberCode"].Value);
                            strQuery += " Update [dbo].[OrderBooking] Set [Date]='" + strDate + "',[BookingNo]='" + txtBookingNo.Text + "',[Marketer]='" + txtMarketer.Text + "',[S_Party]='" + strSaleParty + "',[Haste]='" + strSubParty + "',[Transport]='" + txtTransport.Text + "',[Marka]='', "
                                          + " [Station]='" + txtStation.Text + "',[Booking]='" + txtStation.Text + "',[OrderNo]='" + txtBookingNumber.Text + "',[Items]='" + row.Cells["rt_ItemName"].Value + "',[Variant1]='" + row.Cells["rt_Variant1"].Value + "',[Variant2]='" + row.Cells["rt_Variant2"].Value + "',[Variant3]='" + row.Cells["rt_Variant3"].Value + "',[Variant4]='" + row.Cells["rt_Variant4"].Value + "',[Variant5]='" + row.Cells["rt_Variant5"].Value + "', "
                                          + " [Quantity]='" + row.Cells["rt_Qty"].Value + "',[Amount]=" + dAmt + ",[Personal]='" + strPersonal + "',[UpdatedBy]='" + MainPage.strLoginName + "',[InsertStatus]=1,[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SchemeName]='" + txtScheme.Text + "',[Remark]='" + row.Cells["rt_Remark"].Value + "',[CancelQty]=" + dba.ConvertObjectToDouble(row.Cells["rt_CancelQty"].Value) + ",[MRemark]='" + txtRemark.Text + "',[DeliveryDate]=" + strDeliveryDate + ",[OrderCategory]='"+strOrderCategory+"' Where [OrderCode]='" + txtCode.Text + "' and [SerialNo]=" + txtSerialNo.Text + " and [NumberCode]='" + strAlphaNCode + "' and ID=" + strID
                                          + " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)))>0 Then 'PENDING' else 'CLEAR' end) from OrderBooking OB Where OrderCode = '" + txtCode.Text + "'  and OrderNo = " + txtBookingNumber.Text + " and NumberCode = '" + strAlphabate[row.Index] + "' ";

                        }
                    }
                }

                if (strQuery != "")
                {
                  
                    //string strTransportReason = "";
                    //if (bMasterUpdateStatus && _STRMasterTransportName != txtTransport.Text)
                    //{
                    //    if (strSubPartyID == "SELF")
                    //        strQuery += " Update SupplierMaster Set Transport='" + txtTransport.Text + "' Where (AreaCode+AccountNo)='" + strSalePartyID + "' ";
                    //    else
                    //        strQuery += " Update SupplierMaster Set Transport='" + txtTransport.Text + "' Where (AreaCode+AccountNo)='" + strSubPartyID + "' ";

                    //    strTransportReason = "TRP CHANGED " + _STRMasterTransportName + " TO " + txtTransport.Text;
                    //}

                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason],[ComputerName]) VALUES "
                             + "('ORDER','" + txtCode.Text + "'," + txtSerialNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION','','') ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {                     
                        if(strNetQuery!="")
                            DataBaseAccess.CreateDeleteQuery(strNetQuery);
                        MessageBox.Show("Thank you ! Record updated successfully !", "Updated successfully", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        if (updateStatus)
                        {
                            strAddedOrderDetails = GetRecentOrderDetails_Update();
                            this.Close();
                        }
                        else
                        {
                            btnEdit.Text = "&Edit";
                            BindRecordWithControl(txtSerialNo.Text);
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

        private string GetNextAlphabate(string str)
        {
            try
            {
                if (str != "")
                {
                    int _index = 1;
                    foreach (string strValue in strAlphabate)
                    {
                        if (strValue == str)
                            break;
                        _index++;
                    }
                    return strAlphabate[_index];
                }
                else
                    return strAlphabate[0];
            }
            catch { return strAlphabate[27]; }
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
                        dgrdOrder.Rows[0].Cells["pparty"].Value = _strPPurchaseParty;
                        dgrdOrder.Rows[0].Cells["qty"].Value = _strQty;
                        dgrdOrder.Rows[0].Cells["amt"].Value = _strAmount;
                        dgrdOrder.Rows[0].Cells["qtyType"].Value = _strPackingType;
                        dgrdOrder.Rows[0].Cells["item"].Value = _strItemName;
                        dgrdOrder.Rows[0].Cells["deliveryDate"].Value = txtDate.Text;
                        dgrdOrder.Rows[0].Cells["grSerial"].Value = _strGRSNO;
                    }

                    txtMarketer.Focus();
                }
                else if (updateStatus)
                {
                    txtSerialNo.ReadOnly = true;
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
                    btnAdd.Enabled =btnDownloadOrder.Enabled= false;
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
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtSerialNo.Text != "")
                BindRecordWithControl(txtSerialNo.Text);
        }

        private void dgrdPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strAllGRSNo = Convert.ToString(dgrdPending.CurrentRow.Cells["grsno"].Value);
                    string[] strGRSNo = strAllGRSNo.Split(' ');
                    if (strGRSNo.Length > 1)
                    {
                        if (strGRSNo[0] != "" && strGRSNo[1] != "")
                        {
                            GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strGRSNo[0], strGRSNo[1]);
                            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objGoodsReciept.Show();
                        }
                    }
                }
            }
            catch { }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtCode.Text != "" && txtSerialNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("ORDER", txtCode.Text, txtSerialNo.Text);

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
                            strQuery += " Update GoodsReceive Set OrderNo='' Where OrderNo!='' and OrderNo in (Select (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else (OrderCode+' '+CAST(OrderNo as varchar)) end) OrderNo from OrderBooking  Where OrderCode='" + txtCode.Text + "' and SerialNo=" + txtSerialNo.Text + ") "
                                     + " Delete from OrderBooking Where OrderCode='" + txtCode.Text + "' and SerialNo=" + txtSerialNo.Text
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('ORDER','" + txtCode.Text + "'," + txtSerialNo.Text + ",'" + txtReason.Text + ", Marketer : " + txtMarketer.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

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
                    if (Convert.ToString(row.Cells["grSerial"].Value) != "")
                    {
                        _bStatus = DataBaseAccess.CheckPartyAdjustedAmount(Convert.ToString(row.Cells["grSerial"].Value));
                        if (!_bStatus)
                        {
                            MessageBox.Show("Sorry ! Purchase linked with this order has been adjusted ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                                return false;
                        }
                    }
                    else
                    {
                        if (dba.ConvertObjectToDouble(row.Cells["adjustedQty"].Value) > 0 && dgrdOrder.Rows.Count!=1)
                        {
                            MessageBox.Show("Sorry ! Order adjusted with purchase book ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("ADMIN"))
                                return false;
                        }
                    }
                }
            }
            catch { }
            return true;
        }



        //private void txtGraceDays_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
        //        {
        //            if (e.KeyCode == Keys.Enter)
        //            {
        //                if (rdoNew.Checked)
        //                    dgrdOrder.Focus();
        //                else
        //                    dgrdPending.Focus();
        //            }
        //            else
        //            {
        //                if (MainPage.mymainObject.bFullEditControl)
        //                {
        //                    char objChar = Convert.ToChar(e.KeyCode);
        //                    int value = e.KeyValue;
        //                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //                    {
        //                        SearchData objSearch = new SearchData("OFFERNAME", "SEARCH GRACE DAYS", e.KeyCode);
        //                        objSearch.ShowDialog();
        //                        txtGraceDays.Text = objSearch.strSelectedData;
        //                    }
        //                }
        //            }
        //            e.Handled = true;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

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

                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    if (!CheckAdjustedQty(row))
                        return false;
                }
            }          
            return true;
        }

        private bool CheckAdjustedQty(DataGridViewRow row)
        {
            try
            {

                if (dba.ConvertObjectToDouble(row.Cells["adjustedQty"].Value) > 0)
                {
                    string strOrder = txtCode.Text + " " + row.Cells["orderNo"].Value;
                    if (Convert.ToString(row.Cells["orderCode"].Value) != "")
                        strOrder += " " + row.Cells["orderCode"].Value;
                    string strPSerialNo = dba.GetPurchaseSerialCodeAndSerialNoFromOrder(strOrder);
                    MessageBox.Show("Sorry ! Order No : " + row.Cells["orderNo"].Value + " " + row.Cells["orderCode"].Value + ", is already linked with Purchase No : " + strPSerialNo, "Order Linked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                    return true;
            }
            catch { }
            return false;
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                // {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                string strCode = txtCode.Text;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ORDERCODE", "SEARCH ORDER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtCode.Text = objSearch.strSelectedData;
                    if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtCode.Text == "DLOO")
                    {
                        MessageBox.Show("Sorry ! This code not allowed in order entry !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCode.Text = strCode;
                    }
                    else
                    {
                        if (btnAdd.Text == "&Save")
                        {
                            if (txtCode.Text.Contains("OD"))
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

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
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
                if(e.ColumnIndex==2 || e.ColumnIndex==3)
                {
                   string strOrderStatus= CheckOrderNoAvailability(dgrdOrder.CurrentRow);
                    if (strOrderStatus != "")
                    {
                        MessageBox.Show("Sorry ! This Order no  is already exist in " + strOrderStatus + ",\nPlease try with different order no.", "Already exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);                     
                        dgrdOrder.Focus();
                        dgrdOrder.CurrentCell.Value = "";
                    }
                }
                else if (e.ColumnIndex == 8)
                {
                    CalculateNetAmt();
                }
                else if (e.ColumnIndex == 9)
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
            double dTAmt = 0;
            try
            {
                foreach (DataGridViewRow row in dgrdOrder.Rows)
                {
                    dTAmt += dba.ConvertObjectToDouble(row.Cells["amt"].Value);
                }
            }
            catch { }
            lblNetAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void btnDownloadOrder_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownloadOrder.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int count = dba.DownloadOrderDetails();
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Order downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        btnEdit.Text = "&Edit";
                        BindLastRecord();
                    }
                    else
                        MessageBox.Show("Sorry ! No order found right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
            btnDownloadOrder.Enabled = true;
        }

        private void txtDelDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void dgrdRetailOrderBook_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 11)
                        e.Cancel = true;
                    else if ( e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
                    {
                        string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";
                        bool _bChkStatus = true;
                        //if (!txtCode.Text.Contains("PTN"))
                        //    _bChkStatus = true;

                        SearchCategory objSearch = new SearchCategory("", "ORDERDESIGNNAME", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, true, _bChkStatus);
                        objSearch.ShowDialog();
                        GetAllDesignSizeColor(objSearch, dgrdRetailOrderBook.CurrentRow.Index);
                     
                        e.Cancel = true;
                    }
                    else if (txtCode.Text.Contains("PTN") && (e.ColumnIndex == 12 || e.ColumnIndex == 13) && !MainPage.strUserRole.Contains("SUPERADMIN"))
                    {
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


        private void GetAllDesignSizeColor(SearchCategory objCategory, int rowIndex)
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
                                if (strItem != "ADD NEW DESIGNNAME NAME" && strItem != "ADD NEW ORDERDESIGNNAME NAME")
                                {
                                    string[] strAllItem = strItem.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdRetailOrderBook.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_ItemName"].Value = strAllItem[0];
                                        if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant2"].Value = strAllItem[1];
                                        if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant1"].Value = strAllItem[2];
                                        if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant3"].Value = strAllItem[3];
                                        if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant4"].Value = strAllItem[4];
                                        if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant5"].Value = strAllItem[5];

                                        if (Convert.ToString(dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"].Value) == "")
                                            dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"].Value = 1;

                                        dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Rate"].Value = dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Amount"].Value = strAllItem[7];

                                        //GetSaleRate(dgrdRetailOrderBook.Rows[rowIndex]);

                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowIndex > 0)
                                rowIndex--;

                            dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"];
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {

                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_ItemName"].Value = strAllItem[0];
                                if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant2"].Value = strAllItem[1];
                                if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant1"].Value = strAllItem[2];
                                if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant3"].Value = strAllItem[3];
                                if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant4"].Value = strAllItem[4];
                                if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Variant5"].Value = strAllItem[5];

                                if (Convert.ToString(dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"].Value) == "")
                                    dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"].Value = 1;
                                
                                dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Rate"].Value = dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Amount"].Value= strAllItem[7];
                                  
                                dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_Qty"];

                                //GetSaleRate(dgrdRetailOrderBook.Rows[rowIndex]);
                            }
                        }
                        ArrangeSerialNo();                     

                        //if (Convert.ToString(dgrdRetailOrderBook.Rows[rowIndex].Cells["rt_ItemName"].Value) != "")
                        //{
                        //    dgrdRetailOrderBook.Rows.Add(1);
                        //    dgrdRetailOrderBook.Rows[dgrdRetailOrderBook.RowCount - 1].Cells["rt_SNo"].Value = dgrdRetailOrderBook.Rows.Count;
                        //    dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[dgrdRetailOrderBook.RowCount - 1].Cells["rt_ItemName"];
                        //    dgrdRetailOrderBook.Focus();
                        //}
                    }
                }
            }
            catch
            {
            }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdRetailOrderBook.Rows)
            {
                row.Cells["rt_SNo"].Value = serialNo;
                serialNo++;
            }
        }

        private void GetSaleRate(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, dRate = 0, _dQty = 0;
                if (row != null)
                {
                    object objDisPer = 0;
                    if (Convert.ToString(row.Cells["rt_ItemName"].Value) != "")
                    {
                        DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                        object objValue = dba.GetSaleRate(row.Cells["rt_ItemName"].Value, row.Cells["rt_Variant1"].Value, row.Cells["rt_Variant2"].Value, row.Cells["rt_Variant3"].Value, row.Cells["rt_Variant4"].Value, row.Cells["rt_Variant5"].Value, ref _dQty, ref objDisPer, _date);
                        dDisPer = dba.ConvertObjectToDouble(objDisPer) * -1;
                        dMRP = dba.ConvertObjectToDouble(objValue);
                        //row.Cells["mrp"].Value = dMRP;
                    }
                }

                if (_dQty <= 0)
                    row.DefaultCellStyle.BackColor = Color.Tomato;

                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 + dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;

                row.Cells["rt_Rate"].Value = dRate;
                double dAmt = 0, dQty = dba.ConvertObjectToDouble(row.Cells["rt_Qty"].Value);
                dAmt = dQty * dRate;
                row.Cells["rt_Amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                
            }
            catch
            {
            }
        }


        private void dgrdRetailOrderBook_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
                if (cIndex == 8 || cIndex == 9 || cIndex == 10 || cIndex == 12 || cIndex == 13)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                    txtBox.KeyPress += new KeyPressEventHandler(txtRetailBox_KeyPress);
                }
            }
            catch { }            
        }

        private void txtRetailBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int cIndex = dgrdOrder.CurrentCell.ColumnIndex;
            if (cIndex == 8 || cIndex == 10 || cIndex == 12 || cIndex == 13)
                dba.KeyHandlerPoint(sender, e, 0);
            else if (cIndex == 9)
                dba.ValidateSpace(sender, e);
        }

        private void dgrdRetailOrderBook_KeyDown(object sender, KeyEventArgs e)
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
                        Index = dgrdRetailOrderBook.CurrentCell.RowIndex;
                        IndexColmn = dgrdRetailOrderBook.CurrentCell.ColumnIndex;
                        if (Index < dgrdRetailOrderBook.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdRetailOrderBook.ColumnCount - 9)
                        {
                            IndexColmn += 1;
                            if (!dgrdRetailOrderBook.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                if (!dgrdRetailOrderBook.Columns[IndexColmn].Visible && IndexColmn < dgrdRetailOrderBook.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdRetailOrderBook.Columns[IndexColmn].Visible && IndexColmn < dgrdRetailOrderBook.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdRetailOrderBook.Columns[IndexColmn].Visible && IndexColmn < dgrdRetailOrderBook.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdRetailOrderBook.Columns[IndexColmn].Visible && IndexColmn < dgrdRetailOrderBook.ColumnCount - 1)
                                    IndexColmn++;
                                dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdRetailOrderBook.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdRetailOrderBook.Rows[CurrentRow].Cells["rt_ItemName"].Value);
                            double dQty = dba.ConvertObjectToDouble(dgrdRetailOrderBook.Rows[CurrentRow].Cells["rt_Qty"].Value);

                            if (strItemName != "" && dQty > 0)
                            {
                                dgrdRetailOrderBook.Rows.Add(1);
                                dgrdRetailOrderBook.Rows[dgrdRetailOrderBook.RowCount - 1].Cells["rt_SNo"].Value = dgrdRetailOrderBook.Rows.Count;
                                dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[dgrdRetailOrderBook.RowCount - 1].Cells[2];
                                dgrdRetailOrderBook.Focus();
                            }
                            else
                                txtBookingNumber.Focus();                          
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdRetailOrderBook.Rows.RemoveAt(dgrdRetailOrderBook.CurrentRow.Index);
                        if (dgrdRetailOrderBook.Rows.Count == 0)
                        {
                            dgrdRetailOrderBook.Rows.Add(1);
                            dgrdRetailOrderBook.Rows[0].Cells["rt_SNo"].Value = 1;
                            dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[0].Cells[0];
                            dgrdRetailOrderBook.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }                       
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdRetailOrderBook.CurrentRow.Cells["rt_ID"].Value); 
                        if (strID == "")
                        {
                            dgrdRetailOrderBook.Rows.RemoveAt(dgrdRetailOrderBook.CurrentRow.Index);
                            if (dgrdRetailOrderBook.Rows.Count == 0)
                            {
                                dgrdRetailOrderBook.Rows.Add(1);
                                dgrdRetailOrderBook.Rows[0].Cells["rt_SNo"].Value = 1;
                                dgrdRetailOrderBook.CurrentCell = dgrdRetailOrderBook.Rows[0].Cells[2];
                                dgrdRetailOrderBook.Enabled = true;
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
                                DeleteOrderBookingOneRow(strID);
                            }
                        }
                    }
                    
                }
            }
            catch { }
        }

        private void DeleteOrderBookingOneRow(string strID)
        {
            try
            {
                if (dgrdRetailOrderBook.Rows.Count > 1)
                {
                    string strQuery = " Delete from OrderBooking Where  [OrderCode]='" + txtCode.Text + "' and [SerialNo]=" + txtSerialNo.Text + " and ID=" + strID + " ";
                  
                    int result = dba.ExecuteMyQuery(strQuery);
                    if (result >0)
                    {
                        DataBaseAccess.CreateDeleteQuery(strQuery);
                        dgrdRetailOrderBook.Rows.RemoveAt(dgrdRetailOrderBook.CurrentRow.Index);
                        ArrangeSerialNo();
                    }                  
                }
            }
            catch
            {
            }
        }

        private void txtDeliveryDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtDeliveryDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void CalculateRetailsAmt()
        {
            double dAmt = 0;
            try
            {              
                foreach(DataGridViewRow row in dgrdRetailOrderBook.Rows)
                {
                    dAmt += dba.ConvertObjectToDouble(row.Cells["rt_Amount"].Value);
                }               
            }
            catch { }
            lblNetAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
        }

        private void dgrdRetailOrderBook_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex==8)
                {
                    double dQty = dba.ConvertObjectToDouble(dgrdRetailOrderBook.CurrentRow.Cells["rt_Qty"].Value), dRate = dba.ConvertObjectToDouble(dgrdRetailOrderBook.CurrentRow.Cells["rt_Rate"].Value);
                    dgrdRetailOrderBook.CurrentRow.Cells["rt_Amount"].Value = dQty * dRate;
                    CalculateRetailsAmt();
                }
            }
            catch { }
        }       

        private void dgrdOrder_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    string strPartyName = Convert.ToString(dgrdOrder.CurrentCell.Value);
                    if (strPartyName != "")
                    {
                        SupplierMaster objSupplier = new SupplierMaster(strPartyName);
                        objSupplier.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                        objSupplier.ShowInTaskbar = true;
                        objSupplier.Show();
                    }
                }
            }
            catch { }
        }

        private void dgrdOrder_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 9)
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
      
    }
}
