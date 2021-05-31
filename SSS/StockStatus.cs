using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace SSS
{
    public partial class StockStatus : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        int _chkShowZeroStockItems, _chkShowParentGroup, _chkShowValueOfItem, _chkShowMRPAlso = 0;        

        public StockStatus()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
        }


        private void BindDataGrid()
        {
            try
            {
                string query = "Select ID,MaterialCenter,(CONVERT(varchar,ReportDate,103)) ReportDate,ItemShownBy,ShowZeroStockItems,ShowParentGroup,ShowValueOfItem,ShowMRPAlso, "
                    + " Remark,CreatedBy,UpdatedBy,InsertStatus,UpdateStatus from StockStatus Order by MaterialCenter";
                objTable = dba.GetDataTable(query);
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
                            dgrdName.Rows[rowIndex].Cells["MaterialCenter"].Value = dr["MaterialCenter"];
                            dgrdName.Rows[rowIndex].Cells["ReportDate"].Value = dr["ReportDate"];
                            dgrdName.Rows[rowIndex].Cells["ItemShownBy"].Value = dr["ItemShownBy"];                          
                            dgrdName.Rows[rowIndex].Cells["ShowZeroStockItems"].Value = dr["ShowZeroStockItems"];
                            dgrdName.Rows[rowIndex].Cells["ShowParentGroup"].Value = dr["ShowParentGroup"];
                            dgrdName.Rows[rowIndex].Cells["ShowValueOfItem"].Value = dr["ShowValueOfItem"];
                            dgrdName.Rows[rowIndex].Cells["ShowMRPAlso"].Value = dr["ShowMRPAlso"];
                            dgrdName.Rows[rowIndex].Cells["Remark"].Value = dr["Remark"];
                            dgrdName.Rows[rowIndex].Cells["CreatedBy"].Value = dr["CreatedBy"];
                            dgrdName.Rows[rowIndex].Cells["UpdatedBy"].Value = dr["UpdatedBy"];
                            dgrdName.Rows[rowIndex].Cells["InsertStatus"].Value = dr["InsertStatus"];
                            dgrdName.Rows[rowIndex].Cells["UpdateStatus"].Value = dr["UpdateStatus"];

                            rowIndex++;
                        }
                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex - 1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        if (dgrdName.Rows.Count == 1) { }
                        BindAllDetails(dgrdName.Rows[0]);
                    }
                }
            }
            catch { }
        }

        private void SaveRecord()
        {
            try
            {
                if (chkShowZeroStockItems.Checked == true)
                {
                    _chkShowZeroStockItems = 1;
                }
                if (chkShowParentGroup.Checked == true)
                {
                    _chkShowParentGroup = 1;
                }
                if (chkShowValueOfItem.Checked == true)
                {
                    _chkShowValueOfItem = 1;
                }
                if (chkShowMRPAlso.Checked == true)
                {
                    _chkShowMRPAlso = 1;
                }

                string strQuery = " if not exists (Select MaterialCenter from [dbo].[StockStatus] Where MaterialCenter='" + txtMaterialCenter.Text.Trim() + "') "
                 + "begin INSERT INTO [dbo].[StockStatus] ([MaterialCenter],[ReportDate],[ItemShownBy],[ShowZeroStockItems],[ShowParentGroup],[ShowValueOfItem],[ShowMRPAlso], "
                 + "[Remark],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                 + "('" + txtMaterialCenter.Text.Trim() + "','" + txtReportDate.Text.Trim() + "','" + txtItemShownBy.Text.Trim() + "','" + _chkShowZeroStockItems + "', "
                 + " '" + _chkShowParentGroup + "','" + _chkShowValueOfItem + "','" + _chkShowMRPAlso + "', '" + txtRemark.Text.Trim() + "', "
                 + " DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',1,0) "

                 + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                    + "('" + txtMaterialCenter.Text.Trim() + "','STOCKSTATUS',0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + "0" + ",'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    //DataBaseAccess.CreateDeleteQuery(strQuery);
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnAdd.Text = "&Add";
                    BindDataGrid();
                    lblMsg.Text = "";
                    dgrdName.Focus();

                }
            }
            catch { }
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
                    txtMaterialCenter.Focus();
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

        private bool CheckAvailability()
        {
            try
            {
                if (txtMaterialCenter.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [MaterialCenter] from [StockStatus] Where [MaterialCenter] ='" + txtMaterialCenter.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Material Center : " + txtMaterialCenter.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtMaterialCenter.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtMaterialCenter.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [MaterialCenter] from [StockStatus] Where [MaterialCenter] ='" + txtMaterialCenter.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Material Center : " + txtMaterialCenter.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtMaterialCenter.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtMaterialCenter.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtMaterialCenter.Focus();
                    return false;
                }
            }
            catch { }
            return false;
        }

        private bool ValidateControls()
        {
            //if (txtMinStock.Text == "")
            //{
            //    MessageBox.Show("Sorry ! Decimal points can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    txtMinStock.Focus();
            //    return false;
            //}
            return true;
        }

        private void EnableAllControl()
        {
            txtMaterialCenter.ReadOnly = txtReportDate.ReadOnly = txtItemShownBy.ReadOnly = txtRemark.ReadOnly = false;
            chkShowZeroStockItems.Enabled = chkShowParentGroup.Enabled = chkShowValueOfItem.Enabled = chkShowMRPAlso.Enabled = true;

            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtMaterialCenter.ReadOnly = txtReportDate.ReadOnly = txtItemShownBy.ReadOnly = txtRemark.ReadOnly = true;
            chkShowZeroStockItems.Enabled = chkShowParentGroup.Enabled = chkShowValueOfItem.Enabled = chkShowMRPAlso.Enabled = false;

            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            //txtName.Text = txtSupplierName.Text = txtSearch.Text = lblMsg.Text = "";
            txtMaterialCenter.Text = txtReportDate.Text = txtItemShownBy.Text = txtRemark.Text = "";

            chkShowZeroStockItems.Checked = chkShowParentGroup.Checked = chkShowValueOfItem.Checked = chkShowMRPAlso.Checked = true;            
        }        
        
        private void StockStatus_KeyDown(object sender, KeyEventArgs e)
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

        private void StockStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult dar = MessageBox.Show("Are you sure you want to Close the Form ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dar == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void SearchResult()
        {
            try
            {
                if (objTable != null)
                {
                    if (txtSearch.Text != "")
                    {
                        DataRow[] row = objTable.Select(String.Format("MaterialCenter Like ('%" + txtSearch.Text + "%')"));
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
                        dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                SearchResult();
            }
        }

        private void txtReportDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtReportDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtReportDate.Text.Length > 0)
                {
                    dba.GetDateInExactFormat(sender, true, true, true);
                }
                else
                {
                    txtReportDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                }
            }
                
        }

      

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                dgrdName.Focus();
            }
        }


        private void UpdateRecord()
        {
            try
            {   
                if (chkShowZeroStockItems.Checked == true)
                {
                    _chkShowZeroStockItems = 1;
                }
                if (chkShowParentGroup.Checked == true)
                {
                    _chkShowParentGroup = 1;
                }
                if (chkShowValueOfItem.Checked == true)
                {
                    _chkShowValueOfItem = 1;
                }
                if (chkShowMRPAlso.Checked == true)
                {
                    _chkShowMRPAlso = 1;
                }


                DataGridViewRow row = dgrdName.SelectedRows[0];
                if (lblId.Text != "" && row != null)
                {
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                    {
                        string strOldName = Convert.ToString(row.Cells["MaterialCenter"].Value);
                        
                        string query = "Update StockStatus set MaterialCenter='" + txtMaterialCenter.Text.Trim() + "', ReportDate='" + txtReportDate.Text + "', "
                            + "ItemShownBy='" + txtItemShownBy.Text.Trim() + "', ShowZeroStockItems='" + _chkShowZeroStockItems + "',ShowParentGroup='" + _chkShowParentGroup + "', "
                            + "ShowValueOfItem='" + _chkShowValueOfItem + "', ShowMRPAlso='" + _chkShowMRPAlso + "',Remark='" + txtRemark.Text + "', "
                            + "[UpdateStatus]=1,[UpdatedBy]='" + MainPage.strLoginName + "' Where MaterialCenter='" + strOldName + "' "

                        + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                        + "('" + txtMaterialCenter.Text.Trim() + "','STOCKSTATUS',0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + "0" + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                        int count = dba.ExecuteMyQuery(query);
                        if (count > 0)
                        {
                            DataBaseAccess.CreateDeleteQuery(query);
                            MessageBox.Show("Thank you ! Record is updated successfully !", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataGrid();
                        }
                        else
                            MessageBox.Show("Sorry ! your Record is not updated..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Sorry ! Please select right Unit Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && lblId.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Unit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from StockStatus Where Id=" + lblId.Text + "");
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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


        private void BindAllDetails(DataGridViewRow row)
        {
            try
            {
                txtMaterialCenter.Text = Convert.ToString(row.Cells["MaterialCenter"].Value);
                txtReportDate.Text = Convert.ToString(row.Cells["ReportDate"].Value);
                txtItemShownBy.Text = Convert.ToString(row.Cells["ItemShownBy"].Value);                
                txtRemark.Text = Convert.ToString(row.Cells["Remark"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);

                if (Convert.ToInt16(row.Cells["ShowZeroStockItems"].Value) == 1)
                    chkShowZeroStockItems.Checked = true;
                else
                    chkShowZeroStockItems.Checked = false;

                if (Convert.ToInt16(row.Cells["ShowParentGroup"].Value) == 1)
                    chkShowParentGroup.Checked = true;
                else
                    chkShowParentGroup.Checked = false;

                if (Convert.ToInt16(row.Cells["ShowValueOfItem"].Value) == 1)
                    chkShowValueOfItem.Checked = true;
                else
                    chkShowValueOfItem.Checked = false;

                if (Convert.ToInt16(row.Cells["ShowMRPAlso"].Value) == 1)
                    chkShowMRPAlso.Checked = true;
                else
                    chkShowMRPAlso.Checked = false;


                string strCreatedBy = Convert.ToString(row.Cells["CreatedBy"].Value), strUpdatedBy = Convert.ToString(row.Cells["UpdatedBy"].Value);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                DisableAllControl();
            }
            catch { }
        }

        private void txtMaterialCenter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MaterialCenter", "SEARCH MATERIAL CENTER", e.KeyCode);
                        objSearch.ShowDialog();
                        // if (objSearch.strSelectedData != "")
                        txtMaterialCenter.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                btnAdd.Text = "&Add";
                txtSearch.Clear();
                txtSearch.Focus();
                DisableAllControl();
                if (dgrdName.Rows.Count > 0)
                {
                    dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
                    dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                    BindAllDetails(dgrdName.CurrentRow);
                }
            }
            catch { }
        }

        void textboxes_KeyPress(object sender, KeyPressEventArgs e)
        {
            //call your common method
            dba.ValidateSpace(sender, e);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails(txtMaterialCenter.Text, "STOCKSTATUS", "0");

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

    }

}
