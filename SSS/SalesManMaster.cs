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
    public partial class SalesManMaster : Form
    {
        DataBaseAccess dba;
        public string StrSalesManName = "",__strName="";
        static int intStatus = 0;
        DataTable table = null;
        public SalesManMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindListBoxDetail();
        }

        public SalesManMaster(int count,string strName)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            intStatus = count;
            BindListBoxDetail();
            __strName = strName;
        }

        private void BindRecordWithControls(DataRow dr)
        {
            DisableAllControls();
            lblMsg.Text = "";
            txtMarketerName.Text = Convert.ToString(dr["MarketerName"]);
            txtEmailID.Text = Convert.ToString(dr["EmailId"]);
            txtAddress.Text = Convert.ToString(dr["Address"]);
            txtCity.Text = Convert.ToString(dr["City"]);            
            txtPhoneNo.Text = Convert.ToString(dr["MobileNoII"]);
            txtMobileNo.Text = Convert.ToString(dr["MobieNoI"]);
            
        }

        private void BindListBoxDetail()
        {
            try
            {
                table = dba.GetDataTable("Select * from Marketer Order by MarketerName");
                lboxAgentName.Items.Clear();
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        lboxAgentName.Items.Add(dr["MarketerName"]);
                    }
                    lboxAgentName.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Binding Details Name from SalesMan Master", ex.Message };
                dba.CreateErrorReports(StrReport);
            }
        }

        private void BindSearchListBox(TextBox txtBox)
        {
            try
            {
                lboxAgentName.Items.Clear();
                if (table != null)
                {
                    if (txtBox.Text == "")
                    {                        
                        foreach (DataRow dr in table.Rows)
                        {
                            lboxAgentName.Items.Add(dr["MarketerName"]);
                        }
                        lboxAgentName.SelectedIndex = table.Rows.Count - 1;
                    }
                    else
                    {
                        DataRow[] FilteredRows = table.Select(string.Format("MarketerName LIKE('" + txtBox.Text + "%')"));
                        if (FilteredRows.Length > 0)
                        {                           
                            foreach (DataRow dr in FilteredRows)
                            {
                                lboxAgentName.Items.Add(dr["MarketerName"]);
                            }
                            lboxAgentName.SelectedIndex = FilteredRows.Length - 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] StrReport = { "Error Occur on Binding Name from SalesMan Master", ex.Message };
                dba.CreateErrorReports(StrReport);
            }
        }

        private void ClearAllText()
        {
            txtMarketerName.Clear();
            txtAddress.Clear();
           
            txtEmailID.Clear();
            txtMobileNo.Clear();
            txtPhoneNo.Clear();
           
            txtCity.Clear();
            txtSearchAgent.Clear();
    
            lblMsg.Text = "";
        }

        private void EnableAllControls()
        {
            txtMarketerName.ReadOnly = false;
            txtAddress.ReadOnly = false;
            txtEmailID.ReadOnly = false;
            txtMobileNo.ReadOnly = false;
      
            txtPhoneNo.ReadOnly = false;
            

            txtSearchAgent.ReadOnly = true;
            lboxAgentName.Enabled = false;
        }

        private void DisableAllControls()
        {
            txtMarketerName.ReadOnly = true;
            txtAddress.ReadOnly = true;
            txtEmailID.ReadOnly = true;
            txtMobileNo.ReadOnly = true;
            
            txtPhoneNo.ReadOnly = true;
           
            txtSearchAgent.ReadOnly = false;
            lboxAgentName.Enabled = true;
        }

        public bool CheckDuplicacy()
        {
            if (txtMarketerName.Text != "")
            {
                if (btnAdd.Text == "&Save")
                {
                    int temp = dba.CheckMarkterAvailability(txtMarketerName.Text);
                    if (temp > 0)
                    {
                        lblMsg.Text = txtMarketerName.Text + " is already exist ! ";
                        lblMsg.ForeColor = Color.Red;
                        txtMarketerName.Focus();
                        return false;
                    }
                    else
                    {
                        lblMsg.Text = txtMarketerName.Text + " is available please proceed....";
                        lblMsg.ForeColor = Color.Green;
                        return true;
                    }
                }
                else if (btnEdit.Text == "&Update")
                {
                    if (txtMarketerName.Text != Convert.ToString(lboxAgentName.SelectedItem))
                    {
                        int temp = dba.CheckMarkterAvailability(txtMarketerName.Text);
                        if (temp > 0)
                        {
                            lblMsg.Text = txtMarketerName.Text + " is already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtMarketerName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtMarketerName.Text + " is available please proceed....";
                            lblMsg.ForeColor = Color.Green;
                            return true;
                        }
                    }
                    else
                    {
                        lblMsg.Text = txtMarketerName.Text + " is available please proceed....";
                        lblMsg.ForeColor = Color.Green;
                        return true;
                    }
                }
            }
            else
            {
                lblMsg.Text = "SalesMan Name is required ! ";
                lblMsg.ForeColor = Color.Red;
                txtMarketerName.Focus();
                return false;
            }
            return false;
        }

        private void SaveAgentMaster()
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
                    btnAdd.Text = "&Save";
                    btnEdit.Text = "&Edit";
                    ClearAllText();
                    EnableAllControls();
                    txtMarketerName.Focus();
                }
                else if (btnAdd.Text == "&Save")
                {
                    if (CheckDuplicacy())
                    {
                        DialogResult dr = MessageBox.Show("Are you sure you want to save record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            string[] record = new string[12];
                            record[0] = txtMarketerName.Text;
                            record[1] = txtMobileNo.Text;
                            record[2] = txtPhoneNo.Text;
                            record[3] = txtEmailID.Text;
                            record[4] = txtAddress.Text;
                            record[5] = txtCity.Text;
                            record[6] = DateTime.Now.ToString("MM/dd/yyyy");
                            record[7] = "0";// txtOrderNoFrom.Text;
                            record[8] = "0";// txtOrderNoTo.Text;

                            int result = dba.SaveMarketer(record);

                          //  string strQuery = " Insert into AgentMaster(AgentName,EmailId,Address,Date,State,City,Pincode,MobileNo,Rate,TelPhone,TDS)values('" + txtMarketerName.Text + "','" + txtEmailID.Text + "','" + txtAddress.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + txtState.Text + "','" + txtCity.Text + "','" + txtZipCode.Text + "','" + txtMobileNo.Text + "','" + txtCommRate.Text + "','" + txtPhoneNo.Text + "',"+txtTDS.Text+")";
                          //  int result = dba.ExecuteQuery(strQuery);
                            if (result > 0)
                            {
                                MessageBox.Show("Thanks ! Record saved  successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnAdd.Text = "&Add";
                                if (intStatus > 0)
                                {
                                    StrSalesManName = txtMarketerName.Text;
                                    this.Close();
                                }
                                else
                                {
                                    lblMsg.Text = "";
                                    BindListBoxDetail();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Unable to save Record in Database ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error Saving Record in SalesMan Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public void UpdateAgent()
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
                        BindListBoxDetail();
                        btnAdd.Text = "&Add";
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    txtMarketerName.Focus();
                }
                else if (CheckDuplicacy())
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to update details  ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strSelectedAgent = Convert.ToString(lboxAgentName.SelectedItem);
                        if (strSelectedAgent != "")
                        {
                            string[] record = new string[13];
                            record[0] = txtMarketerName.Text;
                            record[1] = txtMobileNo.Text;
                            record[2] = txtPhoneNo.Text;
                            record[3] = txtEmailID.Text;
                            record[4] = txtAddress.Text;
                            record[5] = txtCity.Text;
                            record[6] = DateTime.Now.ToString("MM/dd/yyyy");
                            record[7] = "0";// txtOrderNoFrom.Text;
                            record[8] = "0";// txtOrderNoTo.Text;
                            record[9] = strSelectedAgent;

                            int result = dba.UpdateMarketer(record);

                           // string strQuery = " Update AgentMaster set AgentName='" + txtMarketerName.Text + "',EmailID='" + txtEmailID.Text + "',Address='" + txtAddress.Text + "', State='" + txtState.Text + "',City='" + txtCity.Text + "',PINCode='" + txtZipCode.Text + "',TelPhone='" + txtPhoneNo.Text + "',MobileNo='" + txtMobileNo.Text + "', Rate='" + txtCommRate.Text + "',TDS="+txtTDS.Text+" Where AgentName='" + strSelectedAgent + "'  ";
                           // int result = dba.ExecuteQuery(strQuery);
                            if (result > 0)
                            {
                                MessageBox.Show("Thank you ! Record  updated  successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnEdit.Text = "&Edit";
                                BindListBoxDetail();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Record is not updated..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                            MessageBox.Show("Please select sales man from list !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error Updateing Record in SalesMan Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public void DeleteAgentRecord()
        {
            try
            {
                string strSelectedAgentName = Convert.ToString(lboxAgentName.SelectedItem);
                if (strSelectedAgentName != "")
                {
                    if (MessageBox.Show("Are you sure you want to delete this sales man details ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string _Query = " Delete From Marketer Where MarketerName='" + strSelectedAgentName + "' ";
                        int temp = dba.ExecuteMyQuery(_Query);
                        if (temp == 1)
                        {
                            MessageBox.Show("Thank you ! sales man deleted  successfully ..", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnAdd.Text = "&Add";
                            btnEdit.Text = "&Edit";
                            BindListBoxDetail();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Record not deleted ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        MessageBox.Show("Please select sales man name from list !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error Deleting Record in SalesMan Master", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtDetails_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                BindSearchListBox(txtSearchAgent);
        }

        private void txtDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    lboxAgentName.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    lboxAgentName.Focus();
                }
            }
            catch { }
        }

        private void lboxAgentDetials_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (table == null || table.Rows.Count == 0)
                        table = dba.GetDataTable(" Select * from Marketer Order by MarketerName ");
                    DataRow[] rows = table.Select(String.Format("MarketerName='" + Convert.ToString(lboxAgentName.SelectedItem) + "'"));
                    if (rows.Length > 0)
                        BindRecordWithControls(rows[0]);
                }
            }
            catch
            {
            }
        }

        private void AgentMaster_KeyDown(object sender, KeyEventArgs e)
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
                else if (e.KeyCode == Keys.PageDown)
                {
                    txtSearchAgent.Focus();
                    if (lboxAgentName.SelectedIndex < lboxAgentName.Items.Count - 1)
                        lboxAgentName.SelectedIndex = lboxAgentName.SelectedIndex + 1;
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    txtSearchAgent.Focus();
                    if (lboxAgentName.SelectedIndex > 0)
                        lboxAgentName.SelectedIndex = lboxAgentName.SelectedIndex - 1;
                }
                else if (e.KeyCode == Keys.Home)
                {
                    txtSearchAgent.Focus();
                    lboxAgentName.SelectedIndex = 0;
                }
                else if (e.KeyCode == Keys.End)
                {
                    txtSearchAgent.Focus();
                    lboxAgentName.SelectedIndex = lboxAgentName.Items.Count - 1;
                }
            }
            catch { }
        }


        private void txtDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (txtSearchAgent.Text.Length == 0)
                {
                    if (Char.IsWhiteSpace(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
            }
            catch { }
        }
       

        private void txtParty_TextChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
            {
                BindSearchListBox(txtMarketerName);
            }
        }

        private void lboxAgentDetials_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (char.IsLetter((Char)e.KeyCode))
                {
                    txtSearchAgent.Focus();
                    txtSearchAgent.Text = txtSearchAgent.Text + e.KeyCode;
                    txtSearchAgent.Select(txtSearchAgent.TextLength, 0);
                }
            }
            catch { }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SaveAgentMaster();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            UpdateAgent();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
                DeleteAgentRecord();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtSearchAgent.Clear();
            txtSearchAgent.Focus();
            BindListBoxDetail();
        }

        private void btnCls_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PointHandler(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox txtBox = sender as TextBox;
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(46) && !txtBox.Text.Contains('.'))
                {
                    e.Handled = false;
                }
                else
                {
                    if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
            }
            catch { }
        }

        private void txtCmsnRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            PointHandler(sender, e);
        }

        private void txtZipCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtParty_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save")
            {
                CheckDuplicacy();
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

        private void txtWithoutSpace(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt.Text.Length == 0)
                {
                    if (e.KeyChar == (char)Keys.Space)
                    {
                        e.Handled = true;
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
                    lboxAgentName.Enabled = txtSearchAgent.Enabled = false;
                return true;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return false;
            }
        }

        private void MarketerMaster_Load(object sender, EventArgs e)
        {
            try
            {
                bool _Status = SetPermission();
                if (_Status && intStatus > 0)
                {
                    panlist.TabStop = txtSearchAgent.TabStop = lboxAgentName.TabStop = false;
                    btnEdit.Enabled = btnDelete.Enabled = btnSearch.Enabled = false;
                    btnAdd.PerformClick();
                    txtMarketerName.Text = __strName;
                    txtMarketerName.Focus();

                }
            }
            catch { }
        }

        private void SalesManMaster_FormClosing(object sender, FormClosingEventArgs e)
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
