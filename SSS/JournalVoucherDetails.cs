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
    public partial class JournalVoucherDetails : Form
    {
        DataBaseAccess dba;
        string strRegionName = "LOCAL",strPartyID="",strVoucherCode="",strVoucherNo="";
        protected internal double dTotalTaxAmt = 0;
        public string strDataQuery = "", strGSTNature = "";
        public JournalVoucherDetails(string strPartyName, string strExpName, string strVCode,string strVNo)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();
                txtPartyName.Text = strPartyName;
                txtAccountName.Text = strExpName;
                strVoucherCode = strVCode;
                strVoucherNo = strVNo;
                string[] strFullParty = strPartyName.Split(' ');
                if (strFullParty.Length > 1)
                {
                    strPartyID = strFullParty[0];
                }

                GetPartyDetails(strPartyID);
                dgrdBillDetails.Rows.Add(1);
                dgrdBillDetails.Rows[dgrdBillDetails.RowCount - 1].Cells["sNo"].Value = dgrdBillDetails.Rows.Count;
            }
            catch
            { }
        }

        public JournalVoucherDetails(string strPartyName, string strVCode, string strVNo)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();             
                strVoucherCode = strVCode;
                strVoucherNo = strVNo; 
                GetPartyDetails("");
                dgrdBillDetails.Rows.Add(1);
                dgrdBillDetails.Rows[dgrdBillDetails.RowCount - 1].Cells["sNo"].Value = dgrdBillDetails.Rows.Count;
            }
            catch
            { }
        }

        private void GetPartyDetails(string strPartyName)
        {
            try
            {
                string strQuery = " Select *,Convert(varchar,InvoiceDate,103) IDate,dbo.GetFullName(PartyID) PartyName,dbo.GetFullName(AccountID) AccountName from JournalVoucherDetails Where VoucherCode='" + strVoucherCode + "' and VoucherNo=" + strVoucherNo + " "
                                + " Select TOP 1 SUBSTRING(CD.GSTNo,1,2)StateName,ISNULL((Select Top 1 * from (Select TOP 1 SUBSTRING(GSTNo,1,2) StateCode from SupplierMaster Where GSTNo!='' and (AreaCode+CAST(AccountNo as nvarchar))='" + strPartyName.Replace("'", "") + "' UNION ALL Select TOP 1 SUBSTRING(GSTNo,1,2) StateCode from SupplierMaster Where GSTNo!='' and GroupName not Like('%DIRECT EXPENSE%') AND GroupName not Like('OTHER CURRENT LIABILITIES') and (AreaCode+CAST(AccountNo as nvarchar)) in (Select AccountID from BalanceAmount Where VoucherCode='" + strVoucherCode + "' and VoucherNo=" + strVoucherNo + "))_Balance) ,'') as PStateName from CompanyDetails CD ";
                               
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    BindDataWithGrid(ds.Tables[0]);
                    DataTable dt = ds.Tables[1];
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        if (Convert.ToString(row["StateName"]) == Convert.ToString(row["PStateName"]))
                        {
                            lblRegion.Text = strRegionName + " (" + row["PStateName"] + ")";
                            dgrdBillDetails.Columns["igstAmt"].Visible = false;
                            dgrdBillDetails.Columns["cgstAmt"].Visible = dgrdBillDetails.Columns["sgstAmt"].Visible = true;
                        }
                        else
                        {
                            strRegionName = "INTERSTATE";
                            dgrdBillDetails.Columns["igstAmt"].Visible = true;
                            dgrdBillDetails.Columns["cgstAmt"].Visible = dgrdBillDetails.Columns["sgstAmt"].Visible = false;
                            lblRegion.Text = strRegionName + " (" + row["PStateName"] + ")";
                        }
                    }
                }
            }
            catch { }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            dgrdBillDetails.Rows.Clear();
            if(dt.Rows.Count>0)
            {
                dgrdBillDetails.Rows.Add(dt.Rows.Count);
                int _rowIndex = 0;
                txtPartyName.Text = Convert.ToString(dt.Rows[0]["PartyName"]);
                txtAccountName.Text = Convert.ToString(dt.Rows[0]["AccountName"]);
                txtReverseCharge.Text = Convert.ToString(dt.Rows[0]["RCMNature"]);
                txtRemark.Text = Convert.ToString(dt.Rows[0]["Remark"]);
                txtInvoiceNo.Text = Convert.ToString(dt.Rows[0]["OriginalInvoiceNo"]);
                txtInvDate.Text = Convert.ToString(dt.Rows[0]["IDate"]);
                lblDiffAmt.Text = dba.ConvertObjectToDouble(dt.Rows[0]["TotalDiffAmt"]).ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dba.ConvertObjectToDouble(dt.Rows[0]["TotalTaxAmt"]).ToString("N2",MainPage.indianCurancy);
              
                foreach (DataRow row in dt.Rows)
                {
                    dgrdBillDetails.Rows[_rowIndex].Cells["sNo"].Value = _rowIndex + 1;
                    dgrdBillDetails.Rows[_rowIndex].Cells["itemName"].Value = row["Other"];
//                    dgrdBillDetails.Rows[_rowIndex].Cells["invDate"].Value = row["IDate"];
                    dgrdBillDetails.Rows[_rowIndex].Cells["diffAmt"].Value = row["DiffAmt"];
                    dgrdBillDetails.Rows[_rowIndex].Cells["gstPer"].Value = row["GSTPer"];
                    dgrdBillDetails.Rows[_rowIndex].Cells["igstAmt"].Value = row["IGSTAmt"];
                    dgrdBillDetails.Rows[_rowIndex].Cells["cgstAmt"].Value = row["CGSTAmt"];
                    dgrdBillDetails.Rows[_rowIndex].Cells["sgstAmt"].Value = row["SGSTAmt"];

                    _rowIndex++;
                }
            }
        }

        private void JournalVoucherDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !dgrdBillDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdBillDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
                    e.Cancel = true;
                else
                {
                    if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("ITEMNAME","JOURNAL", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            string[] strItem = objSearch.strSelectedData.Split('|');
                            if (strItem.Length > 0)
                                dgrdBillDetails.CurrentCell.Value = strItem[0];      
                                                 
                        }
                        e.Cancel = true;
                    }
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void dgrdBillDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdBillDetails.CurrentCell.ColumnIndex == 1)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                }
                else if (dgrdBillDetails.CurrentCell.ColumnIndex == 2 || dgrdBillDetails.CurrentCell.ColumnIndex == 3)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);

                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {           
            if (dgrdBillDetails.CurrentCell.ColumnIndex == 2 || dgrdBillDetails.CurrentCell.ColumnIndex == 3)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void dgrdBillDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdBillDetails.CurrentCell.RowIndex;
                    IndexColmn = dgrdBillDetails.CurrentCell.ColumnIndex;
                    if (Index < dgrdBillDetails.RowCount - 1)
                    {
                        CurrentRow = Index - 1;
                    }
                    else
                    {
                        CurrentRow = Index;
                    }
                    if (IndexColmn < dgrdBillDetails.ColumnCount - 1 && IndexColmn!=4)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                        {
                            if (!dgrdBillDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdBillDetails.ColumnCount - 1)
                                IndexColmn++;
                            if (!dgrdBillDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdBillDetails.ColumnCount - 1)
                                IndexColmn++;
                            dgrdBillDetails.CurrentCell = dgrdBillDetails.Rows[CurrentRow].Cells[IndexColmn];
                        }
                    }
                    else if (Index == dgrdBillDetails.RowCount - 1)
                    {
                        string strItemName = Convert.ToString(dgrdBillDetails.Rows[CurrentRow].Cells["itemName"].Value), strAmt = Convert.ToString(dgrdBillDetails.Rows[CurrentRow].Cells["diffAmt"].Value);

                        if (strItemName != "" && strAmt != "")
                        {
                            dgrdBillDetails.Rows.Add(1);
                            dgrdBillDetails.Rows[dgrdBillDetails.RowCount - 1].Cells["sNo"].Value = dgrdBillDetails.Rows.Count;
                            dgrdBillDetails.CurrentCell = dgrdBillDetails.Rows[dgrdBillDetails.RowCount - 1].Cells["itemName"];
                        }
                        else
                        {
                            btnSubmit.Focus();
                        }
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    dgrdBillDetails.Rows.RemoveAt(dgrdBillDetails.CurrentRow.Index);
                    if (dgrdBillDetails.Rows.Count == 0)
                    {
                        dgrdBillDetails.Rows.Add(1);
                        dgrdBillDetails.Rows[0].Cells["sNo"].Value = 1;
                        dgrdBillDetails.CurrentCell = dgrdBillDetails.Rows[0].Cells["sNo"];
                        dgrdBillDetails.Enabled = true;
                    }
                    else
                    {
                        ArrangeSerialNo();
                    }                   
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    int colIndex = dgrdBillDetails.CurrentCell.ColumnIndex;
                    if (colIndex == 1 || colIndex == 2)
                        dgrdBillDetails.CurrentCell.Value = "";                    
                }
                else if (e.KeyValue == 96)
                    e.Handled = true;
            }
            catch { }
        }


        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdBillDetails.Rows)
            {
                row.Cells["sNo"].Value = serialNo;
                serialNo++;
            }
        }
        
        private void dgrdBillDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //if (e.ColumnIndex == 2)
                //{
                //    string strDate = Convert.ToString(dgrdBillDetails.CurrentCell.EditedFormattedValue);
                //    if (strDate.Length==8)
                //    {
                //        strDate = strDate.Replace("/", "");
                //        if (strDate.Length == 8)
                //        {
                //            TextBox txtDate = new TextBox();
                //            txtDate.Text = strDate;
                //            dba.GetStringFromDateForReporting(txtDate);
                //            if (txtDate.Text.Contains("/"))
                //            {                               
                //                if (e.RowIndex < dgrdBillDetails.Rows.Count - 1)
                //                {
                //                    dgrdBillDetails.EndEdit();
                //                }
                //            }
                //            dgrdBillDetails.CurrentCell.Value = txtDate.Text;
                //        }
                //        else
                //        {
                //            MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);                            
                //        }
                //    }
                //}
               // else 
                if (e.ColumnIndex == 3 || e.ColumnIndex == 2)
                    CalculateAllAmount();
            }
            catch { }
        }

        private void dgrdBillDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //try
            //{
            //    if (e.ColumnIndex == 2)
            //    {
            //        string strDate = Convert.ToString(dgrdBillDetails.CurrentCell.EditedFormattedValue);
            //        if (strDate != "")
            //        {
            //            strDate = strDate.Replace("/", "");
            //            if (strDate.Length == 8)
            //            {
            //                TextBox txtDate = new TextBox();
            //                txtDate.Text = strDate;
            //                dba.GetStringFromDateForReporting(txtDate);
            //                if (!txtDate.Text.Contains("/"))
            //                {
            //                    e.Cancel = true;
            //                }
            //                else
            //                {
            //                    if (e.RowIndex < dgrdBillDetails.Rows.Count - 1)
            //                    {
            //                        dgrdBillDetails.EndEdit();
            //                    }
            //                }
            //                dgrdBillDetails.CurrentCell.Value = txtDate.Text;
            //            }
            //            else
            //            {
            //                MessageBox.Show("Date format is not valid ! Please Specify in ddMMyyyy format ", "Invalid Date Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                e.Cancel = true;
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //}
        }

        private void CalculateAllAmount()
        {
            try
            {
                double dAmt = 0, dTAmt = 0, dTaxRate = 0, dTaxAmt = 0, dTTaxAmt = 0;
                foreach (DataGridViewRow row in dgrdBillDetails.Rows)
                {
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["diffAmt"].Value);
                    dTaxRate = dba.ConvertObjectToDouble(row.Cells["gstPer"].Value);
                    dTTaxAmt += dTaxAmt = (dAmt * dTaxRate) / 100;
                    if (strRegionName == "LOCAL")
                    {
                        row.Cells["cgstAmt"].Value = row.Cells["sgstAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                        row.Cells["igstAmt"].Value = 0;
                    }
                    else
                    {
                        row.Cells["igstAmt"].Value = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                        row.Cells["cgstAmt"].Value = row.Cells["sgstAmt"].Value = 0;
                    }

                }
                lblDiffAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblTaxAmt.Text = dTTaxAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private bool ValidateControl()
        {
            foreach (DataGridViewRow row in dgrdBillDetails.Rows)
            {
                string strName = Convert.ToString(row.Cells["itemName"].Value);
                double dAmt = dba.ConvertObjectToDouble(row.Cells["diffAmt"].Value);
                if (strName == "" && dAmt == 0)
                    dgrdBillDetails.Rows.Remove(row); 
                else if (strName == "")
                {
                    MessageBox.Show("Sorry ! Item name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdBillDetails.CurrentCell = row.Cells["itemName"];
                    dgrdBillDetails.Focus();
                    return false;
                }              
                else if (dAmt == 0)
                {
                    MessageBox.Show("Sorry ! Amount can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdBillDetails.CurrentCell = row.Cells["diffAmt"];
                    dgrdBillDetails.Focus();
                    return false;
                }
            }
            if (txtPartyName.Text == "")
            {
                MessageBox.Show("Sorry ! Party name can't be blank !!", "Party name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPartyName.Focus();
                return false;
            }
            if (txtAccountName.Text == "")
            {
                MessageBox.Show("Sorry ! Account name can't be blank !!", "Account name required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccountName.Focus();
                return false;
            }
            if (txtInvoiceNo.Text == "")
            {
                MessageBox.Show("Sorry ! Invoice no can't be blank !!", "Invoice no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInvoiceNo.Focus();
                return false;
            }          
            if (txtInvDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInvDate.Focus();
                return false;
            }     

            if (Math.Round(dba.ConvertObjectToDouble(lblTaxAmt.Text),0) != Math.Round(dTotalTaxAmt,0))
            {
                DialogResult result = MessageBox.Show("Sorry ! Tax Amt and Diff amt not match, Are you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return true;
                else
                    return false;
            }

            string[] strParty = txtPartyName.Text.Split(' ');
            if (strParty.Length > 0)
            {
                string strQuery = "Select Distinct VoucherNo from JournalVoucherDetails Where VoucherCode='" + strVoucherCode + "' and VoucherNo!=" + strVoucherNo + " and OriginalInvoiceNo='" + txtInvoiceNo.Text + "' and PartyID='"+ strParty[0]+"' ";
                object _obj = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (Convert.ToString(_obj) != "")
                {
                    MessageBox.Show("Sorry ! This Invoice no is already linked with voucher no : "+_obj+" !!", "Date required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInvoiceNo.Focus();
                    return false;
                }
            }

            return true;
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtPartyName.Text = objSearch.strSelectedData;
                        GetRegionName();
                    }
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetRegionName()
        {
            string strQuery = "Select (CASE WHEN SUBSTRING(GSTNo,1,2)=PGSTNo then 'LOCAL' else 'INTERSTATE' end) Region from ( Select TOP 1 SUBSTRING(CD.GSTNo,1,2)GSTNo ,ISNULL((Select TOP 1 SUBSTRING(SM.GSTNo,1,2) PGSTNo from SupplierMaster SM Where (AreaCode+CAST(AccountNo as nvarchar)+' '+Name)='" + txtPartyName.Text + "'),'') as PGSTNo from CompanyDetails CD)_Data ";

            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            strRegionName = Convert.ToString(objValue);
            if (strRegionName == "LOCAL")
            {               
                dgrdBillDetails.Columns["igstAmt"].Visible = false;
                dgrdBillDetails.Columns["cgstAmt"].Visible = dgrdBillDetails.Columns["sgstAmt"].Visible = true;
            }
            else
            {               
                dgrdBillDetails.Columns["igstAmt"].Visible = true;
                dgrdBillDetails.Columns["cgstAmt"].Visible = dgrdBillDetails.Columns["sgstAmt"].Visible = false;
                
            }
            lblRegion.Text = strRegionName;// + " (" + row["PStateName"] + ")";
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtInvoiceNo_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtInvDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtAccountName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtAccountName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtReverseCharge_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REVERSECHARGES", "SEARCH REVERSE CHARGES", e.KeyCode);
                    objSearch.ShowDialog();
                    txtReverseCharge.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if(ValidateControl())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        GetDataQuery();
                    }
                }
            }
            catch { }
        }

        private void GetDataQuery()
        {
            string strQuery = "", strDate = "",strPartyID="",strAccountID="";
            string[] strFullName = txtPartyName.Text.Split(' ');
            if (strFullName.Length > 1)
                strPartyID = strFullName[0].Trim();

            strFullName = txtAccountName.Text.Split(' ');
            if (strFullName.Length > 1)
                strAccountID = strFullName[0].Trim();

            strDate = dba.ConvertDateInExactFormat(txtInvDate.Text).ToString("MM/dd/yyyy");

            foreach (DataGridViewRow row in dgrdBillDetails.Rows)
            {
               
                strQuery += " INSERT INTO [dbo].[JournalVoucherDetails] ([VoucherCode],[VoucherNo],[PartyID],[OriginalInvoiceNo],[InvoiceDate],[DiffAmt],[GSTPer],[IGSTAmt],[CGSTAmt],[SGSTAmt],[Other],[TotalAmt],[TotalDiffAmt],[TotalTaxAmt],[AccountID],[RCMNature],[Remark],[Region],[InsertStatus]) VALUES "
                         + " ('" + strVoucherCode + "',@SerialNo,'" + strPartyID + "','" + txtInvoiceNo.Text + "','" + strDate + "','" + dba.ConvertObjectToDouble(row.Cells["diffAmt"].Value) + "','" + dba.ConvertObjectToDouble(row.Cells["gstPer"].Value) + "','" + dba.ConvertObjectToDouble(row.Cells["igstAmt"].Value) + "','" + dba.ConvertObjectToDouble(row.Cells["cgstAmt"].Value) + "','" + dba.ConvertObjectToDouble(row.Cells["sgstAmt"].Value) + "','"+ row.Cells["itemName"].Value+"','" + dba.ConvertObjectToDouble(lblTotalAmt.Text) + "','" + dba.ConvertObjectToDouble(lblDiffAmt.Text) + "','" + dba.ConvertObjectToDouble(lblTaxAmt.Text) + "','" + strAccountID + "','" + txtReverseCharge.Text + "','" + txtRemark.Text + "','" + strRegionName + "',1) ";
            }
            if (strQuery != "")
            {
                strDataQuery = strQuery;
                this.Close();
            }
        }


    }
}
