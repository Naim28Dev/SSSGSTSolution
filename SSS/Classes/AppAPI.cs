using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using RestSharp;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace SSS
{
    class AppAPI
    {
        //public bool UpdateMobileNo(string strAccountID, string strMobileNo,string strOldMobileNo)
        //{
        //    try
        //    {              
              
        //        string strUrl = "http://app.ssspltd.com/WebApi/User/UpdateMobile?MU_PartyCode=" + strAccountID + "&MU_MobileNO=" + strMobileNo+"&oldMU_MobileNO=" + strOldMobileNo;
        //        string strResult = apicall(strUrl);
        //        if (strResult.Contains("\"status\":\"true\""))
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch { }
        //    return false;
        //}

        //public bool AddNewUser(string strAccountID,string strName, string strMobileNo,string strEmailID)
        //{
        //    try
        //    {
        //        string strUrl = "http://sss.ashishsrivastava.info/WebApi/user/CreateUser?MU_NAME=" + strName + "&MU_EmailID=" + strEmailID + "&MU_MobileNO=" + strMobileNo + "&MU_PersonalEmail=" + strEmailID+"&MUID=1&MU_PartyCode="+strAccountID;
        //        string strResult = apicall(strUrl);
        //        if (strResult.Contains("\"status\":\"true\""))
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch { }
        //    return false;
        //}

        public string apicall(string url)
        {  
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                    var response = webClient.DownloadString(url);
                    return response;
                }
            }
            catch(Exception ex) {
                //System.Windows.Forms.MessageBox.Show(ex.Message);                
            }
            return "";
        }

        public static bool AddNewUserinApp(string strName, string strEmailID, string strMobileNo,string strUserType, string strAccountID,string strUserRole)
        {
            try
            {
                if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                {
                    var request = (HttpWebRequest)WebRequest.Create("http://app.ssspltd.com/apipltd/Add_Modify_Users");

                     UserDetailApk objUserDetailApk = new UserDetailApk();
                    objUserDetailApk.Name = strName;
                    objUserDetailApk.Email = strEmailID;
                    objUserDetailApk.UserMobileNo =strMobileNo;
                    objUserDetailApk.PersonalEmail =strEmailID;
                    objUserDetailApk.UserName = strMobileNo;
                    objUserDetailApk.Password = "sss@" + strAccountID;
                    objUserDetailApk.userType = strUserType;
                    objUserDetailApk.PartyCode= strAccountID;
                    objUserDetailApk.UserRole =strUserRole;

                    string JSONResult = new JavaScriptSerializer().Serialize(objUserDetailApk);

                    var data = Encoding.ASCII.GetBytes(JSONResult);

                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    if (responseString.ToUpper().Contains("SUCCESS"))
                        return true;
                    else
                        return false;
                }
            }
            catch { }
            return false;
        }

        public static bool UpdateMobileNoInApp(string strAccountNo,string strOldMobileNo,string strNewMobileNo)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://app.ssspltd.com/apipltd/Add_Modify_Users");

                UserDetailApk objUserDetailApk = new UserDetailApk();
                objUserDetailApk.PartyCode = Uri.EscapeDataString(strAccountNo);
                objUserDetailApk.OldMobileNo = Uri.EscapeDataString(strOldMobileNo);
                objUserDetailApk.UserMobileNo = Uri.EscapeDataString(strNewMobileNo);

                string JSONResult = new JavaScriptSerializer().Serialize(objUserDetailApk);

                //var postData = "PartyCode=" + Uri.EscapeDataString(strAccountNo);
                //postData += "&OldMobileNo=" + Uri.EscapeDataString(strOldMobileNo);
                //postData += "&UserMobileNo=" + Uri.EscapeDataString(strNewMobileNo);
                var data = Encoding.ASCII.GetBytes(JSONResult);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }


                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
               
                if (responseString.ToUpper().Contains("SUCCESS"))
                    return true;
                else
                    return false;
            }
            catch { }
            return false;
        }

        public static bool NotificationInApp(string strAccountNo, string strTitle, string strMessage)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://148.66.132.75/SSS_API/SSSApi/User/SendNotification");

                var postData = "Partycode=" + strAccountNo;
                postData += "&title=" + strTitle;
                postData += "&msg=" + strMessage;

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (responseString.Contains("succes"))
                    return true;
                else
                    return false;
            }
            catch { }
            return false;
        }

        //public static bool AddNewUserinSSSAddaApp(string strName, string strEmailID, string strMobileNo, string strUserType, string strAccountID, string strUserRole,string strFirmName,string strCity,string strState)
        //{
        //    try
        //    {
        //        if (strUserRole == "CUSTOMER")
        //            strUserRole = "Buyer";
        //        else
        //            strUserRole = "Seller";

        //        if (MainPage.strCompanyName.Contains("SARAOGI"))
        //        {
        //            var request = (HttpWebRequest)WebRequest.Create("http://68.183.90.117/api/SSSADDA_registration");

        //            var postData = "key=" + Uri.EscapeDataString("A123456789");
        //            postData += "&name=" + Uri.EscapeDataString(strName);
        //            postData += "&email=" + Uri.EscapeDataString(strEmailID);
        //            postData += "&phone=" + Uri.EscapeDataString(strMobileNo);
        //            postData += "&firm_name=" + Uri.EscapeDataString(strEmailID);
        //            postData += "&city_name=" + Uri.EscapeDataString(strCity);
        //            postData += "&state_name=" + Uri.EscapeDataString(strState);
        //            postData += "&type=" + Uri.EscapeDataString(strUserRole);
        //            postData += "&currency_id=" + Uri.EscapeDataString("356");

        //            var data = Encoding.ASCII.GetBytes(postData);

        //            request.Method = "POST";
        //            request.ContentType = "application/x-www-form-urlencoded";
        //            request.ContentLength = data.Length;
        //            using (var stream = request.GetRequestStream())
        //            {
        //                stream.Write(data, 0, data.Length);
        //            }

        //            var response = (HttpWebResponse)request.GetResponse();
        //            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //            if (responseString.ToUpper().Contains("SUCCES"))
        //                return true;
        //            else
        //                return false;
        //        }
        //    }
        //    catch { }
        //    return false;
        //}

        public static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }      


        public static string GetSSSAddaID(string strUserRole, string stroldMobileNo)
        {
            string strID = "";
            try {
                if (strUserRole == "CUSTOMER")
                    strUserRole = "Buyer";
                else
                    strUserRole = "Seller";
             

                var restClient = new RestClient("http://68.183.90.117/api/edit_request");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string data = "{\"key\":\"A123456789\",\"type\":\"" + strUserRole + "\",\"phone\":\"" + stroldMobileNo + "\"}";

                request.AddParameter("application/json", data, ParameterType.RequestBody);
                IRestResponse result = restClient.Execute(request);
                string strJSON = result.Content;

                DataTable dtValue = JsonStringToDataTable(strJSON);
                if (dtValue.Rows.Count > 0)
                    strID = Convert.ToString(dtValue.Rows[0]["id"]);
            }
            catch { }
            return strID;
        }

        public static string AddNewUserinSSSAddaApp(string strName, string strEmailID, string strMobileNo, string strUserType, string strAccountID, string strUserRole, string strFirmName, string strCity, string strState, string strGSTNo, string strID)
        {
            try
            {

                if (strUserRole == "CUSTOMER")
                    strUserRole = "Buyer";
                else
                    strUserRole = "Seller";

                if (strID != "")
                    strID = ",\"id\":\"" + strID + "\"";

                var restClient = new RestClient("http://68.183.90.117/api/SSSADDA_registration");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string data = "{\"key\":\"A123456789\",\"from_SSSADDA\":\"1\",\"name\":\"" + strName + "\",\"email\":\"" + strEmailID + "\",\"phone\":\"" + strMobileNo + "\",\"firm_name\":\"" + strFirmName + "\","
                            + "\"city_name\":\"" + strCity + "\",\"state_name\":\"" + strState + "\",\"type\":\"" + strUserRole + "\",\"currency_id\":\"356\"" + strID + "}";

                request.AddParameter("application/json", data, ParameterType.RequestBody);
                IRestResponse result = restClient.Execute(request);
                string strResult = result.Content;
                if (strResult.ToUpper().Contains("SUCCES"))
                {
                    DataTable _dtValue = JsonStringToDataTable(strResult);
                    if (_dtValue.Rows.Count > 0 && _dtValue.Columns.Contains(" Message"))
                        return Convert.ToString(_dtValue.Rows[0][" Message"]); 
                }
            }
            catch (Exception ex) { return ex.Message; }
            return "";
        }

    }

    public class UserDetailApk
    {
        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string MobileN0 { get; set; } = "";

        public string PersonalEmail { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string userType { get; set; } = "";

        public string PartyCode { get; set; } = "";

        public string UserRole { get; set; } = "";

        public string OldMobileNo { get; set; } = "";

        public string UserMobileNo { get; set; } = "";

    }
}
