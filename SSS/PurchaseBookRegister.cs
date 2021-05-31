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
    public partial class PurchaseBookRegister : Form
    {
        DataBaseAccess dba;
        DataGridViewColumn col;
        DataGridViewCell cell;
        DataTable dtOrder = null, dtDetails = null;
        SendSMS objSMS;
        ReportSetting objSetting;
        public PurchaseBookRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            BindColumn();
        }

        public PurchaseBookRegister(string strPName)
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                objSMS = new SendSMS();
                BindColumn();
                txtPurchaseParty.Text = strPName;
                GetAllData();
            }
            catch
            {
            }
        }

        private void PurchaseBookRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelSearch.Visible)
                    panelSearch.Visible = false;
                //else if (panelMissingSNo.Visible)
                //    panelMissingSNo.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        public void BindColumn()
        {
            try
            {
                dtOrder = dba.GetDataTable("Select * from PurchaseFormatSetting where Place > 0 order by  Place asc");
                if (dtOrder.Rows.Count > 0)
                {                 
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
                        if (strColumnName == "BillNo" || strColumnName == "GRSNo" || strColumnName=="SaleBillNo")
                        {
                            DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                            linkCol.LinkColor = Color.Black;
                            linkCol.LinkBehavior = LinkBehavior.HoverUnderline;
                            linkCol.HeaderText = strHeader;
                            linkCol.Name = strColumnName;
                            linkCol.Visible = true;
                            linkCol.Width = 100;
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

                            if (strHeader == "±" || strHeader == "Disc" || strHeader == "Pcs")
                                col.Width = 35;
                            else if (strColumnName == "SalesParty" || strColumnName == "SupplierName" || strColumnName == "Remark")
                                col.Width = 140;
                            else
                                col.Width = 100;

                            col.SortMode = DataGridViewColumnSortMode.Automatic;
                            if (strColumnName.Contains("Date"))
                                col.DefaultCellStyle.Format = "dd/MM/yyyy";
                            if (strColumnName.Contains("Amt"))
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
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSalesParty.Text = objSearch.strSelectedData;
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
                    ClearAll();
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

        private void chkPSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtPFromSNo.ReadOnly = txtPToSNo.ReadOnly = !chkPSNo.Checked;
            txtPFromSNo.Text = txtPToSNo.Text = "";
        }

        private void chkSSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtSFromSNo.ReadOnly = txtSToSNo.ReadOnly = !chkSSNo.Checked;
            txtSFromSNo.Text = txtSToSNo.Text = "";
        }

        private void txtPFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
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
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkPSNo.Checked && (txtPFromSNo.Text == "" || txtPToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkPSNo.Focus();
                }
                else if ((chkSSNo.Checked && (txtSFromSNo.Text == "" || txtSToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter sales serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSSNo.Focus();
                }
                else if (!rdoUnchecked.Checked && txtSalesParty.Text == "" && txtPurchaseParty.Text == "" && !MainPage.mymainObject.bShowAllRecord)
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors or Sundry Creditor !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else if ((chkPSNo.Checked && (txtPFromSNo.Text == "" || txtPToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter purchase serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkPSNo.Focus();
                }
                else if ((chkSSNo.Checked && (txtSFromSNo.Text == "" || txtSToSNo.Text == "")))
                {
                    MessageBox.Show("Sorry ! Please enter sales serial no range or uncheck on serial no ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkSSNo.Focus();
                }
                else if (!rdoUnchecked.Checked && txtSalesParty.Text == "" && txtPurchaseParty.Text == "" && !MainPage.mymainObject.bShowAllRecord)
                {
                    MessageBox.Show("Sorry ! Please enter Sundry Debtors or Sundry Creditor !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesParty.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
            btnSearch.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            panelSearch.Visible=false;
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
                    strQuery += " and  (BillDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (chkPSNo.Checked && txtPFromSNo.Text!="" && txtPToSNo.Text!="")
                    strQuery += " and (BillNo >= " + txtPFromSNo.Text + " and BillNo <=" + txtPToSNo.Text + ") ";
                if (chkSSNo.Checked && txtSFromSNo.Text != "" && txtSToSNo.Text != "")
                    strQuery += " and (CAST(SUBSTRING(SaleBillNo,CHARINDEX(' ',SaleBillNo,0)+1,LEN(SaleBillNo)-CHARINDEX(' ',SaleBillNo,0)+1) as numeric) >= " + txtSFromSNo.Text + " and CAST(SUBSTRING(SaleBillNo,CHARINDEX(' ',SaleBillNo,0)+1,LEN(SaleBillNo)-CHARINDEX(' ',SaleBillNo,0)+1) as numeric) <=" + txtSToSNo.Text + ") ";

                string[] strFullName;
                if (txtPurchaseParty.Text != "")
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and PurchasePartyID = '" + strFullName[0].Trim() + "'  ";
                }

                if (txtItemName.Text != "")              
                    strQuery += " and Item Like('%" + txtItemName.Text + "%') ";                

                if (txtGRNo.Text != "")
                    strQuery += " and GRSNo Like ('% " + txtGRNo.Text + "') ";

                if (txtInvoiceNo.Text != "")
                    strQuery += " and InvoiceNo Like ('%" + txtInvoiceNo.Text + "%') ";

                if (txtSalesParty.Text != "")
                {
                    strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strQuery += " and SalePartyID='" + strFullName[0].Trim() + "' ";
                }
             
                if (txtBillCode.Text != "")
                    strQuery += " and BillCode='" + txtBillCode.Text + "' ";
            

                if (txtNetAmt.Text != "")
                    strQuery += " and CAST(NetAmt as Money) = " + txtNetAmt.Text + " ";

                if (rdoChecked.Checked)
                    strQuery += " and CheckStatus=1 ";
                else if (rdoUnchecked.Checked)
                    strQuery += " and CheckStatus=0 ";

                if (rdoSourceDirect.Checked)
                    strQuery += " and PurchaseSource='DIRECT' ";
                else if (rdoSoureSale.Checked)
                    strQuery += " and PurchaseSource!='DIRECT' ";

                //if (chkWithScheme.Checked)
                //{
                //    strQuery += " and PurchasePartyID in (Select (AreaCode+CAST(AccountNo as varchar)) Name from SupplierMaster Where GroupName='Sundry Creditor' and Other!='') ";
                //}
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in Purchase Book Register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private void GetAllData()
        {
            try
            {
                string strQuery = "", strSubQuery = "";
                strSubQuery = CreateQuery();

                strQuery = "Select * from ( Select 0 BillType, BillNo as PBillNo,''  BillCode,(BillCode+' '+ CAST(BillNo as varchar)) BillNo,GRSNo,(Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName!='SUB PARTY' and AreaCode+AccountNo=SalePartyID) SalesParty,(Select Top 1 AreaCode+AccountNo+' '+Name from SupplierMaster WHere GroupName!='SUB PARTY' and AreaCode+AccountNo=PurchasePartyID) SupplierName,BillDate,CONVERT(varchar,DATEADD(dd,Cast(DueDays as int),BillDate),103) DueDays,SaleBillNo,Pieces,Item,"
                         + " Discount,DiscountStatus,Amount,Freight,Tax,Packing,NetDiscount,Remark,OtherPer,Others,CAST(GrossAmt as Money)GrossAmt, CAST(NetAmt as Money)NetAmt,FreightDiscount,TaxDiscount,PackingDiscount,CreatedBy,UpdatedBy,TaxLedger,TaxAmount,TaxPer,ReverseCharge,[InvoiceNo],[InvoiceDate],[PurchaseSource],(CASE WHEN [CheckStatus]=1 then 'CHECKED' else 'UNCHECKED' end) CheckStatus,[CheckedBy],[Dhara],CAST(GD.IGSTAmt as numeric(18,2))IGSTAmt,CAST(GD.CGSTAmt as numeric(18,2))CGSTAmt,CAST(GD.CGSTAmt as numeric(18,2)) SGSTAmt from PurchaseRecord PR  OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='PURCHASE' and GD.BillCode=(SUBSTRING(PR.GRSNO,0,CHARINDEX(' ',PR.GRSNo,0))) and GD.BillNo=PR.BillNo Group by TaxType) GD Where BillNo!=0 " + strSubQuery + " UNION ALL"
                         + " Select 1 BillType,'' PBillNo,'' as BillCode,'' BillNo,'' as GRSNo,'' as SalesParty,'' as SupplierName,NULL as BillDate,'' DueDays,'' SaleBillNo,'' Pieces,'' Item, '' Discount,'' DiscountStatus,CAST(CAST(SUM(CAST(Amount as Money)) as numeric(18,2)) as nvarchar) as Amount,CAST(CAST(SUM(CAST(Freight as Money)) as numeric(18,2)) as nvarchar) as Freight,CAST(CAST(SUM(CAST(Tax as Money)) as numeric(18,2)) as nvarchar) as Tax,CAST(CAST(SUM(CAST(Packing as Money)) as numeric(18,2)) as nvarchar) as Packing,CAST(CAST(SUM(CAST(NetDiscount as Money)) as numeric(18,2)) as nvarchar) as NetDiscount,'' as Remark,'' OtherPer,'' Others,CAST(CAST(SUM(CAST(GrossAmt as Money)) as numeric(18,2)) as nvarchar) as  GrossAmt,CAST(CAST(SUM(CAST(NetAmt as Money)) as numeric(18,2)) as nvarchar) as NetAmt,'' as FreightDiscount,'' as TaxDiscount,'' as PackingDiscount,'' CreatedBy,'' UpdatedBy,'' TaxLedger,CAST(SUM(CAST(TaxAmount as Money)) as numeric(18,2)) as TaxAmount,'' TaxPer,'' ReverseCharge,'' [InvoiceNo],'' [InvoiceDate],'' [PurchaseSource],'' CheckStatus,'' [CheckedBy],'' [Dhara],CAST(SUM(CAST(IGSTAmt as Money)) as numeric(18,2)) as  IGSTAmt,CAST(SUM(CAST(CGSTAmt as Money)) as numeric(18,2)) as CGSTAmt,CAST(SUM(CAST(CGSTAmt as Money)) as numeric(18,2)) as  SGSTAmt from PurchaseRecord PR  OUTER APPLY(Select (CASE WHEN GD.TaxType='LOCAL' then ROUND((SUM(GD.TaxAmount)/2),2) else 0 end) CGSTAmt,(CASE WHEN GD.TaxType='INTERSTATE' then ROUND(SUM(GD.TaxAmount),2) else 0 end) IGSTAmt from GSTDetails GD WHere BillType='PURCHASE' and GD.BillCode=(SUBSTRING(PR.GRSNO,0,CHARINDEX(' ',PR.GRSNo,0))) and GD.BillNo=PR.BillNo Group by TaxType) GD Where BillNo!=0  " + strSubQuery + ")_Purchase Order by BillType,BillCode,PBillNo ";

                dtDetails = dba.GetDataTable(strQuery);
                BindRecordWithGrid(dtDetails);
                panelSearch.Visible = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Gettting data in Purchase register", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindRecordWithGrid(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            double dGAmt = 0, dNetAmt = 0, dTGrossAmt = 0, dTNetAmt = 0;
            chkAll.Checked = true;
            if (table.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(table.Rows.Count);
                int rowIndex = 0;
                if (dtOrder == null)
                    dtOrder = dba.GetDataTable("Select * from PurchaseFormatSetting where Place > 0 order by  Place asc");
                string strColumnName = "";
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["chkID"].Value = true;
                   
                    dGAmt = Convert.ToDouble(row["GrossAmt"]);
                    dNetAmt = Convert.ToDouble(row["NetAmt"]);
                    if (Convert.ToString(row["BillType"]) == "0")
                    {
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
                        //else if(strColumnName.Contains("Date"))
                        //    dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = row[strColumnName];
                        //else
                        dgrdDetails.Rows[rowIndex].Cells[strColumnName].Value = row[strColumnName]; //dgrdDetails.Rows[rowIndex].Cells[strColumnName].ToolTipText=Convert.ToString(
                    }

                    if (Convert.ToString(row["CheckStatus"]) == "UNCHECKED")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;

                    rowIndex++;
                }

                dgrdDetails.Rows[rowIndex-1].DefaultCellStyle.BackColor = Color.LightGreen;
            }

            lblGrossAmt.Text = dTGrossAmt.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dTNetAmt.ToString("N2", MainPage.indianCurancy);
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

        private void dgrdDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                objSetting = new ReportSetting("Purchase");
                objSetting.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSetting.ShowDialog();
                RefreshPage();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click event of Change Button in Show Purchase Record ", ex.Message };
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
                string[] strReport = { "Error occurred in Refreash Page in Show Purchase Record ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].Name == "BillNo")
                    {
                        string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        string[] strNumber = strInvoiceNo.Split(' ');
                        if (strNumber.Length > 1)
                        {
                            ShowPurchaseBook(strNumber[0], strNumber[1]);
                        }
                    }
                    else if (dgrdDetails.Columns[e.ColumnIndex].Name == "SaleBillNo")
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
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Purchase Grid view  in Show Purchase Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void ShowPurchaseBook(string strCode, string strBillNo)
        {
            //if (dgrdDetails.Columns.Contains("PurchaseSource"))
            //{
            //    if(Convert.ToString(dgrdDetails.CurrentRow.Cells["PurchaseSource"].Value)=="DIRECT")
            //    {
            //        GoodscumPurchase objPurchaseBook = new GoodscumPurchase(strCode, strBillNo);
            //        objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            //        objPurchaseBook.ShowInTaskbar = true;
            //        objPurchaseBook.Show();
            //    }
            //    else
            //    {
            //        PurchaseBook objPurchaseBook = new PurchaseBook(strCode, strBillNo);
            //        objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            //        objPurchaseBook.ShowInTaskbar = true;
            //        objPurchaseBook.Show();
            //    }
            //}
            //else
            //{
                PurchaseBook objPurchaseBook = new PurchaseBook(strCode, strBillNo);
                objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objPurchaseBook.ShowInTaskbar = true;
                objPurchaseBook.Show();
            //}
        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            SaleBook objSale = new SaleBook(strCode, strBillNo);
            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSale.ShowInTaskbar = true;
            objSale.Show();
        }

        private void ShowGoodsReceive(string strCode, string strBillNo)
        {
            if (dgrdDetails.Columns.Contains("PurchaseSource"))
            {
                if (Convert.ToString(dgrdDetails.CurrentRow.Cells["PurchaseSource"].Value) == "DIRECT")
                {
                    GoodscumPurchase objPurchaseBook = new GoodscumPurchase(strCode, strBillNo);
                    objPurchaseBook.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objPurchaseBook.ShowInTaskbar = true;
                    objPurchaseBook.Show();
                }
                else
                {
                    GoodsReceipt objGoodsReciept = new GoodsReceipt(strCode, strBillNo);
                    objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objGoodsReciept.ShowInTaskbar = true;
                    objGoodsReciept.Show();
                }
            }
            else
            {
                GoodsReceipt objGoodsReciept = new GoodsReceipt(strCode, strBillNo);
                objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objGoodsReciept.ShowInTaskbar = true;
                objGoodsReciept.Show();
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentRow.Index >= 0)
                {
                    if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                        else
                            dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (columnIndex >= 0)
                        {
                            if (dgrdDetails.Columns[columnIndex].Name == "BillNo")
                            {
                                string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                                string[] strNumber = strInvoiceNo.Split(' ');
                                if (strNumber.Length > 1)
                                {
                                    ShowPurchaseBook(strNumber[0], strNumber[1]);
                                }
                            }
                            else if (dgrdDetails.Columns[columnIndex].Name == "SaleBillNo")
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
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Down Event of Purchase Grid view  in Show Purchase Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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
                        Reporting.ShowReport objShow = new Reporting.ShowReport("PURCHASE REGISTER PREVIEW");
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
                        row["CompanyName"] = MainPage.strGRCompanyName;
                        if (chkDate.Checked)
                            row["DatePeriod"] = "From " + txtFromDate.Text + "   To   " + txtToDate.Text;
                        else
                            row["DatePeriod"] = "";

                        if (txtSalesParty.Text != "")
                            row["Party"] = "PURCHASE REGISTER OF  :  " + txtSalesParty.Text;
                        else
                            row["Party"] = "PURCHASE REGISTER";


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

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void ClearAll()
        {
            dgrdDetails.Rows.Clear();
            lblGrossAmt.Text = lblNetAmt.Text = "0.00";
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
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.SaleRegister objSales = new Reporting.SaleRegister();
                        objSales.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objSales);
                        else
                        {
                            objSales.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objSales.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                            objSales.PrintToPrinter(1, false, 0, 0);
                        }

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
            btnPrint.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
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
                    saveFileDialog.FileName = "PurchaseBook_Register";
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
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Purchase Bill";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\PurchaseRegister.pdf";

                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.SaleRegister objRegister = new Reporting.SaleRegister();
                    objRegister.SetDataSource(dt);

                    if (File.Exists(strFileName))
                        File.Delete(strFileName);

                    objRegister.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                    objRegister.Close();
                    objRegister.Dispose();
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

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void PurchaseBookRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bPurchaseReport)
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

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (txtPurchaseParty.Text != "")
                {
                    string strPath = "", strSubject = "", strBody = "";//,strEmailID = dba.GetPartyEmailID(txtSalesParty.Text);
                    //if (strEmailID != "")
                    //{
                    strPath = CreatePDFFile();
                    if (strPath != "")
                    {
                        strSubject = "PURCHASE REPORT FROM " + MainPage.strGRCompanyName;
                        strBody = "We are sending Purchase Register , which is Attached with this mail, Please Find it.";
                        SendingEmailPage objEmail = new SendingEmailPage(true, txtPurchaseParty.Text, "", strSubject, strBody, strPath,"","PURCHASE REPORT");
                        objEmail.ShowDialog();
                    }
                    //}
                }
                else
                {
                    MessageBox.Show("Sorry ! Party Name can't be blank ", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Focus();
                }
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }
    }
}
