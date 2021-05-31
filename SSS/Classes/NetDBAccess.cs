using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SSS
{
    class NetDBAccess
    {
        static SqlConnection netcon;
        private static void AsignConnectionString()
        {
            if (MainPage.strOnlineDataBaseName != "" && MainPage.strLiveDataBaseIP != "")
            {
                string strUserName = Convert.ToString(DBConnection.DBCon.LiveDBUserName);
                if (strUserName == "")
                    strUserName = MainPage.strOnlineDataBaseName;
                netcon = new SqlConnection(@"Data Source=" + MainPage.strLiveDataBaseIP + ";Initial Catalog=" + MainPage.strOnlineDataBaseName + "; User Id=" + strUserName + ";Password=" + MainPage.strLiveDBPassword + ";");

            }
            else
                netcon = new SqlConnection();
        }

        private static bool OpenConnection()
        {
            try
            {
                AsignConnectionString();
                if (MainPage.strOnlineDataBaseName != "")
                {
                    if (netcon.State == ConnectionState.Closed)
                        netcon.Open();
                    return true;
                }
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("Please connect the Internet connection !! ", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return false;
        }

        public static object ExecuteMyScalar(string strQuery)
        {
            object objValue = "";
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    if (OpenConnection())
                    {
                        strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                        SqlCommand cmd = new SqlCommand(strQuery, netcon);
                        objValue = cmd.ExecuteScalar();
                    }
                    else
                        objValue = -1;
                }
            }
            catch(Exception ex)
            {
                objValue = -1;
                throw ex;
               // System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return objValue;
        }

        public static int ExecuteMyNonQuery(string strQuery)
        {
            int count = 0;
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    if (OpenConnection())
                    {
                        if (ConnectionState.Open == netcon.State)
                        {
                            strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                            SqlCommand cmd = new SqlCommand(strQuery, netcon);
                            count = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {
            }
            return count;
        }

        public static int ExecuteMyNonQueryWithTransaction(string strQuery)
        {
            int count = 0;
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    if (OpenConnection())
                    {
                        SqlTransaction transaction = netcon.BeginTransaction();
                        try
                        {
                            if (ConnectionState.Open == netcon.State)
                            {
                                strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                                SqlCommand cmd = new SqlCommand(strQuery, netcon, transaction);
                                count = cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            System.Windows.Forms.MessageBox.Show("Sorry ! "+ex.Message,"Warning",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch { }
            return count;
        }

        public static DataTable GetDataTableRecord(string strQuery)
        {           
            DataTable dt = new DataTable();
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    AsignConnectionString();
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netcon);
                    adap.Fill(dt);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataSet GetDataSetRecord(string strQuery)
        {           
            DataSet ds = new DataSet();
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    AsignConnectionString();
                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                    SqlDataAdapter adap = new SqlDataAdapter(strQuery, netcon);
                    adap.Fill(ds);
                }
            }
            catch
            {
            }
            return ds;
        }

        public static double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToDouble(objValue);
            }
            catch
            {
            }
            return dValue;
        }

        public static double CheckAmountLimitValidationFromNet(string strPartyName, string strSQuery)
        {
            double dAmt = 0;
            if (MainPage.IsConnectedToInternet())
            {

                string[] strParty = strPartyName.Split(' ');
                if (strParty.Length > 1)
                {
                    string strQuery = " Select (AmtLimit-BalanceAmt " + strSQuery + ") Amt,SSSName from (Select SSSName,MAX(AmtLimit) AmtLimit,SUM(BalanceAmt) BalanceAmt from ( "
                                    + " Select SM.Other as SSSName,(AmountLimit+CAST(ExtendedAmt as Money)) AmtLimit, "
                                    + " (Select SUM(Amt) from (Select ISNULL(SUM(CAST(Amount as Money)* (CASE WHEN BA.Status='DEBIT' then 1 else -1 end)),0)Amt from BalanceAmount BA Where BA.AccountID=(SM.AreaCode+SM.AccountNo) and (CASE WHEN (BA.Description Not  Like('%CHQ%') AND BA.Description Not Like('%CHEQUE%')) then 1 else BA.ChequeStatus end) =1) Balance) BalanceAmt"
                                    + " from SupplierMaster SM Where SM.Other in (Select SM1.Other from SupplierMaster SM1 Where (SM1.AreaCode+CAST(SM1.AccountNo as varchar))='"+strParty[0]+ "')) Bal  Group by SSSName) Bal ";

                    DataTable _dt = GetDataTableRecord(strQuery);
                    if (_dt.Rows.Count > 0)
                    {
                        dAmt = ConvertObjectToDouble(_dt.Rows[0]["Amt"]);
                        string strOldPartyName = Convert.ToString(_dt.Rows[0]["SSSName"]);
                        if (strOldPartyName != "" && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            strQuery = " IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BalanceAmount]') AND type in (N'U')) BEGIN "
                                     + " Select SUM(Amt) from (Select ISNULL(SUM(CAST(Amount as Money)* (CASE WHEN BA.Status='DEBIT' then 1 else -1 end)),0)Amt from BalanceAmount BA Where PartyName='" + strOldPartyName + "') Balance  end else begin "
                                     + " Select SUM(Amt) from (Select ISNULL(SUM(CAST(Amount as Money)* (CASE WHEN BA.Status='DEBIT' then 1 else -1 end)),0)Amt from _BalanceAmount BA Where PartyName='" + strOldPartyName + "') Balance end ";

                            SearchDataOnOld _mObj = new SearchDataOnOld();
                            object objValue= _mObj.GetValueFromMDB(strQuery);
                            dAmt -= ConvertObjectToDouble(objValue);
                        }
                    }
                }
            }
            return dAmt;
        }

        public static double GetPartyAmountFromQueryFromNet(string strAccountID)
        {
            double dAmt = 0;
            if (strAccountID != "")
            {
                string strQuery = " Select SUM(Amt) Amt From(Select ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount Where Status='Debit' and AccountID in ('" + strAccountID + "') Union All "
                                + " Select -ISNULL(SUM(CAST(Amount as Money)),0) Amt  from BalanceAmount Where Status='Credit' and AccountID in ('" + strAccountID + "'))Balance ";
                object objValue = ExecuteMyScalar(strQuery);
                if (objValue != null)
                {
                    dAmt = DataBaseAccess.ConvertObjectToDoubleStatic(objValue);
                }

                dAmt += DataBaseAccess.GetPartyAmountFromQueryNotSendToNet(strAccountID);
            }
            return dAmt;
        }

        public static int ExecuteMyNonQueryWithWrite(string strQuery)
        {
            int count = 0;
            try
            {
                if (MainPage.IsConnectedToInternet())
                {
                    if (OpenConnection())
                    {
                        if (ConnectionState.Open == netcon.State)
                        {
                            SqlCommand cmd = new SqlCommand(strQuery, netcon);
                            count = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {
            }
            if (count < 1)
                DataBaseAccess.CreateDeleteQuery_Net(strQuery);
            return count;
        }
    }
}
