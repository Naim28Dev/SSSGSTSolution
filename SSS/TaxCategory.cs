using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class TaxCategory : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewTax = false;
        public string StrAddedTax = "",__strName="";

         public TaxCategory()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
        }

         public TaxCategory(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewTax = chk;
            __strName = strName;          
        }

        private void TaxCategory_KeyDown(object sender, KeyEventArgs e)
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

        private void rdoMRPYes_CheckedChanged(object sender, EventArgs e)
        {
            lblCalculateTax.Enabled = lblTaxItem.Enabled = lblPerAmt.Enabled = txtCalculateTaxOn.Enabled = grpItemprice.Enabled = rdoMRPYes.Checked;
        }

        private void rdoTaxChangeYes_CheckedChanged(object sender, EventArgs e)
        {
            lblCalculatedOn.Enabled = lblTaxIGSTRate.Enabled = lblTaxCGSTRate.Enabled = lblTaxSGSTRate.Enabled = txtAmountType.Enabled = txtGreaterORSmaller.Enabled = txtTaxChangeAmt.Enabled = txtChangeIGSTRate.Enabled = txtChangeCGSTRate.Enabled = txtChangeSGSTRate.Enabled = rdoTaxChangeYes.Checked;
        }

        private void txtIGSTRate_KeyPress(object sender, KeyPressEventArgs e)        
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtTaxType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TAXTYPE", "SEARCH TAX TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtTaxType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtGreaterORSmaller_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GREATERSMALLER", "SEARCH AMOUNT CRITERIA", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtGreaterORSmaller.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCalculatedOn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("AMOUNTTYPE", "SEARCH AMOUNT TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtAmountType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void EnableAllControl()
        {
            txtCategoryName.ReadOnly = txtIGSTRate.ReadOnly = txtCGSTRate.ReadOnly = txtSGSTRate.ReadOnly =txtCalculateTaxOn.ReadOnly=txtTaxChangeAmt.ReadOnly=txtChangeIGSTRate.ReadOnly=txtChangeCGSTRate.ReadOnly=txtChangeSGSTRate.ReadOnly= false;
            grpMRP.Enabled = grpTaxRate.Enabled = true;
        }

        private void DisableAllControl()
        {
            txtCategoryName.ReadOnly = txtIGSTRate.ReadOnly = txtCGSTRate.ReadOnly = txtSGSTRate.ReadOnly = txtCalculateTaxOn.ReadOnly = txtTaxChangeAmt.ReadOnly = txtChangeIGSTRate.ReadOnly = txtChangeCGSTRate.ReadOnly = txtChangeSGSTRate.ReadOnly = true;
            grpMRP.Enabled = grpTaxRate.Enabled = false;
        }

        private void ClearAllControl()
        {
            txtCategoryName.Text =txtAmountType.Text= "";
            txtIGSTRate.Text = txtCGSTRate.Text = txtSGSTRate.Text = txtChangeIGSTRate.Text = txtChangeCGSTRate.Text = txtChangeSGSTRate.Text = "0.00";
            txtCalculateTaxOn.Text = "100";
            txtTaxChangeAmt.Text = "0";
            txtGreaterORSmaller.Text = ">";
            rdoMRPNo.Checked = rdoTaxChangeNo.Checked = rdoTaxInNo.Checked = true;
        }

        public bool ValidateControl()
        {
            if (txtCategoryName.Text == "")
            {
                MessageBox.Show("Sorry ! Category Name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }
            if (txtTaxType.Text == "")
            {
                MessageBox.Show("Sorry ! Tax Type can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtTaxType.Focus();
                return false;
            }
            return true;
        }

        private void txtIGSTRate_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtIGSTRate.Text != "" && txtIGSTRate.Text != "0")
                    {
                        double dRate = dba.ConvertObjectToDouble(txtIGSTRate.Text);
                        txtIGSTRate.Text = dRate.ToString("0.00");
                        txtCGSTRate.Text=txtSGSTRate.Text=(dRate/2).ToString("0.00");
                    }
                    else
                        txtIGSTRate.Text = txtCGSTRate.Text = txtSGSTRate.Text = "0.00";
                }
            }
            catch
            {
            }
        }

        private void txtIGSTRate_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = sender as TextBox;
                if (txt != null)
                {
                    if (txt.Text == "0.00" || txt.Text == "0")
                        txt.Text = "";
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                SearchResult();
            }
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                dgrdName.Focus();
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
                        DataRow[] row = objTable.Select(String.Format("TaxName Like ('%" + txtSearch.Text + "%')"));
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


        private bool CheckAvailability()
        {
            try
            {
                if (txtCategoryName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select CategoryName from TaxCategory Where CategoryName ='" + txtCategoryName.Text + "'");
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
                    else if (btnEdit.Text == "&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select CategoryName from TaxCategory Where CategoryName ='" + txtCategoryName.Text + "' and ID !=" + lblId.Text + "");
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
                    ClearAllControl();
                    EnableAllControl();
                    txtCategoryName.Focus();
                    btnAdd.Text = "&Save";
                }
                else
                {
                    if (CheckAvailability() && ValidateControl())
                    {
                        DialogResult dar = MessageBox.Show("Are you sure you want to save record ", " Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dar == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveRecord()
        {
            try
            {
                string strQuery = " if not exists (Select CategoryName from [dbo].[TaxCategory] Where CategoryName='" + txtCategoryName.Text + "') begin INSERT INTO [dbo].[TaxCategory] ([CategoryName],[TaxType],[TaxRateIGST],[TaxRateCGST],[TaxRateSGST],[TaxOnMRP],[CalculateTaxON],[TaxInclPrice],[ChangeTaxRate],[AmountType],[GreaterORSmaller],[ChangeAmt],[TaxChangeRateIGST],[TaxChangeRateCGST],[TaxChangeRateSGST],[Other],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                            + " ('" + txtCategoryName.Text + "','" + txtTaxType.Text + "'," + dba.ConvertObjectToDouble(txtIGSTRate.Text) + "," + dba.ConvertObjectToDouble(txtCGSTRate.Text) + "," + dba.ConvertObjectToDouble(txtSGSTRate.Text) + ",'" + rdoMRPYes.Checked + "'," + dba.ConvertObjectToDouble(txtCalculateTaxOn.Text) + ",'" + rdoTaxInYes.Checked + "','" + rdoTaxChangeYes.Checked + "','" + txtAmountType.Text + "','" + txtGreaterORSmaller.Text + "'," + dba.ConvertObjectToDouble(txtTaxChangeAmt.Text) + "," + dba.ConvertObjectToDouble(txtChangeIGSTRate.Text) + "," + dba.ConvertObjectToDouble(txtChangeCGSTRate.Text) + "," + dba.ConvertObjectToDouble(txtChangeSGSTRate.Text) + ",'','"+MainPage.strLoginName+"','',1,0) end ";
                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    if (MainPage._bItemMirroring)
                        dba.DataMirroringInCurrentFinYear(strQuery);

                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewTax)
                    {
                        StrAddedTax = txtCategoryName.Text;
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


        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select ID,CategoryName,TaxType from [dbo].[TaxCategory] Order by CategoryName");
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
                            dgrdName.Rows[rowIndex].Cells["categoryName"].Value = dr["CategoryName"];
                            dgrdName.Rows[rowIndex].Cells["taxType"].Value = dr["TaxType"];
                            rowIndex++;
                        }
                        dgrdName.CurrentCell = dgrdName.Rows[rowIndex - 1].Cells[1];
                        dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                        if (dgrdName.Rows.Count == 1)
                            BindAllDetails(dgrdName.Rows[0]);
                    }

                }
            }
            catch { }
        }

        private void BindAllDetails(DataGridViewRow rows)
        {
            try
            {
                string strCategoryName = Convert.ToString(rows.Cells["categoryName"].Value);
                if (strCategoryName != null && strCategoryName != "")
                {
                    DataTable _dt = dba.GetDataTable("Select * from TaxCategory Where  CategoryName='" + strCategoryName + "' ");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        lblId.Text = Convert.ToString(row["ID"]);
                        txtCategoryName.Text = Convert.ToString(row["CategoryName"]);
                        txtTaxType.Text = Convert.ToString(row["TaxType"]);
                        txtIGSTRate.Text = Convert.ToString(row["TaxRateIGST"]);
                        txtCGSTRate.Text = Convert.ToString(row["TaxRateCGST"]);
                        txtSGSTRate.Text = Convert.ToString(row["TaxRateSGST"]);
                        rdoMRPYes.Checked = Convert.ToBoolean(row["TaxOnMRP"]);
                        rdoTaxInYes.Checked = Convert.ToBoolean(row["TaxInclPrice"]);
                        rdoTaxChangeYes.Checked = Convert.ToBoolean(row["ChangeTaxRate"]);
                        txtCalculateTaxOn.Text = Convert.ToString(row["CalculateTaxON"]);
                        txtAmountType.Text = Convert.ToString(row["AmountType"]);
                        txtGreaterORSmaller.Text = Convert.ToString(row["GreaterORSmaller"]);
                        txtTaxChangeAmt.Text = Convert.ToString(row["ChangeAmt"]);
                        txtChangeIGSTRate.Text = Convert.ToString(row["TaxChangeRateIGST"]);
                        txtChangeCGSTRate.Text = Convert.ToString(row["TaxChangeRateCGST"]);
                        txtChangeSGSTRate.Text = Convert.ToString(row["TaxChangeRateSGST"]);

                        lblCreatedBy.Text = "";
                        string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                        if (strCreatedBy != "")
                            lblCreatedBy.Text = "Created By : " + strCreatedBy;
                        if (strUpdatedBy != "")
                            lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                    }
                }
                DisableAllControl();
            }
            catch { }
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
                    if (CheckAvailability() && ValidateControl() && lblId.Text != "")
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


        private void UpdateRecord()
        {
            try
            {
                DataGridViewRow row = dgrdName.SelectedRows[0];
                if (lblId.Text != "" && row != null)
                {
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                    {
                        string strOldName = Convert.ToString(row.Cells["categoryName"].Value);
                        if (strOldName != "")
                        {
                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from [dbo].[TaxCategory] Where CategoryName='" + strOldName + "' ");

                            string strQuery = "";
                            strQuery += " UPDATE  [dbo].[TaxCategory] SET [CategoryName]='" + txtCategoryName.Text + "',[TaxType]='" + txtTaxType.Text + "',[TaxRateIGST]=" + dba.ConvertObjectToDouble(txtIGSTRate.Text) + ",[TaxRateCGST]=" + dba.ConvertObjectToDouble(txtCGSTRate.Text) + ",[TaxRateSGST]=" + dba.ConvertObjectToDouble(txtSGSTRate.Text) + ",[TaxOnMRP]='" + rdoMRPYes.Checked + "',"
                                          + " [CalculateTaxON]=" + dba.ConvertObjectToDouble(txtCalculateTaxOn.Text) + ",[TaxInclPrice]='" + rdoTaxInYes.Checked + "',[ChangeTaxRate]='" + rdoTaxChangeYes.Checked + "',[AmountType]='" + txtAmountType.Text + "',[GreaterORSmaller]='" + txtGreaterORSmaller.Text + "',[ChangeAmt]=" + dba.ConvertObjectToDouble(txtTaxChangeAmt.Text) + ",[TaxChangeRateIGST]=" + dba.ConvertObjectToDouble(txtChangeIGSTRate.Text) + ",[TaxChangeRateCGST]=" + dba.ConvertObjectToDouble(txtChangeCGSTRate.Text) + ",[TaxChangeRateSGST]=" + dba.ConvertObjectToDouble(txtChangeSGSTRate.Text) + ",[Other]='',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where CategoryName='" + strOldName + "'  ";
                                           

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                }
                                if (MainPage._bItemMirroring)
                                    dba.DataMirroringInCurrentFinYear(strQuery);

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
                    else
                        MessageBox.Show("Sorry ! Please select right  list .", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Tax Master.", ex.Message };
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
               
                if (dgrdName.Rows.Count > 0)
                {
                    dgrdName.CurrentCell = dgrdName.Rows[dgrdName.Rows.Count - 1].Cells[1];
                    dgrdName.FirstDisplayedCell = dgrdName.CurrentCell;
                    BindAllDetails(dgrdName.CurrentRow);
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        DataGridViewRow row = dgrdName.SelectedRows[0];
                        if (lblId.Text != "" && row != null)
                        {
                            if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                            {
                                string strOldName = Convert.ToString(row.Cells["categoryName"].Value), strQuery = "";
                                if (strOldName != "")
                                {
                                    object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from [dbo].[TaxCategory] Where CategoryName='" + strOldName + "' ");
                                    strQuery = "Delete from [dbo].[TaxCategory] Where CategoryName='" + strOldName + "' ";
                                    int i = dba.ExecuteMyQuery(strQuery);
                                    if (i > 0)
                                    {
                                        if (!Convert.ToBoolean(objStatus))
                                        {
                                            DataBaseAccess.CreateDeleteQuery(strQuery);
                                        }
                                        MessageBox.Show("Record is deleted Successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        lblMsg.Text = "";
                                        ClearAllControl();
                                        BindDataGrid();
                                    }
                                }
                            }
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

        private void TaxCategory_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (IsNewTax)
                    {
                        panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        btnAdd.PerformClick();
                        txtCategoryName.Text = __strName;
                        txtCategoryName.Focus();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                this.BeginInvoke(new MethodInvoker(Close));
                return false;
            }
        }

        private void TaxCategory_FormClosing(object sender, FormClosingEventArgs e)
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
