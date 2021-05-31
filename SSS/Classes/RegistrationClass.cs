using System;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Management;

namespace SSS
{
    class RegistrationClass
    {
        SqlConnection con;
        public RegistrationClass()
        {
           
        }
        public void KeyHandlerPoint(object sender, KeyPressEventArgs e, int count)
        {
            try
            {
                TextBox txt = sender as TextBox;
                Char pressedKey = e.KeyChar;
                if (pressedKey != Convert.ToChar(8))
                {
                    if (txt.Text.Contains("."))
                    {
                        string[] strSplit = txt.Text.Split('.');
                        if (strSplit.Length > 1)
                        {
                            int index = txt.SelectionStart, pointIndex = txt.Text.IndexOf('.');

                            if (strSplit[1].Length == count && index > pointIndex)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                    else
                    {
                        if (pressedKey == Convert.ToChar(46) && count > 0)
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
                }
            }
            catch
            {
            }
        }


        protected internal int ExecuteNonQuery(string strQuery)
        {
            int _count = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCommand cmd = new SqlCommand(strQuery, con);
                _count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            return _count;
        }

        protected internal DataTable GetDataTable(string strQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlDataAdapter adap = new SqlDataAdapter(strQuery, con);
                adap.Fill(dt);
            }
            catch (Exception ex) { throw ex; }
            return dt;
        }

      

        protected internal int SaveUserInfo(string strEmailID, string strPerson, string strMobile, string strMachineID, string strDate,string strPlanType,string strSoftwareType,string strIntallationType)
        {
            string strQuery = "";
            int _count = 0;
            try
            {
                strQuery = " if not exists (Select [MachineID] from [dbo].[Registration] Where [MachineID]='" + strMachineID + "' and [EmailID]='" + strEmailID + "' and [ActivationKey]!='' ) begin "
                         + " INSERT INTO [dbo].[Registration] ([EmailID],[ContactPerson],[MobileNo],[MachineID],[ValidFrom],[ValidTo],[Category],[ActivationKey],[ActivationDate],[Status],[ApprovedBy],[SoftwareType],[InstallationType]) "
                         + " Values ('" + strEmailID + "','" + strPerson + "','" + strMobile + "','" + strMachineID + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'"+ strPlanType+"','',NULL,0,'','"+ strSoftwareType+"','"+ strIntallationType+"') end ";
                            
                _count = ExecuteNonQuery(strQuery);
            }
            catch (Exception ex) { throw ex; }
            return _count;
        }

        protected internal string CheckMachineID(string strEmailID, string strMachineID,ref string strPlanType)
        {
            string strActivationKey = "KEY";
            try
            {
                string strQuery = "";
                strQuery = " Select (Select Top 1 Category from dbo.[Registration] Where ISNULL([ActivationKey],'')!='' and [EmailID]='" + strEmailID + "')PlanType,(Select Top 1 [ActivationKey] from dbo.[Registration] Where [EmailID]='" + strEmailID + "' and [MachineID]='" + strMachineID + "')ActivationKey ";
                DataTable dt = GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    strActivationKey = Convert.ToString(dt.Rows[0]["ActivationKey"]);
                    strPlanType = Convert.ToString(dt.Rows[0]["PlanType"]);
                    if (strActivationKey == "")
                        strActivationKey = "KEY";
                }
            }
            catch (Exception ex)
            { strActivationKey = "";
                throw ex; }
            return strActivationKey;

        }

        protected internal int UpdateActivationKey(string strActivationKey, string strEmailID, string strMachineID)
        {
            string strQuery = "";
            int _count = 0;
            try
            {
                strQuery = " Update dbo.[Registration]  Set ActivationKey='"+ strActivationKey+ "',[ActivationDate]=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) Where [EmailID]='" + strEmailID+"' and [MachineID]='"+ strMachineID+"' and ActivationKey=''  ";
                               
                _count = ExecuteNonQuery(strQuery);
            }
            catch (Exception ex) { throw ex; }
            return _count;
        }

        protected internal string CheckMachineID(ref string strPlanType)
        {
            string strEmailID = "";
            try
            {
                string strQuery = "", strMachineID = GetMachineID();
                strQuery += " Select EmailID,Category from Registration Where MachineID='" + strMachineID + "' and ISNULL(ActivationKey,'')!='' ";
                DataTable _dt = GetDataTable(strQuery);
                if (_dt.Rows.Count > 0)
                {
                    strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                    strPlanType = Convert.ToString(_dt.Rows[0]["Category"]);
                }
            }
            catch (Exception ex) { throw ex; }
            return strEmailID;
        }


        public void ValidateSpace(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt != null)
                {                  

                    if (e.KeyChar == Convert.ToChar(39))
                        e.Handled = true;
                    else if (Char.IsWhiteSpace(e.KeyChar) && e.KeyChar != Convert.ToChar(13))
                    {
                        if (txt.Text.Length == 0 || txt.SelectionStart == 0)
                            e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        public static List<string> GetBoardSerialNumbers()
        {        
            List<string> results = new List<string>();

            string query = " SELECT * FROM Win32_BaseBoard ";
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(query);
            foreach (ManagementObject info in searcher.Get())
            {
                results.Add(
                    info.GetPropertyValue("SerialNumber").ToString());
            }

            return results;
        }

        //public static string UUID
        //{
        //    get
        //    {
        //        string uuid = string.Empty;

        //        ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
        //        ManagementObjectCollection moc = mc.GetInstances();

        //        foreach (ManagementObject mo in moc)
        //        {
        //            uuid = mo.Properties["UUID"].Value.ToString();
        //            break;
        //        }

        //        return uuid;
        //    }
        //}       

        public static string GetMachineID()
        {
            string uuid = string.Empty;
            try
            {
                ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    uuid = mo.Properties["UUID"].Value.ToString();
                    break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return uuid;
        }

        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";

            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementBaseObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() != "True") continue;
                //Only get the first one
                if (result != "") continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                {
                }
            }
            return result;
        }
    }
}
