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
    public partial class MessageMaster : Form
    {
        DataBaseAccess dba;
        public MessageMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindRecord();
        }

        private void BindRecord()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from MessageMaster");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtURL.Text = Convert.ToString(row["URL"]);
                    txtSenderId.Text = Convert.ToString(row["SenderID"]);
                    txtUserName.Text = Convert.ToString(row["UserName"]);
                    txtPassword.Text = Convert.ToString(row["Password"]);
                    txtMessageType.Text = Convert.ToString(row["MessageType"]);
                }
                else
                {
                    btnEdit.Text = "&Add";
                    ClearTextBox();
                }
            }
            catch (Exception ex)
            { }
        }

        private void UpdateMainPageSMSPath()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from MessageMaster");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    MainPage.strSMSURL = Convert.ToString(row["URL"]);
                    MainPage.strSenderID = Convert.ToString(row["SenderID"]);
                    MainPage.strSMSUser = Convert.ToString(row["UserName"]);
                    MainPage.strSMSPassword = Convert.ToString(row["Password"]);
                    MainPage.strMessageType = Convert.ToString(row["MessageType"]);
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Add")
                {
                    btnEdit.Text = "&Save";
                    btnDelete.Enabled = false;
                    EnableAllControl();
                }
                else if (btnEdit.Text == "&Save")
                {
                    if (txtURL.Text != "" && txtSenderId.Text != "")
                    {
                        string strQuery = "Insert into MessageMaster values ('" + txtURL.Text + "','" + txtSenderId.Text + "','" + txtUserName.Text + "','" + txtPassword.Text + "','" + txtMessageType.Text + "')";
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            btnDelete.Enabled = true;
                            BindRecord();
                            UpdateMainPageSMSPath();
                            DisableAllControl();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill the Mendatory Fields...");
                    }
                }
                else if (btnEdit.Text == "&Edit")
                {
                    btnEdit.Text = "&Update";
                    btnDelete.Enabled = false;
                    EnableAllControl();

                }
                else if (btnEdit.Text == "&Update")
                {
                    if (txtURL.Text != "" && txtSenderId.Text != "")
                    {
                        string strQuery = "update MessageMaster set URL='" + txtURL.Text + "',SenderId='" + txtSenderId.Text + "',UserName='" + txtUserName.Text + "',Password='" + txtPassword.Text + "',MessageType='" + txtMessageType.Text + "'";
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            btnDelete.Enabled = true;
                            BindRecord();
                            UpdateMainPageSMSPath();
                            DisableAllControl();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill the Mendatory Fields...");
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void EnableAllControl()
        {
            txtURL.ReadOnly = txtSenderId.ReadOnly = txtUserName.ReadOnly = txtPassword.ReadOnly = txtMessageType.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtURL.ReadOnly = txtSenderId.ReadOnly = txtUserName.ReadOnly = txtPassword.ReadOnly = txtMessageType.ReadOnly = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnEdit.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void MessageMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtURL_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtSenderId_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtMessageType_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "&Edit")
            {
                string strQuery = "Delete from MessageMaster";
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you ! Record Deleted successfully . ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Add";
                    BindRecord();
                    DisableAllControl();
                }
            }
        }

        private void ClearTextBox()
        {
            txtURL.Text = txtSenderId.Text = txtUserName.Text = txtPassword.Text = txtMessageType.Text = "";
        }

        private bool SaveRecord(DataGridViewRow row)
        {
            if (row != null)
            {
                string strOLDTemplateName = Convert.ToString(row.Cells["oldTemplateName"].Value), strTemplateName = Convert.ToString(row.Cells["templateName"].Value), strTemplateID = Convert.ToString(row.Cells["templateID"].Value), strMessage = Convert.ToString(row.Cells["message"].Value);
                if (strTemplateID != "" && strTemplateName != "")
                {
                    string strQuery = "IF NOT EXISTS (SELECT ID FROM [dbo].[TemplateSetting] Where TemplateName='" + strOLDTemplateName + "') Begin INSERT INTO [dbo].[TemplateSetting] ([TemplateName],[TemplateID],[Message],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                    + " ('" + strTemplateName + "','" + strTemplateID + "','" + strMessage + "',Getdate(),'" + MainPage.strLoginName + "','',1,0) end else begin "
                                    + " UPDATE [dbo].[TemplateSetting] SET TemplateID='" + strTemplateID + "',TemplateName='" + strTemplateName + "',Message='" + strMessage + "',UpdatedBy='" + MainPage.strLoginName + "',UpdateStatus=1 Where TemplateName='" + strOLDTemplateName + "' end ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thanks you ! Record saved successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        GetAllRecord();
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save right now. Please again later", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return false;
        }

        private void GetAllRecord()
        {
            try
            {
                string strQuery = "";

                strQuery = " Select * from [dbo].[TemplateSetting] Order by Date desc ";

                DataTable objTable = DataBaseAccess.GetDataTableRecord(strQuery);
                dgrdTemplate.Rows.Clear();
                if (objTable.Rows.Count > 0)
                {
                    dgrdTemplate.Rows.Add(objTable.Rows.Count);
                    int index = 0;
                    foreach (DataRow row in objTable.Rows)
                    {
                        dgrdTemplate.Rows[index].Cells["ID"].Value = row["ID"];
                        dgrdTemplate.Rows[index].Cells["SNo"].Value = index + 1;
                        dgrdTemplate.Rows[index].Cells["templateID"].Value = row["TemplateID"];
                        dgrdTemplate.Rows[index].Cells["templateName"].Value = dgrdTemplate.Rows[index].Cells["oldTemplateName"].Value = row["TemplateName"];
                        dgrdTemplate.Rows[index].Cells["message"].Value = row["Message"];
                        index++;
                    }
                }
            }
            catch
            {
            }

            if (dgrdTemplate.Rows.Count == 0)
            {
                dgrdTemplate.Rows.Add(1);
                dgrdTemplate.Rows[0].Cells["SNo"].Value = 1;
            }
        }

        private void grdPinCodeD_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 5)
                {
                    try
                    {
                        DialogResult dr = MessageBox.Show("Are you sure you want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            SaveRecord(dgrdTemplate.Rows[e.RowIndex]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Somthing went wrong, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (e.ColumnIndex == 6)
                {
                    try
                    {
                        DialogResult dr = MessageBox.Show("Are you sure you want to delete record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            string strTemplateName = Convert.ToString(dgrdTemplate.CurrentRow.Cells["oldTemplateName"].Value);
                            string strQuery = " Delete from [dbo].[TemplateSetting] where TemplateName='" + strTemplateName + "' ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! Record saved successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                GetAllRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Unable to save record right now", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Somthing went wrong, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}