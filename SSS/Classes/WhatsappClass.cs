using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
namespace SSS
{
    class WhatsappClass
    {

        public static string SendWhatsAppMessage_NEW(string strMobileNo, string strMessage, string strFilePath, string strBillType, string strID, string strFileType)
        {
            string strQuery = "", strResult = "", strStatus = "", strPDFFile = "";
            try
            {
                if (strFilePath != "")
                {
                    if (strFileType == "PDF")
                        strPDFFile = "&WhatsAppPDFUrl= " + strFilePath;
                    else if (strFileType == "IMAGE")
                        strPDFFile = "&WhatsAppImageUrl= " + strFilePath;
                    //else if (strFileType == "IMAGE")
                    //    strPDFFile = "&WhatsAppImageUrl= " + strFilePath;
                }
                strMessage = strMessage.Replace(" & ", " AND ").Replace("&", " AND ");

                if (strMessage != "" )
                    strMessage = "&WhatsAppMsg=" + strMessage;

                string strURL = "http://saraogisupersaleswhatsappinteg.appspot.com/SendWhatsAppMessage?WhatsAppTo=91" + strMobileNo + strMessage + strPDFFile;

                strResult = Apicall(strURL);

                if (strResult.Contains("success"))
                    strStatus = "SENT";
                else
                    strStatus = "FAILED";

                strMessage = strMessage.Replace("&WhatsAppMsg=", "");
                if (strID == "")
                {
                    strQuery = "INSERT INTO [dbo].[WhatsAppDetails] ([RemoteID],[Date],[WhatsappNo],[MessageBody],[FilePath],[Status],[BillType],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                            + " (0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + strMobileNo + ",'" + strMessage + "','" + strFilePath + "','" + strStatus + "','" + strBillType + "','" + MainPage.strLoginName + "','',1,0) ";
                            
                }
                else
                {
                    strQuery = " UPDATE [dbo].[WhatsAppDetails] SET [Status]='" + strStatus + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 WHERE ID=" + strID + " and [WhatsappNo]='" + strMobileNo + "' ";
                }

                DataBaseAccess.ExecuteMyNonQuery(strQuery);
            }
            catch { }
            return strResult;
        }
      
        public static string Apicall(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                    webClient.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("saraogisupersales:dc8d61e2c3da46ee9dd13ec9205c64cd"));
                    var response = webClient.DownloadString(url);

                    response = GetSMSStatus(response);
                    return response;
                }
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

        public static string SendWhatsAppMessage(string strMobileNo, string strMessage, string strFilePath, string strBillType, string strID, string strFileType)
        {
            string strQuery = "", strResult = "", strStatus = "",strType= "text";
            try
            {
                if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
                {
                    strMessage = strMessage.Trim();
                    if (strFilePath != "")
                    {
                        if (strFileType == "PDF")
                            strType = "document";
                        else if (strFileType == "IMAGE")
                            strType = "image";
                        else if (strFileType == "VIDEO")
                            strType = "video";
                        else
                            strType = "text";

                        if (strFilePath != "")
                            strFilePath = "&url=" + strFilePath;
                    }
                    else if (strMessage == "")
                        return "";

                    strMessage = strMessage.Replace(" & ", " AND ").Replace("&", " AND ");
                    if (strType == "text" && strMessage == "")
                        return "";

                    string strURL = "http://wxportal.in/hook_url/whatsapp.php?&ui=89&tk=8JGBaE5zV3&ty=" + strType + "&cd=+91&mo=" + strMobileNo + strFilePath + "&ms=" + strMessage;
                    strResult = APIcall_New(strURL);

                    if (strResult.Contains("success"))
                        strStatus = "SENT";
                    else
                        strStatus = "FAILED";

                    strFilePath = strFilePath.Replace("&url=", "");
                    if (strID == "")
                    {
                        strQuery = "INSERT INTO [dbo].[WhatsAppDetails] ([RemoteID],[Date],[WhatsappNo],[MessageBody],[FilePath],[Status],[BillType],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                + " (0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + strMobileNo + ",'" + strMessage + "','" + strFilePath + "','" + strStatus + "','" + strBillType + "','" + MainPage.strLoginName + "','',1,0) ";
                    }
                    else
                    {
                        strQuery = " UPDATE [dbo].[WhatsAppDetails] SET [Status]='" + strStatus + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 WHERE ID=" + strID + " and [WhatsappNo]='" + strMobileNo + "' ";
                    }
                    DataBaseAccess.ExecuteMyNonQuery(strQuery);
                }
               
            }
            catch { }
            return strResult;
        }

        public static string APIcall_New(string url)
        {
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                results = GetSMSStatus(results);
                return results;
            }
            catch
            {
                return "";
            }
        }

