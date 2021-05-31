using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DBConnection;

namespace SSS
{
    public partial class SearchDataOnOld : Form
    {
        public string strSearchData = "", strSelectedData = "";
        string strKPwd = DBCon.K_DBUserPassword, strMPwd = DBCon.M_DBUserPassword;
        string strGroupName="";
        DataTable table = null;
        public bool boxStatus = false,_bPreviousDBStatus=false;
        public ListBox objListBox;

        public SearchDataOnOld()
        {
            InitializeComponent();            
        }

        public SearchDataOnOld(bool _bStatus)
        {
            InitializeComponent();
            _bPreviousDBStatus = _bStatus;
        }
        public SearchDataOnOld(string strData,string strGName, string strHeader, Keys objKey,bool _bStatus)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            strGroupName = strGName;
            _bPreviousDBStatus = _bStatus;
            SetKeyInTextBox(objKey);
            GetDataAndBind();
            SearchRecord();
        }
       

        private static string GetLocalLastDBName()
        {
            string strDB = "";
            try
            {
                string strPath = MainPage.strOldServerPath + @"\Data";
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (!folder.Exists)
                    strPath = @"\\192.168.0.20\SSS\Data\NET";
                if (!folder.Exists)
                    strPath = @"\\SERVER\SSS\Data";

                //if (folder.Exists)
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
                        strDB = "A" + FolderName[FolderName.Count-1];
                    }                   
                }
            }
            catch
            {
            }
            return strDB;
        }

        private static string GetNetLastDBName()
        {
            string strDB = "";
            try
            {
                string strPath = MainPage.strOldServerPath + @"\NET\Data";
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (!folder.Exists)
                    strPath = @"\\192.168.0.20\SSS\NET\Data";
                if (!folder.Exists)
                    strPath = @"\\SERVER\SSS\NET\Data";             

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
                        strDB = "A" + FolderName[FolderName.Count - 1];
                    }
                }
                else
                {
                    DateTime _date =  DateTime.ParseExact("01/04/2021", "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
                    if (MainPage.endFinDate < _date)
                        strDB = "A181";
                    else 
                        strDB = "A182";
                }
            }
            catch
            {
            }
            return strDB;
        }

       
        private static string GetPreviousYearPath()
        {
            string strDB = DataBaseAccess.GetPreviousYearDataBase();
            return strDB;
        }

        private void SetKeyInTextBox(Keys objKey)
        {
            try
            {
                if (Keys.Space != objKey && objKey != Keys.F2)
                {
                    string strKey = objKey.ToString();
                    if (strKey.Contains("NumPad"))
                        strKey = strKey.Replace("NumPad", "");
                    if (strKey.Length == 2)
                        strKey = strKey.Replace("D", "");
                    txtSearch.Text = strKey;
                    txtSearch.SelectionStart = 1;
                }
            }
            catch
            {
            }
        }

        private void GetDataAndBind()
        {
            try
            {
                lbSearchBox.Items.Clear();
                
                if (strSearchData == "ALLPARTY")
                {
                    string _sQuery = "";
                    if (strGroupName != "")
                        _sQuery = " and GroupName='" + strGroupName + "' ";
                    table = GetDataTableFromMDB(" Select Name as ALLPARTY from SupplierMaster where GroupName in ('SUNDRY DEBTORS','SUNDRY CREDITOR','SALES PARTY','PURCHASE PARTY') " + _sQuery + " order by Name");
                }
                else if (strSearchData == "SALEBILLNOFORRETURN")
                {
                    table = GetDataTable(" Select Distinct (BillCode+'|'+CAST(BillNo as varchar)+'|'+Convert(nvarchar,BillDate,103)) as SALEBILLNOFORRETURN,BillNo from SalesRecord " + strGroupName + " Order by BillNo ");
                }
                else if (strSearchData == "PURCHASEBILLNOFORMPURCHASE")
                {
                    table = GetDataTable("Select Distinct (ReceiptCode+'|'+(CAST(ReceiptNo as varchar))+'|'+ISNULL(CONVERT(varchar,ReceivingDate,103),'')) as PURCHASEBILLNOFORMPURCHASE,ReceiptNo from GoodsReceive " + strGroupName + " and SaleBill='PENDING'  Order by ReceiptNo ");
                }
                else if (strSearchData == "SALEBILLNOFORRETURNRETAIL")
                {
                    table = GetDataTable(" Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURNRETAIL,BillNo from SalesBook " + strGroupName + " Order by BillNo ");
                }
                //else if (strSearchData == "SALEBILLNOFORRETURN")
                //{
                //    table = GetDataTable("Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,BillDate,103)) as SALEBILLNOFORRETURN,BillNo from SalesRecord " + strGroupName + " Order by BillNo ");
                //}
                else if (strSearchData == "PURCHASEBILLDETAIL")
                {
                    string strSubQuery = "";
                    if (strGroupName != "")
                        strSubQuery = strGroupName;

                    table = GetDataTable(" Select GR.ItemName+'|'+((PurchasePartyID+' '+SM.Name)+'|'+(BillCode+' '+CAST(BillNo as varchar))+'|'+PR.DiscountStatus+'|'+PR.Discount+'|'+PR.Dhara+'|'+SM.Category+'|'+GR.DesignName) PURCHASEBILLDETAIL from PurchaseRecord PR Left join GoodsReceiveDetails GR on PR.GRSNO=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) Outer Apply (Select TOP 1 Name,Category from SupplierMaster SM Where (SM.AreaCode+ CAST(SM.AccountNo as nvarchar))=PR.PurchasePartyID)SM   Where PR.BillNo!=0 " + strSubQuery + " Order by PR.BillNo ");
                }

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        lbSearchBox.Items.Add(row[0]);
                    }
                }
                
            }
            catch
            {
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchRecord();
        }

        private void SearchRecord()
        {
            try
            {
                lbSearchBox.Items.Clear();
                if (table != null)
                {
                    if (txtSearch.Text == "")
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            lbSearchBox.Items.Add(row[0]);
                        }
                    }
                    else if (table.Rows.Count > 0)
                    {
                        DataRow[] rows = table.Select(String.Format(strSearchData + " Like('%" + txtSearch.Text + "%') "));
                        if (rows.Length > 0)
                        {
                            foreach (DataRow row in rows)
                            {
                                lbSearchBox.Items.Add(row[0]);
                            }
                        }
                    }                  
                }
                else
                {
                    GetDataAndBind();
                }
            }
            catch
            {
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);

                    if (strSelectedData != "" || boxStatus)
                        this.Close();
                }              
                else if (e.KeyCode == Keys.Up)
                {
                    int index = lbSearchBox.SelectedIndex;
                    if (index > 0)
                    {
                        lbSearchBox.SelectedIndex = index - 1;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int index = lbSearchBox.SelectedIndex;
                    if (index < lbSearchBox.Items.Count - 1)
                    {
                        lbSearchBox.SelectedIndex = index + 1;
                    }
                }
            }
            catch
            {
            }
        }

        private void lbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                    if (strSelectedData == "")
                    {
                        if (boxStatus)
                            objListBox = lbSearchBox;
                        else
                            strSelectedData = txtSearch.Text;
                    }

                    if (strSearchData != "" || boxStatus)
                        this.Close();
                }
            }
            catch
            {
            }
        }

        private void lbSearchBox_Click(object sender, EventArgs e)
        {
            try
            {
                strSelectedData = Convert.ToString(lbSearchBox.SelectedItem);
                if (strSelectedData != "")
                {
                    this.Close();
                }
            }
            catch
            {
            }
        }

        private void SearchData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                strSelectedData = "";
                this.Close();
            }

        }

        private void SearchData_FormClosing(object sender, FormClosingEventArgs e)
        {
            //try
            //{              
            //    if (strSelectedData == "")
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //catch
            //{
            //}
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSearch.Text.Length == 0)
            {
                if (Char.IsWhiteSpace(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        public DataTable GetDataTable(string strQuery)
        {
            string strDBName = "";
            if (_bPreviousDBStatus)
                strDBName = GetPreviousYearPath();
            else
                strDBName=GetLocalLastDBName();

            DataTable table = new DataTable();
            if (strDBName != "")
            {
                if (_bPreviousDBStatus)
                {
                    string strConnection = MainPage.con.ConnectionString;

                    SqlConnection netCon = new SqlConnection(strConnection + ";password=" + MainPage.strDBPwd + ";");
                    if (netCon.State == ConnectionState.Closed)
                        netCon.Open();
                    netCon.ChangeDatabase(strDBName);

                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                    adap.Fill(table);
                }
                else
                {
                    // strDBP = DataBaseAccess.Decrypt(strPwd, "sss");
                    //string str_OLDIP = DataBaseAccess.Decrypt(str_OLDDBIP, "sss");
                    string strDBUser = "";
                    if (DBCon.M_LiveDBUserName != "")
                        strDBUser = DBCon.M_LiveDBUserName;
                    else
                        strDBUser = strDBName;

                    SqlConnection netCon = new SqlConnection(@"Data Source=" + DBCon.K_DBIP + "; Initial Catalog=" + strDBName + "; User ID=" + strDBUser + ";password=" + strKPwd + ";");
                    if (netCon.State == ConnectionState.Closed)
                    {
                        netCon.Open();
                    }
                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                    adap.Fill(table);
                }
            }
            return table;
        }

        public static DataTable GetDataTable_M(string strQuery)
        {
            DataTable _dt = new DataTable();
            try
            {
                string strDBName="A182", strDBUser = "";
                if (DBCon.M_LiveDBUserName != "")
                    strDBUser = DBCon.M_LiveDBUserName;
                else
                    strDBUser = strDBName;
                string strCon = @"Data Source=" + DBCon.M_DBIP+"; Initial Catalog="+strDBName+ "; User Id=" + strDBUser + "; Password="+ DBCon.M_DBUserPassword + "; Connection Timeout=10000;";

                SqlConnection netCon = new SqlConnection(strCon);
                if (netCon.State == ConnectionState.Closed)
                    netCon.Open();
                SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                adap.Fill(_dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dt;
        }

        public object GetValueFromMDB(string strQuery)
        {
            object objValue = 0;
            try
            {
                string strDBName = "", strDBUser = "";
                strDBName = GetNetLastDBName();
                DataTable table = new DataTable();
                if (strDBName != "")
                {
                    if (DBCon.M_LiveDBUserName != "")
                        strDBUser = DBCon.M_LiveDBUserName;
                    else
                        strDBUser = strDBName;
                    SqlConnection _netCon = new SqlConnection(@"Data Source=" + DBCon.M_DBIP + "; Initial Catalog=" + strDBName + "; User ID=" + strDBUser + ";password=" + strMPwd + ";");

                    if (_netCon.State == ConnectionState.Closed)
                        _netCon.Open();

                    SqlCommand cmd = new SqlCommand(strQuery, _netCon);
                    objValue = cmd.ExecuteScalar();
                }
            }
            catch
            {
                throw;
            }
            return objValue;
        }

        public DataTable GetDataTableFromMDB(string strQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                string strDBName = "",strDBUser="";
                strDBName = GetNetLastDBName();
                DataTable table = new DataTable();
                if (strDBName != "")
                {
                    if (DBCon.M_LiveDBUserName != "")
                        strDBUser = DBCon.M_LiveDBUserName;
                    else
                        strDBUser = strDBName;
                    SqlConnection _netCon = new SqlConnection(@"Data Source=" + DBCon.M_DBIP + "; Initial Catalog=" + strDBName + "; User ID=" + strDBUser + ";password=" + strMPwd + ";");

                    if (_netCon.State == ConnectionState.Closed)
                        _netCon.Open();

                    SqlDataAdapter _adap = new SqlDataAdapter(strQuery, _netCon);
                    _adap.Fill(dt);
                }
            }
            catch
            {
                //throw;
            }
            return dt;
        }
    }
}
