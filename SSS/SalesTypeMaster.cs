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
    public partial class SalesTypeMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewSaleType = false;
        public string StrAddedSaleType = "",__strName;

        public SalesTypeMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
        }

        public SalesTypeMaster(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewSaleType = chk;
            __strName = strName;
        }

        private void TaxMaster_KeyDown(object sender, KeyEventArgs e)
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

        private void rdoLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoLocal.Checked)
            {
                txtSGSTAccount.Visible = lblTaxAccountSGST.Visible =lblSGSTRate.Visible=txtSGSTRate.Visible= true;
                lblTaxAccountIGST.Text = "Tax Account (CGST) :";
                lblIGSTRate.Text = "CGST Rate (in %) :";              

            }
        }

        private void rdoInterState_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoInterState.Checked)
            {
                txtSGSTAccount.Visible = lblTaxAccountSGST.Visible = lblSGSTRate.Visible = txtSGSTRate.Visible = false;
                lblTaxAccountIGST.Text = "Tax Account  (IGST) :";
                lblIGSTRate.Text = "IGST Rate (in %) :";
                txtSGSTAccount.Text = "";
                txtSGSTRate.Text = "0";
            }
        }

        private void txtIGSTAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "OTHER CURRENT LIABILITIES", "SEARCH TAX ACCOUNT", e.KeyCode);
                        objSearch.ShowDialog();
                        txtIGSTAccount.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSGSTAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "OTHER CURRENT LIABILITIES", "SEARCH TAX ACCOUNT", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSGSTAccount.Text = objSearch.strSelectedData;
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
            txtTaxName.ReadOnly = txtSGSTRate.ReadOnly = txtIGSTRate.ReadOnly = false;
            txtSearch.ReadOnly = true;
            dgrdName.Enabled = false;
            grpEComm.Enabled = grpItemprice.Enabled = grpMRP.Enabled = grpRegion.Enabled = grpGST.Enabled = true;

            grpTTypeBox.Enabled = grptaxCalculation.Enabled = btnDelete.Enabled;

        }

        private void DisableAllControl()
        {
            txtTaxName.ReadOnly=txtSGSTRate.ReadOnly=txtIGSTRate.ReadOnly = true;
            txtSearch.ReadOnly = false;
            dgrdName.Enabled = true;
            grpEComm.Enabled = grpItemprice.Enabled = grpMRP.Enabled = grpRegion.Enabled = grpTTypeBox.Enabled = grpGST.Enabled = false;
        }

        private void ClearAllText()
        {
            txtTaxName.Text = txtIGSTAccount.Text = txtSGSTAccount.Text = txtSearch.Text = lblMsg.Text = lblCreatedBy.Text = txtEcommPortalName.Text = "";
            rdoTaxableVoucherwise.Checked = rdoENo.Checked = rdoLocal.Checked = rdoMRPNo.Checked = rdoTaxInNo.Checked = rdoGSTNo.Checked = true;
            txtIGSTRate.Text = txtSGSTRate.Text = "0.00";
        }


        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select ID,TaxName,Region from SaleTypeMaster Where SaleType='SALES'  Order by TaxName");
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
                            dgrdName.Rows[rowIndex].Cells["taxName"].Value = dr["TaxName"];
                            dgrdName.Rows[rowIndex].Cells["region"].Value = dr["Region"];
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
                string strTaxType = Convert.ToString(rows.Cells["taxName"].Value);
                if (strTaxType != null && strTaxType != "")
                {
                    DataTable _dt = dba.GetDataTable("Select *,dbo.GetFullName(TaxAccountIGST) TaxAIGST,dbo.GetFullName(TaxAccountSGST) TaxASGST,(Select ISNULL(Count(*),0) from SalesRecord Where SalesType=TaxName)_Count from SaleTypeMaster Where  SaleType='SALES' and  TaxName='" + strTaxType + "' ");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        lblId.Text = Convert.ToString(row["ID"]);
                        txtTaxName.Text = Convert.ToString(row["TaxName"]);
                        txtIGSTAccount.Text = Convert.ToString(row["TaxAIGST"]);
                        txtSGSTAccount.Text = Convert.ToString(row["TaxASGST"]);
                        txtIGSTRate.Text = Convert.ToString(row["IGSTTaxRate"]);
                        txtSGSTRate.Text = Convert.ToString(row["SGSTTaxRate"]);
                        txtEcommPortalName.Text = Convert.ToString(row["EcommPortalName"]);
                        if (Convert.ToString(row["Region"]) == "LOCAL")
                            rdoLocal.Checked = true;
                        else
                            rdoInterState.Checked = true;

                        if (Convert.ToString(row["TaxationType"]) == "VOUCHERWISE")
                            rdoTaxableVoucherwise.Checked = true;
                        else if (Convert.ToString(row["TaxationType"]) == "ITEMWISE")
                            rdoTaxableItemwise.Checked = true;
                        else if (Convert.ToString(row["TaxationType"]) == "REVERSECHARGE")
                            rdoReverseCharges.Checked = true;
                        else if (Convert.ToString(row["TaxationType"]) == "EXEMPT")
                            rdoExempt.Checked = true;
                        else if (Convert.ToString(row["TaxationType"]) == "ZERORATED")
                            rdoZeroRated.Checked = true;
                        else //if (Convert.ToString(row["TaxationType"]) == "NONGST")
                            rdoNonGST.Checked = true;

                        rdoEYes.Checked = Convert.ToBoolean(row["EcommType"]);
                        rdoMRPYes.Checked = Convert.ToBoolean(row["TaxOnMRP"]);
                        rdoTaxInYes.Checked = Convert.ToBoolean(row["TaxIncluded"]);
                        rdoGSTYes.Checked = Convert.ToBoolean(row["SkipGST"]);
                        if (!rdoEYes.Checked)
                            rdoENo.Checked = true;
                        if (!rdoMRPYes.Checked)
                            rdoMRPNo.Checked = true;
                        if (!rdoTaxInYes.Checked)
                            rdoTaxInNo.Checked = true;
                        if (!rdoGSTYes.Checked)
                            rdoGSTNo.Checked = true;

                        lblCreatedBy.Text = "";
                        string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                        if (strCreatedBy != "")
                            lblCreatedBy.Text = "Created By : " + strCreatedBy;
                        if (strUpdatedBy != "")
                            lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;
                        if (dba.ConvertObjectToDouble(row["_Count"]) > 0)
                            btnDelete.Enabled = false;
                        else
                            btnDelete.Enabled = true;
                    }
                }
                DisableAllControl();
            }
            catch { }
        }

        public bool ValidateControl()
        {
            if (txtTaxName.Text == "")
            {
                MessageBox.Show("Sorry ! Tax Name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtTaxName.Focus();
                return false;
            }
            return true;
        }

        private bool CheckAvailability()
        {
            try
            {
                if (txtTaxName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select TaxName from SaleTypeMaster Where SaleType='SALES' and TaxName ='" + txtTaxName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Tax Name : " + txtTaxName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTaxName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTaxName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select TaxName from SaleTypeMaster Where SaleType='SALES' and TaxName ='" + txtTaxName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Tax Name : " + txtTaxName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTaxName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTaxName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Tax Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtTaxName.Focus();
                    return false;
                }
            }
            catch { }
            return false;
        }

        private void txtTaxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
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
                    txtTaxName.Focus();
                    grpTTypeBox.Enabled = grptaxCalculation.Enabled = btnDelete.Enabled=true;
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
                string strIGSTAccount = "", strSGSTAccount = "";
                if (txtIGSTAccount.Text != "")
                {
                    string[] strFullName = txtIGSTAccount.Text.Split(' ');
                    if (strFullName.Length > 1)                   
                        strIGSTAccount = strFullName[0].Trim();                    
                }
                if (txtSGSTAccount.Text != "")
                {
                    string[] strFullName = txtSGSTAccount.Text.Split(' ');
                    if (strFullName.Length > 1)                   
                        strSGSTAccount = strFullName[0].Trim();                    
                }

                string strQuery = " if not exists (Select TaxName from [dbo].[SaleTypeMaster] Where  SaleType='SALES' and TaxName='" + txtTaxName.Text + "') begin INSERT INTO [dbo].[SaleTypeMaster] ([TaxName],[Region],[TaxationType],[EcommType],[TaxOnMRP],[TaxIncluded],[EcommPortalName],[TaxAccountIGST],[TaxAccountSGST],[Other],[SkipGST],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy],[SaleType],[IGSTTaxRate],[SGSTTaxRate]) VALUES "
                                            + " ('" + txtTaxName.Text + "','" + GetRegion() + "','" + GetTaxationType() + "','" + rdoEYes.Checked + "','" + rdoMRPYes.Checked + "','" + rdoTaxInYes.Checked + "','" + txtEcommPortalName.Text + "','" + strIGSTAccount + "','" + strSGSTAccount + "','','" + rdoGSTYes.Checked + "', 1,0,'" + MainPage.strLoginName + "','','SALES',"+dba.ConvertObjectToDouble(txtIGSTRate.Text)+","+dba.ConvertObjectToDouble(txtSGSTRate.Text)+") end ";
                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewSaleType)
                    {
                        StrAddedSaleType = txtTaxName.Text;
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

        private string GetRegion()
        {
            if (rdoLocal.Checked)
                return "LOCAL";
            else
                return "INTERSTATE";
        }

        private string GetTaxationType()
        {
            if (rdoTaxableVoucherwise.Checked)
                return "VOUCHERWISE";
            else if (rdoTaxableItemwise.Checked)
                return "ITEMWISE";
            else if (rdoReverseCharges.Checked)
                return "REVERSECHARGE";
            else if (rdoExempt.Checked)
                return "EXEMPT";
            else if (rdoZeroRated.Checked)
                return "ZERORATED";
            else
                return "NONGST";
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
                        string strOldName = Convert.ToString(row.Cells["taxName"].Value);
                        if (strOldName != "")
                        {
                            string strIGSTAccount = "", strSGSTAccount = "";
                            if (txtIGSTAccount.Text != "")
                            {
                                string[] strFullName = txtIGSTAccount.Text.Split(' ');
                                if (strFullName.Length > 1)
                                    strIGSTAccount = strFullName[0].Trim();
                            }
                            if (txtSGSTAccount.Text != "")
                            {
                                string[] strFullName = txtSGSTAccount.Text.Split(' ');
                                if (strFullName.Length > 1)
                                    strSGSTAccount = strFullName[0].Trim();
                            }

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from [dbo].[SaleTypeMaster] Where SaleType='SALES' and TaxName='" + strOldName + "' ");

                            string strQuery = " Update [dbo].[SaleTypeMaster] SET [TaxName]='" + txtTaxName.Text + "',[Region]='" + GetRegion() + "',[TaxationType]='" + GetTaxationType() + "',[EcommType]='" + rdoEYes.Checked + "',[TaxOnMRP]='" + rdoMRPYes.Checked + "',[TaxIncluded]='" + rdoTaxInYes.Checked + "',[EcommPortalName]='" + txtEcommPortalName.Text + "',[TaxAccountIGST]='" + strIGSTAccount + "',[TaxAccountSGST]='" + strSGSTAccount + "',[Other]='',[SkipGST]='" + rdoGSTYes.Checked + "',[UpdateStatus]=1,[UpdatedBy]='" + MainPage.strLoginName + "',[IGSTTaxRate]=" + dba.ConvertObjectToDouble(txtIGSTRate.Text) + ",[SGSTTaxRate]=" + dba.ConvertObjectToDouble(txtSGSTRate.Text) + " Where SaleType='SALES' and TaxName='" + strOldName + "' ";
                            if (strOldName != txtTaxName.Text)
                                strQuery += " Update SalesRecord Set SalesType='" + txtTaxName.Text + "' WHere SalesType='" + strOldName + "' ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                }
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
                                string strOldName = Convert.ToString(row.Cells["taxName"].Value), strQuery = "";
                                if (strOldName != "")
                                {
                                    object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from [dbo].[SaleTypeMaster] Where SaleType='SALES' and TaxName='" + strOldName + "' ");
                                    strQuery = "Delete from [dbo].[SaleTypeMaster] Where SaleType='SALES' and TaxName='" + strOldName + "' ";
                                    int i = dba.ExecuteMyQuery(strQuery);
                                    if (i > 0)
                                    {
                                        if (!Convert.ToBoolean(objStatus))
                                        {
                                            DataBaseAccess.CreateDeleteQuery(strQuery);
                                        }
                                        MessageBox.Show("Record is deleted Successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        lblMsg.Text = "";
                                        ClearAllText();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Text = "&Edit";
                btnAdd.Text = "&Add";
                txtSearch.Clear();
                txtSearch.Focus();
                txtTaxName.ReadOnly = true;
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

        private void txtEcommPortalName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PORTAL NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        if (txtEcommPortalName.Text != "")
                        {
                            DialogResult result = MessageBox.Show("Are you want to add more portal name ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                                txtEcommPortalName.Text += "," + objSearch.strSelectedData;
                            else
                                txtEcommPortalName.Text = objSearch.strSelectedData;
                        }
                        else
                            txtEcommPortalName.Text = objSearch.strSelectedData;
                    }
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void rdoEYes_CheckedChanged(object sender, EventArgs e)
        {
            lblEComm.Enabled = txtEcommPortalName.Enabled = rdoEYes.Checked;
        }

        private void rdoENo_CheckedChanged(object sender, EventArgs e)
        {
            lblEComm.Enabled = txtEcommPortalName.Enabled = rdoEYes.Checked;
        }

        private void SalesTypeMaster_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (IsNewSaleType)
                    {
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdName.TabStop = false;
                        btnAdd.PerformClick();
                        txtTaxName.Text = __strName;
                        txtTaxName.Focus();
                    }
                }
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

        private void txtIGSTRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void SalesTypeMaster_FormClosing(object sender, FormClosingEventArgs e)
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