        private static string GetSMSStatus(string strResult)
        {
            if (strResult.ToUpper().Contains("SUCCESS") || strResult.Contains("-200"))
                return "success";
            else
                return "";
        }

        public static string ApicallForTCI()
        {
            try
            {
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri("http://124.7.209.84/TciServices/ServiceEnquire.asmx?op=getConsignmentResponseMessage"));
                http.ContentType = "text/xml";
                http.Method = "POST";

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(getXMLData("400252823"));

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                WebResponse response = http.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string content = sr.ReadToEnd();
                var stringReader = new StringReader(content);
                var dsSet = new DataSet();
                dsSet.ReadXml(stringReader);

                return content;
            }
            catch(Exception ex) { return ex.Message; }
           
        }

        static string getXMLData(string cnsNo)
        {
            string xmlData = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <getConsignmentResponseMessage xmlns=""http://www.tciexpress.in/"">
                      <pConsignmentNumber>" + cnsNo + @"</pConsignmentNumber>
                      <pUserProfile>
                        <UserID>SARAOGI</UserID>
                        <Password>SSSPL@123</Password>
                      </pUserProfile>
                    </getConsignmentResponseMessage>
                  </soap:Body>
                </soap:Envelope>";
            return xmlData;
        }
        
        public static string ApicallForTCI_()
        {
            try
            {
                string url = "http://124.7.209.84/TciServices/ServiceEnquire.asmx?op=getConsignmentResponseMessage";
                //using (var webClient = new WebClient())
                //{
                //    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                //    webClient.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("SARAOGI:SSSPL@123"));
                //    webClient.QueryString.Add("pConsignmentNumber", "4000080825");

                //    var data = webClient.UploadValues(url, "GET", webClient.QueryString);

                //    // data here is optional, in case we recieve any string data back from the POST request.
                //    var responseString = UnicodeEncoding.UTF8.GetString(data);

                //    var response = webClient.DownloadString(url);
                //    response = GetSMSStatus(response);
                //    return response;
                //}

                System.Net.WebRequest req = null;
                System.Net.WebResponse rsp = null;
                try
                {
                    string strURL = "http://124.7.209.84/TciServices/ServiceEnquire.asmx?op=getConsignmentResponseMessage";
                    req = System.Net.WebRequest.Create(strURL);
                    req.Method = "POST";
                    req.ContentType = "text/xml";
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(req.GetRequestStream());
                    writer.WriteLine(GetXMlData("4000080825"));
                    writer.Close();
                    rsp = req.GetResponse();
                    System.IO.StreamReader respStream = new System.IO.StreamReader(rsp.GetResponseStream(), System.Text.Encoding.Default);                 
                    return respStream.ReadToEnd();
                }
                catch  {throw;}
                finally
                {
                    if (req != null) req.GetRequestStream().Close();
                    if (rsp != null) rsp.GetResponseStream().Close();
                } 
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }           
        }
        public static string GetXMlData(string strCNo)
        {
            string strData = "";
            strData = " <?xml version='1.0' encoding='utf-8'?> "
                    + " <soap:Envelope xmlns:xsi = 'http://www.w3.org/2001/XMLSchema-instance' xmlns: xsd = 'http://www.w3.org/2001/XMLSchema' xmlns: soap = 'http://schemas.xmlsoap.org/soap/envelope/' > "
                    + " <soap:Body> "
                    + " <getConsignmentResponseMessage xmlns = 'http://www.tciexpress.in/'> "
                    + " <pConsignmentNumber>"+ strCNo +"</pConsignmentNumber>"
                    + " <pUserProfile> "
                    + " <UserID>SARAOGI</UserID> "
                    + " <Password>SSSPL@123</Password> "
                    + " </pUserProfile>"
                    + " </getConsignmentResponseMessage>"
                    + " </soap:Body></soap:Envelope> ";
            return strData;
        }

