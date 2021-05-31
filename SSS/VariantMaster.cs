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
    public partial class VariantMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewCategory = false;
        public string StrAddedCategory = "";
        string strVarietyNumber = "1", strCatName = "",__strName="";

        public VariantMaster(string strNo,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strVarietyNumber = strNo;
            strCatName = strName;          
            SetCategoryValue();
            BindDataGrid();          
        }
        public VariantMaster(string strNo,string strName,bool chk,string strVName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strVarietyNumber = strNo;
            strCatName = strName;
            __strName = strVName;
            IsNewCategory = chk;
            SetCategoryValue();
            BindDataGrid();
                    
        }

        private void SetCategoryValue()
        {
            lblSearchHeader.Text = "Search " + strCatName;
            dgrdName.Columns["categoryName"].HeaderText =lblTextHeader.Text= strCatName + " Name";
            lblNameHeader.Text = strCatName.ToUpper() + " MASTER";
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select * from VariantMaster" + strVarietyNumber + " Order by Variant" + strVarietyNumber + " ");
                if (objTable != null && !IsNewCategory)
                {
                    dgrdName.Rows.Clear();
                    if (objTable.Rows.Count > 0)
                    {
                        dgrdName.Rows.Add(objTable.Rows.Count);

                        int rowIndex = 0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            dgrdName.Rows[rowIndex].Cells["id"].Value = dr["ID"];
                            dgrdName.Rows[rowIndex].Cells["categoryName"].Value = dr["Variant" + strVarietyNumber + ""];
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
                lblId.Text = Convert.ToString(row.Cells["ID"].Value);
                txtCategoryName.ReadOnly = true;
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                string Insertquery = "Insert into VariantMaster" + strVarietyNumber + " (Variant" + strVarietyNumber + ",[Remark],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) Values "
                                   + " ('" + txtCategoryName.Text + "','',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'"+MainPage.strLoginName+"','',1,0)";
                int rd = dba.ExecuteMyQuery(Insertquery);
                if (rd > 0)
                {
                    if (MainPage._bItemMirroring)
                        dba.DataMirroringInCurrentFinYear(Insertquery);
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewCategory)
                    {
                        StrAddedCategory = txtCategoryName.Text;
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
            if (txtCategoryName.Text == "")
            {
                MessageBox.Show("Sorry ! Name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }
         
            return true;
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
                    txtCategoryName.ReadOnly = false;                 
                    txtCategoryName.Clear();                 
                    txtCategoryName.Focus();
                    lblMsg.Text = "";
                    btnAdd.Text = "&Save";
                }
                else
                {
                    if (CheckAvailability() && ValidateControls())
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to save record ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
            }
            catch { }
        }

        private void CategoryMaster_KeyDown(object sender, KeyEventArgs e)
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
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bCategoryView)
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record !", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from VariantMaster" + strVarietyNumber + " where ID=" + lblId.Text + "");
                        if (i > 0)
                        {
                            MessageBox.Show("Record is deleted successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";                           
                            txtCategoryName.Clear();
                            txtSearch.Clear();
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
                    string strVariantName = Convert.ToString(row.Cells["categoryName"].Value);
                    if (strVariantName != "")
                    {
                        string query = " Update VariantMaster" + strVarietyNumber + " Set Variant" + strVarietyNumber + "='" + txtCategoryName.Text + "',Date=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),UpdatedBy='" + MainPage.strLoginName + "' Where Variant" + strVarietyNumber + "='" + strVariantName + "' ";

                        int count = dba.ExecuteMyQuery(query);
                        if (count > 0)
                        {
                            if (MainPage._bItemMirroring)
                                dba.DataMirroringInCurrentFinYear(query);

                            DataBaseAccess.CreateDeleteQuery(query);
                            MessageBox.Show("Thank you !! Record is updated successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataGrid();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Record is not updated.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
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
                    txtCategoryName.ReadOnly = false;                    
                }
                else
                {
                    if (CheckAvailability() && ValidateControls() && lblId.Text != "")
                    {
                        DialogResult dir = MessageBox.Show("Are you sure you want to update record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                        DataTable MyTable = dba.GetDataTable("Select Variant" + strVarietyNumber + " from VariantMaster" + strVarietyNumber + " Where Variant" + strVarietyNumber + " ='" + txtCategoryName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry !  Name : "+txtCategoryName.Text+" already exist ! ";
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
                        DataTable MyTable = dba.GetDataTable("Select Variant" + strVarietyNumber + " from VariantMaster" + strVarietyNumber + " Where Variant" + strVarietyNumber + " ='" + txtCategoryName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Name : " + txtCategoryName.Text + " already exist ! ";
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
                    lblMsg.Text = "Sorry ! Name can't be blank ! ";
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
                        DataRow[] row = objTable.Select(String.Format("Variant" + strVarietyNumber + " Like ('%" + txtSearch.Text + "%')"));
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
                txtCategoryName.ReadOnly =  true;
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

        private void dgrdName_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text=="&Edit")
                {
                    SearchResult();
                }                
            }
            catch { }
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
                if ((Char.IsWhiteSpace(e.KeyChar) && (txtCategoryName.Text.Length < 1 || (txtCategoryName.Text.Length <= txtCategoryName.SelectionLength || txtCategoryName.SelectionStart == 0))))
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

        private void CategoryMaster_Load(object sender, EventArgs e)
        {
            try {
                if (SetPermission())
                {
                    if (IsNewCategory)
                    {
                        btnAdd.PerformClick();
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                        txtCategoryName.Text = __strName;
                        txtCategoryName.Focus();
                    }
                }
            }
            catch { }
        }

        private void txtCategoryName_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
                btnAdd.Focus();
            else
                btnEdit.Focus();
        }

        private void txtCategoryName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bCategoryView)
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
      
    }
}
