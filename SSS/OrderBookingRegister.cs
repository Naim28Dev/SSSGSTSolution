using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class OrderBookingRegister : Form
    {
        DataBaseAccess dba;      
        MainPage mainObj;
        ReportSetting objSetting;
        DataTable _dtOrder = null;
        public OrderBookingRegister()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                ArrangeColumn();
                mainObj = MainPage.mymainObject as MainPage;
                txtFromDate.Text = txtFromDelDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = txtToDelDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
                SetCategory();
                if (MainPage.strUserRole.Contains("ADMIN"))
                    btnExpand.Visible = true;
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
                    dgrdorder.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdorder.Columns["variant1"].Visible = true;
                }
                else
                    dgrdorder.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdorder.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdorder.Columns["variant2"].Visible = true;
                }
                else
                    dgrdorder.Columns["variant2"].Visible = false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdorder.Columns["variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdorder.Columns["variant3"].Visible = true;
                }
                else
                    dgrdorder.Columns["variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdorder.Columns["variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdorder.Columns["variant4"].Visible = true;
                }
                else
                    dgrdorder.Columns["variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdorder.Columns["variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdorder.Columns["variant5"].Visible = true;
                }
                else
                    dgrdorder.Columns["variant5"].Visible = false;
            }
            catch
            {
            }
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                double dQty = 0, dCQty = 0, dTQty = 0, dAmt = 0, dTAmt = 0, dPendingQty = 0, dCancelQty = 0, dPendingAmt = 0, dRate = 0, dPQty = 0, dPAmt = 0 ;
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {                      
                        dgrdorder.Rows.Clear();
                        dgrdorder.Rows.Add(table.Rows.Count);
                        string strSchemeName = "", strOfferName = "", strNewID="", strID="";

                        for (int i = 0; i < table.Rows.Count; ++i)
                        {                        
                            DataRow dr = table.Rows[i];
                            DataGridViewRow row = dgrdorder.Rows[i];
                            strNewID = Convert.ToString(dr["ID"]);

                            if (strID != strNewID)
                            {
                                dTQty += dQty = dba.ConvertObjectToDouble(dr["Quantity"]);
                                dPQty = dba.ConvertObjectToDouble(dr["PendingQty"]);
                                dCancelQty += dCQty = dba.ConvertObjectToDouble(dr["CancelQty"]);
                                dTAmt += dAmt = dba.ConvertObjectToDouble(dr["Amount"]);
                                dRate = dAmt / dQty;
                                dPAmt = 0;

                                if (dPQty > 0)
                                {
                                    dPendingQty += dPQty;
                                    dPendingAmt += dPAmt = (dPQty * dRate);
                                }
                                strID = strNewID;
                            }
                            else //if(MainPage.strUserRole.Contains("ADMIN"))
                                row.Visible = false;

                            strSchemeName = Convert.ToString(dr["SchemeName"]);
                            strOfferName = Convert.ToString(dr["OfferName"]);

                            row.Cells["serialNo"].Value = dr["OrderCode"]+" "+ dr["SerialNo"];
                            row.Cells["id"].Value = strNewID;
                            row.Cells["grsno"].Value = dr["GRSNo"];
                            row.Cells["date"].Value =dr["Date"];
                            row.Cells["booking"].Value = dr["BookingNo"];
                            row.Cells["orderNo"].Value = dr["FullOrderNo"];
                            row.Cells["marketer"].Value = dr["Marketer"];                          
                            row.Cells["sparty"].Value = dr["SParty"];
                            row.Cells["parcelType"].Value = dr["Pieces"];
                            row.Cells["item"].Value = dr["Items"];
                            row.Cells["transport"].Value = dr["Transport"];
                            //row.Cells["pvtMarka"].Value = dr["Marka"];
                            //row.Cells["station"].Value = dr["Station"];
                            row.Cells["bookingStation"].Value = dr["Booking"];
                            row.Cells["quantity"].Value = dr["Quantity"];
                            row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy) ;
                            row.Cells["createdBy"].Value = dr["CreatedBy"];
                            row.Cells["updatedBy"].Value = dr["UpdatedBy"];

                            row.Cells["schemeName"].Value = strSchemeName;
                            row.Cells["offerName"].Value = strOfferName;
                            row.Cells["remark"].Value = dr["Remark"];
                            row.Cells["adjustedQty"].Value = dr["AdjustedQty"];
                            row.Cells["cancelQty"].Value = dr["CancelQty"];
                            row.Cells["pendingQty"].Value = dr["PendingQty"];
                            row.Cells["pendingAmt"].Value = dPAmt;
                            row.Cells["mRemark"].Value = dr["MRemark"];
                            row.Cells["status"].Value = dr["Status"];
                            row.Cells["variant1"].Value = dr["variant1"];
                            row.Cells["variant2"].Value = dr["variant2"];
                            row.Cells["variant3"].Value = dr["variant3"];
                            row.Cells["variant4"].Value = dr["variant4"];
                            row.Cells["variant5"].Value = dr["variant5"];
                            row.Cells["orderCategory"].Value = dr["OrderCategory"];
                            row.Cells["ordertype"].Value = dr["ordertype"];

                            row.Cells["saleBillNo"].Value = Convert.ToString(dr["SaleBillNo"]);
                            row.Cells["saleQty"].Value = dba.ConvertObjectToDouble(dr["Qty"]);
                            row.Cells["saleBillDate"].Value = Convert.ToString(dr["SDate"]);
                            row.Cells["diffDate"].Value = dr["DateDifference"];
                            row.Cells["OrderTime"].Value = Convert.ToString(dr["OrderTime"]);

                            if (Convert.ToString(dr["DeliveryDate"])=="")
                            row.Cells["deliveryDate"].Value = Convert.ToString(dr["DeliveryDate"]);
                            else
                                row.Cells["deliveryDate"].Value = dr["DeliveryDate"];

                            if (Convert.ToString(dr["HParty"]) != "")
                                row.Cells["subparty"].Value = dr["HParty"];
                            else
                                row.Cells["subparty"].Value = "SELF";
                            if (Convert.ToString(dr["PParty"]) != "")
                                row.Cells["pparty"].Value = dr["PParty"];
                            else
                                row.Cells["pparty"].Value = "PERSONAL";

                            if (Convert.ToString(dr["Status"]) == "CLEAR")
                                row.DefaultCellStyle.BackColor = Color.LightGreen;

                            if (strSchemeName != "" && strOfferName != "")
                                row.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                            else if (strSchemeName != "")
                                row.DefaultCellStyle.BackColor = Color.LightSalmon;
                            else if (strOfferName != "")
                                row.DefaultCellStyle.BackColor = Color.Thistle;

                            if(Convert.ToString(dr["Remark"]).Contains("HOLD"))
                                row.DefaultCellStyle.BackColor = Color.Gold;

                            if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                                row.DefaultCellStyle.BackColor = Color.Plum;

                            if (dQty== dCQty)
                            {
                                row.Cells["status"].Value = "CANCEL";
                                row.DefaultCellStyle.BackColor = Color.Tomato;
                            }
                        }
                    }
                    else
                    {
                        dgrdorder.Rows.Clear();
                    }
                }

                lblPcs.Text = dTQty.ToString("N0", MainPage.indianCurancy);
                lblAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblPendingQty.Text= dPendingQty.ToString("N0", MainPage.indianCurancy);
                lblCancelQty.Text = dCancelQty.ToString("N0", MainPage.indianCurancy);
                lblPendingAmt.Text = dPendingAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in Show Order Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = !panelSearch.Visible;
        }

        private void GetAdvanceSearchedRecord()
        {
            try
            {
                if (rdoPending.Checked || txtSalesParty.Text != "" || txtPurchaseParty.Text != "" || MainPage.strUserRole.Contains("ADMIN") || MainPage.strLoginName.Contains("MANMOHAN") || MainPage.strLoginName.Contains("TITOO") || txtOrderNo.Text.Length > 2 || txtOrderCode.Text.Contains("OD"))
                {
                    string strQuery = " ", strSubQuery = CreateQuery(); // Select _Order.*,(CASE When NumberCode!='' then (CAST(OrderNo as varchar)+' '+NumberCode) else CAST(OrderNo as varchar) end) FullOrderNo,(CONVERT(varchar,Date,103)) BDate,(SalePartyID+' '+SM1.Name) SParty, (CASE WHEN SubPartyID!='SELF' then (SubPartyID+' '+SM3.Name) else SubPartyID end) HParty,(CASE WHEN PurchasePartyID!='' then (PurchasePartyID+' '+SM2.Name) else 'PERSONAL' end) PParty,CAST((CAST(Quantity as Money)-AdjustedQty-ISNULL(CancelQty,0)) as Numeric(18,0)) PendingQty,SR.SaleBillNo,SR.GRSNo,SR.Qty,CONVERT(varchar,SR.BillDate,103) SDate,ISNULL(DATEDIFF(dd,_Order.Date,BillDate),0) DateDifference,(Select LTRIM(RIGHT(CONVERT(VARCHAR(20), MAX(ETD.Date), 100), 7)) as EDate from EditTrailDetails ETD WHere ETD.BillType='ORDER' and ETD.BillCode=_Order.OrderCode and ETD.BillNo=_Order.SerialNo and ETD.EditStatus='CREATION')as OrderTime,OrderCategory from OrderBooking _Order OUTER APPLY (Select GR.Quantity Qty,SR.SaleBillNo,SR.BillDate,GRSNO from GoodsReceive GR OUTER APPLY (Select SE.GRSNO,(SE.BillCode+' '+CAST(SE.BillNo as varchar)) SaleBillNo,BillDate from SalesEntry SE Where SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))) SR Where GR.OrderNo=LTRIM(RTRIM(_Order.OrderCode+' '+CAST(_Order.OrderNo as varchar)+' '+NumberCode)) UNION ALL Select SBS.Qty,SR.SaleBillNo,SR.BillDate,GRSNO from SalesBookSecondary SBS OUTER APPLY (Select '' as GRSNO,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) SaleBillNo,SB.Date as BillDate from SalesBook SB Where SBS.BillCode=SB.BillCode and SBS.BillNo=SB.BillNo) SR Where SBS.SONumber=LTRIM(RTRIM(_Order.OrderCode+' '+CAST(_Order.OrderNo as varchar)+' '+NumberCode))) SR OUTER APPLY (SELECT Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=_Order.SalePartyID) SM1 OUTER APPLY (SELECT Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=_Order.PurchasePartyID) SM2 OUTER APPLY (SELECT Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=_Order.SubPartyID) SM3 where SerialNo!=0 ", strSubQuery = "";

                    strQuery += " Select * from (";
                    if (rdoOrderType.Checked || rdoSoftwareOrder.Checked)
                        strQuery += " Select 'SOFTWAREAPP' as OrderType,_ORDER.ID,[BookingNo],[Marketer],[OrderNo],[P_Party],[S_Party],[Station],[Items],[Pieces],[Quantity],[Amount],[Transport],[Booking],[Marka],[Haste],[Date],[Personal],[Status],[OrderCode],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[NumberCode],[SerialNo],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderCategory],(CASE When NumberCode!='' then (CAST(OrderNo as varchar)+' '+NumberCode) else CAST(OrderNo as varchar) end) FullOrderNo,(CONVERT(varchar,Date,103)) BDate,(SalePartyID+' '+SM1.Name) SParty, (CASE WHEN SubPartyID!='SELF' then (SubPartyID+' '+SM3.Name) else SubPartyID end) HParty,(CASE WHEN PurchasePartyID!='' then (PurchasePartyID+' '+SM2.Name) else 'PERSONAL' end) PParty,CAST((CAST(Quantity as Money)-AdjustedQty-ISNULL(CancelQty,0)) as Numeric(18,0)) PendingQty,SR.SaleBillNo,SR.GRSNo,SR.Qty,CONVERT(varchar,SR.BillDate,103) SDate,ISNULL(DATEDIFF(dd,_Order.Date,BillDate),0) DateDifference,0 as OrderTime from OrderBooking _Order left join(Select GR.OrderNo as _Orderno, GR.Quantity Qty,(SE.BillCode+' '+CAST(SE.BillNo as varchar)) SaleBillNo,BillDate,GRSNo from GoodsReceive GR left join SalesEntry SE on SE.GRSNo=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) UNION ALL Select SBS.SONumber as _Orderno, SBS.Qty,(SB.BillCode+' '+CAST(SB.BillNo as varchar)) SaleBillNo,SB.Date as BillDate,'' as GRSNO from SalesBookSecondary SBS inner join SalesBook SB on SBS.BillCode=SB.BillCode and SBS.BillNo=SB.BillNo) SR on SR._Orderno=LTRIM(RTRIM(_Order.OrderCode+' '+CAST(_Order.OrderNo as varchar)+' '+NumberCode)) left join(Select SM1.AreaCode,SM1.AccountNo, Name from SupplierMaster SM1 Where SM1.GroupName='SUNDRY DEBTORS')SM1 on (SM1.AreaCode+SM1.AccountNo)=_Order.SalePartyID left join (Select SM2.AreaCode,SM2.AccountNo, Name from SupplierMaster SM2 Where SM2.GroupName='SUNDRY CREDITOR')SM2 on (SM2.AreaCode+SM2.AccountNo)=_Order.PurchasePartyID left join (Select SM3.AreaCode,SM3.AccountNo, Name from SupplierMaster SM3 Where SM3.GroupName='SUB PARTY')SM3 on  (SM3.AreaCode+SM3.AccountNo)=_Order.SubPartyID where SerialNo!=0 " + strSubQuery;
                    if (rdoOrderType.Checked)
                        strQuery += " UNION ALL ";
                    if (rdoOrderType.Checked || rdoAPPOrder.Checked)
                        strQuery += " Select 'APPORDER' as OrderType,_ORDER.ID,[BookingNo],[Marketer],[OrderNo],[P_Party],[S_Party],[Station],[Items],[Pieces],[Quantity],[Amount],[Transport],[Booking],[Marka],[Haste],[Date],[Personal],[Status],[OrderCode],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[NumberCode],[SerialNo],[SalePartyID],[SubPartyID],[PurchasePartyID],[SchemeName],[OfferName],[Remark],[AdjustedQty],[CancelQty],[MRemark],[DeliveryDate],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OrderCategory],(CASE When NumberCode!='' then(CAST(OrderNo as varchar)+' '+NumberCode) else CAST(OrderNo as varchar) end) FullOrderNo,(CONVERT(varchar, Date,103)) BDate,(SalePartyID+' '+SM1.Name) SParty, (CASE WHEN SubPartyID!='SELF' then(SubPartyID+' '+SM3.Name) else SubPartyID end) HParty,(CASE WHEN PurchasePartyID!='' then(PurchasePartyID+' '+SM2.Name) else 'PERSONAL' end) PParty,CAST((CAST(Quantity as Money)-AdjustedQty-ISNULL(CancelQty,0)) as Numeric(18,0)) PendingQty,'' as SaleBillNo,'' as GRSNo,0 as Qty,CONVERT(varchar,_Order.Date,103) as SDate,0 DateDifference,0 as OrderTime from AppOrderBooking _Order left join(Select SM1.AreaCode,SM1.AccountNo, Name from SupplierMaster SM1 Where SM1.GroupName='SUNDRY DEBTORS')SM1 on (SM1.AreaCode+SM1.AccountNo)=_Order.SalePartyID left join (Select SM2.AreaCode,SM2.AccountNo, Name from SupplierMaster SM2 Where SM2.GroupName='SUNDRY CREDITOR')SM2 on (SM2.AreaCode+SM2.AccountNo)=_Order.PurchasePartyID left join (Select SM3.AreaCode,SM3.AccountNo, Name from SupplierMaster SM3 Where SM3.GroupName='SUB PARTY')SM3 on  (SM3.AreaCode+SM3.AccountNo)=_Order.SubPartyID where _Order.InsertStatus=1 " + strSubQuery;

                    strQuery += " )_Order Order By Date,SerialNo";

                    //strSubQuery = CreateQuery();
                    //if (strSubQuery != "")
                    //    strQuery += strSubQuery;

                    //strQuery += " Order By Date,SerialNo,ID";

                    DataTable dt = dba.GetDataTable(strQuery);
                    BindDataWithGrid(dt);
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors or Sundry Creditor !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Advance Searched Record in Show Order Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
                if (chkDelDate.Checked && txtFromDelDate.Text.Length == 10 && txtToDelDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDelDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDelDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (DeliveryDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and DeliveryDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
                if (chkSerial.Checked)
                    strQuery += " and (SerialNo >= " + txtFromSerialNo.Text + " and SerialNo <=" + txtToSerialNo.Text + ") ";

                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and  PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                }

                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and SalePartyID='" + strFullName[0].Trim() + "' ";
                }

                if (txtSubParty.Text != "")
                {
                    if (txtSubParty.Text == "SELF")
                    {
                        strQuery += " and SubPartyID='" + txtSubParty.Text + "' ";
                    }
                    else
                    {
                        strFullName = txtSubParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery += " and SubPartyID='" + strFullName[0].Trim() + "' ";
                    }
                }

                if (txtMarketer.Text != "" && MainPage.strUserRole.Contains("ADMIN"))
                    strQuery += " and Marketer='" + txtMarketer.Text + "' ";

                if (txtItemName.Text != "")
                    strQuery += " and Items Like('%" + txtItemName.Text + "%') ";
                //if (txtRemark.Text != "")
                //    strQuery += " and (Remark Like('%" + txtRemark.Text + "%') OR MRemark Like('%" + txtRemark.Text + "%')) ";

                if (txtOrderNo.Text != "")
                    strQuery += " and  OrderNo = '" + txtOrderNo.Text + "' ";

                if (txtOrderCode.Text != "")
                    strQuery += " and OrderCode='" + txtOrderCode.Text + "' ";

                if (rdoSingle.Text != "")
                    strQuery += " and OrderCategory='*' ";
                else if (rdoDouble.Text != "")
                    strQuery += " and OrderCategory='**' ";

                if (rdoPending.Checked)
                    strQuery += " and Status='PENDING' ";
                else if (rdoClear.Checked)
                    strQuery += " and Status='CLEAR' ";
                else if (rdoHold.Checked)
                    strQuery += " and Status='HOLD' ";
                else if (rdoCancel.Checked)
                    strQuery += " and CancelQty>0 ";

                if (rdoPeti.Checked)
                    strQuery += " and Pieces='PETI' ";
                else if (rdoLosse.Checked)
                    strQuery += " and Pieces='LOOSE' ";
                else if (rdoParcel.Checked)
                    strQuery += " and Pieces='PARCEL' ";

                if (txtScheme.Text != "")
                    strQuery += " and  SchemeName = '" + txtScheme.Text + "' ";

                if (txtGraceDays.Text != "")
                    strQuery += " and OfferName='" + txtGraceDays.Text + "' ";

                //if (chkSchemeSParty.Checked)
                //    strQuery += " and S_Party in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='Sundry Debtors' and Other!='') ";

                //if (chkSchemePParty.Checked)
                //    strQuery += " and P_Party in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='Sundry Creditor' and Other!='') ";

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Show Order Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = false;

                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkDelDate.Checked && (txtFromDelDate.Text.Length != 10 || txtToDelDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Delivery Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill serial no or uncheck serial no box ! ", "Serial no Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetAdvanceSearchedRecord();
                panelSearch.Visible = false;
            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill serial no or uncheck serial no box ! ", "Serial no Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetAdvanceSearchedRecord();              
                panelSearch.Visible = false;
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private void dgrdorder_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdorder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    ShowOrderBookingPage();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event in Datagrid View in Show Order Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowOrderBookingPage()
        {
            string strOrderNo = Convert.ToString(dgrdorder.CurrentRow.Cells["serialNo"].Value); 
            string[] strOrder = strOrderNo.Split(' ');
            if (strOrder.Length > 1)
            {
                if (strOrder[0] != "" && strOrder[1] != "")
                {
                    string strOrderType = Convert.ToString(dgrdorder.CurrentRow.Cells["ordertype"].Value);
                    if (strOrderType == "APPORDER")
                    {
                        AppOrderBooking objOrderBooking = new AppOrderBooking(strOrder[0], strOrder[1]);
                        objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderBooking.Show();
                    }
                    else
                    {
                        OrderBooking objOrderBooking = new OrderBooking(strOrder[0], strOrder[1]);
                        objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderBooking.Show();
                    }
                }
            }
        }

        private void ShowPendingOrderSlip_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            //GetAdvanceSearchedRecord();
        }

        private void rdoPending_CheckedChanged(object sender, EventArgs e)
        {
          //  GetAdvanceSearchedRecord();
        }

        private void rdoClear_CheckedChanged(object sender, EventArgs e)
        {
           // GetAdvanceSearchedRecord();
        }

        private void ClearAllText()
        {
            txtSalesParty.Text = txtSubParty.Text = txtPurchaseParty.Text = txtMarketer.Text = txtItemName.Text = txtOrderCode.Text = txtOrderNo.Text = txtFromSerialNo.Text = txtToSerialNo.Text = "";
           // rdoAll.Checked = true;

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdorder.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    SSS.Reporting.OrderSlipCrystalReport Crystal = new SSS.Reporting.OrderSlipCrystalReport();
                    Crystal.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(Crystal);
                    else
                    {
                        // Crystal.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                        //  Crystal.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        Crystal.PrintToPrinter(1, false, 0, 0);
                    }

                    Crystal.Close();
                    Crystal.Dispose();
                }
                else
                {
                    MessageBox.Show(" Sorry ! There is No Record for Printing ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdorder.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    SSS.Reporting.ShowReport ObjOrder = new SSS.Reporting.ShowReport("Order Slip Register Preview");
                    SSS.Reporting.OrderSlipCrystalReport ObjCrystal = new SSS.Reporting.OrderSlipCrystalReport();
                    ObjCrystal.SetDataSource(dt);
                    ObjOrder.myPreview.ReportSource = ObjCrystal;
                    ObjOrder.Show();

                    ObjCrystal.Close();
                    ObjCrystal.Dispose();
                }
                else
                {
                    MessageBox.Show(" Sorry ! There is No Record for Preview ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("DatePeriod", typeof(String));
                myDataTable.Columns.Add("IColumn", typeof(String));
                myDataTable.Columns.Add("IIColumn", typeof(String));
                myDataTable.Columns.Add("IIIColumn", typeof(String));
                myDataTable.Columns.Add("IVColumn", typeof(String));
                myDataTable.Columns.Add("VColumn", typeof(String));
                myDataTable.Columns.Add("VIColumn", typeof(String));
                myDataTable.Columns.Add("VIIColumn", typeof(String));
                myDataTable.Columns.Add("VIIIColumn", typeof(String));
                myDataTable.Columns.Add("IXColumn", typeof(String));
                myDataTable.Columns.Add("XColumn", typeof(String));
                myDataTable.Columns.Add("XIColumn", typeof(String));

                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IXColumnValue", typeof(String));
                myDataTable.Columns.Add("XColumnValue", typeof(String));
                myDataTable.Columns.Add("XIColumnValue", typeof(String));

                myDataTable.Columns.Add("TotalPcs", typeof(String));
                myDataTable.Columns.Add("TotalAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdorder.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    if (dr.Visible == true)
                    {
                        row["CompanyName"] = MainPage.strCompanyName;
                        if (chkDate.Checked == true)
                        {
                            row["DatePeriod"] = txtFromDate.Text + " To " + txtToDate.Text;
                        }
                        if (dgrdorder.Rows.Count > 0)
                        {
                            int j = 0,k=0;
                            while (j < 11)
                            {
                                k = GetColumnIndexFromDisplayIndex(j);
                                row[j + 2] = dgrdorder.Columns[k].HeaderText;
                                //if (j == 1)
                                //{
                                //    row[j + 13] = dr.Cells[j].Value;
                                //}
                                //else
                                //{
                                row[j + 13] = dr.Cells[k].Value;
                               // }
                                if (j == dgrdorder.ColumnCount)
                                {
                                    j = 11;
                                }
                                j++;
                            }
                            row["TotalPcs"] = lblPcs.Text;
                            row["TotalAmt"] = lblAmount.Text;
                            row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                            myDataTable.Rows.Add(row);
                        }
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdorder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Escape)
                {
                    int columnIndex = dgrdorder.CurrentCell.ColumnIndex,rowIndex=dgrdorder.CurrentRow.Index;
                    if (columnIndex >= 0 && rowIndex >= 0)
                    {
                        if (e.KeyCode == Keys.Space)
                        {
                            if (dgrdorder.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                            {
                                dgrdorder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                            }
                            else
                            {
                                dgrdorder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                            }
                        }
                        else if (e.KeyCode == Keys.Enter)
                        {
                            if (columnIndex == 0)
                            {
                                ShowOrderBookingPage();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event in Datagrid View in Show Order Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSerialNo.ReadOnly = txtToSerialNo.ReadOnly = !chkSerial.Checked;
            txtFromSerialNo.Text = txtToSerialNo.Text = "";
        }

        private void txtFromSerialNo_KeyPress(object sender, KeyPressEventArgs e)
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
        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
                    GetRelatedpartyDetails();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtMarketer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MARKETERNAME", "SEARCH MARKETER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMarketer.Text = objSearch.strSelectedData;
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
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SUBPARTY",txtSalesParty.Text, "SEARCH SUB PARTY", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSubParty.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
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

        private void txtOrderCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ORDERCODE", "SEARCH ORDER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtOrderCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                objSetting = new ReportSetting("OrderColumn");
                objSetting.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSetting.ShowDialog();
                if (objSetting.UpdateCounter > 0)
                {
                    ArrangeColumn();
                    SetCategory();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click event of Change Button in Show Order Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private int GetColumnIndexFromDisplayIndex(int _index)
        {
            int _colIndex = 0;
            foreach (DataGridViewColumn _column in dgrdorder.Columns)
            {
                if (_column.DisplayIndex == _index)
                    return _column.Index;
            }
            return _colIndex;
        }

        private void ArrangeColumn()
        {
            try
            {
                string strColumnName = "";
                int count = 0;
                int place = 0;
                _dtOrder = dba.GetDataTable("Select * from OrderColumnSetting Order by  Place asc");
                if (_dtOrder.Rows.Count > 0)
                {
                    foreach (DataRow row in _dtOrder.Rows)
                    {
                        strColumnName = Convert.ToString(row["ColumnName"]);
                        place = Convert.ToInt32(row["Place"]);
                        if (place > 0)
                        {
                            dgrdorder.Columns[strColumnName].DisplayIndex = count;
                            dgrdorder.Columns[strColumnName].Visible = true;
                            count++;
                        }
                        else
                            dgrdorder.Columns[strColumnName].Visible = false;
                    }
                }
            }
            catch { }
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtScheme.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtGraceDays_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("OFFERNAME", "SEARCH GRACE DAYS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGraceDays.Text = objSearch.strSelectedData;
                }
                e.Handled = true;

            }
            catch
            {
            }
        }

        private void chkDelDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDelDate.ReadOnly = txtToDelDate.ReadOnly = !chkDelDate.Checked;
            txtFromDelDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDelDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDelDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDelDate.Checked, false, false);
        }
        private void btnExpand_Click(object sender, EventArgs e)
        {
            btnExpand.Enabled = false;
            try
            {
                if (btnExpand.Text == "&Expand")
                {
                    foreach (DataGridViewRow row in dgrdorder.Rows)
                    {
                        if (!row.Visible)
                            row.Visible = true;
                    }
                    btnExpand.Text = "&Collapse";
                }
                else
                {
                    string strID = "", strNewID = "";
                    foreach (DataGridViewRow row in dgrdorder.Rows)
                    {
                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strNewID != "")
                        {
                            if (strID == strNewID)
                                row.Visible = false;
                        }
                        strNewID = strID;
                    }
                    btnExpand.Text = "&Expand";
                }
            }
            catch
            {
            }
            btnExpand.Enabled = true;
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", Keys.Space);
                objSearch.ShowDialog();
                txtSalesParty.Text = objSearch.strSelectedData;               
                GetRelatedpartyDetails();
            }
            catch
            {
            }
        }

        private void GetRelatedpartyDetails()
        {
            pnlRelatedParty.Visible = false;
            dgrdRelatedParty.Rows.Clear();

            if (txtSalesParty.Text != "")
            {
                DataTable dt = dba.GetRelatedPartyDetails(txtSalesParty.Text);
                if (dt.Rows.Count > 0)
                {
                    dgrdRelatedParty.Rows.Add(dt.Rows.Count);
                    int _index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdRelatedParty.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                        dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["Name"];
                        _index++;
                    }
                }
            }
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", Keys.Space);
                objSearch.ShowDialog();
                txtPurchaseParty.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void txtSalesParty_Leave(object sender, EventArgs e)
        {
            pnlRelatedParty.Visible = false;
        }

        private void txtSalesParty_Enter(object sender, EventArgs e)
        {
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
            else
                pnlRelatedParty.Visible = false;
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtSalesParty.Text;
                    if (strParty != "")
                    {
                        txtSalesParty.Text = strParty;
                        if(strOldParty!="")
                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                    }
                    txtSalesParty.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdorder.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdorder.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdorder.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdorder.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdorder.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdorder.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdorder.Columns.Count; l++)
                        {
                            if (dgrdorder.Columns[l].HeaderText == "" || !dgrdorder.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdorder.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdorder.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "OrderBookingRegister";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);


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

        private void OrderBookingRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdorder);
        }
    }
}
