using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class SalesBookRegisters : Form
    {
        DataBaseAccess dba;
        DataGridViewColumn col;
        DataGridViewCell cell;
        DataTable dtOrder = null, dtDetails = null;
        SendSMS objSMS;
        ReportSetting objSetting;
        public SalesBookRegisters()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            if (MainPage.strUserRole.Contains("ADMIN"))
                btnSendBillToSupplier.Enabled = true;
            else
                btnSendBillToSupplier.Enabled = false;
            BindColumn();
        }

        public SalesBookRegisters(string strPartyName)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                objSMS = new SendSMS();
                BindColumn();
                txtSalesParty.Text = strPartyName;
                GetAllData();
            }
            catch
            {
            }
        }

        public SalesBookRegisters(bool pStatus)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                objSMS = new SendSMS();
                BindColumn();
                rdoNoPurchase.Checked = pStatus;
                panelSearch.Visible = true;

            }
            catch
            {
            }
        }

        private void SalesBookRegisters_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Escape)
                {
                    if (pnlRelatedParty.Visible)
                        pnlRelatedParty.Visible = false;
                    else if (panelSearch.Visible)
                        panelSearch.Visible = false;
                    else if (panelMissingSNo.Visible)
                        panelMissingSNo.Visible = false;
                    else
                        this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                    SendKeys.Send("{TAB}");
            }
            catch { }
        }

        public void BindColumn()
        {
            try
            {
                dtOrder = dba.GetDataTable("Select * from FormatSetting where Place > 0 order by  Place asc");
                if (dtOrder.Rows.Count > 0)
                {
                    // Create ID Column

                    col = new DataGridViewColumn();
                    cell = new DataGridViewTextBoxCell();
                    col.CellTemplate = cell;
                    col.HeaderText = "ID";
                    col.Name = "id";
                    col.Visible = false;
                    col.Width = 20;
                    dgrdDetails.Columns.Add(col);

                    col = new DataGridViewColumn();
                    cell = new DataGridViewCheckBoxCell();
                    col.CellTemplate = cell;
                    col.HeaderText = "";
                    col.Name = "chkID";
                    col.Visible = true;
                    col.Width = 30;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgrdDetails.Columns.Add(col);

                    string strColumnName = "", strHeader = "";
                    foreach (DataRow row in dtOrder.Rows)
                    {
                        strColumnName = Convert.ToString(row["ColumnName"]);
                        strHeader = Convert.ToString(row["Header"]);

                        col = new DataGridViewColumn();
                        if (strColumnName == "BillNo" || strColumnName == "GRSNo" || strColumnName== "PackedBillNo")
                        {
                            DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                            linkCol.LinkColor = Color.Black;
                            linkCol.LinkBehavior = LinkBehavior.HoverUnderline;
                            linkCol.HeaderText = strHeader;
                            linkCol.Name = strColumnName;
                            linkCol.Visible = true;
                            if (strColumnName == "BillNo")
                            linkCol.Width = 130;
                            else
                                linkCol.Width = 110;
                            linkCol.SortMode = DataGridViewColumnSortMode.Automatic;
                            dgrdDetails.Columns.Add(linkCol);
                        }
                        else
                        {
                            cell = new DataGridViewTextBoxCell();
                            col.CellTemplate = cell;
                            if (strHeader == "+/-")
                                col.HeaderText = Convert.ToChar(177).ToString();
                            else
                                col.HeaderText = strHeader;

                            col.Name = strColumnName;
                            col.Visible = true;

                            if (strHeader == "+/-" || strHeader == "Disc" || strHeader == "Pcs")
                                col.Width = 35;
                            else if (strHeader == "D Day" || strHeader == "R Pcs")
                                col.Width = 55;
                            else if (strColumnName == "SalesParty" || strColumnName == "SupplierName")
                                col.Width = 150;
                            else if (strColumnName == "LrNumber" || strColumnName == "Transport" || strColumnName == "Remark")
                                col.Width = 120;
                            else
                                col.Width = 100;

                            col.SortMode = DataGridViewColumnSortMode.Automatic;
                            if (strColumnName.Contains("Amt") || strColumnName.Contains("Amount"))
                            {
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                col.DefaultCellStyle.Format = "N2";
                            }
                            dgrdDetails.Columns.Add(col);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding Columns in Show Sales Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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
                    ClearAll();
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

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH Sundry Creditor", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;
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
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransport.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCartonType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CARTONTYPE", "SEARCH CARTON TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCartonType.Text = objSearch.strSelectedData;
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

        private void txtOrderNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = btnSearch.Enabled = false;
                if (rdoBilled.Checked || rdoWithoutLR.Checked || txtSalesParty.Text != "" || txtPurchaseParty.Text != "" || MainPage.mymainObject.bShowAllRecord || txtLrNumber.Text.Length>3)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                        MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else if ((chkSerial.Checked && (txtFromSerialNo.Text == "" || txtToSerialNo.Text == "")))
                        MessageBox.Show("Sorry ! Please enter serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        GetAllData();
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors or Sundry Creditor !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnGo.Enabled = btnSearch.Enabled = true;
        }
    

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private string CreateQuery(ref string strSalesQuery, ref string strRemQuery, ref string strRetailQuery, ref string strRetailPurchaseQuery)
        {
            string strQuery = "";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (SR.BillDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    strRetailQuery += " and  (SR.Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.Date <'" + eDate.ToString("MM/dd/yyyy") + "')  ";

                }

                if (chkSerial.Checked && txtFromSerialNo.Text != "" && txtToSerialNo.Text != "")
                {
                    strQuery += " and (SR.BillNo >= " + txtFromSerialNo.Text + " and SR.BillNo <=" + txtToSerialNo.Text + ") ";
                    strRetailQuery += " and (SR.BillNo >= " + txtFromSerialNo.Text + " and SR.BillNo <=" + txtToSerialNo.Text + ") ";
                }

                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    if (txtPurchaseParty.Text != "PERSONAL")
                    {
                        strFullName = txtPurchaseParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            strSalesQuery += " and PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                            strRemQuery += " and PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                            strRetailPurchaseQuery = " and PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                            strRetailQuery += strRetailPurchaseQuery;
                        }
                    }
                    else
                    {
                        strSalesQuery += " and  SupplierName='PERSONAL' ";
                        strRemQuery += " and  PartyName = 'PERSONAL'  ";
                    }
                }

                if (txtItemName.Text != "")
                {
                    strSalesQuery += " and Items Like('%" + txtItemName.Text + "%') ";
                    strRemQuery += " and Item Like('%" + txtItemName.Text + "%') ";
                }

                if (txtGRNo.Text != "")
                {
                    strSalesQuery += " and  GRSNo Like ('% " + txtGRNo.Text + "') ";
                    strRemQuery += " and SerialNo Like('RM % " + txtGRNo.Text + "') ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }

                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strQuery += " and SR.SalePartyID='" + strFullName[0].Trim() + "' ";
                        strRemQuery += " and GR.SalePartyID='" + strFullName[0].Trim() + "' ";
                        strRetailQuery += " and SalePartyID = '" + strFullName[0].Trim() + "'  ";
                    }
                }

                if (txtSubParty.Text != "")
                {
                    if (txtSubParty.Text == "SELF")
                    {
                        strQuery += " and SR.SubPartyID='" + txtSubParty.Text + "' ";
                        strRemQuery += " and GR.SubPartyID ='" + txtSubParty.Text + "' ";
                        strRetailQuery += " and SubPartyID = '" + txtSubParty.Text + "'  ";
                    }
                    else
                    {
                        strFullName = txtSubParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            strQuery += " and SR.SubPartyID='" + strFullName[0].Trim() + "' ";
                            strRemQuery += " and GR.SubPartyID ='" + strFullName[0].Trim() + "' ";
                            strRetailQuery += " and SubPartyID = '" + strFullName[0].Trim() + "'  ";
                        }
                    }
                }

                if (txtBillCode.Text != "")
                {
                    strQuery += " and SR.BillCode='" + txtBillCode.Text + "' ";
                    strRetailQuery += " and SR.BillCode = '" + txtBillCode.Text + "'  ";
                }

                if (txtTransport.Text != "")
                {
                    strQuery += " and SR.Transport='" + txtTransport.Text + "' ";
                    strRetailQuery += " and SR.TransportName = '" + txtTransport.Text + "'  ";
                }

                if (txtCartonType.Text != "")
                    strQuery += " and SR.CartoneType='" + txtCartonType.Text + "' ";

                if (txtLrNumber.Text != "")
                {
                    strQuery += " and SR.LrNumber Like ('%" + txtLrNumber.Text + "%') ";
                    strRetailQuery += " and LRNumber Like ('%" + txtLrNumber.Text + "%') ";
                }

                if (txtRemark.Text != "")
                {
                    strQuery += " and SR.Remark Like ('%" + txtRemark.Text + "%') ";
                    strRetailQuery += " and Remark Like ('%" + txtRemark.Text + "%') ";
                }

                if (txtNetAmt.Text != "")
                {
                    strQuery += " and CAST(SR.NetAmt as Money) = " + txtNetAmt.Text + " ";
                    strRetailQuery += " and CAST(SR.NetAmt as Money) =  " + txtNetAmt.Text + " ";
                }

                if (rdoWithLR.Checked)
                {
                    strQuery += " and SR.LrNumber!='' ";
                    strRetailQuery += " and SR.LrNumber!='' ";
                }
                else if (rdoWithoutLR.Checked)
                {
                    strQuery += " and SR.LrNumber='' ";
                    strRetailQuery += " and LrNumber='' ";
                }

                if (rdoPackedAtOffice.Checked)
                    strQuery += " and SR.GoodsType ='PACKED' ";
                else if (rdoDirectDelivery.Checked)
                {
                    strQuery += " and SR.GoodsType='DIRECT' ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }
                else if (rdoCameAtOffice.Checked)
                {
                    strQuery += " and SR.GoodsType='CAMEOFFICE' ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }

                if (rdoBilled.Checked)
                {
                    strQuery += " and SR.BillStatus ='BILLED' ";
                    strRetailQuery += " and WaybillNo='' ";
                }
                else if (rdoShipped.Checked)
                {
                    strQuery += " and SR.BillStatus='SHIPPED' ";
                    strRetailQuery += " and WaybillNo!='' ";
                }
                else if (rdoLooseInStock.Checked)
                {
                    strQuery += " and SR.BillStatus='STOCK' ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }

                if (rdoStockInOffice.Checked)
                {
                    strQuery += " and (SR.BillStatus ='BILLED' and (SR.GoodsType ='PACKED' OR SR.GoodsType='CAMEOFFICE')) ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }
                else if (rdoStockInMarket.Checked)
                {
                    strQuery += " and (SR.BillStatus ='BILLED' and SR.GoodsType ='DIRECT') ";
                    strRetailQuery += " and SR.BillNo=0 ";
                }

                if (rdoPurchased.Checked)
                {
                    strSalesQuery += " and PurchaseBill='CLEAR' ";
                    strRemQuery += " and GR.Status='CLEAR' ";
                }
                else if (rdoNoPurchase.Checked)
                {
                    strSalesQuery += " and PurchaseBill='PENDING' ";
                    strRemQuery += " and GR.Status='PENDING' ";
                }

                if (txtScheme.Text != "")
                {
                    strSalesQuery += " and  OB.SchemeName = '" + txtScheme.Text + "' ";
                    strRetailQuery += " and  SchemeName = '" + txtScheme.Text + "' ";
                    strRetailPurchaseQuery += " and  SchemeName = '" + txtScheme.Text + "' ";
                }
                if (txtGraceDays.Text != "")
                {
                    strSalesQuery += " and OB.OfferName='" + txtGraceDays.Text + "' ";
                    strRetailQuery += " and  OfferName = '" + txtGraceDays.Text + "' ";
                    strRetailPurchaseQuery += " and  OfferName = '" + txtGraceDays.Text + "' ";
                }

                if (txtSaleType.Text != "")
                {
                    strSalesQuery += " and SalesType='" + txtSaleType.Text + "' ";
                    strRetailQuery += " and SalesType='" + txtSaleType.Text + "' ";
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Sale Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            string strQuery = "",strRetailQuery="", strSubQuery = "", strSalesQuery = "", strRemQuery = "",strTaxableAmt="0.00", strRetailPurchaseQuery="";
            strSubQuery = CreateQuery(ref strSalesQuery, ref strRemQuery, ref strRetailQuery,ref strRetailPurchaseQuery);
            //if (chkShowTaxableValue.Checked)
            //    strTaxableAmt = " (Select SUM(ROUND((CASE WHEN TaxType=1 then ((_Sales.Amount*100.00)/(100.00+TaxRate)) else _Sales.Amount end),2)) Amt from (Select (_GRD.Amount + ((_GRD.Amount * CAST((SE.DiscountStatus + SE.Discount) as Money)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from GoodsReceiveDetails _GRD OUTER APPLY (Select TaxName,TaxIncluded from SaleTypeMaster SMN Where SR.SalesType = SMN.TaxName  and SMN.SaleType='SALES')SMN Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((_GRD.Rate * 100) / (100 + TaxRate)) else _GRD.Rate end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((_GRD.Rate * 100) / (100 + TaxRate)) else _GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _GRD.ItemName = _IM.ItemName ) as GM  Where (_GRD.ReceiptCode+' '+CAST(_GRD.ReceiptNo as varchar))=SE.GRSNo )_Sales) "; 

            if (strRetailPurchaseQuery != "")
                strRetailPurchaseQuery = strRetailQuery.Replace(strRetailPurchaseQuery, "");
            else
                strRetailPurchaseQuery = strRetailQuery;

            strQuery = " Select * from (Select 0 BillType,[ID],([BillCode]+' '+Cast([BillNo] as varchar))BillNo,[BillCode],[BillNo] as OBillNo,[Transport],[Station],[GoodsType],[DueDays],[PackerName],[CartoneType],[CartoneSize],[NetAddLs],[LrNumber],[Parcel],[Remark],[OtherPer],[Others],[OtherPacking],[Postage],[TotalPcs],CAST(GrossAmt as money) GrossAmt, CAST(FinalAmt as Money)FinalAmt,CAST(NetAmt as Money) NetAmt,[ForwardingChallan],[Marka],[OtherPerText],[OtherText],[CreatedBy],[UpdatedBy],CONVERT(varchar,[BillDate],103) BillDate,CONVERT(varchar,[LrDate],103) LrDate,Convert(varchar,[PackingDate],103)PackingDate,(Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName!='SUB PARTY' and AreaCode+AccountNo=SalePartyID) SalesParty,ISNULL((Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName='SUB PARTY' and AreaCode+AccountNo=SubPartyID),'SELF') SubParty,GRSNo,SupplierName,Items,Pieces,Discount,DiscountStatus,SNDhara,CAST(Amount as Money) Amount,Packing,Freight,Tax,CAST(TotalAmt as money) TotalAmt,PBill,RemPcs,PurchaseBill,Personal,[SalesType],[TaxAmount],[ServiceAmount],[GreenTaxAmt],[WayBillNo],[VehicleNo],[TimeOfSupply],[OtherField],IGSTAmt,CGSTAmt,SGSTAmt,BillDate as Date,AttachedBill,BillStatus,Description,PackedBillNo,Marketer,SchemeName,OfferName,NTaxableAmt as TaxableAmt,NTaxableAmt,[RoundOffSign],[RoundOffAmt] from( "
                     + " Select SR.[ID],SR.[BillCode],SR.[BillNo],SR.[SalesParty],SR.[SubParty],SR.[Transport],SR.[Station],SR.[GoodsType],SR.[DueDays],SR.[PackerName],SR.[PackingDate],SR.[CartoneType],SR.[CartoneSize],SR.[NetAddLs],SR.[LrNumber],SR.[LrDate],SR.[Parcel],SR.[Remark],SR.[OtherPer],SR.[Others],SR.[OtherPacking],SR.[Postage],SR.[TotalPcs],SR.[GrossAmt],SR.[FinalAmt],SR.[NetAmt],SR.[BillDate],SR.[ForwardingChallan],SR.[Marka],SR.[OtherPerText],SR.[OtherText],SR.[CreatedBy],SR.[UpdatedBy],SR.[InsertStatus],SR.[UpdateStatus],SR.[SalePartyID],SR.[SubPartyID],SR.[SalesType],SR.[TaxAmount],SR.[ServiceAmount],SR.[GreenTaxAmt],SR.[WayBillNo],SR.[VehicleNo],SR.[TimeOfSupply],SR.[OtherField],SR.[TaxPer],SR.[AttachedBill],SR.[BillStatus],SR.[Description],SR.[PackedBillNo],SR.[WayBillDate],SR.[Description_1],SR.[Description_2],SR.[RoundOffSign],SR.[RoundOffAmt],SE.GRSNo,(CASE When SE.SupplierName !='PERSONAL' then (Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName!='SUB PARTY' and AreaCode+AccountNo=SE.PurchasePartyID) else SE.SupplierName end) SupplierName,Items,Pieces,Discount,DiscountStatus,SNDhara,SE.Amount,SE.Packing,SE.Freight,SE.Tax,SE.TotalAmt,PBill,RemPcs,PurchaseBill,Personal,CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt,OB.Marketer,OB.SchemeName,OB.OfferName," + strTaxableAmt+ " TaxableAmt,ROUND((CAST([NetAmt] as Money)-TaxAmount),2) NTaxableAmt  from SalesRecord SR inner join SalesEntry SE on SR.BillCode=SE.BillCode and SR.BillNo=SE.BillNo OUTER APPLY(Select ROUND(SUM(((GD.TaxAmount*100)/GD.TaxRate)),2) NTaxableAmt,(CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='SALES' and GD.BillCode=SR.BillCode and GD.BillNo=SR.BillNo Group by TaxType) GD  Outer APPLY (Select Marketer,SchemeName,OfferName from OrderBooking OB Inner join GoodsReceive GR on (CASE When OB.NumberCode!='' then (OrderCode+' '+ CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where SE.GRSNo in ((GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)))) OB Where SR.BillNo!=0 " + strSubQuery + strSalesQuery + " Union All  "
                     + " Select SR.[ID],SR.[BillCode],SR.[BillNo],SR.[SalesParty],SR.[SubParty],SR.[Transport],SR.[Station],SR.[GoodsType],SR.[DueDays],SR.[PackerName],SR.[PackingDate],SR.[CartoneType],SR.[CartoneSize],SR.[NetAddLs],SR.[LrNumber],SR.[LrDate],SR.[Parcel],SR.[Remark],SR.[OtherPer],SR.[Others],SR.[OtherPacking],SR.[Postage],SR.[TotalPcs],SR.[GrossAmt],SR.[FinalAmt],SR.[NetAmt],SR.[BillDate],SR.[ForwardingChallan],SR.[Marka],SR.[OtherPerText],SR.[OtherText],SR.[CreatedBy],SR.[UpdatedBy],SR.[InsertStatus],SR.[UpdateStatus],SR.[SalePartyID],SR.[SubPartyID],SR.[SalesType],SR.[TaxAmount],SR.[ServiceAmount],SR.[GreenTaxAmt],SR.[WayBillNo],SR.[VehicleNo],SR.[TimeOfSupply],SR.[OtherField],SR.[TaxPer],SR.[AttachedBill],SR.[BillStatus],SR.[Description],SR.[PackedBillNo],SR.[WayBillDate],SR.[Description_1],SR.[Description_2],SR.[RoundOffSign],SR.[RoundOffAmt], GR.SerialNo as GRSNo,(CASE When GR.PartyName !='PERSONAL' then (Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName!='SUB PARTY' and AreaCode+AccountNo=GR.PurchasePartyID) else PartyName end) SupplierName,GR.Item Items,GR.Pieces,'0' Discount,'' DiscountStatus,'' as SNDhara,'0' as Amount,'0' Packing,'0' Freight,'0' Tax,'0' TotalAmt,'' PBill,'0' as RemPcs,'CLEAR' PurchaseBill,'' Personal,0 as IGSTAmt,0 as CGSTAmt,0 as SGSTAmt,'' as Marketer,'' as SchemeName,'' as OfferName,0 as TaxableAmt,0 as NTaxableAmt from SalesRecord SR inner join  GoodsReturned GR on (SR.BillCode+' '+CAST(SR.BillNo as varchar))=GR.AdjustedSaleBillNumber Where SR.BillNo!=0 " + strSubQuery + strRemQuery + " UNION ALL "
                     + " Select SR.[ID],SR.[BillCode],SR.[BillNo],'' as [SalesParty],'' [SubParty],[TransportName] as Transport,[Station],'' [GoodsType],'' [DueDays],[PackerName],[PackingDate],[CartonType] as [CartoneType],CartonSize as [CartoneSize] ,'' as [NetAddLs],[LrNumber],[LrDate],'' as [Parcel],[Remark],'' as [OtherPer],''  [Others],CAST(SR.PackingAmt as varchar) as  [OtherPacking],CAST(SR.PostageAmt as varchar) as [Postage],CAST(TotalQty as varchar) as [TotalPcs],CAST([GrossAmt] as varchar)[GrossAmt],CAST([FinalAmt] as varchar)[FinalAmt],CAST([NetAmt] as varchar) as [NetAmt],[Date] as [BillDate],'' [ForwardingChallan],'' [Marka],'' as [OtherPerText],'' as [OtherText],SR.[CreatedBy],SR.[UpdatedBy],SR.[InsertStatus],SR.[UpdateStatus],[SalePartyID],[SubPartyID],[SalesType],TaxAmt as  [TaxAmount], [OtherAmt] as [ServiceAmount],GreenTax as [GreenTaxAmt],[WayBillNo],'' [VehicleNo],'' [TimeOfSupply],CAST(NoOfCase as varchar) as [OtherField],[TaxPer],[AttachedBill],'' [BillStatus],[Description],[PackedBillNo],[WayBillDate],'' [Description_1],'' [Description_2],SR.[RoundOffSign],SR.[RoundOffAmt], '' as GRSNo,_PB.SupplierName,SE.ItemName Items,CAST(SE.Qty as varchar) As Pieces,CAST(SE.SDisPer as varchar) Discount,'' DiscountStatus,'' as SNDhara,CAST(SE.Amount as varchar) as Amount,'0' Packing,'0' Freight,'0' Tax,CAST(SR.GrossAmt as varchar) as TotalAmt,'' PBill,'0' as RemPcs,'' PurchaseBill,'' Personal,CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt,'' as Marketer,SchemeName,OfferName,0 as TaxableAmt,([NetAmt]-TaxAmt)NTaxableAmt from SalesBook SR OUTER APPLY (Select SE.ItemName,SE.Variant1,SE.Variant2,SE.Variant3,SE.Variant4,SE.Variant5,SDisPer,SUM(Qty)Qty,SUM(SE.Amount)Amount,_OB.SchemeName,_OB.OfferName from SalesBookSecondary SE left join OrderBooking _OB ON _OB.SalePartyID=SR.SalePartyID and SE.SONumber=(_OB.OrderCode+' '+CAST(_OB.OrderNo as varchar)+' '+_OB.NumberCode) Where SR.BillCode=SE.BillCode and SR.BillNo=SE.BillNo Group by SE.ItemName,SE.Variant1,SE.Variant2,SE.Variant3,SE.Variant4,SE.Variant5,SDisPer,_OB.SchemeName,_OB.OfferName)SE OUTER APPLY(Select ROUND(SUM(((GD.TaxAmount*100)/GD.TaxRate)),2) NTaxableAmt,(CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='SALES' and GD.BillCode=SR.BillCode and GD.BillNo=SR.BillNo Group by TaxType) GD OUTER APPLY (Select Top 1 (PurchasePartyID+' '+Name) as SupplierName,PurchasePartyID  from (Select Top 1  Variant3 as PurchasePartyID from StockMaster _SM Where Variant3!='' and _SM.ItemName=SE.ItemName and _SM.Variant1=SE.Variant1 and _SM.Variant2=SE.Variant2 UNION ALL Select Top 1 _PB.PurchasePartyID from PurchaseBook _PB CROSS APPLY (Select ItemName,Variant1,Variant2 from PurchaseBookSecondary _PBS Where _PB.BillCode=_PBS.BillCode and _PB.BillNo=_PBS.BillNo)_PBS Where _PBS.ItemName=SE.ItemName and _PBS.Variant1=SE.Variant1 and _PBS.Variant2=SE.Variant2 )_Purchase Cross Apply (Select Name from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=PurchasePartyID)_SM)_PB Where SR.BillNo!=0 " + strRetailQuery+ " ) Sales UNION ALL  "
                     + " Select 1 BillType, 0 as [ID],'' BillNo,'' as [BillCode],0 as OBillNo,'' as [Transport],'' as [Station],'' as [GoodsType],'' as [DueDays],'' as [PackerName],'' as [CartoneType],'' as [CartoneSize],'' [NetAddLs],'' as [LrNumber],'' as [Parcel],'' as [Remark],CAST(SUM([OtherPer]) as nvarchar) as [OtherPer],CAST(SUM([Others]) as nvarchar) as [Others],CAST(SUM([OtherPacking]) as nvarchar) as [OtherPacking],CAST(SUM([Postage]) as nvarchar) as [Postage],'' as [TotalPcs],CAST(SUM([GrossAmt]) as nvarchar) as [GrossAmt],CAST(SUM([FinalAmt]) as nvarchar) as [FinalAmt],CAST(SUM([NetAmt]) as nvarchar) as [NetAmt],'' as [ForwardingChallan],'' as [Marka],'' as [OtherPerText],'' as [OtherText],'' as [CreatedBy],'' as [UpdatedBy],'' BillDate,'' LrDate,'' as PackingDate,'' as  SalesParty,'' as  SubParty,'' as GRSNo,'' as SupplierName,'' as Items,'' as Pieces,'' as Discount,'' as DiscountStatus,'' as SNDhara,'' as Amount,'' as Packing,'' as Freight,'' as Tax,'' as TotalAmt,'' as PBill,'' as RemPcs,'' as PurchaseBill,'' as Personal,'' as [SalesType],SUM([TaxAmount])[TaxAmount],SUM([ServiceAmount])[ServiceAmount],SUM([GreenTaxAmt])[GreenTaxAmt],'' as [WayBillNo],'' as [VehicleNo],'' as [TimeOfSupply],'' as [OtherField],SUM(IGSTAmt)IGSTAmt,SUM(CGSTAmt)CGSTAmt,SUM(SGSTAmt) SGSTAmt,'' as Date,'' as AttachedBill,'' as BillStatus,'' as Description,'' as PackedBillNo,'' Marketer,'' SchemeName,'' OfferName,SUM(NTaxableAmt) as TaxableAmt,SUM(NTaxableAmt)NTaxableAmt,'' as [RoundOffSign],0.00 as [RoundOffAmt] from ( "
                     + " Select CAST(SUM(CAST([OtherPer] as Money)) as numeric(18, 2)) as [OtherPer],CAST(SUM(CAST([Others] as Money)) as numeric(18,2))as [Others],CAST(SUM(CAST([OtherPacking] as Money)) as numeric(18,2)) as [OtherPacking],CAST(SUM(CAST([Postage] as Money)) as numeric(18,2)) as [Postage],'' as [TotalPcs],CAST(SUM(CAST([GrossAmt] as Money)) as numeric(18,2)) as [GrossAmt],CAST(SUM(CAST([FinalAmt] as Money)) as numeric(18,2))  as [FinalAmt],CAST(SUM(CAST([NetAmt] as Money)) as numeric(18,2)) as [NetAmt],SUM([TaxAmount])[TaxAmount],SUM([ServiceAmount])[ServiceAmount],SUM([GreenTaxAmt])[GreenTaxAmt],'' as [WayBillNo],'' as [VehicleNo],'' as [TimeOfSupply],'' as [OtherField],CAST(SUM(GD.IGSTAmt) as numeric(18,2))IGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2))CGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2)) SGSTAmt,0 as TaxableAmt,SUM(ROUND((CAST([NetAmt] as Money)-TaxAmount),2)) as NTaxableAmt from SalesRecord SR OUTER APPLY(Select ROUND(SUM(((GD.TaxAmount*100)/GD.TaxRate)),2) NTaxableAmt,(CASE WHEN GD.TaxType= 'LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode=SR.BillCode and GD.BillNo= SR.BillNo Group by TaxType) GD Where SR.BillNo!=0  " + strSubQuery + "  UNION ALL "
                     + " Select CAST(SUM(CAST(([Description]+CAST(DisAmt as varchar)) as Money)) as numeric(18,2)) as [OtherPer],'0' as [Others],CAST(SUM(PackingAmt) as numeric(18,2)) as [OtherPacking],CAST(SUM(PostageAmt) as numeric(18,2)) as [Postage],'' as [TotalPcs],CAST(SUM(CAST([GrossAmt] as Money)) as numeric(18,2)) as [GrossAmt],CAST(SUM(CAST([FinalAmt] as Money)) as numeric(18,2)) as [FinalAmt],CAST(SUM(CAST([NetAmt] as Money)) as numeric(18,2)) as [NetAmt],SUM([TaxAmt])[TaxAmount],CAST(SUM(CAST(([OtherSign]+CAST(OtherAmt as varchar)) as Money)) as numeric(18,2))  [ServiceAmount],SUM([GreenTax])[GreenTaxAmt],'' as [WayBillNo],'' as [VehicleNo],'' as [TimeOfSupply],'' as [OtherField],CAST(SUM(GD.IGSTAmt) as numeric(18,2))IGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2))CGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2)) SGSTAmt,0 as TaxableAmt,SUM(ROUND(([NetAmt]-TaxAmt),2)) as NTaxableAmt from SalesBook SR OUTER APPLY(Select ROUND(SUM(((GD.TaxAmount*100)/GD.TaxRate)),2) NTaxableAmt,(CASE WHEN GD.TaxType= 'LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType = 'SALES' and GD.BillCode=SR.BillCode and GD.BillNo= SR.BillNo Group by TaxType) GD Where SR.BillNo!=0  " + strRetailPurchaseQuery + " )SumSales "
                     + " )_Sales Order by BillType,[Date],BillCode,OBillNo ";

            dtDetails = dba.GetDataTable(strQuery);
            BindRecordWithGrid(dtDetails);

            panelSearch.Visible = false;
        }

        // + " UNION ALL Select 1 BillType, 0 as [ID],'' BillNo,'' as [BillCode],0 as OBillNo,'' as [Transport],'' as [Station],'' as [GoodsType],'' as [DueDays],'' as [PackerName],'' as [CartoneType],'' as [CartoneSize],'' [NetAddLs],'' as [LrNumber],'' as [Parcel],'' as [Remark],CAST(CAST(SUM(CAST([OtherPer] as Money)) as numeric(18,2)) as nvarchar) as [OtherPer],CAST(CAST(SUM(CAST([Others] as Money)) as numeric(18,2)) as nvarchar) as [Others],CAST(CAST(SUM(CAST([OtherPacking] as Money)) as numeric(18,2)) as nvarchar) as [OtherPacking],CAST(CAST(SUM(CAST([Postage] as Money)) as numeric(18,2)) as nvarchar) as [Postage],'' as [TotalPcs],CAST(CAST(SUM(CAST([GrossAmt] as Money)) as numeric(18,2)) as nvarchar) as [GrossAmt],CAST(CAST(SUM(CAST([FinalAmt] as Money)) as numeric(18,2)) as nvarchar) as [FinalAmt],CAST(CAST(SUM(CAST([NetAmt] as Money)) as numeric(18,2)) as nvarchar) as [NetAmt],'' as [ForwardingChallan],'' as [Marka],'' as [OtherPerText],'' as [OtherText],'' as [CreatedBy],'' as [UpdatedBy],'' BillDate,'' LrDate,'' as PackingDate,'' as  SalesParty,'' as  SubParty,'' as GRSNo,'' as SupplierName,'' as Items,'' as Pieces,'' as Discount,'' as DiscountStatus,'' as SNDhara,'' as Amount,'' as Packing,'' as Freight,'' as Tax,'' as TotalAmt,'' as PBill,'' as RemPcs,'' as PurchaseBill,'' as Personal,'' as [SalesType],SUM([TaxAmount])[TaxAmount],SUM([ServiceAmount])[ServiceAmount],SUM([GreenTaxAmt])[GreenTaxAmt],'' as [WayBillNo],'' as [VehicleNo],'' as [TimeOfSupply],'' as [OtherField],CAST(SUM(GD.IGSTAmt) as numeric(18,2))IGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2))CGSTAmt,CAST(SUM(GD.CGSTAmt) as numeric(18,2)) SGSTAmt,'' as Date,'' as AttachedBill,'' as BillStatus,'' as Description,'' as PackedBillNo,'' Marketer,'' SchemeName,'' OfferName,0 as TaxableAmt from SalesRecord SR OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='SALES' and GD.BillCode=SR.BillCode and GD.BillNo=SR.BillNo Group by TaxType) GD  Where SR.BillNo!=0 " + strSubQuery + "

        private void BindRecordWithGrid(DataTable table)
        {
            try
            {
                dgrdDetails.Rows.Clear();
                double dGAmt = 0, dNetAmt = 0, dTGrossAmt = 0, dTNetAmt = 0, dTaxableAmt = 0, dTaxAmt = 0 ;
                chkAll.Checked = false;
                if (table.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(table.Rows.Count);
                    int rowIndex = 0;
                    string strID = "", strNewID = "";
                    if (dtOrder == null)
                        dtOrder = dba.GetDataTable("Select * from FormatSetting where Place > 0 order by  Place asc");
                    string strColumnName = "", strSchemeName = "", strOfferName = "";
                  
                    foreach (DataRow row in table.Rows)
                    {
                        strNewID = Convert.ToString(row["ID"]);
                        dgrdDetails.Rows[rowIndex].Cells["id"].Value = strNewID;
                        dgrdDetails.Rows[rowIndex].Cells["chkID"].Value = false;
                        strSchemeName = Convert.ToString(row["SchemeName"]);
                        strOfferName = Convert.ToString(row["OfferName"]);
                     

                        if (strID != strNewID)
                        {                            
                            if (Convert.ToString(row["BillType"]) == "0")
                            {
                                dGAmt = Convert.ToDouble(row["GrossAmt"]);
                                dNetAmt = Convert.ToDouble(row["NetAmt"]);                               
                                dTaxAmt += Convert.ToDouble(row["TaxAmount"]);
                                dTaxableAmt += dba.ConvertObjectToDouble(row["NTaxableAmt"]);

                                dTGrossAmt += dGAmt;
                                dTNetAmt += dNetAmt;
                            }
                            foreach (DataRow rowOrder in dtOrder.Rows)
                            {
                                strColumnName = Convert.ToString(rowOrder["ColumnName"]);
                                //if (strColumnName == "NetAmt")
                                //    dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = dNetAmt.ToString("N2", MainPage.indianCurancy);
                                //else if (strColumnName == "GrossAmt")
                                //    dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = dGAmt.ToString("N2", MainPage.indianCurancy);
                                //else
                                    dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = row[strColumnName];
                            }
                            strID = strNewID;
                        }
                        else
                        {
                            foreach (DataRow rowOrder in dtOrder.Rows)
                            {
                                strColumnName = Convert.ToString(rowOrder["ColumnName"]);
                                if (strColumnName == "BillNo" || strColumnName == "GRSNo" || strColumnName == "SupplierName" || strColumnName == "Pieces" || strColumnName == "Items" || strColumnName == "Discount" || strColumnName == "DiscountStatus" || strColumnName == "SNDhara" || strColumnName == "Amount" || strColumnName == "Packing" || strColumnName == "Freight" || strColumnName == "Tax" || strColumnName == "PBill" || strColumnName == "PurchaseBill" || strColumnName == "RemPcs" || strColumnName == "UserName" || strColumnName.ToUpper()== "TAXABLEAMT")
                                    dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = row[strColumnName];
                            }
                            dgrdDetails.Rows[rowIndex].Visible = false;
                        }
                        if (strSchemeName != "" && strOfferName != "")
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                        else if (strSchemeName != "")
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                        else if (strOfferName != "")
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Thistle;

                        if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;

                        rowIndex++;
                    }
                    dgrdDetails.Rows[rowIndex - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                }

                lblGrossAmt.Text = dTGrossAmt.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dTNetAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Data in Sale Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            btnExpand.Enabled = false;
            try
            {
                if (btnExpand.Text == "Expand")
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        if (!row.Visible)
                            row.Visible = true;
                    }
                    btnExpand.Text = "Collapse";
                }
                else
                {
                    string strID = "", strNewID = "";
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strID = Convert.ToString(row.Cells["id"].Value);
                        if (strNewID != "")
                        {
                            if (strID == strNewID)
                                row.Visible = false;
                        }
                        strNewID = strID;
                    }
                    btnExpand.Text = "Expand";
                }
            }
            catch
            {
            }
            btnExpand.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            if (panelSearch.Visible)
                panelSearch.Visible = false;
            else
                panelSearch.Visible = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        Reporting.ShowReport objShow = new Reporting.ShowReport("SALES REGISTER PREVIEW");
                        objShow.myPreview.ReportSource = objSales;
                        objShow.ShowDialog();
                        objSales.Close();
                        objSales.Dispose();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalGrossAmt", typeof(String));
                myDataTable.Columns.Add("TotalNetAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells["chkID"].Value))
                    {
                        DataRow row = myDataTable.NewRow();
                        row["CompanyName"] = MainPage.strCompanyName;
                        if (chkDate.Checked)
                            row["DatePeriod"] = "From " + txtFromDate.Text + "   To   " + txtToDate.Text;
                        else
                            row["DatePeriod"] = "";

                        if (txtSalesParty.Text != "")
                            row["Party"] = "SALES REGISTER OF  :  " + txtSalesParty.Text;
                        else
                            row["Party"] = "SALES REGISTER";


                        for (int colIndex = 2; colIndex < dgrdDetails.Columns.Count; colIndex++)
                        {
                            row[colIndex + 1] = dgrdDetails.Columns[colIndex].HeaderText;
                            row[colIndex + 9] = dr.Cells[colIndex].Value;
                            if (colIndex == 9)
                                break;
                        }

                        row["TotalGrossAmt"] = lblGrossAmt.Text;
                        row["TotalNetAmt"] = lblNetAmt.Text;
                        row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                        myDataTable.Rows.Add(row);
                    }
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["chkID"].Value = chkAll.Checked;
            }
            catch
            {
            }
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgrdDetails.Rows.Count > 0)
        //        {
        //            btnPrint.Enabled = false;
        //            DataTable dt = CreateDataTable();
        //            if (dt.Rows.Count > 0)
        //            {
        //                Reporting.SaleRegister objSales = new Reporting.SaleRegister();
        //                objSales.SetDataSource(dt);
        //                objSales.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                objSales.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        //                objSales.PrintToPrinter(1, false, 0, 0);
        //            }
        //            else
        //                MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnPrint.Enabled = true;
        //}

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                   // btnExport.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.SaleRegister objReport = new Reporting.SaleRegister();
                        objReport.SetDataSource(dt);
                        CrystalDecisions.Windows.Forms.CrystalReportViewer objViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
                        objViewer.ReportSource = objReport;
                        objViewer.ExportReport();
                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            //btnExport.Enabled = true;
        }


        private string CreateExcelFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\SalesRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.SaleRegister objRegister = new Reporting.SaleRegister();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
                }
                else
                    strFileName = "";
            }
            catch
            {
                strFileName = "";
            }
            return strFileName;
        }

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\SalesRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.SaleRegister objRegister = new Reporting.SaleRegister();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
                }
                else
                    strFileName = "";
            }
            catch
            {
                strFileName = "";
            }
            return strFileName;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (txtSalesParty.Text != "")
                {
                    string strPath = "", strSubject = "", strBody = "", strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    if (strEmailID != "")
                    {
                        strPath = CreateNormalExcel();
                        if (strPath != "")
                        {
                            strSubject = "SALES REPORT FROM " + MainPage.strCompanyName;
                            strBody = "We are sending Sales Register, which is Attached with this mail, Please Find it.";
                            SendingEmailPage objEmail = new SendingEmailPage(true, txtSalesParty.Text, "", strSubject, strBody, strPath, "", "SALES REPORT");
                            objEmail.ShowDialog();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Party Name can't be blank ", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendSMS.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to send SMS ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = 0, chkCount = 0;
                        foreach (DataGridViewRow row in dgrdDetails.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["chkID"].Value))
                            {
                                chkCount += count = SendSMSToParty(row.Cells["id"].Value);
                                if (count > 0)
                                    row.Cells["chkID"].Value = false;
                            }
                        }
                        if (chkCount > 0)
                        {
                            MessageBox.Show("Thank you ! SMS sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("Warning ! Please select atleast one row for sending sms !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch
            {
            }
            btnSendSMS.Enabled = true;
        }

        private int SendSMSToParty(object objID)
        {
            int count = 0;
            try
            {
                if (dtDetails != null)
                {
                    if (dtDetails.Rows.Count > 0)
                    {
                        DataRow[] dr = dtDetails.Select(String.Format("ID=" + objID));
                        if (dr.Length > 0)
                        {
                            DataRow[] rows = dtDetails.Select(String.Format("BillCode='" + dr[0]["BillCode"] + "' and BillNo=" + dr[0]["BillNo"] + ""));
                            if (rows.Length > 0)
                            {
                                DataRow row = rows[0];
                                string strParty = Convert.ToString(row["SalesParty"]), strMobileNo = dba.GetPartyMobileNo(strParty).ToString();
                                if (strMobileNo != "")
                                {
                                    string strBillNo = row["BillCode"] + " " + row["BillNo"], strLRNo = Convert.ToString(row["LrNumber"]), strLRDate = Convert.ToString(row["LrDate"]), strNetAmt = Convert.ToString(row["NetAmt"]), strTransport = Convert.ToString(row["Transport"]), strDate = Convert.ToString(row["BillDate"]);
                                    string strMessage = "", strSubMsg = "";
                                    if (strTransport != "")
                                        strSubMsg = ", Trp : " + strTransport;
                                    if (strLRNo != "")
                                        strSubMsg += ", Lr.No. " + strLRNo + ", Lr.Dt. " + strLRDate;

                                    strMessage = "We have dispatched one Sale Bill No : " + strBillNo + " :- AMT Rs. " + strNetAmt + ", Dt. " + strDate + strSubMsg + "  from " + GetPPartyName(rows) + ".";

                                    string strResult = objSMS.SendSingleSMS(strMessage, strMobileNo);
                                    if (strResult.Contains("success"))
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return count;
        }

        private string GetPPartyName(DataRow[] rows)
        {
            string strParty = "MIX PARTY";
            if (rows.Length == 1)
            {
                strParty = Convert.ToString(rows[0]["SupplierName"]);
            }
            return strParty;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                objSetting = new ReportSetting("Sales");
                objSetting.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSetting.ShowDialog();
                RefreshPage();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click event of Change Button in Show Sales Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public void RefreshPage()
        {
            try
            {
                dgrdDetails.Columns.Clear();
                lblGrossAmt.Text = lblNetAmt.Text = "0.00";
                BindColumn();
                BindRecordWithGrid(dtDetails);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Refreash Page in Show Sales Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnMissing_Click(object sender, EventArgs e)
        {
            btnMissing.Enabled = false;
            try
            {
                if (!panelMissingSNo.Visible)
                    ShowMissingSerials();
                else
                    panelMissingSNo.Visible = false;
            }
            catch
            {
            }
            btnMissing.Enabled = true;
        }

        private void ShowMissingSerials()
        {
            DateTime sDate = MainPage.startFinDate, eDate = MainPage.endFinDate;
            if (chkDate.Checked)
            {
                sDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }
            eDate = eDate.AddDays(1);
            DataTable dt = dba.GetMissingSaleBillNo(sDate,eDate);
            panelMissingSNo.Visible = true;
            dgrdMissingSNo.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdMissingSNo.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdMissingSNo.Rows[rowIndex].Cells["sSno"].Value = (rowIndex + 1) + ".";
                    dgrdMissingSNo.Rows[rowIndex].Cells["missingSNo"].Value = row[0];
                    rowIndex++;
                }
            }
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panelMissingSNo.Visible = false;
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                e.Cancel = true;
            }
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgrdDetails.Columns[e.ColumnIndex].Name == "BillNo" || dgrdDetails.Columns[e.ColumnIndex].Name == "PackedBillNo")
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                }
                else if (dgrdDetails.Columns[e.ColumnIndex].Name == "GRSNo")
                {
                    string strGoodsNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    string[] strNumber = strGoodsNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            DataBaseAccess.ShowPDFFiles(strNumber[0], strNumber[1]);
                        }
                        else
                        {
                            ShowGoodsReceive(strNumber[0], strNumber[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Sales Grid view  in Show Sales Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                dba.ShowSaleBookPrint(strCode, strBillNo,false, false);
            }
            else
            {
                string strGRSNo = "";
                if (dgrdDetails.Columns.Contains("GRSNo"))
                {
                    strGRSNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["GRSNo"].Value);
                }
                if (strGRSNo != "")
                {
                    SaleBook objSale = new SaleBook(strCode, strBillNo);
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
                else
                {
                    SaleBook_Trading objSale = new SaleBook_Trading(strCode, strBillNo);
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
            }
        }

        private void ShowGoodsReceive(string strCode, string strBillNo)
        {
            GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strCode, strBillNo);
            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objGoodsReciept.ShowInTaskbar = true;
            objGoodsReciept.Show();
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdDetails.CurrentRow.Index;
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;

                    if (dgrdDetails.Columns[columnIndex].Name == "BillNo" || dgrdDetails.Columns[columnIndex].Name == "PackedBillNo")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            ShowSaleBook(strNumber[0], strNumber[1]);
                        }
                    }
                    else if (dgrdDetails.Columns[columnIndex].Name == "GRSNo")
                    {
                        string strGoodsNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strGoodsNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            if (e.Modifiers == Keys.Control)
                            {
                                DataBaseAccess.ShowPDFFiles(strNumber[0], strNumber[1]);
                            }
                            else
                            {
                                ShowGoodsReceive(strNumber[0], strNumber[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Down Event of Sales Grid view  in Show Sales Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dgrdDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
            lblGrossAmt.Text = lblNetAmt.Text = "0.00";
        }

        private void txtToDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
            ClearAll();
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private string CreateNormalExcel()
        {
            NewExcel.Application ExcelApp = new NewExcel.Application();
            NewExcel.Workbook ExcelWorkBook = null;
            NewExcel.Worksheet ExcelWorkSheet = null;
            string strFileName = GetFileName();
            try
            {
                object misValue = System.Reflection.Missing.Value;
                ExcelWorkBook = ExcelApp.Workbooks.Add(NewExcel.XlWBATemplate.xlWBATWorksheet);
                ExcelWorkBook.Worksheets.Add(misValue, misValue, 1, NewExcel.XlSheetType.xlWorksheet);
                ExcelWorkSheet = (NewExcel.Worksheet)ExcelWorkBook.Worksheets[1];
                ExcelWorkSheet.Name = "SALE_DETAILS";

                int colIndex = -1;
                foreach (DataGridViewColumn column in dgrdDetails.Columns)
                {
                    if (colIndex > 0)
                        ExcelWorkSheet.Cells[1, colIndex] = column.HeaderText;
                    colIndex++;
                }

                int _colWidth= 0;
                int columnIndex = 1;
                foreach (NewExcel.Range column in ExcelWorkSheet.Columns)
                {
                    column.HorizontalAlignment = NewExcel.XlHAlign.xlHAlignLeft;
                    _colWidth = dgrdDetails.Columns[columnIndex+1].Width;
                    if(_colWidth > 149)
                    column.ColumnWidth = (double)column.ColumnWidth + 16;
                    else if (_colWidth > 119)
                        column.ColumnWidth = (double)column.ColumnWidth + 10;
                    else if (_colWidth > 99)
                        column.ColumnWidth = (double)column.ColumnWidth + 7;
                    else if (_colWidth > 50)
                        column.ColumnWidth = (double)column.ColumnWidth;
                    else 
                        column.ColumnWidth = (double)column.ColumnWidth-2;
                    column.RowHeight = 15;

                    if (columnIndex+1 > colIndex-1)
                        break;
                    columnIndex++;
                }

                int rowIndex = 2;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkID"].Value))
                    {
                        for (int col = 2; col < dgrdDetails.Columns.Count; col++)
                        {
                            ExcelWorkSheet.Cells[rowIndex, col - 1] = row.Cells[col].Value;
                        }
                        rowIndex++;
                    }
                }

                for (int cIndex = 1; cIndex < dgrdDetails.Columns.Count-1; cIndex++)
                {
                    NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[1, cIndex];
                    objRange.Font.Bold = true;
                    objRange.Interior.ColorIndex = 22;
                }

                for (int rIndex = 2; rIndex < rowIndex; rIndex++)
                {
                    for (int cIndex = 1; cIndex < dgrdDetails.Columns.Count-1; cIndex++)
                    {
                        NewExcel.Range objRange = (NewExcel.Range)ExcelWorkSheet.Cells[rIndex, cIndex];
                        objRange.Cells.BorderAround();                     
                    }
                }

                ExcelWorkBook.SaveAs(strFileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                ExcelWorkBook.Close(true, misValue, misValue);
                ExcelApp.Quit();
                Marshal.ReleaseComObject(ExcelWorkSheet);
                Marshal.ReleaseComObject(ExcelWorkBook);
                Marshal.ReleaseComObject(ExcelApp);
              
            }
            catch(Exception ex)
            {
                strFileName = ex.Message;
            }
            finally
            {
            //    foreach (Process process in Process.GetProcessesByName("Excel"))
            //        process.Kill();
            }
            return strFileName;
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
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

        private void txtSaleType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESTYPE", "SEARCH SALES TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSaleType.Text = objSearch.strSelectedData;
                }
                e.Handled = true;

            }
            catch
            {
            }
        }

        private void btnSendBillToSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                string strBillNos = "", strBillNo = "";
                if (dgrdDetails.Columns.Contains("BillNo"))
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strBillNo = "";
                        if (Convert.ToBoolean(row.Cells["chkID"].Value))
                        {
                            if (dgrdDetails.Columns.Contains("BillNo"))
                                strBillNo = Convert.ToString(row.Cells["BillNo"].Value).Replace(" ", "");
                            if (strBillNo!="")
                            {
                                if (strBillNos != "")
                                    strBillNos += ",";
                                strBillNos += "'" + strBillNo + "'";
                            }
                        }
                    }
                }

                if (strBillNos != "")
                {
                    int _count = dba.SendEmailIDAndWhatsappNumberToSupplier(strBillNos);
                    if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                    else
                        MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
        }

        private void txtSalesParty_Leave(object sender, EventArgs e)
        {
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
                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                    }
                    txtSalesParty.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void txtSalesParty_Enter(object sender, EventArgs e)
        {
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
            else
                pnlRelatedParty.Visible = false;
        }

        private void btnPrintSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrintSelected.Enabled = false;
                    DialogResult result = MessageBox.Show("Are you sure you want to print selected invoice ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        PrintSelectedInvoice();
                    }
                }          
            }
            catch { }
            btnPrintSelected.Enabled = true;
        }

        private void PrintSelectedInvoice()
        {
            try
            {
                string strInvoiceNo = "", strGRSNo = "";
                if (dgrdDetails.Columns.Contains("BillNo"))
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["chkID"].Value))
                        {
                            strInvoiceNo = Convert.ToString(row.Cells["BillNo"].Value);
                            string[] strNumber = strInvoiceNo.Split(' ');
                            if (strNumber.Length > 1)
                            {
                                if (dgrdDetails.Columns.Contains("GRSNo"))
                                    strGRSNo = Convert.ToString(row.Cells["GRSNo"].Value);
                                if (strGRSNo != "")
                                    dba.ShowSaleBookPrint(strNumber[0], strNumber[1], true, true);
                                else
                                    dba.ShowRetailSaleBookPrint(strNumber[0], strNumber[1], true, true);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void SendEmailSelectedInvoice()
        {
            try
            {
                string strBillNos = "", strBillNo = "";
                if (dgrdDetails.Columns.Contains("BillNo"))
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strBillNo = "";
                        if (Convert.ToBoolean(row.Cells["chkID"].Value))
                        {
                            if (dgrdDetails.Columns.Contains("BillNo"))
                                strBillNo = Convert.ToString(row.Cells["BillNo"].Value).Replace(" ", "");
                            if (strBillNo != "")
                            {
                                if (strBillNos != "")
                                    strBillNos += ",";
                                strBillNos += "'" + strBillNo + "'";
                            }
                        }
                    }
                }

                if (strBillNos != "")
                {
                    int _count = dba.SendEmailIDAndWhatsappNumberToCustomer(strBillNos);
                    if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                    else
                        MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
        }

        private void btnEmailSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnEmailSelected.Enabled = false;
                    DialogResult result = MessageBox.Show("Are you sure you want to send selected invoice ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SendEmailSelectedInvoice();
                    }
                }
            }
            catch { }
            btnEmailSelected.Enabled = true;
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                btnPartyName.Enabled = false;
                SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", Keys.Space);
                objSearch.ShowDialog();
                txtSalesParty.Text = objSearch.strSelectedData;
                GetRelatedpartyDetails();
                ClearAll();
            }
            catch
            {
            }
            btnPartyName.Enabled = true;
        }

        private DataTable CreateDataTable_Excel()
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
                myDataTable.Columns.Add("IColumnValue", typeof(String));
                myDataTable.Columns.Add("IIColumnValue", typeof(String));
                myDataTable.Columns.Add("IIIColumnValue", typeof(String));
                myDataTable.Columns.Add("IVColumnValue", typeof(String));
                myDataTable.Columns.Add("VColumnValue", typeof(String));
                myDataTable.Columns.Add("VIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIColumnValue", typeof(String));
                myDataTable.Columns.Add("VIIIColumnValue", typeof(String));
                myDataTable.Columns.Add("TotalPieces", typeof(String));
                myDataTable.Columns.Add("TotalGrossAmt", typeof(String));
                myDataTable.Columns.Add("TotalNetAmt", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
            }
            catch { }
            return myDataTable;
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnExport.Enabled = false;

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
                    for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                        {
                            if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdDetails.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Sale_Book_Register";
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
            catch
            {
            }
            btnExport.Enabled = true;
        }

        private void SalesBookRegisters_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bSaleReport)               
                    dba.EnableCopyOnClipBoard(dgrdDetails);               
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }
        }

        private string GetFileName()
        {
            string strPath = MainPage.strServerPath + "\\Excel_File";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            strPath += "\\SALE_REGISTER.xls";

            try
            {
                FileInfo file = new FileInfo(strPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch
            {
            }
            return strPath;
        }


    }
}

