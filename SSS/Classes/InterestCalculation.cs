using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace SSS
{
    class InterestCalculation
    {     

        public DataTable GetDataTable(string strQuery)
        {
            MainPage.OpenConnection();
            DataTable dt = new DataTable();
            strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
            SqlDataAdapter adap = new SqlDataAdapter(strQuery, MainPage.con);
            adap.SelectCommand.CommandTimeout= 100000;
            adap.Fill(dt);
            MainPage.CloseConnection();
            return dt;
        }

        public DataSet GetDataSet(string strQuery)
        {
            MainPage.OpenConnection();
            DataSet ds = new DataSet();
            strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
            SqlDataAdapter adap = new SqlDataAdapter(strQuery, MainPage.con);
            adap.SelectCommand.CommandTimeout = 1000000;
            adap.Fill(ds);
            MainPage.CloseConnection();
            return ds;
        }

        public DataTable GetMultiQuarterDataTable(string strQuery,string strDataBase)
        {
            DataTable dt = new DataTable();
            if (strDataBase != "")
            {
                MainPage.ChangeDataBase(strDataBase);
                if (MainPage.con.Database == strDataBase)
                {
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;

                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, MainPage.con);
                    adap.SelectCommand.CommandTimeout = 100000;
                    adap.Fill(dt);
                }
            }
            return dt;
        }

        public DataSet GetMultiQuarterDataSet(string strQuery, string strDataBase)
        {
            DataSet ds = new DataSet();
            if (strDataBase != "")
            {
                MainPage.ChangeDataBase(strDataBase);
                if (MainPage.con.Database == strDataBase)
                {
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;

                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, MainPage.con);
                    adap.SelectCommand.CommandTimeout = 100000;
                    adap.Fill(ds);
                }
            }
            return ds;
        }

        public string GetGroupName(string strParty)
        {           
            if (MainPage.con.State == ConnectionState.Closed)
            {
                MainPage.OpenConnection();
            }           
            SqlCommand cmd=new SqlCommand("Select UPPER(GroupName) from SupplierMaster where Name='" + strParty + "' and GroupName!='Sub Party'", MainPage.con);
            object objValue = cmd.ExecuteScalar();
            MainPage.CloseConnection();
            return Convert.ToString(objValue);
        }

        public int ExecuteMyQuery(string strQuery)
        {
            int count = 0;
            MainPage.OpenConnection();
            SqlTransaction transaction = MainPage.con.BeginTransaction();
            try
            {
                strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;

                SqlCommand cmd = new SqlCommand(strQuery, MainPage.con, transaction);
                cmd.CommandTimeout = 1000000;
                count = cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch(Exception ex)
            {              
                transaction.Rollback();
                string[] strReport = { "Exception occurred in Excuting Query ", ex.Message };
                CreateErrorReports(strReport);
            }
            finally
            {
                MainPage.CloseConnection();
            }

            return count;
        }

        public int GetRecordExistance(string strQuery)
        {
            int count = 0;
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter(strQuery, MainPage.con);
            adap.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0][0]) != "")
                    count = Convert.ToInt32(dt.Rows[0][0]);
            }
            MainPage.CloseConnection();
            return count;
        }

        #region Reporting

        public void CreateErrorReports(string[] strReport)
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Reporting";

                Directory.CreateDirectory(strPath);
                StreamWriter sw = new StreamWriter(strPath + "\\Reporting.doc", true);
                sw.Write(sw.NewLine);
                sw.WriteLine(strReport[0] + "   " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString() + "  Computer Name : " + Environment.MachineName + "     User Name :  " + MainPage.strLoginName);
                sw.WriteLine(strReport[1]);
                sw.Close();
            }
            catch { }
        }

        public static void CreateErrorReport(string[] strReport)
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Reporting";

                Directory.CreateDirectory(strPath);
                StreamWriter sw = new StreamWriter(strPath + "\\Reporting.doc", true);
                sw.Write(sw.NewLine);
                sw.WriteLine(strReport[0] + "   " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString() + "  Computer Name : " + Environment.MachineName + "     User Name :  " + MainPage.strLoginName);
                sw.WriteLine(strReport[1]);
                sw.Close();
            }
            catch { }
        }

        #endregion
    }


}

