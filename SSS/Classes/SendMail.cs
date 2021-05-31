using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SSS
{
    class SendMail
    {
        public static async Task<bool> SendEmail(string strReceiver, string strSubject, string strMsgBody, string strAttached, string strID, string strEmailType, bool _bErrorMessage,string strSupplierEmailID="")
        {
            try
            {
                if (MainPage.strSenderEmailID != "" && MainPage.strSenderPassword != "")
                {
                    if (strReceiver == "" && strSupplierEmailID != "")
                    {
                        strReceiver = strSupplierEmailID;
                        strSupplierEmailID = "";
                    }

                    System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress(MainPage.strSenderEmailID);
                    System.Net.Mail.MailAddress toAddress = new System.Net.Mail.MailAddress(strReceiver);

                    System.Net.Mail.MailMessage objMessage = new System.Net.Mail.MailMessage(fromAddress, toAddress);

                    if (strReceiver != "" && strSupplierEmailID != "")
                        objMessage.To.Add(new System.Net.Mail.MailAddress(strSupplierEmailID));

                    objMessage.Subject = strSubject;
                    objMessage.Body = strMsgBody;
                    if (strAttached != "")
                    {
                        string[] strFiles = strAttached.Split(',');
                        foreach (string strFileName in strFiles)
                        {
                            if (strFileName != "")
                            {
                                try
                                {
                                    System.Net.Mail.Attachment objAttached = new System.Net.Mail.Attachment(strFileName);
                                    objMessage.Attachments.Add(objAttached);
                                }
                                catch { }
                            }
                        }
                    }
                    objMessage.IsBodyHtml = true;

                    System.Net.NetworkCredential authentication = new System.Net.NetworkCredential(MainPage.strSenderEmailID, MainPage.strSenderPassword);
                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(MainPage.strSMTPServer, MainPage._SMTPPORTNo);
                    client.UseDefaultCredentials = false;
                    if (MainPage.strSMTPServer.ToUpper().Contains("GMAIL"))
                        client.EnableSsl = true;
                    else
                        client.EnableSsl = false;
                    client.Credentials = authentication;
                    client.SendAsync(objMessage, "Success");
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (_bErrorMessage)
                    MessageBox.Show("Sending Failed : " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        public static bool SendEmailToCompany(string strReceiver, string strSubject, string strMsgBody, string strAttached, string strID, string strEmailType, bool _bErrorMessage)
        {
            try
            {
                // if (MainPage.strSenderEmailID != "" && MainPage.strSenderPassword != "")
                {

                    System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress("noreply@ssssybertech.com");
                    System.Net.Mail.MailAddress toAddress = new System.Net.Mail.MailAddress(strReceiver);
                    System.Net.Mail.MailMessage objMessage = new System.Net.Mail.MailMessage(fromAddress, toAddress);

                    objMessage.To.Add(new System.Net.Mail.MailAddress("priyam@ssssybertech.com"));

                    objMessage.Subject = strSubject;
                    objMessage.Body = strMsgBody;
                    if (strAttached != "")
                    {
                        string[] strFiles = strAttached.Split(',');
                        foreach (string strFileName in strFiles)
                        {
                            if (strFileName != "")
                            {
                                try
                                {
                                    System.Net.Mail.Attachment objAttached = new System.Net.Mail.Attachment(strFileName);
                                    objMessage.Attachments.Add(objAttached);
                                }
                                catch { }
                            }
                        }
                    }
                    objMessage.IsBodyHtml = true;

                    System.Net.NetworkCredential authentication = new System.Net.NetworkCredential("noreply@ssssybertech.com", "Sssemail111@");
                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.ssssybertech.com", 25);
                    client.UseDefaultCredentials = false;
                    if (MainPage.strSMTPServer.ToUpper().Contains("GMAIL"))
                        client.EnableSsl = true;
                    else
                        client.EnableSsl = false;
                    client.Credentials = authentication;
                    client.Send(objMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (_bErrorMessage)
                    MessageBox.Show("Sending Failed : " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }
    }
}
