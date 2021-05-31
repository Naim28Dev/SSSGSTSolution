using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace SSS
{
    public partial class SelectCompany : Form
    {
        DataBaseAccess dba;
        MainPage mainObj;
        public string strCompCode = "";
        ArrayList FolderName;
        
        public SelectCompany()
        {
            InitializeComponent();
            mainObj = (MainPage)MainPage.mymainObject;
            FolderName = new ArrayList();
            dba = new DataBaseAccess();
            GetFirstFolderName();
        }

        private string GetCompanyCode(int row)
        {            
                string strCode = "";
                try
                {
                    strCode = FolderName[row].ToString();
                }
                catch
                {
                }
                return strCode;          
        }

        private void GetFirstFolderName()
        {
            try
            {
                bool chkStatus = true;
                string strPath = MainPage.strServerPath + "\\Data";
                DirectoryInfo objFolder = new DirectoryInfo(strPath);

                if (!objFolder.Exists)
                {
                    objFolder.Create();
                }
             
                if (chkStatus)
                {
                    string[] Folder;
                    Folder = Directory.GetDirectories(strPath);
                    foreach (string folderName in Folder)
                    {
                        FileInfo fi = new FileInfo(folderName);
                        FolderName.Add(fi.Name);
                    }
                }
                FolderName.Sort();
                if (FolderName.Count > 0)
                {
                    GetCompanyNameFromFile();
                }
                else
                {
                    CompanyMaster objNewCompany = new CompanyMaster(1);
                    objNewCompany.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objNewCompany.ShowDialog();
                    objFolder = new DirectoryInfo(strPath);
                    if (objFolder.Exists)
                        GetFolderName();
                    //GetFolderName();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in Select Company ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void GetFolderName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";          
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    string[] Folder;
                    Folder = Directory.GetDirectories(strPath);
                    foreach (string folderName in Folder)
                    {
                        FileInfo fi = new FileInfo(folderName);
                        FolderName.Add(fi.Name);
                    }
                    FolderName.Sort();
                }
                if (FolderName.Count > 0)
                {
                    GetCompanyNameFromFile();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in Select Company ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void GetCompanyNameFromFile()
        {
            try
            {
                int count = 1;
                string strPath = "";
                dgrdCompany.Rows.Clear();
                dgrdCompany.Rows.Add(FolderName.Count);

                foreach (string strName in FolderName)
                {
                    strPath = MainPage.strServerPath + "\\Data\\" + strName + "\\" + strName + ".syber";               
                    StreamReader sr = new StreamReader(strPath);
                    dgrdCompany.Rows[count - 1].Cells[0].Value = count;
                    dgrdCompany.Rows[count - 1].Cells[1].Value = sr.ReadLine();

                    sr.Close();
                    count++;

                }

                dgrdCompany.CurrentCell= dgrdCompany.Rows[dgrdCompany.RowCount - 1].Cells[0];
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Company Name from File in Select Company ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SelectCompany_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
         }

        private void dgrdCompany_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgrdCompany_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetCompanyName();
            }
        }
       
        private void GetCompanyName()
        {
            try
            {
                int rowIndex = dgrdCompany.CurrentRow.Index;

                string strCompany = "";
                try
                {
                    strCompany = Convert.ToString(dgrdCompany.Rows[rowIndex].Cells[1].Value);
                }
                catch
                {
                }
                MainPage.strCompanyName = strCompany;
                strCompCode = GetCompanyCode(rowIndex);
                ChangeDataBase();

                if (MainPage.strLoginName != "")
                {
                    try
                    {
                        if (MainPage.con.ConnectionString != "")
                        {
                            DataBaseAccess.SetFinancialDate();
                        }
                        if (!MainPage._bTaxStatus)
                            MainPage.mymainObject.BackgroundImage = global::SSS.Properties.Resources.BG_KSI;
                        else
                        {
                            if (MainPage.strPlanType == "DIAMOND")
                            {
                                if (MainPage.strSoftwareType == "RETAIL")
                                    MainPage.mymainObject.BackgroundImage = global::SSS.Properties.Resources.bg_d_retail;
                                else
                                    MainPage.mymainObject.BackgroundImage = global::SSS.Properties.Resources.bg_d_trading;
                            }
                            else
                                MainPage.mymainObject.BackgroundImage = global::SSS.Properties.Resources.bg_d_trading;
                        }
                    }
                    catch { }
                }              

            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Company Name in Select Company ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
    
        private void ChangeDataBase()
        {
            try
            {
                if (strCompCode != "")
                {
                    string strDataBase = "A" + strCompCode;

                    // Connection String..........................
                    MainPage.con.Close();

                    //  MainPage.con.ConnectionString z= "Data Source=" + MainPage.strDataBaseIP + ";Initial Catalog=" + strDataBase + "; User Id=" + strDataBase + ";Password=" + MainPage.strLiveDBPassword + ";";
                    if (MainPage.bDBOnNet || MainPage.strFolderName == "DEMO" || MainPage.strOldData == "DEMO" || MainPage.strProductType == "RES_RETAIL")
                        MainPage.con.ConnectionString = "Data Source=" + MainPage.strDataBaseIP + ";Initial Catalog=" + strDataBase + "; User Id=" + strDataBase + ";Password=" + MainPage.strLiveDBPassword + ";";
                    else if (MainPage.strLocalDBIP != "" && MainPage.strLocalDBPwd != "")
                        MainPage.con.ConnectionString = "Data Source=" + MainPage.strLocalDBIP + ";Initial Catalog=" + strDataBase + "; User Id=" + strDataBase + ";Password=" + MainPage.strLocalDBPwd + ";";
                    else
                        MainPage.con.ConnectionString = "Data Source=" + MainPage.strComputerName + @"\SQLEXPRESS; Initial Catalog=" + strDataBase + "; User ID=sss;password=" + MainPage.strDBPwd;


                    if (MainPage.con.State == ConnectionState.Closed)
                    {
                        MainPage.con.Open();
                    }

                    MainPage.strDataBaseFile = strDataBase;

                    this.Hide();
                    ValidateUser();
                }
            }
            catch
            {

            }
        }

        private void ValidateUser()
        {
            try
            {
                string[] strData = dba.IncreaseCounter();

                int counter = Int32.Parse(strData[0]);
                if (counter > 200 && strData[1] != "PAID")
                {
                    MessageBox.Show("Validity has been Expired ! Please Purchase Lincense version of this Software", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mainObj.Close();
                }
                else
                {
                    try
                    {
                        UserLogin ul = new UserLogin();
                        ul.ShowDialog();
                        this.Close();
                    }
                    catch { }

                    MainPage.strUpdateQuery =MainPage.strVersionUpdateQuery= "";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                mainObj.Close();
            }
        }

        private void dgrdCompany_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            GetCompanyName();
        }

       

    }
}
