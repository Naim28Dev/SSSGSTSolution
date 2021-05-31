using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using RestSharp;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace SSS
{
    public class BankAPI
    {
        public static void Register()
        {
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var restClient = new RestClient("https://api.icicibank.com:8443/api/Corporate/CIB/v1/Registration");           
            var request = new RestRequest(Method.POST);
            //request.AddHeader("x-forwarded-for", "163.47.141.138");
            request.AddHeader("host", "api.icicibank.com:8443");
            request.AddHeader("apikey", "5e9aabe6bc4d4695a95eaef9e9aac9d3");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"563421034\",\"USERID\":\"RAJESHSA\",\"URN\":\"SSS\",\"AGGRNAME\":\"SARAOGI\",\"ALIASID\":\"\"}";

            data = EncryptUsingCertificate(data);
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);
            string strResult = DecryptUsingCertificate(result.Content);
           // Console.Write(result.Content);            
        }

        public static void BalanceEnquiry(string strAccountNo)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var restClient = new RestClient("https://api.icicibank.com:8443/api/Corporate/CIB/v1/BalanceInquiry");

            var request = new RestRequest(Method.POST);
            //request.AddHeader("x-forwarded-for", "163.47.141.138");
            request.AddHeader("host", "api.icicibank.com:8443");
            request.AddHeader("apikey", "5e9aabe6bc4d4695a95eaef9e9aac9d3");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"SARAOGIS23112017\",\"USERID\":\"RAJESHSA\",\"URN\":\"ChandraKant\",\"ACCOUNTNO\":\""+ strAccountNo+"\"}";

            data = EncryptUsingCertificate(data);
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);
            string strResult = DecryptUsingCertificate(result.Content);
        }

        public static string TransactionAPI(string strAccountName, string strBankAccountNo,string strIFSCCode,string strTransactionType,double dAmt,string strUID,string strRemark)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var restClient = new RestClient("https://api.icicibank.com:8443/api/Corporate/CIB/v1/Transaction");

            var request = new RestRequest(Method.POST);
            //request.AddHeader("x-forwarded-for", "163.47.141.138");
            request.AddHeader("host", "api.icicibank.com:8443");
            request.AddHeader("apikey", "5e9aabe6bc4d4695a95eaef9e9aac9d3");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"SARAOGIS23112017\",\"USERID\":\"VINITASA\",\"URN\":\"ChandraKant\",\"DEBITACC\" :\"777705000285\",\"CREDITACC\" : \"" + strBankAccountNo+ "\",\"IFSC\" : \""+strIFSCCode+"\",\"AMOUNT\" : \""+dAmt.ToString("0.00")+"\",\"CURRENCY\" : \"INR\",\"TXNTYPE\" : \""+strTransactionType+"\",\"PAYEENAME\" : \""+ strAccountName+"\",\"UNIQUEID\" : \"" + strUID+ "\",\"REMARKS\" : \""+strRemark+"\",\"AGGRNAME\":\"SARAOGI\"}";

            data = EncryptUsingCertificate(data);
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);
            string strResult = DecryptUsingCertificate(result.Content);
            return strResult;
        }

        public static void TransactionAPI_SameAccount(string strBankAccountNo, string strUID)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var restClient = new RestClient("https://api.icicibank.com:8443/api/Corporate/CIB/v1/Transaction");

            var request = new RestRequest(Method.POST);           
            request.AddHeader("host", "api.icicibank.com:8443");
            request.AddHeader("apikey", "5e9aabe6bc4d4695a95eaef9e9aac9d3");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"SARAOGIS23112017\",\"USERID\":\"RAJESHSA\",\"URN\":\"ChandraKant\",\"DEBITACC\" :\""+ strBankAccountNo+ "\",\"CREDITACC\" : \"777705000285\",\"IFSC\" : \"ICIC0000011\",\"AMOUNT\" : \"1000.00\",\"CURRENCY\" : \"INR\",\"TXNTYPE\" : \"TPA\",\"PAYEENAME\" : \"SARAOGI SUPER SALES PVT LTD\",\"UNIQUEID\" : \"" + strUID + "\",\"REMARKS\" : \"TRF\",\"AGGRNAME\":\"SARAOGI\"}";

            data = EncryptUsingCertificate(data);
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);
           string strResult = DecryptUsingCertificate(result.Content);
        }

        public static DataTable GetAccountStatement(string strAccountNo,string strFromDate,string strToDate)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var restClient = new RestClient("https://api.icicibank.com:8443/api/Corporate/CIB/v1/AccountStatement"); //RegistrationStatus Transaction BalanceInquiry
            var request = new RestRequest(Method.POST);
            //request.AddHeader("x-forwarded-for", "163.47.141.138");
            request.AddHeader("host", "api.icicibank.com:8443");
            request.AddHeader("apikey", "5e9aabe6bc4d4695a95eaef9e9aac9d3");
            request.AddHeader("Content-Length", "684");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "text/plain");
            //777705000285
            string data = "{\"CORPID\":\"SARAOGIS23112017\",\"USERID\":\"RAJESHSA\",\"ACCOUNTNO\":\""+ strAccountNo+"\",\"FROMDATE\":\""+ strFromDate+"\",\"TODATE\":\""+ strToDate+"\",\"URN\":\"ChandraKant\",\"AGGRID\":\"CUST0149\"}";

            data = EncryptUsingCertificate(data);
            request.AddParameter("text/plain", data, ParameterType.RequestBody);
            IRestResponse result = restClient.Execute(request);
           
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var obj = serializer.Deserialize<Dictionary<string, object>>(result.Content);
            string str = Convert.ToString(obj["encryptedKey"]);
            string strEncryptedData = Convert.ToString(obj["encryptedData"]);
            byte[] byteData = Convert.FromBase64String(strEncryptedData);
            string strResult = DecryptUsingCertificate(str);

            byte[] _data = System.Convert.FromBase64String(strEncryptedData);
            string _strEnData = System.Text.ASCIIEncoding.ASCII.GetString(_data);
            string _strIV = _strEnData.Substring(0, 16);
            strResult = AESDecrypt(strEncryptedData, strResult, _strIV);
            int _startIndex = strResult.IndexOf("[") + 1, lastIndex = 0;
            DataTable dtValue = new DataTable();
            if (_startIndex > 0)
            {
                lastIndex = strResult.IndexOf("]", strResult.Length - 70);

                strResult = strResult.Substring(_startIndex, (lastIndex - _startIndex));
                strResult = "[" + strResult + "]";

                dtValue = (DataTable)JsonConvert.DeserializeObject(strResult, (typeof(DataTable)));
            }
                        
            return dtValue;
        }

        public DataTable GetStatementDataTablesString(string strJSON)
        {
            DataTable dtValue = (DataTable)JsonConvert.DeserializeObject(strJSON, (typeof(DataTable)));
            return dtValue;
        }

        //public static string GetAccountStatement()
        //{

        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    // registration var restClient = new RestClient("https://apigwuat.icicibank.com:8443/api/Corporate/CIB/v1/Registration");
        //    var restClient = new RestClient("https://apigwuat.icicibank.com:8443/api/Corporate/CIB/v1/AccountStatement"); //RegistrationStatus Transaction BalanceInquiry

        //    var request = new RestRequest(Method.POST);
        //    //request.AddHeader("x-forwarded-for", "163.47.141.138");
        //    request.AddHeader("host", "apigwuat.icicibank.com:8443");
        //    request.AddHeader("apikey", "8107ed0362054ed9afb2af079e19d78f");
        //    request.AddHeader("Content-Length", "684");
        //    request.AddHeader("Accept", "*/*");
        //    request.AddHeader("Content-Type", "text/plain");

        //    //Registration string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"PRACHICIB1\",\"USERID\":\"USER3\",\"URN\":\"ChandraKant\",\"AGGRNAME\":\"SARAOGI\",\"ALIASID\":\"\"}";
        //    //RegistrationStatus string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"PRACHICIB1\",\"USERID\":\"USER3\",\"AGGRNAME\":\"SARAOGI\",\"URN\":\"ChandraKant\"}";
        //    //Trasaction string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"PRACHICIB1\",\"USERID\":\"USER3\",\"URN\":\"ChandraKant\",\"AGGRNAME\":\"SARAOGI\",\"DEBITACC\" :\"000451000301\",\"CREDITACC\" : \"000405002777\",\"IFSC\" : \"ICIC0000011\",\"AMOUNT\" : \"1.00\",\"CURRENCY\" : \"INR\",\"TXNTYPE\" : \"TPA\",\"PAYEENAME\" :\"ChandraKant\",\"UNIQUEID\": \"123456\",\"REMARKS\" : \"TRF\"}";
        //    //Balance Enquiry string data = "{\"AGGRID\":\"CUST0149\",\"CORPID\":\"PRACHICIB1\",\"USERID\":\"USER3\",\"URN\":\"ChandraKant\",\"ACCOUNTNO\" :\"000451000301\"}";

        //    string data = "{\"CORPID\":\"CIBNEXT\",\"USERID\":\"CIBTESTING6\",\"ACCOUNTNO\":\"000405001257\",\"FROMDATE\":\"01-01-2016\",\"TODATE\":\"30-12-2016\",\"URN\":\"ChandraKant\",\"AGGRID\":\"CUST0149\"}";

        //    data = EncryptUsingCertificate(data);
        //    request.AddParameter("text/plain", data, ParameterType.RequestBody);
        //    IRestResponse result = restClient.Execute(request);
        //    //string strResult = DecryptUsingCertificate(result.Content);

        //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    var obj = serializer.Deserialize<Dictionary<string, object>>(result.Content);
        //    string str = Convert.ToString(obj["encryptedKey"]);
        //    string strEncryptedData = Convert.ToString(obj["encryptedData"]);
        //    byte[] byteData = Convert.FromBase64String(strEncryptedData);
        //    string strResult = DecryptUsingCertificate(str);

        //    byte[] _data = System.Convert.FromBase64String(strEncryptedData);
        //    string _strEnData = System.Text.ASCIIEncoding.ASCII.GetString(_data);

        //    string _strIV = _strEnData.Substring(0, 16);

        //    strResult = AESDecrypt(strEncryptedData, strResult, _strIV);

        //    return strResult;
        //}

        public static string AESDecrypt(string data, string pKey, string piv = "")
        {
            string plaintext = string.Empty;
            byte[] inputBytes = Convert.FromBase64String(data);
            byte[] Key = Encoding.UTF8.GetBytes(pKey);
            byte[] iv = string.IsNullOrEmpty(piv) ? null : Encoding.UTF8.GetBytes(piv);

            byte[] plainText = GetCryptoAlgorithm().CreateDecryptor(Key, (iv == null ? Key : iv)).TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            plaintext = ASCIIEncoding.UTF8.GetString(plainText, 16, plainText.Length - 16);
            return plaintext.Replace("\u000e", string.Empty).Replace("\u0002", string.Empty).Replace("\u0006", string.Empty);
        }

        private static RijndaelManaged GetCryptoAlgorithm()
        {
            RijndaelManaged algorithm = new RijndaelManaged();
            algorithm.Padding = PaddingMode.None;
            algorithm.Mode = CipherMode.CBC;
            algorithm.KeySize = 128;
            algorithm.BlockSize = 128;
            return algorithm;
        }

        public static string EncryptUsingCertificate(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            string path = MainPage.strServerPath+ @"\Signature\Key\IC_CERT.txt";
            var collection = new X509Certificate2Collection();
            collection.Import(path);
            var certificate = collection[0];
            var publicKey = certificate.PublicKey.Key as RSACryptoServiceProvider;
            return Convert.ToBase64String(publicKey.Encrypt(byteData, false));
        }

        public static string DecryptUsingCertificate(string data)
        {
            try
            {
                byte[] byteData = Convert.FromBase64String(data);
                string path = MainPage.strServerPath + @"\Signature\Key\Self_Signed.pfx";
                var Password = "1234";

                var collection = new X509Certificate2Collection();
                collection.Import(path, Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                X509Certificate2 certificate = new X509Certificate2();
                certificate = collection[0];
                foreach (var cert in collection)
                {
                    if (cert.FriendlyName == "icici.rkitsoftware.com")
                    {
                        certificate = cert;
                    }
                }

                if (certificate.HasPrivateKey)
                {
                    var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                    byte[] _byte = privateKey.Decrypt(Convert.FromBase64String(data), false);
                    return Encoding.UTF8.GetString(_byte);
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }

        
    }
}
