using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace SSS
{
    public partial class CompanyMaster : Form
    {
        DataBaseAccess dba = null;
        CreateCompany createCompany;
        int statusCode = 0;
       
        public CompanyMaster()
        {
            try
            {
                dba = new DataBaseAccess();
                InitializeComponent();
                createCompany = new CreateCompany();
                txtCompanyCode.Text = dba.GetCompanyCode().ToString("0");
                txtCompanyCode.Focus();
                txtStartDate.Text  = txtEndDate.Text= DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Constructor in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        public CompanyMaster(int status)
        {
            try
            {
                dba = new DataBaseAccess();
                InitializeComponent();
                createCompany = new CreateCompany();
                txtCompanyCode.Text = dba.GetCompanyCode().ToString("0");
                txtCompanyCode.Focus();
                txtStartDate.Text = txtEndDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                statusCode = 1;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Constructor in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public CompanyMaster(string status)
        {
            try
            {
                dba = new DataBaseAccess();
                InitializeComponent();
                createCompany = new CreateCompany();
                btnSubmit.Text = "Up&date";
                btnDelete.Visible = true;
                BindDataWithControls();
                txtCompanyCode.ReadOnly = true;
                txtName.Focus();
                if (!MainPage.strLoginName.Contains("ADMIN"))
                    btnDelete.Enabled = false;
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Constructor in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindDataWithControls()
        {
            DataTable dt = dba.GetCompanyRecord();
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                
                txtCompanyCode.Text = Convert.ToString(dr["CompanyID"]);
                txtName.Text = Convert.ToString(dr["CompanyName"]);
                txtBackupGroup.Text = Convert.ToString(dr["BackupGroup"]);
                txtAddress.Text = Convert.ToString(dr["Address"]);
                txtCity.Text = Convert.ToString(dr["City"]);
                txtCountry.Text = Convert.ToString(dr["Country"]);
                txtPhoneOff1.Text = Convert.ToString(dr["PhoneOff1"]);
                txtPhoneOff2.Text = Convert.ToString(dr["PhoneOff2"]);
                txtPhoneRes.Text = Convert.ToString(dr["PhoneRes"]);
                txtLiveDBIP.Text = Convert.ToString(dr["MobileNo1"]);
                txtMobileNo2.Text = Convert.ToString(dr["MobileNo2"]);
                txtNextYrPath.Text  = Convert.ToString(dr["Next_Y_Path"]);
                txtPrevYrPath.Text = Convert.ToString(dr["Prev_Y_Path"]);
                txtVatRegNo.Text = Convert.ToString(dr["Vat_RegNo"]);
                txtAreaCode.Text = Convert.ToString(dr["CurrencyBase"]);
                txtServerDataBase.Text = Convert.ToString(dr["CurrencyUnit"]);
                txtOnlineDBName.Text = Convert.ToString(dr["Offline_Path"]);
                txtStartDate.Text = Convert.ToString(dr["SDate"]);
                txtEndDate.Text = Convert.ToString(dr["EDate"]);                                
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                btnSubmit.Enabled = false;
                if (ValidateData())
                    SubmitData();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Submit Button in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }            
            btnSubmit.Enabled = true;
        }

        private void SubmitData()
        {
            int result = 0;
            DialogResult dr = MessageBox.Show("Are you sure want to  Save Record .....", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (btnSubmit.Text == "&Submit")
                {                   
                    try
                    {
                       // if (rdoHeadOffice.Checked)
                       // {
                            object objValue = "";
                            try
                            {
                                string strQuery = "select * from master.dbo.sysdatabases where name= 'A" + txtCompanyCode.Text + "' ";
                                objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                            }
                            catch
                            {
                            }
                            if (Convert.ToString(objValue) == "")
                            {
                                CreateDataBase();
                                SaveEntryCompanyRecord();
                                result = SaveData();
                            }
                            else
                            {
                                MessageBox.Show("Please choose Another Company Code because this code is Already in Use !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                      //  }
                        //else if (rdoBranchOffice.Checked)
                        //{
                        //    result = SaveData();
                        //}
                    }
                    catch (Exception ex)
                    {
                        DeleteCompany();
                        MessageBox.Show("Some Problems occurred ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (statusCode == 1)
                    {
                        this.Close();
                    }
                    else
                    {
                        try
                        {
                            MainPage.con.Close();
                            MainPage.con.ConnectionString = "Data Source=" + MainPage.strComputerName + @"\SQLEXPRESS; Initial Catalog=" + MainPage.strDataBaseFile + "; User ID=sss;password="+MainPage.strDBPwd+";";

                            MainPage.con.Open();
                        }
                        catch
                        {
                            this.Close();
                        }
                    }
                }
                else if (btnSubmit.Text == "Up&date")
                {
                    UpdateData();
                }
            }
        }

        private bool ValidateData()
        {
            if (txtCompanyCode.Text == "")
            {
                MessageBox.Show("Sorry ! Company code is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyCode.Focus();
                return false;
            }
            if (txtName.Text == "")
            {
                MessageBox.Show("Sorry ! Company Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }                  
            if (txtStartDate.Text.Length!=10)
            {
                MessageBox.Show("Sorry ! Start date is not valid ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStartDate.Focus();
                return false;
            }
            if (txtEndDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! End date is not valid ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEndDate.Focus();
                return false;
            }

            return true;
        }

        private int SaveData()
        {
            DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);
            string[] record = new string[30];
            record[0] = txtCompanyCode.Text;
            record[1] = txtName.Text;
            record[2] = txtBackupGroup.Text;
            record[3] = txtAddress.Text;
            record[4] = txtCity.Text;
            record[5] = txtCountry.Text;
            record[6] = txtPhoneOff1.Text;
            record[7] = txtPhoneOff2.Text;
            record[8] = txtPhoneRes.Text;
            record[9] = txtLiveDBIP.Text;
            record[10] = txtMobileNo2.Text;
            record[11] = txtNextYrPath.Text;
            record[12] = txtPrevYrPath.Text;
            record[13] = txtVatRegNo.Text;
            record[14] = txtAreaCode.Text;
            record[15] = txtServerDataBase.Text;
            record[16] = sDate.ToString("MM/dd/yyyy");
            record[17] = eDate.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("MM/dd/yyyy hh:mm:ss tt"); 
            record[18] = txtOnlineDBName.Text;

            int count = 0;
          //  if (rdoHeadOffice.Checked)
            //{
                count = dba.SaveNewCompany(record);
            //}
            //else if (rdoBranchOffice.Checked)
            //{
            //    count = dba.SaveBranchCompany(record);
            //}
            
            if (count > 0)
            {                
                MessageBox.Show("Thank You ! Record successfully saved...........", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ClearAllData();
                txtCompanyCode.Text = dba.GetCompanyCode().ToString("000");
            }
            return count;
        }
       
        private void UpdateData()
        {
            DateTime sDate = dba.ConvertDateInExactFormat(txtStartDate.Text), eDate = dba.ConvertDateInExactFormat(txtEndDate.Text);
            eDate = eDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            string[] record = new string[30];
            record[0] = txtCompanyCode.Text;
            record[1] = txtName.Text.Trim();
            record[2] = txtBackupGroup.Text;
            record[3] = txtAddress.Text;
            record[4] = txtCity.Text;
            record[5] = txtCountry.Text;
            record[6] = txtPhoneOff1.Text;
            record[7] = txtPhoneOff2.Text;
            record[8] = txtPhoneRes.Text;
            record[9] = txtLiveDBIP.Text;
            record[10] = txtMobileNo2.Text;
            record[11] = txtNextYrPath.Text;
            record[12] = txtPrevYrPath.Text;
            record[13] = txtVatRegNo.Text;
            record[14] = txtAreaCode.Text;
            record[15] = txtServerDataBase.Text;
            record[16] = sDate.ToString("MM/dd/yyyy");
            record[17] = eDate.ToString("MM/dd/yyyy hh:mm:ss tt"); 
            record[18] = txtOnlineDBName.Text;

            int count = dba.UpdateCompany(record);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Record  Successfully Updated...........","Message",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
               
                MainPage.strCompanyName = txtName.Text;
                if (MainPage._bPaidStatus)
                {
                    MainPage.startFinDate = sDate;
                    MainPage.endFinDate = eDate;
                }
                MainPage.strOnlineDataBaseName = txtOnlineDBName.Text;
                MainPage.strServerDataBaseName = txtServerDataBase.Text;
                MainPage.strLiveDataBaseIP = txtLiveDBIP.Text ;
                SaveEntryCompanyRecord();
            }
        }

        private void SaveEntryCompanyRecord()
        {
            string[] record = new string[3];
            record[0] = txtCompanyCode.Text;
            record[1] = txtName.Text;
            record[2] = txtName.Text + txtCompanyCode.Text;
            dba.SaveRecordInStartingEntry(record);
        }      

        private void txtPhoneRes_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender,e,0);
        }            
              
        private void ClearAllData()
        {
            try
            {
                txtAddress.Clear();
                txtBackupGroup.Clear();               
                txtCity.Clear();
                txtCountry.Clear();
                txtAreaCode.Clear();
                txtServerDataBase.Clear();
                txtLiveDBIP.Clear();
                txtMobileNo2.Clear();
                txtName.Clear();
                txtNextYrPath.Clear();
                txtOnlineDBName.Clear();
                txtPhoneOff1.Clear();
                txtPhoneOff2.Clear();
                txtPhoneRes.Clear();
                txtPrevYrPath.Clear();
                txtVatRegNo.Clear();
                txtStartDate.Text = txtEndDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Clearing all TextBoxes in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void txtCity_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void NewCompany_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode==Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !txtAddress.Focused)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Forms Key Down in New Company", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnNextBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowDialog();

            txtNextYrPath.Text = browse.SelectedPath;
        }

        private void btnPreBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowDialog();

            txtPrevYrPath.Text = browse.SelectedPath;
        }

        private void CreateDataBase()
        {
            string strPath = MainPage.strServerPath;      
            Directory.CreateDirectory(strPath + "\\Data\\" + txtCompanyCode.Text);
            string path = strPath + "\\Data\\" + txtCompanyCode.Text;
            createCompany.CreateDataBase("A" + txtCompanyCode.Text, path);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtCompanyCode.Text != "")
            {
                DialogResult dr = MessageBox.Show("Are you sure want to  Delete "+MainPage.strCompanyName+" ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    DeleteCompany();
                    MessageBox.Show("Company Deleted Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    try
                    {
                        SelectCompany sc = new SelectCompany();
                        sc.ShowDialog();
                        if (sc.strCompCode != "")
                        {
                            MainPage.strDataBaseFile = "A" + sc.strCompCode;
                            MainPage.ChangeDataBase(MainPage.strDataBaseFile);

                        }
                        else
                        {
                            MainPage myObj = MainPage.mymainObject as MainPage;
                            myObj.Close();
                        }
                    }
                    catch
                    {
                    }
                    this.Close();
                }

            }
        }
       
        private void DeleteCompany()
        {
            try
            {
                if (txtCompanyCode.Text != "")
                {
                    dba.DeleteCompany("A" + txtCompanyCode.Text);
                    DirectoryInfo folder = new DirectoryInfo(MainPage.strServerPath + "\\Data\\" + txtCompanyCode.Text);               
                    if (folder.Exists)
                    {
                        Directory.Delete(MainPage.strServerPath + "\\Data\\" + txtCompanyCode.Text, true);             
                    }
                }
            }
            catch
            {
            }
        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtAddress.SelectionStart < 2)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void NewCompany_Load(object sender, EventArgs e)
        {

        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, false, false);
        }

        private void txtStartDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
    }
}

