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
    public partial class SchemeMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewScheme = false;
        public string StrAddedSceheme = "";

        public SchemeMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public SchemeMaster(bool chk)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewScheme = chk;            
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select *,CONVERT(varchar,StartDate,103) SDate,CONVERT(varchar,EndDate,103) EDate from [SchemeMaster] Order by StartDate desc");
                if (objTable != null)
                {
                    dgrdName.Rows.Clear();
                    if (objTable.Rows.Count > 0)
                    {
                        dgrdName.Rows.Add(objTable.Rows.Count);

                        int rowIndex = 0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            dgrdName.Rows[rowIndex].Cells["id"].Value = dr["ID"];
                            dgrdName.Rows[rowIndex].Cells["schemeName"].Value = dr["SchemeName"];
                            dgrdName.Rows[rowIndex].Cells["startDate"].Value = dr["SDate"];
                            dgrdName.Rows[rowIndex].Cells["endDate"].Value = dr["EDate"];
                            dgrdName.Rows[rowIndex].Cells["branchCode"].Value = dr["BranchCode"];
                            dgrdName.Rows[rowIndex].Cells["createdBy"].Value = dr["CreatedBy"];
                            dgrdName.Rows[rowIndex].Cells["updatedBy"].Value = dr["UpdatedBy"];
                            dgrdName.Rows[rowIndex].Cells["remark"].Value = dr["Remark"];
                            dgrdName.Rows[rowIndex].Cells["activeStatus"].Value = dr["ActiveStatus"];

                            rowIndex++;
                        }
                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex-1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        if (dgrdName.Rows.Count == 1)
                            BindAllDetails(dgrdName.Rows[0]);
                    }

                }
            }
            catch { }
        }

        private void BindAllDetails(DataGridViewRow row)
        {
            try
            {
                txtSchemeName.Text = Convert.ToString(row.Cells["schemeName"].Value);
                txtStartDate.Text =Convert.ToString(row.Cells["startDate"].Value);
                txtEndDate.Text = Convert.ToString(row.Cells["endDate"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);
                txtBranchCode.Text = Convert.ToString(row.Cells["branchCode"].Value);
                chkActive.Checked = Convert.ToBoolean(row.Cells["activeStatus"].Value);

                string strCreatedBy = Convert.ToString(row.Cells["CreatedBy"].Value), strUpdatedBy = Convert.ToString(row.Cells["UpdatedBy"].Value);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);

                string strQuery = " if not exists (Select SchemeName from [dbo].[SchemeMaster] Where SchemeName='" + txtSchemeName.Text+"') begin  "
                                + " INSERT INTO [dbo].[SchemeMaster] ([BranchCode],[SchemeName],[StartDate],[EndDate],[Remark],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[ActiveStatus]) VALUES "
                                + " ('" + MainPage.strBranchCode + "','" + txtSchemeName.Text + "','" + sDate.ToString("MM/dd/yyyy") + "','" + eDate.ToString("MM/dd/yyyy") + "','','"+MainPage.strLoginName+"','',1,0,'"+chkActive.Checked.ToString()+"') end ";

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    MessageBox.Show("Thank you ! Record saved successfully ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (IsNewScheme)
                    {
                        StrAddedSceheme = txtSchemeName.Text;
                        this.Close();
                    }
                    else
                    {
                        btnAdd.Text = "&Add";
                        BindDataGrid();
                        lblMsg.Text = "";
                        dgrdName.Focus();
                    }
                }
            }
            catch { }
        }

        private bool ValidateControls()
        {
            if (txtSchemeName.Text == "")
            {
                MessageBox.Show("Sorry ! Scheme name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSchemeName.Focus();
                return false;
            }           
            if (txtStartDate.Text.Length!=10)
            {
                MessageBox.Show("Sorry ! Start date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStartDate.Focus();
                return false;
            }         
            if (txtEndDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! End date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEndDate.Focus();
                return false;
            }
            return true;
        }

        private void EnableAllControl()
        {
            txtSchemeName.ReadOnly = txtStartDate.ReadOnly = txtEndDate.ReadOnly = false;
            txtSearch.ReadOnly =chkActive.Enabled= true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtSchemeName.ReadOnly = txtStartDate.ReadOnly = txtEndDate.ReadOnly = true;
            txtSearch.ReadOnly = chkActive.Enabled = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtSchemeName.Text = txtStartDate.Text = txtEndDate.Text = txtSearch.Text = lblMsg.Text = txtBranchCode.Text = "";
            chkActive.Checked = true;
            txtStartDate.Text = txtEndDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    txtSchemeName.Focus();
                    ClearAllText();
                    EnableAllControl();
                    btnAdd.Text = "&Save";
                    btnEdit.Text = "&Edit";
                }
                else
                {
                    if (CheckAvailability() && ValidateControls())
                    {
                        DialogResult dar = MessageBox.Show("Are you sure you want to Save Record ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dar == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
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
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
                else
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bAccountMasterView)
                    {
                        if (e.KeyCode == Keys.PageUp)
                        {
                            if (dgrdName.CurrentRow.Index > 0)
                            {
                                dgrdName.CurrentCell = dgrdName.Rows[dgrdName.CurrentRow.Index - 1].Cells[1];
                                txtSearch.Focus();
                            }
                        }
                        else if (e.KeyCode == Keys.PageDown)
                        {
                            if (dgrdName.CurrentRow.Index < dgrdName.Rows.Count)
                            {
                                dgrdName.CurrentCell = dgrdName.Rows[dgrdName.CurrentRow.Index + 1].Cells[1];
                                txtSearch.Focus();
                            }
                        }
                        else if (e.KeyCode == Keys.Home)
                        {
                            dgrdName.CurrentCell = dgrdName.Rows[0].Cells[1];
                            txtSearch.Focus();
                        }
                        else if (e.KeyCode == Keys.End)
                        {
                            dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
                            txtSearch.Focus();
                        }
                    }
                }
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {    
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && lblId.Text!="")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Scheme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from [dbo].[SchemeMaster] Where ID=" + lblId.Text + "");
                        if (i > 0)
                        {
                            MessageBox.Show("Record is deleted Successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            ClearAllText();
                            BindDataGrid();
                        }
                    }
                }
                else
                {
                    btnAdd.Text = "&Add";
                    btnEdit.Text = "&Edit";
                }
            }
            catch { }
        }

        private void UpdateRecord()
        {
            try
            {
                DataGridViewRow row = dgrdName.SelectedRows[0];
                if (lblId.Text != "" && row != null)
                {
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                    {
                        DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);


                        string strOldSchemeName = Convert.ToString(row.Cells["schemeName"].Value);
                     
                        string strQuery = " Update [SchemeMaster] Set [SchemeName]='"+txtSchemeName.Text+"',[StartDate]='"+sDate.ToString("MM/dd/yyyy")+ "',[EndDate]='" + eDate.ToString("MM/dd/yyyy") + "',[UpdatedBy]='"+MainPage.strLoginName+ "',[ActiveStatus]='"+ chkActive.Checked.ToString()+"'  Where [SchemeName]='" + strOldSchemeName+"' "
                                       + " Update [OrderBooking] Set SchemeName='" + txtSchemeName.Text + "' Where SchemeName='" + strOldSchemeName + "' ";
                        

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            MessageBox.Show("Thank you ! Record is updated successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataGrid();
                        }
                        else
                            MessageBox.Show("Sorry ! your Record is not updated..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Sorry ! Please select right scheme Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in scheme Master.", ex.Message };
            }
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
                    }
                    btnEdit.Text = "&Update";
                    btnAdd.Text = "&Add";
                    EnableAllControl();
                }
                else
                {
                    if (CheckAvailability() && ValidateControls() && lblId.Text != "")
                    {
                        DialogResult dir = MessageBox.Show("Are you sure you want to update Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dir == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch { }
        }

        private bool CheckAvailability()
        {           
            try
            {
                if (txtSchemeName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select SchemeName from [SchemeMaster] Where SchemeName ='" + txtSchemeName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Scheme Name : "+txtSchemeName.Text+" already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtSchemeName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtSchemeName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select SchemeName from [SchemeMaster] Where SchemeName ='" + txtSchemeName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Scheme Name : " + txtSchemeName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtSchemeName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtSchemeName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Scheme Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtSchemeName.Focus();
                    return false;
                }
            }
            catch { }
            return false;
        }
        
        private void SearchResult()
        {
            try
            {
                if (objTable != null)
                {
                    if (txtSearch.Text!="")
                    {
                        DataRow[] row = objTable.Select(String.Format("SchemeName Like ('%" + txtSearch.Text + "%')"));
                        if (row.Length > 0)
                        {
                            int rowIndex = objTable.Rows.IndexOf(row[0]);
                            dgrdName.CurrentCell = dgrdName.Rows[rowIndex].Cells[1];
                            dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                            if (dgrdName.Rows.Count == 1)
                                BindAllDetails(dgrdName.Rows[0]);
                        }
                    }
                    else
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count-1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        if (dgrdName.Rows.Count == 1)
                            BindAllDetails(dgrdName.Rows[0]);
                    }
                }
                else
                {                    
                    BindDataGrid();
                }
            }
            catch { }
        }

        private void tsbtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                btnAdd.Text = "&Add";
                txtSearch.Clear();
                txtSearch.Focus();
                txtSchemeName.ReadOnly = txtStartDate.ReadOnly = true;
                if (dgrdName.Rows.Count > 0)
                {
                    dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
                    dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                    BindAllDetails(dgrdName.CurrentRow);
                }
            }
            catch { }
        }

        private void tsbtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                SearchResult();
            }
        }


        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                dgrdName.Focus();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
        private void txtname_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdName_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsLetter(e.KeyChar))
                {
                    txtSearch.Focus();
                    txtSearch.Text = txtSearch.Text + e.KeyChar;
                    txtSearch.Select(txtSearch.Text.Length, 0);
                }
                else if (e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Space))
                {
                    txtSearch.Focus();
                    txtSearch.Select(txtSearch.Text.Length, 0);
                }             
            }
            catch { }
        }

        private void dgrdName_MouseClick(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    DataGridViewRow row = dgrdName.CurrentRow;
            //    if (row != null)
            //    {
            //        BindAllDetails(row);
            //    }
            //}
            //catch { }
        }

        private void dgrdName_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (dgrdName.SelectedRows.Count > 0)
                    {
                        if (dgrdName.SelectedRows[0] != null)
                        {
                            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                            {
                                BindAllDetails(dgrdName.SelectedRows[0]);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void UnitMaster_Load(object sender, EventArgs e)
        {
            if (SetPermission())
            {
                if (IsNewScheme)
                {
                    panel2.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                    btnAdd.PerformClick();
                    txtSchemeName.Focus();
                }
            }
        }
      
        private void txtDecimalPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView)
            {
                if (!MainPage.mymainObject.bAccountMasterAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterView)
                    dgrdName.Enabled = txtSearch.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return false;
            }
        }

        private void txtStartDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtEndDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, false, true);
        }
    }
}
