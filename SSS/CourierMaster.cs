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
    public partial class CourierMaster : Form
    {
        DataBaseAccess dba;
        DataTable dt;
        string GetCourierName = "", strSelectedCourier="",__strName="";
        public static string strAddedCourier = "";
        int __status = 0;
        
        public CourierMaster()
        {
            try
            {
                InitializeComponent();              
                dba = new DataBaseAccess();
                SelectName();
                txtCourier.ReadOnly = true;
                BindingListBoxWithCourier();
                lboxCourier.Focus();
                EditOption();
            }
            catch
            {
            }
        }
        
        public CourierMaster(int count, string strName)
        {
            try
            {
                InitializeComponent();              
                dba = new DataBaseAccess();               
                BindingListBoxWithCourier();              
                __status = count;
                __strName = strName;
            }
            catch
            {
            }
        }
      

        public void EditOption()
        {
            try
            {
                if (!(MainPage.mymainObject.bCourierAdd))
                {
                    btnAdd.Enabled = false;
                }
                if (!(MainPage.mymainObject.bCourierEdit))
                {
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                    txtSearch.Focus();
                }
                if (!(MainPage.mymainObject.bCourierView))
                {
                    this.Close();
                    MessageBox.Show("Sorry ! You don't have sufficeint permission to Access this Page ! ", "Permission Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }

        public void BindingListBoxWithCourier()
        {
            try
            {
                lboxCourier.Items.Clear();
                dt = dba.GetDataTable("select CourierName from CourierMaster order by CourierName");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lboxCourier.Items.Add(Convert.ToString(dr["CourierName"]));
                    }
                    if (lboxCourier.Items.Count > 0)
                    {
                        lboxCourier.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
            }
        }

        private void BindingRecord()
        {
            try
            {
                string strName = Convert.ToString(lboxCourier.SelectedItem);
                string StrQuery = "select * from CourierMaster where CourierName='" + strName + "'";
                DataTable table = dba.GetDataTable(StrQuery);
                if (table.Rows.Count > 0)
                {
                    txtCourier.Text = Convert.ToString(table.Rows[0][1]);
                    txtMobileNo.Text = Convert.ToString(table.Rows[0][2]);
                    txtAddress.Text = Convert.ToString(table.Rows[0][3]);
                }
            }
            catch { }
        }

        private void ClearAllRecord()
        {
            txtAddress.Clear();
            txtCourier.Clear();
            txtMobileNo.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add")
            {
                if (btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                    btnEdit.Text = "&Edit";
                }
                BindingListBoxWithCourier();
                txtCourier.ReadOnly = false;
                txtMobileNo.ReadOnly = false;
                txtAddress.ReadOnly = false;
                btnAdd.TabStop = true;
                ClearAllRecord();
                txtCourier.Focus();
                btnAdd.Text = "&Save";
            }
            else
            {
                SaveCourierRecord();
            }

        }

        private void SaveCourierRecord()
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (CheckAvailability())
                    {
                        DialogResult result = MessageBox.Show("Do You Want to Save The Record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " Insert into CourierMaster values ('" + txtCourier.Text + "','" + txtMobileNo.Text + "','" + txtAddress.Text + "','"+MainPage.strLoginName+"','',1,0 ) ";
                            int Count = dba.ExecuteMyQuery(strQuery);
                            if (Count > 0)
                            {
                                MessageBox.Show("Thank You ! Record Saved Successfully ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnAdd.Text = "&Add";
                                if (__status > 0)
                                {
                                    strAddedCourier = txtCourier.Text;
                                    this.Close();
                                    return;                                  
                                  
                                }
                                else
                                {                                   
                                    btnEdit.Text = "&Edit";
                                    ClearAllRecord();                                
                                    BindingListBoxWithCourier();
                                    lboxCourier.Focus();
                                    txtCourier.ReadOnly = true;
                                    txtAddress.ReadOnly = true;
                                    txtMobileNo.ReadOnly = true;                                   
                                    lblMsg.Visible = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Unable to Save Record ! Please Try after some time ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private bool CheckAvailability()
        {
            bool checkStatus = true;
            if (btnAdd.Text == "&Save")
            {
                try
                {
                    if (txtCourier.Text != "")
                    {
                        DataRow[] filteredRows = dt.Select(string.Format("CourierName='" + txtCourier.Text + "'"));
                        if (filteredRows.Length > 0)
                        {
                            lblMsg.Text = txtCourier.Text + "  is Already exist ! Please choose another Name..";
                            lblMsg.ForeColor = Color.Red;
                            lblMsg.Visible = true;
                            checkStatus = false;
                            txtCourier.Focus();
                        }
                        else
                        {
                            lblMsg.Text = txtCourier.Text + "  is Available ........";
                            lblMsg.ForeColor = Color.Green;
                            lblMsg.Visible = true;
                            checkStatus = true;
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Courier Name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        checkStatus = false;
                        txtCourier.Focus();

                    }
                }
                catch (Exception ex)
                {
                    string[] strReport = { "Exception occurred in Leave Event of Courier Name TextBox in Courier Master", ex.Message };
                    dba.CreateErrorReports(strReport);
                }
            }
            else
            {
                lblMsg.Visible = false;
            }
            return checkStatus;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CourierMaster_KeyDown(object sender, KeyEventArgs e)
        {
           // SelectName();
            try
            {
                if (e.KeyCode==Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode==Keys.Enter)
                {
                    SendKeys.Send("{Tab}");
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        GetPreviousRecord();
                    }
                }
                else if (e.KeyCode == Keys.Add)
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        GetNextRecord();
                    }
                }
                else if (e.KeyCode == Keys.Home)
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        GetFirstRecord();
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        GetLastRecord();
                    }
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Key Down Event of Form in New Courier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtCourier_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    lboxCourier.Text = txtCourier.Text;
                }
                if (btnAdd.Text == "&Save")
                {
                    SortingCourierName();
                    CheckAvailability();
                }
                else
                {
                    lblMsg.Visible = false;
                }
            }
            catch { }
        }

        private void SortingCourierName()
        {
            try
            {
                lboxCourier.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    if (txtCourier.Text.Length > 0)
                    {
                        DataRow[] dr = dt.Select(string.Format("CourierName Like ('" + txtCourier.Text + "%')"));

                        if (dr.Length > 0)
                        {
                            foreach (DataRow dr1 in dr)
                            {
                                lboxCourier.Items.Add(dr1["CourierName"]);
                            }
                            lboxCourier.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        foreach (DataRow dr1 in dt.Rows)
                        {
                            lboxCourier.Items.Add(dr1["CourierName"]);
                        }
                        lboxCourier.SelectedIndex = 0;
                    }
                }
            }
            catch { }
        }

        private void lboxCourier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text=="&Edit")
            {
                strSelectedCourier = Convert.ToString(lboxCourier.SelectedItem);
                if (strSelectedCourier != "")
                {
                    BindingRecord();
                }
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
                        BindingRecord();
                    }
                    txtCourier.Focus();
                    txtCourier.SelectionStart = txtCourier.Text.Length;
                    GetCourierName = txtCourier.Text;
                    txtCourier.ReadOnly = false;
                    txtMobileNo.ReadOnly = false;
                    txtAddress.ReadOnly = false;
                    btnEdit.Text = "&Update";
                    btnAdd.TabStop = false;
                    lboxCourier.Enabled = false;
                }
                else
                {
                    UpdateCourierRecord();
                }
            }
            catch { }

        }

        private void UpdateCourierRecord()
        {
            try
            {
                if (btnEdit.Text == "&Update")
                {
                    if (txtCourier.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Do You Want to Update The Record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            //strSelectedCourier = Convert.ToString(lboxCourier.SelectedItem);
                            if (strSelectedCourier != "")
                            {
                                object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from CourierMaster Where CourierName='" + strSelectedCourier + "' ");

                                string strQuery = " Update CourierMaster set CourierName='" + txtCourier.Text + "',MobileNo='" + txtMobileNo.Text + "',Address='" + txtAddress.Text + "',UpdatedBy='" + MainPage.strLoginName + "',UpdateStatus=1 where CourierName='" + strSelectedCourier + "'  "
                                             + " Update CourierRegister Set CourierName='" + txtCourier.Text + "' Where CourierName='" + strSelectedCourier + "' "
                                             + "  Update CourierRegisterIn Set CourierName='" + txtCourier.Text + "' Where CourierName='" + strSelectedCourier + "' ";
                                int count = dba.ExecuteMyQuery(strQuery);
                                if (count > 0)
                                {
                                    if (!Convert.ToBoolean(objStatus))
                                    {
                                        DataBaseAccess.CreateDeleteQuery(strQuery);
                                    }
                                    MessageBox.Show("Record Successfully Updated !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    lboxCourier.Enabled = true;
                                    //    BindingListBoxWithCourier();
                                    // BindingRecord();
                                    txtCourier.ReadOnly = true;
                                    txtAddress.ReadOnly = true;
                                    txtMobileNo.ReadOnly = true;
                                    txtSearch.Clear();
                                    lblMsg.Visible = false;
                                    btnEdit.Text = "&Edit";
                                    btnAdd.Text = "&Add";
                                    btnAdd.TabStop = true;
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! Unable to Updating Record ! Please Try after some time ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! this Record is Not exist in DataBase ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                BindingListBoxWithCourier();
                                btnEdit.Text = "&Edit";
                                lblMsg.Visible = false;
                                txtCourier.ReadOnly = true;
                            }

                        }
                    }
                    else
                    {

                        lblMsg.Text = "Please Choose Courier Name .......";
                        lblMsg.ForeColor = Color.Red;
                        lblMsg.Visible = true;
                        txtCourier.Focus();
                    }
                }
            }
            catch { }
        }

        private string GetId()
        {
            string strId = "", Query = "select CourierMaster.Id from CourierMaster where CourierName='" + GetCourierName + "' ";
            try
            {
                DataTable dt = dba.GetDataTable(Query);
                if (dt.Rows.Count > 0)
                {
                    strId = Convert.ToString(dt.Rows[0]["Id"]);
                }
            }
            catch { }
            return strId;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    btnAdd.Text = "&Add";
                    BindingListBoxWithCourier();
                    txtCourier.ReadOnly = true;
                    lblMsg.Visible = false;
                }
                else// if (btnEdit.Text == "&Update")
                {
                    btnEdit.Text = "&Edit";
                    txtCourier.ReadOnly = true;
                    DeleteCourierRecord();
                }               
                
            }
            catch { }
        }
        private void DeleteCourierRecord()
        {
            try
            {
                int selectedIndex;
                if (btnDelete.Text == "&Delete" && btnAdd.Text == "&Add")
                {
                    if (txtCourier.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Do You Want to Delete The Record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strSelectedName = Convert.ToString(lboxCourier.SelectedItem);
                            selectedIndex = lboxCourier.SelectedIndex;
                            if (strSelectedName != "")
                            {
                                int count = dba.ExecuteMyQuery("Delete from CourierMaster where CourierName='" + strSelectedName + "'");
                                if (count > 0)
                                {
                                    MessageBox.Show("Record Successfully Deleted !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    BindingListBoxWithCourier();
                                    txtCourier.ReadOnly = true;
                                    lboxCourier.Focus();
                                  //  lboxCourier.SelectedIndex = SelectItem;
                                    if (lboxCourier.Items.Count>selectedIndex)
                                    {
                                        lboxCourier.SelectedIndex = selectedIndex;
                                    }
                                    else
                                    {
                                        lboxCourier.SelectedIndex = selectedIndex - 1;
                                    }
                                    txtSearch.Clear();
                                    lblMsg.Visible = false;
                                    btnAdd.Text = "&Add";
                                    btnEdit.Text = "&Edit";
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! Unable to Delete Record ! Please Try after some time ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! this Record is not Available in DataBase ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                BindingListBoxWithCourier();
                                lblMsg.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! There is No Such Record in DataBase", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSearch.Focus();
                    }
                }
            }
            catch { }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearch.ReadOnly = false;
                txtSearch.Focus();
                txtSearch.Clear();
                BindingListBoxWithCourier();
                lblMsg.Visible = false;
                if (btnAdd.Text == "&Save")
                {
                    btnAdd.Text = "&Add";
                   // txtCourier.Text = lboxCourier.SelectedItem.ToString();
                    BindingRecord();
                }
                if (btnEdit.Text == "&Update")
                {
                    btnEdit.Text = "&Edit";
                    txtCourier.ReadOnly = true;
                }
               // BindingRecord();
            }
            catch { }
        }

        private void txtCourier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtCourier.Text.Length == 0 && char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSearch.Text.Length == 0 && char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    lboxCourier.Focus();
                }
                else if (e.KeyCode == Keys.Up)
                {
                    lboxCourier.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    lboxCourier.Focus();
                }
            }
            catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SortingCourierWithListBox();
        }
        private void SortingCourierWithListBox()
        {
            try
            {
                lboxCourier.Items.Clear();
                if (dt.Rows.Count > 0)
                {
                    if (txtSearch.Text.Length > 0)
                    {
                        DataRow[] dr = dt.Select(string.Format("CourierName Like ('" + txtSearch.Text + "%')"));

                        if (dr.Length > 0)
                        {
                            foreach (DataRow dr1 in dr)
                            {
                                lboxCourier.Items.Add(dr1["CourierName"]);
                            }
                            lboxCourier.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                       BindListBox();
                    }
                }
            }
            catch { }
        }

        private void BindListBox()
        {
            try
            {
                //txtCourier.ReadOnly = true;
                dt = dba.GetDataTable("select * from CourierMaster");
                if (dt != null)
                {
                    lboxCourier.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        lboxCourier.Items.Add(dr["CourierName"]);
                    }

                    lboxCourier.SelectedIndex = 0;
                    string StrCourierName = "";
                    try
                    {
                        StrCourierName = Convert.ToString(lboxCourier.SelectedItem);
                    }
                    catch { }
                    DataRow[] drow = dt.Select(String.Format("CourierName='" + StrCourierName + "'"));
                    if (drow.Length > 0)
                    {
                        if (btnAdd.Text != "&Save")
                        {
                            lblName.Text = Convert.ToString(drow[0]["Id"]);
                            txtCourier.Text = Convert.ToString(drow[0]["CourierName"]);
                            txtMobileNo.Text = Convert.ToString(drow[0]["MobileNo"]);
                            txtAddress.Text = Convert.ToString(drow[0]["Address"]);

                            //BindRecordWithRow(drow[0]);
                        }
                    }
                }
            }
            catch
            { }
        }
        private void BindRecordWithRow(DataRow dr)
        {
            try
            {
                txtCourier.Text = dr["CourierName"].ToString();
                lblName.Text = dr["Id"].ToString();
            }
            catch { }
        }

        private void SelectName()
        {
            try
            {
                string Query = ("select distinct CourierName from CourierMaster where CourierName='" + txtCourier.Text + "'");
                DataTable dt = dba.GetDataTable(Query);
                if (dt.Rows.Count > 0)
                {
                    lblName.Text = Convert.ToString(dt.Rows[0]["CourierName"]);
                }
            }
            catch
            {
            }
        }

        private void GetNextRecord()
        {
            try
            {
                if (lboxCourier.Items.Count>1)
                {
                    int itemIndex = lboxCourier.SelectedIndex;
                    if (itemIndex < lboxCourier.Items.Count - 1)
                    {
                        lboxCourier.SelectedIndex = itemIndex + 1;
                    }
                }
            }
            catch { }
        }

        private void GetPreviousRecord()
        {
            try
            {
                if (lboxCourier.Items.Count > 1)
                {
                    int itemIndex = lboxCourier.SelectedIndex;
                    if (itemIndex >0)
                    {
                        lboxCourier.SelectedIndex = itemIndex - 1;
                    }
                    //string StrName = lblName.Text;
                    //string strNextRecord = ("Select Max(CourierName) from CourierMaster where CourierName <('" + StrName + "')");
                    //if (strNextRecord != "")
                    //{
                    //    DisplayAllRecord(strNextRecord);
                    //}
                }
            }
            catch { }
        }

        private void DisplayAllRecord(string strNext)
        {
            string Query = ("select distinct CourierName,Id from CourierMaster where CourierName=(" + strNext + ")");
            DataTable d1 = dba.GetDataTable(Query);
            if (d1.Rows.Count > 0)
            {
                foreach (DataRow dr in d1.Rows)
                {
                    txtCourier.Text = Convert.ToString(dr["CourierName"]);
                    lblName.Text = dr["CourierName"].ToString();
                }
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
                    lboxCourier.Enabled = txtSearch.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return false;
            }
        }

        private void CourierMaster_Load(object sender, EventArgs e)
        {
            try
            {
                if (SetPermission())
                {
                    if (__status>0)
                    {
                        btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                        txtSearch.TabStop = lboxCourier.TabStop = false;
                        btnAdd.PerformClick();
                        txtCourier.Text = __strName;
                        txtCourier.Focus();
                    }
                }
            }
            catch { }
        }

        private void GetFirstRecord()
        {
            try
            {
                if (lboxCourier.Items.Count > 1)
                {
                    lboxCourier.SelectedIndex = 0;
                    //string strNextRecord = ("select min(CourierName) from CourierMaster");
                    //if (strNextRecord != "")
                    //{
                    //    DisplayAllRecord(strNextRecord);
                    //}
                }
            }
            catch { }
        }

        private void CourierMaster_FormClosing(object sender, FormClosingEventArgs e)
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

        private void GetLastRecord()
        {
            try
            {
               if (lboxCourier.Items.Count > 1)
                {
                    lboxCourier.SelectedIndex = lboxCourier.SelectedIndex - 1;
                    //string strNextRecord = ("select max(CourierName) from CourierMaster");
                    //if (strNextRecord != "")
                    //{
                    //    DisplayAllRecord(strNextRecord);
                    //}
                   
                }
            }
            catch { }
        }

        private void lboxCourier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                txtSearch.Text += e.KeyChar.ToString();
                txtSearch.Focus();
                txtSearch.Select(txtSearch.Text.Length, 0);
            }
            else if (e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Space))
            {
                txtSearch.Focus();
                txtSearch.Select(txtSearch.Text.Length, 0);
            }
        }      
    }
}
