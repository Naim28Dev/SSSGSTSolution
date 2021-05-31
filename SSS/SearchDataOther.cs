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

namespace SSS
{
    public partial class SearchDataOther : Form
    {
        public string strSearchData = "", strSelectedData = "",str_OLDDBIP= "xjkEiGxFiEpK8OHTWpWt5pW6hm9aCG4ULAkDeBIC7DVc5N+3C5l+iewb/LaEvprW2T7xZqqeea0olBuqU/dyAUhXTdGeS+me34LTQV9KklQ23LvG8s0O1rMuDHo9p8R3", strDBP="",strMDBIP= "Wv6kK2mT8fOt8uByCk7cdoSqAGhO1ck3elaOgsJYOn3rDa+fCSr6usw+jykDCiMuxACqxJ/2eFRvcFLx1u+1xCgjWKoo3xJZuBPjTD1UBeZHuWg4+aZKUjGu81Se+Bas";
        string strGroupName = "";
        DataTable table = null;
        public bool boxStatus = false,_bPreviousDBStatus=false;
        string strCompanyCode="",strCatNo = "1", strCatName = "", strPONumber = "", strSONumber = "", strDesignName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";

        public ListBox objListBox;

        public SearchDataOther()
        {
            InitializeComponent();            
        }

        public SearchDataOther(bool _bStatus)
        {
            InitializeComponent();
            _bPreviousDBStatus = _bStatus;
        }
        public SearchDataOther(string strData,string strGName, string strHeader, Keys objKey,bool _bStatus)
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

        public SearchDataOther(string strData, string strGName, string strHeader, Keys objKey, bool _bStatus,string strCompany)
        {
            InitializeComponent();
            lblHeader.Text = strHeader;
            strSearchData = strData;
            strGroupName = strGName;
            _bPreviousDBStatus = _bStatus;
            strCompanyCode = strCompany;

            SetKeyInTextBox(objKey);
            GetDataAndBind();
            SearchRecord();
        }

        public SearchDataOther(string strData, string strHeader, string strDesign, string strCat1, string strCat2, string strCat3, string strCat4, string strCat5, Keys objKey, bool bStatus, string strCCode)
        {
            InitializeComponent();
            strCatNo = strData;
            boxStatus = bStatus;
            strCatName = strHeader.ToUpper();
            lblHeader.Text = "SEARCH " + strHeader.ToUpper();
            if (strData == "")
                strSearchData = strHeader;
            else
                strSearchData = "Variant" + strCatNo;
            strDesignName = strDesign;
            strCategory1 = strCat1;
            strCategory2 = strCat2;
            strCategory3 = strCat3;
            strCategory4 = strCat4;
            strCategory5 = strCat5;

            strCompanyCode = strCCode;

            SetKeyInTextBox(objKey);
            GetDataAndBind();
        }

        string strPwd = "CE4Uk6SaZK3gAd/AOzfDXHKdxV+IdyIjUPw/cnBZRsyys2czfZ2kcx0ZhjCoEp4i67AxV+NymCpveYjLXWe8XY82IGDFNbG2++/J/UHb+RjXpKEeFr7Nh1RMUMfAopI3", strMPwd= "RRdDUVMg8WLZb/p+Nj3BgKpIaY6m8dvKLsmp8UdU/CXbhe6n1QJEwnlH2XEiVjWwJi1riA6DF0Z7u1d32NFXRWHQmh6zher1qVZHKW6biyQti/JopuZmVRumaLg41QlJ";

