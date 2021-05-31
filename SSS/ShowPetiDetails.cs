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
    public partial class ShowPetiDetails : Form
    {
        DataBaseAccess dba;
        public ShowPetiDetails()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }

        private void ShowPetiDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
                else if (e.KeyCode == Keys.Enter)
                    SendKeys.Send("{TAB}");
            }
            catch { }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            chkPackingDate.Checked = chkDate.Checked;
        }

        private void chkPackingDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromPackingDate.ReadOnly = txtToPackingDate.ReadOnly = !chkPackingDate.Checked;
            txtFromPackingDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToPackingDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
            txtFromPackingDate.Text = txtFromDate.Text;
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtFromPackingDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }

        private void txtPetiAgent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PETIAGENT", "SEARCH PETI AGENT", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPetiAgent.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPackingType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PETITYPE", "SEARCH PETI TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPackingType.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
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

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string strTransport = txtTransport.Text;
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

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGo.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkPackingDate.Checked && (txtFromPackingDate.Text.Length != 10 || txtToPackingDate.Text.Length != 10))
                    MessageBox.Show("Sorry ! Please enter packing date range or uncheck on packing date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    GetAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and  (SR.BillDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.BillDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }
            if (chkPackingDate.Checked && txtFromPackingDate.Text.Length == 10 && txtToPackingDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromPackingDate.Text), eDate = dba.ConvertDateInExactFormat(txtToPackingDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and  (SR.PackingDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SR.PackingDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (txtMonth.Text != "")
            {
                // strCameAtOfficeQuery += " and UPPER(DATENAME(MM,SR.BillDate))='" + txtMonth.Text + "' ";
                strQuery += " and UPPER(DATENAME(MM,SR.PackingDate))='" + txtMonth.Text + "' ";
            }

            if (txtPetiAgent.Text != "")
            {
                string[] strFullName = txtPetiAgent.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strQuery += " and SR.Description_1='" + strFullName[0].Trim() + "' ";                  
                }
            }

            if (txtPackingType.Text!="")            
                strQuery += " and SR.Description_2='" + txtPackingType.Text + "' ";

            if (txtBillCode.Text != "")
                strQuery += " and SR.BillCode='" + txtBillCode.Text + "' ";

            if (txtTransport.Text != "")
                strQuery += " and SR.Transport='" + txtTransport.Text + "' ";

            return strQuery;
        }

        private void GetAllData()
        {
            string strQuery = "",  strSubQuery = CreateQuery(), strColumnName = "", strGroupName = "",strMainColumn="",strMainGroup="";

            if (chkAll.Checked)
            {
                strColumnName += "GoodsType,(BillCode + ' ' + CAST(BillNo as varchar))_BillNo,BillNo,CONVERT(date, BillDate, 103)_Date,BillDate,PetiAgent,Description_2,";
                strMainColumn = "BillNo,_BillNo,BillDate,GoodsType,_Date,PetiAgent,Description_2,";
            }
            else
            {
                if (chkPackingType.Checked)
                    strColumnName += "GoodsType,";
                if (chkPetiType.Checked)
                    strColumnName += "Description_2,";
                if (chkPetiAgent.Checked)
                    strColumnName += "PetiAgent,";

                strMainColumn = strColumnName;

                if (chkSaleBillNo.Checked)
                {
                    strColumnName += "(BillCode + ' ' + CAST(BillNo as varchar))_BillNo,BillNo,";
                    strMainColumn = "BillNo,_BillNo," + strMainColumn;
                }
                if (chkBillDate.Checked)
                {
                    strColumnName += "CONVERT(date, BillDate, 103)_Date,BillDate,";
                    strMainColumn = "BillDate,_Date," + strMainColumn;
                }
            }

            strGroupName = strColumnName.Replace("_Date", "").Replace("_BillNo", "");
            if (strGroupName != "")
            {
                strGroupName = "Group by " + strGroupName;
                strGroupName = strGroupName.Substring(0, strGroupName.Length - 1);
            }          

            if (strColumnName == "")
            {
                strColumnName = "'TOTAL' as TOTAL,";
                strMainColumn = "TOTAL,";
            }
            if (strMainColumn != "")
            {
                strMainGroup = "Group by " + strMainColumn;
                strMainGroup = strMainGroup.Substring(0, strMainGroup.Length - 1);
            }

            if (rdoPAll.Checked || rdoPackedAtOffice.Checked)
            {
                strQuery = " Select " + strColumnName.Replace("BillDate", "PackingDate").Replace("_Date,PackingDate", "_Date,PackingDate as BillDate") + " SUM(CAST(OtherField as Money)) PetiCount,SUM(CAST(OtherField as Money)*Freight) FreightAmt,SUM(GreenTaxAmt) as GreenTaxAmt from SalesRecord SR OUTER APPLY (Select (Description_1+' '+ Name)PetiAgent from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=SR.Description_1) SM  OUTER APPLY (Select (CASE WHEN TR.City='DELHI' then 180 else 90 end)Freight  from Transport TR Where TransportName=SR.Transport) _TR Where (GoodsType='PACKED' and CartoneSize!='ATTACHED' and PackedBillNo='')" + strSubQuery  + " " + strGroupName.Replace("BillDate", "PackingDate") + " UNION ALL "
                         + " Select " + strColumnName.Replace("BillDate", "PackingDate").Replace("_Date,PackingDate", "_Date,PackingDate as BillDate") + " SUM(CAST(NoOfCase as Money)) PetiCount,SUM(CAST(NoOfCase as Money)*Freight) FreightAmt,SUM(GreenTax) as GreenTaxAmt from SalesBook SR OUTER APPLY (Select 'PACKED' as GoodsType)SB OUTER APPLY (Select (Description_1+' '+ Name)PetiAgent from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=SR.Description_1) SM OUTER APPLY (Select (CASE WHEN TR.City='DELHI' then 180 else 90 end)Freight  from Transport TR Where TransportName=SR.TransportName) _TR Where BillNo!=0 " + strSubQuery.Replace("BillDate", "Date").Replace("SR.Transport", "SR.TransportName")  + " " + strGroupName.Replace("BillDate", "PackingDate") + " ";

            }
            if (rdoPAll.Checked)
                strQuery += " UNION ALL ";
            if (rdoPAll.Checked || rdoCameAtOffice.Checked)
                strQuery += " Select " + strColumnName.Replace("BillDate", "PackingDate").Replace("_Date,PackingDate", "_Date,PackingDate as BillDate") + "SUM(CAST(OtherField as Money)) PetiCount,SUM(CAST(OtherField as Money)*Freight) FreightAmt,SUM(GreenTaxAmt) as GreenTaxAmt from SalesRecord SR OUTER APPLY (Select (Description_1+' '+ Name)PetiAgent from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=SR.Description_1) SM  OUTER APPLY (Select (CASE WHEN TR.City='DELHI' then 180 else 90 end)Freight  from Transport TR Where TransportName=SR.Transport) _TR Where (GoodsType = 'CAMEOFFICE' and CartoneSize != 'ATTACHED' and PackedBillNo = '') " + strSubQuery + " " + strGroupName.Replace("BillDate", "PackingDate");

            strQuery = "  Select " + strMainColumn + " SUM(PetiCount)PetiCount,SUM(FreightAmt)FreightAmt,SUM(GreenTaxAmt) as GreenTaxAmt from (" + strQuery + ")_Sales Where PetiCount>0 " + strMainGroup + " " + strMainGroup.Replace("Group by ", "Order by ");
            
            DataTable _dt = dba.GetDataTable(strQuery);
            BindColumn(_dt);
            BindDataWithGrid(_dt);

        }

        private void BindDataWithGrid(DataTable _dt)
        {
            double dPetiCount = 0, dCount = 0, dRate = 0, dPackingAmt = 0, dTPackingAmt = 0, dFreightAmt = 0, dTFreightAmt = 0, dGreenTaxAmt = 0, dTGreenTaxAmt = 0 ;
            try
            {
                //if (rdoNet.Checked)
                {
                    if (_dt.Rows.Count > 0)
                        dgrdPurchase.Rows.Add(_dt.Rows.Count);
                    int _rowIndex = 0;
                    string strPetiType = "";
                    
                    foreach (DataRow row in _dt.Rows)
                    {
                        if (chkAll.Checked || chkPetiType.Checked)
                            strPetiType = Convert.ToString(row["Description_2"]);

                        dTFreightAmt += dFreightAmt = dba.ConvertObjectToDouble(row["FreightAmt"]);
                        dTGreenTaxAmt += dGreenTaxAmt = dba.ConvertObjectToDouble(row["GreenTaxAmt"]);

                        dgrdPurchase.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                        if (chkAll.Checked || chkPackingType.Checked)
                            dgrdPurchase.Rows[_rowIndex].Cells["GoodsType"].Value = row["GoodsType"];
                        if (chkAll.Checked || chkSaleBillNo.Checked)
                            dgrdPurchase.Rows[_rowIndex].Cells["_BillNo"].Value = row["_BillNo"];
                        if (chkAll.Checked || chkBillDate.Checked)
                            dgrdPurchase.Rows[_rowIndex].Cells["_Date"].Value = row["_Date"];
                        if (chkAll.Checked || chkPetiAgent.Checked)
                        {
                            if (Convert.ToString(row["PetiAgent"]) == "")
                                dgrdPurchase.Rows[_rowIndex].Cells["PetiAgent"].Value = "DIRECT";
                            else
                            dgrdPurchase.Rows[_rowIndex].Cells["PetiAgent"].Value = row["PetiAgent"];
                        }
                        if (chkAll.Checked || chkPetiType.Checked)
                            dgrdPurchase.Rows[_rowIndex].Cells["Description_2"].Value = strPetiType;

                        dPetiCount += dCount = dba.ConvertObjectToDouble(row["PetiCount"]); 
                        dgrdPurchase.Rows[_rowIndex].Cells["netPeti"].Value = Math.Round(dCount, 2);
                        dgrdPurchase.Rows[_rowIndex].Cells["freightAmt"].Value = Math.Round(dFreightAmt, 2);

                        if (strPetiType == "DOUBLE")
                            dRate = 100;
                        else if (strPetiType == "SINGLE")
                            dRate = 63;
                        else if (strPetiType == "PARCEL" || strPetiType== "20MM PARCEL")
                            dRate = 20;
                        else if (strPetiType == "30MM PARCEL")
                            dRate = 30;
                        else if (strPetiType == "40MM PARCEL")
                            dRate = 40;
                        else if (strPetiType == "DOUBLE/SINGLE")
                        {
                            dRate = 163;
                            dCount = dCount / 2;
                        }
                        else
                            dRate = 0;

                        dTPackingAmt += dPackingAmt = Math.Round(dCount * dRate, 2);
                        dgrdPurchase.Rows[_rowIndex].Cells["packingRate"].Value = Math.Round(dRate, 2);
                        dgrdPurchase.Rows[_rowIndex].Cells["packingAmt"].Value = dPackingAmt;
                        dgrdPurchase.Rows[_rowIndex].Cells["greenTaxAmt"].Value = dGreenTaxAmt;

                        _rowIndex++;
                    }
                }                
               
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            lblTotalPeti.Text = dPetiCount.ToString("N2", MainPage.indianCurancy);
            lblTotalPackingAmt.Text = dTPackingAmt.ToString("N2", MainPage.indianCurancy);
            lblFreight.Text = dTFreightAmt.ToString("N2", MainPage.indianCurancy);
            lblGreenTax.Text = dTGreenTaxAmt.ToString("N2", MainPage.indianCurancy);
        }


        private void BindColumn(DataTable _dt)
        {
            dgrdPurchase.Columns.Clear();

            CreateGridviewColumn("sno", "S.No", "RIGHT", 50);
            if (chkAll.Checked || chkPackingType.Checked)
                CreateGridviewColumn("GoodsType", "Goods Type", "LEFT", 110);
            if (chkAll.Checked || chkSaleBillNo.Checked)
                CreateGridviewColumn("_BillNo", "Sale Bill No", "LEFT", 150);
            if (chkAll.Checked || chkBillDate.Checked)
                CreateGridviewColumn("_Date", "Bill Date", "LEFT", 100);
            if (chkAll.Checked || chkPetiAgent.Checked)
                CreateGridviewColumn("petiAgent", "Peti Agent", "LEFT", 150);
            if (chkAll.Checked || chkPetiType.Checked)
                CreateGridviewColumn("Description_2", "Peti Type", "LEFT", 100);

            //if (rdoNet.Checked)
            //{
            CreateGridviewColumn("netPeti", "Net Peti", "RIGHT", 120);
            CreateGridviewColumn("packingRate", "Rate", "RIGHT", 100);
            CreateGridviewColumn("packingAmt", "Packing Amt", "RIGHT", 120);
            CreateGridviewColumn("freightAmt", "Freight Amt", "RIGHT", 120);
            CreateGridviewColumn("greenTaxAmt", "Green T.Amt", "RIGHT", 120);
            if (!chkAll.Checked && !chkPetiType.Checked)
            {
                dgrdPurchase.Columns["packingRate"].Visible = dgrdPurchase.Columns["packingAmt"].Visible = false;
            }
        
        }

        private void CreateGridviewColumn(string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
                _column.Name = strColName;
                _column.HeaderText = strColHeader;
                _column.Width = _width;
                _column.SortMode = DataGridViewColumnSortMode.Automatic;
                if (strAlign == "LEFT")
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                    if(strColName.Contains("Date"))
                        _column.DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                else
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
                    if (_width != 50)
                        _column.DefaultCellStyle.Format = "N2";
                }
                _column.CellTemplate = dataGridViewCell;
                dgrdPurchase.Columns.Add(_column);
            }
            catch { }
        }

        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonth.Text = objSearch.strSelectedData;
                    dgrdPurchase.Rows.Clear();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            chkPackingType.Checked = chkSaleBillNo.Checked = chkBillDate.Checked = chkPetiAgent.Checked = chkPetiType.Checked = chkAll.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdPurchase_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgrdPurchase.Columns[e.ColumnIndex].Name == "_BillNo")
                {
                    string strInvoiceNo = Convert.ToString(dgrdPurchase.CurrentCell.Value);
                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                }

            }
            catch { }
        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            try
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    dba.ShowSaleBookPrint(strCode, strBillNo, false, false);
                }
                else
                {
                    if (dgrdPurchase.Columns.Contains("_BillNo"))
                    {
                        string strInvoiceNo = Convert.ToString(dgrdPurchase.CurrentRow.Cells["_BillNo"].Value);
                        string[] strNumber = strInvoiceNo.Split(' ');

                        string str = dba.GetSalesRecordType(strNumber[0], strNumber[1]);
                        if (str == "")
                        {
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                dba.ShowSaleBookPrint(strNumber[0], strNumber[1], false, false);
                            }
                            else
                            {
                                SaleBook objSale = new SaleBook(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }
                        else
                        {
                            if (str == "RETAIL")
                            {
                                SaleBook_Retail objSale = new SaleBook_Retail(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                            else
                            {
                                SaleBook_Trading objSale = new SaleBook_Trading(strNumber[0], strNumber[1]);
                                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objSale.ShowInTaskbar = true;
                                objSale.Show();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private string CreateSummaryQuery()
        {
            string strSubQuery = CreateQuery(), strQuery = "" ;

            strQuery = " Select PackingType,SUM(NoofPeti)PetiCount from ( "
                     + " Select Description_2 as PackingType, SUM(CAST(OtherField as Money))NoofPeti from SalesRecord SR Where GoodsType = 'PACKED' and PackedBillNo = '' and Description_2!= '' and CAST(OtherField as money) > 0 "+ strSubQuery+"   Group by Description_2 UNION ALL "
                     + " Select Description_2 as PackingType, SUM(CAST(NoOfCase as Money))NoofPeti from SalesBook SR Where PackedBillNo = '' and Description_2!= '' and CAST(NoOfCase as money) > 0 " + strSubQuery.Replace("BillDate", "Date").Replace("SR.Transport", "SR.TransportName") + "  Group by Description_2 "
                     + " )Sales Group by PackingType Order by PackingType "
                     + " Select AgentName,SUM(NoofPeti)PetiCount,Rate,SUM(NoofPeti * Rate)Freight,SUM(GreenTaxAmt) as GreenTaxAmt from ( "
                     + " Select ISNULL(AgentName, 'DIRECT') AgentName, SUM(CAST(OtherField as Money))NoofPeti, Rate,SUM(GreenTaxAmt) as GreenTaxAmt from SalesRecord SR OUTER APPLY(Select(CASE WHEN TR.City LIKE('%DELHI%') THEN 180 else 90 end) Rate from Transport TR Where TR.TransportName = SR.Transport)TR OUTER APPLY(Select(Description_1 + ' ' + Name)AgentName from SupplierMaster Where AreaCode + AccountNo = Description_1)SM Where GoodsType in ('PACKED', 'CAMEOFFICE') and PackedBillNo = '' and Description_1!= '' and CAST(OtherField as money) > 0 " + strSubQuery + "  Group by AgentName, Rate UNION ALL "
                     + " Select ISNULL(AgentName, 'DIRECT') AgentName, SUM(CAST(NoOfCase as Money))NoofPeti, Rate,SUM(GreenTax) as GreenTaxAmt from SalesBook SR OUTER APPLY(Select(CASE WHEN TR.City LIKE('%DELHI%') THEN 180 else 90 end) Rate from Transport TR Where TR.TransportName = SR.TransportName)TR OUTER APPLY(Select(Description_1 + ' ' + Name)AgentName from SupplierMaster Where AreaCode + AccountNo = Description_1)SM Where PackedBillNo = '' and Description_1 != '' and CAST(NoOfCase as money) > 0 " + strSubQuery.Replace("BillDate", "Date").Replace("SR.Transport", "SR.TransportName") + "  Group by AgentName, Rate )Sales Group by AgentName, Rate Order by AgentName,Rate ";

            return strQuery;
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CompanyName", typeof(String));
            dt.Columns.Add("HeaderName", typeof(String));
            dt.Columns.Add("DatePeriod", typeof(String));
            dt.Columns.Add("DoublePackingAmt", typeof(String));
            dt.Columns.Add("SinglePackingAmt", typeof(String));
            dt.Columns.Add("DoubleSingleAmt", typeof(String));
            dt.Columns.Add("ParcelAmt", typeof(String));
            dt.Columns.Add("TotalPackingAmt", typeof(String));
            dt.Columns.Add("PetiAgent", typeof(String));
            dt.Columns.Add("SNo", typeof(String));
            dt.Columns.Add("Rate", typeof(String));
            dt.Columns.Add("Location", typeof(String));
            dt.Columns.Add("FreightAmt", typeof(String));
            dt.Columns.Add("TotalAmt", typeof(String));
            dt.Columns.Add("PrintedBy", typeof(String)); 
            dt.Columns.Add("PetiCount", typeof(String));
            dt.Columns.Add("GreenTaxAmt", typeof(String)); 
            dt.Columns.Add("TotalGreenTaxAmt", typeof(String));
            return dt;
        }

        private DataTable GetDataTableForSummary()
        {
            DataTable dt = CreateDataTable();
            try
            {
                string strQuery = CreateSummaryQuery();
                DataSet _ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (_ds.Tables.Count > 0)
                {
                    DataTable _dtPacking = _ds.Tables[0], _dtFreight = _ds.Tables[1];
                    int _index = 1;
                    double dRate = 0, dFreight = 0, dTotalAmt = 0, dGreenTaxAmt = 0,dTGreenTaxAmt=0 ;
                    string strAgentName = "";
                    foreach (DataRow row in _dtFreight.Rows)
                    {
                        DataRow _row = dt.NewRow();
                                               
                        dRate = dba.ConvertObjectToDouble(row["Rate"]);
                        dFreight = dba.ConvertObjectToDouble(row["Freight"]);
                        dGreenTaxAmt = dba.ConvertObjectToDouble(row["GreenTaxAmt"]);
                        dTotalAmt = GetTotalFreightAmt(_dtFreight, row["AgentName"],ref dTGreenTaxAmt);
                        if (strAgentName != Convert.ToString(row["AgentName"]))
                        {
                            _index = 1;
                            strAgentName = Convert.ToString(row["AgentName"]);
                        }
                        else
                            _index++;

                        _row["SNo"] = _index + ".";
                        _row["Rate"] = dRate.ToString("N2", MainPage.indianCurancy);

                        if (dRate == 90)
                            _row["Location"] = "LOCAL";
                        else
                            _row["Location"] = "OUT SIDER";
                        _row["PetiAgent"] = row["AgentName"];
                        _row["PetiCount"] = dba.ConvertObjectToDouble(row["PetiCount"]).ToString("N0", MainPage.indianCurancy);
                        _row["FreightAmt"] = dFreight.ToString("N2", MainPage.indianCurancy);
                        _row["TotalAmt"] = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                        _row["GreenTaxAmt"] = dGreenTaxAmt.ToString("N2", MainPage.indianCurancy);
                        _row["TotalGreenTaxAmt"] = dTGreenTaxAmt.ToString("N2", MainPage.indianCurancy);

                        dt.Rows.Add(_row);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        DataRow _row = dt.NewRow();
                        dt.Rows.Add(_row);
                    }
                    string strPackingType = "";
                    double dDouble = 0, dSingle = 0, dParcel = 0, dDS = 0, dTotalPAmt = 0,d30MMParcel=0,d40MMParcel=0;
                    foreach (DataRow row in _dtPacking.Rows)
                    {
                        strPackingType = Convert.ToString(row["PackingType"]);
                        if (strPackingType == "DOUBLE")
                            dDouble += dba.ConvertObjectToDouble(row["PetiCount"]);
                        else if (strPackingType == "SINGLE")
                            dSingle += dba.ConvertObjectToDouble(row["PetiCount"]);
                        else if (strPackingType == "PARCEL" || strPackingType== "20MM PARCEL")
                            dParcel += dba.ConvertObjectToDouble(row["PetiCount"]);
                        else if (strPackingType == "30MM PARCEL")
                            d30MMParcel += dba.ConvertObjectToDouble(row["PetiCount"]);
                        else if (strPackingType == "40MM PARCEL")
                            d40MMParcel += dba.ConvertObjectToDouble(row["PetiCount"]);
                        else if (strPackingType == "DOUBLE/SINGLE")
                        {
                            dDS = dba.ConvertObjectToDouble(row["PetiCount"]) / 2;
                            dDouble += dDS;
                            dSingle += dDS;
                        }
                    }

                    DataRow __row = dt.Rows[0];
                    __row["CompanyName"] = MainPage.strPrintComapanyName;
                    __row["HeaderName"] = "PETI SUMMARY";

                    string strDatePeriod = "";
                    if (chkPackingDate.Checked && txtFromPackingDate.Text.Length == 10 && txtToPackingDate.Text.Length == 10)
                        strDatePeriod = "Date Period : " + txtFromPackingDate.Text + " To " + txtToPackingDate.Text;

                    if (txtMonth.Text != "")
                    {
                        if (strDatePeriod != "")
                            strDatePeriod += " in the ";
                        strDatePeriod += "Month of : " + txtMonth.Text;
                    }

                    double dParcelAmt = (dParcel * 20) + (d30MMParcel * 30) + (d40MMParcel * 40);

                    dTotalPAmt = (dDouble * 100) + (dSingle * 63) + dParcelAmt;
                    __row["DatePeriod"] = strDatePeriod;
                    __row["DoublePackingAmt"] = "("+dDouble+"X100) "+ (dDouble * 100).ToString("N2", MainPage.indianCurancy);
                    __row["SinglePackingAmt"] = "(" + dSingle + "X63) " + (dSingle * 63).ToString("N2", MainPage.indianCurancy);
                    __row["ParcelAmt"] = "(" + dParcel + "X20/30/40) " + dParcelAmt.ToString("N2", MainPage.indianCurancy);
                    __row["TotalPackingAmt"] = dTotalPAmt.ToString("N2", MainPage.indianCurancy);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        private double GetTotalFreightAmt(DataTable dt, object objAgent, ref double dGreenTax)
        {
            object obj = dt.Compute("SUM(Freight)", "AgentName='" + objAgent + "' ");
            object objGreen = dt.Compute("SUM(GreenTaxAmt)", "AgentName='" + objAgent + "' ");
            dGreenTax = dba.ConvertObjectToDouble(objGreen);
            return dba.ConvertObjectToDouble(obj);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                DataTable dt = GetDataTableForSummary();
                if (dt.Rows.Count > 0)
                {                   
                    Reporting.PetiSummaryReport objReport = new Reporting.PetiSummaryReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("PETI SUMMARY");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.ShowDialog();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdPurchase.Rows.Count > 0)
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
                    for (int j = 1; j < dgrdPurchase.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdPurchase.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdPurchase.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdPurchase.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdPurchase.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdPurchase.Columns.Count; l++)
                        {
                            if (dgrdPurchase.Columns[l].HeaderText == "" || !dgrdPurchase.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdPurchase.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdPurchase.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Show_Peti_Details";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
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

        private void ShowPetiDetails_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
