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
    public partial class CategoryMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewCategory = false;
        public string StrAddedCategory = "",strGroupName="",_strCategory="";

        public CategoryMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public CategoryMaster(bool chk,string strGName,string strCat)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            strGroupName = strGName;
            _strCategory = strCat;
            IsNewCategory = chk;            
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select * from Category Order by CategoryName");
                if (objTable != null)
                {
                    dgrdName.Rows.Clear();
                    if (objTable.Rows.Count > 0)
                    {
                        dgrdName.Rows.Add(objTable.Rows.Count);

                        int rowIndex = 0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            dgrdName.Rows[rowIndex].Cells["id"].Value = dr["CategoryID"];
                            dgrdName.Rows[rowIndex].Cells["categoryName"].Value = dr["CategoryName"];
                            dgrdName.Rows[rowIndex].Cells["groupName"].Value = dr["GroupName"];
                            dgrdName.Rows[rowIndex].Cells["depreciationPoint"].Value = dr["DepreciationPer"];
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
                txtCategoryName.Text = Convert.ToString(row.Cells["categoryName"].Value);
                txtGroupName.Text = Convert.ToString(row.Cells["groupName"].Value);
                txtDepeciationPer.Text = Convert.ToString(row.Cells["depreciationPoint"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);

                if (txtGroupName.Text == "FIXED ASSETS" || txtGroupName.Text == "FURNITURE / OFFICE ASSETS")
                    lblDepreciation.Visible = txtDepeciationPer.Visible = true;
                else
                    lblDepreciation.Visible = txtDepeciationPer.Visible = false;

                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                string Insertquery = " if not exists (Select CategoryName from Category Where CategoryName='" + txtCategoryName.Text+ "') begin Insert into Category ([CategoryName],[GroupName],[DepreciationPer],[InsertStatus],[UpdateStatus]) Values('" + txtCategoryName.Text + "','" + txtGroupName.Text + "'," + dba.ConvertObjectToDouble(txtDepeciationPer.Text) + ",1,0) end ";
                int _count = dba.ExecuteMyQuery(Insertquery);
                if (_count > 0)
                {
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (IsNewCategory)
                    {
                        StrAddedCategory = txtCategoryName.Text;
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
            if (txtGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! Group Name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtGroupName.Focus();
                return false;
            }
            return true;
        }

        private void EnableAllControl()
        {
            txtCategoryName.ReadOnly = txtDepeciationPer.ReadOnly = false;
            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtCategoryName.ReadOnly =txtDepeciationPer.ReadOnly = true;
            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtCategoryName.Text = txtGroupName.Text = txtDepeciationPer.Text = txtSearch.Text = lblMsg.Text = ""; 
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
                    txtCategoryName.Focus();
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from [Category] Where CategoryID=" + lblId.Text + "");
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
                        string strOldCategoryName = Convert.ToString(row.Cells["categoryName"].Value);

                        string query = " Update [dbo].[Category] set [CategoryName]='" + txtCategoryName.Text + "', [GroupName]='" + txtGroupName.Text + "',[DepreciationPer]=" + dba.ConvertObjectToDouble(txtDepeciationPer.Text) + ",[UpdateStatus]=0 Where [CategoryName]='" + strOldCategoryName + "'"
                        + " Update SupplierMaster Set Category='" + txtCategoryName.Text + "' Where Category='" + strOldCategoryName + "' ";
                        //+ " Update DesignMaster Set ItemName='" + txtUnitName.Text + "' Where ItemUnit='" + row.Cells["UnitCode"].Value + "' ";

                        string strNetQuery = "";
                        if (txtCategoryName.Text != strOldCategoryName)
                        {
                            strNetQuery = " if not exists (Select CategoryName from [dbo].[Category] Where [CategoryName]='" + txtCategoryName.Text + "') begin  "
                                        + " Update [dbo].[Category] set [CategoryName]='" + txtCategoryName.Text + "', [GroupName]='" + txtGroupName.Text + "',[DepreciationPer]=" + dba.ConvertObjectToDouble(txtDepeciationPer.Text) + ",[UpdateStatus]=0 Where [CategoryName]='" + strOldCategoryName + "' end ";
                        }
                        else
                            strNetQuery += " Update [dbo].[Category] set [CategoryName]='" + txtCategoryName.Text + "', [GroupName]='" + txtGroupName.Text + "',[DepreciationPer]=" + dba.ConvertObjectToDouble(txtDepeciationPer.Text )+ ",[UpdateStatus]=0 Where [CategoryName]='" + strOldCategoryName + "'";
                      
                                strNetQuery += " Update SupplierMaster Set Category='" + txtCategoryName.Text + "' Where Category='" + strOldCategoryName + "' ";

                        int count = dba.ExecuteMyQuery(query);
                        if (count > 0)
                        {
                            DataBaseAccess.CreateDeleteQuery(strNetQuery);
                            MessageBox.Show("Thank you ! Record is updated successfully !", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataGrid();
                        }
                        else
                            MessageBox.Show("Sorry ! your Record is not updated..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Sorry ! Please select right Category Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Category Master.", ex.Message };
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
                if (txtCategoryName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [CategoryName] from [dbo].[Category] Where [CategoryName] ='" + txtCategoryName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Category Name : "+txtCategoryName.Text+" already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtCategoryName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtCategoryName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [CategoryName] from [dbo].[Category] Where [CategoryName] ='" + txtCategoryName.Text + "' and CategoryID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Category Name : " + txtCategoryName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtCategoryName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtCategoryName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Category Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtCategoryName.Focus();
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
                        DataRow[] row = objTable.Select(String.Format("CategoryName Like ('%" + txtSearch.Text + "%')"));
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
                txtCategoryName.ReadOnly = txtGroupName.ReadOnly = true;
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
                if (IsNewCategory)
                {
                    panel2.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    btnAdd.PerformClick();
                    if (txtGroupName.Text == "FIXED ASSETS" || txtGroupName.Text == "FURNITURE / OFFICE ASSETS")
                        lblDepreciation.Visible = txtDepeciationPer.Visible = true;
                    else
                        lblDepreciation.Visible = txtDepeciationPer.Visible = false;
                    txtGroupName.Text = strGroupName;
                    txtCategoryName.Text = _strCategory;
                    txtCategoryName.Focus();

                }
            }
        }
      
        private void txtDecimalPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bPartyMasterView)
            {
                if (!MainPage.mymainObject.bPartyMasterAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPartyMasterEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bPartyMasterView)
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

        private void txtFormalName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtGroupName.Text = strData;
                        if (txtGroupName.Text == "FIXED ASSETS" || txtGroupName.Text == "FURNITURE / OFFICE ASSETS")
                            lblDepreciation.Visible = txtDepeciationPer.Visible = true;
                        else
                            lblDepreciation.Visible = txtDepeciationPer.Visible = false;
                    }
                }
            }
            catch
            {
            }

        }
    }
}