        private static string GetLocalLastDBName()
        {
            string strDB = "";
            try
            {
                string strPath = MainPage.strOldServerPath + @"\Data";
                DirectoryInfo folder = new DirectoryInfo(strPath);
              
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

        private DataTable GetCompanyDetails()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("COMPANYNAME", typeof(String));
            try
            {
                string strPath = MainPage.strOldServerPath + @"\Data";
                DirectoryInfo folder = new DirectoryInfo(strPath);
                string strFilePath = "", strFName = "";
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
                            strFName = fi.Name;
                            strFilePath = MainPage.strOldServerPath + "\\Data\\" + strFName + "\\" + strFName + ".syber";
                            if (File.Exists(strFilePath))
                            {
                                StreamReader sr = new StreamReader(strFilePath);
                                DataRow row = _dt.NewRow();
                                row["COMPANYNAME"] = sr.ReadToEnd() + "|" + "A" + strFName;
                                _dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch { }
            return _dt;
        }

        private static string GetLocalLastDBName_NC()
        {
            string strDB = "";
            try
            {
                string strPath = MainPage.strOldServerPath + @"\Data";
                DirectoryInfo folder = new DirectoryInfo(strPath);

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
                        strDB = "A" + FolderName[FolderName.Count - 1];
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
                string strPath = MainPage.strServerPath + @"\NET\Data";
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
                        strDB = "A" + FolderName[FolderName.Count - 1];
                    }
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
                string strAllVariant = "ItemName", strAllVariant_Order = "ItemName";
                if (MainPage.StrCategory1 != "")
                    strAllVariant += "+'|'+ISNULL(Variant1,'')";
                if (MainPage.StrCategory2 != "")
                    strAllVariant += "+'|'+ISNULL(Variant2,'')";
                if (MainPage.StrCategory3 != "")
                    strAllVariant += "+'|'+ISNULL(Variant3,'')";
                if (MainPage.StrCategory4 != "")
                    strAllVariant += "+'|'+ISNULL(Variant4,'')";
                if (MainPage.StrCategory5 != "")
                    strAllVariant += "+'|'+ISNULL(Variant5,'')";

                if (strSearchData == "ALLPARTY")
                {
                    string _sQuery = "";
                    if (strGroupName != "")
                        _sQuery = " and GroupName='" + strGroupName + "' ";
                    table = GetDataTable(" Select Name as ALLPARTY from SupplierMaster where GroupName in ('SUNDRY DEBTORS','SUNDRY CREDITOR') " + _sQuery + " order by Name");
                }
                else if (strSearchData == "DESIGNNAME")
                {
                    if (boxStatus)
                    {
                        if (strCompanyCode != "")
                            table = GetDataTable("Select Distinct (" + strAllVariant + ") as DESIGNNAME from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 and DSM.ActiveStatus=1 Order By DesignName", strCompanyCode);
                        else
                            table = DataBaseAccess.GetDataTableRecord("Select Distinct (" + strAllVariant + ") as DESIGNNAME from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 Order By DesignName");
                    }
                    else
                        table = GetDataTable(" Select Distinct ItemName as DESIGNNAME from Items Where SubGroupName='PURCHASE' Order by ItemName ");
                }
                else if (strSearchData == "PURCHASEBILLNO_OTHER")
                {
                    DataBaseAccess dba = new SSS.DataBaseAccess();
                    string strQuery = " Select (BillCode+' '+CAST(BillNo as varchar)+'|'+CAST(NetAmt as varchar)) PURCHASEBILLNO_OTHER from PurchaseBook Where PurchasePartyID in (Select (AreaCode+AccountNo) from SupplierMaster SM Where GroupName='SUNDRY CREDITOR' and SM.Other='" + strGroupName + "') Order by BillCode,BillNo desc ";
                    table = dba.GetDatFromAllFirm_OtherCompany(strQuery);// Where ISNULL(SpecialDscPer,0)>0 
                }
                else if (strSearchData == "PURCHASEBILLNO")
                {
                    table = DataBaseAccess.GetDataTableRecord(" Select (BillCode+' '+CAST(BillNo as varchar)+'|'+CAST(NetAmt as varchar)) PURCHASEBILLNO from PurchaseBook  Where  Description='' and PurchasePartyID='" + strGroupName + "' and (BillCode+' '+CAST(BillNo as varchar)) not in (Select PB.Description from PurchaseBook PB Where PB.Description!='')  and (BillCode+' '+CAST(BillNo as varchar)) in (Select Distinct (PBS.BillCode+' '+CAST(PBS.BillNo as varchar)) from PurchaseBookSecondary PBS Where Other2!='' and Other2!='" + MainPage.strDataBaseFile + "')  Order by BillCode,BillNo desc ");// Select (BillCode+' '+CAST(BillNo as varchar)) PURCHASEBillNo from PurchaseBook Order by BillCode,BillNo desc ");// Where ISNULL(SpecialDscPer,0)>0 
                }
                else if (strSearchData == "DESIGNNAMESINGLE")
                {
                    if (boxStatus)
                    {
                        if (strCompanyCode != "")
                            table = GetDataTable("Select Distinct ItemName as DESIGNNAMESINGLE from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 and DSM.ActiveStatus=1 Order By ItemName", strCompanyCode);
                        else
                            table = DataBaseAccess.GetDataTableRecord("Select Distinct ItemName as DESIGNNAMESINGLE from Items DM left join ItemSecondary DSM on DM.BillCode=DSM.BillCode and DM.BillNo=DSM.BillNo Where DM.DisStatus=0 Order By ItemName");
                    }
                    else
                        table = GetDataTable(" Select Distinct ItemName as DESIGNNAMESINGLE from Items Where SubGroupName='PURCHASE' Order by ItemName ");
                }
                else if (strSearchData == "_BillNo")
                {
                    string strQuery = " Select (BillCode+' '+CAST(BillNo as varchar))_BillNo from SalesBook Where ISNULL(SpecialDscPer,0)>0 Order by BillCode,BillNo desc ";
                    DataBaseAccess dba = new DataBaseAccess();
                    table = dba.GetDatFromAllFirm_OtherCompany(strQuery);
                }
                else if (strSearchData == "PURCHASERETURNBILLNO")
                {
                    table = GetDataTable(" Select (BillCode+' '+CAST(BillNo as varchar))PURCHASERETURNBILLNO from PurchaseReturn Order by BillCode,BillNo desc ");// Where ISNULL(SpecialDscPer,0)>0 
                }
                else if (strSearchData == "SALERETURNBILLNO")
                {
                    table = GetDataTable(" Select (BillCode+' '+CAST(BillNo as varchar))SALERETURNBILLNO from SaleReturn Order by BillCode,BillNo desc ");
                }
                else if (strSearchData == "SALEBILLNOFORRETURN")
                {
                    table = GetDataTable(" Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,BillDate,103)) as SALEBILLNOFORRETURN,BillNo from SalesRecord " + strGroupName + " Order by BillNo ");
                }
                else if (strSearchData == "SALEBILLNOFORRETURNRETAIL")
                {
                    table = GetDataTable(" Select Distinct (CAST(BillNo as varchar)+'|'+Convert(nvarchar,Date,103)) as SALEBILLNOFORRETURNRETAIL,BillNo from SalesBook " + strGroupName + " Order by BillNo ");
                }
                else if (strSearchData == "STOCKOUTBILLNO")
                {
                    table = NetDBAccess.GetDataTableRecord(" Select DISTINCT (BillCode+' '+CAST(BillNo as varchar))STOCKOUTBILLNO,BillNo from StockTransfer Order by BillNo ");
                }
                else if (strSearchData == "SUPPLIERGSTNO")
                {
                    string strQuery = "", strSubQuery = "";
                    if (strGroupName != "")
                        strSubQuery = " and (Other Like('" + strGroupName + " %') OR Other ='" + strGroupName + "') ";
                    strQuery = " Select Distinct (GSTNo+'|'+State)SUPPLIERGSTNO from SupplierMaster Where GroupName='SUNDRY CREDITOR' and TransactionLock=0 and BlackList=0 and GSTNo!='' " + strSubQuery + " Order by (GSTNo+'|'+State)";
                    if (strCompanyCode != "")
                        table = GetDataTable(strQuery, strCompanyCode);
                    else
                    {
                        DataBaseAccess dba = new DataBaseAccess();
                        table = dba.GetDatFromAllFirm_OtherCompany(strQuery);
                        if (table.Rows.Count > 0)
                            table = table.DefaultView.ToTable(true, strSearchData);
                    }
                }
                else if (strSearchData == "ALLACCOUNTID")
                {
                    string strQuery = "", strSubQuery = "", strOther = "+'|'+Other";
                    if (strGroupName != "")
                        strSubQuery = " and Other Like('" + strGroupName + " %') ";
                    else
                        strOther = "+'|'+Other";

                    strQuery = " Select (AreaCode+AccountNo+' '+NAME" + strOther + ")ALLACCOUNTID from SupplierMaster Where GroupName!='SUB PARTY' " + strSubQuery + " Order by (AreaCode+AccountNo+' '+NAME" + strOther + ") ";
                    if (strCompanyCode != "")
                        table = GetDataTable(strQuery, strCompanyCode);
                    else
                    {
                        DataBaseAccess dba = new DataBaseAccess();
                        table = dba.GetDatFromAllFirm_OtherCompany(strQuery);
                        table = table.DefaultView.ToTable(true, strSearchData);
                    }
                }
                else if (strSearchData == "COMPANYNAME")
                {
                    table = GetCompanyDetails();
                }
                else if (strSearchData == "PURCHASEBILLDETAIL")
                {
                    string strSubQuery = "";
                    if (strGroupName != "")
                        strSubQuery = strGroupName;

                    table = GetDataTable(" Select GR.ItemName+'|'+((PurchasePartyID+' '+SM.Name)+'|'+(BillCode+' '+CAST(BillNo as varchar))+'|'+PR.DiscountStatus+'|'+PR.Discount+'|'+PR.Dhara+'|'+SM.Category) PURCHASEBILLDETAIL from PurchaseRecord PR Left join GoodsReceiveDetails GR on PR.GRSNO=(GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) Outer Apply (Select TOP 1 Name,Category from SupplierMaster SM Where (SM.AreaCode+ CAST(SM.AccountNo as nvarchar))=PR.PurchasePartyID)SM   Where PR.BillNo!=0 " + strSubQuery + " Order by PR.BillNo ");
                }

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        lbSearchBox.Items.Add(row[0]);
                    }
                }
                if (strSearchData == "DESIGNNAME")
                    lbSearchBox.Items.Add("ADD NEW DESIGN NAME");

            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    if (strSearchData == "DESIGNNAME")
                        lbSearchBox.Items.Add("ADD NEW DESIGN NAME");

                    if (lbSearchBox.Items.Count > 0 && !boxStatus)
                        lbSearchBox.SelectedIndex = 0;
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
            if (strSelectedData == "ADD NEW DESIGN NAME")
            {
                if (MainPage.bArticlewiseOpening)
                {
                    ItemMaster objItemMaster = new ItemMaster(true);
                    objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objItemMaster.ShowInTaskbar = true;
                    objItemMaster.ShowDialog();
                    strSelectedData = objItemMaster.StrAddedDesignName;
                }
                else
                {
                    DesignMaster objDesignMaster = new DesignMaster(true);
                    objDesignMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objDesignMaster.ShowDialog();
                    strSelectedData = objDesignMaster.StrAddedDesignName;
                }
                if (strSelectedData == "")
                    e.Cancel = true;
            }
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

        public static DataTable GetDataTable(string strQuery, string strDBName)
        {
            DataTable table = new DataTable();
            if (strDBName != "")
            {

                SqlConnection netCon = SetLocalConnection(strDBName);
                if (netCon.State == ConnectionState.Closed)
                {
                    netCon.Open();
                }
                SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                adap.Fill(table);

            }
            return table;
        }

        public static int ExecuteNoQuery(string strQuery, string strDBName)
        {
            int _count = 0;
            try
            {
                if (strDBName != "")
                {

                    SqlConnection netCon = SetLocalConnection(strDBName);
                    if (netCon.State == ConnectionState.Closed)
                    {
                        netCon.Open();
                    }
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;


                    SqlCommand cmd = new SqlCommand(strQuery, netCon);
                    _count = cmd.ExecuteNonQuery();
                    if (netCon.State == ConnectionState.Open)
                        netCon.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _count;

        }

        public static int ExecuteNoQuery(string strQuery, string strDBName, ref string _strPBillNo)
        {
            int _count = 0;

            try
            {
                if (strDBName != "")
                {

                    SqlConnection netCon = SetLocalConnection(strDBName);
                    if (netCon.State == ConnectionState.Closed)
                    {
                        netCon.Open();
                    }
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;


                    SqlCommand cmd = new SqlCommand(strQuery, netCon);
                    object objValue = cmd.ExecuteScalar();
                    if (Convert.ToString(objValue) != "")
                    {
                        _count = 1;
                          _strPBillNo = Convert.ToString(objValue);
                    }
                    if (netCon.State == ConnectionState.Open)
                        netCon.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _count;
        }

        public DataTable GetDataTable(string strQuery)
        {
            string strDBName = "";
            if (strCompanyCode != "")
                strDBName = strCompanyCode;
            else
            {
                if (_bPreviousDBStatus)
                    strDBName = GetPreviousYearPath();
                else
                    strDBName = GetLocalLastDBName();
            }

            DataTable table = new DataTable();
            if (strDBName != "")
            {
                if (_bPreviousDBStatus)
                {                   
                    SqlConnection netCon = SetLocalConnection(strDBName);
                    if (netCon.State == ConnectionState.Closed)
                        netCon.Open();
                    
                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                    adap.Fill(table);
                }
                else
                {
                   
                    SqlConnection netCon = SetLocalConnection(strDBName);
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

        public static DataSet GetDataSet(string strQuery)
        {
            string strDBName = "";          
                strDBName = GetLocalLastDBName();

            DataSet ds = new DataSet();
            if (strDBName != "")
            {
                SqlConnection netCon = SetLocalConnection(strDBName);
                if (netCon.State == ConnectionState.Closed)
                {
                    netCon.Open();
                }
                SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                adap.Fill(ds);

            }
            return ds;
        }

        public static DataSet GetDataSet(string strQuery,string strDBName)
        { 
            DataSet ds = new DataSet();
            if (strDBName != "")
            {
                {
                    //strDBP = DataBaseAccess.Decrypt(strPwd, "sss");
                    //string str_OLDIP = DataBaseAccess.Decrypt(str_OLDDBIP, "sss");                                      

                    SqlConnection netCon = SetLocalConnection(strDBName);
                    if (netCon.State == ConnectionState.Closed)
                    {
                        netCon.Open();
                    }
                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                    adap.Fill(ds);
                }
            }
            return ds;
        }     

        public static DataTable GetDataTable_NC(string strQuery)
        {
            string strDBName = "";
            strDBName = GetLocalLastDBName_NC();
            DataTable dt = new DataTable();
            try
            {
                if (strDBName != "")
                {
                    SqlConnection netCon = SetLocalConnection(strDBName);                   
                    if (netCon.State == ConnectionState.Closed)
                        netCon.Open();

                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netCon);
                    adap.Fill(dt);
                }
            }
            catch { }
            return dt;
        }

        private static SqlConnection SetLocalConnection(string strDBName)
        {
            SqlConnection netCon = new SqlConnection();

            if (MainPage.strDataBaseFile == strDBName)
                netCon = MainPage.con;
            else if (MainPage.strFolderName == "DEMO" && MainPage.strSytemType == "SINGLE")
                netCon.ConnectionString = "Data Source=" + MainPage.strDataBaseIP + ";Initial Catalog=" + strDBName + "; User Id=" + strDBName + ";Password=" + MainPage.strLiveDBPassword + ";";
            else if (MainPage._localonLocal || (!MainPage._bTaxStatus && (MainPage.strFolderName != "DEMO" && MainPage.strOldData != "DEMO")))
                netCon.ConnectionString = "Data Source=" + MainPage.strComputerName + @"\SQLEXPRESS; Initial Catalog=" + strDBName + "; User ID=sss;password=" + MainPage.strDBPwd + ";";
            else
                netCon.ConnectionString = "Data Source=" + MainPage.strDataBaseIP + ";Initial Catalog=" + strDBName + "; User Id=" + strDBName + ";Password=" + MainPage.strLiveDBPassword + ";";

            return netCon;
        }

        //public object GetValueFromMDB(string strQuery)
        //{
        //    object objValue = 0;
        //    try
        //    {
        //        string strDBName = "";
        //        strDBName = GetNetLastDBName();
        //        DataTable table = new DataTable();
        //        if (strDBName != "")
        //        {
        //            string strPwd = DataBaseAccess.Decrypt(strMPwd, "sss");
        //            string str_IP = DataBaseAccess.Decrypt(strMDBIP, "sss");

        //            SqlConnection _netCon = new SqlConnection(@"Data Source=" + str_IP + "; Initial Catalog=" + strDBName + "; User ID=" + strDBName + ";password=" + strPwd + ";");
        //            if (_netCon.State == ConnectionState.Closed)
        //                _netCon.Open();

        //            SqlCommand cmd = new SqlCommand(strQuery, _netCon);
        //            objValue = cmd.ExecuteScalar();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return objValue;
        //}

    }
}
