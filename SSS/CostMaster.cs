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
    public partial class CostMaster : Form
    {
        DataBaseAccess dba;
        DataTable dt = null;
        public string CostName = "";
        string strName = "", strSelectedItem = "";
        int id = 0;

        public CostMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetData();
        }

        public CostMaster(int chk)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetData();
            id = chk;
        }

        public CostMaster(string strCost)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetData();
            strName = "Update";
            BindItemData();
            btnSubmit.Text = "Up&date";
            btnDelete.Visible = true;
            lboxCost.Focus();
            txtSearchCostName.Visible = true;
        }

        private void StationMaster_Load(object sender, EventArgs e)
        {
            EditOption();
        }

        private void GetData()
        {
            dt = dba.GetDataTable("Select Distinct CostType from CostMaster");
        }

        private void BindListData()
        {
            try
            {
                lboxCost.Items.Clear();

                if (txtCostType.Text == "")
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        lboxCost.Items.Add(dr[0]);
                    }
                }
                else
                {
                    DataRow[] filteredRows = dt.Select(string.Format("{0} LIKE '%{1}%'", "CostType", txtCostType.Text));
                    if (filteredRows.Length > 0)
                    {
                        foreach (DataRow dr in filteredRows)
                        {
                            lboxCost.Items.Add(dr[0]);
                        }
                        lboxCost.SelectedIndex = 0;                     
                    }                    
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind  List Data in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SaveRecord()
        {
            try
            {
                string strQuery = "Insert into CostMaster Values('" + txtCostType.Text + "','" + MainPage.startFinDate + "',1,0) ";
                
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Entry Saved Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    CostName = txtCostType.Text;
                    txtCostType.Clear();
                    lblMsg.Text = "";
                    if (id > 0)
                    {
                        this.Close();
                        return;
                    }
                    else
                    {
                        GetData();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void UpdateRecord()
        {
            try
            {
                object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from CostMaster Where CostType='" + strSelectedItem + "' ");

                string strQuery = "Update CostMaster Set CostType='" + txtCostType.Text + "' Where CostType='"+strSelectedItem+"' ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (!Convert.ToBoolean(objStatus))
                    {
                        DataBaseAccess.CreateDeleteQuery(strQuery);
                    }
                    MessageBox.Show("Thank You ! Cost Centre Updated  Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    GetData();
                    txtSearchCostName.Clear();
                    BindItemData();
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Updating Record in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtStation_TextChanged(object sender, EventArgs e)
        {
            lboxCost.Visible = true;
            if (strName == "")
            {
                BindListData();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCostType.Text != "")
                {
                if (CheckDuplicacy())
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Save Cost Centre", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        
                            if (btnSubmit.Text == "&Submit")
                            {
                                SaveRecord();
                            }
                            else if (btnSubmit.Text == "Up&date")
                            {
                                UpdateRecord();
                            }

                           // dt = dba.GetStationName();
                            //txtCostType.Clear();
                            //txtSearchCostName.Clear();
                            //BindItemData();
                            lblMsg.Visible = false;                        
                    }
                }
                }
                else
                {
                    MessageBox.Show("Cost Centre Name can't be Blank ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Submit Button in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void StationMaster_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 27)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}"); // this.GetNextControl(ActiveControl, true).Focus();
                }
            }
            catch
            {
            }
        }

        private void txtStation_Leave(object sender, EventArgs e)
        {
            if (txtCostType.Text != strSelectedItem || strName == "")
            {
                if (txtCostType.Text != "")
                {
                    CheckDuplicacy();
                }
                else
                {
                    lblMsg.Text = "Please Choose Cost Type .......";
                    lblMsg.ForeColor = Color.Red;
                    lblMsg.Visible = true;
                    txtCostType.Focus();

                }
            }
            else
            {
                lblMsg.Visible = false;
            }
        }

        private bool CheckDuplicacy()
        {
            bool check = true;
            if (txtCostType.Text != strSelectedItem || strName == "")
            {
                DataRow[] filteredRows = dt.Select(string.Format("{0} LIKE '{1}'", "CostType", txtCostType.Text));
                if (filteredRows.Length > 0)
                {
                    lblMsg.Text = txtCostType.Text + "  is Already Exist ! Please choose another Name..";
                    lblMsg.ForeColor = Color.Red;
                    lblMsg.Visible = true;
                    txtCostType.Focus();
                    check = false;
                }
                else
                {
                    lblMsg.Text = txtCostType.Text + "  is Available ........";
                    lblMsg.ForeColor = Color.Green;
                    lblMsg.Visible = true;
                    check = true;
                }
            }
            return check;
        }

        private void BindItemData()
        {
            try
            {
                if (dt != null)
                {
                    lboxCost.Items.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {
                        lboxCost.Items.Add(dr[0]);
                    }
                    if (lboxCost.Items.Count > 0)
                    {
                        lboxCost.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Cost Type Data in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void lboxStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (strName != "")
                {
                    string strItem = Convert.ToString(lboxCost.SelectedItem);                 
                    strSelectedItem = strItem;
                    txtCostType.Text = strItem;
                }
            }
            catch
            {
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (strName != "" && strSelectedItem != "")
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Delete Cost Centre ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strQuery = "Delete from CostMaster Where CostType='" + strSelectedItem + "' ";
                        int result = dba.ExecuteMyQuery(strQuery);
                        if (result > 0)
                        {
                            MessageBox.Show("Cost Centre Deleted Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            GetData();
                            txtCostType.Clear();
                            txtSearchCostName.Clear();
                            BindListData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Delete Button in Cost Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtSearchStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    lboxCost.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lboxCost.Focus();
                }
            }
            catch
            {
            }
        }

        private void txtSearchStation_TextChanged(object sender, EventArgs e)
        {
            BindSearchListData();
        }

        private void BindSearchListData()
        {
            try
            {
                lboxCost.Items.Clear();

                if (txtSearchCostName.Text == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lboxCost.Items.Add(dr[0]);
                    }
                }
                else
                {
                    DataRow[] filteredRows = dt.Select(string.Format("{0} LIKE '{1}%'", "CostType", txtSearchCostName.Text));
                    if (filteredRows.Length > 0)
                    {
                        foreach (DataRow dr in filteredRows)
                        {
                            lboxCost.Items.Add(dr[0]);
                        }
                        lboxCost.SelectedIndex = 0;
                        lboxCost.Visible = true;
                    }                  
                }

            }
            catch
            {                
            }
        }

        private void lboxStation_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsLetter(e.KeyChar))
                {
                    txtSearchCostName.Text += e.KeyChar.ToString();
                    txtSearchCostName.Focus();
                    txtSearchCostName.Select(txtSearchCostName.Text.Length, 0);
                }
                else if (e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Space))
                {
                    txtSearchCostName.Focus();
                    txtSearchCostName.Select(txtSearchCostName.Text.Length, 0);
                }
            }
            catch
            {
            }
        }
   
        private void EditOption()
        {
            if (!Convert.ToBoolean(MainPage.mymainObject.bAccountMasterEdit))
            {
                if (btnSubmit.Text == "Up&date")
                    btnSubmit.Visible = false;
                btnDelete.Visible = false;
            }
        }
        
    }
}
