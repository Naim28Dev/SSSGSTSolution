using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class ShowReportSummary : Form
    {
        DataBaseAccess dba;
        DataGridViewColumn col;
        DataGridViewCell cell;
        DataTable dtOrder = null, dtDetails = null;
        ReportSetting objSetting;
        public ShowReportSummary()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindColumn();           
            txtFromDate.Text = txtGFromDate.Text=txtOFromDate.Text= txtFromDelDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = txtGToDate.Text = txtOToDate.Text = txtToDelDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        public ShowReportSummary(string strSalesParty)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindColumn();
            txtFromDate.Text = txtGFromDate.Text = txtOFromDate.Text = txtFromDelDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = txtGToDate.Text = txtOToDate.Text = txtToDelDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

            txtSalesParty.Text = strSalesParty;
            if (txtSalesParty.Text != "")
            {
                btnSearch.Enabled = btnGo.Enabled = false;
                SearchDetailsData();
                btnSearch.Enabled = btnGo.Enabled = true;
            }
        }


        #region All Text box Events

        private void ShowReportSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlRelatedParty.Visible)
                    pnlRelatedParty.Visible = false;
                else if (pnlColor.Visible)
                    pnlColor.Visible = false;
                else if (panelSearch.Visible)
                    panelSearch.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
            lblGrossAmt.Text =lblTaxableAmt.Text= "0.00";
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkGDate_CheckedChanged(object sender, EventArgs e)
        {
            txtGFromDate.ReadOnly = txtGToDate.ReadOnly = !chkGDate.Checked;
            txtGFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtGToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkODate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.ReadOnly = txtOToDate.ReadOnly = !chkODate.Checked;
            txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkSSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtSFromSNo.ReadOnly = txtSToSNo.ReadOnly = !chkSSerial.Checked;
            txtSFromSNo.Text = txtSToSNo.Text = "";
        }

        //private void chkGSNo_CheckedChanged(object sender, EventArgs e)
        //{
        //    txtGFromSNo.ReadOnly = txtGToSNo.ReadOnly = !chkGSNo.Checked;
        //    txtGFromSNo.Text = txtGToSNo.Text = "";
        //}

        private void chkOSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromSNo.ReadOnly = txtOToSNo.ReadOnly = !chkOSNo.Checked;
            txtOFromSNo.Text = txtOToSNo.Text = "";
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
             dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtGFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkGDate.Checked, false, true);
        }

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkODate.Checked, false, true);
        }

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtMarketerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MARKETERNAME", "SEARCH MARKETER NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMarketerName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtLrNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtCartonSize_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CARTONSIZE", "SEARCH CARTON SIZE", e.KeyCode);
                    objSearch.ShowDialog();
                    GetCartonSize(objSearch.strSelectedData);
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetCartonSize(string strSize)
        {
            txtCartonSize.Clear();
            if (strSize != "")
            {
                string[] strAll = strSize.Split('|');
                if (strAll.Length > 1)
                    txtCartonSize.Text = strAll[0];
            }           
                
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgrdDetails.Rows)
                row.Cells["chkStatus"].Value = chkAll.Checked;
        }

        #endregion

        public void BindColumn()
        {
            try
            {
                dtOrder = dba.GetDataTable("Select * from OrderFormatSetting where Place > 0 order by  Place asc");
                if (dtOrder.Rows.Count > 0)
                {                            

                    col = new DataGridViewColumn();
                    cell = new DataGridViewCheckBoxCell();
                    col.CellTemplate = cell;
                    col.HeaderText = "";
                    col.Name = "chkStatus";
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
                        if (strColumnName == "BillNo" || strColumnName == "OrderNo" || strColumnName == "ReceiptNo" || strColumnName == "BookingNo" || strColumnName == "PackedBillNo")
                        {
                            DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                            linkCol.LinkColor = Color.Black;
                            linkCol.LinkBehavior = LinkBehavior.HoverUnderline;
                            linkCol.HeaderText = strHeader;
                            linkCol.Name = strColumnName;
                            linkCol.Visible = true;
                            linkCol.SortMode = DataGridViewColumnSortMode.Automatic;
                            if (strColumnName == "BillNo")
                                linkCol.Width = 130;
                            else
                                linkCol.Width = 110;
                            linkCol.DefaultCellStyle.NullValue = "";
                            dgrdDetails.Columns.Add(linkCol);
                        }
                        else
                        {
                            cell = new DataGridViewTextBoxCell();
                            col.CellTemplate = cell;
                            col.HeaderText = strHeader;
                            col.Name = strColumnName;
                            col.Visible = true;
                            col.SortMode = DataGridViewColumnSortMode.Automatic;
                            if (strColumnName == "Amount" || strColumnName == "FinalAmt" || strColumnName == "Discount")
                            {
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                col.DefaultCellStyle.Format = "N2";
                            }

                            if (strColumnName == "Quantity" || strColumnName == "Discount")
                                col.Width = 35;
                            else if (strColumnName == "LrNumber" || strColumnName == "Item")
                                col.Width = 100;
                            else if (strColumnName == "SalesParty" || strColumnName == "PurchaseParty" || strColumnName == "Transport")
                                col.Width = 180;
                            else
                                col.Width = 90;

                            col.DefaultCellStyle.NullValue = "";
                            dgrdDetails.Columns.Add(col);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Binding Columns in Show Report Summary Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            if (panelSearch.Visible)
                panelSearch.Visible = false;
            else
                panelSearch.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            SearchDetailsData();
            btnGo.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            SearchDetailsData();
            btnSearch.Enabled = true;
        }

        private void SearchDetailsData()
        {
            try
            {
                ClearAll();
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkSSerial.Checked && (txtSFromSNo.Text == "" || txtSToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSSerial.Focus();
                }
                if (chkDelDate.Checked && (txtFromDelDate.Text.Length != 10 || txtToDelDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter delivery date range or uncheck on delivery date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkOSNo.Checked && (txtOFromSNo.Text == "" || txtOToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter order no range or uncheck on orderl no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkOSNo.Focus();
                }
                else if ((chkGDate.Checked && (txtGFromDate.Text.Length != 10 || txtGToDate.Text.Length != 10)))
                {
                    MessageBox.Show("Sorry ! Please enter goods receive date range or uncheck on goods receive date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkGDate.Focus();
                }
                else if ((chkODate.Checked && (txtOFromDate.Text.Length != 10 || txtOToDate.Text.Length != 10)))
                {
                    MessageBox.Show("Sorry ! Please enter order date range or uncheck on order date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkODate.Focus();
                }
                else if (!rdoWithoutOrder.Checked &&  txtLrNumber.Text.Length <4 && !rdoPendingOrder.Checked && !rdoPendingSale.Checked && !rdoLoose.Checked && !rdoBilled.Checked && !rdoBiltyReceived.Checked && !rdoWithoutLR.Checked && txtSalesParty.Text == "" && txtPurchaseParty.Text == "" && !MainPage.mymainObject.bShowAllRecord)
                {
                    MessageBox.Show("Sorry ! Please enter SUNDRY DEBTORS or SUNDRY CREDITOR !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
        }

        private string CreateQuery(ref string strOrderQuery)
        {
            string strQuery = "";
            try
            {
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (SR.BillDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
                if (chkGDate.Checked && txtGFromDate.Text.Length == 10 && txtGToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtGFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtGToDate.Text);
                    eDate = eDate.AddDays(1);
                    strQuery += " and  (GR.ReceivingDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and GR.ReceivingDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }
                if (chkODate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtOFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
                    eDate = eDate.AddDays(1);
                    strOrderQuery += " and  (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    strQuery += " and  (OB.Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and OB.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }   

                if (chkSSerial.Checked && txtSFromSNo.Text != "" && txtSToSNo.Text != "")
                    strQuery += " and (SR.BillNo >= " + txtSFromSNo.Text + " and SR.BillNo <=" + txtSToSNo.Text + ") ";

                if (chkDelDate.Checked && txtFromDelDate.Text.Length == 10 && txtToDelDate.Text.Length == 10)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDelDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDelDate.Text);
                    eDate = eDate.AddDays(1);
                    strOrderQuery += " and  (DeliveryDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and DeliveryDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                    strQuery += " and  (OB.DeliveryDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and OB.DeliveryDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkOSNo.Checked && txtOFromSNo.Text != "" && txtOToSNo.Text != "")
                {
                    strOrderQuery += " and (OrderNo >= " + txtOFromSNo.Text + " and OrderNo <=" + txtOToSNo.Text + ") ";
                    strQuery += " and (OB.OrderNo >= " + txtOFromSNo.Text + " and OB.OrderNo <=" + txtOToSNo.Text + ") ";
                }

                if (txtBillCode.Text != "")
                {
                    strOrderQuery += " and OrderCode in (Select OrderCode from CompanySetting Where SBillCode='"+txtBillCode.Text+"') ";
                    strQuery += " and GR.ReceiptCode in (Select GReceiveCode from CompanySetting Where SBillCode='" + txtBillCode.Text + "') ";
                }

                if (txtSalesParty.Text != "")
                {
                    string[] strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strOrderQuery += " and SalePartyID='" + strFullName[0].Trim() + "'";
                        strQuery += " and GR.SalePartyID='" + strFullName[0].Trim() + "'";
                    }
                }

                if (txtSubParty.Text != "")
                {
                    if (txtSubParty.Text == "SELF")
                    {
                        strOrderQuery += " and SubPartyID='" + txtSubParty.Text + "'";
                        strQuery += " and GR.SubPartyID='" + txtSubParty.Text + "'";
                    }
                    else
                    {
                        string[] strFullName = txtSubParty.Text.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            strOrderQuery += " and SubPartyID='" + strFullName[0].Trim() + "'";
                            strQuery += " and GR.SubPartyID='" + strFullName[0].Trim() + "'";
                        }
                    }
                }

                if (txtPurchaseParty.Text != "")
                {
                    string[] strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strOrderQuery += " and PurchasePartyID='" + strFullName[0].Trim() + "'";
                        strQuery += " and GR.PurchasePartyID='" + strFullName[0].Trim() + "'";
                    }
                }

                if (txtMarketerName.Text != "" && MainPage.strUserRole.Contains("ADMIN"))
                {
                    strOrderQuery += " and Marketer='" + txtMarketerName.Text + "' ";
                    strQuery += " and OB.Marketer='" + txtMarketerName.Text + "' ";
                }
                if (txtScheme.Text != "")
                {
                    strOrderQuery += " and SchemeName='" + txtScheme.Text + "' ";
                    strQuery += " and OB.SchemeName='" + txtScheme.Text + "' ";
                }
                if (txtGraceDays.Text != "")
                {
                    strOrderQuery += " and OfferName='" + txtGraceDays.Text + "' ";
                    strQuery += " and OB.OfferName='" + txtGraceDays.Text + "' ";
                }

                if (txtTransport.Text != "")
                {
                    strOrderQuery += " and Transport='" + txtTransport.Text + "' ";
                    strQuery += " and SR.Transport='" + txtTransport.Text + "' ";
                }

                if (txtCartonType.Text != "")
                    strQuery += " and SR.CartoneType Like('" + txtCartonType.Text + "') ";
               
                if (txtCartonSize.Text != "")
                    strQuery += " and SR.CartoneSize Like('" + txtCartonSize.Text + "') ";
              
                if (txtMarka.Text != "")
                {
                    strOrderQuery += " and Marka Like('" + txtMarka.Text + "') ";
                    strQuery += " and SR.Marka Like('" + txtMarka.Text + "') ";
                }

                if (txtItemName.Text != "")
                {
                    strOrderQuery += " and Items Like('%" + txtItemName.Text + "%') ";
                    strQuery += " and GR.Item Like('%" + txtItemName.Text + "%') ";
                }

                if (txtLrNumber.Text != "")
                    strQuery += " and SR.LrNumber Like('%" + txtLrNumber.Text + "%') ";

                if (txtNetAmt.Text != "")
                    strQuery += " and CAST(GR.Amount as Money)="+txtNetAmt.Text+"  ";

                if (rdoWithOrder.Checked)
                {
                    strQuery += " and GR.OrderNo!='' ";
                    strOrderQuery += " and Status='CLEAR' " ;
                }
                else if (rdoWithoutOrder.Checked)
                {
                    strQuery += " and GR.OrderNo='' ";
                    strOrderQuery += " and Status='PENDING' ";
                }

                if (rdoStockInOffice.Checked)
                    strQuery += " and ((SR.BillStatus ='BILLED' OR SR.BillStatus='STOCK') and (SR.GoodsType ='PACKED' OR SR.GoodsType='CAMEOFFICE')) ";
                else if (rdoStockInMarket.Checked)
                    strQuery += " and (SR.BillStatus ='BILLED' and SR.GoodsType ='DIRECT') ";

                if (rdoBilled.Checked)
                    strQuery += " and SR.BillStatus ='BILLED' ";
                else if (rdoShipped.Checked)
                    strQuery += " and SR.BillStatus='SHIPPED' ";
                else if (rdoLoose.Checked)
                    strQuery += " and SR.BillStatus='STOCK' ";

                //if (rdoOrderBook.Checked)
                //    strQuery += " and (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) in (Select GRSNo from PurchaseRecord) ";
                //else if (rdoSaleBook.Checked)
                //    strQuery += "  and (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) not in (Select GRSNo from PurchaseRecord) ";

                if (rdoSaleBook.Checked)
                    strQuery += " and SR.BillNo is Not NULL ";
                else if (rdoPurchaseBook.Checked)
                    strQuery += " and GR.ReceiptNo is Not NULL ";

                if (rdoWithLR.Checked)
                    strQuery += " and SR.LrNumber !='' ";
                else if (rdoWithoutLR.Checked)
                    strQuery += " and SR.LrNumber ='' ";
                else if (rdoBiltyReceived.Checked)
                    strQuery += " and (SR.LrNumber ='' and GR.PrintedBy!='' and SR.GoodsType ='DIRECT') ";

                if (rdoDirectDelivery.Checked)                
                    strQuery += " and (SR.GoodsType='DIRECT' OR GR.PackingStatus='DIRECT') ";                  
                else if (rdoPackedAtOffice.Checked)
                    strQuery += " and (SR.GoodsType='PACKED' OR GR.PackingStatus='PACKED') ";
                else if (rdoCameAtOffice.Checked)
                    strQuery += " and (SR.GoodsType='CAMEOFFICE' OR GR.PackingStatus='CAMEOFFICE') ";

                if (rdoPendingSale.Checked)
                    strQuery += " and GR.SaleBill='PENDING' ";
                else if (rdoSaleGenerated.Checked)
                    strQuery += " and GR.SaleBill='CLEAR' ";


                //if (chkSchemeSParty.Checked)
                //{
                //    strQuery += " and GR.SalesParty in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='SUNDRY DEBTORS' and Other!='') ";
                //    strOrderQuery += " and S_Party in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='SUNDRY DEBTORS' and Other!='') ";
                //}

                //if (chkSchemePParty.Checked)
                //{
                //    strQuery += " and GR.PurchaseParty in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Other!='') ";
                //    strOrderQuery += " and P_Party in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Other!='') ";
                //}

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Report summary Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strSubQuery = "",strOrderQuery="", strTaxableQuery="0.00";
                strSubQuery = CreateQuery(ref strOrderQuery);
                if (chkShowTaxableValue.Checked)
                    strTaxableQuery = " (Select SUM(ROUND((CASE WHEN TaxType=1 then ((_Sales.Amount*100.00)/(100.00+TaxRate)) else _Sales.Amount end),2)) Amt from (Select (_GRD.Amount + ((_GRD.Amount * CAST((SE.DiscountStatus + SE.Discount) as Money)) / 100))Amount, GM.TaxRate, SMN.TaxIncluded as TaxType from GoodsReceiveDetails _GRD OUTER APPLY (Select TaxName,TaxIncluded from SaleTypeMaster SMN Where SR.SalesType = SMN.TaxName  and SMN.SaleType='SALES')SMN Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((_GRD.Rate * 100) / (100 + TaxRate)) else _GRD.Rate end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN SMN.TaxIncluded = 1 then((_GRD.Rate * 100) / (100 + TaxRate)) else _GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+(SE.DiscountStatus + SE.Discount)) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _GRD.ItemName = _IM.ItemName ) as GM  Where _GRD.ReceiptCode=GR.ReceiptCode and _GRD.ReceiptNo=GR.ReceiptNo )_Sales)  ";

                strQuery = " Select OrderDate,OrderNo,Marketer,GRSNO as ReceiptNo,Convert(varchar,Date,103)ReceivingDate,SParty as SalesParty,SSParty as SubSalesParty,PParty as PurchaseParty,Item,Pieces,Quantity,Amount,BillDate,BillNo,Marka,Transport,Station, LrNumber,LRDate, PackerName,GoodsType,CartoneType,BillStatus,PackedBillNo,SchemeName,OfferName,PendingQty,NetAmt,ORemark,ISNULL(Convert(varchar,DeliveryDate,103),'') DeliveryDate,PrintedBy from ( ";
                if ((rdoAllOrderClear.Checked || rdoPendingOrder.Checked) && (rdoTransactionAll.Checked || rdoOrderBook.Checked))
                    strQuery += " Select Convert(varchar,Date,103) OrderDate,(OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) OrderNo,Marketer,'' GRSNO,Date,dbo.GetFullName(SalePartyID) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') SSParty,ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') PParty,Items as Item,Pieces,Quantity,Amount, '' BillDate,'' BillNo,Marka,Transport,Station,'' LrNumber,'' LRDate, '' PackerName,'' GoodsType,'' CartoneType,'' as BillStatus,'' as PackedBillNo,ISNULL(SchemeName,'') SchemeName,ISNULL(OfferName,'') OfferName,CAST((CAST(Quantity as Money)-AdjustedQty-CancelQty) as Numeric(18,0)) PendingQty,0 as NetAmt,Remark as ORemark,DeliveryDate,'' as PrintedBy from OrderBooking Where Status='PENDING' " + strOrderQuery;
                
                if ((rdoAllOrderClear.Checked || rdoClearOrder.Checked) && !rdoOrderBook.Checked)
                {
                    if ((rdoAllOrderClear.Checked || rdoPendingOrder.Checked) && (rdoTransactionAll.Checked || rdoOrderBook.Checked))
                        strQuery += " Union All ";

                    strQuery += " Select ISNULL(Convert(varchar,OB.Date,103),'') OrderDate,ISNULL(GR.OrderNo,'')OrderNo,ISNULL(OB.Marketer,'') Marketer,(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) GRSNO,GR.ReceivingDate as Date,((GR.SalePartyID)+' '+SM1.Name) SParty,ISNULL(GR.SubPartyID+' '+SM2.Name,'SELF') SSParty,ISNULL((GR.PurchasePartyID+' '+SM3.Name),'PERSONAL') PParty,GR.Item,GR.Pieces,GR.Quantity,Round((GR.Amount+CAST(GR.Freight as float)+CAST(GR.TAX as float)+CAST(GR.Packing as float)),2) Amount,Convert(varchar,SR.BillDate,103) BillDate, (SR.BillCode+' '+CAST(SR.BillNo as varchar)) BillNo,SR.Marka,SR.Transport,SR.Station,SR.LrNumber,CONVERT(varchar,SR.LrDate,103) LRDate,SR.PackerName,SR.GoodsType,SR.CartoneType,SR.BillStatus,SR.PackedBillNo ,ISNULL(OB.SchemeName,'') SchemeName,ISNULL(OB.OfferName,'') OfferName,CAST((CAST(OB.Quantity as Money)-ISNULL(OB.AdjustedQty,0)-ISNULL(OB.CancelQty,0)) as Numeric(18,0)) PendingQty,"+strTaxableQuery+" NetAmt,OB.Remark as ORemark,OB.DeliveryDate,GR.PrintedBy  "
                             + " from GoodsReceive GR left join SalesEntry SE on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)=SE.GRSNo) left join SalesRecord SR on SE.BillCode=SR.BillCode and SE.BillNo=SR.BillNo left Join OrderBooking OB on ((CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo) Outer Apply (Select Name from SupplierMaster Where AreaCode+AccountNo=GR.SalePartyID)SM1  Outer Apply (Select Name from SupplierMaster Where AreaCode+AccountNo=GR.SubPartyID and GR.SubPartyID!='SELF')SM2  Outer Apply (Select Name from SupplierMaster Where AreaCode+AccountNo=GR.PurchasePartyID)SM3  Where GR.ReceiptNo!=0  " + strSubQuery;
                }
                strQuery += " )Summary Order By Date,GRSNO ";

                dtDetails = dba.GetDataTable(strQuery);
                BindRecordWithGrid();
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Gettting data in Report summary register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid()
        {
            try
            {
                if (dtDetails != null)
                {
                    if (dtDetails.Rows.Count > 0)
                    {
                        if (dtOrder == null)
                            dtOrder = dba.GetDataTable("Select * from OrderFormatSetting where Place > 0 order by  Place asc");

                        dgrdDetails.Rows.Add(dtDetails.Rows.Count);
                        int rowIndex = 0;
                        double dAmt = 0, dTAmt = 0, dNAmt = 0, dTNAmt = 0;
                        string strColumnName = "",strOrderNo="",strQty="",strGRSNo="",strSaleBillNo="",strPcsType="",strBillStatus="",strSchemeName="",strOfferName="";
                        
                        foreach (DataRow row in dtDetails.Rows)
                        {
                            dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                            dTNAmt += dNAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                            dgrdDetails.Rows[rowIndex].Cells["chkStatus"].Value = false;
                            strOrderNo = Convert.ToString(row["OrderNo"]);
                            strGRSNo = Convert.ToString(row["ReceiptNo"]);
                            strSaleBillNo = Convert.ToString(row["BillNo"]);
                            strQty = Convert.ToString(row["Quantity"]);
                            strPcsType = Convert.ToString(row["Pieces"]);
                            strBillStatus = Convert.ToString(row["BillStatus"]);
                            strSchemeName = Convert.ToString(row["SchemeName"]);
                            strOfferName = Convert.ToString(row["OfferName"]);
                            if (strGRSNo != "")
                                dTAmt += dAmt;
                            foreach (DataRow dRow in dtOrder.Rows)
                            {
                                strColumnName = Convert.ToString(dRow["ColumnName"]);
                                if (strColumnName != "")
                                {
                                    if (strColumnName == "Amount")
                                        dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                    else if (strColumnName.ToUpper() == "NETAMT")
                                        dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = dNAmt.ToString("N2", MainPage.indianCurancy);
                                    else
                                        dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = Convert.ToString(row[strColumnName]);
                                }
                            }

                            if (strOrderNo != "" && strSaleBillNo != "" && strGRSNo != "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                            else if (strOrderNo == "" && strGRSNo != "" && strSaleBillNo != "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.SkyBlue;                           
                            else if (strOrderNo != "" && strGRSNo == "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                           if(strBillStatus=="STOCK")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Bisque;
                           else if(strBillStatus == "BILLED")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSeaGreen;

                            if (strSchemeName != "" && strOfferName != "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                            else if (strSchemeName != "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                            else if (strOfferName != "")
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Thistle;

                            if (Convert.ToString(row["ORemark"]).Contains("HOLD"))
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;

                            if (strSchemeName.Contains("TOUR") || strSchemeName.Contains("BENGAL"))
                                dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;


                            rowIndex++;
                        }
                      

                        lblGrossAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                        lblTaxableAmt.Text = dTNAmt.ToString("N2", MainPage.indianCurancy);
                    }
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

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {               
                objSetting = new ReportSetting("Order");
                objSetting.UpdateCounter = 0;
                objSetting.ShowDialog();
                if (objSetting.UpdateCounter > 0)
                    RefreshPage();
            }
            catch
            {
            }
        }

        public void RefreshPage()
        {
            try
            {
                ClearAll();
                dgrdDetails.Columns.Clear();
                BindColumn();
                BindRecordWithGrid();
            }
            catch
            {
            }
        }

        private void dgrdDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
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

                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strCompanyName;
                    if(txtSalesParty.Text !="")
                    row["Party"] = "REPORT SUMMARY OF SUNDRY DEBTORS : "+txtSalesParty.Text;
                    else
                        row["Party"] = "REPORT SUMMARY REGISTER";

                    for (int i = 1; i < dgrdDetails.Columns.Count - 2; i++)
                    {                      
                        row[i + 2] = dgrdDetails.Columns[i].HeaderText;
                        row[i + 17] = dr.Cells[i ].Value;
                        if (i == 15)
                            break;
                    }

                    row["TotalPieces"] = lblGrossAmt.Text;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.ShowReport objShow = new Reporting.ShowReport("REPORT SUMMARY PREVIEW");
                        Reporting.ReportSummaryReports objReport = new Reporting.ReportSummaryReports();
                        objReport.SetDataSource(dt);
                        objShow.myPreview.ReportSource = objReport;
                        objShow.ShowDialog();

                        objReport.Close();
                        objReport.Dispose();
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ReportSummaryReports objReport = new Reporting.ReportSummaryReports();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                    {
                        objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                        objReport.PrintToPrinter(1, false, 0, 0);
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
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
                    saveFileDialog.FileName = "Show_Report_Summary";
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

        private string CreatePDFFile()
        {
            string strPath = "", strFileName = "";
            try
            {

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Report_Summary";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\Report_Summary.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ReportSummaryReports objRegister = new Reporting.ReportSummaryReports();
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
            try
            {
                btnSendEmail.Enabled = false;
                if (txtSalesParty.Text != "")
                {
                    string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    //if (strEmailID != "")
                    //{
                    strPath = CreatePDFFile();
                    if (strPath != "")
                    {
                        strSubject = "REPORT SUMMARY REGISTER FROM " + MainPage.strCompanyName;
                        strBody = "We are sending Report Summary Register , which is Attached with this mail, Please Find it.";
                        SendingEmailPage objEmail = new SendingEmailPage(true, txtSalesParty.Text, "", strSubject, strBody, strPath,"","REPORT SUMMARY");
                        objEmail.ShowDialog();
                    }
                    //}
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].Name == "BillNo" || dgrdDetails.Columns[e.ColumnIndex].Name == "PackedBillNo")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        ShowSaleBook(strInvoiceNo);                      
                    }
                    else if (dgrdDetails.Columns[e.ColumnIndex].Name == "ReceiptNo")
                    {
                        string strGoodsNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        ShowGoodsReceive(strGoodsNo);                       
                    }
                    else if (dgrdDetails.Columns[e.ColumnIndex].Name == "OrderNo")
                    {
                        string strOrderNo = Convert.ToString(dgrdDetails.Rows[e.RowIndex].Cells["OrderNo"].Value);
                        if (strOrderNo != "" && strOrderNo != "-------")
                        {
                            ShowOrderBookingPage(strOrderNo);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event in Datagrid View in Show Report Summary Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowSaleBook(string strBillNo)
        {
            if (strBillNo != "")
            {
                string[] strNumber = strBillNo.Split(' ');
                if (strNumber.Length > 1)
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        dba.ShowSaleBookPrint(strNumber[0], strNumber[1],false,false);
                    }
                    else
                    {
                        SaleBook objSaleBook = new SaleBook(strNumber[0], strNumber[1]);
                        objSaleBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSaleBook.ShowInTaskbar = true;
                        objSaleBook.Show();
                    }
                }
            }
        }

        private void ShowGoodsReceive(string strReceiptNo)
        {
            if (strReceiptNo != "")
            {
                string[] strNumber = strReceiptNo.Split(' ');
                if (strNumber.Length > 1)
                {
                    GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strNumber[0], strNumber[1]);
                    objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objGoodsReciept.ShowInTaskbar = true;
                    objGoodsReciept.Show();
                }
            }
        }

        private void ShowOrderBookingPage(string strAllOrderNo)
        {
            if (strAllOrderNo != "")
            {
                string strNCode = "", strSerialNo = "";
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
        }

        private void lnkHint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pnlColor.Visible)
                pnlColor.Visible = false;
            else
                pnlColor.Visible = true;
        }

        private void dgrdDetails_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkAll.Visible =  false;
                    else
                        chkAll.Visible =  true;
                }
            }
            catch
            {
            }
        }

        private void txtScheme_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
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

        private void txtFromDelDate_Leave(object sender, EventArgs e)
        {
                dba.GetDateInExactFormat(sender, chkDelDate.Checked, false, true);
        }

        private void chkDelDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDelDate.ReadOnly = txtToDelDate.ReadOnly = !chkDelDate.Checked;
            txtFromDelDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDelDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
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

        private void ShowReportSummary_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bReportSummary)
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

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int colIndex = dgrdDetails.CurrentCell.ColumnIndex, rowIndex = dgrdDetails.CurrentRow.Index;
                if (colIndex >= 0 && rowIndex >= 0)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (dgrdDetails.Columns[colIndex].Name == "BillNo" || dgrdDetails.Columns[colIndex].Name == "PackedBillNo")
                        {
                            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            ShowSaleBook(strInvoiceNo);
                        }
                        else if (dgrdDetails.Columns[colIndex].Name == "ReceiptNo")
                        {
                            string strGoodsNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            ShowGoodsReceive(strGoodsNo);
                        }
                        else if (dgrdDetails.Columns[colIndex].Name == "OrderNo")
                        {
                            string strOrderNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            if (strOrderNo != "" && strOrderNo != "-------")
                            {
                                ShowOrderBookingPage(strOrderNo);
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                        {
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key down Event in Show Report Summary Slip", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
    }
}
