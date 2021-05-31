using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace SSS
{
    class SendSMS
    {

        public string SendSingleSMS(string strMessage, string strMobileNo)
        {
            string strQuery = "", strResult="";
            //if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
            {
                string strID = MainPage.strSenderID, strMSGType = MainPage.strMessageType;
                if (MainPage.strSMSURL != "" && MainPage.strSenderID != "")
                {
                    strMessage = strMessage.Replace("&", "%26");                  
                    if (strMSGType != "")
                        strMSGType = "&" + strMSGType;

                    string strUrl = MainPage.strSMSURL.Replace("[USERNAME]", MainPage.strSMSUser).Replace("[PASSWORD]", MainPage.strSMSPassword)+"&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + MainPage.strSenderID + strMSGType;
                    strResult = apicall(strUrl, true);
                }
                if (strResult.Contains("success"))
                {
                    strQuery = "Insert into SMSReport values('" + strID + "'," + strMobileNo + ",'" + strMessage + "','SENT',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',0,1,0)";
                }
                else
                {
                    strQuery = "Insert into SMSReport values('" + strID + "'," + strMobileNo + ",'" + strMessage + "','FAILED',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',0,1,0)";
                }

                DataBaseAccess.ExecuteMyNonQuery(strQuery);
            }
            return strResult;
        }

        public string SendSingleSMSWithoutSaveINDB(string strMessage, string strMobileNo)
        {
            string  strResult = "";
            //if (MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO"))
            {
                strMessage = strMessage.Replace("&", "%26");
                string strID = MainPage.strSenderID, strMSGType = MainPage.strMessageType;
                //if (MainPage.strSMSURL != "" && MainPage.strSenderID != "")
                {
                    strMessage = strMessage.Replace("&", "%26");
                    if (strMSGType != "")
                        strMSGType = "&" + strMSGType;

                    string strUrl = "http://mobicomm.dove-sms.com/mobicomm//submitsms.jsp?user=SSSWEB&key=ee8e045d46XX&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + MainPage.strSenderID + "&accusage=1";
                    strResult = apicall(strUrl, true);
                }                             
            }
            return strResult;
        }

        public string SendSingleSMSWithUnicode(string strMessage, string strMobileNo)
        {
            string strQuery = "", strResult="";
            //if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
            {
                string strID = MainPage.strSenderID, strMSGType = MainPage.strMessageType;
                if (MainPage.strSMSURL != "" && MainPage.strSenderID != "")
                {
                    strMessage = strMessage.Replace("&", "%26");
                    if (strMSGType != "")
                        strMSGType = "&" + strMSGType;

                    string strUrl = MainPage.strSMSURL.Replace("[USERNAME]", MainPage.strSMSUser).Replace("[PASSWORD]", MainPage.strSMSPassword) + "&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + MainPage.strSenderID + strMSGType+ "&unicode=1";
                    strResult = apicall(strUrl, true);
                }


                if (strResult.Contains("success"))
                {
                    strQuery = "Insert into SMSReport values('" + strID + "'," + strMobileNo + ",'" + strMessage + "','SENT',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',0,1,0)";
                }
                else
                {
                    strQuery = "Insert into SMSReport values('" + strID + "'," + strMobileNo + ",'" + strMessage + "','FAILED',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','',0,1,0)";
                }

                DataBaseAccess.ExecuteMyNonQuery(strQuery);
            }
            return strResult;
        }

        public string SendSingleSMS(string strMessage, string strMobileNo , string strSMSID)
        {
            string strQuery = "", strResult="";
            strMessage = strMessage.Replace("&", "%26");           

            string strID = MainPage.strSenderID, strMSGType = MainPage.strMessageType;
            if (MainPage.strSMSURL != "" && MainPage.strSenderID != "")
            {
                strMessage = strMessage.Replace("&", "%26");
                if (strMSGType != "")
                    strMSGType = "&" + strMSGType;

                string strUrl = MainPage.strSMSURL.Replace("[USERNAME]", MainPage.strSMSUser).Replace("[PASSWORD]", MainPage.strSMSPassword) + "&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + MainPage.strSenderID + strMSGType + "&unicode=1";
                strResult = apicall(strUrl, true);
            }

            if (strResult.Contains("success"))
            {
                strQuery = "Update SMSReport Set Status='SENT',UpdatedBy='" + MainPage.strLoginName + "',UpdateStatus=1 Where ID=" + strSMSID + " ";
            }
            else
            {
                strQuery = "Update  SMSReport Set Status='FAILED',UpdatedBy='" + MainPage.strLoginName + "',UpdateStatus=1 Where ID=" + strSMSID + " ";
            }
            DataBaseAccess.ExecuteMyNonQuery(strQuery);
            return strResult;
        }
               

        public string apicall(string url, bool bStatus)
        {
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                if (bStatus)
                    results = GetSMSStatus(results);
                return results;
            }
            catch
            {
                return "";
            }
        }

        private string GetSMSStatus(string strResult)
        {
            if (strResult.Contains("success"))
                return "success";
            else
                return "";
        }

        public string GetSMSBalance()
        {
            string strResult = "";
            try
            {
                string strUrl = "http://mobicomm.dove-sms.com/mobicomm//getbalance.jsp?user=SSSWEB&key=ee8e045d46XX&accusage=1";
                strResult = apicall(strUrl,true);
                strResult=strResult.Replace("trans2:", "");
            }
            catch
            {
                strResult = "";
            }
            return strResult;
        }


  
    }

}
