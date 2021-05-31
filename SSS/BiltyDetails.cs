using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Excel;

namespace SSS
{
    public partial class BiltyDetails : Form
    {
        DataBaseAccess dba;
        bool _EInvoice = false;
        public BiltyDetails()
        {
            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();
                txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
                if (!MainPage.mymainObject.bSaleEdit)
                    btnGenerateJSON.Enabled = false;
            }
            catch { }
        }

        private void BiltyDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
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
            }
            catch
            {
            }
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    char objChar = Convert.ToChar(e.KeyCode);
            //    int value = e.KeyValue;
            //    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            //    {
            //        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
            //        objSearch.ShowDialog();
            //        txtTransport.Text = objSearch.strSelectedData;
            //    }
            //    else
            //        e.Handled = true;
            //}
            //catch
            //{
            //}
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void btnGo_Click(object sender, EventArgs e)
        {

            try
            {
                btnGo.Enabled = chkAll.Checked = false;
                dgrdBilty.Rows.Clear();
                lblBill.Text = "0";
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please uncheck date or enter valid date !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else
                    GetAllData();
            }
            catch
            {
            }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (txtSalesParty.Text != "")
            {
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strQuery += " and SalePartyID='" + strFullName[0] + "' ";
                }
            }
            //if (txtTransport.Text != "")
            //    strQuery += " and Transport='" + txtTransport.Text + "' ";
            if (txtBillCode.Text != "")
                strQuery += " and BillCode='" + txtBillCode.Text + "' ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strQuery += " and BillDate>='" + sDate.ToString("MM/dd/yyyy") + "' and BillDate<'" + eDate.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (chkSerial.Checked && txtFromSerialNo.Text != "" && txtToSerialNo.Text != "")
            {
                strQuery += " and ((BillNo >=" + txtFromSerialNo.Text + " and BillNo<=" + txtToSerialNo.Text + ") OR (BillCode+' '+CAST(BillNo as varchar)) in (Select (SRD.BillCode+' '+CAST(SRD.BillNo as varchar)) from SalesRecord SRD Where CAST(REPLACE(SRD.PackedBillNo,SRD.BillCode+' ','') as Money)>=" + txtToSerialNo.Text + " and CAST(REPLACE(SRD.PackedBillNo,SRD.BillCode+' ','') as Money)<=" + txtFromSerialNo.Text + ")) ";
            }

            if (txtBillNo.Text != "")            
                strQuery += " and (BillNo in (" + txtBillNo.Text + ") OR (BillCode+' '+CAST(BillNo as varchar)) in (Select (SRD.BillCode+' '+CAST(SRD.BillNo as varchar)) from SalesRecord SRD Where REPLACE(SRD.PackedBillNo,SRD.BillCode+' ','') in (" + txtBillNo.Text + ")) ) ";
            
            if (rdoWithLR.Checked)
                strQuery += " and SR.LrNumber !='' ";
            else if (rdoWithoutLR.Checked)
                strQuery += " and SR.LrNumber ='' ";

            if (rdoWithWayBill.Checked)
                strQuery += " and SR.WayBillNo !='' ";
            else if (rdoWithoutWayBillNo.Checked)
                strQuery += " and SR.WayBillNo ='' ";

            if (rdoWithIRN.Checked)
                strQuery += " and SR.IRNNo !='' ";
            else if (rdoWithoutIRN.Checked)
                strQuery += " and SR.IRNNo ='' ";

            return strQuery;
        }

        private void GetAllData()
        {
            string strQuery = "", strSubQuery = CreateQuery();
            strQuery = " Select CONVERT(varchar,BillDate,103) BDate,(SR.BillCode+' '+CAST(SR.BillNo as varchar)) BillNo,(SR.SalePartyID+' '+SM.Name) PartyName,Transport,Station,LrNumber,Convert(varchar,LRDate,103) LDate,WayBillNo,WayBillDate,AttachedBill,Description,GRSNO,SR.GoodsType,SupplierName,SR.PackedBillNo,SR.IRNNo from SalesRecord SR Outer Apply (Select Name from SupplierMaster SM Where (AreaCode+AccountNo)=SR.SalePartyID) SM Outer Apply (Select (BillCode+CAST(BillNo as varchar)) _BillNo from SalesEntry SE Where SE.BillCode=SR.BillCode and SE.BillNo=SR.BillNo Group by (BillCode+CAST(BillNo as varchar)) having(Count(*)=1)) SE1  OUTER APPLY (Select TOP 1 SE.GRSNo from SalesEntry SE Where (SE.BillCode+CAST(SE.BillNo as varchar))=SE1._BillNo  and SR.GoodsType='DIRECT') SE OUTER APPLY (Select TOP 1 SupplierName from SalesEntry SE Where SE.BillCode=SR.BillCode and SR.BillNo=SE.BillNo) SE2 Where SR.BillNo!=0 " + strSubQuery+ " Order By SR.BillNo desc";

            DataTable dt = dba.GetDataTable(strQuery);
            BindDataWithControl(dt);
        }

        private void BindDataWithControl(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                dgrdBilty.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdBilty.Rows[rowIndex].Cells["chk"].Value = false;
                    dgrdBilty.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                    dgrdBilty.Rows[rowIndex].Cells["billNo"].Value = row["BillNo"];
                    dgrdBilty.Rows[rowIndex].Cells["partyName"].Value = row["PartyName"];
                    dgrdBilty.Rows[rowIndex].Cells["transportName"].Value = row["Transport"];
                    dgrdBilty.Rows[rowIndex].Cells["stationName"].Value = row["Station"];
                    dgrdBilty.Rows[rowIndex].Cells["oldLRNumber"].Value =  dgrdBilty.Rows[rowIndex].Cells["lrNumber"].Value = row["LrNumber"];
                    dgrdBilty.Rows[rowIndex].Cells["lrDate"].Value = row["LDate"]; 
                    dgrdBilty.Rows[rowIndex].Cells["waybillNo"].Value = row["WayBillNo"];
                    dgrdBilty.Rows[rowIndex].Cells["waybillDate"].Value = row["WayBillDate"];
                    dgrdBilty.Rows[rowIndex].Cells["attachedBill"].Value = row["AttachedBill"];
                    dgrdBilty.Rows[rowIndex].Cells["purchaseSNo"].Value = row["GRSNO"]; 
                    dgrdBilty.Rows[rowIndex].Cells["goodsType"].Value = row["GoodsType"];
                    dgrdBilty.Rows[rowIndex].Cells["description"].Value = row["Description"];
                    dgrdBilty.Rows[rowIndex].Cells["supplierName"].Value = row["SupplierName"];
                    dgrdBilty.Rows[rowIndex].Cells["IRNNO"].Value = row["IRNNO"];
                    rowIndex++;
                }
            }
            lblBill.Text = dt.Rows.Count.ToString();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllRow();
        }

        private void CheckAllRow()
        {
            foreach (DataGridViewRow row in dgrdBilty.Rows)
                row.Cells["chk"].Value = chkAll.Checked;
        }

        private void dgrdBilty_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {

                if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 11)
                    e.Cancel = true;
                else
                {
                    if (e.ColumnIndex == 4)
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdBilty.CurrentCell.Value = objSearch.strSelectedData;
                        if (objSearch.strSelectedData != "")
                            UpdateRecord(4);
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 5)
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdBilty.CurrentCell.Value = objSearch.strSelectedData;
                        if (objSearch.strSelectedData != "")
                            UpdateRecord(5);
                        e.Cancel = true;
                    }
                }

            }
            catch
            {
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //UpdateRecord();
        }

        //private void UpdateRecord()
        //{
        //    try
        //    {
        //        dgrdBilty.EndEdit();
        //        DialogResult result = MessageBox.Show("Are you sure you want to Update record ? ", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (result == DialogResult.Yes)
        //        {
        //            string strQuery = "";

        //            foreach (DataGridViewRow row in dgrdBilty.Rows)
        //            {
        //                if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
        //                {
        //                    string strBillNo = Convert.ToString(row.Cells["billNo"].Value);
        //                    if (strBillNo!="")
        //                    {
        //                        strQuery += " Update BiltyDetail Set TransportName='" + row.Cells["transportName"].Value + "',StationName='" + row.Cells["stationName"].Value + "',LrNo='" + row.Cells["lrNumber"].Value + "',LrDate=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())), PvtMarka='" + row.Cells["pvtMarka"].Value + "',UpdateStatus=1 Where (BillCode+' '+ CAST(BillNo as varchar))='" + strBillNo + "' ";
        //                    }
        //                }
        //            }

        //            if (strQuery != "")
        //            {
        //                int count = DataBaseAccess.ExecuteMyNonQuery(strQuery);
        //                if (count > 0)
        //                {
        //                    MessageBox.Show("Thank you ! Record updated successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //                    chkAll.Checked = false;
        //                    CheckAllRow();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Sorry ! Unable to update records .. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("Worning : Please select atleast one bill for updation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    DeleteRecord();
        //}

        //private void DeleteRecord()
        //{
        //    try
        //    {
        //        dgrdBilty.EndEdit();
        //        DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (result == DialogResult.Yes)
        //        {
        //            string strQuery = "";

        //            foreach (DataGridViewRow row in dgrdBilty.Rows)
        //            {
        //                if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
        //                {
        //                    string strBillNo = Convert.ToString(row.Cells["billNo"].Value);
        //                    if (strBillNo != "")
        //                    {
        //                        strQuery += " Delete from BiltyDetail  Where (BillCode+' '+ CAST(BillNo as varchar))='" + strBillNo + "' ";
        //                    }
        //                }
        //            }

        //            if (strQuery != "")
        //            {
        //                int count = DataBaseAccess.ExecuteMyNonQuery(strQuery);
        //                if (count > 0)
        //                {
        //                    MessageBox.Show("Thank you ! Record delete successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //                    GetAllData();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Sorry ! Unable to delete records .. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("Worning : Please select atleast one bill for deletion ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdBilty_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdBilty.CurrentCell.ColumnIndex == 6 || dgrdBilty.CurrentCell.ColumnIndex == 7 || dgrdBilty.CurrentCell.ColumnIndex == 8)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);

                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdBilty.CurrentCell.ColumnIndex == 6 || dgrdBilty.CurrentCell.ColumnIndex == 8)
                dba.ValidateSpace(sender, e);
            else if (dgrdBilty.CurrentCell.ColumnIndex == 7)
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void dgrdBilty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdBilty.CurrentRow.Index;
                    if (dgrdBilty.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdBilty.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdBilty.CurrentRow.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                    }
                }
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

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSerialNo.ReadOnly = txtToSerialNo.ReadOnly = !chkSerial.Checked;
            txtFromSerialNo.Text = txtToSerialNo.Text = "";
        }

        private void txtFromSerialNo_Leave(object sender, EventArgs e)
        {
            if (txtToSerialNo.Text == "" && txtFromSerialNo.Text != "")
                txtToSerialNo.Text = txtFromSerialNo.Text;
        }

        private void dgrdBilty_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7)
                {
                    string strDate = Convert.ToString(dgrdBilty.CurrentCell.EditedFormattedValue);
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
                                if (e.RowIndex < dgrdBilty.Rows.Count - 1)
                                {
                                    dgrdBilty.EndEdit();
                                }
                            }
                            dgrdBilty.CurrentCell.Value = txtDate.Text;

                            UpdateRecord(7);
                        }
                        else
                        {
                            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                }
                if (e.ColumnIndex == 6 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10)
                    UpdateRecord(e.ColumnIndex);
            }
            catch { }
        }

        private void dgrdBilty_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7)
                {
                    string strDate = Convert.ToString(dgrdBilty.CurrentCell.EditedFormattedValue);
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
                                if (e.RowIndex < dgrdBilty.Rows.Count - 1)
                                {
                                    dgrdBilty.EndEdit();
                                }
                            }
                            dgrdBilty.CurrentCell.Value = txtDate.Text;
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

        private void UpdateRecord(int _uType)
        {
            try
            {
                string strLRDate = "";
               DataGridViewRow row = dgrdBilty.CurrentRow;
                if (row != null)
                {
                    string strBillNo = Convert.ToString(row.Cells["billNo"].Value), strSubQuery = "", strAttachedBill = Convert.ToString(row.Cells["attachedBill"].Value), strLRNumber = "", strOLDLRNumber = "",strBiltyPath = "";
                    string[] strInvoice = strBillNo.Split(' ');
                    if (_uType == 4)
                    {
                        strSubQuery = "Transport='" + row.Cells["transportName"].Value + "' ";
                    }
                    else if (_uType == 5)
                    {
                        strSubQuery = "Station='" + row.Cells["stationName"].Value + "' ";
                    }
                    else if (_uType == 6)
                    {
                        strLRNumber = Convert.ToString(row.Cells["lrNumber"].Value);
                        strOLDLRNumber = Convert.ToString(row.Cells["oldLRNumber"].Value);                       
                        if (strOLDLRNumber != strLRNumber && !strLRNumber.Contains("PKD") && !strLRNumber.Contains("HAND") && ! strLRNumber.Contains("BUS") && !strLRNumber.Contains("MISS") && strLRNumber != "")
                        {
                            strBiltyPath = DataBaseAccess.GetBiltyPDFFiles(strInvoice[0], strInvoice[1]);
                            if (strBiltyPath == "" && !MainPage.strUserRole.Contains("SUPERADMIN"))
                                return;
                        }

                        strSubQuery = "LrNumber='" + strLRNumber + "' ";
                        if (strLRNumber != "")
                            strSubQuery += ",BillStatus='SHIPPED' ";
                        //else
                          //  strSubQuery += ",BillStatus='BILLED' ";
                    }
                    else if (_uType == 7)
                    {
                         strLRDate = Convert.ToString(row.Cells["lrDate"].Value);
                        if (strLRDate .Length==10)
                            strLRDate = "'" + dba.ConvertDateInExactFormat(strLRDate).ToString("MM/dd/yyyy") + "' ";
                        else
                            strLRDate = "NULL";

                        strSubQuery = "LrDate=" + strLRDate + " ";
                    }
                    else if (_uType == 8)
                    {
                        strSubQuery = "WayBillNo='" + row.Cells["waybillNo"].Value + "' ";
                    }
                    else if (_uType == 9)
                    {
                        strSubQuery = "WayBillDate='" + row.Cells["waybillDate"].Value + "' ";
                    }
                    else if (_uType == 10)
                    {
                        strSubQuery = "Description='" + row.Cells["description"].Value + "' ";
                    }

                    if (strSubQuery != "" && strBillNo != "")
                    {
                        if (strAttachedBill != "" && _uType != 8 && _uType != 9 && _uType != 10)
                        {
                            strAttachedBill = " OR BillNo in (" + strAttachedBill + ") ";
                        }

                        string strQuery = " Update SalesRecord Set " + strSubQuery + ",UpdateStatus=1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+' '+CAST(BillNo as varchar)) in ('" + strBillNo + "') " + strAttachedBill
                                        + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType='SALES' and (BillCode+' '+CAST(BillNo as varchar)) in ('" + strBillNo + "') " + strAttachedBill
                                        + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                        + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'UPDATION' as [EditStatus] from SalesRecord  Where (BillCode+' '+CAST(BillNo as varchar)) in ('" + strBillNo + "') " + strAttachedBill;


                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count <= 0)
                        {
                            MessageBox.Show("Sorry ! unable to updated right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (strAttachedBill != "")
                                GetAllData();
                        }
                        else
                        {
                            if (_uType == 6 && strBiltyPath != "")
                            {
                                strLRDate = Convert.ToString(row.Cells["lrDate"].Value);
                                row.Cells["oldLRNumber"].Value = strLRNumber;
                                SendEmailBiltyToSalesParty(strBiltyPath,strInvoice,strLRNumber,strLRDate);
                            }
                            ShowSaleORPurchase(_uType);
                        }
                    }
                }
            }
            catch { }
        }

        private void ShowSaleORPurchase(int _index)
        {
            try
            {
                if (_index == 6 || _index == 7)
                {
                    string strLRNumber = Convert.ToString(dgrdBilty.CurrentRow.Cells["lrNumber"].Value), strLRDate = Convert.ToString(dgrdBilty.CurrentRow.Cells["lrDate"].Value), strGRSNo = Convert.ToString(dgrdBilty.CurrentRow.Cells["purchaseSNo"].Value), strGoodsType = Convert.ToString(dgrdBilty.CurrentRow.Cells["goodsType"].Value);
                    if (strLRNumber != "" && strLRDate != "")
                    {
                        if (strGoodsType == "DIRECT")
                        {
                            if (strGRSNo != "")
                            {
                                DialogResult result = MessageBox.Show("Are you want to print purchase slip ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    string[] strNumber = strGRSNo.Split(' ');
                                    if (strNumber.Length > 1)
                                    {
                                        GoodscumPurchase objPurchase = new GoodscumPurchase(strNumber[0], strNumber[1]);
                                        objPurchase.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                        objPurchase.ShowInTaskbar = true;
                                        objPurchase.Show();
                                    }

                                }
                            }
                            else
                            {
                                string strSaleBill = Convert.ToString(dgrdBilty.CurrentRow.Cells["billNo"].Value);
                                if (strSaleBill != "")
                                {
                                    DialogResult result = MessageBox.Show("Are you want to open sale book ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (result == DialogResult.Yes)
                                    {
                                        string[] strNumber = strSaleBill.Split(' ');
                                        if (strNumber.Length > 1)
                                        {
                                            ShowSaleBook(strNumber[0], strNumber[1]);
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            string strSaleBill = Convert.ToString(dgrdBilty.CurrentRow.Cells["billNo"].Value);
                            if (strSaleBill != "")
                            {
                                DialogResult result = MessageBox.Show("Are you want to open sale book ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    string[] strNumber = strSaleBill.Split(' ');
                                    if (strNumber.Length > 1)
                                    {
                                        ShowSaleBook(strNumber[0], strNumber[1]);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void dgrdBilty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    string strInvoiceNo = Convert.ToString(dgrdBilty.CurrentCell.Value);
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
            if (Control.ModifierKeys == Keys.Control)
            {
                dba.ShowSaleBookPrint(strCode, strBillNo,false, false);
            }
            else
            {
                SaleBook objSale = new SaleBook(strCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.Show();
            }
        }

        private string GetSelectedSaleBillNo()
        {
            string strSaleBillNo = "";
            foreach (DataGridViewRow row in dgrdBilty.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chk"].Value))
                {
                    if (Convert.ToString(row.Cells["transportName"].Value) != "" && Convert.ToString(row.Cells["transportName"].Value) != "PARTY BY HAND")
                    {
                        if (strSaleBillNo != "")
                            strSaleBillNo += ",";
                        strSaleBillNo += "'" + row.Cells["billNo"].Value + "'";
                    }
                    else
                        MessageBox.Show("Warning !! Please add transport in sale bill no : " + row.Cells["billNo"].Value + " !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return strSaleBillNo;
        }

        private void btnGenerateJSON_Click(object sender, EventArgs e)
        {
            try
            {
                btnGenerateJSON.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want generate JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    GenerateJSON();
            }
            catch { }
            btnGenerateJSON.Enabled = true;
        }

        private void GenerateJSON()
        {
            string strSaleBillNo = GetSelectedSaleBillNo();
            if (strSaleBillNo != "")
            {
                var _success=  dba.GenerateEWayBillJSON(strSaleBillNo);
                if(_success)
                {
                    DialogResult result = MessageBox.Show("Are you want to open eway bill site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://ewaybillgst.gov.in/BillGeneration/BulkUploadEwayBill.aspx");
                }
            }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("Show Bilty Details");                  

                    Reporting.Bilty_Details objReport = new Reporting.Bilty_Details();
                    objReport.SetDataSource(dt);
                    objShowReport.myPreview.ReportSource = objReport;
                    objShowReport.Show();

                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();

            try
            {              
                table.Columns.Add("HeaderName", typeof(String));
                table.Columns.Add("DatePeriod", typeof(String));
                table.Columns.Add("SNo", typeof(String));
                table.Columns.Add("BillNo", typeof(String));
                table.Columns.Add("BillDate", typeof(String));
                table.Columns.Add("SalesParty", typeof(String));
                table.Columns.Add("SubParty", typeof(String));
                table.Columns.Add("SupplierName", typeof(String));
                table.Columns.Add("LRNumber", typeof(String));
                table.Columns.Add("PrintedBy", typeof(String));
        
                int _index = 1;
                DataTable _dt = GetDataTableForPrint();
                foreach (DataRow row in _dt.Rows)
                {
                    DataRow dr = table.NewRow();
                    dr["HeaderName"] = "BILTY DETAILS";
                    dr["SNo"] = _index+".";
                    dr["BillNo"] = row["_BillNo"];
                    dr["BillDate"] = row["BDate"];
                    dr["SalesParty"] = row["PartyName"];
                    dr["SubParty"] = row["GRSNO"];
                    dr["SupplierName"] = row["SupplierName"];
                    dr["LRNumber"] = row["LrNumber"];

                    dr["PrintedBy"] = "PRINTED BY : " + MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    table.Rows.Add(dr);
                    _index++;
                }
            }
            catch
            {
            }

            return table;
        }

        private DataTable GetDataTableForPrint()
        {
            string strQuery = "", strSubQuery = CreateQuery(), strSaleBillNo = GetSelectedSaleBillNoForPrint();
            DataTable _dt = new DataTable();
            if (strSaleBillNo != "")
            {
                strQuery = " Select  Distinct CONVERT(varchar,BillDate,5) BDate,(SR.BillCode+' '+CAST(SR.BillNo as varchar)) _BillNo,(SR.SalePartyID+' '+SalesParty) PartyName,'' as GRSNO,SupplierName,LrNumber,SR.BillNo from SalesRecord SR  Outer Apply (Select GRSNO,(PurchasePartyID+' '+ SupplierName) as  SupplierName from SalesEntry SE Where SE.BillCode=SR.BillCode and SE.BillNo=SR.BillNo) SE Where (SR.BillCode+' '+CAST(SR.BillNo as varchar)) in (" + strSaleBillNo + ") " + strSubQuery + " Order by SR.BillNo ";
                _dt = dba.GetDataTable(strQuery);
            }
            else
            {
                MessageBox.Show("Sorry ! Please select atleast one bill for printing !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return _dt;
        }

        private string GetSelectedSaleBillNoForPrint()
        {
            string strSaleBillNo = "";
            foreach (DataGridViewRow row in dgrdBilty.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chk"].Value))
                {
                    if (strSaleBillNo != "")
                        strSaleBillNo += ",";
                    strSaleBillNo += "'" + row.Cells["billNo"].Value + "'";
                }
            }
            return strSaleBillNo;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {              
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                if (dt.Rows.Count > 0)
                {
                    Reporting.Bilty_Details objReport = new Reporting.Bilty_Details();
                    objReport.SetDataSource(dt);
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(objReport);
                    else
                    {
                        objReport.PrintToPrinter(1, false, 0, 0);
                    }
                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPrint.Enabled = true;
        }

        private void SendEmailBiltyToSalesParty(string strFilePath,string[] strInvoice,string strLRNumber,string strLRDate)
        {
            try
            {
                if (strFilePath != "")
                {
                    string strEmailID = "", strWhatsAppNo = "", strMobileNo="",strPartyName=Convert.ToString(dgrdBilty.CurrentRow.Cells["partyName"].Value);
                    
                    string[] strParty = strPartyName.Split(' ');
                    if (strParty.Length > 1)
                    {
                        string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "' and GroupName='SUNDRY DEBTORS'  ";
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                            strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                            strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                            if (strEmailID != "")
                                SendMail(strEmailID, strFilePath, strPartyName,strInvoice);

                            if (strWhatsAppNo != "")
                            {
                                SendBiltyWhatsappMessage(strWhatsAppNo, strFilePath,strPartyName, strInvoice, strLRNumber,strLRDate);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SendMail(string strEmail, string strpath, string strPartyName, string[] strInvoice)
        {
            try
            {
                string strMessage = "", strSub = "";

                strMessage = "Update ! A/c : " + strPartyName + ", we have update your sale bill no : <b>" + strInvoice[0] + " " + strInvoice[1] + " </b>, and bilty scan copy attached with this mail, please find it.";
                strSub = "Alert ! Sale bill no :  " + strInvoice[0] + " " + strInvoice[1] + " updated.";

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "SALE BILL", true);
            }
            catch
            {
            }
        }


        private void SendBiltyWhatsappMessage(string strMobileNo, string strPath,string strPartyName,string[] strInvoice,string strLRNumber,string strLRDate)
        {
            string _strFileName = "Bilty_" + strInvoice[0].Replace("18 -19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + strInvoice[1] + ".pdf", strMessage = "", strBranchCode = strInvoice[0];
            string strFilePath = MainPage.strHttpPath + "/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(strPartyName);
            string strWhastappMessage = "", strMsgType = "";

            dba.DeleteSaleBillFile(strPath, strBranchCode);

            strMsgType = "bilty_update";
            strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + strInvoice[0] + " " + strInvoice[1] + "\",\"variable3\": \"" + strLRNumber + "\",\"variable4\": \"" + strLRDate + "\",";

            bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            if (!_bStatus)
            {
                DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (_updateResult == DialogResult.Retry)
                    _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            }

            if (_bStatus)
            {
                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                strMsgType = "bilty_copy";
                strWhastappMessage = "{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + strFilePath + "\"}";
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", "");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                    txtFilePath.Text = _browser.FileName;
            }
            catch
            {
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            btnImportExcel.Enabled = false;
            try
            {
                DialogResult reuslt = MessageBox.Show("Are you sure you want to import details !!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == reuslt)
                {
                    DataSet ds = GetDataFromExcel();
                    if (ds != null)
                    {
                        DataTable _dt = ds.Tables[0];                        
                        UpdateWayBillRecord(_dt);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnImportExcel.Enabled = true;
        }

        private void UpdateWayBillRecord(DataTable _dt)
        {
            string strQuery = "", stBillNo = "", strWayBillNo = "", strAllSaleBillNo = "",strIRNNo="", strQRCode="",strACKNo;
            decimal _wayBillNo = 0;
            DateTime _date = DateTime.Now;
            bool _bIRN = false;
            if (_dt.Columns.Contains("IRN"))
                _bIRN = true;
            foreach (DataRow row in _dt.Rows)
            {
                if (strAllSaleBillNo != "")
                    strAllSaleBillNo += ",";               
                if (_bIRN)
                {
                    stBillNo = Convert.ToString(row[4]);
                    strIRNNo = Convert.ToString(row[1]);
                    strACKNo = Convert.ToString(row[2]);
                    strWayBillNo = Convert.ToString(row[11]);
                    strQRCode = Convert.ToString(row[10]);
                    if (strWayBillNo != "" && strWayBillNo.Length < 18)
                    {
                        strAllSaleBillNo += "'" + stBillNo + "'";
                        if (stBillNo != "" && strWayBillNo != "")
                        {
                            _wayBillNo = Decimal.Parse(strWayBillNo, System.Globalization.NumberStyles.Float);
                            if (ConvertDateTime(ref _date, Convert.ToString(row[3])))
                            {
                                strQuery += " Update SalesRecord Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',[BillStatus]='SHIPPED',UpdateStatus =1,[IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                         + " Update SalesBook Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',UpdateStatus =1,[IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                         + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType='SALES' and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                         + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                         + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesRecord  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALESERVICE' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SaleServiceBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";
                            }
                        }
                    }
                    else
                    {
                        strQuery += " Update SalesRecord Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                          + " Update SalesBook Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                          + " Update SaleServiceBook Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                          + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType in ('SALES','SALESERVICE') and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                          + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                          + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesRecord  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALESERVICE' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SaleServiceBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";

                    }
                }
                else
                {
                    stBillNo = Convert.ToString(row[2]);
                    strWayBillNo = Convert.ToString(row[8]);

                    strAllSaleBillNo += "'" + stBillNo + "'";
                    if (stBillNo != "" && strWayBillNo != "")
                    {
                        _wayBillNo = Decimal.Parse(strWayBillNo, System.Globalization.NumberStyles.Float);
                        if (ConvertDateTime(ref _date, Convert.ToString(row[9])))
                        {
                            strQuery += " Update SalesRecord Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',[BillStatus]='SHIPPED',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update SalesBook Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType='SALES' and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                     + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'WAYBILL_UPDATED' as [EditStatus] from SalesRecord  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";
                        }
                    }
                }
            }

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                //if (chkSendSMS.Checked)
                {
                    int _count = dba.SendEmailIDAndWhatsappNumberToSupplier(strAllSaleBillNo);
                    if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                    else
                        MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show("Thank you !! Record imported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ConvertDateTime(ref DateTime _date, string strDate)
        {
            try
            { 
              
                {
                    double dDate = dba.ConvertObjectToDouble(strDate);
                    if (dDate > 0)
                        _date = DateTime.FromOADate(dDate);
                    else
                    {
                        try
                        {
                            char split = '/';
                            if (strDate.Contains("-"))
                                split = '-';
                            string[] strNDate = strDate.Split(' ');
                            string[] strAllDate = strNDate[0].Split(split);
                            string strMonth = strAllDate[0], strFormat = "dd/MM/yyyy";
                            if (strMonth.Length == 1)
                                strFormat = "d/M/yyyy";

                            if (dba.ConvertObjectToInt(strMonth) == MainPage.currentDate.Month)
                            {
                                strFormat = "MM/dd/yyyy";
                                if (strMonth.Length == 1)
                                    strFormat = "M/d/yyyy";
                            }
                            if (strAllDate.Length > 2)
                            {
                                if (strAllDate[2].Length == 2)
                                    strFormat = strFormat.Replace("yyyy", "yy");
                            }

                            if (strDate.Contains("-"))
                                strFormat = strFormat.Replace("/", "-");

                            if (strDate.Length > 10)
                            {
                                string strTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                                if (strDate.Contains("AM") || strDate.Contains("PM"))
                                    strFormat += " " + strTimeFormat;// " hh:mm:ss tt";//
                                else
                                {
                                    string[] strTime = strDate.Split(':');
                                    if (strTime.Length > 2)
                                        strFormat += " HH:mm:ss";
                                    else
                                        strFormat += " HH:mm";
                                }
                            }

                            _date = dba.ConvertDateInExactFormat(strDate, strFormat);
                        }
                        catch
                        {
                            _date = Convert.ToDateTime(strDate);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }


        private DataSet GetDataFromExcel()
        {
            DataSet ds = null;
            try
            {
                if (txtFilePath.Text != "")
                {
                    if (txtFilePath.Text.Contains(".XLS"))
                    {

                        FileStream stream = new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read);
                        IExcelDataReader excelReader = null;
                        if (txtFilePath.Text.ToUpper().Contains(".XLSX"))
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        else
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        excelReader.IsFirstRowAsColumnNames = true;
                        ds = excelReader.AsDataSet();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ds;
        }

        private void btnEInvoice_Click(object sender, EventArgs e)
        {
            btnEInvoice.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want generate E-Invoice ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    GenerateEInvoice();
            }
            catch { }
            btnEInvoice.Enabled = true;
        }

        private void GenerateEInvoice()
        {
            string strSaleBillNo = GetSelectedSaleBillNo();
            if (strSaleBillNo != "")
            {
                var _success = dba.GenerateEInvoiceJSON_SaleBook(true,strSaleBillNo);
                if (_success)
                {
                    DialogResult _result = MessageBox.Show("Are you want to open e-invoice site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://einvoice1.gst.gov.in/Invoice/BulkUpload");
                }
            }
        }
    }
}
