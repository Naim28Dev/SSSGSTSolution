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
    public partial class GoodsReceiveAdjustment : Form
    {
        DataBaseAccess dba;
        public GoodsReceiveAdjustment()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
           txtGFromDate.Text = txtOFromDate.Text =  MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtGToDate.Text = txtOToDate.Text =  MainPage.endFinDate.ToString("dd/MM/yyyy");

        }        

        private void GoodsReceiveAdjustment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private string CreateQuery(ref string strOQuery)
        {
            string strQuery = "";
            if (txtSalesParty.Text !="")
            {
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {

                    strQuery += " and SalePartyID= '" + strFullName[0].Trim() + "' ";
                    strOQuery += " and SalePartyID= '" + strFullName[0].Trim() + "' ";
                }
            }
            if (txtPurchaseParty.Text != "")
            {
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strQuery += " and PurchasePartyID= '" + strFullName[0].Trim() + "' ";
                    strOQuery += " and PurchasePartyID= '" + strFullName[0].Trim() + "' ";
                }
            }

            if (rdoPending.Checked)
            {
                strQuery += " and (OrderNo='0' OR OrderNo ='')  ";
                strOQuery += " and Status='PENDING' ";
            }
            else if (rdoClear.Checked)
            {
                strQuery += " and (OrderNo!='0' OR OrderNo!='') ";
                strOQuery += " and Status='Clear' ";
            }

            if(txtGRNo.Text!="")            
                strQuery += " and  (ReceiptCode+ ' '+CAST(ReceiptNo as varchar)) Like('% " + txtGRNo.Text + "') ";
            

            if (txtOrderNo.Text != "")            
                strOQuery += " and (OrderCode+ ' '+CAST(OrderNo as varchar)) Like('% " + txtOrderNo.Text + "') ";
           

            if (chkGDate.Checked && txtGFromDate.Text.Length == 10 && txtGToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtGFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtGToDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and  (ReceivingDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and ReceivingDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (chkODate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtOFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
                eDate = eDate.AddDays(1);
                strOQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                dgrdGoods.Rows.Clear();
                dgrdorder.Rows.Clear();

                string strQuery = "", strOQuery = "", strSubQuery = "";
                strSubQuery = CreateQuery(ref strOQuery);
                strQuery = "Select Convert(varchar,ReceivingDate,103) Date,(ReceiptCode+' '+CAST(ReceiptNo as varchar)) ReceiptNo,dbo.GetFullName(SalePartyID) SalesParty,dbo.GetFullName(PurchasePartyID) as PurchaseParty,dbo.GetFullName(SubPartyID) SubSalesParty,OrderNo,CONVERT(varchar,OrderDate,103) ODate,Item,Pieces,Quantity,Amount from GoodsReceive Where ReceiptNo!=0 " + strSubQuery + " Order by GoodsReceive.ReceiptNo desc"
                         + " Select Convert(varchar,Date,103) Date,(OrderCode+' '+CAST(OrderNo as varchar)+(CASE WHEN NumberCode!='' then ' '+NumberCode else '' end)) OrderNumber,(SalePartyID+' '+SName) as S_Party,(PurchasePartyID+' '+PName)as P_Party,Marketer,Items,Pieces,CAST((CAST(OB.Quantity as Money)-OB.AdjustedQty-ISNULL(OB.CancelQty,0)) as Numeric(18,0)) Quantity,Amount,UPPER(Status) Status,ISNULL(ActiveStatus,1) as ActiveStatus from OrderBooking OB OUTER APPLY (Select Name as SName from SupplierMaster SM Where GroupName='SUNDRY DEBTORS' and SM.AreaCode+SM.AccountNo=OB.SalePartyID)_SM1 OUTER APPLY (Select Name as PName from SupplierMaster SM Where GroupName='Sundry Creditor' and SM.AreaCode+SM.AccountNo=OB.PurchasePartyID)_SM2 OUTER APPLY (Select ActiveStatus from SchemeMaster SM Where OB.SchemeName=SM.SchemeName)_SM Where OrderNo!=0 " + strOQuery + " Order by OB.OrderNo desc ";

                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    BindGoodsReceiveDetails(ds.Tables[0]);
                    BindOrderDetails(ds.Tables[1]);                   
                }
            }
            catch
            {
            }
        }

        private void BindGoodsReceiveDetails(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                dgrdGoods.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdGoods.Rows[rowIndex].Cells["gChk"].Value = false;
                    dgrdGoods.Rows[rowIndex].Cells["gDate"].Value = row["Date"];
                    dgrdGoods.Rows[rowIndex].Cells["gGRSNO"].Value = row["ReceiptNo"];
                    dgrdGoods.Rows[rowIndex].Cells["gSalesParty"].Value = row["SalesParty"];
                    dgrdGoods.Rows[rowIndex].Cells["gPurchaseParty"].Value = row["PurchaseParty"];
                    dgrdGoods.Rows[rowIndex].Cells["gSubParty"].Value = row["SubSalesParty"];
                    dgrdGoods.Rows[rowIndex].Cells["gOrderNo"].Value = row["OrderNo"];
                    dgrdGoods.Rows[rowIndex].Cells["gODate"].Value = row["ODate"];
                    dgrdGoods.Rows[rowIndex].Cells["gItem"].Value = row["Item"];
                    dgrdGoods.Rows[rowIndex].Cells["pieces"].Value = row["Pieces"];
                    dgrdGoods.Rows[rowIndex].Cells["gQty"].Value = row["Quantity"];
                    dgrdGoods.Rows[rowIndex].Cells["gAmt"].Value = row["Amount"];
                    if (Convert.ToString(row["OrderNo"]) != "0" && Convert.ToString(row["OrderNo"]) != "")
                        dgrdGoods.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    rowIndex++;
                }
            }
        }

        private void BindOrderDetails(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                dgrdorder.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdorder.Rows[rowIndex].Cells["oChk"].Value = false;
                    dgrdorder.Rows[rowIndex].Cells["oDate"].Value = row["Date"];
                    dgrdorder.Rows[rowIndex].Cells["oOrderNo"].Value = row["OrderNumber"];
                    dgrdorder.Rows[rowIndex].Cells["oSalesParty"].Value = row["S_Party"];
                    dgrdorder.Rows[rowIndex].Cells["oPurchaseParty"].Value = row["P_Party"];
                    dgrdorder.Rows[rowIndex].Cells["oMarketer"].Value = row["Marketer"];
                    dgrdorder.Rows[rowIndex].Cells["oItem"].Value = row["Items"];
                    dgrdorder.Rows[rowIndex].Cells["oPType"].Value = row["Pieces"];
                    dgrdorder.Rows[rowIndex].Cells["oQty"].Value = row["Quantity"];
                    dgrdorder.Rows[rowIndex].Cells["oAmount"].Value = row["Amount"];
                    dgrdorder.Rows[rowIndex].Cells["oStatus"].Value = row["Status"];
                    dgrdorder.Rows[rowIndex].Cells["activeStatus"].Value = row["ActiveStatus"];

                    if (Convert.ToString(row["Status"]) == "CLEAR")
                        dgrdorder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    if(!Convert.ToBoolean(row["ActiveStatus"]))
                        dgrdorder.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                    rowIndex++;
                }
            }
        }

        private void dgrdorder_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdGoods_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdorder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
                try
                {
                    if (e.ColumnIndex == 0)
                    {
                        if (Convert.ToBoolean(dgrdorder.CurrentCell.EditedFormattedValue))
                        {
                            foreach (DataGridViewRow row in dgrdorder.Rows)
                            {
                                if (row != dgrdorder.CurrentRow)
                                    row.Cells["oChk"].Value = false;
                            }
                        }
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        ShowOrderBookingPage();
                    }
                }
                catch
                {
                }
            
        }

        private void dgrdGoods_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.ColumnIndex == 0)
                {
                    if (Convert.ToBoolean(dgrdGoods.CurrentCell.EditedFormattedValue))
                    {
                        foreach (DataGridViewRow row in dgrdGoods.Rows)
                        {
                            if (row != dgrdGoods.CurrentRow)
                                row.Cells["gChk"].Value = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 2)
                {
                    ShowGoodsReceivePage();
                }
            }
            catch
            {
            }

        }

        private void ShowOrderBookingPage()
        {
            string strOrderNo = Convert.ToString(dgrdorder.CurrentRow.Cells["oOrderNo"].Value);
            string[] strOrder = strOrderNo.Split(' ');
            if (strOrder.Length > 1)
            {
                if (strOrder[0] != "" && strOrder[1] != "")
                {
                    string strSerialCode = "", strSerialNo = "";
                    dba.GetOrderSerialCodeAndSerialNo(strOrderNo, ref strSerialCode, ref strSerialNo);
                    if (strSerialCode != "" && strSerialNo != "")
                    {
                        OrderBooking objOrderBooking = new OrderBooking(strSerialCode, strSerialNo);
                        objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderBooking.ShowInTaskbar = true;
                        objOrderBooking.Show();
                    }
                }
            }
        }

        private void ShowGoodsReceivePage()
        {
            string strAllGRSNo = Convert.ToString(dgrdGoods.CurrentRow.Cells["gGRSNO"].Value);
            string[] strGRSNo = strAllGRSNo.Split(' ');
            if (strGRSNo.Length > 1)
            {
                if (strGRSNo[0] != "" && strGRSNo[1] != "")
                {
                    GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strGRSNo[0], strGRSNo[1]);
                    objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objGoodsReciept.ShowInTaskbar = true;
                    objGoodsReciept.Show();
                }
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;          
            GetAllData();
          //  txtSalesParty.Text = txtPurchaseParty.Text = "";
            btnGo.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            btnAdjust.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to adjust ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    AdjustRecord();
                }
            }
            catch
            {
            }
            btnAdjust.Enabled = true;
        }

        private void AdjustRecord()
        {
            string strQuery = "", strGQuery = "", strOrderNo = "", strOrderDate = "", strSalesParty = "", strPurchaseParty = "";//,strGRDate="";
            DateTime gDate;
            foreach (DataGridViewRow row in dgrdorder.Rows)
            {
                if (Convert.ToBoolean(row.Cells["oChk"].Value))
                {
                    if (Convert.ToBoolean(row.Cells["activeStatus"].Value))
                    {
                        strOrderNo = Convert.ToString(row.Cells["oOrderNo"].Value);
                        strOrderDate = Convert.ToString(row.Cells["oDate"].Value);
                        strSalesParty = Convert.ToString(row.Cells["oSalesParty"].Value);
                        strPurchaseParty = Convert.ToString(row.Cells["oPurchaseParty"].Value);
                        //strQuery = " Update OrderBooking Set Status='CLEAR',UpdateStatus=1 Where (OrderCode+' '+CAST(OrderNo as varchar)+(CASE WHEN NumberCode!='' then ' '+NumberCode else '' end)) ='" + strOrderNo + "' ";
                        break;
                    }
                    else
                        MessageBox.Show("Sorry ! This order is in fair and Fair is locked now!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (strPurchaseParty != "" && strOrderNo != "")
            {
                string strGOrder = "",strGRSNo="";
                foreach (DataGridViewRow row in dgrdGoods.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["gChk"].Value))
                    {
                        //strGRDate = Convert.ToString(row.Cells["gDate"].Value);
                        //gDate= dba.ConvertDateInExactFormat(strGRDate);

                        if (strSalesParty == Convert.ToString(row.Cells["gSalesParty"].Value) && strPurchaseParty == Convert.ToString(row.Cells["gPurchaseParty"].Value))
                        {
                            strGOrder = Convert.ToString(row.Cells["gOrderNo"].Value);
                            if (strGOrder == "" || strGOrder == "0")
                            {
                                strGRSNo = Convert.ToString(row.Cells["gGRSNO"].Value);
                                if (CheckBillAdjustment(strGRSNo) || MainPage.strUserRole.Contains("SUPERADMIN"))
                                {
                                    DateTime sDate = dba.ConvertDateInExactFormat(strOrderDate);
                                    strGQuery += " Update GoodsReceive Set OrderNo='" + strOrderNo + "',OrderDate='" + sDate.ToString("MM/dd/yyyy") + "',UpdateStatus=1 Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGRSNo + "' ";
                                    if (sDate > Convert.ToDateTime("09/13/2019"))
                                        strGQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)+ GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+GR.Qty),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + row.Cells["gGRSNO"].Value + "'  ";
                                    else
                                        strGQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)+ GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+GR.Qty),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, CAST(GR.Quantity as money) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + row.Cells["gGRSNO"].Value + "'  ";
                                   
                                     strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus],[Reason],[ComputerName]) "
                                              + " Select 'GOODSPURCHASE' as [BillType],ReceiptCode as [BillCode],ReceiptNo as  [BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) as [Date],[NetAmount] as [NetAmt],'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'UPDATION' as [EditStatus],'' as [Reason],'' as [ComputerName] from GoodsReceive Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGRSNo + "' ";
                                }
                            }
                            else
                                MessageBox.Show("Sorry !! This Goods Receive already adjusted with Order No : " + strGOrder + " !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Sorry !! Sundry Debtors and Sundry Creditor didn't macth !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                }
                if (strGQuery != "")
                {
                    strQuery += strGQuery;
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! record successfully adjusted !! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        GetAllData();
                    }
                    else
                        MessageBox.Show("Sorry !! Problem occurred in adjustment please try after some time", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Sorry !! Please select atleast one goods receive for adjustment", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Sorry !! Please select atleast one order for adjustment", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool CheckBillAdjustment(string strpBillNo)
        {
            bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(strpBillNo);
            return netStatus;
        }

        private void UnadjustRecord()
        {
            string strQuery = "", strGQuery = "", strOrderNo = "", strGRSNo="", strOrderDate="";
            foreach (DataGridViewRow row in dgrdorder.Rows)
            {
                if (Convert.ToBoolean(row.Cells["oChk"].Value))
                {
                    if (Convert.ToBoolean(row.Cells["activeStatus"].Value))
                    {
                        strOrderNo = Convert.ToString(row.Cells["oOrderNo"].Value);
                        strOrderDate = Convert.ToString(row.Cells["oDate"].Value);
                        //strQuery = " Update OrderBooking Set Status='PENDING',UpdateStatus=1 Where (OrderCode+' '+CAST(OrderNo as varchar)+(CASE WHEN NumberCode!='' then ' '+NumberCode else '' end)) ='" + strOrderNo + "' ";
                        break;
                    }
                    else
                        MessageBox.Show("Sorry ! This order is in fair and Fair is locked now!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (strOrderNo != "")
            {
                DateTime sDate = dba.ConvertDateInExactFormat(strOrderDate);

                foreach (DataGridViewRow row in dgrdGoods.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["gChk"].Value))
                    {
                        strGRSNo = Convert.ToString(row.Cells["gGRSNO"].Value);
                        if (CheckBillAdjustment(strGRSNo) || MainPage.strUserRole.Contains("SUPERADMIN"))
                        {
                            if (strOrderNo == Convert.ToString(row.Cells["gOrderNo"].Value))
                            {
                                if (sDate > Convert.ToDateTime("09/13/2019"))
                                    strGQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)-GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-GR.Qty),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + row.Cells["gGRSNO"].Value + "'  ";
                                else
                                    strGQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)-GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-GR.Qty),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, CAST(GR.Quantity as money) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + row.Cells["gGRSNO"].Value + "'  ";
                                
                                strGQuery += " Update GoodsReceive Set OrderNo='0',OrderDate=NULL,UpdateStatus=1 Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) ='" + strGRSNo + "' ";
                                break;
                            }
                            else
                                MessageBox.Show("Sorry !! Both Order No didn't match, please select correct goods receive", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                //" Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)- GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-GR.Qty),UpdateStatus=1 from OrderBooking OB  Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and ReceiptCode Like('DL%') and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) ='" + row.Cells["gGRSNO"].Value + "' "
                
                if (strGQuery != "")
                {
                    strQuery += strGQuery;
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! record successfully unadjusted !! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        GetAllData();
                    }
                    else
                        MessageBox.Show("Sorry !! Problem occurred in unadjustment please try after some time", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("Sorry !! Please select atleast one goods receive for unadjustment", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Sorry !! Please select atleast one order for unadjustment", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnUnadjust_Click(object sender, EventArgs e)
        {
            btnUnadjust.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to unadjust ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    UnadjustRecord();
                }
            }
            catch
            {
            }
            btnUnadjust.Enabled = true;
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    txtSalesParty.Text = objRead.ReadDataFromCard("SALESPARTY");
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSalesParty.Text = objSearch.strSelectedData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    txtSalesParty.Text = objRead.ReadDataFromCard("PURCHASEPARTY");
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPurchaseParty.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkODate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.ReadOnly = txtOToDate.ReadOnly = !chkODate.Checked;
            txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkGDate_CheckedChanged(object sender, EventArgs e)
        {
            txtGFromDate.ReadOnly = txtGToDate.ReadOnly = !chkGDate.Checked;
            txtGFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtGToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtOrderNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtOFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtGFromDate_Leave(object sender, EventArgs e)
        {
            if (chkGDate.Checked)
                dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            if (chkODate.Checked)
                dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void dgrdGoods_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdGoods.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdGoods.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdGoods.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch { }
           
        }

        private void dgrdorder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdorder.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdorder.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdorder.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch { }
        }
    }
}
