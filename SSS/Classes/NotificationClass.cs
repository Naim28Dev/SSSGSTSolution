using System.IO;
using System.Net;

namespace SSS
{
    class NotificationClass
    {
        public static bool SetNotification(string strType, string strPartyID, double dAmt, string strBillNo)
        {
            string strMessage = "", strTitle = "";
            try
            {
                if (strType == "PAYMENT")
                {
                    strTitle = "Payment paid";
                    strMessage = "Your payment paid with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (strType == "RECEIPT")
                {
                    strTitle = "Payment received";
                    strMessage = "Your payment received with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (strType == "SALES")
                {
                    strTitle = "Sale bill generated";
                    strMessage = "Your sale bill : " + strBillNo + " generated with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (strType == "SALESERVICE")
                {
                    strTitle = "Sale Service bill generated";
                    strMessage = "Your sale service bill : " + strBillNo + " generated with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (strType == "UPDATESALES")
                {
                    strTitle = "Sale bill updated";
                    strMessage = "Your sale bill : " + strBillNo + " updated with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }
                else if (strType == "PURCHASE")
                {
                    strTitle = "Purchase bill generated";
                    strMessage = "Your purchase bill : " + strBillNo + " generated with the amount of : " + dAmt.ToString("N2", MainPage.indianCurancy);
                }

                if (strTitle != "" && strMessage != "")
                {
                    if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
                    {
                        bool _bStatus = AppAPI.NotificationInApp(strPartyID, strTitle, strMessage);
                        return _bStatus;
                    }          
                }
            }
            catch { }
            return false;
        }

        public static string apicall(string url, bool bStatus)
        {   
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                    var response = webClient.DownloadString(url);
                    if (bStatus)
                        response = GetSMSStatus(response);
                    return response;
                }

            }
            catch(System.Exception ex)
            {
                return ex.Message;
            }
        }

        private static string GetSMSStatus(string strResult)
        {
            if (strResult.Contains("success") || strResult.Contains("-200"))
                return "success";
            else
                return "";
        }
    }
}
