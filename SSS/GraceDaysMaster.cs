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
    public partial class GraceDaysMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewScheme = false;
        public string StrAddedSceheme = "";

        public GraceDaysMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public GraceDaysMaster(bool chk)
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
                string strQuery = "Select *,CONVERT(varchar,StartDate,103) SDate,CONVERT(varchar,EndDate,103) EDate from [GraceDaysMaster] Order by StartDate desc ";
                objTable = dba.GetDataTable(strQuery);
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
                            dgrdName.Rows[rowIndex].Cells["offerName"].Value = dr["OfferName"];
                            dgrdName.Rows[rowIndex].Cells["startDate"].Value = dr["SDate"];
                            dgrdName.Rows[rowIndex].Cells["endDate"].Value = dr["EDate"];
                            dgrdName.Rows[rowIndex].Cells["buyerDays"].Value = dr["BuyerDays"];
                            dgrdName.Rows[rowIndex].Cells["supplierDays"].Value = dr["SupplierDays"];
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
                txtOfferName.Text = Convert.ToString(row.Cells["offerName"].Value);
                txtStartDate.Text =Convert.ToString(row.Cells["startDate"].Value);
                txtEndDate.Text = Convert.ToString(row.Cells["endDate"].Value);
                txtBuyerDays.Text = Convert.ToString(row.Cells["buyerDays"].Value);
                txtSupplierDays.Text = Convert.ToString(row.Cells["supplierDays"].Value);
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

                string strQuery = " if not exists (Select OfferName from [dbo].[GraceDaysMaster] Where OfferName='" + txtOfferName.Text+"') begin  "
                                + " INSERT INTO [dbo].[GraceDaysMaster] ([BranchCode],[OfferName],[BuyerDays],[SupplierDays],[StartDate],[EndDate],[Remark],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[ActiveStatus]) VALUES "
                                + " ('" + MainPage.strBranchCode + "','" + txtOfferName.Text + "','" + txtBuyerDays.Text + "','" + txtSupplierDays.Text + "','" + sDate.ToString("MM/dd/yyyy") + "','" + eDate.ToString("MM/dd/yyyy") + "','','"+MainPage.strLoginName+ "','',1,0,'" + chkActive.Checked.ToString() + "') end ";

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    MessageBox.Show("Thank you ! Record saved successfully ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (IsNewScheme)
                    {
                        StrAddedSceheme = txtOfferName.Text;
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
            if (txtOfferName.Text == "")
            {
                MessageBox.Show("Sorry ! Offer name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOfferName.Focus();
                return false;
            }
            if (txtBuyerDays.Text == "")
            {
                MessageBox.Show("Sorry ! Buyer Grace Days can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuyerDays.Focus();
                return false;
            }
            if (txtSupplierDays.Text == "")
            {
                MessageBox.Show("Sorry ! Supplier Grace Days can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierDays.Focus();
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
            txtOfferName.ReadOnly = txtStartDate.ReadOnly = txtEndDate.ReadOnly=txtBuyerDays.ReadOnly=txtSupplierDays.ReadOnly = false;
            txtSearch.ReadOnly = chkActive.Enabled = true;
            dgrdName.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtOfferName.ReadOnly = txtStartDate.ReadOnly = txtEndDate.ReadOnly = txtBuyerDays.ReadOnly = txtSupplierDays.ReadOnly= true;
            txtSearch.ReadOnly = chkActive.Enabled= false;
            dgrdName.Enabled = true;
        }

        private void ClearAllText()
        {
            txtOfferName.Text = txtStartDate.Text = txtEndDate.Text = txtBuyerDays.Text = txtSupplierDays.Text = txtSearch.Text = lblMsg.Text = txtBranchCode.Text = "";
            txtStartDate.Text = txtEndDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            chkActive.Checked = true;
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
                    txtOfferName.Focus();
                    ClearAllText();
                    btnAdd.TabStop = true;
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Offer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery("Delete from [dbo].[GraceDaysMaster] Where ID=" + lblId.Text + "");
                        if (i > 0)
                        {
                            MessageBox.Show("Thanks ! Record deleted successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


                        string strOldOfferName = Convert.ToString(row.Cells["offerName"].Value);

                        string strQuery = " Update [dbo].[GraceDaysMaster] Set [OfferName]='" + txtOfferName.Text + "',[BuyerDays]='" + txtBuyerDays.Text + "',[SupplierDays]='" + txtSupplierDays.Text + "',[StartDate]='" + sDate.ToString("MM/dd/yyyy") + "',[EndDate]='" + eDate.ToString("MM/dd/yyyy") + "',[UpdatedBy]='" + MainPage.strLoginName + "',[ActiveStatus]='" + chkActive.Checked.ToString() + "' Where [OfferName]='" + strOldOfferName + "' ";
                        if (txtOfferName.Text != strOldOfferName)
                            strQuery += " Update [OrderBooking] Set SchemeName='" + txtOfferName.Text + "' Where OfferName='" + strOldOfferName + "' ";


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
                        MessageBox.Show("Sorry ! Please select right offer Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Offer Master.", ex.Message };
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
                    btnAdd.TabStop = false;
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
                if (txtOfferName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select OfferName from [GraceDaysMaster] Where OfferName ='" + txtOfferName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Offer Name : "+txtOfferName.Text+" already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtOfferName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtOfferName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select OfferName from [GraceDaysMaster] Where OfferName ='" + txtOfferName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Offer Name : " + txtOfferName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtOfferName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtOfferName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Offer Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtOfferName.Focus();
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
                        DataRow[] row = objTable.Select(String.Format("OfferName Like ('%" + txtSearch.Text + "%')"));
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
                txtOfferName.ReadOnly = txtStartDate.ReadOnly = btnAdd.TabStop = true;
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
                    txtOfferName.Focus();
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
                dba.GetDateInExactFormat(sender, true, true, true);
        }
    }
}
