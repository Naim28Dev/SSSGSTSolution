using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class GoodsReceiveRegister : Form
    {
        DataBaseAccess dba;
        public GoodsReceiveRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void GoodsReceiveRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else if (panelPDF.Visible)
                    panelPDF.Visible = false;
                else if (panelMissingSNo.Visible)
                    panelMissingSNo.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
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
                    SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSubParty.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtItemName.Text = objSearch.strSelectedData;
        //        }
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}

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

        private void txtGRCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("GOODSRCODE", "SEARCH GOODS RECEIPT CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtGRCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = true;
            txtSalesParty.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = false;

                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == ""))
                    MessageBox.Show(" Sorry ! Please fill serial no or uncheck serial no box ! ", "Serial no Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetAdvanceSearchedRecord();

            }
            catch
            {
            }
            btnSearch.Enabled = true;
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
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }


        private void GetAdvanceSearchedRecord()
        {
            try
            {
                if (rdoSummary.Checked || rdoWithoutOrder.Checked || rdoPending.Checked || txtDesignName.Text.Length > 10 || txtOrderNo.Text.Length > 3 || txtInvoiceNo.Text.Length > 3 || txtSalesParty.Text != "" || txtPurchaseParty.Text != "" || MainPage.mymainObject.bShowAllRecord)
                {
                    //string strQuery = " Select GR.*,(CONVERT(varchar,ReceivingDate,103)) BDate,(CONVERT(varchar,OrderDate,103)) ODate,(CONVERT(varchar,InvoiceDate,103)) IDate,(GR.SalePartyID+' '+_SM.Name) SParty,(GR.SubPartyID+' '+_SMS.Name) HParty,(GR.PurchasePartyID+' '+_SMP.Name) PParty,ISNULL(OB.SchemeName,'')SchemeName,ISNULL(OB.OfferName,'')OfferName,ISNULL(OB.Marketer,'')Marketer,ISNULL(SchemeDiscount,0)SchemeDiscount,((GR.Amount*ISNULL(SchemeDiscount,0))/100.00) SchemeAmt from GoodsReceive GR Outer Apply (Select Name from SupplierMaster Where (AreaCode+AccountNo)=GR.SalePartyID) _SM Outer Apply (Select Name,SM.Other as PPName from SupplierMaster SM Where (AreaCode+AccountNo)=GR.PurchasePartyID) _SMP Outer Apply (Select Name from SupplierMaster Where (AreaCode+AccountNo)=GR.SubPartyID) _SMS OUTER APPLY (Select Top 1 OB.SchemeName,OB.OfferName,OB.Marketer,SCD.Discount as SchemeDiscount,OrderCategory from  OrderBooking OB left join Scheme_SupplierDetails SCD on SCD.SupplierName=PPName and SCD.SchemeName=OB.SchemeName Where OB.SalePartyID=GR.SalePartyID and RTRIM(OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=GR.OrderNo)OB  Where ReceiptNo!=0 ", strSubQuery = "";
                    string strQuery = " Select GR.*,(CONVERT(varchar,ReceivingDate,103)) BDate,(CONVERT(varchar,OrderDate,103)) ODate,(CONVERT(varchar,InvoiceDate,103)) IDate,(GR.SalePartyID+' '+_SM.Name) SParty,(GR.SubPartyID+' '+_SMS.Name) HParty,(GR.PurchasePartyID+' '+_SMP.Name) PParty,ISNULL(OB.SchemeName,'')SchemeName,ISNULL(OB.OfferName,'')OfferName,ISNULL(OB.Marketer,'')Marketer,ISNULL(OB.OrderCategory,'')OrderCategory,ISNULL(SCD.Discount,0)SchemeDiscount,((GR.Amount*ISNULL(SCD.Discount,0))/100.00) SchemeAmt from GoodsReceive GR  left join OrderBooking OB on OB.SalePartyID=GR.SalePartyID and RTRIM(OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=GR.OrderNo left join SupplierMaster _SM on _SM.GroupName='SUNDRY DEBTORS' and (_SM.AreaCode+_SM.AccountNo)=GR.SalePartyID left join SupplierMaster _SMP on _SMP.GroupName='SUNDRY CREDITOR' and (_SMP.AreaCode+_SMP.AccountNo)=GR.PurchasePartyID left join SupplierMaster _SMS on _SMS.GroupName='SUB PARTY' and (_SMS.AreaCode+_SMS.AccountNo)=GR.SubPartyID left join Scheme_SupplierDetails SCD on SCD.SupplierName=_SMS.Other and SCD.SupplierName!='' and SCD.SchemeName=OB.SchemeName Where ReceiptNo!=0 ", strSubQuery = "";
                    strSubQuery = CreateQuery();
                    if (strSubQuery != "")
                        strQuery += strSubQuery;

                   // strQuery += " Order By ReceiptCode,ReceiptNo";

                    DataTable dt = dba.GetDataTable(strQuery);
                    BindDataWithGrid(dt);
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors or Sundry Creditor !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Advance Searched Record in Goods Receipt Register", ex.Message };
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
                    strQuery += " and  (GR.ReceivingDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and GR.ReceivingDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkSerial.Checked)
                    strQuery += " and (GR.ReceiptNo >= " + txtFromSerialNo.Text + " and GR.ReceiptNo <=" + txtToSerialNo.Text + ") ";

                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    if (txtPurchaseParty.Text != "PERSONAL")
                    {
                        strFullName = txtPurchaseParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery += " and  GR.PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                    }
                    else
                        strQuery += " and  GR.Personal!='' ";
                }

                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and GR.SalePartyID='" + strFullName[0].Trim() + "' ";
                }

                if (txtSubParty.Text != "")
                {
                    if (txtSubParty.Text == "SELF")
                    {
                        strQuery += " and GR.SubPartyID='" + txtSubParty.Text + "' ";
                    }
                    else
                    {
                        strFullName = txtSubParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                            strQuery += " and GR.SubPartyID='" + strFullName[0].Trim() + "' ";
                    }
                }

                if (txtMarketer.Text != "" && MainPage.strUserRole.Contains("ADMIN"))
                    strQuery += " and OB.Marketer='" + txtMarketer.Text + "' ";
                if (txtScheme.Text != "")
                    strQuery += " and OB.SchemeName='" + txtScheme.Text + "'  ";
                if (txtGraceDays.Text != "")
                    strQuery += " and OB.OfferName='" + txtGraceDays.Text + "' ";

                if (txtItemName.Text != "")
                    strQuery += " and ReceiptNo in (Select ReceiptNo from GoodsReceiveDetails Where ItemName in ('" + txtItemName.Text + "'))  ";

                if (txtDesignName.Text != "")
                    strQuery += " and ReceiptNo in (Select ReceiptNo from GoodsReceiveDetails Where DesignName Like ('%" + txtDesignName.Text + "%'))  ";

                if (txtRemark.Text != "")
                    strQuery += " and GR.Remark Like ('%" + txtRemark.Text + "%') ";

                if (txtOrderNo.Text != "")
                    strQuery += " and GR.OrderNo Like ('%" + txtOrderNo.Text + "%') ";

                if (txtOrderCode.Text != "")
                    strQuery += " and GR.OrderNo Like ('" + txtOrderCode.Text + "%') ";

                if (txtGRCode.Text != "")
                    strQuery += " and GR.ReceiptCode = '" + txtGRCode.Text + "' ";

                if (rdoPending.Checked)
                    strQuery += " and GR.SaleBill='PENDING' ";
                else if (rdoClear.Checked)
                    strQuery += " and GR.SaleBill='CLEAR' ";

                if (txtInvoiceNo.Text != "")
                    strQuery += " and GR.InvoiceNo Like('%" + txtInvoiceNo.Text + "%') ";

                if (rdoWithOrder.Checked)
                    strQuery += " and GR.OrderNo !='' ";
                else if (rdoWithoutOrder.Checked)
                    strQuery += " and GR.OrderNo='' ";

                if (rdoPackedAtOffice.Checked)
                    strQuery += " and GR.PackingStatus ='PACKED' ";
                else if (rdoDirectDelivery.Checked)
                    strQuery += " and GR.PackingStatus='DIRECT' ";
                else if (rdoComeAtOffice.Checked)
                    strQuery += " and GR.PackingStatus='CAMEOFFICE' ";
                else if (rdoSummary.Checked)
                    strQuery += " and GR.PackingStatus='SUMMARY' ";

                if (rdoSingle.Checked)
                    strQuery += " and OrderCategory='*' ";
                else if (rdoDouble.Checked)
                {
                    strQuery += " and OrderCategory='**' ";
                    if(rdoPendingPurchase.Checked)
                        strQuery += " and ISNULL(OtherBillStatus,0)=0 ";
                    else if(rdoClearPurchase.Checked)
                        strQuery += " and ISNULL(OtherBillStatus,0)=1 ";
                }

                //if (chkSchemeSParty.Checked)
                //    strQuery += " and SalesParty in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='Sundry Debtors' and Other!='') ";

                //if(chkSchemePParty.Checked)
                //    strQuery += " and PurchaseParty in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='Sundry Creditor' and Other!='') ";
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Goods Receipt Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                double dAmt = 0, dTAmt = 0, dNetAmt = 0, dSchemeAmt = 0;
                dgrdGoods.Rows.Clear();
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        DataView dv = table.DefaultView;
                        dv.Sort = "ReceiptCode,ReceiptNo";
                        table = dv.ToTable();

                        dgrdGoods.Rows.Add(table.Rows.Count);
                        int rowIndex = 0;
                        string strSchemeName = "", strOfferName = "";
                        foreach (DataRow dr in table.Rows)
                        {
                            DataGridViewRow row = dgrdGoods.Rows[rowIndex];
                            dNetAmt += dba.ConvertObjectToDouble(dr["NetAmount"]);
                            dSchemeAmt += dba.ConvertObjectToDouble(dr["SchemeAmt"]);
                            dTAmt += dAmt = dba.ConvertObjectToDouble(dr["Amount"]);
                            strSchemeName = Convert.ToString(dr["SchemeName"]);
                            strOfferName = Convert.ToString(dr["OfferName"]);

                            row.Cells["date"].Value = dr["BDate"];
                            row.Cells["grsno"].Value = dr["ReceiptCode"] + " " + dr["ReceiptNo"];
                            row.Cells["orderNo"].Value = dr["OrderNo"];
                            row.Cells["oDate"].Value = dr["ODate"];
                            row.Cells["buyer"].Value = dr["SParty"];
                            row.Cells["item"].Value = dr["Item"];
                            row.Cells["pieces"].Value = dr["Pieces"];
                            row.Cells["qty"].Value = dr["Quantity"];
                            row.Cells["amount"].Value = dAmt.ToString("N0", MainPage.indianCurancy);
                            row.Cells["freight"].Value = dr["Freight"];
                            row.Cells["packing"].Value = dr["Packing"];
                            row.Cells["tax"].Value = dr["Tax"];
                            row.Cells["packingStatus"].Value = dr["PackingStatus"];
                            row.Cells["createdBy"].Value = dr["createdBy"];
                            row.Cells["printedBy"].Value = dr["PrintedBy"];
                            row.Cells["updatedBy"].Value = dr["UpdatedBy"];
                            row.Cells["box"].Value = dr["Box"];
                            row.Cells["remark"].Value = dr["Remark"];
                            row.Cells["invoiceDate"].Value = dr["IDate"];
                            row.Cells["invoiceNo"].Value = dr["InvoiceNo"];
                            row.Cells["purchaseType"].Value = dr["PurchaseType"];
                            row.Cells["marketerName"].Value = dr["Marketer"];
                            row.Cells["schemeName"].Value = strSchemeName;
                            row.Cells["offerName"].Value = strOfferName;
                            row.Cells["disAmt"].Value = dr["DisAmount"];
                            row.Cells["specialDis"].Value = dr["SpecialDscAmt"];
                            row.Cells["otherAmt"].Value = dr["OtherSign"] + "" + dr["OtherAmount"];
                            row.Cells["netAmt"].Value = dr["NetAmount"];
                            row.Cells["taxAmt"].Value = dr["TaxAmount"];
                            row.Cells["schemeDIs"].Value = dr["SchemeDiscount"];
                            row.Cells["schemeAmt"].Value = dr["SchemeAmt"];
                            row.Cells["tcsPer"].Value = dr["TcsPer"];
                            row.Cells["tcsAmt"].Value = dr["TCSAmt"];
                            row.Cells["oCategory"].Value = Convert.ToString(dr["OrderCategory"]);
                            
                            

                            if (Convert.ToBoolean(dr["PurchaseStatus"]))
                                row.Cells["purchaseSource"].Value = "DIRECT";
                            else
                                row.Cells["purchaseSource"].Value = "SALES";

                            if (Convert.ToString(dr["HParty"]) != "")
                                row.Cells["subParty"].Value = dr["HParty"];
                            else
                                row.Cells["subParty"].Value = "SELF";
                            if (Convert.ToString(dr["PParty"]) != "")
                                row.Cells["supplier"].Value = dr["PParty"];
                            else
                                row.Cells["supplier"].Value = "PERSONAL";

                            if (Convert.ToString(dr["SaleBill"]).ToUpper() == "CLEAR")
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                            else if (Convert.ToString(dr["PackingStatus"]).ToUpper() == "SUMMARY")
                                row.DefaultCellStyle.BackColor = Color.Tomato;
                            else if (Convert.ToString(dr["OrderNo"]) == "")
                                row.DefaultCellStyle.BackColor = Color.LightGray;

                            if (strSchemeName != "" && strOfferName != "")
                                row.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                            else if (strSchemeName != "")
                                row.DefaultCellStyle.BackColor = Color.LightSalmon;
                            else if (strOfferName != "")
                                row.DefaultCellStyle.BackColor = Color.Thistle;
                            if(Convert.ToString(dr["OtherBillStatus"])!="" && Convert.ToString(dr["OrderCategory"])=="**")
                            {
                                if(Convert.ToBoolean(dr["OtherBillStatus"]))
                                    row.DefaultCellStyle.BackColor = Color.Gold;
                            }

                            if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                                row.DefaultCellStyle.BackColor = Color.Plum;

                            rowIndex++;
                        }
                    }
                }

                lblAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);
                lblSchemeAmt.Text = dSchemeAmt.ToString("N0", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in Goods Receipt Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dgrdGoods_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 1)
                    {
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                            ShowGoodsReceivePDFFile();
                        else
                            ShowGoodsReceivePage();
                    }
                    else if (e.ColumnIndex == 2)
                        ShowOrderBookingPage();
                    //else if (e.ColumnIndex == 15)
                    //    ShowGoodsReceivePDFFile();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Data Grid View in Show Goods Received Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowGoodsReceivePDFFile()
        {
            string strAllGRSNo = Convert.ToString(dgrdGoods.CurrentRow.Cells["grsno"].Value);
            string[] strGRSNo = strAllGRSNo.Split(' ');
            if (strGRSNo.Length > 1)
            {
                if (strGRSNo[0] != "" && strGRSNo[1] != "")
                {
                    DataBaseAccess.ShowPDFFiles(strGRSNo[0], strGRSNo[1]);
                }
            }
        }

        private void ShowGoodsReceivePage()
        {
            string strAllGRSNo = Convert.ToString(dgrdGoods.CurrentRow.Cells["grsno"].Value);
            string[] strGRSNo = strAllGRSNo.Split(' ');
            if (strGRSNo.Length > 1)
            {
                if (strGRSNo[0] != "" && strGRSNo[1] != "")
                {
                    if (Convert.ToString(dgrdGoods.CurrentRow.Cells["purchaseSource"].Value) == "DIRECT")
                    {
                        GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strGRSNo[0], strGRSNo[1]);
                        objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objGoodsReciept.ShowInTaskbar = true;
                        objGoodsReciept.Show();
                    }
                    else
                    {
                        GoodsReceipt objGoodsReciept = new GoodsReceipt(strGRSNo[0], strGRSNo[1]);
                        objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objGoodsReciept.ShowInTaskbar = true;
                        objGoodsReciept.Show();
                    }
                }
            }
        }

        private void ShowOrderBookingPage()
        {
            string strAllOrderNo = Convert.ToString(dgrdGoods.CurrentRow.Cells["orderNo"].Value), strNCode = "", strSerialNo = "";
            if (strAllOrderNo != "")
            {
                string[] strOrder = strAllOrderNo.Split(' ');
                if (strOrder.Length > 1)
                {
                    if (strOrder.Length > 2)
                        strNCode = strOrder[2];
                    object objOrder = DataBaseAccess.ExecuteMyScalar("Select SerialNo from OrderBooking Where OrderCode='" + strOrder[0] + "' and OrderNo=" + strOrder[1] + " and NumberCode='" + strNCode + "' ");
                    strSerialNo = Convert.ToString(objOrder);
                    if (strOrder[0] != "" && strSerialNo != "")
                    {

                        OrderBooking objOrderBooking = new OrderBooking(strOrder[0], strSerialNo);
                        objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objOrderBooking.ShowInTaskbar = true;
                        objOrderBooking.Show();
                    }
                }
            }
        }

        private void dgrdGoods_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdGoods_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdGoods.CurrentRow.Index;
                    if (dgrdGoods.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdGoods.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdGoods.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    int columnIndex = dgrdGoods.CurrentCell.ColumnIndex, rowIndex = dgrdGoods.CurrentRow.Index;
                    if (rowIndex >= 0)
                    {
                        if (columnIndex == 1)
                        {
                            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                ShowGoodsReceivePDFFile();
                            else
                                ShowGoodsReceivePage();
                        }
                        else if (columnIndex == 2)
                            ShowOrderBookingPage();
                        else if (columnIndex == 15)
                            ShowGoodsReceivePDFFile();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Data Grid View in Show Goods Received Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.GoodsReceiveReport report = new Reporting.GoodsReceiveReport();
                    report.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(report);
                    else
                    {
                        report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                        report.PrintToPrinter(1, false, 0, 0);
                    }

                    report.Close();
                    report.Dispose();
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
                btnPreview.Enabled = false;
                if (dgrdGoods.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.ShowReport objShow = new Reporting.ShowReport("Report Summary Preview");
                        Reporting.GoodsReceiveReport report = new Reporting.GoodsReceiveReport();
                        report.SetDataSource(dt);
                        objShow.myPreview.ReportSource = report;
                        objShow.ShowDialog();

                        report.Close();
                        report.Dispose();
                    }

                }
                else
                {
                    MessageBox.Show("No Record for Printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                myDataTable.Columns.Add("Party", typeof(String));
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
                myDataTable.Columns.Add("XIIColumn", typeof(String));
                myDataTable.Columns.Add("XIIIColumn", typeof(String));
                myDataTable.Columns.Add("XIVColumn", typeof(String));
                myDataTable.Columns.Add("XVColumn", typeof(String));
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
                myDataTable.Columns.Add("XIIColumnValue", typeof(String));
                myDataTable.Columns.Add("XIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("XIVColumnValue", typeof(String));
                myDataTable.Columns.Add("XVColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPeti", typeof(String));
                myDataTable.Columns.Add("TotalCartoon", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdGoods.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strCompanyName;
                    row["Party"] = "";

                    for (int i = 0; i < dgrdGoods.Columns.Count; i++)
                    {
                        row[i + 3] = dgrdGoods.Columns[i].HeaderText;
                        row[i + 18] = dr.Cells[i].Value;
                        if (i == 14)
                        {
                            i = dgrdGoods.Columns.Count;
                        }
                    }

                    row["TotalPeti"] = "";
                    row["TotalCartoon"] = "";
                    row["TotalPieces"] = "";
                    row["TotalAmount"] = lblAmount.Text;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                try
                {
                    if (dgrdGoods.Rows.Count > 0)
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
                        for (int j = 1; j < dgrdGoods.Columns.Count + 1; j++)
                        {
                            strHeader = dgrdGoods.Columns[j - 1].HeaderText;
                            if (strHeader == "" || !dgrdGoods.Columns[j - 1].Visible)
                            {
                                _skipColumn++;
                                j++;
                            }

                            ExcelApp.Cells[1, j - _skipColumn] = dgrdGoods.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                        }
                        _skipColumn = 0;
                        // Storing Each row and column value to excel sheet
                        for (int k = 0; k < dgrdGoods.Rows.Count; k++)
                        {
                            for (int l = 0; l < dgrdGoods.Columns.Count; l++)
                            {
                                if (dgrdGoods.Columns[l].HeaderText == "" || !dgrdGoods.Columns[l].Visible)
                                {
                                    _skipColumn++;
                                    l++;
                                }
                                if (l < dgrdGoods.Columns.Count)
                                    ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdGoods.Rows[k].Cells[l].Value.ToString();
                            }
                            _skipColumn = 0;
                        }
                        ExcelApp.Columns.AutoFit();


                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Goods_Receive_Register";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        }
                        xlWorkbook.Close(true, misValue, misValue);
                        ExcelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                        MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorry ! " + ex.Message);
                }                
            }
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private void btnPdfClose_Click(object sender, EventArgs e)
        {
            panelPDF.Visible = false;
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panelMissingSNo.Visible = false;
        }

        private void btnMissingPDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnMissingPDF.Enabled = false;
                ArrayList list = dba.GetMissingPDFFiles();
                panelPDF.Visible = true;
                dgrdPDF.Rows.Clear();
                lblPDFCount.Text = "Total Missing PDF File : " + list.Count.ToString();
                if (list.Count > 0)
                {
                    dgrdPDF.Rows.Add(list.Count);
                    for (int i = 0; i < list.Count; ++i)
                    {
                        dgrdPDF.Rows[i].Cells["pSNo"].Value = (i+1)+".";
                        dgrdPDF.Rows[i].Cells["missingPDF"].Value = list[i];
                    }
                }
                else
                {
                    dgrdPDF.Rows.Clear();
                }
            }
            catch
            {
            }
            btnMissingPDF.Enabled = true;
        }

        private void btnMissing_Click(object sender, EventArgs e)
        {
            btnMissing.Enabled = false;
            try
            {
                DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
                if (chkDate.Checked)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                }
                eDate = eDate.AddDays(1);
                DataTable dt = dba.GetMissingGoodsReceiptNo(sDate,eDate);
                panelMissingSNo.Visible = true;
                dgrdMissingSNo.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdMissingSNo.Rows.Add(dt.Rows.Count);
                    int rowIndex = 0;
                    foreach(DataRow row in dt.Rows)
                    {
                        dgrdMissingSNo.Rows[rowIndex].Cells["sSno"].Value = (rowIndex + 1) + ".";
                        dgrdMissingSNo.Rows[rowIndex].Cells["missingSNo"].Value = row[0];
                        rowIndex++;
                    }
                }              
            }
            catch
            {
            }
            btnMissing.Enabled = true;
        }

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSerialNo.ReadOnly = txtToSerialNo.ReadOnly = !chkSerial.Checked;
            txtFromSerialNo.Text = txtToSerialNo.Text = "";
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

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
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

        private void txtDesignName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                btnPartyName.Enabled = false;
                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH Sundry Debtors", Keys.Space);
                objSearch.ShowDialog();
                txtSalesParty.Text = objSearch.strSelectedData;
                GetRelatedpartyDetails();
                
            }
            catch
            {
            }
            btnPartyName.Enabled = true;
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

        private void dgrdRelatedParty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
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
                        if (strOldParty != "")
                            dgrdRelatedParty.CurrentCell.Value = strOldParty;
                    }
                    txtSalesParty.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void GoodsReceiveRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                dba.EnableCopyOnClipBoard(dgrdGoods);
                if (MainPage.mymainObject.bShowAllRecord || MainPage.mymainObject.bSchemeMaster)
                {
                    dgrdGoods.Columns["schemeDIs"].Visible = dgrdGoods.Columns["schemeAmt"].Visible = true;
                }
            }
            catch { }
        }

        private void dgrdPDF_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex ==1 && e.RowIndex >= 0)
                {
                    string strReceiptNo = Convert.ToString(dgrdPDF.CurrentCell.Value);
                    if(strReceiptNo !="")
                    {
                        GoodscumPurchase obj = new GoodscumPurchase("", strReceiptNo);
                        obj.FormBorderStyle = FormBorderStyle.FixedDialog;
                        obj.ShowInTaskbar = true;
                        obj.Show();
                    }
                }
            }
            catch { }
        }

        private void rdoDouble_CheckedChanged(object sender, EventArgs e)
        {
            grpPurchaseStatus.Enabled = rdoDouble.Checked;
        }
    }
}
