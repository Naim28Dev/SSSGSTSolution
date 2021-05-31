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
    public partial class GroupMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewUnit = false;
        public string StrAddedMaster = "", strSelectedItem="",__strGroup="";

        public GroupMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public GroupMaster(bool chk,string strGRP)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewUnit = chk;
            __strGroup = strGRP;
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select * from GroupMaster order by GroupName");
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
                            dgrdName.Rows[rowIndex].Cells["groupName"].Value = dr["GroupName"];
                          
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
                strSelectedItem = txtGroupName.Text = Convert.ToString(row.Cells["groupName"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);
                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                string strQuery = " if not exists (Select GroupName from GroupMaster Where GroupName='" + txtGroupName.Text + "') begin "
                                + " Insert into GroupMaster ([GroupName],[Date],[InsertStatus],[UpdateStatus]) values('" + txtGroupName.Text + "','" + MainPage.currentDate + "',1,0) end ";


                int rd = dba.ExecuteMyQuery(strQuery);
                if (rd > 0)
                {
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewUnit)
                    {
                        StrAddedMaster = txtGroupName.Text;
                        this.Close();
                    }
                    else
                    {                       
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
            if (txtGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! Group name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtGroupName.Focus();
                return false;
            }
            return true;
        }

        private void EnableAllControl()
        {
            txtGroupName.ReadOnly =  false;
            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtGroupName.ReadOnly =  true;
            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtGroupName.Text = txtSearch.Text = lblMsg.Text = ""; 
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
                    btnEdit.Text = "&Edit";
                    txtGroupName.Focus();
                    ClearAllText();
                    EnableAllControl();                    
                    btnAdd.Text = "&Save";
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Unit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from GroupMaster Where ID=" + lblId.Text + "");
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
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text && strSelectedItem!="")
                    {
                        string strQuery = " Update GroupMaster Set GroupName='" + txtGroupName.Text + "' Where GroupName='" + strSelectedItem + "' ";
                        strQuery += " Update AddressBook Set GroupName='" + txtGroupName.Text + "' Where GroupName='" + strSelectedItem + "' ";
                        strQuery += " Update SupplierMaster Set GroupII='" + txtGroupName.Text + "' Where GroupII='" + strSelectedItem + "' ";

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record is updated successfully !", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataGrid();
                        }
                        else
                            MessageBox.Show("Sorry ! your Record is not Updated..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Sorry ! Please select right Unit in deparment list .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Unit Master.", ex.Message };
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

                        btnAdd.Text = "&Add";
                        BindDataGrid();
                    }
                    btnEdit.Text = "&Update";
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
                if (txtGroupName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select GroupName from GroupMaster Where GroupName ='" + txtGroupName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Group Name : "+txtGroupName.Text+" already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtGroupName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtGroupName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select GroupName from GroupMaster Where GroupName ='" + txtGroupName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Group Name : " + txtGroupName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtGroupName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtGroupName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Group Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtGroupName.Focus();
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
                        DataRow[] row = objTable.Select(String.Format("GroupName Like ('%" + txtSearch.Text + "%')"));
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
                txtGroupName.ReadOnly =  true;
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
            try
            {
                if ((Char.IsWhiteSpace(e.KeyChar) && (txtSearch.Text.Length < 1 || (txtSearch.Text.Length <= txtSearch.SelectionLength || txtSearch.SelectionStart == 0))))
                {
                    e.Handled = true;
                }
            }
            catch { }
        }
        private void txtname_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((Char.IsWhiteSpace(e.KeyChar) && (txtGroupName.Text.Length < 1 || (txtGroupName.Text.Length <= txtGroupName.SelectionLength || txtGroupName.SelectionStart == 0))))
                {
                    e.Handled = true;
                }
            }
            catch { }
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
                if (IsNewUnit)
                {
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    panel2.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                    btnAdd.PerformClick();
                    txtGroupName.Text = __strGroup;
                    txtGroupName.Focus();
                }
            }
        }
      
        private void txtDecimalPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bGSTMasterEntry || MainPage.mymainObject.bGSTMasterView || MainPage.mymainObject.bGSTMasterEditDelete)
            {
                if (!MainPage.mymainObject.bGSTMasterEntry)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bGSTMasterEditDelete)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bGSTMasterView)
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
    }
}
