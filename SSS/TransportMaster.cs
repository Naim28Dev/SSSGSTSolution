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
    public partial class TransportMaster : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        bool IsNewTransport = false;
        public string StrAddedTransport = "",strSelectedTransport="",__strTransport="";
        public TransportMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataList();          
        }

        public TransportMaster(string strTransportName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            strSelectedTransport = strTransportName;
            BindDataList();
        }

        public TransportMaster(bool chk,string strTrpt)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindDataList();
            IsNewTransport = chk;
            __strTransport = strTrpt;
        }

        private void BindDataList()
        {
            try
            {
                lboxTransport.Items.Clear();
                objTable = dba.GetDataTable("Select * from Transport Order by TransportName");
                if (objTable != null)
                {                   
                    if (objTable.Rows.Count > 0)
                    {
                        string strTransport = "";
                        int _index = 0,_rowIndex=0;
                        foreach (DataRow dr in objTable.Rows)
                        {
                            strTransport = Convert.ToString(dr["TransportName"]);
                            lboxTransport.Items.Add(strTransport);
                            if (strSelectedTransport == strTransport)
                                _index = _rowIndex;
                            _rowIndex++;
                        }
                        lboxTransport.SelectedIndex = _index;
                    }

                }
            }
            catch { }
        }

        private void EnableAllControls()
        {
            txtTransportName.ReadOnly = txtMobileNo.ReadOnly = txtAddress.ReadOnly = txtContactI.ReadOnly = txtContactII.ReadOnly = txtSTDI.ReadOnly = txtSTDII.ReadOnly = txtPhoneI.ReadOnly = txtPhoneII.ReadOnly =txtGSTNo.ReadOnly=txtGreenTaxAmt.ReadOnly=txtForwardingCharges.ReadOnly=txtExtraCharges.ReadOnly= false;
            txtSearch.ReadOnly = true;
            lboxTransport.Enabled = false;
        }

        private void DisableAllControls()
        {
            txtTransportName.ReadOnly = txtMobileNo.ReadOnly = txtAddress.ReadOnly =  txtContactI.ReadOnly = txtContactII.ReadOnly = txtSTDI.ReadOnly = txtSTDII.ReadOnly = txtPhoneI.ReadOnly = txtPhoneII.ReadOnly =txtGSTNo.ReadOnly= txtGreenTaxAmt.ReadOnly = txtForwardingCharges.ReadOnly = txtExtraCharges.ReadOnly = true;
            txtSearch.ReadOnly = false;
            lboxTransport.Enabled = true;
        }

        private void ClearAllText()
        {
            txtTransportName.Text = txtMobileNo.Text = txtAddress.Text = txtGSTNo.Text = txtCity.Text = txtContactI.Text = txtContactII.Text = txtSTDI.Text = txtSTDII.Text = txtPhoneI.Text = txtPhoneII.Text = txtSearch.Text = lblMsg.Text= txtGreenTaxAmt.Text = txtForwardingCharges.Text = txtExtraCharges.Text = "";
        }

        private void BindAllDetails(DataRow row)
        {
            try
            {
                DisableAllControls();
                lblCreatedBy.Text = "";
                lblId.Text = Convert.ToString(row["ID"]);
                txtTransportName.Text = Convert.ToString(row["TransportName"]);
                txtContactI.Text = Convert.ToString(row["ContactPersonI"]);
                txtContactII.Text = Convert.ToString(row["ContactPersonII"]);
                // txtSTDI.Text = Convert.ToString(row["STDI"]);
                //txtSTDII.Text = Convert.ToString(row["STDII"]);
                //  string strPhoneI = Convert.ToString(row["PhoneNoI"]),strPhoneII= Convert.ToString(row["PhoneNoII"]);
                txtPhoneI.Text = Convert.ToString(row["PhoneNoI"]);
                txtPhoneII.Text = Convert.ToString(row["PhoneNoII"]);
                txtMobileNo.Text = Convert.ToString(row["MobileNo"]);
                txtAddress.Text = Convert.ToString(row["Address"]);
                txtCity.Text = Convert.ToString(row["City"]);
                txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                txtGreenTaxAmt.Text = Convert.ToString(row["GreenTaxAmt"]);
                txtForwardingCharges.Text = Convert.ToString(row["ForwardingCharges"]);
                txtExtraCharges.Text = Convert.ToString(row["ExtraCharges"]);

                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;
            }
            catch { }
        }
               
        private void SaveRecord()
        {
            try
            {
                string[] record = new string[15];

                record[0] = txtTransportName.Text.Trim();
                record[1] = txtContactI.Text;
                record[2] = txtContactII.Text;
                record[3] = txtPhoneI.Text;
                record[4] = txtPhoneII.Text;
                record[5] = txtMobileNo.Text;
                record[6] = txtCity.Text;
                record[7] = txtAddress.Text;
                record[8] = DateTime.Now.ToString("MM/dd/yyyy");
                record[9] = txtGSTNo.Text.Trim();
                record[10] = txtGreenTaxAmt.Text;
                record[11] = MainPage.strLoginName;
                record[12] = txtForwardingCharges.Text;
                record[13] = txtExtraCharges.Text;


                int count = dba.SaveNewTransport(record);
                if (count > 0)
                {
                    MessageBox.Show("Thank you..! Record submitted successfully ", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd.Text = "&Add";
                    if (IsNewTransport)
                    {                      
                        StrAddedTransport = txtTransportName.Text;
                        this.Close();
                    }
                    else
                    {                       
                        BindDataList();
                        lblMsg.Text = "";
                        txtSearch.Focus();
                    }
                }
            }
            catch { }
        }

        private bool ValidateControls()
        {
            if (txtTransportName.Text == "")
            {
                MessageBox.Show("Sorry ! Transport Name can't be blank ! ", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                txtTransportName.Focus();
                return false;
            }
            else if (txtGSTNo.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("GST No can't be Blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGSTNo.Focus();
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
                    ClearAllText();
                    EnableAllControls();
                    txtTransportName.Focus();
                    lblMsg.Text = "";
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

        private void TransportMaster_KeyDown(object sender, KeyEventArgs e)
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
                else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bAccountMasterEdit)
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        if (lboxTransport.SelectedIndex > 0)
                        {
                            lboxTransport.SelectedIndex = lboxTransport.SelectedIndex - 1;
                            txtSearch.Focus();
                        }
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        if (lboxTransport.SelectedIndex < lboxTransport.Items.Count)
                        {
                            lboxTransport.SelectedIndex = lboxTransport.SelectedIndex + 1;
                            txtSearch.Focus();
                        }
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        if (lboxTransport.Items.Count > 0)
                        {
                            lboxTransport.SelectedIndex = 0;
                            txtSearch.Focus();
                        }
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        if (lboxTransport.Items.Count > 0)
                        {
                            lboxTransport.SelectedIndex = lboxTransport.Items.Count-1;
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
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ? ", "Delete Transport Name", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strSeletedTransport = Convert.ToString(lboxTransport.SelectedItem);
                        if (strSeletedTransport != "")
                        {
                            int count = dba.DeleteTransport(strSeletedTransport);

                            if (count > 0)
                            {
                                MessageBox.Show("Record is deleted successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                lblMsg.Text = "";
                                ClearAllText();
                                BindDataList();
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
                if (lblId.Text != "")
                {
                    string strSeletedTransport = Convert.ToString(lboxTransport.SelectedItem);
                    if (strSeletedTransport != "")
                    {
                        string[] record = new string[15];

                        record[0] = txtTransportName.Text.Trim();
                        record[1] = txtContactI.Text;
                        record[2] = txtContactII.Text;
                        record[3] = txtPhoneI.Text;
                        record[4] = txtPhoneII.Text;
                        record[5] = txtMobileNo.Text;
                        record[6] = txtCity.Text;
                        record[7] = txtAddress.Text;
                        record[8] = DateTime.Now.ToString("MM/dd/yyyy");
                        record[9] = strSeletedTransport;
                        record[10] = txtGSTNo.Text.Trim();
                        record[11] = txtGreenTaxAmt.Text;
                        record[12] = MainPage.strLoginName;
                        record[13] = txtForwardingCharges.Text;
                        record[14] = txtExtraCharges.Text;

                        int count = dba.UpdateTransport(record);
                        if (count > 0)
                        {
                            MessageBox.Show("Record is updated successfully!..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMsg.Text = "";
                            btnEdit.Text = "&Edit";
                            BindDataList();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! your Record is not Updated..", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Update Record in Transport Master.", ex.Message };
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
                        BindDataList();
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
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
                if (txtTransportName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select TransportName from Transport Where TransportName ='" + txtTransportName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! "+txtTransportName.Text+" is already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTransportName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTransportName.Text+" is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;                           
                            return true;
                        }
                    }
                    else if (btnEdit.Text=="&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select TransportName from Transport Where TransportName ='" + txtTransportName.Text + "' and ID!=" + lblId.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! " + txtTransportName.Text + " is already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTransportName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTransportName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Transport Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtTransportName.Focus();
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
                lboxTransport.Items.Clear();
                if (objTable != null)
                {
                    if (txtSearch.Text == "")
                    {
                        foreach (DataRow dr in objTable.Rows)
                        {
                            lboxTransport.Items.Add(dr["TransportName"]);
                        }
                        lboxTransport.SelectedIndex = objTable.Rows.Count - 1;
                    }
                    else
                    {
                        DataRow[] FilteredRows = objTable.Select(string.Format("TransportName LIKE('%" + txtSearch.Text + "%')"));
                        if (FilteredRows.Length > 0)
                        {                            
                            foreach (DataRow dr in FilteredRows)
                            {
                                lboxTransport.Items.Add(dr["TransportName"]);
                            }
                            lboxTransport.SelectedIndex = FilteredRows.Length - 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Binding Transport Name from Transport Master", ex.Message };
                //dba.ApplicationReport(StrReport);
            }
        }

        private void tsbtnSearch_Click(object sender, EventArgs e)
        {           
                btnEdit.Text = "&Edit";
                btnAdd.Text = "&Add";
                BindDataList();
                txtSearch.Clear();
                txtSearch.Focus();
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
                lboxTransport.Focus();
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

   

        private void TransportMaster_Load(object sender, EventArgs e)
        {
            if (SetPermission())
            {
                if (IsNewTransport)
                {
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    pnlSearch.TabStop= txtSearch.TabStop = lboxTransport.TabStop = false;
                    btnAdd.PerformClick();
                    txtTransportName.Text = __strTransport;
                    txtTransportName.Focus();
                }
            }
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void lboxTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (char.IsLetter((Char)e.KeyCode))
                {
                    txtSearch.Focus();
                    txtSearch.Text = txtSearch.Text + e.KeyCode;
                    txtSearch.Select(txtSearch.TextLength, 0);
                }
            }
            catch { }
        }

        private void lboxTransport_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text=="&Edit")
                {
                    if (objTable != null)
                    {
                        DataRow[] rows = objTable.Select(String.Format("TransportName='" + Convert.ToString(lboxTransport.SelectedItem) + "'"));
                        if (rows.Length > 0)
                            BindAllDetails(rows[0]);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH STATION NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCity.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void txtGreenTaxAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails(txtTransportName.Text, "TRANSPORT", "0");

                    objEdit.ShowDialog();
                }
            }
            catch { }
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

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private bool SetPermission()
        {
            if ( MainPage.mymainObject.bAccountMasterAdd || MainPage.mymainObject.bAccountMasterEdit || MainPage.mymainObject.bAccountMasterView)
            {
                if (!MainPage.mymainObject.bAccountMasterAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bAccountMasterView)
                    lboxTransport.Enabled = txtSearch.Enabled = false;
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
