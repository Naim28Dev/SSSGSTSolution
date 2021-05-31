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
    public partial class BrandMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewBrand = false;
        public string StrAddedBrand = "",__strName="";

        public BrandMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();          
        }

        public BrandMaster(bool chk,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataGrid();
            IsNewBrand = chk;
            __strName = strName;
        }

        private void BindDataGrid()
        {
            try
            {
                objTable = dba.GetDataTable("Select * from [BrandMaster] BM outer APPLY (Select Top 1 (PurchasePartyID+' '+Name)SupplierName from SupplierMaster SM Where BM.PurchasePartyID=SM.AreaCode+SM.AccountNo)SM Order by [BrandName]");
                if (objTable != null)
                {
                    dgrdDetails.Rows.Clear();
                    if (objTable.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(objTable.Rows.Count);

                        int rowIndex = 0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            dgrdDetails.Rows[rowIndex].Cells["id"].Value = dr["ID"];
                            dgrdDetails.Rows[rowIndex].Cells["BrandName"].Value = dr["brandName"];
                            dgrdDetails.Rows[rowIndex].Cells["suppliername"].Value = dr["SupplierName"];
                            dgrdDetails.Rows[rowIndex].Cells["minStock"].Value = dr["minStock"];
                            dgrdDetails.Rows[rowIndex].Cells["maxStock"].Value = dr["maxStock"];
                            dgrdDetails.Rows[rowIndex].Cells["profitMargin"].Value = dr["Margin"];
                            rowIndex++;
                        }
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex-1].Cells[1];
                        dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                        if (dgrdDetails.Rows.Count == 1)
                            BindAllDetails(dgrdDetails.Rows[0]);
                    }

                }
            }
            catch { }
        }

        private void BindAllDetails(DataGridViewRow row)
        {
            try
            {
                txtBrandName.Text = Convert.ToString(row.Cells["brandName"].Value);
                txtSupplierName.Text = Convert.ToString(row.Cells["suppliername"].Value);
                txtMinStock.Text = Convert.ToString(row.Cells["minStock"].Value);
                txtMaxStock.Text = Convert.ToString(row.Cells["maxStock"].Value);
                txtProfitMargin.Text = Convert.ToString(row.Cells["profitMargin"].Value);
                lblId.Text = Convert.ToString(row.Cells["Id"].Value);
                DisableAllControl();
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                string strPurchasePartyID = "";
                if (txtSupplierName.Text != "")
                {
                    string[] strFullParty = txtSupplierName.Text.Split(' ');
                    if (strFullParty.Length > 1)
                        strPurchasePartyID = strFullParty[0];
                }

                string strQuery = " if not exists (Select BrandName from [dbo].[BrandMaster] Where BrandName='" + txtBrandName.Text + "') begin INSERT INTO [dbo].[BrandMaster] ([BrandName],[PurchasePartyID],[MinStock],[MaxStock],[Margin],[Date],[Remark],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                + "('" + txtBrandName.Text + "','" + strPurchasePartyID + "'," + dba.ConvertObjectToDouble(txtMinStock.Text) + "," + dba.ConvertObjectToDouble(txtMaxStock.Text) + "," + dba.ConvertObjectToDouble(txtProfitMargin.Text) + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'','" + MainPage.strLoginName+"','',1,0) end ";

                int _count = dba.ExecuteMyQuery(strQuery);
                if (_count > 0)
                {
                    if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                        dba.DataMirroringInCurrentFinYear(strQuery);
                    DataBaseAccess.CreateDeleteQuery(strQuery);

                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewBrand)
                    {
                        StrAddedBrand = txtBrandName.Text;
                        this.Close();
                    }
                    else
                    {
                       
                        BindDataGrid();
                        lblMsg.Text = "";
                        dgrdDetails.Focus();
                    }
                }
            }
            catch { }
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
            txtBrandName.ReadOnly = txtMinStock.ReadOnly =txtMaxStock.ReadOnly = txtProfitMargin.ReadOnly = false;
            txtSearch.ReadOnly = true;
            dgrdDetails.Enabled = false;
        }

        private void DisableAllControl()
        {
            txtBrandName.ReadOnly =txtMinStock.ReadOnly = txtMaxStock.ReadOnly = txtProfitMargin.ReadOnly= true;
            txtSearch.ReadOnly = false;
            dgrdDetails.Enabled = true;
        }

        private void ClearAllText()
        {
            txtBrandName.Text = txtSupplierName.Text = txtSearch.Text = lblMsg.Text = "";
            txtMinStock.Text = txtMaxStock.Text =txtProfitMargin.Text= "0";
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
                    txtBrandName.Focus();
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
                            if (dgrdDetails.CurrentRow.Index > 0)
                            {
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.CurrentRow.Index - 1].Cells[1];
                                txtSearch.Focus();
                            }
                        }
                        else if (e.KeyCode == Keys.PageDown)
                        {
                            if (dgrdDetails.CurrentRow.Index < dgrdDetails.Rows.Count)
                            {
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.CurrentRow.Index + 1].Cells[1];
                                txtSearch.Focus();
                            }
                        }
                        else if (e.KeyCode == Keys.Home)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[1];
                            txtSearch.Focus();
                        }
                        else if (e.KeyCode == Keys.End)
                        {
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells[1];
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
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && lblId.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Brand", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        DataGridViewRow row = dgrdDetails.SelectedRows[0];
                        if (lblId.Text != "" && row != null)
                        {
                            if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                            {
                                string strOldBrandName = Convert.ToString(row.Cells["brandName"].Value);
                                string strQuery = "Delete from [BrandMaster] Where [BrandName]=" + strOldBrandName + "";
                                int i = dba.ExecuteMyQuery(strQuery);
                                if (i > 0)
                                {
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                    MessageBox.Show("Record is deleted successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    lblMsg.Text = "";
                                    ClearAllText();
                                    BindDataGrid();
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

        private void UpdateRecord()
        {
            try
            {
                DataGridViewRow row = dgrdDetails.SelectedRows[0];
                if (lblId.Text != "" && row != null)
                {
                    if (Convert.ToString(row.Cells["id"].Value) == lblId.Text)
                    {
                        string strPurchasePartyID = "";
                        if (txtSupplierName.Text != "")
                        {
                            string[] strFullParty = txtSupplierName.Text.Split(' ');
                            if (strFullParty.Length > 1)
                                strPurchasePartyID = strFullParty[0];
                        }

                        string strOldBrandName = Convert.ToString(row.Cells["brandName"].Value);

                        string query = "Update [BrandMaster] set BrandName='" + txtBrandName.Text + "',[PurchasePartyID]='" + strPurchasePartyID + "',[MinStock]=" + dba.ConvertObjectToDouble(txtMinStock.Text) + ",[MaxStock]=" + dba.ConvertObjectToDouble(txtMaxStock.Text) + ",[Margin]=" + dba.ConvertObjectToDouble(txtProfitMargin.Text)+" ,[UpdatedBy]='" + MainPage.strLoginName + "' Where [BrandName]='" + strOldBrandName + "'";
                        if (txtBrandName.Text != strOldBrandName)
                        {
                            query += "Update Items Set  BrandName='" + txtBrandName.Text + "' WHere  BrandName='" + strOldBrandName + "' "
                                  + " Update SalesBookSecondary Set BrandName = '" + txtBrandName.Text + "' WHere BrandName = '" + strOldBrandName + "' "
                                  + " Update PurchaseBookSecondary Set BrandName = '" + txtBrandName.Text + "' WHere BrandName = '" + strOldBrandName + "' "
                                  + " Update StockTransferSecondary Set BrandName = '" + txtBrandName.Text + "' WHere BrandName = '" + strOldBrandName + "' "
                                  + " Update StockMaster Set BrandName = '" + txtBrandName.Text + "' WHere BrandName = '" + strOldBrandName + "' ";
                        }

                        int count = dba.ExecuteMyQuery(query);
                        if (count > 0)
                        {
                            if (MainPage._bItemMirroring && !MainPage.bHSNWisePurchase)
                                dba.DataMirroringInCurrentFinYear(query);

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
                        MessageBox.Show("Sorry ! Please select right brand Name ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in brand Master.", ex.Message };
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
                if (txtBrandName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [BrandName] from [BrandMaster] Where [BrandName] ='" + txtBrandName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Brand Name : " + txtBrandName.Text+" already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtBrandName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtBrandName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select [BrandName] from [BrandMaster] Where [BrandName] ='" + txtBrandName.Text + "' and ID !=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Brand Name : " + txtBrandName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtBrandName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtBrandName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Brand Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtBrandName.Focus();
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
                        DataRow[] row = objTable.Select(String.Format("BrandName Like ('%" + txtSearch.Text + "%')"));
                        if (row.Length > 0)
                        {
                            int rowIndex = objTable.Rows.IndexOf(row[0]);
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[rowIndex].Cells[1];
                            dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                            if (dgrdDetails.Rows.Count == 1)
                                BindAllDetails(dgrdDetails.Rows[0]);
                        }
                    }
                    else
                    {
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count-1].Cells[1];
                        dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                        if (dgrdDetails.Rows.Count == 1)
                            BindAllDetails(dgrdDetails.Rows[0]);
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
                txtBrandName.ReadOnly = txtSupplierName.ReadOnly = true;
                if (dgrdDetails.Rows.Count > 0)
                {
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells[1];
                    dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                    BindAllDetails(dgrdDetails.CurrentRow);
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
                dgrdDetails.Focus();
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
                    if (dgrdDetails.SelectedRows.Count > 0)
                    {
                        if (dgrdDetails.SelectedRows[0] != null)
                        {
                            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                            {
                                BindAllDetails(dgrdDetails.SelectedRows[0]);
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
                if (!MainPage._bBrandWiseMargin)
                    txtProfitMargin.Enabled = false;

                if (IsNewBrand)
                {
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    panSearch.TabStop = pangrid.TabStop = txtSearch.TabStop = dgrdDetails.TabStop = false;
                    btnAdd.PerformClick();
                    txtBrandName.Text = __strName;
                    txtBrandName.Focus();
                }
            }
        }
      
        private void txtDecimalPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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
                    dgrdDetails.Enabled = txtSearch.Enabled = false;
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
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUPPLIER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtSupplierName.Text = objSearch.strSelectedData;
                                           }
                }
            }
            catch
            {
            }

        }

        private void txtProfitMargin_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            catch { }
        }

        private void txtProfitMargin_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "")
                    txtNew.Text = "0.00";
            }
            catch { }
        }

        private void BrandMaster_FormClosing(object sender, FormClosingEventArgs e)
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
