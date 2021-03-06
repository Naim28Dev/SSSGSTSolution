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
    public partial class StationMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewUnit = false;
        public string StrAddedName = "",__strStation="";

        public StationMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public StationMaster(bool chk,string strSt)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewUnit = chk;
            __strStation = strSt;
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select * from Station Order by StationName");
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
                            dgrdName.Rows[rowIndex].Cells["itemName"].Value = dr["StationName"];
                            //dgrdName.Rows[rowIndex].Cells["formalName"].Value = dr["FormalName"];
                            //dgrdName.Rows[rowIndex].Cells["decimalPoint"].Value = dr["DecimalPoint"];
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
                txtName.Text = Convert.ToString(row.Cells["itemName"].Value);
              //  txtFormalName.Text = Convert.ToString(row.Cells["formalName"].Value);
                //txtDecimalPoint.Text = Convert.ToString(row.Cells["decimalPoint"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);
                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
               // string Insertquery = "Insert into UnitMaster (UnitName,FormalName,DecimalPoint) values('" + txtName.Text + "','" + txtFormalName.Text + "','"+txtDecimalPoint.Text+"')";
                int rd = dba.SaveNewStation(txtName.Text);
                if (rd > 0)
                {
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (IsNewUnit)
                    {
                        StrAddedName = txtName.Text;
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
            //if (txtDecimalPoint.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Decimal points can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    txtDecimalPoint.Focus();
            //    return false;
            //}
            return true;
        }

        private void EnableAllControl()
        {
            txtName.ReadOnly =  false;
            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtName.ReadOnly =  true;
            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtName.Text =  txtSearch.Text = lblMsg.Text = ""; 
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from Station Where ID=" + lblId.Text + "");
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
                        string strOldName = Convert.ToString(row.Cells["itemName"].Value);

                        int count = dba.UpdateStation(txtName.Text, strOldName);
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
                        MessageBox.Show("Sorry ! Please select right  list .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Station Master.", ex.Message };
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
                if (txtName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select StationName from Station Where StationName ='" + txtName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Station Name : " + txtName.Text + " already exist ! ";
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
                        DataTable MyTable = dba.GetDataTable("Select StationName from Station Where StationName ='" + txtName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Station Name : " + txtName.Text + " already exist ! ";
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
                    lblMsg.Text = "Sorry ! Station Name can't be blank ! ";
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
                        DataRow[] row = objTable.Select(String.Format("StationName Like ('%" + txtSearch.Text + "%')"));
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
                    txtName.Text = __strStation;
                    txtName.Focus();
                }
            }
        }
      
        private void txtDecimalPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private bool SetPermission()
        {
            if (MainPage.mymainObject.bPartyMasterAdd || MainPage.mymainObject.bPartyMasterEdit || MainPage.mymainObject.bAccountMasterView)
            {
                if (!MainPage.mymainObject.bPartyMasterAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPartyMasterEdit)
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                btnDownload.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download station name ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    if (MainPage.strLiveDataBaseIP != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        int count = dba.DownloadStationMaster(MainPage.strOnlineDataBaseName);
                        if (count > 0)
                        {
                            MessageBox.Show(" Thank you ! New station master downloaded successfully... ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
            btnDownload.Enabled = true;
        }
    }
}
