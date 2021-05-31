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
    public partial class ItemCategoryMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewUnit = false;
        public string StrAddedName = "",__strName="";

        public ItemCategoryMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public ItemCategoryMaster(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewUnit = chk;
            __strName = strName;
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select *,Convert(varchar,Date,103)SDate from ItemCategoryMaster Order by CategoryName");
                if (objTable != null)
                {
                    dgrdName.Rows.Clear();
                    if (objTable.Rows.Count > 0)
                    {
                        dgrdName.Rows.Add(objTable.Rows.Count);

                        int rowIndex = 0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            dgrdName.Rows[rowIndex].Cells["id"].Value = dr["Id"];
                            dgrdName.Rows[rowIndex].Cells["categoryName"].Value = dr["CategoryName"];
                            dgrdName.Rows[rowIndex].Cells["FromRange"].Value = dr["FromRange"];
                            dgrdName.Rows[rowIndex].Cells["ToRange"].Value = dr["ToRange"];
                            dgrdName.Rows[rowIndex].Cells["DisPer"].Value = dr["DisPer"];
                            dgrdName.Rows[rowIndex].Cells["Margin"].Value = dr["Margin"];
                            dgrdName.Rows[rowIndex].Cells["Date"].Value = dr["SDate"];
                            dgrdName.Rows[rowIndex].Cells["CreatedBy"].Value = dr["CreatedBy"];
                            dgrdName.Rows[rowIndex].Cells["UpdatedBy"].Value = dr["UpdatedBy"];

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
                txtName.Text = Convert.ToString(row.Cells["categoryName"].Value);
                txtFromRange.Text = Convert.ToString(row.Cells["FromRange"].Value);
                txtToRange.Text = Convert.ToString(row.Cells["ToRange"].Value);
                txtDisPer.Text = Convert.ToString(row.Cells["DisPer"].Value);
                txtDate.Text = Convert.ToString(row.Cells["date"].Value);
                txtMargin.Text = Convert.ToString(row.Cells["Margin"].Value);

                string strCreatedBy=Convert.ToString(row.Cells["CreatedBy"].Value), strUpdatedBy = Convert.ToString(row.Cells["UpdatedBy"].Value);
                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;


                lblId.Text = Convert.ToString(row.Cells["Id"].Value);
                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                DateTime _date = MainPage.currentDate;
                if (txtDate.Text.Length == 10)
                    _date = dba.ConvertDateInExactFormat(txtDate.Text);
              
                string strQuery = "if not Exists (Select CategoryName from ItemCategoryMaster WHere CategoryName='"+txtName.Text+"' and FromRange="+txtFromRange.Text+ " and ToRange=" + txtToRange.Text + ") begin "
                                + " INSERT INTO [dbo].[ItemCategoryMaster] ([CategoryName],[FromRange],[ToRange],[DisPer],[Margin],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                + " ('"+txtName.Text+"',"+txtFromRange.Text+","+txtToRange.Text+","+txtDisPer.Text+","+txtMargin.Text+",'"+_date.ToString("MM/dd/yyyy")+"','"+MainPage.strLoginName+"','',1,0) end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (MainPage._bItemMirroring)
                        dba.DataMirroringInCurrentFinYear(strQuery);

                    MessageBox.Show("Thank you ! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewUnit)
                    {
                        StrAddedName = txtName.Text;
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
            if (txtName.Text == "")
            {
                MessageBox.Show("Sorry ! Category name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            if (txtFromRange.Text == "")
                txtFromRange.Text = "0";
            if (txtToRange.Text == "")
                txtToRange.Text = "10000";
            if (txtDisPer.Text == "")
                txtDisPer.Text = "0";
            if (txtMargin.Text == "")
                txtMargin.Text = "0";
            return true;
        }

        private void EnableAllControl()
        {
            txtName.ReadOnly = txtFromRange.ReadOnly= txtToRange.ReadOnly=txtDisPer.ReadOnly=txtMargin.ReadOnly=txtDate.ReadOnly= false;
            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtName.ReadOnly = txtFromRange.ReadOnly = txtToRange.ReadOnly = txtDisPer.ReadOnly = txtMargin.ReadOnly = txtDate.ReadOnly = true;
            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtName.Text = txtSearch.Text = lblMsg.Text = "";
            txtFromRange.Text = txtToRange.Text = txtDisPer.Text = txtMargin.Text = "0";

            txtDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
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
                    txtName.Focus();
                    ClearAllText();
                    EnableAllControl();                    
                    btnAdd.Text = "&Save";
                }
                else
                {
                    if (ValidateControls())
                    {
                        if (CheckAvailability(txtName.Text, txtFromRange.Text, txtToRange.Text))
                        {
                            DialogResult dar = MessageBox.Show("Are you sure you want to Save Record ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dar == DialogResult.Yes)
                            {
                                SaveRecord();
                            }
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

                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from ItemCategoryMaster Where ID=" + lblId.Text + "");
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
                        string strOldName = Convert.ToString(row.Cells["categoryName"].Value), strFromRange = Convert.ToString(row.Cells["fromRange"].Value), strToRange = Convert.ToString(row.Cells["toRange"].Value);
                        if (strFromRange == "")
                            strFromRange = "0";
                        if (strToRange == "")
                            strToRange = "0";
                        if (CheckAvailability(txtName.Text, strFromRange, strToRange))
                        {
                            DateTime _date = MainPage.currentDate;
                            if (txtDate.Text.Length == 10)
                                _date = dba.ConvertDateInExactFormat(txtDate.Text);

                            string strQuery = "Update [dbo].[ItemCategoryMaster]  Set [CategoryName]='" + txtName.Text + "',[FromRange]=" + txtFromRange.Text + ",[ToRange]=" + txtToRange.Text + ",[DisPer]=" + txtDisPer.Text + ",[Margin]=" + txtMargin.Text + ",[Date]='" + _date.ToString("MM/dd/yyyy") + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=0 Where CategoryName='" + strOldName + "' and ISNULL(FromRange,0)=" + strFromRange + " and ISNULL(ToRange,0)=" + strToRange + "  ";
                            if (txtName.Text != strOldName)
                                strQuery += " Update [Items] Set [Other]='" + txtName.Text + "' Where [Other]='" + strOldName + "'  ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                                    dba.DataMirroringInCurrentFinYear(strQuery);

                                MessageBox.Show("Thank you ! Record is updated successfully !", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                lblMsg.Text = "";
                                btnEdit.Text = "&Edit";
                                BindDataGrid();
                            }
                            else
                                MessageBox.Show("Sorry ! your Record is not Updated..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please select right  list .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (ValidateControls() && lblId.Text != "")
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

        private bool CheckAvailability(string strCategoryName,string strFromRange,string strToRange)
        {           
            try
            {
                if (txtName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select CategoryName from ItemCategoryMaster  Where CategoryName='" + strCategoryName + "' and FromRange=" + strFromRange + " and ToRange=" + strToRange+" ");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Category Name : " + txtName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select CategoryName from ItemCategoryMaster Where CategoryName='" + strCategoryName + "' and FromRange=" + strFromRange + " and ToRange=" + strToRange + " and ID !=" + lblId.Text + " ");
                       // DataTable MyTable = dba.GetDataTable("Select CSize from CartoneSize Where CSize ='" + txtName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Category Name : " + txtName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Category Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtName.Focus();
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
                txtName.ReadOnly = true;
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
                    panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;                  
                    btnAdd.PerformClick();
                    txtName.Text = __strName;
                    txtName.Focus();
                }
            }
        }



        private bool SetPermission()
        {
            if (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bAccountMasterView)
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

        private void txtFromRange_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void ItemCategoryMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            }
            catch { }
        }
    }
}
