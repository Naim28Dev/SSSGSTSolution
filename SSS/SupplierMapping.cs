using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class SupplierMapping : Form
    {
        DataBaseAccess dba;
        public SupplierMapping()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtStartDate.Text = txtEndDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtSerialCode.Text = MainPage.strUserBranchCode+"M";
            GetAllMarketers();
            BindLastRecord();
        }
        public SupplierMapping(string strSerialCode , string strSerialNo)
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
           // txtStartDate.Text = txtEndDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtSerialCode.Text = strSerialCode;
            GetAllMarketers();
            GetDetails(strSerialNo);
        }
        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(Min(SerialNo),0))VoucherNo from SupplierMapping Where SerialCode = '" + txtSerialCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                GetDetails(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(SerialNo),'') from SupplierMapping Where SerialCode='" + txtSerialCode.Text + "' and SerialNo>" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                GetDetails(strSerialNo);
            }
            else
            {
                BindLastRecord();
            }
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(SerialNo),'') from  SupplierMapping Where SerialCode='" + txtSerialCode.Text + "' and SerialNo<" + txtSerialNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                GetDetails(strSerialNo);
            }
        }
        private void Marketer_Supplier_Details_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdSupplier.Focused)
            {
                SendKeys.Send("{TAB}");
            }
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bCashView)
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        BindNextRecord();
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        BindPreviousRecord();
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        BindFirstRecord();
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        BindLastRecord();
                    }
                }
            }

        }
        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(SerialNo),0))VoucherNo from SupplierMapping Where SerialCode='" + txtSerialCode.Text + "' ");
            txtSerialNo.Text = Convert.ToString(objValue);
            GetDetails(txtSerialNo.Text);
        }
        private void GetAllMarketers()
        {
            try
            {
                DataTable table = dba.GetDataTable("Select Distinct MarketerName from Marketer MK Where MK.OrderCode Like('" + MainPage.strUserBranchCode + "%')  Order by MarketerName ");
                lboxAgentName.Items.Clear();
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        lboxAgentName.Items.Add(dr["MarketerName"]);
                    }
                    lboxAgentName.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Binding Details Name from Marketer Mapping", ex.Message };
                dba.CreateErrorReports(StrReport);
            }
        }

        private void lboxAgentName_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void GetDetails(string SerialNo)
        {
            try
            {
                chkActive.Checked = false;

                dgrdSupplier.Rows.Clear();
                if (SerialNo != "")
                {
                    int _index = 0;
                    string strQuery = " Select ID,MarketerName,SerialCode,SerialNo,CONVERT(varchar,StartDate,103)SDate,CONVERT(varchar,EndDate,103)EDate,ActiveStatus,SupplierName,CreatedBy,UpdatedBy from SupplierMapping SM Where SerialCode = '" + txtSerialCode.Text+"' AND SerialNo = "+dba.ConvertObjectToDouble(SerialNo)+ " ORDER BY MarketerName ";
                    DataTable _dt = dba.GetDataTable(strQuery);
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow _row = _dt.Rows[0];
                        txtStartDate.Text = Convert.ToString(_row["SDate"]);
                        txtEndDate.Text = Convert.ToString(_row["EDate"]);

                        lboxAgentName.SelectedItem = Convert.ToString(_row["MarketerName"]);
                        txtDates.SelectionStart = dba.ConvertDateInExactFormat(txtStartDate.Text);
                        txtDates.SelectionEnd = dba.ConvertDateInExactFormat(txtEndDate.Text);

                        txtSerialCode.Text = Convert.ToString(_row["SerialCode"]);
                        txtSerialNo.Text = Convert.ToString(_row["SerialNo"]);
                        chkActive.Checked = Convert.ToBoolean(_row["ActiveStatus"]);
                        string strCreatedBy = Convert.ToString(_row["CreatedBy"]), strUpdatedBy = Convert.ToString(_row["UpdatedBy"]);

                        if (strCreatedBy != "")
                            lblCreatedBy.Text = "Created by : " + strCreatedBy;
                        if (strUpdatedBy != "")
                            lblCreatedBy.Text += " , Updated by : " + strUpdatedBy;

                        dgrdSupplier.Rows.Add(_dt.Rows.Count);
                        foreach (DataRow row in _dt.Rows)
                        {
                            dgrdSupplier.Rows[_index].Cells["sSno"].Value = (_index + 1) + ".";
                            dgrdSupplier.Rows[_index].Cells["supplierName"].Value = row["SupplierName"];
                            dgrdSupplier.Rows[_index].Cells["sID"].Value = row["ID"];

                            _index++;
                        }
                        EnableDisable(false);
                    }
                }

                AddNewRow();
                txtSerialNo.ReadOnly = false;
            }
            catch(Exception ex) {
                MessageBox.Show("Sorry ! Somting went wrong that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                            if (strAccountName != "")
                            {
                                AddNewRow();
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
                            bool _bStatus = DeleteSingleRow(strID);
                            if (_bStatus)
                                dgrdSupplier.Rows.RemoveAt(dgrdSupplier.CurrentRow.Index);
                        }
                    }

                    if (dgrdSupplier.Rows.Count == 0)
                    {
                        AddNewRow();
                    }
                    else
                    {
                        ArrangeSupplierSerialNo();
                    }
                }
            }
            catch { }
        }
        private void AddNewRow()
        {
            dgrdSupplier.Rows.Add(1);
            dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["sSno"].Value = dgrdSupplier.Rows.Count + ".";
            dgrdSupplier.CurrentCell = dgrdSupplier.Rows[dgrdSupplier.RowCount - 1].Cells["supplierName"];
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
            string strQuery = " Delete from [dbo].[SupplierMapping] Where [ID]=" + strID + " and [BranchCode]='" + MainPage.strUserBranchCode + "' ";

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
                return true;
            else
                return false;

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
                        SearchData objSearch = new SearchData("PURCHASEPARTYNICKNAME_MAPPING", "Search Supplier Name", Keys.Space);
                        objSearch.ShowDialog();
                        if (!CheckDuplicate(objSearch.strSelectedData))
                        {
                            dgrdSupplier.CurrentCell.Value = objSearch.strSelectedData;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Supplier already added !!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private bool CheckDuplicate(string strName)
        {
            foreach(DataGridViewRow dr in dgrdSupplier.Rows)
            {
               if(Convert.ToString(dr.Cells["supplierName"].Value) == strName)
               {
                    return true;
               }
            }
            return false;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateControls()
        {
            string strSelected = Convert.ToString(lboxAgentName.SelectedItem);
            if (strSelected == "")
            {
                MessageBox.Show("Sorry ! Marketer name can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lboxAgentName.Focus();
                return false;
            }
            string strName = "";
            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                strName = Convert.ToString(row.Cells["supplierName"].Value);
                if (strName == "")
                {
                    dgrdSupplier.Rows.Remove(row);
                }
            }
            if (dgrdSupplier.Rows.Count <= 0)
            {
                MessageBox.Show("Sorry ! No supplier selected to save ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lboxAgentName.Focus();
                AddNewRow();
                return false;
            }
            return true;
        }

        private void SetSerialNo()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(SerialNo),0)+1)VoucherNo from SupplierMapping Where SerialCode='" + txtSerialCode.Text + "' ");
            txtSerialNo.Text = Convert.ToString(objValue);
        }
        private void EnableDisable(bool status)
        {
            txtDates.Enabled = status;
            dgrdSupplier.ReadOnly = !status;
            txtStartDate.ReadOnly = txtEndDate.ReadOnly = !status;
            txtSerialCode.ReadOnly = txtSerialNo.ReadOnly = !status;
            chkActive.Enabled = status;
            lboxAgentName.Enabled = status;
        }
        private void ClearAllText()
        {
            lblCreatedBy.Text = "";
            txtSerialCode.Text = MainPage.strUserBranchCode+"M";
            dgrdSupplier.Rows.Clear();
            AddNewRow();
            chkActive.Checked = true;
            lboxAgentName.SelectedIndex = -1;
            txtDates.SelectionStart = txtDates.SelectionEnd = DateTime.Today;
            txtStartDate.Text = txtEndDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            txtDates.Focus();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;

                if(btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Save";
                    btnEdit.Text = "&Edit";
                   
                    SetSerialNo();
                    EnableDisable(true);
                    ClearAllText();
                    GetAllMarketers();
                }
                else if (ValidateControls())
                {
                    DialogResult _result = MessageBox.Show("Are you sure you want to save records ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch { }
            btnAdd.Enabled = true;
        }

        private int SaveRecord()
        {
            string strQuery = "", strID = "";
            string strSelectedMarketer = Convert.ToString(lboxAgentName.SelectedItem);
            DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);

            //strQuery += " DELETE FROM SupplierMapping WHERE [MarketerName]='" + strSelectedMarketer + "' AND [StartDate] < '" + sDate.ToString("MM/dd/yyyy") + "' AND [StartDate] > '" + eDate.ToString("MM/dd/yyyy") + "' ";

            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                if (Convert.ToString(row.Cells["supplierName"].Value) != "")
                {
                    strID = Convert.ToString(row.Cells["sID"].Value);
                    //double i = 1;
                    //DateTime dt = sDate;
                    //while (dt <= eDate)
                    //{
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SupplierMapping] ([MarketerName],[SerialCode],[SerialNo],[StartDate],[EndDate],[SupplierName],[CreatedBy],[InsertStatus]) VALUES "
                                     + " ('" + strSelectedMarketer + "','" + txtSerialCode.Text + "'," + dba.ConvertObjectToDouble(txtSerialNo.Text) + ",'" + sDate.ToString("MM/dd/yyyy") + "','" + eDate.ToString("MM/dd/yyyy") + "','" + row.Cells["supplierName"].Value + "','" + MainPage.strLoginName + "',1) ";
                    }
                    else
                        strQuery += " UPDATE [dbo].[SupplierMapping] Set [SupplierName]='" + row.Cells["supplierName"].Value + "',[StartDate] = '" + sDate.ToString("MM/dd/yyyy") + "',[EndDate] = '" + eDate.ToString("MM/dd/yyyy") + "' Where ID=" + strID + " ";
                    //    dt = sDate.AddDays(i);
                    //    i++;
                    //}
                }
            }

            strQuery += " UPDATE [dbo].[SupplierMapping] SET [ActiveStatus]='" + chkActive.Checked.ToString() + "',[BranchCode]='" + MainPage.strUserBranchCode + "',[RemoteID]=0,[CreatedBy]='" + MainPage.strLoginName + "',[InsertStatus]=1 Where [SerialCode]='" + txtSerialCode.Text + "' AND [SerialNo]=" + dba.ConvertObjectToDouble(txtSerialNo.Text)
                     + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('" + strSelectedMarketer + "','" + txtSerialCode.Text + "'," + dba.ConvertObjectToDouble(txtSerialNo.Text) + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dgrdSupplier.Rows.Count + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                GetDetails(txtSerialNo.Text);
                MessageBox.Show("Thank you ! Record saved successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                btnAdd.Text = "&Add";
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to save record right now !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _count;
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
               // string strSelectedMarketer = Convert.ToString(lboxAgentName.SelectedItem);
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record  ? ", "Delete Scheme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Delete  from [dbo].[SupplierMapping] Where [SerialCode]='" + txtSerialCode.Text + "' and [SerialNo]=" + dba.ConvertObjectToDouble(txtSerialNo.Text);

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record deleted successfully ! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            BindLastRecord();
                        }
                        else
                        { MessageBox.Show("Sorry ! Unable to delete record right now !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            catch { }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && lboxAgentName.Items.Count > 0)
                {
                    string strSelectedMarketer = Convert.ToString(lboxAgentName.SelectedItem);
                    if (strSelectedMarketer != "")
                    {
                        EditTrailDetails objEdit = new EditTrailDetails(strSelectedMarketer, "SUPPLIERMAPPING", "0");
                        objEdit.ShowDialog();
                    }
                }
            }
            catch { }
        }

        private void lBoxDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    string strDate = Convert.ToString(lBoxDate.SelectedItem);
            //    GetDetails(strDate);
            //}
            //catch
            //{
            //}
        }

        private void txtStartDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnAdd.Text = "&Add";
                    }
                    EnableDisable(true);
                    btnEdit.Text = "&Update";
                    txtDates.Focus();
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateControls())
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch { }
            btnEdit.Enabled = true;
        }
        private int UpdateRecord()
        {
            string strQuery = "", strID = "";
            string strSelectedMarketer = Convert.ToString(lboxAgentName.SelectedItem);
            DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);

           // strQuery += " DELETE From SupplierMapping WHERE SerialCode = '" + txtSerialCode.Text + "' AND SerialNo = " + dba.ConvertObjectToDouble(txtSerialNo.Text);

            foreach (DataGridViewRow row in dgrdSupplier.Rows)
            {
                if (Convert.ToString(row.Cells["supplierName"].Value) != "")
                {
                    strID = Convert.ToString(row.Cells["sID"].Value);
                    //double i = 1;
                    //DateTime dt = sDate;
                    //while (dt <= eDate)
                    //{
                        if (strID == "")
                        {
                            strQuery += " INSERT INTO [dbo].[SupplierMapping] ([MarketerName],[SerialCode],[SerialNo],[StartDate],[EndDate],[SupplierName],[CreatedBy],[InsertStatus]) VALUES "
                                     + " ('" + strSelectedMarketer + "','"+txtSerialCode.Text+"',"+dba.ConvertObjectToDouble(txtSerialNo.Text)+",'" + sDate.ToString("MM/dd/yyyy") + "','" + eDate.ToString("MM/dd/yyyy") + "','" + row.Cells["supplierName"].Value + "','" + MainPage.strLoginName + "',1) ";
                        }
                        else
                            strQuery += " UPDATE [dbo].[SupplierMapping] Set [SupplierName]='" + row.Cells["supplierName"].Value + "',[StartDate] = '" + sDate.ToString("MM/dd/yyyy") + "',[EndDate] = '" + eDate.ToString("MM/dd/yyyy") + "' Where ID=" + strID + " ";
                    //    dt = sDate.AddDays(i);
                    //    i++;
                    //}
                }
            }

            strQuery += " UPDATE [dbo].[SupplierMapping] SET [MarketerName] = '" + strSelectedMarketer + "', [ActiveStatus]='" + chkActive.Checked.ToString() + "',[BranchCode]='" + MainPage.strUserBranchCode + "',[RemoteID]=0,[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [SerialCode]='" + txtSerialCode.Text + "' AND [SerialNo]=" + dba.ConvertObjectToDouble(txtSerialNo.Text) 
                    
                        + "  INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                        + "('" + strSelectedMarketer + "','" + txtSerialCode.Text + "'," + dba.ConvertObjectToDouble(txtSerialNo.Text) + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dgrdSupplier.Rows.Count + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                GetDetails(txtSerialNo.Text);
                MessageBox.Show("Thank you ! Record saved successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                btnEdit.Text = "&Edit";
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to save record right now !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _count;
        }
        private void lBoxDate_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.F1)
            //    {
            //        string strMarketer = Convert.ToString(lboxAgentName.SelectedItem);
            //        //if (lBoxDate.SelectedIndex >= 0 && strMarketer != "")
            //        //{
            //        //    DateTime strDate = dba.ConvertDateInExactFormat(Convert.ToString(lBoxDate.SelectedItem));
                   
            //        //    DialogResult result = MessageBox.Show("Are you sure you want to delete record  ? ", "Delete Scheme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        //    if (result == DialogResult.Yes)
            //        //    {
            //        //        string strQuery = " Delete  from [dbo].[SupplierMapping] Where [MarketerName] = '"+ strMarketer + "' AND [StartDate]='" + strDate.ToString("MM/dd/yyyy") + "' ";

            //        //        int count = dba.ExecuteMyQuery(strQuery);
            //        //        if (count > 0)
            //        //        {
            //        //            MessageBox.Show("Thank you ! Record deleted successfully ! ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        //            GetDates(strMarketer);
            //        //        }
            //        //        else
            //        //        { MessageBox.Show("Sorry ! Unable to delete record right now !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            //        //    }
            //        //}
            //    }
            //}
            //catch { }
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtSerialNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        GetDetails(txtSerialNo.Text);
                    }
                }
                else
                {
                    txtSerialNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void txtDates_DateChanged(object sender, DateRangeEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    txtStartDate.Text = (txtDates.SelectionRange.Start).ToString("dd/MM/yyyy");
                    txtEndDate.Text = (txtDates.SelectionRange.End).ToString("dd/MM/yyyy");

                    //GetMarketersOnDateRange();
                }
            }
            catch { }
        }
        //private void GetMarketersOnDateRange()
        //{
        //    try
        //    {
        //        dgrdSupplier.Rows.Clear();
        //        if (txtStartDate.TextLength == 10 && txtEndDate.TextLength == 10)
        //        {
        //            DateTime strSDate = dba.ConvertDateInExactFormat(txtStartDate.Text);
        //            DateTime strEDate = dba.ConvertDateInExactFormat(txtEndDate.Text).AddDays(1);
        //            DataTable table = dba.GetDataTable("SELECT Distinct MarketerName FROM SupplierMapping WHERE StartDate >= '"+ strSDate.ToString("MM/dd/yyyy") + "' AND StartDate < '" + strEDate.ToString("MM/dd/yyyy") + "' ORDER BY MarketerName");
        //            lboxAgentName.Items.Clear();
        //            if (table.Rows.Count > 0)
        //            {
        //                foreach (DataRow dr in table.Rows)
        //                {
        //                    lboxAgentName.Items.Add(dr["MarketerName"]);
        //                }
        //                lboxAgentName.SelectedIndex = 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string[] StrReport = { "Error Occur on Binding Details Name from Marketer Mapping", ex.Message };
        //        dba.CreateErrorReports(StrReport);
        //    }
        //}
    }
}
