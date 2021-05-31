using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class MaterialCenterMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        int StockInBalSheet = 0;
        public string strAddedMCentre = "",__strName="";
        bool _newMCenter = false;

        public MaterialCenterMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
        }

        public MaterialCenterMaster(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            _newMCenter = chk;
            __strName = strName;
        }

        private void BindDataGrid()
        {
            try
            {
                string query = "Select ID,Name,Alias,PrintName,[Group],StockAccount,StockInBalSheet,Address1,Address2, "
                    + " Address3,Remark,GSTNo,StateName,PinCode,CreatedBy,UpdatedBy,InsertStatus,UpdateStatus from MaterialCenterMaster Order by Name";
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
                            dgrdName.Rows[rowIndex].Cells["MCName"].Value = dr["Name"];
                            dgrdName.Rows[rowIndex].Cells["Alias"].Value = dr["Alias"];
                            dgrdName.Rows[rowIndex].Cells["PrintName"].Value = dr["PrintName"];
                            dgrdName.Rows[rowIndex].Cells["Group"].Value = dr["Group"];
                            dgrdName.Rows[rowIndex].Cells["StockAccount"].Value = dr["StockAccount"];
                            dgrdName.Rows[rowIndex].Cells["StockInBalSh"].Value = dr["StockInBalSheet"];
                            dgrdName.Rows[rowIndex].Cells["Address1"].Value = dr["Address1"];
                            dgrdName.Rows[rowIndex].Cells["Address2"].Value = dr["Address2"];
                            dgrdName.Rows[rowIndex].Cells["Address3"].Value = dr["Address3"];
                            dgrdName.Rows[rowIndex].Cells["Remark"].Value = dr["Remark"];
                            dgrdName.Rows[rowIndex].Cells["gstNo"].Value = dr["GSTNo"];
                            dgrdName.Rows[rowIndex].Cells["state"].Value = dr["StateName"];
                            dgrdName.Rows[rowIndex].Cells["pin"].Value = dr["PinCode"];
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
            catch (Exception ex){ }
        }

        private void SaveRecord()
        {
            try
            {
                if (chkStockInBS.Checked == true)
                {
                    StockInBalSheet = 1;
                }

                string strQuery = " if not exists (Select Name from [dbo].[MaterialCenterMaster] Where Name='" + txtName.Text.Trim() + "') "
                 + "begin INSERT INTO [dbo].[MaterialCenterMaster] ([Name],[Alias],[PrintName],[Group],[StockAccount],[StockInBalSheet],[Address1],[Address2],[Address3],[Remark],[Date],[GSTNo],[StateName],[PinCode], "
                 + "[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                 + "('" + txtName.Text.Trim() + "','" + txtAlias.Text.Trim() + "','" + txtPrintName.Text.Trim() + "','" + txtGroup.Text.Trim() + "', "
                 + " '" + txtStockAccount.Text.Trim() + "','" + StockInBalSheet + "','" + txtAddress1.Text.Trim() + "','" + txtAddress2.Text.Trim() + "','" + txtAddress3.Text.Trim() + "', "
                 + " '" + txtRemark.Text.Trim() + "', DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'"+ txtGSTNo.Text.Trim() + "','" + txtState.Text.Trim() + "','" + txtPIN.Text.Trim() + "','" + MainPage.strLoginName + "','',1,0) "
                 
                 + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                    + "('" + txtName.Text.Trim() + "','MATERIALCENTER',0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + "0" + ",'" + MainPage.strLoginName + "',1,0,'CREATION') end ";

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    //DataBaseAccess.CreateDeleteQuery(strQuery);
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (_newMCenter)
                    {
                        strAddedMCentre = txtName.Text;                      
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

        private bool CheckAvailability()
        {
            try
            {
                if (txtName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [Name] from [MaterialCenterMaster] Where [Name] ='" + txtName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Name : " + txtName.Text + " already exist ! ";
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
                    else if (btnEdit.Text == "&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [Name] from [MaterialCenterMaster] Where [Name] ='" + txtName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Name : " + txtName.Text + " already exist ! ";
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
                    lblMsg.Text = "Sorry ! Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtName.Focus();
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
            txtPIN.ReadOnly = txtState.ReadOnly = txtGSTNo.ReadOnly = txtName.ReadOnly = txtAlias.ReadOnly = txtPrintName.ReadOnly = txtGroup.ReadOnly = txtStockAccount.ReadOnly
               = txtAddress1.ReadOnly = txtAddress2.ReadOnly = txtAddress3.ReadOnly = txtRemark.ReadOnly = false;
            chkStockInBS.Enabled = true;

            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtPIN.ReadOnly = txtState.ReadOnly = txtGSTNo.ReadOnly = txtName.ReadOnly = txtAlias.ReadOnly = txtPrintName.ReadOnly = txtGroup.ReadOnly = txtStockAccount.ReadOnly
                = txtAddress1.ReadOnly = txtAddress2.ReadOnly = txtAddress3.ReadOnly = txtRemark.ReadOnly = true;
            chkStockInBS.Enabled = false;

            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            //txtName.Text = txtSupplierName.Text = txtSearch.Text = lblMsg.Text = "";
            txtPIN.Text = txtState.Text = txtGSTNo.Text =  txtName.Text = txtAlias.Text = txtPrintName.Text = txtGroup.Text = txtStockAccount.Text
                = txtAddress1.Text = txtAddress2.Text = txtAddress3.Text = txtRemark.Text = lblMsg.Text = "";

            chkStockInBS.Checked = true;
        }


        private void MaterialCenterMaster_KeyDown(object sender, KeyEventArgs e)
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

        private void MaterialCenterMaster_FormClosing(object sender, FormClosingEventArgs e)
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
                        DataRow[] row = objTable.Select(String.Format("Name Like ('%" + txtSearch.Text + "%')"));
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


        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                dgrdName.Focus();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            //dba.ValidateSpace(sender, e);
        }


        private void BindAllDetails(DataGridViewRow row)
        {
            try
            {
                txtName.Text = Convert.ToString(row.Cells["MCName"].Value);
                txtGroup.Text = Convert.ToString(row.Cells["Group"].Value);
                txtAlias.Text = Convert.ToString(row.Cells["Alias"].Value);
                txtPrintName.Text = Convert.ToString(row.Cells["PrintName"].Value);
                txtStockAccount.Text = Convert.ToString(row.Cells["StockAccount"].Value);
                txtAddress1.Text = Convert.ToString(row.Cells["Address1"].Value);
                txtAddress2.Text = Convert.ToString(row.Cells["Address2"].Value);
                txtAddress3.Text = Convert.ToString(row.Cells["Address3"].Value);
                txtRemark.Text = Convert.ToString(row.Cells["Remark"].Value);
                txtGSTNo.Text = Convert.ToString(row.Cells["gstNo"].Value);
                txtState.Text = Convert.ToString(row.Cells["state"].Value);
                txtPIN.Text = Convert.ToString(row.Cells["pin"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);

                if (Convert.ToInt16(row.Cells["StockInBalSh"].Value) == 1)
                    chkStockInBS.Checked = true;
                else
                    chkStockInBS.Checked = false;
                
                string strCreatedBy = Convert.ToString(row.Cells["CreatedBy"].Value), strUpdatedBy = Convert.ToString(row.Cells["UpdatedBy"].Value);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += ", Updated  By : " + strUpdatedBy;

                DisableAllControl();
            }
            catch(Exception ex) { }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtname_KeyPress(object sender, KeyPressEventArgs e)
        {
            //dba.ValidateSpace(sender, e);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && lblId.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Unit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from MaterialCenterMaster Where Id=" + lblId.Text + "");
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
                if (chkStockInBS.Checked == true)
                {
                    StockInBalSheet = 1;
                }

                DataGridViewRow row = dgrdName.SelectedRows[0];
                if (lblId.Text != "" && row != null)
                {
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                    {
                        string strOldName = Convert.ToString(row.Cells["MCName"].Value);

                        string query = "Update MaterialCenterMaster set Name='" + txtName.Text + "', Alias='" + txtAlias.Text + "', GSTNo='" + txtGSTNo.Text.Trim() + "', StateName='" + txtState.Text.Trim() + "', PinCode='" + txtPIN.Text.Trim() + "', "
                            + "PrintName='" + txtPrintName.Text + "', [Group]='" + txtGroup.Text + "',StockAccount='" + txtStockAccount.Text + "',StockInBalSheet='" + StockInBalSheet + "', "
                            + "Address1='" + txtAddress1.Text + "', Address2='" + txtAddress2.Text + "',Address3='" + txtAddress3.Text + "',Remark='" + txtRemark.Text + "', "
                            + "[UpdateStatus]=1,[UpdatedBy]='" + MainPage.strLoginName + "' Where Name='" + strOldName + "' "                          
                            + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                            + "('" + txtName.Text.Trim() + "','MATERIALCENTER',0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + "0" + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                        if (strOldName != txtName.Text)
                            query += " Update [dbo].[EditTrailDetails] Set BillType='" + txtName.Text.Trim() + "' WHere BillType='" + strOldName + "' and BillCode='MATERIALCENTER' ";

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
                    EditTrailDetails objEdit = new EditTrailDetails(txtName.Text, "MATERIALCENTER", "0");

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void MaterialCenterMaster_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (_newMCenter)
                    {
                        btnAdd.PerformClick();
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                        txtName.Text = __strName;
                        txtName.Focus();
                    }
                }
            }
            catch { }
        }

        private void txtState_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string _strState = txtState.Text;
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtState.Text = objSearch.strSelectedData;
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtGSTNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtGSTNo_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (txtGSTNo.Text != "")
                {
                    bool chk = System.Text.RegularExpressions.Regex.IsMatch(txtGSTNo.Text, @"\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}");
                    if (!chk)
                    {
                        txtGSTNo.ForeColor = Color.Red;
                        MessageBox.Show("Sorry ! GST Number not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    else
                        txtGSTNo.ForeColor = Color.Black;

                }
                else
                    txtGSTNo.ForeColor = Color.Black;
            }
            catch
            {
            }
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
    }

}