        public static string SendWhatsappWithIMIMobile(string strMobileNo, string strMsgType, string strMessage, string strID,string strFileName)
        {
            string strQuery = "",strStatus="";
            try
            {
                if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                {
                    string _str = "";
                    if (strFileName != "")
                    {
                        var __vStatus = ApicallWithApprovedWithDocument(strMobileNo, strMsgType, strMessage, strFileName);
                        _str = __vStatus.Status.ToString();
                    }
                    else
                    {
                        var _vStatus = ApicallWithApproved(strMobileNo, strMsgType, strMessage);
                        _str = _vStatus.Status.ToString();
                    }
                    if (_str.ToLower().Contains("success") || _str.Contains("WaitingForActivation"))
                        strStatus = "SENT";
                    else
                        strStatus = "FAILED";

                    if (strID == "")
                    {
                        strQuery = "INSERT INTO [dbo].[WhatsAppDetails] ([RemoteID],[Date],[WhatsappNo],[MessageBody],[FilePath],[Status],[BillType],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES  "
                                + " (0,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + strMobileNo + ",'" + strMessage + "','" + strFileName + "','" + strStatus + "','" + strMsgType + "','" + MainPage.strLoginName + "','',1,0) ";
                    }
                    else
                    {
                        strQuery = " UPDATE [dbo].[WhatsAppDetails] SET [Status]='" + strStatus + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 WHERE ID=" + strID + " and [WhatsappNo]='" + strMobileNo + "' ";
                    }
                    DataBaseAccess.ExecuteMyNonQuery(strQuery);
                }
            }
            catch { }
            return strStatus;
        }

        public static async Task<string> ApicallWithApproved(string strMobileNo, string strMsgType, string strMessage)
        {
            try
            {
                string strLanguage = "en";
                if (strMsgType.Contains("hindi"))
                    strLanguage = "hi";
                using (var httpClient = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.imiconnect.in/resources/v1/messaging"))
                    {
                        request.Headers.TryAddWithoutValidation("key", "7de45811-6376-11ea-9da9-025282c394f2");
                        string strJSON = "{\"appid\": \"a_158388955372007230\",\"deliverychannel\": \"whatsapp\",\"channels\": {\"OTT-Messaging\": {\"wa\": {\"type\": \"hsm\",\"hsm\": {\"namespace\": \"e840a4b1_5cfc_432a_8fa0_32a7ec91d30a\",\"element_name\": \"" + strMsgType + "\",\"language\": {\"code\": \"" + strLanguage + "\",\"policy\": \"deterministic\"},\"localizable_params\": [" + strMessage + "]}}}},\"destination\": [{\"waid\": [91" + strMobileNo + "]}]}";
                        request.Content = new StringContent(strJSON);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            return "success";
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            return "";
        }

        public static async Task<string> ApicallWithApprovedWithDocument(string strMobileNo,string strMsgType, string strMessage, string strFileUrl)
        {
            try
            {
                string strTemplateID = "", strFileName="";
                if (strMsgType == "sale_bill")
                    strTemplateID = "1493671724133732";
                else if (strMsgType == "sale_bill_update")
                    strTemplateID = "1087501198310953";
                else if (strMsgType == "bilty_update")
                    strTemplateID = "223408828747881";
                else if (strMsgType == "sale_service")
                    strTemplateID = "561573154460382";
                else if (strMsgType == "sale_service_update")
                    strTemplateID = "538633776857372";
                else if (strMsgType == "debit_note")
                    strTemplateID = "608032013129759";
                else if (strMsgType == "debit_note_update_pdf")
                    strTemplateID = "1308793879508486";
                else if (strMsgType == "credit_note")
                    strTemplateID = "173117290326355";
                else if (strMsgType == "credit_note_update_pdf")
                    strTemplateID = "495916861290392";
                else if (strMsgType == "ledger_pdf")
                    strTemplateID = "509731093038323";
                else if (strMsgType == "interest_pdf")
                    strTemplateID = "525503951695942";
                else if (strMsgType == "orderform")
                    strTemplateID = "289450918925346";
                else if (strMsgType == "packing_pdf")
                    strTemplateID = "559906841318778";
                else if (strMsgType == "order_image_new")
                    strTemplateID = "319238485777187";
                else if (strMsgType == "tcsnote_pdf")
                    strTemplateID = "982405215571728";

                if (strTemplateID!=""   && strFileUrl!="")
                {
                    Uri uri = new Uri(strFileUrl);
                    strFileName = System.IO.Path.GetFileName(uri.LocalPath);

                    using (var httpClient = new HttpClient())
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.imiconnect.in/resources/v1/messaging"))
                        {
                            request.Headers.TryAddWithoutValidation("key", "7de45811-6376-11ea-9da9-025282c394f2");
                      
                            string strJSON = "{\"appid\": \"a_158388955372007230\",\"deliverychannel\": \"whatsapp\",\"message\": {\"template\": \""+ strTemplateID+"\",\"parameters\": {"+ strMessage+"\"document\": {\"link\": \"" + strFileUrl + "\",\"filename\": \"" + strFileName + "\"}}},\"destination\": [{\"waid\": [\"91"+strMobileNo+"\"]}]} ";
                            request.Content = new StringContent(strJSON);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var response = await httpClient.SendAsync(request);
                            if (response.IsSuccessStatusCode)
                            {
                                return "success";
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            return "";
        }

        //public static string GetShortURL(string strURL)
        //{
        //    string strShortURL = "";
        //    System.Net.WebRequest req = null;
        //    System.Net.WebResponse rsp = null;
        //    try
        //    {
        //        string strJSON = "{\"url\": \"" + strURL + "\"}";

        //        req = System.Net.WebRequest.Create("https://rel.ink/api/links/");
        //        req.Method = "POST";
        //        req.ContentType = "application/json";

        //        System.IO.StreamWriter writer = new System.IO.StreamWriter(req.GetRequestStream());
        //        writer.WriteLine(strJSON);
        //        writer.Close();
        //        rsp = req.GetResponse();
        //        System.IO.StreamReader respStream = new System.IO.StreamReader(rsp.GetResponseStream(), System.Text.Encoding.Default);
        //        string strResult = respStream.ReadToEnd();

        //        strResult = "[" + strResult + "]";

        //        DataTable dtValue = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult, (typeof(DataTable)));
        //        if (dtValue.Rows.Count > 0)
        //        {
        //            DataRow row = dtValue.Rows[0];
        //            string strCode = Convert.ToString(row["hashid"]);
        //            if (strCode != "")
        //            {
        //                strShortURL = "https://rel.ink/" + strCode;
        //                return strShortURL;
        //            }
        //        }
        //    }
        //    catch { throw; }
        //    finally
        //    {
        //        if (req != null) req.GetRequestStream().Close();
        //        if (rsp != null) rsp.GetResponseStream().Close();
        //    }
        //    return strURL;
        //}

    }

   
}
