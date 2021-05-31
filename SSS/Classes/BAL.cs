using System;
using System.Data;
using System.IO;
using System.Collections;

namespace SSS
{
    class BAL
    {
        private static string GetCurrentFinYear()
        {
            string strYear = "";
            string[] str = MainPage.strCompanyName.Split(' ');
            if (str.Length > 0)
                strYear = str[str.Length - 1];
            return strYear;
        }

        public static DataTable GetCurrentFinancialYearCompany()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CCode", typeof(String));
            dt.Columns.Add("DBIP", typeof(String));
            dt.Columns.Add("DBUser", typeof(String));
            dt.Columns.Add("DBPwd", typeof(String));
            string strYear = GetCurrentFinYear();
            if (strYear != "")
            {
                string strPath = MainPage.strServerPath + @"\Data";
                AllFirmDetails(ref dt,strYear, strPath);
                if (MainPage.strOldServerPath != "")
                {
                    strPath = MainPage.strOldServerPath + @"\Data";
                    AllFirmDetails(ref dt, strYear, strPath);
                }

            }
            return dt;
        }

        public static DataTable GetCurrentFinancialYear_OtherCompany()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CCode", typeof(String));
            dt.Columns.Add("DBIP", typeof(String));
            dt.Columns.Add("DBUser", typeof(String));
            dt.Columns.Add("DBPwd", typeof(String));
            string strYear = GetCurrentFinYear();
            if (strYear != "" && MainPage.strOldServerPath!="")
            {
                string strPath = MainPage.strOldServerPath + @"\Data";
                AllFirmDetails(ref dt, strYear, strPath);                

            }
            return dt;
        }

        protected internal static string GetLocalDBName()
        {
            string strDBName = "", strPath = MainPage.strOldServerPath + @"\Data";
            try
            {
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    string[] Folder;
                    ArrayList FolderName = new ArrayList();
                    Folder = Directory.GetDirectories(strPath);
                    if (Folder.Length > 0)
                    {
                        foreach (string folderName in Folder)
                        {
                            FileInfo fi = new FileInfo(folderName);
                            FolderName.Add(fi.Name);
                        }
                        FolderName.Sort();
                        strDBName = "A"+Convert.ToString(FolderName[FolderName.Count - 1]);
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Sorry ! Local path not exists.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
                else
                    System.Windows.Forms.MessageBox.Show("Sorry ! Local path not exists.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                string[] str = { "Fetch Firm Name", "SORRY ! " + ex.Message };
                DataBaseAccess.CreateErrorReport(str);
            }
            return strDBName;
        }


        private static void AllFirmDetails(ref DataTable _dt, string strYear, string strPath)
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(strPath);
                string strFilePath = "", strFName = "", strFullCompanyName = "";
                if (folder.Exists)
                {
                    string[] Folder;
                    Folder = Directory.GetDirectories(strPath);
                    if (Folder.Length > 0)
                    {
                        foreach (string folderName in Folder)
                        {
                            FileInfo fi = new FileInfo(folderName);
                            strFName = fi.Name;
                            strFilePath = strPath + "\\" + strFName + "\\" + strFName + ".syber";
                            if (File.Exists(strFilePath))
                            {
                                using (StreamReader sr = new StreamReader(strFilePath))
                                {
                                    strFullCompanyName = sr.ReadToEnd().Trim();
                                    if (MainPage.strCompanyName != strFullCompanyName && strFullCompanyName.Contains(strYear))
                                    {
                                        DataRow row = _dt.NewRow();
                                        row["CCode"] = "A" + strFName;
                                        if (MainPage._bTaxStatus || (MainPage.strLocalDBIP == "" && MainPage.strLocalDBPwd == ""))
                                        {
                                            if (MainPage.strFolderName != "DEMO" && MainPage.strOldData != "DEMO")
                                            {
                                                row["DBIP"] = MainPage.strComputerName + @"\SQLEXPRESS";
                                                row["DBPwd"] = MainPage.strDBPwd;
                                                row["DBUser"] = "sss";
                                            }
                                            else
                                            {
                                                row["DBIP"] = MainPage.strDataBaseIP;
                                                row["DBPwd"] = MainPage.strLiveDBPassword;
                                                row["DBUser"] = row["CCode"];
                                            }
                                        }
                                        else
                                        {
                                            row["DBIP"] = MainPage.strLocalDBIP;
                                            row["DBPwd"] = MainPage.strLocalDBPwd;
                                            row["DBUser"] = row["CCode"];
                                        }
                                        _dt.Rows.Add(row);
                                    }
                                }
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Sorry ! Local path not exists.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { string[] str= { "Fetch Firm Name","SORRY ! "+ex.Message};
                DataBaseAccess.CreateErrorReport(str); }
        }
    }
}
