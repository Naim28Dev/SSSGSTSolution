using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace SSS
{
    public partial class SchemeDetailMaster : Form
    {
        DataBaseAccess dba;   
        public SchemeDetailMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetDetails();
        }

        private void GetDetails()
        {
            try
            {
                if (txtSchemeName.Text != "")
                {
                    dgrdSupplier.Rows.Clear();
                    dgrdCustomer.Rows.Clear();
                    string strSQuery = "";
                    if (txtBranchCode.Text != "")
                        strSQuery = " and Other='"+txtBranchCode.Text+"' ";
                    int _index = 0;
                    string strQuery = " Select * from Scheme_CustomerDetails Where SchemeName='" + txtSchemeName.Text + "' Order by CustomerName Select *,Convert(varchar,StartDate,103)SDate,Convert(varchar,EndDate,103)EDate from Scheme_SupplierDetails Where SchemeName = '" + txtSchemeName.Text + "' "+ strSQuery+" Order by SupplierName ";
                    DataSet _ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (_ds.Tables.Count > 0)
                    {
                        DataTable _dt = _ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdCustomer.Rows.Add(_dt.Rows.Count);
                            foreach (DataRow row in _dt.Rows)
                            {
                                dgrdCustomer.Rows[_index].Cells["cSno"].Value = (_index + 1) + ".";
                                dgrdCustomer.Rows[_index].Cells["customerName"].Value = row["CustomerName"];
                                dgrdCustomer.Rows[_index].Cells["targetValue"].Value = row["TargetValue"];
                                dgrdCustomer.Rows[_index].Cells["cID"].Value = row["ID"];
                                _index++;
                            }
                        }
                        _index = 0;
                        _dt = _ds.Tables[1];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdSupplier.Rows.Add(_dt.Rows.Count);
                            foreach (DataRow row in _dt.Rows)
                            {
                                dgrdSupplier.Rows[_index].Cells["sSno"].Value = (_index + 1) + ".";
                                dgrdSupplier.Rows[_index].Cells["supplierName"].Value = row["SupplierName"];
                                dgrdSupplier.Rows[_index].Cells["disPer"].Value = row["Discount"];
                                dgrdSupplier.Rows[_index].Cells["amtValue"].Value = row["BillValue"];
                                dgrdSupplier.Rows[_index].Cells["sID"].Value = row["ID"];
                                dgrdSupplier.Rows[_index].Cells["startDate"].Value = row["SDate"];
                                dgrdSupplier.Rows[_index].Cells["endDate"].Value = row["EDate"];
                                dgrdSupplier.Rows[_index].Cells["branchName"].Value = row["Other"];
                                _index++;
                            }
                        }
                    }
                }

                dgrdCustomer.Rows.Add(1);
                dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["cSno"].Value = dgrdCustomer.Rows.Count+".";
                dgrdCustomer.CurrentCell = dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["customerName"];

                dgrdSupplier.Rows.Add(1);
                dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["sSno"].Value = dgrdSupplier.Rows.Count + ".";
                dgrdSupplier.CurrentCell = dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["supplierName"];

            }
            catch { }
        }
     

        private void UnitMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && ! dgrdSupplier.Focused && !dgrdCustomer.Focused)
                {
                    SendKeys.Send("{TAB}");
                }                
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSchemeName.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Scheme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Delete  from Scheme_CustomerDetails Where SchemeName='" + txtSchemeName.Text + "' and [Other]='" + MainPage.strBranchCode + "'  Delete from Scheme_SupplierDetails Where SchemeName='" + txtSchemeName.Text + "' and [Other]='" + MainPage.strBranchCode + "' ";

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record deleted successfully ! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            DataBaseAccess.CreateDeleteQuery(strQuery);

                            dgrdCustomer.Rows.Clear();
                            dgrdSupplier.Rows.Clear();

                            dgrdCustomer.Rows.Add(1);
                            dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["cSno"].Value = dgrdCustomer.Rows.Count + ".";
                            dgrdCustomer.CurrentCell = dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["customerName"];

                            dgrdSupplier.Rows.Add(1);
                            dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["sSno"].Value = dgrdSupplier.Rows.Count + ".";
                            dgrdSupplier.CurrentCell = dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["supplierName"];


                        }
                        else
                        { MessageBox.Show("Sorry ! Unable to delete record right now !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            catch { }
        }
       
        private void tsbtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        } 

        private void dgrdSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    {
                        Index = dgrdSupplier.CurrentCell.RowIndex;
                        IndexColmn = dgrdSupplier.CurrentCell.ColumnIndex;
                        if (Index < dgrdSupplier.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdSupplier.ColumnCount - 2)
                        {
                            IndexColmn += 1;
                            if (CurrentRow >= 0)
                            {
                                dgrdSupplier.CurrentCell = dgrdSupplier.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdSupplier.RowCount - 1)
                        {
                            string strAccountName = Convert.ToString(dgrdSupplier.Rows[CurrentRow].Cells["supplierName"].Value);
                            double dValue = dba.ConvertObjectToDouble(dgrdSupplier.Rows[CurrentRow].Cells["amtValue"].Value);
                            if (strAccountName != "" && dValue > 0)
                            {
                                dgrdSupplier.Rows.Add(1);
                                dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["sSno"].Value = dgrdSupplier.Rows.Count + ".";
                                dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["startDate"].Value = dgrdSupplier.Rows[CurrentRow].Cells["startDate"].Value;
                                dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["endDate"].Value = dgrdSupplier.Rows[CurrentRow].Cells["endDate"].Value;

                                dgrdSupplier.CurrentCell = dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["supplierName"];
                            }
                        }

                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    string strID = Convert.ToString(dgrdSupplier.CurrentRow.Cells["sID"].Value);
                    if (strID == "")                    
                        dgrdSupplier.Rows.RemoveAt(dgrdSupplier.CurrentRow.Index);                    
                    else
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus= DeleteSingleRow(strID);
                            if(_bStatus)
                                dgrdSupplier.Rows.RemoveAt(dgrdSupplier.CurrentRow.Index);
                        }
                    }

                    if (dgrdSupplier.Rows.Count == 0)
                    {
                        dgrdSupplier.Rows.Add(1);
                        dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["sSno"].Value = dgrdSupplier.Rows.Count + ".";
                        dgrdSupplier.CurrentCell = dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["supplierName"];
                    }
                    else
                    {
                        ArrangeSupplierSerialNo();
                    }
                }
            }
            catch { }
        }

        private void ArrangeSupplierSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                row.Cells["sSno"].Value = serialNo;
                serialNo++;
            }
        }

        private bool DeleteSingleRow(string strID)
        {
            string strQuery = "",strNetQuery="";

            strQuery = " Delete from [dbo].[Scheme_SupplierDetails] Where [ID]=" + strID + "  ";
            strNetQuery = " Delete from [dbo].[Scheme_SupplierDetails] Where [RemoteID]=" + strID + " and [Other]='" +MainPage.strBranchCode + "' ";
            
            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                DataBaseAccess.CreateDeleteQuery(strNetQuery);
                return true;
            }
            else
                return false;
        }

        private bool DeleteSingleRow_Customer(string strID)
        {
            string strQuery = "", strNetQuery = "";

            strQuery = "  Delete from [dbo].[Scheme_CustomerDetails] Where [ID]=" + strID + " and [Other]='" + MainPage.strBranchCode + "' ";
            strNetQuery = "  Delete from [dbo].[Scheme_CustomerDetails] Where [RemoteID]=" + strID + " and [Other]='" + MainPage.strBranchCode + "' ";

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                DataBaseAccess.CreateDeleteQuery(strNetQuery);
                return true;
            }
            else
                return false;
        }

        private void ArrangeCustomerSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdCustomer.Rows)
            {
                row.Cells["cSno"].Value = serialNo;
                serialNo++;
            }
        }

        private void dgrdCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    {
                        Index = dgrdCustomer.CurrentCell.RowIndex;
                        IndexColmn = dgrdCustomer.CurrentCell.ColumnIndex;
                        if (Index < dgrdCustomer.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdCustomer.ColumnCount - 2)
                        {
                            IndexColmn += 1;
                            if (CurrentRow >= 0)
                            {
                                dgrdCustomer.CurrentCell = dgrdCustomer.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdCustomer.RowCount - 1)
                        {
                            string strAccountName = Convert.ToString(dgrdCustomer.Rows[CurrentRow].Cells["customerName"].Value);                           
                            if (strAccountName != "")
                            {
                                dgrdCustomer.Rows.Add(1);
                                dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["cSno"].Value = dgrdCustomer.Rows.Count + ".";
                                dgrdCustomer.CurrentCell = dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["customerName"];
                            }
                        }

                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    string strID = Convert.ToString(dgrdSupplier.CurrentRow.Cells["cID"].Value);
                    if (strID == "")
                        dgrdCustomer.Rows.RemoveAt(dgrdCustomer.CurrentRow.Index);
                    else
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete current row ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            bool _bStatus = DeleteSingleRow_Customer(strID);
                            if (_bStatus)
                                dgrdCustomer.Rows.RemoveAt(dgrdCustomer.CurrentRow.Index);
                        }
                    }
                                       
                    if (dgrdCustomer.Rows.Count == 0)
                    {
                        dgrdCustomer.Rows.Add(1);
                        dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["cSno"].Value = dgrdCustomer.Rows.Count + ".";
                        dgrdCustomer.CurrentCell = dgrdCustomer.Rows[dgrdCustomer.RowCount - 1].Cells["customerName"];
                    }
                    else
                    {
                        ArrangeCustomerSerialNo();
                    }
                }
            }
            catch { }
        }

        private string GetBranchCode(string strName)
        {
            string strQuery = "Select AreaCode from SupplierMaster Where GroupName='SUNDRY CREDITOR' and Other='" + strName + "' ";
            object obj = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(obj);
        }

        private void dgrdSupplier_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                    e.Cancel = true;
                else 
                {
                    if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTYNICKNAME", "Search Supplier Name", Keys.Space);
                        objSearch.ShowDialog();
                        string strSupplierName = objSearch.strSelectedData;
                        dgrdSupplier.CurrentCell.Value = strSupplierName;
                        if (strSupplierName != "")
                            dgrdSupplier.CurrentRow.Cells["branchName"].Value = GetBranchCode(strSupplierName);
                        else
                            dgrdSupplier.CurrentRow.Cells["branchName"].Value = "";
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 6)
                    {
                        SearchData objSearch = new SearchData("BRANCHCODE", "SEARCH BRANCH CODE", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdSupplier.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void dgrdSupplier_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdSupplier.CurrentCell.ColumnIndex == 2 || dgrdSupplier.CurrentCell.ColumnIndex == 3 || dgrdSupplier.CurrentCell.ColumnIndex == 4 || dgrdSupplier.CurrentCell.ColumnIndex == 5)
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
            if (dgrdSupplier.CurrentCell.ColumnIndex == 2 || dgrdSupplier.CurrentCell.ColumnIndex == 3)
                dba.KeyHandlerPoint(sender, e, 2);
            else if (dgrdSupplier.CurrentCell.ColumnIndex == 4 || dgrdSupplier.CurrentCell.ColumnIndex == 5)
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void dgrdCustomer_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                    e.Cancel = true;
                else
                {
                    if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("SALESPARTYNICKNAME", "Search Customer Name", Keys.Space);
                        objSearch.ShowDialog();
                        dgrdCustomer.CurrentCell.Value = objSearch.strSelectedData;
                        e.Cancel = true;
                    }
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void dgrdCustomer_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdCustomer.CurrentCell.ColumnIndex == 2)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_C_KeyPress);
                }
            }
            catch
            {
            }
        }

        private void txtBox_C_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdCustomer.CurrentCell.ColumnIndex == 2)
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSchemeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("SCHEMENAME", "SEARCH SCHEME NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtSchemeName.Text = objSearch.strSelectedData;
                    GetDetails();
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private bool ValidateControls()
        {          
            if (txtSchemeName.Text == "")
            {
                MessageBox.Show("Sorry ! scheme name can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSchemeName.Focus();
                return false;
            }
            string strName,strStartDate,strEndDate,strBranchName;
            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                strName = Convert.ToString(row.Cells["supplierName"].Value);
                strStartDate = Convert.ToString(row.Cells["startDate"].Value);
                strEndDate = Convert.ToString(row.Cells["endDate"].Value);
                strBranchName = Convert.ToString(row.Cells["branchName"].Value);

                if (strName == "")
                {
                    dgrdSupplier.Rows.Remove(row);
                }
                else
                {
                    if (strBranchName == "")
                    {
                        MessageBox.Show("Sorry ! Branch name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdSupplier.CurrentCell = row.Cells["branchName"];
                        dgrdSupplier.Focus();
                        return false;
                    }
                    if (strStartDate == "")
                    {
                        MessageBox.Show("Sorry ! Start date can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdSupplier.CurrentCell = row.Cells["startDate"];
                        dgrdSupplier.Focus();
                        return false;
                    }
                    if (strEndDate == "")
                    {
                        MessageBox.Show("Sorry ! End date can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdSupplier.CurrentCell = row.Cells["endDate"];
                        dgrdSupplier.Focus();
                        return false;
                    }
                    if (strStartDate.Length == 8)
                    {
                        TextBox txtDate = new TextBox();
                        txtDate.Text = strStartDate;
                        dba.GetStringFromDateForReporting(txtDate, true);
                        row.Cells["startDate"].Value = strStartDate = txtDate.Text;
                    }
                    if (strEndDate.Length == 8)
                    {
                        TextBox txtDate = new TextBox();
                        txtDate.Text = strEndDate;
                        dba.GetStringFromDateForReporting(txtDate, true);
                        row.Cells["endDate"].Value = strEndDate = txtDate.Text;
                    }
                    if (strStartDate.Length != 10)
                    {
                        MessageBox.Show("Sorry ! Start date is not valid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdSupplier.CurrentCell = row.Cells["startDate"];
                        dgrdSupplier.Focus();
                        return false;
                    }
                    if (strEndDate.Length != 10)
                    {
                        MessageBox.Show("Sorry ! End date is not valid .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdSupplier.CurrentCell = row.Cells["endDate"];
                        dgrdSupplier.Focus();
                        return false;
                    }
                }
            }          

            foreach (DataGridViewRow row in dgrdCustomer.Rows)
            {
                strName = Convert.ToString(row.Cells["customerName"].Value);
                if (strName == "")
                    dgrdCustomer.Rows.Remove(row);
            }
            if (dgrdCustomer.Rows.Count == 0 && dgrdSupplier.Rows.Count == 0)
            {
                if (dgrdCustomer.Rows.Count == 0)
                {
                    MessageBox.Show("Sorry ! Please enter atleast one customer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdCustomer.Rows.Add();
                    dgrdCustomer.Rows[0].Cells["cSno"].Value = 1 + ".";
                    dgrdCustomer.CurrentCell = dgrdCustomer.Rows[0].Cells["customerName"];
                    return false;
                }
                if (dgrdSupplier.Rows.Count == 0)
                {
                    MessageBox.Show("Sorry ! Please enter atleast one supplier.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdSupplier.Rows.Add();
                    dgrdSupplier.Rows[0].Cells["sSno"].Value = 1 + ".";
                    dgrdSupplier.CurrentCell = dgrdSupplier.Rows[0].Cells["supplierName"];
                    return false;
                }
            }
            return true;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateControls())
                {
                    DialogResult _result = MessageBox.Show("Are you sure you want to submit records ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch { }
        }

        private int SaveRecord()
        {
            string strQuery = "", strID="",strBranchCode="";
            DateTime sDate, eDate;
            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                strID = Convert.ToString(row.Cells["sID"].Value);
                sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["startDate"].Value));
                eDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["endDate"].Value));
                strBranchCode = Convert.ToString(row.Cells["branchName"].Value);
                if (strID == "")
                {
                    strQuery += " if not exists (Select SchemeName from [dbo].[Scheme_SupplierDetails] Where SupplierName='" + row.Cells["supplierName"].Value + "' and SchemeName='" + txtSchemeName.Text + "' and [Other]='" + strBranchCode + "') begin "
                            + " INSERT INTO [dbo].[Scheme_SupplierDetails] ([RemoteID],[SchemeName],[PurchasePartyID],[SupplierName],[Discount],[BillValue],[Remark],[Other],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[StartDate],[EndDate]) VALUES "
                            + " (0,'" + txtSchemeName.Text + "','','" + row.Cells["supplierName"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["amtValue"].Value) + ",'','"+ strBranchCode+"',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',1,0,'"+sDate.ToString("MM/dd/yyyy")+ "','" + eDate.ToString("MM/dd/yyyy") + "') end  ";
                }
                else
                {
                    strQuery += " if exists (Select SchemeName from [dbo].[Scheme_SupplierDetails] Where [ID]=" + strID + ") begin if not exists (Select SchemeName from [dbo].[Scheme_SupplierDetails] Where [ID]!=" + strID + " and SupplierName='" + row.Cells["supplierName"].Value + "' and  SchemeName='" + txtSchemeName.Text + "' and [Other]='" + strBranchCode + "') begin  "
                              + " Update [dbo].[Scheme_SupplierDetails] Set [SchemeName]='" + txtSchemeName.Text + "',[SupplierName]='" + row.Cells["supplierName"].Value + "',[Discount]=" + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + ",[BillValue]=" + dba.ConvertObjectToDouble(row.Cells["amtValue"].Value) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[StartDate]='" + sDate.ToString("MM/dd/yyyy") + "',[EndDate]='" + eDate.ToString("MM/dd/yyyy") + "',[Other]='" + strBranchCode + "' Where [ID]=" + strID + " end end ";
                }
            }

            foreach (DataGridViewRow row in dgrdCustomer.Rows)
            {
                strID = Convert.ToString(row.Cells["cID"].Value);
                if (strID == "")
                {
                    strQuery += " if not exists (Select SchemeName from [dbo].[Scheme_CustomerDetails] Where CustomerName='" + row.Cells["customerName"].Value + "' and  SchemeName='" + txtSchemeName.Text + "') begin   "
                             + " INSERT INTO [dbo].[Scheme_CustomerDetails] ([RemoteID],[SchemeName],[SalesPartyID],[CustomerName],[TargetValue],[Remark],[Other],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                             + " (0,'" + txtSchemeName.Text + "','','" + row.Cells["customerName"].Value + "'," + dba.ConvertObjectToDouble(row.Cells["targetValue"].Value) + ",'','"+MainPage.strBranchCode+"',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',1,0) end ";
                }
                else
                {
                    strQuery += " if exists (Select SchemeName from [dbo].[Scheme_CustomerDetails] Where [ID]=" + strID + ") begin if not exists (Select SchemeName from [dbo].[Scheme_CustomerDetails] Where [ID]!=" + strID + " and CustomerName='" + row.Cells["customerName"].Value + "' and  SchemeName='" + txtSchemeName.Text + "') begin   "
                             + " Update [dbo].[Scheme_CustomerDetails] Set [SchemeName]='" + txtSchemeName.Text + "',[CustomerName]='" + row.Cells["customerName"].Value + "',[TargetValue]=" + dba.ConvertObjectToDouble(row.Cells["targetValue"].Value) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [ID]=" + strID + " end end ";
                }
            }


            int _count = dba.ExecuteMyQuery(strQuery);
            if(_count>0)
            {
                MessageBox.Show("Thank you ! Record saved successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else 
            {
                MessageBox.Show("Sorry ! Unable to save record right now !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _count;
        }

        private void TourMaster_Load(object sender, EventArgs e)
        {
            try
            {
                if (!SetPermission())
                    this.Close();
            }
            catch { }
        }

        private bool SetPermission()
        {
            try
            {
                if (MainPage.mymainObject.bSchemeMaster && (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView))
                {
                    if (!MainPage.mymainObject.bAccountMasterAdd)
                        btnSubmit.Enabled = false;
                    if (!MainPage.mymainObject.bAccountMasterEdit)
                        btnSubmit.Enabled = btnDelete.Enabled = false;

                    btnSubmit.Enabled = MainPage.mymainObject.bAccountMasterView;
                    return true;
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have sufficient permission to access this page.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return false;
                }
            }
            catch { this.Close(); }
            return false;
        }

        private void dgrdSupplier_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach(DataGridViewRow row in dgrdSupplier.Rows)
                {
                    row.Cells["sSno"].Value = _index;
                    _index++;
                }
            }
            catch { }
        }

        private void dgrdCustomer_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdCustomer.Rows)
                {
                    row.Cells["cSno"].Value = _index;
                    _index++;
                }
            }
            catch { }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to scheme supplier/customer details ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadSchemeSupplierName();
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! Scheme supplier details downloaded successfully... ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("No scheme supplier details found  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please enter online databse name and Live IP in company master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnDownload.Enabled = true;
        }

        private void dgrdSupplier_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4 || e.ColumnIndex == 5)
                {
                    string strDate = Convert.ToString(dgrdSupplier.CurrentCell.EditedFormattedValue);
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
                                if (e.RowIndex < dgrdSupplier.Rows.Count - 1)
                                {
                                    dgrdSupplier.EndEdit();
                                }
                            }
                            dgrdSupplier.CurrentCell.Value = txtDate.Text;
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

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                    GetDetails();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }
    }
}
