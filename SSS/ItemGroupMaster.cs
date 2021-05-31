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
    public partial class ItemGroupMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewGroup = false,bPGroup=false;
        public string StrAddedGroup = "",strSelectedGroupName="",__strName="";
        public ItemGroupMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }
        public ItemGroupMaster(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewGroup = chk;
            __strName = strName;
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select Distinct ID,GroupName,TaxCategoryName,ParentGroup,HSNCode,Other,(Select COUNT(*) from Items _IM Where _IM.GroupName=_IGM.GroupName) SaleCount  from ItemGroupMaster _IGM Where _IGM.ParentGroup='' Order by _IGM.GroupName ");
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
                            dgrdName.Rows[rowIndex].Cells["taxCategory"].Value = dr["TaxCategoryName"];
                           // dgrdName.Rows[rowIndex].Cells["parentGroup"].Value = dr["ParentGroup"];
                            dgrdName.Rows[rowIndex].Cells["saleCount"].Value = dr["SaleCount"];
                            dgrdName.Rows[rowIndex].Cells["hsnCode"].Value = dr["HSNCode"];
                            dgrdName.Rows[rowIndex].Cells["other"].Value = dr["Other"];
                            rowIndex++;
                        }

                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex-1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        if (dgrdName.Rows.Count == 1)
                        {
                            string strGroup = Convert.ToString(dgrdName.Rows[0].Cells["groupName"].Value);
                            BindAllDetails(dgrdName.Rows[0]);
                        }
                    }                    
                }
            }
            catch { }
        }

        private void BindAllDetails(DataGridViewRow row)
        {
            try
            {
                if (row != null)
                {
                    strSelectedGroupName = txtGroupName.Text = Convert.ToString(row.Cells["groupName"].Value);
                    txtTaxCategory.Text = Convert.ToString(row.Cells["taxCategory"].Value);
                    txtHSNCode.Text = Convert.ToString(row.Cells["hsnCode"].Value);
                    lblId.Text = Convert.ToString(row.Cells["id"].Value);
                    txtGroupName.ReadOnly = true;

                    if (Convert.ToString(row.Cells["other"].Value) == "SAC")
                        rdoSAC.Checked = true;
                    else
                        rdoHSN.Checked = true;

                    if (dba.ConvertObjectToDouble(row.Cells["saleCount"].Value) > 0 && !MainPage.strUserRole.Contains("SUPERADMIN"))
                    {
                        btnDelete.Enabled = txtTaxCategory.Enabled = false;
                        lblWarning.Text = "Tax category can't be change,\nOnce sale bill generated of this item ";
                    }
                    else
                    {
                        btnDelete.Enabled = txtTaxCategory.Enabled = true;
                        lblWarning.Text = "";
                    }
                }
                SetPermission();
            }
            catch { }
        }

        private string GetHSNType()
        {
            if (rdoHSN.Checked)
                return "HSN";
            else
                return "SAC";
        }


               
        private void SaveRecord()
        {
            try
            {
                string strQuery = "if not exists (Select GroupName from ItemGroupMaster WHere GroupName='" + txtGroupName.Text + "' ) begin "
                                  + " Insert into ItemGroupMaster ([GroupName],[CategoryName],[ParentGroup],[HSNCode],[Other],[InsertStatus],[UpdateStatus],[TaxCategoryName],[TaxRate]) Values "
                                  + " ('" + txtGroupName.Text + "','','','" + txtHSNCode.Text + "','"+GetHSNType()+"',1,0,'"+txtTaxCategory.Text+"',5)  end ";
          
                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    if (MainPage._bItemMirroring)
                        dba.DataMirroringInCurrentFinYear(strQuery);

                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewGroup)
                    {
                        StrAddedGroup = txtGroupName.Text;
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
            //if (txtCategoryName.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Category name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    txtCategoryName.Focus();
            //    return false;
            //}
            //if (txtParentGroup.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Parent Group can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    txtParentGroup.Focus();
            //    return false;
            //}
            if (txtHSNCode.Text == "")
            {
                MessageBox.Show("Sorry ! HSN code can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtHSNCode.Focus();
                return false;
            }
            if (MainPage.strSoftwareType == "AGENT")
            {
                if (txtHSNCode.Text.Length < 6)
                {
                    MessageBox.Show("Sorry ! HSN code must be atleast 6 digit ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    txtHSNCode.Focus();
                    return false;
                }
            }
            else if (txtHSNCode.Text.Length < 4)
            {
                MessageBox.Show("Sorry ! HSN code must be atleast 4 digit ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtHSNCode.Focus();
                return false;
            }
            return true;
        }

        private void ClearAllText()
        {
            txtGroupName.Text =  strSelectedGroupName = txtHSNCode.Text =txtTaxCategory.Text = lblWarning.Text = ""; //txtCategoryName.Text =
            rdoHSN.Checked = true;
           
        }

        private void EnableAllControl()
        {
            txtGroupName.ReadOnly = txtHSNCode.ReadOnly = false;
            rdoHSN.Enabled = rdoSAC.Enabled = true;
        }

        private void DisableAllControl()
        {
            txtGroupName.ReadOnly = txtHSNCode.ReadOnly = true;
            rdoHSN.Enabled = rdoSAC.Enabled = false;
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
                    ClearAllText();
                    EnableAllControl();
                    txtGroupName.Focus();
                    lblMsg.Text = "";
                    btnAdd.Text = "&Save";
                    txtTaxCategory.Enabled = btnDelete.Enabled = true;
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter )
                {
                    SendKeys.Send("{TAB}");
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    if (dgrdName.CurrentRow.Index > 0 && btnAdd.Text!="&Save" && btnEdit.Text!="&Update")
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[dgrdName.CurrentRow.Index - 1].Cells[1];
                        txtSearch.Focus();
                    }
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    if (dgrdName.CurrentRow.Index < dgrdName.Rows.Count && btnAdd.Text != "&Save" && btnEdit.Text != "&Update")
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[dgrdName.CurrentRow.Index+1].Cells[1];
                        txtSearch.Focus();
                    }
                }
                else if (e.KeyCode == Keys.Home)
                {
                    if (btnAdd.Text != "&Save" && btnEdit.Text != "&Update")
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[0].Cells[1];
                        txtSearch.Focus();
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    if (btnAdd.Text != "&Save" && btnEdit.Text != "&Update")
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
                        txtSearch.Focus();
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record !", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " if not Exists (Select ItemName from Items Where GroupName='" + strSelectedGroupName + "') begin Delete from ItemGroupMaster WHere GroupName='" + strSelectedGroupName + "' and ParentGroup='' end ";
                        int i = dba.ExecuteMyQuery(strQuery);
                       // int i = dba.ExecuteMyQuery();
                        if (i > 0)
                        {
                            MessageBox.Show("Record is deleted Successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            ClearAllText();
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

        private int UpdateRecord(string strQuery)
        {
            int count = 0;
            try
            {
                //DataGridViewRow row = dgrdName.SelectedRows[0];
                if (lblId.Text != "")
                {
                    if (lblId.Text != "")
                    {

                        string query = "", strID = "";

                        query += "Update ItemGroupMaster set GroupName='" + txtGroupName.Text + "',[CategoryName]='',[HSNCode]='" + txtHSNCode.Text + "',[TaxCategoryName]='" + txtTaxCategory.Text + "',[Other]='" + GetHSNType() + "',UpdateStatus=1 Where GroupName='" + strSelectedGroupName + "' ";

                        if (txtGroupName.Text != strSelectedGroupName)
                        {
                            query += " Update Items set [GroupName]='" + txtGroupName.Text + "' where [GroupName]='" + strSelectedGroupName + "'   ";
                        }
                        query += strQuery;

                        count = dba.ExecuteMyQuery(query);
                        if (count > 0)
                        {
                            if (MainPage._bItemMirroring)
                                dba.DataMirroringInCurrentFinYear(strQuery);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Group Master.", ex.Message };
                count = 0;
            }
            return count;
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
                           int count= UpdateRecord("");
                           if (count > 0)
                           {
                               MessageBox.Show("Record is updated Successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               lblMsg.Text = "";
                               btnEdit.Text = "&Edit";
                               BindDataGrid();
                           }
                           else
                           {
                               MessageBox.Show("Sorry ! your Record is not Updated..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           }
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
                        DataTable MyTable = dba.GetDataTable("Select GroupName from ItemGroupMaster Where GroupName ='" + txtGroupName.Text + "'");
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

                        DataTable MyTable = dba.GetDataTable("Select GroupName from ItemGroupMaster Where GroupName ='" + txtGroupName.Text + "' and ID not in (" + lblId.Text + ") ");
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
                            dgrdName.Rows[rowIndex].Selected = true;
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

        private void dgrdName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                int rowIndex = dgrdName.CurrentRow.Index;
                if (e.KeyCode == Keys.Down)
                {
                    if (rowIndex < dgrdName.Rows.Count - 1)
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex+1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        //if (dgrdName.Rows.Count == 1)
                        //    BindAllDetails(Convert.ToString(dgrdName.Rows[0].Cells["groupName"].Value));
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    if (rowIndex >0)
                    {
                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex - 1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        //if (dgrdName.Rows.Count == 1)
                        //    BindAllDetails(Convert.ToString(dgrdName.Rows[0].Cells["groupName"].Value));
                    }
                }
            }
            catch 
            { 
            }
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
                                if (dgrdName.SelectedRows.Count > 0)
                                    BindAllDetails(dgrdName.SelectedRows[0]);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void GroupMaster_Load(object sender, EventArgs e)
        {
            if (SetPermission())
            {
                if (IsNewGroup)
                {
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    panSearch.TabStop =pangrid.TabStop= txtSearch.TabStop = dgrdName.TabStop = false;
                    btnAdd.PerformClick();
                    txtGroupName.Text = __strName;
                    txtGroupName.Focus();                    
                }
            }
        }

        private void txtGroupName_Leave(object sender, EventArgs e)
        {
            //if (btnAdd.Text == "&Save")
            //    btnAdd.Focus();
            //else
            //    btnEdit.Focus();
        }

        private void txtGroupName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
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
                    txtSearch.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(Close));
                return false;
            }
        }

        private void btnDownloadMaster_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownloadMaster.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download merged master ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadMergedMaster(MainPage.strOnlineDataBaseName, "GROUPNAME");
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! Master merged successfully... ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("No master found  ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Sorry ! Please enter online databse name and Live IP in company master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
            btnDownloadMaster.Enabled = true;
        }

        private void txtHSNCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void lnkCheckHSN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.SetText(txtHSNCode.Text);
                System.Diagnostics.Process.Start("https://services.gst.gov.in/services/searchhsnsac");
            }
            catch { }
        }

        private void ItemGroupMaster_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        //SearchData objSearch = new SearchData("CATEGORYNAME", "SEARCH CATEGORY NAME", e.KeyCode);
                        //objSearch.ShowDialog();
                        //txtCategoryName.Text = objSearch.strSelectedData;
                        //e.Handled = true;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        //private void txtParentGroup_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
        //        {
        //            char objChar = Convert.ToChar(e.KeyCode);
        //            int value = e.KeyValue;
        //            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //            {
        //                SearchData objSearch = new SearchData("ITEMGROUPNAME", "SEARCH PARENT GROUP NAME", e.KeyCode);
        //                objSearch.ShowDialog();
        //                txtParentGroup.Text = objSearch.strSelectedData;
        //                e.Handled = true;
        //            }
        //            else
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void txtTaxCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TAXCATEGORYNAME","GOODS", "SEARCH TAX CATEGORY", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTaxCategory.Text = objSearch.strSelectedData;
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }      
     

    }
}
