using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace SSS
{
    class SendSMS_Test
    {

        public string SendSingleSMS(string strMessage, string strMobileNo)
        {
            string strQuery = "", strResult="";
            if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
            {
                strMessage = strMessage.Replace("&", "%26");
                string strID = MainPage.strSenderID;
                string strUrl = ""+ MainPage.strSMSURL +"user="+ MainPage.strSMSUser + "&key="+ MainPage.strSMSPassword +"&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + strID + "&accusage=" + MainPage.strMessageType +"";
                strResult = apicall(strUrl, true);

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
                string strSMSURL = MainPage.strSMSURL;
                string strID = MainPage.strSenderID;
                string strUrl = ""+ strSMSURL + strMobileNo + "&message=" + strMessage + "&senderid=" + strID + "&accusage=6 ";
                strResult = apicall(strUrl, true);                
            }
            return strResult;
        }

        public string SendSingleSMSWithUnicode(string strMessage, string strMobileNo)
        {
            string strQuery = "", strResult="";
            if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
            {
                string strID = MainPage.strSenderID;
                strMessage = strMessage.Replace("&", "%26");
                string strUrl = "" + MainPage.strSMSURL + "user=" + MainPage.strSMSUser + "&key=" + MainPage.strSMSPassword + "&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + strID + "&accusage=" + MainPage.strMessageType + "&unicode=1";

                strResult = apicall(strUrl, true);

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
            string strQuery = "";
            strMessage = strMessage.Replace("&", "%26");
            string strSenderID = MainPage.strSenderID;

            string strUrl = "" + MainPage.strSMSURL + "user=" + MainPage.strSMSUser + "&key=" + MainPage.strSMSPassword + "&mobile=" + strMobileNo + "&message=" + strMessage + "&senderid=" + strSenderID + "&accusage=" + MainPage.strMessageType + "";
            string strResult = apicall(strUrl,true);

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
            catch(Exception ex)
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
                string strUrl = "" + MainPage.strSMSURL + "user=" + MainPage.strSMSUser + "&key=" + MainPage.strSMSPassword + "&accusage=" + MainPage.strMessageType + "";
                
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

    //public class GifImage
    //{
    //    private Image gifImage;
    //    private FrameDimension dimension;
    //    private int frameCount;
    //    private int currentFrame = -1;
    //    private bool reverse;
    //    private int step = 1;

    //    public GifImage(string path)
    //    {
    //        gifImage = Image.FromFile(path);
    //        //initialize
    //        dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
    //        //gets the GUID
    //        //total frames in the animation
    //        frameCount = gifImage.GetFrameCount(dimension);
    //    }

    //    public bool ReverseAtEnd
    //    {
    //        //whether the gif should play backwards when it reaches the end
    //        get { return reverse; }
    //        set { reverse = value; }
    //    }

    //    public Image GetNextFrame()
    //    {

    //        currentFrame += step;

    //        //if the animation reaches a boundary...
    //        if (currentFrame >= frameCount || currentFrame < 1)
    //        {
    //            if (reverse)
    //            {
    //                step *= -1;
    //                //...reverse the count
    //                //apply it
    //                currentFrame += step;
    //            }
    //            else
    //            {
    //                currentFrame = 0;
    //                //...or start over
    //            }
    //        }
    //        return GetFrame(currentFrame);
    //    }

    //    public Image GetFrame(int index)
    //    {
    //        gifImage.SelectActiveFrame(dimension, index);
    //        //find the frame
    //        return (Image)gifImage.Clone();
    //        //return a copy of it
    //    }
    //}

}
