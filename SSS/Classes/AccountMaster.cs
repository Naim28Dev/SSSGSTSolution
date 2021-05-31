using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SSS
{
    class AccountMaster
    {          

        #region Get Data For Sales Report
        public DataTable GetColumnSetting()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from FormatSetting Order by Place asc", MainPage.con);
            adap.Fill(dt);
            return dt;
        }


        public int UpdateReportSetting(string[] record)
        {
            if (MainPage.con.State == ConnectionState.Closed)
            {
                MainPage.OpenConnection();
            }
            SqlCommand cmd = new SqlCommand("Update FormatSetting set Place='" + record[3] + "' where ColumnNo='" + record[1] + "'", MainPage.con);
            int count = cmd.ExecuteNonQuery();
            MainPage.CloseConnection();
            return count;
        }           

        #endregion

        #region Get Data For Purchase Report

        public DataTable GetPurchaseColumnSetting()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adap = new SqlDataAdapter("Select * from PurchaseFormatSetting Order by Place asc", MainPage.con);
            adap.Fill(dt);
            return dt;
        }      
      

        public int UpdateOrderReportSetting(string[] record)
        {
            if (MainPage.con.State == ConnectionState.Closed)
            {
                MainPage.OpenConnection();
            }
            SqlCommand cmd = new SqlCommand("Update OrderFormatSetting set Place='" + record[3] + "' where ColumnNo='" + record[1] + "'", MainPage.con);
            int count = cmd.ExecuteNonQuery();
            MainPage.CloseConnection();
            return count;
        }

        public int UpdateOrderColumnReportSetting(string[] record)
        {
            if (MainPage.con.State == ConnectionState.Closed)
            {
                MainPage.OpenConnection();
            }
            SqlCommand cmd = new SqlCommand("Update OrderColumnSetting set Place='" + record[3] + "' where ColumnNo='" + record[1] + "'", MainPage.con);
            int count = cmd.ExecuteNonQuery();
            MainPage.CloseConnection();
            return count;
        }
        
        #endregion
        
       
    }
}
